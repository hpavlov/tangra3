/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Tangra.Model.Helpers;

namespace Tangra.Model.Config
{
	public interface ILightCurveFormCustomizer
	{
		TangraConfig.LightCurvesDisplaySettings FormDisplaySettings { get; }
		void RedrawPlot();
	}

	public interface ISpectraViewFormCustomizer
	{
		TangraConfig.SpectraViewDisplaySettings FormDisplaySettings { get; }
		void RedrawPlot();
	}

	public interface IAdvStatusPopupFormCustomizer
	{
		void UpdateSettings(AdvsSettings advSettings);
		void RefreshState();
	}

	public interface IAavStatusPopupFormCustomizer
	{
		void UpdateSettings(AavSettings advSettings);
		void RefreshState();
	}

	public interface IAddinContainer
	{
		void ReloadAddins();
	}

    public interface ISettingsSerializer
    {
        string LoadSettings();
        void SaveSettings(string settings);

        string LoadRecentFiles();
        void SaveRecentFiles(string settings);
    }

	public enum RecentFileType
	{
		Video,
		LightCurve,
		MPCReport,
		Spectra,
        FitsSequence
	}

	public class RecentFilesConfig
	{
		internal RecentFilesConfig()
        {
			Lists.Clear();
			Lists.Add(RecentFileType.Video, new List<string>());
			Lists.Add(RecentFileType.LightCurve, new List<string>());
			Lists.Add(RecentFileType.MPCReport, new List<string>());
			Lists.Add(RecentFileType.Spectra, new List<string>());
            Lists.Add(RecentFileType.FitsSequence, new List<string>());
        }

		internal RecentFilesConfig(RecentFilesConfig copyFrom)
			: this()
		{
			if (copyFrom != null)
			{
				Lists[RecentFileType.Video].AddRange(copyFrom.Lists[RecentFileType.Video]);
				Lists[RecentFileType.LightCurve].AddRange(copyFrom.Lists[RecentFileType.LightCurve]);
				Lists[RecentFileType.MPCReport].AddRange(copyFrom.Lists[RecentFileType.MPCReport]);
				Lists[RecentFileType.Spectra].AddRange(copyFrom.Lists[RecentFileType.Spectra]);
                Lists[RecentFileType.FitsSequence].AddRange(copyFrom.Lists[RecentFileType.FitsSequence]);
			}
		}

		private int MAX_RECENT_FILES = 20;

		public Dictionary<RecentFileType, List<string>> Lists = new Dictionary<RecentFileType, List<string>>();

		public void NewRecentFile(RecentFileType recentFilesGroup, string filePath)
		{
			while (Lists[recentFilesGroup].IndexOf(filePath) != -1)
				Lists[recentFilesGroup].Remove(filePath);

			Lists[recentFilesGroup].Insert(0, filePath);

			while (Lists[recentFilesGroup].Count > MAX_RECENT_FILES)
				Lists[recentFilesGroup].RemoveAt(MAX_RECENT_FILES - 1);
		}

		private void LoadRecentFiles(string savedContent)
		{
			Lists[RecentFileType.Video].Clear();
			Lists[RecentFileType.LightCurve].Clear();
			Lists[RecentFileType.MPCReport].Clear();
			Lists[RecentFileType.Spectra].Clear();
            Lists[RecentFileType.FitsSequence].Clear();

			string[] lines = savedContent.Split('\n');
			foreach(string line in lines)
			{
				string[] tokens = line.Split('=');

				int listId = 0;
				if (tokens.Length == 2 && int.TryParse(tokens[0], out listId))
				{
                    Lists[(RecentFileType)listId].Add(tokens[1].Replace('*', '=').Trim());
				}
			}
		}

		private string GetRecentFilesList()
		{
			var output = new StringBuilder();

			foreach (RecentFileType key in Lists.Keys)
			{
				foreach (string fileName in Lists[key])
				{
					output.Append(string.Format("{0}={1}\n", (int)key, fileName.Replace('=','*')));
				}				
			}

			return output.ToString();
		}

		public string GetContentToSave()
		{
			return GetRecentFilesList();
		}

        public void Load(string savedContent)
		{
			try
			{
                LoadRecentFiles(savedContent);
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex);

				Lists[RecentFileType.Video].Clear();
				Lists[RecentFileType.LightCurve].Clear();
				Lists[RecentFileType.MPCReport].Clear();
				Lists[RecentFileType.Spectra].Clear();
                Lists[RecentFileType.FitsSequence].Clear();
			}
		}	
	}

	public enum DisplayIntensifyMode
	{
		Off,
		Lo,
		Hi,
		Dynamic
	}
	
	[XmlRoot("Tangra3Config")]
    [Serializable]
	public class TangraConfig
	{
		public static TangraConfig Settings;

        [XmlIgnore]
        public RecentFilesConfig RecentFiles = new RecentFilesConfig();

        public TangraConfig()
        { }

		private TangraConfig(ISettingsSerializer serializer, bool readOnly)
		{
			m_IsReadOnly = readOnly;
		    m_Serializer = serializer;
		}

	    private ISettingsSerializer m_Serializer;

		private bool m_IsReadOnly = true;
		public bool IsReadOnly
		{
			get { return m_IsReadOnly; }
		}

		public AdvsSettings ADVS = new AdvsSettings();
		public AavSettings AAV = new AavSettings();
		public SerSettings SER = new SerSettings();

        private static void LoadXml(string xmlString, string recentFiles)
		{
			var ser = new XmlSerializer(typeof(TangraConfig));
			using (TextReader rdr = new StringReader(xmlString))
			{
				Settings = (TangraConfig)ser.Deserialize(rdr);
				Settings.m_IsReadOnly = false;
			}

            Settings.RecentFiles.Load(recentFiles);

			//if (Settings.DefaultsConfigurationVersion < TangraDefaults.Current.Version)
			//{
			//	TangraDefaults.Current.UpdateDefaultValues(Settings);
			//}
		}


        public static void Load(ISettingsSerializer serializer)
		{
			try
			{
                string xmlContent = serializer.LoadSettings();
                string recentFiles = serializer.LoadRecentFiles();
                LoadXml(xmlContent, recentFiles);

			    Settings.m_Serializer = serializer;
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex);
                Settings = new TangraConfig(serializer, false);				
			}			
		}

        public void Save()
		{
			if (!m_IsReadOnly)
			{
				this.PrepareForSaving();

				var ser = new XmlSerializer(typeof(TangraConfig));
				var strData = new StringBuilder();
				using (TextWriter rdr = new StringWriter(strData))
				{
					ser.Serialize(rdr, this);
					rdr.Flush();
				}
				string xmlString = strData.ToString();
			    string recentFiles = RecentFiles.GetContentToSave();

				try
				{
				    m_Serializer.SaveSettings(xmlString);
                    m_Serializer.SaveRecentFiles(recentFiles);
				}
				catch (UnauthorizedAccessException)
				{
					// TODO: We may have already deleted the previous settings. Think of a better way to save the settings
				}
				catch (IOException ex)
				{
					Trace.WriteLine(ex.ToString());
				}
			}
		}

		public void PrepareForSaving()
		{
			if (PlateSolve.VideoCameras.Count > 0)
				PlateSolve.VideoCameras = PlateSolve.VideoCameras.Distinct(PlateSolve).ToList();
		}

		public static void Reset(ISettingsSerializer serializer)
		{
			var persistedRecentFiles = new RecentFilesConfig(Settings.RecentFiles);
			Settings = new TangraConfig(serializer, false);
			Settings.RecentFiles = persistedRecentFiles;
		}

		public bool HasSiteCoordinatesOrCOD
		{
			get
			{
				return
					(Astrometry.UseMPCCode && !string.IsNullOrEmpty(Astrometry.MPCObservatoryCode)) ||
					(!double.IsNaN(Generic.Longitude) && !double.IsNaN(Generic.Latitude));
			}
		}

		public PhotometrySettings Photometry = new PhotometrySettings();
		public TrackingSettings Tracking = new TrackingSettings();
		public Colors Color = new Colors();
		public GenericSettings Generic = new GenericSettings();
		public SpecialSettings Special = new SpecialSettings();
		public AstrometrySettings Astrometry = new AstrometrySettings();
		public SpectroscopySettings Spectroscopy = new SpectroscopySettings();
		public CatalogSettings StarCatalogue = new CatalogSettings();
		public PlateSolveSettings PlateSolve = new PlateSolveSettings();
		public UrlSettings Urls = new UrlSettings();
		public HelpSystemFlags HelpFlags = new HelpSystemFlags();

	    public LastUsedSettings LastUsed = new LastUsedSettings();

		public TuningSettings Tuning = new TuningSettings();
        public TraceLevelSettings TraceLevels = new TraceLevelSettings();

        public RecentFITSFieldConfigSettings RecentFITSFieldConfig = new RecentFITSFieldConfigSettings();

        [Serializable]
		public class SaturationSettings
		{
			public byte Saturation8Bit = 250;
			public uint Saturation12Bit = 4000;
			public uint Saturation14Bit = 16000;
			public uint Saturation16Bit = 65000;

			public uint GetSaturationForBpp(int bpp, uint maxSignalValue)
			{
				if (bpp == 8)
					return Saturation8Bit;
				else if (bpp == 12)
					return Saturation12Bit;
				else if (bpp == 14)
					return Saturation14Bit;
				else if (bpp == 16)
				{
				    if (maxSignalValue > 0 && maxSignalValue != uint.MinValue && maxSignalValue != uint.MaxValue)
				        return (uint)(0.95 * maxSignalValue); // 95% of max signal value
                    else
				        return Saturation16Bit;
				}

			    return (uint)((1 << bpp) - 1);
			}
		}

		public enum ColourChannel
		{
			Red,
			Green,
			Blue,
			GrayScale
		}

		public enum BackgroundMethod
		{
			AverageBackground = 0,
			BackgroundMode = 1,
			Background3DPolynomial = 2,
			PSFBackground = 3,
			BackgroundMedian = 4
		}

		public enum PhotometryReductionMethod
		{
			AperturePhotometry,
			PsfPhotometry,
			OptimalExtraction
		}

        public enum PreProcessingFilter
        {
            NoFilter,
            LowPassFilter,
            LowPassDifferenceFilter
        }

		public enum SignalApertureUnit
		{
			FWHM,
			Pixels
		}

		public enum PsfFittingMethod
		{
			DirectNonLinearFit,
			LinearFitOfAveragedModel
		}

		public enum PsfFunction
		{
			Gaussian
		}

		public enum PsfQuadrature
		{
			NumericalInAperture,
			Analytical
		}

		public enum KnownCameraResponse
		{
			Undefined = 0,

            [Description("910HX")]
			Wat910HXBD = 1
		}

        [Serializable]
		public class Background3DPolynomialSettings
		{
			public int Order = 2;
		}

        [Serializable]
		public class PhotometrySettings
		{
			public PhotometrySettings()
			{
				Saturation = new SaturationSettings();
				Background3DPoly = new Background3DPolynomialSettings();
			}

			public SaturationSettings Saturation;

			public Background3DPolynomialSettings Background3DPoly;

			public float RememberedEncodingGammaNotForDirectUse = 1.0f;

			public float EncodingGamma = 1.0f;

			public KnownCameraResponse KnownCameraResponse = KnownCameraResponse.Undefined;
            public int[] KnownCameraResponseParams = null;

			public ColourChannel ColourChannel = ColourChannel.Red;

			public float DefaultSignalAperture = 1.2f;
			public int SubPixelSquareSize = 4;
			public SignalApertureUnit SignalApertureUnitDefault = SignalApertureUnit.FWHM;

			public BackgroundMethod BackgroundMethodDefault = BackgroundMethod.AverageBackground;

			
			public PsfFittingMethod PsfFittingMethod = PsfFittingMethod.DirectNonLinearFit;
			public PsfQuadrature PsfQuadrature = PsfQuadrature.NumericalInAperture;
			public PsfFunction PsfFunction = PsfFunction.Gaussian;

			public bool UseUserSpecifiedFWHM;
			public float UserSpecifiedFWHM = 3.2f;

			public float AnnulusInnerRadius = 2.0f;
			public int AnnulusMinPixels = 350;

			public int SNFrameWindow = 64;

			public double MaxResidualStellarMags = 0.3;

		    public static float REJECTION_BACKGROUND_PIXELS_STD_DEV = 6.0f;
		}

		public enum TrackingEngine
		{
			LetTangraChoose,
			TrackingWithRefining,
			AdHocTracking
		}

        [Serializable]
		public class TrackingSettings
		{
			public byte RefiningFrames = 8;
			public float DistanceTolerance = 6.0f;
			public int SearchRadius = 10;
			public bool WarnOnUnsatisfiedGuidingRequirements = false;
			public bool RecoverFromLostTracking = true;
			public bool PlaySound = true;
			public bool PopUpOnLostTracking = true;
			public TrackingEngine SelectedEngine;
			public float AdHokMinCloseDistance = 1.41f;
			public float AdHokMaxCloseDistance = 3;
			public float AdHokMaxElongation = 150.0f; // 150%
			public bool CheckElongation = false;
			public float AdHokMinCertainty = 0.1f;
			public float AdHokGuidingStarMinCertainty = 0.4f;
			public float AdHokMinFWHM = 1.5f;
			public float AdHokMaxFWHM = 12.0f;
			public bool UseNativeTracker = true;
		}

		public enum PSFFittingMode
		{
			FullyManaged,
			NativeMatrixManagedFitting,
			FullyNative
		}

		public enum OCRMode
		{
			FullyManaged,
			Mixed
		}

        [Serializable]
		public class TuningSettings
		{
			public OCRMode OcrMode = OCRMode.Mixed;
			public PSFFittingMode PsfMode = PSFFittingMode.NativeMatrixManagedFitting;
		}

        [Serializable]
        public class TraceLevelSettings
	    {
            public TraceLevel PlateSolving = TraceLevel.Verbose;
	    }

        [Serializable]
		public class SpecialSettings
		{
			public int AboveMedianThreasholdForGuiding = 50;
			public float SignalNoiseForGuiding = 4.5f;
			public uint StarFinderAboveNoiseLevel = 25;
            public float TimesStdDevsForBackgroundNoise = 1.0f;
            public float TimesStdDevsForBackgroundNoiseLP = 1.0f;
            public float TimesStdDevsForBackgroundNoiseLPD = 1.0f;
            public float AboveMedianCoeffLP = 0.8f;
            public float AboveMedianCoeffLPD = 0.2f;

            public float BrightnessFluctoationCoefficientHighFlickering = 1.75f;
            public float BrightnessFluctoationCoefficientNoFlickering = 1.5f;

            public double StarFinderMinFWHM = 2.2;
            public double StarFinderMaxFWHM = 6.5;
            public double StarFinderMinSeparation = 3.5;
            public double StarFinderMinDistanceOfPeakPixelFromCenter = 4.25;
            public int StarFinderFitArea = 7;
            public double MinDistanceForPhotometricGroupingInFWHM = 4.0;
            public int StarFinderMaxNumberOfPotentialStars = 500;
            public float RejectionBackgroundPixelsStdDev = 6.0f;

            public int DefaultComparisonStarPsfFitMatrixSize = 11;
            public int DefaultOccultedStarPsfFitMatrixSize = 15;

            public int AddStarImageFramesToIntegrate = 4;

            public double ToleranceFWHMCoeff = 0.33;
            public double ToleranceTwoBadnessCoeff = 6.67;
            public double ToleranceOneBadnessCoeff = 5.0;
            public double ToleranceMaxValue = 3.5;
            public double ToleranceMinValueFullDisappearance = 1.0;
            public double ToleranceMinValue = 2.0;

            public float MaxAllowedTimestampShiftInMs = 5.0f;
            public float TimesHigherPositionToleranceForFullyOccultedStars = 2.0f;
            public float PalNtscFrameRateDifference = 0.0025f;

		    public float MinGuidingStarCertainty = 0.1f;
            public float GoodGuidingStarCertainty = 0.5f;

			#region Lost Tracking Configuration
			public float LostTrackingMinDistance = 4f;
			public float LostTrackingMinSignalCoeff = 0.38f;
			public float LostTrackingFWHMCoeff = 0.66f;
			public float LostTrackingPositionToleranceCoeff = 4.5f;

			//                               Level: 1   -   2   -   3   -   4   -   5
			//            LostTrackingMinDistance : 4       4       4       4       4
			//         LostTrackingMinSignalCoeff : 0.66    0.5     0.38    0.33    0.25
			//              LostTrackingFWHMCoeff : 0.25    0.33    0.66    0.80    1.00
			// LostTrackingPositionToleranceCoeff : 2.0     4.0     4.5     5.0     5.5
			private float[] CONST_LostTrackingMinDistances = new float[] { 4, 4, 4, 4, 4 };
			private float[] CONST_LostTrackingMinSignalCoeff = new float[] { 0.66f, 0.5f, 0.38f, 0.33f, 0.25f };
			private float[] CONST_LostTrackingFWHMCoeff = new float[] { 0.25f, 0.33f, 0.66f, 0.80f, 1.00f };
			private float[] CONST_LostTrackingPositionToleranceCoeff = new float[] { 2.0f, 4.0f, 4.5f, 5.0f, 5.5f };

			private int m_LostTrackingToleranceLevel = 2;
			public int LostTrackingToleranceLevel
			{
				get
				{
					return m_LostTrackingToleranceLevel;
				}
				set
				{
					m_LostTrackingToleranceLevel = Math.Max(0, Math.Min(value, 4));
					LostTrackingMinDistance = CONST_LostTrackingMinDistances[m_LostTrackingToleranceLevel];
					LostTrackingMinSignalCoeff = CONST_LostTrackingMinSignalCoeff[m_LostTrackingToleranceLevel];
					LostTrackingFWHMCoeff = CONST_LostTrackingFWHMCoeff[m_LostTrackingToleranceLevel];
					LostTrackingPositionToleranceCoeff = CONST_LostTrackingPositionToleranceCoeff[m_LostTrackingToleranceLevel];
				}
			}
			#endregion

		    public bool AllowLCMagnitudeDisplay = false;
		}

		public enum OnOpenOperation
		{
			DoNothing = 0,
			StartLightCurveReduction = 1,
			Astrometry = 2,
			Spectroscopy = 3
		}

		public enum PerformanceQuality
		{
			Responsiveness,
			Speed
		}

        public enum IsolationLevel
        {
            AppDomain,
            None
        }

		public enum OWExportMode
		{
			AlwaysExportEventTimes,
			AskBeforeExportingEventTimes,
			DontExportEventTimes
		}

        [Serializable]
		public class GenericSettings
		{
			public bool RunVideosOnFastestSpeed = true;
			public bool UseMutleCPUTasking = true;
			public OnOpenOperation OnOpenOperation = OnOpenOperation.StartLightCurveReduction;
			public bool ShowProcessingSpeed = false;
			public bool ShowCursorPosition = false;
			public PerformanceQuality PerformanceQuality;
			public bool ReverseGammaCorrection = false;
			public bool ReverseCameraResponse = false;
			public int AviRenderingEngineIndex = 1;

			public bool AcceptBetaUpdates = false;
			public bool SubmitUsageStats = true;

			public bool UseHueIntensityDisplayMode = false;
			public bool UseInvertedDisplayMode = false;
			public DisplayIntensifyMode UseDisplayIntensifyMode = DisplayIntensifyMode.Off;

			public bool OsdOcrEnabled = true;
			public string OcrEngine = "IOTA-VTI";
			public bool OcrAskEveryTime = false;
			public bool OcrInitialSetupCompleted = false;
            public int OcrMaxNumberDigitsToAutoCorrect = 3;

			public int MaxCalibrationFieldsToAttempt = 25;

            public IsolationLevel AddinIsolationLevel = IsolationLevel.AppDomain;
            public bool AddinDebugTraceEnabled = false;
			public OWExportMode OWEventTimesExportMode = OWExportMode.AlwaysExportEventTimes;

			public double Longitude = double.NaN;
			public double Latitude = double.NaN;

            public string[] CustomFITSTimeStampFormats = new string[0];
            public string[] CustomFITSDateFormats = new string[0];
            public string[] CustomFITSTimeFormats = new string[0];

            public bool TangraEndOfLifeWarningShown = false;
		}

        [Serializable]
        public class LastUsedSettings
        {
            public bool AdvancedLightCurveSettings = false;
            public PhotometryReductionMethod AsteroidalReductionType = PhotometryReductionMethod.AperturePhotometry;
            public BackgroundMethod AsteroidalBackgroundReductionMethod = BackgroundMethod.AverageBackground;
			public PhotometryReductionMethod MutualReductionType = PhotometryReductionMethod.AperturePhotometry;
			public BackgroundMethod MutualBackgroundReductionMethod = BackgroundMethod.AverageBackground;
			public PreProcessingFilter MutualDigitalFilter = PreProcessingFilter.NoFilter;
            public int MeasuringZoomImageMode = 0;

	        public int SerFileLastBitPix = 16;
			public double SerFileLastFrameRate = 25.0;

            public List<SameSizeApertureConfig> SameSizeApertures = new List<SameSizeApertureConfig>();
	        public string FitsSeqenceLastFolderLocation;
            public string FitsExportLastFolderLocation;
            public string FitsTimestampFormat;
            public string FitsDateFormat;
            public string FitsTimeFormat;
	        public string EmailAddressForErrorReporting;

	        public double FocalLength;
	        public string AstrRAHours;
	        public string AstrDEDeg;
	        public DateTime ObservationEpoch = new DateTime(2014, 03, 01);
	        public DateTime LastIdentifyObjectsDate;

	        public PhotometryReductionMethod AstrometryPhotometryReductionMethod;
	        public BackgroundMethod AstrometryPhotometryBackgroundMethod;
	        public bool AstrometryFitMagnitudes;
	        public DateTime LastAstrometryUTCDate;
	        public double AstrErrFoVs;
	        public double AstrMinMag;
	        public double AstrMaxMag;
	        public int AstrSearchTypeIndex;

            public float? AstrometryMagFitAperture = null;
            public float? AstrometryMagFitGap = null;
            public float? AstrometryMagFitAnnulus = null;

	        public string SpectroscopyWavelengthConfigurationname;

            public int ReleaseNotesDisplayedForVersion = 0;
        }

        [Serializable]
        public class SameSizeApertureConfig
        {
            public float Value;
            public int ValueMode;
            public int FwhmMode;

            public override string ToString()
            {
                if (ValueMode == 0)
                    return string.Format("{0} pixels", Value.ToString("0.0"));
                else if (ValueMode == 1)
                {
                    if (FwhmMode == 0)
                    {
                        return string.Format("{0} FWHM of Largest Star", Value.ToString("0.0"));
                    }
                    else if (FwhmMode == 1)
                    {
                        return string.Format("{0} FWHM of Average Star", Value.ToString("0.0"));
                    }
                }

                return "Invalid";
            }
        }

        [Serializable]
		public class Colors
		{
			[XmlIgnore]
			public Color Saturation
			{
				get
				{
					return System.Drawing.Color.FromArgb(RGBSaturation);
				}
				set
				{
					RGBSaturation = value.ToArgb();
				}
			}

			[XmlIgnore]
			public Color Target1
			{
				get
				{
					return System.Drawing.Color.FromArgb(RGBTarget1);
				}
				set
				{
					RGBTarget1 = value.ToArgb();
				}
			}

			[XmlIgnore]
			public Color Target2
			{
				get
				{
					return System.Drawing.Color.FromArgb(RGBTarget2);
				}
				set
				{
					RGBTarget2 = value.ToArgb();
				}
			}

			[XmlIgnore]
			public Color Target3
			{
				get
				{
					return System.Drawing.Color.FromArgb(RGBTarget3);
				}
				set
				{
					RGBTarget3 = value.ToArgb();
				}
			}

			[XmlIgnore]
			public Color Target4
			{
				get
				{
					return System.Drawing.Color.FromArgb(RGBTarget4);
				}
				set
				{
					RGBTarget4 = value.ToArgb();
				}
			}

			[XmlIgnore]
			public Color LightcurvesBackground
			{
				get
				{
					return System.Drawing.Color.FromArgb(RGBLightcurvesBackground);
				}
				set
				{
					RGBLightcurvesBackground = value.ToArgb();
				}
			}

			[XmlIgnore]
			public Color LightcurvesGrid
			{
				get
				{
					return System.Drawing.Color.FromArgb(RGBLightCurveGrid);
				}
				set
				{
					RGBLightCurveGrid = value.ToArgb();
				}
			}

			[XmlIgnore]
			public Color LightcurvesLabels
			{
				get
				{
					return System.Drawing.Color.FromArgb(RGBLightCurveLabels);
				}
				set
				{
					RGBLightCurveLabels = value.ToArgb();
				}
			}

			[XmlIgnore]
			public Color LightcurvesTarget1
			{
				get
				{
					return System.Drawing.Color.FromArgb(RGBLightcurveTarget1);
				}
				set
				{
					RGBLightcurveTarget1 = value.ToArgb();
				}
			}

			[XmlIgnore]
			public Color LightcurvesTarget2
			{
				get
				{
					return System.Drawing.Color.FromArgb(RGBLightcurveTarget2);
				}
				set
				{
					RGBLightcurveTarget2 = value.ToArgb();
				}
			}

			[XmlIgnore]
			public Color LightcurvesTarget3
			{
				get
				{
					return System.Drawing.Color.FromArgb(RGBLightcurveTarget3);
				}
				set
				{
					RGBLightcurveTarget3 = value.ToArgb();
				}
			}

			[XmlIgnore]
			public Color LightcurvesTarget4
			{
				get
				{
					return System.Drawing.Color.FromArgb(RGBLightcurveTarget4);
				}
				set
				{
					RGBLightcurveTarget4 = value.ToArgb();
				}
			}

			[XmlIgnore]
			public Color SmallGraphFocusBackground
			{
				get
				{
					return System.Drawing.Color.FromArgb(RGBSmallGraphFocusBackground);
				}
				set
				{
					RGBSmallGraphFocusBackground = value.ToArgb();
				}
			}

			[XmlIgnore]
			public Color SelectionCursorColor
			{
				get
				{
					return System.Drawing.Color.FromArgb(RGBSelectionCursorColor);
				}
				set
				{
					RGBSelectionCursorColor = value.ToArgb();
				}
			}

			public int RGBSaturation = System.Drawing.Color.Fuchsia.ToArgb();
			public int RGBTarget1 = System.Drawing.Color.Aqua.ToArgb();
			public int RGBTarget2 = System.Drawing.Color.Yellow.ToArgb();
			public int RGBTarget3 = System.Drawing.Color.Lime.ToArgb();
			public int RGBTarget4 = System.Drawing.Color.MistyRose.ToArgb();

			public int RGBLightcurvesBackground = SystemColors.ControlDark.ToArgb();
			public int RGBLightCurveGrid = System.Drawing.Color.FromArgb(180, 180, 180).ToArgb();
			public int RGBLightCurveLabels = System.Drawing.Color.White.ToArgb();

			public int RGBLightcurveTarget1 = System.Drawing.Color.Aqua.ToArgb();
			public int RGBLightcurveTarget2 = System.Drawing.Color.Yellow.ToArgb();
			public int RGBLightcurveTarget3 = System.Drawing.Color.Lime.ToArgb();
			public int RGBLightcurveTarget4 = System.Drawing.Color.MistyRose.ToArgb();

			public int RGBSmallGraphFocusBackground = System.Drawing.Color.LightCoral.ToArgb();
			public int RGBSelectionCursorColor = System.Drawing.Color.Red.ToArgb();

			public bool LightcurvesUseCustomColors = false;
			public int LightcurvesColorScheme = 0;
			public float DatapointSize = 5.0f;
			public bool DrawGrid = true;
			public bool DrawInvalidDataPoints = false;
		}

		public enum LightCurvesColorScheme
		{
			Clasic,
			Pastel,
			Contrast,
			Custom
		}

        [Serializable]
		public class LightCurvesDisplaySettings
		{
			public Color BackgroundColor;
			public Brush BackgroundColorBrush;
			public Color GridLinesColor;
			public Pen GridLinesPen;
			public Color LabelsColor;
			public Pen LabelsPen;
			public Brush LabelsBrush;
			public Color SmallGraphFocusBackgroundBrushColor = System.Drawing.Color.LightCoral;
			public Brush SmallGraphFocusBackgroundBrush;

			public Color SelectionCursorColor = System.Drawing.Color.Red;
			public Pen SelectionCursorColorPen;
			public Brush SelectionCursorColorBrush;

			public float DatapointSize = 3.0f;

			public LightCurvesColorScheme ColorScheme = LightCurvesColorScheme.Clasic;
			private bool UseTangraTargetColorsSaved = true;
			public bool UseTangraTargetColors
			{
				get
				{
					switch (ColorScheme)
					{
						case LightCurvesColorScheme.Clasic:
							return true;

						case LightCurvesColorScheme.Pastel:
							return false;

						case LightCurvesColorScheme.Contrast:
							return false;

						default:
							return UseTangraTargetColorsSaved;
					}
				}
				set
				{
					if (ColorScheme == LightCurvesColorScheme.Custom)
						UseTangraTargetColorsSaved = value;
				}
			}

			private Color Target1CustomColor;
			private Color Target2CustomColor;
			private Color Target3CustomColor;
			private Color Target4CustomColor;

			public Color Target1Color
			{
				get
				{
					if (!UseTangraTargetColors)
						return Target1CustomColor;
					else
						return TangraConfig.Settings.Color.Target1;
				}
				set
				{
					if (!UseTangraTargetColors)
						Target1CustomColor = value;
				}
			}

			public Color Target2Color
			{
				get
				{
					if (!UseTangraTargetColors)
						return Target2CustomColor;
					else
						return TangraConfig.Settings.Color.Target2;
				}
				set
				{
					if (!UseTangraTargetColors)
						Target2CustomColor = value;
				}
			}

			public Color Target3Color
			{
				get
				{
					if (!UseTangraTargetColors)
						return Target3CustomColor;
					else
						return TangraConfig.Settings.Color.Target3;
				}
				set
				{
					if (!UseTangraTargetColors)
						Target3CustomColor = value;
				}
			}

			public Color Target4Color
			{
				get
				{
					if (!UseTangraTargetColors)
						return Target4CustomColor;
					else
						return TangraConfig.Settings.Color.Target4;
				}
				set
				{
					if (!UseTangraTargetColors)
						Target4CustomColor = value;
				}
			}


			public Color[] TargetColors;
			public Pen[] TargetPens;
			public Pen[] TargetBackgroundPens;
			public Brush[] TargetBrushes;
			public Pen[] WarningColorPens;
			public Brush[] WarningColorBrushes;

			public Pen Target1Pen;
			public Pen Target2Pen;
			public Pen Target3Pen;
			public Pen Target4Pen;

			public Pen Target1BgPen;
			public Pen Target2BgPen;
			public Pen Target3BgPen;
			public Pen Target4BgPen;

			public Brush Target1Brush;
			public Brush Target2Brush;
			public Brush Target3Brush;
			public Brush Target4Brush;

			public bool DrawGrid;
			public bool DrawInvalidDataPoints;

			public static Font LabelsFont = new Font(FontFamily.GenericMonospace, 9);

			public void Load()
			{
				BackgroundColor = TangraConfig.Settings.Color.LightcurvesBackground;
				GridLinesColor = TangraConfig.Settings.Color.LightcurvesGrid;
				LabelsColor = TangraConfig.Settings.Color.LightcurvesLabels;

				Target1CustomColor = TangraConfig.Settings.Color.LightcurvesTarget1;
				Target2CustomColor = TangraConfig.Settings.Color.LightcurvesTarget2;
				Target3CustomColor = TangraConfig.Settings.Color.LightcurvesTarget3;
				Target4CustomColor = TangraConfig.Settings.Color.LightcurvesTarget4;

				UseTangraTargetColorsSaved = TangraConfig.Settings.Color.LightcurvesUseCustomColors;
				ColorScheme = (LightCurvesColorScheme)TangraConfig.Settings.Color.LightcurvesColorScheme;
				DatapointSize = TangraConfig.Settings.Color.DatapointSize;
				DrawGrid = TangraConfig.Settings.Color.DrawGrid;
				DrawInvalidDataPoints = TangraConfig.Settings.Color.DrawInvalidDataPoints;

				SmallGraphFocusBackgroundBrushColor = TangraConfig.Settings.Color.SmallGraphFocusBackground;
				SelectionCursorColor = TangraConfig.Settings.Color.SelectionCursorColor;
			}

			public void Save()
			{
				TangraConfig.Settings.Color.LightcurvesBackground = BackgroundColor;
				TangraConfig.Settings.Color.LightcurvesGrid = GridLinesColor;
				TangraConfig.Settings.Color.LightcurvesLabels = LabelsColor;

				TangraConfig.Settings.Color.LightcurvesTarget1 = Target1CustomColor;
				TangraConfig.Settings.Color.LightcurvesTarget2 = Target2CustomColor;
				TangraConfig.Settings.Color.LightcurvesTarget3 = Target3CustomColor;
				TangraConfig.Settings.Color.LightcurvesTarget4 = Target4CustomColor;

				TangraConfig.Settings.Color.LightcurvesUseCustomColors = UseTangraTargetColorsSaved;
				TangraConfig.Settings.Color.LightcurvesColorScheme = (int)ColorScheme;

				TangraConfig.Settings.Color.DatapointSize = DatapointSize;
				TangraConfig.Settings.Color.DrawGrid = DrawGrid;
				TangraConfig.Settings.Color.DrawInvalidDataPoints = DrawInvalidDataPoints;

				TangraConfig.Settings.Color.SmallGraphFocusBackground = SmallGraphFocusBackgroundBrushColor;
				TangraConfig.Settings.Color.SelectionCursorColor = SelectionCursorColor;

				TangraConfig.Settings.Save();
			}

			public void Initialize()
			{
				if (GridLinesPen != null) GridLinesPen.Dispose();
				GridLinesPen = new Pen(GridLinesColor);

				if (LabelsBrush != null) LabelsBrush.Dispose();
				LabelsBrush = new SolidBrush(LabelsColor);

				if (LabelsPen != null) LabelsPen.Dispose();
				LabelsPen = new Pen(LabelsColor);

				if (SmallGraphFocusBackgroundBrush != null) SmallGraphFocusBackgroundBrush.Dispose();
				SmallGraphFocusBackgroundBrush = new SolidBrush(SmallGraphFocusBackgroundBrushColor);

				if (SelectionCursorColorBrush != null) SelectionCursorColorBrush.Dispose();
				SelectionCursorColorBrush = new SolidBrush(SelectionCursorColor);

				if (SelectionCursorColorPen != null) SelectionCursorColorPen.Dispose();
				SelectionCursorColorPen = new Pen(SelectionCursorColor);

				if (Target1Pen != null) Target1Pen.Dispose(); Target1Pen = new Pen(Target1Color);
				if (Target2Pen != null) Target2Pen.Dispose(); Target2Pen = new Pen(Target2Color);
				if (Target3Pen != null) Target3Pen.Dispose(); Target3Pen = new Pen(Target3Color);
				if (Target4Pen != null) Target4Pen.Dispose(); Target4Pen = new Pen(Target4Color);

				if (Target1BgPen != null) Target1BgPen.Dispose(); Target1BgPen = new Pen(System.Drawing.Color.FromArgb(64, Target1Color.R, Target1Color.G, Target1Color.B));
				if (Target2BgPen != null) Target2BgPen.Dispose(); Target2BgPen = new Pen(System.Drawing.Color.FromArgb(64, Target2Color.R, Target2Color.G, Target2Color.B));
				if (Target3BgPen != null) Target3BgPen.Dispose(); Target3BgPen = new Pen(System.Drawing.Color.FromArgb(64, Target3Color.R, Target3Color.G, Target3Color.B));
				if (Target4BgPen != null) Target4BgPen.Dispose(); Target4BgPen = new Pen(System.Drawing.Color.FromArgb(64, Target4Color.R, Target4Color.G, Target4Color.B));
				
				if (Target1Brush != null) Target1Brush.Dispose(); Target1Brush = new SolidBrush(Target1Color);
				if (Target2Brush != null) Target2Brush.Dispose(); Target2Brush = new SolidBrush(Target2Color);
				if (Target3Brush != null) Target3Brush.Dispose(); Target3Brush = new SolidBrush(Target3Color);
				if (Target4Brush != null) Target4Brush.Dispose(); Target4Brush = new SolidBrush(Target4Color);

				byte gsTarget1 = (byte)(.299 * Target1Color.R + .587 * Target1Color.G + .114 * Target1Color.B);
				byte gsTarget2 = (byte)(.299 * Target2Color.R + .587 * Target2Color.G + .114 * Target2Color.B);
				byte gsTarget3 = (byte)(.299 * Target3Color.R + .587 * Target3Color.G + .114 * Target3Color.B);
				byte gsTarget4 = (byte)(.299 * Target4Color.R + .587 * Target4Color.G + .114 * Target4Color.B);

				TargetColors = new Color[] { Target1Color, Target2Color, Target3Color, Target4Color };
				TargetPens = new Pen[] { Target1Pen, Target2Pen, Target3Pen, Target4Pen };
				TargetBackgroundPens = new Pen[] { Target1BgPen, Target2BgPen, Target3BgPen, Target4BgPen };
				TargetBrushes = new Brush[] { Target1Brush, Target2Brush, Target3Brush, Target4Brush };

				WarningColorPens = new Pen[]
        	                   	{
									new Pen(System.Drawing.Color.FromArgb(gsTarget1, gsTarget1, gsTarget1)), 
									new Pen(System.Drawing.Color.FromArgb(gsTarget2, gsTarget2, gsTarget2)), 
									new Pen(System.Drawing.Color.FromArgb(gsTarget3, gsTarget3, gsTarget3)), 
									new Pen(System.Drawing.Color.FromArgb(gsTarget4, gsTarget4, gsTarget4))
        	                   	};

				WarningColorBrushes = new Brush[]
        	                   	{
									new SolidBrush(System.Drawing.Color.FromArgb(gsTarget1, gsTarget1, gsTarget1)), 
									new SolidBrush(System.Drawing.Color.FromArgb(gsTarget2, gsTarget2, gsTarget2)), 
									new SolidBrush(System.Drawing.Color.FromArgb(gsTarget3, gsTarget3, gsTarget3)), 
									new SolidBrush(System.Drawing.Color.FromArgb(gsTarget4, gsTarget4, gsTarget4))
        	                   	};

				if (BackgroundColorBrush != null) BackgroundColorBrush.Dispose();
				BackgroundColorBrush = new SolidBrush(BackgroundColor);
			}

			private Color InvertColor(Color clr)
			{
				return System.Drawing.Color.FromArgb(clr.A, 255 - clr.R, 255 - clr.G, 255 - clr.B);
			}
		}

		public enum ColourPalette
		{
			Pastel,
			Intense1,
			Intense2,
			Broad,
			Rainbow1,
			Rainbow2,
			Contrast
		}

        [Serializable]
		public class SpectraViewDisplaySettings
		{
			public Color SpectraLineColor;
			public Color LegendColor;
			public Color GridLinesColor;
			public Color KnownLineColor;
			public Color SpectraApertureColor;
			public Color PlotBackgroundColor;

			public ColourPalette AbsFluxPlotPalette;

			public Font LabelsFont = new Font(FontFamily.GenericMonospace, 9);
			public Font LegendFont = new Font(FontFamily.GenericSansSerif, 8, FontStyle.Bold);

			public Brush[] GreyBrushes = new Brush[256];			
			public Brush LegendBrush;
			public Pen KnownLinePen;
			public Brush KnownLineBrush;
			public Pen GridLinesPen;
			public Pen SpectraPen;
			public Brush KnownLineLabelBrush;

			public Pen SpectraAperturePen;
			public Pen SpectraBackgroundPen;
            public Pen SpectraBackgroundFadedPen;
			public Pen PlotBackgroundPen;

			public Color[] AbsFluxColor;
			public Pen[] AbsFluxPen;
			public Pen[] AbsFluxObsPen;
			public Color AbsFluxDefaultColor;
			public Pen AbsFluxPenDefault;
			public Pen AbsFluxObsPenDefault;

			public void Load()
			{
				SpectraLineColor = TangraConfig.Settings.Spectroscopy.Colors.SpectraLineColor;
				LegendColor = TangraConfig.Settings.Spectroscopy.Colors.LegendColor;
				GridLinesColor = TangraConfig.Settings.Spectroscopy.Colors.GridLinesColor;
				KnownLineColor = TangraConfig.Settings.Spectroscopy.Colors.KnownLineColor;
				SpectraApertureColor = TangraConfig.Settings.Spectroscopy.Colors.SpectraApertureColor;
				PlotBackgroundColor = TangraConfig.Settings.Spectroscopy.Colors.PlotBackgroundColor;
				AbsFluxPlotPalette = TangraConfig.Settings.Spectroscopy.Colors.AbsFluxPlotPalette;
			}

			public void Save()
			{
				TangraConfig.Settings.Spectroscopy.Colors.SpectraLineColor = SpectraLineColor;
				TangraConfig.Settings.Spectroscopy.Colors.LegendColor = LegendColor;
				TangraConfig.Settings.Spectroscopy.Colors.GridLinesColor = GridLinesColor;
				TangraConfig.Settings.Spectroscopy.Colors.KnownLineColor = KnownLineColor;
				TangraConfig.Settings.Spectroscopy.Colors.SpectraApertureColor = SpectraApertureColor;
				TangraConfig.Settings.Spectroscopy.Colors.PlotBackgroundColor = PlotBackgroundColor;
				TangraConfig.Settings.Spectroscopy.Colors.AbsFluxPlotPalette = AbsFluxPlotPalette; 

				TangraConfig.Settings.Save();
			}

			public void Initialize()
			{
				if (LegendBrush != null) LegendBrush.Dispose();
				LegendBrush = new SolidBrush(LegendColor);

				if (SpectraPen != null) SpectraPen.Dispose();
				SpectraPen = new Pen(SpectraLineColor);

                if (GridLinesPen != null) GridLinesPen.Dispose();
                GridLinesPen = new Pen(GridLinesColor);

				if (KnownLinePen != null) KnownLinePen.Dispose();
				KnownLinePen = new Pen(System.Drawing.Color.FromArgb(60, KnownLineColor.R, KnownLineColor.G, KnownLineColor.B));

				if (KnownLineBrush != null) KnownLineBrush.Dispose();
				KnownLineBrush = new SolidBrush(System.Drawing.Color.FromArgb(60, KnownLineColor.R, KnownLineColor.G, KnownLineColor.B));

				if (KnownLineLabelBrush != null) KnownLineLabelBrush.Dispose();
				KnownLineLabelBrush = new SolidBrush(KnownLineColor);

				if (SpectraAperturePen != null) SpectraAperturePen.Dispose();
				SpectraAperturePen = new Pen(SpectraApertureColor);

				if (SpectraBackgroundPen != null) SpectraBackgroundPen.Dispose();
				SpectraBackgroundPen = new Pen(System.Drawing.Color.FromArgb(70, SpectraApertureColor.R, SpectraApertureColor.G, SpectraApertureColor.B));

                if (SpectraBackgroundFadedPen != null) SpectraBackgroundFadedPen.Dispose();
                SpectraBackgroundFadedPen = new Pen(System.Drawing.Color.FromArgb(30, SpectraApertureColor.R, SpectraApertureColor.G, SpectraApertureColor.B));

				if (PlotBackgroundPen != null) PlotBackgroundPen.Dispose();
				PlotBackgroundPen = new Pen(PlotBackgroundColor);

				for (int i = 0; i < 256; i++)
				{
					if (GreyBrushes[i] != null) GreyBrushes[i].Dispose();
					GreyBrushes[i] = new SolidBrush(System.Drawing.Color.FromArgb(i, i, i));
				}

				if (AbsFluxPen != null && AbsFluxPen.Length > 0)
					for (int i = 0; i < AbsFluxPen.Length; i++) if (AbsFluxPen[i] != null) AbsFluxPen[i].Dispose();

				if (AbsFluxObsPen != null && AbsFluxObsPen.Length > 0)
					for (int i = 0; i < AbsFluxObsPen.Length; i++) if (AbsFluxObsPen[i] != null) AbsFluxObsPen[i].Dispose();

				if (AbsFluxPenDefault != null) AbsFluxPenDefault.Dispose();
				if (AbsFluxObsPenDefault != null) AbsFluxObsPenDefault.Dispose();

				AbsFluxColor = new Color[15];
				AbsFluxPen = new Pen[15];
				AbsFluxObsPen = new Pen[15];

				// http://tools.medialab.sciences-po.fr/iwanthue/

				if (AbsFluxPlotPalette == ColourPalette.Intense1)
				{
					// INTENSE 1
					AbsFluxColor[0] = System.Drawing.Color.FromArgb(35, 119, 7);
					AbsFluxColor[1] = System.Drawing.Color.FromArgb(114, 58, 147);
					AbsFluxColor[2] = System.Drawing.Color.FromArgb(227, 58, 14);
					AbsFluxColor[3] = System.Drawing.Color.FromArgb(101, 28, 7);
					AbsFluxColor[4] = System.Drawing.Color.FromArgb(223, 158, 198);
					AbsFluxColor[5] = System.Drawing.Color.FromArgb(38, 93, 69);
					AbsFluxColor[6] = System.Drawing.Color.FromArgb(189, 174, 63);
					AbsFluxColor[7] = System.Drawing.Color.FromArgb(244, 79, 105);
					AbsFluxColor[8] = System.Drawing.Color.FromArgb(95, 198, 132);
					AbsFluxColor[9] = System.Drawing.Color.FromArgb(10, 90, 157);
					AbsFluxColor[10] = System.Drawing.Color.FromArgb(245, 140, 98);
					AbsFluxColor[11] = System.Drawing.Color.FromArgb(156, 166, 244);
					AbsFluxColor[12] = System.Drawing.Color.FromArgb(57, 70, 6);
					AbsFluxColor[13] = System.Drawing.Color.FromArgb(168, 34, 8);
					AbsFluxColor[14] = System.Drawing.Color.FromArgb(254, 110, 42);					
				}
				else if (AbsFluxPlotPalette == ColourPalette.Pastel)
				{
					// PASTEL
					AbsFluxColor[0] = System.Drawing.Color.FromArgb(151, 185, 152);
					AbsFluxColor[1] = System.Drawing.Color.FromArgb(228, 252, 130);
					AbsFluxColor[2] = System.Drawing.Color.FromArgb(236, 214, 249);
					AbsFluxColor[3] = System.Drawing.Color.FromArgb(229, 152, 139);
					AbsFluxColor[4] = System.Drawing.Color.FromArgb(202, 179, 106);
					AbsFluxColor[5] = System.Drawing.Color.FromArgb(135, 213, 236);
					AbsFluxColor[6] = System.Drawing.Color.FromArgb(247, 213, 177);
					AbsFluxColor[7] = System.Drawing.Color.FromArgb(160, 188, 100);
					AbsFluxColor[8] = System.Drawing.Color.FromArgb(206, 171, 167);
					AbsFluxColor[9] = System.Drawing.Color.FromArgb(255, 222, 219);
					AbsFluxColor[10] = System.Drawing.Color.FromArgb(214, 161, 118);
					AbsFluxColor[11] = System.Drawing.Color.FromArgb(173, 181, 224);
					AbsFluxColor[12] = System.Drawing.Color.FromArgb(242, 207, 109);
					AbsFluxColor[13] = System.Drawing.Color.FromArgb(239, 240, 206);
					AbsFluxColor[14] = System.Drawing.Color.FromArgb(129, 194, 202);					
				}
				else if (AbsFluxPlotPalette == ColourPalette.Intense2)
				{
					// INTENSE 2
					AbsFluxColor[0] = System.Drawing.Color.FromArgb(208, 26, 107);
					AbsFluxColor[1] = System.Drawing.Color.FromArgb(20, 201, 130);
					AbsFluxColor[2] = System.Drawing.Color.FromArgb(213, 158, 28);
					AbsFluxColor[3] = System.Drawing.Color.FromArgb(44, 105, 183);
					AbsFluxColor[4] = System.Drawing.Color.FromArgb(227, 81, 255);
					AbsFluxColor[5] = System.Drawing.Color.FromArgb(108, 27, 12);
					AbsFluxColor[6] = System.Drawing.Color.FromArgb(46, 107, 88);
					AbsFluxColor[7] = System.Drawing.Color.FromArgb(235, 33, 53);
					AbsFluxColor[8] = System.Drawing.Color.FromArgb(61, 64, 90);
					AbsFluxColor[9] = System.Drawing.Color.FromArgb(190, 158, 98);
					AbsFluxColor[10] = System.Drawing.Color.FromArgb(149, 59, 179);
					AbsFluxColor[11] = System.Drawing.Color.FromArgb(254, 140, 251);
					AbsFluxColor[12] = System.Drawing.Color.FromArgb(150, 151, 255);
					AbsFluxColor[13] = System.Drawing.Color.FromArgb(243, 130, 37);
					AbsFluxColor[14] = System.Drawing.Color.FromArgb(15, 94, 127);
				}
				else if (AbsFluxPlotPalette == ColourPalette.Broad)
				{
					// BROAD
					AbsFluxColor[0] = System.Drawing.Color.FromArgb(76, 0, 76);
					AbsFluxColor[1] = System.Drawing.Color.FromArgb(162, 0, 162);
					AbsFluxColor[2] = System.Drawing.Color.FromArgb(247, 0, 247);
					AbsFluxColor[3] = System.Drawing.Color.FromArgb(140, 0, 255);
					AbsFluxColor[4] = System.Drawing.Color.FromArgb(0, 0, 223);
					AbsFluxColor[5] = System.Drawing.Color.FromArgb(0, 27, 164);
					AbsFluxColor[6] = System.Drawing.Color.FromArgb(0, 141, 210);
					AbsFluxColor[7] = System.Drawing.Color.FromArgb(0, 255, 255);
					AbsFluxColor[8] = System.Drawing.Color.FromArgb(0, 164, 73);
					AbsFluxColor[9] = System.Drawing.Color.FromArgb(73, 164, 0);
					AbsFluxColor[10] = System.Drawing.Color.FromArgb(195, 225, 0);
					AbsFluxColor[11] = System.Drawing.Color.FromArgb(255, 195, 0);
					AbsFluxColor[12] = System.Drawing.Color.FromArgb(255, 73, 0);
					AbsFluxColor[13] = System.Drawing.Color.FromArgb(212, 0, 0);
					AbsFluxColor[14] = System.Drawing.Color.FromArgb(102, 0, 0);
				}
				else if (AbsFluxPlotPalette == ColourPalette.Rainbow1)
				{
					// RAINBOW 1
					AbsFluxColor[0] = System.Drawing.Color.FromArgb(0, 0, 127);
					AbsFluxColor[1] = System.Drawing.Color.FromArgb(127, 0, 182);
					AbsFluxColor[2] = System.Drawing.Color.FromArgb(102, 0, 226);
					AbsFluxColor[3] = System.Drawing.Color.FromArgb(0, 17, 255);
					AbsFluxColor[4] = System.Drawing.Color.FromArgb(0, 107, 255);
					AbsFluxColor[5] = System.Drawing.Color.FromArgb(0, 197, 255);
					AbsFluxColor[6] = System.Drawing.Color.FromArgb(26, 251, 229);
					AbsFluxColor[7] = System.Drawing.Color.FromArgb(91, 252, 164);
					AbsFluxColor[8] = System.Drawing.Color.FromArgb(157, 254, 98);
					AbsFluxColor[9] = System.Drawing.Color.FromArgb(223, 255, 32);
					AbsFluxColor[10] = System.Drawing.Color.FromArgb(249, 208, 6);
					AbsFluxColor[11] = System.Drawing.Color.FromArgb(252, 131, 3);
					AbsFluxColor[12] = System.Drawing.Color.FromArgb(254, 53, 1);
					AbsFluxColor[13] = System.Drawing.Color.FromArgb(226, 5, 0);
					AbsFluxColor[14] = System.Drawing.Color.FromArgb(153, 0, 0);
				}
				else if (AbsFluxPlotPalette == ColourPalette.Rainbow2)
				{
					// RAINBOW 2
					AbsFluxColor[0] = System.Drawing.Color.FromArgb(204, 0, 255);
					AbsFluxColor[1] = System.Drawing.Color.FromArgb(113, 36, 255);
					AbsFluxColor[2] = System.Drawing.Color.FromArgb(68, 73, 227);
					AbsFluxColor[3] = System.Drawing.Color.FromArgb(54, 111, 180);
					AbsFluxColor[4] = System.Drawing.Color.FromArgb(40, 148, 134);
					AbsFluxColor[5] = System.Drawing.Color.FromArgb(26, 186, 87);
					AbsFluxColor[6] = System.Drawing.Color.FromArgb(12, 223, 40);
					AbsFluxColor[7] = System.Drawing.Color.FromArgb(127, 255, 0);
					AbsFluxColor[8] = System.Drawing.Color.FromArgb(255, 182, 0);
					AbsFluxColor[9] = System.Drawing.Color.FromArgb(255, 109, 0);
					AbsFluxColor[10] = System.Drawing.Color.FromArgb(255, 36, 0);
					AbsFluxColor[11] = System.Drawing.Color.FromArgb(219, 0, 0);
					AbsFluxColor[12] = System.Drawing.Color.FromArgb(175, 73, 73);
					AbsFluxColor[13] = System.Drawing.Color.FromArgb(182, 146, 146);
					AbsFluxColor[14] = System.Drawing.Color.FromArgb(255, 191, 191);
				}
				else
				{
					// CONTRAST
					AbsFluxColor[0] = System.Drawing.Color.Lime;
					AbsFluxColor[1] = System.Drawing.Color.Aqua;
					AbsFluxColor[2] = System.Drawing.Color.Yellow;
					AbsFluxColor[3] = System.Drawing.Color.Fuchsia;
					AbsFluxColor[4] = System.Drawing.Color.FromArgb(255, 128, 0);
					AbsFluxColor[5] = System.Drawing.Color.Blue;
					AbsFluxColor[6] = System.Drawing.Color.Green;
					AbsFluxColor[7] = System.Drawing.Color.FromArgb(64, 64, 64);
					AbsFluxColor[8] = System.Drawing.Color.Red;
					AbsFluxColor[9] = System.Drawing.Color.GreenYellow;
					AbsFluxColor[10] = System.Drawing.Color.Salmon;
					AbsFluxColor[11] = System.Drawing.Color.SlateBlue;
					AbsFluxColor[12] = System.Drawing.Color.Honeydew;
					AbsFluxColor[13] = System.Drawing.Color.MistyRose;
					AbsFluxColor[14] = System.Drawing.Color.RosyBrown;
				}

				AbsFluxDefaultColor = System.Drawing.Color.Purple;

				for (int i = 0; i < 15; i++)
				{
					AbsFluxObsPen[i] = new Pen(System.Drawing.Color.FromArgb(100, AbsFluxColor[i]));
                    AbsFluxPen[i] = new Pen(AbsFluxColor[i]);
				}

				AbsFluxObsPenDefault = new Pen(System.Drawing.Color.FromArgb(100, AbsFluxDefaultColor));
                AbsFluxPenDefault = new Pen(AbsFluxDefaultColor);

			}
		}

        [Serializable]
        public class PersistedConfiguration
        {
            public string Name;
            public int Width;
            public int Height;
            public int Order;
            public float Dispersion;
            public float RMS;
            public float A;
            public float B;
            public float C;
            public float D;
            public bool IsCalibrated;

            public override string ToString()
            {
                return Name;
            }
        }

        [Serializable]
		public class SpectroscopySettings
		{
            [Serializable]
			public class SpectraColors
			{
				[XmlIgnore]
				public Color SpectraLineColor
				{
					get
					{
						return System.Drawing.Color.FromArgb(SpectraLineRGB);
					}
					set
					{
						SpectraLineRGB = value.ToArgb();
					}
				}

				[XmlIgnore]
				public Color LegendColor
				{
					get
					{
						return System.Drawing.Color.FromArgb(LegendColorRGB);
					}
					set
					{
						LegendColorRGB = value.ToArgb();
					}
				}

				[XmlIgnore]
				public Color GridLinesColor
				{
					get
					{
						return System.Drawing.Color.FromArgb(GridLinesColorRGB);
					}
					set
					{
						GridLinesColorRGB = value.ToArgb();
					}
				}

				[XmlIgnore]
				public Color KnownLineColor
				{
					get
					{
						return System.Drawing.Color.FromArgb(KnownLineColorRGB);
					}
					set
					{
						KnownLineColorRGB = value.ToArgb();
					}
				}

				[XmlIgnore]
				public Color SpectraApertureColor
				{
					get
					{
						return System.Drawing.Color.FromArgb(SpectraApertureColorRGB);
					}
					set
					{
						SpectraApertureColorRGB = value.ToArgb();
					}
				}

				[XmlIgnore]
				public Color PlotBackgroundColor
				{
					get
					{
						return System.Drawing.Color.FromArgb(PlotBackgroundColorRGB);
					}
					set
					{
						PlotBackgroundColorRGB = value.ToArgb();
					}
				}

				public int SpectraLineRGB = System.Drawing.Color.Aqua.ToArgb();
				public int LegendColorRGB = System.Drawing.Color.White.ToArgb();
				public int GridLinesColorRGB = System.Drawing.Color.FromArgb(180, 180, 180).ToArgb();
				public int KnownLineColorRGB = System.Drawing.Color.Blue.ToArgb();
				public int SpectraApertureColorRGB = System.Drawing.Color.Red.ToArgb();
				public int PlotBackgroundColorRGB = SystemColors.ControlDark.ToArgb();
				public ColourPalette AbsFluxPlotPalette = ColourPalette.Contrast;
			}

			public SpectroscopyInstrument Instrument;
			public int DefaultWavelengthCalibrationOrder;
		    public bool AllowNegativeValues;
			public int MinWavelength;
			public int MaxWavelength;
			public int AbsFluxResolution;
			public AbsFluxSampling Sampling;

			public SpectraColors Colors;

			public List<PersistedConfiguration> PersistedConfigurations = new List<PersistedConfiguration>();

			public SpectroscopySettings()
			{
				Colors = new SpectraColors();

				DefaultWavelengthCalibrationOrder = 1;
				Instrument = SpectroscopyInstrument.Grating;
			    AllowNegativeValues = true;

				MinWavelength = 4000;
				MaxWavelength = 8000;
				AbsFluxResolution = 10;
				Sampling = AbsFluxSampling.GaussianSmoothingAndBinning;
			}
		}

        [Serializable]
		public class AstrometrySettings : IAstrometrySettings
		{
            [Serializable]
			public class ColorSettings
			{
				[XmlIgnore]
				public Color ReferenceStar
				{
					get
					{
						return System.Drawing.Color.FromArgb(ReferenceStarRGB);
					}
					set
					{
						ReferenceStarRGB = value.ToArgb();
					}
				}

				[XmlIgnore]
				public Color RejectedReferenceStar
				{
					get
					{
						return System.Drawing.Color.FromArgb(RejectedReferenceStarRGB);
					}
					set
					{
						RejectedReferenceStarRGB = value.ToArgb();
					}
				}

				[XmlIgnore]
				public Color UndetectedReferenceStar
				{
					get
					{
						return System.Drawing.Color.FromArgb(UndetectedReferenceStarRGB);
					}
					set
					{
						UndetectedReferenceStarRGB = value.ToArgb();
					}
				}

				[XmlIgnore]
				public Color UserObject
				{
					get
					{
						return System.Drawing.Color.FromArgb(UserObjectRGB);
					}
					set
					{
						UserObjectRGB = value.ToArgb();
					}
				}

				[XmlIgnore]
				public Color CatalogueStar
				{
					get
					{
						return System.Drawing.Color.FromArgb(CatalogueStarRGB);
					}
					set
					{
						CatalogueStarRGB = value.ToArgb();
					}
				}

				public int ReferenceStarRGB = System.Drawing.Color.Lime.ToArgb();
				public int RejectedReferenceStarRGB = System.Drawing.Color.Orange.ToArgb();
				public int UndetectedReferenceStarRGB = System.Drawing.Color.Silver.ToArgb();
				public int UserObjectRGB = System.Drawing.Color.Yellow.ToArgb();
				public int CatalogueStarRGB = System.Drawing.Color.Blue.ToArgb();
			}

			public AstrometrySettings()
			{
				Colors = new ColorSettings();
				MPCHeader = new MPCHeaderSettings();

				DetectionLimit = 0.5; /* This is used to determine whether the fit is good */

				Method = AstrometricMethod.AutomaticFit;

				MinimumNumberOfStars = 7;
				MaximumNumberOfStars = 100;
                MaxResidualInPixels = 1.0;
				MaximumPSFElongation = 35;
			    AssumedPositionUncertaintyPixels = 0.1; // pixels
			    SmallestReportedUncertaintyArcSec = 0.1; // arcsec

				AlignmentMethod = FieldAlignmentMethod.Pyramid;
                MotionFitWeightingMode = WeightingMode.SNR;

#pragma warning disable 612,618
				PyramidDistanceTolerance = 6;
#pragma warning restore 612,618
				PyramidDistanceToleranceInPixels = 6;
				PyramidOptimumStarsToMatch = 25;
				DistributionZoneStars = 5;
				LimitReferenceStarDetection = 0.75; /* This is used for auto limit mag detection */
				MinReferenceStarFWHM = 1.8;
				MaxReferenceStarFWHM = 6.0;
				MaximumPSFElongation = 35; /* 35% */
				PyramidRemoveNonStellarObject = false;

				PyramidFocalLengthAllowance = 0.05;
				PyramidTimeoutInSeconds = 60;
			    PyramidNumberOfPivots = 3;
			    SaveDebugOutput = false;
			}

			public AstrometrySettings Clone()
			{
				AstrometrySettings clone = new AstrometrySettings();

				clone.DetectionLimit = this.DetectionLimit;
				clone.Method = this.Method;
				clone.MinimumNumberOfStars = this.MinimumNumberOfStars;
				clone.MaximumNumberOfStars = this.MaximumNumberOfStars;
                clone.MaxResidualInPixels = this.MaxResidualInPixels;
				clone.AlignmentMethod = this.AlignmentMethod;
#pragma warning disable 612,618
				clone.PyramidDistanceTolerance = this.PyramidDistanceTolerance;
#pragma warning restore 612,618
				clone.PyramidDistanceToleranceInPixels = this.PyramidDistanceToleranceInPixels;
				clone.PyramidOptimumStarsToMatch = this.PyramidOptimumStarsToMatch;
				clone.PyramidFocalLengthAllowance = this.PyramidFocalLengthAllowance;
				clone.PyramidTimeoutInSeconds = this.PyramidTimeoutInSeconds;
				clone.DistributionZoneStars = this.DistributionZoneStars;
				clone.LimitReferenceStarDetection = this.LimitReferenceStarDetection;
				clone.MinReferenceStarFWHM = this.MinReferenceStarFWHM;
				clone.MaxReferenceStarFWHM = this.MaxReferenceStarFWHM;
				clone.MaximumPSFElongation = this.MaximumPSFElongation;
				clone.PyramidRemoveNonStellarObject = this.PyramidRemoveNonStellarObject;
			    clone.PyramidNumberOfPivots = this.PyramidNumberOfPivots;
			    clone.AssumedPositionUncertaintyPixels = this.AssumedPositionUncertaintyPixels;
			    clone.MotionFitWeightingMode = this.MotionFitWeightingMode;
				return clone;
			}

			public ColorSettings Colors;
			public MPCHeaderSettings MPCHeader;

			public MagInputBand DefaultMagInputBand = MagInputBand.Unfiltered;
			public MagOutputBand DefaultMagOutputBand = MagOutputBand.CousinsR;

			public double AssumedTargetVRColour = 0.4;

			public double DetectionLimit { get; set; }

			public static int NumberStarsRequiredForQuadraticFit = 10;

			public AstrometricMethod Method { get; set; }

			public int MinimumNumberOfStars { get; set; }

			public int MaximumNumberOfStars { get; set; }

			public int MaximumPSFElongation { get; set; }

			public double MaxResidualInPixels { get; set; }

			public double MaxPreliminaryResidual
			{
                get { return MaxResidualInPixels * 6.0; }
			}

            public double AssumedPositionUncertaintyPixels { get; set; }

            public double SmallestReportedUncertaintyArcSec { get; set; }

			public FieldAlignmentMethod AlignmentMethod { get; set; }

			[Obsolete]
			public double PyramidDistanceTolerance { get; set; }

			public double PyramidDistanceToleranceInPixels { get; set; }

			public double PyramidOptimumStarsToMatch { get; set; }
			public int DistributionZoneStars { get; set; }
			public double LimitReferenceStarDetection { get; set; }

			public double MinReferenceStarFWHM { get; set; }
			public double MaxReferenceStarFWHM { get; set; }
			public bool PyramidRemoveNonStellarObject { get; set; }
            public int PyramidNumberOfPivots { get; set; }

            public double PyramidFocalLengthAllowance { get; set; }
			public bool PyramidForceFixedFocalLength
			{
				get { return PyramidFocalLengthAllowance <= 0; }
			}

			public int PyramidTimeoutInSeconds { get; set; }

			public bool UseMPCCode { get; set; }
			public string MPCObservatoryCode { get; set; }

            public bool ExportUncertainties { get; set; }

            public bool ExportHigherPositionAccuracy { get; set; }

            public WeightingMode MotionFitWeightingMode { get; set; }

            public bool SaveDebugOutput;
		}

        [Serializable]
		public class UrlSettings
		{
			public UrlSettings()
			{
				SetDefaults();
			}

			public void SetDefaults()
			{
                MPCEphe2HttpsServiceUrl = "https://www.minorplanetcenter.net/cgi-bin/mpeph2.cgi";
                MPCheckHttpsServiceUrl = "https://www.minorplanetcenter.net/cgi-bin/mpcheck.cgi";
			}

			public string MPCEphe2HttpsServiceUrl;
            public string MPCheckHttpsServiceUrl;
		}

        [Serializable]
		public class StarIDSettings
		{
			public double RAHours = double.NaN;
			public double DEDeg = double.NaN;
			public double ErrFoVs = double.NaN;
			public int Method = 0;
			public string ServerName = string.Empty;
			public string DatabaseName = string.Empty;
		}

        [Serializable]
		public class PlateSolveSettings : IEqualityComparer<VideoCamera>
		{
			public StarIDSettings StarIDSettings = new StarIDSettings();
			public string SelectedCameraModel;
			public string SelectedScopeRecorder;
			public bool UseLowPassForAstrometry;

			[XmlElement("SupportedCameras")]
			public List<VideoCamera> VideoCameras = new List<VideoCamera>();

			public List<ScopeRecorderConfiguration> ScopeRecorders = new List<ScopeRecorderConfiguration>();
			public List<PersistedPlateConstants> SolvedPlateConstants = new List<PersistedPlateConstants>();

			public PlateSolveSettings()
			{
				if (VideoCameras.Count == 0)
					VideoCamera.AddCameraConfigurations(VideoCameras);
			}

			public PersistedPlateConstants GetPlateConstants(VideoCamera camera, ScopeRecorderConfiguration config, Rectangle plateSize)
			{
				if (camera != null && config != null)
				{
					foreach (PersistedPlateConstants pltCon in SolvedPlateConstants)
					{
						if (pltCon.CameraModel == camera.Model &&
							pltCon.ScopeRecoderConfig == config.Title &&
							pltCon.PlateWidth == plateSize.Width &&
							pltCon.PlateHeight == plateSize.Height)
						{
							return pltCon;
						}
					}
				}

				return null;
			}

			public void SetPlateConstants(PersistedPlateConstants pltConst, Rectangle plateSize)
			{
				if (SelectedCameraModel != null &&
					SelectedScopeRecorder != null)
				{
					SetPlateConstants(SelectedCamera, SelectedScopeRecorderConfig, plateSize, pltConst);
				}
				else
					throw new InvalidOperationException();
			}

			public void SetPlateConstants(VideoCamera camera, ScopeRecorderConfiguration config, Rectangle plateSize, PersistedPlateConstants pltConst)
			{
				if (camera != null && config != null && pltConst != null)
				{
					foreach (PersistedPlateConstants pltCon in SolvedPlateConstants)
					{
						if (pltCon.CameraModel == camera.Model &&
							pltCon.ScopeRecoderConfig == config.Title &&
							pltCon.PlateWidth == plateSize.Width &&
							pltCon.PlateHeight == plateSize.Height)
						{
							pltCon.EffectiveFocalLength = pltConst.EffectiveFocalLength;
							pltCon.EffectivePixelHeight = pltConst.EffectivePixelHeight;
							pltCon.EffectivePixelWidth = pltConst.EffectivePixelWidth;

							return;
						}
					}

					pltConst.ScopeRecoderConfig = config.Title;
					pltConst.CameraModel = camera.Model;
					pltConst.PlateWidth = plateSize.Width;
					pltConst.PlateHeight = plateSize.Height;

					SolvedPlateConstants.Add(pltConst);
				}
			}

			public ScopeRecorderConfiguration SelectedScopeRecorderConfig
			{
				get
				{
					if (SelectedScopeRecorder != null)
					{
						return ScopeRecorders.FirstOrDefault((c) => c.Title == SelectedScopeRecorder);
					}
					else
						return null;
				}
			}

			public VideoCamera SelectedCamera
			{
				get
				{
					if (SelectedCameraModel != null)
					{
						return VideoCameras.FirstOrDefault((c) => c.Model == SelectedCameraModel);
					}
					else
						return null;
				}
			}

			public PersistedPlateConstants GetPlateConstants(Rectangle plateSize)
			{
				if (SelectedCameraModel != null &&
					SelectedScopeRecorder != null)
				{
					return GetPlateConstants(SelectedCamera, SelectedScopeRecorderConfig, plateSize);
				}
				else
					return null;
			}

			public bool Equals(VideoCamera x, VideoCamera y)
			{
				if (x == null && y == null) return true;
				else if (x == null || y == null) return false;
				else return x.Model == y.Model;
			}

			public int GetHashCode(VideoCamera obj)
			{
				if (obj == null || obj.Model == null) 
					return 0;

				return obj.Model.GetHashCode();
			}
		}

        [Serializable]
		public class PersistedPlateConstants
		{
			public string ScopeRecoderConfig;
			public string CameraModel;
			public int PlateWidth;
			public int PlateHeight;

			public double EffectiveFocalLength;
			public double EffectivePixelWidth;
			public double EffectivePixelHeight;
		}

		public enum MagOutputBand
		{
			JohnsonV,
			CousinsR
		}

		public enum MagInputBand
		{
			Unfiltered,
			JohnsonV,
			CousinsR,
			SLOAN_r,
			SLOAN_g
		}

        [Serializable]
		public class ScopeRecorderConfiguration
		{
            protected static double MIN_KIWI_X_PERC = 0; //0.08183;
            protected static double MAX_KIWI_X_PERC = 1; //0.91540;
            protected static double MIN_KIWI_Y_PERC = 0.83; //0.87370;
            protected static double MAX_KIWI_Y_PERC = 1; //0.93772;

			[XmlIgnore]
			public bool IsNew;

			public string Title;

			public int Top;
			public int Left;
			public int Width;
			public int Height;
			public ConfigurationLimitingMagnitudes LimitingMagnitudes;
			public ConfigurationRawFrameSizes RawFrameSizes;
			public bool FlipHorizontally = false;
			public bool FlipVertically = false;
		    public bool IsInclusionArea = false;
		    public Rectangle OSDExclusionArea;
            public Rectangle InclusionArea;

			public override string ToString()
			{
				return Title;
			}

			public ScopeRecorderConfiguration()
			{
				IsNew = false;
			}

			public ScopeRecorderConfiguration(string cameraModel, int width, int height)
			{
				LimitingMagnitudes = new ConfigurationLimitingMagnitudes();
				RawFrameSizes = new ConfigurationRawFrameSizes();
				RawFrameSizes[cameraModel] = new Rectangle(0, 0, width, height);
				
			    IsInclusionArea = false;
                OSDExclusionArea = GetDefaultOSDExclusionRectangle(width, height);
                InclusionArea = GetDefaultInclusionRectangle(width, height);

                IsNew = true;
			}

			public Rectangle GetRectangle()
			{
				return new Rectangle(Left, Top, Width, Height);
			}

            private Rectangle GetDefaultInclusionRectangle(int width, int height)
            {
                return new Rectangle(0, 0,
                                    (int)Math.Round((MAX_KIWI_X_PERC - MIN_KIWI_X_PERC) * width),
                                    (int)Math.Round((1 - (MAX_KIWI_Y_PERC - MIN_KIWI_Y_PERC)) * height));
            }

            private Rectangle GetDefaultOSDExclusionRectangle(int width, int height)
            {
                return new Rectangle(
                                    (int)Math.Round(MIN_KIWI_X_PERC * width),
                                    (int)Math.Round(MIN_KIWI_Y_PERC * height),
                                    (int)Math.Round((MAX_KIWI_X_PERC - MIN_KIWI_X_PERC) * width),
                                    (int)Math.Round((MAX_KIWI_Y_PERC - MIN_KIWI_Y_PERC) * height));
            }
		}

        [Serializable]
		public class ConfigurationLimitingMagnitudes
		{
			public List<string> Keys = new List<string>();
			public List<double> Values = new List<double>();

			public static double DEFAULT_LIMITING_MAGNITUDE = 12.0;

			internal ConfigurationLimitingMagnitudes()
			{ }

			public double this[string camera]
			{
				get
				{
					int idx = Keys.IndexOf(camera);
					if (idx != -1)
						return Values[idx];
					else
						return DEFAULT_LIMITING_MAGNITUDE;
				}
				set
				{
					int idx = Keys.IndexOf(camera);
					if (idx != -1)
						Values[idx] = value;
					else
					{
						Keys.Add(camera);
						Values.Add(value);
					}
				}
			}
		}

        [Serializable]
		public class ConfigurationRawFrameSizes
		{
			public List<string> Keys = new List<string>();
			public List<long> Values = new List<long>();

			internal ConfigurationRawFrameSizes()
			{ }

			public Rectangle this[string camera]
			{
				get
				{
					int idx = Keys.IndexOf(camera);
					if (idx != -1)
					{
						int width = (int)(Values[idx] % 10000);
						int height = (int)(Values[idx] / 10000);
						return new Rectangle(0, 0, width, height);
					}
					else
						return Rectangle.Empty;
				}
				set
				{
					if (value == Rectangle.Empty)
						throw new ArgumentException("Cannot set empty rectangle as frame size");

					int idx = Keys.IndexOf(camera);
					long valToSave = value.Width + 10000 * value.Height;
					if (idx != -1)
						Values[idx] = valToSave;
					else
					{
						Keys.Add(camera);
						Values.Add(valToSave);
					}
				}
			}
		}

		public enum StarCatalog
		{
			NotSpecified = 0,
			UCAC2 = 1,
			NOMAD = 2,
			UCAC3 = 3,
			PPMXL = 4,
			UCAC4 = 5
		}

        [Serializable]
		public class CatalogSettings
		{
			public StarCatalog Catalog = StarCatalog.NotSpecified;
			public string CatalogLocation = string.Empty;
			public Guid CatalogMagnitudeBandId = Guid.Empty;

			public static string GetNETCode(StarCatalog catalog)
			{
				switch (catalog)
				{
					case StarCatalog.NotSpecified:
						return string.Empty;
					case StarCatalog.UCAC2:
						return "UCAC2";
					case StarCatalog.UCAC3:
						return "UCAC3";
					case StarCatalog.NOMAD:
						return "NOMAD";
					case StarCatalog.PPMXL:
						return "PPMXL";
					case StarCatalog.UCAC4:
						return "UCAC4";
					default:
						throw new ArgumentOutOfRangeException("catalog");
				}
			}
		}

		public class MPCHeaderSettings
		{
			private string m_COD;
			private string m_CON;
			private string m_OBS;
			private string m_MEA;
			private string m_TEL;
			private string m_CON2;
			private string m_ACK;
			private string m_AC2;
			private string m_COM;

			private bool m_DefaultValues = true;

			public string COD
			{
				get { return m_COD; }
				set
				{
					m_COD = value;
					m_DefaultValues = false;
				}
			}

			public string CON
			{
				get { return m_CON; }
				set
				{
					m_CON = value;
					m_DefaultValues = false;
				}
			}

			public string OBS
			{
				get { return m_OBS; }
				set
				{
					m_OBS = value;
					m_DefaultValues = false;
				}
			}

			public string MEA
			{
				get { return m_MEA; }
				set
				{
					m_MEA = value;
					m_DefaultValues = false;
				}
			}

			public string TEL
			{
				get { return m_TEL; }
				set
				{
					m_TEL = value;
					m_DefaultValues = false;
				}
			}

			public string CON2
			{
				get { return m_CON2; }
				set
				{
					m_CON2 = value;
					m_DefaultValues = false;
				}
			}

			public string ACK
			{
				get { return m_ACK; }
				set
				{
					m_ACK = value;
					m_DefaultValues = false;
				}
			}

			public string AC2
			{
				get { return m_AC2; }
				set
				{
					m_AC2 = value;
					m_DefaultValues = false;
				}
			}

			public string COM
			{
				get { return m_COM; }
				set
				{
					m_COM = value;
					m_DefaultValues = false;
				}
			}

			public void UseDefaultValues()
			{
				m_COD = "XXX";
				m_CON = "John Smith [john.smith@email.com]";
				m_OBS = "J. Smith";
				m_MEA = "J. Smith";
				m_TEL = "0.20-m f/3.3 Schmidt-Cassegrain + GPS-tagged video";
				m_CON2 = "";
				m_COM = "";
				m_ACK = "";
				m_AC2 = "";

				m_DefaultValues = true;
			}

			public bool HasDefaultValues
			{
				get { return m_DefaultValues; }
			}
		}

		public class HelpSystemFlags
		{
			public bool DontShowCalibrationHelpFormAgain = false;
		}

		public enum SpectroscopyInstrument
		{
			Grating
		}

		public enum AbsFluxSampling
		{
			GaussianSmoothingAndBinning,
			LaGrangeInterpolation
		}

        public enum ExposureUnit
        {
            Seconds,
            Milliseconds,
            Microseconds,
            Nanoseconds,
            Minutes,
            Hours,
            Days
        }

        public enum TimeStampType
        {
            StartExposure,
            MidExposure,
            EndExposure
        }

        public class FITSFieldConfig
        {
            public string ExposureHeader;
            public ExposureUnit ExposureUnit;
            public string TimeStampHeader;
            public string TimeStampFormat;
            public string TimeStampHeader2;
            public string TimeStampFormat2;
            public bool TimeStampIsDateTimeParts;
            public TimeStampType TimeStampType;
            public string TimeStamp2Header;
            public string TimeStamp2Format;
            public string TimeStamp2Header2;
            public string TimeStamp2Format2;
            public bool TimeStamp2IsDateTimeParts;
            public bool IsTimeStampAndExposure;
            public string FileHash;
            public string CardNamesHash;
        }

	    public class RecentFITSFieldConfigSettings
	    {
	        public List<FITSFieldConfig> Items = new List<FITSFieldConfig>();

	        public void Register(FITSFieldConfig config)
	        {
	            foreach (var item in Items)
	            {
	                if (IsSameConfig(item, config))
	                {
	                    item.FileHash = config.FileHash;
                        item.CardNamesHash = config.CardNamesHash;
	                    return;
	                }
	            }

                while (Items.Count > 10) Items.RemoveAt(Items.Count - 1);

	            Items.Add(config);

                Settings.Generic.CustomFITSTimeStampFormats = AddValueToListIfNoPresent(Settings.Generic.CustomFITSTimeStampFormats, config.TimeStampFormat, config.TimeStamp2Format, FITSTimeStampReader.STANDARD_TIMESTAMP_FORMATS);
                Settings.Generic.CustomFITSDateFormats = AddValueToListIfNoPresent(Settings.Generic.CustomFITSDateFormats, config.TimeStampFormat, config.TimeStamp2Format, FITSTimeStampReader.STANDARD_DATE_FORMATS);
                Settings.Generic.CustomFITSTimeFormats = AddValueToListIfNoPresent(Settings.Generic.CustomFITSTimeFormats, config.TimeStampFormat2, config.TimeStamp2Format2, FITSTimeStampReader.STANDARD_TIME_FORMATS);
	        }

            private string[] AddValueToListIfNoPresent(string[] currList, string newValue1, string newValue2, string[] additionaExcludeList)
	        {
                var currLst = new List<string>(currList);

                var newVals = new string[] { newValue1, newValue2 };
                foreach (string newValue in newVals)
                {
	                if (!string.IsNullOrWhiteSpace(newValue) &&
	                    !currList.Contains(newValue) &&
	                    !additionaExcludeList.Contains(newValue))
	                {	                
	                    currLst.Add(newValue);
	                }                    
                }

                return currLst.ToArray();
	        }

            private bool IsSameConfig(FITSFieldConfig item1, FITSFieldConfig item2)
            {
                return
                    item1.IsTimeStampAndExposure == item2.IsTimeStampAndExposure &&
                    item1.TimeStampIsDateTimeParts == item2.TimeStampIsDateTimeParts &&
                    item1.TimeStamp2IsDateTimeParts == item2.TimeStamp2IsDateTimeParts &&
                    string.Equals(item1.ExposureHeader, item2.ExposureHeader, StringComparison.Ordinal) &&
                    string.Equals(item1.TimeStampFormat, item2.TimeStampFormat, StringComparison.Ordinal) &&
                    string.Equals(item1.TimeStampHeader, item2.TimeStampHeader, StringComparison.Ordinal) &&
                    (
                        !item1.TimeStampIsDateTimeParts ||
                        (string.Equals(item1.TimeStampFormat2, item2.TimeStampFormat2, StringComparison.Ordinal) && string.Equals(item1.TimeStampHeader2, item2.TimeStampHeader2, StringComparison.Ordinal))
                    ) &&
                    string.Equals(item1.TimeStamp2Format, item2.TimeStamp2Format, StringComparison.Ordinal) &&
                    string.Equals(item1.TimeStamp2Header, item2.TimeStamp2Header, StringComparison.Ordinal) &&
                    (
                        !item1.TimeStamp2IsDateTimeParts ||
                        (string.Equals(item1.TimeStamp2Format2, item2.TimeStamp2Format2, StringComparison.Ordinal) && string.Equals(item1.TimeStamp2Header2, item2.TimeStamp2Header2, StringComparison.Ordinal))

                    ) &&
                    item1.TimeStampType == item2.TimeStampType &&
                    item1.ExposureUnit == item2.ExposureUnit;
            }
	    }
	}
}
