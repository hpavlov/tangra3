using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Tangra.Helpers;
using Tangra.Model.Config;
using Tangra.Model.Numerical;

namespace Tangra.VideoOperations.Spectroscopy.AbsFluxCalibration
{
	public class AbsFluxCalibrator
	{
		public int FromWavelength { get; private set; }
		public int ToWavelength { get; private set; }
		public int WavelengthBinSize { get; private set; }

		public bool IsCalibrated { get; private set; }

		private List<AbsFluxSpectra> m_SpectraList = new List<AbsFluxSpectra>();

		internal AbsFluxCalibrator()
		{
			FromWavelength = TangraConfig.Settings.Spectroscopy.MinWavelength;
			ToWavelength = TangraConfig.Settings.Spectroscopy.MaxWavelength;
			WavelengthBinSize = TangraConfig.Settings.Spectroscopy.AbsFluxResolution;
			IsCalibrated = false;
		}

		internal void AddSpectra(AbsFluxSpectra spectra)
		{
			if (!m_SpectraList.Any(x => x.FullFilePath.Equals(spectra.FullFilePath, StringComparison.InvariantCultureIgnoreCase)))
			{
				spectra.RescaleToResolution(FromWavelength, ToWavelength, WavelengthBinSize);
				m_SpectraList.Add(spectra);

				if (m_SpectraList.Count(x => x.IsComplete && x.IsStandard) > 2)
				{
					Calibrate();
				}
				else
				{
					IsCalibrated = false;
				}
			}
		}

		internal void RemoveSpectra(AbsFluxSpectra spectra)
		{
			m_SpectraList.RemoveAll(x => x.FullFilePath.Equals(spectra.FullFilePath, StringComparison.InvariantCultureIgnoreCase));

			if (m_SpectraList.Count(x => x.IsComplete && x.IsStandard) > 2)
			{
				Calibrate();
			}
			else
			{
				// Insificient observations to calibrate. Remove/Reset Calibration. 
				IsCalibrated = false;
			}
		}

		private List<double> m_ExtinctionCoefficients = new List<double>();
		private List<double> m_SensitivityCoefficients = new List<double>();
		private List<double> m_Wavelengths = new List<double>();

		private void Calibrate()
		{
			List<AbsFluxSpectra> standards = m_SpectraList.Where(x => x.IsComplete && x.IsStandard).ToList();
			int numEquations = standards.Count;

			m_ExtinctionCoefficients.Clear();
			m_SensitivityCoefficients.Clear();
			m_Wavelengths.Clear();

			if (numEquations > 2)
			{
				for (int i = 0; i < standards[0].DeltaMagnitiudes.Count; i++)
				{
					var A = new SafeMatrix(numEquations, 2);
					var X = new SafeMatrix(numEquations, 1);

					bool containsNaNs = false;

					for (int j = 0; j < numEquations; j++)
					{
						A[j, 0] = standards[j].InputFile.AirMass;
						A[j, 1] = 1;

						double deltaMag = standards[j].DeltaMagnitiudes[i];
						X[j, 0] = deltaMag;
						if (double.IsNaN(deltaMag)) containsNaNs = true;
					}

					m_Wavelengths.Add(standards[0].ResolvedWavelengths[i]);
					if (!containsNaNs)
					{
						SafeMatrix a_T = A.Transpose();
						SafeMatrix aa = a_T*A;
						SafeMatrix aa_inv = aa.Inverse();
						SafeMatrix bx = (aa_inv*a_T)*X;

						float ke = (float) bx[0, 0];
						float ks = (float) bx[1, 0];

						m_ExtinctionCoefficients.Add(ke);
						m_SensitivityCoefficients.Add(ks);
					}
					else
					{
						m_ExtinctionCoefficients.Add(double.NaN);
						m_SensitivityCoefficients.Add(double.NaN);						
					}
				}

				Trace.WriteLine("------------------------------------------");
				for (int j = 0; j < numEquations; j++)
				{
					standards[j].Residuals.Clear();
					standards[j].ResidualPercentage.Clear();
					for (int i = 0; i < standards[0].DeltaMagnitiudes.Count; i++)
					{
						// DeltaM = KE * AirMass + KS
						double residualOC = standards[j].DeltaMagnitiudes[i] - m_ExtinctionCoefficients[i] * standards[j].InputFile.AirMass - m_SensitivityCoefficients[i];

						standards[j].Residuals.Add(residualOC);
						standards[j].ResidualPercentage.Add(100 * residualOC / standards[j].DeltaMagnitiudes[i]);
					}

					Trace.WriteLine(string.Format("{0}[{1} sec]: {2}%", 
						standards[j].ToString(),
						standards[j].InputFile.Exposure.ToString("0.00"), 
						standards[j].ResidualPercentage.Where(x => !double.IsNaN(x)).ToList().Median().ToString("0.0")));
				}

				IsCalibrated = true;
			}			
		}
	}
}
