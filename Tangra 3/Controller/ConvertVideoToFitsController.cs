﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using nom.tam.fits;
using nom.tam.util;
using Tangra.Config;
using Tangra.Helpers;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Model.Context;
using Tangra.Model.Video;
using Tangra.VideoOperations.LightCurves;

namespace Tangra.Controller
{
    public class ConvertVideoToFitsController
    {
		private Form m_MainFormView;
		private VideoController m_VideoController;
        private Rectangle m_RegionOfInterest;

        private int m_FrameWidth;
        private int m_FrameHeight;
        private bool m_UsesROI;
        private string m_FolderName;
        private bool m_FitsCube;

        private string m_DateObsComment;
        private string m_Note;
        private bool m_IsFitsSequence;

        private bool m_ExportAs8BitFloat;
        private uint m_NormalValue;

        private string m_VideoCamera;
        private string m_NativeFormat;
        private int m_IntegrationRate;
        private VideoFileFormat m_VideoFormat;

        private Dictionary<string, Tuple<string, string>> m_AdditionalFileHeaders = new Dictionary<string, Tuple<string, string>>();

        public ConvertVideoToFitsController(Form mainFormView, VideoController videoController)
		{
			m_MainFormView = mainFormView;
			m_VideoController = videoController;
		}

        internal void StartExport(string fileName, bool fitsCube, Rectangle roi, UsedTimeBase timeBase, bool usesOCR, bool ocrHasDatePart)
        {
            m_FrameWidth = TangraContext.Current.FrameWidth;
            m_FrameHeight = TangraContext.Current.FrameHeight;
            m_IsFitsSequence = m_VideoController.IsFitsSequence;

            if (m_VideoController.IsAstroAnalogueVideo)
                m_IntegrationRate = m_VideoController.AstroAnalogueVideoIntegratedAAVFrames;
            else
                m_IntegrationRate = 0;

            m_NativeFormat = m_VideoController.GetVideoFormat(m_VideoController.GetVideoFileFormat());
            m_VideoCamera = m_VideoController.AstroVideoCameraModel;

            m_RegionOfInterest = roi;
            m_UsesROI = roi.Width != m_FrameWidth || roi.Height != m_FrameHeight;

            m_ExportAs8BitFloat = false;
            m_NormalValue = m_VideoController.EffectiveMaxPixelValue;
            m_VideoFormat = m_VideoController.GetVideoFileFormat();

            if (m_VideoFormat == VideoFileFormat.AAV || m_VideoFormat == VideoFileFormat.AAV2 || m_VideoFormat == VideoFileFormat.AVI)
            {
                if (m_VideoController.VideoBitPix == 8 || m_VideoController.EffectiveMaxPixelValue > 255)
                {
                    // For video from analogue cameras we export as 8-bit floating point numbers
                    m_ExportAs8BitFloat = true;
                }
            }

            m_AdditionalFileHeaders = m_VideoController.GetAdditionalFileHeaders();

            m_FitsCube = fitsCube;
            if (!fitsCube)
            {
                m_FolderName = fileName;

                if (!Directory.Exists(m_FolderName)) 
                    Directory.CreateDirectory(m_FolderName);
            }

            m_Note = string.Format("Converted from {0} file.", m_VideoController.GetVideoFileFormat());
            if (m_Note.Length > HeaderCard.MAX_VALUE_LENGTH) m_Note = m_Note.Substring(0, HeaderCard.MAX_VALUE_LENGTH);

            if (m_UsesROI)
            {
                m_Note += string.Format(" Selected ROI: ({0},{1},{2},{3})", roi.Left, roi.Top, roi.Right, roi.Bottom);
                if (m_Note.Length > HeaderCard.MAX_VALUE_LENGTH) m_Note = m_Note.Substring(0, HeaderCard.MAX_VALUE_LENGTH);                
            }

            if (timeBase == UsedTimeBase.UserEnterred)
            {
                m_DateObsComment = "Date and Time are user entered & computed";
            }
            else if (timeBase == UsedTimeBase.EmbeddedTimeStamp)
            {
                m_DateObsComment = m_VideoController.GetEmbeddedDateTimeComment();
            }
        }

        internal void ProcessFrame(int frameNo, AstroImage astroImage, DateTime timeStamp, float exposureSeconds)
        {
            string fileName = Path.GetFullPath(string.Format("{0}\\{1}_{2}.fits", m_FolderName, frameNo.ToString().PadLeft(5, '0'), timeStamp.ToString("yyyy-MMM-dd_HHmmss_fff")));
            
            if (m_IsFitsSequence &&
                astroImage.Pixelmap.FrameState.Tag != null &&
                astroImage.Pixelmap.FrameState.Tag is BasicHDU)
            {
                ProcessFitsFrame(fileName, astroImage.Pixelmap.FrameState.Tag as BasicHDU);
            }
            else
            {
                uint[] pixels = astroImage.Pixelmap.UnprocessedPixels ?? astroImage.Pixelmap.Pixels;
                ProcessFrame(frameNo, fileName, pixels, timeStamp, exposureSeconds);
            }
        }

        internal void ProcessFrame(int frameNo, string fileName, uint[] pixels, DateTime timeStamp, float exposureSeconds)
        {
            if (m_UsesROI)
            {
                uint[] subpixels = new uint[m_RegionOfInterest.Width * m_RegionOfInterest.Height];

                for (int y = 0; y < m_RegionOfInterest.Height; y++)
                {
                    for (int x = 0; x < m_RegionOfInterest.Width; x++)
                    {
                        subpixels[y * m_RegionOfInterest.Width + x] = pixels[(m_RegionOfInterest.Top + y) * m_FrameWidth + m_RegionOfInterest.Left + x];
                    }
                }

                SaveFitsFrame(frameNo, fileName, m_RegionOfInterest.Width, m_RegionOfInterest.Height, subpixels, timeStamp, exposureSeconds);
            }
            else
            {
                SaveFitsFrame(frameNo, fileName, m_RegionOfInterest.Width, m_RegionOfInterest.Height, pixels, timeStamp, exposureSeconds);
            } 
        }

        private TVal[][] GetFitsDataRegion<TVal>(Array dataArray)
        {
            TVal[][] subpixels = new TVal[m_RegionOfInterest.Height][];

            for (int y = 0; y < m_RegionOfInterest.Height; y++)
            {
                object dataRowObject = dataArray.GetValue(m_FrameHeight - m_RegionOfInterest.Top - y);

                var dataRow = (TVal[])dataRowObject;
                subpixels[m_RegionOfInterest.Height - y - 1] = new TVal[m_RegionOfInterest.Width];

                for (int x = 0; x < m_RegionOfInterest.Width; x++)
                {
                    subpixels[m_RegionOfInterest.Height - y - 1][x] = dataRow[m_RegionOfInterest.Left + x];
                }
            }

            return subpixels;
        }

        internal void ProcessFitsFrame(string fileName, BasicHDU fitsImage)
        {
            object data = null;
            if (m_UsesROI)
            {
                Array dataArray = (Array) fitsImage.Data.DataArray;

                object entry = dataArray.GetValue(0);
                if (entry is byte[])
                {
                    data = GetFitsDataRegion<byte>(dataArray);
                }
                else if (entry is short[])
                {
                    data = GetFitsDataRegion<short>(dataArray);
                }
                else if (entry is int[])
                {
                    data = GetFitsDataRegion<int>(dataArray);
                }
                else if (entry is float[])
                {
                    data = GetFitsDataRegion<float>(dataArray);
                }
                else if (entry is double[])
                {
                    data = GetFitsDataRegion<double>(dataArray);
                }
            }
            else
                data = fitsImage.Data.DataArray;

            SaveFitsFrame(fileName, fitsImage.Header, m_RegionOfInterest.Width, m_RegionOfInterest.Height, data);
        }

        private static int[][] SaveImageData(int width, int height, uint[] data)
        {
            int[][] bimg = new int[height][];

            for (int y = 0; y < height; y++)
            {
                bimg[y] = new int[width];

                for (int x = 0; x < width; x++)
                {
                    bimg[y][x] = (int)Math.Max(0, data[x + (height - y - 1) * width]);
                }
            }

            return bimg;
        }

        private static float[][] SaveNormalizedFloatImageData(int width, int height, uint[] data, uint normalValue)
        {
            float[][] bimg = new float[height][];

            for (int y = 0; y < height; y++)
            {
                bimg[y] = new float[width];

                for (int x = 0; x < width; x++)
                {
                    if (normalValue > 255)
                        bimg[y][x] = (float)Math.Min(255, Math.Max(0, data[x + (height - y - 1) * width] * 255.0 / normalValue));
                    else
                        bimg[y][x] = (float)Math.Min(255, Math.Max(0, data[x + (height - y - 1) * width]));
                }
            }

            return bimg;
        }

        internal void SaveFitsFrame(string fileName, Header header, int width, int height, object data)
        {
            Fits f = new Fits();

            BasicHDU imageHDU = Fits.MakeHDU(data);

            nom.tam.fits.Header hdr = imageHDU.Header;
            hdr.AddValue("SIMPLE", "T", null);

            hdr.AddValue("BZERO", 0, null);
            hdr.AddValue("BSCALE", 1, null);

            hdr.AddValue("NAXIS", 2, null);
            hdr.AddValue("NAXIS1", width, null);
            hdr.AddValue("NAXIS2", height, null);

            string[] RESERVED_KEYS = new string[] { "SIMPLE", "NAXIS", "NAXIS1", "NAXIS2", "BZERO", "BSCALE", "END" };

            var cursor = header.GetCursor();
            while (cursor.MoveNext())
            {
                HeaderCard card = header.FindCard((string)cursor.Key);
                if (card != null && !string.IsNullOrWhiteSpace(card.Key) && !RESERVED_KEYS.Contains(card.Key))
                {
                    hdr.AddValue(card.Key, card.Value, card.Comment);
                }
            }

            hdr.AddValue("NOTES", m_Note, null);
            hdr.AddValue("TANGRAVE", string.Format("{0} v{1}", VersionHelper.AssemblyProduct, VersionHelper.AssemblyFileVersion), "Tangra version");
            hdr.AddValue("END", null, null);

            f.AddHDU(imageHDU);

            // Write a FITS file.
            using (BufferedFile bf = new BufferedFile(fileName, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                f.Write(bf);
                bf.Flush();
            }
        }

        internal void SaveFitsFrame(int frameNo, string fileName, int width, int height, uint[] framePixels, DateTime timeStamp, float exposureSeconds)
        {
            SaveFitsFrame(frameNo, fileName, width, height, framePixels, timeStamp, exposureSeconds, m_DateObsComment,
                m_ExportAs8BitFloat, 
                m_NormalValue,
                m_VideoFormat,
                m_NativeFormat,
                m_IntegrationRate,
                m_VideoCamera,
                m_VideoController.FileName,
                m_AdditionalFileHeaders);
        }

        internal static void SaveFitsFrame(
            int frameNo, 
            string fileName, 
            int width, 
            int height, 
            uint[] framePixels, 
            DateTime timeStamp, 
            float exposureSeconds, 
            string timeStampComment,
            bool exportAs8BitFloat,
            uint normalValue,
            VideoFileFormat videoFileFormat,
            string nativeFormat,
            int integrationRate,
            string videoCamera,
            string originalFileName,
            Dictionary<string, Tuple<string, string>> additionalFileHeaders)
        {
            Fits f = new Fits();

            object data = exportAs8BitFloat
                ? (object)SaveNormalizedFloatImageData(width, height, framePixels, normalValue) 
                : (object)SaveImageData(width, height, framePixels);

            BasicHDU imageHDU = Fits.MakeHDU(data);

            nom.tam.fits.Header hdr = imageHDU.Header;
            hdr.AddValue("SIMPLE", "T", null);

            hdr.AddValue("BITPIX", exportAs8BitFloat ? -32 : 32, null);

            hdr.AddValue("BZERO", 0, null);
            hdr.AddValue("BSCALE", 1, null);

            hdr.AddValue("NAXIS", 2, null);
            hdr.AddValue("NAXIS1", width, null);
            hdr.AddValue("NAXIS2", height, null);

            hdr.AddValue("EXPOSURE", exposureSeconds.ToString("0.000", CultureInfo.InvariantCulture), "Exposure, seconds");

            hdr.AddValue("DATE-OBS", timeStamp.ToString("yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture), timeStampComment);

            hdr.AddValue("CAMERA", videoCamera, "Video camera model (observer specified)");

            hdr.AddValue("FILENAME", originalFileName, null);
            hdr.AddValue("FRAMENO", frameNo, null);

            if (videoFileFormat == VideoFileFormat.AAV || videoFileFormat == VideoFileFormat.AAV2)
            {
                hdr.AddValue("NOTES", "No instrumental delay has been applied to DATE-OBS.", null);

                if (exportAs8BitFloat)
                {
                    hdr.AddValue("VIDEOFMT", nativeFormat, "Native analogue video format");
                    if (integrationRate > 0)
                    {
                        hdr.AddValue("INTGRRTE", integrationRate.ToString(), "Integration rate in video frames");
                    }
                }
            }
            else
            {
                hdr.AddValue("NOTES", string.Format("Converted from {0} file.", videoFileFormat), null);
            }

            foreach (var kvp in additionalFileHeaders)
            {
                hdr.AddValue(kvp.Key, kvp.Value.Item1, kvp.Value.Item2);
            }

            hdr.AddValue("TANGRAVE", string.Format("{0} v{1}", VersionHelper.AssemblyProduct, VersionHelper.AssemblyFileVersion), "Tangra version");
            hdr.AddValue("END", null, null);

            f.AddHDU(imageHDU);

            // Write a FITS file.
            using (BufferedFile bf = new BufferedFile(fileName, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                f.Write(bf);
                bf.Flush();
            }
        }

        internal void FinishExport()
        {
            
        }
    }
}
