using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Tangra.Helpers
{
    public class RotationMapper
    {
        private double m_Sine;
        private double m_Cosine;
        private double m_Minx;
        private double m_Miny;
        private double m_Maxx;
        private double m_Maxy;

        public RotationMapper(int width, int height, double angleDegrees)
        {
            double angleRad = angleDegrees * Math.PI / 180;
            m_Sine = Math.Sin(angleRad);
            m_Cosine = Math.Cos(angleRad);

            double point1x = (-height * m_Sine);
            double point1y = (height * m_Cosine);
            double point2x = (width * m_Cosine - height * m_Sine);
            double point2y = (height * m_Cosine + width * m_Sine);
            double point3x = (width * m_Cosine);
            double point3y = (width * m_Sine);

            m_Minx = Math.Min((float)0, Math.Min(point1x, Math.Min(point2x, point3x)));
            m_Miny = Math.Min((float)0, Math.Min(point1y, Math.Min(point2y, point3y)));
            m_Maxx = Math.Max(point1x, Math.Max(point2x, point3x));
            m_Maxy = Math.Max(point1y, Math.Max(point2y, point3y));

            MaxDestDiagonal = (int)Math.Ceiling(Math.Sqrt(width * width + height * height));
        }

        public int MaxDestDiagonal { get; private set; }

        public PointF GetDestCoords(float x0, float y0)
        {
            double x1 = x0 * m_Cosine - y0 * m_Sine - m_Minx;
            double y1 = x0 * m_Sine + y0 * m_Cosine - m_Miny;

            return new PointF((float) x1, (float) y1);
        }

        public PointF GetSourceCoords(float x1, float y1)
        {
            double srcX = (x1 + m_Minx) * m_Cosine + (y1 + m_Miny) * m_Sine;
            double srcY = (y1 + m_Miny) * m_Cosine - (x1 + m_Minx) * m_Sine;

            return new PointF((float)srcX, (float)srcY);
        }
    }
}
