using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Tangra.Controller;
using Tangra.Helpers;
using Tangra.Model.Config;
using Tangra.Model.Context;
using Tangra.Model.Image;
using Tangra.Model.Video;
using Tangra.Model.VideoOperations;
using Tangra.Video;
using Tangra.VideoOperations.LightCurves;
using Tangra.Config;
using Tangra.VideoOperations.LightCurves.Tracking;
using Tangra.ImageTools;

namespace Tangra.VideoOperations.LightCurves
{
    public partial class ucLightCurves : UserControl
    {
        private LCStateMachine m_StateMachine;
        protected VideoController m_VideoController;
		private TangraConfig.LightCurvesDisplaySettings m_DisplaySettings;

		public ucLightCurves()
        {
            InitializeComponent();

            #region Positioning the separate sheets for display
            pnlUserAction.Top = 3;
            pnlUserAction.Left = 3;
            pnlViewLightCurve.Top = 3;
            pnlViewLightCurve.Left = 3;
            #endregion

            ucUtcTime.OnDateTimeInputComplete += new EventHandler(OnUTCDateTimeInputComplete);

			m_DisplaySettings = new TangraConfig.LightCurvesDisplaySettings();
			m_DisplaySettings.Load();
			m_DisplaySettings.Initialize();
        }

        internal void BeginConfiguration(LCStateMachine stateMachine, VideoController videoController)
        {
            m_StateMachine = stateMachine;
            m_VideoController = videoController;

            UpdateState();
        }

        private string GetReductionMethodDisplayName(TangraConfig.PhotometryReductionMethod method)
        {
            switch(method)
            {
                case TangraConfig.PhotometryReductionMethod.AperturePhotometry:
                    return "Aperture Photometry";
                case TangraConfig.PhotometryReductionMethod.OptimalExtraction:
                    return "Optimal Extraction";
                case TangraConfig.PhotometryReductionMethod.PsfPhotometryAnalytical:
                    return "Analytical Psf Photometry";
                case TangraConfig.PhotometryReductionMethod.PsfPhotometryNumerical:
                    return "Numerical Psf Photometry";
            }

            return "N/A";
        }

        private string GetNoiseMethodDisplayName(TangraConfig.BackgroundMethod method)
        {
            switch (method)
            {
                case TangraConfig.BackgroundMethod.AverageBackground:
                    return "Average Background";
                case TangraConfig.BackgroundMethod.BackgroundMode:
                    return "Background Mode";
                case TangraConfig.BackgroundMethod.PSFBackground:
                    return "PSF Background";
                case TangraConfig.BackgroundMethod.BackgroundGradientFit:
                    return "Background Gradient Fit";
            }

            return "N/A";
        }

        internal void UpdateState()
        {
            this.BackColor = SystemColors.Control;

            if (m_StateMachine.CurrentState == LightCurvesState.SelectTrackingStars ||
                m_StateMachine.CurrentState == LightCurvesState.SelectMeasuringStars)
            {
                pnlUserAction.Visible = true;
                pnlUserAction.BringToFront();
                pnlProcessing.Visible = false;
                pnlViewLightCurve.Visible = false;
                pnlEnterTimes.Visible = false;
                m_StoppedAtFrameNo = -1;
            }
            else if (
                m_StateMachine.CurrentState == LightCurvesState.ReadyToRun ||
                m_StateMachine.CurrentState == LightCurvesState.Running)
            {
                pnlEnterTimes.Visible = false;
                pnlUserAction.Visible = false;
                pnlProcessing.Visible = true;
                pnlProcessing.BringToFront();
                pnlViewLightCurve.Visible = false;
            }
            else if (m_StateMachine.CurrentState == LightCurvesState.SelectingFrameTimes)
            {
                btnStop.Visible = false;
                btnLightCurve.Visible = false;

                pnlProcessing.Visible = true;
                pnlProcessing.BringToFront();
                pnlEnterTimes.Visible = true;
                pnlUserAction.Visible = false;
                pnlViewLightCurve.Visible = false;
            }
            else if (m_StateMachine.CurrentState == LightCurvesState.Viewing)
            {
                pnlEnterTimes.Visible = false;
                pnlUserAction.Visible = false;
                pnlProcessing.Visible = false;
                pnlViewLightCurve.Visible = true;
                pnlViewLightCurve.BringToFront();
            }


            if (m_StateMachine.CurrentState == LightCurvesState.SelectMeasuringStars)
            {
                pnlTrackingSelection.BackColor = SystemColors.ControlLightLight;
                pnlTrackingSelection.BorderStyle = BorderStyle.FixedSingle;
                pnlMeasuringSelection.BackColor = SystemColors.ControlLightLight;
                pnlMeasuringSelection.BorderStyle = BorderStyle.FixedSingle;

                lblInfo.ForeColor = SystemColors.ControlText;
                if (m_StateMachine.MeasuringStars.Count == 0)
                {
                    btnStart.Enabled = false;
                    lblInfo.Text =
                        "- Move to the first frame you want to measure\r\n" +
                        "- Select your first target\r\n" +
                        "- Press the 'Add Object' button";

                    lblSignalMethod.Text = GetReductionMethodDisplayName(LightCurveReductionContext.Instance.ReductionMethod);
                    lblBackgroundMethod.Text = GetNoiseMethodDisplayName(LightCurveReductionContext.Instance.NoiseMethod);

                    TangraContext.Current.CanScrollFrames = true;
                    m_VideoController.UpdateViews();
                }
                else
                {
                    lblInfo.Text = string.Empty;
                    // We need an occulted star before we can start
                    btnStart.Enabled = m_StateMachine.MeasuringStars.Find(m => m.TrackingType == TrackingType.OccultedStar) != null;
                    TangraContext.Current.CanScrollFrames = false;
                }

                btnAddObject.Visible =
                    // If there is a selected object and this is not an added star
                    m_StateMachine.SelectedObject != null && m_StateMachine.SelectedMeasuringStar == -1;

                btnEditObject.Visible =
                    // If there is a selected object and it is an existing star
                    m_StateMachine.SelectedObject != null && m_StateMachine.SelectedMeasuringStar != -1;

                rbDisplayBrightness.Checked = TangraConfig.Settings.LastUsed.MeasuringZoomImageMode != 1;
                rbDisplayPixels.Checked = TangraConfig.Settings.LastUsed.MeasuringZoomImageMode == 1;
            }
            else
            {
                pnlTrackingSelection.BackColor = SystemColors.Control;
                pnlMeasuringSelection.BackColor = SystemColors.Control;
                pnlTrackingSelection.BorderStyle = BorderStyle.FixedSingle;
                pnlMeasuringSelection.BorderStyle = BorderStyle.FixedSingle;
            }

            if (m_StateMachine.CurrentState == LightCurvesState.Running)
            {
                if (m_StateMachine.VideoOperation.IsRefining)
                    btnStop.Text = "Cancel Measurements";
                else if (m_StateMachine.VideoOperation.IsMeasuring)
                    btnStop.Text = "Stop Measurements";

                btnLightCurve.Visible = false;

                pnlMeasureZoomOptions.Visible = m_StateMachine.VideoOperation.IsMeasuring;
            }

            UpdateLCData();            
        }

		internal void SetupLCFileInfo(LCFile lcFile)
		{
			pnlViewLightCurve.Controls.Clear();
			var info = new ucLCFileInfo(lcFile, m_VideoController);
			pnlViewLightCurve.Controls.Add(info);
			info.Dock = DockStyle.Fill;
		}

        public void UpdateProcessedFrames(int processedFramed, int unsuccessfulFrames, int elapsedTime)
        {
            lblProcessedFrames.Text = processedFramed.ToString();
            lblSkippedFrames.Text = unsuccessfulFrames.ToString();
            lblElapsedTime.Text = string.Format("{0}:{1}", elapsedTime / 60, (elapsedTime % 60).ToString("00"));
        }

        public void StopMeasurements()
        {
            m_StateMachine.VideoOperation.StopMeasurements();
            m_StoppedAtFrameNo = m_StateMachine.VideoOperation.m_CurrFrameNo;

            btnStop.Text = "Continue Measurements";

            btnLightCurve.Visible = true;
        }

        public void StoppedAtLastFrame()
        {
			Invoke(new Action(() => timerMoveToFirstFrame.Enabled = true ));
        }

        public void StopRefining()
        {
            // Return back to configuring 
            m_StateMachine.VideoOperation.StopRefining();
            m_StoppedAtFrameNo = -1;
            btnStop.Text = "Start";
        }

        public void FinishWithMeasurements()
        {
            // Once we enter time entering mode we clear any integration
            m_VideoController.SetupFrameIntegration(1, FrameIntegratingMode.NoIntegration, PixelIntegrationType.Mean);
            m_VideoController.UpdateViews();

            m_StateMachine.VideoOperation.FinishedWithMeasurements();

            btnStop.Visible = false;
            btnLightCurve.Visible = false;

            if (!LightCurveReductionContext.Instance.HasEmbeddedTimeStamps)
                PrepareToEnterStarTime();
            else
                m_StateMachine.VideoOperation.ShowLightCurve();
        }

	    private delegate void PrepareToEnterStarTimeCallback();

		private void PrepareToEnterStarTime()
        {
            ucUtcTime.DateTimeUtc = DateTime.Now.ToUniversalTime();
            pnlEnterTimes.Visible = true;
            lblTimesHeader.Text = "Enter the UTC time of the first measured frame:";
            btnNextTime.Text = "Next >>";
            m_FirstTimeSet = false;
            m_StateMachine.VideoOperation.InitGetStartTime();

            ucUtcTime.FocusHourControl();

            m_ShowingFields = false;
            UpdateShowingFieldControls();
            m_FirstTimeFrame = -1;
            m_LastTimeFrame = -1;
        }

        private int GetSelectedTargetIndex()
        {
            if (rbM1.Checked) return 0;
            else if (rbM2.Checked) return 1;
            else if (rbM3.Checked) return 2;
            else if (rbM4.Checked) return 3;

            return -1;
        }

        private void SelectedTargetChanged(object sender, EventArgs e)
        {
            if (m_UpdatingCheckedState)
                return;

            int idx = GetSelectedTargetIndex();

            m_StateMachine.SelectedMeasuringStar = idx;

            UpdateLCData();

            m_VideoController.RefreshCurrentFrame();
        }

        private void UpdateLCData()
        {
            int measuringStars = m_StateMachine.MeasuringStars.FindAll(obj => obj.MeasureThisObject).Count;
            if (measuringStars == 1)
                lblMeasuringStars.Text = "1 target";
            else
                lblMeasuringStars.Text = string.Format("{0} targets", measuringStars);

            int guidingStars = m_StateMachine.MeasuringStars.FindAll(obj => obj.TrackingType == TrackingType.GuidingStar).Count;
            pnlTrackingSelection.Visible = guidingStars > 0;

            if (guidingStars == 1)
                lblTrackingStars.Text = "1 target";
            else
                lblTrackingStars.Text = string.Format("{0} targets", guidingStars);

            UpdateRadioButtonStars();
        }

        private bool m_UpdatingCheckedState = false;

        private void UpdateRadioButtonStars()
        {
            RadioButton[] MRBs = new RadioButton[] { rbM1, rbM2, rbM3, rbM4 };
            Label[] MLBLs = new Label[] { lblM1, lblM2, lblM3, lblM4 };

            if (m_StateMachine == null)
            {
                foreach (RadioButton rb in MRBs) rb.Visible = false;
                foreach (Label lbl in MLBLs) lbl.Visible = false;
                lblSel.Visible = false;
                lblObj.Visible = false;
            }
            else
            {
                m_UpdatingCheckedState = true;
                try
                {
                    for (int i = 0; i < Math.Min(m_StateMachine.MeasuringStars.Count, MRBs.Length); i++)
                    {
                        MRBs[i].Visible = true;
                        MRBs[i].Checked = false;
                        if (i == m_StateMachine.SelectedMeasuringStar) MRBs[i].Checked = true;
                        MLBLs[i].Visible = true;
                    }
                }
                finally
                {
                    m_UpdatingCheckedState = false;
                }

                for (int i = m_StateMachine.MeasuringStars.Count; i < MRBs.Length; i++)
                {
                    MRBs[i].Visible = false;
                    MLBLs[i].Visible = false;
                }

                lblSel.Visible = m_StateMachine.MeasuringStars.Count > 0;
                lblObj.Visible = m_StateMachine.MeasuringStars.Count > 0;
            }
        }

        private void btnAddObject_Click(object sender, EventArgs e)
        {
            if (m_StateMachine.SelectedObject != null)
            {
                int newId = -1;
                bool isNew = m_StateMachine.IsNewObject(m_StateMachine.SelectedObject, false, false, ref newId);
                if (isNew && newId != -1)
                {
                    m_StateMachine.VideoOperation.EnsureStackedAstroImage();

                    frmAddOrEditSingleTarget frmAddSingleTarget = new frmAddOrEditSingleTarget(newId, m_StateMachine.SelectedObject, m_StateMachine.SelectedObjectGaussian, m_StateMachine, m_VideoController);
                    if (frmAddSingleTarget.ShowDialog() == DialogResult.Cancel)
                        return;

                    m_StateMachine.ObjectSelected(frmAddSingleTarget.ObjectToAdd, false, false);
                }

                if (m_StateMachine.MeasuringStars.Count > 0 &&
                    TangraContext.Current.HasVideoLoaded)
                {
                    // Disable playing/moving video after there is at least one object selected
                    TangraContext.Current.CanChangeTool = false;
                    TangraContext.Current.CanScrollFrames = false;
                    m_VideoController.UpdateViews();

                    m_StateMachine.m_ConfiguringFrame = m_StateMachine.VideoOperation.m_CurrFrameNo;
                }

                m_VideoController.RefreshCurrentFrame();
            }
        }

        private void btnEditObject_Click(object sender, EventArgs e)
        {
            if (m_StateMachine.SelectedMeasuringStar != -1)
            {
                m_StateMachine.VideoOperation.EnsureStackedAstroImage();

                TrackedObjectConfig selectedObject = m_StateMachine.MeasuringStars[m_StateMachine.SelectedMeasuringStar];
                frmAddOrEditSingleTarget frmAddSingleTarget =
                    new frmAddOrEditSingleTarget(m_StateMachine.SelectedMeasuringStar, selectedObject, m_StateMachine, m_VideoController);

                DialogResult result = frmAddSingleTarget.ShowDialog();
                if (result == DialogResult.Cancel)
                    return;

                if (result == DialogResult.Abort)
                {
                    m_StateMachine.ObjectSelected(frmAddSingleTarget.ObjectToAdd, false, true /* Ctrl = 'DELETE' */);
                    m_StateMachine.SelectedMeasuringStar = -1;
                    m_VideoController.RefreshCurrentFrame();

                    return;
                }

                m_StateMachine.ObjectEdited(frmAddSingleTarget.ObjectToAdd);

				m_VideoController.RefreshCurrentFrame();
            }
        }

        private void btnStartOver_Click(object sender, EventArgs e)
        {
            if (m_VideoController.ShowMessageBox(
                "All selected objects will be cleared. Do you wish to Continue?",
                "Confirmation",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                m_StateMachine.VideoOperation.StartOver();
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
			bool ctrlHeld = 
				Control.ModifierKeys == Keys.Control || 
				Control.ModifierKeys == Keys.ControlKey;

			LightCurveReductionContext.Instance.DebugTracking = ctrlHeld;

			TrackedObjectConfig occultedStar = m_StateMachine.MeasuringStars.Find(t => t.TrackingType == TrackingType.OccultedStar);
			if (occultedStar == null)
			{
				m_VideoController.ShowMessageBox(
					"No 'Occulted Star' is selected", "Error", 
					MessageBoxButtons.OK, 
					MessageBoxIcon.Error);
				return;
			}

			if (HasObjectsWithAutoAperutreCloserThan8PixelsApart())
			{
				m_VideoController.ShowMessageBox(
					"Tangra cannot complete this measurement because two of the objects are too close.\r\n\r\n " +
					"To continue you need to:\r\n\r\n- Use Aperture Photometry \r\n- Use Manually Positioned aperture for one of the two close objects",
					"Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);

				return;
			}

			int numberGuidingStars = m_StateMachine.MeasuringStars.Count(t => t.TrackingType == TrackingType.GuidingStar);
			int numberComparisonStars = m_StateMachine.MeasuringStars.Count(t => t.TrackingType == TrackingType.ComparisonStar);
			bool warnedAboutGuidingStars = false;

			if (occultedStar.AutoStarsInArea.Count != 1 ||
				occultedStar.IsWeakSignalObject)
			{
				// If this is a user defined position or not the only bright star in the region, we 
				// require at least one guiding star before we can start
				if (numberGuidingStars == 0)
				{
					warnedAboutGuidingStars = true;
					if (m_VideoController.ShowMessageBox(
						"It seems that the occulted star is not very bright. For best tracking results it is recommended to select as many 'Guiding Stars' as possible.\r\n\r\nYou may also consider using software integration if there are no bright stars in the video." +
						"\r\n\r\nWould you like to continue anyway without a 'Guiding Star'?",
						"Warning",
						MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
					{
						return;
					}
				}
			}

            if (!warnedAboutGuidingStars &&
                numberGuidingStars < numberComparisonStars)
            {
                warnedAboutGuidingStars = true;
                if (m_VideoController.ShowMessageBox(
                        "It seems that you may have too few 'Guiding Stars'. For best results it is recommended to use as many 'Guiding Stars' as possible.\r\n\r\nYou should also consider using software integration if there are no bright stars in the video." +
                        "\r\n\r\nWould you like to continue anyway?",
                        "Warning",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
                {
                    return;
                }
            }

			m_StoppedAtFrameNo = -1;
			m_StateMachine.VideoOperation.BeginMeasurements();
        }

        private bool HasObjectsWithAutoAperutreCloserThan8PixelsApart()
        {
            for (int i = 0; i < m_StateMachine.MeasuringStars.Count; i++)
            {
                for (int j = i + 1; j < m_StateMachine.MeasuringStars.Count; j++)
                {
                    TrackedObjectConfig obj1 = m_StateMachine.MeasuringStars[i];
                    TrackedObjectConfig obj2 = m_StateMachine.MeasuringStars[j];

                    if (obj1.IsFixedAperture || obj2.IsFixedAperture) continue;

                    double dist = ImagePixel.ComputeDistance(
                        obj1.ApertureStartingX, obj2.ApertureStartingX,
                        obj1.ApertureStartingY, obj2.ApertureStartingY);

                    double minDist = 8 + (obj1.ApertureInPixels + obj2.ApertureInPixels) / 2;

                    if (dist <= minDist) return true;
                }
            }

            return false;
        }

        private int m_StoppedAtFrameNo = -1;
        private void btnStop_Click(object sender, EventArgs e)
        {
			if (m_StoppedAtFrameNo != -1)
			{
				m_StateMachine.VideoOperation.ContinueMeasurements(m_StoppedAtFrameNo);
				m_StoppedAtFrameNo = -1;
				btnStop.Text = "Stop";

				btnLightCurve.Visible = false;
			}
			else
			{
				if (m_StateMachine.VideoOperation.IsRefining)
				{
					StopRefining();
				}
				else
				{
					StopMeasurements();
				}
			}
        }

        private void btnLightCurve_Click(object sender, EventArgs e)
        {
	        m_VideoController.SelectImageTool<ArrowTool>();

            FinishWithMeasurements();
        }

        private bool m_FirstTimeSet = false;
        private int m_FirstTimeFrame = -1;
        private int m_LastTimeFrame = -1;

		private bool IsDuplicatedFrame(int frameId)
		{
			var avoider = new DuplicateFrameAvoider((VideoController)m_VideoController, frameId);
			return avoider.IsDuplicatedFrame();			
		}

		private void ShowDuplicatedFrameMessage()
		{
			MessageBox.Show(
				"The current frame appears to be duplicate of the previous or the next frame. Because of this the timestamp on this frame " +
				"may not correspond to the actual time for this frame number. Please move to and enter the timestamp of a different video frame.",
				"Problem found", MessageBoxButtons.OK, MessageBoxIcon.Stop);
		}

        private void btnNextTime_Click(object sender, EventArgs e)
        {
			btnContinueWithNoTimes.Visible = false;

			if (!m_FirstTimeSet)
			{
				if (IsDuplicatedFrame(m_StateMachine.VideoOperation.m_CurrFrameNo))
				{
					ShowDuplicatedFrameMessage();
					return;
				}

				m_FirstTimeFrame = m_StateMachine.VideoOperation.m_CurrFrameNo;
				m_FirstTimeSet = true;

				lblTimesHeader.Text = "Enter the UTC time of the last measured frame:";
				btnNextTime.Text = "Finish";
				ucUtcTime.EnterTimeAtTheSameDate();
				m_StateMachine.VideoOperation.SetStartTime(ucUtcTime.DateTimeUtc);
				m_StateMachine.VideoOperation.InitGetEndTime();

				if (m_ShowingFields) m_StateMachine.VideoOperation.ToggleShowFields(true);
			}
			else
			{
				if (IsDuplicatedFrame(m_StateMachine.VideoOperation.m_CurrFrameNo))
				{
					ShowDuplicatedFrameMessage();
					return;
				}

				m_StateMachine.VideoOperation.SetEndTime(ucUtcTime.DateTimeUtc);
				m_LastTimeFrame = m_StateMachine.VideoOperation.m_CurrFrameNo;

                DialogResult checkResult = m_StateMachine.VideoOperation.EnteredTimeIntervalLooksOkay();

                switch (checkResult)
                {
                    case DialogResult.OK:
                    case DialogResult.Ignore:
                        m_StateMachine.VideoOperation.ShowLightCurve();
                        break;

                    case DialogResult.Retry:
                        PrepareToEnterStarTime();
                        return;
                }
			}

			UpdateShowingFieldControls();
        }

        private void OnUTCDateTimeInputComplete(object sender, EventArgs e)
        {
            btnNextTime.Focus();
        }

        private void btnContinueWithNoTimes_Click(object sender, EventArgs e)
        {
            m_StateMachine.VideoOperation.ShowLightCurve();
        }

        private bool m_ShowingFields = false;

        private void btnShowFields_Click(object sender, EventArgs e)
        {
            m_StateMachine.VideoOperation.ToggleShowFields(!m_ShowingFields);
            m_ShowingFields = !m_ShowingFields;

            UpdateShowingFieldControls();
        }

        private void UpdateShowingFieldControls()
        {
            if (m_ShowingFields)
                btnShowFields.Text = "Show Frames";
            else
                btnShowFields.Text = "Show Fields";

            btn1FrMinus.Enabled = m_FirstTimeFrame == -1
                                      ? m_StateMachine.VideoOperation.m_MinFrame < m_StateMachine.VideoOperation.m_CurrFrameNo
                                      : m_FirstTimeFrame < m_StateMachine.VideoOperation.m_CurrFrameNo;

            btn1FrPlus.Enabled = m_LastTimeFrame == -1
                                     ? m_StateMachine.VideoOperation.m_MaxFrame > m_StateMachine.VideoOperation.m_CurrFrameNo
                                     : m_LastTimeFrame > m_StateMachine.VideoOperation.m_CurrFrameNo;
        }

        private void btn1FrPlus_Click(object sender, EventArgs e)
        {
			m_VideoController.StepForward();
			UpdateShowingFieldControls();

			if (m_ShowingFields) m_StateMachine.VideoOperation.ToggleShowFields(true);
        }

        private void btn1FrMinus_Click(object sender, EventArgs e)
        {
            m_VideoController.StepBackward();
			UpdateShowingFieldControls();

			if (m_ShowingFields) m_StateMachine.VideoOperation.ToggleShowFields(true);
        }

        private void timerMoveToFirstFrame_Tick(object sender, EventArgs e)
        {
			timerMoveToFirstFrame.Enabled = false;
			FinishWithMeasurements();
        }

        private void cbxDisplayBrightness_CheckedChanged(object sender, EventArgs e)
        {
			if (rbDisplayBrightness.Checked)
				m_StateMachine.VideoOperation.SetMeasuringZoomImageType(MeasuringZoomImageType.Stripe);
			else
				m_StateMachine.VideoOperation.SetMeasuringZoomImageType(MeasuringZoomImageType.Pixel);
        }
    }
}
