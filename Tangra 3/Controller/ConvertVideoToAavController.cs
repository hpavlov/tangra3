using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Adv;
using Tangra.Helpers;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Model.Context;
using Tangra.Model.Helpers;
using Tangra.Model.Image;
using Tangra.Video.AstroDigitalVideo;
using Tangra.VideoOperations.ConvertVideoToAav;

namespace Tangra.Controller
{
    public class ConvertVideoToAavController
    {
        internal class DetectionInfo
        {
            public int TotalFrames;

            private int m_DetectedIntervals;
            private int m_DetectionsSinceLastFurther;
            public int DetectedIntervals
            {
                get { return m_DetectedIntervals; }
                set
                {
                    m_DetectedIntervals = value;
                    m_DetectionsSinceLastFurther++;
                }
            }

            public int DetectionsSinceLastFurther
            {
                get { return m_DetectionsSinceLastFurther; }
            }

            public int AssumedIntervals;

            private int m_FurtherDetectedIntervals;
            public int FurtherDetectedIntervals
            {
                get { return m_FurtherDetectedIntervals; }
                set
                {
                    m_FurtherDetectedIntervals = value;
                    m_DetectionsSinceLastFurther = 0;
                }
            }

            public int EarlyDetections;
            public int ErrorDetections;
            public int IntegrationInterval;

            public void Clear(int integrationInterval)
            {
                TotalFrames = 0;
                DetectedIntervals = 0;
                AssumedIntervals = 0;
                FurtherDetectedIntervals = 0;
                EarlyDetections = 0;
                ErrorDetections = 0;
                IntegrationInterval = integrationInterval;
            }
        }

        private Form m_MainFormView;
        private VideoController m_VideoController;
        private string m_FileName;
        private AdvRecorder m_Recorder;

        private int m_MaxPixelValue;
        private int m_Width;
        private int m_Height;
        private int m_EndFieldParity = 1;
        private Rectangle m_TestRect;

        private int[,] m_PrevPixels = new int[32, 32];
        private int[,] m_ThisPixels = new int[32, 32];

        private Dictionary<int, double> m_SigmaDict = new Dictionary<int, double>();
        private int m_NextExpectedIntegrationPeriodStartFrameId = -1;
        private int m_FirstIntegrationPeriodStartFrameId = -1;
        private int m_IntegrationPeriod = 1;
        private int m_FirstVtiOsdLine = 0;
        private int m_LastVtiOsdLine = 0;
        private int m_LeftVtiOsdCol = 0;
        private int m_RightVtiOsdCol = 0;
        private bool m_DontValidateIntegrationIntervals;

        private DetectionInfo m_DetectionInfo = new DetectionInfo();

        private ushort[] m_CurrAavFramePixels;

        public ConvertVideoToAavController(Form mainFormView, VideoController videoController)
		{
			m_MainFormView = mainFormView;
			m_VideoController = videoController;

            AdvError.ShowMessageBoxErrorMessage = true;
		}

        internal void StartConversion(
            string fileName, int topVtiOsdRow, int bottomVtiOsdRow, int leftVtiOsdCol, int rightVtiOsdCol, 
            int firstIntegratedFrameId, int integrationInterval, string cameraModel, string sensorInfo,
            bool swapTimestampFields, bool dontValidateIntegrationIntervals, List<HeaderValuePair> additionalHeaders)
        {
            m_VideoController.ClearAAVConversionErrors();

            m_Width = m_VideoController.FramePlayer.Video.Width;
            m_Height = m_VideoController.FramePlayer.Video.Height;

            m_EndFieldParity = swapTimestampFields ? 1 : 0;

            m_TestRect = new Rectangle((m_Width / 2) - 16, (m_Height / 2) - 16, 32, 32);

            m_CurrAavFramePixels = new ushort[m_Width * m_Height];

            m_FileName = fileName;
            m_MaxPixelValue = 0x100 * integrationInterval;

            m_FirstVtiOsdLine = topVtiOsdRow;
            m_LastVtiOsdLine = bottomVtiOsdRow;
            m_LeftVtiOsdCol = leftVtiOsdCol;
            m_RightVtiOsdCol = rightVtiOsdCol;

            m_IntegrationPeriod = integrationInterval;
            m_FirstIntegrationPeriodStartFrameId = firstIntegratedFrameId;
            m_DontValidateIntegrationIntervals = dontValidateIntegrationIntervals;
            m_NextExpectedIntegrationPeriodStartFrameId = firstIntegratedFrameId + integrationInterval;
            m_FramesSoFar = 0;

            m_DetectionInfo.Clear(m_IntegrationPeriod);

            m_Recorder = new AdvRecorder();
            m_Recorder.ImageConfig.SetImageParameters(
                (ushort)m_Width,
                (ushort)m_Height, 
                16, 
                m_MaxPixelValue);

            m_Recorder.FileMetaData.RecorderSoftwareName = "Tangra";
            m_Recorder.FileMetaData.RecorderSoftwareVersion = VersionHelper.AssemblyFileVersion;
            m_Recorder.FileMetaData.CameraModel = !string.IsNullOrWhiteSpace(cameraModel) ? cameraModel : "Unknown";
            m_Recorder.FileMetaData.CameraSensorInfo = !string.IsNullOrWhiteSpace(sensorInfo) ? sensorInfo : "Unknown";

            var frameRate = 1000.0 / m_VideoController.FramePlayer.Video.MillisecondsPerFrame;
            m_Recorder.FileMetaData.NativeFrameRate = frameRate;
            m_Recorder.FileMetaData.EffectiveFrameRate = 1000.0 / (integrationInterval * m_VideoController.FramePlayer.Video.MillisecondsPerFrame);
            
            var nativeStandards = string.Empty;
            if (Math.Abs(frameRate - 25.0) < 0.1)
                nativeStandards = "PAL";
            else if (Math.Abs(frameRate - 29.97) < 0.1)
                nativeStandards = "NTSC";
            m_Recorder.FileMetaData.AddUserTag("NATIVE-VIDEO-STANDARD", nativeStandards);
            m_Recorder.FileMetaData.AddUserTag("FRAME-COMBINING", "Binning");
            m_Recorder.FileMetaData.AddUserTag("OSD-FIRST-LINE", topVtiOsdRow.ToString());
            m_Recorder.FileMetaData.AddUserTag("OSD-LAST-LINE", bottomVtiOsdRow.ToString());
            m_Recorder.FileMetaData.AddUserTag("OSD-LEFT-COLUMN", leftVtiOsdCol.ToString());
            m_Recorder.FileMetaData.AddUserTag("OSD-RIGHT-COLUMN", rightVtiOsdCol.ToString());
            m_Recorder.FileMetaData.AddUserTag("AAV-VERSION", "2");
            m_Recorder.FileMetaData.AddUserTag("AAV16-NORMVAL", m_MaxPixelValue.ToString());
            m_Recorder.FileMetaData.AddUserTag("INTEGRATION-DETECTION", m_DontValidateIntegrationIntervals ? "Manual" : "Automatic");

            if (additionalHeaders != null)
            {
                foreach (var tag in additionalHeaders)
                {
                    if (!string.IsNullOrWhiteSpace(tag.Header))
                    {
                        m_Recorder.FileMetaData.AddUserTag(tag.Header, tag.Value);
                    }
                }
            }

            m_Recorder.FileMetaData.AddCalibrationStreamTag("TYPE", "VTI-OSD-CALIBRATION");
            m_Recorder.FileMetaData.AddCalibrationStreamTag("OSD-FIRST-LINE", topVtiOsdRow.ToString());
            m_Recorder.FileMetaData.AddCalibrationStreamTag("OSD-LAST-LINE", bottomVtiOsdRow.ToString());
            m_Recorder.FileMetaData.AddCalibrationStreamTag("OSD-LEFT-COLUMN", leftVtiOsdCol.ToString());
            m_Recorder.FileMetaData.AddCalibrationStreamTag("OSD-RIGHT-COLUMN", rightVtiOsdCol.ToString());

            m_Recorder.StatusSectionConfig.RecordSystemErrors = true;
            m_Recorder.StatusSectionConfig.AddDefineTag("FRAME-TYPE", Adv2TagType.UTF8String);
            m_Recorder.StatusSectionConfig.AddDefineTag("FRAMES-IN-INTERVAL", Adv2TagType.Int8);
            m_Recorder.StatusSectionConfig.AddDefineTag("NOISE-SIGNATURES", Adv2TagType.UTF8String);
            m_Recorder.StatusSectionConfig.AddDefineTag("ORIGINAL-FRAME-ID", Adv2TagType.Int32);
            m_Recorder.StatusSectionConfig.AddDefineTag("IntegratedFrames", Adv2TagType.Int8);

            m_Recorder.StartRecordingNewFile(m_FileName, 0, true);
            
            int currFrame = m_VideoController.CurrentFrameIndex;
            int maxOcrCalibrationFramesToSave = TangraConfig.Settings.Generic.MaxCalibrationFieldsToAttempt;
            try
            {
                m_VideoController.SetPictureBoxCursor(Cursors.WaitCursor);
                m_VideoController.NotifyFileProgress(-1, maxOcrCalibrationFramesToSave);

                Pixelmap frame;
                ushort[] pixels;

                for (int i = currFrame; i < Math.Min(currFrame + maxOcrCalibrationFramesToSave, m_VideoController.VideoLastFrame); i++)
                {
                    frame = m_VideoController.GetFrame(i);

                    pixels = frame.Pixels.Select(x => (ushort)(integrationInterval * x)).ToArray();
                    m_Recorder.AddCalibrationFrame(pixels, true,
                        PreferredCompression.Lagarith16,
                        new AdvRecorder.AdvStatusEntry() { AdditionalStatusTags = new[] { "VTI-OSD-CALIBRATION", (object)(byte)0, string.Empty, (object)i, (object)(byte)0 } },
                        Adv.AdvImageData.PixelDepth16Bit);

                    m_VideoController.NotifyFileProgress(i - currFrame, maxOcrCalibrationFramesToSave);
                }

                frame = m_VideoController.GetFrame(m_FirstIntegrationPeriodStartFrameId);

                pixels = frame.Pixels.Select(x => (ushort)(integrationInterval * x)).ToArray();
                m_Recorder.AddCalibrationFrame(pixels, true,
                    PreferredCompression.Lagarith16,
                    new AdvRecorder.AdvStatusEntry() { AdditionalStatusTags = new[] { "FIELD-CALIBRATION", (object)(byte)0, string.Empty, (object)m_FirstIntegrationPeriodStartFrameId, (object)(byte)0 } },
                    Adv.AdvImageData.PixelDepth16Bit);
            }
            finally
            {
                m_VideoController.NotifyFileProgress(-1, 0);
                m_VideoController.SetPictureBoxCursor(Cursors.Default);
            }
        }

        private bool IsNewIntegrationPeriod(int frameNo, AstroImage image)
        {
            m_DetectionInfo.TotalFrames++;

            if (m_DontValidateIntegrationIntervals)
            {
                if (frameNo == m_FirstIntegrationPeriodStartFrameId)
                {
                    m_NextExpectedIntegrationPeriodStartFrameId = frameNo + m_IntegrationPeriod;
                    return false;
                }

                if (m_NextExpectedIntegrationPeriodStartFrameId == frameNo)
                {
                    m_NextExpectedIntegrationPeriodStartFrameId = frameNo + m_IntegrationPeriod;
                    return true;
                }

                return false;
            }

            for (int x = 0; x < 32; x++)
                for (int y = 0; y < 32; y++)
                {
                    m_PrevPixels[x, y] = m_ThisPixels[x, y];
                }

            double sigmaSum = 0;

            for (int x = 0; x < 32; x++)
                for (int y = 0; y < 32; y++)
                {
                    m_ThisPixels[x, y] = (int)image.Pixelmap.Pixels[(x + m_TestRect.Left) + (y + m_TestRect.Top) * m_Width];

                    sigmaSum += Math.Abs(m_ThisPixels[x, y] - m_PrevPixels[x, y]) / 2.0;
                }

            if (frameNo == m_FirstIntegrationPeriodStartFrameId)
                return false;

            var currVal = sigmaSum / 1024;

            m_SigmaDict[frameNo] = currVal;

            var preIdentifiedPeriods = m_IntegrationPeriod == 2 ? 16 : 2;

            if (frameNo > m_FirstIntegrationPeriodStartFrameId + preIdentifiedPeriods * m_IntegrationPeriod)
            {
                if (m_IntegrationPeriod == 2)
                {
                    var evenData = m_SigmaDict.Where(kvp => kvp.Key % 2 == 0).Select(kvp => kvp.Value).ToList();
                    var oddData = m_SigmaDict.Where(kvp => kvp.Key % 2 == 1).Select(kvp => kvp.Value).ToList();

                    var medianEven = evenData.Median();
                    var sigmaEven = Math.Sqrt(evenData.Sum(x => (x - medianEven) * (x - medianEven)) / (evenData.Count - 1));

                    var medianOdd = oddData.Median();
                    var sigmaOdd = Math.Sqrt(oddData.Sum(x => (x - medianOdd) * (x - medianOdd)) / (oddData.Count - 1));

                    double hiMedian, loMedian, hiSigma, loSigma;
                    if (medianEven > medianOdd)
                    {
                        hiMedian = medianEven;
                        loMedian = medianOdd;
                        hiSigma = sigmaEven;
                        loSigma = sigmaOdd;
                    }
                    else
                    {
                        hiMedian = medianOdd;
                        loMedian = medianEven;
                        hiSigma = sigmaOdd;
                        loSigma = sigmaEven;
                    }

                    bool newInterval;
                    if (Math.Abs(hiMedian - currVal) < 3 * hiSigma && Math.Abs(loMedian - currVal) > 3 * loSigma)
                        newInterval = true;
                    else if (Math.Abs(hiMedian - currVal) > 3 * hiSigma && Math.Abs(loMedian - currVal) < 3 * loSigma)
                        newInterval = false;
                    else 
                        newInterval = Math.Abs(hiMedian - currVal) < Math.Abs(loMedian - currVal);

                    if (newInterval && frameNo != m_NextExpectedIntegrationPeriodStartFrameId)
                        m_VideoController.RegisterAAVConversionError();
                    else if (!newInterval && frameNo == m_NextExpectedIntegrationPeriodStartFrameId)
                        m_VideoController.RegisterAAVConversionError();

                    if (newInterval)
                    {
                        m_NextExpectedIntegrationPeriodStartFrameId = frameNo + m_IntegrationPeriod;
                        m_DetectionInfo.DetectedIntervals++;
                    }

                    return newInterval;
                }
                else
                {
                    List<double> vals = m_SigmaDict.Values.Where(v => !double.IsNaN(v)).ToList();

                    vals.Sort();

                    double median = vals.Count % 2 == 1
                        ? vals[vals.Count / 2]
                        : (vals[vals.Count / 2] + vals[(vals.Count / 2) - 1]) / 2;

                    List<double> residuals = vals.Select(v => Math.Abs(median - v)).ToList();

                    double variance = residuals.Select(r => r * r).Sum();
                    if (residuals.Count > 1)
                    {
                        variance = Math.Sqrt(variance / (residuals.Count - 1));
                    }
                    else
                        variance = double.NaN;

                    double residual = Math.Abs(median - m_SigmaDict[frameNo]);
                    if (residual > variance)
                    {
                        if (frameNo < m_NextExpectedIntegrationPeriodStartFrameId)
                        {
                            // New early integration period. This can be right if:
                            // 1) The old expected intergation frame has a smaller residual than this one
                            // 2) The next new frame under the new conditions passes the condition for a new integration frame
                            var nextExpectedResidual = GetFutureFrameNoisePatternResidual(m_NextExpectedIntegrationPeriodStartFrameId, median);
                            var nextNewExpectedResidual = GetFutureFrameNoisePatternResidual(frameNo + m_IntegrationPeriod, median);

                            if (nextExpectedResidual < residual && nextNewExpectedResidual > variance)
                            {
                                m_NextExpectedIntegrationPeriodStartFrameId = frameNo + m_IntegrationPeriod;
                                m_DetectionInfo.DetectedIntervals++;
                                m_DetectionInfo.EarlyDetections++;
                                return true;
                            }
                        }
                        else if (frameNo == m_NextExpectedIntegrationPeriodStartFrameId)
                        {
                            m_NextExpectedIntegrationPeriodStartFrameId = frameNo + m_IntegrationPeriod;
                            m_DetectionInfo.DetectedIntervals++;
                            return true;
                        }
                    }
                    else if (frameNo == m_NextExpectedIntegrationPeriodStartFrameId)
                    {
                        m_NextExpectedIntegrationPeriodStartFrameId += m_IntegrationPeriod;

                        // Unidentified integration period. Look at this further 
                        var nextFrameResidual = GetFutureFrameNoisePatternResidual(frameNo + 1, median);
                        var nextNewExpectedResidual = GetFutureFrameNoisePatternResidual(frameNo + m_IntegrationPeriod, median);

                        if (nextFrameResidual < residual && nextNewExpectedResidual > variance)
                        {
                            // NOTE: If the integration rate has been misidentified by a factor of 2, every second time
                            // we will find that the next future integration boundary will be correct. Need to protect against this type of thing. 

                            if (m_DetectionInfo.FurtherDetectedIntervals > 0 && /* Already had at least one 'further detected' interval */
                                m_DetectionInfo.DetectionsSinceLastFurther < 2 /* We haven't had more than one normally detected intervals since the last 'further detection' */)
                            {
                                // Register an error but still accept the interval
                                m_VideoController.RegisterAAVConversionError();
                            }

                            m_DetectionInfo.FurtherDetectedIntervals++;
                            return true;                                
                        }

                        m_VideoController.RegisterAAVConversionError();
                        return false;
                    }                    
                }
            }
            else
            {
                // NOTE: First 2 integration periods should have been identified correctly from the integration detection
                //       so for them only we don't apply the noise pattern check to allow it to get some more datapoints
                if (frameNo >= m_NextExpectedIntegrationPeriodStartFrameId)
                {
                    while (frameNo >= m_NextExpectedIntegrationPeriodStartFrameId)
                        m_NextExpectedIntegrationPeriodStartFrameId += m_IntegrationPeriod;

                    m_DetectionInfo.AssumedIntervals++;
                    return true;
                }
            }

            return false;
        }

        private double GetFutureFrameNoisePatternResidual(int futureFrameNo, double median)
        {
            var prevImage = m_VideoController.GetFrame(futureFrameNo - 1);
            var thisImage = m_VideoController.GetFrame(futureFrameNo);

            double sigmaSum = 0;

            for (int x = 0; x < 32; x++)
                for (int y = 0; y < 32; y++)
                {
                    sigmaSum += Math.Abs((int)thisImage.Pixels[(x + m_TestRect.Left) + (y + m_TestRect.Top) * m_Width] - (int)prevImage.Pixels[(x + m_TestRect.Left) + (y + m_TestRect.Top) * m_Width]) / 2.0;
                }

            sigmaSum /= 1024;

            return Math.Abs(median - sigmaSum);
        }

        private int m_FramesSoFar;
        internal void ProcessFrame(int frameNo, AstroImage image)
        {
            bool isNewIntegrationPeroiod = IsNewIntegrationPeriod(frameNo, image);
            if (isNewIntegrationPeroiod)
            {
                string errors = null;
                if (m_FramesSoFar != m_IntegrationPeriod)
                {
                    m_VideoController.RegisterAAVConversionError();

                    // Normalize the value during  the conversion
                    for (int i = 0; i < m_CurrAavFramePixels.Length; i++)
                        m_CurrAavFramePixels[i] = (ushort)Math.Round(m_CurrAavFramePixels[i] * 1.0 * m_IntegrationPeriod / m_FramesSoFar);

                    errors = string.Format("{0} frames detected in the current interval", m_FramesSoFar);
                }

                string noiseSignatures = string.Empty;
                if (m_SigmaDict.Count >= m_IntegrationPeriod)
                {
                    for (int i = frameNo - m_IntegrationPeriod + 1; i <= frameNo; i++)
                    {
                        if (m_SigmaDict.ContainsKey(i))
                            noiseSignatures += string.Format("{0}={1:0.00};", i, m_SigmaDict[i]);
                    }
                }

                m_Recorder.AddVideoFrame(m_CurrAavFramePixels, true,
                    PreferredCompression.Lagarith16,
                    new AdvRecorder.AdvStatusEntry() { SystemErrors = errors, AdditionalStatusTags = new[] { "DATA", (object)(byte)m_FramesSoFar, noiseSignatures, (object)(int)0, (object)(byte)m_FramesSoFar } },
                    Adv.AdvImageData.PixelDepth16Bit);

                for (int i = 0; i < m_CurrAavFramePixels.Length; i++) 
                    m_CurrAavFramePixels[i] = 0;

                m_FramesSoFar = 0;
            }

            bool copyAllOsdLines = isNewIntegrationPeroiod || frameNo == m_FirstIntegrationPeriodStartFrameId;

            for (int y = 0; y < m_Height; y++)
            {
                for (int x = 0; x < m_Width; x++)
                {
                    bool scale = false;
                    if (y >= m_FirstVtiOsdLine && y <= m_LastVtiOsdLine && x >= m_LeftVtiOsdCol && x <= m_RightVtiOsdCol)
                    {
                        // This is a location inside the VTI-OSD are that needs to be preserved 

                        // NOTE: For first new frame - copy all lines
                        //       For any other frame - copy even/odd lines only
                        if (!copyAllOsdLines && (y % 2) == m_EndFieldParity)
                            continue;

                        scale = true;
                    }

                    if (scale)
                        m_CurrAavFramePixels[y * m_Width + x] = (ushort)(m_IntegrationPeriod * image.Pixelmap[x, y]);
                    else
                        m_CurrAavFramePixels[y * m_Width + x] += (ushort)(image.Pixelmap[x, y]);
                }
            }

            m_FramesSoFar++;
        }

        internal void FinishedConversion()
        {
            m_Recorder.FinishRecording();
        }
    }
}
