using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Image;
using Tangra.PInvoke;
using Tangra.Video.AstroDigitalVideo;

namespace Tangra.Video
{
	public class ADVFormatException : Exception
	{
		public ADVFormatException(string message)
			: base(message)
		{ }
	}

	public class AstroDigitalVideoStream : IFrameStream
	{
		public static AstroDigitalVideoStream OpenFile(string fileName)
		{
			try
			{
				return new AstroDigitalVideoStream(fileName);
			}
			catch(ADVFormatException ex)
			{
				MessageBox.Show(ex.Message, "Error opening ADV file", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			return null;
		}

		private string m_FileName;
		private int m_Width;
		private int m_Height;
		private double m_FrameRate;

		private int m_FirstFrame;
		private int m_CountFrames;

		private int m_BitPix;
		private bool m_FileOpened;

		private AdvFrameInfo m_CurrentFrameInfo;

		private AstroDigitalVideoStream(string fileName)
		{
			CheckAdvFileFormat(fileName);

			m_FileName = fileName;
			var fileInfo = new AdvFileInfo();

			
			TangraCore.ADVOpenFile(fileName, ref fileInfo);

			m_FirstFrame = 0;
			m_CountFrames = fileInfo.CountFrames;

			m_BitPix = fileInfo.Bpp;
			m_Width = fileInfo.Width;
			m_Height = fileInfo.Height;
			m_FrameRate = 25; // ??

			m_FileOpened = true;			
		}

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
			get { return m_BitPix; }
		}

		public int FirstFrame
		{
			get { return 0; }
		}

		public int LastFrame
		{
			get { return m_CountFrames - 1; }
		}

		public int CountFrames
		{
			get { return m_CountFrames; }
		}

		public double FrameRate
		{
			get { return m_FrameRate; }
		}

		public double MillisecondsPerFrame
		{
			get { return (1000 / m_FrameRate); }
		}


		public Pixelmap GetPixelmap(int index)
		{
			if (index >= m_FirstFrame + m_CountFrames)
				throw new ApplicationException("Invalid frame position: " + index);

			uint[] pixels = new uint[m_Width * m_Height];
			byte[] displayBitmapBytes = new byte[m_Width * m_Height];
			byte[] rawBitmapBytes = new byte[(m_Width * m_Height * 3) + 40 + 14 + 1];
			var frameInfo = new AdvFrameInfoNative();

			byte[] gpsFix = new byte[256 * 16];
			byte[] userCommand = new byte[256 * 16];
			byte[] systemError = new byte[256 * 16];

			TangraCore.ADVGetFrame(index, pixels, rawBitmapBytes, displayBitmapBytes, frameInfo, gpsFix, userCommand, systemError);

			m_CurrentFrameInfo = new AdvFrameInfo(frameInfo);
			m_CurrentFrameInfo.UserCommandString = AdvFrameInfo.GetStringFromBytes(userCommand);
			m_CurrentFrameInfo.SystemErrorString = AdvFrameInfo.GetStringFromBytes(systemError);
			m_CurrentFrameInfo.GPSFixString = AdvFrameInfo.GetStringFromBytes(gpsFix);

			//Trace.WriteLine("RETURNED FROM: ADVGetFrame(" + index + ", pixels, rawBitmapBytes, displayBitmapBytes, frameInfo);");

			//IntPtr hBitmap = Marshal.AllocHGlobal();
			//Bitmap displayBitmap = (Bitmap)Bitmap.FromHbitmap(hBitmap);

			//return new Pixelmap(m_Width, m_Height, 8, pixels, displayBitmap, displayBitmapBytes);

			//byte[] byteData = File.ReadAllBytes(@"C:\Hristo\Linux\VBox Shared Folder\Flea3 Images\Flea3.0.pix");
			//uint[] pix = new uint[640*480];
			//for (int i = 0; i < 640 * 480; i++)
			//{
			//    uint number = (uint)((byteData[2 * i] << 8) + byteData[2 * i + 1]);
			//    pix[i] = number;
			//}
			//GenerateBitmapBytes(pixels, rawBitmapBytes);


			using (MemoryStream memStr = new MemoryStream(rawBitmapBytes))
			{
				Bitmap displayBitmap = (Bitmap)Bitmap.FromStream(memStr);

				var rv = new Pixelmap(m_Width, m_Height, m_BitPix, pixels, displayBitmap);
				rv.FrameState = GetCurrentFrameState();
				return rv;
			}
		}

		public int RecommendedBufferSize
		{
			get { return 8; }
		}

		public string VideoFileType
		{
			get { return string.Format("ADV.{0}", BitPix); }
		}

		private FrameStateData GetCurrentFrameState()
		{
			if (m_CurrentFrameInfo != null)
			{
				var rv = new FrameStateData();
				rv.VideoCameraFrameId = m_CurrentFrameInfo.VideoCameraFrameId;
				rv.CentralExposureTime = m_CurrentFrameInfo.MiddleExposureTimeStamp;
				rv.SystemTime = m_CurrentFrameInfo.SystemTime;
				rv.ExposureInMilliseconds = m_CurrentFrameInfo.Exposure10thMs / 10.0f;

				rv.Gain = m_CurrentFrameInfo.Gain;
				rv.Gamma = m_CurrentFrameInfo.Gamma;
				rv.Offset = m_CurrentFrameInfo.Offset;

				rv.NumberSatellites = m_CurrentFrameInfo.GPSTrackedSattelites;

				rv.AlmanacStatus = m_CurrentFrameInfo.GPSAlmanacStatus.ToString("#");

				rv.GPSFixStatus = m_CurrentFrameInfo.GPSFixStatus.ToString("#");

				rv.Messages = string.Empty;
				if (m_CurrentFrameInfo.SystemErrorString != null)
					rv.Messages = string.Concat(rv.Messages, m_CurrentFrameInfo.SystemErrorString, "\r\n");
				if (m_CurrentFrameInfo.UserCommandString != null)
					rv.Messages = string.Concat(rv.Messages, m_CurrentFrameInfo.UserCommandString, "\r\n");
				if (m_CurrentFrameInfo.GPSFixString != null)
					rv.Messages = string.Concat(rv.Messages, m_CurrentFrameInfo.GPSFixString, "\r\n");

				return rv;
			}
			else
				return new FrameStateData();
		}

		public Pixelmap GetIntegratedFrame(int startFrameNo, int framesToIntegrate, bool isSlidingIntegration, bool isMedianAveraging)
		{
			if (startFrameNo < 0 || startFrameNo >= m_FirstFrame + m_CountFrames)
				throw new ApplicationException("Invalid frame position: " + startFrameNo);

			int actualFramesToIntegrate = Math.Min(startFrameNo + framesToIntegrate, m_FirstFrame + m_CountFrames - 1) - startFrameNo;

			uint[] pixels = new uint[m_Width * m_Height];
			byte[] displayBitmapBytes = new byte[m_Width * m_Height];
			byte[] rawBitmapBytes = new byte[(m_Width * m_Height * 3) + 40 + 14 + 1];
			var frameInfo = new AdvFrameInfoNative();

			TangraCore.ADVGetIntegratedFrame(startFrameNo, actualFramesToIntegrate, isSlidingIntegration, isMedianAveraging, pixels, rawBitmapBytes, displayBitmapBytes, frameInfo);

			m_CurrentFrameInfo = new AdvFrameInfo(frameInfo);

			using (MemoryStream memStr = new MemoryStream(rawBitmapBytes))
			{
				Bitmap displayBitmap = (Bitmap)Bitmap.FromStream(memStr);

				return new Pixelmap(m_Width, m_Height, m_BitPix, pixels, displayBitmap);
			}
		}

		public string Engine
		{
			get { return "ADV"; }
		}

		private void CheckAdvFileFormat(string fileName)
		{
			var advFile = AdvFile.OpenFile(fileName);

			CheckAdvFileFormatInternal(advFile);
		}

		private void CheckAdvFileFormatInternal(AdvFile advFile)
		{
			bool fileIsCorrupted = true;
			bool isADVFormat = false;
			int advFormatVersion = -1;
			DoConsistencyCheck(advFile, ref fileIsCorrupted, ref isADVFormat, ref advFormatVersion);

			if (!isADVFormat)
				throw new ADVFormatException("The file is not in ADV format.");

			if (advFormatVersion > 1)
				throw new ADVFormatException("The file ADV version is not supported yet.");

			if (fileIsCorrupted)
				throw new ADVFormatException("The ADV file may be corrupted.\r\n\r\nTry to recover it from Tools -> Repair ADV File");
		}

		private void DoConsistencyCheck(AdvFile advFile, ref bool fileIsCorrupted, ref bool isADVFormat, ref int advFormatVersion)
		{
			fileIsCorrupted = advFile.IsCorrupted;

			string fstsTypeStr;
			if (!advFile.AdvFileTags.TryGetValue("FSTF-TYPE", out fstsTypeStr))
				isADVFormat = false;
			else
				isADVFormat = fstsTypeStr == "ADV";

			string advVersionStr;
			if (!advFile.AdvFileTags.TryGetValue("ADV-VERSION", out advVersionStr))
				advFormatVersion = -1;
			else
				if (!int.TryParse(advVersionStr, out advFormatVersion))
					advFormatVersion = -1;
		}
	}
}
