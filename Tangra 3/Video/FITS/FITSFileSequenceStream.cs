/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using nom.tam.fits;
using Tangra.Helpers;
using Tangra.Model.Config;
using Tangra.Model.Helpers;
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
        private bool m_ZeroOutNegativePixels = false;
        private IFITSTimeStampReader m_TimeStampReader = null;

        public static FITSFileSequenceStream OpenFolder(string[] fitsFiles, IFITSTimeStampReader timeStampReader, bool zeroOutNegativeValues, int firstFrameNo, out bool hasNegativePixels)
        {
			UsageStats.Instance.ProcessedFitsFolderFiles++;
			UsageStats.Instance.Save();

            var rv = new FITSFileSequenceStream(fitsFiles, timeStampReader, zeroOutNegativeValues, firstFrameNo, out hasNegativePixels);
	        rv.FileName = Path.GetDirectoryName(fitsFiles[0]);
	        return rv;
        }

        private FITSFileSequenceStream(string[] fitsFiles, IFITSTimeStampReader timeStampReader, bool zeroOutNegativeValues, int firstFrameNo, out bool hasNegativePixels)
        {
            m_FitsFilesList.AddRange(fitsFiles);

            FirstFrame = firstFrameNo;
            LastFrame = m_FitsFilesList.Count - 1;
            CountFrames = m_FitsFilesList.Count;
            m_ZeroOutNegativePixels = zeroOutNegativeValues;
            m_TimeStampReader = timeStampReader;

            uint[] pixelsFlat;
            int width;
            int height;
            int bpp;
			DateTime? timestamp;
			double? exposure;
            uint minPixelValue;
            uint maxPixelValue;

            FITSHelper.Load16BitFitsFile(m_FitsFilesList[0], m_TimeStampReader, zeroOutNegativeValues, null, out pixelsFlat, out width, out height, out bpp, out timestamp, out exposure, out minPixelValue, out maxPixelValue, out hasNegativePixels);

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
            bool hasNegativePixels;

            var cards = new Dictionary<string, string>();

            FITSHelper.Load16BitFitsFile(m_FitsFilesList[index], m_TimeStampReader, m_ZeroOutNegativePixels,
                (hdu) =>
                {
                    var cursor = hdu.Header.GetCursor();
                    while (cursor.MoveNext())
                    {
                        HeaderCard card = hdu.Header.FindCard((string)cursor.Key);
                        if (card != null && !string.IsNullOrWhiteSpace(card.Key) && card.Key != "END")
                            cards.Add(card.Key, card.Value);                        
                    }
                },
                out pixelsFlat, out width, out height, out bpp, out timestamp, out exposure, out minPixelValue, out maxPixelValue, out hasNegativePixels);

            byte[] displayBitmapBytes = new byte[Width * Height];
            byte[] rawBitmapBytes = new byte[(Width * Height * 3) + 40 + 14 + 1];

            uint[] flatPixelsCopy = new uint[pixelsFlat.Length];
            Array.Copy(pixelsFlat, flatPixelsCopy, pixelsFlat.Length);

            TangraCore.PreProcessors.ApplyPreProcessingPixelsOnly(pixelsFlat, flatPixelsCopy, Width, Height, BitPix, 0 /* No normal value for FITS files */, exposure.HasValue ? (float)exposure.Value : 0);

            TangraCore.GetBitmapPixels(Width, Height, flatPixelsCopy, rawBitmapBytes, displayBitmapBytes, true, (ushort)BitPix, 0);

            Bitmap displayBitmap = Pixelmap.ConstructBitmapFromBitmapPixels(displayBitmapBytes, Width, Height);

            Pixelmap rv = new Pixelmap(Width, Height, BitPix, flatPixelsCopy, displayBitmap, displayBitmapBytes);
            rv.UnprocessedPixels = pixelsFlat;
			if (HasUTCTimeStamps)
			{
			    rv.FrameState.CentralExposureTime = timestamp.HasValue ? timestamp.Value : DateTime.MinValue;
				rv.FrameState.ExposureInMilliseconds = exposure.HasValue ? (float)(exposure.Value * 1000.0) : 0;
			}

            rv.FrameState.AdditionalProperties = new SafeDictionary<string, object>();
            foreach (string key in cards.Keys)
                rv.FrameState.AdditionalProperties.Add(key, cards[key]);

            return rv;
        }

        public int RecommendedBufferSize
        {
            get { return Math.Min(8, CountFrames); }
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
