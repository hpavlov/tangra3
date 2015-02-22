/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tangra.VideoOperations.Astrometry.MPCReport
{
    public partial class frmDisplayMPCReport : Form
    {
        private MPCReportFile m_ReportFile;
        private IMPCReportFileManager m_ReportFileManager;

        public frmDisplayMPCReport(MPCReportFile reportFile, IMPCReportFileManager manager)
        {
            InitializeComponent();

            m_ReportFile = reportFile;
            m_ReportFileManager = manager;
        }

        internal void ParseReportFile(MPCReportFile reportFile, IMPCReportFileManager manager)
        {
            m_ReportFile = reportFile;
            m_ReportFileManager = manager;

            LoadReportFileData();
        }

        private void LoadReportFileData()
        {
            Text = string.Format("MPC Report - {0}", System.IO.Path.GetFileName(m_ReportFile.ReportFileName));
            tbxData.Text = m_ReportFile.ToAscii();            
        }

        private void frmDisplayMPCReport_Load(object sender, EventArgs e)
        {
            LoadReportFileData();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(tbxData.Text);
        }

        private void btnNewFile_Click(object sender, EventArgs e)
        {
            m_ReportFileManager.CloseReportFile();
            Close();
        }
    }
}
