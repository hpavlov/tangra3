using System;
using System.Collections.Generic;
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

        public void Calculate(IMeasurementPositionProvider provider, WeightingMode weighting, int numChunks, double outlierSigmaCoeff = 3.0)
        {
            if (provider != null)
            {
                var allEntries = provider.Measurements.ToList();
                var minUncertainty = provider.MinPositionUncertaintyPixels*provider.ArsSecsInPixel;
                double instDelayTimeOfDay = ((double) provider.InstrumentalDelaySec/SECONDS_IN_A_DAY);

                int numEntriesPerChunk = allEntries.Count / numChunks;

                for (int i = 0; i < numChunks; i++)
                {
                    int firstId = i * numEntriesPerChunk;
                    int lastId = (i + 1) * numEntriesPerChunk  - 1;
                    if (lastId > (numChunks - 1)*numEntriesPerChunk) lastId = allEntries.Count;

                    var chunkPosExtractor = new FastMotionChunkPositionExtractor(firstId, lastId);
                    var chunkEntries = allEntries.Skip(firstId).Take(lastId - firstId).ToArray();
                    chunkPosExtractor.Calculate(chunkEntries, weighting, outlierSigmaCoeff, instDelayTimeOfDay, minUncertainty);
                    m_Chunks.Add(chunkPosExtractor);
                }
            }
        }
     }

    internal class FastMotionChunkPositionExtractor
    {
        public int FirstEntryId { get; private set; }
        public int LastEntryId { get; private set; }

        private LinearRegression m_RegressionRA;
        private LinearRegression m_RegressionDE;

        private MeasurementPositionEntry[] m_Entries;

        public FastMotionChunkPositionExtractor(int firstEntryId, int lastEntryId)
        {
            FirstEntryId = firstEntryId;
            LastEntryId = lastEntryId;
        }

        public void Calculate(MeasurementPositionEntry[] entries, WeightingMode weighting, double outlierSigmaCoeff, double instDelayTimeOfDay, double minUncertainty)
        {
            m_Entries = entries;

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

            var secondPassEntriesRA = new List<MeasurementPositionEntry>();
            var secondPassEntriesDE = new List<MeasurementPositionEntry>();

            regRA.Solve();
            regDE.Solve();

            var outlierLimitRA = regRA.StdDev * outlierSigmaCoeff;
            var residualsRA = regRA.Residuals.ToArray();
            var outlierLimitDE = regDE.StdDev * outlierSigmaCoeff;
            var residualsDE = regDE.Residuals.ToArray();

            for (int i = 0; i < entries.Length; i++)
            {
                if (Math.Abs(residualsRA[i]) <= outlierLimitRA) secondPassEntriesRA.Add(entries[i]);
                if (Math.Abs(residualsDE[i]) <= outlierLimitDE) secondPassEntriesDE.Add(entries[i]);
            }

            m_RegressionRA = new LinearRegression();
            foreach (var entry in secondPassEntriesRA)
            {
                var midFrameTime = entry.TimeOfDayUTC - instDelayTimeOfDay;

                if (weighting == WeightingMode.None)
                    m_RegressionRA.AddDataPoint(midFrameTime, entry.RADeg);
                else
                {
                    var weight = CalulateWeight(entry, entry.SolutionUncertaintyRACosDEArcSec, minUncertainty, weighting);
                    m_RegressionRA.AddDataPoint(midFrameTime, entry.RADeg, weight);
                }
            }

            m_RegressionRA.Solve();

            m_RegressionDE = new LinearRegression();
            foreach (var entry in secondPassEntriesDE)
            {
                var midFrameTime = entry.TimeOfDayUTC - instDelayTimeOfDay;
                if (weighting == WeightingMode.None)
                    m_RegressionDE.AddDataPoint(midFrameTime, entry.DEDeg);
                else
                {
                    var weight = CalulateWeight(entry, entry.SolutionUncertaintyDEArcSec, minUncertainty, weighting);
                    m_RegressionDE.AddDataPoint(midFrameTime, entry.DEDeg, weight);
                }
            }
            m_RegressionDE.Solve();
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

        public string GetReportByTimeOfDay(double closestTimeOfDay, string obsCode, string designation, DateTime obsDate)
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
    }
}
