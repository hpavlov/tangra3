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
	}
}
