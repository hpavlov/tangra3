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

	    private float m_RawValue;
        public float RawValue 
        {
            get { return m_RawValue; }
            set
            {
                m_RawValue = value;
                ProcessedValue = m_RawValue;
            }
        }
	    public float SmoothedValue;

        public float ProcessedValue;

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

		public SpectraCalibration Calibration;
        public MeasurementInfo MeasurementInfo;

		public bool IsCalibrated()
		{
			return Calibration != null;
		}

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

	        MeasurementInfo = new MeasurementInfo(reader);
	        if (reader.ReadBoolean())
	            Calibration = new SpectraCalibration(reader);
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

            MeasurementInfo.WriteTo(writer);

            if (Calibration == null)
            {
                writer.Write(false);
            }
            else
            {
                writer.Write(true);
                Calibration.WriteTo(writer);    
            }
	    }
    }

	public class SpectraCalibration
	{
		public float Pixel1 { get; set; }
		public float Pixel2 { get; set; }
		public float Wavelength1 { get; set; }
		public float Wavelength2 { get; set; }
		public float Dispersion { get; set; }
		public float ZeroPixel { get; set; }

		private static int SERIALIZATION_VERSION = 1;

		internal SpectraCalibration()
		{ }

		public SpectraCalibration(BinaryReader reader)
		{
			int version = reader.ReadInt32();

			Pixel1 = reader.ReadSingle();
			Pixel2 = reader.ReadSingle();
			Wavelength1 = reader.ReadSingle();
			Wavelength2 = reader.ReadSingle();
			Dispersion = reader.ReadSingle();
			ZeroPixel = reader.ReadSingle();
		}

		public void WriteTo(BinaryWriter writer)
		{
			writer.Write(SERIALIZATION_VERSION);

			writer.Write(Pixel1);
			writer.Write(Pixel2);
			writer.Write(Wavelength1);
			writer.Write(Wavelength2);
			writer.Write(Dispersion);
			writer.Write(ZeroPixel);
		}
	}

    public class MeasurementInfo
    {
        // SpectraReductionContext items
        public int FramesToMeasure { get; set; }
        public int MeasurementAreaWing { get; set; }
        public int BackgroundAreaWing { get; set; }
        public PixelCombineMethod BackgroundMethod { get; set; }
        public PixelCombineMethod FrameCombineMethod { get; set; }
        public bool UseFineAdjustments { get; set; }
        public bool UseLowPassFilter { get; set; }

        // Other items
        public int FirstMeasuredFrame;
        public int LastMeasuredFrame;
        public DateTime? FirstFrameTimeStamp;
        public DateTime? LastFrameTimeStamp;

        private static int SERIALIZATION_VERSION = 1;

		internal MeasurementInfo()
		{ }

        public MeasurementInfo(BinaryReader reader)
		{
			int version = reader.ReadInt32();

            FramesToMeasure = reader.ReadInt32();
            MeasurementAreaWing = reader.ReadInt32();
            BackgroundAreaWing = reader.ReadInt32();
            BackgroundMethod = (PixelCombineMethod)reader.ReadInt32();
            FrameCombineMethod = (PixelCombineMethod)reader.ReadInt32();
            UseFineAdjustments = reader.ReadBoolean();
            UseLowPassFilter = reader.ReadBoolean();
            FirstMeasuredFrame = reader.ReadInt32();
            LastMeasuredFrame = reader.ReadInt32();
            bool hasFirstTimestamp = reader.ReadBoolean();
            if (hasFirstTimestamp) FirstFrameTimeStamp = new DateTime(reader.ReadInt64());
            bool hasLastTimestamp = reader.ReadBoolean();
            if (hasLastTimestamp) LastFrameTimeStamp = new DateTime(reader.ReadInt64());
		}

		public void WriteTo(BinaryWriter writer)
		{
			writer.Write(SERIALIZATION_VERSION);

            writer.Write(FramesToMeasure);
            writer.Write(MeasurementAreaWing);
            writer.Write(BackgroundAreaWing);
            writer.Write((int)BackgroundMethod);
            writer.Write((int)FrameCombineMethod);
            writer.Write(UseFineAdjustments);
            writer.Write(UseLowPassFilter);
            writer.Write(FirstMeasuredFrame);
            writer.Write(LastMeasuredFrame);
		    writer.Write(FirstFrameTimeStamp.HasValue);
            if (FirstFrameTimeStamp.HasValue) writer.Write(FirstFrameTimeStamp.Value.Ticks);
            writer.Write(LastFrameTimeStamp.HasValue);
            if (LastFrameTimeStamp.HasValue) writer.Write(LastFrameTimeStamp.Value.Ticks);
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
				    int xx = (int) x;

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
				        float sum = 0;
				        int numPoints = 0;
				        for (float kx = -0.4f; kx < 0.5f; kx+=0.2f)
                        for (float ky = -0.4f; ky < 0.5f; ky += 0.2f)
                        {
                            p = m_Mapper.GetSourceCoords(x + kx, p1.Y + ky + z);
                            int xxx = (int)Math.Round(p.X);
                            int yyy = (int)Math.Round(p.Y);
                            if (m_SourceVideoFrame.Contains(xxx, yyy))
                            {
                                sum += m_Image.Pixelmap[xxx, yyy];
                                numPoints++;
                            }
                        }
                        point.RawValue += (sum / numPoints);
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
