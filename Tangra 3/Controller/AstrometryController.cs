using System;
using System.Collections.Concurrent;
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

        internal class FrameTime
        {
            public int RequestedFrameNo;
            public int ResolvedFrameNo;
            public DateTime UT;
            public double ClosestNormalFrameNo;
            public DateTime ClosestNormalFrameTime;
            public int ClosestNormalIntervalFirstFrameNo;
            public int ClosestNormalIntervalLastFrameNo;
        }

        internal FrameTime GetTimeForFrame(MeasurementContext context, int frameNumber, int firstVideoFrame)
        {
            var rv = new FrameTime();
            rv.RequestedFrameNo = frameNumber;

            double instrumentalDelayFrames = 0;
            double instrumentalDelaySeconds = 0;

            if (context.InstrumentalDelayUnits == InstrumentalDelayUnits.Frames)
                instrumentalDelayFrames = context.InstrumentalDelay;
            else if (context.InstrumentalDelayUnits == InstrumentalDelayUnits.Seconds)
                instrumentalDelaySeconds = context.InstrumentalDelay;

            int precision = 1000000;
            double sigma = 0.000005;
            if (context.FrameTimeType == FrameTimeType.NonIntegratedFrameTime)
            {
                if (context.VideoFileFormat == VideoFileFormat.AVI)
                {
                    // No integration is used, directly derive the time and apply instrumental delay
                    rv.ResolvedFrameNo = frameNumber - firstVideoFrame;
                    rv.UT =
                        context.FirstFrameUtcTime.AddSeconds(
                        ((frameNumber - context.FirstFrameId - instrumentalDelayFrames) / context.FrameRate) - instrumentalDelaySeconds);
                }
                else
                {
                    rv.ResolvedFrameNo = frameNumber;

                    FrameStateData frameState = m_VideoController.GetFrameStateData(frameNumber);
                    rv.UT = frameState.CentralExposureTime;

                    // Convert Central time to the timestamp of the end of the first field
                    rv.UT = rv.UT.AddMilliseconds(-0.5 * frameState.ExposureInMilliseconds);

                    if (context.NativeVideoFormat == "PAL") rv.UT = rv.UT.AddMilliseconds(20);
                    else if (context.NativeVideoFormat == "NTSC") rv.UT = rv.UT.AddMilliseconds(16.68);

                    if (context.AavStackedMode)
                    {
                        // TODO: Apply instrumental delay for Aav Stacked Frames
                        rv.UT = rv.UT.AddSeconds(-1 * instrumentalDelaySeconds);
                    }
                    else
                    {
                        // Then apply instrumental delay
                        rv.UT = rv.UT.AddSeconds(-1 * instrumentalDelaySeconds);
                    }
                }
            }
            else
            {
                // Integration was used. Find the integrated frame no
                int integratedFrameNo = frameNumber - firstVideoFrame;

                double deltaMiddleFrame = 0;
                if (context.IntegratedFramesCount > 1)
                    deltaMiddleFrame = (context.IntegratedFramesCount / 2) - 0.5;

                // The instrumental delay is always from the timestamp of the first frame of the integrated interval

                rv.ResolvedFrameNo = integratedFrameNo;

                rv.UT =
                    context.FirstFrameUtcTime.AddSeconds(
                    ((integratedFrameNo - deltaMiddleFrame - context.FirstFrameId - instrumentalDelayFrames) / context.FrameRate) - instrumentalDelaySeconds);
            }

            if (context.MovementExpectation == MovementExpectation.Slow)
            {
                precision = 100000;
                sigma = 0.000005;
            }
            else
            {
                precision = 1000000;
                sigma = 0.0000005;
            }

            double utTime = (rv.UT.Hour + rv.UT.Minute / 60.0 + (rv.UT.Second + (rv.UT.Millisecond / 1000.0)) / 3600.0) / 24;


            double roundedTime = Math.Truncate(Math.Round(utTime * precision)) / precision;

            rv.ClosestNormalFrameTime = new DateTime(rv.UT.Year, rv.UT.Month, rv.UT.Day).AddDays(roundedTime);

            TimeSpan delta = new TimeSpan(rv.ClosestNormalFrameTime.Ticks - rv.UT.Ticks);
            rv.ClosestNormalFrameNo = rv.ResolvedFrameNo + (delta.TotalSeconds * context.FrameRate);

            double framesEachSide = sigma * 24 * 3600 * context.FrameRate;
            rv.ClosestNormalIntervalFirstFrameNo = (int)Math.Floor(rv.ClosestNormalFrameNo - framesEachSide);
            rv.ClosestNormalIntervalLastFrameNo = (int)Math.Ceiling(rv.ClosestNormalFrameNo + framesEachSide);

            //Trace.WriteLine(string.Format("{0} / {1}; {2} / {3}; {4} / {5}", 
            //    roundedTime.ToString("0.00000"), utTime, 
            //    rv.ClosestNormalFrameNo, rv.ResolvedFrameNo,
            //    rv.UT.ToString("HH:mm:ss.fff"), rv.ClosestNormalFrameTime.ToString("HH:mm:ss.fff")));

            return rv;
        }
	}
}
