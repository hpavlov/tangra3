/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Numerical;

namespace Tangra.VideoOperations.LightCurves.InfoForms
{
    /// <summary>
    /// For a more sufisticated signal distribution viewer see
    /// 
    /// http://mathworld.wolfram.com/NonlinearLeastSquaresFitting.html
    ///  
    /// to determine the min and max positions on the X axis use 90% of the bins with values + 5 bins i.e. remove the very distance positive 
    /// or negative values The current 2 dimmentional PSF fit probably uses the same method (find its name from the Numerical Recepies, it's probably what 
    /// 
    /// Frank said he is using) but with one extra partial derivative. I will need the same method for sigmoid fitting.
    /// 
    /// Find average and 3 Sigma. Then use a windows of +/- 6 sigma. Then devide this into 100-150 bins and compute the dela X for a bin 
    /// 
    /// (bin width). Allow the user to chnage this. Then compute all bins and do a non linear gaussian fit. Display the data
    /// 
    /// </summary>
    public partial class frmNoiseDistribution : Form
    {
        private LCMeasurementHeader m_Header;
		private List<List<LCMeasurement>> m_AllReadings;
        private List<frmLightCurve.BinnedValue>[] m_AllBinnedReadings;
        private Brush[] m_AllBrushes;
        private Color[] m_AllColors;
    	private Color m_BackgroundColor;

        private int m_SigmaRegionToInclude = 7;
        private int m_NoOfBins = 25;
        private double m_Median;
        private double m_Variance;

        internal frmNoiseDistribution(
            LCMeasurementHeader header,
            List<List<LCMeasurement>> allReadings,
            List<frmLightCurve.BinnedValue>[] allBinnedReadings,
            Brush[] allBrushes,
            Color[] allColors,
			Color background)
        {
            InitializeComponent();

            m_Header = header;
			m_AllReadings = allReadings;
            m_AllBinnedReadings = allBinnedReadings;
            m_AllBrushes = allBrushes;
            m_AllColors = allColors;
        	m_BackgroundColor = background;
        	
            pictureBox.Image = new Bitmap(pictureBox.Width, pictureBox.Height);

            pb1.BackColor = m_AllColors[0];
            pb2.BackColor = m_AllColors[1];
            pb3.BackColor = m_AllColors[2];
            pb4.BackColor = m_AllColors[3];

            rbTarget1.Enabled = header.ObjectCount > 0;
            rbTarget2.Enabled = header.ObjectCount > 1;
            rbTarget3.Enabled = header.ObjectCount > 2;
            rbTarget4.Enabled = header.ObjectCount > 3;

            if (header.ObjectCount > 0)
                DisplayDistributionForObject(0);
        }
        private void DisplayDistributionForObject(int objectIndex)
        {
			if (m_AllReadings[objectIndex] != null)
			{
				List<double> data = m_AllReadings[objectIndex]
					.Where(mea => mea.TotalReading != 0 && mea.TotalBackground != 0)
					.Select(d => (double)d.AdjustedReading).ToList();

				DisplayDistributionFullProcess(data, m_AllBrushes[objectIndex]);				
			}
        }

        private void DisplayDistributionFullProcess(List<double> data, Brush brush)
        {
            data.Sort();
            int nintyFivePercIdx = data.Count/20;
            double fromVal = 1.5 * data[nintyFivePercIdx];
            double toVal = 1.5 * data[data.Count - 1 - nintyFivePercIdx];
            m_Median = data.Count % 2 == 1
                ? data[data.Count / 2]
                : (data[data.Count / 2] + data[(data.Count / 2) - 1]) / 2;

            m_Variance = 0;
            foreach(double val in data)
            {
                m_Variance += (m_Median - val) * (m_Median - val);
            }
            m_Variance = Math.Sqrt(m_Variance / (data.Count - 1));

            DisplayDistribution(data, brush);
        }

        private void DisplayDistribution(List<double> data, Brush brush)
        {
            double fromVal = m_Median - m_SigmaRegionToInclude * m_Variance;
            double toVal = m_Median + m_SigmaRegionToInclude * m_Variance;

            double binWidth = (toVal - fromVal) / m_NoOfBins;
            int halfBinsNo = m_NoOfBins / 2;
            Dictionary<int, int> dic = new Dictionary<int, int>();
            for (int j = -halfBinsNo; j <= halfBinsNo; j++)
            {
                dic.Add(j, 0);
            }

            List<double> residuals = new List<double>();
            foreach (double mea in data)
            {
                double res = m_Median - mea;
                residuals.Add(res);
                double val = res/binWidth;

                int idx = (int)Math.Round(val);
                if (idx >= -halfBinsNo && idx <= halfBinsNo) dic[idx] += 1;
            }

            DrawDistribution(dic, brush);
        }

        private void DrawDistribution(Dictionary<int, int> dic, Brush brush)
        {
            GaussianFit fit = new GaussianFit();
            foreach (int key in dic.Keys)
            {
                if (dic[key] != 0)
                    fit.AddPoint(key, dic[key]);
            }
            fit.Solve();

            int maxY = dic.Values.Max();
            int minX = dic.Keys.Min();
            int maxX = dic.Keys.Max();
            int x0 = 10;
            int y0 = 10;

            float scaleX = (pictureBox.Width - 2 * x0) * 1.0f / (maxX - minX);
            float scaleY = (pictureBox.Height - 2 * y0) * 1.0f / maxY;

            if (maxY != 0 && maxX != minX)
            {
                using (Graphics g = Graphics.FromImage(pictureBox.Image))
                {
					g.Clear(m_BackgroundColor);

                    float halfBarWidth = (pictureBox.Width - 2 * x0) / (2.0f * dic.Keys.Count);

                    foreach (int x in dic.Keys)
                    {
                        int y = dic[x];
                        float xx = x0 + (x - minX) * scaleX;
                        float yy = pictureBox.Height - y0 - y * scaleY;

                        g.FillRectangle(brush, xx - halfBarWidth, yy + 1, 2 * halfBarWidth, pictureBox.Height - y0 - yy);
                    }

                    g.Save();
                }
                
            }
            
            pictureBox.Refresh();
        }

        private void TargetChecked(object sender, EventArgs e)
        {
            ProcessCuerrentTarget();
        }

        private void ProcessCuerrentTarget()
        {
            if (rbTarget1.Checked)
                DisplayDistributionForObject(0);
            else if (rbTarget2.Checked)
                DisplayDistributionForObject(1);
            else if (rbTarget3.Checked)
                DisplayDistributionForObject(2);
            else if (rbTarget4.Checked)
                DisplayDistributionForObject(3);       
        }

        private void nudSigmaRange_ValueChanged(object sender, EventArgs e)
        {
            m_SigmaRegionToInclude = (int)nudSigmaRange.Value;
            ProcessCuerrentTarget();
        }

        private void nudNumberBins_ValueChanged(object sender, EventArgs e)
        {
            m_NoOfBins = (int)nudNumberBins.Value;
            ProcessCuerrentTarget();
        }
    }
}
