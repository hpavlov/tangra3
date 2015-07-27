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
		private string m_DisplayName;
		public AbsFluxInputFile InputFile { get; private set; }
		public bool IsComplete { get; private set; }

		internal AbsFluxSpectra(AbsFluxInputFile inputFile)
		{
			InputFile = inputFile;
			IsComplete = false;

			Match match = STAR_DESIGNATION_REGEX.Match(inputFile.FileName);
			if (match.Success && !string.IsNullOrEmpty(match.Value))
			{
				m_ExtractedStarName = match.Value.Replace('_', ' ').Replace(" ", " ").Trim();
				m_DisplayName = m_ExtractedStarName;

				if (inputFile.Epoch > DateTime.MinValue)
					m_DisplayName += inputFile.Epoch.ToString(" (HH:mm UT)");
			}

			if (inputFile.Epoch != DateTime.MinValue &&
			    !float.IsNaN(inputFile.Latitude) && !float.IsNaN(inputFile.Longitude) &&
				!float.IsNaN(inputFile.RAHours) && !float.IsNaN(inputFile.DEDeg))
			{
				IsComplete = true;
			}
		}

		public override string ToString()
		{
			return m_DisplayName ?? InputFile.FileName;
		}

		public string FullFilePath
		{
			get { return InputFile.FullPath; }			
		}

	}
}
