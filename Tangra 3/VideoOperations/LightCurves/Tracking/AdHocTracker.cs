using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Model.Image;
using Tangra.Model.VideoOperations;

namespace Tangra.VideoOperations.LightCurves.Tracking
{
	internal class AdHocTracker : BaseTracker
	{
		private float MIN_SAME_STAR_DISTANCE;
		private float MAX_AUTO_ASSUME_DUPLICATE_STAR_DISTANCE;
		private float STELLAR_OBJECT_MAX_ELONGATION;
		private float STELLAR_OBJECT_MIN_FWHM;
		private float STELLAR_OBJECT_MAX_FWHM;
		private float STELLAR_OBJECT_MIN_CERTAINTY;

		internal AdHocTracker(List<TrackedObjectConfig> measuringStars)
			: base(measuringStars)
		{
			MIN_SAME_STAR_DISTANCE = TangraConfig.Settings.Tracking.AdHokMaxCloseDistance;
			MAX_AUTO_ASSUME_DUPLICATE_STAR_DISTANCE = TangraConfig.Settings.Tracking.AdHokMinCloseDistance;
			STELLAR_OBJECT_MAX_ELONGATION = TangraConfig.Settings.Tracking.AdHokMaxElongation;
			STELLAR_OBJECT_MIN_FWHM = TangraConfig.Settings.Tracking.AdHokMinFWHM;
			STELLAR_OBJECT_MAX_FWHM = TangraConfig.Settings.Tracking.AdHokMaxFWHM;
			STELLAR_OBJECT_MIN_CERTAINTY = TangraConfig.Settings.Tracking.AdHokMinCertainty;
		}

		public override void NextFrame(int frameNo, IAstroImage astroImage)
		{
			IsTrackedSuccessfully = false;

			var resolvedCandidates = new Dictionary<int, List<PSFFit>>();
			var candidateCenters = new List<List<IImagePixel>>();

			// For each of the Tracked objects, locate all objects (stars) in a fixed area around the previous known position
			for (int i = 0; i < m_TrackedObjects.Count; i++)
			{
				TrackedObjectLight trackedObject = (TrackedObjectLight)m_TrackedObjects[i];
				trackedObject.NextFrame();

				uint[,] pixels;
				List<IImagePixel> centers = FindCandidateStarsInArea(astroImage, trackedObject.LastKnownGoodPosition.X, trackedObject.LastKnownGoodPosition.Y, out pixels);
				List<PSFFit> candidateStars = RefineCandidateStarsInArea(trackedObject, trackedObject.LastKnownGoodPosition.X, trackedObject.LastKnownGoodPosition.Y, pixels, centers, trackedObject.RefinedFWHM, trackedObject.RefinedIMAX);
				resolvedCandidates.Add(trackedObject.TargetNo, candidateStars);
				candidateCenters.Add(centers);
			}
			
			// TODO: Now check the distances between the found objects to identify which are the correct ones. Start from an object with only one identified possibility or 
			if (resolvedCandidates.Values.All(x => x.Count == 1))
			{
				// There is exactly one candidate star for each object
				if (IsCandidateMatch(
					resolvedCandidates[0][0], 
					resolvedCandidates.Count > 1 ? resolvedCandidates[1][0] : null, 
					resolvedCandidates.Count > 2 ? resolvedCandidates[2][0] : null,
					resolvedCandidates.Count > 3 ? resolvedCandidates[3][0] : null))
				{
					// TODO: Resolve manually positioned objects

					IsTrackedSuccessfully = true;
				}
			}
			else
			{
				for (int i = 0; i < m_TrackedObjects.Count; i++)
					// If we didn't locate one of the objects, then simulate that we found a NULL object so the rest of the objects will be still checked
					if (resolvedCandidates[i].Count == 0) resolvedCandidates[i].Add(null);

				if (m_TrackedObjects.Count == 2)
					FindObjectMatchTwoTargets(resolvedCandidates.Values.ToList());
				else if (m_TrackedObjects.Count == 3)
					FindObjectMatchThreeTargets(resolvedCandidates.Values.ToList());
				else if (m_TrackedObjects.Count == 4)
					FindObjectMatchFourTargets(resolvedCandidates.Values.ToList());
				else
					throw new ArgumentOutOfRangeException();

			}


			IsTrackedSuccessfully = true;

			foreach (TrackedObjectBase trackedObject in m_TrackedObjects)
			{
				if (!trackedObject.IsLocated)
				{
					if (trackedObject.OriginalObject.IsFixedAperture)
					{
						// TODO: Locate manually positioned objects
					}
					else if (trackedObject.IsOccultedStar && m_IsFullDisappearance)
					{
						// Ignore the state of fully disappearing occulted stars when determining the "TrackedSuccessfully" state
					}
					else
						IsTrackedSuccessfully = false;
				}
			}

			if (IsTrackedSuccessfully)
			{
				// TODO: After every successful identification update the following:
	
				// - Average FWHM of all tracked objects
				// - Average distances in the past 25 frames
				// - Record the change in the average distances in the previous sets of 25 frames. Use this to derive the average distances when identifying objects and make sure small drifts are allowed 
				//   to cater for relative object movement and field rotation
				RefinedAverageFWHM = m_TrackedObjects.Cast<TrackedObjectLight>().Average(x => x.RefinedFWHM);
				UpdateMeasuredRelativeDistances();
			}

			Bitmap bmp = new Bitmap(m_TrackedObjects.Count* 200, 400, PixelFormat.Format24bppRgb);
			using(Graphics g = Graphics.FromImage(bmp))
			{
				for (int i = 0; i < m_TrackedObjects.Count; i++)
				{
					m_TrackedObjects[i].PSFFit.DrawDataPixels(g, new Rectangle(i * 200 + 15, 15, 170, 170), m_TrackedObjects[i].OriginalObject.ApertureInPixels, Pens.Yellow, 8, 0);
					m_TrackedObjects[i].PSFFit.DrawGraph(g, new Rectangle(i * 200 + 2, 200, 180, 180), 8, 0);

					foreach (IImagePixel candidateStar in candidateCenters[i])
					{
						float x = i * 200 + 15 + 85 + (float)(m_TrackedObjects[i].PSFFit.XCenter - candidateStar.XDouble) * 10;
						float y = 15 + 85 + (float)(m_TrackedObjects[i].PSFFit.YCenter - candidateStar.YDouble) * 10;

						g.DrawLine(Pens.LawnGreen, x - 5, y, x + 5, y);
						g.DrawLine(Pens.LawnGreen, x, y - 5, x, y + 5);
					}
				}

				g.Flush();
			}
			bmp.Save(@"D:\Hristo\Tangra3\Test Data\TrackingDebug\" + frameNo.ToString() + ".bmp");
		}

		private void FindObjectMatchTwoTargets(List<List<PSFFit>> candidateObjects)
		{
			for (int i0 = 0; i0 < candidateObjects[0].Count; i0++)
			{
				for (int i1 = 0; i1 < candidateObjects[1].Count; i1++)
				{
					if (IsCandidateMatch(candidateObjects[0][i0], candidateObjects[1][i1], null, null))
					{
						return;
					}
				}
			}
		}

		private void FindObjectMatchThreeTargets(List<List<PSFFit>> candidateObjects)
		{
			for (int i0 = 0; i0 < candidateObjects[0].Count; i0++)
			{
				for (int i1 = 0; i1 < candidateObjects[1].Count; i1++)
				{
					for (int i2 = 0; i2 < candidateObjects[2].Count; i2++)
					{
						if (IsCandidateMatch(candidateObjects[0][i0], candidateObjects[1][i1], candidateObjects[2][i2], null))
						{
							return;
						}
					}
				}
			}
		}

		private void FindObjectMatchFourTargets(List<List<PSFFit>> candidateObjects)
		{
			for (int i0 = 0; i0 < candidateObjects[0].Count; i0++)
			{
				for (int i1 = 0; i1 < candidateObjects[1].Count; i1++)
				{
					for (int i2 = 0; i2 < candidateObjects[2].Count; i2++)
					{
						for (int i3 = 0; i3 < candidateObjects[3].Count; i3++)
						{
							if (IsCandidateMatch(candidateObjects[0][i0], candidateObjects[1][i1], candidateObjects[2][i2], candidateObjects[3][i3]))
							{
								return;
							}
						}
					}
				}
			}
		}
		 
		private void UpdateMeasuredRelativeDistances()
		{
			for (int i = 0; i < m_TrackedObjects.Count; i++)
			{
				for (int j = 0; j < m_TrackedObjects.Count; j++)
				{
					List<double> distanceList = m_PastMeasuredRelativeDistances[i][j];
					if (distanceList.Count > 25) distanceList.RemoveAt(0);
					double distabce = m_TrackedObjects[i].Center.DistanceTo(m_TrackedObjects[j].Center);
					distanceList.Add(distabce);

					m_PastAverageRelativeDistances[i][j] = distanceList.Average();
				}
			}
		}

		private List<IImagePixel> FindCandidateStarsInArea(IAstroImage astroImage, int xCenter, int yCenter, out uint[,] pixels)
		{
			bool useBrightest = true;
			var rv = new List<IImagePixel>();
			uint medianNoise = astroImage.MedianNoise;

			int topXOffset = xCenter - 8;
			int topYOffset = yCenter - 8;

			pixels = astroImage.GetPixelsArea(xCenter, yCenter, 17);

			for (int x = 1; x < 10; x++)
			{
				for (int y = 1; y < 10; y++)
				{
					uint centerPixel = pixels[x, y];

					if (centerPixel >= pixels[x - 1, y - 1] && medianNoise < pixels[x - 1, y - 1] &&
					    centerPixel >= pixels[x, y - 1] && medianNoise < pixels[x, y - 1] &&
					    centerPixel >= pixels[x + 1, y - 1] && medianNoise < pixels[x + 1, y - 1] &&
					    centerPixel >= pixels[x - 1, y] && medianNoise < pixels[x - 1, y] &&
					    centerPixel >= pixels[x + 1, y] && medianNoise < pixels[x + 1, y] &&
					    centerPixel >= pixels[x - 1, y + 1] && medianNoise < pixels[x - 1, y + 1] &&
					    centerPixel >= pixels[x, y + 1] && medianNoise < pixels[x, y + 1] &&
					    centerPixel >= pixels[x + 1, y + 1] && medianNoise < pixels[x + 1, y + 1])
					{
						// Found a local maximum
						IImagePixel closestPixel = null;
						IImagePixel brightestPixel = null;
						double closestDistance = 1000;

						foreach (IImagePixel existingPixel in rv)
						{
							double distance = Math.Sqrt(Math.Pow(existingPixel.X - x - topXOffset, 2) + Math.Pow(existingPixel.Y - y - topYOffset, 2));
							if (distance < closestDistance)
							{
								closestDistance = distance;
								closestPixel = existingPixel;
							}
							if (brightestPixel == null || existingPixel.Brightness > brightestPixel.Brightness)
							{
								brightestPixel = existingPixel;
							}
						}

						if (useBrightest)
						{
							if (brightestPixel == null)
							{
								// This is the first candidate or no candidates are close enough to be part of the same star
								rv.Add(new ImagePixel(centerPixel, x + topXOffset, y + topYOffset));
							}
							else
							{
								if (brightestPixel.Brightness >= centerPixel)
								{
									// The closest existing candidate is close enough and brighter, so keep it
								}
								else
								{
									// The new candidate is brighter so replace the existing candidate
									rv.Remove(closestPixel);
									rv.Add(new ImagePixel(centerPixel, x + topXOffset, y + topYOffset));
								}
							}

						}
						else
						{
							if (closestPixel == null || closestDistance > MIN_SAME_STAR_DISTANCE)
							{
								// This is the first candidate or no candidates are close enough to be part of the same star
								rv.Add(new ImagePixel(centerPixel, x + topXOffset, y + topYOffset));
							}
							else if (closestDistance < MIN_SAME_STAR_DISTANCE && closestDistance > MAX_AUTO_ASSUME_DUPLICATE_STAR_DISTANCE)
							{
								if (closestPixel.Brightness >= centerPixel)
								{
									// The closest existing candidate is close enough and brighter, so keep it
								}
								else
								{
									// The new candidate is brighter so replace the existing candidate
									rv.Remove(closestPixel);
									rv.Add(new ImagePixel(centerPixel, x + topXOffset, y + topYOffset));
								}
							}							
						}
					}
				}
			}

			return rv;
		}

		private List<PSFFit> RefineCandidateStarsInArea(TrackedObjectLight trackedObject, int xCenter, int yCenter, uint[,] pixels, List<IImagePixel> candidateCenters, float expectedFWHM, float expectedIMAX)
		{
			var rv = new List<PSFFit>();
			for (int i = 0; i < candidateCenters.Count; i++)
			{
				IImagePixel center = candidateCenters[i];

				//uint[,] pixels = astroImage.GetPixelsArea(center.X, center.Y, 17);

				var fit = new PSFFit(xCenter, yCenter);
				fit.FittingMethod = PSFFittingMethod.NonLinearFit;
				fit.Fit(pixels);

				if (fit.IsSolved && fit.ElongationPercentage <= STELLAR_OBJECT_MAX_ELONGATION && fit.FWHM > STELLAR_OBJECT_MIN_FWHM && fit.FWHM < STELLAR_OBJECT_MAX_FWHM && fit.Certainty > STELLAR_OBJECT_MIN_CERTAINTY)
				{
					PSFFit closestPSFfit = null;
					double closestDistance = 1000;

					foreach (PSFFit existingPsf in rv)
					{
						double distance = Math.Sqrt(Math.Pow(existingPsf.XCenter - fit.XCenter, 2) + Math.Pow(existingPsf.YCenter - fit.YCenter, 2));
						if (distance < closestDistance)
						{
							closestDistance = distance;
							closestPSFfit = existingPsf;
						}
					}

					if (closestPSFfit == null || closestDistance > expectedFWHM || (closestDistance > MIN_SAME_STAR_DISTANCE && float.IsNaN(expectedFWHM)))
					{
						// This is the first candidate or no candidates are close enough to be part of the same star
						rv.Add(fit);
					}
					else if (closestDistance < expectedFWHM)
					{
						if (closestPSFfit.FWHM <= fit.FWHM)
						{
							// The closest existing candidate is thinner, so keep it
						}
						else
						{
							// The new candidate is thinner so replace the existing candidate
							rv.Remove(closestPSFfit);
							rv.Add(fit);
						}
					}
				}
				else
				{
					if (i == candidateCenters.Count - 1 && rv.Count == 0)
					{						
						// We are not going to get any candidate stars for this target. So we set the reason code
						if (fit.FWHM < STELLAR_OBJECT_MIN_FWHM || fit.FWHM > STELLAR_OBJECT_MAX_FWHM)
							trackedObject.SetIsTracked(false, NotMeasuredReasons.FWHMOutOfRange, fit);
						else if (!fit.IsSolved)
							trackedObject.SetIsTracked(false, NotMeasuredReasons.PSFFittingFailed, fit);
						else if (fit.ElongationPercentage > STELLAR_OBJECT_MAX_ELONGATION)
							trackedObject.SetIsTracked(false, NotMeasuredReasons.ObjectTooElongated, fit);
						else if (fit.Certainty < STELLAR_OBJECT_MIN_CERTAINTY)
							trackedObject.SetIsTracked(false, NotMeasuredReasons.ObjectCertaintyTooSmall, fit);
						else
							trackedObject.SetIsTracked(false, NotMeasuredReasons.UnknownReason, fit);

						//fit.ShowDialog(8);
					}
				}
			}			

			return rv;
		}

		private bool CheckCandidate(PSFFit[] allCandidates, FailedDistanceLogger distanceLogger, int candidateIndex)
		{
			if (m_TrackedObjects.Count > candidateIndex && !m_TrackedObjects[candidateIndex].OriginalObject.IsFixedAperture)
			{
				// Test distances from first object
				PSFFit candidate = allCandidates[candidateIndex];
				if (candidate != null)
				{
					List<double> expectedDistances = m_PastAverageRelativeDistances[candidateIndex];
					for (int i = 0; i < m_TrackedObjects.Count; i++)
					{
						if (i != 0 && !double.IsNaN(expectedDistances[i]))
						{
							bool failedFlag = false;

							if (allCandidates[i] != null)
							{
								double computedDistance = allCandidates[i].DistanceTo(candidate);
								if (Math.Abs(computedDistance - expectedDistances[i]) > PositionTolerance)
								{
									failedFlag = true;
								}
							}
							else
							{
								failedFlag = true;
							}

							if (failedFlag)
							{
								distanceLogger.MarkFailedDistanceCheck(candidateIndex, i);
							}
						}
					}
				}
				else
					distanceLogger.MarkFailedDistanceCheckForObject(candidateIndex);
			}

			return true;
		}

		private bool IsCandidateMatch(PSFFit candidate0, PSFFit candidate1, PSFFit candidate2, PSFFit candidate3)
		{
			bool isMatch = true;
			PSFFit[] allCandidates = new PSFFit[] {candidate0, candidate1, candidate2, candidate3};

			for (int i = 0; i < m_TrackedObjects.Count; i++)
			{
				if (allCandidates[i] != null)
					// We are going to attempt to locate all objects for which we have candidate stars. Others will have their not found reason already set
					((TrackedObjectLight)m_TrackedObjects[i]).NewMatchEvaluation();
			}

			var distanceLogger = new FailedDistanceLogger(m_TrackedObjects.Count);

			isMatch &= CheckCandidate(allCandidates, distanceLogger, 0);
			if (isMatch) isMatch &= CheckCandidate(allCandidates, distanceLogger, 1);
			if (isMatch) isMatch &= CheckCandidate(allCandidates, distanceLogger, 2);
			if (isMatch) isMatch &= CheckCandidate(allCandidates, distanceLogger, 3);

			if (isMatch)
			{
				bool atLeastOneDistanceOkay = false;

				for (int i = 0; i < m_TrackedObjects.Count; i++)
				{
					var trackedObject = (TrackedObjectLight) m_TrackedObjects[i];

					if (distanceLogger.AreDistancesOkay(i))
					{
						// Some objects were not found at the right distance
						trackedObject.SetTrackedObjectMatch(allCandidates[i]);
						atLeastOneDistanceOkay = true;
					}
					else if (allCandidates[i] != null)
					{
						if (trackedObject.OriginalObject.TrackingType == TrackingType.OccultedStar && m_IsFullDisappearance)
							trackedObject.SetIsTracked(false, NotMeasuredReasons.FullyDisappearingStarMarkedTrackedWithoutBeingFound, allCandidates[i]);
						else
							trackedObject.SetIsTracked(false, NotMeasuredReasons.FailedToLocateAfterDistanceCheck, allCandidates[i]);
					}
					else if (allCandidates[i] != null)
					{
						// No candidate stars were located. The not location reason sould have been already set
						trackedObject.SetIsTracked(false, NotMeasuredReasons.UnknownReason, allCandidates[i]);
					}
				}

				if (!atLeastOneDistanceOkay)
					isMatch = false;
			}

			return isMatch;
		}
	}
}
