using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Tangra;
using Tangra.Config;
using Tangra.Controller;
using Tangra.Model.Config;
using Tangra.Model.Context;
using Tangra.Model.Video;
using Tangra.Model.VideoOperations;
using Tangra.Video;
using Tangra.VideoOperations.LightCurves;

namespace Tangra.VideoOperations.LightCurves
{
    public partial class frmSelectReductionType : Form
    {
	    private VideoController m_VideoContoller;

		public frmSelectReductionType(VideoController videoContoller, IFramePlayer framePlayer)
        {
            InitializeComponent();

			m_VideoContoller = videoContoller;
            rbAsteroidal.Checked = true;

            ucStretching.Initialize(framePlayer.Video, framePlayer.CurrentFrameIndex);

			#region Configure the Reduction Settings. The same must be done in the frmConfigureReprocessing and ucPhotometry
			// Removes the Background Gradient
			cbxBackgroundMethod.Items.RemoveAt(2);
			#endregion

            SetComboboxIndexFromPhotometryReductionMethod(TangraConfig.Settings.LastUsed.ReductionType);
            cbxDigitalFilter.SelectedIndex = 0;
			SetComboboxIndexFromBackgroundMethod(TangraConfig.Settings.Photometry.BackgroundMethodDefault);

            m_MoreEnabled = TangraConfig.Settings.LastUsed.AdvancedLightCurveSettings;
            SetMoreLessOptions();

			FrameAdjustmentsPreview.Instance.ParentForm = this;
			FrameAdjustmentsPreview.Instance.FramePlayer = framePlayer;
			FrameAdjustmentsPreview.Instance.Reset(m_VideoContoller, framePlayer.CurrentFrameIndex);
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
            switch(method)
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

				default:
					cbxReductionType.SelectedIndex = 0;
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

				case TangraConfig.BackgroundMethod.BackgroundGradientFit:
					cbxBackgroundMethod.SelectedIndex = -1;
					break;

				case TangraConfig.BackgroundMethod.PSFBackground:
					cbxBackgroundMethod.SelectedIndex = 2;
					break;

				case TangraConfig.BackgroundMethod.BackgroundMedian:
					cbxBackgroundMethod.SelectedIndex = 3;
		            break;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
			if (rbLunarOccultation.Checked)
			{
				MessageBox.Show("Lunar Occultations will be supported soon.");
				return;
			}

			LightCurveReductionContext.Instance.DigitalFilter = (TangraConfig.PreProcessingFilter)cbxDigitalFilter.SelectedIndex;
			LightCurveReductionContext.Instance.NoiseMethod = ComboboxIndexToBackgroundMethod();
			LightCurveReductionContext.Instance.ReductionMethod = ComboboxIndexToPhotometryReductionMethod();
	        LightCurveReductionContext.Instance.PsfQuadratureMethod = TangraConfig.Settings.Photometry.PsfQuadrature;
			LightCurveReductionContext.Instance.FullDisappearance = cbxFullDisappearance.Checked;
			LightCurveReductionContext.Instance.HighFlickering = cbxFlickering.Checked;
			LightCurveReductionContext.Instance.WindOrShaking = cbxShaking.Checked;
			LightCurveReductionContext.Instance.StopOnLostTracking = cbxShaking.Checked && cbxStopOnLostTracking.Checked;
			LightCurveReductionContext.Instance.FieldRotation = cbxFieldRotation.Checked;
			LightCurveReductionContext.Instance.IsDriftThrough = cbxDriftTrough.Checked;

			if (rbAsteroidal.Checked)
				LightCurveReductionContext.Instance.LightCurveReductionType = LightCurveReductionType.Asteroidal;
			else if (rbMutualEvent.Checked)
				LightCurveReductionContext.Instance.LightCurveReductionType = LightCurveReductionType.MutualEvent;
			else
				LightCurveReductionContext.Instance.LightCurveReductionType = LightCurveReductionType.UntrackedMeasurement;

			if (rbRunningAverage.Checked)
				LightCurveReductionContext.Instance.FrameIntegratingMode = FrameIntegratingMode.SlidingAverage;
			else if (rbBinning.Checked)
				LightCurveReductionContext.Instance.FrameIntegratingMode = FrameIntegratingMode.SteppedAverage;
			else
				LightCurveReductionContext.Instance.FrameIntegratingMode = FrameIntegratingMode.NoIntegration;

			if (rbMedian.Checked)
				LightCurveReductionContext.Instance.PixelIntegrationType = PixelIntegrationType.Median;
			else
				LightCurveReductionContext.Instance.PixelIntegrationType = PixelIntegrationType.Mean;

			LightCurveReductionContext.Instance.NumberFramesToIntegrate = (int)nudIntegrateFrames.Value;

			if (LightCurveReductionContext.Instance.FullDisappearance &&
				LightCurveReductionContext.Instance.ReductionMethod != TangraConfig.PhotometryReductionMethod.AperturePhotometry)
			{
				DialogResult result = MessageBox.Show(
					"Your selected photometry method does not work well with full disappearances. It is recommended to use Aperture Photometry instead. Do you want to use Aperture Photometry for this reduction?",
					"Warning",
					MessageBoxButtons.YesNoCancel,
					MessageBoxIcon.Warning,
					MessageBoxDefaultButton.Button1);

				if (result == DialogResult.Cancel)
					return;
				else if (result == DialogResult.Yes)
					LightCurveReductionContext.Instance.ReductionMethod = TangraConfig.PhotometryReductionMethod.AperturePhotometry;
			}

			LightCurveReductionContext.Instance.SaveTrackingSession = false;			

			TangraConfig.Settings.LastUsed.ReductionType = (TangraConfig.PhotometryReductionMethod)cbxReductionType.SelectedIndex;
			TangraConfig.Settings.LastUsed.AdvancedLightCurveSettings = m_MoreEnabled;
			TangraConfig.Settings.Save();

			TangraContext.Current.UsingIntegration = LightCurveReductionContext.Instance.FrameIntegratingMode != FrameIntegratingMode.NoIntegration;
			TangraContext.Current.NumberFramesToIntegrate = LightCurveReductionContext.Instance.NumberFramesToIntegrate;
			m_VideoContoller.UpdateViews();

			LightCurveReductionContext.Instance.UseStretching = ucStretching.rbStretching.Checked;
			LightCurveReductionContext.Instance.UseClipping = ucStretching.rbClipping.Checked;
			LightCurveReductionContext.Instance.UseBrightnessContrast = ucStretching.rbBrightnessContrast.Checked;
			LightCurveReductionContext.Instance.FromByte = ucStretching.FromByte;
			LightCurveReductionContext.Instance.ToByte = ucStretching.ToByte;
			LightCurveReductionContext.Instance.Brightness = ucStretching.Brightness;
			LightCurveReductionContext.Instance.Contrast = ucStretching.Contrast;

	        LightCurveReductionContext.Instance.BitPix = m_VideoContoller.VideoBitPix;
			LightCurveReductionContext.Instance.MaxPixelValue = m_VideoContoller.VideoBitPix != 16 ? (uint)1 << m_VideoContoller.VideoBitPix : m_VideoContoller.VideoAav16NormVal;

            m_VideoContoller.RefreshCurrentFrame();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void rbNoIntegration_CheckedChanged(object sender, EventArgs e)
        {
			gbxPixelIntegration.Enabled = !rbNoIntegration.Checked;
			pnlFrameCount.Enabled = !rbNoIntegration.Checked;

			if (rbNoIntegration.Checked)
				FrameAdjustmentsPreview.Instance.NoIntegration();
			else
				SetupIntegrationPreView();
        }

        private bool m_MoreEnabled = false;

        private void btnMoreOrLess_Click(object sender, EventArgs e)
        {
            m_MoreEnabled = !m_MoreEnabled;
            SetMoreLessOptions();
        }

        public void SetMoreLessOptions()
        {
            if (!m_MoreEnabled)
            {
                btnMoreOrLess.Text = "More Options";

                if (tabControl1.TabPages.Count == 4)
                {
                    tabControl1.TabPages.Remove(tabIntegration);
                    tabControl1.TabPages.Remove(tabReduction);
                    tabControl1.TabPages.Remove(tabStretching);
                }
            }
            else
            {
                btnMoreOrLess.Text = "Less Options";

                if (tabControl1.TabPages.Count == 1)
                {
                    tabControl1.TabPages.Add(tabIntegration);
                    tabControl1.TabPages.Add(tabStretching);
                    tabControl1.TabPages.Add(tabReduction);
                }
            }
        }

        private void cbxShaking_CheckedChanged(object sender, EventArgs e)
        {
			cbxStopOnLostTracking.Visible = 
				TangraConfig.Settings.Tracking.SelectedEngine != TangraConfig.TrackingEngine.AdHocTracking && 
				cbxShaking.Checked;
        }

        private void rbUntrackedMeasurement_CheckedChanged(object sender, EventArgs e)
        {
            if (rbUntrackedMeasurement.Checked)
            {
                if (cbxReductionType.SelectedIndex != 0 ||
                    cbxBackgroundMethod.SelectedIndex > 1)
                {
                    MessageBox.Show(
                        "Untracked measurements can be only used with aperture photometry and average or mode background. The reduction and/or background methods have been changed accordingly.", 
                        "Settings changed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                if (cbxReductionType.SelectedIndex != 0) cbxReductionType.SelectedIndex = 0;
                if (cbxBackgroundMethod.SelectedIndex > 1) cbxBackgroundMethod.SelectedIndex = 0;
            }
        }

        private void cbxBackgroundMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rbUntrackedMeasurement.Checked)
            {
                if (cbxBackgroundMethod.SelectedIndex > 1)
                {
                    MessageBox.Show(
                        "Only average background and background mode can be used with Untracked Measurements. Please change the measurement type if you want to use a different background method.", 
                        "Invalid Background Method", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    cbxBackgroundMethod.SelectedIndex = 0;
                }
            }
        }

        private void cbxReductionType_SelectedIndexChanged(object sender, EventArgs e)
        {
			// No background selection when doing Analytical PSF quadrature
	        if (ComboboxIndexToPhotometryReductionMethod() == TangraConfig.PhotometryReductionMethod.PsfPhotometry &&
	            TangraConfig.Settings.Photometry.PsfQuadrature == TangraConfig.PsfQuadrature.Analytical)
	        {
		        // Forcing a PSF Background when Analytical Psf Quadrature is used
		        SetComboboxIndexFromBackgroundMethod(TangraConfig.BackgroundMethod.PSFBackground);
		        cbxBackgroundMethod.Enabled = false;
		        lblAnalyticalPsfInfo.Visible = true;
	        }
	        else
	        {
		        cbxBackgroundMethod.Enabled = true;
				lblAnalyticalPsfInfo.Visible = false;
	        }


	        if (rbUntrackedMeasurement.Checked)
            {
                if (cbxReductionType.SelectedIndex != 0)
                {
                    MessageBox.Show(
                        "Only aperture photometry can be used with Untracked Measurements. Please change the measurement type if you want to use a different reduction method.",
                        "Invalid Reduction Method", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    cbxReductionType.SelectedIndex = 0;
                }
            }
        }

		private void SetupIntegrationPreView()
		{
			FrameAdjustmentsPreview.Instance.Integration(
				(int)nudIntegrateFrames.Value,
				rbBinning.Checked
					? FrameIntegratingMode.SteppedAverage
					: (rbRunningAverage.Checked
							? FrameIntegratingMode.SlidingAverage
							: FrameIntegratingMode.NoIntegration),
				rbMedian.Checked
					? PixelIntegrationType.Median
					: PixelIntegrationType.Mean);			
		}

		private void nudIntegrateFrames_ValueChanged(object sender, EventArgs e)
		{
			SetupIntegrationPreView();
		}

		private void rbMean_CheckedChanged(object sender, EventArgs e)
		{
			SetupIntegrationPreView();
		}

		private void frmSelectReductionType_Move(object sender, EventArgs e)
		{
			FrameAdjustmentsPreview.Instance.MoveForm(this.Right, this.Top);
		}

		private void cbxDigitalFilter_SelectedIndexChanged(object sender, EventArgs e)
		{
			// NOTE: Nothing to do here. Digital filters are set when pressing OK and will be applied at measurement time only (i.e. they are not a pre-processing thing)
		}
    }
}
