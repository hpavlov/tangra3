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
			return M0 + C * (1 - Math.Pow(1 - Math.Exp(1 - Math.Cosh((t - T0) / D)), G));
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

		public void Solve(bool useNormalisation, double t0)
		{
			var varTimes = new List<double>();
			var varMagnitudes = new List<double>();
			
			double constNorm = double.NaN;
			if (useNormalisation)
			{
				List<double> medianCalcList = new List<double>(m_CompData);
				medianCalcList.Sort();
				constNorm = medianCalcList[medianCalcList.Count/2];
			}

			M0 = 10; // Get from known max magnitude of star

			for (int i = 0; i < m_NumObs; i++)
			{
				double mag = M0 - 2.5 * Math.Log10(m_VarData[i] / (useNormalisation ? constNorm : m_CompData[i]));

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

			// TODO: 1) Exclude C from the fit. Fix to 1
			//       2) Exclude M0 from the fit. Compute after each itteration from the median value of the datapoints around the T0 from the last itteration
			//       3) Do the nonlinear least square only on T0, G and D

			// Starting values			
			C = 1;
			T0 = t0; // Get from Kwee van Worden 
			G = 0.5;
			// Get D from solving the equation to D (with other starting values specified)
			D = (varTimes[varMagnitudes.Count - 1] - T0) / Acosh(1 - Math.Log(1 - Math.Pow(1 - (varMagnitudes[varMagnitudes.Count - 1] - M0) / C, 1 / G)));

			NumberIterations = 100;

			double[] minCorr = new double[] {double.MaxValue, double.MaxValue, double.MaxValue, double.MaxValue, double.MaxValue};

			for (int iter = NumberIterations; iter > 0; iter--)
			{
				var A = new SafeMatrix(varMagnitudes.Count, 5);
				var X = new SafeMatrix(varMagnitudes.Count, 1);

				for (int index = 0; index < varMagnitudes.Count; index++)
				{
					double t = varTimes[index];
					double difference = ComputeModelValue(t) - varMagnitudes[index];
					X[index, 0] = -difference;

					double GAMMA = 1 - Math.Exp(1 - Math.Cosh((t - T0) / D));
					double GAMMA_G = Math.Pow(GAMMA, G);
					double GAMMA_GM1 = Math.Pow(GAMMA, G - 1);

					A[index, 0] = 1.0; // M0
					A[index, 1] = 1 - GAMMA_G; // C
					A[index, 2] = (C * G / D) * GAMMA_GM1 * Math.Exp(1 - Math.Cosh((t - T0) / D)) * Math.Sinh((t - T0) / D); // T0
					A[index, 3] = A[index, 2] * (t - T0) / D; // D
					A[index, 4] = -C * GAMMA_G * Math.Log(G); // G
				}

				SafeMatrix a_T = A.Transpose();
				SafeMatrix aa = a_T * A;
				SafeMatrix aa_inv = aa.Inverse();
				SafeMatrix Y = (aa_inv * a_T) * X;

				if (minCorr[0] > Math.Abs(Y[0, 0])) minCorr[0] = Y[0, 0];
				if (minCorr[1] > Math.Abs(Y[1, 0])) minCorr[1] = Y[1, 0];
				if (minCorr[2] > Math.Abs(Y[2, 0])) minCorr[2] = Y[2, 0];
				if (minCorr[3] > Math.Abs(Y[3, 0])) minCorr[3] = Y[3, 0];
				if (minCorr[4] > Math.Abs(Y[4, 0])) minCorr[4] = Y[4, 0];

				if (Y[2, 0] > 0.0001) Y[2, 0] = 0.0001;
				if (Y[2, 0] < -0.0001) Y[2, 0] = -0.0001;
				T0 += Y[2, 0];

				
				// Make sure we have a very good estimation of T0 before varying the other parameters
				if (Y[0, 0] > 0.02) Y[0, 0] = 0.02;
				if (Y[0, 0] < -0.02) Y[0, 0] = -0.02;
				M0 += Y[0, 0];

				//if (Y[1, 0] > 0.01) Y[1, 0] = 0.01;
				//if (Y[1, 0] < -0.01) Y[1, 0] = -0.01;
				//C += Y[1, 0];

				if (Y[3, 0] > 0.01) Y[3, 0] = 0.01;
				if (Y[3, 0] < -0.01) Y[3, 0] = -0.01;
				D += Y[3, 0];
				if (D < 0) D = 0.001;

				// Enforce maximum correction for G of |0.1|. G > 0
				if (Y[4, 0] > 0.01) Y[4, 0] = 0.01;
				if (Y[4, 0] < -0.01) Y[4, 0] = -0.01;
				G += Y[4, 0];
				if (G < 0) G = 0.001;
				
			}

			Trace.WriteLine(string.Format("{0}; {1}; {2}; {3}; {4}", minCorr[0], minCorr[1], minCorr[2], minCorr[3], minCorr[4]));
			Times.Clear();
			NormIntensities.Clear();


			for (int index = 0; index < varMagnitudes.Count; index++)
			{
				double t = varTimes[index];
				Times.Add(t);
				double magVal = ComputeModelValue(t);
				NormIntensities.Add(Math.Pow(10, (10 - magVal)/2.5));
			}
		}

		public bool IsOutlier(int i, double mag)
		{
			return false;
		}
	}
}
