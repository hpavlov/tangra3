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
	public class NativeTrackedObject : ITrackedObject, IMeasurableObject
    {
        private NativeTrackedObjectPsfFit m_NativePsfFit;

        private ushort m_TrackingFlags;

        public float RefinedFWHM;
        public float RefinedIMAX;

		private bool m_IsOccultedStar;
		private bool m_IsFullDisappearance;

        public List<double> m_RecentFWHMs = new List<double>();
        public List<double> m_RecentIMAXs = new List<double>();

        public NativeTrackedObject(int trackedObjectId, ITrackedObjectConfig originalObject, bool isFullDisappearance)
        {
            TargetNo = trackedObjectId;
            OriginalObject = originalObject;
	        m_IsOccultedStar = OriginalObject.TrackingType == TrackingType.OccultedStar;
	        m_IsFullDisappearance = isFullDisappearance;
            m_NativePsfFit = new NativeTrackedObjectPsfFit(8);
        }

        internal void LoadFromNativeData(NativeTrackedObjectInfo trackingInfo, NativePsfFitInfo psfInfo, double[] residuals)
        {
            Center = new ImagePixel(trackingInfo.CenterX, trackingInfo.CenterY);
            LastKnownGoodPosition = new ImagePixel(trackingInfo.LastGoodPositionX, trackingInfo.LastGoodPositionY);
            IsLocated = trackingInfo.IsLocated == 1;
            IsOffScreen = trackingInfo.IsOffScreen == 1;
            m_TrackingFlags = trackingInfo.TrackingFlags;

            m_NativePsfFit.LoadFromNativePsfFitInfo(psfInfo, residuals);
        }

        public IImagePixel Center { get; private set; }

        public IImagePixel LastKnownGoodPosition { get; set; }

        public bool IsLocated { get; private set; }

        public bool IsOffScreen { get; private set; }

        public ITrackedObjectConfig OriginalObject { get; private set; }

        public int TargetNo { get; private set; }

        public ITrackedObjectPsfFit PSFFit
        {
            get { return m_NativePsfFit; }
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
