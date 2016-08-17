/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

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
using Tangra.Helpers;
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
		
			SetComboboxIndexFromPhotometryReductionMethod(TangraConfig.Settings.LastUsed.AsteroidalReductionType);
			cbxDigitalFilter.SelectedIndex = 0;
			SetComboboxIndexFromBackgroundMethod(TangraConfig.Settings.LastUsed.AsteroidalBackgroundReductionMethod);				
		
            m_MoreEnabled = TangraConfig.Settings.LastUsed.AdvancedLightCurveSettings;
            SetMoreLessOptions();

			FrameAdjustmentsPreview.Instance.ParentForm = this;
			FrameAdjustmentsPreview.Instance.FramePlayer = framePlayer;
			FrameAdjustmentsPreview.Instance.Reset(m_VideoContoller, framePlayer.CurrentFrameIndex);

			m_DefaultReductionSettingsModified = false;
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
                return TangraConfig.BackgroundMethod.Background3DPolynomial;
			else if (cbxBackgroundMethod.SelectedIndex == 3)
				return TangraConfig.BackgroundMethod.PSFBackground;
			else if (cbxBackgroundMethod.SelectedIndex == 4)
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

				case TangraConfig.BackgroundMethod.Background3DPolynomial:
					cbxBackgroundMethod.SelectedIndex = 2;
					break;

				case TangraConfig.BackgroundMethod.PSFBackground:
					cbxBackgroundMethod.SelectedIndex = 3;
					break;

				case TangraConfig.BackgroundMethod.BackgroundMedian:
					cbxBackgroundMethod.SelectedIndex = 4;
		            break;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
			if (rbMutualEvent.Checked)
			{
				if (!rbMutualEcl.Checked && !rbMutualOcc.Checked)
				{
					MessageBox.Show(this, "Please select if this is an Occultation or Eclipse.", "Question", MessageBoxButtons.OK, MessageBoxIcon.Question);
					return;
				}
			}

			if (rbLunarEvent.Checked)
			{
				if (!rbLunarD.Checked && !rbLunarR.Checked && !rbLunarGraze.Checked)
				{
					MessageBox.Show(this, "Please select the type of Lunar Occultation - Total Disappearance, Total Reapearance or a Grazing Occultation.", "Question", MessageBoxButtons.OK, MessageBoxIcon.Question);
					return;
				}
			}

			LightCurveReductionContext.Instance.DigitalFilter = (TangraConfig.PreProcessingFilter)cbxDigitalFilter.SelectedIndex;
			LightCurveReductionContext.Instance.NoiseMethod = ComboboxIndexToBackgroundMethod();
			LightCurveReductionContext.Instance.ReductionMethod = ComboboxIndexToPhotometryReductionMethod();
	        LightCurveReductionContext.Instance.PsfQuadratureMethod = TangraConfig.Settings.Photometry.PsfQuadrature;
			LightCurveReductionContext.Instance.FullDisappearance = rbSpecializedTracking.Checked && cbxFullDisappearance.Checked;
            LightCurveReductionContext.Instance.HighFlickeringOrLargeStars = !rbFastTracking.Checked;
            LightCurveReductionContext.Instance.WindOrShaking = !rbFastTracking.Checked;
            LightCurveReductionContext.Instance.StopOnLostTracking = !rbFastTracking.Checked && cbxStopOnLostTracking.Checked;
			LightCurveReductionContext.Instance.FieldRotation = !rbFastTracking.Checked;
            LightCurveReductionContext.Instance.IsDriftThrough = rbSpecializedTracking.Checked && cbxDriftTrough.Checked;
	        LightCurveReductionContext.Instance.LightCurveReductionSubType = ReductionSubType.Unknown;

			if (rbAsteroidal.Checked)
				LightCurveReductionContext.Instance.LightCurveReductionType = LightCurveReductionType.Asteroidal;
	        else if (rbVariableOrTransit.Checked)
				LightCurveReductionContext.Instance.LightCurveReductionType = LightCurveReductionType.VariableStarOrTransit;
			else if (rbMutualEvent.Checked)
			{
				LightCurveReductionContext.Instance.LightCurveReductionType = LightCurveReductionType.MutualEvent;
				LightCurveReductionContext.Instance.LightCurveReductionSubType = rbMutualOcc.Checked
					                                                                 ? ReductionSubType.Occultation
					                                                                 : ReductionSubType.Eclipse;
			}
			else if (rbLunarEvent.Checked)
			{
				if (rbLunarD.Checked)
					LightCurveReductionContext.Instance.LightCurveReductionType = LightCurveReductionType.TotalLunarDisappearance;
				else if (rbLunarR.Checked)
					LightCurveReductionContext.Instance.LightCurveReductionType = LightCurveReductionType.TotalLunarReppearance;
				else if (rbLunarGraze.Checked)
					LightCurveReductionContext.Instance.LightCurveReductionType = LightCurveReductionType.LunarGrazingOccultation;
				else
					throw new IndexOutOfRangeException();
			}
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

			// Analytical PSF quadrature limitations
			if (ComboboxIndexToPhotometryReductionMethod() == TangraConfig.PhotometryReductionMethod.PsfPhotometry &&
				TangraConfig.Settings.Photometry.PsfQuadrature == TangraConfig.PsfQuadrature.Analytical)
			{
				if (LightCurveReductionContext.Instance.NoiseMethod != TangraConfig.BackgroundMethod.PSFBackground &&
				    LightCurveReductionContext.Instance.NoiseMethod != TangraConfig.BackgroundMethod.Background3DPolynomial)
				{
					DialogResult result = MessageBox.Show(
						"Only 'PSF Background' and '3D-Polynomial Fit' can be used as background method when PSF photometry with analytical quadrature is used. Please choose one of those two background methods.",
						"Error",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error,
						MessageBoxDefaultButton.Button1);

					return;
				}				
			}

			// TODO: Do a check for Mutual Events - if PSF Photometry + LP Filter is used
			//
			//if (LightCurveReductionContext.Instance.LightCurveReductionType == LightCurveReductionType.MutualEvent &&
			//	LightCurveReductionContext.Instance.ReductionMethod != TangraConfig.PhotometryReductionMethod.AperturePhotometry)
			//{
			//	DialogResult result = MessageBox.Show(
			//		"Your selected photometry method does not work well with full disappearances. It is recommended to use Aperture Photometry instead. Do you want to use Aperture Photometry for this reduction?",
			//		"Warning",
			//		MessageBoxButtons.YesNoCancel,
			//		MessageBoxIcon.Warning,
			//		MessageBoxDefaultButton.Button1);

			//	if (result == DialogResult.Cancel)
			//		return;
			//	else if (result == DialogResult.Yes)
			//		LightCurveReductionContext.Instance.ReductionMethod = TangraConfig.PhotometryReductionMethod.AperturePhotometry;
			//}


			LightCurveReductionContext.Instance.SaveTrackingSession = false;

	        if (rbLunarEvent.Checked)
	        {
		        // NOTE: The photometry/background methods used for Lunar Occultations always default to Aperture Photomtry + Median Background

				if (LightCurveReductionContext.Instance.ReductionMethod != TangraConfig.PhotometryReductionMethod.AperturePhotometry ||
					LightCurveReductionContext.Instance.NoiseMethod != TangraConfig.BackgroundMethod.BackgroundMedian)
				{
					MessageBox.Show(
						"Lunar occultations can be only used with aperture photometry and median background. The reduction and/or background methods have been changed accordingly.",
						"Settings changed", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}

				LightCurveReductionContext.Instance.ReductionMethod = TangraConfig.PhotometryReductionMethod.AperturePhotometry;
		        LightCurveReductionContext.Instance.NoiseMethod = TangraConfig.BackgroundMethod.BackgroundMedian;
	        }
			if (rbMutualEvent.Checked)
	        {
				TangraConfig.Settings.LastUsed.MutualReductionType = ComboboxIndexToPhotometryReductionMethod();
				TangraConfig.Settings.LastUsed.MutualBackgroundReductionMethod = ComboboxIndexToBackgroundMethod();
		        TangraConfig.Settings.LastUsed.MutualDigitalFilter = (TangraConfig.PreProcessingFilter)cbxDigitalFilter.SelectedIndex;
	        }
	        else
	        {
				TangraConfig.Settings.LastUsed.AsteroidalReductionType = ComboboxIndexToPhotometryReductionMethod();
				TangraConfig.Settings.LastUsed.AsteroidalBackgroundReductionMethod = ComboboxIndexToBackgroundMethod();
	        }
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
			LightCurveReductionContext.Instance.MaxPixelValue = (m_VideoContoller.VideoBitPix != 16 || m_VideoContoller.VideoAav16NormVal == 0)
					? (uint)1 << m_VideoContoller.VideoBitPix 
					: m_VideoContoller.VideoAav16NormVal;

            m_VideoContoller.RefreshCurrentFrame();

			TangraContext.Current.CrashReportInfo.ReductionContext = LightCurveReductionContext.Instance.XmlSerialize();

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

                if (tabsOptions.TabPages.Count == 4)
                {
                    tabsOptions.TabPages.Remove(tabIntegration);
                    tabsOptions.TabPages.Remove(tabReduction);
                    tabsOptions.TabPages.Remove(tabStretching);
                }
            }
            else
            {
                btnMoreOrLess.Text = "Less Options";

                if (tabsOptions.TabPages.Count == 1)
                {
                    tabsOptions.TabPages.Add(tabIntegration);
                    tabsOptions.TabPages.Add(tabStretching);
                    tabsOptions.TabPages.Add(tabReduction);
                }
            }
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
			m_DefaultReductionSettingsModified = true;
        }

        private void cbxReductionType_SelectedIndexChanged(object sender, EventArgs e)
        {
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
	        m_DefaultReductionSettingsModified = true;
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
			m_DefaultReductionSettingsModified = true;
		}

	    private bool m_DefaultReductionSettingsModified = false;

		private void rbMutualEvent_CheckedChanged(object sender, EventArgs e)
		{
			if (rbMutualEvent.Checked)
			{
				rbMutualOcc.Checked = false;
				rbMutualEcl.Checked = false;
				pnlMutualType.Visible = true;
			}
			else
			{
				pnlMutualType.Visible = false;
			}

			if (!m_DefaultReductionSettingsModified)
			{
				if (rbMutualEvent.Checked)
				{
					SetComboboxIndexFromPhotometryReductionMethod(TangraConfig.Settings.LastUsed.MutualReductionType);
					cbxDigitalFilter.SelectedIndex = (int)TangraConfig.Settings.LastUsed.MutualDigitalFilter;
					SetComboboxIndexFromBackgroundMethod(TangraConfig.Settings.LastUsed.MutualBackgroundReductionMethod);
				}
				else
				{
					SetComboboxIndexFromPhotometryReductionMethod(TangraConfig.Settings.LastUsed.AsteroidalReductionType);
					cbxDigitalFilter.SelectedIndex = 0;
					SetComboboxIndexFromBackgroundMethod(TangraConfig.Settings.LastUsed.AsteroidalBackgroundReductionMethod);
				}

				m_DefaultReductionSettingsModified = false;
			}
		}

        private void frmSelectReductionType_Load(object sender, EventArgs e)
        {
            if (!m_VideoContoller.SupportsSoftwareIntegration)
                tabsOptions.TabPages.Remove(tabIntegration);

            TrackingModeChanged(this, EventArgs.Empty);
        }

		private void rbVariableOrTransit_CheckedChanged(object sender, EventArgs e)
		{
			if (rbVariableOrTransit.Checked)
			{
				m_AutoCheck = true;
				try
				{
					cbxFullDisappearance.Checked = false;
					cbxFullDisappearance.Enabled = false;
				}
				finally
				{
					m_AutoCheck = false;
				}
			}
			else
			{
				m_AutoCheck = true;
				try
				{
					cbxFullDisappearance.Checked = m_UserCheckedFullDisapearance;
					cbxFullDisappearance.Enabled = true;
				}
				finally
				{
					m_AutoCheck = false;
				}
			}
		}

	    private bool m_UserCheckedFullDisapearance = false;
	    private bool m_AutoCheck = false;
		private void rbLunarEvent_CheckedChanged(object sender, EventArgs e)
		{
			if (rbLunarEvent.Checked)
			{
				rbLunarD.Checked = false;
				rbLunarR.Checked = false;
				rbLunarGraze.Checked = false;
				pnlLunarType.Visible = true;

				m_AutoCheck = true;
				try
				{
					cbxFullDisappearance.Checked = true;
					cbxFullDisappearance.Enabled = false;
				}
				finally
				{
					m_AutoCheck = false;
				}
			}
			else
			{
				pnlLunarType.Visible = false;

				m_AutoCheck = true;
				try
				{
					cbxFullDisappearance.Checked = m_UserCheckedFullDisapearance;
					cbxFullDisappearance.Enabled = true;
				}
				finally
				{
					m_AutoCheck = false;
				}
			}
		}

		private void cbxFullDisappearance_CheckedChanged(object sender, EventArgs e)
		{
			if (!m_AutoCheck)
				m_UserCheckedFullDisapearance = cbxFullDisappearance.Checked;
		}

        private void TrackingModeChanged(object sender, EventArgs e)
        {
            if (rbFastTracking.Checked)
            {
                LightCurveReductionContext.Instance.HighFlickeringOrLargeStars = false;
                LightCurveReductionContext.Instance.WindOrShaking = false;
                LightCurveReductionContext.Instance.FieldRotation = false;

                cbxStopOnLostTracking.Visible = false;
                cbxFullDisappearance.Visible = false;
                cbxDriftTrough.Visible = false;
                lblTrackingDescr.Text = "Recommended for bright stars and stable image. Not suitable for full disappearances and drift through.";
                lblTrackingDescr.Visible = true;
            }
            else
            {
                LightCurveReductionContext.Instance.HighFlickeringOrLargeStars = true;
                LightCurveReductionContext.Instance.WindOrShaking = true;
                LightCurveReductionContext.Instance.FieldRotation = true;

                cbxStopOnLostTracking.Visible = true;

                if (rbSpecializedTracking.Checked)
                {
                    // Full Disappearance, Drift Through   
                    cbxFullDisappearance.Visible = true;
                    cbxDriftTrough.Visible = true;
                    lblTrackingDescr.Visible = false;
                }
                else
                {
                    cbxFullDisappearance.Visible = false;
                    cbxDriftTrough.Visible = false;
                    lblTrackingDescr.Text = "Good for less-optimal conditions: faint or large stars, flickering, wind, field rotation.";
                    lblTrackingDescr.Visible = true;
                }
            }
        }
    }
}
