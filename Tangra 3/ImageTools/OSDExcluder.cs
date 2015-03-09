/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Tangra.Config;
using Tangra.Controller;
using Tangra.Model.Config;
using Tangra.Model.Context;

namespace Tangra.ImageTools
{
    internal class OSDExcluder : FrameSizer
    {
        private int m_VideoWidth;
        private int m_VideoHeight;

		public OSDExcluder(VideoController videoController)
			: base(videoController)
        {
            m_VideoWidth = TangraContext.Current.FrameWidth;
			m_VideoHeight = TangraContext.Current.FrameHeight;
            
        }

        public override void Activate()
        {
	        LimitByInclusion = false;
            
            base.Activate();
        }

        public override void Deactivate()
        {
            // Save the modified frame
	        if (LimitByInclusion)
		        TangraConfig.Settings.OSDSizes.AddOrUpdateOSDRectangleForFrameSize(m_VideoWidth, m_VideoHeight, m_UserFrame);
	        else
				TangraConfig.Settings.IncludeAreaSizes.AddOrUpdateInclusionRectangleForFrameSize(m_VideoWidth, m_VideoHeight, m_UserFrame);

			TangraConfig.Settings.Save();

            base.Deactivate();
        }

		internal void SetAreaType(AreaType areaType)
		{
			// This will load the correct area in the m_UserFrame too
			LimitByInclusion = areaType == AreaType.Inclusion;
		}
    }
}
