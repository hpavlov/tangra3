/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Tangra.Model.Image;
using Tangra.OCR.TimeExtraction;

namespace Tangra.OCR.IotaVtiOsdProcessor
{
	internal abstract class IotaVtiOcrState
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

		protected void PlotImage(Graphics graphics, IotaVtiOcrProcessor stateManager, bool isOddField)
		{
			if (stateManager.BlockOffsetsX != null &&
								stateManager.BlockOffsetsX.Length == IotaVtiOcrProcessor.MAX_POSITIONS &&
								stateManager.BlockOffsetsX[1] > 0)
			{
				for (int i = 0; i < IotaVtiOcrProcessor.MAX_POSITIONS; i++)
				{
					if (stateManager.BlockOffsetsX[i] > 0)
					{
						graphics.DrawRectangle(
							Pens.Chartreuse,
							stateManager.BlockOffsetsX[i],
							stateManager.BlockOffsetY(isOddField),
							stateManager.BlockWidth,
							stateManager.BlockHeight);
					}
				}
			}
			else
			{
				graphics.DrawRectangle(
						Pens.Chartreuse,
						0,
						stateManager.BlockOffsetY(isOddField),
						m_Width,
						stateManager.BlockHeight);
			}
		}
	}

	internal class IotaVtiTimeStampStrings
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

    internal class IotaVtiTimeStamp : IVtiTimeStamp
	{
		public IotaVtiTimeStamp(IotaVtiTimeStampStrings timeStampStrings)
		{
			int.TryParse(timeStampStrings.NumSat + "", out NumSat);
		    int ival;
            Hours = ParseTwoDigitOCRedNumber(timeStampStrings.HH);
            Minutes = ParseTwoDigitOCRedNumber(timeStampStrings.MM);
            Seconds = ParseTwoDigitOCRedNumber(timeStampStrings.SS);
            if (int.TryParse(timeStampStrings.FFFF1.Length == 4 ? timeStampStrings.FFFF1.Replace(' ', '0') : timeStampStrings.FFFF2.Replace(' ', '0'), out ival)) Milliseconds10 = ival;
            if (int.TryParse(timeStampStrings.FRAMENO.Replace(' ', '0'), out ival)) FrameNumber = ival;
            OcredCharacters = string.Format("{0} {1}{2}{3} {4}{5} {6}", timeStampStrings.NumSat, timeStampStrings.HH, timeStampStrings.MM, timeStampStrings.SS, timeStampStrings.FFFF1, timeStampStrings.FFFF2, timeStampStrings.FRAMENO);
		}

        public IotaVtiTimeStamp(IotaVtiTimeStamp timeStamp)
        {
            Hours = timeStamp.Hours;
            Minutes = timeStamp.Minutes;
            Seconds = timeStamp.Seconds;
            Milliseconds10 = timeStamp.Milliseconds10;
            FrameNumber = timeStamp.FrameNumber;
        }

        private int ParseTwoDigitOCRedNumber(string str)
        {
            if (str == null) return 0;
            
            if (str.Length != 2)
            {
                return ParseIntValue(str, 1);
            }
            else
            {
                if (str[1] == ' ')
                    return ParseIntValue(str.Trim(), 10);
                else
                    return ParseIntValue(str.Trim(), 1);
            }
        }

        private int ParseIntValue(string str, int multiplier)
        {
            int ival;

            if (int.TryParse(str, out ival))
                return multiplier * ival;
            else
                return 0; 
        }

		public int NumSat;
		public int Hours { get; private set; }
        public int Minutes { get; private set; }
        public int Seconds { get; private set; }
        public int Milliseconds10 { get; private set; }
        public int FrameNumber { get; private set; }

        public bool ContainsDate
        {
            get { return false; }
        }

        public int Year { get; private set; }
        public int Month { get; private set; }
        public int Day { get; private set; }

        public string OcredCharacters { get; private set; }

        public bool ContainsFrameNumbers
        {
            get { return true; }
        }

        public void Correct(int hours, int minutes, int seconds, int milliseconds10)
        {
            Hours = hours;
			Minutes = minutes;
			Seconds = seconds;
            Milliseconds10 = milliseconds10;
        }

        public void CorrectFrameNumber(int frameNo)
        {
            FrameNumber = frameNo;
        }
	}

	internal static class Extensions
	{
		public static IValueType MostCommonValue<TModel, IValueType>(this IEnumerable<TModel> list, Expression<Func<TModel, IValueType>> expression)
		{
			var dict = new Dictionary<IValueType, int>();

			foreach (TModel model in list)
			{
				var selectedValue = expression.Compile().Invoke(model);
				if (!dict.ContainsKey(selectedValue))
					dict.Add(selectedValue, 0);
				else
					dict[selectedValue]++;
			}

			if (dict.Count == 0)
				return default(IValueType);

			IValueType[] keys = dict.Keys.ToArray();
			int[] occurrences = dict.Values.ToArray();

			Array.Sort(occurrences, keys);

			return keys[keys.Length - 1];
		}
	}
}
