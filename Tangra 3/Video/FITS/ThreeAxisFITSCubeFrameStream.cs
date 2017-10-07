using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using nom.tam.fits;
using nom.tam.util;
using Tangra.Controller;
using Tangra.Helpers;
using Tangra.Model.Helpers;
using Tangra.Model.Image;
using Tangra.Model.Video;
using Tangra.Model.VideoOperations;

namespace Tangra.Video.FITS
{
    internal class ThreeAxisFITSCubeFrameStream : IDisposable, IFrameStream, IFITSStream
    {
        internal const string CUBE_3D_FITS_FILE_ENGINE = "FITS-Cube3D";

        private Fits m_FitsFile;
        private BufferedFile m_BufferedFile;
        private BasicHDU m_ImageHDU;

        private int m_NumFrames;

        private int m_HeightIndex;
        private int m_WidthIndex;
        private int m_FrameIndex;

        private int m_CurrentFrameIndex;
        private string m_FileName;

        private int m_Width;
        private int m_Height;
        private int m_Bpp;
        private int m_BZero;
        private int m_NegPixCorrection;
        
        private FITSTimeStampReader m_TimeStampReader;

        private short m_MinPixelValue;
        private uint m_MaxPixelValue;
        
        private double m_ExposureSeconds;
        private DateTime m_FirstFrameMidTime;

        private Array m_ArrayData;

        private Dictionary<string, string> m_Cards = new Dictionary<string, string>();

        public static ThreeAxisFITSCubeFrameStream OpenFile(string fileName, VideoController videoController)
        {
            var fitsFile = new Fits();
            var bufferedFile = new BufferedFile(fileName, FileAccess.Read, FileShare.ReadWrite);
            fitsFile.Read(bufferedFile);

            var imageHDU = fitsFile.GetHDU(0);
            ThreeAxisFITSCubeFrameStream fitsStream = null;

            videoController.SetCursor(Cursors.WaitCursor);

            try
            {
                var hasher = new SHA1CryptoServiceProvider();
                hasher.Initialize();
                byte[] combinedFileNamesBytes = Encoding.UTF8.GetBytes(fileName);
                var hash = hasher.ComputeHash(combinedFileNamesBytes, 0, combinedFileNamesBytes.Length);
                var fitsFileHash = Convert.ToBase64String(hash);

                var frm = new frmDefineFitsCube3D(imageHDU, fitsFileHash, videoController);
                if (videoController.ShowDialog(frm) == DialogResult.OK)
                {
                    fitsStream = new ThreeAxisFITSCubeFrameStream(
                        fileName, fitsFile, bufferedFile, imageHDU, frm.TimeStampReader,
                        frm.WidthIndex, frm.HeightIndex, frm.FrameIndex,
                        frm.MinPixelValue, frm.MaxPixelValue, frm.BitPix, frm.NegPixCorrection);
                }
            }
            finally
            {
                if (fitsStream == null)
                    bufferedFile.Dispose();
            }

            return fitsStream;
        }

        private ThreeAxisFITSCubeFrameStream(
            string fileName, Fits fitsFile, BufferedFile bufferedFile, BasicHDU imageHDU, 
            FITSTimeStampReader timeStampReader,
            int widthIndex, int heightIndex, int frameIndex,
            short minPixelValue, uint maxPixelValue, int bitPix, int negPixCorrection)
        {
            m_FileName = fileName;

            bool isMidPoint;
            double? exposureSecs;
            var startExposure = timeStampReader.ParseExposure(null, imageHDU.Header, out isMidPoint, out exposureSecs);

            if (startExposure.HasValue && exposureSecs.HasValue)
            {
                m_ExposureSeconds = exposureSecs.Value;
                m_FirstFrameMidTime = startExposure.Value;                
            }

            m_MinPixelValue = minPixelValue;
            m_MaxPixelValue = maxPixelValue;
            m_Bpp = bitPix;

            m_FitsFile = fitsFile;
            m_TimeStampReader = timeStampReader;

            m_BufferedFile = bufferedFile;
            m_ImageHDU = imageHDU;

            m_HeightIndex = heightIndex;
            m_WidthIndex = widthIndex;
            m_FrameIndex = frameIndex;

            m_NumFrames = m_ImageHDU.Axes[frameIndex];
            m_Height = m_ImageHDU.Axes[heightIndex];
            m_Width = m_ImageHDU.Axes[widthIndex];

            m_ArrayData = (Array)m_ImageHDU.Data.DataArray;
            m_BZero = FITSHelper2.GetBZero(m_ImageHDU);
            m_NegPixCorrection = negPixCorrection;

            m_Cards = new Dictionary<string, string>();
            var cursor = m_ImageHDU.Header.GetCursor();
            while (cursor.MoveNext())
            {
                HeaderCard card = m_ImageHDU.Header.FindCard((string)cursor.Key);
                if (card != null && !string.IsNullOrWhiteSpace(card.Key) && card.Key != "END")
                {
                    if (m_Cards.ContainsKey(card.Key))
                        m_Cards[card.Key] += "\r\n" + card.Value;
                    else
                        m_Cards.Add(card.Key, card.Value);
                }
            }


            HasUTCTimeStamps = startExposure.HasValue;

            VideoFileType = string.Format("FITS.{0}::Cube3D", bitPix);
        }

        public Pixelmap GetPixelmap(int index)
        {
            if (index < 0 || index >= m_NumFrames)
                return null;

            m_CurrentFrameIndex = index;

            var frameData = FITSHelper2.GetPixelsFrom3DCube(m_ArrayData, index, m_FrameIndex, m_HeightIndex, m_WidthIndex, m_NumFrames, m_Height, m_Width);
            if (frameData == null)
                return null;

            uint[] pixelsFlat = FITSHelper2.Load16BitImageData(frameData, m_Height, m_Width, m_BZero - m_NegPixCorrection, 0);

            DateTime timestamp = m_FirstFrameMidTime.AddSeconds(index*m_ExposureSeconds);
            return FitsStreamHelper.BuildFitsPixelmap(Width, Height, pixelsFlat, BitPix, true, m_ExposureSeconds, timestamp, m_ImageHDU, m_Cards);
        }

        public void Dispose()
        {
            m_BufferedFile.Dispose();
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
            get { return m_Bpp; }
        }

        public uint MinPixelValue
        {
            get { return m_MinPixelValue > 0 ? (uint)m_MinPixelValue : 0; }
        }

        public uint MaxPixelValue
        {
            get { return m_MaxPixelValue; }
        }

        public int FirstFrame
        {
            get { return 0; }
        }

        public int LastFrame
        {
            get { return m_NumFrames - 1; }
        }

        public int CountFrames
        {
            get { return m_NumFrames; }
        }

        public double FrameRate
        {
            get { return 1; }
        }

        public double MillisecondsPerFrame
        {
            get { return m_ExposureSeconds * 1000; }
        }

        public int RecommendedBufferSize
        {
            get { return 1; }
        }

        public string VideoFileType { get; private set; }

        public Pixelmap GetIntegratedFrame(int startFrameNo, int framesToIntegrate, bool isSlidingIntegration, bool isMedianAveraging)
        {
            throw new NotSupportedException();
        }

        public string Engine
        {
            get { return CUBE_3D_FITS_FILE_ENGINE; }
        }

        public string FileName
        {
            get { return m_FileName; }
        }

        public uint GetAav16NormVal()
        {
            return 0;
        }

        public bool SupportsSoftwareIntegration
        {
            get { return false; }
        }

        public bool SupportsFrameFileNames
        {
            get { return false; }
        }

        public string GetFrameFileName(int index)
        {
            throw new NotSupportedException();
        }

        public bool HasUTCTimeStamps { get; private set; }
    }
}
