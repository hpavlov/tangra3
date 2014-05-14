using System;
using System.Collections.Generic;
using System.Diagnostics;
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

			IsSingleObject = m_ObjectGroup.Count == 1;
			if (IsSingleObject)
			{
				SingleObject = (TrackedObjectLight)m_ObjectGroup[0];
				BrigherOriginalObject = m_ObjectGroup[0].OriginalObject;
				ContainsOcultedStar = SingleObject.OriginalObject.IsOcultedStar();
				m_LastDeltaX = double.NaN;
				m_LastDeltaY = double.NaN;
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

				m_LastDeltaX = m_ObjectGroup[0].LastKnownGoodPosition.XDouble - m_ObjectGroup[1].LastKnownGoodPosition.XDouble;
				m_LastDeltaY = m_ObjectGroup[0].LastKnownGoodPosition.YDouble - m_ObjectGroup[1].LastKnownGoodPosition.YDouble;
			}

			TrackLater = m_IsFullDisappearance && ContainsOcultedStar;

			m_CurrentXVector = double.NaN;
			m_CurrentYVector = double.NaN;
		}

		internal readonly TrackedObjectLight SingleObject;

		internal readonly ITrackedObjectConfig BrigherOriginalObject;

		internal readonly bool TrackLater;

		private readonly int m_BrighterObjectIndex = -1;
		private readonly int m_NonOccultedObjectIndex = -1;

		private List<double> m_RecentXVectors = new List<double>();

		private List<double> m_RecentYVectors = new List<double>();

		private double m_CurrentXVector = double.NaN;
		private double m_CurrentYVector = double.NaN;
		private double m_LastDeltaX = double.NaN;
		private double m_LastDeltaY = double.NaN;

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

		internal void SetIsTracked(bool isMeasured, NotMeasuredReasons reason)
		{
			foreach (ITrackedObject obj in m_ObjectGroup)
			{
				TrackedObjectLight trackedObject = (TrackedObjectLight)obj;
				trackedObject.SetIsTracked(isMeasured, reason);
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

		private bool NoMatch(out TrackedObjectLight obj1, out TrackedObjectLight obj2)
		{
			obj1 = null;
			obj2 = null;
			return false;
		}

		private bool Match1122(out TrackedObjectLight obj1, out TrackedObjectLight obj2)
		{
			obj1 = (TrackedObjectLight)m_ObjectGroup[0];
			obj2 = (TrackedObjectLight)m_ObjectGroup[1];
			return true;
		}

		private bool Match1212(out TrackedObjectLight obj1, out TrackedObjectLight obj2)
		{
			obj1 = (TrackedObjectLight)m_ObjectGroup[1];
			obj2 = (TrackedObjectLight)m_ObjectGroup[0];
			return true;
		}

		internal bool IdentifyObjects(PSFFit fit1, PSFFit fit2, float minGuidingStarCertainty, out TrackedObjectLight obj1, out TrackedObjectLight obj2)
		{
            // Make sure the two are not too far away and are also not too close
            double centDiff = ImagePixel.ComputeDistance(fit1.XCenter, fit2.XCenter, fit1.YCenter, fit2.YCenter);
            double oldCentDiff = ImagePixel.ComputeDistance(LastCenterObject1.XDouble, LastCenterObject2.XDouble, LastCenterObject1.YDouble, LastCenterObject2.YDouble);
            double diffDistRatio = centDiff / oldCentDiff;

			if (diffDistRatio < 0.5 || diffDistRatio > 2 || centDiff < 1 /* Two PSFs in the group are too close (possibly the same) */)
				return NoMatch(out obj1, out obj2);

            bool fit1Brighter = fit1.Brightness > fit2.Brightness;
            bool fit1Certain = fit1.Certainty > minGuidingStarCertainty;
            bool fit2Certain = fit2.Certainty > minGuidingStarCertainty;
            bool brighterFitCertain = fit1Brighter ? fit1Certain : fit2Certain;

            if (!brighterFitCertain)
				return NoMatch(out obj1, out obj2);

			double prevX1X2 = LastCenterObject1.XDouble - LastCenterObject2.XDouble;
			double prevY1Y2 = LastCenterObject1.YDouble - LastCenterObject2.YDouble;
			double thisX1X2 = fit1.XCenter - fit2.XCenter;
			double thisY1Y2 = fit1.YCenter - fit2.YCenter;
			double delta1122 = Math.Abs(prevX1X2 - thisX1X2) + Math.Abs(prevY1Y2 - thisY1Y2);
			double delta1212 = Math.Abs(prevX1X2 + thisX1X2) + Math.Abs(prevY1Y2 + thisY1Y2);

			if (delta1122 < 1 && delta1212 > 4)
				return Match1122(out obj1, out obj2);
			else if (delta1212 < 1 && delta1122 > 4)
				return Match1212(out obj1, out obj2);

			bool match1122Ok = (Math.Abs(prevX1X2 - thisX1X2) < Math.Abs(prevX1X2 + thisX1X2)) && (Math.Abs(prevY1Y2 - thisY1Y2) < Math.Abs(prevY1Y2 + thisY1Y2));
			bool match1212Ok = (Math.Abs(prevX1X2 + thisX1X2) < Math.Abs(prevX1X2 - thisX1X2)) && (Math.Abs(prevY1Y2 + thisY1Y2) < Math.Abs(prevY1Y2 - thisY1Y2));

			if ((!match1122Ok && !match1212Ok) || (match1122Ok && match1212Ok))
				return NoMatch(out obj1, out obj2);

			double d11 = ImagePixel.ComputeDistance(fit1.XCenter, LastCenterObject1.XDouble, fit1.YCenter, LastCenterObject1.YDouble);
			double d22 = ImagePixel.ComputeDistance(fit2.XCenter, LastCenterObject2.XDouble, fit2.YCenter, LastCenterObject2.YDouble);
			double d12 = ImagePixel.ComputeDistance(fit1.XCenter, LastCenterObject2.XDouble, fit1.YCenter, LastCenterObject2.YDouble);
			double d21 = ImagePixel.ComputeDistance(fit2.XCenter, LastCenterObject1.XDouble, fit2.YCenter, LastCenterObject1.YDouble);

			if (d11 < d12 && d22 < d21 && match1122Ok)
				return Match1122(out obj1, out obj2);
			else if (d11 > d12 && d22 > d21 && match1212Ok)
				return Match1212(out obj1, out obj2);

		    double bDiff = Math.Abs(fit1.Brightness - fit2.Brightness);
            double bRatio = bDiff / Math.Max((double)fit1.Brightness, (double)fit2.Brightness);
            if (bRatio > 0.5)
            {
                // More than 2 times brightness difference. We can use the brightness to determine identify

                double b11 = Math.Abs(fit1.Brightness - LastCenterObject1.Brightness);
                double b22 = Math.Abs(fit2.Brightness - LastCenterObject2.Brightness);
                double b12 = Math.Abs(fit1.Brightness - LastCenterObject2.Brightness);
                double b21 = Math.Abs(fit2.Brightness - LastCenterObject1.Brightness);

				if (((fit1Brighter && b12 > b11 && fit1Certain) || (!fit1Brighter && b21 > b22 && fit2Certain)) && match1122Ok)
					return Match1122(out obj1, out obj2);
				else if (((fit1Brighter && b12 < b11 && fit1Certain) || (!fit1Brighter && b21 < b22 && fit2Certain)) == match1212Ok)
					return Match1212(out obj1, out obj2);
            }

			return NoMatch(out obj1, out obj2);
		}

		internal bool IdentifyBrightObject(PSFFit fit1, PSFFit fit2, float minStarCertainty, out TrackedObjectLight obj1, out TrackedObjectLight obj2, out IImagePixel center1, out IImagePixel center2)
		{
			double brightness1 = (fit1.Certainty > minStarCertainty ? fit1.Brightness : 1);
			double brightness2 = (fit2.Certainty > minStarCertainty ? fit2.Brightness : 1);
			double bDiff = Math.Abs(brightness1 - brightness2);
			double bRatio = bDiff / Math.Max(brightness1, brightness2);

			if (bRatio > 0.5)
			{
				bool fit1Brighter = brightness1 > brightness2;
				bool oldFit1Brighter = LastCenterObject1.Brightness > LastCenterObject2.Brightness;
				double oldDeltaXBrightFaint = oldFit1Brighter ? LastCenterObject1.XDouble - LastCenterObject2.XDouble : LastCenterObject2.XDouble - LastCenterObject1.XDouble;
				double oldDeltaYBrightFaint = oldFit1Brighter ? LastCenterObject1.YDouble - LastCenterObject2.YDouble : LastCenterObject2.YDouble - LastCenterObject1.YDouble;

				if (!(fit1Brighter ^ oldFit1Brighter))
				{
					// 1 == 1; 2 == 2
					if (fit1Brighter)
					{
						center1 = new ImagePixel((int)brightness1, fit1.XCenter, fit1.YCenter);
						center2 = new ImagePixel((int)brightness2, fit1.XCenter - oldDeltaXBrightFaint, fit1.YCenter - oldDeltaYBrightFaint);
						return Match1122(out obj1, out obj2);
					}
					else
					{
						center1 = new ImagePixel((int)brightness2, fit2.XCenter, fit2.YCenter);
						center2 = new ImagePixel((int)brightness1, fit2.XCenter - oldDeltaXBrightFaint, fit2.YCenter - oldDeltaYBrightFaint);
						return Match1122(out obj1, out obj2);
					}
				}
				else
				{
					// 1 == 2; 2 == 1
					if (fit1Brighter)
					{
						center2 = new ImagePixel((int)brightness1, fit1.XCenter, fit1.YCenter);
						center1 = new ImagePixel((int)brightness2, fit1.XCenter - oldDeltaXBrightFaint, fit1.YCenter - oldDeltaYBrightFaint);
						return Match1212(out obj1, out obj2);
					}
					else
					{
						center2 = new ImagePixel((int)brightness2, fit2.XCenter, fit2.YCenter);
						center1 = new ImagePixel((int)brightness1, fit2.XCenter - oldDeltaXBrightFaint, fit2.YCenter - oldDeltaYBrightFaint);
						return Match1212(out obj1, out obj2);
					}
				}
			}

			center1 = null;
			center2 = null;
			return NoMatch(out obj1, out obj2);
		}

		internal bool CheckIdentifiedObjects(double x1, double x2, double y1, double y2)
		{
			if (!double.IsNaN(m_CurrentXVector))
			{
				if (Math.Abs(x1 - x2 - m_CurrentXVector) > 2 || Math.Abs(y1 - y2 - m_CurrentYVector) > 2)
				{
					return false;
				}
			}

			while (m_RecentXVectors.Count >= 10) m_RecentXVectors.RemoveAt(0);
			while (m_RecentYVectors.Count >= 10) m_RecentYVectors.RemoveAt(0);

			m_LastDeltaX = x1 - x2;
			m_LastDeltaY = y1 - y2;

			m_RecentXVectors.Add(m_LastDeltaX);
			m_RecentYVectors.Add(m_LastDeltaY);

			if (m_RecentXVectors.Count >= 10)
			{
				m_CurrentXVector = m_RecentXVectors.Average();
				m_CurrentYVector = m_RecentYVectors.Average();
			}
			else
			{
				m_CurrentXVector = double.NaN;
				m_CurrentYVector = double.NaN;
			}

			if (Math.Abs(m_LastDeltaX - m_CurrentXVector) > 2 || Math.Abs(m_LastDeltaY - m_CurrentYVector) > 2)
			{
				Trace.WriteLine("OOOPS!");
			}
			return true;
		}

		internal void ValidatePosition()
		{
			if (Math.Abs(m_ObjectGroup[0].LastKnownGoodPosition.XDouble - m_ObjectGroup[1].LastKnownGoodPosition.XDouble - m_LastDeltaX) > 0.001 ||
				Math.Abs(m_ObjectGroup[0].LastKnownGoodPosition.YDouble - m_ObjectGroup[1].LastKnownGoodPosition.YDouble - m_LastDeltaY) > 0.001)
			{
				Trace.WriteLine("OOOPS!");
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

		private int  m_FailedGroupFits = 0;

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
								trackedObject.SetIsTracked(false, NotMeasuredReasons.ObjectCertaintyTooSmall);
							}
							else if (fit.FWHM < STELLAR_OBJECT_MIN_FWHM || fit.FWHM > STELLAR_OBJECT_MAX_FWHM)
							{
								trackedObject.SetIsTracked(false, NotMeasuredReasons.FWHMOutOfRange);
							}
							else if (TangraConfig.Settings.Tracking.CheckElongation && fit.ElongationPercentage > STELLAR_OBJECT_MAX_ELONGATION)
							{
								trackedObject.SetIsTracked(false, NotMeasuredReasons.ObjectTooElongated);
							}
							else
							{
								trackedObject.SetTrackedObjectMatch(fit);
								trackedObject.SetIsTracked(true, NotMeasuredReasons.TrackedSuccessfully);
							}
						}
					}
					else
					{
						string dbg = "";

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
								dbg += "Ni-";
								objectGroup.SetIsTracked(false, NotMeasuredReasons.PSFFittingFailed);

								//Bitmap bmp = new Bitmap(100, 200);
								//using (Graphics g = Graphics.FromImage(bmp))
								//{
								//	doubleFit.DrawInternalPoints(g, new Rectangle(0, 0, 100, 200), 5, 5, Brushes.Lime, Brushes.Yellow, 8);
								//	g.Save();
								//}
								//bmp.Save(@"D:\Hristo\mutual_double_fit.bmp");
								m_FailedGroupFits++;
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
                                        trackedObject.SetIsTracked(true, NotMeasuredReasons.TrackedSuccessfully);
										dbg += "TsGs-";
									}
									else if (fit.FWHM < STELLAR_OBJECT_MIN_FWHM || fit.FWHM > STELLAR_OBJECT_MAX_FWHM)
									{
										trackedObject.SetIsTracked(false, NotMeasuredReasons.FWHMOutOfRange);
										dbg += "Fw-";
									    errors++;
									}
									else if (TangraConfig.Settings.Tracking.CheckElongation && fit.ElongationPercentage > STELLAR_OBJECT_MAX_ELONGATION)
									{
										trackedObject.SetIsTracked(false, NotMeasuredReasons.ObjectTooElongated);
										dbg += "Elo-";
                                        errors++;
									}
									else
									{
										trackedObject.SetIsTracked(true, NotMeasuredReasons.TrackedSuccessfully);
										dbg += "Ts-";
									}
								}

                                if (tooSmallCertainties == 2)
                                {
                                    trackedObjects[0].SetIsTracked(false, NotMeasuredReasons.ObjectCertaintyTooSmall);
                                    trackedObjects[1].SetIsTracked(false, NotMeasuredReasons.ObjectCertaintyTooSmall);
                                    errors++;
	                                m_FailedGroupFits++;
									dbg += "Uncer-";
                                }

                                if (errors == 0)
                                {
									if (objectGroup.CheckIdentifiedObjects(fits[0].XCenter, fits[1].XCenter, fits[0].YCenter, fits[1].YCenter))
									{
										trackedObjects[0].SetTrackedObjectMatch(fits[0]);
										trackedObjects[1].SetTrackedObjectMatch(fits[1]);
										m_FailedGroupFits = 0;
										dbg += "Id-";

										double dist = ImagePixel.ComputeDistance(fits[0].XCenter, fits[1].XCenter, fits[0].YCenter, fits[1].YCenter);
										if (dist < 2)
										{
											Trace.WriteLine("TOO CLOSE");
										}

										if (dist > 16)
										{
											Trace.WriteLine("TOO FAR");
										}
									}
									else
									{
										dbg += "NoId-";
									}
                                }
							}
						}
						else
							dbg += "NoSlv-";

						objectGroup.ValidatePosition();
						Trace.WriteLine(dbg);
					}
				}
			}

			bool atLeastOneGroupLocated = false;

			for (int i = 0; i < m_TrackedObjectGroups.Count; i++)
			{
				TrackedObjectGroup trackedGroup = m_TrackedObjectGroups[i];

				if (!trackedGroup.IsSingleObject && trackedGroup.LastCenterObject1 != null && trackedGroup.LastCenterObject2 != null)
				{
					Trace.WriteLine(string.Format("({0}, {1}, {2}) ({3},{4},{5}) [{6},{7}]",
						trackedGroup.LastCenterObject1.XDouble, trackedGroup.LastCenterObject1.YDouble, trackedGroup.LastCenterObject1.Brightness,
						trackedGroup.LastCenterObject2.XDouble, trackedGroup.LastCenterObject2.YDouble, trackedGroup.LastCenterObject2.Brightness,
						trackedGroup.LastCenterObject1.XDouble - trackedGroup.LastCenterObject2.XDouble, trackedGroup.LastCenterObject1.YDouble - trackedGroup.LastCenterObject2.YDouble));
				}

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
					trackedGroup.SetIsTracked(false, NotMeasuredReasons.FitSuspectAsNoGuidingStarsAreLocated);
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
							trackedGroup.SingleObject.SetIsTracked(true, NotMeasuredReasons.TrackedSuccessfully);
							trackedGroup.SingleObject.SetTrackedObjectMatch(fit);
						}
						else if (m_IsFullDisappearance)
							trackedGroup.SingleObject.SetIsTracked(false, NotMeasuredReasons.FullyDisappearingStarMarkedTrackedWithoutBeingFound);
						else
							trackedGroup.SingleObject.SetIsTracked(false, NotMeasuredReasons.ObjectCertaintyTooSmall);
					}
					else
					{
						string dbg = "";
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
							IImagePixel center1 = null;
							IImagePixel center2 = null;
							TrackedObjectLight trackedObject1;
							TrackedObjectLight trackedObject2;

							bool resortToBrightness = false;
							bool groupIdentified = trackedGroup.IdentifyObjects(fit1, fit2, GUIDING_STAR_MIN_CERTAINTY, out trackedObject1, out trackedObject2);
							if (!groupIdentified && m_IsFullDisappearance)
							{
								dbg += "ReBr::";
								groupIdentified = trackedGroup.IdentifyBrightObject(fit1, fit2, STELLAR_OBJECT_MIN_CERTAINTY, out trackedObject1, out trackedObject2, out center1, out center2);
								resortToBrightness = true;
							}

							if (!groupIdentified)
							{
								trackedGroup.SetIsTracked(false, NotMeasuredReasons.PSFFittingFailed);
								dbg += "PsF::";
							}
							else
							{
								PSFFit[] fits = new PSFFit[] { fit1, fit2 };
								IImagePixel[] centers = new IImagePixel[] { center1, center2 };
								TrackedObjectLight[] trackedObjects = new TrackedObjectLight[] { trackedObject1, trackedObject2 };

								bool objectCheckSuccessful = resortToBrightness
									? trackedGroup.CheckIdentifiedObjects(center1.XDouble, center2.XDouble, center1.YDouble, center2.YDouble)
									: trackedGroup.CheckIdentifiedObjects(fits[0].XCenter, fits[1].XCenter, fits[0].YCenter, fits[1].YCenter);

								if (objectCheckSuccessful)
								{
									dbg += "ChS::";
									bool atLeastOneOK = (fit1.IsSolved && fit1.Certainty > GUIDING_STAR_MIN_CERTAINTY) || (fit2.IsSolved && fit2.Certainty > GUIDING_STAR_MIN_CERTAINTY);
									double dist = ImagePixel.ComputeDistance(fit1.XCenter, fit2.XCenter, fit1.YCenter, fit2.YCenter);

									if (!atLeastOneOK || ((dist < 2 || dist > 16) && !resortToBrightness))
									{
										trackedGroup.SetIsTracked(false, NotMeasuredReasons.PSFFittingFailed);
									}

									int cntOk = 0;
									for (int j = 0; j < 2; j++)
									{
										PSFFit fit = fits[j];
										IImagePixel center = centers[j];
										TrackedObjectLight trackedObject = trackedObjects[j];

										if (fit.IsSolved && fit.Certainty > STELLAR_OBJECT_MIN_CERTAINTY)
										{
											if (resortToBrightness && trackedObject.IsOccultedStar)
											{
												trackedObject.SetIsTracked(true, NotMeasuredReasons.FullyDisappearingStarMarkedTrackedWithoutBeingFound, center);
												dbg += "OccDi::";
											}
											else
											{
												trackedObject.SetTrackedObjectMatch(fit);
												trackedObject.SetIsTracked(true, NotMeasuredReasons.TrackedSuccessfully);
												dbg += "TrSuc::";
											}

											cntOk++;
										}
										else if (m_IsFullDisappearance && trackedObject.IsOccultedStar)
										{
											trackedObject.SetIsTracked(false, NotMeasuredReasons.FullyDisappearingStarMarkedTrackedWithoutBeingFound, resortToBrightness ? center : null);
											dbg += "BadCerSuc::";
										}
										else
										{
											trackedObject.SetIsTracked(false, NotMeasuredReasons.ObjectCertaintyTooSmall, resortToBrightness ? center : null);
											dbg += "BadCerNOSuc::";
										}
									}

									if (cntOk == 2)
									{
										m_FailedGroupFits = 0;
									}									
								}
								else
								{
									trackedObjects[0].SetIsTracked(false, NotMeasuredReasons.FailedToLocateAfterDistanceCheck);
									trackedObjects[1].SetIsTracked(false, NotMeasuredReasons.FailedToLocateAfterDistanceCheck);
									m_FailedGroupFits++;
									dbg += "ChU::";
								}
							}
						}
						else
						{
							m_FailedGroupFits++;
							dbg += "NoFi::";
						}

						trackedGroup.ValidatePosition();
						Trace.WriteLine(dbg);
					}
				}
			}

			IsTrackedSuccessfully = atLeastOneGroupLocated;

			if (IsTrackedSuccessfully)
				RefinedAverageFWHM = m_TrackedObjects.Cast<TrackedObjectLight>().Average(x => x.RefinedFWHM);

			if (m_FailedGroupFits > 10)
			{
				Trace.WriteLine("SH*T HAPPENED");
			}
		}
	}
}

