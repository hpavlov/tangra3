using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Helpers;
using Tangra.Model.Config;

namespace Tangra.Config.SettingPannels
{
	public partial class ucSpectroscopy : SettingsPannel
	{
		public ucSpectroscopy()
		{
			InitializeComponent();
		}

		public override void LoadSettings()
		{
			cbxInstrumentType.SetCBXIndex((int)TangraConfig.Settings.Spectroscopy.Instrument);

			rbOrder1.Checked = TangraConfig.Settings.Spectroscopy.DefaultWavelengthCalibrationOrder == 1;
			rbOrder2.Checked = TangraConfig.Settings.Spectroscopy.DefaultWavelengthCalibrationOrder == 2;
			rbOrder3.Checked = TangraConfig.Settings.Spectroscopy.DefaultWavelengthCalibrationOrder == 3;

		    cbxAllowNegativeValues.Checked = TangraConfig.Settings.Spectroscopy.AllowNegativeValues;

		}

		public override void SaveSettings()
		{
			TangraConfig.Settings.Spectroscopy.Instrument = (TangraConfig.SpectroscopyInstrument)cbxInstrumentType.SelectedIndex;
		    TangraConfig.Settings.Spectroscopy.AllowNegativeValues = cbxAllowNegativeValues.Checked;
			if (rbOrder1.Checked)
				TangraConfig.Settings.Spectroscopy.DefaultWavelengthCalibrationOrder = 1;
			else if (rbOrder2.Checked)
				TangraConfig.Settings.Spectroscopy.DefaultWavelengthCalibrationOrder = 2;
			else if (rbOrder3.Checked)
				TangraConfig.Settings.Spectroscopy.DefaultWavelengthCalibrationOrder = 3;
		}
	}
}
