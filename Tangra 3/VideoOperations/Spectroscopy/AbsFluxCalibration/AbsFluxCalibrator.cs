using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.Model.Config;

namespace Tangra.VideoOperations.Spectroscopy.AbsFluxCalibration
{
	public class AbsFluxCalibrator
	{
		private int m_FromWavelength;
		private int m_ToWavelength;
		private int m_WavelengthBinSize;

		private List<AbsFluxSpectra> m_SpectraList = new List<AbsFluxSpectra>();

		internal AbsFluxCalibrator()
		{
			m_FromWavelength = TangraConfig.Settings.Spectroscopy.MinWavelength;
			m_ToWavelength = TangraConfig.Settings.Spectroscopy.MaxWavelength;
			m_WavelengthBinSize = TangraConfig.Settings.Spectroscopy.AbsFluxResolution;
		}

		internal void AddSpectra(AbsFluxSpectra spectra)
		{
			if (!m_SpectraList.Any(x => x.FullFilePath.Equals(spectra.FullFilePath, StringComparison.InvariantCultureIgnoreCase)))
			{
				spectra.RescaleToResolution(m_FromWavelength, m_ToWavelength, m_WavelengthBinSize);
				m_SpectraList.Add(spectra);
			}
		}


		internal void RemoveSpectra(AbsFluxSpectra spectra)
		{
			m_SpectraList.RemoveAll(x => x.FullFilePath.Equals(spectra.FullFilePath, StringComparison.InvariantCultureIgnoreCase));
		}
	}
}
