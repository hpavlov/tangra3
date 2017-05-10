using System;
using System.Collections.Generic;
using System.Drawing;
using Tangra.OCR.Model;

namespace Tangra.OCR.GpsBoxSprite
{
    public class OsdLineConfig
    {
        public decimal Left;
        public decimal Right;
        public decimal Top;
        public decimal Bottom;
        public decimal BoxWidth;
        public decimal BoxHeight
        {
            get { return Bottom - Top; }
        }
        public decimal FullnessScore;
        public int[] BoxIndexes;
    }

    public class OcrCalibrationFrame
    {
        public OsdLineConfig[] DetectedOsdLines;
        public uint[] ProcessedPixels;
        public uint[] RawPixels;
    }

    internal class GpsBoxSpriteLineOcr
    {
        private int m_Top;
        private int m_Bottom;
        private decimal m_Left;
        private decimal m_BlockWidth;
        private int[] m_BlockIndexes;
        private int m_BlockHeight;

        public GpsBoxSpriteLineOcr(int top, int bottom, decimal left, decimal blockWidth, int[] blockIndexes)
        {
            m_Top = top;
            m_Bottom = bottom;
            m_BlockHeight = m_Bottom - m_Top + 1;
            m_Left = left;
            m_BlockWidth = blockWidth;
            m_BlockIndexes = blockIndexes;
        }

        public GpsBoxSpriteLineOcr(OsdLineConfig lineConfig)
        {
            m_Left = lineConfig.Left;
            m_BlockWidth = lineConfig.BoxWidth;
            m_Top = (int)lineConfig.Top;
            m_BlockHeight = m_Bottom - m_Top + 1;
            m_Bottom = (int)lineConfig.Bottom;
            m_BlockIndexes = lineConfig.BoxIndexes;
        }

        public void DrawLegend(Graphics graphics)
        {
            foreach (var idx in m_BlockIndexes)
            {
                float x = (float)m_Left + idx * (float)m_BlockWidth;
                graphics.DrawRectangle(Pens.LightSlateGray, x, (float)m_Top, (float)m_BlockWidth, (float)(m_Bottom - m_Top));
            }
        }

        internal List<Tuple<decimal[,], decimal[,]>> ExtractBlockNumbers(OcrCalibrationFrame cfgFrame, int frameWidth, int frameHeight)
        {
            var rv = new List<Tuple<decimal[,], decimal[,]>>();

            SubPixelImage subPixels = new SubPixelImage(cfgFrame.ProcessedPixels, frameWidth, frameHeight);

            int blockIntWidth = (int) Math.Ceiling(m_BlockWidth);
            int blockFieldHeight = (int)Math.Ceiling((m_Bottom - m_Top) / 2.0);
            foreach (var blockIndex in m_BlockIndexes)
            {
                decimal x0 = m_Left + blockIndex * m_BlockWidth;
                decimal[,] oddBlock = new decimal[blockFieldHeight, blockIntWidth];
                decimal[,] evenBlock = new decimal[blockFieldHeight, blockIntWidth];
                for (int y = m_Top; y < m_Bottom; y++)
                {
                    for (int x = 0; x < blockIntWidth; x++)
                    {
                        var pix = subPixels.GetWholePixelAt(x0 + x, y);
                        var fieldY = (y - m_Top) / 2;
                        if (y % 2 == 1)
                            oddBlock[fieldY, x] = pix;
                        else
                            evenBlock[fieldY, x] = pix;
                    }
                }

                rv.Add(Tuple.Create(oddBlock, evenBlock));
            }

            return rv;
        }
    }
}
