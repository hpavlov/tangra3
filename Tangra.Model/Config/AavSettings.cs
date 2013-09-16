using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.Model.Config
{
	public class AavSettings
	{
		public bool OverlayTimestamp = false;

		public bool OverlayAllMessages = true;
		public bool OverlayCameraInfo = true;
		public bool OverlayAdvsInfo = false;

		public bool PopupTimestamp = true;
		public bool PopupExposure = true;

		public bool PopupSystemTime;
		public bool PopupSatellites = true;
		public bool PopupGPSFix = true;
		public bool PopupAlmanac = true;

		public bool SplitFieldsOSD = true;
		public int SplitFieldsOSDParity = 1;
	}
}
