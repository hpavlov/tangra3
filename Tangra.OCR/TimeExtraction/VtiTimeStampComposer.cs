using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Tangra.Model.Helpers;
using Tangra.Model.VideoOperations;
using Tangra.OCR.IotaVtiOsdProcessor;

namespace Tangra.OCR.TimeExtraction
{
    public interface ICalibrationErrorProcessor
    {
        int ErrorCount { get; }
        void AddErrorImage(string fileName, uint[] pixels);
        void AddErrorText(string error);
    }

    public class VtiTimeStampComposer
    {
        internal const float FIELD_DURATION_PAL = 20.00f;
        internal const float FIELD_DURATION_NTSC = 16.68f;

        private int m_FrameStep = -1;
        private bool m_EvenBeforeOdd;
        private VideoFormat? m_VideoFormat;
        internal OcrTimeStampCorrector m_Corrector = new OcrTimeStampCorrector();
        private IVideoController m_VideoController;
        private int m_IntegratedAAVFrames;
        private ICalibrationErrorProcessor m_CalibrationErrorProcessor;
        private Func<uint[]> m_GetCurrImageQuery;

        internal VtiTimeStampComposer(VideoFormat? format, int integratedAAVFrames, bool evenBeforeOdd, IVideoController videoController, ICalibrationErrorProcessor calibrationErrorProcessor, Func<uint[]> getCurrImageQuery)
        {
            m_VideoFormat = format;
            m_IntegratedAAVFrames = integratedAAVFrames;
            m_VideoController = videoController;
            m_CalibrationErrorProcessor = calibrationErrorProcessor;
            m_GetCurrImageQuery = getCurrImageQuery;
            m_EvenBeforeOdd = evenBeforeOdd;
            m_Corrector.Reset(m_VideoFormat);
        }

        internal DateTime ExtractDateTime(int frameNo, int frameStep, IVtiTimeStamp oddFieldOSD, IVtiTimeStamp evenFieldOSD)
        {
            bool failedValidation = false;
            string failedReason = null;

            if (oddFieldOSD == null || evenFieldOSD == null)
                return DateTime.MinValue;

            if (frameStep != m_FrameStep)
            {
                m_FrameStep = frameStep;
                m_Corrector.Reset(m_VideoFormat);
            }

            if (oddFieldOSD.ContainsFrameNumbers)
            {
                if (oddFieldOSD.FrameNumber != evenFieldOSD.FrameNumber - 1 &&
                    oddFieldOSD.FrameNumber != evenFieldOSD.FrameNumber + 1)
                {
                    // Video fields are not consequtive
                    failedValidation = true;
                    failedReason = "Video fields are not consequtive";
                }
            }
            

            try
            {
                DateTime oddFieldTimestamp = oddFieldOSD.ContainsDate
                    ? new DateTime(oddFieldOSD.Year, oddFieldOSD.Month, oddFieldOSD.Day, oddFieldOSD.Hours, oddFieldOSD.Minutes, oddFieldOSD.Seconds).AddMilliseconds(Math.Min(10000, oddFieldOSD.Milliseconds10) / 10.0f)
                    : new DateTime(1, 1, 1, oddFieldOSD.Hours, oddFieldOSD.Minutes, oddFieldOSD.Seconds).AddMilliseconds(Math.Min(10000, oddFieldOSD.Milliseconds10) / 10.0f);

                DateTime evenFieldTimestamp = evenFieldOSD.ContainsDate 
                    ?  new DateTime(evenFieldOSD.Year, evenFieldOSD.Month, evenFieldOSD.Day, evenFieldOSD.Hours, evenFieldOSD.Minutes, evenFieldOSD.Seconds).AddMilliseconds(Math.Min(10000, evenFieldOSD.Milliseconds10) / 10.0f)
                    :  new DateTime(1, 1, 1, evenFieldOSD.Hours, evenFieldOSD.Minutes, evenFieldOSD.Seconds).AddMilliseconds(Math.Min(10000, evenFieldOSD.Milliseconds10) / 10.0f);

                double fieldDuration = Math.Abs(new TimeSpan(oddFieldTimestamp.Ticks - evenFieldTimestamp.Ticks).TotalMilliseconds);

                if (m_VideoFormat == VideoFormat.PAL &&
                    (Math.Abs(fieldDuration - FIELD_DURATION_PAL) > 1.0))
                {
                    // PAL field is not 20 ms
                    failedValidation = true;
                    failedReason = string.Format("PAL field is not 20 ms. It is {0} ms", fieldDuration);
                }

                if (m_VideoFormat == VideoFormat.NTSC &&
                    (Math.Abs(fieldDuration - FIELD_DURATION_NTSC) > 1.0))
                {
                    // NTSC field is not 16.68 ms
                    failedValidation = true;
                    failedReason = string.Format("NTSC field is not 16.68 ms. It is {0} ms", fieldDuration);
                }

                int oddEvenFieldDirection = oddFieldTimestamp.Ticks - evenFieldTimestamp.Ticks < 0 ? -1 : 1;
                if (m_Corrector.OddEvenFieldDirectionIsKnown() &&
                    oddEvenFieldDirection != m_Corrector.GetOddEvenFieldDirection())
                {
                    // Field timestamps are wrong have changed order (did they swap)?
                    failedValidation = true;
                    failedReason = "Field timestamps are wrong have changed order (did they swap)?";
                }

                if (failedValidation)
                {
                    string correctionInfo;
                    failedValidation = !m_Corrector.TryToCorrect(frameNo, frameStep, null, oddFieldOSD, evenFieldOSD, m_EvenBeforeOdd, ref oddFieldTimestamp, ref evenFieldTimestamp, out correctionInfo);
                    failedReason += ". " + correctionInfo;
                }

                if (failedValidation)
                {
                    string errorText = string.Format("OCR ERR: FrameNo: {0}, Odd Timestamp: {1}:{2}:{3}.{4} {5}, Even Timestamp: {6}:{7}:{8}.{9} {10}, {11}",
                        frameNo, oddFieldOSD.Hours, oddFieldOSD.Minutes, oddFieldOSD.Seconds, oddFieldOSD.Milliseconds10, oddFieldOSD.FrameNumber,
                        evenFieldOSD.Hours, evenFieldOSD.Minutes, evenFieldOSD.Seconds, evenFieldOSD.Milliseconds10, evenFieldOSD.FrameNumber, failedReason);

                    Trace.WriteLine(errorText);

                    if (m_CalibrationErrorProcessor.ErrorCount < 16)
                    {
                        var copy = new List<uint>();
                        copy.AddRange(m_GetCurrImageQuery());
                        m_CalibrationErrorProcessor.AddErrorImage(string.Format("ocrerr_{0}.bmp", frameNo), copy.ToArray());
                        m_CalibrationErrorProcessor.AddErrorText(errorText);
                    }

                    if (m_VideoController != null)
                        m_VideoController.RegisterOcrError();
                }
                else
                    m_Corrector.RegisterSuccessfulTimestamp(frameNo, oddFieldOSD, evenFieldOSD, oddFieldTimestamp, evenFieldTimestamp);

                if (oddFieldOSD.ContainsFrameNumbers)
                {
                    if (oddFieldOSD.FrameNumber == evenFieldOSD.FrameNumber - 1)
                    {
                        return failedValidation ? DateTime.MinValue : oddFieldTimestamp;
                    }
                    else
                    {
                        return failedValidation ? DateTime.MinValue : evenFieldTimestamp;
                    }
                }
                else
                {
                    if (failedValidation) return DateTime.MinValue;

                    if (m_EvenBeforeOdd)
                        return evenFieldTimestamp;
                    else 
                        return oddFieldTimestamp;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.GetFullStackTrace());

                if (m_VideoController != null)
                    m_VideoController.RegisterOcrError();

                return DateTime.MinValue;
            }
        }

        internal DateTime ExtractAAVDateTime(int frameNo, int frameStep, IVtiTimeStamp oddFieldOSD, IVtiTimeStamp evenFieldOSD)
        {
            bool failedValidation = false;
            string failedReason = null;

            if (oddFieldOSD == null || evenFieldOSD == null)
                return DateTime.MinValue;

            int integratedAavFields = 2 * m_IntegratedAAVFrames;

            if (oddFieldOSD.ContainsFrameNumbers)
            {
                if (oddFieldOSD.FrameNumber != evenFieldOSD.FrameNumber - integratedAavFields + 1 &&
                    oddFieldOSD.FrameNumber != evenFieldOSD.FrameNumber + integratedAavFields - 1)
                {
                    // Video fields are not consequtive
                    failedValidation = true;
                    failedReason = "Integration interval is incomplete";
                }                
            }

            try
            {
                //   | e               e |        |   o              o|
                //   |.e.o|.e.o|.e.o|.e.o|        |.e.o|.e.o|.e.o|.e.o|

                DateTime oddFieldTimestamp = new DateTime(1, 1, 1, oddFieldOSD.Hours, oddFieldOSD.Minutes, oddFieldOSD.Seconds).AddMilliseconds(Math.Min(10000, oddFieldOSD.Milliseconds10) / 10.0f);
                DateTime evenFieldTimestamp = new DateTime(1, 1, 1, evenFieldOSD.Hours, evenFieldOSD.Minutes, evenFieldOSD.Seconds).AddMilliseconds(Math.Min(10000, evenFieldOSD.Milliseconds10) / 10.0f);

                double integrationPeriodDuration = Math.Abs(new TimeSpan(oddFieldTimestamp.Ticks - evenFieldTimestamp.Ticks).TotalMilliseconds);

                if (m_VideoFormat != null)
                {
                    if (m_VideoFormat == VideoFormat.PAL)
                    {
                        var calcDuration = (integrationPeriodDuration + FIELD_DURATION_PAL) / integratedAavFields;
                        if ((Math.Abs(calcDuration - FIELD_DURATION_PAL) > 1.0))
                        {
                            // PAL field is not 20 ms
                            failedValidation = true;
                            failedReason = string.Format("PAL field is not 20 ms. It is {0} ms", calcDuration);
                        }
                    }

                    if (m_VideoFormat == VideoFormat.NTSC)
                    {
                        var calcDuration = (integrationPeriodDuration + FIELD_DURATION_NTSC) / integratedAavFields;
                        if (Math.Abs(calcDuration - FIELD_DURATION_NTSC) > 1.0)
                        {
                            // NTSC field is not 16.68 ms
                            failedValidation = true;
                            failedReason = string.Format("NTSC field is not 16.68 ms. It is {0} ms", calcDuration);
                        }
                    }
                }

                if (failedValidation)
                {
                    string correctionInfo;
                    failedValidation = !m_Corrector.TryToCorrect(frameNo, frameStep, integratedAavFields, oddFieldOSD, evenFieldOSD, m_EvenBeforeOdd, ref oddFieldTimestamp, ref evenFieldTimestamp, out correctionInfo);
                    failedReason += ". " + correctionInfo;
                }

                if (failedValidation)
                {
                    string errorText = string.Format("OCR ERR: FrameNo: {0}, Odd Timestamp: {1}:{2}:{3}.{4} {5}, Even Timestamp: {6}:{7}:{8}.{9} {10}, {11}",
                        frameNo, oddFieldOSD.Hours, oddFieldOSD.Minutes, oddFieldOSD.Seconds, oddFieldOSD.Milliseconds10, oddFieldOSD.FrameNumber,
                        evenFieldOSD.Hours, evenFieldOSD.Minutes, evenFieldOSD.Seconds, evenFieldOSD.Milliseconds10, evenFieldOSD.FrameNumber, failedReason);

                    Trace.WriteLine(errorText);

                    if (m_CalibrationErrorProcessor.ErrorCount < 16)
                    {
                        var copy = new List<uint>();
                        copy.AddRange(m_GetCurrImageQuery());
                        m_CalibrationErrorProcessor.AddErrorImage(string.Format("ocrerr_{0}.bmp", frameNo), copy.ToArray());
                        m_CalibrationErrorProcessor.AddErrorText(errorText);
                    }

                    if (m_VideoController != null)
                        m_VideoController.RegisterOcrError();
                }
                else
                    m_Corrector.RegisterSuccessfulTimestamp(frameNo, oddFieldOSD, evenFieldOSD, oddFieldTimestamp, evenFieldTimestamp);

                if (oddFieldOSD.ContainsFrameNumbers)
                {
                    if (oddFieldOSD.FrameNumber == evenFieldOSD.FrameNumber - integratedAavFields + 1)
                    {
                        return failedValidation ? DateTime.MinValue : oddFieldTimestamp;
                    }
                    else
                    {
                        return failedValidation ? DateTime.MinValue : evenFieldTimestamp;
                    }                    
                }
                else
                {
                    if (failedValidation) return DateTime.MinValue;

                    if (m_EvenBeforeOdd)
                        return evenFieldTimestamp;
                    else
                        return oddFieldTimestamp;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.GetFullStackTrace());

                return DateTime.MinValue;
            }
        }
    }
}
