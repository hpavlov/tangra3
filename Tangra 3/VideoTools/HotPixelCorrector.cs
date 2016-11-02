using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Tangra.Model.Astro;
using Tangra.Model.Image;
using Tangra.PInvoke;
using Tangra.VideoOperations.LightCurves.Tracking;

namespace Tangra.VideoTools
{
    public static class HotPixelCorrector
    {
        // TODO: Make those configurable 
        private static double PEAK_PIXEL_LEVEL_REQUIRED = 0.75;

        public static void Initialize()
        {
            s_CombinedSample = new uint[7, 7];
            s_NumSamplesCombined = 0;
            s_Samples = new List<uint>[7, 7];
            s_Candidates = null;
            s_CandidateScores = null;

            for (int x = 0; x < 7; x++)
                for (int y = 0; y < 7; y++)
                {
                    s_Samples[x, y] = new List<uint>();
                }
        }

        public static void Cleanup()
        {

        }

        private static uint[,] s_CombinedSample = new uint[7, 7];
        private static List<uint>[,] s_Samples = new List<uint>[7, 7];
        private static int s_NumSamplesCombined = 0;
        private static uint s_CombinedSampleMedian = 0;
        private static uint s_MaxPixelValue = 0;

        private static uint[,] s_OriginalSample = new uint[7, 7];
        private static uint s_OriginalSampleMedian = 0;

        public static void RegisterHotPixelSample(uint[,] area, uint maxPixelValue)
        {
            s_MaxPixelValue = maxPixelValue;
            bool isFirstSample = s_NumSamplesCombined == 0;

            var score = ScoreArea(area);

            for (int x = 0; x < 7; x++)
            {
                for (int y = 0; y < 7; y++)
                {
                    int x1 = x + score.Item2;
                    int y1 = y + score.Item3;
                    if (x1 >= 0 && x1 < 7 && y1 >= 0 && y1 < 7)
                    {
                        s_Samples[x, y].Add(area[x1, y1]);
                    }
                }
            }
            s_NumSamplesCombined++;            
            // Build Combined Median Sample
            var combinedMedianList = new List<uint>();
            for (int x = 0; x < 7; x++)
            {
                for (int y = 0; y < 7; y++)
                {
                    s_Samples[x, y].Sort();
                    if (s_Samples[x, y].Count % 2 == 1)
                        s_CombinedSample[x, y] = s_Samples[x, y][s_Samples[x, y].Count / 2];
                    else
                        s_CombinedSample[x, y] = (uint)Math.Round((s_Samples[x, y][(s_Samples[x, y].Count / 2) - 1] + s_Samples[x, y][s_Samples[x, y].Count / 2]) / 2.0);


                    combinedMedianList.Add(s_CombinedSample[x, y]);
                    if (isFirstSample)
                        s_OriginalSample[x, y] = s_CombinedSample[x, y];

                }
            }
            combinedMedianList.Sort();
            s_CombinedSampleMedian = combinedMedianList[combinedMedianList.Count / 2];
            if (isFirstSample)
                s_OriginalSampleMedian = s_CombinedSampleMedian;

        }

        private static ImagePixel[] s_Candidates = null;
        private static double[] s_CandidateScores = null;
        private static int s_TopCandidatesToTake = 0;

        public static Dictionary<ImagePixel, double> LocateHotPixels(AstroImage image, int averageNTopCandidates)
        {
            var rv = LocateHotPixels(image, s_OriginalSample, s_OriginalSampleMedian);
            if (averageNTopCandidates > 0)
            {
                var allCenters = averageNTopCandidates < s_Candidates.Length
                    ? s_Candidates.Take(averageNTopCandidates)
                    : s_Candidates;

                // Horrible hack !!!
                s_CombinedSample = new uint[7, 7];
                s_NumSamplesCombined = 1;

                foreach (var pixel in allCenters)
                {
                    var model = image.GetPixelsArea(pixel.X, pixel.Y, 7);
                    RegisterHotPixelSample(model, image.Pixelmap.MaxSignalValue);
                }
                
                rv = LocateHotPixels(image, s_CombinedSample, s_CombinedSampleMedian);
            }

            return rv;
        }

        public static Dictionary<ImagePixel, double> LocateHotPixels(AstroImage image, uint[,] model, uint modelMedian)
        {
            var rv = new Dictionary<ImagePixel, double>();
            if (s_NumSamplesCombined > 0)
            {
                int width = image.Pixelmap.Width;
                int height = image.Pixelmap.Height;
                uint abv = model[3, 3] - modelMedian;
                uint minPeakLevel = (uint)(modelMedian + PEAK_PIXEL_LEVEL_REQUIRED * abv);
                EnumeratePeakPixels(image.Pixelmap.GetPixelsCopy(), width, height, minPeakLevel, Rectangle.Empty,
                    (x, y, z) =>
                    {
                        if (x >= 3 && x < width - 3 && y >= 3 && y < height - 3)
                        {
                            var newPix = new ImagePixel((int) z, x, y);
                            if (!rv.Keys.ToArray().Any(p => p.DistanceTo(newPix) < 5))
                                rv.Add(newPix, long.MinValue);
                        }
                            
                    });

                foreach (ImagePixel center in rv.Keys.ToArray())
                {
                    uint[,] testArea = image.GetPixelsArea(center.X, center.Y, 7);
                    var score = ScoreArea(testArea);
                    rv[center] = score.Item1;
                }

                var positions = rv.Keys.ToArray();
                var scores = rv.Values.ToArray();
                Array.Sort(scores, positions);
                s_Candidates = positions;
                s_CandidateScores = scores;
            }

            return rv;
        }

        private static Tuple<double, int, int> ScoreArea(uint[,] area, int x0 = 0, int y0 = 0)
        {
            double smallestDiff = double.MaxValue;
            int bx = 0;
            int by = 0;
            if (s_NumSamplesCombined > 0)
            {
                for (int dx = -3; dx < 3; dx++)
                {
                    for (int dy = -3; dy < 3; dy++)
                    {
                        long diffScore = 0;
                        int numPixelsInDiff = 0;
                        for (int x = 0; x < 7; x++)
                        {
                            for (int y = 0; y < 7; y++)
                            {
                                int x1 = x + dx;
                                int y1 = y + dy;

                                if (x1 < 0) x1 += 7;
                                if (x1 >= 7) x1 -= 7;
                                if (y1 < 0) y1 += 7;
                                if (y1 >= 7) y1 -= 7;
                                numPixelsInDiff++;
                                diffScore += Math.Abs((int)s_CombinedSample[x, y] - (int)area[x1 + x0, y1 + y0]);
                            }
                        }

                        double weightedDiff = 1.0*diffScore/numPixelsInDiff;
                        if (smallestDiff > weightedDiff)
                        {
                            smallestDiff = weightedDiff;
                            bx = dx;
                            by = dy;
                        }
                    }
                }
            }
            return Tuple.Create(smallestDiff, bx, by);
        }

        private static void EnumeratePeakPixels(uint[,] data, int nWidth, int nHeight, uint aboveNoiseLevelRequired, Rectangle excludeArea, Action<int, int, uint> callback)
        {
            bool excludeAreaDefined = Rectangle.Empty != excludeArea;

            for (int i = 1; i < nWidth - 1; i++)
                for (int j = 1; j < nHeight - 1; j++)
                {
                    if (excludeAreaDefined && excludeArea.Contains(i, j))
                        continue;

                    uint testedPixel = data[i, j];

                    if (testedPixel > aboveNoiseLevelRequired &&
                        testedPixel >= data[i - 1, j] &&
                        testedPixel >= data[i - 1, j - 1] &&
                        testedPixel >= data[i, j - 1] &&
                        testedPixel >= data[i + 1, j - 1] &&
                        testedPixel >= data[i + 1, j] &&
                        testedPixel >= data[i + 1, j + 1] &&
                        testedPixel >= data[i, j + 1] &&
                        testedPixel >= data[i - 1, j + 1])
                    {
                        callback(i, j, testedPixel);
                    }
                }

        }

        private static bool m_ShowHotPixelPositions = false;

        public static void ConfigurePreProcessing(bool removePixels)
        {
            m_ShowHotPixelPositions = !removePixels;

            ReconfigurePreProcessing();
        }

        public static void ReconfigurePreProcessing()
        {
            if (!m_ShowHotPixelPositions && s_Candidates != null)
                TangraCore.PreProcessors.PreProcessingAddRemoveHotPixels(s_CombinedSample, s_Candidates.Take(s_TopCandidatesToTake).ToArray(), s_CombinedSampleMedian, s_MaxPixelValue);
            else
                TangraCore.PreProcessors.PreProcessingAddRemoveHotPixels(s_CombinedSample, new ImagePixel[0], 0, 0);            
        }

        private static Font s_Font = new Font(FontFamily.GenericSansSerif, 6, FontStyle.Regular);

        public static void DrawOverlay(Graphics g, int topNumCandidates, bool showPeakPixels)
        {
            if (s_CandidateScores != null)
            {
                s_TopCandidatesToTake = topNumCandidates;

                var hotPixelsList = s_Candidates.Take(topNumCandidates);
                if (m_ShowHotPixelPositions)
                {
                    foreach (ImagePixel pixel in hotPixelsList)
                    {
                        g.DrawEllipse(Pens.Yellow, pixel.X - 5, pixel.Y - 5, 10, 10);
                    }
                }

                if (showPeakPixels && topNumCandidates < s_Candidates.Count())
                {
                    for (int i = topNumCandidates; i < s_Candidates.Length; i++)
                    {
                        ImagePixel pixel= s_Candidates[i];
                        g.DrawLine(Pens.Pink, pixel.X - 5, pixel.Y, pixel.X + 5, pixel.Y);
                        g.DrawLine(Pens.Pink, pixel.X, pixel.Y - 5, pixel.X, pixel.Y + 5);
                        g.DrawString(s_CandidateScores[i].ToString("0.00"), s_Font, Brushes.Pink, pixel.X + 1, pixel.Y - 10);
                    }
                }
            }
        }
    }
}
