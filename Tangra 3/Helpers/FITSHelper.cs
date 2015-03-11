/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

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


			    uint bzero = 0;
                HeaderCard bZeroCard = imageHDU.Header.FindCard("BZERO");
			    if (bZeroCard != null)
			    {
                    try
                    {
                        bzero = (uint)double.Parse(bZeroCard.Value);                        
                    }
                    catch
			        { }
			    }

			    pixels = Load16BitImageData((Array)imageHDU.Data.DataArray, imageHDU.Axes[0], imageHDU.Axes[1], bzero, out medianValue);
				return true;
			}
		}

        public static DateTime? ParseExposure(Header header, out bool isMidPoint, out double? fitsExposure)
        {
            // FITS Card definitions taken from here:
            // http://www.cyanogen.com/help/maximdl/FITS_File_Header_Definitions.htm
            // http://www.cv.nrao.edu/fits/documents/standards/year2000.txt

            isMidPoint = false;
            fitsExposure = null;

            HeaderCard exposureCard = header.FindCard("EXPOSURE");
            if (exposureCard == null) exposureCard = header.FindCard("EXPTIME");
            if (exposureCard == null) exposureCard = header.FindCard("RAWTIME");
            if (exposureCard != null && !string.IsNullOrWhiteSpace(exposureCard.Value))
            {
                fitsExposure = double.Parse(exposureCard.Value.Trim(), CultureInfo.InvariantCulture);

                string dateTimeStr = null;
                HeaderCard timeCard = header.FindCard("TIMEOBS");
                HeaderCard dateCard;
                if (timeCard != null)
                {
                    dateCard = header.FindCard("DATEOBS");
                    if (dateCard != null)
                        dateTimeStr = string.Format("{0}T{1}", dateCard.Value, timeCard.Value);
                }
                else
                {
                    dateCard = header.FindCard("DATE-OBS");
                    timeCard = header.FindCard("TIME-OBS");
                    if (timeCard != null && dateCard != null)
                        dateTimeStr = string.Format("{0}T{1}", dateCard.Value, timeCard.Value);
                    else if (dateCard != null)
                        dateTimeStr = dateCard.Value;
                    else
                    {
                        timeCard = header.FindCard("MIDPOINT");
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
                            return startObs.AddSeconds(fitsExposure.Value/2.0);
                        else
                            return startObs;
                    }
                }
            }

            return null;
        }

		private static Regex FITS_DATE_REGEX = new Regex("(?<DateStr>\\d\\d\\d\\d\\-\\d\\d\\-\\d\\dT\\d\\d:\\d\\d:\\d\\d(\\.\\d+)?)");
        public static void Load16BitFitsFile(string fileName, out uint[] pixelsFlat, out int width, out int height, out int bpp, out DateTime? timestamp, out double? exposure, out uint minPixelValue, out uint maxPixelValue)
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
						bool isMidPoint = false;
                        fitsTimestamp = ParseExposure(imageHDU.Header, out isMidPoint, out fitsExposure);
					    
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

            maxPixelValue = 0;
            minPixelValue = uint.MaxValue;
            uint mask = (uint)(((uint)1 << pixBpp) - 1);

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
                    uint val = pixels[x, y] & mask;
					pixelsFlat[x + y * width] = val;
                    if (maxPixelValue < val) maxPixelValue = val;
                    if (minPixelValue > val) minPixelValue = val;
				}
			}

			// TODO: If data type is float or double, then what bpp should be reported? 

            if (maxPixelValue < 256)
				bpp = 8;
            else if (maxPixelValue < 4096)
				bpp = 12;
            else if (maxPixelValue < 16384)
				bpp = 14;
			else
				bpp = pixBpp;
		}

		private static uint[,] Load16BitImageData(Array dataArray, int height, int width, uint bzero , out uint medianValue)
		{
			var medianCalcList = new List<uint>();

			uint[,] data = new uint[width, height];

			for (int y = 0; y < height; y++)
			{
				object dataRowObject = dataArray.GetValue(y);

				short[] dataRow;
				if (dataRowObject is short[])
					dataRow = (short[])dataRowObject;
				else if (dataRowObject is Array)
				{
					Array arr = (Array) dataRowObject;
					dataRow = new short[arr.Length];
					for (int i = 0; i < arr.Length; i++)
					{
						dataRow[i] = (short)Convert.ToInt32(arr.GetValue(i));
					}
				}
				else
					throw new ArrayTypeMismatchException();

				for (int x = 0; x < width; x++)
				{
                    uint val = (uint)(bzero + (int)dataRow[x]);

                    data[x, height - y - 1] = val;
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
