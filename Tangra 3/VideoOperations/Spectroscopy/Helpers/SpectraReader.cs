using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
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

		private static int SERIALIZATION_VERSION = 1;

		internal SpectraPoint()
		{ }

		public SpectraPoint(BinaryReader reader)
		{
			int version = reader.ReadInt32();

			PixelNo = reader.ReadInt32();
			Wavelength = reader.ReadSingle();
			RawSignal = reader.ReadSingle();
			RawSignalPixelCount = reader.ReadSingle();
			RawBackgroundPerPixel = reader.ReadSingle();
			RawValue = reader.ReadSingle();
			SmoothedValue = reader.ReadSingle();
		}

		public void WriteTo(BinaryWriter writer)
		{
			writer.Write(SERIALIZATION_VERSION);

			writer.Write(PixelNo);
			writer.Write(Wavelength);
			writer.Write(RawSignal);
			writer.Write(RawSignalPixelCount);
			writer.Write(RawBackgroundPerPixel);
			writer.Write(RawValue);
			writer.Write(SmoothedValue);
		}
	}

    public class MasterSpectra : Spectra
    {
        public int CombinedMeasurements;

		private static int SERIALIZATION_VERSION = 1;

	    internal MasterSpectra()
	    { }

	    public MasterSpectra(BinaryReader reader)
	    {
		    int version = reader.ReadInt32();

			CombinedMeasurements = reader.ReadInt32();
			SignalAreaWidth = reader.ReadInt32();
			MaxPixelValue = reader.ReadUInt32();
			MaxSpectraValue = reader.ReadUInt32();
			ZeroOrderPixelNo = reader.ReadInt32();

			int pixelsCount = reader.ReadInt32();
		    Points = new List<SpectraPoint>();
			for (int i = 0; i < pixelsCount; i++)
			{
				var point = new SpectraPoint(reader);
				Points.Add(point);
			}
	    }

	    public void WriteTo(BinaryWriter writer)
	    {
			writer.Write(SERIALIZATION_VERSION);

			writer.Write(CombinedMeasurements);
			writer.Write(SignalAreaWidth);
			writer.Write(MaxPixelValue);
		    writer.Write(MaxSpectraValue);
			writer.Write(ZeroOrderPixelNo);

			writer.Write(Points.Count);
		    foreach (var spectraPoint in Points)
		    {
				spectraPoint.WriteTo(writer);
		    }
	    }
    }

	public class Spectra
	{
		public int SignalAreaWidth;
		public int BackgroundAreaHalfWidth;
		public uint MaxPixelValue;
		public uint MaxSpectraValue;
	    public int ZeroOrderPixelNo;
		public List<SpectraPoint> Points = new List<SpectraPoint>();
	}

	public enum PixelCombineMethod
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
		private List<uint>[] m_BgValuesList;

		public SpectraReader(AstroImage image, float angleDegrees)
		{
			m_Image = image;
			m_Mapper = new RotationMapper(image.Width, image.Height, angleDegrees);
			m_SourceVideoFrame = new RectangleF(0, 0, image.Width, image.Height);
		}

		public Spectra ReadSpectra(float x0, float y0, int halfWidth, int bgHalfWidth, PixelCombineMethod bgMethod)
		{
			var rv = new Spectra()
			{
				SignalAreaWidth = 2 * halfWidth,
				BackgroundAreaHalfWidth = bgHalfWidth,
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
					int xx = (int) p.X;

					if (xx < xFrom) xFrom = xx;
					if (xx > xTo) xTo = xx;
				}
			}

			m_BgValues = new uint[xTo - xFrom + 1];
			m_BgPixelCount = new uint[xTo - xFrom + 1];
			m_BgValuesList = new List<uint>[xTo - xFrom + 1];

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
                if (bgMethod == PixelCombineMethod.Average)
				{
					ReadAverageBackgroundForPixelIndex(halfWidth, bgHalfWidth, x, p1.Y, x - xFrom);
				}
				else if (bgMethod == PixelCombineMethod.Median)
				{
					ReadMedianBackgroundForPixelIndex(halfWidth, bgHalfWidth, x, p1.Y, x - xFrom);
				}

				#endregion
			}

			// Apply background
			foreach(SpectraPoint point in rv.Points)
			{
                if (bgMethod == PixelCombineMethod.Average)
				{
					point.RawBackgroundPerPixel = GetAverageBackgroundValue(point.PixelNo, xFrom, xTo, bgHalfWidth);
				}
				else if (bgMethod == PixelCombineMethod.Median)
				{
					point.RawBackgroundPerPixel = GetMedianBackgroundValue(point.PixelNo, xFrom, xTo, bgHalfWidth);
				}

				point.RawValue -= point.RawBackgroundPerPixel * point.RawSignalPixelCount;
				if (point.RawValue < 0) point.RawValue = 0;
			}

			rv.MaxSpectraValue = (uint)Math.Ceiling(rv.Points.Where(x => x.PixelNo > rv.ZeroOrderPixelNo + 20).Select(x => x.RawValue).Max());

			return rv;
		}

		private float GetMedianBackgroundValue(int pixelNo, int xFrom, int xTo, int horizontalSpan)
		{
			var allAreaBgPixels = new List<uint>();
			int idxFrom = Math.Max(xFrom, pixelNo - horizontalSpan);
			int idxTo = Math.Min(xTo, pixelNo + horizontalSpan);

			for (int i = idxFrom; i <= idxTo; i++)
			{
				allAreaBgPixels.AddRange(m_BgValuesList[i - xFrom]);
			}

			allAreaBgPixels.Sort();

			return allAreaBgPixels.Count == 0 
				? 0 
				: allAreaBgPixels[allAreaBgPixels.Count / 2];
		}

		private float GetAverageBackgroundValue(int pixelNo, int xFrom, int xTo, int horizontalSpan)
		{
			int idxFrom = Math.Max(xFrom, pixelNo - horizontalSpan);
			int idxTo = Math.Min(xTo, pixelNo + horizontalSpan);
			float bgSum = 0;
			uint pixCount = 0;
			for (int i = idxFrom; i <= idxTo; i++)
			{
				bgSum += m_BgValues[i - xFrom];
				pixCount += m_BgPixelCount[i - xFrom];
			}
			return pixCount == 0 
				? 0 
				: bgSum / pixCount;
		}

		private void ReadMedianBackgroundForPixelIndex(int halfWidth, int bgHalfWidth, float x1, float y1, int index)
		{
			var allBgPixels = new List<uint>();

			for (int z = -bgHalfWidth - halfWidth; z < -halfWidth; z++)
			{
				PointF p = m_Mapper.GetSourceCoords(x1, y1 + z);
				int xx = (int)Math.Round(p.X);
				int yy = (int)Math.Round(p.Y);

				if (m_SourceVideoFrame.Contains(xx, yy))
				{
					allBgPixels.Add(m_Image.Pixelmap[xx, yy]);
				}
			}

			for (int z = halfWidth + 1; z <= halfWidth + bgHalfWidth; z++)
			{
				PointF p = m_Mapper.GetSourceCoords(x1, y1 + z);
				int xx = (int)Math.Round(p.X);
				int yy = (int)Math.Round(p.Y);

				if (m_SourceVideoFrame.Contains(xx, yy))
				{
					allBgPixels.Add(m_Image.Pixelmap[xx, yy]);
				}
			}

			m_BgValuesList[index] = allBgPixels;
			m_BgPixelCount[index] = 1;
		}

		private void ReadAverageBackgroundForPixelIndex(int halfWidth, int bgHalfWidth, float x1, float y1, int index)
		{
			uint bgValue = 0;
			uint bgPixelCount = 0;

			for (int z = - bgHalfWidth - halfWidth; z < -halfWidth; z++)
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

			for (int z = halfWidth + 1; z <= halfWidth + bgHalfWidth; z++)
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
