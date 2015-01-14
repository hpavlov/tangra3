/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Model.Image;
using Tangra.Model.VideoOperations;
using Tangra.PInvoke;

namespace Tangra.VideoOperations.LightCurves.Tracking
{
	public class NativeSimplifiedTracker : ITracker
	{
		private TrackedObjectConfig m_OccultedStarConfig;
	    private List<ITrackedObject> m_TrackedObjects = new List<ITrackedObject>();
        private List<NativeTrackedObject> m_NativeTrackedObject = new List<NativeTrackedObject>();

		internal NativeSimplifiedTracker(int width, int height, List<TrackedObjectConfig> measuringStars, bool isFullDisappearance)
		{
			NativeTracking.ConfigureNativeTracker();

			var dataRange = PSFFittingDataRange.DataRange8Bit;

			switch (LightCurveReductionContext.Instance.BitPix)
			{
				case 8:
					dataRange = PSFFittingDataRange.DataRange8Bit;
					break;

				case 12:
					dataRange = PSFFittingDataRange.DataRange12Bit;
					break;

				case 14:
					dataRange = PSFFittingDataRange.DataRange14Bit;
					break;

				case 16:
					dataRange = PSFFittingDataRange.DataRange16Bit;
					break;

				default:
					dataRange = PSFFittingDataRange.DataRange16Bit;
					break;
			}

			NativeTracking.InitNewTracker(
				width, 
				height, 
				measuringStars.Count, 
				LightCurveReductionContext.Instance.FullDisappearance,
				dataRange,
                LightCurveReductionContext.Instance.MaxPixelValue);

			for (int i = 0; i < measuringStars.Count; i++)
			{
				TrackedObjectConfig obj = measuringStars[i];
				NativeTracking.ConfigureTrackedObject(i, obj);
				var nativeObj = new NativeTrackedObject(i, LightCurveReductionContext.Instance.BitPix, obj, isFullDisappearance);
                m_NativeTrackedObject.Add(nativeObj);
                m_TrackedObjects.Add(nativeObj);
			}

			m_OccultedStarConfig = measuringStars.Single(x => x.TrackingType == TrackingType.OccultedStar);
		}

		public bool IsTrackedSuccessfully { get; private set; }

		public void InitializeNewTracking()
		{
		    NativeTracking.InitialiseNewTracking();

            RefinedAverageFWHM = float.NaN;
            MedianValue = uint.MinValue;

		    m_NativeTrackedObject.ForEach(x => x.InitializeNewTracking());
		}

		public List<ITrackedObject> TrackedObjects
		{
            get { return m_TrackedObjects; }
		}

        public float RefinedAverageFWHM { get; protected set; }

        public float[] RefinedFWHM
        {
            get
            {
                return m_TrackedObjects
					.Cast<NativeTrackedObject>()
                    .Select(x => x.RefinedFWHM)
                    .ToArray();
            }
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

        public uint MedianValue { get; protected set; }

		public float RefiningPercentageWorkLeft
		{
			get { return 0; }
		}

		public bool SupportsManualCorrections
		{
			get
			{
				return true;
			}
		}

		public void DoManualFrameCorrection(int targetId, int deltaX, int deltaY)
		{
            foreach (ITrackedObject trackedObject in TrackedObjects)
			{
				if (trackedObject.TargetNo == targetId)
				{
					trackedObject.SetIsTracked(
						true,
						NotMeasuredReasons.TrackedSuccessfully,
						new ImagePixel(trackedObject.Center.Brightness, trackedObject.Center.XDouble + deltaX, trackedObject.Center.YDouble + deltaY),
                        1);

					NativeTracking.DoManualFrameCorrection(targetId, deltaX, deltaY);
					
					break;
				}
			}
		}

		public void NextFrame(int frameNo, IAstroImage astroImage)
		{
            m_NativeTrackedObject.ForEach(x => x.NextFrame());

			uint[] pixels = astroImage.GetPixelmapPixels();

            IsTrackedSuccessfully = NativeTracking.TrackNextFrame(frameNo, pixels, m_NativeTrackedObject);
		}

		public void BeginMeasurements(IAstroImage astroImage)
		{
			// Nothing special to do when beginning measurements
		}
	}
}
