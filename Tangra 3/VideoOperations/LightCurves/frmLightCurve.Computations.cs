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
using Tangra.Config;
using Tangra.Model.Config;
using Tangra.Model.Numerical;
using Tangra.Model.Video;
using Tangra.Model.VideoOperations;
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
        private List<List<LCMeasurement>> m_AllReadings = new List<List<LCMeasurement>>(new List<LCMeasurement>[] { new List<LCMeasurement>(), new List<LCMeasurement>(), new List<LCMeasurement>(), new List<LCMeasurement>() });
        private List<List<LCMeasurement>> m_InitialNoFilterReadings = null;

        // NOTE: This is used by the SDK which will not be released in 
    	//private TangraLightCurveImpl m_LightCurveImpl;

        private List<BinnedValue>[] m_AllBinnedReadings = new List<BinnedValue>[4]
        {
            new List<BinnedValue>(), new List<BinnedValue>(), new List<BinnedValue>(), new List<BinnedValue>()
        };

        private int m_MinX = 45;
        private int m_MinY = 25;
        private int m_MaxY, m_MaxX;

        private LightCurveContext m_Context;

        private void ReComputeSignalSeries()
        {
            int minValue = int.MaxValue;
            int maxValue = int.MinValue;

            for (int i = 0; i < m_Header.ObjectCount; i++)
            {
                // We may be normalizing on object that is not displayed
				//if (!m_IncludeObjects[i]) continue;

                for (int j = 0; j < m_AllReadings[i].Count; j++)
                {
                    LCMeasurement reading = m_AllReadings[i][j];
                    int adjustedValue = 0;

                    if (m_Context.ProcessingType == ProcessingType.SignalOnly)
                    {
                        adjustedValue = (int)reading.TotalReading;
                    }
                    else if (m_Context.ProcessingType == ProcessingType.SignalMinusBackground)
                    {
                        adjustedValue = (int)reading.TotalReading - (int)reading.TotalBackground;
                    }
                    else if (m_Context.ProcessingType == ProcessingType.BackgroundOnly)
                    {
                        adjustedValue = (int)reading.TotalBackground;
                    }
					else if (m_Context.ProcessingType == ProcessingType.SignalDividedByBackground)
					{
						int intBG = (int) reading.TotalBackground;
						if (intBG != 0)
							adjustedValue = 100 * (int)reading.TotalReading / (int)reading.TotalBackground;
						else
							adjustedValue = 100 * (int)reading.TotalReading;
					}
					else if (m_Context.ProcessingType == ProcessingType.SignalDividedByNoise)
					{
						adjustedValue = (int)(100 * ComputeSignalToNoiceRatio(i, j, false));
					}

                    if (m_IncludeObjects[i])
                    {
                        if (minValue > adjustedValue) minValue = adjustedValue;
                        if (maxValue < adjustedValue) maxValue = adjustedValue;                        
                    }

                    reading.AdjustedReading = adjustedValue;
                    m_AllReadings[i][j] = reading;
                }
            }

            m_Header.MinAdjustedReading = minValue;
            m_Header.MaxAdjustedReading = maxValue;

            if (m_Context.Binning > 0)
            {
                ComputeBinning(); 
            }

            if (m_Context.Normalisation > -1)
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

        	//SetSDKLightCurveData();
        }

        //private void SetSDKLightCurveData()
        //{
        //    m_LightCurveImpl.LightCurveContextImpl.DisplayedValueType = (SDK.DisplayedValueType)((int)m_Context.ProcessingType);
        //    m_LightCurveImpl.LightCurveContextImpl.BinSize = (byte)m_Context.Binning;
        //    m_LightCurveImpl.LightCurveContextImpl.NormalizationObject = m_Context.Normalisation;

        //    if (m_LightCurveImpl.LightCurveContextImpl.BinSize == 0)
        //    {
        //        // No Binning
        //        for (int i = 0; i < m_Header.ObjectCount; i++)
        //        {
        //            List<TangraLightCurveImpl.TangraLightCurveDataPointImpl> data = m_LightCurveImpl.DataPointsImpl[i];

        //            bool insertMode = false;
        //            if (data.Count != m_AllReadings[i].Count)
        //            {
        //                data = new List<TangraLightCurveImpl.TangraLightCurveDataPointImpl>();
        //                insertMode = true;
        //            }

        //            for (int j = 0; j < m_AllReadings[i].Count; j++)
        //            {
        //                LCMeasurement mea = m_AllReadings[i][j];

        //                if (insertMode)
        //                    data.Add(new TangraLightCurveImpl.TangraLightCurveDataPointImpl(mea));
        //                else
        //                    data[j].Value = mea.AdjustedReading;
        //            }

        //            if (insertMode)
        //                m_LightCurveImpl.DataPointsImpl[i] = data;
        //        }
        //    }
        //    else
        //    {
        //        // Binning
        //        for (int i = 0; i < m_Header.ObjectCount; i++)
        //        {
        //            List<TangraLightCurveImpl.TangraLightCurveDataPointImpl> data = m_LightCurveImpl.DataPointsImpl[i];

        //            bool insertMode = false;
        //            if (data.Count != m_AllBinnedReadings[i].Count)
        //            {
        //                data = new List<TangraLightCurveImpl.TangraLightCurveDataPointImpl>();
        //                insertMode = true;
        //            }
					
        //            for (int j = 0; j < m_AllBinnedReadings[i].Count; j++)
        //            {
        //                BinnedValue mea = m_AllBinnedReadings[i][j];

        //                if (insertMode)
        //                    data.Add(new TangraLightCurveImpl.TangraLightCurveDataPointImpl(mea));
        //                else
        //                    data[j].Value = (float)mea.AdjustedValue;
        //            }

        //            if (insertMode)
        //                m_LightCurveImpl.DataPointsImpl[i] = data;
        //        }
        //    }

        //    m_LightCurveImpl.LightCurveContextImpl.MinAdjustedReading = m_Header.MinAdjustedReading;
        //    m_LightCurveImpl.LightCurveContextImpl.MaxAdjustedReading = m_Header.MaxAdjustedReading;

        //    m_LightCurveImpl.FireDataPointsRecomputed();
        //}

		private double ComputeSignalToNoiceRatio(int objectId, int middleFrameId, bool useAdjustedReading)
		{
			int halfFrameWindow = TangraConfig.Settings.Photometry.SNFrameWindow / 2;

			int from = Math.Max(0, middleFrameId - halfFrameWindow);
			int to = Math.Min(m_AllReadings[objectId].Count, from + halfFrameWindow * 2);
			double sum = 0;
			for (int k = from; k < to; k++)
			{
				sum += useAdjustedReading ? (int)m_AllReadings[objectId][k].AdjustedReading : (int)m_AllReadings[objectId][k].TotalReading;
			}
			double avrge = sum / (to - from);
			sum = 0;
			for (int k = from; k < to; k++)
			{
				double residual = (useAdjustedReading ? (int)m_AllReadings[objectId][k].AdjustedReading : (int)m_AllReadings[objectId][k].TotalReading) - avrge;
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
                // We may be normalizing on object that is not displayed
				//if (!m_IncludeObjects[i]) continue;

                m_AllBinnedReadings[i].Clear();

                int idx = 1;
                int binnedSum = 0;
                int binnedSuccessfulReadings = 0;
                int binNo = 1;
                BinnedValue binnedValue = new BinnedValue();
                binnedValue.ReadingIndexFrom = 0;
                binnedValue.BinNo = binNo;

                foreach (LCMeasurement reading in m_AllReadings[i])
                {
                    if (idx % m_Context.Binning == 0)
                    {
                        binnedValue.TotalSum = binnedSum;
                    	binnedValue.AdjustedValue = binnedSum * 1.0 / m_Context.Binning;
						binnedValue.IsSuccessfulReading = binnedSuccessfulReadings == m_Context.Binning; // Average out the non binned values due to unsuccessful readings

                        binnedValue.ReadingIndexTo = idx - 1;
                        m_AllBinnedReadings[i].Add(binnedValue);

                        if (m_IncludeObjects[i])
                        {
                            if (minValue > binnedValue.AdjustedValue) minValue = (int) binnedValue.AdjustedValue - 1;
                            if (maxValue < binnedValue.AdjustedValue) maxValue = (int) binnedValue.AdjustedValue + 1;
                        }

                        binNo++;
                        binnedSuccessfulReadings = 0;

						binnedSum = reading.AdjustedReading;

                        if (reading.IsSuccessfulReading)
							binnedSuccessfulReadings++;
                        
                        binnedValue = new BinnedValue();
                        binnedValue.ReadingIndexFrom = idx;
                        binnedValue.BinNo = binNo;
                    }
                    else
                    {
						binnedSum += reading.AdjustedReading;

                        if (reading.IsSuccessfulReading)
							binnedSuccessfulReadings++;

                        //binnedSum += reading.PrevSuccessfulReading.AdjustedReading;
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

            switch(m_Context.NormMethod)
            {
                case LightCurveContext.NormalisationMethod.LinearFit:
                    if (m_Context.Binning > 0)
                        PerformLinearFitBinnedNormalisation(ref normalIndexes);
                    else
                        PerformLinearFitReadingsNormalisation(ref normalIndexes);
                    break;

                case LightCurveContext.NormalisationMethod.FrameByFrame:
                    if (m_Context.Binning > 0)
                        PerformAverageBinnedNormalisation(ref normalIndexes, 1);
                    else
                        PerformAverageReadingsNormalisation(ref normalIndexes, 1);
                    break;

                case LightCurveContext.NormalisationMethod.Average4Frame:
                    if (m_Context.Binning > 0)
                        PerformAverageBinnedNormalisation(ref normalIndexes, 4);
                    else
                        PerformAverageReadingsNormalisation(ref normalIndexes, 4);
                    break;

                case LightCurveContext.NormalisationMethod.Average16Frame:
                    if (m_Context.Binning > 0)
                        PerformAverageBinnedNormalisation(ref normalIndexes, 16);
                    else
                        PerformAverageReadingsNormalisation(ref normalIndexes, 16);
                    break;
            }

            int minValue = int.MaxValue;
            int maxValue = int.MinValue;

           
            if (m_Context.Binning > 0)
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
						if (minValue > normValue) minValue = (int)normValue;
						if (maxValue < normValue) maxValue = (int)normValue;
                    }
                }
            }
            else
            {
                while (normalIndexes.Count < m_AllReadings[0].Count) normalIndexes.Add(1);

                for (int i = 0; i < m_Header.ObjectCount; i++)
                {
					if (!m_IncludeObjects[i]) continue;

                    int upper = m_AllReadings[i].Count;
                    for (int j = 0; j < upper; j++)
                    {
						double normValue = Math.Round(m_AllReadings[i][j].AdjustedReading * normalIndexes[j]);
						LCMeasurement measurement = m_AllReadings[i][j];
                        measurement.AdjustedReading = (int)normValue;
						if (minValue > normValue) minValue = (int)normValue;
						if (maxValue < normValue) maxValue = (int)normValue;
                        m_AllReadings[i][j] = measurement;
					}
                }
            }

        	m_Header.MaxAdjustedReading = maxValue;
            m_Header.MinAdjustedReading = minValue;
        }

        private void PerformLinearFitBinnedNormalisation(ref List<double> normalIndexes)
        {
            var linearRegression = new LinearRegression();

            foreach (BinnedValue binnedValue in m_AllBinnedReadings[m_Context.Normalisation])
            {
                linearRegression.AddDataPoint(binnedValue.BinNo, binnedValue.AdjustedValue);
            }

            linearRegression.Solve();

            double firstValue = linearRegression.ComputeY(m_AllBinnedReadings[m_Context.Normalisation][0].BinNo);

            foreach (BinnedValue binnedValue in m_AllBinnedReadings[m_Context.Normalisation])
            {
                normalIndexes.Add(firstValue / linearRegression.ComputeY(binnedValue.BinNo));
            }
        }

        private void PerformLinearFitReadingsNormalisation(ref List<double> normalIndexes)
        {
            var linearRegression = new LinearRegression();

            foreach (LCMeasurement reading in m_AllReadings[m_Context.Normalisation])
            {
                linearRegression.AddDataPoint(reading.CurrFrameNo, reading.AdjustedReading);
            }

            linearRegression.Solve();

            double firstValue = linearRegression.ComputeY(m_AllReadings[m_Context.Normalisation][0].CurrFrameNo);

            foreach (LCMeasurement reading in m_AllReadings[m_Context.Normalisation])
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
                    averages.Add(m_AllReadings[m_Context.Normalisation][i].AdjustedReading);
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
                            sum += m_AllReadings[m_Context.Normalisation][idx].AdjustedReading;
                            readings++;
                        }
                    }

                    if (readings == 0)
                        averages.Add(0);
                    else
                        averages.Add((1.0 * sum / readings));                    
                }
            }

            double firstValue = averages[0];

            for (int i = 0; i < maxIdx; i++)
            {
                normalIndexes.Add(firstValue / averages[i]);
            }         
        }


        private void PerformAverageBinnedNormalisation(ref List<double> normalIndexes, int numFrames)
        {
            List<double> averages = new List<double>();

            int maxIdx = m_AllBinnedReadings[m_Context.Normalisation].Count;
            for (int i = 0; i < maxIdx; i++)
            {
                double sum = 0;
                int readings = 0;
                for (int j = -numFrames / 2; j <= numFrames / 2; j++)
                {
                    int idx = i + j;
                    if (idx >= 0 && idx < maxIdx)
                    {
                        sum += m_AllBinnedReadings[m_Context.Normalisation][idx].AdjustedValue;
                        readings++;
                    }
                }

                if (readings == 0)
                    averages.Add(0);
                else
                    averages.Add((1.0 * sum / readings));
            }

            double firstValue = averages[0];

            for (int i = 0; i < maxIdx; i++)
            {
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

            uint[] preferredIntervals = new uint[] { 50000, 25000, 10000, 5000, 2500, 1000, 500, 250, 100, 50, 25, 10, 5, 1, 1 };
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
							//return goodIntervals[i + 1];
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

		public bool ExportToCSV(string fileName)
		{
			StringBuilder output = new StringBuilder();

		    double absoluteTimeError = m_TimestampDiscrepencyFlag
		                                   ? m_Header.GetAbsoluteTimeDeltaInMilliseconds()
		                                   : 0;

			uint objCount = m_Header.ObjectCount;
			if (m_Context.Binning > 0)
			{
				// Binned frames
				int count = m_AllBinnedReadings[0].Count;
				int firstFrameNoInBin = (int)m_Header.MinFrame;

                CSVExportAddCommonHeader(output, true);

				output.Append("BinNo,Time (UT)");

				for (int j = 0; j < objCount; j++)
					output.AppendFormat(",Binned Measurment ({0})", j + 1);

				output.AppendLine();

                string isBadTimeString = null;

                double resolutionInSecs = m_Context.Binning / (2.0 * m_Header.FramesPerSecond);
                string timeFormat = "HH:mm:ss";

                if (resolutionInSecs < 0.06)
                {
                    timeFormat = "HH:mm:ss.ff";
                    if (absoluteTimeError > 5) isBadTimeString = "??:??:??.??";
                }
                else if (resolutionInSecs < 0.6)
                {
                    timeFormat = "HH:mm:ss.f";
                    if (absoluteTimeError > 50) isBadTimeString = "??:??:??.?";
                }
                else
                    if (absoluteTimeError > 500) isBadTimeString = "??:??:??";

				for (int i = 0; i < count; i++)
				{
					DateTime middleBinTime = m_LCFile.GetTimeForFrame(firstFrameNoInBin + (m_Context.Binning / 2.0));
                        //FirstFrameTime.AddSeconds((firstFrameNoInBin - m_Header.FirstFrameInVideoFile + (m_Context.Binning / 2.0)) / m_Header.FramesPerSecond);

				    string timeStr;
					if (isBadTimeString != null)
					{
						if (i == m_Header.FirstTimedFrameNo)
							timeStr = m_Header.FirstTimedFrameTime.ToString(timeFormat);
						else if (i == m_Header.LastTimedFrameNo)
							timeStr = m_Header.SecondTimedFrameTime.ToString(timeFormat);
						else
							timeStr = isBadTimeString;
					}
					else
					{
						if (middleBinTime == DateTime.MaxValue)
							timeStr = "";
						else
							timeStr = middleBinTime.ToString(timeFormat);
					}

					output.AppendFormat("{0},{1}", i + 1, timeStr);

					for (int j = 0; j < objCount; j++)
					{
						BinnedValue reading = m_AllBinnedReadings[j][i];
						output.AppendFormat(",{0}",
							reading.AdjustedValue.ToString(CultureInfo.InvariantCulture));
					}

					output.AppendLine();

					firstFrameNoInBin += m_Context.Binning;
				}
			}
			else
			{
                CSVExportAddCommonHeader(output, false);

                bool onlyExportSignalMunusBg = m_Context.SignalMethod == TangraConfig.PhotometryReductionMethod.PsfPhotometryAnalytical;

				int count = m_AllReadings[0].Count;

				output.Append("FrameNo,Time (UT)");

				for (int j = 0; j < objCount; j++)
				{
                    if (!onlyExportSignalMunusBg)
					    output.AppendFormat(",Signal ({0}), Background ({0})", j + 1);
                    else
                        output.AppendFormat(",SignalMinusBackground ({0})", j + 1);
				}
				output.AppendLine();

			    string isBadTimeString = null;
                if (absoluteTimeError > 5) isBadTimeString = "??:??:??.???";
                string timeFormat = "HH:mm:ss.fff";

				for (int i = 0; i < count; i++)
				{
					uint frameNo = m_AllReadings[0][i].CurrFrameNo;
					DateTime currFrameTime = m_LCFile.GetTimeForFrame(frameNo);
                            //FirstFrameTime.AddSeconds((frameNo - m_Header.MinFrame)/m_Header.FramesPerSecond);

                    string timeStr;
					if (isBadTimeString != null)
					{
						if (currFrameTime == DateTime.MaxValue)
							timeStr = "";
						else if (frameNo == m_Header.FirstTimedFrameNo)
							timeStr = m_Header.FirstTimedFrameTime.ToString(timeFormat);
						else if (frameNo == m_Header.LastTimedFrameNo)
							timeStr = m_Header.SecondTimedFrameTime.ToString(timeFormat);
						else
							timeStr = isBadTimeString;
					}
					else
					{
						if (currFrameTime == DateTime.MaxValue)
							timeStr = "";
						else
							timeStr = currFrameTime.ToString(timeFormat);
					}

					output.AppendFormat("{0},{1}", frameNo, timeStr);

					for (int j = 0; j < objCount; j++)
					{
						LCMeasurement reading = m_AllReadings[j][i];
                        if (reading.IsSuccessfulReading)
                        {
                            if (!onlyExportSignalMunusBg)
                                output.AppendFormat(",{0},{1}",
                                                    reading.TotalReading.ToString(CultureInfo.InvariantCulture),
                                                    reading.TotalBackground.ToString(CultureInfo.InvariantCulture));
                            else
                                output.AppendFormat(",{0}", reading.TotalReading.ToString(CultureInfo.InvariantCulture));
                        }
                        else
                            output.Append(",,");
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

        private void CSVExportAddCommonHeader(StringBuilder output, bool binning)
        {
            Version ver = Assembly.GetExecutingAssembly().GetName().Version;

            output.AppendFormat("Tangra v{0}", ver.ToString()); output.AppendLine();
            if (binning)
                output.AppendFormat("Measurments of {0} objects; Bins of {1} frames; Binned Measurement = {2}", m_Header.ObjectCount, m_Context.Binning, m_Context.ProcessingType);
            else
                output.AppendFormat("Measurments of {0} objects", m_Header.ObjectCount);

            output.AppendLine();
            output.Append(m_Header.PathToVideoFile); output.AppendLine();
            output.AppendFormat("{0} {1}", m_Header.ReductionType, m_Header.SourceInfo); output.AppendLine();
            output.AppendLine();output.AppendLine();
            bool addPSFReductionDetails = m_Context.SignalMethod != TangraConfig.PhotometryReductionMethod.AperturePhotometry;
            bool addPSFAverageModelDetails = addPSFReductionDetails && m_Context.PsfFittingMethod == TangraConfig.PsfFittingMethod.LinearFitOfAveragedModel;
            bool addIntegrationInfo = m_Footer.ReductionContext.FrameIntegratingMode != FrameIntegratingMode.NoIntegration;

            output.Append("Reversed Gamma, Colour, Measured Band, Integration, Digital Filter, Signal Method, Background Method");
            if (addPSFReductionDetails) output.Append(", PSF Fitting");
            if (addPSFAverageModelDetails) output.Append(", Modeled FWHM, Average FWHM"); 
            output.AppendLine();
            output.AppendFormat("{0},{1},{2},{3},{4},{5},{6}", 
                m_Context.EncodingGamma.ToString("0.00"),
                m_Footer.ReductionContext.IsColourVideo ? "yes" : "no", m_Footer.ReductionContext.ColourChannel,
                addIntegrationInfo
                    ? string.Format("{0} {1} of {2} frames", m_Footer.ReductionContext.PixelIntegrationType, m_Footer.ReductionContext.FrameIntegratingMode, m_Footer.ReductionContext.NumberFramesToIntegrate) 
                    : "no",

            m_Context.Filter, m_Context.SignalMethod, m_Context.BackgroundMethod);
            if (addPSFReductionDetails) output.AppendFormat(",{0}", m_Context.PsfFittingMethod);
            if (addPSFAverageModelDetails) output.AppendFormat(",{0},{1}", float.IsNaN(m_Context.ManualAverageFWHM) ? "auto" : "manual", !float.IsNaN(m_Context.ManualAverageFWHM) ? m_Context.ManualAverageFWHM.ToString("0.00") : m_Footer.RefinedAverageFWHM.ToString("0.00"));

            output.AppendLine();output.AppendLine();
            output.Append("Object, Type, Aperture, Tolerance, FWHM, Measured, StartingX, StartingY, Fixed"); output.AppendLine();
            for (int j = 0; j < m_Header.ObjectCount; j++)
            {
                TrackedObjectConfig obj = m_Footer.TrackedObjects[j];
                output.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7},{8}",
                    j + 1, obj.TrackingType.ToString(), m_Context.ReProcessApertures[j].ToString("0.00"),
                    obj.TrackingType == TrackingType.OccultedStar ? obj.PositionTolerance.ToString("0.00") : "", obj.RefinedFWHM.ToString("0.00"),
                    obj.MeasureThisObject ? "yes" : "no",
                    obj.ApertureStartingX.ToString("0.0"), obj.ApertureStartingY.ToString("0.0"), obj.IsFixedAperture ? "yes" : "no");
                output.AppendLine();
            }
            output.AppendLine(); output.AppendLine(); 
        }

        //internal class CSVCommonHeaderInfo
        //{
        //    public bool IsTangraExport;
        //    public bool IsCompatible;
        //    public string TangraVersion;
        //    public bool IsBinnedMeasurements;
        //    public int ObjectCount;
        //    public LightCurveReductionType ReductionType;
        //    public bool IsNotAperturePhotometry;
        //    public bool IsLinearFitOfAveragedModel;
        //    public string EncodingGamma;
        //    public bool IsColourVideo;
        //    public Tangra.Config.ColourChannel ColourChannel;
        //    public bool IsUsingIntegration;
        //    public LightCurveContext.FilterType Filter;
        //    public PhotometryReductionMethod SignalMethod;
        //    public BackgroundMethod BackgroundMethod;
        //    public PsfFittingMethod PsfFittingMethod;
        //    public bool IsManualAverageFWHM;
        //    public string ManualAverageFWHM;
        //    public PixelIntegrationType PixelIntegrationType;
        //    public FrameIntegratingMode FrameIntegratingMode;
        //    public int NumberFramesToIntegrate;
        //}

        //private CSVCommonHeaderInfo CSVParseCommonHeader(string[] lines)
        //{
        //    var rv = new CSVCommonHeaderInfo();

        //    if (lines != null && lines.Length > 13)
        //    {
        //        Regex regexLine1 = new Regex("^Tangra v(\\d+\\.\\d+\\.\\d+\\.\\d+\\.)$");
        //        Match line1Match = regexLine1.Match(lines[0]);
        //        if (line1Match.Success)
        //        {
        //            rv.TangraVersion = line1Match.Groups[1].Value;
        //            rv.IsTangraExport = true;

        //            if (lines[1].Contains("Bins of"))
        //            {
        //                rv.IsBinnedMeasurements = true;
        //            }
        //            else
        //            {
        //                Regex regexLine2 = new Regex("^Measurments of (\\d+) objects$");

        //                Match line2Match = regexLine2.Match(lines[1]);
        //                if (line2Match.Success)
        //                {
        //                    rv.ObjectCount = int.Parse(line2Match.Groups[1].Value);

        //                    if (m_Header.ObjectCount == rv.ObjectCount)
        //                    {

        //                        // lines[2] == m_Header.PathToVideoFile

        //                        Regex regexLine4 = new Regex("^([^\\s]+)\\s([^\\s]+)$");

        //                        Match line4Match = regexLine4.Match(lines[3]);
        //                        if (line4Match.Success)
        //                        {
        //                            rv.ReductionType = (LightCurveReductionType)Enum.Parse(typeof(LightCurveReductionType), line4Match.Groups[1].Value);

        //                            if (lines[6].StartsWith("Reversed Gamma, Colour, Measured Band, Integration, Digital Filter, Signal Method, Background Method"))
        //                            {
        //                                if (lines[6].Contains(", PSF Fitting")) rv.IsNotAperturePhotometry = true;
        //                                if (lines[6].Contains(", Modeled FWHM, Average FWHM")) rv.IsLinearFitOfAveragedModel = true;

        //                                if (rv.IsNotAperturePhotometry ^ m_Context.SignalMethod != PhotometryReductionMethod.AperturePhotometry)
        //                                {
        //                                    string[] tokens = lines[7].Split(',');
        //                                    if (tokens.Length == 7 || tokens.Length == 8 || tokens.Length == 10)
        //                                    {
        //                                        rv.EncodingGamma = tokens[0];
        //                                        rv.IsColourVideo = tokens[1] == "yes";
        //                                        rv.ColourChannel = (Tangra.Config.ColourChannel)Enum.Parse(typeof(Tangra.Config.ColourChannel), tokens[2]);
        //                                        rv.IsUsingIntegration = tokens[3] != "no";

        //                                        if (rv.IsUsingIntegration)
        //                                        {
        //                                            Regex regexIntegration = new Regex("^([^\\s]+)\\s([^\\s]+)\\sof\\s([^\\s]+)\\sframes$");
        //                                            Match matchIntegration = regexIntegration.Match(tokens[3]);
        //                                            if (matchIntegration.Success)
        //                                            {
        //                                                rv.PixelIntegrationType = (PixelIntegrationType)Enum.Parse(typeof(PixelIntegrationType), matchIntegration.Groups[1].Value);
        //                                                rv.FrameIntegratingMode = (FrameIntegratingMode)Enum.Parse(typeof(FrameIntegratingMode), matchIntegration.Groups[2].Value);
        //                                                rv.NumberFramesToIntegrate  = int.Parse(matchIntegration.Groups[3].Value);
        //                                            }
        //                                        }
        //                                        rv.Filter = (LightCurveContext.FilterType)Enum.Parse(typeof(LightCurveContext.FilterType), tokens[4]);
        //                                        rv.SignalMethod = (PhotometryReductionMethod)Enum.Parse(typeof(PhotometryReductionMethod), tokens[5]);
        //                                        rv.BackgroundMethod = (BackgroundMethod)Enum.Parse(typeof(BackgroundMethod), tokens[6]);

        //                                        if (tokens.Length == 8)
        //                                        {
        //                                            rv.PsfFittingMethod = (PsfFittingMethod)Enum.Parse(typeof(PsfFittingMethod), tokens[7]);
        //                                        }
        //                                        else if (tokens.Length == 10)
        //                                        {
        //                                            rv.IsManualAverageFWHM = (tokens[8] == "manual");
        //                                            rv.ManualAverageFWHM = tokens[9];
        //                                        }

        //                                        if (m_Context.EncodingGamma.ToString("0.00") == rv.EncodingGamma &&
        //                                            m_Footer.ReductionContext.IsColourVideo ^ rv.IsColourVideo &&
        //                                            m_Footer.ReductionContext.ColourChannel == rv.ColourChannel && 
        //                                            m_Footer.ReductionContext.FrameIntegratingMode != FrameIntegratingMode.NoIntegration ^ rv.IsUsingIntegration &&
        //                                            m_Context.Filter == rv.Filter &&
        //                                            m_Context.SignalMethod == rv.SignalMethod &&
        //                                            m_Context.BackgroundMethod == rv.BackgroundMethod)
        //                                        {
        //                                            //if (addPSFReductionDetails) output.AppendFormat(",{0}", m_Context.PsfFittingMethod);
        //                                            //if (addPSFAverageModelDetails) output.AppendFormat(",{0},{1}", float.IsNaN(m_Context.ManualAverageFWHM) ? "auto" : "manual", !float.IsNaN(m_Context.ManualAverageFWHM) ? m_Context.ManualAverageFWHM.ToString("0.00") : m_Footer.RefinedAverageFWHM.ToString("0.00"));
        //                                            //
        //                                            //                                                    addIntegrationInfo
        //                                            //        ? string.Format("{0} {1} of {2} frames", m_Footer.ReductionContext.PixelIntegrationType, m_Footer.ReductionContext.FrameIntegratingMode, m_Footer.ReductionContext.NumberFramesToIntegrate)
        //                                            //        : "no",


        //                                            if (lines[10] == "Object, Type, Aperture, Tolerance, FWHM, Measured, StartingX, StartingY, Fixed")
        //                                            {

        //                                                //for (int j = 0; j < m_Header.ObjectCount; j++)
        //                                                //{
        //                                                //    TrackedObjectConfig obj = m_Footer.TrackedObjects[j];
        //                                                //    output.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7},{8}",
        //                                                //        j + 1, obj.TrackingType.ToString(), m_Context.ReProcessApertures[j].ToString("0.00"),
        //                                                //        obj.TrackingType == TrackingType.OccultedStar ? obj.PositionTolerance.ToString("0.00") : "", obj.RefinedFWHM.ToString("0.00"),
        //                                                //        obj.MeasureThisObject ? "yes" : "no",
        //                                                //        obj.ApertureStartingX.ToString("0.0"), obj.ApertureStartingY.ToString("0.0"), obj.IsFixedAperture ? "yes" : "no");
        //                                                //    output.AppendLine();
        //                                                //}
        //                                                //output.AppendLine(); output.AppendLine();  

        //                                                rv.IsCompatible = true;
        //                                            }                                                    
        //                                        }
        //                                    }                                                                                               
        //                                }                                
        //                            }
        //                        }                                                                
        //                    }
        //                }                        
        //            }
        //        }              
        //    }

        //    return rv;
        //}

        //internal void UpdateLightCurveFromCSV(string[] csvLines)
        //{
        //    StringBuilder output = new StringBuilder();
        //    CSVExportAddCommonHeader(output, false);

        //    // TODO: To check the compatibility, Verify that lines from 2 to end of header are same as the lines from the file.


        //    //CSVCommonHeaderInfo headerInfo = CSVParseCommonHeader(csvLines);
        //    //if (!headerInfo.IsTangraExport)
        //    //{
        //    //    MessageBox.Show(this, "This is not a Tangra Light Curve export file.", "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    //    return;
        //    //}

        //    //if (!headerInfo.IsCompatible)
        //    //{
        //    //    MessageBox.Show(this, "This Tangra Light Curve export file is not compatible with the current light curve.", "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    //    return;
        //    //}

        //    //// TODO: Set all the header related stuff (measurement methods etc).

        //    bool onlyExportSignalMunusBg = m_Context.SignalMethod == PhotometryReductionMethod.PsfPhotometryAnalytical;

        //    int count = m_AllReadings[0].Count;

        //    output.Append("FrameNo,Time (UT)");

        //    for (int j = 0; j < objCount; j++)
        //    {
        //        if (!onlyExportSignalMunusBg)
        //            output.AppendFormat(",Signal ({0}), Background ({0})", j + 1);
        //        else
        //            output.AppendFormat(",SignalMinusBackground ({0})", j + 1);
        //    }
        //    if (addXYPairs)
        //    {
        //        for (int j = 0; j < objCount; j++)
        //        {
        //            output.AppendFormat(",X ({0}),Y ({0})", j + 1);
        //        }
        //    }

        //    output.AppendLine();

        //    string isBadTimeString = null;
        //    if (absoluteTimeError > 5) isBadTimeString = "??:??:??.???";
        //    string timeFormat = "HH:mm:ss.fff";

        //    for (int i = 0; i < count; i++)
        //    {
        //        uint frameNo = m_AllReadings[0][i].CurrFrameNo;
        //        DateTime currFrameTime = m_Header.GetTimeForFrame(frameNo);
        //        //FirstFrameTime.AddSeconds((frameNo - m_Header.MinFrame)/m_Header.FramesPerSecond);

        //        string timeStr;
        //        if (isBadTimeString != null)
        //        {
        //            if (currFrameTime == DateTime.MaxValue)
        //                timeStr = "";
        //            else if (frameNo == m_Header.FirstTimedFrameNo)
        //                timeStr = m_Header.FirstTimedFrameTime.ToString(timeFormat);
        //            else if (frameNo == m_Header.LastTimedFrameNo)
        //                timeStr = m_Header.SecondTimedFrameTime.ToString(timeFormat);
        //            else
        //                timeStr = isBadTimeString;
        //        }
        //        else
        //        {
        //            if (currFrameTime == DateTime.MaxValue)
        //                timeStr = "";
        //            else
        //                timeStr = currFrameTime.ToString(timeFormat);
        //        }

        //        output.AppendFormat("{0},{1}", frameNo, timeStr);

        //        for (int j = 0; j < objCount; j++)
        //        {
        //            LCMeasurement reading = m_AllReadings[j][i];
        //            if (reading.IsSuccessfulReading)
        //            {
        //                if (!onlyExportSignalMunusBg)
        //                    output.AppendFormat(",{0},{1}",
        //                                        reading.TotalReading.ToString(CultureInfo.InvariantCulture),
        //                                        reading.TotalBackground.ToString(CultureInfo.InvariantCulture));
        //                else
        //                    output.AppendFormat(",{0}", reading.TotalReading.ToString(CultureInfo.InvariantCulture));
        //            }
        //            else
        //                output.Append(",,");
        //        }


        //        if (addXYPairs)
        //        {
        //            for (int j = 0; j < objCount; j++)
        //            {
        //                LCMeasurement reading = m_AllReadings[j][i];
        //                output.AppendFormat(",{0},{1}", reading.X0.ToString("0.0"), reading.Y0.ToString("0.0"));
        //            }
        //        }

        //        output.AppendLine();
        //    }            
        //}


        internal class BinnedValue
        {
            public int TotalSum;
            public double AdjustedValue;
            public int ReadingIndexFrom;
            public int ReadingIndexTo;
            public int BinNo;
        	public bool IsSuccessfulReading;
        }

        internal class LightCurveContext
        {
            // NOTE: This is used in the SDK
            //private TangraLightCurveImpl m_LightCurveImpl;

            internal LightCurveContext(LCFile lcFile /*, TangraLightCurveImpl lightCurveImpl*/)
            {
                if (lcFile.Header.MeasuredFrames > 0)
                {
                    for (int i = 0; i < lcFile.Header.ObjectCount; i++)
                    {
                        m_ReProcessApertures[i] = lcFile.Header.MeasurementApertures[i];
                        m_ReProcessFitAreas[i] = 2 * (lcFile.Header.PsfFitMatrixSizes[i]/ 2) + 1;
                    }
                }

            	for (int i = 0; i < 4; i++)
            		m_ObjectTitles[i] = string.Format("Object {0}", i);

                //m_LightCurveImpl = lightCurveImpl;
            }

            internal enum FilterType
            {
                NoFilter,
                LowPass,
                LowPassDifference
            }

            internal enum NormalisationMethod
            {
                Average4Frame,
                Average16Frame,
                LinearFit,
                FrameByFrame
            }

            internal enum BackgroundComputationMethod
            {
                FromPSFFit,
                FromBackgroundDistribution
            }

            private bool m_Dirty = false;
            private bool m_FirstZoomedFrameChanged = false;
            private bool m_RequiresFullReprocessing = false;
            private ProcessingType m_ProcessingType = ProcessingType.SignalMinusBackground;
            private int m_Binning = 0;
            private int m_Normalisation = -1;
            private FilterType m_Filter = FilterType.NoFilter;
            private NormalisationMethod m_NormMethod = NormalisationMethod.LinearFit;
            private float[] m_ReProcessApertures = new float[4];
            private int[] m_ReProcessFitAreas = new int[4];

            public uint m_SelectedFrameNo = 0;

            public uint SelectedFrameNo
            {
                get { return m_SelectedFrameNo; }
                set
                {
                    if (m_SelectedFrameNo != value)
                    {
                        m_SelectedFrameNo = value;

                        //m_LightCurveImpl.LightCurveContextImpl.SelectedFrameNo = (int)m_SelectedFrameNo;
                        //m_LightCurveImpl.FireSelectedFrameChanged();                    
                    }
                }
            }

        	public int BitPix;
        	public uint MaxPixelValue;
            public IDisplayBitmapConverter DisplayBitmapConverter;

			public bool CustomBinning { get; set; }

        	public string[] m_ObjectTitles = new string[4];
        	public string[] m_ChartTitleLines;

            #region Backed-up settings so they can be restored when CancelChanges() is requested 

            private bool m_bu_Dirty = false;
            private bool m_bu_RequiresFullReprocessing = false;
            private ProcessingType m_bu_ProcessingType = ProcessingType.SignalMinusBackground;
            private int m_bu_Binning = 0;
            private int m_bu_Normalisation = -1;
            private FilterType m_bu_Filter = FilterType.NoFilter;
            private NormalisationMethod mbu__NormMethod = NormalisationMethod.LinearFit;
            private float[] m_bu_ReProcessApertures = new float[4];
            private int[] m_bu_ReProcessFitAreas = new int[4];
            public uint bu_SelectedFrameNo = 0;
			public string[] m_bu_ObjectTitles = new string[4];
			public string[] m_bu_ChartTitleLines;

            internal void PrepareForCancelling()
            {
                m_bu_Dirty = m_Dirty;
                m_bu_RequiresFullReprocessing = m_RequiresFullReprocessing;
                m_bu_ProcessingType = m_ProcessingType;
                m_bu_Binning = m_Binning;
                m_bu_Normalisation = m_Normalisation;
                m_bu_Filter = m_Filter;
                mbu__NormMethod = m_NormMethod;
                bu_SelectedFrameNo = SelectedFrameNo;

                for (int i = 0; i < 4; i++)
                    m_bu_ReProcessApertures[i] = m_ReProcessApertures[i];

                for (int i = 0; i < 4; i++)
                    m_bu_ReProcessFitAreas[i] = m_ReProcessFitAreas[i];

				for (int i = 0; i < 4; i++)
					m_bu_ObjectTitles[i] = m_ObjectTitles[i];

            	m_bu_ChartTitleLines = m_ChartTitleLines;
            }

            internal void CancelChanges()
            {
                m_Dirty = m_bu_Dirty;
                m_RequiresFullReprocessing = m_bu_RequiresFullReprocessing;
                m_ProcessingType = m_bu_ProcessingType;
                m_Binning = m_bu_Binning;
                m_Normalisation = m_bu_Normalisation;
                m_Filter = m_bu_Filter;
                m_NormMethod = mbu__NormMethod;
                SelectedFrameNo = bu_SelectedFrameNo;

                for (int i = 0; i < 4; i++)
                    m_ReProcessApertures[i] = m_bu_ReProcessApertures[i];

                for (int i = 0; i < 4; i++)
                    m_ReProcessFitAreas[i] = m_bu_ReProcessFitAreas[i];

				for (int i = 0; i < 4; i++)
					m_ObjectTitles[i] = m_bu_ObjectTitles[i];

               m_ChartTitleLines = m_bu_ChartTitleLines;
            }

            #endregion

            public ProcessingType ProcessingType
            {
                get { return m_ProcessingType; }
                set
                {
                    if (m_ProcessingType != value)
                    {
                        m_ProcessingType = value;
                        m_Dirty = true;
                    }
                }
            }

            public int Binning
            {
                get { return m_Binning; }
                set
                {
                    if (m_Binning != value)
                    {
                        m_Binning = value;
                        m_Dirty = true;
                    }
                }
            }

            public int Normalisation
            {
                get { return m_Normalisation; }
                set
                {
                    if (m_Normalisation != value)
                    {
                        m_Normalisation = value;
                        m_Dirty = true;
                    }
                }
            }

            public FilterType Filter
            {
                get { return m_Filter; }
                set
                {
                    if (m_Filter != value)
                    {
                        m_Filter = value;
                        m_Dirty = true;
                        m_RequiresFullReprocessing = true;
                    }
                }
            }

            public NormalisationMethod NormMethod
            {
                get { return m_NormMethod; }
                set
                {
                    if (m_NormMethod != value)
                    {
                        m_NormMethod = value;
                        // If the normalization is specified, changing the method will make the data durty
                        m_Dirty = m_Normalisation > -1;
                    }
                }
            }

            private TangraConfig.BackgroundMethod m_BackgroundMethod = TangraConfig.BackgroundMethod.BackgroundMode;

            public TangraConfig.BackgroundMethod BackgroundMethod
            {
                get { return m_BackgroundMethod; }
                set
                {
                    if (m_BackgroundMethod != value)
                    {
                        m_BackgroundMethod = value;
                        m_Dirty = true;

                        m_RequiresFullReprocessing = true;
                    }
                }
            }

            private TangraConfig.PhotometryReductionMethod m_SignalMethod = TangraConfig.PhotometryReductionMethod.AperturePhotometry;

            public TangraConfig.PhotometryReductionMethod SignalMethod
            {
                get { return m_SignalMethod; }
                set
                {
                    if (m_SignalMethod != value)
                    {
                        m_SignalMethod = value;
                        m_Dirty = true;

                        m_RequiresFullReprocessing = true;
                    }
                }
            }

        	private byte[] m_DecodingGammaMatrix = new byte[256];
			public byte[] DecodingGammaMatrix
			{
				get { return m_DecodingGammaMatrix; }
			}

            private double m_EncodingGamma = 1.0;
            public double EncodingGamma
            {
                get { return m_EncodingGamma; }
                set
                {
                    if (m_EncodingGamma != value)
                    {
                        m_EncodingGamma = value;

						double decodingGamma = 1 / m_EncodingGamma;

						for (int i = 0; i < 256; i++)
							m_DecodingGammaMatrix[i] = (byte)Math.Max(0, Math.Min(255, Math.Round(256 * Math.Pow(i / 256.0, decodingGamma))));

                        m_Dirty = true;

                        m_RequiresFullReprocessing = true;
                    }
                }
            }

        	public bool UseClipping;
        	public bool UseStretching;
			public bool UseBrightnessContrast;
			public byte FromByte;
			public byte ToByte;
			public int Brightness;
			public int Contrast;

        	private CompositeFramePreProcessor m_FramePreProcessor = null;

			internal void InitFrameBytePreProcessors()
			{
				m_FramePreProcessor = new CompositeFramePreProcessor();

				if (UseBrightnessContrast)
				{
                    throw new NotImplementedException();
					//IFramePreProcessor bytePreProcessor = new FrameByteBrightnessContrast(Brightness, Contrast, true, BitPix);
					//m_FramePreProcessor.AddPreProcessor(bytePreProcessor);
				}
				else if (UseStretching)
				{
                    throw new NotImplementedException();
					//IFramePreProcessor bytePreProcessor = new FrameByteStretcher(FromByte, ToByte, true, BitPix);
					//m_FramePreProcessor.AddPreProcessor(bytePreProcessor);
				}
				else if (UseClipping)
				{
                    throw new NotImplementedException();
					//IFramePreProcessor bytePreProcessor = new FrameByteClipper(FromByte, ToByte, true, BitPix);
					//m_FramePreProcessor.AddPreProcessor(bytePreProcessor);
				}				
			}

            private TangraConfig.PsfFittingMethod m_PsfFittingMethod = TangraConfig.PsfFittingMethod.DirectNonLinearFit;

            public TangraConfig.PsfFittingMethod PsfFittingMethod
            {
                get { return m_PsfFittingMethod; }
                set
                {
                    if (m_PsfFittingMethod != value)
                    {
                        m_PsfFittingMethod = value;
                        m_Dirty = true;

                        m_RequiresFullReprocessing = true;
                    }
                }
            }

            private float m_ManualAverageFWHM = float.NaN;

            public float ManualAverageFWHM
            {
                get { return m_ManualAverageFWHM; }
                set
                {
                    if (m_ManualAverageFWHM != value)
                    {
                        m_ManualAverageFWHM = value;
                        m_Dirty = true;

                        m_RequiresFullReprocessing = true;
                    }
                }
            }

            public float[] ReProcessApertures
            {
                get { return m_ReProcessApertures; }
            }

            public int[] ReProcessFitAreas
            {
                get { return m_ReProcessFitAreas; }
            }


			public string[] ObjectTitles
			{
				get { return m_ObjectTitles; }
			}

        	public string[] ChartTitleLines
        	{
				get { return m_ChartTitleLines; }
                set { m_ChartTitleLines = value; }
        	}

            public bool Dirty
            {
                get { return m_Dirty; }
            }

            public bool RequiresFullReprocessing
            {
                get { return m_RequiresFullReprocessing; }
            }
            
            public bool FirstZoomedFrameChanged
            {
                get { return m_FirstZoomedFrameChanged; }
            }

            public void MarkClean()
            {
                m_Dirty = false;
                m_RequiresFullReprocessing = false;
                m_FirstZoomedFrameChanged = false;
            }

            public void MarkDirtyNoFullReprocessing()
            {
                m_Dirty = true;
            }

            public void MarkFirstZoomedFrameChanged()
            {
                m_FirstZoomedFrameChanged = true;
            }

            public void MarkDirtyWithFullReprocessing()
            {
                m_Dirty = true;
                m_RequiresFullReprocessing = true;
            }

            public uint[,] ApplyPreProcessing(uint[,] sourcePixels)
            {
                if (this.EncodingGamma == 1.0 &&
					this.Filter == FilterType.NoFilter &&
					!this.UseClipping && !this.UseStretching && !this.UseBrightnessContrast)
				{
					return sourcePixels;
				}

				int width = sourcePixels.GetLength(0);
				int height = sourcePixels.GetLength(0);

            	uint[,] newData = new uint[width, height];
            	for (int x = 0; x < width; x++)
            	{
            		for (int y = 0; y < height; y++)
            		{
            			newData[x, y] = sourcePixels[x, y];
            		}
            	}

				// gamma, clipping, stretching, brightness / contrast and digital filters

				if (m_FramePreProcessor != null)
					newData = m_FramePreProcessor.OnPreProcessPixels(newData);

				// Apply a pre-processing filter
				if (Filter != FilterType.NoFilter)
				{
                    if (Filter == FilterType.LowPass)
                    {
                        throw new NotImplementedException();
                        //newData = BitmapFilter.LowPassFilter(newData);
                    }
                    else if (Filter == FilterType.LowPassDifference)
                    {
                        throw new NotImplementedException();
                        //newData = BitmapFilter.LowPassDifferenceFilter(newData);
                    }
				}

				if (this.EncodingGamma != 1.0)
				{

					for (int x = 0; x < width; x++)
					{
						for (int y = 0; y < height; y++)
						{
							newData[x, y] = this.DecodingGammaMatrix[newData[x, y]];
						}
					}
				}

				return newData;
            }
        }
    }
}
