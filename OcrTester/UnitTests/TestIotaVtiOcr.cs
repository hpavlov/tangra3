using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Tangra.OCR.IotaVtiOsdProcessor;

namespace OcrTester.UnitTests
{
    [TestFixture]
    public class TestIotaVtiOcr
    {
        [Test]
        [TestCase("12", 12)]
        [TestCase(" 2", 2)]
        [TestCase("1 ", 10)]
        [TestCase(" ", 0)]
        [TestCase("", 0)]
        [TestCase(null, 0)]
        public void TestIotaVtiTimeStamp_Hours(string hh, int expectedHours)
        {
            var str = new IotaVtiTimeStampStrings()
            {
                NumSat = ' ',
                HH = hh,
                MM = "",
                SS = "",
                FFFF1 = "",
                FFFF2 = "",
                FRAMENO = ""
            };

            var ts = new IotaVtiTimeStamp(str);

            Assert.AreEqual(expectedHours, ts.Hours);

        }

        [Test]
        [TestCase("12", 12)]
        [TestCase(" 2", 2)]
        [TestCase("1 ", 10)]
        [TestCase(" ", 0)]
        [TestCase("", 0)]
        [TestCase(null, 0)]
        public void TestIotaVtiTimeStamp_Minutes(string mm, int expectedMinutes)
        {
            var str = new IotaVtiTimeStampStrings()
            {
                NumSat = ' ',
                HH = "",
                MM = mm,
                SS = "",
                FFFF1 = "",
                FFFF2 = "",
                FRAMENO = ""
            };

            var ts = new IotaVtiTimeStamp(str);

            Assert.AreEqual(expectedMinutes, ts.Minutes);
        }

        [Test]
        [TestCase("12", 12)]
        [TestCase(" 2", 2)]
        [TestCase("1 ", 10)]
        [TestCase(" ", 0)]
        [TestCase("", 0)]
        [TestCase(null, 0)]
        public void TestIotaVtiTimeStamp_Seconds(string ss, int expectedSeconds)
        {
            var str = new IotaVtiTimeStampStrings()
            {
                NumSat = ' ',
                HH = "",
                MM = "",
                SS = ss,
                FFFF1 = "",
                FFFF2 = "",
                FRAMENO = ""
            };

            var ts = new IotaVtiTimeStamp(str);

            Assert.AreEqual(expectedSeconds, ts.Seconds);
        }

        [Test]
        [TestCase("1204", 1204)]
        [TestCase(" 204", 204)]
        [TestCase("120 ", 1200)]
        [TestCase("1  4", 1004)]
        [TestCase(" ", 0)]
        [TestCase("", 0)]
        public void TestIotaVtiTimeStamp_FrameNo(string frameNo, int expectedFrameNo)
        {
            var str = new IotaVtiTimeStampStrings()
            {
                NumSat = ' ',
                HH = "",
                MM = "",
                SS = "",
                FFFF1 = "",
                FFFF2 = "",
                FRAMENO = frameNo
            };

            var ts = new IotaVtiTimeStamp(str);

            Assert.AreEqual(expectedFrameNo, ts.FrameNumber);
        }

        [Test]
        [TestCase("1204", 1204)]
        [TestCase(" 204", 204)]
        [TestCase("120 ", 1200)]
        [TestCase("1  4", 1004)]
        public void TestIotaVtiTimeStamp_FFF1(string fff1, int expectedFFF)
        {
            var str = new IotaVtiTimeStampStrings()
            {
                NumSat = ' ',
                HH = "",
                MM = "",
                SS = "",
                FFFF1 = fff1,
                FFFF2 = "",
                FRAMENO = ""
            };

            var ts = new IotaVtiTimeStamp(str);

            Assert.AreEqual(expectedFFF, ts.Milliseconds10);
        }

        [Test]
        [TestCase("1204", 1204)]
        [TestCase(" 204", 204)]
        [TestCase("120 ", 1200)]
        [TestCase("1  4", 1004)]
        public void TestIotaVtiTimeStamp_FFF2(string fff2, int expectedFFF)
        {
            var str = new IotaVtiTimeStampStrings()
            {
                NumSat = ' ',
                HH = "",
                MM = "",
                SS = "",
                FFFF1 = "",
                FFFF2 = fff2,
                FRAMENO = ""
            };

            var ts = new IotaVtiTimeStamp(str);

            Assert.AreEqual(expectedFFF, ts.Milliseconds10);
        }
    }
}
