/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Controller;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Model.Context;
using Tangra.Model.Image;
using Tangra.Model.ImageTools;
using Tangra.Model.Video;
using Tangra.Model.VideoOperations;
using Tangra.OCR;
using Tangra.VideoOperations.LightCurves;
using Tangra.VideoOperations.MakeDarkFlatField;

namespace Tangra.VideoOperations.ConvertVideoToFits
{
    public enum ConvertVideoToFitsState
    {
        Configuring,
        EnteringTimes,
        Converting,
        Finished
    }

    public class ConvertVideoToFitsOperation : VideoOperationBase, IVideoOperation
    {
        private ConvertVideoToFitsController m_ConvertVideoToFitsController;
        private VideoController m_VideoController;
        private ucConvertVideoToFits m_ControlPanel = null;
        private object m_SyncRoot = new object();

        private ConvertVideoToFitsState m_Status;

        private OcrExtensionManager m_OcrExtensionManager;
        private ITimestampOcr m_TimestampOCR;
        private DateTime m_OCRedTimeStamp;
        private bool m_DebugMode;

        public ConvertVideoToFitsOperation()
        {

        }

        public ConvertVideoToFitsOperation(ConvertVideoToFitsController convertVideoToFitsController, OcrExtensionManager ocrExtensionManager, bool debugMode)
        {
            m_ConvertVideoToFitsController = convertVideoToFitsController;
            m_OcrExtensionManager = ocrExtensionManager;
            m_DebugMode = debugMode;
        }

        public bool InitializeOperation(IVideoController videoController, Panel controlPanel, IFramePlayer framePlayer, Form topForm)
        {
            m_Status = ConvertVideoToFitsState.Configuring;
            m_VideoController = (VideoController)videoController;

            // We don't allow loading of calibration frames for now. Doing so with complicate the export
            TangraContext.Current.CanLoadFlatFrame = false;
            TangraContext.Current.CanLoadDarkFrame = false;
            TangraContext.Current.CanLoadBiasFrame = false;

            if (m_ControlPanel == null)
            {
                lock (m_SyncRoot)
                {
                    if (m_ControlPanel == null)
                    {
                        m_ControlPanel = new ucConvertVideoToFits(this, (VideoController)videoController);
                    }
                }
            }

            controlPanel.Controls.Clear();
            controlPanel.Controls.Add(m_ControlPanel);
            m_ControlPanel.Dock = DockStyle.Fill;

            return true;
        }

        public void FinalizeOperation()
        { }

        public void PlayerStarted()
        { }

        private int m_FirstFrame;        
        private int m_LastFrame;

        public void NextFrame(int frameNo, MovementType movementType, bool isLastFrame, AstroImage astroImage, int firstFrameInIntegrationPeriod, string fileName)
        {
            m_ControlPanel.NextFrame(frameNo, m_Status); 
            
            if (m_Status == ConvertVideoToFitsState.Converting)
            {
                var timestamp = DateTime.MinValue;
                float exposureSeconds = 0;

                if (m_TimestampOCR != null)
                {
                    uint[] osdPixels = astroImage.GetPixelmapPixels();

                    if (m_TimestampOCR.RequiresConfiguring)
                        m_TimestampOCR.TryToAutoConfigure(osdPixels);

                    if (!m_TimestampOCR.RequiresConfiguring)
                    {
                        if (!m_TimestampOCR.ExtractTime(frameNo, osdPixels, out m_OCRedTimeStamp))
                        {
                            m_OCRedTimeStamp = DateTime.MinValue;
                        }
                    }

                    timestamp = m_OCRedTimeStamp;
                }
                else
                {
                    if (m_VideoController.HasEmbeddedTimeStamps())
                    {
                        timestamp = astroImage.Pixelmap.FrameState.CentralExposureTime;
                    }
                    else
                    {
                        timestamp = m_StartFrameTime.AddTicks((long)(frameNo - m_StartTimeFrame) * (m_EndFrameTime.Ticks - m_StartFrameTime.Ticks) / (m_EndTimeFrame - m_StartTimeFrame));
                        exposureSeconds = (float)(new TimeSpan((m_EndFrameTime.Ticks - m_StartFrameTime.Ticks) / (m_EndTimeFrame - m_StartTimeFrame)).TotalSeconds);
                    }
                }

                m_ConvertVideoToFitsController.ProcessFrame(frameNo, astroImage, timestamp, exposureSeconds);

                if (isLastFrame || m_LastFrame == frameNo)
                {
                    m_ConvertVideoToFitsController.FinishExport();
                    m_ControlPanel.ExportFinished();
                    m_Status = ConvertVideoToFitsState.Finished;
                }

                if (m_LastFrame == frameNo)
                    m_VideoController.StopVideo();
            }
        }

        internal void StartExport(string fileName, bool fitsCube, int firstFrame, int lastFrame, Rectangle roi, UsedTimeBase timeBase)
        {
            m_Status = ConvertVideoToFitsState.Converting;
            m_FirstFrame = firstFrame;
            m_LastFrame = lastFrame;
            m_ConvertVideoToFitsController.StartExport(fileName, fitsCube, roi, timeBase, m_TimestampOCR != null);

            m_VideoController.PlayVideo(m_FirstFrame);
        }

        private DateTime m_StartFrameTime;
        private DateTime m_EndFrameTime;
        private int m_StartTimeFrame;
        private int m_EndTimeFrame;

        public void SetStartTime(DateTime startTime, int frameNo)
        {
            m_StartFrameTime = startTime;
            m_StartTimeFrame = frameNo;
        }

        public void SetEndTime(DateTime endTime, int frameNo)
        {
            m_EndFrameTime = endTime;
            m_EndTimeFrame = frameNo;
        }

        public DialogResult EnteredTimeIntervalLooksOkay()
        {
            if (m_VideoController.IsAstroAnalogueVideo && !m_VideoController.AstroAnalogueVideoHasOcrOrNtpData)
                return CheckAavRate();
            else
                return CheckPALOrNTSCRate();
        }

        public DialogResult CheckAavRate()
        {
            int totalIntegratedFrames = 0;
            for (int i = m_StartTimeFrame; i < m_EndTimeFrame; i++)
            {
                FrameStateData frameState = m_VideoController.GetFrameStateData(i);

                totalIntegratedFrames += frameState.NumberIntegratedFrames.Value;
            }

            // The actual integration could even be of PAL or NTSC frames

            TimeSpan ts = new TimeSpan(Math.Abs(m_EndFrameTime.Ticks - m_StartFrameTime.Ticks)/*Taking ABS to handle backwards measuring*/);
            double videoTimeInSecPAL = totalIntegratedFrames / 25.0;
            double videoTimeInSecNTSC = totalIntegratedFrames / 29.97;

            bool isTimeOk =
                (videoTimeInSecPAL > 0 && Math.Abs((videoTimeInSecPAL - ts.TotalSeconds) * 1000) < TangraConfig.Settings.Special.MaxAllowedTimestampShiftInMs) ||
                (videoTimeInSecNTSC > 0 && Math.Abs((videoTimeInSecNTSC - ts.TotalSeconds) * 1000) < TangraConfig.Settings.Special.MaxAllowedTimestampShiftInMs);

            if (!isTimeOk)
            {
                if (MessageBox.Show(
                    string.Format(
                        "The time computed from the measured number of frames in this AAV video is off by more than {0} ms from the entered time. This may indicate " +
                        "incorrectly entered start or end time or an almanac update or a leap second event. Please enter the start and end times again. The export operation can " +
                        "only continute if the times match exactly.",
                        (TangraConfig.Settings.Special.MaxAllowedTimestampShiftInMs).ToString("0.00")),
                    "Error",
                    MessageBoxButtons.RetryCancel, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1) == DialogResult.Retry)
                {
                    return DialogResult.Retry;
                }

                return DialogResult.Abort;
            }

            return DialogResult.OK;
        }


        public DialogResult CheckPALOrNTSCRate()
        {
            if (m_VideoController.VideoFrameRate < 24.0 || m_VideoController.VideoFrameRate > 31.0)
            {
                MessageBox.Show(
                    string.Format("This video has an unusual frame rate of {0}. Tangra cannot run internal checks for the correctness of the entered frame times. The export operation cannot continue!", m_VideoController.VideoFrameRate.ToString("0.00")),
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return DialogResult.Abort;
            }

            double acceptedVideoFrameRate = m_VideoController.VideoFrameRate > 24.0 && m_VideoController.VideoFrameRate < 26.0
                                                ? 25.0 /* PAL */
                                                : 29.97 /* NTSC */;

            string videoType = "Unusual Framerate";
            if (m_VideoController.VideoFrameRate > 24.0 && m_VideoController.VideoFrameRate < 26.0) videoType = "PAL";
            else if (m_VideoController.VideoFrameRate > 29.0 && m_VideoController.VideoFrameRate < 31.0) videoType = "NTSC";

            TimeSpan ts = new TimeSpan(Math.Abs(m_EndFrameTime.Ticks - m_StartFrameTime.Ticks)/*Taking ABS to handle backwards measuring*/);
            double videoTimeInSec = (m_EndTimeFrame - m_StartTimeFrame) / acceptedVideoFrameRate;

            if (m_VideoController.IsPlainAviVideo && (videoTimeInSec < 0 || Math.Abs((videoTimeInSec - ts.TotalSeconds) * 1000) > TangraConfig.Settings.Special.MaxAllowedTimestampShiftInMs))
            {
                if (MessageBox.Show(
                    string.Format(
                        "The time computed from the measured number of frames in this {1} video is off by more than {0} ms from the entered time. This may indicate " +
                        "incorrectly entered start or end time or an almanac update or a leap second event. Please enter the start and end times again. The export operation can " +
                        "only continute if the times match exactly.",
                        (TangraConfig.Settings.Special.MaxAllowedTimestampShiftInMs).ToString("0.00"),
                        videoType),
                    "Error",
                    MessageBoxButtons.RetryCancel, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1) == DialogResult.Retry)
                {
                    return DialogResult.Retry;
                }

                return DialogResult.Abort;
            }

            double derivedFrameRate = (m_EndTimeFrame - m_StartTimeFrame) / ts.TotalSeconds;

            // 1) compute 1 ms plus 1ms for each 30 sec up to the max of 4ms. e.g. if this is a PAL video and we have measured 780 frames, this makes 780 / 25fps (PAL) = 31.2 sec. So we take excess = 1 + 1 sec.
            // 2) max allowed difference = 1.33 * Module[video frame rate - (video frame rate * num frames + excess) / num frames]
            int allowedExcess = 1 + Math.Min(4, (int)((m_EndTimeFrame - m_StartTimeFrame) / acceptedVideoFrameRate) / 30);
            double maxAllowedFRDiff = 1.33 * Math.Abs(acceptedVideoFrameRate - ((acceptedVideoFrameRate * (m_EndTimeFrame - m_StartTimeFrame) + allowedExcess) / (m_EndTimeFrame - m_StartTimeFrame)));

            if (m_VideoController.IsPlainAviVideo && Math.Abs(derivedFrameRate - acceptedVideoFrameRate) > maxAllowedFRDiff)
            {
                if (MessageBox.Show(
                    "Based on your entered frame times it appears that there may be dropped frames, incorrectly entered start " +
                    "or end time, an almanac update or a leap second event. Please enter the start and end times again. The export operation can " +
                        "only continute if the times match exactly.",
                    "Error",
                    MessageBoxButtons.RetryCancel, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1) == DialogResult.Retry)
                {
                    return DialogResult.Retry;
                }

                return DialogResult.Abort;
            }

            return DialogResult.OK;
        }

        public bool HasEmbeddedTimeStamps()
        {
            if (m_VideoController.HasEmbeddedTimeStamps())
                return true;

            InitializeTimestampOCR();

            if (m_TimestampOCR != null)
                return true;
            else
            {
                m_Status = ConvertVideoToFitsState.EnteringTimes;
                return false;
            }
        }

        private void InitializeTimestampOCR()
        {
            m_TimestampOCR = null;

            if (m_VideoController.IsVideoWithVtiOsdTimeStamp &&
                (TangraConfig.Settings.Generic.OsdOcrEnabled || TangraConfig.Settings.Generic.OcrAskEveryTime))
            {
                bool forceSaveErrorReport = false;

                if (TangraConfig.Settings.Generic.OcrAskEveryTime)
                {
                    var frm = new frmChooseOcrEngine(m_OcrExtensionManager);
                    frm.StartPosition = FormStartPosition.CenterParent;
                    frm.ShowForceErrorReportOption = m_DebugMode;
                    if (m_VideoController.ShowDialog(frm) == DialogResult.Cancel ||
                        !TangraConfig.Settings.Generic.OsdOcrEnabled)
                        return;

                    forceSaveErrorReport = frm.ForceErrorReport;
                }

                m_TimestampOCR = m_OcrExtensionManager.GetCurrentOcr();

                if (m_TimestampOCR != null)
                {
                    var data = new TimestampOCRData();
                    data.FrameWidth = TangraContext.Current.FrameWidth;
                    data.FrameHeight = TangraContext.Current.FrameHeight;
                    data.OSDFrame = LightCurveReductionContext.Instance.OSDFrame;
                    data.VideoFrameRate = (float)m_VideoController.VideoFrameRate;
                    data.ForceErrorReport = forceSaveErrorReport;

                    m_TimestampOCR.Initialize(data, m_VideoController,
#if WIN32
 (int)TangraConfig.Settings.Tuning.OcrMode
#else
                        (int)TangraConfig.OCRMode.FullyManaged
#endif
);

                    int maxCalibrationFieldsToAttempt = TangraConfig.Settings.Generic.MaxCalibrationFieldsToAttempt;

                    if (m_TimestampOCR.RequiresCalibration)
                    {
                        // NOTE: If we are measuring the video backwards the OCR will need to be intialized forward (i.e. double backwards)
                        bool measuringBackwards = LightCurveReductionContext.Instance.LightCurveReductionType == LightCurveReductionType.TotalLunarReppearance;

                        int firstCalibrationFrame = measuringBackwards
                            ? Math.Min(m_VideoController.CurrentFrameIndex + 30, m_VideoController.VideoLastFrame)
                            : m_VideoController.CurrentFrameIndex;

                        Pixelmap frame = m_VideoController.GetFrame(firstCalibrationFrame);
                        m_TimestampOCR.ProcessCalibrationFrame(m_VideoController.CurrentFrameIndex, frame.Pixels);

                        bool isCalibrated = true;

                        if (m_TimestampOCR.InitiazliationError == null)
                        {
                            int calibrationFramesProcessed = 0;
                            m_VideoController.StatusChanged("Calibrating OCR");
                            FileProgressManager.BeginFileOperation(maxCalibrationFieldsToAttempt);
                            try
                            {
                                var processingMethod = new Func<int, bool>(delegate(int i)
                                {
                                    if (m_TimestampOCR == null)
                                        return false;

                                    frame = m_VideoController.GetFrame(i);
                                    isCalibrated = m_TimestampOCR.ProcessCalibrationFrame(i, frame.Pixels);

                                    if (m_TimestampOCR.InitiazliationError != null)
                                    {
                                        // This doesn't like like what the OCR engine is expecting. Abort ....
                                        m_TimestampOCR = null;
                                        return false;
                                    }

                                    calibrationFramesProcessed++;

                                    FileProgressManager.FileOperationProgress(calibrationFramesProcessed);

                                    if (isCalibrated)
                                        return true;

                                    if (calibrationFramesProcessed > maxCalibrationFieldsToAttempt)
                                        return true;

                                    return false;
                                });

                                if (measuringBackwards)
                                {
                                    for (int i = firstCalibrationFrame - 1; i > m_VideoController.CurrentFrameIndex; i--)
                                    {
                                        if (processingMethod(i))
                                            break;
                                    }
                                }
                                else
                                {
                                    for (int i = firstCalibrationFrame + 1; i < m_VideoController.VideoLastFrame; i++)
                                    {
                                        if (processingMethod(i))
                                            break;
                                    }
                                }
                            }
                            finally
                            {
                                FileProgressManager.EndFileOperation();
                            }
                        }

                        if (forceSaveErrorReport || !isCalibrated)
                        {
                            var frmReport = new frmOsdOcrCalibrationFailure();
                            frmReport.StartPosition = FormStartPosition.CenterParent;
                            frmReport.TimestampOCR = m_TimestampOCR;
                            frmReport.ForcedErrorReport = forceSaveErrorReport;

                            if (frmReport.CanSendReport())
                                m_VideoController.ShowDialog(frmReport);

                            m_TimestampOCR = null;
                        }
                        else if (m_TimestampOCR != null)
                        {
                            if (m_TimestampOCR.InitiazliationError != null)
                            {
                                m_VideoController.ShowMessageBox(
                                    m_TimestampOCR.InitiazliationError,
                                    "Error reading OSD timestamp",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);

                                m_TimestampOCR = null;
                            }
                        }
                    }
                }
            }
        }

        public void ImageToolChanged(ImageTool newTool, ImageTool oldTool)
        {
            //TODO:
        }

        public void PreDraw(Graphics g)
        {
            //TODO:
        }

        public void PostDraw(Graphics g)
        {
            if (m_TimestampOCR != null)
            {
                // Plot the positions of the timestamp blocks
                m_TimestampOCR.DrawLegend(g);
            }
        }

        public bool HasCustomZoomImage 
        {
            get
            {
                return false;
            }
        }

        public bool DrawCustomZoomImage(Graphics g, int width, int height)
        {
            return false;
        }

        public bool AvoidImageOverlays
        {
            get
            {
                // No overlays allowed during the whole process
                return true;
            }
        }
    }
}
