﻿using System;
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
		public ucAstrometry()
		{
			InitializeComponent();
		}

		public override void LoadSettings()
		{
			cbxAstrFitMethod.SetCBXIndex((int)TangraConfig.Settings.Astrometry.Method);
			nudAstrMinimumStars.SetNUDValue(TangraConfig.Settings.Astrometry.MinimumNumberOfStars);
			nudAstrMaximumStars.SetNUDValue(TangraConfig.Settings.Astrometry.MaximumNumberOfStars);
			nudMaxElongation.SetNUDValue((decimal)TangraConfig.Settings.Astrometry.MaximumPSFElongation);
			nudAstrMaxResidual.SetNUDValue((decimal)TangraConfig.Settings.Astrometry.MaxResidual);
			cbForceStellarObjectRequirements.Checked = TangraConfig.Settings.Astrometry.PyramidRemoveNonStellarObject;

			nudAstrDistTolerance.SetNUDValue((decimal)TangraConfig.Settings.Astrometry.PyramidDistanceToleranceInPixels);
			nudAstrOptimumStars.SetNUDValue((decimal)TangraConfig.Settings.Astrometry.PyramidOptimumStarsToMatch);
			nudZoneStarIndex.SetNUDValue((decimal)TangraConfig.Settings.Astrometry.DistributionZoneStars);
			nudLimitMagDetection.SetNUDValue((decimal)TangraConfig.Settings.Astrometry.LimitReferenceStarDetection);
			nudMinFWHM.SetNUDValue((decimal)TangraConfig.Settings.Astrometry.MinReferenceStarFWHM);
			nudMaxFWHM.SetNUDValue((decimal)TangraConfig.Settings.Astrometry.MaxReferenceStarFWHM);
			nudAstrFocLenVariation.SetNUDValue((decimal)TangraConfig.Settings.Astrometry.PyramidFocalLengthAllowance * 100);
			nudAstrAttemptTimeout.SetNUDValue((decimal)TangraConfig.Settings.Astrometry.PyramidTimeoutInSeconds);

			cbxMagBand.SetCBXIndex((int)TangraConfig.Settings.Astrometry.DefaultMagOutputBand);
			nudMagResidual.SetNUDValue((decimal)TangraConfig.Settings.Photometry.MaxResidualStellarMags);
			nudTargetVRColour.SetNUDValue((decimal)TangraConfig.Settings.Astrometry.AssumedTargetVRColour);
		}

		public override void SaveSettings()
		{
			TangraConfig.Settings.Astrometry.Method = (AstrometricMethod)cbxAstrFitMethod.SelectedIndex;
			TangraConfig.Settings.Astrometry.MinimumNumberOfStars = (int)nudAstrMinimumStars.Value;
			TangraConfig.Settings.Astrometry.MaximumNumberOfStars = (int)nudAstrMaximumStars.Value;
			TangraConfig.Settings.Astrometry.MaximumPSFElongation = (int)nudMaxElongation.Value;
			TangraConfig.Settings.Astrometry.MaxResidual = (double)nudAstrMaxResidual.Value;
			TangraConfig.Settings.Astrometry.PyramidRemoveNonStellarObject = cbForceStellarObjectRequirements.Checked;

			TangraConfig.Settings.Astrometry.PyramidDistanceToleranceInPixels = (double)nudAstrDistTolerance.Value;
			TangraConfig.Settings.Astrometry.PyramidOptimumStarsToMatch = (double)nudAstrOptimumStars.Value;
			TangraConfig.Settings.Astrometry.DistributionZoneStars = (int)nudZoneStarIndex.Value;
			TangraConfig.Settings.Astrometry.LimitReferenceStarDetection = (double)nudLimitMagDetection.Value;
			TangraConfig.Settings.Astrometry.MinReferenceStarFWHM = (double)nudMinFWHM.Value;
			TangraConfig.Settings.Astrometry.MaxReferenceStarFWHM = (double)nudMaxFWHM.Value;

			TangraConfig.Settings.Astrometry.PyramidFocalLengthAllowance = (double)nudAstrFocLenVariation.Value / 100.0;
			TangraConfig.Settings.Astrometry.PyramidTimeoutInSeconds = (int)nudAstrAttemptTimeout.Value;

			TangraConfig.Settings.Astrometry.DefaultMagOutputBand = (TangraConfig.MagOutputBand)cbxMagBand.SelectedIndex;
			TangraConfig.Settings.Photometry.MaxResidualStellarMags = (double)nudMagResidual.Value;
			TangraConfig.Settings.Astrometry.AssumedTargetVRColour = (double)nudTargetVRColour.Value;
		}
	}
}