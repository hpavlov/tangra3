using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Astro;
using Tangra.Model.Image;

namespace Tangra
{
	public partial class frmTargetPSFViewerForm : Form
	{
		private PSFFit m_PSFFit;
		private int m_Bpp;
		private uint m_NormVal;

		public frmTargetPSFViewerForm()
		{
			InitializeComponent();
		}

		public void ShowTargetPSF(PSFFit psfFit, int bpp, uint aav16Normval, uint[,] backgroundPixels)
		{
			m_PSFFit = psfFit;
			m_Bpp = bpp;
			m_NormVal = bpp == 16 ? aav16Normval : 0;

			picFIT.Image = new Bitmap(picFIT.Width, picFIT.Height);

			if (m_PSFFit != null)
			{
				using (Graphics g = Graphics.FromImage(picFIT.Image))
				{
					m_PSFFit.DrawGraph(g, new Rectangle(0, 0, picFIT.Image.Width, picFIT.Image.Height), m_Bpp, m_NormVal);
					g.Save();
				}

				int side = m_PSFFit.MatrixSize * 16;
				picPixels.Width = side;
				picPixels.Height = side;
				picPixels.Image = new Bitmap(side, side);
				using (Graphics g = Graphics.FromImage(picPixels.Image))
				{
					m_PSFFit.DrawDataPixels(g, new Rectangle(0, 0, picPixels.Width, picPixels.Height), -1, Pens.Lime, m_Bpp, m_NormVal);
					g.Save();
				}


				lblI0.Text = m_PSFFit.I0.ToString("0.0");
				lblIMax.Text = m_PSFFit.IMax.ToString("0.0");
				lblIAmp.Text = (m_PSFFit.IMax - m_PSFFit.I0).ToString("0.0");
				lblX.Text = m_PSFFit.XCenter.ToString("0.0");
				lblY.Text = m_PSFFit.YCenter.ToString("0.0");						
			}
			else
			{
				using (Graphics g = Graphics.FromImage(picFIT.Image))
				{
					g.Clear(Color.DimGray);
					g.Save();
				}

				picPixels.Image = new Bitmap(16 * 17, 16 * 17);
				using (Graphics g = Graphics.FromImage(picPixels.Image))
				{
					g.Clear(Color.DimGray);
					g.Save();
				}

				lblI0.Text = "N/A";
				lblIMax.Text = "N/A";
				lblIAmp.Text = "N/A";
				lblY.Text = "N/A";
				lblX.Text = "N/A";
			}

			CalculateAndDisplayBackground(backgroundPixels);

			picFIT.Refresh();
			picPixels.Refresh();
		}

		private void CalculateAndDisplayBackground(uint[,] backgroundPixels)
		{
			if (backgroundPixels != null)
			{
				int bgWidth = backgroundPixels.GetLength(0);
				int bgHeight = backgroundPixels.GetLength(1);

				var bgPixels = new List<uint>();

				for(int x = 0; x < bgWidth; x++)
				for(int y = 0; y < bgHeight; y++)
				{
					if (m_PSFFit == null || !m_PSFFit.IsSolved ||
						ImagePixel.ComputeDistance(m_PSFFit.XCenter, x + bgWidth - m_PSFFit.MatrixSize, m_PSFFit.YCenter, y + bgHeight - m_PSFFit.MatrixSize) > 3 * m_PSFFit.FWHM)
					{
						bgPixels.Add(backgroundPixels[x, y]);
					}
				}

				bgPixels.Sort();
				double background = m_Bpp < 12 
					? bgPixels[bgPixels.Count / 2] // for 8 bit videos Median background works better
					: bgPixels.Average(x => x); // for 12+bit videos average background works better

				double residualsSquareSum = 0;
				foreach (uint bgPixel in bgPixels)
				{
					residualsSquareSum += (background - bgPixel) * (background - bgPixel);
				}
				double noise = Math.Sqrt(residualsSquareSum / (bgPixels.Count - 1));
				
				lblBackground.Text = background.ToString("0.0");
				lblNoise.Text = noise.ToString("0.0");

				if (m_PSFFit != null && noise != 0)
				{
					double snr = (m_PSFFit.IMax - m_PSFFit.I0) / noise;
					lblSNR.Text = snr.ToString("0.0");
				}
				else
					lblSNR.Text = "N/A";	
			
				if (m_PSFFit != null)
				{
					lblFitVariance.Text = m_PSFFit.GetVariance().ToString("0.0");
				}
				else
					lblFitVariance.Text = "N/A";
			}
			else
			{
				lblBackground.Text = "N/A";
				lblNoise.Text = "N/A";
				lblSNR.Text = "N/A";
			}
		}

		private void btnCopy_Click(object sender, EventArgs e)
		{
			Clipboard.SetText(string.Format("Base={0}  Maximum={1}  Amplitude={2}  X={3}  Y={4}\r\nBackground={5} 1-sigma Noise={6} SNR={7}", lblI0.Text, lblIMax.Text, lblIAmp.Text, lblX.Text, lblY.Text, lblBackground.Text, lblNoise.Text, lblSNR.Text));
		}
	}
}
