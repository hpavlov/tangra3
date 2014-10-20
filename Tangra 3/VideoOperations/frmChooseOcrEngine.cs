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
using Tangra.Model.Config;
using Tangra.Model.Context;
using Tangra.Model.Image;
using Tangra.OCR;
using Tangra.VideoOperations.LightCurves;

namespace Tangra.VideoOperations
{
	public partial class frmChooseOcrEngine : Form
	{
		internal bool ShowForceErrorReportOption;

		internal bool ForceErrorReport
		{
			get { return cbxForceErrorReport.Checked; }
		}

		public frmChooseOcrEngine()
		{
			InitializeComponent();

			if (!string.IsNullOrEmpty(TangraConfig.Settings.Generic.OcrEngine))
				cbxOcrEngine.SelectedIndex = cbxOcrEngine.Items.IndexOf(TangraConfig.Settings.Generic.OcrEngine);	
			
			if (cbxOcrEngine.SelectedIndex == -1)
				cbxOcrEngine.SelectedIndex = 0;

			cbxEnableOsdOcr.Checked = TangraConfig.Settings.Generic.OsdOcrEnabled;
			cbxOcrAskEveryTime.Checked = TangraConfig.Settings.Generic.OcrAskEveryTime;
			pnlOsdOcr.Enabled = cbxEnableOsdOcr.Checked;
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			TangraConfig.Settings.Generic.OsdOcrEnabled = cbxEnableOsdOcr.Checked;
			TangraConfig.Settings.Generic.OcrEngine = cbxOcrEngine.Text;
			TangraConfig.Settings.Generic.OcrAskEveryTime = cbxOcrAskEveryTime.Checked;
			
			TangraConfig.Settings.Save();

			DialogResult = DialogResult.OK;
			Close();
		}

		private void cbxEnableOsdOcr_CheckedChanged(object sender, EventArgs e)
		{
			pnlOsdOcr.Enabled = cbxEnableOsdOcr.Checked;
		}

		private void frmChooseOcrEngine_Load(object sender, EventArgs e)
		{
			cbxForceErrorReport.Visible = ShowForceErrorReportOption;
			cbxForceErrorReport.Checked = ShowForceErrorReportOption;
		}
	}
}
