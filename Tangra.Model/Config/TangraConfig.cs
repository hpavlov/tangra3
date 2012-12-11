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

	public class TangraConfig
	{
		public static TangraConfig Settings = new TangraConfig(false);

		public TangraConfig()
			: this(true)
		{ }

		public TangraConfig(bool readOnly)
		{
			m_IsReadOnly = readOnly;
		}

		private bool m_IsReadOnly = true;
		public bool IsReadOnly
		{
			get { return m_IsReadOnly; }
		}

		public AdvsSettings ADVS = new AdvsSettings();

		private static void LoadXml(string xmlString)
		{
			XmlSerializer ser = new XmlSerializer(typeof(TangraConfig));
			using (TextReader rdr = new StringReader(xmlString))
			{
				try
				{
					Settings = (TangraConfig)ser.Deserialize(rdr);
					Settings.m_IsReadOnly = false;
				}
				catch (Exception ex)
				{
					Trace.WriteLine(ex);
					Settings = new TangraConfig(false);
				}
			}

			//if (Settings.DefaultsConfigurationVersion < TangraDefaults.Current.Version)
			//{
			//    TangraDefaults.Current.UpdateDefaultValues(Settings);
			//}
		}

		public static void Load()
		{
			try
			{
				using (IsolatedStorageFile fs = IsolatedStorageFile.GetMachineStoreForAssembly())
				using (IsolatedStorageFileStream isolatedStorageFileStream = new IsolatedStorageFileStream("TangraUserSettings.xml", FileMode.OpenOrCreate, fs))
				{
					string xmlContent;
					using (TextReader rdr = new StreamReader(isolatedStorageFileStream))
					{
						xmlContent = rdr.ReadToEnd();
					}

					LoadXml(xmlContent);
				}
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex);
				Settings = new TangraConfig(false);				
			}			
		}

		public void Save()
		{
			if (!m_IsReadOnly)
			{
				XmlSerializer ser = new XmlSerializer(typeof(TangraConfig));
				StringBuilder strData = new StringBuilder();
				using (TextWriter rdr = new StringWriter(strData))
				{
					ser.Serialize(rdr, this);
					rdr.Flush();
				}
				string xmlString = strData.ToString();
				try
				{
					using (IsolatedStorageFile fs = IsolatedStorageFile.GetMachineStoreForAssembly())
					using (IsolatedStorageFileStream isolatedStorageFileStream = new IsolatedStorageFileStream("TangraUserSettings.xml", FileMode.Create, fs))
					{
						using (TextWriter wrt = new StreamWriter(isolatedStorageFileStream))
						{
							wrt.Write(xmlString);
							isolatedStorageFileStream.Flush(true);
						}
					}
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

		public PhotometrySettings Photometry = new PhotometrySettings();
		public TrackingSettings Tracking = new TrackingSettings();
		public Colors Color = new Colors();
		public GenericSettings Generic = new GenericSettings();
		public SpecialSettings Special = new SpecialSettings();

		public class SaturationSettings
		{
			public byte Saturation8Bit = 250;
			public uint Saturation12Bit = 4000;

			public uint GetSaturationForBpp(int bpp)
			{
				if (bpp == 8)
					return Saturation8Bit;
				else if (bpp == 12)
					return Saturation12Bit;

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
			AverageBackground,
			BackgroundMode,
			BackgroundGradientFit,
			PSFBackground,
			BackgroundMedian
		}

		public enum PhotometryReductionMethod
		{
			AperturePhotometry,
			PsfPhotometryNumerical,
			PsfPhotometryAnalytical,
			OptimalExtraction
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

		public class PhotometrySettings
		{
			public PhotometrySettings()
			{
				Saturation = new SaturationSettings();
				m_EncodingGamma = 1.0f;
				for (int i = 0; i < 256; i++)
					m_DecodingGammaMatrix[i] = (byte)i;
			}

			public SaturationSettings Saturation;

			private float m_EncodingGamma = 1.0f;
			private byte[] m_DecodingGammaMatrix = new byte[256];
			public float EncodingGamma
			{
				get
				{
					return m_EncodingGamma;
				}
				set
				{
					if (m_EncodingGamma != value)
					{
						m_EncodingGamma = value;
						float decodingGamma = 1 / m_EncodingGamma;

						for (int i = 0; i < 256; i++)
							m_DecodingGammaMatrix[i] = (byte)Math.Max(0, Math.Min(255, Math.Round(256 * Math.Pow(i / 256.0, decodingGamma))));
					}
				}
			}

			public byte[] DecodingGammaMatrix
			{
				get { return m_DecodingGammaMatrix; }
			}

			public ColourChannel ColourChannel = ColourChannel.Red;

			public float DefaultSignalAperture = 1.0f;
			public SignalApertureUnit DefaultSignalApertureUnit = SignalApertureUnit.FWHM;

			public BackgroundMethod DefaultBackgroundMethod = BackgroundMethod.BackgroundMode;

			public PsfFittingMethod PsfFittingMethod = PsfFittingMethod.DirectNonLinearFit;
			public PsfQuadrature PsfQuadrature = PsfQuadrature.NumericalInAperture;
			public PsfFunction PsfFunction = PsfFunction.Gaussian;

			public bool UseUserSpecifiedFWHM;
			public float UserSpecifiedFWHM = 3.2f;

			public float AnulusInnerRadius = 2.0f;
			public int AnulusMinPixels = 350;

			public int SNFrameWindow = 64;

			public double MaxResidualStellarMags = 0.3;
		}

		public class TrackingSettings
		{
			public byte RefiningFrames = 16;
			public float DistanceTolerance = 6.0f;
			public int SearchRadius = 10;
			public bool WarnOnUnsatisfiedGuidingRequirements = false;
			public bool RecoverFromLostTracking = true;
			public bool PlaySound = true;
		}

		public class SpecialSettings
		{
			public int AboveMedianThreasholdForGuiding = 50;
			public float SignalNoiseForGuiding = 4.5f;
			public uint StarFinderAboveNoiseLevel = 25;

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

		public class GenericSettings
		{
			public bool RunVideosOnFastestSpeed = true;
			public bool UseMutleCPUTasking = true;
			public OnOpenOperation OnOpenOperation = OnOpenOperation.StartLightCurveReduction;
			public bool ShowProcessingSpeed = false;
			public PerformanceQuality PerformanceQuality;
			public bool GammaCorrectFullFrame = false;
			public int PreferredRenderingEngineIndex = 0;

			public bool AcceptBetaUpdates = false;

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
