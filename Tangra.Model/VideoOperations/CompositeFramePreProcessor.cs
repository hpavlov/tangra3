using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.Model.Image;

namespace Tangra.Model.VideoOperations
{
    public class CompositeFramePreProcessor : IFramePreProcessor
    {
        private List<IFramePreProcessor> m_PreProcessors = new List<IFramePreProcessor>();

        public void Clear()
        {
            m_PreProcessors.Clear();
        }

        public void AddPreProcessor(IFramePreProcessor preProcessor) 
        {
            m_PreProcessors.Add(preProcessor);
        }

        public void RemovePreProcessor(IFramePreProcessor preProcessor)
        {
            m_PreProcessors.Remove(preProcessor);
        }

        #region IFramePreProcessor Members

        public void OnPreProcess(Pixelmap newFrame)
        {
            if (m_PreProcessors.Count > 0)
            {
                foreach (IFramePreProcessor preProcessor in m_PreProcessors)
                {
                    preProcessor.OnPreProcess(newFrame);
                }                
            }
        }

        public uint[,] OnPreProcessPixels(uint[,] pixels)
		{
			if (m_PreProcessors.Count > 0)
			{
				foreach (IFramePreProcessor preProcessor in m_PreProcessors)
				{
					pixels = preProcessor.OnPreProcessPixels(pixels);
				}
			}

			return pixels;
		}

        #endregion
    }
}
