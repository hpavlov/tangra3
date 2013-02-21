using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using Tangra.Model.Helpers;
using Tangra.Model.Image;
using Tangra.Model.VideoOperations;

namespace Tangra.VideoOperations.LightCurves.InfoForms
{
    public class FrameBytePreProcessor : IFramePreProcessor
    {
        protected readonly uint[] m_MappedBytes;
        private readonly bool m_PreProcessFrames;

    	protected readonly int m_BitPix;
		protected readonly uint m_MaxPixelValue;

        protected FrameBytePreProcessor(bool preProcessFrames, int bitPix)
        {
            m_PreProcessFrames = preProcessFrames;
        	m_BitPix = bitPix;
        	m_MaxPixelValue = bitPix.GetMaxValueForBitPix();

			m_MappedBytes = new uint[m_MaxPixelValue + 1];
        }


        public void MapByteData(uint[,] data)
        {
            int width = data.GetLength(0);
            int height = data.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    data[x, y] = m_MappedBytes[data[x, y]];
                }
            }
        }

        #region IFramePreProcessor Members
        public void OnPreProcess(Pixelmap newFrame)
        {
        	Trace.Assert(newFrame.BitPixCamera <= 16);

			if (m_PreProcessFrames && 
				newFrame != null)
            {

				for (int y = 0; y < newFrame.Height; ++y)
				{
					for (int x = 0; x < newFrame.Width; ++x)
					{
						newFrame[x, y] = m_MappedBytes[newFrame[x, y]];
					}
				}				
            }
        }

        public uint[,] OnPreProcessPixels(uint[,] pixels)
		{
			if (m_PreProcessFrames &&
				pixels != null)
			{
				int width = pixels.GetLength(0);
				int height = pixels.GetLength(1);

				for (int y = 0; y < width; ++y)
				{
					for (int x = 0; x < height; ++x)
					{
						pixels[x, y] = m_MappedBytes[pixels[x, y]];
					}
				}
			}

			return pixels;
		}

    	#endregion
    }
}
