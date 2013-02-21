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

		public FrameDigitalFilter(TangraConfig.PreProcessingFilter filter, bool preProcessFrames)
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
					pixels = BitmapFilter.LowPassFilter(pixels);
				else if (m_Filter == TangraConfig.PreProcessingFilter.LowPassDifferenceFilter)
					pixels = BitmapFilter.LowPassDifferenceFilter(pixels, false);
			}

			return pixels;
		}

		#endregion
	}
}
