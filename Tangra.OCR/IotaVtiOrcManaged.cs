using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Tangra.Model.Config;
using Tangra.Model.Image;
using Tangra.Model.VideoOperations;
using Tangra.OCR.IotaVtiOsdProcessor;

namespace Tangra.OCR
{
	public class IotaVtiOrcManaged : ITimestampOcr
	{
	    private IVideoController m_VideoController;

		private TimestampOCRData m_InitializationData;
		private int m_FromLine;
		private int m_ToLine;
		private uint[] m_OddFieldPixels;
		private uint[] m_EvenFieldPixels;
        private uint[] m_OddFieldPixelsPreProcessed;
        private uint[] m_EvenFieldPixelsPreProcessed;
		private int m_FieldAreaHeight;
		private int m_FieldAreaWidth;

		private bool m_TVSafeMode;
        private IotaVtiOcrProcessor m_Processor;

		private Dictionary<string, uint[]> m_CalibrationImages = new Dictionary<string, uint[]>();
	    private uint[] m_LatestFrameImage;

		private IotaVtiOcrCorrector m_Corrector = new IotaVtiOcrCorrector();

		public string NameAndVersion()
		{
			return "Generic IOTA-VTI OCR v1.0";
		}

		public string OSDType()
		{
			return "IOTA-VTI";
		}

		public void Initialize(TimestampOCRData initializationData, IVideoController videoController)
		{
			m_InitializationData = initializationData;
		    m_VideoController = videoController;
            m_Processor = null;
		}

		public TimestampOCRData InitializationData
		{
			get { return m_InitializationData; }
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

	        uint median = m_OddFieldPixels.Median();
			for (int i = 0; i < m_OddFieldPixels.Length; i++)
			{
				int darkCorrectedValue = (int) m_OddFieldPixels[i] - (int) median;
				if (darkCorrectedValue < 0) darkCorrectedValue = 0;
				m_OddFieldPixels[i] = (uint) darkCorrectedValue;
			}

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

			median = m_EvenFieldPixels.Median();
			for (int i = 0; i < m_EvenFieldPixels.Length; i++)
			{
				int darkCorrectedValue = (int)m_EvenFieldPixels[i] - (int)median;
				if (darkCorrectedValue < 0) darkCorrectedValue = 0;
				m_EvenFieldPixels[i] = (uint)darkCorrectedValue;
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

            m_LatestFrameImage = data;
        }


		public bool ExtractTime(int frameNo, uint[] data, out DateTime time)
		{
            if (m_Processor.IsCalibrated)
            {
                if (m_VideoController != null)
                    m_VideoController.RegisterExtractingOcrTimestamps();

                PrepareOsdVideoFields(data);

                m_Processor.Process(m_OddFieldPixelsPreProcessed, m_FieldAreaWidth, m_FieldAreaHeight, null, frameNo, true);
                IotaVtiTimeStamp oddFieldOSD = m_Processor.CurrentOcredTimeStamp;

                m_Processor.Process(m_EvenFieldPixelsPreProcessed, m_FieldAreaWidth, m_FieldAreaHeight, null, frameNo, false);
                IotaVtiTimeStamp evenFieldOSD = m_Processor.CurrentOcredTimeStamp;

                time = ExtractDateTime(frameNo, oddFieldOSD, evenFieldOSD);
            }
            else
                time = DateTime.MinValue;

            return time != DateTime.MinValue;
		}

        public bool ExtractTime(int frameNo, uint[] oddPixels, uint[] evenPixels, int width, int height, out DateTime time)
	    {
            if (m_Processor.IsCalibrated)
            {
                m_Processor.Process(oddPixels, width, height, null, frameNo, true);
                IotaVtiTimeStamp oddFieldOSD = m_Processor.CurrentOcredTimeStamp;

                m_Processor.Process(evenPixels, width, height, null, frameNo, false);
                IotaVtiTimeStamp evenFieldOSD = m_Processor.CurrentOcredTimeStamp;

                time = ExtractDateTime(frameNo, oddFieldOSD, evenFieldOSD);
            }
            else
                time = DateTime.MinValue;

            return time != DateTime.MinValue;
	    }

        public List<uint[]> GetLearntDigitPatterns()
        {
            var rv = new List<uint[]>();

            if (m_Processor != null && 
                m_Processor.IsCalibrated)
            {
                rv.Add(m_Processor.ZeroDigitPattern);
                rv.Add(m_Processor.OneDigitPattern);
                rv.Add(m_Processor.TwoDigitPattern);
                rv.Add(m_Processor.ThreeDigitPattern);
                rv.Add(m_Processor.FourDigitPattern);
                rv.Add(m_Processor.FiveDigitPattern);
                rv.Add(m_Processor.SixDigitPattern);
                rv.Add(m_Processor.SevenDigitPattern);
                rv.Add(m_Processor.EightDigitPattern);
                rv.Add(m_Processor.NineDigitPattern);
                rv.Add(m_Processor.ThreeEightXorPattern);
                rv.Add(m_Processor.SixEightXorPattern);
                rv.Add(m_Processor.NineEightXorPattern);
            }

            return rv;
        }

	    public int BlockWidth
	    {
            get
            {
                if (m_Processor != null &&
                    m_Processor.IsCalibrated)
                {
                    return m_Processor.BlockWidth;
                }

                return -1;
            }
    	}

        public int BlockHeight
        {
            get
            {
                if (m_Processor != null &&
                    m_Processor.IsCalibrated)
                {
                    return m_Processor.BlockHeight;
                }

                return -1;
            }
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

		private void LocateTopAndBottomLineOfTimestamp(uint[] preProcessedPixels, int imageWidth, int fromHeight, int toHeight, out int bestTopPosition, out int bestBottomPosition)
		{
			int bestTopScope = -1;
			bestBottomPosition = -1;
			bestTopPosition = -1;			
			int bestBottomScope = -1;


			for (int y = fromHeight + 1; y < toHeight - 1; y++)
			{
				int topScore = 0;
				int bottomScore = 0;

				for (int x = 0; x < imageWidth; x++)
				{
					if (preProcessedPixels[x + imageWidth * (y + 1)] < 127 && preProcessedPixels[x + imageWidth * y] > 127)
					{
						topScore++;
					}

					if (preProcessedPixels[x + imageWidth * (y - 1)] < 127 && preProcessedPixels[x + imageWidth * y] > 127)
					{
						bottomScore++;
					}
				}

				if (topScore > bestTopScope)
				{
					bestTopScope = topScore;
					bestTopPosition = y;
				}

				if (bottomScore > bestBottomScope)
				{
					bestBottomScope = bottomScore;
					bestBottomPosition = y;
				}
			}			
		}

        private bool LocateTimestampPosition(uint[] data)
        {
            uint[] preProcessedPixels = new uint[data.Length];
            Array.Copy(data, preProcessedPixels, data.Length);

            // Process the image
            uint median = preProcessedPixels.Median();
            for (int i = 0; i < preProcessedPixels.Length; i++)
            {
                int darkCorrectedValue = (int)preProcessedPixels[i] - (int)median;
                if (darkCorrectedValue < 0) darkCorrectedValue = 0;
                preProcessedPixels[i] = (uint)darkCorrectedValue;
            }

            uint[] blurResult = BitmapFilter.GaussianBlur(preProcessedPixels, 8, m_InitializationData.FrameWidth, m_InitializationData.FrameHeight);
            uint average = 128;
            uint[] sharpenResult = BitmapFilter.Sharpen(blurResult, 8, m_InitializationData.FrameWidth, m_InitializationData.FrameHeight, out average);

            // Binerize and Inverse
            for (int i = 0; i < sharpenResult.Length; i++)
            {
                sharpenResult[i] = sharpenResult[i] > average ? (uint)0 : (uint)255;
            }
            uint[] denoised = BitmapFilter.Denoise(sharpenResult, 8, m_InitializationData.FrameWidth, m_InitializationData.FrameHeight, out average, false);

            for (int i = 0; i < denoised.Length; i++)
            {
                preProcessedPixels[i] = denoised[i] < 127 ? (uint)0 : (uint)255;
            }

			int bestBottomPosition = -1;
			int bestTopPosition = -1;
			LocateTopAndBottomLineOfTimestamp(
				preProcessedPixels, 
				m_InitializationData.FrameWidth,  
				m_InitializationData.FrameHeight / 2 + 1, 
				m_InitializationData.FrameHeight, 
				out bestTopPosition, 
				out bestBottomPosition);

            m_FromLine = bestTopPosition - 10;
            m_ToLine = bestBottomPosition + 10;
            if (m_ToLine > m_InitializationData.FrameHeight)
                m_ToLine = m_InitializationData.FrameHeight - 2;

            if ((m_ToLine - m_FromLine) %2 == 1)
            {
                if (m_FromLine % 2 == 1)
                    m_FromLine--;
                else
                    m_ToLine++;
            }

			#region We need to make sure that the two fields have the same top and bottom lines

			// Create temporary arrays so the top/bottom position per field can be further refined
			m_FieldAreaHeight = (m_ToLine - m_FromLine) / 2;
			m_FieldAreaWidth = m_InitializationData.FrameWidth;
			m_OddFieldPixels = new uint[m_InitializationData.FrameWidth * m_FieldAreaHeight];
			m_EvenFieldPixels = new uint[m_InitializationData.FrameWidth * m_FieldAreaHeight];
			m_OddFieldPixelsPreProcessed = new uint[m_InitializationData.FrameWidth * m_FieldAreaHeight];
			m_EvenFieldPixelsPreProcessed = new uint[m_InitializationData.FrameWidth * m_FieldAreaHeight];

			int[] DELTAS = new int[] { 0, -1, 1 };
	        int fromLineBase = m_FromLine;
			int toLineBase = m_ToLine;
	        bool matchFound = false;

	        for (int deltaIdx = 0; deltaIdx < DELTAS.Length; deltaIdx++)
	        {
		        m_FromLine = fromLineBase + DELTAS[deltaIdx];
				m_ToLine = toLineBase + DELTAS[deltaIdx];

				PrepareOsdVideoFields(data);
				
				int bestBottomPositionOdd = -1;
				int bestTopPositionOdd = -1;
				int bestBottomPositionEven = -1;
				int bestTopPositionEven = -1;

				LocateTopAndBottomLineOfTimestamp(
					m_OddFieldPixelsPreProcessed,
					m_FieldAreaWidth, 1, m_FieldAreaHeight - 1, 
					out bestTopPositionOdd, out bestBottomPositionOdd);

				LocateTopAndBottomLineOfTimestamp(
					m_EvenFieldPixelsPreProcessed,
					m_FieldAreaWidth, 1, m_FieldAreaHeight - 1, 
					out bestTopPositionEven, out bestBottomPositionEven);

				if (bestBottomPositionOdd == bestBottomPositionEven &&
				    bestTopPositionOdd == bestTopPositionEven)
				{
					matchFound = true;
					m_FromLine = fromLineBase;
					m_ToLine = toLineBase;

					break;
				}
			}
			#endregion

			m_TVSafeMode = m_ToLine + (m_ToLine - m_FromLine) / 2 < m_InitializationData.FrameHeight;

	        return matchFound;
        }

        private void EnsureProcessorInitialized(uint[] data)
        {
            if (m_Processor == null)
            {
                LocateTimestampPosition(data);

                m_FieldAreaHeight = (m_ToLine - m_FromLine) / 2;
                m_FieldAreaWidth = m_InitializationData.FrameWidth;
                m_OddFieldPixels = new uint[m_InitializationData.FrameWidth * m_FieldAreaHeight];
                m_EvenFieldPixels = new uint[m_InitializationData.FrameWidth * m_FieldAreaHeight];
                m_OddFieldPixelsPreProcessed = new uint[m_InitializationData.FrameWidth * m_FieldAreaHeight];
                m_EvenFieldPixelsPreProcessed = new uint[m_InitializationData.FrameWidth * m_FieldAreaHeight];

                m_InitializationData.OSDFrame.Width = m_FieldAreaWidth;
                m_InitializationData.OSDFrame.Height = m_FieldAreaHeight;

                m_Processor = new IotaVtiOcrProcessor(m_TVSafeMode);
            }
        }

		public bool ProcessCalibrationFrame(int frameNo, uint[] data)
		{			
            if (m_Processor == null)
		        EnsureProcessorInitialized(data);

			bool wasCalibrated = m_Processor.IsCalibrated;

		    if (!m_Processor.IsCalibrated)
		        PrepareOsdVideoFields(data);

            if (!m_Processor.IsCalibrated)
                m_Processor.Process(m_OddFieldPixelsPreProcessed, m_FieldAreaWidth, m_FieldAreaHeight, null, frameNo, true);

            if (!m_Processor.IsCalibrated)
                m_Processor.Process(m_EvenFieldPixelsPreProcessed, m_FieldAreaWidth, m_FieldAreaHeight, null, frameNo, false);

            if (!m_Processor.IsCalibrated)
            {
				uint[] pixelsEven = new uint[m_FieldAreaWidth * m_FieldAreaHeight];
				Array.Copy(m_EvenFieldPixelsPreProcessed, pixelsEven, m_EvenFieldPixelsPreProcessed.Length);
				m_CalibrationImages.Add(string.Format(@"{0}-even.bmp", frameNo.ToString("0000000")), pixelsEven);

				uint[] pixelsOdd = new uint[m_FieldAreaWidth * m_FieldAreaHeight];
				Array.Copy(m_OddFieldPixelsPreProcessed, pixelsOdd, m_OddFieldPixelsPreProcessed.Length);
				m_CalibrationImages.Add(string.Format(@"{0}-odd.bmp", frameNo.ToString("0000000")), pixelsOdd);

				uint[] pixelsEvenOrg = new uint[m_FieldAreaWidth * m_FieldAreaHeight];
				Array.Copy(m_EvenFieldPixels, pixelsEvenOrg, m_EvenFieldPixels.Length);
				m_CalibrationImages.Add(string.Format(@"ORG-{0}-even.bmp", frameNo.ToString("0000000")), pixelsEvenOrg);

				uint[] pixelsOddOrg = new uint[m_FieldAreaWidth * m_FieldAreaHeight];
				Array.Copy(m_OddFieldPixels, pixelsOddOrg, m_OddFieldPixelsPreProcessed.Length);
				m_CalibrationImages.Add(string.Format(@"ORG-{0}-odd.bmp", frameNo.ToString("0000000")), pixelsOddOrg);
            }

			if (!wasCalibrated && m_Processor.IsCalibrated)
				m_Corrector.Reset(m_Processor.VideoFormat);

		    return m_Processor.IsCalibrated;
		}

	    public bool ProcessCalibrationFrame(int frameNo, uint[] oddPixels, uint[] evenPixels, int width, int height, bool isTVSafeMode)
	    {
			if (m_Processor == null)
			{
				m_FieldAreaHeight = height;
				m_FieldAreaWidth = width;
				m_OddFieldPixels = new uint[m_FieldAreaWidth * m_FieldAreaHeight];
				m_EvenFieldPixels = new uint[m_FieldAreaWidth * m_FieldAreaHeight];
				m_OddFieldPixelsPreProcessed = new uint[m_FieldAreaWidth * m_FieldAreaHeight];
				m_EvenFieldPixelsPreProcessed = new uint[m_FieldAreaWidth * m_FieldAreaHeight];

				m_Processor = new IotaVtiOcrProcessor(isTVSafeMode);
			}

			bool wasCalibrated = m_Processor.IsCalibrated;

            if (!m_Processor.IsCalibrated)
                m_Processor.Process(oddPixels, width, height, null, frameNo, true);

            if (!m_Processor.IsCalibrated)
                m_Processor.Process(evenPixels, width, height, null, frameNo, false);

			if (!wasCalibrated && m_Processor.IsCalibrated)
				m_Corrector.Reset(m_Processor.VideoFormat);

            return m_Processor.IsCalibrated;
	    }

	    public Dictionary<string, uint[]> GetCalibrationReportImages()
		{
			return m_CalibrationImages;
		}

        public uint[] GetLastUnmodifiedImage()
        {
            return m_LatestFrameImage;
        }

		public void AddConfiguration(uint[] data, OCRConfigEntry config)
		{ }
	    
        private DateTime ExtractDateTime(int frameNo, IotaVtiTimeStamp oddFieldOSD, IotaVtiTimeStamp evenFieldOSD)
        {
            bool failedValidation = false;

	        if (oddFieldOSD == null || evenFieldOSD == null)
		        return DateTime.MinValue;

            if (oddFieldOSD.FrameNumber != evenFieldOSD.FrameNumber - 1 &&
                oddFieldOSD.FrameNumber != evenFieldOSD.FrameNumber + 1)
            {
                // Video fields are not consequtive
                failedValidation = true;
            }

            DateTime oddFieldTimestamp = new DateTime(1, 1, 1, oddFieldOSD.Hours, oddFieldOSD.Minutes, oddFieldOSD.Seconds, (int)Math.Round(oddFieldOSD.Milliseconds10 / 10.0f));
            DateTime evenFieldTimestamp = new DateTime(1, 1, 1, evenFieldOSD.Hours, evenFieldOSD.Minutes, evenFieldOSD.Seconds, (int)Math.Round(evenFieldOSD.Milliseconds10 / 10.0f));

            double fieldDuration = Math.Abs(new TimeSpan(oddFieldTimestamp.Ticks - evenFieldTimestamp.Ticks).TotalMilliseconds);
            
            if (m_Processor.VideoFormat.Value == VideoFormat.PAL &&
                (Math.Abs(fieldDuration - IotaVtiOcrProcessor.FIELD_DURATION_PAL) > 1.0))
            {
                // PAL field is not 20ms
                failedValidation = true;
            }

            if (m_Processor.VideoFormat.Value == VideoFormat.NTSC &&
                (Math.Abs(fieldDuration - IotaVtiOcrProcessor.FIELD_DURATION_NTSC) > 1.0))
            {
                // NTSC field is not 20ms
                failedValidation = true;
            }

			if (failedValidation)
				failedValidation = !m_Corrector.TryToCorrect(frameNo, oddFieldOSD, evenFieldOSD, ref oddFieldTimestamp, ref evenFieldTimestamp);

            if (failedValidation)
            {	            
                if (m_VideoController != null)
                    m_VideoController.RegisterOcrError();
            }
	        else
				m_Corrector.RegisterSuccessfulTimestamp(frameNo, oddFieldOSD, evenFieldOSD, oddFieldTimestamp, evenFieldTimestamp);
            
            if (oddFieldOSD.FrameNumber == evenFieldOSD.FrameNumber - 1)
            {
				return failedValidation ? DateTime.MinValue : oddFieldTimestamp;
            }
            else
            {
				return failedValidation ? DateTime.MinValue : evenFieldTimestamp;
            }
        }
	}
}
