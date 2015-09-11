
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms.VisualStyles;
using Tangra.Model.Helpers;

namespace Tangra.VideoOperations.Spectroscopy.AbsFluxCalibration
{
	internal class AbsFluxInputFile
	{
		public float Longitude { get; private set; }
		public float Latitude { get; private set; }
		public float RAHours { get; private set; }
		public float DEDeg { get; private set; }
		private double m_JD;
		private DateTime m_EpochUT;
		public float AirMass { get; private set; }
		public float FHWM { get; private set; }
		private float m_Gain;
		public float Exposure { get; private set; }
		private float m_Dispersion;
		public string Target { get; private set; }

		public string FullPath { get; private set; }
		public string FileName { get; private set; }
		private string m_FileExt;
		private SpectraFile m_SpectraFile;

		private List<double> m_Wavelengths = new List<double>();
		private List<double> m_Fluxes = new List<double>();

		public bool ContainsWavelengthData { get; private set; }

		public AbsFluxInputFile(string filePath)
		{
			FullPath = filePath;
			FileName = Path.GetFileName(filePath);
			m_FileExt = Path.GetExtension(filePath);
			ContainsWavelengthData = false;
			if (IsFileTypeSupported(m_FileExt))
				LoadSupportedFile(filePath);
		}

		public DateTime Epoch
		{
			get { return m_EpochUT; }
		}

		public List<double> Wavelengths
		{
			get { return m_Wavelengths; }
		}

		public List<double> Fluxes
		{
			get { return m_Fluxes; }
		}

		public override string ToString()
		{
			return FileName;
		}

		private void LoadSupportedFile(string filePath)
		{
			//if (".spectra".Equals(m_FileExt, StringComparison.InvariantCultureIgnoreCase))
			//	LoadSpectraFile(filePath);

			if (".dat".Equals(m_FileExt, StringComparison.InvariantCultureIgnoreCase))
				LoadExportedSpectraFile(filePath);
		}

		private static Regex s_HeaderRegex = new Regex(@"#\s*(?<HdrName>[^=]+)=(?<HdrValue>[^#]*)");

		private void LoadExportedSpectraFile(string filePath)
		{
			AirMass = float.NaN;
			FHWM = float.NaN;
			Exposure = float.NaN;
			RAHours = float.NaN;
			DEDeg = float.NaN;
			Longitude = float.NaN;
			Latitude = float.NaN;
			Target = null;
			m_EpochUT = DateTime.MinValue;

			var lines = new List<string>(File.ReadAllLines(filePath));

			char[] WV_FLX_SEP = " \t".ToCharArray(); 
			for (int i = 0; i < lines.Count; i++)
			{
				string line = lines[i];
				if (line.StartsWith("#"))
				{
					Match match = s_HeaderRegex.Match(line);
					if (match.Success && match.Groups["HdrName"] != null && match.Groups["HdrValue"] != null)
					{
						string headerName = match.Groups["HdrName"].Value.ToUpper();
						string headerValue = match.Groups["HdrValue"].Value;

						SetHeader(headerName, headerValue);
					}
				}
				else
				{
					string[] waveFluxPair = line.Split(WV_FLX_SEP, StringSplitOptions.RemoveEmptyEntries);
					if (waveFluxPair.Length == 2)
					{
						double wavelength;
						double flux;
						if (double.TryParse(waveFluxPair[0], NumberStyles.Float, CultureInfo.InvariantCulture, out wavelength) &&
							double.TryParse(waveFluxPair[1], NumberStyles.Float, CultureInfo.InvariantCulture, out flux))
						{
							m_Wavelengths.Add(wavelength);
							m_Fluxes.Add(flux);			
						}
					}
				}
			}

			if (float.IsNaN(Exposure) || float.IsNaN(RAHours) || float.IsNaN(DEDeg))
			{

				return;
			}

			ContainsWavelengthData = m_Wavelengths.Count > 0;
		}
		
		private void SetHeader(string name, string value)
		{
			//# Longitude=150.7769
			//# Latitude=-33.8092
			//# RA=2.3958 # hours
			//# DEC=-50.9320 # degrees
			//# JD=2457206.23238 # UT
			//# X=1.459 # air mass
			//# Gain=34.0 # dB
			//# Exposure=1.00 # sec
			//# Target=HD 14943
			//# Camera=WAT-910BD
			//# Telescope=14" LX200 ACF
			//# Recorder=OccuRec v2.8.2
			//# Observer=Hristo Pavlov
			//# WavelengthCalibration=3-rd order[-3.069448E-13,3.490942E-07,0.09728059,37.99137]
			//# Dispersion=9.93 # A/pix

			float floatVal;
			if (name == "LONGITUDE")
			{
				if (float.TryParse(value, out floatVal)) Longitude = floatVal;
			}
			else if (name == "LATITUDE")
			{
				if (float.TryParse(value, out floatVal)) Latitude = floatVal;
			}
			else if (name == "RA")
			{
				if (float.TryParse(value, out floatVal)) RAHours = floatVal;
			}
			else if (name == "DEC")
			{
				if (float.TryParse(value, out floatVal)) DEDeg = floatVal;
			}
			else if (name == "JD")
			{
				if (double.TryParse(value, out m_JD))
					m_EpochUT = JulianDayHelper.DateTimeAtJD(m_JD);
			}
			else if (name == "X")
			{
				if (float.TryParse(value, out floatVal)) AirMass = floatVal;
			}
			else if (name == "FWHM")
			{
				if (float.TryParse(value, out floatVal)) FHWM = floatVal;
			}
			else if (name == "GAIN") float.TryParse(value, out m_Gain);
			else if (name == "EXPOSURE")
			{
				if (float.TryParse(value, out floatVal)) Exposure = floatVal;
			}
			else if (name == "TARGET") Target = value;
			else if (name == "DISPERSION") float.TryParse(value, out m_Dispersion);
		}

		private void LoadSpectraFile(string filePath)
		{
			try
			{
				m_SpectraFile = SpectraFile.Load(filePath);
				if (m_SpectraFile.Data.IsCalibrated())
				{
					ContainsWavelengthData = true;

					// TODO: Do an automtic spectra export
					//m_SpectraFile.Data.Export
				}
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex);					
			}			
		}

		public static bool IsFileTypeSupported(string fileExt)
		{
			if (".spectra".Equals(fileExt, StringComparison.InvariantCultureIgnoreCase))
				return false;

			if (".dat".Equals(fileExt, StringComparison.InvariantCultureIgnoreCase))
				return true;

			return false;
		}
	}
}
