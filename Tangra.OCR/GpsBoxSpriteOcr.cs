using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using Tangra.Model.Helpers;
using Tangra.Model.Image;
using Tangra.Model.VideoOperations;
using Tangra.OCR.GpsBoxSprite;
using Tangra.OCR.Model;
using Tangra.OCR.TimeExtraction;

namespace Tangra.OCR
{
    public class GpsBoxSpriteOcr : ITimestampOcr, ICalibrationErrorProcessor
    {
        private IVideoController m_VideoController;
        private TimestampOCRData m_InitializationData;

        private Dictionary<string, Tuple<uint[], int, int>> m_CalibrationImages = new Dictionary<string, Tuple<uint[], int, int>>();
        private List<string> m_CalibrationErrors = new List<string>();
        private uint[] m_LatestFrameImage;

        internal GpsBoxSpriteOcrProcessor m_Processor;

        private VtiTimeStampComposer m_TimeStampComposer;

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
        private int m_MinCalibrationFrames;

        private bool m_ForceErrorReport;

        public void Initialize(TimestampOCRData initializationData, IVideoController videoController, int performanceMode)
        {
			m_InitializationData = initializationData;
		    m_VideoController = videoController;
            m_Processor = null;

            m_ImageWidth = initializationData.FrameWidth;
            m_ImageHeight = initializationData.FrameHeight;

            m_MinCalibrationFrames = (int)Math.Ceiling(m_InitializationData.VideoFrameRate);

            m_ForceErrorReport = initializationData.ForceErrorReport;
            m_CalibrationImages.Clear();
            m_CalibrationErrors.Clear();
        }

        public bool ExtractTime(int frameNo, int frameStep, uint[] rawData, bool debug, out DateTime time)
        {
            LastOddFieldOSD = null;
            LastEvenFieldOSD = null;

            if (m_Processor.IsCalibrated && m_TimeStampComposer != null)
            {
                if (m_VideoController != null)
                    m_VideoController.RegisterExtractingOcrTimestamps();

                // NOTE: Image needs to be split and processed separately, otherwise the 

                var data = PreProcessImageForOCR(frameNo, rawData, m_TopMostLine, m_BottomMostLine);

                m_Processor.Process(data, frameNo, debug);
                LastOddFieldOSD = m_Processor.CurrentOcredOddFieldTimeStamp;
                LastEvenFieldOSD = m_Processor.CurrentOcredEvenFieldTimeStamp;
                string failedReason;

                if (m_InitializationData.IntegratedAAVFrames > 0)
                {
                    time = m_TimeStampComposer.ExtractAAVDateTime(frameNo, frameStep, LastOddFieldOSD, LastEvenFieldOSD, out failedReason);
                    LastFailedReason = failedReason;
                }
                else
                {
                    time = m_TimeStampComposer.ExtractDateTime(frameNo, frameStep, LastOddFieldOSD, LastEvenFieldOSD, out failedReason);
                    LastFailedReason = failedReason;
                }
            }
            else
                time = DateTime.MinValue;

            return time != DateTime.MinValue;
        }

        public IVtiTimeStamp LastOddFieldOSD { get; private set; }
        public IVtiTimeStamp LastEvenFieldOSD { get; private set; }
        public string LastFailedReason { get; private set; }

        public Bitmap GetOCRDebugImage()
        {
            return m_Processor != null 
                ? m_Processor.GetDebugBitmap() 
                : null;
        }

        public Bitmap GetOCRCalibrationDebugImage()
        {
            return m_Processor != null
                ? m_Processor.GetBlockSignaturesImage()
                : null;
        }

        public bool RequiresCalibration
        {
            get { return true; }
        }

        private Tuple<int, int>[] m_OsdLineVerticals;
        private Tuple<decimal, decimal>[] m_OsdBlocksLeftAndWidth;

        private List<OcrCalibrationFrame> m_CalibrationFrames = new List<OcrCalibrationFrame>();
 
        public bool ProcessCalibrationFrame(int frameNo, uint[] data)
        {
            // If RequiresCalibration returns true, Tangra will be calling ProcessCalibrationFrame() either until
            // this method returns 'true' or until InitiazliationError returns a non null value, which will be considered 
            // as failure to recognize the timestamp/calibrate and in this case OCR will not be run with the measurements

            // NOTE: In case of a calibration error Tangra will offer the end user to submit an error report
            // All images returned by GetCalibrationReportImages() and all errors returned by GetCalibrationErrors()
            // will be included in this error report. See the IotaVtiOcrManaged implementation for how to add calibration images to the dictionary


            var calibFrame = ProcessCalibrationFrame(frameNo, data, m_OsdLineVerticals, m_OsdBlocksLeftAndWidth);
            m_CalibrationFrames.Add(calibFrame);

            if (m_CalibrationFrames.Count > 1 && m_OsdLineVerticals == null)
            {
                var linesSoFar = m_CalibrationFrames.SelectMany(x => x.DetectedOsdLines);
                var numOsdLinesTester = m_CalibrationFrames.Select(x => x.DetectedOsdLines.Length).ToList().GroupBy(x => x).ToDictionary(x => x.Key, y => y.ToArray());
                var numOsdLines = numOsdLinesTester.Keys.Max();
                if (numOsdLines > 1)
                {
                    // NOTE: At least 2 samples to decide on Top/Bottom line positions
                    var medianConfig = DeterimineMedianOsdLinePositions(linesSoFar, ref numOsdLines);
                    if (medianConfig.BoxTops.Length == numOsdLines && medianConfig.BoxBottoms.Length == numOsdLines)
                    {
                        var topsSorted = medianConfig.BoxTops.ToList();
                        var bottomsSorted = medianConfig.BoxBottoms.ToList();
                        topsSorted.Sort();                        
                        bottomsSorted.Sort();

                        m_OsdLineVerticals = new Tuple<int, int>[numOsdLines];
                        for (int i = 0; i < numOsdLines; i++)
                        {
                            m_OsdLineVerticals[i] = Tuple.Create((int)topsSorted[i], (int)bottomsSorted[i]);
                        }
                    }                    
                }
            }

            if (m_CalibrationFrames.Count > 4 && m_OsdLineVerticals != null && m_OsdBlocksLeftAndWidth == null)
            {
                // NOTE: At least 5 samples to decide on Left/Width of lines
                var linesSoFar = m_CalibrationFrames.SelectMany(x => x.DetectedOsdLines).ToArray();
                m_OsdBlocksLeftAndWidth = new Tuple<decimal, decimal>[m_OsdLineVerticals.Length];

                for (int i = 0; i < m_OsdLineVerticals.Length; i++)
                {
                    var leftList = linesSoFar.Where(x => Math.Abs(x.Top - m_OsdLineVerticals[i].Item1) < 2).Select(x => x.Left).ToList();
                    var widthList = linesSoFar.Where(x => Math.Abs(x.Top - m_OsdLineVerticals[i].Item1) < 2).Select(x => x.BoxWidth).ToList();

                    if (leftList.Count > 4)
                    {
                        // Outlier handling. Remove the bigest and smallest values
                        leftList.Sort();
                        leftList = leftList.Skip(1).Take(leftList.Count - 2).ToList();

                        widthList.Sort();
                        widthList = widthList.Skip(1).Take(widthList.Count - 2).ToList();
                    }
                    var averageLeft = leftList.Average();
                    var averageWidth = widthList.Average();

                    var leftSigma = Math.Sqrt((double)leftList.Select(x => (x - averageLeft) * (x - averageLeft)).Sum() / (leftList.Count - 1));
                    var widthSigma = Math.Sqrt((double)widthList.Select(x => (x - averageWidth) * (x - averageWidth)).Sum() / (leftList.Count - 1));

                    if (leftSigma < 0.33 && widthSigma < 0.1)
                    {
                        m_OsdBlocksLeftAndWidth[i] = Tuple.Create(averageLeft, averageWidth);
                    }
                    else
                    {
                        // Failed to get the left/width for one of the lines
                        m_OsdBlocksLeftAndWidth = null;
                        break;
                    }
                }
            }

            if (m_OsdLineVerticals != null && m_OsdBlocksLeftAndWidth != null)
            {
                if (m_Processor == null)
                    TryInitializeProcessor();
                else
                    m_Processor.ProcessCalibrationFrame(calibFrame);

                if (m_Processor != null && m_Processor.IsCalibrated)
                {
                    m_TimeStampComposer = new VtiTimeStampComposer(m_Processor.VideoFormat, m_InitializationData.IntegratedAAVFrames, m_Processor.EvenBeforeOdd, m_Processor.DuplicatedFields, m_VideoController, this, () => m_Processor.CurrentImage);
                    return true;
                }
                    
            }

            return false;
        }

        public void PrepareFailedCalibrationReport()
        {
            // NOTE: Add calibration frames OSD position data here if required
            if (m_Processor != null)
                m_Processor.PrepareCalibrationFailedReport(this);
        }

        private uint[] PreProcessImageForOCR(int frameNo, uint[] data, int? topMostLine, int? bottomMostLine)
        {
            m_LatestFrameImage = data;

            if (topMostLine != null && bottomMostLine != null)
            {
                // NOTE: Top and bottom most lines are known. Only pre-process the video frame lines with the VTI-OSD
                uint[] rv = new uint[data.Length];
                int top = Math.Max(0, topMostLine.Value - 5);
                int bottom = Math.Max(m_InitializationData.FrameHeight, bottomMostLine.Value + 5);
                uint[] roi = new uint[m_InitializationData.FrameWidth * (bottom - top)];
                int idx = 0;
                for (int i = top * m_InitializationData.FrameWidth; i < bottom * m_InitializationData.FrameWidth; i++, idx++)
                {
                    roi[idx] = data[i];
                }

                var averageDetectedOsdHeight = m_OsdLineVerticals.Select(x => (x.Item2 - x.Item1)).Average();

                string error = null;
                idx = 0;
                var pproc = PrepareOsdVideoFields(frameNo, roi, m_InitializationData.FrameWidth, bottom - top, (int)Math.Round(0.8 * averageDetectedOsdHeight), ref error);
                for (int i = top * m_InitializationData.FrameWidth; i < bottom * m_InitializationData.FrameWidth; i++, idx++)
                {
                    rv[i] = pproc[idx];
                }

                if (error != null) InitiazliationError = error;
                return rv;
            }
            else
            {
                string error = null;
                // Based on an experimentally determined nominal height of 15 pixels for a standard PAL frame height of 576
                var expectedScaledOsdHeight = (int)Math.Round(15.0 * 576 / m_InitializationData.FrameHeight);
                var rv = PreProcessImageOSDForOCR(data, m_InitializationData.FrameWidth, m_InitializationData.FrameHeight, expectedScaledOsdHeight, ref error);

                if (error != null) InitiazliationError = error;
                return rv;
            }
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

        private uint[] PrepareOsdVideoFields(int frameNo, uint[] data, int width, int height, int minFrameOsdDigitHeight, ref string error)
        {
            // TODO: This is suboptimal. Need to make the OCR work with the two separate fields from the start
            var fieldHeight = (int)Math.Ceiling(height / 2.0);
            uint[] oddFieldPixels = new uint[width * fieldHeight];
            uint[] evenFieldPixels = new uint[width * fieldHeight];
            for (int y = 0; y < height; y++)
            {
                bool isOddLine = y % 2 == 0; // y is zero-based so odd/even calculations should be performed counting from zero
                int lineNo = y / 2;
                if (isOddLine)
                    Array.Copy(data, y * width, oddFieldPixels, lineNo * width, width);
                else
                    Array.Copy(data, y * width, evenFieldPixels, lineNo * width, width);
            }

            var preProcessedOddPixels = PreProcessImageOSDForOCR(oddFieldPixels, width, fieldHeight, minFrameOsdDigitHeight / 2, ref error);
            var preProcessedEvenPixels = PreProcessImageOSDForOCR(evenFieldPixels, width, fieldHeight, minFrameOsdDigitHeight / 2, ref error);


            if (m_ForceErrorReport || (m_Processor != null && !m_Processor.IsCalibrated))
            {
                AddErrorImage(string.Format(@"{0}-even.bmp", frameNo.ToString("0000000")), preProcessedEvenPixels, width, fieldHeight);
                AddErrorImage(string.Format(@"{0}-odd.bmp", frameNo.ToString("0000000")), preProcessedOddPixels, width, fieldHeight);

                AddErrorImage(string.Format(@"ORG-{0}-even.bmp", frameNo.ToString("0000000")), evenFieldPixels, width, fieldHeight);
                AddErrorImage(string.Format(@"ORG-{0}-odd.bmp", frameNo.ToString("0000000")), oddFieldPixels, width, fieldHeight);
            }

            var preProcessedPixels = new uint[width * height];
            for (int y = 0; y < height; y++)
            {
                bool isOddLine = (y - 1) % 2 == 1; // y is zero-based so odd/even calculations should be performed counting from zero
                int lineNo = y / 2;
                if (isOddLine)
                    Array.Copy(preProcessedOddPixels, lineNo * width, preProcessedPixels, y * width, width);
                else
                    Array.Copy(preProcessedEvenPixels, lineNo * width, preProcessedPixels, y * width, width);
            }

            return preProcessedPixels;
        }

        public static uint[] PreProcessImageOSDForOCR(uint[] data, int width, int height, int maxNoiseHeight, ref string error)
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

            LargeChunkDenoiser.RemoveSmallHeightNoise(preProcessedPixels, width, height, maxNoiseHeight);
            for (int i = 0; i < preProcessedPixels.Length; i++)
            {
                preProcessedPixels[i] = preProcessedPixels[i] < 127 ? (uint)255 : (uint)0;
            }
            return preProcessedPixels;
        }

        private void TryInitializeProcessor()
        {
            InitiazliationError = null;

            if (m_Processor == null)
            {
                if (m_OsdLineVerticals == null || m_OsdBlocksLeftAndWidth == null)
                {
                    InitiazliationError = "Cannot detect timestamp position.";
                    return;
                }

                try
                {
                    m_Processor = new GpsBoxSpriteOcrProcessor(m_CalibrationFrames, m_OsdLineVerticals, m_OsdBlocksLeftAndWidth, m_ImageWidth, m_ImageHeight);
                }
                catch (Exception ex)
                {
                    m_Processor = null;
                    InitiazliationError = ex.Message;
                }
            }
        }
        
        private class OsdLinePositions
        {
            public decimal BoxHeight;
            public decimal BoxWidth;
            public decimal BoxLeft;
            public decimal[] BoxTops;
            public decimal[] BoxBottoms;
        }

        private OsdLinePositions DeterimineMedianOsdLinePositions(IEnumerable<OsdLineConfig> allLineConfigs, ref int numOsdLines)
        {
            var rv = new OsdLinePositions();

            rv.BoxHeight = allLineConfigs.Select(x => x.BoxHeight).ToList().Median();
            rv.BoxWidth = allLineConfigs.Select(x => x.BoxWidth).ToList().Median();

            var topGroups = new Dictionary<decimal, List<decimal>>();
            var bottomGroups = new Dictionary<decimal, List<decimal>>();
            var leftGroups = new Dictionary<decimal, List<decimal>>();
            foreach (var osdLineCfg in allLineConfigs)
            {
                var topList = new List<decimal>();
                foreach (var key in topGroups.Keys)
                {
                    if (Math.Abs(key - osdLineCfg.Top) < rv.BoxHeight / 3)
                    {
                        topList = topGroups[key];
                        topGroups.Remove(key);
                        break;
                    }
                }

                topList.Add(osdLineCfg.Top);
                decimal newKey = topList.Median();
                topGroups[newKey] = topList;

                var bottomList = new List<decimal>();
                foreach (var key in bottomGroups.Keys)
                {
                    if (Math.Abs(key - osdLineCfg.Bottom) < rv.BoxHeight / 3)
                    {
                        bottomList = bottomGroups[key];
                        bottomGroups.Remove(key);
                        break;
                    }
                }

                bottomList.Add(osdLineCfg.Bottom);
                newKey = bottomList.Median();
                bottomGroups[newKey] = bottomList;

                var leftList = new List<decimal>();
                foreach (var key in leftGroups.Keys)
                {
                    if (Math.Abs(key - osdLineCfg.Left) < rv.BoxWidth / 3)
                    {
                        leftList = leftGroups[key];
                        leftGroups.Remove(key);
                        break;
                    }
                }

                leftList.Add(osdLineCfg.Left);
                newKey = leftList.Median();
                leftGroups[newKey] = leftList;
            }

            rv.BoxLeft = leftGroups.Keys.Min();
            var boxTops = new List<decimal>();
            var topWeights = topGroups.Values.Select(x => x.Count).ToArray();
            var topVals = topGroups.Keys.ToArray();
            var boxBottoms = new List<decimal>();
            var bottomWeights = bottomGroups.Values.Select(x => x.Count).ToArray();
            var bottomVals = bottomGroups.Keys.ToArray();

            numOsdLines = Math.Min(Math.Min(topVals.Length, bottomVals.Length), numOsdLines);

            Array.Sort(topWeights, topVals);
            for (int i = 1; i <= numOsdLines; i++)
            {
                var topVal = topVals[topVals.Length - i];
                boxTops.Add(topVal);
            }

            Array.Sort(bottomWeights, bottomVals);
            for (int i = 1; i <= numOsdLines; i++)
            {
                var bottomVal = bottomVals[bottomVals.Length - i];
                boxBottoms.Add(bottomVal);
            }

            rv.BoxTops = boxTops.ToArray();
            rv.BoxBottoms = boxBottoms.ToArray();
            return rv;
        }

        private Dictionary<int, List<Tuple<int, int>>> DetermineOsdLineVerticals(uint[] data, int minWhiteLevel)
        {
            var scoresTop = new List<int>();
            var scoresBottom = new List<int>();
            var scoreLines = new List<int>();

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
            return candidates;
        }

        private int? m_TopMostLine = null;
        private int? m_BottomMostLine = null;

        private OcrCalibrationFrame ProcessCalibrationFrame(int frameNo, uint[] rawData, Tuple<int, int>[] osdLineVerticals, Tuple<decimal, decimal>[] osdLineLeftAndWidths)
        {
            m_TopMostLine = null;
            m_BottomMostLine = null;

            if (osdLineVerticals != null && osdLineVerticals.Length > 0)
            {
                m_TopMostLine = osdLineVerticals.Select(x => x.Item1).Min();
                m_BottomMostLine = osdLineVerticals.Select(x => x.Item2).Max();
            }

            var data = PreProcessImageForOCR(frameNo, rawData, m_TopMostLine, m_BottomMostLine);

            int minWhiteLevel = 127; //PreProcessed image will only have 0 and 255 pixels, so we always use 127 here

            var subPixelData = new SubPixelImage(data, m_ImageWidth, m_ImageHeight);

            var selectedOsdLines = new List<OsdLineConfig>();

            if (osdLineVerticals == null || osdLineVerticals.Length == 0)
            {
                var detectedOsdLines = new List<OsdLineConfig>();            
                var candidates = DetermineOsdLineVerticals(data, minWhiteLevel);

                foreach (var pair in candidates.Values)
                {
                    var confirmedOsdLinesInFrame = new List<Tuple<decimal, decimal, decimal, decimal>>();
                    foreach (var ln in pair)
                    {
                        var leftRightLocation = ConfirmOsdLinePresence(subPixelData, ln.Item1, ln.Item2, minWhiteLevel);
                        if (leftRightLocation != null) confirmedOsdLinesInFrame.Add(Tuple.Create((decimal)ln.Item1, (decimal)ln.Item2, leftRightLocation.Item1, leftRightLocation.Item2));
                    }
                    if (confirmedOsdLinesInFrame.Count > 0)
                    {
                        var osdLineConfigs = RefineOsdLinePositions(subPixelData, confirmedOsdLinesInFrame.ToArray(), minWhiteLevel);
                        if (osdLineConfigs != null) detectedOsdLines.AddRange(osdLineConfigs);
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
                
                foreach (var kvp in weightedConfigs)
                {
                    var maxScore = kvp.Value.Max(x => x.FullnessScore);
                    selectedOsdLines.Add(kvp.Value.FirstOrDefault(x => x.FullnessScore == maxScore));
                }
            }
            else if (osdLineLeftAndWidths == null)
            {
                var confirmedOsdLinesInFrame = new List<Tuple<decimal, decimal, decimal, decimal>>();
                foreach (var osdLine in osdLineVerticals)
                {
                    var leftRightLocation = ConfirmOsdLinePresence(subPixelData, osdLine.Item1, osdLine.Item2, minWhiteLevel);
                    if (leftRightLocation != null) confirmedOsdLinesInFrame.Add(Tuple.Create((decimal)osdLine.Item1, (decimal)osdLine.Item2, leftRightLocation.Item1, leftRightLocation.Item2));
                }
                if (confirmedOsdLinesInFrame.Count > 0)
                {
                    var osdLineConfigs = RefineOsdLinePositions(subPixelData, confirmedOsdLinesInFrame.ToArray(), minWhiteLevel);
                    if (osdLineConfigs != null) selectedOsdLines.AddRange(osdLineConfigs);                    
                }
            }
            else
            {
                selectedOsdLines = new List<OsdLineConfig>();
            }

            return new OcrCalibrationFrame()
            {
                DetectedOsdLines = selectedOsdLines.ToArray(),
                RawPixels = rawData,
                ProcessedPixels = data
            };
        }

        private Tuple<decimal, decimal> ConfirmOsdLinePresence(SubPixelImage subPixelData, int lineTop, int lineBottom, int minWhiteLevel)
        {
            int m_MinBlockWidth = MIN_BLOCK_WIDTH;
            int m_MaxBlockWidth = MAX_BLOCK_WIDTH;

            int height = (lineBottom - lineTop);

            decimal nomWidthFromHeight = 13.0M * height / 32.0M; // NOTE: Nominal ratio of 13x32 from a sample image
            decimal minBlockWidth = Math.Max(m_MinBlockWidth, nomWidthFromHeight * 0.6M);
            decimal maxBlockWidth = Math.Min(m_MaxBlockWidth, nomWidthFromHeight * 1.3M);

            int probeWidth = (int)minBlockWidth;

            decimal startLeftPos = 0;
            decimal endRightPos = m_ImageWidth - maxBlockWidth - 1;
            #region Find start end horizontal lines
            for (int i = 0; i < m_ImageWidth - maxBlockWidth - probeWidth; i++)
            {
                int conseqHorWhites = 0;
                for (int j = i; j < i + probeWidth; j++)
                {
                    for (int y = lineTop; y < lineBottom; y++)
                    {
                        if ((double)subPixelData.GetWholePixelAt(j, y) > minWhiteLevel)
                        {
                            conseqHorWhites++;
                            break;
                        }
                    }
                }

                if (conseqHorWhites == probeWidth)
                {
                    startLeftPos = i;
                    break;
                }
            }

            for (int i = 0; i < m_ImageWidth - maxBlockWidth - probeWidth; i++)
            {

                int conseqHorWhites = 0;
                for (int j = i; j < i + probeWidth; j++)
                {
                    for (int y = lineTop; y < lineBottom; y++)
                    {
                        if ((double)subPixelData.GetWholePixelAt(m_ImageWidth - j, y) > minWhiteLevel)
                        {
                            conseqHorWhites++;
                            break;
                        }
                    }
                }

                if (conseqHorWhites == probeWidth)
                {
                    endRightPos = m_ImageWidth - i + minBlockWidth;
                    break;
                }
            }
            #endregion

            #region Reject cases where insufficient number of boxes with continous vertical white is detected
            // Find boxes with continous white points somewhere on the horizontal
            // Consider more than one boxes to appear attached together without a gap
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
                        var contWidth = x - boxStart;
                        if (contWidth > maxBlockWidth)
                        {
                            var attachedBoxes = (int)(contWidth / nomWidthFromHeight);
                            var aveWidth = contWidth / attachedBoxes;
                            var rem = contWidth - (aveWidth * attachedBoxes);
                            if (aveWidth >= minBlockWidth && rem < minBlockWidth / 3)
                            {
                                for (int i = 0; i < attachedBoxes; i++)
                                {
                                    boxes.Add(Tuple.Create(boxStart + i * aveWidth, x + i * aveWidth));
                                }
                            }
                        }
                        else
                        {
                            if (contWidth >= minBlockWidth)
                                boxes.Add(Tuple.Create(boxStart, x));
                        }
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
            #endregion

            return Tuple.Create(startLeftPos, endRightPos);
        }

        private decimal DetermineBlackToWhiteTransition(Dictionary<decimal, decimal> data)
        {
            var maxScore = data.Select(x => x.Value).Max();
            var edgeScore = maxScore * PERCENT_WHITE_AT_EDGE_START; 
            foreach (var kvp in data)
            {
                if (kvp.Value > edgeScore)
                    return kvp.Key;
            }
            return data.ToArray()[data.Count / 2].Key;
        }

        private List<OsdLineConfig> RefineOsdLinePositions(SubPixelImage subPixelData, Tuple<decimal, decimal, decimal, decimal>[] osdLineConstraints, int minWhiteLevel)
        {
            var rv = new List<OsdLineConfig>();
            
            var lineScores = new Dictionary<decimal, decimal>();

            // 1) Determine the top, bottom and right positions accurately for each osd line
            foreach (var osdLine in osdLineConstraints)
            {
                decimal[] topBottomRough = new decimal[] { osdLine.Item1, osdLine.Item2 };
                decimal[] topBottomFine = new decimal[] { osdLine.Item1, osdLine.Item2 };
                int leftRnd = (int)Math.Ceiling(osdLine.Item3);
                int rightRnd = (int)Math.Floor(osdLine.Item4);
                
                for (int i = 0; i < 2; i++)
                {
                    decimal horizontal = topBottomRough[i];
                    int sign = i == 0 ? 1 : -1;
                    Func<decimal, decimal, bool> checkCond = (x, y) =>
                    {
                        return (i == 0) ? x < y : x > y;
                    };
                    lineScores.Clear();
                    for (decimal y = horizontal - 1 * sign; checkCond(y, horizontal + 1 * sign); y += 0.025m * sign)
                    {
                        decimal horScore = 0;
                        for (int x = leftRnd; x <= rightRnd; x++)
                        {
                            horScore += subPixelData.GetWholePixelAt(x, y);
                        }
                        lineScores.Add(y, horScore);
                    }

                    topBottomFine[i] = DetermineBlackToWhiteTransition(lineScores);
                }

                decimal rightFine = osdLine.Item4;
                lineScores.Clear();
                for (decimal x = rightFine + 1; x > rightFine - 1; x -= 0.1m)
                {
                    decimal vertScore = 0;
                    for (int y = (int)topBottomRough[0]; y <= (int)topBottomRough[1]; y++)
                    {
                        vertScore += subPixelData.GetWholePixelAt(x, y);
                    }
                    lineScores.Add(x, vertScore);
                }
                rightFine = DetermineBlackToWhiteTransition(lineScores);

                var cfg = new OsdLineConfig()
                {
                    Top = topBottomFine[0],
                    Bottom = topBottomFine[1],
                    Right = rightFine
                };
                rv.Add(cfg);
            }


            // 2) Determine the left position accurately for all lines simultaneously
            decimal leftFine = osdLineConstraints.Select(x => x.Item3).Average();
            lineScores.Clear();
            for (decimal x = leftFine - 1; x < leftFine + 1; x += 0.025m)
            {
                decimal vertScore = 0;
                foreach (var osdLine in osdLineConstraints)
                {
                    for (int y = (int)osdLine.Item1; y <= (int)osdLine.Item2; y++)
                    {
                        vertScore += subPixelData.GetWholePixelAt(x, y);
                    }
                }
                lineScores.Add(x, vertScore);
            }
            
            leftFine = DetermineBlackToWhiteTransition(lineScores);
            rv.ForEach(x => x.Left = leftFine);

            // 3) Determine the best fitting box width for all lines
            decimal heightAve = rv.Select(x => x.Bottom - x.Top).Average();
            var lineLengths = rv.Select(x => x.Right - x.Left);
            var widthAve = 13.0M * heightAve / 32.0M; // NOTE: Nominal ratio of 13x32 from a sample image
            var arrScores = new decimal[40];
            var arrWidths = new decimal[40];
            var idx = 0;
            for (decimal i = -2; i < 2; i+=0.1m)
            {
                var test = widthAve + i;
                var score = lineLengths.Select(x => Math.Abs((x / test) - Math.Round(x / test))).Sum();
                arrScores[idx] = score;
                arrWidths[idx] = test;
                idx++;
            }
            Array.Sort(arrScores, arrWidths);
            widthAve = arrWidths[0];

            // Try to locate the gaps between
            var data = new Dictionary<decimal, decimal>();
            for (decimal width = widthAve - 2; width <= widthAve + 2; width += 0.025m)
            {
                decimal lineScore = 0;

                foreach (var cfg in rv)
                {
                    var boxesFrom = cfg.Left;
                    var boxesTo = cfg.Right;
                    var numBoxPositions = (int)Math.Round((boxesTo - boxesFrom) / width);

                    for (int i = 0; i < numBoxPositions; i++)
                    {
                        decimal x = cfg.Left + i * width;
                        for (int y = (int)Math.Ceiling(cfg.Top); y < Math.Floor(cfg.Bottom); y++)
                        {
                            lineScore += subPixelData.GetWholePixelAt(x, y);
                        }
                    }
                }

                data.Add(width, lineScore);
            }

            var arrKeys = data.Keys.ToArray();
            var arrWeights = data.Values.ToArray();
            Array.Sort(arrWeights, arrKeys);

            decimal boxesLeft = rv[0].Left;
            decimal boxWidth = arrKeys[0];

            foreach (var cfg in rv)
            {
                cfg.BoxWidth = boxWidth;

                var fullness = new Dictionary<int, decimal>();


                for (int i = -1 * (int)(boxesLeft / boxWidth); i < (m_ImageWidth - boxesLeft) / boxWidth; i++)
                {
                    decimal boxFullness = 0M;
                    for (int j = 0; j < boxWidth; j++)
                    {
                        var x = boxesLeft + (i * boxWidth) + j;
                        for (decimal y = cfg.Top; y < cfg.Bottom; y++)
                        {
                            boxFullness += subPixelData.GetWholePixelAt(x, y) > minWhiteLevel ? 1 : 0;
                        }
                    }
                    fullness.Add(i, boxFullness / (boxWidth * (cfg.Bottom - cfg.Top)));
                }

                cfg.BoxIndexes = fullness.Where(kvp => kvp.Value > MIN_DIGIT_BOX_FULLNESS_PERCENTAGE).Select(kvp => kvp.Key).ToArray();
                cfg.FullnessScore = fullness.Where(kvp => kvp.Value > MIN_DIGIT_BOX_FULLNESS_PERCENTAGE).Select(kvp => kvp.Value).Sum();
            }

            return rv;
        }

        // NOTE: This may be a good number if the timestamp is one one or two lines
        //       but what if the timestamp is on 3 or 4 lines ??
        private static int MAX_TOP_BOTTOM_EDGES_TO_CONSIDER = 15;
        private static int MIN_VIABLE_HEIGHT_PIXELS = 7;
        private static decimal MAX_GAP_PIXELS = 3.0M;
        private static decimal MIN_DIGIT_BOX_FULLNESS_PERCENTAGE = 0.15M;
        private static decimal PERCENT_WHITE_AT_EDGE_START = 0.35M;
        private static int MIN_BLOCK_WIDTH = 5;
        private static int MAX_BLOCK_WIDTH = 16;


        public Dictionary<string, Tuple<uint[], int, int>> GetCalibrationReportImages()
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

        public int ErrorCount
        {
            get { return m_CalibrationErrors.Count; }
        }

        public void AddErrorImage(string fileName, uint[] pixels, int width, int height)
        {
            m_CalibrationImages[fileName] = Tuple.Create(pixels, width, height);
        }

        public void AddErrorText(string error)
        {
            m_CalibrationErrors.Add(error);
        }
    }
}
