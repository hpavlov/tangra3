using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Context;
using Tangra.Model.Image;
using Tangra.PInvoke;
using nom.tam.fits;
using nom.tam.util;

namespace Tangra.Helpers
{
	public static class FITSHelper
	{
		public delegate bool CheckOpenedFitsFileCallback(BasicHDU imageHDU);

		public static bool Load16BitFitsFile(string fileName, out uint[,] pixels, out uint medianValue, CheckOpenedFitsFileCallback callback)
		{
			Fits fitsFile = new Fits();

			using (BufferedFile bf = new BufferedFile(fileName, FileAccess.Read, FileShare.ReadWrite))
			{
				fitsFile.Read(bf);

				BasicHDU imageHDU = fitsFile.GetHDU(0);

				if (callback != null && !(callback(imageHDU)))
				{
					pixels = new uint[0,0];
					medianValue = 0;
					return false;
				}

				pixels = Load16BitImageData((Array)imageHDU.Data.DataArray, imageHDU.Axes[0], imageHDU.Axes[1], out medianValue);
				return true;
			}				
		}

		public static void Load16BitFitsFile(string fileName, out uint[] pixelsFlat, out int width, out int height, out int bpp)
		{

			int pixWidth = 0;
			int pixHeight = 0;
			int pixBpp = 0;

			uint[,] pixels;
			uint medianValue;

			Load16BitFitsFile(
				fileName,
				out pixels,
				out medianValue,
				delegate(BasicHDU imageHDU)
				{
					pixWidth = imageHDU.Axes[1];
					pixHeight = imageHDU.Axes[0];
					pixBpp = imageHDU.BitPix;

					return true;
				}
			);

			width = pixWidth;
			height = pixHeight;			
			pixelsFlat = new uint[width * height];

			uint maxPresentPixelValue = 0;

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					uint val = pixels[x, y];
					pixelsFlat[x + y * width] = val;
					if (maxPresentPixelValue < val)
						maxPresentPixelValue = val;
				}
			}

			if (maxPresentPixelValue < 256)
				bpp = 8;
			else if (maxPresentPixelValue < 4096)
				bpp = 12;
			else if (maxPresentPixelValue < 16384)
				bpp = 14;
			else
				bpp = pixBpp;			
		}

		private static uint[,] Load16BitImageData(Array dataArray, int height, int width, out uint medianValue)
		{
			var medianCalcList = new List<uint>();

			uint[,] data = new uint[width, height];

			for (int y = 0; y < height; y++)
			{
				short[] dataRow = (short[])dataArray.GetValue(y);

				for (int x = 0; x < width; x++)
				{
					uint val = (uint)dataRow[x];

					data[x, y] = val;
					medianCalcList.Add(val);
				}
			}

			if (medianCalcList.Count > 0)
			{
				medianCalcList.Sort();

				if (medianCalcList.Count % 2 == 1)
					medianValue = medianCalcList[medianCalcList.Count / 2];
				else
					medianValue = (medianCalcList[medianCalcList.Count / 2] + medianCalcList[1 + (medianCalcList.Count / 2)]) / 2;
			}
			else
				medianValue = 0;

			return data;
		}

	}
}
