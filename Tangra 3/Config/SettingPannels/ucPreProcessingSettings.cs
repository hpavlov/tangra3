using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Config;
using Tangra.PInvoke;

namespace Tangra.Config.SettingPannels
{
	public partial class ucPreProcessingSettings : SettingsPannel
	{
		public ucPreProcessingSettings()
		{
			InitializeComponent();			
		}

		public override void LoadSettings()
		{
			cbxDarkFrameAdjustToMedian.Checked = TangraConfig.Settings.Generic.DarkFrameAdjustLevelToMedian;
		}

		public override void SaveSettings()
		{
			TangraConfig.Settings.Generic.DarkFrameAdjustLevelToMedian = cbxDarkFrameAdjustToMedian.Checked;
		}

		public override void OnPostSaveSettings()
		{
			TangraCore.PreProcessors.SetDarkFrameAdjustLevelToMedian();
		}
	}
}
