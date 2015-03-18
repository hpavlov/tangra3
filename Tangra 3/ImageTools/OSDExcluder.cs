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

		internal void SetAreaType(AreaType areaType)
		{
			Rectangle userFrame;

			LimitByInclusion = areaType == AreaType.Inclusion;
		    if (LimitByInclusion)
		        userFrame = TangraConfig.Settings.PlateSolve.SelectedScopeRecorderConfig.InclusionArea;
            else
                userFrame = TangraConfig.Settings.PlateSolve.SelectedScopeRecorderConfig.OSDExclusionArea;

			SetUserFrame(userFrame);
		}
    }
}
