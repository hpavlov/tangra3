using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Context;
using Tangra.TangraService;

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
                frmUnhandledException frm = new frmUnhandledException(error);
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
            return "Tangra v." + Assembly.GetExecutingAssembly().GetName().Version.ToString() + "\r\n\r\n" +
                    m_Error.ToString() + "\r\n\r\nUser Comments:\r\n" + tbxUserComments.Text;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            string errorReport = GetErrorReport();

            Cursor = Cursors.WaitCursor;
            try
            {
                var binding = new BasicHttpBinding();
                var address = new EndpointAddress("http://208.106.227.157/CGI-BIN/TangraErrors/ErrorReports.asmx");
                var client = new TangraErrorSinkSoapClient(binding, address);
                client.ReportError(errorReport);
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
