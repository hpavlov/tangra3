/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Config;
using Tangra.Controller;
using Tangra.Helpers;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Model.Context;
using Tangra.Model.Helpers;
using Tangra.Model.Image;
using Tangra.Model.Video;
using Tangra.MotionFitting;
using Tangra.SDK;
using Tangra.StarCatalogues;
using Tangra.Video;
using Tangra.VideoOperations.Astrometry.Engine;
using Tangra.VideoOperations.LightCurves;

namespace Tangra.VideoOperations.Astrometry
{
	internal partial class frmRunMultiFrameMeasurements : Form
	{
		private VideoAstrometryOperation m_VideoAstrometry;
		private MeasurementContext m_MeasurementContext;
	    private FieldSolveContext m_FieldSolveContext;
		private VideoController m_VideoController;
	    private AstrometryController m_AstrometryController;
		private AddinsController m_AddinsController;

        private List<ITangraAddinAction> m_AstrometryAddinActions = new List<ITangraAddinAction>();
        private List<ITangraAddin> m_AstrometryAddins = new List<ITangraAddin>();

		private bool m_AavStacking;
		private bool m_AavIntegration;
		private VideoFileFormat m_VideoFileFormat;
	    private string m_NativeFormat;
		private bool m_Initialised;

        internal frmRunMultiFrameMeasurements(
			VideoController videoController, 
            AstrometryController astrometryController,
            AddinsController addinsController, 
            VideoAstrometryOperation astrometry, 
            MeasurementContext measurementContext, 
            FieldSolveContext fieldSolveContext,
            out List<ITangraAddinAction> astrometryAddinActions, 
            out List<ITangraAddin> astrometryAddins)
		{
			InitializeComponent();

			pboxAperturePreview.Image = new Bitmap(pboxAperturePreview.Width, pboxAperturePreview.Height, PixelFormat.Format24bppRgb);

			m_VideoController = videoController;
	        m_AddinsController = addinsController;
            m_AstrometryController = astrometryController;

			m_VideoAstrometry = astrometry;
			m_MeasurementContext = measurementContext;
            m_FieldSolveContext = fieldSolveContext;          

            SyncTimeStampControlWithExpectedFrameTime();
			nudMinStars.SetNUDValue(TangraConfig.Settings.Astrometry.MinimumNumberOfStars);

			m_MeasurementContext.MaxMeasurements = 200;

			#region Configure the Reduction Settings. The same must be done in the frmConfigureReprocessing and frmSelectReductionType
			// Removes the Background Gradient
			cbxBackgroundMethod.Items.RemoveAt(2);
			#endregion

			SetComboboxIndexFromPhotometryReductionMethod(TangraConfig.Settings.LastUsed.AstrometryPhotometryReductionMethod);
			SetComboboxIndexFromBackgroundMethod(TangraConfig.Settings.LastUsed.AstrometryPhotometryBackgroundMethod);
			cbxFitMagnitudes.Checked = TangraConfig.Settings.LastUsed.AstrometryFitMagnitudes;


            if (TangraConfig.Settings.LastUsed.AstrometryMagFitAperture.HasValue &&
                TangraConfig.Settings.LastUsed.AstrometryMagFitGap.HasValue &&
                TangraConfig.Settings.LastUsed.AstrometryMagFitAnnulus.HasValue)
            {
                nudAperture.ValueChanged -= nudAperture_ValueChanged;
                try
                {
                    nudAperture.SetNUDValue(TangraConfig.Settings.LastUsed.AstrometryMagFitAperture.Value);
                    nudGap.SetNUDValue(TangraConfig.Settings.LastUsed.AstrometryMagFitGap.Value);
                    nudAnnulus.SetNUDValue(TangraConfig.Settings.LastUsed.AstrometryMagFitAnnulus.Value);
                }
                finally
                {
                    nudAperture.ValueChanged += nudAperture_ValueChanged;
                }
            }
            else
                ResetAperture();

			cbxOutputMagBand.SelectedIndex = (int)TangraConfig.Settings.Astrometry.DefaultMagOutputBand;
            
            lblCatBand.Text = string.Format("Magnitude Band for Photometry (from {0})", m_FieldSolveContext.StarCatalogueFacade.CatalogNETCode);
            cbxCatalogPhotometryBand.Items.Clear();
            cbxCatalogPhotometryBand.Items.AddRange(m_FieldSolveContext.StarCatalogueFacade.CatalogMagnitudeBands);
            CatalogMagnitudeBand defaultBand = cbxCatalogPhotometryBand.Items.Cast<CatalogMagnitudeBand>().FirstOrDefault(e => e.Id == TangraConfig.Settings.StarCatalogue.CatalogMagnitudeBandId);
            if (defaultBand != null)
                cbxCatalogPhotometryBand.SelectedItem = defaultBand;
            else
                cbxCatalogPhotometryBand.SelectedIndex = 0;

        	cbxExpectedMotion.SelectedIndex = 1;
        	cbxSignalType.Visible = false;
        	cbxSignalType.SelectedIndex = 0;
        	cbxFrameTimeType.SelectedIndex = 0;
        	pnlIntegration.Visible = false;
        	cbxInstDelayUnit.SelectedIndex = 0;

	        m_AstrometryAddinActions = m_AddinsController.GetAstrometryActions(out m_AstrometryAddins);

            foreach (ITangraAddinAction action in m_AstrometryAddinActions)
            {
                clbAddinsToRun.Items.Add(new AddinActionEntry(action));
            }
            astrometryAddinActions = m_AstrometryAddinActions;
            astrometryAddins = m_AstrometryAddins;

	        cbxInstDelayCamera.Items.Clear();
	        cbxInstDelayCamera.Items.Add(string.Empty);
	        cbxInstDelayCamera.Items.AddRange(InstrumentalDelayConfigManager.GetAvailableCameras().ToArray());

	        m_Initialised = true;
		}

	    private void ResetAperture()
	    {
            if (TangraConfig.Settings.Photometry.SignalApertureUnitDefault == TangraConfig.SignalApertureUnit.FWHM)
            {
                if (AstrometryContext.Current.CurrentPhotometricFit != null &&
                    AstrometryContext.Current.CurrentPhotometricFit.PSFGaussians.Count > 3)
                {
                    double medianFwhm = AstrometryContext.Current.CurrentPhotometricFit.PSFGaussians.Select(x => x.FWHM).ToList().Median();
                    nudAperture.Value = (decimal)(medianFwhm * TangraConfig.Settings.Photometry.DefaultSignalAperture);
                }
                else
                    nudAperture.Value = 4;
            }
            else
                nudAperture.Value = (decimal)TangraConfig.Settings.Photometry.DefaultSignalAperture;
	    }

        internal class AddinActionEntry
        {
            internal ITangraAddinAction Action;

            internal AddinActionEntry(ITangraAddinAction action)
            {
                Action = action;
            }

            public override string ToString()
            {
                return Action.DisplayName ?? "Unknown";
            }
        }

        private void SyncTimeStampControlWithExpectedFrameTime()
        {
	        VideoFileFormat format = m_VideoController.GetVideoFileFormat();
			if (format == VideoFileFormat.AVI ||
                format == VideoFileFormat.AAV || /* Old AAV files with manually entered timestamps */
                (format == VideoFileFormat.AAV2 && !m_VideoController.HasEmbeddedTimeStamps()))
			{
				double milisecondsDiff =
					(m_VideoController.CurrentFrameIndex - AstrometryContext.Current.FieldSolveContext.FrameNoOfUtcTime) * 1000.0 / m_VideoController.VideoFrameRate;

				ucUtcTimePicker.DateTimeUtc = AstrometryContext.Current.FieldSolveContext.UtcTime.AddMilliseconds(milisecondsDiff);
			    if (m_VideoController.HasTimestampOCR())
                    m_VideoController.AssertOCRTimestamp(ucUtcTimePicker.DateTimeUtc, true);
            }
			else
			{
                ucUtcTimePicker.DateTimeUtc = m_VideoController.GetBestGuessDateTimeForCurrentFrame();
			}

            if (format == VideoFileFormat.AAV || /* Old AAV files with manually entered timestamps */
                (format == VideoFileFormat.AAV2 && !m_VideoController.HasEmbeddedTimeStamps()))
            {
                // Refresh the current frame to split the AAV timestamp for manual validation
                // NOTE: This assumes the Split option is selected in the settings (which will be by default)
                m_VideoController.RefreshCurrentFrame();
            }
        }

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}

			base.Dispose(disposing);
		}


		private void frmRunMultiFrameMeasurements_Load(object sender, EventArgs e)
		{
			cbxExpectedMotion.SelectedIndex = (int)m_MeasurementContext.MovementExpectation;
			cbxSignalType.SelectedIndex = (int)m_MeasurementContext.ObjectExposureQuality;
			cbxFrameTimeType.SelectedIndex = (int)m_MeasurementContext.FrameTimeType;
			nudInstrDelay.SetNUDValue(m_MeasurementContext.InstrumentalDelay);
			cbxInstDelayUnit.SelectedIndex = (int)m_MeasurementContext.InstrumentalDelayUnits;
			nudIntegratedFrames.SetNUDValue(m_MeasurementContext.IntegratedFramesCount);

            nudInstrDelay.Enabled = true;
            cbxInstDelayUnit.Enabled = true;
		    nudIntegratedFrames.Enabled = true;
            btnDetectIntegration.Enabled = true;

			cbxInstDelayCamera.SelectedIndex = cbxInstDelayCamera.Items.IndexOf(TangraConfig.Settings.PlateSolve.SelectedCameraModel);
			cbxInstDelayMode.SelectedIndex = 0; // Automatic
			if (cbxInstDelayCamera.SelectedIndex == -1)
			{
				cbxInstDelayCamera.SelectedIndex = 0; // No camera selected
				cbxInstDelayMode.SelectedIndex = 1; // Manual
			}

			m_AavStacking = false;
			m_AavIntegration = false;
			m_VideoFileFormat = m_VideoController.GetVideoFileFormat();
			if (m_VideoFileFormat != VideoFileFormat.AVI)
			{
				cbxFrameTimeType.SelectedIndex = 0;
				cbxFrameTimeType.Enabled = false;

				DateTime? timeStamp = m_VideoController.GetCurrentFrameTime();
				if (timeStamp != null && timeStamp != DateTime.MinValue)
				{
					DateTime timestamp = timeStamp.Value;

                    if (m_VideoFileFormat.IsAAV())
					{
                        m_NativeFormat = m_VideoController.GetVideoFormat(m_VideoFileFormat);

						// Compute the first timestamp value
					    if (m_VideoController.HasTimestampOCR())
					    {
					        timestamp = m_FieldSolveContext.UtcTime.Date.Add(timestamp.TimeOfDay);
					    }
					    else
					    {
                            FrameStateData frameState = m_VideoController.GetCurrentFrameState();
                            timestamp = timestamp.AddMilliseconds(-0.5 * frameState.ExposureInMilliseconds);

                            if (m_NativeFormat == "PAL") timestamp = timestamp.AddMilliseconds(20);
                            else if (m_NativeFormat == "NTSC") timestamp = timestamp.AddMilliseconds(16.68);
					    }
					}

					ucUtcTimePicker.DateTimeUtc = timestamp;
					ucUtcTimePicker.Enabled = false; // The video has embedded timestamp so the users should not enter times manually
				}

                if (m_VideoFileFormat.IsAAV())
				{
					nudIntegratedFrames.Enabled = false;

					if (m_VideoController.AstroAnalogueVideoStackedFrameRate > 0)
					{
						cbxFrameTimeType.Items[0] = "AAV stacked frame";
						pnlIntegration.Visible = true;
						btnDetectIntegration.Visible = false;
						lblFrames.Text = string.Format("frames (stacking: {0} frames)", m_VideoController.AstroAnalogueVideoStackedFrameRate);
						m_AavStacking = true;
						nudIntegratedFrames.Value = m_VideoController.AstroAnalogueVideoIntegratedAAVFrames;
					}
					else if (m_VideoController.AstroAnalogueVideoIntegratedAAVFrames > 0)
					{
						cbxFrameTimeType.Items[0] = "AAV integrated frame";
						pnlIntegration.Visible = true;
						btnDetectIntegration.Visible = false;
						nudIntegratedFrames.Value = m_VideoController.AstroAnalogueVideoIntegratedAAVFrames;
						m_AavIntegration = true;
					}
				}
			}
            else
                // This will be set in the exported CSV file and will then be used to identify repeated measurements from integrated AAV files
                m_NativeFormat = "AVI-Integrated";

			activePage = 0;
			DisplayActivePage();

			// Allow time corrections to be set automatically even when the integration is not recognized ("use suggested value" button) or something
		}

		private void btnStartStop_Click(object sender, EventArgs e)
		{
            if (activePage == 3 || (activePage == 2 && m_AstrometryAddinActions.Count == 0))
				StartMeasurements();
			else
			{
				if (activePage == 0)
				{
					if ((MovementExpectation)cbxExpectedMotion.SelectedIndex == MovementExpectation.FastFlyby)
					{
						if ((ObjectExposureQuality)cbxSignalType.SelectedIndex == ObjectExposureQuality.Underexposed)
						{
							MessageBox.Show(this,
								"This measurement is not supported yet. If you are interested in measuring a video of underexposed NEA and can provide this video for testing, please contact the author of Tangra.",
								"Not Supported", MessageBoxButtons.OK, MessageBoxIcon.Stop);

							return;
						}
						else if ((ObjectExposureQuality)cbxSignalType.SelectedIndex == ObjectExposureQuality.Trailed)
						{
							MessageBox.Show(this,
								"This measurement is not supported yet. If you are interested in measuring a video of trailed NEA and can provide this video for testing, please contact the author of Tangra.",
								"Not Supported", MessageBoxButtons.OK, MessageBoxIcon.Stop);

							return;
						}
					}

				    if (cbxInstDelayCamera.SelectedIndex == 0 && cbxInstDelayMode.SelectedIndex == 0)
				    {
                        MessageBox.Show(this,
                                "Please configure the instrumental delay manually by selecting 'Manual' mode and then choose the camera model or configure the instrumental delay in seconds directly.",
                                "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Stop);

                        return;
				    }

                    if (nudIntegratedFrames.Value > 1 && m_VideoFileFormat == VideoFileFormat.AVI && (MovementExpectation)cbxExpectedMotion.SelectedIndex != MovementExpectation.Slow)
				    {
				        if (MessageBox.Show(this,
				            "To measure AVI files of fly-bys recorded with integrated video cameras it is required the file format to be converted to AAV first using\r\n\r\nFile -> Convert Video to AAV\r\n\r\nfrom the Tangra's menu. Press 'Yes' to go back and then cancel the operation and convert the video to AAV format before measuring it again.\r\n\r\nIf you press 'No' the individual positions will be measured but Tangra will not be able to derive high precision astrometry from the fast motion.",
				            "Tangra", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
				        {
                            return;
				        }
				    }

					ucFrameInterval.Recalculate();
				}

				activePage++;
				DisplayActivePage();
			}
		}

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (activePage == 0)
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
            else
            {
                activePage--;
                DisplayActivePage();
            }
        }

		private void StartMeasurements()
		{
            #region Remove the unchecked actions
            if (m_AstrometryAddinActions.Count > 0)
            {
                m_AstrometryAddinActions.Clear();

                foreach (AddinActionEntry selectedAction in clbAddinsToRun.CheckedItems)
                {
                    m_AstrometryAddinActions.Add(selectedAction.Action);
                }
                List<string> executingActions = m_AstrometryAddinActions
                       .Select(a => a.DisplayName)
                       .ToList();

                for (int i = m_AstrometryAddins.Count - 1; i >= 0; i--)
                {
                    ITangraAddin addin = m_AstrometryAddins[i];
                    ITangraAddinAction[] actions =  addin.GetAddinActions();
                    bool found = false;
                    foreach(ITangraAddinAction action in actions)
                    {
                        if (executingActions.IndexOf(action.DisplayName) > -1)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found) m_AstrometryAddins.RemoveAt(i);
                }
            }
            #endregion

		    var plateConfig = m_AstrometryController.GetCurrentAstroPlate();

            double pixArcsecFactor = plateConfig.GetDistanceInArcSec(
                    plateConfig.CenterXImage, plateConfig.CenterYImage,
                    plateConfig.CenterXImage + 1, plateConfig.CenterYImage + 1);

            m_MeasurementContext.MaxStdDev = pixArcsecFactor * (double)nudMaxStdDev.Value;
			m_MeasurementContext.FrameInterval = ucFrameInterval.Value;
			m_MeasurementContext.FirstFrameUtcTime = ucUtcTimePicker.DateTimeUtc;
			m_MeasurementContext.PerformPhotometricFit = cbxFitMagnitudes.Checked;
			
			m_MeasurementContext.ApertureSize = (float)nudAperture.Value;
			m_MeasurementContext.AnnulusInnerRadius = ((float)nudGap.Value + (float)nudAperture.Value) / (float)nudAperture.Value;
			m_MeasurementContext.AnnulusMinPixels = (int)(Math.PI * (Math.Pow((float)nudAnnulus.Value + (float)nudGap.Value + (float)nudAperture.Value, 2) - Math.Pow((float)nudGap.Value + (float)nudAperture.Value, 2)));

			m_MeasurementContext.PhotometryReductionMethod = ComboboxIndexToPhotometryReductionMethod();
			m_MeasurementContext.PhotometryBackgroundMethod = ComboboxIndexToBackgroundMethod();
			m_MeasurementContext.ExportCSV = cbxExport.Checked;

			m_MeasurementContext.PhotometryMagOutputBand = (TangraConfig.MagOutputBand)cbxOutputMagBand.SelectedIndex;
			m_MeasurementContext.PhotometryCatalogBandId = ((CatalogMagnitudeBand)cbxCatalogPhotometryBand.SelectedItem).Id;
			m_MeasurementContext.StarCatalogueFacade = m_FieldSolveContext.StarCatalogueFacade;
			m_MeasurementContext.AssumedTargetVRColour = TangraConfig.Settings.Astrometry.AssumedTargetVRColour;

			m_MeasurementContext.StopOnNoFit = cbxStopOnNoFit.Checked;
			m_MeasurementContext.MovementExpectation = (MovementExpectation)cbxExpectedMotion.SelectedIndex;
			m_MeasurementContext.ObjectExposureQuality = (ObjectExposureQuality)cbxSignalType.SelectedIndex;
			m_MeasurementContext.FrameTimeType = (FrameTimeType)cbxFrameTimeType.SelectedIndex;
			m_MeasurementContext.InstrumentalDelay = (double)nudInstrDelay.Value;
			m_MeasurementContext.InstrumentalDelayUnits = (InstrumentalDelayUnits)cbxInstDelayUnit.SelectedIndex;
			m_MeasurementContext.IntegratedFramesCount = m_AavIntegration ? 1 : (int)nudIntegratedFrames.Value;
            m_MeasurementContext.IntegratedExposureSeconds = m_MeasurementContext.IntegratedFramesCount / m_VideoController.VideoFrameRate;
			m_MeasurementContext.AavIntegration = m_AavIntegration;
			m_MeasurementContext.AavStackedMode = m_AavStacking;
			m_MeasurementContext.VideoFileFormat = m_VideoFileFormat;
            m_MeasurementContext.NativeVideoFormat = m_NativeFormat;
			m_MeasurementContext.MaxMeasurements = (int)nudNumberMeasurements.Value;

			// TODO: Detect the integration automatically and position to the first frame of the next interval
			TangraConfig.Settings.LastUsed.AstrometryPhotometryReductionMethod = ComboboxIndexToPhotometryReductionMethod();
			TangraConfig.Settings.LastUsed.AstrometryPhotometryBackgroundMethod = ComboboxIndexToBackgroundMethod();
			TangraConfig.Settings.LastUsed.AstrometryFitMagnitudes = cbxFitMagnitudes.Checked;
			TangraConfig.Settings.LastUsed.LastAstrometryUTCDate = ucUtcTimePicker.DateTimeUtc;

		    if (cbxFitMagnitudes.Checked)
		    {
		        TangraConfig.Settings.LastUsed.AstrometryMagFitAperture = (float)nudAperture.Value;
                TangraConfig.Settings.LastUsed.AstrometryMagFitGap = (float)nudGap.Value;
                TangraConfig.Settings.LastUsed.AstrometryMagFitAnnulus = (float)nudAnnulus.Value;
		    }
			TangraConfig.Settings.Save();

			m_VideoAstrometry.StartAstrometricMeasurements(new ucAstrometryObjectInfo.StartAstrometricMeasurementsEventArgs(m_MeasurementContext));

			Close();
		}

		public TangraConfig.PhotometryReductionMethod ComboboxIndexToPhotometryReductionMethod()
		{
			if (cbxReductionType.SelectedIndex == 0)
				return TangraConfig.PhotometryReductionMethod.AperturePhotometry;
			else if (cbxReductionType.SelectedIndex == 1)
				return TangraConfig.PhotometryReductionMethod.PsfPhotometry;
			else if (cbxReductionType.SelectedIndex == 2)
				return TangraConfig.PhotometryReductionMethod.OptimalExtraction;
			else
				return TangraConfig.PhotometryReductionMethod.AperturePhotometry;
		}

		public void SetComboboxIndexFromPhotometryReductionMethod(TangraConfig.PhotometryReductionMethod method)
		{
			switch (method)
			{
				case TangraConfig.PhotometryReductionMethod.AperturePhotometry:
					cbxReductionType.SelectedIndex = 0;
					break;

				case TangraConfig.PhotometryReductionMethod.PsfPhotometry:
					cbxReductionType.SelectedIndex = 1;
					break;

				case TangraConfig.PhotometryReductionMethod.OptimalExtraction:
					cbxReductionType.SelectedIndex = 2;
					break;
			}
		}

		public TangraConfig.BackgroundMethod ComboboxIndexToBackgroundMethod()
		{
			if (cbxBackgroundMethod.SelectedIndex == 0)
				return TangraConfig.BackgroundMethod.AverageBackground;
			else if (cbxBackgroundMethod.SelectedIndex == 1)
				return TangraConfig.BackgroundMethod.BackgroundMode;
			else if (cbxBackgroundMethod.SelectedIndex == 2)
				return TangraConfig.BackgroundMethod.PSFBackground;
			else if (cbxBackgroundMethod.SelectedIndex == 3)
				return TangraConfig.BackgroundMethod.BackgroundMedian;
			else
				return TangraConfig.BackgroundMethod.AverageBackground;
		}

		public void SetComboboxIndexFromBackgroundMethod(TangraConfig.BackgroundMethod method)
		{
			switch (method)
			{
				case TangraConfig.BackgroundMethod.AverageBackground:
					cbxBackgroundMethod.SelectedIndex = 0;
					break;

				case TangraConfig.BackgroundMethod.BackgroundMode:
					cbxBackgroundMethod.SelectedIndex = 1;
					break;

				case TangraConfig.BackgroundMethod.PSFBackground:
					cbxBackgroundMethod.SelectedIndex = 2;
					break;

				case TangraConfig.BackgroundMethod.BackgroundMedian:
					cbxBackgroundMethod.SelectedIndex = 3;
					break;
			}
		}

		private void ucFrameInterval_FrameIntervalChanged(object sender, FrameIntervalChangedEventArgs e)
		{
			if (TangraContext.Current.HasVideoLoaded)
			{
				m_MeasurementContext.MaxMeasurements = m_VideoController.VideoCountFrames / e.FrameInterval;
				nudNumberMeasurements.Value = nudNumberMeasurements.Minimum;
				nudNumberMeasurements.Maximum = m_MeasurementContext.MaxMeasurements;
				nudNumberMeasurements.Value = Math.Min(200, m_MeasurementContext.MaxMeasurements);
			}
		}

		private void cbxFitMagnitudes_CheckedChanged(object sender, EventArgs e)
		{
			pnlPhotometryMethods.Enabled = cbxFitMagnitudes.Checked;
			pnlExportPhotometry.Enabled = cbxFitMagnitudes.Checked;
		}

		private void cbxExport_CheckedChanged(object sender, EventArgs e)
		{
			pnlExportConfig.Enabled = cbxExport.Checked;
		}

		private void cbxFrameTimeType_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (cbxFrameTimeType.SelectedIndex != 0)
			{
				pnlIntegration.Visible = true;

				nudIntegratedFrames.Enabled = true;
				btnDetectIntegration.Enabled = true;
			}
			else
			{
				pnlIntegration.Visible = false;

				nudIntegratedFrames.Enabled = false;
				btnDetectIntegration.Enabled = false;
			}

			SetInstDelayControlsState();
		}

		private void cbxExpectedMotion_SelectedIndexChanged(object sender, EventArgs e)
		{
			cbxSignalType.Visible = cbxExpectedMotion.SelectedIndex == 2;

			if (cbxExpectedMotion.SelectedIndex == 0)
				nudMaxStdDev.Value = 0.3M;
			else
				// 0.5px tolerance for tast motion 
				nudMaxStdDev.Value = 0.5M;
		}

		private void cbxInstDelayUnit_SelectedIndexChanged(object sender, EventArgs e)
		{
			//nudInstrDelay.DecimalPlaces = 2 - cbxInstDelayUnit.SelectedIndex;
		}

		private int activePage = 0;

		private void DisplayActivePage()
		{
			if (activePage < 0) activePage = 0;
			if (activePage > 3) activePage = 3;

			if (activePage == 0)
			{
				pnlTime.Visible = true;
				pnlAstrometry.Visible = false;
				pnlPhotometry.Visible = false;
                pnlAddins.Visible = false;
				gbConfig.Text = "Object and Time";
			}
			else if (activePage == 1)
			{
				pnlTime.Visible = false;
				pnlAstrometry.Visible = true;
				pnlPhotometry.Visible = false;
                pnlAddins.Visible = false;
				gbConfig.Text = "Astrometry";
			}
			else if (activePage == 2)
			{
				pnlTime.Visible = false;
				pnlAstrometry.Visible = false;
				pnlPhotometry.Visible = true;
                pnlAddins.Visible = false;
				gbConfig.Text = "Photometry";
				PlotAperturePreview();
			}
            else if (activePage == 3 && m_AstrometryAddinActions.Count != 0)
            {
                pnlTime.Visible = false;
                pnlAstrometry.Visible = false;
                pnlPhotometry.Visible = false;
                pnlAddins.Visible = true;
                gbConfig.Text = "Add-ins";
            }

            if (activePage > 0)
                btnPrevious.Text = "< Previous";
            else
                btnPrevious.Text = "Cancel";

            if (activePage < 3 || (activePage == 2 && m_AstrometryAddinActions.Count == 0))
                btnNext.Text = "Next >";
            else
                btnNext.Text = "Start";
		}

		private void btnDetectIntegration_Click(object sender, EventArgs e)
		{
			frmIntegrationDetection frm = new frmIntegrationDetection(m_VideoController, m_VideoController.CurrentFrameIndex);
			frm.StartPosition = FormStartPosition.CenterParent;
			DialogResult res = frm.ShowDialog(this);

			if (res == DialogResult.OK)
			{
				if (frm.IntegratedFrames != null)
				{
					m_VideoAstrometry.MovingToFirstIntegratedFrame();
					cbxInstDelayMode.SelectedIndex = 0; // Set delay type to automatic
					nudIntegratedFrames.SetNUDValue(frm.IntegratedFrames.Interval);
					m_VideoController.MoveToFrame(frm.IntegratedFrames.StartingAtFrame);

				    SyncTimeStampControlWithExpectedFrameTime();

					btnDetectIntegration.Enabled = false;
				}
			}
			else if (res == DialogResult.Cancel)
			{
				if ((MovementExpectation)cbxExpectedMotion.SelectedIndex != MovementExpectation.Slow)
				{
					MessageBox.Show(
						this,
						"When measuring fast moving objects it is absolutely essential to move the video to the first frame of an integration interval " +
					    "before starting the measurements. Press 'Cancel' now and ensure that you have manually positioned to the first frame of the next " +
					    "integration interval before you continue.",
						"Warning",
						MessageBoxButtons.OK,
						MessageBoxIcon.Warning);
				}
			}
		}

		private void linkExamples_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			ShellHelper.OpenUrl("http://www.hristopavlov.net/Tangra/AstrometryExamples");
		}

		private void nudIntegratedFrames_ValueChanged(object sender, EventArgs e)
		{
			SetInstrumentalDelay();
		}

		private void cbxInstDelayMode_SelectedIndexChanged(object sender, EventArgs e)
		{
			SetInstDelayControlsState();
			SetInstrumentalDelay();
		}

		private void SetInstDelayControlsState()
		{
			if (cbxInstDelayMode.SelectedIndex == 0)
			{
				cbxInstDelayCamera.Enabled = false;
				cbxInstDelayUnit.Enabled = false;
				nudInstrDelay.Enabled = false;
			}
			else if (cbxInstDelayMode.SelectedIndex == 1)
			{
				if (cbxInstDelayMode.SelectedIndex == 0)
				{
					cbxInstDelayCamera.Enabled = false;
					cbxInstDelayUnit.Enabled = true;
					nudInstrDelay.Enabled = true;
				}
				else if (cbxInstDelayMode.SelectedIndex == 1)
				{
					cbxInstDelayCamera.Enabled = true;
					cbxInstDelayUnit.Enabled = cbxInstDelayCamera.SelectedIndex == 0;
					nudInstrDelay.Enabled = cbxInstDelayCamera.SelectedIndex == 0;
				}
			}
		}

		private void cbxInstDelayCamera_SelectedIndexChanged(object sender, EventArgs e)
		{
			SetKnownManualInstrumentalDelay();
		}

		private void SetInstrumentalDelay()
		{
			if (cbxFrameTimeType.SelectedIndex == 1)
				SetKnownAutomaticInstrumentalDelay();
			else if (cbxFrameTimeType.SelectedIndex == 0)
				SetKnownManualInstrumentalDelay();
		}

		private void SetKnownAutomaticInstrumentalDelay()
		{
			int integration = (int)nudIntegratedFrames.Value;

			string cameraModel = m_VideoController.AstroVideoCameraModel ?? AstrometryContext.Current.VideoCamera.Model;
            InstrumentalDelayConfiguration corrections = InstrumentalDelayConfigManager.GetConfigurationForCameraForAstrometry(cameraModel);

		    float? corr = corrections.CalculateDelay(integration, new DelayRequest());
			if (corr.HasValue)
			{
				cbxInstDelayCamera.SelectedIndex = cbxInstDelayCamera.Items.IndexOf(cameraModel);
				SetKnownManualInstrumentalDelay();
			}
		}

		private void SetKnownManualInstrumentalDelay()
		{
			if (cbxInstDelayCamera.SelectedIndex > -1)
			{
				if (cbxInstDelayCamera.Text == string.Empty)
				{
					nudInstrDelay.Enabled = true;
					cbxInstDelayUnit.Enabled = true;
				}
				else
				{
                    InstrumentalDelayConfiguration delaysConfig = InstrumentalDelayConfigManager.GetConfigurationForCameraForAstrometry(cbxInstDelayCamera.Text);
                    float? corr = delaysConfig.CalculateDelay((int)nudIntegratedFrames.Value, new DelayRequest());
				    if (corr.HasValue)
				    {
                        nudInstrDelay.Value = -1 * (decimal)corr.Value;
                        cbxInstDelayUnit.SelectedIndex = 1;
				    }
					nudInstrDelay.Enabled = false;
					cbxInstDelayUnit.Enabled = false;
				}
			}
		}

		private void PlotAperturePreview()
		{
			if (m_MeasurementContext.ObjectToMeasure != null)
			{
				float x0 = m_MeasurementContext.ObjectToMeasure.X0;
				float y0 = m_MeasurementContext.ObjectToMeasure.Y0;

				// x0 and y0 were measured on the first frame but we may have moved to a different frame due to positioning to the first frame of integration period
				// so we determine the position of the object on the current frame in order to draw the aperture nicely centered
				var fit = new PSFFit((int)x0, (int)y0);
				fit.Fit(m_VideoController.GetCurrentAstroImage(false).GetMeasurableAreaPixels((int)x0, (int)y0));

				if (fit.IsSolved)
				{
					x0 = (float)fit.XCenter;
					y0 = (float)fit.YCenter;
				}

				byte[,] bmpPixels = m_VideoController.GetCurrentAstroImage(false).GetMeasurableAreaDisplayBitmapPixels((int) x0, (int) y0, 85);

				Bitmap bmp = Pixelmap.ConstructBitmapFromBitmapPixels(bmpPixels, 85, 85);
				using (Graphics g = Graphics.FromImage(pboxAperturePreview.Image))
				{
					g.DrawImage(bmp, 0, 0);
					float xCenter = (x0 - (int)x0) + 42;
					float yCenter = (y0 - (int)y0) + 42;

					float radius = (float) nudAperture.Value;
					g.DrawEllipse(Pens.YellowGreen, xCenter - radius, yCenter - radius, 2 * radius, 2 * radius);

					radius = (float)(nudAperture.Value + nudGap.Value);
					g.DrawEllipse(Pens.YellowGreen, xCenter - radius, yCenter - radius, 2 * radius, 2 * radius);

					radius = (float)(nudAperture.Value + nudGap.Value + nudAnnulus.Value);
					g.DrawEllipse(Pens.YellowGreen, xCenter - radius, yCenter - radius, 2 * radius, 2 * radius);

					g.Save();
				}

				pboxAperturePreview.Invalidate();
			}
		}

		private void nudAperture_ValueChanged(object sender, EventArgs e)
		{
			float annulusInnerRadius = TangraConfig.Settings.Photometry.AnnulusInnerRadius;
			float annulusMinPixels = TangraConfig.Settings.Photometry.AnnulusMinPixels;
			if (m_Initialised)
			{
				// If we have already set the initial values, we want to keep the new inner radius/min pixel values 
				annulusInnerRadius = ((float)nudGap.Value + (float)nudAperture.Value) / (float)nudAperture.Value;
				annulusMinPixels = (int)(Math.PI * (Math.Pow((float)nudAnnulus.Value + (float)nudGap.Value + (float)nudAperture.Value, 2) - Math.Pow((float)nudGap.Value + (float)nudAperture.Value, 2)));
			}

			float innerRadius = annulusInnerRadius * (float)nudAperture.Value;
			float outernRadius = (float)Math.Sqrt(annulusMinPixels / Math.PI + innerRadius * innerRadius);
			decimal gap = (decimal)innerRadius - nudAperture.Value;
			decimal annulus = (decimal)outernRadius - nudGap.Value - nudAperture.Value;

			nudGap.SetNUDValue(gap);
			nudAnnulus.SetNUDValue(annulus);

			PlotAperturePreview();
		}

		private void AnnulusSizeChanged(object sender, EventArgs e)
		{
			PlotAperturePreview();
		}

        private void btnResetApertures_Click(object sender, EventArgs e)
        {
            ResetAperture();
        }

	}
}
