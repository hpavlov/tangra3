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
using Tangra.Controller;
using Tangra.Model.Config;
using Tangra.Model.Helpers;
using Tangra.PInvoke;

namespace Tangra.Video.FITS
{
    public partial class frmChooseTimeHeaders : Form
    {
        internal FITSTimeStampReader TimeStampReader;
        internal bool FlipVertically;
        internal bool FlipHorizontally;
        internal int NegPixCorrection;
        private List<HeaderEntry> m_AllCards = new List<HeaderEntry>();
        private FitsTimestampHelper m_TimeStampHelper;
 
        private string m_FilesHash;
        private string m_CardNamesHash;
        private VideoController m_VideoController;

        private bool m_PixelMappingReviewed;

        private int m_BelowZeroCorr;

        public short MinPixelValue { get; private set; }

        public uint[] Pixels { get; private set; }

        public int BZero { get; private set; }

        public uint MaxPixelValue { get; private set; }

        public bool HasNegativePixels { get; private set; }

        public int BitPix { get; private set; }

        public frmChooseTimeHeaders()
        {
            InitializeComponent();
        }

        public frmChooseTimeHeaders(string fileName, string filesHash, VideoController videoController)
            : this()
        {
            m_VideoController = videoController;

            FITSData fitsData;
            m_VideoController.SetCursor(Cursors.WaitCursor);
            try
            {
                fitsData = FITSHelper2.LoadFitsFile(fileName, null, 0);

                BZero = FITSHelper2.GetBZero(fitsData.HDU);
                m_BelowZeroCorr = Math.Max(BZero, Math.Abs(fitsData.PixelStats.MinPixelValue) - BZero);
                
                fitsData = FITSHelper2.LoadFitsFile(fileName, null, BZero - m_BelowZeroCorr);
            }
            finally
            {
                m_VideoController.SetCursor(Cursors.Default);
            }

            m_FilesHash = filesHash;

            var hasher = new SHA1CryptoServiceProvider();
            hasher.Initialize();
            var orderedCardNames = new List<string>();

            var cursor = fitsData.HDU.Header.GetCursor();
            while (cursor.MoveNext())
            {
                var card = fitsData.HDU.Header.FindCard((string)cursor.Key);
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

            cbxExposure.Items.AddRange(m_AllCards.ToArray());

            cbxExposureUnits.Items.Clear();
            cbxExposureUnits.Items.AddRange(Enum.GetNames(typeof(TangraConfig.ExposureUnit)));
            cbxExposureUnits.SelectedIndex = 0;

            m_TimeStampHelper = new FitsTimestampHelper(m_FilesHash, m_AllCards, UseRecentFITSConfig);
            ucTimestampControl.Initialise(m_AllCards, m_FilesHash, m_CardNamesHash, m_TimeStampHelper);
            ucTimestampControl2.Initialise(m_AllCards, m_FilesHash, m_CardNamesHash, m_TimeStampHelper);
            ucTimestampControl2.SetTimeStampType(TangraConfig.TimeStampType.EndExposure, false);
            m_TimeStampHelper.TryIdentifyPreviousConfigApplyingForCurrentFiles();

            if (fitsData.PixelStats.HasNegativePixels)
            {
                NegPixCorrection = fitsData.PixelStats.MinPixelValue;
            }
            else
            {
                // No requirement to review the pixel mapping if there are no negative pixels
                // We can go with the defaults
                m_PixelMappingReviewed = true;
                NegPixCorrection = 0;
            }

            Pixels = fitsData.PixelsFlat;
            MinPixelValue = fitsData.PixelStats.MinPixelValue;
            MaxPixelValue = fitsData.PixelStats.MaxPixelValue;
            HasNegativePixels = fitsData.PixelStats.HasNegativePixels;
            BitPix = fitsData.PixelStats.BitPix;
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

        private bool m_ExposureValid = false;

        private void VerifyExposure()
        {
            m_ExposureValid = false;
            var entry = cbxExposure.SelectedItem as HeaderEntry;
            if (entry != null)
            {
                string value = entry.Card.Value;
                m_ExposureValid = m_TimeStampHelper.VerifyExposure(value);
            }

            if (m_ExposureValid)
                pbxExposureOK.BringToFront();
            else
                pbxExposureWarning.BringToFront();
        }

        private void UseRecentFITSConfig(TangraConfig.FITSFieldConfig config)
        {
            ucTimestampControl.UseRecentFitsConfig(config);
            ucTimestampControl2.UseRecentFitsConfig2(config);

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

            if (config.FileHash == m_FilesHash || m_CardNamesHash == config.CardNamesHash)
            {
                cbxExposureUnits.SelectedIndex = (int)config.ExposureUnit;
            }
            else
            {
                cbxExposureUnits.SelectedIndex = -1;
            }

            rbStartEndTimestamp.Checked = !config.IsTimeStampAndExposure;
        }

        private void cbxExposureUnits_SelectedIndexChanged(object sender, EventArgs e)
        {
            VerifyExposure();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (rbTimeDuration.Checked)
            {
                if (!ucTimestampControl.ValidateInput())
                    return;

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

                if (HasNegativePixels && !m_PixelMappingReviewed)
                {
                    if (!ReviewPixelMapping())
                    {
                        MessageBox.Show("As there are negative pixels you need to confirm the pixel value mapping before continuing.");
                        btnPixelValueMapping.Focus();
                        return;
                    }
                }

                var singleTimeStampConfig = ucTimestampControl.GetSelectedInput();

                var config = new TangraConfig.FITSFieldConfig()
                {
                    ExposureHeader = cbxExposure.Text,
                    ExposureUnit = (TangraConfig.ExposureUnit) cbxExposureUnits.SelectedIndex,
                    TimeStampIsDateTimeParts = singleTimeStampConfig.TimeStampIsDateTimeParts,
                    TimeStampHeader = singleTimeStampConfig.TimeStampHeader,
                    TimeStampFormat = singleTimeStampConfig.TimeStampFormat,
                    TimeStampHeader2 = singleTimeStampConfig.TimeStampHeader2,
                    TimeStampFormat2 = singleTimeStampConfig.TimeStampFormat2,
                    TimeStampType = singleTimeStampConfig.TimeStampType,
                    IsTimeStampAndExposure = true
                };

                TimeStampReader = new FITSTimeStampReader(config);

                config.FileHash = m_FilesHash;
                config.CardNamesHash = m_CardNamesHash;
                TangraConfig.Settings.RecentFITSFieldConfig.Register(config);
                TangraConfig.Settings.Save();

                FlipVertically = cbxFlipVertically.Checked;
                FlipHorizontally = cbxFlipHorizontally.Checked;

                DialogResult = DialogResult.OK;
                Close();
            }
            else if (rbStartEndTimestamp.Checked)
            {
                if (!ucTimestampControl.ValidateInput())
                    return;

                if (!ucTimestampControl2.ValidateInput())
                    return;

                var startTimeStampConfig = ucTimestampControl.GetSelectedInput();
                var endTimeStampConfig = ucTimestampControl2.GetSelectedInput();

                var config = new TangraConfig.FITSFieldConfig()
                {
                    TimeStampIsDateTimeParts = startTimeStampConfig.TimeStampIsDateTimeParts,
                    TimeStampHeader = startTimeStampConfig.TimeStampHeader,
                    TimeStampFormat = startTimeStampConfig.TimeStampFormat,
                    TimeStampHeader2 = startTimeStampConfig.TimeStampHeader2,
                    TimeStampFormat2 = startTimeStampConfig.TimeStampFormat2,
                    TimeStamp2IsDateTimeParts = endTimeStampConfig.TimeStampIsDateTimeParts,
                    TimeStamp2Header = endTimeStampConfig.TimeStampHeader,
                    TimeStamp2Format = endTimeStampConfig.TimeStampFormat,
                    TimeStamp2Header2 = endTimeStampConfig.TimeStampHeader2,
                    TimeStamp2Format2 = endTimeStampConfig.TimeStampFormat2,
                    IsTimeStampAndExposure = false
                };

                TimeStampReader = new FITSTimeStampReader(config);

                config.FileHash = m_FilesHash;
                config.CardNamesHash = m_CardNamesHash;
                TangraConfig.Settings.RecentFITSFieldConfig.Register(config);
                TangraConfig.Settings.Save();

                FlipVertically = cbxFlipVertically.Checked;
                FlipHorizontally = cbxFlipHorizontally.Checked;

                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void rbStartEndTimestamp_CheckedChanged(object sender, EventArgs e)
        {
            if (rbStartEndTimestamp.Checked)
            {

                ucTimestampControl2.BringToFront();
                ucTimestampControl2.Visible = true;
                pnlExposure.SendToBack();
                pnlExposure.Visible = false;
                ucTimestampControl.SetTimeStampType(TangraConfig.TimeStampType.StartExposure, false);
            }
            else
            {
                ucTimestampControl2.SendToBack();
                ucTimestampControl2.Visible = false;
                pnlExposure.BringToFront();
                pnlExposure.Visible = true;
                ucTimestampControl.SetTimeStampType(null, true);
            }
        }

        private bool ReviewPixelMapping()
        {
            var frm = new frmDefinePixelMapping(Pixels, NegPixCorrection, BZero, MinPixelValue, MaxPixelValue, BitPix);
            if (m_VideoController.ShowDialog(frm) != DialogResult.OK)
                return false;

            NegPixCorrection = frm.NegPixCorrection;
            MaxPixelValue = frm.MaxPixelValue;
            BitPix = frm.BitPix;
            m_PixelMappingReviewed = true;
            return true;
        }

        private void btnPixelValueMapping_Click(object sender, EventArgs e)
        {
            ReviewPixelMapping();
        }
    }
}
