using System;
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

        public void Calculate(IMeasurementPositionProvider provider, WeightingMode weighting, int numChunks, bool removeOutliers, double minPositionUncertaintyPixels, double outlierSigmaCoeff = 3.0)
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
                    var chunkEntries = allEntries.Skip(firstId).Take(lastId - firstId).ToArray();
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
                double closestTimeOfDay = chunk.GetMidAreaTimeOfDay();

                lines.Add(chunk.GetReportByTimeOfDay(closestTimeOfDay, obsCode, designation, obsDate));
            }

            return lines.ToArray();
        }

        public const int PADDING = 5;
        public const int BORDER = 2;

        public void PlotRAFit(Graphics g, int fullWidth, int fullHeight)
        {
            Plot(g, Pens.Lime, 
                fullWidth, fullHeight, 
                m_Chunks.Min(x => x.MinRADeg),
                m_Chunks.Max(x => x.MaxRADeg),
                (x) => x.RADeg, 
                (x) => x.RAWeightDeg);
        }

        internal void Plot(Graphics g, Pen pen,
            int fullWidth, int fullHeight, 
            double minY, double maxY, 
            Func<CalculatedEntry, double> getVal,
            Func<CalculatedEntry, double> getWeight)
        {
            g.Clear(SystemColors.ControlDarkDark);

            if (m_Chunks.Count == 0) return;

            float clientAreaWidth = fullWidth - 2 * PADDING - 2 * BORDER;
            float clientAreaHeight = fullHeight - 2 * PADDING - 2 * BORDER;

            g.DrawRectangle(SystemPens.ControlDark, PADDING, PADDING, clientAreaWidth + 2 * BORDER, clientAreaHeight + 2 * BORDER);

            double minX = m_Chunks.Min(x => x.MinTimeOfDayUTC);
            double maxX = m_Chunks.Max(x => x.MaxTimeOfDayUTC);

            float scaleX = (float)(clientAreaWidth / (maxX - minX));
            float scaleY = (float)(clientAreaHeight / (maxY - minY));

            foreach (var chunk in m_Chunks)
            {
                foreach (var entry in chunk.Entries)
                {
                    float x = (float)Math.Round(PADDING + BORDER + (scaleX * (entry.TimeOfDayUTC - minX)));
                    float y = fullHeight - PADDING - BORDER - (float)(scaleY * (getVal(entry) - minY));

                    g.DrawEllipse(pen, x - 1, y - 1, 2, 2);

                    float yErr = (float)(scaleY * getWeight(entry));
                    if (yErr > 0)
                    {
                        g.DrawLine(pen, x, y - yErr, x, y + yErr);
                        g.DrawLine(pen, x - 1, y - yErr, x + 1, y - yErr);
                        g.DrawLine(pen, x - 1, y + yErr, x + 1, y + yErr);
                    }
                }
            }
        }

        public void PlotDECFit(Graphics g, int fullWidth, int fullHeight)
        {
            Plot(g, Pens.Aqua,
                fullWidth, fullHeight,
                m_Chunks.Min(x => x.MinDEDeg),
                m_Chunks.Max(x => x.MaxDEDeg),
                (x) => x.DEDeg,
                (x) => x.DEWeightDeg);
        }
    }

    internal class FastMotionChunkPositionExtractor
    {
        public int FirstEntryId { get; private set; }
        public int LastEntryId { get; private set; }

        private LinearRegression m_RegressionRA;
        private LinearRegression m_RegressionDE;

        private double m_InstDelayTimeOfDay;

        private List<MeasurementPositionEntry> m_Entries;

        public FastMotionChunkPositionExtractor(int firstEntryId, int lastEntryId)
        {
            FirstEntryId = firstEntryId;
            LastEntryId = lastEntryId;
        }

        public void Calculate(MeasurementPositionEntry[] entries, WeightingMode weighting, bool removeOutliers, double outlierSigmaCoeff, double instDelayTimeOfDay, double minUncertainty)
        {
            m_InstDelayTimeOfDay = instDelayTimeOfDay;

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

        internal double GetMidAreaTimeOfDay()
        {
            if (m_Entries.Count == 0)
                return 0;

            if (m_Entries.Count == 1)
                return m_Entries[0].TimeOfDayUTC;

            return (m_Entries[0].TimeOfDayUTC + m_Entries[m_Entries.Count - 1].TimeOfDayUTC) / 2.0;
        }

        internal string GetReportByTimeOfDay(double closestTimeOfDay, string obsCode, string designation, DateTime obsDate)
        {
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

        internal double MinRADeg
        {
            get { return m_Entries[0].RADeg; }
        }

        internal double MaxRADeg
        {
            get { return m_Entries[m_Entries.Count - 1].RADeg; }
        }

        internal double MinDEDeg
        {
            get { return m_Entries[0].DEDeg; }
        }

        internal double MaxDEDeg
        {
            get { return m_Entries[m_Entries.Count - 1].DEDeg; }
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
                        RAWeightDeg = weightsRA.Length > i ? weightsRA[i] / 3600.0 : 0,
                        DEWeightDeg = weightsDE.Length > i ? weightsDE[i] / 3600.0 : 0,
                    };
                }
            }
        }
    }

    internal class CalculatedEntry
    {
        public double TimeOfDayUTC;
        public double RADeg;
        public double DEDeg;
        public double RADegFitted;
        public double DEDegFitted;
        public double RAWeightDeg;
        public double DEWeightDeg;
    }
}
