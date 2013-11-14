using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Config;
using Tangra.Model.Context;
using Tangra.Model.Video;
using Tangra.PInvoke;
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

			m_MainForm.pnlPlayControls.Enabled = TangraContext.Current.HasVideoLoaded;

			m_MainForm.miExportToFits.Enabled = TangraContext.Current.HasAnyFileLoaded;
			m_MainForm.miExportToBMP.Enabled = TangraContext.Current.HasAnyFileLoaded;
			m_MainForm.miExportToCSV.Enabled = TangraContext.Current.HasAnyFileLoaded;


			m_MainForm.miReduceLightCurve.Enabled = TangraContext.Current.HasAnyFileLoaded;
			m_MainForm.miMakeDarkFlat.Enabled = TangraContext.Current.HasAnyFileLoaded;

			m_MainForm.miLoadDark.Enabled = TangraContext.Current.HasAnyFileLoaded && TangraContext.Current.CanLoadDarkFrame;
			m_MainForm.miLoadFlat.Enabled = TangraContext.Current.HasAnyFileLoaded && TangraContext.Current.CanLoadFlatFrame;

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
			
			m_MainForm.miADVStatusData.Enabled = TangraContext.Current.HasVideoLoaded && TangraContext.Current.UsingADV;
			m_MainForm.tsbtnIntensify.Visible = TangraContext.Current.HasVideoLoaded;
			m_MainForm.miTargetPSFViewer.Enabled = TangraContext.Current.HasVideoLoaded;

		    m_MainForm.miFSTSFileViewer.Enabled = true;

			UpdateConfigurationControls();
		}

		private void UpdateConfigurationControls()
		{
			m_MainForm.ssFPS.Visible = m_FramePlayer.IsRunning;
			if (!m_FramePlayer.IsRunning)
				m_MainForm.ssFPS.Text = string.Empty;

			if (TangraContext.Current.HasVideoLoaded)
			{
                m_MainForm.ssRenderingEngine.Text = TangraContext.Current.RenderingEngine;
				m_MainForm.ssRenderingEngine.Visible = true;
			}
			else
				m_MainForm.ssRenderingEngine.Visible = false;

			m_MainForm.ssFPS.Visible = TangraConfig.Settings.Generic.ShowProcessingSpeed;

			PreProcessingInfo preProcessingInfo;
			TangraCore.PreProcessors.PreProcessingGetConfig(out preProcessingInfo);

			if (preProcessingInfo != null && preProcessingInfo.PreProcessing)
			{
				string preProcessingInfoStr = string.Empty;
				string preProcessingInfoTooltip = string.Empty;
				switch (preProcessingInfo.PreProcessingType)
				{
					case PreProcessingType.BrightnessContrast:
						preProcessingInfoStr += string.Format("B{0}:C{1}", preProcessingInfo.Brigtness, preProcessingInfo.Contrast);
						preProcessingInfoTooltip += string.Format("Using Brightness of {0} and Contrast of {1}\r\n", preProcessingInfo.Brigtness, preProcessingInfo.Contrast);
						break;
					case PreProcessingType.Clipping:
						preProcessingInfoStr += string.Format("C{0}-{1}", preProcessingInfo.ClippingFrom, preProcessingInfo.ClippingTo);
						preProcessingInfoTooltip += string.Format("Using Clipping from {0} to {1}\r\n", preProcessingInfo.ClippingFrom, preProcessingInfo.ClippingTo);
						break;
					case PreProcessingType.Stretching:
						preProcessingInfoStr += string.Format("S{0}-{1}", preProcessingInfo.StretchingFrom, preProcessingInfo.StretchingTo);
						preProcessingInfoTooltip += string.Format("Using Stretching from {0} to {1}\r\n", preProcessingInfo.StretchingFrom, preProcessingInfo.StretchingTo);
						break;
				}

				if (Math.Abs(preProcessingInfo.GammaCorrection - 1) > 0.01)
				{
					preProcessingInfoStr += string.Format("|G{0}", preProcessingInfo.GammaCorrection.ToString("0.00"));
					preProcessingInfoTooltip += string.Format("Reversing encoding Gamma of {0}\r\n", preProcessingInfo.GammaCorrection.ToString("0.00"));
				}

				if (preProcessingInfo.DarkFrameBytes > 0) preProcessingInfoStr += "|DARK";
				if (preProcessingInfo.FlatFrameBytes > 0) preProcessingInfoStr += "|FLAT";

				if (preProcessingInfo.Filter != TangraConfig.PreProcessingFilter.NoFilter)
				{
					if (preProcessingInfo.Filter == TangraConfig.PreProcessingFilter.LowPassFilter)
					{
						preProcessingInfoStr += "|LPF";
						preProcessingInfoTooltip += "Using Low Pass (LP) filter\r\n";
					}
					else if (preProcessingInfo.Filter == TangraConfig.PreProcessingFilter.LowPassDifferenceFilter)
					{
						preProcessingInfoStr += "|LPDF";
						preProcessingInfoTooltip += "Using Low Pass Difference (LPD) filter\r\n";
					}
				}

				m_MainForm.ssPreProcessing.Text = preProcessingInfoStr.TrimStart('|');
				m_MainForm.ssPreProcessing.ToolTipText = preProcessingInfoTooltip;
				m_MainForm.ssPreProcessing.Visible = true;
			}
			else
			{
				m_MainForm.ssPreProcessing.Visible = false;
			}

			m_MainForm.ssSoftwareIntegration.Text = string.Format("Integrating {0} frames", TangraContext.Current.NumberFramesToIntegrate);
			m_MainForm.ssSoftwareIntegration.Visible = TangraContext.Current.UsingIntegration;
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

        internal void PrintOcrTimeStamps(string oddFieldOSD, string evenFieldOSD)
        {
            m_MainForm.lblOCRTimeStampOdd.Text = oddFieldOSD;
            m_MainForm.lblOCRTimeStampEven.Text = evenFieldOSD;
            m_MainForm.lblOCRTimeStampOdd.Visible = true;
            m_MainForm.lblOCRTimeStampEven.Visible = true;
        }
	}
}
