using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.VideoOperations.Spectroscopy.FilterResponses;

namespace Tangra.VideoOperations.Spectroscopy.AbsFluxCalibration
{
	public partial class frmConfigureAbsFluxExport : Form
	{
		public ExportedMags SelectedExportMags = ExportedMags.None;

		public frmConfigureAbsFluxExport()
		{
			InitializeComponent();
		}

		private void cbxExpostSyntheticMags_CheckedChanged(object sender, EventArgs e)
		{
			gbxMagConfig.Enabled = cbxExpostSyntheticMags.Checked;
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			SelectedExportMags = ExportedMags.None;

			if (cbxExpostSyntheticMags.Checked)
			{
				if (cb_U.Checked) SelectedExportMags |= ExportedMags.Johnson_U;
				if (cb_B.Checked) SelectedExportMags |= ExportedMags.Johnson_B;
				if (cb_V.Checked) SelectedExportMags |= ExportedMags.Johnson_V;
				if (cb_R.Checked) SelectedExportMags |= ExportedMags.Johnson_R;
				if (cb_I.Checked) SelectedExportMags |= ExportedMags.Johnson_I;
				if (cb_Sloan_u.Checked) SelectedExportMags |= ExportedMags.Sloan_u;
				if (cb_Sloan_g.Checked) SelectedExportMags |= ExportedMags.Sloan_g;
				if (cb_Sloan_r.Checked) SelectedExportMags |= ExportedMags.Sloan_r;
				if (cb_Sloan_i.Checked) SelectedExportMags |= ExportedMags.Sloan_i;
				if (cb_Sloan_z.Checked) SelectedExportMags |= ExportedMags.Sloan_z;
			}

			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
