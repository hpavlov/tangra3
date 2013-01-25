using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Astro;

namespace Tangra
{
	public partial class frmTargetPSFViewerForm : Form
	{
		private PSFFit m_PSFFit;
		private int m_Bpp;

		public frmTargetPSFViewerForm()
		{
			InitializeComponent();
		}

		public void ShowTargetPSF(PSFFit psfFit, int bpp)
		{
			m_PSFFit = psfFit;
			m_Bpp = bpp;


			picFIT.Image = new Bitmap(picFIT.Width, picFIT.Height);

			if (m_PSFFit != null)
			{
				using (Graphics g = Graphics.FromImage(picFIT.Image))
				{
					m_PSFFit.DrawGraph(g, new Rectangle(0, 0, picFIT.Image.Width, picFIT.Image.Height), m_Bpp);
					g.Save();
				}

				int side = m_PSFFit.MatrixSize * 16;
				picPixels.Width = side;
				picPixels.Height = side;
				picPixels.Image = new Bitmap(side, side);
				using (Graphics g = Graphics.FromImage(picPixels.Image))
				{
					m_PSFFit.DrawDataPixels(g, new Rectangle(0, 0, picPixels.Width, picPixels.Height), -1, Pens.Lime, m_Bpp);
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

			picFIT.Refresh();
			picPixels.Refresh();


		}

		private void btnCopy_Click(object sender, EventArgs e)
		{
			Clipboard.SetText(string.Format("Base={0}  Maximum={1}  Amplitude={2}  X={3}  Y={4}", lblI0.Text, lblIMax.Text, lblIAmp.Text, lblX.Text, lblY.Text));
		}
	}
}
