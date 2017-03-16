using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
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
        private List<MeasurementPositionEntry> m_AllEntries;
        private bool m_RemoveOutliers;
        private double m_OutliersSigmaThreashold = 3;



        public void Calculate(
            IMeasurementPositionProvider provider, 
            ReductionSettings settings)
        {
            if (provider != null)
            {
                m_Chunks.Clear();
                int numChunks = settings.NumberOfChunks;

                if (numChunks < 1) numChunks = 1;
                m_RemoveOutliers = settings.RemoveOutliers;
                m_OutliersSigmaThreashold = settings.OutliersSigmaThreashold;

                m_AllEntries = provider.Measurements.Where(x => x.TimeOfDayUTC != 0).ToList();
                m_AllEntries.ForEach(x =>
                {
                    x.ConstraintPoint = false;
                    x.MidConstraintPoint = false;
                });

                double instDelayTimeOfDay = ((double)settings.InstrumentalDelaySec / SECONDS_IN_A_DAY);

                int defPoints = 0;
                int numEntriesPerChunk = m_AllEntries.Count / numChunks;
                int startMidConIdx = 0;

                switch (settings.ConstraintPattern)
                {
                    case 1:
                    case 2:
                        defPoints = 3;
                        numEntriesPerChunk = (m_AllEntries.Count - defPoints * 2 * numChunks) / numChunks;
                        break;
                    case 3:
                        defPoints = 3;
                        numEntriesPerChunk = (m_AllEntries.Count - defPoints * 2 * numChunks) / numChunks;
                        startMidConIdx = (int)Math.Round((m_AllEntries.Count - defPoints * numChunks * 1.0) / 2.0);
                        var midConstPoints = m_AllEntries.Skip(startMidConIdx).Take(defPoints * numChunks).ToList();
                        midConstPoints.ForEach(x =>
                        {
                            x.ConstraintPoint = true;
                            x.MidConstraintPoint = true;
                        });
                        break;
                }

                int lastChunkFill = (m_AllEntries.Count - defPoints * numChunks - 1) - ((numChunks) * numEntriesPerChunk - 1 + defPoints * numChunks);
                for (int i = 0; i < numChunks; i++)
                {                    
                    int firstId = i * numEntriesPerChunk + defPoints * numChunks;
                    int lastId = (i + 1) * numEntriesPerChunk - 1 + defPoints * numChunks;
                    if (lastId > (numChunks - 1)*numEntriesPerChunk + defPoints*numChunks)
                    {
                        lastId = m_AllEntries.Count - defPoints * numChunks - 1;
                    }

                    var chunkPosExtractor = new FastMotionChunkPositionExtractor(firstId, lastId);
                    var chunkEntries = m_AllEntries.Skip(firstId).Take(lastId - firstId + 1).Where(x => !x.ConstraintPoint).ToList();


                    if (settings.ConstraintPattern == 1)
                    {
                        for (int j = 0; j < defPoints; j++)
                        {
                            var d1 = m_AllEntries[i * defPoints + j];
                            d1.ConstraintPoint = true;
                            chunkEntries.Insert(j, d1);
                            var d2 = m_AllEntries[(numEntriesPerChunk + defPoints) * numChunks + i * defPoints + j + lastChunkFill];
                            d2.ConstraintPoint = true;
                            chunkEntries.Add(d2);
                        }
                    }
                    else if (settings.ConstraintPattern == 2)
                    {
                        for (int j = 0; j < defPoints; j++)
                        {
                            var d1 = m_AllEntries[i + numChunks * j];
                            d1.ConstraintPoint = true;
                            chunkEntries.Insert(j, d1);
                            var d2 = m_AllEntries[(numEntriesPerChunk + defPoints) * numChunks + i + numChunks * j + lastChunkFill];
                            d2.ConstraintPoint = true;
                            chunkEntries.Add(d2);
                        }
                    }
                    else if (settings.ConstraintPattern == 3)
                    {
                        for (int j = 0; j < defPoints; j++)
                        {
                            var d1 = m_AllEntries[i + numChunks * j];
                            d1.ConstraintPoint = true;
                            chunkEntries.Insert(j, d1);
                            var d3 = m_AllEntries[(numEntriesPerChunk + defPoints) * numChunks + i + numChunks * j + lastChunkFill];
                            d3.ConstraintPoint = true;
                            chunkEntries.Add(d3);
                        }

                        var midIdx = chunkEntries.Count / 2;
                        for (int j = 0; j < defPoints; j++)
                        {
                            var d2 = m_AllEntries[startMidConIdx + i + numChunks * j];
                            d2.ConstraintPoint = true;
                            chunkEntries.Insert(midIdx + j, d2);
                        }
                    }

                    if (chunkEntries.Count > 2)
                    {
                        chunkPosExtractor.Calculate(
                            chunkEntries.ToArray(),
                            settings.Weighting,
                            settings.RemoveOutliers,
                            m_OutliersSigmaThreashold,
                            instDelayTimeOfDay,
                            settings.BestPositionUncertaintyArcSec,
                            settings.FactorInPositionalUncertainty,
                            settings.ErrorMethod,
                            settings.SmallestReportedUncertaintyArcSec);

                        m_Chunks.Add(chunkPosExtractor);
                    }
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

            double combinedUncert = Math.Sqrt(m_Chunks.Sum(x => x.GetMidPointCombinedSQUncertainty())) * 3600.0;
            Trace.WriteLine(string.Format("Combined total measurement uncertainty: {0} arcsec", combinedUncert));

            return lines.ToArray();
        }

        public void CalculateSingleMeasurement(double timeOfDay, out double raHours, out double deDeg, out double errRACosDEArcSec, out double errDEArcSec)
        {
            raHours = double.NaN;
            deDeg = double.NaN;
            errRACosDEArcSec = double.NaN;
            errDEArcSec = double.NaN;

            foreach (var chunk in m_Chunks)
            {
                if (chunk.MinTimeOfDayUTCInstrDelayApplied <= timeOfDay && chunk.MaxTimeOfDayUTCInstrDelayApplied >= timeOfDay)
                {
                    chunk.GetPosition(timeOfDay, out raHours, out errRACosDEArcSec, out deDeg, out errDEArcSec);
                    return;
                }
            }
        }

        public const int PADDING_L = 15;
        public const int PADDING_R = 5;
        public const int BORDER = 7;
        public const int TITLE_PADDING = 5;

        public void PlotRAFit(Graphics g, int fullWidth, int fullHeight)
        {
            if (m_Chunks.Count > 0)
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

            double minX = m_Chunks.Min(x => x.MinTimeOfDayUTCInstrDelayApplied);
            double maxX = m_Chunks.Max(x => x.MaxTimeOfDayUTCInstrDelayApplied);

            float scaleX = (float)(clientAreaWidth / (maxX - minX));
            float scaleY = (float)(clientAreaHeight / (maxY - minY));

            var repX = new List<double>();
            var repY = new List<double>();
            double? motionRate = null;

            Pen constraintPen = new Pen(Color.FromArgb(100, pen.Color));
            var allFrameNos = m_AllEntries.Select(x => x.FrameNo).ToList();
            foreach (var chunk in m_Chunks)
            {
                double? startX = null;
                double? startYCalc = null;
                CalculatedEntry lastEntry = null;
                foreach (var entry in chunk.Entries)
                {
                    if (!entry.IsConstraintPoint) lastEntry = entry;

                    float x = (float)Math.Round(PADDING_L + BORDER + (scaleX * (entry.TimeOfDayUTCInstrDelayApplied - minX)));
                    float y = fullHeight - PADDING_L - BORDER - (float)(scaleY * (getVal(entry) - minY));

                    if (!startX.HasValue && !entry.IsConstraintPoint)
                    {
                        startX = entry.TimeOfDayUTCInstrDelayApplied;
                        startYCalc = getCalcVal(entry);
                    }

                    var entryPen = entry.IsConstraintPoint ? constraintPen : pen;

                    g.DrawEllipse(entryPen, x - 1, y - 1, 2, 2);

                    float yErr = (float)(scaleY * getWeight(entry));
                    if (yErr > 0)
                    {
                        g.DrawLine(entryPen, x, y - yErr, x, y + yErr);
                        g.DrawLine(entryPen, x - 1, y - yErr, x + 1, y - yErr);
                        g.DrawLine(entryPen, x - 1, y + yErr, x + 1, y + yErr);
                    }

                    if (allFrameNos.IndexOf(entry.FrameNo) == -1)
                        Trace.WriteLine(string.Format("ERROR: Frame number {0} has been already used in another chunk!", entry.FrameNo));
                    else
                        allFrameNos.Remove(entry.FrameNo);
                }

                // Plot fitted line
                if (lastEntry != null)
                {
                    double endX = lastEntry.TimeOfDayUTCInstrDelayApplied;
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

                    var rate = Math.Abs((endYCalc - startYCalc.Value) * 3600.0 / ((endX - startX.Value) * SECONDS_IN_A_DAY));
                    if (!motionRate.HasValue || motionRate.Value < rate)
                        motionRate = rate;
                }
            }

            int allRemovedOutliers = m_Chunks.Sum(x => x.RemovedOutliers);

            // Plot total included points and outliers
            string statsText = m_RemoveOutliers
                ? string.Format("Included points: {0}, Excluded outliers: {1}", m_AllEntries.Count, allRemovedOutliers)
                : string.Format("Included points: {0}", m_AllEntries.Count);

            var szf = g.MeasureString(statsText, s_LegendFont);
            string rateText = null;
            SizeF rszf = SizeF.Empty;
            if (motionRate.HasValue)
            {
                rateText = string.Format("Rate({0}): {1:0.00}\"/sec", motionName, motionRate.Value);
                rszf = g.MeasureString(rateText, s_LegendFont);
            }

            bool topLeft = aveIncl > 0;
            if (topLeft)
            {
                g.FillRectangle(SystemBrushes.ControlDarkDark, PADDING_L + BORDER, PADDING_R + BORDER, szf.Width, szf.Height);
                g.DrawString(statsText, s_LegendFont, Brushes.Azure, PADDING_L + BORDER, PADDING_R + BORDER);

                g.FillRectangle(SystemBrushes.ControlDarkDark, fullWidth - PADDING_R - BORDER - rszf.Width, fullHeight - PADDING_L - BORDER - rszf.Height - 2, rszf.Width, rszf.Height);
                g.DrawString(rateText, s_LegendFont, Brushes.Azure, fullWidth - PADDING_R - BORDER - rszf.Width, fullHeight - PADDING_L - BORDER - rszf.Height - 2);
            }
            else
            {
                g.FillRectangle(SystemBrushes.ControlDarkDark, fullWidth - PADDING_R - BORDER - szf.Width, PADDING_R + BORDER, szf.Width, szf.Height);
                g.DrawString(statsText, s_LegendFont, Brushes.Azure, fullWidth - PADDING_R - BORDER - szf.Width, PADDING_R + BORDER);

                g.FillRectangle(SystemBrushes.ControlDarkDark, PADDING_L + BORDER, fullHeight - PADDING_L - BORDER - rszf.Height - 2, rszf.Width, rszf.Height);
                g.DrawString(rateText, s_LegendFont, Brushes.Azure, PADDING_L + BORDER, fullHeight - PADDING_L - BORDER - rszf.Height - 2);
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
                string text = string.Format("StdDev from {0} measurements: {1:0.00} arcsec", repX.Count, stdDevArcSec);
                szf = g.MeasureString(text, s_LegendFont);

                if (topLeft)
                {
                    g.FillRectangle(SystemBrushes.ControlDarkDark, PADDING_L + BORDER, PADDING_R + BORDER + szf.Height + 2, szf.Width, szf.Height);
                    g.DrawString(text, s_LegendFont, Brushes.Azure, PADDING_L + BORDER, PADDING_R + BORDER + szf.Height + 2);
                }
                else
                {
                    g.FillRectangle(SystemBrushes.ControlDarkDark, fullWidth - PADDING_R - BORDER - szf.Width, PADDING_R + BORDER + szf.Height + 2, szf.Width, szf.Height);
                    g.DrawString(text, s_LegendFont, Brushes.Azure, fullWidth - PADDING_R - BORDER - szf.Width, PADDING_R + BORDER + szf.Height + 2);
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
            if (m_Chunks.Count > 0)
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
    }

    internal class FastMotionChunkPositionExtractor
    {
        public int FirstEntryId { get; private set; }
        public int LastEntryId { get; private set; }

        private LinearRegression m_RegressionRA;
        private LinearRegression m_RegressionDE;

        private double m_InstDelayTimeOfDay;
        private WeightingMode m_Weighting;
        private double? m_PosUncertaintyMedArcSec;
        private ErrorMethod m_ErrorMethod;
        private double m_SmallestReportedUncertaintyArcSec;
        private double m_MinSinglePositionUncertainty;

        private List<MeasurementPositionEntry> m_Entries;

        public FastMotionChunkPositionExtractor(int firstEntryId, int lastEntryId)
        {
            FirstEntryId = firstEntryId;
            LastEntryId = lastEntryId;
        }

        public void Calculate(
            MeasurementPositionEntry[] entries, WeightingMode weighting, bool removeOutliers, double outlierSigmaCoeff, 
            double instDelayTimeOfDay, double minUncertainty,
            bool includePositionalUncertainties, ErrorMethod errorMethod, double smallestReportedUncertaintyArcSec)
        {
            m_InstDelayTimeOfDay = instDelayTimeOfDay;
            m_Weighting = weighting;
            m_ErrorMethod = errorMethod;
            m_SmallestReportedUncertaintyArcSec = smallestReportedUncertaintyArcSec;

            m_MinSinglePositionUncertainty = minUncertainty;

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
                    var weightRA = CalulateWeight(entry, entry.SolutionUncertaintyRACosDEArcSec);
                    var weightDE = CalulateWeight(entry, entry.SolutionUncertaintyDEArcSec);
                    regRA.AddDataPoint(midFrameTime, entry.RADeg, weightRA);
                    regDE.AddDataPoint(midFrameTime, entry.DEDeg, weightDE);
                }
            }

            m_Entries = new List<MeasurementPositionEntry>();

            regRA.Solve();
            regDE.Solve();

            RemovedOutliers = 0;

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
                    else
                        RemovedOutliers++;
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
                        var weightRA = CalulateWeight(entry, entry.SolutionUncertaintyRACosDEArcSec);
                        m_RegressionRA.AddDataPoint(midFrameTime, entry.RADeg, weightRA);

                        var weightDE = CalulateWeight(entry, entry.SolutionUncertaintyDEArcSec);
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

            if (includePositionalUncertainties)
            {
                var posUncertaintyAveLst = new List<double>();
                foreach (var entry in m_Entries)
                {
                    var posUncertainty = entry.FWHMArcSec / (2.355 * entry.SNR);
                    if (posUncertainty < m_MinSinglePositionUncertainty) posUncertainty = m_MinSinglePositionUncertainty;
                    posUncertaintyAveLst.Add(posUncertainty);
                }
                var posUncertaintyMedian = posUncertaintyAveLst.Median();                
                m_PosUncertaintyMedArcSec = posUncertaintyMedian / Math.Sqrt(posUncertaintyAveLst.Count);
            }
            else
            {
                m_PosUncertaintyMedArcSec = null;
            }
        }

        private double CalulateWeight(MeasurementPositionEntry entry, double solutionUncertaintyArcSec)
        {
            return FlyByMotionFitter.ComputeWeight(m_Weighting, solutionUncertaintyArcSec, entry.FWHMArcSec, entry.SNR, entry.DetectionCertainty, m_MinSinglePositionUncertainty);
        }

        internal double GetMidPointDelayCorrectedTimeOfDay()
        {
            if (m_Entries.Count == 0)
                return 0;

            if (m_Entries.Count == 1)
                return m_Entries[0].TimeOfDayUTC - m_InstDelayTimeOfDay;

            var dataEntriesOnly = m_Entries.Where(x => !x.ConstraintPoint).ToList();
            if (dataEntriesOnly.Count > 1)
                return (dataEntriesOnly[0].TimeOfDayUTC + dataEntriesOnly[dataEntriesOnly.Count - 1].TimeOfDayUTC)/2.0 - m_InstDelayTimeOfDay;
            else
                return double.NaN;
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

        internal void GetPosition(double timeOfDay, out double raHours, out double errRACosDEArcSec, out double deDeg, out double errDEArcSec)
        {
            raHours = m_RegressionRA.ComputeYWithError(timeOfDay, out errRACosDEArcSec, m_ErrorMethod) / 15.0;
            deDeg = m_RegressionDE.ComputeYWithError(timeOfDay, out errDEArcSec, m_ErrorMethod);

            errRACosDEArcSec *= Math.Cos(deDeg * Math.PI / 180) * 3600;
            errDEArcSec *= 3600;

            if (m_PosUncertaintyMedArcSec.HasValue)
            {
                errRACosDEArcSec = Math.Sqrt(errRACosDEArcSec * errRACosDEArcSec + m_PosUncertaintyMedArcSec.Value * m_PosUncertaintyMedArcSec.Value);
                errDEArcSec = Math.Sqrt(errDEArcSec * errDEArcSec + m_PosUncertaintyMedArcSec.Value * m_PosUncertaintyMedArcSec.Value);
            }

            if (errRACosDEArcSec < m_SmallestReportedUncertaintyArcSec) errRACosDEArcSec = m_SmallestReportedUncertaintyArcSec;
            if (errDEArcSec < m_SmallestReportedUncertaintyArcSec) errDEArcSec = m_SmallestReportedUncertaintyArcSec;
        }

        internal double GetMidPointCombinedSQUncertainty()
        {
            double closestTimeOfDay = GetMidPointDelayCorrectedTimeOfDay();

            if (m_RegressionRA != null && m_RegressionDE != null)
            {
                var normalTime = double.Parse(closestTimeOfDay.ToString("0.000000"));

                double errRA, errDE;
                m_RegressionRA.ComputeYWithError(normalTime, out errRA, m_ErrorMethod);
                m_RegressionDE.ComputeYWithError(normalTime, out errDE, m_ErrorMethod);

                return errRA * errRA + errDE * errDE;
            }

            return 0;
        }

        internal string GetMidPointReport(string obsCode, string designation, DateTime obsDate)
        {
            double closestTimeOfDay = GetMidPointDelayCorrectedTimeOfDay();

            MPCObsLine obsLine = new MPCObsLine(obsCode.PadLeft(3).Substring(0, 3));
            obsLine.SetObject(designation.PadLeft(12).Substring(0, 12));

            if (m_RegressionRA != null && m_RegressionDE != null)
            {
                var normalTime = double.Parse(closestTimeOfDay.ToString("0.000000"));
                if (double.IsNaN(normalTime) || double.IsInfinity(normalTime)) return null;

                DateTime mpcTime = obsDate.Date.AddDays(normalTime);
                double errRA, errDE;
                double mpcRAHours = m_RegressionRA.ComputeYWithError(normalTime, out errRA, m_ErrorMethod) / 15.0;
                double mpcDEDeg = m_RegressionDE.ComputeYWithError(normalTime, out errDE, m_ErrorMethod);

                errRA *= Math.Cos(mpcDEDeg * Math.PI / 180) * 3600;
                errDE *= 3600;

                if (m_PosUncertaintyMedArcSec.HasValue)
                {
                    errRA = Math.Sqrt(errRA * errRA + m_PosUncertaintyMedArcSec.Value * m_PosUncertaintyMedArcSec.Value);
                    errDE = Math.Sqrt(errDE * errDE + m_PosUncertaintyMedArcSec.Value * m_PosUncertaintyMedArcSec.Value);
                }

                if (errRA < m_SmallestReportedUncertaintyArcSec) errRA = m_SmallestReportedUncertaintyArcSec;
                if (errDE < m_SmallestReportedUncertaintyArcSec) errDE = m_SmallestReportedUncertaintyArcSec;

                var mag = m_Entries.Select(x => x.Mag).ToList().Median();

                obsLine.SetPosition(mpcRAHours, mpcDEDeg, mpcTime, true, TangraConfig.Settings.Astrometry.ExportHigherPositionAccuracy);
                obsLine.SetMagnitude(mag, MagnitudeBand.Cousins_R);

                if (TangraConfig.Settings.Astrometry.ExportUncertainties)
                    obsLine.SetUncertainty(errRA, errDE);

                return obsLine.BuildObservationASCIILine();
            }

            return null;
        }

        internal int NumDataPoints
        {
            get { return m_Entries.Count; }
        }

        internal double MinTimeOfDayUTCInstrDelayApplied
        {
            get { return m_Entries[0].TimeOfDayUTC - m_InstDelayTimeOfDay; }
        }

        internal double MaxTimeOfDayUTCInstrDelayApplied
        {
            get { return m_Entries[m_Entries.Count - 1].TimeOfDayUTC - m_InstDelayTimeOfDay; }
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

        internal int RemovedOutliers { get; private set; }

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
                        FrameNo = entry.FrameNo,
                        RADeg = entry.RADeg,
                        DEDeg = entry.DEDeg,
                        TimeOfDayUTCInstrDelayApplied = entry.TimeOfDayUTC - m_InstDelayTimeOfDay,
                        IsConstraintPoint = entry.ConstraintPoint,
                        IsMidConstraintPoint = entry.MidConstraintPoint,
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
        public int FrameNo;
        public bool IsFirstInSeries;
        public bool IsConstraintPoint;
        public bool IsMidConstraintPoint;
        public double TimeOfDayUTCInstrDelayApplied;
        public double RADeg;
        public double DEDeg;
        public double RADegFitted;
        public double DEDegFitted;
        public double RAWeightDeg;
        public double DEWeightDeg;
    }
}
