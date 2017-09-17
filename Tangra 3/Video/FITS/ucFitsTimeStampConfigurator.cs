using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Config;
using Tangra.Model.Helpers;

namespace Tangra.Video.FITS
{
    public partial class ucFitsTimeStampConfigurator : UserControl
    {
        public class SingleTimeStampConfig
        {
            public TangraConfig.TimeStampType TimeStampType;
            public string TimeStampHeader;
            public string TimeStampFormat;
            public string TimeStampHeader2;
            public string TimeStampFormat2;
            public bool TimeStampIsDateTimeParts;
        }

        private bool m_SeparateDateTimeMode;

        private List<HeaderEntry> m_AllCards;
        private string m_FilesHash;
        private string m_CardNamesHash;
        private FitsTimestampHelper m_TimeStampHelper;

        public ucFitsTimeStampConfigurator()
        {
            InitializeComponent();
        }

        internal void Initialise(List<HeaderEntry> allCards, string fileHash, string cardsHash, FitsTimestampHelper timestampHelper)
        {
            m_AllCards = allCards;
            m_FilesHash = fileHash;
            m_CardNamesHash = cardsHash;
            m_TimeStampHelper = timestampHelper;

            cbxTimeStamp.Items.AddRange(m_AllCards.ToArray());
            cbxSecondCard.Items.AddRange(m_AllCards.ToArray());

            cbxTimestampType.SelectedIndex = 0;

            UpdateControls();
        }

        private void SetTimeStampFormats()
        {
            cbxTimeStampFormat.Items.Clear();

            var formats = new List<object>();
            formats.AddRange(FITSTimeStampReader.STANDARD_TIMESTAMP_FORMATS);

            if (TangraConfig.Settings.Generic.CustomFITSTimeStampFormats != null)
                formats.AddRange(TangraConfig.Settings.Generic.CustomFITSTimeStampFormats);

            cbxTimeStampFormat.Items.AddRange(formats.ToArray());
            if (!string.IsNullOrEmpty(TangraConfig.Settings.LastUsed.FitsTimestampFormat))
            {
                int idx = cbxTimeStampFormat.Items.IndexOf(TangraConfig.Settings.LastUsed.FitsTimestampFormat);
                if (idx > -1)
                {
                    cbxTimeStampFormat.SelectedIndex = idx;
                }
            }
            else
            {
                cbxTimeStampFormat.SelectedIndex = 0;
            }
        }

        private void SetDateTimeFormats()
        {
            cbxTimeStampFormat.Items.Clear();
            cbxSecondCardFormat.Items.Clear();

            var dateFormats = new List<object>();
            dateFormats.AddRange(FITSTimeStampReader.STANDARD_DATE_FORMATS);

            var timeFormats = new List<object>();
            timeFormats.AddRange(FITSTimeStampReader.STANDARD_TIME_FORMATS);

            if (TangraConfig.Settings.Generic.CustomFITSDateFormats != null)
                dateFormats.AddRange(TangraConfig.Settings.Generic.CustomFITSDateFormats);

            if (TangraConfig.Settings.Generic.CustomFITSTimeFormats != null)
                timeFormats.AddRange(TangraConfig.Settings.Generic.CustomFITSTimeFormats);

            cbxTimeStampFormat.Items.AddRange(dateFormats.ToArray());
            cbxSecondCardFormat.Items.AddRange(timeFormats.ToArray());

            if (!string.IsNullOrEmpty(TangraConfig.Settings.LastUsed.FitsDateFormat))
            {
                int idx = cbxTimeStampFormat.Items.IndexOf(TangraConfig.Settings.LastUsed.FitsDateFormat);
                if (idx > -1)
                {
                    cbxTimeStampFormat.SelectedIndex = idx;
                }
            }
            else
            {
                cbxTimeStampFormat.SelectedIndex = 0;
            }

            if (!string.IsNullOrEmpty(TangraConfig.Settings.LastUsed.FitsTimeFormat))
            {
                int idx = cbxSecondCardFormat.Items.IndexOf(TangraConfig.Settings.LastUsed.FitsTimeFormat);
                if (idx > -1)
                {
                    cbxSecondCardFormat.SelectedIndex = idx;
                }
            }
            else
            {
                cbxSecondCardFormat.SelectedIndex = 0;
            }  
        }

        internal void UseRecentFitsConfig(TangraConfig.FITSFieldConfig config)
        {
            rbSeparateDateTime.Checked = config.TimeStampIsDateTimeParts;

            for (int i = 0; i < cbxTimeStamp.Items.Count; i++)
            {
                if (((HeaderEntry)cbxTimeStamp.Items[i]).Card.Key == config.TimeStampHeader)
                {
                    cbxTimeStamp.SelectedIndex = i;
                    break;
                }
            }

            if (config.TimeStampFormat != null)
                cbxTimeStampFormat.SelectedIndex = cbxTimeStampFormat.Items.IndexOf(config.TimeStampFormat);

            if (config.TimeStampIsDateTimeParts)
            {
                for (int i = 0; i < cbxSecondCard.Items.Count; i++)
                {
                    if (((HeaderEntry)cbxSecondCard.Items[i]).Card.Key == config.TimeStampHeader2)
                    {
                        cbxSecondCard.SelectedIndex = i;
                        break;
                    }
                }

                if (config.TimeStampFormat2 != null)
                    cbxSecondCardFormat.SelectedIndex = cbxSecondCardFormat.Items.IndexOf(config.TimeStampFormat2);                
            }

            if (config.FileHash == m_FilesHash || m_CardNamesHash == config.CardNamesHash)
            {
                cbxTimestampType.SelectedIndex = (int)config.TimeStampType;
            }
            else
            {
                cbxTimestampType.SelectedIndex = -1;
            }
        }

        private void rbSeparateDateTime_CheckedChanged(object sender, EventArgs e)
        {
            m_SeparateDateTimeMode = rbSeparateDateTime.Checked;
            UpdateControls();
            DisplaySelectedTimeStampCardValues();
            VerifyTimeStamp();
        }

        private void UpdateControls()
        {
            if (m_SeparateDateTimeMode)
            {
                pnlSecondCard.Visible = true;
                tbxTimeStampValue.Top = 98;

                SetDateTimeFormats();
            }
            else
            {
                pnlSecondCard.Visible = false;
                tbxTimeStampValue.Top = 72;

                SetTimeStampFormats();
            }
        }

        private void cbxTimeStamp_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplaySelectedTimeStampCardValues();
            VerifyTimeStamp();
        }

        private void cbxSecondCard_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplaySelectedTimeStampCardValues();
            VerifyTimeStamp();
        }

        private void DisplaySelectedTimeStampCardValues()
        {
            string cardValues = null;
            var entry = cbxTimeStamp.SelectedItem as HeaderEntry;
            if (entry != null)
                cardValues = entry.Card.ToString();
            else
                cardValues = string.Empty;

            if (m_SeparateDateTimeMode)
            {
                cardValues += "\r\n";
                entry = cbxSecondCard.SelectedItem as HeaderEntry;
                if (entry != null)
                    cardValues += entry.Card.ToString();
            }

            tbxTimeStampValue.Text = cardValues;
        }

        private void cbxTimeStampFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            VerifyTimeStamp();
        }

        public bool ValidateInput()
        {
            if (!m_TimeStampMappingValid)
            {
                MessageBox.Show("Please choose valid settings for the time timestamp.");
                return false;
            }

            if (cbxTimestampType.SelectedIndex == -1)
            {
                MessageBox.Show("Please choose timestamp type.");
                cbxTimestampType.Focus();
                return false;
            }

            return true;
        }

        public SingleTimeStampConfig GetSelectedInput()
        {
            var rv = new SingleTimeStampConfig();
            rv.TimeStampType = (TangraConfig.TimeStampType) cbxTimestampType.SelectedIndex;
            rv.TimeStampHeader = cbxTimeStamp.Text;
            rv.TimeStampFormat = cbxTimeStampFormat.Text;
            rv.TimeStampIsDateTimeParts = m_SeparateDateTimeMode;
            if (m_SeparateDateTimeMode)
            {
                rv.TimeStampHeader2 = cbxSecondCard.Text;
                rv.TimeStampFormat2 = cbxSecondCardFormat.Text;                
            }
            return rv;
        }

        private bool m_TimeStampMappingValid = false;

        public void VerifyTimeStamp()
        {
            if (m_SeparateDateTimeMode)
                VerifySeparateDateTime();
            else
                VerifySingleTimeStamp();
        }

        private void VerifySingleTimeStamp()
        {
            m_TimeStampMappingValid = false;
            var entry = cbxTimeStamp.SelectedItem as HeaderEntry;
            if (entry != null)
            {
                string value = entry.Card.Value;
                string format = cbxTimeStampFormat.Text;
                m_TimeStampMappingValid = m_TimeStampHelper.VerifyTimeStamp(value, format);
            }

            if (m_TimeStampMappingValid)
                pbxTimeStampOK.BringToFront();
            else
                pbxTimeStampWarning.BringToFront();
        }

        private void VerifySeparateDateTime()
        {
            m_TimeStampMappingValid = false;
            var entry1 = cbxTimeStamp.SelectedItem as HeaderEntry;
            var entry2 = cbxSecondCard.SelectedItem as HeaderEntry;
            
            if (entry1 != null)
            {
                string value = entry1.Card.Value;
                string format = cbxTimeStampFormat.Text;
                var dateValid = m_TimeStampHelper.VerifyTimeStamp(value, format);

                if (dateValid)
                    pbxTimeStampOK.BringToFront();
                else
                    pbxTimeStampWarning.BringToFront(); 
            }

            if (entry2 != null)
            {
                string value2 = entry2.Card.Value;
                string format2 = cbxSecondCardFormat.Text;
                var timeValid = m_TimeStampHelper.VerifyTimeStamp(value2, format2);

                if (timeValid)
                    pbxTimeStamp2OK.BringToFront();
                else
                    pbxTimeStamp2Warning.BringToFront();
            }

            if (entry1 != null && entry2 != null)
            {
                string value = entry1.Card.Value;
                string format = cbxTimeStampFormat.Text;
                string value2 = entry2.Card.Value;
                string format2 = cbxSecondCardFormat.Text;
                m_TimeStampMappingValid = m_TimeStampHelper.VerifyTimeStamp(value, format, value2, format2);
            }        
        }

        private void cbxTimestampType_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbxSecondCardType.SelectedIndex = cbxTimestampType.SelectedIndex;
        }

        private void cbxSecondCardFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            VerifyTimeStamp();
        }
    }
}
