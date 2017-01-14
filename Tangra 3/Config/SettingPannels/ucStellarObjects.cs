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
	public partial class ucStellarObjects : SettingsPannel
	{
        public ucStellarObjects()
		{
			InitializeComponent();
		}

		public override void LoadSettings()
		{
			nudMaxElongation.SetNUDValue((decimal)TangraConfig.Settings.Astrometry.MaximumPSFElongation);
			cbForceStellarObjectRequirements.Checked = TangraConfig.Settings.Astrometry.PyramidRemoveNonStellarObject;
			nudLimitMagDetection.SetNUDValue((decimal)TangraConfig.Settings.Astrometry.LimitReferenceStarDetection);
			nudMinFWHM.SetNUDValue((decimal)TangraConfig.Settings.Astrometry.MinReferenceStarFWHM);
			nudMaxFWHM.SetNUDValue((decimal)TangraConfig.Settings.Astrometry.MaxReferenceStarFWHM);
		}

		public override void SaveSettings()
		{
			TangraConfig.Settings.Astrometry.MaximumPSFElongation = (int)nudMaxElongation.Value;
			TangraConfig.Settings.Astrometry.PyramidRemoveNonStellarObject = cbForceStellarObjectRequirements.Checked;
			TangraConfig.Settings.Astrometry.LimitReferenceStarDetection = (double)nudLimitMagDetection.Value;
			TangraConfig.Settings.Astrometry.MinReferenceStarFWHM = (double)nudMinFWHM.Value;
			TangraConfig.Settings.Astrometry.MaxReferenceStarFWHM = (double)nudMaxFWHM.Value;
		}
	}
}
