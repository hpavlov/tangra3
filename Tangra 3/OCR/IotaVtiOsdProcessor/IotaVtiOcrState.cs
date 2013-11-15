using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Tangra.OCR.IotaVtiOsdProcessor
{
    public abstract class IotaVtiOcrState
    {
        protected int m_Width;
        protected int m_Height;

        protected int m_MinBlockWidth;
        protected int m_MaxBlockWidth;
        protected int m_MinBlockHeight;
        protected int m_MaxBlockHeight;

        public abstract void InitialiseState(IotaVtiOcrProcessor stateManager);
        public abstract void FinaliseState(IotaVtiOcrProcessor stateManager);
        public abstract void Process(IotaVtiOcrProcessor stateManager, Graphics g, int frameNo, bool isOddField);

        protected static int GetDiffSignature(uint[] probe, uint[] etalon)
        {
            int rv = 0;

            for (int i = 0; i < probe.Length; i++)
            {
                if (etalon[i] != probe[i])
                    rv++;
            }

            return rv;
        }
    }

	public class IotaVtiTimeStampStrings
	{
		public char NumSat;
		public string HH;
		public string MM;
		public string SS;
		public string FFFF1;
		public string FFFF2;
		public string FRAMENO;

		public bool AllCharsPresent()
		{
			return
				HH.Length == 2 &&
				MM.Length == 2 &&
				SS.Length == 2 &&
				(FFFF1.Length == 4 || FFFF2.Length == 4) &&
				FRAMENO.Length > 0 &&
				FRAMENO.IndexOf(' ') == -1;
		}
	}

	public class IotaVtiTimeStamp
	{
		public IotaVtiTimeStamp(IotaVtiTimeStampStrings timeStampStrings)
		{
			int.TryParse(timeStampStrings.NumSat + "", out NumSat);
			Hours = int.Parse(timeStampStrings.HH);
			Minutes = int.Parse(timeStampStrings.MM);
			Seconds = int.Parse(timeStampStrings.SS);
			Milliseconds10 = int.Parse(timeStampStrings.FFFF1.Length == 4 ? timeStampStrings.FFFF1 : timeStampStrings.FFFF2);
			FrameNumber = int.Parse(timeStampStrings.FRAMENO);
		}

		public int NumSat;
		public int Hours;
		public int Minutes;
		public int Seconds;
		public int Milliseconds10;
		public int FrameNumber;
	}

	public enum VideoFormat
	{
		PAL,
		NTSC
	}

	public static class Extensions
    {
        public static T Median<T>(this IList<T> list)
        {
            if (list.Count == 0)
                return default(T);

            T[] arrayList = list.ToArray();
            Array.Sort(arrayList);

            return arrayList[list.Count / 2];
        }
    }
}
