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
using Tangra.Controller;
using Tangra.Model.Astro;
using Tangra.Model.Config;

namespace Tangra.VideoTools
{
	public partial class frmDefineDisplayDynamicRange : Form
	{
		private VideoController m_VideoController;

		private Dictionary<uint, float> m_Distribution = new Dictionary<uint, float>();
		private Dictionary<uint, float> m_DisplayData = new Dictionary<uint, float>();

		private uint m_MaxPixelValue = 0;
		private uint m_MaxBuckets = 255;
		private int m_Bpp = 8;

		private float m_BucketFactor = 1;

		public frmDefineDisplayDynamicRange()
		{
			InitializeComponent();
		}

		public frmDefineDisplayDynamicRange(VideoController videoController)
		{
			InitializeComponent();

			m_VideoController = videoController;

			m_MaxPixelValue = m_VideoController.EffectiveMaxPixelValue;
			m_Bpp = m_VideoController.VideoBitPix;

			tbarFrom.Minimum = 0;
			tbarFrom.Maximum = (int)m_MaxPixelValue;
			tbarFrom.Value = (int)Math.Min(m_MaxPixelValue, Math.Max(0, m_VideoController.DynamicFromValue));
			tbarFrom.TickFrequency = (int)Math.Ceiling(tbarFrom.Maximum / 256.0);
			tbarFrom.SmallChange = tbarFrom.TickFrequency;
			tbarFrom.LargeChange = tbarFrom.TickFrequency;
			tbarTo.Minimum = 0;
			tbarTo.Maximum = (int)m_MaxPixelValue;
			tbarTo.Value = (int)Math.Min(m_MaxPixelValue, Math.Max(0, m_VideoController.DynamicToValue));
			tbarTo.TickFrequency = (int)Math.Ceiling(tbarTo.Maximum / 256.0);
			tbarTo.SmallChange = tbarFrom.TickFrequency;
			tbarTo.LargeChange = tbarFrom.TickFrequency;

			AstroImage image = m_VideoController.GetCurrentAstroImage(false);
			if (image != null)
			{
				PopulateDistribution(image.Pixelmap.Pixels);
			}

			PlotData();
			DrawHistogram();
		}

		private void PopulateDistribution(uint[] pixels)
		{
			m_Distribution.Clear();
			m_DisplayData.Clear();

			if (m_Bpp <= 8)
			{
				for (uint i = 0; i <= m_MaxPixelValue; i++)
				{
					m_Distribution.Add(i, 0);
					m_DisplayData.Add(i, 0);
				}

				for (int i = 0; i < pixels.Length; i++)
				{
					m_Distribution[pixels[i]]++;
				}
			}
			else
			{
				for (uint i = 0; i <= m_MaxBuckets; i++)
				{
					m_Distribution.Add(i, 0);
					m_DisplayData.Add(i, 0);
				}

				m_BucketFactor = 1.0f * m_MaxBuckets / m_MaxPixelValue;

				for (int i = 0; i < pixels.Length; i++)
				{
					uint val = pixels[i];
					uint bucket = Math.Max(0, Math.Min(m_MaxBuckets - 1, (uint)Math.Round(val * m_BucketFactor)));

					m_Distribution[bucket]++;
				}
			}
		}

		private void PlotData()
		{
			for (uint i = 0; i <= m_MaxBuckets; i++)
			{
				if (rbScaleLog.Checked)
					m_DisplayData[i] = (float)Math.Log10(m_Distribution[i] + 1);
				else
					m_DisplayData[i] = m_Distribution[i];
			}
		}

		private void DrawHistogram()
		{
			if (picHistogram.Image != null)
				picHistogram.Image.Dispose();

			picHistogram.Image = new Bitmap(picHistogram.Width, picHistogram.Height);

			float maxVal = m_DisplayData.Values.Max();
			int XGAP = 10;
			int YGAP = 10;

			using (Graphics g = Graphics.FromImage(picHistogram.Image))
			{
				float xScale = (picHistogram.Image.Width - 2 * XGAP) * 1.0f / (m_MaxBuckets + 1);
				float yScale = (picHistogram.Image.Height - 2 * YGAP) * 1.0f / maxVal;

				g.FillRectangle(SystemBrushes.ControlDark,
								new Rectangle(0, 0, picHistogram.Image.Width, picHistogram.Image.Height));
				g.DrawRectangle(Pens.Black, XGAP, YGAP, picHistogram.Image.Width - 2 * XGAP + 1, picHistogram.Image.Height - 2 * YGAP);

				foreach (byte key in m_DisplayData.Keys)
				{
					float xFrom = XGAP + key * xScale + 1;
					float xSize = Math.Max(0.5f, xScale);
					//uint bucket = Math.Max(0, Math.Min(m_MaxBuckets - 1, (uint)Math.Round(key * m_BucketFactor)));

					float ySize = m_DisplayData[key] * yScale;
					float yFrom = picHistogram.Image.Height - YGAP - ySize;

					g.FillRectangle(Brushes.LimeGreen, xFrom, yFrom, xSize, ySize);
				}

				g.Save();
			}

			picHistogram.Refresh();
		}

		private void rbScaleLinear_CheckedChanged(object sender, EventArgs e)
		{
			PlotData();
			DrawHistogram();
		}

		private void DynamicRangeChanged(object sender, EventArgs e)
		{
			timer1.Enabled = false;
			timer1.Enabled = true;

			pnlSelectedRange.Visible = tbarFrom.Value < tbarTo.Value;
			if (tbarFrom.Value < tbarTo.Value)
			{
				pnlSelectedRange.Left = 15 + (int)(504 * (1.0 * tbarFrom.Value / tbarFrom.Maximum));
				pnlSelectedRange.Width = (int)(504 * (1.0 * (tbarTo.Value - tbarFrom.Value) / tbarTo.Maximum)) - 1;
			}
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			timer1.Enabled = false;
			m_VideoController.SetDisplayIntensifyMode(DisplayIntensifyMode.Dynamic, tbarFrom.Value, tbarTo.Value);
		}

	}
}
