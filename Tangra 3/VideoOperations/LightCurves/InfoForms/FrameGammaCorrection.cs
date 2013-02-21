using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.VideoOperations.LightCurves.InfoForms
{
	public class FrameGammaCorrection : FrameBytePreProcessor
	{
		private readonly double m_EncodingGamma;

		public FrameGammaCorrection(double encodingGamma, bool preProcessFrames, int bitPix)
			: base(preProcessFrames, bitPix)
		{
			m_EncodingGamma = encodingGamma;

			double decodingGamma = 1 / m_EncodingGamma;

			if (bitPix == 8 || bitPix == 12 || bitPix == 16)
			{
				for (int i = 0; i <= m_MaxPixelValue; i++)
					m_MappedBytes[i] = (byte)Math.Max(0, Math.Min(m_MaxPixelValue, Math.Round((m_MaxPixelValue + 1) * Math.Pow(i * 1.0/ (m_MaxPixelValue + 1), decodingGamma))));				
			}
			else 
				throw new NotSupportedException("Only 8bpp, 12bpp and 16bpp are supported");

		}
	}
}
