using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using nom.tam.fits;
using Tangra.Controller;
using Tangra.Helpers;
using Tangra.Model.Config;
using Tangra.Model.Helpers;

namespace Tangra.Video.FITS
{
    public partial class frmDefineFitsCube3D : Form
    {
        public FITSTimeStampReader TimeStampReader { get; private set; }

        public int WidthIndex { get; private set; }

        public int HeightIndex { get; private set; }

        public int FrameIndex { get; private set; }

        public short MinPixelValue { get; private set; }
        
        public uint MaxPixelValue { get; private set; }

        public bool HasNegativePixels { get; private set; }

        public int BitPix { get; private set; }

        public int NegPixCorrection { get; private set; }

        private BasicHDU m_ImageHDU;

        public frmDefineFitsCube3D()
        {
            InitializeComponent();
        }

        private VideoController m_VideoController;
        private string m_FilesHash;
        private string m_CardNamesHash;
        private List<HeaderEntry> m_AllCards = new List<HeaderEntry>();
        private FitsTimestampHelper m_TimeStampHelper;
 
        public frmDefineFitsCube3D(BasicHDU imageHDU, string filesHash, VideoController videoController)
            : this()
        {
            m_VideoController = videoController;

            m_ImageHDU = imageHDU;
            m_FilesHash = filesHash;

            var hasher = new SHA1CryptoServiceProvider();
            hasher.Initialize();
            var orderedCardNames = new List<string>();

            var cursor = m_ImageHDU.Header.GetCursor();
            while (cursor.MoveNext())
            {
                var card = m_ImageHDU.Header.FindCard((string)cursor.Key);
                if (card != null)
                {
                    m_AllCards.Add(new HeaderEntry(card));
                }
                orderedCardNames.Add((string)cursor.Key);
            }

            orderedCardNames.Sort();
            byte[] combinedCardNamesBytes = Encoding.UTF8.GetBytes(string.Join("|", orderedCardNames));
            var hash = hasher.ComputeHash(combinedCardNamesBytes, 0, combinedCardNamesBytes.Length);
            m_CardNamesHash = Convert.ToBase64String(hash);

            cbxNaxisOrder.SelectedIndex = 0;
            cbxExposure.Items.AddRange(m_AllCards.ToArray());

            cbxExposureUnits.Items.Clear();
            cbxExposureUnits.Items.AddRange(Enum.GetNames(typeof(TangraConfig.ExposureUnit)));

            cbxExposureUnits.SelectedIndex = 0;

            m_TimeStampHelper = new FitsTimestampHelper(m_FilesHash, m_AllCards, UseRecentFitsConfig);
            ucTimestampControl.Initialise(m_AllCards, m_FilesHash, m_CardNamesHash, m_TimeStampHelper);
            m_TimeStampHelper.TryIdentifyPreviousConfigApplyingForCurrentFiles();
        }

        private void UpdateSizesLabels()
        {
            lblFrames.Text = m_ImageHDU.Axes[FrameIndex].ToString();
            lblHeight.Text = m_ImageHDU.Axes[HeightIndex].ToString();
            lblWidth.Text = m_ImageHDU.Axes[WidthIndex].ToString();

            short minPixelValue;
            uint maxPixelValue;
            bool hasNegativePixels;
            int bpp;
            int bzero;
            m_VideoController.SetCursor(Cursors.WaitCursor);
            try
            {
                ParseFirstFrame(out minPixelValue, out maxPixelValue, out bpp, out bzero, out hasNegativePixels);
            }
            finally
            {
                m_VideoController.SetCursor(Cursors.Default);
            }

            if (hasNegativePixels)
                ucNegativePixelsTreatment.Initialise(bzero, minPixelValue, maxPixelValue);
            else
                NegPixCorrection = 0;

            ucNegativePixelsTreatment.Visible = hasNegativePixels;

            MinPixelValue = minPixelValue;
            MaxPixelValue = maxPixelValue;
            HasNegativePixels = hasNegativePixels;
            BitPix = bpp;
        }

        private void cbxNaxisOrder_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cbxNaxisOrder.SelectedIndex)
            {
                case 0:
                    FrameIndex = 0;
                    HeightIndex = 1;
                    WidthIndex = 2;
                    break;
                case 1:
                    FrameIndex = 0;
                    WidthIndex = 1;
                    HeightIndex = 2;
                    break;
                case 2:
                    WidthIndex = 0;
                    HeightIndex = 1;
                    FrameIndex = 2;
                    break;
                case 3:
                    HeightIndex = 0;
                    WidthIndex = 1;
                    FrameIndex = 2;
                    break;
                case 4:
                    WidthIndex = 0;
                    FrameIndex = 1;
                    HeightIndex = 2;
                    break;
                case 5:
                    HeightIndex = 0;
                    FrameIndex = 1;
                    WidthIndex = 2;
                    break;
            }

            UpdateSizesLabels();            
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

        private void UseRecentFitsConfig(TangraConfig.FITSFieldConfig config)
        {
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

            ucTimestampControl.UseRecentFitsConfig(config);

            if (config.FileHash == m_FilesHash || m_CardNamesHash == config.CardNamesHash)
            {
                cbxExposureUnits.SelectedIndex = (int)config.ExposureUnit;
            }
            else
            {
                cbxExposureUnits.SelectedIndex = -1;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
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

            var singleTimeStampConfig = ucTimestampControl.GetSelectedInput();

            var config = new TangraConfig.FITSFieldConfig()
            {
                ExposureHeader = cbxExposure.Text,
                ExposureUnit = (TangraConfig.ExposureUnit)cbxExposureUnits.SelectedIndex,
                IsTimeStampAndExposure = true,
                TimeStampIsDateTimeParts = singleTimeStampConfig.TimeStampIsDateTimeParts,
                TimeStampHeader = singleTimeStampConfig.TimeStampHeader,
                TimeStampFormat = singleTimeStampConfig.TimeStampFormat,
                TimeStampHeader2 = singleTimeStampConfig.TimeStampHeader2,
                TimeStampFormat2 = singleTimeStampConfig.TimeStampFormat2,
                TimeStampType = singleTimeStampConfig.TimeStampType,
            };

            TimeStampReader = new FITSTimeStampReader(config);

            config.FileHash = m_FilesHash;
            config.CardNamesHash = m_CardNamesHash;
            TangraConfig.Settings.RecentFITSFieldConfig.Register(config);
            TangraConfig.Settings.Save();

            if (HasNegativePixels)
            {
                NegPixCorrection = ucNegativePixelsTreatment.NegPixCorrection;

                MaxPixelValue = (uint)((int)MaxPixelValue - NegPixCorrection);
                BitPix = FITSHelper.GetBppForMaxPixelValue(MaxPixelValue);
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private bool Load16BitFitsFile(string fileName, IFITSTimeStampReader timeStampReader,
            out uint[,] pixels, out int width, out int height, out uint medianValue, out Type pixelDataType, out bool hasNegativePixels, out short minValue, out uint maxValue,
            FITSHelper.CheckOpenedFitsFileCallback callback)
        {
            float frameExposure;

            width = m_ImageHDU.Axes[WidthIndex];
            height = m_ImageHDU.Axes[HeightIndex];

            return FITSHelper.LoadFitsDataInternal(
                m_ImageHDU,
                GetFirstFramePixelArray(), fileName, timeStampReader,
                out pixels, out medianValue, out pixelDataType, out frameExposure, out hasNegativePixels, out minValue, out maxValue, callback, 
                (Array dataArray, int h, int w, double bzero, out uint[,] ppx, out uint median, out Type dataType, out bool hasNegPix, out short minV, out uint maxV) =>
                {
                    ppx = FITSHelper.Load16BitImageData(dataArray, m_ImageHDU.Axes[HeightIndex], m_ImageHDU.Axes[WidthIndex], (int)bzero, out median, out dataType, out hasNegPix, out minV, out maxV);
                });
        }

        private Array GetFirstFramePixelArray()
        {
            return FITSHelper.GetPixelsFrom3DCube(
                (Array)m_ImageHDU.Data.DataArray, 0, 
                FrameIndex, HeightIndex, WidthIndex,
                m_ImageHDU.Axes[FrameIndex], m_ImageHDU.Axes[HeightIndex], m_ImageHDU.Axes[WidthIndex]);
        }

        private void ParseFirstFrame(out short minPixelValue, out uint maxPixelValue, out int bpp, out int bzero, out bool hasNegativePixels)
        {
            uint[] pixelsFlat;
            int width;
            int height;
            DateTime? timestamp;
            double? exposure;
            int bz = 0;
            var cards = new Dictionary<string, string>();

            FITSHelper.Load16BitFitsFile(null, Load16BitFitsFile, null,
                (hdu) =>
                {
                    var cursor = hdu.Header.GetCursor();
                    bz = FITSHelper.GetBZero(hdu);
                    while (cursor.MoveNext())
                    {
                        HeaderCard card = hdu.Header.FindCard((string)cursor.Key);
                        if (card != null && !string.IsNullOrWhiteSpace(card.Key) && card.Key != "END")
                        {
                            if (cards.ContainsKey(card.Key))
                                cards[card.Key] += "\r\n" + card.Value;
                            else
                                cards.Add(card.Key, card.Value);
                        }
                    }
                }, out pixelsFlat, out width, out height, out bpp, out timestamp, out exposure, out minPixelValue, out maxPixelValue, out hasNegativePixels);

            bzero = bz;
        }
    }
}
