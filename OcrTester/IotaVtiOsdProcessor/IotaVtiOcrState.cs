using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Tangra.Model.Image;

namespace OcrTester.IotaVtiOsdProcessor
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

	internal class IotaVtiTimeStamp
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

		public IotaVtiTimeStamp(IotaVtiTimeStamp timeStamp)
		{
			Hours = timeStamp.Hours;
			Minutes = timeStamp.Minutes;
			Seconds = timeStamp.Seconds;
			Milliseconds10 = timeStamp.Milliseconds10;
			FrameNumber = timeStamp.FrameNumber;
		}

		public int NumSat;
		public int Hours;
		public int Minutes;
		public int Seconds;
		public int Milliseconds10;
		public int FrameNumber;
	}

	internal enum VideoFormat
	{
		PAL,
		NTSC
	}

	internal static class Extensions
	{
		public static T Median<T>(this IList<T> list)
		{
			if (list.Count == 0)
				return default(T);

			T[] arrayList = list.ToArray();
			Array.Sort(arrayList);

			return arrayList[list.Count / 2];
		}

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
