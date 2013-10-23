using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.VideoOperations.LightCurves.Tracking
{
	internal class AdHocTracker : ITracker
	{
		internal AdHocTracker(List<TrackedObjectConfig> measuringStars)
		{
			
		}

		public bool IsTrackedSuccessfully
		{
			get { throw new NotImplementedException(); }
		}

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
			get { throw new NotImplementedException(); }
		}

		public uint MedianValue
		{
			get { throw new NotImplementedException(); }
		}

		public float RefiningPercentageWorkLeft
		{
			get { throw new NotImplementedException(); }
		}

		public void DoManualFrameCorrection(int manualTrackingDeltaX, int manualTrackingDeltaY)
		{
			throw new NotImplementedException();
		}

		public void NextFrame(int frameNo, Model.Astro.IAstroImage astroImage)
		{
			throw new NotImplementedException();
		}

		public void BeginMeasurements(Model.Astro.IAstroImage astroImage)
		{
			throw new NotImplementedException();
		}
	}
}
