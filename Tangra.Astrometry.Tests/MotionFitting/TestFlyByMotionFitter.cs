using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using NUnit.Framework;
using Tangra.Model.Helpers;
using Tangra.MotionFitting;

namespace Tangra.Tests.MotionFitting
{
    public class TestCaseConfiguration
    {
        public MovementExpectation Motion { get; set; }
        public string RAHours { get; set; }
        public string DEDeg { get; set; }
        public string MPCTimeString { get; set; }
        public double Rate { get; set; }

        [XmlIgnore]
        public double RA
        {
            get { return AstroConvert.ToRightAcsension(RAHours); }
        }

        [XmlIgnore]
        public double DE
        {
            get { return AstroConvert.ToDeclination(DEDeg); }
        }

        [XmlIgnore]
        public DateTime Time
        {
            get
            {
                int dspos = MPCTimeString.IndexOf('.');
                var rv = DateTime.ParseExact(MPCTimeString.Substring(0, dspos), "yyyy MM dd", CultureInfo.InvariantCulture);

                rv = rv.AddDays(double.Parse("0" + MPCTimeString.Substring(dspos), CultureInfo.InvariantCulture));

                return rv;
            }
        }
    }

    public class TestCaseData
    {
        public TestCaseConfiguration TestConfig;
        public FlybyMeasurementContext MeasurementContext;
        public FittingContext FittingContext;
        public List<SingleMultiFrameMeasurement> Measurements = new List<SingleMultiFrameMeasurement>();
    }

    [TestFixture]
    public class TestFlyByMotionFitter
    {
        private TestCaseData LoadTestCaseData(int testCaseId)
        {
            var rv = new TestCaseData();

            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream str = assembly.GetManifestResourceStream(string.Format("Tangra.Astrometry.Tests.MotionFitting.TestCasesData.TestCase{0}.xml", testCaseId)))
            using (TextReader rdr = new StreamReader(str))
            {
                string xmlStr = rdr.ReadToEnd();
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlStr);

                var tcSer = new XmlSerializer(typeof (TestCaseConfiguration));
                var mcSer = new XmlSerializer(typeof(FlybyMeasurementContext));
                var fcSer = new XmlSerializer(typeof(FittingContext));
                var smfmSer = new XmlSerializer(typeof(SingleMultiFrameMeasurement));

                rv.TestConfig = (TestCaseConfiguration)tcSer.Deserialize(new StringReader(xmlDoc.DocumentElement["TestCaseConfiguration"].OuterXml));
                rv.MeasurementContext = (FlybyMeasurementContext)mcSer.Deserialize(new StringReader(xmlDoc.DocumentElement["FlybyMeasurementContext"].OuterXml));
                rv.FittingContext = (FittingContext)fcSer.Deserialize(new StringReader(xmlDoc.DocumentElement["FittingContext"].OuterXml));
                foreach (XmlElement node in xmlDoc.DocumentElement["Mesurements"])
                {
                    rv.Measurements.Add((SingleMultiFrameMeasurement)smfmSer.Deserialize(new StringReader(node.OuterXml)));
                }
            }

            return rv;
        }

        [Test]
        [TestCase(1)]
        public void RunEmbeddedTestCases(int testCaseId)
        {
            var data = LoadTestCaseData(testCaseId);
            var fitter = new FlyByMotionFitter();

            double motionRate;

            if (data.TestConfig.Motion == MovementExpectation.SlowFlyby)
            {
                var raFit = fitter.FitAndPlotSlowFlyby(
                    data.Measurements.ToDictionary(x => x.FrameNo, y => y),
                    data.MeasurementContext,
                    data.FittingContext,
                    FittingValue.RA,
                    null, null, null, 0, 0, 0, 0, out motionRate);

                Assert.AreEqual(data.TestConfig.RA * 15.0 * 3600, raFit.FittedValue * 3600, 0.1);
                Assert.AreEqual(data.TestConfig.Rate, motionRate, 0.01);

                TimeSpan timeDiff = data.TestConfig.Time - raFit.FittedValueTime;
                Assert.AreEqual(0, timeDiff.TotalMilliseconds, 0.5,
                    string.Format("Normal Time Difference. Expected: {0:0.0000000}, Actual: {1:0.0000000}",
                        GetFractionalDays(data.TestConfig.Time),
                        GetFractionalDays(raFit.FittedValueTime)));

                var deFit = fitter.FitAndPlotSlowFlyby(
                    data.Measurements.ToDictionary(x => x.FrameNo, y => y),
                    data.MeasurementContext,
                    data.FittingContext,
                    FittingValue.DEC,
                    null, null, null, 0, 0, 0, 0, out motionRate);

                Assert.AreEqual(data.TestConfig.DE * 3600, deFit.FittedValue * 3600, 0.1);

                #region Derive motion rate, coordinates on the first frame and compute expected position at returned normal time
                double ra0 = 0;
                double de0 = 0;
                int count = 0;
                int frameIdFromIntPeriodOfMinFrame = (data.MeasurementContext.MinFrameNo - data.FittingContext.FirstFrameIdInIntegrationPeroid) % data.FittingContext.IntegratedFramesCount;
                int lastFrameIdFromFirstIntPeriod = data.MeasurementContext.MinFrameNo - frameIdFromIntPeriodOfMinFrame + data.FittingContext.IntegratedFramesCount;
                for (int i = 0; i < data.FittingContext.IntegratedFramesCount - frameIdFromIntPeriodOfMinFrame; i++)
                {
                    if (data.Measurements[i].FrameNo <= lastFrameIdFromFirstIntPeriod)
                    {
                        ra0 += data.Measurements[i].RADeg;
                        de0 += data.Measurements[i].DEDeg;
                        count++;
                    }
                }
                // Use Mean rather than Median
                ra0 /= count;
                de0 /= count;
                double ra1 = 0;
                double de1 = 0;
                int count1 = 0;
                double ra2 = 0;
                double de2 = 0;
                int count2 = 0;
                int? lastIntervalId = null;
                int? secondLastIntervalId = null;
                for (int i = data.Measurements.Count - 1; i >= 0; i--)
                {
                    int interval = (data.Measurements[i].FrameNo - data.FittingContext.FirstFrameIdInIntegrationPeroid) / data.FittingContext.IntegratedFramesCount;
                    if (lastIntervalId == null || lastIntervalId.Value == interval)
                    {
                        ra1 += data.Measurements[i].RADeg;
                        de1 += data.Measurements[i].DEDeg;
                        lastIntervalId = interval;
                        count1++;
                    }
                    else if (secondLastIntervalId == null || secondLastIntervalId.Value == interval)
                    {
                        ra2 += data.Measurements[i].RADeg;
                        de2 += data.Measurements[i].DEDeg;
                        secondLastIntervalId = interval;
                        count2++;
                    }
                    else
                        break;
                }

                if (lastIntervalId != null && secondLastIntervalId != null)
                {
                    ra1 /= count1;
                    de1 /= count1;
                    ra2 /= count2;
                    de2 /= count2;
                    int numIntervals = lastIntervalId.Value - (data.MeasurementContext.MinFrameNo - data.FittingContext.FirstFrameIdInIntegrationPeroid) / data.FittingContext.IntegratedFramesCount;
                    if (count2 > count1)
                    {
                        ra1 = ra2;
                        de1 = de2;
                        numIntervals = secondLastIntervalId.Value - (data.MeasurementContext.MinFrameNo - data.FittingContext.FirstFrameIdInIntegrationPeroid) / data.FittingContext.IntegratedFramesCount;
                    }
                    double secondsDiff = numIntervals * data.FittingContext.IntegratedFramesCount * (1.0 / data.FittingContext.FrameRate);

                    double raRateArcSecPerSec = (ra1 - ra0) * 3600 / secondsDiff;
                    double deRateArcSecPerSec = (de1 - de0) * 3600 / secondsDiff;

                    TimeSpan diffNormalMinusStartTime = raFit.FittedValueTime - data.FittingContext.FirstFrameUtcTime.AddSeconds(-1 * data.FittingContext.InstrumentalDelay);
                    double raAtNormalTimeArcSec = ra0 * 3600 + raRateArcSecPerSec * diffNormalMinusStartTime.TotalSeconds;
                    double deAtNormalTimeArcSec = de0 * 3600 + deRateArcSecPerSec * diffNormalMinusStartTime.TotalSeconds;

                    double fittedRaArcSec = raFit.FittedValue * 3600;
                    double fittedDecArcSec = deFit.FittedValue * 3600;

                    Trace.WriteLine(string.Format("RA-Diff={0}\", DEC-Diff-{1}\"", Math.Abs(raAtNormalTimeArcSec - fittedRaArcSec), Math.Abs(deAtNormalTimeArcSec - fittedDecArcSec)));

                    // TODO: Improve this to have more meaningful sigmas for asserting equal results

                    double oneSigmaRA = Math.Sqrt(data.Measurements.Sum(x => x.StdDevRAArcSec * x.StdDevRAArcSec) / data.Measurements.Count);
                    double oneSigmaDE = Math.Sqrt(data.Measurements.Sum(x => x.StdDevDEArcSec * x.StdDevDEArcSec) / data.Measurements.Count);

                    Assert.AreEqual(raAtNormalTimeArcSec, fittedRaArcSec, oneSigmaRA);
                    Assert.AreEqual(deAtNormalTimeArcSec, fittedDecArcSec, oneSigmaDE);
                }
                #endregion
            }
        }

        [Test]
        [TestCase(16, 16, 25.0, 0.35, false, null)]
        [TestCase(16, 8, 25.0, 0.35, false, null)]
        [TestCase(16, 4, 25.0, 0.35, false, null)]
        [TestCase(16, 2, 25.0, 0.35, false, null)]
        [TestCase(16, 1, 25.0, 0.35, false, null)]
        [TestCase(16, 16, 25.0, 0.35, false, 0)]
        [TestCase(16, 16, 25.0, 0.35, false, 1)]
        [TestCase(16, 16, 25.0, 0.35, false, 2)]
        [TestCase(16, 16, 25.0, 0.35, false, 3)]
        [TestCase(16, 16, 25.0, 0.35, false, 4)]
        [TestCase(16, 16, 25.0, 0.35, false, 5)]
        [TestCase(16, 16, 25.0, 0.35, false, 6)]
        [TestCase(16, 16, 25.0, 0.35, false, 7)]
        [TestCase(16, 16, 25.0, 0.35, false, 8)]
        [TestCase(16, 16, 25.0, 0.35, false, 9)]
        [TestCase(16, 16, 25.0, 0.35, false, 10)]
        [TestCase(16, 16, 25.0, 0.35, false, 11)]
        [TestCase(16, 16, 25.0, 0.35, false, 12)]
        [TestCase(16, 16, 25.0, 0.35, false, 13)]
        [TestCase(16, 16, 25.0, 0.35, false, 14)]
        [TestCase(16, 16, 25.0, 0.35, false, 15)]
        [TestCase(16, 16, 29.97, 0.29, false, null)]
        [TestCase(16, 8, 29.97, 0.29, false, null)]
        [TestCase(16, 4, 29.97, 0.29, false, null)]
        [TestCase(16, 2, 29.97, 0.29, false, null)]
        [TestCase(16, 1, 29.97, 0.29, false, null)]
        [TestCase(2, 2, 25.0, 0.07, false, null)]
        [TestCase(4, 4, 25.0, 0.11, false, null)]
        [TestCase(8, 8, 25.0, 0.19, false, null)]
        [TestCase(32, 32, 25.0, 0.67, false, null)]
        [TestCase(64, 64, 25.0, 1.31, false, null)]
        [TestCase(128, 128, 25.0, 2.59, false, null)]
        public void TestInstrumentalDelayInAVI(int integratedFrames, int measureFramesPerInterval, double frameRate, double instrumentalDelaySec, bool missFirstFrame, int? clickedFrameIndex)
        {
            // Simulated 10 integrated frames with integration starting at frame 100
            // Simulated motion in RA with rate 2.34567s per minute
            // Simulated motion in DEC with rate 34.5678" per minute

            double frameDurationSec = 1.0 / frameRate;
            const double raArcSecRatePerMinute = 2.34567;
            double raArcSecRatePerIntegrationPeriod = integratedFrames * frameDurationSec * raArcSecRatePerMinute / 60.0;
            const double startingRaDeg = 123.0;
            const int firstFrameId = 100;
            const int numIntegrationIntervalsMeasured = 10;
            DateTime firstIntegratedFrameRealMidExposureUT = DateTime.ParseExact("2016 09 08 13:45:04.718", "yyyy MM dd HH:mm:ss.fff", CultureInfo.InvariantCulture);

            const double decArcSecRatePerMinute = 34.5678;
            double decArcSecRatePerIntegrationPeriod = integratedFrames * frameDurationSec * decArcSecRatePerMinute / 60.0;
            const double startingDecDeg = 42.0;

            DateTime midExpWithDelay = firstIntegratedFrameRealMidExposureUT.AddSeconds(instrumentalDelaySec);
            DateTime endFirstFieldTimeStamp = midExpWithDelay;

            var measurements = new Dictionary<int, SingleMultiFrameMeasurement>();
            
            var fittingContext = new FittingContext()
            {
                AavStackedMode = false,
                FirstFrameIdInIntegrationPeroid = firstFrameId,
                FirstFrameUtcTime = endFirstFieldTimeStamp,
                FrameRate = frameRate,
                FrameTimeType = FrameTimeType.TimeStampOfFirstIntegratedFrame,
                InstrumentalDelay = instrumentalDelaySec,
                InstrumentalDelayUnits = InstrumentalDelayUnits.Seconds,
                IntegratedFramesCount = integratedFrames,
                MovementExpectation = MovementExpectation.SlowFlyby,
                NativeVideoFormat = null,
                ObjectExposureQuality = ObjectExposureQuality.GoodSignal
            };

            var meaContext = new FlybyMeasurementContext()
            {
                FirstVideoFrame = 0, /* First frame is in the whole video file */
                MaxStdDev = 0,
                UserMidValue = 0 /* This is used for plotting only */
            };

            for (int i = 0; i < numIntegrationIntervalsMeasured; i++)
            {
                double de = startingDecDeg + (decArcSecRatePerIntegrationPeriod * i) / 3600;
                double ra = startingRaDeg + (raArcSecRatePerIntegrationPeriod * i) / 3600;

                for (int f = 0; f < measureFramesPerInterval; f++)
                {
                    var mea = new SingleMultiFrameMeasurement()
                    {
                        RADeg = ra,
                        DEDeg = de,
                        FrameNo = firstFrameId + i * integratedFrames + f,
                        Mag = 14
                    };

                    measurements.Add(mea.FrameNo, mea);
                }
            }

            if (missFirstFrame)
                // Simulate a prevoous buggy condition: Missed first frame
                measurements.Remove(firstFrameId);

            meaContext.MinFrameNo = measurements.Keys.Min();
            meaContext.MaxFrameNo = measurements.Keys.Max();
            if (clickedFrameIndex == null)
                // When not specified explicitely compute a normal position close to the last measured frame
                meaContext.UserMidFrame = meaContext.MaxFrameNo;
            else
                meaContext.UserMidFrame = meaContext.MinFrameNo + clickedFrameIndex.Value;

            var fitter = new FlyByMotionFitter();
            double motionRate;

            var raFit = fitter.FitAndPlotSlowFlyby(
                measurements, meaContext, fittingContext, FittingValue.RA,
                null, null, null, 0, 0, 0, 0, out motionRate);

            var deFit = fitter.FitAndPlotSlowFlyby(
                measurements, meaContext, fittingContext, FittingValue.DEC,
                null, null, null, 0, 0, 0, 0, out motionRate);

            Assert.IsNotNull(raFit);
            Assert.IsNotNull(deFit);

            Assert.IsTrue(raFit.IsVideoNormalPosition);
            Assert.IsTrue(deFit.IsVideoNormalPosition);

            #region Compute Expected Normal Position and Time of Normal Position
            
            int userFrameIntegrationInterval = (meaContext.UserMidFrame - firstFrameId) / integratedFrames;
            double userFrameIntegrationIntervalMidFrame = firstFrameId + userFrameIntegrationInterval * integratedFrames + integratedFrames / 2 - (frameDurationSec / 2 /* Correction for finding the middle even frames block */);
            DateTime userFrameIntegrationIntervalMidExposureRealTimeStamp = firstIntegratedFrameRealMidExposureUT.AddSeconds(userFrameIntegrationInterval * integratedFrames * frameDurationSec);
            
            double userFrameOffsetFromMidExposureSec = (meaContext.UserMidFrame - userFrameIntegrationIntervalMidFrame) * frameDurationSec;
            DateTime userFrameRealProjectedTimeStamp = userFrameIntegrationIntervalMidExposureRealTimeStamp.AddSeconds(userFrameOffsetFromMidExposureSec);

            double fractionalDaysProjectedTimeStamp = GetFractionalDays(userFrameRealProjectedTimeStamp);
            
            double fractionalDaysClosestNormalTimeStamp = Math.Round(fractionalDaysProjectedTimeStamp * 1E6) / 1E6;
            DateTime closestNormalTimeStamp =
                new DateTime(userFrameRealProjectedTimeStamp.Year, userFrameRealProjectedTimeStamp.Month,
                    userFrameRealProjectedTimeStamp.Day).AddDays(fractionalDaysClosestNormalTimeStamp);
            #endregion

            TimeSpan diffExpectedMinusFitted = closestNormalTimeStamp - raFit.FittedValueTime;
            if (diffExpectedMinusFitted.TotalMilliseconds > 0.5)
            {
                diffExpectedMinusFitted = closestNormalTimeStamp.AddDays(0.000001) - raFit.FittedValueTime;
                if (diffExpectedMinusFitted.TotalMilliseconds > 0.5)
                {
                    diffExpectedMinusFitted = closestNormalTimeStamp.AddDays(-0.000001) - raFit.FittedValueTime;
                    if (diffExpectedMinusFitted.TotalMilliseconds > 0.5)
                    {
                        Assert.Fail("Normal Time Difference. Expected: {0:0.0000000}, Actual: {1:0.0000000}",
                                GetFractionalDays(closestNormalTimeStamp),
                                GetFractionalDays(raFit.FittedValueTime));
                    }
                }
            }

            // Make sure we calculate the position for the selected normal frame by the fitter, which may be 1 microday away from ours
            closestNormalTimeStamp = raFit.FittedValueTime;

            TimeSpan diffNormalMinusStartTime = closestNormalTimeStamp - firstIntegratedFrameRealMidExposureUT;
            double raAtNormalTimeArcSec = startingRaDeg * 3600 + raArcSecRatePerMinute * diffNormalMinusStartTime.TotalMinutes;
            double deAtNormalTimeArcSec = startingDecDeg * 3600 + decArcSecRatePerMinute * diffNormalMinusStartTime.TotalMinutes;

            double fittedRaArcSec = raFit.FittedValue * 3600;
            double fittedDecArcSec = deFit.FittedValue * 3600;

            Trace.WriteLine(string.Format("RA-Diff={0}\", DEC-Diff-{1}\"", Math.Abs(raAtNormalTimeArcSec - fittedRaArcSec), Math.Abs(deAtNormalTimeArcSec - fittedDecArcSec)));

            Assert.AreEqual(raAtNormalTimeArcSec, fittedRaArcSec, 0.1 /* 0.1 arcsec */);
            Assert.AreEqual(deAtNormalTimeArcSec, fittedDecArcSec, 0.1 /* 0.1 arcsec */);
        }

        private double GetFractionalDays(DateTime time)
        {
            return (time.Hour + (time.Minute + (time.Second + time.Millisecond / 1000.0) / 60.0) / 60.0) / 24.0;
        }
    }
}
