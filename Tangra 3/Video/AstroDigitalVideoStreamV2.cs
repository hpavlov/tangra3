using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Helpers;
using Tangra.Model.Context;
using Tangra.Model.Helpers;
using Tangra.Model.Image;
using Tangra.Model.Video;
using Tangra.PInvoke;

namespace Tangra.Video
{
    public class AstroDigitalVideoStreamV2 : IFrameStream
    {
        public static IFrameStream OpenFile(string fileName, out AdvEquipmentInfo equipmentInfo, out GeoLocationInfo geoLocation)
        {
            equipmentInfo = new AdvEquipmentInfo();
            geoLocation = new GeoLocationInfo();
            try
            {
                IFrameStream rv = new AstroDigitalVideoStreamV2(fileName, ref equipmentInfo, ref geoLocation);

                TangraContext.Current.RenderingEngine = equipmentInfo.Engine == "AAV" ? "AstroAnalogueVideo" : "AstroDigitalVideo";

                if (equipmentInfo.Engine == "AAV")
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

        private object m_SyncLock = new object();

        private AstroDigitalVideoStreamV2(string fileName, ref AdvEquipmentInfo equipmentInfo, ref GeoLocationInfo geoLocation)
        {
            //CheckAdvFileFormat(fileName, ref equipmentInfo, ref geoLocation);

            m_FileName = fileName;
            var fileInfo = new Adv2FileInfo();

            TangraCore.ADV2OpenFile(fileName, ref fileInfo);

            m_FirstFrame = 0;
            m_CountFrames = fileInfo.CountMaintFrames;

            m_BitPix = fileInfo.DataBpp;
            m_Width = fileInfo.Width;
            m_Height = fileInfo.Height;
            m_Aav16NormVal = (uint)fileInfo.MaxPixelValue;

            m_FrameRate = 0;

            m_Engine = "ADV2";
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
            uint[] pixels = new uint[m_Width * m_Height];
            uint[] unprocessedPixels = new uint[m_Width * m_Height];
            byte[] displayBitmapBytes = new byte[m_Width * m_Height];
            byte[] rawBitmapBytes = new byte[(m_Width * m_Height * 3) + 40 + 14 + 1];
            var frameInfo = new Adv2FrameInfoNative();

            lock (m_SyncLock)
            {
                TangraCore.ADV2GetFrame(index, pixels, unprocessedPixels, rawBitmapBytes, displayBitmapBytes, frameInfo);
            }

            using (MemoryStream memStr = new MemoryStream(rawBitmapBytes))
            {
                Bitmap displayBitmap;

                try
                {
                    displayBitmap = (Bitmap)Bitmap.FromStream(memStr);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.GetFullStackTrace());
                    displayBitmap = new Bitmap(m_Width, m_Height);
                }

                var rv = new Pixelmap(m_Width, m_Height, m_BitPix, pixels, displayBitmap, displayBitmapBytes);
                rv.SetMaxSignalValue(m_Aav16NormVal);
                //rv.FrameState = GetCurrentFrameState(index);
                rv.UnprocessedPixels = unprocessedPixels;
                return rv;
            }
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
    }
}
