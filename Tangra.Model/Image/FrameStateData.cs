using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.Model.Image
{
	public struct FrameStateData
	{
		public long VideoCameraFrameId;
		public DateTime CentralExposureTime;
		public DateTime SystemTime;
		public float ExposureInMilliseconds;
		public float Gamma;
		public float Gain;
		public float Offset;
		public string Messages;
		public int NumberSatellites;
		public string AlmanacStatus;
		public string AlmanacOffset;
		public string GPSFixStatus;
		public int? NumberIntegratedFrames;

		public bool IsEmpty()
		{
			return
				CentralExposureTime == DateTime.MinValue &&
				SystemTime == DateTime.MinValue &&
				Gamma == 0 &&
				Gain == 0 &&
				Messages == null;
		}

        public bool HasValidTimeStamp
        {
            get
            {
                return CentralExposureTime.Ticks != 633979008000000000;        
            }            
        }

	    public bool IsGainKnown
	    {
	        get
	        {
	            return Gain != -100;
	        }
	    }
	}
}
