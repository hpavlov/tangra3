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
    public partial class ucGeneralTangra : SettingsPannel
	{
		public ucGeneralTangra()
		{
			InitializeComponent();
		}

        public override void LoadSettings()
        {
			cbPerformanceQuality.SetCBXIndex((int)TangraConfig.Settings.Generic.PerformanceQuality);
			cbxRunVideosAtFastest.Checked = TangraConfig.Settings.Generic.RunVideosOnFastestSpeed;
			cbxOnOpenOperation.SetCBXIndex((int)TangraConfig.Settings.Generic.OnOpenOperation);
			cbShowProcessingSpeed.Checked = TangraConfig.Settings.Generic.ShowProcessingSpeed;
			cbxShowCursorPosition.Checked = TangraConfig.Settings.Generic.ShowCursorPosition;
			cbxBetaUpdates.Checked = TangraConfig.Settings.Generic.AcceptBetaUpdates;
        }

        public override void SaveSettings()
        {
			TangraConfig.Settings.Generic.PerformanceQuality = (TangraConfig.PerformanceQuality)cbPerformanceQuality.SelectedIndex;

			TangraConfig.Settings.Generic.AcceptBetaUpdates = cbxBetaUpdates.Checked;
			TangraConfig.Settings.Generic.RunVideosOnFastestSpeed = cbxRunVideosAtFastest.Checked;
			TangraConfig.Settings.Generic.OnOpenOperation = (TangraConfig.OnOpenOperation)cbxOnOpenOperation.SelectedIndex;
			TangraConfig.Settings.Generic.ShowProcessingSpeed = cbShowProcessingSpeed.Checked;
			TangraConfig.Settings.Generic.ShowCursorPosition = cbxShowCursorPosition.Checked;
        }
	}
}
