using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Tangra.Model.Numerical;

namespace Tangra.Astrometry.Tests.MotionFitting
{
    [TestFixture]
    public class TestTDistribution
    {
        private static Dictionary<int, double> s_T_Coeff_95 = new Dictionary<int, double>()
        {
            {4, 2.776}, {5, 2.571}, {6, 2.447}, {7, 2.365}, {8, 2.306}, {9, 2.262}, {10, 2.228},
            {11, 2.201}, {12, 2.179}, {13, 2.160}, {14, 2.145}, {15, 2.131}, {16, 2.120}, {17, 2.110}, {18, 2.101}, {19, 2.093}, {20, 2.086},
            {21, 2.080}, {22, 2.074}, {23, 2.069}, {24, 2.064}, {25, 2.060}, {26, 2.056}, {27, 2.052}, {28, 2.048}, {29, 2.045}, {30, 2.042},
            {60, 2.000}, {90, 1.987}, {120, 1.980}, {150, 1.976}, {180, 1.973}, {210, 1.971}, {240, 1.970}, {270, 1.969}, {300, 1.968}, {350, 1.967},
            {400, 1.966}, {450, 1.965}, {550, 1.964}, {750, 1.963}, {1000, 1.962}, {2000, 1.960}
        };

        [Test]
        public void TestCriticalPointCalculation()
        {
            foreach (int key in s_T_Coeff_95.Keys)
            {
                var coeff = TDistribution.CalculateCriticalValue(key, (1 - 0.95), 0.0001);
                Assert.AreEqual(s_T_Coeff_95[key], coeff, 0.002);
            }
        }
    }
}
