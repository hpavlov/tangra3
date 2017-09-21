/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        internal const string FITS_SEQUENCE_ENGINE = "FITS-SEQ";

        private List<string> m_FitsFilesList = new List<string>();

        private short m_MinPixelValueFirstImage;
        private uint m_MaxPixelValueFirstImage;
        private IFITSTimeStampReader m_TimeStampReader = null;

        public static FITSFileSequenceStream OpenFolder(string[] fitsFiles, IFITSTimeStampReader timeStampReader, int firstFrameNo)
        {
			UsageStats.Instance.ProcessedFitsFolderFiles++;
			UsageStats.Instance.Save();

            var rv = new FITSFileSequenceStream(fitsFiles, timeStampReader, firstFrameNo);
	        rv.FileName = Path.GetDirectoryName(fitsFiles[0]);
	        return rv;
        }

        private FITSFileSequenceStream(string[] fitsFiles, IFITSTimeStampReader timeStampReader, int firstFrame)
        {
            m_FitsFilesList.AddRange(fitsFiles);

            var firstNumeredFrameNo = GetFirstImageNumberOfConsistentlyNamedFiles();
            if (firstFrame == 0 && firstNumeredFrameNo.HasValue)
                firstFrame = firstNumeredFrameNo.Value;

            FirstFrame = firstFrame;
            LastFrame = m_FitsFilesList.Count - 1 + firstFrame;
            CountFrames = m_FitsFilesList.Count;
            m_TimeStampReader = timeStampReader;

            uint[] pixelsFlat;
            int width;
            int height;
            int bpp;
			DateTime? timestamp;
			double? exposure;
            short minPixelValue;
            uint maxPixelValue;
            bool hasNegativePixels;

            FITSHelper.Load16BitFitsFile(m_FitsFilesList[0], null, m_TimeStampReader, null, out pixelsFlat, out width, out height, out bpp, out timestamp, out exposure, out minPixelValue, out maxPixelValue, out hasNegativePixels);

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
                return Path.GetFileName(m_FitsFilesList[index - FirstFrame]);

            return null;
        }

        private static Regex NUMBERED_FITS_FILE = new Regex(@"(\d+)$", RegexOptions.Singleline | RegexOptions.Compiled);

        private int? GetFirstImageNumberOfConsistentlyNamedFiles()
        {
            int? firstFrameNo = null;
            int prevFrameNo = -1;
            foreach (string fileName in m_FitsFilesList)
            {
                var fileNameOnly = Path.GetFileNameWithoutExtension(fileName);
                var match = NUMBERED_FITS_FILE.Match(fileNameOnly);
                if (!match.Success)
                    return null;
                int thisFrameNo = int.Parse(match.Groups[1].Value);
                if (firstFrameNo == null)
                {
                    firstFrameNo = thisFrameNo;
                }
                else if (prevFrameNo + 1 != thisFrameNo)
                    return null;

                prevFrameNo = thisFrameNo;
            }

            return firstFrameNo;
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
            short minPixelValue;
            uint maxPixelValue;
            bool hasNegativePixels;

            var cards = new Dictionary<string, string>();
            BasicHDU fitsImage = null;

            FITSHelper.Load16BitFitsFile(m_FitsFilesList[index - FirstFrame], null, m_TimeStampReader,
                (hdu) =>
                {
                    fitsImage = hdu;
                    var cursor = hdu.Header.GetCursor();
                    while (cursor.MoveNext())
                    {
                        HeaderCard card = hdu.Header.FindCard((string)cursor.Key);
                        if (card != null && !string.IsNullOrWhiteSpace(card.Key) && card.Key != "END")
                        {
                            if (cards.ContainsKey(card.Key))
                                cards[card.Key] += "\r\n" + card.Value;
                            else
                                cards.Add(card.Key, card.Value);
                        }                        
                    }
                },
                out pixelsFlat, out width, out height, out bpp, out timestamp, out exposure, out minPixelValue, out maxPixelValue, out hasNegativePixels);

            return FitsStreamHelper.BuildFitsPixelmap(Width, Height, pixelsFlat, BitPix, HasUTCTimeStamps, exposure, timestamp, fitsImage, cards);
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
            get { return FITS_SEQUENCE_ENGINE; }
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
            get { return m_MinPixelValueFirstImage > 0 ? (uint)m_MinPixelValueFirstImage : 0; }
        }

        public uint MaxPixelValue
        {
            get { return m_MaxPixelValueFirstImage; }
        }
    }
}
