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
using Tangra.Config;
using Tangra.Config.SettingPannels;
using Tangra.Helpers;
using Tangra.Model.Config;

namespace Tangra.Config.SettingPannels
{
    public partial class ucGeneralTangra : SettingsPannel
    {
		public ucGeneralTangra()
		{
			InitializeComponent();
		}

        public override void LoadSettings()
        {
			cbPerformanceQuality.SetCBXIndex((int)TangraConfig.Settings.Generic.PerformanceQuality);
			cbxRunVideosAtFastest.Checked = TangraConfig.Settings.Generic.RunVideosOnFastestSpeed;
			cbxOnOpenOperation.SetCBXIndex((int)TangraConfig.Settings.Generic.OnOpenOperation);
			cbShowProcessingSpeed.Checked = TangraConfig.Settings.Generic.ShowProcessingSpeed;
			cbxShowCursorPosition.Checked = TangraConfig.Settings.Generic.ShowCursorPosition;
			cbxBetaUpdates.Checked = TangraConfig.Settings.Generic.AcceptBetaUpdates;
			cbxSendStats.Checked = TangraConfig.Settings.Generic.SubmitUsageStats;

	        UpdateFileAssociationsButtonState();
        }

        public override void SaveSettings()
        {
			TangraConfig.Settings.Generic.PerformanceQuality = (TangraConfig.PerformanceQuality)cbPerformanceQuality.SelectedIndex;

			TangraConfig.Settings.Generic.AcceptBetaUpdates = cbxBetaUpdates.Checked;
			TangraConfig.Settings.Generic.RunVideosOnFastestSpeed = cbxRunVideosAtFastest.Checked;
			TangraConfig.Settings.Generic.OnOpenOperation = (TangraConfig.OnOpenOperation)cbxOnOpenOperation.SelectedIndex;
			TangraConfig.Settings.Generic.ShowProcessingSpeed = cbShowProcessingSpeed.Checked;
			TangraConfig.Settings.Generic.ShowCursorPosition = cbxShowCursorPosition.Checked;
			TangraConfig.Settings.Generic.SubmitUsageStats = cbxSendStats.Checked;
        }

		private void UpdateFileAssociationsButtonState()
		{
			if (CurrentOS.IsWindows)
			{
				btnAssociateFiles.Visible = true;
				btnAssociateFiles.Enabled = true;
				var fileAssociation = new TangraFileAssociations();

				if (fileAssociation.Registered)
				{
					btnAssociateFiles.Text = "Re-Associate files with Tangra";
				}
				else
				{
					btnAssociateFiles.Text = "Associate files with Tangra";

					if (!fileAssociation.CanRegisterWithoutElevation)
					{
						btnAssociateFiles.FlatStyle = FlatStyle.System;
						WindowsHelpers.SendMessage(btnAssociateFiles.Handle, WindowsHelpers.BCM_SETSHIELD, 0, (IntPtr)1);
					}
				}

				if (fileAssociation.CanRegisterWithoutElevation)
					btnAssociateFiles.Tag = fileAssociation;
			}
			else
				btnAssociateFiles.Visible = false;			
		}

		private void btnAssociateFiles_Click(object sender, EventArgs e)
		{
			TangraFileAssociations fileAssocHelper = btnAssociateFiles.Tag as TangraFileAssociations;
			if (fileAssocHelper != null)
			{
				fileAssocHelper.Associate();
				UpdateFileAssociationsButtonState();
			}
			else
			{
				// Launch itself as administrator passing the right parameter the do the job
				var proc = new ProcessStartInfo();
				proc.UseShellExecute = true;
				proc.WorkingDirectory = Environment.CurrentDirectory;
				proc.FileName = Application.ExecutablePath;
				proc.Arguments = TangraFileAssociations.COMMAND_LINE_ASSOCIATE;
				proc.Verb = "runas";

				btnAssociateFiles.Enabled = false;
				try
				{
					Process newProcess = new Process() { StartInfo = proc };
					newProcess.Start();
					newProcess.WaitForExit();
				}
				catch
				{
					// The user refused to allow privileges elevation.
					// Do nothing and return directly ...
					return;
				}
				finally
				{
					UpdateFileAssociationsButtonState();
				}				
			}
		}
	}
}
