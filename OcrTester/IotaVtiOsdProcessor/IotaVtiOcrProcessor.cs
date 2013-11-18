using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tangra.Model.Image;

namespace OcrTester.IotaVtiOsdProcessor
{
    public class IotaVtiOcrProcessor
    {
        internal const int MAX_POSITIONS = 29;
        internal const int FIRST_FRAME_NO_DIGIT_POSITIONS = 22;

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

        public int CurrentImageWidth { get; set; }

        public int CurrentImageHeight { get; set; }

        public int BlockWidth { get; set; }

        public int BlockHeight { get; set; }

        public int BlockOffsetX { get; set; }

		public int[] BlockOffsetsX { get; set; }

        public int LastBlockOffsetsX { get; set; }

        public int SecondLastBlockOffsetsX { get; set; }

        public int BlockOffsetY { get; set; }

        public string CurrentOcredString { get; set; }

        public int LastFrameNoDigitPosition { get; set; }

        public bool SwapFieldsOrder { get; set; }

        public IotaVtiOcrProcessor()
        {
            ChangeState<IotaVtiOcrCalibratingState>();
        }

        public void Process(uint[] pixels, int width, int height, Graphics g)
        {
            CurrentImage = pixels;
            CurrentImageWidth = width;
            CurrentImageHeight = height;

            m_CurrentSate.Process(this, g);
        }

        public void ChangeState<T>() where T : IotaVtiOcrState, new()
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
            return GetBlockAtXOffset(pixelmap, BlockOffsetX + positionIndex * BlockWidth);
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

        public void SetOcredString(string ocredValue)
        {
            CurrentOcredString = ocredValue;
        }
    }
}
