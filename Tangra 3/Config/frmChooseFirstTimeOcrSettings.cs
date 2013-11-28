using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Config;

namespace Tangra.Config
{
	public partial class frmChooseFirstTimeOcrSettings : Form
	{
		public frmChooseFirstTimeOcrSettings()
		{
			InitializeComponent();
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			if (rbIOTAVTI.Checked)
			{
				TangraConfig.Settings.Generic.OsdOcrEnabled = true;
				TangraConfig.Settings.Generic.OcrAskEveryTime = false;
			}
			else if (rbOther.Checked)
			{
				TangraConfig.Settings.Generic.OsdOcrEnabled = false;
				TangraConfig.Settings.Generic.OcrAskEveryTime = false;
			}
			else if (rbMany.Checked)
			{
				TangraConfig.Settings.Generic.OsdOcrEnabled = true;
				TangraConfig.Settings.Generic.OcrAskEveryTime = true;
			}

			TangraConfig.Settings.Generic.OcrInitialSetupCompleted = true;

			TangraConfig.Settings.Save();

			Close();
		}
	}
}
