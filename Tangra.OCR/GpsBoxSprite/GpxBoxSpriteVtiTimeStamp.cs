using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.OCR.TimeExtraction;

namespace Tangra.OCR.GpsBoxSprite
{
    public class GpxBoxSpriteVtiTimeStamp : IVtiTimeStamp
    {
        internal GpxBoxSpriteVtiTimeStamp(
            int year, int month, int day, 
            int hour, int min, int sec, int ms10First, int ms10Second, 
            string ocredChars, bool isOddField, bool useFirstMs10)
        {
            Year = year;
            Month = month;
            Day = day;
            Hours = hour;
            Minutes = min;
            Seconds = sec;
            Milliseconds10First = ms10First;
            Milliseconds10Second = ms10Second;
            m_IsOddField = isOddField;
            m_UseFirstMs10 = useFirstMs10;
            Milliseconds10 = isOddField || useFirstMs10 ? ms10First : ms10Second; /* For the normal 'Odd' fields, the end timestamp is the first millis */
            OcredCharacters = ocredChars;
        }

        public GpxBoxSpriteVtiTimeStamp Clone()
        {
            return new GpxBoxSpriteVtiTimeStamp(Year, Month, Day, Hours, Minutes, Seconds, Milliseconds10First, Milliseconds10Second, OcredCharacters, m_IsOddField, m_UseFirstMs10);
        }

        private bool m_IsOddField;
        private bool m_UseFirstMs10;

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
        { }
    }
}
