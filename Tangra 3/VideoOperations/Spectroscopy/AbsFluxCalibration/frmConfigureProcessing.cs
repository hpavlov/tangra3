using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Helpers;

namespace Tangra.VideoOperations.Spectroscopy.AbsFluxCalibration
{
	public partial class frmConfigureProcessing : Form
	{
		internal CalibrationContext Context;

		public frmConfigureProcessing()
		{
			InitializeComponent();
		}

		public frmConfigureProcessing(CalibrationContext context)
			: this()
		{
			Context = context;

			nudMinWavelength.SetNUDValue(context.FromWavelength);
			nudMaxWavelength.SetNUDValue(context.ToWavelength);
			nudResolution.SetNUDValue(context.WavelengthBinSize);
			cbxUseBlurring.Checked = context.UseBlurring;
			rbNonLinearMag.Checked = Context.Model == AbsFluxModel.NonLinearMag;
			rbNonLinearGain.Checked = Context.Model == AbsFluxModel.NonLinearGain;
			rbLinear.Checked = Context.Model == AbsFluxModel.Linear;
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			Context.FromWavelength = (int)nudMinWavelength.Value;
			Context.ToWavelength = (int)nudMaxWavelength.Value;
			Context.WavelengthBinSize = (int)nudResolution.Value;
			Context.UseBlurring = cbxUseBlurring.Checked;

			if (rbNonLinearMag.Checked)
				Context.Model = AbsFluxModel.NonLinearMag;
			else if (rbNonLinearGain.Checked)
				Context.Model = AbsFluxModel.NonLinearGain;
			else if (rbLinear.Checked)
				Context.Model = AbsFluxModel.Linear;

			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
