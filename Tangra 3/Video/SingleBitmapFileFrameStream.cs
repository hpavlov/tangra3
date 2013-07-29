using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Tangra.Model.Config;
using Tangra.Model.Image;
using Tangra.Model.Video;
using Tangra.VideoOperations.LightCurves;

namespace Tangra.Video
{
	internal class SingleBitmapFileFrameStream : IDisposable, IFrameStream
	{
		internal const string SINGLE_BMP_FILE_ENGINE = "BMP.Image";

		private Pixelmap m_Pixelmap;

		private LCFile m_lcFile;

		private int m_FirstFrame;
		private int m_LastFrame;
		private int m_NumFrames;

		public static SingleBitmapFileFrameStream OpenFile(string fileName)
		{
			Bitmap bmp = (Bitmap)Image.FromFile(fileName);
			return new SingleBitmapFileFrameStream(bmp);
		}

		public SingleBitmapFileFrameStream(Bitmap bitmap)
		{
			m_Pixelmap = Pixelmap.ConstructFromBitmap(bitmap, TangraConfig.ColourChannel.Red);

			m_FirstFrame = 0;
			m_LastFrame = 0;
			m_NumFrames = 1;
		}

		public SingleBitmapFileFrameStream(LCFile lightCurveFile)
		{
			m_lcFile = lightCurveFile;

			m_Pixelmap = Pixelmap.ConstructForLCFileAveragedFrame(m_lcFile.Footer.AveragedFrameBytes, m_lcFile.Footer.AveragedFrameWidth, m_lcFile.Footer.AveragedFrameHeight, m_lcFile.Footer.AveragedFrameBpp);

			m_FirstFrame = (int)m_lcFile.Header.MinFrame;
			m_LastFrame = (int)m_lcFile.Header.MaxFrame;
			m_NumFrames = (int)m_lcFile.Header.MeasuredFrames;
		}

		#region IFrameStream Members

		public int Width
		{
			get { return m_Pixelmap.Width; }
		}

		public int Height
		{
			get { return m_Pixelmap.Height; }
		}

		public int BitPix
		{
			get { return 8; }
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
			return new Pixelmap(m_Pixelmap);
		}

		public int RecommendedBufferSize
		{
			get { return 1; }
		}

		public string VideoFileType
		{
			get { return "BMP Image"; }
		}

		public string Engine { get { return SINGLE_BMP_FILE_ENGINE; } }

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
