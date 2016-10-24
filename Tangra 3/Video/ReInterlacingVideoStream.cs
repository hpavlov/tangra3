using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Tangra.Model.Context;
using Tangra.Model.Image;
using Tangra.Model.Video;
using Tangra.PInvoke;

namespace Tangra.Video
{
    public enum ReInterlaceMode
    {
        None,
        SwapFields,
        ShiftOneField,
        SwapAndShiftOneField
    }

    public class ReInterlacingVideoStream : IFrameStream, IDisposable
    {
        public static IFrameStream Create(IFrameStream baseStream, ReInterlaceMode mode)
        {
            return new ReInterlacingVideoStream(baseStream, mode);
        }

        private ReInterlacingVideoStream(IFrameStream baseStream, ReInterlaceMode mode)
        {
            m_BaseStream = baseStream;
            ChangeReInterlaceMode(mode);
        }

        public bool ChangeReInterlaceMode(ReInterlaceMode mode)
        {
            if (m_Mode != mode)
            {
                m_Mode = mode;
                if (mode == ReInterlaceMode.SwapFields)
                    TangraContext.Current.ReInterlacingMode = "FieldSwap";
                else if (mode == ReInterlaceMode.ShiftOneField)
                {
                    TangraContext.Current.ReInterlacingMode = "FieldShift";
                    m_ShiftMode = 0;
                }
                else if (mode == ReInterlaceMode.SwapAndShiftOneField)
                {
                    TangraContext.Current.ReInterlacingMode = "FieldSwapAndShift";
                    m_ShiftMode = 1;
                }
                else
                {
                    TangraContext.Current.ReInterlacingMode = null;
                    m_ShiftMode = 0;
                }

                return true;
            }

            return false;
        }


        private IFrameStream m_BaseStream;
        private ReInterlaceMode m_Mode;
        private int m_ShiftMode = 0;

        public int Width
        {
            get { return m_BaseStream.Width; }
        }

        public int Height
        {
            get { return m_BaseStream.Height; }
        }

        public int BitPix
        {
            get { return m_BaseStream.BitPix; }
        }

        public int FirstFrame
        {
            get { return m_BaseStream.BitPix; }
        }

        public int LastFrame
        {
            get
            {
                if (m_Mode == ReInterlaceMode.ShiftOneField || m_Mode == ReInterlaceMode.SwapAndShiftOneField)
                    return m_BaseStream.LastFrame - 1;
                else
                    return m_BaseStream.LastFrame;
            }
        }

        public int CountFrames
        {
            get
            {
                if (m_Mode == ReInterlaceMode.ShiftOneField || m_Mode == ReInterlaceMode.SwapAndShiftOneField)
                    return m_BaseStream.CountFrames - 1;
                else
                    return m_BaseStream.CountFrames;
            }
        }

        public double FrameRate
        {
            get { return m_BaseStream.FrameRate; }
        }

        public double MillisecondsPerFrame
        {
            get { return m_BaseStream.MillisecondsPerFrame; }
        }

        private int m_LastPrevFrameId = -1;
        private uint[] m_LastPrevFramePixels = null;
        private uint[] m_LastPrevFrameOriginalPixels = null;
        private byte[] m_LastPrevFrameBitmapBytes = null;

        public Pixelmap GetPixelmap(int index)
        {
            if (index < FirstFrame) index = FirstFrame;
            if (index > LastFrame) index = LastFrame;

            uint[] pixels;
            uint[] originalPixels;
            Bitmap videoFrame;
            byte[] bitmapBytes;

            if (m_Mode == ReInterlaceMode.SwapFields)
            {
                TangraVideo.GetFrame(index, out pixels, out originalPixels, out videoFrame, out bitmapBytes);
                
                byte[] bitmapPixels = new byte[Width * Height * 3 + 40 + 14 + 1];

                TangraCore.SwapVideoFields(pixels, originalPixels, Width, Height, bitmapPixels, bitmapBytes);

                videoFrame = Pixelmap.ConstructBitmapFromBitmapPixels(bitmapBytes, Width, Height);

                var rv = new Pixelmap(Width, Height, 8, pixels, videoFrame, bitmapBytes);
                rv.UnprocessedPixels = originalPixels;
                return rv;
            }
            else if (m_Mode == ReInterlaceMode.ShiftOneField || m_Mode == ReInterlaceMode.SwapAndShiftOneField)
            {
                uint[] pixels2;
                uint[] originalPixels2;
                Bitmap videoFrame2;
                byte[] bitmapBytes2;

                if (m_LastPrevFrameId == index)
                {
                    pixels = m_LastPrevFramePixels;
                    originalPixels = m_LastPrevFrameOriginalPixels;
                    bitmapBytes = m_LastPrevFrameBitmapBytes;
                }
                else
                    TangraVideo.GetFrame(index, out pixels, out originalPixels, out videoFrame, out bitmapBytes);

                TangraVideo.GetFrame(index + 1, out pixels2, out originalPixels2, out videoFrame2, out bitmapBytes2);

                m_LastPrevFrameId = index + 1;
                m_LastPrevFramePixels = pixels2;
                m_LastPrevFrameOriginalPixels = originalPixels2;
                m_LastPrevFrameBitmapBytes = bitmapBytes2;

                byte[] bitmapPixels = new byte[Width * Height * 3 + 40 + 14 + 1];

                TangraCore.ShiftVideoFields(pixels, originalPixels, pixels2, originalPixels2, Width, Height, m_ShiftMode, bitmapPixels, bitmapBytes);

                videoFrame = Pixelmap.ConstructBitmapFromBitmapPixels(bitmapBytes, Width, Height);

                var rv = new Pixelmap(Width, Height, 8, pixels, videoFrame, bitmapBytes);
                rv.UnprocessedPixels = originalPixels;
                return rv;
            }
            else if (m_Mode == ReInterlaceMode.None)
            {
                return m_BaseStream.GetPixelmap(index);                 
            }
            else
                throw new NotSupportedException();
        }

        public string GetFrameFileName(int index)
        {
            throw new NotImplementedException();
        }

        public int RecommendedBufferSize
        {
            get { return m_BaseStream.RecommendedBufferSize; }
        }

        public string VideoFileType
        {
            get { return m_BaseStream.VideoFileType; }
        }

        public Pixelmap GetIntegratedFrame(int startFrameNo, int framesToIntegrate, bool isSlidingIntegration, bool isMedianAveraging)
        {
            if (m_Mode == ReInterlaceMode.None)
                return m_BaseStream.GetIntegratedFrame(startFrameNo, framesToIntegrate, isSlidingIntegration, isMedianAveraging);
            else
                throw new NotSupportedException();
        }

        public string Engine
        {
            get { return m_BaseStream.Engine; }
        }

        public string FileName
        {
            get { return m_BaseStream.FileName; }
        }

        public uint GetAav16NormVal()
        {
            return m_BaseStream.GetAav16NormVal();
        }

        public bool SupportsSoftwareIntegration
        {
            get { return m_Mode == ReInterlaceMode.None; }
        }

        public bool SupportsFrameFileNames
        {
            get { return false; }
        }

        public void Dispose()
        {
            var disposable = m_BaseStream as IDisposable;

            if (disposable != null)
                disposable.Dispose();
        }
    }
}
