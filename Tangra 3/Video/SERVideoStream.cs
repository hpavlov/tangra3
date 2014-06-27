using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Helpers;
using Tangra.Model.Image;
using Tangra.Model.Video;
using Tangra.PInvoke;
using Tangra.Video.SER;

namespace Tangra.Video
{
	public class SERVideoStream : IFrameStream
	{
		public static SERVideoStream OpenFile(string fileName, IWin32Window parentForm)
		{
			var fileInfo = new SerFileInfo();

			byte[] observer = new byte[40];
			byte[] instrument = new byte[40];
			byte[] telescope = new byte[40];

			TangraCore.SEROpenFile(fileName, ref fileInfo, observer, instrument, telescope, false);

			var frmInfo = new frmEnterSERFileInfo(fileInfo);
			if (frmInfo.ShowDialog(parentForm) == DialogResult.OK)
			{
				TangraCore.SERCloseFile();

				return new SERVideoStream(fileName, frmInfo.FrameRate, frmInfo.BitPix);
			}
			return null;
		}


		private SerFileInfo m_FileInfo;
		private string m_FileName;

		private SerFrameInfo m_CurrentFrameInfo;

		private SERVideoStream(string fileName, double frameRate, int cameraBitPix)
		{
			m_FileInfo = new SerFileInfo();

			byte[] observer = new byte[40];
			byte[] instrument = new byte[40];
			byte[] telescope = new byte[40];

			TangraCore.SEROpenFile(fileName, ref m_FileInfo, observer, instrument, telescope, false);

			m_FileName = fileName;

			BitPix = cameraBitPix;
			FrameRate = frameRate;
			MillisecondsPerFrame = 1000 / frameRate;

			Observer = Encoding.UTF8.GetString(observer).Trim();
			Instrument = Encoding.UTF8.GetString(instrument).Trim();
			Telescope = Encoding.UTF8.GetString(telescope).Trim();
		}

		public string Observer { get; private set; }

		public string Instrument { get; private set; }

		public string Telescope { get; private set; }

		public int Width
		{
			get { return m_FileInfo.Width; }
		}

		public int Height
		{
			get { return m_FileInfo.Height; }
		}

		public int BitPix { get; private set; }

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

		public double FrameRate { get; private set; }

		public double MillisecondsPerFrame { get; private set; }


		public Pixelmap GetPixelmap(int index)
		{
			if (index < FirstFrame || index > LastFrame)
				throw new ApplicationException("Invalid frame position: " + index);

			uint[] pixels = new uint[Width * Height];
			byte[] displayBitmapBytes = new byte[Width * Height];
			byte[] rawBitmapBytes = new byte[(Width * Height * 3) + 40 + 14 + 1];

			var frameInfo = new SerNativeFrameInfo();

			TangraCore.SERGetFrame(index, pixels, rawBitmapBytes, displayBitmapBytes, BitPix, ref frameInfo);

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

			TangraCore.SERGetIntegratedFrame(startFrameNo, actualFramesToIntegrate, isSlidingIntegration, isMedianAveraging, pixels, rawBitmapBytes, displayBitmapBytes, BitPix, ref frameInfo);

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
