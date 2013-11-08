using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.Model.Astro;
using Tangra.Model.Image;
using Tangra.PInvoke;

namespace Tangra.VideoOperations.LightCurves.Tracking
{
    public class NativeTrackedObject : ITrackedObject
    {
        private NativeTrackedObjectPsfFit m_NativePsfFit;

        private ushort m_TrackingFlags;

        public float RefinedFWHM;
        public float RefinedIMAX;

        public List<double> m_RecentFWHMs = new List<double>();
        public List<double> m_RecentIMAXs = new List<double>();

        public NativeTrackedObject(int trackedObjectId, ITrackedObjectConfig originalObject)
        {
            TargetNo = trackedObjectId;
            OriginalObject = originalObject;
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
    }

}
