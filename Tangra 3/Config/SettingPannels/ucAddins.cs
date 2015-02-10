/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

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
#else
			CheckOWIntegrationStatus();
#endif
		}

#if WIN32
		private void CheckOWIntegrationStatus()
		{
			string owIntegrationError = null;
			if (!OccultWatcherHelper.IsOccultWatcherFound())
				owIntegrationError = "OccultWatcher is not installed.";
			else if (!OccultWatcherHelper.IsOccultWatcherReportingAddinFound())
				owIntegrationError = "OW IOTA Reporting add-in ver 1.8 or later is not installed.";
			else if (!m_AddinsController.OccultToolsAddinIsLoaded())
				owIntegrationError = "Occult Tools for Tangra is not loaded.";

			if (!string.IsNullOrEmpty(owIntegrationError))
			{
				cbxOwEventTimesExport.Visible = false;
				lblOWIntegrationError.Visible = true;
				lblOWIntegrationError.Text = owIntegrationError;
			}
			else
			{
				cbxOwEventTimesExport.Visible = true;
				lblOWIntegrationError.Visible = false;				
			}
		}
#endif

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
                Cursor = Cursors.WaitCursor;
                try
                {
                    btnConfigure.Tag = addin.Instance;
                    lblDisplayName.Text = addin.Instance.DisplayName;
                    lblVersion.Text = addin.Instance.Version;
	                lblAuthor.Text = addin.Instance.Author;
                    lblDescription.Text = addin.Instance.Description;
                    lblUrl.Text = addin.Instance.Url;
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
        }

        private void lblUrl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!string.IsNullOrEmpty(lblUrl.Text) && lblUrl.Text.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
				ShellHelper.OpenUrl(lblUrl.Text);
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
			ShellHelper.OpenUrl("http://www.hristopavlov.net/OccultWatcher/");
		}
	}
}
