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
        private SpectroscopyController m_SpectroscopyController;
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

        public ucSpectroscopy(VideoSpectroscopyOperation videoOperation, VideoController videoContoller, SpectroscopyController spectroscopyController, IFramePlayer framePlayer)
            : this()
        {
            m_VideoOperation = videoOperation;
	        m_FramePlayer = framePlayer;
	        m_VideoContoller = videoContoller;
            m_SpectroscopyController = spectroscopyController;
        }

        public void ClearSpectra()
        {
            btnMeasure.Enabled = false;
            picSpectra.Visible = false;
            lblSelectStarNote.Visible = true; 
        }

		public void PreviewSpectra(Spectra spectra)
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

	    public void MeasurementsStarted()
	    {
			btnMeasure.Enabled = false;
	    }

	    public void MeasurementsFinished()
	    {
			btnMeasure.Visible = false;
		    btnShowSpectra.Visible = true;
		    btnShowSpectra.BringToFront();
	    }

		private void btnMeasure_Click(object sender, EventArgs e)
		{
			var frm = new frmRunMultiFrameSpectroscopy(m_FramePlayer, m_VideoOperation, m_VideoContoller.GetCurrentAstroImage(false));
			if (m_VideoContoller.ShowDialog(frm) == DialogResult.OK)
			{
                m_SpectroscopyController.SpectraReductionContext.Reset();
			    m_SpectroscopyController.SpectraReductionContext.FramesToMeasure = frm.NumberOfMeasurements;
			    m_SpectroscopyController.SpectraReductionContext.MeasurementAreaWing = frm.MeasurementAreaWing;
                m_SpectroscopyController.SpectraReductionContext.BackgroundAreaWing = frm.BackgroundAreaWing;
                m_SpectroscopyController.SpectraReductionContext.BackgroundAreaGap = frm.BackgroundAreaGap;
			    m_SpectroscopyController.SpectraReductionContext.BackgroundMethod = frm.BackgroundMethod;
			    m_SpectroscopyController.SpectraReductionContext.PixelValueCoefficient = frm.PixelValueCoefficient;
			    m_SpectroscopyController.SpectraReductionContext.FrameCombineMethod = frm.FrameCombineMethod;
			    m_SpectroscopyController.SpectraReductionContext.UseFineAdjustments = frm.UseFineAdjustments;
			    m_SpectroscopyController.SpectraReductionContext.UseLowPassFilter = frm.UseLowPassFilter;
                m_SpectroscopyController.SpectraReductionContext.AlignmentAbsorptionLinePos = frm.AlignmentAbsorptionLinePos;
				m_SpectroscopyController.SpectraReductionContext.ExposureSeconds = frm.ExposureSeconds;

                m_VideoOperation.StartMeasurements();
			}
		}

		private void btnShowSpectra_Click(object sender, EventArgs e)
		{
			m_VideoOperation.DisplaySpectra();
		}
    }
}
