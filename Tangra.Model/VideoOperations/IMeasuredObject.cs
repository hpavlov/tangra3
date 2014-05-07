using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.Model.Astro;
using Tangra.Model.Image;

namespace Tangra.Model.VideoOperations
{
    [Flags]
    public enum NotMeasuredReasons
    {
        /* First Level Reasons */
        UnknownReason = 0,
        TrackedSuccessfully = 1,
		MeasuredSuccessfully = 1,
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
        FWHMOutOfRange = 4 << 16,


		/* Light Tracker Reasons */
		ObjectTooElongated = 1 << 24,
		ObjectCertaintyTooSmall = 2 << 24,
    }

	public interface IMeasurableObject
	{
		bool IsOccultedStar { get; }
		bool MayHaveDisappeared { get; }
		int PsfFittingMatrixSize { get; }
		int PsfGroupId { get; }
		IImagePixel Center { get; }
	}
}
