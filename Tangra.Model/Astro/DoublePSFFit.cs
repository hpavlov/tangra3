using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Tangra.Model.Config;
using Tangra.Model.Helpers;
using Tangra.Model.Numerical;

namespace Tangra.Model.Astro
{
	public class DoublePSFFit : PSFFitBase, IPSFFit, ITrackedObjectPsfFit
	{
		private static double CERTAINTY_CONST = 0.5 / 0.03;

		private static int NumberIterations = 10;
		private int m_xCenter;
		private int m_yCenter;

		private double m_X0;
		private double m_Y0;
		private double m_R0;
		private double m_IStarMax;

		private double m_X1;
		private double m_Y1;
		private double m_X2;
		private double m_Y2;
		private double m_R1;
		private double m_R2;
		private double m_IBackground;
		private double m_IStarMax1;
		private double m_IStarMax2;

		private bool m_IsSolved;

		private int m_MatrixSize;

		private int m_HalfWidth;

		private uint m_Saturation;

		private double[,] m_Residuals;

		public DoublePSFFit(int x0, int y0)
		{
			m_xCenter = x0;
			m_yCenter = y0;
		}

		// I(x, y) = IBackground + IStarMax1 * Exp ( -((x - X1)*(x - X1) + (y - Y1)*(y - Y1)) / (r1 * r1)) + IStarMax2 * Exp ( -((x - X2)*(x - X2) + (y - Y2)*(y - Y2)) / (r2 * r2))
		public void Fit(uint[,] intensity, int x1start, int y1start, int x2start, int y2start, bool useNativeMatrix)
		{
			m_IsSolved = false;

			try
			{
				int full_width = (int)Math.Round(Math.Sqrt(intensity.Length));

				m_MatrixSize = full_width;

				int half_width = full_width / 2;

				m_HalfWidth = half_width;

				switch (PSFFit.DataRange)
				{
					case PSFFittingDataRange.DataRange8Bit:
						m_Saturation = TangraConfig.Settings.Photometry.Saturation.Saturation8Bit;
						break;

					case PSFFittingDataRange.DataRange12Bit:
						m_Saturation = TangraConfig.Settings.Photometry.Saturation.Saturation12Bit;
						break;

					case PSFFittingDataRange.DataRange14Bit:
						m_Saturation = TangraConfig.Settings.Photometry.Saturation.Saturation14Bit;
						break;

					case PSFFittingDataRange.DataRange16Bit:
						m_Saturation = (uint)(TangraConfig.Settings.Photometry.Saturation.Saturation8Bit * PSFFit.NormVal / 255.0);
						break;

					default:
						m_Saturation = TangraConfig.Settings.Photometry.Saturation.Saturation8Bit;
						break;
				}

				int nonSaturatedPixels = 0;

				double IBackground = 0;
				double r1 = 4.0;
				double r2 = 4.0;
				double IStarMax1 = 0;
				double IStarMax2 = 0;

				double found_x1 = x1start;
				double found_y1 = y1start;
				double found_x2 = x2start;
				double found_y2 = y2start;

				for (int iter = NumberIterations; iter > 0; iter--)
				{
					if (iter == NumberIterations)
					{
						uint zval = 0;

						IBackground = 0.0; /* assume no backgnd intensity at first... */
						for (int i = 0; i < full_width; i++)
						{
							for (int j = 0; j < full_width; j++)
							{
								if (intensity[i, j] > zval) zval = intensity[i, j];
								if (intensity[i, j] < m_Saturation) nonSaturatedPixels++;
							}
						}
						IStarMax1 = (double)zval - IBackground;
						IStarMax2 = IStarMax1;
					}

					double[] dx1 = new double[full_width];
					double[] dy1 = new double[full_width];
					double[] fx1 = new double[full_width];
					double[] fy1 = new double[full_width];
					double[] dx2 = new double[full_width];
					double[] dy2 = new double[full_width];
					double[] fx2 = new double[full_width];
					double[] fy2 = new double[full_width];

					double r1_squared = r1 * r1;
					double r2_squared = r2 * r2;

					for (int i = 0; i < full_width; i++)
					{
						dx1[i] = (double)i - found_x1;
						fx1[i] = Math.Exp(-dx1[i] * dx1[i] / r1_squared);
						dy1[i] = (double)i - found_y1;
						fy1[i] = Math.Exp(-dy1[i] * dy1[i] / r1_squared);
						dx2[i] = (double)i - found_x2;
						fx2[i] = Math.Exp(-dx2[i] * dx2[i] / r2_squared);
						dy2[i] = (double)i - found_y2;
						fy2[i] = Math.Exp(-dy2[i] * dy2[i] / r2_squared);
					}

					SafeMatrix A = new SafeMatrix(nonSaturatedPixels, 9);
					SafeMatrix X = new SafeMatrix(nonSaturatedPixels, 1);

					int index = -1;
					for (int i = 0; i < full_width; i++)
					{
						for (int j = 0; j < full_width; j++)
						{
							uint zval = intensity[i, j];

							if (zval < m_Saturation)
							{
								index++;

								double exp_val1 = fx1[i] * fy1[j];
								double exp_val2 = fx2[i] * fy2[j];

								double residual = IBackground + IStarMax1 * exp_val1 + IStarMax2 * exp_val2 - (double)zval;
								X[index, 0] = -residual;

								A[index, 0] = 1.0; /* slope in i0 */
								A[index, 1] = exp_val1;
								A[index, 2] = exp_val2;
								A[index, 3] = 2.0 * IStarMax1 * dx1[i] * exp_val1 / r1_squared;
								A[index, 4] = 2.0 * IStarMax1 * dy1[j] * exp_val1 / r1_squared;
								A[index, 5] = 2.0 * IStarMax2 * dx2[i] * exp_val2 / r2_squared;
								A[index, 6] = 2.0 * IStarMax2 * dy2[j] * exp_val2 / r2_squared;
								A[index, 7] = 2.0 * IStarMax1 * (dx1[i] * dx1[i] + dy1[j] * dy1[j]) * exp_val1 / (r1 * r1_squared);
								A[index, 8] = 2.0 * IStarMax2 * (dx2[i] * dx2[i] + dy2[j] * dy2[j]) * exp_val2 / (r2 * r2_squared);
							}
						}
					}

					SafeMatrix Y;

					if (useNativeMatrix)
					{
						Y = TangraModelCore.SolveLinearSystemFast(A, X);
					}
					else
					{
						SafeMatrix a_T = A.Transpose();
						SafeMatrix aa = a_T * A;
						SafeMatrix aa_inv = aa.Inverse();
						Y = (aa_inv * a_T) * X;
					}

					/* we need at least 9 unsaturated pixels to solve 8 params */
					if (nonSaturatedPixels > 9) /* Request all pixels to be good! */
					{
						IBackground += Y[0, 0];
						IStarMax1 += Y[1, 0];
						IStarMax2 += Y[2, 0];

						for (int i = 3; i < 7; i++)
						{
							if (Y[i, 0] > 1.0) Y[i, 0] = 1.0;
							if (Y[i, 0] < -1.0) Y[i, 0] = -1.0;
						}

						found_x1 += Y[3, 0];
						found_y1 += Y[4, 0];
						found_x2 += Y[5, 0];
						found_y2 += Y[6, 0];

						if (Y[7, 0] > r1 / 10.0) Y[7, 0] = r1 / 10.0;
						if (Y[7, 0] < -r1 / 10.0) Y[7, 0] = -r1 / 10.0;

						r1 += Y[7, 0];

						if (Y[8, 0] > r1 / 10.0) Y[8, 0] = r1 / 10.0;
						if (Y[8, 0] < -r1 / 10.0) Y[8, 0] = -r1 / 10.0;

						r2 += Y[8, 0];
					}
					else
					{
						m_IsSolved = false;
						return;
					}
				}

				m_IsSolved = true;
				m_IBackground = IBackground;
				m_IStarMax1 = IStarMax1;
				m_IStarMax2 = IStarMax2;
				m_X1 = found_x1;
				m_Y1 = found_y1;
				m_R1 = r1;
				m_X2 = found_x2;
				m_Y2 = found_y2;
				m_R2 = r2;

				// NOTE: The brighter object becomes the object to be tracked/returned as main object
				if (m_IStarMax1 > m_IStarMax2)
				{
					m_IStarMax = IStarMax1;
					m_X0 = found_x1;
					m_Y0 = found_y1;
					m_R0 = r1;
				}
				else
				{
					m_IStarMax = IStarMax2;
					m_X0 = found_x2;
					m_Y0 = found_y2;
					m_R0 = r2;					
				}

				m_Residuals = new double[full_width, full_width];

				for (int x = 0; x < full_width; x++)
					for (int y = 0; y < full_width; y++)
					{
						m_Residuals[x, y] = intensity[x, y] - GetPSFValueInternal(x, y);
					}
			}
			catch (DivideByZeroException)
			{ }
		}

		private double GetPSFValueInternal(double x, double y)
		{
			// I(x, y) = IBackground + IStarMax1 * Exp ( -((x - X1)*(x - X1) + (y - Y1)*(y - Y1)) / (r1 * r1)) + IStarMax2 * Exp ( -((x - X2)*(x - X2) + (y - Y2)*(y - Y2)) / (r2 * r2))
			return m_IBackground + 
				m_IStarMax1 * Math.Exp(-((x - m_X1) * (x - m_X1) + (y - m_Y1) * (y - m_Y1)) / (m_R1 * m_R1)) + 
				m_IStarMax2 * Math.Exp(-((x - m_X2) * (x - m_X2) + (y - m_Y2) * (y - m_Y2)) / (m_R2 * m_R2));
		}

		public void DrawGraph(System.Drawing.Graphics g, System.Drawing.Rectangle rect, int bpp, uint normVal)
		{
			throw new NotImplementedException();
		}

		public void DrawDataPixels(System.Drawing.Graphics g, System.Drawing.Rectangle rect, int bpp, uint normVal)
		{
			throw new NotImplementedException();
		}

		public int MatrixSize
		{
			get { return m_MatrixSize; }
		}

		public double XCenter
		{
			get { return m_xCenter + m_X0 - m_HalfWidth; }
		}

		public double YCenter
		{
			get { return m_yCenter + m_Y0 - m_HalfWidth; }
		}

		public double X1Center
		{
			get { return m_xCenter + m_X1 - m_HalfWidth; }
		}

		public double Y1Center
		{
			get { return m_yCenter + m_Y1 - m_HalfWidth; }
		}

		public double X2Center
		{
			get { return m_xCenter + m_X2 - m_HalfWidth; }
		}

		public double Y2Center
		{
			get { return m_yCenter + m_Y2 - m_HalfWidth; }
		}

		public override double FWHM
		{
			get { return 2 * Math.Sqrt(Math.Log(2)) * m_R0; }
		}

		public double IMax
		{
			get { return m_IBackground + m_IStarMax; }
		}

		public double I0
		{
			get { return m_IBackground; }
		}

		public double X0
		{
			get { return m_X0; }
		}

		public double Y0
		{
			get { return m_Y0; }
		}

		public double FWHM1
		{
			get { return 2 * Math.Sqrt(Math.Log(2)) * m_R1; }
		}

		public double FWHM2
		{
			get { return 2 * Math.Sqrt(Math.Log(2)) * m_R2; }
		}

		public bool IsSolved
		{
			get { return m_IsSolved; }
		}

		public double GetPSFValueAt(double x, double y)
		{
			return GetPSFValueInternal(x, y);
		}

		public double GetResidualAt(int x, int y)
		{
			return m_Residuals[x, y];
		}

		public void DrawDataPixels(Graphics g, Rectangle rect, float aperture, Pen aperturePen, int bpp, uint normVal)
		{
			throw new NotImplementedException();
		}

		public override void DrawInternalPoints(Graphics g, Rectangle rect, float aperture, Brush incldedPinBrush, int bpp)
		{
			DrawInternalPoints(g, rect, aperture, aperture, incldedPinBrush, incldedPinBrush, Brushes.Gray, Pens.GreenYellow, bpp);
		}

		public void DrawInternalPoints(Graphics g, Rectangle rect, float aperture1, float aperture2, Brush incldedPinBrush1, Brush incldedPinBrush2, int bpp)
		{
			DrawInternalPoints(g, rect, aperture1, aperture2, incldedPinBrush1, incldedPinBrush2, Brushes.Gray, Pens.GreenYellow, bpp);
		}

		public override float X0_Matrix
		{
			get { return (float)m_X0; }
		}

		public override float Y0_Matrix
		{
			get { return (float)m_Y0; }
		}

		public float X1_Matrix
		{
			get { return (float)m_X1; }
		}

		public float Y1_Matrix
		{
			get { return (float)m_Y1; }
		}

		public float X2_Matrix
		{
			get { return (float)m_X2; }
		}

		public float Y2_Matrix
		{
			get { return (float)m_Y2; }
		}

		private void DrawInternalPoints(
			Graphics g, Rectangle rect, 
			float aperture1, float aperture2, Brush incldedPinBrush1, Brush incldedPinBrush2,
			Brush excludedBrush, Pen curvePen, int bpp)
		{
			float margin = 3.0f;
			double maxZ = 256.0;
			if (bpp == 14) maxZ = 16384.0;
			else if (bpp == 12) maxZ = 4096.0;
			else if (bpp == 16) maxZ = PSFFit.NormVal;

			maxZ = Math.Min(maxZ, GetPSFValueInternal(m_X0, m_Y0) + 5);

			int totalSteps = 100;

			float halfWidth = (float)m_MatrixSize / 2.0f;
			float step = (float)(m_MatrixSize * 1.0 / totalSteps);

			float yScale = (float)((rect.Height * 1.0 - 2 * margin) / (maxZ - m_IBackground));
			float xScale = (float)(rect.Width * 1.0 - 2 * margin) / (halfWidth * 2);

			float xPrev1 = float.NaN;
			float xPrev2 = float.NaN;
			float yPrev1 = float.NaN;
			float yPrev2 = float.NaN;

			g.FillRectangle(SystemBrushes.ControlDarkDark, rect);

			double xa = m_X1 < m_X2 ? m_X1 : m_X2;
			double xb = m_X1 < m_X2 ? m_X2 : m_X1;
			double ya = m_X1 < m_X2 ? m_Y1 : m_Y2;
			double yb = m_X1 < m_X2 ? m_Y2 : m_Y1;

			double _a = (yb - ya) / (xb - xa);
			double _b = (ya - _a * xa + yb - _a * xb) / 2;
			
			for (float w = 0; w < m_MatrixSize; w += step)
			{
				float z = (float)GetPSFValueInternal(w, _a * w + _b);
				float x = margin + (float)(w) * xScale;
				float y = rect.Height - margin - (z - (float)m_IBackground) * yScale;

				if (!float.IsNaN(xPrev1) &&
					y > margin && yPrev1 > margin &&
					y < rect.Height && yPrev1 < rect.Height)
				{
					g.DrawLine(curvePen, xPrev1, yPrev1, x, y);
				}

				xPrev1 = x;
				yPrev1 = y;
			}

			if (m_Residuals == null)
				return;

			for (int x = 0; x < m_MatrixSize; x++)
				for (int y = 0; y < m_MatrixSize; y++)
				{
					double z0 = GetPSFValueInternal(x, y);
					double z = z0 + m_Residuals[x, y];
					double d1 = Math.Sqrt((x - m_X1) * (x - m_X1) + (y - m_Y1) * (y - m_Y1));
					double d2 = Math.Sqrt((x - m_X2) * (x - m_X2) + (y - m_Y2) * (y - m_Y2));

					float xVal1 = (float)(margin + (m_X1 + Math.Sign(x - m_X1) * d1) * xScale);
					float xVal2 = (float)(margin + (m_X2 + Math.Sign(x - m_X2) * d2) * xScale);
					float yVal = rect.Height - margin - (float)(z - m_IBackground) * yScale;

					if (d1 < d2)
						g.FillRectangle((d1 <= aperture1) ? incldedPinBrush1 : excludedBrush, xVal1 - 1, yVal - 1, 3, 3);
					else
						g.FillRectangle((d2 <= aperture2) ? incldedPinBrush2 : excludedBrush, xVal2 - 1, yVal - 1, 3, 3);
				}

			// Uncomment to draw the FWHM
			//g.DrawLine(
			//    Pens.White,
			//    rect.Width / 2 - ((float)FWHM * xScale / 2),
			//    rect.Height - margin - ((float)m_IStarMax - (float)m_IBackground) * yScale / 2,
			//    rect.Width / 2 + ((float)FWHM * xScale / 2),
			//    rect.Height - margin - ((float)m_IStarMax - (float)m_IBackground) * yScale / 2);
		}

		public PSFFit GetGaussian1()
		{
			using (MemoryStream memStr = new MemoryStream())
			{
				var wrt = new BinaryWriter(memStr);
				BinarySerialize1(wrt);

				memStr.Position = 0;

				var rdr = new BinaryReader(memStr);
				return PSFFit.Load(rdr);
			}
		}

		public PSFFit GetGaussian2()
		{
			using (MemoryStream memStr = new MemoryStream())
			{
				var wrt = new BinaryWriter(memStr);
				BinarySerialize2(wrt);

				memStr.Position = 0;

				var rdr = new BinaryReader(memStr);
				return PSFFit.Load(rdr);
			}
		}

		private void BinarySerialize1(BinaryWriter writer)
		{
			const byte STREAM_VERSION = 3;

			writer.Write(STREAM_VERSION);

			writer.Write(m_IBackground);
			writer.Write(m_IStarMax1);
			writer.Write(m_X1);
			writer.Write(m_Y1);
			writer.Write(m_R1);

			writer.Write(m_MatrixSize);
			writer.Write(m_HalfWidth);
			writer.Write(m_Saturation);

			writer.Write(m_xCenter);
			writer.Write(m_yCenter);

			for (int x = 0; x < m_MatrixSize; x++)
				for (int y = 0; y < m_MatrixSize; y++)
					writer.Write(m_Residuals[x, y]);

			writer.Write((int)0);
			writer.Write((int)0);
			writer.Write((int)0);

			writer.Write(double.NaN);
			writer.Write(double.NaN);			
		}

		private void BinarySerialize2(BinaryWriter writer)
		{
			const byte STREAM_VERSION = 3;

			writer.Write(STREAM_VERSION);

			writer.Write(m_IBackground);
			writer.Write(m_IStarMax2);
			writer.Write(m_X2);
			writer.Write(m_Y2);
			writer.Write(m_R2);

			writer.Write(m_MatrixSize);
			writer.Write(m_HalfWidth);
			writer.Write(m_Saturation);

			writer.Write(m_xCenter);
			writer.Write(m_yCenter);

			for (int x = 0; x < m_MatrixSize; x++)
				for (int y = 0; y < m_MatrixSize; y++)
					writer.Write(m_Residuals[x, y]);

			writer.Write((int)0);
			writer.Write((int)0);
			writer.Write((int)0);

			writer.Write(double.NaN);
			writer.Write(double.NaN);
		}
	}
}
