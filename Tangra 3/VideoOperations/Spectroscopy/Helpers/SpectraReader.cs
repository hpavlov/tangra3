using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using Tangra.Helpers;
using Tangra.Model.Astro;

namespace Tangra.VideoOperations.Spectroscopy.Helpers
{
	public class SpectraPoint
	{
		public int PixelNo;
		public float Wavelength;
        public float RawSignal;
        public float RawSignalPixelCount;        
		public float RawBackgroundPerPixel;
        public float RawValue;
		public float SmoothedValue;
	}

	public class Spectra
	{
		public int PixelWidth;
		public uint MaxPixelValue;
	    public int ZeroOrderPixelNo;
		public List<SpectraPoint> Points = new List<SpectraPoint>();
	}

	public enum SpectraCombineMethod
	{
		Average,
		Median
	}

	public class SpectraReader
	{
		private AstroImage m_Image;
		private RotationMapper m_Mapper;
		private RectangleF m_SourceVideoFrame;

	    private uint[] m_BgValues;
		private uint[] m_BgPixelCount;

		public SpectraReader(AstroImage image, float angleDegrees)
		{
			m_Image = image;
			m_Mapper = new RotationMapper(image.Width, image.Height, angleDegrees);
			m_SourceVideoFrame = new RectangleF(0, 0, image.Width, image.Height);
		}

		public Spectra ReadSpectra(float x0, float y0, int halfWidth, SpectraCombineMethod bgMethod = SpectraCombineMethod.Average)
		{
			var rv = new Spectra()
			{
				PixelWidth = 2 * halfWidth,
                MaxPixelValue = (uint)(2 * halfWidth) * m_Image.Pixelmap.MaxSignalValue
			};

			int xFrom = int.MaxValue;
			int xTo = int.MinValue;

			// Find the destination pixel range at the destination horizontal
			PointF p1 = m_Mapper.GetDestCoords(x0, y0);
		    rv.ZeroOrderPixelNo = (int)Math.Round(p1.X);
            
			for (float x = p1.X - m_Mapper.MaxDestDiagonal; x < p1.X + m_Mapper.MaxDestDiagonal; x++)
			{
				PointF p = m_Mapper.GetSourceCoords(x, p1.Y);
				if (m_SourceVideoFrame.Contains(p))
				{
					int xx = (int)Math.Round(p.X);

					if (xx < xFrom) xFrom = xx;
					if (xx > xTo) xTo = xx;
				}
			}

			m_BgValues = new uint[xTo - xFrom + 1];
			m_BgPixelCount = new uint[xTo - xFrom + 1];

			// Get all readings in the range
			for (int x = xFrom; x <= xTo; x++)
			{
				var point = new SpectraPoint();
				point.PixelNo = x;
			    point.RawSignalPixelCount = 0;

				for (int z = -halfWidth; z <= halfWidth; z++)
				{
					PointF p = m_Mapper.GetSourceCoords(x, p1.Y +z);
					int xx = (int)Math.Round(p.X);
					int yy = (int)Math.Round(p.Y);

				    if (m_SourceVideoFrame.Contains(xx, yy))
				    {
				        point.RawValue += m_Image.Pixelmap[xx, yy];
				        point.RawSignalPixelCount++;
				    }
				}

			    point.RawSignal = point.RawValue;
				rv.Points.Add(point);

				#region Reads background 
				if (bgMethod == SpectraCombineMethod.Average)
				{
					ReadAverageBackgroundForPixelIndex(halfWidth, x, p1.Y, x - xFrom);
				}
				#endregion
			}

			// Apply background
			foreach(SpectraPoint point in rv.Points)
			{
				if (bgMethod == SpectraCombineMethod.Average)
				{
					point.RawBackgroundPerPixel = GetAverageBackgroundValue(point.PixelNo, xFrom, xTo, halfWidth);
					point.RawValue -= point.RawBackgroundPerPixel * point.RawSignalPixelCount;
				    if (point.RawValue < 0) point.RawValue = 0;
				}
			}

			return rv;
		}

		private float GetAverageBackgroundValue(int pixelNo, int xFrom, int xTo, int halfWidth)
		{
			int idxFrom = Math.Max(xFrom, pixelNo - halfWidth);
			int idxTo = Math.Min(xTo, pixelNo + halfWidth);
			float bgSum = 0;
			uint pixCount = 0;
			for (int i = idxFrom; i <= idxTo; i++)
			{
				bgSum += m_BgValues[i - xFrom];
				pixCount += m_BgPixelCount[i - xFrom];
			}
			return bgSum / pixCount;
		}

		private void ReadAverageBackgroundForPixelIndex(int halfWidth, float x1, float y1, int index)
		{
			uint bgValue = 0;
			uint bgPixelCount = 0;

			for (int z = -2 * halfWidth; z < -halfWidth; z++)
			{
				PointF p = m_Mapper.GetSourceCoords(x1, y1 + z);
				int xx = (int)Math.Round(p.X);
				int yy = (int)Math.Round(p.Y);

				if (m_SourceVideoFrame.Contains(xx, yy))
				{
					bgValue += m_Image.Pixelmap[xx, yy];
					bgPixelCount++;
				}
			}

			for (int z = halfWidth + 1; z <= 2 * halfWidth; z++)
			{
				PointF p = m_Mapper.GetSourceCoords(x1, y1 + z);
				int xx = (int)Math.Round(p.X);
				int yy = (int)Math.Round(p.Y);

				if (m_SourceVideoFrame.Contains(xx, yy))
				{
					bgValue += m_Image.Pixelmap[xx, yy];
					bgPixelCount++;
				}
			}

			m_BgValues[index] = bgValue;
			m_BgPixelCount[index] = bgPixelCount;
		}
	}
}
