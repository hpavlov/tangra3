/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Xml;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using AutoUpdate.Schema;
using AutoUpdateSelfUpdate;
using System.Threading;

namespace AutoUpdate
{
    public partial class frmUpdate : Form, IProgressUpdate
    {
        private int m_allFilesToUpdate = 0;
        internal static Exception s_Error = null;

	    private string tangra3Path;
	    private bool acceptBetaUpdates;
	    private Updater updater;

        public frmUpdate()
        {
            InitializeComponent();

	        string[] args = Environment.GetCommandLineArgs();

            tangra3Path = AppDomain.CurrentDomain.BaseDirectory;

            if (args.Length == 1)
            {
                acceptBetaUpdates = false;
            }
            else if (args.Length == 2)
            {
                acceptBetaUpdates = args[1] == "beta";
            }

            Version currVer = Assembly.GetExecutingAssembly().GetName().Version;
            lblVersion.Text = string.Format("ver.{0}.{1}", currVer.Major, currVer.Minor);

			Trace.WriteLine(string.Format("Accept Beta Updates: {0}", acceptBetaUpdates), SharedUpdateConstants.UPDATER_PROGRAM_NAME);

            updater = new Updater(tangra3Path, acceptBetaUpdates);
			Config.Instance.Load(tangra3Path, acceptBetaUpdates);

            SetMiniLayout();
        }

        private void frmUpdate_Load(object sender, EventArgs e)
        {
            updateTimer.Interval = 1000;
            updateTimer.Enabled = true;
            lblStatus.Text = "Checking for updates ...";
            pbUpdate.Style = ProgressBarStyle.Marquee;
        }

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                updateTimer.Enabled = false;

                UpdateSchema schema = updater.NewUpdatesAvailable();
                if (schema != null)
                {
                    List<UpdateObject> modulesToUpdate = new List<UpdateObject>();

                    #region Get all modules to update and refresh the information in the UI about thse updates
                    foreach (UpdateObject module in schema.AllUpdateObjects)
                    {
                        if (!module.NewUpdatesAvailable(tangra3Path))
                            continue;

                        if (!(module is Schema.SoftwareUpdate))
                            modulesToUpdate.Add(module);
                    }

                    if (modulesToUpdate.Count > 0)
                    {
                        lbModulesToUpdate.Items.Clear();
                        foreach (UpdateObject module in modulesToUpdate)
                        {
                            if (module is Schema.SoftwareMainUpdate)
								lbModulesToUpdate.Items.Add(SharedUpdateConstants.MAIN_PROGRAM_NAME);
                            else if (module is Schema.ModuleUpdate)
                                lbModulesToUpdate.Items.Add((module as Schema.ModuleUpdate).ModuleName);
                        }

                        if (modulesToUpdate.Count > 1)
                            // We only show a list of updates if there is more than one component to update
                            SetMaxiLayout();
                    }
                    #endregion

                    if (schema.AutoUpdate != null &&
                        schema.AutoUpdate.NewUpdatesAvailable(tangra3Path))
                    {
                        // Update the AutoUpdate. Will need to start another process to finish this
                        // and store the assembly for this other process as a resource

                        schema.AutoUpdate.Update(updater, tangra3Path, acceptBetaUpdates, this);
                        if (m_Abort) return;
                    }

                    #region Setup the Progress Bar and compute the files to be updated
                    m_allFilesToUpdate = 0;
                    m_CurrentFileIndex = 0;
                    foreach (UpdateObject module in modulesToUpdate)
                    {
                        if (module.NewUpdatesAvailable(tangra3Path))
                            m_allFilesToUpdate += module.AllFiles.Count;
                    }

                    pbUpdate.Maximum = m_allFilesToUpdate + 2;
                    pbUpdate.Value = 1;
                    pbUpdate.Style = ProgressBarStyle.Continuous;
                    #endregion

                    #region Prepare: Kill running instances of Tangra3
                    lblStatus.Text = "Preparing to update ...";
                    pbUpdate.Update();
                    lblStatus.Update();


                    //NOTE: Delete the OWSelfUpdate.exe file if the key presents in the registry
                    //      then set the key value to an empty string.
                    if (!string.IsNullOrEmpty(Config.Instance.SelfUpdateFileNameToDelete))
                    {
                        try
                        {
                            if (System.IO.File.Exists(Config.Instance.SelfUpdateFileNameToDelete))
                                System.IO.File.Delete(Config.Instance.SelfUpdateFileNameToDelete);

                            Config.Instance.ResetSelfUpdateFileName();
                        }
                        catch (Exception)
                        { }
                    }

                    pbUpdate.Value = 1;
                    pbUpdate.Update();
                    updater.PrepareToUpdate();
                    #endregion

                    lblStatus.Text = "Downloading ...";
                    pbUpdate.Value = 2;
                    pbUpdate.Update();
                    lblStatus.Update();

                    foreach (UpdateObject module in modulesToUpdate)
                    {
                        if (module.NewUpdatesAvailable(tangra3Path))
                        {
                            lblInfo.Text = string.Format("Updating {0} to {3}version {1} released on {2}", module.ModuleName, module.VersionString, module.ReleaseDate, acceptBetaUpdates ? "beta " : "");
                            lblInfo.Update();

                            Thread thr = new Thread(new ParameterizedThreadStart(UpdateWorkerThread));
                            thr.Start(new KeyValuePair<UpdateObject, IProgressUpdate>(module, this));

                            while (thr.IsAlive)
                            {
                                Update();
                                Application.DoEvents();
                                Thread.Sleep(250);
                            }

                            thr = null;
                            if (m_Abort) return;
                        }
                    }

					lblStatus.Text = "Restarting " + SharedUpdateConstants.MAIN_PROGRAM_NAME + " ...";
                    pbUpdate.Value = pbUpdate.Maximum;
                    pbUpdate.Update();
                    lblStatus.Update();

                    System.Threading.Thread.Sleep(1000);

                    if (System.IO.File.Exists(Config.Instance.Tangra3ExePath(tangra3Path)))
						Process.Start(Config.Instance.Tangra3ExePath(tangra3Path));

                    this.Close();
                    Application.Exit();
                }
                else
                {
                    // No new updates
                    lblStatus.Text = string.Format("There are no new {0}updates available", acceptBetaUpdates ? "beta " : "");
                    int currVer = Config.Instance.CurrentlyInstalledTangra3Version(tangra3Path);
                    lblInfo.Text = string.Format("Your version {0} is the latest", Config.Instance.VersionToVersionString(currVer));
                    pbUpdate.Maximum = 10;
                    pbUpdate.Value = 10;
                    pbUpdate.Style = ProgressBarStyle.Continuous;
                }
            }
            catch (Exception ex)
            {
                s_Error = ex;
                Close();
            }
        }

        
        private void UpdateWorkerThread(object status)
        {
            KeyValuePair<UpdateObject, IProgressUpdate> data = (KeyValuePair<UpdateObject, IProgressUpdate>)status;
            UpdateObject module = data.Key;
            IProgressUpdate ipu = data.Value;

            try
            {
				module.Update(updater, tangra3Path, acceptBetaUpdates, ipu);
            }
            catch (Exception ex)
            {
                ipu.OnError(ex);
            }
        }

        private int m_CurrentFileIndex = 0;
        private bool m_Abort = false;

        void IProgressUpdate.UpdateProgress(string message, int value)
        {
	        Invoke(new ReceieveMessageDelegate(ReceieveMessage), 0, message, value);
        }

        void IProgressUpdate.OnError(Exception error)
        {
            Trace.WriteLine(error);
			Invoke(new ReceieveMessageDelegate(ReceieveMessage), 1, error, null);
        }

        void IProgressUpdate.RefreshMainForm()
        {
            Update();
        }

        private void SetMiniLayout()
        {
            this.Width = 392;
            this.Height = 111;
            lblStatus.Visible = true;
            pbUpdate.Left = 12;
            pbUpdate.Top = 30;
            lblInfo.Left = 9;
            lblInfo.Top = 56;
            lbModulesToUpdate.Left = 12;
            lbModulesToUpdate.Top = 81;
            lbModulesToUpdate.Visible = false;
        }

        private void SetMaxiLayout()
        {
            this.Width = 392;
            this.Height = 163; 
            lblStatus.Visible = true;
            pbUpdate.Left = 12;
            pbUpdate.Top = 77;
            lblInfo.Left = 9;
            lblInfo.Top = 103;
            lbModulesToUpdate.Left = 12;
            lbModulesToUpdate.Top = 28;
            lbModulesToUpdate.Visible = true;
        }

	    internal delegate void ReceieveMessageDelegate(int messageId, object message, object argument);

        private void ReceieveMessage(int messageId, object message, object argument)
        {
			if (messageId == 0)
			{
				string msg = message as string;
				int value = (int)argument;

				m_CurrentFileIndex++;

				pbUpdate.Value = m_CurrentFileIndex + 1 <= pbUpdate.Maximum ? m_CurrentFileIndex + 1 : pbUpdate.Maximum;
				lblStatus.Text = msg;
				pbUpdate.Update();
				lblStatus.Update();
			}
			else if (messageId == 1)
			{
				var error = message as Exception;

				MessageBox.Show("An unanticipated error has occured. Please try to update again later.", SharedUpdateConstants.UPDATER_PROGRAM_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
				Trace.WriteLine(error.ToString());

				pbUpdate.Value = pbUpdate.Maximum;
				lblStatus.Text = error.Message;
				pbUpdate.Update();
				pbUpdate.Style = ProgressBarStyle.Continuous;
				lblStatus.Update();

				m_Abort = true;
			}
		}
    }
}