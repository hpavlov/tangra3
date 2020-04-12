using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.Model.Image;
using Tangra.Model.Video;

namespace Tangra.Video
{
	public class RotatedVideoStream : IFrameStream, IDisposable
	{
		private IFrameStream m_Video;
		private double m_Angle;

		public RotatedVideoStream(IFrameStream originalVideo, double angleDegrees)
		{
			m_Video = originalVideo;
			m_Angle = angleDegrees;
		}

		private Pixelmap RotatePixelMap(Pixelmap org)
		{
			if (org == null) return null;

			return org.Rotate(m_Angle);
		}

		public int Width { get; private set; }

		public int Height { get; private set; }

		public int BitPix
		{
			get { return m_Video.BitPix; }
		}

		public int FirstFrame
		{
			get { return m_Video.FirstFrame; }
		}

		public int LastFrame
		{
			get { return m_Video.LastFrame; }
		}

		public int CountFrames
		{
			get { return m_Video.CountFrames; }
		}

		public double FrameRate
		{
			get { return m_Video.FrameRate; }
		}

		public double MillisecondsPerFrame
		{
			get { return m_Video.MillisecondsPerFrame; }
		}

		public Pixelmap GetPixelmap(int index)
		{
			Pixelmap image = m_Video.GetPixelmap(index);
			return RotatePixelMap(image);
		}

		public string GetFrameFileName(int index)
		{
			return m_Video.GetFrameFileName(index);
		}

		public int RecommendedBufferSize
		{
			get { return m_Video.RecommendedBufferSize; }
		}

		public string VideoFileType
		{
			get { return m_Video.VideoFileType; }
		}

		public Pixelmap GetIntegratedFrame(int startFrameNo, int framesToIntegrate, bool isSlidingIntegration, bool isMedianAveraging)
		{
			Pixelmap image = GetIntegratedFrame(startFrameNo, framesToIntegrate, isSlidingIntegration, isMedianAveraging);
			return RotatePixelMap(image);
		}

		public string Engine
		{
			get { return m_Video.Engine; }
		}

		public string FileName
		{
			get { return m_Video.FileName; }
		}

		public uint GetAav16NormVal()
		{
			return GetAav16NormVal();
		}

		public bool SupportsSoftwareIntegration
		{
			get { return m_Video.SupportsSoftwareIntegration; }
		}

        public bool SupportsIntegrationByMedian
        {
            get { return m_Video.SupportsIntegrationByMedian; }
        }

		public bool SupportsFrameFileNames
		{
			get { return m_Video.SupportsFrameFileNames; }
		}

		public void Dispose()
		{
			var disposable = m_Video as IDisposable;
			if (disposable != null)
				disposable.Dispose();
		}
	}
}
