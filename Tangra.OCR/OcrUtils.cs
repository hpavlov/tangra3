using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.Model.Helpers;
using Tangra.Model.Image;
using Tangra.OCR.TimeExtraction;

namespace Tangra.OCR
{
    public class OcrUtils
    {
        public static uint[] PreProcessImageOSDForOCR(uint[] data, int width, int height, int maxNoiseHeight, ref string error)
        {
            return PreProcessImageOSDForOCR(data, width, height, maxNoiseHeight, ref error, null);
        }

        public static uint[] PreProcessImageOSDForOCR(uint[] data, int width, int height, int maxNoiseHeight, ref string error, ICalibrationErrorProcessor logger)
        {
            uint[] preProcessedPixels = new uint[data.Length];
            Array.Copy(data, preProcessedPixels, data.Length);

            // Process the image
            uint median = preProcessedPixels.Median();
            for (int i = 0; i < preProcessedPixels.Length; i++)
            {
                int darkCorrectedValue = (int)preProcessedPixels[i] - (int)median;
                if (darkCorrectedValue < 0) darkCorrectedValue = 0;
                preProcessedPixels[i] = (uint)darkCorrectedValue;
            }

            if (median > 250)
            {
                error = "The background is too bright.";
                return null;
            }

            if (logger != null) logger.AddErrorImage("pp-median-corrected.bmp", preProcessedPixels.ToArray(), width, height);

            uint[] blurResult = BitmapFilter.GaussianBlur(preProcessedPixels, 8, width, height);

            if (logger != null) logger.AddErrorImage("pp-gauss-blur.bmp", blurResult.ToArray(), width, height);

            uint average = 128;
            uint[] sharpenResult = BitmapFilter.Sharpen(blurResult, 8, width, height, out average);

            if (logger != null) logger.AddErrorImage("pp-sharpened.bmp", sharpenResult.ToArray(), width, height);

            // Binerize and Inverse
            for (int i = 0; i < sharpenResult.Length; i++)
            {
                sharpenResult[i] = sharpenResult[i] > average ? (uint)0 : (uint)255;
            }
            if (logger != null) logger.AddErrorImage("pp-binerized.bmp", sharpenResult.ToArray(), width, height);

            uint[] denoised = BitmapFilter.Denoise(sharpenResult, 8, width, height, out average, false);

            if (logger != null) logger.AddErrorImage("pp-denoised.bmp", denoised.ToArray(), width, height);

            for (int i = 0; i < denoised.Length; i++)
            {
                preProcessedPixels[i] = denoised[i] < 127 ? (uint)0 : (uint)255;
            }

            LargeChunkDenoiser.RemoveSmallHeightNoise(preProcessedPixels, width, height, maxNoiseHeight);
            for (int i = 0; i < preProcessedPixels.Length; i++)
            {
                preProcessedPixels[i] = preProcessedPixels[i] < 127 ? (uint)255 : (uint)0;
            }
            if (logger != null) logger.AddErrorImage("pp-final.bmp", preProcessedPixels.ToArray(), width, height);

            return preProcessedPixels;
        }


        private static uint[] PrepareOsdVideoFields(int frameNo, uint[] data, int width, int height, int minFrameOsdDigitHeight, ref string error, ICalibrationErrorProcessor logger)
        {
            // TODO: This is suboptimal. Need to make the OCR work with the two separate fields from the start
            var fieldHeight = (int)Math.Ceiling(height / 2.0);
            uint[] oddFieldPixels = new uint[width * fieldHeight];
            uint[] evenFieldPixels = new uint[width * fieldHeight];
            for (int y = 0; y < height; y++)
            {
                bool isOddLine = y % 2 == 0; // y is zero-based so odd/even calculations should be performed counting from zero
                int lineNo = y / 2;
                if (isOddLine)
                    Array.Copy(data, y * width, oddFieldPixels, lineNo * width, width);
                else
                    Array.Copy(data, y * width, evenFieldPixels, lineNo * width, width);
            }

            var preProcessedOddPixels = OcrUtils.PreProcessImageOSDForOCR(oddFieldPixels, width, fieldHeight, minFrameOsdDigitHeight / 2, ref error);
            var preProcessedEvenPixels = OcrUtils.PreProcessImageOSDForOCR(evenFieldPixels, width, fieldHeight, minFrameOsdDigitHeight / 2, ref error);

            if (logger != null && logger.ShouldLogErrorImage)
            {
                logger.AddErrorImage(string.Format(@"{0}-even.bmp", frameNo.ToString("0000000")), preProcessedEvenPixels, width, fieldHeight);
                logger.AddErrorImage(string.Format(@"{0}-odd.bmp", frameNo.ToString("0000000")), preProcessedOddPixels, width, fieldHeight);

                logger.AddErrorImage(string.Format(@"ORG-{0}-even.bmp", frameNo.ToString("0000000")), evenFieldPixels, width, fieldHeight);
                logger.AddErrorImage(string.Format(@"ORG-{0}-odd.bmp", frameNo.ToString("0000000")), oddFieldPixels, width, fieldHeight);
            }

            var preProcessedPixels = new uint[width * height];
            for (int y = 0; y < height; y++)
            {
                bool isOddLine = (y - 1) % 2 == 1; // y is zero-based so odd/even calculations should be performed counting from zero
                int lineNo = y / 2;
                if (isOddLine)
                    Array.Copy(preProcessedOddPixels, lineNo * width, preProcessedPixels, y * width, width);
                else
                    Array.Copy(preProcessedEvenPixels, lineNo * width, preProcessedPixels, y * width, width);
            }

            return preProcessedPixels;
        }

        public static uint[] PreProcessImageOSDForOCR(int frameNo, uint[] data, int width, int height, double averageDetectedOsdHeight, int topMostLine, int bottomMostLine, ref string error, ICalibrationErrorProcessor logger)
        {
            // NOTE: Top and bottom most lines are known. Only pre-process the video frame lines with the VTI-OSD
            uint[] rv = new uint[data.Length];
            int top = Math.Max(0, topMostLine - 5);
            int bottom = Math.Max(height, bottomMostLine + 5);
            uint[] roi = new uint[width * (bottom - top)];
            int idx = 0;
            for (int i = top * width; i < bottom * width; i++, idx++)
            {
                roi[idx] = data[i];
            }
           
            idx = 0;
            var pproc = PrepareOsdVideoFields(frameNo, roi, width, bottom - top, (int)Math.Round(0.8 * averageDetectedOsdHeight), ref error, logger);
            for (int i = top * width; i < bottom * width; i++, idx++)
            {
                rv[i] = pproc[idx];
            }

            return rv;
        }
    }
}
