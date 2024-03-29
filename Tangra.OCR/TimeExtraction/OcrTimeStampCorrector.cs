﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Tangra.Model.Config;
using Tangra.OCR.IotaVtiOsdProcessor;
using Tangra.OCR.TimeExtraction;

namespace Tangra.OCR.TimeExtraction
{
	internal class OcrTimeStampCorrector
	{
		private int m_PrevFrameNo;
		internal long m_PrevOddTicks;
		internal long m_PrevEvenTicks;
		private long m_PrevOddFieldNo;
		private long m_PrevEvenFieldNo;
        private IVtiTimeStamp m_PrevOddFieldOSD;
        private IVtiTimeStamp m_PrevEvenFieldOSD;
		private VideoFormat? m_VideoFormat;

		public void Reset(VideoFormat? videoFormat)
		{
			m_PrevFrameNo = -1;
			m_PrevOddTicks = -1;
			m_PrevEvenTicks = -1;
			m_PrevOddFieldNo = -1;
			m_PrevEvenFieldNo = -1;

			m_VideoFormat = videoFormat;
		}

        public bool TryToCorrect(int frameNo, int frameStep, int? aavIntegratedFields, IVtiTimeStamp oddFieldOSD, IVtiTimeStamp evenFieldOSD, bool evenBeforeOdd, ref DateTime oddFieldTimestamp, ref DateTime evenFieldTimestamp, out string correctionDebugInfo)
		{
            if (m_PrevFrameNo == -1 || m_PrevOddTicks == -1 || m_PrevEvenTicks == -1)
            {
                correctionDebugInfo = "Cannot correct. m_PrevFrameNo == -1 || m_PrevOddTicks == -1 || m_PrevEvenTicks == -1";
                return false;
            }

            if (oddFieldOSD.ContainsDate)
            {
                correctionDebugInfo = string.Format("IOTA-VTI Correction Attempt for Frame {0}. {1:D4}-{2:D2}-{3:D2} {4:D2}:{5:D2}:{6:D2}.{7:D4} ({8}) - {9:D4}-{10:D2}-{11:D2} {12:D2}:{13:D2}:{14:D2}.{15:D4} ({16}). FrameStep: {17}. Previous: {18:D4}-{19:D2}-{20:D2} {21:D2}:{22:D2}:{23:D2}.{24:D4} ({25}) - {26:D4}-{27:D2}-{28:D2} {29:D2}:{30:D2}:{31:D2}.{32:D4} ({33})",
                        frameNo,
                        oddFieldOSD.Year, oddFieldOSD.Month, oddFieldOSD.Day, oddFieldOSD.Hours, oddFieldOSD.Minutes, oddFieldOSD.Seconds, oddFieldOSD.Milliseconds10, oddFieldOSD.FrameNumber,
                        evenFieldOSD.Year, evenFieldOSD.Month, evenFieldOSD.Day, evenFieldOSD.Hours, evenFieldOSD.Minutes, evenFieldOSD.Seconds, evenFieldOSD.Milliseconds10, evenFieldOSD.FrameNumber,
                        frameStep,
                        m_PrevOddFieldOSD.Year, m_PrevOddFieldOSD.Month, m_PrevOddFieldOSD.Day, m_PrevOddFieldOSD.Hours, m_PrevOddFieldOSD.Minutes, m_PrevOddFieldOSD.Seconds, m_PrevOddFieldOSD.Milliseconds10, m_PrevOddFieldNo,
                        m_PrevEvenFieldOSD.Year, m_PrevEvenFieldOSD.Month, m_PrevEvenFieldOSD.Day, m_PrevEvenFieldOSD.Hours, m_PrevEvenFieldOSD.Minutes, m_PrevEvenFieldOSD.Seconds, m_PrevEvenFieldOSD.Milliseconds10, m_PrevEvenFieldNo);
            }
            else
            {
                correctionDebugInfo = string.Format("IOTA-VTI Correction Attempt for Frame {0}. {1:D2}:{2:D2}:{3:D2}.{4:D4} ({5}) - {6:D2}:{7:D2}:{8:D2}.{9:D4} ({10}). FrameStep: {11}. Previous: {12:D2}:{13:D2}:{14:D2}.{15:D4} ({16}) - {17:D2}:{18:D2}:{19:D2}.{20:D4} ({21})",
                        frameNo,
                        oddFieldOSD.Hours, oddFieldOSD.Minutes, oddFieldOSD.Seconds, oddFieldOSD.Milliseconds10, oddFieldOSD.FrameNumber,
                        evenFieldOSD.Hours, evenFieldOSD.Minutes, evenFieldOSD.Seconds, evenFieldOSD.Milliseconds10, evenFieldOSD.FrameNumber,
                        frameStep,
                        m_PrevOddFieldOSD.Hours, m_PrevOddFieldOSD.Minutes, m_PrevOddFieldOSD.Seconds, m_PrevOddFieldOSD.Milliseconds10, m_PrevOddFieldNo,
                        m_PrevEvenFieldOSD.Hours, m_PrevEvenFieldOSD.Minutes, m_PrevEvenFieldOSD.Seconds, m_PrevEvenFieldOSD.Milliseconds10, m_PrevEvenFieldNo);                
            }

            float knownFrameDuration = m_VideoFormat == VideoFormat.PAL
                        ? 2 * VtiTimeStampComposer.FIELD_DURATION_PAL
                        : 2 * VtiTimeStampComposer.FIELD_DURATION_NTSC;

			double fieldDuration;
            bool returnBlank;
            bool isDuplicateFrame = false;

			if (!evenBeforeOdd)
			{
				DateTime oddTimestampToCheck = oddFieldTimestamp;

                fieldDuration = Math.Abs(new TimeSpan(oddFieldTimestamp.Ticks - m_PrevEvenTicks).TotalMilliseconds) - (frameStep - 1) * knownFrameDuration;

                if (IsDuplicatedOrDroppedFrames(m_PrevOddFieldOSD, m_PrevEvenFieldOSD, oddFieldOSD, evenFieldOSD, frameStep, knownFrameDuration, aavIntegratedFields, out returnBlank))
                {
                    if (returnBlank)
                    {
                        // The Duplicate/Dropped frame engine decided it is safer to return a blank frame
                        return false;
                    }
                    else
                    {
                        // Duplicate or dropped frame detected. No corrections requied.
                        isDuplicateFrame = true;
                    }
                }
			    else
			    {
                    if (!IsFieldDurationOkay(fieldDuration))
                    {
                        if (!TryCorrectTimestamp(m_PrevEvenTicks, oddFieldTimestamp, oddFieldOSD, frameStep, null, aavIntegratedFields != null))
                        {
                            Trace.WriteLine(correctionDebugInfo);
                            Trace.WriteLine("IOTA-VTI Correction Failed: Cannot correct field duration PrevEven -> CurrOdd.");
                            return false;
                        }
                        else
                        {
                            if (oddFieldOSD.ContainsDate)
                                oddTimestampToCheck = new DateTime(oddFieldOSD.Year, oddFieldOSD.Month, oddFieldOSD.Day, oddFieldOSD.Hours, oddFieldOSD.Minutes, oddFieldOSD.Seconds).AddMilliseconds(Math.Min(10000, oddFieldOSD.Milliseconds10) / 10.0f); 
                            else
                                oddTimestampToCheck = new DateTime(1, 1, 1, oddFieldOSD.Hours, oddFieldOSD.Minutes, oddFieldOSD.Seconds).AddMilliseconds(Math.Min(10000, oddFieldOSD.Milliseconds10) / 10.0f); 
                        }
                    }

                    fieldDuration = Math.Abs(new TimeSpan(oddTimestampToCheck.Ticks - evenFieldTimestamp.Ticks).TotalMilliseconds);
                    if (aavIntegratedFields != null) fieldDuration = fieldDuration / (aavIntegratedFields.Value - 1);

                    if (!IsFieldDurationOkay(fieldDuration))
                    {
                        if (!TryCorrectTimestamp(oddTimestampToCheck.Ticks, evenFieldTimestamp, evenFieldOSD, 1, aavIntegratedFields, aavIntegratedFields != null))
                        {
                            Trace.WriteLine(correctionDebugInfo);
                            Trace.WriteLine("IOTA-VTI Correction Failed: Cannot correct field duration CurrOdd -> CurrEven.");
                            return false;
                        }
                    }			        
			    }
			}
			else
			{
				DateTime evenTimestampToCheck = evenFieldTimestamp;

                fieldDuration = Math.Abs(new TimeSpan(evenFieldTimestamp.Ticks - m_PrevOddTicks).TotalMilliseconds) - (frameStep - 1) * knownFrameDuration;

                if (IsDuplicatedOrDroppedFrames(m_PrevEvenFieldOSD, m_PrevOddFieldOSD, evenFieldOSD, oddFieldOSD, frameStep, knownFrameDuration, aavIntegratedFields, out returnBlank))
			    {
                    if (returnBlank)
                    {
                        // The Duplicate/Dropped frame engine decided it is safer to return a blank frame
                        return false;
                    }
                    else
                    {
                        // Duplicate or dropped frame detected. No corrections requied.
                        isDuplicateFrame = true;
                    }
			    }
			    else
			    {
                    if (!IsFieldDurationOkay(fieldDuration))
                    {
                        if (
                            !TryCorrectTimestamp(m_PrevOddTicks, evenFieldTimestamp, evenFieldOSD, frameStep, null,
                                aavIntegratedFields != null))
                        {
                            Trace.WriteLine(correctionDebugInfo);
                            Trace.WriteLine("IOTA-VTI Correction Failed: Cannot correct field duration PrevOdd -> CurrEven.");
                            return false;
                        }
                        else
                        {
                            if (evenFieldOSD.ContainsDate)
                                evenTimestampToCheck = new DateTime(evenFieldOSD.Year, evenFieldOSD.Month, evenFieldOSD.Day, evenFieldOSD.Hours, evenFieldOSD.Minutes, evenFieldOSD.Seconds).AddMilliseconds(Math.Min(10000, evenFieldOSD.Milliseconds10) / 10.0f);
                            else
                                evenTimestampToCheck = new DateTime(1, 1, 1, evenFieldOSD.Hours, evenFieldOSD.Minutes, evenFieldOSD.Seconds).AddMilliseconds(Math.Min(10000, evenFieldOSD.Milliseconds10) / 10.0f);
                        }                            
                    }

                    fieldDuration = Math.Abs(new TimeSpan(evenTimestampToCheck.Ticks - oddFieldTimestamp.Ticks).TotalMilliseconds);
                    if (aavIntegratedFields != null) fieldDuration = fieldDuration / (aavIntegratedFields.Value - 1);

                    if (!IsFieldDurationOkay(fieldDuration))
                    {
                        if (!TryCorrectTimestamp(evenTimestampToCheck.Ticks, oddFieldTimestamp, oddFieldOSD, 1, aavIntegratedFields, aavIntegratedFields != null))
                        {
                            Trace.WriteLine(correctionDebugInfo);
                            Trace.WriteLine("IOTA-VTI Correction Failed: Cannot correct field duration CurrEven -> CurrOdd.");
                            return false;
                        }
                    }
			    }
			}

            if (oddFieldOSD.ContainsFrameNumbers && !isDuplicateFrame)
            {
                var aavFrameNoCorr = aavIntegratedFields.HasValue ? (aavIntegratedFields.Value - 2) : 0;

                if (!evenBeforeOdd)
                {
                    if (m_PrevEvenFieldNo + 1 != oddFieldOSD.FrameNumber)
                    {
                        oddFieldOSD.CorrectFrameNumber((int)(m_PrevEvenFieldNo + 1));
                    }

                    if (oddFieldOSD.FrameNumber + 1 + aavFrameNoCorr != evenFieldOSD.FrameNumber)
                    {
                        evenFieldOSD.CorrectFrameNumber((int)(oddFieldOSD.FrameNumber + 1 + aavFrameNoCorr));
                    }
                }
                else
                {
                    if (m_PrevOddFieldNo + 1 != evenFieldOSD.FrameNumber)
                    {
                        evenFieldOSD.CorrectFrameNumber((int)(m_PrevOddFieldNo + 1));
                    }

                    if (evenFieldOSD.FrameNumber + 1 + aavFrameNoCorr != oddFieldOSD.FrameNumber)
                    {
                        oddFieldOSD.CorrectFrameNumber((int)(evenFieldOSD.FrameNumber + 1 + aavFrameNoCorr));
                    }
                }                
            }

            if (oddFieldOSD.ContainsDate)
            {
                oddFieldTimestamp = new DateTime(oddFieldOSD.Year, oddFieldOSD.Month, oddFieldOSD.Day, oddFieldOSD.Hours, oddFieldOSD.Minutes, oddFieldOSD.Seconds).AddMilliseconds(Math.Min(10000, oddFieldOSD.Milliseconds10) / 10.0f);
                evenFieldTimestamp = new DateTime(evenFieldOSD.Year, evenFieldOSD.Month, evenFieldOSD.Day, evenFieldOSD.Hours, evenFieldOSD.Minutes, evenFieldOSD.Seconds).AddMilliseconds(Math.Min(10000, evenFieldOSD.Milliseconds10) / 10.0f);
            }
            else
            {
                oddFieldTimestamp = new DateTime(1, 1, 1, oddFieldOSD.Hours, oddFieldOSD.Minutes, oddFieldOSD.Seconds).AddMilliseconds(Math.Min(10000, oddFieldOSD.Milliseconds10) / 10.0f);
                evenFieldTimestamp = new DateTime(1, 1, 1, evenFieldOSD.Hours, evenFieldOSD.Minutes, evenFieldOSD.Seconds).AddMilliseconds(Math.Min(10000, evenFieldOSD.Milliseconds10) / 10.0f);                
            }

			return true;
		}

	    public bool TryToCorrectDuplicateField(int frameNo, int frameStep, IVtiTimeStamp oddFieldOSD, IVtiTimeStamp evenFieldOSD, ref DateTime frameTimestamp, out string correctionDebugInfo)
	    {
            if (m_PrevFrameNo == -1 || m_PrevOddTicks == -1 || m_PrevEvenTicks == -1)
            {
                correctionDebugInfo = "Cannot correct. m_PrevFrameNo == -1 || m_PrevOddTicks == -1 || m_PrevEvenTicks == -1";
                return false;
            }

            if (oddFieldOSD.ContainsDate)
            {
                correctionDebugInfo = string.Format("IOTA-VTI Correction Attempt for Frame {0}. {1:D4}-{2:D2}-{3:D2} {4:D2}:{5:D2}:{6:D2}.{7:D4} ({8}) - {9:D4}-{10:D2}-{11:D2} {12:D2}:{13:D2}:{14:D2}.{15:D4} ({16}). FrameStep: {17}. Previous: {18:D4}-{19:D2}-{20:D2} {21:D2}:{22:D2}:{23:D2}.{24:D4} ({25}) - {26:D4}-{27:D2}-{28:D2} {29:D2}:{30:D2}:{31:D2}.{32:D4} ({33})",
                        frameNo,
                        oddFieldOSD.Year, oddFieldOSD.Month, oddFieldOSD.Day, oddFieldOSD.Hours, oddFieldOSD.Minutes, oddFieldOSD.Seconds, oddFieldOSD.Milliseconds10, oddFieldOSD.FrameNumber,
                        evenFieldOSD.Year, evenFieldOSD.Month, evenFieldOSD.Day, evenFieldOSD.Hours, evenFieldOSD.Minutes, evenFieldOSD.Seconds, evenFieldOSD.Milliseconds10, evenFieldOSD.FrameNumber,
                        frameStep,
                        m_PrevOddFieldOSD.Year, m_PrevOddFieldOSD.Month, m_PrevOddFieldOSD.Day, m_PrevOddFieldOSD.Hours, m_PrevOddFieldOSD.Minutes, m_PrevOddFieldOSD.Seconds, m_PrevOddFieldOSD.Milliseconds10, m_PrevOddFieldNo,
                        m_PrevEvenFieldOSD.Year, m_PrevEvenFieldOSD.Month, m_PrevEvenFieldOSD.Day, m_PrevEvenFieldOSD.Hours, m_PrevEvenFieldOSD.Minutes, m_PrevEvenFieldOSD.Seconds, m_PrevEvenFieldOSD.Milliseconds10, m_PrevEvenFieldNo);
            }
            else
            {
                correctionDebugInfo = string.Format("IOTA-VTI Correction Attempt for Frame {0}. {1:D2}:{2:D2}:{3:D2}.{4:D4} ({5}) - {6:D2}:{7:D2}:{8:D2}.{9:D4} ({10}). FrameStep: {11}. Previous: {12:D2}:{13:D2}:{14:D2}.{15:D4} ({16}) - {17:D2}:{18:D2}:{19:D2}.{20:D4} ({21})",
                        frameNo,
                        oddFieldOSD.Hours, oddFieldOSD.Minutes, oddFieldOSD.Seconds, oddFieldOSD.Milliseconds10, oddFieldOSD.FrameNumber,
                        evenFieldOSD.Hours, evenFieldOSD.Minutes, evenFieldOSD.Seconds, evenFieldOSD.Milliseconds10, evenFieldOSD.FrameNumber,
                        frameStep,
                        m_PrevOddFieldOSD.Hours, m_PrevOddFieldOSD.Minutes, m_PrevOddFieldOSD.Seconds, m_PrevOddFieldOSD.Milliseconds10, m_PrevOddFieldNo,
                        m_PrevEvenFieldOSD.Hours, m_PrevEvenFieldOSD.Minutes, m_PrevEvenFieldOSD.Seconds, m_PrevEvenFieldOSD.Milliseconds10, m_PrevEvenFieldNo);
            }

            float knownFrameDuration = m_VideoFormat == VideoFormat.PAL
                        ? 2 * VtiTimeStampComposer.FIELD_DURATION_PAL
                        : 2 * VtiTimeStampComposer.FIELD_DURATION_NTSC;

            var fieldDuration = Math.Abs(new TimeSpan(frameTimestamp.Ticks - m_PrevEvenTicks).TotalMilliseconds / 2.0) - (frameStep - 1) * knownFrameDuration;

            bool returnBlank;
            if (IsDuplicatedOrDroppedFrames(m_PrevOddFieldOSD, m_PrevEvenFieldOSD, oddFieldOSD, evenFieldOSD, frameStep, knownFrameDuration, null, out returnBlank))
            {
                if (returnBlank)
                {
                    // The Duplicate/Dropped frame engine decided it is safer to return a blank frame
                    return false;
                }
                else
                {
                    // Duplicate or dropped frame detected. No corrections requied.
                }
            }
            else
            {
                if (!IsFieldDurationOkay(fieldDuration))
                {
                    long fieldDistance = (long)Math.Round((10000 * (m_VideoFormat == VideoFormat.PAL ? VtiTimeStampComposer.FIELD_DURATION_PAL : VtiTimeStampComposer.FIELD_DURATION_NTSC)));

                    if (!TryCorrectTimestamp(m_PrevOddTicks + fieldDistance, frameTimestamp, oddFieldOSD, frameStep, null, false))
                    {
                        Trace.WriteLine(correctionDebugInfo);
                        Trace.WriteLine("IOTA-VTI Correction Failed: Cannot correct field duration PrevEven -> CurrOdd.");
                        return false;
                    }
                }
            }

            if (oddFieldOSD.ContainsDate)
            {
                frameTimestamp = new DateTime(oddFieldOSD.Year, oddFieldOSD.Month, oddFieldOSD.Day, oddFieldOSD.Hours, oddFieldOSD.Minutes, oddFieldOSD.Seconds).AddMilliseconds(Math.Min(10000, oddFieldOSD.Milliseconds10) / 10.0f);
            }
            else
            {
                frameTimestamp = new DateTime(1, 1, 1, oddFieldOSD.Hours, oddFieldOSD.Minutes, oddFieldOSD.Seconds).AddMilliseconds(Math.Min(10000, oddFieldOSD.Milliseconds10) / 10.0f);
            }

            return true;
	    }

	    // Note this method only looks at exact duplicate or dropped frames, assuming that all timestamp digits have been recognized correctly
        // It is also looking for exactly one single dropped frame
        private bool IsDuplicatedOrDroppedFrames(IVtiTimeStamp prefFrameField1, IVtiTimeStamp prefFrameField2, IVtiTimeStamp field1, IVtiTimeStamp field2, int frameStep, float knownFrameDurationMS, int? aavIntegratedFields, out bool returnBlank)
        {
            // The main challenge here is to correctly identify duplicate/dropped frames and not miss-identify OCR errors
            // that could accidentaly make the extracted time look like there was a duplicate/dropped frame
            // For this reason we require matching in both timestamps and field numbers (if field numbers are supported)
            if (prefFrameField1.HoursMinSecMilliEquals(field1) && prefFrameField2.HoursMinSecMilliEquals(field2))
            {
                Trace.WriteLine(string.Format("{0} {1} {2} {3}", prefFrameField1.AsString(), prefFrameField2.AsString(), field1.AsString(), field2.AsString()));

                if (prefFrameField1.ContainsFrameNumbers && field1.ContainsFrameNumbers)
                {
                    if (prefFrameField1.FrameNumber == field1.FrameNumber || prefFrameField2.FrameNumber == field2.FrameNumber)
                    {
                        Trace.WriteLine("OCR engine identifies duplicated frame.");
                        returnBlank = false;
                        return true;
                    }
                    else
                    {
                        Trace.WriteLine("OCR engine decided to return a blank timestamp while identifying duplicated frame.");
                        returnBlank = true;
                        return true;
                    }
                }
                else
                {
                    Trace.WriteLine("OCR engine identifies duplicated frame. Frame numbers are not supported.");
                    returnBlank = false;
                    return true;
                }
            }

            var newField1Ticks = field1.GetTicks();
            var droppedField1ExpectedTicks = new DateTime(prefFrameField1.GetTicks()).AddMilliseconds((frameStep + 1) * knownFrameDurationMS).Ticks;
            var newField2Ticks = field2.GetTicks();
            var droppedField2ExpectedTicks = new DateTime(prefFrameField2.GetTicks()).AddMilliseconds((frameStep + 1) * knownFrameDurationMS).Ticks;

            if (Math.Abs(new TimeSpan(droppedField1ExpectedTicks - newField1Ticks).TotalMilliseconds) < 2 && Math.Abs(new TimeSpan(droppedField2ExpectedTicks - newField2Ticks).TotalMilliseconds) < 2)
            {
                if (prefFrameField1.ContainsFrameNumbers && field1.ContainsFrameNumbers)
                {
                    if (prefFrameField1.FrameNumber + (frameStep + 1) * 2 == field1.FrameNumber || prefFrameField2.FrameNumber + (frameStep + 1) * 2  == field2.FrameNumber)
                    {
                        Trace.WriteLine("OCR engine identifies dropped frame.");
                        returnBlank = false;
                        return true;
                    }
                    else
                    {
                        Trace.WriteLine("OCR engine decided to return a blank timestamp while identifying dropped frame.");
                        returnBlank = true;
                        return true;
                    }
                }
                else
                {
                    Trace.WriteLine("OCR engine identifies dropped frame. Frame numbers are not supported.");
                    returnBlank = false;
                    return true;
                }                
            }

            returnBlank = false;
            return false;
	    }

        private bool TryCorrectTimestamp(long prevFieldTimestamp, DateTime fieldToCorrectTimestamp, IVtiTimeStamp fieldToCorrect, int frameStep, int? aavIntegratedFields, bool isAav)
        {
            string dateFormatComp = fieldToCorrect.ContainsDate ? "yyyy-MM-dd HH:mm:ss.fff" : "HH:mm:ss.fff";
          
            double stepCorrection = frameStep > 1 ? ((20000 * (frameStep - 1) * (m_VideoFormat == VideoFormat.PAL ? VtiTimeStampComposer.FIELD_DURATION_PAL : VtiTimeStampComposer.FIELD_DURATION_NTSC))) : 0;
            double fieldDistance = (10000 * (m_VideoFormat == VideoFormat.PAL ? VtiTimeStampComposer.FIELD_DURATION_PAL : VtiTimeStampComposer.FIELD_DURATION_NTSC));
            double aavCorrection = aavIntegratedFields.HasValue ? (10000 * (aavIntegratedFields.Value - 2) * (m_VideoFormat == VideoFormat.PAL ? VtiTimeStampComposer.FIELD_DURATION_PAL : VtiTimeStampComposer.FIELD_DURATION_NTSC)) : 0;
			long expectedTimestamp =
                prevFieldTimestamp + (long)Math.Round(stepCorrection + fieldDistance + aavCorrection);

            // Round the expected timestamp to a millisecond
            expectedTimestamp = 10000L * ((long)Math.Round(expectedTimestamp / 10000.0));

			// NOTE: We ignore the last digit from the milliseconds when comparing this timestamps. While this allows for incorectly read 10th of milliseconds to be passed
			//       unactioned, it doesn't create any timing or measurement issues
			DateTime expectedDateTime = new DateTime(expectedTimestamp);
            string expectedTimestampString = expectedDateTime.ToString(dateFormatComp);
            string expectedTimestampStringM1 = expectedDateTime.AddMilliseconds(-1).ToString(dateFormatComp);
            string expectedTimestampStringP1 = expectedDateTime.AddMilliseconds(1).ToString(dateFormatComp);
            string actualTimestampString = fieldToCorrectTimestamp.ToString(dateFormatComp);

			string difference = XorStrings(expectedTimestampString, actualTimestampString);
			long numberDifferences = difference.ToCharArray().Count(c => c != '\0');
            string differenceM1 = XorStrings(expectedTimestampStringM1, actualTimestampString);
            long numberDifferencesM1 = differenceM1.ToCharArray().Count(c => c != '\0');
            string differenceP1 = XorStrings(expectedTimestampStringP1, actualTimestampString);
            long numberDifferencesP1 = differenceP1.ToCharArray().Count(c => c != '\0');

            if (numberDifferences < numberDifferencesM1 && numberDifferences < numberDifferencesP1)
            {
                // Already correct
            }
            else if (numberDifferencesM1 < numberDifferences && numberDifferencesM1 < numberDifferencesP1)
            {
                numberDifferences = numberDifferencesM1;
                expectedDateTime = expectedDateTime.AddMilliseconds(-1);
            }
            else if (numberDifferencesP1 < numberDifferences && numberDifferencesP1 < numberDifferencesM1)
            {
                numberDifferences = numberDifferencesP1;
                expectedDateTime = expectedDateTime.AddMilliseconds(1);
            }

            if (numberDifferences <= TangraConfig.Settings.Generic.OcrMaxNumberDigitsToAutoCorrect || 
                (isAav && fieldToCorrect.Milliseconds10 == 0) /* Duplicated video field in AAV */)
			{
				// We can correct the one or two offending characters

                if (fieldToCorrect.ContainsDate)
                    fieldToCorrect.CorrectDate(expectedDateTime.Year, expectedDateTime.Month, expectedDateTime.Day);

                fieldToCorrect.Correct(expectedDateTime.Hour, expectedDateTime.Minute, expectedDateTime.Second, (int) Math.Round((expectedDateTime.Ticks%10000000)/1000.0));
                fieldToCorrect.NumberOfCorrectedDifferences = (int)numberDifferences;
				return true;
			}
			else
			{
				// Cannot correct more than one differences. Why not??
				return false;
			}
		}

		public string XorStrings(string a, string b)
		{
			char[] charAArray = a.ToCharArray();
			char[] charBArray = b.ToCharArray();
			char[] result = new char[charAArray.Length];
			int len = 0;

			// Set length to be the length of the shorter string
			if (a.Length > b.Length)
				len = b.Length - 1;
			else
				len = a.Length - 1;

			for (int i = 0; i <= len; i++)
			{
				result[i] = (char)(charAArray[i] ^ charBArray[i]);
			}

			return new string(result);
		}

        public void RegisterSuccessfulTimestamp(int frameNo, IVtiTimeStamp oddFieldOSD, IVtiTimeStamp evenFieldOSD, DateTime oddFieldTimestamp, DateTime evenFieldTimestamp)
		{
			m_PrevFrameNo = frameNo;
			m_PrevOddTicks = oddFieldTimestamp.Ticks;
			m_PrevEvenTicks = evenFieldTimestamp.Ticks;
			m_PrevOddFieldNo = oddFieldOSD.FrameNumber;
			m_PrevEvenFieldNo = evenFieldOSD.FrameNumber;
			m_PrevOddFieldOSD = new VtiTimeStamp(oddFieldOSD);
			m_PrevEvenFieldOSD = new VtiTimeStamp(evenFieldOSD);
		}

	    public void RegisterUnsuccessfulTimestamp(int frameNoframeNo, double fieldDurationMS, int? frameStep, int? integratedAavFields)
	    {
	        // NOTE: Registering an unsuccessful timestamp is done to help correct
            //       the next timestamp if there is an OCR error. The most common case covered here is 
            //       that the unsuccessful timestamp is simply the next consequtive frame which is unrecognized

            // TODO: Implement this
	    }

		public bool OddEvenFieldDirectionIsKnown()
		{
			return m_PrevOddTicks != -1 && m_PrevEvenTicks != -1;
		}

		public int GetOddEvenFieldDirection()
		{
			return m_PrevOddTicks - m_PrevEvenTicks < 0 ? -1 : 1;
		}

		private bool IsFieldDurationOkay(double fieldDuration)
		{
			if (m_VideoFormat == VideoFormat.PAL &&
                (Math.Abs(fieldDuration - VtiTimeStampComposer.FIELD_DURATION_PAL) > 1.0))
			{
				// PAL field is not 20ms
				return false;
			}

			if (m_VideoFormat == VideoFormat.NTSC &&
                (Math.Abs(fieldDuration - VtiTimeStampComposer.FIELD_DURATION_NTSC) > 1.0))
			{
				// NTSC field is not 20ms
				return false;
			}

			return true;
		}
	}
}
