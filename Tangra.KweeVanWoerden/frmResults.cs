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
using Tangra.SDK;

namespace Tangra.KweeVanWoerden
{
	public partial class frmResults : Form
	{
		internal KweeVanWoerdenMinimum.KweeVanWoerdenResult Results;
	    internal ITangraHost TangraHost;

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
            tbxUncertaintyInSec.Text = (Results.Time_Of_Minimum_Uncertainty * 86400.0).ToString("0.0");

		    picGraph.Image = new Bitmap(picGraph.Width, picGraph.Height);
            using (Graphics g = Graphics.FromImage(picGraph.Image))
            {
                g.Clear(SystemColors.ControlDark);
                float xScale = picGraph.Width * 1.0f / Results.Buckets.Count;
                float yScale = picGraph.Height * 1.0f / (float)Results.Buckets.Max();

                float xScaleSQ = picGraph.Width * 1.0f / Results.Sum_Of_Squares_Count.Count;
                float yScaleSQ = picGraph.Height * 1.0f / (float)Results.Sum_Of_Squares_Count.Max();

                for (int i = 0; i < Results.Buckets.Count - 1; i++)
                {
                    Brush brush = i < Results.Start_Light_Curve || i > Results.Stop_Light_Curve ? Brushes.DarkBlue : Brushes.Aqua;
                    g.FillRectangle(brush, xScale * i, yScale * (float)Results.Buckets[i], xScale, picGraph.Height - 2 - yScale * (float)Results.Buckets[i]);

                    if (i > Results.Start_Light_Curve && i < Results.Stop_Light_Curve)
                    {
                        g.FillRectangle(Brushes.OrangeRed, xScaleSQ * i, yScaleSQ * (float)Results.Sum_Of_Squares_Count[i], xScaleSQ, 4);                        
                    }
                }
                g.Save();
            }

		    picGraph.Refresh();
		}

        private void btnCalcHJD_Click(object sender, EventArgs e)
        {
            var frm = new frmHJDCalculation();
            frm.TimeOfMinimumJD = Results.Time_Of_Minimum_JD;
            frm.ShowDialog(TangraHost.ParentWindow);
        }
	}
}
