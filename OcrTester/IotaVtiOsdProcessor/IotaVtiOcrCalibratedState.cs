using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OcrTester.IotaVtiOsdProcessor
{
	internal class IotaVtiOcrCalibratedState : IotaVtiOcrState
	{
		public override void InitialiseState(IotaVtiOcrProcessor stateManager)
		{
			m_Width = stateManager.CurrentImageWidth;
			m_Height = stateManager.CurrentImageHeight;
		}

		public override void FinaliseState(IotaVtiOcrProcessor stateManager)
		{ }


		public override void Process(IotaVtiOcrProcessor stateManager, Graphics graphics, int frameNo, bool isOddField)
		{
			if (m_Width != stateManager.CurrentImageWidth || m_Height != stateManager.CurrentImageHeight)
			{
				stateManager.ChangeState<IotaVtiOcrCalibratingState>();
				return;
			}

			uint[] pixels = stateManager.CurrentImage;
			IotaVtiTimeStampStrings ocredValue = OcrField(pixels, stateManager);
			stateManager.SetOcredString(ocredValue);

			if (graphics != null)
				base.PlotImage(graphics, stateManager);
		}

		internal static IotaVtiTimeStampStrings OcrField(uint[] pixels, IotaVtiOcrProcessor stateManager)
		{
			char char1 = OcrBlock(pixels, stateManager, 1);
			char char3 = OcrBlock(pixels, stateManager, 3);
			char char4 = OcrBlock(pixels, stateManager, 4);
			char char6 = OcrBlock(pixels, stateManager, 6);
			char char7 = OcrBlock(pixels, stateManager, 7);
			char char9 = OcrBlock(pixels, stateManager, 9);
			char char10 = OcrBlock(pixels, stateManager, 10);

			char char12 = OcrBlock(pixels, stateManager, 12);
			char char13 = OcrBlock(pixels, stateManager, 13);
			char char14 = OcrBlock(pixels, stateManager, 14);
			char char15 = OcrBlock(pixels, stateManager, 15);

			char char17 = OcrBlock(pixels, stateManager, 17);
			char char18 = OcrBlock(pixels, stateManager, 18);
			char char19 = OcrBlock(pixels, stateManager, 19);
			char char20 = OcrBlock(pixels, stateManager, 20);

			char char22 = OcrBlock(pixels, stateManager, 22);
			char char23 = OcrBlock(pixels, stateManager, 23);
			char char24 = OcrBlock(pixels, stateManager, 24);
			char char25 = OcrBlock(pixels, stateManager, 25);
			char char26 = OcrBlock(pixels, stateManager, 26);
			char char27 = OcrBlock(pixels, stateManager, 27);
			char char28 = OcrBlock(pixels, stateManager, 28);

			var rv = new IotaVtiTimeStampStrings()
			{
				NumSat = char1,
				HH = string.Format("{0}{1}", char3, char4).TrimEnd(),
				MM = string.Format("{0}{1}", char6, char7).TrimEnd(),
				SS = string.Format("{0}{1}", char9, char10).TrimEnd(),
				FFFF1 = string.Format("{0}{1}{2}{3}", char12, char13, char14, char15).TrimEnd(),
				FFFF2 = string.Format("{0}{1}{2}{3}", char17, char18, char19, char20).TrimEnd(),
				FRAMENO = string.Format("{0}{1}{2}{3}{4}{5}{6}", char22, char23, char24, char25, char26, char27, char28).TrimEnd()
			};

			return rv;
		}

		private static char OcrBlock(uint[] fieldPixels, IotaVtiOcrProcessor stateManager, int blockIndex)
		{
			uint[] block = stateManager.GetBlockAtPosition(fieldPixels, blockIndex);

			int[] diffSignatures = new int[10];
			int[] diffDigits = new int[10];
			diffSignatures[0] = GetDiffSignature(block, stateManager.ZeroDigitPattern);
			diffDigits[0] = 0;
			diffSignatures[1] = GetDiffSignature(block, stateManager.OneDigitPattern);
			diffDigits[1] = 1;
			diffSignatures[2] = GetDiffSignature(block, stateManager.TwoDigitPattern);
			diffDigits[2] = 2;
			diffSignatures[3] = GetDiffSignature(block, stateManager.ThreeDigitPattern);
			diffDigits[3] = 3;
			diffSignatures[4] = GetDiffSignature(block, stateManager.FourDigitPattern);
			diffDigits[4] = 4;
			diffSignatures[5] = GetDiffSignature(block, stateManager.FiveDigitPattern);
			diffDigits[5] = 5;
			diffSignatures[6] = GetDiffSignature(block, stateManager.SixDigitPattern);
			diffDigits[6] = 6;
			diffSignatures[7] = GetDiffSignature(block, stateManager.SevenDigitPattern);
			diffDigits[7] = 7;
			diffSignatures[8] = GetDiffSignature(block, stateManager.EightDigitPattern);
			diffDigits[8] = 8;
			diffSignatures[9] = GetDiffSignature(block, stateManager.NineDigitPattern);
			diffDigits[9] = 9;

			Array.Sort(diffSignatures, diffDigits);

			if (stateManager.IsMatchingSignature(diffSignatures[0]))
			{
				if (diffDigits[0] == 8 || diffDigits[0] == 6 || diffDigits[0] == 9)
				{
					// If we matched to a 6, 8 or 9, then do additional check as those three characters are too similar
				}

				return diffDigits[0].ToString()[0];
			}

			return ' ';
		}
	}
}
