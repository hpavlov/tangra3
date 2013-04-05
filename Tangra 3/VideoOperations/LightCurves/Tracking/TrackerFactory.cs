using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.VideoOperations.LightCurves;
using Tangra.VideoOperations.LightCurves.Tracking;

namespace Tangra.VideoOperations.LightCurves.Tracking
{
    internal static class TrackerFactory
    {
        public static Tracker CreateTracker(LightCurveReductionType lightCurveReductionType, List<TrackedObjectConfig> measuringStars)
        {
            // NOTE: Figure out what tracker to create based on the type of event, number of objects and their intencity

            if (lightCurveReductionType == LightCurveReductionType.Asteroidal)
            {
                if (measuringStars.Count == 1)
                    return new OneStarTracker(measuringStars);
                else
                    return new OccultationTracker(measuringStars);

            }
            else if (lightCurveReductionType == LightCurveReductionType.UntrackedMeasurement)
            {
                return new UntrackedTracker(measuringStars);
            }

            throw new NotSupportedException();
        }
    }
}
