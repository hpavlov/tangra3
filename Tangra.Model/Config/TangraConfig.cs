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

namespace Tangra.Model.Config
{
	public interface ILightCurveFormCustomizer
	{
		TangraConfig.LightCurvesDisplaySettings FormDisplaySettings { get; }
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
		LightCurve
	}

	public class RecentFilesConfig
	{
		internal RecentFilesConfig()
        {
			Lists.Clear();
			Lists.Add(0, new List<string>());
			Lists.Add(1, new List<string>());
			Lists.Add(2, new List<string>());
        }

		internal RecentFilesConfig(RecentFilesConfig copyFrom)
			: this()
		{
			if (copyFrom != null)
			{
				Lists[0].AddRange(copyFrom.Lists[0]);
				Lists[1].AddRange(copyFrom.Lists[1]);
				Lists[2].AddRange(copyFrom.Lists[2]);				
			}
		}

		private int MAX_RECENT_FILES = 20;

		public Dictionary<int, List<string>> Lists = new Dictionary<int, List<string>>();

		public void NewRecentFile(RecentFileType recentFilesGroup, string filePath)
		{
			if (Lists[(int)recentFilesGroup].IndexOf(filePath) != -1)
				Lists[(int)recentFilesGroup].Remove(filePath);

			Lists[(int)recentFilesGroup].Insert(0, filePath);

			while (Lists.Count > MAX_RECENT_FILES)
				Lists[(int)recentFilesGroup].RemoveAt(MAX_RECENT_FILES - 1);
		}

		private void LoadRecentFiles(string savedContent)
		{
			Lists[0].Clear();
			Lists[1].Clear();
			Lists[2].Clear();

			string[] lines = savedContent.Split('\n');
			foreach(string line in lines)
			{
				string[] tokens = line.Split('=');

				int listId = 0;
				if (tokens.Length == 2 && int.TryParse(tokens[0], out listId))
				{
					Lists[listId].Add(tokens[1].Trim());					
				}
			}
		}

		private string GetRecentFilesList()
		{
			var output = new StringBuilder();

			foreach (int key in Lists.Keys)
			{
				foreach (string fileName in Lists[key])
				{
					output.Append(string.Format("{0}={1}\n", key, fileName));
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

				Lists[0].Clear();
				Lists[1].Clear();
				Lists[2].Clear();
			}				
		}	
	}

	public enum DisplayIntensifyMode
	{
		Off,
		Lo,
		Hi
	}
	
	[XmlRoot("Tangra3Config")]
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

		public static void Reset(ISettingsSerializer serializer)
		{
			var persistedRecentFiles = new RecentFilesConfig(Settings.RecentFiles);
			Settings = new TangraConfig(serializer, false);
			Settings.RecentFiles = persistedRecentFiles;
		}

		public PhotometrySettings Photometry = new PhotometrySettings();
		public TrackingSettings Tracking = new TrackingSettings();
		public Colors Color = new Colors();
		public GenericSettings Generic = new GenericSettings();
		public SpecialSettings Special = new SpecialSettings();

	    public LastUsedSettings LastUsed = new LastUsedSettings();

		public TuningSettings Tuning = new TuningSettings();

		public class SaturationSettings
		{
			public byte Saturation8Bit = 250;
			public uint Saturation12Bit = 4000;
			public uint Saturation14Bit = 16000;

			public uint GetSaturationForBpp(int bpp)
			{
				if (bpp == 8)
					return Saturation8Bit;
				else if (bpp == 12)
					return Saturation12Bit;
				else if (bpp == 14)
					return Saturation14Bit;

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

		public class Background3DPolynomialSettings
		{
			public bool Try1stOrder = true;
			public bool Try2ndOrder = true;
			public bool Try3rdOrder = true;
		}

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

			public ColourChannel ColourChannel = ColourChannel.Red;

			public float DefaultSignalAperture = 1.2f;
			public SignalApertureUnit SignalApertureUnitDefault = SignalApertureUnit.FWHM;

			public BackgroundMethod BackgroundMethodDefault = BackgroundMethod.BackgroundMedian;

			
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

		public class TrackingSettings
		{
			public byte RefiningFrames = 8;
			public float DistanceTolerance = 6.0f;
			public int SearchRadius = 10;
			public bool WarnOnUnsatisfiedGuidingRequirements = false;
			public bool RecoverFromLostTracking = true;
			public bool PlaySound = true;
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

		public class TuningSettings
		{
			public OCRMode OcrMode = OCRMode.Mixed;
			public PSFFittingMode PsfMode = PSFFittingMode.NativeMatrixManagedFitting;
		}

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
		}

		public enum OnOpenOperation
		{
			DoNothing = 0,
			StartLightCurveReduction = 1
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

		public class GenericSettings
		{
			public bool RunVideosOnFastestSpeed = true;
			public bool UseMutleCPUTasking = true;
			public OnOpenOperation OnOpenOperation = OnOpenOperation.StartLightCurveReduction;
			public bool ShowProcessingSpeed = false;
			public bool ShowCursorPosition = false;
			public PerformanceQuality PerformanceQuality;
			public bool ReverseGammaCorrection = false;
			public int AviRenderingEngineIndex = 1;
			public bool DarkFrameAdjustLevelToMedian = false;

			public bool AcceptBetaUpdates = false;

			public bool UseHueIntensityDisplayMode = false;
			public bool UseInvertedDisplayMode = false;
			public DisplayIntensifyMode UseDisplayIntensifyMode = DisplayIntensifyMode.Off;

			public bool OsdOcrEnabled = true;
			public string OcrEngine = "IOTA-VTI";
			public bool OcrAskEveryTime = false;
			public bool OcrInitialSetupCompleted = false;
			public int OcrMaxNumberErrorsToAutoCorrect = 2;

			public int MaxCalibrationFieldsToAttempt = 25;

            public IsolationLevel AddinIsolationLevel = IsolationLevel.AppDomain;
			public OWExportMode OWEventTimesExportMode = OWExportMode.AlwaysExportEventTimes;
		}

        public class LastUsedSettings
        {
            public bool AdvancedLightCurveSettings = false;
            public PhotometryReductionMethod ReductionType = PhotometryReductionMethod.AperturePhotometry;
            public int MeasuringZoomImageMode = 0;

            public List<SameSizeApertureConfig> SameSizeApertures = new List<SameSizeApertureConfig>();
        }

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

		public class LightCurvesDisplaySettings
		{
			public Color BackgroundColor;
			public Brush BackgroundColorBrush;
			public Color GridLinesColor;
			public Pen GridLinesPen;
			public Color LabelsColor;
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
			public Brush[] TargetBrushes;
			public Pen[] WarningColorPens;
			public Brush[] WarningColorBrushes;

			public Pen Target1Pen;
			public Pen Target2Pen;
			public Pen Target3Pen;
			public Pen Target4Pen;

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
	}
}
