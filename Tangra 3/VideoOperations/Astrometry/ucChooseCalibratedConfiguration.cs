/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Astrometry;
using Tangra.Config;
using Tangra.Helpers;
using Tangra.Model.Config;
using Tangra.Model.Context;
using Tangra.Model.Video;
using Tangra.Photometry;
using Tangra.VideoOperations.Astrometry.Engine;
using Tangra.VideoOperations.LightCurves;

namespace Tangra.VideoOperations.Astrometry
{
	public partial class ucChooseCalibratedConfiguration : UserControl
	{
		private string m_SelectedCamera = null;
		private string m_SelectedScopeRecorder = null;

		private frmChooseCamera m_Form = null;

		public ucChooseCalibratedConfiguration()
		{
			InitializeComponent();

			AstrometryContext.Current.FieldSolveContext = new FieldSolveContext(); 
		}

		internal int FrameWidth;
		internal int FrameHeight;

		public void SetParentForm(frmChooseCamera form)
		{
			m_Form = form;
		}

		private void ucCameraSettings_Load(object sender, EventArgs e)
		{
			m_SelectedCamera = TangraConfig.Settings.PlateSolve.SelectedCameraModel;
			ReloadCameras();

			m_SelectedScopeRecorder = TangraConfig.Settings.PlateSolve.SelectedScopeRecorder;
			ReloadScopeRecorders();

			gbxSolved.Visible = false;
			pnlEditableConfigSettings.Visible = false;
			pnlNotSolved.Visible = false;

			SetFilterMode();

			UpdateEnabledDisabledState();
		}

		private void ReloadCameras()
		{
			cbxSavedCameras.Items.Clear();

			foreach (VideoCamera camera in TangraConfig.Settings.PlateSolve.VideoCameras)
			{
				cbxSavedCameras.Items.Add(camera);

				if (camera.Model == m_SelectedCamera)
					cbxSavedCameras.SelectedIndex = cbxSavedCameras.Items.Count - 1;
			}
		}

		private void ReloadScopeRecorders()
		{
			cbxSavedConfigurations.Items.Clear();

			foreach (TangraConfig.ScopeRecorderConfiguration entry in TangraConfig.Settings.PlateSolve.ScopeRecorders)
			{
				cbxSavedConfigurations.Items.Add(entry);

				if (entry.Title == m_SelectedScopeRecorder)
					cbxSavedConfigurations.SelectedIndex = cbxSavedConfigurations.Items.Count - 1;
			}

			if (cbxSavedConfigurations.Items.Count == 0)
			{
				cbxSavedConfigurations.Items.Add("<No Configs Available - Press 'New'>");
				cbxSavedConfigurations.SelectedIndex = 0;
			}
		}

		private void UpdateEnabledDisabledState()
		{
			VideoCamera camera = cbxSavedCameras.SelectedItem as VideoCamera;
			TangraConfig.ScopeRecorderConfiguration config = cbxSavedConfigurations.SelectedItem as TangraConfig.ScopeRecorderConfiguration;

			btnOK.Enabled = camera != null && config != null;

			if (camera != null && config != null)
			{
				CheckSolvedPlateConstants(camera, config);
			}
			else
			{
				gbxSolved.Visible = false;
				pnlEditableConfigSettings.Visible = false;
				pnlNotSolved.Visible = false;
			}
		}

		private void cbxSavedCameras_SelectedIndexChanged(object sender, EventArgs e)
		{
			VideoCamera camera = cbxSavedCameras.SelectedItem as VideoCamera;

			if (camera == null)
			{
				tbxPixelX.Text = string.Empty;
				tbxPixelY.Text = string.Empty;
				tbxMatrixWidth.Text = string.Empty;
				tbxMatrixHeight.Text = string.Empty;
			}
			else
			{
				tbxPixelX.Text = camera.CCDMetrics.CellWidth.ToString("0.0#");
				tbxPixelY.Text = camera.CCDMetrics.CellHeight.ToString("0.0#");
				tbxMatrixWidth.Text = camera.CCDMetrics.MatrixWidth.ToString();
				tbxMatrixHeight.Text = camera.CCDMetrics.MatrixHeight.ToString();
			}

			UpdateEnabledDisabledState();
		}

		private void btnNew_Click(object sender, EventArgs e)
		{
			frmEditCamera frmNewCamera = new frmEditCamera(null);
			if (frmNewCamera.ShowDialog(this) == DialogResult.OK)
			{
				m_SelectedCamera = frmNewCamera.Camera.Model;
				ReloadCameras();
			}
		}

		private void btnDelete_Click(object sender, EventArgs e)
		{
			VideoCamera camera = cbxSavedCameras.SelectedItem as VideoCamera;

			if (camera != null)
			{
				if (MessageBox.Show(this,
					string.Format("Are you sure you want to delete '{0}'", camera.Model),
					"Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
				{
					TangraConfig.Settings.PlateSolve.VideoCameras.Remove(camera);
					if (TangraConfig.Settings.PlateSolve.SelectedCameraModel == camera.Model)
						TangraConfig.Settings.PlateSolve.SelectedCameraModel = null;

					TangraConfig.Settings.Save();

					m_SelectedCamera = null;
					ReloadCameras();
				}
			}
		}

		private void btnEditCamera_Click(object sender, EventArgs e)
		{
			VideoCamera camera = cbxSavedCameras.SelectedItem as VideoCamera;

			if (camera != null)
			{
				frmEditCamera frmNewCamera = new frmEditCamera(camera);
				if (frmNewCamera.ShowDialog(this) == DialogResult.OK)
				{
					m_SelectedCamera = frmNewCamera.Camera.Model;
					ReloadCameras();
				}
			}
		}

		private void btnDelConfig_Click(object sender, EventArgs e)
		{
			TangraConfig.ScopeRecorderConfiguration entry = cbxSavedConfigurations.SelectedItem as TangraConfig.ScopeRecorderConfiguration;

			if (entry != null)
			{
				if (MessageBox.Show(this,
					string.Format("Are you sure you want to delete '{0}' and all calibrations of this configuration?", entry.Title),
					"Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
				{
				    TangraConfig.Settings.PlateSolve.SolvedPlateConstants.RemoveAll(c => c.ScopeRecoderConfig == entry.Title);
					TangraConfig.Settings.PlateSolve.ScopeRecorders.Remove(entry);

					if (TangraConfig.Settings.PlateSolve.SelectedScopeRecorder == entry.Title)
						TangraConfig.Settings.PlateSolve.SelectedScopeRecorder = null;

					TangraConfig.Settings.Save();

					m_SelectedScopeRecorder = null;
					ReloadScopeRecorders();
					UpdateEnabledDisabledState();
				}
			}
		}

		private void btnNewConfig_Click(object sender, EventArgs e)
		{
			if (cbxSavedCameras.SelectedItem != null)
			{
				VideoCamera camera = cbxSavedCameras.SelectedItem as VideoCamera;
				frmEditConfigName frmNewConfig = new frmEditConfigName(null, camera, FrameWidth, FrameHeight, null);

				if (frmNewConfig.ShowDialog(this) == DialogResult.OK)
				{
					m_SelectedScopeRecorder = frmNewConfig.Config.Title;
					TangraConfig.ScopeRecorderConfiguration entry = TangraConfig.Settings.PlateSolve.ScopeRecorders.FirstOrDefault(r => r.Title == m_SelectedScopeRecorder);

					if (frmNewConfig.PltConst != null)
					{
						Rectangle plateRect = new Rectangle(0, 0, TangraContext.Current.FrameWidth, TangraContext.Current.FrameHeight);

						TangraConfig.PersistedPlateConstants pltConst = new TangraConfig.PersistedPlateConstants();

						pltConst.EffectiveFocalLength = frmNewConfig.PltConst.EffectiveFocalLength;
						pltConst.EffectivePixelWidth = frmNewConfig.PltConst.EffectivePixelWidth;
						pltConst.EffectivePixelHeight = frmNewConfig.PltConst.EffectivePixelHeight;

						TangraConfig.Settings.PlateSolve.SetPlateConstants(camera, entry, plateRect, pltConst);
						TangraConfig.Settings.Save();
					}

					ReloadScopeRecorders();
					CheckSolvedPlateConstants(camera, entry);
				}
			}
			else
				cbxSavedCameras.Focus();
		}

		private void cbxSavedConfigurations_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateEnabledDisabledState();
		}

		private void CheckSolvedPlateConstants(VideoCamera camera, TangraConfig.ScopeRecorderConfiguration config)
		{
			TangraConfig.PersistedPlateConstants pltCopnst = 
				TangraConfig.Settings.PlateSolve.GetPlateConstants(
					camera, 
					config,
					new Rectangle(0, 0, TangraContext.Current.FrameWidth, TangraContext.Current.FrameHeight));

			if (pltCopnst == null)
			{
				gbxSolved.Visible = false;
				pnlNotSolved.Visible = true;

				cbxSolveConstantsNow.Checked = true;
				cbxSolveConstantsNow.Visible = true;
			}
			else
			{
				gbxSolved.Visible = true;

				cbxSolveConstantsNow.Checked = false;
				pnlNotSolved.Visible = false;

				tbxSolvedCellX.Text = pltCopnst.EffectivePixelWidth.ToString("0.000");
				tbxSolvedCellY.Text = pltCopnst.EffectivePixelHeight.ToString("0.000");
				tbxSolvedFocalLength.Text = pltCopnst.EffectiveFocalLength.ToString("0.0");
			}

			pnlEditableConfigSettings.Visible = true;

			toolTip.SetToolTip(lblLimitingMagnitude, string.Format("  The limiting magnitude when {0}\r\nis used with this configuration", camera.Model));
			toolTip.SetToolTip(nudLimitMagnitude, string.Format("  The limiting magnitude when {0}\r\nis used with this configuration", camera.Model));

			m_Setting = true;
			try
			{
				nudLimitMagnitude.Value = (decimal)config.LimitingMagnitudes[camera.Model];
			    m_LimitingMagnitudeChanged = false;

				cbxFlipVertically.Checked = config.FlipVertically;
				cbxFlipHorizontally.Checked = config.FlipHorizontally;
			}
			finally
			{
				m_Setting = false;
			}
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			VideoCamera camera = cbxSavedCameras.SelectedItem as VideoCamera;
			TangraConfig.ScopeRecorderConfiguration config = cbxSavedConfigurations.SelectedItem as TangraConfig.ScopeRecorderConfiguration;

			if (camera != null && config != null)
			{
				Rectangle rawFrame = config.RawFrameSizes[camera.Model];
				if (rawFrame == Rectangle.Empty)
				{
					rawFrame = new Rectangle(0, 0, FrameWidth, FrameHeight);
					config.RawFrameSizes[camera.Model] = rawFrame;
				}

                m_LimitingMagnitudeChanged |= config.LimitingMagnitudes[camera.Model] != (double)nudLimitMagnitude.Value; 
                config.LimitingMagnitudes[camera.Model] = (double)nudLimitMagnitude.Value;
                if (m_LimitingMagnitudeChanged && config.LimitingMagnitudes[camera.Model] < 13)
                {
                    MessageBox.Show("Your limitting magnitude may result in too few stars being used for a fit. Unless your field of view is a degree or more it is recommended to use at least limitting magnitude of 13.\r\n\r\nThe limiting magnitude has the meaning of 'What is the faintest star you can record with this equipment using the maximum useful exposure'. For non integrating video cameras it is recommended to use one magnitude fainter than what you have determined experimentally to be your limiting magnitude.", "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    m_LimitingMagnitudeChanged = false;
                }

                config.FlipVertically = cbxFlipVertically.Checked;
                config.FlipHorizontally = cbxFlipHorizontally.Checked;

                TangraConfig.Settings.Save();

				if (rawFrame.Height != FrameHeight ||
					rawFrame.Width != FrameWidth)
				{
					MessageBox.Show(
						"The opened video frame size is incompatible with the selected configuration. May be " +
						"the recording equipment is different? Please create a new configuration.",
						"Wrong Configuration", MessageBoxButtons.OK, MessageBoxIcon.Error);

					cbxSavedConfigurations.Focus();
					return;
				}
				else if (rawFrame == Rectangle.Empty)
				{
					// This is an existing configuration with no parameters for this frame size, so create one 
					config.RawFrameSizes[camera.Model] = new Rectangle(0, 0, FrameWidth, FrameHeight);
				}

				m_Form.IsNewConfiguration = config.IsNew;
				m_Form.SolvePlateConstantsNow = cbxSolveConstantsNow.Checked;

				TangraConfig.Settings.PlateSolve.SelectedCameraModel = camera.Model;
				TangraConfig.Settings.PlateSolve.SelectedScopeRecorder = config.Title;
				TangraConfig.Settings.PlateSolve.UseLowPassForAstrometry = miLowPass.Checked;
				TangraConfig.Settings.Save();

				m_Form.DialogResult = DialogResult.OK;
				m_Form.Close();
			}
		}


		private void btnRecalibrate_Click(object sender, EventArgs e)
		{
			VideoCamera camera = cbxSavedCameras.SelectedItem as VideoCamera;
			TangraConfig.ScopeRecorderConfiguration config = cbxSavedConfigurations.SelectedItem as TangraConfig.ScopeRecorderConfiguration;

			if (camera != null && config != null)
			{
				m_Form.SolvePlateConstantsNow = true;

				TangraConfig.Settings.PlateSolve.SelectedCameraModel = camera.Model;
				TangraConfig.Settings.PlateSolve.SelectedScopeRecorder = config.Title;
				TangraConfig.Settings.Save();

				m_Form.DialogResult = DialogResult.OK;
				m_Form.Close();
			}
		}

		private void btnEditConfiguration_Click(object sender, EventArgs e)
		{
			TangraConfig.ScopeRecorderConfiguration entry = cbxSavedConfigurations.SelectedItem as TangraConfig.ScopeRecorderConfiguration;
			VideoCamera camera = cbxSavedCameras.SelectedItem as VideoCamera;
			Rectangle plateRect = new Rectangle(0, 0, TangraContext.Current.FrameWidth, TangraContext.Current.FrameHeight);

			TangraConfig.PersistedPlateConstants pltConst = TangraConfig.Settings.PlateSolve.GetPlateConstants(camera, entry, plateRect);

			if (entry != null && camera != null)
			{
				frmEditConfigName frmNewConfig = new frmEditConfigName(entry, camera, FrameWidth, FrameHeight, pltConst);
				if (frmNewConfig.ShowDialog(this) == DialogResult.OK)
				{
					m_SelectedScopeRecorder = frmNewConfig.Config.Title;

					if (frmNewConfig.UpdatePlateConstants && frmNewConfig.PltConst != null)
					{
						pltConst = frmNewConfig.PltConst;

						TangraConfig.Settings.PlateSolve.SetPlateConstants(camera, entry, plateRect, pltConst);
					}

					ReloadScopeRecorders();
					CheckSolvedPlateConstants(camera, entry);
				}
			}
		}

		private bool m_Setting = false;
	    private bool m_LimitingMagnitudeChanged = false;
		private void UpdateCurrentConfigValues()
		{
			VideoCamera camera = cbxSavedCameras.SelectedItem as VideoCamera;
			TangraConfig.ScopeRecorderConfiguration config = cbxSavedConfigurations.SelectedItem as TangraConfig.ScopeRecorderConfiguration;

			if (camera != null && config != null && !m_Setting)
			{
				Rectangle rawFrame = config.RawFrameSizes[camera.Model];
				if (rawFrame == Rectangle.Empty)
					config.RawFrameSizes[camera.Model] = new Rectangle(0, 0, FrameWidth, FrameHeight);

#if ASTROMETRY_DEBUG
				Trace.Assert(config.RawFrameSizes[camera.Model].Width == FrameWidth);
				Trace.Assert(config.RawFrameSizes[camera.Model].Height == FrameHeight);
#endif

			    m_LimitingMagnitudeChanged = config.LimitingMagnitudes[camera.Model] != (double) nudLimitMagnitude.Value;                
				config.LimitingMagnitudes[camera.Model] = (double)nudLimitMagnitude.Value;
				config.FlipVertically = cbxFlipVertically.Checked;
				config.FlipHorizontally = cbxFlipHorizontally.Checked;

				TangraConfig.Settings.Save();
			}
		}

		private void nudLimitMagnitude_ValueChanged(object sender, EventArgs e)
		{
			UpdateCurrentConfigValues();
		}

		private void cbxFlipVertically_CheckedChanged(object sender, EventArgs e)
		{
			UpdateCurrentConfigValues();
		}

		private void cbxFlipHorizontally_CheckedChanged(object sender, EventArgs e)
		{
			UpdateCurrentConfigValues();
		}

		private void btnPreProcessingFilter_Click(object sender, EventArgs e)
		{
			contextMenuFilter.Show(
				btnPreProcessingFilter,
				new Point(0, 0),
				ToolStripDropDownDirection.BelowRight);
		}

		private void contextMenuFilter_Opening(object sender, CancelEventArgs e)
		{
			if (AstrometryContext.Current.FieldSolveContext.UseFilter == TangraConfig.PreProcessingFilter.NoFilter)
				miNoFilter.Checked = true;
			else if (AstrometryContext.Current.FieldSolveContext.UseFilter == TangraConfig.PreProcessingFilter.LowPassFilter)
				miLowPass.Checked = true;
		}

		private bool m_NoFilterSetting = false;

		private void miFilter_CheckedChanged(object sender, EventArgs e)
		{
			if (m_NoFilterSetting) return;

			ToolStripMenuItem clickedItem = sender as ToolStripMenuItem;
			if (clickedItem == miNoFilter)
				AstrometryContext.Current.FieldSolveContext.UseFilter = TangraConfig.PreProcessingFilter.NoFilter;
			else if (clickedItem == miLowPass)
				AstrometryContext.Current.FieldSolveContext.UseFilter = TangraConfig.PreProcessingFilter.LowPassFilter;

			SetFilterMode();
		}

		private void SetFilterMode()
		{
			m_NoFilterSetting = true;
			try
			{
				miNoFilter.Checked = false;
				miLowPass.Checked = false;

				if (AstrometryContext.Current.FieldSolveContext.UseFilter == TangraConfig.PreProcessingFilter.NoFilter)
				{
					btnPreProcessingFilter.Text = "No Filter";
					miNoFilter.Checked = true;
				}
				else if (AstrometryContext.Current.FieldSolveContext.UseFilter == TangraConfig.PreProcessingFilter.LowPassFilter)
				{
					btnPreProcessingFilter.Text = "Low-Pass Filter";
					miLowPass.Checked = true;
				}
			}
			finally
			{
				m_NoFilterSetting = false;
			}
		}

		private void linkLblAboutCalibration_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			ShellHelper.OpenUrl("http://www.hristopavlov.net/Tangra/Calibration");
		}

	}
}