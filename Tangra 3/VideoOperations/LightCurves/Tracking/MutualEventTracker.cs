using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Model.Image;
using Tangra.Model.VideoOperations;

namespace Tangra.VideoOperations.LightCurves.Tracking
{
	public class MutualEventTrackerException : Exception
	{
		public MutualEventTrackerException(string message)
			: base(message)
		{ }
	}

	public class TrackedObjectGroup
	{
		private List<ITrackedObject> m_ObjectGroup = new List<ITrackedObject>();

		private bool m_IsFullDisappearance;

		public TrackedObjectGroup(ITrackedObject[] group, bool isFullDisappearance)
		{
			m_IsFullDisappearance = isFullDisappearance;
			m_ObjectGroup.AddRange(group);

			TrackLater =
				m_IsFullDisappearance &&
				m_ObjectGroup.Count == 1 && 
				m_ObjectGroup[0].OriginalObject.IsOcultedStar();
			
			IsSingleObject = m_ObjectGroup.Count == 1;
			if (IsSingleObject)
			{
				SingleObject = (TrackedObjectLight)m_ObjectGroup[0];
				BrigherOriginalObject = m_ObjectGroup[0].OriginalObject;
				ContainsOcultedStar = SingleObject.OriginalObject.IsOcultedStar();
			}
			else
			{
				if (m_ObjectGroup[0].OriginalObject.Gaussian.IMax > m_ObjectGroup[1].OriginalObject.Gaussian.IMax)
				{
					BrigherOriginalObject = m_ObjectGroup[0].OriginalObject;
					m_BrighterObjectIndex = 0;
					m_NonOccultedObjectIndex = m_ObjectGroup[0].OriginalObject.IsOcultedStar() ? 1 : 0;
				}
				else
				{
					BrigherOriginalObject = m_ObjectGroup[1].OriginalObject;
					m_BrighterObjectIndex = 1;
					m_NonOccultedObjectIndex = m_ObjectGroup[1].OriginalObject.IsOcultedStar() ? 0 : 1;
				}

				ContainsOcultedStar = 
					m_ObjectGroup[0].OriginalObject.IsOcultedStar() || 
					m_ObjectGroup[1].OriginalObject.IsOcultedStar();
			}
		}

		internal readonly TrackedObjectLight SingleObject;

		internal readonly ITrackedObjectConfig BrigherOriginalObject;

		internal readonly bool TrackLater;

		private readonly int m_BrighterObjectIndex = -1;
		private readonly int m_NonOccultedObjectIndex = -1;

		internal IImagePixel SingleObjectLastCenter
		{
			get { return m_ObjectGroup[0].LastKnownGoodPosition; }
		}

		internal IImagePixel BrigherObjectLastCenter
		{
			get { return m_ObjectGroup[m_BrighterObjectIndex].LastKnownGoodPosition; }
		}

		internal IImagePixel NonOccultedObjectLastCenter
		{
			get { return m_ObjectGroup[m_NonOccultedObjectIndex].LastKnownGoodPosition; }
		}

		internal IImagePixel LastCenterObject1
		{
			get { return m_ObjectGroup[0].LastKnownGoodPosition; }
		}

		internal IImagePixel LastCenterObject2
		{
			get { return m_ObjectGroup[1].LastKnownGoodPosition; }
		}

		internal readonly bool IsSingleObject;

		internal void NextFrame()
		{
			foreach (ITrackedObject obj in m_ObjectGroup)
			{
				TrackedObjectLight trackedObject = (TrackedObjectLight)obj;
				trackedObject.NextFrame();				
			}
		}

		internal void SetIsTracked(bool isMeasured, NotMeasuredReasons reason, PSFFit currentlyEstimatedfit)
		{
			foreach (ITrackedObject obj in m_ObjectGroup)
			{
				TrackedObjectLight trackedObject = (TrackedObjectLight)obj;
				trackedObject.SetIsTracked(isMeasured, reason, currentlyEstimatedfit);
			}
		}

		internal bool IsLocated
		{
			get
			{
				if (IsSingleObject)
				    return SingleObject.IsLocated;
				else
                    return m_ObjectGroup[m_BrighterObjectIndex].IsLocated;
			}
		}

		internal bool ContainsOcultedStar { get; private set; }

		internal bool IdentifyObjects(PSFFit fit1, PSFFit fit2, float minGuidingStarCertainty, out TrackedObjectLight obj1, out TrackedObjectLight obj2)
		{
            // Make sure the two are not too far away and are also not too close
            double centDiff = ImagePixel.ComputeDistance(fit1.XCenter, fit2.XCenter, fit1.YCenter, fit2.YCenter);
            double oldCentDiff = ImagePixel.ComputeDistance(LastCenterObject1.XDouble, LastCenterObject2.XDouble, LastCenterObject1.YDouble, LastCenterObject2.YDouble);
            double diffDistRatio = centDiff / oldCentDiff;

            if (diffDistRatio < 0.5 || diffDistRatio > 2)
            {
                obj1 = null;
                obj2 = null;
                return false;
            }

            bool fit1Brighter = fit1.Brightness > fit2.Brightness;
            bool fit1Certain = fit1.Certainty > minGuidingStarCertainty;
            bool fit2Certain = fit2.Certainty > minGuidingStarCertainty;
            bool brighterFitCertain = fit1Brighter ? fit1Certain : fit2Certain;

            if (!brighterFitCertain)
            {
                obj1 = null;
                obj2 = null;
                return false;
            }

			double d11 = ImagePixel.ComputeDistance(fit1.XCenter, LastCenterObject1.XDouble, fit1.YCenter, LastCenterObject1.YDouble);
			double d22 = ImagePixel.ComputeDistance(fit2.XCenter, LastCenterObject2.XDouble, fit2.YCenter, LastCenterObject2.YDouble);
			double d12 = ImagePixel.ComputeDistance(fit1.XCenter, LastCenterObject2.XDouble, fit1.YCenter, LastCenterObject2.YDouble);
			double d21 = ImagePixel.ComputeDistance(fit2.XCenter, LastCenterObject1.XDouble, fit2.YCenter, LastCenterObject1.YDouble);

			if (d11 < d12 && d22 < d21)
			{
				// 1 = 1; 2 = 2
				obj1 = (TrackedObjectLight)m_ObjectGroup[0];
				obj2 = (TrackedObjectLight)m_ObjectGroup[1];
				return true;
			}
			else if (d11 > d12 && d22 > d21)
			{
				// 1 = 2; 2 = 1
				obj1 = (TrackedObjectLight) m_ObjectGroup[1];
				obj2 = (TrackedObjectLight) m_ObjectGroup[0];
				return true;
			}

			double x11 = fit1.XCenter - LastCenterObject1.XDouble;
			double x22 = fit2.XCenter - LastCenterObject2.XDouble;
			double x12 = fit1.XCenter - LastCenterObject2.XDouble;
			double x21 = fit2.XCenter - LastCenterObject1.XDouble;
			int sign11 = Math.Sign(x11);
			int sign22 = Math.Sign(x22);
			int sign12 = Math.Sign(x12);
			int sign21 = Math.Sign(x21);
			
			if (sign11 == sign22 && sign12 != sign21 && Math.Abs(x11 - x22) < Math.Abs(x12 - x21))
			{
				// 1 = 1; 2 = 2
				obj1 = (TrackedObjectLight)m_ObjectGroup[0];
				obj2 = (TrackedObjectLight)m_ObjectGroup[1];
				return true;
			}
			else if (sign12 == sign21 && sign11 != sign22 && Math.Abs(x11 - x22) > Math.Abs(x12 - x21))
			{
				// 1 = 2; 2 = 1
				obj1 = (TrackedObjectLight)m_ObjectGroup[1];
				obj2 = (TrackedObjectLight)m_ObjectGroup[0];
				return true;
			}

		    double bDiff = Math.Abs(fit1.Brightness - fit2.Brightness);
            double bRatio = bDiff / Math.Max((double)fit1.Brightness, (double)fit2.Brightness);
            if (bRatio > 0.5)
            {
                // More than 2 times brightness difference. We can use the brightness to determine identify

                double b11 = Math.Abs(fit1.Brightness - LastCenterObject1.Brightness);
                double b22 = Math.Abs(fit2.Brightness - LastCenterObject2.Brightness);
                double b12 = Math.Abs(fit1.Brightness - LastCenterObject2.Brightness);
                double b21 = Math.Abs(fit2.Brightness - LastCenterObject1.Brightness);

                if ((fit1Brighter && b12 > b11 && fit1Certain) || (!fit1Brighter && b21 > b22 && fit2Certain))
                {
                    // fit1 = last object 1
                    obj1 = (TrackedObjectLight)m_ObjectGroup[0];
                    obj2 = (TrackedObjectLight)m_ObjectGroup[1];
                    return true;
                }
                else if ((fit1Brighter && b12 < b11 && fit1Certain) || (!fit1Brighter && b21 < b22 && fit2Certain))
                {
                    // fit1 = last object 1
                    obj1 = (TrackedObjectLight)m_ObjectGroup[1];
                    obj2 = (TrackedObjectLight)m_ObjectGroup[0];
                    return true;
                }
            }

//			if (Math.Max(d11, d22) > Math.Max(d21, d12) && Math.Min(d11, d22) < Math.Min(d21, d12))
//			{
//				// 1 = 2; 2 = 1
//				obj1 = (TrackedObjectLight)m_ObjectGroup[1];
//				obj2 = (TrackedObjectLight)m_ObjectGroup[0];
//				return true;
//			}
//			else if (Math.Max(d11, d22) < Math.Max(d21, d12) && Math.Min(d11, d22) > Math.Min(d21, d12))
//			{
//				// 1 = 1; 2 = 2
//				obj1 = (TrackedObjectLight)m_ObjectGroup[0];
//				obj2 = (TrackedObjectLight)m_ObjectGroup[1];
//				return true;
//			}
//			else
			{
				obj1 = null;
				obj2 = null;
				return false;
			}
		}
	}

	public class MutualEventTracker : BaseTracker
	{
		private float STELLAR_OBJECT_MAX_ELONGATION;
		private float STELLAR_OBJECT_MIN_FWHM;
		private float STELLAR_OBJECT_MAX_FWHM;
		private float STELLAR_OBJECT_MIN_CERTAINTY;
		private float GUIDING_STAR_MIN_CERTAINTY;

		private bool m_IsFullDisappearance;

		private List<TrackedObjectGroup> m_TrackedObjectGroups = new List<TrackedObjectGroup>();

		internal MutualEventTracker(List<TrackedObjectConfig> measuringStars, bool isFullDisappearance)
			: base(measuringStars)
		{
			STELLAR_OBJECT_MAX_ELONGATION = TangraConfig.Settings.Tracking.AdHokMaxElongation;
			STELLAR_OBJECT_MIN_FWHM = TangraConfig.Settings.Tracking.AdHokMinFWHM;
			STELLAR_OBJECT_MAX_FWHM = TangraConfig.Settings.Tracking.AdHokMaxFWHM;
			STELLAR_OBJECT_MIN_CERTAINTY = TangraConfig.Settings.Tracking.AdHokMinCertainty;
			GUIDING_STAR_MIN_CERTAINTY = TangraConfig.Settings.Tracking.AdHokGuidingStarMinCertainty;

			m_IsFullDisappearance = isFullDisappearance;

			foreach (ITrackedObject obj in m_TrackedObjects)
                obj.LastKnownGoodPosition = new ImagePixel(obj.OriginalObject.Gaussian.Brightness, obj.OriginalObject.ApertureStartingX, obj.OriginalObject.ApertureStartingY);

			foreach (ITrackedObject nonGroupedObject in m_TrackedObjects.Where(x => !x.OriginalObject.ProcessInPsfGroup))
			{
				m_TrackedObjectGroups.Add(new TrackedObjectGroup(new ITrackedObject[] { nonGroupedObject }, isFullDisappearance));
			}

			List<ITrackedObject> groupedObjects = m_TrackedObjects.Where(x => x.OriginalObject.ProcessInPsfGroup).ToList();

			if (groupedObjects.Count%2 != 0) 
				throw new MutualEventTrackerException("Odd number of grouped objects is not currently supported.");

			int[] groupIds = groupedObjects
				.Select(x => x.OriginalObject.GroupId)
				.Distinct()
				.ToArray();

			foreach (int groupId in groupIds)
			{
				ITrackedObject[] objectsInGroup = groupedObjects.Where(x => x.OriginalObject.GroupId == groupId).ToArray();
				m_TrackedObjectGroups.Add(new TrackedObjectGroup(objectsInGroup, isFullDisappearance));
			}

			// TODO: Also pass the settings for background 3D-Polynomial fitting

			// TODO: What about fully disappearing targets and doing PSF photometry of them? While this is not a problem of the tracker, what do we do with all this?
		}


		public override void NextFrame(int frameNo, IAstroImage astroImage)
		{
			IsTrackedSuccessfully = false;

			// For each of the non manualy positioned Tracked objects do a PSF fit in the area of its previous location 
			for (int i = 0; i < m_TrackedObjectGroups.Count; i++)
			{
				TrackedObjectGroup objectGroup = m_TrackedObjectGroups[i];
				objectGroup.NextFrame();

				if (objectGroup.TrackLater)
				{
					// Group position will be determined after the rest of the stars are found 
				}
				else
				{
					if (objectGroup.IsSingleObject)
					{
						TrackedObjectLight trackedObject = (TrackedObjectLight) objectGroup.SingleObject;
						uint[,] pixels = astroImage.GetPixelsArea(objectGroup.SingleObjectLastCenter.X, objectGroup.SingleObjectLastCenter.Y, 17);
						var fit = new PSFFit(objectGroup.SingleObjectLastCenter.X, objectGroup.SingleObjectLastCenter.Y);
						fit.FittingMethod = PSFFittingMethod.NonLinearFit;
						fit.Fit(pixels);

						if (fit.IsSolved)
						{
							if (fit.Certainty < GUIDING_STAR_MIN_CERTAINTY)
							{
								trackedObject.SetIsTracked(false, NotMeasuredReasons.ObjectCertaintyTooSmall, (PSFFit) null);
							}
							else if (fit.FWHM < STELLAR_OBJECT_MIN_FWHM || fit.FWHM > STELLAR_OBJECT_MAX_FWHM)
							{
								trackedObject.SetIsTracked(false, NotMeasuredReasons.FWHMOutOfRange, (PSFFit) null);
							}
							else if (TangraConfig.Settings.Tracking.CheckElongation && fit.ElongationPercentage > STELLAR_OBJECT_MAX_ELONGATION)
							{
								trackedObject.SetIsTracked(false, NotMeasuredReasons.ObjectTooElongated, (PSFFit) null);
							}
							else
							{
								trackedObject.SetTrackedObjectMatch(fit);
								trackedObject.SetIsTracked(true, NotMeasuredReasons.TrackedSuccessfully, fit);
							}
						}
					}
					else
					{
						int areaCenterX = objectGroup.BrigherObjectLastCenter.X;
						int areaCenterY = objectGroup.BrigherObjectLastCenter.Y;

						uint[,] pixels = astroImage.GetPixelsArea(areaCenterX, areaCenterY, 35);
						var doubleFit = new DoublePSFFit(areaCenterX, areaCenterY);

						int x1 = objectGroup.LastCenterObject1.X - areaCenterX + 17;
						int y1 = objectGroup.LastCenterObject1.Y - areaCenterY + 17;
						int x2 = objectGroup.LastCenterObject2.X - areaCenterX + 17;
						int y2 = objectGroup.LastCenterObject2.Y - areaCenterY + 17;

						if (TangraConfig.Settings.Photometry.PsfFittingMethod == TangraConfig.PsfFittingMethod.LinearFitOfAveragedModel)
							doubleFit.FittingMethod = PSFFittingMethod.LinearFitOfAveragedModel;
						doubleFit.Fit(pixels, x1, y1, x2, y2);

						if (doubleFit.IsSolved)
						{
							PSFFit fit1 = doubleFit.GetGaussian1();
							PSFFit fit2 = doubleFit.GetGaussian2();

							TrackedObjectLight trackedObject1;
							TrackedObjectLight trackedObject2;

							bool groupIdentified = objectGroup.IdentifyObjects(fit1, fit2, GUIDING_STAR_MIN_CERTAINTY, out trackedObject1, out trackedObject2);
							if (!groupIdentified)
							{
								objectGroup.SetIsTracked(false, NotMeasuredReasons.PSFFittingFailed, null);
								//Bitmap bmp = new Bitmap(100, 200);
								//using (Graphics g = Graphics.FromImage(bmp))
								//{
								//	doubleFit.DrawInternalPoints(g, new Rectangle(0,0, 100, 200), 5, 5, Brushes.Lime, Brushes.Yellow, 8);
								//	g.Save();
								//}
								//bmp.Save(@"D:\Hristo\mutual_double_fit.bmp");
							}
							else
							{
								PSFFit[] fits = new PSFFit[] { fit1, fit2 };
								TrackedObjectLight[] trackedObjects = new TrackedObjectLight[] { trackedObject1, trackedObject2 };

							    int tooSmallCertainties = 0;
							    int errors = 0;

								for (int j = 0; j < 2; j++)
								{
									PSFFit fit = fits[j];
									TrackedObjectLight trackedObject = trackedObjects[j];

									if (fit.Certainty < GUIDING_STAR_MIN_CERTAINTY)
									{
									    tooSmallCertainties++;
                                        trackedObject.SetIsTracked(true, NotMeasuredReasons.TrackedSuccessfully, fit);
									}
									else if (fit.FWHM < STELLAR_OBJECT_MIN_FWHM || fit.FWHM > STELLAR_OBJECT_MAX_FWHM)
									{
										trackedObject.SetIsTracked(false, NotMeasuredReasons.FWHMOutOfRange, (PSFFit)null);
									    errors++;
									}
									else if (TangraConfig.Settings.Tracking.CheckElongation && fit.ElongationPercentage > STELLAR_OBJECT_MAX_ELONGATION)
									{
										trackedObject.SetIsTracked(false, NotMeasuredReasons.ObjectTooElongated, (PSFFit)null);
                                        errors++;
									}
									else
									{
										
										trackedObject.SetIsTracked(true, NotMeasuredReasons.TrackedSuccessfully, fit);
									}
								}

                                if (tooSmallCertainties == 2)
                                {
                                    trackedObjects[0].SetIsTracked(false, NotMeasuredReasons.ObjectCertaintyTooSmall, (PSFFit)null);
                                    trackedObjects[1].SetIsTracked(false, NotMeasuredReasons.ObjectCertaintyTooSmall, (PSFFit)null);
                                    errors++;
                                }

                                if (errors == 0)
                                {
                                    trackedObjects[0].SetTrackedObjectMatch(fits[0]);
                                    trackedObjects[1].SetTrackedObjectMatch(fits[1]);
                                }
							}
						}

					}
				}
			}

			bool atLeastOneGroupLocated = false;

			for (int i = 0; i < m_TrackedObjectGroups.Count; i++)
			{
				TrackedObjectGroup trackedGroup = m_TrackedObjectGroups[i];

				bool containsFullyDisappearingTarget = trackedGroup.ContainsOcultedStar && m_IsFullDisappearance;

				if (!containsFullyDisappearingTarget && trackedGroup.IsLocated)
					atLeastOneGroupLocated = true;

				if (!containsFullyDisappearingTarget)
					continue;

				int numReferences = 0;
				double x_double;
				double y_double;	

				if (trackedGroup.IsSingleObject && m_IsFullDisappearance)
				{
					// This is the case for single fully disappearing targets
					double totalX = 0;
					double totalY = 0;
					numReferences = 0;

					for (int j = 0; j < m_TrackedObjectGroups.Count; j++)
					{
						TrackedObjectGroup referenceGroup = (TrackedObjectGroup)m_TrackedObjectGroups[j];
						if (referenceGroup.IsLocated)
						{
							totalX += (trackedGroup.BrigherOriginalObject.ApertureStartingX - referenceGroup.BrigherOriginalObject.ApertureStartingX) + referenceGroup.BrigherObjectLastCenter.XDouble;
							totalY += (trackedGroup.BrigherOriginalObject.ApertureStartingY - referenceGroup.BrigherOriginalObject.ApertureStartingY) + referenceGroup.BrigherObjectLastCenter.YDouble;
							numReferences++;
							atLeastOneGroupLocated = true;
						}
					}

					x_double = totalX / numReferences;
					y_double = totalY / numReferences;					
				}
				else
				{
					// The fully disappearing target is in a group. The other target would have been located. We use the last known position of the other targets
					numReferences = 1;
					x_double = trackedGroup.NonOccultedObjectLastCenter.XDouble;
					y_double = trackedGroup.NonOccultedObjectLastCenter.YDouble;	
				}


				if (numReferences == 0)
				{
					trackedGroup.SetIsTracked(false, NotMeasuredReasons.FitSuspectAsNoGuidingStarsAreLocated, (PSFFit)null);
				}
				else
				{
					int x = (int)(Math.Round(x_double));
					int y = (int)(Math.Round(y_double));

					uint[,] pixels = astroImage.GetPixelsArea(x, y, 35);

					if (trackedGroup.IsSingleObject)
					{
						PSFFit fit = new PSFFit(x, y);

						fit.Fit(pixels);

						if (fit.IsSolved && fit.Certainty > STELLAR_OBJECT_MIN_CERTAINTY)
						{
							trackedGroup.SingleObject.SetIsTracked(true, NotMeasuredReasons.TrackedSuccessfully, fit);
							trackedGroup.SingleObject.SetTrackedObjectMatch(fit);
						}
						else if (m_IsFullDisappearance)
							trackedGroup.SingleObject.SetIsTracked(false, NotMeasuredReasons.FullyDisappearingStarMarkedTrackedWithoutBeingFound, (PSFFit)null);
						else
							trackedGroup.SingleObject.SetIsTracked(false, NotMeasuredReasons.ObjectCertaintyTooSmall, (PSFFit)null);
					}
					else
					{
						DoublePSFFit doubleFit = new DoublePSFFit(x, y);

						int x1 = trackedGroup.LastCenterObject1.X - x + 17;
						int y1 = trackedGroup.LastCenterObject1.Y - y + 17;
						int x2 = trackedGroup.LastCenterObject2.X - x + 17;
						int y2 = trackedGroup.LastCenterObject2.Y - y + 17;

						if (TangraConfig.Settings.Photometry.PsfFittingMethod == TangraConfig.PsfFittingMethod.LinearFitOfAveragedModel)
							doubleFit.FittingMethod = PSFFittingMethod.LinearFitOfAveragedModel;

						doubleFit.Fit(pixels, x1, y1, x2, y2);

						if (doubleFit.IsSolved)
						{
							PSFFit fit1 = doubleFit.GetGaussian1();
							PSFFit fit2 = doubleFit.GetGaussian2();

							TrackedObjectLight trackedObject1;
							TrackedObjectLight trackedObject2;

							bool groupIdentified = trackedGroup.IdentifyObjects(fit1, fit2, GUIDING_STAR_MIN_CERTAINTY, out trackedObject1, out trackedObject2);
							if (!groupIdentified)
							{
								trackedGroup.SetIsTracked(false, NotMeasuredReasons.PSFFittingFailed, null);
							}
							else
							{
								PSFFit[] fits = new PSFFit[] { fit1, fit2 };
								TrackedObjectLight[] trackedObjects = new TrackedObjectLight[] { trackedObject1, trackedObject2 };

								for (int j = 0; j < 2; j++)
								{
									PSFFit fit = fits[j];
									TrackedObjectLight trackedObject = trackedObjects[j];

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
					}
				}
			}

			IsTrackedSuccessfully = atLeastOneGroupLocated;

			if (IsTrackedSuccessfully)
				RefinedAverageFWHM = m_TrackedObjects.Cast<TrackedObjectLight>().Average(x => x.RefinedFWHM);
		}
	}
}
