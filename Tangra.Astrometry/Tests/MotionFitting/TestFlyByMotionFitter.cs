using System;
using System.Collections.Generic;
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
            using (Stream str = assembly.GetManifestResourceStream(string.Format("Tangra.Tests.MotionFitting.TestCasesData.TestCase{0}.xml", testCaseId)))
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

                Assert.AreEqual(data.TestConfig.RA * 15.0, raFit.FittedValue, 0.1 / 3600);
                Assert.AreEqual(data.TestConfig.Rate, motionRate, 0.01);
                Assert.AreEqual(data.TestConfig.Time.Ticks, raFit.FittedValueTime.Ticks, TimeSpan.FromDays(0.0000001).Ticks);

                var deFit = fitter.FitAndPlotSlowFlyby(
                    data.Measurements.ToDictionary(x => x.FrameNo, y => y),
                    data.MeasurementContext,
                    data.FittingContext,
                    FittingValue.DEC,
                    null, null, null, 0, 0, 0, 0, out motionRate);

                Assert.AreEqual(data.TestConfig.DE, deFit.FittedValue, 0.1 / 3600);
            }
        }
    }
}
