/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.Model.Astro;
using Tangra.Model.Image;
using Tangra.Model.VideoOperations;
using Tangra.PInvoke;

namespace Tangra.VideoOperations.LightCurves.Tracking
{
    public enum NativeTrackerNotMeasuredReasons
    {
        TrackedSuccessfully = 0,
        ObjectCertaintyTooSmall,
        FWHMOutOfRange,
        ObjectTooElongated,
        FitSuspectAsNoGuidingStarsAreLocated,
        FixedObject,
        FullyDisappearingStarMarkedTrackedWithoutBeingFound
    };

	public class NativeTrackedObject : ITrackedObject, IMeasurableObject
    {
        private NativeTrackedObjectPsfFit m_NativePsfFit;

        private uint m_TrackingFlags;

        public float RefinedFWHM;
        public float RefinedIMAX;

		private bool m_IsOccultedStar;
		private int m_PsfGroupId;
		private bool m_IsFullDisappearance;

        public List<double> m_RecentFWHMs = new List<double>();
        public List<double> m_RecentIMAXs = new List<double>();

        public NativeTrackedObject(int trackedObjectId, int bitPix, ITrackedObjectConfig originalObject, bool isFullDisappearance)
        {
            TargetNo = trackedObjectId;
            OriginalObject = originalObject;
	        m_IsOccultedStar = OriginalObject.TrackingType == TrackingType.OccultedStar;
			m_PsfGroupId = OriginalObject.GroupId;
	        m_IsFullDisappearance = isFullDisappearance;
            m_NativePsfFit = new NativeTrackedObjectPsfFit(bitPix);
        }

        internal void LoadFromNativeData(NativeTrackedObjectInfo trackingInfo, NativePsfFitInfo psfInfo, double[] residuals)
        {
            Center = new ImagePixel(trackingInfo.CenterX, trackingInfo.CenterY);
            LastKnownGoodPosition = new ImagePixel(trackingInfo.LastGoodPositionX, trackingInfo.LastGoodPositionY);
            LastKnownGoodPsfCertainty = trackingInfo.LastGoodPsfCertainty;
            IsLocated = trackingInfo.IsLocated == 1;
            IsOffScreen = trackingInfo.IsOffScreen == 1;
            m_TrackingFlags = TranslateTrackingFlags((NativeTrackerNotMeasuredReasons)trackingInfo.TrackingFlags);

            m_NativePsfFit.LoadFromNativePsfFitInfo(psfInfo, residuals);
        }

        private uint TranslateTrackingFlags(NativeTrackerNotMeasuredReasons nativeTrackerFlag)
        {
            switch (nativeTrackerFlag)
            {
                case NativeTrackerNotMeasuredReasons.TrackedSuccessfully:
                    return (uint)NotMeasuredReasons.TrackedSuccessfully;

                case NativeTrackerNotMeasuredReasons.ObjectCertaintyTooSmall:
                    return (uint)NotMeasuredReasons.ObjectCertaintyTooSmall;

                case NativeTrackerNotMeasuredReasons.FWHMOutOfRange:
                    return (uint)NotMeasuredReasons.FWHMOutOfRange;

                case NativeTrackerNotMeasuredReasons.ObjectTooElongated:
                    return (uint)NotMeasuredReasons.ObjectTooElongated;

                case NativeTrackerNotMeasuredReasons.FitSuspectAsNoGuidingStarsAreLocated:
                    return (uint)NotMeasuredReasons.FitSuspectAsNoGuidingStarsAreLocated;

                case NativeTrackerNotMeasuredReasons.FixedObject:
                    return (uint)NotMeasuredReasons.FixedObject;

                case NativeTrackerNotMeasuredReasons.FullyDisappearingStarMarkedTrackedWithoutBeingFound:
                    return (uint)NotMeasuredReasons.FullyDisappearingStarMarkedTrackedWithoutBeingFound;
            }


            return 0;
        }

        public IImagePixel Center { get; private set; }

        public IImagePixel LastKnownGoodPosition { get; set; }

        public double LastKnownGoodPsfCertainty { get; set; }

	    public bool IsLocated { get; private set; }

        public bool IsOffScreen { get; private set; }

        public ITrackedObjectConfig OriginalObject { get; private set; }

        public int TargetNo { get; private set; }

        public ITrackedObjectPsfFit PSFFit
        {
            get { return m_NativePsfFit; }
        }

        public void SetIsTracked(bool isMeasured, NotMeasuredReasons reason, IImagePixel estimatedCenter, double? certainty)
        {
            Center = estimatedCenter;
            IsLocated = isMeasured;
        }

		public uint GetTrackingFlags()
		{
			return m_TrackingFlags;
		}

        public void InitializeNewTracking()
        {
            RefinedFWHM = float.NaN;
            m_RecentFWHMs.Clear();
            m_RecentIMAXs.Clear();

            LastKnownGoodPosition = OriginalObject.AsImagePixel;
            LastKnownGoodPsfCertainty = OriginalObject.Gaussian != null ? OriginalObject.Gaussian.Certainty : 0;
            IsLocated = false;
        }

        public void NextFrame()
        {
            if (IsLocated && PSFFit != null)
            {
                if (m_RecentFWHMs.Count > 25) m_RecentFWHMs.RemoveAt(0);
                m_RecentFWHMs.Add(PSFFit.FWHM);
                RefinedFWHM = (float)m_RecentFWHMs.Average();

                if (OriginalObject.TrackingType != TrackingType.OccultedStar)
                {
                    if (m_RecentIMAXs.Count > 25) m_RecentIMAXs.RemoveAt(0);
                    m_RecentIMAXs.Add(PSFFit.IMax);
                    RefinedIMAX = (float)m_RecentIMAXs.Average();
                }
            }

            IsLocated = false;
        }

		bool IMeasurableObject.IsOccultedStar
		{
			get { return m_IsOccultedStar; }
		}

		int IMeasurableObject.PsfGroupId
		{
			get { return m_PsfGroupId; }
		}

		bool IMeasurableObject.MayHaveDisappeared
		{
			get { return m_IsOccultedStar && m_IsFullDisappearance; }
		}

		int IMeasurableObject.PsfFittingMatrixSize
		{
			get { return PSFFit != null ? PSFFit.MatrixSize : OriginalObject.PsfFitMatrixSize; }
		}
	}

}
