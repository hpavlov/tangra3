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

				if (chosenCatalogue != TangraConfig.StarCatalog.GaiaDR2 && !Directory.Exists(tbxCatalogueLocation.Text))
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

				    if (chosenCatalogue == TangraConfig.StarCatalog.GaiaDR2)
				    {
                        var rv = MessageBox.Show(
                            this,
                            string.Format("'{0}' does look like a valid API Token. Do you want to use it anyway?", path),
                            "Tangra",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning,
                            MessageBoxDefaultButton.Button2);

				        return rv == DialogResult.Yes;
				    }
				    else
				    {
                        MessageBox.Show(
                            this,
                            string.Format("Selected folder does not contain the {0} catalogue", chosenCatalogue),
                            "Tangra",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);

                        return false;

				    }
				}

				tbxCatalogueLocation.Text = path;
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
			// For each supported catalog we provide custom options for magnitude band to use for photometry
			TangraConfig.StarCatalog catalog = (TangraConfig.StarCatalog)(cbxCatalogue.SelectedIndex + 1);
			switch (catalog)
			{
				case TangraConfig.StarCatalog.UCAC3:
					cbxCatalogPhotometryBand.Items.Clear();
					cbxCatalogPhotometryBand.Items.AddRange(m_CatalogValidator.MagnitudeBandsForCatalog(catalog));
					break;

				case TangraConfig.StarCatalog.UCAC2:
					cbxCatalogPhotometryBand.Items.Clear();
					cbxCatalogPhotometryBand.Items.AddRange(m_CatalogValidator.MagnitudeBandsForCatalog(catalog));
					break;

				case TangraConfig.StarCatalog.NOMAD:
					cbxCatalogPhotometryBand.Items.Clear();
					cbxCatalogPhotometryBand.Items.AddRange(m_CatalogValidator.MagnitudeBandsForCatalog(catalog));
					break;

				case TangraConfig.StarCatalog.PPMXL:
					cbxCatalogPhotometryBand.Items.Clear();
					cbxCatalogPhotometryBand.Items.AddRange(m_CatalogValidator.MagnitudeBandsForCatalog(catalog));
					break;

				case TangraConfig.StarCatalog.UCAC4:
					cbxCatalogPhotometryBand.Items.Clear();
					cbxCatalogPhotometryBand.Items.AddRange(m_CatalogValidator.MagnitudeBandsForCatalog(catalog));
					break;

                case TangraConfig.StarCatalog.GaiaDR2:
                    cbxCatalogPhotometryBand.Items.Clear();
                    cbxCatalogPhotometryBand.Items.AddRange(m_CatalogValidator.MagnitudeBandsForCatalog(catalog));
                    break;
			}

			if (cbxCatalogPhotometryBand.Items.Count > 0)
				cbxCatalogPhotometryBand.SelectedIndex = 0;

		    if (catalog == TangraConfig.StarCatalog.GaiaDR2)
		    {
                btnBrowseLocation.Visible = false;
                lblCatalogPath.Text = "API Token";
		        pnlGaiaAIPNotes.Visible = true;
		    }
		    else
		    {
                lblCatalogPath.Text = "Path";
		        btnBrowseLocation.Visible = true;
                pnlGaiaAIPNotes.Visible = false;
		    }
		}

        private void OpenLinkLabelLink(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShellHelper.OpenUrl((sender as LinkLabel).Text);
        }
	}
}
