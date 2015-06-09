using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Controller;
using Tangra.Model.Video;
using Tangra.Model.VideoOperations;
using Tangra.VideoOperations.Spectroscopy.Helpers;

namespace Tangra.VideoOperations.Spectroscopy
{
    public partial class ucSpectroscopy : UserControl
    {
        private VideoSpectroscopyOperation m_VideoOperation;
	    private VideoController m_VideoContoller;
	    private IFramePlayer m_FramePlayer;

	    private static Pen[] m_GreyPens = new Pen[256];

	    static ucSpectroscopy()
	    {
		    for (int i = 0; i < 256; i++)
		    {
			    m_GreyPens[i] = new Pen(Color.FromArgb(i, i, i));
		    }
	    }

        public ucSpectroscopy()
        {
            InitializeComponent();

	        picSpectra.Image = new Bitmap(picSpectra.Width, picSpectra.Height, PixelFormat.Format24bppRgb);
        }

        public ucSpectroscopy(VideoSpectroscopyOperation videoOperation, VideoController videoContoller, IFramePlayer framePlayer)
            : this()
        {
            m_VideoOperation = videoOperation;
	        m_FramePlayer = framePlayer;
	        m_VideoContoller = videoContoller;
        }

        public void ClearSpectra()
        {
            btnMeasure.Enabled = false;
            picSpectra.Visible = false;
            lblSelectStarNote.Visible = true; 
        }

		public void DisplaySpectra(Spectra spectra)
	    {
			btnMeasure.Enabled = true;
			picSpectra.Visible = true;
			lblSelectStarNote.Visible = false;

		    int xOffset = spectra.ZeroOrderPixelNo - 10;
            float xCoeff = picSpectra.Width * 1.0f / spectra.Points.Count;
			float colorCoeff = 256.0f / spectra.MaxPixelValue;

			using (Graphics g = Graphics.FromImage(picSpectra.Image))
			{
				foreach (SpectraPoint point in spectra.Points)
				{
					byte clr = (byte)(Math.Round(point.RawValue * colorCoeff));
					float x = xCoeff * (point.PixelNo - xOffset);
                    if (x >= 0)
                    {
                        g.DrawLine(m_GreyPens[clr], x, 0, x, picSpectra.Width);
                    }
				}

				g.Save();
			}

			picSpectra.Invalidate();
	    }

		private void btnMeasure_Click(object sender, EventArgs e)
		{
			var frm = new frmRunMultiFrameSpectroscopy(m_FramePlayer, m_VideoOperation, m_VideoContoller.GetCurrentAstroImage(false));
			if (m_VideoContoller.ShowDialog(frm) == DialogResult.OK)
			{
				m_VideoOperation.StartMeasurements(frm.NumberOfMeasurements, frm.MeasurementAreaWing, frm.BackgroundMethod, frm.FrameCombineMethod);
			}
		}
    }
}
