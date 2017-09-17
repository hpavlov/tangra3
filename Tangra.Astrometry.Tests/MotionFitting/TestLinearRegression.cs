using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Tangra.Model.Numerical;

namespace Tangra.Tests.MotionFitting
{
    [TestFixture]
    public class TestLinearRegression
    {
        [Test]
        public void Test1()
        {
            // Based on https://www.medcalc.org/manual/weighted-regression-worked-example.php

            var reg = new LinearRegression();

            double[] x_values = new double[]
            {
                27, 21, 22, 24, 25, 23, 20, 20, 29, 24, 25, 28, 26, 38, 32, 33, 31, 34, 37, 38, 33, 35, 30, 31, 37, 39, 46,
                49, 40, 42, 43, 46, 43, 44, 46, 47, 45, 49, 48, 40, 42, 55, 54, 57, 52, 53, 56, 52, 50, 59, 50, 52, 58,
                57
            };

            double[] y_values = new double[]
            {
                73, 66, 63, 75, 71, 70, 65, 70, 79, 72, 68, 67, 79, 91, 76, 69, 66, 73, 78, 87, 76, 79, 73, 80, 68, 75, 89,
                101, 70, 72, 80, 83, 75, 71, 80, 96, 92, 80, 70, 90, 85, 76, 71, 99, 86, 79, 92, 85, 71, 90, 91, 100, 80,
                109
            };

            for (int i = 0; i < x_values.Length; i++)
            {
                reg.AddDataPoint(x_values[i], y_values[i]);
            }
            reg.Solve();

            Assert.AreEqual(0.5800, reg.A, 0.0001);
            Assert.AreEqual(56.1569, reg.B, 0.0001);
            Assert.AreEqual(8.1457, reg.StdDev, 0.0001);
            Assert.AreEqual(0.09695, reg.Uncertainty_A, 0.0001);
            Assert.AreEqual(3.9937, reg.Uncertainty_B, 0.0001);

            var residuals = reg.Residuals.ToArray();

            var reg2 = new LinearRegression();
            for (int i = 0; i < x_values.Length; i++)
            {
                reg2.AddDataPoint(x_values[i], Math.Abs(residuals[i]));
            }
            reg2.Solve();

            Assert.AreEqual(0.1982, reg2.A, 0.0001);
            Assert.AreEqual(-1.5495, reg2.B, 0.0001);
            Assert.AreEqual(4.4606, reg2.StdDev, 0.0001);
            Assert.AreEqual(0.05309, reg2.Uncertainty_A, 0.0001);
            Assert.AreEqual(2.1869, reg2.Uncertainty_B, 0.0001);

            var predictedValues = new double[x_values.Length];

            for (int i = 0; i < x_values.Length; i++)
            {
                predictedValues[i] = reg2.ComputeY(x_values[i]);
            }

            var reg3 = new LinearRegression();
            var factor = 1;
            for (int i = 0; i < x_values.Length; i++)
            {
                double weight = factor / (predictedValues[i] * predictedValues[i]);
                reg3.AddDataPoint(x_values[i], y_values[i], weight);
            }
            reg3.Solve();

            Assert.AreEqual(0.5963, reg3.A, 0.0001);
            Assert.AreEqual(55.5658, reg3.B, 0.0001);
            Assert.AreEqual(1.2130, reg3.StdDevUnscaled, 0.0001);
        }

        [Test]
        public void Test2()
        {
            var reg = new LinearRegression();
            reg.AddDataPoint(1, 6);
            reg.AddDataPoint(2, 5);
            reg.AddDataPoint(3, 7);
            reg.AddDataPoint(4, 10);

            reg.Solve();

            Assert.AreEqual(1.4, reg.A, 0.001);
            Assert.AreEqual(3.5, reg.B, 0.001);
        }

        [Test]
        public void Test3()
        {
            // Based on https://onlinecourses.science.psu.edu/stat501/node/352

            double[] x_values = new double[] { 0.21, 0.20, 0.19, 0.18, 0.17, 0.16, 0.15 };
            double[] y_values = new double[] { 0.1726, 0.1707, 0.1637, 0.1640, 0.1613, 0.1617, 0.1598 };
            double[] sd_values = new double[] { 0.01988, 0.01938, 0.01896, 0.02037, 0.01654, 0.01594, 0.01763 };

            var reg_ols = new LinearRegression();
            var reg_wls = new LinearRegression();
            var reg_wls2 = new LinearRegression();

            for (int i = 0; i < x_values.Length; i++)
            {
                reg_ols.AddDataPoint(x_values[i], y_values[i]);
                reg_wls.AddDataPoint(x_values[i], y_values[i], 1 / (sd_values[i] * sd_values[i]));
                reg_wls2.AddDataPoint(x_values[i], y_values[i], 2 / (sd_values[i] * sd_values[i]));
            }

            reg_ols.Solve();
            reg_wls.Solve();
            reg_wls2.Solve();

            Assert.AreEqual(0.2048, reg_wls.A, 0.0001);
            Assert.AreEqual(0.12796, reg_wls.B, 0.0001);

            Assert.AreEqual(0.2048, reg_wls2.A, 0.0001);
            Assert.AreEqual(0.12796, reg_wls2.B, 0.0001);

            if (Math.Abs(reg_wls.StdDevUnscaled - reg_wls2.StdDevUnscaled) < 0.0001)
            {
                Assert.Fail("Proportional weights result in different StdDevs in the weighted population");
            };

            Assert.AreEqual(reg_wls.StdDev, reg_wls2.StdDev, 0.0001, "Proportional weights result in the same scaled StdDev");

            Assert.AreEqual(0.2100, reg_ols.A, 0.0001);
            Assert.AreEqual(0.12703, reg_ols.B, 0.0001);
        }

        [Test]
        public void Test4()
        {
            // Based on https://onlinecourses.science.psu.edu/stat501/node/397

            double[] x_values = new double[] { 16, 14, 22, 10, 14, 17, 10, 13, 19, 12, 18, 11 };
            double[] y_values = new double[] { 77, 70, 85, 50, 62, 70, 55, 63, 88, 57, 81, 51 };

            var reg_ols = new LinearRegression();

            for (int i = 0; i < x_values.Length; i++)
            {
                reg_ols.AddDataPoint(x_values[i], y_values[i]);
            }

            reg_ols.Solve();

            Assert.AreEqual(3.269, reg_ols.A, 0.001);
            Assert.AreEqual(19.47, reg_ols.B, 0.01);
            Assert.AreEqual(4.5983, reg_ols.StdDev, 0.0001);
            Assert.AreEqual(0.365, reg_ols.Uncertainty_A, 0.001);
            Assert.AreEqual(5.52, reg_ols.Uncertainty_B, 0.01);

            var residuals = reg_ols.Residuals.ToArray();

            var reg2 = new LinearRegression();
            for (int i = 0; i < x_values.Length; i++)
            {
                reg2.AddDataPoint(x_values[i], Math.Abs(residuals[i]));
            }
            reg2.Solve();

            var predictedValues = new double[x_values.Length];

            for (int i = 0; i < x_values.Length; i++)
            {
                predictedValues[i] = reg2.ComputeY(x_values[i]);
            }

            var reg_wls = new LinearRegression();
            var factor = 1;
            for (int i = 0; i < x_values.Length; i++)
            {
                double weight = factor / (predictedValues[i] * predictedValues[i]);
                reg_wls.AddDataPoint(x_values[i], y_values[i], weight);
            }
            reg_wls.Solve();

            Assert.AreEqual(3.421, reg_wls.A, 0.001);
            Assert.AreEqual(17.30, reg_wls.B, 0.01);
            Assert.AreEqual(1.15935, reg_wls.StdDevUnscaled, 0.00001);
        }
    }
}
