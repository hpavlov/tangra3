using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Tangra.Model.Helpers;

namespace Tangra.VideoOperations.LightCurves.Report
{
	public class EventTimesReport
	{
		public string LcFilePath { get; set; }
		public string VideoFilePath { get; set; }
		public string SourceInfo { get; set; }
		public string Provider { get; set; }
		public string TimingType { get; set; }
		public bool HasEmbeddedTimeStamps { get; set; }
		public string ReductionMethod { get; set; }
		public string NoiseMethod { get; set; }
		public int BitPix { get; set; }

		/// <summary>
		/// Flea3-03S1 with ADVS
		/// Flea3-03S3 with ADVS
		/// Flea3-28S4M with ADVS
		/// Grasshopper Express with ADVS
		/// G-Star
		/// WAT120N
		/// WAT120N+
		/// WAT910HX
		/// WAT910BD
		/// Mintron 12V1C-EX
		/// Samsung SCB-2000
		/// PC165-DNR
		/// LN-300-11673
		/// KPC-350BH
		/// PC164C
		/// WAT902H
		/// </summary>
		public string CameraName { get; set; }

		public int AveragedFrameHeight { get; set; }
		public int AveragedFrameWidth { get; set; }
		public bool IsThisAMiss { get; set; }
		public DateTime? RecordedFromUT { get; set; }
		public DateTime? RecordedToUT { get; set; }
		public DateTime? AnalysedFromUT { get; set; }
		public DateTime? AnalysedToUT { get; set; }
		/// <summary>
		/// ADV|AAV|AVI
		/// </summary>
		public string VideoFileFormat { get; set; }

		/// <summary>
		/// NTSC/EIA
		/// PAL/CCIR
		/// ADVS
		/// AAV-NTSC
		/// AAV-PAL
		/// </summary>
		public string VideoFormat { get; set; }

		public double ExposureDuration { get; set; }
		
		/// <summary>
		/// Fields
		/// Frames
		/// Seconds
		/// </summary>
		public string ExposureUnit { get; set; }
		
		public string InstrumentalDelaysApplied { get; set; }
		public DateTime ReportTime { get; set; }



		[XmlArrayItem("Event")]
		public List<OccultationEventInfo> Events = new List<OccultationEventInfo>();

		public void SaveReport()
		{
			ReportTime = DateTime.Now;
			bool reportSaved = false;
			
			string directory = Path.GetDirectoryName(LcFilePath);
			string reportFileName = Path.ChangeExtension(LcFilePath, ".trep.xml");
			string fileNameOnly = Path.GetFileName(reportFileName);

			if (directory != null && Directory.Exists(directory))
			{
				reportSaved = TrySaveReport(reportFileName);
			}

			if (!reportSaved)
			{
				directory = Path.GetFullPath(string.Format("{0}\\Tangra3\\Reports", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)));
				if (!Directory.Exists(directory))
				{
					try
					{
						Directory.CreateDirectory(directory);
					}
					catch (Exception ex)
					{
						Trace.WriteLine(ex.FullExceptionInfo());
					}
				}

				if (Directory.Exists(directory))
				{
					reportFileName = Path.GetFullPath(string.Format("{0}\\{1}", directory, fileNameOnly));
					reportSaved = TrySaveReport(reportFileName);
				}
			}
			

			if (reportSaved)
			{
				// TODO: Update the registry settings for Windows only and only if OW is present on the system and if the ReportingAddin is installed
				// TODO: Modify OW Reporting Addin to save something in the Registry (and maintain its Registry Key) where Tangra will save data
			}
		}

		private bool TrySaveReport(string fullFileName)
		{
			try
			{
				var xmlSer = new XmlSerializer(typeof (EventTimesReport));
				using (var fs = new FileStream(fullFileName, FileMode.Create, FileAccess.ReadWrite))
				{
					xmlSer.Serialize(fs, this);
				}

				return File.Exists(fullFileName);
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.FullExceptionInfo());
			}

			return false;
		}

	}

	public class OccultationEventInfo
	{
		public int EventId { get; set; }
		public float DFrame { get; set; }
		public float RFrame { get; set; }
		public float DFrameErrorMinus { get; set; }
		public float DFrameErrorPlus { get; set; }
		public float RFrameErrorMinus { get; set; }
		public float RFrameErrorPlus { get; set; }
		public string DTimeString { get; set; }
		public string RTimeString { get; set; }

		public DateTime DTime { get; set; }
		public DateTime RTime { get; set; }
		public int DTimeErrorMS { get; set; }
		public int RTimeErrorMS { get; set; }
		public DateTime DTimeMostProbable { get; set; }
		public DateTime RTimeMostProbable { get; set; }
	}
}
