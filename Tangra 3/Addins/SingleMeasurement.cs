using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.SDK;
using Tangra.VideoOperations.LightCurves;

namespace Tangra.Addins
{
	[Serializable]
	public class SingleMeasurement : MarshalByRefObject, ISingleMeasurement
	{
		internal SingleMeasurement(LCMeasurement lcMeasurement)
		{
			CurrFrameNo = (int)lcMeasurement.CurrFrameNo;
			TargetNo = lcMeasurement.TargetNo;
			Measurement = lcMeasurement.TotalReading - lcMeasurement.TotalBackground;
		}

		public int CurrFrameNo { get; private set; }

		public byte TargetNo { get; private set; }

		public float Measurement { get; private set; }
	}
}
