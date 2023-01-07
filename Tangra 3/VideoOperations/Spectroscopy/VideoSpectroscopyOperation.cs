﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Controller;
using Tangra.Helpers;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Model.Image;
using Tangra.Model.ImageTools;
using Tangra.Model.Video;
using Tangra.Model.VideoOperations;
using Tangra.PInvoke;
using Tangra.VideoOperations.LightCurves.Tracking;
using Tangra.VideoOperations.Spectroscopy.Helpers;
using Tangra.VideoOperations.Spectroscopy.Tracking;

namespace Tangra.VideoOperations.Spectroscopy
{
    public enum SpectroscopyState
    {
        ChoosingStar,
        ChoosingAngleManually,
        StarConfirmed,
        RunningMeasurements,
		DisplayingMeasurements
    }

    public class VideoSpectroscopyOperation : VideoOperationBase, IVideoOperation
    {
        private VideoController m_VideoController;
	    private IFramePlayer m_FramePlayer;
        private SpectroscopyController m_SpectroscopyController;
        private TangraConfig.PersistedConfiguration m_Configuration;
        private ucSpectroscopy m_ControlPanel = null;
        private object m_SyncRoot = new object();

        private int m_OriginalWidth;
        private int m_OriginalHeight;
        private RectangleF m_OriginalVideoFrame;

	    private SpectraReader m_Reader;
		private SpectroscopyStarTracker m_Tracker;

        private SpectroscopyState m_OperationState = SpectroscopyState.ChoosingStar;
		public IImagePixel SelectedStar { get; private set; }
        public Point SelectedAnglePoint { get; private set; }
        public double SelectedStarFWHM { get; private set; }

        public int MeasurementAreaWing { get; private set; }
        public int BackgroundAreaWing { get; private set; }
        public int BackgroundAreaGap { get; private set; }
        public float PixelValueCoefficient { get; private set; }

        private List<Spectra> m_AllFramesSpectra = new List<Spectra>();
	    private MasterSpectra m_MasterSpectra;

        private int m_CurrentFrameNo;
        private int? m_FirstMeasuredFrame;
	    private byte[] m_FrameBitmapPixels;
        private DateTime? m_FirstFrameTimeStamp;

		private PSFFit m_SelectedStarGaussian;
		public float SelectedStarBestAngle { get; private set; }
	    private bool m_CancelMeasurementsRequested = false;

        public VideoSpectroscopyOperation()
        { }

        public VideoSpectroscopyOperation(SpectroscopyController spectroscopyController, TangraConfig.PersistedConfiguration configuration, bool debugMode)
        {
            m_Configuration = configuration;
            m_SpectroscopyController = spectroscopyController;
            PixelValueCoefficient = 1;
        }

        public bool InitializeOperation(IVideoController videoContoller, Panel controlPanel, IFramePlayer framePlayer, Form topForm)
        {
            m_VideoController = (VideoController)videoContoller;
	        m_FramePlayer = framePlayer;

            if (m_ControlPanel == null)
            {
                lock (m_SyncRoot)
                {
                    if (m_ControlPanel == null)
                    {
						m_ControlPanel = new ucSpectroscopy(this, (VideoController)videoContoller, m_SpectroscopyController, framePlayer);
                    }
                }
            }

            controlPanel.Controls.Clear();
            controlPanel.Controls.Add(m_ControlPanel);
            m_ControlPanel.Dock = DockStyle.Fill;

            m_OperationState = SpectroscopyState.ChoosingStar;
            SelectedStar = null;
            SelectedAnglePoint = Point.Empty;

            m_OriginalWidth = framePlayer.Video.Width;
            m_OriginalHeight = framePlayer.Video.Height;
            m_OriginalVideoFrame = new Rectangle(0, 0, m_OriginalWidth, m_OriginalHeight);

            return true;
        }

	    public TangraConfig.SpectraViewDisplaySettings DisplaySettings
	    {
		    get { return m_SpectroscopyController.DisplaySettings; }
	    }

        public void FinalizeOperation()
        {
            
        }

        public void PlayerStarted()
        {
            
        }

        internal void StartMeasurements()
	    {
            MeasurementAreaWing = m_SpectroscopyController.SpectraReductionContext.MeasurementAreaWing;
            BackgroundAreaWing = m_SpectroscopyController.SpectraReductionContext.BackgroundAreaWing;
            BackgroundAreaGap = m_SpectroscopyController.SpectraReductionContext.BackgroundAreaGap;
            PixelValueCoefficient = m_SpectroscopyController.SpectraReductionContext.PixelValueCoefficient;

		    var starToTrack = new TrackedObjectConfig()
		    {
                ApertureInPixels = (float)(SelectedStarFWHM * 2),
				MeasureThisObject = false,
				ApertureDX = 0,
				ApertureDY = 0,
			
				Gaussian = m_SelectedStarGaussian,
				ApertureStartingX = (float)m_SelectedStarGaussian.XCenter,
				ApertureStartingY = (float)m_SelectedStarGaussian.YCenter,
				TrackingType = TrackingType.OccultedStar,
				IsWeakSignalObject = false
		    };

			m_Tracker = new SpectroscopyStarTracker(starToTrack);

            m_OperationState = SpectroscopyState.RunningMeasurements;
	        m_AllFramesSpectra.Clear();

			m_ControlPanel.MeasurementsStarted();

            m_FirstMeasuredFrame = null;
	        m_CancelMeasurementsRequested = false;
			m_FramePlayer.Start(FramePlaySpeed.Fastest, null, 1);
	    }

	    internal void StopMeasurements()
	    {
		    m_CancelMeasurementsRequested = true;
	    }

	    public void NextFrame(int frameNo, MovementType movementType, bool isLastFrame, AstroImage astroImage, int firstFrameInIntegrationPeriod, string fileName)
        {
            m_CurrentFrameNo = frameNo;

	        if (m_OperationState == SpectroscopyState.RunningMeasurements)
	        {
                if (m_FirstMeasuredFrame == null)
                {
                    m_FirstMeasuredFrame = m_CurrentFrameNo;
                    if (m_VideoController.HasEmbeddedTimeStamps()) m_FirstFrameTimeStamp = m_VideoController.GetCurrentFrameTime();
					else if (m_VideoController.HasSystemTimeStamps()) m_FirstFrameTimeStamp = m_VideoController.GetCurrentFrameTime();
	                m_FrameBitmapPixels = astroImage.Pixelmap.DisplayBitmapPixels;
                }

		        m_Tracker.NextFrame(frameNo, astroImage);
		        if (m_Tracker.IsTrackedSuccessfully)
		        {
			        TrackedObject trackedStar = m_Tracker.TrackedStar;
			        SelectedStar = trackedStar.Center;

                    m_Reader = new SpectraReader(astroImage, SelectedStarBestAngle, m_SpectroscopyController.SpectraReductionContext.PixelValueCoefficient);

                    Spectra thisFrameSpectra = m_Reader.ReadSpectra(
                        trackedStar.ThisFrameX, 
                        trackedStar.ThisFrameY, 
                        m_SpectroscopyController.SpectraReductionContext.MeasurementAreaWing, 
                        m_SpectroscopyController.SpectraReductionContext.BackgroundAreaWing, 
                        m_SpectroscopyController.SpectraReductionContext.BackgroundAreaGap,
                        m_SpectroscopyController.SpectraReductionContext.BackgroundMethod);

                    thisFrameSpectra.ZeroOrderFWHM = trackedStar.PSFFit != null ? (float)trackedStar.PSFFit.FWHM : float.NaN;

		            m_AllFramesSpectra.Add(thisFrameSpectra);
		        }

				if (isLastFrame || m_CancelMeasurementsRequested || m_AllFramesSpectra.Count >= m_SpectroscopyController.SpectraReductionContext.FramesToMeasure)
                {
                    m_FramePlayer.Stop();

                    m_MasterSpectra = m_SpectroscopyController.ComputeResult(
                        m_AllFramesSpectra, 
                        m_SpectroscopyController.SpectraReductionContext.FrameCombineMethod,
                        m_SpectroscopyController.SpectraReductionContext.UseFineAdjustments,
                        m_SpectroscopyController.SpectraReductionContext.AlignmentAbsorptionLinePos);

                    m_AllFramesSpectra.Clear();
					
                    m_MasterSpectra.MeasurementInfo = m_SpectroscopyController.GetMeasurementInfo();
                    m_MasterSpectra.MeasurementInfo.FirstMeasuredFrame = m_FirstMeasuredFrame.Value;
                    m_MasterSpectra.MeasurementInfo.LastMeasuredFrame = m_CurrentFrameNo;
					m_MasterSpectra.MeasurementInfo.FirstFrameTimeStamp = m_FirstFrameTimeStamp;
					if (m_VideoController.HasEmbeddedTimeStamps())
						m_MasterSpectra.MeasurementInfo.LastFrameTimeStamp = m_VideoController.GetCurrentFrameTime();
					else if (m_VideoController.HasSystemTimeStamps())
						m_MasterSpectra.MeasurementInfo.LastFrameTimeStamp = m_VideoController.GetCurrentFrameTime();

                    FrameStateData frameStatus = m_VideoController.GetCurrentFrameState();
                    m_MasterSpectra.MeasurementInfo.Gain = frameStatus.Gain;
                    m_MasterSpectra.MeasurementInfo.ExposureSeconds = m_SpectroscopyController.SpectraReductionContext.ExposureSeconds;

					m_MasterSpectra.MeasurementInfo.FrameBitmapPixels = m_FrameBitmapPixels;

	                m_SpectroscopyController.PopulateMasterSpectraObservationDetails(m_MasterSpectra);

					m_OperationState = SpectroscopyState.DisplayingMeasurements;
	                m_ControlPanel.MeasurementsFinished();
					DisplaySpectra();
                }

		        Application.DoEvents();
	        }
        }

	    internal void DisplaySpectra()
	    {
			m_SpectroscopyController.DisplaySpectra(m_MasterSpectra, m_Configuration, m_SpectroscopyController.DisplaySettings);
	    }

        public void ImageToolChanged(ImageTool newTool, ImageTool oldTool)
        {
            
        }

        public void PreDraw(Graphics g)
        {
            
        }

	    

        public void PostDraw(Graphics g)
        {
            if (SelectedStar != null)
            {
                if (m_OperationState == SpectroscopyState.ChoosingAngleManually && SelectedAnglePoint != Point.Empty)
                {   
                    PointF p1 = new PointF((float)SelectedStar.XDouble, (float)SelectedStar.YDouble);
                    g.DrawLine(Pens.Aqua, p1, SelectedAnglePoint);
                }
                else if (m_OperationState == SpectroscopyState.StarConfirmed ||
                         m_OperationState == SpectroscopyState.RunningMeasurements)
                {
                    g.DrawEllipse(Pens.Aqua, (float)SelectedStar.XDouble - 5, (float)SelectedStar.YDouble - 5, 10, 10);
                    var mapper = new RotationMapper(m_OriginalWidth, m_OriginalHeight, SelectedStarBestAngle);
                    float halfWidth = (float)MeasurementAreaWing;
                    float bgGap = (float)BackgroundAreaGap;
                    float bgSide = (float)BackgroundAreaWing;

                    PointF p0 = mapper.GetDestCoords((float)SelectedStar.XDouble, (float)SelectedStar.YDouble);
                    for (float i = p0.X - mapper.MaxDestDiagonal; i < p0.X + mapper.MaxDestDiagonal; i++)
                    {
                        PointF p1 = mapper.GetSourceCoords(i, p0.Y - halfWidth);
                        PointF p2 = mapper.GetSourceCoords(i + 1, p0.Y - halfWidth);
						if (m_OriginalVideoFrame.Contains(p1) && m_OriginalVideoFrame.Contains(p2)) g.DrawLine(m_SpectroscopyController.DisplaySettings.SpectraAperturePen, p1, p2);

                        PointF p3 = mapper.GetSourceCoords(i, p0.Y + halfWidth);
                        PointF p4 = mapper.GetSourceCoords(i + 1, p0.Y + halfWidth);
						if (m_OriginalVideoFrame.Contains(p3) && m_OriginalVideoFrame.Contains(p4)) g.DrawLine(m_SpectroscopyController.DisplaySettings.SpectraAperturePen, p3, p4);

                        p1 = mapper.GetSourceCoords(i, p0.Y - halfWidth - bgGap);
                        p2 = mapper.GetSourceCoords(i + 1, p0.Y - halfWidth - bgGap);
						if (m_OriginalVideoFrame.Contains(p1) && m_OriginalVideoFrame.Contains(p2)) g.DrawLine(m_SpectroscopyController.DisplaySettings.SpectraBackgroundPen, p1, p2);

                        p3 = mapper.GetSourceCoords(i, p0.Y + halfWidth + bgGap);
                        p4 = mapper.GetSourceCoords(i + 1, p0.Y + halfWidth + bgGap);
						if (m_OriginalVideoFrame.Contains(p3) && m_OriginalVideoFrame.Contains(p4)) g.DrawLine(m_SpectroscopyController.DisplaySettings.SpectraBackgroundPen, p3, p4);

                        p1 = mapper.GetSourceCoords(i, p0.Y - halfWidth - bgSide - bgGap);
                        p2 = mapper.GetSourceCoords(i + 1, p0.Y - halfWidth - bgSide - bgGap);
						if (m_OriginalVideoFrame.Contains(p1) && m_OriginalVideoFrame.Contains(p2)) g.DrawLine(m_SpectroscopyController.DisplaySettings.SpectraBackgroundPen, p1, p2);

                        p3 = mapper.GetSourceCoords(i, p0.Y + halfWidth + bgSide + bgGap);
                        p4 = mapper.GetSourceCoords(i + 1, p0.Y + halfWidth + bgSide + bgGap);
						if (m_OriginalVideoFrame.Contains(p3) && m_OriginalVideoFrame.Contains(p4)) g.DrawLine(m_SpectroscopyController.DisplaySettings.SpectraBackgroundPen, p3, p4);
                    }                    
                }
            }
        }

        internal void UpdateMeasurementAreasDisplay(int signalAreaHalfWidth, int backgroundAreaHalfWidth, int backgroundAreaGap)
		{
			MeasurementAreaWing  = signalAreaHalfWidth;
            BackgroundAreaWing = backgroundAreaHalfWidth;
            BackgroundAreaGap = backgroundAreaGap;

			m_VideoController.RedrawCurrentFrame(false, true);
	    }

        public override void MouseClick(ObjectClickEventArgs e)
        {
            if (m_OperationState == SpectroscopyState.ChoosingStar && e.Gausian != null && e.Gausian.IsSolved && e.Gausian.Certainty > 0.2)
            {
                SelectedStar = new ImagePixel(e.Gausian.XCenter, e.Gausian.YCenter);
                SelectedStarFWHM = e.Gausian.FWHM;
                m_SelectedStarGaussian = e.Gausian;
                MeasurementAreaWing = (int)(2 * Math.Ceiling(SelectedStarFWHM));
                BackgroundAreaWing = MeasurementAreaWing;
                BackgroundAreaGap = 5;

                SelectedAnglePoint = Point.Empty;
                m_ControlPanel.ClearSpectra();

                m_OperationState = SpectroscopyState.ChoosingAngleManually;

                m_VideoController.RedrawCurrentFrame(false, true);
            }
            else if (m_OperationState == SpectroscopyState.ChoosingAngleManually && SelectedStar != null)
            {
                double atanAgnle = 180 * Math.Atan((SelectedStar.YDouble - e.Pixel.YDouble) / (e.Pixel.XDouble - SelectedStar.XDouble)) / Math.PI;
                //if (atanAgnle < 0) atanAgnle = 360 + atanAgnle; // MCF
                int roughAngle = (int)atanAgnle;
                float bestAngle = m_SpectroscopyController.LocateSpectraAngle(roughAngle);
                if (!float.IsNaN(bestAngle))
                {
                    SetBestAngle(bestAngle);
                    m_VideoController.RedrawCurrentFrame(false, true);
                }
            }
        }

        private void SetBestAngle(float bestAngle)
        {
            SelectedStarBestAngle = bestAngle;

            m_OperationState = SpectroscopyState.StarConfirmed;

            var reader = new SpectraReader(m_VideoController.GetCurrentAstroImage(false), bestAngle, PixelValueCoefficient);

            Spectra spectra = reader.ReadSpectra(
                (float)SelectedStar.XDouble, 
                (float)SelectedStar.YDouble,
                MeasurementAreaWing,
                BackgroundAreaWing, 
                BackgroundAreaGap,
                PixelCombineMethod.Average);

            m_ControlPanel.PreviewSpectra(spectra);
        }

        public override void MouseMove(Point location)
        {
            if (m_OperationState == SpectroscopyState.ChoosingAngleManually && SelectedStar != null)
            {
                SelectedAnglePoint = location;
                m_VideoController.RedrawCurrentFrame(false, true);
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
            get
            {
                // No overlays allowed during the whole process
                return true;
            }
        }
    }
}
