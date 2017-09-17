using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using nom.tam.fits;
using Tangra.Model.Config;
using Tangra.Model.Helpers;
using Tangra.VideoOperations.LightCurves;

namespace Tangra.Video.FITS
{
    public class LCFITSTimeStampReader : IFITSTimeStampReader
    {
        private LCFile m_LCFile;
        private Dictionary<string, uint> m_FitsIndex = new Dictionary<string, uint>();

        internal LCFITSTimeStampReader(LCFile lcFile)
        {
            m_LCFile = lcFile;

            var files = lcFile.Data[0].Select(x => x.CurrFileName).ToArray();
            var frameNos = lcFile.Data[0].Select(x => x.CurrFrameNo).ToArray();

            if (m_LCFile.CanDetermineFrameTimes)
            {
                for (int i = 0; i < files.Length; i++)
                {
                    m_FitsIndex.Add(files[i].ToLower(), frameNos[i]);
                }
            }
            else
            {
                m_FitsIndex = null;
            }
        }

        public DateTime? ParseExposure(string fileName, Header header, out bool isMidPoint, out double? fitsExposure)
        {
            isMidPoint = true;
            fitsExposure = null;
            if (m_FitsIndex == null) return null;

            string lowerFileNameOnly = Path.GetFileName(fileName);
            if (string.IsNullOrWhiteSpace(lowerFileNameOnly))
                return null;

            uint frameNo;
            if (m_FitsIndex.TryGetValue(lowerFileNameOnly, out frameNo) &&
                m_LCFile.CanDetermineFrameTimes)
            {
                string correctedForInstrumentalDelayMessage;
                return m_LCFile.GetTimeForFrame(frameNo, out correctedForInstrumentalDelayMessage);
            }
            return null;
        }
    }
}
