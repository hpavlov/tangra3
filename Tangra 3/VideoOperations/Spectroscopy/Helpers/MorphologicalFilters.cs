using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Controller;


namespace Tangra.VideoOperations.Spectroscopy.Helpers
{
    public class MorphologicalFilters
    {
        /// <summary>
        /// A class that performs morphological image processing, to find the angle of the spectra about the star (normally horizontal running to the riht of the star)
        /// </summary>
        /// <param name="image">the image frame</param>
        public MorphologicalFilters(AstroImage image)
        {
            m_Image = image;
            m_Width = image.Width;
            m_Height = image.Height;

            // From empirical testing (on Sirius, Rigel, Betelguese and DV Carina) on a Watec 120N+ camera (excluding borders and OCR),
            // I found that 3xmedianNoise gives a good threshold value (better than the original code that was based on the star's intensity)
            m_IntensityThreshold = (uint)(3.0F * m_Image.MedianNoise);
            m_FractionOfWhitePixels = 0.0f;
            m_FractionOfWhitePixelsIsOkay = false;

            // A thin area around the border can be excluded from the image processing, as experience has found that (particularly on a
            // long integration setting) that this can be noisy and introduce spurious lines in the Hough transform
            m_BorderExclusionSize = 15;

            m_WhitePixels.Clear();

            // Each thinning pass removes a one pixel layer from the circumference of the spectra, so to thin the spectra down to a one
            // pixel wide line we need to make more passes than the radius of the star
            m_ThinningMaximumPasses = 25;
            m_ThinningActualPasses = 0;
            m_ThinningNumberOfHits = 0;
            m_StructuralElements = new List<int[,]>();

            m_MaximumAllowedNumberOfLines = 10;
            m_NumberOfLinesActuallyFound = 0;
            m_ThetaOfLinesFound = new List<int>();
            m_RhoOfLinesFound = new List<int>();

            m_HoughAngleStartingAngleDifference = 0;
        }

        private readonly AstroImage m_Image;
        private readonly int m_Width;
        private readonly int m_Height;

        // conversion from gray scale to black and white image variables

        private uint m_IntensityThreshold;
        private float m_FractionOfWhitePixels;
        private bool m_FractionOfWhitePixelsIsOkay;

        private ushort m_BorderExclusionSize;

        private readonly HashSet<Point> m_WhitePixels = new HashSet<Point>();

        // image thinning variables
        private int m_ThinningMaximumPasses;
        private int m_ThinningActualPasses;
        private int m_ThinningNumberOfHits;
        private readonly List<int[,]> m_StructuralElements;

        // hough image transform variables
        private int[,] m_HoughImage;
        private int m_MaximumAllowedNumberOfLines;
        private int m_NumberOfLinesActuallyFound;
        private readonly List<int> m_ThetaOfLinesFound;
        private readonly List<int> m_RhoOfLinesFound;
        private double[] m_Theta;

        private int m_HoughAngleStartingAngleDifference;

        /// <summary>
        /// Reports the number of white pixels in the black and white image, as a fraction of the total number of pixels
        /// </summary>
        public float FractionOfWhitePixels { get { return m_FractionOfWhitePixels; } }

        /// <summary>
        /// Set to exclude an area around the image border from subsequent image processing
        /// </summary>
        public ushort BorderExclusionSize 
        { 
            get { return m_BorderExclusionSize; } 
            set 
            { 
                if (value < 0) 
                {
                    throw new ArgumentOutOfRangeException("Invalid argument in spectroscopy: The image border exclusion should be a positive number");
                }
                if (value > 0.5* Math.Min(m_Width, m_Height))
                {
                    throw new ArgumentOutOfRangeException("Invalid argument in spectroscopy: The image border exclusion is too large and will exclude the entire image");
                }
                m_BorderExclusionSize = value; 
            } 
        }

        /// <summary>
        /// The maximum allowed number of passes run by the thinning method. Each pass removes the outer layer of pixels from the spectra
        /// </summary>
        public int ThinningMaximumPasses 
        { 
            get { return m_ThinningMaximumPasses; } 
            set 
            { 
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException("Invalid argument in spectroscopy: The number of passes to thin the spectra line(s) should be at least 1");
                }
                if (value > 0.5 * Math.Min(m_Width, m_Height))
                {
                    throw new ArgumentOutOfRangeException("Invalid argument in spectroscopy: The number of passes to thin the spectra line(s) is too large");
                }
                m_ThinningMaximumPasses = value; 
            } 
        }

        /// <summary>
        /// The actual number of passes performed by the thinning method. If equals the maximum allowed number of passes then you need to increase the number of passes allowed.
        /// </summary>
        public int ThinningActualPasses { get { return m_ThinningActualPasses; } }

        /// <summary>
        /// The number of white pixels that were thinned in the last pass of the thinning method. If > 0 then you need to increase the number of passes allowed.
        /// </summary>
        public int ThinningNumberOfHits { get { return m_ThinningNumberOfHits; } }

        /// <summary>
        /// The image produced by performing the Hough transform on the black and white image
        /// </summary>
        public int[,] HoughImage {  get { return m_HoughImage; } }

        /// <summary>
        /// The GetHoughAngle method will search for at most this number of lines (aka number of spectra)
        /// </summary>
        public int MaximumAllowedNumberOfLines 
        { 
            get { return m_MaximumAllowedNumberOfLines; } 
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException("Invalid argument in spectroscopy: The number of spectra (lines) to search for should be at least 1");
                }
                m_MaximumAllowedNumberOfLines = value;
            }
        }

        /// <summary>
        /// How many lines (aka spectra) were actually found by the GetHoughAngle method
        /// </summary>
        public int NumberOfLinesActuallyFound { get { return m_NumberOfLinesActuallyFound; } }

        /// <summary>
        /// The hough angles of the lines found by the GetHoughAngle method
        /// </summary>
        public List<int> ThetaFound { get { return m_ThetaOfLinesFound; } }

        /// <summary>
        /// The hough distance of the lines found by the GetHoughAngle method
        /// </summary>
        public List<int> RhoFound {  get { return m_RhoOfLinesFound; } }

        /// <summary>
        /// Saves the original image frame to a file
        /// </summary>
        /// <param name="filename">The image's filename, for example @"C:\Temp\original.png"</param>
        public void WriteOriginalImageToFile(string filename)
        {
            uint maximumValue = m_Image.MaxSignalValue; 

            // scale the intensity if maximum value > 255 (maximum value allowed in bitmaps)
            uint scaleFactor = 1;
            if (maximumValue > 255) scaleFactor = (maximumValue / 255);

            Bitmap originalImageBitmap = new Bitmap(m_Width, m_Height);

            for (int i = 0; i < m_Width; i++)
            {
                for (int j = 0; j < m_Height; j++)
                {
                    uint value = m_Image.Pixelmap[i, j] / scaleFactor;

                    originalImageBitmap.SetPixel(i, j, Color.FromArgb((int)value, 0, 0));
                }
            }
            originalImageBitmap.Save(filename);

        }

        /// <summary>
        /// Automatically set the intensity threshold (for converting the original image to black and white)
        /// </summary>
        private void SetIntensityThreshold()
        {
            uint medianThreshold = (uint)(3.0F * m_Image.MedianNoise);

            // copy image to 1d array, so can sort
            uint[] sortedImage = new uint[m_Width * m_Height];
            int index = 0;
            for (int y = 0; y < m_Height; y++)
            {
                for (int x = 0; x < m_Width; x++)
                {
                    sortedImage[index] = m_Image.GetPixel(x, y);
                    index++;
                }
            }
            Array.Sort(sortedImage);
            index = sortedImage.Length;
            // set the threshold so only the 2.5% of the brightest pixels are chosen
            index = (int)(0.975 * index);
            uint sortedImageThreshold = sortedImage[index];

            //Console.WriteLine("Debugging: medianThreshold = {0}", medianThreshold);
            //Console.WriteLine("Debugging: sortedImageThreshold = {0}", sortedImageThreshold);

            m_IntensityThreshold = Math.Max(medianThreshold,sortedImageThreshold);

        }

        /// <summary>
        /// Makes a black and white copy of the original image, for subsequent image processing
        /// </summary>
        public void ConvertGreyScaleImageToBlackAndWhite()
        {
            SetIntensityThreshold();

            uint countOfWhitePixels = 0;

            Point pt = new Point();

            // do the loops at least 1 in from the borders, so don't get 'Array Index out of bounds' exception when do the image filtering
            for (int i = 1 + m_BorderExclusionSize; i < m_Width - 1 - m_BorderExclusionSize; i++)
            {
                  for (int j = 1; j < m_Height - 1; j++)
                    {
                    uint value = m_Image.Pixelmap[i, j];

                    if (value >= m_IntensityThreshold)
                    {
                        countOfWhitePixels++;
                        pt.X = i;
                        pt.Y = j;
                        m_WhitePixels.Add(pt);
                    }
                }
            }

            m_FractionOfWhitePixels = (float)countOfWhitePixels / (float)(m_Width * m_Height);
            CheckFractionOfWhitePixelsIsOkay();

        }

        private void CheckFractionOfWhitePixelsIsOkay()
        {
            m_FractionOfWhitePixelsIsOkay = ((m_WhitePixels.Count > 20) && (m_FractionOfWhitePixels < 0.200));
            Console.WriteLine("FractionOfWhitePixelsIsOkay = {0}", m_FractionOfWhitePixelsIsOkay);
        }

        /// <summary>
        /// Saves the black and white image (as currently processed) to a file
        /// </summary>
        /// <param name="filename">The image's filename, for example @"C:\Temp\ThinnedBW.png"</param>
        public void WriteBlackAndWhiteImageToFile(string filename)
        {

            Bitmap blackAndWhitBitmap = new Bitmap(m_Width, m_Height);

            foreach (Point pt in m_WhitePixels)
            {
                blackAndWhitBitmap.SetPixel(pt.X, pt.Y, Color.White);
            }

            blackAndWhitBitmap.Save(filename);
        }

        /// <summary>
        /// Applies a dilation filter to the black and white image. This fills in any small gaps in the white parts in the image.
        /// </summary>
        public void ApplyDilationFilter()
        {
            if (!m_FractionOfWhitePixelsIsOkay)
            {
                // exit without changing the image
                return;
            }

            int width = m_Width - 1;
            int height = m_Height - 1;

            HashSet<Point> whitePixelsToBeAdded = new HashSet<Point>();
            HashSet<Point> whitePixelsToBeRemoved = new HashSet<Point>();

            int newI;
            int newJ;
            Point newPt = new Point();

            Point surroundingPt = new Point();

            foreach (Point pt in m_WhitePixels)
            {
                int i = pt.X;
                int j = pt.Y;

                // delete from the set if the array index is out of bounds
                if ((i < 1) || (i > width) || (j < 1) || (j > height))
                {
                    whitePixelsToBeRemoved.Add(pt);
                    continue;
                }

                for (int iOffset = -1; iOffset <= 1; iOffset++)
                {
                    newI = i + iOffset;
                    // skip if image index out of bounds
                    if ((newI < 1) || (newI > width)) continue;

                    for (int jOffset = -1; jOffset <= 1; jOffset++)
                    {
                        newJ = j + jOffset;
                        // skip if array index out of bounds
                        if ((newJ < 1) || (newJ > height)) continue;

                        newPt.X = newI;
                        newPt.Y = newJ;
                        //whitePixelsToBeAdded.Add(newPt);
                        if (m_WhitePixels.Contains(newPt)) continue;
                        // variation on the normal dilation filter -
                        // instead of changing all black pixels immediately surrounding this white pixel to white,
                        // check how many white pixels surround each black pixel and only change the black pixel (to white)
                        // if more than one surrounding white pixels
                        int numberOfSurroundingWhitePixels = 0;
                        for (int x = newI - 1; x <= newI + 1; x++)
                        {
                            for (int y = newJ - 1; y <= newJ + 1; y++)
                            {
                                surroundingPt.X = x;
                                surroundingPt.Y = y;
                                if (m_WhitePixels.Contains(surroundingPt))
                                {
                                    numberOfSurroundingWhitePixels++;
                                }
                            }
                        }
                        if (numberOfSurroundingWhitePixels > 1) whitePixelsToBeAdded.Add(newPt);
                    }
                }
            }

            //remove the out of bounds white pixels
            foreach (var s in whitePixelsToBeRemoved)
            {
                m_WhitePixels.Remove(s);
            }

            // add the new white pixels to the original set
            m_WhitePixels.UnionWith(whitePixelsToBeAdded);
        }

        /// <summary>
        /// Applies an erosion filter to the black and white image. This removes the outer layer of the white parts in the image.
        /// </summary>
        public void ApplyErosionFilter()
        {
            if (!m_FractionOfWhitePixelsIsOkay)
            {
                // exit without changing the image
                return;
            }

            int width = m_Width - 1;
            int height = m_Height - 1;

            HashSet<Point> whitePixelsToBeRemoved = new HashSet<Point>();

            int newI;
            int newJ;
            Point newPt = new Point();

            foreach (Point pt in m_WhitePixels)
            {
                int i = pt.X;
                int j = pt.Y;

                // delete from the set if the array index is out of bounds
                if ((i < 1) || (i > width) || (j < 1) || (j > height))
                {
                    whitePixelsToBeRemoved.Add(pt);
                    continue;
                }

                // if there is a black pixel surrounding the pixel then change the pixel to black
                for (int iOffset = -1; iOffset <= 1; iOffset++)
                {
                    newI = i + iOffset;
                    // skip if image index out of bounds
                    if ((newI < 1) || (newI > width)) continue;

                    for (int jOffset = -1; jOffset <= 1; jOffset++)
                    {
                        newJ = j + jOffset;
                        // skip if array index out of bounds
                        if ((newJ < 1) || (newJ > height)) continue;

                        newPt.X = newI;
                        newPt.Y = newJ;
                        // Is a surrounding pixel black, i.e. not in the white pixels set
                        if (!m_WhitePixels.Contains(newPt))
                        {
                            whitePixelsToBeRemoved.Add(pt);
                            goto EARLY_EXIT_OF_FILTER_LOOPS;
                        }
                    }
                }
            EARLY_EXIT_OF_FILTER_LOOPS:
                ;

            }
            foreach (var s in whitePixelsToBeRemoved)
            {
                m_WhitePixels.Remove(s);
            }
        }

        private void LocateTopAndBottomLineOfTimestamp(out int bestVtiOsdTopPosition, out int bestVtiOsdBottomPosition)
        {
            bestVtiOsdTopPosition = -1;
            bestVtiOsdBottomPosition = -1;

            int bestTopLineScore = -1;
            int bestBottomLineScore = -1;

            Point pt = new Point();
            Point topLinePt = new Point();
            Point bottomLinePt = new Point();

            for (int y = 1; y < m_Height - 1; y++)
            {
                int topLineScore = 0;
                int bottomLineScore = 0;

                pt.Y = y;
                topLinePt.Y = y - 1;
                bottomLinePt.Y = y + 1;

                for (int x = 1; x < m_Width - 1; x++)
                {
                    pt.X = x;

                    if (m_WhitePixels.Contains(pt))
                    {
                        topLinePt.X = x;
                        if (!m_WhitePixels.Contains(topLinePt))
                        {
                            topLineScore++;

                        }

                        bottomLinePt.X = x;
                        if (!m_WhitePixels.Contains(bottomLinePt))
                        {
                            bottomLineScore++;
                        }

                    }

                }

                if (topLineScore > bestTopLineScore)
                {
                    bestVtiOsdTopPosition = y;
                    bestTopLineScore = topLineScore;
                }

                if (bottomLineScore > bestBottomLineScore)
                {
                    bestVtiOsdBottomPosition = y;
                    bestBottomLineScore = bottomLineScore;
                }
            }

        }

        private void RemoveTimeStamp(int bestVtiOsdTopPosition, int bestVtiOsdBottomPosition)
        {
            HashSet<Point> whitePixelsToBeRemoved = new HashSet<Point>();

            foreach (Point pt in m_WhitePixels)
            {
                int y = pt.Y;
                if ((y >= bestVtiOsdTopPosition) && (y <= bestVtiOsdBottomPosition))
                {
                    whitePixelsToBeRemoved.Add(pt);
                }
            }

            foreach (var s in whitePixelsToBeRemoved)
            {
                m_WhitePixels.Remove(s);
            }


        }

        private bool FoundTimestampPosition(int bestVtiOsdTopPosition, int bestVtiOsdBottomPosition)
        {

            if (bestVtiOsdBottomPosition < bestVtiOsdTopPosition ||
                bestVtiOsdBottomPosition - bestVtiOsdTopPosition < 10 || 
                bestVtiOsdBottomPosition - bestVtiOsdTopPosition > 60 || 
                bestVtiOsdTopPosition < 0 || 
                bestVtiOsdBottomPosition > m_Height)
            {
                return false;
            }
            return true;
        }

        public void AutoDetectAndRemoveTimeStamp()
        {
            int bestVtiOsdTopPosition;
            int bestVtiOsdBottomPosition;

            // ToDo test some video recordings without a time-stamp and/or plain avi file

            LocateTopAndBottomLineOfTimestamp(out bestVtiOsdTopPosition, out bestVtiOsdBottomPosition);

            if (FoundTimestampPosition(bestVtiOsdTopPosition, bestVtiOsdBottomPosition))
            {
                RemoveTimeStamp(bestVtiOsdTopPosition, bestVtiOsdBottomPosition);

                m_FractionOfWhitePixels = (float)m_WhitePixels.Count / (float)(m_Width * m_Height);
                //Console.WriteLine("Fraction of white pixels after removing OCR = {0}", m_FractionOfWhitePixels);
                CheckFractionOfWhitePixelsIsOkay();
            }
        }

        /// <summary>
        /// Rotates the elements of the supplied array by 90 degrees.
        /// </summary>
        /// <param name="originalStructuralElement">A 3x3 array of elements with values -1, 0 or +1</param>
        /// <returns></returns>
        private int[,] Rotate90degrees(int[,] originalStructuralElement)
        {
            // Assumes the Structural Element is a 3x3 array

            int[,] structuralElement1 = new int[3, 3];
            int[,] structuralElement2 = new int[3, 3];

            // first swapping in i and j
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    structuralElement1[i, j] = originalStructuralElement[j, i];
                }
            }

            // then swap in j (about the middle column)
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0, reversedJ = 2; j < 3; j++, reversedJ--)
                {
                    structuralElement2[i, j] = structuralElement1[i, reversedJ];
                }
            }

            return structuralElement2;
        }

        /// <summary>
        /// Create the Structural Elements used by the thinning method
        /// </summary>
        private void CreateHitAndMissStructuralElements()
        {
            // The following Structural Elements are used for morphological thinning,
            // by checking if an octagon will fit inside the white pixels and is
            // guaranteed to produce a connected skeleton

            // create the first two Structural Elements
            // Note: -1 are for the "don't care" values in the StructuralElement
            //        0 <=> false, and
            //        1 <=> true 
            int[,] structuralElement1 =
            {
                {  0,  0,  0 },
                { -1,  1, -1 },
                {  1,  1,  1 }
            };

            int[,] structuralElement2 =
            {
                { -1,  0,  0 },
                {  1,  1,  0 },
                { -1,  1, -1 }
            };

            m_StructuralElements.Clear();
            m_StructuralElements.Add(structuralElement1);
            m_StructuralElements.Add(structuralElement2);

            // rotate each SE 3 times and add to list
            for (int i = 0; i < 3; i++)
            {
                structuralElement1 = Rotate90degrees(structuralElement1);
                structuralElement2 = Rotate90degrees(structuralElement2);
                m_StructuralElements.Add(structuralElement1);
                m_StructuralElements.Add(structuralElement2);
            }
        }

        /// <summary>
        /// Thin the spectra(s) down to one pixel wide line(s)
        /// </summary>
        public void ApplyThinningFilter()
        {
            if (!m_FractionOfWhitePixelsIsOkay)
            {
                // exit without changing the image
                return;
            }

            CreateHitAndMissStructuralElements();

            int width = m_Width - 1;
            int height = m_Height - 1;

            HashSet<Point> whitePixelsToBeRemoved = new HashSet<Point>();

            int i;
            int j;

            int newI;
            int newJ;
            Point newPt = new Point();

            m_ThinningActualPasses = 0;
            m_ThinningNumberOfHits = 0;

            bool hit = false;
            int structuralElementij;
            bool value;
            bool valueSE;

            for (int k = 0; k < m_ThinningMaximumPasses; k++)
            {

                m_ThinningNumberOfHits = 0;

                foreach (int[,] structurallElement in m_StructuralElements)
                {
                    whitePixelsToBeRemoved.Clear();

                    foreach (Point pt in m_WhitePixels)
                    {
                        // extract the coordinates from the set element
                        i = pt.X;
                        j = pt.Y;

                        
                        for (int iOffset = -1; iOffset <= 1; iOffset++)
                        {
                            newI = i + iOffset;
                            for (int jOffset = -1; jOffset <= 1; jOffset++)
                            {
                                newJ = j + jOffset;
                                structuralElementij = structurallElement[1 + iOffset, 1 + jOffset];
                                if (structuralElementij == -1) continue; // skip this pixel as a "don't care" StructuralElement
                                valueSE = (structuralElementij == 1);
                                newPt.X = newI;
                                newPt.Y = newJ;

                                value = m_WhitePixels.Contains(newPt);
                                if ((value != valueSE)) // corresponding pixel and StructuralElement don't match so a miss
                                {
                                    hit = false;
                                    goto EARLY_EXIT_OF_FILTER_LOOPS;

                                }
                            }
                        }
                        hit = true;

                    EARLY_EXIT_OF_FILTER_LOOPS:
                        if (hit)
                        {
                            m_ThinningNumberOfHits++;
                            whitePixelsToBeRemoved.Add(pt);
                        }
                    }

                    // update the white pixels set
                    foreach (var s in whitePixelsToBeRemoved)
                    {
                        m_WhitePixels.Remove(s);
                    }
                }

                m_ThinningActualPasses = k;
                if (m_ThinningNumberOfHits < 1)
                {
                    break;
                }

            }

        }

        /// <summary>
        /// Convert the angle of the spectra to the angle used by the Hough transform (differs by 90 degrees)
        /// </summary>
        /// <param name="roughStartingAngle">The angle of the spectra as roughly aligned by the user</param>
        /// <returns></returns>
        public int CalculateRoughHoughAngle(int roughStartingAngle)
        {
            // The hough angle is perpendicular to the angle of the spectral line(s)
            int roughHoughAngle = 90 - roughStartingAngle;

            // The spectral line's angle is always measured relative to the star, while the hough angle is relative to the image origin so they may still be 180 degrees different (depending on if the spectral line runs to the left or right of the star)
            m_HoughAngleStartingAngleDifference = 0;
            if (roughStartingAngle < 0)
            {
                m_HoughAngleStartingAngleDifference = 180;
            }
            roughHoughAngle += m_HoughAngleStartingAngleDifference;

            roughHoughAngle %= 360;
            if (roughHoughAngle < 0) roughHoughAngle += 360;

            return roughHoughAngle;
        }

        /// <summary>
        /// Convert the angle found by the Hough transform to the angle relative to the star
        /// </summary>
        /// <param name="houghAngle">the (median) angle of the line(s) found by the Hough transform </param>
        /// <returns></returns>
        public float CalculateBestAngle(double houghAngle)
        {
            float bestAngle = ((float)(90 - houghAngle));
            
            if (bestAngle < 0) bestAngle += 360;
            bestAngle += m_HoughAngleStartingAngleDifference;

            return bestAngle;
        }

        /// <summary>
        /// The Hough transform of the black and white image allows the angle(s) of the line(s) to be calculated
        /// </summary>
        /// <param name="roughStartingAngle">Optional approximate Hough angle of the spectra line(s)</param>
        public void CalculateHoughTransformOfImage(int? roughStartingAngle = null)
        {
            int diagonal = (int)Math.Sqrt(m_Width * m_Width + m_Height * m_Height);

            // theta is in degrees, always between 0 - 360 (may be a smaller range)
            const int MAXIMUM_THETA_ARRAY_SIZE = 360;
            double fromTheta = 0;
            double toTheta = 359;
            double stepTheta = (toTheta - fromTheta) / (MAXIMUM_THETA_ARRAY_SIZE -1);
            double theta1;
            double theta1InRadians;

            if (roughStartingAngle.HasValue)
            {
                fromTheta = roughStartingAngle.Value - 36;
                toTheta = roughStartingAngle.Value + 36;
                stepTheta = (toTheta - fromTheta) / (MAXIMUM_THETA_ARRAY_SIZE-1);
            }

            m_Theta = new double[MAXIMUM_THETA_ARRAY_SIZE];
            for (uint thetaIndex = 0; thetaIndex < MAXIMUM_THETA_ARRAY_SIZE; thetaIndex++)
            {
                m_Theta[thetaIndex] = fromTheta + thetaIndex * stepTheta;
            }

            // rho is in pixels, always >= 0 and less than the diagonal length of the image
            const int MAXIMUM_RHO_ARRAY_SIZE = 600;
            double fromRho = 0.0F;
            double toRho = diagonal;
            double stepRho = (toRho - fromRho) / (MAXIMUM_RHO_ARRAY_SIZE -1);
            double rho1;
            int rhoIndex1;
            double rho2;
            int rhoIndex2;

            // pre-calculate the trig values, as quicker to lookup 
            double[] cosTheta = new double[MAXIMUM_THETA_ARRAY_SIZE];
            double[] sinTheta = new double[MAXIMUM_THETA_ARRAY_SIZE];
            for (uint k = 0; k < MAXIMUM_THETA_ARRAY_SIZE; k++)
            {
                theta1 = m_Theta[k];
                theta1InRadians = (Math.PI * theta1 / 180);
                cosTheta[k] = Math.Cos(theta1InRadians);
                sinTheta[k]= Math.Sin(theta1InRadians);
            }

            m_HoughImage = new int[MAXIMUM_THETA_ARRAY_SIZE, MAXIMUM_RHO_ARRAY_SIZE];

            int i;
            int j;

            bool givenRhoTooLargeWarningMessage = false;

            foreach (Point pt in m_WhitePixels)
            {
                i = pt.X;
                j = pt.Y;

                for (uint thetaIndex = 0; thetaIndex < (MAXIMUM_THETA_ARRAY_SIZE-1); thetaIndex++)
                {
                    // convert from the array index to the corresponding value of theta
                    theta1 = m_Theta[thetaIndex];
                    theta1InRadians = (float)(Math.PI * theta1 / 180.0F);

                    // calculate rho for a line through the pixel at (i,j) at an angle of theta
                    rho1 = (i * cosTheta[thetaIndex] + j * sinTheta[thetaIndex]);

                    // calculate corresponding index in the rho array
                    rhoIndex1 = (int)Math.Round((rho1 - fromRho) / stepRho);

                    // fill in the blanks in the curve to the next index
                    rho2 = (i * cosTheta[thetaIndex + 1] + j * sinTheta[thetaIndex + 1]);
                    rhoIndex2 = (int)Math.Round((rho2 - fromRho) / stepRho);

                    if (Math.Min(rhoIndex1, rhoIndex2) < 0)
                    {
                        // negative values in the hough transform are ignored (they carry the same information as the positive values and so are redundant)
                        continue;
                    }
                    if (Math.Max(rhoIndex1, rhoIndex2) > MAXIMUM_RHO_ARRAY_SIZE)
                    {
                        if (!givenRhoTooLargeWarningMessage)
                        {
                            Console.WriteLine("Warning: rho value(s) in the Hough transform (that is used to detect spectral lines) is larger than the image");
                            givenRhoTooLargeWarningMessage = true;
                        }

                        continue;
                    }

                    // increment the value at this index
                    for (int rhoIndex = Math.Min(rhoIndex1, rhoIndex2); rhoIndex < Math.Max(rhoIndex1, rhoIndex2); rhoIndex++)
                    {
                        m_HoughImage[thetaIndex, rhoIndex]++;
                    }

                }
            }

        }

        /// <summary>
        /// Saves the Hough transform as a gray scale image. 
        /// If the peak intensities (corresponding to the line(s)) have been found by the GetHoughAngle method, then these are shown by red pixels.
        /// </summary>
        /// <param name="filename">The image's filename, for example @"C:\Temp\Hough.png"</param>
        public void WriteHoughImageToFile(string filename)
        {
            int width = m_HoughImage.GetLength(0);
            int height = m_HoughImage.GetLength(1);

            Bitmap blackAndWhiteBitmap = new Bitmap(width, height);

            int pixelIntensity;

            // get the maximum intensity in the hough image
            int maximumPixelIntensity = 0;
            float pixelIntensityScaleFactor;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    pixelIntensity = (int)m_HoughImage[i, j];
                    if (pixelIntensity > maximumPixelIntensity) maximumPixelIntensity = pixelIntensity;
                }
            }
            // Use a smaller value than 255 to calculate the scale factor, to ensure rounding errors doesn't give an intensity > 255
            pixelIntensityScaleFactor = 250.0F / maximumPixelIntensity;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    pixelIntensity = (int)(pixelIntensityScaleFactor * m_HoughImage[i, j]);
                    blackAndWhiteBitmap.SetPixel(i, j, Color.FromArgb(pixelIntensity, pixelIntensity, pixelIntensity));
                }
            }

            // draw any peaks found 
            if (m_NumberOfLinesActuallyFound > 0)
            {
                for (int k=0; k<m_NumberOfLinesActuallyFound; k++)
                {
                    int i = m_ThetaOfLinesFound[k];
                    int j = m_RhoOfLinesFound[k];
                    blackAndWhiteBitmap.SetPixel(i, j, Color.FromArgb(255, Color.Red));
                }
            }

            blackAndWhiteBitmap.Save(filename);
        }

        /// <summary>
        /// Finds the peak intensity(s) in the Hough transform image. These correspond to the Hough coordinates of the spectra line(s).
        /// Assumes all the spectra lines are parallel.
        /// </summary>
        /// <returns>Returns the median value of the spectra line(s), or (for a badly thresholded image) the original rough angle</returns>
        public double GetHoughAngle(int roughHoughAngle)
        {

            if (!m_FractionOfWhitePixelsIsOkay)
            {
                // exit with the original (manually selected) rough angle
                return roughHoughAngle;
            }

            m_ThetaOfLinesFound.Clear();
            m_RhoOfLinesFound.Clear();

            m_NumberOfLinesActuallyFound = 0;

            int houghPeakIntensityThreshold;

            int medianThetaIndex;

            int thetaLength = m_HoughImage.GetLength(0);
            int rhoLength = m_HoughImage.GetLength(1);

            // make a working copy of the hough image, as this method makes changes to it
            int[,] houghImage = new int[thetaLength, rhoLength];
            for (int i=0; i<thetaLength; i++)
            {
                for (int j=0; j<rhoLength; j++)
                {
                    houghImage[i, j] = m_HoughImage[i, j];
                }
            }

            // calculate the 'radius' of the neighborhood about each peak that will be zeroed prior to searching for the next peak
            // round up to the next odd number of the image size / 50
            int thetaPeakNeighborhoodWidth = thetaLength / 50;
            thetaPeakNeighborhoodWidth = 2 * (thetaPeakNeighborhoodWidth / 2) + 1;
            int rhoPeakNeighborhoodHeight = rhoLength / 50;
            rhoPeakNeighborhoodHeight = 2 * (rhoPeakNeighborhoodHeight / 2) + 1;
            // make sure is at least three pixels
            thetaPeakNeighborhoodWidth = Math.Max(thetaPeakNeighborhoodWidth, 3);
            rhoPeakNeighborhoodHeight = Math.Max(rhoPeakNeighborhoodHeight, 3);

            // get the largest peak intensity in the hough transform image
            int peakIntensity;
            int maximumPeakIntensity = 0;
            int rhoIndexOfMaximumPeakIntensity = 0;
            int thetaIndexOfMaximumPeakIntensity = 0;

            for (int thetaIndex = 0; thetaIndex < thetaLength; thetaIndex++)
            {
                for (int rhoIndex = 0; rhoIndex < rhoLength; rhoIndex++)
                {
                    peakIntensity = houghImage[thetaIndex, rhoIndex];
                    if (peakIntensity > maximumPeakIntensity)
                    {
                        rhoIndexOfMaximumPeakIntensity = rhoIndex;
                        thetaIndexOfMaximumPeakIntensity = thetaIndex;
                        maximumPeakIntensity = peakIntensity;
                    }
                }
            }

            // check that the peak is valid, i.e. sufficiently larger than the average (background) intensity
            // if not, then return with no lines found and the rough angle (initially entered by the user)

            // first, calculate the average
            int sum = 0;
            int count = 0;
            int value = 0;
            for (int i = 0; i < thetaLength; i++)
            {
                for (int j = 0; j < rhoLength; j++)
                {
                    value = m_HoughImage[i, j];
                    if (value > 0)
                    {
                        sum += value;
                        count++;
                    }
                }
            }
            double mean = (double)sum / (double)count;

            // next, check the peak is not significantly larger than the average
            if (maximumPeakIntensity < (10 * mean))
            {
                return roughHoughAngle;
            }


            m_NumberOfLinesActuallyFound++;
            m_ThetaOfLinesFound.Add(thetaIndexOfMaximumPeakIntensity);
            m_RhoOfLinesFound.Add(rhoIndexOfMaximumPeakIntensity);
            medianThetaIndex = thetaIndexOfMaximumPeakIntensity;

            // debugging:
            Console.WriteLine("Hough Peaks debugging:");
            //Console.WriteLine("1st peak:  intensity = {0}, indices's theta = {1}, rho = {2}, theta (degrees) = {3}", maximumPeakIntensity, thetaIndexOfMaximumPeakIntensity, rhoIndexOfMaximumPeakIntensity, m_theta[thetaIndexOfMaximumPeakIntensity]);

            if (m_MaximumAllowedNumberOfLines > 1)
            {
                // set the threshold for subsequent peaks at 50% of the first (largest) peak
                houghPeakIntensityThreshold = (maximumPeakIntensity / 2);

                for (int i = 1; i < m_MaximumAllowedNumberOfLines; i++)
                {

                    // set the previous peak and its neighboring pixels to zero
                    int rhoFrom = (int)(rhoIndexOfMaximumPeakIntensity - rhoPeakNeighborhoodHeight);
                    int rhoTo = (int)(rhoIndexOfMaximumPeakIntensity + rhoPeakNeighborhoodHeight);
                    int thetaFrom = (int)(thetaIndexOfMaximumPeakIntensity - thetaPeakNeighborhoodWidth);
                    int thetaTo = (int)(thetaIndexOfMaximumPeakIntensity + thetaPeakNeighborhoodWidth);

                    for (int rhoIndex = rhoFrom; rhoIndex <= rhoTo; rhoIndex++)
                    {
                        if ((rhoIndex < 0) || (rhoIndex >= rhoLength)) continue;
                        for (int thetaIndex = thetaFrom; thetaIndex <= thetaTo; thetaIndex++)
                        {
                            if ((thetaIndex < 0) || (thetaIndex >= thetaLength)) continue;

                            houghImage[thetaIndex, rhoIndex] = 0;
                        }
                    }

                    // find the next peak
                    maximumPeakIntensity = houghPeakIntensityThreshold;
                    rhoIndexOfMaximumPeakIntensity = 0;
                    thetaIndexOfMaximumPeakIntensity = 0;

                    for (int thetaIndex = 0; thetaIndex < thetaLength; thetaIndex++)
                    {
                        for (int rhoIndex = 0; rhoIndex < rhoLength; rhoIndex++)
                        {
                            peakIntensity = houghImage[thetaIndex, rhoIndex];
                            if (peakIntensity >= maximumPeakIntensity)
                            {
                                rhoIndexOfMaximumPeakIntensity = rhoIndex;
                                thetaIndexOfMaximumPeakIntensity = thetaIndex;
                                maximumPeakIntensity = peakIntensity;
                            }
                        }
                    }


                    // if below the threshold then exit the loop early
                    if (maximumPeakIntensity <= houghPeakIntensityThreshold)
                    {
                        goto ALL_PEAKS_FOUND;
                    }

                    // else process the next peak
                    m_NumberOfLinesActuallyFound++;
                    m_ThetaOfLinesFound.Add(thetaIndexOfMaximumPeakIntensity);
                    m_RhoOfLinesFound.Add(rhoIndexOfMaximumPeakIntensity);

                    // debugging:
                    //Console.WriteLine("next peak: intensity = {0}, indices's theta = {1}, rho = {2}, theta (degrees) = {3}", maximumPeakIntensity, thetaIndexOfMaximumPeakIntensity, rhoIndexOfMaximumPeakIntensity, m_theta[thetaIndexOfMaximumPeakIntensity]);

                }

            ALL_PEAKS_FOUND:

                List<int> thetaFound = m_ThetaOfLinesFound.ToList();
                thetaFound.Sort();
                int k = m_NumberOfLinesActuallyFound / 2;
                if ((m_NumberOfLinesActuallyFound > 1) && ((m_NumberOfLinesActuallyFound % 2) == 0))
                {
                    medianThetaIndex = (thetaFound[k-1] + thetaFound[k]) / 2;
                }
                else
                {
                    medianThetaIndex = thetaFound[k];
                }

                // debugging
                Console.WriteLine("median theta index = {0}, angle (degrees) = {1}", medianThetaIndex, m_Theta[medianThetaIndex]);
            }

            return m_Theta[medianThetaIndex];

        }

    }
}
