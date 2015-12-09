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
		internal void ExportMagnitudes(ExportedMags mags, List<AbsFluxSpectra> spectra, StringBuilder output)
		{
			if ((mags & ExportedMags.Johnson_U) == ExportedMags.Johnson_U)
				ExportMagnitudes(FilterResponseDatabase.Instance.Johnson_U, spectra, output);

			if ((mags & ExportedMags.Johnson_B) == ExportedMags.Johnson_B)
				ExportMagnitudes(FilterResponseDatabase.Instance.Johnson_B, spectra, output);

			if ((mags & ExportedMags.Johnson_V) == ExportedMags.Johnson_V)
				ExportMagnitudes(FilterResponseDatabase.Instance.Johnson_V, spectra, output);

			if ((mags & ExportedMags.Johnson_V) == ExportedMags.Johnson_V)
				ExportMagnitudes(FilterResponseDatabase.Instance.Johnson_V, spectra, output);

			if ((mags & ExportedMags.Johnson_R) == ExportedMags.Johnson_R)
				ExportMagnitudes(FilterResponseDatabase.Instance.Johnson_R, spectra, output);

			if ((mags & ExportedMags.Sloan_u) == ExportedMags.Sloan_u)
				ExportMagnitudes(FilterResponseDatabase.Instance.SLOAN_u, spectra, output);

			if ((mags & ExportedMags.Sloan_g) == ExportedMags.Sloan_g)
				ExportMagnitudes(FilterResponseDatabase.Instance.SLOAN_g, spectra, output);

			if ((mags & ExportedMags.Sloan_r) == ExportedMags.Sloan_r)
				ExportMagnitudes(FilterResponseDatabase.Instance.SLOAN_r, spectra, output);

			if ((mags & ExportedMags.Sloan_i) == ExportedMags.Sloan_i)
				ExportMagnitudes(FilterResponseDatabase.Instance.SLOAN_i, spectra, output);

			if ((mags & ExportedMags.Sloan_z) == ExportedMags.Sloan_z)
				ExportMagnitudes(FilterResponseDatabase.Instance.SLOAN_z, spectra, output);
		}

		private void ExportMagnitudes(FilterResponse filterResponse, List<AbsFluxSpectra> spectra, StringBuilder output)
		{
			EnsureReferenceSums(filterResponse);

			output.Append(filterResponse.Designation);
			for (int j = 0; j < spectra.Count; j++)
			{
				output.Append(",");
				double syntheticMag = ComputeSyntheticMag(filterResponse, spectra[j]);
				output.Append(syntheticMag.ToString("0.00"));
			}
			output.AppendLine();
		}

		private Dictionary<double, double> m_SynthetizedReferneceFluxMagnitudes = new Dictionary<double, double>();

		private void EnsureReferenceSums(FilterResponse filterResponse)
		{
			m_SynthetizedReferneceFluxMagnitudes.Clear();

			foreach (CalSpecStar star in CalSpecDatabase.Instance.Stars)
			{
				List<double> wavelengths = star.DataPoints.Keys.ToList();
				List<double> fluxes = wavelengths.Select(x => star.DataPoints[x]).ToList();

				double referenceSum = 0;

				foreach (int wavelength in filterResponse.Response.Keys)
				{
					double targetVal = InterpolateValue(wavelengths, fluxes, wavelength);

					referenceSum += targetVal;
				}

				m_SynthetizedReferneceFluxMagnitudes.Add(referenceSum, star.MagV /*TODO: Get other magnitides from UCAC4*/);
			}
		}

		private double ComputeSyntheticMag(FilterResponse filterResponse, AbsFluxSpectra spectra)
		{
			double targetSum = 0;
			
			foreach (int wavelength in filterResponse.Response.Keys)
			{
				double targetVal = InterpolateValue(spectra.ResolvedWavelengths, spectra.AbsoluteFluxes, wavelength);

				targetSum += targetVal;
			}

			//TODO:

			return 0;
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
