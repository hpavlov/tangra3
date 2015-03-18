/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Tangra.Helpers;
using Tangra.Model.Config;
using Tangra.Model.Image;
using Tangra.Model.Video;
using Tangra.PInvoke;

namespace Tangra.Video.FITS
{
    internal class FITSFileSequenceStream : IDisposable, IFrameStream, IFITSStream
    {
        private List<string> m_FitsFilesList = new List<string>();

        private uint m_MinPixelValueFirstImage;
        private uint m_MaxPixelValueFirstImage;

        public static FITSFileSequenceStream OpenFolder(string[] fitsFiles)
        {
			UsageStats.Instance.ProcessedFitsFolderFiles++;
			UsageStats.Instance.Save();

            var rv =  new FITSFileSequenceStream(fitsFiles);
	        rv.FileName = Path.GetDirectoryName(fitsFiles[0]);
	        return rv;
        }

        private FITSFileSequenceStream(string[] fitsFiles)
        {
            m_FitsFilesList.AddRange(fitsFiles);

            FirstFrame = 0;
            LastFrame = m_FitsFilesList.Count - 1;
            CountFrames = m_FitsFilesList.Count;

            uint[] pixelsFlat;
            int width;
            int height;
            int bpp;
			DateTime? timestamp;
			double? exposure;
            uint minPixelValue;
            uint maxPixelValue;

            FITSHelper.Load16BitFitsFile(m_FitsFilesList[0], out pixelsFlat, out width, out height, out bpp, out timestamp, out exposure, out minPixelValue, out maxPixelValue);

            m_MinPixelValueFirstImage = minPixelValue;
            m_MaxPixelValueFirstImage = maxPixelValue;

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

        public string GetFrameFileName(int index)
        {
            if (index >= 0 && index < m_FitsFilesList.Count)
                return Path.GetFileName(m_FitsFilesList[index]);

            return null;
        }

        public bool SupportsFrameFileNames
        {
            get { return true; }
        }

        public Pixelmap GetPixelmap(int index)
        {
            uint[] pixelsFlat;
            int width;
            int height;
            int bpp;
			DateTime? timestamp;
			double? exposure;
            uint minPixelValue;
            uint maxPixelValue;

            FITSHelper.Load16BitFitsFile(m_FitsFilesList[index], out pixelsFlat, out width, out height, out bpp, out timestamp, out exposure, out minPixelValue, out maxPixelValue);

            byte[] displayBitmapBytes = new byte[Width * Height];
            byte[] rawBitmapBytes = new byte[(Width * Height * 3) + 40 + 14 + 1];

            uint[] flatPixelsCopy = new uint[pixelsFlat.Length];
            Array.Copy(pixelsFlat, flatPixelsCopy, pixelsFlat.Length);

            TangraCore.PreProcessors.ApplyPreProcessingPixelsOnly(flatPixelsCopy, Width, Height, BitPix, 0 /* No normal value for FITS files */, exposure.HasValue ? (float)exposure.Value : 0);

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

        public uint MinPixelValue
        {
            get { return m_MinPixelValueFirstImage; }
        }

        public uint MaxPixelValue
        {
            get { return m_MaxPixelValueFirstImage; }
        }
    }
}
