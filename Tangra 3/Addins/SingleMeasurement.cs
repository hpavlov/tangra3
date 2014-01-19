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
		private static float INVALID_MEASUREMENT_VALUE = float.NaN;

        internal SingleMeasurement(LCMeasurement lcMeasurement, double frameNo, LCFile lcFile, bool dontIncludeTimes)
		{
			CurrFrameNo = (int)lcMeasurement.CurrFrameNo;
			TargetNo = lcMeasurement.TargetNo;
			Measurement = lcMeasurement.IsSuccessfulReading ? 1.0f * lcMeasurement.AdjustedReading : INVALID_MEASUREMENT_VALUE;
			Background = lcMeasurement.IsSuccessfulReading 
				// NOTE: Make sure negative backgrounds are sent as negative values (not as the serialized UINTs)
				? 1.0f * (int)lcMeasurement.TotalBackground 
				: INVALID_MEASUREMENT_VALUE;
            string isCorrectedForInstrumentalDelay;

            if (dontIncludeTimes || 
                !lcFile.Footer.ReductionContext.HasEmbeddedTimeStamps /* If the times are entered by the user, only include the times for the frames enterred by the user*/)
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
			Measurement = binnedMeasurement.IsSuccessfulReading ? (float)binnedMeasurement.AdjustedValue : INVALID_MEASUREMENT_VALUE;
			Background = binnedMeasurement.IsSuccessfulReading ? (float)binnedMeasurement.BackgroundValue : INVALID_MEASUREMENT_VALUE;
            string isCorrectedForInstrumentalDelay;
            if (dontIncludeTimes || 
                !lcFile.Footer.ReductionContext.HasEmbeddedTimeStamps /* If the times are entered by the user, only include the times for the frames enterred by the user*/)
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

		public float Background { get; private set; }

        public DateTime Timestamp { get; private set; }

        public bool IsCorrectedForInstrumentalDelay { get; private set; }
	}
}
