using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Model.Helpers;
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
	    private int m_SubPixelSquare = 0;
        private uint m_SaturationValue = 255;
        private float m_TimesHigherPositionToleranceForFullyOccultedStars = 2.0f;
        private int m_BitPix = 8;
        private uint m_MaxPixelValue = 256;

        public MeasurementsHelper(
            int bitPix,
            TangraConfig.BackgroundMethod backgroundMethod,
            int subPixelSquare,
            uint saturationValue)
        {
            m_BitPix = bitPix;
            m_MaxPixelValue = Pixelmap.GetMaxValueForBitPix(m_BitPix);
            m_BackgroundMethod = backgroundMethod;
			m_SubPixelSquare = subPixelSquare;
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
            MeasurementsHelper clone = new MeasurementsHelper(m_BitPix, m_BackgroundMethod, m_SubPixelSquare, m_SaturationValue);
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
	    private float m_Aperture;

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

	    public float Aperture
	    {
			get { return m_Aperture; }
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
			uint[,] backgroundArea, float bgAnnulusFactor,
            IBackgroundModelProvider backgroundModel,
			int centerX = 0, int centerY = 0)
        {
            if (matrix.GetLength(0) != 17 && matrix.GetLength(0) != 35)
                throw new ApplicationException("Measurement error. Correlation: AFP-101");

			int side = matrix.GetLength(0);
			float halfSide = 1.0f*side / 2f;

            m_HasSaturatedPixels = false;
            m_PSFBackground = double.NaN;
            m_FoundBestPSFFit = null;
			m_XCenter = x0 - x0Int + halfSide;
			m_YCenter = y0 - y0Int + halfSide;
			m_Aperture = aperture;
            m_PixelData = matrix;

            m_TotalPixels = 0;

			m_TotalReading = GetReading(
				m_Aperture,
				m_PixelData, side, side,
				m_XCenter, m_YCenter, null, ref m_TotalPixels, centerX - 8, centerY - 8);

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
                    int offset = (35 - side) / 2;
                    m_TotalBackground = Get3DPolynomialBackground(backgroundArea, m_Aperture, m_XCenter + offset, m_YCenter + offset, backgroundModel);
				}
	            else if (m_BackgroundMethod != TangraConfig.BackgroundMethod.PSFBackground)
	            {
                    int offset = (35 - side) / 2;
                    double bgFromAnnulus = GetBackground(backgroundArea, m_Aperture, m_XCenter + offset, m_YCenter + offset, bgAnnulusFactor);
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
			PSFFit fit,
            uint[,] matrix, int x0Int, int y0Int, float x0, float y0,
            float aperture, int matrixSize, bool useNumericalQadrature,
            bool isFullyDisappearingOccultedStar,
            uint[,] backgroundArea,
            bool mayBeOcculted /* Some magic based on a pure guess */,
            double refinedFWHM, float bgAnnulusFactor)
        {
            double distance = ImagePixel.ComputeDistance(fit.XCenter, x0, fit.YCenter, y0);
            double tolerance = isFullyDisappearingOccultedStar
                ? m_TimesHigherPositionToleranceForFullyOccultedStars * m_PositionTolerance
                : m_PositionTolerance;

            if (fit.IsSolved)                
				SetPsfFitReading(fit, aperture, useNumericalQadrature, backgroundArea, bgAnnulusFactor);
            else
            {
				m_Aperture = aperture;
				m_XCenter = (float)fit.X0_Matrix;
				m_YCenter = (float)fit.Y0_Matrix;
	            m_TotalReading = 0;
				m_TotalBackground = 0;
            }

            if (!fit.IsSolved || // The PSF solution failed, mark the reading invalid
                (distance > tolerance && !mayBeOcculted) || // If this doesn't look like a full disappearance, then make the reading invalid
                (fit.FWHM < 0.5 * refinedFWHM || fit.FWHM > 1.5 * refinedFWHM) // The FWHM is too small or too large, make the reading invalid
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
			PSFFit fit,
            uint[,] matrix, int x0Int, int y0Int, float x0, float y0, float modelFWHM,
            float aperture, int matrixSize, bool useNumericalQadrature,
            bool isFullyDisappearingOccultedStar,
            uint[,] backgroundArea,
            bool mayBeOcculted /* Some magic based on a pure guess */,
			float bgAnnulusFactor,
            IBackgroundModelProvider backgroundModel)
        {
            double distance = ImagePixel.ComputeDistance(fit.XCenter, x0, fit.YCenter, y0);
            double tolerance = isFullyDisappearingOccultedStar
                ? m_TimesHigherPositionToleranceForFullyOccultedStars * m_PositionTolerance
                : m_PositionTolerance;

            // We first go and do the measurement anyway
            if (fit.IsSolved)
                SetPsfFitReading(fit, aperture, useNumericalQadrature, backgroundArea, bgAnnulusFactor, backgroundModel);
            else
            {
				m_Aperture = aperture;
				m_XCenter = (float)fit.X0_Matrix;
				m_YCenter = (float)fit.Y0_Matrix;
				m_TotalReading = 0;
				m_TotalBackground = 0;
            }

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

        private void SetPsfFitReading(PSFFit fit, float aperture, bool useNumericalQadrature, uint[,] backgroundArea, float bgAnnulusFactor, IBackgroundModelProvider backgroundModel = null)
        {
			m_Aperture = aperture;
	        m_XCenter = (float)fit.X0_Matrix;
			m_YCenter = (float)fit.Y0_Matrix;

            if (useNumericalQadrature)
            {
                double r0Square = fit.R0 * fit.R0;

                m_TotalReading = Integration.IntegrationOverCircularArea(
                    delegate(double x, double y)
                    {
                        double combinedValue = fit.I0 + (fit.IMax - fit.I0) * Math.Exp((-x * x + -y * y) / r0Square);

                        if (fit.UsesBackgroundModel)
                            return combinedValue + fit.GetFittedBackgroundModelValue(x, y);
                        else
                            return combinedValue;
                    },
					m_Aperture);

                if (m_BackgroundMethod == TangraConfig.BackgroundMethod.PSFBackground)
                {
					m_TotalBackground = Math.PI * m_Aperture * m_Aperture * fit.I0;
                }
				else if (m_BackgroundMethod == TangraConfig.BackgroundMethod.Background3DPolynomial)
				{
                    if (!fit.UsesBackgroundModel && backgroundModel == null)
						throw new InvalidOperationException("3D-Polynomial was not applied correctly.");

                    int offset = (35 - fit.MatrixSize) / 2;
                    m_TotalBackground = Integration.IntegrationOverCircularArea(
                        (x, y) => fit.UsesBackgroundModel ? fit.GetFittedBackgroundModelValue(x, y) : backgroundModel.ComputeValue(x + offset, y + offset),
                        m_Aperture);
				}
                else
                {
                    int offset = (35 - fit.MatrixSize) / 2;
                    double bgFromAnnulus = GetBackground(backgroundArea, m_Aperture, m_XCenter + offset, m_YCenter + offset, bgAnnulusFactor);
					m_TotalBackground = Math.PI * m_Aperture * m_Aperture * (float)bgFromAnnulus;
                }
            }
            else
            {
                // Analytical quadrature (Gauss Integral)		
                if (m_BackgroundMethod == TangraConfig.BackgroundMethod.PSFBackground)
                {
                    m_TotalReading = (fit.IMax - fit.I0) * fit.R0 * fit.R0 * Math.PI;
                    m_TotalBackground = 0; // The background has been already included in the TotalReading
                }
				else if (m_BackgroundMethod == TangraConfig.BackgroundMethod.Background3DPolynomial)
				{
					if (!fit.UsesBackgroundModel)
						throw new InvalidOperationException("3D-Polynomial was not applied correctly.");

					m_TotalReading = (fit.IMax - fit.I0) * fit.R0 * fit.R0 * Math.PI;
					m_TotalBackground = 0; // The background has been already included in the TotalReading
				}
                else
				{
                    throw new InvalidOperationException("Analytical quadrature only works with PSFBackground and Background3DPolynomial.");
				}
            }
        }

		internal NotMeasuredReasons DoOptimalExtractionPhotometry(
			PSFFit fit,
            uint[,] matrix, int x0Int, int y0Int, float x0, float y0,
            float aperture, int matrixSize, bool isFullyDisappearingOccultedStar,
            uint[,] backgroundArea,
            bool mayBeOcculted /* Some magic based on a pure guess */,
			double refinedFWHM, float bgAnnulusFactor)
        {
            // Proceed as with PSF Non linear, Then find the variance of the PSF fit and use a weight based on the residuals of the PSF fit
            // Signal[x, y] = PSF(x, y) + weight * Residual(x, y)

            
            double distance = ImagePixel.ComputeDistance(fit.XCenter, x0, fit.YCenter, y0);
            double tolerance = isFullyDisappearingOccultedStar
                ? m_TimesHigherPositionToleranceForFullyOccultedStars * m_PositionTolerance
                : m_PositionTolerance;

            float psfBackground = (float)fit.I0;

            // almost like aperture            
			if (matrix.GetLength(0) != 17 && matrix.GetLength(0) != 35)
                throw new ApplicationException("Measurement error. Correlation: AFP-101B");

			int side = matrix.GetLength(0);
			float halfSide = side * 1f / 2f;

            m_HasSaturatedPixels = false;
            m_PSFBackground = double.NaN;
            m_FoundBestPSFFit = null;
			m_XCenter = x0 - x0Int + halfSide;
			m_YCenter = y0 - y0Int + halfSide;
			m_Aperture = aperture;
            m_PixelData = matrix;

            m_TotalPixels = 0;

            double varR0Sq = fit.GetVariance();
            varR0Sq = varR0Sq * varR0Sq * 2;

            double deltaX = fit.X0_Matrix - m_XCenter;
            double deltaY = fit.Y0_Matrix - m_YCenter;

            m_TotalReading = GetReading(
				m_Aperture,
                m_PixelData, side, side,
                m_XCenter, m_YCenter,
                delegate(int x, int y)
                {
                    double videoPixel = m_PixelData[x, y];
                    double psfValue = fit.GetValue(x + deltaX, y + deltaY);
                    if (fit.UsesBackgroundModel) psfValue += fit.GetFittedBackgroundModelValue(x + deltaX, y + deltaY);
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
					if (!fit.UsesBackgroundModel)
						throw new InvalidOperationException("3D-Polynomial was not applied correctly.");

                    m_TotalBackground = Integration.IntegrationOverCircularArea(
                        (x, y) => fit.GetFittedBackgroundModelValue(x, y),
                        m_Aperture);
				}
                else if (m_BackgroundMethod != TangraConfig.BackgroundMethod.PSFBackground)
                {
                    int offset = (35 - fit.MatrixSize) / 2;
                    double bgFromAnnulus = GetBackground(backgroundArea, m_Aperture, m_XCenter + offset, m_YCenter + offset, bgAnnulusFactor);
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

		public void FindBestFit(float x0, float y0, Filter filter, uint[,] matrix, int bpp, ref int matrixSize, bool fixedAperture)
        {
			FindBestFit(x0, y0, filter, matrix, bpp, true, ref matrixSize, fixedAperture, false);
        }

        public double BestMatrixSizeDistanceDifferenceTolerance = 3.0;

        private void FindBestFit(
            float x0, float y0,
			Filter filter, uint[,] matrix, int bpp,
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
                }
            }
            else
            {
                m_XCenter = halfDataSize - 1;
                m_YCenter = halfDataSize - 1;
            }

			m_TotalReading = 0;
			m_TotalBackground = 0;
	        m_PSFBackground = 0;
        }

        private delegate double GetPixelMeasurementCallback(int x, int y);

        private double GetReading(
            float aperture, uint[,] data,
            int nWidth, int nHeight,
            float x0, float y0,
            GetPixelMeasurementCallback pixelMeasurementCallback,
            ref float totalPixels,
			int xCent = 0, int yCent = 0)
        {
			double total = 0;
            totalPixels = 0;

            for (int x = 0; x < nWidth; x++)
                for (int y = 0; y < nHeight; y++)
                {
	                DebugContext.SubPixelData subPixelInfo = null;
					if (DebugContext.DebugSubPixelMeasurements)
					{
						subPixelInfo = new DebugContext.SubPixelData();
						DebugContext.CurrentSubPixels[DebugContext.TargetNo][x + xCent, y + yCent] = subPixelInfo;
						subPixelInfo.X0 = x0 + xCent;
						subPixelInfo.Y0 = y0 + yCent;
					}

					double dist = Math.Sqrt((x0 - x) * (x0 - x) + (y0 - y) * (y0 - y));
                    if (dist + 1.5 <= aperture)
                    {
                        // If the point plus 1 pixel diagonal is still in the aperture
                        // then add the readin directly

	                    if (pixelMeasurementCallback != null)
		                    total += pixelMeasurementCallback(x, y);
	                    else
		                    total += (int) data[x, y];

	                    totalPixels++;

						if (DebugContext.DebugSubPixelMeasurements)
							subPixelInfo.FullyIncluded = true;

					}
                    else if (dist - 1.5 <= aperture)
                    {
                        float subpixels = 0;
	                    bool hasSubpixels = false;
						bool isEvenSide = m_SubPixelSquare % 2 == 0;
						if (m_SubPixelSquare > 0)
						{
							if (DebugContext.DebugSubPixelMeasurements)
								subPixelInfo.Included = new bool[m_SubPixelSquare, m_SubPixelSquare];

							for (int dx = 0; dx < m_SubPixelSquare; dx++)
								for (int dy = 0; dy < m_SubPixelSquare; dy++)
                                {
									double xx = isEvenSide
										? x - 1.0 + (2.0 * dx + 1.0) / (2.0 * m_SubPixelSquare)
										: x - 0.5 + (dx - (m_SubPixelSquare / 2)) * 1.0 / m_SubPixelSquare;

									double yy = isEvenSide
										? y - 1.0 + (2.0 * dy + 1.0) / (2.0 * m_SubPixelSquare)
										: y - 0.5 + (dy - (m_SubPixelSquare / 2)) * 1.0 / m_SubPixelSquare;

                                    dist = Math.Sqrt((x0 - xx) * (x0 - xx) + (y0 - yy) * (y0 - yy));
	                                if (dist <= aperture)
	                                {
		                                subpixels += 1.0f/(m_SubPixelSquare*m_SubPixelSquare);
		                                hasSubpixels = true;
										if (DebugContext.DebugSubPixelMeasurements)
											subPixelInfo.Included[dx, dy] = true;
	                                }
                                }
                        }
						else if (dist <= aperture)
						{
							subpixels = 1.0f;
							hasSubpixels = true;
							if (DebugContext.DebugSubPixelMeasurements)
								subPixelInfo.FullyIncluded = true;
						}

						if (hasSubpixels)
						{
							if (pixelMeasurementCallback != null)
								total += pixelMeasurementCallback(x, y)*subpixels;
							else
								total += (int) data[x, y]*subpixels;

							totalPixels += subpixels;
						}

						if (DebugContext.DebugSubPixelMeasurements)
						{
							subPixelInfo.TotalSubpixels = subpixels;
							subPixelInfo.TotalReading = total;
						}
                    }

                    if (data[x, y] >= m_SaturationValue) m_HasSaturatedPixels = true;
	                
                }

            return Math.Round(total);
        }

        private double Get3DPolynomialBackground(uint[,] backgroundArea, float aperture, double x0, double y0, IBackgroundModelProvider backgroundModel)
		{
			int side = backgroundArea.GetLength(0);

			float totalBgPixels = 0;
			return GetReading(
				aperture,
				backgroundArea, side, side,
                (float)x0, (float)y0,
                (x, y) => backgroundModel.ComputeValue(x, y), 
                ref totalBgPixels);
		}

		private double GetBackground(uint[,] pixels, float signalApertureRadius, double x0, double y0, float bgAnnulusFactor)
        {
            int nWidth = pixels.GetLength(0);
            int nHeight = pixels.GetLength(1);

			float innerRadius = (float)(bgAnnulusFactor * m_InnerRadiusOfBackgroundApertureInSignalApertures * signalApertureRadius);
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

				case TangraConfig.BackgroundMethod.Background3DPolynomial:
					throw new ApplicationException("Measurement error. Correlation: 3DP-103");
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
            TangraConfig.PsfFittingMethod psfFittingMethod,
            float aperture,
            double refinedFWHM,
            float refinedAverageFWHM,
			IMeasurableObject measurableObject,
			IImagePixel[] objectsInGroup,
			float[] aperturesInGroup,
            bool fullDisappearance)
        {
			// NOTE: This is how the center of the pixel area passed in data and background arrays is determined
			// TODO: Pass the center as an argument
            int centerX = (int)Math.Round(center.XDouble);
            int centerY = (int)Math.Round(center.YDouble);

            float msrX0 = (float)center.XDouble;
            float msrY0 = (float)center.YDouble;

			float bgAnnulusFactor = 1;

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

			float modelFWHM = float.NaN;
            if (psfFittingMethod == TangraConfig.PsfFittingMethod.LinearFitOfAveragedModel)
			{
				if (TangraConfig.Settings.Photometry.UseUserSpecifiedFWHM)
					modelFWHM = TangraConfig.Settings.Photometry.UserSpecifiedFWHM;
				else
					modelFWHM = refinedAverageFWHM;
			}

			DoublePSFFit doublefit = null;
			PSFFit fit = null;
		    IBackgroundModelProvider backgroundModel = null;

            // 1 - Fit a PSF to the current obejct
			if (objectsInGroup != null && objectsInGroup.Length == 2)
			{
                // 1A - When this is a star group
				int x1 = (int) Math.Round((data.GetLength(0)/2) + objectsInGroup[0].XDouble - center.XDouble);
				int y1 = (int) Math.Round((data.GetLength(0)/2) + objectsInGroup[0].YDouble - center.YDouble);
				int x2 = (int) Math.Round((data.GetLength(0)/2) + objectsInGroup[1].XDouble - center.XDouble);
				int y2 = (int) Math.Round((data.GetLength(0)/2) + objectsInGroup[1].YDouble - center.YDouble);
				doublefit = new DoublePSFFit(centerX, centerY);

                if (psfFittingMethod == TangraConfig.PsfFittingMethod.LinearFitOfAveragedModel &&
					!float.IsNaN(modelFWHM))
				{
					doublefit.FittingMethod = PSFFittingMethod.LinearFitOfAveragedModel;
					doublefit.SetAveragedModelFWHM(modelFWHM);
				}

				doublefit.Fit(data, x1, y1, x2, y2);

				PSFFit star1 = doublefit.GetGaussian1();
				PSFFit star2 = doublefit.GetGaussian2();

				if (m_BackgroundMethod == TangraConfig.BackgroundMethod.Background3DPolynomial)
				{
					var bg3dFit = new Background3DPolynomialFit();
					bg3dFit.Fit(data, star1, star2);

					doublefit.Fit(data, bg3dFit, x1, y1, x2, y2);

					star1 = doublefit.GetGaussian1();
					star2 = doublefit.GetGaussian2();
                    backgroundModel = bg3dFit;
				}

				double d1 = ImagePixel.ComputeDistance(measurableObject.Center.XDouble, doublefit.X1Center, measurableObject.Center.YDouble, doublefit.Y1Center);
				double d2 = ImagePixel.ComputeDistance(measurableObject.Center.XDouble, doublefit.X2Center, measurableObject.Center.YDouble, doublefit.Y2Center);
				
				fit = (d1 < d2) ? star1 : star2;

				if (reductionMethod == TangraConfig.PhotometryReductionMethod.AperturePhotometry)
				{
					// NOTE: If aperture photometry is used, we measure the double object in a single larger aperture centered at the middle
					double alpha = Math.Atan((star2.YCenter - star1.YCenter) / (star2.XCenter - star1.XCenter));

					float dx = (float)((star1.FWHM - star2.FWHM) * Math.Cos(alpha));
					float dy = (float)((star1.FWHM - star2.FWHM) * Math.Sin(alpha));

					msrX0 = (float)(star1.XCenter - star1.FWHM + star2.XCenter + star2.FWHM) / 2.0f - dx;
					msrY0 = (float)(star1.YCenter - star1.FWHM + star2.YCenter + star2.FWHM) / 2.0f - dy; 
					
					aperture = aperturesInGroup.Sum();
					bgAnnulusFactor = 0.67f;
				}
			}
			else if (reductionMethod != TangraConfig.PhotometryReductionMethod.AperturePhotometry)
			{
                // 1B - When this is a single star
				fit = new PSFFit(centerX, centerY);

                if (psfFittingMethod == TangraConfig.PsfFittingMethod.LinearFitOfAveragedModel &&
					!float.IsNaN(modelFWHM))
				{
					fit.FittingMethod = PSFFittingMethod.LinearFitOfAveragedModel;
					fit.SetAveragedModelFWHM(modelFWHM);
				}

				fit.Fit(data, measurableObject.PsfFittingMatrixSize);

				if (m_BackgroundMethod == TangraConfig.BackgroundMethod.Background3DPolynomial)
				{
                    // If 3D Poly Background is used then fit the background, and supply it to the PSF Fitting 
					var bg3dFit = new Background3DPolynomialFit();
					bg3dFit.Fit(backgroundPixels, fit, null);

				    backgroundModel = bg3dFit;

                    if (psfFittingMethod != TangraConfig.PsfFittingMethod.LinearFitOfAveragedModel) 
                        /* 3D Poly modelling works in a direct fit only with non-linear fitting */
                        fit.Fit(backgroundPixels, bg3dFit, measurableObject.PsfFittingMatrixSize);
				}
			}
            else if (
                reductionMethod == TangraConfig.PhotometryReductionMethod.AperturePhotometry && 
                m_BackgroundMethod == TangraConfig.BackgroundMethod.Background3DPolynomial)
            {
                // 1C - Single star with aperture photometry and 3D Poly Background
                var bg3dFit = new Background3DPolynomialFit();
                bg3dFit.Fit(backgroundPixels, (float)(centerX - msrX0 + 17), (float)(centerY - msrY0 + 17), 2 * aperture);

                backgroundModel = bg3dFit;
            }
			
            // 2 - Do the actual photometric measurements (signal and background) based on the selected methods
			if (reductionMethod == TangraConfig.PhotometryReductionMethod.PsfPhotometry)
			{
                // 2A - PSF Photometry
				if (TangraConfig.Settings.Photometry.PsfFittingMethod == TangraConfig.PsfFittingMethod.DirectNonLinearFit)
				{
					return DoNonLinearProfileFittingPhotometry(
						fit,
						data, centerX, centerY, msrX0, msrY0,
						aperture,
						measurableObject.PsfFittingMatrixSize,
						psfQuadrature == TangraConfig.PsfQuadrature.NumericalInAperture,
						measurableObject.IsOccultedStar && fullDisappearance,
						backgroundPixels,
						measurableObject.MayHaveDisappeared,
						refinedFWHM, 
						bgAnnulusFactor);
				}
                else if (psfFittingMethod == TangraConfig.PsfFittingMethod.LinearFitOfAveragedModel)
				{
					return DoLinearProfileFittingOfAveragedMoodelPhotometry(
						fit,
						data, centerX, centerY, msrX0, msrY0, modelFWHM,
						aperture,
						measurableObject.PsfFittingMatrixSize,
						psfQuadrature == TangraConfig.PsfQuadrature.NumericalInAperture,
						measurableObject.IsOccultedStar && fullDisappearance,
						backgroundPixels,
						measurableObject.MayHaveDisappeared,
                        bgAnnulusFactor, backgroundModel);
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
                    backgroundPixels, bgAnnulusFactor, backgroundModel,
                    measurableObject.Center.X, measurableObject.Center.Y);
			}
			else if (reductionMethod == TangraConfig.PhotometryReductionMethod.OptimalExtraction)
			{
				return DoOptimalExtractionPhotometry(
					fit,
					data, centerX, centerY, msrX0, msrY0,
					aperture,
					measurableObject.PsfFittingMatrixSize,
					measurableObject.IsOccultedStar && fullDisappearance,
					backgroundPixels,
					measurableObject.MayHaveDisappeared,
					refinedFWHM, bgAnnulusFactor);
			}
			else
				throw new ArgumentOutOfRangeException("reductionMethod");
        }
    }
}
