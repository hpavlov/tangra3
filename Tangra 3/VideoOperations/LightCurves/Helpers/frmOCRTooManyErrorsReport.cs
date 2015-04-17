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
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Tangra.VideoOperations.LightCurves.Helpers
{
	public partial class frmOCRTooManyErrorsReport : Form
	{
		internal int OCRErrors;

		private Regex m_SimpleEmailRegex = new Regex("^[^@]+@[^\\.]+\\.[^\\.]+$");

		public frmOCRTooManyErrorsReport()
		{
			InitializeComponent();
		}

		private void tbxEmail_TextChanged(object sender, EventArgs e)
		{
			btnSend.Enabled = m_SimpleEmailRegex.IsMatch(tbxEmail.Text);
		}

		private void frmOCRTooManyErrorsReport_Load(object sender, EventArgs e)
		{
			lblInfo.Text = string.Format(lblInfo.Text, OCRErrors);
		}
	}
}
