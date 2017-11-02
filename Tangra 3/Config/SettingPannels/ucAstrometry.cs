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
	public partial class ucAstrometry : SettingsPannel
	{
	    private bool m_SettingsLoaded;

		public ucAstrometry()
		{
			InitializeComponent();
		}

		public override void LoadSettings()
		{
			cbxAstrFitMethod.SetCBXIndex((int)TangraConfig.Settings.Astrometry.Method);
			nudAstrMinimumStars.SetNUDValue(TangraConfig.Settings.Astrometry.MinimumNumberOfStars);
			nudAstrMaximumStars.SetNUDValue(TangraConfig.Settings.Astrometry.MaximumNumberOfStars);
            nudAstrMaxResidual.SetNUDValue((decimal)TangraConfig.Settings.Astrometry.MaxResidualInPixels);
            nudAstrAssumedUncertainty.SetNUDValue((decimal)TangraConfig.Settings.Astrometry.AssumedPositionUncertaintyPixels);
            nudAstrSmallestReportedUncertainty.SetNUDValue((decimal)TangraConfig.Settings.Astrometry.SmallestReportedUncertaintyArcSec);
            nudNumberOfPivots.SetNUDValue((decimal)TangraConfig.Settings.Astrometry.PyramidNumberOfPivots);

			nudAstrDistTolerance.SetNUDValue((decimal)TangraConfig.Settings.Astrometry.PyramidDistanceToleranceInPixels);
			nudAstrOptimumStars.SetNUDValue((decimal)TangraConfig.Settings.Astrometry.PyramidOptimumStarsToMatch);
			nudZoneStarIndex.SetNUDValue((decimal)TangraConfig.Settings.Astrometry.DistributionZoneStars);
			nudAstrFocLenVariation.SetNUDValue((decimal)TangraConfig.Settings.Astrometry.PyramidFocalLengthAllowance * 100);
			nudAstrAttemptTimeout.SetNUDValue((decimal)TangraConfig.Settings.Astrometry.PyramidTimeoutInSeconds);

			cbxMagBand.SetCBXIndex((int)TangraConfig.Settings.Astrometry.DefaultMagOutputBand);
			nudMagResidual.SetNUDValue((decimal)TangraConfig.Settings.Photometry.MaxResidualStellarMags);
			nudTargetVRColour.SetNUDValue((decimal)TangraConfig.Settings.Astrometry.AssumedTargetVRColour);
            cbxExportUncertainties.Checked = TangraConfig.Settings.Astrometry.ExportUncertainties;
            cbxExportHigherPositionAccuracy.Checked = TangraConfig.Settings.Astrometry.ExportHigherPositionAccuracy;

		    m_SettingsLoaded = true;
		}

		public override void SaveSettings()
		{
			TangraConfig.Settings.Astrometry.Method = (AstrometricMethod)cbxAstrFitMethod.SelectedIndex;
			TangraConfig.Settings.Astrometry.MinimumNumberOfStars = (int)nudAstrMinimumStars.Value;
			TangraConfig.Settings.Astrometry.MaximumNumberOfStars = (int)nudAstrMaximumStars.Value;
            TangraConfig.Settings.Astrometry.MaxResidualInPixels = (double)nudAstrMaxResidual.Value;
            TangraConfig.Settings.Astrometry.AssumedPositionUncertaintyPixels = (double)nudAstrAssumedUncertainty.Value;
            TangraConfig.Settings.Astrometry.SmallestReportedUncertaintyArcSec = (double)nudAstrSmallestReportedUncertainty.Value;
            TangraConfig.Settings.Astrometry.PyramidNumberOfPivots = (int)nudNumberOfPivots.Value;

			TangraConfig.Settings.Astrometry.PyramidDistanceToleranceInPixels = (double)nudAstrDistTolerance.Value;
			TangraConfig.Settings.Astrometry.PyramidOptimumStarsToMatch = (double)nudAstrOptimumStars.Value;
			TangraConfig.Settings.Astrometry.DistributionZoneStars = (int)nudZoneStarIndex.Value;

			TangraConfig.Settings.Astrometry.PyramidFocalLengthAllowance = (double)nudAstrFocLenVariation.Value / 100.0;
			TangraConfig.Settings.Astrometry.PyramidTimeoutInSeconds = (int)nudAstrAttemptTimeout.Value;

			TangraConfig.Settings.Astrometry.DefaultMagOutputBand = (TangraConfig.MagOutputBand)cbxMagBand.SelectedIndex;
			TangraConfig.Settings.Photometry.MaxResidualStellarMags = (double)nudMagResidual.Value;
			TangraConfig.Settings.Astrometry.AssumedTargetVRColour = (double)nudTargetVRColour.Value;

            TangraConfig.Settings.Astrometry.ExportUncertainties = cbxExportUncertainties.Checked;
            TangraConfig.Settings.Astrometry.ExportHigherPositionAccuracy = cbxExportHigherPositionAccuracy.Checked;
		}

        private void cbxExportHigherPositionAccuracy_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxExportHigherPositionAccuracy.Checked)
                WarnHighPrecisionAndUncertaintiesMPC();
        }

        private void cbxExportUncertainties_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxExportUncertainties.Checked)
                WarnHighPrecisionAndUncertaintiesMPC();
        }

	    private void WarnHighPrecisionAndUncertaintiesMPC()
	    {
	        if (!m_SettingsLoaded)
	            return;

	        MessageBox.Show(
                "Uncertainties and Positions with High Precision will be displayed in Tangra but will not be exported to MPC observation files as they don't support them.", 
                "Tangra",
                MessageBoxButtons.OK, 
                MessageBoxIcon.Information);
	    }
	}
}
