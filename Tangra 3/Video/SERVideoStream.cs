using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

		private SERVideoStream(string fileName)
		{
			m_FileInfo = new SerFileInfo();
			TangraCore.SEROpenFile(fileName, ref m_FileInfo);

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
			get { throw new NotImplementedException(); }
		}

		public double MillisecondsPerFrame
		{
			get { throw new NotImplementedException(); }
		}

		public Pixelmap GetPixelmap(int index)
		{
			throw new NotImplementedException();
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
			throw new NotImplementedException();
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
			throw new NotImplementedException();
		}
	}
}
