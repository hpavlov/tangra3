using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using Tangra.Model.Helpers;
using Tangra.Model.VideoOperations;
using Tangra.OCR.GpsBoxSprite;
using Tangra.OCR.IotaVtiOsdProcessor;

namespace Tangra.OCR.TimeExtraction
{
    public interface ICalibrationErrorProcessor
    {
        bool ShouldLogErrorImage { get; }
        int ErrorCount { get; }
        void AddErrorImage(string fileName, uint[] pixels, int width, int height);
        void AddErrorText(string error);
    }
    
    public class VtiTimeStampComposer
    {
        internal const float FIELD_DURATION_PAL = 20.00f;
        internal const float FIELD_DURATION_NTSC = 16.68f;
        internal const float FRAME_DURATION_PAL = 40.00f;
        internal const float FRAME_DURATION_NTSC = 33.36f;

        private int m_FrameStep = -1;
        private bool m_EvenBeforeOdd;
        private bool m_DuplicatedFields;
        private VideoFormat? m_VideoFormat;
        internal OcrTimeStampCorrector m_Corrector = new OcrTimeStampCorrector();
        private IVideoController m_VideoController;
        private int m_IntegratedAAVFrames;
        private ICalibrationErrorProcessor m_CalibrationErrorProcessor;
        private Func<uint[]> m_GetCurrImageQuery;

        internal VtiTimeStampComposer(VideoFormat? format, int integratedAAVFrames, bool evenBeforeOdd, bool duplicatedFields, IVideoController videoController, ICalibrationErrorProcessor calibrationErrorProcessor, Func<uint[]> getCurrImageQuery)
        {
            m_VideoFormat = format;
            m_IntegratedAAVFrames = integratedAAVFrames;
            m_VideoController = videoController;
            m_CalibrationErrorProcessor = calibrationErrorProcessor;
            m_GetCurrImageQuery = getCurrImageQuery;
            m_EvenBeforeOdd = evenBeforeOdd;
            m_DuplicatedFields = duplicatedFields;
            m_Corrector.Reset(m_VideoFormat);
        }

        internal DateTime ExtractDateTime(int frameNo, int frameStep, IVtiTimeStamp oddFieldOSD, IVtiTimeStamp evenFieldOSD, out string failedReason)
        {
            if (m_DuplicatedFields)
                return ExtractDateTimeDuplicateFields(frameNo, frameStep, oddFieldOSD, evenFieldOSD, out failedReason);
            else
                return ExtractDateTimeBothFields(frameNo, frameStep, oddFieldOSD, evenFieldOSD, out failedReason);
        }

        internal DateTime ExtractDateTimeBothFields(int frameNo, int frameStep, IVtiTimeStamp oddFieldOSD, IVtiTimeStamp evenFieldOSD, out string failedReason)
        {
            bool failedValidation = false;
            failedReason = null;

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
                    failedReason = string.Format("PAL field is not 20 ms. It is {0} ms. ", fieldDuration);
                }

                if (m_VideoFormat == VideoFormat.NTSC &&
                    (Math.Abs(fieldDuration - FIELD_DURATION_NTSC) > 1.0))
                {
                    // NTSC field is not 16.68 ms
                    failedValidation = true;
                    failedReason = string.Format("NTSC field is not 16.68 ms. It is {0} ms. ", fieldDuration);
                }

                int oddEvenFieldDirection = oddFieldTimestamp.Ticks - evenFieldTimestamp.Ticks < 0 ? -1 : 1;
                if (m_Corrector.OddEvenFieldDirectionIsKnown() &&
                    oddEvenFieldDirection != m_Corrector.GetOddEvenFieldDirection())
                {
                    // Field timestamps are wrong have changed order (did they swap)?
                    failedValidation = true;
                    failedReason += "Field timestamps are wrong have changed order (did they swap)? ";
                }

                if (Math.Max(m_Corrector.m_PrevOddTicks, m_Corrector.m_PrevEvenTicks) > Math.Min(oddFieldTimestamp.Ticks, evenFieldTimestamp.Ticks))
                {
                    failedValidation = true;
                    failedReason += "Field timestamps are earlier than previous timestamps. ";
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
                        m_CalibrationErrorProcessor.AddErrorImage(string.Format("ocrerr_{0}.bmp", frameNo), copy.ToArray(), 0, 0);
                        m_CalibrationErrorProcessor.AddErrorText(errorText);
                    }

                    if (m_VideoController != null)
                        m_VideoController.RegisterOcrError();

                    m_Corrector.RegisterUnsuccessfulTimestamp(frameNo, fieldDuration, frameStep, null);
                }
                else
                    m_Corrector.RegisterSuccessfulTimestamp(frameNo, oddFieldOSD, evenFieldOSD, oddFieldTimestamp, evenFieldTimestamp);

                if (failedValidation) return DateTime.MinValue;
                failedReason = null;

                if (oddFieldOSD.ContainsFrameNumbers)
                {
                    if (oddFieldOSD.FrameNumber == evenFieldOSD.FrameNumber - 1)
                        return oddFieldTimestamp;
                    else
                        return evenFieldTimestamp;
                }
                else
                {
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

        internal DateTime ExtractDateTimeDuplicateFields(int frameNo, int frameStep, IVtiTimeStamp oddFieldOSD, IVtiTimeStamp evenFieldOSD, out string failedReason)
        {
            bool failedValidation = false;
            failedReason = null;

            if (oddFieldOSD == null || evenFieldOSD == null)
                return DateTime.MinValue;

            if (frameStep != m_FrameStep)
            {
                m_FrameStep = frameStep;
                m_Corrector.Reset(m_VideoFormat);
            }

            try
            {
                DateTime frameTimestamp1 = oddFieldOSD.ContainsDate
                    ? new DateTime(oddFieldOSD.Year, oddFieldOSD.Month, oddFieldOSD.Day, oddFieldOSD.Hours, oddFieldOSD.Minutes, oddFieldOSD.Seconds).AddMilliseconds(Math.Min(10000, oddFieldOSD.Milliseconds10) / 10.0f)
                    : new DateTime(1, 1, 1, oddFieldOSD.Hours, oddFieldOSD.Minutes, oddFieldOSD.Seconds).AddMilliseconds(Math.Min(10000, oddFieldOSD.Milliseconds10) / 10.0f);

                DateTime frameTimestamp2 = evenFieldOSD.ContainsDate
                    ? new DateTime(evenFieldOSD.Year, evenFieldOSD.Month, evenFieldOSD.Day, evenFieldOSD.Hours, evenFieldOSD.Minutes, evenFieldOSD.Seconds).AddMilliseconds(Math.Min(10000, evenFieldOSD.Milliseconds10) / 10.0f)
                    : new DateTime(1, 1, 1, evenFieldOSD.Hours, evenFieldOSD.Minutes, evenFieldOSD.Seconds).AddMilliseconds(Math.Min(10000, evenFieldOSD.Milliseconds10) / 10.0f);

                double equalityCheck = Math.Abs(new TimeSpan(frameTimestamp1.Ticks - frameTimestamp2.Ticks).TotalMilliseconds);

                if (Math.Abs(equalityCheck) > 1.0)
                {
                    // The OCR of the two identical fields has returned different result. This could happen
                    failedValidation = true;
                    failedReason = "The timestamps of identical video fields are not the same. ";
                }

                double frameDuration = 0;
                if (m_Corrector.m_PrevOddTicks != -1)
                {
                    if (Math.Max(m_Corrector.m_PrevOddTicks, m_Corrector.m_PrevEvenTicks) > Math.Min(frameTimestamp1.Ticks, frameTimestamp2.Ticks))
                    {
                        failedValidation = true;
                        failedReason += "Field timestamps are earlier than previous timestamps. ";
                    }

                    frameDuration = Math.Abs(new TimeSpan(m_Corrector.m_PrevOddTicks - frameTimestamp1.Ticks).TotalMilliseconds);

                    if (m_VideoFormat == VideoFormat.PAL &&
                        (Math.Abs(frameDuration - FRAME_DURATION_PAL) > 1.0))
                    {
                        // PAL field is not 20 ms
                        failedValidation = true;
                        failedReason = string.Format("PAL field is not 40 ms. It is {0} ms. ", frameDuration);
                    }

                    if (m_VideoFormat == VideoFormat.NTSC &&
                        (Math.Abs(frameDuration - FRAME_DURATION_NTSC) > 1.0))
                    {
                        // NTSC field is not 16.68 ms
                        failedValidation = true;
                        failedReason = string.Format("NTSC field is not 33.36 ms. It is {0} ms. ", frameDuration);
                    }

                    if (failedValidation)
                    {
                        string correctionInfo;
                        failedValidation = !m_Corrector.TryToCorrectDuplicateField(frameNo, frameStep, oddFieldOSD, evenFieldOSD, ref frameTimestamp1, out correctionInfo);
                        frameTimestamp2 = frameTimestamp1;
                        failedReason += ". " + correctionInfo;
                    }                    
                }
                else
                { 
                    // If this is the first time we extract, make sure duration is correct, before registering it as successful
                    var fieldDuration = (((GpxBoxSpriteVtiTimeStamp)oddFieldOSD).Milliseconds10First - ((GpxBoxSpriteVtiTimeStamp)oddFieldOSD).Milliseconds10Second) / 10.0;
                    if (fieldDuration < 0) fieldDuration += 1000;

                    if (m_VideoFormat == VideoFormat.PAL &&
                        (Math.Abs(fieldDuration - FIELD_DURATION_PAL) > 1.0))
                    {
                        // PAL field is not 20.00 ms
                        failedValidation = true;
                        failedReason = string.Format("PAL field of first OCR-ed frame is not 20.00 ms. It is {0} ms. ", frameDuration);
                    }

                    if (m_VideoFormat == VideoFormat.NTSC && 
                        (Math.Abs(fieldDuration - FIELD_DURATION_NTSC) > 1.0))
                    {
                        // NTSC field is not 16.68 ms
                        failedValidation = true;
                        failedReason = string.Format("NTSC field of first OCR-ed frame is not 33.36 ms. It is {0} ms. ", frameDuration);
                    }
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
                        m_CalibrationErrorProcessor.AddErrorImage(string.Format("ocrerr_{0}.bmp", frameNo), copy.ToArray(), 0, 0);
                        m_CalibrationErrorProcessor.AddErrorText(errorText);
                    }

                    if (m_VideoController != null)
                        m_VideoController.RegisterOcrError();

                    m_Corrector.RegisterUnsuccessfulTimestamp(frameNo, frameDuration / 2.0, frameStep, null);
                }
                else
                    m_Corrector.RegisterSuccessfulTimestamp(frameNo, oddFieldOSD, evenFieldOSD, frameTimestamp1, frameTimestamp1);

                if (failedValidation) return DateTime.MinValue;
                failedReason = null;

                return frameTimestamp1;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.GetFullStackTrace());

                if (m_VideoController != null)
                    m_VideoController.RegisterOcrError();

                return DateTime.MinValue;
            }
        }

        internal DateTime ExtractAAVDateTime(int frameNo, int frameStep, IVtiTimeStamp oddFieldOSD, IVtiTimeStamp evenFieldOSD, out string failedReason)
        {
            bool failedValidation = false;
            failedReason = null;

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
                double fieldDuration;
                if (m_VideoFormat != null)
                {
                    if (m_VideoFormat == VideoFormat.PAL)
                    {
                        fieldDuration = (integrationPeriodDuration + FIELD_DURATION_PAL)/integratedAavFields;
                        if ((Math.Abs(fieldDuration - FIELD_DURATION_PAL) > 1.0))
                        {
                            // PAL field is not 20 ms
                            failedValidation = true;
                            failedReason = string.Format("PAL field is not 20 ms. It is {0} ms", fieldDuration);
                        }
                    }
                    else if (m_VideoFormat == VideoFormat.NTSC)
                    {
                        fieldDuration = (integrationPeriodDuration + FIELD_DURATION_NTSC)/integratedAavFields;
                        if (Math.Abs(fieldDuration - FIELD_DURATION_NTSC) > 1.0)
                        {
                            // NTSC field is not 16.68 ms
                            failedValidation = true;
                            failedReason = string.Format("NTSC field is not 16.68 ms. It is {0} ms", fieldDuration);
                        }
                    }
                    else
                        throw new ApplicationException("AAV videos must be PAL or NTSC.");
                }
                else
                    throw new ApplicationException("Cannot identify the native format of an AAV video.");

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
                        m_CalibrationErrorProcessor.AddErrorImage(string.Format("ocrerr_{0}.bmp", frameNo), copy.ToArray(), 0, 0);
                        m_CalibrationErrorProcessor.AddErrorText(errorText);
                    }

                    if (m_VideoController != null)
                        m_VideoController.RegisterOcrError();

                    m_Corrector.RegisterUnsuccessfulTimestamp(frameNo, fieldDuration, frameStep, integratedAavFields);
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
