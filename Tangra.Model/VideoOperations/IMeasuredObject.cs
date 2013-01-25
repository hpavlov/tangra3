using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.Model.Astro;

namespace Tangra.Model.VideoOperations
{
    [Flags]
    public enum NotMeasuredReasons
    {
        /* First Level Reasons */
        UnknownReason = 0,
        TrackedSuccessfully = 1,
        FixedObject = 2,
        GuidingStarBrightnessFluctoationTooHigh = 3,
        PSFFittingFailed = 4,
        FoundObjectNotWithInExpectedPositionTolerance = 5,
        FullyDisappearingStarMarkedTrackedWithoutBeingFound = 6,
        FitSuspectAsNoGuidingStarsAreLocated = 7,
        ObjectExpectedPositionIsOffScreen = 8,

        LAST_FIRST_LEVEL_REASON = 15,

        /* Second Level Reasons */
        TrackedSuccessfullyAfterDistanceCheck = 1 << 8,
        TrackedSuccessfullyAfterWiderAreaSearch = 2 << 8,
        TrackedSuccessfullyAfterStarRecognition = 3 << 8,
        FailedToLocateAfterDistanceCheck = 4 << 8,
        FailedToLocateAfterWiderAreaSearch = 5 << 8,
        FailedToLocateAfterStarRecognition = 6 << 8,

        /* Third Level Reasons */
        NoPixelsToMeasure = 1 << 16,
        MeasurementPSFFittingFailed = 2 << 16,
        DistanceToleranceTooHighForNonFullDisappearingOccultedStar = 3 << 16,
        FWHMOutOfRange = 4 << 16
    }

    public interface IMeasuredObject
    {
        void SetIsMeasured(bool isMeasured, NotMeasuredReasons reason);
        bool IsOcultedStar { get; }
        int PsfFitMatrixSize { get; }
        double AppMeaAveragePixel { get; set; }
        double ApertureArea { get; set; }
        float RefinedOrLastSignalLevel { get; }
        PSFFit PSFFit { get; }
    }
}
