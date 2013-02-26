using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Model.Context;
using Tangra.Model.Video;
using Tangra.Video;

namespace Tangra.VideoOperations.LightCurves
{
    public partial class ucPreProcessing : UserControl
    {
        private TangraConfig.LightCurvesDisplaySettings m_DisplaySettings = new TangraConfig.LightCurvesDisplaySettings();
        private Bitmap m_CurrFrame;
        private IFrameStream m_VideoStream;

        public byte FromByte
        {
            get { return (byte)m_FromByte; }
        }

        public byte ToByte
        {
            get { return (byte)m_ToByte; }
        }

        public int Brightness
        {
            get { return m_Brightness; }
        }

        public int Contrast
        {
            get { return m_Contrast; }
        }

        public ucPreProcessing()
        {
            InitializeComponent();
        }

        public void Initialize(IFrameStream videoStream)
        {
            m_VideoStream = videoStream;

            m_DisplaySettings.Load();
            m_DisplaySettings.Initialize();

            if (m_VideoStream != null)
            {
                nudFrameNo.Minimum = m_VideoStream.FirstFrame;
                nudFrameNo.Maximum = m_VideoStream.LastFrame;
                nudFrameNo.Value = nudFrameNo.Minimum;
            }

            picHistogram.Image = new Bitmap(picHistogram.Width, picHistogram.Height);
            picStretched.Image = new Bitmap(picStretched.Width, picStretched.Height);

            m_HistogramData.Clear();
            for (int i = 0; i < 256; i++)
                m_HistogramData.Add(0);

            m_DisplayData.Clear();
            for (int i = 0; i < 256; i++)
                m_DisplayData.Add(0);

            m_DisplayWindowData.Clear();
            for (int i = 0; i < 256; i++)
                m_DisplayWindowData.Add(0);

            m_FromByte = 0;
            m_ToByte = 128;
            UpdateFromToControls();

            pnlClipStretch.Visible = true;
            pnlBrightnessContrast.Visible = false;

            EnsureWindowData();
        }


        private void DrawBrightnessContrastView()
        {
            if (m_CurrFrame != null &&
                rbBrightnessContrast.Checked)
            {
                // -255, 255
                m_Brightness = (int)Math.Round(255.0 * (tbBrightness.Value - (tbBrightness.Maximum / 2.0)) / (tbBrightness.Maximum / 2.0));
                // -100, 100
                m_Contrast = (sbyte)Math.Round(100.0 * (tbContrast.Value - (tbContrast.Maximum / 2.0)) / (tbContrast.Maximum / 2.0));

                FrameAdjustmentsPreview.Instance.BrightnessContrast(m_Brightness, (sbyte)m_Contrast);
            }
        }

        public void DrawHistograms()
        {
            if (picHistogram.Image == null) return;
            if (picStretched.Image == null) return;

            using (Graphics g = Graphics.FromImage(picHistogram.Image))
            {
                g.Clear(SystemColors.Control);

                if (!rbNone.Checked)
                    DrawFullHistogram(g);

                g.Save();
            }

            using (Graphics g = Graphics.FromImage(picStretched.Image))
            {
                g.Clear(SystemColors.Control);

                if (!rbNone.Checked)
                    DrawWindowHistogram(g);

                g.Save();
            }

            picHistogram.Refresh();
            picStretched.Refresh();
        }

        private void DrawFullHistogram(Graphics g)
        {
            EnsureHistogramData();

            float maxVal = m_DisplayData.Max();
            int XGAP = 0;
            int YGAP = 0;

            if (maxVal > 0)
            {
                float xScale = (picHistogram.Image.Width - 2 * XGAP) * 1.0f / 256;
                float yScale = (picHistogram.Image.Height - 2 * YGAP) * 1.0f / maxVal;

                g.FillRectangle(m_DisplaySettings.BackgroundColorBrush, new Rectangle(0, 0, picHistogram.Image.Width, picHistogram.Image.Height));

                g.DrawRectangle(Pens.Black, XGAP, YGAP, picHistogram.Image.Width - 2 * XGAP + 1, picHistogram.Image.Height - 2 * YGAP);

                // Highlight the current window
                g.FillRectangle(m_DisplaySettings.SmallGraphFocusBackgroundBrush, m_FromByte * xScale, 0, (m_ToByte - m_FromByte) * xScale, picStretched.Image.Height);

                for (int i = 0; i < 256; i++)
                {
                    float xFrom = XGAP + i * xScale + 1;
                    float xSize = xScale;
                    float yFrom = picHistogram.Image.Height - YGAP - m_DisplayData[i] * yScale;
                    float ySize = m_DisplayData[i] * yScale;

                    g.FillRectangle(Brushes.LimeGreen, xFrom, yFrom, xSize, ySize);
                }

                g.Save();
            }
        }

        private void DrawWindowHistogram(Graphics g)
        {
            float maxVal = m_DisplayWindowData.Max();
            int XGAP = 0;
            int YGAP = 0;

            if (maxVal != 0)
            {
                float xScale = (picStretched.Image.Width - 2 * XGAP) * 1.0f / 256;
                float yScale = (picStretched.Image.Height - 2 * YGAP) * 1.0f / maxVal;

                g.FillRectangle(m_DisplaySettings.BackgroundColorBrush, new Rectangle(0, 0, picStretched.Image.Width, picStretched.Image.Height));

                g.DrawRectangle(Pens.Black, XGAP, YGAP, picStretched.Image.Width - 2 * XGAP + 1,
                                picStretched.Image.Height - 2 * YGAP);

                for (int i = 0; i < 256; i++)
                {
                    float xFrom = XGAP + i * xScale + 1;
                    float xSize = xScale;
                    float yFrom = picStretched.Image.Height - YGAP - m_DisplayWindowData[i] * yScale;
                    float ySize = m_DisplayWindowData[i] * yScale;

                    g.FillRectangle(Brushes.LimeGreen, xFrom, yFrom, xSize, ySize);
                }
            }

            g.Save();
        }

        private int m_HistogramFrameNo = -1;
        private List<byte> m_HistogramData = new List<byte>();
        private List<float> m_DisplayData = new List<float>();
        private List<float> m_DisplayWindowData = new List<float>();
        private int m_FromByte = 0;
        private int m_ToByte = 255;
        private int m_Brightness = 0;
        private int m_Contrast = 0;

        private void EnsureHistogramData()
        {
            if (m_VideoStream != null)
            {
                if (m_HistogramFrameNo != (int)nudFrameNo.Value)
                {
                    m_HistogramFrameNo = (int)nudFrameNo.Value;
                    for (int i = 0; i < 256; i++)
                        m_HistogramData[i] = 0;

                    //int colourOffset = -1;
                    //switch(TangraConfig.Settings.Photometry.ColourChannel)
                    //{
                    //    case ColourChannel.Red:
                    //        colourOffset = 2;
                    //        break;
                    //    case ColourChannel.Green:
                    //        colourOffset = 1;
                    //        break;
                    //    case ColourChannel.Blue:
                    //        colourOffset = 0;
                    //        break;
                    //    case ColourChannel.GrayScale:
                    //        colourOffset = -1;
                    //        break;
                    //}

                    //if (m_CurrFrame != null)
                    //    m_CurrFrame.Dispose();

                    FrameAdjustmentsPreview.Instance.MoveToFrame(m_HistogramFrameNo);

                    m_CurrFrame = m_VideoStream.GetPixelmap(m_HistogramFrameNo).CreateDisplayBitmapDoNotDispose();

                    BitmapData bmData = m_CurrFrame.LockBits(new Rectangle(0, 0, m_CurrFrame.Width, m_CurrFrame.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                    try
                    {
                        int stride = bmData.Stride;
                        System.IntPtr Scan0 = bmData.Scan0;

                        unsafe
                        {
                            byte* p = (byte*)(void*)Scan0;

                            int nOffset = stride - m_CurrFrame.Width * 3;

                            for (int y = 0; y < m_CurrFrame.Height; ++y)
                            {
                                for (int x = 0; x < m_CurrFrame.Width; ++x)
                                {
                                    byte bt = p[2]; // use RED channel
                                    //if (colourOffset != -1)
                                    //    bt = p[2];
                                    //else
                                    //    bt = AstroImage.GetGrayScaleReading(p[0], p[1], p[2]);

                                    m_HistogramData[bt]++;

                                    p += 3;
                                }
                                p += nOffset;
                            }
                        }
                    }
                    finally
                    {
                        m_CurrFrame.UnlockBits(bmData);
                    }

                    for (int i = 0; i < 256; i++)
                        m_DisplayData[(byte)i] = (float)Math.Log10(m_HistogramData[(byte)i] + 1);

                    EnsureWindowData();
                }
            }
        }

        private void EnsureWindowData()
        {
            float range = rbWindow.Checked
                              ? 256.0f / (trackBarWindow.Maximum - 1)
                              : (float)(nudTo.Value - nudFrom.Value);

            if (rbWindow.Checked)
            {
                m_FromByte = (byte)Math.Max(0, Math.Round((trackBarWindow.Value - 1) * range));
                m_ToByte = (byte)Math.Min(255, Math.Round(trackBarWindow.Value * range));

                UpdateFromToControls();
            }
            else
            {
                m_FromByte = (byte)nudFrom.Value;
                m_ToByte = (byte)nudTo.Value;
            }

            if (rbStretching.Checked)
            {
                for (int i = m_FromByte; i < m_ToByte; i++)
                {
                    float val = m_DisplayData[i];

                    for (int j = 0; j < 256; j++)
                    {
                        if (j >= (i - m_FromByte) * 256.0f / range && j < (i - m_FromByte + 1) * 256.0f / range)
                            m_DisplayWindowData[j] = val;
                    }
                }
            }
            else if (rbClipping.Checked)
            {
                for (int i = 0; i < 256; i++)
                {
                    if (i < m_FromByte || i > m_ToByte)
                        m_DisplayWindowData[i] = 0;
                    else
                        m_DisplayWindowData[i] = m_DisplayData[i];
                }
            }
        }

        private void UpdateFromToControls()
        {
            nudFrom.Value = m_FromByte;
            nudTo.Value = m_ToByte;
        }

        private void nudFrameNo_ValueChanged(object sender, EventArgs e)
        {
            UpdateViews();
        }

        private void UpdateViews()
        {
            EnsureWindowData();
            DrawHistograms();
            DrawBrightnessContrastView();

            ushort pixelFrom = (ushort)m_FromByte;
            ushort pixelTo = (ushort)m_ToByte;

            if (m_VideoStream != null && m_VideoStream.BitPix == 12)
            {
                pixelFrom <<= 4;
                pixelTo <<= 4;
            }
			else if (m_VideoStream != null && m_VideoStream.BitPix == 14)
			{
				pixelFrom <<= 6;
				pixelTo <<= 6;
			}

			if (rbStretching.Checked)
				FrameAdjustmentsPreview.Instance.Stretching(pixelFrom, pixelTo);
			else if (rbClipping.Checked)
				FrameAdjustmentsPreview.Instance.Clipping(pixelFrom, pixelTo);
        }

        private void trackBarWindow_ValueChanged(object sender, EventArgs e)
        {
            if (trackBarWindow.Value >= trackBarWindow.Maximum)
                trackBarWindow.Value = trackBarWindow.Maximum - 1;

            UpdateViews();
        }

        private void nudStretchingMagnification_ValueChanged(object sender, EventArgs e)
        {
            trackBarWindow.Maximum = (int)nudStretchingMagnification.Value + 1;

            UpdateViews();
        }

        private void nudFrom_ValueChanged(object sender, EventArgs e)
        {
            UpdateViews();
        }

        private void nudTo_ValueChanged(object sender, EventArgs e)
        {
            UpdateViews();
        }

        private bool m_SyncValues = false;

        private void tbBrightness_ValueChanged(object sender, EventArgs e)
        {
            m_SyncValues = true;
            try
            {
                nudBrightness.Value = tbBrightness.Value;
                DrawBrightnessContrastView();
            }
            finally
            {
                m_SyncValues = false;
            }
        }

        private void tbContrast_ValueChanged(object sender, EventArgs e)
        {
            m_SyncValues = true;
            try
            {
                nudContrast.Value = tbContrast.Value - 100;
                DrawBrightnessContrastView();
            }
            finally
            {
                m_SyncValues = false;
            }
        }

        private void nudBrightness_ValueChanged(object sender, EventArgs e)
        {
            if (!m_SyncValues)
            {
                tbBrightness.Value = (int)nudBrightness.Value;
            }
        }

        private void nudContrast_ValueChanged(object sender, EventArgs e)
        {
            if (!m_SyncValues)
            {
                tbContrast.Value = (int)nudContrast.Value + 100;
            }
        }

        private void rgPreProcessing_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool enabled = !rbNone.Checked;

            picHistogram.Enabled = enabled;
            picStretched.Enabled = enabled;
            nudFrameNo.Enabled = enabled;
            nudStretchingMagnification.Enabled = enabled;
            trackBarWindow.Enabled = enabled;
            gbStretchClipSettings.Enabled = enabled;
            pnlBrightnessContrast.Enabled = enabled;
            pnlClipStretch.Enabled = enabled;

			if (!enabled)
				FrameAdjustmentsPreview.Instance.NoPreProcessing();
			else
            {
                if (rbStretching.Checked || rbClipping.Checked)
                {
                    pnlClipStretch.Visible = true;
                    pnlBrightnessContrast.Visible = false;
                }

                bool brightnessMode = rbBrightnessContrast.Checked;

                if (brightnessMode)
                {
                    pnlClipStretch.Visible = false;
                    pnlBrightnessContrast.Visible = true;
                    gbStretchClipSettings.Visible = false;
                    nudStretchingMagnification.Visible = false;
                    lblMagn.Visible = false;
                    lblMagnX.Visible = false;
                    pnlControlsBrContr.Visible = false;


                }
                else
                {
                    pnlClipStretch.Visible = true;
                    pnlBrightnessContrast.Visible = false;
                    gbStretchClipSettings.Visible = true;
                    nudStretchingMagnification.Visible = true;
                    lblMagn.Visible = true;
                    lblMagnX.Visible = true;
                    pnlControlsBrContr.Visible = true;
                }
            }

            DrawHistograms();
            UpdateViews();
        }

        private void rgWindowOrCustom_SelectedIndexChanged(object sender, EventArgs e)
        {
            trackBarWindow.Visible = rbWindow.Checked;
            pnlCustomStretch.Visible = !rbWindow.Checked;
        }
    }
}
