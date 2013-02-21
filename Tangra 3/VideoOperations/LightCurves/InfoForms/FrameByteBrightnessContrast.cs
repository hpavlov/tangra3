using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using Tangra.Helpers;
using Tangra.Model.Helpers;
using Tangra.Model.Image;

namespace Tangra.VideoOperations.LightCurves.InfoForms
{
    public class FrameByteBrightnessContrast : FrameBytePreProcessor
    {
        private readonly int m_Brigtness;
        private readonly sbyte m_Contrast;

        public FrameByteBrightnessContrast(int brigtness, int contrast, bool preProcessFrames, int bitPix)
            : base(preProcessFrames, bitPix)
        {
            m_Brigtness = brigtness;
            m_Contrast = (sbyte)contrast;

			uint maxPixelValue = bitPix.GetMaxValueForBitPix();

			Trace.Assert(maxPixelValue <= int.MaxValue, "32bit images not supported.");

			Pixelmap image = new Pixelmap((int)maxPixelValue + 1, 1, bitPix, new uint[maxPixelValue + 1], null, null);
            {
				for (int i = 0; i <= maxPixelValue; i++)
                    image[i, 0] = (uint)i;

                BitmapFilter.Brightness(image, m_Brigtness);
                BitmapFilter.Contrast(image, m_Contrast);

                for (int i = 0; i <= maxPixelValue; i++)
                    m_MappedBytes[i] = image[i, 0];
            }
        }
    }
}
