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
	public partial class ucPhotometry : SettingsPannel
	{
		public ucPhotometry()
		{
			InitializeComponent();
		}

        public override void LoadSettings()
        {
			nudPhotoAperture.SetNUDValue((decimal)TangraConfig.Settings.Photometry.DefaultSignalAperture);
			cbxPhotoSignalApertureType.SetCBXIndex((int)TangraConfig.Settings.Photometry.DefaultSignalApertureUnit);
			SetComboboxIndexFromBackgroundMethod(TangraConfig.Settings.Photometry.DefaultBackgroundMethod);
			nudInnerAnulusInApertures.SetNUDValue((decimal)TangraConfig.Settings.Photometry.AnulusInnerRadius);
			nudMinimumAnulusPixels.SetNUDValue((int)TangraConfig.Settings.Photometry.AnulusMinPixels);
			cbxPsfQuadrature.SetCBXIndex((int)TangraConfig.Settings.Photometry.PsfQuadrature);
			cbxPsfFittingMethod.SetCBXIndex((int)TangraConfig.Settings.Photometry.PsfFittingMethod);
			nudUserSpecifiedFWHM.SetNUDValue((int)TangraConfig.Settings.Photometry.UserSpecifiedFWHM);
            rbSeeingUser.Checked = TangraConfig.Settings.Photometry.UseUserSpecifiedFWHM;
        }

        public override void SaveSettings()
        {
			TangraConfig.Settings.Photometry.DefaultSignalAperture = (float)nudPhotoAperture.Value;
			TangraConfig.Settings.Photometry.DefaultSignalApertureUnit = (TangraConfig.SignalApertureUnit)cbxPhotoSignalApertureType.SelectedIndex;
			TangraConfig.Settings.Photometry.DefaultBackgroundMethod = ComboboxIndexToBackgroundMethod();
			TangraConfig.Settings.Photometry.AnulusInnerRadius = (float)nudInnerAnulusInApertures.Value;
			TangraConfig.Settings.Photometry.AnulusMinPixels = (int)nudMinimumAnulusPixels.Value;
            TangraConfig.Settings.Photometry.PsfQuadrature = (TangraConfig.PsfQuadrature)cbxPsfQuadrature.SelectedIndex;
			TangraConfig.Settings.Photometry.PsfFittingMethod = (TangraConfig.PsfFittingMethod)cbxPsfFittingMethod.SelectedIndex;
			TangraConfig.Settings.Photometry.UserSpecifiedFWHM = (float)nudUserSpecifiedFWHM.Value;
            TangraConfig.Settings.Photometry.UseUserSpecifiedFWHM = rbSeeingUser.Checked;
        }

		public TangraConfig.BackgroundMethod ComboboxIndexToBackgroundMethod()
		{
			if (cbxBackgroundMethod.SelectedIndex == 0)
				return TangraConfig.BackgroundMethod.AverageBackground;
			else if (cbxBackgroundMethod.SelectedIndex == 1)
				return TangraConfig.BackgroundMethod.BackgroundMode;
			else if (cbxBackgroundMethod.SelectedIndex == 2)
				return TangraConfig.BackgroundMethod.PSFBackground;
			else if (cbxBackgroundMethod.SelectedIndex == 3)
				return TangraConfig.BackgroundMethod.BackgroundMedian;
			else
				return TangraConfig.BackgroundMethod.AverageBackground;
		}

		public void SetComboboxIndexFromBackgroundMethod(TangraConfig.BackgroundMethod method)
		{
			switch (method)
			{
				case TangraConfig.BackgroundMethod.AverageBackground:
					cbxBackgroundMethod.SelectedIndex = 0;
					break;

				case TangraConfig.BackgroundMethod.BackgroundMode:
					cbxBackgroundMethod.SelectedIndex = 1;
					break;

				case TangraConfig.BackgroundMethod.BackgroundGradientFit:
					cbxBackgroundMethod.SelectedIndex = -1;
					break;

				case TangraConfig.BackgroundMethod.PSFBackground:
					cbxBackgroundMethod.SelectedIndex = 2;
					break;

				case TangraConfig.BackgroundMethod.BackgroundMedian:
					cbxBackgroundMethod.SelectedIndex = 3;
					break;
			}
		}

        private void rbSeeingUser_CheckedChanged(object sender, EventArgs e)
        {
            pnlUserSeeing.Enabled = rbSeeingUser.Checked;
        }
	}
}
