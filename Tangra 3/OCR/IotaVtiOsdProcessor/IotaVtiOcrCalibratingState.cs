using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tangra.Model.Image;
using Tangra.OCR.IotaVtiOsdProcessor;

namespace Tangra.OCR.IotaVtiOsdProcessor
{
    public class CalibratedBlockPosition
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

        public uint[] Image;

        public uint[] LastFrameNoDigit;
        public uint[] SecondLastFrameNoDigit;

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
    }


    public class IotaVtiOcrCalibratingState : IotaVtiOcrState
    {
        private List<CalibratedBlockPosition> m_CalibratedPositons = new List<CalibratedBlockPosition>();

        private int RateBlock(uint[] pixels, int imageWidth, int imageHeight, Rectangle block)
        {
            int rv = 0;
            bool leftOk = (block.Left - 1) > 0 && (block.Left - 1) < imageWidth;
			bool left2Ok = block.Left > 0 && block.Left < imageWidth;
            bool bottomOk = (block.Bottom - 1) < imageHeight;
            bool bottom2Ok = block.Bottom < imageHeight;
            bool topOk = block.Top - 1 >= 0;

            int totalWhiteWhite = 0;
            for (int y = block.Top; y < block.Bottom; y++)
            {
				if (y >= imageHeight || y < 0) continue;

                if (leftOk && left2Ok)
                {
                    if (pixels[block.Left - 1 + imageWidth * y] > 127 && pixels[block.Left + imageWidth * y] < 127) rv += 2;
                    else if (pixels[block.Left - 1 + imageWidth * y] > 127 && pixels[block.Left + imageWidth * y] > 127)
                        totalWhiteWhite++;
                }
            }
            if (totalWhiteWhite == block.Bottom - block.Top)
                rv += 2 * (block.Bottom - block.Top);

            for (int x = block.Left; x < block.Right; x++)
            {
                if (x >= imageWidth || x < 0) continue;
	            if (block.Top < 0) continue;

                if (bottomOk && pixels[x + imageWidth * (block.Bottom - 1)] < 127) rv++;
                if (bottom2Ok && pixels[x + imageWidth * block.Bottom] > 127) rv++;
                if (topOk && pixels[x + imageWidth * (block.Top - 1)] > 127) rv++;
                if (pixels[x + imageWidth * block.Top] < 127) rv++;
            }

            return rv;
        }

        private void CalibrateBlockPositons(IotaVtiOcrProcessor stateManager, int frameNo, bool isOddField)
        {
            int count = 0;
            int maxRating = -1;

            int bestXOffs = -1;
            int bestYOffs = -1;
            int bestWidth = -1;
            int bestHeight = -1;

            for (int xOffs = -4; xOffs < 8; xOffs++)
            {
                for (int yOffs = -5; yOffs < 5; yOffs++)
                {
                    for (int x = m_MinBlockWidth; x <= m_MaxBlockWidth; x++)
                    {
                        for (int y = m_MinBlockHeight; y <= m_MaxBlockHeight; y++)
                        {
                            int totalRating = 0;
                            Rectangle rect;

                            for (int i = 0; i < IotaVtiOcrProcessor.MAX_POSITIONS; i++)
                            {
                                rect = new Rectangle(xOffs + i * x, yOffs, x, y);
                                totalRating += RateBlock(stateManager.CurrentImage, stateManager.CurrentImageWidth, stateManager.CurrentImageHeight, rect);
                            }

                            if (totalRating > maxRating)
                            {
                                maxRating = totalRating;
                                bestXOffs = xOffs;
                                bestYOffs = yOffs;
                                bestWidth = x;
                                bestHeight = y;
                            }

                            count++;
                        }
                    }
                }
            }

            stateManager.BlockWidth = bestWidth;
            stateManager.BlockHeight = bestHeight;
            stateManager.BlockOffsetX = bestXOffs;
            stateManager.BlockOffsetY = bestYOffs;

            m_CalibratedPositons.Add(
                new CalibratedBlockPosition(stateManager.CurrentImage)
                {
                    BlockWidth = bestWidth,
                    BlockHeight = bestHeight,
                    BlockOffsetX = bestXOffs,
                    BlockOffsetY = bestYOffs,
                    FrameNo = frameNo,
                    IsOddField = isOddField
                });
        }

        private void Reinitialise(int width, int height)
        {
            m_Width = width;
            m_Height = height;

            m_MaxBlockWidth = m_Width / 27;
            m_MinBlockWidth = m_Width / 29;
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

        public override void Process(IotaVtiOcrProcessor stateManager, Graphics graphics, int frameNo, bool isOddField)
        {
            if (m_Width != stateManager.CurrentImageWidth || m_Height != stateManager.CurrentImageHeight)
            {
                Reinitialise(stateManager.CurrentImageWidth, stateManager.CurrentImageHeight);
            }

            if (m_CalibratedPositons.Count >= 10)
            {
                if (CheckBlockPositions(stateManager))
                {
                    var normalizedPositions = new List<CalibratedBlockPosition>();
                    if (DigitPatternsRecognized(stateManager, out normalizedPositions))
					{
                        if (RecognizedTimestampsConsistent(stateManager, normalizedPositions))
						{
							stateManager.ChangeState<IotaVtiOcrCalibratedState>();
							stateManager.Process(stateManager.CurrentImage, stateManager.CurrentImageWidth, stateManager.CurrentImageHeight, graphics, frameNo, isOddField);
							return;							
						}
					}
                }

                // Make sure we always remove the whole frame, so swapped fields create less issues when learning the characters
                m_CalibratedPositons.RemoveAt(0);
                m_CalibratedPositons.RemoveAt(0);
            }

            CalibrateBlockPositons(stateManager, frameNo, isOddField);

            if (graphics != null)
            {
                for (int i = 0; i < IotaVtiOcrProcessor.MAX_POSITIONS; i++)
                {
                    graphics.DrawRectangle(Pens.Chartreuse, i * stateManager.BlockWidth + stateManager.BlockOffsetX, stateManager.BlockOffsetY, stateManager.BlockWidth, stateManager.BlockHeight);
                }
            }
        }

        private bool CheckBlockPositions(IotaVtiOcrProcessor stateManager)
        {
            int widthMedian = m_CalibratedPositons.Select(x => x.BlockWidth).ToList().Median();
            int heightMedian = m_CalibratedPositons.Select(x => x.BlockHeight).ToList().Median();
            int offsetXMedian = m_CalibratedPositons.Select(x => x.BlockOffsetX).ToList().Median();
            int offsetYMedian = m_CalibratedPositons.Select(x => x.BlockOffsetY).ToList().Median();

            int numItemsWithDifferentValues = m_CalibratedPositons
                .Count(x =>
                       x.BlockWidth != widthMedian ||
                       x.BlockHeight != heightMedian ||
                       x.BlockOffsetX != offsetXMedian ||
                       x.BlockOffsetY != offsetYMedian);

            if (numItemsWithDifferentValues <= 3)
            {
                stateManager.BlockWidth = widthMedian;
                stateManager.BlockHeight = heightMedian;
                stateManager.BlockOffsetX = offsetXMedian;
                stateManager.BlockOffsetY = offsetYMedian;
            }

            int[] arrLastDigitPos  = m_CalibratedPositons.Select(x => x.GetLastFrameNoDigitPosition(stateManager)).ToArray();
            Array.Sort(arrLastDigitPos);

            if (arrLastDigitPos[0] == arrLastDigitPos[arrLastDigitPos.Length - 1] && // All positions in all frames must be the same ...
                arrLastDigitPos[0] != -1 && // recognized position ...
                arrLastDigitPos[0] > IotaVtiOcrProcessor.FIRST_FRAME_NO_DIGIT_POSITIONS + 2) // and should have at least 2 digits in the frame number
            {
                stateManager.LastFrameNoDigitPosition = arrLastDigitPos[0] - 1;
                return true;
            }
            else
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

        private bool DigitPatternsRecognized(IotaVtiOcrProcessor stateManager, out List<CalibratedBlockPosition> normalizedPositions)
        {
            normalizedPositions = new List<CalibratedBlockPosition>();
            Trace.Assert(m_CalibratedPositons.Count == 10);

            foreach (CalibratedBlockPosition blockPosition in m_CalibratedPositons)
                blockPosition.PrepareLastTwoDigitsFromTheFrameNumber(stateManager);

            List<int> forwardDiffs = m_CalibratedPositons
                .Select(x => GetDiffSignature(x.SecondLastFrameNoDigit, m_CalibratedPositons[0].SecondLastFrameNoDigit))
                .ToList();

            List<int> backwardDiffs = m_CalibratedPositons
                .Select(x => GetDiffSignature(x.SecondLastFrameNoDigit, m_CalibratedPositons[m_CalibratedPositons.Count - 1].SecondLastFrameNoDigit))
                .ToList();

            BinirizeDiffArray(forwardDiffs);
            BinirizeDiffArray(backwardDiffs);

            int forwardChangeIndex = forwardDiffs.FindIndex(x => x == 1);
            int backwardChangeIndex = backwardDiffs.FindIndex(x=> x == 0);

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
                    forwardDiffs[2 * i] =  forwardDiffs[2 * i + 1];
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

        private bool RecognizedTimestampsConsistent(IotaVtiOcrProcessor stateManager, List<CalibratedBlockPosition> normalizedPositions)
		{
			var allTimeStamps = new List<IotaVtiTimeStamp>();
			int index = 0;
            int totalTimestamps = normalizedPositions.Count;

			for (;;)
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
					return false;

				var timeStamp = new IotaVtiTimeStamp(timeStampStrings);

				if (stateManager.SwapFieldsOrder)
				{
					if (index + 1 == totalTimestamps - 1)
						break;

                    IotaVtiTimeStampStrings timeStampStrings2 = IotaVtiOcrCalibratedState.OcrField(normalizedPositions[index + 1].Image, stateManager);
					if (!timeStampStrings2.AllCharsPresent())
						return false;
					
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
