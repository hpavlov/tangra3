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
    public partial class ucCompatibility : SettingsPannel
	{
        public ucCompatibility()
		{
			InitializeComponent();
		}

        public override void LoadSettings()
        {
            cbxPsfOptimization.SetCBXIndex((int)TangraConfig.Settings.Tuning.PsfMode);
            rbSimplifiedTrackerNative.Checked = TangraConfig.Settings.Tracking.UseNativeTracker;
			rbSimplifiedTrackerManaged.Checked = !TangraConfig.Settings.Tracking.UseNativeTracker;
        }

        public override void SaveSettings()
        {
            TangraConfig.Settings.Tuning.PsfMode = (TangraConfig.PSFFittingMode)cbxPsfOptimization.SelectedIndex;
            TangraConfig.Settings.Tracking.UseNativeTracker = rbSimplifiedTrackerNative.Checked;
        }
	}
}
