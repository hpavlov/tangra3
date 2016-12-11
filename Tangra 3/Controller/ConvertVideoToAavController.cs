using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Adv;
using Tangra.Helpers;
using Tangra.Model.Astro;
using Tangra.Model.Context;
using Tangra.Model.Image;
using Tangra.Video.AstroDigitalVideo;

namespace Tangra.Controller
{
    public class ConvertVideoToAavController
    {
        private Form m_MainFormView;
        private VideoController m_VideoController;
        private string m_FileName;
        private AdvRecorder m_Recorder = new AdvRecorder();

        private int m_MaxPixelValue;
        private int m_Width;
        private int m_Height;
        private Rectangle m_TestRect;

        private int[,] m_PrevPixels = new int[32, 32];
        private int[,] m_ThisPixels = new int[32, 32];

        private Dictionary<int, double> m_SigmaDict = new Dictionary<int, double>();
        private int m_NextExpectedIntegrationPeriodStartFrameId = -1;
        private int m_FirstIntegrationPeriodStartFrameId = -1;
        private int m_IntegrationPeriod = 1;
        private int m_FirstVtiOsdLine = 0;
        private int m_LastVtiOsdLine = 0;

        private ushort[] m_CurrAavFramePixels;

        public ConvertVideoToAavController(Form mainFormView, VideoController videoController)
		{
			m_MainFormView = mainFormView;
			m_VideoController = videoController;

            AdvError.ShowMessageBoxErrorMessage = true;
		}

        internal void StartConversion(string fileName, int topVtiOsdRow, int bottomVtiOsdRow, int firstIntegratedFrameId, int integrationInterval, string cameraModel, string sensorInfo)
        {
            m_VideoController.ClearAAVConversionErrors();

            m_Width = m_VideoController.FramePlayer.Video.Width;
            m_Height = m_VideoController.FramePlayer.Video.Height;

            m_TestRect = new Rectangle((m_Width / 2) - 16, (m_Height / 2) - 16, 32, 32);

            m_CurrAavFramePixels = new ushort[m_Width * m_Height];

            m_FileName = fileName;
            m_MaxPixelValue = 0xFF * integrationInterval;

            m_FirstVtiOsdLine = topVtiOsdRow;
            m_LastVtiOsdLine = bottomVtiOsdRow;

            m_IntegrationPeriod = integrationInterval;
            m_FirstIntegrationPeriodStartFrameId = firstIntegratedFrameId;
            m_NextExpectedIntegrationPeriodStartFrameId = firstIntegratedFrameId + integrationInterval;

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
            m_Recorder.FileMetaData.AddUserTag("AAV-VERSION", "2");
            m_Recorder.FileMetaData.AddUserTag("AAV16-NORMVAL", m_MaxPixelValue.ToString());

            m_Recorder.FileMetaData.AddCalibrationStreamTag("TYPE", "VTI-OSD-CALIBRATION");
            m_Recorder.FileMetaData.AddCalibrationStreamTag("OSD-FIRST-LINE", topVtiOsdRow.ToString());
            m_Recorder.FileMetaData.AddCalibrationStreamTag("OSD-LAST-LINE", bottomVtiOsdRow.ToString());

            m_Recorder.StatusSectionConfig.RecordSystemErrors = true;
            m_Recorder.StatusSectionConfig.AddDefineTag("FRAME-TYPE", Adv2TagType.UTF8String);
            m_Recorder.StatusSectionConfig.AddDefineTag("FRAMES-IN-INTERVAL", Adv2TagType.Int8);
            m_Recorder.StatusSectionConfig.AddDefineTag("NOISE-SIGNATURES", Adv2TagType.UTF8String);
            m_Recorder.StatusSectionConfig.AddDefineTag("ORIGINAL-FRAME-ID", Adv2TagType.Int32);

            m_Recorder.StartRecordingNewFile(m_FileName, 0, true);
            
            int currFrame = m_VideoController.CurrentFrameIndex;
            try
            {
                m_VideoController.SetPictureBoxCursor(Cursors.WaitCursor);
                m_VideoController.NotifyFileProgress(-1, 16);

                Pixelmap frame;
                ushort[] pixels;

                for (int i = currFrame; i < Math.Min(currFrame + 16, m_VideoController.VideoLastFrame); i++)
                {
                    frame = m_VideoController.GetFrame(i);

                    pixels = frame.Pixels.Select(x => (ushort)(integrationInterval * x)).ToArray();
                    m_Recorder.AddCalibrationFrame(pixels, true,
                        PreferredCompression.Lagarith16,
                        new AdvRecorder.AdvStatusEntry() { AdditionalStatusTags = new[] { "VTI-OSD-CALIBRATION", (object)(byte)0, string.Empty, (object)i } },
                        Adv.AdvImageData.PixelDepth16Bit);

                    m_VideoController.NotifyFileProgress(i - currFrame, 16);
                }

                frame = m_VideoController.GetFrame(m_FirstIntegrationPeriodStartFrameId);

                pixels = frame.Pixels.Select(x => (ushort)(integrationInterval * x)).ToArray();
                m_Recorder.AddCalibrationFrame(pixels, true,
                    PreferredCompression.Lagarith16,
                    new AdvRecorder.AdvStatusEntry() { AdditionalStatusTags = new[] { "FIELD-CALIBRATION", (object)(byte)0, string.Empty, (object)m_FirstIntegrationPeriodStartFrameId } },
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

            m_SigmaDict[frameNo] = sigmaSum / 1024;


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

            if (frameNo > m_FirstIntegrationPeriodStartFrameId + 2*m_IntegrationPeriod)
            {
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
                            return true;
                        }
                    }
                    else if (frameNo == m_NextExpectedIntegrationPeriodStartFrameId)
                    {
                        m_NextExpectedIntegrationPeriodStartFrameId = frameNo + m_IntegrationPeriod;
                        return true;
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
                    new AdvRecorder.AdvStatusEntry() { SystemErrors = errors, AdditionalStatusTags = new[] { "DATA", (object)(byte)m_FramesSoFar, noiseSignatures, (object)(int)0 } },
                    Adv.AdvImageData.PixelDepth16Bit);

                for (int i = 0; i < m_CurrAavFramePixels.Length; i++) 
                    m_CurrAavFramePixels[i] = 0;

                m_FramesSoFar = 0;
            }

            bool copyAllOsdLines = isNewIntegrationPeroiod || frameNo == m_FirstIntegrationPeriodStartFrameId;

            for (int y = 0; y < m_Height; y++)
            {
                bool scale = false;
                if (y >= m_FirstVtiOsdLine && y <= m_LastVtiOsdLine)
                {
                    // NOTE: For first new frame - copy all lines
                    //       For any other frame - copy even lines
                    if (!copyAllOsdLines && (y % 2) == 1) 
                        continue;

                    scale = true;
                }

                for (int x = 0; x < m_Width; x++)
                {
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
