using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Astrometry.Recognition;
using Tangra.Model.Config;
using Tangra.Model.Helpers;
using Tangra.StarCatalogues.GaiaOnline;
using Tangra.VideoTools;

namespace Tangra.Config.SettingPannels
{
	public partial class ucStarCatalogues : SettingsPannel
	{
		private ICatalogValidator m_CatalogValidator;
		private bool m_ShowCatalogRequiredHint;

		public ucStarCatalogues(bool showCatalogRequiredHint)
		{
			InitializeComponent();

			m_ShowCatalogRequiredHint = showCatalogRequiredHint;
		}

		public override void LoadSettings()
		{
			m_CatalogValidator = new StarCatalogueFacade(TangraConfig.Settings.StarCatalogue);

			cbxCatalogue.SelectedIndex = (int)TangraConfig.Settings.StarCatalogue.Catalog - 1;
			tbxCatalogueLocation.Text = TangraConfig.Settings.StarCatalogue.CatalogLocation;
            cbxUseGaia.Checked = TangraConfig.Settings.StarCatalogue.UseGaiaDR2;
            tbxGaiaAPIToken.Text = TangraConfig.Settings.StarCatalogue.GaiaAPIToken;
			if (Guid.Empty != TangraConfig.Settings.StarCatalogue.CatalogMagnitudeBandId)
			{
				CatalogMagnitudeBand bnd = cbxCatalogPhotometryBand.Items.Cast<CatalogMagnitudeBand>().FirstOrDefault(mb => mb.Id == TangraConfig.Settings.StarCatalogue.CatalogMagnitudeBandId);
				if (bnd != null)
					cbxCatalogPhotometryBand.SelectedItem = bnd;
			}
		}

		public override bool ValidateSettings()
		{
			if (cbxCatalogue.SelectedIndex != -1)
			{
				TangraConfig.StarCatalog chosenCatalogue = (TangraConfig.StarCatalog)(cbxCatalogue.SelectedIndex + 1);

				if (!Directory.Exists(tbxCatalogueLocation.Text))
				{
					tbxCatalogueLocation.Focus();
					MessageBox.Show(
						this, 
						"Please select a valid directory", 
						"Tangra", 
						MessageBoxButtons.OK,
						MessageBoxIcon.Error);
					return false;
				}

				string path = tbxCatalogueLocation.Text;
				if (!m_CatalogValidator.IsValidCatalogLocation(chosenCatalogue, ref path))
				{
					tbxCatalogueLocation.Focus();

                    MessageBox.Show(
                        this,
                        string.Format("Selected folder does not contain the {0} catalogue", chosenCatalogue),
                        "Tangra",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    return false;
				}

				tbxCatalogueLocation.Text = path;

			    if (cbxUseGaia.Checked)
			    {
                    var token = tbxGaiaAPIToken.Text;
			        if (!GaiaTapCatalogue.IsValidApiToken(ref token))
			        {
                        var rv = MessageBox.Show(
                            this,
                            string.Format("'{0}' does look like a valid API Token. Do you want to use it anyway?", tbxGaiaAPIToken.Text),
                            "Tangra",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning,
                            MessageBoxDefaultButton.Button2);

			            if (rv != DialogResult.Yes)
			            {
                            tbxGaiaAPIToken.Focus();
			                return false;
			            }
			        }
			    }
			}

			return true;
		}

		public override void SaveSettings()
		{
			if (cbxCatalogue.SelectedIndex != -1)
			{
				TangraConfig.Settings.StarCatalogue.Catalog = (TangraConfig.StarCatalog)(cbxCatalogue.SelectedIndex + 1);
				TangraConfig.Settings.StarCatalogue.CatalogLocation = tbxCatalogueLocation.Text;
				TangraConfig.Settings.StarCatalogue.CatalogMagnitudeBandId = ((CatalogMagnitudeBand)cbxCatalogPhotometryBand.SelectedItem).Id;
			    TangraConfig.Settings.StarCatalogue.UseGaiaDR2 = cbxUseGaia.Checked;
			    TangraConfig.Settings.StarCatalogue.GaiaAPIToken = tbxGaiaAPIToken.Text;
			}
		}

		private void ProcessShowCatalogFlag()
		{
			if (m_ShowCatalogRequiredHint)
			{
				if (TangraConfig.Settings.StarCatalogue.Catalog == TangraConfig.StarCatalog.NotSpecified)
				{
					MessageBox.Show("Star catalog is required for calibration and astrometry.", "Star Catalog Required", MessageBoxButtons.OK, MessageBoxIcon.Error);
					cbxCatalogue.Focus();
				}
				else if (!m_CatalogValidator.VerifyCurrentCatalogue(TangraConfig.Settings.StarCatalogue.Catalog, ref TangraConfig.Settings.StarCatalogue.CatalogLocation))
				{
					MessageBox.Show("The current star catalog location is invalid.", "Star Catalog Invalid", MessageBoxButtons.OK, MessageBoxIcon.Error);
					tbxCatalogueLocation.Focus();
					tbxCatalogueLocation.SelectAll();
				}
			}
		}

		private void ucStarCatalogues_Load(object sender, EventArgs e)
		{
			ProcessShowCatalogFlag();
		}

		private void btnBrowseLocation_Click(object sender, EventArgs e)
		{
			if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
				tbxCatalogueLocation.Text = folderBrowserDialog.SelectedPath;
		}

		private void btnMPCHeader_Click(object sender, EventArgs e)
		{
			frmMPCObserver frm = new frmMPCObserver(frmMPCObserver.MPCHeaderSettingsMode.TangraSettings);
			frm.ShowDialog(this);
		}

		private void cbxCatalogue_SelectedIndexChanged(object sender, EventArgs e)
		{
		    UpdateCatalogueMagniudeBands();
		}

        private void OpenLinkLabelLink(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShellHelper.OpenUrl((sender as LinkLabel).Text);
        }

        private void cbxUseGaia_CheckedChanged(object sender, EventArgs e)
        {
            pnlGaiaAIPNotes.Visible = cbxUseGaia.Checked;
            UpdateCatalogueMagniudeBands();
        }

	    private void UpdateCatalogueMagniudeBands()
	    {
            // For each supported catalog we provide custom options for magnitude band to use for photometry
            TangraConfig.StarCatalog catalog = (TangraConfig.StarCatalog)(cbxCatalogue.SelectedIndex + 1);
            bool useGaia = cbxUseGaia.Checked;
            switch (catalog)
            {
                case TangraConfig.StarCatalog.UCAC3:
                    cbxCatalogPhotometryBand.Items.Clear();
                    cbxCatalogPhotometryBand.Items.AddRange(m_CatalogValidator.MagnitudeBandsForCatalog(catalog, useGaia));
                    break;

                case TangraConfig.StarCatalog.UCAC2:
                    cbxCatalogPhotometryBand.Items.Clear();
                    cbxCatalogPhotometryBand.Items.AddRange(m_CatalogValidator.MagnitudeBandsForCatalog(catalog, useGaia));
                    break;

                case TangraConfig.StarCatalog.NOMAD:
                    cbxCatalogPhotometryBand.Items.Clear();
                    cbxCatalogPhotometryBand.Items.AddRange(m_CatalogValidator.MagnitudeBandsForCatalog(catalog, useGaia));
                    break;

                case TangraConfig.StarCatalog.PPMXL:
                    cbxCatalogPhotometryBand.Items.Clear();
                    cbxCatalogPhotometryBand.Items.AddRange(m_CatalogValidator.MagnitudeBandsForCatalog(catalog, useGaia));
                    break;

                case TangraConfig.StarCatalog.UCAC4:
                    cbxCatalogPhotometryBand.Items.Clear();
                    cbxCatalogPhotometryBand.Items.AddRange(m_CatalogValidator.MagnitudeBandsForCatalog(catalog, useGaia));
                    break;
            }

            if (cbxCatalogPhotometryBand.Items.Count > 0)
                cbxCatalogPhotometryBand.SelectedIndex = 0;
	    }
	}
}
