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

		private TangraConfig.SpectroscopySettings.PersistedConfiguration m_SelectedConfiguration;

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
			TangraConfig.Settings.LastUsed.SpectroscopyWavelengthConfigurationname = m_SelectedConfiguration.Name;
			TangraConfig.Settings.Save();	

			DialogResult = DialogResult.OK;
			Close();
		}

		private void cbxSavedConfigurations_SelectedIndexChanged(object sender, EventArgs e)
		{
			var selectedConfig = cbxSavedConfigurations.SelectedItem as TangraConfig.SpectroscopySettings.PersistedConfiguration;
			if (selectedConfig != null)
				m_SelectedConfiguration = selectedConfig;
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
	}
}