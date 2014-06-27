using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Numerical;

namespace Tangra.VideoTools
{
	internal class BackgroundModelGenerator
	{
		private int m_X0;
		private int m_Y0;
		private int m_Radius;

		internal BackgroundModelGenerator(int x0, int y0, int radius)
		{
			m_X0 = x0;
			m_Y0 = y0;
			m_Radius = radius;
		}

		internal double A;
		internal double B;
		internal double C;
		internal double D;
		internal double E;
		internal double F;
		internal double G;
		internal double H;
		internal double I;
		internal double J;

		internal int[,] GenerateBackground(int order, int frequency, double shift, double framesDone, int x0, int y0, int side)
		{
			int[,] rv = new int[300, 200];
			double deltaX = -shift;
			double step = 2 * shift / frequency;

			for (int x = 0; x < framesDone - 1; x++)
			{
				if ((x / frequency) % 2 == 0)
					deltaX += step;
				else
					deltaX -= step;
			}
			Trace.WriteLine(string.Format("framesDone:{0}, deltaX = {1}", framesDone, deltaX));

			for (int x = 0; x < 300; x++)
				for (int y = 0; y < 200; y++)
				{
					if (x > x0 - side && x < x0 + side && y > y0 - side && y < y0 + side)
					{
						if (order == 1)
						{
							rv[x, y] = (int)(A * (x + deltaX) + B * y + C);
						}
						else if (order == 2)
						{
							rv[x, y] = (int)(A * (x + deltaX) * (x + deltaX) + B * (x + deltaX) * y + C * y * y + D * (x + deltaX) + E * y + F);
						}
						else if (order == 3)
						{
							rv[x, y] = (int)(
								A * (x + deltaX) * (x + deltaX) * (x + deltaX) +
								B * (x + deltaX) * (x + deltaX) * y +
								C * y * y * (x + deltaX) +
								D * y * y * y +
								E * (x + deltaX) * (x + deltaX) + F * (x + deltaX) * y + G * y * y + H * (x + deltaX) + I * y + J);
						}
					}
				}

			return rv;
		}

		internal void GenerateBackgroundModelParameters(int order, double depth)
		{
			Random rnd = new Random((int)DateTime.Now.Ticks);

			if (order == 1)
			{
				// z = ax + by + c

				int dist = rnd.Next(m_X0, m_X0 + (int)(1.2 * m_Radius));
				int d2 = rnd.Next((int)depth / 2, (int)depth);

				SafeMatrix A = new SafeMatrix(3, 3);
				SafeMatrix X = new SafeMatrix(3, 1);

				A[0, 0] = m_X0; A[0, 1] = m_Y0; A[0, 2] = 1; X[0, 0] = depth;
				A[1, 0] = m_X0 + dist / 2; A[1, 1] = m_Y0 + dist / 3; A[1, 2] = 1; X[1, 0] = d2;
				A[2, 0] = m_X0 + dist; A[2, 1] = m_Y0 + dist / 2; A[2, 2] = 1; X[2, 0] = 0;

				SafeMatrix a_T = A.Transpose();
				SafeMatrix aa = a_T * A;
				SafeMatrix aa_inv = aa.Inverse();
				SafeMatrix bx = (aa_inv * a_T) * X;

				this.A = bx[0, 0];
				B = bx[1, 0];
				C = bx[2, 0];
			}
			else if (order == 2)
			{
				// z = axx + bxy + cyy + dx + ey + f

				int[] xArr = new int[6];
				int[] yArr = new int[6];
				double[] zArr = new double[6];

				xArr[0] = m_X0; yArr[0] = m_Y0; zArr[0] = depth;
				xArr[1] = m_X0 + m_Radius; yArr[1] = m_Y0 + m_Radius / 3; zArr[1] = 0;
				for (int i = 2; i < 6; i++)
				{
					xArr[i] = rnd.Next(m_X0, m_X0 + (int)(1.2 * m_Radius));
					yArr[i] = rnd.Next(m_Y0, m_Y0 + (int)(0.6 * m_Radius));
					zArr[i] = rnd.Next(0, (int)depth);
				}

				// Start with an approximation
				// z = axx +     + cyy +         + f

				SafeMatrix A = new SafeMatrix(3, 3);
				SafeMatrix X = new SafeMatrix(3, 1);

				for (int i = 0; i < 3; i++)
				{
					A[i, 0] = xArr[i] * xArr[i];
					A[i, 1] = yArr[i] * yArr[i];
					A[i, 2] = 1;
					X[i, 0] = zArr[i];
				}

				SafeMatrix a_T = A.Transpose();
				SafeMatrix aa = a_T * A;
				SafeMatrix aa_inv = aa.Inverse();
				SafeMatrix bx = (aa_inv * a_T) * X;

				this.A = bx[0, 0];
				C = bx[1, 0];
				F = bx[2, 0];

				B = 0;
				D = 0;
				E = 0;

				/*
				A = new SafeMatrix(6, 6);
				X = new SafeMatrix(6, 1);

				for (int i = 0; i < 6; i++)
				{
					A[i, 0] = xArr[i] * xArr[i];
					A[i, 1] = xArr[i] * yArr[i];
					A[i, 2] = yArr[i] * yArr[i];
					A[i, 3] = xArr[i];
					A[i, 4] = yArr[i]; 
					A[i, 5] = 1;
					X[i, 0] = zArr[i];
				}

				a_T = A.Transpose();
				aa = a_T * A;
				aa_inv = aa.Inverse();
				bx = (aa_inv * a_T) * X;

				m_A = bx[0, 0];
				m_B = bx[1, 0];
				m_C = bx[2, 0];
				m_D = bx[3, 0];
				m_E = bx[4, 0];
				m_F = bx[5, 0];
				 */
			}
			else if (order == 3)
			{
				// z = axxx + bxxy + cxyy + dyyy + exx + fxy + gyy + hx + iy + j

				int[] xArr = new int[10];
				int[] yArr = new int[10];
				double[] zArr = new double[10];

				xArr[0] = m_X0; yArr[0] = m_Y0; zArr[0] = depth;
				xArr[1] = m_X0 + m_Radius; yArr[1] = m_Y0 + m_Radius / 3; zArr[1] = 0;
				for (int i = 2; i < 6; i++)
				{
					xArr[i] = rnd.Next(m_X0, m_X0 + (int)(1.2 * m_Radius));
					yArr[i] = rnd.Next(m_Y0, m_Y0 + (int)(0.6 * m_Radius));
					zArr[i] = rnd.Next(0, (int)depth);
				}

				// Start with an approximation
				// z = axxx +     + dyyy +   +  fxy +    + j

				SafeMatrix A = new SafeMatrix(4, 4);
				SafeMatrix X = new SafeMatrix(4, 1);

				for (int i = 0; i < 4; i++)
				{
					A[i, 0] = xArr[i] * xArr[i] * xArr[i];
					A[i, 1] = yArr[i] * yArr[i] * yArr[i];
					A[i, 2] = xArr[i] * yArr[i];
					A[i, 3] = 1;
					X[i, 0] = zArr[i];
				}

				SafeMatrix a_T = A.Transpose();
				SafeMatrix aa = a_T * A;
				SafeMatrix aa_inv = aa.Inverse();
				SafeMatrix bx = (aa_inv * a_T) * X;

				this.A = bx[0, 0];
				D = bx[1, 0];
				F = bx[2, 0];
				J = bx[3, 0];

				B = 0;
				C = 0;
				E = 0;
				G = 0;
				H = 0;
				I = 0;
			}
		}

		internal void PlotBackground(PictureBox pbox, int order)
		{
			pbox.Image = new Bitmap(pbox.Width, pbox.Height, PixelFormat.Format24bppRgb);

			Bitmap bmp = new Bitmap(pbox.Width, pbox.Height, PixelFormat.Format24bppRgb);
			int[,] bgModel = GenerateBackground(order, 1, 0, 0, m_X0, m_Y0, Math.Max(pbox.Width, pbox.Height));

			for (int x = 0; x < pbox.Width; x++)
			{
				for (int y = 0; y < pbox.Height; y++)
				{
					int clr = Math.Max(0, Math.Min(255, bgModel[x, y]));
					bmp.SetPixel(x, y, Color.FromArgb(clr, clr, clr));
				}
			}

			using (Graphics g = Graphics.FromImage(pbox.Image))
			{
				g.DrawImage(bmp, 0, 0);
				g.Save();
			}

			pbox.Invalidate();
			pbox.Update();
		}
	}
}

