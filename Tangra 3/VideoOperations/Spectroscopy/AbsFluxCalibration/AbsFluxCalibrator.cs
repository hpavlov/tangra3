using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.VideoOperations.Spectroscopy.AbsFluxCalibration
{
	public class AbsFluxCalibrator
	{
		private List<AbsFluxSpectra> m_Spectra = new List<AbsFluxSpectra>();
 
		internal void AddSpectra(AbsFluxSpectra spectra)
		{
			if (!m_Spectra.Any(x => x.FullFilePath.Equals(spectra.FullFilePath, StringComparison.InvariantCultureIgnoreCase)))
				m_Spectra.Add(spectra);
		}


		internal void RemoveSpectra(AbsFluxSpectra spectra)
		{
			m_Spectra.RemoveAll(x => x.FullFilePath.Equals(spectra.FullFilePath, StringComparison.InvariantCultureIgnoreCase));
		}
	}
}
