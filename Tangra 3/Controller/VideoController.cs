/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Tangra.Helpers;
using Tangra.Model.Helpers;
using Tangra.Video.FITS;
using Tangra.Video.SER;
using Tangra.VideoOperations.Astrometry;
using Tangra.VideoOperations.LightCurves.Tracking;
using Tangra.VideoTools;
using nom.tam.fits;
using nom.tam.util;
using Tangra.Config;
using Tangra.ImageTools;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Model.Context;
using Tangra.Model.Image;
using Tangra.Model.ImageTools;
using Tangra.Model.Video;
using Tangra.Model.VideoOperations;
using Tangra.OCR;
using Tangra.OCR.TimeExtraction;
using Tangra.PInvoke;
using Tangra.SDK;
using Tangra.Video;
using Tangra.Video.AstroDigitalVideo;
using Tangra.VideoOperations;
using Tangra.VideoOperations.LightCurves;
using Tangra.VideoOperations.LightCurves.Helpers;
using Tangra.VideoOperations.MakeDarkFlatField;
using Tangra.View;
using Cursor = System.Windows.Forms.Cursor;
using TangraCore = Tangra.PInvoke.TangraCore;

namespace Tangra.Controller
{
    public class TangraOpenFileArgs
    {
        public double FrameRate { get; set; }
        public int BitPix { get; set; }
        public SerUseTimeStamp SerTiming { get; set; }
        public bool GrayScaleRGB { get; set; }
    }

	public class VideoController : IDisposable, IVideoFrameRenderer, IVideoController, IImagePixelProvider, IFileInfoProvider
	{
		private VideoFileView m_VideoFileView;
        private ZoomedImageView m_ZoomedImageView;
	    private ImageToolView m_ImageToolView;

		private Form m_MainFormView;
	    private Panel m_pnlControlerPanel;

		private IFramePlayer m_FramePlayer;

	    public IFramePlayer FramePlayer
	    {
	        get { return m_FramePlayer; }
	    }

		private FrameStateData m_FrameState;

		private frmAdvStatusPopup m_AdvStatusForm;
	    private frmAavStatusPopup m_AavStatusForm;
	    private frmSerStatusPopup m_SerStatusForm;
	    private frmFitsHeaderPopup m_FitsStatusForm;
	    private frmTargetPSFViewerForm m_TargetPSFViewerForm;
	    private frmQhyHeaderViewer m_QhyHeaderForm;

		private AdvOverlayManager m_OverlayManager = new AdvOverlayManager();

        private AstroImage m_AstroImage;
        private RenderFrameContext m_CurrentFrameContext;
	    private int m_CurrentFrameId;
	    private DateTime? m_CurrentOCRRedTimeStamp;

        private ImageTool m_ImageTool;
        private IVideoOperation m_CurrentOperation;

	    private PSFFit m_TargetPsfFit;
	    private uint[,] m_TargetBackgroundPixels;

	    private LightCurveController m_LightCurveController;
		private AddinsController m_AddinsController;
	    private frmMain m_MainForm;

        internal IWin32Window Win32MainWindow
	    {
	        get { return m_MainForm; }
	    }

		private RenderOverlayCallback m_CustomOverlayRenderer;

        private OcrExtensionManager m_OcrExtensionManager;
        private ITimestampOcr m_TimestampOCR;
        private int m_NumberOcredVtiOsdFrames;
        private int m_NumberFailedOcredVtiOsdFrames;

	    public OcrExtensionManager OcrExtensionManager
	    {
	        set { m_OcrExtensionManager = value; }
	    }

		private DisplayIntensifyMode m_DisplayIntensifyMode = DisplayIntensifyMode.Off;
	    private bool m_DisplayInvertedMode = false;
        private bool m_DisplayHueIntensityMode = false;
	    private bool m_DisplayHueBackgroundMode = false;
	    private int m_HBMTarget1X = 0;
		private int m_HBMTarget1Y = 0;
		private int m_HBMTarget2X = 0;
		private int m_HBMTarget2Y = 0;
		private int m_HBMTarget3X = 0;
		private int m_HBMTarget3Y = 0;
		private int m_HBMTarget4X = 0;
		private int m_HBMTarget4Y = 0;
	    private int m_DynamicFromValue = 0;
		private int m_DynamicToValue = 0;
		private uint m_DynamicMaxPixelValue = 0;

		private List<Action<int, bool>> m_OnStoppedCallbacks = new List<Action<int, bool>>();

		private int m_NewFileStreamId;
		private int m_QhyCheckedFileStreamId;

		public VideoController(Form mainFormView, VideoFileView videoFileView, ZoomedImageView zoomedImageView, ImageToolView imageToolView, Panel pnlControlerPanel)
		{
			m_FramePlayer = new FramePlayer();
			m_VideoFileView = videoFileView;
            m_ZoomedImageView = zoomedImageView;
			m_ImageToolView = imageToolView;
			m_MainFormView = mainFormView;
	        m_MainForm = (frmMain) mainFormView;
            m_pnlControlerPanel = pnlControlerPanel;
            videoFileView.SetFramePlayer(m_FramePlayer);
			m_ImageToolView.SetVideoController(this);
			CloseOpenedVideoFile();

			InitialisePersistedSettings();
		}

		private void InitialisePersistedSettings()
		{
			m_DisplayHueIntensityMode = TangraConfig.Settings.Generic.UseHueIntensityDisplayMode;
			m_MainForm.tsmiHueIntensity.Checked = TangraConfig.Settings.Generic.UseHueIntensityDisplayMode;

			m_DisplayInvertedMode = TangraConfig.Settings.Generic.UseInvertedDisplayMode;
			m_MainForm.tsmiInverted.Checked = TangraConfig.Settings.Generic.UseInvertedDisplayMode;

			m_DisplayIntensifyMode = TangraConfig.Settings.Generic.UseDisplayIntensifyMode;
			m_MainForm.tsmiOff.Checked = m_DisplayIntensifyMode == DisplayIntensifyMode.Off;
			m_MainForm.tsmiLo.Checked = m_DisplayIntensifyMode == DisplayIntensifyMode.Lo;
			m_MainForm.tsmiHigh.Checked = m_DisplayIntensifyMode == DisplayIntensifyMode.Hi;

			if (m_DisplayHueIntensityMode) UsageStats.Instance.HueIntensityModeUsed++;
			if (m_DisplayInvertedMode) UsageStats.Instance.InvertedModeUsed++;
			if (m_DisplayIntensifyMode == DisplayIntensifyMode.Hi) UsageStats.Instance.HighGammaModeUsed++;
			if (m_DisplayIntensifyMode == DisplayIntensifyMode.Lo) UsageStats.Instance.LowGammaModeUsed++;
			if (m_DisplayIntensifyMode == DisplayIntensifyMode.Off) UsageStats.Instance.NoGammaModeUsed++;

			UsageStats.Instance.Save();
		}

		internal void SetLightCurveController(LightCurveController lightCurveController)
		{
			m_LightCurveController = lightCurveController;
		}

		internal void SetAddinsController(AddinsController addinsController)
		{
			m_AddinsController = addinsController;
		}

		public void CloseOpenedVideoFile()
		{
			if (m_FramePlayer.Video != null)
			{
				TangraContext.Current.Reset();
				m_FramePlayer.CloseVideo();
			}

			if (m_CurrentOperation != null)
			{
				m_CurrentOperation.FinalizeOperation();
				m_CurrentOperation = null;
			}

			if (m_ImageTool != null)
			{
				m_ImageTool.Deactivate();
				m_ImageTool = null;
			}

			DeselectFeature();

			if (m_ZoomedImageView != null)
				m_ZoomedImageView.ClearZoomedImage();

			EnsureAllPopUpFormsClosed();

			m_AstroImage = null;
		    m_TimestampOCR = null;
			m_CurrentFrameContext = RenderFrameContext.Empty;

            TangraCore.PreProcessors.ClearAll();

			UpdateViews();

			m_MainForm.pnlControlerPanel.Controls.Clear();
		}

		internal bool SingleBitmapFile(LCFile lcFile)
		{
			return OpenVideoFileInternal(
				null, 
				() => new SingleBitmapFileFrameStream(lcFile));
		}

		internal bool SingleBitmapFile(byte[] bitmapBytes, int width, int height)
		{
			return OpenVideoFileInternal(
				null,
				() => new SingleBitmapFileFrameStream(bitmapBytes, width, height));
		}

	    public bool OpenFitsFileSequence(string folderName)
	    {
            string[] fitsFiles = null;
            try
            {
                fitsFiles = Directory.GetFiles(folderName, "*.fit*", SearchOption.TopDirectoryOnly);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.GetFullStackTrace());
                ShowMessageBox("Please specify a valid folder path. '" + folderName + "' appears to be invalid.", "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (fitsFiles.Length == 0)
            {
                ShowMessageBox("No FITS files found inside " + folderName, "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else
            {
                var fitsType = FITSHelper2.GetFitsType(fitsFiles[0]);
                if (fitsType == FitsType.Invalid)
                {
                    ShowMessageBox("The FITS files must have either 2 axis or 3 axis with the third being an RGB selection.", "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                else if (fitsType == FitsType.RGBFrames)
                {
                    ShowMessageBox("Tangra currently doesn't support FITS images with 3 axis with an RGB colour planes. It these files have been created in MaximDL then use the 'Convert to mono' function before opening them in Tangra.", "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                var frm = new frmSortFitsFiles(this);
                frm.SetFiles(fitsFiles, fitsType);
                frm.StartPosition = FormStartPosition.CenterParent;

                if (frm.ShowDialog(m_MainForm) == DialogResult.OK)
                {
                    if (!string.IsNullOrWhiteSpace(frm.ErrorMessage))
                    {
                        ShowMessageBox(frm.ErrorMessage, "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    fitsFiles = frm.GetSortedFiles();

                    return OpenFitsFileSequence(folderName, fitsFiles, frm.TimeStampReader, frm.BitPix, frm.NegPixCorrection, frm.FlipVertically, frm.FlipHorizontally);
                }
                else
                    return false;
            }
	    }

	    public bool OpenFitsFileSequence(
            string folderName, string[] sortedFitsFiles, IFITSTimeStampReader fitsTimeStampReader,
            int? bitBix, int negPixCorrection, bool flipVertically, bool flipHorizontally, int firstFrameNo = 0)
		{
            TangraContext.Current.IsFitsStream = false;
		    TangraContext.Current.IsAviFile = false;

            try
            {
                return OpenVideoFileInternal(
                    folderName,
                    () =>
                    {
                        FITSFileSequenceStream stream = FITSFileSequenceStream.OpenFolder(sortedFitsFiles, fitsTimeStampReader, firstFrameNo, bitBix, negPixCorrection);

                        SetFlipSettings(flipVertically, flipHorizontally);

                        TangraContext.Current.IsFitsStream = true;
                        TangraContext.Current.IsAviFile = false;
                        return stream;
                    });
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.GetFullStackTrace());

                MessageBox.Show(
                    m_MainForm,
                    "Error opening FITS files: " + ex.Message,
                    "Tangra",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1);

                return false;
            }
        }

		public bool OpenBitmapFileSequence(string folderName)
		{
			string[] fitsFiles = Directory.GetFiles(folderName, "*.bmp", SearchOption.TopDirectoryOnly);
			if (fitsFiles.Length == 0)
			{
				ShowMessageBox("No BMP files found inside " + folderName, "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
			else
			{
				return OpenVideoFileInternal(
					folderName,
					() =>
					{
						return BMPFileSequenceStream.OpenFolder(fitsFiles);
					});
			}
		}

	    public bool OpenVideoFile(string fileName, TangraOpenFileArgs args = null)
	    {
			string fileExtension = Path.GetExtension(fileName);

	        if (fileExtension != null) 
                fileExtension = fileExtension.ToLower();

	        if (fileExtension == ".aav")
	            AavFileHelper.CheckAndCorrectBadMaxPixelValue(fileName, this);

		    return OpenVideoFileInternal(
				fileName, 
				() =>
				{
					IFrameStream frameStream = null;

                    try
                    {
                        if (fileExtension == ".adv" || fileExtension == ".aav")
                        {
                            AdvFileMetadataInfo fileMetadataInfo;
                            GeoLocationInfo geoLocation;
                            frameStream = AstroDigitalVideoStream.OpenFile(fileName, out fileMetadataInfo, out geoLocation);
                            if (frameStream != null)
                            {
                                TangraContext.Current.UsingADV = true;
                                m_OverlayManager.InitAdvFile(fileMetadataInfo, geoLocation, frameStream.FirstFrame);
                            }
                        }
                        else if (fileExtension == ".ser")
                        {
                            SerEquipmentInfo equipmentInfo;
                            frameStream = SERVideoStream.OpenFile(fileName, m_MainForm, args, out equipmentInfo);
                            if (frameStream != null)
                            {
                                TangraContext.Current.IsSerFile = true;
                                m_OverlayManager.InitSerFile(equipmentInfo, frameStream.FirstFrame);
                            }
                        }
                        else if (fileExtension == ".bmp")
                        {
                            frameStream = SingleBitmapFileFrameStream.OpenFile(fileName);
                        }
                        else if (fileExtension == ".fit" || fileExtension == ".fits")
                        {
	                        try
	                        {
	                            var cubeType = FITSHelper2.GetFitsCubeType(fileName);
	                            switch (cubeType)
	                            {
	                                case FitsCubeType.NotACube:
								        bool hasNegativePixels;
								        frameStream = SingleFITSFileFrameStream.OpenFile(fileName, out hasNegativePixels);
	                                    if (hasNegativePixels)
	                                        InformUserAboutNegativePixels();

	                                    break;
                                    case FitsCubeType.ThreeAxisCube:
                                        frameStream = ThreeAxisFITSCubeFrameStream.OpenFile(fileName, this);
	                                    break;
                                    case FitsCubeType.MultipleHDUsCube:
                                        break;
	                            }
	                        }
							catch (Exception ex)
							{
								Trace.WriteLine(ex.GetFullStackTrace());

								MessageBox.Show(
									m_MainForm,
									"Error opening FITS file: " + ex.Message,
									"Tangra",
									MessageBoxButtons.OK,
									MessageBoxIcon.Error,
									MessageBoxDefaultButton.Button1);

								return null;
							}
                        }
                        else
                        {
                            frameStream = VideoStream.OpenFile(fileName);
                            frameStream = ReInterlacingVideoStream.Create(frameStream, ReInterlaceMode.None);
                        }


                        m_NewFileStreamId = frameStream != null ? frameStream.GetHashCode() : 0;
                        return frameStream;
                    }
                    catch (IOException ioex)
                    {
                        MessageBox.Show(ioex.Message, "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        return null;
                    }					
				});
	    }

        private void InformUserAboutNegativePixels()
        {
            MessageBox.Show(
                m_MainForm, 
                "Negative pixel values have been found in this FITS file. They will be zeroed out?", 
                "Tangra", 
                MessageBoxButtons.OK, 
                MessageBoxIcon.Information, 
                MessageBoxDefaultButton.Button1);
        }

		public bool OpenVideoFileInternal(string fileName, Func<IFrameStream> frameStreamFactoryMethod)
		{
			TangraContext.Current.UsingADV = false;
		    TangraContext.Current.IsAAV2 = false;
			TangraContext.Current.IsSerFile = false;
		    TangraContext.Current.IsFitsStream = false;
            TangraContext.Current.IsAviFile = false;
			TangraContext.Current.FileName = null;
			TangraContext.Current.FileFormat = null;
			TangraContext.Current.HasVideoLoaded = false;

			m_OverlayManager.Reset();

		    TangraContext.Current.CrashReportInfo.VideoFile = fileName;

			TangraCore.PreProcessors.ClearAll();
			TangraCore.PreProcessors.AddGammaCorrection(TangraConfig.Settings.Photometry.EncodingGamma);
			TangraCore.PreProcessors.AddCameraResponseCorrection(TangraConfig.Settings.Photometry.KnownCameraResponse, TangraConfig.Settings.Photometry.KnownCameraResponseParams);

			IFrameStream frameStream = frameStreamFactoryMethod();

            if (frameStream != null && m_FramePlayer != null)
			{
				m_FramePlayer.OpenVideo(frameStream);

                if (GetVideoFileFormat() == VideoFileFormat.AVI && string.IsNullOrEmpty(GetVideoFormat(VideoFileFormat.AVI)))
                {
                    ShowMessageBox(
                        string.Format("The frame rate of this avi file is {0:0.000} fps which is neither PAL nor NTSC standard. Some operations in Tangra may not work correctly with this file!", frameStream.FrameRate),
                        "Tangra",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                };

				if (!string.IsNullOrEmpty(fileName))
				{
					TangraContext.Current.FileName = Path.GetFileName(fileName);
					TangraContext.Current.FileFormat = frameStream.VideoFileType;
				    TangraContext.Current.CrashReportInfo.StreamType = m_FramePlayer.Video.GetType().Name;
				}

				if (!IsAstroDigitalVideo)
					HideAdvStatusForm();

               
				TangraContext.Current.FrameWidth = m_FramePlayer.Video.Width;
				TangraContext.Current.FrameHeight = m_FramePlayer.Video.Height;
				TangraContext.Current.FirstFrame = m_FramePlayer.Video.FirstFrame;
				TangraContext.Current.LastFrame = m_FramePlayer.Video.LastFrame;

				PSFFit.SetDataRange(m_FramePlayer.Video.BitPix, m_FramePlayer.Video.GetAav16NormVal());

				TangraContext.Current.HasVideoLoaded = true;
				TangraContext.Current.CanPlayVideo = true;
				TangraContext.Current.CanChangeTool = true;
			    TangraContext.Current.CanLoadBiasFrame = true;
				TangraContext.Current.CanLoadDarkFrame = true;
				TangraContext.Current.CanLoadFlatFrame = true;
				TangraContext.Current.CanScrollFrames = true;
                TangraContext.Current.IsAviFile = m_FramePlayer.IsAviVideo;
                TangraContext.Current.IsAAV2 = m_FramePlayer.IsAstroAnalogueVideoV2;

				TangraContext.Current.HasImageLoaded = true;
                TangraContext.Current.UndefinedFrameRate = double.IsNaN(m_FramePlayer.Video.FrameRate);

				m_VideoFileView.UpdateVideoSizeAndLengthControls();

				// Turn off the Dynamic Range (if active) when opening a new video 
				if (m_DisplayIntensifyMode == DisplayIntensifyMode.Dynamic)
				{
					m_DisplayIntensifyMode = DisplayIntensifyMode.Off;
					m_MainForm.tsmiDynamic.Checked = false;
				}
				m_DynamicFromValue = 0;
				m_DynamicToValue = 0;
				m_DynamicMaxPixelValue = 0;

			    m_FramePlayer.MoveToFrame(frameStream.FirstFrame);

                IFITSStream fitsSteream = m_FramePlayer.Video as IFITSStream;
                if (fitsSteream != null)
                    SetFITSDynamicRange(fitsSteream, frameStream);

				m_VideoFileView.Update();

				if (
					(IsAstroDigitalVideo && !IsAdvStatusFormVisible) ||
					(IsAstroAnalogueVideo && !IsAavStatusFormVisible))
				{
					ToggleAstroVideoStatusForm(true);
				}
                else if (IsSerVideo && !IsSerStatusFormVisible)
                {
                    ToggleSerStatusForm(true);
                }
                else if ((IsFitsSequence || IsFitsFile) && !IsFitsStatusFormVisible)
                {
                    ToggleFitsStatusForm(true);
                }

				if (!string.IsNullOrEmpty(fileName))
                    RegisterRecentFile(IsFitsSequence ? RecentFileType.FitsSequence : RecentFileType.Video, fileName);

				m_ImageTool = ImageTool.SwitchTo<ArrowTool>(null, m_ImageToolView, m_ImageTool);
                m_AddinsController.SetFileInfoProvider(this);

				return true;
			}
			else
			{
			    if (TangraContext.Current.CustomRenderer != null)
			    {
                    if (!string.IsNullOrEmpty(fileName))
                        RegisterRecentFile(RecentFileType.Video, fileName);

			        TangraContext.Current.CustomRenderer.ShowModal(this);
			    }

				// TODO: Show error message	
			}

			return false;
		}

	    private void SetFITSDynamicRange(IFITSStream fitsSteream, IFrameStream frameStream)
	    {
	        try
	        {
	            m_MainForm.Cursor = Cursors.WaitCursor;
	            m_MainForm.Update();

	            int dynamicValueFrom = (int) fitsSteream.MinPixelValue;

	            for (double coeff = 0.05; coeff < 2.5; coeff += 0.05)
	            {
	                int dynamicValueTo = (int) (coeff*fitsSteream.MaxPixelValue + 0.95*fitsSteream.MinPixelValue);

	                SetDisplayIntensifyMode(DisplayIntensifyMode.Dynamic, dynamicValueFrom, dynamicValueTo, false);

	                var pixMap = m_FramePlayer.GetFrame(frameStream.FirstFrame, false);

	                BitmapFilter.ApplyDynamicRange(pixMap.DisplayBitmap, pixMap, m_DynamicFromValue, m_DynamicToValue, m_DisplayInvertedMode, m_DisplayHueIntensityMode, EffectiveMaxPixelValue);

	                var dynPixMap = Pixelmap.ConstructFromBitmap(pixMap.DisplayBitmap, TangraConfig.ColourChannel.Red);
	                int sq = Math.Min(64, pixMap.Height/3);
	                double averagePixel = dynPixMap.DisplayBitmapPixels.Skip(pixMap.Width*sq).Take(sq*sq).Average(x => x);
	                if (averagePixel > 50 && averagePixel < 150)
	                    break;
	            }

	            SetDisplayIntensifyMode(DisplayIntensifyMode.Dynamic, m_DynamicFromValue, m_DynamicToValue);

	            m_MainForm.tsmiOff.Checked = false;
	            m_MainForm.tsmiLo.Checked = false;
	            m_MainForm.tsmiHigh.Checked = false;
	            m_MainForm.tsmiDynamic.Checked = true;
	        }
	        finally
	        {
	            m_MainForm.Cursor = Cursors.Default;
	        }
	    }

	    public void LoadAvailableOcrEngines(ComboBox cbxOcrEngine)
	    {
	        m_OcrExtensionManager.LoadAvailableOcrEngines(cbxOcrEngine);
	    }

	    public void InitializeTimestampOCR()
        {
            m_TimestampOCR = null;
            m_NumberOcredVtiOsdFrames = 0;
            m_NumberFailedOcredVtiOsdFrames = 0;

            if (IsVideoWithVtiOsdTimeStamp &&
                !TangraContext.Current.UsingIntegration && // OCR not supported when using software integration
                (TangraConfig.Settings.Generic.OsdOcrEnabled || TangraConfig.Settings.Generic.OcrAskEveryTime))
            {
                bool forceSaveErrorReport = false;

                if (TangraConfig.Settings.Generic.OcrAskEveryTime)
                {
                    var frm = new frmChooseOcrEngine(m_OcrExtensionManager);
                    frm.StartPosition = FormStartPosition.CenterParent;
                    frm.ShowForceErrorReportOption = false;
                    if (ShowDialog(frm) == DialogResult.Cancel ||
                        !TangraConfig.Settings.Generic.OsdOcrEnabled)
                        return;

                    forceSaveErrorReport = frm.ForceErrorReport;
                }

                m_TimestampOCR = m_OcrExtensionManager.GetCurrentOcr();

                if (m_TimestampOCR != null)
                {
                    var data = new TimestampOCRData();
                    data.FrameWidth = TangraContext.Current.FrameWidth;
                    data.FrameHeight = TangraContext.Current.FrameHeight;
                    data.OSDFrame = LightCurveReductionContext.Instance.OSDFrame;
                    data.VideoFrameRate = (float)VideoFrameRate;
                    data.ForceErrorReport = forceSaveErrorReport;
                    data.IntegratedAAVFrames = m_FramePlayer.IsAstroAnalogueVideoV2 ? ((AstroDigitalVideoStreamV2)m_FramePlayer.Video).IntegratedAAVFrames : 0;

                    m_TimestampOCR.Initialize(data, this,
#if WIN32
 (int)TangraConfig.Settings.Tuning.OcrMode
#else
                        (int)TangraConfig.OCRMode.FullyManaged
#endif
);

                    int maxCalibrationFieldsToAttempt = Math.Min(VideoLastFrame - CurrentFrameIndex - 1, TangraConfig.Settings.Generic.MaxCalibrationFieldsToAttempt);
                    if (m_FramePlayer.IsAstroAnalogueVideoV2)
                        // Make sure we are not trying to read more AAV OCR calibration frames than what's available                         
                        maxCalibrationFieldsToAttempt = Math.Min(((AstroDigitalVideoStreamV2)m_FramePlayer.Video).GetNumberOfOcrCalibrationFrames(), maxCalibrationFieldsToAttempt);

                    if (m_TimestampOCR.RequiresCalibration)
                    {
                        // NOTE: If we are measuring the video backwards the OCR will need to be intialized forward (i.e. double backwards)
                        bool measuringBackwards = LightCurveReductionContext.Instance.LightCurveReductionType == LightCurveReductionType.TotalLunarReppearance;

                        int firstCalibrationFrame = measuringBackwards
                            ? Math.Min(CurrentFrameIndex + 30, VideoLastFrame)
                            : (m_FramePlayer.IsAstroAnalogueVideoV2 ? 0 : CurrentFrameIndex);

                        var pixels = GetOsdCalibrationFrame(firstCalibrationFrame);
                        m_TimestampOCR.ProcessCalibrationFrame(firstCalibrationFrame, pixels);

                        bool isCalibrated = true;

                        if (m_TimestampOCR.InitiazliationError == null)
                        {
                            int calibrationFramesProcessed = 0;
                            StatusChanged("Calibrating OCR");
                            FileProgressManager.BeginFileOperation(maxCalibrationFieldsToAttempt);
                            try
                            {
                                var processingMethod = new Func<int, bool>(delegate(int i)
                                {
                                    if (m_TimestampOCR == null)
                                        return false;

                                    pixels = GetOsdCalibrationFrame(i);
                                    isCalibrated = m_TimestampOCR.ProcessCalibrationFrame(i, pixels);

                                    if (m_TimestampOCR.InitiazliationError != null)
                                    {
                                        // This doesn't look like what the OCR engine is expecting. Abort ....
                                        m_TimestampOCR = null;
                                        m_NumberFailedOcredVtiOsdFrames++;
                                        return false;
                                    }

                                    calibrationFramesProcessed++;

                                    FileProgressManager.FileOperationProgress(calibrationFramesProcessed);
                                    Application.DoEvents();

                                    if (isCalibrated)
                                        return true;

                                    if (calibrationFramesProcessed > maxCalibrationFieldsToAttempt || i + 1 == maxCalibrationFieldsToAttempt)
                                    {
                                        m_TimestampOCR.PrepareFailedCalibrationReport();
                                        return true;
                                    }

                                    return false;
                                });

                                if (measuringBackwards)
                                {
                                    for (int i = firstCalibrationFrame - 1; i > CurrentFrameIndex; i--)
                                    {
                                        if (processingMethod(i))
                                            break;
                                    }
                                }
                                else
                                {
                                    for (int i = firstCalibrationFrame + 1; i < VideoLastFrame; i++)
                                    {
                                        if (processingMethod(i))
                                            break;
                                    }
                                }
                            }
                            finally
                            {
                                FileProgressManager.EndFileOperation();
                                StatusChanged("Ready");
                            }
                        }

                        if (forceSaveErrorReport || !isCalibrated)
                        {
                            var frmReport = new frmOsdOcrCalibrationFailure(this);
                            frmReport.StartPosition = FormStartPosition.CenterParent;
                            frmReport.TimestampOCR = m_TimestampOCR;
                            frmReport.ForcedErrorReport = forceSaveErrorReport;

                            if (frmReport.CanSendReport())
                                ShowDialog(frmReport);

                            m_TimestampOCR = null;
                        }
                        else if (m_TimestampOCR != null)
                        {
                            if (m_TimestampOCR.InitiazliationError != null)
                            {
                                m_NumberFailedOcredVtiOsdFrames++;

                                ShowMessageBox(
                                    m_TimestampOCR.InitiazliationError,
                                    "Error reading OSD timestamp",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);

                                m_TimestampOCR = null;
                            }
                        }
                    }
                }
            }
        }

	    private int m_vtiOsdAdjustment = 0;
	    private uint[] GetOsdCalibrationFrame(int frameId)
	    {
	        if (m_FramePlayer.IsAstroAnalogueVideoV2)
	        {
	            Pixelmap pixelMap;
	            do
	            {
                    pixelMap = ((AstroDigitalVideoStreamV2)m_FramePlayer.Video).GetPixelmap(frameId + m_vtiOsdAdjustment, 1);
	                if (pixelMap != null)
	                {
	                    if (!pixelMap.FrameState.IsVtiOsdCalibrationFrame)
	                    {
	                        m_vtiOsdAdjustment++;
	                    }
	                    else
	                    {
	                        uint[] rv = pixelMap.UnprocessedPixels;
	                        if (pixelMap.MaxSignalValue > 0)
	                        {
	                            for (int i = 0; i < rv.Length; i++)
	                            {
	                                rv[i] = (uint) Math.Round(rv[i]*255.0/pixelMap.MaxSignalValue);
	                            }
	                        }
	                        return rv;
	                    }
	                }

                } 
                while (pixelMap != null);

	            return null;
	        }
            else
                return GetFrame(frameId).Pixels;
	    }

	    public void SubmitOCRErrorsIfAny()
	    {
            if (TangraContext.Current.OcrErrors > 8)
            {
                var frm = new frmOCRTooManyErrorsReport();
                frm.OCRErrors = TangraContext.Current.OcrErrors;

                if (ShowDialog(frm) == DialogResult.OK)
                {
                    var images = m_TimestampOCR.GetCalibrationReportImages();
                    var lastImage = m_TimestampOCR.GetLastUnmodifiedImage();
                    var ocrDebugImage = m_TimestampOCR.GetOCRDebugImage();
                    bool reportSendingErrored = false;
                    try
                    {
                        string message = string.Format("Encountered {0} OCR Errors.", TangraContext.Current.OcrErrors);
                        if (m_LightCurveController != null && m_LightCurveController.Context != null && m_LightCurveController.Context.AllReadings != null)
                            message = string.Format("Encountered {0} OCR Errors from {1} OCR-ed Frames.", TangraContext.Current.OcrErrors, m_LightCurveController.Context.AllReadings.Count);

                        frmOsdOcrCalibrationFailure.SendOcrErrorReport(this, message, m_TimestampOCR, images, lastImage, ocrDebugImage, frm.tbxEmail.Text);
                    }
                    catch (Exception ex)
                    {
                        m_LightCurveController.ShowMessageBox("Error submitting report: " + ex.Message, "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        reportSendingErrored = true;
                    }
                    finally
                    {
                        if (!reportSendingErrored)
                            MessageBox.Show("The error report was submitted successfully.", "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }	        
	    }

	    public void UpdateOCRStatistics()
	    {
            UsageStats.Instance.FramesIOTATimeStampRead += m_NumberOcredVtiOsdFrames;
            UsageStats.Instance.FramesIOTATimeStampReadingErrored += m_NumberFailedOcredVtiOsdFrames;	        
	    }

	    public string GetTimestampOCRNameAndVersion()
	    {
	        return m_TimestampOCR != null ? m_TimestampOCR.NameAndVersion() : string.Empty;
	    }

        public DateTime OCRTimestamp(bool debug = false)
        {
            if (m_CurrentOCRRedTimeStamp.HasValue)
                return m_CurrentOCRRedTimeStamp.Value;

            DateTime ocredTimeStamp = DateTime.MinValue;

            var osdPixels = m_AstroImage.GetOcrPixels();

            if (!m_TimestampOCR.ExtractTime(m_CurrentFrameId, m_FramePlayer.FrameStep, osdPixels, debug, out ocredTimeStamp))
            {
                ocredTimeStamp = DateTime.MinValue;
                m_NumberFailedOcredVtiOsdFrames++;
            }
            else
            {
                m_NumberOcredVtiOsdFrames++;
            }

            m_CurrentOCRRedTimeStamp = ocredTimeStamp;
            return ocredTimeStamp;
	    }

	    public DateTime GetNextFrameOCRTimestamp()
	    {
	        DateTime ocredTimeStamp = DateTime.MinValue;

	        int frameNo = m_CurrentFrameId + m_FramePlayer.FrameStep;
            var pixelmap = m_FramePlayer.GetFrame(frameNo, true);
	        var astroImage = new AstroImage(pixelmap);
            var osdPixels = astroImage.GetOcrPixels();

            if (!m_TimestampOCR.ExtractTime(frameNo, m_FramePlayer.FrameStep, osdPixels, false, out ocredTimeStamp))
                return DateTime.MinValue;
            else
                return ocredTimeStamp;

	    }

        public bool AssertOCRTimestamp(DateTime calculatedTimeStamp, bool showPopUp)
        {
            string errorMessage = null;
            DateTime ocrTimeStamp = OCRTimestamp();
            if (ocrTimeStamp == DateTime.MinValue)
                errorMessage = string.Format("Could not read the timestamp of the current frame id: {0}", m_CurrentFrameId);
            else
            {
                var span = ocrTimeStamp.TimeOfDay - calculatedTimeStamp.TimeOfDay;
                if (span.TotalMilliseconds > 2)
                    errorMessage = string.Format("The OCR-ed and calculated timestamp for the current frame {0} differs by {1:0.0}ms.\r\n\r\nOCRed time: {2}, Calculated time: {3}.\r\n\r\nThis could indicate dropped frames!",
                        m_CurrentFrameId, span.TotalMilliseconds, ocrTimeStamp.TimeOfDay, calculatedTimeStamp.TimeOfDay);
            }

            if (errorMessage != null)
            {
                Trace.WriteLine(errorMessage);
                if (showPopUp)
                    ShowMessageBox("OCR Timestamp Error:\r\n\r\n" + errorMessage, "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return true;
            }
            return false;
        }

		internal void RegisterRecentFile(RecentFileType type, string fileName)
		{
			TangraConfig.Settings.RecentFiles.NewRecentFile(type, fileName);
			TangraConfig.Settings.Save();

			m_MainForm.BuildRecentFilesMenu();
		}

        public void SetImage(Pixelmap currentPixelmap, int currentFrameId, RenderFrameContext frameContext, bool isOldFrameRefreshed)
        {
            m_AstroImage = new AstroImage(currentPixelmap);
            m_CurrentFrameContext = frameContext;

            m_CurrentOCRRedTimeStamp = null;
            m_CurrentFrameId = currentFrameId;

            m_FrameState = currentPixelmap.FrameState;

			if (m_AdvStatusForm != null && m_AdvStatusForm.Visible)
				m_AdvStatusForm.ShowStatus(m_FrameState);

			if (m_AavStatusForm != null && m_AavStatusForm.Visible)
				m_AavStatusForm.ShowStatus(m_FrameState);

			if (m_SerStatusForm != null && m_SerStatusForm.Visible)
				m_SerStatusForm.ShowStatus(m_FrameState);

            if (m_FitsStatusForm != null && m_FitsStatusForm.Visible)
                m_FitsStatusForm.ShowStatus(m_FrameState);

            if (m_QhyCheckedFileStreamId != m_NewFileStreamId)
            {
                HideQhyHeaderForm();
                if (QHYImageHeader.IsAvailable(m_AstroImage))
                {
                    ToggleQhyHeaderForm(true);
                }
                m_QhyCheckedFileStreamId = m_NewFileStreamId;
            }

            if (m_QhyHeaderForm != null && m_QhyHeaderForm.Visible)
                m_QhyHeaderForm.ShowImageHeader(m_AstroImage);

	        if (!isOldFrameRefreshed)
	        {
				m_TargetPsfFit = null;
				ShowTargetPSF();
			}
		}

        public void NewFrameDisplayed()
        {
            if (m_CurrentOperation != null)
            {
                m_CurrentOperation.NextFrame(
                    m_CurrentFrameContext.CurrentFrameIndex, 
                    m_CurrentFrameContext.MovementType, 
                    m_CurrentFrameContext.IsLastFrame, 
                    m_AstroImage, 
                    m_CurrentFrameContext.FirstFrameInIntegrationPeriod,
                    m_CurrentFrameContext.CurrentFrameFileName);

                if (m_CurrentOperation.HasCustomZoomImage &&
                    m_ZoomedImageView != null)
                {
                    m_ZoomedImageView.DrawCustomZoomImage(m_CurrentOperation);
                }
            }

            if (m_ImageTool != null)
                m_ImageTool.OnNewFrame(m_CurrentFrameContext.CurrentFrameIndex, m_CurrentFrameContext.IsLastFrame);
        }

		public bool HasAstroImageState
		{
			get { return !m_FrameState.IsEmpty(); }
		}

        public double VideoFrameRate
        {
            get
            {
                return m_FramePlayer.Video.FrameRate;
            }
        }

	    public int VideoCountFrames
	    {
		    get { return m_FramePlayer.Video.CountFrames; }
	    }

		public int VideoFirstFrame
		{
			get { return m_FramePlayer.Video.FirstFrame; }
		}

	    public int VideoLastFrame
	    {
			get { return m_FramePlayer.Video.FirstFrame + m_FramePlayer.Video.CountFrames - 1; }
	    }

		public int VideoBitPix
		{
			get
			{
				return m_FramePlayer.Video.BitPix;
			}
		}

	    public uint VideoAav16NormVal
	    {
		    get { return m_FramePlayer.Video.GetAav16NormVal(); }
	    }

	    public uint EffectiveMaxPixelValue
	    {
		    get
		    {
			    if (m_FramePlayer.Video.BitPix == 16 && VideoAav16NormVal > 0)
				    return VideoAav16NormVal;
			    else
				    return (uint)((1 << m_FramePlayer.Video.BitPix) - 1);
		    }
	    }

	    public int DynamicFromValue
	    {
			get { return m_DynamicFromValue; }
	    }

		public int DynamicToValue
		{
			get { return m_DynamicToValue; }
		}

	    public string CurrentVideoFileName
	    {
            get { return m_FramePlayer.Video == null ? null : m_FramePlayer.Video.FileName; }
	    }

		public string CurrentVideoFileType
	    {
            get { return m_FramePlayer.Video == null ? null : m_FramePlayer.Video.VideoFileType; }
	    }

		public string CurrentVideoFileEngine
		{
            get { return m_FramePlayer.Video == null ? null : m_FramePlayer.Video.Engine; }
		}

        public SerUseTimeStamp GetSerTimingType()
        {
            if (m_FramePlayer.Video.Engine == "SER")
            {
                var serPlayer = m_FramePlayer.Video as SERVideoStream;
                if (serPlayer != null)
                    return serPlayer.UseTimeStamp;
            }

            return SerUseTimeStamp.None;
        }

		public void SetupFrameIntegration(int framesToIntegrate, FrameIntegratingMode frameMode, PixelIntegrationType pixelIntegrationType)        
		{
			m_FramePlayer.SetupFrameIntegration(framesToIntegrate, frameMode, pixelIntegrationType);
		}

        public bool IsUsingSteppedAveraging
        {
            get { return m_FramePlayer.FrameIntegratingMode == FrameIntegratingMode.SteppedAverage; }
        }

        public int FramesToIntegrate
        {
            get { return m_FramePlayer.FramesToIntegrate; }
        }

        public void OverlayStateForFrame(Bitmap displayBitmap, RenderFrameContext context)
		{
            if (m_CurrentOperation != null &&
                m_CurrentOperation.AvoidImageOverlays)
            {
                // The current operation doesn't want overlays displayed
            }
            else
                m_OverlayManager.OverlayStateForFrame(displayBitmap, m_FrameState, context, IsAstroDigitalVideo, IsAstroAnalogueVideo);
		}

        public void CompleteRenderFrame(Graphics g)
        {
            if (m_CurrentOperation != null)
                m_CurrentOperation.PreDraw(g);

			if (m_ImageTool != null)
				m_ImageTool.PreDraw(g);


            if (m_CurrentOperation != null)
                m_CurrentOperation.PostDraw(g);

	        if (m_ImageTool != null)
		        m_ImageTool.PostDraw(g);

            if (m_TimestampOCR != null)
                // Plot the positions of the timestamp blocks
                m_TimestampOCR.DrawLegend(g);
        }

	    private static Font s_OCRPrintFont = new Font(FontFamily.GenericMonospace, 10);

        internal void PrintOCRedTimeStamp(Graphics g, bool isLastFrame)
        {
            if (m_TimestampOCR != null)
            {
                string output = m_TimestampOCR.OSDType();
                var size = g.MeasureString(output, s_OCRPrintFont);
                var vertPading = 2;
                g.FillRectangle(Brushes.DarkSlateGray, m_AstroImage.Width - size.Width - 10, 10, size.Width, size.Height);
                g.DrawString(output, s_OCRPrintFont, Brushes.Lime, m_AstroImage.Width - size.Width - 10, 10);

                DateTime ocrTimeStamp = OCRTimestamp(DebugOCR);                
                output = string.Format("{0}", ocrTimeStamp.Year != 1 ? ocrTimeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff") : ocrTimeStamp.ToString("HH:mm:ss.fff"));
                size = g.MeasureString(output, s_OCRPrintFont);
                g.FillRectangle(Brushes.DarkSlateGray, m_AstroImage.Width - size.Width - 10, 10 + size.Height + vertPading, size.Width, size.Height);
                g.DrawString(output, s_OCRPrintFont, Brushes.Lime, m_AstroImage.Width - size.Width - 10, 10 + size.Height + vertPading);

                int i = 1;
                int correctionsMade = 0;
                if (m_TimestampOCR.LastOddFieldOSD != null)
                {
                    correctionsMade += m_TimestampOCR.LastOddFieldOSD.NumberOfCorrectedDifferences;

                    if (DebugOCR)
                    {
                        i++;
                        output = string.Format("Odd: {0}", m_TimestampOCR.LastOddFieldOSD.AsFullString());
                        size = g.MeasureString(output, s_OCRPrintFont);
                        g.FillRectangle(Brushes.DarkSlateGray, m_AstroImage.Width - size.Width - 10, 10 + i * (size.Height + vertPading), size.Width, size.Height);
                        g.DrawString(output, s_OCRPrintFont, Brushes.Yellow, m_AstroImage.Width - size.Width - 10, 10 + i * (size.Height + vertPading));
                    }

                    i++;
                    output = string.Format("Odd OCR: {0}", m_TimestampOCR.LastOddFieldOSD.OcredCharacters);
                    size = g.MeasureString(output, s_OCRPrintFont);
                    g.FillRectangle(Brushes.DarkSlateGray, m_AstroImage.Width - size.Width - 10, 10 + i * (size.Height + vertPading), size.Width, size.Height);
                    g.DrawString(output, s_OCRPrintFont, Brushes.Yellow, m_AstroImage.Width - size.Width - 10, 10 + i * (size.Height + vertPading));
                }

                if (m_TimestampOCR.LastEvenFieldOSD != null)
                {
                    correctionsMade += m_TimestampOCR.LastEvenFieldOSD.NumberOfCorrectedDifferences;

                    if (DebugOCR)
                    {
                        i++;
                        output = string.Format("Even: {0}", m_TimestampOCR.LastEvenFieldOSD.AsFullString());
                        size = g.MeasureString(output, s_OCRPrintFont);
                        g.FillRectangle(Brushes.DarkSlateGray, m_AstroImage.Width - size.Width - 10, 10 + i * (size.Height + vertPading), size.Width, size.Height);
                        g.DrawString(output, s_OCRPrintFont, Brushes.Yellow, m_AstroImage.Width - size.Width - 10, 10 + i * (size.Height + vertPading));
                    }

                    i++;
                    output = string.Format("Even OCR: {0}", m_TimestampOCR.LastEvenFieldOSD.OcredCharacters);
                    size = g.MeasureString(output, s_OCRPrintFont);
                    g.FillRectangle(Brushes.DarkSlateGray, m_AstroImage.Width - size.Width - 10, 10 + i * (size.Height + vertPading), size.Width, size.Height);
                    g.DrawString(output, s_OCRPrintFont, Brushes.Yellow, m_AstroImage.Width - size.Width - 10, 10 + i * (size.Height + vertPading));                    
                }

                if (correctionsMade > 0)
                {
                    i++;
                    output = string.Format("{0} corrections made to raw OCRed chars.", correctionsMade);
                    size = g.MeasureString(output, s_OCRPrintFont);
                    g.FillRectangle(Brushes.DarkSlateGray, m_AstroImage.Width - size.Width - 10, 10 + i * (size.Height + vertPading), size.Width, size.Height);
                    g.DrawString(output, s_OCRPrintFont, Brushes.Yellow, m_AstroImage.Width - size.Width - 10, 10 + i * (size.Height + vertPading));                                                                                   
                }

                if (!string.IsNullOrWhiteSpace(m_TimestampOCR.LastFailedReason))
                {
                    var tokens = m_TimestampOCR.LastFailedReason.Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    foreach(var token in tokens)
                    {
                        if (string.IsNullOrWhiteSpace(token)) continue;
                        output = token;
                        i++;
                        size = g.MeasureString(output, s_OCRPrintFont);
                        g.FillRectangle(Brushes.DarkSlateGray, m_AstroImage.Width - size.Width - 10, 10 + i * (size.Height + vertPading), size.Width, size.Height);
                        g.DrawString(output, s_OCRPrintFont, Brushes.Orange, m_AstroImage.Width - size.Width - 10, 10 + i * (size.Height + vertPading));                                            
                    }
                }
                else
                {
                    // If this is not an OCR error then give it up to 5 points minus the corrections made
                    OCRScore += (5 - correctionsMade);
                    OCRScoredFrames++;
                }

                if (isLastFrame)
                {
                    output = string.Format("Total Score: {0:0.000}%", OCRScore * 100.0 / (5 * OCRScoredFrames));
                    i++;
                    size = g.MeasureString(output, s_OCRPrintFont);
                    g.FillRectangle(Brushes.DarkSlateGray, m_AstroImage.Width - size.Width - 10, 10 + i * (size.Height + vertPading), size.Width, size.Height);
                    g.DrawString(output, s_OCRPrintFont, Brushes.WhiteSmoke, m_AstroImage.Width - size.Width - 10, 10 + i * (size.Height + vertPading));
                }

                var dbgImage = m_TimestampOCR.GetOCRDebugImage();
                if (dbgImage != null)
                    g.DrawImage(dbgImage, 10, 10);
            }
        }

	    internal bool HasCustomRenderers
	    {
	        get { return m_CustomOverlayRenderer != null && m_CurrentOperation == null; }
	    }

		internal void RunCustomRenderers(Graphics g, bool isLastFrame)
		{
			if (m_CustomOverlayRenderer != null && m_CurrentOperation == null)
			{
				// Run custom overlay renderers when no operation is selected
				try
				{
                    m_CustomOverlayRenderer(g, isLastFrame);
				}
				catch (Exception ex)
				{
					Trace.WriteLine(ex);
				}
			}
		}

        public bool m_ShowFields;

        public void ToggleShowFieldsMode(bool showFields)
        {
            m_ShowFields = showFields;
            RedrawCurrentFrame(m_ShowFields);
        }

        public void RedrawCurrentFrame(bool showFields, bool reloadImage = false, bool reprocess = true)
		{
            if (m_AstroImage != null &&
                m_AstroImage.Pixelmap != null)
            {
                if (showFields)
                {
                    using (Bitmap image = m_AstroImage.Pixelmap.CreateNewDisplayBitmap())
                    {
                        Bitmap pixelMapWithFields = BitmapFilter.ToVideoFields(image);
                        m_MainForm.pictureBox.Image = pixelMapWithFields;
                    }
                }
				else if (reloadImage)
				{
                    var newBmp = m_AstroImage.Pixelmap.CreateNewDisplayBitmap();
				    if (reprocess)
                        ApplyDisplayModeAdjustments(newBmp, false, m_AstroImage.Pixelmap);    

                    m_MainForm.pictureBox.Image = newBmp;
				}
				else
                    m_MainForm.pictureBox.Image = m_AstroImage.Pixelmap.DisplayBitmap;
            }

		    if (!showFields)
		    {
		        using (Graphics g = Graphics.FromImage(m_MainForm.pictureBox.Image))
		        {
		            CompleteRenderFrame(g);
		            g.Save();
		        }
		    }
		    else
		    {
                // Still run custom renderers in Field mode. This is particularly useful for OCR troubleshooting
                if (HasCustomRenderers)
                {
                    using (Graphics g = Graphics.FromImage(m_MainForm.pictureBox.Image))
                    {
                        RunCustomRenderers(g, false);
                        g.Save();
                    }
                }                
		    }

			m_MainForm.pictureBox.Refresh();
		}

		internal void OverwriteCurrentFrame(Bitmap newBmp)
		{
			if (m_MainForm != null &&
				m_MainForm.pictureBox != null)
			{
				m_MainForm.pictureBox.Image = newBmp;
				m_MainForm.pictureBox.Invalidate();
			}
		}

		public void InvalidatePictureBox()
		{
			if (m_MainForm != null &&
			    m_MainForm.pictureBox != null)
			{
				m_MainForm.pictureBox.Invalidate();
			}
		}

		public bool IsRunning
		{
			get { return m_FramePlayer.IsRunning; }
		}

		public bool IsAstroDigitalVideo
		{
			get { return m_FramePlayer.IsAstroDigitalVideo; }
		}

		public bool IsAstroAnalogueVideo
		{
			get { return m_FramePlayer.IsAstroAnalogueVideo; }
		}

		public bool IsSerVideo
		{
			get { return m_FramePlayer.Video.Engine == "SER"; }
		}

	    public bool IsFitsSequence
	    {
            get { return m_FramePlayer.Video.Engine == FITSFileSequenceStream.FITS_SEQUENCE_ENGINE; }
	    }

        public bool IsFitsCube
        {
            get { return m_FramePlayer.Video.Engine == ThreeAxisFITSCubeFrameStream.CUBE_3D_FITS_FILE_ENGINE; }
        }

        public bool IsFitsFile
        {
            get { return m_FramePlayer.Video.Engine == SingleFITSFileFrameStream.SINGLE_FITS_FILE_ENGINE; }
        }

        public bool RequiresAcquisitionDelayCorrection
        {
            get
            {
                // Acquisition delay corrections are requried for ...
                return 
                    // ... any ser file
                    IsSerVideo || 
                    // ... any FITS sequence
                    IsFitsSequence ||
                    // .. any ADV version 2 file, which is not an AAV file
                    (IsAstroDigitalVideo && !IsAstroAnalogueVideo && m_FramePlayer.Video.Engine == "ADV2");
            }
        }

	    public double? KnownAcquisitionDelayMs
	    {
	        get
	        {
                if (m_FramePlayer.IsAstroAnalogueVideoV2 && m_FramePlayer.Video.Engine == "ADV2")
	            {
	                return ((AstroDigitalVideoStreamV2)m_FramePlayer.Video).KnownAcquisitionDelayMs;
	            }

	            return null;
	        }
	    }

        public bool SupportsSoftwareIntegration
        {
            get { return m_FramePlayer.Video.SupportsSoftwareIntegration; }
        }

        public bool SupportsIntegrationByMedian
        {
            get { return m_FramePlayer.Video.SupportsIntegrationByMedian; }
        }

	    public bool HasTimestampOCR()
	    {
            return m_TimestampOCR != null;
	    }

        public bool OCRTimeStampHasDatePart()
        {
            return m_TimestampOCR != null && m_TimestampOCR.TimeStampHasDatePart;
        }

		public bool HasEmbeddedTimeStamps()
		{
			return (IsAstroDigitalVideo && !IsAstroAnalogueVideo) ||
			       (IsAstroAnalogueVideo && AstroAnalogueVideoHasOcrOrNtpData) ||
                   (IsSerVideo && (((SERVideoStream)m_FramePlayer.Video).HasUTCTimeStamps || ((SERVideoStream)m_FramePlayer.Video).HasFireCaptureTimeStamps)) ||
                   ((m_FramePlayer.Video is IFITSStream) && ((IFITSStream)m_FramePlayer.Video).HasUTCTimeStamps);
		}

		public bool HasSystemTimeStamps()
		{
			return IsAstroDigitalVideo || IsAstroAnalogueVideo;
		}

		public bool IsPlainAviVideo
		{
			get
			{
				return 
					!m_FramePlayer.IsAstroDigitalVideo && 
					!m_FramePlayer.IsAstroAnalogueVideo &&
					m_FramePlayer.Video.Engine != "SER" && 
					m_FramePlayer.Video.FileName != null &&
					m_FramePlayer.Video.FileName.EndsWith(".AVI", StringComparison.InvariantCultureIgnoreCase);
			}
		}

		public bool IsAstroAnalogueVideoWithNtpTimestampsInNtpDebugMode
		{
			get
			{
				// We make it possible to OCR the timestamps in NTP debug mode, so NTP timestamps can be compared to the IOTA-VTI OSD timestamps
				// This is for testing purposes only and is not intended for production use!
				return
					m_FramePlayer.IsAstroAnalogueVideo &&
					m_FramePlayer.AstroAnalogueVideoHasNtpData &&
					!m_FramePlayer.AstroAnalogueVideoHasOcrData &&
					TangraConfig.Settings.AAV.NtpTimeDebugFlag &&
					m_FramePlayer.AstroAnalogueVideoIntegratedAAVFrames == 1;
			}
		}

		public bool IsVideoWithVtiOsdTimeStamp
		{
			get
			{
				return 
					IsPlainAviVideo || 
					IsAstroAnalogueVideoWithNtpTimestampsInNtpDebugMode ||
					(m_FramePlayer.IsAstroAnalogueVideo && Math.Abs(m_FramePlayer.AstroAnalogueVideoIntegratedAAVFrames) == 1) ||
                    (m_FramePlayer.IsAstroAnalogueVideoV2 && m_FramePlayer.AstroAnalogueVideoHasOcrCalibrationFrames) ||
					m_FramePlayer.Video.Engine == BMPFileSequenceStream.ENGINE;
			}
		}

		public void GetAdditionalAAVTimes(int frameNo, ref DateTime? frameMidTimeNTPRaw, ref DateTime? frameMidTimeNTPTangra, ref DateTime? frameMidTimeWindowsRaw)
		{
			FrameStateData stateData = m_FramePlayer.GetFrameStatusChannel(frameNo);

			if (stateData.AdditionalProperties != null)
			{
				frameMidTimeNTPRaw = (DateTime)stateData.AdditionalProperties["MidTimeNTPRaw"];
				frameMidTimeNTPTangra = (DateTime)stateData.AdditionalProperties["MidTimeNTPFitted"];
				frameMidTimeWindowsRaw = (DateTime)stateData.AdditionalProperties["MidTimeWindowsRaw"];
			}
		}

        public bool AstroAnalogueVideoHasOcrOrNtpData
        {
			get { return m_FramePlayer.AstroAnalogueVideoHasOcrData || m_FramePlayer.AstroAnalogueVideoHasNtpData; }
        }

		public int AstroAnalogueVideoNormaliseNtpDataIfNeeded(out float oneSigmaError)
		{
			if (m_FramePlayer.IsAstroAnalogueVideo && m_FramePlayer.AstroAnalogueVideoHasNtpData && !m_FramePlayer.AstroAnalogueVideoHasOcrData)
			{
				var frm = new frmBuildingNtpTimebase();
				try
				{
					frm.StartPosition = FormStartPosition.CenterParent;

					return m_FramePlayer.AstroAnalogueVideoNormaliseNtpDataIfNeeded((percDone) =>
						{
							if (percDone == 0)
							{
								frm.Show(m_MainForm);
							}

							if (percDone == 100)
							{
								frm.Close();
							}
							else
							{
								frm.SetProgress(percDone);
								frm.Update();
								Application.DoEvents();
							}
						},
						out oneSigmaError);
				}
				finally
				{
					if (frm.Visible)
						frm.Close();
				}
				
			}

			oneSigmaError = float.NaN;
			return - 1;
		}

        public int AstroAnalogueVideoIntegratedAAVFrames
        {
            get { return m_FramePlayer.AstroAnalogueVideoIntegratedAAVFrames; }
        }

        public int AstroAnalogueVideoStackedFrameRate
        {
            get { return m_FramePlayer.AstroAnalogueVideoStackedFrameRate; }
        }

	    public string AstroVideoCameraModel
	    {
			get { return m_FramePlayer.AstroVideoCameraModel; }
	    }

		public string AstroVideoNativeVideoStandard
		{
			get { return m_FramePlayer.AstroVideoNativeVideoStandard; }
		}

	    public GeoLocationInfo GeoLocation
	    {
			get
			{
				return m_FramePlayer.IsAstroDigitalVideo ? m_FramePlayer.GeoLocation : null;
			}
	    }

		public void MoveToFrame(int frameNo)
		{
			m_FramePlayer.MoveToFrame(frameNo);

            if (m_ShowFields)
                RedrawCurrentFrame(true);
		}

		public void StepBackward()
		{
			m_FramePlayer.StepBackward();
		}

		public void StepForward()
		{
			m_FramePlayer.StepForward();
		}

		public void StepBackward(int seconds)
		{
			m_FramePlayer.StepBackward(seconds);
		}

		public void StepForward(int seconds)
		{
			m_FramePlayer.StepForward(seconds);
		}

		public void InitializePlayingDirection(bool playBackwards)
		{
		    if (m_FramePlayer.InitializePlayingDirection(playBackwards))
		    {
		        RedrawCurrentFrame(false, true);
                RefreshCurrentFrame();
		    }
		}

		public void PlayVideo(int? startAtFrame = null, uint step = 1)
		{
			m_FramePlayer.Start(FramePlaySpeed.Fastest, startAtFrame, step);

		    TangraContext.Current.OperationInProgress = true;

			m_VideoFileView.Update();
		}

        public int CurrentFrameIndex
        {
            get { return m_FramePlayer.CurrentFrameIndex; }
        }

		public void StopVideo(Action<int, bool> callback = null)
		{
			if (callback != null)
				m_OnStoppedCallbacks.Add(callback);

			m_FramePlayer.Stop();

			OnVideoPlayerStopped();

			m_VideoFileView.Update();
		}

	    private bool savedCanScrollFramesStateOnVideoStarted = false;

		private void OnVideoPlayerStarted()
		{
			savedCanScrollFramesStateOnVideoStarted = TangraContext.Current.CanScrollFrames;
			TangraContext.Current.CanScrollFrames = false;
		}

		private void OnVideoPlayerStopped()
		{
			TangraContext.Current.CanScrollFrames = savedCanScrollFramesStateOnVideoStarted;
            TangraContext.Current.OperationInProgress = false;
		}
        
	    private bool CanApplyDisplayModeAdjustmentsNow()
	    {
            if (m_CurrentOperation != null &&
                m_CurrentOperation is ReduceLightCurveOperation &&
	            ((ReduceLightCurveOperation) m_CurrentOperation).HasStackedSelectionFrame)
	        {
                // When using a stacked frame selection the DisplayModeAdjustments are applied when building the statacked selection frame
                // not when the frame already exists
	            return false;
	        }

	        return true;
	    }

	    private void NotifyOperationOfFrameDetailsChange()
	    {
	        if (m_CurrentOperation != null &&
	            m_CurrentOperation is ReduceLightCurveOperation &&
	            ((ReduceLightCurveOperation) m_CurrentOperation).IsStackedSelectionEnabled)
	        {
                // Something has changed, make sure we re-render the stacked frame as well
                m_CurrentOperation.DisplayIntensifyModeChanged();
	        }
	    }

		public void RefreshCurrentFrame()
		{
		    NotifyOperationOfFrameDetailsChange();

			m_FramePlayer.RefreshCurrentFrame();
		}

		private Control m_WinControl;
		private IVideoFrameRenderer m_FrameRenderer;

		public void InitVideoSystem(PlayerContext playerContext)
		{
			m_WinControl = playerContext.MainThreadControl;
			m_FrameRenderer = playerContext.FrameRenderer;

			m_FramePlayer.SetFrameRenderer(this);

            TangraVideo.SetVideoEngine(TangraConfig.Settings.Generic.AviRenderingEngineIndex);

			m_VideoFileView.Reset();
		}

		public void Dispose()
		{
			m_FramePlayer.DisposeResources();
			m_FramePlayer = null;
		}

		public void UpdateViews()
		{
			m_VideoFileView.Update();

			m_ImageToolView.Update(m_ImageTool);

			//if (TangraConfig.Settings.Generic.PerformanceQuality == Tangra.Model.Config.TangraConfig.PerformanceQuality.Responsiveness)
			//{
			//    if (m_CurrentFrameContext.CurrentFrameIndex % 12 == 0)
			//    {
			//        statusStrip.Invalidate();
			//        pnlControlerPanel.Invalidate();
			//    }
			//    m_ZoomedImageView.Invalidate();
			//}
		}

		public void UIThreadInvoke(Action callback)
		{
			m_MainForm.Invoke(new Action(callback));
		}

        public void UpdateZoomedImage(Bitmap zoomedBitmap, ImagePixel center, Pixelmap displayPixelmap = null)
        {
			ApplyDisplayModeAdjustments(zoomedBitmap, false, displayPixelmap);

            m_ZoomedImageView.UpdateImage(zoomedBitmap);

			ZoomedCenter = new ImagePixel(center);
        }

        public void ClearZoomedImage()
        {
            m_ZoomedImageView.ClearZoomedImage();
	        ZoomedCenter = null;
        }

		internal void ClearControlPanel()
		{
			m_pnlControlerPanel.Controls.Clear();
		}

		internal Panel ControlerPanel
		{
			get { return m_pnlControlerPanel;}
		}

		internal void SetPictureBoxCursor(Cursor cursor)
		{
			if (m_MainForm != null)
				m_MainForm.pictureBox.Cursor = cursor;
		}

		internal void SetCursor(Cursor cursor)
		{
			if (m_MainForm != null)
				m_MainForm.Cursor = cursor;
		}

        public AstroImage GetCurrentAstroImage(bool integrated)
        {
            if (integrated && m_FramePlayer.Video.SupportsSoftwareIntegration)
            {
				// NOTE: This only seems to be used by AddEditTarget form in a case of no wind and shaking (which is the case most of the times)
                Pixelmap image = m_FramePlayer.GetIntegratedFrame(m_CurrentFrameContext.CurrentFrameIndex, TangraConfig.Settings.Special.AddStarImageFramesToIntegrate, true /* 'true'so we start from the current frame */, false);

                return new AstroImage(image);
            }
            else
			{
				// Don't integrate if the wind or shaking flag is set. Faint stars are unlikely to be seen anyway 
				// with integration in a case of a good shake
				return m_AstroImage;
			}
        }

	    public Bitmap StackFrames(int stackedFrames)
	    {
	        try
	        {
                var frameNo = Math.Max(FramePlayer.CurrentFrameIndex, FramePlayer.Video.FirstFrame);
	            stackedFrames = Math.Min(stackedFrames, FramePlayer.Video.CountFrames);

                var intPixMap = FramePlayer.GetIntegratedFrame(frameNo, stackedFrames, false, false);
                ApplyDisplayModeAdjustments(intPixMap.DisplayBitmap, true, intPixMap);

                if (!TangraConfig.Settings.HelpFlags.DontShowStackedSelectionHelpFormAgain)
                {
                    TangraConfig.Settings.HelpFlags.DontShowStackedSelectionHelpFormAgain = true;
                    TangraConfig.Settings.Save();

                    var frm = new frmStackedSelectionHelpNote();
                    frm.StartPosition = FormStartPosition.CenterParent;
                    ShowForm(frm);
                }

                return intPixMap.DisplayBitmap;
	        }
	        catch (Exception ex)
	        {
                Trace.WriteLine(ex);
	            return null;
	        }
	    }

		internal Pixelmap GetFrame(int frameId)
		{
			return m_FramePlayer.GetFrame(frameId, true);
		}

		internal FrameStateData GetFrameStateData(int frameId)
		{
			return m_FramePlayer.GetFrameStatusChannel(frameId);
		}
		public FrameStateData GetCurrentFrameState()
		{
			return m_FrameState;
		}

		void IVideoFrameRenderer.PlayerStarted()
		{
			OnVideoPlayerStarted();

			try
			{
				m_WinControl.Invoke(new FramePlayer.SimpleDelegate(m_FrameRenderer.PlayerStarted));
			}
			catch (ObjectDisposedException)
			{ }
            catch (InvalidAsynchronousStateException)
            { }
			catch (InvalidOperationException)
			{ }

			UpdateViews();
		}

		void IVideoFrameRenderer.PlayerStopped(int lastDisplayedFrame, bool userStopRequest)
		{
			OnVideoPlayerStopped();

			try
			{
				//Trace.WriteLine(string.Format("FramePlayer: Stoped at frame {0}. User requested stop: {1}", lastDisplayedFrame, userStopRequest));

				foreach (Action<int, bool> callback in m_OnStoppedCallbacks)
				{
					try
					{
						callback(lastDisplayedFrame, userStopRequest);
					}
					catch (Exception ex)
					{
						Trace.WriteLine(ex);
					}
				}
				m_OnStoppedCallbacks.Clear();

				m_WinControl.Invoke(new Action<int, bool>(m_FrameRenderer.PlayerStopped), new object[] { lastDisplayedFrame, userStopRequest});
			}
			catch (ObjectDisposedException)
			{ }
            catch(InvalidAsynchronousStateException)
            { }
			catch (InvalidOperationException)
			{ }

			UpdateViews();
		}

        void IVideoFrameRenderer.RenderFrame(int currentFrameIndex, Pixelmap currentPixelmap, MovementType movementType, bool isLastFrame, int msToWait, int firstFrameInIntegrationPeriod, string frameFileName)
		{
			try
			{
				m_WinControl.Invoke(
					new RenderFrameCallback(m_FrameRenderer.RenderFrame),
					new object[]
                            {
                                currentFrameIndex,
                                currentPixelmap,
                                movementType,
                                isLastFrame,
                                msToWait,
                                firstFrameInIntegrationPeriod,
                                frameFileName
                            });
			}
			catch (ObjectDisposedException)
			{ }
            catch (InvalidAsynchronousStateException)
            { }
			catch (InvalidOperationException)
			{ }
		}

		public void ApplyDisplayModeAdjustments(Bitmap displayBitmap, bool enableBackgroundGlow = false, Pixelmap displayPixelmap = null)
        {
		    if (!CanApplyDisplayModeAdjustmentsNow())
		    {
		        return;
		    }

			if (m_DisplayIntensifyMode != DisplayIntensifyMode.Off || m_DisplayInvertedMode || m_DisplayHueIntensityMode || m_DisplayHueBackgroundMode)
            {
                // For display purposes only we apply display gamma and/or invert when requested by the user

				if (enableBackgroundGlow && m_DisplayHueBackgroundMode)
				{
					BitmapFilter.ProcessHueBackgroundMode(displayBitmap, m_HBMTarget1X, m_HBMTarget1Y, m_HBMTarget2X, m_HBMTarget2Y, m_HBMTarget3X, m_HBMTarget3Y, m_HBMTarget4X, m_HBMTarget4Y);
				}
				else if (m_DisplayIntensifyMode == DisplayIntensifyMode.Dynamic && m_DynamicToValue - m_DynamicFromValue > 0 && displayPixelmap != null)
                    BitmapFilter.ApplyDynamicRange(displayBitmap, displayPixelmap, m_DynamicFromValue, m_DynamicToValue, m_DisplayInvertedMode, m_DisplayHueIntensityMode, EffectiveMaxPixelValue);
				else if (m_DisplayIntensifyMode != DisplayIntensifyMode.Off)
					BitmapFilter.ApplyGamma(displayBitmap, m_DisplayIntensifyMode == DisplayIntensifyMode.Hi, m_DisplayInvertedMode, m_DisplayHueIntensityMode);
                else if (m_DisplayInvertedMode || m_DisplayHueIntensityMode)
                    BitmapFilter.ProcessInvertAndHueIntensity(displayBitmap, m_DisplayInvertedMode, m_DisplayHueIntensityMode);
            }
        }

	    public void ApplyDynamicRangeAdjustments(Bitmap displayBitmap, Pixelmap displayPixelmap)
	    {
	        if (m_DisplayIntensifyMode == DisplayIntensifyMode.Dynamic && m_DynamicToValue - m_DynamicFromValue > 0)
                BitmapFilter.ApplyDynamicRange(displayBitmap, displayPixelmap, m_DynamicFromValue, m_DynamicToValue, m_DisplayInvertedMode, m_DisplayHueIntensityMode, EffectiveMaxPixelValue);
	    }

		public void SetDisplayIntensifyMode(DisplayIntensifyMode newMode, int? dynamicFromValue, int?  dynamicToValue, bool refresh = true)
		{
			m_DisplayIntensifyMode = newMode;
			if (dynamicFromValue.HasValue && dynamicToValue.HasValue)
			{
				m_DynamicFromValue = dynamicFromValue.Value;
				m_DynamicToValue = dynamicToValue.Value;
				m_DynamicMaxPixelValue = EffectiveMaxPixelValue;
			}
			else if (m_DynamicMaxPixelValue != EffectiveMaxPixelValue)
			{
				m_DynamicFromValue = 0;
				m_DynamicToValue = (int)EffectiveMaxPixelValue;
				m_DynamicMaxPixelValue = EffectiveMaxPixelValue;
			}

			if (newMode != DisplayIntensifyMode.Dynamic)
			{
				TangraConfig.Settings.Generic.UseDisplayIntensifyMode = newMode;
				TangraConfig.Settings.Save();
			}

			if (newMode == DisplayIntensifyMode.Dynamic) UsageStats.Instance.CustomDynamicRangeUsed++;
			if (newMode == DisplayIntensifyMode.Hi) UsageStats.Instance.HighGammaModeUsed++;
			if (newMode == DisplayIntensifyMode.Lo) UsageStats.Instance.LowGammaModeUsed++;
			if (newMode == DisplayIntensifyMode.Off) UsageStats.Instance.NoGammaModeUsed++;
			UsageStats.Instance.Save();

			if (refresh && 
                !m_FramePlayer.IsRunning &&
				m_FramePlayer.Video != null)
			{
                RefreshCurrentFrame();
			}
		}

		public void SetDisplayInvertMode(bool inverted)
		{
			m_DisplayInvertedMode = inverted;
			if (inverted)
			{
				m_DisplayHueBackgroundMode = false;
				m_MainForm.miJupiterGlow.Checked = false;

				UsageStats.Instance.InvertedModeUsed++;
				UsageStats.Instance.Save();
			}

			TangraConfig.Settings.Generic.UseInvertedDisplayMode = inverted;
			TangraConfig.Settings.Save();

			if (!m_FramePlayer.IsRunning &&
				m_FramePlayer.Video != null)
			{
                RefreshCurrentFrame();
			}
		}

        public void SetDisplayHueMode(bool hueSelected)
        {
            m_DisplayHueIntensityMode = hueSelected;

	        if (hueSelected)
	        {
		        m_DisplayHueBackgroundMode = false;
		        m_MainForm.miJupiterGlow.Checked = false;

				UsageStats.Instance.HueIntensityModeUsed++;
				UsageStats.Instance.Save();
	        }

	        TangraConfig.Settings.Generic.UseHueIntensityDisplayMode = hueSelected;
	        TangraConfig.Settings.Save();

            if (!m_FramePlayer.IsRunning &&
                m_FramePlayer.Video != null)
            {
                RefreshCurrentFrame();
            }
        }

		public void SetDisplayHueBackgroundMode(bool hueBackgroundMode)
		{
			m_DisplayHueBackgroundMode = hueBackgroundMode;

			if (m_DisplayHueBackgroundMode)			
			{
				// When the Hue Background Mode is turned ON we turn off everything else
				m_DisplayHueIntensityMode = false;
				m_MainForm.tsmiHueIntensity.Checked = false;
				m_DisplayInvertedMode = false;
				m_MainForm.tsmiInverted.Checked = false;
				m_DisplayIntensifyMode = DisplayIntensifyMode.Off;
                m_MainForm.tsmiOff.Checked = true;
                m_MainForm.tsmiLo.Checked = false;
                m_MainForm.tsmiHigh.Checked = false;
                m_MainForm.tsmiDynamic.Checked = false;

				TangraConfig.Settings.Generic.UseHueIntensityDisplayMode = false;
				TangraConfig.Settings.Generic.UseInvertedDisplayMode = false;
				TangraConfig.Settings.Generic.UseDisplayIntensifyMode = DisplayIntensifyMode.Off;
				TangraConfig.Settings.Save();
			}

			if (!m_FramePlayer.IsRunning &&
				m_FramePlayer.Video != null)
			{
                RefreshCurrentFrame();
			}
		}

		internal void SetDisplayHueBackgroundModeTargets(List<TrackedObjectConfig> trackedObjects)
	    {
			if (trackedObjects.Count > 0)
			{
				m_HBMTarget1X = trackedObjects[0].OriginalFieldCenterX;
				m_HBMTarget1Y = trackedObjects[0].OriginalFieldCenterY;
			}
			if (trackedObjects.Count > 1)
			{
				m_HBMTarget2X = trackedObjects[1].OriginalFieldCenterX;
				m_HBMTarget2Y = trackedObjects[1].OriginalFieldCenterY;
			}
			if (trackedObjects.Count > 2)
			{
				m_HBMTarget3X = trackedObjects[2].OriginalFieldCenterX;
				m_HBMTarget3Y = trackedObjects[2].OriginalFieldCenterY;
			}
			if (trackedObjects.Count > 3)
			{
				m_HBMTarget4X = trackedObjects[3].OriginalFieldCenterX;
				m_HBMTarget4Y = trackedObjects[3].OriginalFieldCenterY;
			}

			if (!m_FramePlayer.IsRunning &&
				m_FramePlayer.Video != null)
			{
                RefreshCurrentFrame();
			}
	    }
	    

		public void SetDisplayHueBackgroundModeTargets(List<ITrackedObject> trackedObjectsPositions)
		{
			if (trackedObjectsPositions.Count > 0 && trackedObjectsPositions[0].Center != null)
			{
				m_HBMTarget1X = trackedObjectsPositions[0].Center.X;
				m_HBMTarget1Y = trackedObjectsPositions[0].Center.Y;
			}
			if (trackedObjectsPositions.Count > 1 && trackedObjectsPositions[1].Center != null)
			{
				m_HBMTarget2X = trackedObjectsPositions[1].Center.X;
				m_HBMTarget2Y = trackedObjectsPositions[1].Center.Y;
			}
			if (trackedObjectsPositions.Count > 2 && trackedObjectsPositions[2].Center != null)
			{
				m_HBMTarget3X = trackedObjectsPositions[2].Center.X;
				m_HBMTarget3Y = trackedObjectsPositions[2].Center.Y;
			}
			if (trackedObjectsPositions.Count > 3 && trackedObjectsPositions[3].Center != null)
			{
				m_HBMTarget4X = trackedObjectsPositions[3].Center.X;
				m_HBMTarget4Y = trackedObjectsPositions[3].Center.Y;
			}

			if (!m_FramePlayer.IsRunning &&
				m_FramePlayer.Video != null)
			{
                RefreshCurrentFrame();
			}
		}

		public void ToggleAstroVideoStatusForm()
		{
			ToggleAstroVideoStatusForm(false);
		}

		public void ToggleSerStatusForm()
		{
			ToggleSerStatusForm(false);
		}

	    public void ToggleFitsStatusForm()
	    {
            ToggleFitsStatusForm(false);
	    }

		private void HideAdvStatusForm()
		{
			if (m_AdvStatusForm != null && m_AdvStatusForm.Visible)
				m_AdvStatusForm.Hide();
		}

		private void HideAavStatusForm()
		{
			if (m_AavStatusForm != null && m_AavStatusForm.Visible)
				m_AavStatusForm.Hide();			
		}

		private void HideSerStatusForm()
		{
			if (m_SerStatusForm != null && m_SerStatusForm.Visible)
				m_SerStatusForm.Hide();
		}

	    private void HideFitsStatusForm()
	    {
            if (m_FitsStatusForm != null && m_FitsStatusForm.Visible)
                m_FitsStatusForm.Hide();	        
	    }

        private void HideQhyHeaderForm()
        {
            if (m_QhyHeaderForm != null && m_QhyHeaderForm.Visible)
                m_QhyHeaderForm.Hide();
        }

		private void ToggleAstroVideoStatusForm(bool forceShow)
		{
			if (IsAstroDigitalVideo)
			{
				HideAavStatusForm();

				if (m_AdvStatusForm == null)
				{
					m_AdvStatusForm = new frmAdvStatusPopup(TangraConfig.Settings.ADVS);
					m_AdvStatusForm.Show(m_MainFormView);
					PositionAdvStatusForm();
					m_AdvStatusForm.ShowStatus(m_FrameState);
				}
				else if (!m_AdvStatusForm.Visible)
				{
					try
					{
						m_AdvStatusForm.Show(m_MainFormView);
					}
					catch (ObjectDisposedException)
					{
						m_AdvStatusForm = new frmAdvStatusPopup(TangraConfig.Settings.ADVS);
						m_AdvStatusForm.Show(m_MainFormView);
					}

					PositionAdvStatusForm();
					m_AdvStatusForm.ShowStatus(m_FrameState);
				}
				else if (!forceShow)
				{
					HideAdvStatusForm();
				}
				else
				{
					PositionAdvStatusForm();
					m_AdvStatusForm.ShowStatus(m_FrameState);
				}
			}
			else if (IsAstroAnalogueVideo)
			{
				HideAdvStatusForm();

				if (m_AavStatusForm == null)
				{
					m_AavStatusForm = new frmAavStatusPopup(TangraConfig.Settings.AAV);
					m_AavStatusForm.Show(m_MainFormView);
					PositionAavStatusForm();
					m_AavStatusForm.ShowStatus(m_FrameState);
				}
				else if (!m_AavStatusForm.Visible)
				{
					try
					{
						m_AavStatusForm.Show(m_MainFormView);
					}
					catch (ObjectDisposedException)
					{
						m_AavStatusForm = new frmAavStatusPopup(TangraConfig.Settings.AAV);
						m_AavStatusForm.Show(m_MainFormView);
					}

					PositionAdvStatusForm();
					m_AavStatusForm.ShowStatus(m_FrameState);
				}
				else if (!forceShow)
				{
					HideAavStatusForm();
				}
				else
				{
					PositionAavStatusForm();
					m_AavStatusForm.ShowStatus(m_FrameState);
				}				
			}
		}

		private void PositionAavStatusForm()
		{
			if (m_AavStatusForm != null &&
				m_AavStatusForm.Visible)
			{
				m_AavStatusForm.Left = m_MainFormView.Right;
				m_AavStatusForm.Top = m_MainFormView.Top;
			}			
		}

		private void PositionAdvStatusForm()
		{
			if (m_AdvStatusForm != null &&
				m_AdvStatusForm.Visible)
			{
				m_AdvStatusForm.Left = m_MainFormView.Right;
				m_AdvStatusForm.Top = m_MainFormView.Top;
			}
		}

		private void PositionSerStatusForm()
		{
			if (m_SerStatusForm != null &&
				m_SerStatusForm.Visible)
			{
				m_SerStatusForm.Left = m_MainFormView.Right;
				m_SerStatusForm.Top = m_MainFormView.Top;
			}
		}

        private void PositionFitsStatusForm()
        {
            if (m_FitsStatusForm != null &&
                m_FitsStatusForm.Visible)
            {
                m_FitsStatusForm.Left = m_MainFormView.Right;
                m_FitsStatusForm.Top = m_MainFormView.Top;
            }
        }

		private void ToggleSerStatusForm(bool forceShow)
		{
			HideSerStatusForm();

			if (m_SerStatusForm == null)
			{
				m_SerStatusForm = new frmSerStatusPopup(TangraConfig.Settings.SER);
				m_SerStatusForm.Show(m_MainFormView);
				PositionSerStatusForm();
				m_SerStatusForm.ShowStatus(m_FrameState);
			}
			else if (!m_SerStatusForm.Visible)
			{
				try
				{
					m_SerStatusForm.Show(m_MainFormView);
				}
				catch (ObjectDisposedException)
				{
					m_SerStatusForm = new frmSerStatusPopup(TangraConfig.Settings.SER);
					m_SerStatusForm.Show(m_MainFormView);
				}

				PositionSerStatusForm();
				m_SerStatusForm.ShowStatus(m_FrameState);
			}
			else if (!forceShow)
			{
				HideSerStatusForm();
			}
			else
			{
				PositionSerStatusForm();
				m_SerStatusForm.ShowStatus(m_FrameState);
			}
		}

        private void ToggleFitsStatusForm(bool forceShow)
        {
            HideFitsStatusForm();

            if (m_FitsStatusForm == null)
            {
                m_FitsStatusForm = new frmFitsHeaderPopup();
                m_FitsStatusForm.Show(m_MainFormView);
                PositionFitsStatusForm();
                m_FitsStatusForm.ShowStatus(m_FrameState);
            }
            else if (!m_FitsStatusForm.Visible)
            {
                try
                {
                    m_FitsStatusForm.Show(m_MainFormView);
                }
                catch (ObjectDisposedException)
                {
                    m_FitsStatusForm = new frmFitsHeaderPopup();
                    m_FitsStatusForm.Show(m_MainFormView);
                }

                PositionFitsStatusForm();
                m_FitsStatusForm.ShowStatus(m_FrameState);
            }
            else if (!forceShow)
            {
                HideFitsStatusForm();
            }
            else
            {
                PositionFitsStatusForm();
                m_FitsStatusForm.ShowStatus(m_FrameState);
            }
        }

        public void TogglePSFViewerForm()
        {
            TogglePSFViewerForm(false);
        }

        public void TogglePSFViewerForm(bool forceShow)
        {
            if (m_TargetPSFViewerForm == null)
                m_TargetPSFViewerForm = new frmTargetPSFViewerForm(this);

            if (m_FramePlayer.Video != null && !m_TargetPSFViewerForm.Visible)
            {
                try
                {
                    m_TargetPSFViewerForm.Show(m_MainFormView);
                }
                catch (ObjectDisposedException)
                {
                    m_TargetPSFViewerForm = new frmTargetPSFViewerForm(this);
                    m_TargetPSFViewerForm.Show(m_MainFormView);
                }

                PositionTargetPSFViewerForm();

				ShowTargetPSF();

				UsageStats.Instance.TargetPSFViewerFormShown++;
				UsageStats.Instance.Save();
            }
            else
            {
                m_TargetPSFViewerForm.Hide();
            }            
        }


        private void PositionTargetPSFViewerForm()
        {
            if (m_TargetPSFViewerForm != null &&
                m_TargetPSFViewerForm.Visible)
            {
                m_TargetPSFViewerForm.Left = m_MainFormView.Right;
                m_TargetPSFViewerForm.Top = m_MainFormView.Top + (m_MainFormView.Height - m_TargetPSFViewerForm.Height);
            }
        }

        private void ShowTargetPSF()
        {
            if (m_TargetPSFViewerForm != null && m_FramePlayer.Video != null)
				m_TargetPSFViewerForm.ShowTargetPSF(m_TargetPsfFit, m_FramePlayer.Video.BitPix, m_FramePlayer.Video.GetAav16NormVal(), m_TargetBackgroundPixels);
        }

        public void ToggleQhyHeaderForm()
        {
            ToggleQhyHeaderForm(false);
        }

        public void ToggleQhyHeaderForm(bool forceShow)
        {
            if (m_QhyHeaderForm == null)
                m_QhyHeaderForm = new frmQhyHeaderViewer(this);

            if (m_FramePlayer.Video != null && !m_QhyHeaderForm.Visible)
            {
                try
                {
                    m_QhyHeaderForm.Show(m_MainFormView);
                }
                catch (ObjectDisposedException)
                {
                    m_QhyHeaderForm = new frmQhyHeaderViewer(this);
                    m_QhyHeaderForm.Show(m_MainFormView);
                }

                PositionQhyHeaderViewerForm();

                ShowQhyImageHeader();
            }
            else
            {
                m_QhyHeaderForm.Hide();
            }
        }

        private void PositionQhyHeaderViewerForm()
        {
            if (m_QhyHeaderForm != null &&
                m_QhyHeaderForm.Visible)
            {
                m_QhyHeaderForm.Left = m_MainFormView.Right;
                m_QhyHeaderForm.Top = m_MainFormView.Top + (m_MainFormView.Height - m_QhyHeaderForm.Height);
            }
        }

        private void ShowQhyImageHeader()
        {
            if (m_QhyHeaderForm != null && m_FramePlayer.Video != null)
                m_QhyHeaderForm.ShowImageHeader(m_AstroImage);
        }

		public void ShowFSTSFileViewer()
		{
			UsageStats.Instance.FSTSFileViewerInvoked++;
			UsageStats.Instance.Save();

			if (TangraContext.Current.HasVideoLoaded && (m_FramePlayer.IsAstroDigitalVideo || m_FramePlayer.IsAstroAnalogueVideo))
			{
			    if (ShowMessageBox("This operation will close the file opened in Tangra. Do you wish to continue?", "Tangra", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			    {
			        var fileName = m_FramePlayer.Video.FileName;

                    CloseOpenedVideoFile();

                    ChooseAdvViewerForFileVersion(fileName);			        
			    }
			}
            else
			{
			    if (m_MainForm.openAdvFileDialog.ShowDialog(m_MainFormView) == DialogResult.OK)
			    {
                    ChooseAdvViewerForFileVersion(m_MainForm.openAdvFileDialog.FileName);
			    }
			}
		}

	    private void ChooseAdvViewerForFileVersion(string fileName)
	    {
	        int fileVersion = TangraCore.ADV2GetFormatVersion(fileName);
	        if (fileVersion == 1)
	        {
                var viewer = new frmAdvViewer(fileName);
	            viewer.Show(m_MainFormView);
	        }
            else if (fileVersion == 2)
            {
                var viewer = new frmAdv2Viewer(fileName);
                viewer.Show(m_MainFormView);
            }
	    }

		public void RepairAdvFile(string fileName)
		{
			var frmRebuilder = new frmAdvIndexRebuilder(fileName);
			if (frmRebuilder.ShowDialog(m_MainFormView) == DialogResult.OK &&
				File.Exists(frmRebuilder.NewFileName))
			{
				OpenVideoFile(frmRebuilder.NewFileName);
			}		
		}

		public void NotifyMainFormMoved()
		{
			PositionAdvStatusForm();
            PositionAavStatusForm();
            PositionSerStatusForm();
		    PositionTargetPSFViewerForm();
            PositionFitsStatusForm();
            PositionQhyHeaderViewerForm();
		}

		public bool IsAdvStatusFormVisible
		{
			get
			{
				return m_AdvStatusForm != null && m_AdvStatusForm.Visible;
			}			
		}

		public IAdvStatusPopupFormCustomizer AdvStatusPopupFormCustomizer
		{
			get { return m_AdvStatusForm; }
		}

		public bool IsAavStatusFormVisible
		{
			get
			{
				return m_AavStatusForm != null && m_AavStatusForm.Visible;
			}
		}

		public IAavStatusPopupFormCustomizer AavStatusPopupFormCustomizer
		{
			get { return m_AavStatusForm; }
		}

	    public bool IsTargetPSFViewerFormVisible
	    {
            get
            {
                return m_TargetPSFViewerForm != null && m_TargetPSFViewerForm.Visible;
            }
	    }

        public bool IsQhyHeaderFormVisible
        {
            get
            {
                return m_QhyHeaderForm != null && m_QhyHeaderForm.Visible;
            }
        }

        public bool IsSerStatusFormVisible
        {
            get
            {
                return m_SerStatusForm != null && m_SerStatusForm.Visible;
            }
        }

        public bool IsFitsStatusFormVisible
        {
            get
            {
                return m_FitsStatusForm != null && m_FitsStatusForm.Visible;
            }
        }

		public ImageTool SelectImageTool<TImageTool>() where TImageTool : ImageTool, new()
        {
            if (m_ImageTool != null) m_ImageTool.Deactivate();

			m_ImageTool = ImageTool.SwitchTo<TImageTool>(m_CurrentOperation, m_ImageToolView, m_ImageTool);

			return m_ImageTool;
        }

		public bool ActivateOperation<TOperation>(params object[] constructorParams) where TOperation : class, IVideoOperation, new()
		{
			return ActivateOperation<TOperation>(true, constructorParams);
		}

		private bool ActivateOperation<TOperation>(
			bool checkPrevOperationActive, /* We need this because for Astrometry the calibration is chosen earlier and check should be done before this */
			params object[] constructorParams) where TOperation : class, IVideoOperation, new()
		{
			if (checkPrevOperationActive && m_CurrentOperation != null && m_CurrentOperation.GetType() != typeof(ReduceLightCurveOperation))
			{
				if (MessageBox.Show(
						m_MainFormView,
						"There is another operation active. Continue?",
						"Question",
						MessageBoxButtons.YesNo,
						MessageBoxIcon.Question) == DialogResult.No)
				{
					return false;
				}
			}

			IVideoOperation oldOperation = m_CurrentOperation;

			DeselectFeature();

			if (typeof (TOperation) == typeof (ReduceLightCurveOperation))
			{
				// Don't close the light curve form, when a light curve viewing operation
			}
			else
				m_LightCurveController.EnsureLightCurveFormClosed();

			m_pnlControlerPanel.Controls.Clear();

			try
			{
				m_CurrentOperation = CreateOperation<TOperation>(constructorParams);

				if (!m_CurrentOperation.InitializeOperation(this, m_pnlControlerPanel, m_FramePlayer, m_MainFormView))
				{
					m_CurrentOperation = oldOperation;
					StatusChanged("Ready");
					return false;
				}
				else
				{
					var operation = m_CurrentOperation as IRequiresAddinsController;
					if (operation != null)
						operation.SetAddinsController(m_AddinsController);

					return true;
				}
			}
			finally
			{
				m_VideoFileView.Update();
				TangraContext.Current.CrashReportInfo.VideoOperation = m_CurrentOperation != null ? m_CurrentOperation.GetType().Name : null;
			}
		}

		private IVideoOperation CreateOperation<TOperation>(params object[] constructorParams) where TOperation : class, IVideoOperation, new()
		{
			if (constructorParams != null && constructorParams.Length > 0)
			{
				Type[] types = new Type[constructorParams.Length];
				for (int i = 0; i < constructorParams.Length; i++)
				{
					types[i] = constructorParams[i].GetType();
				}

				ConstructorInfo ci = typeof(TOperation).GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, types, null);
				return (TOperation)ci.Invoke(constructorParams);
			}
			else
				return new TOperation();			
		}

		internal IVideoOperation SetOperation<TOperation>(params object[] constructorParams) where TOperation : class, IVideoOperation, new()
		{
			m_CurrentOperation = CreateOperation<TOperation>(constructorParams);
			return m_CurrentOperation;
		}

		private void DeselectFeature()
		{

		}

		internal void EnsureLightCurveForm()
		{
			EnsureAllPopUpFormsClosed();
			m_LightCurveController.EnsureLightCurveForm();
		}

		private void EnsureAllPopUpFormsClosed()
		{
			if (m_LightCurveController != null)
				m_LightCurveController.EnsureLightCurveFormClosed();

			try
			{
				if (m_AdvStatusForm != null)
				{
					m_AdvStatusForm.Close();
				}
			}
			finally
			{
				m_AdvStatusForm = null;
			}

			try
			{
				if (m_AavStatusForm != null)
				{
					m_AavStatusForm.Close();
				}
			}
			finally
			{
				m_AavStatusForm = null;
			}

            try
            {
                if (m_SerStatusForm != null)
                {
                    m_SerStatusForm.Close();
                }
            }
            finally
            {
                m_SerStatusForm = null;
            }

            try
            {
                if (m_FitsStatusForm != null)
                {
                    m_FitsStatusForm.Close();
                }
            }
            finally
            {
                m_FitsStatusForm = null;
            }

			try
			{
				if (m_TargetPSFViewerForm != null)
				{
					m_TargetPSFViewerForm.Close();
				}
			}
			finally
			{
				m_TargetPSFViewerForm = null;
			}
		}

	    public ImagePixel ZoomedCenter = new ImagePixel(-1, -1);

		public void MouseClick(Point location, MouseEventArgs e)
        {
            bool shiftHeld = Control.ModifierKeys == Keys.Shift;
            bool controlHeld = Control.ModifierKeys == Keys.Control;

            var pixel = new ImagePixel(location.X, location.Y);
            m_TargetPsfFit = null;

            if (shiftHeld)
            {
                // No fitting when Shift is pressed
            }
            else
            {
                // Ctrl means fit in smaller area
                int matrixSize = controlHeld ? 7 : 17;

                if (m_AstroImage != null)
                {
                    uint[,] data = m_AstroImage.GetMeasurableAreaPixels(pixel.X, pixel.Y, matrixSize);
                    m_TargetPsfFit = new PSFFit(pixel.X, pixel.Y);
					m_TargetPsfFit.Fit(data);
                    if (m_TargetPsfFit.IsSolved)
                    {
                        pixel = new ImagePixel(m_TargetPsfFit.Brightness, m_TargetPsfFit.XCenter, m_TargetPsfFit.YCenter);
                        //isFit = psfFit.Certainty > 0.25;
                    }
					m_TargetBackgroundPixels = m_AstroImage.GetMeasurableAreaPixels(pixel.X, pixel.Y, 35);
                }
            }

            var args = new ObjectClickEventArgs(
                pixel, 
				m_TargetPsfFit, 
				location,
                Control.ModifierKeys == Keys.Shift, 
				Control.ModifierKeys == Keys.Control,
				e);

	        bool drawStandardZoomImade = true;
			
			ZoomedCenter = new ImagePixel(pixel);

	        if (m_CurrentOperation != null)
	        {
		        m_CurrentOperation.MouseClick(args);

				if (m_ZoomedImageView != null && m_CurrentOperation.HasCustomZoomImage)
					drawStandardZoomImade = !m_ZoomedImageView.DrawCustomZoomImage(m_CurrentOperation);
	        }

	        if (m_ImageTool != null)
	        {
		        m_ImageTool.MouseClick(args);

		        if (drawStandardZoomImade)
			        drawStandardZoomImade = m_ImageTool.ZoomBehaviour == ZoomImageBehaviour.DisplayCentroid;
	        }

	        if (m_ZoomedImageView != null && drawStandardZoomImade)
			{
				if (m_AstroImage != null)
				{
				    Pixelmap zoomPixelmap;
                    Bitmap zoomedBmp = m_AstroImage.GetZoomImagePixels(pixel.X, pixel.Y, TangraConfig.Settings.Color.Saturation, TangraConfig.Settings.Photometry.Saturation, out zoomPixelmap);
                    UpdateZoomedImage(zoomedBmp, pixel, zoomPixelmap);

				}
				else
					m_ZoomedImageView.ClearZoomedImage();
			}

            ShowTargetPSF();
        }

        public void MouseDoubleClick(Point location)
        {
            if (m_CurrentOperation != null)
                m_CurrentOperation.MouseDoubleClick(location);

            if (m_ImageTool != null)
                m_ImageTool.MouseDoubleClick(location);
        }

        public void MouseDown(Point location)
        {
            if (m_CurrentOperation != null)
                m_CurrentOperation.MouseDown(location);

            if (m_ImageTool != null)
                m_ImageTool.MouseDown(location);
        }

        public void MouseLeave()
        {
            if (m_CurrentOperation != null)
                m_CurrentOperation.MouseLeave();

            if (m_ImageTool != null)
                m_ImageTool.MouseLeave();
        }

        public void MouseMove(Point location)
        {
            if (m_CurrentOperation != null)
                m_CurrentOperation.MouseMove(location);

            if (m_ImageTool != null)
                m_ImageTool.MouseMove(location);
        }

        public void MouseUp(Point location)
        {
            if (m_CurrentOperation != null)
                m_CurrentOperation.MouseUp(location);

            if (m_ImageTool != null)
                m_ImageTool.MouseUp(location);
        }

        public void StatusChanged(string displayName)
        {
            m_VideoFileView.StatusChanged(displayName);
        }

		internal void NotifyFileProgress(int current, int max)
		{
			m_VideoFileView.OnFileProgress(current, max);
		}

		public void NotifyBeginLongOperation()
		{
			m_VideoFileView.BeginLongOperation();
		}

		public void NotifyEndLongOperation()
		{
			m_VideoFileView.EndLongOperation();
		}

		public DialogResult ShowMessageBox(string message, string title, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
		{
			return MessageBox.Show(m_MainFormView, message, title, buttons, icon, defaultButton);
		}

        public DialogResult ShowMessageBox(string message, string title, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            return MessageBox.Show(m_MainFormView, message, title, buttons, icon);
        }

		public DialogResult ShowDialog(Form frm)
		{
			return frm.ShowDialog(m_MainFormView);
		}

        public void ShowForm(Form frm)
        {
            frm.Show(m_MainFormView);
            frm.Top = m_MainFormView.Top + (m_MainFormView.Height - frm.Height) / 2;
            frm.Left = m_MainFormView.Left + (m_MainFormView.Width - frm.Width) / 2;
        }

		public DialogResult ShowSaveFileDialog(string title, string filter, ref string fileName, IWin32Window ownerWindow = null)
		{
			var sfd = new SaveFileDialog()
			{
				Title = title,
				FileName = fileName,
				Filter = filter				
			};

			DialogResult rv = sfd.ShowDialog(ownerWindow ?? m_MainFormView);

			fileName = sfd.FileName;
			return rv;
		}

		public DialogResult ShowOpenFileDialog(string title, string filter, out string fileName)
		{
			var ofd = new OpenFileDialog()
			{
				Title = title,
				Filter = filter
			};

			DialogResult rv = ofd.ShowDialog(m_MainFormView);

			fileName = ofd.FileName;
			return rv;
		}

	    public DialogResult ShowBrowseFolderDialog(string title, string selectedFolder, out string folderPath)
	    {
            var frm = new frmChooseFolder(title, selectedFolder);
	        frm.StartPosition = FormStartPosition.CenterParent;
	        DialogResult rv = frm.ShowDialog(m_MainFormView);
	        folderPath = frm.SelectedFolderPath;
	        return rv;
	    }

		public void ShowTangraSettingsDialog(bool showCatalogRequiredHint, bool showLocationRequiredHint)
		{
			m_MainForm.ShowSettings(showCatalogRequiredHint, showLocationRequiredHint);
		}

        public void ExportToFits(Pixelmap pixelmap)
        {
            string filter;

            if (pixelmap.BitPixCamera == 8)
            {
                filter = "FITS Image 8 bit (*.fit;*.fits)|*.fit;*.fits";
            }
            else if (pixelmap.BitPixCamera <= 16)
            {
                filter = "FITS Image 16 bit (*.fit;*.fits)|*.fit;*.fits";
            }
            else
            {
                filter = "FITS Image 32 bit (*.fit;*.fits)|*.fit;*.fits";
            }

            string fileName = string.Empty;
            var sfd = new SaveFileDialog()
            {
                Title = "Export video frame as FITS image ...",
                FileName = fileName,
                Filter = filter,
                DefaultExt = "fit"
            };

            DialogResult rv = sfd.ShowDialog(m_MainFormView);

            if (rv == DialogResult.OK)
            {
				UsageStats.Instance.ExportToFITSUsed++;
				UsageStats.Instance.Save();

                fileName = sfd.FileName;

                bool exportAs8BitFloat = false;
                DateTime timeStamp;
                float exposure;

                int integrationRate = 0;
                if (this.IsAstroAnalogueVideo)
                    integrationRate = this.AstroAnalogueVideoIntegratedAAVFrames;
                
                var videoFormat = this.GetVideoFileFormat();
                var nativeFormat = this.GetVideoFormat(videoFormat);
                var videoCamera = this.AstroVideoCameraModel;
                var normalValue = this.EffectiveMaxPixelValue;

                if (videoFormat == VideoFileFormat.AAV || videoFormat == VideoFileFormat.AAV2 || videoFormat == VideoFileFormat.AVI)
                {
                    if (this.VideoBitPix == 8 || this.EffectiveMaxPixelValue > 255)
                    {
                        // For video from analogue cameras we export as 8-bit floating point numbers
                        exportAs8BitFloat = true;
                    }
                }

                var additionalFileHeaders = GetAdditionalFileHeaders();

                string dateObsComment = null;
                if (this.HasEmbeddedTimeStamps())
                {
                    dateObsComment = GetEmbeddedDateTimeComment();
                    timeStamp = pixelmap.FrameState.CentralExposureTime;
                    exposure = pixelmap.FrameState.ExposureInMilliseconds / 1000.0f;
                }
                else
                {
                    dateObsComment = "No Date and Time available";
                    timeStamp = DateTime.MinValue;
                    exposure = 0;
                }

                ConvertVideoToFitsController.SaveFitsFrame(
                    m_CurrentFrameId, 
                    fileName,
                    pixelmap.Width,
                    pixelmap.Height,
                    pixelmap.Pixels,
                    timeStamp,
                    exposure,
                    dateObsComment,
                    exportAs8BitFloat,
                    normalValue,
                    videoFormat,
                    nativeFormat, 
                    integrationRate,
                    videoCamera,
                    this.FileName,
                    additionalFileHeaders);
            }
        }

		public void RegisterOcrError()
        {
            TangraContext.Current.OcrErrors++;
            m_VideoFileView.Update();
        }

        public void RegisterExtractingOcrTimestamps()
        {
            if (!TangraContext.Current.OcrExtractingTimestamps)
            {
                TangraContext.Current.OcrErrors = 0;
                TangraContext.Current.AstrometryOCRFailedRead = 0;
                TangraContext.Current.AstrometryOCRDroppedFrames = 0;
                TangraContext.Current.AstrometryOCRDuplicatedFrames = 0;
                TangraContext.Current.AstrometryOCRTimeErrors = 0; 
                TangraContext.Current.OcrExtractingTimestamps = true;
                m_VideoFileView.Update();
            }
        }

        public void RegisterAAVConversionError()
        {
            TangraContext.Current.AAVConvertErrors++;
            m_VideoFileView.Update();
        }

        public void ClearAAVConversionErrors()
        {
            TangraContext.Current.AAVConvertErrors = 0;
            m_VideoFileView.Update();
        }

		public void ShowFileInformation()
		{
			if (m_FramePlayer.Video != null)
			{
				UsageStats.Instance.FileInformationMenuUsed++;
				UsageStats.Instance.Save();

				var frm = new frmFileInformation(m_FramePlayer.Video);
				frm.StartPosition = FormStartPosition.CenterParent;
				frm.ShowDialog(m_MainForm);
			}
		}

		internal Point GetImageCoordinatesFromZoomedImage(Point zoomedImageMousePos)
		{
			if (ZoomedCenter != null)
			{
				return new Point(ZoomedCenter.X - 15 + (zoomedImageMousePos.X / 8),
								 ZoomedCenter.Y - 15 + (zoomedImageMousePos.Y / 8));
			}
			return new Point(0, 0);
		}

		internal void DisplayCursorPositionDetails(Point imagePos, string moreInfo = null)
		{
			bool hintVisible = false;
			string hint = string.Empty;

			if (TangraConfig.Settings.Generic.ShowCursorPosition &&
				imagePos.X >= 0 &&
				imagePos.Y >= 0)
			{
				if (m_AstroImage != null)
				{
					uint pixval = m_AstroImage.GetPixel(imagePos.X, imagePos.Y);
					hint = string.Format("({0}, {1})={2}", imagePos.X, imagePos.Y, pixval);
					hintVisible = true;
				}
				else
				{
					hint = string.Format("X={0} Y={1}", imagePos.X, imagePos.Y);
					hintVisible = true;
				}
				
			}
			
			if (!string.IsNullOrEmpty(moreInfo))
			{
				hint = hint.TrimEnd(' ') + " " + moreInfo;
				hintVisible = true;
			}
			
			m_MainForm.ssMoreInfo.Visible = hintVisible;
			if (hintVisible)
				m_MainForm.ssMoreInfo.Text = hint;
		}

		internal void ChangeImageTool(ImageTool newTool)
		{
			ImageTool oldImageTool = m_ImageTool;

			m_ImageTool = newTool;
			if (!m_ImageTool.TrySwitchTo(m_CurrentOperation, m_ImageToolView, oldImageTool))
			{
				m_ImageTool = oldImageTool;
			}

			//HandleToolSelection();
		}

		internal ImageTool CurrentImageTool 
		{
			get { return m_ImageTool; }
		}

		int IImagePixelProvider.Width
		{
			get { return m_FramePlayer.Video.Width; }
		}

		int IImagePixelProvider.Height
		{
			get { return m_FramePlayer.Video.Height; }
		}

	    int IImagePixelProvider.FirstFrame
	    {
            get { return m_FramePlayer.Video.FirstFrame; }
	    }

		int IImagePixelProvider.LastFrame
		{
			get { return m_FramePlayer.Video.LastFrame; }
		}

		int[,] IImagePixelProvider.GetPixelArray(int frameNo, Rectangle rect)
		{
			if (rect.Width != 32 || rect.Height != 32)
				throw new NotSupportedException();

			int[,] rv = new int[32, 32];

			Pixelmap pixelmap = m_FramePlayer.GetFrame(frameNo, true);

			for (int x = 0; x < 32; x++)
			for (int y = 0; y < 32; y++)
			{
				rv[x, y] = (int)pixelmap.Pixels[(x + rect.Left) + (y + rect.Top) * pixelmap.Width];
			}

			return rv;
		}

		public void SetFlipSettings(bool flipVertically, bool flipHorizontally)
		{
            m_FramePlayer.SetFlipSettings(flipVertically, flipHorizontally);
		}

		public delegate void RenderOverlayCallback(Graphics g, bool isLastFrame);

	    internal bool DebugOCR;

	    internal int OCRScore;
	    internal int OCRScoredFrames;

	    internal void RegisterOverlayRenderer(RenderOverlayCallback callback)
		{
			m_CustomOverlayRenderer = callback;
		}

		internal void DeregisterOverlayRenderer(RenderOverlayCallback callback)
		{
			m_CustomOverlayRenderer = null;
		}

        internal void ShutdownVideo()
        {
            if (m_FramePlayer.IsRunning)
            {
                m_FramePlayer.Stop();
                int counter = 0;
                while (m_FramePlayer.IsRunning && counter < 20)
                {
                    Thread.Sleep(50);
                    counter++;
                }
            }
        }

        private static string s_AngleParserRegex = @"[^\d \.\-\+]";

	    internal Dictionary<string, string> GetVideoFileTags()
	    {
	        var rv = new Dictionary<string, string>();

	        var aavStream = m_FramePlayer.Video as AstroDigitalVideoStream;
            if (aavStream != null)
            {
                rv.Add("ObjectName", aavStream.GetFileTag("OBJECT"));
                rv.Add("Telescope", aavStream.GetFileTag("TELESCOP"));
                rv.Add("Instrument", aavStream.GetFileTag("CAMERA-MODEL"));
                rv.Add("Recorder", aavStream.GetFileTag("RECORDER"));
                rv.Add("Observer", aavStream.GetFileTag("OBSERVER"));

                
                try
                {
                    string raStr = aavStream.GetFileTag("RA_OBJ");
                    double ra = AstroConvert.ToRightAcsension(Regex.Replace(raStr, s_AngleParserRegex, ""));
                    rv.Add("RA", ra.ToString(CultureInfo.InvariantCulture));
                }
                catch
                { }

                try
                {
                    string deStr = aavStream.GetFileTag("DEC_OBJ");
                    double dec = AstroConvert.ToDeclination(Regex.Replace(deStr, s_AngleParserRegex, ""));
                    rv.Add("DEC", dec.ToString(CultureInfo.InvariantCulture));
                }
                catch
                { }

                try
                {
                    string lngStr = aavStream.GetFileTag("LONGITUDE");
                    double lng = AstroConvert.ToDeclination(Regex.Replace(lngStr, s_AngleParserRegex, ""));
                    rv.Add("Longitude", lng.ToString(CultureInfo.InvariantCulture));
                }
                catch
                { }

                try
                {
                    string latStr = aavStream.GetFileTag("LATITUDE");
                    double lat = AstroConvert.ToDeclination(Regex.Replace(latStr, s_AngleParserRegex, ""));
                    rv.Add("Latitude", lat.ToString(CultureInfo.InvariantCulture));
                }
                catch
                { }               
            }

	        return rv;
	    }

	    private static Regex s_RegexSourceToFileFormat = new Regex("^(?<format>(ADV2?|AAV2?|AVI|SER|FITS))\\..+$");

		/// <summary>
		/// AVI|AAV|ADV|SER|FITS
		/// </summary>
		/// <returns></returns>
		internal VideoFileFormat GetVideoFileFormat()
		{
			if (m_FramePlayer.Video != null &&
				m_FramePlayer.Video.VideoFileType != null)
			{
				Match match = s_RegexSourceToFileFormat.Match(m_FramePlayer.Video.VideoFileType);
				if (match.Success &&
					match.Groups["format"] != null &&
					match.Groups["format"].Success)
				{
					try
					{
						return (VideoFileFormat)Enum.Parse(typeof(VideoFileFormat), match.Groups["format"].Value);
					}
					catch (FormatException)
					{ }
				}
			}
			return VideoFileFormat.Unknown;
		}

		/// <summary>
		/// PAL|NTSC|Digital
		/// </summary>
		/// <returns></returns>
		internal string GetVideoFormat(VideoFileFormat videoFileFormat)
		{
            if (videoFileFormat == VideoFileFormat.ADV || videoFileFormat == VideoFileFormat.ADV2 || videoFileFormat == VideoFileFormat.SER)
				return "Digital";
            if (videoFileFormat.IsAAV())
				return  AstroVideoNativeVideoStandard;
			else if (!double.IsNaN(m_FramePlayer.Video.FrameRate))
			{
				if (Math.Abs(25 - m_FramePlayer.Video.FrameRate) < 1)
					return "PAL";
				else if (Math.Abs(29.97 - m_FramePlayer.Video.FrameRate) < 1)
					return "NTSC";
			}

			return "";
		}

		internal DateTime? GetCurrentFrameTime()
		{
			FrameStateData frameState = GetCurrentFrameState();

			if (frameState.HasValidNtpTimeStamp)
			{
				float oneSigma;
				AstroAnalogueVideoNormaliseNtpDataIfNeeded(out oneSigma);
			}

            if (frameState.HasValidTimeStamp &&
                frameState.CentralExposureTime != DateTime.MaxValue &&
			    frameState.CentralExposureTime != DateTime.MinValue)
				return frameState.CentralExposureTime;

		    if (HasTimestampOCR())
		    {
		        var ocredTimestamp = OCRTimestamp();
		        if (ocredTimestamp != DateTime.MinValue)
		            return ocredTimestamp;
		    }

            if (frameState.HasValidNtpTimeStamp)
                return frameState.EndFrameNtpTime;

		    return null;
		}

		internal void RotateVideo()
		{
			m_FramePlayer.RotateVideo(45);
		}

        public string GetVideoFileMatchingLcFile(string fileName, string searchPath)
        {
            if (File.Exists(fileName))
                return fileName;


            string nextGuess = Path.GetFullPath(Path.GetDirectoryName(searchPath) + "\\" + Path.GetFileName(fileName));
            if (File.Exists(nextGuess))
                return nextGuess;

            nextGuess =
                Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + "\\" + Path.GetFileName(fileName));
            if (File.Exists(nextGuess))
                return nextGuess;

            return null;
        }

        public string FileName
        {
            get { return TangraContext.Current.FileName; }
        }

	    public bool EnsureNotADuplicatedFrame()
	    {
            var dupFrameChecker = new DuplicateFrameAvoider(this, CurrentFrameIndex);
            if (dupFrameChecker.IsDuplicatedFrame())
            {
                int nextFollowingGoodFrameId = dupFrameChecker.GetFirstGoodFrameId();

                if (ShowMessageBox(
                    string.Format("The current frame appears to be a duplicate. Press 'OK' for Tangra to move to the next good frame (Frame: {0})", nextFollowingGoodFrameId),
                    "Tangra",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    MoveToFrame(nextFollowingGoodFrameId);
                    return true;
                }
                else
                    return false;
            }

	        return true;
	    }

	    private static Regex FILE_NAME_DATE_REGEX = new Regex(@"^(\d{4}\-\w+\-\d+)");

	    public DateTime GetBestGuessDateTimeForCurrentFrame(DateTime? defaultValue = null)
	    {
	        TimeSpan ocrTimeOfDay = TimeSpan.Zero;
	        if (HasTimestampOCR())
	        {
                var ocrTime = OCRTimestamp();
	            if (OCRTimeStampHasDatePart())
	                return ocrTime;

	            ocrTimeOfDay = ocrTime.TimeOfDay;
	        }

	        var currFrameTime = GetCurrentFrameTime();
	        if (currFrameTime != null)
	        {
	            var dt = currFrameTime.Value;
	            if (dt.Year <= 1)
	            {
	                dt = DateTime.Today.Add(ocrTimeOfDay);
	            }
	            return dt;
	        }

	        if (IsAstroAnalogueVideo || IsAstroDigitalVideo)
	        {
	            var currFrameState = GetCurrentFrameState();

	            if (currFrameState.HasValidTimeStamp)
	                return currFrameState.CentralExposureTime.AddMilliseconds(-0.5*currFrameState.ExposureInMilliseconds);

	            if (currFrameState.HasValidSystemTimeStamp)
	                return currFrameState.SystemTime;

	            if (currFrameState.HasValidNtpTimeStamp)
	                return currFrameState.EndFrameNtpTime.AddMilliseconds(-1*currFrameState.ExposureInMilliseconds);
	        }

	        if (m_FramePlayer != null &&
	            m_FramePlayer.Video != null &&
	            m_FramePlayer.Video.FileName != null)
	        {
	            string fileName = Path.GetFileName(m_FramePlayer.Video.FileName);

	            var match = FILE_NAME_DATE_REGEX.Match(fileName);
                if (match.Success)
                {
                    string dateString = match.Groups[1].Value;
                    DateTime dateVal;
                        
                    if (DateTime.TryParse(dateString, CultureInfo.CurrentCulture, DateTimeStyles.RoundtripKind, out dateVal))
                        return dateVal.Date.Add(ocrTimeOfDay);

                    if (DateTime.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out dateVal))
                        return dateVal.Date.Add(ocrTimeOfDay);
                }

                m_TimestampOCR = null;
	        }

            return defaultValue.HasValue ? defaultValue.Value : DateTime.UtcNow;
	    }

        public Dictionary<string, Tuple<string, string>> GetAdditionalFileHeaders()
	    {
            var additionalFileHeaders = new Dictionary<string, Tuple<string, string>>();

            var fileHeaderProvider = this.FramePlayer.Video as IFileHeaderProvider;
            if (fileHeaderProvider != null)
            {
                var AAV_HEADERS = new Dictionary<string, string>
                {
                    {"LATITUDE", "LATITUDE" },
                    {"LONGITUDE", "LONGITUD" },
                    {"OBSERVER", "OBSERVER" }, 
                    {"TELESCOP", "TELESCOP" },
                    {"OBJECT", "OBJECT"},
                    {"RA_OBJ", "RA_OBJ" },
                    {"DEC_OBJ", "DEC_OBJ"},
                    {"RECORDER-SOFTWARE", "REC-SOFT" },
                    {"RECORDER-SOFTWARE-VERSION", "REC-VER" },
                    {"ADVLIB-VERSION", "ADVLIB"}
                };

                var videoFormat = this.GetVideoFileFormat();
                var nativeFormat = this.GetVideoFormat(videoFormat);
                var videoCamera = this.AstroVideoCameraModel;

                var coppiedFromHeaderDescription = string.Format("Copied from {0} file headers", videoFormat);
                var fileHeaders = fileHeaderProvider.GetFileHeaders();
                fileHeaders
                    .Where(kvp => AAV_HEADERS.ContainsKey(kvp.Key))
                    .Select(kvp => kvp)
                    .ToList()
                    .ForEach(kvp => additionalFileHeaders[AAV_HEADERS[kvp.Key]] = Tuple.Create(kvp.Value, coppiedFromHeaderDescription));

                if (videoFormat == VideoFileFormat.AAV || videoFormat == VideoFileFormat.AAV2)
                {
                    var camera = videoCamera;
                    if (camera.IndexOf(nativeFormat, StringComparison.InvariantCultureIgnoreCase) == -1)
                    {
                        camera = camera.Trim() + string.Format(" ({0})", nativeFormat);
                    }
                    var instumentalDelaySelectedConfig = InstrumentalDelayConfigManager.GetConfigurationForCamera(camera);
                    float instDelay;
                    if (instumentalDelaySelectedConfig.TryGetValue(this.AstroAnalogueVideoIntegratedAAVFrames, out instDelay))
                    {
                        additionalFileHeaders["INSTDELY"] = Tuple.Create(
                            instDelay.ToString(CultureInfo.InvariantCulture),
                            string.Format("Instr. delay in sec. for x{0} frames integration for '{1}' camera. This has not been applied to DATE-OBS", this.AstroAnalogueVideoIntegratedAAVFrames, camera));
                    }
                }
            }

            return additionalFileHeaders;
	    }

	    public string GetEmbeddedDateTimeComment()
	    {
	        string dateObsComment = null;
            if (this.HasEmbeddedTimeStamps())
            {
                var videoFormat = this.GetVideoFileFormat();

                if (this.HasTimestampOCR())
                {
                    if (this.OCRTimeStampHasDatePart())
                        dateObsComment = "Date and Time are ORC-ed";
                    else
                        dateObsComment = "Time is ORC-ed, Date is user entered";
                }
                else if (videoFormat == VideoFileFormat.ADV || videoFormat == VideoFileFormat.ADV2)
                {
                    dateObsComment = "Date and Time are mid-frame recorded";
                }
                else if (!this.IsFitsSequence)
                {
                    dateObsComment = "Date and Time are saved by recorder";
                }
            }

            return dateObsComment;
	    }
    }
}
