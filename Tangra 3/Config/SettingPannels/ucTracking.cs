using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Config;
using Tangra.Config.SettingPannels;
using Tangra.Helpers;
using Tangra.Model.Config;

namespace Tangra.Config.SettingPannels
{
	public partial class ucTracking : SettingsPannel
	{
		public ucTracking()
		{
			InitializeComponent();
		}

        public override void LoadSettings()
        {
			nudRefiningFrames.SetNUDValue(TangraConfig.Settings.Tracking.RefiningFrames);
			cbWarnOnUnsatisfiedGuidingRequirements.Checked = TangraConfig.Settings.Tracking.WarnOnUnsatisfiedGuidingRequirements;
			nudMinAboveMedian.SetNUDValue(TangraConfig.Settings.Special.AboveMedianThreasholdForGuiding);
			nudMinSNRatio.SetNUDValue((decimal)TangraConfig.Settings.Special.SignalNoiseForGuiding);

			cbxRecoverFromLostTracking.Checked = TangraConfig.Settings.Tracking.RecoverFromLostTracking;
			tbRecoveryTolerance.Value = TangraConfig.Settings.Special.LostTrackingToleranceLevel;
			cbxPlaySound.Checked = TangraConfig.Settings.Tracking.PlaySound;

			nudMinAboveMedian.SetNUDValue((double)TangraConfig.Settings.Special.StarFinderAboveNoiseLevel);
	        cbxTrackingEngine.SelectedIndex = (int)TangraConfig.Settings.Tracking.SelectedEngine;

			nudMaxElongation.SetNUDValue((double)TangraConfig.Settings.Tracking.AdHokMaxElongation);
	        cbxTestPSFElongation.Checked = TangraConfig.Settings.Tracking.CheckElongation;
			nudMaxElongation.Enabled = cbxTestPSFElongation.Checked;

			nudMinFWHM.SetNUDValue((double)TangraConfig.Settings.Tracking.AdHokMinFWHM);
			nudMaxFWHM.SetNUDValue((double)TangraConfig.Settings.Tracking.AdHokMaxFWHM);
			nudDetectionCertainty.SetNUDValue((double)TangraConfig.Settings.Tracking.AdHokMinCertainty);
			cbxNativeTracker.Checked = TangraConfig.Settings.Tracking.UseNativeTracker;
        }

        public override void SaveSettings()
        {
			TangraConfig.Settings.Tracking.RefiningFrames = (byte)nudRefiningFrames.Value;
			TangraConfig.Settings.Tracking.WarnOnUnsatisfiedGuidingRequirements = cbWarnOnUnsatisfiedGuidingRequirements.Checked;
			TangraConfig.Settings.Special.AboveMedianThreasholdForGuiding = (int)nudMinAboveMedian.Value;
			TangraConfig.Settings.Special.SignalNoiseForGuiding = (float)nudMinSNRatio.Value;

			TangraConfig.Settings.Tracking.RecoverFromLostTracking = cbxRecoverFromLostTracking.Checked;
			TangraConfig.Settings.Tracking.PlaySound = cbxPlaySound.Checked;
			TangraConfig.Settings.Special.StarFinderAboveNoiseLevel = (uint)nudMinAboveMedian.Value;
			TangraConfig.Settings.Tracking.SelectedEngine = (TangraConfig.TrackingEngine)cbxTrackingEngine.SelectedIndex;

			TangraConfig.Settings.Tracking.AdHokMaxElongation = (float)nudMaxElongation.Value;
			TangraConfig.Settings.Tracking.CheckElongation = cbxTestPSFElongation.Checked;
			nudMaxElongation.Enabled = cbxTestPSFElongation.Checked;

			TangraConfig.Settings.Tracking.AdHokMinFWHM = (float)nudMinFWHM.Value;
			TangraConfig.Settings.Tracking.AdHokMaxFWHM = (float)nudMaxFWHM.Value;
			TangraConfig.Settings.Tracking.AdHokMinCertainty = (float)nudDetectionCertainty.Value;
			TangraConfig.Settings.Tracking.UseNativeTracker = cbxNativeTracker.Checked;
        }

		private void cbxRecoverFromLostTracking_CheckedChanged(object sender, EventArgs e)
		{
			pnlRecoverySettings.Enabled = cbxRecoverFromLostTracking.Checked;
		}

		private void tbRecoveryTolerance_ValueChanged(object sender, EventArgs e)
		{
			TangraConfig.Settings.Special.LostTrackingToleranceLevel = tbRecoveryTolerance.Value;
		}

		private void cbxTestPSFElongation_CheckedChanged(object sender, EventArgs e)
		{
			nudMaxElongation.Enabled = cbxTestPSFElongation.Checked;
		}
	}
}
