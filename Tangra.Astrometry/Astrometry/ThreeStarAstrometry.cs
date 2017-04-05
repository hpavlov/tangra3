using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.Model.Astro;
using Tangra.Model.Helpers;
using Tangra.Model.Image;
using Tangra.Model.Numerical;
using Tangra.StarCatalogues;

namespace Tangra.Astrometry
{
    public class ThreeStarAstrometry : IAstrometricFit
    {
        private static int MAX_ATTEMPTS = 20;

        private static double DEG_TO_RAD = Math.PI/180.0;
        private static double RAD_TO_DEG = 180.0 / Math.PI;

        private double m_A;
        private double m_B;
        private double m_C;
        private double m_D;
        private double m_E;
        private double m_F;

        private double m_A0Rad;
        private double m_D0Rad;

        public Dictionary<ImagePixel, IStar> UserStars = new Dictionary<ImagePixel, IStar>();

        public ThreeStarAstrometry(AstroPlate image, Dictionary<ImagePixel, IStar> userStarIdentification, int tolerance)
        {
            if (userStarIdentification.Count != 3) 
                throw new InvalidOperationException();

            Image = image;
            UserStars = userStarIdentification.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            double a0 = userStarIdentification.Values.Average(x => x.RADeg) * DEG_TO_RAD;
            double d0 = userStarIdentification.Values.Average(x => x.DEDeg) * DEG_TO_RAD;
            double corr = double.MaxValue;
            int attempts = 0;

            do
            {
                SafeMatrix AX = new SafeMatrix(3, 3);
                SafeMatrix X = new SafeMatrix(3, 1);
                SafeMatrix AY = new SafeMatrix(3, 3);
                SafeMatrix Y = new SafeMatrix(3, 1);

                int i = 0;
                foreach (var pixel in userStarIdentification.Keys)
                {
                    IStar star = userStarIdentification[pixel];
                    double a = star.RADeg * DEG_TO_RAD;
                    double d = star.DEDeg * DEG_TO_RAD;

                    AX[i, 0] = pixel.XDouble;
                    AX[i, 1] = pixel.YDouble;
                    AX[i, 2] = 1;
                    AY[i, 0] = pixel.XDouble;
                    AY[i, 1] = pixel.YDouble;
                    AY[i, 2] = 1;

                    X[i, 0] = Math.Cos(d) * Math.Sin(a - a0) / (Math.Cos(d0) * Math.Cos(d) * Math.Cos(a - a0) + Math.Sin(d0) * Math.Sin(d));
                    Y[i, 0] = (Math.Cos(d0) * Math.Sin(d) - Math.Cos(d) * Math.Sin(d0) * Math.Cos(a - a0)) / (Math.Sin(d0) * Math.Sin(d) + Math.Cos(d0) * Math.Cos(d) * Math.Cos(a - a0));

                    i++;
                }

                SafeMatrix a_T = AX.Transpose();
                SafeMatrix aa = a_T * AX;
                SafeMatrix aa_inv = aa.Inverse();
                SafeMatrix bx = (aa_inv * a_T) * X;

                m_A = bx[0, 0];
                m_B = bx[1, 0];
                m_C = bx[2, 0];

                a_T = AY.Transpose();
                aa = a_T * AY;
                aa_inv = aa.Inverse();
                bx = (aa_inv * a_T) * Y;

                m_D = bx[0, 0];
                m_E = bx[1, 0];
                m_F = bx[2, 0];

                m_A0Rad = a0;
                m_D0Rad = d0;

                double ra_c, de_c;
                GetRADEFromImageCoords(Image.CenterXImage, Image.CenterYImage, out ra_c, out de_c);

                corr = AngleUtility.Elongation(ra_c, de_c, a0 * RAD_TO_DEG, d0 * RAD_TO_DEG) * 3600;
                a0 = ra_c * DEG_TO_RAD;
                d0 = de_c * DEG_TO_RAD;
                attempts++;
            }
            while (corr > tolerance && attempts < MAX_ATTEMPTS);

            Success = corr <= tolerance;
        }

        public void GetRADEFromImageCoords(double x, double y, out double RADeg, out double DEDeg)
        {
            double X = x * m_A + y * m_B + m_C;
            double Y = x * m_D + y * m_E + m_F;

            double a = m_A0Rad + Math.Atan(X / (Math.Cos(m_D0Rad) - Y * Math.Sin(m_D0Rad)));
            double tan_d = (Math.Cos(a - m_A0Rad) * (Y + Math.Tan(m_D0Rad))) / (1 - Y * Math.Tan(m_D0Rad));
            double d = Math.Atan(tan_d);
            //double sin_d = (Math.Sin(m_D0Rad) + Y * Math.Cos(m_D0Rad)) / Math.Sqrt(1 + X * X + Y * Y);
            //double d1 = Math.Asin(sin_d);
            
            RADeg = a * RAD_TO_DEG;
            DEDeg = d * RAD_TO_DEG;
        }

        public void GetImageCoordsFromRADE(double RADeg, double DEDeg, out double x, out double y)
        {
            double a = RADeg * DEG_TO_RAD;
            double d = DEDeg * DEG_TO_RAD;

            double X = Math.Cos(d) * Math.Sin(a - m_A0Rad) / (Math.Cos(m_D0Rad) * Math.Cos(d) * Math.Cos(a - m_A0Rad) + Math.Sin(m_D0Rad) * Math.Sin(d));
            double Y = (Math.Cos(m_D0Rad) * Math.Sin(d) - Math.Cos(d) * Math.Sin(m_D0Rad) * Math.Cos(a - m_A0Rad)) / (Math.Sin(m_D0Rad) * Math.Sin(d) + Math.Cos(m_D0Rad) * Math.Cos(d) * Math.Cos(a - m_A0Rad));

            x = (m_E * X - m_B * Y - m_E * m_C + m_B * m_F) /(m_E * m_A - m_B * m_D);
            y = (X - m_C - m_A * x) / m_B;
        }

        public double GetDistanceInArcSec(double x1, double y1, double x2, double y2)
        {
            double ra1, de1, ra2, de2;
            GetRADEFromImageCoords(x1, y1, out ra1, out de1);
            GetRADEFromImageCoords(x2, y2, out ra2, out de2);

            return AngleUtility.Elongation(ra1, de1, ra2, de2) * 3600.0;
        }

        public bool Success { get; private set; }

        public double RA0Deg
        {
            get { return m_A0Rad * RAD_TO_DEG; }
        }

        public double DE0Deg
        {
            get { return m_D0Rad * RAD_TO_DEG; }
        }

        public AstroPlate Image { get; private set; }

        public static ThreeStarAstrometry SolveByThreeStars(
            AstroPlate image,
            Dictionary<PSFFit, IStar> userStarIdentification,
            int tolerance)
        {
            Dictionary<ImagePixel, IStar> transformedDict =
                userStarIdentification.ToDictionary(
                    kvp => new ImagePixel(255, kvp.Key.XCenter, kvp.Key.YCenter),
                    kvp => kvp.Value);

            var solution = new ThreeStarAstrometry(image, transformedDict, tolerance);
            if (solution.Success)
                return solution;
            else
                return null;
        }
    }
}
