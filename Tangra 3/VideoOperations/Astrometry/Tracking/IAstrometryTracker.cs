using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.Astrometry;
using Tangra.Model.Astro;
using Tangra.VideoOperations.LightCurves.Tracking;

namespace Tangra.VideoOperations.Astrometry.Tracking
{
    public class TrackedAstrometricObject
    {
        public PSFFit PSFFit { get; set; }
        public double RAHours { get; set; }
        public double DEDeg { get; set; }
        public double LastKnownX { get; set; }
        public double LastKnownY { get; set; }
    }

    public class TrackedAstrometricObjectConfig
    {
        public PSFFit Gaussian { get; set; }
        public double StartingX { get; set; } 
        public double StartingY { get; set; }
        public double RADeg { get; set; }
        public double DEDeg { get; set; }
    }

    public interface IAstrometryTracker
    {
        bool IsTrackedSuccessfully { get; }
        bool SupportsManualCorrections { get; }
        bool InitializeNewTracking(IAstroImage astroImage, TrackedAstrometricObjectConfig objectToTrack);
        TrackedAstrometricObject TrackedObject { get; }
        void NextFrame(int frameNo, IAstroImage astroImage, IStarMap starMap, LeastSquareFittedAstrometry astrometricFit);
    }
}
