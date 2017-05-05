using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Tangra.OCR.GpsBoxSprite
{
    public class OsdLineConfig
    {
        public decimal Left;
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

    internal class GpsBoxSpriteLineOcr
    {
        private OsdLineConfig m_LineConfig;

        public GpsBoxSpriteLineOcr(OsdLineConfig lineConfig)
        {
            m_LineConfig = lineConfig;
        }

        public void DrawLegend(Graphics graphics)
        {
            foreach (var idx in m_LineConfig.BoxIndexes)
            {
                float x = (float)m_LineConfig.Left + idx * (float)m_LineConfig.BoxWidth;
                graphics.DrawRectangle(Pens.LightSlateGray, x, (float)m_LineConfig.Top, (float)m_LineConfig.BoxWidth, (float)(m_LineConfig.Bottom - m_LineConfig.Top));
            }
        }
    }
}
