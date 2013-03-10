using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Tangra.Model.Image
{
    public class ConvMatrix
    {
        public float TopLeft = 0, TopMid = 0, TopRight = 0;
        public float MidLeft = 0, Pixel = 1, MidRight = 0;
        public float BottomLeft = 0, BottomMid = 0, BottomRight = 0;
        public int Factor = 1;
        public int Offset = 0;

        public void SetAll(int nVal)
        {
            TopLeft = TopMid = TopRight = MidLeft = Pixel = MidRight = BottomLeft = BottomMid = BottomRight = nVal;
        }

        public ConvMatrix()
        { }

        public ConvMatrix(float[,] matrix)
        {
            TopLeft = matrix[0, 0];
            TopMid = matrix[0, 1];
            TopRight = matrix[0, 2];

            MidLeft = matrix[1, 0];
            Pixel = matrix[1, 1];
            MidRight = matrix[1, 2];

            BottomLeft = matrix[2, 0];
            BottomMid = matrix[2, 1];
            BottomRight = matrix[2, 2];
        }
    }

    public class ConvMatrix5x5
    {
        public readonly float[,] Values;

        public ConvMatrix5x5(float[,] matrix)
        {
            Values = matrix;
        }
    }

    public static class Convolution
    {
        // http://www.student.kuleuven.be/~m0216922/CG/filtering.html

        public static bool Conv3x3(Pixelmap image, ConvMatrix m)
        {
            // Avoid divide by zero errors
            if (0 == m.Factor)
                return false;

            var result = new Pixelmap(image.Width, image.Height, image.BitPixCamera, image.Pixels, image.DisplayBitmap, image.DisplayBitmapPixels);

            for (int y = 0; y < image.Height - 2; ++y)
            {
                for (int x = 0; x < image.Width - 2; ++x)
                {
                    ulong nPixel = (ulong)Math.Round((((image[x, y] * m.TopLeft) +
                                                (image[x + 1, y] * m.TopMid) +
                                                (image[x + 2, y] * m.TopRight) +
                                                (image[x, y + 1] * m.MidLeft) +
                                                (image[x + 1, y + 1] * m.Pixel) +
                                                (image[x + 2, y + 1] * m.MidRight) +
                                                (image[x, y + 2] * m.BottomLeft) +
                                                (image[x, y + 2] * m.BottomMid) +
                                                (image[x, y + 2] * m.BottomRight))
                                                / m.Factor) + m.Offset);

                    if (nPixel < 0) nPixel = 0;
                    if (nPixel > image.MaxPixelValue) nPixel = image.MaxPixelValue;
                    result[x + 1, y] = (uint)nPixel;
                }
            }

            //// GDI+ still lies to us - the return format is BGR, NOT RGB. 
            //using (Bitmap bSrc = (Bitmap)b.Clone())
            //{
            //    BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
            //                                   ImageLockMode.ReadWrite,
            //                                   PixelFormat.Format24bppRgb);
            //    BitmapData bmSrc = bSrc.LockBits(new Rectangle(0, 0, bSrc.Width, bSrc.Height),
            //                                     ImageLockMode.ReadWrite,
            //                                     PixelFormat.Format24bppRgb);
            //    int stride = bmData.Stride;
            //    int stride2 = stride*2;

            //    System.IntPtr Scan0 = bmData.Scan0;
            //    System.IntPtr SrcScan0 = bmSrc.Scan0;

            //    unsafe
            //    {
            //        byte* p = (byte*) (void*) Scan0;
            //        byte* pSrc = (byte*) (void*) SrcScan0;
            //        int nOffset = stride - b.Width*3;
            //        int nWidth = b.Width - 2;
            //        int nHeight = b.Height - 2;

            //        int nPixel;

            //        for (int y = 0; y < nHeight; ++y)
            //        {
            //            for (int x = 0; x < nWidth; ++x)
            //            {
            //                nPixel = (int) Math.Round((((pSrc[2]*m.TopLeft) +
            //                                            (pSrc[5]*m.TopMid) +
            //                                            (pSrc[8]*m.TopRight) +
            //                                            (pSrc[2 + stride]*m.MidLeft) +
            //                                            (pSrc[5 + stride]*m.Pixel) +
            //                                            (pSrc[8 + stride]*m.MidRight) +
            //                                            (pSrc[2 + stride2]*m.BottomLeft) +
            //                                            (pSrc[5 + stride2]*m.BottomMid) +
            //                                            (pSrc[8 + stride2]*m.BottomRight))
            //                                           /m.Factor) + m.Offset);

            //                if (nPixel < 0) nPixel = 0;
            //                if (nPixel > 255) nPixel = 255;
            //                p[5 + stride] = (byte) nPixel;

            //                nPixel = (int) Math.Round(((((pSrc[1]*m.TopLeft) +
            //                                             (pSrc[4]*m.TopMid) +
            //                                             (pSrc[7]*m.TopRight) +
            //                                             (pSrc[1 + stride]*m.MidLeft) +
            //                                             (pSrc[4 + stride]*m.Pixel) +
            //                                             (pSrc[7 + stride]*m.MidRight) +
            //                                             (pSrc[1 + stride2]*m.BottomLeft) +
            //                                             (pSrc[4 + stride2]*m.BottomMid) +
            //                                             (pSrc[7 + stride2]*m.BottomRight))
            //                                            /m.Factor) + m.Offset));

            //                if (nPixel < 0) nPixel = 0;
            //                if (nPixel > 255) nPixel = 255;
            //                p[4 + stride] = (byte) nPixel;

            //                nPixel = (int) Math.Round(((((pSrc[0]*m.TopLeft) +
            //                                             (pSrc[3]*m.TopMid) +
            //                                             (pSrc[6]*m.TopRight) +
            //                                             (pSrc[0 + stride]*m.MidLeft) +
            //                                             (pSrc[3 + stride]*m.Pixel) +
            //                                             (pSrc[6 + stride]*m.MidRight) +
            //                                             (pSrc[0 + stride2]*m.BottomLeft) +
            //                                             (pSrc[3 + stride2]*m.BottomMid) +
            //                                             (pSrc[6 + stride2]*m.BottomRight))
            //                                            /m.Factor) + m.Offset));

            //                if (nPixel < 0) nPixel = 0;
            //                if (nPixel > 255) nPixel = 255;
            //                p[3 + stride] = (byte) nPixel;

            //                p += 3;
            //                pSrc += 3;
            //            }

            //            p += nOffset;
            //            pSrc += nOffset;
            //        }
            //    }

            //    b.UnlockBits(bmData);
            //    bSrc.UnlockBits(bmSrc);
            //}

            return true;
        }

        public static uint[,] Conv3x3(uint[,] data, ConvMatrix m)
        {
            // Avoid divide by zero errors
            if (0 == m.Factor)
                return data;

            uint[,] result = new uint[data.GetLength(0), data.GetLength(1)];

            int nWidth = data.GetLength(0) - 2;
            int nHeight = data.GetLength(1) - 2;

            int nPixel;

            for (int y = 0; y < nHeight; ++y)
            {
                for (int x = 0; x < nWidth; ++x)
                {
                    nPixel = (int)Math.Round((((data[x, y] * m.TopLeft) +
                                                (data[x + 1, y] * m.TopMid) +
                                                (data[x + 2, y] * m.TopRight) +
                                                (data[x, y + 1] * m.MidLeft) +
                                                (data[x + 1, y + 1] * m.Pixel) +
                                                (data[x + 2, y + 1] * m.MidRight) +
                                                (data[x, y + 2] * m.BottomLeft) +
                                                (data[x + 1, y + 2] * m.BottomMid) +
                                                (data[x + 2, y + 2] * m.BottomRight))
                                               / m.Factor) + m.Offset);

                    if (nPixel < 0) nPixel = 0;
                    if (nPixel > 255) nPixel = 255;
                    result[x + 1, y + 1] = (uint)nPixel;
                }
            }

            return result;
        }


        public static bool Conv5x5(Pixelmap image, ConvMatrix5x5 m)
        {
            throw new NotImplementedException();
        }
    }

	public static class BitmapFilter
	{
		// http://www.echoview.com/WebHelp/Reference/Algorithms/Operators/Convolution_algorithms.htm
		private static ConvMatrix LOW_PASS_FILTER_MATRIX =
			new ConvMatrix(new float[,]
            {
                { 1 / 16.0f, 1 / 8.0f, 1 / 16.0f }, 
                { 1 / 8.0f, 1 / 4.0f, 1 / 8.0f }, 
                { 1 / 16.0f, 1 / 8.0f, 1 / 16.0f }
            });

		private static ConvMatrix MEAN_FILTER_MATRIX =
			new ConvMatrix(new float[,]
            {
                { 1, 1, 1 }, 
                { 1, 1, 1 }, 
                { 1, 1, 1 }
            });

		private static ConvMatrix MEAN_LESS_FILTER_MATRIX =
			new ConvMatrix(new float[,]
            {
                { 1 / 9.0f, 1 / 9.0f, 1 / 9.0f }, 
                { 1 / 9.0f, 1 / 9.0f, 1 / 9.0f }, 
                { 1 / 9.0f, 1 / 9.0f, 1 / 9.0f }
            });


		private static uint[,] SetArrayEdgesToZero(uint[,] data)
		{
			int nWidth = data.GetLength(0);
			int nHeight = data.GetLength(1);

			for (int i = 0; i < nWidth; i++)
			{
				data[i, 0] = 0;
				data[i, nHeight - 1] = 0;
			}

			for (int j = 0; j < nHeight; j++)
			{
				data[0, j] = 0;
				data[nWidth - 1, j] = 0;
			}

			return data;
		}

		public static uint[,] CutArrayEdges(uint[,] data, int linesToCut)
		{
			int nWidth = data.GetLength(0);
			int nHeight = data.GetLength(1);

			uint[,] cutData = new uint[nWidth - 2 * linesToCut, nHeight - 2 * linesToCut];

			for (int i = linesToCut; i < nWidth - linesToCut; i++)
				for (int j = linesToCut; j < nHeight - linesToCut; j++)
				{
					cutData[i - linesToCut, j - linesToCut] = data[i, j];
				}

			return cutData;
		}

		public static bool LowPassFilter(Pixelmap b)
		{
			return Convolution.Conv3x3(b, LOW_PASS_FILTER_MATRIX);
		}

		public static uint[,] LowPassFilter(uint[,] b, bool cutEdges)
		{
			uint[,] data = Convolution.Conv3x3(b, LOW_PASS_FILTER_MATRIX);
			if (cutEdges)
				return CutArrayEdges(data, 1);
			else
			{
				data = SetArrayEdgesToZero(data);
				return data;
			}
		}

		public static uint[,] LowPassFilter(uint[,] b)
		{
			return Convolution.Conv3x3(b, LOW_PASS_FILTER_MATRIX);
		}

		public static uint[,] LowPassDifferenceFilter(uint[,] b, bool cutEdges)
		{
			uint[,] data = LowPassDifferenceFilter(b, cutEdges);
			if (cutEdges)
				return CutArrayEdges(data, 1);
			else
				return data;
		}
		public static bool LowPassDifference(Pixelmap image)
		{
			uint[,] pixels = GetPixelArray(image);

			pixels = LowPassDifferenceFilter(pixels, false);

			image.CopyPixelsFrom(pixels, image.BitPixCamera);

			return true;
		}

		public static bool Brightness(Pixelmap image, int nBrightness)
		{
			if (nBrightness < -image.MaxPixelValue || nBrightness > image.MaxPixelValue)
				return false;

			for (int y = 0; y < image.Height; ++y)
			{
				for (int x = 0; x < image.Width; ++x)
				{
					long nVal = image[x, y] + nBrightness;

					if (nVal < 0) nVal = 0;
					if (nVal > image.MaxPixelValue) nVal = image.MaxPixelValue;

					image[x, y] = (uint)nVal;
				}
			}

			return true;
		}

		public static bool Contrast(Pixelmap image, sbyte nContrast)
		{
			if (nContrast < -100) return false;
			if (nContrast > 100) return false;

			double pixel = 0, contrast = (100.0 + nContrast) / 100.0;

			contrast *= contrast;

			for (int y = 0; y < image.Height; ++y)
			{
				for (int x = 0; x < image.Width; ++x)
				{
					uint val = image[x, y];

					pixel = val * 1.0 / image.MaxPixelValue;
					pixel -= 0.5;
					pixel *= contrast;
					pixel += 0.5;
					pixel *= image.MaxPixelValue;
					if (pixel < 0) pixel = 0;
					if (pixel > image.MaxPixelValue) pixel = image.MaxPixelValue;
					image[x, y] = (uint)pixel;
				}
			}

			return true;
		}

		private static uint[,] GetPixelArray(Pixelmap image)
		{
			var rv = new uint[image.Width, image.Height];

			for (int y = 0; y < image.Height; ++y)
			{
				for (int x = 0; x < image.Width; ++x)
				{
					rv[x, y] = image[x, y];
				}
			}

			return rv;
		}

		public static Bitmap ToVideoFields(Bitmap frameBitmap)
		{
			int width = frameBitmap.Width;
			int height = frameBitmap.Height;

			Bitmap fieldBitmap = new Bitmap(width, height, frameBitmap.PixelFormat);

			BitmapData bmData = frameBitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, frameBitmap.PixelFormat);
			BitmapData bmResult = fieldBitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, frameBitmap.PixelFormat);

			int stride = bmData.Stride;

			unsafe
			{
				int destRow = 0;

				for (int y = 0; y < height; y += 2)
				{
					int srcOffset = y * stride;
					int destOffset = destRow * stride;

					byte[] row = new byte[stride];
					Marshal.Copy((IntPtr)((long)(void*)bmData.Scan0 + srcOffset), row, 0, stride);
					Marshal.Copy(row, 0, (IntPtr)((long)(void*)bmResult.Scan0 + destOffset), stride);
					destRow++;
				}

				for (int y = 1; y < height; y += 2)
				{
					int srcOffset = y * stride;
					int destOffset = destRow * stride;


					byte[] row = new byte[stride];
					Marshal.Copy(new IntPtr(bmData.Scan0.ToInt32() + srcOffset), row, 0, stride);
					Marshal.Copy(row, 0, new IntPtr(bmResult.Scan0.ToInt32() + destOffset), stride);
					destRow++;
				}
			}

			frameBitmap.UnlockBits(bmData);
			fieldBitmap.UnlockBits(bmResult);

			return fieldBitmap;
		}

		private static float LO_GAMMA = 0.45f;
		private static float HI_GAMMA = 0.25f;

		private static byte[] LO_GAMMA_TABLE = new byte[256];
		private static byte[] HI_GAMMA_TABLE= new byte[256];

		static BitmapFilter()
		{
			double lowGammaMax = 255.0 / Math.Pow(255, LO_GAMMA);
			double highGammaMax = 255.0 / Math.Pow(255, HI_GAMMA);

			for (int i = 0; i <= 255; i++)
			{
				double lowGammaValue = lowGammaMax * Math.Pow(i, LO_GAMMA);
				double highGammaValue = highGammaMax * Math.Pow(i, HI_GAMMA);

				LO_GAMMA_TABLE[i] = (byte)Math.Max(0, Math.Min(255, Math.Round(lowGammaValue)));
				HI_GAMMA_TABLE[i] = (byte)Math.Max(0, Math.Min(255, Math.Round(highGammaValue)));
			}
		}

		public static void ApplyGamma(Bitmap bitmap, bool hiGamma, bool invertAfterGamma)
		{
			int width = bitmap.Width;
			int height = bitmap.Height;

            BitmapData bmData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

			int stride = bmData.Stride;

			unsafe
			{
				byte* p = (byte*)(void*)bmData.Scan0;

				int nOffset = stride - bmData.Width * 3;

				for (int y = 0; y < bmData.Height; ++y)
				{
					for (int x = 0; x < bmData.Width; ++x)
					{
						p[0] = hiGamma ? HI_GAMMA_TABLE[p[0]] : LO_GAMMA_TABLE[p[0]];
						p[1] = hiGamma ? HI_GAMMA_TABLE[p[1]] : LO_GAMMA_TABLE[p[1]];
						p[2] = hiGamma ? HI_GAMMA_TABLE[p[2]] : LO_GAMMA_TABLE[p[2]];

						if (invertAfterGamma)
						{
							p[0] = (byte)(255 - p[0]);
							p[1] = (byte)(255 - p[1]);
							p[2] = (byte)(255 - p[2]);
						}
						p += 3;
					}
					p += nOffset;
				}
			}

			bitmap.UnlockBits(bmData);
			
		}

		public static void Invert(Bitmap bitmap)
		{
			int width = bitmap.Width;
			int height = bitmap.Height;

			BitmapData bmData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

			int stride = bmData.Stride;

			unsafe
			{
				byte* p = (byte*)(void*)bmData.Scan0;

                int nOffset = stride - bmData.Width * 3;

				for (int y = 0; y < bmData.Height; ++y)
				{
					for (int x = 0; x < bmData.Width; ++x)
					{
						p[0] = (byte)(255 - p[0]);
						p[1] = (byte)(255 - p[1]);
						p[2] = (byte)(255 - p[2]);

                        p += 3;
					}
					p += nOffset;
				}
			}

			bitmap.UnlockBits(bmData);

		}
	}
}
