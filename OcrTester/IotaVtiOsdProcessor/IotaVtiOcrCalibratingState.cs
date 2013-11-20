using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tangra.Model.Image;

namespace OcrTester.IotaVtiOsdProcessor
{
	internal class CalibratedBlockPosition
	{
		public CalibratedBlockPosition(uint[] image)
		{
			Image = new uint[image.Length];
			Array.Copy(image, Image, image.Length);
		}

		public int FrameNo;
		public bool IsOddField;

		public int BlockWidth;
		public int BlockHeight;
		public int BlockOffsetX;
		public int BlockOffsetY;
		public int[] BlockOffsetsX = new int[IotaVtiOcrProcessor.MAX_POSITIONS];

		public uint[] Image;

		public uint[] LastFrameNoDigit;
		public uint[] SecondLastFrameNoDigit;
		public int[] BestStartingPositions;

		public int GetLastFrameNoDigitPosition(IotaVtiOcrProcessor stateManager)
		{
			for (int i = IotaVtiOcrProcessor.FIRST_FRAME_NO_DIGIT_POSITIONS; i <= IotaVtiOcrProcessor.MAX_POSITIONS; i++)
			{
				uint[] pixels = stateManager.GetBlockAtPosition(Image, i);
				int nuberSignalPixels = pixels.Count(x => x < 127);
				if (stateManager.IsMatchingSignature(nuberSignalPixels) /* Matches to blank block */)
					return i;
			}

			return -1;
		}

		public void PrepareLastTwoDigitsFromTheFrameNumber(IotaVtiOcrProcessor stateManager)
		{
			LastFrameNoDigit = stateManager.GetBlockAtPosition(Image, stateManager.LastFrameNoDigitPosition);
			SecondLastFrameNoDigit = stateManager.GetBlockAtPosition(Image, stateManager.LastFrameNoDigitPosition - 1);
		}

		public void PrepareLastTwoDigitsFromTheFrameNumber(IotaVtiOcrProcessor stateManager, int lastBlockX, int secondLastBlockX)
		{
			LastFrameNoDigit = stateManager.GetBlockAtXOffset(Image, lastBlockX);
			SecondLastFrameNoDigit = stateManager.GetBlockAtXOffset(Image, secondLastBlockX);
		}
	}

	internal class IotaVtiOcrCalibratingState : IotaVtiOcrState
	{
		private List<CalibratedBlockPosition> m_CalibratedPositons = new List<CalibratedBlockPosition>();

		private void CalibrateBlockPositonsTop(IotaVtiOcrProcessor stateManager)
		{
			int count = 0;
			int maxRating = -1;

			int bestYOffs = -1;
			int bestHeight = -1;

			int imageHeight = stateManager.CurrentImageHeight;
			int imageWidth = stateManager.CurrentImageWidth;

			for (int top = 0; top < 20; top++)
			{
				for (int y = m_MinBlockHeight; y <= m_MaxBlockHeight; y++)
				{
					int totalRating = 0;

					int bottom = top + y;

					bool bottomOk = (bottom - 1) < imageHeight;
					bool bottom2Ok = bottom < imageHeight;
                    bool bottom3Ok = (bottom + 1) < imageHeight;

					for (int x = 0; x < stateManager.CurrentImageWidth; x++)
					{
						if (x >= stateManager.CurrentImageWidth || x < 0) continue;
						if (top < 0) continue;
						if (bottom - 1 >= stateManager.CurrentImageHeight) continue;

						if (bottomOk && stateManager.CurrentImage[x + imageWidth * (bottom - 1)] < 127 &&
                           ((bottom2Ok && stateManager.CurrentImage[x + imageWidth * bottom] > 127) || bottom == imageHeight) &&
                           ((bottom3Ok && stateManager.CurrentImage[x + imageWidth * (bottom + 1)] > 127) || bottom >= imageHeight - 1))
                            totalRating++;


                        if (stateManager.CurrentImage[x + imageWidth * (top + 1)] < 127 &&
						   stateManager.CurrentImage[x + imageWidth * top] > 127 &&
                           (top == 0 || stateManager.CurrentImage[x + imageWidth * (top - 1)] > 127)) 
                            totalRating++;
					}

					if (totalRating > maxRating)
					{
						maxRating = totalRating;
						bestYOffs = top;
						bestHeight = y;
					}

					count++;
				}
			}

			stateManager.BlockHeight = bestHeight;
			stateManager.BlockOffsetY = bestYOffs;
		}

		private void CalibrateBlockPositonsWidth(IotaVtiOcrProcessor stateManager)
		{
			int maxRating = -1;

			int bestWidth = -1;
			var bestStartingPositions = new List<int>();

			// Determined previously
			int bestHeight = stateManager.BlockHeight;
			int bestYOffs = stateManager.BlockOffsetY;

			int imageWidth = stateManager.CurrentImageWidth;
			var startingPositions = new List<int>();

			for (int width = m_MinBlockWidth; width <= m_MaxBlockWidth; width++)
			{
				int totalRating = 0;
				startingPositions.Clear();

				for (int x = 1; x < stateManager.CurrentImageWidth - width - 1; x++)
				{
					bool prevTwoVerticalsAreWhite = true;

					for (int y = bestYOffs; y < bestYOffs + bestHeight + 1; y++)
					{
					    if (y >= stateManager.CurrentImageHeight) continue;

						if (stateManager.CurrentImage[x - 1 + imageWidth * y] < 127 || stateManager.CurrentImage[x + imageWidth * y] < 127)
						{
							prevTwoVerticalsAreWhite = false;
							break;
						}
					}

					if (!prevTwoVerticalsAreWhite)
						continue;

					for (int y = bestYOffs; y < bestYOffs + bestHeight + 1; y++)
					{
                        if (y >= stateManager.CurrentImageHeight) continue;

						if (stateManager.CurrentImage[x + 1 + imageWidth * y] < 127)
						{
							totalRating++;

							if (stateManager.CurrentImage[x + width - 1 + imageWidth * y] < 127 &&
								stateManager.CurrentImage[x + width + imageWidth * y] > 127 &&
								stateManager.CurrentImage[x + width + 1 + imageWidth * y] > 127)
							{
								startingPositions.Add(x);
								totalRating++;
							}
						}
					}
				}

				if (totalRating > maxRating)
				{
					// Collect stats about the starting positions ??
					maxRating = totalRating;
					bestWidth = width;
					bestStartingPositions.Clear();
					bestStartingPositions.AddRange(startingPositions);
				}
			}

			stateManager.BlockWidth = bestWidth;

			var calibrationBlock = new CalibratedBlockPosition(stateManager.CurrentImage)
			{
				BlockWidth = bestWidth,
				BlockHeight = bestHeight,
				BlockOffsetY = bestYOffs,
				BestStartingPositions = bestStartingPositions.ToArray()
			};
			m_CalibratedPositons.Add(calibrationBlock);
		}

		private void Reinitialise(int width, int height)
		{
			m_Width = width;
			m_Height = height;

			m_MaxBlockWidth = m_Width / 27;
			m_MinBlockWidth = (m_Width / 30) - 2;
			m_MinBlockHeight = Math.Min(10, 2 * m_Height / 3);
			m_MaxBlockHeight = m_Height - 2;

			m_CalibratedPositons.Clear();
		}

		public override void InitialiseState(IotaVtiOcrProcessor stateManager)
		{
			m_Width = 0;
			m_Height = 0;
		}

		public override void FinaliseState(IotaVtiOcrProcessor stateManager)
		{ }

		private int ROUTH_START_FRAME_NUMBER_BLOCKS;

		public override void Process(IotaVtiOcrProcessor stateManager, Graphics graphics, int frameNo, bool isOddField)
		{
			if (m_Width != stateManager.CurrentImageWidth || m_Height != stateManager.CurrentImageHeight)
			{
				Reinitialise(stateManager.CurrentImageWidth, stateManager.CurrentImageHeight);
				ROUTH_START_FRAME_NUMBER_BLOCKS = (int)Math.Round(IotaVtiOcrProcessor.COEFF_FIRST_FRAME_NO_DIGIT_POSITION * stateManager.CurrentImageWidth);
			}

			CalibrateBlockPositonsTop(stateManager);
			CalibrateBlockPositonsWidth(stateManager);

			if (m_CalibratedPositons.Count == 10)
			{
				var normalizedPositions = new List<CalibratedBlockPosition>();

				if (CalibrateFrameNumberBlockPositions(stateManager, out normalizedPositions) &&
					RecognizedTimestampsConsistent(stateManager, normalizedPositions))
				{
					stateManager.ChangeState<IotaVtiOcrCalibratedState>();
					stateManager.Process(stateManager.CurrentImage, stateManager.CurrentImageWidth, stateManager.CurrentImageHeight, graphics, frameNo, isOddField);
					return;
				}
				else
				{
					// Make sure we always remove the whole frame, so swapped fields create less issues when learning the characters
					m_CalibratedPositons.RemoveAt(0);
					m_CalibratedPositons.RemoveAt(0);
				}
			}

			if (graphics != null)
				base.PlotImage(graphics, stateManager);
		}

		private int GetLastFrameNoDigitStartPosition(IotaVtiOcrProcessor stateManager)
		{
			uint[] result = new uint[m_CalibratedPositons[0].Image.Length];
			for (int i = 0; i < m_CalibratedPositons.Count - 2; i++)
			{
				uint[] prev = m_CalibratedPositons[i].Image;
				uint[] next = m_CalibratedPositons[i + 1].Image;

				for (int j = 0; j < result.Length; j++)
				{
					int x = j % stateManager.CurrentImageWidth;
					if (x > ROUTH_START_FRAME_NUMBER_BLOCKS && prev[j] != next[j])
						result[j]++;
				}
			}

			int bestRating = -1;
			int bestStartPosition = -1;

			for (int x = ROUTH_START_FRAME_NUMBER_BLOCKS; x < stateManager.CurrentImageWidth - stateManager.BlockWidth; x++)
			{
				int currentRating = 0;

				for (int y = 0; y < stateManager.BlockHeight; y++)
				{
					for (int k = 0; k < stateManager.BlockWidth; k++)
					{
						if (result[x + k + (stateManager.BlockOffsetY + y) * stateManager.CurrentImageWidth] > 0)
							currentRating++;
					}
				}

				if (currentRating > bestRating)
				{
					bestStartPosition = x;
					bestRating = currentRating;
				}
			}

			return bestStartPosition;
		}

		private bool CalibrateFrameNumberBlockPositions(IotaVtiOcrProcessor stateManager, out List<CalibratedBlockPosition> normalizedPositions)
		{
			int last = GetLastFrameNoDigitStartPosition(stateManager);

			if (last > -1)
			{
				int secondLast = last - stateManager.BlockWidth;

				// We now know the position of the two digits. We can now 'learn' the digits from '0' to '9' finding the change of the second last digit
				// and then infering the values from '0' to '9' of the last digit

				foreach (CalibratedBlockPosition blockPosition in m_CalibratedPositons)
					blockPosition.PrepareLastTwoDigitsFromTheFrameNumber(stateManager, last, secondLast);

				if (DigitPatternsRecognized(stateManager, out normalizedPositions))
				{
					stateManager.LastBlockOffsetsX = last;
					stateManager.SecondLastBlockOffsetsX = secondLast;

					// Now go and determine the positions of the remaining blocks by matching their value to the 'learned' digits
					return LocateRemainingBlockPositions(stateManager);
				}
			}
			else
				normalizedPositions = new List<CalibratedBlockPosition>();

			return false;
		}


		private char OcrBlock(IotaVtiOcrProcessor stateManager, uint[] image, int startingPosition, out int rating)
		{
			uint[] block = stateManager.GetBlockAtXOffset(image, startingPosition);

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

				rating = diffSignatures[0];
				return diffDigits[0].ToString()[0];
			}

			rating = int.MaxValue;
			return ' ';
		}

		private bool LocateRemainingBlockPositions(IotaVtiOcrProcessor stateManager)
		{
			var confirmedBlockPositions = new List<int>();

			foreach (CalibratedBlockPosition blockPosition in m_CalibratedPositons)
			{
				int prevMatchLocation = -1;
				int pendingLocation = -1;
				int pendingRating = int.MaxValue;

				for (int i = 0; i < stateManager.LastBlockOffsetsX; i++)
				{
					// Try to match each learned digit to every position 
					int rating = 0;
					char ch = OcrBlock(stateManager, blockPosition.Image, i, out rating);
					if (ch != ' ')
					{
						if (prevMatchLocation == -1 ||
							i == prevMatchLocation + 1)
						{
							// Sequential matches
							if (pendingRating > rating)
							{
								pendingRating = rating;
								pendingLocation = i;
							}

							prevMatchLocation = i;
						}

						Trace.WriteLine(string.Format("IOTA-VTI OCR: Recognized '{0}' at position {1} with score {2}.", ch, i, rating));
					}
					else
					{
						if (pendingLocation > 0)
							// Save the location
							confirmedBlockPositions.Add(pendingLocation);

						prevMatchLocation = -1;
						pendingLocation = -1;
						pendingRating = int.MaxValue;
					}
				}
			}

			List<int> distinctPositions = confirmedBlockPositions.Distinct().ToList();
			distinctPositions.Sort();
			int idx = 0;
			int posIdx = 0;
			stateManager.BlockOffsetsX = new int[IotaVtiOcrProcessor.MAX_POSITIONS];
			for (int i = 0; i < IotaVtiOcrProcessor.MAX_POSITIONS; i++)
				stateManager.BlockOffsetsX[i] = -1;

			int[] CHAR_INDEXES = stateManager.IsTvSafeMode
				? new int[] { 3, 4, 6, 7, 9, 10, 12, 13, 14, 15, 17, 18, 19, 20, 22, 23, 24, 25, 26, 27, 28 }
				: new int[] { 1, 3, 4, 6, 7, 9, 10, 12, 13, 14, 15, 17, 18, 19, 20, 22, 23, 24, 25, 26, 27, 28 };

			if (!stateManager.IsTvSafeMode)
				// In Non TV-Safe mode, make sure we are not trying to recognize the GPS Fix character, where 'P' can be mistaken for an '8'
				distinctPositions.RemoveAll(x => x < stateManager.BlockWidth);

			while (distinctPositions.Count > 0)
			{
				int nextPos = distinctPositions[0];
				if (distinctPositions.Count > 1 && distinctPositions[1] == nextPos + 1)
				{
					int occurances0 = confirmedBlockPositions.Count(x => x == distinctPositions[0]);
					int occurances1 = confirmedBlockPositions.Count(x => x == distinctPositions[1]);
					if (occurances1 > occurances0)
						nextPos = distinctPositions[1];

					distinctPositions.RemoveAt(0);
				}

				stateManager.BlockOffsetsX[CHAR_INDEXES[posIdx]] = nextPos;
				posIdx++;
				distinctPositions.RemoveAt(0);
			}

			stateManager.BlockOffsetsX[CHAR_INDEXES[posIdx]] = stateManager.LastBlockOffsetsX;

			return true;
		}

		private void BinirizeDiffArray(List<int> diffs)
		{
			double averageValue = diffs.Average();
			for (int i = 0; i < diffs.Count; i++)
			{
				diffs[i] = diffs[i] < averageValue ? 0 : 1;
			}
		}

		private bool DigitPatternsRecognized(IotaVtiOcrProcessor stateManager, out List<CalibratedBlockPosition> normalizedPositions)
		{
			normalizedPositions = new List<CalibratedBlockPosition>();
			Trace.Assert(m_CalibratedPositons.Count == 10);

			List<int> forwardDiffs = m_CalibratedPositons
				.Select(x => GetDiffSignature(x.SecondLastFrameNoDigit, m_CalibratedPositons[0].SecondLastFrameNoDigit))
				.ToList();

			List<int> backwardDiffs = m_CalibratedPositons
				.Select(x => GetDiffSignature(x.SecondLastFrameNoDigit, m_CalibratedPositons[m_CalibratedPositons.Count - 1].SecondLastFrameNoDigit))
				.ToList();

			BinirizeDiffArray(forwardDiffs);
			BinirizeDiffArray(backwardDiffs);

			int forwardChangeIndex = forwardDiffs.FindIndex(x => x == 1);
			int backwardChangeIndex = backwardDiffs.FindIndex(x => x == 0);

			bool swapped = false;
			bool isMatch = forwardChangeIndex == backwardChangeIndex &&
						   forwardDiffs.FindIndex(forwardChangeIndex, x => x == 0) == -1 &&
						   backwardDiffs.FindIndex(backwardChangeIndex, x => x == 1) == -1;

			if (!isMatch)
			{
				// Try with swapped fields
				for (int i = 0; i < forwardDiffs.Count / 2; i++)
				{
					int tmp = forwardDiffs[2 * i];
					forwardDiffs[2 * i] = forwardDiffs[2 * i + 1];
					forwardDiffs[2 * i + 1] = tmp;

					tmp = backwardDiffs[2 * i];
					backwardDiffs[2 * i] = backwardDiffs[2 * i + 1];
					backwardDiffs[2 * i + 1] = tmp;
				}

				forwardChangeIndex = forwardDiffs.FindIndex(x => x == 1);
				backwardChangeIndex = backwardDiffs.FindIndex(x => x == 0);

				isMatch = forwardChangeIndex == backwardChangeIndex &&
						   forwardDiffs.FindIndex(forwardChangeIndex, x => x == 0) == -1 &&
						   backwardDiffs.FindIndex(backwardChangeIndex, x => x == 1) == -1;
				swapped = true;
			}

			if (isMatch)
			{

				if (swapped)
				{
					for (int i = 0; i < m_CalibratedPositons.Count / 2; i++)
					{
						normalizedPositions.Add(m_CalibratedPositons[2 * i + 1]);
						normalizedPositions.Add(m_CalibratedPositons[2 * i]);
					}
				}
				else
					normalizedPositions.AddRange(m_CalibratedPositons);

				int indexPixelPrev = forwardChangeIndex > 0 ? forwardChangeIndex - 1 : 9;
				int indexPixelNext = forwardChangeIndex < normalizedPositions.Count - 1 ? forwardChangeIndex + 1 : forwardChangeIndex - 9;
				int signalChange = normalizedPositions[forwardChangeIndex].LastFrameNoDigit.Count(x => x < 127);
				int signalPixelsPrev = normalizedPositions[indexPixelPrev].LastFrameNoDigit.Count(x => x < 127);
				int signalPixelsNext = normalizedPositions[indexPixelNext].LastFrameNoDigit.Count(x => x < 127);

				// OneAfterZero: 9 - 0 - 1
				// ZeroAfterOne: 8 - 1 - 0

				if (signalChange < signalPixelsPrev && signalChange < signalPixelsNext)
				{
					// One before Zero
					stateManager.LearnDigitPattern(normalizedPositions[indexPixelNext].LastFrameNoDigit, 0);
					stateManager.LearnDigitPattern(normalizedPositions[forwardChangeIndex].LastFrameNoDigit, 1);
					stateManager.LearnDigitPattern(normalizedPositions[indexPixelPrev].LastFrameNoDigit, 8);

					// 3 - 2 - 5 - 4 - 7 - 6 - 9 - (8 - 1 - 0) - 3 - 2 - 5 - 4 - 7 - 6  - 9 - 8

					int walkBackIndex = indexPixelPrev - 1;
					if (walkBackIndex >= 0) stateManager.LearnDigitPattern(normalizedPositions[walkBackIndex].LastFrameNoDigit, 9);
					walkBackIndex--;
					if (walkBackIndex >= 0) stateManager.LearnDigitPattern(normalizedPositions[walkBackIndex].LastFrameNoDigit, 6);
					walkBackIndex--;
					if (walkBackIndex >= 0) stateManager.LearnDigitPattern(normalizedPositions[walkBackIndex].LastFrameNoDigit, 7);
					walkBackIndex--;
					if (walkBackIndex >= 0) stateManager.LearnDigitPattern(normalizedPositions[walkBackIndex].LastFrameNoDigit, 4);
					walkBackIndex--;
					if (walkBackIndex >= 0) stateManager.LearnDigitPattern(normalizedPositions[walkBackIndex].LastFrameNoDigit, 5);
					walkBackIndex--;
					if (walkBackIndex >= 0) stateManager.LearnDigitPattern(normalizedPositions[walkBackIndex].LastFrameNoDigit, 2);
					walkBackIndex--;
					if (walkBackIndex >= 0) stateManager.LearnDigitPattern(normalizedPositions[walkBackIndex].LastFrameNoDigit, 3);

					int walkForwardIndex = indexPixelNext + 1;
					if (walkForwardIndex < 10) stateManager.LearnDigitPattern(normalizedPositions[walkForwardIndex].LastFrameNoDigit, 3);
					walkForwardIndex++;
					if (walkForwardIndex < 10) stateManager.LearnDigitPattern(normalizedPositions[walkForwardIndex].LastFrameNoDigit, 2);
					walkForwardIndex++;
					if (walkForwardIndex < 10) stateManager.LearnDigitPattern(normalizedPositions[walkForwardIndex].LastFrameNoDigit, 5);
					walkForwardIndex++;
					if (walkForwardIndex < 10) stateManager.LearnDigitPattern(normalizedPositions[walkForwardIndex].LastFrameNoDigit, 4);
					walkForwardIndex++;
					if (walkForwardIndex < 10) stateManager.LearnDigitPattern(normalizedPositions[walkForwardIndex].LastFrameNoDigit, 7);
					walkForwardIndex++;
					if (walkForwardIndex < 10) stateManager.LearnDigitPattern(normalizedPositions[walkForwardIndex].LastFrameNoDigit, 6);
					walkForwardIndex++;
					if (walkForwardIndex < 10) stateManager.LearnDigitPattern(normalizedPositions[walkForwardIndex].LastFrameNoDigit, 9);

					SetupSixEightNineThreeDiffs(stateManager);

					stateManager.SwapFieldsOrder = true;

					return true;
				}
				else if (signalPixelsNext < signalChange && signalPixelsNext < signalPixelsPrev)
				{
					stateManager.LearnDigitPattern(normalizedPositions[forwardChangeIndex].LastFrameNoDigit, 0);

					// One after Zero
					int nextDigitIndex = indexPixelNext;
					for (int i = 0; i < 9; i++)
					{
						stateManager.LearnDigitPattern(normalizedPositions[nextDigitIndex].LastFrameNoDigit, 1 + i);

						nextDigitIndex++;
						if (nextDigitIndex > 9) nextDigitIndex -= 10;
					}

					stateManager.SwapFieldsOrder = false;

					SetupSixEightNineThreeDiffs(stateManager);

					return true;
				}
				else
					return false;
			}

			return false;
		}

		private uint[] XorPatterns(uint[] pattern1, uint[] pattern2, out int numDifferentPixels)
		{
			uint[] rv = new uint[pattern1.Length];
			numDifferentPixels = 0;

			for (int i = 0; i < pattern1.Length; i++)
			{
				if (pattern1[i] < 127 && pattern2[i] < 127)
				{
					rv[i] = 255;
				}
				else if (pattern1[i] > 127 && pattern2[i] > 127)
				{
					rv[i] = 255;
				}
				else
				{
					rv[i] = 0;
					numDifferentPixels++;
				}
			}

			return rv;
		}
		private void SetupSixEightNineThreeDiffs(IotaVtiOcrProcessor stateManager)
		{
			uint[] eightPattern = stateManager.EightDigitPattern;
			int numDifferentPixels;
			uint[] xoredPattern;

			uint[] sixPattern = stateManager.SixDigitPattern;
			xoredPattern = XorPatterns(eightPattern, sixPattern, out numDifferentPixels);
			stateManager.SixEightXorPattern = xoredPattern;
			stateManager.SixEightXorPatternFactor = numDifferentPixels;

			uint[] ninePattern = stateManager.NineDigitPattern;
			xoredPattern = XorPatterns(eightPattern, ninePattern, out numDifferentPixels);
			stateManager.NineEightXorPattern = xoredPattern;
			stateManager.NineEightXorPatternFactor = numDifferentPixels;

			uint[] threePattern = stateManager.ThreeDigitPattern;
			xoredPattern = XorPatterns(eightPattern, threePattern, out numDifferentPixels);
			stateManager.ThreeEightXorPattern = xoredPattern;
			stateManager.ThreeEightXorPatternFactor = numDifferentPixels;
		}

		private bool RecognizedTimestampsConsistent(IotaVtiOcrProcessor stateManager, List<CalibratedBlockPosition> normalizedPositions)
		{
			var allTimeStamps = new List<IotaVtiTimeStamp>();
			int index = 0;
			int totalTimestamps = normalizedPositions.Count;

			for (; ; )
			{
				if (index == 0 && normalizedPositions[0].FrameNo != normalizedPositions[1].FrameNo)
				{
					Trace.Assert(false);
					index++;
					continue;
				}

				if (index == totalTimestamps - 1)
					break;

				IotaVtiTimeStampStrings timeStampStrings = IotaVtiOcrCalibratedState.OcrField(normalizedPositions[index].Image, stateManager);
				if (!timeStampStrings.AllCharsPresent())
				{
					return false;
				}

				var timeStamp = new IotaVtiTimeStamp(timeStampStrings);

				if (stateManager.SwapFieldsOrder)
				{
					if (index + 1 == totalTimestamps - 1)
						break;

					IotaVtiTimeStampStrings timeStampStrings2 = IotaVtiOcrCalibratedState.OcrField(normalizedPositions[index + 1].Image, stateManager);
					if (!timeStampStrings2.AllCharsPresent())
					{
						return false;
					}
					var timeStamp2 = new IotaVtiTimeStamp(timeStampStrings2);
					allTimeStamps.Add(timeStamp2);

					index++;
				}

				allTimeStamps.Add(timeStamp);

				index++;
			}

			float fieldDurationMS = 0;

			for (int i = 0; i < allTimeStamps.Count - 1; i++)
			{
				if (allTimeStamps[i].FrameNumber != allTimeStamps[i + 1].FrameNumber - 1 &&
					allTimeStamps[i].FrameNumber != allTimeStamps[i + 1].FrameNumber + 1)
					return false;

				int totalMillisecondsThis = (allTimeStamps[i].Hours * 3600 + allTimeStamps[i].Minutes * 60 + allTimeStamps[i].Seconds) * 10000 + allTimeStamps[i].Milliseconds10;
				int totalMillisecondsNext = (allTimeStamps[i + 1].Hours * 3600 + allTimeStamps[i + 1].Minutes * 60 + allTimeStamps[i + 1].Seconds) * 10000 + allTimeStamps[i + 1].Milliseconds10;

				fieldDurationMS = Math.Abs((totalMillisecondsNext - totalMillisecondsThis) / 10f);

				if (Math.Abs(fieldDurationMS - IotaVtiOcrProcessor.FIELD_DURATION_PAL) > 0.1 && Math.Abs(fieldDurationMS - IotaVtiOcrProcessor.FIELD_DURATION_NTSC) > 0.1)
					return false;
			}

			if (Math.Abs(fieldDurationMS - IotaVtiOcrProcessor.FIELD_DURATION_PAL) < 0.1)
				stateManager.VideoFormat = VideoFormat.PAL;
			else if (Math.Abs(fieldDurationMS - IotaVtiOcrProcessor.FIELD_DURATION_NTSC) < 0.1)
				stateManager.VideoFormat = VideoFormat.NTSC;
			else
				stateManager.VideoFormat = null;

			return true;
		}
	}
}
