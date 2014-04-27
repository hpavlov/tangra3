using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Controller;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Model.Image;
using Tangra.VideoOperations.LightCurves.Tracking;

namespace Tangra.VideoOperations.LightCurves.MutualEvents
{
    public partial class frmAddOrEditMutualEventsTarget : Form
    {
        private ImagePixel m_Center;
        private int m_ObjectId;
        private float m_X0;
        private float m_Y0;
	    private float m_Aperture;
        private float m_FWHM;
        private PSFFit m_Gaussian;

        private TangraConfig.PreProcessingFilter SelectedFilter;
        private uint[,] m_ProcessingPixels;
        private byte[,] m_DisplayPixels;
        private LCStateMachine m_State;

        internal readonly TrackedObjectConfig ObjectToAdd;
        private AstroImage m_AstroImage;
        private bool m_IsEdit;

		private Pen m_Pen;
		private Brush m_Brush;
		private Color m_Color;

        private VideoController m_VideoController;

		private const int MAGN_FACTOR = 5;
		private const int AREA_SIDE = 35;

        public frmAddOrEditMutualEventsTarget()
        {
            InitializeComponent();
        }

        internal frmAddOrEditMutualEventsTarget(int objectId, ImagePixel center, PSFFit gaussian, LCStateMachine state, VideoController videoController)
        {
            InitializeComponent();

            m_VideoController = videoController;

            Text = "Add 'Mutual Event' Target";
            btnAdd.Text = "Add";
            btnDontAdd.Text = "Don't Add";
            btnDelete.Visible = false;
            m_IsEdit = false;

            m_ObjectId = objectId;
            m_State = state;
			m_AstroImage = m_VideoController.GetCurrentAstroImage(false);

            ObjectToAdd = new TrackedObjectConfig();

            m_Center = new ImagePixel(center);

	        Initialise();
        }

		private void Initialise()
		{
			picTarget1Pixels.Image = new Bitmap(AREA_SIDE * MAGN_FACTOR, AREA_SIDE * MAGN_FACTOR, PixelFormat.Format24bppRgb);
			picTarget1PSF.Image = new Bitmap(picTarget1PSF.Width, picTarget1PSF.Height);

			if (m_ObjectId == 0)
				m_Color = TangraConfig.Settings.Color.Target1;
			else if (m_ObjectId == 1)
				m_Color = TangraConfig.Settings.Color.Target2;
			else if (m_ObjectId == 2)
				m_Color = TangraConfig.Settings.Color.Target3;
			else if (m_ObjectId == 3)
				m_Color = TangraConfig.Settings.Color.Target4;

			m_Pen = new Pen(m_Color);
			m_Brush = new SolidBrush(m_Color);

			m_ProcessingPixels = m_AstroImage.GetMeasurableAreaPixels(m_Center.X, m_Center.Y, 35);
			m_DisplayPixels = m_AstroImage.GetMeasurableAreaDisplayBitmapPixels(m_Center.X, m_Center.Y, 35);

			m_Aperture = float.NaN;

			DrawCollorPanel();

			CalculatePSF();

			UpdateViews();
		}

		private void UpdateViews()
		{
			PlotPixelArea();
			PlotGaussian();
		}

		private void DrawCollorPanel()
		{
			pbox1.Image = new Bitmap(16, 16);
			using (Graphics g = Graphics.FromImage(pbox1.Image))
			{
				g.Clear(m_Color);
				g.DrawRectangle(Pens.Black, 0, 0, 15, 15);
			}

			pbox1.Refresh();
		}

		/// <summary>
		/// This method displays the currently selected pixels. If there are any filters
		///  to be applied they should have been already applied
		/// </summary>
		private void PlotPixelArea()
		{
			Bitmap bmp = picTarget1Pixels.Image as Bitmap;
			if (bmp != null && m_ProcessingPixels != null)
			{
				byte peak = 0;
				for (int x = 0; x < AREA_SIDE; x++)
					for (int y = 0; y < AREA_SIDE; y++)
					{
						if (peak < m_DisplayPixels[x, y])
							peak = m_DisplayPixels[x, y];
					}

				// This copes the pixels to a new array of pixels for displaying. This new array may have slightly different
				// dimentions (LP) and pixel intensities may be normalized (LPD)
				for (int x = 0; x < AREA_SIDE; x++)
				{
					for (int y = 0; y < AREA_SIDE; y++)
					{
						byte pixelValue = m_DisplayPixels[x, y];

						Color pixelcolor = SystemColors.Control;

						if (pixelValue < TangraConfig.Settings.Photometry.Saturation.Saturation8Bit)
						{
							if (SelectedFilter == TangraConfig.PreProcessingFilter.LowPassDifferenceFilter)
							{
								pixelcolor = Color.FromArgb(150 * pixelValue / peak, 150 * pixelValue / peak,
															150 * pixelValue / peak);
							}
							else if (SelectedFilter == TangraConfig.PreProcessingFilter.LowPassFilter)
							{
								if (x >= 1 && x < AREA_SIDE && y >= 1 && y < AREA_SIDE)
									pixelcolor = Color.FromArgb(pixelValue, pixelValue, pixelValue);
							}
							else
								pixelcolor = Color.FromArgb(pixelValue, pixelValue, pixelValue);
						}
						else
							pixelcolor = TangraConfig.Settings.Color.Saturation;


						for (int dx = 0; dx < MAGN_FACTOR; dx++)
						{
							for (int dy = 0; dy < MAGN_FACTOR; dy++)
							{
								bmp.SetPixel(MAGN_FACTOR * x + dx, MAGN_FACTOR * y + dy, pixelcolor);
							}
						}
					}
				}

				m_VideoController.ApplyDisplayModeAdjustments(bmp);

				using (Graphics g = Graphics.FromImage(bmp))
				{
					float radius = (float)m_Aperture * MAGN_FACTOR;

					float x0 = m_X0 + 0.5f;
					float y0 = m_Y0 + 0.5f;

					g.DrawEllipse(
						m_Pen,
						(float)(MAGN_FACTOR * (x0) - radius),
						(float)(MAGN_FACTOR * (y0) - radius),
						2 * radius,
						2 * radius);

					g.Save();
				}

				picTarget1Pixels.Refresh();
			}
		}

		internal void PlotGaussian()
		{
			if (m_Gaussian == null) return;

			Bitmap bmp = picTarget1PSF.Image as Bitmap;
			if (bmp != null)
			{
				Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);

				using (Graphics g = Graphics.FromImage(bmp))
				{
					m_Gaussian.DrawInternalPoints(g, rect, m_Aperture, m_Brush, m_AstroImage.Pixelmap.BitPixCamera);
					g.Save();
				}
			}


			lblFWHM1.Text =
				string.Format("{0} px = {1} FWHM", m_Aperture.ToString("0.00"), (m_Aperture / m_FWHM).ToString("0.00"));

			picTarget1PSF.Refresh();
		}

		private void CalculatePSF()
		{
			if (rbOneObject.Checked)
			{
				CalculateSingleObjectPSF();
			}
			else if (rbTwoObjects.Checked)
			{
				CalculateTwoObjectsPSFs();
			} 
		}

		private void CalculateSingleObjectPSF()
		{
			m_Gaussian = new PSFFit(m_Center.X, m_Center.Y);
			PSFFit.NormVal = m_VideoController.VideoAav16NormVal;

			m_Gaussian.Fit(m_ProcessingPixels);
			m_FWHM = (float)m_Gaussian.FWHM;
			m_X0 = m_Gaussian.X0_Matrix;
			m_Y0 = m_Gaussian.Y0_Matrix;

			if (float.IsNaN(m_Aperture))
			{
				if (m_Gaussian.IsSolved && TangraConfig.Settings.Photometry.SignalApertureUnitDefault == TangraConfig.SignalApertureUnit.FWHM)
					m_Aperture = (float)(m_Gaussian.FWHM * TangraConfig.Settings.Photometry.DefaultSignalAperture);
				else
					m_Aperture = (float)(TangraConfig.Settings.Photometry.DefaultSignalAperture);
			}
		}

		private void CalculateTwoObjectsPSFs()
		{
			
		}
    }
}
