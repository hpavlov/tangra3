using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Config;

namespace Tangra.Config.SettingPannels
{
	public partial class ucCustomizeSpectroscopy : SettingsPannel
	{
		public ucCustomizeSpectroscopy()
		{
			InitializeComponent();
		}

		public override void LoadSettings()
		{
			ucColorPickerReferenceStar.SelectedColor = TangraConfig.Settings.Spectroscopy.Colors.SpectraLine;
		}

		public override void SaveSettings()
		{
			TangraConfig.Settings.Spectroscopy.Colors.SpectraLine = ucColorPickerReferenceStar.SelectedColor;
		}
	}
}
