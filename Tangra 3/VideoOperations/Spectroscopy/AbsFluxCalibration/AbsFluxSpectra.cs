using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Tangra.Model.Helpers;

namespace Tangra.VideoOperations.Spectroscopy.AbsFluxCalibration
{
	internal class AbsFluxSpectra
	{
		public int WavelengthFrom { get; set; }
		public int WavelengthTo { get; set; }
		public int WavelengthBinSize { get; set; }
		public int DataFromWavelength { get; set; }
		public int DataToWavelength { get; set; }
		public List<double> ObservedFluxes = new List<double>();
		public List<double> AbsoluteFluxes = new List<double>();
		public List<double> DeltaMagnitiudes = new List<double>();
		public List<double> ResolvedWavelengths = new List<double>();
		public List<double> Residuals = new List<double>();
		public List<double> ResidualObsFlux = new List<double>();
		public List<double> ResidualPercentage = new List<double>();
		public List<double> ResidualPercentageFlux = new List<double>();		
		public double AverageBiasPercentage { get; set; }

		private static Regex STAR_DESIGNATION_REGEX = new Regex("(HD|BD|HIP|TYC)[\\d\\s\\-\\+_]+");

		private string m_ExtractedStarName;
		private string m_DisplayName;
		public AbsFluxInputFile InputFile { get; private set; }
		public CalSpecStar m_CalSpecStar;
		public bool IsComplete { get; private set; }
		public int Number;
		public bool PlotSpectra;
        public bool HasObjectCoordinates { get; private set; }
        public bool HasObservationTime { get; private set; }

	    public bool IsStandard
		{
			get { return m_CalSpecStar != null; }
		}

        internal void SetAsProgramStar()
        {
            if (HasObjectCoordinates &&
                HasObservationTime)
            {
                IsComplete = true;
            }
        }

		internal AbsFluxSpectra(AbsFluxInputFile inputFile)
		{
			InputFile = inputFile;
			IsComplete = false;

			if (!string.IsNullOrEmpty(inputFile.Target))
			{
				// First try to read the target from the export
				m_DisplayName = inputFile.Target;
				m_ExtractedStarName = null;
			}
			else
			{
				// Then try to parse the star from the file
				Match match = STAR_DESIGNATION_REGEX.Match(inputFile.FileName);
				if (match.Success && !string.IsNullOrEmpty(match.Value))
				{
					m_ExtractedStarName = match.Value.Replace('_', ' ').Replace("  ", " ").Trim();
					m_DisplayName = m_ExtractedStarName;
				}				
			}

		    HasObjectCoordinates = !float.IsNaN(inputFile.Latitude) && !float.IsNaN(inputFile.Longitude) &&
		                           !float.IsNaN(inputFile.RAHours) && !float.IsNaN(inputFile.DEDeg);

		    HasObservationTime = inputFile.Epoch != DateTime.MinValue;

            if (HasObservationTime && HasObjectCoordinates)
			{
				// If we have the center of the field saved in the export then identify the standard star by the position
				List<CalSpecStar> standardsInOneDegreeRadius = CalSpecDatabase.Instance.Stars.Where(x => AngleUtility.Elongation(x.RA_J2000_Hours * 15, x.DE_J2000_Deg, InputFile.RAHours * 15, InputFile.DEDeg) < 1).ToList();
				if (standardsInOneDegreeRadius.Count == 1)
				{
					m_CalSpecStar = standardsInOneDegreeRadius[0];
					m_DisplayName = m_CalSpecStar.AbsFluxStarId;

					IsComplete = true;
					PlotSpectra = true;
				}
			}

			if (inputFile.Epoch > DateTime.MinValue)
				m_DisplayName += string.Format(" ({0} UT)", inputFile.Epoch.ToString("HH:mm"));

			if (string.IsNullOrEmpty(m_DisplayName))
				m_DisplayName = Path.GetFileNameWithoutExtension(inputFile.FileName);


			DataFromWavelength = (int)Math.Ceiling(inputFile.Wavelengths[0]);
			DataFromWavelength = (int)Math.Floor(inputFile.Wavelengths[inputFile.Wavelengths.Count - 1]);
		}

		public override string ToString()
		{
			return string.Format("{0}.{1}", Number, m_DisplayName ?? InputFile.FileName);
		}

		public string FullFilePath
		{
			get { return InputFile.FullPath; }			
		}

		internal void RescaleToResolution(int fromWavelength, int toWavelength, int step)
		{
			WavelengthFrom = fromWavelength - (int)Math.Ceiling(step / 2.0);
			WavelengthBinSize = step;
			WavelengthTo = toWavelength + (int) Math.Ceiling(step/2.0);

			RescaleData(WavelengthFrom, WavelengthTo, WavelengthBinSize, InputFile.Wavelengths, InputFile.Fluxes, ObservedFluxes, ResolvedWavelengths);
			if (m_CalSpecStar != null)
			{
				List<double> absWavelengths = m_CalSpecStar.DataPoints.Keys.ToList();
				List<double> absFluxes = m_CalSpecStar.DataPoints.Values.ToList();
				RescaleData(WavelengthFrom, WavelengthTo, WavelengthBinSize, absWavelengths, absFluxes, AbsoluteFluxes, null);

				DeltaMagnitiudes.Clear();
				double exposure = InputFile.Exposure;
				for (int i = 0; i < ObservedFluxes.Count; i++)
				{
					DeltaMagnitiudes.Add(-2.5 * Math.Log10((ObservedFluxes[i] / exposure) / AbsoluteFluxes[i]));
				}
			}
		}

		private static float FWHM_COEFF = (float)(4 * Math.Log(2));

		private static float GetGaussianValue(float fwhm, float distance)
		{
			// FWHM * FWHM = 4 * (2 ln(2)) * c * c => 2*c*c = FWHM*FWHM / (4 * ln(2))
			return (float)Math.Exp(-FWHM_COEFF * distance * distance / (fwhm * fwhm));
		}

		private static void RescaleData(int fromWavelength, int toWavelength, int step, List<double> wavelengths, List<double> fluxes, List<double> rescaledFluxes, List<double> resolvedWavelengths)
		{
			// Blurring: 
			// 1) Find the number of original datapoints in resolution/blur interval
			// 2) Use this number as a kernel for Gaussian blur of all participating data points
			// 3) Perform a blur using twice the resolution interval 
			// 4) Get the average of the participating blurred data points
			// 5) This is the value to be used for the bin

			int totalPoints = (toWavelength - fromWavelength) / step;

			int totalSourceDataPoints = wavelengths.Count;
			double firstDataWaveLength = wavelengths[0];
			double lastDataWaveLength = wavelengths[totalSourceDataPoints - 1];

			double totalSourceDataPointsRange = lastDataWaveLength - firstDataWaveLength;
			int binSize = (int)Math.Ceiling(step / (totalSourceDataPointsRange / totalSourceDataPoints));
			var kernel = new float[2 * binSize + 1];
			for (int i = 0; i < binSize; i++)
			{
				kernel[binSize + i] = kernel[binSize - i] = GetGaussianValue(binSize, i);
			}

			var partFluxes = new List<double>();

			rescaledFluxes.Clear();
			rescaledFluxes.AddRange(new double[totalPoints]);
			
			if (resolvedWavelengths != null)
			{
				resolvedWavelengths.Clear();
				resolvedWavelengths.AddRange(new double[totalPoints]);				
			}

			for (int i = 0; i < totalPoints; i++)
			{
				float midWvL = fromWavelength + (i + 0.5f) * step;
				int binFromWvL = fromWavelength + i * step;
				int binToWvL = fromWavelength + (i + 1) * step;

				if (resolvedWavelengths != null) resolvedWavelengths[i] = midWvL;

				if (binFromWvL < firstDataWaveLength || binFromWvL > lastDataWaveLength)
				{
					rescaledFluxes[i] = double.NaN;
					continue;
				}

				// double blurredValue = Sum(all_blurred_point_values) / num_points;

				int idxFrom = wavelengths.FindLastIndex(x => x < binFromWvL);
				int idxTo = wavelengths.FindIndex(x => x > binToWvL);

				partFluxes.Clear();

				for (int j = idxFrom + 1; j < idxTo; j++)
				{
					double sum = 0;
					double weight = 0;

					for (int k = j - binSize; k <= j + binSize; k++)
					{
						if (k > 0 && k < wavelengths.Count - 1)
						{
							weight += kernel[k - j + binSize];
							sum += kernel[k - j + binSize] * fluxes[k];
						}
					}

					double blurredFlux = 0;

					if (weight > 0)
						blurredFlux = sum / weight;
					else
						blurredFlux = fluxes[j];

					partFluxes.Add(blurredFlux);
				}

				rescaledFluxes[i] = partFluxes.Any() ? partFluxes.Average() : 0;
			}

		}
	}
}
