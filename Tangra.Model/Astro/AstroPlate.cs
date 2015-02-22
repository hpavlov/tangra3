using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Tangra.Model.Astro;
using Tangra.Model.Helpers;
using Tangra.Model.Image;

namespace Tangra.Model.Astro
{
    public enum CoordinateReference
    {
        TopLeftCorner,
        Center
    }

    [Serializable]
    public class AstroPlate : ISerializable
    {
        private CCDMatrix m_Matrix;
        private int m_ImageWidth;
        private int m_ImageHeight;
    	private int m_BitPix;

        private double m_MatrixToImageScaleX;
        private double m_MatrixToImageScaleY;

        public double EffectiveFocalLength = double.NaN;
        public double EffectivePixelWidth = double.NaN;
        public double EffectivePixelHeight = double.NaN;

        public readonly Rectangle FullFrame = Rectangle.Empty;

		public AstroPlate()
		{
			/* This is used from Pyxis for XML Serialization */
		}

		public AstroPlate(CCDMatrix matrix, int imageWidth, int imageHeight, int BitPix)
        {
        	m_BitPix = BitPix;
            m_Matrix = matrix;
            m_ImageWidth = imageWidth;
            m_ImageHeight = imageHeight;

            m_MatrixToImageScaleX = (double)m_ImageWidth / (double)m_Matrix.Width;
            m_MatrixToImageScaleY = (double)m_ImageHeight / (double)m_Matrix.Height;

            EffectivePixelWidth = m_Matrix.CellX * m_MatrixToImageScaleX;
            EffectivePixelHeight = m_Matrix.CellY * m_MatrixToImageScaleY;

            FullFrame = new Rectangle(0, 0, m_ImageWidth, m_ImageHeight);
        }

		public AstroPlate Clone()
		{
			CCDMatrix clonedMatrix = new CCDMatrix(m_Matrix.CellX, m_Matrix.CellY, m_Matrix.Width, m_Matrix.Height);
			AstroPlate clone = new AstroPlate(clonedMatrix, m_ImageWidth, m_ImageHeight, m_BitPix);

			clone.EffectivePixelWidth = this.EffectivePixelWidth;
			clone.EffectivePixelHeight = this.EffectivePixelHeight;
			clone.EffectiveFocalLength = this.EffectiveFocalLength;

			return clone;
		}

        public int ImageWidth
        {
            get { return m_ImageWidth; }
        }

        public int ImageHeight
        {
            get { return m_ImageHeight; }
        }

        public double CenterXImage
        {
            get { return m_ImageWidth / 2.0; }
        }

        public double CenterYImage
        {
            get { return m_ImageHeight / 2.0; }
        }

    	public int BitPix
    	{
    		get { return m_BitPix; }
    	}

        public double GetMaxFOVInArcSec()
        {
            if (double.IsNaN(EffectiveFocalLength))
                throw new InvalidOperationException("EffectiveFocalLength must be set before computing MaxFOVInDeg");

            double dxRad = m_ImageWidth * EffectivePixelWidth / (EffectiveFocalLength * 1000.0);
            double dyRad = m_ImageHeight * EffectivePixelHeight / (EffectiveFocalLength * 1000.0);
            return AngleUtility.Elongation(0, 0, dxRad * 180 / Math.PI, dyRad * 180 / Math.PI) * 3600.0;
        }

        public double GetDistanceInPixels(int x1, int y1, int x2, int y2)
        {
            double dx = (x1 - x2);
            double dy = (y1 - y2);

            return Math.Sqrt(dx * dx + dy * dy);
        }

		public double GetDistanceInPixels(double distanceInArcSec)
		{
			double distRad = (Math.PI * (distanceInArcSec / 3600) / 180);
			return distRad / (EffectivePixelWidth / (EffectiveFocalLength * 1000.0));
		}

        public double GetDistanceInArcSec(double x1, double y1, double x2, double y2)
        {
        	return GetDistanceInArcSec(x1, y1, x2, y2, EffectiveFocalLength);
        }

		public double GetDistanceInArcSec(double x1, double y1, double x2, double y2, double effectiveFocalLength)
		{
			double dxRad = (x1 - x2) * EffectivePixelWidth / (effectiveFocalLength * 1000.0);
			double dyRad = (y1 - y2) * EffectivePixelHeight / (effectiveFocalLength * 1000.0);
			return AngleUtility.Elongation(0, 0, dxRad * 180 / Math.PI, dyRad * 180 / Math.PI) * 3600.0;
		}

		public double GetDistanceInArcSec(double distanceInPixels)
		{
			double dxRad = (distanceInPixels) * EffectivePixelWidth / (EffectiveFocalLength * 1000.0);
			double dyRad = (distanceInPixels) * EffectivePixelHeight / (EffectiveFocalLength * 1000.0);
			return AngleUtility.Elongation(0, 0, dxRad * 180 / Math.PI, dyRad * 180 / Math.PI) * 3600.0;
		}

		public double GetFocalLengthFromMatch(
			double ijCelectialDist, double ikCelectialDist,
			ImagePixel iCenter, ImagePixel jCenter, ImagePixel kCenter,
			double toleranceInArcSec, double focalLenthAllowance)
		{
			// TODO: Should be able to solve this directly !!! ... may be

			// TODO: see how is the ratio changing when varying the focal length
			// TODO: Non linear least squares?
			double minFocLen = EffectiveFocalLength * (1 - focalLenthAllowance);
			double maxFocLen = EffectiveFocalLength * (1 + focalLenthAllowance);

			double maxDist_ij = GetDistanceInArcSec(iCenter.XDouble, iCenter.YDouble, jCenter.XDouble, jCenter.YDouble, minFocLen);
			double minDist_ij = GetDistanceInArcSec(iCenter.XDouble, iCenter.YDouble, jCenter.XDouble, jCenter.YDouble, maxFocLen);

			double maxDist_ik = GetDistanceInArcSec(iCenter.XDouble, iCenter.YDouble, kCenter.XDouble, kCenter.YDouble, minFocLen);
			double minDist_ik = GetDistanceInArcSec(iCenter.XDouble, iCenter.YDouble, kCenter.XDouble, kCenter.YDouble, maxFocLen);

			if (ijCelectialDist < minDist_ij - toleranceInArcSec ||
				ijCelectialDist > maxDist_ij + toleranceInArcSec)
				return double.NaN;

			if (ikCelectialDist < minDist_ik - toleranceInArcSec ||
				ikCelectialDist > maxDist_ik + toleranceInArcSec)
				return double.NaN;

			int MAX_ITTERATIONS = 50;

			double fittedFocalLength = EffectiveFocalLength;
			double fitToleranceInArcSec = toleranceInArcSec * 0.1;

			double bestFitFocLen = fittedFocalLength;
			double bestFitResidual = double.MaxValue;

			int itteratons = 0;
			do
			{
				itteratons++;

				double maxijDist = GetDistanceInArcSec(iCenter.XDouble, iCenter.YDouble, jCenter.XDouble, jCenter.YDouble, minFocLen);
				double minijDist = GetDistanceInArcSec(iCenter.XDouble, iCenter.YDouble, jCenter.XDouble, jCenter.YDouble, maxFocLen);
				double maxikDist = GetDistanceInArcSec(iCenter.XDouble, iCenter.YDouble, kCenter.XDouble, kCenter.YDouble, minFocLen);
				double minikDist = GetDistanceInArcSec(iCenter.XDouble, iCenter.YDouble, kCenter.XDouble, kCenter.YDouble, maxFocLen); 
			
				double distij = GetDistanceInArcSec(iCenter.XDouble, iCenter.YDouble, jCenter.XDouble, jCenter.YDouble, fittedFocalLength);
				double distik = GetDistanceInArcSec(iCenter.XDouble, iCenter.YDouble, kCenter.XDouble, kCenter.YDouble, fittedFocalLength);

				double ijDiff = Math.Abs(ijCelectialDist - distij);
				double ikDiff = Math.Abs(ikCelectialDist - distik);

				double residual = Math.Sqrt(ijDiff * ijDiff + ikDiff * ikDiff);
				if (bestFitResidual > residual)
				{
					bestFitResidual = residual;
					bestFitFocLen = fittedFocalLength;
				}

				if (ijDiff < fitToleranceInArcSec &&
					ikDiff < fitToleranceInArcSec) break;

				if (ijCelectialDist < minDist_ij - toleranceInArcSec) return double.NaN;
				if (maxDist_ij + toleranceInArcSec < ijCelectialDist) return double.NaN;
				if (ikCelectialDist < minDist_ik - toleranceInArcSec) return double.NaN;
				if (maxDist_ik + toleranceInArcSec < ikCelectialDist) return double.NaN;

				bool closerToMax = ijDiff > ikDiff
					? maxijDist - ijCelectialDist > ijCelectialDist - minijDist
					: maxikDist - ikCelectialDist > ikCelectialDist - minikDist;

				if (closerToMax)
				{
					// It is closer to the max distance (min focal length)	
					minFocLen = fittedFocalLength;
					fittedFocalLength = (minFocLen + maxFocLen) / 2.0;
				}
				else
				{
					// It is closer to the min distance (max focal length)
					maxFocLen = fittedFocalLength;
					fittedFocalLength = (minFocLen + maxFocLen) / 2.0;
				}
			}
			while (itteratons < MAX_ITTERATIONS);

			double dist_ij = GetDistanceInArcSec(iCenter.XDouble, iCenter.YDouble, jCenter.XDouble, jCenter.YDouble, bestFitFocLen);
			double dist_ik = GetDistanceInArcSec(iCenter.XDouble, iCenter.YDouble, kCenter.XDouble, kCenter.YDouble, bestFitFocLen);

		   if (Math.Abs(ijCelectialDist - dist_ij) < toleranceInArcSec &&
			   Math.Abs(ikCelectialDist - dist_ik) < toleranceInArcSec)
		   {
			   return bestFitFocLen;
		   }
		   else
		   {
//#if UNIT_TESTS
//                double dRad = (ratioDifference / 3600.0) * Math.PI / 180;
//                double dx = (dRad) / (EffectivePixelWidth / (EffectiveFocalLength * 1000.0));
//                double dy = (dRad) / (EffectivePixelHeight / (EffectiveFocalLength * 1000.0));
//                Console.WriteLine(string.Format("INSUFF TOLERANCE: {0} [{2}x{3}px] -> {1}", ratioDifference, toleranceInArcSec, dx.ToString("0.0"), dy.ToString("0.0")));
//#endif
			   return double.NaN;
		   }
		}

    	public void ImageCoordsToMatrixCoords(double imgX, double imgY, out double mtxX, out double mtxY, CoordinateReference coorRef)
        {
            if (coorRef == CoordinateReference.TopLeftCorner)
            {
                double imgDX = (imgX - CenterXImage);
                double imgDY = (imgY - CenterYImage);

                mtxX = m_Matrix.CenterXMatrix + imgDX / m_MatrixToImageScaleX;
                mtxY = m_Matrix.CenterYMatrix + imgDY / m_MatrixToImageScaleY;
            }
            else
            {
                mtxX = imgX / m_MatrixToImageScaleX;
                mtxY = imgY / m_MatrixToImageScaleY;
            }
        }

        public void MatrixCoordsToImageCoords(double mtxX, double mtxY, out double imgX, out double imgY, CoordinateReference coorRef)
        {
            if (coorRef == CoordinateReference.TopLeftCorner)
            {
                double mtxDX = (mtxX - m_Matrix.CenterXMatrix);
                double mtxDY = (mtxY - m_Matrix.CenterYMatrix);

                imgX = CenterXImage + mtxDX * m_MatrixToImageScaleX;
                imgY = CenterYImage + mtxDY * m_MatrixToImageScaleY;
            }
            else
            {
                imgX = mtxX * m_MatrixToImageScaleX;
                imgY = mtxY * m_MatrixToImageScaleY;
            }
        }

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("m_ImageWidth", m_ImageWidth);
            info.AddValue("m_ImageHeight", m_ImageHeight);
            info.AddValue("m_MatrixToImageScaleX", m_MatrixToImageScaleX);
            info.AddValue("m_MatrixToImageScaleY", m_MatrixToImageScaleY);
            info.AddValue("EffectiveFocalLength", EffectiveFocalLength);
            info.AddValue("EffectivePixelWidth", EffectivePixelWidth);
            info.AddValue("EffectivePixelHeight", EffectivePixelHeight);
            
            BinaryFormatter fmt = new BinaryFormatter();
            using(MemoryStream mem = new MemoryStream())
            {
                fmt.Serialize(mem, m_Matrix);
                mem.Seek(0, SeekOrigin.Begin);

                info.AddValue("m_Matrix", mem.ToArray());
            }

			info.AddValue("BitPix", m_BitPix);
        }


        public AstroPlate(SerializationInfo info, StreamingContext context)
        {
            m_ImageWidth = info.GetInt32("m_ImageWidth");
            m_ImageHeight = info.GetInt32("m_ImageHeight");
            m_MatrixToImageScaleX = info.GetDouble("m_MatrixToImageScaleX");
            m_MatrixToImageScaleY = info.GetDouble("m_MatrixToImageScaleY");
            EffectiveFocalLength = info.GetDouble("EffectiveFocalLength");
            EffectivePixelWidth = info.GetDouble("EffectivePixelWidth");
            EffectivePixelHeight = info.GetDouble("EffectivePixelHeight");

            byte[] data = (byte[])info.GetValue("m_Matrix", typeof(byte[]));

            BinaryFormatter fmt = new BinaryFormatter();
            using (MemoryStream mem = new MemoryStream(data))
            {
                m_Matrix = (CCDMatrix)fmt.Deserialize(mem);
            }

			try
			{
				m_BitPix = info.GetInt32("m_BitPix");
			}
			catch
			{
				m_BitPix = 8;
			}
        }

		public static AstroPlate FromReflectedObject(object reflObj)
		{
			var rv = new AstroPlate();

			rv.m_ImageWidth = StarMap.GetPropValue<int>(reflObj, "m_ImageWidth");
			rv.m_ImageHeight = StarMap.GetPropValue<int>(reflObj, "m_ImageHeight");
			rv.m_MatrixToImageScaleX = StarMap.GetPropValue<double>(reflObj, "m_MatrixToImageScaleX");
			rv.m_MatrixToImageScaleY = StarMap.GetPropValue<double>(reflObj, "m_MatrixToImageScaleY");
			rv.EffectiveFocalLength = StarMap.GetPropValue<double>(reflObj, "EffectiveFocalLength");
			rv.EffectivePixelWidth = StarMap.GetPropValue<double>(reflObj, "EffectivePixelWidth");
			rv.EffectivePixelHeight = StarMap.GetPropValue<double>(reflObj, "EffectivePixelHeight");

			object obj = StarMap.GetPropValue<object>(reflObj, "m_Matrix");
			rv.m_Matrix = CCDMatrix.FromReflectedObject(obj);

			try
			{
				rv.m_BitPix = StarMap.GetPropValue<int>(reflObj, "BitPix");	
			}
			catch
			{
				rv.m_BitPix = 8;
			}
			
			return rv;
		}

    	#endregion
    }
}
