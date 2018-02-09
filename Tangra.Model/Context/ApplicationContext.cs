using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using Tangra.Model.Config;

namespace Tangra.Model.Context
{
    public class CrashReportInfo
    {
        public string VideoFile { get; set; }
        public string StreamType { get; set; }
		public string VideoOperation { get; set; }		
		public string Tracker { get; set; }
        public long FrameNumber { get; set; }

		[XmlIgnore]
		public string ReductionContext { get; set; }

        public long WorkingSet64 { get; set; }
        public long VirtualMemorySize64 { get; set; }
        public long PrivateMemorySize64 { get; set; }
        public long PeakWorkingSet64 { get; set; }
        public long MinWorkingSet  { get; set; }
        public long MaxWorkingSet { get; set; }
    }

	public class TangraContext
	{
		public static TangraContext Current = new TangraContext();

		private bool m_HasVideoLoaded;

		public bool HasVideoLoaded
		{
			get { return m_HasVideoLoaded; }
			set
			{
				m_HasVideoLoaded = value;
				CanScrollFrames = value;
				CanPlayVideo = value;
			}
		}
		public bool HasImageLoaded;
        public bool HasLightCurveLoaded;
		public bool CanChangeTool = true;
		public bool CanPlayVideo = true;
        public bool UndefinedFrameRate = false;

		public bool CanScrollFrames;

		public bool IsCalibrating = false;

		public bool HasConfigurationChosen = false;
		public bool HasConfigurationSolved = false;

		public bool UsingADV = false;
		public bool IsSerFile = false;
	    public bool IsFitsStream = false;
        public bool IsAviFile = false;
        public bool IsAAV2 = false;

		public bool RecordingDebugSession = false;

		public bool UsingIntegration = false;
		public int NumberFramesToIntegrate = 0;

		public bool OSDExcludeToolDisabled = false;

		public bool CanLoadDarkFrame = false;
		public bool CanLoadFlatFrame = false;
        public bool CanLoadBiasFrame = false;

		public bool DarkFrameLoaded = false;
		public bool FlatFrameLoaded = false;
        public bool BiasFrameLoaded = false;
		public bool CanProcessLightCurvePixels = false;

	    public bool OperationInProgress = false;

		public bool HasAnyFileLoaded
		{
			get { return HasVideoLoaded || HasImageLoaded; }
		}

	    public bool CanStartNewOperation
	    {
	        get { return !HasLightCurveLoaded && !OperationInProgress; }
	    }

		public int FrameWidth = 0;
		public int FrameHeight = 0;
		public int FirstFrame = -1;
		public int LastFrame = 0;

		public string FileName;
		public string FileFormat;

	    public int OcrErrors = 0;
        public bool OcrExtractingTimestamps = false;
	    public int AstrometryOCRFailedRead = 0;
        public int AstrometryOCRDroppedFrames = 0;
        public int AstrometryOCRDuplicatedFrames = 0;
        public int AstrometryOCRTimeErrors = 0;
	    public int AAVConvertErrors = 0;

        public CrashReportInfo CrashReportInfo;

	    public string RenderingEngine;
        public ReInterlaceMode ReInterlacingMode;

		private bool m_RestartRequest = false;
		public bool HasRestartRequest()
		{
			return m_RestartRequest;
		}

        public void RestartApplication()
        {
            if (m_RestartRequest)
                return;

            m_RestartRequest = true;

            Application.Restart();
            Process.GetCurrentProcess().Kill();
        }

		public void Reset()
		{
			HasVideoLoaded = false;
			HasImageLoaded = false;
		    HasLightCurveLoaded = false;
			CanChangeTool = false;
			CanChangeTool = false;

			CanScrollFrames = false;
			CanPlayVideo = false;
		    UndefinedFrameRate = false;

			UsingADV = false;
		    IsAAV2 = false;
			IsSerFile = false;
            IsFitsStream = false;
		    IsAviFile = false;
			RecordingDebugSession = false;
			UsingIntegration = false;

			HasConfigurationChosen = false;
			HasConfigurationSolved = false;
			IsCalibrating = false;

			OSDExcludeToolDisabled = false;

			FrameWidth = 0;
			FrameHeight = 0;
			FirstFrame = -1;
			LastFrame = 0;

			FileName = null;
			FileFormat = null;

			DarkFrameLoaded = false;
			FlatFrameLoaded = false;
			CanLoadDarkFrame = false;
			CanLoadFlatFrame = false;
		    CanLoadBiasFrame = false;

		    RenderingEngine = null;
            ReInterlacingMode = ReInterlaceMode.None;

	        OcrErrors = 0;
            OcrExtractingTimestamps = false;
		    AstrometryOCRFailedRead = 0;
            AstrometryOCRDroppedFrames = 0;
            AstrometryOCRDuplicatedFrames = 0;
		    AstrometryOCRTimeErrors = 0;
		    AAVConvertErrors = 0;

		    OperationInProgress = false;
			CanProcessLightCurvePixels = false;

		    CrashReportInfo = new CrashReportInfo();
		}
	}
}
