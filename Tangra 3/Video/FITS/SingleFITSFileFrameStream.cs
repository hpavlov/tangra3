/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using nom.tam.fits;
using Tangra.Controller;
using Tangra.Helpers;
using Tangra.Model.Config;
using Tangra.Model.Context;
using Tangra.Model.Helpers;
using Tangra.Model.Image;
using Tangra.Model.Video;
using Tangra.Model.VideoOperations;
using Tangra.PInvoke;
using Tangra.VideoOperations.LightCurves;

namespace Tangra.Video.FITS
{
    internal class SingleFITSFileFrameStream : IDisposable, IFrameStream, IFITSStream
	{
		internal const string SINGLE_FITS_FILE_ENGINE = "FITS.Image";

		private int m_FirstFrame;
		private int m_LastFrame;
		private int m_NumFrames;

		private int m_Width;
		private int m_Height;
		private int m_Bpp;
        private float m_Exposure;
		private uint[] m_FlatPixels;

        private short m_MinPixelValue;
        private uint m_MaxPixelValue;

        private Dictionary<string, string> m_Cards = new Dictionary<string, string>();

        public static SingleFITSFileFrameStream OpenFile(string fileName, out bool hasNegativePixels)
		{
            var fitsData = FITSHelper2.LoadFitsFile(fileName, null, 0);

		    TangraContext.Current.RenderingEngine = SINGLE_FITS_FILE_ENGINE;

            hasNegativePixels = fitsData.PixelStats.HasNegativePixels;

            return new SingleFITSFileFrameStream(fitsData.PixelsFlat, fitsData.Width, fitsData.Height, fitsData.PixelStats.BitPix, fitsData.Exposure, fitsData.PixelStats.MinPixelValue, fitsData.PixelStats.MaxPixelValue, fitsData.Cards);
		}

		private SingleFITSFileFrameStream(uint[] flatPixels, int width, int height, int bpp, double? exposure, short minPixelValue, uint maxPixelValue, IDictionary<string, string> cards)
		{
			m_FlatPixels = flatPixels;
			m_Width = width;
			m_Height = height;
			m_Bpp = bpp;
		    m_Exposure = exposure.HasValue ? (float)exposure.Value : 0;

		    HasUTCTimeStamps = false; // TODO: Add support for timestamps for single FITS files

			m_FirstFrame = 0;
			m_LastFrame = 0;
			m_NumFrames = 1;

		    m_MinPixelValue = minPixelValue;
		    m_MaxPixelValue = maxPixelValue;

            if (cards != null)
                foreach (var kvp in cards) m_Cards.Add(kvp.Key, kvp.Value);
		}

		#region IFrameStream Members

		public int Width
		{
			get { return m_Width; }
		}

		public int Height
		{
			get { return m_Height; }
		}

		public int BitPix
		{
			get { return m_Bpp; }
		}

		public uint GetAav16NormVal()
		{
			return 0;
		}

		public int FirstFrame
		{
			get { return m_FirstFrame; }
		}

		public int LastFrame
		{
			get { return m_LastFrame; }
		}

		public int CountFrames
		{
			get { return m_NumFrames; }
		}

		public double FrameRate
		{
			get { return 1; }
		}

		public double MillisecondsPerFrame
		{
			get { return 1000; }
		}

		public void EnsureFrameOpen()
		{ }

		public void EnsureFrameClose()
		{ }

        public string GetFrameFileName(int index)
        {
            throw new NotSupportedException();
        }

        public bool SupportsFrameFileNames
        {
            get { return false; }
        }

		public Pixelmap GetPixelmap(int index)
		{
			byte[] displayBitmapBytes = new byte[m_Width * m_Height];
			byte[] rawBitmapBytes = new byte[(m_Width * m_Height * 3) + 40 + 14 + 1];

			uint[] flatPixelsCopy = new uint[m_FlatPixels.Length];
			Array.Copy(m_FlatPixels, flatPixelsCopy, m_FlatPixels.Length);

            TangraCore.PreProcessors.ApplyPreProcessingPixelsOnly(m_FlatPixels, flatPixelsCopy, m_Width, m_Height, m_Bpp, 0 /* No normal value for FITS files */, m_Exposure);

            TangraCore.GetBitmapPixels(m_Width, m_Height, flatPixelsCopy, rawBitmapBytes, displayBitmapBytes, true, (ushort)m_Bpp, 0);

			Bitmap displayBitmap = Pixelmap.ConstructBitmapFromBitmapPixels(displayBitmapBytes, m_Width, m_Height);

			Pixelmap rv = new Pixelmap(m_Width, m_Height, m_Bpp, flatPixelsCopy, displayBitmap, displayBitmapBytes);
		    rv.UnprocessedPixels = m_FlatPixels;

		    if (m_Cards != null && m_Cards.Count > 0)
		    {
                rv.FrameState.AdditionalProperties = new SafeDictionary<string, object>();
                foreach (string key in m_Cards.Keys)
                    rv.FrameState.AdditionalProperties.Add(key, m_Cards[key]);		        
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

		public string VideoFileType
		{
			get { return "FITS Image"; }
		}

		public string Engine { get { return SINGLE_FITS_FILE_ENGINE; } }

		public bool IsNativeStream { get { return false; } }

		public Pixelmap GetIntegratedFrame(int startFrameNo, int framesToIntegrate, bool isSlidingIntegration, bool isMedianAveraging)
		{
			throw new NotSupportedException();
		}

		public string FileName
		{
			get { return null; }
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{ }

		#endregion



        public uint MinPixelValue
        {
            get { return m_MinPixelValue > 0 ? (uint)m_MinPixelValue : 0; }
        }

        public uint MaxPixelValue
        {
            get { return m_MaxPixelValue; }
        }

        public bool HasUTCTimeStamps { get; private set; }
    }
}
