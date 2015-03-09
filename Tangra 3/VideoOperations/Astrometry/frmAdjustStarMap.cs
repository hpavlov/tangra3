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
using Tangra.Astrometry.Recognition;
using Tangra.Config;
using Tangra.Controller;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Model.Context;
using Tangra.Model.Image;
using Tangra.Model.VideoOperations;
using Tangra.VideoOperations.Astrometry.Engine;

namespace Tangra.VideoOperations.Astrometry
{
	public partial class frmAdjustStarMap : Form
	{
		internal StarMap StarMap;
		internal StarMapInternalConfig StarMapConfig;
		internal Bitmap RawBitmap;

		private Pen m_Pen;

		private IVideoController m_VideoController;

		public frmAdjustStarMap()
		{
			InitializeComponent();
		}


		public frmAdjustStarMap(IVideoController videoController)
		{
			InitializeComponent();

			m_VideoController = videoController;

			m_Pen = new Pen(TangraConfig.Settings.Astrometry.Colors.CatalogueStar);

			Width = Math.Max(616, TangraContext.Current.FrameWidth + 19);
			Height = Math.Max(496, TangraContext.Current.FrameHeight + 96);

			hsrcFrames.Minimum = m_VideoController.VideoFirstFrame;
			hsrcFrames.Maximum = m_VideoController.VideoLastFrame;
			hsrcFrames.Value = hsrcFrames.Maximum;
		}

		private void ExtractStarMap()
		{
			this.Enabled = false;
			try
			{
				StarMapConfig = StarMapInternalConfig.Default;
				StarMapConfig.StarMapperTolerance = m_Depth;
				int optimumStars = -1;
				if (m_Auto)
					optimumStars = (int)TangraConfig.Settings.Astrometry.PyramidOptimumStarsToMatch;

				if (RawBitmap != null) RawBitmap.Dispose();

				AstroImage astroImage = m_VideoController.GetCurrentAstroImage(false);

				StarMap = new StarMap(
					TangraConfig.Settings.Astrometry.PyramidRemoveNonStellarObject,
					TangraConfig.Settings.Astrometry.MinReferenceStarFWHM,
					TangraConfig.Settings.Astrometry.MaxReferenceStarFWHM,
					TangraConfig.Settings.Astrometry.MaximumPSFElongation,
					TangraConfig.Settings.Astrometry.LimitReferenceStarDetection);

				StarMap.FindBestMap(
					StarMapConfig, 
					astroImage, 
					AstrometryContext.Current.OSDRectToExclude, 
					AstrometryContext.Current.RectToInclude,
					AstrometryContext.Current.LimitByInclusion, 
					optimumStars);

				// NOTE: This will crash 
				imgFrame.Image = RawBitmap;
				using (Graphics g = Graphics.FromImage(imgFrame.Image))
				{
					float RADIUS = 3.5f;
					foreach (StarMapFeature ftr in StarMap.Features)
					{
						ImagePixel center = ftr.GetCenter();
						if (center != null && center != ImagePixel.Unspecified)
						{
							g.DrawEllipse(m_Pen, (float)center.XDouble - RADIUS, (float)center.YDouble - RADIUS, 2 * RADIUS, 2 * RADIUS);
						}
					}

					g.Save();
					imgFrame.Refresh();
				}				
			}
			finally
			{
				this.Enabled = true;
				UpdateDepthControls();
			}
		}

		private void hsrcFrames_ValueChanged(object sender, EventArgs e)
		{
			lblFrameNo.Text = hsrcFrames.Value.ToString();
			ExtractStarMap();
		}

		private void tbStarMap_ValueChanged(object sender, EventArgs e)
		{
			ExtractStarMap();
		}

		private void frmAdjustStarMap_Load(object sender, EventArgs e)
		{
			hsrcFrames.Value = hsrcFrames.Minimum;
		}

		internal static StarMap AdjustStarMap(IWin32Window parentForm, IVideoController videoController)
		{
			frmAdjustStarMap frm = new frmAdjustStarMap(videoController);

			if (frm.ShowDialog(parentForm) == DialogResult.OK)
				return frm.StarMap;
			else
				return null;
		}

		private int m_Depth = 2;
		private bool m_Auto = false;

		private void btnDown_Click(object sender, EventArgs e)
		{
			m_Depth--;

			ExtractStarMap();
		}


		private void btnUp_Click(object sender, EventArgs e)
		{
			m_Depth++;
			ExtractStarMap();
		}

		private void UpdateDepthControls()
		{
			int MIN_DEPTH = 1;
			int MAX_DEPTH = 10;
			
			m_Depth = Math.Min(MAX_DEPTH, Math.Max(MIN_DEPTH, m_Depth));
			
			if (m_Auto)
			{
				btnUp.Enabled = false;
				btnDown.Enabled = false;
				lblDepth.Enabled = false;
			}
			else
			{
				btnUp.Enabled = m_Depth < MAX_DEPTH;
				btnDown.Enabled = m_Depth > MIN_DEPTH;
				lblDepth.Text = m_Depth.ToString();
				lblDepth.Enabled = true;
			}
		}

		private void rbAuto_CheckedChanged(object sender, EventArgs e)
		{
			m_Auto = rbAuto.Checked;
			ExtractStarMap();
		}
	}
}
