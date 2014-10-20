/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using Tangra.Helpers;
using Tangra.Model.Helpers;


namespace Tangra.VideoOperations.LightCurves.InfoForms
{
    public class FrameByteStretcher : FrameBytePreProcessor
    {
		private readonly ushort m_FromValue;
		private readonly ushort m_ToValue;
		public FrameByteStretcher(ushort fromValue, ushort toValue, bool preProcessFrames, int bitPix)
			: base(preProcessFrames, bitPix)
        {
			m_FromValue = fromValue;
			m_ToValue = toValue;

			uint maxPixelValue = bitPix.GetMaxValueForBitPix();

			Trace.Assert(maxPixelValue <= int.MaxValue, "32bit images not supported.");

			int magn = (int)Math.Round(256.0 / (m_ToValue - m_FromValue));

			for (int i = 0; i < m_FromValue; i++)
                m_MappedBytes[i] = 0;

			for (int i = m_FromValue; i < m_ToValue; i++)
                m_MappedBytes[i] = (uint)Math.Min(ushort.MaxValue, Math.Max((ushort)0, i * magn));

			for (int i = m_ToValue; i <= maxPixelValue; i++)
				m_MappedBytes[i] = maxPixelValue;     
        }
    }
}
