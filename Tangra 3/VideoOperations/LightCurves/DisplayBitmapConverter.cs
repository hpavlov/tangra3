/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.VideoOperations.LightCurves
{
    public interface IDisplayBitmapConverter
    {
        byte ToDisplayBitmapByte(uint pixel);
        string GetConfig();
        void SetConfig(string config);
    }

    public class DisplayBitmapConverter : IDisplayBitmapConverter
    {
        public byte ToDisplayBitmapByte(uint pixel)
        {
            return 0;
        }

        public static IDisplayBitmapConverter Default = new DefaultDisplayBitmapConverter();

        public static IDisplayBitmapConverter Default12Bit = new TwelveBitDisplayBitmapConverter();


        internal class DefaultDisplayBitmapConverter : IDisplayBitmapConverter
        {

            public byte ToDisplayBitmapByte(uint pixel)
            {
                return (byte)(pixel & 0xFF);
            }

            public string GetConfig()
            {
                return string.Empty;
            }

            public void SetConfig(string config)
            { }
        }

        internal class TwelveBitDisplayBitmapConverter : IDisplayBitmapConverter
        {

            public byte ToDisplayBitmapByte(uint pixel)
            {
                return (byte)Math.Max(0, Math.Min(255, Math.Round(0xFF * (pixel * 1.0f / 0xFFF))));
            }

            public string GetConfig()
            {
                return string.Empty;
            }

            public void SetConfig(string config)
            { }
        }

		internal class FourteenBitDisplayBitmapConverter : IDisplayBitmapConverter
        {

            public byte ToDisplayBitmapByte(uint pixel)
            {
                return (byte)Math.Max(0, Math.Min(255, Math.Round(0xFF * (pixel * 1.0f / 0x3FFF))));
            }

            public string GetConfig()
            {
                return string.Empty;
            }

            public void SetConfig(string config)
            { }
        }

		internal class SixteenBitDisplayBitmapConverter : IDisplayBitmapConverter
		{
			private uint m_NormVal = uint.MaxValue;

			public SixteenBitDisplayBitmapConverter(uint normValue)
			{
				m_NormVal = normValue;
			}

			public byte ToDisplayBitmapByte(uint pixel)
			{
				return (byte)Math.Max(0, Math.Min(255, Math.Round(0xFF * (pixel * 1.0f / m_NormVal))));
			}

			public string GetConfig()
			{
				return string.Empty;
			}

			public void SetConfig(string config)
			{ }
		}

        public string GetConfig()
        {
            throw new NotImplementedException();
        }

        public void SetConfig(string config)
        {
            throw new NotImplementedException();
        }

        public static IDisplayBitmapConverter ConstructConverter(int bitPix, uint maxPixelValue, string config)
        {
            if (string.IsNullOrEmpty(config))
            {
                if (bitPix == 8)
                    return new DefaultDisplayBitmapConverter();
                else if (bitPix == 12)
                    return new TwelveBitDisplayBitmapConverter();
				else if (bitPix == 14)
					return new FourteenBitDisplayBitmapConverter();
				else if (bitPix == 16)
					return new SixteenBitDisplayBitmapConverter(maxPixelValue);				
            }

            throw new NotImplementedException();
        }
    }
}
