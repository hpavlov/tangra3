using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Tangra.OccultTools	
{
    public partial class frmConfig : Form
    {
	    private OccultToolsAddinSettings m_Settings;

        public frmConfig()
        {
            InitializeComponent();
        }

		internal void SetSettings(OccultToolsAddinSettings settings)
		{
			m_Settings = settings;
			tbxOccultPath.Text = m_Settings.OccultLocation;
		}

        private void butLocateExec_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
            {
                tbxOccultPath.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(tbxOccultPath.Text))
            {
                MessageBox.Show(string.Format("Path '{0}' does not exist", tbxOccultPath.Text));
                tbxOccultPath.SelectAll();
                tbxOccultPath.Focus();
                return;
            }

            if (!File.Exists(Path.Combine(tbxOccultPath.Text,"OccultUtilities.dll")))
            {
                MessageBox.Show(string.Format("Cannot find Occult4 installation in '{0}'", tbxOccultPath.Text));
                tbxOccultPath.SelectAll();
                tbxOccultPath.Focus();
                return;
            }

			m_Settings.OccultLocation = tbxOccultPath.Text;

			if (!OccultUtilitiesWrapper.HasSupportedVersionOfOccult(m_Settings.OccultLocation))
            {
				MessageBox.Show("This addin requires Occult version 4.1.0.12 or later");
                tbxOccultPath.SelectAll();
                tbxOccultPath.Focus();
                return; 
            }

			m_Settings.Save();
            DialogResult = DialogResult.OK;
            Close();
        }

		private void llOccult4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("http://www.lunar-occultations.com/iota/occult4.htm");
		}
    }
}