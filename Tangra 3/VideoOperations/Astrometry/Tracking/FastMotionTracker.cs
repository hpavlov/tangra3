using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Tangra.Astrometry;
using Tangra.Controller;
using Tangra.Helpers;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Model.Helpers;
using Tangra.Model.Image;
using Tangra.Model.Numerical;
using Tangra.StarCatalogues;
using Tangra.VideoOperations.Astrometry.Engine;
using Tangra.VideoOperations.LightCurves.Tracking;

namespace Tangra.VideoOperations.Astrometry.Tracking
{
    public class FastMotionTracker : IAstrometryTracker
    {
        public bool IsTrackedSuccessfully { get; private set; }

        private TrackedAstrometricObjectConfig m_ObjectToTrack;
        private AstrometryController m_AstrometryController;
        private MeasurementContext m_MeasurementContext;
        private int m_RepeatedIntergationPositions = 1;
        private double m_LastMovementPixels;
        private List<IStar> m_LastFrameStars = new List<IStar>();

        private List<double> m_PastFramePosX = new List<double>();
        private List<double> m_PastFramePosY = new List<double>();
        private List<long> m_PastFrameTimes = new List<long>();
        private List<int> m_PastFrameNos = new List<int>();

        internal FastMotionTracker(AstrometryController astrometryController, MeasurementContext measurementContext)
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
            if (m_RepeatedIntergationPositions == 0) m_RepeatedIntergationPositions = 1;

            m_LastMovementPixels = 0;

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
            int searchRadius = (int)Math.Ceiling(Math.Max(m_LastMovementPixels, CoreAstrometrySettings.Default.PreMeasureSearchCentroidRadius));

            PSFFit psfFit = null;
            
            int startingX = (int)TrackedObject.LastKnownX;
            int startingY = (int)TrackedObject.LastKnownY;

            if (m_RepeatedIntergationPositions * 4 < m_PastFrameNos.Count)
            {
                var expectedPos = GetExpectedPosition(frameNo);
                if (expectedPos != null)
                {
                    startingX = expectedPos.X;
                    startingY = expectedPos.Y;
                }
            }

            var nearbyFeatures = starMap.GetFeaturesInRadius(startingX, startingY, searchRadius).ToArray();

            var nonStarNearbyFeature = new List<StarMapFeature>();
            foreach (var feature in nearbyFeatures)
            {
                var center = feature.GetCenter();
                var referenceStarFeatures = astrometricFit.FitInfo.AllStarPairs.Where(x => x.FitInfo.UsedInSolution).ToList();
                var refStar = referenceStarFeatures.FirstOrDefault(s => Math.Sqrt((s.x - center.X) * (s.x - center.X) + (s.y - center.Y) * (s.y - center.Y)) < 2);

                double raf, def;
                astrometricFit.GetRADEFromImageCoords(center.XDouble, center.YDouble, out raf, out def);
                var pastKnownStar = m_LastFrameStars.FirstOrDefault(s => AngleUtility.Elongation(s.RADeg, s.DEDeg, raf, def) * 3600.0 < 2);

                if (refStar == null && pastKnownStar == null) nonStarNearbyFeature.Add(feature);
            }

            if (nonStarNearbyFeature.Count > 0)
            {
                StarMapFeature closestFeature = nonStarNearbyFeature[0];
                var lastKnownCenter = new ImagePixel(TrackedObject.LastKnownX, TrackedObject.LastKnownY);
                var smallestDistance = lastKnownCenter.DistanceTo(closestFeature.GetCenter());

                for (int i = 1; i < nonStarNearbyFeature.Count; i++)
                {
                    var distance = lastKnownCenter.DistanceTo(nonStarNearbyFeature[i].GetCenter());
                    if (distance < smallestDistance)
                    {
                        smallestDistance = distance;
                        closestFeature = nonStarNearbyFeature[i];
                    }
                }

                if (closestFeature != null)
                {
                    var center = closestFeature.GetCenter();
                    AstrometryContext.Current.StarMap.GetPSFFit(center.X, center.Y, PSFFittingMethod.NonLinearFit, out psfFit);
                }
            }

            if (psfFit == null)
            {
                // The expected location cannot be matched with any brighter feature so it is likely a faint object
                // with no brighter objects around. Lets find the brightest (faint) object in the are and use it
                ImagePixel centroid = AstrometryContext.Current.StarMap.GetCentroid(startingX, startingY, searchRadius);

                if (centroid != null)
                    AstrometryContext.Current.StarMap.GetPSFFit(centroid.X, centroid.Y, PSFFittingMethod.NonLinearFit, out psfFit);
            }

            if (psfFit != null)
            {
                double ra, de;
                astrometricFit.GetRADEFromImageCoords(psfFit.XCenter, psfFit.YCenter, out ra, out de);

                double maxPosDiffArcSec =
                        astrometricFit.GetDistanceInArcSec(astrometricFit.Image.CenterXImage, astrometricFit.Image.CenterYImage,
                        astrometricFit.Image.CenterXImage + Math.Max(m_LastMovementPixels, CoreAstrometrySettings.Default.MaxAllowedDefaultMotionInPixels), astrometricFit.Image.CenterYImage);

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

                if (TrackedObject.PSFFit != null)
                {
                    m_LastMovementPixels = 1.2 * ImagePixel.ComputeDistance(TrackedObject.LastKnownX, psfFit.XCenter, TrackedObject.LastKnownY, psfFit.YCenter);
                }
                TrackedObject.LastKnownX = psfFit.XCenter;
                TrackedObject.LastKnownY = psfFit.YCenter;
                TrackedObject.PSFFit = psfFit;

                var lastKnownCenter = new ImagePixel(TrackedObject.LastKnownX, TrackedObject.LastKnownY);
                var thisFrameStars = astrometricFit.FitInfo.AllStarPairs.Where(x => lastKnownCenter.DistanceTo(x.x, x.y) > 2 * psfFit.FWHM).ToList();
                if (thisFrameStars.Count > 0)
                {
                    m_LastFrameStars = thisFrameStars.Select(x => new Star(x.StarNo, x.RADeg, x.DEDeg, x.Mag) as IStar).ToList();
                }

                IsTrackedSuccessfully = true;
            }

            if (psfFit != null && psfFit.XCenter > 0 && psfFit.YCenter > 0)
            {
                m_PastFramePosX.Add(psfFit.XCenter);
                m_PastFramePosY.Add(psfFit.YCenter);
                m_PastFrameNos.Add(frameNo);
            }
        }

        private ImagePixel GetExpectedPosition(int frameNo)
        {
            ImagePixel rv = null;

            var intervalValues = new Dictionary<int, List<ImagePixel>>();
            var intervalMedians = new Dictionary<int, ImagePixel>();

            int earliestFrame = m_PastFrameNos[0];
            for (int i = 0; i < m_PastFrameNos.Count; i++)
            {
                int integrationInterval = (m_PastFrameNos[i] - earliestFrame) / m_MeasurementContext.IntegratedFramesCount;

                List<ImagePixel> intPoints;
                if (!intervalValues.TryGetValue(integrationInterval, out intPoints))
                {
                    intPoints = new List<ImagePixel>();
                    intervalValues.Add(integrationInterval, intPoints);
                }

                intPoints.Add(new ImagePixel(m_PastFramePosX[i], m_PastFramePosY[i]));
            }

            var calcBucketX = new List<double>();
            var calcBucketY = new List<double>();
            foreach (int key in intervalValues.Keys)
            {
                calcBucketX.Clear();
                calcBucketY.Clear();

                intervalValues[key].ForEach(v =>
                {
                    calcBucketX.Add(v.XDouble);
                    calcBucketY.Add(v.YDouble);
                });

                double xMed = calcBucketX.Median();
                double yMed = calcBucketY.Median();

                intervalMedians.Add(key, new ImagePixel(xMed, yMed));
            }

            var xMotion = new LinearRegression();
            var yMotion = new LinearRegression();

            foreach (int intInt in intervalMedians.Keys)
            {
                long t = intInt;
                double x = intervalMedians[intInt].XDouble;
                double y = intervalMedians[intInt].YDouble;
                if (x > 0 && y > 0)
                {
                    xMotion.AddDataPoint(t, x);
                    yMotion.AddDataPoint(t, y);
                }
            }

            try
            {
                xMotion.Solve();
                yMotion.Solve();

                int currIntInterval = (frameNo - earliestFrame) / m_MeasurementContext.IntegratedFramesCount;

                rv = new ImagePixel(xMotion.ComputeY(currIntInterval), yMotion.ComputeY(currIntInterval));
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.GetFullStackTrace());
            }

            return rv;
            
        }

        public void BeginMeasurements(IAstroImage astroImage)
        { }
    }
}
