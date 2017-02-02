using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Tangra.MotionFitting
{
    public interface IMeasurementPositionProvider
    {
        IEnumerable<MeasurementPositionEntry> Measurements { get; }
        int NumberOfMeasurements { get; }
        string ObjectDesignation { get; }
        string ObservatoryCode { get; }
        DateTime ObservationDate { get; }
        decimal InstrumentalDelaySec { get; }
        double MinPositionUncertaintyPixels { get; }
        double ArsSecsInPixel { get; }
    }
}
