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
using Tangra.Helpers;

namespace Tangra.VideoOperations.LightCurves.MutualEvents
{
    public partial class frmAddOrEditMutualEventsTarget : Form
    {
        private ImagePixel m_Center;
        private int m_ObjectId;
        private float m_X0;
        private float m_Y0;
	    private float? m_Aperture = null;
		private float m_X0Center;
		private float m_Y0Center;
		private float m_X1;
		private float m_Y1;
		private float m_X2;
		private float m_Y2;
		private float? m_Aperture1 = null;
		private float? m_Aperture2 = null;
		private float m_X1Center;
		private float m_Y1Center;
		private float m_X2Center;
		private float m_Y2Center;
        private float m_FWHM;
		private float m_FWHM1;
		private float m_FWHM2;
		private PSFFit m_Gaussian;
		private DoublePSFFit m_DoubleGaussian;

		private int m_X1Start;
		private int m_Y1Start;
		private int m_X2Start;
		private int m_Y2Start;
	    private bool m_UserPeakMode = false;

        private TangraConfig.PreProcessingFilter SelectedFilter;
        private uint[,] m_ProcessingPixels;
        private byte[,] m_DisplayPixels;
        private LCStateMachine m_State;

		internal TrackedObjectConfig ObjectToAdd { get; private set; }
		internal TrackedObjectConfig ObjectToAdd2 { get; private set; }
	    private AstroImage m_AstroImage;
        private bool m_IsEdit;

		private Pen m_Pen;
		private Pen m_Pen2;
		private Brush m_Brush;
		private Brush m_Brush2;
		private Color m_Color;
		private Color m_Color2;

        private VideoController m_VideoController;

		private const int MAGN_FACTOR = 6;
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

	        ObjectToAdd = null;
			ObjectToAdd2 = null;

            m_Center = new ImagePixel(center);

	        Initialise();
        }

		private void Initialise()
		{
			picTarget1Pixels.Image = new Bitmap(AREA_SIDE * MAGN_FACTOR, AREA_SIDE * MAGN_FACTOR, PixelFormat.Format24bppRgb);
			picTarget1PSF.Image = new Bitmap(picTarget1PSF.Width, picTarget1PSF.Height);

			if (m_ObjectId == 0)
			{
				m_Color = TangraConfig.Settings.Color.Target1;
				m_Color2 = TangraConfig.Settings.Color.Target2;
			}
			else if (m_ObjectId == 1)
			{
				m_Color = TangraConfig.Settings.Color.Target2;
				m_Color2 = TangraConfig.Settings.Color.Target3;
			}
			else if (m_ObjectId == 2)
			{
				m_Color = TangraConfig.Settings.Color.Target3;
				m_Color2 = TangraConfig.Settings.Color.Target4;
			}
			else if (m_ObjectId == 3)
			{
				m_Color = TangraConfig.Settings.Color.Target4;
				m_Color2 = TangraConfig.Settings.Color.Target4;
			}

			m_Pen = new Pen(m_Color);
			m_Brush = new SolidBrush(m_Color);
			m_Pen2 = new Pen(m_Color2);
			m_Brush2 = new SolidBrush(m_Color2);

			m_ProcessingPixels = m_AstroImage.GetMeasurableAreaPixels(m_Center.X, m_Center.Y, 35);
			m_DisplayPixels = m_AstroImage.GetMeasurableAreaDisplayBitmapPixels(m_Center.X, m_Center.Y, 35);

			bool occultedStartAlreadyPicked = m_State.MeasuringStars.Any(x => x.IsOcultedStar());

			if (occultedStartAlreadyPicked)
			{
				rbOcculted.Enabled = false;
				rbReference.Checked = true;
			}
			else
			{
				rbOcculted.Enabled = true;
				rbOcculted.Checked = true;
			}

			if (!TryAutoLocateDoubleObject())
				rbOneObject.Checked = true;

			m_Aperture = null;
			m_Aperture1 = null;
			m_Aperture2 = null;

			UpdateStateControls();

			DrawCollorPanel();
			DrawCollorPanel2();

			CalculatePSF();
		}

		private bool TryAutoLocateDoubleObject()
		{
			var pixelsFlatList = new List<Tuple<int, int, uint>>();

			for (int x = 0; x < 35; x++)
			{
				for (int y = 0; y < 35; y++)
				{
					uint pixel = m_ProcessingPixels[x, y];
					pixelsFlatList.Add(new Tuple<int, int, uint>(x, y, pixel));
				}
			}

			// Sort by brghtness (brigher at the top)
			pixelsFlatList.Sort((x, y) => y.Item3.CompareTo(x.Item3));

			Tuple<int, int, uint> brightProbe1 = pixelsFlatList[0];
			Tuple<int, int, uint> brightProbe2 = pixelsFlatList[1];
			Tuple<int, int, uint> secondPeakProbe1 = null;
			Tuple<int, int, uint> secondPeakProbe2 = null;

			for (int i = 0; i < pixelsFlatList.Count; i++)
			{
				if (secondPeakProbe1 == null &&
				    ImagePixel.ComputeDistance(brightProbe1.Item1, pixelsFlatList[i].Item1, brightProbe1.Item2, pixelsFlatList[i].Item2) > 3)
				{
					secondPeakProbe1 = pixelsFlatList[i];
				}

				if (secondPeakProbe2 == null &&
					ImagePixel.ComputeDistance(brightProbe2.Item1, pixelsFlatList[i].Item1, brightProbe2.Item2, pixelsFlatList[i].Item2) > 3)
				{
					secondPeakProbe2 = pixelsFlatList[i];
				}

				if (secondPeakProbe1 != null && secondPeakProbe2 != null)
					break;
			}

			if (secondPeakProbe1 != null &&
				IsGoodDoubleObjectFit(brightProbe1, secondPeakProbe1))
			{
				return true;
			}

			if (secondPeakProbe2 != null &&
				IsGoodDoubleObjectFit(brightProbe2, secondPeakProbe2))
			{
				return true;
			}

			return false;
		}

		private bool IsGoodDoubleObjectFit(Tuple<int, int, uint> object1, Tuple<int, int, uint> object2)
		{
			var doubleFit = new DoublePSFFit(100, 100);
			doubleFit.Fit(m_ProcessingPixels, object1.Item1, object1.Item2, object2.Item1, object2.Item2, false);
	
			if (doubleFit.IsSolved &&
			    Math.Abs(doubleFit.FWHM1 - doubleFit.FWHM2) < Math.Max(1, Math.Min(doubleFit.FWHM1, doubleFit.FWHM2)*0.25) &&
				doubleFit.IAmplitude1 > 0 && doubleFit.IAmplitude2 > 0)
			{
				double imaxRatio = doubleFit.IAmplitude1 / doubleFit.IAmplitude2;
				if (imaxRatio > 1) imaxRatio = 1 / imaxRatio;

				if (imaxRatio >= 0.25)
				{
					// We require at least 1:4 ratio in the maximums for the auto-detection to accept that it has found correctly two objects
					m_X1Start = object1.Item1;
					m_Y1Start = object1.Item2;
					m_X2Start = object2.Item1;
					m_Y2Start = object2.Item2;

					return true;
				}
				else
					return false;
			}

			return false;
		}

		private void UpdateViews()
		{
			PlotPixelArea();
			PlotGaussian();
		}

		private void DrawCollorPanel()
		{
			pbox1.Image = new Bitmap(16, 16);
			pbox1a.Image = new Bitmap(16, 16);

			using (Graphics g = Graphics.FromImage(pbox1.Image))
			{
				g.Clear(m_Color);
				g.DrawRectangle(Pens.Black, 0, 0, 15, 15);
			}

			using (Graphics g = Graphics.FromImage(pbox1a.Image))
			{
				g.Clear(m_Color);
				g.DrawRectangle(Pens.Black, 0, 0, 15, 15);
			}

			pbox1.Refresh();
			pbox1a.Refresh();
		}

		private void DrawCollorPanel2()
		{
			pbox2.Image = new Bitmap(16, 16);
			pbox2a.Image = new Bitmap(16, 16);

			using (Graphics g = Graphics.FromImage(pbox2.Image))
			{
				g.Clear(m_Color2);
				g.DrawRectangle(Pens.Black, 0, 0, 15, 15);
			}

			using (Graphics g = Graphics.FromImage(pbox2a.Image))
			{
				g.Clear(m_Color2);
				g.DrawRectangle(Pens.Black, 0, 0, 15, 15);
			}

			pbox2.Refresh();
			pbox2a.Refresh();
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
					if (m_UserPeakMode)
					{
						if (FirstObjectPeakDefined())
						{
							g.DrawLine(Pens.Coral, MAGN_FACTOR * m_X1Start, MAGN_FACTOR * (m_Y1Start - 3), MAGN_FACTOR * m_X1Start, MAGN_FACTOR * (m_Y1Start + 3));
							g.DrawLine(Pens.Coral, MAGN_FACTOR * (m_X1Start - 3), MAGN_FACTOR * m_Y1Start, MAGN_FACTOR * (m_X1Start + 3), MAGN_FACTOR * m_Y1Start);
						}

						if (SecondObjectPeakDefined())
						{
							g.DrawLine(Pens.Coral, MAGN_FACTOR * m_X2Start, MAGN_FACTOR * (m_Y2Start - 3), MAGN_FACTOR * m_X2Start, MAGN_FACTOR * (m_Y2Start + 3));
							g.DrawLine(Pens.Coral, MAGN_FACTOR * (m_X2Start - 3), MAGN_FACTOR * m_Y2Start, MAGN_FACTOR * (m_X2Start + 3), MAGN_FACTOR * m_Y2Start);
						}
					}
					else
					{
						float?[] apertures = new float?[] { m_Aperture, m_Aperture1, m_Aperture2 };
						float[] xPos = new float[] { m_X0, m_X1, m_X2 };
						float[] yPos = new float[] { m_Y0, m_Y1, m_Y2 };
						Pen[] pens = new Pen[] { m_Pen, m_Pen, m_Pen2 };

						for (int i = 0; i < 3; i++)
						{
							if (apertures[i] != null)
							{
								float radius = (float)apertures[i] * MAGN_FACTOR;

								float x0 = xPos[i] + 0.5f;
								float y0 = yPos[i] + 0.5f;

								g.DrawEllipse(
									pens[i],
									(float)(MAGN_FACTOR * (x0) - radius),
									(float)(MAGN_FACTOR * (y0) - radius),
									2 * radius,
									2 * radius);
							}
						}
					}

					g.Save();
				}

				picTarget1Pixels.Refresh();
			}
		}

		internal void PlotGaussian()
		{
			if (m_Gaussian == null && m_DoubleGaussian == null) return;

			Bitmap bmp = picTarget1PSF.Image as Bitmap;
			if (bmp != null)
			{
				Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);

				using (Graphics g = Graphics.FromImage(bmp))
				{
					if (m_DoubleGaussian != null)
					{
						float aperture1 = m_Aperture1.HasValue ? m_Aperture1.Value : 0;
						float aperture2 = m_Aperture2.HasValue ? m_Aperture2.Value : 0;
						m_DoubleGaussian.DrawInternalPoints(g, rect, aperture1, aperture2, m_Brush, m_Brush2, m_AstroImage.Pixelmap.BitPixCamera);	
					}
					else if (m_Gaussian != null)
					{
						float aperture = m_Aperture.HasValue ? m_Aperture.Value : 0;
						m_Gaussian.DrawInternalPoints(g, rect, aperture, m_Brush, m_AstroImage.Pixelmap.BitPixCamera);						
					}
					g.Save();
				}
			}

			if (rbTwoObjects.Checked)
			{
				if (m_Aperture1 != null)
					lblFWHM1.Text = string.Format("{0} px = {1} FWHM", m_Aperture1.Value.ToString("0.00"), (m_Aperture1.Value / m_FWHM1).ToString("0.00"));

				if (m_Aperture2 != null)
					lblFWHM2.Text = string.Format("{0} px = {1} FWHM", m_Aperture2.Value.ToString("0.00"), (m_Aperture2.Value / m_FWHM2).ToString("0.00"));
			}
			else
			{
				if (m_Aperture != null)
					lblFWHM1.Text = string.Format("{0} px = {1} FWHM", m_Aperture.Value.ToString("0.00"), (m_Aperture.Value / m_FWHM).ToString("0.00"));				
			}

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

			UpdateViews();
		}

		private void CalculateSingleObjectPSF()
		{
			var psfFit = new PSFFit(m_Center.X, m_Center.Y);
			psfFit.Fit(m_ProcessingPixels);

			if (psfFit.IsSolved)
			{
				m_Gaussian = psfFit;
				m_FWHM = (float)m_Gaussian.FWHM;
				m_X0 = m_Gaussian.X0_Matrix;
				m_Y0 = m_Gaussian.Y0_Matrix;
				m_X0Center = (float)psfFit.XCenter;
				m_Y0Center = (float)psfFit.YCenter;

				if (m_Aperture == null)
				{
					if (TangraConfig.Settings.Photometry.SignalApertureUnitDefault == TangraConfig.SignalApertureUnit.FWHM)
						m_Aperture = (float)(m_Gaussian.FWHM * TangraConfig.Settings.Photometry.DefaultSignalAperture);
					else
						m_Aperture = (float)(TangraConfig.Settings.Photometry.DefaultSignalAperture);

					nudAperture1.SetNUDValue(m_Aperture.Value);
				}

				m_Aperture1 = null;
				m_Aperture2 = null;
				m_DoubleGaussian = null;
			}
		}

		private void CalculateTwoObjectsPSFs()
		{
			if (!m_UserPeakMode &&
				!FirstObjectPeakDefined() && !SecondObjectPeakDefined() && 
				!LocatePeaks())
			{
				m_UserPeakMode = true;

				MessageBox.Show(
					this, 
					"Please click on the image to define the center of each of the two objects.", 
					"Tangra", 
					MessageBoxButtons.OK, 
					MessageBoxIcon.Question);

				picTarget1Pixels.Cursor = Cursors.Cross;

				UpdateViews();

				return;
			}

			if (!FirstObjectPeakDefined() || !SecondObjectPeakDefined())
			{
				MessageBox.Show(
					this,
					"Cannot locate two objects.",
					"Tangra",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);				
			}

			var psfFit = new DoublePSFFit(m_Center.X, m_Center.Y);
			psfFit.Fit(m_ProcessingPixels, m_X1Start, m_Y1Start, m_X2Start, m_Y2Start, false);

			if (psfFit.IsSolved)
			{
				m_DoubleGaussian = psfFit;
				m_FWHM = (float)psfFit.FWHM;
				m_X0 = psfFit.X0_Matrix;
				m_Y0 = psfFit.Y0_Matrix;

				m_FWHM1 = (float)psfFit.FWHM1;
				m_FWHM2 = (float)psfFit.FWHM2;
				m_X1 = psfFit.X1_Matrix;
				m_Y1 = psfFit.Y1_Matrix;
				m_X2 = psfFit.X2_Matrix;
				m_Y2 = psfFit.Y2_Matrix;
				m_X1Center = (float)psfFit.X1Center;
				m_Y1Center = (float)psfFit.Y1Center;
				m_X2Center = (float)psfFit.X2Center;
				m_Y2Center = (float)psfFit.Y2Center;

				if (m_Aperture1 == null)
				{
					if (TangraConfig.Settings.Photometry.SignalApertureUnitDefault == TangraConfig.SignalApertureUnit.FWHM)
						m_Aperture1 = (float)(psfFit.FWHM1 * TangraConfig.Settings.Photometry.DefaultSignalAperture);
					else
						m_Aperture1 = (float)(TangraConfig.Settings.Photometry.DefaultSignalAperture);

					nudAperture1.SetNUDValue(m_Aperture1.Value);
				}

				if (m_Aperture2 == null)
				{
					if (TangraConfig.Settings.Photometry.SignalApertureUnitDefault == TangraConfig.SignalApertureUnit.FWHM)
						m_Aperture2 = (float)(psfFit.FWHM2 * TangraConfig.Settings.Photometry.DefaultSignalAperture);
					else
						m_Aperture2 = (float)(TangraConfig.Settings.Photometry.DefaultSignalAperture);

					nudAperture2.SetNUDValue(m_Aperture2.Value);
				}

				m_Aperture = null;
				m_Gaussian = null;
			}
		}

		private bool LocatePeaks()
		{
			ResetPeaks();

			return false;
		}

		private void ResetPeaks()
		{
			m_X1Start = 0;
			m_Y1Start = 0;
			m_X2Start = 0;
			m_Y2Start = 0;	
		}

		private bool FirstObjectPeakDefined()
		{
			return m_X1Start != 0 && m_Y1Start != 0;
		}

		private bool SecondObjectPeakDefined()
		{
			return m_X2Start != 0 && m_Y2Start != 0;
		}

		private void picTarget1Pixels_MouseClick(object sender, MouseEventArgs e)
		{
			if (m_UserPeakMode)
			{
				if (!FirstObjectPeakDefined())
				{
					m_X1Start = e.X / MAGN_FACTOR;
					m_Y1Start = e.Y / MAGN_FACTOR;

					PlotPixelArea();
				}
				else
				{
					m_X2Start = e.X / MAGN_FACTOR;
					m_Y2Start = e.Y / MAGN_FACTOR;

					m_UserPeakMode = false;
					picTarget1Pixels.Cursor = Cursors.Default;

					CalculatePSF();
				}
			}
		}

		private void rbTwoObjects_CheckedChanged(object sender, EventArgs e)
		{
			ResetPeaks();

			UpdateStateControls();

			CalculatePSF();
		}

		private void CopyObjectToAdd()
		{
			ObjectToAdd = new TrackedObjectConfig();
			ObjectToAdd.ApertureInPixels = (float)nudAperture1.Value;
			ObjectToAdd.MeasureThisObject = true; /* We measure all objects (comparison or occulted), unless specified otherwise by the user */
			ObjectToAdd.ApertureMatrixX0 = m_X0;
			ObjectToAdd.ApertureMatrixY0 = m_Y0;
			ObjectToAdd.ApertureDX = 0;
			ObjectToAdd.ApertureDY = 0;

			ObjectToAdd.AutoStarsInArea.Clear();

			ObjectToAdd.OriginalFieldCenterX = m_Center.X;
			ObjectToAdd.OriginalFieldCenterY = m_Center.Y;

			ObjectToAdd.IsWeakSignalObject = false;

			ObjectToAdd.Gaussian = m_Gaussian;
			ObjectToAdd.ApertureStartingX = m_X0Center;
			ObjectToAdd.ApertureStartingY = m_Y0Center;

			ObjectToAdd.ProcessInGroup = false;

			if (rbOcculted.Checked)
			{
				ObjectToAdd.TrackingType = TrackingType.OccultedStar;
				ObjectToAdd.PositionTolerance = 1; /* TODO: Is 1 the best value here? */
				if (ObjectToAdd.Gaussian != null)
					// Correction for really large stars
					ObjectToAdd.PositionTolerance += (float)(ObjectToAdd.Gaussian.FWHM / 4);

				ObjectToAdd.PsfFitMatrixSize = TangraConfig.Settings.Special.DefaultOccultedStarPsfFitMatrixSize;
			}
			else
			{
				ObjectToAdd.TrackingType = TrackingType.GuidingStar;
				ObjectToAdd.PsfFitMatrixSize = (int)TangraConfig.Settings.Special.DefaultComparisonStarPsfFitMatrixSize;
				ObjectToAdd.IsWeakSignalObject = false; // Must have a fit for a guiding star
			}

			ObjectToAdd2 = null;
		}

	    private void CopyDoubleObjectsToAdd()
	    {
			// Add the first star (with bigger amplitude)
			ObjectToAdd = new TrackedObjectConfig();
			ObjectToAdd.ApertureInPixels = (float)nudAperture1.Value;
			ObjectToAdd.MeasureThisObject = true; /* We measure all objects (comparison or occulted), unless specified otherwise by the user */
			ObjectToAdd.ApertureMatrixX0 = m_X1;
			ObjectToAdd.ApertureMatrixY0 = m_Y1;
			ObjectToAdd.ApertureDX = 0;
			ObjectToAdd.ApertureDY = 0;

			ObjectToAdd.AutoStarsInArea.Clear();

			ObjectToAdd.OriginalFieldCenterX = m_Center.X;
			ObjectToAdd.OriginalFieldCenterY = m_Center.Y;

			ObjectToAdd.IsWeakSignalObject = false;

			ObjectToAdd.Gaussian = m_DoubleGaussian.GetGaussian1();
			ObjectToAdd.ApertureStartingX = m_X1Center;
			ObjectToAdd.ApertureStartingY = m_Y1Center;

		    ObjectToAdd.ProcessInGroup = true;

			if (rbOcculted.Checked && rbOccElc1.Checked)
			{
				ObjectToAdd.TrackingType = TrackingType.OccultedStar;
				ObjectToAdd.PositionTolerance = 1; /* TODO: Is 1 the best value here? */
				if (ObjectToAdd.Gaussian != null)
					// Correction for really large stars
					ObjectToAdd.PositionTolerance += (float)(ObjectToAdd.Gaussian.FWHM / 4);

				ObjectToAdd.PsfFitMatrixSize = TangraConfig.Settings.Special.DefaultOccultedStarPsfFitMatrixSize;
			}
			else
			{
				ObjectToAdd.TrackingType = TrackingType.GuidingStar;
				ObjectToAdd.PsfFitMatrixSize = (int)TangraConfig.Settings.Special.DefaultComparisonStarPsfFitMatrixSize;
				ObjectToAdd.IsWeakSignalObject = false; // Must have a fit for a guiding star
			}

			// Add the second star (with bigger amplitude)
		    ObjectToAdd2 = new TrackedObjectConfig();
			ObjectToAdd2.ApertureInPixels = (float)nudAperture2.Value;
			ObjectToAdd2.MeasureThisObject = true; /* We measure all objects (comparison or occulted), unless specified otherwise by the user */
			ObjectToAdd2.ApertureMatrixX0 = m_X2;
			ObjectToAdd2.ApertureMatrixY0 = m_Y2;
			ObjectToAdd2.ApertureDX = 0;
			ObjectToAdd2.ApertureDY = 0;

			ObjectToAdd2.AutoStarsInArea.Clear();

			ObjectToAdd2.OriginalFieldCenterX = m_Center.X;
			ObjectToAdd2.OriginalFieldCenterY = m_Center.Y;

			ObjectToAdd2.ProcessInGroup = true;

			ObjectToAdd2.IsWeakSignalObject = false;

			ObjectToAdd2.Gaussian = m_DoubleGaussian.GetGaussian2();
			ObjectToAdd2.ApertureStartingX = m_X2Center;
			ObjectToAdd2.ApertureStartingY = m_Y2Center;

			if (rbOcculted.Checked && rbOccElc2.Checked)
			{
				ObjectToAdd2.TrackingType = TrackingType.OccultedStar;
				ObjectToAdd2.PositionTolerance = 1; /* TODO: Is 1 the best value here? */
				if (ObjectToAdd.Gaussian != null)
					// Correction for really large stars
					ObjectToAdd2.PositionTolerance += (float)(ObjectToAdd.Gaussian.FWHM / 4);

				ObjectToAdd2.PsfFitMatrixSize = TangraConfig.Settings.Special.DefaultOccultedStarPsfFitMatrixSize;
			}
			else
			{
				ObjectToAdd2.TrackingType = TrackingType.GuidingStar;
				ObjectToAdd2.PsfFitMatrixSize = (int)TangraConfig.Settings.Special.DefaultComparisonStarPsfFitMatrixSize;
				ObjectToAdd2.IsWeakSignalObject = false; // Must have a fit for a guiding star
			}
	    }

	    private void nudAperture1_ValueChanged(object sender, EventArgs e)
		{
			if (rbTwoObjects.Checked)
			{
				m_Aperture1 = (float)nudAperture1.Value;
			}
			else
			{
				m_Aperture = (float)nudAperture1.Value;	
			}
			
			UpdateViews();
		}

		private void nudAperture2_ValueChanged(object sender, EventArgs e)
		{
			if (rbTwoObjects.Checked)
			{
				m_Aperture2 = (float)nudAperture2.Value;

				UpdateViews();
			}
		}

		private void UpdateStateControls()
		{
			bool twoObjects = rbTwoObjects.Checked;

			pbox2.Visible = twoObjects;
			pbox2a.Visible = twoObjects;
			nudAperture2.Visible = twoObjects;
			lblAperture2.Visible = twoObjects;
			lblFWHM2.Visible = twoObjects;

			rbOccElc1.Visible = rbOcculted.Checked;
			rbOccElc2.Visible = twoObjects && rbOcculted.Checked;

			gbxGroupType.Text = twoObjects ? "The Group Contains ..." : "The Object Is ...";			
			rbOcculted.Text = twoObjects ? "The Occulted / Eclipsed Star" : "Occulted / Eclipsed Star";
			rbReference.Text = twoObjects ? "Reference Stars Only" : "Reference Stars";
		}

		private void rbOcculted_CheckedChanged(object sender, EventArgs e)
		{
			UpdateStateControls();

			if (rbOcculted.Checked) 
				rbOccElc1.Checked = true;
		}

		private void btnAdd_Click(object sender, EventArgs e)
		{
			if (rbTwoObjects.Checked)
				CopyDoubleObjectsToAdd();
			else
				CopyObjectToAdd();

			DialogResult = DialogResult.OK;
			Close();
		}
    }
}
