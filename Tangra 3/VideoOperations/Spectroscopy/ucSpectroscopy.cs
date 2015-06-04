using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tangra.VideoOperations.Spectroscopy
{
    public partial class ucSpectroscopy : UserControl
    {
        private VideoSpectroscopyOperation m_VideoOperation;
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

        public ucSpectroscopy(VideoSpectroscopyOperation videoOperation)
            : this()
        {
            m_VideoOperation = videoOperation;
        }

		public void DisplaySpectra(Spectra spectra)
	    {
			btnMeasure.Enabled = true;
			picSpectra.Visible = true;
			lblSelectStarNote.Visible = false;

		    float xCoeff = picSpectra.Width * 1.0f / spectra.Points.Count;
			float colorCoeff = 256.0f / spectra.MaxPixelValue;

			using (Graphics g = Graphics.FromImage(picSpectra.Image))
			{
				foreach (SpectraPoint point in spectra.Points)
				{
					byte clr = (byte)(Math.Round(point.RawValue * colorCoeff));
					float x = xCoeff * point.PixelNo;
					g.DrawLine(m_GreyPens[clr], x, 0, x, picSpectra.Width);
				}

				g.Save();
			}
			picSpectra.Invalidate();
	    }
    }
}
