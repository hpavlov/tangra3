using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.Video;

namespace Tangra.VideoOperations.LightCurves.Helpers
{
    public class LCFileImagePixelProvider : IImagePixelProvider
    {
        private LCFile m_LCFile;

        internal LCFileImagePixelProvider(LCFile lcFile)
        {
            m_LCFile = lcFile;
        }

        public int Width
        {
            get { return m_LCFile.Footer.AveragedFrameWidth; }
        }

        public int Height
        {
            get { return m_LCFile.Footer.AveragedFrameWidth; }
        }

        public int FirstFrame
        {
            get { return (int)m_LCFile.Header.MinFrame; }
        }

        public int LastFrame
        {
            get { return (int)m_LCFile.Header.MaxFrame; }
        }

        public int[,] GetPixelArray(int frameNo, System.Drawing.Rectangle rect)
        {
            if (rect.Width != 32 || rect.Height != 32)
                throw new NotSupportedException();

            int[,] rv = new int[32, 32];

            if (frameNo >= m_LCFile.Header.MinFrame && frameNo < m_LCFile.Header.MaxFrame)
            {
                LCMeasurement mea = m_LCFile.Data[0][frameNo - (int) m_LCFile.Header.MinFrame];
                for (int x = 0; x < 32; x++)
                for (int y = 0; y < 32; y++)
                {
                    rv[x,y] = (int)mea.PixelData[x, y]; // NOTE: PixelData is 35x35
                }
            }

            return rv;
        }
    }
}
