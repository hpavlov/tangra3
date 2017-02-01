using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.VideoOperations.Astrometry.MotionFitting
{
    public interface IMeasurementPositionProvider
    {
        IEnumerable<MeasurementPositionEntry> Measurements { get; }
        string ObjectDesignation { get; }
        string ObservatoryCode { get; }
        DateTime ObservationDate { get; }
        decimal InstrumentalDelay { get; }
    }
}
