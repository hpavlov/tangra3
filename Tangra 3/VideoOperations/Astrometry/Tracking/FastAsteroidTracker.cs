using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Tangra.Astrometry;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Model.Helpers;
using Tangra.Model.Image;
using Tangra.VideoOperations.Astrometry.Engine;
using Tangra.VideoOperations.LightCurves.Tracking;

namespace Tangra.VideoOperations.Astrometry.Tracking
{
    public class FastAsteroidTracker : IAstrometryTracker
    {
        public bool IsTrackedSuccessfully { get; private set; }

        private TrackedAstrometricObjectConfig m_ObjectToTrack;

        public bool InitializeNewTracking(IAstroImage astroImage, TrackedAstrometricObjectConfig objectToTrack)
        {
            m_ObjectToTrack = objectToTrack;

            TrackedObject = new TrackedAstrometricObject()
            {
                LastKnownX = m_ObjectToTrack.StartingX,
                LastKnownY = m_ObjectToTrack.StartingY,
                RAHours = m_ObjectToTrack.RADeg / 15.0,
                DEDeg = m_ObjectToTrack.DEDeg
            };

            return true;
        }

        public TrackedAstrometricObject TrackedObject { get; private set; }

        public bool SupportsManualCorrections
        {
            get { return false; }
        }

        public void DoManualFrameCorrection(int targetId, int manualTrackingDeltaX, int manualTrackingDeltaY)
        { }

        public void NextFrame(int frameNo, IAstroImage astroImage, IStarMap starMap, LeastSquareFittedAstrometry astrometricFit)
        {
            IsTrackedSuccessfully = false;

            ImagePixel centroid = AstrometryContext.Current.StarMap.GetCentroid(
                (int)TrackedObject.LastKnownX,
                (int)TrackedObject.LastKnownY,
                CoreAstrometrySettings.Default.PreMeasureSearchCentroidRadius);

            //starMap.GetFeatureInRadius()
            //TODO: Use larger area for the fit, Use Features (first) for the tracking
            // TODO: Check signal detection setting ??

            if (centroid != null)
            {
                PSFFit psfFit;
                AstrometryContext.Current.StarMap.GetPSFFit(
                    centroid.X, centroid.Y, PSFFittingMethod.NonLinearFit, out psfFit);

                if (psfFit != null)
                {
                    double ra, de;
                    astrometricFit.GetRADEFromImageCoords(psfFit.XCenter, psfFit.YCenter, out ra, out de);

                    double maxPosDiffArcSec =
                            astrometricFit.GetDistanceInArcSec(astrometricFit.Image.CenterXImage, astrometricFit.Image.CenterYImage,
                            astrometricFit.Image.CenterXImage + CoreAstrometrySettings.Default.PreMeasureSearchCentroidRadius, astrometricFit.Image.CenterYImage);

                    if (!double.IsNaN(TrackedObject.RAHours))
                    {
                        double posDif = 3600 * AngleUtility.Elongation(15 * TrackedObject.RAHours, TrackedObject.DEDeg, ra, de);
                        if (posDif > maxPosDiffArcSec)
                        {
                            // NOTE: Not a valid measurement
                            Trace.WriteLine(string.Format("The target position is too far from the last measured position", posDif));
                            return;
                        }
                    }

                    TrackedObject.RAHours = ra / 15.0;
                    TrackedObject.DEDeg = de;
                    TrackedObject.LastKnownX = psfFit.XCenter;
                    TrackedObject.LastKnownY = psfFit.YCenter;
                    TrackedObject.PSFFit = psfFit;

                    IsTrackedSuccessfully = true;
                }
            }
        }

        public void BeginMeasurements(IAstroImage astroImage)
        { }
    }
}
