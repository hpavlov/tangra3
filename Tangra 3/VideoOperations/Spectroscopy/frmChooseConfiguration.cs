/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Config;
using Tangra.Model.Context;
using Tangra.Model.Video;

namespace Tangra.VideoOperations.Spectroscopy
{
	public partial class frmChooseConfiguration : Form
	{
		private int m_Width;
		private int m_Height;

		private TangraConfig.PersistedConfiguration m_SelectedConfiguration;

	    public TangraConfig.PersistedConfiguration SelectedConfiguration
	    {
            get { return m_SelectedConfiguration; }
	    }

		public frmChooseConfiguration()
		{
			InitializeComponent();
		}

		public frmChooseConfiguration(int width, int height, int bitPix)
			: this()
		{
			m_Width = width;
			m_Height = height;

			m_SelectedConfiguration = null;
			LoadConfigurations();
		}

		private void LoadConfigurations()
		{
			cbxSavedConfigurations.Items.Clear();
		    pnlCalibrated.Visible = false;
            pnlNotCalibrated.Visible = false;

			var compatibleConfigurations = TangraConfig.Settings.Spectroscopy.PersistedConfigurations.Where(x => x.Width == m_Width && x.Height == m_Height);

			foreach (var config in compatibleConfigurations)
			{
				cbxSavedConfigurations.Items.Add(config);
				if ((m_SelectedConfiguration == null && config.Name == TangraConfig.Settings.LastUsed.SpectroscopyWavelengthConfigurationname) ||
					(m_SelectedConfiguration != null && config.Name == m_SelectedConfiguration.Name))
					cbxSavedConfigurations.SelectedIndex = cbxSavedConfigurations.Items.Count - 1;
			}
		}

		private void btnNewConfig_Click(object sender, EventArgs e)
		{
			var frm = new frmEditWavelengthConfigName(null, m_Width, m_Height);
			if (frm.ShowDialog(this) == DialogResult.OK)
			{
				m_SelectedConfiguration = frm.Config;
				LoadConfigurations();
			}
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
            if (m_SelectedConfiguration == null)
            {
                MessageBox.Show("Please select a configuration or create a new one.", "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

			TangraConfig.Settings.LastUsed.SpectroscopyWavelengthConfigurationname = m_SelectedConfiguration.Name;
			TangraConfig.Settings.Save();	

			DialogResult = DialogResult.OK;
			Close();
		}

		private void cbxSavedConfigurations_SelectedIndexChanged(object sender, EventArgs e)
		{
            pnlCalibrated.Visible = false;
            pnlNotCalibrated.Visible = false;

			var selectedConfig = cbxSavedConfigurations.SelectedItem as TangraConfig.PersistedConfiguration;
		    if (selectedConfig != null)
		    {
		        m_SelectedConfiguration = selectedConfig;
                if (selectedConfig.IsCalibrated)
                {
                    lblDispersion.Text = string.Format("{0} A/pix", selectedConfig.Dispersion.ToString("0.00"));
                    lblRMS.Text = string.Format("{0} pix", selectedConfig.RMS.ToString("0.00"));
                    switch (selectedConfig.Order)
                    {
                        case 2:
                            lblCalibratedCaption.Text = "2-nd Order Polynomial Calibration";
                            break;

                        case 3:
                            lblCalibratedCaption.Text = "3-rd Order Polynomial Calibration";
                            break;

                        default:
                            lblCalibratedCaption.Text = "1-st Order Polynomial Calibration";
                            break;
                    }

                    pnlCalibrated.Visible = true;
                    pnlNotCalibrated.Visible = false;
                    pnlNotCalibrated.SendToBack();
                }
                else
                {
                    pnlNotCalibrated.Visible = true;
                    pnlCalibrated.Visible = false;
                    pnlCalibrated.SendToBack();
                }
		    }
		}

		private void btnEdit_Click(object sender, EventArgs e)
		{
			var frm = new frmEditWavelengthConfigName(m_SelectedConfiguration, m_Width, m_Height);
			if (frm.ShowDialog(this) == DialogResult.OK)
			{
				m_SelectedConfiguration = frm.Config;
				LoadConfigurations();
			}
		}

        private void btnDelConfig_Click(object sender, EventArgs e)
        {
            var selectedConfig = cbxSavedConfigurations.SelectedItem as TangraConfig.PersistedConfiguration;

		    if (selectedConfig != null)
            {
                if (MessageBox.Show(this,
                    string.Format("Are you sure you want to delete '{0}'", selectedConfig.Name),
                    "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    TangraConfig.Settings.Spectroscopy.PersistedConfigurations.RemoveAll(x => x.Name == selectedConfig.Name);
                    TangraConfig.Settings.Save();

                    m_SelectedConfiguration = null;
                    LoadConfigurations();
                }
            }
        }
	}
}