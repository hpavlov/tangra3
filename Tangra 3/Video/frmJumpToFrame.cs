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

namespace Tangra.Video
{
	public partial class frmJumpToFrame : Form
	{
		public frmJumpToFrame()
		{
			InitializeComponent();
		}

		private void frmJumpToFrame_Load(object sender, EventArgs e)
		{
			nudFrameToJumpTo.Focus();
			nudFrameToJumpTo.Select(0, nudFrameToJumpTo.Text.Length);
		}

		private void nudFrameToJumpTo_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\r')
			{
				DialogResult = DialogResult.OK;
				Close();
			}
		}
	}
}
