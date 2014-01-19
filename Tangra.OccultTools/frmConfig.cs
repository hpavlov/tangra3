using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Tangra.OccultTools.OccultWrappers;

namespace Tangra.OccultTools	
{
    public partial class frmConfig : Form
    {
	    private OccultToolsAddinSettings m_Settings;
        private IOccultWrapper m_OccultWrapper;

        public frmConfig()
        {
            InitializeComponent();
        }

        internal void SetSettings(OccultToolsAddinSettings settings, IOccultWrapper occultWrapper)
		{
			m_Settings = settings;
            m_OccultWrapper = occultWrapper;
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

            string errorMessage = m_OccultWrapper.HasSupportedVersionOfOccult(m_Settings.OccultLocation);
            if (errorMessage != null)
            {
                MessageBox.Show(errorMessage);
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