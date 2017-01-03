using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Tangra.Astrometry;
using Tangra.Helpers;
using Tangra.ImageTools;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Model.Context;
using Tangra.Model.Image;
using Tangra.Model.VideoOperations;
using Tangra.MotionFitting;
using Tangra.VideoOperations.Astrometry;
using Tangra.VideoOperations.Astrometry.Engine;
using Tangra.VideoOperations.LightCurves;

namespace Tangra.Controller
{
	internal interface ICalibrationRunner
	{
		void RunCalibrationWithCurrentPreliminaryFit();
	}

	public class AstrometryController : IAstrometryController
	{
		List<Tuple<INotificationReceiver, Type>> m_NotificationReceivers = new List<Tuple<INotificationReceiver, Type>>();
		private object m_SyncLock = new object();

		private VideoController m_VideoController;
		private LongOperationsManager m_LongOperationsManager;

		internal AstrometryController(VideoController videoController, LongOperationsManager longOperationsManager)
		{
			m_VideoController = videoController;
			m_LongOperationsManager = longOperationsManager;
		}
 
		public void Subscribe(INotificationReceiver receiver, Type notificationType)
		{
			lock (m_SyncLock)
			{
				m_NotificationReceivers.Add(new Tuple<INotificationReceiver, Type>(receiver, notificationType));
			}
		}

		public void Unsubscribe(INotificationReceiver receiver)
		{
			lock (m_SyncLock)
			{
				for (int i = m_NotificationReceivers.Count - 1; i >= 0; i--)
				{
					if (object.Equals(m_NotificationReceivers[i].Item1, receiver))
						m_NotificationReceivers.RemoveAt(i);
				}
			}
		}

		public void SendNotification(object notification)
		{
			if (notification == null)
				return;

			lock (m_SyncLock)
			{
				for (int i = m_NotificationReceivers.Count - 1; i >= 0; i--)
				{
					if (m_NotificationReceivers[i].Item2 == notification.GetType())
						m_NotificationReceivers[i].Item1.ReceieveMessage(notification);
				}
			}
		}

		public void NotifyBeginLongOperation(string description)
		{
			m_LongOperationsManager.BeginLongOperation(description);
		}

		public void NotifyEndLongOperation()
		{
			m_LongOperationsManager.EndLongOperation();
		}

		public AstroImage AstroImage { get; private set; }

		public AstroPlate GetCurrentAstroPlate()
		{
			CCDMatrix matrix;

			if (AstrometryContext.Current.VideoCamera == null)
			{
				// This is used for the case where no configuration has been loaded.
				matrix = new CCDMatrix(1, 1, TangraContext.Current.FrameWidth, TangraContext.Current.FrameHeight);
			}
			else
				matrix = new CCDMatrix(
					AstrometryContext.Current.VideoCamera.CCDMetrics.CellWidth,
					AstrometryContext.Current.VideoCamera.CCDMetrics.CellHeight,
					AstrometryContext.Current.VideoCamera.CCDMetrics.MatrixWidth,
					AstrometryContext.Current.VideoCamera.CCDMetrics.MatrixHeight);

			AstroPlate image = new AstroPlate(matrix, TangraContext.Current.FrameWidth, TangraContext.Current.FrameHeight, m_VideoController.VideoBitPix);

			if (AstrometryContext.Current.PlateConstants != null)
			{
				image.EffectivePixelWidth = AstrometryContext.Current.PlateConstants.EffectivePixelWidth;
				image.EffectivePixelHeight = AstrometryContext.Current.PlateConstants.EffectivePixelHeight;
				image.EffectiveFocalLength = AstrometryContext.Current.PlateConstants.EffectiveFocalLength;
			}

			return image;
		}

		private ICalibrationRunner m_CalibrationRunner;

		internal void RegisterCalibrationRunner(ICalibrationRunner runner)
		{
			lock (m_SyncLock)
			{
				m_CalibrationRunner = runner;
			}
		}

		private VideoAstrometryOperation m_Operation;

		internal void SetOperation(VideoAstrometryOperation operation)
		{
			m_Operation = operation;
		}

		internal void NewObjectSelected(SelectedObject objInfo)
		{
			if (m_Operation != null)
				m_Operation.NewObjectSelected(objInfo);
		}

		public void RunCalibrationWithCurrentPreliminaryFit()
		{
			lock (m_SyncLock)
			{
				if (m_CalibrationRunner != null)
				{
					m_CalibrationRunner.RunCalibrationWithCurrentPreliminaryFit();
				}
			}
		}

		public Rectangle OSDRectToExclude
		{
			get { return AstrometryContext.Current.OSDRectToExclude; }
		}

		public Rectangle RectToInclude
		{
			get { return AstrometryContext.Current.RectToInclude; }
		}

		public bool LimitByInclusion
		{
			get { return AstrometryContext.Current.LimitByInclusion; }
		}

		public void SetManuallyIdentifyStarState(bool enable)
		{
			m_Operation.SetManuallyIdentifyStarState(enable);
		}

		public void TriggerPlateReSolve()
		{
			m_Operation.TriggerPlateReSolve();
		}

        public FlyByMotionFitter.FrameTimeInfo GetFrameTimeInfo(int frameId)
        {
            if (m_VideoController.IsAstroAnalogueVideo && m_VideoController.HasTimestampOCR())
            {
                var exposure = m_VideoController.AstroAnalogueVideoIntegratedAAVFrames * m_VideoController.FramePlayer.Video.MillisecondsPerFrame;
                var timestamp = m_VideoController.OCRTimestamp();
                if (timestamp != DateTime.MinValue)
                {
                    timestamp = AstrometryContext.Current.FieldSolveContext.UtcTime.Date.Add(timestamp.TimeOfDay);
                    timestamp = timestamp.AddMilliseconds(0.5 * exposure);
                    var nativeFormat = m_VideoController.GetVideoFormat(m_VideoController.GetVideoFileFormat());
                    if (nativeFormat == "PAL") timestamp = timestamp.AddMilliseconds(-20);
                    else if (nativeFormat == "NTSC") timestamp = timestamp.AddMilliseconds(-16.68);
                }

                return new FlyByMotionFitter.FrameTimeInfo()
                {
                    CentralExposureTime = timestamp,
                    ExposureInMilliseconds = exposure
                };
            }

            var frameStateInfo = m_VideoController.GetFrameStateData(frameId);
            return new FlyByMotionFitter.FrameTimeInfo()
            {
                CentralExposureTime = frameStateInfo.CentralExposureTime,
                ExposureInMilliseconds = frameStateInfo.ExposureInMilliseconds
            };
        }
	}
}
