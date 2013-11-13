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

        internal Pixelmap CurrentImage;

        public int BlockWidth { get; set; }

        public int BlockHeight { get; set; }

        public int BlockOffsetX { get; set; }

        public int BlockOffsetY { get; set; }

        public int LastFrameNoDigitPosition { get; set; }

        public IotaVtiOcrProcessor()
        {
            m_CurrentSate = new IotaVtiOcrCalibratingState();
        }

        public void Process(Pixelmap image, Graphics g)
        {
            CurrentImage = image;

            m_CurrentSate.Process(this, g);
        }

        public uint[] GetBlockAt(int x0, int y0)
        {
            return GetBlockAt(CurrentImage, x0, y0);
        }

        public uint[] GetBlockAt(Pixelmap pixelmap, int x0, int y0)
        {
            if (y0 >= BlockOffsetY && y0 <= BlockOffsetY + BlockHeight)
            {
                for (int i = 0; i < MAX_POSITIONS; i++)
                {
                    if (x0 >= BlockOffsetX + i * BlockWidth && x0 < BlockOffsetX + (i + 1) * BlockWidth)
                    {
                        return GetBlockAtPosition(pixelmap, i);
                    }
                }
            }

            return null;
        }

        public uint[] GetBlockAtPosition(Pixelmap pixelmap, int positionIndex)
        {
            uint[] blockPixels = new uint[BlockWidth * BlockHeight];

            for (int y = 0; y < BlockHeight; y++)
            {
                for (int x = 0; x < BlockWidth; x++)
                {
                    blockPixels[x + y * BlockWidth] = pixelmap[BlockOffsetX + positionIndex * BlockWidth + x, BlockOffsetY + y];
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
    }
}
