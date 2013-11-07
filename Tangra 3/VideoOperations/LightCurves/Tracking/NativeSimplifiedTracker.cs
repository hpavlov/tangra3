using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.PInvoke;

namespace Tangra.VideoOperations.LightCurves.Tracking
{
	public class NativeSimplifiedTracker : ITracker
	{
		private TrackedObjectConfig m_OccultedStarConfig;

		internal NativeSimplifiedTracker(int width, int height, List<TrackedObjectConfig> measuringStars)
		{
			NativeTracking.TrackerSettings(
				TangraConfig.Settings.Tracking.CheckElongation ? TangraConfig.Settings.Tracking.AdHokMaxElongation : 0,
				TangraConfig.Settings.Tracking.AdHokMinFWHM,
				TangraConfig.Settings.Tracking.AdHokMaxFWHM,
				TangraConfig.Settings.Tracking.AdHokMinCertainty);

			NativeTracking.TrackerNewConfiguration(
				width, 
				height, 
				measuringStars.Count, 
				LightCurveReductionContext.Instance.FullDisappearance);

			for (int i = 0; i < measuringStars.Count; i++)
			{
				TrackedObjectConfig obj = measuringStars[i];
				NativeTracking.ConfigureTrackedObject(i, obj);
			}

			m_OccultedStarConfig = measuringStars.Single(x => x.TrackingType == TrackingType.OccultedStar);
		}

		public bool IsTrackedSuccessfully { get; private set; }

		public void InitializeNewTracking()
		{
			throw new NotImplementedException();
		}

		public List<ITrackedObject> TrackedObjects
		{
			get { throw new NotImplementedException(); }
		}

		public float RefinedAverageFWHM
		{
			get { throw new NotImplementedException(); }
		}

		public float[] RefinedFWHM
		{
			get { throw new NotImplementedException(); }
		}

		public float PositionTolerance
		{
			get
			{
				return !float.IsNaN(RefinedAverageFWHM)
				  ? 2 * RefinedAverageFWHM
				  : m_OccultedStarConfig.PositionTolerance;
			}
		}

		public uint MedianValue
		{
			get { throw new NotImplementedException(); }
		}

		public float RefiningPercentageWorkLeft
		{
			get { return 0; }
		}

		public void DoManualFrameCorrection(int manualTrackingDeltaX, int manualTrackingDeltaY)
		{
			throw new NotImplementedException();
		}

		public void NextFrame(int frameNo, IAstroImage astroImage)
		{
			IsTrackedSuccessfully = NativeTracking.TrackNextFrame(frameNo, astroImage.GetPixelmapPixels());

			// TODO: 
		}

		public void BeginMeasurements(IAstroImage astroImage)
		{
			// Nothing special to do when beginning measurements
		}
	}
}
