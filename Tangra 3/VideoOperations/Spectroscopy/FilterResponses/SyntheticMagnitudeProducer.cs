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
		Johnson_U = 1,
		Johnson_B = 2,
		Johnson_V = 4,
		Johnson_R = 8,
		Johnson_I = 16,
		Sloan_u = 32,
		Sloan_g = 64,
		Sloan_r = 128,
		Sloan_i = 256,
		Sloan_z = 512
	}

	internal class SyntheticMagnitudeProducer
	{
		internal void ExportMagnitudes(ExportedMags mags, List<AbsFluxSpectra> spectra, List<AbsFluxSpectra> standardSpectras, StringBuilder output)
		{
			if ((mags & ExportedMags.Johnson_U) == ExportedMags.Johnson_U)
				ExportMagnitudes(FilterResponseDatabase.Instance.Johnson_U, spectra, standardSpectras, output);

			if ((mags & ExportedMags.Johnson_B) == ExportedMags.Johnson_B)
				ExportMagnitudes(FilterResponseDatabase.Instance.Johnson_B, spectra, standardSpectras, output);

			if ((mags & ExportedMags.Johnson_V) == ExportedMags.Johnson_V)
				ExportMagnitudes(FilterResponseDatabase.Instance.Johnson_V, spectra, standardSpectras, output);

			if ((mags & ExportedMags.Johnson_R) == ExportedMags.Johnson_R)
				ExportMagnitudes(FilterResponseDatabase.Instance.Johnson_R, spectra, standardSpectras, output);

			if ((mags & ExportedMags.Johnson_I) == ExportedMags.Johnson_I)
				ExportMagnitudes(FilterResponseDatabase.Instance.Johnson_I, spectra, standardSpectras, output);

			if ((mags & ExportedMags.Sloan_u) == ExportedMags.Sloan_u)
				ExportMagnitudes(FilterResponseDatabase.Instance.SLOAN_u, spectra, standardSpectras, output);

			if ((mags & ExportedMags.Sloan_g) == ExportedMags.Sloan_g)
				ExportMagnitudes(FilterResponseDatabase.Instance.SLOAN_g, spectra, standardSpectras, output);

			if ((mags & ExportedMags.Sloan_r) == ExportedMags.Sloan_r)
				ExportMagnitudes(FilterResponseDatabase.Instance.SLOAN_r, spectra, standardSpectras, output);

			if ((mags & ExportedMags.Sloan_i) == ExportedMags.Sloan_i)
				ExportMagnitudes(FilterResponseDatabase.Instance.SLOAN_i, spectra, standardSpectras, output);

			if ((mags & ExportedMags.Sloan_z) == ExportedMags.Sloan_z)
				ExportMagnitudes(FilterResponseDatabase.Instance.SLOAN_z, spectra, standardSpectras, output);
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

				m_SynthetizedReferneceFluxMagnitudes.Add(referenceSum, star.MagV /*TODO: Get other magnitides from UCAC4*/);
			}

			// NOTE: Is this a good way to estimate the error?
			m_AverageAbsFluxFitMagError = standardAbsFluxFitMagErrors.Average();
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

			double averageMag = allMags.Average();
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
