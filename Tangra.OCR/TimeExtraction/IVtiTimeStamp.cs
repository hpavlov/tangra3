using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;

namespace Tangra.OCR.TimeExtraction
{
    internal enum VideoFormat
    {
        PAL,
        NTSC
    }

    public interface IVtiTimeStamp
    {
        bool ContainsFrameNumbers { get; }
        int FrameNumber { get; }
        int Hours { get; }
        int Minutes { get; }
        int Seconds { get; }
        int Milliseconds10 { get; }
        bool ContainsDate { get; }
        int Year { get; }
        int Month { get; }
        int Day { get; }
        string OcredCharacters { get; }
        void Correct(int hours, int minutes, int seconds, int milliseconds10);
        void CorrectDate(int year, int month, int day);
        void CorrectFrameNumber(int frameNo);
    }

    public static class IVtiTimeStampExtensions
    {
        public static bool HoursMinSecMilliEquals(this IVtiTimeStamp x, IVtiTimeStamp y)
        {
            return x.Hours == y.Hours && x.Minutes == y.Minutes && x.Seconds == y.Seconds && x.Milliseconds10 == y.Milliseconds10;
        }

        public static string AsString(this IVtiTimeStamp x)
        {
            if (x.ContainsFrameNumbers)
                return string.Format("{0:00}:{1:00}:{2:00}.{3:0000} FrmNo:{4}", x.Hours, x.Minutes, x.Seconds, x.Milliseconds10, x.FrameNumber);
            else
                return string.Format("{0:00}:{1:00}:{2:00}.{3:0000}", x.Hours, x.Minutes, x.Seconds, x.Milliseconds10);
        }

        public static string AsFullString(this IVtiTimeStamp x)
        {
            if (x.ContainsDate)
                return string.Format("{0}-{1:00}-{2:00} {3}", x.Year, x.Month, x.Day, x.AsString());
            else
                return x.AsString();
        }

        public static long GetTicks(this IVtiTimeStamp x)
        {
            return new DateTime(Math.Max(1, x.Year), 1, 1)
                .AddMonths(Math.Max(1, x.Month) - 1)
                .AddDays(Math.Max(1, x.Day) - 1)
                .AddHours(x.Hours)
                .AddMinutes(x.Minutes)
                .AddSeconds(x.Seconds)
                .AddMilliseconds(x.Milliseconds10/10.0)
                .Ticks;
        }
    }

    public class VtiTimeStamp : IVtiTimeStamp
    {
        public VtiTimeStamp(IVtiTimeStamp timeStamp)
		{
			Hours = timeStamp.Hours;
			Minutes = timeStamp.Minutes;
			Seconds = timeStamp.Seconds;
			Milliseconds10 = timeStamp.Milliseconds10;
			FrameNumber = timeStamp.FrameNumber;
            ContainsFrameNumbers = timeStamp.ContainsFrameNumbers;

            ContainsDate = timeStamp.ContainsDate;
            Year = timeStamp.Year;
            Month = timeStamp.Month;
            Day = timeStamp.Day;
		}

        public bool ContainsFrameNumbers { get; private set; }
        public int FrameNumber { get; private set; }
        public int Hours { get; private set; }
        public int Minutes { get; private set; }
        public int Seconds { get; private set; }
        public int Milliseconds10 { get; private set; }

        public bool ContainsDate { get; private set; }
        public int Year { get; private set; }
        public int Month { get; private set; }
        public int Day { get; private set; }

        public string OcredCharacters { get; private set; }

        public void Correct(int hours, int minutes, int seconds, int milliseconds10)
        {
            Hours = hours;
            Minutes = minutes;
            Seconds = seconds;
            Milliseconds10 = milliseconds10;
        }

        public void CorrectDate(int year, int month, int day)
        {
            Year = year;
            Month = month;
            Day = day;
        }

        public void CorrectFrameNumber(int frameNo)
        {
            if (ContainsFrameNumbers)
                FrameNumber = frameNo;
        }
    }
    	
}
