using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Model.Numerical;
using Tangra.Model.VideoOperations;

namespace Tangra.Model.Image
{
    public class MeasurementsHelper
    {
        public enum Filter
        {
            None,
            LP,
            LPD
        }

        private TangraConfig.BackgroundMethod m_BackgroundMethod;
        private bool m_UseSubPixels = false;
        private uint m_SaturationValue = 255;
        private float m_TimesHigherPositionToleranceForFullyOccultedStars = 2.0f;
        private int m_BitPix = 8;
        private uint m_MaxPixelValue = 256;

        public MeasurementsHelper(
            int bitPix,
            TangraConfig.BackgroundMethod backgroundMethod,
            bool useSubpixelMeasurement,
            uint saturationValue)
        {
            m_BitPix = bitPix;
            m_MaxPixelValue = Pixelmap.GetMaxValueForBitPix(m_BitPix);
            m_BackgroundMethod = backgroundMethod;
            m_UseSubPixels = useSubpixelMeasurement;
            m_SaturationValue = saturationValue;

            m_TimesHigherPositionToleranceForFullyOccultedStars =
                TangraConfig.Settings.Special.TimesHigherPositionToleranceForFullyOccultedStars;
        }


        private float m_InnerRadiusOfBackgroundApertureInSignalApertures;
        private int m_NumberOfPixelsInBackgroundAperture;
        private float m_RejectionBackgoundPixelsStdDevCoeff;
        private float m_PositionTolerance;

        public void SetCoreProperties(
            float innerRadiusOfBackgroundApertureInSignalApertures,
            int numberOfPixelsInBackgroundAperture,
            float rejectionBackgoundPixelsStdDevCoeff,
            float positionTolerance)
        {
            m_InnerRadiusOfBackgroundApertureInSignalApertures = innerRadiusOfBackgroundApertureInSignalApertures;
            m_NumberOfPixelsInBackgroundAperture = numberOfPixelsInBackgroundAperture;
            m_RejectionBackgoundPixelsStdDevCoeff = rejectionBackgoundPixelsStdDevCoeff;

            if (m_BitPix > 16) throw new NotSupportedException();

            m_PositionTolerance = positionTolerance;
        }

        public delegate uint[,] GetImagePixelsDelegate(int x, int y, int matrixSize);

        public event GetImagePixelsDelegate GetImagePixelsCallback;

        public MeasurementsHelper Clone()
        {
            MeasurementsHelper clone = new MeasurementsHelper(m_BitPix, m_BackgroundMethod, m_UseSubPixels, m_SaturationValue);
            clone.m_InnerRadiusOfBackgroundApertureInSignalApertures = this.m_InnerRadiusOfBackgroundApertureInSignalApertures;
            clone.m_NumberOfPixelsInBackgroundAperture = this.m_NumberOfPixelsInBackgroundAperture;
            clone.m_RejectionBackgoundPixelsStdDevCoeff = this.m_RejectionBackgoundPixelsStdDevCoeff;

            clone.m_PositionTolerance = this.m_PositionTolerance;

            return clone;
        }

        private double m_PSFBackground;
        private PSFFit m_FoundBestPSFFit;
        private double m_TotalReading;
        private double m_TotalBackground;
        private float m_TotalPixels;
        private uint[,] m_PixelData;
        private float m_XCenter;
        private float m_YCenter;

        private bool m_HasSaturatedPixels;

        public PSFFit FoundBestPSFFit
        {
            get
            {
                return m_FoundBestPSFFit;
            }
        }

        public double PSFBackground
        {
            get
            {
                return m_PSFBackground;
            }
        }

        public float XCenter
        {
            get { return m_XCenter; }
        }

        public float YCenter
        {
            get { return m_YCenter; }
        }

        public bool HasSaturatedPixels
        {
            get { return m_HasSaturatedPixels; }
        }


        public double TotalReading
        {
            get { return m_TotalReading; }
        }

        public double TotalBackground
        {
            get { return m_TotalBackground; }
        }

        public float TotalPixels
        {
            get { return m_TotalPixels; }
        }

        public uint[,] PixelData
        {
            get { return m_PixelData; }
        }

		internal NotMeasuredReasons DoAperturePhotometry(
            uint[,] matrix, int x0Int, int y0Int, float x0, float y0, float aperture, int matrixSize,
            uint[,] backgroundArea)
        {
            if (matrix.GetLength(0) != 17)
                throw new ApplicationException("Measurement error. Correlation: AFP-101");

            m_HasSaturatedPixels = false;
            m_PSFBackground = double.NaN;
            m_FoundBestPSFFit = null;
            m_XCenter = x0 - x0Int + 8.5f;
            m_YCenter = y0 - y0Int + 8.5f;

            m_PixelData = matrix;

            m_TotalPixels = 0;

            m_TotalReading = GetReading(
                aperture,
                m_PixelData, 17, 17,
                m_XCenter, m_YCenter, null, ref m_TotalPixels);

            if (m_TotalPixels > 0)
            {
                if (m_BackgroundMethod == TangraConfig.BackgroundMethod.PSFBackground)
                {
                    PSFFit fit = new PSFFit(x0Int, y0Int);
                    fit.Fit(matrix, matrixSize);

                    if (!fit.IsSolved)
                        m_TotalBackground = 0;
                    else
                        m_TotalBackground = (uint)Math.Round(m_TotalPixels * (float)fit.I0);
                }
				else if (m_BackgroundMethod == TangraConfig.BackgroundMethod.Background3DPolynomial)
				{
					throw new NotImplementedException();
				}
	            else if (m_BackgroundMethod != TangraConfig.BackgroundMethod.PSFBackground)
	            {
		            double bgFromAnnulus = GetBackground(backgroundArea, aperture, 17, 17);
		            m_TotalBackground = (uint) Math.Round(m_TotalPixels*(float) bgFromAnnulus);
	            }
	            else
	            {
		            throw new ApplicationException("Measurement error. Correlation: AFP-102");
	            }

	            return NotMeasuredReasons.MeasuredSuccessfully;
            }
            else
            {
                m_TotalBackground = 0;
                m_TotalReading = 0;
	            return NotMeasuredReasons.NoPixelsToMeasure;
            }
        }

		internal NotMeasuredReasons DoNonLinearProfileFittingPhotometry(
            uint[,] matrix, int x0Int, int y0Int, float x0, float y0,
            float aperture, int matrixSize, bool useNumericalQadrature,
            bool isFullyDisappearingOccultedStar,
            uint[,] backgroundArea,
            bool mayBeOcculted /* Some magic based on a pure guess */,
            double refinedFWHM)
        {
            PSFFit fit = new PSFFit(x0Int, y0Int);
            fit.Fit(matrix, matrixSize);
            double distance = ImagePixel.ComputeDistance(fit.XCenter, x0, fit.YCenter, y0);
            double tolerance = isFullyDisappearingOccultedStar
                ? m_TimesHigherPositionToleranceForFullyOccultedStars * m_PositionTolerance
                : m_PositionTolerance;

            // We first go and do the measurement anyway
            if (fit.IsSolved)
                SetPsfFitReading(fit, aperture, useNumericalQadrature, backgroundArea);

            if (!fit.IsSolved || // The PSF solution failed, mark the reading invalid
                (distance > tolerance && !mayBeOcculted) || // If this doesn't look like a full disappearance, then make the reading invalid
                (fit.FWHM < 0.75 * refinedFWHM || fit.FWHM > 1.25 * refinedFWHM) // The FWHM is too small or too large, make the reading invalid
                )
            {
                return !fit.IsSolved
                        ? NotMeasuredReasons.MeasurementPSFFittingFailed
                        : (distance > tolerance && !mayBeOcculted)
                            ? NotMeasuredReasons.DistanceToleranceTooHighForNonFullDisappearingOccultedStar
                            : NotMeasuredReasons.FWHMOutOfRange;
            }
			else
				return NotMeasuredReasons.MeasuredSuccessfully;
        }

		internal NotMeasuredReasons DoLinearProfileFittingOfAveragedMoodelPhotometry(
            uint[,] matrix, int x0Int, int y0Int, float x0, float y0, float modelFWHM,
            float aperture, int matrixSize, bool useNumericalQadrature,
            bool isFullyDisappearingOccultedStar,
            uint[,] backgroundArea,
            bool mayBeOcculted /* Some magic based on a pure guess */)
        {
            PSFFit fit = new PSFFit(x0Int, y0Int);
            fit.FittingMethod = PSFFittingMethod.LinearFitOfAveragedModel;
            fit.SetAveragedModelFWHM(modelFWHM);
            fit.Fit(matrix, matrixSize);

            double distance = ImagePixel.ComputeDistance(fit.XCenter, x0, fit.YCenter, y0);
            double tolerance = isFullyDisappearingOccultedStar
                ? m_TimesHigherPositionToleranceForFullyOccultedStars * m_PositionTolerance
                : m_PositionTolerance;

            // We first go and do the measurement anyway
            if (fit.IsSolved)
                SetPsfFitReading(fit, aperture, useNumericalQadrature, backgroundArea);

			if (!fit.IsSolved || // The PSF solution failed, mark the reading invalid
				(distance > tolerance && !mayBeOcculted)// If this doesn't look like a full disappearance, then make the reading invalid
				)
			{
				return !fit.IsSolved
						? NotMeasuredReasons.MeasurementPSFFittingFailed
						: NotMeasuredReasons.DistanceToleranceTooHighForNonFullDisappearingOccultedStar;
			}
			else
				return NotMeasuredReasons.MeasuredSuccessfully;
        }

        private void SetPsfFitReading(PSFFit fit, float aperture, bool useNumericalQadrature, uint[,] backgroundArea)
        {
            if (useNumericalQadrature)
            {
                double r0Square = fit.R0 * fit.R0;

                m_TotalReading = Integration.IntegrationOverCircularArea(
                    delegate(double x, double y)
                    {
                        return fit.I0 + (fit.IMax - fit.I0) * Math.Exp((-x * x + -y * y) / r0Square);
                    },
                    aperture);

                if (m_BackgroundMethod == TangraConfig.BackgroundMethod.PSFBackground)
                {
                    m_TotalBackground = Math.PI * aperture * aperture * fit.I0;
                }
				else if (m_BackgroundMethod == TangraConfig.BackgroundMethod.Background3DPolynomial)
				{
					throw new NotImplementedException();
				}
                else
                {
                    double bgFromAnnulus = GetBackground(backgroundArea, aperture, 17, 17);
                    m_TotalBackground = Math.PI * aperture * aperture * (float)bgFromAnnulus;
                }
            }
            else
            {
                double apertureForBackground = 3 * fit.FWHM;

                // Analytical quadrature (Gauss Integral)		
                if (m_BackgroundMethod == TangraConfig.BackgroundMethod.PSFBackground)
                {
                    m_TotalReading = (fit.IMax - fit.I0) * fit.R0 * fit.R0 * Math.PI;
                    m_TotalBackground = Math.PI * apertureForBackground * apertureForBackground * (float)fit.I0;
                }
                else
                {
                    double bgFromAnnulus = GetBackground(backgroundArea, aperture, 17, 17);
                    m_TotalReading = (fit.IMax - bgFromAnnulus) * fit.R0 * fit.R0 * Math.PI;
                    m_TotalBackground = Math.PI * apertureForBackground * apertureForBackground * (float)bgFromAnnulus;
                }

                // Artificially add some background
                m_TotalReading += m_TotalBackground;
            }
        }

		internal NotMeasuredReasons DoOptimalExtractionPhotometry(
            uint[,] matrix, int x0Int, int y0Int, float x0, float y0,
            float aperture, int matrixSize, bool isFullyDisappearingOccultedStar,
            uint[,] backgroundArea,
            bool mayBeOcculted /* Some magic based on a pure guess */,
            double refinedFWHM)
        {
            // Proceed as with PSF Non linear, Then find the variance of the PSF fit and use a weight based on the residuals of the PSF fit
            // Signal[x, y] = PSF(x, y) + weight * Residual(x, y)

            PSFFit fit = new PSFFit(x0Int, y0Int);
            fit.Fit(matrix, matrixSize);
            double distance = ImagePixel.ComputeDistance(fit.XCenter, x0, fit.YCenter, y0);
            double tolerance = isFullyDisappearingOccultedStar
                ? m_TimesHigherPositionToleranceForFullyOccultedStars * m_PositionTolerance
                : m_PositionTolerance;

            float psfBackground = (float)fit.I0;

            // almost like aperture            
            if (matrix.GetLength(0) != 17)
                throw new ApplicationException("Measurement error. Correlation: AFP-101B");

            m_HasSaturatedPixels = false;
            m_PSFBackground = double.NaN;
            m_FoundBestPSFFit = null;
            m_XCenter = x0 - x0Int + 8.5f;
            m_YCenter = y0 - y0Int + 8.5f;

            m_PixelData = matrix;

            m_TotalPixels = 0;

            double varR0Sq = fit.GetVariance();
            varR0Sq = varR0Sq * varR0Sq * 2;

            double deltaX = fit.X0_Matrix - m_XCenter;
            double deltaY = fit.Y0_Matrix - m_YCenter;

            m_TotalReading = GetReading(
                aperture,
                m_PixelData, 17, 17,
                m_XCenter, m_YCenter,
                delegate(int x, int y)
                {
                    double videoPixel = m_PixelData[x, y];
                    double psfValue = fit.GetValue(x + deltaX, y + deltaY);
                    double residual = videoPixel - psfValue;

                    double weight = Math.Exp(-residual * residual / varR0Sq);
                    return psfValue + residual * weight;
                },
                ref m_TotalPixels);

            if (m_TotalPixels > 0)
            {
                if (m_BackgroundMethod == TangraConfig.BackgroundMethod.PSFBackground)
                {
                    if (float.IsNaN(psfBackground))
                        m_TotalBackground = 0;
                    else
                        m_TotalBackground = (uint)Math.Round(m_TotalPixels * psfBackground);
                }
				else if (m_BackgroundMethod == TangraConfig.BackgroundMethod.Background3DPolynomial)
				{
					throw new NotImplementedException();
				}
                else if (m_BackgroundMethod != TangraConfig.BackgroundMethod.PSFBackground)
                {
                    double bgFromAnnulus = GetBackground(backgroundArea, aperture, 17, 17);
                    m_TotalBackground = (uint)Math.Round(m_TotalPixels * (float)bgFromAnnulus);
                }
                else
                {
                    throw new ApplicationException("Measurement error. Correlation: AFP-102B");
                }

                if (!fit.IsSolved || // The PSF solution failed, mark the reading invalid
                                (distance > tolerance && !mayBeOcculted) || // If this doesn't look like a full disappearance, then make the reading invalid
                                (fit.FWHM < 0.75 * refinedFWHM || fit.FWHM > 1.25 * refinedFWHM) // The FWHM is too small or too large, make the reading invalid
                                )
                {
                    return !fit.IsSolved
                            ? NotMeasuredReasons.MeasurementPSFFittingFailed
                            : (distance > tolerance && !mayBeOcculted)
                                ? NotMeasuredReasons.DistanceToleranceTooHighForNonFullDisappearingOccultedStar
                                : NotMeasuredReasons.FWHMOutOfRange;
                }
            }
            else
            {
                // We can't do much here, but this should never happen (??)
                m_TotalBackground = 0;
                m_TotalReading = 0;

                return NotMeasuredReasons.NoPixelsToMeasure;
            }

			return NotMeasuredReasons.MeasuredSuccessfully;
        }

        public void Measure(float x0, float y0, float precomputedAperture, Filter filter, uint[,] matrix, int bpp, double psfBackground, ref int matrixSize, bool fixedAperture)
        {
            Measure(x0, y0, precomputedAperture, filter, matrix, bpp, psfBackground, true, ref matrixSize, fixedAperture, false);
        }

        public double BestMatrixSizeDistanceDifferenceTolerance = 3.0;

        private void Measure(
            float x0, float y0, float precomputedAperture,
			Filter filter, uint[,] matrix, int bpp, double psfBackground,
            bool findBestMatrixSize, ref int matrixSize,
            bool fixedAperture, bool doCentroidFitForFixedAperture)
        {
            m_HasSaturatedPixels = false;
            m_PSFBackground = double.NaN;
            m_FoundBestPSFFit = null;
            m_PixelData = null;

            if (filter == Filter.LPD)
            {
				m_PixelData = ImageFilters.LowPassDifferenceFilter(matrix, bpp);
            }
            else if (filter == Filter.LP)
            {
				m_PixelData = ImageFilters.LowPassFilter(matrix, bpp, false);
            }
            else
                m_PixelData = matrix;


            int x0i = (int)Math.Round(x0);
            int y0i = (int)Math.Round(y0);

            bool isSolved = false;
            Trace.Assert(m_PixelData.GetLength(0) == m_PixelData.GetLength(1));
            int halfDataSize = (int)Math.Ceiling(m_PixelData.GetLength(0) / 2.0);

            if (findBestMatrixSize)
            {

                if (!fixedAperture)
                {
                    PSFFit fit = new PSFFit(x0i, y0i);

                    #region Copy only the matix around the PFS Fit Area
                    do
                    {
                        int halfSize = matrixSize / 2;
                        uint[,] fitAreaMatrix = new uint[matrixSize, matrixSize];
                        int xx = 0, yy = 0;
                        for (int y = halfDataSize - halfSize; y <= halfDataSize + halfSize; y++)
                        {
                            xx = 0;
                            for (int x = halfDataSize - halfSize; x <= halfDataSize + halfSize; x++)
                            {
                                fitAreaMatrix[xx, yy] = m_PixelData[x, y];
                                xx++;
                            }
                            yy++;
                        }
                        fit.Fit(fitAreaMatrix);

                        m_XCenter = fit.X0_Matrix + halfDataSize - halfSize;
                        m_YCenter = fit.Y0_Matrix + halfDataSize - halfSize;

                        if (!findBestMatrixSize)
                            break;

                        if (Math.Sqrt((m_XCenter - halfDataSize) * (m_XCenter - halfDataSize) + (m_YCenter - halfDataSize) * (m_YCenter - halfDataSize)) <
                            BestMatrixSizeDistanceDifferenceTolerance)
                            break;

                        matrixSize -= 2;
                    } while (matrixSize > 0);

                    #endregion

                    isSolved = fit.IsSolved;
                    if (isSolved)
                    {
                        m_FoundBestPSFFit = fit;
                        m_PSFBackground = fit.I0;
                    }
                }
                else
                {
                    // Fixed Aperture
                    if (doCentroidFitForFixedAperture)
                    {
                        //"Do a 5x5 centroid cenetring, then get the new byte[,] around this new center, and then pass for measuring with a fixed aperture value");
                    }

                    m_XCenter = halfDataSize - 1;
                    m_YCenter = halfDataSize - 1;

                    m_PSFBackground = psfBackground;
                    // Fixed apertures are always considered solved
                    isSolved = true;
                }
            }
            else
            {
                isSolved = true;

                m_XCenter = halfDataSize - 1;
                m_YCenter = halfDataSize - 1;
                m_PSFBackground = psfBackground;
            }


            if (isSolved)
            {
                m_TotalPixels = 0;

                m_TotalReading = GetReading(
                    precomputedAperture,
                    m_PixelData, 17, 17,
                    m_XCenter, m_YCenter, null, ref m_TotalPixels);

                if (m_BackgroundMethod == TangraConfig.BackgroundMethod.PSFBackground && !double.IsNaN(psfBackground))
                {
                    m_TotalBackground = (uint)Math.Round(m_TotalPixels * psfBackground);
                }
                else
                {
                    uint[,] biggerArea;

                    if (GetImagePixelsCallback == null)
                        biggerArea = m_PixelData;
                    else
                        biggerArea = GetImagePixelsCallback((int)Math.Round(x0), (int)Math.Round(y0), 35);

                    double bgFromAnnulus = GetBackground(biggerArea, precomputedAperture, 17, 17);
                    m_TotalBackground = (uint)Math.Round(m_TotalPixels * bgFromAnnulus);
                }
            }
            else
            {
                m_TotalReading = 0;
                m_TotalBackground = 0;
                m_XCenter = 0;
                m_YCenter = 0;
            }
        }

        private delegate double GetPixelMeasurementCallback(int x, int y);

        private uint GetReading(
            float aperture, uint[,] data,
            int nWidth, int nHeight,
            float x0, float y0,
            GetPixelMeasurementCallback pixelMeasurementCallback,
            ref float totalPixels)
        {
            double total = 0;
            totalPixels = 0;
            for (int x = 0; x < nWidth; x++)
                for (int y = 0; y < nHeight; y++)
                {
                    double dist = Math.Sqrt((x0 - x) * (x0 - x) + (y0 - y) * (y0 - y));
                    if (dist + 1.5 <= aperture)
                    {
                        // If the point plus 1 pixel diagonal is still in the aperture
                        // then add the readin directly

                        if (pixelMeasurementCallback != null)
                            total += (float)pixelMeasurementCallback(x, y);
                        else
                            total += data[x, y];

                        totalPixels++;
                    }
                    else if (dist - 1.5 <= aperture)
                    {
                        float subpixels = 0;
                        if (m_UseSubPixels)
                        {
                            // Represent the pixels as 5x5 subpixels with 5 times lesses intencity and then add up
                            for (int dx = -2; dx <= 2; dx++)
                                for (int dy = -2; dy <= 2; dy++)
                                {
                                    double xx = x + dx / 5.0;
                                    double yy = y + dy / 5.0;
                                    dist = Math.Sqrt((x0 - xx) * (x0 - xx) + (y0 - yy) * (y0 - yy));
                                    if (dist <= aperture)
                                        subpixels += 1.0f / 25;
                                }
                        }
                        else if (dist <= aperture)
                            subpixels = 1.0f;

                        if (pixelMeasurementCallback != null)
                            total += (float)pixelMeasurementCallback(x, y) * subpixels;
                        else
                            total += data[x, y] * subpixels;

                        totalPixels += subpixels;
                    }

                    if (data[x, y] >= m_SaturationValue) m_HasSaturatedPixels = true;
                }

            return (uint)Math.Round(total);
        }

        private double GetBackground(uint[,] pixels, float signalApertureRadius, double x0, double y0)
        {
            int nWidth = pixels.GetLength(0);
            int nHeight = pixels.GetLength(1);

            float innerRadius = (float)(m_InnerRadiusOfBackgroundApertureInSignalApertures * signalApertureRadius);
            float outernRadius = (float)Math.Sqrt(m_NumberOfPixelsInBackgroundAperture / Math.PI + innerRadius * innerRadius);

			if (m_BackgroundMethod == TangraConfig.BackgroundMethod.Background3DPolynomial || m_BackgroundMethod == TangraConfig.BackgroundMethod.PSFBackground)
                throw new InvalidOperationException("Bad code path");

            // We want the annular background aperture to contain between 50 and 250 pixels.
            List<uint> allBgReadings = new List<uint>();

            for (int x = 0; x < nWidth; x++)
                for (int y = 0; y < nHeight; y++)
                {
                    double dist = Math.Sqrt((x - x0) * (x - x0) + (y - y0) * (y - y0));
                    if (dist >= innerRadius && dist <= outernRadius)
                    {
                        uint reading = pixels[x, y];
                        allBgReadings.Add(reading);
                    }
                }

            switch (m_BackgroundMethod)
            {
                case TangraConfig.BackgroundMethod.BackgroundMode:
                    return GetBackgroundMode(allBgReadings);

                case TangraConfig.BackgroundMethod.AverageBackground:
                    return GetBackgroundPlainFit(allBgReadings);

                case TangraConfig.BackgroundMethod.BackgroundMedian:
                    return GetBackgroundMedian(allBgReadings);
            }

            throw new NotSupportedException();
        }

        private double GetBackgroundMode(List<uint> allBgReadings)
        {
            // Method 1: - Keep recomputing Median and Mean until they stop changing
            //           - Then use MODE (mode = 3 x (median) - 2 x (mean))

            List<float> allBgResiduals = new List<float>();
            uint sum = 0;

            double oldMean = double.NaN;
            double oldMedian = double.NaN;
            double mean = double.NaN;
            double median = double.NaN;
            double mode = 0;

            int numPasses = 0;
            do
            {
                oldMean = mean;
                oldMedian = median;

                sum = 0;
                for (int i = 0; i < allBgReadings.Count; i++)
                    sum += allBgReadings[i];


                mean = 1.0 * sum / allBgReadings.Count;
                median = 0;

                if (allBgReadings.Count > 0)
                {
                    allBgReadings.Sort();
                    if (allBgReadings.Count % 2 == 1)
                    {
                        if (allBgReadings.Count > 2)
                            median = (allBgReadings[-1 + allBgReadings.Count / 2] + allBgReadings[allBgReadings.Count / 2] + allBgReadings[1 + allBgReadings.Count / 2]) / 3.0;
                        else
                            median = allBgReadings[allBgReadings.Count / 2];
                    }
                    else
                    {
                        if (allBgReadings.Count > 3)
                            median = (allBgReadings[(allBgReadings.Count / 2) + 1] + allBgReadings[allBgReadings.Count / 2] + allBgReadings[(allBgReadings.Count / 2) - 1] + allBgReadings[(allBgReadings.Count / 2) - 2]) / 4.0;
                        else
                            median = (allBgReadings[allBgReadings.Count / 2] + allBgReadings[(allBgReadings.Count / 2) - 1]) / 2.0;
                    }

                    mode = 3 * median - 2 * mean;
                }

                // Compute the residuals and StdDev
                double sqVariance = 0;
                allBgResiduals.Clear();
                for (int i = 0; i < allBgReadings.Count; i++)
                {
                    float residual = (float)(allBgReadings[i] - mean);
                    allBgResiduals.Add(residual);
                    sqVariance += residual * residual;
                }
                double stdDev = Math.Sqrt(sqVariance / allBgReadings.Count);

                // Remove all points beyond RejectionBackgoundPixelsStdDevCoeff * sigma
                for (int i = allBgReadings.Count - 1; i >= 0; i--)
                {
                    if (Math.Abs(allBgResiduals[i]) > stdDev)
                    {
                        allBgReadings.RemoveAt(i);
                    }
                }
                numPasses++;
            }
            while (
                (Math.Abs(oldMean - mean) > 5 && Math.Abs(oldMedian - median) > 5) ||
                numPasses < 2);

            return mode;
        }

        private double GetBackgroundMedian(List<uint> allBgReadings)
        {
            double median = 0;

            if (allBgReadings.Count > 0)
            {
                allBgReadings.Sort();
                if (allBgReadings.Count % 2 == 1)
                    median = allBgReadings[allBgReadings.Count / 2];
                else
                    median = (allBgReadings[allBgReadings.Count / 2] + allBgReadings[(allBgReadings.Count / 2) - 1]) / 2.0;
            }

            return median;
        }

        private double GetBackgroundPlainFit(List<uint> allBgReadings)
        {
            // Method 2: Use Average and remove points until 

            List<float> allBgResiduals = new List<float>();
            uint sum = 0;

            for (int i = 0; i < allBgReadings.Count; i++)
                sum += allBgReadings[i];

            int numRemovedPoints = 0;
            double average = 0;
            do
            {
                // Compute the average value
                average = 1.0 * sum / allBgReadings.Count;
                double sqVariance = 0;
                allBgResiduals.Clear();
                // Compute the residuals
                for (int i = 0; i < allBgReadings.Count; i++)
                {
                    float residual = (float)(allBgReadings[0] - average);
                    allBgResiduals.Add(residual);
                    sqVariance += residual * residual;
                }
                sqVariance = Math.Sqrt(sqVariance / (allBgReadings.Count - 1));
                sqVariance = sqVariance * m_RejectionBackgoundPixelsStdDevCoeff;

                numRemovedPoints = 0;
                // Remove all points beyond RejectionBackgoundPixelsStdDevCoeff * sigma
                for (int i = allBgReadings.Count - 1; i >= 0; i--)
                {
                    if (Math.Abs(allBgResiduals[i]) > sqVariance)
                    {
                        numRemovedPoints++;
                        allBgReadings.RemoveAt(i);
                    }
                }

                sum = 0;
                if (numRemovedPoints > 0)
                {
                    for (int i = 0; i < allBgReadings.Count; i++)
                    {
                        sum += allBgReadings[i];
                    }
                }
            }
            while (numRemovedPoints > 0 && allBgReadings.Count > 0);

            return average;
        }

		public NotMeasuredReasons MeasureObject(
            IImagePixel center,
            uint[,] data,
            uint[,] backgroundPixels,
			int bpp,
            TangraConfig.PreProcessingFilter filter,
            bool synchronise,
            TangraConfig.PhotometryReductionMethod reductionMethod,
			TangraConfig.PsfQuadrature psfQuadrature,
            float aperture,
            double refinedFWHM,
            float refinedAverageFWHM,
			IMeasurableObject measurableObject,
            bool fullDisappearance)
        {
            int centerX = (int)Math.Round(center.XDouble);
            int centerY = (int)Math.Round(center.YDouble);

            float msrX0 = (float)center.XDouble;
            float msrY0 = (float)center.YDouble;

            switch (filter)
            {
                case TangraConfig.PreProcessingFilter.LowPassFilter:
					data = ImageFilters.LowPassFilter(data, bpp, true);
					backgroundPixels = ImageFilters.LowPassFilter(backgroundPixels, bpp, true);
                    break;
                case TangraConfig.PreProcessingFilter.LowPassDifferenceFilter:
					data = ImageFilters.LowPassDifferenceFilter(data, bpp, true);
					backgroundPixels = ImageFilters.LowPassDifferenceFilter(backgroundPixels, bpp, true);
                    break;
                default:
                    break;
            }

			if (reductionMethod == TangraConfig.PhotometryReductionMethod.PsfPhotometry)
			{
				if (TangraConfig.Settings.Photometry.PsfFittingMethod == TangraConfig.PsfFittingMethod.DirectNonLinearFit)
				{
					return DoNonLinearProfileFittingPhotometry(
						data, centerX, centerY, msrX0, msrY0,
						aperture,
						measurableObject.PsfFittingMatrixSize,
						psfQuadrature == TangraConfig.PsfQuadrature.NumericalInAperture,
						measurableObject.IsOccultedStar && fullDisappearance,
						backgroundPixels,
						measurableObject.MayHaveDisappeared, 
						refinedFWHM);
				}
				else if (TangraConfig.Settings.Photometry.PsfFittingMethod == TangraConfig.PsfFittingMethod.LinearFitOfAveragedModel)
				{
					float modelFWHM = float.NaN;
					if (TangraConfig.Settings.Photometry.UseUserSpecifiedFWHM)
						modelFWHM = TangraConfig.Settings.Photometry.UserSpecifiedFWHM;
					else
						modelFWHM = refinedAverageFWHM;

					return DoLinearProfileFittingOfAveragedMoodelPhotometry(
						data, centerX, centerY, msrX0, msrY0, modelFWHM,
						aperture,
						measurableObject.PsfFittingMatrixSize,
						psfQuadrature == TangraConfig.PsfQuadrature.NumericalInAperture,
						measurableObject.IsOccultedStar && fullDisappearance,
						backgroundPixels,
						measurableObject.MayHaveDisappeared);
				}
				else
					throw new NotImplementedException();
			}
			else if (reductionMethod == TangraConfig.PhotometryReductionMethod.AperturePhotometry)
			{
				return DoAperturePhotometry(
					data, centerX, centerY, msrX0, msrY0,
					aperture,
					measurableObject.PsfFittingMatrixSize,
					backgroundPixels);
			}
			else if (reductionMethod == TangraConfig.PhotometryReductionMethod.OptimalExtraction)
			{
				return DoOptimalExtractionPhotometry(
					data, centerX, centerY, msrX0, msrY0,
					aperture,
					measurableObject.PsfFittingMatrixSize,
					measurableObject.IsOccultedStar && fullDisappearance,
					backgroundPixels,
					measurableObject.MayHaveDisappeared, 
					refinedFWHM);
			}
			else
				throw new ArgumentOutOfRangeException("reductionMethod");
        }
    }
}
