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
        }

		private void cbxRecoverFromLostTracking_CheckedChanged(object sender, EventArgs e)
		{
			pnlRecoverySettings.Enabled = cbxRecoverFromLostTracking.Checked;
		}

		private void tbRecoveryTolerance_ValueChanged(object sender, EventArgs e)
		{
			TangraConfig.Settings.Special.LostTrackingToleranceLevel = tbRecoveryTolerance.Value;
		}
	}
}
