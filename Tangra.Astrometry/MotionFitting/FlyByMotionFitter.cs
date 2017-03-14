using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using Tangra.Model.Config;
using Tangra.Model.Context;
using Tangra.Model.Helpers;
using Tangra.Model.Image;
using Tangra.Model.Numerical;
using Tangra.Model.Video;

namespace Tangra.MotionFitting
{
    public enum FrameTimeType
    {
        NonIntegratedFrameTime,
        TimeStampOfFirstIntegratedFrame
    }

    public enum InstrumentalDelayUnits
    {
        Frames,
        Seconds
    }

    public enum MovementExpectation
    {
        Slow,  // < 2"/min
        SlowFlyby, // (2"/min, 200.0"/min)
        FastFlyby // > 200"/min
    }

    public enum ObjectExposureQuality
    {
        GoodSignal,
        Underexposed,
        Trailed
    }

    public class FittingContext
    {
        public DateTime FirstFrameUtcTime { get; set; }
        public int FirstFrameIdInIntegrationPeroid { get; set; }
        public double FrameRate { get; set; }
        public MovementExpectation MovementExpectation { get; set; }
        public FrameTimeType FrameTimeType { get; set; }
        public double InstrumentalDelay { get; set; }
        public InstrumentalDelayUnits InstrumentalDelayUnits { get; set; }
        public int IntegratedFramesCount { get; set; }
        public bool AavStackedMode { get; set; }
        public VideoFileFormat VideoFileFormat { get; set; }
        public string NativeVideoFormat { get; set; }
        public ObjectExposureQuality ObjectExposureQuality { get; set; }
        public WeightingMode Weighting { get; set; }
    }

    public class SingleMultiFrameMeasurement
    {
        public double RADeg { get; set; }
        public double DEDeg { get; set; }
        public double Mag { get; set; }
        public double StdDevRAArcSec { get; set; }
        public double StdDevDEArcSec { get; set; }
        public double SolutionUncertaintyRACosDEArcSec { get; set; }
        public double SolutionUncertaintyDEArcSec { get; set; }
        public double FWHMArcSec { get; set; }
        public double Detection { get; set; }
        public double SNR { get; set; }
        public int FrameNo { get; set; }
        public DateTime? OCRedTimeStamp { private get; set; }
        public DateTime? CalculatedTimeStamp { private get; set; }

        public DateTime? FrameTimeStamp
        {
            get { return OCRedTimeStamp ?? CalculatedTimeStamp; }
        }
    }

    public class ProcessingValues
    {
        public double Value;
        public double StdDev;
    }

    public class FlybyPlottingContext
    {
        public Pen IncludedPen;
        public Pen ExcludedPen;
        public Pen AveragePen;
        public double MinValue;
    }

    public class FlybyMeasurementContext
    {        
        public double MaxStdDev;
        public double MinPositionUncertaintyPixels;
        public double UserMidValue;
        public double ArsSecsInPixel;
        public int UserMidFrame;
        public int FirstVideoFrame;
        public int MinFrameNo;
        public int MaxFrameNo;
    }

    public class ProcessingReturnValues
    {
        public int EarliestFrame;
        public int LatestFrame;
        public double FittedValue;
        public double FittedValueUncertaintyArcSec;
        public DateTime FittedValueTime;
        public bool IsVideoNormalPosition;
        public double FittedNormalFrame;
    }

    public delegate ProcessingValues GetProcessingValueCallback(SingleMultiFrameMeasurement measurement);

    public enum FittingValue
    {
        RA,
        DEC
    }

    public class FlyByMotionFitter
    {
        private static Brush s_NormalTimeIntervalHighlightBrush = new SolidBrush(Color.FromArgb(143, 142, 112));

        internal class FrameTime
        {
            public int RequestedFrameNo;
            public int ResolvedFrameNo;
            public DateTime UT;
            public double ClosestNormalFrameNo;
            public DateTime ClosestNormalFrameTime;
            public int ClosestNormalIntervalFirstFrameNo;
            public int ClosestNormalIntervalLastFrameNo;
        }

        public class FrameTimeInfo
        {
            public DateTime CentralExposureTime;
            public double ExposureInMilliseconds;
        }

        internal delegate FrameTime GetTimeForFrameCallback(FittingContext context, int frameNumber, int firstVideoFrame);

        public delegate FrameTimeInfo GetFrameStateDataCallback(int frameId);

        private FrameTime GetTimeForFrame(FittingContext context, int frameNumber, int firstVideoFrame, GetFrameStateDataCallback getFrameStateData, DateTime? ocrTime)
        {
            var rv = new FrameTime();
            rv.RequestedFrameNo = frameNumber;

            double instrumentalDelayFrames = 0;
            double instrumentalDelaySeconds = 0;

            if (context.InstrumentalDelayUnits == InstrumentalDelayUnits.Frames)
                instrumentalDelayFrames = context.InstrumentalDelay;
            else if (context.InstrumentalDelayUnits == InstrumentalDelayUnits.Seconds)
                instrumentalDelaySeconds = context.InstrumentalDelay;

            int precision = 1000000;
            double sigma = 0.000005;
            if (context.FrameTimeType == FrameTimeType.NonIntegratedFrameTime)
            {
                if (context.VideoFileFormat == VideoFileFormat.AVI)
                {
                    // No integration is used, directly derive the time and apply instrumental delay
                    rv.ResolvedFrameNo = frameNumber - firstVideoFrame;
                    rv.UT =
                        context.FirstFrameUtcTime.AddSeconds(
                        ((frameNumber - context.FirstFrameIdInIntegrationPeroid - instrumentalDelayFrames) / context.FrameRate) - instrumentalDelaySeconds);
                }
                else if (context.VideoFileFormat == VideoFileFormat.AAV2 && ocrTime.HasValue)
                {
                    rv.ResolvedFrameNo = frameNumber - firstVideoFrame;
                    var dateTime = context.FirstFrameUtcTime.Date.Add(ocrTime.Value.TimeOfDay);
                    rv.UT = dateTime.AddSeconds(-instrumentalDelayFrames/context.FrameRate-instrumentalDelaySeconds);
                }
                else
                {
                    rv.ResolvedFrameNo = frameNumber;

                    var frameState = getFrameStateData(frameNumber);
                    rv.UT = frameState.CentralExposureTime;

                    // Convert Central time to the timestamp of the end of the first field
                    rv.UT = rv.UT.AddMilliseconds(-0.5 * frameState.ExposureInMilliseconds);

                    if (context.NativeVideoFormat == "PAL") rv.UT = rv.UT.AddMilliseconds(20);
                    else if (context.NativeVideoFormat == "NTSC") rv.UT = rv.UT.AddMilliseconds(16.68);

                    if (context.AavStackedMode)
                    {
                        throw new NotSupportedException("AAV Stacked Mode is not supported!");
                    }
                    else
                    {
                        // Then apply instrumental delay
                        rv.UT = rv.UT.AddSeconds(-1 * instrumentalDelaySeconds);
                    }
                }
            }
            else
            {
                // Integration was used. Find the integrated frame no
                int integratedFrameNo = frameNumber - firstVideoFrame;

                double deltaMiddleFrame = 0;
                if (context.IntegratedFramesCount > 1)
                    deltaMiddleFrame = (context.IntegratedFramesCount / 2) - 0.5 
                        /* This 0.5 frame correction is only used for the 'visual' mapping between where the user clicked and what is the most 'logical' normal frame to where they clicked */;

                // The instrumental delay is always from the timestamp of the first frame of the integrated interval

                rv.ResolvedFrameNo = integratedFrameNo;

                rv.UT =
                    context.FirstFrameUtcTime.AddSeconds(
                    ((integratedFrameNo - deltaMiddleFrame - context.FirstFrameIdInIntegrationPeroid - instrumentalDelayFrames) / context.FrameRate) - instrumentalDelaySeconds);
            }

            if (context.MovementExpectation == MovementExpectation.Slow)
            {
                precision = 100000;
                sigma = 0.000005;
            }
            else
            {
                precision = 1000000;
                sigma = 0.0000005;
            }

            double utTime = (rv.UT.Hour + rv.UT.Minute / 60.0 + (rv.UT.Second + (rv.UT.Millisecond / 1000.0)) / 3600.0) / 24;


            double roundedTime = Math.Truncate(Math.Round(utTime * precision)) / precision;

            rv.ClosestNormalFrameTime = new DateTime(rv.UT.Year, rv.UT.Month, rv.UT.Day).AddDays(roundedTime);

            TimeSpan delta = new TimeSpan(rv.ClosestNormalFrameTime.Ticks - rv.UT.Ticks);
            rv.ClosestNormalFrameNo = rv.ResolvedFrameNo + (delta.TotalSeconds * context.FrameRate);

            double framesEachSide = sigma * 24 * 3600 * context.FrameRate;
            rv.ClosestNormalIntervalFirstFrameNo = (int)Math.Floor(rv.ClosestNormalFrameNo - framesEachSide);
            rv.ClosestNormalIntervalLastFrameNo = (int)Math.Ceiling(rv.ClosestNormalFrameNo + framesEachSide);

            //Trace.WriteLine(string.Format("{0} / {1}; {2} / {3}; {4} / {5}", 
            //    roundedTime.ToString("0.00000"), utTime, 
            //    rv.ClosestNormalFrameNo, rv.ResolvedFrameNo,
            //    rv.UT.ToString("HH:mm:ss.fff"), rv.ClosestNormalFrameTime.ToString("HH:mm:ss.fff")));

            return rv;
        }

        private bool m_DumpTestCaseData = false;

        public ProcessingReturnValues FitAndPlotSlowFlyby(
            Dictionary<int, SingleMultiFrameMeasurement> measurements,
            FlybyMeasurementContext meaContext,
            FittingContext fittingContext,
            FittingValue fittingValue,
            GetFrameStateDataCallback getFrameStateDataCallback,
            Graphics g, FlybyPlottingContext plottingContext, float xScale, float yScale, int imageWidth, int imageHight,
            out double motionRate)
        {
            try
            {
                #region Building Test Cases
                if (m_DumpTestCaseData)
                {
                    var mvSer = new XmlSerializer(typeof(FlybyMeasurementContext));
                    var sb = new StringBuilder();
                    using (var wrt = new StringWriter(sb))
                    {
                        mvSer.Serialize(wrt, meaContext);
                    }
                    Trace.WriteLine(sb.ToString());
                    var fcSer = new XmlSerializer(typeof(FittingContext));
                    sb.Clear();
                    using (var wrt = new StringWriter(sb))
                    {
                        fcSer.Serialize(wrt, fittingContext);
                    }
                    Trace.WriteLine(sb.ToString());

                    var smfmSer = new XmlSerializer(typeof(SingleMultiFrameMeasurement));
                    foreach (int key in measurements.Keys)
                    {
                        sb.Clear();
                        using (var wrt2 = new StringWriter(sb))
                        {
                            smfmSer.Serialize(wrt2, measurements[key]);
                            if (measurements[key].FrameNo != key)
                                throw new InvalidOperationException();
                            Trace.WriteLine(sb.ToString());
                        }
                    }
                }
                #endregion

                // Do linear regression, use residual based exclusion rules
                // Report the interpolated position at the middle of the measured interva
                // Don't forget to add the video normal position flag in the OBS file
                // Expect elongated images and apply instrumental delay corrections

                motionRate = double.NaN;

                var rv = new ProcessingReturnValues();

                int numFramesUser = 0;

                rv.EarliestFrame = int.MaxValue;
                rv.LatestFrame = int.MinValue;

                var intervalValues = new Dictionary<int, Tuple<List<double>, List<double>>>();
                var intervalMedians = new Dictionary<double, double>();
                var intervalWeights = new Dictionary<double, double>();

                LinearRegression regression = null;
                if (measurements.Values.Count > 1)
                {
                    rv.EarliestFrame = measurements.Values.Select(m => m.FrameNo).Min();
                    rv.LatestFrame = measurements.Values.Select(m => m.FrameNo).Max();

                    var minUncertainty = meaContext.MinPositionUncertaintyPixels * meaContext.ArsSecsInPixel;

                    foreach (SingleMultiFrameMeasurement measurement in measurements.Values)
                    {
                        int integrationInterval = (measurement.FrameNo - fittingContext.FirstFrameIdInIntegrationPeroid) / fittingContext.IntegratedFramesCount;

                        Tuple<List<double>,List<double>> intPoints;
                        if (!intervalValues.TryGetValue(integrationInterval, out intPoints))
                        {
                            intPoints = Tuple.Create(new List<double>(), new List<double>());
                            intervalValues.Add(integrationInterval, intPoints);
                        }

                        if (fittingValue == FittingValue.RA)
                        {
                            intPoints.Item1.Add(measurement.RADeg);
                            intPoints.Item2.Add(ComputePositionWeight(measurement.SolutionUncertaintyRACosDEArcSec, measurement, minUncertainty, fittingContext.Weighting));
                        }
                        else
                        {
                            intPoints.Item1.Add(measurement.DEDeg);
                            intPoints.Item2.Add(ComputePositionWeight(measurement.SolutionUncertaintyDEArcSec, measurement, minUncertainty, fittingContext.Weighting));
                        }
                    }

                    if (intervalValues.Count > 2)
                    {
                        regression = new LinearRegression();

                        foreach (int integratedFrameNo in intervalValues.Keys)
                        {
                            Tuple<List<double>, List<double>> data = intervalValues[integratedFrameNo];

                            double median;
                            double medianWeight;

                            WeightedMedian(data, out median, out medianWeight);

                            // Assign the data point to the middle of the integration interval (using frame numbers)
                            //
                            // |--|--|--|--|--|--|--|--|
                            // |           |           |
                            //
                            // Because the time associated with the first frame is the middle of the frame, but the 
                            // time associated with the middle of the interval is the end of the field then the correction
                            // is (N / 2) - 0.5 frames when integration is used or no correction when integration of x1 is used.

                            double dataPointFrameNo =
                                rv.EarliestFrame +
                                fittingContext.IntegratedFramesCount * integratedFrameNo
                                + (fittingContext.IntegratedFramesCount / 2)
                                - (fittingContext.IntegratedFramesCount > 1 ? 0.5 : 0);

                            intervalMedians.Add(dataPointFrameNo, median);
                            intervalWeights.Add(dataPointFrameNo, medianWeight);
                            if (fittingContext.Weighting != WeightingMode.None)
                                regression.AddDataPoint(dataPointFrameNo, median, medianWeight);
                            else
                                regression.AddDataPoint(dataPointFrameNo, median);
                        }

                        regression.Solve();

                        var firstPos = measurements[rv.EarliestFrame];
                        var lastPos = measurements[rv.LatestFrame];
                        double distanceArcSec = AngleUtility.Elongation(firstPos.RADeg, firstPos.DEDeg, lastPos.RADeg, lastPos.DEDeg) * 3600;
                        var firstTime = GetTimeForFrame(fittingContext, rv.EarliestFrame, meaContext.FirstVideoFrame, getFrameStateDataCallback, firstPos.FrameTimeStamp);
                        var lastTime = GetTimeForFrame(fittingContext, rv.LatestFrame, meaContext.FirstVideoFrame, getFrameStateDataCallback, lastPos.FrameTimeStamp);
                        double elapsedSec = new TimeSpan(lastTime.UT.Ticks - firstTime.UT.Ticks).TotalSeconds;
                        motionRate = distanceArcSec / elapsedSec;
                    }
                }

                FrameTime resolvedTime = null;
                if (int.MinValue != meaContext.UserMidFrame)
                {
                    // Find the closest video 'normal' MPC time and compute the frame number for it
                    // Now compute the RA/DE for the computed 'normal' frame
                    resolvedTime = GetTimeForFrame(fittingContext, meaContext.UserMidFrame, meaContext.FirstVideoFrame, getFrameStateDataCallback, measurements[meaContext.UserMidFrame].FrameTimeStamp);

                    #region Plotting Code
                    if (g != null)
                    {
                        float xPosBeg = (float)(resolvedTime.ClosestNormalIntervalFirstFrameNo - rv.EarliestFrame) * xScale + 5;
                        float xPosEnd = (float)(resolvedTime.ClosestNormalIntervalLastFrameNo - rv.EarliestFrame) * xScale + 5;

                        g.FillRectangle(s_NormalTimeIntervalHighlightBrush, xPosBeg, 1, (xPosEnd - xPosBeg), imageHight - 2);
                    }
                    #endregion
                }

                Dictionary<double, double> secondPassData = new Dictionary<double, double>();

                int minFrameId = measurements.Keys.Min();

                #region Plotting Code
                if (g != null)
                {
                    foreach (SingleMultiFrameMeasurement measurement in measurements.Values)
                    {
                        float x = (measurement.FrameNo - minFrameId) * xScale + 5;

                        ProcessingValues val = new ProcessingValues()
                        {
                            Value = fittingValue == FittingValue.RA ? measurement.RADeg : measurement.DEDeg,
                            StdDev = fittingValue == FittingValue.RA ? measurement.StdDevRAArcSec / 3600.0 : measurement.StdDevDEArcSec / 3600.0
                        };

                        double valueFrom = val.Value - val.StdDev;
                        double valueTo = val.Value + val.StdDev;

                        float yFrom = (float)(valueFrom - plottingContext.MinValue) * yScale + 5;
                        float yTo = (float)(valueTo - plottingContext.MinValue) * yScale + 5;

                        g.DrawLine(plottingContext.IncludedPen, x, yFrom, x, yTo);
                        g.DrawLine(plottingContext.IncludedPen, x - 1, yFrom, x + 1, yFrom);
                        g.DrawLine(plottingContext.IncludedPen, x - 1, yTo, x + 1, yTo);
                    }
                }
                #endregion

                foreach (double integrFrameNo in intervalMedians.Keys)
                {
                    double val = intervalMedians[integrFrameNo];

                    double fittedValAtFrame = regression != null
                        ? regression.ComputeY(integrFrameNo)
                        : double.NaN;

                    bool included = Math.Abs(fittedValAtFrame - val) < 3 * regression.StdDev;

                    #region Plotting Code
                    if (g != null)
                    {
                        if (fittingContext.IntegratedFramesCount > 1)
                        {
                            Pen mPen = included ? plottingContext.IncludedPen : plottingContext.ExcludedPen;

                            float x = (float)(integrFrameNo - minFrameId) * xScale + 5;
                            float y = (float)(val - plottingContext.MinValue) * yScale + 5;

                            g.DrawEllipse(mPen, x - 3, y - 3, 6, 6);
                            g.DrawLine(mPen, x - 5, y - 5, x + 5, y + 5);
                            g.DrawLine(mPen, x + 5, y - 5, x - 5, y + 5);
                        }
                    }
                    #endregion

                    if (included) secondPassData.Add(integrFrameNo, val);
                }

                #region Second Pass
                regression = null;
                if (secondPassData.Count > 2)
                {
                    regression = new LinearRegression();
                    foreach (double frameNo in secondPassData.Keys)
                    {
                        if (fittingContext.Weighting != WeightingMode.None)
                            regression.AddDataPoint(frameNo, secondPassData[frameNo], intervalWeights[frameNo]);
                        else
                            regression.AddDataPoint(frameNo, secondPassData[frameNo]);
                    }
                    regression.Solve();
                }
                #endregion

                if (regression != null)
                {
                    #region Plotting Code
                    if (g != null)
                    {
                        double leftFittedVal = regression.ComputeY(rv.EarliestFrame);
                        double rightFittedVal = regression.ComputeY(rv.LatestFrame);

                        double err = 3 * regression.StdDev;

                        float leftAve = (float)(leftFittedVal - plottingContext.MinValue) * yScale + 5;
                        float rightAve = (float)(rightFittedVal - plottingContext.MinValue) * yScale + 5;
                        float leftX = 5 + (float)(rv.EarliestFrame - rv.EarliestFrame) * xScale;
                        float rightX = 5 + (float)(rv.LatestFrame - rv.EarliestFrame) * xScale;

                        g.DrawLine(plottingContext.AveragePen, leftX, leftAve - 1, rightX, rightAve - 1);
                        g.DrawLine(plottingContext.AveragePen, leftX, leftAve, rightX, rightAve);
                        g.DrawLine(plottingContext.AveragePen, leftX, leftAve + 1, rightX, rightAve + 1);

                        float leftMin = (float)(leftFittedVal - err - plottingContext.MinValue) * yScale + 5;
                        float leftMax = (float)(leftFittedVal + err - plottingContext.MinValue) * yScale + 5;
                        float rightMin = (float)(rightFittedVal - err - plottingContext.MinValue) * yScale + 5;
                        float rightMax = (float)(rightFittedVal + err - plottingContext.MinValue) * yScale + 5;

                        g.DrawLine(plottingContext.AveragePen, leftX, leftMin, rightX, rightMin);
                        g.DrawLine(plottingContext.AveragePen, leftX, leftMax, rightX, rightMax);
                    }
                    #endregion

                    if (int.MinValue != meaContext.UserMidFrame &&
                        resolvedTime != null)
                    {
                        // Find the closest video 'normal' MPC time and compute the frame number for it
                        // Now compute the RA/DE for the computed 'normal' frame

                        double fittedValueUncertainty;
                        double fittedValueAtMiddleFrame = regression.ComputeYWithError(resolvedTime.ClosestNormalFrameNo, out fittedValueUncertainty);

                        Trace.WriteLine(string.Format("{0}; Included: {1}; Normal Frame No: {2}; Fitted Val: {3} +/- {4:0.00}",
                            meaContext.UserMidValue.ToString("0.00000"),
                            numFramesUser, resolvedTime.ClosestNormalFrameNo,
                            AstroConvert.ToStringValue(fittedValueAtMiddleFrame, "+HH MM SS.T"),
                            regression.StdDev * 60 * 60));

                        // Report the interpolated position at the middle of the measured interval
                        // Don't forget to add the video normal position flag in the OBS file
                        // Expect elongated images and apply instrumental delay corrections

                        rv.FittedValue = fittedValueAtMiddleFrame;
                        rv.FittedValueTime = resolvedTime.ClosestNormalFrameTime;
                        rv.IsVideoNormalPosition = true;
                        rv.FittedNormalFrame = resolvedTime.ClosestNormalFrameNo;
                        rv.FittedValueUncertaintyArcSec = fittedValueUncertainty * 60 * 60;

                        #region Plotting Code
                        if (g != null)
                        {
                            // Plot the frame
                            float xPos = (float)(resolvedTime.ClosestNormalFrameNo - rv.EarliestFrame) * xScale + 5;
                            float yPos = (float)(rv.FittedValue - plottingContext.MinValue) * yScale + 5;
                            g.DrawLine(Pens.Yellow, xPos, 1, xPos, imageHight - 2);
                            g.FillEllipse(Brushes.Yellow, xPos - 3, yPos - 3, 6, 6);
                        }
                        #endregion
                    }
                    else
                        rv.FittedValue = double.NaN;
                }
                else
                    rv.FittedValue = double.NaN;

                return rv;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.GetFullStackTrace());
                motionRate = 0;
                return null;
            }
        }

        public static double ComputeWeight(WeightingMode mode, double solutionUncertaintyArcSec, double fwhmArcSec, double snr, double detection, double minUncertainty)
        {
            if (mode == WeightingMode.SNR)
            {
                // Positional uncertainty estimation by Neuschaefer and Windhorst 1994
                var sigmaPosition = fwhmArcSec / (2.355 * snr);
                if (sigmaPosition < minUncertainty) sigmaPosition = minUncertainty;
                var combinedUncertainty = Math.Sqrt(sigmaPosition * sigmaPosition + solutionUncertaintyArcSec * solutionUncertaintyArcSec);
                return 1 / (combinedUncertainty * combinedUncertainty);
            }
            else if (mode == WeightingMode.SolutionUncertainty)
            {
                if (solutionUncertaintyArcSec < minUncertainty)
                    return 1 / (minUncertainty * minUncertainty);
                else 
                    return 1 / (solutionUncertaintyArcSec * solutionUncertaintyArcSec);
            }
            else if (mode == WeightingMode.Detection)
            {
                return detection / (solutionUncertaintyArcSec * solutionUncertaintyArcSec);
            }

            return 1;            
        }

        private double ComputePositionWeight(double solutionUncertaintyArcSec, SingleMultiFrameMeasurement measurement, double minUncertainty, WeightingMode mode)
        {
            return FlyByMotionFitter.ComputeWeight(mode, solutionUncertaintyArcSec, measurement.FWHMArcSec, measurement.SNR, measurement.Detection, minUncertainty);
        }

        private void WeightedMedian(Tuple<List<double>, List<double>> data, out double median, out double medianWeight)
        {
            if (data.Item1.Count == 1)
            {
                median = data.Item1[0];
                medianWeight = data.Item2[0];
            }
            else
            {
                var posArr = data.Item1.ToArray();
                var weightArr = data.Item2.ToArray();
                Array.Sort(posArr, weightArr);

                double weightSum = weightArr.Sum();
                double sumSoFar = 0;
                for (int i = 0; i < weightArr.Length; i++)
                {
                    sumSoFar += weightArr[i] / weightSum;
                    if (sumSoFar >= 0.5)
                    {
                        if (i > 0 && weightArr[i - 1] > weightArr[i])
                        {
                            // If the previous entry has higher weight, then use it 
                            median = posArr[i - 1];
                            medianWeight = weightArr[i - 1];
                        }
                        else
                        {
                            median = posArr[i];
                            medianWeight = weightArr[i];
                        }
                        return;
                    }
                }

                // Fallback. Should never hit here though.
                median = posArr.Length % 2 == 1
                        ? posArr[posArr.Length / 2]
                        : (posArr[posArr.Length / 2] + posArr[(posArr.Length / 2) - 1]) / 2.0;

                medianWeight = weightArr.Length % 2 == 1
                        ? weightArr[weightArr.Length / 2]
                        : (weightArr[weightArr.Length / 2] + weightArr[(weightArr.Length / 2) - 1]) / 2.0;
            }
        }

        public ProcessingReturnValues FitAndPlotSlowMotion(
            Dictionary<int, SingleMultiFrameMeasurement> measurements,
            FlybyMeasurementContext meaContext,
            GetProcessingValueCallback getValueCallback,
            Graphics g, FlybyPlottingContext plottingContext, float xScale, float yScale, int imageWidth)
        {
            // Compute median, use median based exclusion rules
            // Report the median position for the time at the middle of the measured interval 
            // Do not expect elongated image (no corrections from the exposure)
            // May apply instrumental delay corrections for the frame time

            var rv = new ProcessingReturnValues();

            double sum = 0;
            double userSum = 0;
            double stdDevUserSum = 0;
            int numFramesUser = 0;

            double userMidFrom = meaContext.UserMidValue - meaContext.MaxStdDev;
            double userMidTo = meaContext.UserMidValue + meaContext.MaxStdDev;

            rv.EarliestFrame = int.MaxValue;
            rv.LatestFrame = int.MinValue;

            List<double> medianList = new List<double>();
            List<double> medianWeightsList = new List<double>();
            var minPosUncertaintyArcSec = TangraConfig.Settings.Astrometry.AssumedPositionUncertaintyPixels * meaContext.ArsSecsInPixel;
            foreach (SingleMultiFrameMeasurement measurement in measurements.Values)
            {
                float x = (measurement.FrameNo - meaContext.MinFrameNo) * xScale + 5;
                ProcessingValues val = getValueCallback(measurement);
                double valueFrom = val.Value - val.StdDev;
                double valueTo = val.Value + val.StdDev;

                float yFrom = (float)(valueFrom - plottingContext.MinValue) * yScale + 5;
                float yTo = (float)(valueTo - plottingContext.MinValue) * yScale + 5;

                sum += val.Value;

                Pen mPen = plottingContext.IncludedPen;
                if (!double.IsNaN(meaContext.UserMidValue))
                {
                    if ((valueFrom >= userMidFrom && valueFrom <= userMidTo) ||
                        (valueTo >= userMidFrom && valueTo <= userMidTo))
                    {
                        numFramesUser++;
                        userSum += val.Value;
                        medianList.Add(val.Value);
                        medianWeightsList.Add(ComputePositionWeight(val.StdDev, measurement, minPosUncertaintyArcSec, WeightingMode.SNR));
 
                        stdDevUserSum += val.StdDev * val.StdDev;
                        if (rv.EarliestFrame > measurement.FrameNo) rv.EarliestFrame = measurement.FrameNo;
                        if (rv.LatestFrame < measurement.FrameNo) rv.LatestFrame = measurement.FrameNo;
                    }
                    else
                        mPen = plottingContext.ExcludedPen;
                }


                g.DrawLine(mPen, x, yFrom, x, yTo);
                g.DrawLine(mPen, x - 1, yFrom, x + 1, yFrom);
                g.DrawLine(mPen, x - 1, yTo, x + 1, yTo);
            }

            if (!double.IsNaN(meaContext.UserMidValue) && numFramesUser > 0)
            {
                double average = userSum / numFramesUser;
                double err = Math.Sqrt(stdDevUserSum) / (numFramesUser - 1);
                float yAve = (float)(average - plottingContext.MinValue) * yScale + 5;
                g.DrawLine(plottingContext.AveragePen, 5, yAve - 1, imageWidth - 5, yAve - 1);
                g.DrawLine(plottingContext.AveragePen, 5, yAve, imageWidth - 5, yAve);
                g.DrawLine(plottingContext.AveragePen, 5, yAve + 1, imageWidth - 5, yAve + 1);

                float yMin = (float)(userMidFrom - plottingContext.MinValue) * yScale + 5;
                float yMax = (float)(userMidTo - plottingContext.MinValue) * yScale + 5;
                g.DrawLine(plottingContext.AveragePen, 5, yMin, imageWidth - 5, yMin);
                g.DrawLine(plottingContext.AveragePen, 5, yMax, imageWidth - 5, yMax);

                double median;
                double medianWeight;

                WeightedMedian(Tuple.Create(medianList, medianWeightsList), out median, out medianWeight);

                double standardMedian = medianList.Median();

                Trace.WriteLine(string.Format("{0}; Included: {1}; Average: {2}; Wighted Median: {3}; Standard Median: {4}",
                    meaContext.UserMidValue.ToString("0.00000"),
                    numFramesUser, AstroConvert.ToStringValue(average, "+HH MM SS.TTT"),
                    AstroConvert.ToStringValue(median, "+HH MM SS.TTT"),
                    AstroConvert.ToStringValue(standardMedian, "+HH MM SS.TTT")));

                rv.FittedValue = median;

                var stdDevArcSec = 3600 * Math.Sqrt(medianList.Sum(x => (x - median) * (x - median)) / (medianList.Count - 1));
                var tCoeff95 = TDistribution.CalculateCriticalValue(medianList.Count, (1 - 0.95), 0.0001);
                var error95 = 1.253 * tCoeff95 * stdDevArcSec / Math.Sqrt(medianList.Count);
                rv.FittedValueUncertaintyArcSec = error95;
                rv.IsVideoNormalPosition = false;
            }
            else
            {
                double average = sum / measurements.Count;
                float yAve = (float)(average - plottingContext.MinValue) * yScale + 5;
                g.DrawLine(Pens.WhiteSmoke, 5, yAve, imageWidth - 5, yAve);

                rv.FittedValue = double.NaN;
            }

            return rv;
        }

        public ProcessingReturnValues FitAndPlotFastFlyby(
            Dictionary<int, SingleMultiFrameMeasurement> measurements,
            FlybyMeasurementContext meaContext,
            FittingContext fittingContext,
            FittingValue fittingValue,
            GetFrameStateDataCallback getFrameStateDataCallback,
            Graphics g, FlybyPlottingContext plottingContext, float xScale, float yScale, int imageWidth, int imageHeight,
            out double motionRate)
        {
            // Do linear regression, use residual based exclusion rules
            // Two possible modes: (A) non trailed and (B) trailed images
            // A) Non Trailed Images
            // Report interpolated times for video normal position [How to use the MPC page for this?]
            // Don't forget to add the video normal position flag in the OBS file
            // Expect elongated images and apply instrumental delay corrections (in both integrated and non integrated modes)
            // B) Trailed Images
            // TODO: R&D Required

            if (fittingContext.ObjectExposureQuality == ObjectExposureQuality.GoodSignal)
            {
                return FitAndPlotSlowFlyby(
                    measurements, meaContext, fittingContext, fittingValue, getFrameStateDataCallback,
                    g, plottingContext, xScale, yScale, imageWidth, imageHeight, out motionRate);
            }
            else
            {
                MessageBox.Show("This operation is currently not suppored.");
                motionRate = double.NaN;
                return new ProcessingReturnValues();
            }
        }    
    }
}
