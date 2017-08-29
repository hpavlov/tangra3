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
using Tangra.Model.Config;

namespace Tangra.Helpers
{
	public partial class frmChooseFitsFolder : Form
	{
		public frmChooseFitsFolder()
		{
			InitializeComponent();
		}

        public string SelectedFolderPath
        {
            get { return cbxFolderPath.Text; }
            set { cbxFolderPath.Text = value; }
        }

		private void btnBrowseForFolder_Click(object sender, EventArgs e)
		{
			if (Directory.Exists(cbxFolderPath.Text))
                openFitsSequenceDialog.SelectedPath = cbxFolderPath.Text;

			if (openFitsSequenceDialog.ShowDialog(this) == DialogResult.OK)
                cbxFolderPath.Text = openFitsSequenceDialog.SelectedPath;
		}

	    private void frmChooseFitsFolder_Load(object sender, EventArgs e)
	    {
	        cbxFolderPath.Items.Clear();

	        foreach (string recentFilePath in TangraConfig.Settings.RecentFiles.Lists[RecentFileType.FitsSequence])
	        {
	            if (Directory.Exists(recentFilePath))
	            {
	                cbxFolderPath.Items.Add(recentFilePath);
	            }
	        }
	    }
	}
}
