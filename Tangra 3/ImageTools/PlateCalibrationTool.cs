/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
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
using Tangra.VideoOperations.Astrometry;
using Tangra.VideoOperations.Astrometry.Engine;

namespace Tangra.ImageTools
{
	internal enum FittingsState
	{
		Configuring,
		Activated,
		Fitting,
		Solved
	}

	internal enum AreaType
	{
		OSDExclusion,
		Inclusion
	}

	internal delegate void OnAreaChanged();

	internal interface IPlateCalibrationTool
	{
		void RotationChanged(int newValue);
		void FocalLengthChanged(int newValue);
		void AspectChanged(double newValue);
		void LimitMagnitudeChanged(int newValue);
		void ShowLabels(bool showLabels);
		void ShowGrid(bool showLabels);
		void ShowMagnitudes(bool showMagnitudes);
		void Rotate180Degrees(bool rotate180);
		void Solve(bool debug);
		void ActivateCalibration();
		void ActivateOsdAreaSizing();
		void SetAreaType(AreaType areaType);
	    void SetTolerance(int tolerance);
		void ChangePlottedFit(int fitGrade);
		void Set3StarIdMode();
		void SetManualFitMode();
		event OnAreaChanged AreaChanged;
	}

	internal class PlateCalibrationTool : CalibrationToolBase, INotificationReceiver, IDisposable
	{
		internal PlateCalibrationTool(AstrometryController astrometryController, VideoController videoController, bool debugMode)
			: base(astrometryController, videoController)
		{
			m_AstrometryController.Subscribe(this, typeof(OperationNotifications));
			m_State = FittingsState.Configuring;

			m_DebugMode = debugMode;

			m_OSDExcluderTool = new OSDExcluder(videoController);
			m_OSDExcluderTool.AreaChanged += m_OSDExcluderTool_AreaChanged;

			if (m_PlatesolveController == null)
				m_PlatesolveController = new ucCalibrationPanel(astrometryController, videoController, this);
		}

		void m_OSDExcluderTool_AreaChanged()
		{
			if (AreaChangedHandler != null)
				AreaChangedHandler();
		}

		public override ZoomImageBehaviour ZoomBehaviour
		{
			get
			{
				return ZoomImageBehaviour.Custom;
			}
		}

		private OSDExcluder m_OSDExcluderTool;

		private FieldSolveContext m_FieldSolveContext;

		public override void MouseDown(Point location)
		{
			if (m_State == FittingsState.Configuring)
				m_OSDExcluderTool.MouseDown(location);
			if (m_State == FittingsState.Activated)
				base.MouseDown(location);
		}

		public override void MouseUp(Point location)
		{
			if (m_State == FittingsState.Configuring)
				m_OSDExcluderTool.MouseUp(location);
			if (m_State == FittingsState.Activated)
				base.MouseUp(location);
		}

		public override void MouseDoubleClick(Point location)
		{ }

		public override void MouseLeave()
		{
			if (m_State == FittingsState.Configuring)
				m_OSDExcluderTool.MouseLeave();
			if (m_State == FittingsState.Activated)
			{
				m_VideoController.InvalidatePictureBox();
			}
		}

		public override void MouseMove(Point location)
		{
			if (m_State == FittingsState.Activated)
				base.MouseMove(location);
			else if (m_State == FittingsState.Solved && m_SolvedPlate != null)
			{
				double ra, de;
				m_SolvedPlate.GetRADEFromImageCoords(location.X, location.Y, out ra, out de);

                string info = string.Format("RA={0} DE={1}", AstroConvert.ToStringValue(ra / 15, "HHhMMmSS.Ts"), AstroConvert.ToStringValue(de, "+DD°MM'SS\""));
				m_VideoController.DisplayCursorPositionDetails(location, info);
			}
			else
			{
				if (m_State == FittingsState.Configuring)
					m_OSDExcluderTool.MouseMove(location);

				m_VideoController.DisplayCursorPositionDetails(location, null);
			}
		}

		public override void MouseClick(ObjectClickEventArgs e)
		{
			if (m_State == FittingsState.Configuring)
				m_OSDExcluderTool.MouseClick(e);
			if (m_State == FittingsState.Solved)
			{
				IStarMap starMap = AstrometryContext.Current.StarMap;
				if (starMap != null)
				{
					int x, y;
					StarMapFeature feature = starMap.GetFeatureInRadius(e.Pixel.X, e.Pixel.Y, 5);
					if (feature != null)
					{
						x = feature.GetCenter().X;
						y = feature.GetCenter().Y;
					}
					else
					{
						x = e.Pixel.X;
						y = e.Pixel.Y;
					}

					int searchArea = Control.ModifierKeys == Keys.Shift ? 5 : 10;

					PSFFit psfFit;
					ImagePixel pixelCent = starMap.GetPSFFit(x, y, searchArea, out psfFit);
					if (pixelCent != null && pixelCent != ImagePixel.Unspecified)
					{
						PlateConstStarPair selectedPair = null;
						LeastSquareFittedAstrometry astrometry = FittedAstrometryFromUserSelectedFitGrade();
						if (astrometry != null)
						{
							foreach (PlateConstStarPair pair in astrometry.FitInfo.AllStarPairs)
							{
								if (Math.Abs(pair.x - pixelCent.X) < 2 &&
									Math.Abs(pair.y - pixelCent.Y) < 2)
								{
									selectedPair = pair;
									break;
								}
							}	
						}

						DrawHighResFeature(pixelCent, selectedPair, astrometry);
					}
					else
						ClearZoomImage();
				}
			}
		}

		public override void OnNewFrame(int currentFrameIndex, bool isLastFrame)
		{
			InitStarMap();

			DrawCatalogStarsFit();
		}

		protected override void ReinitializePlateConstants()
		{
			m_FieldSolveContext = AstrometryContext.Current.FieldSolveContext;
			base.ReinitializePlateConstants();
		}

		protected override void DrawEquatorialGrid(Graphics g)
		{
			if (m_State == FittingsState.Solved) return;

			base.DrawEquatorialGrid(g);
		}

		public override void PostDraw(Graphics g)
		{
			base.PostDraw(g);

			if (m_State == FittingsState.Configuring)
				m_OSDExcluderTool.PostDraw(g);
		}

		private void ClearZoomImage()
		{
			m_VideoController.ClearZoomedImage();
		}

		public override void Activate()
		{
			base.Activate();

			if (m_State != FittingsState.Configuring)
			{
				m_AstrometryController.SendNotification(new OperationNotifications(NotificationType.ConfigSolveStarted, null));
			}
			else
			{
				if (!TangraConfig.Settings.HelpFlags.DontShowCalibrationHelpFormAgain)
				{
					var frmInfo = new frmFirstTimeCalibrationRequired();
					frmInfo.StartPosition = FormStartPosition.CenterParent;
					m_VideoController.ShowDialog(frmInfo);
				}
			}
		}

		public override void Deactivate()
		{
			TangraConfig.Settings.Save();

			if (m_State != FittingsState.Configuring)
				m_AstrometryController.SendNotification(new OperationNotifications(NotificationType.ConfigSolveFinished, null));
		}

		public override void ActivateOsdAreaSizing()
		{
			if (m_State == FittingsState.Configuring)
			{
				m_OSDExcluderTool.Activate();
			}
		}

		public override void SetAreaType(AreaType areaType)
		{
			if (m_State == FittingsState.Configuring)
			{
				m_OSDExcluderTool.SetAreaType(areaType);
				m_VideoController.RedrawCurrentFrame(false, true);
			}
		}

		public override void ActivateCalibration()
		{
			if (m_State == FittingsState.Configuring)
				m_OSDExcluderTool.Deactivate();

			CompleteActivation();

			m_VideoController.RedrawCurrentFrame(false, true);

			TangraContext.Current.OSDExcludeToolDisabled = true;
			m_VideoController.UpdateViews();
		}

		protected void InitStarMap()
		{
			var starMap = new StarMap();

			AstroImage image = m_VideoController.GetCurrentAstroImage(false);

			starMap.FindBestMap(
				AstrometryContext.Current.StarMapConfig, 
				image,
				AstrometryContext.Current.OSDRectToExclude,
				AstrometryContext.Current.RectToInclude,
				AstrometryContext.Current.LimitByInclusion);

			AstrometryContext.Current.StarMap = starMap;

#if ASTROMETRY_DEBUG
			Trace.Assert(AstrometryContext.Current.StarMap != null);
			Trace.Assert(AstrometryContext.Current.StarMap.Features.Count > 0);
#endif
		}

		private void CompleteActivation()
		{
			m_VideoController.SetPictureBoxCursor(CustomCursors.PanCursor);

			m_FieldSolveContext = AstrometryContext.Current.FieldSolveContext;

			m_Image = m_AstrometryController.GetCurrentAstroPlate();
			m_Image.EffectiveFocalLength = m_FieldSolveContext.FocalLength;

			m_RADegCenter = m_FieldSolveContext.RADeg;
			m_DEDegCenter = m_FieldSolveContext.DEDeg;

			StarCatalogueFacade facade = new StarCatalogueFacade(TangraConfig.Settings.StarCatalogue);
			m_CatalogueStars = facade.GetStarsInRegion(
				m_FieldSolveContext.RADeg, m_FieldSolveContext.DEDeg,
				2.5 * m_Image.GetMaxFOVInArcSec() / 3600.0, m_FieldSolveContext.LimitMagn, (float)m_FieldSolveContext.Epoch);

			m_Eta = GetUserCameraConfigParam("rotation", 0.0);
			m_FLength = m_FieldSolveContext.FocalLength;
			m_Aspect = GetUserCameraConfigParam("aspect", 1.0);
			m_LimitMag = (int)Math.Round(m_FieldSolveContext.LimitMagn);
			m_ShowLabels = GetUserCameraConfigParam("labels", 0) == 1;
			m_Grid = GetUserCameraConfigParam("grid", 0) == 1;
			m_ShowMagnitudes = GetUserCameraConfigParam("magnitudes", 0) == 1;
			
			UpdateControls();

			InitStarMap();

			m_SolvedPlate = new DirectTransRotAstrometry(m_Image, m_FieldSolveContext.RADeg, m_FieldSolveContext.DEDeg, m_Eta, m_Aspect);

			m_CalibrationContext = new CalibrationContext();
			m_CalibrationContext.StarMap = AstrometryContext.Current.StarMap;
			m_CalibrationContext.PreliminaryFit = m_SolvedPlate;
			m_FieldSolveContext.CatalogueStars = m_CatalogueStars;
			m_CalibrationContext.FieldSolveContext = m_FieldSolveContext;
			m_CalibrationContext.PlateConfig = m_Image;

			TangraConfig.AstrometrySettings calibrationSettings = TangraConfig.Settings.Astrometry.Clone();
			calibrationSettings.MaxResidual = CorePyramidConfig.Default.CalibrationMaxResidual;
			calibrationSettings.PyramidDistanceToleranceInPixels = CorePyramidConfig.Default.CalibrationPyramidDistanceToleranceInPixels;
			calibrationSettings.MinimumNumberOfStars = CorePyramidConfig.Default.CalibrationNumberOfStars;
			m_PlateCalibrator = new PlateCalibration(m_CalibrationContext, calibrationSettings, m_AstrometryController);
			
			UpdateToolControlDisplay();
			m_PlatesolveController.SetupControls();
			m_SolveState = -1;
			DrawCatalogStarsFit();

			m_State = FittingsState.Activated;
		}

		private void UpdateControls()
		{
			m_PlatesolveController.UpdateAspect(m_Aspect);
			m_PlatesolveController.UpdateFocalLength((int)m_FLength);
			m_PlatesolveController.UpdateLimitMagnitude((int)Math.Round(m_LimitMag));
			m_PlatesolveController.UpdateRotation((int)m_Eta);
			m_PlatesolveController.UpdateShowGrid(m_Grid);
			m_PlatesolveController.UpdateShowLabels(m_ShowLabels);
			m_PlatesolveController.UpdateShowMagnitudes(m_ShowMagnitudes);
		}

		internal delegate void OnPixelOperation(int idx, int x, int y);

		private void OverCursorOperation(int x, int y, OnPixelOperation operation)
		{
			int[,] coeffs = { { 1, 0, 0, -16 }, { 1, 0, 0, 16 }, { 0, -16, 1, 0 }, { 0, 16, 1, 0 } };

			int idx = 0;
			for (int k = 0; k < 4; k++)
			{
				int a = coeffs[k, 0];
				int b = coeffs[k, 1];
				int c = coeffs[k, 2];
				int d = coeffs[k, 3];

				for (int i = -16; i < 17; i++)
				{
					operation(idx, x + a * i + b, y + c * i + d);
					idx++;
				}
			}
		}

		// TODO: Use the imeplementation in the MainForm instead
		private void DrawHighResFeature(ImagePixel pixel, PlateConstStarPair selectedPair, LeastSquareFittedAstrometry astrometry)
		{
			int x0 = pixel.X;
			int y0 = pixel.Y;
			if (x0 < 15) x0 = 15; if (y0 < 15) y0 = 15;
			if (x0 > AstrometryContext.Current.FullFrame.Width - 15) x0 = AstrometryContext.Current.FullFrame.Width - 15;
			if (y0 > AstrometryContext.Current.FullFrame.Height - 15) y0 = AstrometryContext.Current.FullFrame.Height - 15;

			int bytes;
			int bytesPerPixel;
			byte[] selectedPoints;
			int selIdx;

			Bitmap featureBitmap = new Bitmap(31 * 8, 31 * 8, PixelFormat.Format24bppRgb);
			m_VideoController.UpdateZoomedImage(featureBitmap, pixel);

			BitmapData zoomedData = featureBitmap.LockBits(new Rectangle(0, 0, 31 * 8, 31 * 8), ImageLockMode.ReadWrite, featureBitmap.PixelFormat);
			try
			{
				bytes = zoomedData.Stride * featureBitmap.Height;
				byte[] zoomedValues = new byte[bytes];

				Marshal.Copy(zoomedData.Scan0, zoomedValues, 0, bytes);

				bytesPerPixel = AstrometryContext.Current.BytesPerPixel;

				byte saturatedR = TangraConfig.Settings.Color.Saturation.R;
				byte saturatedG = TangraConfig.Settings.Color.Saturation.G;
				byte saturatedB = TangraConfig.Settings.Color.Saturation.B;

				selIdx = 0;
				for (int y = 0; y < 31; y++)
					for (int x = 0; x < 31; x++)
					{
						for (int i = 0; i < 8; i++)
							for (int j = 0; j < 8; j++)
							{
								int zoomedX = 8 * x + i;
								int zoomedY = 8 * y + j;

								int zoomedIdx = zoomedData.Stride * zoomedY + zoomedX * bytesPerPixel;

								if (zoomedValues[zoomedIdx] > TangraConfig.Settings.Photometry.Saturation.Saturation8Bit)
								{
									// Saturation detected
									zoomedValues[zoomedIdx] = saturatedR;
									zoomedValues[zoomedIdx + 1] = saturatedG;
									zoomedValues[zoomedIdx + 2] = saturatedB;
								}
							}

						selIdx++;
					}

				Marshal.Copy(zoomedValues, 0, zoomedData.Scan0, bytes);
			}
			finally
			{
				featureBitmap.UnlockBits(zoomedData);
			}

			Pen starPen = catalogStarPen;

			double xFitUnscaled = pixel.XDouble;
			double yFitUnscaled = pixel.YDouble;

			if (selectedPair != null && selectedPair.FitInfo.UsedInSolution)
			{
				starPen = referenceStarPen;
				astrometry.GetImageCoordsFromRADE(selectedPair.RADeg, selectedPair.DEDeg, out xFitUnscaled, out yFitUnscaled);
			}
			else if (selectedPair != null && selectedPair.FitInfo.ExcludedForHighResidual)
			{
				starPen = rejectedStarPen;
				astrometry.GetImageCoordsFromRADE(selectedPair.RADeg, selectedPair.DEDeg, out xFitUnscaled, out yFitUnscaled);
			}
			else
			{
				starPen = unrecognizedStarPen;
			}

			double xFit = (8 * (xFitUnscaled - x0 + 16)) - 4;
			double yFit = (8 * (yFitUnscaled - y0 + 16)) - 4;
		}

		#region IPictureToolControl Members

		private void SaveUserCameraConfigParam(string paramName, object paramValue)
		{
			string strVal = Convert.ToString(paramValue, CultureInfo.InvariantCulture);
			//if (AppConfig.Instance.GenericProperties.ContainsKey(paramName))
			//    AppConfig.Instance.GenericProperties[paramName] = strVal;
			//else
			//    AppConfig.Instance.GenericProperties.Add(paramName, strVal);
		}

		private string GetUserCameraConfigParam(string paramName)
		{
			//string strVal;
			//if (AppConfig.Instance.GenericProperties.TryGetValue(paramName, out strVal))
			//{
			//    return strVal;
			//}

			return null;
		}

		private double GetUserCameraConfigParam(string paramName, double defaultValue)
		{
			string strVal = GetUserCameraConfigParam(paramName);
			if (string.IsNullOrEmpty(strVal)) return defaultValue;

			return double.Parse(strVal);
		}

		private int GetUserCameraConfigParam(string paramName, int defaultValue)
		{
			string strVal = GetUserCameraConfigParam(paramName);
			if (string.IsNullOrEmpty(strVal)) return defaultValue;

			return int.Parse(strVal);
		}

		public override void AspectChanged(double newValue)
		{
			m_Aspect = newValue;
			ReinitializePlateConstants();
			DrawCatalogStarsFit();
			SaveUserCameraConfigParam("aspect", newValue);
		}

		public override void FocalLengthChanged(int newValue)
		{
			m_FLength = newValue;
			ReinitializePlateConstants();
			DrawCatalogStarsFit();
			SaveUserCameraConfigParam("foclen", newValue);
		}

		public override void LimitMagnitudeChanged(int newValue)
		{
			m_LimitMag = newValue;
			DrawCatalogStarsFit();
			SaveUserCameraConfigParam("limmag", newValue);
		}

		public override void RotationChanged(int newValue)
		{
			m_Eta = newValue;
			ReinitializePlateConstants();
			DrawCatalogStarsFit();
			SaveUserCameraConfigParam("rotation", newValue);
		}

		public override void Rotate180Degrees(bool rotate180)
		{
			// Nothing to do now
		}

		public override void ShowLabels(bool showLabels)
		{
			m_ShowLabels = showLabels;
			DrawCatalogStarsFit();
			SaveUserCameraConfigParam("labels", showLabels);
		}

		public override void ShowGrid(bool showGrid)
		{
			m_Grid = showGrid;
			DrawCatalogStarsFit();
			SaveUserCameraConfigParam("grid", showGrid);
		}

		public override void ShowMagnitudes(bool showMagnitudes)
		{
			m_ShowMagnitudes = showMagnitudes;
			DrawCatalogStarsFit();
			SaveUserCameraConfigParam("magnitudes", showMagnitudes);
		}

		public override void Solve(bool debug)
		{
			m_State = FittingsState.Fitting;
			m_VideoController.SetPictureBoxCursor(Cursors.Arrow);

			m_PlateCalibrator.PreliminaryFit = new DirectTransRotAstrometry(m_Image, m_RADegCenter, m_DEDegCenter, m_Eta, m_Aspect);

			SolvePlateConstants(debug);
		}

		private int m_SolveState = 0;

		#endregion

		private bool m_DebugMode = false;

		private void SolvePlateConstants(bool debug)
		{
			m_LimitMag = CorePyramidConfig.Default.CalibrationFirstPlateSolveLimitMagnitude;
			for (double maxMag = CorePyramidConfig.Default.CalibrationFirstPlateSolveLimitMagnitude; maxMag <= m_FieldSolveContext.LimitMagn; maxMag += 0.5)
			{
				int count = m_FieldSolveContext.CatalogueStars.Count(s => s.Mag < maxMag);
				if (count > CorePyramidConfig.Default.CalibrationMaxStarsForAlignment)
					break;
				else
					m_LimitMag = maxMag;
			}

			if (m_DebugMode && debug)
			{
				if (m_SolveState == -1)
				{
					Trace.WriteLine("Course Fit");
					m_PlateCalibrator.SolvePlateConstantsPhase1(m_LimitMag, false);
					m_PlateCalibrator.SolvePlateConstantsPhase2(FitOrder.Linear, false);
					DrawCatalogStarsFit();
					DrawStarPairs(m_LimitMag);
				}


				if (m_SolveState == 0)
				{
					Trace.WriteLine("Fine Fit");
					m_LimitMag = (int)Math.Ceiling(m_FieldSolveContext.LimitMagn);
					m_PlateCalibrator.SolvePlateConstantsPhase1(m_LimitMag, true);
					m_PlateCalibrator.SolvePlateConstantsPhase2(FitOrder.Linear, true);

					DrawCatalogStarsFit();
					DrawStarPairs(m_LimitMag);
				}

				if (m_SolveState == 1)
				{
					m_SolvedPlate = m_PlateCalibrator.SolvePlateConstantsPhase4(m_LimitMag);
					DrawCatalogStarsFit();
					m_PlatesolveController.OnCalibrationFinished();
					m_PlatesolveController.OnSuccessfulCalibration(m_CalibrationContext);
					DrawCatalogStarsFit();
				}

				m_SolveState++;
			}
			else
			{
				if (m_PlatesolveController != null &&
					m_PlatesolveController.ParentForm != null)
				{
					m_PlatesolveController.ParentForm.Cursor = Cursors.WaitCursor;
					try
					{
			
						m_SolveState = 0;
						m_PlateCalibrator.SolvePlateConstantsPhase1(m_LimitMag, false);

						if (m_SolveState != -1)
						{
							m_SolveState = 1;
							m_PlateCalibrator.SolvePlateConstantsPhase2(FitOrder.Linear, false);

						}

						if (m_CalibrationContext.FirstAstrometricFit == null) m_SolveState = -1;

						if (m_SolveState != -1)
						{
							m_SolveState = 2;
							m_PlateCalibrator.SolvePlateConstantsPhase1(m_LimitMag, true);
						}

						if (m_SolveState != -1)
						{
							m_SolveState = 3;
							m_PlateCalibrator.SolvePlateConstantsPhase2(FitOrder.Linear, true);
						}

						if (m_CalibrationContext.SecondAstrometricFit == null) m_SolveState = -1;

						m_PlatesolveController.OnCalibrationFinished();

						if (m_SolveState != -1)
						{
							m_SolveState = 4;
							m_SolvedPlate = m_PlateCalibrator.SolvePlateConstantsPhase4(m_LimitMag);

							if (m_CalibrationContext.PlateConstants != null)
							{
								TangraConfig.Settings.PlateSolve.SetPlateConstants(m_CalibrationContext.PlateConstants, m_CalibrationContext.PlateConfig.FullFrame);
								TangraConfig.Settings.Save();

								m_UserStarIdentification.Clear();

								m_FitGrade = 4; /* Use the improved fit if possible */
								m_LimitMag = m_CalibrationContext.FieldSolveContext.LimitMagn;
								m_PlatesolveController.UpdateLimitMagnitude((int) Math.Round(m_LimitMag));

								DrawCatalogStarsFit();

								MessageBox.Show(
									"Tangra successfully calibrated this configuration. Now you can open \r\n" +
									"and process videos recorded with this equipment.\r\n\r\n" +
									"If you want to process the current video you will have to reload it.", "Configuration Solved",
									MessageBoxButtons.OK, MessageBoxIcon.Information);

								// The video must be reloaded.
								TangraContext.Current.HasVideoLoaded = false;
								m_VideoController.UpdateViews();

								m_PlatesolveController.OnSuccessfulCalibration(m_CalibrationContext);
								m_State = FittingsState.Solved;
							}
                            else
							{
                                m_PlatesolveController.OnUnsuccessfulCalibration(m_CalibrationContext, m_PlateCalibrator);

                                m_State = FittingsState.Activated;
                                m_VideoController.SetPictureBoxCursor(CustomCursors.PanEnabledCursor);
                                MessageBox.Show(
                                    "Tangra was unable to calibrate this configuration. Please try again.", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                                m_RADegCenter = m_CalibrationContext.PreliminaryFit.RA0Deg;
                                m_DEDegCenter = m_CalibrationContext.PreliminaryFit.DE0Deg;
                                m_SolvedPlate = m_CalibrationContext.PreliminaryFit;

								DrawCatalogStarsFit();
							}
						}
						else
						{
                            m_PlatesolveController.OnUnsuccessfulCalibration(m_CalibrationContext, m_PlateCalibrator);

							m_State = FittingsState.Activated;
							m_VideoController.SetPictureBoxCursor(CustomCursors.PanEnabledCursor);
							MessageBox.Show(
								"Tangra was unable to calibrate this configuration. Please try again.", "Error",
							    MessageBoxButtons.OK, MessageBoxIcon.Error);

							m_RADegCenter = m_CalibrationContext.PreliminaryFit.RA0Deg;
							m_DEDegCenter = m_CalibrationContext.PreliminaryFit.DE0Deg;
							m_SolvedPlate = m_CalibrationContext.PreliminaryFit;

							DrawCatalogStarsFit();
						}
					}
					finally
					{
						m_PlatesolveController.ParentForm.Cursor = Cursors.Default;
					}
				}
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			m_AstrometryController.Unsubscribe(this);
		}

		#endregion

		public void ReceieveMessage(object message)
		{
			var notification = message as OperationNotifications;
			if (notification != null && notification.Notification == NotificationType.ConfigSolveContinueStartup)
			{
				CompleteActivation();
			}
		}
	}
}
