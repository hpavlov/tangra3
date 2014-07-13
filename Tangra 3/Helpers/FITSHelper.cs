using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

		private static Regex FITS_DATE_REGEX = new Regex("(?<DateStr>\\d\\d\\d\\d\\-\\d\\d\\-\\d\\dT\\d\\d:\\d\\d:\\d\\d(\\.\\d+)?)");
		public static void Load16BitFitsFile(string fileName, out uint[] pixelsFlat, out int width, out int height, out int bpp, out DateTime? timestamp, out double? exposure)
		{
			int pixWidth = 0;
			int pixHeight = 0;
			int pixBpp = 0;

			uint[,] pixels;
			uint medianValue;

			DateTime? fitsTimestamp = null;
			double? fitsExposure = null;

			Load16BitFitsFile(
				fileName,
				out pixels,
				out medianValue,
				delegate(BasicHDU imageHDU)
				{
					pixWidth = imageHDU.Axes[1];
					pixHeight = imageHDU.Axes[0];
					pixBpp = imageHDU.BitPix;

					try
					{
						// FITS Card definitions taken from here:
						// http://www.cyanogen.com/help/maximdl/FITS_File_Header_Definitions.htm
						// http://www.cv.nrao.edu/fits/documents/standards/year2000.txt

						bool isMidPoint = false;
						HeaderCard exposureCard = imageHDU.Header.FindCard("EXPOSURE");
						if (exposureCard == null) exposureCard = imageHDU.Header.FindCard("EXPTIME");
						if (exposureCard == null) exposureCard = imageHDU.Header.FindCard("RAWTIME");
						if (exposureCard != null && !string.IsNullOrWhiteSpace(exposureCard.Value))
						{
							fitsExposure = double.Parse(exposureCard.Value.Trim(), CultureInfo.InvariantCulture);

							string dateTimeStr = null;
							HeaderCard timeCard = imageHDU.Header.FindCard("TIMEOBS");
							HeaderCard dateCard;
							if (timeCard != null)
							{
								dateCard = imageHDU.Header.FindCard("DATEOBS");
								if (dateCard != null)
									dateTimeStr = string.Format("{0}T{1}", dateCard.Value, timeCard.Value);
							}
							else
							{
								dateCard = imageHDU.Header.FindCard("DATE-OBS");
								timeCard = imageHDU.Header.FindCard("TIME-OBS");
								if (timeCard != null && dateCard != null)
									dateTimeStr = string.Format("{0}T{1}", dateCard.Value, timeCard.Value);
								else if (dateCard != null)
									dateTimeStr = dateCard.Value;
								else
								{
									timeCard = imageHDU.Header.FindCard("MIDPOINT");
									if (timeCard != null)
									{
										dateTimeStr = timeCard.Value;
										isMidPoint = true;
									}
								}
							}

							if (!string.IsNullOrWhiteSpace(dateTimeStr))
							{
								Match regexMatch = FITS_DATE_REGEX.Match(dateTimeStr);
								if (regexMatch.Success)
								{
									DateTime startObs = DateTime.Parse(regexMatch.Groups["DateStr"].Value.Trim(), CultureInfo.InvariantCulture);
									// DATE-OBS is the start of the observation, unless given in "MIDPOINT"
									if (!isMidPoint)
										fitsTimestamp = startObs.AddSeconds(fitsExposure.Value / 2.0);
								}
							}
						}
					}
					catch (Exception ex)
					{
						Trace.WriteLine(ex.ToString());
					}

					return true;
				}
			);

			width = pixWidth;
			height = pixHeight;
			pixelsFlat = new uint[width * height];

			timestamp = fitsTimestamp;
			exposure = fitsExposure;

			uint maxPresentPixelValue = 0;
            uint mask = (uint)(((uint)1 << pixBpp) - 1);

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
                    uint val = pixels[x, y] & mask;
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
