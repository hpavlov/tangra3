/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using nom.tam.util;
using Tangra.Astrometry;
using Tangra.Config;
using Tangra.Helpers;
using Tangra.Model.Config;
using Tangra.Model.Video;

namespace Tangra.VideoOperations.Spectroscopy
{
	public partial class frmEditWavelengthConfigName : Form
	{
		private TangraConfig.SpectroscopySettings.PersistedConfiguration m_Config;
		private bool m_New = false;
		private int m_FrameWidth;
		private int m_FrameHeight;
		private string m_OriginalName;
		private bool m_UpdateCoefficients;

		internal TangraConfig.SpectroscopySettings.PersistedConfiguration Config
		{
			get { return m_Config; }
		}

		public frmEditWavelengthConfigName(
			TangraConfig.SpectroscopySettings.PersistedConfiguration config,
			int frameWidth,
			int frameHeight)
		{
			InitializeComponent();

			m_FrameWidth = frameWidth;
			m_FrameHeight = frameHeight;

			if (config == null)
			{
				config = new TangraConfig.SpectroscopySettings.PersistedConfiguration { Width = frameWidth, Height = frameHeight };
				tbxConfigName.Enabled = true;
				m_New = true;
				Height = 260;
				pnlSolvedWavelengthConf.Visible = false;
				cbxEditConfig.Visible = false;
			}
			else
			{
				tbxConfigName.Enabled = false;
				m_OriginalName = config.Name;
				Height = 324;
				pnlSolvedWavelengthConf.Visible = true;
				cbxEditConfig.Visible = true;

				tbxSolvedA.Text = config.A.ToString();
				tbxSolvedB.Text = config.B.ToString();
				if (config.Order > 1)
				{					
					tbxSolvedC.Text = config.C.ToString();
					if (config.Order > 2)
						tbxSolvedD.Text = config.D.ToString();
				}
				nudConfigOrder.SetNUDValue(config.Order);
			}

			m_Config = config;
			tbxConfigName.Text = config.Name;
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			m_UpdateCoefficients = false;

			if (m_New)
			{
				if (tbxConfigName.Text.Length == 0)
				{
					MessageBox.Show(this, "Specify config name", "Validation Error", MessageBoxButtons.OK,
					                MessageBoxIcon.Error);

					tbxConfigName.Focus();
					tbxConfigName.SelectAll();

					return;
				}

				if (TangraConfig.Settings.Spectroscopy.PersistedConfigurations.Exists((c) => c.Name == tbxConfigName.Text))
				{
					MessageBox.Show(this, "This name already exists. Specify a different config name",
					                "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					tbxConfigName.Focus();
					tbxConfigName.SelectAll();

					return;
				}
			}
			else if (cbxEditConfig.Checked)
			{
				if (tbxSolvedA.Text.Length != 0 && tbxSolvedA.Text.Length != 0 && tbxSolvedA.Text.Length != 0 &&
				    tbxSolvedD.Text.Length != 0)
				{
					float a, b, c, d;
					if (!float.TryParse(tbxSolvedA.Text, out a))
					{
						MessageBox.Show(this, "Enter a valid number for the A coefficient", "Validation Error", MessageBoxButtons.OK,
							MessageBoxIcon.Error);
						tbxSolvedA.Focus();
						tbxSolvedA.SelectAll();

						return;
					}

					if (!float.TryParse(tbxSolvedB.Text, out b))
					{
						MessageBox.Show(this, "Enter a valid number for the B coefficient", "Validation Error", MessageBoxButtons.OK,
							MessageBoxIcon.Error);
						tbxSolvedB.Focus();
						tbxSolvedB.SelectAll();

						return;
					}

					if (!float.TryParse(tbxSolvedC.Text, out c))
					{
						MessageBox.Show(this, "Enter a valid number for the C coefficient", "Validation Error", MessageBoxButtons.OK,
							MessageBoxIcon.Error);
						tbxSolvedC.Focus();
						tbxSolvedC.SelectAll();

						return;
					}

					if (!float.TryParse(tbxSolvedD.Text, out d))
					{
						MessageBox.Show(this, "Enter a valid number for the D coefficient", "Validation Error", MessageBoxButtons.OK,
							MessageBoxIcon.Error);
						tbxSolvedD.Focus();
						tbxSolvedD.SelectAll();

						return;
					}

					m_Config.A = a;
					m_Config.B = b;
					m_Config.C = c;
					m_Config.D = d;
					m_Config.Order = (int) nudConfigOrder.Value;
					m_UpdateCoefficients = true;
				}
				else
					return;
			}

			m_Config.Name = tbxConfigName.Text;

			if (m_New)
				TangraConfig.Settings.Spectroscopy.PersistedConfigurations.Add(m_Config);
			else
			{
				// This will be edited elsewhere
				var config = TangraConfig.Settings.Spectroscopy.PersistedConfigurations.SingleOrDefault(x => x.Name == m_OriginalName);
				if (config != null)
				{
					config.Name = m_Config.Name;

					if (m_UpdateCoefficients)
					{
						config.Order = m_Config.Order;
						config.A = m_Config.A;
						config.B = m_Config.B;
						config.C = m_Config.C;
						config.D = m_Config.D;
					}
				}

			}

			TangraConfig.Settings.Save();

			DialogResult = DialogResult.OK;
			Close();
		}

		private void tbxConfigName_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\r')
			{
				btnOK_Click(this, EventArgs.Empty);
			}
		}

		private void cbxEditConfig_CheckedChanged(object sender, EventArgs e)
		{
			pnlSolvedWavelengthConf.Enabled = cbxEditConfig.Checked;
			tbxConfigName.Enabled = cbxEditConfig.Checked;
		}
	}
}
