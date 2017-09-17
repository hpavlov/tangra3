using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;
using nom.tam.fits;
using NUnit.Framework;
using Tangra.Model.Config;
using Tangra.Model.Helpers;

namespace Tangra.Tests.FITS
{
    [TestFixture]
    public class TestFitsTimeStampReader
    {
        [Test]
        [TestCase("yyyy-MM-ddTHH:mm:ss.fff", "2017-09-13T18:04:58.121")]
        [TestCase("dd/MM/yyyy HH:mm:ss.fff", "13/09/2017 18:04:58.121")]
        public void TestTimeStampExposure(string format, string value)
        {
            var cfg = new TangraConfig.FITSFieldConfig()
            {
                IsTimeStampAndExposure = true,
                ExposureHeader = "EXP",
                ExposureUnit = TangraConfig.ExposureUnit.Milliseconds,
                TimeStampType = TangraConfig.TimeStampType.StartExposure,
                TimeStampFormat = format,
                TimeStampHeader = "TIMSTMP"
            };

            var timestampReader = new FITSTimeStampReader(cfg);

            var hdr = new Header();
            hdr.AddValue("TIMSTMP", value, "");
            hdr.AddValue("EXP", "240.6", "");

            bool isMidPoint;
            double? fitsExposure;
            var timeStamp = timestampReader.ParseExposure(null, hdr, out isMidPoint, out fitsExposure);

            Assert.AreEqual(true, isMidPoint);
            Assert.AreEqual(0.2406, fitsExposure, 0.00001);
            Assert.AreEqual(new DateTime(2017, 09, 13, 18, 04, 58).AddMilliseconds(121 + 240.6 / 2), timeStamp);
        }

        [Test]
        [TestCase(TangraConfig.TimeStampType.StartExposure, 120.3)]
        [TestCase(TangraConfig.TimeStampType.MidExposure, 0)]
        [TestCase(TangraConfig.TimeStampType.EndExposure, -120.3)]
        public void TestTimeStampType(TangraConfig.TimeStampType timeStampType, double diff)
        {
            var cfg = new TangraConfig.FITSFieldConfig()
            {
                IsTimeStampAndExposure = true,
                ExposureHeader = "EXP",
                ExposureUnit = TangraConfig.ExposureUnit.Milliseconds,
                TimeStampType = timeStampType,
                TimeStampFormat = "yyyy-MM-ddTHH:mm:ss.fff",
                TimeStampHeader = "TIMSTMP"
            };

            var timestampReader = new FITSTimeStampReader(cfg);

            var hdr = new Header();
            hdr.AddValue("TIMSTMP", "2017-09-13T18:04:58.121", "");
            hdr.AddValue("EXP", "240.6", "");

            bool isMidPoint;
            double? fitsExposure;
            var timeStamp = timestampReader.ParseExposure(null, hdr, out isMidPoint, out fitsExposure);

            Assert.AreEqual(true, isMidPoint);
            Assert.AreEqual(0.2406, fitsExposure, 0.00001);
            Assert.AreEqual(new DateTime(2017, 09, 13, 18, 04, 58).AddMilliseconds(121 + diff), timeStamp);

            cfg = new TangraConfig.FITSFieldConfig()
            {
                IsTimeStampAndExposure = true,
                TimeStampIsDateTimeParts = true,
                ExposureHeader = "EXP",
                ExposureUnit = TangraConfig.ExposureUnit.Milliseconds,
                TimeStampType = timeStampType,
                TimeStampFormat = "yyyy-MM-dd",
                TimeStampFormat2 = "HH:mm:ss.fff",
                TimeStampHeader = "OBS-DATE",
                TimeStampHeader2 = "OBS-TIME"
            };

            timestampReader = new FITSTimeStampReader(cfg);

            hdr = new Header();
            hdr.AddValue("OBS-DATE", "2017-09-13", "");
            hdr.AddValue("OBS-TIME", "18:04:58.121", "");
            hdr.AddValue("EXP", "240.6", "");

            timeStamp = timestampReader.ParseExposure(null, hdr, out isMidPoint, out fitsExposure);

            Assert.AreEqual(true, isMidPoint);
            Assert.AreEqual(0.2406, fitsExposure, 0.00001);
            Assert.AreEqual(new DateTime(2017, 09, 13, 18, 04, 58).AddMilliseconds(121 + diff), timeStamp);
        }

        [Test]
        [TestCase("yyyy-MM-ddTHH:mm:ss.fff", "2017-09-13T18:04:58.001", "2017-09-13T18:04:58.241")]
        [TestCase("dd/MM/yyyy HH:mm:ss.fff", "13/09/2017 18:04:58.001", "13/09/2017 18:04:58.241")]
        public void TestStartEndTimeStamp(string format, string value, string value2)
        {
            var cfg = new TangraConfig.FITSFieldConfig()
            {
                IsTimeStampAndExposure = false,
                TimeStampType = TangraConfig.TimeStampType.StartExposure,
                TimeStampFormat = format,
                TimeStampHeader = "TIMSTMP",
                TimeStamp2Format = format,
                TimeStamp2Header = "TIMSTMP2"
            };

            var timestampReader = new FITSTimeStampReader(cfg);

            var hdr = new Header();
            hdr.AddValue("TIMSTMP", value, "");
            hdr.AddValue("TIMSTMP2", value2, "");

            bool isMidPoint;
            double? fitsExposure;
            var timeStamp = timestampReader.ParseExposure(null, hdr, out isMidPoint, out fitsExposure);

            Assert.AreEqual(true, isMidPoint);
            Assert.AreEqual(0.240, fitsExposure, 0.0001);
            Assert.AreEqual(new DateTime(2017, 09, 13, 18, 04, 58).AddMilliseconds(121), timeStamp);
        }

        [Test]
        [TestCase("yyyy-MM-dd", "2017-09-13", "HH:mm:ss.fff", "18:04:58.121")]
        [TestCase("dd/MM/yyyy", "13/09/2017", "HH:mm:ss.fff", "18:04:58.121")]
        [TestCase("yyyy-MM-dd", "2017-09-13", FITSTimeStampReader.SECONDS_FROM_MIDNIGHT, "65098.121")]
        [TestCase("dd/MM/yyyy", "13/09/2017", FITSTimeStampReader.SECONDS_FROM_MIDNIGHT, "65098.121")]
        public void TestDateTimeExposure(string format, string value, string format2, string value2)
        {
            var cfg = new TangraConfig.FITSFieldConfig()
            {
                IsTimeStampAndExposure = true,
                TimeStampIsDateTimeParts = true,
                ExposureHeader = "EXP",
                ExposureUnit = TangraConfig.ExposureUnit.Milliseconds,
                TimeStampType = TangraConfig.TimeStampType.StartExposure,
                TimeStampFormat = format,
                TimeStampFormat2 = format2,
                TimeStampHeader = "OBS-DATE",
                TimeStampHeader2 = "OBS-TIME"

            };

            var timestampReader = new FITSTimeStampReader(cfg);

            var hdr = new Header();
            hdr.AddValue("OBS-DATE", value, "");
            hdr.AddValue("OBS-TIME", value2, "");
            hdr.AddValue("EXP", "240.6", "");

            bool isMidPoint;
            double? fitsExposure;
            var timeStamp = timestampReader.ParseExposure(null, hdr, out isMidPoint, out fitsExposure);

            Assert.AreEqual(true, isMidPoint);
            Assert.AreEqual(0.2406, fitsExposure, 0.00001);
            Assert.AreEqual(new DateTime(2017, 09, 13, 18, 04, 58).AddMilliseconds(121 + 240.6 / 2), timeStamp);
        }

        [Test]
        [TestCase("yyyy-MM-dd", "2017-09-13", "HH:mm:ss.fff", "18:04:58.001", "yyyy-MM-dd", "2017-09-13", "HH:mm:ss.fff", "18:04:58.241")]
        [TestCase("dd/MM/yyyy", "13/09/2017", "HH:mm:ss.fff", "18:04:58.001", "dd/MM/yyyy", "13/09/2017", "HH:mm:ss.fff", "18:04:58.241")]
        [TestCase("dd/MM/yyyy", "13/09/2017", FITSTimeStampReader.SECONDS_FROM_MIDNIGHT, "65098.001", "dd/MM/yyyy", "13/09/2017", "HH:mm:ss.fff", "18:04:58.241")]
        [TestCase("dd/MM/yyyy", "13/09/2017", FITSTimeStampReader.SECONDS_FROM_MIDNIGHT, "65098.001", "dd/MM/yyyy", "13/09/2017", FITSTimeStampReader.SECONDS_FROM_MIDNIGHT, "65098.241")]
        public void TestDateTimeStartEnd(string formatS, string valueS, string formatS2, string valueS2, string formatE, string valueE, string formatE2, string valueE2)
        {
            var cfg = new TangraConfig.FITSFieldConfig()
            {
                IsTimeStampAndExposure = false,
                TimeStampIsDateTimeParts = true,
                TimeStampType = TangraConfig.TimeStampType.StartExposure,
                TimeStampFormat = formatS,
                TimeStampFormat2 = formatS2,
                TimeStampHeader = "OBS-DATE",
                TimeStampHeader2 = "OBS-TIME",
                TimeStamp2IsDateTimeParts = true,
                TimeStamp2Format = formatE,
                TimeStamp2Format2 = formatE2,
                TimeStamp2Header = "END-DATE",
                TimeStamp2Header2 = "END-TIME"

            };

            var timestampReader = new FITSTimeStampReader(cfg);

            var hdr = new Header();
            hdr.AddValue("OBS-DATE", valueS, "");
            hdr.AddValue("OBS-TIME", valueS2, "");
            hdr.AddValue("END-DATE", valueE, "");
            hdr.AddValue("END-TIME", valueE2, "");

            bool isMidPoint;
            double? fitsExposure;
            var timeStamp = timestampReader.ParseExposure(null, hdr, out isMidPoint, out fitsExposure);

            Assert.AreEqual(true, isMidPoint);
            Assert.AreEqual(0.240, fitsExposure, 0.0001);
            Assert.AreEqual(new DateTime(2017, 09, 13, 18, 04, 58).AddMilliseconds(121), timeStamp);
        }
    }
}
