using System;
using System.Collections.Generic;
using System.Drawing;
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
		ITrackedObjectPsfFit PSFFit { get; }
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

		public static double DistanceTo(this IImagePixel pixel1, IImagePixel pixel2)
		{
			return Math.Sqrt(Math.Pow(pixel1.XDouble - pixel2.XDouble, 2) + Math.Pow(pixel1.YDouble - pixel2.YDouble, 2));
		}

		public static double DistanceTo(this PSFFit psf1, PSFFit psf2)
		{
			return Math.Sqrt(Math.Pow(psf1.XCenter - psf2.XCenter, 2) + Math.Pow(psf1.YCenter - psf2.YCenter, 2));
		}
	}
	
}
