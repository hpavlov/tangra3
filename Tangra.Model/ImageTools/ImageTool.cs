using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.VideoOperations;

namespace Tangra.Model.ImageTools
{
    public enum ZoomImageBehaviour
    {
        None,
        DisplayCentroid,
        Custom
    }

    public abstract class ImageTool
    {
        protected IVideoOperation m_VideoOperation;
		protected IImageToolView m_ImageToolView;

	    public int MouseX;
		public int MouseY;

        public bool TrySwitchTo(IVideoOperation videoOperation, IImageToolView imageToolView, ImageTool oldTool)
        {
            if (!CanSwitchNow()) return false;

            if (oldTool != null)
                oldTool.Deactivate();

			m_VideoOperation = videoOperation;
			m_ImageToolView = imageToolView;

            Activate();

            return true;
        }

        public static ImageTool SwitchTo<T>(IVideoOperation videoOperation, IImageToolView imageToolView, ImageTool oldTool)
            where T : ImageTool, new()
        {
            ImageTool instance = new T();
			instance.TrySwitchTo(videoOperation, imageToolView, oldTool);

            return instance;
        }

        protected virtual bool CanSwitchNow()
        {
            return true;
        }

        public virtual void Activate()
        {
            
        }

        public virtual void Deactivate()
        {
	        MouseX = -1;
			MouseY = -1;

			m_ImageToolView.Update(this);
        }

        public virtual void MouseLeave()
        {
			MouseX = -1;
			MouseY = -1;

			m_ImageToolView.Update(this);        
        }
        
		public virtual void MouseMove(Point location)
		{
			MouseX = location.X;
			MouseY = location.Y;

			m_ImageToolView.Update(this);
		}

        public virtual void MouseClick(ObjectClickEventArgs e) { }
        public virtual void MouseDown(Point location) { }
        public virtual void MouseUp(Point location) { }
        public virtual void MouseDoubleClick(Point location) { }
        public virtual void OnNewFrame(int currentFrameIndex, bool isLastFrame) { }

        public virtual ZoomImageBehaviour ZoomBehaviour
        {
            get { return ZoomImageBehaviour.DisplayCentroid; }
        }
    }
}
