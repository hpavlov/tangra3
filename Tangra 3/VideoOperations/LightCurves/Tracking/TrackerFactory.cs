using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.Model.Config;
using Tangra.VideoOperations.LightCurves;
using Tangra.VideoOperations.LightCurves.Tracking;

namespace Tangra.VideoOperations.LightCurves.Tracking
{
    internal static class TrackerFactory
    {
		public static ITracker CreateTracker(int imageWidth, int imageHeight, LightCurveReductionType lightCurveReductionType, List<TrackedObjectConfig> measuringStars, out string usedTrackerType)
        {
            if (lightCurveReductionType == LightCurveReductionType.Asteroidal)
            {
				// NOTE: Figure out what tracker to create based on the type of event, number of objects and their intensity
				bool createRefiningTracker = TangraConfig.Settings.Tracking.SelectedEngine == TangraConfig.TrackingEngine.TrackingWithRefining;

				if (TangraConfig.Settings.Tracking.SelectedEngine == TangraConfig.TrackingEngine.LetTangraChoose)
				{
					if (LightCurveReductionContext.Instance.WindOrShaking ||
						LightCurveReductionContext.Instance.StopOnLostTracking ||
						LightCurveReductionContext.Instance.IsDriftThrough ||
						LightCurveReductionContext.Instance.HighFlickering ||
						(measuringStars.Count == 1 && LightCurveReductionContext.Instance.FullDisappearance))
					{
						createRefiningTracker = true;
					}
				}

	            if (createRefiningTracker)
	            {
		            if (measuringStars.Count == 1)
		            {
			            usedTrackerType = "One star tracking";
			            return new OneStarTracker(measuringStars);
		            }
		            else
		            {
						usedTrackerType = "Tracking with recovery";
			            return new OccultationTracker(measuringStars);
		            }
	            }
	            else
	            {
					if (TangraConfig.Settings.Tracking.UseNativeTracker)
					{
						usedTrackerType = "Simplified Native";
						return new NativeSimplifiedTracker(imageWidth, imageHeight, measuringStars, LightCurveReductionContext.Instance.FullDisappearance);
					}
					else
					{
						usedTrackerType = "Simplified";
						return new SimplifiedTracker(measuringStars);						
					}					
	            }
            }
            else if (lightCurveReductionType == LightCurveReductionType.UntrackedMeasurement)
            {
	            usedTrackerType = "Untracked";
                return new UntrackedTracker(measuringStars);
            }

            throw new NotSupportedException();
        }
    }
}
