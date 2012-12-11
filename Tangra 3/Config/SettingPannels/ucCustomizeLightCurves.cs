using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Config;
using Tangra.Model.Config;

namespace Tangra.Config.SettingPannels
{
	public partial class ucCustomizeLightCurves : SettingsPannel
	{
		public ucCustomizeLightCurves()
		{
			InitializeComponent();
		}

        public override void LoadSettings()
        {
			cpPhotometryTarget.SelectedColor = TangraConfig.Settings.Color.Target1;
			cpPhotometryComparison1.SelectedColor = TangraConfig.Settings.Color.Target2;
			cpPhotometryComparison2.SelectedColor = TangraConfig.Settings.Color.Target3;
			cpPhotometryComparison3.SelectedColor = TangraConfig.Settings.Color.Target4;
        }

        public override void SaveSettings()
        {
			TangraConfig.Settings.Color.Saturation = cpPhotometrySaturated.SelectedColor;
			TangraConfig.Settings.Color.Target1 = cpPhotometryTarget.SelectedColor;
			TangraConfig.Settings.Color.Target2 = cpPhotometryComparison1.SelectedColor;
			TangraConfig.Settings.Color.Target3 = cpPhotometryComparison2.SelectedColor;
			TangraConfig.Settings.Color.Target4 = cpPhotometryComparison3.SelectedColor;
        }
	}
}
