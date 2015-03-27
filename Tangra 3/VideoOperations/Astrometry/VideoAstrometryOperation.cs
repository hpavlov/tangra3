/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Addins;
using Tangra.AstroServices;
using Tangra.Astrometry;
using Tangra.Astrometry.Analysis;
using Tangra.Astrometry.Recognition;
using Tangra.Controller;
using Tangra.Helpers;
using Tangra.ImageTools;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Model.Context;
using Tangra.Model.Helpers;
using Tangra.Model.Image;
using Tangra.Model.Video;
using Tangra.Model.VideoOperations;
using Tangra.SDK;
using Tangra.StarCatalogues;
using Tangra.VideoOperations.Astrometry.Engine;
using Tangra.PInvoke;

namespace Tangra.VideoOperations.Astrometry
{
	public class ManualFitDisplaySettings
	{
		public int LimitingMagnitude;
		public int MinMagnitude;
		public int MaxMagnitude;
		public int Rotation;
		public bool ShowGrid;
		public bool ShowStarLabels;
		public bool ShowStarMagnitudes;
	}

	public class VideoAstrometryOperation : VideoOperationBase, IVideoOperation, IRequiresAddinsController, IAstrometryProvider
	{
		private enum DisplayedView
		{
			None,
			AstrometricFit,
			ManualFit
		}

		private VideoController m_VideoController;
		private AstrometryController m_AstrometryController;
		private AddinsController m_AddinsController;
		private AstrometricState m_AstrometricState;

		private Panel m_ControlPanelHolder = null;
		private ucAstrometryObjectInfo m_ViewControl;

		private DisplayedView m_CurrentView = DisplayedView.None;

		private int m_CurrentFrameIndex;
		private UserObjectContext m_UserObject;
		private MeasurementContext m_MeasurementContext;

		private bool m_DebugMode;

		private List<ITangraAddinAction> m_AstrometryAddinActions = new List<ITangraAddinAction>();
		private List<ITangraAddin> m_AstrometryAddins = new List<ITangraAddin>();

		private FieldSolveContext m_Context;
		private AstroPlate m_Image;
		private List<IStar> m_CatalogueStars;

		private Pen referenceStarPen;
		private Pen rejectedStarPen;
		private Pen unrecognizedStarPen;
		private Brush catalogBrushReference;
		private Brush catalogBrushRejected;
		private Brush catalogBrushUnrecognized;

		private Pen identifiedObjectPen;
		private Brush identifiedObjectBrush;

		private bool m_VeryFirstTimeDrawn;
		private IStarMap m_StarMap;

		private LeastSquareFittedAstrometry m_AstrometricFit;
		private StarMagnitudeFit m_PhotometricFit;
		private DistanceBasedAstrometrySolver m_DistBasedMatcher;
		private PerformMatchResult m_CurrentMatchResult;

		private Dictionary<PSFFit, double> m_UidentifiedObjects = null;
		private Dictionary<PSFFit, IStar> m_IdentifiedObjects = null;
		private Dictionary<PSFFit, double> m_UnknownObjects = null;

		private float m_empericalPSFR0 = float.NaN;
		private AstroImage m_AstroImage;

		private bool m_ShowStarLabels = false;
		private bool m_ShowObjectLabels = true;
		private bool m_ShowStarMagnitudes = false;

		private static Font s_StarInfoFont = new Font(FontFamily.GenericSerif, 7);
		private static Font s_UserObjectFont = new Font(FontFamily.GenericSerif, 10);

		public VideoAstrometryOperation()
		{ }

		public VideoAstrometryOperation(AstrometryController astrometryController, bool debugMode)
		{
			m_AstrometricState = AstrometricState.EnsureAstrometricState();
			m_AstrometryController = astrometryController;
			m_AstrometryController.SetOperation(this);

			m_DebugMode = debugMode;
		}

		public bool InitializeOperation(IVideoController videoController, Panel controlPanel, IFramePlayer framePlayer, Form topForm)
		{
			m_VideoController = videoController as VideoController; // Hack
			
			m_ControlPanelHolder = controlPanel;

			m_AstrometricState.Measurements.Clear();
			m_AstrometricState.MeasuringState = AstrometryInFramesState.AttemptingInitialFit;

			EnsureSwitchedToAstormetricFitView(controlPanel);

			if (PrepareForAstrometry())
			{
				videoController.RefreshCurrentFrame();
				return true;
			}

			return false;
		}

		public void SetAddinsController(AddinsController addinsController)
		{
			m_AddinsController = addinsController;
		}

		private bool PrepareForAstrometry()
		{
			m_Image = m_AstrometryController.GetCurrentAstroPlate();

			frmConfigureAstrometricFit frmFieldSolve = new frmConfigureAstrometricFit(m_VideoController, m_Image);
			if (m_VideoController.ShowDialog(frmFieldSolve) == DialogResult.OK)
			{
				m_Context = frmFieldSolve.Context;
				m_CatalogueStars = m_Context.CatalogueStars;
				m_ViewControl.m_CatalogueStars = m_CatalogueStars;
				m_empericalPSFR0 = float.NaN;

				AstrometryContext.Current.FieldSolveContext = m_Context;

				if (referenceStarPen != null) referenceStarPen.Dispose();
				if (rejectedStarPen != null) rejectedStarPen.Dispose();
				if (unrecognizedStarPen != null) unrecognizedStarPen.Dispose();
				referenceStarPen = new Pen(TangraConfig.Settings.Astrometry.Colors.ReferenceStar);
				rejectedStarPen = new Pen(TangraConfig.Settings.Astrometry.Colors.RejectedReferenceStar);
				unrecognizedStarPen = new Pen(TangraConfig.Settings.Astrometry.Colors.UndetectedReferenceStar);

				if (catalogBrushReference != null) catalogBrushReference.Dispose();
				catalogBrushReference = new SolidBrush(TangraConfig.Settings.Astrometry.Colors.ReferenceStar);
				if (catalogBrushRejected != null) catalogBrushRejected.Dispose();
				catalogBrushRejected = new SolidBrush(TangraConfig.Settings.Astrometry.Colors.RejectedReferenceStar);
				if (catalogBrushUnrecognized != null) catalogBrushUnrecognized.Dispose();
				catalogBrushUnrecognized = new SolidBrush(TangraConfig.Settings.Astrometry.Colors.UndetectedReferenceStar);

				if (identifiedObjectPen != null) identifiedObjectPen.Dispose();
				if (identifiedObjectBrush != null) identifiedObjectBrush.Dispose();
				identifiedObjectPen = new Pen(TangraConfig.Settings.Astrometry.Colors.UserObject);
				identifiedObjectBrush = new SolidBrush(TangraConfig.Settings.Astrometry.Colors.UserObject);

				m_AstrometricState.Reset();
				m_AstrometricState.MeasuringState = AstrometryInFramesState.AttemptingInitialFit;


				if (m_Context.FoundObject != null)
					m_AstrometricState.IdentifiedObjects.Add(m_Context.FoundObject);

				return true;
			}
			else
				return false;
		}

		internal void StartAstrometricMeasurements(ucAstrometryObjectInfo.StartAstrometricMeasurementsEventArgs e)
		{
			m_MeasurementContext = e.Context;
			m_ViewControl.m_MeasurementContext = m_MeasurementContext;
			m_UserObject = e.Context.ObjectToMeasure;

			m_MeasurementContext.FirstFrameId = m_CurrentFrameIndex;
			m_MeasurementContext.FrameRate = m_VideoController.VideoFrameRate;

			m_AstrometricState.ObjectToMeasure = m_UserObject;
			m_AstrometricState.ObjectToMeasureSelected = true;

			m_AstrometricState.Measurements.Clear();
			m_AstrometricState.MeasuringState = AstrometryInFramesState.RunningMeasurements;
			m_ViewControl.FrameMeasurementStarted();

            if (m_AstrometryAddins.Count > 0)
            {
                m_AddinsController.SetAstrometryProvider(this);

                foreach (ITangraAddin addin in m_AstrometryAddins)
                    addin.OnEventNotification(AddinFiredEventType.BeginMultiFrameAstrometry);
            }

			m_VideoController.PlayVideo(null, (uint)m_MeasurementContext.FrameInterval);
		}

		private void EnsureSwitchedToAstormetricFitView(Panel controlPanel)
		{
			if (m_ViewControl == null)
			{
				m_ViewControl = new ucAstrometryObjectInfo(m_AstrometryController, m_VideoController);

				m_ViewControl.OnStopMeasurementCommand += new EventHandler(m_ViewControl_OnStopMeasurementCommand);
				m_ViewControl.OnIdentifyObjectsCommand += new EventHandler(m_ViewControl_OnIdentifyObjectsCommand);
				m_ViewControl.OnStartMeasurementCommand += new EventHandler<ucAstrometryObjectInfo.StartAstrometricMeasurementsEventArgs>(m_ViewControl_OnStartMeasurementCommand);
				m_ViewControl.OnSendErrorReport += new EventHandler(m_ViewControl_OnSendErrorReport);
				m_ViewControl.OnSaveUnitTestData += new EventHandler<ucAstrometryObjectInfo.SaveUnitTestDataEventArgs>(m_ViewControl_OnSaveUnitTestData);
				m_ViewControl.OnDifferentFieldCenter += new EventHandler(m_ViewControl_OnDifferentFieldCenter);
				m_ViewControl.OnResolveObject += new EventHandler(m_ViewControl_OnResolveObject);
			}

			controlPanel.Controls.Clear();
			controlPanel.Controls.Add(m_ViewControl);
			m_CurrentView = DisplayedView.AstrometricFit;

			m_ViewControl.InitializeNewAstrometry();
		}

		void m_ViewControl_OnStartMeasurementCommand(object sender, ucAstrometryObjectInfo.StartAstrometricMeasurementsEventArgs e)
		{
			m_UserObject = e.Context.ObjectToMeasure;

			frmRunMultiFrameMeasurements frm = new frmRunMultiFrameMeasurements(m_VideoController, m_AddinsController, this, e.Context, m_Context, out m_AstrometryAddinActions, out m_AstrometryAddins);
			m_VideoController.ShowDialog(frm);
		}

		void m_ViewControl_OnDifferentFieldCenter(object sender, EventArgs e)
		{
			if (PrepareForAstrometry())
			{
				m_VideoController.RefreshCurrentFrame();
			}
			else
			{
				m_AstrometricState.MeasuringState = AstrometryInFramesState.Aborting;
				m_AstrometryController.SendNotification(new OperationNotifications(NotificationType.AbortCurrentOprtation, null));
			}
		}

		void m_ViewControl_OnResolveObject(object sender, EventArgs e)
		{
			if (m_AstrometricFit != null)
			{
				PlateObjectResolver objectResolver = new PlateObjectResolver(
					m_AstrometryController,
					m_VideoController,
					m_VideoController.GetCurrentAstroImage(false),
					m_AstrometricFit,
					m_Context.CatalogueStars,
					m_Context.LimitMagn + 1.5);

				m_VideoController.SetCursor(Cursors.WaitCursor);
				try
				{
					objectResolver.ResolveObjects(
							TangraConfig.PhotometryReductionMethod.OptimalExtraction,
							TangraConfig.PsfQuadrature.NumericalInAperture,
							TangraConfig.PsfFittingMethod.DirectNonLinearFit,
							TangraConfig.BackgroundMethod.BackgroundMode,
							TangraConfig.PreProcessingFilter.NoFilter,
							TangraConfig.Settings.StarCatalogue.CatalogMagnitudeBandId,
							AstrometryContext.Current.OSDRectToExclude,
							AstrometryContext.Current.RectToInclude,
							AstrometryContext.Current.LimitByInclusion,
							TangraConfig.Settings.Astrometry,
							ObjectResolverSettings.Default);
				}
				finally
				{
					m_VideoController.SetCursor(Cursors.Default);
				}

				m_UidentifiedObjects = objectResolver.UidentifiedObjects;
				m_IdentifiedObjects = objectResolver.IdentifiedObjects;
				m_UnknownObjects = objectResolver.UnknownObjects;

				m_VideoController.RedrawCurrentFrame(false, true);
			}
		}
		void m_ViewControl_OnSaveUnitTestData(object sender, ucAstrometryObjectInfo.SaveUnitTestDataEventArgs e)
		{
#if GET_UNIT_TEST_DATA
			string tempDir = Path.GetTempPath() + "\\" + Guid.NewGuid().ToString();
			Directory.CreateDirectory(tempDir);

			SaveFailedFitData(m_Image, m_StarMap, m_Context, m_CatalogueStars, TangraConfig.Settings.Astrometry, tempDir);

			ZipUnZip.Zip(tempDir, e.OutputFileName, false);

			Directory.Delete(tempDir, true);
#endif
		}

		void m_ViewControl_OnSendErrorReport(object sender, EventArgs e)
		{
			MessageBox.Show("This hasn't been implemented yet.");
		}

		void m_ViewControl_OnIdentifyObjectsCommand(object sender, EventArgs e)
		{
			if (AstrometryContext.Current.CurrentAstrometricFit != null)
			{
				if (m_VideoController.IsRunning)
					m_VideoController.StopVideo();

				TangraContext.Current.HasVideoLoaded = false;
				m_VideoController.UpdateViews();
				try
				{
					double maxFieldInAcrmin = m_AstrometryController.GetCurrentAstroPlate().GetMaxFOVInArcSec() / 60.0;

					frmIdentifyObjects frm = new frmIdentifyObjects(
						AstrometryContext.Current.CurrentAstrometricFit,
						maxFieldInAcrmin / 2.0,
						AstrometryContext.Current.FieldSolveContext.UtcTime,
						AstrometryContext.Current.FieldSolveContext.LimitMagn + 0.5,
						AstrometryContext.Current.FieldSolveContext.ObsCode);

					if (m_VideoController.ShowDialog(frm) == DialogResult.OK)
					{
						m_AstrometricState.Reset();
						if (frm.IdentifiedObjects != null)
						{
							foreach (IIdentifiedObject obj in frm.IdentifiedObjects)
								m_AstrometricState.IdentifiedObjects.Add(obj);
						}
					}
				}
				finally
				{
					TangraContext.Current.HasVideoLoaded = true;
					TangraContext.Current.CanPlayVideo = false;
					m_VideoController.UpdateViews();

					m_VideoController.RefreshCurrentFrame();
				}
			}
		}

		void m_ViewControl_OnStopMeasurementCommand(object sender, EventArgs e)
		{
			m_AstrometricState.MeasuringState = AstrometryInFramesState.Ready;

			m_VideoController.StopVideo();

			m_ViewControl.FrameMeasurementFinished();

			foreach (ITangraAddin addin in m_AstrometryAddins)
				addin.OnEventNotification(AddinFiredEventType.EndMultiFrameAstrometry);
		}

		public void FinalizeOperation()
		{
			m_VideoController.ClearZoomedImage();
			m_VideoController.ClearControlPanel();
		}

		public void PlayerStarted()
		{ }

		private void EnsureDistBasedMatcher()
		{
			if (m_DistBasedMatcher == null)
			{
				m_DistBasedMatcher = new DistanceBasedAstrometrySolver(m_AstrometryController, m_Image, TangraConfig.Settings.Astrometry, m_CatalogueStars, m_Context.DetermineAutoLimitMagnitude);

				m_DistBasedMatcher.SetMinMaxMagOfStarsForAstrometry(m_Context.PyramidMinMag, m_Context.LimitMagn);
				m_DistBasedMatcher.SetMinMaxMagOfStarsForPyramidAlignment(m_Context.PyramidMinMag, m_Context.PyramidMaxMag);

				m_DistBasedMatcher.InitNewMatch(m_StarMap, PyramidMatchType.PlateSolve);
			}
		}

		public void NextFrame(int frameNo, Model.Video.MovementType movementType, bool isLastFrame, Model.Astro.AstroImage astroImage, int firstFrameInIntegrationPeriod, string fileName)
		{
			m_CurrentFrameIndex = frameNo;

			m_UidentifiedObjects = null;
			m_IdentifiedObjects = null;
			m_UnknownObjects = null;

			m_AstroImage = astroImage;

			if (m_AstrometricState.MeasuringState == AstrometryInFramesState.Aborting)
				return;

			if (m_AstrometricState.MeasuringState == AstrometryInFramesState.RunningMeasurements &&
				m_AstrometricState.Measurements.Count > 0)
			{
				SingleMultiFrameMeasurement lastMeasurement = m_AstrometricState.Measurements[m_AstrometricState.Measurements.Count - 1];
				m_ViewControl.AddNewMeasurement(lastMeasurement);
			}

			StarMap starMap = new StarMap(
					TangraConfig.Settings.Astrometry.PyramidRemoveNonStellarObject,
					TangraConfig.Settings.Astrometry.MinReferenceStarFWHM,
					TangraConfig.Settings.Astrometry.MaxReferenceStarFWHM,
					TangraConfig.Settings.Astrometry.MaximumPSFElongation,
					TangraConfig.Settings.Astrometry.LimitReferenceStarDetection);

		    AstroImage noPreProcImage = astroImage;

            if (TangraCore.PreProcessors.UsesPreProcessing())
            {
                // Make sure we are using the unprocessed pixels to build the starmap for astrometry
                Pixelmap unprocessPixelmap = new Pixelmap(astroImage.Pixelmap.Width, astroImage.Pixelmap.Height,
                                                          astroImage.Pixelmap.BitPixCamera, astroImage.Pixelmap.UnprocessedPixels,
                                                          null, null);
                unprocessPixelmap.SetMaxSignalValue(astroImage.Pixelmap.MaxSignalValue);
                noPreProcImage = new AstroImage(unprocessPixelmap);                
            }

			starMap.FindBestMap(
				AstrometryContext.Current.StarMapConfig,
                noPreProcImage,
				AstrometryContext.Current.OSDRectToExclude,
				AstrometryContext.Current.RectToInclude,
				AstrometryContext.Current.LimitByInclusion);

			AstrometryContext.Current.StarMap = starMap;

			m_StarMap = AstrometryContext.Current.StarMap;

#if GET_UNIT_TEST_DATA
			SaveFailedFitData();
#endif

#if PROFILING
			Profiler.Instance.StartTimer("MATCH");
#endif

			if (movementType != MovementType.Step && movementType != MovementType.StepBackwards)
			{
				// NOTE: This will also reset the full range of stars
				m_DistBasedMatcher = null;
				m_VeryFirstTimeDrawn = false;
			}

			EnsureDistBasedMatcher();

			m_DistBasedMatcher.InitNewFrame(m_StarMap);
			m_CurrentMatchResult = m_DistBasedMatcher.PerformMatch(out m_AstrometricFit);

			if (m_CurrentMatchResult == PerformMatchResult.FieldAlignmentFailed ||
				m_CurrentMatchResult == PerformMatchResult.FitImprovementFailed ||
				m_CurrentMatchResult == PerformMatchResult.SearchAborted)
			{
				if (m_MeasurementContext == null || m_MeasurementContext.StopOnNoFit)
				{
					if (m_AstrometricState.MeasuringState == AstrometryInFramesState.RunningMeasurements)
					{
						m_VideoController.StopVideo();
						m_AstrometricState.MeasuringState = AstrometryInFramesState.Paused;

						TangraContext.Current.CanPlayVideo = false;
						TangraContext.Current.CanScrollFrames = false;
						m_VideoController.UpdateViews();
					}

					m_ViewControl.FitFailed();

					return;
				}
			}

#if PROFILING
			Profiler.Instance.StopTimer("MATCH");
#endif

			if (m_AstrometricFit == null &&
				!m_DistBasedMatcher.SearchAborted)
			{
#if DEBUG_VERSION
                string imageDbg = PersistanceManager.SerializeAsBase64String(m_Image);
                byte[] starMapDbg = PersistanceManager.Serialize(m_StarMap);
                string starsDbg = PersistanceManager.SerializeAsBase64String(m_CatalogueStars);

                File.WriteAllText(@"C:\Astrometry_imageDbg.txt", imageDbg);
                File.WriteAllBytes(@"C:\Astrometry_starmapDbg.bin", starMapDbg);
                File.WriteAllText(@"C:\Astrometry_starsDbg.txt", starsDbg);
#endif
			}

			AstrometryContext.Current.CurrentAstrometricFit = m_AstrometricFit;

			m_PhotometricFit = null;
			AstrometryContext.Current.CurrentPhotometricFit = null;

			if (m_AstrometricFit != null)
			{
				m_AstrometricState.AstrometricFit = m_AstrometricFit;

				if (m_AstrometricState.MeasuringState == AstrometryInFramesState.AttemptingInitialFit)
					m_AstrometricState.MeasuringState = AstrometryInFramesState.Ready;

				m_VideoController.StatusChanged(string.Format("{0} Fit ({1} Stars - {2}) ",
					m_AstrometricFit.FitOrder,
					m_AstrometricFit.FitInfo.NumberOfStarsUnsedInSolution(),
					m_Context.StarCatalogueFacade.CatalogNETCode));


				if (m_CurrentView == DisplayedView.ManualFit)
				{
					// A successful fit was obtained after a manual alignment
					EnsureSwitchedToAstormetricFitView(m_ControlPanelHolder);
				}

				// If we are not running mesaurements then do a photometry fit (its cheap)
				// when doing measurements only do photometry fits if the user has requested
				if ((m_MeasurementContext != null && m_MeasurementContext.PerformPhotometricFit) ||
					m_AstrometricState.MeasuringState != AstrometryInFramesState.RunningMeasurements)
				{
#if PROFILING
					Profiler.Instance.StartTimer("PHOTMETRIC_FIT");
#endif

					m_PhotometricFit = StarMagnitudeFit.PerformFit(
						m_AstrometryController,
						m_VideoController,
						m_AstroImage.Pixelmap.BitPixCamera,
						m_AstrometricFit.FitInfo,
						m_MeasurementContext == null
							? TangraConfig.PhotometryReductionMethod.AperturePhotometry
							: m_MeasurementContext.PhotometryReductionMethod,
						TangraConfig.Settings.Photometry.PsfQuadrature,
						TangraConfig.Settings.Photometry.PsfFittingMethod,
						m_MeasurementContext == null
							? TangraConfig.BackgroundMethod.AverageBackground
							: m_MeasurementContext.PhotometryBackgroundMethod,
						TangraConfig.Settings.PlateSolve.UseLowPassForAstrometry
							? TangraConfig.PreProcessingFilter.LowPassFilter
							: TangraConfig.PreProcessingFilter.NoFilter,
						m_CatalogueStars,
						m_MeasurementContext == null
							? Guid.Empty
							: m_MeasurementContext.PhotometryCatalogBandId,
						TangraConfig.Settings.Photometry.EncodingGamma,
						m_MeasurementContext != null ? m_MeasurementContext.ApertureSize : (float?)null,
						m_MeasurementContext != null ? m_MeasurementContext.AnnulusInnerRadius : (float?)null,
						m_MeasurementContext != null ? m_MeasurementContext.AnnulusMinPixels : (int?)null,
						ref m_empericalPSFR0);

#if PROFILING
					Profiler.Instance.StopTimer("PHOTMETRIC_FIT");
#endif

					AstrometryContext.Current.CurrentPhotometricFit = m_PhotometricFit;
				}

				// NOTE: Photometric fit debug code
				//Trace.WriteLine(string.Format("Photometric Fit: a = {0}; b = {1}; From {2} stars magnitudes {8} to {9}; Sigma = {3} ({4} | {5}); Gamma = {6}; Excluded Stars: {7}",
				//	m_PhotometricFit.A.ToString("0.00"), m_PhotometricFit.B.ToString("0.00"), m_PhotometricFit.NumStars,
				//	m_PhotometricFit.Sigma.ToString("0.00"), m_PhotometricFit.MinRes.ToString("0.00"),
				//	m_PhotometricFit.MaxRes.ToString("0.00"), m_PhotometricFit.EncodingGamma.ToString("0.00"), m_PhotometricFit.ExcludedStars,
				//	 m_PhotometricFit.MinMag.ToString("0.00"), m_PhotometricFit.MaxMag.ToString("0.00")));

				//float[] gammas = new float[] {0.35f, 0.45f, 1.0f};
				//foreach (float gamma in gammas)
				//{
				//	StarMagnitudeFit photoFit = StarMagnitudeFit.PerformFit(m_AstrometricFit.FitInfo, m_CatalogueStars, gamma);
				//	photoData.Append(
				//		string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8},", photoFit.A, photoFit.B, photoFit.NumStars, photoFit.ExcludedStars, photoFit.Sigma, photoFit.MinRes, photoFit.MaxRes, photoFit.MinMag, photoFit.MaxMag));
				//}
			}
			else
			{
				m_VideoController.StatusChanged("No Fit");
				m_AstrometricState.AstrometricFit = null;
			}

			

			bool isLastMeasurement = isLastFrame;

			// Run measurements if in this mode
			if (m_AstrometricState.MeasuringState == AstrometryInFramesState.RunningMeasurements && 
				m_MeasurementContext != null)
			{
				if (m_AstrometricState.Measurements.Count >= m_MeasurementContext.MaxMeasurements ||
					m_CurrentFrameIndex + m_MeasurementContext.FrameInterval > m_VideoController.VideoLastFrame)
				{
					m_VideoController.StopVideo();
					isLastMeasurement = true;
				}
				else
				{
					MeasurePositionInFrames(frameNo);
					ExecuteAstrometryAddins();
				}
			}
			else
			{
				// We only show info about the astrometric solution when not in measurement mode 
				// as we don't want to hide the measurement control
				m_ViewControl.PresentAstrometricFit(m_AstrometricFit, m_PhotometricFit);
			}

			if (isLastMeasurement)
			{
				m_ViewControl.FrameMeasurementFinished();
				m_AstrometricState.MeasuringState = AstrometryInFramesState.Ready;
				foreach (ITangraAddin addin in m_AstrometryAddins)
					addin.OnEventNotification(AddinFiredEventType.EndMultiFrameAstrometry);

				m_ViewControl.ShowMeasurementsView();
			}

			if (!m_VeryFirstTimeDrawn)
			{
				m_VeryFirstTimeDrawn = true;
				m_VideoController.RedrawCurrentFrame(false, true);
			}

			AstrometryContext.Current.CurrentAstrometricFit = m_AstrometricFit;
			AstrometryContext.Current.CurrentPhotometricFit = m_PhotometricFit;
		}

		private void MeasurePositionInFrames(int frameNo)
		{
			if (m_AstrometricFit != null &&
				m_AstrometricState.ObjectToMeasure != null)
			{
				if (m_AstrometricFit.StdDevRAArcSec <= m_MeasurementContext.MaxStdDev &&
					m_AstrometricFit.StdDevDEArcSec <= m_MeasurementContext.MaxStdDev)
				{
					ImagePixel centroid = AstrometryContext.Current.StarMap.GetCentroid(
						(int)m_AstrometricState.ObjectToMeasure.X0,
						(int)m_AstrometricState.ObjectToMeasure.Y0,
						CoreAstrometrySettings.Default.PreMeasureSearchCentroidRadius);

					// TODO: Check signal detection setting ??
					if (centroid != null)
					{
						PSFFit psfFit;
						AstrometryContext.Current.StarMap.GetPSFFit(
							centroid.X, centroid.Y, PSFFittingMethod.NonLinearFit, out psfFit);

						if (psfFit != null)
						{
							double ra, de;
							m_AstrometricFit.GetRADEFromImageCoords(psfFit.XCenter, psfFit.YCenter, out ra, out de);

							double maxPosDiffArcSec =
									m_AstrometricFit.GetDistanceInArcSec(m_AstrometricFit.Image.CenterXImage, m_AstrometricFit.Image.CenterYImage,
									m_AstrometricFit.Image.CenterXImage + CoreAstrometrySettings.Default.PreMeasureSearchCentroidRadius, m_AstrometricFit.Image.CenterYImage);

							if (!double.IsNaN(m_AstrometricState.ObjectToMeasure.RADeg))
							{
								double posDif = 3600 * AngleUtility.Elongation(m_AstrometricState.ObjectToMeasure.RADeg, m_AstrometricState.ObjectToMeasure.DEDeg, ra, de);
								if (posDif > maxPosDiffArcSec)
								{
									// NOTE: Not a valid measurement
									Trace.WriteLine(string.Format("The target position is too far from the last measured position", posDif));
									return;
								}
							}

							Trace.WriteLine(string.Format(
								"Measuring Astometry in frame: RA = {0}; DE = {1}; Std.Dev = {2}\"; {3}\"",
								AstroConvert.ToStringValue(ra / 15.0, "HH MM SS.TT"),
								AstroConvert.ToStringValue(de, "+DD MM SS.T"),
								m_AstrometricFit.StdDevRAArcSec,
								m_AstrometricFit.StdDevDEArcSec));

							m_AstrometricState.ObjectToMeasure.X0 = (float)psfFit.XCenter;
							m_AstrometricState.ObjectToMeasure.Y0 = (float)psfFit.YCenter;
							m_AstrometricState.ObjectToMeasure.RADeg = ra;
							m_AstrometricState.ObjectToMeasure.DEDeg = de;
							m_AstrometricState.ObjectToMeasure.Gaussian = psfFit;
							m_AstrometricState.ObjectToMeasure.AstrometricFit = m_AstrometricFit;

							SingleMultiFrameMeasurement measurement = new SingleMultiFrameMeasurement();
							measurement.FrameNo = frameNo;
							measurement.RADeg = ra;
							measurement.DEDeg = de;
							measurement.StdDevRAArcSec = m_AstrometricFit.StdDevRAArcSec;
							measurement.StdDevDEArcSec = m_AstrometricFit.StdDevDEArcSec;
							measurement.AstrometricFit = m_AstrometricFit;
							measurement.Gaussian = psfFit;

							if (m_PhotometricFit != null)
							{
								bool isSaturated;
								double I = AstrometryContext.Current.CurrentPhotometricFit.GetIntencity(new ImagePixel(255, psfFit.XCenter, psfFit.YCenter), out isSaturated);
								double mag = AstrometryContext.Current.CurrentPhotometricFit.GetMagnitudeForIntencity(I);
								measurement.Mag = mag;
							}
							else
								measurement.Mag = double.NaN;

							m_AstrometricState.Measurements.Add(measurement);
						}
					}
				}
			}
		}

		private void ExecuteAstrometryAddins()
		{
		    if (m_AstrometryAddins.Count > 0)
		    {
		        m_AddinsController.SetAstrometryProvider(this);

		        foreach (ITangraAddinAction addinAction in m_AstrometryAddinActions)
		        {
		            addinAction.Execute();
		        }
		    }
		}

		public void ImageToolChanged(Model.ImageTools.ImageTool newTool, Model.ImageTools.ImageTool oldTool)
		{ }

		public void PreDraw(Graphics g)
		{ }

		public void PostDraw(Graphics g)
		{
			if (m_AstrometricState.MeasuringState == AstrometryInFramesState.Aborting)
				return;

			if (m_AstrometricState.SelectedObject != null)
			{
				m_ViewControl.PresentSelectedObject(m_AstrometricState.SelectedObject);
				m_AstrometricState.SelectedObject = null;
			}

			if (m_AstrometricFit != null)
			{
				string catalogPrefix = TangraConfig.Settings.StarCatalogue.Catalog.ToString();

				List<ulong> starNos = new List<ulong>();

				foreach (PlateConstStarPair refStar in m_AstrometricFit.FitInfo.AllStarPairs)
				{
					Pen starPen = unrecognizedStarPen;
					Brush labelBruish = catalogBrushUnrecognized;

					if (refStar.FitInfo.UsedInSolution)
					{
						starPen = referenceStarPen;
						labelBruish = catalogBrushReference;
					}

					if (refStar.FitInfo.ExcludedForHighResidual)
					{
						starPen = rejectedStarPen;
						labelBruish = catalogBrushRejected;
					}

					double x, y;
					m_AstrometricFit.GetImageCoordsFromRADE(refStar.RADeg, refStar.DEDeg, out x, out y);
					g.DrawEllipse(starPen, (float)x - 5, (float)y - 5, 10, 10);

					if (m_ShowStarLabels || m_ShowStarMagnitudes)
					{
						string output;
						if (m_ShowStarLabels && m_ShowStarMagnitudes)
							output = string.Format("{0} {1} ({2}m)", catalogPrefix, refStar.StarNo, refStar.Mag);
						else if (m_ShowStarLabels)
							output = string.Format("{0} {1}", catalogPrefix, refStar.StarNo);
						else
							output = string.Format("{0}m", refStar.Mag);

						g.DrawString(output, s_StarInfoFont, labelBruish, (float)x + 10, (float)y + 10);
					}

					starNos.Add(refStar.StarNo);
				}

				foreach (IStar catStar in m_DistBasedMatcher.Context.ConsideredStars)
				{
					if (starNos.IndexOf(catStar.StarNo) == -1)
					{
						double x, y;
						m_AstrometricFit.GetImageCoordsFromRADE(catStar.RADeg, catStar.DEDeg, out x, out y);
						g.DrawEllipse(unrecognizedStarPen, (float)x - 5, (float)y - 5, 10, 10);

						if (m_ShowStarLabels)
							g.DrawString(catStar.GetStarDesignation(0), s_StarInfoFont, catalogBrushUnrecognized, (float)x + 10, (float)y + 10);

					}
				}

				#region Draw all identified objects

				if (m_AstrometricState != null &&
					m_AstrometricState.IdentifiedObjects.Count > 0)
				{
					foreach (IIdentifiedObject entry in m_AstrometricState.IdentifiedObjects)
					{
						DrawKnownObject(g, entry.RAHours * 15.0, entry.DEDeg, entry.ObjectName);
					}
				}

				#endregion

				#region Draw all currently resolved objects 
				if (m_IdentifiedObjects != null && m_UnknownObjects != null)
				{
					foreach (PSFFit star in m_IdentifiedObjects.Keys)
					{
						float x = (float)star.XCenter;
						float y = (float)star.YCenter;

						g.DrawEllipse(Pens.GreenYellow, x - 5, y - 5, 10, 10);
					}

					foreach (PSFFit star in m_UnknownObjects.Keys)
					{
						float x = (float)star.XCenter;
						float y = (float)star.YCenter;

						g.DrawEllipse(Pens.Tomato, x - 8, y - 8, 16, 16);
					}
				}
				#endregion

				#region Draw the measured object cross cursor
				if (m_AstrometricState.ObjectToMeasure != null)
				{
					g.DrawEllipse(
						identifiedObjectPen,
						m_AstrometricState.ObjectToMeasure.X0 - 3,
						m_AstrometricState.ObjectToMeasure.Y0 - 3, 6, 6);

					g.DrawLine(identifiedObjectPen,
							   m_AstrometricState.ObjectToMeasure.X0 - 8,
							   m_AstrometricState.ObjectToMeasure.Y0,
							   m_AstrometricState.ObjectToMeasure.X0 - 4,
							   m_AstrometricState.ObjectToMeasure.Y0);

					g.DrawLine(identifiedObjectPen,
							   m_AstrometricState.ObjectToMeasure.X0 + 8,
							   m_AstrometricState.ObjectToMeasure.Y0,
							   m_AstrometricState.ObjectToMeasure.X0 + 4,
							   m_AstrometricState.ObjectToMeasure.Y0);

					g.DrawLine(identifiedObjectPen,
							   m_AstrometricState.ObjectToMeasure.X0,
							   m_AstrometricState.ObjectToMeasure.Y0 - 8,
							   m_AstrometricState.ObjectToMeasure.X0,
							   m_AstrometricState.ObjectToMeasure.Y0 - 4);

					g.DrawLine(identifiedObjectPen,
							   m_AstrometricState.ObjectToMeasure.X0,
							   m_AstrometricState.ObjectToMeasure.Y0 + 8,
							   m_AstrometricState.ObjectToMeasure.X0,
							   m_AstrometricState.ObjectToMeasure.Y0 + 4);
				}
				#endregion
			}

		}

		private void DrawKnownObject(Graphics g, double raDeg, double deDeg, string name)
		{
			double x, y;
			m_AstrometricFit.GetImageCoordsFromRADE(raDeg, deDeg, out x, out y);

			PSFFit pstFit;
			m_StarMap.GetPSFFit((int)x, (int)y, PSFFittingMethod.NonLinearFit, out pstFit);

			double distance = m_AstrometricFit.GetDistanceInArcSec(x, y, pstFit.XCenter, pstFit.YCenter);

			if (distance < CoreAstrometrySettings.Default.IdentifiedObjectResidualInArcSec)
			{
				g.DrawEllipse(identifiedObjectPen, (float)pstFit.XCenter - 5, (float)pstFit.YCenter - 5, 10, 10);

				if (m_ShowObjectLabels)
					g.DrawString(name, s_UserObjectFont, identifiedObjectBrush, (float)pstFit.XCenter + 10, (float)pstFit.YCenter + 10);
			}
			else
			{
				g.DrawEllipse(unrecognizedStarPen, (float)x - 5, (float)y - 5, 10, 10);

				if (m_ShowObjectLabels)
					g.DrawString(name, s_UserObjectFont, catalogBrushUnrecognized, (float)pstFit.XCenter + 10, (float)pstFit.YCenter + 10);
			}
		}

		public bool HasCustomZoomImage
		{
			get { return false; }
		}

		public bool DrawCustomZoomImage(System.Drawing.Graphics g, int width, int height)
		{
			return false;
		}

		public bool AvoidImageOverlays
		{
			get { return true; }
		}

		internal void NewObjectSelected(SelectedObject objInfo)
		{
			m_ViewControl.PresentSelectedObject(objInfo);
		}

		public ITangraAstrometricSolution GetCurrentFrameAstrometricSolution()
		{
			return new AstrometricSolutionImpl(m_AstrometricFit, m_PhotometricFit, m_AstrometricState, m_Context);
		}
	}
}
