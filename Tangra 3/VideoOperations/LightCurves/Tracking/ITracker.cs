using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.Model.Astro;
using Tangra.Model.Image;

namespace Tangra.VideoOperations.LightCurves.Tracking
{
	public interface ITracker
	{
		bool IsTrackedSuccessfully { get; }
		void InitializeNewTracking();
		List<ITrackedObject> TrackedObjects { get; }
		float RefinedAverageFWHM { get; }
		float[] RefinedFWHM { get; }
		float PositionTolerance { get; }
		uint MedianValue { get; }
		float RefiningPercentageWorkLeft { get; }

		void DoManualFrameCorrection(int manualTrackingDeltaX, int manualTrackingDeltaY);
		void NextFrame(int frameNo, IAstroImage astroImage);
		void BeginMeasurements(IAstroImage astroImage);
	}
		
	public interface ITrackedObject
	{
		IImagePixel Center { get; }
		IImagePixel LastKnownGoodPosition { get; set; }
		bool IsLocated { get; }
		bool IsOffScreen { get; }
		ITrackedObjectConfig OriginalObject { get; }
		int TargetNo { get; }
		PSFFit PSFFit { get; }
	}

	public interface ITrackedObjectConfig
	{
		float ApertureInPixels { get; }	
		bool IsWeakSignalObject { get; }
		int PsfFitMatrixSize { get; }
		float RefinedFWHM { get; set; }
		float ApertureStartingX { get; }
		float ApertureStartingY { get; }
		TrackingType TrackingType { get; }
		bool IsFixedAperture { get; }
		ImagePixel AsImagePixel { get; }
		float PositionTolerance { get; }
		bool IsCloseToOtherStars { get; }
	}

	public static class TrackerExtensions
	{
		public static bool IsOcultedStar(this ITrackedObjectConfig cfg)
		{
			return cfg.TrackingType == TrackingType.OccultedStar;
		}
	}
	
}
