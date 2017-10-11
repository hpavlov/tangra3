/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Tangra.Helpers;
using Tangra.Model.Config;
using Tangra.Model.Context;
using Tangra.Model.Helpers;
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
	    private string m_TangraDisplayVersion;

		public VideoFileView(frmMain mainForm)
		{
			m_MainForm = mainForm;

			m_ExtraWidth = m_MainForm.Width - m_MainForm.pictureBox.Width + 1;
			m_ExtraHeight = m_MainForm.Height - m_MainForm.pictureBox.Height + 1;

		    Version tangraVersion = Assembly.GetExecutingAssembly().GetName().Version;
            bool isBeta = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(BetaReleaseAttribute), false).Length == 1;
            m_TangraDisplayVersion = string.Format("v{0}.{1}{2}", tangraVersion.Major, tangraVersion.Minor, isBeta ? " BETA" : "");
		}

		internal void SetFramePlayer(IFramePlayer framePlayer)
		{
			m_FramePlayer = framePlayer;
		}

		public void Update()
		{
			if (TangraContext.Current.HasAnyFileLoaded)
			{
                m_MainForm.Text = string.Format("Tangra {0} - {1}, {2}", m_TangraDisplayVersion, TangraContext.Current.FileName, TangraContext.Current.FileFormat);
			}
			else
			{
                m_MainForm.Text = string.Format("Tangra {0}", m_TangraDisplayVersion);

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

            m_MainForm.miExportVideoToFITS.Enabled = TangraContext.Current.HasVideoLoaded && TangraContext.Current.CanStartNewOperation;
		    if (TangraContext.Current.IsFitsStream)
                m_MainForm.miExportVideoToFITS.Text = "&Crop FITS Images";
            else
                m_MainForm.miExportVideoToFITS.Text = "&Convert Video to FITS";

            m_MainForm.miConvertVideoToAAV.Enabled = TangraContext.Current.HasVideoLoaded && TangraContext.Current.IsAviFile && TangraContext.Current.CanStartNewOperation;

            m_MainForm.miReduceLightCurve.Enabled = TangraContext.Current.HasAnyFileLoaded && TangraContext.Current.CanStartNewOperation;
            m_MainForm.miAstrometry.Enabled = TangraContext.Current.HasAnyFileLoaded && TangraContext.Current.CanStartNewOperation;
            m_MainForm.miSpectroscopy.Enabled = TangraContext.Current.HasAnyFileLoaded && TangraContext.Current.CanStartNewOperation;
            m_MainForm.miMakeDarkFlat.Enabled = TangraContext.Current.HasAnyFileLoaded && TangraContext.Current.CanStartNewOperation;

            m_MainForm.miLoadDark.Enabled = TangraContext.Current.HasAnyFileLoaded && TangraContext.Current.CanLoadDarkFrame && !TangraContext.Current.OperationInProgress;
            m_MainForm.miLoadMasterDark.Enabled = TangraContext.Current.HasAnyFileLoaded && TangraContext.Current.CanLoadDarkFrame && !TangraContext.Current.OperationInProgress;
            m_MainForm.miLoadDarkLongerExp.Enabled = TangraContext.Current.HasAnyFileLoaded && TangraContext.Current.CanLoadDarkFrame && !TangraContext.Current.OperationInProgress;
            m_MainForm.miLoadFlat.Enabled = TangraContext.Current.HasAnyFileLoaded && TangraContext.Current.CanLoadFlatFrame && !TangraContext.Current.OperationInProgress;
            m_MainForm.miLoadBias.Enabled = TangraContext.Current.HasAnyFileLoaded && TangraContext.Current.CanLoadBiasFrame && !TangraContext.Current.OperationInProgress;

			m_MainForm.pnlPlayControls.Enabled = TangraContext.Current.HasVideoLoaded;
			m_MainForm.pnlPlayButtons.Enabled = TangraContext.Current.HasVideoLoaded;
			m_MainForm.btnJumpTo.Enabled = TangraContext.Current.HasVideoLoaded && TangraContext.Current.CanScrollFrames;

			m_MainForm.btnPlay.Enabled = TangraContext.Current.HasVideoLoaded && TangraContext.Current.CanPlayVideo;
			m_MainForm.btnStop.Enabled = TangraContext.Current.HasVideoLoaded && TangraContext.Current.CanPlayVideo;
            m_MainForm.btn10SecMinus.Enabled = TangraContext.Current.HasVideoLoaded && TangraContext.Current.CanScrollFrames && !TangraContext.Current.UndefinedFrameRate;
            m_MainForm.btn10SecPlus.Enabled = TangraContext.Current.HasVideoLoaded && TangraContext.Current.CanScrollFrames && !TangraContext.Current.UndefinedFrameRate;
			m_MainForm.btn1FrMinus.Enabled = TangraContext.Current.HasVideoLoaded && TangraContext.Current.CanScrollFrames;
			m_MainForm.btn1FrPlus.Enabled = TangraContext.Current.HasVideoLoaded && TangraContext.Current.CanScrollFrames;
            m_MainForm.btn1SecMinus.Enabled = TangraContext.Current.HasVideoLoaded && TangraContext.Current.CanScrollFrames && !TangraContext.Current.UndefinedFrameRate;
            m_MainForm.btn1SecPlus.Enabled = TangraContext.Current.HasVideoLoaded && TangraContext.Current.CanScrollFrames && !TangraContext.Current.UndefinedFrameRate;

            m_MainForm.miFrameStatusData.Enabled = TangraContext.Current.HasVideoLoaded && (TangraContext.Current.UsingADV || TangraContext.Current.IsSerFile || TangraContext.Current.IsFitsStream);
		    if (TangraContext.Current.IsFitsStream) m_MainForm.miFrameStatusData.Text = "FITS &Header Viewer"; 
            else m_MainForm.miFrameStatusData.Text = "Frame &Data Viewer";
			m_MainForm.tsbtnIntensify.Visible = TangraContext.Current.HasVideoLoaded;
			m_MainForm.miTargetPSFViewer.Enabled = TangraContext.Current.HasVideoLoaded;
            m_MainForm.miIntegrationDetection.Enabled = TangraContext.Current.HasVideoLoaded && !TangraContext.Current.UsingADV && !TangraContext.Current.IsSerFile;
		    m_MainForm.miTimestampOCR.Visible = TangraContext.Current.HasVideoLoaded && (!TangraContext.Current.UsingADV || TangraContext.Current.IsAAV2) && !TangraContext.Current.IsSerFile;

		    m_MainForm.miFSTSFileViewer.Enabled = true;
			m_MainForm.miFileInfo.Enabled = TangraContext.Current.HasAnyFileLoaded;

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

            string preProcessingInfoStr = string.Empty;
            string preProcessingInfoTooltip = string.Empty;
		    bool usesReInterlacing = !string.IsNullOrEmpty(TangraContext.Current.ReInterlacingMode);
		    if (usesReInterlacing)
		    {
		        preProcessingInfoStr = TangraContext.Current.ReInterlacingMode;
                preProcessingInfoTooltip += string.Format("Using {0} Re-Interlacing\r\n", TangraContext.Current.ReInterlacingMode);
		    }

		    if (preProcessingInfo != null && preProcessingInfo.PreProcessing)
			{				
				
				switch (preProcessingInfo.PreProcessingType)
				{
					case PreProcessingType.BrightnessContrast:
						preProcessingInfoStr += string.Format("|B{0}:C{1}", preProcessingInfo.Brigtness, preProcessingInfo.Contrast);
						preProcessingInfoTooltip += string.Format("Using Brightness of {0} and Contrast of {1}\r\n", preProcessingInfo.Brigtness, preProcessingInfo.Contrast);
						break;
					case PreProcessingType.Clipping:
						preProcessingInfoStr += string.Format("|C{0}-{1}", preProcessingInfo.ClippingFrom, preProcessingInfo.ClippingTo);
						preProcessingInfoTooltip += string.Format("Using Clipping from {0} to {1}\r\n", preProcessingInfo.ClippingFrom, preProcessingInfo.ClippingTo);
						break;
					case PreProcessingType.Stretching:
						preProcessingInfoStr += string.Format("|S{0}-{1}", preProcessingInfo.StretchingFrom, preProcessingInfo.StretchingTo);
						preProcessingInfoTooltip += string.Format("Using Stretching from {0} to {1}\r\n", preProcessingInfo.StretchingFrom, preProcessingInfo.StretchingTo);
						break;
				}

				if (Math.Abs(preProcessingInfo.GammaCorrection - 1) > 0.01)
				{
					preProcessingInfoStr += string.Format("|G{0}", preProcessingInfo.GammaCorrection.ToString("0.00"));
					preProcessingInfoTooltip += string.Format("Reversing encoding Gamma of {0}\r\n", preProcessingInfo.GammaCorrection.ToString("0.00"));
				}
				if (preProcessingInfo.ReversedCameraResponse != TangraConfig.KnownCameraResponse.Undefined)
				{
                    preProcessingInfoStr += string.Format("|REV:{0}", preProcessingInfo.ReversedCameraResponse.GetEnumDescription());
					preProcessingInfoTooltip += string.Format("Reversing camera response for {0}\r\n", preProcessingInfo.ReversedCameraResponse.ToString());
				}

                if (preProcessingInfo.BiasFrameBytes > 0) preProcessingInfoStr += "|BIAS";
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

			    if (preProcessingInfo.HotPixelsPosCount > 0)
			    {
                    preProcessingInfoStr += string.Format("|HotPix:{0}", preProcessingInfo.HotPixelsPosCount);
			    }
			}

            if (!string.IsNullOrWhiteSpace(preProcessingInfoStr))
		    {
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

            m_MainForm.ssOCR.Visible = TangraContext.Current.OcrExtractingTimestamps;
            if (TangraContext.Current.OcrErrors > 0)
            {
                m_MainForm.ssOCR.ForeColor = Color.Red;
                m_MainForm.ssOCR.Text = string.Format("OCR ERR {0}", TangraContext.Current.OcrErrors);
            }
            else
            {
                m_MainForm.ssOCR.ForeColor = Color.Green;
                m_MainForm.ssOCR.Text = "OCR";
            }

            bool recDbgVisible =  
                TangraContext.Current.AstrometryOCRFailedRead > 0 ||
                TangraContext.Current.AstrometryOCRDroppedFrames > 0 ||
                TangraContext.Current.AstrometryOCRDuplicatedFrames > 0 ||
                TangraContext.Current.AstrometryOCRTimeErrors > 0 ||
                TangraContext.Current.AAVConvertErrors > 0;


            m_MainForm.tslblRecDbg.Visible = recDbgVisible;
		    if (recDbgVisible)
		    {
		        string dbgText;
                if (TangraContext.Current.AstrometryOCRFailedRead > 0)
		        {
                    dbgText = string.Format("TimeStamp Errors. {0} Unread.", TangraContext.Current.AstrometryOCRFailedRead);
		            m_MainForm.tslblRecDbg.ForeColor = Color.Red;
		        }
                else if (TangraContext.Current.AAVConvertErrors > 0)
                {
                    dbgText = string.Format(" {0} Bad AAV Intervals.", TangraContext.Current.AAVConvertErrors);
                    m_MainForm.tslblRecDbg.ForeColor = Color.Red;
                }
                else
                {
                    dbgText = "TimeStamp Warnings. ";
                    m_MainForm.tslblRecDbg.ForeColor = Color.Orange;
                }

		        if (TangraContext.Current.AstrometryOCRTimeErrors > 0)
		            dbgText += string.Format(" {0} Bad.", TangraContext.Current.AstrometryOCRTimeErrors);
                if (TangraContext.Current.AstrometryOCRDroppedFrames > 0)
                    dbgText += string.Format(" {0} Dropped.", TangraContext.Current.AstrometryOCRDroppedFrames);
                if (TangraContext.Current.AstrometryOCRDuplicatedFrames > 0)
                    dbgText += string.Format(" {0} Duplicated.", TangraContext.Current.AstrometryOCRDuplicatedFrames);

                m_MainForm.tslblRecDbg.Text = dbgText;
		    }		        
		}

		public void UpdateVideoSizeAndLengthControls()
		{
			int width = Math.Max(800, TangraContext.Current.FrameWidth + m_ExtraWidth);
			int height = Math.Max(600, TangraContext.Current.FrameHeight + m_ExtraHeight);
			m_MainForm.Width = width;
			m_MainForm.Height = height;

			m_MainForm.scrollBarFrames.Minimum = TangraContext.Current.FirstFrame;
			m_MainForm.scrollBarFrames.Maximum = TangraContext.Current.LastFrame;
		    m_MainForm.scrollBarFrames.SmallChange = 1;
            m_MainForm.scrollBarFrames.LargeChange = 1;
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
