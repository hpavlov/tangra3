using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Model.Image;

namespace Tangra.Model.Astro
{
	public abstract class StarMapBase
	{
		protected Pixelmap m_Pixelmap;
		protected int m_FullWidth = 0;
		protected int m_FullHeight = 0;

		
		protected uint m_NoiseLevel = 0;
		protected uint m_AverageBackgroundNoise;

		public abstract StarMapFeature GetFeatureInRadius(int x, int y, int radius);

		public ImagePixel GetPSFFit(int x, int y, PSFFittingMethod method)
		{
			PSFFit psfFit;
			return GetPSFFit(x, y, method, out psfFit);
		}

		public ImagePixel GetPSFFit(int x, int y, PSFFittingMethod method, out PSFFit psfFit)
		{
			StarMapFeature feature = GetFeatureInRadius(x, y, 5);
			int dimention = 9;
			if (CoreAstrometrySettings.Default.SearchAreaAuto)
			{
				if (feature != null)
				{
					if (feature.PixelCount > 25) dimention = 9;
					if (feature.PixelCount > 40) dimention = 11;
					if (feature.PixelCount > 60) dimention = 13;
					if (feature.PixelCount > 80) dimention = 15;
				}
			}
			else
                dimention = 2 * ((int)Math.Round(CoreAstrometrySettings.Default.SearchArea) / 2) + 1;

			return GetPSFFit(x, y, dimention, method, out psfFit);
		}

		public ImagePixel GetPSFFit(int x, int y, int fitMatrixSize, out PSFFit psfFit)
		{
			return GetPSFFit(x, y, fitMatrixSize, PSFFittingMethod.NonLinearFit, out psfFit);
		}

		public ImagePixel GetPSFFit(int x, int y, int fitMatrixSize, PSFFittingMethod method, out PSFFit psfFit)
		{
			psfFit = new PSFFit(x, y);
			psfFit.FittingMethod = method;

			int dimention = 2 * (fitMatrixSize / 2) + 1;

			uint[,] data = new uint[dimention, dimention];
			int halfWidth = dimention / 2;

#if ASTROMETRY_DEBUG
			Trace.Assert(2 * halfWidth + 1 == dimention);
#endif

            PixelAreaOperation2(x, y, halfWidth,
			   delegate(int x1, int y1, uint z)
			   {
				   data[x1 - x + halfWidth, y1 - y + halfWidth] = z;
			   });

			psfFit.Fit(data);
			if (psfFit.IsSolved)
			{
				ImagePixel psfPixel = new ImagePixel((int)Math.Min(m_Pixelmap.MaxSignalValue, (uint)Math.Round(psfFit.IMax)), psfFit.XCenter, psfFit.YCenter);
				psfPixel.SignalNoise = psfFit.Certainty;
				return psfPixel;
			}

			return null;
		}


		public ImagePixel GetCentroid(int x, int y, int radius)
		{
			return GetCentroid(x, y, radius, false);
		}

		public ImagePixel GetCentroid(int x, int y, int radius, bool doPeakPixelFirst)
		{
			uint minimum = UInt32.MaxValue;
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

			if (maximum < m_AverageBackgroundNoise) 
				return null;

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

                PixelAreaOperation2(xMax, yMax, radius,
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
				return ImagePixel.Unspecified;

			ImagePixel retVal = new ImagePixel((int)m_Pixelmap[(int)Math.Round(xx), (int)Math.Round(yy)], xx, yy);

			// This is used for relative comparison between ImagePixels
			retVal.SignalNoise = retVal.Brightness * 1.0 / m_NoiseLevel;

			return retVal;
		}

		internal delegate void PixelAreaOperationCallback(int x, int y, uint z);
		internal void PixelAreaOperation(int x, int y, int radius, PixelAreaOperationCallback callback)
		{
			for (int i = x - radius; i <= x + radius; i++)
				for (int j = y - radius; j <= y + radius; j++)
				{
					if (i >= 0 && i < m_FullWidth && j >= 0 && j < m_FullHeight)
					{
						callback(i, j, m_Pixelmap[i, j] /** pixel*/);
					}
				}
		}

		internal delegate void PixelAreaOperationCallback2(int x, int y, uint z);
		internal void PixelAreaOperation2(int x, int y, int radius, PixelAreaOperationCallback2 callback)
		{
			for (int i = x - radius; i <= x + radius; i++)
				for (int j = y - radius; j <= y + radius; j++)
				{
					if (i >= 0 && i < m_FullWidth && j >= 0 && j < m_FullHeight)
					{
						callback(i, j, m_Pixelmap[i, j] /** pixel*/);
					}
				}
		}


		public uint[,] GetImageArea(int x, int y, int width)
		{
			uint[,] output = new uint[width, width];

			int halfWidth = width / 2;
			PixelAreaOperation(x, y, 2 + halfWidth,
				   delegate(int x1, int y1, uint z)
				   {
					   int xo = x1 - x + halfWidth;
					   int yo = y1 - y + halfWidth;

					   if (xo >= 0 && yo >= 0 &&
						   xo < width && yo < width)
						   output[xo, yo] = z;

				   });

			return output;
		}
	}
}
