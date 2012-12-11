using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Tangra.Model.Context;
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
#if PRODUCTION
                m_MainForm.miViewTrackingDebug.Visible = false;
                m_MainForm.miOpenFailedCalibration.Visible = false;
				m_MainForm.miOpenFailedPlateSolveFile.Visible = false;
#endif

			m_MainForm.pnlPlayControls.Enabled = TangraContext.Current.HasVideoLoaded;

			m_MainForm.miExportToFits.Enabled = TangraContext.Current.HasAnyFileLoaded;
			m_MainForm.miExportToBMP.Enabled = TangraContext.Current.HasAnyFileLoaded;
			//m_MainForm.miExportToCSV.Enabled = HasAnyFileLoaded;

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
			m_MainForm.btnJumpTo.Enabled = TangraContext.Current.HasVideoLoaded;

			m_MainForm.btnPlay.Enabled = TangraContext.Current.HasVideoLoaded && TangraContext.Current.CanPlayVideo;
			m_MainForm.btnStop.Enabled = TangraContext.Current.HasVideoLoaded && TangraContext.Current.CanPlayVideo;
			m_MainForm.btn10SecMinus.Enabled = TangraContext.Current.HasVideoLoaded && TangraContext.Current.CanScrollFrames;
			m_MainForm.btn10SecPlus.Enabled = TangraContext.Current.HasVideoLoaded && TangraContext.Current.CanScrollFrames;
			m_MainForm.btn1FrMinus.Enabled = TangraContext.Current.HasVideoLoaded && TangraContext.Current.CanScrollFrames;
			m_MainForm.btn1FrPlus.Enabled = TangraContext.Current.HasVideoLoaded && TangraContext.Current.CanScrollFrames;
			m_MainForm.btn1SecMinus.Enabled = TangraContext.Current.HasVideoLoaded && TangraContext.Current.CanScrollFrames;
			m_MainForm.btn1SecPlus.Enabled = TangraContext.Current.HasVideoLoaded && TangraContext.Current.CanScrollFrames;

			//m_MainForm.miDetectIntegration.Enabled = HasVideoLoaded && CanPlayVideo;

			//m_MainForm.miADVDataViewer.Enabled = HasVideoLoaded;
			//m_MainForm.miADVStatusData.Enabled = HasVideoLoaded;
			//m_MainForm.miTargetPSFViewer.Enabled = HasVideoLoaded;

			UpdateConfigurationControls();
		}

		private void UpdateConfigurationControls()
		{
			m_MainForm.ssFPS.Visible = m_FramePlayer.IsRunning;
			if (!m_FramePlayer.IsRunning)
				m_MainForm.ssFPS.Text = string.Empty;

			//ssGamma.Text = string.Format("Gamma = {0}", TangraConfig.Settings.Photometry.EncodingGamma.ToString("0.00"));

			//if (TangraConfig.Settings.Photometry.EncodingGamma != 1)
			//    ssGamma.BackColor = Color.FromArgb(255, 255, 192);
			//else
			//    ssGamma.BackColor = ssFrameNo.BackColor;

			//tslblDarkFrameLoaded.Visible = VideoContext.Current.DarkField != null;
			//tslblFlatFrameLoaded.Visible = VideoContext.Current.FlatField != null;

			//if (UsingDirectShow || UsingAviSynth || UsingADV)
			//{
			//    if (UsingAviSynth)
			//        tslblUsingAviSynth.Text = "AviSynth";
			//    else if (UsingDirectShow)
			//        tslblUsingAviSynth.Text = "DirectShow";
			//    else if (UsingADV)
			//        tslblUsingAviSynth.Text = "ADV";

			//    tslblUsingAviSynth.Visible = true;
			//}
			//else
			//    tslblUsingAviSynth.Visible = false;

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
			//ssFPS.Visible = TangraConfig.Settings.Generic.ShowProcessingSpeed && m_FramePlayer.IsRunning;
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
        
	}
}
