using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Tangra.OCR.GpsBoxSprite
{
    public class GpsBoxSpriteOcrProcessor
    {
        private List<GpsBoxSpriteLineOcr> m_Lines = new List<GpsBoxSpriteLineOcr>();
 
        public GpsBoxSpriteOcrProcessor(List<OsdLineConfig> lineConfigs)
        {
            foreach (var lineConfig in lineConfigs)
            {
                m_Lines.Add(new GpsBoxSpriteLineOcr(lineConfig));
            }
        }

        public void DrawLegend(Graphics graphics)
        {
            foreach (var line in m_Lines)
                line.DrawLegend(graphics);
        }
    }
}
