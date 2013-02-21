using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Tangra.Helpers;
using Tangra.Model.Helpers;

namespace Tangra.VideoOperations.LightCurves.InfoForms
{
    public class FrameByteClipper : FrameBytePreProcessor
    {
		private readonly ushort m_FromValue;
		private readonly ushort m_ToValue;

        public FrameByteClipper(ushort fromValue, ushort toValue, bool preProcessFrames, int bitPix)
			: base(preProcessFrames, bitPix)
        {
			m_FromValue = fromValue;
			m_ToValue = toValue;

			uint maxPixelValue = bitPix.GetMaxValueForBitPix();

			Trace.Assert(maxPixelValue <= int.MaxValue, "32bit images not supported.");

			for (uint i = 0; i < m_FromValue; i++)
                m_MappedBytes[i] = 0;

			for (uint i = m_FromValue; i < m_ToValue; i++)
                m_MappedBytes[i] = i;

			for (uint i = m_ToValue; i <= maxPixelValue; i++)
				m_MappedBytes[i] = maxPixelValue;
        }
    }
}
