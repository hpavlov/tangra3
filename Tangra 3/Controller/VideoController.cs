using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
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
using Tangra.View;

namespace Tangra.Controller
{
    public class VideoController : IDisposable, IVideoFrameRenderer, IVideoController
	{
		private VideoFileView m_VideoFileView;
        private ZoomedImageView m_ZoomedImageView;

		private Form m_MainFormView;
	    internal Panel m_pnlControlerPanel;

		private IFramePlayer m_FramePlayer;

		private FrameStateData m_FrameState;

		private frmAdvStatusPopup m_AdvStatusForm;
	    private frmTargetPSFViewerForm m_TargetPSFViewerForm;

		private AdvOverlayManager m_OverlayManager = new AdvOverlayManager();

        private AstroImage m_AstroImage;
        private RenderFrameContext m_CurrentFrameContext;

        private ImageTool m_ImageTool;
        private IVideoOperation m_CurrentOperation;

	    private PSFFit m_TargetPsfFit;
	    private uint[,] m_TargetBackgroundPixels;

	    private LightCurveController m_LightCurveController;
	    private frmMain m_MainForm;

        public VideoController(Form mainFormView, VideoFileView videoFileView, ZoomedImageView zoomedImageView, Panel pnlControlerPanel)
		{
			m_FramePlayer = new FramePlayer();
			m_VideoFileView = videoFileView;
            m_ZoomedImageView = zoomedImageView;
			m_MainFormView = mainFormView;
	        m_MainForm = (frmMain) mainFormView;
            m_pnlControlerPanel = pnlControlerPanel;
            videoFileView.SetFramePlayer(m_FramePlayer);
		}

		public void SetLightCurveController(LightCurveController lightCurveController)
		{
			m_LightCurveController = lightCurveController;
		}

		public void CloseOpenedVideoFile()
		{
			if (m_FramePlayer.Video != null)
			{
				TangraContext.Current.Reset();
				UpdateViews();

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

			//m_MainForm.m_FramePreprocessor.Clear();
			//m_MainForm.m_DefaultArrowImageTool = null;

			DeselectFeature();

			if (m_ZoomedImageView != null)
				m_ZoomedImageView.ClearZoomedImage();

			//m_MainForm.m_FramePreprocessor.Clear();

			EnsureAllPopUpFormsClosed();

			m_AstroImage = null;
			m_CurrentFrameContext = RenderFrameContext.Empty;
		}

		internal bool SingleBitmapFile(LCFile lcFile)
		{
			return OpenVideoFileInternal(
				null, 
				() => new SingleBitmapFileFrameStream(lcFile));
		}

	    public bool OpenVideoFile(string fileName)
	    {
			string fileExtension = Path.GetExtension(fileName);

		    return OpenVideoFileInternal(
				fileName, 
				() =>
				{
					IFrameStream frameStream = null;

					if (fileExtension == ".adv")
					{
						AdvEquipmentInfo equipmentInfo;
						frameStream = AstroDigitalVideoStream.OpenFile(fileName, out equipmentInfo);
						TangraContext.Current.UsingADV = true;
						m_OverlayManager.Init(equipmentInfo, frameStream.FirstFrame);
					}
					else
					{
						frameStream = VideoStream.OpenFile(fileName);
					}

					return frameStream;
				});
	    }

		public bool OpenVideoFileInternal(string fileName, Func<IFrameStream> frameStreamFactoryMethod)
		{
			TangraContext.Current.UsingADV = false;

			TangraContext.Current.FileName = null;
			TangraContext.Current.FileFormat = null;
			TangraContext.Current.HasVideoLoaded = false;
			m_OverlayManager.Reset();

		    TangraContext.Current.CrashReportInfo.VideoFile = fileName;

			TangraCore.PreProcessors.ClearAll();
			TangraCore.PreProcessors.AddGammaCorrection(TangraConfig.Settings.Photometry.EncodingGamma);

			// TODO: Update the views so they clear the currently displayed frame information

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

                if (m_FramePlayer.Video.BitPix == 8)
                    PSFFit.DataRange = PSFFittingDataRange.DataRange8Bit;
                else if (m_FramePlayer.Video.BitPix == 12)
                    PSFFit.DataRange = PSFFittingDataRange.DataRange12Bit;
				else if (m_FramePlayer.Video.BitPix == 14)
					PSFFit.DataRange = PSFFittingDataRange.DataRange14Bit;
                else
                    throw new ApplicationException("PSF fitting only supports 8, 12 and 14 bit data.");

				TangraContext.Current.HasVideoLoaded = true;
				TangraContext.Current.CanPlayVideo = true;
				TangraContext.Current.CanChangeTool = true;
				TangraContext.Current.CanLoadDarkFrame = true;
				TangraContext.Current.CanScrollFrames = true;				
				
				TangraContext.Current.HasImageLoaded = true;

				m_VideoFileView.UpdateVideoSizeAndLengthControls();

				m_FramePlayer.MoveToFrame(frameStream.FirstFrame);

				m_VideoFileView.Update();

				if (IsAstroDigitalVideo && !IsAdvStatusFormVisible)
				{
					ToggleAdvStatusForm(true);
				}

				if (!string.IsNullOrEmpty(fileName))
					RegisterRecentFile(RecentFileType.Video, fileName);

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
                m_CurrentOperation.NextFrame(m_CurrentFrameContext.CurrentFrameIndex, m_CurrentFrameContext.MovementType, m_CurrentFrameContext.IsLastFrame, m_AstroImage);

#if PROFILING
                Profiler.Instance.StartTimer("PAINTING");
#endif
                if (m_CurrentOperation.HasCustomZoomImage &&
                    m_ZoomedImageView != null)
                {
                    m_ZoomedImageView.DrawCustomZoomImage(m_CurrentOperation);
                }
#if PROFILING
                Profiler.Instance.StopTimer("PAINTING");
#endif
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

		public void OverlayStateForFrame(Bitmap displayBitmap, int frameId)
		{
            if (m_CurrentOperation != null &&
                m_CurrentOperation.AvoidImageOverlays)
            {
                // The current operation doesn't want overlays displayed
            }
            else
			    m_OverlayManager.OverlayStateForFrame(displayBitmap, m_FrameState, frameId);
		}

        public void CompleteRenderFrame(Graphics g)
        {
            if (m_CurrentOperation != null)
                m_CurrentOperation.PreDraw(g);

            if (m_CurrentOperation != null)
                m_CurrentOperation.PostDraw(g);            
        }

		public void RedrawCurrentFrame(bool showFields)
		{			
			if (showFields)
			{
				using (Bitmap image = m_AstroImage.Pixelmap.CreateNewDisplayBitmap())
				{
					Bitmap pixelMapWithFields = BitmapFilter.ToVideoFields(image);
					m_MainForm.pictureBox.Image = pixelMapWithFields;					
				}
			}
			else
				m_MainForm.pictureBox.Image = m_AstroImage.Pixelmap.DisplayBitmap;

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

		public bool IsRunning
		{
			get { return m_FramePlayer.IsRunning; }			
		}

		public bool IsAstroDigitalVideo
		{
			get { return m_FramePlayer.IsAstroDigitalVideo; }
		}

		public void MoveToFrame(int frameNo)
		{
			m_FramePlayer.MoveToFrame(frameNo);
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

		public void PlayVideo()
		{
			m_FramePlayer.Start(FramePlaySpeed.Fastest, 1);

			// NOTE: Will this always be called in the UI thread?
			m_VideoFileView.Update();
		}

		public void StopVideo()
		{
			m_FramePlayer.Stop();

			OnVideoPlayerStopped();

			// NOTE: Will this always be called in the UI thread?
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

            TangraVideo.SetVideoEngine(TangraConfig.Settings.Generic.PreferredRenderingEngineIndex);

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

        public void UpdateZoomedImage(Bitmap zoomedBitmap)
        {
            m_ZoomedImageView.UpdateImage(zoomedBitmap);
        }

        public void ClearZoomedImage()
        {
            m_ZoomedImageView.ClearZoomedImage();
        }

		internal void SetPictureBoxCursor(Cursor cursor)
		{
			if (m_MainForm != null)
				m_MainForm.pictureBox.Cursor = cursor;
		}

        public AstroImage GetCurrentAstroImage(bool integrated)
        {
            if (integrated)
            {
                // IntegratedAstroImage.ReadIntegrateImage(m_Host.FramePlayer, m_CurrFrameNo);
				// TODO: Remove IFramePlayet.GetIntegratedFrame() and replace with a call to IFramePlayet.GeeFrame() plus setting up the integration parameters
				//       This only seems to be used by AddEditTarget form in a case of no wind and shaking (which is the case most of the times)
                Pixelmap image = m_FramePlayer.GetIntegratedFrame(m_CurrentFrameContext.CurrentFrameIndex, TangraConfig.Settings.Special.AddStarImageFramesToIntegrate, true /* 'true'so we start from the current frame */, false);
                
                // TODO:
                // m_Host.ApplyPreProcessing(image);

                return new AstroImage(image);
            }
            else
                // Don't integrate if the wind or shaking flag is set. Faint stars are unlikely to be seen anyway 
                // with integration in a case of a good shake
                return m_AstroImage;          
        }

		internal Pixelmap GetFrame(int frameId)
		{
			return m_FramePlayer.GetFrame(frameId, true);
		}

		public FrameStateData GetCurrentFrameState()
		{
			return m_FrameState;
		}

		void IVideoFrameRenderer.PlayerStarted()
		{
			OnVideoPlayerStarted();

			// TODO: Notify all views

			try
			{
				m_WinControl.Invoke(new FramePlayer.SimpleDelegate(m_FrameRenderer.PlayerStarted));
			}
			catch (ObjectDisposedException)
			{ }

			UpdateViews();
		}

		void IVideoFrameRenderer.PlayerStopped()
		{
			OnVideoPlayerStopped();

			// TODO: Notify all views

			try
			{
				m_WinControl.Invoke(new FramePlayer.SimpleDelegate(m_FrameRenderer.PlayerStopped));
			}
			catch (ObjectDisposedException)
			{ }

			UpdateViews();
		}

		void IVideoFrameRenderer.RenderFrame(int currentFrameIndex, Pixelmap currentPixelmap, MovementType movementType, bool isLastFrame, int msToWait)
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
                                msToWait
                            });
			}
			catch (ObjectDisposedException)
			{ }
		}

		public void ToggleAdvStatusForm()
		{
			ToggleAdvStatusForm(false);
		}

		private void HideAdvStatusForm()
		{
			if (m_AdvStatusForm != null && m_AdvStatusForm.Visible)
				m_AdvStatusForm.Hide();
		}

		private void ToggleAdvStatusForm(bool forceShow)
		{
			if (IsAstroDigitalVideo)
			{
				if (m_AdvStatusForm == null)
				{
					m_AdvStatusForm = new frmAdvStatusPopup(TangraConfig.Settings.ADVS);
					m_AdvStatusForm.Show(m_MainFormView);
					PositionAdvstatusForm();
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

					PositionAdvstatusForm();
					m_AdvStatusForm.ShowStatus(m_FrameState);
				}
				else if (!forceShow)
				{
					HideAdvStatusForm();
				}
				else
				{
					PositionAdvstatusForm();
					m_AdvStatusForm.ShowStatus(m_FrameState);
				}
			}
		}

		private void PositionAdvstatusForm()
		{
			if (m_AdvStatusForm != null &&
				m_AdvStatusForm.Visible)
			{
				m_AdvStatusForm.Left = m_MainFormView.Right;
				m_AdvStatusForm.Top = m_MainFormView.Top;
			}
		}

		//private void EnsureAllPopupFormsClosed()
		//{
		//    if (m_TargetPSFViewerForm != null &&
		//        m_TargetPSFViewerForm.Visible)
		//    {
		//        m_TargetPSFViewerForm.Close();
		//        m_TargetPSFViewerForm = null;
		//    }

		//    if (m_AdvStatusForm != null &&
		//        m_AdvStatusForm.Visible)
		//    {
		//        m_AdvStatusForm.Close();
		//        m_AdvStatusForm = null;
		//    }
		//}

        public void TogglePSFViewerForm()
        {
            TogglePSFViewerForm(false);
        }

        public void TogglePSFViewerForm(bool forceShow)
        {
            if (m_TargetPSFViewerForm == null)
                m_TargetPSFViewerForm = new frmTargetPSFViewerForm();

            if (m_FramePlayer.Video != null && !m_TargetPSFViewerForm.Visible)
            {
                try
                {
                    m_TargetPSFViewerForm.Show(m_MainFormView);
                }
                catch (ObjectDisposedException)
                {
                    m_TargetPSFViewerForm = new frmTargetPSFViewerForm();
                    m_TargetPSFViewerForm.Show(m_MainFormView);
                }

                PositionTargetPSFViewerForm();

				ShowTargetPSF();
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
				m_TargetPSFViewerForm.ShowTargetPSF(m_TargetPsfFit, m_FramePlayer.Video.BitPix, m_TargetBackgroundPixels);
        }

		public void ShowFSTSFileViewer()
		{
			if (TangraContext.Current.HasVideoLoaded && m_FramePlayer.IsAstroDigitalVideo)
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
			PositionAdvstatusForm();
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

			m_ImageTool = ImageTool.SwitchTo<TImageTool>(m_CurrentOperation, m_ImageTool);

			return m_ImageTool;
        }

		public bool ActivateOperation<TOperation>(params object[] constructorParams) where TOperation : class, IVideoOperation, new()
		{
			//m_FramePreprocessor.Clear();

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
					// TODO: Test if this is actually needed to the reprocessing to be applied (then remove code if not needed)
					// Redraw the current frame so the pre-processing is included as well
					// m_FramePlayer.MoveToFrame(m_CurrentFrameId);					

					return true;
				}
			}
			finally
			{
				m_VideoFileView.Update();
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
					UpdateZoomedImage(zoomedBmp);
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
    }
}
