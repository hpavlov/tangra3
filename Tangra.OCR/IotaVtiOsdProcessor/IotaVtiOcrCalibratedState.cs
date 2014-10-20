/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tangra.OCR.IotaVtiOsdProcessor;

namespace Tangra.OCR.IotaVtiOsdProcessor
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

		private static List<int> s_DiffSignatures = new List<int>(new int[10]);
		private static List<int> s_DiffDigits = new List<int>(new int[10]);
		private const int MAX_MATCHES = 4;
		private static int[] s_StrictMatches = new int[MAX_MATCHES];
		private static int[] s_Matches = new int[MAX_MATCHES];


		private static char OcrBlock(uint[] fieldPixels, IotaVtiOcrProcessor stateManager, int blockIndex, bool isOddField)
		{
			uint[] block = stateManager.GetBlockAtPosition(fieldPixels, blockIndex, isOddField);

			s_DiffSignatures[0] = GetDiffSignature(block, stateManager.ZeroDigitPattern);
			s_DiffDigits[0] = 0;
			s_DiffSignatures[1] = GetDiffSignature(block, stateManager.OneDigitPattern);
			s_DiffDigits[1] = 1;
			s_DiffSignatures[2] = GetDiffSignature(block, stateManager.TwoDigitPattern);
			s_DiffDigits[2] = 2;
			s_DiffSignatures[3] = GetDiffSignature(block, stateManager.ThreeDigitPattern);
			s_DiffDigits[3] = 3;
			s_DiffSignatures[4] = GetDiffSignature(block, stateManager.FourDigitPattern);
			s_DiffDigits[4] = 4;
			s_DiffSignatures[5] = GetDiffSignature(block, stateManager.FiveDigitPattern);
			s_DiffDigits[5] = 5;
			s_DiffSignatures[6] = GetDiffSignature(block, stateManager.SixDigitPattern);
			s_DiffDigits[6] = 6;
			s_DiffSignatures[7] = GetDiffSignature(block, stateManager.SevenDigitPattern);
			s_DiffDigits[7] = 7;
			s_DiffSignatures[8] = GetDiffSignature(block, stateManager.EightDigitPattern);
			s_DiffDigits[8] = 8;
			s_DiffSignatures[9] = GetDiffSignature(block, stateManager.NineDigitPattern);
			s_DiffDigits[9] = 9;

			double maxStrictMatch = stateManager.BlockWidth * stateManager.BlockHeight / 12.0;
			double maxMatch = stateManager.BlockWidth * stateManager.BlockHeight / 8.0;
			int strictMatchesIdx = 0;
			int matchesIdx = 0;

			for (int i = 0; i < 10; i++)
			{
				if (s_DiffSignatures[i] < maxStrictMatch && strictMatchesIdx < MAX_MATCHES)
				{
					s_StrictMatches[strictMatchesIdx] = s_DiffDigits[i];
					strictMatchesIdx++;
				}

				if (s_DiffSignatures[i] < maxMatch && matchesIdx < MAX_MATCHES)
				{
					s_Matches[matchesIdx] = s_DiffDigits[i];
					matchesIdx++;
				}
			}

			if (strictMatchesIdx == 1)
			{
				return s_StrictMatches[0].ToString()[0];
			}
			else if (strictMatchesIdx == 0 && matchesIdx == 1)
			{
				return s_Matches[0].ToString()[0];
			}
			else if (strictMatchesIdx > 1 || matchesIdx > 1)
			{
				var allMatches = new List<int>();
				if (strictMatchesIdx > 1)
				{
					for (int i = 0; i < strictMatchesIdx; i++)
						allMatches.Add(s_StrictMatches[i]);
				}
				else if (matchesIdx > 1)
				{
					for (int i = 0; i < matchesIdx; i++)
						allMatches.Add(s_Matches[i]);
				}

				bool areAll8693Items = !allMatches.Any(x => x != 3 && x != 6 && x != 9 && x != 8);
				if (areAll8693Items)
				{
					bool is8Present = allMatches.Contains(8);
					bool is9Present = allMatches.Contains(9);
					bool is6Present = allMatches.Contains(6);
					bool is3Present = allMatches.Contains(3);
					return DistinguishEightFromSimilarChars(block, stateManager, is9Present, is6Present, is3Present, is8Present);

				}
			}


			return ' ';
		}


        private static char DistinguishEightFromSimilarChars(uint[] block, IotaVtiOcrProcessor stateManager, bool couldBeNine, bool couldBeSix, bool couldBeThree, bool couldBeEight)
		{
            int chanceToBeSix = couldBeSix ? GetPercentSimilarities(block, stateManager.SixEightXorPattern, stateManager.SixEightXorPatternFactor) : 0;
            int chanceToBeNine = couldBeNine ? GetPercentSimilarities(block, stateManager.NineEightXorPattern, stateManager.NineEightXorPatternFactor) : 0;
            int chanceToBeThree = couldBeThree ? GetPercentSimilarities(block, stateManager.ThreeEightXorPattern, stateManager.ThreeEightXorPatternFactor) : 0;

			int minRequiredChance = couldBeEight ? 50 : 0;

			if (chanceToBeThree > minRequiredChance && chanceToBeThree > chanceToBeSix && chanceToBeThree > chanceToBeNine)
			{
				return '3';
			}
			else if (chanceToBeSix > minRequiredChance && chanceToBeSix > chanceToBeNine && chanceToBeSix > chanceToBeThree)
			{
				return '6';
			}
			else if (chanceToBeNine > minRequiredChance && chanceToBeNine > chanceToBeSix && chanceToBeNine > chanceToBeThree)
			{
				return '9';
			}
			else if (couldBeEight)
			{
				return '8';
			}

	        return ' ';
		}

        private static int GetPercentSimilarities(uint[] probe, uint[] xorEtalon, int totalSimilarities)
        {
            int rv = 0;

            for (int i = 0; i < probe.Length; i++)
            {
                if (xorEtalon[i] == probe[i])
                    rv++;
            }

            return (int)Math.Round(100.0 * rv / totalSimilarities);
        }
	}
}
