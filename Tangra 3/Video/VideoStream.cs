using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using Tangra.Model.Config;
using Tangra.Model.Context;
using Tangra.Model.Image;
using Tangra.Model.Video;
using Tangra.PInvoke;

namespace Tangra.Video
{
	public class InvalidVideoFileException : Exception
	{
		public InvalidVideoFileException(string message)
			: base(message)
		{ }
	}

	public class VideoStream : IFrameStream, IDisposable
	{
		public static VideoStream OpenFile(string fileName)
		{
		    List<string> allEngines = TangraVideo.EnumVideoEngines().ToList();

		    var allEnginesByAttemptOrder = new List<string>(allEngines);
            allEnginesByAttemptOrder.RemoveAt(TangraConfig.Settings.Generic.AviRenderingEngineIndex);
            allEnginesByAttemptOrder.Insert(0, allEngines[TangraConfig.Settings.Generic.AviRenderingEngineIndex]);

            Dictionary<int, string> allEnginesByIndex = allEnginesByAttemptOrder.ToDictionary(x => allEngines.IndexOf(x), x => x);

		    Exception lastError = null;

            foreach (int engineIdx in allEnginesByIndex.Keys)
		    {
                try
                {
                    TangraVideo.SetVideoEngine(engineIdx);

                    VideoFileInfo fileInfo = TangraVideo.OpenFile(fileName);

                    TangraContext.Current.RenderingEngine = allEnginesByIndex[engineIdx];

                    var rv = new VideoStream(fileInfo, fileName);

                    // Try to load the first frame to be sure that it is going to work, before accepting this video engine for rendering
                    rv.GetPixelmap(fileInfo.FirstFrame);

                    return rv;
                }
                catch (Exception ex)
                {
                    lastError = ex;
                }		        
		    }

            throw new InvalidVideoFileException(lastError != null 
                ? lastError.Message 
                : "None of the rendering engines was able to open the file.");
		}

		private string m_FileName;
		private VideoFileInfo m_OpenedFileInfo;

		private VideoStream(VideoFileInfo openedfileInfo, string fileName)
		{
			m_OpenedFileInfo = openedfileInfo;
			m_FileName = fileName;
		}

		public string FileName
		{
			get { return m_FileName; }
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

		public uint GetAav16NormVal()
		{
			return 255;
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
		    byte[] bitmapBytes;
            TangraVideo.GetFrame(index, out pixels, out videoFrame, out bitmapBytes);

            return new Pixelmap(Width, Height, 8, pixels, videoFrame, bitmapBytes);
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
			if (startFrameNo < FirstFrame) startFrameNo = FirstFrame;
			if (startFrameNo > LastFrame) startFrameNo = LastFrame;

			uint[] pixels;
			Bitmap videoFrame;
			byte[] bitmapBytes;

			TangraVideo.GetIntegratedFrame(startFrameNo, framesToIntegrate, isSlidingIntegration, isMedianAveraging, out pixels, out videoFrame, out bitmapBytes);

			return new Pixelmap(Width, Height, 8, pixels, videoFrame, bitmapBytes);
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
