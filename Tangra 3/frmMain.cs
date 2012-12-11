using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Tangra.Config;
using Tangra.Controller;
using Tangra.Model.Config;
using Tangra.Model.Context;
using Tangra.Model.Image;
using Tangra.Model.Video;
using Tangra.PInvoke;
using Tangra.Video;
using Tangra.View;

namespace Tangra
{
	public partial class frmMain : Form, IVideoFrameRenderer
	{
		private VideoController m_VideoController;
		private VideoFileView m_VideoFileView;
		private bool m_FormLoaded = false;

		public frmMain()
		{
			InitializeComponent();

			TangraConfig.Load();

			m_VideoFileView = new VideoFileView(this);
			m_VideoController = new VideoController(m_VideoFileView);			
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

			m_VideoController.Dispose();
		}

		#region Frame Rendering

		private RenderFrameContext m_LastFrameContext = RenderFrameContext.Empty;
		private VideoContext m_VideoContext = new VideoContext();

		public void PlayerStarted()
		{
			m_VideoController.UpdateViews();
		}

		public void PlayerStopped()
		{
			m_VideoController.UpdateViews();
		}

		public void RenderFrame(
			int currentFrameIndex,
			Pixelmap currentPixelmap,
			MovementType movementType,
			bool isLastFrame,
			int msToWait)
		{
			m_LastFrameContext = new RenderFrameContext()
			{
				CurrentFrameIndex = currentFrameIndex,
				MovementType = movementType,
				IsLastFrame = isLastFrame,
				MsToWait = msToWait,
			};

			RenderFrame(m_LastFrameContext, currentPixelmap);
		}

		private int m_CurrentFrameId = -1;
		
		private Stopwatch m_sw = new Stopwatch();
		private volatile int m_RefreshFrameLockOwner;
		private bool m_RefreshAtTheEndOfRenderFrame = false;
		private SpinLock m_ReentrancyGuard = new SpinLock(true);

		internal void RenderFrame(RenderFrameContext frameContext, Pixelmap currentPixelmap)
		{
			bool taken = false;

			if (frameContext.MsToWait > -1)
			{
				m_sw.Reset();
				m_sw.Start();
			}

			if (m_ReentrancyGuard.IsHeldByCurrentThread)
			{
				while ((Interlocked.CompareExchange(ref m_RefreshFrameLockOwner, 2, 0) == 0))
				{
					Thread.Sleep(1);
				}
				try
				{
					if (m_ReentrancyGuard.IsHeldByCurrentThread)
					{
						m_RefreshAtTheEndOfRenderFrame = true;
						return;
					}
				}
				finally
				{
					this.m_RefreshFrameLockOwner = 0;
				}
			}

			m_ReentrancyGuard.Enter(ref taken);

			try
			{
				m_RefreshAtTheEndOfRenderFrame = false;

				DoRenderFrame(currentPixelmap, frameContext);

				if (frameContext.MsToWait > -1)
				{
					m_sw.Stop();
					int msToWaitReal = frameContext.MsToWait - (int)m_sw.ElapsedMilliseconds;
					if (msToWaitReal > 0)
						Thread.Sleep(msToWaitReal);
				}

				while ((Interlocked.CompareExchange(ref m_RefreshFrameLockOwner, 1, 0) == 0))
				{
					Thread.Sleep(1);
				}
				try
				{
					if (m_RefreshAtTheEndOfRenderFrame)
						m_VideoController.RefreshCurrentFrame();
				}
				finally
				{
					m_RefreshFrameLockOwner = 0;
				}

				//m_TangraApplicationImpl.SetCurrentFrame(frameContext.CurrentFrameIndex);
			}
			catch (OutOfMemoryException)
			{
				MessageBox.Show(
					"There was not enough free memory to complete the operation. Please stop other running applications and try again.",
					"Tangra",
					MessageBoxButtons.OK, MessageBoxIcon.Stop);

				Application.Exit();
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.ToString());

#if PRODUCTION
                frmUnhandledException.HandleExceptionNoRestart(this, ex);
#else
				Debugger.Break();
#endif
			}
			finally
			{
				if (taken)
					m_ReentrancyGuard.Exit();
			}
		}

		private int m_FPSLastFrameNo;
		private long m_FPSLastSavedTicks;

		private void DoRenderFrame(Pixelmap currentPixelmap, RenderFrameContext frameContext)
		{
#if PROFILING
            sw.Reset();
            sw.Start();
            swRendering.Start();
            try
            {
#endif
			// NOTE: Disposing the images may happen before they have been rendered and this would cause the following error:
			// Parameter is not valid.
			//  at System.Drawing.Image.get_Width()
			//

			if (m_VideoContext.Pixelmap != null)
			{
				m_VideoContext.Pixelmap.Dispose();
				m_VideoContext.Pixelmap = null;
			}

			m_VideoContext.Pixelmap = currentPixelmap;
			m_VideoContext.RenderFrameContext = frameContext;

			if (currentPixelmap == null)
				return;

			//VideoContext.Current.DisplayBitmap = currentPixelmap.CreateNewDisplayBitmapDoNotDispose();
			

#if !PRODUCTION
			Trace.Assert(frameContext.CurrentFrameIndex >= scrollBarFrames.Minimum);
			Trace.Assert(frameContext.CurrentFrameIndex <= scrollBarFrames.Maximum);
#endif
			scrollBarFrames.Value = frameContext.CurrentFrameIndex;

			//if (m_VideoContext.FirstPlayedIndex == -1) m_VideoContext.FirstPlayedIndex = frameContext.CurrentFrameIndex;

			//if (TangraConfig.Settings.Generic.ShowProcessingSpeed &&
			//    m_FramePlayer.IsRunning &&
			//    VideoContext.Current.PlayStarted != DateTime.MaxValue)
			{
			    // If the interval between now and the last saved ticks is more than X (1 sec):
			    //  - Compute and display the new FPS
			    //  - Save current Ticks and Frame
			    if (m_FPSLastFrameNo != -1)
			    {
			        double totalSec = (new TimeSpan(DateTime.Now.Ticks - m_FPSLastSavedTicks)).TotalSeconds;
			        if (totalSec >= 1.0)
			        {
						int totalFrames = frameContext.CurrentFrameIndex - m_FPSLastFrameNo;
						ssFPS.Text = string.Format("{0} fps", ((double)totalFrames / (totalSec /* m_FramePlayer.FrameStep*/)).ToString("0.0"));

						m_FPSLastFrameNo = frameContext.CurrentFrameIndex;
			            m_FPSLastSavedTicks = DateTime.Now.Ticks;
			        }
			    }
			    else
			    {
					m_FPSLastFrameNo = frameContext.CurrentFrameIndex;
			        m_FPSLastSavedTicks = DateTime.Now.Ticks;
			    }
			}

			//TODO: The ZoomImage should be another view? or is it part of the current main view? The question is do we allow actions/tools to modify the ZoomImage, and the answer it probably YES
			//ClearZoomImage();

#if PROFILING
                Profiler.Instance.StartTimer("PAINTING");
#endif
			m_CurrentFrameId = frameContext.CurrentFrameIndex;

			if (frameContext.MovementType != MovementType.Refresh)
			{
				// Only set the AstroImage if this is not a Refresh. Otherwise the pre-processing will be lost in 
				// consequative refreshes and the AstroImage will be wrong even after the first Refresh
				//VideoContext.Current.AstroImage = new AstroImage(currentPixelmap, false);
				//VideoContext.Current.AstroImageState = currentPixelmap.FrameState;
			}

#if PROFILING
                Profiler.Instance.StopTimer("PAINTING");
#endif

			//ssFrameNo.Text = string.Format("Frame: {0}", currentFrameIndex);

//            if (m_CurrentOperation != null)
//            {
//                m_CurrentOperation.NextFrame(currentFrameIndex, movementType, isLastFrame);

//#if PROFILING
//                    Profiler.Instance.StartTimer("PAINTING");
//#endif
//                if (m_CurrentOperation.HasCustomZoomImage &&
//                    zoomedImage.Image != null)
//                {
//                    using (Graphics g = Graphics.FromImage(zoomedImage.Image))
//                    {
//                        m_CurrentOperation.DrawCustomZoomImage(g, zoomedImage.Image.Width, zoomedImage.Image.Height);
//                        g.Save();
//                        zoomedImage.Invalidate();
//                    }
//                }
//#if PROFILING
//                    Profiler.Instance.StopTimer("PAINTING");
//#endif
//            }

#if PROFILING
                Profiler.Instance.StartTimer("PAINTING");
#endif

			//PreProcessingInfo info;
			//Core.PreProcessors.PreProcessingGetConfig(out info);
			//ApplicationState.Current.PreProcessingInfo = info;

			//if (TangraConfig.Settings.Generic.PerformanceQuality == PerformanceQuality.Responsiveness)
			//{
			//    if (currentFrameIndex % 12 == 0)
			//    {
			//        statusStrip.Invalidate();
			//        pnlControlerPanel.Invalidate();
			//    }
			//    zoomedImage.Invalidate();
			//}

			CompleteRenderFrame();

			Update();

			// TODO: Replace with an Observer Pattern that notifies all views that are interested!
			//NotificationManager.PostMessage(this, MSG_ID_FRAME_CHANGED, currentFrameIndex, null);

#if PROFILING
                Profiler.Instance.StopTimer("PAINTING");
#endif

			//if (m_FramePlayer.IsAstroDigitalVideo &&
			//    m_AdvStatusForm != null &&
			//    m_AdvStatusForm.Visible)
			//{
			//    m_AdvStatusForm.ShowStatus(VideoContext.Current.AstroImageState);
			//}



#if PROFILING
            }
            finally
            {
                sw.Stop();
                Profiler.Instance.AppendMetric("FRAME_RENDER_TIME_SECONDS", sw.ElapsedMilliseconds / 1000.0);
                swRendering.Stop();
            }

            if (currentFrameIndex % 1 == 0)
            {
                Trace.WriteLine(string.Format("Last 100 frames total: {0} sec", swRendering.Elapsed.TotalSeconds.ToString("0.0")),
                    "PROFILING");
                swRendering.Reset();

                Profiler.Instance.PrintMetrics();
                Profiler.Instance.Reset();
            }
#endif
		}

		private void CompleteRenderFrame()
		{
			//VideoContext.Current.DisplayBitmap = bitmapDoNotDispose;

			pictureBox.Image = m_VideoContext.Pixelmap.DisplayBitmap;

			//if (!VideoContext.Current.AstroImageState.IsEmpty())
			//{
			//    m_OverlayManager.OverlayStateForFrame(VideoContext.Current.DisplayBitmap, VideoContext.Current.AstroImageState, currentFrameIndex);
			//}


			//if (m_ImageTool != null)
			//    m_ImageTool.OnNewFrame(currentFrameIndex, isLastFrame);


			//using (Graphics g = Graphics.FromImage(VideoContext.Current.DisplayBitmap))
			//{
			//    if (m_CurrentOperation != null)
			//        m_CurrentOperation.PreDraw(g);

			//    //if (m_CurrentOperation != null &&
			//    //   (m_CurrentOperation.Type & OperationTypes.DrawStars) != 0)
			//    //{
			//    //    foreach (AstroPixel star in stars)
			//    //    {
			//    //        m_CurrentOperation.DrawStar(g, star);
			//    //    }
			//    //}

			//    if (m_CurrentOperation != null)
			//        m_CurrentOperation.PostDraw(g);

			//    g.Save();
			//    //VideoContext.Current.DisplayBitmap.Save(@"C:\Tangra-DisplayBitmap.bmp");
			//}
		}

		#endregion

		private void miAbout_Click(object sender, EventArgs e)
		{
			var frmAbout = new frmAbout();
			frmAbout.ShowDialog(this);
		}

		private void miOpenVideo_Click(object sender, EventArgs e)
		{
			if (openVideoFileDialog.ShowDialog(this) == DialogResult.OK)
			{
				m_VideoController.OpenVideoFile(openVideoFileDialog.FileName);				
			}
		}

		private void frmMain_Load(object sender, EventArgs e)
		{
			m_VideoController.InitVideoSystem(new PlayerContext(this));

			m_FormLoaded = true;
		}

		private void frmMain_Resize(object sender, EventArgs e)
		{
			if (m_FormLoaded &&
				FormWindowState.Normal == WindowState)
			{
				//TODO: Implement remembering form position into the IsolatedFileStorage
				//PositionMemento.SaveControlPosition(this);
			}

			ConfigureImageScrollbars();
		}

		private void ConfigureImageScrollbars()
		{
			if (FormWindowState.Maximized == WindowState)
			{
				pictureBox.Dock = DockStyle.None;
				pictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
				//pnlVideoFrame.AutoScroll = true;
			}
			else
			{
				pictureBox.Dock = DockStyle.Fill;
				pictureBox.SizeMode = PictureBoxSizeMode.Normal;
				//pnlVideoFrame.AutoScroll = false;
			}
		}

		private void btnPlay_Click(object sender, EventArgs e)
		{
			m_VideoController.PlayVideo();
		}

		private void btnStop_Click(object sender, EventArgs e)
		{
			m_VideoController.StopVideo();
		}

		private void miSettings_Click(object sender, EventArgs e)
		{
			// TODO: Pass the ADVS State form
			var frmSettings = new frmTangraSettings(null, null);
			frmSettings.ShowDialog(this);
		}
	}
}
