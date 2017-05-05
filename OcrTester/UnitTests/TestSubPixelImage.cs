using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Tangra.Model.Astro;
using Tangra.OCR.Model;

namespace OcrTester.Framework
{
    [TestFixture]
    public class TestSubPixelImage
    {
        [Test]
        public void TestSubPixelCalculation()
        {
            var sp = new SubPixelImage(new uint[] { 100, 100, 100, 100 }, 2, 2);
            for (decimal y = 0; y < 2; y += 0.2M)
            {
                for (decimal x = 0; x < 2; x += 0.2M)
                {
                    Assert.AreEqual(100, sp.GetWholePixelAt(x, y));
                }    
            }

        }

        [Test]
        public void TestSubPixelCalculations_2()
        {
            var sp = new SubPixelImage(new uint[] { 100, 60, 80, 10 }, 2, 2);
            Assert.AreEqual(100, sp.GetWholePixelAt(0, 0));

            Assert.AreEqual(64.8M, sp.GetWholePixelAt(0.4M, 0.6M));
        }
    }
}
