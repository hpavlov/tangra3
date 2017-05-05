using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Model.Image;
using Tangra.Model.VideoOperations;
using Tangra.OCR;
using Tangra.OCR.Model;

namespace OcrTester
{
    public partial class frmNewDev : Form, IVideoController
    {
        private Bitmap m_Frame;
        private Pixelmap m_Pixelmap;
        private AstroImage m_Image;
        private uint[] m_InitialPixels;

        public frmNewDev()
        {
            InitializeComponent();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (File.Exists(tbxFileLocation.Text))
            {
                m_Frame = (Bitmap)Bitmap.FromFile(tbxFileLocation.Text);
                //m_FieldsFrame = BitmapFilter.ToVideoFields(m_Frame);

                m_Pixelmap = Pixelmap.ConstructFromBitmap(m_Frame, TangraConfig.ColourChannel.GrayScale);
                m_Image = new AstroImage(m_Pixelmap);

                m_InitialPixels = m_Image.GetOcrPixels();

                pictureBox1.Load(tbxFileLocation.Text);
            }
        }


        private void btnCalibrate_Click(object sender, EventArgs e)
        {
            var ocr = new Tangra.OCR.GpsBoxSpriteOcr();
            ocr.Initialize(new TimestampOCRData() { FrameWidth = m_Image.Width, FrameHeight = m_Image.Height}, this, 0);
            ocr.ProcessCalibrationFrame(0, m_InitialPixels);
            using (Graphics g = Graphics.FromImage(pictureBox1.Image))
            {
                ocr.DrawLegend(g);
                g.Save();
            }
            pictureBox1.Invalidate();
        }

        private void btnPlotBoxes_Click(object sender, EventArgs e)
        {
            using (Graphics g = Graphics.FromImage(pictureBox1.Image))
            {
                for (int i = 0; i < 20; i++)
                {
                    float x = (float)nudOffset.Value + i * (float)nudWidth.Value;
                    g.DrawRectangle(Pens.Lime, x, (float)nudTop.Value, (float)nudWidth.Value, (float)nudHeight.Value);
                }

                g.Save();
            }

            pictureBox1.Invalidate();
        }


        void IVideoController.StatusChanged(string displayName)
        {
            throw new NotImplementedException();
        }

        DialogResult IVideoController.ShowMessageBox(string message, string title, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            throw new NotImplementedException();
        }

        DialogResult IVideoController.ShowMessageBox(string message, string title, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
        {
            throw new NotImplementedException();
        }

        DialogResult IVideoController.ShowSaveFileDialog(string title, string filter, ref string fileName, IWin32Window ownerWindow)
        {
            throw new NotImplementedException();
        }

        DialogResult IVideoController.ShowOpenFileDialog(string title, string filter, out string fileName)
        {
            throw new NotImplementedException();
        }

        void IVideoController.UpdateViews()
        {
            throw new NotImplementedException();
        }

        void IVideoController.RefreshCurrentFrame()
        {
            throw new NotImplementedException();
        }

        void IVideoController.RedrawCurrentFrame(bool showFields, bool reloadImage)
        {
            throw new NotImplementedException();
        }

        void IVideoController.ToggleShowFieldsMode(bool showFields)
        {
            throw new NotImplementedException();
        }

        void IVideoController.UpdateZoomedImage(Bitmap zoomedBitmap, ImagePixel center)
        {
            throw new NotImplementedException();
        }

        void IVideoController.ClearZoomedImage()
        {
            throw new NotImplementedException();
        }

        AstroImage IVideoController.GetCurrentAstroImage(bool integrated)
        {
            throw new NotImplementedException();
        }

        FrameStateData IVideoController.GetCurrentFrameState()
        {
            throw new NotImplementedException();
        }

        double IVideoController.VideoFrameRate
        {
            get { throw new NotImplementedException(); }
        }

        int IVideoController.VideoBitPix
        {
            get { throw new NotImplementedException(); }
        }

        int IVideoController.VideoCountFrames
        {
            get { throw new NotImplementedException(); }
        }

        int IVideoController.VideoFirstFrame
        {
            get { throw new NotImplementedException(); }
        }

        int IVideoController.VideoLastFrame
        {
            get { throw new NotImplementedException(); }
        }

        void IVideoController.PlayVideo(int? startAtFrame, uint step)
        {
            throw new NotImplementedException();
        }

        void IVideoController.StopVideo(Action<int, bool> callback)
        {
            throw new NotImplementedException();
        }

        void IVideoController.MoveToFrame(int frameId)
        {
            throw new NotImplementedException();
        }

        void IVideoController.StepForward()
        {
            throw new NotImplementedException();
        }

        void IVideoController.StepBackward()
        {
            throw new NotImplementedException();
        }

        bool IVideoController.IsAstroDigitalVideo
        {
            get { throw new NotImplementedException(); }
        }

        bool IVideoController.HasAstroImageState
        {
            get { throw new NotImplementedException(); }
        }

        string IVideoController.CurrentVideoFileName
        {
            get { throw new NotImplementedException(); }
        }

        string IVideoController.CurrentVideoFileType
        {
            get { throw new NotImplementedException(); }
        }

        void IVideoController.RegisterExtractingOcrTimestamps()
        {
            throw new NotImplementedException();
        }

        void IVideoController.RegisterOcrError()
        {
            throw new NotImplementedException();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string error = null;
            uint[] dataOut = new uint[m_InitialPixels.Length];
            uint[] dataDebugNoLChD = new uint[m_InitialPixels.Length];
            //GpsBoxSpriteOcr.PrepareOsdArea(m_InitialPixels, dataOut, dataDebugNoLChD, m_Image.Width, m_Image.Height);
            var rv = GpsBoxSpriteOcr.PreProcessImageForOCR(m_InitialPixels, m_Image.Width, m_Image.Height, ref error);
            var bmp = Pixelmap.ConstructBitmapFrom8BitPixelmap(new Pixelmap(m_Image.Width, m_Image.Height, 8, rv, null, null));
            using (Graphics g = Graphics.FromImage(pictureBox1.Image))
            {
                g.DrawImage(bmp, new Point(0, 0));
                g.Save();
            }
            pictureBox1.Invalidate();            
        }
    }
}
