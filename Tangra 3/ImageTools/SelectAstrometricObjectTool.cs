/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Astrometry;
using Tangra.Astrometry.Recognition;
using Tangra.AstroServices;
using Tangra.Controller;
using Tangra.Model.Astro;
using Tangra.Model.Context;
using Tangra.Model.Helpers;
using Tangra.Model.Image;
using Tangra.Model.ImageTools;
using Tangra.Model.VideoOperations;
using Tangra.VideoOperations.Astrometry;
using Tangra.VideoOperations.Astrometry.Engine;

namespace Tangra.ImageTools
{
	internal class SelectedObject
	{
		public double RADeg;
		public double DEDeg;
		public double X0;
		public double Y0;
		public PlateConstStarPair FittedStar;
		public LeastSquareFittedAstrometry Solution;
		public ImagePixel Pixel;
		public PSFFit Gaussian;
		public IIdentifiedObject IdentifiedObject;
	}

	internal class SelectAstrometricObjectTool : ImageTool
	{

		public enum SelectObjectState
		{
			NoObject = 0,
			ObjectLocked,
			ObjectSelected
		}

		protected SelectObjectState m_State;
		protected PlateConstStarPair m_Object;

		private AstrometricState m_AstrometricState;
		private AstrometryController m_AstrometryController;
		private VideoController m_VideoController;

		private SelectedObject m_SelectedObject;

		public SelectAstrometricObjectTool(AstrometryController astrometryController, VideoController videoController)
		{
			m_AstrometryController = astrometryController;
			m_VideoController = videoController;
		}

		public override void Activate()
		{
			m_VideoController.SetPictureBoxCursor(Cursors.Arrow);

			m_AstrometricState = AstrometricState.EnsureAstrometricState();

			TangraContext.Current.OSDExcludeToolDisabled = true;
			m_VideoController.UpdateViews();
		}

		public override void Deactivate()
		{ }

		public override void MouseLeave()
		{ }

		public override void MouseMove(Point location)
		{
			IStarMap map = AstrometryContext.Current.StarMap;
			if (map == null) return;

			bool nearbyStarFound = false;

			AstrometricState state = AstrometryContext.Current.AstrometricState;
			if (state != null)
			{
				if (state.AstrometricFit != null)
				{
					for (int radius = 1; radius < 8; radius++)
					{
						ImagePixel centroid = map.GetCentroid(location.X, location.Y, radius);
						if (centroid == null) continue;


						foreach (PlateConstStarPair star in state.AstrometricFit.FitInfo.AllStarPairs)
						{
							if (Math.Abs(star.x - centroid.XDouble) < radius &&
								Math.Abs(star.y - centroid.YDouble) < radius)
							{
								m_Object = star;

								nearbyStarFound = true;
								break;
							}
						}

						if (nearbyStarFound) break;
					}

					if (!nearbyStarFound)
					{
						m_State = SelectObjectState.NoObject;
					}
					else
						m_State = SelectObjectState.ObjectLocked;


					if (m_AstrometricState.MeasuringState == AstrometryInFramesState.Ready)
					{
						double ra, de;
						state.AstrometricFit.GetRADEFromImageCoords(location.X, location.Y, out ra, out de);

						string moreInfo = string.Format("RA={0} DE={1}", AstroConvert.ToStringValue(ra / 15, "HHhMMmSS.Ts"), AstroConvert.ToStringValue(de, "+DD°MM'SS\""));
						m_VideoController.DisplayCursorPositionDetails(location, moreInfo);
					}
				}
				else
				{
					StarMapFeature nearbyFeature = map.GetFeatureInRadius(location.X, location.Y, 8);
					nearbyStarFound = nearbyFeature != null && nearbyFeature.PixelCount > 4;
				}

				m_VideoController.SetPictureBoxCursor(nearbyStarFound ? Cursors.Hand : (state.ManualStarIdentificationMode ? Cursors.Cross : Cursors.Default));
			}
		}

		public override void MouseClick(ObjectClickEventArgs e)
		{
			if (e.MouseEventArgs.Button == MouseButtons.Right && m_AstrometricState.ManualStarIdentificationMode)
			{
				m_AstrometryController.SetManuallyIdentifyStarState(false);
			}

			if (e.Pixel != null)
			{
				if ((m_AstrometricState.MeasuringState == AstrometryInFramesState.Ready || m_AstrometricState.MeasuringState == AstrometryInFramesState.FitFailed)
					&& m_AstrometricState.ManualStarIdentificationMode 
					&& AstrometryContext.Current.FieldSolveContext.CatalogueStars != null 
					&& AstrometryContext.Current.FieldSolveContext.CatalogueStars.Count > 3)
				{
					
					var frmIdentifyCalibrationStar = new frmIdentifyCalibrationStar(AstrometryContext.Current.FieldSolveContext.CatalogueStars, m_AstrometricState.ManuallyIdentifiedStars, false);
					DialogResult res = m_VideoController.ShowDialog(frmIdentifyCalibrationStar);
					if (res == DialogResult.Abort)
					{
						m_AstrometricState.ManuallyIdentifiedStars.Clear();
						return;
					}
					else if (res == DialogResult.OK && frmIdentifyCalibrationStar.SelectedStar != null)
					{
						m_AstrometricState.ManuallyIdentifiedStars.Add(e.Gausian, frmIdentifyCalibrationStar.SelectedStar);
						m_AstrometryController.TriggerPlateReSolve();
					}
					m_AstrometryController.SetManuallyIdentifyStarState(false);
				}
				else if (m_AstrometricState.MeasuringState != AstrometryInFramesState.RunningMeasurements)
				{
					AstrometricState astrometryTracker = AstrometryContext.Current.AstrometricState;
					if (astrometryTracker != null &&
						astrometryTracker.AstrometricFit != null)
					{
						var objInfo = new SelectedObject() { X0 = e.Pixel.XDouble, Y0 = e.Pixel.YDouble };
						astrometryTracker.AstrometricFit.GetRADEFromImageCoords(e.Pixel.XDouble, e.Pixel.YDouble, out objInfo.RADeg, out objInfo.DEDeg);
						objInfo.FittedStar = astrometryTracker.AstrometricFit.FitInfo.GetFittedStar(e.Pixel);
						objInfo.Solution = astrometryTracker.AstrometricFit;
						objInfo.Pixel = e.Pixel;
						objInfo.Gaussian = e.Gausian;
						if (m_AstrometricState.IdentifiedObjects != null)
							objInfo.IdentifiedObject = m_AstrometricState.GetIdentifiedObjectAt(objInfo.RADeg, objInfo.DEDeg);
						else
							objInfo.IdentifiedObject = null;


						// We don't want to reload the current frame as this will result in trying another Astrometric Fit
						// So we send a message to the Astrometry component about the newly selected object
						m_AstrometryController.NewObjectSelected(objInfo);

						m_SelectedObject = objInfo;
						m_VideoController.RedrawCurrentFrame(false, true);
					}
				}
			}
		}

		public override void MouseDown(Point location)
		{ }

		public override void MouseUp(Point location)
		{ }


		public override void OnNewFrame(int currentFrameIndex, bool isLastFrame)
		{
			m_SelectedObject = null;
		}

		public override void PostDraw(Graphics g)
		{
			base.PostDraw(g);

			if (m_SelectedObject != null)
			{
				float x0 = m_SelectedObject.Pixel.X;
				float y0 = m_SelectedObject.Pixel.Y;

				g.DrawEllipse(Pens.Yellow, x0 - 7, y0 - 7, 15, 15);
				g.DrawLine(Pens.Yellow, x0 - 13, y0, x0 - 4, y0);
				g.DrawLine(Pens.Yellow, x0 + 13, y0, x0 + 4, y0);
				g.DrawLine(Pens.Yellow, x0, y0 - 13, x0, y0 - 4);
				g.DrawLine(Pens.Yellow, x0, y0 + 13, x0, y0 + 4);
			}

            if (m_AstrometricState.ManualStarIdentificationMode && m_AstrometricState.ManuallyIdentifiedStars != null)
		    {
		        foreach (var psf in m_AstrometricState.ManuallyIdentifiedStars.Keys)
		        {
                    g.DrawEllipse(Pens.DarkRed, (float)psf.XCenter - 7, (float)psf.YCenter - 7, 2 * 7, 2 * 7);
                    g.DrawEllipse(Pens.DarkRed, (float)psf.XCenter - 7 - 2, (float)psf.YCenter - 7 - 2, 2 * 7 + 4, 2 * 7 + 4);	            
		        }
		    }
		}
	}
}
