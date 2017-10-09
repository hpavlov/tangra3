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

		internal SerUseTimeStamp UseEmbeddedTimeStamps;

		public frmEnterSERFileInfo(SerFileInfo info, bool hasFireCaptureTimestamps)
		{
			InitializeComponent();

			cbxBitPix.Items.Clear();

            bool hasEmbeddedTimeStamps =
                info.SequenceStartTimeUTCHi != 0 &&
                info.SequenceStartTimeUTCHi >> 0x1F == 0;

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

			    switch(info.PixelDepthPerPlane)
			    {
			        case 12:
                        cbxBitPix.SelectedIndex = 1;
			            break;
                    case 14:
                        cbxBitPix.SelectedIndex = 2;
                        break;
                    case 16:
                        cbxBitPix.SelectedIndex = 3;
                        break;
                    default:
				        int selIndex = cbxBitPix.Items.IndexOf(TangraConfig.Settings.LastUsed.SerFileLastBitPix.ToString());
				        if (selIndex != -1)
					        cbxBitPix.SelectedIndex = selIndex;
				        else
					        cbxBitPix.SelectedIndex = 4;
			            break;
			    }
			}

			nudFrameRate.SetNUDValue(TangraConfig.Settings.LastUsed.SerFileLastFrameRate);
		    pnlFrameRate.Visible = !hasEmbeddedTimeStamps;

		    if (hasFireCaptureTimestamps)
		    {
		        cbxTimeSource.Items.Add("FireCapture Log File");
                cbxTimeSource.SelectedIndex = cbxTimeSource.Items.Count - 1;
		    }
		    else
		    {
                cbxTimeSource.SelectedIndex = hasEmbeddedTimeStamps ? 1 : 0;
                if (!hasEmbeddedTimeStamps) cbxTimeSource.Enabled = false;
            }
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			FrameRate = (double) nudFrameRate.Value;
			BitPix = Convert.ToInt32(cbxBitPix.SelectedItem);
			UseEmbeddedTimeStamps = (SerUseTimeStamp) cbxTimeSource.SelectedIndex;

			TangraConfig.Settings.LastUsed.SerFileLastBitPix = BitPix;
			TangraConfig.Settings.LastUsed.SerFileLastFrameRate = FrameRate;
			TangraConfig.Settings.Save();

			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
