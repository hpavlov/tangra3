using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Config;
using Tangra.Helpers;
using Tangra.Model.Config;

namespace Tangra.Config
{
	public partial class frmMPCObserver : Form
	{
        public enum MPCHeaderSettingsMode
        {
            TangraSettings,
            NewMPCReport
        }

	    private MPCHeaderSettingsMode m_Mode;

	    public MPCObsHeader Header;

		public frmMPCObserver(MPCHeaderSettingsMode mode)
		{
			InitializeComponent();

		    m_Mode = mode;

			tbxCOD.Text = TangraConfig.Settings.Astrometry.MPCHeader.COD;

			if (!string.IsNullOrEmpty(TangraConfig.Settings.Astrometry.MPCObservatoryCode) &&
				(string.IsNullOrEmpty(tbxCOD.Text) || tbxCOD.Text == "XXX"))
			{
				tbxCOD.Text = TangraConfig.Settings.Astrometry.MPCObservatoryCode;
			}

            tbxCON.Text = TangraConfig.Settings.Astrometry.MPCHeader.CON;
            tbxOBS.Text = TangraConfig.Settings.Astrometry.MPCHeader.OBS;
            tbxMEA.Text = TangraConfig.Settings.Astrometry.MPCHeader.MEA;
            tbxTEL.Text = TangraConfig.Settings.Astrometry.MPCHeader.TEL;
            tbxCon2.Text = TangraConfig.Settings.Astrometry.MPCHeader.CON2;
            tbxAck2.Text = TangraConfig.Settings.Astrometry.MPCHeader.AC2;

            if (mode == MPCHeaderSettingsMode.TangraSettings)
            {
                tbxAck.Text = "";
                tbxAck.Enabled = false;
                tbxCom.Text = "";
                tbxCom.Enabled = false;
            }
		}

		private void btnHelp_Click(object sender, EventArgs e)
		{
			ShellHelper.OpenUrl("http://www.cfa.harvard.edu/iau/info/ObsDetails.html");
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show(
					this,
					"Tangra does not perform a validation on the MPC report header. Please make sure it is correctly formatted before submitting a report. Press 'Help' for more information.",
					"Warning",
					MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.Cancel)
				return;

			TangraConfig.Settings.Astrometry.MPCHeader.COD = tbxCOD.Text;
            TangraConfig.Settings.Astrometry.MPCHeader.CON = tbxCON.Text;
            TangraConfig.Settings.Astrometry.MPCHeader.OBS = tbxOBS.Text;
            TangraConfig.Settings.Astrometry.MPCHeader.MEA = tbxMEA.Text;
            TangraConfig.Settings.Astrometry.MPCHeader.TEL = tbxTEL.Text;
            TangraConfig.Settings.Astrometry.MPCHeader.CON2 = tbxCon2.Text;
            TangraConfig.Settings.Astrometry.MPCHeader.AC2 = tbxAck2.Text;

			if (m_Mode == MPCHeaderSettingsMode.NewMPCReport)
            {
                Header = new MPCObsHeader(TangraConfig.Settings.Astrometry.MPCHeader);
                Header.ACK = tbxAck.Text;
                Header.COM = tbxCom.Text;
            }

			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
