using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using Tangra.Model.Config;
using Tangra.Model.Helpers;

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

		public Pixelmap(Pixelmap cloneFrom)
		{
			Width = cloneFrom.Width;
			Height = cloneFrom.Height;
			m_BitPix = cloneFrom.m_BitPix;
			m_Pixels = cloneFrom.m_Pixels;			
			m_DisplayBitmapPixels = cloneFrom.m_DisplayBitmapPixels;
			try
			{
				m_Bitmap = cloneFrom.DisplayBitmap != null ? (Bitmap) cloneFrom.DisplayBitmap.Clone() : null;
			}
			catch (Exception ex)
			{				
				Trace.WriteLine(ex);
				Trace.Assert(false);
			}
			
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
			else if (bitPix == 14)
				return 16383;
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

		public Bitmap CreateNewDisplayBitmap()
		{
			Bitmap bmp = null;

			if (m_DisplayBitmapPixels != null)
			{
				return ConstructBitmapFromBitmapPixels(m_DisplayBitmapPixels, Width, Height);
			}

			return bmp;
		}

		public static Bitmap ConstructBitmapFrom8BitPixelmap(Pixelmap image)
		{
			float background = 0;
			float range = 255;
			bool hasRangeChange = false;

			var bmp = new Bitmap(image.Width, image.Height, PixelFormat.Format24bppRgb);

			// GDI+ still lies to us - the return format is BGR, NOT RGB.
			BitmapData bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

			int stride = bmData.Stride;
			System.IntPtr Scan0 = bmData.Scan0;

			unsafe
			{
				byte* p = (byte*)(void*)Scan0;

				int nOffset = stride - bmp.Width * 3;

				for (int y = 0; y < bmp.Height; ++y)
				{
					for (int x = 0; x < bmp.Width; ++x)
					{
						byte color;

						if (hasRangeChange)
						{
							float displayValue = (image[x, y] - background) * 255.0f / range;
							color = (byte)Math.Max(0, Math.Min(255, Math.Round(displayValue)));
						}
						else
							color = (byte)(image[x, y] & 0xFF);

						p[0] = color;
						p[1] = color;
						p[2] = color;

						p += 3;
					}
					p += nOffset;
				}
			}

			bmp.UnlockBits(bmData);

			return bmp;
		}

		public static Bitmap ConstructBitmapFrom32bppArgbBitmapPixels(byte[] pixels, int width, int height)
		{
			var bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);

			// GDI+ still lies to us - the return format is BGR, NOT RGB.
			BitmapData bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

			int stride = bmData.Stride;
			System.IntPtr Scan0 = bmData.Scan0;

			unsafe
			{
				byte* p = (byte*)(void*)Scan0;

				int nOffset = stride - bmp.Width * 3;

				for (int y = 0; y < bmp.Height; ++y)
				{
					for (int x = 0; x < bmp.Width; ++x)
					{
						byte color;
						int index = 4 * x + y * 4 * width;
						color = (byte)(pixels[index]);

						p[0] = color;
						p[1] = color;
						p[2] = color;

						p += 3;
					}
					p += nOffset;
				}
			}

			bmp.UnlockBits(bmData);

			return bmp;			
		}

		public static Bitmap ConstructBitmapFromBitmapPixels(byte[] pixels, int width, int height)
		{
			var bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);

			// GDI+ still lies to us - the return format is BGR, NOT RGB.
			BitmapData bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

			int stride = bmData.Stride;
			System.IntPtr Scan0 = bmData.Scan0;

			unsafe
			{
				byte* p = (byte*)(void*)Scan0;

				int nOffset = stride - bmp.Width * 3;

				for (int y = 0; y < bmp.Height; ++y)
				{
					for (int x = 0; x < bmp.Width; ++x)
					{
						byte color;
						int index = x + y * width;
						color = (byte)(pixels[index]);

						p[0] = color;
						p[1] = color;
						p[2] = color;

						p += 3;
					}
					p += nOffset;
				}
			}

			bmp.UnlockBits(bmData);

			return bmp;
		}

		private static Pixelmap ConstructForLCFileAveragedFrame(uint[] data, int width, int height, int bitpixCamera, byte[] bytes)
		{
			Bitmap bmp = ConstructBitmapFromBitmapPixels(bytes, width, height);
			Pixelmap rv = new Pixelmap(width, height, bitpixCamera, data, bmp, bytes);
			return rv;
		}


		public static Pixelmap ConstructForLCFileAveragedFrame(byte[] bytes, int width, int height, int bitpixCamera)
		{
			return ConstructForLCFileAveragedFrame(new uint[bytes.Length], width, height, bitpixCamera, bytes);
		}

		public static Pixelmap ConstructForLCFile32bppArgbAveragedFrame(byte[] bytes, int width, int height, int bitpixCamera)
		{
			Bitmap bmp = ConstructBitmapFrom32bppArgbBitmapPixels(bytes, width, height);
			Pixelmap rv = new Pixelmap(width, height, bitpixCamera, new uint[bytes.Length], bmp, bytes);
			return rv;
		}

		public static byte GetColourChannelValue(TangraConfig.ColourChannel channel, byte red, byte green, byte blue)
		{
			if (channel == TangraConfig.ColourChannel.GrayScale)
				return (byte)(.299 * red + .587 * green + .114 * blue);
			else if (channel == TangraConfig.ColourChannel.Red)
				return red;
			else if (channel == TangraConfig.ColourChannel.Green)
				return green;
			else if (channel == TangraConfig.ColourChannel.Blue)
				return blue;

			throw new ArgumentOutOfRangeException();
		}

		public static Pixelmap ConstructFromBitmap(Bitmap bmp, TangraConfig.ColourChannel channel)
		{
			uint[] pixels = new uint[bmp.Width * bmp.Height];
			byte[] displayBitmapPixels = new byte[bmp.Width * bmp.Height];

			// GDI+ still lies to us - the return format is BGR, NOT RGB.
			BitmapData bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

			int stride = bmData.Stride;
			System.IntPtr Scan0 = bmData.Scan0;

			unsafe
			{
				byte* p = (byte*)(void*)Scan0;

				int nOffset = stride - bmp.Width * 3;

				for (int y = 0; y < bmp.Height; ++y)
				{
					for (int x = 0; x < bmp.Width; ++x)
					{
						byte blue = p[0];
						byte green = p[1];
						byte red = p[2];

						byte val = GetColourChannelValue(channel, red, green, blue);

						pixels[x + y * bmp.Width] = val;
						displayBitmapPixels[x + y * bmp.Width] = val;

						p += 3;
					}
					p += nOffset;
				}
			}

			bmp.UnlockBits(bmData);

			return new Pixelmap(bmp.Width, bmp.Height, 8, pixels, bmp, displayBitmapPixels);
		}

		public static Pixelmap ConstructFromBitmap(Bitmap bmp, byte[] bitmapData, TangraConfig.ColourChannel channel)
		{
			uint[] pixels = new uint[bmp.Width * bmp.Height];

			byte[] displayBitmapPixels = new byte[bmp.Width * bmp.Height];

			// NOTE: Those 2 for loops are very slow. Need performance improvements
			for (int y = 0; y < bmp.Height; ++y)
			{
				for (int x = 0; x < bmp.Width; ++x)
				{
					byte val = bitmapData[(x + (bmp.Height - 1 - y) * bmp.Width) * 3];

					pixels[x + y * bmp.Width] = val;
					displayBitmapPixels[x + y * bmp.Width] = val;
				}
			}

			return new Pixelmap(bmp.Width, bmp.Height, 8, pixels, bmp, displayBitmapPixels);
		}		

		public void CopyPixelsFrom(uint[,] pixels, int bpp)
		{
			if (pixels.GetLength(0) != Width || pixels.GetLength(1) != Height)
				throw new ArgumentException("Incompatible source image");

			uint maxImagePixelValue = bpp.GetMaxValueForBitPix();

			for (int y = 0; y < Height; y++)
			{
				for (int x = 0; x < Width; x++)
				{
					if (bpp == BitPixCamera)
						this[x, y] = (uint)pixels[x, y];
					else
						this[x, y] = (uint)(MaxPixelValue * 1.0 * pixels[x, y] / maxImagePixelValue);
				}
			}
		}

		public uint[,] GetPixelsCopy()
		{
			uint[,] pixels = new uint[Width, Height];

			for (int y = 0; y < Height; y++)
			{
				for (int x = 0; x < Width; x++)
				{
					pixels[x, y] = this[x, y];
				}
			}

			return pixels;
		}

	}
}
