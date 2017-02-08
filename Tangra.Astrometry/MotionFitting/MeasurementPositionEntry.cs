using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.MotionFitting
{
    public class MeasurementPositionEntry
    {
        public int FrameNo;
        public string RawTimeStamp;
        public double TimeOfDayUTC;
        public double RADeg;
        public double DEDeg;
        public double Mag;
        public double SolutionUncertaintyRACosDEArcSec;
        public double SolutionUncertaintyDEArcSec;
        public double FWHMArcSec;
        public double DetectionCertainty;
        public double SNR;
        public bool ConstraintPoint;
        public bool MidConstraintPoint;
    }
}
