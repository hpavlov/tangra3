/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tangra.Helpers
{
	public partial class frmChooseFitsFolder : Form
	{
		public frmChooseFitsFolder()
		{
			InitializeComponent();
		}

		private void btnBrowseForFolder_Click(object sender, EventArgs e)
		{
			if (Directory.Exists(tbxFolderPath.Text))
				openFitsSequenceDialog.SelectedPath = tbxFolderPath.Text;

			if (openFitsSequenceDialog.ShowDialog(this) == DialogResult.OK)
				tbxFolderPath.Text = openFitsSequenceDialog.SelectedPath;
		}
	}
}
