/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Config;
using Tangra.Controller;
using Tangra.Helpers;
using Tangra.Model.Config;
using Tangra.Model.Context;
using Tangra.SDK;
using Tangra.StarCatalogues;
using Tangra.Video;
using Tangra.VideoOperations.Astrometry.Engine;

namespace Tangra.VideoOperations.Astrometry
{
	internal partial class frmRunMultiFrameMeasurements : Form
	{
		private VideoAstrometryOperation m_VideoAstrometry;
		private MeasurementContext m_MeasurementContext;
	    private FieldSolveContext m_FieldSolveContext;
		private VideoController m_VideoController;
		private AddinsController m_AddinsController;

        private List<ITangraAddinAction> m_AstrometryAddinActions = new List<ITangraAddinAction>();
        private List<ITangraAddin> m_AstrometryAddins = new List<ITangraAddin>();

        internal frmRunMultiFrameMeasurements(
			VideoController videoController, AddinsController addinsController, VideoAstrometryOperation astrometry, MeasurementContext measurementContext, FieldSolveContext fieldSolveContext,
            out List<ITangraAddinAction> astrometryAddinActions, out List<ITangraAddin> astrometryAddins)
		{
			InitializeComponent();

			m_VideoController = videoController;
	        m_AddinsController = addinsController;

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

			cbxOutputMagBand.SelectedIndex = (int)TangraConfig.Settings.Astrometry.DefaultMagOutputBand;
            
            lblCatBand.Text = string.Format("Magnitude Band for Photometry (from {0})", m_FieldSolveContext.StarCatalogueFacade.CatalogNETCode);
            cbxCatalogPhotometryBand.Items.Clear();
            cbxCatalogPhotometryBand.Items.AddRange(m_FieldSolveContext.StarCatalogueFacade.CatalogMagnitudeBands);
            CatalogMagnitudeBand defaultBand = cbxCatalogPhotometryBand.Items.Cast<CatalogMagnitudeBand>().FirstOrDefault(e => e.Id == TangraConfig.Settings.StarCatalogue.CatalogMagnitudeBandId);
            if (defaultBand != null)
                cbxCatalogPhotometryBand.SelectedItem = defaultBand;
            else
                cbxCatalogPhotometryBand.SelectedIndex = 0;

        	cbxExpectedMotion.SelectedIndex = 0;
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
			double milisecondsDiff =
				(m_VideoController.CurrentFrameIndex - AstrometryContext.Current.FieldSolveContext.FrameNoOfUtcTime) * 1000.0 / m_VideoController.VideoFrameRate;

			ucUtcTimePicker.DateTimeUtc = AstrometryContext.Current.FieldSolveContext.UtcTime.AddMilliseconds(milisecondsDiff);            
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

            m_MeasurementContext.MaxStdDev = (double)nudMaxStdDev.Value;
			m_MeasurementContext.FrameInterval = ucFrameInterval.Value;
			m_MeasurementContext.FirstFrameUtcTime = ucUtcTimePicker.DateTimeUtc;
			m_MeasurementContext.PerformPhotometricFit = cbxFitMagnitudes.Checked;
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
			m_MeasurementContext.IntegratedFramesCount = (int)nudIntegratedFrames.Value;
			m_MeasurementContext.MaxMeasurements = (int)nudNumberMeasurements.Value;

			// TODO: Detect the integration automatically and position to the first frame of the next interval
			TangraConfig.Settings.LastUsed.AstrometryPhotometryReductionMethod = ComboboxIndexToPhotometryReductionMethod();
			TangraConfig.Settings.LastUsed.AstrometryPhotometryBackgroundMethod = ComboboxIndexToBackgroundMethod();
			TangraConfig.Settings.LastUsed.AstrometryFitMagnitudes = cbxFitMagnitudes.Checked;
			TangraConfig.Settings.LastUsed.LastAstrometryUTCDate = ucUtcTimePicker.DateTimeUtc;
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
				decimal oldValue = nudNumberMeasurements.Value;
				nudNumberMeasurements.Value = nudNumberMeasurements.Minimum;
				nudNumberMeasurements.Maximum = m_MeasurementContext.MaxMeasurements;
				nudNumberMeasurements.Value = Math.Min(nudNumberMeasurements.Maximum, oldValue);
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
			pnlIntegration.Visible = cbxFrameTimeType.SelectedIndex != 0;

            nudInstrDelay.Enabled = true;
            cbxInstDelayUnit.Enabled = true;
            nudIntegratedFrames.Enabled = true;
            btnDetectIntegration.Enabled = true;		  
		}

		private void cbxExpectedMotion_SelectedIndexChanged(object sender, EventArgs e)
		{
			cbxSignalType.Visible = cbxExpectedMotion.SelectedIndex == 2;

			if (cbxExpectedMotion.SelectedIndex == 0)
				nudMaxStdDev.Value = 0.5M;
			else
				// 1" tolerance for tast motion 
				nudMaxStdDev.Value = 1.0M;
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
			int integration = (int)nudIntegratedFrames.Value;

			Dictionary<int, float> corrections = InstrumentalDelayConfigManager.GetConfigurationForCamera(m_VideoController.AstroVideoCameraModel ?? AstrometryContext.Current.VideoCamera.Model);

			float corr;
			if (corrections.TryGetValue(integration, out corr))
			{
				if (!float.IsNaN(corr))
				{
					nudInstrDelay.SetNUDValue(corr);
					cbxInstDelayUnit.SelectedIndex = 0;
				}
			}
		}
	}
}
