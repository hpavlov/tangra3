
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Tangra.VideoOperations.Spectroscopy.AbsFluxCalibration
{
	internal class SpectraFileUIWrapper
	{
		private string m_FileName;
		private string m_FileExt;
		private SpectraFile m_SpectraFile;

		private bool m_ContainsWavelengthData;

		public SpectraFileUIWrapper(string filePath)
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
			if (".spectra".Equals(m_FileExt, StringComparison.InvariantCultureIgnoreCase))
				LoadSpectraFile(filePath);
		}

		private void LoadSpectraFile(string filePath)
		{
			try
			{
				m_SpectraFile = SpectraFile.Load(filePath);
				if (m_SpectraFile.Data.IsCalibrated())
				{
					m_ContainsWavelengthData = true;
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
				return true;

			return false;
		}
	}
}
