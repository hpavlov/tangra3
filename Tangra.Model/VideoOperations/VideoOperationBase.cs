using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.ImageTools;

namespace Tangra.Model.VideoOperations
{
    public class VideoOperationBase 
    {
        public virtual void MouseLeave() { }
        public virtual void MouseMove(Point location) { }
        public virtual void MouseClick(ObjectClickEventArgs e) { }
        public virtual void MouseDown(Point location) { }
        public virtual void MouseUp(Point location) { }
        public virtual void MouseDoubleClick(Point location) { }
    }
}
