using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Tangra.Config;
using Tangra.Controller;
using Tangra.Helpers;
using Tangra.ImageTools;
using Tangra.Model.Config;
using Tangra.Model.Context;
using Tangra.Model.Image;
using Tangra.Model.Video;
using Tangra.PInvoke;
using Tangra.Video;
using Tangra.Video.AstroDigitalVideo;
using Tangra.VideoOperations.LightCurves;
using Tangra.View;

namespace Tangra
{
	public partial class frmMain : Form, IVideoFrameRenderer
	{
		private VideoController m_VideoController;
	    private LightCurveController m_LightCurveController;
		private VideoFileView m_VideoFileView;
        private ZoomedImageView m_ZoomedImageView;
		private bool m_FormLoaded = false;

		public frmMain()
		{
			InitializeComponent();

			TangraConfig.Load(ApplicationSettingsSerializer.Instance);

			m_VideoFileView = new VideoFileView(this);
		    m_ZoomedImageView = new ZoomedImageView(zoomedImage, this);

            m_VideoController = new VideoController(this, m_VideoFileView, m_ZoomedImageView, pnlControlerPanel);
            m_LightCurveController = new LightCurveController(this, m_VideoController);

			BuildRecentFilesMenu();
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
            bool isNewFrame = m_CurrentFrameId != frameContext.CurrentFrameIndex;
			m_CurrentFrameId = frameContext.CurrentFrameIndex;

			if (frameContext.MovementType != MovementType.Refresh)
			{
				// Only set the AstroImage if this is not a Refresh. Otherwise the pre-processing will be lost in 
				// consequative refreshes and the AstroImage will be wrong even after the first Refresh
                m_VideoController.SetImage(currentPixelmap, frameContext);
			}

#if PROFILING
                Profiler.Instance.StopTimer("PAINTING");
#endif

			ssFrameNo.Text = string.Format("Frame: {0}", frameContext.CurrentFrameIndex);

            m_VideoController.NewFrameDisplayed();

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

            if (isNewFrame)
                NotificationManager.Instance.NotifyCurrentFrameChanged(m_CurrentFrameId);

#if PROFILING
                Profiler.Instance.StopTimer("PAINTING");
#endif

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
			pictureBox.Image = m_VideoContext.Pixelmap.DisplayBitmap;

			if (m_VideoController.HasAstroImageState)
			{
				m_VideoController.OverlayStateForFrame(m_VideoContext.Pixelmap.DisplayBitmap, m_CurrentFrameId);
			}

            using (Graphics g = Graphics.FromImage(m_VideoContext.Pixelmap.DisplayBitmap))
            {
                m_VideoController.CompleteRenderFrame(g);

                g.Save();

                //m_VideoContext.Pixelmap.DisplayBitmap.Save(@"C:\Tangra-DisplayBitmap.bmp");
            }
		}

		#endregion

		private void miAbout_Click(object sender, EventArgs e)
		{
			var frmAbout = new frmAbout();
			frmAbout.ShowDialog(this);
		}

		private void miOpenVideo_Click(object sender, EventArgs e)
		{
            if (CurrentOS.IsWindows)
            {
                openVideoFileDialog.Filter = "All Supported Files (*.avi;*.avs;*.adv)|*.avi;*.avs;*.adv";
                openVideoFileDialog.DefaultExt = "avi";
            }
            else
            {
                // On Non-Windows OS currently only ADV files are supported
                openVideoFileDialog.Filter = "All Supported Files (*.adv)|*.adv";
                openVideoFileDialog.DefaultExt = "adv";
            }

		    if (openVideoFileDialog.ShowDialog(this) == DialogResult.OK)
			{
				OpenTangraFile(openVideoFileDialog.FileName);
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

		public void OpenTangraFile(string fileName)
		{
			string fileExt = Path.GetExtension(fileName);

			if (fileExt == ".lc")
			{
				// TODO: Load a light curve file
			}
			else
			{
				if (m_VideoController.OpenVideoFile(fileName))
					RegisterRecentFile(RecentFileType.Video, fileName);
			}
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
			// TODO: Pass the LC form

			var frmSettings = new frmTangraSettings(null, m_VideoController.AdvStatusPopupFormCustomizer);
			frmSettings.StartPosition = FormStartPosition.CenterParent;
			if (frmSettings.ShowDialog(this) == DialogResult.OK)
			{
				m_VideoController.RefreshCurrentFrame();
			}
		}

		private void scrollBarFrames_Scroll(object sender, ScrollEventArgs e)
		{
			if (!TangraContext.Current.CanScrollFrames)
				return;

			if (e.Type == ScrollEventType.EndScroll)
			{
				if (!m_VideoController.IsRunning)
				{
					m_VideoController.MoveToFrame(e.NewValue);
				}
			}
			else
			{
				//ssFrameNo.Text = "Frame: " + e.NewValue.ToString();

				displayFrameTimer.Tag = e;
				displayFrameTimer.Enabled = false;
				displayFrameTimer.Enabled = true;
			}
		}

		private void displayFrameTimer_Tick(object sender, EventArgs e)
		{
			displayFrameTimer.Enabled = false;

			ScrollEventArgs se = displayFrameTimer.Tag as ScrollEventArgs;

			if (se != null)
			{
				if (!m_VideoController.IsRunning)
				{
					m_VideoController.MoveToFrame(se.NewValue);
				}
			}
		}

		private void btn1FrMinus_Click(object sender, EventArgs e)
		{
			if (!m_VideoController.IsRunning)
			{
				m_VideoController.StepBackward();
			}
		}

		private void btn1FrPlus_Click(object sender, EventArgs e)
		{
			if (!m_VideoController.IsRunning)
			{
				m_VideoController.StepForward();
			}
		}

		private void btn1SecMinus_Click(object sender, EventArgs e)
		{
			if (!m_VideoController.IsRunning)
			{
				m_VideoController.StepBackward(1);
			}
		}

		private void btn1SecPlus_Click(object sender, EventArgs e)
		{
			if (!m_VideoController.IsRunning)
			{
				m_VideoController.StepForward(1);
			}
		}

		private void btn10SecMinus_Click(object sender, EventArgs e)
		{
			if (!m_VideoController.IsRunning)
			{
				m_VideoController.StepBackward(10);
			}
		}

		private void btn10SecPlus_Click(object sender, EventArgs e)
		{
			if (!m_VideoController.IsRunning)
			{
				m_VideoController.StepForward(10);
			}
		}

		private void btnJumpTo_Click(object sender, EventArgs e)
		{
			if (!m_VideoController.IsRunning)
			{
				//frmJumpToFrame frm = new frmJumpToFrame();
				//frm.nudFrameToJumpTo.Properties.MinValue = m_VideoController.Video.FirstFrame;
				//frm.nudFrameToJumpTo.Properties.MaxValue = m_VideoController.Video.LastFrame;
				//frm.nudFrameToJumpTo.Value = m_CurrentFrameId;

				//if (frm.ShowDialog(this) == DialogResult.OK)
				//    m_FramePlayer.MoveToFrame((int)frm.nudFrameToJumpTo.Value);
			}
		}

		private void miTools_DropDownOpening(object sender, EventArgs e)
		{
			miADVStatusData.Checked = m_VideoController.IsAdvStatusFormVisible;
		    miTargetPSFViewer.Checked = m_VideoController.IsTargetPSFViewerFormVisible;
		}

		private void miADVStatusData_Click(object sender, EventArgs e)
		{
			if (m_VideoController.IsAstroDigitalVideo)
			{
				m_VideoController.ToggleAdvStatusForm();
			}
		}

        private void miTargetPSFViewer_Click(object sender, EventArgs e)
        {
            m_VideoController.TogglePSFViewerForm();
        }

		private void frmMain_Move(object sender, EventArgs e)
		{
			m_VideoController.NotifyMainFormMoved();
		}

        private void BuildRecentFilesMenu()
        {
            miRecentVideos.DropDownItems.Clear();

            foreach (string recentFilePath in TangraConfig.Settings.RecentFiles.Lists[(int)RecentFileType.Video])
            {
                if (File.Exists(recentFilePath))
                {
                    ToolStripMenuItem miRecentFile = (ToolStripMenuItem)miRecentVideos.DropDownItems.Add(recentFilePath);
                    miRecentFile.Tag = recentFilePath;
                    miRecentFile.Click += new EventHandler(miRecentFileMenuItemClick);
                }
            }

            miRecentVideos.Enabled = miRecentVideos.DropDownItems.Count > 0;

			miRecentLightCurves.DropDownItems.Clear();

			foreach (string recentFilePath in TangraConfig.Settings.RecentFiles.Lists[(int)RecentFileType.LightCurve])
			{
				if (File.Exists(recentFilePath))
				{
					ToolStripMenuItem miRecentFile = (ToolStripMenuItem)miRecentLightCurves.DropDownItems.Add(recentFilePath);
					miRecentFile.Tag = recentFilePath;
					miRecentFile.Click += new EventHandler(miRecentFileMenuItemClick);
				}
			}

			miRecentLightCurves.Enabled = miRecentLightCurves.DropDownItems.Count > 0;
        }

		internal void RegisterRecentFile(RecentFileType type, string fileName)
        {
			TangraConfig.Settings.RecentFiles.NewRecentFile(type, fileName);
            TangraConfig.Settings.Save();

            BuildRecentFilesMenu();
        }

		private void miRecentFileMenuItemClick(object sender, EventArgs e)
		{
			ToolStripMenuItem mi = (sender as ToolStripMenuItem);
			if (mi != null && mi.Tag is string)
			{
				string filePath = mi.Tag as string;
				if (!string.IsNullOrEmpty(filePath) &&
					File.Exists(filePath))
				{
					OpenTangraFile(filePath);
				}
			}
		}

        private void miExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void miOnlineHelp_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.hristopavlov.net/Tangra3");
        }

        #region Picture Box Events
        private void pictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            m_VideoController.MouseClick(e.Location);
        }

        private void pictureBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            m_VideoController.MouseDoubleClick(e.Location);
        }

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            m_VideoController.MouseDown(e.Location);
        }

        private void pictureBox_MouseLeave(object sender, EventArgs e)
        {
            m_VideoController.MouseLeave();
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            m_VideoController.MouseMove(e.Location);
        }

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            m_VideoController.MouseUp(e.Location);
        }
        #endregion

        private void miReduceLightCurve_Click(object sender, EventArgs e)
        {
            m_VideoController.ActivateOperation<ReduceLightCurveOperation>();
        }

		private void miFSTSFileViewer_Click(object sender, EventArgs e)
		{
			m_VideoController.ShowFSTSFileViewer();
		}

		private void miRepairAdvFile_Click(object sender, EventArgs e)
		{
			if (openCorruptedAdvFileDialog.ShowDialog(this) == DialogResult.OK)
			{
				m_VideoController.RepairAdvFile(openCorruptedAdvFileDialog.FileName);
			}
		}

        private void miOpenLightCurve_Click(object sender, EventArgs e)
        {
            m_LightCurveController.LoadLightCurve();
        }
	}
}
