/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Tangra.Controller;
using Tangra.Helpers;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Model.Helpers;

namespace Tangra.Video.FITS
{
	public partial class frmDefinePixelMapping : Form
	{
		private Dictionary<int, float> m_Distribution = new Dictionary<int, float>();
		private Dictionary<int, float> m_DisplayData = new Dictionary<int, float>();

		private uint m_MaxBuckets;
	    private int m_MinPixelValue;
	    private int m_BZero;
	    private string m_PrefixPerc;
	    private string m_SufixPerc;
		private float m_BucketFactor = 1;
	    private int m_Prefix;
        private int m_Sufix;
	    private bool m_Logarithmic;

	    private uint[] m_MinValAdjImagePixels;

	    private int m_PosMinValCorrection;

        public int NegPixCorrection { get; private set; }

        public int BitPix { get; private set; }

        public uint MaxPixelValue { get; private set; }

	    public frmDefinePixelMapping()
		{
			InitializeComponent();
		}

        public frmDefinePixelMapping(uint[] minValAdjImagePixels, int negPixCorrection, int bZero, int minPixelValue, uint maxPixelValue, int bitPix)
		{
			InitializeComponent();

			MaxPixelValue = maxPixelValue;
		    m_MinPixelValue = minPixelValue;
		    m_BZero = bZero;
		    NegPixCorrection = negPixCorrection;
            if (m_MinPixelValue + m_BZero > 0)
                m_PosMinValCorrection = -1 * (m_MinPixelValue + m_BZero);

            m_MinValAdjImagePixels = minValAdjImagePixels;

		    if (bitPix <= 8)
		        rb8Bit.Checked = true;
		    else if (bitPix <= 12)
		        rb12Bit.Checked = true;
		    else if (bitPix <= 14)
		        rb14Bit.Checked = true;
		    else
		        rb16Bit.Checked = true;

            nudZeroPoint.Minimum = m_MinPixelValue + m_BZero;
		    nudZeroPoint.Maximum = Math.Max(0, bZero);
            nudZeroPoint.SetNUDValue(negPixCorrection);

            rbScaleLog.Checked = true;
            m_Logarithmic = true;

            ReCalculateAll();
		}

	    private void ReCalculateAll()
	    {
            PopulateDistribution(m_MinValAdjImagePixels);
            PlotData();
            DrawHistogram();       
	    }

		private void PopulateDistribution(uint[] pixels)
		{
			m_Distribution.Clear();
			m_DisplayData.Clear();

            // We base the scaling on 255 buckets for the standard range for the given BitPix
            // Limit the outside buckets to 25% on each side
            var maxValueForBitPix = BitPix.GetMaxValueForBitPix();
            m_BucketFactor = 1.0f * 255 / maxValueForBitPix;
		    m_MaxBuckets = 255 + 128;

            for (int i = -64; i < 320; i++)
            {
                m_Distribution.Add(i, 0);
                m_DisplayData.Add(i + 64, 0);
            }

		    m_Prefix = 0;
            m_Sufix = 0;

            if (BitPix <= 8)
			{
				for (int i = 0; i < pixels.Length; i++)
				{
                    AddDistributionEntry((int)pixels[i] + m_MinPixelValue + m_BZero - NegPixCorrection + m_PosMinValCorrection);
				}
			}
			else
			{
				for (int i = 0; i < pixels.Length; i++)
				{
                    int val = (int)pixels[i] + m_MinPixelValue + m_BZero - NegPixCorrection + m_PosMinValCorrection;
                    int bucket = (int)Math.Round(val * m_BucketFactor);

                    AddDistributionEntry(bucket);
				}
			}
		}

        private void AddDistributionEntry(int index)
        {
            if (index < 0) m_Prefix++;
            if (index > 255) m_Sufix++;

	        if (m_Distribution.ContainsKey(index))
                m_Distribution[index]++;

	    }

		private void PlotData()
		{
			for (int i = -64; i < 320; i++)
			{
			    var distVal = m_Distribution[i];
                m_DisplayData[i + 64] = m_Logarithmic ? (float)Math.Log10(distVal + 1) : distVal;
			}

		    string FORMAT = "0.000";

            m_PrefixPerc = (100.0 * m_Prefix / m_MinValAdjImagePixels.Length).ToString(FORMAT);
            m_SufixPerc = (100.0 * m_Sufix / m_MinValAdjImagePixels.Length).ToString(FORMAT);

            if (m_PrefixPerc == FORMAT)
		        m_PrefixPerc = "";
		    else m_PrefixPerc += "%";

            if (m_SufixPerc == FORMAT)
                m_SufixPerc = "";
            else m_SufixPerc += "%";
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

				g.FillRectangle(Brushes.LightGray, new Rectangle(0, 0, picHistogram.Image.Width, picHistogram.Image.Height));
				g.DrawRectangle(Pens.Black, XGAP, YGAP, picHistogram.Image.Width - 2 * XGAP + 1, picHistogram.Image.Height - 2 * YGAP);
                g.FillRectangle(Brushes.Honeydew, new Rectangle(XGAP + (int)(xScale * 64), YGAP + 1, picHistogram.Image.Width - 2 * (XGAP + (int)(xScale * 64)) + 1, picHistogram.Image.Height - 2 * YGAP - 2));

				foreach (byte key in m_DisplayData.Keys)
				{
					float xFrom = XGAP + key * xScale + 1;
					float xSize = Math.Max(0.5f, xScale);

					float ySize = m_DisplayData[key] * yScale;
					float yFrom = picHistogram.Image.Height - YGAP - ySize;

					g.FillRectangle(Brushes.CornflowerBlue, xFrom, yFrom, xSize, ySize);
				}

			    if (!string.IsNullOrWhiteSpace(m_PrefixPerc))
			    {
                    var sizePref = g.MeasureString(m_PrefixPerc, s_DetailsFont);
                    g.DrawString(m_PrefixPerc, s_DetailsFont, Brushes.DarkRed, XGAP + (int)(xScale * 32) - sizePref.Width / 2, picHistogram.Image.Height / 2.0f - sizePref.Height / 2);
			    }

                if (!string.IsNullOrWhiteSpace(m_SufixPerc))
                {
                    var sizeSuff = g.MeasureString(m_SufixPerc, s_DetailsFont);
                    g.DrawString(m_SufixPerc, s_DetailsFont, Brushes.DarkRed, picHistogram.Image.Width - XGAP - (int)(xScale * 32) - sizeSuff.Width / 2, picHistogram.Image.Height / 2.0f - sizeSuff.Height / 2);
                }

				g.Save();
			}

			picHistogram.Refresh();
		}

	    private static Font s_DetailsFont = new Font(FontFamily.GenericSansSerif, 7);

        private void btnOK_Click(object sender, EventArgs e)
        {
            MaxPixelValue = (uint) (m_MinValAdjImagePixels.Max() + m_MinPixelValue - NegPixCorrection);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void rb8Bit_CheckedChanged(object sender, EventArgs e)
        {
            BitPix = 8;
            ReCalculateAll();
        }

        private void rb12Bit_CheckedChanged(object sender, EventArgs e)
        {
            BitPix = 12;
            ReCalculateAll();
        }

        private void rb14Bit_CheckedChanged(object sender, EventArgs e)
        {
            BitPix = 14;
            ReCalculateAll();
        }

        private void rb16Bit_CheckedChanged(object sender, EventArgs e)
        {
            BitPix = 16;
            ReCalculateAll();
        }

        private void nudZeroPoint_ValueChanged(object sender, EventArgs e)
        {
            NegPixCorrection = (int)nudZeroPoint.Value + m_PosMinValCorrection;
            
            ReCalculateAll();
        }

        private void btnMinValue_Click(object sender, EventArgs e)
        {
            nudZeroPoint.SetNUDValue(m_MinPixelValue);
        }

        private void rbScaleLinear_CheckedChanged(object sender, EventArgs e)
        {
            m_Logarithmic = rbScaleLog.Checked;
            ReCalculateAll();
        }
	}
}
