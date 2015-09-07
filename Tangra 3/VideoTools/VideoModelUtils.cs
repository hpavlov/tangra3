using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Tangra.Model.Image;

namespace Tangra.VideoTools
{
    public static class VideoModelUtils
    {
        private static RNGCryptoServiceProvider cryptoRand = new RNGCryptoServiceProvider();

        public static double Random(double mean, double stdDev)
        {
            if (Math.Abs(stdDev) < 0.5) return mean;

            byte[] twoBytes = new byte[2];
            cryptoRand.GetBytes(twoBytes);
            double u1 = twoBytes[0] * 1.0 / 0xFF; //these are uniform(0,1) random doubles
            if (u1 == 0) u1 = 1; // corner case handling to avoid Log(0)
            double u2 = twoBytes[1] * 1.0 / 0xFF;
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            double randNormal = mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
            return randNormal;
        }

        public static void GenerateNoise(Bitmap bmp, int[,] simulatedBackground, int mean, int stdDev)
        {
            BitmapData bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;
            unsafe
            {
                byte* p = (byte*)(void*)Scan0;

                int nOffset = stride - bmp.Width * 3;

                for (int y = 0; y < bmp.Height; ++y)
                {
                    for (int x = 0; x < bmp.Width; ++x)
                    {
                        double bgPixel = Math.Min(255, Math.Max(0, simulatedBackground[x, y] + Math.Abs(Random(mean, stdDev))));
                        byte val = (byte)Math.Round(bgPixel);

                        p[0] = val;
                        p[1] = val;
                        p[2] = val;

                        p += 3;
                    }
                    p += nOffset;
                }
            }

            bmp.UnlockBits(bmData);
        }

        public static void GenerateStar(Bitmap bmp, float x0, float y0, float fwhm, float iMax)
        {
            double r0 = fwhm / (2 * Math.Sqrt(Math.Log(2)));
            int maxPsfModelDist = (int)(6 * fwhm);

            // GDI+ still lies to us - the return format is BGR, NOT RGB.
            BitmapData bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;
            unsafe
            {
                byte* p = (byte*)(void*)Scan0;

                int nOffset = stride - bmp.Width * 3;

                for (int y = 0; y < bmp.Height; ++y)
                {
                    for (int x = 0; x < bmp.Width; ++x)
                    {
                        if (Math.Abs(x - x0) < maxPsfModelDist && Math.Abs(y - y0) < maxPsfModelDist)
                        {
                            int counter = 0;
                            double sum = 0;
                            for (double dx = -0.5; dx < 0.5; dx += 0.1)
                            {
                                for (double dy = -0.5; dy < 0.5; dy += 0.1)
                                {
                                    double thisVal = Math.Min(255, Math.Max(0, iMax * Math.Exp(-((x + dx - x0) * (x + dx - x0) + (y + dy - y0) * (y + dy - y0)) / (r0 * r0))));
                                    sum += thisVal;
                                    counter++;
                                }
                            }

                            int val = (int)Math.Round(sum / counter);

                            int p1 = p[0] + val;
                            int p2 = p[1] + val;
                            int p3 = p[2] + val;

                            p[0] = (byte)Math.Min(255, Math.Max(0, p1));
                            p[1] = (byte)Math.Min(255, Math.Max(0, p2));
                            p[2] = (byte)Math.Min(255, Math.Max(0, p3));
                        }

                        p += 3;
                    }
                    p += nOffset;
                }
            }

            bmp.UnlockBits(bmData);
        }

        public static void GenerateNoise(Pixelmap pixelmap, int[,] simulatedBackground, int mean, int stdDev)
        {
            uint maxSignalValue = pixelmap.MaxSignalValue;

            for (int y = 0; y < pixelmap.Height; ++y)
            {
                for (int x = 0; x < pixelmap.Width; ++x)
                {
                    double bgPixel = Math.Min(maxSignalValue, Math.Max(0, simulatedBackground[x, y] + Math.Abs(Random(mean, stdDev))));
                    uint val = (uint)Math.Round(bgPixel);

                    pixelmap[x, y] = val;
                }
            }
        }

        public static void GenerateStar(Pixelmap pixelmap, float x0, float y0, float fwhm, float iMax, int psfModel)
        {
            double r0 = fwhm / (2 * Math.Sqrt(Math.Log(2)));
            int maxPsfModelDist = (int)(6 * fwhm);
            uint maxSignalValue = pixelmap.MaxSignalValue;
            double fiveThirds = 5.0 / 3.0;
            for (int y = 0; y < pixelmap.Height; ++y)
            {
                for (int x = 0; x < pixelmap.Width; ++x)
                {
                    if (Math.Abs(x - x0) < maxPsfModelDist && Math.Abs(y - y0) < maxPsfModelDist)
                    {
                        int counter = 0;
                        double sum = 0;
                        for (double dx = -0.5; dx < 0.5; dx += 0.1)
                        {
                            for (double dy = -0.5; dy < 0.5; dy += 0.1)
                            {
                                double modelVal = 0;
                                if (psfModel == 0) 
                                    modelVal = iMax * Math.Exp(-((x + dx - x0) * (x + dx - x0) + (y + dy - y0) * (y + dy - y0)) / (r0 * r0));
                                else if (psfModel == 1)
                                    modelVal = iMax * Math.Exp(-(Math.Pow((x + dx - x0), fiveThirds) + Math.Pow((y + dy - y0), fiveThirds)) / (r0 * r0));

                                double thisVal = Math.Min(maxSignalValue, Math.Max(0, modelVal));
                                sum += thisVal;
                                counter++;
                            }
                        }

                        long val = (long)Math.Round(sum / counter);
                        val += (long)pixelmap[x, y];

                        pixelmap[x, y] = (uint)Math.Min(maxSignalValue, Math.Max(0, val));
                    }
                }
            }
        }
    }
}
