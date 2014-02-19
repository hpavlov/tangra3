using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.Model.Numerical
{
    public class LinearRegression
    {
        private List<double> m_XValues = new List<double>();
        private List<double> m_YValues = new List<double>();

        private double m_A = 0;
        private double m_B = 0;

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
        }

        public void AddDataPoint(double x, double y)
        {
            m_XValues.Add(x);
            m_YValues.Add(y);
        }

        public void Solve()
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
            SafeMatrix aa_inv = aa.Inverse();
            SafeMatrix bx = (aa_inv * a_T) * X;

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

        public double ComputeY(double x)
        {
            return m_A * x + m_B;
        }

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

        private void EnsureResiduals()
        {
            if (m_Residuals == null)
            {
                m_Residuals = new List<double>();
                m_StdDev = 0;

                for (int i = 0; i < m_XValues.Count; i++)
                {
                    double residual = m_YValues[i] - ComputeY(m_XValues[i]);
                    m_Residuals.Add(residual);
                    m_StdDev += residual * residual;
                }

                m_StdDev = Math.Sqrt(m_StdDev / (m_XValues.Count - 1));

                m_ChiSquare = 0;
                double stdsq = m_StdDev * m_StdDev;
                for (int i = 0; i < m_Residuals.Count; i++)
                {
                    m_ChiSquare += (m_Residuals[i] * m_Residuals[i]) / stdsq;
                }
                m_ChiSquare = m_ChiSquare / (m_Residuals.Count - 3);
            }            
        }
    }
}
