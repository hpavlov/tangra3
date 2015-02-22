using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Tangra.Model.Astro;

namespace Tangra.Model.Astro
{
    [Serializable]
    public class CCDMatrix : ISerializable
    {
        private double m_CellX;
        private double m_CellY;
        private int m_Width;
        private int m_Height;


		public CCDMatrix()
		{
			/* Used by Pyxis for XML Serialization */
		}

        public double CellX
        {
            get { return m_CellX; }
        }

        public double CellY
        {
            get { return m_CellY; }
        }

        public int Width
        {
            get { return m_Width; }
        }

        public int Height
        {
            get { return m_Height; }
        }

        public double CenterXMatrix 
        {
            get { return m_Width / 2.0; }
        }

        public double CenterYMatrix
        {
            get { return m_Height / 2.0; }
        }

        public CCDMatrix(double cellX, double cellY, int matrixWidth, int matrixHeight)
        {
            m_CellX = cellX;
            m_CellY = cellY;
            m_Width = matrixWidth;
            m_Height = matrixHeight;
        }

        public double GetDistanceInMicrons(int x1, int y1, int x2, int y2)
        {
            double dx = (x1 - x2) * CellX;
            double dy = (y1 - y2) * CellY;

            return Math.Sqrt(dx * dx + dy * dy);
        }

        #region ISerializable Members
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("m_CellX", m_CellX);
            info.AddValue("m_CellY", m_CellY);
            info.AddValue("m_Width", m_Width);
            info.AddValue("m_Height", m_Height);
        }

        public CCDMatrix(SerializationInfo info, StreamingContext context)
        {
            m_CellX = info.GetDouble("m_CellX");
            m_CellY = info.GetDouble("m_CellY");
            m_Width = info.GetInt32("m_Width");
            m_Height = info.GetInt32("m_Height");
        }

		public static CCDMatrix FromReflectedObject(object reflObj)
		{
			var rv = new CCDMatrix();

			rv.m_CellX = StarMap.GetPropValue<double>(reflObj, "m_CellX");
			rv.m_CellY = StarMap.GetPropValue<double>(reflObj, "m_CellY");
			rv.m_Width = StarMap.GetPropValue<int>(reflObj, "m_Width");
			rv.m_Height = StarMap.GetPropValue<int>(reflObj, "m_Height");
			
			return rv;
		}

    	#endregion
    }
}
