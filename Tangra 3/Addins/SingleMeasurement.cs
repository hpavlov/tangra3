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
        internal SingleMeasurement(LCMeasurement lcMeasurement, double frameNo, LCFile lcFile)
		{
			CurrFrameNo = (int)lcMeasurement.CurrFrameNo;
			TargetNo = lcMeasurement.TargetNo;
			Measurement = 1.0f * lcMeasurement.AdjustedReading;
            string isCorrectedForInstrumentalDelay;
            Timestamp = lcFile.GetTimeForFrame(frameNo, out isCorrectedForInstrumentalDelay);
            IsCorrectedForInstrumentalDelay = !string.IsNullOrEmpty(isCorrectedForInstrumentalDelay);
		}

        internal SingleMeasurement(frmLightCurve.BinnedValue binnedMeasurement, int targetNo, double binMiddleFrameNo, LCFile lcFile)
		{
			CurrFrameNo = (int)binnedMeasurement.BinNo;
			TargetNo = (byte)targetNo;
			Measurement = (float)binnedMeasurement.AdjustedValue;
            string isCorrectedForInstrumentalDelay;
            Timestamp = lcFile.GetTimeForFrame(binMiddleFrameNo, out isCorrectedForInstrumentalDelay);
            IsCorrectedForInstrumentalDelay = !string.IsNullOrEmpty(isCorrectedForInstrumentalDelay);
		}

		public int CurrFrameNo { get; private set; }

		public byte TargetNo { get; private set; }

		public float Measurement { get; private set; }

        public DateTime Timestamp { get; private set; }

        public bool IsCorrectedForInstrumentalDelay { get; private set; }
	}
}
