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
		internal SingleMeasurement(LCMeasurement lcMeasurement)
		{
			CurrFrameNo = (int)lcMeasurement.CurrFrameNo;
			TargetNo = lcMeasurement.TargetNo;
			Measurement = 1.0f * lcMeasurement.AdjustedReading;
		}

		internal SingleMeasurement(frmLightCurve.BinnedValue binnedMeasurement, int targetNo)
		{
			CurrFrameNo = (int)binnedMeasurement.BinNo;
			TargetNo = (byte)targetNo;
			Measurement = (float)binnedMeasurement.AdjustedValue;
		}

		public int CurrFrameNo { get; private set; }

		public byte TargetNo { get; private set; }

		public float Measurement { get; private set; }
	}
}
