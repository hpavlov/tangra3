using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private SafeMatrix m_WeightMatrix;

        public void SolveWithWeights()
        {
            if (m_XValues.Count < 3)
                throw new InvalidOperationException("Cannot get a linear fit from less than 3 points.");

            SafeMatrix A = new SafeMatrix(m_XValues.Count, 2);
            SafeMatrix X = new SafeMatrix(m_XValues.Count, 1);
            m_WeightMatrix = new SafeMatrix(m_XValues.Count, m_XValues.Count);

            for (int i = 0; i < m_XValues.Count; i++)
            {
                A[i, 0] = m_XValues[i];
                A[i, 1] = 1;

                X[i, 0] = m_YValues[i];

                m_WeightMatrix[i, i] = m_Weights[i];
            }

            SafeMatrix a_T = A.Transpose();
            SafeMatrix aa = (a_T * m_WeightMatrix) * A;
            SafeMatrix aa_inv = m_CoVarianceMatrix = aa.Inverse();
            SafeMatrix bx = ((aa_inv * a_T) * m_WeightMatrix) * X;

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
            get
            {
                EnsureResiduals();

                return m_A_Uncertainty;
            }
        }

        public double Uncertainty_B
        {
            get
            {
                EnsureResiduals();

                return m_B_Uncertainty;
            }
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

        private List<double> m_Residuals;
        private double m_StdDev = double.NaN;
        private double m_StdDevUnscaled = double.NaN;
        private double m_ChiSquare = double.NaN;

        public double StdDev
        {
            get
            {
                EnsureResiduals();

                return m_StdDev;
            }
        }

        public double StdDevUnscaled
        {
            get
            {
                EnsureResiduals();

                return m_StdDevUnscaled;
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

        public IEnumerable<double> Weights
        {
            get { return m_Weights; }
        }

        public IEnumerable<double> XValues
        {
            get { return m_XValues; }
        }

        public IEnumerable<double> YValues
        {
            get { return m_YValues; }
        }

        private void EnsureResiduals()
        {
            if (m_Residuals == null)
            {
                m_Residuals = new List<double>();
                m_StdDev = 0;
                bool hasWeights = m_Weights.Count > 0;

                // For discussions about estimating standard residual error in WLS see, http://files.eric.ed.gov/fulltext/ED282906.pdf
                // We adopt a scaling solution as described in: http://stats.stackexchange.com/questions/73966/in-weighted-least-squares-how-do-i-weight-the-residuals-to-get-an-accurate-z-s
                double weightMeanScaler = hasWeights ? (m_Weights.Sum() / m_Weights.Count) : 1;

                for (int i = 0; i < m_XValues.Count; i++)
                {
                    double residual = m_YValues[i] - ComputeY(m_XValues[i]);

                    m_Residuals.Add(residual);
                    var resSq = residual * residual;
                    if (hasWeights) resSq *= m_Weights[i];

                    m_StdDev += resSq;
                }

                var variance = m_StdDev / (m_XValues.Count - 2);

                m_StdDevUnscaled = m_StdDev = Math.Sqrt(variance);
                if (hasWeights)
                    m_StdDev = Math.Sqrt(variance/weightMeanScaler);

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

                if (hasWeights)
                {
                    // Variance proportionality constant, as described in:
                    // http://www.markirwin.net/stat149/Lecture/Lecture3.pdf, Page 18
                    // http://stats.stackexchange.com/questions/138938/correct-standard-errors-for-weighted-linear-regression
                    //
                    // Computed as: Var(beta) = Sigma^2 * (X'WX)^-1
                    // However computation did not return a meaningful standard errpr

                    //SafeMatrix X = new SafeMatrix(m_Residuals.Count, 1);
                    //for (int i = 0; i < m_Residuals.Count; i++)
                    //{
                    //    X[i, 0] = m_XValues[i];
                    //}

                    //SafeMatrix X_T = X.Transpose();
                    //SafeMatrix xtw = X_T * m_WeightMatrix;
                    //SafeMatrix mm = xtw * X;
                    //SafeMatrix mm_inv = mm.Inverse();

                    //var varProp  = Math.Sqrt(variance * mm_inv[0, 0]);
                    //Trace.WriteLine(varProp);
                }
            }
        }
    }
}
