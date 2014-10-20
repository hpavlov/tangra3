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
			nudSubPixelSquare.SetNUDValue(TangraConfig.Settings.Photometry.SubPixelSquareSize);
			cbxPhotoSignalApertureType.SetCBXIndex((int)TangraConfig.Settings.Photometry.SignalApertureUnitDefault);

			SetComboboxIndexFromBackgroundMethod(TangraConfig.Settings.Photometry.BackgroundMethodDefault);
			nudInnerAnnulusInApertures.SetNUDValue((decimal)TangraConfig.Settings.Photometry.AnnulusInnerRadius);
			nudMinimumAnnulusPixels.SetNUDValue((int)TangraConfig.Settings.Photometry.AnnulusMinPixels);
			cbxPsfQuadrature.SetCBXIndex((int)TangraConfig.Settings.Photometry.PsfQuadrature);
			cbxPsfFittingMethod.SetCBXIndex((int)TangraConfig.Settings.Photometry.PsfFittingMethod);
			nudUserSpecifiedFWHM.SetNUDValue((int)TangraConfig.Settings.Photometry.UserSpecifiedFWHM);
            rbSeeingUser.Checked = TangraConfig.Settings.Photometry.UseUserSpecifiedFWHM;
			nudSNFrameWindow.SetNUDValue(TangraConfig.Settings.Photometry.SNFrameWindow);

			rb3DFirstOrder.Checked = TangraConfig.Settings.Photometry.Background3DPoly.Order == 1;
			rb3DSecondOrder.Checked = TangraConfig.Settings.Photometry.Background3DPoly.Order == 2;
			rb3DThirdOrder.Checked = TangraConfig.Settings.Photometry.Background3DPoly.Order == 3;
        }

        public override void SaveSettings()
        {
			TangraConfig.Settings.Photometry.DefaultSignalAperture = (float)nudPhotoAperture.Value;
			TangraConfig.Settings.Photometry.SubPixelSquareSize = (int)nudSubPixelSquare.Value;

			TangraConfig.Settings.Photometry.SignalApertureUnitDefault = (TangraConfig.SignalApertureUnit)cbxPhotoSignalApertureType.SelectedIndex;
			TangraConfig.Settings.Photometry.BackgroundMethodDefault = ComboboxIndexToBackgroundMethod();
			TangraConfig.Settings.Photometry.AnnulusInnerRadius = (float)nudInnerAnnulusInApertures.Value;
			TangraConfig.Settings.Photometry.AnnulusMinPixels = (int)nudMinimumAnnulusPixels.Value;
            TangraConfig.Settings.Photometry.PsfQuadrature = (TangraConfig.PsfQuadrature)cbxPsfQuadrature.SelectedIndex;
			TangraConfig.Settings.Photometry.PsfFittingMethod = (TangraConfig.PsfFittingMethod)cbxPsfFittingMethod.SelectedIndex;
			TangraConfig.Settings.Photometry.UserSpecifiedFWHM = (float)nudUserSpecifiedFWHM.Value;
            TangraConfig.Settings.Photometry.UseUserSpecifiedFWHM = rbSeeingUser.Checked;
			TangraConfig.Settings.Photometry.SNFrameWindow = (int)nudSNFrameWindow.Value;

			if (rb3DFirstOrder.Checked)
				TangraConfig.Settings.Photometry.Background3DPoly.Order = 1;
			else if (rb3DSecondOrder.Checked)
				TangraConfig.Settings.Photometry.Background3DPoly.Order = 2;
			else if (rb3DThirdOrder.Checked)
				TangraConfig.Settings.Photometry.Background3DPoly.Order = 3;
        }

		public override bool ValidateSettings()
		{
			return true;
		}

		public TangraConfig.BackgroundMethod ComboboxIndexToBackgroundMethod()
		{
			if (cbxBackgroundMethod.SelectedIndex == 0)
				return TangraConfig.BackgroundMethod.AverageBackground;
			else if (cbxBackgroundMethod.SelectedIndex == 1)
				return TangraConfig.BackgroundMethod.BackgroundMode;
			else if (cbxBackgroundMethod.SelectedIndex == 2)
				return TangraConfig.BackgroundMethod.Background3DPolynomial;
			else if (cbxBackgroundMethod.SelectedIndex == 3)
				return TangraConfig.BackgroundMethod.PSFBackground;
			else if (cbxBackgroundMethod.SelectedIndex == 4)
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

				case TangraConfig.BackgroundMethod.Background3DPolynomial:
					cbxBackgroundMethod.SelectedIndex = 2;
					break;

				case TangraConfig.BackgroundMethod.PSFBackground:
					cbxBackgroundMethod.SelectedIndex = 3;
					break;

				case TangraConfig.BackgroundMethod.BackgroundMedian:
					cbxBackgroundMethod.SelectedIndex = 4;
					break;
			}
		}

        private void rbSeeingUser_CheckedChanged(object sender, EventArgs e)
        {
            pnlUserSeeing.Enabled = rbSeeingUser.Checked;
        }

		private void cbxPsfFittingMethod_SelectedIndexChanged(object sender, EventArgs e)
		{
			pnlSeeingSettings.Visible = cbxPsfFittingMethod.SelectedIndex == (int)TangraConfig.PsfFittingMethod.LinearFitOfAveragedModel;
		}
	}
}
