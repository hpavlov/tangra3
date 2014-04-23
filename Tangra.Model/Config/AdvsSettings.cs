using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.Model.Config
{
	public class AdvsSettings
	{
		public bool OverlayTimestamp = false;

		public bool OverlayGain;
		public bool OverlayGamma;
		public bool OverlayAllMessages = true;
        public bool OverlayCameraInfo = true;
	    public bool OverlayAdvsInfo = false;
		public bool OverlayGeoLocation = true;

		public bool PopupTimestamp = true;
		public bool PopupExposure = true;
		public bool PopupVideoCameraFrameId = false;

		public bool PopupSystemTime;
		public bool PopupSatellites = true;
		public bool PopupOffset = true;
		public bool PopupGamma = true;
		public bool PopupGain = true;
		public bool PopupGPSFix = true;
		public bool PopupAlmanac = true;
	}
}
