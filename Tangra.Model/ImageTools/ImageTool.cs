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
        //protected IImageHostCallbacks m_Host;

        public bool TrySwitchTo(IVideoOperation videoOperation, ImageTool oldTool)
        {
            m_VideoOperation = videoOperation;

            if (!CanSwitchNow()) return false;

            if (oldTool != null)
                oldTool.Deactivate();

            Activate();

            return true;
        }

        public static ImageTool SwitchTo<T>(IVideoOperation videoOperation, ImageTool oldTool)
            where T : ImageTool, new()
        {
            ImageTool instance = new T();
            instance.TrySwitchTo(videoOperation, oldTool);

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
            
        }

        public virtual void MouseLeave() { }
        public virtual void MouseMove(Point location) { }
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
