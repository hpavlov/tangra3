using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace Tangra.Model.Image
{
	public class Pixelmap : IDisposable
	{
        private uint m_MaxPixelValue;
        private int m_BitPix = 8;

		private uint[] m_Pixels;
		private Bitmap m_Bitmap;
        private byte[] m_DisplayBitmapPixels;

		public FrameStateData FrameState;

        public Pixelmap(int width, int height, int bitPix, uint[] pixels, Bitmap bmp, byte[] displayBitmapBytes)
		{
		    Width = width;
			Height = height;
			m_BitPix = bitPix;
			m_Pixels = pixels;
			m_Bitmap = bmp;
            m_DisplayBitmapPixels = displayBitmapBytes;
		}

        public int Width { get; set; }
        public int Height { get; set; }

        public int BitPixCamera
        {
            get { return m_BitPix; }
            set
            {
                m_BitPix = value;
                m_MaxPixelValue = GetMaxValueForBitPix(m_BitPix);
            }
        }

        public static uint GetMaxValueForBitPix(int bitPix)
        {
            if (bitPix == 8)
                return byte.MaxValue;
            else if (bitPix == 12)
                return 4095;
            else if (bitPix == 16)
                return ushort.MaxValue;
            else
                return uint.MaxValue;
        }

        public uint MaxPixelValue { get { return m_MaxPixelValue; } }

        public uint this[int x, int y]
        {
            get { return m_Pixels[x + (y * Width)]; }
            set { m_Pixels[x + (y * Width)] = value; }
        }

		public Bitmap DisplayBitmap
		{
			get { return m_Bitmap; }
		}

        public byte[] DisplayBitmapPixels
        {
            get
            {
                return m_DisplayBitmapPixels;
            }
        }

        public uint[] Pixels
        {
            get
            {
                return m_Pixels;
            }
        }

		public void Dispose()
		{
			if (m_Bitmap != null)
			{
				m_Bitmap.Dispose();
				m_Bitmap = null;
			}
		}

        public Bitmap CreateDisplayBitmapDoNotDispose()
        {
            if (m_Bitmap != null && m_Bitmap.PixelFormat == PixelFormat.Format24bppRgb)
                return m_Bitmap;

            Trace.Assert(false);
            throw new InvalidOperationException("m_Bitmap must be set when creating the Pixelmap");
        }
	}
}
