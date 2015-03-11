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
		public OSDExcluder(VideoController videoController)
			: base(videoController)
        { }

        public override void Activate()
        {
	        LimitByInclusion = false;
            
            base.Activate();
        }

        public override void Deactivate()
        {
            // Save the modified frame
            if (LimitByInclusion)
            {
                TangraConfig.Settings.PlateSolve.SelectedScopeRecorderConfig.InclusionArea = m_UserFrame;
                TangraConfig.Settings.PlateSolve.SelectedScopeRecorderConfig.IsInclusionArea = true;
            }
            else
            {
                TangraConfig.Settings.PlateSolve.SelectedScopeRecorderConfig.OSDExclusionArea = m_UserFrame;
                TangraConfig.Settings.PlateSolve.SelectedScopeRecorderConfig.IsInclusionArea = false;
            }

			TangraConfig.Settings.Save();

            base.Deactivate();
        }

		internal void SetAreaType(AreaType areaType)
		{
			LimitByInclusion = areaType == AreaType.Inclusion;
		    if (LimitByInclusion)
		        m_UserFrame = TangraConfig.Settings.PlateSolve.SelectedScopeRecorderConfig.InclusionArea;
            else
                m_UserFrame = TangraConfig.Settings.PlateSolve.SelectedScopeRecorderConfig.OSDExclusionArea;
		}
    }
}
