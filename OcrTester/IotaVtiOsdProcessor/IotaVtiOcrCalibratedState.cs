/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

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
			IotaVtiTimeStampStrings ocredValue = OcrField(pixels, stateManager, isOddField);
			stateManager.SetOcredString(ocredValue);

			if (graphics != null)
				base.PlotImage(graphics, stateManager, isOddField);
		}

		internal static IotaVtiTimeStampStrings OcrField(uint[] pixels, IotaVtiOcrProcessor stateManager, bool isOddField)
		{
			char char1 = OcrBlock(pixels, stateManager, 1, isOddField);
			char char3 = OcrBlock(pixels, stateManager, 3, isOddField);
			char char4 = OcrBlock(pixels, stateManager, 4, isOddField);
			char char6 = OcrBlock(pixels, stateManager, 6, isOddField);
			char char7 = OcrBlock(pixels, stateManager, 7, isOddField);
			char char9 = OcrBlock(pixels, stateManager, 9, isOddField);
			char char10 = OcrBlock(pixels, stateManager, 10, isOddField);

			char char12 = OcrBlock(pixels, stateManager, 12, isOddField);
			char char13 = OcrBlock(pixels, stateManager, 13, isOddField);
			char char14 = OcrBlock(pixels, stateManager, 14, isOddField);
			char char15 = OcrBlock(pixels, stateManager, 15, isOddField);

			char char17 = OcrBlock(pixels, stateManager, 17, isOddField);
			char char18 = OcrBlock(pixels, stateManager, 18, isOddField);
			char char19 = OcrBlock(pixels, stateManager, 19, isOddField);
			char char20 = OcrBlock(pixels, stateManager, 20, isOddField);

			char char22 = OcrBlock(pixels, stateManager, 22, isOddField);
			char char23 = OcrBlock(pixels, stateManager, 23, isOddField);
			char char24 = OcrBlock(pixels, stateManager, 24, isOddField);
			char char25 = OcrBlock(pixels, stateManager, 25, isOddField);
			char char26 = OcrBlock(pixels, stateManager, 26, isOddField);
			char char27 = OcrBlock(pixels, stateManager, 27, isOddField);
			char char28 = OcrBlock(pixels, stateManager, 28, isOddField);

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

		private static char OcrBlock(uint[] fieldPixels, IotaVtiOcrProcessor stateManager, int blockIndex, bool isOddField)
		{
			uint[] block = stateManager.GetBlockAtPosition(fieldPixels, blockIndex, isOddField);

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
