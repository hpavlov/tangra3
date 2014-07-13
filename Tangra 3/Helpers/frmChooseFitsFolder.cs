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
