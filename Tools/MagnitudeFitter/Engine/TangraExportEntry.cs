using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tangra.Model.Helpers;

namespace MagnitudeFitter.Engine
{
//Star No	 RA Deg (J2000)	 DE Deg (J2000)	 Catalog Mag (R)	 B-V	 Sloan r'	 Average Intencity	 Error	 Median Intencity	 Error	 Average PSF Amplitude	 Error	 Median PSF Amplitude	 Error	 All Frames	 Saturated Frames	 Excluded Frames				
//3520156298	283.2394	-19.66837	 NaN	0.458	 	338219.1	13963.37	336411	14081.12	12025.48	1812.139	11955	1813.523	100	0	0		18.18282453	 	0.458

	public class TangraExportEntry
	{
		public string StarName;

		public float RADeg_2000;
		public float DEDeg_2000;

		public float CatalogMag = float.NaN;
		public float APASS_BV_Colour = float.NaN;
		public float APASS_Sloan_r = float.NaN;

		public float AverageIntensity = float.NaN;
		public float AverageIntensityError = float.NaN;
		public float MedianIntensity = float.NaN;
		public float MedianIntensityError = float.NaN;
		public float AveragePSFAmplitude = float.NaN;
		public float AveragePSFAmplitudeError = float.NaN;
		public float MedianPSFAmplitude = float.NaN;
		public float MedianPSFAmplitudeError = float.NaN;

		public int MeasuredFrames;
		public int SaturatedFrames;
		public int ExcludedFromFrames;

		public double InstrMag;
		public double InstrMagErr;
		public double Residual;

		public bool IsValid { get; private set; }

		internal TangraExportEntry(string[] tokens)
		{
			IsValid = false;

			if (tokens.Length == 17)
				ParseAPASSEntry(tokens);
		}

		private void ParseAPASSEntry(string[] tokens)
		{
			StarName = tokens[0];
			if (!float.TryParse(tokens[1], NumberStyles.Number, CultureInfo.InvariantCulture, out RADeg_2000))
				return;
			if (!float.TryParse(tokens[2], NumberStyles.Number, CultureInfo.InvariantCulture, out DEDeg_2000))
				return;

			if (tokens[3] == "NaN" || string.IsNullOrWhiteSpace(tokens[3]))
				CatalogMag = float.NaN;
			else float.TryParse(tokens[3], NumberStyles.Number, CultureInfo.InvariantCulture, out CatalogMag);

			if (tokens[4] == "NaN" || string.IsNullOrWhiteSpace(tokens[4]))
				APASS_BV_Colour = float.NaN;
			else float.TryParse(tokens[4], NumberStyles.Number, CultureInfo.InvariantCulture, out APASS_BV_Colour);

			if (tokens[5] == "NaN" || string.IsNullOrWhiteSpace(tokens[5]))
				APASS_BV_Colour = float.NaN;
			else float.TryParse(tokens[5], NumberStyles.Number, CultureInfo.InvariantCulture, out APASS_Sloan_r);

			float.TryParse(tokens[6], NumberStyles.Number, CultureInfo.InvariantCulture, out AverageIntensity);
			float.TryParse(tokens[7], NumberStyles.Number, CultureInfo.InvariantCulture, out AverageIntensityError);
			float.TryParse(tokens[8], NumberStyles.Number, CultureInfo.InvariantCulture, out MedianIntensity);
			float.TryParse(tokens[9], NumberStyles.Number, CultureInfo.InvariantCulture, out MedianIntensityError);
			float.TryParse(tokens[10], NumberStyles.Number, CultureInfo.InvariantCulture, out AveragePSFAmplitude);
			float.TryParse(tokens[11], NumberStyles.Number, CultureInfo.InvariantCulture, out AveragePSFAmplitudeError);
			float.TryParse(tokens[12], NumberStyles.Number, CultureInfo.InvariantCulture, out MedianPSFAmplitude);
			float.TryParse(tokens[13], NumberStyles.Number, CultureInfo.InvariantCulture, out MedianPSFAmplitudeError);

			if (!int.TryParse(tokens[14], NumberStyles.Number, CultureInfo.InvariantCulture, out MeasuredFrames))
				return;

			if (!int.TryParse(tokens[15], NumberStyles.Number, CultureInfo.InvariantCulture, out SaturatedFrames))
				return;

			if (!int.TryParse(tokens[16], NumberStyles.Number, CultureInfo.InvariantCulture, out ExcludedFromFrames))
				return;

			IsValid = true;
		}
	}
}
