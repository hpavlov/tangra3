/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Helpers;
using Tangra.Model.Helpers;

namespace Tangra.Config
{
	public partial class frmInstDelayConfigChooser : Form
	{
		public frmInstDelayConfigChooser()
		{
			InitializeComponent();

			cbxCameras.Enabled = false;
			rbDontCorrect.Checked = true;
		}

		public string SelectedCamera
		{
			get
			{
				return rbCorrect.Checked ? cbxCameras.Text : null;
			}
		}

		public void SetCameraModels(List<string> cameras, string selectedCamera, string videoStandard)
		{
			cbxCameras.Items.Clear();

			string videoStandardNameChunk = string.Concat("(", videoStandard, ")");

			if (!string.IsNullOrEmpty(videoStandard))
				cbxCameras.Items.AddRange(cameras.Where(x => x.Contains(videoStandardNameChunk)).Cast<object>().ToArray());

			if (cbxCameras.Items.Count == 0)
				cbxCameras.Items.AddRange(cameras.Cast<object>().ToArray());

			// First look for exact match
			string cameraToSelect = cameras.FirstOrDefault(x => x.Replace(" ", "").Equals(selectedCamera.Replace(" ", ""), StringComparison.InvariantCultureIgnoreCase) && x.Contains(videoStandardNameChunk));

			if (cameraToSelect == null) 
				// Then do a 'starts with' search
				cameraToSelect = cameras.FirstOrDefault(x => x.Replace(" ", "").Contains(selectedCamera.Replace(" ", "")) && x.Contains(videoStandardNameChunk));

			if (cameraToSelect != null)
			{
				cbxCameras.SelectedIndex = cbxCameras.Items.IndexOf(cameraToSelect);
				rbCorrect.Checked = true;
			}
			else
				rbDontCorrect.Checked = true;
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			ShellHelper.OpenUrl("http://www.dangl.at/ausruest/vid_tim/vid_tim1.htm");
		}

		private void rbCorrect_CheckedChanged(object sender, EventArgs e)
		{
			cbxCameras.Enabled = rbCorrect.Checked;
		}
	}
}
