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
using Tangra.Config;
using Tangra.Helpers;
using Tangra.Model.Config;

namespace Tangra.VideoOperations.Astrometry
{
	public partial class frmFirstTimeCalibrationRequired : Form
	{
		public frmFirstTimeCalibrationRequired()
		{
			InitializeComponent();
		}

		private void btnContinue_Click(object sender, EventArgs e)
		{
			if (cbxNotShowAgain.Checked)
			{
				TangraConfig.Settings.HelpFlags.DontShowCalibrationHelpFormAgain = true;
				TangraConfig.Settings.Save();
			}

			DialogResult = DialogResult.OK;
			Close();
		}

        private void lnkCalinrationHelpLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShellHelper.OpenUrl("http://www.hristopavlov.net/Tangra/Calibration/");
        }
	}
}
