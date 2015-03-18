using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tangra.Addins
{
	public partial class frmConfigureAddin : Form
	{
		public frmConfigureAddin()
		{
			InitializeComponent();

			SetExportMag(Properties.Settings.Default.ExportMag);
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			Properties.Settings.Default.ExportMag = GetExportMag();
			Properties.Settings.Default.Save();

			Close();
		}

		private void SetExportMag(ExportMagType type)
		{
			if (type == ExportMagType.V) rbV.Checked = true;
			else if (type == ExportMagType.R) rbR.Checked = true;
			else if (type == ExportMagType.B) rbB.Checked = true;
			else if (type == ExportMagType.J) rbJ.Checked = true;
			else if (type == ExportMagType.K) rbK.Checked = true;
			else
				rbClear.Checked = true;
		}

		private ExportMagType GetExportMag()
		{
			if (rbV.Checked) return ExportMagType.V;
			if (rbR.Checked) return ExportMagType.R;
			if (rbB.Checked) return ExportMagType.B;
			if (rbJ.Checked) return ExportMagType.J;
			if (rbK.Checked) return ExportMagType.K;
			
			return ExportMagType.Clear;
		}

		private void groupBox1_Enter(object sender, EventArgs e)
		{

		}
	}
}
