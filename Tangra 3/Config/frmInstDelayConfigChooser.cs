using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tangra.Config
{
	public partial class frmInstDelayConfigChooser : Form
	{
		public frmInstDelayConfigChooser()
		{
			InitializeComponent();
		}

		public string SelectedCamera
		{
			get
			{
				return cbxCameras.SelectedText;
			}
		}

		public void SetCameraModels(List<string> cameras, string selectedCamera, string videoStandard)
		{
			cbxCameras.Items.Clear();
			cbxCameras.Items.AddRange(cameras.Cast<object>().ToArray());

			string cameraToSelect = cameras.SingleOrDefault(x => x.Replace(" ", "").Contains(selectedCamera.Replace(" ", "")) && x.Contains(videoStandard));
			if (cameraToSelect != null)
				cbxCameras.SelectedIndex = cbxCameras.Items.IndexOf(cameraToSelect);
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("http://www.dangl.at/ausruest/vid_tim/vid_tim1.htm");
		}
	}
}
