using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Addins;
using Tangra.Config;
using Tangra.Config.SettingPannels;
using Tangra.Controller;
using Tangra.Helpers;
using Tangra.Model.Config;
using Tangra.SDK;

namespace Tangra.Config.SettingPannels
{
    public partial class ucAddins : SettingsPannel
    {
        private AddinsController m_AddinsController;
	    private List<IAddinContainer> m_AddinContainers = new List<IAddinContainer>();

		public ucAddins(AddinsController addinsController, IAddinContainer[] addinContainers)
		{
			InitializeComponent();

            m_AddinsController = addinsController;
			m_AddinContainers.AddRange(addinContainers);

#if !WIN32
			pnlOccultWatcherSettings.Visible = false;
#endif
		}

        public override void LoadSettings()
        {
            cbxIsolationLevel.SelectedIndex = (int) TangraConfig.Settings.Generic.AddinIsolationLevel;
            tbxAddinsDirectory.Text = AddinManager.ADDINS_DIRECTORY;
			cbxOwEventTimesExport.SelectedIndex = (int) TangraConfig.Settings.Generic.OWEventTimesExportMode;

            m_AddinsController.ShowLoadedAddins(lbxLoadedAddins);
            lbxLoadedAddins.SelectedIndex = -1;
            pnlAddinInfo.Visible = false;
            btnConfigure.Enabled = false;
			btnReloadAddins.Visible = m_AddinsController.CanReloadAddins();
	        btnUnloadAddins.Visible = m_AddinsController.CanUnloadAddins();
        }

        public override void SaveSettings()
        {
            TangraConfig.Settings.Generic.AddinIsolationLevel = (TangraConfig.IsolationLevel) cbxIsolationLevel.SelectedIndex;
			TangraConfig.Settings.Generic.OWEventTimesExportMode = (TangraConfig.OWExportMode) cbxOwEventTimesExport.SelectedIndex;
        }

        private void lbxLoadedAddins_SelectedIndexChanged(object sender, EventArgs e)
        {
            var addin = lbxLoadedAddins.SelectedItem as Addin;
            pnlAddinInfo.Visible = addin != null;
            btnConfigure.Enabled = addin != null;
            if (addin != null)
            {
                btnConfigure.Tag = addin.Instance;
                lblDisplayName.Text = addin.Instance.DisplayName;
                lblVersion.Text = addin.Instance.Version;
                lblDescription.Text = addin.Instance.Description;
                lblUrl.Text = addin.Instance.Url;

            }
        }

        private void lblUrl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!string.IsNullOrEmpty(lblUrl.Text) && lblUrl.Text.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
                Process.Start(lblUrl.Text);
        }

        private void btnConfigure_Click(object sender, EventArgs e)
        {
            var addinInstance = btnConfigure.Tag as ITangraAddin;
            if (addinInstance != null)
                addinInstance.Configure();
        }

        private void btnReloadAddins_Click(object sender, EventArgs e)
        {
            if (ParentForm != null)
                ParentForm.Enabled = false;

            Cursor = Cursors.WaitCursor;

            try
            {
                TangraConfig.Settings.Generic.AddinIsolationLevel = (TangraConfig.IsolationLevel)cbxIsolationLevel.SelectedIndex;

                m_AddinsController.ReloadAddins();
	            m_AddinContainers.ForEach(x => x.ReloadAddins());
            }
            finally
            {
                Cursor = Cursors.Default;

                if (ParentForm != null) 
                    ParentForm.Enabled = true;

                LoadSettings();
            }
        }

        private void btnUnloadAddins_Click(object sender, EventArgs e)
        {
            if (ParentForm != null)
                ParentForm.Enabled = false;

            Cursor = Cursors.WaitCursor;

            try
            {
                m_AddinsController.UnloadAddins();
            }
            finally
            {
                Cursor = Cursors.Default;

                if (ParentForm != null)
                    ParentForm.Enabled = true;

                LoadSettings();
            }
        }

		private void btnNavigateTo_Click(object sender, EventArgs e)
		{
			Process.Start(tbxAddinsDirectory.Text);
		}

		private void linkLblOW_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("http://www.hristopavlov.net/OccultWatcher/");
		}
	}
}
