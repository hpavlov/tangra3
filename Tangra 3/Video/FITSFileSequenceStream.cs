﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Tangra.Helpers;
using Tangra.Model.Image;
using Tangra.Model.Video;
using Tangra.PInvoke;

namespace Tangra.Video
{
    internal class FITSFileSequenceStream : IDisposable, IFrameStream
    {
        private List<string> m_FitsFilesList = new List<string>();
 
        public static FITSFileSequenceStream OpenFolder(string[] fitsFiles)
        {
            var rv =  new FITSFileSequenceStream(fitsFiles);
	        rv.FileName = Path.GetDirectoryName(fitsFiles[0]);
	        return rv;
        }

        private FITSFileSequenceStream(string[] fitsFiles)
        {
            m_FitsFilesList.AddRange(fitsFiles);
            m_FitsFilesList.Sort();

            FirstFrame = 0;
            LastFrame = m_FitsFilesList.Count - 1;
            CountFrames = m_FitsFilesList.Count;

            uint[] pixelsFlat;
            int width;
            int height;
            int bpp;
			DateTime? timestamp;
			double? exposure;

			FITSHelper.Load16BitFitsFile(m_FitsFilesList[0], out pixelsFlat, out width, out height, out bpp, out timestamp, out exposure);

            Width = width;
            Height = height;
            BitPix = bpp;

	        HasUTCTimeStamps = timestamp.HasValue;

            VideoFileType = string.Format("FITS.{0}::SEQ", bpp);
        }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public int BitPix { get; private set; }

        public int FirstFrame { get; private set; }

        public int LastFrame { get; private set; }

        public int CountFrames { get; private set; }

        public double FrameRate
        {
            get { return double.NaN; }
        }

        public double MillisecondsPerFrame
        {
            get { return 0; }
        }

        public Pixelmap GetPixelmap(int index)
        {
            uint[] pixelsFlat;
            int width;
            int height;
            int bpp;
			DateTime? timestamp;
			double? exposure;

			FITSHelper.Load16BitFitsFile(m_FitsFilesList[index], out pixelsFlat, out width, out height, out bpp, out timestamp, out exposure);

            byte[] displayBitmapBytes = new byte[Width * Height];
            byte[] rawBitmapBytes = new byte[(Width * Height * 3) + 40 + 14 + 1];

            uint[] flatPixelsCopy = new uint[pixelsFlat.Length];
            Array.Copy(pixelsFlat, flatPixelsCopy, pixelsFlat.Length);

            TangraCore.PreProcessors.ApplyPreProcessingPixelsOnly(flatPixelsCopy, Width, Height, BitPix);

            TangraCore.GetBitmapPixels(Width, Height, flatPixelsCopy, rawBitmapBytes, displayBitmapBytes, true, BitPix, 0);

            Bitmap displayBitmap = Pixelmap.ConstructBitmapFromBitmapPixels(displayBitmapBytes, Width, Height);

            Pixelmap rv = new Pixelmap(Width, Height, BitPix, flatPixelsCopy, displayBitmap, displayBitmapBytes);

			if (HasUTCTimeStamps)
			{
				rv.FrameState = new FrameStateData()
				{
					CentralExposureTime = timestamp.HasValue ? timestamp.Value : DateTime.MinValue,
					ExposureInMilliseconds = exposure.HasValue ? (float)(exposure.Value * 1000.0) : 0
				};
			}

            return rv;
        }

        public int RecommendedBufferSize
        {
            get { return 1; }
        }

        public bool SupportsSoftwareIntegration
        {
            get { return false; }
        }

        public string VideoFileType { get; private set; }

        public Pixelmap GetIntegratedFrame(int startFrameNo, int framesToIntegrate, bool isSlidingIntegration, bool isMedianAveraging)
        {
            throw new NotImplementedException();
        }

        public string Engine
        {
            get { return "FITS-SEQ"; }
        }

        public string FileName { get; private set; }

        public uint GetAav16NormVal()
        {
            return 0;
        }

        public void Dispose()
        {
            
        }

		public bool HasUTCTimeStamps { get; private set; }
    }
}