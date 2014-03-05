using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Tangra.Helpers;
using Tangra.Model.Config;
using Tangra.Model.Context;
using Tangra.Model.Image;
using Tangra.Model.Video;
using Tangra.PInvoke;
using Tangra.VideoOperations.LightCurves;

namespace Tangra.Video
{
	internal class SingleFITSFileFrameStream : IDisposable, IFrameStream
	{
		internal const string SINGLE_FITS_FILE_ENGINE = "FITS.Image";

		private int m_FirstFrame;
		private int m_LastFrame;
		private int m_NumFrames;

		private int m_Width;
		private int m_Height;
		private int m_Bpp;
		private uint[] m_FlatPixels;

		public static SingleFITSFileFrameStream OpenFile(string fileName)
		{
			uint[] pixelsFlat;
			int width;
			int height;
			int bpp;

			FITSHelper.Load16BitFitsFile(fileName, out pixelsFlat, out width, out height, out bpp);

			TangraContext.Current.RenderingEngine = SINGLE_FITS_FILE_ENGINE;

			return new SingleFITSFileFrameStream(pixelsFlat, width, height, bpp);
		}

		private SingleFITSFileFrameStream(uint[] flatPixels, int width, int height, int bpp)
		{
			m_FlatPixels = flatPixels;
			m_Width = width;
			m_Height = height;
			m_Bpp = bpp;

			m_FirstFrame = 0;
			m_LastFrame = 0;
			m_NumFrames = 1;
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
			return 1;
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

		public Pixelmap GetPixelmap(int index)
		{
			byte[] displayBitmapBytes = new byte[m_Width * m_Height];
			byte[] rawBitmapBytes = new byte[(m_Width * m_Height * 3) + 40 + 14 + 1];

			uint[] flatPixelsCopy = new uint[m_FlatPixels.Length];
			Array.Copy(m_FlatPixels, flatPixelsCopy, m_FlatPixels.Length);

			TangraCore.PreProcessors.ApplyPreProcessingPixelsOnly(flatPixelsCopy, m_Width, m_Height, m_Bpp);

			TangraCore.GetBitmapPixels(m_Width, m_Height, flatPixelsCopy, rawBitmapBytes, displayBitmapBytes, true, m_Bpp, 0);

			Bitmap displayBitmap = Pixelmap.ConstructBitmapFromBitmapPixels(displayBitmapBytes, m_Width, m_Height);

			Pixelmap rv = new Pixelmap(m_Width, m_Height, m_Bpp, flatPixelsCopy, displayBitmap, displayBitmapBytes);

			return rv;
		}

		public int RecommendedBufferSize
		{
			get { return 1; }
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


	}
}
