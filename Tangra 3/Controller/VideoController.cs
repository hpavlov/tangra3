﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Tangra.Helpers;
using Tangra.Video.FITS;
using Tangra.Video.SER;
using Tangra.VideoOperations.Astrometry;
using Tangra.VideoOperations.LightCurves.Tracking;
using Tangra.VideoTools;
using nom.tam.fits;
using nom.tam.util;
using Tangra.ImageTools;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Model.Context;
using Tangra.Model.Image;
using Tangra.Model.ImageTools;
using Tangra.Model.Video;
using Tangra.Model.VideoOperations;
using Tangra.PInvoke;
using Tangra.Video;
using Tangra.Video.AstroDigitalVideo;
using Tangra.VideoOperations.LightCurves;
using Tangra.VideoOperations.MakeDarkFlatField;
using Tangra.View;
using Cursor = System.Windows.Forms.Cursor;

namespace Tangra.Controller
{
	public class VideoController : IDisposable, IVideoFrameRenderer, IVideoController, IImagePixelProvider
	{
		private VideoFileView m_VideoFileView;
        private ZoomedImageView m_ZoomedImageView;
	    private ImageToolView m_ImageToolView;

		private Form m_MainFormView;
	    private Panel m_pnlControlerPanel;

		private IFramePlayer m_FramePlayer;

		private FrameStateData m_FrameState;

		private frmAdvStatusPopup m_AdvStatusForm;
	    private frmAavStatusPopup m_AavStatusForm;
	    private frmSerStatusPopup m_SerStatusForm;
	    private frmTargetPSFViewerForm m_TargetPSFViewerForm;

		private AdvOverlayManager m_OverlayManager = new AdvOverlayManager();

        private AstroImage m_AstroImage;
        private RenderFrameContext m_CurrentFrameContext;

        private ImageTool m_ImageTool;
        private IVideoOperation m_CurrentOperation;

	    private PSFFit m_TargetPsfFit;
	    private uint[,] m_TargetBackgroundPixels;

	    private LightCurveController m_LightCurveController;
		private AddinsController m_AddinsController;
	    private frmMain m_MainForm;

		private RenderOverlayCallback m_CustomOverlayRenderer;

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

				m_MainForm.pnlControlerPanel.Controls.Clear();
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
			m_CurrentFrameContext = RenderFrameContext.Empty;

            TangraCore.PreProcessors.ClearAll();

			UpdateViews();
		}

		internal bool SingleBitmapFile(LCFile lcFile)
		{
			return OpenVideoFileInternal(
				null, 
				() => new SingleBitmapFileFrameStream(lcFile));
		}

		public bool OpenFitsFileSequence(string folderName)
        {
            string[] fitsFiles = Directory.GetFiles(folderName, "*.fit*", SearchOption.TopDirectoryOnly);
            if (fitsFiles.Length == 0)
            {
                ShowMessageBox("No FITS files found inside " + folderName, "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else
            {
                var frm = new frmSortFitsFiles();
                frm.SetFiles(fitsFiles);
                frm.StartPosition = FormStartPosition.CenterParent;
                frm.ShowDialog(m_MainForm);
                fitsFiles = frm.GetSortedFiles();

                return OpenVideoFileInternal(
                    folderName,
                    () =>
                    {
                        return FITSFileSequenceStream.OpenFolder(fitsFiles);
                    });
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

	    public bool OpenVideoFile(string fileName)
	    {
			string fileExtension = Path.GetExtension(fileName);

	        if (fileExtension != null) 
                fileExtension = fileExtension.ToLower();

		    return OpenVideoFileInternal(
				fileName, 
				() =>
				{
					IFrameStream frameStream = null;

                    try
                    {
                        if (fileExtension == ".adv" || fileExtension == ".aav")
                        {
                            AdvEquipmentInfo equipmentInfo;
                            GeoLocationInfo geoLocation;
                            frameStream = AstroDigitalVideoStream.OpenFile(fileName, out equipmentInfo, out geoLocation);
                            if (frameStream != null)
                            {
                                TangraContext.Current.UsingADV = true;
                                m_OverlayManager.InitAdvFile(equipmentInfo, geoLocation, frameStream.FirstFrame);
                            }
                        }
                        else if (fileExtension == ".ser")
                        {
                            SerEquipmentInfo equipmentInfo;
                            frameStream = SERVideoStream.OpenFile(fileName, m_MainForm, out equipmentInfo);
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
                            frameStream = SingleFITSFileFrameStream.OpenFile(fileName);
                        }
                        else
                        {
                            frameStream = VideoStream.OpenFile(fileName);
                        }

                        return frameStream;
                    }
                    catch (IOException ioex)
                    {
                        MessageBox.Show(ioex.Message, "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        return null;
                    }					
				});
	    }

		public bool OpenVideoFileInternal(string fileName, Func<IFrameStream> frameStreamFactoryMethod)
		{
			TangraContext.Current.UsingADV = false;
			TangraContext.Current.IsSerFile = false;
			TangraContext.Current.FileName = null;
			TangraContext.Current.FileFormat = null;
			TangraContext.Current.HasVideoLoaded = false;
			m_OverlayManager.Reset();

		    TangraContext.Current.CrashReportInfo.VideoFile = fileName;

			TangraCore.PreProcessors.ClearAll();
			TangraCore.PreProcessors.AddGammaCorrection(TangraConfig.Settings.Photometry.EncodingGamma);

			IFrameStream frameStream = frameStreamFactoryMethod();

		    if (frameStream != null)
			{
				m_FramePlayer.OpenVideo(frameStream);

				if (!string.IsNullOrEmpty(fileName))
				{
					TangraContext.Current.FileName = Path.GetFileName(fileName);
					TangraContext.Current.FileFormat = frameStream.VideoFileType;
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
                {
                    SetDisplayIntensifyMode(DisplayIntensifyMode.Dynamic, (int)fitsSteream.MinPixelValue, (int)fitsSteream.MaxPixelValue);
                    m_MainForm.tsmiOff.Checked = false;
                    m_MainForm.tsmiLo.Checked = false;
                    m_MainForm.tsmiHigh.Checked = false;
                    m_MainForm.tsmiDynamic.Checked = true;
                }

				m_VideoFileView.Update();

				if (
					(IsAstroDigitalVideo && !IsAdvStatusFormVisible) ||
					(IsAstroAnalogueVideo && !IsAavStatusFormVisible))
				{
					ToggleAstroVideoStatusForm(true);
				}

				if (!string.IsNullOrEmpty(fileName))
					RegisterRecentFile(RecentFileType.Video, fileName);

				m_ImageTool = ImageTool.SwitchTo<ArrowTool>(null, m_ImageToolView, m_ImageTool);


				return true;
			}
			else
			{
				// TODO: Show error message	
			}

			return false;
		}

		internal void RegisterRecentFile(RecentFileType type, string fileName)
		{
			TangraConfig.Settings.RecentFiles.NewRecentFile(type, fileName);
			TangraConfig.Settings.Save();

			m_MainForm.BuildRecentFilesMenu();
		}

        public void SetImage(Pixelmap currentPixelmap, RenderFrameContext frameContext, bool isOldFrameRefreshed)
		{
            m_AstroImage = new AstroImage(currentPixelmap);
            m_CurrentFrameContext = frameContext;
            m_FrameState = currentPixelmap.FrameState;

			if (m_AdvStatusForm != null && m_AdvStatusForm.Visible)
				m_AdvStatusForm.ShowStatus(m_FrameState);

			if (m_AavStatusForm != null && m_AavStatusForm.Visible)
				m_AavStatusForm.ShowStatus(m_FrameState);

			if (m_SerStatusForm != null && m_SerStatusForm.Visible)
				m_SerStatusForm.ShowStatus(m_FrameState);

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
		    get { return m_FramePlayer.Video.FileName; }
	    }

		public string CurrentVideoFileType
	    {
		    get { return m_FramePlayer.Video.VideoFileType; }
	    }

		public string CurrentVideoFileEngine
		{
			get { return m_FramePlayer.Video.Engine; }
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

		public void OverlayStateForFrame(Bitmap displayBitmap, int frameId)
		{
            if (m_CurrentOperation != null &&
                m_CurrentOperation.AvoidImageOverlays)
            {
                // The current operation doesn't want overlays displayed
            }
            else
				m_OverlayManager.OverlayStateForFrame(displayBitmap, m_FrameState, frameId, IsAstroDigitalVideo, IsAstroAnalogueVideo);
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
        }

		internal void RunCustomRenderers(Graphics g)
		{
			if (m_CustomOverlayRenderer != null && m_CurrentOperation == null)
			{
				// Run custom overlay renderers when no operation is selected
				try
				{
					m_CustomOverlayRenderer(g);
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

		public void RedrawCurrentFrame(bool showFields, bool reloadImage = false)
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
					// NOTE: Applying pre-processing here will break the Astrometry overlays
					m_MainForm.pictureBox.Image = m_AstroImage.Pixelmap.CreateNewDisplayBitmap();
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
			get { return m_FramePlayer.Video.Engine == "FITS-SEQ"; }
	    }

        public bool SupportsSoftwareIntegration
        {
            get { return m_FramePlayer.Video.SupportsSoftwareIntegration; }
        }

		public bool HasEmbeddedTimeStamps()
		{
			return IsAstroDigitalVideo ||
			       (IsAstroAnalogueVideo && AstroAnalogueVideoHasOcrOrNtpData) ||
				   (IsSerVideo && ((SERVideoStream)m_FramePlayer.Video).HasUTCTimeStamps) ||
				   (IsFitsSequence && ((FITSFileSequenceStream)m_FramePlayer.Video).HasUTCTimeStamps);
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

		public int AstroAnalogueVideoNormaliseNtpDataIfNeeded()
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
						});
				}
				finally
				{
					if (frm.Visible)
						frm.Close();
				}
				
			}

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

		public void PlayVideo(int? startAtFrame = null, uint step = 1)
		{
			m_FramePlayer.Start(FramePlaySpeed.Fastest, startAtFrame, step);

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
		}

		public void RefreshCurrentFrame()
		{
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

        public void UpdateZoomedImage(Bitmap zoomedBitmap, ImagePixel center)
        {
			ApplyDisplayModeAdjustments(zoomedBitmap);

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
			};
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
			catch (InvalidOperationException)
			{ }
		}

		public void ApplyDisplayModeAdjustments(Bitmap displayBitmap, bool enableBackgroundGlow = false, Pixelmap displayPixelmap = null)
        {
			if (m_DisplayIntensifyMode != DisplayIntensifyMode.Off || m_DisplayInvertedMode || m_DisplayHueIntensityMode || m_DisplayHueBackgroundMode)
            {
                // For display purposes only we apply display gamma and/or invert when requested by the user

				if (enableBackgroundGlow && m_DisplayHueBackgroundMode)
				{
					BitmapFilter.ProcessHueBackgroundMode(displayBitmap, m_HBMTarget1X, m_HBMTarget1Y, m_HBMTarget2X, m_HBMTarget2Y, m_HBMTarget3X, m_HBMTarget3Y, m_HBMTarget4X, m_HBMTarget4Y);
				}
				else if (m_DisplayIntensifyMode == DisplayIntensifyMode.Dynamic && m_DynamicToValue - m_DynamicFromValue > 0 && displayPixelmap != null)
                    BitmapFilter.ApplyDynamicRange(displayBitmap, displayPixelmap, m_DynamicFromValue, m_DynamicToValue, m_DisplayInvertedMode, m_DisplayHueIntensityMode);
				else if (m_DisplayIntensifyMode != DisplayIntensifyMode.Off)
					BitmapFilter.ApplyGamma(displayBitmap, m_DisplayIntensifyMode == DisplayIntensifyMode.Hi, m_DisplayInvertedMode, m_DisplayHueIntensityMode);
                else if (m_DisplayInvertedMode || m_DisplayHueIntensityMode)
                    BitmapFilter.ProcessInvertAndHueIntensity(displayBitmap, m_DisplayInvertedMode, m_DisplayHueIntensityMode);
            }            
        }


		public void SetDisplayIntensifyMode(DisplayIntensifyMode newMode, int? dynamicFromValue, int?  dynamicToValue)
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

			if (!m_FramePlayer.IsRunning &&
				m_FramePlayer.Video != null)
			{
				m_FramePlayer.RefreshCurrentFrame();
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
				m_FramePlayer.RefreshCurrentFrame();
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
                m_FramePlayer.RefreshCurrentFrame();
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
				m_FramePlayer.RefreshCurrentFrame();
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
				m_FramePlayer.RefreshCurrentFrame();
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
				m_FramePlayer.RefreshCurrentFrame();
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

		public void ShowFSTSFileViewer()
		{
			UsageStats.Instance.FSTSFileViewerInvoked++;
			UsageStats.Instance.Save();

			if (TangraContext.Current.HasVideoLoaded && (m_FramePlayer.IsAstroDigitalVideo || m_FramePlayer.IsAstroAnalogueVideo))
			{
				var viewer = new frmAdvViewer(m_FramePlayer.Video.FileName);
				viewer.Show(m_MainFormView);
			}
            else
			{
			    if (m_MainForm.openAdvFileDialog.ShowDialog(m_MainFormView) == DialogResult.OK)
			    {
                    var viewer = new frmAdvViewer(m_MainForm.openAdvFileDialog.FileName);
                    viewer.Show(m_MainFormView);
			    }
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
		    PositionTargetPSFViewerForm();
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

        public void MouseClick(Point location)
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
                pixel, m_TargetPsfFit, location,
                Control.ModifierKeys == Keys.Shift, Control.ModifierKeys == Keys.Control);

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
					Bitmap zoomedBmp = m_AstroImage.GetZoomImagePixels(pixel.X, pixel.Y, TangraConfig.Settings.Color.Saturation, TangraConfig.Settings.Photometry.Saturation);
					UpdateZoomedImage(zoomedBmp, pixel);

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

		public DialogResult ShowSaveFileDialog(string title, string filter, ref string fileName)
		{
			var sfd = new SaveFileDialog()
			{
				Title = title,
				FileName = fileName,
				Filter = filter				
			};

			DialogResult rv = sfd.ShowDialog(m_MainFormView);

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

		public void ShowTangraSettingsDialog(bool showCatalogRequiredHint, bool showLocationRequiredHint)
		{
			m_MainForm.ShowSettings(showCatalogRequiredHint, showLocationRequiredHint);
		}

        internal delegate T SetFITSDataDelegate<T>(uint clr);

        private T[][] SaveImageData<T>(Pixelmap pixelmap, SetFITSDataDelegate<T> setValue)
        {
            T[][] bimg = new T[pixelmap.Height][];

            for (int y = 0; y < pixelmap.Height; y++)
            {
                bimg[y] = new T[pixelmap.Width];

                for (int x = 0; x < pixelmap.Width; x++)
                {
                    bimg[y][x] = setValue(pixelmap[x, y]);
                }
            }

            return bimg;
        }

        public void ExportToFits(Pixelmap pixelmap)
        {
            int fitsBitDepth = 8;
            string filter;

            if (pixelmap.BitPixCamera == 8)
            {
                filter = "FITS Image 8 bit (*.fit;*.fits)|*.fit;*.fits";
                fitsBitDepth = 8;
            }
            else if (pixelmap.BitPixCamera <= 16)
            {
                filter = "FITS Image 16 bit (*.fit;*.fits)|*.fit;*.fits";
                fitsBitDepth = 16;
            }
            else
            {
                filter = "FITS Image 32 bit (*.fit;*.fits)|*.fit;*.fits";
                fitsBitDepth = 32;
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

                Fits f = new Fits();

                object data = null;
                if (fitsBitDepth == 16)
                {
                    data = SaveImageData<short>(pixelmap, delegate(uint val) { return (short)val; });
                }
                else if (fitsBitDepth == 8)
                {
                    data = SaveImageData<byte>(pixelmap, delegate(uint val) { return (byte)val; });
                }
                else if (fitsBitDepth == 32)
                {
                    data = SaveImageData<uint>(pixelmap, delegate(uint val) { return val; });
                }

                BasicHDU imageHDU = Fits.MakeHDU(data);

                nom.tam.fits.Header hdr = imageHDU.Header;
                hdr.AddValue("SIMPLE", "T", null);

                // Options include unsigned 8-bit (8), signed 16 bit (16), signed 32 bit (32), 32-bit IEEE float (-32), and 64-bit IEEE float (-64). The standard format is 16
                hdr.AddValue("BITPIX", fitsBitDepth, null);
                hdr.AddValue("NAXIS", 2, null);
                hdr.AddValue("NAXIS1", pixelmap.Width, null);
                hdr.AddValue("NAXIS2", pixelmap.Height, null);

                var hrdForm = new frmFITSHeader(hdr);
                if (hrdForm.ShowDialog() == DialogResult.Cancel) return;

                f.AddHDU(imageHDU);

                // Write a FITS file.
                using (BufferedFile bf = new BufferedFile(sfd.FileName, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    f.Write(bf);
                    bf.Flush();
                }
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
                TangraContext.Current.OcrExtractingTimestamps = true;
                m_VideoFileView.Update();
            }
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

		public delegate void RenderOverlayCallback(Graphics g);
 
		internal void RegisterOverlayRenderer(RenderOverlayCallback callback)
		{
			m_CustomOverlayRenderer = callback;
		}

		internal void DeregisterOverlayRenderer(RenderOverlayCallback callback)
		{
			m_CustomOverlayRenderer = null;
		}
	}
}
