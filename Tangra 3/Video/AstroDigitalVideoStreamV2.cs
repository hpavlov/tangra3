using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Adv;
using Tangra.Helpers;
using Tangra.Model.Config;
using Tangra.Model.Context;
using Tangra.Model.Image;
using Tangra.Model.Video;
using Tangra.PInvoke;
using Tangra.Video.AstroDigitalVideo;
using AdvFrameInfo = Tangra.PInvoke.AdvFrameInfo;
using Extensions = Tangra.Model.Helpers.Extensions;

namespace Tangra.Video
{
    public class AstroDigitalVideoStreamV2 : IFrameStream, IDisposable
    {
        public static IFrameStream OpenFile(string fileName, out AdvFileMetadataInfo fileMetadataInfo, out GeoLocationInfo geoLocation)
        {
            fileMetadataInfo = new AdvFileMetadataInfo();
            geoLocation = new GeoLocationInfo();
            try
            {
                IFrameStream rv = new AstroDigitalVideoStreamV2(fileName, ref fileMetadataInfo, ref geoLocation);

                TangraContext.Current.RenderingEngine = fileMetadataInfo.Engine == "AAV" ? "AstroAnalogueVideo" : "AstroDigitalVideo";

                if (fileMetadataInfo.Engine == "AAV")
                    UsageStats.Instance.ProcessedAavFiles++;
                else
                    UsageStats.Instance.ProcessedAdvFiles++;
                UsageStats.Instance.Save();

                return rv;
            }
            catch (ADVFormatException ex)
            {
                MessageBox.Show(ex.Message, "Error opening ADV/AAV file", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return null;
        }

        private string m_FileName;
        private int m_Width;
        private int m_Height;
        private double m_FrameRate;

        private int m_FirstFrame;
        private int m_CountFrames;

        private int m_BitPix;
        private uint m_MaxPixelValue;
        private string m_Engine;
        private string m_VideoStandard;
        private double m_NativeFrameRate;
        private double m_EffectiveFrameRate;
        private int m_IntegratedAAVFrames;
        private int m_OsdFirstLine = 0;
        private int m_OsdLastLine = 0;
        private int m_StackingRate = 0;
        private int? m_AAVVersion = null;

        private AdvFile2 m_AdvFile;

        private object m_SyncLock = new object();

        private GeoLocationInfo geoLocation;

        public GeoLocationInfo GeoLocation
        {
            get { return geoLocation; }
        }

        private AstroDigitalVideoStreamV2(string fileName, ref AdvFileMetadataInfo fileMetadataInfo, ref GeoLocationInfo geoLocation)
        {
            //CheckAdvFileFormat(fileName, ref fileMetadataInfo, ref geoLocation);

            m_FileName = fileName;
            
            m_AdvFile = new AdvFile2(fileName);
            
            m_FirstFrame = 0;
            m_CountFrames = m_AdvFile.MainSteamInfo.FrameCount;

            m_BitPix = m_AdvFile.DataBpp;
            m_Width = m_AdvFile.Width;
            m_Height = m_AdvFile.Height;
            m_MaxPixelValue = (uint)m_AdvFile.MaxPixelValue;

            m_FrameRate = 0;

            m_Engine = "ADV2";

            fileMetadataInfo.Recorder = GetFileTag("RECORDER-SOFTWARE");
            CameraModel = fileMetadataInfo.Camera = GetFileTag("CAMERA-MODEL");
            fileMetadataInfo.Engine = GetFileTag("FSTF-TYPE");

            fileMetadataInfo.AdvrVersion = GetFileTag("RECORDER-SOFTWARE-VERSION");
            fileMetadataInfo.SensorInfo = GetFileTag("CAMERA-SENSOR-INFO");

            fileMetadataInfo.ObjectName = GetFileTag("OBJNAME");

            int aavVersion;
            if (int.TryParse(GetFileTag("AAV-VERSION"), out aavVersion))
            {
                m_AAVVersion = aavVersion;
                m_VideoStandard = GetFileTag("NATIVE-VIDEO-STANDARD");
                double.TryParse(GetFileTag("NATIVE-FRAME-RATE"), out m_NativeFrameRate);

                int.TryParse(GetFileTag("OSD-FIRST-LINE"), out m_OsdFirstLine);
                int.TryParse(GetFileTag("OSD-LAST-LINE"), out m_OsdLastLine);

                if (m_OsdLastLine > m_Height) m_OsdLastLine = m_Height;
                if (m_OsdFirstLine < 0) m_OsdFirstLine = 0;

                m_IntegratedAAVFrames = -1;

                if (double.TryParse(GetFileTag("EFFECTIVE-FRAME-RATE"), out m_EffectiveFrameRate) && m_NativeFrameRate != 0)
                {
                    m_IntegratedAAVFrames = (int)Math.Round(m_NativeFrameRate / m_EffectiveFrameRate);
                    m_FrameRate = m_EffectiveFrameRate; // This is important for OCR-ing as the frame rate is used to derive the frame exposure
                }

                int.TryParse(GetFileTag("FRAME-STACKING-RATE"), out m_StackingRate);
                if (m_StackingRate == 1) m_StackingRate = 0; // Video stacked at x1 is a non-stacked video  

                m_Engine = string.Format("AAV{0}", aavVersion);
            }

            this.geoLocation = new GeoLocationInfo()
            {
                //TODO
            };

            geoLocation = this.geoLocation;
        }

        public string GetFileTag(string tagName)
        {
            string tagValue;
            if (m_AdvFile.SystemMetadataTags.TryGetValue(tagName, out tagValue))
                return tagValue;

            if (m_AdvFile.UserMetadataTags.TryGetValue(tagName, out tagValue))
                return tagValue;

            return null;
        }

        public int Width
        {
            get { return m_Width; }
        }

        public int Height
        {
            get { return m_Height; }
        }

        public int BitPix
        {
            get { return m_BitPix; }
        }

        public int FirstFrame
        {
            get { return m_FirstFrame; }
        }

        public int LastFrame
        {
            get { return m_CountFrames - m_FirstFrame - 1; }
        }

        public int CountFrames
        {
            get { return m_CountFrames; }
        }

        public double FrameRate
        {
            get { return m_FrameRate; }
        }

        public double MillisecondsPerFrame
        {
            get { return (1000 / m_FrameRate); }
        }

        public int IntegratedAAVFrames
        {
            get { return m_IntegratedAAVFrames; }
        }

        public int AAVStackingRate
        {
            get { return m_StackingRate; }
        }

        public bool NtpDataAvailable
        {
            get
            {
                // TODO: Implement this once there are sample AAVv2 videos with NTP timestamps
                return false;
            }
        }

        public int AstroAnalogueVideoNormaliseNtpDataIfNeeded(Action<int> progressCallback, out float oneSigmaError)
        {
            int ntpError = -1;
            oneSigmaError = float.NaN;

            // TODO: Implement this once there are sample AAVv2 videos with NTP timestamps
            return ntpError;
        }


        public string VideoStandard
        {
            get { return m_VideoStandard; }
        }

        public Pixelmap GetPixelmap(int index)
        {
            return GetPixelmap(index, 0);
        }

        public Pixelmap GetPixelmap(int index, int streamId)
        {
            if (m_AdvFile.MainSteamInfo.FrameCount == 0) 
                return null;

            uint[] pixels;
            uint[] unprocessedPixels = new uint[m_Width * m_Height];
            byte[] displayBitmapBytes = new byte[m_Width * m_Height];
            byte[] rawBitmapBytes = new byte[(m_Width * m_Height * 3) + 40 + 14 + 1];
            
            Adv.AdvFrameInfo advFrameInfo;
            lock (m_SyncLock)
            {
                if (streamId == 0)
                    pixels = m_AdvFile.GetMainFramePixels((uint)index, out advFrameInfo);
                else if (streamId == 1)
                    pixels = m_AdvFile.GetCalibrationFramePixels((uint)index, out advFrameInfo);
                else
                    throw new ArgumentOutOfRangeException("streamId");

                if (unprocessedPixels.Length != pixels.Length) 
                    throw new ApplicationException("ADV Buffer Error");

                Array.Copy(pixels, unprocessedPixels, pixels.Length);
            }

            TangraCore.PreProcessors.ApplyPreProcessingPixelsOnly(pixels, m_Width, m_Height, m_BitPix, m_MaxPixelValue,(float)(advFrameInfo.UtcExposureMilliseconds / 1000.0));

            TangraCore.GetBitmapPixels(Width, Height, pixels, rawBitmapBytes, displayBitmapBytes, true, (ushort)BitPix, m_MaxPixelValue);

            Bitmap displayBitmap = null;

            if (m_AAVVersion != null && m_IntegratedAAVFrames > 0 && TangraConfig.Settings.AAV.SplitFieldsOSD && m_OsdFirstLine * m_OsdLastLine != 0)
            {
                TangraCore.BitmapSplitFieldsOSD(rawBitmapBytes, m_OsdFirstLine, m_OsdLastLine);
                using (MemoryStream memStr = new MemoryStream(rawBitmapBytes))
                {
                    try
                    {
                        displayBitmap = (Bitmap)Bitmap.FromStream(memStr);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.GetFullStackTrace());
                        displayBitmap = new Bitmap(m_Width, m_Height);
                    }
                }
            }
            else
                displayBitmap = Pixelmap.ConstructBitmapFromBitmapPixels(displayBitmapBytes, Width, Height);

            Pixelmap rv = new Pixelmap(Width, Height, BitPix, pixels, displayBitmap, displayBitmapBytes);
            rv.SetMaxSignalValue(m_MaxPixelValue);
            rv.FrameState = GetCurrentFrameState(advFrameInfo);
            rv.UnprocessedPixels = pixels;
            return rv;
        }

        private FrameStateData GetCurrentFrameState(Adv.AdvFrameInfo frameInfo)
        {
            if (frameInfo != null)
            {
                var rv = new FrameStateData();
                rv.VideoCameraFrameId = (long)frameInfo.VideoCameraFrameId;
                rv.CentralExposureTime = frameInfo.UtcMidExposureTime;
                rv.SystemTime = frameInfo.SystemTimestamp;

                rv.ExposureInMilliseconds = frameInfo.Exposure / 10.0f;

                rv.NumberIntegratedFrames = 0;
                rv.NumberStackedFrames = 0;

                int almanacStatus = frameInfo.GPSAlmanacStatus;
                int almanacOffset = frameInfo.GPSAlmanacOffset;

                //if (!frameInfo.AlmanacStatusIsGood && m_AlamanacOffsetLastFrameIsGood)
                //{
                //    // When the current almanac is not good, but last frame is, then apply the known almanac offset automatically
                //    almanacOffset = m_AlmanacOffsetLastFrame;
                //    rv.CentralExposureTime = rv.CentralExposureTime.AddSeconds(m_AlmanacOffsetLastFrame);
                //    almanacStatus = 2; // Certain
                //}

                rv.Gain = frameInfo.Gain;
                rv.Gamma = frameInfo.Gamma;
                //rv.Temperature = frameInfo.Temperature;
                rv.Offset = frameInfo.Offset;

                rv.NumberSatellites = frameInfo.GPSTrackedSattelites;

                rv.AlmanacStatus = AdvStatusValuesHelper.TranslateGpsAlmanacStatus(almanacStatus);

                rv.AlmanacOffset = AdvStatusValuesHelper.TranslateGpsAlmanacOffset(almanacStatus, almanacOffset, almanacStatus > 0);

                rv.GPSFixStatus = frameInfo.GPSFixStatus.ToString("#");

                rv.Messages = string.Empty;

                if (frameInfo.HasErrorMessage)
                    rv.Messages = Convert.ToString(frameInfo.Status["Error"]);

                rv.IsVtiOsdCalibrationFrame = Convert.ToString(frameInfo.Status["FRAME-TYPE"]) == "VTI-OSD-CALIBRATION";

                return rv;
            }
            else
                return new FrameStateData();
        }

        public FrameStateData GetFrameStatusChannel(int index)
        {
            if (index >= m_FirstFrame + m_CountFrames)
                throw new ApplicationException("Invalid frame position: " + index);

            Adv.AdvFrameInfo advFrameInfo;
            lock (m_SyncLock)
            {
                m_AdvFile.GetMainFramePixels((uint) index, out advFrameInfo);
            }

            return GetCurrentFrameState(advFrameInfo);
        }

        public Pixelmap GetIntegratedFrame(int startFrameNo, int framesToIntegrate, bool isSlidingIntegration, bool isMedianAveraging)
        {
            throw new NotImplementedException();
        }

        public string GetFrameFileName(int index)
        {
            throw new NotSupportedException();
        }

        public int RecommendedBufferSize
        {
            get { return Math.Min(8, CountFrames); }
        }

        public string VideoFileType
        {
            get { return string.Format("{0}.{1}", m_Engine, BitPix); }
        }

        public string Engine
        {
            get { return m_Engine; }
        }

        public string FileName
        {
            get { return m_FileName; }
        }

        public uint GetAav16NormVal()
        {
            return m_MaxPixelValue;
        }

        public bool SupportsSoftwareIntegration
        {
            get { return false; }
        }

        public bool SupportsFrameFileNames
        {
            get { return false; }
        }

        public string CameraModel { get; private set; }

        public void Dispose()
        {
            if (m_AdvFile != null)
            {
                m_AdvFile.Close();
                m_AdvFile = null;
            }
        }
    }
}
