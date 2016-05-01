/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Tangra.Astrometry;
using Tangra.AstroServices;
using Tangra.Config;
using Tangra.Controller;
using Tangra.Helpers;
using Tangra.ImageTools;
using Tangra.Model.Config;
using Tangra.Model.Helpers;
using Tangra.Model.Image;
using Tangra.Model.Numerical;
using Tangra.Model.VideoOperations;
using Tangra.StarCatalogues;
using Tangra.VideoOperations.Astrometry.Engine;
using Tangra.VideoOperations.Astrometry.MPCReport;
using Tangra.VideoOperations.LightCurves;

namespace Tangra.VideoOperations.Astrometry
{
	public partial class ucAstrometryObjectInfo : UserControl, INotificationReceiver, IMPCReportFileManager
	{
		public class AstrometryDisplaySettingsEventArgs : EventArgs
		{
			public readonly bool ShowStarLabels;
			public readonly bool ShowStarMagnitudes;
			public readonly bool ShowObjectLabels;

			public AstrometryDisplaySettingsEventArgs(bool showStarLabels, bool showStarMagnitudes, bool showObjectLabels)
			{
				ShowStarLabels = showStarLabels;
				ShowStarMagnitudes = showStarMagnitudes;
				ShowObjectLabels = showObjectLabels;
			}
		}

		internal class StartAstrometricMeasurementsEventArgs : EventArgs
		{
			public readonly MeasurementContext Context;

			public StartAstrometricMeasurementsEventArgs(MeasurementContext context)
			{
				Context = context;
			}
		}

		internal class SaveUnitTestDataEventArgs : EventArgs
		{
			public readonly string OutputFileName;

			public SaveUnitTestDataEventArgs(string outputFileName)
			{
				OutputFileName = outputFileName;
			}
		}

		internal event EventHandler<StartAstrometricMeasurementsEventArgs> OnStartMeasurementCommand;
		internal event EventHandler OnStopMeasurementCommand;
		internal event EventHandler OnIdentifyObjectsCommand;
        internal event EventHandler OnSendErrorReport;
	    internal event EventHandler OnDifferentFieldCenter;
		internal event EventHandler OnResolveObject;
		internal event EventHandler<SaveUnitTestDataEventArgs> OnSaveUnitTestData;

		private Dictionary<int, SingleMultiFrameMeasurement> m_AllMeasurements = new Dictionary<int, SingleMultiFrameMeasurement>();
		private double m_MinRA, m_MaxRA, m_MinDE, m_MaxDE;
		private int m_MinFrame, m_MaxFrame;
		private double m_UserRAMid = double.NaN;
		private double m_UserDEMid = double.NaN;
		private int m_UserFrame = int.MinValue;

		private Rectangle m_FullRect;
		private UserObjectContext m_UserObject = new UserObjectContext();
		internal MeasurementContext m_MeasurementContext;
		internal List<IStar> m_CatalogueStars;
		private AstrometricState m_AstrometricState;
		private bool m_JustAfterNewFit = true;

		private int m_EarliestRAFrame = int.MaxValue;
		private int m_LatestRAFrame = int.MinValue;
		private int m_EarliestDEFrame = int.MaxValue;
		private int m_LatestDEFrame = int.MinValue;

		private static Font s_SymbolFont12 = new Font("Symbol", 12, FontStyle.Bold);

		private static Pen s_RAAveragePen = new Pen(Color.FromArgb(0, 128, 192));
		private static Pen s_DEAveragePen = new Pen(Color.FromArgb(0, 128, 64));

		internal AstrometryController m_AstrometryController;
		private VideoController m_VideoController;

		private Bitmap m_RAImage;
		private Bitmap m_DEImage;

		public ucAstrometryObjectInfo()
		{
			InitializeComponent();
		}

		public ucAstrometryObjectInfo(AstrometryController astrometryController, VideoController videoController)
		{
			InitializeComponent();

			m_AstrometryController = astrometryController;
			m_VideoController = videoController;

			m_AstrometryController.Subscribe(this, typeof(OperationNotifications));

			pbPSFFit.Image = new Bitmap(pbPSFFit.Width, pbPSFFit.Height);
			m_FullRect = new Rectangle(0, 0, pbPSFFit.Width, pbPSFFit.Height);

			m_RAImage = new Bitmap(pnlRASeries.Width, pnlRASeries.Height);
			m_DEImage = new Bitmap(pnlDESeries.Width, pnlDESeries.Height);

			m_AstrometricState = AstrometricState.EnsureAstrometricState();

			m_CurrentReportFile = AstrometryContext.Current.CurrentReportFile;

			btnResolveObjects.Visible = false;
			btnSaveUnitTestData.Visible = false;

#if GET_UNIT_TEST_DATA
            btnResolveObjects.Visible = true;
			btnSaveUnitTestData.Visible = true;
#endif

		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}

			m_AstrometryController.Unsubscribe(this);

			base.Dispose(disposing);
		}

		internal void InitializeNewAstrometry()
		{
			SwitchToStandardControls();
			pnlAstrometry.Visible = true;
			pnlMeasurements.Visible = false;

			pnlSwitchControl.Visible = false;

			lblTimeValue.Visible = false;
			lblAverageTime.Visible = false;
			lblAlpha.Visible = false;
			lblDelta.Visible = false;

			m_MPCDE = double.NaN;
			m_MPCRAHours = double.NaN;

			btnAddToMCPReport.Enabled = false;

			Refresh();
		}

		private void SwitchToStandardControls()
		{
			pnlStandardControls.Visible = true;
			pnlAstrometryControls.Visible = false;
			pnlStandardControls.Left = 2;
			pnlStandardControls.Top = 203;
		}

		private void SwitchToAstrometryControls(bool endData)
		{
			pnlStandardControls.Visible = false;
			pnlAstrometryControls.Visible = true;
			pnlAstrometryControls.Left = 2;
			pnlAstrometryControls.Top = 203;

			if (endData)
			{
				pnlEndAstrometry.Visible = true;
				btnAddToMCPReport.Enabled = false;
				pnlSwitchControl.Visible = true;

				btnStop.Visible = false;
				btnIdentify.Visible = false;
				btnManuallyIdentifyStar.Visible = false;
			}
			else
			{
				pnlEndAstrometry.Visible = false;
				btnStop.Visible = true;
			}
		}

		internal void AddNewMeasurement(SingleMultiFrameMeasurement measurement)
		{
			if (measurement != null)
			{
				if (m_AllMeasurements.ContainsKey(measurement.FrameNo))
					return;

				m_AllMeasurements.Add(measurement.FrameNo, measurement);

				double minRa = measurement.RADeg - measurement.StdDevRAArcSec / 3600.0;
				double maxRa = measurement.RADeg + measurement.StdDevRAArcSec / 3600.0;
				double minDe = measurement.DEDeg - measurement.StdDevDEArcSec / 3600.0;
				double maxDe = measurement.DEDeg + measurement.StdDevDEArcSec / 3600.0;

				if (m_MinRA > minRa) m_MinRA = minRa;
				if (m_MaxRA < maxRa) m_MaxRA = maxRa;
				if (m_MinDE > minDe) m_MinDE = minDe;
				if (m_MaxDE < maxDe) m_MaxDE = maxDe;
				if (m_MinFrame > measurement.FrameNo) m_MinFrame = measurement.FrameNo;
				if (m_MaxFrame < measurement.FrameNo) m_MaxFrame = measurement.FrameNo;

				DrawRAPanel();
				DrawDEPanel();
			}
		}

		internal void FrameMeasurementStarted()
		{
			InitNewMultiframeMeasurement();

			SwitchToAstrometryControls(false);
			pnlSwitchControl.Visible = true;
		}

		internal void FrameMeasurementFinished()
		{
			pnlSwitchControl.Visible = true;
			ShowMeasurementsView();
		}

		private void InitNewMultiframeMeasurement()
		{
			m_AllMeasurements.Clear();
			m_MinRA = m_MinDE = double.MaxValue;
			m_MaxRA = m_MaxDE = double.MinValue;
			m_MinFrame = int.MaxValue;
			m_MaxFrame = int.MinValue;
			m_UserRAMid = double.NaN;
			m_UserDEMid = double.NaN;
			m_UserFrame = int.MinValue;
			m_AstrometricState.Measurements.Clear();

			pnlMeasurements.Visible = true;
			pnlObject.Visible = false;
			pnlAstrometry.Visible = false;
			pnlMeasurements.Top = 0;
			pnlMeasurements.Left = 0;

			lblAlpha.Visible = false;
			lblDelta.Visible = false;
			lblRate.Visible = false;
			lblRateVal.Visible = false;

			m_MPCDE = double.NaN;
			m_MPCRAHours = double.NaN;

			lblTimeValue.Visible = false;
			lblAverageTime.Visible = false;
			lblAstRA.Text = string.Empty;
			lblAstDE.Text = string.Empty;
		}

		internal void PresentSelectedObject(SelectedObject objInfo)
		{
			SwitchToStandardControls();
			pnlSwitchControl.Visible = m_AllMeasurements.Count > 0;
			m_JustAfterNewFit = false;

			pnlObject.Visible = true;
			pnlAstrometry.Visible = false;
			pnlMeasurements.Visible = false;
			pnlObject.Top = 0;
			pnlObject.Left = 0;

			pnlObject.Visible = objInfo != null;
			btnFramesAstrometry.Visible = false;

			if (objInfo != null)
			{
				if (objInfo.FittedStar != null)
				{
                    if (TangraConfig.Settings.StarCatalogue.Catalog == TangraConfig.StarCatalog.PPMXL)
					    lblObjectName.Text = objInfo.FittedStar.StarNo.ToString(); // PPMXL numbers are too large so to save space don't add the catalog name
                    else
						lblObjectName.Text = string.Format("{0} {1}", TangraConfig.Settings.StarCatalogue.Catalog, objInfo.FittedStar.StarNo);
					lblX.Text = objInfo.FittedStar.x.ToString("0.0");
					lblY.Text = objInfo.FittedStar.y.ToString("0.0");

					if (AstrometryContext.Current.CurrentPhotometricFit != null)
					{
						bool isSaturated;
						double I = AstrometryContext.Current.CurrentPhotometricFit.GetIntencity(objInfo.Pixel, out isSaturated);

						double colorJKIndex = double.NaN;

						IStar matchedStar = m_CatalogueStars.FirstOrDefault(s => s.StarNo == objInfo.FittedStar.StarNo);
						if (matchedStar != null)
							colorJKIndex = matchedStar.MagJ - matchedStar.MagK;
						else
							colorJKIndex = ColourIndexTables.GetJKFromVR(m_MeasurementContext.AssumedTargetVRColour);

						double mag = AstrometryContext.Current.CurrentPhotometricFit.GetMagnitudeForIntencity(I, colorJKIndex);
						if (double.IsNaN(mag))
							lblIntensity.Text = string.Format("{0}", I.ToString("0"));
						else
							lblIntensity.Text = string.Format("{0} (m = {1})", I.ToString("0"), mag.ToString("0.0"));
						lblMagFitTitle.Visible = true;
						lblIntensity.Visible = true;
						lblIntensity.ForeColor = isSaturated ? Color.Red : SystemColors.ControlText;
					}
					else
					{
						lblIntensity.Visible = false;
						lblMagFitTitle.Visible = false;
					}

					lblMag.Text = string.Format("m = {0}", objInfo.FittedStar.Mag.ToString("0.0"));
					lblResRaTitle.Text = "Residual RA =";
					lblResDeTitle.Text = "Residual DE =";
					lblResRA.Text = string.Format("{0}\"", objInfo.FittedStar.FitInfo.ResidualRAArcSec.ToString("0.00"));
					lblResDE.Text = string.Format("{0}\"", objInfo.FittedStar.FitInfo.ResidualDEArcSec.ToString("0.00"));
				}
				else
				{
					lblX.Text = objInfo.Pixel.XDouble.ToString("0.0");
					lblY.Text = objInfo.Pixel.YDouble.ToString("0.0");

					if (objInfo.IdentifiedObject != null)
					{
						lblObjectName.Text = objInfo.IdentifiedObject.ObjectName;
						lblMag.Text = string.Format("m = {0}", objInfo.IdentifiedObject.Mag.ToString("0.0"));
					}
					else
					{
						lblObjectName.Text = "Unknown Object";
						lblMag.Text = "";
					}

					lblResRaTitle.Text = "Std.Dev RA =";
					lblResDeTitle.Text = "Std.Dev DE =";

					lblResRA.Text = string.Format("{0}\"", objInfo.Solution.StdDevRAArcSec.ToString("0.00"));
					lblResDE.Text = string.Format("{0}\"", objInfo.Solution.StdDevDEArcSec.ToString("0.00"));
				}

				lblRA.Text = AstroConvert.ToStringValue(objInfo.RADeg / 15.0, "HH MM SS.TT");
				lblDE.Text = AstroConvert.ToStringValue(objInfo.DEDeg, "+DD MM SS.T");

				btnFramesAstrometry.Visible = !m_AstrometricState.ObjectToMeasureSelected;
				if (!m_AstrometricState.ObjectToMeasureSelected)
				{
					m_UserObject.RADeg = objInfo.RADeg;
					m_UserObject.DEDeg = objInfo.DEDeg;
					m_UserObject.X0 = (float)objInfo.Pixel.XDouble;
					m_UserObject.Y0 = (float)objInfo.Pixel.YDouble;
				}

				if (objInfo.Gaussian != null)
				{
					pbPSFFit.Visible = true;

					using (Graphics g = Graphics.FromImage(pbPSFFit.Image))
					{
						objInfo.Gaussian.DrawGraph(g, m_FullRect, m_VideoController.VideoBitPix, m_VideoController.VideoAav16NormVal);
						g.Save();
					}

					pbPSFFit.Invalidate();

					double certaintyRounded = Math.Round(objInfo.Gaussian.Certainty * 100) / 100.0;
					lblMax.Text = certaintyRounded < 0 ? "N/A" : (certaintyRounded).ToString("0.00");
					lblFWHM.Text = objInfo.Gaussian.FWHM.ToString("0.00");
					pnlPSFInfo.Visible = true;
				}
				else
				{
					pbPSFFit.Visible = false;
					pnlPSFInfo.Visible = false;
				}

				if (AstrometryContext.Current.CurrentPhotometricFit != null && objInfo.Pixel != null)
				{
					bool isSaturated;
					double I = AstrometryContext.Current.CurrentPhotometricFit.GetIntencity(objInfo.Pixel, out isSaturated);

					double colorJKIndex = double.NaN;

					IStar matchedStar = objInfo.FittedStar != null
					    ? m_CatalogueStars.FirstOrDefault(s => s.StarNo == objInfo.FittedStar.StarNo)
					    : null;

					if (matchedStar != null)
						colorJKIndex = matchedStar.MagJ - matchedStar.MagK;
					else
					{						 
						colorJKIndex = ColourIndexTables.GetJKFromVR(m_MeasurementContext != null 
							? m_MeasurementContext.AssumedTargetVRColour 
							: TangraConfig.Settings.Astrometry.AssumedTargetVRColour);
					}

					double mag = AstrometryContext.Current.CurrentPhotometricFit.GetMagnitudeForIntencity(I, colorJKIndex);
						
					if (double.IsNaN(mag))
						lblIntensity.Text = string.Format("{0}", I.ToString("0"));
					else
						lblIntensity.Text = string.Format("{0} (m = {1})", I.ToString("0"), mag.ToString("0.0"));
					lblIntensity.ForeColor = isSaturated ? Color.Red : SystemColors.ControlText;

					lblIntensity.Visible = true;
					lblMagFitTitle.Visible = true;
				}
				else
					lblIntensity.Text = "";
			}
		}

		internal void PresentAstrometricFit(LeastSquareFittedAstrometry fit, StarMagnitudeFit magFit)
		{
			pnlAstrometry.Visible = true;
			pnlObject.Visible = false;
			pnlMeasurements.Visible = false;
			pnlAstrometry.Top = 0;
			pnlAstrometry.Left = 0;

			pnlAstrometry.Visible = fit != null;
			pnlFitSuccessful.Visible = fit != null;
			pnlFitFailed.Visible = !pnlFitSuccessful.Visible;
			pnlFitInfo.Visible = fit != null;

			if (fit != null)
			{
				lblStdDevRA.Text = string.Format("{0}\"", fit.StdDevRAArcSec.ToString("0.00"));
				lblStdDevDE.Text = string.Format("{0}\"", fit.StdDevDEArcSec.ToString("0.00"));

				if (magFit != null)
				{
					if (double.IsNaN(magFit.Sigma))
						lblStdDevMag.Text = "NaN";
					else
						lblStdDevMag.Text = string.Format("{0} mag", magFit.Sigma.ToString("0.00"));
				}
				else
					lblStdDevMag.Text = "N/A";

				double onePixX = fit.GetDistanceInArcSec(fit.Image.CenterXImage, fit.Image.CenterYImage, fit.Image.CenterXImage + 1, fit.Image.CenterYImage);
				double onePixY = fit.GetDistanceInArcSec(fit.Image.CenterXImage, fit.Image.CenterYImage, fit.Image.CenterXImage, fit.Image.CenterYImage + 1);

				lblPixelSizeX.Text = string.Format("{0}\"", onePixX.ToString("0.0"));
				lblPixelSizeY.Text = string.Format("{0}\"", onePixY.ToString("0.0"));
			}
		}

		public void FitFailed()
		{
			InitializeNewAstrometry();

			pnlAstrometry.Visible = true;
			pnlObject.Visible = false;
			pnlMeasurements.Visible = false;
			pnlAstrometry.Top = 0;
			pnlAstrometry.Left = 0;
			pnlAstrometry.BringToFront();

			pnlFitFailed.Visible = true;
			pnlFitSuccessful.Visible = false;
			pnlFitFailed.BringToFront();
		}

		public void ReceieveMessage(object notification)
		{
			var opNotification = notification as OperationNotifications;

			if (opNotification != null &&
				opNotification.Notification == NotificationType.NewAstrometricFit)
			{
				m_JustAfterNewFit = true;

				if (m_AstrometricState.MeasuringState == AstrometryInFramesState.Ready)
				{
					LeastSquareFittedAstrometry fit = opNotification.Argument as LeastSquareFittedAstrometry;
					StarMagnitudeFit magFit = opNotification.Argument2 as StarMagnitudeFit;
					PresentAstrometricFit(fit, magFit);
				}
			}
			else if (opNotification != null &&
				opNotification.Notification == NotificationType.NewSelectedObject)
			{
				PresentSelectedObject((SelectedObject)opNotification.Argument);
			}
			else if (opNotification != null &&
				opNotification.Notification == NotificationType.NewAstrometricMeasurement)
			{
				AddNewMeasurement((SingleMultiFrameMeasurement)opNotification.Argument);
			}
			else if (opNotification != null &&
				opNotification.Notification == NotificationType.EndOfMeasurementLastFrame)
			{
				ShowMeasurementsView();
			}
		}

		private void btnFramesAstrometry_Click(object sender, EventArgs e)
		{
			if (OnStartMeasurementCommand != null)
			{
			    lblM.Visible = false;
                lblMeaMag.Visible = false;

				OnStartMeasurementCommand(this,
					new StartAstrometricMeasurementsEventArgs(
						new MeasurementContext() { ObjectToMeasure = m_UserObject }));
			}
		}

		private void btnStop_Click(object sender, EventArgs e)
		{
			if (OnStopMeasurementCommand != null)
				OnStopMeasurementCommand(this, EventArgs.Empty);
		}


		private static Pen s_SecondLinesPen = new Pen(Color.FromArgb(140, 140, 140));

        private ProcessingReturnValues DrawRAPanel()
		{
            ProcessingReturnValues retVal = null;

			using (Graphics g = Graphics.FromImage(m_RAImage))
			{
				g.Clear(Color.Gray);

				int midY = m_RAImage.Height / 2;
				g.DrawLine(Pens.DarkGray, 5, midY, m_RAImage.Width - 5, midY);

				int minFrame = m_AllMeasurements.Keys.Min();
				int maxFrame = m_AllMeasurements.Keys.Max();
				int frames = 1 +  maxFrame - minFrame;

				if (m_AllMeasurements.Count > 0)
				{
                    float xScale = m_RAImage.Width * 1.0f / frames;
					float yScale = (m_RAImage.Height - 10) / (float)(m_MaxRA - m_MinRA);

					double from = m_MinRA - (6 / 3600.0);
					double to = m_MaxRA + (6 / 3600.0);
					double curr = Math.Truncate((from * 3600.0) / 5) * 5 / (3600.0);
					while (curr < to)
					{
						float y = (float)(curr - m_MinRA) * yScale + 5;
						g.DrawLine(s_SecondLinesPen, 5, y, m_RAImage.Width - 5, y);
						curr += (1 / 3600.0);
					}

					var meaVals = new MeasurementValues
					{
						Context = m_MeasurementContext,
						UserMidValue = m_UserRAMid,
						UserMidFrame = m_UserFrame,
						MaxStdDev = m_MeasurementContext.MaxStdDev/3600.0,
						MinValue = m_MinRA,
						IncludedPen = Pens.SkyBlue,
						ExcludedPen = Pens.Tomato,
						AveragePen = s_RAAveragePen,
                        FirstVideoFrame = m_VideoController.VideoFirstFrame,
						MinFrameNo = minFrame,
						MaxFrameNo = maxFrame
					};
		
					#region Compute the RA or DE value at the middle of the interval

						
					GetProcessingValueCallback processingValueCallback =
						delegate(SingleMultiFrameMeasurement measurement)
						{
							return new ProcessingValues
							{
								StdDev = measurement.StdDevRAArcSec / 3600.0,
								Value = measurement.RADeg
							};
						};

					switch (m_MeasurementContext.MovementExpectation)
					{
						case MovementExpectation.Slow:
							retVal = FitAndPlotSlowMotion(
										m_AllMeasurements, meaVals, processingValueCallback,
										g, xScale, yScale, m_RAImage.Width);
							break;

						case MovementExpectation.SlowFlyby:
							retVal = FitAndPlotSlowFlyby(
										m_AllMeasurements, meaVals, processingValueCallback,
										g, xScale, yScale, m_RAImage.Width, m_RAImage.Height);

							break;

						case MovementExpectation.FastFlyby:
							retVal = FitAndPlotFastFlyby(
										m_AllMeasurements, meaVals, processingValueCallback,
										g, xScale, yScale, m_RAImage.Width, m_RAImage.Height);

							break;

						default:
							throw new IndexOutOfRangeException();
					}
						
					#endregion
					
					m_EarliestDEFrame = retVal.EarliestFrame;
					m_LatestDEFrame = retVal.LatestFrame;

					if (!double.IsNaN(retVal.FittedValue))
					{
						lblAstRA.Text = string.Format("{0}", AstroConvert.ToStringValue(retVal.FittedValue / 15, "HH MM SS.TT"));
						lblAlpha.Visible = true;
						m_RADeg = retVal.FittedValue;
						m_MPCRAHours = m_RADeg / 15;
						m_MPCTime = retVal.FittedValueTime;
						m_MPCTimePrecission = TimeSpan.MinValue;
						m_MPCIsVideoNormalPosition = retVal.IsVideoNormalPosition;
					}

					CheckAndSetTimeAndMPCAdd();
				}

				g.DrawString("a", s_SymbolFont12, Brushes.Yellow, 5, 5);
				g.Save();
			}

			pnlRASeries.Image = m_RAImage;
			if (m_AstrometricState.MeasuringState != AstrometryInFramesState.RunningMeasurements)
				pnlRASeries.Refresh();

            return retVal;
		}

        private ProcessingReturnValues DrawDEPanel()
		{
            ProcessingReturnValues retVal = null;

			using (Graphics g = Graphics.FromImage(m_DEImage))
			{
				g.Clear(Color.Gray);

				int midY = m_DEImage.Height / 2;
				g.DrawLine(Pens.DarkGray, 5, midY, m_DEImage.Width - 5, midY);

				int minFrame = m_AllMeasurements.Keys.Min();
				int maxFrame = m_AllMeasurements.Keys.Max();
                int frames = 1 +  maxFrame - minFrame;

				if (m_AllMeasurements.Count > 0)
				{
                    float xScale = m_DEImage.Width * 1.0f / frames;
					float yScale = Math.Abs((m_DEImage.Height - 10) / (float)(m_MaxDE - m_MinDE));

					double from = m_MinDE - (6 / 3600.0);
					double to = m_MaxDE + (6 / 3600.0);
					double curr = Math.Truncate((from * 3600.0) / 5) * 5 / 3600.0;
					while (curr < to)
					{
						float y = (float)(curr - m_MinDE) * yScale + 5;
						g.DrawLine(s_SecondLinesPen, 5, y, m_DEImage.Width - 5, y);
						curr += (1 / 3600.0);
					}

					var meaVals = new MeasurementValues 
					{ 
						Context = m_MeasurementContext,
						UserMidValue = m_UserDEMid,
						UserMidFrame = m_UserFrame,
						MaxStdDev = m_MeasurementContext.MaxStdDev / 3600.0,
						MinValue = m_MinDE,
						IncludedPen = Pens.LimeGreen,
						ExcludedPen = Pens.Tomato,
						AveragePen = s_DEAveragePen,
                        FirstVideoFrame = m_VideoController.VideoFirstFrame,
						MinFrameNo = minFrame,
						MaxFrameNo = maxFrame
					};
		
					#region Compute the RA or DE value at the middle of the interval

					GetProcessingValueCallback processingValueCallback =
						delegate(SingleMultiFrameMeasurement measurement)
							{
								return new ProcessingValues
									    {
									       	StdDev = measurement.StdDevDEArcSec/3600.0,
									       	Value = measurement.DEDeg
									    };
							};

					switch (m_MeasurementContext.MovementExpectation)
					{
						case MovementExpectation.Slow:
							retVal = FitAndPlotSlowMotion(
								m_AllMeasurements, meaVals, processingValueCallback, 
								g, xScale, yScale, m_DEImage.Width);

							break;

						case MovementExpectation.SlowFlyby:
							retVal = FitAndPlotSlowFlyby(
								m_AllMeasurements, meaVals, processingValueCallback,
								g, xScale, yScale, m_DEImage.Width, m_RAImage.Height);

							break;

						case MovementExpectation.FastFlyby:
							retVal = FitAndPlotFastFlyby(
								m_AllMeasurements, meaVals, processingValueCallback,
								g, xScale, yScale, m_DEImage.Width, m_RAImage.Height);

							break;
						default:
							throw new IndexOutOfRangeException();
					}
						
					#endregion

					m_EarliestDEFrame = retVal.EarliestFrame;
					m_LatestDEFrame = retVal.LatestFrame;

					if (!double.IsNaN(retVal.FittedValue))
					{
						lblAstDE.Text = AstroConvert.ToStringValue(retVal.FittedValue, "+DD MM SS.T");
						lblDelta.Visible = true;
						m_DEDeg = retVal.FittedValue;
						m_MPCDE = m_DEDeg;
						m_MPCTime = retVal.FittedValueTime;
						m_MPCTimePrecission = TimeSpan.MinValue;
						m_MPCIsVideoNormalPosition = retVal.IsVideoNormalPosition;
					}

					CheckAndSetTimeAndMPCAdd();
				}

				g.DrawString("d", s_SymbolFont12, Brushes.Yellow, 5, 5);
				g.Save();
			}

			pnlDESeries.Image = m_DEImage;
			if (m_AstrometricState.MeasuringState != AstrometryInFramesState.RunningMeasurements)
				pnlDESeries.Refresh();

            return retVal;
		}

		#region RA/DE Computation

		private class ProcessingValues
		{
			public double Value;
			public double StdDev;
		}

		private class MeasurementValues
		{
			public MeasurementContext Context;
			public double MaxStdDev;
			public double UserMidValue;
			public int UserMidFrame;
			public double MinValue;
			public Pen IncludedPen;
			public Pen ExcludedPen;
			public Pen AveragePen;
		    public int FirstVideoFrame;
			public int MinFrameNo;
			public int MaxFrameNo;
		}

		private class ProcessingReturnValues
		{
			public int EarliestFrame;
			public int LatestFrame;
			public double FittedValue;
			public DateTime FittedValueTime;
			public bool IsVideoNormalPosition;
		    public double FittedNormalFrame;
		}

		private delegate ProcessingValues GetProcessingValueCallback(SingleMultiFrameMeasurement measurement);


		private static ProcessingReturnValues FitAndPlotSlowMotion(
			Dictionary<int, SingleMultiFrameMeasurement> measurements,
			MeasurementValues meaValues, 
			GetProcessingValueCallback getValueCallback, 
			Graphics g, float xScale, float yScale, int imageWidth)
		{
			// Compute median, use median based exclusion rules
			// Report the median position for the time at the middle of the measured interval 
			// Do not expect elongated image (no corrections from the exposure)
			// May apply instrumental delay corrections for the frame time

			var rv = new ProcessingReturnValues();

			double sum = 0;
			double userSum = 0;
			double stdDevUserSum = 0;
			int numFramesUser = 0;

			double userMidFrom = meaValues.UserMidValue - meaValues.MaxStdDev;
			double userMidTo = meaValues.UserMidValue + meaValues.MaxStdDev;

			rv.EarliestFrame = int.MaxValue;
			rv.LatestFrame = int.MinValue;

			List<double> medianList = new List<double>();
			foreach (SingleMultiFrameMeasurement measurement in measurements.Values)
			{
				float x = (measurement.FrameNo - meaValues.MinFrameNo) * xScale + 5;
				ProcessingValues val = getValueCallback(measurement);
				double valueFrom = val.Value - val.StdDev;
				double valueTo = val.Value + val.StdDev;

				float yFrom = (float)(valueFrom - meaValues.MinValue) * yScale + 5;
				float yTo = (float)(valueTo - meaValues.MinValue) * yScale + 5;

				sum += val.Value;

				Pen mPen = meaValues.IncludedPen;
				if (!double.IsNaN(meaValues.UserMidValue))
				{
					if ((valueFrom >= userMidFrom && valueFrom <= userMidTo) ||
						(valueTo >= userMidFrom && valueTo <= userMidTo))
					{
						numFramesUser++;
						userSum += val.Value;
						medianList.Add(val.Value);
						stdDevUserSum += val.StdDev * val.StdDev;
						if (rv.EarliestFrame > measurement.FrameNo) rv.EarliestFrame = measurement.FrameNo;
						if (rv.LatestFrame < measurement.FrameNo) rv.LatestFrame = measurement.FrameNo;
					}
					else
						mPen = meaValues.ExcludedPen;
				}


				g.DrawLine(mPen, x, yFrom, x, yTo);
				g.DrawLine(mPen, x - 1, yFrom, x + 1, yFrom);
				g.DrawLine(mPen, x - 1, yTo, x + 1, yTo);
			}

			if (!double.IsNaN(meaValues.UserMidValue) && numFramesUser > 0)
			{
				double average = userSum / numFramesUser;
				double err = Math.Sqrt(stdDevUserSum) / (numFramesUser - 1);
				float yAve = (float)(average - meaValues.MinValue) * yScale + 5;
				g.DrawLine(meaValues.AveragePen, 5, yAve - 1, imageWidth - 5, yAve - 1);
				g.DrawLine(meaValues.AveragePen, 5, yAve, imageWidth - 5, yAve);
				g.DrawLine(meaValues.AveragePen, 5, yAve + 1, imageWidth - 5, yAve + 1);

				float yMin = (float)(userMidFrom - meaValues.MinValue) * yScale + 5;
				float yMax = (float)(userMidTo - meaValues.MinValue) * yScale + 5;
				g.DrawLine(meaValues.AveragePen, 5, yMin, imageWidth - 5, yMin);
				g.DrawLine(meaValues.AveragePen, 5, yMax, imageWidth - 5, yMax);

				double median = 0;
				medianList.Sort();
				if (numFramesUser % 2 == 1)
					median = medianList[numFramesUser / 2];
				else
					median = (medianList[numFramesUser / 2] + medianList[(numFramesUser / 2) - 1]) / 2;

				Trace.WriteLine(string.Format("{0}; Included: {1}; Average: {2}; Median: {3}", 
					meaValues.UserMidValue.ToString("0.00000"),
					numFramesUser, AstroConvert.ToStringValue(average, "+HH MM SS.T"), 
					AstroConvert.ToStringValue(median, "+HH MM SS.T")));

				rv.FittedValue = median;
				rv.IsVideoNormalPosition = false;
			}
			else
			{
				double average = sum / measurements.Count;
				float yAve = (float)(average - meaValues.MinValue) * yScale + 5;
				g.DrawLine(Pens.WhiteSmoke, 5, yAve, imageWidth - 5, yAve);

				rv.FittedValue = double.NaN;
			}

			return rv;
		}

		private static Brush s_NormalTimeIntervalHighlightBrush = new SolidBrush(Color.FromArgb(143, 142, 112));

		private ProcessingReturnValues FitAndPlotSlowFlyby(
			Dictionary<int, SingleMultiFrameMeasurement> measurements,
			MeasurementValues meaValues,
			GetProcessingValueCallback getValueCallback,
			Graphics g, float xScale, float yScale, int imageWidth, int imageHight)
		{
			// Do linear regression, use residual based exclusion rules
			// Report the interpolated position at the middle of the measured interva
			// Don't forget to add the video normal position flag in the OBS file
			// Expect elongated images and apply instrumental delay corrections

			var rv = new ProcessingReturnValues();

			int numFramesUser = 0;

			rv.EarliestFrame = int.MaxValue;
			rv.LatestFrame = int.MinValue;

			Dictionary<int, List<double>> intervalValues = new Dictionary<int, List<double>>();
			Dictionary<double, double> intervalMedians = new Dictionary<double, double>();

			LinearRegression regression = null;
			if (measurements.Values.Count > 1)
			{
				rv.EarliestFrame = measurements.Values.Select(m => m.FrameNo).Min();
				rv.LatestFrame = measurements.Values.Select(m => m.FrameNo).Max();
				
				foreach (SingleMultiFrameMeasurement measurement in measurements.Values)
				{
					int integrationInterval = (measurement.FrameNo - rv.EarliestFrame) / meaValues.Context.IntegratedFramesCount;

					List<double> intPoints;
					if (!intervalValues.TryGetValue(integrationInterval, out intPoints))
					{
						intPoints = new List<double>();
						intervalValues.Add(integrationInterval, intPoints);
					}

					ProcessingValues val = getValueCallback(measurement);
					intPoints.Add(val.Value);
				}

				if (intervalValues.Count > 2)
				{
					regression = new LinearRegression();

					foreach(int integratedFrameNo in intervalValues.Keys)
					{
						List<double> data = intervalValues[integratedFrameNo];
						data.Sort();

						double median = data.Count%2 == 1
						        ? data[data.Count/2]
						        : (data[data.Count/2] + data[(data.Count/2) - 1])/2.0;

						// Assign the data point to the middle of the integration interval (using frame numbers)
						//
						// |--|--|--|--|--|--|--|--|
						// |           |           |
						//
						// Because the time associated with the first frame is the middle of the frame, but the 
						// time associated in the middle of the interval is the end of the field then the correction
						// is (N / 2) - 0.5 frames

						double dataPointFrameNo = 
							rv.EarliestFrame + 
							meaValues.Context.IntegratedFramesCount * integratedFrameNo 
							+ (meaValues.Context.IntegratedFramesCount / 2)
							- 0.5;

						intervalMedians.Add(dataPointFrameNo, median);
						regression.AddDataPoint(dataPointFrameNo, median);	
					}

					regression.Solve();

					var firstPos = measurements[rv.EarliestFrame];
					var lastPos = measurements[rv.LatestFrame];
					double distanceArcSec = AngleUtility.Elongation(firstPos.RADeg, firstPos.DEDeg, lastPos.RADeg, lastPos.DEDeg) * 3600;
					var firstTime = m_AstrometryController.GetTimeForFrame(meaValues.Context, rv.EarliestFrame, meaValues.FirstVideoFrame);
                    var lastTime = m_AstrometryController.GetTimeForFrame(meaValues.Context, rv.LatestFrame, meaValues.FirstVideoFrame);
					double elapsedSec = new TimeSpan(lastTime.UT.Ticks - firstTime.UT.Ticks).TotalSeconds;
					m_MotionRate = distanceArcSec / elapsedSec;
				}
			}

			AstrometryController.FrameTime resolvedTime = null;
			if (int.MinValue != meaValues.UserMidFrame)
			{
				// Find the closest video 'normal' MPC time and compute the frame number for it
				// Now compute the RA/DE for the computed 'normal' frame
                resolvedTime = m_AstrometryController.GetTimeForFrame(meaValues.Context, meaValues.UserMidFrame, meaValues.FirstVideoFrame);

				float xPosBeg = (float)(resolvedTime.ClosestNormalIntervalFirstFrameNo - rv.EarliestFrame) * xScale + 5;
				float xPosEnd = (float)(resolvedTime.ClosestNormalIntervalLastFrameNo - rv.EarliestFrame) * xScale + 5;

				g.FillRectangle(s_NormalTimeIntervalHighlightBrush, xPosBeg, 1, (xPosEnd - xPosBeg), imageHight - 2);
			}

			Dictionary<double, double> secondPassData = new Dictionary<double, double>();

		    int minFrameId = measurements.Keys.Min();

			foreach (SingleMultiFrameMeasurement measurement in measurements.Values)
			{
                float x = (measurement.FrameNo - minFrameId) * xScale + 5;

				ProcessingValues val = getValueCallback(measurement);
	
				double valueFrom = val.Value - val.StdDev;
				double valueTo = val.Value + val.StdDev;

				float yFrom = (float)(valueFrom - meaValues.MinValue) * yScale + 5;
				float yTo = (float)(valueTo - meaValues.MinValue) * yScale + 5;

				g.DrawLine(meaValues.IncludedPen, x, yFrom, x, yTo);
				g.DrawLine(meaValues.IncludedPen, x - 1, yFrom, x + 1, yFrom);
				g.DrawLine(meaValues.IncludedPen, x - 1, yTo, x + 1, yTo);
			}

			foreach(double integrFrameNo in intervalMedians.Keys)
			{
				double val = intervalMedians[integrFrameNo];

				double fittedValAtFrame = regression != null
					? regression.ComputeY(integrFrameNo)
					: double.NaN;

				bool included = Math.Abs(fittedValAtFrame - val) < 3 * regression.StdDev;

				if (meaValues.Context.IntegratedFramesCount > 1)
				{
					Pen mPen = included ? meaValues.IncludedPen : meaValues.ExcludedPen;

					float x = (float)(integrFrameNo - minFrameId) * xScale + 5;
					float y = (float)(val - meaValues.MinValue) * yScale + 5;

					g.DrawEllipse(mPen, x - 3, y - 3, 6, 6);
					g.DrawLine(mPen, x - 5, y - 5, x + 5, y + 5);
					g.DrawLine(mPen, x + 5, y - 5, x - 5, y + 5);					
				}

				if (included) secondPassData.Add(integrFrameNo, val);
			}

			#region Second Pass
			regression = null;
			if (secondPassData.Count > 2)
			{
				regression = new LinearRegression();
				foreach (double frameNo in secondPassData.Keys)
				{
					regression.AddDataPoint(frameNo, secondPassData[frameNo]);
				}
				regression.Solve();
			}
			#endregion

			if (regression != null)
			{
                double leftFittedVal = regression.ComputeY(rv.EarliestFrame);
                double rightFittedVal = regression.ComputeY(rv.LatestFrame);

                double err = 3 * regression.StdDev;

                float leftAve = (float)(leftFittedVal - meaValues.MinValue) * yScale + 5;
                float rightAve = (float)(rightFittedVal - meaValues.MinValue) * yScale + 5;
                float leftX = 5 + (float)(rv.EarliestFrame - rv.EarliestFrame) * xScale;
                float rightX = 5 + (float)(rv.LatestFrame - rv.EarliestFrame) * xScale;

                g.DrawLine(meaValues.AveragePen, leftX, leftAve - 1, rightX, rightAve - 1);
                g.DrawLine(meaValues.AveragePen, leftX, leftAve, rightX, rightAve);
                g.DrawLine(meaValues.AveragePen, leftX, leftAve + 1, rightX, rightAve + 1);                    

				float leftMin = (float)(leftFittedVal - err - meaValues.MinValue) * yScale + 5;
				float leftMax = (float)(leftFittedVal + err - meaValues.MinValue) * yScale + 5;
				float rightMin = (float)(rightFittedVal - err - meaValues.MinValue) * yScale + 5;
				float rightMax = (float)(rightFittedVal + err - meaValues.MinValue) * yScale + 5;

                g.DrawLine(meaValues.AveragePen, leftX, leftMin, rightX, rightMin);
                g.DrawLine(meaValues.AveragePen, leftX, leftMax, rightX, rightMax);

				if (int.MinValue != meaValues.UserMidFrame && 
					resolvedTime != null)
				{
					// Find the closest video 'normal' MPC time and compute the frame number for it
					// Now compute the RA/DE for the computed 'normal' frame
					
					double fittedValueAtMiddleFrame = regression.ComputeY(resolvedTime.ClosestNormalFrameNo);

					Trace.WriteLine(string.Format("{0}; Included: {1}; Normal Frame No: {2}; Fitted Val: {3}",
						meaValues.UserMidValue.ToString("0.00000"),
						numFramesUser, resolvedTime.ClosestNormalFrameNo,
						AstroConvert.ToStringValue(fittedValueAtMiddleFrame, "+HH MM SS.T")));

					// Report the interpolated position at the middle of the measured interval
					// Don't forget to add the video normal position flag in the OBS file
					// Expect elongated images and apply instrumental delay corrections

					rv.FittedValue = fittedValueAtMiddleFrame;
					rv.FittedValueTime = resolvedTime.ClosestNormalFrameTime;
					rv.IsVideoNormalPosition = true;
				    rv.FittedNormalFrame = resolvedTime.ClosestNormalFrameNo;

					// Plot the frame
					float xPos = (float)(resolvedTime.ClosestNormalFrameNo- rv.EarliestFrame) * xScale + 5;
					float yPos = (float)(rv.FittedValue - meaValues.MinValue) * yScale + 5; 
					g.DrawLine(Pens.Yellow, xPos, 1, xPos, imageHight - 2);
					g.FillEllipse(Brushes.Yellow, xPos - 3, yPos - 3, 6, 6);
					
				}
				else
					rv.FittedValue = double.NaN;
			}
			else
				rv.FittedValue = double.NaN;

			return rv;
		}

		private ProcessingReturnValues FitAndPlotFastFlyby(Dictionary<int, SingleMultiFrameMeasurement> measurements,
			MeasurementValues meaValues,
			GetProcessingValueCallback getValueCallback,
			Graphics g, float xScale, float yScale, int imageWidth, int imageHeight)
		{
			// Do linear regression, use residual based exclusion rules
			// Two possible modes: (A) non trailed and (B) trailed images
			// A) Non Trailed Images
			// Report interpolated times for video normal position [How to use the MPC page for this?]
			// Don't forget to add the video normal position flag in the OBS file
			// Expect elongated images and apply instrumental delay corrections (in both integrated and non integrated modes)
			// B) Trailed Images
			// TODO: R&D Required

			if (meaValues.Context.ObjectExposureQuality == ObjectExposureQuality.GoodSignal)
			{
				return FitAndPlotSlowFlyby(measurements, meaValues, getValueCallback, g, xScale, yScale, imageWidth, imageHeight);
			}
			else
			{
				MessageBox.Show("This operation is currently not suppored.");
				return new ProcessingReturnValues();
			}
		}
		#endregion

		private void DisplayMeaMag()
        {
            lblM.Visible = false;
            lblMeaMag.Visible = false;
            m_MPCMag = double.NaN;

            if (m_AllMeasurements.Count > 2)
            {
                List<double> allMags = m_AllMeasurements.Select(m => m.Value.Mag).Where(m => !double.IsNaN(m)).ToList();
                allMags.Sort();

                if (allMags.Count > 1)
                {
                    m_MPCMag = allMags[allMags.Count/2];

                    m_MPCMagBand = MagnitudeBand.Cousins_R;

                    if (m_MeasurementContext != null)
                    {
                        switch (m_MeasurementContext.PhotometryMagOutputBand)
                        {
                            case TangraConfig.MagOutputBand.CousinsR:
                                m_MPCMagBand = MagnitudeBand.Cousins_R;
                                break;

                            case TangraConfig.MagOutputBand.JohnsonV:
                                m_MPCMagBand = MagnitudeBand.Johnson_V;
                                break;
                        }
                    }
                }

                if (!double.IsNaN(m_MPCMag))
                {
                    m_MPCMag = m_MeasurementContext.StarCatalogueFacade.ConvertMagnitude(m_MPCMag, m_MeasurementContext.AssumedTargetVRColour, m_MeasurementContext.PhotometryCatalogBandId, m_MeasurementContext.PhotometryMagOutputBand);
                    string bandStr = "";
                    switch(m_MeasurementContext.PhotometryMagOutputBand)
                    {
                        case TangraConfig.MagOutputBand.CousinsR:
                            bandStr = " (R)";
                            break;

                        case TangraConfig.MagOutputBand.JohnsonV:
                            bandStr = " (V)";
                            break;
                    }
                    lblMeaMag.Text = m_MPCMag.ToString("0.0") + bandStr;
                    lblMeaMag.Visible = true;
                    lblM.Visible = true;                    
                }
            }            
        }

		private void pnlRAChart_MouseClick(object sender, MouseEventArgs e)
		{
			float yScale = (pnlRASeries.Height - 10) / (float)(m_MaxRA - m_MinRA);
			m_UserRAMid = m_MinRA + (e.Y - 5) / yScale;

			float xScale = (pnlRASeries.Width) / (float)(m_MaxFrame - m_MinFrame);
			m_UserFrame = (int)Math.Round(m_MinFrame + (e.X - 5) / xScale);
			Trace.WriteLine(string.Format("Clicked: x = {0}; FrameNo = {1}", e.X, m_UserFrame));

			if (m_AstrometricState.MeasuringState != AstrometryInFramesState.RunningMeasurements)
			{
				DrawRAPanel();

                if (m_MeasurementContext.MovementExpectation != MovementExpectation.Slow)
                {
                    ProcessingReturnValues retVal = DrawDEPanel();

                    if (e.Button == MouseButtons.Middle &&
                        retVal != null && retVal.IsVideoNormalPosition)
                    {
						m_VideoController.MoveToFrame((int)Math.Round(retVal.FittedNormalFrame));
                    }
                }
			}
		}

		private void pnlDEChart_MouseClick(object sender, MouseEventArgs e)
		{
			float yScale = (pnlDESeries.Height - 10) / (float)(m_MaxDE - m_MinDE);
			m_UserDEMid = m_MinDE + (e.Y - 5) / yScale;

			float xScale = (pnlDESeries.Width) / (float)(m_MaxFrame - m_MinFrame);
			m_UserFrame = (int)Math.Round(m_MinFrame + (e.X - 5) / xScale);
			Trace.WriteLine(string.Format("Clicked: x = {0}; FrameNo = {1}", e.X, m_UserFrame));

			if (m_AstrometricState.MeasuringState != AstrometryInFramesState.RunningMeasurements)
			{
				DrawDEPanel();

                if (m_MeasurementContext.MovementExpectation != MovementExpectation.Slow)
                {
                    ProcessingReturnValues retVal = DrawRAPanel();

                    if (e.Button == MouseButtons.Middle &&
                        retVal != null && retVal.IsVideoNormalPosition)
                    {
						m_VideoController.MoveToFrame((int)Math.Round(retVal.FittedNormalFrame));
                    }
                }
			}
		}

		private void CheckAndSetTimeAndMPCAdd()
		{
			if (!double.IsNaN(m_MPCDE) && !double.IsNaN(m_MPCRAHours))
			{
			    int precision = 1000000;
			    string format = "00.000000";
				// TODO: Different ways of getting the time based on the different expected motion type?

			    DisplayMeaMag();

				switch (m_MeasurementContext.MovementExpectation)
				{
					case MovementExpectation.Slow:
						{
							int earliestFrame = Math.Min(m_EarliestRAFrame, m_EarliestDEFrame);
							int latestFrame = Math.Max(m_LatestRAFrame, m_LatestDEFrame);
							m_MPCTime =
								m_MeasurementContext.FirstFrameUtcTime.AddSeconds(
									((earliestFrame + latestFrame)/2 - m_MeasurementContext.FirstFrameId)/m_MeasurementContext.FrameRate);

							m_MPCTimePrecission = TimeSpan.FromSeconds((latestFrame - earliestFrame)/m_MeasurementContext.FrameRate);
						    precision = 100000;
                            format = "00.00000";
						}
						break;

					case MovementExpectation.SlowFlyby:
						{
							// Don't forget to add the video normal position flag in the OBS file
							// Expect elongated images and apply instrumental delay corrections

							lblRate.Visible = true;
							lblRateVal.Visible = true;
							lblRateVal.Text = string.Format("{0:F2}\"/s", m_MotionRate);

                            precision = 1000000;
                            format = "00.000000";
						}
						break;

					case MovementExpectation.FastFlyby:
						{
							// Don't forget to add the video normal position flag in the OBS file
							// Expect elongated images and apply instrumental delay corrections

							lblRate.Visible = true;
							lblRateVal.Visible = true;
							lblRateVal.Text = string.Format("{0:F2}\"/s", m_MotionRate);

                            precision = 1000000;
                            format = "00.000000";
						}
						break;

					default:
						throw new IndexOutOfRangeException();
				}

				bool isNormalTime = m_MPCTimePrecission == TimeSpan.MinValue;
                decimal roundedError = isNormalTime ? 0 : (decimal)m_MPCTimePrecission.TotalDays;
                double roundedTime = (m_MPCTime.Hour + m_MPCTime.Minute / 60.0 + (m_MPCTime.Second + (m_MPCTime.Millisecond / 1000.0)) / 3600.0) / 24;

                m_FolderName = m_MPCTime.ToString("yyyy-MM-") + (m_MPCTime.Day + roundedTime).ToString();

                int power = 1;
				if (!isNormalTime)
				{
                    while (roundedError < 1 && roundedError != 0)
					{
						roundedError = roundedError * 10;
						power *= 10;
					}

                    if (power != 0)
					    roundedError = Math.Truncate(Math.Round(roundedError)) / power;					
				}

                if (precision != 0)
                    roundedTime = Math.Truncate(Math.Round(roundedTime * precision)) / precision;

                lblTimeValue.Text = string.Format("{0} {1} {2}", m_MPCTime.ToString("yyyy MM"),
                    (m_MPCTime.Day + roundedTime).ToString(format), isNormalTime ? "(normal)" : "");

				lblTimeValue.Visible = true;
				lblAverageTime.Visible = true;

				btnAddToMCPReport.Enabled = true;				
			}
		}


		private double m_RADeg;
		private double m_DEDeg;
		private DateTime m_MPCTime;
	    private TimeSpan m_MPCTimePrecission;
        private double m_MPCRAHours;
        private double m_MPCDE;
	    private double m_MPCMag;
	    private MagnitudeBand m_MPCMagBand;
		private bool m_MPCIsVideoNormalPosition;
	    private string m_MPCObjectDesignation;
		private double m_MotionRate;

		private string m_FolderName;

		private void btnIdentify_Click(object sender, EventArgs e)
		{
			if (OnIdentifyObjectsCommand != null)
				OnIdentifyObjectsCommand(this, EventArgs.Empty);
		}

		private MPCReportFile m_CurrentReportFile;

		private void btnAddToMCPReport_Click(object sender, EventArgs e)
		{
			// Do we have a resolved object with the approximate coordinates of the user object?
			string objectDesignation = null;
			foreach (IIdentifiedObject knownObject in m_AstrometricState.IdentifiedObjects)
			{
				double distInArcSec = 3600.0 * AngleUtility.Elongation(m_RADeg, m_DEDeg, knownObject.RAHours * 15, knownObject.DEDeg);
				if (distInArcSec <= 6)
				{
					// If so then see if we can get an MPC designation from it's name
					objectDesignation = MPCObsLine.GetObjectCode(knownObject.ObjectName);
					if (objectDesignation != null)
						break;
				}

			}

			if (objectDesignation == null || objectDesignation.Length != 13)
			{
				// If not then show the frmChooseMPCObject form
				frmChooseMPCObject frmChooseMpcObject = new frmChooseMPCObject();

				//  If we have an existing report opened and there is a previous report (done this time)
				//  then default the object designation to the designation of the last object
				if (m_CurrentReportFile != null &&
					!string.IsNullOrEmpty(m_CurrentReportFile.LastObjectDesignation))
				{
					frmChooseMpcObject.tbxLine.Text = m_CurrentReportFile.LastObjectDesignation;
				}

				if (frmChooseMpcObject.ShowDialog(ParentForm) != DialogResult.OK)
					return;

                // TODO: Check the designation is valid before the frmChooseMPCObject form returns 
				objectDesignation = frmChooseMpcObject.tbxLine.Text;
			}

		    m_MPCObjectDesignation = objectDesignation;

		    SaveToReportFile();
		}

        private void SaveToReportFile()
        {
            if (m_CurrentReportFile == null)
            {
                // Is there a report form currently opened? 
                // If no then ask the user to append to an existing report or create a new one
                frmChooseReportFile reportFileForm = new frmChooseReportFile();
                if (reportFileForm.ShowDialog(this.ParentForm) != DialogResult.OK)
                    return;

                if (reportFileForm.IsNewReport)
                {
                    frmMPCObserver frmObserver = new frmMPCObserver(frmMPCObserver.MPCHeaderSettingsMode.NewMPCReport);
                    if (frmObserver.ShowDialog(ParentForm) == DialogResult.Cancel)
                        return;

                    if (saveFileDialog.ShowDialog(ParentForm) != DialogResult.OK)
                        return;

                    MPCObsHeader header = frmObserver.Header;
                    header.NET = m_MeasurementContext.StarCatalogueFacade.CatalogNETCode;
                    m_CurrentReportFile = new MPCReportFile(saveFileDialog.FileName, header);

					TangraConfig.Settings.RecentFiles.NewRecentFile(RecentFileType.MPCReport, saveFileDialog.FileName);
					TangraConfig.Settings.Save();
                }
                else
                {
                    m_CurrentReportFile = new MPCReportFile(reportFileForm.ReportFileName);

                    if (m_CurrentReportFile.Header.NET != m_MeasurementContext.StarCatalogueFacade.CatalogNETCode)
                    {
                        MessageBox.Show(
                            string.Format("The selected observation file uses {0} rather than {1}. Pelase select a different observation file or change the used catalog.",
                            m_CurrentReportFile.Header.NET, m_MeasurementContext.StarCatalogueFacade.CatalogNETCode), "Error", MessageBoxButtons.OK,  MessageBoxIcon.Error);

                        m_CurrentReportFile = null;
                        return;
                    }
                }
            }

            if (m_CurrentReportFile != null)
            {
                // Append the observation to the form
                if (!m_CurrentReportFile.AddObservation(
					m_MPCObjectDesignation, m_MPCRAHours, m_MPCDE, m_MPCTime, m_MPCTimePrecission, m_MPCMag, m_MPCMagBand, m_MPCIsVideoNormalPosition))
                    MessageBox.Show("Observation already added", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    m_CurrentReportFile.Save();

                m_CurrentReportFile.Present(this);

	            AstrometryContext.Current.CurrentReportFile = m_CurrentReportFile;
            }
        }

        public void CloseReportFile()
        {
            m_CurrentReportFile = null;
	        AstrometryContext.Current.CurrentReportFile = null;
        }

		private void btnShowMesaurements_Click(object sender, EventArgs e)
		{
			ShowMeasurementsView();
		}

		private void btnShowSelectedObject_Click(object sender, EventArgs e)
		{
			ShowSelectedObjectView();
		}

		internal void ShowMeasurementsView()
		{
			// Load the measurements stuff
			SwitchToAstrometryControls(true);

			pnlMeasurements.Visible = true;
			pnlObject.Visible = false;
			pnlAstrometry.Visible = false;
			pnlMeasurements.Top = 0;
			pnlMeasurements.Left = 0;

			btnShowMesaurements.BackColor = SystemColors.Window;
			btnShowSelectedObject.BackColor = SystemColors.Control;
		}

		private void ShowSelectedObjectView()
		{
			// Load the normal stuff
			SwitchToStandardControls();

			if (m_JustAfterNewFit)
			{
				pnlAstrometry.Visible = true;
				pnlObject.Visible = false;
				pnlMeasurements.Visible = false;
				pnlAstrometry.Top = 0;
				pnlAstrometry.Left = 0;
			}
			else
			{
				pnlObject.Visible = true;
				pnlAstrometry.Visible = false;
				pnlMeasurements.Visible = false;
				pnlObject.Top = 0;
				pnlObject.Left = 0;
			}

			btnShowMesaurements.BackColor = SystemColors.Control;
			btnShowSelectedObject.BackColor = SystemColors.Window;
			btnFramesAstrometry.Visible = !m_AstrometricState.ObjectToMeasureSelected;
		}

		private void btnSendErrorFit_Click(object sender, EventArgs e)
		{
            if (OnSendErrorReport != null)
            {
                btnSendErrorFit.Enabled = false;
                Cursor = Cursors.WaitCursor;
                try
                {
                    OnSendErrorReport(sender, e);
                }
                finally
                {
                    btnSendErrorFit.Enabled = true;
                    Cursor = Cursors.Default;
                }
            }
		}

		private void btnDifferentFieldCenter_Click(object sender, EventArgs e)
		{
            if (OnDifferentFieldCenter != null)
                OnDifferentFieldCenter(sender, e);
        }

		private void btnResolveObjects_Click(object sender, EventArgs e)
		{
			if (OnResolveObject != null)
				OnResolveObject(sender, e);
		}

        private void linkCommonIssues1_Click(object sender, EventArgs e)
        {
			ShellHelper.OpenUrl("http://www.hristopavlov.net/Tangra/CommonPlateSolveIssues");
        }

        private void linkCommonIssues2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
			ShellHelper.OpenUrl("http://www.hristopavlov.net/Tangra/CommonPlateSolveIssues");
        }

		private void btnSaveUnitTestData_Click(object sender, EventArgs e)
		{
			if (OnSaveUnitTestData != null)
			{
				if (saveUnitTestDataFileDialog.ShowDialog(this.ParentForm) == DialogResult.OK)
				{
					OnSaveUnitTestData(this, new SaveUnitTestDataEventArgs(saveUnitTestDataFileDialog.FileName));	
				}				
			}
		}

		private void btnManuallyIdentifyStar_Click(object sender, EventArgs e)
		{
			m_AstrometryController.SetManuallyIdentifyStarState(true);
		}

		private void btnManuallyIdentifyStar2_Click(object sender, EventArgs e)
		{
			m_AstrometryController.SetManuallyIdentifyStarState(true);
		}

		internal void SettManuallyIdentifyStarButtonPressed(bool pressed)
		{
			btnManuallyIdentifyStar.Enabled = !pressed;
			btnManuallyIdentifyStar2.Enabled = !pressed;
		}
	}
}
