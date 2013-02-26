using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using Tangra.Model.Config;
using Tangra.Model.Image;

namespace Tangra.Model.Astro
{
    internal class PotentialStarStruct
    {
        public int X;
        public int Y;
        public uint Z;
    }

    public static class StarFinder
    {
        public static List<PSFFit> GetStarsInArea(ref uint[,] data, int bpp, uint noiseValue)
        {
            return GetStarsInArea(ref data, bpp, noiseValue, TangraConfig.PreProcessingFilter.NoFilter);
        }

        public static List<PSFFit> GetStarsInArea(ref uint[,] data, int bpp, uint noiseValue, TangraConfig.PreProcessingFilter filter)
        {
            return GetStarsInArea(ref data, bpp, noiseValue, filter, null, null);
        }

        internal static List<PSFFit> GetStarsInArea(
            ref uint[,] data, int bpp, uint noiseValue, TangraConfig.PreProcessingFilter filter,
            List<PotentialStarStruct> allPotentialStars,
            List<PSFFit> allFoundStars)
        {
            uint ABOVE_NOISE_LEVEL_REQUIRED = TangraConfig.Settings.Special.StarFinderAboveNoiseLevel;
            double MIN_DISTANCE = TangraConfig.Settings.Special.StarFinderMinSeparation;
            return GetStarsInArea(ref data, bpp, filter, allPotentialStars, allFoundStars, noiseValue + ABOVE_NOISE_LEVEL_REQUIRED, MIN_DISTANCE);
        }

        internal static List<PotentialStarStruct> GetPeakPixelsInArea(
            uint[,] data, out uint[,] lpdData, int bpp, uint aboveNoiseLevelRequired,
            double minDistanceInPixels, bool useLPDFilter, Rectangle excludeArea)
        {

            if (useLPDFilter)
                lpdData = ImageFilters.LowPassDifferenceFilter(data, false);
            else
                lpdData = data;

            int nWidth = lpdData.GetLength(0);
            int nHeight = lpdData.GetLength(1);

            List<PotentialStarStruct> potentialStars = new List<PotentialStarStruct>();

            ExaminePeakPixelCandidate examinePixelCallback =
             delegate(int x, int y, uint z)
             {
                 bool tooClose = false;

                 // Local maximum, test for a star
                 foreach (PotentialStarStruct prevStar in potentialStars)
                 {
                     double dist =
                         Math.Sqrt((prevStar.X - x) * (prevStar.X - x) +
                                   (prevStar.Y - y) * (prevStar.Y - y));
                     if (dist <= minDistanceInPixels)
                     {
                         tooClose = true;
                         if (prevStar.Z < z)
                         {
                             prevStar.Z = z;
                             prevStar.X = x;
                             prevStar.Y = y;
                         }

                         break;
                     }
                 }

                 if (!tooClose)
                 {
                     potentialStars.Add(new PotentialStarStruct() { X = x, Y = y, Z = z });
                 }

                 // An early return if too many peak pixels have been found
                 return (potentialStars.Count <=
                         TangraConfig.Settings.Special.
                             StarFinderMaxNumberOfPotentialStars);
             };

            if (useLPDFilter)
                CheckAllPixels(lpdData, nWidth, nHeight, aboveNoiseLevelRequired, excludeArea, examinePixelCallback);
            else
                CheckPixelsFromBrightToFaint(lpdData, nWidth, nHeight, bpp, aboveNoiseLevelRequired, excludeArea, examinePixelCallback);

            return potentialStars;
        }

        internal delegate bool ExaminePeakPixelCandidate(int x, int y, uint z);

        private static void CheckAllPixels(uint[,] data, int nWidth, int nHeight, uint aboveNoiseLevelRequired, Rectangle excludeArea, ExaminePeakPixelCandidate callback)
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
                        if (!callback(i, j, testedPixel))
                            return;
                    }
                }

        }

        private static void CheckPixelsFromBrightToFaint(uint[,] data, int nWidth, int nHeight, int bpp, uint aboveNoiseLevelRequired, Rectangle excludeArea, ExaminePeakPixelCandidate callback)
        {
            bool excludeAreaDefined = Rectangle.Empty != excludeArea;

	        int includsionCheckStep = 10;
			if (bpp == 12) includsionCheckStep = 50;
			else if (bpp == 14) includsionCheckStep = 60;

	        int maxPixelValue = 255;
			if (bpp == 12) maxPixelValue = 0xFFF;
			else if (bpp == 14) maxPixelValue = 0x3FFF;

			int currInclusionLimit = maxPixelValue - includsionCheckStep + 1;

            while (currInclusionLimit > aboveNoiseLevelRequired)
            {
                for (int i = 1; i < nWidth - 1; i++)
                    for (int j = 1; j < nHeight - 1; j++)
                    {
                        if (excludeAreaDefined && excludeArea.Contains(i, j))
                            continue;

                        uint testedPixel = data[i, j];

                        if (testedPixel > currInclusionLimit &&
                            testedPixel <= currInclusionLimit + includsionCheckStep &&
                            testedPixel >= data[i - 1, j] &&
                            testedPixel >= data[i - 1, j - 1] &&
                            testedPixel >= data[i, j - 1] &&
                            testedPixel >= data[i + 1, j - 1] &&
                            testedPixel >= data[i + 1, j] &&
                            testedPixel >= data[i + 1, j + 1] &&
                            testedPixel >= data[i, j + 1] &&
                            testedPixel >= data[i - 1, j + 1])
                        {
                            if (!callback(i, j, testedPixel))
                                return;
                        }
                    }

                currInclusionLimit -= includsionCheckStep;
            }
        }

        internal static List<PSFFit> GetStarsInArea(
            ref uint[,] data, int bpp, TangraConfig.PreProcessingFilter filter,
            List<PotentialStarStruct> allPotentialStars,
            List<PSFFit> allFoundStars,
            uint aboveNoiseLevelRequired, double minDistanceInPixels)
        {
            return GetStarsInArea(ref data, bpp, filter, allPotentialStars,
                allFoundStars, aboveNoiseLevelRequired, minDistanceInPixels, filter == TangraConfig.PreProcessingFilter.LowPassDifferenceFilter, Rectangle.Empty, null);
        }

        internal delegate void FilterPotentialStars(List<PotentialStarStruct> peakPixels);

        internal static List<PSFFit> GetStarsInArea(
            ref uint[,] data, int bpp, TangraConfig.PreProcessingFilter filter,
            List<PotentialStarStruct> allPotentialStars,
            List<PSFFit> allFoundStars,
            uint aboveNoiseLevelRequired, double minDistanceInPixels,
            bool useLPDFilter, Rectangle excludeArea, FilterPotentialStars filterCallback)
        {

            double minFWHM = TangraConfig.Settings.Special.StarFinderMinFWHM;
            double maxFWHM = TangraConfig.Settings.Special.StarFinderMaxFWHM;

            int STAR_MATRIX_FIT = TangraConfig.Settings.Special.StarFinderFitArea;

            Stopwatch sw = new Stopwatch();
            sw.Start();
            uint[,] lpdData;
            List<PotentialStarStruct> potentialStars = GetPeakPixelsInArea(
                data, out lpdData, bpp, aboveNoiseLevelRequired, minDistanceInPixels, useLPDFilter, excludeArea);

            if (filterCallback != null) filterCallback(potentialStars);
            sw.Stop();
            Trace.WriteLine(string.Format("GetPeakPixelsInArea: {0} sec", sw.Elapsed.TotalSeconds.ToString("0.00")));

            if (potentialStars.Count > 3)
            {
                // Only include the 3 brightest stars. The other ones cannot be stars 
                potentialStars.Sort((x, y) => y.Z.CompareTo(x.Z));
                potentialStars = potentialStars.Take(3).ToList();
            }

            // Debugging purposes
            if (allPotentialStars != null)
                allPotentialStars.AddRange(potentialStars);

            // TODO: Check whether no filter gives better results than LP filter
            //byte[,] lpData = BitmapFilter.LowPassFilter(data, false);
            uint[,] lpData = data;

            List<PSFFit> foundStars = new List<PSFFit>();

            double MIN_DISTANCE_OF_PEAK_PIXEL_FROM_CENTER = TangraConfig.Settings.Special.StarFinderMinDistanceOfPeakPixelFromCenter;

            sw.Reset();
            sw.Start();

            foreach (PotentialStarStruct starToTest in potentialStars)
            {
                PSFFit fit = new PSFFit(starToTest.X, starToTest.Y);
                int fitMatrix = (int)Math.Min(data.GetLength(0), STAR_MATRIX_FIT + 2);

                // Get a matrix with 1 pixel larger each way and set the border pixels to zero
                fit.Fit(lpData, fitMatrix, starToTest.X, starToTest.Y, true);

                if (fit.IsSolved)
                {
                    double distanceFromCenter = ImagePixel.ComputeDistance(fit.X0_Matrix, fitMatrix / 2, fit.Y0_Matrix, fitMatrix / 2);

                    if (fit.Certainty > 0 &&
                        fit.FWHM >= minFWHM &&
                        fit.FWHM <= maxFWHM &&
                        distanceFromCenter < MIN_DISTANCE_OF_PEAK_PIXEL_FROM_CENTER &&
                        fit.IMax > aboveNoiseLevelRequired)
                    {

                        // This object passes all tests to be furhter considered as a star
                        foundStars.Add(fit);
                    }
                }


                if (allFoundStars != null)
                    allFoundStars.Add(fit);
            }

            foundStars.Sort((f1, f2) => f1.IMax.CompareTo(f2.IMax));

            PSFFit[] testStars = foundStars.ToArray();
            for (int i = 0; i < testStars.Length; i++)
            {
                PSFFit fainterStar = testStars[i];
                for (int j = i + 1; j < testStars.Length; j++)
                {
                    PSFFit brighterStar = testStars[j];

                    if (fainterStar.UniqueId == brighterStar.UniqueId) continue;

                    // If a the max of a fainter star is inside the fit of a brighter star
                    // then see if it is simply not a point of the other star

                    double dist = Math.Sqrt((fainterStar.XCenter - brighterStar.XCenter) * (fainterStar.XCenter - brighterStar.XCenter) + (fainterStar.YCenter - brighterStar.YCenter) * (fainterStar.YCenter - brighterStar.YCenter));
                    if (dist <= minDistanceInPixels)
                    {
                        if (foundStars.Contains(fainterStar)) foundStars.Remove(fainterStar);
                    }
                }
            }
            sw.Stop();
            Trace.WriteLine(string.Format("Doing PSFFitting: {0} sec", sw.Elapsed.TotalSeconds.ToString("0.00")));

            switch (filter)
            {
                case TangraConfig.PreProcessingFilter.NoFilter:
                    break;

                case TangraConfig.PreProcessingFilter.LowPassFilter:
                    data = lpData;
                    break;

                case TangraConfig.PreProcessingFilter.LowPassDifferenceFilter:
                    data = lpdData;
                    break;
            }

            return foundStars;
        }

        //internal static PSFFit GetPSFFitForPeakPixel(byte[,] data, PotentialStarStruct starToTest, float aboveNoiseLevelRequired)
        //{
        //    double minFWHM = TangraConfig.Settings.Special.StarFinderMinFWHM;
        //    double maxFWHM = TangraConfig.Settings.Special.StarFinderMaxFWHM;

        //    return GetPSFFitForPeakPixel(data, starToTest, aboveNoiseLevelRequired, minFWHM, maxFWHM);
        //}

        internal static PSFFit GetPSFFitForPeakPixel(
            uint[,] data,
            PotentialStarStruct starToTest,
            float aboveNoiseLevelRequired,
            double minFWHM,
            double maxFWHM)
        {

            int STAR_MATRIX_FIT = TangraConfig.Settings.Special.StarFinderFitArea;
            double MIN_DISTANCE_OF_PEAK_PIXEL_FROM_CENTER = TangraConfig.Settings.Special.StarFinderMinDistanceOfPeakPixelFromCenter;


            PSFFit fit = new PSFFit(starToTest.X, starToTest.Y);
            int fitMatrix = (int)Math.Min(data.GetLength(0), STAR_MATRIX_FIT + 2);

            // Get a matrix with 1 pixel larger each way and set the border pixels to zero
            fit.Fit(data, fitMatrix, starToTest.X, starToTest.Y, true);

            if (fit.IsSolved)
            {
                double distanceFromCenter = ImagePixel.ComputeDistance(fit.X0_Matrix, fitMatrix / 2, fit.Y0_Matrix, fitMatrix / 2);

                if (fit.Certainty > 0 &&
                    fit.FWHM >= minFWHM &&
                    fit.FWHM <= maxFWHM &&
                    distanceFromCenter < MIN_DISTANCE_OF_PEAK_PIXEL_FROM_CENTER &&
                    fit.IMax > aboveNoiseLevelRequired)
                {
                    //not good for lost tracking allow higher FWHM

                    // This object passes all tests to be furhter considered as a star
                    return fit;
                }
            }

            return null;
        }

    }
}
