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
using Tangra.Config;
using Tangra.Model.Config;

namespace Tangra.VideoOperations.Astrometry.MPCReport
{
	public partial class frmChooseReportFile : Form
	{
		internal class RecentObsFile
		{
			public string FilePath;
			public DateTime LastModified;
			private string m_DisplayName;

			public RecentObsFile(string filePath, FileInfo fi)
			{
				FilePath = filePath;
				LastModified = fi.LastWriteTime;

				m_DisplayName = string.Format("{0} - {1}", Path.GetFileNameWithoutExtension(FilePath), LastModified.ToString("dd MMM yyyy - HH:mm:ss"));
			}

			public override string ToString()
			{
				return m_DisplayName;
			}
		}

		internal bool IsNewReport = false;
		internal string ReportFileName;

		public frmChooseReportFile()
		{
			InitializeComponent();

            m_LastManyDays = 30;
		    LoadFiles();
		}

        private void LoadFiles()
        {
            lbxAvailabeReports.Items.Clear();
            lbxAvailabeReports.Items.Add("[New Report File]");

            // Load all available reports:
            DateTime dtNow = DateTime.Now;

            List<RecentObsFile> obsFiles = new List<RecentObsFile>();

			foreach (string obsFile in TangraConfig.Settings.RecentFiles.Lists[RecentFileType.MPCReport])
            {
                if (File.Exists(obsFile))
                {
                    FileInfo fi = new FileInfo(obsFile);
                    if (fi.LastWriteTime.AddDays(m_LastManyDays) > dtNow)
                        obsFiles.Add(new RecentObsFile(obsFile, fi));
                }
            }

            lbxAvailabeReports.Items.AddRange(obsFiles.ToArray());
            lbxAvailabeReports.SelectedIndex = 0;            
        }

		private void btnOK_Click(object sender, EventArgs e)
		{
			IsNewReport = lbxAvailabeReports.SelectedIndex == 0;

            RecentObsFile selectedObsFile = lbxAvailabeReports.SelectedItem as RecentObsFile;
			if (selectedObsFile != null)
			{
				ReportFileName = selectedObsFile.FilePath;
				TangraConfig.Settings.RecentFiles.NewRecentFile(RecentFileType.MPCReport, ReportFileName);
				TangraConfig.Settings.RecentFiles.Lists[RecentFileType.MPCReport].Insert(0, ReportFileName);
				TangraConfig.Settings.Save();
			}

			DialogResult = DialogResult.OK;
			Close();
		}

	    private int m_LastManyDays;

        private void FilterReportFilesByLastModifiedDate(object sender, EventArgs e)
        {
            var rb = (sender as RadioButton);
            if (rb != null)
            {
                m_LastManyDays = Convert.ToInt32(rb.Tag);
                LoadFiles();    
            }
            
        }

        private void btnDeleteAll_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                "Please confirm that you want to delete all report files. This cannot be undone.", 
                "Warning", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
				foreach (string obsFile in TangraConfig.Settings.RecentFiles.Lists[RecentFileType.MPCReport])
                {
                    if (File.Exists(obsFile))
                    {
                        try
                        {
                            File.Delete(obsFile);
                        }
                        catch(Exception)
                        { }
                    }
                }

                LoadFiles();
            }

        }
	}
}
