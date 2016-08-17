using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using nom.tam.fits;
using Tangra.Model.Config;

namespace Tangra.Video.FITS
{
    public partial class frmChooseTimeHeaders : Form
    {
        internal FITSTimeStampReader TimeStampReader;

        public frmChooseTimeHeaders()
        {
            InitializeComponent();
        }

        public frmChooseTimeHeaders(Header hdr)
            : this()
        {
            var cursor = hdr.GetCursor();
            while (cursor.MoveNext())
            {
                var card = hdr.FindCard((string)cursor.Key);
                if (card != null)
                {
                    cbxTimeStamp.Items.Add(new HeaderEntry(card));
                    cbxTimeStamp2.Items.Add(new HeaderEntry(card));
                    cbxExposure.Items.Add(new HeaderEntry(card));
                }
            }

            cbxExposureUnits.Items.Clear();
            cbxExposureUnits.Items.AddRange(Enum.GetNames(typeof(ExposureUnit)));

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

        private void VerifyTimeStamp()
        {
            m_TimeStampMappingValid = false;
            var entry = cbxTimeStamp.SelectedItem as HeaderEntry;
            if (entry != null)
            {
                string value = entry.Card.Value;
                string format = cbxTimeStampFormat.Text;
                try
                {
                    var parsedTimestamp = DateTime.ParseExact(value, format, CultureInfo.InvariantCulture);
                    m_TimeStampMappingValid = true;
                }
                catch
                { }
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
                try
                {
                    var parsedTimestamp = DateTime.ParseExact(value, format, CultureInfo.InvariantCulture);
                    m_TimeStamp2MappingValid = true;
                }
                catch
                { }
            }

            if (m_TimeStamp2MappingValid)
                pbxTimeStamp2OK.BringToFront();
            else
                pbxTimeStamp2Warning.BringToFront();
        }

        private bool m_ExposureValid = false;

        private void VerifyExposure()
        {
            m_ExposureValid = false;
            var entry = cbxExposure.SelectedItem as HeaderEntry;
            if (entry != null)
            {
                string value = entry.Card.Value;
                try
                {
                    var parsedValue = double.Parse(value, CultureInfo.InvariantCulture);
                    m_ExposureValid = true;
                }
                catch
                { }
            }

            if (m_ExposureValid)
                pbxExposureOK.BringToFront();
            else
                pbxExposureWarning.BringToFront();
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

                TimeStampReader = new FITSTimeStampReader(cbxExposure.Text, (ExposureUnit)cbxExposureUnits.SelectedIndex, cbxTimeStamp.Text, cbxTimeStampFormat.Text, (TimeStampType)cbxTimestampType.SelectedIndex);
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

                TimeStampReader = new FITSTimeStampReader(cbxTimeStamp.Text, cbxTimeStampFormat.Text, cbxTimeStamp2.Text, cbxTimeStamp2Format.Text);
                DialogResult = DialogResult.OK;
                Close();
            }
        }
    }
}
