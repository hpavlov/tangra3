using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tangra.KweeVanWoerden
{
	public partial class frmResults : Form
	{
		internal KweeVanWoerdenMinimum.KweeVanWoerdenResult Results;

		public frmResults()
		{
			InitializeComponent();
		}


		private void btnSaveFiles_Click(object sender, EventArgs e)
		{
			if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
			{
				File.WriteAllLines(Path.GetFullPath(folderBrowserDialog1.SelectedPath + @"\Observations_File.txt"), Results.Observations_File);
				File.WriteAllLines(Path.GetFullPath(folderBrowserDialog1.SelectedPath + @"\Normals_File.txt"), Results.Normals_File);
				File.WriteAllLines(Path.GetFullPath(folderBrowserDialog1.SelectedPath + @"\Summary_File.txt"), Results.Summary_File);

				Process.Start(folderBrowserDialog1.SelectedPath);
			}
		}

		private void frmResults_Load(object sender, EventArgs e)
		{
			if (Results.Success)
			{
				tbxErrorMessage.Visible = false;
				lblCalcStatus.ForeColor = Color.Green;
				lblCalcStatus.Text = "The calculation was successful";
			}
			else
			{
				tbxErrorMessage.Text = Results.ErrorMessage;
				tbxErrorMessage.Visible = true;
				lblCalcStatus.ForeColor = Color.Red;
				lblCalcStatus.Text = "Error occurred during the calculation";
			}

			// 1E-6 days is a precision of 0.08 sec - good enough for our purposes
			tbxT0JD.Text = Results.Time_Of_Minimum_JD.ToString("0.000000");
			tbxT0Uncertainty.Text = Results.Time_Of_Minimum_Uncertainty.ToString("0.000000");
			tbxT0.Text = Results.T0.ToString("0.000000");
			tbxTotalObs.Text = Results.NumberObservations.ToString();
			tbxIncludedObs.Text = Results.IncludedObservations.ToString() + "%";
		}
	}
}
