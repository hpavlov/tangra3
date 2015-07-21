﻿
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
	internal class ExportedSpectraUIWrapper
	{
		private float m_Longitude;
		private float m_Latitude;
		private float m_RAHours;
		private float m_DEDeg;
		private float m_JD;
		private DateTime m_EpochUT;
		private float m_AirMass;
		private float m_Gain;
		private float m_Exposure;
		private float m_Dispersion;
		private string m_Target;


		private string m_FileName;
		private string m_FileExt;
		private SpectraFile m_SpectraFile;

		private bool m_ContainsWavelengthData;

		public ExportedSpectraUIWrapper(string filePath)
		{
			m_FileName = Path.GetFileName(filePath);
			m_FileExt = Path.GetExtension(filePath);
			m_ContainsWavelengthData = false;
			if (IsFileTypeSupported(m_FileExt))
				LoadSupportedFile(filePath);
		}

		public bool ContainsWavelengthData
		{
			get { return m_ContainsWavelengthData; }
		}

		public override string ToString()
		{
			return m_FileName;
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
			m_AirMass = float.NaN;
			m_Exposure = float.NaN;
			m_RAHours = float.NaN;
			m_DEDeg = float.NaN;

			var lines = new List<string>(File.ReadAllLines(filePath));

			string[] headerLines = lines.Where(x => x.StartsWith("#")).ToArray();
			foreach (string headerLine in headerLines)
			{
				Match match = s_HeaderRegex.Match(headerLine);
				if (match.Success && match.Groups["HdrName"] != null && match.Groups["HdrValue"] != null)
				{
					string headerName = match.Groups["HdrName"].Value.ToUpper();
					string headerValue = match.Groups["HdrValue"].Value;

					SetHeader(headerName, headerValue);
				}
			}

			if (float.IsNaN(m_Exposure) || float.IsNaN(m_RAHours) || float.IsNaN(m_DEDeg))
			{

				return;
			}

			m_ContainsWavelengthData = true;

			//0.03497931	10225.38
			//10.62753	10177.19
			//21.21906	9708.325

			// TODO: Read exported values
		}
		
		private void SetHeader(string name, string value)
		{
			//# Longitude=150.7769
			//# Latitude=-33.8092
			//# RA=2.3958 # hours
			//# DEC=-50.9320 # degrees
			//# JD=2457206.23238 # UT
			//# Z=1.459 # air mass
			//# Gain=34.0 # dB
			//# Exposure=1.00 # sec
			//# Target=HD 14943
			//# Camera=WAT-910BD
			//# Telescope=14" LX200 ACF
			//# Recorder=OccuRec v2.8.2
			//# Observer=Hristo Pavlov
			//# WavelengthCalibration=3-rd order[-3.069448E-13,3.490942E-07,0.09728059,37.99137]
			//# Dispersion=9.93 # A/pix

			if (name == "LONGITUDE") float.TryParse(value, out m_Longitude);
			else if (name == "LATITUDE") float.TryParse(value, out m_Latitude);
			else if (name == "RA") float.TryParse(value, out m_RAHours);
			else if (name == "DEC") float.TryParse(value, out m_DEDeg);
			else if (name == "JD")
			{
				if (float.TryParse(value, out m_JD))
					m_EpochUT = JulianDayHelper.DateTimeAtJD(m_JD);
			}
			else if (name == "Z") float.TryParse(value, out m_AirMass);
			else if (name == "GAIN") float.TryParse(value, out m_Gain);
			else if (name == "EXPOSURE") float.TryParse(value, out m_Exposure);
			else if (name == "TARGET") m_Target = value;
			else if (name == "DISPERSION") float.TryParse(value, out m_Dispersion);
		}

		private void LoadSpectraFile(string filePath)
		{
			try
			{
				m_SpectraFile = SpectraFile.Load(filePath);
				if (m_SpectraFile.Data.IsCalibrated())
				{
					m_ContainsWavelengthData = true;

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
