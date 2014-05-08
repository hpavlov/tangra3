using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.Model.Config
{
	public class AavSettings
	{
		public bool Overlay_Timestamp = false;

		public bool Overlay_AllMessages = true;
		public bool Overlay_CameraInfo = true;
		public bool Overlay_AdvsInfo = true;

		public bool Popup_Timestamp = false;
		public bool Popup_Exposure = false;

		public bool Popup_SystemTime = true;
		public bool Popup_Satellites = false;
		public bool Popup_GPSFix = false;
		public bool Popup_Almanac = false;

		public bool SplitFieldsOSD = true;

		public bool NtpTimeDebugFlag = false;
	}
}
