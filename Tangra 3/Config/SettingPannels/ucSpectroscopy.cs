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

			nudMinWavelength.SetNUDValue(TangraConfig.Settings.Spectroscopy.MinWavelength);
			nudMaxWavelength.SetNUDValue(TangraConfig.Settings.Spectroscopy.MaxWavelength);
			nudResolution.SetNUDValue(TangraConfig.Settings.Spectroscopy.AbsFluxResolution);

			if (TangraConfig.Settings.Spectroscopy.Sampling == TangraConfig.AbsFluxSampling.GaussianSmoothingAndBinning)
				rbGaussSampling.Checked = true;
			else if (TangraConfig.Settings.Spectroscopy.Sampling == TangraConfig.AbsFluxSampling.LaGrangeInterpolation)
				rbLaGrangeSampling.Checked = true;
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

			TangraConfig.Settings.Spectroscopy.MinWavelength = (int)nudMinWavelength.Value;
			TangraConfig.Settings.Spectroscopy.MaxWavelength = (int)nudMaxWavelength.Value;
			TangraConfig.Settings.Spectroscopy.AbsFluxResolution = (int)nudResolution.Value;

			if (rbGaussSampling.Checked)
				TangraConfig.Settings.Spectroscopy.Sampling = TangraConfig.AbsFluxSampling.GaussianSmoothingAndBinning;
			else if (rbLaGrangeSampling.Checked)
				TangraConfig.Settings.Spectroscopy.Sampling = TangraConfig.AbsFluxSampling.LaGrangeInterpolation;
		}
	}
}
