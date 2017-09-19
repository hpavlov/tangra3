/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Tangra.Model.Image;
using nom.tam.fits;
using nom.tam.util;
using Tangra.Model.Helpers;

namespace Tangra.Model.Helpers
{
	public static class FITSHelper
	{
		public delegate bool CheckOpenedFitsFileCallback(BasicHDU imageHDU);

        public delegate void LoadFitsDataCallback<TData>(Array dataArray, int height, int width, double bzero, out TData[,] pixels, out TData medianValue, out Type pixelDataType, out bool hasNegPix, out short minRawValue, out uint maxVal);

        public static bool LoadFloatingPointFitsFile(string fileName, IFITSTimeStampReader timeStampReader, out float[,] pixels, out float medianValue, out Type pixelDataType, out float exposureSeconds, out bool hasNegativePixels, CheckOpenedFitsFileCallback callback)
        {
            short minRawValue_discarded;
            uint maxVal_discarded;
            return LoadFitsFileInternal<float>(
                fileName, timeStampReader, out pixels, out medianValue, out pixelDataType, out exposureSeconds, out hasNegativePixels, out minRawValue_discarded, out maxVal_discarded, callback, 
                (Array dataArray, int height, int width, double bzero, out float[,] ppx, out float median, out Type dataType, out bool hasNegPix, out short minV, out uint maxV) =>
			{
                float minFV_discarded;
                float maxFV_discarded;
                ppx = LoadFloatImageData(dataArray, height, width, (float)bzero, out median, out dataType, out hasNegPix, out minFV_discarded, out maxFV_discarded);
			    minV = 0;
			    maxV = 0;
			});
		}

        public delegate bool Load16BitFitsFileCallback(string fileName, IFITSTimeStampReader timeStampReader, out uint[,] pixels, out int width, out int height, out uint medianValue, out Type pixelDataType, out bool hasNegativePixels, out short minRawValue, out uint maxValue, CheckOpenedFitsFileCallback callback);

        public static bool Load16BitFitsFile(string fileName, IFITSTimeStampReader timeStampReader, out uint[,] pixels, out uint medianValue, out Type pixelDataType, out bool hasNegativePixels, out short minRawValue, out uint maxValue, CheckOpenedFitsFileCallback callback)
		{
		    float exposureSeconds;
            return LoadFitsFileInternal<uint>(fileName, timeStampReader, out pixels, out medianValue, out pixelDataType, out exposureSeconds, out hasNegativePixels, out minRawValue, out maxValue, callback, 
                (Array dataArray, int height, int width, double bzero, out uint[,] ppx, out uint median, out Type dataType, out bool hasNegPix, out short minV, out uint maxV) =>
				{
                    ppx = Load16BitImageData(dataArray, height, width, (int)bzero, out median, out dataType, out hasNegPix, out minV, out maxV);
				});
		}

        private static bool LoadFitsFileInternal<TData>(
            string fileName, IFITSTimeStampReader timeStampReader, 
            out TData[,] pixels, out TData medianValue, out Type pixelDataType, out float frameExposure, out bool hasNegativePixels, out short minRawValue, out uint maxVal,
            CheckOpenedFitsFileCallback callback, LoadFitsDataCallback<TData> loadDataCallback)
		{
			Fits fitsFile = new Fits();

			using (BufferedFile bf = new BufferedFile(fileName, FileAccess.Read, FileShare.ReadWrite))
			{
				fitsFile.Read(bf);

				BasicHDU imageHDU = fitsFile.GetHDU(0);
			    Array pixelsArray = (Array) imageHDU.Data.DataArray;
                return LoadFitsDataInternal<TData>(
                    imageHDU, pixelsArray, fileName, timeStampReader, 
                    out pixels, out medianValue, out pixelDataType, out frameExposure, out hasNegativePixels, out minRawValue, out maxVal,
                    callback, loadDataCallback);
			}
		}

        public static int GetBZero(BasicHDU imageHDU)
	    {
            int bzero = 0;
            HeaderCard bZeroCard = imageHDU.Header.FindCard("BZERO");
            if (bZeroCard != null && !string.IsNullOrWhiteSpace(bZeroCard.Value))
            {
                try
                {
                    bzero = (int)double.Parse(bZeroCard.Value.Replace(',', '.'), CultureInfo.InvariantCulture);
                }
                catch
                { }
            }

            return bzero;
	    }

        public static bool LoadFitsDataInternal<TData>(
            BasicHDU imageHDU, Array pixelsArray, string fileName, IFITSTimeStampReader timeStampReader, 
            out TData[,] pixels, out TData medianValue, out Type pixelDataType, out float frameExposure, out bool hasNegativePixels,  out short minRawValue, out uint maxVal,
            CheckOpenedFitsFileCallback callback, LoadFitsDataCallback<TData> loadDataCallback)
	    {
            hasNegativePixels = false;

            if (callback != null && !(callback(imageHDU)))
            {
                pixels = new TData[0, 0];
                medianValue = default(TData);
                pixelDataType = typeof(TData);
                frameExposure = 0;
                minRawValue = 0;
                maxVal = 0;
                return false;
            }

            int bzero = GetBZero(imageHDU);

            bool isMidPoint;
            double? fitsExposure = null;
            try
            {
                ParseExposure(fileName, imageHDU.Header, timeStampReader, out isMidPoint, out fitsExposure);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
            frameExposure = fitsExposure.HasValue ? (float)fitsExposure.Value : 0;

            loadDataCallback(pixelsArray, imageHDU.Axes[0], imageHDU.Axes[1], bzero, out pixels, out medianValue, out pixelDataType, out hasNegativePixels, out minRawValue, out maxVal);

            return true;
	    }

        public static DateTime? ParseExposure(string fileName, Header header, IFITSTimeStampReader timeStampReader, out bool isMidPoint, out double? fitsExposure)
	    {
	        if (timeStampReader == null)
                return ParseExposureInternal(header, out isMidPoint, out fitsExposure);
            else
                return timeStampReader.ParseExposure(fileName, header, out isMidPoint, out fitsExposure);
	    }

	    private static DateTime? ParseExposureInternal(Header header, out bool isMidPoint, out double? fitsExposure)
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
                double doubleNum;
                if (double.TryParse(exposureCard.Value.Trim().Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out doubleNum))
                    fitsExposure = doubleNum;
                else
                    throw new FormatException(string.Format("Cannot parse FITS exposure '{0}' as a floating point number.", exposureCard.Value));
            }
                

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
                    if (!isMidPoint && fitsExposure.HasValue)
                        return startObs.AddSeconds(fitsExposure.Value / 2.0);
                    else
                        return startObs;
                }
            }

            return null;
        }

	    public enum FitsCubeType
	    {
	        NotACube,
            ThreeAxisCube,
            MultipleHDUsCube
	    }

        public static FitsCubeType GetFitsCubeType(string fileName)
	    {
            Fits fitsFile = new Fits();

            using (BufferedFile bf = new BufferedFile(fileName, FileAccess.Read, FileShare.ReadWrite))
            {
                fitsFile.Read(bf);

                BasicHDU imageHDU = fitsFile.GetHDU(0);
                if (fitsFile.NumberOfHDUs == 1 && imageHDU.Axes.Length == 3)
                    return FitsCubeType.ThreeAxisCube;

                if (fitsFile.NumberOfHDUs > 1 && imageHDU.Axes.Length == 2)
                    return FitsCubeType.MultipleHDUsCube;
            }

            return FitsCubeType.NotACube;
	    }

		private static Regex FITS_DATE_REGEX = new Regex("(?<DateStr>\\d\\d\\d\\d\\-\\d\\d\\-\\d\\dT\\d\\d:\\d\\d:\\d\\d(\\.\\d+)?)");
        public static void Load16BitFitsFile(
            string fileName, Load16BitFitsFileCallback loadFitsFileCallback, IFITSTimeStampReader timeStampReader, Action<BasicHDU> fitsFileLoadedCallback,
            out uint[] pixelsFlat, out int width, out int height, out int bpp, out DateTime? timestamp, 
            out double? exposure, out short minPixelValue, out uint maxPixelValue, out bool hasNegativePixels)
		{
			int pixWidth = 0;
			int pixHeight = 0;
			int pixBpp = 0;

			uint[,] pixels;
			uint medianValue;

			DateTime? fitsTimestamp = null;
			double? fitsExposure = null;
	        Type pixelDataType = null;

            CheckOpenedFitsFileCallback checkFileCallback = delegate(BasicHDU imageHDU)
            {
                pixWidth = imageHDU.Axes[1];
                pixHeight = imageHDU.Axes[0];
                pixBpp = Math.Abs(imageHDU.BitPix); /* float and double are -32 and -64 respectively */

                try
                {
                    bool isMidPoint = false;
                    fitsTimestamp = ParseExposure(fileName, imageHDU.Header, timeStampReader, out isMidPoint, out fitsExposure);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.ToString());
                }

                if (fitsFileLoadedCallback != null)
                    fitsFileLoadedCallback(imageHDU);

                return true;
            };

            if (loadFitsFileCallback != null)
            {
                int overwrittenHeight;
                int overwrittenWidth;

                loadFitsFileCallback(
                    fileName,
                    timeStampReader,
                    out pixels,
                    out overwrittenWidth,
                    out overwrittenHeight,
                    out medianValue,
                    out pixelDataType,
                    out hasNegativePixels,
                    out minPixelValue,
                    out maxPixelValue,
                    checkFileCallback
                );

                width = overwrittenWidth;
                height = overwrittenHeight;
            }
            else
            {
                Load16BitFitsFile(
                    fileName,
                    timeStampReader,
                    out pixels,
                    out medianValue,
                    out pixelDataType,
                    out hasNegativePixels,
                    out minPixelValue,
                    out maxPixelValue,
                    checkFileCallback
                );

                width = pixWidth;
                height = pixHeight;
            }

			pixelsFlat = new uint[width * height];

			timestamp = fitsTimestamp;
			exposure = fitsExposure;

            uint mask = (uint)(((uint)1 << pixBpp) - 1);            
            if (pixelDataType == typeof (float) || pixelDataType == typeof (double))
            {
                mask = 0;
                pixBpp = 16; // Pretending float data is 16 bit. This is the maximum currently supported by Tangra
            }

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
                    uint val = pixels[x, y];
                    if (mask > 0) val = val & mask;
					pixelsFlat[x + y * width] = val;
				}
			}

            bpp = GetBppForMaxPixelValue(maxPixelValue);

            if (bpp == 32)
            {
                int shift = Math.Max(0, (int)Math.Ceiling(pixBpp / 16.0) - 1);

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        pixelsFlat[x + y * width] = pixelsFlat[x + y * width] >> shift;
                    }
                }

                bpp = 16;
            }	
		}

	    public static int GetBppForMaxPixelValue(uint maxPixelValue)
	    {
	        if (maxPixelValue < 256)
				return 8;
            else if (maxPixelValue < 4096)
				return 12;
            else if (maxPixelValue < 16384)
				return 14;
            else if (maxPixelValue < 65535)
                return 16;
            else
                return 32;
	    }

        private static float[,] LoadFloatImageData(Array dataArray, int height, int width, float bzero, out float medianValue, out Type dataType, out bool hasNegPix, out float minRawValue, out float maxValue)
		{
			var medianCalcList = new List<float>();

			float[,] data = new float[width, height];

			dataType = null;
            hasNegPix = false;
            minRawValue = float.MaxValue;
            maxValue = 0;

			for (int y = 0; y < height; y++)
			{
				object dataRowObject = dataArray.GetValue(y);

				float[] dataRow;
				if (dataRowObject is float[])
				{
					dataRow = (float[]) dataRowObject;
					dataType = typeof(float);
				}
				else if (dataRowObject is Array)
				{
					Array arr = (Array) dataRowObject;
					dataRow = new float[arr.Length];
					for (int i = 0; i < arr.Length; i++)
					{
						if (dataType == null) dataType = arr.GetValue(i).GetType();
						dataRow[i] = (float) Convert.ToSingle(arr.GetValue(i));
					}
				}
				else
					throw new ArrayTypeMismatchException();

				for (int x = 0; x < width; x++)
				{
				    if (minRawValue > dataRow[x]) minRawValue = dataRow[x];

                    float val = (float)(bzero + dataRow[x]);
				    if (!hasNegPix && val < 0) hasNegPix = true;

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

			    maxValue = medianCalcList[medianCalcList.Count - 1];
			}
			else
				medianValue = 0;

			return data;

		}

		public static uint[,] Load16BitImageData(
            Array dataArray, int height, int width, int bzero,
            out uint medianValue, out Type dataType, out bool hasNegPix, out short minRawValue, out uint maxVal)
		{
			var medianCalcList = new List<uint>();

			dataType = null;
		    hasNegPix = false;
            maxVal = 0;
            minRawValue = short.MaxValue;

			uint[,] data = new uint[width, height];

			for (int y = 0; y < height; y++)
			{
				object dataRowObject = dataArray.GetValue(y);

				short[] dataRow;
				if (dataRowObject is short[])
				{
					dataRow = (short[]) dataRowObject;
					dataType = typeof (short);
				}
                else if (dataRowObject is float[])
                {
                    dataType = typeof(float);
                    Array arr = (Array)dataRowObject;
                    dataRow = new short[arr.Length];
                    for (int i = 0; i < arr.Length; i++)
                    {
                        float pixelVal = (float)arr.GetValue(i);

                        if (pixelVal >= short.MaxValue)
                            dataRow[i] = short.MaxValue;
                        else if (pixelVal <= short.MinValue)
                            dataRow[i] = short.MinValue;
                        else
                            dataRow[i] = (short)Convert.ToInt32(pixelVal);
                    }
                }
                else if (dataRowObject is Array)
				{
					Array arr = (Array) dataRowObject;
					dataRow = new short[arr.Length];
					for (int i = 0; i < arr.Length; i++)
					{
						if (dataType == null) dataType = arr.GetValue(i).GetType();
					    object pixelVal = arr.GetValue(i);

					    double dblVal = Convert.ToDouble(pixelVal);
					    if (dblVal >= short.MaxValue)
					        dataRow[i] = short.MaxValue;
                        else if (dblVal <= short.MinValue)
                            dataRow[i] = short.MinValue;
                        else
                            dataRow[i] = (short)Convert.ToInt32(pixelVal);
					}
				}
				else
					throw new ArrayTypeMismatchException();

				for (int x = 0; x < width; x++)
				{
				    uint val;

                    if (minRawValue > dataRow[x]) minRawValue = dataRow[x];

                    int intVal = bzero + dataRow[x];
                    if (intVal < 0)
                    {
                        intVal = 0;
                        if (!hasNegPix) hasNegPix = true;
                    }

                    val = (uint)intVal;

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

                maxVal = medianCalcList[medianCalcList.Count - 1];
			}
			else
				medianValue = 0;

			return data;
		}
	}
}
