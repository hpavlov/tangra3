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
    public partial class frmChooseFolder : Form
    {
        public frmChooseFolder()
        {
            InitializeComponent();
        }

        public frmChooseFolder(string title, string selectedFolder)
            : this()
        {
            lblSelectionTitle.Text = title;
            tbxFolderPath.Text = selectedFolder;
        }

        public string SelectedFolderPath
        {
            get { return tbxFolderPath.Text; }
            set { tbxFolderPath.Text = value; }
        }

        private void btnBrowseForFolder_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(tbxFolderPath.Text))
                browseFolderDialog.SelectedPath = tbxFolderPath.Text;

            browseFolderDialog.Description = lblSelectionTitle.Text;

            if (browseFolderDialog.ShowDialog(this) == DialogResult.OK)
                tbxFolderPath.Text = browseFolderDialog.SelectedPath;
        }
    }
}
