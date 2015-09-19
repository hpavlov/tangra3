using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MagnitudeFitter.Engine;
using Tangra.Model.Numerical;
using System.Diagnostics;
using System.Drawing.Imaging;

namespace MagnitudeFitter
{
	public partial class frmMain : Form
	{
		private List<TangraCsvExport> m_Exports = new List<TangraCsvExport>();
 
		public frmMain()
		{
			InitializeComponent();

			pictureBox.Image = new Bitmap(pictureBox.Width, pictureBox. Height, PixelFormat.Format24bppRgb);			
		}

		private void miExit_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void miOpenExports_Click(object sender, EventArgs e)
		{
			if (openFileDialog.ShowDialog(this) == DialogResult.OK)
			{
				string dir = Path.GetDirectoryName(openFileDialog.FileName);
				string extension = Path.GetExtension(openFileDialog.FileName);

				if (dir != null)
				{
					m_Exports.Clear();

					string[] eligibleFiles = Directory.GetFiles(dir, "*." + extension.TrimStart('.'), SearchOption.TopDirectoryOnly);
					foreach (string fileName in eligibleFiles)
					{
						var export = new TangraCsvExport(fileName);

						// Remove entries that have been detected in less than 90% of all frames and that have any saturated pixels detected
						int maxFrames = export.Entries.Max(x => x.MeasuredFrames);
						export.Entries.RemoveAll(x => x.SaturatedFrames > 0 || (x.MeasuredFrames - x.ExcludedFromFrames) < 0.95 * maxFrames);

						m_Exports.Add(export);
					}

					RecalculateFit();
                    RecalculateFixedColourIndexFit();
				}
			}
		}

		private void RecalculateFit()
		{
		    int totalMeasuredFrames = m_Exports.SelectMany(x => x.Entries).Select(x => x.MeasuredFrames).ToList().Median();

			List<TangraExportEntry> datapoints = m_Exports
				.SelectMany(x => x.Entries)
				.Where(x =>
					!float.IsNaN(x.APASS_Sloan_r) && Math.Abs(x.APASS_Sloan_r) > 0.00001 && !float.IsNaN(x.MedianIntensity) &&
                    !float.IsNaN(x.MedianIntensity) && !float.IsNaN(x.APASS_BV_Colour) && x.MeasuredFrames > 0.95 * totalMeasuredFrames && x.SaturatedFrames  == 0)
				.ToList();

			for (int i = 0; i < datapoints.Count; i++)
			{
				datapoints[i].InstrMag = -2.5 * Math.Log10(datapoints[i].MedianIntensity) + 32;
				datapoints[i].InstrMagErr = Math.Abs(-2.5 * Math.Log10((datapoints[i].MedianIntensity + datapoints[i].MedianIntensityError) / datapoints[i].MedianIntensity));
			}

			datapoints = datapoints.Where(x => x.InstrMagErr < 0.2).ToList();

			if (datapoints.Count < 4) return;

			double variance = 0;
			double Ka = 0;
			double Kb = 0;
			double Kc = 0;

			int MAX_ITTER = 2;
			for (int itt = 0; itt <= MAX_ITTER; itt++)
			{
				SafeMatrix A = new SafeMatrix(datapoints.Count, 3);
				SafeMatrix X = new SafeMatrix(datapoints.Count, 1);

				int idx = 0;
				for (int i = 0; i < datapoints.Count; i++)
				{
					A[idx, 0] = datapoints[i].InstrMag;
					A[idx, 1] = datapoints[i].APASS_BV_Colour;
					A[idx, 2] = 1;

					X[idx, 0] = datapoints[i].APASS_Sloan_r;

					idx++;
				}

				SafeMatrix a_T = A.Transpose();
				SafeMatrix aa = a_T * A;
				SafeMatrix aa_inv = aa.Inverse();
				SafeMatrix bx = (aa_inv * a_T) * X;

				Ka = bx[0, 0];
				Kb = bx[1, 0];
				Kc = bx[2, 0];

				double resSum = 0;
				for (int i = 0; i < datapoints.Count; i++)
				{
					double computedMag = Ka * datapoints[i].InstrMag + Kb * datapoints[i].APASS_BV_Colour + Kc;

					double diff = computedMag - datapoints[i].APASS_Sloan_r;

					resSum += diff * diff;
					datapoints[i].Residual = diff;
				}

				variance = Math.Sqrt(resSum / datapoints.Count);

				if (itt < MAX_ITTER)
					datapoints.RemoveAll(x => Math.Abs(x.Residual) > 2 * variance || Math.Abs(x.Residual) > 0.2);
			}

			Trace.WriteLine(string.Format("r' = {0} * M + {1} * (B-V) + {2} +/- {3}", Ka.ToString("0.0000"), Kb.ToString("0.0000"), Kc.ToString("0.00"), variance.ToString("0.00")));

			PlotData(datapoints, Ka, Kb, Kc);
		}

        private void RecalculateFixedColourIndexFit()
        {
            int totalMeasuredFrames = m_Exports.SelectMany(x => x.Entries).Select(x => x.MeasuredFrames).ToList().Median();

            List<TangraExportEntry> datapoints = m_Exports
                .SelectMany(x => x.Entries)
                .Where(x =>
                    !float.IsNaN(x.APASS_Sloan_r) && Math.Abs(x.APASS_Sloan_r) > 0.00001 && !float.IsNaN(x.MedianIntensity) &&
                    !float.IsNaN(x.MedianIntensity) && !float.IsNaN(x.APASS_BV_Colour) && x.MeasuredFrames > 0.95 * totalMeasuredFrames && x.SaturatedFrames == 0)
                .ToList();

            float FIXED_COLOUR_COEFF = 0.0375f;
            for (int i = 0; i < datapoints.Count; i++)
            {
                datapoints[i].InstrMag = -2.5 * Math.Log10(datapoints[i].MedianIntensity) + 32 - datapoints[i].APASS_BV_Colour * FIXED_COLOUR_COEFF;
                datapoints[i].InstrMagErr = Math.Abs(-2.5 * Math.Log10((datapoints[i].MedianIntensity + datapoints[i].MedianIntensityError) / datapoints[i].MedianIntensity));
            }

            datapoints = datapoints.Where(x => x.InstrMagErr < 0.2).ToList();

            if (datapoints.Count < 4) return;

            double variance = 0;
            double Ka = 0;
            double Kb = 0;

            int MAX_ITTER = 2;
            for (int itt = 0; itt <= MAX_ITTER; itt++)
            {
                SafeMatrix A = new SafeMatrix(datapoints.Count, 2);
                SafeMatrix X = new SafeMatrix(datapoints.Count, 1);

                int idx = 0;
                for (int i = 0; i < datapoints.Count; i++)
                {
                    A[idx, 0] = datapoints[i].InstrMag;
                    A[idx, 1] = 1;

                    X[idx, 0] = datapoints[i].APASS_Sloan_r;

                    idx++;
                }

                SafeMatrix a_T = A.Transpose();
                SafeMatrix aa = a_T * A;
                SafeMatrix aa_inv = aa.Inverse();
                SafeMatrix bx = (aa_inv * a_T) * X;

                Ka = bx[0, 0];
                Kb = bx[1, 0];

                double resSum = 0;
                for (int i = 0; i < datapoints.Count; i++)
                {
                    double computedMag = Ka * datapoints[i].InstrMag + Kb;

                    double diff = computedMag - datapoints[i].APASS_Sloan_r;

                    resSum += diff * diff;
                    datapoints[i].Residual = diff;
                }

                variance = Math.Sqrt(resSum / datapoints.Count);

                if (itt < MAX_ITTER)
                    datapoints.RemoveAll(x => Math.Abs(x.Residual) > 2 * variance || Math.Abs(x.Residual) > 0.2);
            }

            Trace.WriteLine(string.Format("r' + {3} * (B-V) +  = {0} * M + {1} +/- {2}", Ka.ToString("0.0000"), Kb.ToString("0.0000"), variance.ToString("0.00"), FIXED_COLOUR_COEFF.ToString("0.00000")));
        }


		private void PlotData(List<TangraExportEntry> datapoints, double a, double b, double c)
		{
			double MIN_X = datapoints.Min(x => x.InstrMag - x.InstrMagErr) - 0.5;
			double MAX_X = datapoints.Max(x => x.InstrMag + x.InstrMagErr) + 0.5;
			double MIN_Y = datapoints.Min(x => x.APASS_Sloan_r) - 0.5;
			double MAX_Y = datapoints.Max(x => x.APASS_Sloan_r) + 0.5;

			float xCoeff = (float)(pictureBox.Image.Width/(MAX_X - MIN_X));
			float yCoeff = (float)(pictureBox.Image.Height/(MAX_Y - MIN_Y));

			using (Graphics g = Graphics.FromImage(pictureBox.Image))
			{
				g.Clear(Color.WhiteSmoke);

				foreach (TangraExportEntry entry in datapoints)
				{
					float x = (float)((entry.InstrMag - MIN_X + b * entry.APASS_BV_Colour) * xCoeff);
					float y = (float)(pictureBox.Image.Height - (entry.APASS_Sloan_r - MIN_Y) * yCoeff);
					float dx = (float)(entry.InstrMagErr * xCoeff);
					float dy = (float)(0.05 * yCoeff);

					g.DrawLine(Pens.Blue, x - dx, y, x + dx, y);
					g.DrawLine(Pens.Blue, x, y - dy, x, y + dy);
				}

				float xx = float.NaN;
				float yy = float.NaN;
				for (double m = MIN_X; m < MAX_X; m+=0.01)
				{
					double r = m * a + c;
					float x = (float)((m - MIN_X) * xCoeff);
					float y = (float)(pictureBox.Image.Height - (r - MIN_Y) * yCoeff);

					if (!float.IsNaN(xx))
					{
						g.DrawLine(Pens.Red, x, y, xx, yy);
					}

					xx = x;
					yy = y;
				}

				g.Save();
			}

			pictureBox.Invalidate();
			pictureBox.Update();
		}

		private void frmMain_Load(object sender, EventArgs e)
		{
			pictureBox.BackColor = Color.WhiteSmoke;
			pictureBox.Invalidate();
			pictureBox.Update();
		}
	}
}
