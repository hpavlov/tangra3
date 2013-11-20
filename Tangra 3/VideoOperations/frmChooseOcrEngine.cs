using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Controller;
using Tangra.Model.Config;
using Tangra.Model.Context;
using Tangra.Model.Image;
using Tangra.OCR;
using Tangra.VideoOperations.LightCurves;

namespace Tangra.VideoOperations
{
	public partial class frmChooseOcrEngine : Form
	{
		public frmChooseOcrEngine()
		{
			InitializeComponent();

			if (!string.IsNullOrEmpty(TangraConfig.Settings.Generic.OcrEngine))
				cbxOcrEngine.SelectedIndex = cbxOcrEngine.Items.IndexOf(TangraConfig.Settings.Generic.OcrEngine);	
			
			if (cbxOcrEngine.SelectedIndex == -1)
				cbxOcrEngine.SelectedIndex = 0;

			//cbxDontAsk.Checked = !TangraConfig.Settings.Generic.OsdOcrAskMeToConfirmEveryTime;
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			TangraConfig.Settings.Generic.OcrEngine = cbxOcrEngine.Text;
			//TangraConfig.Settings.Generic.OsdOcrAskMeToConfirmEveryTime = !cbxDontAsk.Checked;
			TangraConfig.Settings.Save();

			DialogResult = DialogResult.OK;
			Close();
		}

	}
}
