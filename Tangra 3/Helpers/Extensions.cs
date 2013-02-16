using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Helpers;
using Tangra.VideoOperations.LightCurves;

namespace Tangra.Helpers
{
	public static class Extensions
	{
		public static void SetNUDValue(this NumericUpDown nud, double value)
		{
			if (!double.IsNaN(value))
				SetNUDValue(nud, (decimal)value);
		}

		public static void SetNUDValue(this NumericUpDown nud, int value)
		{
			SetNUDValue(nud, (decimal)value);
		}

		public static void SetNUDValue(this NumericUpDown nud, decimal value)
		{
			if (value < nud.Minimum)
				nud.Value = nud.Minimum;
			else if (value > nud.Maximum)
				nud.Value = nud.Maximum;
			else
				nud.Value = value;
		}

		public static void SetCBXIndex(this ComboBox cbx, int index)
		{
			if (cbx.Items.Count > 0)
				cbx.SelectedIndex = Math.Max(0, Math.Min(cbx.Items.Count - 1, index));
			else
				cbx.SelectedIndex = -1;
		}

        public static void DrawDataPixels(this uint[,] pixels, Graphics g, Rectangle rect, IDisplayBitmapConverter pixelToByteConverter)
        {
			pixels.DrawDataPixels(g, rect, pixelToByteConverter, -1, -1, 0, null);
        }

		public static void DrawDataPixels(this uint[,] pixels, Graphics g, Rectangle rect, IDisplayBitmapConverter pixelToByteConverter, float x0, float y0, float aperture, Pen aperturePen)
        {
            if (rect.Width != rect.Height) return;

            int coeff = rect.Width / pixels.GetLength(0);
            if (coeff == 0) return;

            int size = pixels.GetLength(0);

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    uint val = pixels[x, y];
                    //Brush brush = val <= saturation ? AllGrayBrushes.GrayBrush(val) : SaturatedBrush;                	
                	byte color = pixelToByteConverter.ToDisplayBitmapByte(val);
                    g.FillRectangle(AllGrayBrushes.GrayBrush(color), rect.Left + x * coeff, rect.Top + y * coeff, coeff, coeff);
                }
            }

            if (aperture != 0)
                g.DrawEllipse(
                    aperturePen,
                    rect.Left + (x0 - aperture + 0.5f) * coeff, rect.Top + (y0 - aperture + 0.5f) * coeff,
                    aperture * 2 * coeff, aperture * 2 * coeff);
		}
	}
}
