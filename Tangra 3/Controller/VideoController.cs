using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
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
				frameStream = VideoStream.OpenFile(fileName);

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

				m_VideoFileView.Update();
			}
			else
			{
				// TODO: Show error message	
			}
		}

		public void PlayVideo()
		{			
			TangraContext.Current.CanScrollFrames = false;

			m_FramePlayer.Start(FramePlaySpeed.Fastest, 1);

			m_VideoFileView.Update();
		}

		public void StopVideo()
		{
			m_FramePlayer.Stop();

			TangraContext.Current.CanScrollFrames = true;

			m_VideoFileView.Update();
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

			// TODO: Get the configured "prefered" video engine from Tangra Config
			TangraVideo.SetVideoEngine(0);

			m_VideoFileView.Reset();
		}

		public void Dispose()
		{
			m_FramePlayer.DisposeResources();
			m_FramePlayer = null;
		}

		void IVideoFrameRenderer.PlayerStarted()
		{
			// TODO: Notify all views

			try
			{
				m_WinControl.Invoke(new FramePlayer.SimpleDelegate(m_FrameRenderer.PlayerStarted));
			}
			catch (ObjectDisposedException)
			{ }
		}

		void IVideoFrameRenderer.PlayerStopped()
		{
			// TODO: Notify all views

			try
			{
				m_WinControl.Invoke(new FramePlayer.SimpleDelegate(m_FrameRenderer.PlayerStopped));
			}
			catch (ObjectDisposedException)
			{ }

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
