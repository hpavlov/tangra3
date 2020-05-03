/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.SDK;
using Tangra.VideoOperations.LightCurves;
using Tangra.VideoOperations.LightCurves.InfoForms;

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
            TimeCorrectonsInfo timeCorrectonsInfo;

            if (dontIncludeTimes || 
                !lcFile.Footer.ReductionContext.HasEmbeddedTimeStamps /* If the times are entered by the user, only include the times for the frames enterred by the user*/)
            {
                Timestamp = DateTime.MinValue;
                timeCorrectonsInfo = null;

				if ((int)frameNo == lcFile.Header.FirstTimedFrameNo)
					Timestamp = lcFile.Header.FirstTimedFrameTime;
				else if ((int)frameNo == lcFile.Header.LastTimedFrameNo)
					Timestamp = lcFile.Header.SecondTimedFrameTime;
            }
            else
            {
                Timestamp = lcFile.GetTimeForFrame(frameNo, out timeCorrectonsInfo);
            }

            IsCorrectedForInstrumentalDelay = lcFile.InstrumentalDelayCorrectionsNotRequired() || timeCorrectonsInfo != null;
            IsSuccessful = lcMeasurement.IsSuccessfulReading;
		}

        internal SingleMeasurement(frmLightCurve.BinnedValue binnedMeasurement, int targetNo, double binMiddleFrameNo, LCFile lcFile, bool dontIncludeTimes, int totalBins)
		{
			CurrFrameNo = (int)binnedMeasurement.BinNo;
			TargetNo = (byte)targetNo;
			Measurement = binnedMeasurement.IsSuccessfulReading ? (float)binnedMeasurement.AdjustedValue : INVALID_MEASUREMENT_VALUE;
			Background = binnedMeasurement.IsSuccessfulReading ? (float)binnedMeasurement.BackgroundValue : INVALID_MEASUREMENT_VALUE;
            TimeCorrectonsInfo timeCorrectonsInfo;
            if (dontIncludeTimes ||
				/* If the times are entered by the user, only include the times for the first and last bin derived from the frame times enterred by the user*/
				(!lcFile.Footer.ReductionContext.HasEmbeddedTimeStamps && binnedMeasurement.BinNo != 1 && binnedMeasurement.BinNo != totalBins))
            {
                Timestamp = DateTime.MinValue;
                timeCorrectonsInfo = null;
            }
            else
            {
                Timestamp = lcFile.GetTimeForFrame(binMiddleFrameNo, out timeCorrectonsInfo);
            }

            IsCorrectedForInstrumentalDelay = lcFile.InstrumentalDelayCorrectionsNotRequired() || timeCorrectonsInfo != null;
            IsSuccessful = binnedMeasurement.IsSuccessfulReading;
		}

		public int CurrFrameNo { get; private set; }

		public byte TargetNo { get; private set; }

		public float Measurement { get; private set; }

		public float Background { get; private set; }

        public DateTime Timestamp { get; private set; }

        public bool IsCorrectedForInstrumentalDelay { get; private set; }

        public bool IsSuccessful { get; private set; }
	}
}
