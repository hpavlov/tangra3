using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Tangra.Model.Config;
using Tangra.Model.Image;

namespace Tangra.Model.Astro
{
	public interface IAstroImage
	{
		uint[,] GetPixelsCopy();
		uint[,] GetPixelsArea(int xCenter, int yCenter, int matrixSize);
		IImagePixel GetCentroid(int x, int y, int radius, uint noiseLevel);
		uint MedianNoise { get; }
		int Width { get; }
		int Height { get; }
		uint[] GetPixelmapPixels();
	}

	public class AstroImage : IAstroImage
    {
        protected Pixelmap m_Pixelmap;

        protected Rectangle m_Rect;

        protected int m_Width = 0;
        protected int m_Height = 0;
        protected int m_BitPix = 32;
        protected int m_BitPixCamera = 16;

        public AstroImage(Pixelmap pixelmap)
            : this(pixelmap, new Rectangle(0, 0, pixelmap.Width, pixelmap.Height))
        { }

        public AstroImage(Pixelmap pixelmap, Rectangle processArea)
        {
            m_Pixelmap = pixelmap;
            m_Rect = processArea;

            m_Width = pixelmap.Width;
            m_Height = pixelmap.Height;
            m_BitPix = pixelmap.BitPixCamera;

            if (m_Rect.Width > m_Width) m_Rect.Width = m_Width;
            if (m_Rect.Height > m_Height) m_Rect.Height = m_Height;
        }

        public int Width
        {
            get { return m_Width; }
        }

        public int Height
        {
            get { return m_Height; }
        }

        public Pixelmap Pixelmap
        {
            get { return m_Pixelmap; }
        }

		public uint[,] GetPixelsCopy()
		{
			return m_Pixelmap.GetPixelsCopy();
		}

		public uint[] GetPixelmapPixels()
		{
			return m_Pixelmap.Pixels;
		}

        private uint m_MedianNoise = UInt32.MaxValue;
        public uint MedianNoise
        {
            get
            {
                if (m_MedianNoise == UInt32.MaxValue)
                    m_MedianNoise = GetMedian(m_Pixelmap);

                return m_MedianNoise;
            }
        }

        public static uint GetMedian(Pixelmap image)
        {
            List<uint> allPixels = new List<uint>();

            for (int y = 0; y < image.Height; ++y)
            {
                for (int x = 0; x < image.Width; ++x)
                {
                    allPixels.Add(image[x, y]);
                }
            }

            allPixels.Sort();

            return allPixels[allPixels.Count / 2];
        }

        public uint[,] GetMeasurableAreaPixels(ImagePixel center)
        {
            return GetMeasurableAreaPixels(center.X, center.Y);
        }

        public uint[,] GetMeasurableAreaPixels(int xCenter, int yCenter)
        {
            return GetMeasurableAreaPixels(xCenter, yCenter, 17);
        }

        public uint[,] GetMeasurableAreaPixels(int xCenter, int yCenter, int matrixSize)
        {
            uint[,] pixels = new uint[matrixSize, matrixSize];

            int height = m_Height;
            int width = m_Width;

            int x0 = xCenter;
            int y0 = yCenter;

            int halfWidth = matrixSize / 2;

            for (int x = x0 - halfWidth; x <= x0 + halfWidth; x++)
                for (int y = y0 - halfWidth; y <= y0 + halfWidth; y++)
                {
                    uint byteVal = 0;

                    if (x >= 0 && x < width && y >= 0 & y < height)
                    {
                        byteVal = m_Pixelmap[x, y];
                    }

                    pixels[x - x0 + halfWidth, y - y0 + halfWidth] = byteVal;
                }

            return pixels;
        }

		public uint[,] GetPixelsArea(int xCenter, int yCenter, int matrixSize)
		{
			return GetMeasurableAreaPixels(xCenter, yCenter, matrixSize);
		}

        public byte[,] GetMeasurableAreaDisplayBitmapPixels(ImagePixel center)
        {
            return GetMeasurableAreaDisplayBitmapPixels(center.X, center.Y);
        }

        public byte[,] GetMeasurableAreaDisplayBitmapPixels(int xCenter, int yCenter)
        {
            return GetMeasurableAreaDisplayBitmapPixels(xCenter, yCenter, 17);
        }

        public byte[,] GetMeasurableAreaDisplayBitmapPixels(int xCenter, int yCenter, int matrixSize)
        {
            byte[,] pixels = new byte[matrixSize, matrixSize];

            int height = m_Height;
            int width = m_Width;

            int x0 = xCenter;
            int y0 = yCenter;

            int halfWidth = matrixSize / 2;

            for (int x = x0 - halfWidth; x <= x0 + halfWidth; x++)
                for (int y = y0 - halfWidth; y <= y0 + halfWidth; y++)
                {
                    byte byteVal = 0;

                    if (x >= 0 && x < width && y >= 0 & y < height)
                    {
                        byteVal = m_Pixelmap.DisplayBitmapPixels[x + y * m_Width];
                    }

                    pixels[x - x0 + halfWidth, y - y0 + halfWidth] = byteVal;
                }

            return pixels;
        }

        private delegate void PixelAreaOperationCallback(int x, int y, uint z);
        private void PixelAreaOperation(int x, int y, int radius, PixelAreaOperationCallback callback)
        {
            for (int i = x - radius; i <= x + radius; i++)
                for (int j = y - radius; j <= y + radius; j++)
                {
                    if (i >= 0 && i < m_Pixelmap.Width && j >= 0 && j < m_Pixelmap.Height)
                    {
                        uint pixel = m_Pixelmap[i, j];
                        callback(i, j, pixel);
                    }
                }
        }

        public IImagePixel GetCentroid(int x, int y, int radius, uint noiseLevel)
        {
            return GetCentroid(x, y, radius, true, noiseLevel);
        }

        public ImagePixel GetCentroid(int x, int y, int radius, bool doPeakPixelFirst, uint noiseLevel)
        {
            uint minimum = m_Pixelmap.MaxPixelValue;
            uint maximum = 0;
            int xMax = x, yMax = y;
            PixelAreaOperation(x, y, radius,
                               delegate(int x1, int y1, uint z)
                               {
                                   if (minimum > z) minimum = z;
                                   if (maximum < z)
                                   {
                                       maximum = z;

                                       if (doPeakPixelFirst)
                                       {
                                           xMax = x1;
                                           yMax = y1;
                                       }
                                   }
                               });

            if (maximum < noiseLevel) return null;

            double xx = xMax;
            double yy = yMax;
            double deltax = 0;
            double deltay = 0;

            for (int itter = 0; itter < 10; itter++)
            {
                xx += deltax;
                yy += deltay;

                double sumMomentumX = 0;
                double sumMomentumY = 0;
                double sumIntensity = 0;

                PixelAreaOperation(xMax, yMax, radius,
                                   delegate(int x1, int y1, uint z)
                                   {
                                       uint diff = Math.Max(z - minimum, 0);
                                       sumMomentumX += diff * (x1 - xx);
                                       sumMomentumY += diff * (y1 - yy);
                                       sumIntensity += diff;
                                   });

                deltax = sumMomentumX / sumIntensity;
                deltay = sumMomentumY / sumIntensity;
            }

            if (double.IsNaN(xx) || double.IsNaN(yy))
                return null;

            ImagePixel retVal = new ImagePixel(xx, yy);

            retVal.SignalNoise = retVal.Brightness * 1.0 / noiseLevel;

            return retVal;
        }

        public float GetAverageFWHM()
        {
            return 3.6f;
        }

        public const int BytesPerDisplayBitmapPixel = 3;

        public Bitmap GetZoomImagePixels(int x0, int y0, Color saturationColor, TangraConfig.SaturationSettings saturationLevels)
        {
            int height = m_Pixelmap.Height;
            int width = m_Pixelmap.Width;

            if (x0 < 15) x0 = 15; if (y0 < 15) y0 = 15;
            if (x0 > width - 16) x0 = width - 16;

            if (y0 > height - 16) y0 = height - 16;
            Bitmap featureBitmap = new Bitmap(31 * 8, 31 * 8, PixelFormat.Format24bppRgb);

            uint saturationLevel = saturationLevels.GetSaturationForBpp(m_Pixelmap.BitPixCamera);

            BitmapData zoomedData = featureBitmap.LockBits(new Rectangle(0, 0, 31 * 8, 31 * 8), ImageLockMode.ReadWrite, featureBitmap.PixelFormat);
            try
            {
                int bytes = zoomedData.Stride * featureBitmap.Height;
                byte[] zoomedValues = new byte[bytes];

                byte saturatedR = saturationColor.R;
                byte saturatedG = saturationColor.G;
                byte saturatedB = saturationColor.B;

                int selIdx = 0;

                for (int yy = -15; yy < 16; yy++)
                    for (int xx = -15; xx < 16; xx++)
                    {
                        for (int i = 0; i < 8; i++)
                            for (int j = 0; j < 8; j++)
                            {
                                int x = x0 + xx;
                                int y = y0 + yy;

                                int zoomedX = 8 * (xx + 15) + i;
                                int zoomedY = 8 * (yy + 15) + j;


                                byte zoomedByte = 0;
                                uint pixelVal = 0;
                                if (x >= 0 && x < width && y >= 0 && y < height)
                                {
                                    zoomedByte = m_Pixelmap.DisplayBitmapPixels[y * width + x];
                                    pixelVal = m_Pixelmap[x, y];
                                }

                                int zoomedIdx = zoomedData.Stride * zoomedY + zoomedX * AstroImage.BytesPerDisplayBitmapPixel;

                                if (pixelVal > saturationLevel)
                                {
                                    // Saturation detected
                                    zoomedValues[zoomedIdx] = saturatedR;
                                    zoomedValues[zoomedIdx + 1] = saturatedG;
                                    zoomedValues[zoomedIdx + 2] = saturatedB;
                                }
                                else
                                {
                                    zoomedValues[zoomedIdx] = zoomedByte;
                                    zoomedValues[zoomedIdx + 1] = zoomedByte;
                                    zoomedValues[zoomedIdx + 2] = zoomedByte;
                                }
                            }

                        selIdx++;
                    }

                Marshal.Copy(zoomedValues, 0, zoomedData.Scan0, bytes);
            }
            finally
            {
                featureBitmap.UnlockBits(zoomedData);
            }

            return featureBitmap;
        }

        public static byte GetGrayScaleReading(byte bt0_b, byte bt1_g, byte bt2_r)
        {
            // Assuming BGR (bt0 = B; bt1 = G; bt2 = R)
            return (byte)(.299 * bt2_r + .587 * bt1_g + .114 * bt0_b);
        }

		public byte GetDisplayPixel(int x, int y)
		{
			if (x >= 0 && x < m_Pixelmap.Width && y >= 0 && y < m_Pixelmap.Height)
			{
				return m_Pixelmap.DisplayBitmapPixels[x + m_Pixelmap.Width * y];
			}
			else
				return 0;
		}

		public static PSFFit GetPSFFitForPeakPixel(
			uint[,] data,
			PotentialStarStruct starToTest,
			float aboveNoiseLevelRequired,
			double minFWHM,
			double maxFWHM)
		{

			int STAR_MATRIX_FIT = TangraConfig.Settings.Special.StarFinderFitArea;
			double MIN_DISTANCE_OF_PEAK_PIXEL_FROM_CENTER = TangraConfig.Settings.Special.StarFinderMinDistanceOfPeakPixelFromCenter;


			PSFFit fit = new PSFFit(starToTest.X, starToTest.Y);
			int fitMatrix = (int)Math.Min(data.GetLength(0), STAR_MATRIX_FIT + 2);

			// Get a matrix with 1 pixel larger each way and set the border pixels to zero
			fit.Fit(data, fitMatrix, starToTest.X, starToTest.Y, true);

			if (fit.IsSolved)
			{
				double distanceFromCenter = ImagePixel.ComputeDistance(fit.X0_Matrix, fitMatrix / 2, fit.Y0_Matrix, fitMatrix / 2);

				if (fit.Certainty > 0 &&
					fit.FWHM >= minFWHM &&
					fit.FWHM <= maxFWHM &&
					distanceFromCenter < MIN_DISTANCE_OF_PEAK_PIXEL_FROM_CENTER &&
					fit.IMax > aboveNoiseLevelRequired)
				{
					//not good for lost tracking allow higher FWHM

					// This object passes all tests to be furhter considered as a star
					return fit;
				}
			}

			return null;
		}

    }
}

