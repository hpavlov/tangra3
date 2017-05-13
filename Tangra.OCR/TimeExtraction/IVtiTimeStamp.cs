using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;

namespace Tangra.OCR.TimeExtraction
{
    public interface IVtiTimeStamp
    {
        int FrameNumber { get; }
        int Hours { get; }
        int Minutes { get; }
        int Seconds { get; }
        int Milliseconds10 { get; }
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
		}

        public int FrameNumber { get; private set; }
        public int Hours { get; private set; }
        public int Minutes { get; private set; }
        public int Seconds { get; private set; }
        public int Milliseconds10 { get; private set; }

        public void Correct(int hours, int minutes, int seconds, int milliseconds10)
        {
            Hours = hours;
            Minutes = minutes;
            Seconds = seconds;
            Milliseconds10 = milliseconds10;
        }
    }
    	
}
