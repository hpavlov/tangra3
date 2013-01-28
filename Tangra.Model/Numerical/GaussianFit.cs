using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Tangra.Model.Numerical
{
    public class GaussianFit
    {
        private static int NumberIterations = 20;

        private List<double> m_XValues = new List<double>();
        private List<double> m_YValues = new List<double>();

        private double m_IBackground;
        private double m_IMax;
        private double m_R0;
        private double m_X0;

        public GaussianFit()
        { }

        public double IBackground
        {
            get { return m_IBackground; }
        }

        public double IMax
        {
            get { return m_IMax; }
        }

        public double R0
        {
            get { return m_R0; }
        }

        public double Sigma
        {
            get { return Math.Sqrt(2) * m_R0; }
        }

        public double X0
        {
            get { return m_X0; }
        }

        public double ValueForX(double x)
        {
            return IBackground + IMax * Math.Exp(-(m_X0 - x) * (m_X0 - x) / (m_R0 * m_R0));
        }

        public double FWHM
        {
            get { return 2 * Math.Sqrt(Math.Log(2)) * m_R0; }
        }
        public void AddPoint(double x, double y)
        {
            m_XValues.Add(x);
            m_YValues.Add(y);
        }

        public void Solve()
        {
            try
            {
                int dataCount = m_YValues.Count;

                for (int iter = NumberIterations; iter > 0; iter--)
                {
                    if (iter == NumberIterations)
                    {
                        m_IBackground = 0.0; /* assume no backgnd intensity at first... */
                        m_IMax = m_YValues.Max();
                        m_X0 = m_XValues.Average();
                        double halfMax = m_IMax / 2;
                        double x1 = double.MaxValue;
                        double x2 = double.MinValue;
                        double minX1 = double.MaxValue;
                        double minX2 = double.MaxValue;
                        for (int i = 0; i < dataCount; i++)
                        {
                            double diff = m_YValues[i] - halfMax;
                            double diffAbs = Math.Abs(diff);
                            if (m_XValues[i] < m_X0)
                            {
                                if (diffAbs < minX1)
                                {
                                    minX1 = diffAbs;
                                    x1 = m_XValues[i];
                                }
                            }
                            else
                            {
                                if (diffAbs < minX2)
                                {
                                    minX2 = diffAbs;
                                    x2 = m_XValues[i];
                                }
                            }
                        }

                        double fwhm = 0.8 * (x2 - x1);
                        m_R0 = fwhm / (2 * Math.Sqrt(Math.Log(2)));
                    }

                    double r0_squared = m_R0 * m_R0;

                    SafeMatrix A = new SafeMatrix(dataCount, 4);
                    SafeMatrix X = new SafeMatrix(dataCount, 1);

                    int index = -1;

                    for (int i = 0; i < dataCount; i++)
                    {
                        double dx = m_XValues[i] - m_X0;
                        double fx = Math.Exp(-dx * dx / r0_squared);

                        index++;

                        double exp_val = fx * fx;

                        double residual = m_IBackground + m_IMax * exp_val - m_YValues[i];
                        X[index, 0] = -residual;

                        A[index, 0] = 1.0; /* slope in i0 */
                        A[index, 1] = exp_val; /* slope in a0 */
                        A[index, 2] = 2.0 * m_IMax * dx * exp_val / r0_squared;
                        A[index, 3] = 2.0 * m_IMax * (dx * dx) * exp_val / (m_R0 * r0_squared);
                    }


                    SafeMatrix a_T = A.Transpose();
                    SafeMatrix aa = a_T * A;
                    SafeMatrix aa_inv = aa.Inverse();
                    SafeMatrix Y = (aa_inv * a_T) * X;

                    m_IBackground += Y[0, 0];
                    m_IMax += Y[1, 0];

                    for (int i = 2; i < 4; i++)
                    {
                        //#if 0
                        if (Y[i, 0] > 1.0) Y[i, 0] = 1.0;
                        if (Y[i, 0] < -1.0) Y[i, 0] = -1.0;
                        //#endif
                        //if(Math.Abs(Y[i, 0]) > 1.0)
                        //{
                        //    m_IsSolved = false;
                        //    return;
                        //}
                    }

                    m_X0 += Y[2, 0];

                    if (Y[3, 0] > m_R0 / 10.0) Y[3, 0] = m_R0 / 10.0;
                    if (Y[3, 0] < -m_R0 / 10.0) Y[3, 0] = -m_R0 / 10.0;

                    m_R0 += Y[3, 0];
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
        }
    }
}
