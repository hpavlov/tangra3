/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Astrometry;
using Tangra.Astrometry.Recognition;
using Tangra.Config;
using Tangra.Controller;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Model.Context;
using Tangra.Model.Helpers;
using Tangra.Model.Image;
using Tangra.Model.ImageTools;
using Tangra.Model.VideoOperations;
using Tangra.Resources;
using Tangra.StarCatalogues;
using Tangra.VideoOperations.Astrometry;
using Tangra.VideoOperations.Astrometry.Engine;

namespace Tangra.ImageTools
{
	internal abstract class CalibrationToolBase : ImageTool, IPlateCalibrationTool
	{
		protected int m_xOldCursor = -1;
		protected int m_yOldCursor = -1;

		protected double m_Eta = 0;
		protected double m_FLength = 660;
		protected double m_Aspect = 1.0;
		protected double m_LimitMag = 10;
		protected bool m_ShowLabels = false;
		protected bool m_ShowMagnitudes = false;
		protected bool m_Grid = false;

	    protected int m_Tolerance = 2;

		protected FittingsState m_State = FittingsState.Configuring;

		protected Pen catalogStarPen;
		protected Pen referenceStarPen;
		protected Pen rejectedStarPen;
		protected Pen unrecognizedStarPen;
		protected Brush catalogBrushReference;
		protected Brush catalogBrushRejected;
		protected Brush catalogBrushUnrecognized;

		protected static Font m_StarInfoFont = new Font(FontFamily.GenericSerif, 8);

		protected ucCalibrationPanel m_PlatesolveController;

		protected IAstrometryController m_AstrometryController;
		protected VideoController m_VideoController;

		protected AstroPlate m_Image;

		protected PlateCalibration m_PlateCalibrator;
		protected CalibrationContext m_CalibrationContext;
		protected List<IStar> m_CatalogueStars;
		protected IAstrometricFit m_SolvedPlate;

		protected CalibrationToolBase(AstrometryController astrometryController, VideoController videoController)
		{
			m_AstrometryController = astrometryController;
			m_VideoController = videoController;
		}

		public void LoadControler(Panel panel)
		{
			panel.Controls.Clear();
			m_PlatesolveController.UpdateState((int)FittingsState.Configuring);

			panel.Controls.Add(m_PlatesolveController);
		}


		public override void Activate()
		{
			if (catalogStarPen != null) catalogStarPen.Dispose();
			if (referenceStarPen != null) referenceStarPen.Dispose();
			if (rejectedStarPen != null) rejectedStarPen.Dispose();
			if (unrecognizedStarPen != null) unrecognizedStarPen.Dispose();
			catalogStarPen = new Pen(TangraConfig.Settings.Astrometry.Colors.CatalogueStar);
			referenceStarPen = new Pen(TangraConfig.Settings.Astrometry.Colors.ReferenceStar);
			rejectedStarPen = new Pen(TangraConfig.Settings.Astrometry.Colors.RejectedReferenceStar);
			unrecognizedStarPen = new Pen(TangraConfig.Settings.Astrometry.Colors.UndetectedReferenceStar);
			if (catalogBrushReference != null) catalogBrushReference.Dispose();
			catalogBrushReference = new SolidBrush(TangraConfig.Settings.Astrometry.Colors.ReferenceStar);
			if (catalogBrushRejected != null) catalogBrushRejected.Dispose();
			catalogBrushRejected = new SolidBrush(TangraConfig.Settings.Astrometry.Colors.RejectedReferenceStar);
			if (catalogBrushUnrecognized != null) catalogBrushUnrecognized.Dispose();
			catalogBrushUnrecognized = new SolidBrush(TangraConfig.Settings.Astrometry.Colors.UndetectedReferenceStar);

			m_Panning = false;
			m_Is3StarIdMode = true;
		}

		public void DrawStarPairs(double limitMag)
		{
			UpdateToolControlDisplay();
		}

		public override void PostDraw(Graphics g)
		{
			if (m_CatalogueStars == null)
			{
				if (AstrometryContext.Current.StarMap != null)
				{
					foreach (StarMapFeature feature in AstrometryContext.Current.StarMap.Features)
					{
						ImagePixel center = feature.GetCenter();

						g.DrawEllipse(Pens.Red, center.X - 2, center.Y - 2, 5, 5);
					}
				}
			}
			else if (m_CatalogueStars != null && m_State == FittingsState.Configuring)
			{
				if (m_SolvedPlate != null && m_PlateCalibrator.ConstantsSolver != null)
				{
					foreach (IStar star in m_CatalogueStars)
					{
						if (star.Mag > m_LimitMag) continue;

						double x, y;
						m_SolvedPlate.GetImageCoordsFromRADE(star.RADeg, star.DEDeg, out x, out y);

						PlateConstStarPair pair = m_PlateCalibrator.ConstantsSolver.Pairs.FirstOrDefault((p) => p.StarNo == star.StarNo);

						Pen starPen = catalogStarPen;
						if (pair == null)
							starPen = rejectedStarPen;
						else if (!pair.FitInfo.ExcludedForHighResidual)
						{
							starPen = referenceStarPen;

							if (AstrometryContext.Current.StarMap != null)
							{
								StarMapFeature feature =
									AstrometryContext.Current.StarMap.Features.FirstOrDefault((f) => f.FeatureId == pair.FeatureId);
								if (feature != null)
									g.DrawLine(Pens.Beige, (float) x, (float) y, (float) feature.GetCenter().XDouble,
									           (float) feature.GetCenter().YDouble);
							}
						}

						g.DrawLine(starPen, (float) x - 3, (float) y, (float) x + 3, (float) y);
						g.DrawLine(starPen, (float) x, (float) y - 3, (float) x, (float) y + 3);
					}
				}
			}
			else
			{
				DrawCatalogStarsFit(g);
			}

			g.Save();
		}

		protected double GetStarDiameter(double mLimit, double rLimit, double m)
		{
			if (m >= mLimit - 1) return rLimit;
			if (m >= mLimit - 3) return rLimit + (mLimit - m) * 1.5;
			if (m >= mLimit - 7) return rLimit + 3 + (mLimit - 3 - m);

			return rLimit + 9 + (mLimit - 7 - m) * 0.5;
		}

		protected LeastSquareFittedAstrometry FittedAstrometryFromUserSelectedFitGrade()
		{
			LeastSquareFittedAstrometry astrometry = null;
			switch (m_FitGrade)
			{
				case 0:					
					if (m_CalibrationContext.InitialSecondAstrometricFit != null)
						astrometry = m_CalibrationContext.InitialSecondAstrometricFit;
					else if (m_CalibrationContext.InitialFirstAstrometricFit != null)
						astrometry = m_CalibrationContext.InitialFirstAstrometricFit;
					break;
				case 1:
					astrometry = m_CalibrationContext.FirstAstrometricFit;
					break;
				case 2:
					astrometry = m_CalibrationContext.SecondAstrometricFit;
					break;
				case 3:
					astrometry = m_CalibrationContext.DistanceBasedFit;
					break;
				case 4:
					astrometry = m_CalibrationContext.ImprovedDistanceBasedFit;
					break;

				default:
					astrometry = null;
					break;
			}

			return astrometry;
		}


		protected bool m_Is3StarIdMode = true;


		public void DrawCatalogStarsFit()
		{
			m_VideoController.RedrawCurrentFrame(false, true);
		}

		public void DrawCatalogStarsFit(Graphics g)
		{
			if (m_CatalogueStars == null)
			{
				m_LimitMag = -100;
				return;
			}

			bool hasManualStars = 
				m_Is3StarIdMode &&
				m_UserStarIdentification != null &&
				m_UserStarIdentification.Count > 0;

			LeastSquareFittedAstrometry astrometry = null;

			astrometry = FittedAstrometryFromUserSelectedFitGrade();
			IAstrometricFit fit = null;
			if (astrometry != null)
				fit = astrometry;
			else
				fit = m_SolvedPlate;

            if (fit != null)
            {
				double limitMag = (astrometry != null && astrometry.FitInfo.AllStarPairs.Count > 0)
					? astrometry.FitInfo.AllStarPairs.Max(p => p.Mag)
					: m_LimitMag;

                foreach (IStar star in m_CatalogueStars)
                {
					if (star.Mag > limitMag) continue;

                    double x, y;
                    fit.GetImageCoordsFromRADE(star.RADeg, star.DEDeg, out x, out y);

                    Pen starPen = catalogStarPen;
                    Brush labelBruish = catalogBrushUnrecognized;

                    if (astrometry != null)
                    {
                        PlateConstStarPair pair = astrometry.FitInfo.AllStarPairs.Find((p) => p.StarNo == star.StarNo);

                        if (pair != null && pair.FitInfo.UsedInSolution)
                        {
                            starPen = referenceStarPen;
                            labelBruish = catalogBrushReference;
                        }
                        else if (pair != null && pair.FitInfo.ExcludedForHighResidual)
                        {
                            starPen = rejectedStarPen;
                            labelBruish = catalogBrushRejected;
                        }
                        else
                        {
                            starPen = unrecognizedStarPen;
                            labelBruish = catalogBrushUnrecognized;
                        }

                        if (pair != null)
                            g.DrawLine(starPen, (float)x, (float)y, (float)pair.x, (float)pair.y);
                    }

                    if (!m_Is3StarIdMode || astrometry != null)
                    {
                        float rad = (float)GetStarDiameter(m_LimitMag, 5, star.Mag) / 2;
                        g.DrawEllipse(starPen, (float)x - rad, (float)y - rad, 2 * rad, 2 * rad);
                    }

                    if (m_ShowLabels || m_ShowMagnitudes)
                    {
                        string label;
                        if (m_ShowLabels && m_ShowMagnitudes)
                            label = string.Format("{0} ({1}m)", star.GetStarDesignation(0), star.Mag);
                        else if (m_ShowLabels)
                            label = string.Format("{0}", star.GetStarDesignation(0));
                        else
                            label = string.Format("{0}m", star.Mag);

                        g.DrawString(label, m_StarInfoFont, labelBruish, (float)x + 10, (float)y + 10);
                    }
                }

                if (m_Is3StarIdMode && astrometry == null)
				{
					// Draw all features from the starMap (unless the configuration has been solved)
					if (AstrometryContext.Current.StarMap != null)
					{
						foreach (StarMapFeature feature in AstrometryContext.Current.StarMap.Features)
						{
							ImagePixel center = feature.GetCenter();

							PSFFit psfFit;
							AstrometryContext.Current.StarMap.GetPSFFit(center.X, center.Y, PSFFittingMethod.NonLinearAsymetricFit, out psfFit);

#if ASTROMETRY_DEBUG
							PSFFit psfFit2;
							AstrometryContext.Current.StarMap.GetPSFFit(center.X, center.Y, PSFFittingMethod.NonLinearFit, out psfFit2);
							double elong = psfFit.RX0 / psfFit.RY0;
							double elongPerc = Math.Abs(1 - elong) * 100; 
							Trace.WriteLine(string.Format("({0:0}, {1:0}) Rx = {2:0.00} Ry = {3:0.00}, e = {4:0.000} ({5:0}%), FWHMxy = {6:0.0} | FWHMxx = {7:0.0}",
								center.X, center.Y, psfFit.RX0, psfFit.RY0, elong, elongPerc,
								psfFit.FWHM, psfFit2.FWHM));
#endif
							Pen pen = catalogStarPen;

#if ASTROMETRY_DEBUG
							if (psfFit.FWHM < TangraConfig.Settings.Astrometry.MinReferenceStarFWHM ||
								psfFit.FWHM > TangraConfig.Settings.Astrometry.MaxReferenceStarFWHM ||
								psfFit.ElongationPercentage > TangraConfig.Settings.Astrometry.MaximumPSFElongation)
							{
								pen = rejectedStarPen;
                            }
#endif
                            
                            g.DrawLine(pen, (float)center.XDouble - 9, (float)center.YDouble, (float)center.XDouble - 5, (float)center.YDouble);
							g.DrawLine(pen, (float)center.XDouble + 5, (float)center.YDouble, (float)center.XDouble + 9, (float)center.YDouble);
							g.DrawLine(pen, (float)center.XDouble, (float)center.YDouble - 9, (float)center.XDouble, (float)center.YDouble - 5);
							g.DrawLine(pen, (float)center.XDouble, (float)center.YDouble + 5, (float)center.XDouble, (float)center.YDouble + 9);
						}
					}
				}
				else
				{
					if (m_Grid)
						DrawEquatorialGrid(g);
				}

				#region Draw the manual single star identification
				if (hasManualStars)
				{
					foreach(PSFFit psf in m_UserStarIdentification.Keys)
					{
						IStar star = m_UserStarIdentification[psf];

						float rad = (float)GetStarDiameter(m_LimitMag, 5, star.Mag) / 2;

						g.DrawEllipse(Pens.DarkRed, (float)psf.XCenter - rad, (float)psf.YCenter - rad, 2 * rad, 2 * rad);
						g.DrawEllipse(Pens.DarkRed, (float)psf.XCenter - rad - 2, (float)psf.YCenter - rad - 2, 2 * rad + 4, 2 * rad + 4);

					}
				}
				#endregion;
			}

			UpdateToolControlDisplay();
		}

		protected virtual void DrawEquatorialGrid(Graphics g)
		{
			// We want at least 5 but no more than 10 RA and DE lines to fit the diagonal
			double diagonalInArcSec = m_SolvedPlate.GetDistanceInArcSec(0, 0, m_Image.ImageWidth, m_Image.ImageHeight);

			double ra0, de0;
			m_SolvedPlate.GetRADEFromImageCoords(m_Image.CenterXImage, m_Image.CenterYImage, out ra0, out de0);
			Debug.Assert(Math.Abs(m_SolvedPlate.RA0Deg - ra0) < 1 / 3600.0);
			Debug.Assert(Math.Abs(m_SolvedPlate.DE0Deg - de0) < 1 / 3600.0);

			// We are not drawing lines when too close to the poles
			if (de0 + 2 * diagonalInArcSec / 3600.0 > 90) return;
			if (de0 - 2 * diagonalInArcSec / 3600.0 < -90) return;

			int MAX_GRID_LINES = 10;
			int MIN_GRID_LINES = 5;

			int[] intervals = new int[] { 1, 5, 10, 30, 60, 60 * 5, 60 * 10, 60 * 30, 60 * 60 };

			double deFrom, deTo;
			#region Calculating  the interval in declination
			double maxArcSec = diagonalInArcSec / MIN_GRID_LINES;
			double minArcSec = diagonalInArcSec / MAX_GRID_LINES;

			int deInterval = -1;
			for (int i = 0; i < intervals.Length; i++)
			{
				if (intervals[i] >= minArcSec &&
					intervals[i] <= maxArcSec)
				{
					deInterval = intervals[i];
					break;
				}
				else if (intervals[i] < minArcSec)
					deInterval = intervals[i];
			}
			#endregion

			double raFrom, raTo;
			#region Calculating  the interval in right accension

			double[] raArr = new double[4];
			double[] deArr = new double[4];
			m_SolvedPlate.GetRADEFromImageCoords(0, 0, out raArr[0], out deArr[0]);
			m_SolvedPlate.GetRADEFromImageCoords(0, m_Image.ImageHeight, out raArr[1], out deArr[1]);
			m_SolvedPlate.GetRADEFromImageCoords(m_Image.ImageWidth, 0, out raArr[2], out deArr[2]);
			m_SolvedPlate.GetRADEFromImageCoords(m_Image.ImageWidth, m_Image.ImageHeight, out raArr[3], out deArr[3]);

			// From the RA of the 4 corners we need to find the fromRA and toRA
			double raMin = double.MaxValue, raMax = double.MinValue;
			int minIdx = 0, maxIdx = 0;
			for (int i = 0; i < 4; i++)
			{
				if (raArr[i] > raMax)
				{
					raMax = raArr[i];
					maxIdx = i;
				}

				if (raArr[i] < raMin)
				{
					raMin = raArr[i];
					minIdx = i;
				}
			}
			double shortestDistance = AngleUtility.Elongation(raArr[minIdx], 0, raArr[maxIdx], 0);
			if (shortestDistance < (raArr[maxIdx] - raArr[minIdx]) - 10 / 3600.0 /* tolerance 10" */)
			{
				// We are going accross the 0 point
				Debug.Assert(false, "This is not implemented yet");
				return;
				//raFrom = 0;
				//raTo = 0;
				//for (int i = 0; i < 4; i++)
				//{
				//    if (i == minIdx) continue;
				//    if (i == maxIdx) continue;

				//    if (raArr[0] > raMax)
				//    {
				//        raMax = raArr[0];
				//        maxIdx = 1;
				//    }

				//    if (raArr[0] < raMin)
				//    {
				//        raMin = raArr[0];
				//        minIdx = i;
				//    }
				//}
				//raTo = raArr[maxIdx];
			}
			else
			{
				raFrom = raArr[minIdx];
				raTo = raArr[maxIdx];
			}

			minArcSec = 3600.0 * (raTo - raFrom) / (15 * MAX_GRID_LINES);
			maxArcSec = 3600.0 * (raTo - raFrom) / (15 * MIN_GRID_LINES);
			int raInterval = -1;
			for (int i = 0; i < intervals.Length; i++)
			{
				if (intervals[i] >= minArcSec &&
					intervals[i] <= maxArcSec)
				{
					raInterval = intervals[i];
					break;
				}
				else if (intervals[i] < minArcSec)
					raInterval = intervals[i];
			}
			#endregion

#if DEBUG
            if (raInterval == -1 || deInterval == -1)
            {
                Debug.Assert(false, "Problem with raInterval or deInterval");
                return;
            }
#endif

			raFrom = (Math.Floor((raFrom * 3600.0) / (15 * raInterval)) - 1) * 15 * raInterval / 3600.0;
			raTo = (Math.Ceiling((raTo * 3600.0) / (15 * raInterval)) + 1) * 15 * raInterval / 3600.0;

			deFrom = (Math.Floor((de0 * 3600.0 - (diagonalInArcSec / 2)) / (deInterval)) - 1) * deInterval / 3600.0;
			deTo = (Math.Ceiling((de0 * 3600.0 + (diagonalInArcSec / 2)) / (deInterval)) + 1) * deInterval / 3600.0;

			//Debug.WriteLine(string.Format("Draw Grid -> C: ({0}, {1}) F:({2}, {3}) T:({4}, {5})", 
			//    AstroConvert.ToStringValue(ra0, "HH MM SS.T"), AstroConvert.ToStringValue(de0, "+DD MM SS.T"),
			//    AstroConvert.ToStringValue(raFrom, "HH MM SS.T"), AstroConvert.ToStringValue(raTo, "HH MM SS.T"),
			//    AstroConvert.ToStringValue(deFrom, "+DD MM SS.T"), AstroConvert.ToStringValue(deTo, "+DD MM SS.T")));

			double x1, y1, x2, y2;
			double stepRA = (15 * raInterval) / 3600.0;
			double stepDE = deInterval / 3600.0;

			for (double ra = raFrom; ra <= raTo; ra += stepRA)
			{
				x1 = double.NaN; y1 = double.NaN;
				for (double de = deFrom; de < deTo; de += stepDE / 10.0)
				{
					m_SolvedPlate.GetImageCoordsFromRADE(ra, de, out x2, out y2);
					if (x2 >= 0 && x2 <= m_Image.ImageWidth && y2 >= 0 && y2 <= m_Image.ImageHeight)
					{
						if (!double.IsNaN(x1))
							g.DrawLine(Pens.Purple, (float)x1, (float)y1, (float)x2, (float)y2);

						x1 = x2; y1 = y2;
					}
				}
			}

			for (double de = deFrom; de <= deTo; de += stepDE)
			{
				x1 = double.NaN; y1 = double.NaN;
				for (double ra = raFrom; ra < raTo; ra += stepRA / 10.0)
				{
					m_SolvedPlate.GetImageCoordsFromRADE(ra, de, out x2, out y2);
					if (x2 >= 0 && x2 <= m_Image.ImageWidth && y2 >= 0 && y2 <= m_Image.ImageHeight)
					{
						if (!double.IsNaN(x1))
							g.DrawLine(Pens.Purple, (float)x1, (float)y1, (float)x2, (float)y2);

						x1 = x2; y1 = y2;
					}
				}
			}
		}


		private Point m_StarPoint = Point.Empty;
		private bool m_Panning = false;
		private PSFFit m_SelectedCalibrationStar;

		protected double m_RADegCenter;
		protected double m_DEDegCenter;

		protected Dictionary<PSFFit, IStar> m_UserStarIdentification = new Dictionary<PSFFit, IStar>();

		public override void MouseDown(Point location)
		{
			if (m_SelectedCalibrationStar != null)
			{
				frmIdentifyCalibrationStar frmIdentifyCalibrationStar = new frmIdentifyCalibrationStar(m_CatalogueStars, m_UserStarIdentification);
				DialogResult res = m_VideoController.ShowDialog(frmIdentifyCalibrationStar);
				if (res == DialogResult.Abort)
				{
					m_UserStarIdentification.Clear();
				}
				else if (res == DialogResult.OK &&
					frmIdentifyCalibrationStar.SelectedStar != null)
				{
					m_UserStarIdentification.Add(m_SelectedCalibrationStar, frmIdentifyCalibrationStar.SelectedStar);
					if (m_UserStarIdentification.Keys.Count > 0 && 
						m_UserStarIdentification.Keys.Count < 3)
					{
						m_VideoController.ShowMessageBox(
							string.Format("Identify another {0} star(s) to attempt calibration", 3 - m_UserStarIdentification.Keys.Count),
							"Tangra", MessageBoxButtons.OK, MessageBoxIcon.Information);
					}

					if (m_UserStarIdentification.Keys.Count > 2)
					{
					    List<PSFFit> keysList = m_UserStarIdentification.Keys.ToList();
                        IStar star1 = m_UserStarIdentification[keysList[0]];
                        IStar star2 = m_UserStarIdentification[keysList[1]];
                        IStar star3 = m_UserStarIdentification[keysList[2]];
                        double ArcSec1 = 1 / 3600.0;
					    bool badRA = false;
                        bool badDE = false;
                        if (Math.Abs(star1.RADeg - star2.RADeg) < ArcSec1 || Math.Abs(star1.RADeg - star3.RADeg) < ArcSec1 || Math.Abs(star2.RADeg - star3.RADeg) < ArcSec1)
                        {
                            badRA = true;
                        }

                        if (Math.Abs(star1.DEDeg - star2.DEDeg) < ArcSec1 || Math.Abs(star1.DEDeg - star3.DEDeg) < ArcSec1 || Math.Abs(star2.DEDeg - star3.DEDeg) < ArcSec1)
                        {
                            badDE = true;
                        }

                        if (badRA)
                        {
							m_VideoController.ShowMessageBox(
                                "Two of the stars have almost identical Right Ascension. Please try again with different stars.",
                                "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);

                            m_UserStarIdentification.Clear(); 
                        }
                        else if (badDE)
                        {
							m_VideoController.ShowMessageBox(
                                "Two of the stars have almost identical Declination. Please try again with different stars.",
                                "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);

                            m_UserStarIdentification.Clear(); 
                        }
                        else
                        {
                            ThreeStarAstrometry threeStarSolution = ThreeStarAstrometry.SolveByThreeStars(m_Image, m_UserStarIdentification, m_Tolerance);
                            if (threeStarSolution != null)
                            {
                                m_SolvedPlate = threeStarSolution;

                                m_PlatesolveController.UpdateFocalLength((int)Math.Round(threeStarSolution.Image.EffectiveFocalLength));
                                m_RADegCenter = threeStarSolution.RA0Deg;
                                m_DEDegCenter = threeStarSolution.DE0Deg;
                                m_Image.EffectiveFocalLength = threeStarSolution.Image.EffectiveFocalLength;

                                m_UserStarIdentification.Clear();

                                m_AstrometryController.RunCalibrationWithCurrentPreliminaryFit();
                            }
                            else
                            {
								m_VideoController.ShowMessageBox(
                                    "Cannot complete calibration. Please try again with higher initial fit tolerance and/or with different stars. Be sure that the stars are well separated.",
                                    "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);

                                m_UserStarIdentification.Remove(m_UserStarIdentification.Keys.ToList()[2]);
                            }                            
                        }
					}
				}

				DrawCatalogStarsFit();
			}	
			else
			{
				m_StarPoint = new Point(location.X, location.Y);
				m_Panning = true;
				m_VideoController.SetPictureBoxCursor(CustomCursors.PanEnabledCursor);
			}
		}

		public override void MouseUp(Point location)
		{
			if (m_Panning &&
				m_StarPoint != Point.Empty)
			{
				double raDeg1, deDeg1, raDeg2, deDeg2;
				if (m_SolvedPlate != null)
				{
					m_SolvedPlate.GetRADEFromImageCoords(location.X, location.Y, out raDeg1, out deDeg1);
					m_SolvedPlate.GetRADEFromImageCoords(m_StarPoint.X, m_StarPoint.Y, out raDeg2, out deDeg2);

					m_RADegCenter += (raDeg2 - raDeg1);
					m_DEDegCenter += (deDeg2 - deDeg1);
				}

				m_Panning = false;
				m_StarPoint = Point.Empty;
				m_VideoController.SetPictureBoxCursor(m_Is3StarIdMode ? Cursors.Arrow : CustomCursors.PanCursor);
			}
		}

		public override void MouseMove(Point location)
		{
			bool dirty = false;

			int posX = location.X < 16
						   ? 16
						   : (location.X > TangraContext.Current.FrameWidth - 17
								  ? TangraContext.Current.FrameWidth - 17
								  : location.X);
			int posY = location.Y < 16
						   ? 16
						   : (location.Y > TangraContext.Current.FrameHeight - 17
								  ? TangraContext.Current.FrameHeight - 17
								  : location.Y);

			m_VideoController.DisplayCursorPositionDetails(location);

			m_SelectedCalibrationStar = null;

			if (!m_Panning)
			{
				if (m_Is3StarIdMode)
				{
					StarMapFeature closestFeature = AstrometryContext.Current.StarMap.GetFeatureInRadius(posX, posY, 2);
					if (closestFeature != null)
					{
						if (m_Is3StarIdMode)
						{
							m_VideoController.SetPictureBoxCursor(Cursors.Hand);
							PSFFit psfFit;
							AstrometryContext.Current.StarMap.GetPSFFit(posX, posY, 13, out psfFit);
							m_SelectedCalibrationStar = psfFit;
						}
					}
					else
						m_VideoController.SetPictureBoxCursor(Cursors.Arrow);
				}
				else
					m_VideoController.SetPictureBoxCursor(CustomCursors.PanCursor);
			}
			else if (m_Panning &&
				m_StarPoint != Point.Empty)
			{
				ResetPreviousStar();

                if (m_SolvedPlate != null)
                {
                    double raDeg1, deDeg1, raDeg2, deDeg2;
                    m_SolvedPlate.GetRADEFromImageCoords(location.X, location.Y, out raDeg1, out deDeg1);
                    m_SolvedPlate.GetRADEFromImageCoords(m_StarPoint.X, m_StarPoint.Y, out raDeg2, out deDeg2);

                    double ra = m_RADegCenter + (raDeg2 - raDeg1);
                    double de = m_DEDegCenter + (deDeg2 - deDeg1);

                    if (m_SolvedPlate is LeastSquareFittedAstrometry)
                        m_SolvedPlate = new LeastSquareFittedAstrometry(m_Image, ra, de, null /*m_SolvePlateConts*/);
                    else if (m_SolvedPlate is TangentalTransRotAstrometry)
                        m_SolvedPlate = new TangentalTransRotAstrometry(m_SolvedPlate as TangentalTransRotAstrometry, m_Image, ra, de, m_Eta);
                    else
                        m_SolvedPlate = new DirectTransRotAstrometry(m_Image, ra, de, m_Eta, m_Aspect);
                }

                dirty = true;
			}

			if (dirty)
			{
				DrawCatalogStarsFit();
			}
		}

		protected void UpdateToolControlDisplay()
		{
			m_PlatesolveController.SetManualStarIdentificationMode(
				m_UserStarIdentification != null && m_UserStarIdentification.Count > 0);		
		}

		protected virtual void ReinitializePlateConstants()
		{
			m_Image = m_AstrometryController.GetCurrentAstroPlate();
			m_Image.EffectiveFocalLength = m_FLength;

			if (m_SolvedPlate is LeastSquareFittedAstrometry)
				m_SolvedPlate = new LeastSquareFittedAstrometry(m_Image, m_RADegCenter, m_DEDegCenter, null /*m_SolvePlateConts*/);
			else if (m_SolvedPlate is TangentalTransRotAstrometry)
				m_SolvedPlate = new TangentalTransRotAstrometry(m_SolvedPlate as TangentalTransRotAstrometry, m_Image, m_RADegCenter, m_DEDegCenter, m_Eta);
			else
			{
				m_SolvedPlate = new DirectTransRotAstrometry(m_Image, m_RADegCenter, m_DEDegCenter, m_Eta, m_Aspect);
			}
		}

		protected StarMapFeature m_PreviousFeature;
		protected IStar m_PreviousStar;

		protected virtual void IdentifyStar(StarMapFeature feature, IStar star)
		{
			if (m_SolvedPlate != null)
			{
				ImagePixel featureCenter = feature.GetCenter();

				if (m_PreviousStar == null ||
					m_PreviousStar.StarNo == star.StarNo)
				{
					// Translation only
					double x, y;
					m_SolvedPlate.GetImageCoordsFromRADE(star.RADeg, star.DEDeg, out x, out y);

					double newCenterX = m_Image.FullFrame.Width / 2 - featureCenter.X + x;
					double newCenterY = m_Image.FullFrame.Height / 2 - featureCenter.Y + y;

					m_SolvedPlate.GetRADEFromImageCoords(newCenterX, newCenterY, out m_RADegCenter, out m_DEDegCenter);

					m_PreviousFeature = feature;
					m_PreviousStar = star;
				}
				else
				{
					// TODO: Rotate and streach until the feature fits 

				}


				ReinitializePlateConstants();
				DrawCatalogStarsFit();

				// TODO: For now we only support 1 star centering
				m_PreviousStar = null;
				m_PreviousStar = null;
			}
		}

		protected void ResetPreviousStar()
		{
			m_PreviousStar = null;
			m_PreviousFeature = null;
		}

		#region IPlateCalibrationTool Members

		protected int m_FitGrade = 0;

		public void ChangePlottedFit(int fitGrade)
		{
			m_FitGrade = fitGrade;
			DrawCatalogStarsFit();
		}

		public abstract void RotationChanged(int newValue);

		public abstract void FocalLengthChanged(int newValue);

		public abstract void AspectChanged(double newValue);

		public abstract void LimitMagnitudeChanged(int newValue);

		public abstract void ShowLabels(bool showLabels);

		public abstract void ShowGrid(bool showLabels);

		public abstract void ShowMagnitudes(bool showMagnitudes);

		public abstract void Rotate180Degrees(bool rotate180);

		public abstract void Solve(bool debug);

		public virtual void ActivateCalibration() { }

		public virtual void ActivateOsdAreaSizing() { }

		public virtual void SetAreaType(AreaType areaType) { }

	    public virtual void SetTolerance(int tolerance)
	    {
	        m_Tolerance = tolerance;
	    }

	    protected OnAreaChanged AreaChangedHandler;

		public event OnAreaChanged AreaChanged
		{
			add
			{
				AreaChangedHandler -= value;
				AreaChangedHandler += value;
			}

			remove
			{
				AreaChangedHandler -= value;
			}
		}

		public void Set3StarIdMode()
		{
			m_Is3StarIdMode = true;
			m_VideoController.RedrawCurrentFrame(false);
			DrawCatalogStarsFit();
		}

		public void SetManualFitMode()
		{
			m_Is3StarIdMode = false;
			m_VideoController.RedrawCurrentFrame(false);
			DrawCatalogStarsFit();
		}
		#endregion
	}
}
