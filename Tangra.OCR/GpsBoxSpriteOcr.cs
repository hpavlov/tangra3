using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Tangra.Model.Helpers;
using Tangra.Model.Image;
using Tangra.Model.VideoOperations;
using Tangra.OCR.GpsBoxSprite;
using Tangra.OCR.Model;

namespace Tangra.OCR
{
    public class GpsBoxSpriteOcr : ITimestampOcr
    {
        private IVideoController m_VideoController;
        private TimestampOCRData m_InitializationData;

        private Dictionary<string, uint[]> m_CalibrationImages = new Dictionary<string, uint[]>();
        private List<string> m_CalibrationErrors = new List<string>();
        private uint[] m_LatestFrameImage;

        internal GpsBoxSpriteOcrProcessor m_Processor;

        public string NameAndVersion()
        {
            return "GPS-BOX-SPRITE v1.0";
        }

        public string OSDType()
        {
            return "GPS-BOX-SPRITE";
        }

        private int m_ImageWidth;
        private int m_ImageHeight;

        public void Initialize(TimestampOCRData initializationData, IVideoController videoController, int performanceMode)
        {
			m_InitializationData = initializationData;
		    m_VideoController = videoController;
            m_Processor = null;

            m_ImageWidth = initializationData.FrameWidth;
            m_ImageHeight = initializationData.FrameHeight;
        }

        public bool ExtractTime(int frameNo, int frameStep, uint[] data, out DateTime time)
        {
            if (m_VideoController != null)
                // Leave this to capture statistics
                m_VideoController.RegisterExtractingOcrTimestamps();

            m_LatestFrameImage = data;

            // TODO: Add main OCR Implementation HERE


            // NOTE: if there is an error extracting the timestamp then call m_VideoController.RegisterOcrError(); for Tangra to show a counter in the status bar with number of errors so far
            // NOTE: This dummy implementation simply marks all frames as OCR errors
            if (m_VideoController != null)
                m_VideoController.RegisterOcrError();

            // NOTE: See IotaVtiOrcManaged.ExtractDateTime() for ideas about checking incorrectly extracted times and attempting to correct them.

            time = DateTime.MinValue;
            return false;
        }

        public bool RequiresCalibration
        {
            get { return true; }
        }

        public bool ProcessCalibrationFrame(int frameNo, uint[] data)
        {
            // If RequiresCalibration returns true, Tangra will be calling ProcessCalibrationFrame() either until
            // this method returns 'true' or until InitiazliationError returns a non null value, which will be considered 
            // as failure to recognize the timestamp/calibrate and in this case OCR will not be run with the measurements

            // NOTE: In case of a calibration error Tangra will offer the end user to submit an error report
            // All images returned by GetCalibrationReportImages() and all errors returned by GetCalibrationErrors()
            // will be included in this error report. See the IotaVtiOcrManaged implementation for how to add calibration images to the dictionary

            if (m_Processor == null)
                TryInitializeProcessor(data);

            if (m_Processor == null)
                return false;

            return true;
        }

        private uint[] PreProcessImageForOCR(uint[] data)
        {
            string error = null;
            var rv = PreProcessImageForOCR(data, m_InitializationData.FrameWidth, m_InitializationData.FrameHeight, ref error);
            
            if (error != null) InitiazliationError = error;
            return rv;
        }

        public static void PrepareOsdArea(uint[] dataIn, uint[] dataOut, uint[] dataDebugNoLChD, int width, int height)
        {
            //DebugPrepareOsdArea(dataIn, dataOut, width, height);
            //return;

            // Split into fields only in the region where IOTA-VTI could be, Then threat as two separate images, and for each of them do:
            // 1) Gaussian blur (small) BitmapFilter.LOW_PASS_FILTER_MATRIX
            // 2) Sharpen BitmapFilter.SHARPEN_MATRIX
            // 3) Binarize - get Average, all below change to 0, all above change to Max (256)
            // 4) De-noise BitmapFilter.DENOISE_MATRIX

            uint median = dataIn.Median();
            for (int i = 0; i < dataIn.Length; i++)
            {
                int darkCorrectedValue = (int)dataIn[i] - (int)median;
                if (darkCorrectedValue < 0) darkCorrectedValue = 0;
                dataIn[i] = (uint)darkCorrectedValue;
            }

            uint[] blurResult = BitmapFilter.GaussianBlur(dataIn, 8, width, height);

            uint average = 128;
            uint[] sharpenResult = BitmapFilter.Sharpen(blurResult, 8, width, height, out average);

            // Binerize and Inverse
            for (int i = 0; i < sharpenResult.Length; i++)
            {
                sharpenResult[i] = sharpenResult[i] > average ? (uint)0 : (uint)255;
            }

            uint[] denoised = BitmapFilter.Denoise(sharpenResult, 8, width, height, out average, false);

            for (int i = 0; i < denoised.Length; i++)
            {
                dataOut[i] = denoised[i] < 127 ? (uint)0 : (uint)255;
            }

            Array.Copy(dataOut, dataDebugNoLChD, dataOut.Length);

            
            LargeChunkDenoiser.RemoveSmallHeightNoise(dataOut, width, height, 15);
        }

        public static uint[] PreProcessImageForOCR(uint[] data, int width, int height, ref string error)
        {
            uint[] preProcessedPixels = new uint[data.Length];
            Array.Copy(data, preProcessedPixels, data.Length);

            // Process the image
            uint median = preProcessedPixels.Median();
            for (int i = 0; i < preProcessedPixels.Length; i++)
            {
                int darkCorrectedValue = (int)preProcessedPixels[i] - (int)median;
                if (darkCorrectedValue < 0) darkCorrectedValue = 0;
                preProcessedPixels[i] = (uint)darkCorrectedValue;
            }

            if (median > 250)
            {
                error = "The background is too bright.";
                return null;
            }

            uint[] blurResult = BitmapFilter.GaussianBlur(preProcessedPixels, 8, width, height);
            uint average = 128;
            uint[] sharpenResult = BitmapFilter.Sharpen(blurResult, 8, width, height, out average);

            // Binerize and Inverse
            for (int i = 0; i < sharpenResult.Length; i++)
            {
                sharpenResult[i] = sharpenResult[i] > average ? (uint)0 : (uint)255;
            }
            uint[] denoised = BitmapFilter.Denoise(sharpenResult, 8, width, height, out average, false);

            for (int i = 0; i < denoised.Length; i++)
            {
                preProcessedPixels[i] = denoised[i] < 127 ? (uint)0 : (uint)255;
            }

            LargeChunkDenoiser.RemoveSmallHeightNoise(preProcessedPixels, width, height, 15);
            for (int i = 0; i < preProcessedPixels.Length; i++)
            {
                preProcessedPixels[i] = preProcessedPixels[i] < 127 ? (uint)255 : (uint)0;
            }
            return preProcessedPixels;
        }

        private void TryInitializeProcessor(uint[] data)
        {
            InitiazliationError = null;

            if (m_Processor == null)
            {
                var locatedLines = LocateTimestampPosition(data);
                if (locatedLines != null)
                {
                    // TODO: Identify the digit boxes configuration in regards to GPX Box Sprite OSD position and configuration


                    //m_FieldAreaHeight = (m_ToLine - m_FromLine) / 2;
                    //m_FieldAreaWidth = m_InitializationData.FrameWidth;
                    //m_OddFieldPixels = new uint[m_InitializationData.FrameWidth * m_FieldAreaHeight];
                    //m_EvenFieldPixels = new uint[m_InitializationData.FrameWidth * m_FieldAreaHeight];
                    //m_OddFieldPixelsPreProcessed = new uint[m_InitializationData.FrameWidth * m_FieldAreaHeight];
                    //m_EvenFieldPixelsPreProcessed = new uint[m_InitializationData.FrameWidth * m_FieldAreaHeight];
                    //m_OddFieldPixelsDebugNoLChD = new uint[m_InitializationData.FrameWidth * m_FieldAreaHeight];
                    //m_EvenFieldPixelsDebugNoLChD = new uint[m_InitializationData.FrameWidth * m_FieldAreaHeight];

                    //m_InitializationData.OSDFrame.Width = m_FieldAreaWidth;
                    //m_InitializationData.OSDFrame.Height = m_FieldAreaHeight;

                    m_Processor = new GpsBoxSpriteOcrProcessor(locatedLines);
                }
            }
        }

        private List<OsdLineConfig> LocateTimestampPosition(uint[] rawData)
        {
            var scoresTop = new List<int>();
            var scoresBottom = new List<int>();
            var scoreLines = new List<int>();

            var data = PreProcessImageForOCR(rawData);

            int minWhiteLevel = (int)((255 + data.Median())/ 2);

            for (int y = m_ImageHeight / 2; y < m_ImageHeight - 1; y++)
            {
                int scoreTop = 0;
                int scoreBot = 0;
                for (int x = 0; x < m_ImageWidth / 2; x++)
                {
                    var top3Pixel = data[x + (y - 3) * m_ImageWidth];
                    var top2Pixel = data[x + (y - 2) * m_ImageWidth];
                    var topPixel = data[x + (y - 1) * m_ImageWidth];
                    var currPixel = data[x + y * m_ImageWidth];
                    var botPixel = data[x + (y + 1) * m_ImageWidth];
                    var bot2Pixel = data[x + (y + Math.Min(2, m_ImageHeight - y - 1)) * m_ImageWidth];
                    var bot3Pixel = data[x + (y + Math.Min(3, m_ImageHeight - y - 1)) * m_ImageWidth];

                    if (topPixel < currPixel && topPixel < botPixel && topPixel < bot2Pixel && topPixel < bot3Pixel && currPixel > minWhiteLevel && botPixel > minWhiteLevel && bot2Pixel > minWhiteLevel && bot3Pixel > minWhiteLevel) scoreTop--;
                    if (botPixel < currPixel && botPixel < topPixel && botPixel < top2Pixel && botPixel < top3Pixel && currPixel > minWhiteLevel && topPixel > minWhiteLevel && top2Pixel > minWhiteLevel && top3Pixel > minWhiteLevel) scoreBot--;
                }

                scoresTop.Add(scoreTop);
                scoresBottom.Add(scoreBot);
                scoreLines.Add(y);
            }

            var bestTopLines = scoreLines.ToArray();
            var bestTopLineScores = scoresTop.ToArray();
            var bestBotLines = scoreLines.ToArray();
            var bestBotLineScores = scoresBottom.ToArray();

            Array.Sort(bestTopLineScores, bestTopLines);
            Array.Sort(bestBotLineScores, bestBotLines);

            var tops = new Dictionary<int, int>();
            var bottoms = new Dictionary<int, int>();

            for (int i = 0; i < MAX_TOP_BOTTOM_EDGES_TO_CONSIDER; i++)
            {
                tops.Add(bestTopLines[i], bestTopLineScores[i]);
                bottoms.Add(bestBotLines[i], bestBotLineScores[i]);
            }

            foreach (var topLineNo in tops.Keys.ToArray())
            {
                if (tops.ContainsKey(topLineNo) && tops.ContainsKey(topLineNo + 1))
                {
                    tops[topLineNo] += tops[topLineNo + 1];
                    tops.Remove(topLineNo + 1);
                }
            }

            foreach (var botLineNo in bottoms.Keys.ToArray())
            {
                if (bottoms.ContainsKey(botLineNo) && bottoms.ContainsKey(botLineNo - 1))
                {
                    bottoms[botLineNo] += bottoms[botLineNo - 1];
                    bottoms.Remove(botLineNo - 1);
                }
            }

            foreach (var tk in tops.Keys.ToArray()) tops[tk] = Math.Abs(tops[tk]);
            foreach (var bk in bottoms.Keys.ToArray()) bottoms[bk] = Math.Abs(bottoms[bk]);

            var topScoreThreashold = tops.Values.Max() / 2;
            var botScoreThreashold = bottoms.Values.Max() / 2;

            var topCandidates = tops.Where(kvp => kvp.Value > topScoreThreashold).Select(kvp => kvp.Key).ToArray();
            var botCandidates = bottoms.Where(kvp => kvp.Value > botScoreThreashold).Select(kvp => kvp.Key).ToArray();

            var pairs = new Dictionary<int, List<Tuple<int, int>>>();

            foreach (var potTop in topCandidates)
            {
                foreach (var potBot in botCandidates)
                {
                    var distancePix = potBot - potTop;
                    if (distancePix > MIN_VIABLE_HEIGHT_PIXELS)
                    {
                        List<Tuple<int, int>> exEntry;
                        if (!pairs.TryGetValue(distancePix, out exEntry) &&
                            !pairs.TryGetValue(distancePix - 1, out exEntry) &&
                            !pairs.TryGetValue(distancePix + 1, out exEntry))
                        {
                            exEntry = new List<Tuple<int, int>>();
                            pairs[distancePix] = exEntry;
                        }

                        exEntry.Add(Tuple.Create(potTop, potBot));
                    }
                }
            }

            var candidates = pairs.Where(kvp => kvp.Value.Count > 1).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);


            var detectedOsdLines = new List<OsdLineConfig>();
            var subPixelData = new SubPixelImage(data, m_ImageWidth, m_ImageHeight);

            foreach (var pair in candidates.Values)
            {
                foreach (var ln in pair)
                {
                    var osdLineConfig = LocateCharacterPositionsOnALine(subPixelData, ln.Item1, ln.Item2, minWhiteLevel);
                    if (osdLineConfig != null) detectedOsdLines.Add(osdLineConfig);
                }
            }

            Dictionary<List<decimal>, List<OsdLineConfig>> weightedConfigs = new Dictionary<List<decimal>, List<OsdLineConfig>>();

            // Combine osdPairs in buckets based on Top position and then pick the highest score ones
            foreach (var cfg in detectedOsdLines)
            {
                var bucketKeys = new List<decimal>();
                var bucketData = new List<OsdLineConfig>();

                foreach (var keys in weightedConfigs.Keys)
                {
                    if (keys.Max(x => Math.Abs(cfg.Top - x)) < cfg.BoxWidth / 2)
                    {
                        bucketKeys = keys;
                        bucketData = weightedConfigs[keys];
                        break;
                    }
                }

                bucketKeys.Add(cfg.Top);
                bucketData.Add(cfg);
                weightedConfigs[bucketKeys] = bucketData;
            }

            var selectedOsdLines = new List<OsdLineConfig>();
            foreach (var kvp in weightedConfigs)
            {
                var maxScore = kvp.Value.Max(x => x.FullnessScore);
                selectedOsdLines.Add(kvp.Value.FirstOrDefault(x => x.FullnessScore == maxScore));
            }

            return selectedOsdLines;
        }

        private OsdLineConfig LocateCharacterPositionsOnALine(SubPixelImage subPixelData, int lineTop, int lineBottom, int minWhiteLevel)
        {
            int m_MinBlockWidth = 5;
            int m_MaxBlockWidth = 16;

            int height = (lineBottom - lineTop);

            decimal nomWidthFromHeight = 13.0M * height / 32.0M; // NOTE: Nominal ratio of 13x32 from a sample image
            decimal minBlockWidth = Math.Max(m_MinBlockWidth, nomWidthFromHeight * 0.6M);
            decimal maxBlockWidth = Math.Min(m_MaxBlockWidth, nomWidthFromHeight * 1.3M);

            int probeWidth = (int)(minBlockWidth / 2);

            decimal startLeftPos = 0;
            decimal endRightPos = m_ImageWidth - maxBlockWidth - 1;
            #region Find start end horizontal lines
            List<double> meas = new List<double>();
            for (int i = 0; i < m_ImageWidth - maxBlockWidth - probeWidth; i++)
            {
                double lineSum = 0;
                for (int y = lineTop; y < lineBottom; y++)
                {
                    for (int j = i; j < i + probeWidth; j++)
                    {
                        lineSum += (double)subPixelData.GetWholePixelAt(j, y);
                    }
                }

                if (i > 3 * minBlockWidth)
                {
                    var ave = meas.Average();
                    var sigma = Math.Sqrt(meas.Select(x => (x - ave) * (x - ave)).Sum() / (meas.Count - 1));
                    var adjMeas = meas.Where(x => x > ave - 3 * sigma).ToArray();
                    ave = adjMeas.Length > 0 ? adjMeas.Average() : ave;
                    sigma = Math.Sqrt(adjMeas.Select(x => (x - ave) * (x - ave)).Sum() / (adjMeas.Length - 1));
                    if (lineSum > ave + 6 * sigma)
                    {
                        startLeftPos = i - (minBlockWidth / 2);
                        break;
                    }
                }

                meas.Add(lineSum);
            }

            meas.Clear();
            for (int i = 0; i < m_ImageWidth - maxBlockWidth - probeWidth; i++)
            {
                double lineSum = 0;
                for (int y = lineTop; y < lineBottom; y++)
                {
                    for (int j = i; j < i + probeWidth; j++)
                    {
                        lineSum += (double)subPixelData.GetWholePixelAt(m_ImageWidth - j, y);
                    }
                }

                if (i > 3 * minBlockWidth)
                {
                    var ave = meas.Average();
                    var sigma = Math.Sqrt(meas.Select(x => (x - ave) * (x - ave)).Sum() / (meas.Count - 1));
                    var adjMeas = meas.Where(x => x > ave - 3 * sigma).ToArray();
                    ave = adjMeas.Length > 0 ? adjMeas.Average() : ave;
                    sigma = Math.Sqrt(adjMeas.Select(x => (x - ave) * (x - ave)).Sum() / (adjMeas.Length - 1));
                    if (lineSum > ave + 3 * sigma)
                    {
                        endRightPos = m_ImageWidth - i + minBlockWidth;
                        break;
                    }
                }

                meas.Add(lineSum);
            }
            #endregion

            // Find boxes with continous white points somewhere on the horizontal
            List<Tuple<decimal, decimal>> boxes = new List<Tuple<decimal, decimal>>();
            bool brightPixelFound = false;
            decimal boxStart = -1;
            for (decimal x = startLeftPos; x <= endRightPos; x++)
            {
                bool brightOnVertical = false;
                for (int y = lineTop; y < lineBottom; y++)
                {
                    if (subPixelData.GetWholePixelAt(x, y) > minWhiteLevel)
                    {
                        brightOnVertical = true;
                        break;
                    }
                }

                if (brightOnVertical)
                {
                    if (!brightPixelFound)
                    {
                        boxStart = x;
                        brightPixelFound = true;
                    }
                }
                else
                {
                    if (brightPixelFound)
                    {
                        brightPixelFound = false;
                        if (x - boxStart >= minBlockWidth && x - boxStart <= maxBlockWidth)
                            boxes.Add(Tuple.Create(boxStart, x));
                    }
                }
            }

            if (boxes.Count < 2) return null;

            var lstWidths = new List<decimal>();
            for (int i = 0; i < boxes.Count - 1; i++)
            {
                if (boxes[i + 1].Item1 - boxes[i].Item2 < MAX_GAP_PIXELS)
                    lstWidths.Add(boxes[i + 1].Item1 - boxes[i].Item1);
            }

            if (lstWidths.Count == 0) return null;

            var widthAve = lstWidths.Average();
            var boxesFrom = boxes.Select(x => x.Item1).Min();
            var boxesTo = boxes.Select(x => x.Item1).Max();
            var numBoxPositions = (int)Math.Round((boxesTo - boxesFrom) / widthAve);

            // Try to locate the gaps between
            var data = new Dictionary<Tuple<decimal, decimal>, decimal>();
            for (decimal width = widthAve - 2 * subPixelData.Step; width <= widthAve + 2 * subPixelData.Step; width += subPixelData.Step)
            {
                for (decimal startOffset = boxesFrom - 2 * subPixelData.Step; startOffset <= boxesFrom + 2 * subPixelData.Step; startOffset += subPixelData.Step)
                {
                    decimal lineScore = 0;
                    for (int i = 0; i < numBoxPositions; i++)
                    {
                        decimal x = startOffset + i * width;
                        for (int y = lineTop; y < lineBottom; y++)
                        {
                            lineScore += subPixelData.GetWholePixelAt(x, y);
                        }
                    }

                    data.Add(Tuple.Create(width, startOffset), lineScore);
                }
            }

            var arrKeys = data.Keys.ToArray();
            var arrWeights = data.Values.ToArray();
            Array.Sort(arrWeights, arrKeys);

            decimal boxesLeft = arrKeys[0].Item2;
            decimal boxWidth = Math.Round(arrKeys[0].Item1);
            var fullness = new Dictionary<int, decimal>();

            for (int i = 0; i < (m_ImageWidth - boxesLeft) / boxWidth; i++)
            {
                decimal boxFullness = 0M;
                for (int j = 0; j < boxWidth; j++)
                {
                    var x = boxesLeft + (i * boxWidth) + j;
                    for (int y = lineTop; y < lineBottom; y++)
                    {
                        boxFullness += subPixelData.GetWholePixelAt(x, y) > minWhiteLevel ? 1 : 0;
                    }
                }
                fullness.Add(i, boxFullness / (boxWidth * (lineBottom - lineTop)));
            }

            var rv = new OsdLineConfig()
            {
                Top = lineTop,
                Bottom = lineBottom,
                Left = arrKeys[0].Item2,
                BoxWidth = arrKeys[0].Item1,
                BoxIndexes = fullness.Where(kvp => kvp.Value > MIN_DIGIT_BOX_FULLNESS_PERCENTAGE).Select(kvp => kvp.Key).ToArray(),
                FullnessScore = fullness.Where(kvp => kvp.Value > MIN_DIGIT_BOX_FULLNESS_PERCENTAGE).Select(kvp => kvp.Value).Sum()
            };

            return rv;
        }

        // NOTE: This may be a good number if the timestamp is one one or two lines
        //       but what if the timestamp is on 3 or 4 lines ??
        private static int MAX_TOP_BOTTOM_EDGES_TO_CONSIDER = 15;
        private static int MIN_VIABLE_HEIGHT_PIXELS = 7;
        private static decimal MAX_GAP_PIXELS = 3.0M;
        private static decimal MIN_DIGIT_BOX_FULLNESS_PERCENTAGE = 0.25M;

        public Dictionary<string, uint[]> GetCalibrationReportImages()
        {
            return m_CalibrationImages;
        }

        public List<string> GetCalibrationErrors()
        {
            return m_CalibrationErrors;
        }

        public uint[] GetLastUnmodifiedImage()
        {
            return m_LatestFrameImage;
        }

        public TimestampOCRData InitializationData
        {
            get { return m_InitializationData; }
        }

        public void DrawLegend(Graphics graphics)
        {
            if (m_Processor != null)
                m_Processor.DrawLegend(graphics);
        }

        public string InitiazliationError { get; private set; }
    }
}
