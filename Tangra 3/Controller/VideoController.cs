using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Config;
using Tangra.Model.Context;
using Tangra.Model.Image;
using Tangra.Model.Video;
using Tangra.PInvoke;
using Tangra.Video;
using Tangra.Video.AstroDigitalVideo;
using Tangra.View;

namespace Tangra.Controller
{
	public class VideoController : IDisposable, IVideoFrameRenderer
	{
		private VideoFileView m_VideoFileView;
		private Form m_MainFormView;
		private IFramePlayer m_FramePlayer;

		private FrameStateData m_FrameState;

		private frmAdvStatusPopup m_AdvStatusForm;

		private AdvOverlayManager m_OverlayManager = new AdvOverlayManager();

		public VideoController(Form mainFormView, VideoFileView videoFileView)
		{
			m_FramePlayer = new FramePlayer();
			m_VideoFileView = videoFileView;
			m_MainFormView = mainFormView;
			videoFileView.SetFramePlayer(m_FramePlayer);
		}

		public bool OpenVideoFile(string fileName)
		{
			IFrameStream frameStream;

			string fileExtension = Path.GetExtension(fileName);

			TangraContext.Current.UsingADV = false;
			TangraContext.Current.UsingDirectShow = false;
			TangraContext.Current.FileName = null;
			TangraContext.Current.FileFormat = null;
			TangraContext.Current.HasVideoLoaded = false;
			m_OverlayManager.Reset();

			// TODO: Update the views so they clear the currently displayed frame information

            if (fileExtension == ".adv")
            {
                frameStream = AstroDigitalVideoStream.OpenFile(fileName);
                TangraContext.Current.UsingADV = true;
            }
            else
            {
                frameStream = VideoStream.OpenFile(fileName);
            }

		    if (frameStream != null)
			{
				m_FramePlayer.OpenVideo(frameStream);

				TangraContext.Current.FileName = Path.GetFileName(fileName);
				TangraContext.Current.FileFormat = frameStream.VideoFileType;

				if (!IsAstroDigitalVideo)
					HideAdvStatusForm();

				TangraContext.Current.FrameWidth = m_FramePlayer.Video.Width;
				TangraContext.Current.FrameHeight = m_FramePlayer.Video.Height;
				TangraContext.Current.FirstFrame = m_FramePlayer.Video.FirstFrame;
				TangraContext.Current.LastFrame = m_FramePlayer.Video.LastFrame;

				TangraContext.Current.HasVideoLoaded = true;
				TangraContext.Current.CanPlayVideo = true;
				TangraContext.Current.CanChangeTool = true;
				TangraContext.Current.CanLoadDarkFrame = true;
				TangraContext.Current.CanScrollFrames = true;				

				m_FramePlayer.MoveToFrame(frameStream.FirstFrame);

				TangraContext.Current.HasImageLoaded = true;

				m_VideoFileView.UpdateVideoSizeAndLengthControls();

				m_VideoFileView.Update();

				if (IsAstroDigitalVideo && !IsAdvStatusFormVisible)
				{
					ToggleAdvStatusForm(true);
				}

				return true;
			}
			else
			{
				// TODO: Show error message	
			}

			return false;
		}

		public void SetFrameStateData(FrameStateData frameState)
		{
			m_FrameState = frameState;

			if (m_AdvStatusForm != null && m_AdvStatusForm.Visible)
				m_AdvStatusForm.ShowStatus(m_FrameState);
		}

		public bool HasAstroImageState
		{
			get { return !m_FrameState.IsEmpty(); }
		}

		public void OverlayStateForFrame(Bitmap displayBitmap, int frameId)
		{
			m_OverlayManager.OverlayStateForFrame(displayBitmap, m_FrameState, frameId);
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

			OnVideoPlayerStarted();

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

		private void OnVideoPlayerStarted()
		{
			TangraContext.Current.CanScrollFrames = false;
		}

		private void OnVideoPlayerStopped()
		{
			TangraContext.Current.CanScrollFrames = true;
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
				// NOTE: Is this going to work in MONO on Linux/MacOSX
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

		public void NotifyMainFormMoved()
		{
			PositionAdvstatusForm();
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
	}
}
