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
    public class CalibratedBlockPosition
    {
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


    public class IotaVtiOcrCalibratingState : IotaVtiOcrState
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

                    for (int x = 0; x < stateManager.CurrentImageWidth; x++)
                    {
                        if (x >= stateManager.CurrentImageWidth || x < 0) continue;
                        if (top < 0) continue;
                        if (bottom >= stateManager.CurrentImageHeight) continue;

                        if (bottomOk && stateManager.CurrentImage[x + imageWidth * (bottom - 1)] < 127) totalRating++;
                        if (bottom2Ok && stateManager.CurrentImage[x + imageWidth * bottom] > 127) totalRating++;

						if (stateManager.CurrentImage[x + imageWidth * (top + 1)] < 127) totalRating++;
						if (stateManager.CurrentImage[x + imageWidth * top] > 127) totalRating++;
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
                        if (//stateManager.CurrentImage[x - 1 + imageWidth*y] > 127 &&
                            //stateManager.CurrentImage[x + imageWidth*y] > 127 &&
                            stateManager.CurrentImage[x + 1 + imageWidth*y] < 127)
                        {
	                        startingPositions.Add(x);
                            totalRating++;

                            if (stateManager.CurrentImage[x + width - 1 + imageWidth*y] < 127 &&
                                stateManager.CurrentImage[x + width + imageWidth*y] > 127 &&
                                stateManager.CurrentImage[x + width + 1 + imageWidth*y] > 127)
                            {
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

			var calibrationBlock = new CalibratedBlockPosition()
			{
				BlockWidth = bestWidth,
				BlockHeight = bestHeight,
				BlockOffsetY = bestYOffs,
				Image = stateManager.CurrentImage,
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
            m_MinBlockHeight = (int)Math.Round(0.66 * m_Height);
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

        public override void Process(IotaVtiOcrProcessor stateManager, Graphics graphics)
        {
            if (m_Width != stateManager.CurrentImageWidth || m_Height != stateManager.CurrentImageHeight)
            {
                Reinitialise(stateManager.CurrentImageWidth, stateManager.CurrentImageHeight);
				ROUTH_START_FRAME_NUMBER_BLOCKS  = (int)Math.Round(555.0 * stateManager.CurrentImageWidth / 720);
            }

            CalibrateBlockPositonsTop(stateManager);
            CalibrateBlockPositonsWidth(stateManager);

			if (m_CalibratedPositons.Count == 10)
			{
				if (CalibrateFrameNumberBlockPositions(stateManager))
				{
                    stateManager.ChangeState<IotaVtiOcrCalibratedState>();
                    stateManager.Process(stateManager.CurrentImage, stateManager.CurrentImageWidth, stateManager.CurrentImageHeight, graphics);
                    return;
				}
				else
				{
                    m_CalibratedPositons.RemoveAt(0);
                    m_CalibratedPositons.RemoveAt(0);
				}
			}

            if (graphics != null)
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
                                stateManager.BlockOffsetY,
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
                            stateManager.BlockOffsetY,
                            m_Width,
                            stateManager.BlockHeight);
                }
            }
        }

		private bool CalibrateFrameNumberBlockPositions(IotaVtiOcrProcessor stateManager)
		{
            List<int> bestStartPositions = new List<int>();

            foreach (CalibratedBlockPosition blockPosition in m_CalibratedPositons)
                bestStartPositions.AddRange(blockPosition.BestStartingPositions.Where(x => x >= ROUTH_START_FRAME_NUMBER_BLOCKS));

            List<int> uniqueStartingPositionsList = bestStartPositions.Distinct().ToList();
            List<int> occurancesList = new List<int>(new int[uniqueStartingPositionsList.Count]);

            #region Populate the occurances, merging at the same time all starting positions that differ by one
            uniqueStartingPositionsList.Sort();
            int index = uniqueStartingPositionsList.Count - 1;

            while (index >= 0)
			{
                if (index > 0 &&
                    uniqueStartingPositionsList[index] - 1 == uniqueStartingPositionsList[index - 1])
                {
                    // Two sequential starting positions. Merge them (assume the smaller as the correct one)
                    occurancesList[index] =
                        bestStartPositions.Count(x => x == uniqueStartingPositionsList[index]) +
                        bestStartPositions.Count(x => x == uniqueStartingPositionsList[index - 1]);

                    occurancesList.RemoveAt(index - 1);
                    uniqueStartingPositionsList.RemoveAt(index); // We leave the smaller of the two as the accepted starting position
                    index--;
                }
                else
                    occurancesList[index] = bestStartPositions.Count(x => x == uniqueStartingPositionsList[index]);

			    index--;
            }
            #endregion

		    int[] occurances = occurancesList.ToArray();
		    int[] uniqueStartingPositions = uniqueStartingPositionsList.ToArray();

            #region Remove all starting positions that are less than full width from each side of the best position guesses
            var blockStartingPositions = new List<int>();
            for (int i = uniqueStartingPositions.Length - 1; i >= 0; i--)
            {
                int suggestedPos = uniqueStartingPositions[i];
                if (occurances[i] > 30.0 * stateManager.BlockHeight / 14.0)
                    blockStartingPositions.Add(suggestedPos);
            }

            for (int i = uniqueStartingPositions.Length - 1; i >=0; i--)
            {
                bool removeCandidate = false;
                if (occurances[i] < 10)
                    removeCandidate = true;
                else if (blockStartingPositions.Any(x => x != uniqueStartingPositions[i] && Math.Abs(x - uniqueStartingPositions[i]) < stateManager.BlockWidth))
                    removeCandidate = true;

                if (removeCandidate)
                    occurances[i] = -1;
            }
            #endregion

            Array.Sort(occurances, uniqueStartingPositions);

            blockStartingPositions.Clear();
			for (int i = uniqueStartingPositions.Length - 1; i >= 0; i--)
			{
				int suggestedPos = uniqueStartingPositions[i];
                if (occurances[i] > 0)
					blockStartingPositions.Add(suggestedPos);
			}

			if (blockStartingPositions.Count > 1)
			{
				// Group those that are within 3 pixels
				blockStartingPositions.Sort();
				int last = blockStartingPositions[blockStartingPositions.Count - 1];
				int secondLast = blockStartingPositions[blockStartingPositions.Count - 2];

				// We now know the position of the two digits. We can now 'learn' the digits from '0' to '9' finding the change of the second last digit
                // and then infering the values from '0' to '9' of the last digit

                foreach (CalibratedBlockPosition blockPosition in m_CalibratedPositons)
                    blockPosition.PrepareLastTwoDigitsFromTheFrameNumber(stateManager, last, secondLast);

				if (DigitPatternsRecognized(stateManager))
				{
				    stateManager.LastBlockOffsetsX = last;
                    stateManager.SecondLastBlockOffsetsX = secondLast;
                    // Now go and determine the positions of the remaining blocks by matching their value to the 'learned' digits
				    return LocateRemainingBlockPositions(stateManager);
				}
			}

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
                    // TODO: Try to match each learned digit to every position 
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
                        Trace.WriteLine(string.Format("Found '{0}' at position {1} with rating {2}", ch, i, rating));
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

            int[] CHAR_INDEXES = new int[] { 1, 3, 4, 6, 7, 9, 10, 12, 13, 14, 15, 17, 18, 19, 20, 22, 23, 24, 25, 26, 27, 28 };

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

            return false;
        }

        private void BinirizeDiffArray(List<int> diffs)
        {
            double averageValue = diffs.Average();
            for (int i = 0; i < diffs.Count; i++)
            {
                diffs[i] = diffs[i] < averageValue ? 0 : 1;
            }
        }

        private bool DigitPatternsRecognized(IotaVtiOcrProcessor stateManager)
        {
            var normalizedPositions = new List<CalibratedBlockPosition>();
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

                    return true;
                }
                else
                    return false;
            }

            return false;
        }
    }
}
