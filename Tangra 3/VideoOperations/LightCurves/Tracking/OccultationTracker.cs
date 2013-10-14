using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Model.Image;
using Tangra.Model.VideoOperations;
using Tangra.VideoOperations.LightCurves;

namespace Tangra.VideoOperations.LightCurves.Tracking
{
    internal class OccultationTracker : Tracker
    {
        private int m_RefiningFramesLeft = 0;
        private bool m_IsTrackedSuccessfully = false;

        private bool[] m_IsOffScreenPrev = new bool[4];

        public OccultationTracker(List<TrackedObjectConfig> measuringStars)
            : base(measuringStars)
        { }

        public override void InitializeNewTracking()
        {
            base.InitializeNewTracking();

            m_RefiningFramesLeft = TangraConfig.Settings.Tracking.RefiningFrames;
        }

        public override void NextFrame(int frameNo, IAstroImage astroImage)
        {
            base.NextFrame(frameNo, astroImage);

            #region run the full star recognition recovery if enabled
            if (TangraConfig.Settings.Tracking.RecoverFromLostTracking &&
                m_RefiningFramesLeft <= 0)
            {
                bool notAllStarsLocated = TrackedObjects.Exists(o => !o.IsLocated);
                bool notAllLocateFirstStarsLocated = LocateFirstObjects.Exists(o => !o.IsLocated);
                if (notAllLocateFirstStarsLocated && LocateFirstObjects.Count > 1)
                    LocateStarsWithStarRecognition(astroImage);

                // TODO: Use the notAllStarsLocated to troubleshoot the pattern recognition alignment
            }
            #endregion

            bool allGuidingStarsLocated = true;
            foreach (TrackedObject trackedObject in LocateFirstObjects)
            {
                if (!trackedObject.IsLocated && !trackedObject.IsOffScreen)
                    allGuidingStarsLocated = false;
            }

            if (m_RefiningFramesLeft > 0)
            {
                if (allGuidingStarsLocated)
                    m_RefiningFramesLeft--;
            }
            else
            {
               m_IsTrackedSuccessfully =
                    LocateFirstObjects.Count > 0 
                            ? allGuidingStarsLocated
                            : OccultedStar.IsLocated;
            }

            if (!m_Refining &&
                (LocateFirstObjects.Count > 0 && allGuidingStarsLocated) &&
                LightCurveReductionContext.Instance.FieldRotation)
            {
                for (int i = 0; i < TrackedObjects.Count; i++)
                {
					TrackedObject obj1 = TrackedObjects[i] as TrackedObject;

                    for (int j = 0; j < TrackedObjects.Count; j++)
                    {
                        if (i == j) continue;

						TrackedObject obj2 = TrackedObjects[j] as TrackedObject;

                        long pairId = (((long)obj1.TargetNo) << 32) + (long)obj2.TargetNo;

                        double oldDistance = m_RefinedDistances[pairId];
                        double newDistance = ImagePixel.ComputeDistance(
                            obj1.ThisFrameX, obj2.ThisFrameX,
                            obj1.ThisFrameY, obj2.ThisFrameY);

                        m_RefinedDistances[pairId] = (oldDistance + newDistance) / 2.0;

                        LocationVector vector = obj1.OtherGuidingStarsLocationVectors[obj2.TargetNo];
                        vector.DeltaXToAdd = (vector.DeltaXToAdd + obj2.ThisFrameX - obj1.ThisFrameX) / 2;
                        vector.DeltaYToAdd = (vector.DeltaYToAdd + obj2.ThisFrameY- obj1.ThisFrameY) / 2;
                    }
                }
            }


            if (!m_IsTrackedSuccessfully)
            {
                // If the tracking has failed, then some objects may be offscreen and have NaN position
                // So inherit the IsOffScreen flag from the previous position
                foreach (TrackedObject obj in TrackedObjects)
                    if (m_IsOffScreenPrev[obj.TargetNo] && float.IsNaN(obj.ThisFrameX))
                        obj.SetIsLocated(false, NotMeasuredReasons.ObjectExpectedPositionIsOffScreen);
            }
            else
            {
                foreach (TrackedObject obj in TrackedObjects) m_IsOffScreenPrev[obj.TargetNo] = obj.IsOffScreen;
            }
        }

        private float m_MinLocateSignal = float.NaN;
        private double m_MinLocateDistance = double.NaN;
        private List<int> m_LocateObjects = new List<int>();
        private void EnsureComputedRefinedData()
        {
			if (!float.IsNaN(m_MinLocateSignal))
			{
				m_LocateObjects.Clear();
				for (int i = 0; i < TrackedObjects.Count; i++)
				{
					if (
						(!(TrackedObjects[i] as TrackedObject).IsOcultedStar || !LightCurveReductionContext.Instance.FullDisappearance) &&
						!TrackedObjects[i].IsOffScreen &&
						!float.IsNaN((TrackedObjects[i] as TrackedObject).LastFrameX)
						)
					{
						// Don't include targets offscreen, or targets that were not found during the last tracking (could have been off screen)
						m_LocateObjects.Add((TrackedObjects[i] as TrackedObject).TargetNo);
					}
				}
				return;
			}

            m_MinLocateSignal = TrackedObjects.Cast<TrackedObject>().Min(o => o.RefinedOrLastSignalLevel == 0 ? 255f : o.RefinedOrLastSignalLevel);

            // Locate all peak pixels with signal higher than (minSignal + medianNoise) / 2
            m_MinLocateSignal = (m_MedianValue + m_MinLocateSignal) / 2f;

            double minDistance = double.MaxValue;

            for (int i = 0; i < TrackedObjects.Count; i++)
            {
				if ((!(TrackedObjects[i] as TrackedObject).IsOcultedStar && !TrackedObjects[i].IsOffScreen) ||
                    !LightCurveReductionContext.Instance.FullDisappearance)
                {
					m_LocateObjects.Add((TrackedObjects[i] as TrackedObject).TargetNo);
                }

                for (int j = 0; j < TrackedObjects.Count; j++)
                {
                    if (i == j) continue;

                    double dist = ImagePixel.ComputeDistance(
                        TrackedObjects[i].OriginalObject.ApertureStartingX,
                        TrackedObjects[j].OriginalObject.ApertureStartingX,
                        TrackedObjects[i].OriginalObject.ApertureStartingY,
                        TrackedObjects[j].OriginalObject.ApertureStartingY);

                    if (minDistance > dist)
                        minDistance = dist;
                }
            }

            m_MinLocateDistance = minDistance / 2.0;
        }

        private void LocateStarsWithStarRecognition(IAstroImage astroImage)
        {
			EnsureComputedRefinedData();

			if (m_MinLocateDistance > 8)
			{
				if (!LightCurveReductionContext.Instance.WindOrShaking)
				{
					//TODO: If the wind flag is not set, then use a 3 frame binned integration to locate the stars on       
				}

				uint[,] pixels = astroImage.GetPixelsCopy();

				List<PotentialStarStruct> peakPixels = new List<PotentialStarStruct>();
				AutoDiscoveredStars.Clear();
				AutoDiscoveredStars = StarFinder.GetStarsInArea(
					ref pixels, 8, TangraConfig.PreProcessingFilter.NoFilter, peakPixels, null,
					(uint)Math.Round(TangraConfig.Settings.Special.LostTrackingMinSignalCoeff * m_MinLocateSignal),
					TangraConfig.Settings.Special.LostTrackingMinDistance, false,
					LightCurveReductionContext.Instance.OSDFrame, ReducePeakPixels);

				Stopwatch sw = new Stopwatch();
				sw.Start();
				if (m_LocateObjects.Count == 1 &&
					AutoDiscoveredStars.Count == 1)
				{
					LocateSingleStarsWithStarRecognition(AutoDiscoveredStars, astroImage);
				}
				else if (
					m_LocateObjects.Count == 2 &&
					peakPixels.Count > 1)
				{
					LocateTwoStarsWithStarRecognition(peakPixels, astroImage, pixels);
				}
				else if (
					m_LocateObjects.Count > 2 &&
					peakPixels.Count > 1)
				{
					List<TrackedObject> goodTrackedObjects = TrackedObjects.Cast<TrackedObject>().ToList().FindAll(t => t.LastKnownGoodPosition != null);
					if (goodTrackedObjects.Count < 2)
					{
						// We don't have at least one good pair. Fail.
					}
					else if (goodTrackedObjects.Count >= 2)
					{
						goodTrackedObjects.Sort((a, b) =>
									Math.Min(b.LastKnownGoodPosition.XDouble, b.LastKnownGoodPosition.YDouble).CompareTo(
									Math.Min(a.LastKnownGoodPosition.XDouble, a.LastKnownGoodPosition.YDouble)));

						Trace.WriteLine(string.Format("StarRecognitionDistanceBasedLocation: Using objects {0} and {1}", goodTrackedObjects[0].TargetNo, goodTrackedObjects[1].TargetNo));
						// There is only 1 good pair so fallback to using 2 star recognition
						LocateTwoStarsWithStarRecognition(peakPixels, astroImage, pixels, goodTrackedObjects[0], goodTrackedObjects[1]);
					}
				}

				sw.Stop();
				Trace.WriteLine(string.Format("StarRecognitionDistanceBasedLocation: {0} sec", sw.Elapsed.TotalSeconds.ToString("0.00")));
			}
        }

        public override bool IsTrackedSuccessfully
        {
            get
            {
                return m_IsTrackedSuccessfully;
            }
        }

        public override float RefiningPercentageWorkLeft
        {
            get
            {
                return m_RefiningFramesLeft * 1.0f / TangraConfig.Settings.Tracking.RefiningFrames;
            }
        }

        public override int RefiningFramesRemaining
        {
            get { return m_RefiningFramesLeft; }
        }

		private void ReducePeakPixels(List<PotentialStarStruct> potentialStars)
		{
			if (m_LocateObjects.Count == 2)
				ReducePeakPixels2Targets(potentialStars);
		}

		private void ReducePeakPixels2Targets(List<PotentialStarStruct> potentialStars)
		{
			const double TOLERANCE = 9;

			List<int> possibleStarIndexes = new List<int>();

			TrackedObject trackedObject1 = TrackedObjects.Cast<TrackedObject>().ToList().Find(o => o.TargetNo == m_LocateObjects[0]) as TrackedObject;
			TrackedObject trackedObject2 = TrackedObjects.Cast<TrackedObject>().ToList().Find(o => o.TargetNo == m_LocateObjects[1]) as TrackedObject;
			if (trackedObject1.TargetNo != trackedObject2.TargetNo &&
				trackedObject1.LastKnownGoodPosition != null &&
				trackedObject2.LastKnownGoodPosition != null)
			{
				double deltaX = Math.Abs(trackedObject1.LastKnownGoodPosition.X - trackedObject2.LastKnownGoodPosition.X);
				double deltaY = Math.Abs(trackedObject1.LastKnownGoodPosition.Y - trackedObject2.LastKnownGoodPosition.Y);

				for (int i = 0; i < potentialStars.Count; i++)
				{
					for (int j = i + 1; j < potentialStars.Count; j++)
					{
						PotentialStarStruct px1 = potentialStars[i];
						PotentialStarStruct px2 = potentialStars[j];

						double distX = Math.Abs(px1.X - px2.X);
						double distY = Math.Abs(px1.Y - px2.Y);

						if (Math.Abs(distX - deltaX) < TOLERANCE &&
							Math.Abs(distY - deltaY) < TOLERANCE)
						{
							possibleStarIndexes.Add(i);
							possibleStarIndexes.Add(j);
						}
					}
				}
			}

			for (int i = potentialStars.Count - 1; i >= 0; i--)
			{
				if (possibleStarIndexes.IndexOf(i) == -1)
					potentialStars.RemoveAt(i);
			}
		}

        private void LocateSingleStarsWithStarRecognition(List<PSFFit> stars, IAstroImage astroImage)
        {
			TrackedObject trackedObject = TrackedObjects.Cast<TrackedObject>().ToList().Find(o => o.TargetNo == m_LocateObjects[0]);
            trackedObject.ThisFrameX = (float)stars[0].XCenter;
            trackedObject.ThisFrameY = (float)stars[0].YCenter;
            trackedObject.ThisFrameFit = stars[0];
            trackedObject.SetIsLocated(true, NotMeasuredReasons.TrackedSuccessfullyAfterStarRecognition);
            AutoDiscoveredStars.Clear();

            if (TrackedObjects.Count > 1 &&
                LocateFirstObjects.Contains(trackedObject))
            {
                // If guiding star then run the locate second objects again  
                ReLocateNonGuidingObjects(astroImage);
            }
        }

        private struct CandidatePair
        {
            public int Weight;
            public PSFFit Star1;
            public PSFFit Star2;
            public TrackedObject Object1;
            public TrackedObject Object2;
        }

		private List<CandidatePair> LocateStarPairsWithStarRecognition(TrackedObject trackedObject1, TrackedObject trackedObject2, List<PotentialStarStruct> stars, uint[,] pixels)
		{
			List<CandidatePair> candidates = new List<CandidatePair>();

			double minFWHM = (1 - TangraConfig.Settings.Special.LostTrackingFWHMCoeff) * m_AverageFWHM;
			double maxFWHM = (1 + TangraConfig.Settings.Special.LostTrackingFWHMCoeff) * m_AverageFWHM;

			if (trackedObject1.TargetNo != trackedObject2.TargetNo &&
				trackedObject1.LastKnownGoodPosition != null &&
				trackedObject2.LastKnownGoodPosition != null)
			{
				double deltaX = trackedObject1.LastKnownGoodPosition.X - trackedObject2.LastKnownGoodPosition.X;
				double deltaY = trackedObject1.LastKnownGoodPosition.Y - trackedObject2.LastKnownGoodPosition.Y;

				// Looking for two stars with the same distance and similar brighness
				for (int i = 0; i < stars.Count; i++)
				{
					for (int j = 0; j < stars.Count; j++)
					{
						if (i == j) continue;

						double deltaXStars = stars[i].X - stars[j].X;
						double deltaYStars = stars[i].Y - stars[j].Y;

						if (Math.Abs(deltaX - deltaXStars) < TangraConfig.Settings.Special.LostTrackingPositionToleranceCoeff * PositionTolerance &&
							Math.Abs(deltaY - deltaYStars) < TangraConfig.Settings.Special.LostTrackingPositionToleranceCoeff * PositionTolerance)
						{
							// Now compute PSFFits from the pixels
							PSFFit fit1 = AstroImage.GetPSFFitForPeakPixel(pixels, stars[i], m_MinLocateSignal, minFWHM, maxFWHM);
							PSFFit fit2 = AstroImage.GetPSFFitForPeakPixel(pixels, stars[j], m_MinLocateSignal, minFWHM, maxFWHM);

							if (fit1 != null && fit2 != null)
							{
								AutoDiscoveredStars.Add(fit1);
								AutoDiscoveredStars.Add(fit2);

								deltaXStars = fit1.XCenter - fit2.XCenter;
								deltaYStars = fit1.YCenter - fit2.YCenter;

								if (Math.Abs(deltaX - deltaXStars) < PositionTolerance &&
									Math.Abs(deltaY - deltaYStars) < PositionTolerance)
								{
									// Compute a certainty and add to the candidates dictionary            
									CandidatePair pair = new CandidatePair();
									pair.Star1 = fit1;
									pair.Star2 = fit2;
									pair.Object1 = trackedObject1;
									pair.Object2 = trackedObject2;

									int[] BRIGTHNESS_MATCH_WEIGTH = new int[] { 0, 0, 0, 0, 0, 1, 1, 1, 2, 3, 3 };

									int weight = (trackedObject1.IsGuidingStar ? 2 : 0) + (trackedObject2.IsGuidingStar ? 2 : 0);
									double d1 = pair.Star1.IMax - pair.Star1.I0;
									double d2 = pair.Star2.IMax - pair.Star2.I0;

									d1 = pair.Object1.RefinedOrLastSignalLevel / d1;
									if (d1 > 1) d1 = 1 / d1;
									int d1i = Math.Min(10, (int)Math.Round((1 - d1) * 10));

									weight += BRIGTHNESS_MATCH_WEIGTH[d1i];

									d2 = pair.Object2.RefinedOrLastSignalLevel / d2;
									if (d2 > 1) d2 = 1 / d2;
									int d2i = Math.Min(10, (int)Math.Round((1 - d2) * 10));

									weight += BRIGTHNESS_MATCH_WEIGTH[d2i];

									double distanceFromLastKnownGoodPosition =
										ImagePixel.ComputeDistance(
											pair.Star1.XCenter, pair.Object1.LastKnownGoodPosition.XDouble,
											pair.Star1.YCenter, pair.Object1.LastKnownGoodPosition.YDouble);

									int closeToPrevCoeff = LightCurveReductionContext.Instance.WindOrShaking ? 2 : 1;
									int closeToPrevPosWeighting = 0;
									if (distanceFromLastKnownGoodPosition < 4 * closeToPrevCoeff)
										closeToPrevPosWeighting = 3;
									else if (distanceFromLastKnownGoodPosition < 8 * closeToPrevCoeff)
										closeToPrevPosWeighting = 2;
									else if (distanceFromLastKnownGoodPosition < 16 * closeToPrevCoeff)
										closeToPrevPosWeighting = 1;
									else if (distanceFromLastKnownGoodPosition > 32 * closeToPrevCoeff)
										closeToPrevPosWeighting = -1;

									weight += closeToPrevPosWeighting;

									pair.Weight = weight;
									candidates.Add(pair);
								}
								else
									Trace.WriteLine("Pair with close distances has been rejected.");

							}
						}
					}
				}
			}

			return candidates;
		}

		private void LocateTwoStarsWithStarRecognition(List<PotentialStarStruct> stars, IAstroImage astroImage, uint[,] pixels)
		{
			TrackedObject trackedObject1 = TrackedObjects.Cast<TrackedObject>().ToList().Find(o => o.TargetNo == m_LocateObjects[0]);
			TrackedObject trackedObject2 = TrackedObjects.Cast<TrackedObject>().ToList().Find(o => o.TargetNo == m_LocateObjects[1]);

			LocateTwoStarsWithStarRecognition(stars, astroImage, pixels, trackedObject1, trackedObject2);
		}

		private void LocateTwoStarsWithStarRecognition(
			List<PotentialStarStruct> stars, IAstroImage astroImage, uint[,] pixels,
			TrackedObject trackedObject1, TrackedObject trackedObject2)
		{
			List<CandidatePair> candidates = LocateStarPairsWithStarRecognition(trackedObject1, trackedObject2, stars, pixels);

			LocateTwoStarsWithStarRecognition(candidates, astroImage, trackedObject1, trackedObject2);
		}

        private void LocateTwoStarsWithStarRecognition(
            List<CandidatePair> candidates, IAstroImage astroImage, 
            TrackedObject trackedObject1, TrackedObject trackedObject2)
        {
            try
            {
                if (candidates.Count > 0)
                {
                    candidates.Sort((c1, c2) => c2.Weight.CompareTo(c1.Weight));
                    CandidatePair bestPair = candidates[0];

                    bestPair.Object1.ThisFrameFit = bestPair.Star1;
                    bestPair.Object1.ThisFrameX = (float)bestPair.Star1.XCenter;
                    bestPair.Object1.ThisFrameY = (float)bestPair.Star1.YCenter;
					bestPair.Object1.SetIsLocated(true, NotMeasuredReasons.TrackedSuccessfullyAfterStarRecognition);

                    bestPair.Object2.ThisFrameFit = bestPair.Star2;
                    bestPair.Object2.ThisFrameX = (float)bestPair.Star2.XCenter;
                    bestPair.Object2.ThisFrameY = (float)bestPair.Star2.YCenter;
					bestPair.Object2.SetIsLocated(true, NotMeasuredReasons.TrackedSuccessfullyAfterStarRecognition);

                    ReLocateNonGuidingObjects(astroImage);
                }
                else
                {
					trackedObject1.SetIsLocated(false, NotMeasuredReasons.FailedToLocateAfterStarRecognition);
					trackedObject2.SetIsLocated(false, NotMeasuredReasons.FailedToLocateAfterStarRecognition);
                }
            }
            finally
            {
                AutoDiscoveredStars.Clear();
            }
        }

        private class TrackedObjectPair
        {
            public TrackedObject TrackedObject1;
            public TrackedObject TrackedObject2;
        }

        private class CandidateTriangle
        {
            public CandidatePair Pair1;
            public CandidatePair Pair2;
            public CandidatePair Pair3;
            public int Weight
            {
                get { return Pair1.Weight + Pair2.Weight + Pair3.Weight;  }
            }
        }
    }
}
