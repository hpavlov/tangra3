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
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using Tangra.Model.Config;
using Tangra.Model.Context;
using Tangra.TangraService;
using System.Diagnostics;

namespace Tangra
{
    public partial class frmUnhandledException : Form
    {
        private Exception m_Error;
        private static bool m_RestartRequest = true;
        private static object m_SyncRoot = new object();

        public frmUnhandledException(Exception error)
        {
            InitializeComponent();

            m_Error = error;
	        tbxEmailAddress.Text = TangraConfig.Settings.LastUsed.EmailAddressForErrorReporting;
        }

        internal static void HandleException(object sender, Exception error)
        {
            lock (m_SyncRoot)
            {
                if (TangraContext.Current.HasRestartRequest())
                    return;
            }

            if (error != null)
            {
                var frm = new frmUnhandledException(error);
                frm.StartPosition = FormStartPosition.CenterParent;

                frm.ShowDialog(sender as IWin32Window);
            }
        }

        internal static void HandleExceptionNoRestart(object sender, Exception error)
        {
            m_RestartRequest = false;

            HandleException(sender, error);
        }
        

        private void lblDetails_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string errorReport = GetErrorReport();
            Clipboard.SetText(errorReport);

            MessageBox.Show("The error report has been copied to the clipboard.\r\nPaste it in a text editor (for example Notepad) to review it.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private string GetErrorReport()
        {
            return "Tangra 3 Error Report\r\n\r\n" +
                    m_Error.ToString() + "\r\n\r\nUser Comments:\r\n" +
					"User Email: " + (tbxEmailAddress.Text.Trim().Length == 0 ? "[UNSPECIFIED]" : tbxEmailAddress.Text) + "\r\n\r\n" + 
					tbxUserComments.Text + "\r\n\r\n" + 
					GetExtendedTangraInfo();
        }

        private string GetExtendedTangraInfo()
        {
            var ser = new XmlSerializer(typeof (CrashReportInfo));
            var crashReportInfo = new StringBuilder();

            if (TangraContext.Current.CrashReportInfo != null)
            {
                try
                {
                    TangraContext.Current.CrashReportInfo.WorkingSet64 = Process.GetCurrentProcess().WorkingSet64;
                    TangraContext.Current.CrashReportInfo.VirtualMemorySize64 = Process.GetCurrentProcess().VirtualMemorySize64;
                    TangraContext.Current.CrashReportInfo.PrivateMemorySize64 = Process.GetCurrentProcess().PrivateMemorySize64;
                    TangraContext.Current.CrashReportInfo.PeakWorkingSet64 = Process.GetCurrentProcess().PeakWorkingSet64;
                    TangraContext.Current.CrashReportInfo.MinWorkingSet = Process.GetCurrentProcess().MinWorkingSet.ToInt64();
                    TangraContext.Current.CrashReportInfo.MaxWorkingSet = Process.GetCurrentProcess().MaxWorkingSet.ToInt64();
                }
                catch
                { }
                
                using(TextWriter wrt = new StringWriter(crashReportInfo))
                {
                    ser.Serialize(wrt, TangraContext.Current.CrashReportInfo);
                    wrt.Flush();
                }

	            if (TangraContext.Current.CrashReportInfo.ReductionContext != null)
		            crashReportInfo.Append("\r\n\r\n" + TangraContext.Current.CrashReportInfo.ReductionContext + "\r\n\r\n");
            }

			var moduleInfo = new StringBuilder("\r\n\r\nLoaded modules:\r\n\r\n");
			foreach (ProcessModule module in Process.GetCurrentProcess().Modules)
			{
				try
				{
					moduleInfo.AppendFormat("{0} v{1}, {2}\r\n", module.ModuleName, module.FileVersionInfo.FileVersion, module.FileName);	
				}
				catch
				{ }				
			}

            return
                frmSystemInfo.GetFullVersionInfo() + "\r\n" +
                string.Format("Tangra Install Location: {0}\r\n", AppDomain.CurrentDomain.BaseDirectory) +
                crashReportInfo.ToString() +
				moduleInfo.ToString();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            string errorReport = GetErrorReport();

            Cursor = Cursors.WaitCursor;
            try
            {
                var binding = new BasicHttpBinding();
                var address = new EndpointAddress("http://www.tangra-observatory.org/TangraErrors/ErrorReports.asmx");
                var client = new TangraService.ServiceSoapClient(binding, address);
                client.ReportError(errorReport);
	            
				TangraConfig.Settings.LastUsed.EmailAddressForErrorReporting = tbxEmailAddress.Text;
	            TangraConfig.Settings.Save();

                if (MessageBox.Show(
                    "The error report was sent successfully. Press 'Retry' to try to continue or 'Cancel' to restart Tangra.", 
                    "Question", 
                    MessageBoxButtons.RetryCancel, 
                    MessageBoxIcon.Question, 
                    MessageBoxDefaultButton.Button1) == DialogResult.Cancel)
                {
                    Application.Restart();
                }
            }
            finally
            {
                Cursor = Cursors.Default;
                Close();
            }
        }

        private void frmUnhandledException_FormClosed(object sender, FormClosedEventArgs e)
        {
            lock (m_SyncRoot)
            {
                if (TangraContext.Current.HasRestartRequest())
                    return;

                if (m_RestartRequest)
                {
                    MessageBox.Show("Tangra will now restart", "Restart", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    TangraContext.Current.RestartApplication();
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

    }
}
