using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.VideoOperations.Spectroscopy.AbsFluxCalibration;

namespace Tangra.VideoOperations.Spectroscopy.FilterResponses
{
	[Flags]
	public enum ExportedMags
	{
		None = 0,
		Johnson_B = 1,
		Johnson_V = 2,
		Johnson_R = 4,
		Sloan_g = 8,
		Sloan_r = 16,
		Sloan_i = 32
	}

	internal class SyntheticMagnitudeProducer
	{
		internal void ExportMagnitudes(ExportedMags mags, List<AbsFluxSpectra> spectra, List<AbsFluxSpectra> standardSpectras, StringBuilder output)
		{
			if ((mags & ExportedMags.Johnson_B) == ExportedMags.Johnson_B)
				ExportMagnitudes(FilterResponseDatabase.Instance.Johnson_B, spectra, standardSpectras, output);

			if ((mags & ExportedMags.Johnson_V) == ExportedMags.Johnson_V)
				ExportMagnitudes(FilterResponseDatabase.Instance.Johnson_V, spectra, standardSpectras, output);

			if ((mags & ExportedMags.Johnson_R) == ExportedMags.Johnson_R)
				ExportMagnitudes(FilterResponseDatabase.Instance.Johnson_R, spectra, standardSpectras, output);

			if ((mags & ExportedMags.Sloan_g) == ExportedMags.Sloan_g)
				ExportMagnitudes(FilterResponseDatabase.Instance.SLOAN_g, spectra, standardSpectras, output);

			if ((mags & ExportedMags.Sloan_r) == ExportedMags.Sloan_r)
				ExportMagnitudes(FilterResponseDatabase.Instance.SLOAN_r, spectra, standardSpectras, output);

			if ((mags & ExportedMags.Sloan_i) == ExportedMags.Sloan_i)
				ExportMagnitudes(FilterResponseDatabase.Instance.SLOAN_i, spectra, standardSpectras, output);
		}

		private void ExportMagnitudes(FilterResponse filterResponse, List<AbsFluxSpectra> spectra, List<AbsFluxSpectra> standardSpectras, StringBuilder output)
		{
			EnsureReferenceSums(filterResponse, standardSpectras);

			output.Append(filterResponse.Designation);
			for (int j = 0; j < spectra.Count; j++)
			{
				output.Append(",");
				double magError;
				double syntheticMag = ComputeSyntheticMag(filterResponse, spectra[j], out magError);
				output.Append(syntheticMag.ToString("0.00") + " +/- " + magError.ToString("0.00"));
			}
			output.AppendLine();
		}

		private Dictionary<double, double> m_SynthetizedReferneceFluxMagnitudes = new Dictionary<double, double>();
		private double m_AverageAbsFluxFitMagError;

		private double GetReferenceMagnitude(FilterResponse filterResponse, CalSpecStar star)
		{
			switch (filterResponse.Band)
			{
				case PhotometricBand.B:
					return star.MagB;
				case PhotometricBand.V:
					return star.MagV;
				case PhotometricBand.R:
					return star.MagR;
				case PhotometricBand.Sloan_g:
					return star.Mag_g;
				case PhotometricBand.Sloan_r:
					return star.Mag_r;
				case PhotometricBand.Sloan_i:
					return star.Mag_i;
				default:
					return star.MagV;
			}
		}
		private void EnsureReferenceSums(FilterResponse filterResponse, List<AbsFluxSpectra> standardSpectras)
		{
			m_SynthetizedReferneceFluxMagnitudes.Clear();
			var standardAbsFluxFitMagErrors = new List<double>();

			foreach (CalSpecStar star in CalSpecDatabase.Instance.Stars)
			{
				AbsFluxSpectra fittedSpectra = standardSpectras.FirstOrDefault(x => x.m_CalSpecStar.CalSpecStarId == star.CalSpecStarId);
				if (fittedSpectra == null) continue;

				List<double> wavelengths = fittedSpectra.ResolvedWavelengths;
				List<double> fluxes = fittedSpectra.AbsoluteFluxes;
				List<double> residuals = fittedSpectra.Residuals;

				double referenceSum = 0;
				double prevNonNaNVal = 0;

				double residualSum = 0;
				double prevNonNaNResidualVal = 0;

				foreach (int wavelength in filterResponse.Response.Keys)
				{
					double responseCoeff = filterResponse.Response[wavelength];

					double val = InterpolateValue(wavelengths, fluxes, wavelength);

					if (!double.IsNaN(val)) prevNonNaNVal = val;
					referenceSum += prevNonNaNVal * responseCoeff;

					double residualVal = InterpolateValue(wavelengths, residuals, wavelength);

					if (!double.IsNaN(residualVal)) prevNonNaNResidualVal = residualVal;
					residualSum += prevNonNaNResidualVal * responseCoeff;
				}

				standardAbsFluxFitMagErrors.Add(Math.Abs(Math.Log10((referenceSum + residualSum)/referenceSum)));

				m_SynthetizedReferneceFluxMagnitudes.Add(referenceSum, GetReferenceMagnitude(filterResponse, star));
			}

			// NOTE: Is this a good way to estimate the error?
			m_AverageAbsFluxFitMagError = standardAbsFluxFitMagErrors.Average(); // / Math.Sqrt(standardAbsFluxFitMagErrors.Count);
		}

		private double ComputeSyntheticMag(FilterResponse filterResponse, AbsFluxSpectra spectra, out double magError)
		{
			double targetSum = 0;
			double prevNonNaNVal = 0;
			
			foreach (int wavelength in filterResponse.Response.Keys)
			{
				double responseCoeff = filterResponse.Response[wavelength];

				double targetVal = InterpolateValue(spectra.ResolvedWavelengths, spectra.AbsoluteFluxes, wavelength);
				if (!double.IsNaN(targetVal)) prevNonNaNVal = targetVal;

				targetSum += prevNonNaNVal * responseCoeff;
			}

			var allMags = new List<double>();
			foreach (double conv in m_SynthetizedReferneceFluxMagnitudes.Keys)
			{
				double mag = m_SynthetizedReferneceFluxMagnitudes[conv] + 2.5 * Math.Log10(conv / targetSum);
				allMags.Add(mag);
			}

			// First itteration
			double averageMag = allMags.Average();
			double threeSigma = 3 * Math.Sqrt(allMags.Select(x => (averageMag - x) * (averageMag - x)).Sum()) / (allMags.Count - 1);
			allMags = allMags.Where(x => (Math.Abs(x - averageMag) <= threeSigma)).ToList();

			// Second itteration after removing outliers
			averageMag = allMags.Average();
			magError = m_AverageAbsFluxFitMagError + Math.Sqrt(allMags.Select(x => (averageMag - x) * (averageMag - x)).Sum()) / (allMags.Count - 1);
			return averageMag;
		}

		private double InterpolateValue(List<double> x, List<double> y, double x1)
		{
			for (int i = 0; i < x.Count - 1; i++)
			{
				if (x[i] < x1 && x[i + 1] >= x1)
				{
					return y[i] + (y[i + 1] - y[i]) * ((x1 - x[i]) / (x[i + 1] - x[i]));
				}
			}

			return Double.NaN;
		}
	}
}
