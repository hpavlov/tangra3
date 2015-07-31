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
					standards[j].ResidualPercentageFlux.Clear();
					standards[j].ResidualPercentageObsFlux.Clear();

					for (int i = 0; i < standards[0].DeltaMagnitiudes.Count; i++)
					{
						// DeltaM = KE * AirMass + KS = -2.5 * Math.Log10((ObservedFluxes[i] / exposure) / AbsoluteFluxes[i])
						double calculatedDeltaMag = m_ExtinctionCoefficients[i] * standards[j].InputFile.AirMass + m_SensitivityCoefficients[i];
						double calculatedFluxRatio = Math.Pow(10, calculatedDeltaMag / -2.5);
						double calculatedAbsoluteFlux = (standards[j].ObservedFluxes[i] / standards[j].InputFile.Exposure) / calculatedFluxRatio;
						double calculatedObservedFlux = standards[j].AbsoluteFluxes[i] * calculatedFluxRatio * standards[j].InputFile.Exposure;

						double residualAbsoluteFluxOC = standards[j].AbsoluteFluxes[i] - calculatedAbsoluteFlux;

						standards[j].Residuals.Add(residualAbsoluteFluxOC);
						standards[j].ResidualPercentage.Add(100 * (calculatedDeltaMag - standards[j].DeltaMagnitiudes[i]) / standards[j].DeltaMagnitiudes[i]);
						standards[j].ResidualPercentageFlux.Add(100 * residualAbsoluteFluxOC / standards[j].AbsoluteFluxes[i]);
						standards[j].ResidualPercentageObsFlux.Add(100 * (standards[j].ObservedFluxes[i] - calculatedObservedFlux) / standards[j].ObservedFluxes[i]);
					}

					Trace.WriteLine(string.Format("{0}[{1} sec, {2}]: {3}%, AbsFlux: {4}%, ObsFlux: {5}%", 
						standards[j].ToString(),
						standards[j].InputFile.Exposure.ToString("0.00"),
						standards[j].InputFile.AirMass.ToString("0.000"), 
						standards[j].ResidualPercentage.Where(x => !double.IsNaN(x)).ToList().Median().ToString("0.0"),
						standards[j].ResidualPercentageFlux.Where(x => !double.IsNaN(x)).ToList().Median().ToString("0.0"),
						standards[j].ResidualPercentageObsFlux.Where(x => !double.IsNaN(x)).ToList().Median().ToString("0.0")));
				}

				IsCalibrated = true;
			}			
		}
	}
}
