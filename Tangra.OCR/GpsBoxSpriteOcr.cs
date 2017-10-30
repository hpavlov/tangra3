using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using Tangra.Functional;
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

        public bool TimeStampHasDatePart
        {
            get { return true; }
        }

        private int m_ImageWidth;
        private int m_ImageHeight;
        private int m_MinCalibrationFrames;

        private bool m_ForceErrorReport;
        private bool m_SingleLineMode;

        public void Initialize(TimestampOCRData initializationData, IVideoController videoController, int performanceMode, bool optionFlag)
        {
			m_InitializationData = initializationData;
		    m_VideoController = videoController;
            m_Processor = null;
            m_SingleLineMode = optionFlag; // For GPS-BOX-SPRITE, the option flag has a meaning of a single line mode

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
                if (numOsdLines > 0)
                {
                    // NOTE: At least 2 samples to decide on Top/Bottom line positions
                    var medianConfig = DeterimineMedianOsdLinePositions(linesSoFar, ref numOsdLines);
                    if (medianConfig.BoxTops.Length == numOsdLines && medianConfig.BoxBottoms.Length == numOsdLines)
                    {
                        var topsSorted = medianConfig.BoxTops.Select(x => (int)x).ToList();
                        var bottomsSorted = medianConfig.BoxBottoms.Select(x => (int)x).ToList();
                        topsSorted.Sort();
                        bottomsSorted.Sort();

                        int[] lineHeights = new int[numOsdLines];
                        int[] lineGaps = new int[numOsdLines - 1];

                        for (int i = 0; i < numOsdLines; i++)
                        {
                            lineHeights[i] = bottomsSorted[i] - topsSorted[i];
                            if (i < numOsdLines - 1)
                                lineGaps[i] = topsSorted[i + 1] - bottomsSorted[i];
                        }

                        if (!m_SingleLineMode && numOsdLines > 1)
                        {
                            // Logic to remove 'fake' lines if they are too high. Distance between lines should be small and consistent. 
                            // Pick first two lines starting from bottom which are a certain distance apart

                            for (int i = numOsdLines - 1; i > 0; i--)
                            {
                                var height2 = lineHeights[i];
                                var height1 = lineHeights[i - 1];
                                var gap = lineGaps[i - 1];
                                if (Math.Abs(height2 - height1) < 2 &&
                                    height2 <= MAX_VIABLE_HEIGHT_PIXELS &&
                                    height1 <= MAX_VIABLE_HEIGHT_PIXELS &&
                                    gap <= MAX_VIABLE_GAP &&
                                    m_ImageHeight - bottomsSorted[i] < MAX_VIABLE_HEIGHT_PIXELS /* Bottom line must be close to the bottom of the image */)
                                {
                                    m_OsdLineVerticals = new Tuple<int, int>[2];
                                    m_OsdLineVerticals[0] = Tuple.Create(topsSorted[i - 1], bottomsSorted[i - 1]);
                                    m_OsdLineVerticals[1] = Tuple.Create(topsSorted[i], bottomsSorted[i]);
                                    break;
                                }
                            }
                        }
                        else if (m_SingleLineMode && numOsdLines == 1 && m_CalibrationFrames.Count > 4)
                        {
                            if (lineHeights[0] <= MAX_VIABLE_HEIGHT_PIXELS &&
                                m_ImageHeight - bottomsSorted[0] < MAX_VIABLE_HEIGHT_PIXELS /* Bottom line must be close to the bottom of the image */)
                            {
                                m_OsdLineVerticals = new Tuple<int, int>[1];
                                m_OsdLineVerticals[0] = Tuple.Create(topsSorted[0], bottomsSorted[0]);
                            }
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
                        
                        if (widthList.Count >= 10)
                        {
                            // If we have at least 10 samples and most are around the median, then remove outliers
                            var medianWidth = widthList.Median();
                            var medianWidthSigma = (decimal)Math.Sqrt((double)widthList.Select(x => (x - medianWidth) * (x - medianWidth)).Sum() / (leftList.Count - 1));
                            var aroundMedianList = widthList.Where(x => Math.Abs(x - medianWidth) < medianWidthSigma).ToList();
                            if (aroundMedianList.Count*1.0/widthList.Count > 0.75)
                            {
                                widthList = aroundMedianList;
                            }
                        }
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
                {
                    var goodCalibrationFrames = m_CalibrationFrames.Where(x => IsGoodCalibrationFrame(x)).ToList();
                    TryInitializeProcessor(goodCalibrationFrames);
                }
                else
                {
                    if (IsGoodCalibrationFrame(calibFrame)) 
                        m_Processor.ProcessCalibrationFrame(calibFrame);
                }
                if (m_Processor != null && m_Processor.IsCalibrated)
                {
                    m_TimeStampComposer = new VtiTimeStampComposer(m_Processor.VideoFormat, m_InitializationData.IntegratedAAVFrames, m_Processor.EvenBeforeOdd, m_Processor.DuplicatedFields, m_VideoController, this, () => m_Processor.CurrentImage);
                    return true;
                }
                    
            }

            return false;
        }

        private bool IsGoodCalibrationFrame(OcrCalibrationFrame frame)
        {
            if (m_OsdLineVerticals == null || m_OsdBlocksLeftAndWidth == null)
                return false;

            // Good calibration frame needs to contain all lines at the correct vertical position
            if (!m_OsdLineVerticals.All(cfg => frame.DetectedOsdLines.Any(x => x.Top - cfg.Item1 < x.BoxHeight / 3)))
                return false;

            var verticallyPositionedLines = frame.DetectedOsdLines.Where(x => m_OsdLineVerticals.Any(cfg => Math.Abs(x.Top - cfg.Item1) < x.BoxHeight / 3)).ToArray();

            frame.DetectedOsdLines = verticallyPositionedLines;

            // Good calibration frame needs to have its box width be very close to the accepted box width
            return verticallyPositionedLines.All(x => Math.Abs(x.BoxWidth - m_OsdBlocksLeftAndWidth[0].Item2) < 1.0M);
        }

        public void PrepareFailedCalibrationReport()
        {
            // NOTE: Add calibration frames OSD position data here if required
            //for (int i = 0; i < m_CalibrationFrames.Count; i++)
            //{
            //    decimal? top = null;
            //    decimal? bottom = null;
            //    if (m_CalibrationFrames[i].DetectedOsdLines != null &&
            //        m_CalibrationFrames[i].DetectedOsdLines.Length > 0)
            //    {
            //        top = m_CalibrationFrames[i].DetectedOsdLines.Average(x => x.Top);
            //        bottom = m_CalibrationFrames[i].DetectedOsdLines.Average(x => x.Bottom);
            //    }

            //    string fileName = Path.GetFullPath(@"F:\WORK\tangra3\OcrTester\AutomatedTestingImages\PixelExport\" + i + ".dat");
            //    using(FileStream fs = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write))
            //    using (BinaryWriter wrt = new BinaryWriter(fs))
            //    {
            //        wrt.Write(top ?? 0M);
            //        wrt.Write(bottom ?? 0M);
            //        wrt.Write(m_ImageWidth);
            //        wrt.Write(m_ImageHeight);
            //        foreach(var pix in m_CalibrationFrames[i].ProcessedPixels)
            //            wrt.Write(pix);
            //    }
            //}

            if (m_LatestRawCalibrationFrame != null)
            {
                string error = null;
                var expectedScaledOsdHeight = (int)Math.Round(15.0 * 576 / m_InitializationData.FrameHeight);
                PreProcessImageOSDForOCR(m_LatestFrameImage, m_ImageWidth, m_ImageHeight, expectedScaledOsdHeight, ref error, this);
            }

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
                // TODO: Consider locating the black background, of present, to cut out the remainder of the image.
                //LocateBlackOSDBackground(data, m_InitializationData.FrameWidth, m_InitializationData.FrameHeight);
                
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

        private void LocateBlackOSDBackground(uint[] data, int width, int height)
        {
            //// GPSBOXSPRITE may have a black background behind the OSD. We try to locate it here
            

            //// skip black area at the left of the image
            //for (int x0 = 0; x0 < width && data[x0 + width * height / 2] == 0; x0++)
            
            //for (int x = x0; x < width; x++)
            //{
            //    int startedY = 0;
            //    for (int y = height / 2; y < height; y++)
            //    {
            //        var isBlack = data[x + width * y] == 0;

            //        if (!started)
            //        {
            //            started = isBlack;
            //        }
            //        else
            //        {
                        
            //        }
            //    }    
            //}            
        }

        public static uint[] PreProcessImageOSDForOCR(uint[] data, int width, int height, int maxNoiseHeight, ref string error)
        {
            return PreProcessImageOSDForOCR(data, width, height, maxNoiseHeight, ref error, null);
        }

        private static uint[] PreProcessImageOSDForOCR(uint[] data, int width, int height, int maxNoiseHeight, ref string error, ICalibrationErrorProcessor logger)
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

            if (logger != null) logger.AddErrorImage("pp-median-corrected.bmp", preProcessedPixels.ToArray(), width, height);

            uint[] blurResult = BitmapFilter.GaussianBlur(preProcessedPixels, 8, width, height);

            if (logger != null) logger.AddErrorImage("pp-gauss-blur.bmp", blurResult.ToArray(), width, height);

            uint average = 128;
            uint[] sharpenResult = BitmapFilter.Sharpen(blurResult, 8, width, height, out average);

            if (logger != null) logger.AddErrorImage("pp-sharpened.bmp", sharpenResult.ToArray(), width, height);

            // Binerize and Inverse
            for (int i = 0; i < sharpenResult.Length; i++)
            {
                sharpenResult[i] = sharpenResult[i] > average ? (uint)0 : (uint)255;
            }
            if (logger != null) logger.AddErrorImage("pp-binerized.bmp", sharpenResult.ToArray(), width, height);

            uint[] denoised = BitmapFilter.Denoise(sharpenResult, 8, width, height, out average, false);

            if (logger != null) logger.AddErrorImage("pp-denoised.bmp", denoised.ToArray(), width, height);

            for (int i = 0; i < denoised.Length; i++)
            {
                preProcessedPixels[i] = denoised[i] < 127 ? (uint)0 : (uint)255;
            }

            LargeChunkDenoiser.RemoveSmallHeightNoise(preProcessedPixels, width, height, maxNoiseHeight);
            for (int i = 0; i < preProcessedPixels.Length; i++)
            {
                preProcessedPixels[i] = preProcessedPixels[i] < 127 ? (uint)255 : (uint)0;
            }
            if (logger != null) logger.AddErrorImage("pp-final.bmp", preProcessedPixels.ToArray(), width, height);

            return preProcessedPixels;
        }

        private void TryInitializeProcessor(List<OcrCalibrationFrame> goodCalibrationFrames)
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
                    m_Processor = new GpsBoxSpriteOcrProcessor(goodCalibrationFrames, m_OsdLineVerticals, m_OsdBlocksLeftAndWidth, m_ImageWidth, m_ImageHeight, m_SingleLineMode);
                    if (!m_Processor.InitSuccess)
                        m_Processor = null;
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

        private Tuple<int[], int[], int[], int[]> ScoreVerticalLines(uint[] data, int minWhiteLevel, Func<uint, uint, uint, uint, uint, uint, uint, Tuple<bool, bool>> scoreCallback)
        {
            var scoresTop = new List<int>();
            var scoresBottom = new List<int>();
            var scoreLines = new List<int>();
            var topLimit = m_ImageHeight / 2;

            for (int y = topLimit; y < m_ImageHeight - 1; y++)
            {
                int scoreTop = 0;
                int scoreBot = 0;
                for (int x = 0; x < m_ImageWidth/2; x++)
                {
                    var top3Pixel = data[x + (y - 3) * m_ImageWidth];
                    var top2Pixel = data[x + (y - 2) * m_ImageWidth];
                    var topPixel = data[x + (y - 1) * m_ImageWidth];
                    var currPixel = data[x + y * m_ImageWidth];
                    var botPixel = data[x + (y + 1) * m_ImageWidth];
                    var bot2Pixel = data[x + (y + Math.Min(2, m_ImageHeight - y - 1)) * m_ImageWidth];
                    var bot3Pixel = data[x + (y + Math.Min(3, m_ImageHeight - y - 1)) * m_ImageWidth];

                    var score = scoreCallback(top3Pixel, top2Pixel, topPixel, currPixel, botPixel, bot2Pixel, bot3Pixel);
                    if (score.Item1) scoreTop--;
                    if (score.Item2) scoreBot--;
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

            return Tuple.Create(bestTopLines, bestBotLineScores, bestBotLines, bestBotLineScores);
        }

        private Tuple<int[], int[]> DetermineOsdLineVerticalsEx(uint[] data, int minWhiteLevel, out Dictionary<int, int> scores)
        {
            // NOTE: data has been already pre-processed with large chunk noice removed so background is zero

            scores = new Dictionary<int, int>();
            var topLimit = m_ImageHeight / 2;

            for (int y = 0; y < m_ImageHeight - 1; y++)
            {
                scores[y] = 0;

                for (int x = 0; x < m_ImageWidth / 2; x++)
                {
                    var currPixel = data[x + y * m_ImageWidth];
                    if (currPixel > minWhiteLevel) 
                        scores[y]++;
                }
            }

            var tops = new List<int>();
            var bottoms = new List<int>();

            const int MIN_SCORE = 10; // The millis OSD row has 10 characters. If they are all 1's this gives a minimum positive score of 10

            for (int y = topLimit; y < m_ImageHeight - 1; y++)
            {
                if (scores[y] > MIN_SCORE)
                {
                    int fromY = y;
                    do
                    {
                        y++;
                    }
                    while (y < m_ImageHeight - 1 && scores.ContainsKey(y) && scores[y] > MIN_SCORE);
                    tops.Add(fromY);
                    bottoms.Add(y);
                }
            }

            return Tuple.Create(tops.ToArray(), bottoms.ToArray());
        }

        private Tuple<int[], int[]> DetermineOsdLineVerticalsOld(uint[] data, int minWhiteLevel)
        {
            var scoresTop = new List<int>();
            var scoresBottom = new List<int>();
            var scoreLines = new List<int>();
            var topLimit = m_ImageHeight / 2;

            for (int y = topLimit; y < m_ImageHeight - 1; y++)
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

            return Tuple.Create(topCandidates, botCandidates);
        }

        private Dictionary<int, List<Tuple<int, int, int>>> DetermineOsdLineVerticals(uint[] data, int minWhiteLevel)
        {
            Dictionary<int, int> scores;
            var lineProbes = DetermineOsdLineVerticalsEx(data, minWhiteLevel, out scores);
            if (lineProbes.Item1.Length < 2)
            {
                lineProbes = DetermineOsdLineVerticalsOld(data, minWhiteLevel);
            }

            var topCandidates = lineProbes.Item1;
            var botCandidates = lineProbes.Item2;

            var pairs = new Dictionary<int, List<Tuple<int, int, int>>>();

            foreach (var potTop in topCandidates)
            {
                foreach (var potBot in botCandidates)
                {
                    var distancePix = potBot - potTop;
                    if (distancePix > MIN_VIABLE_HEIGHT_PIXELS && distancePix < MAX_VIABLE_HEIGHT_PIXELS)
                    {
                        List<Tuple<int, int, int>> exEntry;
                        if (!pairs.TryGetValue(distancePix, out exEntry) &&
                            !pairs.TryGetValue(distancePix - 1, out exEntry) &&
                            !pairs.TryGetValue(distancePix + 1, out exEntry))
                        {
                            exEntry = new List<Tuple<int, int, int>>();
                            pairs[distancePix] = exEntry;
                        }

                        int score = scores.Where((kvp) => kvp.Key >= potTop && kvp.Key <= potBot).Sum(kvp => kvp.Value);
                        exEntry.Add(Tuple.Create(potTop, potBot, score));
                    }
                }
            }

            Dictionary<int, List<Tuple<int, int, int>>> candidates;
            if (m_SingleLineMode)
                candidates = pairs.Where(kvp => kvp.Value.Count == 1 && (kvp.Value.First().Item3 / (kvp.Value.First().Item2 - kvp.Value.First().Item1)) > m_ImageWidth / 10).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            else
                candidates = pairs.Where(kvp => kvp.Value.Count > 1).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            return candidates;
        }

        private int? m_TopMostLine = null;
        private int? m_BottomMostLine = null;
        private uint[] m_LatestRawCalibrationFrame;

        private OcrCalibrationFrame ProcessCalibrationFrame(int frameNo, uint[] rawData, Tuple<int, int>[] osdLineVerticals, Tuple<decimal, decimal>[] osdLineLeftAndWidths)
        {
            m_LatestRawCalibrationFrame = rawData.ToArray();

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
                #region Detect Lines
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
                #endregion
            }            
            else
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

            int rightMostProbeLocation = (int)(0.6 * m_ImageWidth); // NOTE: OSD will not extend more than 60% on the horizontal (typically)

            int startPosZero = (int)(1.5M * nomWidthFromHeight);
            decimal startLeftPos = startPosZero;
            decimal endRightPos = rightMostProbeLocation - maxBlockWidth - 1;
            #region Find start end horizontal lines
            for (int i = startPosZero; i < rightMostProbeLocation - maxBlockWidth - probeWidth; i++)
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

            for (int i = startPosZero; i < rightMostProbeLocation - maxBlockWidth - probeWidth; i++)
            {

                int conseqHorWhites = 0;
                for (int j = i; j < i + probeWidth; j++)
                {
                    for (int y = lineTop; y < lineBottom; y++)
                    {
                        if ((double)subPixelData.GetWholePixelAt(rightMostProbeLocation - j, y) > minWhiteLevel)
                        {
                            conseqHorWhites++;
                            break;
                        }
                    }
                }

                if (conseqHorWhites == probeWidth)
                {
                    endRightPos = rightMostProbeLocation - i + minBlockWidth;
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
                            var attachedBoxes = Math.Max(1, (int)(contWidth / nomWidthFromHeight));
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
            /*
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
            */

            // 3) Determine the best fitting box width for all lines
            decimal heightAve = rv.Select(x => x.Bottom - x.Top).Average();
            var widthAve = 13.0M * heightAve / 32.0M; // NOTE: Nominal ratio of 13x32 from a sample image

            /*
            if (m_SingleLineMode && rv.Count == 1)
            {
                // 24 characters in a single line mode
                widthAve = (rv[0].Right - rv[0].Left)/24.0M;
            }
            else
            {
                var lineLengths = rv.Select(x => x.Right - x.Left);
                var arrScores = new decimal[40];
                var arrWidths = new decimal[40];
                var idx = 0;
                for (decimal i = -2; i < 2; i += 0.1m)
                {
                    var test = widthAve + i;
                    var score = lineLengths.Select(x => Math.Abs((x / test) - Math.Round(x / test))).Sum();
                    arrScores[idx] = score;
                    arrWidths[idx] = test;
                    idx++;
                }
                Array.Sort(arrScores, arrWidths);
                // NOTE: Check an alternative weighted average from the top 2 widths: (arrWidths[0] * (arrScores[1] / Math.Max(0.000001M, arrScores[0])) + arrWidths[1]) / (1 + (arrScores[1] / Math.Max(0.000001M, arrScores[0])));
                widthAve = arrWidths[0];
            }

            var leftWidth = m_SingleLineMode
                ? DetermineLeftAndWidthFromBlockMatching(subPixelData, minWhiteLevel, widthAve, rv) 
                : DetermineLeftAndWidthFromGaps(subPixelData, minWhiteLevel, widthAve, rv);

            decimal boxesLeft = leftWidth.Item1;
            decimal boxWidth = leftWidth.Item2; */

            var osdFrame = new OsdFrame(m_ImageWidth, m_ImageHeight, rv[0].Top, rv[0].Bottom, subPixelData.Pixels);
            decimal boxesLeft = osdFrame.FindLeft(25);
            decimal boxWidth = osdFrame.FindWidth(boxesLeft, widthAve);
            boxesLeft = osdFrame.ImproveLeft(boxesLeft, boxWidth);

            foreach (var cfg in rv)
            {
                cfg.BoxWidth = boxWidth;
                cfg.Left = boxesLeft;

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

        private Tuple<decimal, decimal> DetermineLeftAndWidthFromBlockMatching(SubPixelImage subPixelData, int minWhiteLevel, decimal widthAve, List<OsdLineConfig> lineConfigs)
        {
            var cfg = lineConfigs[0];

            var blocks = new List<Tuple<decimal, decimal>>();
            var scores = new Dictionary<decimal, decimal>();

            var vertTop = (int)Math.Ceiling(cfg.Top);
            var vertBottom = Math.Floor(cfg.Bottom);

            // 1. Identify blocks in 0.05 pixel intervals
            decimal MIN_SCORE = 5M * (vertBottom - vertTop) / 28M;
            decimal? startBlock = null;
            for (decimal x = cfg.Left - 5; x <= cfg.Right + 5; x += 0.05m)
            {
                decimal score = 0;
                for (int y = vertTop; y < vertBottom; y++)
                {
                    score += subPixelData.GetWholePixelAt(x, y) > minWhiteLevel ? 1 : 0;
                }
                scores[x] = score;
                if (score > MIN_SCORE)
                {
                    if (startBlock == null)
                        startBlock = x;
                }
                else
                {
                    if (startBlock != null)
                    {
                        blocks.Add(Tuple.Create(startBlock.Value, x));
                        startBlock = null;
                    }
                }
            }

            //var unoBlocks = blocks.Where(x => (x.Item2 - x.Item1) > 0.5M * widthAve && (x.Item2 - x.Item1) < 1.5M * widthAve);
            //var medianBlockWidth = unoBlocks.Count() > 2 ? unoBlocks.Select(x => x.Item2 - x.Item1).ToList().Median() : widthAve;
            //int MAX_STUCK_BLOCKS = 8;
            //var blocksClone = blocks.ToList();
            //var scaledWidths = new List<decimal>();
            //for (int i = 1; i <= MAX_STUCK_BLOCKS; i++)
            //{
            //    var items = blocksClone.Where(x => ((x.Item2 - x.Item1) / i) > 0.66M * medianBlockWidth && ((x.Item2 - x.Item1) / i) < 1.33M * medianBlockWidth).ToList();
            //    items.ForEach(x => blocksClone.Remove(x));
            //    scaledWidths.AddRange(items.Select(x => (x.Item2 - x.Item1)/i));
            //}
            //decimal averageWidth = scaledWidths.Average();

            decimal averageWidth = widthAve;
            decimal roughLeft = blocks.First().Item1;

            var deltas = blocks.Select(x => x.Item1 - (Math.Round((x.Item1 - roughLeft) / averageWidth)) * averageWidth).ToList();
            roughLeft = deltas.Average();

            var numBoxPositions = (int)Math.Round((blocks.Last().Item2 - blocks.First().Item1) / averageWidth);

            var topTuples = new List<Tuple<decimal, decimal>>();
            decimal topScore = -1;
            for (decimal width = averageWidth - 2; width <= averageWidth + 2; width += 0.025m)
            {
                for (decimal x = roughLeft - 2; x <= roughLeft + 2; x += 0.05m)
                {
                    decimal score = 0;
                    for (int i = 0; i < numBoxPositions; i++)
                    {
                        var left = x + i * width;
                        var right = x + (i + 1) * width;
                        var containedBlock = blocks.FirstOrDefault(b => left >= b.Item1 - 0.5M && right <= b.Item2 + 0.5M);
                        if (containedBlock != null)
                        {
                            var leftDiff = Math.Abs(containedBlock.Item1 - left); if (leftDiff > width) leftDiff = 0;
                            var rightDiff = Math.Abs(containedBlock.Item2 - right); if (rightDiff > width) rightDiff = 0;
                            score += Math.Min(leftDiff, rightDiff);
                        }
                    }

                    if (score > topScore)
                    {
                        topScore = score;
                        topTuples.Clear();
                        topTuples.Add(Tuple.Create(x, width));
                    }
                    else if (score == topScore)
                    {
                        topTuples.Add(Tuple.Create(x, width));
                    }
                }
            }

            if (topTuples.Count > 1 && topTuples.GroupBy(x => x.Item2).Count() == 1)
                return Tuple.Create(topTuples.Select(x => x.Item1).Average(), topTuples[0].Item2);
            else if (topTuples.Count > 0)
                return Tuple.Create(topTuples[0].Item1, topTuples[0].Item2);
            else
                return Tuple.Create(roughLeft, averageWidth);
        }

        private Tuple<decimal, decimal> DetermineLeftAndWidthFromGaps(SubPixelImage subPixelData, int minWhiteLevel, decimal widthAve, List<OsdLineConfig> lineConfigs)
        {
            // Try to locate the gaps between
            var data = new Dictionary<decimal, decimal>();
            var fullnessData = new Dictionary<decimal, decimal>();
            for (decimal width = widthAve - 2; width <= widthAve + 2; width += 0.025m)
            {
                var vals = new List<Tuple<decimal, decimal>>();
                foreach (var cfg in lineConfigs)
                {
                    var boxesFrom = cfg.Left;
                    var boxesTo = cfg.Right;
                    var vertTop = (int) Math.Ceiling(cfg.Top);
                    var vertBottom = Math.Floor(cfg.Bottom);
                    var numBoxPositions = (int) Math.Round((boxesTo - boxesFrom)/width);

                    if (m_SingleLineMode && numBoxPositions != 24) continue;

                    for (int i = 0; i < numBoxPositions; i++)
                    {
                        decimal x = cfg.Left + i*width;
                        decimal boxFullness = 0M;
                        decimal lineScore = 0;
                        for (int j = 0; j < width; j++)
                        {
                            for (int y = vertTop; y < vertBottom; y++)
                            {
                                if (j == 0)
                                    lineScore += subPixelData.GetWholePixelAt(x, y) > minWhiteLevel ? 1 : 0;

                                boxFullness += subPixelData.GetWholePixelAt(x + j, y) > minWhiteLevel ? 1 : 0;
                            }
                        }

                        var fullnesPercent = boxFullness*100M/(width*(vertBottom - vertTop));
                        vals.Add(Tuple.Create(lineScore, fullnesPercent));
                    }
                }

                decimal totalLineScore = 0M;
                decimal totalFullness = 0M;
                int totalFull = 0;
                const int MIN_FULLNESS_PERCENT = 10;
                for (int i = 0; i < vals.Count - 1; i++)
                {
                    var curr = vals[i];
                    var next = vals[i + 1];
                    if (curr.Item2 > MIN_FULLNESS_PERCENT && next.Item2 > MIN_FULLNESS_PERCENT)
                    {
                        totalLineScore += next.Item1;
                        totalFullness += curr.Item2;
                        totalFull++;
                    }
                }

                totalLineScore = vals.Sum(x => x.Item1);
                data.Add(width, totalLineScore);
                fullnessData.Add(width, totalFullness);
            }

            var arrKeys = data.Keys.ToArray();
            var arrWeights = data.Values.ToArray();
            Array.Sort(arrWeights, arrKeys);


            var arrKeysF = fullnessData.Keys.ToArray();
            var arrWeightsF = fullnessData.Values.ToArray();
            Array.Sort(arrWeightsF, arrKeysF);

            var lstKeysF = arrKeysF.ToList();
            var lstKeys = arrKeys.ToList();

            decimal boxesLeft = lineConfigs[0].Left;
            const int TOP_SCORES_TO_CONSIDER = 10;

            decimal[] topWidths = new decimal[2*TOP_SCORES_TO_CONSIDER];
            decimal[] topWidthCombinedScore = new decimal[2*TOP_SCORES_TO_CONSIDER];

            for (int i = 0; i < TOP_SCORES_TO_CONSIDER; i++)
            {
                topWidths[i] = arrKeys[i];
                topWidthCombinedScore[i] = i + lstKeysF.IndexOf(arrKeys[i]);
            }
            for (int i = 0; i < TOP_SCORES_TO_CONSIDER; i++)
            {
                topWidths[i + TOP_SCORES_TO_CONSIDER] = arrKeysF[i];
                topWidthCombinedScore[i + TOP_SCORES_TO_CONSIDER] = i + lstKeys.IndexOf(arrKeysF[i]);
            }
            Array.Sort(topWidthCombinedScore, topWidths);
            return Tuple.Create(boxesLeft, topWidths[0]);
        }

        // NOTE: This may be a good number if the timestamp is one one or two lines
        //       but what if the timestamp is on 3 or 4 lines ??
        private static int MAX_TOP_BOTTOM_EDGES_TO_CONSIDER = 15;
        private static int MIN_VIABLE_HEIGHT_PIXELS = 7;
        private static int MAX_VIABLE_HEIGHT_PIXELS = 40;
        private static int MAX_VIABLE_GAP = 12;
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
