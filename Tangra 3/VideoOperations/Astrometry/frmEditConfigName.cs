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
using Tangra.Astrometry;
using Tangra.Config;
using Tangra.Helpers;
using Tangra.Model.Config;
using Tangra.Model.Video;

namespace Tangra.VideoOperations.Astrometry
{
	public partial class frmEditConfigName : Form
	{
		private TangraConfig.ScopeRecorderConfiguration m_Config;
		private VideoCamera m_Camera;
		private bool m_New = false;
		private TangraConfig.PersistedPlateConstants m_PltConst;

		private double m_EffectiveCellX;
		private double m_EffectiveCellY;
		private double m_EffectiveFocalLength;
		private bool m_UpdatePlateConstants = false;
		private int m_FrameWidth;
		private int m_FrameHeight;

		internal TangraConfig.ScopeRecorderConfiguration Config
		{
			get { return m_Config; }
		}

		internal TangraConfig.PersistedPlateConstants PltConst
		{
			get { return m_PltConst; }
		}

		internal bool UpdatePlateConstants
		{
			get { return m_UpdatePlateConstants; }
		}

		public frmEditConfigName(
			TangraConfig.ScopeRecorderConfiguration config,
			VideoCamera camera,
			int frameWidth,
			int frameHeight,
			TangraConfig.PersistedPlateConstants pltConst)
		{
			InitializeComponent();

			m_FrameWidth = frameWidth;
			m_FrameHeight = frameHeight;

			if (config == null)
			{
				config = new TangraConfig.ScopeRecorderConfiguration(camera.Model, frameWidth, frameHeight);
				tbxConfigName.Enabled = true;
				m_New = true;
				Height = 260;
				pnlSolvedPlateConf.Visible = false;
				cbxEditConfig.Visible = false;
			    pnlOSDAreaConf.Visible = false;
			    cbxEditOSDArea.Visible = false;
			}
			else
			{
				tbxConfigName.Enabled = false;
                Height = 396;
				pnlSolvedPlateConf.Visible = true;
				cbxEditConfig.Visible = true;
				if (pltConst != null)
				{
					tbxSolvedCellX.Text = pltConst.EffectivePixelWidth.ToString();
					tbxSolvedCellY.Text = pltConst.EffectivePixelHeight.ToString();
					tbxSolvedFocalLength.Text = pltConst.EffectiveFocalLength.ToString();
				}
			}

            cbxAreaType.SelectedIndex = config.IsInclusionArea ? 1 : 0;
            if (config.IsInclusionArea)
                SetOSDDimentions(config.InclusionArea);
            else
                SetOSDDimentions(config.OSDExclusionArea);

			m_Config = config;
			m_Camera = camera;
			tbxConfigName.Text = config.Title;
			m_PltConst = pltConst;
		}

	    private void SetOSDDimentions(Rectangle rect)
	    {
            nudOSDLeft.SetNUDValue(rect.Left);
            nudOSDTop.SetNUDValue(rect.Top);
            nudOSDWidth.SetNUDValue(rect.Width);
            nudOSDHeight.SetNUDValue(rect.Height);        
	    }

	    private Rectangle GetOSDDimentions()
	    {
            return new Rectangle((int)nudOSDLeft.Value, (int)nudOSDTop.Value, (int)nudOSDWidth.Value, (int)nudOSDHeight.Value);
	    }

		private void btnOK_Click(object sender, EventArgs e)
		{
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

				if (TangraConfig.Settings.PlateSolve.ScopeRecorders.Exists((c) => c.Title == tbxConfigName.Text))
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
				if (tbxSolvedCellX.Text.Length != 0 && tbxSolvedCellY.Text.Length != 0 && tbxSolvedFocalLength.Text.Length != 0)
				{
					if (!double.TryParse(tbxSolvedCellX.Text, out m_EffectiveCellX))
					{
						MessageBox.Show(this, "Enter a valid number for Pixel Width", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						tbxSolvedCellX.Focus();
						tbxSolvedCellX.SelectAll();

						return;
					}

					if (!double.TryParse(tbxSolvedCellY.Text, out m_EffectiveCellY))
					{
						MessageBox.Show(this, "Enter a valid number for Pixel Height", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						tbxSolvedCellY.Focus();
						tbxSolvedCellY.SelectAll();

						return;
					}

					if (!double.TryParse(tbxSolvedFocalLength.Text, out m_EffectiveFocalLength))
					{
						MessageBox.Show(this, "Enter a valid number for Focal Length", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						tbxSolvedFocalLength.Focus();
						tbxSolvedFocalLength.SelectAll();

						return;
					}

					if (m_PltConst == null)
					{
						m_PltConst = new TangraConfig.PersistedPlateConstants()
						{
							CameraModel = m_Camera.Model,
							ScopeRecoderConfig = tbxConfigName.Text,
							PlateWidth = m_FrameWidth,
							PlateHeight = m_FrameHeight
						};
					}

					m_PltConst.EffectiveFocalLength = m_EffectiveFocalLength;
					m_PltConst.EffectivePixelWidth = m_EffectiveCellX;
					m_PltConst.EffectivePixelHeight = m_EffectiveCellY;
					m_UpdatePlateConstants = true;
				}
				else if (tbxSolvedCellX.Text.Length != 0 || tbxSolvedCellY.Text.Length != 0 || tbxSolvedFocalLength.Text.Length != 0)
				{
					MessageBox.Show(this, "When manually enterring the effective plate constants all three values must be entered", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

					if (tbxSolvedCellX.Text.Length == 0)
					{
						tbxSolvedCellX.Focus();
						tbxSolvedCellX.SelectAll();
					}
					else if (tbxSolvedCellY.Text.Length == 0)
					{
						tbxSolvedCellY.Focus();
						tbxSolvedCellY.SelectAll();
					}
					else if (tbxSolvedFocalLength.Text.Length == 0)
					{
						tbxSolvedFocalLength.Focus();
						tbxSolvedFocalLength.SelectAll();
					}

					return;
				}
			}

		    if (cbxEditOSDArea.Checked)
		    {
		        m_Config.IsInclusionArea = cbxAreaType.SelectedIndex == 1;
		        if (cbxAreaType.SelectedIndex == 1)
                    m_Config.InclusionArea = GetOSDDimentions();
                else 
                    m_Config.OSDExclusionArea = GetOSDDimentions();
		    }

			m_Config.Title = tbxConfigName.Text;

			if (m_New)
				TangraConfig.Settings.PlateSolve.ScopeRecorders.Add(m_Config);
			else
			{
				// This will be edited elsewhere
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
			pnlSolvedPlateConf.Enabled = cbxEditConfig.Checked;
		}

        private void cbxEditOSDArea_CheckedChanged(object sender, EventArgs e)
        {
            pnlOSDAreaConf.Enabled = cbxEditOSDArea.Checked;
        }
	}
}
