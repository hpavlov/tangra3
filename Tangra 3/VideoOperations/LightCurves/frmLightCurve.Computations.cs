/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Tangra.Addins;
using Tangra.Config;
using Tangra.Controller;
using Tangra.Helpers;
using Tangra.Model.Config;
using Tangra.Model.Helpers;
using Tangra.Model.Image;
using Tangra.Model.Numerical;
using Tangra.Model.Video;
using Tangra.Model.VideoOperations;
using Tangra.SDK;
using Tangra.VideoOperations.LightCurves.InfoForms;
using Tangra.VideoOperations.LightCurves.Report;
using Tangra.VideoOperations.LightCurves.Tracking;


namespace Tangra.VideoOperations.LightCurves
{
    public partial class frmLightCurve
    {
    	private LCFile m_LCFile;
    	private string m_LCFilePath;
        private LCMeasurementHeader m_Header = LCMeasurementHeader.Empty;
        private LCMeasurementFooter m_Footer = LCMeasurementFooter.Empty;
    	private List<LCFrameTiming> m_FrameTiming = new List<LCFrameTiming>();

		private List<BinnedValue>[] m_AllBinnedReadings = new List<BinnedValue>[4]
		{
			new List<BinnedValue>(), new List<BinnedValue>(), new List<BinnedValue>(), new List<BinnedValue>()
		};

        private int m_MinX = 45;
        private int m_MinY = 25;
        private int m_MaxY, m_MaxX;

        private void ReComputeSignalSeries()
        {
            int minValue = int.MaxValue;
            int maxValue = int.MinValue;

			int totalMinValue = int.MaxValue;
			int totalMaxValue = int.MinValue;

			if (m_Header.ObjectCount == 0 || m_LightCurveController.Context.AllReadings[0].Count == 0)
			{
				totalMinValue = 0;
				totalMaxValue = 0;
			}

            for (int i = 0; i < m_Header.ObjectCount; i++)
            {
	            int[] last10Values = new int[10];

                for (int j = 0; j < m_LightCurveController.Context.AllReadings[i].Count; j++)
                {
                    LCMeasurement reading = m_LightCurveController.Context.AllReadings[i][j];
                    int adjustedValue = 0;

                    if (m_LightCurveController.Context.ProcessingType == ProcessingType.SignalOnly)
                    {
                        adjustedValue = (int)reading.TotalReading;
                    }
                    else if (m_LightCurveController.Context.ProcessingType == ProcessingType.SignalMinusBackground)
                    {
                        adjustedValue = (int)reading.TotalReading - (int)reading.TotalBackground;
                    }
                    else if (m_LightCurveController.Context.ProcessingType == ProcessingType.BackgroundOnly)
                    {
                        adjustedValue = (int)reading.TotalBackground;
                    }
					else if (m_LightCurveController.Context.ProcessingType == ProcessingType.SignalDividedByBackground)
					{
						int intBG = (int) reading.TotalBackground;
						if (intBG != 0)
							adjustedValue = 100 * (int)reading.TotalReading / (int)reading.TotalBackground;
						else
							adjustedValue = 100 * (int)reading.TotalReading;
					}
					else if (m_LightCurveController.Context.ProcessingType == ProcessingType.SignalDividedByNoise)
					{
						adjustedValue = (int)(100 * ComputeSignalToNoiceRatio(i, j, false));
					}

					if (totalMinValue > adjustedValue) totalMinValue = adjustedValue;
					if (totalMaxValue < adjustedValue) totalMaxValue = adjustedValue;

                    if (m_IncludeObjects[i] && reading.IsSuccessfulReading)
                    {
						bool includeInMinMaxCalcs = j > 10;

						if (m_LightCurveController.Context.OutlierRemoval && j >= 10)
						{
							// Deal with values that are too large or too small
							long average = (
								(long)last10Values[0] + (long)last10Values[1] + (long)last10Values[2] + (long)last10Values[3] + (long)last10Values[4] +
								(long)last10Values[5] + (long)last10Values[6] + (long)last10Values[7] + (long)last10Values[8] + (long)last10Values[9]) / 10;

							if (j == 10)
							{
								// the first 10 items we don't check, so we take the median and then make all 10 items the same
								List<int> firstTen = last10Values.ToList();
								int median = (firstTen[4] + firstTen[5])/2;
								for (int k = 0; k < 10; k++) last10Values[k] = median;
								average = median;
							}

							if (adjustedValue > 1000 || adjustedValue < -1000)
							{
								if (Math.Abs(adjustedValue) > Math.Abs(2 * average))
								{
									adjustedValue = last10Values[(j - 1) % 10];
									reading.SetIsMeasured(NotMeasuredReasons.OutlierMeasurement);
									includeInMinMaxCalcs = false;
								}
							}
						}

						if (includeInMinMaxCalcs)
						{
							if (minValue > adjustedValue) minValue = adjustedValue;
							if (maxValue < adjustedValue) maxValue = adjustedValue;														
						}																				

						if (j < 10 || includeInMinMaxCalcs)
							last10Values[j % 10] = adjustedValue;
                    }

                    reading.AdjustedReading = adjustedValue;
                    m_LightCurveController.Context.AllReadings[i][j] = reading;
                }
            }

	        if (minValue == int.MaxValue) minValue = totalMinValue;
			if (maxValue == int.MinValue) maxValue = totalMaxValue;

            m_Header.MinAdjustedReading = minValue;
            m_Header.MaxAdjustedReading = maxValue;

            if (m_LightCurveController.Context.Binning > 0)
            {
                ComputeBinning(); 
            }

            if (m_LightCurveController.Context.Normalisation > -1)
            {
                Cursor = Cursors.WaitCursor;
                try
                {
                    PerformNormalization();
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
        }

		private double ComputeSignalToNoiceRatio(int objectId, int middleFrameId, bool useAdjustedReading)
		{
			int halfFrameWindow = TangraConfig.Settings.Photometry.SNFrameWindow / 2;

			int from = Math.Max(0, middleFrameId - halfFrameWindow);
			int to = Math.Min(m_LightCurveController.Context.AllReadings[objectId].Count, from + halfFrameWindow * 2);
			double sum = 0;
			for (int k = from; k < to; k++)
			{
				sum += useAdjustedReading ? (int)m_LightCurveController.Context.AllReadings[objectId][k].AdjustedReading : (int)m_LightCurveController.Context.AllReadings[objectId][k].TotalReading;
			}
			double avrge = sum / (to - from);
			sum = 0;
			for (int k = from; k < to; k++)
			{
				double residual = (useAdjustedReading ? (int)m_LightCurveController.Context.AllReadings[objectId][k].AdjustedReading : (int)m_LightCurveController.Context.AllReadings[objectId][k].TotalReading) - avrge;
				sum += residual * residual;
			}
			double variance = Math.Sqrt(sum / (to - from - 1));

			if (variance != 0)
				return avrge / (int)variance;
			else
				return avrge;
		}

        private void ComputeBinning()
        {
            int minValue = int.MaxValue;
            int maxValue = int.MinValue;

            for (int i = 0; i < m_Header.ObjectCount; i++)
            {
                m_AllBinnedReadings[i].Clear();

                int idx = 1;
                int binnedSum = 0;
	            int binnedBackgroundSum = 0;
                int binnedSuccessfulReadings = 0;
                int binNo = 1;
                int delta_bin_ref_frame = m_LightCurveController.Context.Binning - (m_LightCurveController.Context.BinningFirstFrame - (int)m_LightCurveController.Context.MinFrame);

                BinnedValue binnedValue = new BinnedValue();
                binnedValue.ReadingIndexFrom = 0;
                binnedValue.BinNo = binNo;

                foreach (LCMeasurement reading in m_LightCurveController.Context.AllReadings[i])
                {
                    binnedSum += reading.AdjustedReading;
                    binnedBackgroundSum += (int)reading.TotalBackground;

                    if (reading.IsSuccessfulReading)
                        binnedSuccessfulReadings++;

                    if ((idx + delta_bin_ref_frame) % m_LightCurveController.Context.Binning == 0)
                    {
                        binnedValue.TotalSum = binnedSum;
	                    binnedValue.TotalBackgroundSum = binnedBackgroundSum;
                    	binnedValue.AdjustedValue = binnedSum * 1.0 / m_LightCurveController.Context.Binning;
						binnedValue.BackgroundValue = binnedBackgroundSum * 1.0 / m_LightCurveController.Context.Binning;
						binnedValue.IsSuccessfulReading = binnedSuccessfulReadings == m_LightCurveController.Context.Binning; // Average out the non binned values due to unsuccessful readings

                        binnedValue.ReadingIndexTo = idx - 1;
	                    binnedValue.BinMiddleFrameNo = (int)(m_LightCurveController.Context.AllReadings[i][binnedValue.ReadingIndexFrom].CurrFrameNo + m_LightCurveController.Context.AllReadings[i][binnedValue.ReadingIndexTo].CurrFrameNo)/2;
                        m_AllBinnedReadings[i].Add(binnedValue);

                        if (m_IncludeObjects[i])
                        {
                            if (minValue > binnedValue.AdjustedValue) minValue = (int) binnedValue.AdjustedValue - 1;
                            if (maxValue < binnedValue.AdjustedValue) maxValue = (int) binnedValue.AdjustedValue + 1;
                        }

                        binNo++;
                        binnedSuccessfulReadings = 0;

						binnedSum = reading.AdjustedReading;
	                    binnedBackgroundSum = (int)reading.TotalBackground;
                        
                        binnedValue = new BinnedValue();
                        binnedValue.ReadingIndexFrom = idx;
                        binnedValue.BinNo = binNo;
                    }

                    idx++;
                }
            }

            m_Header.MinAdjustedReading = minValue;
            m_Header.MaxAdjustedReading = maxValue;
        }

        private void PerformNormalization()
        {
            List<double> normalIndexes = new List<double>();

            switch(m_LightCurveController.Context.NormMethod)
            {
                case LightCurveContext.NormalisationMethod.LinearFit:
                    if (m_LightCurveController.Context.Binning > 0)
                        PerformLinearFitBinnedNormalisation(ref normalIndexes);
                    else
                        PerformLinearFitReadingsNormalisation(ref normalIndexes);
                    break;

                case LightCurveContext.NormalisationMethod.FrameByFrame:
                    if (m_LightCurveController.Context.Binning > 0)
                        PerformAverageBinnedNormalisation(ref normalIndexes, 1);
                    else
                        PerformAverageReadingsNormalisation(ref normalIndexes, 1);
                    break;

                case LightCurveContext.NormalisationMethod.Average4Frame:
                    if (m_LightCurveController.Context.Binning > 0)
                        PerformAverageBinnedNormalisation(ref normalIndexes, 4);
                    else
                        PerformAverageReadingsNormalisation(ref normalIndexes, 4);
                    break;

                case LightCurveContext.NormalisationMethod.Average16Frame:
                    if (m_LightCurveController.Context.Binning > 0)
                        PerformAverageBinnedNormalisation(ref normalIndexes, 16);
                    else
                        PerformAverageReadingsNormalisation(ref normalIndexes, 16);
                    break;
            }

            int minValue = int.MaxValue;
            int maxValue = int.MinValue;

           
            if (m_LightCurveController.Context.Binning > 0)
            {
                while (normalIndexes.Count < m_AllBinnedReadings[0].Count) normalIndexes.Add(1);

                for (int i = 0; i < m_Header.ObjectCount; i++)
                {
					if (!m_IncludeObjects[i]) continue;

                    int upper = m_AllBinnedReadings[i].Count;
                    for (int j = 0; j < upper; j++)
                    {
						double normValue = m_AllBinnedReadings[i][j].AdjustedValue * normalIndexes[j];
                        m_AllBinnedReadings[i][j].AdjustedValue = normValue;
						if (m_AllBinnedReadings[i][j].IsSuccessfulReading || m_DisplaySettings.DrawInvalidDataPoints)
						{
							if (minValue > normValue) minValue = (int)normValue;
							if (maxValue < normValue) maxValue = (int)normValue;
						}
                    }
                }
            }
            else
            {
                while (normalIndexes.Count < m_LightCurveController.Context.AllReadings[0].Count) normalIndexes.Add(1);

                for (int i = 0; i < m_Header.ObjectCount; i++)
                {
					if (!m_IncludeObjects[i]) continue;

                    int upper = m_LightCurveController.Context.AllReadings[i].Count;
                    for (int j = 0; j < upper; j++)
                    {
						double normValue = Math.Round(m_LightCurveController.Context.AllReadings[i][j].AdjustedReading * normalIndexes[j]);
						LCMeasurement measurement = m_LightCurveController.Context.AllReadings[i][j];
                        measurement.AdjustedReading = (int)normValue;
						if (measurement.IsSuccessfulReading || m_DisplaySettings.DrawInvalidDataPoints)
						{
							if (minValue > normValue) minValue = (int)normValue;
							if (maxValue < normValue) maxValue = (int)normValue;
						}
                        m_LightCurveController.Context.AllReadings[i][j] = measurement;
					}
                }
            }

        	m_Header.MaxAdjustedReading = maxValue;
            m_Header.MinAdjustedReading = minValue;
        }

        private void PerformLinearFitBinnedNormalisation(ref List<double> normalIndexes)
        {
            var linearRegression = new LinearRegression();

            foreach (BinnedValue binnedValue in m_AllBinnedReadings[m_LightCurveController.Context.Normalisation])
            {
                if (binnedValue.IsSuccessfulReading)
                    linearRegression.AddDataPoint(binnedValue.BinNo, binnedValue.AdjustedValue);
            }

            linearRegression.Solve();

            double firstValue = linearRegression.ComputeY(m_AllBinnedReadings[m_LightCurveController.Context.Normalisation][0].BinNo);

            foreach (BinnedValue binnedValue in m_AllBinnedReadings[m_LightCurveController.Context.Normalisation])
            {
                normalIndexes.Add(firstValue / linearRegression.ComputeY(binnedValue.BinNo));
            }
        }

        private void PerformLinearFitReadingsNormalisation(ref List<double> normalIndexes)
        {
            var linearRegression = new LinearRegression();

            foreach (LCMeasurement reading in m_LightCurveController.Context.AllReadings[m_LightCurveController.Context.Normalisation])
            {
                if (reading.IsSuccessfulReading)
                    linearRegression.AddDataPoint(reading.CurrFrameNo, reading.AdjustedReading);
            }

            linearRegression.Solve();

            double firstValue = linearRegression.ComputeY(m_LightCurveController.Context.AllReadings[m_LightCurveController.Context.Normalisation][0].CurrFrameNo);

            foreach (LCMeasurement reading in m_LightCurveController.Context.AllReadings[m_LightCurveController.Context.Normalisation])
            {
                normalIndexes.Add(firstValue / linearRegression.ComputeY(reading.CurrFrameNo));
            }
        }

        private void PerformAverageReadingsNormalisation(ref List<double> normalIndexes, int numFrames)
        {
            var averages = new List<double>();
            var maxIdx = (int)m_Header.MeasuredFrames;

            for (int i = 0; i < maxIdx; i++)
            {
                if (numFrames == 1)
                {
                    if (m_LightCurveController.Context.AllReadings[m_LightCurveController.Context.Normalisation][i].IsSuccessfulReading)
                        averages.Add(m_LightCurveController.Context.AllReadings[m_LightCurveController.Context.Normalisation][i].AdjustedReading);
                    else
                        averages.Add(double.NaN);
                }
                else
                {
                    long sum = 0;
                    int readings = 0;
                    for (int j = -numFrames / 2; j <= numFrames / 2; j++)
                    {
                        int idx = i + j;
                        if (idx >= 0 && idx < maxIdx)
                        {
                            if (m_LightCurveController.Context.AllReadings[m_LightCurveController.Context.Normalisation][idx].IsSuccessfulReading)
                            {
                                sum += m_LightCurveController.Context.AllReadings[m_LightCurveController.Context.Normalisation][idx].AdjustedReading;
                                readings++;
                            }
                        }
                    }

                    if (readings == 0)
                        averages.Add(double.NaN);
                    else
                        averages.Add((1.0 * sum / readings));
                }
            }

	        int nonNanValues = averages.Count(x => !double.IsNaN(x));

			if (nonNanValues < 3)
			{
				for (int i = 0; i < maxIdx; i++) normalIndexes.Add(1);
			}
			else
			{
				List<double> first3 = averages.Where(x => !double.IsNaN(x)).Take(3).ToList();
				first3.Sort();
				double normValue = first3[1];

				for (int i = 0; i < maxIdx; i++)
				{
					if (double.IsNaN(averages[i]))
						normalIndexes.Add(1);
					else
						normalIndexes.Add(normValue / averages[i]);
				}
			}
        }


        private void PerformAverageBinnedNormalisation(ref List<double> normalIndexes, int numFrames)
        {
            List<double> averages = new List<double>();

            int maxIdx = m_AllBinnedReadings[m_LightCurveController.Context.Normalisation].Count;
            for (int i = 0; i < maxIdx; i++)
            {
                double sum = 0;
                int readings = 0;
                for (int j = -numFrames / 2; j <= numFrames / 2; j++)
                {
                    int idx = i + j;
                    if (idx >= 0 && idx < maxIdx)
                    {
                        if (m_LightCurveController.Context.AllReadings[m_LightCurveController.Context.Normalisation][idx].IsSuccessfulReading)
                        {
                            sum += m_AllBinnedReadings[m_LightCurveController.Context.Normalisation][idx].AdjustedValue;
                            readings++;
                        }
                    }
                }

                if (readings == 0)
                    averages.Add(double.NaN);
                else
                    averages.Add((1.0 * sum / readings));
            }
            double firstValue = averages.FirstOrDefault(x => !double.IsNaN(x));
            if (double.IsNaN(firstValue) || firstValue == 0)
            {
                for (int i = 0; i < maxIdx; i++) normalIndexes.Add(1);
            }

            for (int i = 0; i < maxIdx; i++)
            {
                if (double.IsNaN(averages[i]))
                    normalIndexes.Add(1);
                else
                    normalIndexes.Add(firstValue / averages[i]);
            }
        }

        private BinnedValue GetBinForFrameNo(int objNo, uint frameNo)
        {
            frameNo -= (uint)m_Header.MinFrame;
            List<BinnedValue> objBinnedVals = m_AllBinnedReadings[objNo];
            foreach(BinnedValue val in objBinnedVals)
            {
                if (val.ReadingIndexFrom <= frameNo &&
                    val.ReadingIndexTo >= frameNo)
                {
                    return val;
                }
            }

            return null;
        }

		private uint GetYAxisInterval(Graphics g)
        {
			uint idealInterval = 0;
			float fullScale = m_Header.MaxAdjustedReading - m_Header.MinAdjustedReading;
            int oneSeventhFullScale = (int)fullScale / 7;

			uint[] preferredIntervals = new uint[] { 1000000, 500000, 250000, 100000, 50000, 25000, 10000, 5000, 2500, 1000, 500, 250, 100, 50, 25, 10, 5, 1, 1 };
            for (int i = 0; i < preferredIntervals.Length - 1; i++)
            {
                if (preferredIntervals[i] == oneSeventhFullScale)
                    return preferredIntervals[i];

                if (preferredIntervals[i] > oneSeventhFullScale &&
                    preferredIntervals[i + 1] < oneSeventhFullScale)
                {
                    idealInterval = preferredIntervals[i + 1];
                	float betweenMarksInterval = 1.0f * (m_MaxY - m_MinY) / idealInterval;
					string label = m_Header.MaxAdjustedReading.ToString();
					SizeF labelSize = g.MeasureString(label, s_AxisFont);
					if (labelSize.Height > betweenMarksInterval - 6)
						return preferredIntervals[i];
					else
						return idealInterval;
                }
            }

            return (uint)fullScale / 10;

	
        }

        private uint GetXAxisTimeInterval(Graphics g)
        {
            DateTime? lastTimestamp = m_Header.GetFrameTime(m_MaxDisplayedFrame);
            DateTime? firstTimestamp = m_Header.GetFrameTime(m_MinDisplayedFrame);
            if (!lastTimestamp.HasValue || !firstTimestamp.HasValue)
                return 120;

            float fullScale = (float)new TimeSpan(lastTimestamp.Value.Ticks - firstTimestamp.Value.Ticks).TotalSeconds;

            string label = "23:59";
            string labelSecs = "23:59:59";

            SizeF labelSize = g.MeasureString(label, s_AxisFont);

            int numLabels = (int)Math.Floor((m_MaxX - m_MinX) / (1.5f * labelSize.Width));

            labelSize = g.MeasureString(labelSecs, s_AxisFont);
            int numLabelsSecs = (int)Math.Floor((m_MaxX - m_MinX) / (1.2f * labelSize.Width));

            uint[] goodIntervalsInMinutes = new uint[] { 1, 2, 5, 10, 15, 30, 1 * 60, 2 * 60, 5 * 60, 10 * 60, 15 * 60, 30 * 60, 60 * 60, 120 * 60 };

            float markIntervalSize = fullScale / numLabels;

            uint interval = goodIntervalsInMinutes.FirstOrDefault(x => x > markIntervalSize);
            if (interval < 60)
            {
                markIntervalSize = fullScale / numLabelsSecs;
                interval = goodIntervalsInMinutes.FirstOrDefault(x => x > markIntervalSize);
            }

            if (interval == 0) interval = 120;

            return interval;
        }

		private uint GetXAxisInterval(Graphics g)
        {
			float fullScale = (m_MaxDisplayedFrame - m_MinDisplayedFrame);

			double closestValue = double.MaxValue;
			int closestIndex = -1;

			uint[] goodIntervals = new uint[] { 1, 5, 10, 25, 50, 100, 250, 500, 1000, 2000, 2500, 5000, 10000, 20000, 25000, 50000, 100000, 200000, 250000, 500000, 1000000, 1000000 };
            for (int i = 0; i < goodIntervals.Length - 1; i++)
            {
                double numMarks = fullScale / goodIntervals[i];
                if (numMarks > 10 &&
                    numMarks < 25)
                {
                	do
                	{
                		uint idealInterval = goodIntervals[i];
                		float betweenMarksInterval = 1.0f*(m_MaxX - m_MinX)/((uint) fullScale/idealInterval);
                		string label = fullScale.ToString();
                		SizeF labelSize = g.MeasureString(label, s_AxisFont);

						if (labelSize.Width > betweenMarksInterval - 20)
						{
							i++;
						}
						else
						{	
							return idealInterval;
						}
					} while (i < goodIntervals.Length - 1);
                }
				else
                {
                	if (closestValue > Math.Abs(numMarks - 25))
                	{
                		closestValue = Math.Abs(numMarks - 25);
                		closestIndex = i;
                	}
                }
            }

			do
			{
				uint idealInterval = goodIntervals[closestIndex];
				float betweenMarksInterval = 1.0f * (m_MaxX - m_MinX) / ((uint)fullScale / idealInterval);
				string label = fullScale.ToString();
				SizeF labelSize = g.MeasureString(label, s_AxisFont);

				if (labelSize.Width > betweenMarksInterval - 20)
				{
					closestIndex++;
				}
				else
				{
					return idealInterval;
				}
			} while (closestIndex < goodIntervals.Length - 1);

            return (uint)(fullScale / 15);
        }

		public bool ExportToCSV(string fileName, CSVExportOptions options)
		{
			var output = new StringBuilder();

		    AtmosphericExtinctionCalculator atmExtCalc = null;
            if (options.ExportAtmosphericExtinction)
                atmExtCalc = new AtmosphericExtinctionCalculator(options.RAHours, options.DEDeg, options.LongitudeDeg, options.LatitudeDeg, options.HeightKM);

            string videoSystem;
            double absoluteTimeError = m_TimestampDiscrepencyFlag
                                           ? m_Header.GetAbsoluteTimeDeltaInMilliseconds(out videoSystem)
                                           : 0;

			uint objCount = m_Header.ObjectCount;
			if (m_LightCurveController.Context.Binning > 0)
			{
				// Binned frames
				int count = m_AllBinnedReadings[0].Count;
				int firstFrameNoInBin = (int)m_Header.MinFrame;

                CSVExportAddCommonHeader(output, options, true);

                output.Append(string.Format("BinNo,Time ({0})", options.FormatTimeLabel()));

				for (int j = 0; j < objCount; j++)
                    output.Append(options.FormatPhotometricValueHeaderForObject(j + 1, false, true));


                bool isBadTime = false;
			    double timePrecisionSec = 0;

                double resolutionInSecs = m_LightCurveController.Context.Binning / (2.0 * m_Header.FramesPerSecond);

				if (double.IsInfinity(resolutionInSecs) || double.IsNaN(resolutionInSecs)
					&& m_Header.LcFile != null && m_Header.LcFile.FrameTiming != null && m_Header.LcFile.FrameTiming.Count > 0)
				{

					List<int> allExposures = m_Header.LcFile.FrameTiming.Select(x => x.FrameDurationInMilliseconds).ToList();
					allExposures.Sort();
					resolutionInSecs = m_LightCurveController.Context.Binning * allExposures[allExposures.Count / 2] / 1000.0;
				}

                if (resolutionInSecs < 0.1)
                {
                    timePrecisionSec = 0.001;
                    if (absoluteTimeError > 5) isBadTime = true;
                }
                else if (resolutionInSecs < 1)
                {
                    timePrecisionSec = 0.01;
                    if (absoluteTimeError > 5) isBadTime = true;
                }
                else if (resolutionInSecs < 10)
                {
                    timePrecisionSec = 0.1;
                    if (absoluteTimeError > 50) isBadTime = true;
                }
                else if (absoluteTimeError > 500)
                {
                    timePrecisionSec = 0.01;
                    isBadTime = true;
                }
                else if (!m_LCFile.Footer.ReductionContext.HasEmbeddedTimeStamps
                    /* If the times are entered by the user, only include the times for the frames enterred by the user. Unless the calculated timing descrepency is small (m_TimestampDiscrepencyFlag = false) */)
                {
                    timePrecisionSec = 0.01;
                    isBadTime = m_TimestampDiscrepencyFlag;
                }

                if (isBadTime && 
                    m_LCFile.Footer.ReductionContext.LightCurveReductionType == LightCurveReductionType.MutualEvent && absoluteTimeError/500 < resolutionInSecs)
				{
					// Force export the time for mutual events where half the resolution is better than the absolute timing error 
                    isBadTime = false;
				}

			    if (options.PhotometricFormat == PhotometricFormat.Magnitudes && atmExtCalc != null)
			    {
			        output.Append(", Atmospheric Extinction, Altitude (deg), Air Mass");
			    }

			    output.AppendLine();

				for (int i = 0; i < count; i++)
				{
					string isCorrectedForInstrumentalDelay;
                    double midBinFrameNumber = firstFrameNoInBin + (m_LightCurveController.Context.Binning / 2.0) - 0.5;

				    DateTime middleBinTime = DateTime.MinValue;

                    if (midBinFrameNumber <= m_Header.MaxFrame && midBinFrameNumber >= m_Header.MinFrame)
                    {
                        int midBinFrameNumberInt = (int)midBinFrameNumber;
                        double midBinFrameNumberFrac = midBinFrameNumber - midBinFrameNumberInt;

                        middleBinTime = m_LCFile.GetTimeForFrame(midBinFrameNumberInt, out isCorrectedForInstrumentalDelay);
                        if (middleBinTime > DateTime.MinValue && midBinFrameNumberFrac > 0.00001 && midBinFrameNumberInt + 1 <= m_Header.MaxFrame)
                        {
                            DateTime middleBinPlusOneTime = m_LCFile.GetTimeForFrame(midBinFrameNumberInt + 1, out isCorrectedForInstrumentalDelay);
                            middleBinTime = middleBinTime.AddTicks((long)((middleBinPlusOneTime.Ticks - middleBinTime.Ticks) * midBinFrameNumberFrac));
                        }
                    }

				    string timeStr;
                    if (isBadTime)
					{
                        if (m_Header.FirstTimedFrameNo >= firstFrameNoInBin && m_Header.FirstTimedFrameNo <= firstFrameNoInBin + m_LightCurveController.Context.Binning)
							timeStr = options.FormatTime(m_Header.FirstTimedFrameTime, timePrecisionSec);
                        else if (m_Header.LastTimedFrameNo >= firstFrameNoInBin && m_Header.LastTimedFrameNo <= firstFrameNoInBin + m_LightCurveController.Context.Binning)
                            timeStr = options.FormatTime(m_Header.SecondTimedFrameTime, timePrecisionSec);
						else
                            timeStr = options.FormatInvalidTime(timePrecisionSec);
					}
					else
					{
                        if (middleBinTime == DateTime.MinValue || middleBinTime == DateTime.MaxValue)
							timeStr = "";
						else
                            timeStr = options.FormatTime(middleBinTime, timePrecisionSec);
					}

					output.AppendFormat("{0},{1}", i + 1, timeStr);

					for (int j = 0; j < objCount; j++)
					{
						BinnedValue reading = m_AllBinnedReadings[j][i];
                        output.Append(options.FormatPhotometricValue(reading.IsSuccessfulReading, reading.AdjustedValue, 0, false, true));
					}

                    if (options.PhotometricFormat == PhotometricFormat.Magnitudes && atmExtCalc != null &&
                        middleBinTime != DateTime.MinValue && middleBinTime != DateTime.MaxValue)
                    {
                        double altitudeDeg;
                        double airMass;
                        double extinction = atmExtCalc.CalculateExtinction(middleBinTime, out altitudeDeg, out airMass);
                        output.AppendFormat(",{0},{1},{2}", extinction.ToString(5), altitudeDeg.ToString(3), airMass.ToString(5));
                    }

					output.AppendLine();

					firstFrameNoInBin += m_LightCurveController.Context.Binning;
				}
			}
			else
			{
                CSVExportAddCommonHeader(output, options, false);

				bool onlyExportSignalMunusBg =
					m_LightCurveController.Context.SignalMethod == TangraConfig.PhotometryReductionMethod.PsfPhotometry &&
					m_LightCurveController.Context.PsfQuadratureMethod == TangraConfig.PsfQuadrature.Analytical;

				int count = m_LightCurveController.Context.AllReadings[0].Count;

                output.Append(string.Format("FrameNo,Time ({0})", options.FormatTimeLabel()));

				for (int j = 0; j < objCount; j++)
				{
                    output.Append(options.FormatPhotometricValueHeaderForObject(j + 1, onlyExportSignalMunusBg, false));
				}

			    bool isBadTime = false;
                double timePrecisionSec = 0;

                if (absoluteTimeError > 5)
                {
                    timePrecisionSec = 0.001;
                    isBadTime = true;
                }                    
                else if (!m_LCFile.Footer.ReductionContext.HasEmbeddedTimeStamps
                    /* If the times are entered by the user, only include the times for the frames enterred by the user. Unless the calculated timing descrepency is small (m_TimestampDiscrepencyFlag = false) */)
                {
                    timePrecisionSec = 0.01;
                    isBadTime = m_TimestampDiscrepencyFlag;                    
                }
                else
                    timePrecisionSec = 0.001;

                if (options.PhotometricFormat == PhotometricFormat.Magnitudes && atmExtCalc != null)
                {
                    output.Append(", Atmospheric Extinction, Altitude (deg), Air Mass");
                }
                output.AppendLine();

				for (int i = 0; i < count; i++)
				{
					uint frameNo = m_LightCurveController.Context.AllReadings[0][i].CurrFrameNo;
					string isCorrectedForInstrumentalDelay;
					DateTime currFrameTime = m_LCFile.GetTimeForFrame(frameNo, out isCorrectedForInstrumentalDelay);

				    if (!string.IsNullOrEmpty(m_LightCurveController.Context.InstrumentalDelayConfigName) && isCorrectedForInstrumentalDelay == null)
                        // Single frames not corrected for instrumental delays, where insrumental delays are known, are considered bad times
				        currFrameTime = DateTime.MaxValue;

                    string timeStr;
                    if (isBadTime)
					{
                        if (currFrameTime == DateTime.MinValue || currFrameTime == DateTime.MaxValue)
                            timeStr = "";
                        else if (frameNo == m_Header.FirstTimedFrameNo)
                            timeStr = options.FormatTime(m_Header.FirstTimedFrameTime, timePrecisionSec);
                        else if (frameNo == m_Header.LastTimedFrameNo)
                            timeStr = options.FormatTime(m_Header.SecondTimedFrameTime, timePrecisionSec);
                        else
                            timeStr = options.FormatInvalidTime(timePrecisionSec);
					}
					else
					{
                        if (currFrameTime == DateTime.MinValue || currFrameTime == DateTime.MaxValue)
							timeStr = "";
						else
                            timeStr = options.FormatTime(currFrameTime, timePrecisionSec);
					}

					output.AppendFormat("{0},{1}", frameNo, timeStr);

					for (int j = 0; j < objCount; j++)
					{
						LCMeasurement reading = m_LightCurveController.Context.AllReadings[j][i];
						output.Append(options.FormatPhotometricValue(reading.IsSuccessfulReading, (int)reading.TotalReading, (int)reading.TotalBackground, onlyExportSignalMunusBg, false));
					}

                    if (options.PhotometricFormat == PhotometricFormat.Magnitudes && atmExtCalc != null &&
                        currFrameTime != DateTime.MinValue && currFrameTime != DateTime.MaxValue)
                    {
                        double altitudeDeg;
                        double airMass;
                        double extinction = atmExtCalc.CalculateExtinction(currFrameTime, out altitudeDeg, out airMass);
                        output.AppendFormat(",{0},{1},{2}", extinction.ToString(5), altitudeDeg.ToString(3), airMass.ToString(5));
                    }

					output.AppendLine();
				}
			}

			try
			{
				File.WriteAllText(fileName, output.ToString());
				return true;
			}
			catch(IOException ex)
			{
				MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
		}

        private void CSVExportAddCommonHeader(StringBuilder output, CSVExportOptions options, bool binning)
        {
            Version ver = Assembly.GetExecutingAssembly().GetName().Version;

            output.AppendFormat("Tangra v{0}", ver.ToString()); output.AppendLine();
            if (binning)
                output.AppendFormat("Measurments of {0} objects; Bins of {1} frames; Binned Measurement = {2}", m_Header.ObjectCount, m_LightCurveController.Context.Binning, m_LightCurveController.Context.ProcessingType);
            else
                output.AppendFormat("Measurments of {0} objects", m_Header.ObjectCount);

            output.AppendLine();
            output.Append(m_Header.PathToVideoFile); output.AppendLine();
            output.AppendFormat("{0} {1}", m_Header.ReductionType, m_Header.SourceInfo); output.AppendLine();
            output.AppendLine();output.AppendLine();
            bool addPSFReductionDetails = m_LightCurveController.Context.SignalMethod != TangraConfig.PhotometryReductionMethod.AperturePhotometry;
            bool addPSFAverageModelDetails = addPSFReductionDetails && m_LightCurveController.Context.PsfFittingMethod == TangraConfig.PsfFittingMethod.LinearFitOfAveragedModel;
            bool addIntegrationInfo = m_Footer.ReductionContext.FrameIntegratingMode != FrameIntegratingMode.NoIntegration;

            string instrumentalDelayStatus = "Not Applied";
            if (m_LightCurveController.Context.TimingType == MeasurementTimingType.EmbeddedTimeForEachFrame && m_LightCurveController.Context.InstrumentalDelayCorrectionsNotRequired)
                instrumentalDelayStatus = "Not Required";
            else if (!string.IsNullOrEmpty(m_LightCurveController.Context.InstrumentalDelayConfigName) && m_LightCurveController.Context.TimingType != MeasurementTimingType.UserEnteredFrameReferences)
                instrumentalDelayStatus = "Applied";

            output.Append("Reversed Gamma, Colour, Measured Band, Integration, Digital Filter, Signal Method, Background Method, Instrumental Delay Corrections, Camera, AAV Integration, First Frame, Last Frame");
            if (addPSFReductionDetails) output.Append(", PSF Fitting");
            if (addPSFAverageModelDetails) output.Append(", Modeled FWHM, Average FWHM");
            if (options.PhotometricFormat == PhotometricFormat.Magnitudes) output.Append(", Zero Magnitude");
            output.AppendLine();
            output.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}", 
                m_LightCurveController.Context.EncodingGamma.ToString("0.00"),
                m_Footer.ReductionContext.IsColourVideo ? "yes" : "no", 
                m_Footer.ReductionContext.ColourChannel,
                addIntegrationInfo
                    ? string.Format("{0} {1} of {2} frames", m_Footer.ReductionContext.PixelIntegrationType, m_Footer.ReductionContext.FrameIntegratingMode, m_Footer.ReductionContext.NumberFramesToIntegrate) 
                    : "no",
                m_LightCurveController.Context.Filter, 
                m_LightCurveController.Context.SignalMethod,
                m_LightCurveController.Context.BackgroundMethod,
                instrumentalDelayStatus,
                !string.IsNullOrEmpty(m_LightCurveController.Context.CameraName) ?  m_LightCurveController.Context.CameraName : m_LightCurveController.Context.InstrumentalDelayConfigName,
                m_LightCurveController.Context.AAVFrameIntegration == -1 ? "" : m_LightCurveController.Context.AAVFrameIntegration.ToString(),
                m_LightCurveController.Context.MinFrame,
                m_LightCurveController.Context.MaxFrame);

            if (addPSFReductionDetails) output.AppendFormat(",{0}", m_LightCurveController.Context.PsfFittingMethod);
            if (addPSFAverageModelDetails) output.AppendFormat(",{0},{1}", float.IsNaN(m_LightCurveController.Context.ManualAverageFWHM) ? "auto" : "manual", !float.IsNaN(m_LightCurveController.Context.ManualAverageFWHM) ? m_LightCurveController.Context.ManualAverageFWHM.ToString("0.00") : m_Footer.RefinedAverageFWHM.ToString("0.00"));
            if (options.PhotometricFormat == PhotometricFormat.Magnitudes) output.AppendFormat(",{0}", options.M0.ToString("0.000"));

            output.AppendLine();output.AppendLine();
            output.Append("Object, Type, Aperture, Tolerance, FWHM, Measured, StartingX, StartingY, Fixed"); output.AppendLine();
            for (int j = 0; j < m_Header.ObjectCount; j++)
            {
                TrackedObjectConfig obj = m_Footer.TrackedObjects[j];
                output.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7},{8}",
                    j + 1, obj.TrackingType.ToString(), m_LightCurveController.Context.ReProcessApertures[j].ToString("0.00"),
                    obj.TrackingType == TrackingType.OccultedStar ? obj.PositionTolerance.ToString("0.00") : "", obj.RefinedFWHM.ToString("0.00"),
                    obj.MeasureThisObject ? "yes" : "no",
                    obj.ApertureStartingX.ToString("0.0"), obj.ApertureStartingY.ToString("0.0"), obj.IsFixedAperture ? "yes" : "no");
                output.AppendLine();
            }
            output.AppendLine(); output.AppendLine(); 
        }

        internal class BinnedValue
        {
            public int TotalSum;
	        public int TotalBackgroundSum;
            public double AdjustedValue;
			public double BackgroundValue;
            public int ReadingIndexFrom;
            public int ReadingIndexTo;
            public int BinNo;
			public int BinMiddleFrameNo;
        	public bool IsSuccessfulReading;
        }

		string ILightCurveDataProvider.FileName
		{
			get { return File.Exists(m_LCFilePath) ? m_LCFilePath : m_LCFile.Header.PathToVideoFile; }
		}

		int ILightCurveDataProvider.NumberOfMeasuredComparisonObjects
		{
			get { return m_LCFile.Header.ObjectCount - 1; }
		}

        bool ILightCurveDataProvider.HasReliableTimeBase
		{
            get { return !m_TimestampDiscrepencyFlag; }
		}

        bool ILightCurveDataProvider.CameraCorrectionsHaveBeenApplied
        {
            get { return m_CameraCorrectionsHaveBeenAppliedFlag; }
        }

        bool ILightCurveDataProvider.HasEmbeddedTimeStamps
        {
            get { return m_HasEmbeddedTimeStamps; }
        }

	    string ILightCurveDataProvider.VideoCameraName
	    {
			get { return m_LightCurveController.Context.CameraName; }
	    }

        string ILightCurveDataProvider.VideoSystem
        {
            get
            {
                m_LCFile.Header.LcFile = m_LCFile;

                // AVI|AAV|ADV
				VideoFileFormat videoSystem = m_LCFile.Header.GetVideoFileFormat();
                // PAL|NTSC|Digital
                string videoFormat = m_LCFile.Header.GetVideoFormat(videoSystem);

                if (string.IsNullOrEmpty(videoFormat) &&
                    (m_LCFile.Header.TimingType == MeasurementTimingType.OCRedTimeForEachFrame || m_LCFile.Header.TimingType == MeasurementTimingType.EmbeddedTimeForEachFrame) &&
                    m_LCFile.FrameTiming != null && m_LCFile.FrameTiming.Count > 1)
                {
                    if (Math.Abs(1000.0 / 25 - m_LCFile.FrameTiming[1].FrameDurationInMilliseconds) < 1)
                        videoFormat = "PAL";
                    else if (Math.Abs(1000.0 / 29.97 - m_LCFile.FrameTiming[1].FrameDurationInMilliseconds) < 1)
                        videoFormat = "NTSC";
                }

                if (videoSystem == VideoFileFormat.ADV)
                    return "ADVS";
				else if (videoSystem == VideoFileFormat.SER)
					return "SER";
                else if (videoSystem == VideoFileFormat.AAV)
                {
                    if (videoFormat == "PAL" || videoFormat == "NTSC")
                        return string.Format("{0}-{1}", videoSystem, videoFormat);
                    else
                        return "AAV";
                }
                else
                    return videoFormat;
            }
        }

        int ILightCurveDataProvider.NumberIntegratedFrames
        {
            get { return m_LCFile.Footer.AAVFrameIntegration > 0 ? m_LCFile.Footer.AAVFrameIntegration : 0; }
        }

	    int ILightCurveDataProvider.MinFrameNumber
	    {
			get
			{
				if (m_LightCurveController.Context.Binning > 0)
					return 0;
				else
					return (int) m_LCFile.Header.MinFrame;
			}
	    }

	    int ILightCurveDataProvider.MaxFrameNumber
	    {
			get
			{
				if (m_LightCurveController.Context.Binning > 0)
					return m_AllBinnedReadings[0].Count;
				else
					return (int)m_LCFile.Header.MaxFrame;
			}
	    }

        int ILightCurveDataProvider.CurrentlySelectedFrameNumber
        {
            get { return (int)m_LightCurveController.Context.SelectedFrameNo; }
        }

		void ILightCurveDataProvider.SetNoOccultationEvents()
		{
			if (m_EventTimesReport != null)
			{
				m_EventTimesReport.IsThisAMiss = true;
			}
		}

        public void SetTimeExtractionEngine(string engineAndVersion)
        {
            if (m_EventTimesReport != null)
            {
                m_EventTimesReport.Provider = engineAndVersion;
            }
        }

        void ILightCurveDataProvider.SetFoundOccultationEvent(int eventId, float dFrame, float rFrame, float dFrameErrorMinus, float dFrameErrorPlus, float rFrameErrorMinus, float rFrameErrorPlus, string dTime, string rTime, bool cameraDelaysKnownToAOTA)
		{
			if (m_EventTimesReport != null)
			{
				m_EventTimesReport.IsThisAMiss = false;

				var evt = new OccultationEventInfo()
				{
					EventId = eventId,
					DFrame = dFrame,
					RFrame = rFrame,
					DFrameErrorMinus = dFrameErrorMinus,
					DFrameErrorPlus = dFrameErrorPlus,
					RFrameErrorMinus = rFrameErrorMinus,
					RFrameErrorPlus = rFrameErrorPlus,
					DTimeString = dTime,
					RTimeString = rTime
				};

                if (m_LCFile != null)
                {
                    m_LCFile.Header.LcFile = m_LCFile;

                    // NOTE: For AOTA xx.0 is the beginning of the frame and xx.5 is the middle of the frame. 
                    //       Tangra associates all frame times with the middle of the frame so we adjust the AOTA frames to Tangra frames by subtracting 0.5 below
                    bool gotTimes = false;

                    if (m_LCFile.FrameTiming != null && m_LCFile.FrameTiming.Count > 0)
                    {
                        DateTime DTimeFrom = m_LCFile.Header.GetTimeForFrameFromFrameTiming(dFrame - 0.5 + dFrameErrorMinus, true);
                        DateTime DTimeTo = m_LCFile.Header.GetTimeForFrameFromFrameTiming(dFrame - 0.5 + dFrameErrorPlus, true);

                        evt.DTime = new DateTime((DTimeFrom.Ticks + DTimeTo.Ticks) / 2);
                        evt.DTimeErrorMS = (int)Math.Round(new TimeSpan(DTimeTo.Ticks - DTimeFrom.Ticks).TotalMilliseconds / 2);

                        DateTime RTimeFrom = m_LCFile.Header.GetTimeForFrameFromFrameTiming(rFrame - 0.5 + rFrameErrorMinus, true);
                        DateTime RTimeTo = m_LCFile.Header.GetTimeForFrameFromFrameTiming(rFrame - 0.5 + rFrameErrorPlus, true);

                        evt.RTime = new DateTime((RTimeFrom.Ticks + RTimeTo.Ticks) / 2);
                        evt.RTimeErrorMS = (int)Math.Round(new TimeSpan(RTimeTo.Ticks - RTimeFrom.Ticks).TotalMilliseconds / 2);

                        evt.DTimeMostProbable = m_LCFile.Header.GetTimeForFrameFromFrameTiming(dFrame - 0.5, true);
                        evt.RTimeMostProbable = m_LCFile.Header.GetTimeForFrameFromFrameTiming(rFrame - 0.5, true);

                        gotTimes = true;
                    }
                    else if (m_LCFile.Header.TimingType == MeasurementTimingType.UserEnteredFrameReferences && m_LCFile.Header.FirstTimedFrameNo != m_LCFile.Header.LastTimedFrameNo)
                    {
                        DateTime DTimeFrom = m_LCFile.Header.GetTimeForFrameFromManuallyEnteredTimes(dFrame - 0.5 + dFrameErrorMinus);
                        DateTime DTimeTo = m_LCFile.Header.GetTimeForFrameFromManuallyEnteredTimes(dFrame - 0.5 + dFrameErrorPlus);

                        evt.DTime = new DateTime((DTimeFrom.Ticks + DTimeTo.Ticks) / 2);
                        evt.DTimeErrorMS = (int)Math.Round(new TimeSpan(DTimeTo.Ticks - DTimeFrom.Ticks).TotalMilliseconds / 2);

                        DateTime RTimeFrom = m_LCFile.Header.GetTimeForFrameFromManuallyEnteredTimes(rFrame - 0.5 + rFrameErrorMinus);
                        DateTime RTimeTo = m_LCFile.Header.GetTimeForFrameFromManuallyEnteredTimes(rFrame - 0.5 + rFrameErrorPlus);

                        evt.RTime = new DateTime((RTimeFrom.Ticks + RTimeTo.Ticks) / 2);
                        evt.RTimeErrorMS = (int)Math.Round(new TimeSpan(RTimeTo.Ticks - RTimeFrom.Ticks).TotalMilliseconds / 2);

                        evt.DTimeMostProbable = m_LCFile.Header.GetTimeForFrameFromManuallyEnteredTimes(dFrame - 0.5);
                        evt.RTimeMostProbable = m_LCFile.Header.GetTimeForFrameFromManuallyEnteredTimes(rFrame - 0.5);

                        gotTimes = true;
                    }
                    else
                    {
                        // TODO: Use the times from AOTA??
                    }

                    DateTime? aotaDTime = null;
                    DateTime? aotaRTime = null;

                    // Parse the +/- part of each time e.g. "9 40 25.79 ± 0.07"
                    string[] tokens = dTime.Split('±');
                    if (tokens.Length == 2)
                    {
                        evt.DTimeErrorMSAOTA = (int)Math.Round(double.Parse(tokens[1].Trim()) * 1000.0);
                        string[] ttokens = tokens[0].Trim().Split(' ');

                        aotaDTime = m_LCFile.Header.GetFrameTime(m_LCFile.Header.MinFrame);
                        if (aotaDTime.HasValue)
                        {
                            double secs = double.Parse(ttokens[2]);
                            int secsInt = (int) secs;
                            int millilsecsInt = (int) Math.Round((secs - secsInt)*1000.0);
                            aotaDTime = new DateTime(aotaDTime.Value.Year, aotaDTime.Value.Month, aotaDTime.Value.Day, int.Parse(ttokens[0]), int.Parse(ttokens[1]), secsInt, millilsecsInt);

                            tokens = rTime.Split('±');
                            if (tokens.Length == 2)
                            {
                                evt.RTimeErrorMSAOTA = (int)Math.Round(double.Parse(tokens[1].Trim()) * 1000.0);
                                ttokens = tokens[0].Trim().Split(' ');

                                aotaRTime = m_LCFile.Header.GetFrameTime(m_LCFile.Header.MinFrame);
                                if (aotaRTime.HasValue)
                                {
                                    secs = double.Parse(ttokens[2]);
                                    secsInt = (int)secs;
                                    millilsecsInt = (int)Math.Round((secs - secsInt) * 1000.0);
                                    aotaRTime = new DateTime(aotaRTime.Value.Year, aotaRTime.Value.Month, aotaRTime.Value.Day, int.Parse(ttokens[0]), int.Parse(ttokens[1]), secsInt, millilsecsInt);

                                    if (aotaRTime.Value < aotaDTime.Value) aotaRTime.Value.AddDays(1);

                                    evt.DTimeAOTA = aotaDTime.Value;
                                    evt.RTimeAOTA = aotaRTime.Value;
                                }
                            }
                        }
                    }

                    if (gotTimes && m_EventTimesReport.TangraCanApplyInstrumentalDelays == InstrumentalDelayStatus.Yes)
                    {
                        double instrDelay = m_LCFile.GetInstrumentalDelayAtFrameInSeconds(dFrame - 0.5);
                        evt.DTime = evt.DTime.AddSeconds(instrDelay);
                        evt.RTime = evt.RTime.AddSeconds(instrDelay);
                        evt.DTimeMostProbable = evt.DTimeMostProbable.AddSeconds(instrDelay);
                        evt.RTimeMostProbable = evt.RTimeMostProbable.AddSeconds(instrDelay);
                    }

					if (aotaDTime.HasValue && aotaRTime.HasValue &&
					    (Math.Abs(new TimeSpan(aotaDTime.Value.Ticks - evt.DTime.Ticks).TotalMilliseconds) > 10 || Math.Abs(new TimeSpan(aotaRTime.Value.Ticks - evt.RTime.Ticks).TotalMilliseconds) > 10) &&
					    m_EventTimesReport.VideoFileFormat != "AVI" &&
					    m_EventTimesReport.TangraCanApplyInstrumentalDelays == InstrumentalDelayStatus.Yes &&
					    cameraDelaysKnownToAOTA)
					{
					    // AOTA and Tangra times are different. Ask the use which times they want to use
					    // Add a flag in the report to indicate which times to be used in OW
					    var frm = new frmChooseTimesToReport();
                        frm.SetTimes(evt);
                        frm.CameraNameTangra = m_LCFile.Footer.CameraName;
                        frm.CameraNameAOTA = m_EventTimesReport.CameraName;
                        frm.TangraKnowsCameraDelays = m_EventTimesReport.TangraCanApplyInstrumentalDelays == InstrumentalDelayStatus.Yes;
                        frm.AOTAKnowsCameraDelays = cameraDelaysKnownToAOTA;
					    frm.ShowDialog(this);

                        evt.ReportTangraTimesRatherThanAOTATimes = frm.UseTangrasTimes;
                        evt.InstrumentalDelaysApplied = evt.ReportTangraTimesRatherThanAOTATimes 
                            ? m_EventTimesReport.TangraCanApplyInstrumentalDelays == InstrumentalDelayStatus.Yes 
                            : cameraDelaysKnownToAOTA;
					}
					else if (m_EventTimesReport.VideoFileFormat == "AVI")
					{
					    // Always use the times provided by AOTA when working with AVI files
					    evt.InstrumentalDelaysApplied = cameraDelaysKnownToAOTA;
					    evt.ReportTangraTimesRatherThanAOTATimes = false;
					}
					else if ((m_EventTimesReport.VideoFileFormat == "ADV" || m_EventTimesReport.VideoFileFormat == "AAV") && !cameraDelaysKnownToAOTA)
					{
					    // For ADV/AAV files prefer Tangra's times (when AOTA doesn't know hwo to apply delays)
                        evt.InstrumentalDelaysApplied = m_EventTimesReport.TangraCanApplyInstrumentalDelays != InstrumentalDelayStatus.No;
                        evt.ReportTangraTimesRatherThanAOTATimes = true;
					}
                    else
                    {
                        // Otherwise use AOTA's Times
                        evt.InstrumentalDelaysApplied = cameraDelaysKnownToAOTA;
                        evt.ReportTangraTimesRatherThanAOTATimes = false;
                    }
                }

				m_EventTimesReport.Events.Add(evt);
			}
		}

		private void GetAOTAStarIndexes(out int occultedStarIndex, out int comp1Index, out int comp2Index, out int comp3Index)
		{
			comp1Index = -1;
			comp2Index = -1;
			comp3Index = -1;

			TrackedObjectConfig occultedTrackedObject = m_LCFile.Footer.TrackedObjects.SingleOrDefault(x => x.TrackingType == TrackingType.OccultedStar);
			occultedStarIndex = occultedTrackedObject != null ? m_LCFile.Footer.TrackedObjects.IndexOf(occultedTrackedObject) : 0;

			if (!m_IncludeObjects[occultedStarIndex])
			{
				// If the occulted star is not currently displayed, then select the first visible object to be the occulted one
				if (m_IncludeObjects[0])
					occultedStarIndex = 0;
				else if (m_IncludeObjects[1])
					occultedStarIndex = 1;
				else if (m_IncludeObjects[2])
					occultedStarIndex = 2;
				else if (m_IncludeObjects[3])
					occultedStarIndex = 4;
			}

			int nextCompStarIndex = 0;
			for (int i = 0; i < m_LCFile.Header.ObjectCount; i++)
			{
				if (m_IncludeObjects[i] && i != occultedStarIndex)
				{
					nextCompStarIndex++;

					switch (nextCompStarIndex)
					{
						case 1:
							comp1Index = i;
							break;
						case 2:
							comp2Index = i;
							break;
						case 3:
							comp3Index = i;
							break;
					}
				}
			}
		}

		ISingleMeasurement[] ILightCurveDataProvider.GetTargetMeasurements()
		{
			int occultedStarIndex;
			int comp1Index;
			int comp2Index;
			int comp3Index;

			GetAOTAStarIndexes(out occultedStarIndex, out comp1Index, out comp2Index, out comp3Index);

			if (m_LightCurveController.Context.Binning > 0)
				return m_AllBinnedReadings[occultedStarIndex].Select(x => new SingleMeasurement(x, occultedStarIndex, x.BinMiddleFrameNo + (m_LightCurveController.Context.Binning / 2.0), m_LCFile, m_TimestampDiscrepencyFlag)).ToArray();
			else
                return m_LightCurveController.Context.AllReadings[occultedStarIndex].Select(x => new SingleMeasurement(x, x.CurrFrameNo, m_LCFile, m_TimestampDiscrepencyFlag)).ToArray();
		}

        ITangraDrawingSettings ILightCurveDataProvider.GetTangraDrawingSettings()
        {
            return new TangraDrawingSettings(m_DisplaySettings);
        }

		ISingleMeasurement[] ILightCurveDataProvider.GetComparisonObjectMeasurements(int comparisonObjectId)
		{
			int occultedStarIndex;
			int comp1Index;
			int comp2Index;
			int comp3Index;

			GetAOTAStarIndexes(out occultedStarIndex, out comp1Index, out comp2Index, out comp3Index);
			if (m_LightCurveController.Context.Binning > 0)
			{
				if (comparisonObjectId == 0 && comp1Index > -1)
					return m_AllBinnedReadings[comp1Index].Select(x => new SingleMeasurement(x, comp1Index, x.BinMiddleFrameNo + (m_LightCurveController.Context.Binning / 2.0), m_LCFile, m_TimestampDiscrepencyFlag)).ToArray();
				else if (comparisonObjectId == 1 && comp2Index > -1)
					return m_AllBinnedReadings[comp2Index].Select(x => new SingleMeasurement(x, comp2Index, x.BinMiddleFrameNo + (m_LightCurveController.Context.Binning / 2.0), m_LCFile, m_TimestampDiscrepencyFlag)).ToArray();
				else if (comparisonObjectId == 2 && comp3Index > -1)
					return m_AllBinnedReadings[comp3Index].Select(x => new SingleMeasurement(x, comp3Index, x.BinMiddleFrameNo + (m_LightCurveController.Context.Binning / 2.0), m_LCFile, m_TimestampDiscrepencyFlag)).ToArray();
			}
			else
			{
				if (comparisonObjectId == 0 && comp1Index > -1)
                    return m_LightCurveController.Context.AllReadings[comp1Index].Select(x => new SingleMeasurement(x, x.CurrFrameNo, m_LCFile, m_TimestampDiscrepencyFlag)).ToArray();
				else if (comparisonObjectId == 1 && comp2Index > -1)
                    return m_LightCurveController.Context.AllReadings[comp2Index].Select(x => new SingleMeasurement(x, x.CurrFrameNo, m_LCFile, m_TimestampDiscrepencyFlag)).ToArray();
				else if (comparisonObjectId == 2 && comp3Index > -1)
                    return m_LightCurveController.Context.AllReadings[comp3Index].Select(x => new SingleMeasurement(x, x.CurrFrameNo, m_LCFile, m_TimestampDiscrepencyFlag)).ToArray();
			}

			return null;
		}

		void ILightCurveDataProvider.GetIntegrationRateAndFirstFrame(out int integrationRate, out int firstIntegratingFrame)
		{
			// TODO: Ask the user to confirm
			integrationRate = 1;
			firstIntegratingFrame = 0;
		}

    }
}
