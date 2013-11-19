using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tangra.Model.Image;
using Tangra.OCR.IotaVtiOsdProcessor;

namespace Tangra.OCR.IotaVtiOsdProcessor
{
    internal class IotaVtiOcrProcessor
    {
        internal const int MAX_POSITIONS = 29;
        internal const int FIRST_FRAME_NO_DIGIT_POSITIONS = 22;
	    internal const float FIELD_DURATION_PAL = 20.00f;
		internal const float FIELD_DURATION_NTSC = 16.68f;
		internal const float COEFF_FIRST_FRAME_NO_DIGIT_POSITION = 555.0f / 720.0f;

        public uint[] ZeroDigitPattern;
        public uint[] OneDigitPattern;
        public uint[] TwoDigitPattern;
        public uint[] ThreeDigitPattern;
        public uint[] FourDigitPattern;
        public uint[] FiveDigitPattern;
        public uint[] SixDigitPattern;
        public uint[] SevenDigitPattern;
        public uint[] EightDigitPattern;
        public uint[] NineDigitPattern;

        private IotaVtiOcrState m_CurrentSate;

        internal uint[] CurrentImage;

        public int CurrentImageWidth { get; private set; }

        public int CurrentImageHeight { get; private set; }

        public int BlockWidth { get; set; }

        public int BlockHeight { get; set; }

        public int BlockOffsetX { get; set; }

		public int[] BlockOffsetsX { get; set; }

        public int LastBlockOffsetsX { get; set; }

        public int SecondLastBlockOffsetsX { get; set; }

        public int BlockOffsetY { get; set; }

        public string CurrentOcredString { get; set; }

        public IotaVtiTimeStamp CurrentOcredTimeStamp { get; set; }

        public int LastFrameNoDigitPosition { get; set; }

		public bool IsTvSafeMode { get; set; }

	    public bool IsCalibrated
        {
            get { return m_CurrentSate is IotaVtiOcrCalibratedState; }
        }

        public IotaVtiOcrProcessor(bool isTVSafeMode)
        {
            ChangeState<IotaVtiOcrCalibratingState>();
	        IsTvSafeMode = isTVSafeMode;
        }

        public void Process(uint[] pixels, int width, int height, Graphics g, int frameNo, bool isOddField)
        {
            CurrentImage = pixels;
            CurrentImageWidth = width;
            CurrentImageHeight = height;

            m_CurrentSate.Process(this, g, frameNo, isOddField);
        }

        internal void ChangeState<T>() where T : IotaVtiOcrState, new()
        {
            if (m_CurrentSate != null)
                m_CurrentSate.FinaliseState(this);

            m_CurrentSate = new T();
            m_CurrentSate.InitialiseState(this);
        }

        public uint[] GetBlockAt(int x0, int y0)
        {
            return GetBlockAt(CurrentImage, x0, y0);
        }

        public uint[] GetBlockAt(uint[] pixelmap, int x0, int y0)
        {
            if (y0 >= BlockOffsetY && y0 <= BlockOffsetY + BlockHeight)
            {
                for (int i = 0; i < MAX_POSITIONS - 1; i++)
                {
                    if (x0 >= BlockOffsetsX[i] && x0 < BlockOffsetsX[i + 1])
                    {
                        return GetBlockAtXOffset(pixelmap, BlockOffsetsX[i]);
                    }
                }
            }

            return null;
        }

        public bool IsMatchingSignature(int nuberMarkedPixels)
        {
            return 10 * nuberMarkedPixels < BlockWidth * BlockHeight;
        }

        public uint[] GetBlockAtPosition(int positionIndex)
        {
            return GetBlockAtPosition(CurrentImage, positionIndex);
        }

        public uint[] GetBlockAtPosition(uint[] pixelmap, int positionIndex)
        {
			return GetBlockAtXOffset(pixelmap, BlockOffsetsX[positionIndex]);
        }

        public uint[] GetBlockAtXOffset(uint[] pixelmap, int xOffset)
        {
            uint[] blockPixels = new uint[BlockWidth * BlockHeight];

            for (int y = 0; y < BlockHeight; y++)
            {
                for (int x = 0; x < BlockWidth; x++)
                {
                    blockPixels[x + y * BlockWidth] = pixelmap[xOffset + x + (BlockOffsetY + y) * CurrentImageWidth];
                }
            }
            return blockPixels;
        }

		public bool SwapFieldsOrder { get; set; }

		public VideoFormat? VideoFormat { get; set; }

	    public void LearnDigitPattern(uint[] pattern, int digit)
        {
            if (digit == 0)
                ZeroDigitPattern = pattern;
            else if (digit == 1)
                OneDigitPattern = pattern;
            else if (digit == 2)
                TwoDigitPattern = pattern;
            else if (digit == 3)
                ThreeDigitPattern = pattern;
            else if (digit == 4)
                FourDigitPattern = pattern;
            else if (digit == 5)
                FiveDigitPattern = pattern;
            else if (digit == 6)
                SixDigitPattern = pattern;
            else if (digit == 7)
                SevenDigitPattern = pattern;
            else if (digit == 8)
                EightDigitPattern = pattern;
            else if (digit == 9)
                NineDigitPattern = pattern;
        }

        public void SetOcredString(IotaVtiTimeStampStrings ocredValue)
        {
            CurrentOcredString = string.Format("{0}|{1}:{2}:{3}|{4} {5}|{6}", ocredValue.NumSat, ocredValue.HH, ocredValue.MM, ocredValue.SS, ocredValue.FFFF1, ocredValue.FFFF2, ocredValue.FRAMENO);

            if (ocredValue.AllCharsPresent())
                CurrentOcredTimeStamp = new IotaVtiTimeStamp(ocredValue);
            else
                CurrentOcredTimeStamp = null;
        }
    }
}
