using System;
using System.Collections.Generic;
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
using Tangra.View;

namespace Tangra.Controller
{
	public class VideoController : IDisposable, IVideoFrameRenderer
	{
		private VideoFileView m_VideoFileView;
		private IFramePlayer m_FramePlayer;

		public VideoController(VideoFileView videoFileView)
		{
			m_FramePlayer = new FramePlayer();
			m_VideoFileView = videoFileView;
			videoFileView.SetFramePlayer(m_FramePlayer);
		}

		public void OpenVideoFile(string fileName)
		{
			IFrameStream frameStream;

			string fileExtension = Path.GetExtension(fileName);

			TangraContext.Current.UsingADV = false;
			TangraContext.Current.UsingDirectShow = false;

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
			}
			else
			{
				// TODO: Show error message	
			}
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
	}
}
