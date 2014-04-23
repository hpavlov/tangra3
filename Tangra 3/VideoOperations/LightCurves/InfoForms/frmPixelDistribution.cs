using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Image;

namespace Tangra.VideoOperations.LightCurves.InfoForms
{
    public partial class frmPixelDistribution : Form
    {
        private List<List<LCMeasurement>> m_AllReadings;
		private Dictionary<uint, float> m_Distribution = new Dictionary<uint, float>();
		private Dictionary<uint, float> m_DisplayData = new Dictionary<uint, float>();
        private List<uint> m_allBgValues = new List<uint>();

    	private uint m_MaxPixelValue = 0;
		private uint m_MaxBuckets = 255;
    	private int m_Bpp = 8;

    	private float m_BucketFactor = 1;

        internal frmPixelDistribution(List<List<LCMeasurement>> readings, uint maxPixelValue, int bpp)
        {
            InitializeComponent();

            m_AllReadings = readings;

        	m_MaxPixelValue = maxPixelValue;
        	m_Bpp = bpp;

            PopulateDistributionFromLCMeasurements();
            PlotData();

            Text = "Measurement Areas Combined Histogram";
        }

        internal frmPixelDistribution(List<Pixelmap> allFrames, int firstFrame, int lastFrame)
        {
            InitializeComponent();

			if (allFrames[0] != null)
			{
				m_MaxPixelValue = allFrames[0].MaxPixelValue;
				m_Bpp = allFrames[0].BitPixCamera;

				PopulateDistributionFromBitmaps(allFrames);
			}

            PlotData();

            Text = string.Format("Histogram of All Pixels from Frames {0} - {1}", firstFrame, lastFrame);
        }

		private void PopulateDistributionFromBitmaps(List<Pixelmap> allFrames)
        {
            m_Distribution.Clear();
            m_DisplayData.Clear();
		    m_allBgValues.Clear();

			int sum = 0;

			if (m_Bpp == 8)
			{
				for (uint i = 0; i <= m_MaxPixelValue; i++)
				{
					m_Distribution.Add(i, 0);
					m_DisplayData.Add(i, 0);
				}

				foreach (Pixelmap image	 in allFrames)
				{
					if (image != null)
					{
						for (int y = 0; y < image.Height; ++y)
						{
							for (int x = 0; x < image.Width; ++x)
							{
								uint val = image[x, y];
								m_Distribution[val]++;
								sum++;

								m_allBgValues.Add(val);
							}
						}						
					}
				}				
			}
			else if (m_Bpp == 12 || m_Bpp == 14)
			{

				for (uint i = 0; i <= m_MaxBuckets; i++)
				{
					m_Distribution.Add(i, 0);
					m_DisplayData.Add(i, 0);
				}

				m_BucketFactor = 1.0f * m_MaxBuckets / m_MaxPixelValue;
				foreach (Pixelmap image in allFrames)
				{
					if (image != null)
					{
						for (int y = 0; y < image.Height; ++y)
						{
							for (int x = 0; x < image.Width; ++x)
							{
								uint val = image[x, y];
								uint bucket = Math.Max(0, Math.Min(m_MaxBuckets - 1, (uint) Math.Round(val*m_BucketFactor)));

								m_Distribution[bucket]++;
								sum++;

								m_allBgValues.Add(val);
							}
						}
					}
				}				
			}

			lblPixCount.Text = string.Format("{0} million", (sum / 1000000.0).ToString("0.00"));
		    ComputeAndDisplayMedianAndSigma();
        }

        private void PopulateDistributionFromLCMeasurements()
        {
            m_Distribution.Clear();
            m_DisplayData.Clear();
            m_allBgValues.Clear();

            int sum = 0;

            if (m_Bpp == 8)
            {
                for (uint i = 0; i <= m_MaxPixelValue; i++)
                {
                    m_Distribution.Add(i, 0);
                    m_DisplayData.Add(i, 0);
                }

                for (int i = 0; i < m_AllReadings.Count; i++)
                {
                    foreach (LCMeasurement mea in m_AllReadings[i])
                    {
                        int width = mea.PixelData.GetLength(0);
                        int height = mea.PixelData.GetLength(1);

                        for (int x = 0; x < width; x++)
                        {
                            for (int y = 0; y < height; y++)
                            {
                                sum++;

                                m_Distribution[mea.PixelData[x, y]]++;
                                m_allBgValues.Add(mea.PixelData[x, y]);
                            }
                        }
                    }
                }
            }
            else if (m_Bpp == 12 || m_Bpp == 14)
            {

                for (uint i = 0; i <= m_MaxBuckets; i++)
                {
                    m_Distribution.Add(i, 0);
                    m_DisplayData.Add(i, 0);
                }

                m_BucketFactor = 1.0f*m_MaxBuckets/m_MaxPixelValue;

                for (int i = 0; i < m_AllReadings.Count; i++)
                {
                    foreach (LCMeasurement mea in m_AllReadings[i])
                    {
                        int width = mea.PixelData.GetLength(0);
                        int height = mea.PixelData.GetLength(1);

                        for (int x = 0; x < width; x++)
                        {
                            for (int y = 0; y < height; y++)
                            {
                                uint val = mea.PixelData[x, y];
                                uint bucket = Math.Max(0,
                                                       Math.Min(m_MaxBuckets - 1, (uint) Math.Round(val*m_BucketFactor)));

                                m_Distribution[bucket]++;
                                sum++;

                                m_allBgValues.Add(val);
                            }
                        }
                    }
                }
            }

            lblPixCount.Text = string.Format("{0} million", (sum/1000000.0).ToString("0.00"));
            ComputeAndDisplayMedianAndSigma();
        }

        private void ComputeAndDisplayMedianAndSigma()
        {
            m_allBgValues.Sort();

            uint median = m_allBgValues[m_allBgValues.Count / 2];
            var allBgResiduals = new List<int>();
            double residualsSum = 0;
            double sigma;
            foreach (uint bgValue in m_allBgValues)
            {
                int residual = Math.Abs((int)bgValue - (int)median);
                residualsSum += residual * residual;
                allBgResiduals.Add(residual);
            }
            sigma = Math.Sqrt(residualsSum / (allBgResiduals.Count - 1));
            allBgResiduals.RemoveAll(v => v > 3 * sigma);
            residualsSum = 0;
            foreach (uint bgResidual in allBgResiduals)
            {
                residualsSum += bgResidual * bgResidual;
            }
            sigma = Math.Sqrt(residualsSum / (allBgResiduals.Count - 1));

            lblMedianBackground.Text = median.ToString();
            lblBackgroundSigma.Text = string.Format("{0:0.0}", sigma);            
        }

        private void PlotData()
        {
            for (uint i = 0; i <= m_MaxBuckets; i++)
            {
                if (rbScaleLog.Checked)
                    m_DisplayData[i] = (float)Math.Log10(m_Distribution[i] + 1);
                else
                    m_DisplayData[i] = m_Distribution[i];
            }    
        }

        private void frmPixelDistribution_Load(object sender, EventArgs e)
        {
            DrawHistogram();
        }

        private void DrawHistogram()
        {
            if (picHistogram.Image != null)
                picHistogram.Image.Dispose();

            picHistogram.Image = new Bitmap(picHistogram.Width, picHistogram.Height);

            float maxVal = m_DisplayData.Values.Max();
            int XGAP = 10;
            int YGAP = 10;

            using (Graphics g = Graphics.FromImage(picHistogram.Image))
            {
                float xScale = (picHistogram.Image.Width - 2 * XGAP) * 1.0f / (m_MaxBuckets + 1);
                float yScale = (picHistogram.Image.Height - 2 * YGAP) * 1.0f / maxVal;

                g.FillRectangle(SystemBrushes.ControlDark,
                                new Rectangle(0, 0, picHistogram.Image.Width, picHistogram.Image.Height));
                g.DrawRectangle(Pens.Black, XGAP, YGAP, picHistogram.Image.Width - 2 * XGAP + 1, picHistogram.Image.Height - 2 * YGAP);

                foreach (byte key in m_DisplayData.Keys)
                {
                    float xFrom = XGAP + key * xScale + 1;
                    float xSize = Math.Max(0.5f, xScale);
					//uint bucket = Math.Max(0, Math.Min(m_MaxBuckets - 1, (uint)Math.Round(key * m_BucketFactor)));

					float ySize = m_DisplayData[key] * yScale;
					float yFrom = picHistogram.Image.Height - YGAP - ySize;
					
                    g.FillRectangle(Brushes.LimeGreen, xFrom, yFrom, xSize, ySize);
                }

                g.Save();
            }

            picHistogram.Refresh();
        }

        private void picHistogram_Resize(object sender, EventArgs e)
        {
            DrawHistogram();
        }

        private void rbScaleLinear_CheckedChanged(object sender, EventArgs e)
        {
            PlotData();
            DrawHistogram();
        }
    }
}
