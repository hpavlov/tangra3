using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using Tangra.Model.Image;
using Tangra.PInvoke;

namespace Tangra.Video
{
	public class VideoStream : IFrameStream, IDisposable
	{
		public static VideoStream OpenFile(string fileName)
		{
			try
			{
				VideoFileInfo fileInfo = TangraVideo.OpenFile(fileName);
				return new VideoStream(fileInfo);
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex);
				return null;
			}
		}

		private VideoFileInfo m_OpenedFileInfo;

		private VideoStream(VideoFileInfo openedfileInfo)
		{
			m_OpenedFileInfo = openedfileInfo;
		}

		public int Width
		{
			get { return m_OpenedFileInfo.Width; }
		}

		public int Height
		{
			get { return m_OpenedFileInfo.Height; }
		}

		public int BitPix
		{
			get { return 8; }
		}

		public int FirstFrame
		{
			get { return m_OpenedFileInfo.FirstFrame; }
		}

		public int LastFrame
		{
			get { return m_OpenedFileInfo.FirstFrame + m_OpenedFileInfo.CountFrames - 1; }
		}

		public int CountFrames
		{
			get { return m_OpenedFileInfo.CountFrames; }
		}

		public double FrameRate
		{
			get { return m_OpenedFileInfo.FrameRate; }
		}

		public double MillisecondsPerFrame
		{
			get { return 1000.0/m_OpenedFileInfo.FrameRate; }
		}

		public Pixelmap GetPixelmap(int index)
		{
			if (index < FirstFrame) index = FirstFrame;
			if (index > LastFrame) index = LastFrame;

			uint[] pixels;
			Bitmap videoFrame;
			TangraVideo.GetFrame(index, out pixels, out videoFrame);

			return new Pixelmap(Width, Height, 8, pixels, videoFrame);
		}

		public int RecommendedBufferSize
		{
			get { return 8; }
		}

		public string VideoFileType
		{
			get { return m_OpenedFileInfo.VideoFileType; }
		}

		public Pixelmap GetIntegratedFrame(int startFrameNo, int framesToIntegrate, bool isSlidingIntegration, bool isMedianAveraging)
		{
			throw new NotImplementedException();
		}

		public string Engine
		{
			get { return m_OpenedFileInfo.Engine; }
		}

		public void Dispose()
		{
			TangraVideo.CloseFile();
		}
	}
}
