using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.VideoOperations.LightCurves;
using Tangra.Model.ImageTools;
using System.Windows.Forms;
using Tangra.VideoOperations.LightCurves.Tracking;
using System.Drawing;
using Tangra.Resources;
using Tangra.Controller;

namespace Tangra.ImageTools
{
	public class CorrectTrackingTool : ImageTool
	{
		public enum CorrectTrackingState
		{
			Normal,
			Correcting
		}

		public enum CorrectTrackingMode
		{
			All,
			Target1,
			Target2,
			Target3,
			Target4
		}

		private CorrectTrackingState m_State;
		private CorrectTrackingMode m_Mode;

		private ReduceLightCurveOperation m_LightCurvesVideoOperation;
		private VideoController m_VideoController;
		private ITracker m_Tracker;

		private int m_X0;
		private int m_Y0;

		internal ITracker Tracker
		{
			get { return m_Tracker; }
		}

		internal void Initialize(ReduceLightCurveOperation videoOperation, ITracker tracker, VideoController videoController)
		{
			m_LightCurvesVideoOperation = videoOperation;
			m_VideoController = videoController;
			m_Tracker = tracker;

			if (m_Tracker.SupportsManualCorrections)
				m_VideoController.SetPictureBoxCursor(CustomCursors.PanCursor);

			m_State = CorrectTrackingState.Normal;
			m_Mode = CorrectTrackingMode.All;

			for (int i = 0; i < tracker.TrackedObjects.Count; i++)
			{
				m_LightCurvesVideoOperation.SetManualTrackingCorrection(i, 0, 0);	
			}			
		}

		internal bool SupportsManualCorrections
		{
			get { return m_Tracker.SupportsManualCorrections; }
		}

		internal void SetCorrectionMode(CorrectTrackingMode newMode)
		{
			m_Mode = newMode;
		}

		public override void Activate()
		{
			// NOTE: The actual activation is done on Initialize()
		}

		public override void Deactivate()
		{
			base.Deactivate();

			if (m_VideoController != null)
				m_VideoController.SetPictureBoxCursor(Cursors.Arrow);
		}

		public override void MouseDown(Point location)
		{
			if (m_Tracker.SupportsManualCorrections && m_State == CorrectTrackingState.Normal)
			{
				m_State = CorrectTrackingState.Correcting;
				m_VideoController.SetPictureBoxCursor(CustomCursors.PanEnabledCursor);

				m_X0 = location.X;
				m_Y0 = location.Y;
			}
		}

		public override void MouseMove(Point location)
		{
			if (m_Tracker.SupportsManualCorrections && m_State == CorrectTrackingState.Correcting)
			{
				int deltaX = location.X - m_X0;
				int deltaY = location.Y - m_Y0;

				// If correcting see if there is a location in a radius of a few pixels which fits very well with the current targets

				if (m_Mode == CorrectTrackingMode.All)
				{
					for (int i = 0; i < m_Tracker.TrackedObjects.Count; i++)
					{
						m_LightCurvesVideoOperation.SetManualTrackingCorrection(i, deltaX, deltaY);
					}					
				}
				else
					m_LightCurvesVideoOperation.SetManualTrackingCorrection((int)m_Mode - 1, deltaX, deltaY);
				
				m_VideoController.RefreshCurrentFrame();
			}
		}

		public override void MouseUp(Point location)
		{
			if (m_Tracker.SupportsManualCorrections)
				m_VideoController.SetPictureBoxCursor(CustomCursors.PanCursor);

			m_State = CorrectTrackingState.Normal;
		}

		public override void MouseLeave()
		{
			if (m_Tracker.SupportsManualCorrections)
				m_VideoController.SetPictureBoxCursor(CustomCursors.PanCursor);

			m_State = CorrectTrackingState.Normal;
		}

		public override ZoomImageBehaviour ZoomBehaviour
		{
			get
			{
				return ZoomImageBehaviour.None;
			}
		}
	}
}
