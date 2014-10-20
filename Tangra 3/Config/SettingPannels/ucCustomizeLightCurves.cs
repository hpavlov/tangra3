/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

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
            cbxAllowMagnitudeDisplay.Checked = TangraConfig.Settings.Special.AllowLCMagnitudeDisplay;
        }

        public override void SaveSettings()
        {
			TangraConfig.Settings.Color.Saturation = cpPhotometrySaturated.SelectedColor;
			TangraConfig.Settings.Color.Target1 = cpPhotometryTarget.SelectedColor;
			TangraConfig.Settings.Color.Target2 = cpPhotometryComparison1.SelectedColor;
			TangraConfig.Settings.Color.Target3 = cpPhotometryComparison2.SelectedColor;
			TangraConfig.Settings.Color.Target4 = cpPhotometryComparison3.SelectedColor;
            TangraConfig.Settings.Special.AllowLCMagnitudeDisplay = cbxAllowMagnitudeDisplay.Checked;
        }
	}
}
