using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Tangra.Model.Helpers;

namespace Tangra.Tests.FITS
{
    [TestFixture]
    public class TestFitsNegativePixelHandling
    {
        private void RunTest(Array pixels, Type type)
        {
            uint medianVal;
            Type dataType;
            bool hasNegPix;

            int minValue;
            uint maxValue;
            int bz = 0;
            FITSHelper2.Load16BitImageData(pixels, 3, 2, bz, 0, out medianVal, out dataType, out hasNegPix, out minValue, out maxValue);

            Assert.AreEqual(type, dataType);
            Assert.AreEqual(true, hasNegPix);
            Assert.AreEqual(-13, minValue);
            Assert.AreEqual(34, maxValue);

            bz = -10;
            FITSHelper2.Load16BitImageData(pixels, 3, 2, bz, 0, out medianVal, out dataType, out hasNegPix, out minValue, out maxValue);
            Assert.AreEqual(-13, minValue);
            Assert.AreEqual(24, maxValue);


            bz = -14;
            FITSHelper2.Load16BitImageData(pixels, 3, 2, bz, 0, out medianVal, out dataType, out hasNegPix, out minValue, out maxValue);
            Assert.AreEqual(-13, minValue);
            Assert.AreEqual(20, maxValue);
            Assert.AreEqual(true, hasNegPix); // Zeroing out negative pixels doesn't change the fact that there are negative pixels
        }

        [Test]
        public void TestNegativePixelDetection_Int16()
        {
            Array pixels = new Array[3];
            pixels.SetValue(new short[2] { -3, 5 }, 0);
            pixels.SetValue(new short[2] { 0, -13 }, 1);
            pixels.SetValue(new short[2] { 34, 12 }, 2);

            RunTest(pixels, typeof(short));
        }

        [Test]
        public void TestNegativePixelDetection_Float()
        {
            Array pixels = new Array[3];
            pixels.SetValue(new float[2] { -3, 5 }, 0);
            pixels.SetValue(new float[2] { 0, -13 }, 1);
            pixels.SetValue(new float[2] { 34, 12 }, 2);

            RunTest(pixels, typeof (float));
        }
    }
}
