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
        [Test]
        public void TestTimeStampExposure()
        {
            uint medianVal;
            Type dataType;
            bool hasNegPix;

            Array pixels = new Array[3];
            pixels.SetValue(new short[2] { -3, 5 }, 0);
            pixels.SetValue(new short[2] { 0, -13 }, 1);
            pixels.SetValue(new short[2] { 34, 12 }, 2);

            FITSHelper.Load16BitImageData(pixels, false, 3, 2, 0, out medianVal, out dataType, out hasNegPix);
        }
    }
}
