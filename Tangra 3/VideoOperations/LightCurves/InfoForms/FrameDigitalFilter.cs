using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.Model.Config;
using Tangra.Model.Image;
using Tangra.Model.VideoOperations;


namespace Tangra.VideoOperations.LightCurves.InfoForms
{
	public class FrameDigitalFilter : IFramePreProcessor
	{
        private readonly bool m_PreProcessFrames;
		private readonly TangraConfig.PreProcessingFilter m_Filter;
		private readonly int m_Bpp;

		public FrameDigitalFilter(TangraConfig.PreProcessingFilter filter, bool preProcessFrames, int bpp)
        {
            m_PreProcessFrames = preProcessFrames;
			m_Filter = filter;
        }

		#region IFramePreProcessor Members

		public void OnPreProcess(Pixelmap newFrame)
		{
			if (m_PreProcessFrames &&
				newFrame != null)
			{
				if (m_Filter == TangraConfig.PreProcessingFilter.LowPassFilter)
					BitmapFilter.LowPassFilter(newFrame);
				else if (m_Filter == TangraConfig.PreProcessingFilter.LowPassDifferenceFilter)
					BitmapFilter.LowPassDifference(newFrame);
			}
		}

		public uint[,] OnPreProcessPixels(uint[,] pixels)
		{
			if (m_PreProcessFrames &&
				pixels != null)
			{
				if (m_Filter == TangraConfig.PreProcessingFilter.LowPassFilter)
					pixels = BitmapFilter.LowPassFilter(pixels, m_Bpp);
				else if (m_Filter == TangraConfig.PreProcessingFilter.LowPassDifferenceFilter)
					pixels = BitmapFilter.LowPassDifferenceFilter(pixels, m_Bpp, false);
			}

			return pixels;
		}

		#endregion
	}
}
