using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Helpers;
using Tangra.Model.Config;
using Tangra.PInvoke;

namespace Tangra.Video.SER
{
	public partial class frmEnterSERFileInfo : Form
	{
		public frmEnterSERFileInfo()
		{
			InitializeComponent();
		}

		internal double FrameRate = 0;

		internal int BitPix = 0;

		internal int NormVal = 0;

		public frmEnterSERFileInfo(SerFileInfo info)
		{
			InitializeComponent();

			cbxBitPix.Items.Clear();

			if (info.PixelDepthPerPlane == 8)
			{
				cbxBitPix.Items.Add("8");
				cbxBitPix.SelectedIndex = 0;
			}
			else
			{
				cbxBitPix.Items.Add("8");
				cbxBitPix.Items.Add("12");
				cbxBitPix.Items.Add("14");
				cbxBitPix.Items.Add("16");

				int selIndex = cbxBitPix.Items.IndexOf(TangraConfig.Settings.LastUsed.SerFileLastBitPix.ToString());
				if (selIndex != -1)
					cbxBitPix.SelectedIndex = selIndex;
				else
					cbxBitPix.SelectedIndex = 4;
			}

			nudFrameRate.SetNUDValue(TangraConfig.Settings.LastUsed.SerFileLastFrameRate);
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			FrameRate = (double) nudFrameRate.Value;
			BitPix = Convert.ToInt32(cbxBitPix.SelectedItem);

			TangraConfig.Settings.LastUsed.SerFileLastBitPix = BitPix;
			TangraConfig.Settings.LastUsed.SerFileLastFrameRate = FrameRate;
			TangraConfig.Settings.Save();

			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
