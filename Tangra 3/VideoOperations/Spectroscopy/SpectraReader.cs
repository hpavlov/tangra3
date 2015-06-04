using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using Tangra.Helpers;
using Tangra.Model.Astro;

namespace Tangra.VideoOperations.Spectroscopy
{
	public class SpectraPoint
	{
		public int PixelNo;
		public float Wavelength;
		public uint RawValue;
		public float SmoothedValue;
	}

	public class Spectra
	{
		public int PixelWidth;
		public uint MaxPixelValue;
		public List<SpectraPoint> Points = new List<SpectraPoint>();
	}

	public class SpectraReader
	{
		private AstroImage m_Image;
		private RotationMapper m_Mapper;
		private RectangleF m_SourceVideoFrame;

		public SpectraReader(AstroImage image, float angleDegrees)
		{
			m_Image = image;
			m_Mapper = new RotationMapper(image.Width, image.Height, angleDegrees);
			m_SourceVideoFrame = new RectangleF(0, 0, image.Width, image.Height);
		}

		public Spectra ReadSpectra(float x0, float y0, int halfWidth)
		{
			var rv = new Spectra()
			{
				PixelWidth = 2 * halfWidth,
				MaxPixelValue = (uint)(2 * halfWidth) * m_Image.Pixelmap.MaxPixelValue
			};

			int xFrom = int.MaxValue;
			int xTo = int.MinValue;

			// Find the destination pixel range at the destination horizontal
			PointF p1 = m_Mapper.GetDestCoords(x0, y0);
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
			

			// Get all readings in the range
			for (int x = xFrom; x <= xTo; x++)
			{
				var point = new SpectraPoint();
				point.PixelNo = x;

				for (int z = -halfWidth; z <= halfWidth; z++)
				{
					PointF p = m_Mapper.GetSourceCoords(x, p1.Y +z);
					int xx = (int)Math.Round(p.X);
					int yy = (int)Math.Round(p.Y);

					if (m_SourceVideoFrame.Contains(xx, yy))
						point.RawValue += m_Image.Pixelmap[xx, yy];
				}

				rv.Points.Add(point);
			}

			return rv;
		}
	}
}
