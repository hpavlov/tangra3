using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Adv;
using Tangra.Helpers;
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
        private uint m_Aav16NormVal;
        private string m_Engine;

        private AdvFile2 m_AdvFile;

        private object m_SyncLock = new object();

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
            m_Aav16NormVal = (uint)m_AdvFile.MaxPixelValue;

            m_FrameRate = 0;

            m_Engine = "ADV2";

            fileMetadataInfo.Recorder = GetFileTag("RECORDER-SOFTWARE");
            fileMetadataInfo.Camera = GetFileTag("CAMERA-MODEL");
            fileMetadataInfo.Engine = GetFileTag("FSTF-TYPE");

            fileMetadataInfo.AdvrVersion = GetFileTag("RECORDER-SOFTWARE-VERSION");
            fileMetadataInfo.SensorInfo = GetFileTag("CAMERA-SENSOR-INFO");

            fileMetadataInfo.ObjectName = GetFileTag("OBJNAME");
        }

        public string GetFileTag(string tagName)
        {
            string tagValue;
            if (m_AdvFile.SystemMetadataTags.TryGetValue(tagName, out tagValue))
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

        public Pixelmap GetPixelmap(int index)
        {
            uint[] pixels;
            uint[] unprocessedPixels = new uint[m_Width * m_Height];
            byte[] displayBitmapBytes = new byte[m_Width * m_Height];
            byte[] rawBitmapBytes = new byte[(m_Width * m_Height * 3) + 40 + 14 + 1];
            
            Adv.AdvFrameInfo advFrameInfo;
            lock (m_SyncLock)
            {
                pixels = m_AdvFile.GetMainFramePixels((uint)index, out advFrameInfo);

                if (unprocessedPixels.Length != pixels.Length) 
                    throw new ApplicationException("ADV Buffer Error");

                Array.Copy(pixels, unprocessedPixels, pixels.Length);
            }

            TangraCore.PreProcessors.ApplyPreProcessingPixelsOnly(pixels, m_Width, m_Height, m_BitPix, m_Aav16NormVal,(float)(advFrameInfo.UtcExposureMilliseconds / 1000.0));

            TangraCore.GetBitmapPixels(Width, Height, pixels, rawBitmapBytes, displayBitmapBytes, true, (ushort)BitPix, 0);

            Bitmap displayBitmap = Pixelmap.ConstructBitmapFromBitmapPixels(displayBitmapBytes, Width, Height);

            Pixelmap rv = new Pixelmap(Width, Height, BitPix, pixels, displayBitmap, displayBitmapBytes);
            rv.SetMaxSignalValue(m_Aav16NormVal);
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

                return rv;
            }
            else
                return new FrameStateData();
        }
        
        private FrameStateData GetCurrentFrameState(Adv2FrameInfoNative frameInfo)
        {
            if (frameInfo != null)
            {
                var rv = new FrameStateData();
                rv.VideoCameraFrameId = frameInfo.VideoCameraFrameId;
                rv.CentralExposureTime = frameInfo.MiddleExposureTimeStamp;
                rv.SystemTime = frameInfo.SystemTime;

                rv.ExposureInMilliseconds = frameInfo.Exposure / 10.0f;

                rv.NumberIntegratedFrames = 0;
                rv.NumberStackedFrames = 0;

                int almanacStatus = frameInfo.GPSAlmanacStatus;
                int almanacOffset = frameInfo.GetSignedAlamancOffset();

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

                rv.Messages = string.Empty; // TODO: This is currently not coming through

                if (m_FrameRate > 0)
                    rv.ExposureInMilliseconds = (float)(1000 / m_FrameRate);

                return rv;
            }
            else
                return new FrameStateData();
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
            return m_Aav16NormVal;
        }

        public bool SupportsSoftwareIntegration
        {
            get { return false; }
        }

        public bool SupportsFrameFileNames
        {
            get { return false; }
        }

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
