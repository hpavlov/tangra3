using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Tangra.KweeVanWoerden
{
	public class ModelFitter
	{
		private List<double> m_Times = new List<double>();
		private List<double> m_VarData = new List<double>();
		private List<double> m_CompData = new List<double>();
		private int m_NumObs;

		public int NumberIterations = 20;

		public double M0 { get; private set; }
		public double C { get; private set; }
		public double T0 { get; private set; }
		public double D { get; private set; }
		public double G { get; private set; }

		public bool IsSolved { get; private set; }

		public List<double> Times = new List<double>();
		public List<double> NormIntensities = new List<double>();

		public ModelFitter(int numObs, double[] times, double[] varData, double[] compData)
		{
			m_NumObs = numObs;
			m_Times.AddRange(times);
			m_VarData.AddRange(varData);
			m_CompData.AddRange(compData);
		}

		private double ComputeModelValue(double t)
		{
            return M0 - Math.Pow(1 - Math.Exp(1 - Math.Cosh((t - T0) / D)), G);
		}

		/// <summary>
		/// Returns the hyperbolic arc cosine of the specified number.
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		public static double Acosh(double x)
		{
			if (x < 1.0) throw new ArithmeticException("range exception");
			return Math.Log(x + Math.Sqrt(x * x - 1));
		}

        private double FitM0ForT0(double t0, double constNorm)
        {
            for (int i0 = 0; i0 < m_Times.Count - 1; i0++)
            {
                if (m_Times[i0] <= t0 && m_Times[i0 + 1] > t0)
                {
                    // index found
                    var varDataAroundT0 = new List<double>();
                    var compDataAroundT0 = new List<double>();
                    varDataAroundT0.AddRange(m_VarData.Where((x, i) => i > i0 - 10 && i < i0 + 10));
                    compDataAroundT0.AddRange(m_CompData.Where((x, i) => i > i0 - 10 && i < i0 + 10));
                    var dataAroundT0 = new List<double>();
                    for (int k = 0; k < varDataAroundT0.Count; k++)
                    {
                        dataAroundT0.Add( - 2.5 * Math.Log10(varDataAroundT0[k] / (!double.IsNaN(constNorm) ? constNorm : compDataAroundT0[k])));
                    }
                    dataAroundT0.Sort();
                    return dataAroundT0[dataAroundT0.Count / 2];
                }
            }

            throw new IndexOutOfRangeException();
        }

		public void Solve(bool useNormalisation, double t0_initialVal)
		{
			var varTimes = new List<double>();
			var varMagnitudes = new List<double>();
			
			double constNorm = double.NaN;
			if (!useNormalisation)
			{
				List<double> medianCalcList = new List<double>(m_CompData);
				medianCalcList.Sort();
				constNorm = medianCalcList[medianCalcList.Count/2];
			}

		    C = 1; // Fix C to 1
            M0 = FitM0ForT0(t0_initialVal, constNorm) - 1; // Get from known max magnitude of star

			for (int i = 0; i < m_NumObs; i++)
			{
				double mag = M0 - 2.5 * Math.Log10(m_VarData[i] / (!useNormalisation ? constNorm : m_CompData[i]));

				if (IsOutlier(i, mag)) continue;

				varTimes.Add(m_Times[i]);
				varMagnitudes.Add(mag);
			}

			if (varMagnitudes.Count < 7)
			{
				// Not enought data points for solving 5 variables
				IsSolved = false;
				return;
			}

			// Starting values			
			C = 1;
			T0 = t0_initialVal; // Get from Kwee van Worden 
			G = 0.5;
			// Get D from solving the equation to D (with other starting values specified)
			D = (varTimes[varMagnitudes.Count - 1] - T0) / Acosh(1 - Math.Log(1 - Math.Pow(1 - (varMagnitudes[varMagnitudes.Count - 1] - M0) / C, 1 / G)));
		    if (double.IsNaN(D) || double.IsInfinity(D)) D = 0.1;

			NumberIterations = 100;

			double[] minCorr = new double[] {double.MaxValue, double.MaxValue, double.MaxValue, double.MaxValue, double.MaxValue};


            //// Our guess ot T0 is very close, so assume it is contant and solve for G and D
            //for (int iter = NumberIterations; iter > 0; iter--)
            //{
            //    var A = new SafeMatrix(varMagnitudes.Count, 2);
            //    var X = new SafeMatrix(varMagnitudes.Count, 1);

            //    for (int index = 0; index < varMagnitudes.Count; index++)
            //    {
            //        double t = varTimes[index];
            //        double difference = ComputeModelValue(t) - varMagnitudes[index];
            //        X[index, 0] = -difference;

            //        double GAMMA = 1 - Math.Exp(1 - Math.Cosh((t - T0) / D));
            //        double GAMMA_G = Math.Pow(GAMMA, G);

            //        A[index, 0] = GAMMA_G * Math.Log(G); // G
            //        A[index, 1] = -(G / D) * Math.Pow(GAMMA, G - 1) * Math.Exp(1 - Math.Cosh((t - T0) / D)) * Math.Sinh((t - T0) / D) * (t - T0) / D; // D
            //    }

            //    SafeMatrix a_T = A.Transpose();
            //    SafeMatrix aa = a_T * A;
            //    SafeMatrix aa_inv = aa.Inverse();
            //    SafeMatrix Y = (aa_inv * a_T) * X;

            //    D += Y[1, 0];
            //    if (D < 0) D = 0.001;

            //    // Enforce maximum correction for G of |0.1|. G > 0
            //    //if (Y[0, 0] > 0.01) Y[0, 0] = 0.01;
            //    //if (Y[0, 0] < -0.01) Y[0, 0] = -0.01;
            //    G += Y[0, 0];
            //    if (G < 0) G = 0.001;

            //    Trace.WriteLine(string.Format("D Corr: {0}\t\t G Corr: {1}", Y[1, 0], Y[0, 0]));
            //}

			for (int iter = NumberIterations; iter > 0; iter--)
			{
				var A = new SafeMatrix(varMagnitudes.Count, 3);
				var X = new SafeMatrix(varMagnitudes.Count, 1);

				for (int index = 0; index < varMagnitudes.Count; index++)
				{
					double t = varTimes[index];
					double difference = ComputeModelValue(t) - varMagnitudes[index];
					X[index, 0] = -difference;

					double GAMMA = 1 - Math.Exp(1 - Math.Cosh((t - T0) / D));
					double GAMMA_G = Math.Pow(GAMMA, G);

					A[index, 0] = -GAMMA_G * Math.Log(G); // G
                    A[index, 1] = (G / D) * Math.Pow(GAMMA, G - 1) * Math.Exp(1 - Math.Cosh((t - T0) / D)) * Math.Sinh((t - T0) / D); // T0
					A[index, 2] = A[index, 1] * (t - T0) / D; // D
				}

				SafeMatrix a_T = A.Transpose();
				SafeMatrix aa = a_T * A;
				SafeMatrix aa_inv = aa.Inverse();
				SafeMatrix Y = (aa_inv * a_T) * X;

				if (minCorr[0] > Math.Abs(Y[0, 0])) minCorr[0] = Y[0, 0];
				if (minCorr[1] > Math.Abs(Y[1, 0])) minCorr[1] = Y[1, 0];
				if (minCorr[2] > Math.Abs(Y[2, 0])) minCorr[2] = Y[2, 0];

				//if (Y[1, 0] > 0.0001) Y[1, 0] = 0.0001;
				//if (Y[1, 0] < -0.0001) Y[1, 0] = -0.0001;
				T0 += Y[1, 0];

                Trace.WriteLine(string.Format("T0 Corr: {0}; ", Y[1, 0]));
				
                //if (Y[2, 0] > 0.01) Y[2, 0] = 0.01;
                //if (Y[2, 0] < -0.01) Y[2, 0] = -0.01;
				D += Y[2, 0];
				if (D < 0) D = 0.001;

				// Enforce maximum correction for G of |0.1|. G > 0
                //if (Y[0, 0] > 0.01) Y[0, 0] = 0.01;
                //if (Y[0, 0] < -0.01) Y[0, 0] = -0.01;
				G += Y[0, 0];
				if (G < 0) G = 0.001;
				
			}

			//Trace.WriteLine(string.Format("{0}; {1}; {2}; {3}; {4}", minCorr[0], minCorr[1], minCorr[2], minCorr[3], minCorr[4]));

			Times.Clear();
			NormIntensities.Clear();
		    var residuals = new List<double>();

			for (int index = 0; index < varMagnitudes.Count; index++)
			{
				double t = varTimes[index];
				Times.Add(t);
				double magVal = ComputeModelValue(t);
                double residual = magVal - varMagnitudes[index];
				NormIntensities.Add(Math.Pow(10, (10 - magVal)/2.5));
			    residuals.Add(residual);
			}
		}

		public bool IsOutlier(int i, double mag)
		{
			return false;
		}
	}
}
