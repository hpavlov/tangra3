using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Astrometry.Recognition;
using Tangra.Model.Config;
using Tangra.Model.Helpers;

namespace Tangra.Config.SettingPannels
{
	public partial class ucLocation : SettingsPannel
	{
		private ICatalogValidator m_CatalogValidator;
        public ucLocation()
		{
			InitializeComponent();
		}

		public override void LoadSettings()
		{
			m_CatalogValidator = new StarCatalogueFacade(TangraConfig.Settings.StarCatalogue);

			if (double.IsNaN(TangraConfig.Settings.Generic.Longitude))
			{
				cbxLongitude.SelectedIndex = -1;
				tbxLongitude.Text = string.Empty;
			}
			else
			{
				double longNoSign = Math.Abs(TangraConfig.Settings.Generic.Longitude);
				tbxLongitude.Text = AstroConvert.ToStringValue(longNoSign, "DD MM SS");
				if (TangraConfig.Settings.Generic.Longitude < 0)
					cbxLongitude.SelectedIndex = 1;
				else
					cbxLongitude.SelectedIndex = 0;
			}

			rbMPCCode.Checked = TangraConfig.Settings.Astrometry.UseMPCCode;
			tbxMPCCode.Text = TangraConfig.Settings.Astrometry.MPCObservatoryCode;

			if (double.IsNaN(TangraConfig.Settings.Generic.Latitude))
			{
				cbxLatitude.SelectedIndex = -1;
				tbxLatitude.Text = string.Empty;
			}
			else
			{
				double latNoSign = Math.Abs(TangraConfig.Settings.Generic.Latitude);
				tbxLatitude.Text = AstroConvert.ToStringValue(latNoSign, "DD MM SS");
				if (TangraConfig.Settings.Generic.Latitude < 0)
					cbxLatitude.SelectedIndex = 1;
				else
					cbxLatitude.SelectedIndex = 0;
			}
			
		}

		public override void SaveSettings()
		{
			if (rbMPCCode.Checked)
			{
				TangraConfig.Settings.Astrometry.UseMPCCode = true;
				TangraConfig.Settings.Astrometry.MPCObservatoryCode = tbxMPCCode.Text;
			}
			else
			{
				TangraConfig.Settings.Astrometry.UseMPCCode = false;
				if (!string.IsNullOrEmpty(tbxLongitude.Text))
				{
					int sign = cbxLongitude.SelectedIndex == 1 ? -1 : 1;
					TangraConfig.Settings.Generic.Longitude = sign * AstroConvert.ToDeclination(tbxLongitude.Text);
				}

				if (!string.IsNullOrEmpty(tbxLatitude.Text))
				{
					int sign = cbxLatitude.SelectedIndex == 1 ? -1 : 1;
					TangraConfig.Settings.Generic.Latitude = sign * AstroConvert.ToDeclination(tbxLatitude.Text);
				}
			}
		}

		private void btnMPCHeader_Click(object sender, EventArgs e)
		{
			frmMPCObserver frm = new frmMPCObserver(frmMPCObserver.MPCHeaderSettingsMode.TangraSettings);
			frm.ShowDialog(this);
		}

		private void rbCoordinates_CheckedChanged(object sender, EventArgs e)
		{
			pnlCoordinates.Enabled = rbCoordinates.Checked;
			tbxMPCCode.Enabled = rbMPCCode.Checked;
		}
	}
}
