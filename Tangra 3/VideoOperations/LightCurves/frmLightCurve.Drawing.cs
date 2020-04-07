/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Tangra.Config;
using Tangra.Controller;
using Tangra.Model.Astro;
using Tangra.Helpers;
using Tangra.Model.Context;

namespace Tangra.VideoOperations.LightCurves
{
    public partial class frmLightCurve
    {
        private Bitmap m_Graph = null;
        private Bitmap m_SmallGraph = null;
        private float m_ScaleX;
    	private float m_TimestampScaleX;
        private float m_ScaleY;
        private List<uint> m_FramesIndex = new List<uint>();
        private List<uint> m_MeasurementsIndex = new List<uint>();
    	private bool[] m_IncludeObjects = new bool[4];

    	private int m_ZoomLevel = 0;
        private bool m_IsFirstDraw = true;
        private bool m_ZoomScrollMode = false;

        private void DrawGraph(Graphics g)
        {
            if (!LCMeasurementHeader.IsEmpty(m_Header) && m_LightCurveController.Context != null)
            {
                if (
                    m_Graph == null ||
                    (pnlChart.Width != m_Graph.Width || pnlChart.Height != m_Graph.Height) ||
                    m_LightCurveController.Context.Dirty || m_LightCurveController.Context.FirstZoomedFrameChanged)
                {
                    if (m_LightCurveController.Context.Dirty)
                    {
                        SetSmallGraphDirty();
                        pnlSmallGraph.Invalidate();
                    }

                    m_Graph = new Bitmap(pnlChart.Width, pnlChart.Height);

                    DrawGraphOnBitmap();

                    if (m_LightCurveController.Context.Dirty)
                    {
                        m_LightCurveController.Context.MarkClean();
                        UpdateContextDisplays();
                    }

                    if (m_NewMinDisplayedFrame != -1)
                    {
                        // This repaint has been done as a result of a zoom in/zoom out operation
                        // In this case we want to keep the selected frame drawn, if there is a selected frame
                        if (m_LightCurveController.Context.SelectedFrameNo >= m_Header.MinFrame &&
                            m_LightCurveController.Context.SelectedFrameNo <= m_Header.MaxFrame)
                        {
                            ReSelectCurrentMeasurement(false);
                        }

                        m_NewMinDisplayedFrame = -1;
                    }
                    else
                        ReDrawCurrentSelectedMeasurementLine();

                    if (m_IsFirstDraw)
                    {
                        m_IsFirstDraw = false;
                        m_LightCurveController.OnNewSelectedMeasurements(m_SelectedMeasurements);
                    }
                }

                g.DrawImage(m_Graph, 0, 0);
                g.Save();
            }
        }

        private void SetSmallGraphDirty()
        {
            if (m_SmallGraph != null)
                m_SmallGraph.Dispose();

            m_SmallGraph = null;
        }

    	private uint m_MinDisplayedFrame;
		private uint m_MaxDisplayedFrame;
    	private long m_MinDisplayedFrameTimestampTicks;
		private long m_MaxDisplayedFrameTimestampTicks;

		private void DrawGraphOnBitmap()
        {
            using (Graphics g = Graphics.FromImage(m_Graph))
            {
                DrawGraphOnBitmap(g);
            }
        }

        private void DrawGraphOnBitmap(Graphics g)
        {
            if (!m_Reprocessing)
            {
                if (m_Header.TimingType == MeasurementTimingType.EmbeddedTimeForEachFrame)
                    DrawGraphOnBitmapByTimeStamp(g);
                else
                    DrawGraphOnBitmapByFrameNumber(g);
            }
        }

        private void DrawGraphOnBitmapByTimeStamp(Graphics g)
		{
			if (!LCMeasurementHeader.IsEmpty(m_Header))
			{
				// i.e. signal, signal - noise, binned signal etc
				if (m_LightCurveController.Context.Dirty)
					ReComputeSignalSeries();

				if (m_ZoomLevel < 1) m_ZoomLevel = 1;

				if (m_ZoomLevel == 1)
					m_MinDisplayedFrame = m_Header.MinFrame;

				float totalWidth = (m_Header.MaxFrame - m_Header.MinFrame);

				if (m_NewMinDisplayedFrame != -1)
					m_MinDisplayedFrame = (uint)m_NewMinDisplayedFrame;

				m_MinDisplayedFrame = Math.Max(m_MinDisplayedFrame, m_Header.MinFrame);
				m_MaxDisplayedFrame = Math.Min((uint)(m_MinDisplayedFrame + totalWidth / m_ZoomLevel), m_Header.MaxFrame);

				m_MinDisplayedFrameTimestampTicks = m_Header.GetTimeForFrameFromFrameTiming(m_MinDisplayedFrame, true).Ticks;
				m_MaxDisplayedFrameTimestampTicks = m_Header.GetTimeForFrameFromFrameTiming(m_MaxDisplayedFrame, true).Ticks;

				btnZoomOut.Enabled = m_ZoomLevel > 1;
				sbZoomStartFrame.Visible = btnZoomOut.Enabled;

				m_FramesIndex.Clear();
				m_MeasurementsIndex.Clear();
				m_SelectedMeasurements = null;
				pnlMeasurementDetails.Visible = false;

				SetLegendColors();

			    bool drawLine = m_LightCurveController.Context.ChartType == LightCurveContext.LightCurveMode.Line;

				uint axisInterval = GetYAxisInterval(g);
				if (m_LightCurveController.Context.Dirty && axisInterval != 0)
				{
					m_Header.MinAdjustedReading =
						(int)(axisInterval * ((m_Header.MinAdjustedReading / axisInterval) - (m_Header.MinAdjustedReading > 0 ? 0 : 1)));
					m_Header.MaxAdjustedReading =
						(int)(axisInterval * ((m_Header.MaxAdjustedReading / axisInterval) + (m_Header.MaxAdjustedReading > 0 ? 1 : 0)));
				}

				float xTimestampScale = (float)(pnlChart.Width - m_MinX - m_MinY) / (float)(m_MaxDisplayedFrameTimestampTicks - m_MinDisplayedFrameTimestampTicks);
				if (m_Header.MaxFrame == m_Header.MinFrame) xTimestampScale = 0;

				float yScale = (float)((pnlChart.Height - 10) - m_MinY * 2) / (float)(m_Header.MaxAdjustedReading - m_Header.MinAdjustedReading);
				if (m_Header.MaxAdjustedReading == m_Header.MinAdjustedReading) yScale = 0;

				m_TimestampScaleX = xTimestampScale;
				m_ScaleY = yScale;
				m_MaxY = pnlChart.Height - m_MinY;
				m_MaxX = pnlChart.Width - m_MinY; /* this is correct we are using m_MinY for the right X axis spacing */

                g.FillRectangle(m_DisplaySettings.BackgroundColorBrush, 0, 0, m_Graph.Width, m_Graph.Height);

				if (double.IsNaN(xTimestampScale) || double.IsNaN(xTimestampScale))
				{
					g.DrawString("Error: Missing or corrupted readings",
						new Font(FontFamily.GenericSerif, 10),
						Brushes.Pink, 10, 10);
				}
				else
				{
					float datapointFrom = m_DisplaySettings.DatapointSize / 2.0f;
					float datapointSize = m_DisplaySettings.DatapointSize;

					int idx = 0;
					m_FramesIndex.Add((uint)m_MinDisplayedFrame);
					m_MeasurementsIndex.Add(0);
					double minAdjustedReading = double.MaxValue;

				    for (int i = 0; i < m_Header.ObjectCount; i++)
				    {
				        if (m_LightCurveController.Context.Binning > 0)
				        {
				            var binnedValues = m_AllBinnedReadings[i];
				            if (binnedValues.Any())
				            {
                                var minAdj = binnedValues.Min(x => x.AdjustedValue);
                                if (minAdj < minAdjustedReading) minAdjustedReading = minAdj;
				            }
				        }
				        else
				        {
				            var readings = m_LightCurveController.Context.AllReadings[i];
				            if (readings.Any())
				            {
                                var minAdj = readings.Min(x => x.AdjustedReading);
                                if (minAdj < minAdjustedReading) minAdjustedReading = minAdj;
				            }
				        }
				    }
                    DrawAxisByTimeStamp(g, axisInterval, minAdjustedReading);

				    for (int i = 0; i < m_Header.ObjectCount; i++)
					{
						if (!m_IncludeObjects[i]) continue;

						PointF prevPoint = PointF.Empty;
						List<BinnedValue> binnedValues = m_AllBinnedReadings[i];

						BinnedValue currBin = null;
						int binnedIdx = -1;
						int nextPlotIndex = -1;
						int readingIdx = 0;
						bool prevReadingIsOffScreen = true;
						foreach (LCMeasurement reading in m_LightCurveController.Context.AllReadings[i])
						{
							idx++;
							readingIdx++;
							bool drawThisReading = (reading.CurrFrameNo >= m_MinDisplayedFrame) && (reading.CurrFrameNo <= m_MaxDisplayedFrame);

							bool readingIsOffScreen = true;
							float adjustedReading = -1;
							if (m_LightCurveController.Context.Binning > 0)
							{
								if (currBin == null ||
									currBin.ReadingIndexTo <= readingIdx)
								{
									binnedIdx++;
									if (binnedIdx < binnedValues.Count)
									{
										currBin = binnedValues[binnedIdx];
										nextPlotIndex = (currBin.ReadingIndexFrom + currBin.ReadingIndexTo) / 2;
									}
								}

								if (nextPlotIndex == readingIdx &&
									currBin != null)
								{
									adjustedReading = (float)currBin.AdjustedValue;
									drawThisReading = true;
									// When binning draw all points regardless of whether all points in the bin
									// are for example off screen or not. Too difficult to handle
									readingIsOffScreen = false;
									prevReadingIsOffScreen = false;
								}
								else
									drawThisReading = false;

                                if (!m_DisplaySettings.DrawInvalidDataPoints && currBin != null && !currBin.IsSuccessfulReading) drawThisReading = false;
							}
							else
							{
								adjustedReading = reading.AdjustedReading;
								readingIsOffScreen = reading.IsOffScreen;
                                if (!m_DisplaySettings.DrawInvalidDataPoints && !reading.IsSuccessfulReading) drawThisReading = false;
							}

							if (drawThisReading && !readingIsOffScreen)
							{
								long currFrameTimestampTicks = m_Header.GetTimeForFrameFromFrameTiming((int)reading.CurrFrameNo, true).Ticks;
								float x = m_MinX + (currFrameTimestampTicks - m_MinDisplayedFrameTimestampTicks) * m_TimestampScaleX;

								float y = pnlChart.Height - (m_MinY + (adjustedReading - (int)m_Header.MinAdjustedReading) * yScale);

								if (x < m_MinX || x > m_MaxX)
									/* Excluding binned values outside the graph area */
									continue;

								if (float.IsNaN(y) || float.IsNaN(x) || float.IsInfinity(y) || float.IsInfinity(x))
									/* Excluding NaN binned values */
									continue;

								Pen pen = GetPenForTarget(i, reading.IsSuccessfulReading);
								Brush brush = GetBrushForTarget(i, reading.IsSuccessfulReading);

								if (prevPoint != PointF.Empty)
								{
									if (!prevReadingIsOffScreen && drawLine)
										g.DrawLine(pen, prevPoint.X, prevPoint.Y, x, y);
								}
								else
									prevPoint = new PointF();

								prevPoint.X = x;
								prevPoint.Y = y;


								try
								{
									g.FillEllipse(brush, x - datapointFrom, y - datapointFrom, datapointSize, datapointSize);
								}
								catch (OverflowException)
								{ }
							}
							prevReadingIsOffScreen = readingIsOffScreen;

							if (i == 0 && idx % 250 == 0)
							{
								m_FramesIndex.Add((uint)reading.CurrFrameNo);
								m_MeasurementsIndex.Add((uint)idx);
							}
						}
					}
				}

				if (!string.IsNullOrWhiteSpace(m_LightCurveController.Context.ChartTitle))
				{
					SizeF sizeF = g.MeasureString(m_LightCurveController.Context.ChartTitle, s_TitleFont);
					float x0 = (pnlChart.Width - sizeF.Width) / 2;
					float y0 = m_MinY + 6;
					g.FillRectangle(m_DisplaySettings.BackgroundColorBrush, x0 - 2, y0 - 2, sizeF.Width + 5, sizeF.Height + 4);
					g.DrawRectangle(m_DisplaySettings.LabelsPen, x0 - 2, y0 - 2, sizeF.Width + 5, sizeF.Height + 4);
					g.DrawString(m_LightCurveController.Context.ChartTitle, s_TitleFont, m_DisplaySettings.LabelsBrush, x0, y0);
				}

				g.Save();
			}
		}

        private void DrawGraphOnBitmapByFrameNumber(Graphics g)
        {
            if (!LCMeasurementHeader.IsEmpty(m_Header))
            {
                // i.e. signal, signal - noise, binned signal etc
                if (m_LightCurveController.Context.Dirty)
                    ReComputeSignalSeries();

				if (m_ZoomLevel < 1) m_ZoomLevel = 1;

                if (m_ZoomLevel == 1)
                    m_MinDisplayedFrame = m_Header.MinFrame;
                
				float totalWidth = (m_Header.MaxFrame - m_Header.MinFrame);

                if (m_NewMinDisplayedFrame != -1)
                    m_MinDisplayedFrame = (uint)m_NewMinDisplayedFrame;

                m_MinDisplayedFrame = Math.Max(m_MinDisplayedFrame, m_Header.MinFrame);
                m_MaxDisplayedFrame = Math.Min((uint)(m_MinDisplayedFrame + totalWidth / m_ZoomLevel), m_Header.MaxFrame);

                btnZoomOut.Enabled = m_ZoomLevel > 1;
                sbZoomStartFrame.Visible = btnZoomOut.Enabled;

                m_FramesIndex.Clear();
                m_MeasurementsIndex.Clear();
                m_SelectedMeasurements = null;
                pnlMeasurementDetails.Visible = false;

                SetLegendColors();

                bool drawLine = m_LightCurveController.Context.ChartType == LightCurveContext.LightCurveMode.Line;

                uint axisInterval = GetYAxisInterval(g);
                if (m_LightCurveController.Context.Dirty && axisInterval != 0)
                {
                    m_Header.MinAdjustedReading =
                        (int)(axisInterval * ((m_Header.MinAdjustedReading/axisInterval) - (m_Header.MinAdjustedReading > 0 ? 0 : 1)));
                    m_Header.MaxAdjustedReading =
                        (int)(axisInterval * ((m_Header.MaxAdjustedReading / axisInterval) + (m_Header.MaxAdjustedReading > 0 ? 1 : 0)));
                }

                float xScale = (float)(pnlChart.Width - m_MinX - m_MinY) / (float)(m_MaxDisplayedFrame - m_MinDisplayedFrame);
                if (m_Header.MaxFrame == m_Header.MinFrame) xScale = 0;

                float yScale = (float)((pnlChart.Height - 10) - m_MinY * 2) / (float)(m_Header.MaxAdjustedReading - m_Header.MinAdjustedReading);
                if (m_Header.MaxAdjustedReading == m_Header.MinAdjustedReading) yScale = 0;

                m_ScaleX = xScale;
                m_ScaleY = yScale;
                m_MaxY = pnlChart.Height - m_MinY;
                m_MaxX = pnlChart.Width - m_MinY; /* this is correct we are using m_MinY for the right X axis spacing */

                g.FillRectangle(m_DisplaySettings.BackgroundColorBrush, 0, 0, m_Graph.Width, m_Graph.Height);

                if (double.IsNaN(xScale) || double.IsNaN(xScale))
                {
                    g.DrawString("Error: Missing or corrupted readings",
                        new Font(FontFamily.GenericSerif, 10),
                        Brushes.Pink, 10, 10);
                }
                else
                {
                    float datapointFrom = m_DisplaySettings.DatapointSize / 2.0f;
                    float datapointSize = m_DisplaySettings.DatapointSize;

                    int idx = 0;
					m_FramesIndex.Add((uint)m_MinDisplayedFrame);
                    m_MeasurementsIndex.Add(0);

                    double minAdjustedReading = double.MaxValue;

                    for (int i = 0; i < m_Header.ObjectCount; i++)
                    {
                        if (m_LightCurveController.Context.Binning > 0)
                        {
                            var binnedValues = m_AllBinnedReadings[i];
                            if (binnedValues.Any())
                            {
                                var minAdj = binnedValues.Min(x => x.AdjustedValue);
                                if (minAdj < minAdjustedReading) minAdjustedReading = minAdj;
                            }
                        }
                        else
                        {
                            var readings = m_LightCurveController.Context.AllReadings[i];
                            if (readings.Any())
                            {
                                var minAdj = readings.Min(x => x.AdjustedReading);
                                if (minAdj < minAdjustedReading) minAdjustedReading = minAdj;
                            }
                        }
                    }

                    DrawAxis(g, axisInterval, minAdjustedReading);

                    for (int i = 0; i < m_Header.ObjectCount; i++)
                    {
						if (!m_IncludeObjects[i]) continue;

                        PointF prevPoint = PointF.Empty;
                        List<BinnedValue> binnedValues = m_AllBinnedReadings[i];

                        BinnedValue currBin = null;
                        int binnedIdx = -1;
                        int nextPlotIndex = -1;
                        int readingIdx = -1;
						bool prevReadingIsOffScreen = true;
                        foreach (LCMeasurement reading in m_LightCurveController.Context.AllReadings[i])
                        {
                            idx++;
                            readingIdx++;
                            bool drawThisReading = (reading.CurrFrameNo >= m_MinDisplayedFrame) && (reading.CurrFrameNo <= m_MaxDisplayedFrame);
								
                            bool readingIsOffScreen = true;
                            float adjustedReading = -1;
                            if (m_LightCurveController.Context.Binning > 0)
                            {
                                if (currBin == null || 
                                    currBin.ReadingIndexTo < readingIdx)
                                {
                                    binnedIdx++;
                                    if (binnedIdx < binnedValues.Count)
                                    {
                                        currBin = binnedValues[binnedIdx];
                                        nextPlotIndex = (currBin.ReadingIndexFrom + currBin.ReadingIndexTo) / 2;
                                    }
                                }

                                if (nextPlotIndex == readingIdx &&
                                    currBin != null)
                                {
                                    adjustedReading = (float) currBin.AdjustedValue;
                                    drawThisReading = true;
									// When binning draw all points regardless of whether all points in the bin
									// are for example off screen or not. Too difficult to handle
                                    readingIsOffScreen = false;
                                    prevReadingIsOffScreen = false;
                                }
                                else
                                    drawThisReading = false;


                                if (!m_DisplaySettings.DrawInvalidDataPoints && currBin != null && !currBin.IsSuccessfulReading) drawThisReading = false;
                            }
                            else
                            {
                                adjustedReading = reading.AdjustedReading;
								readingIsOffScreen = reading.IsOffScreen;
                                if (!m_DisplaySettings.DrawInvalidDataPoints && !reading.IsSuccessfulReading) drawThisReading = false;
                            }

                            if (drawThisReading && !readingIsOffScreen)
                            {
                                float x = m_MinX + ((int)reading.CurrFrameNo - (int)m_MinDisplayedFrame) * xScale;
                                float y = pnlChart.Height - (m_MinY + (adjustedReading - (int)m_Header.MinAdjustedReading) * yScale);

                                if (x < m_MinX || x > m_MaxX)
                                    /* Excluding binned values outside the graph area */
                                    continue;

                                if (float.IsNaN(y) || float.IsNaN(x) || float.IsInfinity(y) || float.IsInfinity(x))
                                    /* Excluding NaN binned values */
                                    continue;

								Pen pen = GetPenForTarget(i, reading.IsSuccessfulReading);
								Brush brush = GetBrushForTarget(i, reading.IsSuccessfulReading);

                                if (prevPoint != PointF.Empty)
                                {
                                    try
                                    {
                                        if (!prevReadingIsOffScreen && drawLine)
                                            g.DrawLine(pen, prevPoint.X, prevPoint.Y, x, y);
                                    }
                                    catch (OverflowException)
                                    { }
                                }
                                else
                                    prevPoint = new PointF();

                                prevPoint.X = x;
                                prevPoint.Y = y;

								try
								{
									g.FillEllipse(brush, x - datapointFrom, y - datapointFrom, datapointSize, datapointSize);
								}
								catch (OverflowException)
								{ }
                            }
							prevReadingIsOffScreen = readingIsOffScreen;

                            if (i == 0 && idx % 250 == 0)
                            {
                                m_FramesIndex.Add((uint)reading.CurrFrameNo);
                                m_MeasurementsIndex.Add((uint)idx);
                            }
                        }
                    }
                }

				if (!string.IsNullOrWhiteSpace(m_LightCurveController.Context.ChartTitle))
				{
					SizeF sizeF = g.MeasureString(m_LightCurveController.Context.ChartTitle, s_TitleFont);
					float x0 = (pnlChart.Width - sizeF.Width)/2;
					float y0 = m_MinY + 4;
					g.FillRectangle(m_DisplaySettings.BackgroundColorBrush, x0 - 2, y0 - 2, sizeF.Width + 5, sizeF.Height + 4);
					g.DrawRectangle(m_DisplaySettings.LabelsPen, x0 - 2, y0 - 2, sizeF.Width + 5, sizeF.Height + 4);
					g.DrawString(m_LightCurveController.Context.ChartTitle, s_TitleFont, m_DisplaySettings.LabelsBrush, x0, y0);
				}

                g.Save();
            }
        }

        private static Font s_BrandFont = new Font(FontFamily.GenericSansSerif, 5);
        private static Font s_AxisFont = new Font(FontFamily.GenericMonospace, 9);
		private static Font s_TitleFont = new Font(FontFamily.GenericSerif, 11, FontStyle.Bold);

        private void GetYAxisMagnitudeCalibration(Graphics g, double minAdjustedReading, out double intensityFrom, out double intensityTo, out double intensityFactor)
        {
            int magMin = (int)Math.Floor(m_LightCurveController.Context.MagnitudeConverter.ComputeMagnitude(Math.Max(0, minAdjustedReading * 0.9)) * 10) + 1;
            int magMax = (int)Math.Ceiling(m_LightCurveController.Context.MagnitudeConverter.ComputeMagnitude(m_Header.MaxAdjustedReading) * 10) - 1;

            SizeF labelSize = g.MeasureString("12.0", s_AxisFont);
            double maxLabels = (m_MaxY - m_MinY) / (labelSize.Height * 2);

            int[] best_intervals = new int[] { 1, 2, 5, 10, 20, 50 };

            int interval = 50;
            for (int i = 0; i < best_intervals.Length - 1; i++)
            {
                int numMarks = (int)Math.Ceiling((magMin - magMax) / (double)best_intervals[i]);
                if (numMarks >= 5 && numMarks <= 10 && maxLabels * 2 * numMarks < (m_MaxY - m_MinY))
                {
                    interval = best_intervals[i];
                    break;
                }
            }
            
            while (magMin % interval != 0) magMin += 1;
            intensityFactor = Math.Pow(10, (interval / 25.0));

            intensityFrom = m_LightCurveController.Context.MagnitudeConverter.ComputeFlux((magMin / 10.0) + 1);
            intensityTo = m_LightCurveController.Context.MagnitudeConverter.ComputeFlux((magMax / 10.0));
        }

        private void DrawAxisByTimeStamp(Graphics g, uint interval, double minAdjustedReading)
		{
			if (interval == 0) interval = 1;

			g.DrawLine(m_DisplaySettings.GridLinesPen, m_MinX, m_MinY, m_MinX, m_MaxY);
			g.DrawLine(m_DisplaySettings.GridLinesPen, m_MinX, m_MaxY, m_MaxX, m_MaxY);

            if (m_LightCurveController.Context.YAxisLabels == LightCurveContext.YAxisMode.Magnitudes && 
                m_LightCurveController.Context.MagnitudeConverter.CanComputeMagnitudes &&
                m_LightCurveController.Context.ProcessingType == ProcessingType.SignalMinusBackground)
            {

                double intensityFactor, intensityFrom, intensityTo;
                GetYAxisMagnitudeCalibration(g, minAdjustedReading, out intensityFrom, out intensityTo, out intensityFactor);

                double i = intensityFrom;
                float yMin = m_MaxY - (float)(i - intensityFrom) * m_ScaleY;
                float yMax = m_MaxY - (float)(i - intensityTo) * m_ScaleY;

                while (i < m_Header.MaxAdjustedReading)
                {
                    float magnitude = (float)m_LightCurveController.Context.MagnitudeConverter.ComputeMagnitude(i);
                    string label = magnitude.ToString("0.0");
                    SizeF labelSize = g.MeasureString(label, s_AxisFont);
                    float x = m_MinX - labelSize.Width;
                    float y = m_MaxY - (float)(i - m_Header.MinAdjustedReading) * m_ScaleY;

                    if (i > m_Header.MinAdjustedReading && m_DisplaySettings.DrawGrid)
                    {
                        g.DrawLine(m_DisplaySettings.GridLinesPen, m_MinX, y, m_MaxX, y);
                        g.DrawString(label, s_AxisFont, m_DisplaySettings.LabelsBrush, x, y - labelSize.Height / 2);
                    }

                    i *= intensityFactor;
                }

                string labelFlux = "Mag";
                SizeF labelFluxSize = g.MeasureString(labelFlux, s_AxisFont);

                g.DrawString(labelFlux, s_AxisFont, m_DisplaySettings.LabelsBrush, m_MinX - labelFluxSize.Width, m_MinY - labelFluxSize.Height);
            } 
            else
		    {
                for (int i = m_Header.MinAdjustedReading; i < m_Header.MaxAdjustedReading; i += (int)interval)
                {
                    string label = GetYAxisLabel(i);
                    SizeF labelSize = g.MeasureString(label, s_AxisFont);
                    float x = m_MinX - labelSize.Width;
                    float y = m_MaxY - (i - m_Header.MinAdjustedReading) * m_ScaleY;
                    g.DrawString(label, s_AxisFont, m_DisplaySettings.LabelsBrush, x, y - labelSize.Height / 2);
                    if (i != m_Header.MinAdjustedReading)
                    {
                        if (m_DisplaySettings.DrawGrid)
                            g.DrawLine(m_DisplaySettings.GridLinesPen, m_MinX, y, m_MaxX, y);
                        else
                        {
                            g.DrawLine(m_DisplaySettings.GridLinesPen, m_MinX, y, m_MinX + m_StrokeLen, y);
                            g.DrawLine(m_DisplaySettings.GridLinesPen, m_MaxX - m_StrokeLen, y, m_MaxX, y);
                        }
                    }
                        
                }

                string labelFlux = "ADU";
                SizeF labelFluxSize = g.MeasureString(labelFlux, s_AxisFont);

                g.DrawString(labelFlux, s_AxisFont, m_DisplaySettings.LabelsBrush, m_MinX - labelFluxSize.Width, m_MinY - labelFluxSize.Height);
		    }


		    g.DrawLine(m_DisplaySettings.GridLinesPen, m_MinX, m_MinY, m_MaxX, m_MinY);

			if (m_LightCurveController.Context.XAxisLabels == LightCurveContext.XAxisMode.FrameNo)
			{
                interval = GetXAxisInterval(g);

                uint firstMark = interval * (1 + m_MinDisplayedFrame / interval);

                for (uint i = firstMark; i <= m_MaxDisplayedFrame; i += interval)
                {
                    string label = i.ToString();
                    SizeF labelSize = g.MeasureString(label, s_AxisFont);

                    long currAxisPosTicks = m_Header.GetTimeForFrameFromFrameTiming(i, true).Ticks;
                    float x = m_MinX + (currAxisPosTicks - m_MinDisplayedFrameTimestampTicks) * m_TimestampScaleX;
                    float y = m_MaxY + 5;

                    if (firstMark != m_MinDisplayedFrame && x < m_MaxX)
                    {
						if (x + labelSize.Width / 2 < m_MaxX - 2 && x - labelSize.Width / 2 > m_MinX) 
							g.DrawString(label, s_AxisFont, m_DisplaySettings.LabelsBrush, x - labelSize.Width / 2, y);
                        if (m_DisplaySettings.DrawGrid)
                            g.DrawLine(m_DisplaySettings.GridLinesPen, x + 1, m_MinY, x + 1, m_MaxY);
                        else
                        {
                            g.DrawLine(m_DisplaySettings.GridLinesPen, x + 1, m_MinY, x + 1, m_MinY + m_StrokeLen);
                            g.DrawLine(m_DisplaySettings.GridLinesPen, x + 1, m_MaxY - m_StrokeLen, x + 1, m_MaxY);
                        }
                    }
                }
			}
            else if (m_LightCurveController.Context.XAxisLabels == LightCurveContext.XAxisMode.Time)
            {
                interval = GetXAxisTimeInterval(g);
                DateTime? firstTime = m_Header.GetFrameTime(m_MinDisplayedFrame);
                if (firstTime.HasValue)
                {
                    DateTime? frame1 = m_Header.GetFrameTime(m_MinDisplayedFrame);

                    for (uint i = m_MinDisplayedFrame; i < m_MaxDisplayedFrame; i++)
                    {
                        DateTime? frame2 = m_Header.GetFrameTime(i + 1);

                        if (frame1.HasValue && frame2.HasValue && frame1.Value != DateTime.MinValue && frame2.Value != DateTime.MinValue)
                        {
                            if (((int)(new TimeSpan(frame2.Value.Ticks - frame2.Value.Date.Ticks).TotalSeconds / interval) >
                                (int)(new TimeSpan(frame1.Value.Ticks - frame1.Value.Date.Ticks).TotalSeconds / interval)) ||
								(frame2.Value.Date.Ticks > frame1.Value.Date.Ticks) /* Date Change */)
                            {
                                string label = interval < 60 ? frame2.Value.ToString("HH:mm:ss") : frame2.Value.ToString("HH:mm");
                                SizeF labelSize = g.MeasureString(label, s_AxisFont);

                                long currAxisPosTicks = m_Header.GetTimeForFrameFromFrameTiming(i + 1, true).Ticks;
                                float x = m_MinX + (currAxisPosTicks - m_MinDisplayedFrameTimestampTicks) * m_TimestampScaleX;
                                float y = m_MaxY + 5;

								if (m_DisplaySettings.DrawGrid) 
                                    g.DrawLine(m_DisplaySettings.GridLinesPen, x + 1, m_MinY, x + 1, m_MaxY);
                                else
                                {
                                    g.DrawLine(m_DisplaySettings.GridLinesPen, x + 1, m_MinY, x + 1, m_MinY + m_StrokeLen);
                                    g.DrawLine(m_DisplaySettings.GridLinesPen, x + 1, m_MaxY - m_StrokeLen, x + 1, m_MaxY);
                                }

								if (x + labelSize.Width / 2 < m_MaxX - 2 && x - labelSize.Width / 2 > m_MinX) 
                                    g.DrawString(label, s_AxisFont, m_DisplaySettings.LabelsBrush, x - labelSize.Width / 2, y);
                            }
                        }

                        if (frame2 != null)
                            frame1 = frame2;
                    }

                    g.DrawString("UT", s_AxisFont, m_DisplaySettings.LabelsBrush, m_MaxX, m_MaxY + 5);
                }
            }

			g.DrawLine(m_DisplaySettings.GridLinesPen, m_MaxX, m_MinY, m_MaxX, m_MaxY);

		}

		private string GetYAxisLabel(int axisValue)
		{
			if (axisValue < 10000) return axisValue.ToString();
			else if (axisValue < 10000000) return string.Format("{0}K", axisValue / 1000);
			else return string.Format("{0}M", axisValue / 1000000);
		}

        private void DrawAxis(Graphics g, uint interval, double minAdjustedReading)
        {
			if (interval == 0) interval = 1; 

            g.DrawLine(m_DisplaySettings.GridLinesPen, m_MinX, m_MinY, m_MinX, m_MaxY);
            g.DrawLine(m_DisplaySettings.GridLinesPen, m_MinX, m_MaxY, m_MaxX, m_MaxY);

            if (m_LightCurveController.Context.YAxisLabels == LightCurveContext.YAxisMode.Magnitudes &&
                m_LightCurveController.Context.MagnitudeConverter.CanComputeMagnitudes &&
                m_LightCurveController.Context.ProcessingType == ProcessingType.SignalMinusBackground)
            {

                double intensityFactor, intensityFrom, intensityTo;
                GetYAxisMagnitudeCalibration(g, minAdjustedReading, out intensityFrom, out intensityTo, out intensityFactor);

                double i = intensityFrom;
                float yMin = m_MaxY - (float)(i - intensityFrom) * m_ScaleY;
                float yMax = m_MaxY - (float)(i - intensityTo) * m_ScaleY;

                var axisLabels = new List<Tuple<string, float, float>>();

                while (i < m_Header.MaxAdjustedReading)
                {
                    float magnitude = (float)m_LightCurveController.Context.MagnitudeConverter.ComputeMagnitude(i);
                    string label = magnitude.ToString("0.0");
                    SizeF labelSize = g.MeasureString(label, s_AxisFont);
                    float x = m_MinX - labelSize.Width;
                    float y = m_MaxY - (float)(i - m_Header.MinAdjustedReading) * m_ScaleY;

                    if (i > m_Header.MinAdjustedReading && m_DisplaySettings.DrawGrid)
                    {
                        g.DrawLine(m_DisplaySettings.GridLinesPen, m_MinX, y, m_MaxX, y);
                        axisLabels.Insert(0, Tuple.Create(label, x, y - labelSize.Height / 2));
                    }

                    i *= intensityFactor;
                }

                float prevPlottedLblBottom = 0;
                for (int j = 0; j < axisLabels.Count; j++)
                {
                    var entry = axisLabels[j];
                    SizeF labelSize = g.MeasureString(entry.Item1, s_AxisFont);
                    if (prevPlottedLblBottom < entry.Item3 - labelSize.Height/2)
                    {
                        g.DrawString(entry.Item1, s_AxisFont, m_DisplaySettings.LabelsBrush, entry.Item2, entry.Item3 - labelSize.Height / 2);
                        prevPlottedLblBottom = entry.Item3 + labelSize.Height / 2;
                    }
                }

                string labelFlux = "Mag";
                SizeF labelFluxSize = g.MeasureString(labelFlux, s_AxisFont);

                g.DrawString(labelFlux, s_AxisFont, m_DisplaySettings.LabelsBrush, m_MinX - labelFluxSize.Width, m_MinY - labelFluxSize.Height);
            } 
            else if (m_LightCurveController.Context.YAxisLabels == LightCurveContext.YAxisMode.Flux)
    	    {
                for (int i = m_Header.MinAdjustedReading; i < m_Header.MaxAdjustedReading; i += (int)interval)
                {
                    string label = GetYAxisLabel(i);

                    SizeF labelSize = g.MeasureString(label, s_AxisFont);
                    float x = m_MinX - labelSize.Width;
                    float y = m_MaxY - (i - m_Header.MinAdjustedReading) * m_ScaleY;
                    g.DrawString(label, s_AxisFont, m_DisplaySettings.LabelsBrush, x, y - labelSize.Height / 2);
                    if (i != m_Header.MinAdjustedReading)
                    {
                        if (m_DisplaySettings.DrawGrid)
                            g.DrawLine(m_DisplaySettings.GridLinesPen, m_MinX, y, m_MaxX, y);
                        else
                        {
                            g.DrawLine(m_DisplaySettings.GridLinesPen, m_MinX, y, m_MinX + m_StrokeLen, y);
                            g.DrawLine(m_DisplaySettings.GridLinesPen, m_MaxX - m_StrokeLen, y, m_MaxX, y);
                        }
                    }                        
                }

                string labelFlux = "ADU";
                SizeF labelFluxSize = g.MeasureString(labelFlux, s_AxisFont);

                g.DrawString(labelFlux, s_AxisFont, m_DisplaySettings.LabelsBrush, m_MinX - labelFluxSize.Width, m_MinY - labelFluxSize.Height);
    	    }
            

    	    g.DrawLine(m_DisplaySettings.GridLinesPen, m_MinX, m_MinY, m_MaxX, m_MinY);

			if (m_LightCurveController.Context.XAxisLabels == LightCurveContext.XAxisMode.FrameNo)
			{
				interval = GetXAxisInterval(g);

				uint firstMark = interval * (1 + m_MinDisplayedFrame / interval);

				for (uint i = firstMark; i <= m_MaxDisplayedFrame; i += interval)
				{
					string label = i.ToString();
					SizeF labelSize = g.MeasureString(label, s_AxisFont);
					float x = m_MinX + (i - m_MinDisplayedFrame) * m_ScaleX;
					float y = m_MaxY + 5;

					if (firstMark != m_MinDisplayedFrame && x < m_MaxX)
					{
						if (m_DisplaySettings.DrawGrid) 
                            g.DrawLine(m_DisplaySettings.GridLinesPen, x + 1, m_MinY, x + 1, m_MaxY);
						else
						{
                            g.DrawLine(m_DisplaySettings.GridLinesPen, x + 1, m_MinY, x + 1, m_MinY + m_StrokeLen);
                            g.DrawLine(m_DisplaySettings.GridLinesPen, x + 1, m_MaxY - m_StrokeLen, x + 1, m_MaxY);
						}

						if (x + labelSize.Width / 2 < m_MaxX - 2 && x - labelSize.Width / 2 > m_MinX) 
                            g.DrawString(label, s_AxisFont, m_DisplaySettings.LabelsBrush, x - labelSize.Width / 2, y);
					}
				}
			}
			else if (m_LightCurveController.Context.XAxisLabels == LightCurveContext.XAxisMode.Time)
			{
                interval = GetXAxisTimeInterval(g);
				DateTime? firstTime = m_LCFile.Header.GetFrameTime(m_MinDisplayedFrame);
                if (firstTime.HasValue)
                {
                    DateTime? frame1 = m_LCFile.Header.GetFrameTime(m_MinDisplayedFrame);

                    for (uint i = m_MinDisplayedFrame + 1; i < m_MaxDisplayedFrame; i++)
                    {
                        DateTime? frame2 = m_LCFile.Header.GetFrameTime(i + 1);

                        if (frame1.HasValue && frame2.HasValue && frame1.Value != DateTime.MinValue && frame2.Value != DateTime.MinValue)
                        {
                            if (((int)(new TimeSpan(frame2.Value.Ticks - frame2.Value.Date.Ticks).TotalSeconds/interval) >
                                (int)(new TimeSpan(frame1.Value.Ticks - frame1.Value.Date.Ticks).TotalSeconds/interval)) ||
								(frame2.Value.Date.Ticks > frame1.Value.Date.Ticks) /* Date Change */)
                            {
                                string label = interval < 60 ? frame2.Value.ToString("HH:mm:ss") : frame2.Value.ToString("HH:mm");
                                SizeF labelSize = g.MeasureString(label, s_AxisFont);
                                float x = m_MinX + (i + 1 - m_MinDisplayedFrame) * m_ScaleX;
                                float y = m_MaxY + 5;

								if (x + labelSize.Width / 2 < m_MaxX - 2 && x - labelSize.Width / 2 > m_MinX) 
									g.DrawString(label, s_AxisFont, m_DisplaySettings.LabelsBrush, x - labelSize.Width / 2, y);
								if (m_DisplaySettings.DrawGrid) 
                                    g.DrawLine(m_DisplaySettings.GridLinesPen, x + 1, m_MinY, x + 1, m_MaxY);
								else
								{
                                    g.DrawLine(m_DisplaySettings.GridLinesPen, x + 1, m_MinY, x + 1, m_MinY + m_StrokeLen);
                                    g.DrawLine(m_DisplaySettings.GridLinesPen, x + 1, m_MaxY - m_StrokeLen, x + 1, m_MaxY);
								}
                            }
                        }

                        if (frame2 != null)
                            frame1 = frame2;
                    }

                    g.DrawString("UT", s_AxisFont, m_DisplaySettings.LabelsBrush, m_MaxX, m_MaxY + 5);
                }
			}

            g.DrawLine(m_DisplaySettings.GridLinesPen, m_MaxX, m_MinY, m_MaxX, m_MaxY);
        }

        private void SetLegendColors()
        {
            pb1.Visible = false; pb2.Visible = false; pb3.Visible = false; pb4.Visible = false;
            lblMeasurement1.Visible = false; lblMeasurement2.Visible = false; lblMeasurement3.Visible = false; lblMeasurement4.Visible = false;
			lblMagnitude1.Visible = false; lblMagnitude2.Visible = false; lblMagnitude3.Visible = false; lblMagnitude4.Visible = false;
            picTarget1Pixels.Visible = false; picTarget2Pixels.Visible = false; picTarget3Pixels.Visible = false; picTarget4Pixels.Visible = false;
            picTarget1PSF.Visible = false; picTarget2PSF.Visible = false; picTarget3PSF.Visible = false; picTarget4PSF.Visible = false;
			lblSNLBL1.Visible = false; lblSNLBL2.Visible = false; lblSNLBL3.Visible = false; lblSNLBL4.Visible = false;
			lblSN1.Visible = false; lblSN2.Visible = false; lblSN3.Visible = false; lblSN4.Visible = false;
            
            if (m_Header.ObjectCount > 0 && m_IncludeObjects[0])
            {
                pb1.BackColor = m_DisplaySettings.Target1Color;
				pb1.Visible = true; lblMeasurement1.Visible = true; lblMagnitude1.Visible = true;
				lblSNLBL1.Visible = true; lblSN1.Visible = true;
                picTarget1Pixels.Visible = true; picTarget1PSF.Visible = true;
            }

			if (m_Header.ObjectCount > 1 && m_IncludeObjects[1])
            {
                pb2.BackColor = m_DisplaySettings.Target2Color;
				pb2.Visible = true; lblMeasurement2.Visible = true; lblMagnitude2.Visible = true;
				lblSNLBL2.Visible = true; lblSN2.Visible = true;
                picTarget2Pixels.Visible = true; picTarget2PSF.Visible = true;
            }

			if (m_Header.ObjectCount > 2 && m_IncludeObjects[2])
            {
                pb3.BackColor = m_DisplaySettings.Target3Color;
				pb3.Visible = true; lblMeasurement3.Visible = true; lblMagnitude3.Visible = true;
				lblSNLBL3.Visible = true; lblSN3.Visible = true;
                picTarget3Pixels.Visible = true; picTarget3PSF.Visible = true;
            }

			if (m_Header.ObjectCount > 3 && m_IncludeObjects[3])
            {
                pb4.BackColor = m_DisplaySettings.Target4Color;
				pb4.Visible = true; lblMeasurement4.Visible = true; lblMagnitude4.Visible = true;
				lblSNLBL4.Visible = true; lblSN4.Visible = true;
                picTarget4Pixels.Visible = true; picTarget4PSF.Visible = true;
            }
        }

        private Pen GetPenForTarget(int targetNo, bool isGoodReading)
        {
			if (targetNo >= 0 && targetNo <= 3)
			{
				if (isGoodReading)
					return m_DisplaySettings.TargetPens[targetNo];
				else
					return m_DisplaySettings.WarningColorPens[targetNo];
			}

        	return Pens.Black;
        }

		private Brush GetBrushForTarget(int targetNo, bool isGoodReading)
        {
			if (targetNo >= 0 && targetNo <= 3)
			{
				if (isGoodReading)
					return m_DisplaySettings.TargetBrushes[targetNo];
				else
					return m_DisplaySettings.WarningColorBrushes[targetNo];
			}
			return Brushes.Black;
        }

        private void SelectFrame(uint currFrameNo, bool movePlayerToFrame)
        {
            m_ZoomScrollMode = false;
            pnlSmallGraph.Invalidate();

            m_LightCurveController.Context.SelectedFrameNo = currFrameNo;
            m_AddinsController.OnLightCurveCurrentFrameChanged((int)currFrameNo);
            SetSmallGraphDirty();

            ReSelectCurrentMeasurement(movePlayerToFrame);
        }


        private void ReSelectCurrentMeasurement(bool movePlayerToFrame)
        {
            int fromId = 0;
            int toId = (int)m_Header.MeasuredFrames - 1;

            for (int i = 0; i < m_FramesIndex.Count; i++)
            {
                if (m_FramesIndex[i] < m_LightCurveController.Context.SelectedFrameNo)
                    fromId = (int)m_MeasurementsIndex[i];

                if (m_FramesIndex[i] > m_LightCurveController.Context.SelectedFrameNo)
                {
                    toId = (int)m_MeasurementsIndex[i];
                    break;
                }
            }

            for (int i = fromId; i < toId; i++)
            {
                if (m_LightCurveController.Context.SelectedFrameNo >= m_LightCurveController.Context.AllReadings[0][i].CurrFrameNo &&
                    (i + 1 == m_LightCurveController.Context.AllReadings[0].Count || m_LightCurveController.Context.SelectedFrameNo <= m_LightCurveController.Context.AllReadings[0][i + 1].CurrFrameNo))
                {
                    int idx = m_LightCurveController.Context.SelectedFrameNo == m_LightCurveController.Context.AllReadings[0][i].CurrFrameNo ? i : i + 1;

                    SelectMeasurement(idx);
                    DisplayCurrentMeasurements();

					if (movePlayerToFrame &&
                        TangraContext.Current.HasVideoLoaded)
					{
                        m_LightCurveController.MoveToFrameNoIntegrate((int)m_LightCurveController.Context.SelectedFrameNo);
					}
					 
                    break;
                }
            }
        }

        private void ReDrawCurrentSelectedMeasurementLine()
        {
            if (m_LightCurveController.Context.SelectedFrameNo >= m_MinDisplayedFrame &&
                m_LightCurveController.Context.SelectedFrameNo <= m_MaxDisplayedFrame)
            {
                int fromId = 0;
                int toId = (int)m_Header.MeasuredFrames - 1;

                for (int i = 0; i < m_FramesIndex.Count; i++)
                {
                    if (m_FramesIndex[i] < m_LightCurveController.Context.SelectedFrameNo)
                        fromId = (int)m_MeasurementsIndex[i];

                    if (m_FramesIndex[i] > m_LightCurveController.Context.SelectedFrameNo)
                    {
                        toId = (int)m_MeasurementsIndex[i];
                        break;
                    }
                }

                for (int i = fromId; i < toId; i++)
                {
                    if (m_LightCurveController.Context.SelectedFrameNo >= m_LightCurveController.Context.AllReadings[0][i].CurrFrameNo &&
                        (i + 1 == m_LightCurveController.Context.AllReadings[0].Count || m_LightCurveController.Context.SelectedFrameNo <= m_LightCurveController.Context.AllReadings[0][i + 1].CurrFrameNo))
                    {
                        int idx = m_LightCurveController.Context.SelectedFrameNo == m_LightCurveController.Context.AllReadings[0][i].CurrFrameNo ? i : i + 1;

                        SelectMeasurement(idx);

                        break;
                    }
                }                
            }
        }

        private LCMeasurement[] m_SelectedMeasurements = null;
        private Dictionary<int, Color> m_OldLineBackup = new Dictionary<int, Color>();
        private void SelectMeasurement(int? nullableId)
        {
	        bool hasSelection = nullableId.HasValue;
	        int id = nullableId.HasValue ? nullableId.Value : -1;

            if (m_Graph == null) return;

			float datapointFrom = m_DisplaySettings.DatapointSize / 2.0f;
			float datapointSize = m_DisplaySettings.DatapointSize;

            using (Graphics g = Graphics.FromImage(m_Graph))
            {
                if (m_SelectedMeasurements != null &&
                    m_SelectedMeasurements.Length > 0)
                {
                    float x = float.NaN;

                    // 'Deselect' the old objects
                    for (int i = 0; i < m_Header.ObjectCount; i++)
                    {
                        if (!m_IncludeObjects[i]) continue;

                        LCMeasurement reading = m_SelectedMeasurements[i];

                        if (!LCMeasurement.IsEmpty(reading) &&
							!reading.IsOffScreen)
                        {
							if (m_Header.TimingType == MeasurementTimingType.EmbeddedTimeForEachFrame)
							{
								long currFrameTimestampTicks = m_Header.GetTimeForFrameFromFrameTiming((int)reading.CurrFrameNo, true).Ticks;
								x = m_MinX + (currFrameTimestampTicks - m_MinDisplayedFrameTimestampTicks) * m_TimestampScaleX;
							}
							else
								x = m_MinX + (reading.CurrFrameNo - m_MinDisplayedFrame) * m_ScaleX;

                            if (m_LightCurveController.Context.Binning == 0)
                            {
                                // Only draw the points if there is no binning
                                float y = pnlChart.Height - (m_MinY + (reading.AdjustedReading - m_Header.MinAdjustedReading) * m_ScaleY);

                                bool restoreThisReading = m_DisplaySettings.DrawInvalidDataPoints || reading.IsSuccessfulReading;
                                if (restoreThisReading)
                                {
                                    Brush brush = GetBrushForTarget(i, reading.IsSuccessfulReading);
									try
									{
										g.FillEllipse(brush, x - datapointFrom, y - datapointFrom, datapointSize, datapointSize);
									}
									catch (OverflowException)
									{ }                                    
                                }
                            }
                        }
                    }

                    if (!float.IsNaN(x))
                    {
                        int intX = (int)Math.Round(x) + 1;
						if (intX >= 0 && intX < m_Graph.Width)
						{
							foreach (int y in m_OldLineBackup.Keys)
							{
								m_Graph.SetPixel(intX, y, m_OldLineBackup[y]);
							}
						}
                    }
                }
                else
                    m_SelectedMeasurements = new LCMeasurement[m_Header.ObjectCount];

	            if (id < 0) id = 0;
				if (id >= m_LightCurveController.Context.AllReadings[0].Count) id = m_LightCurveController.Context.AllReadings[0].Count - 1;

                if (hasSelection)
                {
                    m_OldLineBackup.Clear();

                    if (m_LightCurveController.Context.AllReadings[0][id].CurrFrameNo <= m_MaxDisplayedFrame &&
                        m_LightCurveController.Context.AllReadings[0][id].CurrFrameNo >= m_MinDisplayedFrame)
                    {
                    	float x;
						
						if (m_Header.TimingType == MeasurementTimingType.EmbeddedTimeForEachFrame)
						{
							long currFrameTimestampTicks = m_Header.GetTimeForFrameFromFrameTiming((int)m_LightCurveController.Context.AllReadings[0][id].CurrFrameNo, true).Ticks;
							x = m_MinX + (currFrameTimestampTicks - m_MinDisplayedFrameTimestampTicks) * m_TimestampScaleX;
						}
						else
							x = m_MinX + (m_LightCurveController.Context.AllReadings[0][id].CurrFrameNo - m_MinDisplayedFrame) * m_ScaleX;

                        int intX = (int)Math.Round(x) + 1;
                        for (int   i = m_MinY; i <= m_MaxY; i++)
                        {
							if (intX >= 0 && intX <= m_Graph.Width && i >= 0 && i <= m_Graph.Height)
								m_OldLineBackup.Add(i, m_Graph.GetPixel(intX, i));
                        }

                        try
                        {
                            g.DrawLine(m_DisplaySettings.SelectionCursorColorPen, intX, m_MinY, intX, m_MaxY);
                        }
                        catch (OverflowException)
                        { }						

                        for (int i = 0; i < m_Header.ObjectCount; i++)
                        {
                            LCMeasurement reading = m_LightCurveController.Context.AllReadings[i][id];

                            m_SelectedMeasurements[i] = reading;

                            if (!m_IncludeObjects[i]) continue;

                            bool drawThisReading = m_DisplaySettings.DrawInvalidDataPoints || reading.IsSuccessfulReading;

                            if (m_LightCurveController.Context.Binning == 0 && !reading.IsOffScreen && drawThisReading)
                            {
                                float y = pnlChart.Height - (m_MinY + (reading.AdjustedReading - m_Header.MinAdjustedReading) * m_ScaleY);
								try
								{
									g.FillEllipse(m_DisplaySettings.SelectionCursorColorBrush, x - datapointFrom, y - datapointFrom, datapointSize, datapointSize);
								}
								catch (OverflowException)
								{ }
                            }
                        }
                    }

					if (!pnlMeasurementDetails.Visible && m_SmallGraph == null)
						pnlMeasurementDetails.Visible = true;
                }
                else
                {
					m_OldLineBackup.Clear();
	                m_SelectedMeasurements = null;
					if (pnlMeasurementDetails.Visible)
						pnlMeasurementDetails.Visible = false;
                }

                g.Save();
            }

            PlotMeasuredPixels();
            PlotGaussians();

            using (Graphics g = pnlChart.CreateGraphics())
            {
                g.DrawImage(m_Graph, 0, 0);
            }
        }

        private void PlotMeasuredPixels()
        {
			if (!TangraContext.Current.CanProcessLightCurvePixels)
				return;

            PictureBox[] targetBoxes = new PictureBox[] { picTarget1Pixels, picTarget2Pixels, picTarget3Pixels, picTarget4Pixels };

			if (m_SelectedMeasurements != null)
			{
				for (int i = 0; i < m_SelectedMeasurements.Length; i++)
				{
					LCMeasurement reading = m_SelectedMeasurements[i];
					if (!LCMeasurement.IsEmpty(reading) &&
						reading.TargetNo >= 0 &&
						reading.TargetNo <= 3)
					{
						PlotSingleTargetPixels(targetBoxes[reading.TargetNo], reading);
					}
				}				
			}
        }

        private void PlotSingleTargetPixels(PictureBox pictureBox, LCMeasurement reading)
        {
			if (!TangraContext.Current.CanProcessLightCurvePixels)
				return;

            Bitmap bmp = pictureBox.Image as Bitmap;
			if (bmp != null)
            {

                int pixelsCenterX = (int)Math.Round(reading.X0);
                int pixelsCenterY = (int)Math.Round(reading.Y0);

                for (int x = 0; x < 17; x++)
                    for (int y = 0; y < 17; y++)
                    {
                        // NOTE: Need the +9 to convert from 17x17 coordinates to 35x35
                        uint pixelValue = reading.PixelData[x + 9, y + 9];
                        byte pixcolor = m_LightCurveController.Context.DisplayBitmapConverter.ToDisplayBitmapByte(pixelValue);
                        Color pixelcolor = Color.FromArgb(pixcolor, pixcolor, pixcolor);
                        bmp.SetPixel(2 * x, 2 * y, pixelcolor);
                        bmp.SetPixel(2 * x + 1, 2 * y, pixelcolor);
                        bmp.SetPixel(2 * x, 2 * y + 1, pixelcolor);
                        bmp.SetPixel(2 * x + 1, 2 * y + 1, pixelcolor);
                    }

                using (Graphics g = Graphics.FromImage(bmp))
                {
                    float radius = m_LightCurveController.Context.ReProcessApertures[reading.TargetNo]*2;

					g.DrawEllipse(
							m_DisplaySettings.TargetPens[reading.TargetNo],
							(float)(2 * (reading.X0 - pixelsCenterX) + 17 - radius),
							(float)(2 * (reading.Y0 - pixelsCenterY) + 17 - radius),
							2 * radius,
							2 * radius);

                    g.Save();
                }

                pictureBox.Refresh();
            }
        }

        private void PlotGaussians()
        {
            PictureBox[] psfBoxes = new PictureBox[] { picTarget1PSF, picTarget2PSF, picTarget3PSF, picTarget4PSF };

			if (m_SelectedMeasurements != null)
			{
				for (int i = 0; i < m_SelectedMeasurements.Length; i++)
				{
					LCMeasurement reading = m_SelectedMeasurements[i];
					if (!LCMeasurement.IsEmpty(reading) &&
						reading.TargetNo >= 0 &&
						reading.TargetNo <= 3)
					{
						if (reading.PsfFit == null)
						{
							int x0Int = (int)Math.Round(reading.X0);
							int y0Int = (int)Math.Round(reading.Y0);

							reading.PsfFit = new PSFFit(x0Int, y0Int);
							reading.PsfFit.FittingMethod = PSFFittingMethod.NonLinearFit;
							int pixelDataWidth = reading.PixelData.GetLength(0);
							int pixelDataHeight = reading.PixelData.GetLength(1);
							reading.PsfFit.Fit(
								reading.PixelData,
								m_LightCurveController.Context.ReProcessFitAreas[reading.TargetNo],
								x0Int - reading.PixelDataX0 + (pixelDataWidth / 2) + 1,
								y0Int - reading.PixelDataY0 + (pixelDataHeight / 2) + 1,
								false);
						}

						psfBoxes[reading.TargetNo].Visible = true;
						PlotSingleGaussian(psfBoxes[reading.TargetNo], reading, m_LightCurveController.Context.ReProcessApertures[i], m_DisplaySettings.TargetBrushes, m_Footer.ReductionContext.BitPix);
					}
				}
			}
        }

        internal static void PlotSingleGaussian(
            PictureBox pictureBox,
            LCMeasurement reading,
            float aperture,
            Brush[] allBrushes,
			int bpp)
        {

            Bitmap bmp = pictureBox.Image as Bitmap;
            if (bmp != null)
            {
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);

                using (Graphics g = Graphics.FromImage(bmp))
                {
                    reading.PsfFit.DrawInternalPoints(g, rect, aperture, allBrushes[reading.TargetNo], bpp);
                    g.Save();
                }
            }

            pictureBox.Refresh();
        }

        private SpinLock m_SpinLock = new SpinLock();

		internal void OnNewSelectedMeasurements(LCMeasurement[] selectedMeasurements)
	    {
            if (m_frmZoomedPixels != null && m_frmZoomedPixels.Visible)
				m_frmZoomedPixels.HandleNewSelectedFrame(selectedMeasurements);

            if (m_frmPSFFits != null && m_frmPSFFits.Visible)
				m_frmPSFFits.HandleNewSelectedFrame(selectedMeasurements);

            if (m_frmBackgroundHistograms != null && m_frmBackgroundHistograms.Visible)
				m_frmBackgroundHistograms.HandleNewSelectedFrame(selectedMeasurements);

			if (m_SelectedMeasurements == null ||
			    selectedMeasurements[0].CurrFrameNo != m_SelectedMeasurements[0].CurrFrameNo)
			{
                m_SelectedMeasurements = selectedMeasurements;
				SelectMeasurement((int)(selectedMeasurements[0].CurrFrameNo - m_Header.MinFrame));
				DisplayCurrentMeasurements();
			}
	    }


	    internal void HandleNewSelectedFrame(int newFrameId)
	    {
		    bool lockTaken = false;
			m_SpinLock.Enter(ref lockTaken);
            try
            {
				if (newFrameId < m_MinDisplayedFrame ||
					newFrameId > m_MaxDisplayedFrame)
                {
                    // The selected frme is outside the current window
                    if (newFrameId < m_Header.MinFrame ||
                        newFrameId > m_Header.MaxFrame)
                    {
                        SelectMeasurement(null);
                        NotificationManager.Instance.NotifyUserRequestToChangeCurrentFrame(null);
                        return;
                    }

                    // If this is zoom mode, then move the window to cover the frame
                    if (m_ZoomLevel > 1)
                        SlideWindowToMiddleFrame((uint) newFrameId);

                    // TODO: Doesn't select the frame if the window has been moved
                }

                if (
                    m_SelectedMeasurements != null &&
                    !LCMeasurement.IsEmpty(m_SelectedMeasurements[0]) &&
                    m_SelectedMeasurements[0].CurrFrameNo == newFrameId)
                {
                    NotificationManager.Instance.NotifyUserRequestToChangeCurrentFrame(m_SelectedMeasurements);
                    return;
                }

                SelectFrame((uint)newFrameId, false);

                NotificationManager.Instance.NotifyUserRequestToChangeCurrentFrame(m_SelectedMeasurements);
            }
            finally
            {
				if (lockTaken)
					m_SpinLock.Exit();
            }
        }


        private void DisplayCurrentMeasurements()
        {
            if (m_SelectedMeasurements != null)
            {
	            LCMeasurement firstNonExcludedMeasurement = m_SelectedMeasurements.FirstOrDefault(x => !LCMeasurement.IsEmpty(x));

				if (!LCMeasurement.IsEmpty(firstNonExcludedMeasurement))
				{
				    lblFrameNo.Text = firstNonExcludedMeasurement.CurrFrameNo.ToString();
                    if (firstNonExcludedMeasurement.CurrFileName != null) lblFrameNo.Text += string.Format("     {0}", firstNonExcludedMeasurement.CurrFileName);
					string correctedForInstrumentalDelayMessage = null;
					if (m_LCFile.CanDetermineFrameTimes)
					{
						string frameTimeLbl = m_LCFile.GetTimeForFrame(firstNonExcludedMeasurement.CurrFrameNo, out correctedForInstrumentalDelayMessage).ToString("HH:mm:ss.fff");

						if (!m_LCFile.Footer.AavNtpNoFittingUsed && !float.IsNaN(m_LCFile.Footer.AavNtpFitOneSigmaError))
							frameTimeLbl += string.Format(" +/-{0} ms", m_LCFile.Footer.AavNtpFitOneSigmaError.ToString("0.0"));

						lblFrameTime.Text = frameTimeLbl;

						if (correctedForInstrumentalDelayMessage != null)
						{
							lblInstDelayWarning.ForeColor = Color.Green;
							lblFrameTime.BackColor = SystemColors.Control;
							toolTip1.SetToolTip(lblInstDelayWarning, correctedForInstrumentalDelayMessage);
							toolTip1.SetToolTip(lblFrameTime, correctedForInstrumentalDelayMessage);
						}
                        else if (m_LightCurveController.Context.InstrumentalDelayCorrectionsNotRequired)
                        {
                            lblInstDelayWarning.ForeColor = Color.Green;
                            lblFrameTime.BackColor = SystemColors.Control;
                            toolTip1.SetToolTip(lblFrameTime, "Instrumental delay correction not required");
                        }
						else
						{
							lblInstDelayWarning.ForeColor = Color.Red;
							lblFrameTime.BackColor = Color.FromArgb(244, 206, 231);
							toolTip1.SetToolTip(lblInstDelayWarning, "Instrumental delay has *NOT* been applied to the times");
							toolTip1.SetToolTip(lblFrameTime, "Instrumental delay has *NOT* been applied to the times");
						}
					}
					else
						lblFrameTime.Text = "N/A";
				}

	            bool hasMagnitudes = m_LightCurveController.Context.MagnitudeConverter.CanComputeMagnitudes;
				double[] magnitudes = m_LightCurveController.Context.MagnitudeConverter.ComputeMagnitudes(m_SelectedMeasurements);

				if (m_Header.ObjectCount > 0 &&
					!LCMeasurement.IsEmpty(m_SelectedMeasurements[0]) /*The object may be deselected*/)
				{
					lblMeasurement1.Text = string.Format("{0}", m_SelectedMeasurements[0].AdjustedReading);
					lblMagnitude1.Text = hasMagnitudes ? string.Format("{0}", magnitudes[0].ToString("0.0")) : "";
					lblSN1.Text = string.Format("{0}", ComputeSignalToNoiceRatio(0, (int)(m_SelectedMeasurements[0].CurrFrameNo - m_Header.MinFrame), true).ToString("0.00"));
				}

				if (m_Header.ObjectCount > 1 &&
					!LCMeasurement.IsEmpty(m_SelectedMeasurements[1]) /*The object may be deselected*/)
				{
					lblMeasurement2.Text = string.Format("{0}", m_SelectedMeasurements[1].AdjustedReading);
					lblMagnitude2.Text = hasMagnitudes ? string.Format("{0}", magnitudes[1].ToString("0.0")) : "";
					lblSN2.Text = string.Format("{0}", ComputeSignalToNoiceRatio(1, (int)(m_SelectedMeasurements[0].CurrFrameNo - m_Header.MinFrame), true).ToString("0.00"));
				}

				if (m_Header.ObjectCount > 2 &&
					!LCMeasurement.IsEmpty(m_SelectedMeasurements[2]) /*The object may be deselected*/)
				{
					lblMeasurement3.Text = string.Format("{0}", m_SelectedMeasurements[2].AdjustedReading);
					lblMagnitude3.Text = hasMagnitudes ? string.Format("{0}", magnitudes[2].ToString("0.0")) : "";
					lblSN3.Text = string.Format("{0}", ComputeSignalToNoiceRatio(2, (int)(m_SelectedMeasurements[0].CurrFrameNo - m_Header.MinFrame), true).ToString("0.00"));
				}

				if (m_Header.ObjectCount > 3 &&
					!LCMeasurement.IsEmpty(m_SelectedMeasurements[3]) /*The object may be deselected*/)
				{
					lblMeasurement4.Text = string.Format("{0}", m_SelectedMeasurements[3].AdjustedReading);
					lblMagnitude4.Text = hasMagnitudes ? string.Format("{0}", magnitudes[3].ToString("0.0")) : "";
					lblSN4.Text = string.Format("{0}", ComputeSignalToNoiceRatio(3, (int)(m_SelectedMeasurements[0].CurrFrameNo - m_Header.MinFrame), true).ToString("0.00"));
				}

            	pnlMeasurementDetails.Visible = true;
                pnlBinInfo.Visible = m_LightCurveController.Context.Binning > 0;
	            pnlGeoLocation.Visible = m_GeoLocationInfo != null;
                if (m_LightCurveController.Context.Binning > 0)
                {
                    BinnedValue binnedVal = GetBinForFrameNo(0, m_SelectedMeasurements[0].CurrFrameNo);
                    if (binnedVal != null)
						lblBinNo.Text = string.Format("{0} ({1} - {2})", binnedVal.BinNo, m_Header.MinFrame + binnedVal.ReadingIndexFrom, m_Header.MinFrame + binnedVal.ReadingIndexTo);
                    else
                        lblBinNo.Text = string.Empty;
                }
            }
        }

        private void DrawColoredRectangle(ToolStripItem menuItem, int objectNo)
        {
            DrawColoredRectangle(menuItem, objectNo, false);
        }

        private void DrawColoredRectangle(ToolStripItem menuItem, int objectNo, bool forse)
        {
            if (menuItem.Image == null)
            {
                menuItem.Image = new Bitmap(16, 16);
                forse = true;
            }

            if (forse)
            {
                Bitmap bmp = (Bitmap)menuItem.Image;
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(m_DisplaySettings.TargetColors[objectNo]);
                    g.DrawRectangle(Pens.Black, 0, 0, 15, 15);
                    g.Save();
                }

                menuItem.Image = bmp;
            }
        }

        private void DrawColoredRectangleWithCheckBox(ToolStripMenuItem menuItem, int objectNo)
        {
            if (menuItem.Image == null)
                menuItem.Image = new Bitmap(16, 16);

            Bitmap bmp = (Bitmap)menuItem.Image;
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(m_DisplaySettings.TargetColors[objectNo]);
                g.DrawRectangle(Pens.Black, 0, 0, 15, 15);

                if (menuItem.Checked)
                {
                    g.DrawLine(Pens.Green, 2, 6, 6, 10);
                    g.DrawLine(Pens.Green, 6, 10, 12, 4);
                    g.DrawLine(Pens.Green, 3, 6, 6, 9);
                    g.DrawLine(Pens.Green, 6, 9, 11, 4);
                }

                g.Save();
            }

            menuItem.Image = bmp;
        }
    }
}
