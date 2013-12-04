using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.Model.Config;
using Tangra.OCR.IotaVtiOsdProcessor;

namespace Tangra.OCR
{
	internal class IotaVtiOcrCorrector
	{
		private int m_PrevFrameNo;
		private long m_PrevOddTicks;
		private long m_PrevEvenTicks;
		private long m_PrevOddFieldNo;
		private long m_PrevEvenFieldNo;
		private IotaVtiTimeStamp m_PrevOddFieldOSD;
		private IotaVtiTimeStamp m_PrevEvenFieldOSD;
		private VideoFormat m_VideoFormat;

		public void Reset(VideoFormat? videoFormat)
		{
			m_PrevFrameNo = -1;
			m_PrevOddTicks = -1;
			m_PrevEvenTicks = -1;
			m_PrevOddFieldNo = -1;
			m_PrevEvenFieldNo = -1;

			if (!videoFormat.HasValue)
				throw new ArgumentException();

			m_VideoFormat = videoFormat.Value;
		}

		public bool TryToCorrect(int frameNo, IotaVtiTimeStamp oddFieldOSD, IotaVtiTimeStamp evenFieldOSD, ref DateTime oddFieldTimestamp, ref DateTime evenFieldTimestamp)
		{
			if (m_PrevFrameNo == -1 || m_PrevOddTicks == -1 || m_PrevEvenTicks == -1)
				return false;

			double fieldDuration;

			if (m_PrevEvenTicks > m_PrevOddTicks)
			{
				fieldDuration = Math.Abs(new TimeSpan(oddFieldTimestamp.Ticks - m_PrevEvenTicks).TotalMilliseconds);
				
				if (!IsFieldDurationOkay(fieldDuration))
				{
					if (!TryCorrectTimestamp(m_PrevEvenTicks, oddFieldTimestamp, oddFieldOSD))
						return false;
				}

				fieldDuration = Math.Abs(new TimeSpan(oddFieldTimestamp.Ticks - evenFieldTimestamp.Ticks).TotalMilliseconds);

				if (!IsFieldDurationOkay(fieldDuration))
				{
					if (!TryCorrectTimestamp(oddFieldTimestamp.Ticks, evenFieldTimestamp, evenFieldOSD))
						return false;
				}
			}
			else if (m_PrevEvenTicks < m_PrevOddTicks)
			{
				fieldDuration = Math.Abs(new TimeSpan(evenFieldTimestamp.Ticks - m_PrevOddTicks).TotalMilliseconds);

				if (!IsFieldDurationOkay(fieldDuration))
				{
					if (!TryCorrectTimestamp(m_PrevOddTicks, evenFieldTimestamp, evenFieldOSD))
						return false;
				}

				fieldDuration = Math.Abs(new TimeSpan(evenFieldTimestamp.Ticks - oddFieldTimestamp.Ticks).TotalMilliseconds);

				if (!IsFieldDurationOkay(fieldDuration))
				{
					if (!TryCorrectTimestamp(evenFieldTimestamp.Ticks, oddFieldTimestamp, oddFieldOSD))
						return false;
				}
			}

			if (m_PrevOddFieldNo < m_PrevEvenFieldNo)
			{
				if (m_PrevEvenFieldNo + 1 != oddFieldOSD.FrameNumber)
				{
					// We don't correct Field numbers as they are not used anywhere for now, we just ignore them as no errors
				}

				if (oddFieldOSD.FrameNumber + 1 != evenFieldOSD.FrameNumber)
				{
					// We don't correct Field numbers as they are not used anywhere for now, we just ignore them as no errors
				}
			}
			else if (m_PrevOddFieldNo > m_PrevEvenFieldNo)
			{
				if (m_PrevEvenFieldNo + 1 != oddFieldOSD.FrameNumber)
				{
					// We don't correct Field numbers as they are not used anywhere for now, we just ignore them as no errors
				}

				if (oddFieldOSD.FrameNumber + 1 != evenFieldOSD.FrameNumber)
				{
					// We don't correct Field numbers as they are not used anywhere for now, we just ignore them as no errors
				}
			}

			oddFieldTimestamp = new DateTime(1, 1, 1, oddFieldOSD.Hours, oddFieldOSD.Minutes, oddFieldOSD.Seconds, (int)Math.Round(oddFieldOSD.Milliseconds10 / 10.0f));
			evenFieldTimestamp = new DateTime(1, 1, 1, evenFieldOSD.Hours, evenFieldOSD.Minutes, evenFieldOSD.Seconds, (int)Math.Round(evenFieldOSD.Milliseconds10 / 10.0f));

			return true;
		}

		private bool TryCorrectTimestamp(long prevFieldTimestamp, DateTime fieldToCorrectTimestamp, IotaVtiTimeStamp fieldToCorrect)
		{
			long expectedTimestamp = 
				prevFieldTimestamp +
				(long)Math.Round(10000 * (m_VideoFormat == VideoFormat.PAL ? IotaVtiOcrProcessor.FIELD_DURATION_PAL : IotaVtiOcrProcessor.FIELD_DURATION_NTSC));

			// NOTE: We ignore the last digit from the milliseconds when comparing this timestamps. While this allows for incorectly read 10th of milliseconds to be passed
			//       unactioned, it doesn't create any timing or measurement issues
			DateTime expectedDateTime = new DateTime(expectedTimestamp);
			string expectedTimestampString = expectedDateTime.ToString("HH:mm:ss.fff");
			string actualTimestampString = fieldToCorrectTimestamp.ToString("HH:mm:ss.fff");

			string difference = XorStrings(expectedTimestampString, actualTimestampString);
			long numberDifferences = difference.ToCharArray().Count(c => c != '\0');

			if (numberDifferences <= TangraConfig.Settings.Generic.OcrMaxNumberErrorsToAutoCorrect)
			{
				// We can correct the single offending character

				fieldToCorrect.Hours = expectedDateTime.Hour;
				fieldToCorrect.Minutes = expectedDateTime.Minute;
				fieldToCorrect.Seconds = expectedDateTime.Second;
				fieldToCorrect.Milliseconds10 = expectedDateTime.Millisecond * 10;

				return true;
			}
			else
			{
				// Cannot correct more than one differences. Why not??
				return false;
			}

			return true;
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

			for (int i = 0; i < len; i++)
			{
				result[i] = (char)(charAArray[i] ^ charBArray[i]);
			}

			return new string(result);
		}

		public void RegisterSuccessfulTimestamp(int frameNo, IotaVtiTimeStamp oddFieldOSD, IotaVtiTimeStamp evenFieldOSD, DateTime oddFieldTimestamp, DateTime evenFieldTimestamp)
		{
			m_PrevFrameNo = frameNo;
			m_PrevOddTicks = oddFieldTimestamp.Ticks;
			m_PrevEvenTicks = evenFieldTimestamp.Ticks;
			m_PrevOddFieldNo = oddFieldOSD.FrameNumber;
			m_PrevEvenFieldNo = evenFieldOSD.FrameNumber;
			m_PrevOddFieldOSD = new IotaVtiTimeStamp(oddFieldOSD);
			m_PrevEvenFieldOSD = new IotaVtiTimeStamp(evenFieldOSD);
		}

		private bool IsFieldDurationOkay(double fieldDuration)
		{
			if (m_VideoFormat == VideoFormat.PAL &&
				(Math.Abs(fieldDuration - IotaVtiOcrProcessor.FIELD_DURATION_PAL) > 1.0))
			{
				// PAL field is not 20ms
				return false;
			}

			if (m_VideoFormat == VideoFormat.NTSC &&
				(Math.Abs(fieldDuration - IotaVtiOcrProcessor.FIELD_DURATION_NTSC) > 1.0))
			{
				// NTSC field is not 20ms
				return false;
			}

			return true;
		}
	}
}
