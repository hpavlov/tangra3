using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.Model.Astro;
using Tangra.Model.Context;
using Tangra.Model.Image;
using Tangra.Model.VideoOperations;
using Tangra.VideoOperations.LightCurves.Tracking;


namespace Tangra.VideoOperations.LightCurves
{
    public enum LightCurvesState
    {
        SelectTrackingStars,
        SelectMeasuringStars,
        ReadyToRun,
        Running,
        SelectingFrameTimes,
        Viewing   
    }

    internal class LCState
    {
        protected LCStateMachine Context;

        protected LCState(LCStateMachine context)
        {
            Context = context;
        }

        public virtual void Initialize()
        { }

        public virtual void ObjectSelected(TrackedObjectConfig selectedObject, bool shift, bool ctrl)
        { }

        public virtual bool IsNewObject(ImagePixel star, bool shift, bool ctrl, ref int newOrExistingObjectId)
		{
			return false;
		}   
    }

    internal class LCStateMachine
    {
        internal LCState m_CurrentStateObject;

        internal List<TrackedObjectConfig> m_MeasuringStars = new List<TrackedObjectConfig>();
        internal List<float> m_MeasuringApertures = new List<float>();
        internal List<int> m_PsfFitMatrixSizes = new List<int>();
        internal LightCurvesState m_CurrentState;

        internal ReduceLightCurveOperation VideoOperation;

        private IVideoController m_VideoController;

        internal int m_ConfiguringFrame;
        internal bool m_HasBeenPaused = false;

        public LCStateMachine(ReduceLightCurveOperation videoOperation, IVideoController videoController)
        {
            m_VideoController = videoController;
            VideoOperation = videoOperation;

            m_CurrentStateObject = new LCStateSelectMeasuringStars(this);
            m_CurrentState = LightCurvesState.SelectMeasuringStars;
            m_CurrentStateObject.Initialize();
        }

        public void SetCanPlayVideo(bool canPlayVideo)
        {
            TangraContext.Current.CanPlayVideo = canPlayVideo;
            m_VideoController.UpdateViews();
        }

        internal int m_SelectedMeasuringStar = -1;

        private bool m_DontSavePixelData = false;

        public bool DontSavePixelData
        {
            get { return m_DontSavePixelData; }
            set { m_DontSavePixelData = value; }
        }

        public int SelectedMeasuringStar
        {
            get { return m_SelectedMeasuringStar; }
            set { m_SelectedMeasuringStar = value; }
        }

        public LightCurvesState CurrentState
        {
            get { return m_CurrentState; }
        }

        public List<TrackedObjectConfig> MeasuringStars
        {
            get { return m_MeasuringStars; }
        }

        public List<float> MeasuringApertures
        {
            get { return m_MeasuringApertures; }
        }

        public List<int> PsfFitMatrixSizes
        {
            get { return m_PsfFitMatrixSizes; }
        }

        public ImagePixel SelectedObject { get; set; }

        public PSFFit SelectedObjectGaussian { get; set; }

        public int SelectedObjectFrameNo { get; set; }

        public void ObjectSelected(TrackedObjectConfig selectedObject, bool shift, bool ctrl)
        {
            m_CurrentStateObject.ObjectSelected(selectedObject, shift, ctrl);
        }

		public void ObjectEdited(TrackedObjectConfig selectedObject)
		{
			m_CurrentStateObject.ObjectSelected(selectedObject, false, false);
		}

        public bool IsNewObject(ImagePixel star, bool shift, bool ctrl, ref int newOrExistingObjectId)
        {
            return m_CurrentStateObject.IsNewObject(star, shift, ctrl, ref newOrExistingObjectId);
        }

        public LCState ChangeState(LightCurvesState newState)
        {
            if (newState == LightCurvesState.SelectMeasuringStars)
            {
                m_CurrentState = LightCurvesState.SelectMeasuringStars;
                m_CurrentStateObject = new LCStateSelectMeasuringStars(this);
            }
            else if (newState == LightCurvesState.ReadyToRun)
            {
                m_CurrentState = LightCurvesState.ReadyToRun;
                m_CurrentStateObject = new LCStateReadyToRun(this);
            }
            else if (newState == LightCurvesState.Running)
            {
                m_CurrentState = LightCurvesState.Running;
                m_CurrentStateObject = new LCStateRunning(this);
            }
            else if (newState == LightCurvesState.SelectingFrameTimes)
            {
                // HACK: Don't change the m_CurrentStateObject 
                m_CurrentState = LightCurvesState.SelectingFrameTimes;
            }
            else if (newState == LightCurvesState.Viewing)
            {
                m_CurrentState = LightCurvesState.Viewing;
                m_CurrentStateObject = new LCStateViewingLightCurve(this);
            }

            m_CurrentStateObject.Initialize();
            return m_CurrentStateObject;
        }
    }
}
