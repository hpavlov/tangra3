using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.Model.Helpers;

namespace Tangra.Model.Numerical
{
    public class LinearRegression
    {
        private List<double> m_XValues = new List<double>();
        private List<double> m_YValues = new List<double>();
        private List<double> m_Weights = new List<double>();

        private double m_A = 0;
        private double m_B = 0;
        private SafeMatrix m_CoVarianceMatrix = null;
        private double m_B_Uncertainty = double.NaN;
        private double m_A_Uncertainty = double.NaN;

        public LinearRegression()
        { }

        public void Reset()
        {
            m_XValues.Clear();
            m_YValues.Clear();
            m_A = 0;
            m_B = 0;
            m_Residuals = null;
            m_StdDev = double.NaN;
            s_TCoeff = double.NaN;
        }

        public void AddDataPoint(double x, double y)
        {
            m_XValues.Add(x);
            m_YValues.Add(y);
        }

        public void AddDataPoint(double x, double y, double weight)
        {
            m_XValues.Add(x);
            m_YValues.Add(y);
            m_Weights.Add(weight);
        }

        public void Solve()
        {
            if (m_Weights.Count > 0)
                SolveWithWeights();
            else
                SolveNoWeights();
        }

        public void SolveNoWeights()
        {
            if (m_XValues.Count < 3)
                throw new InvalidOperationException("Cannot get a linear fit from less than 3 points.");

            SafeMatrix A = new SafeMatrix(m_XValues.Count, 2);
            SafeMatrix X = new SafeMatrix(m_XValues.Count, 1);

            for (int i = 0; i < m_XValues.Count; i++)
            {
                A[i, 0] = m_XValues[i];
                A[i, 1] = 1;

                X[i, 0] = m_YValues[i];
            }

            SafeMatrix a_T = A.Transpose();
            SafeMatrix aa = a_T * A;
            SafeMatrix aa_inv = m_CoVarianceMatrix = aa.Inverse();
            SafeMatrix bx = (aa_inv * a_T) * X;

            m_A = bx[0, 0];
            m_B = bx[1, 0];
        }

        public void SolveWithWeights()
        {
            if (m_XValues.Count < 3)
                throw new InvalidOperationException("Cannot get a linear fit from less than 3 points.");

            SafeMatrix A = new SafeMatrix(m_XValues.Count, 2);
            SafeMatrix X = new SafeMatrix(m_XValues.Count, 1);
            SafeMatrix W = new SafeMatrix(m_XValues.Count, m_XValues.Count);

            // Normalize weights
            double medianWeight = m_Weights.Median();
            for (int i = 0; i < m_Weights.Count; i++)
                m_Weights[i] = m_Weights[i]/medianWeight;

            for (int i = 0; i < m_XValues.Count; i++)
            {
                A[i, 0] = m_XValues[i];
                A[i, 1] = 1;

                X[i, 0] = m_YValues[i];

                W[i, i] = m_Weights[i];
            }

            SafeMatrix a_T = A.Transpose();
            SafeMatrix aa = (a_T * W) * A;
            SafeMatrix aa_inv = m_CoVarianceMatrix = aa.Inverse();
            SafeMatrix bx = ((aa_inv * a_T) * W) * X;

            m_A = bx[0, 0];
            m_B = bx[1, 0];
        }

		public int NumberOfDataPoints
		{
			get { return m_YValues.Count; }
		}

        public double A
        {
            get { return m_A; }
        }

        public double B
        {
            get { return m_B; }
        }

        public double Uncertainty_A
        {
            get { return m_A_Uncertainty; }
        }

        public double Uncertainty_B
        {
            get { return m_B_Uncertainty; }
        }

        public SafeMatrix CoVarianceMatrix
        {
            get { return m_CoVarianceMatrix; }
        }

        public double ComputeY(double x)
        {
            return m_A * x + m_B;
        }

        public double ComputeYWithError(double x, out double uncertainty, double confidenceIntervalPerc = 0.95)
        {
            EnsureResiduals();

            var tDistCoeff = GetTDistributionCoeff(m_XValues.Count, 1 - confidenceIntervalPerc);

            uncertainty = tDistCoeff * m_StdDev * Math.Sqrt((1.0 / m_XValues.Count) + m_XValues.Count * Math.Pow(x - m_AverageSampleX, 2) / m_SS);

            return m_A * x + m_B;
        }

        private double m_AverageSampleX = double.NaN;
        private double m_SS = double.NaN;
        private static double s_TCoeff = double.NaN;
        private static double s_TConfidence = 0;
        private static int s_TDegFreedom = 0;

        private static double GetTDistributionCoeff(int df, double a)
        {
            if (double.IsNaN(s_TCoeff) || Math.Abs(s_TConfidence - a) < 0.0001 || s_TDegFreedom != df)
            {
                s_TCoeff = TDistribution.CalculateCriticalValue(df, a, 0.0001);
                s_TConfidence = a;
                s_TDegFreedom = df;
            }

            return s_TCoeff;
        }

        private double GetTDistribution95CoeffFromTable()
        {
            if (double.IsNaN(s_TCoeff))
            {
                int df = m_XValues.Count - 2;
                if (df >= 2000) 
                    s_TCoeff = 1.960;
                else if (s_T_Coeff_95.ContainsKey(df))
                    s_TCoeff = s_T_Coeff_95[df];
                else
                {
                    s_TCoeff = 2.000;
                    var allKeys = s_T_Coeff_95.Keys.ToArray();
                    for (int i = 30; i < s_T_Coeff_95.Count; i++)
                    {
                        if (df >= allKeys[i - 1] && df < allKeys[i])
                        {
                            s_TCoeff = s_T_Coeff_95[allKeys[i - 1]];
                            break;
                        }
                    }
                }
            }

            return s_TCoeff;
        }

        private static Dictionary<int, double> s_T_Coeff_95 = new Dictionary<int, double>()
        {
            {1, 12.706}, {2, 4.303}, {3, 3.182}, {4, 2.776}, {5, 2.571}, {6, 2.447}, {7, 2.365}, {8, 2.306}, {9, 2.262}, {10, 2.228},
            {11, 2.201}, {12, 2.179}, {13, 2.160}, {14, 2.145}, {15, 2.131}, {16, 2.120}, {17, 2.110}, {18, 2.101}, {19, 2.093}, {20, 2.086},
            {21, 2.080}, {22, 2.074}, {23, 2.069}, {24, 2.064}, {25, 2.060}, {26, 2.056}, {27, 2.052}, {28, 2.048}, {29, 2.045}, {30, 2.042},
            {60, 2.000}, {90, 1.987}, {120, 1.980}, {150, 1.976}, {180, 1.973}, {210, 1.971}, {240, 1.970}, {270, 1.969}, {300, 1.968}, {350, 1.967},
            {400, 1.966}, {450, 1.965}, {550, 1.964}, {750, 1.963}, {1000, 1.962}, {2000, 1.960}
        };

        private List<double> m_Residuals;
        private double m_StdDev = double.NaN;
        private double m_ChiSquare = double.NaN;

        public double StdDev
        {
            get
            {
                EnsureResiduals();

                return m_StdDev;
            }
        }

        public double ChiSquare
        {
            get
            {
                EnsureResiduals();

                return m_ChiSquare;
            }
        }

        public IEnumerable<double> Residuals
        {
            get { return m_Residuals;}
        }

        private void EnsureResiduals()
        {
            if (m_Residuals == null)
            {
                m_Residuals = new List<double>();
                m_StdDev = 0;
                bool hasWeights = m_Weights.Count > 0;
                // http://files.eric.ed.gov/fulltext/ED282906.pdf
                // TODO: Check page 18 of http://www.markirwin.net/stat149/Lecture/Lecture3.pdf
                double weightScaler = hasWeights ? (m_Weights.Count/m_Weights.Sum()) : 1;

                for (int i = 0; i < m_XValues.Count; i++)
                {
                    double residual = m_YValues[i] - ComputeY(m_XValues[i]);

                    m_Residuals.Add(residual);
                    var resSq = residual * residual;
                    if (hasWeights) resSq *= weightScaler * m_Weights[i];

                    m_StdDev += resSq;
                }

                var variance = hasWeights 
                    ? m_StdDev / (m_Weights.Sum() - 2) 
                    : m_StdDev / (m_XValues.Count - 2);

                m_StdDev = Math.Sqrt(variance);

                m_ChiSquare = 0;
                double stdsq = m_StdDev * m_StdDev;
                for (int i = 0; i < m_Residuals.Count; i++)
                {
                    m_ChiSquare += (m_Residuals[i] * m_Residuals[i]) / stdsq;
                }
                m_ChiSquare = m_ChiSquare / (m_Residuals.Count - 3);

                m_AverageSampleX = m_XValues.Average();

                var sumSquares = m_XValues.Sum(x => x*x);
                m_SS = m_XValues.Count * sumSquares - Math.Pow(m_XValues.Sum(), 2);

                m_B_Uncertainty = Math.Sqrt(variance * sumSquares / m_SS);
                m_A_Uncertainty = Math.Sqrt(variance * m_XValues.Count / m_SS);
            }
        }
    }
}
