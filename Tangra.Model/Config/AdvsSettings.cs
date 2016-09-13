/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.Model.Config
{
	public class AdvsSettings
	{
		public bool OverlayTimestamp = false;

		public bool OverlayAllMessages = true;
        public bool OverlayCameraInfo = true;
	    public bool OverlayAdvsInfo = false;
		public bool OverlayGeoLocation = true;
	    public bool OverlayObjectName = true;

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
