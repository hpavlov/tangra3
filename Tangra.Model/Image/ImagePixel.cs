using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.Model.Image
{
	public interface IImagePixel
	{
		int X { get; }
		int Y { get; }
		double XDouble { get; }
		double YDouble { get; }
		bool IsSpecified { get; }
		int Brightness { get; }
	}

    public class ImagePixel : IImagePixel
    {
		public int X { get; private set; }
		public int Y { get; private set; }
		public double XDouble { get; private set; }
		public double YDouble { get; private set; }

		public int Brightness { get; private set; }

        /// <summary>
        /// This needs to be set by the code that created the ImagePixel
        /// </summary>
        public double SignalNoise;

        public static ImagePixel Unspecified = new ImagePixel(int.MinValue, double.NaN, double.NaN);

		public ImagePixel(IImagePixel clone)
            : this(clone.Brightness, clone.XDouble, clone.YDouble)
		{ }

    	public ImagePixel(ImagePixel clone)
            : this(clone.Brightness, clone.XDouble, clone.YDouble)
		{ }

        public ImagePixel(double x, double y)
            : this(int.MinValue, x, y)
        { }

        public ImagePixel(int brightness, double x, double y)
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
