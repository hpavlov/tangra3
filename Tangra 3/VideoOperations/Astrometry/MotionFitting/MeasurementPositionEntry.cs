using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.VideoOperations.Astrometry.MotionFitting
{
    public class MeasurementPositionEntry
    {
        public int FrameNo;
        public double TimeOfDayUTC;
        public double RADeg;
        public double DEDeg;
        public double Mag;
        public double SolutionUncertaintyRACosDEArcSec;
        public double SolutionUncertaintyDEArcSec;
        public double FWHM;
        public double DetectionCertainty;
        public double SNR;
    }
}
