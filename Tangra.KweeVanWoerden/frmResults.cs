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
		internal KweeVanWoerdenMinimum.PolynomialFitResult PolyResults;
	    internal ITangraHost TangraHost;

		public frmResults()
		{
			InitializeComponent();
		}

        public override object InitializeLifetimeService()
        {
            // The lifetime of the object is managed by the add-in
            return null;
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
		    tbxT0UT.Text =  AstroUtilities.JDToDateTimeUtc(Results.Time_Of_Minimum_JD).ToString("dd MMM yyyy, HH:mm:ss.fff");
			tbxT0Uncertainty.Text = Results.Time_Of_Minimum_Uncertainty.ToString("0.000000");
			tbxT0.Text = Results.T0.ToString("0.000000");
			tbxTotalObs.Text = Results.NumberObservations.ToString();
			tbxIncludedObs.Text = Results.IncludedObservations.ToString() + "%";
            tbxUncertaintyInSec.Text = (Results.Time_Of_Minimum_Uncertainty * 86400.0).ToString("0.0");

		    PlotKweeVanWoerden();


            tbxT0JD_CF.Text = PolyResults.Time_Of_Minimum_JD.ToString("0.000000");
            tbxT0UT_CF.Text = AstroUtilities.JDToDateTimeUtc(PolyResults.Time_Of_Minimum_JD).ToString("dd MMM yyyy, HH:mm:ss.fff");

			nudM0.Value = (decimal)PolyResults.M0;
			nudC.Value = (decimal)PolyResults.C;
			nudD.Value = (decimal)PolyResults.D;
			nudG.Value = (decimal)PolyResults.G;

		    PlotPolyFit();
		}

        private void PlotPolyFit()
        {
            picGraphPoly.Image = new Bitmap(picGraphPoly.Width, picGraphPoly.Height);
            using (Graphics g = Graphics.FromImage(picGraphPoly.Image))
            {
                g.Clear(SystemColors.ControlDark);

                float xScale = picGraphPoly.Width * 1.0f / PolyResults.DataPoints.Count;
                double maxVal = PolyResults.DataPoints.Max();
                double minVal = PolyResults.DataPoints.Min();

                double maxPoly = PolyResults.FittedValues.Where(x => !double.IsNaN(x)).Max();
                double minPoly = PolyResults.FittedValues.Where(x => !double.IsNaN(x)).Min();
                float polyCoeff = (float)((maxVal - minVal) / (maxPoly - minPoly));

                float yScale = (float)((picGraphPoly.Height - 2) * 1.0f / (maxVal - minVal));

                for (int i = 0; i < PolyResults.FittedValues.Count - 1; i++)
                {
                    if (!double.IsNaN(PolyResults.FittedValues[i]))
                    {
                        g.FillRectangle(Brushes.OrangeRed, xScale * i, picGraphPoly.Height - 2 - yScale * polyCoeff * (float)(PolyResults.FittedValues[i] - minPoly), xScale, 2);

                        if (i >= PolyResults.StartIndex && i < PolyResults.StopIndex)
                            g.FillRectangle(Brushes.Yellow, xScale * i, picGraphPoly.Height - 2 - yScale * polyCoeff * (float)(ComputeModelValue(PolyResults.TimePoints[i - PolyResults.StartIndex]) - minPoly), xScale, 2);
                    }

                    g.FillRectangle(Brushes.Aqua, xScale * i, picGraphPoly.Height - 2 - yScale * (float)(PolyResults.DataPoints[i] - minVal), xScale, 2);     
                }

                g.Save();
            }

            picGraphPoly.Refresh();
        }

		private double ComputeModelValue(double t)
		{
			double magVal = PolyResults.M0 + PolyResults.C * (1 - Math.Pow(1 - Math.Exp(1 - Math.Cosh((t - PolyResults.T0) / PolyResults.D)), PolyResults.G));
			return Math.Pow(10, (10 - magVal)/2.5);
		}

        private void PlotKweeVanWoerden()
        {
            picGraph.Image = new Bitmap(picGraph.Width, picGraph.Height);
            using (Graphics g = Graphics.FromImage(picGraph.Image))
            {
                g.Clear(SystemColors.ControlDark);
                float xScale = picGraph.Width * 1.0f / Results.Buckets.Count;
                float yScale = picGraph.Height * 1.0f / (float)Results.Buckets.Max();

                float xScaleSQ = picGraph.Width * 1.0f / Results.Sum_Of_Squares_Mean.Count;
                float yScaleSQ = picGraph.Height * 1.0f / (float)Results.Sum_Of_Squares_Mean.Max();

                for (int i = 0; i < Results.Buckets.Count - 1; i++)
                {
                    Brush brush = i < Results.Start_Light_Curve || i > Results.Stop_Light_Curve ? Brushes.DarkBlue : Brushes.Aqua;
                    g.FillRectangle(i == Results.Sum_Of_Squares_Smallest_Index ? Brushes.GreenYellow : brush, xScale * i, yScale * (float)Results.Buckets[i], xScale, picGraph.Height - 2 - yScale * (float)Results.Buckets[i]);

                    if (i > Results.Start_Light_Curve && i < Results.Stop_Light_Curve)
                    {
                        g.FillRectangle(Brushes.OrangeRed, xScaleSQ * i, yScaleSQ * (float)Results.Sum_Of_Squares_Mean[i], xScaleSQ, 4);
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
            frm.ShowDialog(this);
            if (!double.IsNaN(frm.TimeOfMinimumHJD))
            {
                tbxT0HJD.Text = frm.TimeOfMinimumHJD.ToString();
                tbxT0HJD_CF.Text = (frm.TimeCorrectionHJD + PolyResults.Time_Of_Minimum_JD).ToString();

                btnCalcHJD.Visible = false;
                btnCalcHJD2.Visible = false;
            }
        }

		private void button2_Click(object sender, EventArgs e)
		{
			PolyResults.M0 = (double)nudM0.Value;
			PolyResults.C = (double)nudC.Value;
			PolyResults.D = (double)nudD.Value;
			PolyResults.G = (double)nudG.Value;

			PlotPolyFit();
		}
	}
}
