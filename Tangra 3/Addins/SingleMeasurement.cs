using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.SDK;
using Tangra.VideoOperations.LightCurves;

namespace Tangra.Addins
{
	[Serializable]
	public class SingleMeasurement : ISingleMeasurement
	{
        internal SingleMeasurement(LCMeasurement lcMeasurement, double frameNo, LCFile lcFile, bool dontIncludeTimes)
		{
			CurrFrameNo = (int)lcMeasurement.CurrFrameNo;
			TargetNo = lcMeasurement.TargetNo;
			Measurement = 1.0f * lcMeasurement.AdjustedReading;
            string isCorrectedForInstrumentalDelay;
            if (dontIncludeTimes)
            {
                Timestamp = DateTime.MinValue;
                isCorrectedForInstrumentalDelay = null;

				if ((int)frameNo == lcFile.Header.FirstTimedFrameNo)
					Timestamp = lcFile.Header.FirstTimedFrameTime;
				else if ((int)frameNo == lcFile.Header.LastTimedFrameNo)
					Timestamp = lcFile.Header.SecondTimedFrameTime;
            }
            else
            {
                Timestamp = lcFile.GetTimeForFrame(frameNo, out isCorrectedForInstrumentalDelay);
            }
            
            IsCorrectedForInstrumentalDelay = !string.IsNullOrEmpty(isCorrectedForInstrumentalDelay);
		}

        internal SingleMeasurement(frmLightCurve.BinnedValue binnedMeasurement, int targetNo, double binMiddleFrameNo, LCFile lcFile, bool dontIncludeTimes)
		{
			CurrFrameNo = (int)binnedMeasurement.BinNo;
			TargetNo = (byte)targetNo;
			Measurement = (float)binnedMeasurement.AdjustedValue;
            string isCorrectedForInstrumentalDelay;
            if (dontIncludeTimes)
            {
                Timestamp = DateTime.MinValue;
                isCorrectedForInstrumentalDelay = null;
            }
            else
            {
                Timestamp = lcFile.GetTimeForFrame(binMiddleFrameNo, out isCorrectedForInstrumentalDelay);
            }
            IsCorrectedForInstrumentalDelay = !string.IsNullOrEmpty(isCorrectedForInstrumentalDelay);
		}

		public int CurrFrameNo { get; private set; }

		public byte TargetNo { get; private set; }

		public float Measurement { get; private set; }

        public DateTime Timestamp { get; private set; }

        public bool IsCorrectedForInstrumentalDelay { get; private set; }
	}
}
