/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Tangra.Model.Config
{
	public class AavSettings
	{
		public bool Overlay_Timestamp = false;

		public bool Overlay_AllMessages = true;
		public bool Overlay_CameraInfo = true;
		public bool Overlay_AdvsInfo = true;
        public bool Overlay_ObjectName = true;

		public bool Popup_Timestamp = false;
		public bool Popup_Exposure = false;

		public bool Popup_Gain = true;
		public bool Popup_Gamma = true;
		public bool Popup_Temperature = true;

		public bool Popup_SystemTime = true;
		public bool Popup_Satellites = false;
		public bool Popup_GPSFix = false;
		public bool Popup_Almanac = false;

		public bool Popup_NtpTimestamp = false;

		public bool SplitFieldsOSD = true;

		public bool NtpTimeDebugFlag = false;

		public bool NtpTimeUseDirectTimestamps = false;

	    public string ConvertOsdPositionPerFrameSize
	    {
	        get
	        {
	            var output = new StringBuilder();

	            foreach (var key in m_OsdPositionsPerFrameSize.Keys)
	            {
	                var val = m_OsdPositionsPerFrameSize[key];
	                output.AppendFormat("{0},{1};{2},{3}|", key.Item1, key.Item2, val.Item1, val.Item2);
	            }

	            return output.ToString();
	        }
	        set
	        {
	            m_OsdPositionsPerFrameSize.Clear();

	            if (value != null)
	            {
	                var entries = value.Split('|');
	                foreach (string entry in entries)
	                {
	                    var data = entry.Split(';');
	                    if (data.Length == 2)
	                    {
	                        var keys = data[0].Split(',');
                            var vals = data[1].Split(',');
	                        if (keys.Length == 2 && vals.Length == 2)
	                        {
	                            m_OsdPositionsPerFrameSize.Add(
                                    Tuple.Create(int.Parse(keys[0]), int.Parse(keys[1])),
	                                Tuple.Create(int.Parse(vals[0]), int.Parse(vals[1])));
	                        }
	                    }
	                }
	            }
	        }
	    }

        private Dictionary<Tuple<int, int>, Tuple<int, int>> m_OsdPositionsPerFrameSize = new Dictionary<Tuple<int, int>, Tuple<int, int>>();
 
	    public void RegisterOsdPosition(int width, int height, int osdTopLine, int osdBottomLine)
	    {
	        m_OsdPositionsPerFrameSize[Tuple.Create(width, height)] = Tuple.Create(osdTopLine, osdBottomLine);
	    }

	    public Tuple<int, int> GetLastOsdPositionForFrameSize(int width, int height)
	    {
	        var key = Tuple.Create(width, height);
	        Tuple<int, int> rv;
	        if (m_OsdPositionsPerFrameSize.TryGetValue(key, out rv))
	            return rv;
	        else
	            return null;
	    }
	}
}
