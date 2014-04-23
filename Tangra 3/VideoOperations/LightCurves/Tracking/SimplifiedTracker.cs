using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Model.Image;
using Tangra.Model.VideoOperations;

namespace Tangra.VideoOperations.LightCurves.Tracking
{
	public class SimplifiedTracker : BaseTracker
	{
		private float STELLAR_OBJECT_MAX_ELONGATION;
		private float STELLAR_OBJECT_MIN_FWHM;
		private float STELLAR_OBJECT_MAX_FWHM;
		private float STELLAR_OBJECT_MIN_CERTAINTY;
		private float GUIDING_STAR_MIN_CERTAINTY;

		internal SimplifiedTracker(List<TrackedObjectConfig> measuringStars)
			: base(measuringStars)
		{
			STELLAR_OBJECT_MAX_ELONGATION = TangraConfig.Settings.Tracking.AdHokMaxElongation;
			STELLAR_OBJECT_MIN_FWHM = TangraConfig.Settings.Tracking.AdHokMinFWHM;
			STELLAR_OBJECT_MAX_FWHM = TangraConfig.Settings.Tracking.AdHokMaxFWHM;
			STELLAR_OBJECT_MIN_CERTAINTY = TangraConfig.Settings.Tracking.AdHokMinCertainty;
			GUIDING_STAR_MIN_CERTAINTY = TangraConfig.Settings.Tracking.AdHokGuidingStarMinCertainty;
		}

		public override void NextFrame(int frameNo, IAstroImage astroImage)
		{
			IsTrackedSuccessfully = false;

			// For each of the non manualy positioned Tracked objects do a PSF fit in the area of its previous location 
			for (int i = 0; i < m_TrackedObjects.Count; i++)
			{
				TrackedObjectLight trackedObject = (TrackedObjectLight)m_TrackedObjects[i];
				trackedObject.NextFrame();

				if (trackedObject.OriginalObject.IsFixedAperture || trackedObject.OriginalObject.TrackingType == TrackingType.OccultedStar)
				{
					// Star position will be determined after the rest of the stars are found 
				}
				else
				{
					uint[,] pixels = astroImage.GetPixelsArea(trackedObject.Center.X, trackedObject.Center.Y, 17);
					var fit = new PSFFit(trackedObject.Center.X, trackedObject.Center.Y);
					fit.FittingMethod = TangraConfig.Settings.Tracking.CheckElongation ? PSFFittingMethod.NonLinearAsymetricFit : PSFFittingMethod.NonLinearFit;
					fit.Fit(pixels);

					if (fit.IsSolved)
					{
						if (fit.Certainty < GUIDING_STAR_MIN_CERTAINTY)
						{
							trackedObject.SetIsTracked(false, NotMeasuredReasons.ObjectCertaintyTooSmall, (PSFFit)null);
						}
						else if (fit.FWHM < STELLAR_OBJECT_MIN_FWHM || fit.FWHM > STELLAR_OBJECT_MAX_FWHM)
						{
							trackedObject.SetIsTracked(false, NotMeasuredReasons.FWHMOutOfRange, (PSFFit)null);
						}
						else if (TangraConfig.Settings.Tracking.CheckElongation && fit.ElongationPercentage > STELLAR_OBJECT_MAX_ELONGATION)
						{
							trackedObject.SetIsTracked(false, NotMeasuredReasons.ObjectTooElongated, (PSFFit)null);
						}
						else
						{
							trackedObject.SetTrackedObjectMatch(fit);
							trackedObject.SetIsTracked(true, NotMeasuredReasons.TrackedSuccessfully, fit);
						}
					}
				}					
			}

			bool atLeastOneObjectLocated = false;

			for (int i = 0; i < m_TrackedObjects.Count; i++)
			{
				TrackedObjectLight trackedObject = (TrackedObjectLight) m_TrackedObjects[i];

				double totalX = 0;
				double totalY = 0;
				int numReferences = 0;
				
				for (int j = 0; j < m_TrackedObjects.Count; j++)
				{
					TrackedObjectLight referenceObject = (TrackedObjectLight)m_TrackedObjects[j];
					if (referenceObject.IsLocated)
					{
						totalX += (trackedObject.OriginalObject.ApertureStartingX - referenceObject.OriginalObject.ApertureStartingX) + referenceObject.Center.XDouble;
						totalY += (trackedObject.OriginalObject.ApertureStartingY - referenceObject.OriginalObject.ApertureStartingY) + referenceObject.Center.YDouble;
						numReferences++;
						atLeastOneObjectLocated = true;
					}
				}

				if (numReferences == 0)
				{
					trackedObject.SetIsTracked(false, NotMeasuredReasons.FitSuspectAsNoGuidingStarsAreLocated, (PSFFit)null);
				}
				else
				{
					double x_double = totalX / numReferences;
					double y_double = totalY / numReferences;

					if (trackedObject.OriginalObject.IsFixedAperture)
					{
						trackedObject.SetIsTracked(true, NotMeasuredReasons.FixedObject, new ImagePixel(x_double, y_double));
					}
					else if (trackedObject.OriginalObject.IsOcultedStar())
					{
						int x = (int)(Math.Round(totalX / numReferences));
						int y = (int)(Math.Round(totalY / numReferences));

						int matrixSize = (int)(Math.Round(trackedObject.OriginalObject.ApertureInPixels * 1.5));
						if (matrixSize % 2 == 0) matrixSize++;
						if (matrixSize > 17) matrixSize = 17;

						uint[,] pixels = astroImage.GetPixelsArea(x, y, matrixSize);


						PSFFit fit = new PSFFit(x, y);
						
						fit.Fit(pixels);


						if (fit.IsSolved && fit.Certainty > STELLAR_OBJECT_MIN_CERTAINTY)
						{
							trackedObject.SetIsTracked(true, NotMeasuredReasons.TrackedSuccessfully, fit);
							trackedObject.SetTrackedObjectMatch(fit);
						}
						else if (m_IsFullDisappearance)
							trackedObject.SetIsTracked(false, NotMeasuredReasons.FullyDisappearingStarMarkedTrackedWithoutBeingFound, (PSFFit)null);
						else
							trackedObject.SetIsTracked(false, NotMeasuredReasons.ObjectCertaintyTooSmall, (PSFFit)null);
					}
				}
			}

			IsTrackedSuccessfully = atLeastOneObjectLocated;
		}
	}
}
