using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Tangra.Model.Config;
using Tangra.Model.Helpers;
using Tangra.Model.Numerical;

namespace Tangra.MotionFitting
{
    public class FastMotionPositionExtractor
    {
        private const double SECONDS_IN_A_DAY = 60 * 60 * 24;
        private List<FastMotionChunkPositionExtractor> m_Chunks = new List<FastMotionChunkPositionExtractor>(); 

        public void Calculate(
            IMeasurementPositionProvider provider, 
            WeightingMode weighting, 
            int numChunks, 
            bool removeOutliers, 
            bool fitSameIncline, 
            double minPositionUncertaintyPixels, 
            double outlierSigmaCoeff = 3.0)
        {
            if (provider != null)
            {
                m_Chunks.Clear();
                if (numChunks < 1) numChunks = 1;

                var allEntries = provider.Measurements.ToList();
                var minUncertainty = minPositionUncertaintyPixels * provider.ArsSecsInPixel;
                double instDelayTimeOfDay = ((double) provider.InstrumentalDelaySec/SECONDS_IN_A_DAY);

                int numEntriesPerChunk = allEntries.Count / numChunks;

                for (int i = 0; i < numChunks; i++)
                {
                    int firstId = i * numEntriesPerChunk;
                    int lastId = (i + 1) * numEntriesPerChunk  - 1;
                    if (lastId > (numChunks - 1)*numEntriesPerChunk) lastId = allEntries.Count;

                    var chunkPosExtractor = new FastMotionChunkPositionExtractor(firstId, lastId);
                    var chunkEntries = allEntries.Skip(firstId).Take(lastId - firstId).Where(x => x.TimeOfDayUTC != 0).ToArray();
                    chunkPosExtractor.Calculate(chunkEntries, weighting, removeOutliers, outlierSigmaCoeff, instDelayTimeOfDay, minUncertainty);
                    m_Chunks.Add(chunkPosExtractor);
                }
            }
        }

        public string[] ExtractPositions(string obsCode, string designation, DateTime obsDate)
        {
            var lines = new List<string>();

            foreach (var chunk in m_Chunks)
            {
                lines.Add(chunk.GetMidPointReport(obsCode, designation, obsDate));
            }

            return lines.ToArray();
        }

        public const int PADDING_L = 15;
        public const int PADDING_R = 5;
        public const int BORDER = 7;
        public const int TITLE_PADDING = 5;

        public void PlotRAFit(Graphics g, int fullWidth, int fullHeight)
        {
            Plot(g, Pens.Lime, 
                fullWidth, fullHeight, 
                m_Chunks.Min(x => x.MinRADeg),
                m_Chunks.Max(x => x.MaxRADeg),
                "RA",
                m_Chunks.Average(x => x.InclinationRA),
                (x) => x.RADeg, 
                (x) => x.RAWeightDeg,
                (x) => x.RADegFitted,
                (c) => c.GetMidPointRAPosition());
        }

        internal void Plot(Graphics g, Pen pen,
            int fullWidth, int fullHeight, 
            double minY, double maxY, 
            string motionName,
            double aveIncl,
            Func<CalculatedEntry, double> getVal,
            Func<CalculatedEntry, double> getWeight,
            Func<CalculatedEntry, double> getCalcVal,
            Func<FastMotionChunkPositionExtractor, double> getMidPointPos)
        {
            g.Clear(SystemColors.ControlDarkDark);

            if (m_Chunks.Count == 0) return;

            float clientAreaWidth = fullWidth - PADDING_L - PADDING_R - 2 * BORDER;
            float clientAreaHeight = fullHeight - PADDING_L - PADDING_R - 2 * BORDER;

            g.DrawRectangle(SystemPens.ControlDark, PADDING_L, PADDING_R, clientAreaWidth + 2 * BORDER, clientAreaHeight + 2 * BORDER);

            double minX = m_Chunks.Min(x => x.MinTimeOfDayUTC);
            double maxX = m_Chunks.Max(x => x.MaxTimeOfDayUTC);

            float scaleX = (float)(clientAreaWidth / (maxX - minX));
            float scaleY = (float)(clientAreaHeight / (maxY - minY));

            var repX = new List<double>();
            var repY = new List<double>();

            foreach (var chunk in m_Chunks)
            {
                double? startX = null;
                double? startYCalc = null;
                CalculatedEntry lastEntry = null;
                foreach (var entry in chunk.Entries)
                {
                    lastEntry = entry;
                    float x = (float)Math.Round(PADDING_L + BORDER + (scaleX * (entry.TimeOfDayUTC - minX)));
                    float y = fullHeight - PADDING_L - BORDER - (float)(scaleY * (getVal(entry) - minY));

                    if (!startX.HasValue)
                    {
                        startX = entry.TimeOfDayUTC;
                        startYCalc = getCalcVal(entry);
                    }
                    g.DrawEllipse(pen, x - 1, y - 1, 2, 2);

                    float yErr = (float)(scaleY * getWeight(entry));
                    if (yErr > 0)
                    {
                        g.DrawLine(pen, x, y - yErr, x, y + yErr);
                        g.DrawLine(pen, x - 1, y - yErr, x + 1, y - yErr);
                        g.DrawLine(pen, x - 1, y + yErr, x + 1, y + yErr);
                    }
                }

                // Plot fitted line
                if (lastEntry != null)
                {
                    double endX = lastEntry.TimeOfDayUTC;
                    double endYCalc = getCalcVal(lastEntry);

                    float x1 = (float)Math.Round(PADDING_L + BORDER + (scaleX * (startX.Value - minX)));
                    float y1 = fullHeight - PADDING_L - BORDER - (float)(scaleY * (startYCalc.Value - minY));
                    float x2 = (float)Math.Round(PADDING_L + BORDER + (scaleX * (endX - minX)));
                    float y2 = fullHeight - PADDING_L - BORDER - (float)(scaleY * (endYCalc - minY)); 
                    
                    g.DrawLine(Pens.Azure, x1, y1, x2, y2);
                    g.DrawLine(Pens.Azure, x1, y1 + 1, x2, y2 + 1);

                    double fittedY = getMidPointPos(chunk);
                    double midX = chunk.GetMidPointDelayCorrectedTimeOfDay();
                    float xf = (float)Math.Round(PADDING_L + BORDER + (scaleX * (midX - minX)));
                    float yf = fullHeight - PADDING_L - BORDER - (float)(scaleY * (fittedY - minY));
                    g.FillEllipse(Brushes.Azure, xf - 3, yf - 3, 7, 7);

                    repX.Add(midX);
                    repY.Add(fittedY);
                }
            }

            // Calculate the StdDev of linear motion from all reported points and draw it on the plot
            if (repX.Count > 2)
            {
                var repReg = new LinearRegression();
                for (int i = 0; i < repX.Count; i++)
                {
                    repReg.AddDataPoint(repX[i], repY[i]);
                }

                repReg.Solve();

                double stdDevArcSec = repReg.StdDev * 3600.0;
                string text = string.Format("{0} points StdDev: {1:0.00} arcsec", repX.Count, stdDevArcSec);
                var szf = g.MeasureString(text, s_LegendFont);
                bool topLeft = aveIncl > 0;
                if (topLeft)
                {
                    g.FillRectangle(SystemBrushes.ControlDarkDark, PADDING_L + BORDER, PADDING_R + BORDER, szf.Width, szf.Height);
                    g.DrawString(text, s_LegendFont, Brushes.Azure, PADDING_L + BORDER, PADDING_R + BORDER);
                }
                else
                {
                    g.FillRectangle(SystemBrushes.ControlDarkDark, fullWidth - PADDING_R - BORDER - szf.Width, PADDING_R + BORDER, szf.Width, szf.Height);
                    g.DrawString(text, s_LegendFont, Brushes.Azure, fullWidth - PADDING_R - BORDER - szf.Width, PADDING_R + BORDER);
                }
            }

            // Plot Axis Marks
            var minYArcSec = (long)Math.Ceiling(minY * 3600);
            var maxYArcSec = (long)Math.Floor(maxY * 3600);
            for (long wholeArcSec = minYArcSec; wholeArcSec <= maxYArcSec; wholeArcSec++)
            {
                int len = 1;
                if (wholeArcSec % 60 == 0) len = 5;
                else if (wholeArcSec % 10 == 0) len = 3;

                float y = fullHeight - PADDING_L - BORDER - (float)(scaleY * (wholeArcSec / 3600.0 - minY));
                g.DrawLine(SystemPens.ControlDark, PADDING_L + 1, y, PADDING_L + 1 + len, y);
                g.DrawLine(SystemPens.ControlDark, fullWidth - PADDING_R - 1, y, fullWidth - PADDING_R - 1 - len, y);
            }
            var minXSeconds = (long)Math.Ceiling(minX * SECONDS_IN_A_DAY);
            var maxXSeconds = (long)Math.Floor(maxX * SECONDS_IN_A_DAY);
            for (long wholeSeconds = minXSeconds; wholeSeconds <= maxXSeconds; wholeSeconds++)
            {
                int len = 1;
                if (wholeSeconds % 60 == 0) len = 5;
                else if (wholeSeconds % 10 == 0) len = 3;

                float x = (float)Math.Round(PADDING_L + BORDER + (scaleX * (wholeSeconds / SECONDS_IN_A_DAY - minX)));
                g.DrawLine(SystemPens.ControlDark, x, fullHeight - PADDING_L - 1, x, fullHeight - PADDING_L - 1 - len);
                g.DrawLine(SystemPens.ControlDark, x, PADDING_R + 1, x, PADDING_R + 1 + len);
            }

            string axisText = "Time (sec)";
            var axf = g.MeasureString(axisText, s_LegendFont);
            g.DrawString(axisText, s_LegendFont, Brushes.Azure, new PointF(PADDING_L + (clientAreaWidth - axf.Width) / 2, fullHeight - axf.Height));

            axisText = string.Format("Motion {0} (arcsec)", motionName);
            axf = g.MeasureString(axisText, s_LegendFont);
            s_VerticalDrawFormat.Alignment = StringAlignment.Far;
            g.TranslateTransform(fullWidth, fullHeight );
            g.RotateTransform(180);
            g.DrawString(axisText, s_LegendFont, Brushes.Azure, new PointF(TITLE_PADDING + clientAreaWidth + axf.Height, (fullHeight + axf.Width) / 2), s_VerticalDrawFormat);
        }

        private static Font s_LegendFont = new Font(FontFamily.GenericMonospace, 8, FontStyle.Regular);
        private static StringFormat s_VerticalDrawFormat = new StringFormat(StringFormatFlags.DirectionVertical);

        public void PlotDECFit(Graphics g, int fullWidth, int fullHeight)
        {
            Plot(g, Pens.Aqua,
                fullWidth, fullHeight,
                m_Chunks.Min(x => x.MinDEDeg),
                m_Chunks.Max(x => x.MaxDEDeg),
                "DE",
                m_Chunks.Average(x => x.InclinationDE),
                (x) => x.DEDeg,
                (x) => x.DEWeightDeg,
                (x) => x.DEDegFitted,
                (c) => c.GetMidPointDEPosition());
        }
    }

    internal class FastMotionChunkPositionExtractor
    {
        public int FirstEntryId { get; private set; }
        public int LastEntryId { get; private set; }

        private LinearRegression m_RegressionRA;
        private LinearRegression m_RegressionDE;

        private double m_InstDelayTimeOfDay;
        private WeightingMode m_Weighting;

        private List<MeasurementPositionEntry> m_Entries;

        public FastMotionChunkPositionExtractor(int firstEntryId, int lastEntryId)
        {
            FirstEntryId = firstEntryId;
            LastEntryId = lastEntryId;
        }

        public void Calculate(MeasurementPositionEntry[] entries, WeightingMode weighting, bool removeOutliers, double outlierSigmaCoeff, double instDelayTimeOfDay, double minUncertainty)
        {
            m_InstDelayTimeOfDay = instDelayTimeOfDay;
            m_Weighting = weighting;

            var regRA = new LinearRegression();
            var regDE = new LinearRegression();

            foreach (var entry in entries)
            {
                var midFrameTime = entry.TimeOfDayUTC - instDelayTimeOfDay;
                if (weighting == WeightingMode.None)
                {
                    regRA.AddDataPoint(midFrameTime, entry.RADeg);
                    regDE.AddDataPoint(midFrameTime, entry.DEDeg);
                }
                else
                {
                    var weightRA = CalulateWeight(entry, entry.SolutionUncertaintyRACosDEArcSec, minUncertainty, weighting);
                    var weightDE = CalulateWeight(entry, entry.SolutionUncertaintyDEArcSec, minUncertainty, weighting);
                    regRA.AddDataPoint(midFrameTime, entry.RADeg, weightRA);
                    regDE.AddDataPoint(midFrameTime, entry.DEDeg, weightDE);
                }
            }

            m_Entries = new List<MeasurementPositionEntry>();

            regRA.Solve();
            regDE.Solve();

            if (removeOutliers)
            {
                var outlierLimitRA = regRA.StdDev*outlierSigmaCoeff;
                var residualsRA = regRA.Residuals.ToArray();
                var outlierLimitDE = regDE.StdDev*outlierSigmaCoeff;
                var residualsDE = regDE.Residuals.ToArray();

                for (int i = 0; i < entries.Length; i++)
                {
                    if (Math.Abs(residualsRA[i]) <= outlierLimitRA && Math.Abs(residualsDE[i]) <= outlierLimitDE)
                        m_Entries.Add(entries[i]);
                }

                m_RegressionRA = new LinearRegression();
                m_RegressionDE = new LinearRegression();

                foreach (var entry in m_Entries)
                {
                    var midFrameTime = entry.TimeOfDayUTC - instDelayTimeOfDay;

                    if (weighting == WeightingMode.None)
                    {
                        m_RegressionRA.AddDataPoint(midFrameTime, entry.RADeg);
                        m_RegressionDE.AddDataPoint(midFrameTime, entry.DEDeg);
                    }
                    else
                    {
                        var weightRA = CalulateWeight(entry, entry.SolutionUncertaintyRACosDEArcSec, minUncertainty,
                            weighting);
                        m_RegressionRA.AddDataPoint(midFrameTime, entry.RADeg, weightRA);

                        var weightDE = CalulateWeight(entry, entry.SolutionUncertaintyDEArcSec, minUncertainty,
                            weighting);
                        m_RegressionDE.AddDataPoint(midFrameTime, entry.DEDeg, weightDE);
                    }
                }

                m_RegressionRA.Solve();
                m_RegressionDE.Solve();
            }
            else
            {
                m_RegressionRA = regRA;
                m_RegressionDE = regDE;
                m_Entries = entries.ToList();
            }
        }

        private double CalulateWeight(MeasurementPositionEntry entry, double solutionUncertaintyArcSec, double minUncertainty, WeightingMode weighting)
        {
            if (weighting == WeightingMode.SNR)
            {
                // Positional uncertainty estimation by Neuschaefer and Windhorst 1994
                var sigmaPosition = entry.FWHMArcSec / (2.355 * entry.SNR);
                var combinedUncertainty = Math.Sqrt(sigmaPosition * sigmaPosition + solutionUncertaintyArcSec * solutionUncertaintyArcSec);
                if (combinedUncertainty < minUncertainty) combinedUncertainty = minUncertainty;
                return 1 / (combinedUncertainty * combinedUncertainty);
            }
            else if (weighting == WeightingMode.Detection)
            {
                return entry.DetectionCertainty / (solutionUncertaintyArcSec * solutionUncertaintyArcSec);
            }
            else
                return 1;
        }

        internal double GetMidPointDelayCorrectedTimeOfDay()
        {
            if (m_Entries.Count == 0)
                return 0;

            if (m_Entries.Count == 1)
                return m_Entries[0].TimeOfDayUTC - m_InstDelayTimeOfDay;

            return (m_Entries[0].TimeOfDayUTC + m_Entries[m_Entries.Count - 1].TimeOfDayUTC) / 2.0 - m_InstDelayTimeOfDay;
        }

        internal double GetMidPointRAPosition()
        {
            double closestTimeOfDay = GetMidPointDelayCorrectedTimeOfDay();

            if (m_RegressionRA != null)
            {
                var normalTime = double.Parse(closestTimeOfDay.ToString("0.000000"));
                return m_RegressionRA.ComputeY(normalTime);
            }

            return 0;
        }

        internal double GetMidPointDEPosition()
        {
            double closestTimeOfDay = GetMidPointDelayCorrectedTimeOfDay();
            if (m_RegressionDE != null)
            {
                var normalTime = double.Parse(closestTimeOfDay.ToString("0.000000"));
                return m_RegressionDE.ComputeY(normalTime);
            }

            return 0;
        }

        internal string GetMidPointReport(string obsCode, string designation, DateTime obsDate)
        {
            double closestTimeOfDay = GetMidPointDelayCorrectedTimeOfDay();
                
            MPCObsLine obsLine = new MPCObsLine(obsCode.Substring(0, 3).PadLeft(3));
            obsLine.SetObject(designation.PadLeft(12).Substring(0, 12));

            if (m_RegressionRA != null && m_RegressionDE != null)
            {
                var normalTime = double.Parse(closestTimeOfDay.ToString("0.000000"));

                DateTime mpcTime = obsDate.Date.AddDays(normalTime);
                double errRA, errDE;
                double mpcRAHours = m_RegressionRA.ComputeYWithError(normalTime, out errRA) / 15.0;
                double mpcDEDeg = m_RegressionDE.ComputeYWithError(normalTime, out errDE);

                errRA *= Math.Cos(mpcDEDeg * Math.PI / 180) * 3600;
                errDE *= 3600;

                var mag = m_Entries.Select(x => x.Mag).ToList().Median();

                obsLine.SetPosition(mpcRAHours, mpcDEDeg, mpcTime, true);
                obsLine.SetMagnitude(mag, MagnitudeBand.Cousins_R);
                obsLine.SetUncertainty(errRA, errDE);

                return obsLine.BuildObservationASCIILine();
            }

            return null;
        }

        internal int NumDataPoints
        {
            get { return m_Entries.Count; }
        }

        internal double MinTimeOfDayUTC
        {
            get { return m_Entries[0].TimeOfDayUTC; }
        }

        internal double MaxTimeOfDayUTC
        {
            get { return m_Entries[m_Entries.Count - 1].TimeOfDayUTC; }
        }

        internal double InclinationRA
        {
            get { return m_RegressionRA.A;  }
        }

        internal double InclinationDE
        {
            get { return m_RegressionDE.A; }
        }

        internal double MinRADeg
        {
            get
            {
                if (m_Weighting == WeightingMode.None)
                    return m_Entries.Min(x => x.RADeg);
                else
                    return m_Entries.Min(x => x.RADeg - Math.Sqrt(x.FWHMArcSec * x.FWHMArcSec / (2.355 * 2.355 * x.SNR * x.SNR) + x.SolutionUncertaintyRACosDEArcSec * x.SolutionUncertaintyRACosDEArcSec) / 3600.0);
            }
        }

        internal double MaxRADeg
        {
            get
            {
                if (m_Weighting == WeightingMode.None)
                    return m_Entries.Max(x => x.RADeg);
                else
                    return m_Entries.Max(x => x.RADeg + Math.Sqrt(x.FWHMArcSec * x.FWHMArcSec / (2.355 * 2.355 * x.SNR * x.SNR) + x.SolutionUncertaintyRACosDEArcSec * x.SolutionUncertaintyRACosDEArcSec) / 3600.0);
            }
        }

        internal double MinDEDeg
        {
            get
            {
                if (m_Weighting == WeightingMode.None)
                    return m_Entries.Min(x => x.DEDeg);
                else
                    return m_Entries.Min(x => x.DEDeg - Math.Sqrt(x.FWHMArcSec * x.FWHMArcSec / (2.355 * 2.355 * x.SNR * x.SNR) + x.SolutionUncertaintyDEArcSec * x.SolutionUncertaintyDEArcSec) / 3600.0);
            }
        }

        internal double MaxDEDeg
        {
            get
            {
                if (m_Weighting == WeightingMode.None)
                    return m_Entries.Max(x => x.DEDeg);
                else
                    return m_Entries.Max(x => x.DEDeg + Math.Sqrt(x.FWHMArcSec * x.FWHMArcSec / (2.355 * 2.355 * x.SNR * x.SNR) + x.SolutionUncertaintyDEArcSec * x.SolutionUncertaintyDEArcSec) / 3600.0);
            }
        }

        internal IEnumerable<CalculatedEntry> Entries
        {
            get
            {
                var weightsRA = m_RegressionRA.Weights.ToArray();
                var weightsDE = m_RegressionDE.Weights.ToArray();

                for (int i = 0; i < m_Entries.Count; i++)
                {
                    var entry = m_Entries[i];
                    yield return new CalculatedEntry()
                    {
                        RADeg = entry.RADeg,
                        DEDeg = entry.DEDeg,
                        TimeOfDayUTC = entry.TimeOfDayUTC,
                        RADegFitted = m_RegressionRA.ComputeY(entry.TimeOfDayUTC - m_InstDelayTimeOfDay),
                        DEDegFitted = m_RegressionDE.ComputeY(entry.TimeOfDayUTC - m_InstDelayTimeOfDay),
                        RAWeightDeg = weightsRA.Length > i ? Math.Sqrt(1 / weightsRA[i]) / 3600.0 : 0,
                        DEWeightDeg = weightsDE.Length > i ? Math.Sqrt(1 / weightsDE[i]) / 3600.0 : 0,
                    };
                }
            }
        }
    }

    internal class CalculatedEntry
    {
        public bool IsFirstInSeries;
        public double TimeOfDayUTC;
        public double RADeg;
        public double DEDeg;
        public double RADegFitted;
        public double DEDegFitted;
        public double RAWeightDeg;
        public double DEWeightDeg;
    }
}
