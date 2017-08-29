using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using nom.tam.fits;
using Tangra.Model.Config;

namespace Tangra.Video.FITS
{
    public partial class frmChooseTimeHeaders : Form
    {
        internal FITSTimeStampReader TimeStampReader;
        private List<HeaderEntry> m_AllCards = new List<HeaderEntry>();
 
        private string m_FilesHash;
        private string m_CardNamesHash;

        public frmChooseTimeHeaders()
        {
            InitializeComponent();
        }

        public frmChooseTimeHeaders(Header hdr, string filesHash)
            : this()
        {
            m_FilesHash = filesHash;

            var hasher = new SHA1CryptoServiceProvider();
            hasher.Initialize();
            var orderedCardNames = new List<string>();

            var cursor = hdr.GetCursor();
            while (cursor.MoveNext())
            {
                var card = hdr.FindCard((string)cursor.Key);
                if (card != null)
                {
                    m_AllCards.Add(new HeaderEntry(card));                   
                }
                orderedCardNames.Add((string) cursor.Key);
            }

            orderedCardNames.Sort();
            byte[] combinedCardNamesBytes = Encoding.UTF8.GetBytes(string.Join("|", orderedCardNames));
            var hash = hasher.ComputeHash(combinedCardNamesBytes, 0, combinedCardNamesBytes.Length);
            m_CardNamesHash = Convert.ToBase64String(hash);

            cbxTimeStamp.Items.AddRange(m_AllCards.ToArray());
            cbxTimeStamp2.Items.AddRange(m_AllCards.ToArray());
            cbxExposure.Items.AddRange(m_AllCards.ToArray());

            cbxExposureUnits.Items.Clear();
            cbxExposureUnits.Items.AddRange(Enum.GetNames(typeof(TangraConfig.ExposureUnit)));

            cbxTimestampType.SelectedIndex = 0;
            cbxExposureUnits.SelectedIndex = 0;

            // End timestamp
            cbxTimestampType2.SelectedIndex = 2;
            cbxTimestampType2.Enabled = false;

            cbxTimeStampFormat.Items.Clear();
            cbxTimeStamp2Format.Items.Clear();
            var formats = new List<object>();
            formats.Add("yyyy-MM-ddTHH:mm:ss.fff");
            formats.Add("dd/MM/yyyy HH:mm:ss.fff");

            if (TangraConfig.Settings.Generic.CustomFITSTimeStampFormats != null)
                formats.AddRange(TangraConfig.Settings.Generic.CustomFITSTimeStampFormats);

            cbxTimeStampFormat.Items.AddRange(formats.ToArray());
            cbxTimeStamp2Format.Items.AddRange(formats.ToArray());
            if (!string.IsNullOrEmpty(TangraConfig.Settings.LastUsed.FitsTimestampFormat))
            {
                int idx = cbxTimeStampFormat.Items.IndexOf(TangraConfig.Settings.LastUsed.FitsTimestampFormat);
                if (idx > -1)
                {
                    cbxTimeStampFormat.SelectedIndex = idx;
                    cbxTimeStamp2Format.SelectedIndex = idx;
                }
            }
            else
            {
                cbxTimeStampFormat.SelectedIndex = 0;
                cbxTimeStamp2Format.SelectedIndex = 0;
            }

            TryIdentifyPreviousConfigApplyingForCurrentFiles();
        }

        public class HeaderEntry
        {
            public HeaderCard Card;

            public HeaderEntry(HeaderCard card)
            {
                Card = card;
            }

            public override string ToString()
            {
                return Card.Key;
            }
        }

        private void cbxTimeStamp_SelectedIndexChanged(object sender, EventArgs e)
        {
            var entry = cbxTimeStamp.SelectedItem as HeaderEntry;
            if (entry != null)
                tbxTimeStampValue.Text = entry.Card.ToString();
            else
                tbxTimeStampValue.Text = string.Empty;

            VerifyTimeStamp();
        }

        private void cbxExposure_SelectedIndexChanged(object sender, EventArgs e)
        {
            var entry = cbxExposure.SelectedItem as HeaderEntry;
            if (entry != null)
                tbxExposureValue.Text = entry.Card.ToString();
            else
                tbxExposureValue.Text = string.Empty;

            VerifyExposure();
        }

        private void cbxTimeStamp2_SelectedIndexChanged(object sender, EventArgs e)
        {
            var entry = cbxTimeStamp2.SelectedItem as HeaderEntry;
            if (entry != null)
                tbxTimeStamp2Value.Text = entry.Card.ToString();
            else
                tbxTimeStamp2Value.Text = string.Empty;

            VerifyTimeStamp2();
        }

        private bool m_TimeStampMappingValid = false;

        private bool VerifyTimeStamp(string value, string format)
        {
            try
            {
                var parsedTimestamp = DateTime.ParseExact(value, format, CultureInfo.InvariantCulture);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void VerifyTimeStamp()
        {
            m_TimeStampMappingValid = false;
            var entry = cbxTimeStamp.SelectedItem as HeaderEntry;
            if (entry != null)
            {
                string value = entry.Card.Value;
                string format = cbxTimeStampFormat.Text;
                m_TimeStampMappingValid = VerifyTimeStamp(value, format);
            }

            if (m_TimeStampMappingValid)
                pbxTimeStampOK.BringToFront();
            else
                pbxTimeStampWarning.BringToFront();
        }

        private bool m_TimeStamp2MappingValid = false;

        private void VerifyTimeStamp2()
        {
            m_TimeStamp2MappingValid = false;
            var entry = cbxTimeStamp2.SelectedItem as HeaderEntry;
            if (entry != null)
            {
                string value = entry.Card.Value;
                string format = cbxTimeStamp2Format.Text;
                m_TimeStamp2MappingValid = VerifyTimeStamp(value, format);
            }

            if (m_TimeStamp2MappingValid)
                pbxTimeStamp2OK.BringToFront();
            else
                pbxTimeStamp2Warning.BringToFront();
        }

        private bool m_ExposureValid = false;

        private bool VerifyExposure(string value)
        {
            try
            {
                var parsedValue = double.Parse(value, CultureInfo.InvariantCulture);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void VerifyExposure()
        {
            m_ExposureValid = false;
            var entry = cbxExposure.SelectedItem as HeaderEntry;
            if (entry != null)
            {
                string value = entry.Card.Value;
                m_ExposureValid = VerifyExposure(value);
            }

            if (m_ExposureValid)
                pbxExposureOK.BringToFront();
            else
                pbxExposureWarning.BringToFront();
        }

        void TryIdentifyPreviousConfigApplyingForCurrentFiles()
        {
            if (TangraConfig.Settings.RecentFITSFieldConfig.Items.Count > 0)
            {
                var sameHashConfig = TangraConfig.Settings.RecentFITSFieldConfig.Items.FirstOrDefault(x => x.FileHash == m_FilesHash);
                if (sameHashConfig != null && RecentFITSConfigMatches(sameHashConfig))
                {
                    UseRecentFITSConfig(sameHashConfig);
                    return;
                }

                foreach (var config in TangraConfig.Settings.RecentFITSFieldConfig.Items)
                {
                    if (RecentFITSConfigMatches(config))
                    {
                        UseRecentFITSConfig(config);
                        return;
                    }
                }
            }
        }

        private bool RecentFITSConfigMatches(TangraConfig.FITSFieldConfig config)
        {
            if (config.IsTimeStampAndExposure)
            {
                var exposureCard = m_AllCards.FirstOrDefault(x => x.Card.Key == config.ExposureHeader);
                var timestampCard = m_AllCards.FirstOrDefault(x => x.Card.Key == config.TimeStampHeader);

                if (exposureCard != null && timestampCard != null)
                {
                    return 
                        VerifyExposure(exposureCard.Card.Value) &&
                        VerifyTimeStamp(timestampCard.Card.Value, config.TimeStampFormat);
                }
                else
                    return false;
            }
            else
            {
                var startTimestampCard = m_AllCards.FirstOrDefault(x => x.Card.Key == config.TimeStampHeader);
                var endTimestampCard = m_AllCards.FirstOrDefault(x => x.Card.Key == config.TimeStamp2Header);

                if (startTimestampCard != null && endTimestampCard != null)
                {
                    return
                        VerifyTimeStamp(startTimestampCard.Card.Value, config.TimeStampFormat) &&
                        VerifyTimeStamp(endTimestampCard.Card.Value, config.TimeStamp2Format);
                }
                else
                    return false;
            }
        }

        private void UseRecentFITSConfig(TangraConfig.FITSFieldConfig config)
        {
            for (int i = 0; i < cbxTimeStamp.Items.Count; i++)
            {
                if (((HeaderEntry) cbxTimeStamp.Items[i]).Card.Key == config.TimeStampHeader)
                {
                    cbxTimeStamp.SelectedIndex = i;
                    break;
                }
            }

            if (config.ExposureHeader != null)
            {
                for (int i = 0; i < cbxExposure.Items.Count; i++)
                {
                    if (((HeaderEntry)cbxExposure.Items[i]).Card.Key == config.ExposureHeader)
                    {
                        cbxExposure.SelectedIndex = i;
                        break;
                    }
                }
            }

            if (config.TimeStamp2Header != null)
            {
                for (int i = 0; i < cbxTimeStamp2.Items.Count; i++)
                {
                    if (((HeaderEntry)cbxTimeStamp2.Items[i]).Card.Key == config.TimeStamp2Header)
                    {
                        cbxTimeStamp2.SelectedIndex = i;
                        break;
                    }
                }
            }

            if (config.TimeStampFormat != null)
                cbxTimeStampFormat.SelectedIndex = cbxTimeStampFormat.Items.IndexOf(config.TimeStampFormat);

            if (cbxTimeStampFormat.SelectedIndex == -1)
                cbxTimeStampFormat.Text = config.TimeStampFormat;

            if (config.TimeStamp2Format != null)
                cbxTimeStamp2Format.SelectedIndex = cbxTimeStamp2Format.Items.IndexOf(config.TimeStamp2Format);

            if (cbxTimeStamp2Format.SelectedIndex == -1)
                cbxTimeStamp2Format.Text = config.TimeStamp2Format;

            if (config.FileHash == m_FilesHash || m_CardNamesHash == config.CardNamesHash)
            {
                cbxExposureUnits.SelectedIndex = (int)config.ExposureUnit;
                cbxTimestampType.SelectedIndex = (int)config.TimeStampType;
            }
            else
            {
                cbxExposureUnits.SelectedIndex = -1;
                cbxTimestampType.SelectedIndex = -1;
            }

            rbStartEndTimestamp.Checked = !config.IsTimeStampAndExposure;
        }

        private void cbxTimeStampFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            VerifyTimeStamp();
        }

        private void cbxExposureUnits_SelectedIndexChanged(object sender, EventArgs e)
        {
            VerifyExposure();
        }

        private void cbxTimeStamp2Format_SelectedIndexChanged(object sender, EventArgs e)
        {
            VerifyTimeStamp2();
        }

        private void pnlEndTimeStamp_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (rbTimeDuration.Checked)
            {
                if (!m_TimeStampMappingValid)
                {
                    MessageBox.Show("Please choose valid settings for the time timestamp.");
                    return;
                }

                if (!m_ExposureValid)
                {
                    MessageBox.Show("Please choose valid settings for the exposure.");
                    return;
                }

                if (cbxExposureUnits.SelectedIndex == -1)
                {
                    MessageBox.Show("Please choose exposure units.");
                    cbxExposureUnits.Focus();
                    return;                    
                }

                if (cbxTimestampType.SelectedIndex == -1)
                {
                    MessageBox.Show("Please choose timestamp type.");
                    cbxTimestampType.Focus();
                    return;
                }

                var config = new TangraConfig.FITSFieldConfig()
                {
                    ExposureHeader = cbxExposure.Text,
                    ExposureUnit = (TangraConfig.ExposureUnit) cbxExposureUnits.SelectedIndex,
                    TimeStampType = (TangraConfig.TimeStampType) cbxTimestampType.SelectedIndex,
                    TimeStampHeader = cbxTimeStamp.Text,
                    TimeStampFormat = cbxTimeStampFormat.Text,
                    IsTimeStampAndExposure = true
                };

                TimeStampReader = new FITSTimeStampReader(config);

                config.FileHash = m_FilesHash;
                config.CardNamesHash = m_CardNamesHash;
                TangraConfig.Settings.RecentFITSFieldConfig.Register(config);
                TangraConfig.Settings.Save();

                DialogResult = DialogResult.OK;
                Close();
            }
            else if (rbStartEndTimestamp.Checked)
            {
                if (!m_TimeStampMappingValid)
                {
                    MessageBox.Show("Please choose valid settings for the start time timestamp.");
                    return;
                }

                if (!m_TimeStamp2MappingValid)
                {
                    MessageBox.Show("Please choose valid settings for the end time timestamp.");
                    return;
                }

                var config = new TangraConfig.FITSFieldConfig()
                {
                    TimeStampHeader = cbxTimeStamp.Text,
                    TimeStampFormat = cbxTimeStampFormat.Text,
                    TimeStamp2Header = cbxTimeStamp2.Text,
                    TimeStamp2Format = cbxTimeStamp2Format.Text,
                    IsTimeStampAndExposure = false
                };

                TimeStampReader = new FITSTimeStampReader(config);

                config.FileHash = m_FilesHash;
                config.CardNamesHash = m_CardNamesHash;
                TangraConfig.Settings.RecentFITSFieldConfig.Register(config);
                TangraConfig.Settings.Save();

                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void rbStartEndTimestamp_CheckedChanged(object sender, EventArgs e)
        {
            if (rbStartEndTimestamp.Checked)
            {
                pnlEndTimeStamp.BringToFront();
                pnlExposure.SendToBack();
                cbxTimestampType.SelectedIndex = (int)TangraConfig.TimeStampType.StartExposure;
                cbxTimestampType.Enabled = false;
            }
            else
            {
                pnlExposure.BringToFront();
                pnlEndTimeStamp.SendToBack();
                cbxTimestampType.Enabled = true;
            }
        }
    }
}
