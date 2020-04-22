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
using Tangra.Model.Config;
using Tangra.Model.Image;

namespace Tangra.Video.SER
{
	public partial class frmSerStatusPopup : Form
	{
		private SerSettings m_Settings;
		private FrameStateData m_FrameState;

		public frmSerStatusPopup(SerSettings settings)
		{
			InitializeComponent();

			m_Settings = settings;
		}

		public void ShowStatus(FrameStateData frameState)
		{
			m_FrameState = frameState;

			var statusText = new StringBuilder();

			statusText.AppendLine(string.Format("Time Stamp: {0}", m_FrameState.SystemTime.ToString("dd MMM yyyy HH:mm:ss.fff")));
            statusText.AppendLine(string.Format("Reference: {0}", m_FrameState.SerTimeStampReference));

			lblStatusCombined.Text = statusText.ToString();
		}

		private void btnCopy_Click(object sender, EventArgs e)
		{
			Clipboard.SetText(lblStatusCombined.Text);
		}
	}
}
