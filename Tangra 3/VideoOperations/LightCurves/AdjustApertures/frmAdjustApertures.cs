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

		public frmAdjustApertures()
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

			if (TangraConfig.Settings.Photometry.SignalApertureUnitDefault == TangraConfig.SignalApertureUnit.FWHM)
			{
				nudCommonAperture.SetNUDValue(TangraConfig.Settings.Photometry.DefaultSignalAperture);
				cbxCommonUnit.SelectedIndex = 1;
				nudFWHMAperture.SetNUDValue(TangraConfig.Settings.Photometry.DefaultSignalAperture);
			}
			else if (TangraConfig.Settings.Photometry.SignalApertureUnitDefault == TangraConfig.SignalApertureUnit.Pixels)
			{
				nudCommonAperture.SetNUDValue(1.2);
				cbxCommonUnit.SelectedIndex = 0;
				nudFWHMAperture.SetNUDValue(1.2);
			}

			if (!rbSameApertures.Checked)
			{
				// Set the default settings for "same size apertures" (use 3 * FWHM as default)
				nudCommonAperture.SetNUDValue(3);
				cbxCommonUnit.SelectedIndex = 1;
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

			UpdateUI();
		}


		public void UpdateUI()
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

			pnlBoundToFWHM.Enabled = rbBoundToFWHM.Checked;
			pnlSameApertures.Enabled = rbSameApertures.Checked;
			pnlCusomApertures.Enabled = rbCusomApertures.Checked;
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

				float apX = 9 + target.ApertureMatrixX0;
				float apY = 9 + target.ApertureMatrixY0;
				float apRectX = 2 * (apX - target.ApertureInPixels);
				float apRectY = 2 * (apY - target.ApertureInPixels);
				float apertureSize = 4 * Model.Apertures[targetId];
				g.DrawEllipse(Controller.DisplaySettings.TargetPens[targetId], apRectX, apRectY, apertureSize, apertureSize);

				g.Save();
			}

			pic.Update();
		}

		private void cbxCommonUnit_SelectedIndexChanged(object sender, EventArgs e)
		{
			cbxCommonFWHMType.Visible = cbxCommonUnit.SelectedIndex == 1;
			if (cbxCommonFWHMType.SelectedIndex == -1 &&
			    cbxCommonUnit.SelectedIndex == 1)
			{
				cbxCommonFWHMType.SelectedIndex = 0;
			}
		}

		private void OnSelectedAdjustmentModeChanged(object sender, EventArgs e)
		{
			if (rbBoundToFWHM.Checked)
			{
				for (int i = 0; i < Model.Apertures.Length; i++)
				{
					Model.Apertures[i] = Model.FWHMs[i] * (float)nudFWHMAperture.Value;
				}				
			}
			else if (rbSameApertures.Checked)
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
					else if (cbxCommonFWHMType.SelectedIndex == 0)
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
}
