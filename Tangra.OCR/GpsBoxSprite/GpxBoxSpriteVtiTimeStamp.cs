using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.OCR.TimeExtraction;

namespace Tangra.OCR.GpsBoxSprite
{
    public class GpxBoxSpriteVtiTimeStamp : IVtiTimeStamp
    {
        internal GpxBoxSpriteVtiTimeStamp(int year, int month, int day, int hour, int min, int sec, int ms10First, int ms10Second)
        {
            Year = year;
            Month = month;
            Day = day;
            Hours = hour;
            Minutes = min;
            Seconds = sec;
            Milliseconds10First = ms10First;
            Milliseconds10Second = ms10Second;
            Milliseconds10 = ms10First > ms10Second ? ms10First : ms10Second;
        }

        public GpxBoxSpriteVtiTimeStamp Clone()
        {
            return new GpxBoxSpriteVtiTimeStamp(Year, Month, Day, Hours, Minutes, Seconds, Milliseconds10First, Milliseconds10Second);
        }

        public bool ContainsFrameNumbers 
        {
            get { return false; }
        }

        public bool ContainsDate
        {
            get { return true; }
        }
        
        public int Year { get; private set; }
        public int Month { get; private set; }
        public int Day { get; private set; }

        public int FrameNumber { get; private set; }

        public int Hours { get; private set; }

        public int Minutes { get; private set; }

        public int Seconds { get; private set; }

        public int Milliseconds10 { get; private set; }

        public int Milliseconds10First { get; private set; }
        public int Milliseconds10Second { get; private set; }

        public void Correct(int hours, int minutes, int seconds, int milliseconds10)
        {
            Hours = hours;
            Minutes = minutes;
            Seconds = seconds;
            Milliseconds10 = milliseconds10;
        }
    }
}
