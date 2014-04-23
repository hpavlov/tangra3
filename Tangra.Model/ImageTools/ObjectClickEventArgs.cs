using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Astro;
using Tangra.Model.Image;

namespace Tangra.Model.ImageTools
{
    public class ObjectClickEventArgs
    {
        public readonly Point ClickLocation;
        public readonly ImagePixel Pixel;
        public readonly PSFFit Gausian;
        public readonly bool Shift;
        public readonly bool Control;

        public ObjectClickEventArgs(ImagePixel pixel, PSFFit gausian, Point location, bool shiftHeld, bool ctrlHeld)
        {
            ClickLocation = location;
            Pixel = pixel;
            Gausian = gausian;
            Shift = shiftHeld;
            Control = ctrlHeld;
        }
    }
}
