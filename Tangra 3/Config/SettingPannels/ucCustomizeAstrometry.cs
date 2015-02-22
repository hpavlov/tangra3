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
	public partial class ucCustomizeAstrometry : SettingsPannel
	{
		public ucCustomizeAstrometry()
		{
			InitializeComponent();
		}

		public override void LoadSettings()
		{
			ucColorPickerReferenceStar.SelectedColor = TangraConfig.Settings.Astrometry.Colors.ReferenceStar;
			ucColorPickerRejectedReferenceStar.SelectedColor = TangraConfig.Settings.Astrometry.Colors.RejectedReferenceStar;
			ucColorPickerUndetectedStar.SelectedColor = TangraConfig.Settings.Astrometry.Colors.UndetectedReferenceStar;
			ucColorPickerCatalogStar.SelectedColor = TangraConfig.Settings.Astrometry.Colors.CatalogueStar;
			ucColorPickerUserObject.SelectedColor = TangraConfig.Settings.Astrometry.Colors.UserObject;
		}

		public override void SaveSettings()
		{
			TangraConfig.Settings.Astrometry.Colors.ReferenceStar = ucColorPickerReferenceStar.SelectedColor;
			TangraConfig.Settings.Astrometry.Colors.RejectedReferenceStar = ucColorPickerRejectedReferenceStar.SelectedColor;
			TangraConfig.Settings.Astrometry.Colors.UndetectedReferenceStar = ucColorPickerUndetectedStar.SelectedColor;
			TangraConfig.Settings.Astrometry.Colors.CatalogueStar = ucColorPickerCatalogStar.SelectedColor;
			TangraConfig.Settings.Astrometry.Colors.UserObject = ucColorPickerUserObject.SelectedColor;
		}
	}
}
