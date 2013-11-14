using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Tangra.Controller;
using Tangra.Model.Config;
using Tangra.Model.Image;
using Tangra.OCR.IotaVtiOsdProcessor;

namespace Tangra.OCR
{
	public class IotaVtiOrcManaged : ITimestampOcr
	{
	    private VideoController m_VideoController;

		private TimestampOCRData m_InitializationData;
		private int m_FromLine;
		private int m_ToLine;
		private uint[] m_OddFieldPixels;
		private uint[] m_EvenFieldPixels;
        private uint[] m_OddFieldPixelsPreProcessed;
        private uint[] m_EvenFieldPixelsPreProcessed;
		private int m_FieldAreaHeight;
		private int m_FieldAreaWidth;

        private IotaVtiOcrProcessor m_Processor = new IotaVtiOcrProcessor();

		public string NameAndVersion()
		{
			return "Generic IOTA-VTI OCR v1.0";
		}

		public string OSDType()
		{
			return "IOTA-VTI";
		}

		public void Initialize(TimestampOCRData initializationData, VideoController videoController)
		{
			m_InitializationData = initializationData;
		    m_VideoController = videoController;

			m_FromLine = (int)Math.Round(530.0 * initializationData.FrameHeight / 567.0);
			m_ToLine = initializationData.FrameHeight;

			if ((m_ToLine - m_FromLine) % 2 == 1) 
				m_FromLine--;

			m_FieldAreaHeight = (m_ToLine - m_FromLine) / 2;
			m_FieldAreaWidth = m_InitializationData.FrameWidth;
			m_OddFieldPixels = new uint[initializationData.FrameWidth * m_FieldAreaHeight];
			m_EvenFieldPixels = new uint[initializationData.FrameWidth * m_FieldAreaHeight];
            m_OddFieldPixelsPreProcessed = new uint[initializationData.FrameWidth * m_FieldAreaHeight];
            m_EvenFieldPixelsPreProcessed = new uint[initializationData.FrameWidth * m_FieldAreaHeight];

            m_Processor = new IotaVtiOcrProcessor();
		}

		public void RefiningFrame(uint[] data, float refiningPercentageLeft)
		{ }

		public void PrepareForMeasurements(uint[] data)
		{
			
		}

        private void PrepareOsdVideoFields(uint[] data)
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
            // 3) Binarize - get Average, all below change to 0, all above change to Max (256)
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
                m_OddFieldPixelsPreProcessed[i] = denoised[i] < 127 ? (uint)0 : (uint)255;
            }

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
                m_EvenFieldPixelsPreProcessed[i] = denoised[i] < 127 ? (uint)0 : (uint)255;
            }
        }


		public bool ExtractTime(int frameNo, uint[] data, out DateTime time)
		{
            if (m_Processor.IsCalibrated)
            {
                PrepareOsdVideoFields(data);

                m_Processor.Process(m_OddFieldPixelsPreProcessed, m_FieldAreaWidth, m_FieldAreaHeight, null, frameNo, true);
                string oddFieldOSD = m_Processor.CurrentOcredString;

                m_Processor.Process(m_EvenFieldPixelsPreProcessed, m_FieldAreaWidth, m_FieldAreaHeight, null, frameNo, false);
                string evenFieldOSD = m_Processor.CurrentOcredString;

                time = ExtractDateTime(frameNo, oddFieldOSD, evenFieldOSD);
            }
            else
                time = DateTime.MinValue;

            return time != DateTime.MinValue;
		}

		public bool RequiresConfiguring
		{
			get { return false; }
		}

		public bool RequiresCalibration
		{
			get { return true; }
		}

		public void TryToAutoConfigure(uint[] data)
		{
		}

		public bool ProcessCalibrationFrame(int frameNo, uint[] data)
		{
		    if (!m_Processor.IsCalibrated)
		        PrepareOsdVideoFields(data);

            if (!m_Processor.IsCalibrated)
                m_Processor.Process(m_OddFieldPixelsPreProcessed, m_FieldAreaWidth, m_FieldAreaHeight, null, frameNo, true);

            if (!m_Processor.IsCalibrated)
                m_Processor.Process(m_EvenFieldPixelsPreProcessed, m_FieldAreaWidth, m_FieldAreaHeight, null, frameNo, false);

            //if (!m_Processor.IsCalibrated)
            //{
            //    Bitmap img = Pixelmap.ConstructBitmapFromBitmapPixels(m_EvenFieldPixelsPreProcessed, m_InitializationData.FrameWidth, m_FieldAreaHeight);
            //    img.Save(string.Format(@"D:\Work\tangra3\TestFiles\FailedCalibration\{0}-even.bmp", frameNo.ToString("0000")));

            //    img = Pixelmap.ConstructBitmapFromBitmapPixels(m_OddFieldPixelsPreProcessed, m_InitializationData.FrameWidth, m_FieldAreaHeight);
            //    img.Save(string.Format(@"D:\Work\tangra3\TestFiles\FailedCalibration\{0}-odd.bmp", frameNo.ToString("0000")));
            //}

		    return m_Processor.IsCalibrated;
		}

		public void AddConfiguration(uint[] data, OCRConfigEntry config)
		{ }

        private DateTime ExtractDateTime(int frameNo, string oddFieldOSD, string evenFieldOSD)
        {
            m_VideoController.PrintOcrTimeStamps(oddFieldOSD, evenFieldOSD);
            return DateTime.MinValue;
        }
	}
}
