using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.Model.Image
{
    public class ConvMatrix
    {
        public float TopLeft = 0, TopMid = 0, TopRight = 0;
        public float MidLeft = 0, Pixel = 1, MidRight = 0;
        public float BottomLeft = 0, BottomMid = 0, BottomRight = 0;
        public int Factor = 1;
        public int Offset = 0;

        public void SetAll(int nVal)
        {
            TopLeft = TopMid = TopRight = MidLeft = Pixel = MidRight = BottomLeft = BottomMid = BottomRight = nVal;
        }

        public ConvMatrix()
        { }

        public ConvMatrix(float[,] matrix)
        {
            TopLeft = matrix[0, 0];
            TopMid = matrix[0, 1];
            TopRight = matrix[0, 2];

            MidLeft = matrix[1, 0];
            Pixel = matrix[1, 1];
            MidRight = matrix[1, 2];

            BottomLeft = matrix[2, 0];
            BottomMid = matrix[2, 1];
            BottomRight = matrix[2, 2];
        }
    }

    public class ConvMatrix5x5
    {
        public readonly float[,] Values;

        public ConvMatrix5x5(float[,] matrix)
        {
            Values = matrix;
        }
    }

    public static class Convolution
    {
        // http://www.student.kuleuven.be/~m0216922/CG/filtering.html

        public static bool Conv3x3(Pixelmap image, ConvMatrix m)
        {
            // Avoid divide by zero errors
            if (0 == m.Factor)
                return false;

            var result = new Pixelmap(image.Width, image.Height, image.BitPixCamera, image.Pixels, image.DisplayBitmap, image.DisplayBitmapPixels);

            for (int y = 0; y < image.Height - 2; ++y)
            {
                for (int x = 0; x < image.Width - 2; ++x)
                {
                    ulong nPixel = (ulong)Math.Round((((image[x, y] * m.TopLeft) +
                                                (image[x + 1, y] * m.TopMid) +
                                                (image[x + 2, y] * m.TopRight) +
                                                (image[x, y + 1] * m.MidLeft) +
                                                (image[x + 1, y + 1] * m.Pixel) +
                                                (image[x + 2, y + 1] * m.MidRight) +
                                                (image[x, y + 2] * m.BottomLeft) +
                                                (image[x, y + 2] * m.BottomMid) +
                                                (image[x, y + 2] * m.BottomRight))
                                                / m.Factor) + m.Offset);

                    if (nPixel < 0) nPixel = 0;
                    if (nPixel > image.MaxPixelValue) nPixel = image.MaxPixelValue;
                    result[x + 1, y] = (uint)nPixel;
                }
            }

            //// GDI+ still lies to us - the return format is BGR, NOT RGB. 
            //using (Bitmap bSrc = (Bitmap)b.Clone())
            //{
            //    BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
            //                                   ImageLockMode.ReadWrite,
            //                                   PixelFormat.Format24bppRgb);
            //    BitmapData bmSrc = bSrc.LockBits(new Rectangle(0, 0, bSrc.Width, bSrc.Height),
            //                                     ImageLockMode.ReadWrite,
            //                                     PixelFormat.Format24bppRgb);
            //    int stride = bmData.Stride;
            //    int stride2 = stride*2;

            //    System.IntPtr Scan0 = bmData.Scan0;
            //    System.IntPtr SrcScan0 = bmSrc.Scan0;

            //    unsafe
            //    {
            //        byte* p = (byte*) (void*) Scan0;
            //        byte* pSrc = (byte*) (void*) SrcScan0;
            //        int nOffset = stride - b.Width*3;
            //        int nWidth = b.Width - 2;
            //        int nHeight = b.Height - 2;

            //        int nPixel;

            //        for (int y = 0; y < nHeight; ++y)
            //        {
            //            for (int x = 0; x < nWidth; ++x)
            //            {
            //                nPixel = (int) Math.Round((((pSrc[2]*m.TopLeft) +
            //                                            (pSrc[5]*m.TopMid) +
            //                                            (pSrc[8]*m.TopRight) +
            //                                            (pSrc[2 + stride]*m.MidLeft) +
            //                                            (pSrc[5 + stride]*m.Pixel) +
            //                                            (pSrc[8 + stride]*m.MidRight) +
            //                                            (pSrc[2 + stride2]*m.BottomLeft) +
            //                                            (pSrc[5 + stride2]*m.BottomMid) +
            //                                            (pSrc[8 + stride2]*m.BottomRight))
            //                                           /m.Factor) + m.Offset);

            //                if (nPixel < 0) nPixel = 0;
            //                if (nPixel > 255) nPixel = 255;
            //                p[5 + stride] = (byte) nPixel;

            //                nPixel = (int) Math.Round(((((pSrc[1]*m.TopLeft) +
            //                                             (pSrc[4]*m.TopMid) +
            //                                             (pSrc[7]*m.TopRight) +
            //                                             (pSrc[1 + stride]*m.MidLeft) +
            //                                             (pSrc[4 + stride]*m.Pixel) +
            //                                             (pSrc[7 + stride]*m.MidRight) +
            //                                             (pSrc[1 + stride2]*m.BottomLeft) +
            //                                             (pSrc[4 + stride2]*m.BottomMid) +
            //                                             (pSrc[7 + stride2]*m.BottomRight))
            //                                            /m.Factor) + m.Offset));

            //                if (nPixel < 0) nPixel = 0;
            //                if (nPixel > 255) nPixel = 255;
            //                p[4 + stride] = (byte) nPixel;

            //                nPixel = (int) Math.Round(((((pSrc[0]*m.TopLeft) +
            //                                             (pSrc[3]*m.TopMid) +
            //                                             (pSrc[6]*m.TopRight) +
            //                                             (pSrc[0 + stride]*m.MidLeft) +
            //                                             (pSrc[3 + stride]*m.Pixel) +
            //                                             (pSrc[6 + stride]*m.MidRight) +
            //                                             (pSrc[0 + stride2]*m.BottomLeft) +
            //                                             (pSrc[3 + stride2]*m.BottomMid) +
            //                                             (pSrc[6 + stride2]*m.BottomRight))
            //                                            /m.Factor) + m.Offset));

            //                if (nPixel < 0) nPixel = 0;
            //                if (nPixel > 255) nPixel = 255;
            //                p[3 + stride] = (byte) nPixel;

            //                p += 3;
            //                pSrc += 3;
            //            }

            //            p += nOffset;
            //            pSrc += nOffset;
            //        }
            //    }

            //    b.UnlockBits(bmData);
            //    bSrc.UnlockBits(bmSrc);
            //}

            return true;
        }

        public static uint[,] Conv3x3(uint[,] data, ConvMatrix m)
        {
            // Avoid divide by zero errors
            if (0 == m.Factor)
                return data;

            uint[,] result = new uint[data.GetLength(0), data.GetLength(1)];

            int nWidth = data.GetLength(0) - 2;
            int nHeight = data.GetLength(1) - 2;

            int nPixel;

            for (int y = 0; y < nHeight; ++y)
            {
                for (int x = 0; x < nWidth; ++x)
                {
                    nPixel = (int)Math.Round((((data[x, y] * m.TopLeft) +
                                                (data[x + 1, y] * m.TopMid) +
                                                (data[x + 2, y] * m.TopRight) +
                                                (data[x, y + 1] * m.MidLeft) +
                                                (data[x + 1, y + 1] * m.Pixel) +
                                                (data[x + 2, y + 1] * m.MidRight) +
                                                (data[x, y + 2] * m.BottomLeft) +
                                                (data[x + 1, y + 2] * m.BottomMid) +
                                                (data[x + 2, y + 2] * m.BottomRight))
                                               / m.Factor) + m.Offset);

                    if (nPixel < 0) nPixel = 0;
                    if (nPixel > 255) nPixel = 255;
                    result[x + 1, y + 1] = (uint)nPixel;
                }
            }

            return result;
        }


        public static bool Conv5x5(Pixelmap image, ConvMatrix5x5 m)
        {
            throw new NotImplementedException();
        }
    }
}
