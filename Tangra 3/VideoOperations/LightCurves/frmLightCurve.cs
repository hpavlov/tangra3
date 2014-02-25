using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Tangra.Controller;
using Tangra.Helpers;
using Tangra.Model.Config;
using Tangra.Model.Helpers;
using Tangra.Model.Numerical;
using Tangra.Model.Video;
using Tangra.Resources;
using Tangra.SDK;
using Tangra.VideoOperations.LightCurves.InfoForms;
using Tangra.VideoOperations.LightCurves.Report;
using Tangra.VideoOperations.LightCurves.Tracking;


namespace Tangra.VideoOperations.LightCurves
{
	public partial class frmLightCurve : Form, ILightCurveFormCustomizer, ILightCurveDataProvider, IAddinContainer
    {
	    private LightCurveController m_LightCurveController;
		private AddinsController m_AddinsController;

		private GeoLocationInfo m_GeoLocationInfo;

        private int m_NewMinDisplayedFrame = -1;

        internal static byte MSG_FORM_CLOSED = 7;
        internal static byte MSG_LOAD_LIGHT_CURVE_COMMAND = 8;

        private TangraConfig.LightCurvesDisplaySettings m_DisplaySettings = new TangraConfig.LightCurvesDisplaySettings();

		public frmLightCurve(LightCurveController controller, AddinsController addinsController)
        {
            InitializeComponent();

            m_LightCurveController = controller;
			m_AddinsController = addinsController;

            // Not implemented yet, may be one day ...
            miFullReprocess.Visible = false;

            m_DisplaySettings.Load();
            m_DisplaySettings.Initialize();

            m_AllReadings[0] = new List<LCMeasurement>();
            m_AllReadings[1] = new List<LCMeasurement>();
            m_AllReadings[2] = new List<LCMeasurement>();
            m_AllReadings[3] = new List<LCMeasurement>();

        	picTarget1Pixels.Image = new Bitmap(34, 34);
			picTarget2Pixels.Image = new Bitmap(34, 34);
			picTarget3Pixels.Image = new Bitmap(34, 34);
			picTarget4Pixels.Image = new Bitmap(34, 34);
			picTarget1PSF.Image = new Bitmap(34, 34);
			picTarget2PSF.Image = new Bitmap(34, 34);
			picTarget3PSF.Image = new Bitmap(34, 34);
			picTarget4PSF.Image = new Bitmap(34, 34);

        	miIncludeObject.Image = new Bitmap(15, 15);
            m_SmallGraph = null;

            pnlChart.Paint += new PaintEventHandler(pnlChart_Paint);
            pnlSmallGraph.Paint += new PaintEventHandler(pnlSmallGraph_Paint);

            Text = "Light Curves";

            Top = 100; Left = 100; // Default values for the very first time

            PositionMemento.LoadControlPosition(this);

			miAddins.Visible = false;
			miAddins.DropDownItems.Clear();
        }

		internal frmLightCurve(LightCurveController controller, AddinsController addinsController, LCFile lcFile, string lcFilePath)
			: this(controller, addinsController)
        {
			m_LCFile = lcFile;
        	m_LCFilePath = lcFilePath;
            m_Header = lcFile.Header;
			m_Header.LcFile = lcFile;
            m_Footer = lcFile.Footer;
			m_FrameTiming = lcFile.FrameTiming;

            for (int i = 0; i < m_Header.ObjectCount; i++)
                m_AllReadings[i] = lcFile.Data[i];

            Text = "Light Curves - " + Path.GetFileName(lcFilePath);

            m_Context = new LightCurveContext(lcFile);
            m_Context.NormMethod = LightCurveContext.NormalisationMethod.Average4Frame;

            OnNewLCFile();
        }


        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);

            try
            {
                if (m_frmZoomedPixels != null) m_frmZoomedPixels.Dispose();
                if (m_frmPSFFits != null) m_frmPSFFits.Dispose();
                if (m_frmBackgroundHistograms != null) m_frmBackgroundHistograms.Dispose();
            }
            catch(Exception ex)
            {
                Trace.WriteLine(ex);
            }
            m_LCFile = null;

	        m_LightCurveController.OnLightCurveClosed();
        }

		public TangraConfig.LightCurvesDisplaySettings FormDisplaySettings
		{
			get { return m_DisplaySettings; }
		}

		private void UpdateFormTitle()
		{
			Text = string.Format(
				  "{0} - {1}, {2}{3}{4}",
				  m_LCFilePath != null ? Path.GetFileName(m_LCFilePath) : "Light Curve",
				  ExplainSignalMethod(), ExplainBackgroundMethod(), ExplainDigitalFilter(), ExplainGamma()); 
		}

		private string ExplainSignalMethod()
		{
			switch (m_Context.SignalMethod)
			{
				case TangraConfig.PhotometryReductionMethod.AperturePhotometry:
					return "Aperture Photometry";

				case TangraConfig.PhotometryReductionMethod.PsfPhotometry:
					return "PSF Photometry";

				case TangraConfig.PhotometryReductionMethod.OptimalExtraction:
					return "Optimal Extraction Photometry";

				default:
					return "???";
			}
		}

		private string ExplainBackgroundMethod()
		{
			switch (m_Context.BackgroundMethod)
			{
                case TangraConfig.BackgroundMethod.AverageBackground:
					return "Average Background";

                case TangraConfig.BackgroundMethod.BackgroundMode:
					return "Background Mode";

				case TangraConfig.BackgroundMethod.PSFBackground:
					return "PSF Background";

                case TangraConfig.BackgroundMethod.BackgroundMedian:
                    return "Background Median";

				default:
					return "???";
			}
		}

		private string ExplainDigitalFilter()
		{
			switch (m_Context.Filter)
			{
				case LightCurveContext.FilterType.NoFilter:
					return "";

				case LightCurveContext.FilterType.LowPass:
					return ", LP Filter";

				case LightCurveContext.FilterType.LowPassDifference:
					return ", LPD Filter";

				default:
					return "???";
			}
		}

		private string ExplainGamma()
		{
			if (m_Context.EncodingGamma == 1)
				return "";
			else
				return string.Format(", Gamma = {0}", m_Context.EncodingGamma.ToString("0.00"));
		}

		internal void SetNewLcFile(LCFile lcFile)
		{
			m_AllReadings = lcFile.Data;
			m_LCFile = lcFile;
			m_Header = lcFile.Header;
			m_Header.LcFile = lcFile;
			m_Footer = lcFile.Footer;
			m_FrameTiming = lcFile.FrameTiming;

			m_Context = new LightCurveContext(lcFile);
			
			OnNewLCFile();

			m_IsFirstDraw = true;
			pnlChart.Invalidate();
		}

		internal void SetGeoLocation(GeoLocationInfo geoLocationInfo)
		{
			m_GeoLocationInfo = geoLocationInfo;
			pnlGeoLocation.Visible = geoLocationInfo != null;

			if (geoLocationInfo != null)
				tbxGeoLocation.Text = geoLocationInfo.GetFormattedGeoLocation();
		}
		
        private void OnNewLCFile()
        {
            m_Header.MinAdjustedReading = 0;

            // If there are a lot of measurements choose an appropriate binning value
            // But only when this is a mutual event
            if (m_Footer.ReductionContext.LightCurveReductionType == LightCurveReductionType.MutualEvent)
            {
                if (m_Header.MeasuredFrames > 20000)
                    m_Context.Binning = 32;
                else if (m_Header.MeasuredFrames > 10000)
                    m_Context.Binning = 16;
                else if (m_Header.MeasuredFrames > 5000)
                    m_Context.Binning = 8;
                else if (m_Header.MeasuredFrames > 2500)
                    m_Context.Binning = 4;
                else
                    m_Context.Binning = 0;
            }
            else
                m_Context.Binning = 0;

            m_Context.BackgroundMethod = (TangraConfig.BackgroundMethod)m_Header.BackgroundType;
            m_Context.SignalMethod = m_Footer.ReductionContext.ReductionMethod;
            m_Context.PsfFittingMethod = m_Footer.ProcessedWithTangraConfig.Photometry.PsfFittingMethod;
	        m_Context.PsfQuadratureMethod = m_Footer.ProcessedWithTangraConfig.Photometry.PsfQuadrature;
            m_Context.ManualAverageFWHM = m_Footer.ProcessedWithTangraConfig.Photometry.UseUserSpecifiedFWHM
                                              ? m_Footer.ProcessedWithTangraConfig.Photometry.UserSpecifiedFWHM
                                              : float.NaN;
            m_Context.EncodingGamma = m_Footer.ProcessedWithTangraConfig.Photometry.EncodingGamma;

			m_Context.UseClipping = m_Footer.ReductionContext.UseClipping;
			m_Context.UseStretching = m_Footer.ReductionContext.UseStretching;
			m_Context.UseBrightnessContrast = m_Footer.ReductionContext.UseBrightnessContrast;
			m_Context.FromByte = m_Footer.ReductionContext.FromByte;
			m_Context.ToByte = m_Footer.ReductionContext.ToByte;
			m_Context.Brightness = m_Footer.ReductionContext.Brightness;
			m_Context.Contrast = m_Footer.ReductionContext.Contrast;
			m_Context.BitPix = m_Footer.ReductionContext.BitPix;
        	m_Context.InitFrameBytePreProcessors();
			m_Context.MaxPixelValue = m_Footer.ReductionContext.MaxPixelValue;
            m_Context.DisplayBitmapConverter = m_Footer.ReductionContext.DisplayBitmapConverterImpl;

            m_Context.Filter = LightCurveContext.FilterType.NoFilter;
            if (m_Header.FilterType == 1)
                m_Context.Filter = LightCurveContext.FilterType.LowPass;
            else if (m_Header.FilterType == 2)
                m_Context.Filter = LightCurveContext.FilterType.LowPassDifference;

            m_Context.InstrumentalDelayConfigName = m_Footer.InstrumentalDelayConfigName;
            m_Context.CameraName = m_Footer.CameraName;
            m_Context.AAVFrameIntegration = m_Footer.AAVFrameIntegration;
            m_Context.TimingType = m_Header.TimingType;
            m_Context.MinFrame = m_Header.MinFrame;
            m_Context.MaxFrame = m_Header.MaxFrame;

			ToolStripMenuItem[] allObjMenuItems = new ToolStripMenuItem[] { miIncludeObj1, miIncludeObj2, miIncludeObj3, miIncludeObj4 };

			for (int i = 0; i < 4; i++)
			{
				allObjMenuItems[i].Checked = false;
				allObjMenuItems[i].Visible = false;
				m_IncludeObjects[i] = false;
                DrawColoredRectangleWithCheckBox(allObjMenuItems[i], i);
			}

			for (int i = 0; i < m_Header.ObjectCount; i++)
			{
				allObjMenuItems[i].Checked = true;
				allObjMenuItems[i].Visible = true;
				m_IncludeObjects[i] = true;
				allObjMenuItems[i].Text = string.Format("Obejct {0} ({1})", i + 1, ExplainTrackingType(m_Footer.TrackedObjects[i].TrackingType));
			}

            if (m_Header.ReductionType == LightCurveReductionType.MutualEvent)
                    m_Context.ProcessingType = ProcessingType.SignalOnly;
            else
                m_Context.ProcessingType = ProcessingType.SignalMinusBackground;

            m_MinDisplayedFrame = m_Header.MinFrame;
            m_MaxDisplayedFrame = m_Header.MaxFrame;
            m_ZoomLevel = 1;

            sbZoomStartFrame.Minimum = (int)m_Header.MinFrame;
            sbZoomStartFrame.Maximum = (int)m_Header.MaxFrame;

            m_InitialNoFilterReadings = m_AllReadings;

			if (m_Footer.ReductionContext.FrameIntegratingMode == FrameIntegratingMode.SteppedAverage &&
				m_Footer.ReductionContext.NumberFramesToIntegrate > 1)
			{
				object miPreDefBinning = SetBinning(m_Footer.ReductionContext.NumberFramesToIntegrate);
				if (miPreDefBinning != null)
					BinningMenuItemChecked(miPreDefBinning, EventArgs.Empty);
			}

            // Mark it dirty so the values are computed for the first time
            m_Context.MarkDirtyNoFullReprocessing();

			bool hasEmbeddedTimeStamps = m_Footer.ReductionContext.HasEmbeddedTimeStamps;

            m_CameraCorrectionsHaveBeenAppliedFlag =
                (!string.IsNullOrEmpty(m_Context.InstrumentalDelayConfigName)) ||
                (m_Context.TimingType == MeasurementTimingType.EmbeddedTimeForEachFrame && m_Context.BitPix > 8);

			if (m_Header.SecondTimedFrameTime != DateTime.MinValue || hasEmbeddedTimeStamps)
            {
				if (!hasEmbeddedTimeStamps)
				{
                    string videoSystem;
                    double timeDelta = m_Header.GetAbsoluteTimeDeltaInMilliseconds(out videoSystem);

					m_TimestampDiscrepencyFlag = timeDelta > TangraConfig.Settings.Special.MaxAllowedTimestampShiftInMs;

					if (m_TimestampDiscrepencyFlag)
					{
                        if (videoSystem == null)
						{
                            MessageBox.Show(this,
                                            "This video has an unusual frame rate.\r\n\r\nPlease use the timestamps on the corresponding video frames when timing events from this video.",
                                            "Warning",
                                            MessageBoxButtons.OK, MessageBoxIcon.Warning);

							lblFrameTime.ForeColor = Color.Red;
						}
                        else
                        {
                            MessageBox.Show(this,
                                            string.Format(
                                                "The time derived from entered frame times in this {1} video shows an error higher than {0} ms.\r\n\r\nPlease use the timestamps on the corresponding video frames when timing events from this video."
                                                , timeDelta.ToString("0.0"), videoSystem),
                                            "Warning",
                                            MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            lblFrameTime.ForeColor = Color.Red;
                        }
					}
					else
					{
						lblFrameTime.ForeColor = SystemColors.WindowText;
					}					
				}
				else
				{
					lblFrameTime.ForeColor = Color.DarkGreen;
				}
            }
            else
                lblFrameTime.ForeColor = Color.Red;
            
			UpdateContextDisplays();
			UpdateFormTitle();

			if (m_frmZoomedPixels != null)
			{
				m_frmZoomedPixels.Close();
				m_frmZoomedPixels.Dispose();
			}

			//TODO: Remember the last shown form: PSF or Zoom
        	m_frmZoomedPixels = new frmZoomedPixels(m_Context, m_LCFile, m_DisplaySettings);

            miShowZoomedAreas.Checked = true;
            ShowZoomedAreas();

			m_frmPSFFits = new frmPSFFits(m_Context, m_LCFile, m_DisplaySettings);
			miShowPSFFits.Checked = false;
			HidePSFFits();

        	m_frmBackgroundHistograms = new frmBackgroundHistograms(m_Context, m_LCFile, m_DisplaySettings);
        	miBackgroundHistograms.Checked = false;
        	HideBackgroundHistograms();

			Regex regexAavSourceInfo = new Regex("^Video \\(AAV\\.\\d+\\)$");

	        lblInstDelayWarning.SendToBack();
            lblInstDelayWarning.Visible = 
                // Can determine the frame times
                m_LCFile.CanDetermineFrameTimes &&
                (
				    // AAV file with embedded timestamps (The given time is a central exposure time)
				    (m_Header.TimingType == MeasurementTimingType.EmbeddedTimeForEachFrame && regexAavSourceInfo.IsMatch(m_Header.SourceInfo)) ||
				    // or timestamps read off the screen
				    m_Header.TimingType == MeasurementTimingType.OCRedTimeForEachFrame ||
				    // or user entered star/end times from the VTI OSD
				    m_Header.TimingType == MeasurementTimingType.UserEnteredFrameReferences
                );

			m_AddinsController.SetLightCurveDataProvider(this);

			// If the current .LC file has NTP timestamps then make the menu for exporting them visible
			// NOTE: This is for debugging purposes only!
	        miExportNTPDebugData.Visible =
		        m_LCFile.FrameTiming.Count > 0 &&
		        m_LCFile.FrameTiming[0].FrameMidTimeNTPRaw.HasValue;
        }

		private string ExplainTrackingType(TrackingType type)
		{
			switch(type)
			{
				case TrackingType.GuidingStar:
					return "Guiding Star";

				case TrackingType.OccultedStar:
					return "Occulted Star";

				case TrackingType.ComparisonStar:
					return "Comparison Star";

				default:
					return "???";
			}
		}

        private void pnlChart_Paint(object sender, PaintEventArgs e)
        {
            DrawGraph(e.Graphics);
        }

        private void miSave_Click(object sender, EventArgs e)
        {
	        SaveLCFile();
        }

		public bool EnsureLCFileSaved()
		{
			if (string.IsNullOrEmpty(m_LCFilePath) || !File.Exists(m_LCFilePath))
				SaveLCFile();

			return 
				!string.IsNullOrEmpty(m_LCFilePath) && File.Exists(m_LCFilePath);
		}

		EventTimesReport m_EventTimesReport;

		public bool PrepareForLightCurveEventTimeExtraction(string addinName)
		{
			m_EventTimesReport = null;

			bool lcFileSaved = EnsureLCFileSaved();
			if (lcFileSaved)
			{
				m_LCFile.Header.LcFile = m_LCFile;

                if (m_Header.TimingType == MeasurementTimingType.EmbeddedTimeForEachFrame &&
                    m_LCFile.Header.HasNonEqualySpacedDataPoints())
				{
					MessageBox.Show(
						this, 
						"Cannot extract event times when the processed light curve has non equally spaced data points.", 
						"Tangra3 - " + addinName, 
						MessageBoxButtons.OK, 
						MessageBoxIcon.Error);

					return false;
				}

				m_EventTimesReport = new EventTimesReport()
				{
                    TangraVersion = string.Format("Tangra v{0}", frmAbout.AssemblyFileVersion),
                    AddinAction = addinName,
					LcFilePath = m_LCFilePath,
					VideoFilePath = m_LCFile.Header.PathToVideoFile,
					SourceInfo = m_LCFile.Header.SourceInfo,
					TimingType = m_LCFile.Header.TimingType.ToString(),
					HasEmbeddedTimeStamps = m_LCFile.Footer.ReductionContext.HasEmbeddedTimeStamps,
					ReductionMethod = m_LCFile.Footer.ReductionContext.ReductionMethod.ToString(),
					NoiseMethod = m_LCFile.Footer.ReductionContext.NoiseMethod.ToString(),
					BitPix = m_LCFile.Footer.ReductionContext.BitPix,					
					AveragedFrameHeight = m_LCFile.Footer.AveragedFrameHeight,
					AveragedFrameWidth = m_LCFile.Footer.AveragedFrameWidth,
				};

				if (m_LCFile.Header.FirstTimedFrameNo == 0 &&
					m_LCFile.Header.LastTimedFrameNo == 0 &&
					m_LCFile.Header.TimingType == MeasurementTimingType.UserEnteredFrameReferences)
				{
					m_EventTimesReport.NoTimeBaseAvailable = true;
				}

			    m_EventTimesReport.TimestampDiscrepencyFlag = m_TimestampDiscrepencyFlag;

				m_EventTimesReport.CameraName = m_LCFile.Footer.CameraName;
				if (string.IsNullOrEmpty(m_EventTimesReport.CameraName))
					m_EventTimesReport.CameraName = m_LCFile.Footer.InstrumentalDelayConfigName;

				m_EventTimesReport.RecordedFromUT = m_LCFile.Header.GetVideoRecordStartTimeUT();
				m_EventTimesReport.RecordedToUT = m_LCFile.Header.GetVideoRecordEndTimeUT();
				m_EventTimesReport.AnalysedFromUT = m_LCFile.Header.GetFirstAnalysedFrameTimeUT();
				m_EventTimesReport.AnalysedToUT = m_LCFile.Header.GetLastAnalysedFrameTimeUT();
				
				m_EventTimesReport.VideoFileFormat = m_LCFile.Header.GetVideoFileFormat();
				m_EventTimesReport.VideoFormat = m_LCFile.Header.GetVideoFormat(m_EventTimesReport.VideoFileFormat);

				double duration;
				string mode;
				m_LCFile.Header.GetExposureModeAndDuration(m_EventTimesReport.VideoFileFormat, out duration, out mode);
				m_EventTimesReport.ExposureDuration = duration;
				m_EventTimesReport.ExposureUnit = mode;

				m_EventTimesReport.InstrumentalDelaysApplied = m_LCFile.Header.GetInstrumentalDelaysApplied(m_EventTimesReport.VideoFileFormat);
			}

			return lcFileSaved;
		}

		public void FinishedLightCurveEventTimeExtraction()
		{
			if (m_EventTimesReport != null &&
                !string.IsNullOrEmpty(m_EventTimesReport.Provider))
			{
				m_EventTimesReport.SaveReport();
				
#if WIN32
				if (TangraConfig.Settings.Generic.OWEventTimesExportMode != TangraConfig.OWExportMode.DontExportEventTimes)
				{
					if (m_EventTimesReport.NoTimeBaseAvailable)
					{
						MessageBox.Show(
							this,
							"The results cannot be provided to OccultWatcher because a time base is not available for this light curve and event times cannot be determined.",
							"Tangra3",
							MessageBoxButtons.OK,
							MessageBoxIcon.Error);
					}
                    else if (m_EventTimesReport.TimestampDiscrepencyFlag)
                    {
                        MessageBox.Show(
                            this,
                            "The results cannot be provided to OccultWatcher because the time base is not reliable.",
                            "Tangra3",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
					else
					{
						bool export = true;
						if (TangraConfig.Settings.Generic.OWEventTimesExportMode == TangraConfig.OWExportMode.AskBeforeExportingEventTimes)
						{
							export = MessageBox.Show(
								string.Format("Would you like to make the results provided by {0} available for Occult Watcher's IOTA Reporting Add-in?", m_EventTimesReport.Provider),
								"Tangra3 - " + m_EventTimesReport.AddinAction,
								MessageBoxButtons.YesNo,
								MessageBoxIcon.Question,
								MessageBoxDefaultButton.Button1) == DialogResult.Yes;
						}

                        if (export)
						    OccultWatcherHelper.NotifyOccultWatcherIfInstalled(m_EventTimesReport, this);
					}
				}
#endif
			}

            m_EventTimesReport = null;
		}

		private void SaveLCFile()
		{
		    m_LightCurveController.ConfigureSaveLcFileDialog(saveFileDialog);

			if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
			{
				Update();

				Cursor = Cursors.WaitCursor;
				try
				{
					LCFile.Save(saveFileDialog.FileName, m_Header, m_AllReadings, m_FrameTiming, m_Footer);
					m_LCFilePath = saveFileDialog.FileName;
					m_LightCurveController.RegisterRecentFile(RecentFileType.LightCurve, saveFileDialog.FileName);
				}
				finally
				{
					Cursor = Cursors.Default;
				}
			}			
		}

        private void pnlChart_MouseClick(object sender, MouseEventArgs e)
        {
            // Find the closest blob
            if (e.Y > m_MinY)
            {
                uint currFrameNo = 0;

                if (m_Header.TimingType == MeasurementTimingType.EmbeddedTimeForEachFrame)
                {
					long currFrameTimestampTicks = (long)Math.Round((e.X - m_MinX) / m_TimestampScaleX) + m_MinDisplayedFrameTimestampTicks;
                    currFrameNo = m_Header.GetFrameNumberForFrameTicksFromFrameTiming(currFrameTimestampTicks);
                }
                else
                    currFrameNo = (uint)Math.Round((e.X - m_MinX) / m_ScaleX + m_MinDisplayedFrame);

                SelectFrame(currFrameNo, true);

                pnlChart.Focus();
            }
        }

        private void SignalMenuItemChecked(object sender, EventArgs e)
        {
            if (sender == miSignalMinusNoise)
            {
                miSignalMinusNoise.Checked = true;
                miSignalOnly.Checked = false;
                miNoiseOnly.Checked = false;
				miSignalDividedByBackground.Checked = false;
				miSignalDividedByNoise.Checked = false;

                m_Context.ProcessingType = ProcessingType.SignalMinusBackground;
                tslblSignalType.Text = "Signal-minus-Background";
            }
            else if (sender == miSignalOnly)
            {
                miSignalMinusNoise.Checked = false;
                miSignalOnly.Checked = true;
                miNoiseOnly.Checked = false;
				miSignalDividedByBackground.Checked = false;
				miSignalDividedByNoise.Checked = false;

                m_Context.ProcessingType = ProcessingType.SignalOnly;
                tslblSignalType.Text = "Signal-Only";
            }
            else if (sender == miNoiseOnly)
            {
                miSignalMinusNoise.Checked = false;
                miSignalOnly.Checked = false;
                miNoiseOnly.Checked = true;
				miSignalDividedByBackground.Checked = false;
				miSignalDividedByNoise.Checked = false;

                m_Context.ProcessingType = ProcessingType.BackgroundOnly;
                tslblSignalType.Text = "Background-Only";
            }
			else if (sender == miSignalDividedByBackground)
			{
				miSignalMinusNoise.Checked = false;
				miSignalOnly.Checked = false;
				miNoiseOnly.Checked = false;
				miSignalDividedByBackground.Checked = true;
				miSignalDividedByNoise.Checked = false;

				m_Context.ProcessingType = ProcessingType.SignalDividedByBackground;
				tslblSignalType.Text = "Signal-divided by-Background ( % )";
			}
			else if (sender == miSignalDividedByNoise)
			{
				miSignalMinusNoise.Checked = false;
				miSignalOnly.Checked = false;
				miNoiseOnly.Checked = false;
				miSignalDividedByBackground.Checked = false;
				miSignalDividedByNoise.Checked = true;

				m_Context.ProcessingType = ProcessingType.SignalDividedByNoise;
				tslblSignalType.Text = "Signal-divided by-Noise ( % )";
			}

            pnlChart.Invalidate();
        }

        private void normalisationToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            miNormalisation1.Visible = m_Header.ObjectCount > 0;
            miNormalisation2.Visible = m_Header.ObjectCount > 1;
            miNormalisation3.Visible = m_Header.ObjectCount > 2;
            miNormalisation4.Visible = m_Header.ObjectCount > 3;

            DrawColoredRectangle(miNormalisation1, 0);
            DrawColoredRectangle(miNormalisation2, 1);
            DrawColoredRectangle(miNormalisation3, 2);
            DrawColoredRectangle(miNormalisation4, 3);

            if (m_Header.ReductionType == LightCurveReductionType.Asteroidal)
            {
                miNormalizationSeparator.Visible = true;
                miNormalizationAllNonOcculted.Visible = true;
            }
            else
            {
                miNormalizationSeparator.Visible = false;
                miNormalizationAllNonOcculted.Visible = false;
            }
        }

		private object SetBinning(int bins)
		{
			if (bins == 0)
			{
				return miNoBinning;
			}
			else if (bins == 4)
			{
				return miBinning4;
			}
			else if (bins == 8)
			{
				return miBinning8;
			}
			else if (bins == 16)
			{
				return miBinning16;
			}
			else if (bins == 32)
			{
				return miBinning32;
			}
			else if (bins == 64)
			{
				return miBinning64;
			}
			else
			{
				miNoBinning.Checked = false;
				miBinning4.Checked = false;
				miBinning8.Checked = false;
				miBinning16.Checked = false;
				miBinning32.Checked = false;
				miBinning64.Checked = false;

				m_Context.CustomBinning = true;
				m_Context.Binning = bins;
				tslblBinning.Text = string.Format("Binning {0} Frames", m_Context.Binning);

				return null;
			}
		}

        private void BinningMenuItemChecked(object sender, EventArgs e)
        {
			// Binning is not supported for light curves with non equaly spaced datapoints.
			if (
				sender != miNoBinning &&
				m_Header.TimingType == MeasurementTimingType.EmbeddedTimeForEachFrame &&
				m_Header.HasNonEqualySpacedDataPoints())
			{
                m_LightCurveController.ShowMessageBox(
					"Binning is not supported for light curves with non equaly spaced datapoints.",
					"Tangra",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);

				return;
			}

			if (sender == miCustomBinning)
			{
				frmCustomBinning frmCustomBinning = new frmCustomBinning();
				frmCustomBinning.nudNumFramesToBin.Value = m_Context.Binning;
				if (frmCustomBinning.ShowDialog(this) == DialogResult.OK)
				{
					int userBins = (int)frmCustomBinning.nudNumFramesToBin.Value;

					sender = SetBinning(userBins);
				}
				else
					return;
			}

            if (sender == miNoBinning)
            {
                miNoBinning.Checked = true;
                miBinning4.Checked = false;
                miBinning8.Checked = false;
                miBinning16.Checked = false;
                miBinning32.Checked = false;
                miBinning64.Checked = false;

				m_Context.CustomBinning = false;
                m_Context.Binning = 0;
                tslblBinning.Text = "No Binning";
            }
            else if (sender == miBinning4)
            {
                miNoBinning.Checked = false;
                miBinning4.Checked = true;
                miBinning8.Checked = false;
                miBinning16.Checked = false;
                miBinning32.Checked = false;
                miBinning64.Checked = false;

				m_Context.CustomBinning = false;
                m_Context.Binning = 4;
                tslblBinning.Text = "Binning 4 Frames";
            }
            else if (sender == miBinning8)
            {
                miNoBinning.Checked = false;
                miBinning4.Checked = false;
                miBinning8.Checked = true;
                miBinning16.Checked = false;
                miBinning32.Checked = false;
                miBinning64.Checked = false;

				m_Context.CustomBinning = false;
                m_Context.Binning = 8;
                tslblBinning.Text = "Binning 8 Frames";
            }
            else if (sender == miBinning16)
            {
                miNoBinning.Checked = false;
                miBinning4.Checked = false;
                miBinning8.Checked = false;
                miBinning16.Checked = true;
                miBinning32.Checked = false;
                miBinning64.Checked = false;

				m_Context.CustomBinning = false;
                m_Context.Binning = 16;
                tslblBinning.Text = "Binning 16 Frames";
            }
            else if (sender == miBinning32)
            {
                miNoBinning.Checked = false;
                miBinning4.Checked = false;
                miBinning8.Checked = false;
                miBinning16.Checked = false;
                miBinning32.Checked = true;
                miBinning64.Checked = false;

				m_Context.CustomBinning = false;
                m_Context.Binning = 32;
                tslblBinning.Text = "Binning 32 Frames";
            }
            else if (sender == miBinning64)
            {
                miNoBinning.Checked = false;
                miBinning4.Checked = false;
                miBinning8.Checked = false;
                miBinning16.Checked = false;
                miBinning32.Checked = false;
                miBinning64.Checked = true;

				m_Context.CustomBinning = false;
                m_Context.Binning = 64;
                tslblBinning.Text = "Binning 64 Frames";
            }

            pnlChart.Invalidate();
        }

        private void NormalizationMenuItemChecked(object sender, EventArgs e)
        {
            // Remove the checkbox from the sub menu items
            DrawColoredRectangle(miNormalisation1, 0, true);
            DrawColoredRectangle(miNormalisation2, 1, true);
            DrawColoredRectangle(miNormalisation3, 2, true);
            DrawColoredRectangle(miNormalisation4, 3, true);

            if (sender == miNoNormalization)
            {
                miNoNormalization.Checked = true;
                miNormalisation1.Checked = false;
                miNormalisation2.Checked = false;
                miNormalisation3.Checked = false;
                miNormalisation4.Checked = false;

                m_Context.Normalisation = -1;
                tslblNormalisation.Text = "No Normalisation";
                tslblNormalisation.Image = null;
            }
            else if (sender == miNormalisation1)
            {
                miNoNormalization.Checked = false;
                miNormalisation1.Checked = true;
                miNormalisation2.Checked = false;
                miNormalisation3.Checked = false;
                miNormalisation4.Checked = false;

                m_Context.Normalisation = 0;
                tslblNormalisation.Text = "Normalised";
                DrawColoredRectangle(tslblNormalisation, 0, true);
                DrawColoredRectangleWithCheckBox(miNormalisation1, 0);
            }
            else if (sender == miNormalisation2)
            {
                miNoNormalization.Checked = false;
                miNormalisation1.Checked = false;
                miNormalisation2.Checked = true;
                miNormalisation3.Checked = false;
                miNormalisation4.Checked = false;

                m_Context.Normalisation = 1;
                tslblNormalisation.Text = "Normalised";
                DrawColoredRectangle(tslblNormalisation, 1, true);
                DrawColoredRectangleWithCheckBox(miNormalisation2, 1);
            }
            else if (sender == miNormalisation3)
            {
                miNoNormalization.Checked = false;
                miNormalisation1.Checked = false;
                miNormalisation2.Checked = false;
                miNormalisation3.Checked = true;
                miNormalisation4.Checked = false;

                m_Context.Normalisation = 2;
                tslblNormalisation.Text = "Normalised";
                DrawColoredRectangle(tslblNormalisation, 2, true);
                DrawColoredRectangleWithCheckBox(miNormalisation3, 2);
            }
            else if (sender == miNormalisation4)
            {
                miNoNormalization.Checked = false;
                miNormalisation1.Checked = false;
                miNormalisation2.Checked = false;
                miNormalisation3.Checked = false;
                miNormalisation4.Checked = true;

                m_Context.Normalisation = 3;                
                tslblNormalisation.Text = "Normalised";
                DrawColoredRectangle(tslblNormalisation, 3, true);
                DrawColoredRectangleWithCheckBox(miNormalisation4, 3);
            }

            statusStrip1.Refresh();
            pnlChart.Invalidate();
        }

        private bool ReprocessAllSeries()
        {
            frmReprocessSeries frm = new frmReprocessSeries();
            frm.Header = m_Header;
            frm.Footer = m_Footer;
            frm.Context = m_Context;
            frm.AllReadings = m_InitialNoFilterReadings;
            frm.AllColor = m_DisplaySettings.TargetColors;

            if (frm.ShowDialog(this) == DialogResult.Cancel)
            {
                return false;
            }

            m_AllReadings = frm.AllReadings;
            
            UpdateFormTitle();

            return true;
        }

        private static List<int> s_QuickBinnings = new List<int>(new int[] {4, 8, 16, 32});

        private void UpdateContextDisplays()
        {
            if (m_Context.SignalMethod == TangraConfig.PhotometryReductionMethod.PsfPhotometry && 
				m_Context.PsfQuadratureMethod == TangraConfig.PsfQuadrature.Analytical)
            {
                m_Context.ProcessingType = ProcessingType.SignalMinusBackground;
                miSignalOnly.Visible = false;
                miNoiseOnly.Visible = false;
            }
            else
            {
                miSignalOnly.Visible = true;
                miNoiseOnly.Visible = true;
            }

            #region DisplayedValueType
            miSignalOnly.Checked = false;
            miSignalMinusNoise.Checked = false;
            miNoiseOnly.Checked = false;
            switch (m_Context.ProcessingType)
            {
                case ProcessingType.SignalOnly:
                    miSignalOnly.Checked = true;
                    tslblSignalType.Text = "Signal-Only";
                    break;

                case ProcessingType.SignalMinusBackground:
                    miSignalMinusNoise.Checked = true;
                    tslblSignalType.Text = "Signal-minus-Background";
                    break;

                case ProcessingType.BackgroundOnly:
                    miNoiseOnly.Checked = true;
                    tslblSignalType.Text = "Background-Only";
                    break;

				case ProcessingType.SignalDividedByBackground:
					miSignalDividedByBackground.Checked = true;
					tslblSignalType.Text = "Signal-divided by-Background ( % )";
					break;

				case ProcessingType.SignalDividedByNoise:
            		miSignalDividedByNoise.Checked = true;
            		tslblSignalType.Text = "Signal-divided by-Noise ( % )";
            		break;
            }
            #endregion

            #region Normalisation
            miNoNormalization.Checked = false;
            miNormalisation1.Checked = false;
            miNormalisation2.Checked = false;
            miNormalisation3.Checked = false;
            miNormalisation4.Checked = false;

            switch (m_Context.Normalisation)
            {
                case -1:
                    miNoNormalization.Checked = true;
                    tslblNormalisation.Text = "No Normalisation";
                    tslblNormalisation.Image = null;
                    break;
                case 0:
                    miNormalisation1.Checked = true;
                    tslblNormalisation.Text = "Normalised";
                    DrawColoredRectangle(tslblNormalisation, 0, true);
                    break;
                case 1:
                    miNormalisation2.Checked = true;
                    tslblNormalisation.Text = "Normalised";
                    DrawColoredRectangle(tslblNormalisation, 1, true);
                    break;
                case 2:
                    miNormalisation3.Checked = true;
                    tslblNormalisation.Text = "Normalised";
                    DrawColoredRectangle(tslblNormalisation, 2, true);
                    break;
                case 3:
                    miNormalisation4.Checked = true;
                    tslblNormalisation.Text = "Normalised";
                    DrawColoredRectangle(tslblNormalisation, 3, true);
                    break;
            }

            miNormalisation16DataPoints.Enabled = m_Context.Normalisation >= 0;
            miNormalisation4DataPoints.Enabled = m_Context.Normalisation >= 0;
            miNormalisation1DataPoints.Enabled = m_Context.Normalisation >= 0;
            miNormalisationLinearFit.Enabled = m_Context.Normalisation >= 0;
            #endregion

            #region Binning
            if (m_Context.Binning == 0)
                tslblBinning.Text = "No Binning";
            else
                tslblBinning.Text = string.Format("Binning {0} Frames", m_Context.Binning);

            miNoBinning.Checked = false;
            miBinning4.Checked = false;
            miBinning8.Checked = false;
            miBinning16.Checked = false;
            miBinning32.Checked = false;
            miBinning64.Checked = false;


            if (s_QuickBinnings.IndexOf(m_Context.Binning) != -1)
            {
                miNoBinning.Checked = false;
                switch(m_Context.Binning)
                {
                    case 4:
                        miBinning4.Checked = true;
                        break;
                    case 8:
                        miBinning8.Checked = true;
                        break;
                    case 16:
                        miBinning16.Checked = true;
                        break;
                    case 32:
                        miBinning32.Checked = true;
                        break;
                    case 64:
                        miBinning64.Checked = true;
                        break;
                    default:
                        miNoBinning.Checked = true;
                        break;
                }
            }
            #endregion

			#region Included Graphs
			ToolStripMenuItem[] menuItems = new ToolStripMenuItem[]
        	                                	{miIncludeObj1, miIncludeObj2, miIncludeObj3, miIncludeObj4};
        	int numChecked = 0;
			for (int i = 0; i < 4; i++)
        	{
        		menuItems[i].Checked = m_IncludeObjects[i];
				if (m_IncludeObjects[i]) numChecked++;
                DrawColoredRectangleWithCheckBox(menuItems[i], i);
			}

        	DrawIncludedObjectImage(miIncludeObject.Image, numChecked);
        	statusStrip1.Refresh();
			#endregion
		}

		private void DrawIncludedObjectImage(Image img, int numObjects)
		{
			using(Graphics g = Graphics.FromImage(img))
			{
				try
				{
					if (numObjects == 1)
					{
						for (int i = 0; i < 4; i++)
						{
							if (m_IncludeObjects[i])
							{
                                g.Clear(m_DisplaySettings.TargetColors[i]);
								g.Save();
								return;
							}
						}
					}
					else if (numObjects == 2)
					{
						int objDrawn = 0;
						for (int i = 0; i < 4; i++)
						{
							if (m_IncludeObjects[i])
							{
								if (objDrawn == 0)
								{
                                    g.FillRectangle(m_DisplaySettings.TargetBrushes[i], 0, 0, 7, 15);
									objDrawn++;
								}
								else if (objDrawn == 1)
								{
                                    g.FillRectangle(m_DisplaySettings.TargetBrushes[i], 7, 0, 7, 15);
                                    g.DrawLine(Pens.Black, 7, 0, 7, 15);
									g.Save();
									return;
								}
							}
						}
					}
					else if (numObjects == 3)
					{
						int objDrawn = 0;
						for (int i = 0; i < 4; i++)
						{
							if (m_IncludeObjects[i])
							{
								if (objDrawn == 0)
								{
                                    g.FillPolygon(m_DisplaySettings.TargetBrushes[i], new Point[] { new Point(0, 3), new Point(7, 7), new Point(15, 3), new Point(15, 0), new Point(0, 0), new Point(0, 3) });
									objDrawn++;
								}
								else if (objDrawn == 1)
								{
                                    g.FillPolygon(m_DisplaySettings.TargetBrushes[i], new Point[] { new Point(0, 3), new Point(7, 7), new Point(7, 15), new Point(0, 15), new Point(0, 3) });
									objDrawn++;
								}
								else if (objDrawn == 2)
								{
                                    g.FillPolygon(m_DisplaySettings.TargetBrushes[i], new Point[] { new Point(15, 3), new Point(7, 7), new Point(7, 15), new Point(15, 15), new Point(15, 3) });
                                    g.DrawLine(Pens.Black, 0, 3, 7, 7);
                                    g.DrawLine(Pens.Black, 15, 3, 7, 7);
                                    g.DrawLine(Pens.Black, 7, 15, 7, 7);
									g.Save();
									return;
								}
							}
						}
					}
					else if (numObjects == 4)
					{
						int objDrawn = 0;
						for (int i = 0; i < 4; i++)
						{
							if (m_IncludeObjects[i])
							{
								if (objDrawn == 0)
								{
                                    g.FillRectangle(m_DisplaySettings.TargetBrushes[i], 0, 0, 7, 7);
									objDrawn++;
								}
								else if (objDrawn == 1)
								{
                                    g.FillRectangle(m_DisplaySettings.TargetBrushes[i], 7, 0, 7, 7);
									objDrawn++;
								}
								else if (objDrawn == 2)
								{
                                    g.FillRectangle(m_DisplaySettings.TargetBrushes[i], 0, 7, 7, 7);
									objDrawn++;
								}
								else if (objDrawn == 3)
								{
                                    g.FillRectangle(m_DisplaySettings.TargetBrushes[i], 7, 7, 7, 7);
                                    g.DrawLine(Pens.Black, 7, 0, 7, 15);
                                    g.DrawLine(Pens.Black, 0, 7, 15, 7);
									g.Save();
									return;
								}
							}
						}
					}
				}
				finally
				{
					g.DrawRectangle(Pens.Black, 0, 0, 14, 14);
				}
			}
		}

        private void miReprocess_Click(object sender, EventArgs e)
		{
			#region Integration/Pre-Processing warnings
			if (m_Footer.ReductionContext.UseBrightnessContrast || 
				m_Footer.ReductionContext.UseClipping ||
				m_Footer.ReductionContext.UseStretching)
			{
				if (m_Footer.ReductionContext.FrameIntegratingMode != FrameIntegratingMode.NoIntegration)
				{
					if (MessageBox.Show(
										"Do you wish to reprocess the original integrated data?\r\n\r\n" +
										"During the original reduction of this video an integration and a pre-processing have been used. If you continue with the 'Quick Reprocess' the original integrated frames will be measured which will leave out any stretching, clipping or brightness/contrast adjustments made during the initial reduction. The integration cannot be removed and you will have to re-measure the original video if you want different integration settings.\r\n\r\n" +
										"To apply pre-processing for this reduction you will need to use the 'Full Re-Process' menu.", "Tangra", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
					{
						return;
					}					
				}
				else
				{
					if (MessageBox.Show(
										"Do you wish to reprocess the original raw data?\r\n\r\n" +
										"During the original reduction of this video a pre-processing has been used. If you continue with the 'Quick Reprocess' the original frames will be measured which will leave out any stretching, clipping or brightness/contrast adjustments made during the initial reduction.\r\n\r\n" +
										"To apply pre-processing for this reduction you will need to use the 'Full Re-Process' menu.", "Tangra", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
					{
						return;
					}					
				}
			}
			else
			{
				if (m_Footer.ReductionContext.FrameIntegratingMode != FrameIntegratingMode.NoIntegration)
				{
					if (MessageBox.Show(
										"Do you wish to reprocess the original integrated data?\r\n\r\n" +
										"During the original reduction of this video an integration has been used. If you continue with the 'Quick Reprocess' the integrated frames will be measured. The integration cannot be removed and you will have to re-measure the original video if you want different integration settings.\r\n\r\n" +
										"To apply pre-processing for this reduction you will need to use the 'Full Re-Process' menu.", "Tangra", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
					{
						return;
					}
				}
			}
			#endregion

			frmConfigureReprocessing frmConfigureReprocessing = new frmConfigureReprocessing(
                m_Header, 
				m_Footer,
                m_Context, 
                m_InitialNoFilterReadings,
                m_DisplaySettings.TargetColors,
                m_DisplaySettings.TargetBrushes,
                m_DisplaySettings.TargetPens);

            m_Context.PrepareForCancelling();

            if (m_AllReadings[0][0].PsfFit == null)
                // If there are PSF fits, because we have just opened the file, we need to reprocess regardless of the settings
                m_Context.MarkDirtyWithFullReprocessing();

            if (frmConfigureReprocessing.ShowDialog(this) == DialogResult.OK)
            {
                if (m_Context.RequiresFullReprocessing)
                {
                    if (!ReprocessAllSeries())
                    {
                        m_Context.CancelChanges();
                        return;
                    }
                }

            	UpdateFormTitle();

                m_Context.MarkDirtyNoFullReprocessing();
                pnlChart.Invalidate();
            }
            else
                m_Context.CancelChanges();
        }

        private bool m_TimestampDiscrepencyFlag = false;

	    private bool m_CameraCorrectionsHaveBeenAppliedFlag = false;

		private void HandleIncludeExcludeObject(object sender, EventArgs e)
		{
			ToolStripMenuItem[] allObjMenuItems = new ToolStripMenuItem[] { miIncludeObj1, miIncludeObj2, miIncludeObj3, miIncludeObj4 };
			PictureBox[] targetBoxes = new PictureBox[] { picTarget1Pixels, picTarget2Pixels, picTarget3Pixels, picTarget4Pixels };
			PictureBox[] psfBoxes = new PictureBox[] { picTarget1PSF, picTarget2PSF, picTarget3PSF, picTarget4PSF };
			Label[] msrmntLabels = new Label[] {lblMeasurement1, lblMeasurement2, lblMeasurement3, lblMeasurement4};
			Label[] snlblLabels = new Label[] { lblSNLBL1, lblSNLBL1, lblSNLBL1, lblSNLBL1 };
			Label[] snLabels = new Label[] { lblSN1, lblSN1, lblSN1, lblSN1 };
			

			int numCheckedBefore = 0;
			foreach (ToolStripMenuItem mi in allObjMenuItems)
				if (mi.Checked) numCheckedBefore++;

			ToolStripMenuItem currMi = sender as ToolStripMenuItem;
			if (currMi != null)
			{
				currMi.Checked = !currMi.Checked;

				int numCheckedAfter = 0;
                for(int i =0; i< 4; i++)
                {
                    ToolStripMenuItem mi = allObjMenuItems[i];
                    if (mi.Checked) numCheckedAfter++;
                }

			    if (numCheckedAfter == 0)
				{
					// We don't allow to have no checked items
					currMi.Checked = !currMi.Checked;
					return;
				}

				for (int i = 0; i < 4; i++)
				{
					m_IncludeObjects[i] = allObjMenuItems[i].Checked;

					targetBoxes[i].Visible = m_IncludeObjects[i];
					psfBoxes[i].Visible = m_IncludeObjects[i];
					msrmntLabels[i].Visible = m_IncludeObjects[i];
					snlblLabels[i].Visible = m_IncludeObjects[i];
					snLabels[i].Visible = m_IncludeObjects[i];

                    DrawColoredRectangleWithCheckBox(allObjMenuItems[i], i);
				}

				m_Context.MarkDirtyNoFullReprocessing();
				pnlChart.Invalidate();
			}
		}

        internal void CloseFormDontSendMessage()
        {
            m_NoSendMessage = true;
            Close();
        }

        private bool m_NoSendMessage = false;

        private void frmLightCurve_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!m_NoSendMessage)
                NotificationManager.Instance.NotifyLightCurveFormClosed();

            HideZoomedAreas();
            if (m_frmZoomedPixels != null)
            {
                m_frmZoomedPixels.Close();
                m_frmZoomedPixels.Dispose();
                m_frmZoomedPixels = null;
            }

        	HidePSFFits();
			if (m_frmPSFFits != null)
            {
				m_frmPSFFits.Close();
				m_frmPSFFits.Dispose();
				m_frmPSFFits = null;
            }

        	HideBackgroundHistograms();
			if (m_frmBackgroundHistograms != null)
			{
				m_frmBackgroundHistograms.Close();
				m_frmBackgroundHistograms.Dispose();
				m_frmBackgroundHistograms = null;
			}
        }

		private void frmLightCurve_Resize(object sender, EventArgs e)
		{
            if (m_Context != null) m_Context.MarkDirtyNoFullReprocessing();
		    pnlChart.Invalidate();
		}

        private void miLoad_Click(object sender, EventArgs e)
        {
            m_LightCurveController.LoadLightCurve();
        }

        private void miSaveAsImage_Click(object sender, EventArgs e)
        {
			if (m_Graph == null) return;
            Bitmap bmp = AddImageLegend();

			if (saveImageDialog.ShowDialog(this) == DialogResult.OK)
			{
                bmp.Save(
					saveImageDialog.FileName,
					GetImageFormatFromFileExtension(Path.GetExtension(saveImageDialog.FileName)));
			}
        }

        private ImageFormat GetImageFormatFromFileExtension(string fileExt)
        {
            fileExt = fileExt.ToLower();

            if (fileExt == ".jpg")
                return ImageFormat.Jpeg;
            if (fileExt == ".png")
                return ImageFormat.Png;

            return ImageFormat.Bmp;
        }

		private Bitmap AddImageLegend()
		{
			if (m_Graph == null) return null;
			int logoWidth = Properties.Resources.lc_logo.Width;

			Bitmap export = new Bitmap(m_Graph.Width, m_Graph.Height + logoWidth + 12);

			using (Graphics g = Graphics.FromImage(export))
			{
				g.Clear(m_DisplaySettings.BackgroundColor);
				g.DrawImage(m_Graph, new Point(0, 0));
				g.DrawImage(Properties.Resources.lc_logo, new Point(m_MinX - logoWidth - 6, m_MinY - logoWidth - 6));

				string legend = string.Format(
					  "{0} - {1}, {2}{3}{4}",
					  m_LCFilePath != null ? Path.GetFileName(m_LCFilePath) : "Light Curve",
					  ExplainSignalMethod(), ExplainBackgroundMethod(), ExplainDigitalFilter(), ExplainGamma());

				SizeF labelSize = g.MeasureString(legend, s_AxisFont);
				float x = m_MinX;
				float y = m_MinY - 6 - labelSize.Height;
				g.DrawString(legend, s_AxisFont, m_DisplaySettings.LabelsBrush, x, y);

				uint interval = GetXAxisInterval(g);
				uint firstMark = interval * (1 + m_MinDisplayedFrame / interval);
				string isCorrectedForInstrumentalDelay;
				DateTime firstTime = m_LCFile.GetTimeForFrame(firstMark, out isCorrectedForInstrumentalDelay);
				double frameDuration = m_LCFile.Header.SecondsPerFrameComputed();
				string frameDurStr = string.Empty;
				if (!double.IsNaN(frameDuration))
					frameDurStr = string.Format(", 1 frame = {0} sec", frameDuration.ToString("0.000"));

                if (firstTime != DateTime.MaxValue)
				    legend = string.Format("{0} = {1} UT{2}", firstMark, firstTime.ToString("dd MMM yyyy, HH:mm:ss.fff"), frameDurStr);
                else
                    legend = string.Format("No frame times entered{0} ", frameDurStr);

				labelSize = g.MeasureString(firstMark.ToString(), s_AxisFont);
				x = m_MinX + (firstMark - m_MinDisplayedFrame) * m_ScaleX - labelSize.Width / 2;
				y = m_MaxY + 12 + labelSize.Height;
				
				g.DrawString(legend, s_AxisFont, m_DisplaySettings.LabelsBrush, x, y);

				g.Save();
			}				
			

			return export;
		}

        private void miCopyToClipboard_Click(object sender, EventArgs e)
        {
			if (m_Graph == null) return;

        	Bitmap bmp = AddImageLegend();

			if (bmp != null)
			{
				Clipboard.SetImage(bmp);
				SystemSounds.Exclamation.Play();
				bmp.Dispose();
			}
        }

		private void miExportTangraCSV_Click(object sender, EventArgs e)
		{
            if (!string.IsNullOrEmpty(m_LCFilePath) && File.Exists(m_LCFilePath))
            {
                saveCSVDialog.InitialDirectory = Path.GetDirectoryName(m_LCFilePath);
                saveCSVDialog.FileName = Path.ChangeExtension(Path.GetFileName(m_LCFilePath), ".csv");
            }

			if (saveCSVDialog.ShowDialog() == DialogResult.OK)
			{
				if (ExportToCSV(saveCSVDialog.FileName))
					Process.Start(saveCSVDialog.FileName);
			}
        }

		private void btnZoomOut_Click(object sender, EventArgs e)
		{
            m_ZoomLevel--;

            SetFirstFrameSuchThatSelectedFrameIsInTheMiddleForCurrentZoomLevel();

            m_Context.MarkFirstZoomedFrameChanged();
            pnlChart.Invalidate();

            m_ZoomScrollMode = true;
            pnlSmallGraph.Invalidate();
		}

		private void btnZoomIn_Click(object sender, EventArgs e)
		{
            m_NoChangeAfterZoom =
                m_ZoomLevel == 1 &&
                m_Context.SelectedFrameNo >= m_Header.MinFrame &&
                m_Context.SelectedFrameNo <= m_Header.MaxFrame;

		    m_ZoomLevel++;

            SetFirstFrameSuchThatSelectedFrameIsInTheMiddleForCurrentZoomLevel();

            m_Context.MarkFirstZoomedFrameChanged();
            pnlChart.Invalidate();

            m_ZoomScrollMode = true;
            pnlSmallGraph.Invalidate();
		}

        private bool m_NoChangeAfterZoom = false;

        private void SetFirstFrameSuchThatSelectedFrameIsInTheMiddleForCurrentZoomLevel()
        {
            int framesInTheWindows = (int) (m_Header.MeasuredFrames/m_ZoomLevel);
            int focusedFrameId;
            if (m_NoChangeAfterZoom &&
                m_Context.SelectedFrameNo >= m_Header.MinFrame &&
                m_Context.SelectedFrameNo <= m_Header.MaxFrame)
            {
                m_NewMinDisplayedFrame = Math.Max((int) (m_Header.MinFrame),
                                                  (int) (m_Context.SelectedFrameNo - (framesInTheWindows/2)));
                focusedFrameId = (int)m_Context.SelectedFrameNo;
            }
            else
            {
                // Use the frame in the middle
                focusedFrameId = (int)(m_MinDisplayedFrame + m_MaxDisplayedFrame) / 2;
            
                m_NewMinDisplayedFrame = Math.Max((int) (m_Header.MinFrame),
                                                  (int)(focusedFrameId - (framesInTheWindows / 2)));
            }

            SetScrollBarPostionNoScroll(focusedFrameId);
            m_NoChangeAfterZoom = false;
        }

        private void SetScrollBarPostionNoScroll(int newPosition)
        {
            sbZoomStartFrame.ValueChanged -= hScrollBar1_ValueChanged;
            try
            {

                sbZoomStartFrame.Value = newPosition;
            }
            finally
            {
                sbZoomStartFrame.ValueChanged += hScrollBar1_ValueChanged;
            }
        }
		private void hScrollBar1_ValueChanged(object sender, EventArgs e)
		{
            firstFrameTimer.Enabled = false;
            firstFrameTimer.Enabled = true;
		}

        private void SlideWindowToMiddleFrame(uint middleFrame)
        {
            uint oldMiddleFrame = (m_MaxDisplayedFrame + m_MinDisplayedFrame) / 2;
            uint windowWidth = m_MaxDisplayedFrame - m_MinDisplayedFrame;
            uint newMinDisplayedFrame = (m_MinDisplayedFrame + (middleFrame - oldMiddleFrame));

            // When sliding the window, the MinFrame cannot be smaller than 0 and cannot be larger than Max - WindowWidth
            newMinDisplayedFrame = Math.Max(m_Header.MinFrame, newMinDisplayedFrame);
            newMinDisplayedFrame = Math.Min(newMinDisplayedFrame, m_Header.MaxFrame - windowWidth);

            m_MinDisplayedFrame = newMinDisplayedFrame;
            m_NewMinDisplayedFrame = -1;
            m_ZoomScrollMode = true;

            m_Context.MarkFirstZoomedFrameChanged();
            pnlChart.Invalidate();

            pnlSmallGraph.Invalidate();
        }

        private void firstFrameTimer_Tick(object sender, EventArgs e)
        {
            firstFrameTimer.Enabled = false;

            SlideWindowToMiddleFrame((uint)sbZoomStartFrame.Value);
        }

        private void pnlChart_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                if (m_Context.SelectedFrameNo >= m_MinDisplayedFrame) SelectFrame(m_Context.SelectedFrameNo - 1, true);
            }
            else if (e.KeyCode == Keys.Right)
            {
                if (m_Context.SelectedFrameNo < m_MaxDisplayedFrame) SelectFrame(m_Context.SelectedFrameNo + 1, true);
            }
			else if (e.KeyCode == Keys.Escape)
			{
                NotificationManager.Instance.NotifyUserRequestToChangeCurrentFrame(null);
			    SelectMeasurement(-1);
			}
        }

        internal void SelectFrame(uint frameNo)
        {
            if (frameNo >= m_LCFile.Header.MinFrame && 
                frameNo <= m_LCFile.Header.MaxFrame)
            {
                m_Context.SelectedFrameNo = frameNo;
                SelectFrame(m_Context.SelectedFrameNo, true);                
            }
        }

        #region Small Graph
        private void sbZoomStartFrame_MouseCaptureChanged(object sender, EventArgs e)
        {
            if (sbZoomStartFrame.Capture)
            {
                m_ZoomScrollMode = true;
                pnlSmallGraph.Invalidate();
            }
        }

        private void pnlSmallGraph_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(SystemColors.Control /* This should be the same colour as the panel i.e. SystemColors.Control */);

            if (m_ZoomScrollMode && m_ZoomLevel > 1)
            {
                pnlMeasurementDetails.Visible = false;
                DoDrawSmallGraph(e.Graphics);
            }
        }

        private int m_SmallGraphZoomFromX;
        private int m_SmallGraphZoomToX;
        private float m_SmallGraphXScale;
        private int m_SmallGraphMinX;

        private void DoDrawSmallGraph(Graphics gPanel)
        {
            gPanel.Clear(m_DisplaySettings.BackgroundColor);
 
            int minX = 5;
            int minY = 2;

            int idx = 0;
            float xScale = (float)(pnlSmallGraph.Width - minX - minY) / (float)(m_Header.MaxFrame - m_Header.MinFrame);
            if (m_Header.MaxFrame == m_Header.MinFrame) xScale = 0;

            float yScale = (float)((pnlSmallGraph.Height - 10) - minY * 2) / (float)(m_Header.MaxAdjustedReading - m_Header.MinAdjustedReading);
            if (m_Header.MaxAdjustedReading == m_Header.MinAdjustedReading) yScale = 0;

            float xMin = minX + (m_MinDisplayedFrame - m_Header.MinFrame) * xScale;
            float xWidth = ((m_Header.MaxFrame - m_Header.MinFrame) * 1.0f / m_ZoomLevel) * xScale;

            m_SmallGraphZoomFromX = (int) xMin;
            m_SmallGraphZoomToX = (int)(xMin + xWidth);
            m_SmallGraphXScale = xScale;
            m_SmallGraphMinX = minX;

            gPanel.FillRectangle(m_DisplaySettings.SmallGraphFocusBackgroundBrush, xMin, minY, xWidth, pnlSmallGraph.Height - 2 * minY);


            #region Draw Small Image
            if (m_SmallGraph == null)
            {
                m_SmallGraph = new Bitmap(pnlSmallGraph.Width, pnlSmallGraph.Height);
                using (Graphics g = Graphics.FromImage(m_SmallGraph))
                {
                    for (int i = 0; i < m_Header.ObjectCount; i++)
                    {
                        if (!m_IncludeObjects[i]) continue;

                        PointF prevPoint = PointF.Empty;
                        List<BinnedValue> binnedValues = m_AllBinnedReadings[i];

                        BinnedValue currBin = null;
                        int binnedIdx = -1;
                        int nextPlotIndex = -1;
                        int readingIdx = 0;
                        foreach (LCMeasurement reading in m_AllReadings[i])
                        {
                            idx++;
                            readingIdx++;
                            bool drawThisReading = 
                                (reading.CurrFrameNo >= m_Header.MinFrame) && (reading.CurrFrameNo <= m_Header.MaxFrame) &&
                                reading.IsSuccessfulReading;

                            float adjustedReading = -1;
                            if (m_Context.Binning > 0)
                            {
                                if (currBin == null ||
                                    currBin.ReadingIndexTo <= readingIdx)
                                {
                                    binnedIdx++;
                                    if (binnedIdx < binnedValues.Count)
                                    {
                                        currBin = binnedValues[binnedIdx];
                                        nextPlotIndex = (currBin.ReadingIndexFrom + currBin.ReadingIndexTo) / 2;
                                    }
                                }

                                if (nextPlotIndex == readingIdx &&
                                    currBin != null)
                                {
                                    adjustedReading = (float)currBin.AdjustedValue;
                                    drawThisReading = true;
                                }
                                else
                                    drawThisReading = false;
                            }
                            else
                            {
                                adjustedReading = reading.AdjustedReading;
                            }

                            if (drawThisReading)
                            {
								Pen pen = GetPenForTarget(i, reading.IsSuccessfulReading);

                                float x = minX + ((int)reading.CurrFrameNo - (int)m_Header.MinFrame) * xScale;
                                float y = pnlSmallGraph.Height - (minY + (adjustedReading - (int)m_Header.MinAdjustedReading) * yScale);

                                if (prevPoint != PointF.Empty)
                                {
                                    g.DrawLine(pen, prevPoint.X, prevPoint.Y, x, y);
                                }
                                else
                                    prevPoint = new PointF();

                                prevPoint.X = x;
                                prevPoint.Y = y;

                                //g.FillEllipse(brush, x - 2, y - 2, 5, 5);
                            }

                            if (i == 0 && idx % 250 == 0)
                            {
                                m_FramesIndex.Add((uint)reading.CurrFrameNo);
                                m_MeasurementsIndex.Add((uint)idx);
                            }
                        }
                    }

                    // Draw Selected Measurement
                    if (m_Context.SelectedFrameNo >= m_Header.MinFrame &&
                        m_Context.SelectedFrameNo <= m_Header.MaxFrame)
                    {
                        float x = minX + ((int)m_Context.SelectedFrameNo - (int)m_Header.MinFrame) * xScale;
                        g.DrawLine(m_DisplaySettings.SelectionCursorColorPen, x, minY, x, pnlSmallGraph.Height - minY);
                    }

                    g.Save();
                }
            }
            #endregion

            gPanel.DrawImage(m_SmallGraph, 0, 0);
        }

        private void pnlSmallGraph_Resize(object sender, EventArgs e)
        {
            SetSmallGraphDirty();
            pnlSmallGraph.Invalidate();
            //Trace.WriteLine("Curve:1232");
        }

        private bool m_IsPanning = false;
        private int m_PanningFromCursorAtFrame;
        private int m_PanningFromMinFrame;

        private int m_LastFrameAtCursor = -1;

        private void pnlSmallGraph_MouseMove(object sender, MouseEventArgs e)
        {
            if (!m_IsPanning)
            {
                Cursor newCursor = Cursors.Arrow;

                if (m_ZoomScrollMode && m_ZoomLevel > 1)
                {
                    if (e.X >= m_SmallGraphZoomFromX && e.X <= m_SmallGraphZoomToX)
                        newCursor = CustomCursors.PanCursor;
                    else
                        newCursor = Cursors.Hand;
                }

                if (pnlSmallGraph.Cursor != newCursor)
                    pnlSmallGraph.Cursor = newCursor;
            }
            else
            {
                // We will keep getting MOUSE_MOVE messages even when there is no actual move, so check the coordinate
                uint frameAtCursor = (uint)Math.Round((e.X - m_SmallGraphMinX) / m_SmallGraphXScale + m_Header.MinFrame);

                TryMoveToFrameAtCursor(frameAtCursor);
            }
        }

        private void pnlSmallGraph_MouseDown(object sender, MouseEventArgs e)
        {
            if (m_ZoomScrollMode && m_ZoomLevel > 1)
            {
                if (e.X >= m_SmallGraphZoomFromX && e.X <= m_SmallGraphZoomToX)
                {
                    m_IsPanning = true;
                    m_PanningFromCursorAtFrame = (int)Math.Round((e.X - m_SmallGraphMinX) / m_SmallGraphXScale + m_Header.MinFrame);
                    m_PanningFromMinFrame = sbZoomStartFrame.Value;
                    pnlSmallGraph.Cursor = CustomCursors.PanEnabledCursor;
                }
                else
                {
                    // Move to this location
                    uint frameAtCursor = (uint)Math.Round((e.X - m_SmallGraphMinX) / m_SmallGraphXScale + m_Header.MinFrame);
                    MoveToFrameAndSelectFrame(frameAtCursor);
                }
            }
        }

        private void MoveToFrameAndSelectFrame(uint frameAtCursor)
        {
            uint halfWindowWidth = m_ZoomScrollMode ? (m_MaxDisplayedFrame - m_MinDisplayedFrame) / 2 : 0;
            frameAtCursor = Math.Max(m_Header.MinFrame + halfWindowWidth, frameAtCursor);
            frameAtCursor = Math.Min(frameAtCursor, m_Header.MaxFrame - halfWindowWidth);

            m_Context.SelectedFrameNo = frameAtCursor;
            if (m_ZoomScrollMode)
                TryMoveToFrameAtCursor(frameAtCursor);

            SetSmallGraphDirty();

            m_NewMinDisplayedFrame = m_ZoomScrollMode ? (int)(frameAtCursor - halfWindowWidth) : -1;
            m_SmallGraph = null;
            m_ZoomScrollMode = false;

            pnlChart.Invalidate();
            pnlSmallGraph.Invalidate();
        }

        private void TryMoveToFrameAtCursor(uint frameAtCursor)
        {
            if (m_LastFrameAtCursor != frameAtCursor)
            {
                m_LastFrameAtCursor = (int) frameAtCursor;

                uint halfWindowWidth = m_ZoomScrollMode ? (m_MaxDisplayedFrame - m_MinDisplayedFrame)/2 : 0;
                frameAtCursor = Math.Max(m_Header.MinFrame + halfWindowWidth, frameAtCursor);
                frameAtCursor = Math.Min(frameAtCursor, m_Header.MaxFrame - halfWindowWidth);

                bool lockTaken = false;
                m_SpinLock.Enter(ref lockTaken);
                try
                {
                    if (frameAtCursor >= sbZoomStartFrame.Minimum &&
                        frameAtCursor <= sbZoomStartFrame.Maximum)
                    {
                        // This is how we move to a frame (unfortunate)
                        SlideWindowToMiddleFrame(frameAtCursor);

                        sbZoomStartFrame.ValueChanged -= hScrollBar1_ValueChanged;
                        try
                        {

                            sbZoomStartFrame.Value = (int)frameAtCursor;
                        }
                        finally
                        {
                            sbZoomStartFrame.ValueChanged += hScrollBar1_ValueChanged;
                        }
                    }
                }
                finally
                {
                    if (lockTaken)
                        m_SpinLock.Exit();
                }
            }
        }

        private void pnlSmallGraph_MouseUp(object sender, MouseEventArgs e)
        {
            m_IsPanning = false;
        }

        private void pnlSmallGraph_MouseLeave(object sender, EventArgs e)
        {
            m_IsPanning = false;
        }
        #endregion

        private void miNoiseDistribution_Click(object sender, EventArgs e)
        {
            frmNoiseDistribution frmNoiseDistribution = new frmNoiseDistribution(
				m_Header, m_AllReadings, m_AllBinnedReadings, m_DisplaySettings.TargetBrushes, m_DisplaySettings.TargetColors, m_DisplaySettings.BackgroundColor);
            frmNoiseDistribution.ShowDialog(this);
        }

        private void miPixelDistribution_Click(object sender, EventArgs e)
        {
            frmPixelDistribution frm;

            Cursor = Cursors.WaitCursor;
            try
            {
                frm = new frmPixelDistribution(m_AllReadings, m_Context.MaxPixelValue, m_Footer.ReductionContext.BitPix);
            }
            finally
            {
                Cursor = Cursors.Default;    
            }
            frm.ShowDialog(this);
        }

        private void miDisplaySettings_Click(object sender, EventArgs e)
        {
            frmLightCurveSettings frm = new frmLightCurveSettings(m_DisplaySettings, this);
            frm.ShowDialog(this);
        }

        public void RedrawPlot()
        {
            m_Context.MarkDirtyNoFullReprocessing();
            pnlChart.Invalidate();
            pnlChart.Refresh();
            Application.DoEvents();
        }

    	private bool m_HandlingZoomedAndPSF = false;

		private void miShowZoomedAreas_Click(object sender, EventArgs e)
		{
			if (!m_HandlingZoomedAndPSF)
			{
				m_HandlingZoomedAndPSF = true;
				try
				{
					if (miShowZoomedAreas.Checked)
					{
						if (miShowPSFFits.Checked)
						{
							miShowPSFFits.Checked = false;
							HidePSFFits();
						}
						else if (miBackgroundHistograms.Checked)
						{
							miBackgroundHistograms.Checked = false;
							HideBackgroundHistograms();
						}

						ShowZoomedAreas();
					}
					else
						HideZoomedAreas();
					
				}
				finally
				{
					m_HandlingZoomedAndPSF = false;
				}				
			}
		}

		private void miShowPSFFits_Click(object sender, EventArgs e)
		{
			if (!m_HandlingZoomedAndPSF)
			{
				m_HandlingZoomedAndPSF = true;
				try
				{
					if (miShowPSFFits.Checked)
					{
						if (miShowZoomedAreas.Checked)
						{
							miShowZoomedAreas.Checked = false;
							HideZoomedAreas();
						}
						else if (miBackgroundHistograms.Checked)
						{
							miBackgroundHistograms.Checked = false;
							HideBackgroundHistograms();
						}

						ShowPSFFits();

					}
					else
						HidePSFFits();

				}
				finally
				{
					m_HandlingZoomedAndPSF = false;
				}
			}
		}

		private void miBackgroundHistograms_Click(object sender, EventArgs e)
		{
			if (!m_HandlingZoomedAndPSF)
			{
				m_HandlingZoomedAndPSF = true;
				try
				{
					if (miBackgroundHistograms.Checked)
					{
						if (miShowZoomedAreas.Checked)
						{
							miShowZoomedAreas.Checked = false;
							HideZoomedAreas();
						}
						else if (miShowPSFFits.Checked)
						{
							miShowPSFFits.Checked = false;
							HidePSFFits();
						}

						ShowBackgroundHistograms();

					}
					else
						HideBackgroundHistograms();

				}
				finally
				{
					m_HandlingZoomedAndPSF = false;
				}
			}
		}

    	private frmZoomedPixels m_frmZoomedPixels;

		private void ShowZoomedAreas()
		{
            if (!m_frmZoomedPixels.Visible)
            {
                m_frmZoomedPixels.Show(this);
                m_frmZoomedPixels.Left = this.Right;
                m_frmZoomedPixels.Top = this.Top;
            }
		}

        internal void HideZoomedAreas()
        {
            if (m_frmZoomedPixels.Visible) m_frmZoomedPixels.Hide();
            miShowZoomedAreas.Checked = false;
        }

		private frmPSFFits m_frmPSFFits;

		private void ShowPSFFits()
		{
			if (!m_frmPSFFits.Visible)
			{
				m_frmPSFFits.Show(this);
				m_frmPSFFits.Left = this.Right;
				m_frmPSFFits.Top = this.Top;
			}
		}

		internal void HidePSFFits()
		{
			if (m_frmPSFFits.Visible) m_frmPSFFits.Hide();
			miShowPSFFits.Checked = false;
		}

    	private frmBackgroundHistograms m_frmBackgroundHistograms;

		private void ShowBackgroundHistograms()
		{
			if (!m_frmBackgroundHistograms.Visible)
			{
				m_frmBackgroundHistograms.Show(this);
				m_frmBackgroundHistograms.Left = this.Right;
				m_frmBackgroundHistograms.Top = this.Top;
			}
		}

		internal void HideBackgroundHistograms()
		{
			if (m_frmBackgroundHistograms.Visible) m_frmBackgroundHistograms.Hide();
			miBackgroundHistograms.Checked = false;
		}

        private void frmLightCurve_Move(object sender, EventArgs e)
        {
            if (m_frmZoomedPixels != null &&
                m_frmZoomedPixels.Visible)
            {
                m_frmZoomedPixels.Left = this.Right;
                m_frmZoomedPixels.Top = this.Top;
            }

            if (m_frmPSFFits != null &&
                m_frmPSFFits.Visible)
            {
                m_frmPSFFits.Left = this.Right;
                m_frmPSFFits.Top = this.Top;
            }

            if (m_frmBackgroundHistograms != null &&
                m_frmBackgroundHistograms.Visible)
            {
                m_frmBackgroundHistograms.Left = this.Right;
                m_frmBackgroundHistograms.Top = this.Top;
            }
        }

		private void miFullReprocess_Click(object sender, EventArgs e)
		{

		}

        private void OnNormalisationMethodChanged(object sender, EventArgs e)
        {
            if (sender == miNormalisation16DataPoints)
            {
                miNormalisation16DataPoints.Checked = true;
                miNormalisation4DataPoints.Checked = false;
                miNormalisation1DataPoints.Checked = false;
                miNormalisationLinearFit.Checked = false;

                m_Context.NormMethod = LightCurveContext.NormalisationMethod.Average16Frame;
            }
            else if (sender == miNormalisation4DataPoints)
            {
                miNormalisation16DataPoints.Checked = false;
                miNormalisation4DataPoints.Checked = true;
                miNormalisation1DataPoints.Checked = false;
                miNormalisationLinearFit.Checked = false;

                m_Context.NormMethod = LightCurveContext.NormalisationMethod.Average4Frame;
            }
            else if (sender == miNormalisation1DataPoints)
            {
                miNormalisation16DataPoints.Checked = false;
                miNormalisation4DataPoints.Checked = false;
                miNormalisation1DataPoints.Checked = true;
                miNormalisationLinearFit.Checked = false;

                m_Context.NormMethod = LightCurveContext.NormalisationMethod.FrameByFrame;
            }
            else if (sender == miNormalisationLinearFit)
            {
                miNormalisation16DataPoints.Checked = false;
                miNormalisation4DataPoints.Checked = false;
                miNormalisation1DataPoints.Checked = false;
                miNormalisationLinearFit.Checked = true;

                m_Context.NormMethod = LightCurveContext.NormalisationMethod.LinearFit;
            }

            statusStrip1.Refresh();
            pnlChart.Invalidate();
        }

        private void frmLightCurve_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (FormWindowState.Normal == WindowState)
            {
                PositionMemento.SaveControlPosition(this);
            }
        }

        private void miAdjustMeasurements_Click(object sender, EventArgs e)
        {
            List<LCFileSeriesEntry> dataList = new List<LCFileSeriesEntry>();

            int upper = m_AllReadings[0].Count;
            for (int j = 0; j < upper; j++)
            {
                uint frameNo = m_AllReadings[0][j].CurrFrameNo;

                LCMeasurement measurement1 = m_Header.ObjectCount > 0 ? m_AllReadings[0][j] : LCMeasurement.Empty;
                LCMeasurement measurement2 = m_Header.ObjectCount > 1 ? m_AllReadings[1][j] : LCMeasurement.Empty;
                LCMeasurement measurement3 = m_Header.ObjectCount > 2 ? m_AllReadings[2][j] : LCMeasurement.Empty;
                LCMeasurement measurement4 = m_Header.ObjectCount > 3 ? m_AllReadings[3][j] : LCMeasurement.Empty;

                LCFileSeriesEntry entry = new LCFileSeriesEntry(m_Header.ObjectCount, frameNo, measurement1, measurement2, measurement3, measurement4);
                dataList.Add(entry);
            }

            frmEditLcDataSeries frm = new frmEditLcDataSeries();
            frm.SetDataSource(dataList);
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                for (int j = 0; j < upper; j++)
                {

                    if (m_Header.ObjectCount > 0) m_AllReadings[0][j] = frm.DataList[j].Measurement1;
                    if (m_Header.ObjectCount > 1) m_AllReadings[1][j] = frm.DataList[j].Measurement2;
                    if (m_Header.ObjectCount > 2) m_AllReadings[2][j] = frm.DataList[j].Measurement3;
                    if (m_Header.ObjectCount > 3) m_AllReadings[3][j] = frm.DataList[j].Measurement4;
                }

                RedrawPlot();
            }

        }

		private void frmLightCurve_Load(object sender, EventArgs e)
		{
			ReloadAddins();
		}

		public void ReloadAddins()
		{
			m_AddinsController.BuildLightCurveMenuAddins(miAddins);
		}

		private void miExportNTPDebugData_Click(object sender, EventArgs e)
		{
			if (m_LCFile != null && m_LCFile.FrameTiming != null && m_LCFile.FrameTiming.Count > 0)
			{
				var output = new StringBuilder();

				var ntpList = new List<double>();
				var ntpFittedList = new List<double>();
				var windowsList = new List<double>();

				output.AppendFormat("\"OCR Time\",\"NTP Raw Time\",\"NTP Fitted Time\",\"Windows Raw Time\",\"OCR-NTPRaw\",\"OCR-NTPFitted\",\"OCR-WindowsRaw\"\r\n");
				foreach (LCFrameTiming entry in m_LCFile.FrameTiming)
				{
					double diffNTP = new TimeSpan(entry.FrameMidTime.Ticks - entry.FrameMidTimeNTPRaw.Value.Ticks).TotalMilliseconds;
					double diffNTPTangra = new TimeSpan(entry.FrameMidTime.Ticks - entry.FrameMidTimeNTPTangra.Value.Ticks).TotalMilliseconds;
					double diffWindows = new TimeSpan(entry.FrameMidTime.Ticks - entry.FrameMidTimeWindowsRaw.Value.Ticks).TotalMilliseconds;

					output.AppendFormat("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\"\r\n",
					    entry.FrameMidTime.ToString("HH:mm:ss.fff"),
						entry.FrameMidTimeNTPRaw.Value.ToString("HH:mm:ss.fff"),
						entry.FrameMidTimeNTPTangra.Value.ToString("HH:mm:ss.fff"),
						entry.FrameMidTimeWindowsRaw.Value.ToString("HH:mm:ss.fff"),
						diffNTP.ToString("0.000"),
						diffNTPTangra.ToString("0.000"),
						diffWindows.ToString("0.000"));

					ntpList.Add(Math.Abs(diffNTP));
					ntpFittedList.Add(Math.Abs(diffNTPTangra));
					windowsList.Add(Math.Abs(diffWindows));
				}

				double averageNtpDiff = ntpList.Average();
				double averageNtpFittedDiff = ntpFittedList.Average();
				double averageWindowsDiff = windowsList.Average();

				double ntpOneSigma = Math.Sqrt(ntpList.Sum(x => (averageNtpDiff - x) * (averageNtpDiff - x)) / (ntpList.Count - 1));
				double ntpFittedOneSigma = Math.Sqrt(ntpFittedList.Sum(x => (averageNtpFittedDiff - x) * (averageNtpFittedDiff - x)) / (ntpFittedList.Count - 1));
				double windowsOneSigma = Math.Sqrt(windowsList.Sum(x => (averageWindowsDiff - x) * (averageWindowsDiff - x)) / (windowsList.Count - 1));

				double maxNtpDiff = ntpList.Max((x) => Math.Abs(x));
				double maxNtpFittedDiff = ntpFittedList.Max((x) => Math.Abs(x));
				double maxWindowsDiff = windowsList.Max((x) => Math.Abs(x));

				if (MessageBox.Show(
					this, 
					string.Format("Results for differnces between OCR-ed time and NTP time:\r\n\r\nOCR-NTPRaw: Average = {0:0.0} ms, 1-Sigma = {1:0.00} ms, Max = {2:0.0} ms\r\nOCR-NTPFitted: Average = {3:0.0} ms, 1-Sigma = {4:0.00} ms, Max = {5:0.0} ms\r\n OCR-WindowsRaw: Average = {6:0.0} ms, 1-Sigma = {7:0.00} ms, Max = {8:0.0} ms\r\n\r\n\r\nWould you like to save all the used data in a CSV format?", 
						averageNtpDiff, ntpOneSigma, maxNtpDiff,
						averageNtpFittedDiff, ntpFittedOneSigma, maxNtpFittedDiff,
						averageWindowsDiff, windowsOneSigma, maxWindowsDiff),
                    "Tangra",
					MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
				{
					string tempFile = Path.ChangeExtension(Path.GetTempFileName(), ".csv");
					File.WriteAllText(tempFile, output.ToString());
					Process.Start(tempFile);
				}
			}
		}
	}
}
