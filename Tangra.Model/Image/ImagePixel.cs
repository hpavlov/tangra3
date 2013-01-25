using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.Model.Image
{
    public class ImagePixel
    {
        public readonly int X;
        public readonly int Y;
        public readonly double XDouble;
        public readonly double YDouble;
        public uint Brightness;

        /// <summary>
        /// This needs to be set by the code that created the ImagePixel
        /// </summary>
        public double SignalNoise;

        public static ImagePixel Unspecified = new ImagePixel(uint.MinValue, double.NaN, double.NaN);

    	public ImagePixel(ImagePixel clone)
            : this(clone.Brightness, clone.XDouble, clone.YDouble)
		{ }

        public ImagePixel(double x, double y)
            : this(uint.MinValue, x, y)
        { }

        public ImagePixel(uint brightness, double x, double y)
        {
            XDouble = x;
            YDouble = y;
            Brightness = brightness;

            X = (int)Math.Round(XDouble);
            Y = (int)Math.Round(YDouble);
        }

        public bool IsSpecified
        {
            get
            {
                return !double.IsNaN(XDouble) && !double.IsNaN(XDouble);
            }
        }

        public override int GetHashCode()
        {
            return X * 10000 + Y;
        }

        public override bool Equals(object obj)
        {
            if (obj == null ||
                !(obj is ImagePixel))
                return false;

            return (obj as ImagePixel).GetHashCode() == this.GetHashCode();
        }

        public static bool operator ==(ImagePixel a, ImagePixel b)
        {
            if ((object)a == null && (object)b == null)
                return true;

            if ((object)a == null) return false;

            return a.Equals(b);
        }

        public static bool operator !=(ImagePixel a, ImagePixel b)
        {
            return !(a == b);
        }

        public static double ComputeDistance(double x1, double x2, double y1, double y2)
        {
            return Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
        }
    }
}
