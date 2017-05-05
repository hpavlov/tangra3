using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.Model.Astro;

namespace Tangra.OCR.Model
{
    public class SubPixelImage
    {
        private int m_Factor = 5;
        private decimal m_Step;
        private int m_FactorSquare;
        private int m_Width;
        private int m_Height;
        private uint[] m_Pixels;

        private SubPixelImage()
        {
            m_FactorSquare = m_Factor * m_Factor;
            m_Step = 1.0M / m_Factor;
        }

        public SubPixelImage(AstroImage image)
            : this()
        {
            m_Width = image.Width;
            m_Height = image.Height;
            m_Pixels = image.GetOcrPixels();
        }

        public SubPixelImage(uint[] pixels, int width, int height)
            : this()
        {
            m_Width = width;
            m_Height = height;
            m_Pixels = pixels;
        }

        public decimal Width
        {
            get { return m_Width; }
        }

        public decimal Height
        {
            get { return m_Height; }
        }

        public decimal Step
        {
            get { return m_Step; }
        }

        public decimal GetWholePixelAt(decimal xf, decimal yf)
        {
            int x = (int)(xf);
            int y = (int)(yf);
            int x1 = x + 1;
            int y1 = y + 1;
            if (x1 >= m_Width) x1 = m_Width - 1;
            if (y1 >= m_Height) y1 = m_Height - 1;

            decimal xFactor = (1 - (xf - x));
            decimal x1Factor = (xf - x);
            decimal yFactor = (1 - (yf - y));
            decimal y1Factor = (yf - y);

            var sum =
                m_Pixels[x + m_Width * y] * xFactor * yFactor +
                m_Pixels[x1 + m_Width * y] * x1Factor * yFactor +
                m_Pixels[x + m_Width * y1] * xFactor * y1Factor +
                m_Pixels[x1 + m_Width * y1] * x1Factor * y1Factor;

            return sum;
        }
    }
}
