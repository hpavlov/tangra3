using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Tangra.VideoOperations.Spectroscopy.AbsFluxCalibration
{
	internal class AbsFluxSpectra
	{
		public int WavelengthFrom { get; set; }
		public int WavelengthBinSize { get; set; }
		public List<double> Fluxes = new List<double>();
		public List<double> Magnitiudes = new List<double>();

		private static Regex STAR_DESIGNATION_REGEX = new Regex("(HD|BD|HIP|TYC)[\\d\\s\\-\\+_]+");

		private string m_ExtractedStarName;
		private AbsFluxInputFile m_InputFile;

		internal AbsFluxSpectra(AbsFluxInputFile inputFile)
		{
			m_InputFile = inputFile;

			Match match = STAR_DESIGNATION_REGEX.Match(inputFile.FileName);
			if (match.Success && !string.IsNullOrEmpty(match.Value))
			{
				m_ExtractedStarName = match.Value.Replace('_', ' ').Replace(" ", " ").Trim();
			}
		}

		public override string ToString()
		{
			return m_ExtractedStarName ?? m_InputFile.FileName;
		}
	}
}
