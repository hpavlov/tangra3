/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using Tangra.Model.Config;
using Tangra.Model.Helpers;
using Tangra.Video;
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

		public static void DrawDataPixels(this uint[,] pixels, Graphics g, Rectangle rect, int bpp, uint normVal, float x0, float y0, float aperture, Pen aperturePen)
        {
            if (rect.Width != rect.Height) return;

            int coeff = rect.Width / pixels.GetLength(0);
            if (coeff == 0) return;

            int size = pixels.GetLength(0);

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
	                double z = pixels[x, y];

					if (bpp == 16 && normVal == 0)
						z = z * 255.0f / 65535;
					else if (bpp == 14)
						z = z * 255.0f / 16383;
					else if (bpp == 12)
						z = z * 255.0f / 4095;
					else if (bpp == 16 && normVal > 0)
						z = z * 255.0f / normVal;

					byte color = (byte)(Math.Max(0, Math.Min(255, z)));

                    g.FillRectangle(AllGrayBrushes.GrayBrush(color), rect.Left + x * coeff, rect.Top + y * coeff, coeff, coeff);
                }
            }

            if (aperture != 0)
                g.DrawEllipse(
                    aperturePen,
                    rect.Left + (x0 - aperture + 0.5f) * coeff, rect.Top + (y0 - aperture + 0.5f) * coeff,
                    aperture * 2 * coeff, aperture * 2 * coeff);
		}

		public static string XmlSerialize<TType>(this TType instance) where TType: class
		{
			var ser = new XmlSerializer(typeof(TType));
			var xmlSer = new StringBuilder();

			if (instance != null)
			{
				using (TextWriter wrt = new StringWriter(xmlSer))
				{
					ser.Serialize(wrt, instance);
					wrt.Flush();
				}
			}

			return xmlSer.ToString();
		}

        public static void Serialize<T>(this object instance, Stream stream)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            xmlSerializer.Serialize(stream, instance);
        }

        public static string AsSerialized(this object instance)
        {
            var xmlSerializer = new XmlSerializer(instance.GetType());
            StringBuilder output = new StringBuilder();
            using (StringWriter wrt = new StringWriter(output))
            {
                xmlSerializer.Serialize(wrt, instance);
                wrt.Flush();
            }
            return output.ToString();
        }

        public static T Deserialize<T>(this string serializedXmlString)
        {
            using (var reader = new StringReader(serializedXmlString))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(reader);
            }
        }

        public static T Deserialize<T>(XmlNode rootNode)
        {
            using (var reader = new XmlNodeReader(rootNode))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(reader);
            }
        }

        public static string Serialize<T>(this object instance)
        {
            using (var memStr = new StringWriter())
            {
                var xmlSerializer = new XmlSerializer(typeof(T));
                xmlSerializer.Serialize(memStr, instance);
                return memStr.ToString();
            }
        }

	    public static string GetTextDescription(this ReInterlaceMode mode)
	    {
            if (mode == ReInterlaceMode.SwapFields)
                return "FieldSwap";
            else if (mode == ReInterlaceMode.ShiftOneField)
            {
                return "FieldShift";
            }
            else if (mode == ReInterlaceMode.SwapAndShiftOneField)
            {
                return "FieldSwapAndShift";
            }
            else
            {
                return null;
            }
	    }
	}
}
