using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Tangra.Model.Config;
using Tangra.Model.Image;

namespace Tangra.OCR
{
	public class IotaVtiOrcManaged : ITimestampOcr
	{
		private TimestampOCRData m_InitializationData;
		private int m_FromLine;
		private int m_ToLine;
		private uint[] m_OddFieldPixels;
		private uint[] m_EvenFieldPixels;
		private int m_FieldAreaHeight;
		private int m_FieldAreaWidth;

		public string NameAndVersion()
		{
			return "IOTA-VTI OCR v0.1";
		}

		public string OSDType()
		{
			return "IOTA-VTI";
		}

		public void Initialize(TimestampOCRData initializationData)
		{
			m_InitializationData = initializationData;

			m_FromLine = (int)Math.Round(530.0 * initializationData.FrameHeight / 567.0);
			m_ToLine = initializationData.FrameHeight;

			if ((m_ToLine - m_FromLine) % 2 == 1) 
				m_FromLine--;

			m_FieldAreaHeight = (m_ToLine - m_FromLine) / 2;
			m_FieldAreaWidth = m_InitializationData.FrameWidth;
			m_OddFieldPixels = new uint[initializationData.FrameWidth * m_FieldAreaHeight];
			m_EvenFieldPixels = new uint[initializationData.FrameWidth * m_FieldAreaHeight];
		}

		public void RefiningFrame(uint[] data, float refiningPercentageLeft)
		{ }

		public void PrepareForMeasurements(uint[] data)
		{
			
		}

		public bool ExtractTime(int frameNo, uint[] data, out DateTime time)
		{
			for (int y = m_FromLine; y < m_ToLine; y++)
			{
				bool isOddLine = y % 2 == 1;
				int lineNo = (y - m_FromLine) / 2;
				if (isOddLine)
					Array.Copy(data, y * m_FieldAreaWidth, m_OddFieldPixels, lineNo * m_FieldAreaWidth, m_FieldAreaWidth);
				else
					Array.Copy(data, y * m_FieldAreaWidth, m_EvenFieldPixels, lineNo * m_FieldAreaWidth, m_FieldAreaWidth);
			}

			// Split into fields only in the region where IOTA-VTI could be, Then threat as two separate images, and for each of them do:
			// 1) Gaussian blur (small) BitmapFilter.LOW_PASS_FILTER_MATRIX
			// 2) Sharpen BitmapFilter.SHARPEN_MATRIX
			// 3) Binarize - get Median, all below change to 0, all above change to Max (256)
			// 4) De-noise BitmapFilter.DENOISE_MATRIX
			
			// TODO: Move the Covariance operations into C++ land

			uint[] blurResult = BitmapFilter.GaussianBlur(m_OddFieldPixels, 8, m_InitializationData.FrameWidth, m_FieldAreaHeight);
			uint average = 128;
			uint[] sharpenResult = BitmapFilter.Sharpen(blurResult, 8, m_InitializationData.FrameWidth, m_FieldAreaHeight, out average);

			// Binerize and Inverse
			for (int i = 0; i < sharpenResult.Length; i++)
			{
				sharpenResult[i] = sharpenResult[i] > average ? (uint)0 : (uint)255;
			}
			uint[] denoised = BitmapFilter.Denoise(sharpenResult, 8, m_InitializationData.FrameWidth, m_FieldAreaHeight, out average, false);

			for (int i = 0; i < denoised.Length; i++)
			{
				denoised[i] = denoised[i] < 127 ? (uint)0 : (uint)255;
			}
			Bitmap img = Pixelmap.ConstructBitmapFromBitmapPixels(denoised, m_InitializationData.FrameWidth, m_FieldAreaHeight);
			img.Save(string.Format(@"D:\Hristo\Tangra3\Test Data\OcrTestImages\{0}-odd.bmp", frameNo));
			img.Dispose();

			blurResult = BitmapFilter.GaussianBlur(m_EvenFieldPixels, 8, m_InitializationData.FrameWidth, m_FieldAreaHeight);
			average = 128;
			sharpenResult = BitmapFilter.Sharpen(blurResult, 8, m_InitializationData.FrameWidth, m_FieldAreaHeight, out average);

			// Binerize and Inverse
			for (int i = 0; i < sharpenResult.Length; i++)
			{
				sharpenResult[i] = sharpenResult[i] > average ? (uint)0 : (uint)255;
			}
			denoised = BitmapFilter.Denoise(sharpenResult, 8, m_InitializationData.FrameWidth, m_FieldAreaHeight, out average, false);

			for (int i = 0; i < denoised.Length; i++)
			{
				denoised[i] = denoised[i] < 127 ? (uint)0 : (uint)255;
			}
			img = Pixelmap.ConstructBitmapFromBitmapPixels(denoised, m_InitializationData.FrameWidth, m_FieldAreaHeight);
			img.Save(string.Format(@"D:\Hristo\Tangra3\Test Data\OcrTestImages\{0}-even.bmp", frameNo));
			img.Dispose();
			// Now the image is ready for alignment and recognition


			time = DateTime.MinValue;
			return false;
		}

		public bool RequiresConfiguring
		{
			get { return false; }
		}

		public bool RequiresCalibration
		{
			get { return false; }
		}

		public void TryToAutoConfigure(uint[] data)
		{
		}

		public OCRConfigEntry TryCalibrate(uint[] data)
		{
			return new OCRConfigEntry();
		}

		public void AddConfiguration(uint[] data, OCRConfigEntry config)
		{ }
	}
}
