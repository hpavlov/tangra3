/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Tangra.Helpers;
using Tangra.Model.Config;
using Tangra.VideoOperations.LightCurves.Tracking;

namespace Tangra.VideoOperations.LightCurves.AdjustApertures
{
	public partial class frmAdjustApertures : Form
	{
		internal AdjustAperturesController Controller;
		internal AdjustAperturesViewModel Model;

		private Brush[] m_GragBrushes;
	    private bool m_SettingControlValues = false;

		public frmAdjustApertures()
		{
			InitializeComponent();
		}

		internal frmAdjustApertures(LCStateMachine stateMachine)
		{
			InitializeComponent();

			picTarget1Pixels.Image = new Bitmap(picTarget1Pixels.Width, picTarget1Pixels.Height);
			picTarget2Pixels.Image = new Bitmap(picTarget2Pixels.Width, picTarget2Pixels.Height);
			picTarget3Pixels.Image = new Bitmap(picTarget3Pixels.Width, picTarget3Pixels.Height);
			picTarget4Pixels.Image = new Bitmap(picTarget4Pixels.Width, picTarget4Pixels.Height);

			m_GragBrushes = new Brush[256];
			for (int i = 0; i < 256; i++)
			{
				m_GragBrushes[i] = new SolidBrush(Color.FromArgb(i, i, i));
			}

            m_SettingControlValues = true;
            try
            {
				nudCommonAperture.SetNUDValue(stateMachine.MeasuringApertures[0]);
				cbxCommonUnit.SelectedIndex = 0;

                if (!rbSameApertures.Checked)
                {
                    // Set the default settings for "same size apertures" (use a diameter of 4 * FWHM as default (radius of 2 * FWHM))
                    nudCommonAperture.SetNUDValue(1.5);
                    cbxCommonUnit.SelectedIndex = 1;
                }
            }
            finally
            {
                m_SettingControlValues = false;
            }

		    miRecent.DropDownItems.Clear();

            foreach (TangraConfig.SameSizeApertureConfig config in TangraConfig.Settings.LastUsed.SameSizeApertures)
            {
                var subMenu = new ToolStripMenuItem(config.ToString());
                subMenu.Tag = config;
                subMenu.Click += subMenu_Click;
                miRecent.DropDownItems.Add(subMenu);
            }

		    miRecent.Visible = miRecent.DropDownItems.Count > 0;

		    RecalcApertureSizesAndUpdateUI();
		}

        void subMenu_Click(object sender, EventArgs e)
        {
            var subMenu = sender as ToolStripMenuItem;
            if (subMenu != null)
            {
                var config = (TangraConfig.SameSizeApertureConfig) subMenu.Tag;
                if (config != null)
                {
                    m_SettingControlValues = true;
                    try
                    {
                        nudCommonAperture.SetNUDValue(config.Value);
                        cbxCommonUnit.SelectedIndex = config.ValueMode;
                        cbxCommonFWHMType.SelectedIndex = config.FwhmMode;
                    }
                    finally
                    {
                        m_SettingControlValues = false;
                    }

                    UpdateUI();
                }
            }
        }

		private void frmAdjustApertures_Shown(object sender, EventArgs e)
		{
			if (Model.MeasuringStars.Length > 0)
			{
				nudAperture1.Visible = true;
				pb1.Visible = true;				
				pb1.BackColor = Controller.DisplaySettings.Target1Color;
				picTarget1Pixels.Visible = true;
			}

			if (Model.MeasuringStars.Length > 1)
			{
				nudAperture2.Visible = true;
				pb2.Visible = true;
				pb2.BackColor = Controller.DisplaySettings.Target2Color;
				picTarget2Pixels.Visible = true;
			}

			if (Model.MeasuringStars.Length > 2)
			{
				nudAperture3.Visible = true;
				pb3.Visible = true;
				pb3.BackColor = Controller.DisplaySettings.Target3Color;
				picTarget3Pixels.Visible = true;
			}

			if (Model.MeasuringStars.Length > 3)
			{
				nudAperture4.Visible = true;
				pb4.Visible = true;
				pb4.BackColor = Controller.DisplaySettings.Target4Color;
				picTarget4Pixels.Visible = true;
			}

            RecalcApertureSizesAndUpdateUI();
		}


		public void UpdateUI()
		{
            m_SettingControlValues = true;
		    try
		    {
                if (Model.MeasuringStars.Length > 0)
                {
                    nudAperture1.SetNUDValue(Model.Apertures[0]);
                    PlotPixels(picTarget1Pixels, 0);
                }

                if (Model.MeasuringStars.Length > 1)
                {
                    nudAperture2.SetNUDValue(Model.Apertures[1]);
                    PlotPixels(picTarget2Pixels, 1);
                }

                if (Model.MeasuringStars.Length > 2)
                {
                    nudAperture3.SetNUDValue(Model.Apertures[2]);
                    PlotPixels(picTarget3Pixels, 2);
                }

                if (Model.MeasuringStars.Length > 3)
                {
                    nudAperture4.SetNUDValue(Model.Apertures[3]);
                    PlotPixels(picTarget4Pixels, 3);
                }

                pnlSameApertures.Enabled = rbSameApertures.Checked;
                pnlCusomApertures.Enabled = rbCusomApertures.Checked;
		    }
		    finally
		    {
                m_SettingControlValues = false;
		    }
		}

		private void PlotPixels(PictureBox pic, int targetId)
		{
			TrackedObjectConfig target = Model.MeasuringStars[targetId];

			uint[,] pixels = Controller.GetPixels(target.OriginalFieldCenterX, target.OriginalFieldCenterY);

			using (Graphics g = Graphics.FromImage(pic.Image))
			{
				for (int x = 0; x < 35; x++)
				{
					for (int y = 0; y < 35; y++)
					{
						byte pixel = (byte)Math.Max(0, Math.Min(255, pixels[x, y]));
						g.FillRectangle(m_GragBrushes[pixel], 2*x, 2*y, 2, 2);
					}
				}

			    float apertureInPixels = Model.Apertures[targetId];
				float apX = 9 + target.ApertureMatrixX0;
				float apY = 9 + target.ApertureMatrixY0;
                float apRectX = 2 * (apX - apertureInPixels);
                float apRectY = 2 * (apY - apertureInPixels);
                float apertureSize = 4 * apertureInPixels;
				g.DrawEllipse(Controller.DisplaySettings.TargetPens[targetId], apRectX, apRectY, apertureSize, apertureSize);

                float innerRadius = apertureInPixels * TangraConfig.Settings.Photometry.AnnulusInnerRadius;
                g.DrawEllipse(
					Controller.DisplaySettings.TargetBackgroundPens[targetId],
                    2 * (apX - innerRadius),
                    2 * (apY - innerRadius),
                    4 * innerRadius, 4 * innerRadius);

                float outerRadius = (float)Math.Sqrt(TangraConfig.Settings.Photometry.AnnulusMinPixels / Math.PI + innerRadius * innerRadius);
                g.DrawEllipse(
					Controller.DisplaySettings.TargetBackgroundPens[targetId],
                    2 * (apX - outerRadius),
                    2 * (apY - outerRadius),
                    4 * outerRadius, 4 * outerRadius);

				g.Save();
			}

		    Controller.ApplyDisplayModeAdjustments((Bitmap)pic.Image);

			pic.Invalidate();
		}

		private void cbxCommonUnit_SelectedIndexChanged(object sender, EventArgs e)
		{
            m_SettingControlValues = true;
		    try
		    {
                cbxCommonFWHMType.Visible = cbxCommonUnit.SelectedIndex == 1;
                if (cbxCommonFWHMType.SelectedIndex == -1 &&
                    cbxCommonUnit.SelectedIndex == 1)
                {
                    cbxCommonFWHMType.SelectedIndex = 0;
                }
		    }
		    finally
		    {
                m_SettingControlValues = false;
		    }

		    RecalcApertureSizesAndUpdateUI();
		}

        private void RecalcApertureSizesAndUpdateUI()
        {
            if (Model != null && !m_SettingControlValues)
            {
                if (rbSameApertures.Checked)
                {
                    float commonValue = 4.0f;
                    if (cbxCommonUnit.SelectedIndex == 0)
                    {
                        commonValue = (float)nudCommonAperture.Value;
                    }
                    else if (cbxCommonUnit.SelectedIndex == 1)
                    {
                        if (cbxCommonFWHMType.SelectedIndex == 0)
                        {
                            commonValue = (float)nudCommonAperture.Value * Model.FWHMs.Max();
                        }
                        else if (cbxCommonFWHMType.SelectedIndex == 1)
                        {
                            commonValue = (float)nudCommonAperture.Value * Model.FWHMs.Average();
                        }
                    }

                    for (int i = 0; i < Model.Apertures.Length; i++)
                    {
                        Model.Apertures[i] = commonValue;
                    }
                }
                else if (rbCusomApertures.Checked)
                {
                    for (int i = 0; i < Model.Apertures.Length; i++)
                    {
                        Model.Apertures[i] = Model.MeasuringStars[i].ApertureInPixels;
                    }
                }

                UpdateUI();
            }
        }

		private void OnSelectedAdjustmentModeChanged(object sender, EventArgs e)
		{
		    RecalcApertureSizesAndUpdateUI();
		}

        private void OnAperturesChanged(object sender, EventArgs e)
        {
            RecalcApertureSizesAndUpdateUI();
        }

        private void OnCustomAperturesChanged(object sender, EventArgs e)
        {
            if (sender == nudAperture1)
                Model.Apertures[0] = (float)nudAperture1.Value;
            else if (sender == nudAperture2)
                Model.Apertures[1] = (float)nudAperture2.Value;
            else if (sender == nudAperture3)
                Model.Apertures[2] = (float)nudAperture3.Value;
            else if (sender == nudAperture4)
                Model.Apertures[3] = (float)nudAperture4.Value;

            UpdateUI();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (rbSameApertures.Checked)
            {
                var newConfig = new TangraConfig.SameSizeApertureConfig()
                {
                    Value = (float) nudCommonAperture.Value,
                    ValueMode = cbxCommonUnit.SelectedIndex,
                    FwhmMode = cbxCommonFWHMType.SelectedIndex
                };

                int existingDuplicateIndex  = TangraConfig.Settings.LastUsed.SameSizeApertures.FindIndex(x => 
                    Math.Abs(x.Value - newConfig.Value) < 0.1 && 
                    x.ValueMode == newConfig.ValueMode && 
                    x.FwhmMode == newConfig.FwhmMode);

                if (existingDuplicateIndex > -1)
                    TangraConfig.Settings.LastUsed.SameSizeApertures.RemoveAt(existingDuplicateIndex);

                if (TangraConfig.Settings.LastUsed.SameSizeApertures.Count == 3)
                    TangraConfig.Settings.LastUsed.SameSizeApertures.RemoveAt(2);

                TangraConfig.Settings.LastUsed.SameSizeApertures.Insert(0, newConfig);
                TangraConfig.Settings.Save();
            }

            DialogResult = DialogResult.OK;
            Close();
        }
	}
}
