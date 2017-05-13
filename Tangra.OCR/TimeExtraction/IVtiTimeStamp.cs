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
        void Correct(int hours, int minutes, int seconds, int milliseconds10);
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

        public void Correct(int hours, int minutes, int seconds, int milliseconds10)
        {
            Hours = hours;
            Minutes = minutes;
            Seconds = seconds;
            Milliseconds10 = milliseconds10;
        }
    }
    	
}
