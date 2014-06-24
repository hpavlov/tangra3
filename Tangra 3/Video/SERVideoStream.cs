using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Tangra.Model.Helpers;
using Tangra.Model.Image;
using Tangra.Model.Video;
using Tangra.PInvoke;

namespace Tangra.Video
{
	public class SERVideoStream : IFrameStream
	{
		public static SERVideoStream OpenFile(string fileName)
		{
			return new SERVideoStream(fileName);
		}


		private SerFileInfo m_FileInfo;
		private string m_FileName;

		private SerFrameInfo m_CurrentFrameInfo;

		private SERVideoStream(string fileName)
		{
			m_FileInfo = new SerFileInfo();

			byte[] observer = new byte[40];
			byte[] instrument = new byte[40];
			byte[] telescope = new byte[40];

			TangraCore.SEROpenFile(fileName, ref m_FileInfo, observer, instrument, telescope);

			m_FileName = fileName;
		}

		public int Width
		{
			get { return m_FileInfo.Width; }
		}

		public int Height
		{
			get { return m_FileInfo.Height; }
		}

		public int BitPix
		{
			get { return m_FileInfo.PixelDepthPerPlane; }
		}

		public int FirstFrame
		{
			get { return 0; }
		}

		public int LastFrame
		{
			get { return m_FileInfo.CountFrames - 1; }
		}

		public int CountFrames
		{
			get { return m_FileInfo.CountFrames; }
		}

		public double FrameRate
		{
			get
			{
				// TODO: Make it possible to report "Unknown Framerate", which will lead to disabling the +/-1 sec and +/-10 sec buttons
				return 25;
			}
		}

		public double MillisecondsPerFrame
		{
			get
			{
				// TODO: Make it possible to report "Unknown Exposure", which will lead to disabling the +/-1 sec and +/-10 sec buttons
				return 40;
			}
		}

		public Pixelmap GetPixelmap(int index)
		{
			if (index < FirstFrame || index > LastFrame)
				throw new ApplicationException("Invalid frame position: " + index);

			uint[] pixels = new uint[Width * Height];
			byte[] displayBitmapBytes = new byte[Width * Height];
			byte[] rawBitmapBytes = new byte[(Width * Height * 3) + 40 + 14 + 1];

			var frameInfo = new SerNativeFrameInfo();

			TangraCore.SERGetFrame(index, pixels, rawBitmapBytes, displayBitmapBytes, ref frameInfo);

			m_CurrentFrameInfo = new SerFrameInfo(frameInfo);

			using (var memStr = new MemoryStream(rawBitmapBytes))
			{
				Bitmap displayBitmap;

				try
				{
					displayBitmap = (Bitmap)Bitmap.FromStream(memStr);
				}
				catch (Exception ex)
				{
					Trace.WriteLine(ex.GetFullStackTrace());
					displayBitmap = new Bitmap(Width, Height);
				}

				var rv = new Pixelmap(Width, Height, BitPix, pixels, displayBitmap, displayBitmapBytes);

				return rv;
			}
		}

		public int RecommendedBufferSize
		{
			get { return 8; }
		}

		public string VideoFileType
		{
			get { return string.Format("SER.{0}", m_FileInfo.PixelDepthPerPlane); }
		}

		public Pixelmap GetIntegratedFrame(int startFrameNo, int framesToIntegrate, bool isSlidingIntegration, bool isMedianAveraging)
		{
			if (startFrameNo < FirstFrame || startFrameNo > LastFrame)
				throw new ApplicationException("Invalid frame position: " + startFrameNo);

			int actualFramesToIntegrate = Math.Min(startFrameNo + framesToIntegrate, LastFrame - 1) - startFrameNo;

			uint[] pixels = new uint[Width * Height];
			byte[] displayBitmapBytes = new byte[Width * Height];
			byte[] rawBitmapBytes = new byte[(Width * Height * 3) + 40 + 14 + 1];
			var frameInfo = new SerNativeFrameInfo();

			TangraCore.SERGetIntegratedFrame(startFrameNo, actualFramesToIntegrate, isSlidingIntegration, isMedianAveraging, pixels, rawBitmapBytes, displayBitmapBytes, ref frameInfo);

			m_CurrentFrameInfo = new SerFrameInfo(frameInfo);

			using (var memStr = new MemoryStream(rawBitmapBytes))
			{
				Bitmap displayBitmap = (Bitmap)Bitmap.FromStream(memStr);

				return new Pixelmap(Width, Height, BitPix, pixels, displayBitmap, displayBitmapBytes);
			}
		}

		public string Engine
		{
			get { return "SER"; }
		}

		public string FileName
		{
			get { return m_FileName; }
		}

		public uint GetAav16NormVal()
		{
			return m_FileInfo.NormalisationValue;
		}
	}
}
