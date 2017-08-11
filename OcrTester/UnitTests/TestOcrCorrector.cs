using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Tangra.Model.Config;
using Tangra.OCR.TimeExtraction;

namespace OcrTester.UnitTests
{
    [TestFixture]
    public class TestIVtiTimeStampExtensions
    {
        [Test]
        public void TestGetTicks()
        {
            var ts = new MockedTimeStamp()
            {
                Year = 2017,
                Month = 7,
                Day = 9,
                Hours = 15,
                Minutes = 56,
                Seconds = 34,
                Milliseconds10 = 2308
            } as IVtiTimeStamp;

            var calcTicks = ts.GetTicks();
            Assert.AreEqual("2017-07-09 15:56:34.231", new DateTime(calcTicks).ToString("yyyy-MM-dd HH:mm:ss.fff"));
        }

        [Test]
        public void TestGetTicks_CornerCaseJan1()
        {
            var ts = new MockedTimeStamp()
            {
                Year = 2017,
                Month = 1,
                Day = 9,
                Hours = 15,
                Minutes = 56,
                Seconds = 34,
                Milliseconds10 = 2308
            } as IVtiTimeStamp;

            var calcTicks = ts.GetTicks();
            Assert.AreEqual("2017-01-09 15:56:34.231", new DateTime(calcTicks).ToString("yyyy-MM-dd HH:mm:ss.fff"));

            ts = new MockedTimeStamp()
            {
                Year = 2017,
                Month = 12,
                Day = 1,
                Hours = 15,
                Minutes = 56,
                Seconds = 34,
                Milliseconds10 = 2308
            };

            calcTicks = ts.GetTicks();
            Assert.AreEqual("2017-12-01 15:56:34.231", new DateTime(calcTicks).ToString("yyyy-MM-dd HH:mm:ss.fff"));

            ts = new MockedTimeStamp()
            {
                Year = 2017,
                Month = 1,
                Day = 1,
                Hours = 15,
                Minutes = 56,
                Seconds = 34,
                Milliseconds10 = 2308
            };

            calcTicks = ts.GetTicks();
            Assert.AreEqual("2017-01-01 15:56:34.231", new DateTime(calcTicks).ToString("yyyy-MM-dd HH:mm:ss.fff"));
        }

        [Test]
        public void TestGetTicks_InvalidDateTimeValuesDontCauseCrash()
        {
            var ts = new MockedTimeStamp()
            {
                Year = 2017,
                Month = 13,
                Day = 34,
                Hours = 32,
                Minutes = 75,
                Seconds = 92,
                Milliseconds10 = 991829
            } as IVtiTimeStamp;

            var calcTicks = ts.GetTicks();
            Assert.AreNotEqual("2017-13-34 32:75:92.992", new DateTime(calcTicks).ToString("yyyy-MM-dd HH:mm:ss.fff"));
        }
    }

    public class MockedTimeStamp : IVtiTimeStamp
    {
        public bool ContainsFrameNumbers { get; set; }

        public int FrameNumber { get; set; }

        public int Hours { get; set; }

        public int Minutes { get; set; }

        public int Seconds { get; set; }

        public int Milliseconds10 { get; set; }

        public bool ContainsDate { get; set; }

        public int Year { get; set; }

        public int Month { get; set; }

        public int Day { get; set; }

        public string OcredCharacters { get; set; }

        public void Correct(int hours, int minutes, int seconds, int milliseconds10)
        {
            Hours = hours;
            Minutes = minutes;
            Seconds = seconds;
            Milliseconds10 = milliseconds10;
        }

        public void CorrectDate(int year, int month, int day)
        {
            Year = year;
            Month = month;
            Day = day;
        }

        public void CorrectFrameNumber(int frameNo)
        {
            FrameNumber = frameNo;
        }
    }

    [TestFixture]
    public class TestOcrCorrector
    {
        private IVtiTimeStamp MockIOTAVTITimeStamp(int hours, int minutes, int seconds, int milliseconds, int? frameNo = 0)
        {
            return new MockedTimeStamp()
            {
                Hours = hours,
                Minutes = minutes,
                Seconds = seconds,
                Milliseconds10 = 10 * milliseconds,
                FrameNumber = frameNo != null ? frameNo.Value : 0,
                ContainsDate = false,
                ContainsFrameNumbers = true
            };
        }

        [SetUp]
        public void SetUp()
        {
            TangraConfig.Settings = new TangraConfig();
            TangraConfig.Settings.Generic.OcrMaxNumberDigitsToAutoCorrect = 3;
        }

        [Test]
        [TestCase(true, false, 052.7, 1000, 072.7, 1001, 092.7, 0, 112.7, 1003, 1002, 1003, null)]
        [TestCase(true, false, 052.7, 1000, 072.7, 1001, 092.7, 1002, 112.7, 0, 1002, 1003, null)]
        [TestCase(true, true, 072.7, 1000, 052.7, 999, 112.7, 1002, 092.7, 0, 1002, 1001, null)]
        [TestCase(true, true, 072.7, 1000, 052.7, 999, 112.7, 0, 092.7, 1001, 1002, 1001, null)]
        [TestCase(false, false, 052.7, 1000, 069.4, 1001, 86.1, 0, 102.8, 1003, 1002, 1003, null)]
        [TestCase(false, false, 052.7, 1000, 069.4, 1001, 86.1, 1002, 102.8, 0, 1002, 1003, null)]
        [TestCase(false, true, 069.4, 1000, 052.7, 999, 102.8, 1002, 86.1, 0, 1002, 1001, null)]
        [TestCase(false, true, 069.4, 1000, 052.7, 999, 102.8, 0, 86.1, 1001, 1002, 1001, null)]
        [TestCase(true, false, 052.7, 1000, 192.7, 1007, 212.7, 0, 352.7, 1015, 1008, 1015, 8)]
        [TestCase(true, true, 192.7, 1007, 052.7, 1000, 352.7, 1015, 212.7, 0, 1015, 1008, 8)]
        public void CorrectBadFrameNumber(
            bool pal, bool evenBeforeOdd, 
            double poms, int pofn, double pems, int pefn, 
            double oms, int ofn, double ems, int efn,
            int eofn, int eefn, int? aavIntFields)
        {
            var corr = new OcrTimeStampCorrector();
            corr.Reset(pal ? VideoFormat.PAL : VideoFormat.NTSC);

            DateTime tsPrevOdd = new DateTime(01, 01, 01, 10, 23, 45).AddMilliseconds(poms);
            DateTime tsPrevEven = new DateTime(01, 01, 01, 10, 23, 45).AddMilliseconds(pems);
            corr.RegisterSuccessfulTimestamp(1,
                MockIOTAVTITimeStamp(tsPrevOdd.Hour, tsPrevOdd.Minute, tsPrevOdd.Second, tsPrevOdd.Millisecond, pofn),
                MockIOTAVTITimeStamp(tsPrevEven.Hour, tsPrevEven.Minute, tsPrevEven.Second, tsPrevEven.Millisecond, pefn),
                tsPrevOdd, tsPrevEven);

            DateTime tsOdd = new DateTime(01, 01, 01, 10, 23, 45).AddMilliseconds(oms);
            DateTime tsEven = new DateTime(01, 01, 01, 10, 23, 45).AddMilliseconds(ems);
            var nfoOdd = MockIOTAVTITimeStamp(tsOdd.Hour, tsOdd.Minute, tsOdd.Second, tsOdd.Millisecond, ofn);
            var nfoEven = MockIOTAVTITimeStamp(tsEven.Hour, tsEven.Minute, tsEven.Second, tsEven.Millisecond, efn);

            string debugInfo;
            bool corrected = corr.TryToCorrect(2, 1, aavIntFields, nfoOdd, nfoEven, evenBeforeOdd, ref tsOdd, ref tsEven, out debugInfo);

            Assert.IsTrue(corrected);
            Assert.AreEqual(eofn, nfoOdd.FrameNumber);
            Assert.AreEqual(eefn, nfoEven.FrameNumber);            
        }

        [Test]
        [TestCase(true, false, 052.7, 072.7, 10, 23, 45, 092.7, 18, 23, 45, 112.7, null, "10:23:45.093", "10:23:45.113")]
        [TestCase(true, false, 052.7, 072.7, 10, 23, 45, 092.7, 18, 23, 55, 112.7, null, "10:23:45.093", "10:23:45.113")]
        [TestCase(true, false, 052.7, 072.7, 10, 23, 45, 092.7, 18, 23, 55, 172.7, null, "10:23:45.093", "10:23:45.113")]
        [TestCase(true, false, 052.7, 072.7, 10, 23, 45, 892.7, 10, 23, 45, 112.7, null, "10:23:45.093", "10:23:45.113")]
        [TestCase(true, false, 052.7, 072.7, 18, 23, 45, 892.7, 10, 23, 45, 112.7, null, "10:23:45.093", "10:23:45.113")]
        [TestCase(true, false, 052.7, 072.7, 10, 23, 45, 882.7, 10, 23, 45, 112.7, null, "10:23:45.093", "10:23:45.113")]
        [TestCase(true, true, 072.7, 052.7, 18, 23, 45, 112.7, 10, 23, 45, 092.7, null, "10:23:45.113", "10:23:45.093")]
        [TestCase(true, true, 072.7, 052.7, 18, 23, 46, 112.7, 10, 23, 45, 092.7, null, "10:23:45.113", "10:23:45.093")]
        [TestCase(true, true, 072.7, 052.7, 18, 13, 45, 712.7, 10, 23, 45, 092.7, null, "10:23:45.113", "10:23:45.093")]
        [TestCase(true, true, 072.7, 052.7, 10, 23, 45, 112.7, 10, 23, 45, 892.7, null, "10:23:45.113", "10:23:45.093")]
        [TestCase(true, true, 072.7, 052.7, 10, 23, 45, 112.7, 10, 23, 45, 882.7, null, "10:23:45.113", "10:23:45.093")]
        [TestCase(true, true, 072.7, 052.7, 10, 23, 45, 112.7, 10, 23, 55, 882.7, null, "10:23:45.113", "10:23:45.093")]
        [TestCase(false, false, 052.7, 069.4, 10, 23, 45, 86.1, 18, 23, 45, 102.8, null, "10:23:45.086", "10:23:45.103")]
        [TestCase(false, false, 052.7, 069.4, 10, 23, 45, 86.1, 18, 23, 45, 182.8, null, "10:23:45.086", "10:23:45.103")]
        [TestCase(false, false, 052.7, 069.4, 10, 23, 45, 86.1, 18, 23, 46, 182.8, null, "10:23:45.086", "10:23:45.103")]
        [TestCase(false, false, 052.7, 069.4, 10, 23, 45, 986.1, 10, 23, 45, 102.8, null, "10:23:45.086", "10:23:45.103")]
        [TestCase(false, false, 052.7, 069.4, 18, 23, 45, 986.1, 10, 23, 45, 102.8, null, "10:23:45.086", "10:23:45.103")]
        [TestCase(false, false, 052.7, 069.4, 18, 23, 46, 986.1, 10, 23, 45, 102.8, null, "10:23:45.086", "10:23:45.103")]
        [TestCase(false, true, 069.4, 052.7, 18, 23, 45, 102.8, 10, 23, 45, 86.1, null, "10:23:45.103", "10:23:45.086")]
        [TestCase(false, true, 069.4, 052.7, 18, 23, 46, 102.8, 10, 23, 45, 86.1, null, "10:23:45.103", "10:23:45.086")]
        [TestCase(false, true, 069.4, 052.7, 18, 23, 46, 182.8, 10, 23, 45, 86.1, null, "10:23:45.103", "10:23:45.086")]
        [TestCase(false, true, 069.4, 052.7, 10, 23, 45, 102.8, 10, 23, 46, 86.1, null, "10:23:45.103", "10:23:45.086")]
        [TestCase(false, true, 069.4, 052.7, 10, 23, 45, 102.8, 10, 23, 46, 186.1, null, "10:23:45.103", "10:23:45.086")]
        [TestCase(false, true, 069.4, 052.7, 10, 23, 45, 102.8, 10, 23, 46, 181.1, null, "10:23:45.103", "10:23:45.086")]
        [TestCase(true, false, 052.7, 192.7, 10, 23, 45, 212.7, 18, 23, 46, 352.7, 8, "10:23:45.213", "10:23:45.353")]
        [TestCase(true, true, 192.7, 052.7, 18, 23, 46, 352.7, 10, 23, 45, 212.7, 8, "10:23:45.353", "10:23:45.213")]
        public void CorrectUpTo3DigitDifferencesInSingleField(
            bool pal, bool evenBeforeOdd,
            double poms, double pems,
            int oh, int om, int os, double oms,
            int eh, int em, int es, double ems, int? aavIntFields,
            string eof, string eef)
        {
            var corr = new OcrTimeStampCorrector();
            corr.Reset(pal ? VideoFormat.PAL : VideoFormat.NTSC);

            DateTime tsPrevOdd = new DateTime(01, 01, 01, 10, 23, 45).AddMilliseconds(poms);
            DateTime tsPrevEven = new DateTime(01, 01, 01, 10, 23, 45).AddMilliseconds(pems);

            corr.RegisterSuccessfulTimestamp(1,
                MockIOTAVTITimeStamp(tsPrevOdd.Hour, tsPrevOdd.Minute, tsPrevOdd.Second, tsPrevOdd.Millisecond, evenBeforeOdd ? 1001 : 1000),
                MockIOTAVTITimeStamp(tsPrevEven.Hour, tsPrevEven.Minute, tsPrevEven.Second, tsPrevEven.Millisecond, evenBeforeOdd ? 1000 : 1001),
                tsPrevOdd, tsPrevEven);

            DateTime tsOdd = new DateTime(01, 01, 01, oh, om, os).AddMilliseconds(oms);
            DateTime tsEven = new DateTime(01, 01, 01, eh, em, es).AddMilliseconds(ems);
            var nfoOdd = MockIOTAVTITimeStamp(tsOdd.Hour, tsOdd.Minute, tsOdd.Second, tsOdd.Millisecond, 0);
            var nfoEven = MockIOTAVTITimeStamp(tsEven.Hour, tsEven.Minute, tsEven.Second, tsEven.Millisecond, 0);

            string debugInfo;
            bool corrected = corr.TryToCorrect(2, 1, aavIntFields, nfoOdd, nfoEven, evenBeforeOdd, ref tsOdd, ref tsEven, out debugInfo);

            Assert.IsTrue(corrected);
            Assert.AreEqual(eof, string.Format("{0}:{1}:{2}.{3:000}", nfoOdd.Hours, nfoOdd.Minutes, nfoOdd.Seconds, nfoOdd.Milliseconds10 / 10.0));
            Assert.AreEqual(eef, string.Format("{0}:{1}:{2}.{3:000}", nfoEven.Hours, nfoEven.Minutes, nfoEven.Seconds, nfoEven.Milliseconds10 / 10.0));
        }

        [Test]
        public void NoCorrectionsRequired()
        {
            var corr = new OcrTimeStampCorrector();
            corr.Reset(VideoFormat.PAL);

            DateTime tsPrevOdd = new DateTime(01, 01, 01, 10, 23, 45).AddMilliseconds(100);
            DateTime tsPrevEven = new DateTime(01, 01, 01, 10, 23, 45).AddMilliseconds(120);
            corr.RegisterSuccessfulTimestamp(1,
                MockIOTAVTITimeStamp(tsPrevOdd.Hour, tsPrevOdd.Minute, tsPrevOdd.Second, tsPrevOdd.Millisecond, 1),
                MockIOTAVTITimeStamp(tsPrevEven.Hour, tsPrevEven.Minute, tsPrevEven.Second, tsPrevEven.Millisecond, 2),
                tsPrevOdd, tsPrevEven);

            DateTime tsOdd = new DateTime(01, 01, 01, 10, 23, 45).AddMilliseconds(140);
            DateTime tsEven = new DateTime(01, 01, 01, 10, 23, 45).AddMilliseconds(160);
            var nfoOdd = MockIOTAVTITimeStamp(tsOdd.Hour, tsOdd.Minute, tsOdd.Second, tsOdd.Millisecond, 3);
            var nfoEven = MockIOTAVTITimeStamp(tsEven.Hour, tsEven.Minute, tsEven.Second, tsEven.Millisecond, 4);

            string debugInfo;
            bool corrected = corr.TryToCorrect(2, 1, null, nfoOdd, nfoEven, false, ref tsOdd, ref tsEven, out debugInfo);

            Assert.IsTrue(corrected);
            Assert.AreEqual("10:23:45.140", string.Format("{0}:{1}:{2}.{3:000}", nfoOdd.Hours, nfoOdd.Minutes, nfoOdd.Seconds, nfoOdd.Milliseconds10 / 10.0));
            Assert.AreEqual("10:23:45.160", string.Format("{0}:{1}:{2}.{3:000}", nfoEven.Hours, nfoEven.Minutes, nfoEven.Seconds, nfoEven.Milliseconds10 / 10.0));
            Assert.AreEqual(3, nfoOdd.FrameNumber);
            Assert.AreEqual(4, nfoEven.FrameNumber);  
        }

        [Test]
        public void MoreThanThreeCorrectionsAreRejected()
        {
            var corr = new OcrTimeStampCorrector();
            corr.Reset(VideoFormat.PAL);

            DateTime tsPrevOdd = new DateTime(01, 01, 01, 10, 23, 45).AddMilliseconds(100);
            DateTime tsPrevEven = new DateTime(01, 01, 01, 10, 23, 45).AddMilliseconds(120);
            corr.RegisterSuccessfulTimestamp(1,
                MockIOTAVTITimeStamp(tsPrevOdd.Hour, tsPrevOdd.Minute, tsPrevOdd.Second, tsPrevOdd.Millisecond, 1),
                MockIOTAVTITimeStamp(tsPrevEven.Hour, tsPrevEven.Minute, tsPrevEven.Second, tsPrevEven.Millisecond, 2),
                tsPrevOdd, tsPrevEven);

            DateTime tsOdd = new DateTime(01, 01, 01, 10, 23, 45).AddMilliseconds(140);
            DateTime tsEven = new DateTime(01, 01, 01, 18, 33, 45).AddMilliseconds(768);
            var nfoOdd = MockIOTAVTITimeStamp(tsOdd.Hour, tsOdd.Minute, tsOdd.Second, tsOdd.Millisecond, 3);
            var nfoEven = MockIOTAVTITimeStamp(tsEven.Hour, tsEven.Minute, tsEven.Second, tsEven.Millisecond, 4);

            string debugInfo;
            bool corrected = corr.TryToCorrect(2, 1, null, nfoOdd, nfoEven, false, ref tsOdd, ref tsEven, out debugInfo);

            Assert.IsFalse(corrected);
        }

        //[Test]
        // TODO: Refactor the whole time extraction to allow for invalid dates, hours, minutes etc
        public void NonRepresentableTimeIsAllowed()
        {
            var corr = new OcrTimeStampCorrector();
            corr.Reset(VideoFormat.PAL);

            DateTime tsPrevOdd = new DateTime(01, 01, 01, 10, 23, 45).AddMilliseconds(100);
            DateTime tsPrevEven = new DateTime(01, 01, 01, 10, 23, 45).AddMilliseconds(120);
            corr.RegisterSuccessfulTimestamp(1,
                MockIOTAVTITimeStamp(tsPrevOdd.Hour, tsPrevOdd.Minute, tsPrevOdd.Second, tsPrevOdd.Millisecond, 1),
                MockIOTAVTITimeStamp(tsPrevEven.Hour, tsPrevEven.Minute, tsPrevEven.Second, tsPrevEven.Millisecond, 2),
                tsPrevOdd, tsPrevEven);

            DateTime tsOdd = new DateTime(01, 01, 01, 10, 23, 45).AddMilliseconds(140);
            var nfoOdd = MockIOTAVTITimeStamp(tsOdd.Hour, tsOdd.Minute, tsOdd.Second, tsOdd.Millisecond, 3);
            var nfoEven = MockIOTAVTITimeStamp(70, 93, 75, 160, 4);

            string debugInfo;
            DateTime tsEven = new DateTime();
            bool corrected = corr.TryToCorrect(2, 1, null, nfoOdd, nfoEven, false, ref tsOdd, ref tsEven, out debugInfo);

            Assert.IsFalse(corrected);
            Assert.AreEqual("10:23:45.160", string.Format("{0}:{1}:{2}.{3:000}", nfoEven.Hours, nfoEven.Minutes, nfoEven.Seconds, nfoEven.Milliseconds10 / 10.0));
        }

        [Test]
        public void DuplicatedFrame()
        {
            var corr = new OcrTimeStampCorrector();
            corr.Reset(VideoFormat.PAL);

            DateTime tsPrevOdd = new DateTime(01, 01, 01, 10, 23, 45).AddMilliseconds(100);
            DateTime tsPrevEven = new DateTime(01, 01, 01, 10, 23, 45).AddMilliseconds(120);
            corr.RegisterSuccessfulTimestamp(1,
                MockIOTAVTITimeStamp(tsPrevOdd.Hour, tsPrevOdd.Minute, tsPrevOdd.Second, tsPrevOdd.Millisecond, 1),
                MockIOTAVTITimeStamp(tsPrevEven.Hour, tsPrevEven.Minute, tsPrevEven.Second, tsPrevEven.Millisecond, 2),
                tsPrevOdd, tsPrevEven);

            DateTime tsOdd = new DateTime(01, 01, 01, 10, 23, 45).AddMilliseconds(100);
            DateTime tsEven = new DateTime(01, 01, 01, 10, 23, 45).AddMilliseconds(120);
            var nfoOdd = MockIOTAVTITimeStamp(tsOdd.Hour, tsOdd.Minute, tsOdd.Second, tsOdd.Millisecond, 1);
            var nfoEven = MockIOTAVTITimeStamp(tsEven.Hour, tsEven.Minute, tsEven.Second, tsEven.Millisecond, 2);

            string debugInfo;
            bool corrected = corr.TryToCorrect(2, 1, null, nfoOdd, nfoEven, false, ref tsOdd, ref tsEven, out debugInfo);

            Assert.IsTrue(corrected);
            Assert.AreEqual("10:23:45.100", string.Format("{0}:{1}:{2}.{3:000}", nfoOdd.Hours, nfoOdd.Minutes, nfoOdd.Seconds, nfoOdd.Milliseconds10 / 10.0));
            Assert.AreEqual("10:23:45.120", string.Format("{0}:{1}:{2}.{3:000}", nfoEven.Hours, nfoEven.Minutes, nfoEven.Seconds, nfoEven.Milliseconds10 / 10.0));
            Assert.AreEqual(3, nfoOdd.FrameNumber);
            Assert.AreEqual(4, nfoEven.FrameNumber);
        }

        [Test]
        [TestCase(true, 100, 120, 180, 200)]
        [TestCase(false, 100, 117, 167, 183)]
        public void DroppedFrame(bool isPal, int pms1, int pms2, int ms1, int ms2)
        {
            var corr = new OcrTimeStampCorrector();
            corr.Reset(isPal ? VideoFormat.PAL : VideoFormat.NTSC);

            DateTime tsPrevOdd = new DateTime(01, 01, 01, 10, 23, 45).AddMilliseconds(pms1);
            DateTime tsPrevEven = new DateTime(01, 01, 01, 10, 23, 45).AddMilliseconds(pms2);
            corr.RegisterSuccessfulTimestamp(1,
                MockIOTAVTITimeStamp(tsPrevOdd.Hour, tsPrevOdd.Minute, tsPrevOdd.Second, tsPrevOdd.Millisecond, 1),
                MockIOTAVTITimeStamp(tsPrevEven.Hour, tsPrevEven.Minute, tsPrevEven.Second, tsPrevEven.Millisecond, 2),
                tsPrevOdd, tsPrevEven);

            DateTime tsOdd = new DateTime(01, 01, 01, 10, 23, 45).AddMilliseconds(ms1);
            DateTime tsEven = new DateTime(01, 01, 01, 10, 23, 45).AddMilliseconds(ms2);
            var nfoOdd = MockIOTAVTITimeStamp(tsOdd.Hour, tsOdd.Minute, tsOdd.Second, tsOdd.Millisecond, 5);
            var nfoEven = MockIOTAVTITimeStamp(tsEven.Hour, tsEven.Minute, tsEven.Second, tsEven.Millisecond, 6);

            string debugInfo;
            bool corrected = corr.TryToCorrect(2, 1, null, nfoOdd, nfoEven, false, ref tsOdd, ref tsEven, out debugInfo);

            Assert.IsTrue(corrected);
            Assert.AreEqual(string.Format("10:23:45.{0:000}", ms1), string.Format("{0}:{1}:{2}.{3:000}", nfoOdd.Hours, nfoOdd.Minutes, nfoOdd.Seconds, nfoOdd.Milliseconds10 / 10.0));
            Assert.AreEqual(string.Format("10:23:45.{0:000}", ms2), string.Format("{0}:{1}:{2}.{3:000}", nfoEven.Hours, nfoEven.Minutes, nfoEven.Seconds, nfoEven.Milliseconds10 / 10.0));
            Assert.AreEqual(3, nfoOdd.FrameNumber);
            Assert.AreEqual(4, nfoEven.FrameNumber);
        }

        
        [Test]
        [TestCase(true, false, 18, 25, 05, 088.7, 18, 2, 5, 808.6, null, "18:25:05.789", "18:25:05.809")]
        [TestCase(true, true, 18, 2, 5, 808.6, 18, 25, 05, 088.7, null, "18:25:05.809", "18:25:05.789")]
        public void TestTwoCorrectionsAtTheSameTime(
            bool pal, bool evenBeforeOdd,
            int oh, int om, int os, double oms,
            int eh, int em, int es, double ems, int? aavIntFields,
            string eof, string eef)
        {
            var corr = new OcrTimeStampCorrector();
            corr.Reset(pal ? VideoFormat.PAL : VideoFormat.NTSC);

            DateTime tsPrevOdd = new DateTime(01, 01, 01, 18, 25, 05).AddMilliseconds(evenBeforeOdd ? 769 : 749);
            DateTime tsPrevEven = new DateTime(01, 01, 01, 18, 25, 05).AddMilliseconds(evenBeforeOdd ? 749 : 769);

            corr.RegisterSuccessfulTimestamp(1,
                MockIOTAVTITimeStamp(tsPrevOdd.Hour, tsPrevOdd.Minute, tsPrevOdd.Second, tsPrevOdd.Millisecond, evenBeforeOdd ? 1001 : 1000),
                MockIOTAVTITimeStamp(tsPrevEven.Hour, tsPrevEven.Minute, tsPrevEven.Second, tsPrevEven.Millisecond, evenBeforeOdd ? 1000 : 1001),
                tsPrevOdd, tsPrevEven);

            DateTime tsOdd = new DateTime(01, 01, 01, oh, om, os).AddMilliseconds(oms);
            DateTime tsEven = new DateTime(01, 01, 01, eh, em, es).AddMilliseconds(ems);
            var nfoOdd = MockIOTAVTITimeStamp(tsOdd.Hour, tsOdd.Minute, tsOdd.Second, tsOdd.Millisecond, 0);
            var nfoEven = MockIOTAVTITimeStamp(tsEven.Hour, tsEven.Minute, tsEven.Second, tsEven.Millisecond, 0);

            string debugInfo;
            bool corrected = corr.TryToCorrect(2, 1, aavIntFields, nfoOdd, nfoEven, evenBeforeOdd, ref tsOdd, ref tsEven, out debugInfo);

            Assert.IsTrue(corrected);
            Assert.AreEqual(eof, string.Format("{0:00}:{1:00}:{2:00}.{3:000}", nfoOdd.Hours, nfoOdd.Minutes, nfoOdd.Seconds, nfoOdd.Milliseconds10 / 10.0));
            Assert.AreEqual(eef, string.Format("{0:00}:{1:00}:{2:00}.{3:000}", nfoEven.Hours, nfoEven.Minutes, nfoEven.Seconds, nfoEven.Milliseconds10 / 10.0));
        }

        [Test]
        public void TestUserSubmittedFailedCorrections()
        {
            // TODO:
            //ORC ERR: FrameNo: 227, Odd Timestamp: 3:55:8.995 140698, Even Timestamp: 3:55:8.1152 140699, NTSC field is not 16.68 ms. It is 15 ms. IOTA-VTI Correction Attempt for Frame 227. 03:55:08.0995 (140698) - 03:55:08.1152 (140699)
            //ORC ERR: FrameNo: 228, Odd Timestamp: 3:55:8.1319 140700, Even Timestamp: 3:55:8.1496 140701, NTSC field is not 16.68 ms. It is 18 ms. IOTA-VTI Correction Attempt for Frame 228. 03:55:08.1319 (140700) - 03:55:08.1496 (140701)
            //ORC ERR: FrameNo: 230, Odd Timestamp: 3:55:8.1996 140704, Even Timestamp: 3:55:8.2153 140705, NTSC field is not 16.68 ms. It is 15 ms. IOTA-VTI Correction Attempt for Frame 230. 03:55:08.1996 (140704) - 03:55:08.2153 (140705)
            //ORC ERR: FrameNo: 231, Odd Timestamp: 3:55:8.2320 140706, Even Timestamp: 3:55:8.2497 140707, NTSC field is not 16.68 ms. It is 18 ms. IOTA-VTI Correction Attempt for Frame 231. 03:55:08.2320 (140706) - 03:55:08.2497 (140707)
            //ORC ERR: FrameNo: 721, Odd Timestamp: 3:55:24.5818 141686, Even Timestamp: 3:55:24.5995 141687, NTSC field is not 16.68 ms. It is 18 ms. IOTA-VTI Correction Attempt for Frame 721. 03:55:24.5818 (141686) - 03:55:24.5995 (141687)
            //ORC ERR: FrameNo: 724, Odd Timestamp: 3:55:24.6819 141692, Even Timestamp: 3:55:24.6996 141693, NTSC field is not 16.68 ms. It is 18 ms. IOTA-VTI Correction Attempt for Frame 724. 03:55:24.6819 (141692) - 03:55:24.6996 (141693)
            //ORC ERR: FrameNo: 727, Odd Timestamp: 3:55:24.7820 141698, Even Timestamp: 3:55:24.7997 141699, NTSC field is not 16.68 ms. It is 18 ms. IOTA-VTI Correction Attempt for Frame 727. 03:55:24.7820 (141698) - 03:55:24.7997 (141699)
            //ORC ERR: FrameNo: 730, Odd Timestamp: 3:55:24.8821 141704, Even Timestamp: 3:55:24.8998 141705, NTSC field is not 16.68 ms. It is 18 ms. IOTA-VTI Correction Attempt for Frame 730. 03:55:24.8821 (141704) - 03:55:24.8998 (141705)
            //ORC ERR: FrameNo: 733, Odd Timestamp: 3:55:24.9822 141710, Even Timestamp: 3:55:24.9999 141711, NTSC field is not 16.68 ms. It is 18 ms. IOTA-VTI Correction Attempt for Frame 733. 03:55:24.9822 (141710) - 03:55:24.9999 (141711)
            //ORC ERR: FrameNo: 1219, Odd Timestamp: 3:55:41.1995 142682, Even Timestamp: 3:55:41.2152 142683, NTSC field is not 16.68 ms. It is 15 ms. IOTA-VTI Correction Attempt for Frame 1219. 03:55:41.1995 (142682) - 03:55:41.2152 (142683)

            //ORC ERR: FrameNo: 724, Odd Timestamp: 3:55:24.6819 141692, Even Timestamp: 3:55:24.6996 141693, NTSC field is not 16.68 ms. It is 18 ms. IOTA-VTI Correction Attempt for Frame 724. 03:55:24.6819 (141692) - 03:55:24.6996 (141693)

            //OCR ERR: FrameNo: 3643, Odd Timestamp: 2:14:26.7739 207286, Even Timestamp: 2:14:26.7999 207287, PAL field is not 20 ms. It is 26 ms. IOTA-VTI Correction Attempt for Frame 3643. 02:14:26.7739 (207286) - 02:14:26.7999 (207287). FrameStep: 1
            //OCR ERR: FrameNo: 3651, Odd Timestamp: 2:14:27.999 207302, Even Timestamp: 2:14:27.1139 207309, PAL field is not 20 ms. It is 14 ms. IOTA-VTI Correction Attempt for Frame 3651. 02:14:27.0999 (207302) - 02:14:27.1139 (207309). FrameStep: 1
            //OCR ERR: FrameNo: 3653, Odd Timestamp: 2:14:27.1739 207306, Even Timestamp: 2:14:27.1999 207307, PAL field is not 20 ms. It is 26 ms. IOTA-VTI Correction Attempt for Frame 3653. 02:14:27.1739 (207306) - 02:14:27.1999 (207307). FrameStep: 1
            //OCR ERR: FrameNo: 3663, Odd Timestamp: 2:14:27.5739 207326, Even Timestamp: 2:14:27.5999 207327, PAL field is not 20 ms. It is 26 ms. IOTA-VTI Correction Attempt for Frame 3663. 02:14:27.5739 (207326) - 02:14:27.5999 (207327). FrameStep: 1
            //OCR ERR: FrameNo: 3666, Odd Timestamp: 2:14:27.6999 207332, Even Timestamp: 2:14:27.7139 207333, PAL field is not 20 ms. It is 14 ms. IOTA-VTI Correction Attempt for Frame 3666. 02:14:27.6999 (207332) - 02:14:27.7139 (207333). FrameStep: 1
            //OCR ERR: FrameNo: 3668, Odd Timestamp: 2:14:27.7739 207336, Even Timestamp: 2:14:27.7999 207337, PAL field is not 20 ms. It is 26 ms. IOTA-VTI Correction Attempt for Frame 3668. 02:14:27.7739 (207336) - 02:14:27.7999 (207337). FrameStep: 1
            //OCR ERR: FrameNo: 3673, Odd Timestamp: 2:14:27.9739 207346, Even Timestamp: 2:14:27.9999 207347, PAL field is not 20 ms. It is 26 ms. IOTA-VTI Correction Attempt for Frame 3673. 02:14:27.9739 (207346) - 02:14:27.9999 (207347). FrameStep: 1
            //OCR ERR: FrameNo: 3674, Odd Timestamp: 2:14:28.139 207348, Even Timestamp: 2:14:28.939 207349, PAL field is not 20 ms. It is 80 ms. IOTA-VTI Correction Attempt for Frame 3674. 02:14:28.0139 (207348) - 02:14:28.0939 (207349). FrameStep: 1
            //OCR ERR: FrameNo: 3676, Odd Timestamp: 2:14:28.999 207352, Even Timestamp: 2:14:28.1139 207353, PAL field is not 20 ms. It is 14 ms. IOTA-VTI Correction Attempt for Frame 3676. 02:14:28.0999 (207352) - 02:14:28.1139 (207353). FrameStep: 1
            //OCR ERR: FrameNo: 3683, Odd Timestamp: 2:14:28.3739 207366, Even Timestamp: 2:14:28.3999 207367, PAL field is not 20 ms. It is 26 ms. IOTA-VTI Correction Attempt for Frame 3683. 02:14:28.3739 (207366) - 02:14:28.3999 (207367). FrameStep: 1
            //OCR ERR: FrameNo: 3686, Odd Timestamp: 2:14:28.4999 207372, Even Timestamp: 2:14:28.5139 207373, PAL field is not 20 ms. It is 14 ms. IOTA-VTI Correction Attempt for Frame 3686. 02:14:28.4999 (207372) - 02:14:28.5139 (207373). FrameStep: 1
            //OCR ERR: FrameNo: 3691, Odd Timestamp: 2:14:28.6999 207382, Even Timestamp: 2:14:28.7139 207383, PAL field is not 20 ms. It is 14 ms. IOTA-VTI Correction Attempt for Frame 3691. 02:14:28.6999 (207382) - 02:14:28.7139 (207383). FrameStep: 1
            //OCR ERR: FrameNo: 3696, Odd Timestamp: 2:14:28.8999 207392, Even Timestamp: 2:14:28.9139 207399, PAL field is not 20 ms. It is 14 ms. IOTA-VTI Correction Attempt for Frame 3696. 02:14:28.8999 (207392) - 02:14:28.9139 (207399). FrameStep: 1
            //OCR ERR: FrameNo: 3698, Odd Timestamp: 2:14:28.9739 207396, Even Timestamp: 2:14:28.9999 207397, PAL field is not 20 ms. It is 26 ms. IOTA-VTI Correction Attempt for Frame 3698. 02:14:28.9739 (207396) - 02:14:28.9999 (207397). FrameStep: 1
            //OCR ERR: FrameNo: 3699, Odd Timestamp: 2:14:29.139 207398, Even Timestamp: 2:14:29.939 207399, PAL field is not 20 ms. It is 80 ms. IOTA-VTI Correction Attempt for Frame 3699. 02:14:29.0139 (207398) - 02:14:29.0939 (207399). FrameStep: 1
            //OCR ERR: FrameNo: 3703, Odd Timestamp: 2:14:29.1739 207406, Even Timestamp: 2:14:29.1999 207407, PAL field is not 20 ms. It is 26 ms. IOTA-VTI Correction Attempt for Frame 3703. 02:14:29.1739 (207406) - 02:14:29.1999 (207407). FrameStep: 1
        }
    }

    // TODO: Blank timestamp followed by incorrectly ORC-ed frame. Would that be causing a lot of wrong corrections?

    // TODO: Test stepped measurements ?? Is this used in astrometry ?
    
}
