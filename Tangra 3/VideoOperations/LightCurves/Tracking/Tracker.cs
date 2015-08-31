/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Tangra.Config;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Model.Image;
using Tangra.Model.VideoOperations;
using Tangra.VideoOperations.LightCurves;

namespace Tangra.VideoOperations.LightCurves.Tracking
{
	internal class Tracker : ITracker
    {
		public List<ITrackedObject> TrackedObjects { get; private set; }


		public List<TrackedObject> LocateFirstObjects = new List<TrackedObject>();
        public List<TrackedObject> LocateSecondObjects = new List<TrackedObject>();

        public List<PSFFit> AutoDiscoveredStars = new List<PSFFit>();

        private List<int> LocateFirstObjectsTargetIds = new List<int>();
        public Dictionary<int, TrackedObject> TrackedObjectsByTargetId = new Dictionary<int, TrackedObject>();

        public TrackedObject OccultedStar;

        protected Dictionary<long, double> m_RefinedDistances = new Dictionary<long, double>();
        protected Dictionary<int, double> m_RefinedFWHM = new Dictionary<int, double>();
        protected double m_AverageFWHM;

		public float[] RefinedFWHM { get; private set; }

        protected bool m_Refining = false;
    	private bool m_AllGuidingStarsFailed = false;

        public Tracker(List<TrackedObjectConfig> trackedObjects)
        {
			TrackedObjects = new List<ITrackedObject>();

            int i = -1;
            foreach(TrackedObjectConfig originalObject in trackedObjects)
            {
                i++;
                TrackedObject trackedObject = new TrackedObject((byte)i, originalObject);
                TrackedObjects.Add(trackedObject);
                TrackedObjectsByTargetId.Add(trackedObject.TargetNo, trackedObject);

                if (IsLocateFirstObject(originalObject))
                {
                    LocateFirstObjects.Add(trackedObject);
                    LocateFirstObjectsTargetIds.Add(trackedObject.TargetNo);
                }
                else
                    LocateSecondObjects.Add(trackedObject);

                if (trackedObject.IsOccultedStar) OccultedStar = trackedObject;
             }

            m_AllowedSignalFluctoation = LightCurveReductionContext.Instance.HighFlickeringOrLargeStars ? 1.90f : 1.30f;

            //if this is not an aperture photometry we MUST have a PSF fit anyway so
            //       also fit objects with fixed aperture. Also not allow fixed apertures to be added
            //       when the object is faint and there is no good fit.  Test with first guiding and second faint occulted.

            RefinedFWHM = new float[trackedObjects.Count];
        }

		public virtual bool InitializeNewTracking(IAstroImage astroImage)
        {
            m_Refining = true;
			return true;
        }

        internal bool IsLocateFirstObject(TrackedObjectConfig objectConfig)
        {
            if (objectConfig.TrackingType == TrackingType.GuidingStar)
                // A pure guiding star
                return true;

            if (objectConfig.TrackingType == TrackingType.OccultedStar &&
                LightCurveReductionContext.Instance.LightCurveReductionType == LightCurveReductionType.MutualEvent &&
                !LightCurveReductionContext.Instance.FullDisappearance)
            {
                // Occulted star in Mutual events (which is not fully disappearing) will be bright enough for guiding
                return true;
            }

            if (objectConfig.TrackingType == TrackingType.OccultedStar &&
                objectConfig.AutoStarsInArea.Count == 1 &&
                !objectConfig.IsWeakSignalObject &&
                !LightCurveReductionContext.Instance.FullDisappearance)
            {
                // Occulted star, which is the only bright in the area and doesn't have a manually placed aperture 
                // and is not a full or almost full disappearance
                return true;
            }

            return false;
        }

        public float PositionTolerance
        {
            get
            {
                return OccultedStar.OriginalObject.PositionTolerance;
            }
        }

		private uint m_MedianValueStart = uint.MaxValue;
		protected uint m_MedianValue = uint.MaxValue;
        protected float m_AllowedSignalFluctoation = 1.45f;

        private int m_FrameNo;

        public uint MedianValue
        {
            get { return m_MedianValue; }
        }

        public float RefinedAverageFWHM
        {
            get { return (float)m_AverageFWHM; }
        }

		public virtual void NextFrame(int frameNo, IAstroImage astroImage)
        {
            m_FrameNo = frameNo;

            if (!m_Refining)
            {
                int numLocatedGuidingStars = LocateFirstObjects.Sum(t => t.IsLocated ? 1 : 0);

                // TODO: Think of chaning this to rely on at least one guiding star being located
                //       rather than all guiding stars being located successfully !
                if (numLocatedGuidingStars == LocateFirstObjects.Count)
                {
                    foreach (TrackedObject trackedObject in TrackedObjects)
                    {
                        trackedObject.LastKnownGoodFrameId = frameNo - 1;
                        trackedObject.LastKnownGoodPosition = new ImagePixel((int)trackedObject.ThisSignalLevel, trackedObject.ThisFrameX, trackedObject.ThisFrameY);
                        trackedObject.LastKnownGoodPsfCertainty = trackedObject.ThisFrameCertainty;
                    }
                }   
            }

        	m_AllGuidingStarsFailed = false;
            foreach (TrackedObject trackedObject in TrackedObjects)
                trackedObject.NewFrame();

            // Locate all guiding objects based on their previous positions
            foreach (TrackedObject trackedObject in LocateFirstObjects)
                LocateObjectAsGuidingStar(astroImage, trackedObject, false);

            if (m_Refining)
            {
                if (m_MedianValueStart == uint.MaxValue)
                    m_MedianValueStart = astroImage.MedianNoise;

                UpdateReferencePosition();
            }
            else
            {
                // Based on the refined distances, brightness and allowed tolerance
                // confirm the positions and fix some of them if needed
                CheckAndCorrectGuidingStarPositions(astroImage, true);
                ComputeFrameShift();
            }

            foreach (TrackedObject trackedObject in LocateSecondObjects)
                LocateNonGuidingObject(astroImage, trackedObject);

			if (m_AllGuidingStarsFailed)
				LocateSecondObjects.ForEach((o) => o.SetIsLocated(false, NotMeasuredReasons.FitSuspectAsNoGuidingStarsAreLocated));
        }

        protected void ReLocateNonGuidingObjects(IAstroImage astroImage)
        {
            CheckAndCorrectGuidingStarPositions(astroImage, false);
            ComputeFrameShift();

            foreach (TrackedObject trackedObject in LocateSecondObjects)
                LocateNonGuidingObject(astroImage, trackedObject); 
        }

        public virtual void BeginMeasurements(IAstroImage astroImage)
        {
            m_Refining = false;

            m_MedianValue = (byte)Math.Min(254, ((astroImage.MedianNoise + m_MedianValueStart) / 2.0));

            FinalizeRefining();
        }

        private void LocateObjectAsGuidingStar(IAstroImage astroImage, TrackedObject trackedObject, bool useLowPassFilter)
        {
            trackedObject.SetIsLocated(false, NotMeasuredReasons.UnknownReason);

            int bestMaxFixAreaSize = !float.IsNaN(trackedObject.OriginalObject.RefinedFWHM) ? (int)(6 * trackedObject.OriginalObject.RefinedFWHM) : 17;
            if (bestMaxFixAreaSize > 17) bestMaxFixAreaSize = 35; // We only support FSP Fitting of 17 or 35 square matrixes
            if (bestMaxFixAreaSize < 17) bestMaxFixAreaSize = 17;

            // Try all fits from 5 to 15. Find the one with the highest peak, then do a fit around this area with the configured psf matrix size
            uint[,] pixels;
            if (useLowPassFilter)
            {
                pixels = astroImage.GetPixelsArea((int)trackedObject.LastFrameX, (int)trackedObject.LastFrameY, bestMaxFixAreaSize + 2);
                pixels = EnhanceByteAreaForSearch(pixels);
            }
            else
            {
                pixels = astroImage.GetPixelsArea((int)trackedObject.LastFrameX, (int)trackedObject.LastFrameY, bestMaxFixAreaSize);
            }

            // There is only one object in the area, just do a wide fit followed by a fit with the selected matrix size
            PSFFit gaussian = new PSFFit((int)trackedObject.LastFrameX, (int)trackedObject.LastFrameY);
            gaussian.Fit(pixels, bestMaxFixAreaSize);

            if (gaussian.Certainty < TangraConfig.Settings.Special.MinGuidingStarCertainty &&
				(trackedObject.LastKnownGoodPsfCertainty > TangraConfig.Settings.Special.GoodGuidingStarCertainty || LightCurveReductionContext.Instance.HighFlickeringOrLargeStars))
            {
                // We have a problem. Try to find the star in the area using other means
                IImagePixel centroid = astroImage.GetCentroid((int)trackedObject.LastFrameX, (int)trackedObject.LastFrameY, bestMaxFixAreaSize, m_MedianValue);

                if (centroid != null)
                {
                    double maxAllowedDistance = !float.IsNaN(trackedObject.OriginalObject.RefinedFWHM)
                                                    ? 2 * trackedObject.OriginalObject.RefinedFWHM
                                                    : 3 * TangraConfig.Settings.Special.LostTrackingMinDistance;

	                if (LightCurveReductionContext.Instance.WindOrShaking) maxAllowedDistance *= 1.5;

                    if (centroid.DistanceTo(new ImagePixel((int)trackedObject.LastFrameX, (int)trackedObject.LastFrameY)) < maxAllowedDistance)
                    {
                        pixels = astroImage.GetPixelsArea(centroid.X, centroid.Y, bestMaxFixAreaSize);
                        gaussian = new PSFFit(centroid.X, centroid.Y);
                        gaussian.Fit(pixels, bestMaxFixAreaSize);
                    }                    
                }
            }

            IImagePixel firstCenter =
                gaussian.IsSolved
                    ? new ImagePixel(gaussian.Brightness, (int)gaussian.XCenter, (int)gaussian.YCenter)
                    : astroImage.GetCentroid((int)trackedObject.LastFrameX, (int)trackedObject.LastFrameY, bestMaxFixAreaSize, m_MedianValue);

            if (!gaussian.IsSolved || gaussian.Certainty < TangraConfig.Settings.Special.MinGuidingStarCertainty)
                firstCenter = null;

            if (firstCenter != null)
            {
                // Do a second fit 
                pixels = astroImage.GetPixelsArea(firstCenter.X, firstCenter.Y, bestMaxFixAreaSize);

                int secondFitAreaSize = Math.Max(trackedObject.PsfFitMatrixSize, (int)Math.Round(gaussian.FWHM * 2.5));
                if (secondFitAreaSize % 2 == 0) secondFitAreaSize++;
                secondFitAreaSize = Math.Min(17, secondFitAreaSize);

                gaussian = new PSFFit(firstCenter.X, firstCenter.Y);
                gaussian.Fit(pixels, secondFitAreaSize);

                if (gaussian.IsSolved)
                {
                    double signal = gaussian.IMax - gaussian.I0; if (signal < 0) signal = 0;
                    double brightnessFluctoation = signal > trackedObject.RefinedOrLastSignalLevel
                                                       ? signal / trackedObject.RefinedOrLastSignalLevel
                                                       : trackedObject.RefinedOrLastSignalLevel / signal;
                    //double brightnessFluctoation = (trackedObject.RefinedOrLastSignalLevel - gaussian.IMax + gaussian.I0) / trackedObject.RefinedOrLastSignalLevel;
                    double fluckDiff = Math.Abs(brightnessFluctoation)/m_AllowedSignalFluctoation;

                    //if (trackedObject.LastSignalLevel != 0 && 
                    //    fluckDiff > 1 && 
                    //    LightCurveReductionContext.Instance.WindOrShaking)
                    //{
                    //    // If the located object is not similar brightness as expected, then search for our object in a wider area
                    //    try
                    //    {
                    //        IImagePixel centroid = astroImage.GetCentroid((int)trackedObject.LastFrameX, (int)trackedObject.LastFrameY, 14, m_MedianValueStart);
                    //        pixels = astroImage.GetPixelsArea(centroid.X, centroid.Y, 17);
                    //        gaussian = new PSFFit(centroid.X, centroid.Y);
                    //        gaussian.Fit(pixels, trackedObject.PsfFitMatrixSize);

                    //        if (gaussian.IsSolved)
                    //        {
                    //            signal = gaussian.IMax - gaussian.I0; if (signal < 0) signal = 0;
                    //            brightnessFluctoation = signal > trackedObject.RefinedOrLastSignalLevel
                    //                                   ? signal / trackedObject.RefinedOrLastSignalLevel
                    //                                   : trackedObject.RefinedOrLastSignalLevel / signal;
                    //            //brightnessFluctoation = (trackedObject.RefinedOrLastSignalLevel - gaussian.IMax + gaussian.I0) / trackedObject.RefinedOrLastSignalLevel;
                    //            fluckDiff = Math.Abs(brightnessFluctoation) / m_AllowedSignalFluctoation;
                    //        }
                    //        else
                    //        {
                    //           Trace.WriteLine(string.Format("Frame {0}: Cannot confirm target {1} [Guiding.WindOrShaking]. Cannot solve third PSF", m_FrameNo, trackedObject.TargetNo));
                    //        }
                    //    }
                    //    catch { }
                    //}

                    if (!trackedObject.HasRefinedPositions || fluckDiff < 1 || LightCurveReductionContext.Instance.HighFlickeringOrLargeStars)
                    {
                        trackedObject.PSFFit = gaussian;
                        trackedObject.ThisFrameX = (float)gaussian.XCenter;
                        trackedObject.ThisFrameY = (float)gaussian.YCenter;
                        trackedObject.ThisSignalLevel = (float)(gaussian.IMax - gaussian.I0);
                        trackedObject.ThisFrameCertainty = (float) gaussian.Certainty;
                        trackedObject.SetIsLocated(true, NotMeasuredReasons.TrackedSuccessfully);

                        if (m_Refining)
                        {
                            trackedObject.RegisterRefinedPosition(trackedObject.Center, trackedObject.ThisSignalLevel, gaussian.FWHM);
                        }
                    }
                    else
                    {
                        if (useLowPassFilter)
                        {
                            // Only show the warning the second time around
                            Trace.WriteLine(
                                string.Format(
                                    "Frame {0}: Guiding target #{1} is suspect because the brightness fluctuation is too big: {2}",
                                    m_FrameNo, trackedObject.TargetNo, fluckDiff.ToString("0.00")));

							trackedObject.SetIsLocated(false, NotMeasuredReasons.GuidingStarBrightnessFluctoationTooHigh);
                        }
                    }
                }
                else
                {
                    Trace.WriteLine(string.Format("Frame {0}: Cannot confirm target {1} [Guiding]. Cannot solve second PSF", m_FrameNo, trackedObject.TargetNo));
					trackedObject.SetIsLocated(false, NotMeasuredReasons.PSFFittingFailed);
                }
            }
            else
            {
                Trace.WriteLine(string.Format("Frame {0}: Cannot confirm target {1} [Guiding]. Cannot solve first PSF", m_FrameNo, trackedObject.TargetNo));
				trackedObject.SetIsLocated(false, NotMeasuredReasons.PSFFittingFailed);
            }

            if (!trackedObject.IsLocated)
            {
                // Could not locate the object this time
                trackedObject.ThisSignalLevel = trackedObject.LastSignalLevel;
                if (m_Refining)
                {
                    // Use the last known coordinates for refining frames
                    trackedObject.ThisFrameX = trackedObject.LastFrameX;
                    trackedObject.ThisFrameY = trackedObject.LastFrameY;
                    trackedObject.ThisSignalLevel = trackedObject.LastSignalLevel;
                    trackedObject.ThisFrameCertainty = (float)trackedObject.LastKnownGoodPsfCertainty;
                    trackedObject.RegisterRefinedPosition(ImagePixel.Unspecified, float.NaN, double.NaN);
                }
                else
                {
                    // Make the position invalid, which will cause the distance check for this object to fail
                    // which will trigger another fitting later on. Eventually the previous position may be still used
                    trackedObject.ThisFrameX = float.NaN;
                    trackedObject.ThisFrameY = float.NaN;
                    trackedObject.ThisFrameCertainty = float.NaN;
                }
            }
        }


        private double m_LastFrameDeltaX = 0;
        private double m_LastFrameDeltaY = 0;

        private void UpdateReferencePosition()
        {
            // Computes DeltaX and DeltaY from the previous frame
            if (m_Refining)
            {
                double deltaX = 0;
                double deltaY = 0;
                int deltaPositions = 0;

                foreach (TrackedObject trackedObject in LocateFirstObjects)
                {
                    if (trackedObject.IsLocated)
                    {
                        double dx, dy;
                        trackedObject.GetPositionShift(out dx, out dy);
                        deltaX += dx;
                        deltaY += dy;
                        deltaPositions++;
                    }
                }

                if (deltaPositions > 0)
                {
                    m_LastFrameDeltaX = deltaX / deltaPositions;
                    m_LastFrameDeltaY = deltaY / deltaPositions;
                }
            }
        }

        private void ComputeFrameShift()
        {
            if (!m_Refining)
            {
                double deltaX = 0;
                double deltaY = 0;
                int deltaPositions = 0;
                
                foreach (TrackedObject trackedObject in LocateFirstObjects)
                {
                    if (trackedObject.IsLocated &&
                        trackedObject.LastKnownGoodPosition != null)
                    {
                        deltaPositions++;
                        deltaX += trackedObject.ThisFrameX - trackedObject.LastKnownGoodPosition.XDouble;
                        deltaY += trackedObject.ThisFrameY - trackedObject.LastKnownGoodPosition.YDouble;
                    }
                }

                if (deltaPositions > 0)
                {
                    m_LastFrameDeltaX = deltaX / deltaPositions;
                    m_LastFrameDeltaY = deltaY / deltaPositions;
                }
                else
                {
                    m_LastFrameDeltaX = double.NaN;
                    m_LastFrameDeltaY = double.NaN;
                }
            }
        }

        private void LocateNonGuidingObject(IAstroImage astroImage, TrackedObject trackedObject)
        {
            IImagePixel prevPos = trackedObject.HasRefinedPositions
                                     ? trackedObject.LastRefinedPosition
                                     : trackedObject.LastKnownGoodPosition != null
                                         ? trackedObject.LastKnownGoodPosition
                                         : trackedObject.OriginalObject.AsImagePixel;

            IImagePixel newStaringPos = 
				m_AllGuidingStarsFailed
					? prevPos
                    : new ImagePixel(prevPos.Brightness, prevPos.XDouble + m_LastFrameDeltaX, prevPos.YDouble + m_LastFrameDeltaY);

            if (trackedObject.OriginalObject.IsWeakSignalObject)
            {
                LocateNonGuidingObject(astroImage, trackedObject, newStaringPos);
            }
            else if (trackedObject.OriginalObject.IsFixedAperture)
            {
                LocateFixedApertureObject(astroImage, trackedObject, newStaringPos);
            }
            else if (
                trackedObject.OriginalObject.TrackingType == TrackingType.OccultedStar &&
                LightCurveReductionContext.Instance.FullDisappearance &&
                !trackedObject.OriginalObject.IsCloseToOtherStars)
            {
                LocateFullDisappearingObject(astroImage, trackedObject, newStaringPos);
            }
            else
            {
                LocateSingleNonGuidingObject(astroImage, trackedObject, newStaringPos);
            }
        }

        private void GetAverageObjectPositionsFromGuidingStars(TrackedObject trackedObject, IImagePixel newStaringPos, out double averageX, out double averageY)
        {
            List<TrackedObject> resolvedGuidingStars = LocateFirstObjects.FindAll(o => o.IsLocated);
            if (resolvedGuidingStars.Count == 0)
            {
                averageX = newStaringPos.XDouble;
                averageY = newStaringPos.YDouble;
            }
            else
            {
                averageX = 0;
                averageY = 0;

                foreach (TrackedObject resolvedGuidingStar in resolvedGuidingStars)
                {
                    if (m_Refining)
                    {
                        averageX += (resolvedGuidingStar.ThisFrameX + trackedObject.OriginalObject.ApertureStartingX -
                                     resolvedGuidingStar.OriginalObject.ApertureStartingX);
                        averageY += (resolvedGuidingStar.ThisFrameY + trackedObject.OriginalObject.ApertureStartingY -
                                     resolvedGuidingStar.OriginalObject.ApertureStartingY);
                    }
                    else
                    {
                        LocationVector vec =
                            resolvedGuidingStar.OtherGuidingStarsLocationVectors[trackedObject.TargetNo];
                        averageX += (resolvedGuidingStar.ThisFrameX + vec.DeltaXToAdd);
                        averageY += (resolvedGuidingStar.ThisFrameY + vec.DeltaYToAdd);
                    }
                }

                averageX /= resolvedGuidingStars.Count;
                averageY /= resolvedGuidingStars.Count;
            }
        }


        private void LocateNonGuidingObject(IAstroImage astroImage, TrackedObject trackedObject, IImagePixel newStaringPos)
        {
			trackedObject.SetIsLocated(false, NotMeasuredReasons.UnknownReason);

            double averageX = 0;
            double averageY = 0;

            GetAverageObjectPositionsFromGuidingStars(trackedObject, newStaringPos, out averageX, out averageY);

            trackedObject.ThisFrameX = (float)averageX;
            trackedObject.ThisFrameY = (float)averageY;
            trackedObject.PSFFit = null;
            trackedObject.ThisSignalLevel = float.NaN;
            trackedObject.ThisFrameCertainty = float.NaN;

            List<TrackedObject> resolvedGuidingStars = LocateFirstObjects.FindAll(o => o.IsLocated);
            if (resolvedGuidingStars.Count == 0)
            {
                if (m_Refining)
                    trackedObject.RegisterRefinedPosition(newStaringPos, float.NaN, double.NaN);
            }
            else
            {
				FitObjectInLimitedArea(trackedObject, astroImage, (float) averageX, (float) averageY);

                if (m_Refining)
                    trackedObject.RegisterRefinedPosition(
                        new ImagePixel((int)trackedObject.ThisSignalLevel, trackedObject.ThisFrameX, trackedObject.ThisFrameY), 
                        trackedObject.ThisSignalLevel,
						trackedObject.PSFFit != null ? trackedObject.PSFFit.FWHM : double.NaN);
            }            
        }

        private void LocateFixedApertureObject(IAstroImage astroImage, TrackedObject trackedObject, IImagePixel newStaringPos)
        {
            double averageX = 0;
            double averageY = 0;

            GetAverageObjectPositionsFromGuidingStars(trackedObject, newStaringPos, out averageX, out averageY);

			trackedObject.SetIsLocated(true, NotMeasuredReasons.FixedObject);
            trackedObject.ThisFrameX = (float)averageX;
            trackedObject.ThisFrameY = (float)averageY;
			trackedObject.PSFFit = null;
            trackedObject.ThisSignalLevel = float.NaN;
            trackedObject.ThisFrameCertainty = float.NaN;
        }

        protected void FitObjectInLimitedArea(TrackedObject trackedObject, IAstroImage astroImage, float startingX, float startingY)
        {
            int smallestMatrixSize = (int)Math.Round(trackedObject.OriginalObject.ApertureInPixels * 2);
            if (smallestMatrixSize % 2 == 0) smallestMatrixSize++;

            // If this is not an aperture photometry we still derive a PSF 
            uint[,] pixels = astroImage.GetPixelsArea((int)Math.Round(startingX), (int)Math.Round(startingY), 17);

        	bool isFSPSolved = false;
        	bool isTooFar = true;

			for (int i = Math.Max(trackedObject.PsfFitMatrixSize, smallestMatrixSize); i >= smallestMatrixSize; i -= 2)
            {
                int borderZeroes = (trackedObject.PsfFitMatrixSize - i) / 2;
                for (int x = 0; x < pixels.GetLength(0); x++)
                {
                    for (int y = 0; y < borderZeroes; y++)
                    {
                        pixels[x, y] = 0;
                        pixels[x, pixels.GetLength(1) - y - 1] = 0;
                    }
                }
                for (int y = 0; y < pixels.GetLength(1); y++)
                {
                    for (int x = 0; x < borderZeroes; x++)
                    {
                        pixels[x, y] = 0;
                        pixels[pixels.GetLength(0) - x - 1, y] = 0;
                    }
                }

                PSFFit gaussian = new PSFFit((int)Math.Round(startingX), (int)Math.Round(startingY));
                gaussian.Fit(pixels, trackedObject.PsfFitMatrixSize);
				isFSPSolved = gaussian.IsSolved;
				isTooFar = true;
                if (gaussian.IsSolved)
                {
                    trackedObject.ThisFrameCertainty = (float)gaussian.Certainty;
                    double dist = ImagePixel.ComputeDistance((float)gaussian.XCenter, (float)startingX, (float)gaussian.YCenter, (float)startingY);
                    if (dist <= PositionTolerance)
                    {
						isTooFar = false;
						trackedObject.PSFFit = gaussian;
                        trackedObject.ThisFrameX = (float)gaussian.XCenter;
                        trackedObject.ThisFrameY = (float)gaussian.YCenter;
                        trackedObject.ThisSignalLevel = (float)(gaussian.IMax - gaussian.I0);
                        trackedObject.ThisFrameCertainty = (float)gaussian.Certainty;
                    	trackedObject.SetIsLocated(true, NotMeasuredReasons.TrackedSuccessfully);
                        return;
                    }
                }
            }

			trackedObject.PSFFit = null;
            trackedObject.ThisFrameX = startingX;
            trackedObject.ThisFrameY = startingY;
			trackedObject.SetIsLocated(false, 
				!isFSPSolved 
					? NotMeasuredReasons.PSFFittingFailed
					: isTooFar 
						? NotMeasuredReasons.FoundObjectNotWithInExpectedPositionTolerance
						: NotMeasuredReasons.UnknownReason);
        }

        private void LocateFullDisappearingObject(IAstroImage astroImage, TrackedObject trackedObject, IImagePixel newStaringPos)
        {
            if (m_Refining)
                // Full disapearance is not expected during refining
                LocateNonGuidingObject(astroImage, trackedObject, newStaringPos);
            else
            {
                double averageX, averageY;
                GetAverageObjectPositionsFromGuidingStars(trackedObject, newStaringPos, out averageX, out averageY);

                trackedObject.ThisFrameX = (float)averageX + 0.5f;
                trackedObject.ThisFrameY = (float)averageY + 0.5f;
				trackedObject.PSFFit = null;
                trackedObject.ThisSignalLevel = float.NaN;
                trackedObject.ThisFrameCertainty = 1;

                int x0 = (int) Math.Round(averageX);
                int y0 = (int) Math.Round(averageY);
 
				trackedObject.SetIsLocated(false, NotMeasuredReasons.UnknownReason);
                PSFFit gaussian = null;

                int smallestMatrixSize = (int)Math.Round(trackedObject.OriginalObject.ApertureInPixels * 2);
                if (smallestMatrixSize % 2 == 0) smallestMatrixSize++;

                // If this is not an aperture photometry we still derive a PSF 
                uint[,] pixels = astroImage.GetPixelsArea(x0, y0, 17);

                for (int i = trackedObject.PsfFitMatrixSize; i >= smallestMatrixSize; i -= 2)
                {
                    int borderZeroes = (trackedObject.PsfFitMatrixSize - i) / 2;
                    for (int x = 0; x < pixels.GetLength(0); x++)
                    {
                        for (int y = 0; y < borderZeroes; y++)
                        {
                            pixels[x, y] = 0;
                            pixels[x, pixels.GetLength(1) - y - 1] = 0;
                        }
                    }
                    for (int y = 0; y < pixels.GetLength(1); y++)
                    {
                        for (int x = 0; x < borderZeroes; x++)
                        {
                            pixels[x, y] = 0;
                            pixels[pixels.GetLength(0) - x - 1, y] = 0;
                        }
                    }

                    gaussian = new PSFFit(x0, y0);
                    gaussian.Fit(pixels, trackedObject.PsfFitMatrixSize);
                    if (gaussian.IsSolved)
                    {
                        double dist = ImagePixel.ComputeDistance((float)gaussian.XCenter, (float)averageX, (float)gaussian.YCenter, (float)averageY);
                        if (dist <= PositionTolerance)
                        {
							trackedObject.SetIsLocated(true, NotMeasuredReasons.TrackedSuccessfully);
                            break;
                        }
                        else
							trackedObject.SetIsLocated(false, NotMeasuredReasons.FoundObjectNotWithInExpectedPositionTolerance);
                    }
					else
						trackedObject.SetIsLocated(false, NotMeasuredReasons.PSFFittingFailed);
                }

                if (gaussian != null)
                {
					if (!trackedObject.IsLocated)
						trackedObject.SetIsLocated(true, NotMeasuredReasons.FullyDisappearingStarMarkedTrackedWithoutBeingFound);

					trackedObject.PSFFit = gaussian;
                    trackedObject.ThisSignalLevel = (float)(gaussian.IMax - gaussian.I0);
                    
                    trackedObject.ThisFrameX = (float)gaussian.XCenter;
                    trackedObject.ThisFrameY = (float)gaussian.YCenter;
                    trackedObject.ThisFrameCertainty = (float)gaussian.Certainty;
                }
            }
        }

        private void LocateSingleNonGuidingObject(IAstroImage astroImage, TrackedObject trackedObject, IImagePixel newStaringPos)
        {
            LocateNonGuidingObject(astroImage, trackedObject, newStaringPos);            
        }


        private void CheckAndCorrectGuidingStarPositions(IAstroImage astroImage, bool retryWithLPFilterIfSuspectObjects)
        {
            List<int> goodObjects = new List<int>();
            List<int> suspectObjects = new List<int>();

            for (int k = 0; k < 2; k++)
            {
                goodObjects.Clear();
                suspectObjects.Clear();

                if (LocateFirstObjects.Count == 1)
                {
                    if (!LocateFirstObjects[0].IsLocated ||
                        float.IsNaN(LocateFirstObjects[0].ThisFrameX) ||
                        float.IsNaN(LocateFirstObjects[0].ThisFrameY))
                    {
                        suspectObjects.Add(LocateFirstObjects[0].TargetNo);
                    }
                }
                else
                {
                    // See how many of the distances are okay
                    for (int i = 0; i < LocateFirstObjects.Count; i++)
                    {
                        TrackedObject obj1 = LocateFirstObjects[i];

                        for (int j = 0; j < LocateFirstObjects.Count; j++)
                        {
                            if (i == j) continue;

                            TrackedObject obj2 = LocateFirstObjects[j];

                            long pairId = (((long) obj1.TargetNo) << 32) + (long) obj2.TargetNo;
                            double expectedDistance = m_RefinedDistances[pairId];
                            double actualDistance =
                                ImagePixel.ComputeDistance(obj1.ThisFrameX, obj2.ThisFrameX, obj1.ThisFrameY,
                                                           obj2.ThisFrameY);

                            if (Math.Abs(expectedDistance - actualDistance) <= PositionTolerance)
                            {
                                goodObjects.Add(obj1.TargetNo);
                                goodObjects.Add(obj2.TargetNo);
                            }
                            else
                            {
                                suspectObjects.Add(obj1.TargetNo);
                                suspectObjects.Add(obj2.TargetNo);
                            }
                        }
                    }
                }

                if (k == 0 && 
                    suspectObjects.Count > 0 &&
                    retryWithLPFilterIfSuspectObjects)
                {
                    // There are some suspect objects. Try to locate the guiding stars with a low pass filter
                    foreach (TrackedObject trackedObject in LocateFirstObjects)
                        LocateObjectAsGuidingStar(astroImage, trackedObject, true);
                }
                else
                    break;
            }

            if (suspectObjects.Count > 0)
            {
                if (goodObjects.Distinct().Count() == LocateFirstObjectsTargetIds.Count)
                {
                    // There is at least one good distance involving each of the locate first objects
                    // This means the suspect distance it not too bad so we ignore and continue
                }
                else
                {
                    if (goodObjects.Count == 0)
                    {
                        List<TrackedObject> okayObjects = LocateFirstObjects.FindAll(o => !o.IsOccultedStar && o.IsLocated);

                        if (okayObjects.Count == 0)
                        {
                            // If one of the objects is the occulted star (found) and non of the other guiding stars are found
                            // then make all objects not found and use the latest expected positions

                            foreach(TrackedObject obj in LocateFirstObjects)
                            {
								obj.PSFFit = null;
                                obj.ThisFrameX = obj.LastFrameX;
                                obj.ThisFrameY = obj.LastFrameY;
                                obj.ThisFrameCertainty = (float)obj.LastKnownGoodPsfCertainty;
								obj.SetIsLocated(false, NotMeasuredReasons.FitSuspectAsNoGuidingStarsAreLocated);
                            }

                        	m_AllGuidingStarsFailed = true;
                            return;
                        }

                        
                        // If no distances are okay consider the brightest located first object as the correctly identified one 
                        // and then find the positions of the other ones based on the expected distances (and do another PSF fit in a smaller area)
                        okayObjects = LocateFirstObjects.FindAll(o => o.IsLocated);
                        okayObjects.Sort((o1, o2) => o1.LastSignalLevel.CompareTo(o2.LastSignalLevel));
                        TrackedObject brightestObject = okayObjects[0];

                        foreach (TrackedObject obj in LocateFirstObjects)
                        {
                            if (obj.TargetNo == brightestObject.TargetNo) continue;

                            LocationVector vec = brightestObject.OtherGuidingStarsLocationVectors[obj.TargetNo];
                            double expectedX = brightestObject.ThisFrameX + vec.DeltaXToAdd;
                            double expectedY = brightestObject.ThisFrameY + vec.DeltaYToAdd;

                            if (expectedX < RefinedAverageFWHM || expectedX > astroImage.Width - RefinedAverageFWHM ||
                                expectedY < RefinedAverageFWHM || expectedY > astroImage.Height - RefinedAverageFWHM)
                            {
                                // The expected position in Off Screen.     
								obj.PSFFit = null;
                                obj.ThisFrameX = float.NaN;
                                obj.ThisFrameY = float.NaN;
                                obj.ThisFrameCertainty = float.NaN;
                                obj.SetIsLocated(false, NotMeasuredReasons.ObjectExpectedPositionIsOffScreen);
                                continue;
                            }

                            uint[,] pixels = astroImage.GetPixelsArea((int)Math.Round(expectedX), (int)Math.Round(expectedY), 19);
                            pixels = EnhanceByteAreaForSearch(pixels);

                            PSFFit gaussian = new PSFFit((int)Math.Round(expectedX), (int)Math.Round(expectedY));
                            gaussian.Fit(pixels, obj.PsfFitMatrixSize);
                            if (gaussian.IsSolved)
                            {
                                double distance = ImagePixel.ComputeDistance(gaussian.XCenter, expectedX, gaussian.YCenter, expectedY);
                                if (distance <= 2 * PositionTolerance)
                                {
                                    // Object successfully located
									obj.PSFFit = gaussian;
                                    obj.ThisFrameX = (float)gaussian.XCenter;
                                    obj.ThisFrameY = (float)gaussian.YCenter;
                                    obj.ThisFrameCertainty = (float)gaussian.Certainty;
									obj.SetIsLocated(true, NotMeasuredReasons.TrackedSuccessfullyAfterDistanceCheck);
                                    Trace.WriteLine(string.Format("Frame {0}: Successfully located object #{1} after a close look up {2}px from the expected position.", m_FrameNo, obj.TargetNo, distance.ToString("0.00")));
                                }
                                else
                                {
									obj.PSFFit = null;
                                    obj.ThisFrameX = (float)expectedX;
                                    obj.ThisFrameY = (float)expectedY;
                                    obj.ThisFrameCertainty = 0;
									obj.SetIsLocated(false, NotMeasuredReasons.FailedToLocateAfterDistanceCheck);
                                    Trace.WriteLine(string.Format("Frame {0}: Cannot locate object #{1}. A close look up found it {2}px from the expected position.", m_FrameNo, obj.TargetNo, distance.ToString("0.00")));
                                }
                            }
                        }
                    }
                    else
                    {
                        // Some objects are okay and some are not okay. 
                        List<int> badObjectIds = suspectObjects.FindAll(o => goodObjects.IndexOf(o) == -1).Distinct().ToList();
                        List<int> goodObjectIds = goodObjects.Distinct().ToList();
                        foreach(int badObjectId in badObjectIds)
                        {
                            double expectedX = 0;
                            double expectedY = 0;

                            TrackedObject badObject = TrackedObjectsByTargetId[badObjectId];
                            foreach (int goodObjectId in goodObjectIds)
                            {
                                TrackedObject goodObject = TrackedObjectsByTargetId[goodObjectId];
                                LocationVector vec = goodObject.OtherGuidingStarsLocationVectors[badObjectId];

                                expectedX += (goodObject.ThisFrameX + vec.DeltaXToAdd);
                                expectedY += (goodObject.ThisFrameY + vec.DeltaYToAdd);
                            }

                            expectedX /= goodObjectIds.Count;
                            expectedY /= goodObjectIds.Count;

                            uint[,] pixels = astroImage.GetPixelsArea((int)Math.Round(expectedX), (int)Math.Round(expectedY), 19);
                            pixels = EnhanceByteAreaForSearch(pixels);

                            PSFFit gaussian = new PSFFit((int)Math.Round(expectedX), (int)Math.Round(expectedY));
                            gaussian.Fit(pixels, badObject.PsfFitMatrixSize);
                            if (gaussian.IsSolved)
                            {
                                double distance = ImagePixel.ComputeDistance(gaussian.XCenter, expectedX, gaussian.YCenter, expectedY);
                                if (distance <= 2 * PositionTolerance)
                                {
                                    // Object successfully located
									badObject.PSFFit = gaussian;
                                    badObject.ThisFrameX = (float)gaussian.XCenter;
                                    badObject.ThisFrameY = (float)gaussian.YCenter;
                                    badObject.ThisFrameCertainty = (float)gaussian.Certainty;
									badObject.SetIsLocated(true, NotMeasuredReasons.TrackedSuccessfullyAfterDistanceCheck);
                                    Trace.WriteLine(string.Format("Frame {0}: Successfully located object #{1} after a close look up {2}px from the expected position.", m_FrameNo, badObjectId, distance.ToString("0.00")));
                                }
                                else
                                {
									badObject.PSFFit = null;
                                    badObject.ThisFrameX = (float)expectedX;
                                    badObject.ThisFrameY = (float)expectedY;
                                    badObject.ThisFrameCertainty = 0;
									badObject.SetIsLocated(false, NotMeasuredReasons.FailedToLocateAfterDistanceCheck);
                                    Trace.WriteLine(string.Format("Frame {0}: Cannot locate object #{1}. A close look up found it {2}px from the expected position.", m_FrameNo, badObjectId, distance.ToString("0.00")));
                                }
                            }
                        }
                    } 
                }
            }
            else if (
                goodObjects.Count == 0 &&
                LocateFirstObjects.Count == 1 &&
                float.IsNaN(LocateFirstObjects[0].ThisFrameX))
            {
                // Single 'lost' guiding star or single object (occulted star only)
                TrackedObject obj1 = LocateFirstObjects[0];

                float bestFluctDiff = float.MaxValue;

                int coeff = LightCurveReductionContext.Instance.WindOrShaking ? 3 : 1;
                for (int i = 0; i < coeff * obj1.PsfFitMatrixSize; i++)
                {
                    int searchRadius = obj1.PsfFitMatrixSize + i;
                    IImagePixel centroid = astroImage.GetCentroid(
                        (int) Math.Round(obj1.LastFrameX), (int) Math.Round(obj1.LastFrameY),
                        searchRadius, m_MedianValue);

                    if (centroid != null)
                    {
                        uint[,] pixels = astroImage.GetPixelsArea(centroid.X, centroid.Y, 19);
                        PSFFit gaussian = new PSFFit(centroid.X, centroid.Y);
                        gaussian.Fit(pixels, obj1.PsfFitMatrixSize);
                        if (gaussian.IsSolved)
                        {
                            double brightnessFluctoation = (obj1.RefinedOrLastSignalLevel - gaussian.IMax + gaussian.I0) / obj1.RefinedOrLastSignalLevel;
                            double fluckDiff = Math.Abs(brightnessFluctoation) / m_AllowedSignalFluctoation;

                            if (fluckDiff < 1 || LightCurveReductionContext.Instance.HighFlickeringOrLargeStars)
                            {
								obj1.PSFFit = gaussian;
                                obj1.ThisFrameX = (float) gaussian.XCenter;
                                obj1.ThisFrameY = (float) gaussian.YCenter;
                                obj1.ThisSignalLevel = (float) (gaussian.IMax - gaussian.I0);
                                obj1.ThisFrameCertainty = (float)gaussian.Certainty;
                                obj1.SetIsLocated(true, NotMeasuredReasons.TrackedSuccessfullyAfterWiderAreaSearch);
                                Trace.WriteLine(
                                    string.Format(
                                        "Frame {0}: Successfully located object #{1} after a wider search in {2}px region.",
                                        m_FrameNo, obj1.TargetNo, searchRadius));
                                break;
                            }
                            else
                            {
                                if (bestFluctDiff > fluckDiff)
                                    bestFluctDiff = (float)fluckDiff;
                            }
                        }
                    }
                }

                if (!obj1.IsLocated)
                {
					obj1.PSFFit = null;
                    obj1.ThisFrameX = obj1.LastFrameX;
                    obj1.ThisFrameY = obj1.LastFrameY;
                    obj1.ThisSignalLevel = obj1.LastSignalLevel;
                    obj1.ThisFrameCertainty = (float) obj1.LastKnownGoodPsfCertainty;
					obj1.SetIsLocated(false, NotMeasuredReasons.TrackedSuccessfullyAfterWiderAreaSearch);
                    Trace.WriteLine(string.Format("Frame {0}: Cannot locate object #{1} after a wider search in {2}px region. Best fluct diff: {3}",
                        m_FrameNo, obj1.TargetNo, obj1.PsfFitMatrixSize * 2, bestFluctDiff.ToString("0.00")));
                }
            }
        }

        private uint[,] EnhanceByteAreaForSearch(uint[,] pixels)
        {
            return ImageFilters.CutArrayEdges(pixels);
        }

        protected PSFFit GetBestFitGaussian(AstroImage astroImage, TrackedObject guidingStar)
        {   
            // Try all fits from 5 to 15. Find the one with the highest peak, then do a fit around this area with the configured psf matrix size
            uint[,] pixels = astroImage.GetMeasurableAreaPixels((int)guidingStar.LastFrameX, (int)guidingStar.LastFrameY);

            PSFFit bestGaussian = null;


			if (!guidingStar.OriginalObject.IsCloseToOtherStars)
            {
                // There is only one object in the area, just do a whide fit followed by a fit with the selected matrix size
                PSFFit gaussian = new PSFFit((int) guidingStar.LastFrameX, (int) guidingStar.LastFrameY);
                gaussian.Fit(pixels, pixels.GetLength(0));
                if (gaussian.IsSolved)
                {
                    bestGaussian = gaussian;
                }
            }
            else
            {
                // There is more than one object in the area. We need a smarter way to find the one we need.
                if (guidingStar.IsOccultedStar)
                {
                    for (int matSize = 5; matSize < 15; matSize += 2)
                    {
                        PSFFit gaussian = new PSFFit((int) guidingStar.LastFrameX, (int) guidingStar.LastFrameY);
                        gaussian.Fit(pixels, matSize);
                        if (gaussian.IsSolved)
                        {
                            if (bestGaussian == null ||
                                bestGaussian.IMax < gaussian.IMax)
                            {
                                bestGaussian = gaussian;
                            }
                        }
                    }
                }
                else
                {
                    PSFFit gaussian = new PSFFit((int) guidingStar.LastFrameX, (int) guidingStar.LastFrameY);
                    gaussian.Fit(pixels, guidingStar.PsfFitMatrixSize);
                }
            }
            
            if (bestGaussian != null)
            {
                pixels = astroImage.GetMeasurableAreaPixels((int)bestGaussian.XCenter, (int)bestGaussian.YCenter);
                bestGaussian = new PSFFit((int)bestGaussian.XCenter, (int)bestGaussian.YCenter);
                bestGaussian.Fit(pixels, guidingStar.PsfFitMatrixSize);
            }

            return bestGaussian;
        }

        public virtual float RefiningPercentageWorkLeft
        {
            get { return 0; }
        }

        public virtual int RefiningFramesRemaining
        {
            get { return 0; }
        }

        public virtual bool IsTrackedSuccessfully
        {
            get { return false; }
        }

        private void FinalizeRefining()
        {
            ComputeRefinedFlickering();
            ComputeRefinedDistances();
            ComputeRefinedFWHM();
        }

        private void ComputeRefinedFlickering()
        {
            List<float> flickering = new List<float>();
            List<float> maxResiduals = new List<float>();

            foreach (TrackedObject trackedObject in LocateFirstObjects)
            {
                float flicker, maxResidual;
                trackedObject.ComputeRefinedFlickering(out flicker, out maxResidual);

                if (!float.IsNaN(flicker))
                {
                    flickering.Add(flicker);
                    maxResiduals.Add(maxResidual);                    
                }
            }

            if (maxResiduals.Count > 0)
            {
                float coeff = LightCurveReductionContext.Instance.HighFlickeringOrLargeStars 
					? TangraConfig.Settings.Special.BrightnessFluctoationCoefficientHighFlickering 
					: TangraConfig.Settings.Special.BrightnessFluctoationCoefficientNoFlickering;

                m_AllowedSignalFluctoation = coeff * (1 + maxResiduals.Average());
            }

            Trace.WriteLine(string.Format("Allowed Brightness Fluctuations: {0}", m_AllowedSignalFluctoation.ToString("0.00")));


            foreach (TrackedObject trackedObject in LocateSecondObjects)
            {
                float flicker, maxResidual;
                trackedObject.ComputeRefinedFlickering(out flicker, out maxResidual);
            }
        }

        private void ComputeRefinedDistances()
        {
            for (int i = 0; i < TrackedObjects.Count; i++)
            {
				TrackedObject obj1 = TrackedObjects[i] as TrackedObject;
                obj1.OtherGuidingStarsLocationVectors = new Dictionary<int, LocationVector>();

                for (int j = 0; j < TrackedObjects.Count; j++)
                {    
                    if (i == j) continue;

					TrackedObject obj2 = TrackedObjects[j] as TrackedObject;

                    double xVector, yVector;
                    double refinedDistance = obj1.ComputeRefinedDistances(obj2, out xVector, out yVector);

                    long pairId = (((long)obj1.TargetNo) << 32) + (long)obj2.TargetNo;
                    m_RefinedDistances.Add(pairId, refinedDistance);

                    LocationVector vector = new LocationVector();
                    vector.DeltaXToAdd = xVector;
                    vector.DeltaYToAdd = yVector;
                    obj1.OtherGuidingStarsLocationVectors.Add(obj2.TargetNo, vector);
                }
            }
        }

        private void ComputeRefinedFWHM()
        {
            m_RefinedFWHM.Clear();

            m_AverageFWHM = 0;
            int numDataPoints = 0;
            foreach (TrackedObject trackedObject in TrackedObjects)
            {
                double fwhm = trackedObject.ComputeRefinedFWHM();
                if (!double.IsNaN(fwhm))
                {
                    m_RefinedFWHM.Add(trackedObject.TargetNo, fwhm);
                    m_AverageFWHM += fwhm;
                    numDataPoints++;

					RefinedFWHM[trackedObject.TargetNo] = (float)fwhm;
                    trackedObject.OriginalObject.RefinedFWHM = (float)fwhm;
                }
            }

            if (numDataPoints != 0) m_AverageFWHM /= numDataPoints;

            if (m_AverageFWHM == 0) m_AverageFWHM = 3.5; /* Fallback to a dummy default value */

            Trace.WriteLine(string.Format("Averaged FWHM: {0}px", m_AverageFWHM.ToString("0.00")));

            for (int i = 0; i < RefinedFWHM.Count(); i++)
            {
				if (RefinedFWHM[i] == 0) RefinedFWHM[i] = (float)m_AverageFWHM;
            }
        }
		
		public bool SupportsManualCorrections
		{
			get { return true; }
		}

		public virtual void DoManualFrameCorrection(int targetId, int deltaX, int deltaY)
        {
            foreach (TrackedObject trackedObject in TrackedObjects)
            {
				if (trackedObject.LastKnownGoodPosition != null && 
					trackedObject.TargetNo == targetId)
				{
					trackedObject.ThisFrameX = (float)trackedObject.LastKnownGoodPosition.XDouble + deltaX;
					trackedObject.ThisFrameY = (float)trackedObject.LastKnownGoodPosition.YDouble + deltaY;
                    trackedObject.ThisFrameCertainty = 1;
					break;
				}
            }
        }
    }
}
