using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tangra.VideoTools
{
	public partial class frmGenerate3DPolyBackground : Form
	{
		internal int PolyOrder;
		internal double PolyDepth;
		internal BackgroundModelGenerator BackgroundModelGenerator { get; private set; }

		public frmGenerate3DPolyBackground()
		{
			InitializeComponent();
		}

		internal frmGenerate3DPolyBackground(BackgroundModelGenerator bgModelGenerator)
		{
			InitializeComponent();

			BackgroundModelGenerator = bgModelGenerator;

		}

		private void btnRandom_Click(object sender, EventArgs e)
		{
			if (BackgroundModelGenerator != null)
			{
				if (rbOrder1.Checked) PolyOrder = 1;
				else if (rbOrder2.Checked) PolyOrder = 2;
				else if (rbOrder3.Checked) PolyOrder = 3;

				PolyDepth = (double)nudPolyDepth.Value;

				BackgroundModelGenerator.GenerateBackgroundModelParameters(PolyOrder, PolyDepth);
			}

			DisplayModelParameters();
			BackgroundModelGenerator.PlotBackground(pboxPlot, PolyOrder);
		}

		private void frmGenerate3DPolyBackground_Load(object sender, EventArgs e)
		{
			DisplayModelParameters();
			BackgroundModelGenerator.PlotBackground(pboxPlot, PolyOrder);
		}

		private void DisplayModelParameters()
		{
			tbxA.Text = BackgroundModelGenerator.A.ToString();
			tbxB.Text = BackgroundModelGenerator.B.ToString();
			tbxC.Text = BackgroundModelGenerator.C.ToString();
			if (rbOrder2.Checked || rbOrder3.Checked)
			{
				tbxD.Text = BackgroundModelGenerator.D.ToString();
				tbxE.Text = BackgroundModelGenerator.E.ToString();
				tbxF.Text = BackgroundModelGenerator.F.ToString();
			}
			else
			{
				tbxD.Text = string.Empty;
				tbxE.Text = string.Empty;
				tbxF.Text = string.Empty;
			}

			if (rbOrder3.Checked)
			{
				tbxG.Text = BackgroundModelGenerator.G.ToString();
				tbxH.Text = BackgroundModelGenerator.H.ToString();
				tbxI.Text = BackgroundModelGenerator.I.ToString();
				tbxJ.Text = BackgroundModelGenerator.J.ToString();
			}
			else
			{
				tbxG.Text = string.Empty;
				tbxH.Text = string.Empty;
				tbxI.Text = string.Empty;
				tbxJ.Text = string.Empty;
			}
		}

		private void PolyOrderChanged(object sender, EventArgs e)
		{
			if (rbOrder1.Checked)
			{
				lblPolyEq.Text = "z = A * x + B * x + C";
				tbxD.Enabled = false;
				tbxE.Enabled = false;
				tbxF.Enabled = false;
				tbxG.Enabled = false;
				tbxH.Enabled = false;
				tbxI.Enabled = false;
				tbxJ.Enabled = false;
			}
			else if (rbOrder2.Checked)
			{
				lblPolyEq.Text = "z = A * x^2 + B * x * y + C * y^2 + D * x + E * y + F";
				tbxD.Enabled = true;
				tbxE.Enabled = true;
				tbxF.Enabled = true;
				tbxG.Enabled = false;
				tbxH.Enabled = false;
				tbxI.Enabled = false;
				tbxJ.Enabled = false;

			}
			else if (rbOrder3.Checked)
			{
				lblPolyEq.Text = "z = A * x^3 + B * x^2 * y + C * x * y^2 +  D * y^3 + E * x^2 + F * x * y + G * y^2 + H * x + I * y + J";
				tbxD.Enabled = true;
				tbxE.Enabled = true;
				tbxF.Enabled = true;
				tbxG.Enabled = true;
				tbxH.Enabled = true;
				tbxI.Enabled = true;
				tbxJ.Enabled = true;
			}

			DisplayModelParameters();
			BackgroundModelGenerator.PlotBackground(pboxPlot, PolyOrder);
		}

		private void btnPlot_Click(object sender, EventArgs e)
		{
			if (!MapModelParameters()) return;

			BackgroundModelGenerator.PlotBackground(pboxPlot, PolyOrder);
		}

		private bool MapModelParameters()
		{
			if (rbOrder1.Checked) PolyOrder = 1;
			else if (rbOrder2.Checked) PolyOrder = 2;
			else if (rbOrder3.Checked) PolyOrder = 3;

			PolyDepth = (double)nudPolyDepth.Value;
			
			if (!GetParamValue(tbxA, x => BackgroundModelGenerator.A = x)) return false;
			if (!GetParamValue(tbxB, x => BackgroundModelGenerator.B = x)) return false;
			if (!GetParamValue(tbxC, x => BackgroundModelGenerator.C = x)) return false;

			if (rbOrder2.Checked || rbOrder3.Checked)
			{
				if (!GetParamValue(tbxD, x => BackgroundModelGenerator.D = x)) return false;
				if (!GetParamValue(tbxE, x => BackgroundModelGenerator.E = x)) return false;
				if (!GetParamValue(tbxF, x => BackgroundModelGenerator.F = x)) return false;
			}

			if (rbOrder3.Checked)
			{
				if (!GetParamValue(tbxG, x => BackgroundModelGenerator.G = x)) return false;
				if (!GetParamValue(tbxH, x => BackgroundModelGenerator.H = x)) return false;
				if (!GetParamValue(tbxI, x => BackgroundModelGenerator.I = x)) return false;
				if (!GetParamValue(tbxJ, x => BackgroundModelGenerator.J = x)) return false;
			}

			return true;
		}

		private bool GetParamValue(TextBox tbx, Action<double> setter)
		{
			try
			{
				double intVal = double.Parse(tbx.Text);
				setter(intVal);
			}
			catch (FormatException fex)
			{
				MessageBox.Show(this, fex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				tbxA.Focus();
				return false;
			}

			return true;
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			if (!MapModelParameters()) return;

			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
