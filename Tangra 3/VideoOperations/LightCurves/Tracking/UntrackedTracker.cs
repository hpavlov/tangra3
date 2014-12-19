/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using Tangra.Model.Astro;
using Tangra.Model.VideoOperations;

namespace Tangra.VideoOperations.LightCurves.Tracking
{
    internal class UntrackedTracker : Tracker
    {
        public UntrackedTracker(List<TrackedObjectConfig> measuringStars)
            : base(measuringStars)
        { }

        public override void NextFrame(int frameNo, IAstroImage astroImage)
        {
            // All objects are 'fixed' so always return the same positions
            foreach (TrackedObject trackedObject in TrackedObjects)
            {
                trackedObject.SetIsLocated(true, NotMeasuredReasons.TrackedSuccessfully);
                trackedObject.ThisFrameX = trackedObject.OriginalObject.ApertureStartingX;
                trackedObject.ThisFrameY = trackedObject.OriginalObject.ApertureStartingY;
                trackedObject.ThisFrameCertainty = trackedObject.OriginalObject.Gaussian != null ? (float)trackedObject.OriginalObject.Gaussian.Certainty : 0;
            }
        }

        public override bool IsTrackedSuccessfully
        {
            get
            {
                return true;
            }
        }

        public override int RefiningFramesRemaining
        {
            get
            {
                return 0;
            }
        }

        public override float RefiningPercentageWorkLeft
        {
            get
            {
                return 0;
            }
        }
    }
}
