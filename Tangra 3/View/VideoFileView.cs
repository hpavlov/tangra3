using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Config;
using Tangra.Model.Context;
using Tangra.Model.Video;
using Tangra.Video;

namespace Tangra.View
{
	public class VideoFileView
	{
		private frmMain m_MainForm;
		private int m_ExtraWidth;
		private int m_ExtraHeight;
		private IFramePlayer m_FramePlayer;

		public VideoFileView(frmMain mainForm)
		{
			m_MainForm = mainForm;

			m_ExtraWidth = m_MainForm.Width - m_MainForm.pictureBox.Width;
			m_ExtraHeight = m_MainForm.Height - m_MainForm.pictureBox.Height;
		}

		internal void SetFramePlayer(IFramePlayer framePlayer)
		{
			m_FramePlayer = framePlayer;
		}

		public void Update()
		{
			if (TangraContext.Current.HasAnyFileLoaded)
			{
				m_MainForm.Text = string.Format("Tangra v3.0 [Beta Version] - {0}, {1}", TangraContext.Current.FileName,
				                                TangraContext.Current.FileFormat);
			}
			else
			{
				m_MainForm.Text = "Tangra v3.0 [Beta Version]";

				if (m_MainForm.pictureBox.Image != null)
				{
					try
					{
						using (Graphics g = Graphics.FromImage(m_MainForm.pictureBox.Image))
						{
							g.Clear(SystemColors.ControlDark);
							g.Save();
						}
					}
					catch (ArgumentException)
					{ }

					m_MainForm.pictureBox.Refresh();
					m_MainForm.pictureBox.Image = null;
				}

				m_MainForm.ssFPS.Text = string.Empty;
			}

#if PRODUCTION
                m_MainForm.miViewTrackingDebug.Visible = false;
                m_MainForm.miOpenFailedCalibration.Visible = false;
				m_MainForm.miOpenFailedPlateSolveFile.Visible = false;
#endif
			m_MainForm.pnlPlayControls.Enabled = TangraContext.Current.HasVideoLoaded;

			m_MainForm.miExportToFits.Enabled = TangraContext.Current.HasAnyFileLoaded;
			m_MainForm.miExportToBMP.Enabled = TangraContext.Current.HasAnyFileLoaded;
			m_MainForm.miExportToCSV.Enabled = TangraContext.Current.HasAnyFileLoaded;

			//m_MainForm.miLoadDark.Enabled = HasAnyFileLoaded && (!(m_MainForm.m_CurrentOperation is MakeDarkFlatField) || CanLoadDarkFrame);
			//m_MainForm.miLoadFlat.Enabled = HasAnyFileLoaded && !(m_MainForm.m_CurrentOperation is MakeDarkFlatField);
			//m_MainForm.miGetLightCurves.Enabled = HasAnyFileLoaded && !(m_MainForm.m_CurrentOperation is LightCurves);
			//m_MainForm.miProduceDarkOrFlat.Enabled = HasVideoLoaded && !(m_MainForm.m_CurrentOperation is MakeDarkFlatField);

			m_MainForm.miReduceLightCurve.Enabled = TangraContext.Current.HasAnyFileLoaded;
			//m_MainForm.miAstrometry.Enabled = HasAnyFileLoaded;
			//m_MainForm.miPixelHistogram.Enabled = HasVideoLoaded;

			//m_MainForm.tbtnArrow.Enabled = CanChangeTool;
			//m_MainForm.tbtnCross.Enabled = CanChangeTool && HasAnyFileLoaded;
			//m_MainForm.tbtnFrameSize.Enabled = CanChangeTool && HasAnyFileLoaded && !OSDExcludeToolDisabled;

			m_MainForm.pnlPlayControls.Enabled = TangraContext.Current.HasVideoLoaded;
			m_MainForm.pnlPlayButtons.Enabled = TangraContext.Current.HasVideoLoaded;
			m_MainForm.btnJumpTo.Enabled = TangraContext.Current.HasVideoLoaded && TangraContext.Current.CanScrollFrames;

			m_MainForm.btnPlay.Enabled = TangraContext.Current.HasVideoLoaded && TangraContext.Current.CanPlayVideo;
			m_MainForm.btnStop.Enabled = TangraContext.Current.HasVideoLoaded && TangraContext.Current.CanPlayVideo;
			m_MainForm.btn10SecMinus.Enabled = TangraContext.Current.HasVideoLoaded && TangraContext.Current.CanScrollFrames;
			m_MainForm.btn10SecPlus.Enabled = TangraContext.Current.HasVideoLoaded && TangraContext.Current.CanScrollFrames;
			m_MainForm.btn1FrMinus.Enabled = TangraContext.Current.HasVideoLoaded && TangraContext.Current.CanScrollFrames;
			m_MainForm.btn1FrPlus.Enabled = TangraContext.Current.HasVideoLoaded && TangraContext.Current.CanScrollFrames;
			m_MainForm.btn1SecMinus.Enabled = TangraContext.Current.HasVideoLoaded && TangraContext.Current.CanScrollFrames;
			m_MainForm.btn1SecPlus.Enabled = TangraContext.Current.HasVideoLoaded && TangraContext.Current.CanScrollFrames;
			
			//m_MainForm.miDetectIntegration.Enabled = HasVideoLoaded && CanPlayVideo;

			m_MainForm.miADVStatusData.Enabled = TangraContext.Current.HasVideoLoaded && TangraContext.Current.UsingADV;
			m_MainForm.miTargetPSFViewer.Enabled = TangraContext.Current.HasVideoLoaded;

		    m_MainForm.miFSTSFileViewer.Enabled = true;

			UpdateConfigurationControls();
		}

		private void UpdateConfigurationControls()
		{
			m_MainForm.ssFPS.Visible = m_FramePlayer.IsRunning;
			if (!m_FramePlayer.IsRunning)
				m_MainForm.ssFPS.Text = string.Empty;

			if (TangraConfig.Settings.Photometry.EncodingGamma != 1 && TangraContext.Current.HasAnyFileLoaded)
            {
                m_MainForm.ssGamma.Visible = true;
                m_MainForm.ssGamma.BackColor = Color.FromArgb(255, 255, 192);
                m_MainForm.ssGamma.Text = string.Format("Gamma = {0}", TangraConfig.Settings.Photometry.EncodingGamma.ToString("0.00"));
            }
            else
                m_MainForm.ssGamma.Visible = false;

			//tslblDarkFrameLoaded.Visible = VideoContext.Current.DarkField != null;
			//tslblFlatFrameLoaded.Visible = VideoContext.Current.FlatField != null;

			if (TangraContext.Current.HasVideoLoaded)
			{
                m_MainForm.tslblUsingAviSynth.Text = TangraContext.Current.RenderingEngine;
				m_MainForm.tslblUsingAviSynth.Visible = true;
			}
			else
				m_MainForm.tslblUsingAviSynth.Visible = false;

			//ssPreProcessing.Visible = PreProcessingInfo != null &&
			//                          PreProcessingInfo.PreProcessing;

			//if (PreProcessingInfo != null && PreProcessingInfo.PreProcessing)
			//{
			//    string preProcessingInfoStr = string.Empty;
			//    switch (PreProcessingInfo.PreProcessingType)
			//    {
			//        case PreProcessingType.BrightnessContrast:
			//            preProcessingInfoStr += string.Format("B{0}:C{1}", PreProcessingInfo.Brigtness, PreProcessingInfo.Contrast);
			//            break;
			//        case PreProcessingType.Clipping:
			//            preProcessingInfoStr += string.Format("C{0}-{1}", PreProcessingInfo.ClippingFrom, PreProcessingInfo.ClippingTo);
			//            break;
			//        case PreProcessingType.Stretching:
			//            preProcessingInfoStr += string.Format("S{0}-{1}", PreProcessingInfo.StretchingFrom, PreProcessingInfo.StretchingTo);
			//            break;
			//        case PreProcessingType.None:
			//            preProcessingInfoStr += "N";
			//            break;
			//    }
			//    if (PreProcessingInfo.GammaCorrection != 1)
			//        preProcessingInfoStr += string.Format("|G{0}", PreProcessingInfo.GammaCorrection.ToString("0.000"));
			//    if (PreProcessingInfo.DarkFrameBytes > 0) preProcessingInfoStr += "|Drk";
			//    if (PreProcessingInfo.FlatFrameBytes > 0) preProcessingInfoStr += "|Flt";
			//    if (PreProcessingInfo.Filter != PreProcessingFilter.NoFilter)
			//    {
			//        if (PreProcessingInfo.Filter == PreProcessingFilter.LowPassFilter)
			//            preProcessingInfoStr += "|LPF";
			//        else if (PreProcessingInfo.Filter == PreProcessingFilter.LowPassDifferenceFilter)
			//            preProcessingInfoStr += "|LPDF";
			//    }

			//    ssPreProcessing.Text = preProcessingInfoStr;
			//}

			//tslblRecDbg.Visible = RecordingDebugSession;
			m_MainForm.tslblIntegration.Text = string.Format("Integrating {0} frames", TangraContext.Current.NumberFramesToIntegrate);
			m_MainForm.tslblIntegration.Visible = TangraContext.Current.UsingIntegration;
			m_MainForm.ssFrameNo.Visible = TangraContext.Current.HasVideoLoaded;			
		}

		public void UpdateVideoSizeAndLengthControls()
		{
			int width = Math.Max(800, TangraContext.Current.FrameWidth + m_ExtraWidth);
			int height = Math.Max(600, TangraContext.Current.FrameHeight + m_ExtraHeight);
			m_MainForm.Width = width;
			m_MainForm.Height = height;

			m_MainForm.scrollBarFrames.Minimum = TangraContext.Current.FirstFrame;
			m_MainForm.scrollBarFrames.Maximum = TangraContext.Current.LastFrame;
		}

		public void Reset()
		{
			TangraContext.Current.Reset();
			Update();
		}

        public void StatusChanged(string displayName)
        {
            m_MainForm.ssStatus.Text = displayName;
        }

		#region File Progress Bar
		private void OnFileProgressStart(int maxValue)
		{
			
			m_MainForm.tsProgessBar.Minimum = 0;
			m_MainForm.tsProgessBar.Maximum = maxValue;
			m_MainForm.tsProgessBar.Value = 0;
			m_MainForm.tsProgessBar.Visible = true;
			m_MainForm.Cursor = Cursors.WaitCursor;
			m_MainForm.statusStrip.Update();			
		}

		private void OnFileProgressEnd()
		{
			m_MainForm.tsProgessBar.Value = m_MainForm.tsProgessBar.Maximum;
			m_MainForm.tsProgessBar.Visible = false;
			m_MainForm.Cursor = Cursors.Default;
			m_MainForm.statusStrip.Update();
		}

		private void OnFileProgress(int currentValue)
		{
			m_MainForm.tsProgessBar.Value = Math.Min(currentValue, m_MainForm.tsProgessBar.Maximum);
			m_MainForm.statusStrip.Update();
		}

		private void HandleFileProgress(int currentValue, int maxValue)
		{
			if (currentValue < 0)
			{
				if (maxValue > 0)
					OnFileProgressStart(maxValue);
				else
					OnFileProgressEnd();
			}
			else
				OnFileProgress(currentValue);				
		}

		internal delegate void OnFileProgressCallback(int currentValue, int maxValue);

		internal void OnFileProgress(int currentValue, int maxValue)
		{
			m_MainForm.Invoke(new OnFileProgressCallback(HandleFileProgress), currentValue, maxValue);		
		}


		internal delegate void OnLongOperationCallback(bool begin);

		internal void OnLongOperation(bool begin)
		{
			m_MainForm.Cursor = begin ? Cursors.WaitCursor : Cursors.Default;
			m_MainForm.Update();
		}

		internal void BeginLongOperation()
		{
			m_MainForm.Invoke(new OnLongOperationCallback(OnLongOperation), true);	
		}

		internal void EndLongOperation()
		{
			m_MainForm.Invoke(new OnLongOperationCallback(OnLongOperation), false);
		}
		#endregion
	}
}
