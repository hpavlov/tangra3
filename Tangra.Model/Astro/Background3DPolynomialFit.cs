using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Tangra.Model.Config;
using Tangra.Model.Image;
using Tangra.Model.Numerical;

namespace Tangra.Model.Astro
{
	public interface IBackgroundModelProvider
	{
		double ComputeValue(double x, double y);
	}

	public class Background3DPolynomialFit : IBackgroundModelProvider
	{
		private bool m_TryFirstOrder;
		private bool m_TrySecondOrder;
		private bool m_TryThirdOrder;

		private List<double> m_ZValues = new List<double>();
		private List<double> m_XValues = new List<double>();
		private List<double> m_YValues = new List<double>();
		private List<double> m_XXValues = new List<double>();
		private List<double> m_XYValues = new List<double>();
		private List<double> m_YYValues = new List<double>();
		private List<double> m_XXXValues = new List<double>();
		private List<double> m_XXYValues = new List<double>();
		private List<double> m_XYYValues = new List<double>();
		private List<double> m_YYYValues = new List<double>();
		private List<double> m_Residuals = new List<double>();

		private int m_BestFit = -1;

		private double m_FirstA;
		private double m_FirstB;
		private double m_FirstC;
		private double m_SecondA;
		private double m_SecondB;
		private double m_SecondC;
		private double m_SecondD;
		private double m_SecondE;
		private double m_SecondF;
		private double m_ThirdA;
		private double m_ThirdB;
		private double m_ThirdC;
		private double m_ThirdD;
		private double m_ThirdE;
		private double m_ThirdF;
		private double m_ThirdG;
		private double m_ThirdH;
		private double m_ThirdI;
		private double m_ThirdJ;
		private double m_FirstVariance;
		private double m_SecondVariance;
		private double m_ThirdVariance;

		private int m_Width;
		private int m_Height;

		private ImagePixel m_Star1Center;
		private double m_Star1MinDistance;
		private ImagePixel m_Star2Center;
		private double m_Star2MinDistance;

		public Background3DPolynomialFit()
		{
			m_TryFirstOrder = TangraConfig.Settings.Photometry.Background3DPoly.Try1stOrder;
			m_TrySecondOrder = TangraConfig.Settings.Photometry.Background3DPoly.Try2ndOrder;
			m_TryThirdOrder = TangraConfig.Settings.Photometry.Background3DPoly.Try3rdOrder;
		}

		public void Fit(uint[,] pixels, float starX0, float starY0, float startMinDistance)
		{
			m_Star1Center = new ImagePixel(-1, starX0, starY0);
			m_Star1MinDistance = startMinDistance;
			m_Star2Center = null;

			Fit(pixels);
		}

		public void Fit(uint[,] pixels, PSFFit star1, PSFFit star2)
		{
            m_Star1Center = new ImagePixel(star1.Brightness, star1.X0_Matrix, star1.Y0_Matrix);
			m_Star1MinDistance = star1.FWHM * 2.0;

			if (star2 != null)
			{
                m_Star2Center = new ImagePixel(star2.Brightness, star2.X0_Matrix, star2.Y0_Matrix);
				m_Star2MinDistance = star2.FWHM * 2.0;
			}
			else
				m_Star2Center = null;

			Fit(pixels);
		}

		private void Fit(uint[,] pixels)
		{
			m_Width = pixels.GetLength(0);
			m_Height = pixels.GetLength(1);

			if (m_Width != m_Height)
				throw new ArgumentException("Matrix not square.");

			for (int x = 0; x < m_Width; x++)
			{
				for (int y = 0; y < m_Height; y++)
				{
					if (IncludePixel(x, y))
					{
						m_XValues.Add(x);
						m_YValues.Add(y);
						
						if (m_TrySecondOrder)
						{							
							m_XXValues.Add(x * x);
							m_XYValues.Add(x * y);
							m_YYValues.Add(y * y);
						}
						
						if (m_TryThirdOrder)
						{														
							m_XXXValues.Add(x * x * x);
							m_XXYValues.Add(x * x * y);
							m_XYYValues.Add(x * y * y);
							m_YYYValues.Add(y * y * y);
						}

						m_ZValues.Add(pixels[x, y]);
					}
				}
			}

			SolveAll();
		}

		private bool IncludePixel(int x, int y)
		{
			if (m_Star1Center != null &&
				ImagePixel.ComputeDistance(m_Star1Center.XDouble, x, m_Star1Center.YDouble, y) <= m_Star1MinDistance)
			{
				return false;
			}

			if (m_Star2Center != null &&
				ImagePixel.ComputeDistance(m_Star2Center.XDouble, x, m_Star2Center.YDouble, y) <= m_Star2MinDistance)
			{
				return false;
			}
			
			return true;
		}

		private void SolveAll()
		{
			if (m_TryFirstOrder)
			{
				try
				{
					SolveFirstOrderFit();
				}
				catch(Exception ex)
				{
					Trace.WriteLine(ex);
					m_FirstVariance = double.MaxValue;
				}
			}				
			else
				m_FirstVariance = double.MaxValue;

			if (m_TrySecondOrder)
			{
				try
				{
					SolveSecondOrderFit();
				}
				catch (Exception ex)
				{
					Trace.WriteLine(ex);
					m_SecondVariance = double.MaxValue;
				}
			}
			else
				m_SecondVariance = double.MaxValue;

			if (m_TryThirdOrder)
			{
				try
				{
					SolveThirdOrderFit();
				}
				catch (Exception ex)
				{
					Trace.WriteLine(ex);
					m_ThirdVariance = double.MaxValue;
				}
			}				

			else
				m_ThirdVariance = double.MaxValue;

			if (m_FirstVariance < m_SecondVariance &&
			    m_FirstVariance < m_ThirdVariance)
			{
				m_BestFit = 1;
			}
			else if (
				m_SecondVariance < m_FirstVariance &&
				m_SecondVariance < m_ThirdVariance)
			{
				m_BestFit = 2;
			}
			else if (
				m_ThirdVariance < m_FirstVariance &&
				m_ThirdVariance < m_SecondVariance)
			{
				m_BestFit = 3;
			}
			else
				throw new InvalidOperationException();
		}

		// First Order (3 params): Z = A * x + B * y + C
		private void SolveFirstOrderFit()
		{
			SafeMatrix A = new SafeMatrix(m_ZValues.Count, 3);
			SafeMatrix X = new SafeMatrix(m_ZValues.Count, 1);

			for (int i = 0; i < m_ZValues.Count; i++)
			{
				A[i, 0] = m_XValues[i];
				A[i, 1] = m_YValues[i];
				A[i, 2] = 1;

				X[i, 0] = m_ZValues[i];
			}

			SafeMatrix a_T = A.Transpose();
			SafeMatrix aa = a_T * A;
			SafeMatrix aa_inv = aa.Inverse();
			SafeMatrix bx = (aa_inv * a_T) * X;

			m_FirstA = bx[0, 0];
			m_FirstB = bx[1, 0];
			m_FirstC = bx[2, 0];

			m_Residuals.Clear();

			double sumResidualsSQ = 0;
			for (int i = 0; i < m_ZValues.Count; i++)
			{
				double res = m_ZValues[i] - ComputeFirstOrderValue(m_XValues[i], m_YValues[i]);
				m_Residuals.Add(res);
				sumResidualsSQ += res * res;
			}

			m_FirstVariance = Math.Sqrt(sumResidualsSQ / (m_ZValues.Count - 1));
		}

		// Second Order (6 params): Z = A * x * x + B * x * y + C * y * y + D * x + E * y + F 	
		private void SolveSecondOrderFit()
		{
			SafeMatrix A = new SafeMatrix(m_ZValues.Count, 6);
			SafeMatrix X = new SafeMatrix(m_ZValues.Count, 1);

			for (int i = 0; i < m_ZValues.Count; i++)
			{
				A[i, 0] = m_XXValues[i];
				A[i, 1] = m_XYValues[i];
				A[i, 2] = m_YYValues[i];
				A[i, 3] = m_XValues[i];
				A[i, 4] = m_YValues[i];
				A[i, 5] = 1;

				X[i, 0] = m_ZValues[i];
			}

			SafeMatrix a_T = A.Transpose();
			SafeMatrix aa = a_T * A;
			SafeMatrix aa_inv = aa.Inverse();
			SafeMatrix bx = (aa_inv * a_T) * X;

			m_SecondA = bx[0, 0];
			m_SecondB = bx[1, 0];
			m_SecondC = bx[2, 0];
			m_SecondD = bx[3, 0];
			m_SecondE = bx[4, 0];
			m_SecondF = bx[5, 0];

			m_Residuals.Clear();

			double sumResidualsSQ = 0;
			for (int i = 0; i < m_ZValues.Count; i++)
			{
				double res = m_ZValues[i] - ComputeSecondOrderValue(m_XValues[i], m_YValues[i]);
				m_Residuals.Add(res);
				sumResidualsSQ += res * res;
			}

			m_SecondVariance = Math.Sqrt(sumResidualsSQ / (m_ZValues.Count - 1));
		}

		// Third Order (10 params): Z = A * x * x * x + B * x * x * y + C * y * y * x + D * y * y * y + E * x * x + F * x * y + G * y * y + H * x + I * y + J 
		private void SolveThirdOrderFit()
		{
			SafeMatrix A = new SafeMatrix(m_ZValues.Count, 10);
			SafeMatrix X = new SafeMatrix(m_ZValues.Count, 1);

			for (int i = 0; i < m_ZValues.Count; i++)
			{
				A[i, 0] = m_XXXValues[i];
				A[i, 1] = m_XXYValues[i];
				A[i, 2] = m_XYYValues[i];
				A[i, 3] = m_YYYValues[i];
				A[i, 4] = m_XXValues[i];
				A[i, 5] = m_XYValues[i];
				A[i, 6] = m_YYValues[i];
				A[i, 7] = m_XValues[i];
				A[i, 8] = m_YValues[i];
				A[i, 9] = 1;

				X[i, 0] = m_ZValues[i];
			}

			SafeMatrix a_T = A.Transpose();
			SafeMatrix aa = a_T*A;
			SafeMatrix aa_inv = aa.Inverse();
			SafeMatrix bx = (aa_inv*a_T)*X;

			m_ThirdA = bx[0, 0];
			m_ThirdB = bx[1, 0];
			m_ThirdC = bx[2, 0];
			m_ThirdD = bx[3, 0];
			m_ThirdE = bx[4, 0];
			m_ThirdF = bx[5, 0];
			m_ThirdG = bx[6, 0];
			m_ThirdH = bx[7, 0];
			m_ThirdI = bx[8, 0];
			m_ThirdJ = bx[9, 0];

			m_Residuals.Clear();

			double sumResidualsSQ = 0;
			for (int i = 0; i < m_ZValues.Count; i++)
			{
				double res = m_ZValues[i] - ComputeThirdOrderValue(m_XValues[i], m_YValues[i]);
				m_Residuals.Add(res);
				sumResidualsSQ += res * res;
			}

			m_ThirdVariance = Math.Sqrt(sumResidualsSQ/(m_ZValues.Count - 1));
		}

		// First Order (3 params): Z = A * x + B * y + C
		private double ComputeFirstOrderValue(double x, double y)
		{
			return m_FirstA * x +  m_FirstB * y + m_FirstC;
		}

		// Second Order (6 params): Z = A * x * x + B * x * y + C * y * y + D * x + E * y + F 
		private double ComputeSecondOrderValue(double x, double y)
		{
			return m_SecondA * x * x + m_SecondB * x * y + m_SecondC * y * y + m_SecondD * x + m_SecondE * y + m_SecondF;
		}

		// Third Order (10 params): Z = A * x * x * x + B * x * x * y + C * y * y * x + D * y * y * y + E * x * x + F * x * y + G * y * y + H * x + I * y + J 
		private double ComputeThirdOrderValue(double x, double y)
		{
			return m_ThirdA * x * x * x + m_ThirdB * x * x * y + m_ThirdC * x * y * y + m_ThirdD * y * y * y + m_ThirdE * x * x + m_ThirdF * x * y + m_ThirdG * y * y + m_ThirdH * x + m_ThirdI * y + m_ThirdJ;
		}

		public double ComputeValue(double x, double y)
		{
			if (m_BestFit == 1)
				return ComputeFirstOrderValue(x, y);
			else if (m_BestFit == 2)
				return ComputeSecondOrderValue(x, y);
			else if (m_BestFit == 3)
				return ComputeThirdOrderValue(x, y);

			throw new ArgumentOutOfRangeException();
		}

		public uint[,] GetFittedPixels()
		{
			uint[,] pixels = new uint[m_Width, m_Height];

			for (int x = 0; x < m_Width; x++)
			{
				for (int y = 0; y < m_Height; y++)
				{
					pixels[x, y] = (uint)Math.Round(ComputeValue(x, y));
				}
			}

			return pixels;
		}
	}
}
