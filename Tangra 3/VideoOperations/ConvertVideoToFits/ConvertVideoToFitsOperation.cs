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

        private DateTime m_OCRedTimeStamp;
        private bool m_DebugMode;

        public ConvertVideoToFitsOperation()
        {

        }

        public ConvertVideoToFitsOperation(ConvertVideoToFitsController convertVideoToFitsController, bool debugMode)
        {
            m_ConvertVideoToFitsController = convertVideoToFitsController;
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
        private DateTime? m_PrevOCRedTimeStamp = null;
        private DateTime? m_AttachDateToOCR = null;

        public void NextFrame(int frameNo, MovementType movementType, bool isLastFrame, AstroImage astroImage, int firstFrameInIntegrationPeriod, string fileName)
        {
            m_ControlPanel.NextFrame(frameNo, m_Status); 
            
            if (m_Status == ConvertVideoToFitsState.Converting)
            {
                var timestamp = DateTime.MinValue;
                float exposureSeconds = 0;

                if (m_VideoController.HasTimestampOCR())
                {
                    m_OCRedTimeStamp = m_VideoController.OCRTimestamp();

                    timestamp = m_OCRedTimeStamp;
                    if (m_AttachDateToOCR.HasValue)
                    {
                        timestamp = m_AttachDateToOCR.Value.Date.Add(timestamp.TimeOfDay);
                        if (m_PrevOCRedTimeStamp.HasValue && m_PrevOCRedTimeStamp.Value > timestamp &&
                            timestamp.Hour == 23 && timestamp.Hour == 0)
                        {
                            timestamp = timestamp.AddDays(1);
                            m_AttachDateToOCR = m_AttachDateToOCR.Value.AddDays(1);
                        }
                    }

                    if (m_PrevOCRedTimeStamp.HasValue)
                        exposureSeconds = (float) new TimeSpan(timestamp.Ticks - m_PrevOCRedTimeStamp.Value.Ticks).TotalSeconds;
                    else
                    {
                        var nextTimeStamp = m_VideoController.GetNextFrameOCRTimestamp();
                        if (nextTimeStamp != DateTime.MinValue)
                            exposureSeconds = (float)new TimeSpan(nextTimeStamp.TimeOfDay.Ticks - timestamp.TimeOfDay.Ticks).TotalSeconds;
                    }
                    m_PrevOCRedTimeStamp = timestamp;
                }
                else
                {
                    if (m_VideoController.HasEmbeddedTimeStamps())
                    {
                        timestamp = astroImage.Pixelmap.FrameState.CentralExposureTime;
                        exposureSeconds = astroImage.Pixelmap.FrameState.ExposureInMilliseconds / 1000.0f;
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
                    m_VideoController.ShowMessageBox("Export completed.", "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                if (m_LastFrame == frameNo)
                    m_VideoController.StopVideo();
            }
        }

        internal void StartExport(string fileName, bool fitsCube, int firstFrame, int lastFrame, int step, Rectangle roi, UsedTimeBase timeBase)
        {
            m_Status = ConvertVideoToFitsState.Converting;
            m_FirstFrame = firstFrame;
            m_LastFrame = lastFrame;
            m_ConvertVideoToFitsController.StartExport(fileName, fitsCube, roi, timeBase, m_VideoController.HasTimestampOCR(), m_VideoController.OCRTimeStampHasDatePart());

            m_VideoController.PlayVideo(m_FirstFrame, (uint)step);
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

        public void SetAttachDateToOCR(DateTime date)
        {
            m_AttachDateToOCR = date.Date;
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

        public bool HasEmbeddedTimeStamps(out bool hasDatePart)
        {
            hasDatePart = false;
            if (m_VideoController.HasEmbeddedTimeStamps())
                return true;

            m_VideoController.InitializeTimestampOCR();

            if (m_VideoController.HasTimestampOCR())
            {
                hasDatePart = m_VideoController.OCRTimeStampHasDatePart();
                return true;
            }
            else
            {
                m_Status = ConvertVideoToFitsState.EnteringTimes;
                return false;
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
