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

        public override void NextFrame(int frameNo, AstroImage astroImage)
        {
            // All objects are 'fixed' so always return the same positions
            foreach (TrackedObject trackedObject in TrackedObjects)
            {
                trackedObject.SetIsLocated(true, NotMeasuredReasons.TrackedSuccessfully);
                trackedObject.ThisFrameX = trackedObject.OriginalObject.ApertureStartingX;
                trackedObject.ThisFrameY = trackedObject.OriginalObject.ApertureStartingY;
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
