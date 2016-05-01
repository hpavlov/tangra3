using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Tangra.Astrometry;
using Tangra.Controller;
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
        private AstrometryController m_AstrometryController;
        private MeasurementContext m_MeasurementContext;
        private int m_RepeatedIntergationPositions = 1;

        private List<double> m_PastFramePosX = new List<double>();
        private List<double> m_PastFramePosY = new List<double>();
        private List<long> m_PastFrameTimes = new List<long>();

        internal FastAsteroidTracker(AstrometryController astrometryController, MeasurementContext measurementContext)
        {
            m_AstrometryController = astrometryController;
            m_MeasurementContext = measurementContext;
        }

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

            m_RepeatedIntergationPositions = m_MeasurementContext.IntegratedFramesCount / m_MeasurementContext.FrameInterval;

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

            PSFFit psfFit = null;

            var frameTime = m_AstrometryController.GetTimeForFrame(m_MeasurementContext, frameNo, frameNo);

            if (m_RepeatedIntergationPositions * 4 > m_PastFrameTimes.Count)
            {
                var expectedPos = GetExpectedPosition(frameTime.UT.Ticks);
                if (expectedPos != null)
                    AstrometryContext.Current.StarMap.GetPSFFit(expectedPos.X, expectedPos.Y, PSFFittingMethod.NonLinearFit, out psfFit);
            }

            if (psfFit == null)
            {
                var brightestFeature = starMap.GetFeatureInRadius((int)TrackedObject.LastKnownX, (int)TrackedObject.LastKnownY, CoreAstrometrySettings.Default.PreMeasureSearchCentroidRadius);

                if (brightestFeature != null)
                {
                    var center = brightestFeature.GetCenter();
                    var referenceStarFeatures = astrometricFit.FitInfo.AllStarPairs.Where(x => x.FitInfo.UsedInSolution).ToList();
                    var refStar = referenceStarFeatures.FirstOrDefault(s => Math.Sqrt((s.x - center.X) * (s.x - center.X) + (s.y - center.Y) * (s.y - center.Y)) < 2);
                    if (refStar == null)
                        // The brightest feature is not a reference star, so we assume it is our object
                        AstrometryContext.Current.StarMap.GetPSFFit(center.X, center.Y, PSFFittingMethod.NonLinearFit, out psfFit);
                }
            }

            if (psfFit == null)
            {
                ImagePixel centroid = AstrometryContext.Current.StarMap.GetCentroid(
                    (int)TrackedObject.LastKnownX,
                    (int)TrackedObject.LastKnownY,
                    CoreAstrometrySettings.Default.PreMeasureSearchCentroidRadius);

                if (centroid != null)
                    AstrometryContext.Current.StarMap.GetPSFFit(centroid.X, centroid.Y, PSFFittingMethod.NonLinearFit, out psfFit);
            }

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

            m_PastFramePosX.Add(psfFit != null ? psfFit.XCenter : -1);
            m_PastFramePosY.Add(psfFit != null ? psfFit.YCenter : -1);
            m_PastFrameTimes.Add(frameTime.UT.Ticks); 
        }

        private ImagePixel GetExpectedPosition(long ticks)
        {
            // TODO: Implement this. Check for Position (-1, -1) and exclude from calculations
            //       Group by m_RepeatedIntergationPositions measurements and average position. See what to do with time of group measurements
            return null;
        }

        public void BeginMeasurements(IAstroImage astroImage)
        { }
    }
}
