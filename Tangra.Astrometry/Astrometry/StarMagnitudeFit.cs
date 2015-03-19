/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Model.Image;
using Tangra.Model.Numerical;
using Tangra.Model.VideoOperations;
using Tangra.Photometry;
using Tangra.StarCatalogues;

namespace Tangra.Astrometry
{
	public class StarMagnitudeFit
	{
		private static bool EXCLUDE_BAD_RESIDUALS = false;
		private static bool EXCLUDE_SATURATED_STARS = false;

		private static int MAX_ITERR = 10;

		// Resulting apertue = FWHM * APERTURE_IN_FWHM + FIXED_APERTURE
		private static float APERTURE_IN_FWHM = 0f;
		private static float FIXED_APERTURE = 5f;
		private static uint SATURATION_8BIT = 250;
		private static uint SATURATION_16BIT = 4000;
		private static int PSF_FIT_AREA_SIZE = 11;

		private List<double> m_Intencities;
		private List<double> m_Magnitudes;
		private List<double> m_Colours;
		private List<double> m_Residuals;
		private List<IStar> m_StarNumbers;
		private List<PSFFit> m_PSFGaussians;
		private List<bool> m_SaturatedFlags;
		private List<double> m_ProfileFittedAmplitudes;
		private double m_A;
		private double m_B;
		private double m_C;
		private float m_EncodingGamma;
		private double m_Sigma;
		private double m_MinRes;
		private double m_MaxRes;
		private double m_MinMag;
		private double m_MaxMag;

		private int m_BitPix;

		private TangraConfig.PreProcessingFilter m_Filter;
		private double m_EmpericalFWHM;
		private TangraConfig.PhotometryReductionMethod m_PhotometryReductionMethod;
		private TangraConfig.BackgroundMethod m_PhotometryBackgroundMethod;
		private TangraConfig.PsfQuadrature m_PsfQuadrature;
		private TangraConfig.PsfFittingMethod m_PsfFittingMethod;

		private int m_ExcludedStars = 0;

		private AstroImage m_CurrentAstroImage;

		private MeasurementsHelper m_Measurer;

		public StarMagnitudeFit(
			AstroImage astroImage,
			int bitPix,
			List<double> intencities,
			List<double> magnitudes,
			List<double> colours,
			List<IStar> starNumbers,
			List<PSFFit> psfGaussians,
			List<double> profileFittedAmplitudes,
			List<bool> saturatedFlags,
			double a, double b, double c, float encodingGamma, int excludedStars,
			TangraConfig.PreProcessingFilter filter, double empericalFWHM, 
			TangraConfig.PhotometryReductionMethod photometryReductionMethod, 
			TangraConfig.BackgroundMethod photometryBackgroundMethod,
			TangraConfig.PsfQuadrature psfQuadrature,
			TangraConfig.PsfFittingMethod psfFittingMethod)
		{
			m_CurrentAstroImage = astroImage;
			m_BitPix = bitPix;
			m_Intencities = intencities;
			m_Magnitudes = magnitudes;
			m_Colours = colours;
			m_Residuals = new List<double>();
			m_StarNumbers = starNumbers;
			m_PSFGaussians = psfGaussians;
			m_EncodingGamma = encodingGamma;
			m_ExcludedStars = excludedStars;
			m_SaturatedFlags = saturatedFlags;
			m_ProfileFittedAmplitudes = profileFittedAmplitudes;

			m_Sigma = 0;
			for (int i = 0; i < intencities.Count; i++)
			{
				double computed = a + b * -2.5 * Math.Log10(intencities[i]);
				double diff = Math.Abs(computed - magnitudes[i]);
				m_Residuals.Add(diff);

				m_Sigma += diff*diff;
			}
			m_Sigma = Math.Sqrt(m_Sigma / (m_Residuals.Count - 1));

			m_Filter = filter;
			m_EmpericalFWHM = empericalFWHM;
			m_PhotometryReductionMethod = photometryReductionMethod;
			m_PhotometryBackgroundMethod = photometryBackgroundMethod;
			m_PsfQuadrature = psfQuadrature;
			m_PsfFittingMethod = psfFittingMethod;

			m_A = a;
			m_B = b;
			m_C = c;

			m_Measurer = new MeasurementsHelper(
				m_BitPix,
				m_PhotometryBackgroundMethod, 
				TangraConfig.Settings.Photometry.SubPixelSquareSize,
                TangraConfig.Settings.Photometry.Saturation.GetSaturationForBpp(bitPix));

			m_Measurer.SetCoreProperties(
				TangraConfig.Settings.Photometry.AnnulusInnerRadius,
				TangraConfig.Settings.Photometry.AnnulusMinPixels,
                CorePhotometrySettings.Default.RejectionBackgroundPixelsStdDev,
				2 /* TODO: This must be configurable */);

			m_Sigma = 0;
			m_MinRes = double.MaxValue;
			m_MaxRes = double.MinValue;
			m_MinMag = double.MaxValue;
			m_MaxMag = double.MinValue;
			for (int i = 0; i < m_Residuals.Count; i++)
			{
				double res = m_Residuals[i];

				m_Sigma += res * res;
				if (m_MinRes > res) m_MinRes = res;
				if (m_MaxRes < res) m_MaxRes = res;

				double mag = m_Magnitudes[i];
				if (m_MinMag > mag) m_MinMag = mag;
				if (m_MaxMag < mag) m_MaxMag = mag;
			}

			m_Sigma = Math.Sqrt(m_Sigma / m_Residuals.Count);
		}

		public string GetExportableData()
		{
			StringBuilder bld = new StringBuilder();
			for (int i = 0; i < m_Intencities.Count; i++)
			{
				for (int j = i + 1; j < m_Intencities.Count; j++)
				{
					bld.AppendLine(string.Format("{0}, {1}, {2}, {3}", m_Magnitudes[i], m_Magnitudes[j], m_Intencities[i], m_Intencities[j]));
				}
			}

			return bld.ToString();
		}

		public int ExcludedStars
		{
			get { return m_ExcludedStars; }
		}

		public double A
		{
			get { return m_A; }
		}

		public double B
		{
			get { return m_B; }
		}

		public double C
		{
			get { return m_C; }
		}

		public int NumStars
		{
			get { return m_Residuals.Count; }
		}

		public double Sigma
		{
			get { return m_Sigma; }
		}

		public double MinRes
		{
			get { return m_MinRes; }
		}

		public double MaxRes
		{
			get { return m_MaxRes; }
		}

		public double MinMag
		{
			get { return m_MinMag; }
		}

		public double MaxMag
		{
			get { return m_MaxMag; }
		}

		public float EncodingGamma
		{
			get { return m_EncodingGamma; }
		}

		public double GetMagnitudeForIntencity(double intencity)
		{
			double assumedTargetJK = ColourIndexTables.GetJKFromVR(TangraConfig.Settings.Astrometry.AssumedTargetVRColour);

			return GetMagnitudeForIntencity(intencity, assumedTargetJK);
		}

		public double GetMagnitudeForIntencity(double intencity, double jkColour)
		{
			return m_A * -2.5 * Math.Log10(intencity) + m_B * jkColour + m_C;
		}

		public static double Aperture(double fwhm)
		{
			return fwhm * APERTURE_IN_FWHM + FIXED_APERTURE;
		}

		public double GetIntencity(ImagePixel center, out bool isSaturated)
		{
			isSaturated = false;
			 
            uint[,] data = m_CurrentAstroImage.GetMeasurableAreaPixels(center.X, center.Y, 17);

			PSFFit fit = new PSFFit(center.X, center.Y);
			fit.Fit(data, PSF_FIT_AREA_SIZE);
			if (!fit.IsSolved) 
				return double.NaN;


			int areaSize = m_Filter == TangraConfig.PreProcessingFilter.NoFilter ? 17 : 19;

			int centerX = (int)Math.Round(center.XDouble);
			int centerY = (int)Math.Round(center.YDouble);

			data = m_CurrentAstroImage.GetMeasurableAreaPixels(centerX, centerY, areaSize);
			uint[,] backgroundPixels = m_CurrentAstroImage.GetMeasurableAreaPixels(centerX, centerY, 35);

			m_Measurer.MeasureObject(
				center,
				data,
				backgroundPixels,
				m_CurrentAstroImage.Pixelmap.BitPixCamera,
				m_Filter,
				m_PhotometryReductionMethod,
				m_PsfQuadrature,
				m_PsfFittingMethod,
				(float)Aperture(fit.FWHM),
				fit.FWHM,
				(float)m_EmpericalFWHM,
				new FakeIMeasuredObject(fit), 
				null, null,
				false);

			isSaturated = m_Measurer.HasSaturatedPixels;

			return m_Measurer.TotalReading - m_Measurer.TotalBackground;
		}

		public double GetBackgroundIntencity()
		{
			return double.NaN;
		}

		public List<double> Intencities
		{
			get { return m_Intencities; }
		}

		public List<double> Magnitudes
		{
			get { return m_Magnitudes; }
		}

		public List<IStar> StarNumbers
		{
			get { return m_StarNumbers; }
		}

		public List<PSFFit> PSFGaussians
		{
			get { return m_PSFGaussians; }
		}

		public List<double> ProfileFittedAmplitudes
		{
			get { return m_ProfileFittedAmplitudes; }
		}

		public List<bool> SaturatedFlags
		{
			get { return m_SaturatedFlags; }
		}

		public static void PSFPhotometry(FitInfo astrometricFit, List<IStar> catalogueStars, AstroImage currentAstroImage, Rectangle osdRectToExclude, Rectangle rectToInclude, bool limitByInclusion)
		{
			StringBuilder output = new StringBuilder();

			foreach (PlateConstStarPair pair in astrometricFit.AllStarPairs)
			{
                uint[,] data = currentAstroImage.GetMeasurableAreaPixels(
					(int)Math.Round(pair.x), (int)Math.Round(pair.y), 9);

				if (limitByInclusion && !rectToInclude.Contains((int)pair.x, (int)pair.y)) continue;
				if (!limitByInclusion && osdRectToExclude.Contains((int)pair.x, (int)pair.y)) continue;

				PSFFit gaussian = new PSFFit((int)Math.Round(pair.x), (int)Math.Round(pair.y));
				gaussian.Fit(data);
				if (gaussian.IsSolved)
				{
					IStar star = catalogueStars.Find(s => s.StarNo == pair.StarNo);
					if (star != null &&
						!double.IsNaN(star.MagR))
					{
						output.AppendLine(string.Format("{0}, {1}, {2}, {3}, {4}", pair.StarNo, star.MagR, gaussian.R0, gaussian.IMax, gaussian.I0));
					}
				}
			}

			File.WriteAllText(@"C:\PSF_Photo.csv", output.ToString());
		}


		private class MagFitRecord
		{
			public IStar Star;
			public PlateConstStarPair Pair;
			public PSFFit PsfFit;
			public bool Saturation;
		}

		public static StarMagnitudeFit PerformFit(
			IAstrometryController astrometryController,
			IVideoController videoController,
			int bitPix,
			FitInfo astrometricFit,
			TangraConfig.PhotometryReductionMethod photometryReductionMethod,
			TangraConfig.PsfQuadrature psfQuadrature,
			TangraConfig.PsfFittingMethod psfFittingMethod,
			TangraConfig.BackgroundMethod photometryBackgroundMethod,
			TangraConfig.PreProcessingFilter filter,
			List<IStar> catalogueStars,
            Guid magnitudeBandId,
			float encodingGamma,
			float? aperture,
			ref float empericalPSFR0)
		{
			MeasurementsHelper measurer = new MeasurementsHelper(
				bitPix,
				photometryBackgroundMethod,
				TangraConfig.Settings.Photometry.SubPixelSquareSize,
                TangraConfig.Settings.Photometry.Saturation.GetSaturationForBpp(bitPix));

			measurer.SetCoreProperties(
				TangraConfig.Settings.Photometry.AnnulusInnerRadius,
				TangraConfig.Settings.Photometry.AnnulusMinPixels,
                CorePhotometrySettings.Default.RejectionBackgroundPixelsStdDev,
				2 /* TODO: This must be configurable */);

			var bgProvider = new BackgroundProvider(videoController);
			measurer.GetImagePixelsCallback += new MeasurementsHelper.GetImagePixelsDelegate(bgProvider.measurer_GetImagePixelsCallback);

			List<double> intencities = new List<double>();
			List<double> magnitudes = new List<double>();
			List<double> colours = new List<double>();
			List<double> residuals = new List<double>();
			List<bool> saturatedFlags = new List<bool>();
			List<IStar> stars = new List<IStar>();
			List<PSFFit> gaussians = new List<PSFFit>();

			List<MagFitRecord> fitRecords = new List<MagFitRecord>();

			AstroImage currentAstroImage = videoController.GetCurrentAstroImage(false);
			Rectangle osdRectToExclude = astrometryController.OSDRectToExclude;
			Rectangle rectToInclude = astrometryController.RectToInclude;
			bool limitByInclusion = astrometryController.LimitByInclusion;

			int matSize = CorePhotometrySettings.Default.MatrixSizeForCalibratedPhotometry;
			uint saturatedValue = TangraConfig.Settings.Photometry.Saturation.GetSaturationForBpp(bitPix);

			double a = double.NaN;
			double b = double.NaN;
			double c = double.NaN;
			int excludedStars = 0;
			double empericalFWHM = double.NaN;

			try
			{
				foreach (PlateConstStarPair pair in astrometricFit.AllStarPairs)
				{
					if (limitByInclusion && !rectToInclude.Contains((int)pair.x, (int)pair.y)) continue;
					if (!limitByInclusion && osdRectToExclude.Contains((int)pair.x, (int)pair.y)) continue;

					IStar star = catalogueStars.Find(s => s.StarNo == pair.StarNo);
					if (star == null || double.IsNaN(star.Mag) || star.Mag == 0) continue;

					uint[,] data = currentAstroImage.GetMeasurableAreaPixels((int)pair.x, (int)pair.y, matSize);

					PSFFit fit = new PSFFit((int)pair.x, (int)pair.y);
					fit.Fit(data, PSF_FIT_AREA_SIZE);
					if (!fit.IsSolved) continue;

					MagFitRecord record = new MagFitRecord();
					record.Star = star;
					record.Pair = pair;
					record.PsfFit = fit;
					record.Saturation = IsSaturated(data, matSize, saturatedValue);

					if (!EXCLUDE_SATURATED_STARS || !record.Saturation)
						fitRecords.Add(record);
				}

				// We need the average R0 if it hasn't been determined yet
				if (float.IsNaN(empericalPSFR0))
				{
					empericalPSFR0 = 0;
					foreach (MagFitRecord rec in fitRecords)
					{
						empericalPSFR0 += (float)rec.PsfFit.R0;
					}
					empericalPSFR0 /= fitRecords.Count;
				}

				empericalFWHM = 2 * Math.Sqrt(Math.Log(2)) * empericalPSFR0;

				foreach (MagFitRecord record in fitRecords)
				{
					ImagePixel center = new ImagePixel(255, record.Pair.x, record.Pair.y);
					int areaSize = filter == TangraConfig.PreProcessingFilter.NoFilter ? 17 : 19;

					int centerX = (int)Math.Round(center.XDouble);
					int centerY = (int)Math.Round(center.YDouble);

					uint[,] data = currentAstroImage.GetMeasurableAreaPixels(centerX, centerY, areaSize);
					uint[,] backgroundPixels = currentAstroImage.GetMeasurableAreaPixels(centerX, centerY, 35);

					measurer.MeasureObject(
						center,
						data,
						backgroundPixels,
						currentAstroImage.Pixelmap.BitPixCamera,
						filter,
						photometryReductionMethod,
						psfQuadrature,
						psfFittingMethod,
						aperture != null ? aperture.Value : (float)Aperture(record.PsfFit.FWHM),
						record.PsfFit.FWHM,
						(float)empericalFWHM,
						new FakeIMeasuredObject(record.PsfFit), 
						null, 
						null, 
						false);

					double intensity = measurer.TotalReading - measurer.TotalBackground;
					if (intensity > 0)
					{
						intencities.Add(intensity);
						
						magnitudes.Add(record.Star.GetMagnitudeForBand(magnitudeBandId));
						colours.Add(record.Star.MagJ - record.Star.MagK);

						gaussians.Add(record.PsfFit);
						stars.Add(record.Star);
						saturatedFlags.Add(measurer.HasSaturatedPixels);
					}
				}


				// Remove stars with unusual PSF fit radii (once only)
				double sum = 0;
				for (int i = 0; i < gaussians.Count; i++)
				{
					sum += gaussians[i].R0;
				}
				double averageR = sum / gaussians.Count;

				residuals.Clear();
				sum = 0;
				for (int i = 0; i < gaussians.Count; i++)
				{
					residuals.Add(averageR - gaussians[i].R0);
					sum += (averageR - gaussians[i].R0) * (averageR - gaussians[i].R0);
				}
				double stdDev = Math.Sqrt(sum) / gaussians.Count;

				if (EXCLUDE_BAD_RESIDUALS)
				{
					for (int i = residuals.Count - 1; i >= 0; i--)
					{
						if (Math.Abs(residuals[i]) > 6 * stdDev)
						{
							intencities.RemoveAt(i);
							magnitudes.RemoveAt(i);
							colours.RemoveAt(i);
							stars.RemoveAt(i);
							gaussians.RemoveAt(i);
							saturatedFlags.RemoveAt(i);
						}
					}
				}

				double maxResidual = Math.Max(0.1, TangraConfig.Settings.Photometry.MaxResidualStellarMags);

				for (int itter = 1; itter <= MAX_ITERR; itter++)
				{
					residuals.Clear();

					SafeMatrix A = new SafeMatrix(intencities.Count, 3);
					SafeMatrix X = new SafeMatrix(intencities.Count, 1);

					int idx = 0;
					for (int i = 0; i < intencities.Count; i++)
					{
						A[idx, 0] = magnitudes[i];
						A[idx, 1] = colours[i];
						A[idx, 2] = 1;

						X[idx, 0] = -2.5 * Math.Log10(intencities[i]);

						idx++;
					}

					SafeMatrix a_T = A.Transpose();
					SafeMatrix aa = a_T * A;
					SafeMatrix aa_inv = aa.Inverse();
					SafeMatrix bx = (aa_inv * a_T) * X;

					double Ka = bx[0, 0];
					double Kb = bx[1, 0];
					double Kc = bx[2, 0];

					// -2.5 * a * Log(Median-Intensity) = A * Mv + B * Mjk + C - b
					// -2.5 * Log(Median-Intensity) = Ka * Mv + Kb * Mjk + Kc
					// Mv = -2.5 * a * Log(Median-Intensity) - b * Mjk - c
					a = 1 / Ka;
					b = -Kb / Ka;
					c = -Kc / Ka;

					int starsExcludedThisTime = 0;

					if (EXCLUDE_BAD_RESIDUALS)
					{
						List<int> indexesToRemove = new List<int>();
						for (int i = 0; i < intencities.Count; i++)
						{
							double computed = a * -2.5 * Math.Log10(intencities[i]) + b * colours[i] + c;

							double diff = Math.Abs(computed - magnitudes[i]);
							if (itter < MAX_ITERR)
							{
								if (Math.Abs(diff) > maxResidual)
								{
									indexesToRemove.Add(i);
								}
							}
							else
								residuals.Add(diff);
						}


						for (int i = indexesToRemove.Count - 1; i >= 0; i--)
						{
							int idxToRemove = indexesToRemove[i];
							intencities.RemoveAt(idxToRemove);
							magnitudes.RemoveAt(idxToRemove);
							colours.RemoveAt(idxToRemove);
							stars.RemoveAt(idxToRemove);
							gaussians.RemoveAt(idxToRemove);
							saturatedFlags.RemoveAt(idxToRemove);

							excludedStars++;
							starsExcludedThisTime++;
						}
					}

					if (starsExcludedThisTime == 0)
						break;
				}					
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.ToString());
			}

			return new StarMagnitudeFit(
				currentAstroImage,
				bitPix,
				intencities, magnitudes, colours, stars, gaussians, new List<double>(), 
				saturatedFlags, a, b, c, encodingGamma, excludedStars, filter, empericalFWHM, 
				photometryReductionMethod, photometryBackgroundMethod, psfQuadrature, psfFittingMethod);
		}

		public static bool IsSaturated(uint[,] data, int matSize, uint saturatedValue)
		{
			for (int x = 0; x < matSize; x++)
			{
				for (int y = 0; y < matSize; y++)
				{
					if (data[x, y] >= saturatedValue)
						return true;
				}
			}

			return false;
		}
	}

	public class BackgroundProvider
	{
		private IVideoController m_VideoController;

		public BackgroundProvider(IVideoController videoController)
		{
			m_VideoController = videoController;
		}

		public uint[,] measurer_GetImagePixelsCallback(int x, int y, int matrixSize)
		{
			AstroImage currentAstroImage = m_VideoController.GetCurrentAstroImage(false);
			return currentAstroImage.GetMeasurableAreaPixels(x, y, matrixSize);
		}
	}
}
