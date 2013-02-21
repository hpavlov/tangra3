using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Model.Image;
using Tangra.VideoOperations.LightCurves;
using Tangra.VideoOperations.LightCurves.Tracking;

namespace Tangra.VideoOperations.LightCurves
{
	public partial class frmAddOrEditSingleTarget : Form
	{
		private ImagePixel m_Center;
		private int m_ObjectId;
        private float m_X0;
	    private float m_Y0;
        private float m_FWHM;
	    private PSFFit m_Gaussian;

		private Pen m_Pen;
		private Brush m_Brush;
		private Color m_Color = Color.WhiteSmoke;

		private float m_Aperture = 2.5f;
		private TangraConfig.PreProcessingFilter SelectedFilter;
		private uint[,] m_ProcessingPixels;
		private byte[,] m_DisplayPixels;
		private LCStateMachine m_State;

	    private List<PSFFit> m_AutoStarsInArea;
        private List<PSFFit> m_AutoStarsInLargerArea;
	    private bool m_IsBrightEnoughForAutoGuidingStar;
	    //private bool m_DefaultToComparisonStar;

	    internal readonly TrackedObjectConfig ObjectToAdd;
	    private AstroImage m_AstroImage;
	    private bool m_IsEdit;
		private bool m_AutocenteredApertureAvailable;

        internal frmAddOrEditSingleTarget(int objectId, TrackedObjectConfig selectedObject, LCStateMachine state)
        {
            InitializeComponent();

			m_AutocenteredApertureAvailable = true;

            Text = "Edit Object";

            btnAdd.Text = "Save";
            btnDontAdd.Text = "Cancel";
            btnDelete.Visible = true;
            m_IsEdit = true;

            m_ObjectId = objectId;
            m_State = state;
            m_AstroImage = m_State.VideoOperation.m_StackedAstroImage;

            ObjectToAdd = selectedObject;

			if (selectedObject.TrackingType != TrackingType.ComparisonStar)
                nudFitMatrixSize.Value = selectedObject.PsfFitMatrixSize;

            m_Center = new ImagePixel(
                selectedObject.OriginalFieldCenterX,
                selectedObject.OriginalFieldCenterY);

            if (ObjectToAdd.PositionTolerance > 0)
                nudPositionTolerance.Value = (decimal)ObjectToAdd.PositionTolerance;

            Initialize();

            if (!selectedObject.IsWeakSignalObject && !selectedObject.IsFixedAperture)
                GetFitInMatrix(selectedObject.Gaussian, ref selectedObject.PsfFitMatrixSize, selectedObject.ApertureInPixels);
            else
            {
                m_ProcessingPixels = m_AstroImage.GetMeasurableAreaPixels(m_Center);
				m_DisplayPixels = m_AstroImage.GetMeasurableAreaDisplayBitmapPixels(m_Center);

                m_FWHM = 6;
                m_Gaussian = null;
                m_X0 = selectedObject.ApertureMatrixX0;
                m_Y0 = selectedObject.ApertureMatrixY0;
                dx = selectedObject.ApertureDX;
                dy = selectedObject.ApertureDY;

                PlotSingleTargetPixels();

                nudAperture1.Value = (decimal)Math.Round(ObjectToAdd.ApertureInPixels, 2);
            }

			SetHeightAndType();

			if (selectedObject.TrackingType == TrackingType.GuidingStar)
				SelectedObjectType = TrackingType.GuidingStar;
			else if (selectedObject.TrackingType == TrackingType.OccultedStar)
				SelectedObjectType = TrackingType.OccultedStar;
        }


		internal frmAddOrEditSingleTarget(int objectId, ImagePixel center, PSFFit gaussian, LCStateMachine state)
		{
			InitializeComponent();

			m_AutocenteredApertureAvailable = true;

            Text = "Add Object";
		    btnAdd.Text = "Add";
            btnDontAdd.Text = "Don't Add";
            btnDelete.Visible = false;
            m_IsEdit = false;

		    nudFitMatrixSize.Value = 11; // Config.AppConfig.Instance.PhotometrySettings.DefaultFsfFitArea;
		    nudFitMatrixSize.Maximum = 15;

			m_ObjectId = objectId;
			m_State = state;
            m_AstroImage = m_State.VideoOperation.m_StackedAstroImage;

            ObjectToAdd = new TrackedObjectConfig();

            m_Center = new ImagePixel(center);

            Initialize();

            if (rbOccultedStar.Enabled)
				SelectedObjectType = TrackingType.OccultedStar;
			else
				SelectedObjectType = TrackingType.GuidingStar;

            // Apply filtering to the processing pixels according to the configured default filter value
            int matirxSize = (int)nudFitMatrixSize.Value;
            
            GetFitInMatrix(gaussian, ref matirxSize);

            nudFitMatrixSize.Maximum = matirxSize;

			if (SelectedObjectType != TrackingType.OccultedStar)
				SetHeightAndType();
		}

        private void Initialize()
        {

#if !PRODUCTION
            btnExplain.Visible = true;
#else
            btnExplain.Visible = false;
#endif
            SelectedFilter = LightCurveReductionContext.Instance.DigitalFilter;

            picTarget1Pixels.Image = new Bitmap(119, 119);
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

            rbManuallyPositionedAperture.Checked = ObjectToAdd.IsFixedAperture;

            bool occStarSelected = m_State.MeasuringStars.Exists(t => t.TrackingType == TrackingType.OccultedStar)
                                   && (ObjectToAdd.TrackingType != TrackingType.OccultedStar);

            rbOccultedStar.Enabled = !occStarSelected;
            pnlTolerance.Enabled = !occStarSelected;

            
            nudAperture1.Maximum = LightCurveReductionContext.Instance.GetMaxApertureSize();

            DrawCollorPanel();
        }

        private void SetHeightAndType()
        {
            gbAperture.Visible = LightCurveReductionContext.Instance.AllowsManuallyPlacedAperture;

            if (!m_IsBrightEnoughForAutoGuidingStar)
            {
                gbxObjectType.Height = 100;
                rbComparisonStar.Visible = true;
                bool needsGuidingStar =
                    !m_State.MeasuringStars.Exists(t => t.TrackingType == TrackingType.GuidingStar || t.TrackingType == TrackingType.OccultedStar);

                if (needsGuidingStar)
                {
                    rbGuidingStar.Checked = true;
                    rbComparisonStar.Checked = m_IsEdit && ObjectToAdd.TrackingType == TrackingType.ComparisonStar;
                }
                else
                {
                    rbComparisonStar.Checked = true;
                    rbGuidingStar.Checked = m_IsEdit && ObjectToAdd.TrackingType == TrackingType.GuidingStar;
                }

                if (LightCurveReductionContext.Instance.AllowsManuallyPlacedAperture)
                {
                    Height = 419;
                    gbAperture.Top = 118;
                }
                else
                {
                    Height = 370;
                }
            }
            else
            {
                gbxObjectType.Height = 80;
                rbComparisonStar.Visible = false;

                if (LightCurveReductionContext.Instance.AllowsManuallyPlacedAperture)
                {
                    Height = 408;
                    gbAperture.Top = 98;
                }
                else
                {
                    Height = 354;
                }
            }

            PlotSingleTargetPixels(); // Because the number of radio buttons may have changed
        }

        private bool GetFitInMatrix(PSFFit gaussian, ref int matirxSize)
        {
            return GetFitInMatrix(gaussian, ref matirxSize, float.NaN);
        }

        private bool GetFitInMatrix(PSFFit gaussian, ref int matirxSize, float preselectedAperture)
        {
            rbGuidingStar.Text = "Guiding Star";
            m_IsBrightEnoughForAutoGuidingStar = false;

            nudFitMatrixSize.ValueChanged -= nudFitMatrixSize_ValueChanged;
            try
            {
                uint[,] autoStarsPixels = m_AstroImage.GetMeasurableAreaPixels(m_Center.X, m_Center.Y, 35);
                m_AutoStarsInLargerArea = StarFinder.GetStarsInArea(
                    ref autoStarsPixels,
                    m_AstroImage.Pixelmap.BitPixCamera,
                    m_AstroImage.MedianNoise,
                    LightCurveReductionContext.Instance.DigitalFilter);

                m_ProcessingPixels = ImageFilters.CutArrayEdges(autoStarsPixels, 9);
                m_DisplayPixels = m_AstroImage.GetMeasurableAreaDisplayBitmapPixels(m_Center.X, m_Center.Y, 17);

                m_AutoStarsInArea = new List<PSFFit>();
                foreach (PSFFit autoStar in m_AutoStarsInLargerArea)
                {
                    if (autoStar.XCenter > 9 && autoStar.XCenter < 9 + 17 &&
                        autoStar.YCenter > 9 && autoStar.YCenter < 9 + 17)
                    {
                        // Don't change original star so use a clone
                        PSFFit clone = autoStar.Clone();
                        clone.SetNewFieldCenterFrom35PixMatrix(8, 8);
                        m_AutoStarsInArea.Add(clone);
                    }
                }

                int oldMatirxSize = matirxSize;

                if (m_AutoStarsInArea.Count == 0)
                {
                    rbGuidingStar.Text = "Guiding Star";

                    // There are no stars that are bright enough. Simply let the user do what they want
                    m_X0 = 8;
                    m_Y0 = 8;
                    m_FWHM = 6;
                    m_Gaussian = null;

                    nudFitMatrixSize.Value = 11;
                }
                else if (m_AutoStarsInArea.Count == 1)
                {
                    // There is exactly one good star found. Go and do a fit in a wider area
                    double bestFindTolerance = 3.0;
                    //LightCurveReductionContext.Instance.VeryCloseObjectsFlag 
                    //    ? 1.5
                    //    : 3.0;

                    for (int i = 0; i < 2; i++)
                    {
                        MeasurementsHelper measurement = ReduceLightCurveOperation.DoConfiguredMeasurement(m_ProcessingPixels, gaussian, m_AstroImage.Pixelmap.BitPixCamera, bestFindTolerance, ref m_Aperture, ref matirxSize);
                        if (measurement != null && matirxSize != -1)
                        {
                            if (/*LightCurveReductionContext.Instance.VeryCloseObjectsFlag && */ matirxSize < 5)
                            {
                                // Do a centroid in the full area, and get another matix centered at the centroid 
                                ImagePixel centroid = new ImagePixel(m_Center.X, m_Center.Y); //m_AstroImage.GetCentroid(m_Center.X, m_Center.Y, 9));

                                m_ProcessingPixels = m_AstroImage.GetMeasurableAreaPixels(centroid);
                                m_DisplayPixels = m_AstroImage.GetMeasurableAreaDisplayBitmapPixels(centroid);

                                m_X0 = centroid.X;
                                m_Y0 = centroid.Y;
                                m_FWHM = 6;
                                m_Gaussian = null;

                                nudFitMatrixSize.Value = 11;
                            }
                            else
                            {
                                m_X0 = measurement.XCenter;
                                m_Y0 = measurement.YCenter;
                                if (measurement.FoundBestPSFFit != null)
                                {
                                    m_FWHM = (float)measurement.FoundBestPSFFit.FWHM;
                                    m_Gaussian = measurement.FoundBestPSFFit;
                                }
                                else
                                {
                                    m_FWHM = 6;
                                    m_Gaussian = null;
                                }
                                m_ProcessingPixels = measurement.PixelData;
                                nudFitMatrixSize.Value = matirxSize;
                            }
                        }
                        else
                        {
                            matirxSize = oldMatirxSize;
                            return false;
                        }

                        if (m_Gaussian != null)
                        {
                            if (IsBrightEnoughtForGuidingStar())
                            {
                                rbGuidingStar.Text = "Guiding/Comparison Star";
                                m_IsBrightEnoughForAutoGuidingStar = true;
                            }

                            break;
                        }

                        //if (i == 0 && LightCurveReductionContext.Instance.VeryCloseObjectsFlag)
                        //    bestFindTolerance = 3.0;
                    }
                }
                else if (m_AutoStarsInArea.Count > 1)
                {
                    rbGuidingStar.Text = "Guiding Star";

                    // There are more stars found.
                    double xBest = m_Gaussian != null
                                      ? m_Gaussian.XCenter
                                      : m_IsEdit ? ObjectToAdd.ApertureMatrixX0
                                                 : 8.5;

                    double yBest = m_Gaussian != null
                                      ? m_Gaussian.YCenter
                                      : m_IsEdit ? ObjectToAdd.ApertureMatrixY0
                                                 : 8.5;

                    // by default use the one closest to the original location
                    PSFFit closestFit = m_AutoStarsInArea[0];
                    double closestDist = double.MaxValue;
                    foreach (PSFFit star in m_AutoStarsInArea)
                    {
                        double dist =
                            Math.Sqrt((star.XCenter - xBest) * (star.XCenter - xBest) +
                                      (star.YCenter - yBest) * (star.YCenter - yBest));
                        if (closestDist > dist)
                        {
                            closestDist = dist;
                            closestFit = star;
                        }
                    }


                    m_X0 = (float)closestFit.XCenter;
                    m_Y0 = (float)closestFit.YCenter;
                    m_FWHM = (float)closestFit.FWHM;
                    m_Gaussian = closestFit;

                    //m_ProcessingPixels = measurement.PixelData;
                    nudFitMatrixSize.Value = m_IsEdit ? ObjectToAdd.PsfFitMatrixSize : closestFit.MatrixSize;
                }

                decimal appVal = (decimal)m_Aperture;
                if (float.IsNaN(preselectedAperture))
                {
                    if (nudAperture1.Maximum < appVal) nudAperture1.Maximum = appVal + 1;
                }
                else
                    appVal = (decimal)preselectedAperture;

                nudAperture1.Value = Math.Round(appVal, 2);

                PlotSingleTargetPixels();

                PlotGaussian();

                return true;
            }
            finally
            {
                nudFitMatrixSize.ValueChanged += nudFitMatrixSize_ValueChanged;
            }       
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
		private void PlotSingleTargetPixels()
		{
			Bitmap bmp = picTarget1Pixels.Image as Bitmap;
            if (bmp != null && m_ProcessingPixels != null)
			{

				//int pixelsCenterX = (int)Math.Round(m_Gaussian.X0_Matrix);
				//int pixelsCenterY = (int)Math.Round(m_Gaussian.Y0_Matrix);

				//byte[,] diplayPixels = Pixelmap.PixelsToBitmapBytes(m_ProcessingPixels, VideoContext.Current.Pixelmap.DisplayBitmapConverter);

				byte peak = 0;
				for (int x = 0; x < 17; x++)
					for (int y = 0; y < 17; y++)
					{
						if (peak < m_DisplayPixels[x, y])
							peak = m_DisplayPixels[x, y];
					}

				// This copes the pixels to a new array of pixels for displaying. This new array may have slightly different
				// dimentions (LP) and pixel intensities may be normalized (LPD)
				for (int x = 0; x < 17; x++)
					for (int y = 0; y < 17; y++)
					{
						byte pixelValue = m_DisplayPixels[x, y];

						Color pixelcolor = SystemColors.Control;

						if (pixelValue < TangraConfig.Settings.Photometry.Saturation.Saturation8Bit)
						{
							if (SelectedFilter == TangraConfig.PreProcessingFilter.LowPassDifferenceFilter)
							{
								pixelcolor = Color.FromArgb(150 * pixelValue / peak, 150 * pixelValue / peak, 150 * pixelValue / peak);
							}
                            else if (SelectedFilter == TangraConfig.PreProcessingFilter.LowPassFilter)
							{
								if (x >= 1 && x < 16 && y >= 1 && y < 16)
									pixelcolor = Color.FromArgb(pixelValue, pixelValue, pixelValue);
							}
							else
								pixelcolor = Color.FromArgb(pixelValue, pixelValue, pixelValue);
						}
						else
							pixelcolor = TangraConfig.Settings.Color.Saturation;


						for (int dx = 0; dx < 7; dx++)
							for (int dy = 0; dy < 7; dy++)
							{
								bmp.SetPixel(7 * x + dx, 7 * y + dy, pixelcolor);
							}
					}

				using (Graphics g = Graphics.FromImage(bmp))
				{
					float radius = (float)m_Aperture * 7;

					float x0 = m_X0 + dx + 0.5f;
                    float y0 = m_Y0 + dy + 0.5f;

					g.DrawEllipse(
						m_Pen,
						(float)(7 * (x0) - radius),
						(float)(7 * (y0) - radius),
						2 * radius,
						2 * radius);

					if (SelectedObjectType == TrackingType.GuidingStar && !pnlUDLR.Visible)
                    {
                        float halfMatrixSize = (int)nudFitMatrixSize.Value / 2.0f;
                        g.DrawRectangle(m_Pen,
                            7 * (x0 - halfMatrixSize),
                            7 * (y0 - halfMatrixSize),
                            14 * halfMatrixSize,
                            14 * halfMatrixSize);
                    }
					else if (SelectedObjectType == TrackingType.OccultedStar)
                    {
                        radius = (float)(m_Aperture + (float)nudPositionTolerance.Value) * 7;
                        g.DrawEllipse(
                            m_Pen,
                            (float)(7 * (x0) - radius),
                            (float)(7 * (y0) - radius),
                            2 * radius,
                            2 * radius);
                    }

					// TODO: Draw the background annulus

					// TODO: Autostars do not work very well (false positives are returned). Make them work and then enable the code below
					//if (m_AutoStarsInArea != null &&
					//    m_AutoStarsInArea.Count > 1)
					//{
					//    foreach(PSFFit fit in m_AutoStarsInArea)
					//    {
					//        g.DrawLine(m_Pen, 7f * (float)(fit.XCenter + 0.5 - 1), 7f * (float)(fit.YCenter + 0.5), 7f * (float)(fit.XCenter + 0.5 + 1), 7f * (float)(fit.YCenter + 0.5));
					//        g.DrawLine(m_Pen, 7f * (float)(fit.XCenter + 0.5), 7f * (float)(fit.YCenter + 0.5 - 1), 7f * (float)(fit.XCenter + 0.5), 7f * (float)(fit.YCenter + 0.5 + 1));
					//    }
					//}
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
				string.Format("{0} px = {1} FWHM", m_Aperture.ToString("0.00"),(m_Aperture / m_FWHM).ToString("0.00"));

			picTarget1PSF.Refresh();
		}

		private void nudAperture1_ValueChanged(object sender, EventArgs e)
		{
			m_Aperture = (float)nudAperture1.Value;
			PlotSingleTargetPixels();
			PlotGaussian();
		}

		private float dx, dy;
		private void btnLeft_Click(object sender, EventArgs e)
        {
		    bool shiftHeld = Control.ModifierKeys == Keys.Control;
            dx -= shiftHeld ? 0.02f : 0.1f;
			PlotSingleTargetPixels();
		}

		private void btnRight_Click(object sender, EventArgs e)
		{
            bool shiftHeld = Control.ModifierKeys == Keys.Control;
            dx += shiftHeld ? 0.02f : 0.1f;
			PlotSingleTargetPixels();
		}

		private void btnUp_Click(object sender, EventArgs e)
		{
            bool shiftHeld = Control.ModifierKeys == Keys.Control;
            dy -= shiftHeld ? 0.02f : 0.1f;
			PlotSingleTargetPixels();
		}

		private void btnDown_Click(object sender, EventArgs e)
		{
            bool shiftHeld = Control.ModifierKeys == Keys.Control;
            dy += shiftHeld ? 0.02f : 0.1f;
			PlotSingleTargetPixels();
		}

        private void CopyObjectToAdd()
        {
            ObjectToAdd.ApertureInPixels = (float)nudAperture1.Value;
            ObjectToAdd.MeasureThisObject = true; /* We measure all objects (comparison or occulted), unless specified otherwise by the user */
            ObjectToAdd.ApertureMatrixX0 = m_X0;
            ObjectToAdd.ApertureMatrixY0 = m_Y0;
            ObjectToAdd.ApertureDX = 0;
            ObjectToAdd.ApertureDY = 0;

            ObjectToAdd.AutoStarsInArea.Clear();
            if (m_AutoStarsInLargerArea != null)
            {
                m_AutoStarsInLargerArea.ForEach((g) => g.SetNewFieldCenterFrom35PixMatrix(m_Center.X, m_Center.Y));
                ObjectToAdd.AutoStarsInArea.AddRange(m_AutoStarsInLargerArea);
            }

            ObjectToAdd.OriginalFieldCenterX = m_Center.X;
            ObjectToAdd.OriginalFieldCenterY = m_Center.Y;

            if (rbManuallyPositionedAperture.Checked)
            {
                ObjectToAdd.IsFixedAperture = true;
                ObjectToAdd.ApertureStartingX = (float)m_Center.X + m_X0 - 8f + dx;
                ObjectToAdd.ApertureStartingY = (float)m_Center.Y + m_Y0 - 8f + dy;

                ObjectToAdd.ApertureDX = dx;
                ObjectToAdd.ApertureDY = dy;
            }
            else
            {
                if (m_Gaussian != null)
                    m_Gaussian.SetNewFieldCenterFrom17PixMatrix(m_Center.X, m_Center.Y);
                else
                    ObjectToAdd.IsWeakSignalObject = true;

                ObjectToAdd.Gaussian = m_Gaussian;
                if (m_Gaussian != null)
                {
                    ObjectToAdd.ApertureStartingX = (float)m_Gaussian.XCenter;
                    ObjectToAdd.ApertureStartingY = (float)m_Gaussian.YCenter;   
                }
                else
                {
                    // Forced guiding star
                    ObjectToAdd.ApertureStartingX = (float)m_Center.X;
                    ObjectToAdd.ApertureStartingY = (float)m_Center.Y; 
                }
            }

			if (SelectedObjectType == TrackingType.GuidingStar)
            {
                ObjectToAdd.TrackingType = TrackingType.GuidingStar;
                ObjectToAdd.PsfFitMatrixSize = (int)nudFitMatrixSize.Value;
            	ObjectToAdd.IsWeakSignalObject = false; // Must have a fit for a guiding star
            }
			else if (SelectedObjectType == TrackingType.ComparisonStar)
            {
                ObjectToAdd.TrackingType = TrackingType.ComparisonStar;
                ObjectToAdd.PsfFitMatrixSize = TangraConfig.Settings.Special.DefaultComparisonStarPsfFitMatrixSize;
            }
			else if (SelectedObjectType == TrackingType.OccultedStar)
            {
                ObjectToAdd.TrackingType = TrackingType.OccultedStar;
                ObjectToAdd.PositionTolerance = (float)nudPositionTolerance.Value;
                ObjectToAdd.PsfFitMatrixSize = TangraConfig.Settings.Special.DefaultOccultedStarPsfFitMatrixSize;
            	ObjectToAdd.IsWeakSignalObject = rbAutoCenteredAperture.Checked;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            CopyObjectToAdd();

            //if (LightCurveReductionContext.Instance.SaveTrackingSession)
            //    LightCurveReductionContext.Instance.SessionFile.AddOrEditObject(m_AstroImage, ObjectToAdd, m_ObjectId);

            DialogResult = DialogResult.OK;
            Close();
        }

	    private int m_MatrixSize = 0;

        private void nudFitMatrixSize_ValueChanged(object sender, EventArgs e)
        {
            if (m_Gaussian != null)
            {
                int matirxSize = (int)nudFitMatrixSize.Value;
                if (GetFitInMatrix(m_Gaussian, ref matirxSize))
                {
                    m_MatrixSize = matirxSize;
                }

                nudFitMatrixSize.ValueChanged -= nudFitMatrixSize_ValueChanged;
                try
                {
                    nudFitMatrixSize.Value = m_MatrixSize;
                }
                finally
                {
                    nudFitMatrixSize.ValueChanged += nudFitMatrixSize_ValueChanged;
                }

                // The matrix size has changed manually, go and plot new settings
                PlotGaussian();
                PlotSingleTargetPixels();
            }
            else if (!m_IsBrightEnoughForAutoGuidingStar)
            {
                // If there is no Gaussian because of a bad fit, then if the user wants to use the star as a gusing star
                // we still want to use the changed matrix fit area and plot it
                PlotSingleTargetPixels();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            CopyObjectToAdd();

            //if (LightCurveReductionContext.Instance.SaveTrackingSession)
            //    LightCurveReductionContext.Instance.SessionFile.DeleteObject(ObjectToAdd);

            DialogResult = DialogResult.Abort;
            Close();
        }

        private void ToggleSelectedRadioButton(TrackingType newTrackingType)
        {
			rbOccultedStar.CheckedChanged -= rgObjectType_SelectedIndexChanged;
            rbGuidingStar.CheckedChanged -= rgObjectType_SelectedIndexChanged;
            rbComparisonStar.CheckedChanged -= rgObjectType_SelectedIndexChanged;

            try
            {
            	SelectedObjectType = newTrackingType;
            }
            finally
            {
                rbComparisonStar.CheckedChanged += rgObjectType_SelectedIndexChanged;
                rbGuidingStar.CheckedChanged += rgObjectType_SelectedIndexChanged;
                rbOccultedStar.CheckedChanged += rgObjectType_SelectedIndexChanged;
            }
        }

        private void rbFixedAperture_CheckedChanged(object sender, EventArgs e)
        {
            if (rbManuallyPositionedAperture.Checked)
            {
                pnlUDLR.Visible = true;
                pnlAutoFitSize.Enabled = false;
                if (!m_IsBrightEnoughForAutoGuidingStar)
                    rbGuidingStar.Text = "Guiding Star";
                else
					rbGuidingStar.Text = "Comparison Star";
                pnlUDLR.BringToFront();
            }
            else
            {
                pnlUDLR.Visible = false;
                pnlAutoFitSize.Enabled = true;
                if (!m_IsBrightEnoughForAutoGuidingStar)
					rbGuidingStar.Text = "Guiding Star";
                else
					rbGuidingStar.Text = "Guiding/Comparison Star";
                pnlUDLR.SendToBack();                
            }

            dx = 0; dy = 0;
            PlotSingleTargetPixels();
        }

        private void nudPositionTolerance_ValueChanged(object sender, EventArgs e)
        {
            PlotSingleTargetPixels();
        }


	    private PSFFit newStar = null;
        private void picTarget1Pixels_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_AutoStarsInArea != null &&
                m_AutoStarsInArea.Count > 1)
            {
                float x = e.X / 7.0f;
                float y = e.Y / 7.0f;

                PSFFit star = m_AutoStarsInArea.Find(
                    s =>
                    {
                        //Trace.WriteLine(string.Format("TESTED-STAR({0}, {1}) -> MOUSE:({2}, {3}); RESIDUAL({4};{5})",
                        //    (s.YCenter + 0.5).ToString("0.0"), (s.XCenter + 0.5).ToString("0.0"),
                        //    x.ToString("0.0"), y.ToString("0.0"),
                        //    Math.Abs(s.XCenter - x + 0.5f).ToString("0.0"), Math.Abs(s.YCenter - y + 0.5).ToString("0.0")));

                        return Math.Abs(s.XCenter - x + 0.5f) < 2.0f && Math.Abs(s.YCenter - y + 0.5) < 2.0f;
                    }
                    );

                if (star != null &&
                    m_Gaussian.UniqueId != star.UniqueId)
                {
                    newStar = star;
                    Cursor = Cursors.Hand;
                }
                else
                    Cursor = Cursors.Default;
            }
        }

        private void picTarget1Pixels_Click(object sender, EventArgs e)
        {
            if (newStar != null)
            {
                m_Gaussian = newStar;
                m_X0 = (float)newStar.XCenter;
                m_Y0 = (float)newStar.YCenter;                
                m_FWHM = (float)newStar.FWHM;

                nudFitMatrixSize.ValueChanged -= nudFitMatrixSize_ValueChanged;
                try
                {
                    nudFitMatrixSize.Value = newStar.MatrixSize;
                }
                finally
                {
                    nudFitMatrixSize.ValueChanged += nudFitMatrixSize_ValueChanged;    
                }
                PlotSingleTargetPixels();
                PlotGaussian();
            }
        }

        internal bool IsBrightEnoughtForGuidingStar()
        {
            double signal;
            double sn;

            // TODO: Compute the noise as the average noise from 4 frames 

            // To compute the 'noise' we get the (TimesStdDevsForBackgroundNoise) * sigma background fluctuation excluding 1FWHM area around the center
            byte oneSigmaBg = 0;
            int from = 0;
            int to = m_ProcessingPixels.GetLength(0);
            double aboveMedianLevelRequired;
            if (LightCurveReductionContext.Instance.DigitalFilter != TangraConfig.PreProcessingFilter.NoFilter)
            {
                from++;
                to--;
            }
            switch(LightCurveReductionContext.Instance.DigitalFilter)
            {
                case TangraConfig.PreProcessingFilter.LowPassFilter:
                    aboveMedianLevelRequired = TangraConfig.Settings.Special.AboveMedianThreasholdForGuiding * TangraConfig.Settings.Special.AboveMedianCoeffLP;
                    break;
                case TangraConfig.PreProcessingFilter.LowPassDifferenceFilter:
                    aboveMedianLevelRequired = TangraConfig.Settings.Special.AboveMedianThreasholdForGuiding * TangraConfig.Settings.Special.AboveMedianCoeffLPD;
                    break;
                default:
                    aboveMedianLevelRequired = TangraConfig.Settings.Special.AboveMedianThreasholdForGuiding;
                    break;
            }

			List<uint> bgBytes = new List<uint>();
            for (int x = from; x < to; x++)
            {
                for (int y = from; y < to; y++)
                {
                    double distanceFromFit = ImagePixel.ComputeDistance(m_Gaussian.XCenter, x, m_Gaussian.YCenter, y);
                    if (distanceFromFit > m_Gaussian.FWHM)
                        bgBytes.Add(m_ProcessingPixels[x, y]);
                }
            }
            bgBytes.Sort();
            uint median = 0;
			if (bgBytes.Count > 0)
			{
				median = bgBytes.Count % 2 == 1
								  ? bgBytes[bgBytes.Count / 2]
								  : ((bgBytes[bgBytes.Count / 2] + bgBytes[(bgBytes.Count / 2) - 1]) / 2);				
			}

            double var = 0;
            bgBytes.ForEach(b => var += (median - b) * (median - b));
			oneSigmaBg = (byte)Math.Round(Math.Sqrt(var / Math.Max(1, bgBytes.Count - 1)));

            signal = m_Gaussian.IMax - median;
			sn = signal / oneSigmaBg;

            Trace.WriteLine(string.Format("Signal = {0}, S/N = {1}", signal.ToString("0.00"), sn.ToString("0.00")));

            if (signal >= aboveMedianLevelRequired &&
                sn >= TangraConfig.Settings.Special.SignalNoiseForGuiding)
            {
                return true;
            }

            Trace.WriteLine(string.Format("Not good for a guiding star: Required Signal = {0}, Required S/N = {1}",
                aboveMedianLevelRequired.ToString("0.00"), 
                TangraConfig.Settings.Special.SignalNoiseForGuiding.ToString("0.00")));

            return false;
        }

        private void btnExplain_Click(object sender, EventArgs e)
        {
//#if !PRODUCTION

//            bool ctrlHolded = Control.ModifierKeys == Keys.Control;

//            ObjectFinderStruct str = new ObjectFinderStruct(
//                m_Center, ctrlHolded 
//                    ? m_AstroImage.GetMeasurableAreaPixels(m_Center.X, m_Center.Y, 35)
//                    : m_ProcessingPixels,
//                m_AstroImage.MedianNoise, LightCurveReductionContext.Instance.DigitalFilter);
//            frmObjectFinder frm = new frmObjectFinder(str.AsBase64String());
//            frm.ShowDialog();
//#endif
        }

		private void rgObjectType_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (SelectedObjectType == TrackingType.GuidingStar)
            {
                if (!m_IsBrightEnoughForAutoGuidingStar &&
                    TangraConfig.Settings.Tracking.WarnOnUnsatisfiedGuidingRequirements &&
                    MessageBox.Show(this.ParentForm,
                                    "This object doesn't seem to be suitable for a guiding star. It may be too faint, with a too low S/N ratio, too large, overexposed or close to another object. If the problem is a low S/N ratio then a software integration is recommended, otherwise in many cases you may be still able to use the object for guiding. " +
                                    "\r\n\r\nDo you want to continue and use this star as a guiding star?",
                                    "Warning",
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
                {
					ToggleSelectedRadioButton(TrackingType.ComparisonStar);
                }
                else
					ToggleSelectedRadioButton(TrackingType.GuidingStar);
            }

			if (SelectedObjectType == TrackingType.OccultedStar)
			{
				ToggleSelectedRadioButton(TrackingType.OccultedStar);

				if (ObjectToAdd.PositionTolerance == 0)
				{

					// only update tollerance if this is a new object / not been saved as occulted star
					double tolerance = 1;
					if (m_Gaussian != null) tolerance = m_Gaussian.FWHM * TangraConfig.Settings.Special.ToleranceFWHMCoeff;
					if (tolerance < TangraConfig.Settings.Special.ToleranceMinValueFullDisappearance) tolerance = TangraConfig.Settings.Special.ToleranceMinValueFullDisappearance;
					if (LightCurveReductionContext.Instance.WindOrShaking && LightCurveReductionContext.Instance.HighFlickering)
						tolerance = TangraConfig.Settings.Special.ToleranceTwoBadnessCoeff * tolerance;
					else if (LightCurveReductionContext.Instance.HighFlickering || LightCurveReductionContext.Instance.HighFlickering)
						tolerance = TangraConfig.Settings.Special.ToleranceOneBadnessCoeff * tolerance;

					if (!LightCurveReductionContext.Instance.FullDisappearance)
					{
						if (tolerance < TangraConfig.Settings.Special.ToleranceMinValue) tolerance = TangraConfig.Settings.Special.ToleranceMinValue;
					}
					if (tolerance > TangraConfig.Settings.Special.ToleranceMaxValue) tolerance = TangraConfig.Settings.Special.ToleranceMaxValue;
					nudPositionTolerance.Value = (decimal)tolerance;
				}
			}

			pnlAutoFitSize.Enabled = SelectedObjectType == TrackingType.GuidingStar;
			pnlTolerance.Enabled = SelectedObjectType == TrackingType.OccultedStar;

			m_AutocenteredApertureAvailable = true;

			if (SelectedObjectType == TrackingType.GuidingStar)
            {
            	// If a guiding star is selected then it is always an autocentered aperture
				rbAutoCenteredAperture.Checked = true;
				m_AutocenteredApertureAvailable = false;
            }
			else if (!m_AutocenteredApertureAvailable)
                // otherwise if 'autocentered' is not available change to manual aperture
                rbManuallyPositionedAperture.Checked = true;

            PlotSingleTargetPixels();

		}

		private TrackingType SelectedObjectType
		{
			get
			{
				if (rbGuidingStar.Checked)
					return TrackingType.GuidingStar;

                if (rbOccultedStar.Checked)
                    return TrackingType.OccultedStar;
                else
                    return
                        TrackingType.ComparisonStar;
			}
			set
			{
				if (value == TrackingType.GuidingStar)
                    rbGuidingStar.Checked = true;
				else
				{
                    if (value == TrackingType.ComparisonStar && rbComparisonStar.Visible)
                        rbComparisonStar.Checked = true;
					else if (value == TrackingType.OccultedStar)
					{
						rbOccultedStar.Checked = true;
					}
				}
			}
		}

	}
}
