using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using nom.tam.fits;
using nom.tam.util;

namespace Tangra.Model.Helpers
{
    public class FITSData
    {
        public BasicHDU HDU;
        public uint[] PixelsFlat;
        public Dictionary<string, string> Cards;
        public int Width;
        public int Height;
        public PixelStats PixelStats;
        public DateTime? Timestamp;
        public double? Exposure;
    }

    public class FITSMetrics
    {
        public int Width;
        public int Height;
        public int BitPix;
        public DateTime? Timestamp;
        public short MinPixelValue;
        public uint MaxPixelValue;      
    }

    public class PixelStats
    {
        public int BitPix;
        public int BZero;
        public Type PixelType;
        public int MinPixelValue;
        public uint MaxPixelValue;
        public uint MedianPixelValue;
        public bool HasNegativePixels;
        public int RawBitPix;
    }

    public enum FitsType
    {
        RGBFrames,
        SingleFrame,
        Invalid
    }

    public enum FitsCubeType
    {
        NotACube,
        ThreeAxisCube,
        MultipleHDUsCube
    }

    public static class FITSHelper2
    {
        //public static FITSData LoadFitsFile(string fileName, BasicHDU hdu, Array pixels, IFITSTimeStampReader timeStampReader, int negativePixelCorrection)
        //{

        //}

        public static FITSData LoadFitsFile(string fileName, IFITSTimeStampReader timeStampReader, int negativePixelCorrection)
        {
            var rv = new FITSData();

            Fits fitsFile = new Fits();

            using (BufferedFile bf = new BufferedFile(fileName, FileAccess.Read, FileShare.ReadWrite))
            {
                fitsFile.Read(bf);

                rv.HDU = fitsFile.GetHDU(0);

                rv.Width = rv.HDU.Axes[1];
                rv.Height = rv.HDU.Axes[0];

                Array pixelsArray = (Array)rv.HDU.Data.DataArray;

                rv.PixelStats = new PixelStats();
                rv.PixelStats.BZero = GetBZero(rv.HDU);
                rv.PixelStats.RawBitPix = rv.HDU.BitPix;

                uint mask = (uint)(((uint)1 << rv.PixelStats.RawBitPix) - 1);
                rv.PixelStats.BitPix = rv.PixelStats.RawBitPix;
                if (rv.PixelStats.BitPix < 0)
                {
                    /* float and double are -32 and -64 respectively */
                    // Pretending float data is 16 bit. This is the maximum currently supported by Tangra
                    // Not using a mask here, and the pixel values will be converted further down
                    mask = 0;
                }

                rv.PixelsFlat = Load16BitImageData(pixelsArray, rv.Height, rv.Width, rv.PixelStats.BZero - negativePixelCorrection, mask,
                    out rv.PixelStats.MedianPixelValue, out rv.PixelStats.PixelType,
                    out rv.PixelStats.HasNegativePixels, out rv.PixelStats.MinPixelValue,
                    out rv.PixelStats.MaxPixelValue);


                rv.PixelStats.BitPix = GetBppForMaxPixelValue(rv.PixelStats.MaxPixelValue);


                if (rv.PixelStats.BitPix == 32)
                {
                    // Tangra only supports up to 16 bit so convert down to 16 bit
                    int shift = Math.Max(0, (int)Math.Ceiling(Math.Abs(rv.HDU.BitPix) / 16.0) - 1);

                    for (int y = 0; y < rv.Height; y++)
                    {
                        for (int x = 0; x < rv.Width; x++)
                        {
                            rv.PixelsFlat[x + y * rv.Width] = rv.PixelsFlat[x + y * rv.Width] >> shift;
                        }
                    }

                    rv.PixelStats.BitPix = 16;
                }
            }


            // Read Timestamp & Exposure
            if (timeStampReader != null)
            {
                bool isMidPoint;
                rv.Timestamp = timeStampReader.ParseExposure(fileName, rv.HDU.Header, out isMidPoint, out rv.Exposure);                
            }


            // Read card from header
            rv.Cards = new Dictionary<string, string>();
            var cursor = rv.HDU.Header.GetCursor();
            while (cursor.MoveNext())
            {
                HeaderCard card = rv.HDU.Header.FindCard((string)cursor.Key);
                if (card != null && !string.IsNullOrWhiteSpace(card.Key) && card.Key != "END")
                {
                    if (rv.Cards.ContainsKey(card.Key))
                        rv.Cards[card.Key] += "\r\n" + card.Value;
                    else
                        rv.Cards.Add(card.Key, card.Value);
                }
            }

            return rv;
        }

        public static DateTime? ParseExposure(string fileName, Header header, IFITSTimeStampReader timeStampReader, out bool isMidPoint, out double? fitsExposure)
        {
            if (timeStampReader == null)
                return ParseExposureInternal(header, out isMidPoint, out fitsExposure);
            else
                return timeStampReader.ParseExposure(fileName, header, out isMidPoint, out fitsExposure);
        }

        private static Regex FITS_DATE_REGEX = new Regex("(?<DateStr>\\d\\d\\d\\d\\-\\d\\d\\-\\d\\dT\\d\\d:\\d\\d:\\d\\d(\\.\\d+)?)");

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


        public static uint[] Load16BitImageData(Array dataArray, int height, int width, int bzero, uint mask)
        {
            uint median;
            Type dataType;
            bool hasNegPix;
            int minValue;
            uint maxValue;

            return Load16BitImageData(dataArray, height, width, bzero, mask, out median, out dataType, out hasNegPix, out minValue, out maxValue);
        }


        public static uint[] Load16BitImageData(
            Array dataArray, int height, int width, int bzero, uint mask,
            out uint medianValue, out Type dataType, out bool hasNegPix, out int minRawValue, out uint maxVal)
        {
            var medianCalcList = new List<uint>();

            dataType = null;
            hasNegPix = false;
            maxVal = 0;
            medianValue = 0;
            minRawValue = int.MaxValue;

            uint[] data = new uint[width * height];

            if (dataArray == null)
                return data;

            for (int y = 0; y < height; y++)
            {
                object dataRowObject = dataArray.GetValue(y);

                short[] dataRow;
                if (dataRowObject is short[])
                {
                    dataRow = (short[])dataRowObject;
                    dataType = typeof(short);
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
                    Array arr = (Array)dataRowObject;
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

                    if (mask > 0) val = val & mask;

                    data[x + (height - y - 1) * width] = val;
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

        public static FitsType GetFitsType(string fileName)
        {
            Fits fitsFile = new Fits();

            using (BufferedFile bf = new BufferedFile(fileName, FileAccess.Read, FileShare.ReadWrite))
            {
                fitsFile.Read(bf);

                BasicHDU imageHDU = fitsFile.GetHDU(0);
                if (fitsFile.NumberOfHDUs == 1 && imageHDU.Axes.Length == 3 && imageHDU.Axes[0] == 3)
                    return FitsType.RGBFrames;

                if (fitsFile.NumberOfHDUs == 1 && imageHDU.Axes.Length == 2)
                    return FitsType.SingleFrame;

            }

            return FitsType.Invalid;
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


        public static Array GetPixelsFrom3DCube(Array cube, int index, int framePos, int heightPos, int widthPos, int totalFrames, int height, int width)
        {
            // NOTE: The axis should always be FrameId-Height-Width, however we have the implementation for the other cases just in case
            switch (framePos)
            {
                case 0:
                    var frameArr = cube.GetValue(index) as Array;
                    if (heightPos == 1)
                        return frameArr;
                    else if (heightPos == 2)
                    {
                        var pixType = (frameArr.GetValue(0) as Array).GetValue(0).GetType();
                        var rv = Array.CreateInstance(typeof(Array), height);
                        for (int i = 0; i < height; i++)
                        {
                            rv.SetValue(Array.CreateInstance(pixType, width), i);
                        }

                        for (int w = 0; w < width; w++)
                        {
                            var wArr = frameArr.GetValue(w) as Array;
                            for (int h = 0; h < height; h++)
                            {
                                (rv.GetValue(h) as Array).SetValue(wArr.GetValue(h), w);
                            }
                        }

                        return rv;
                    }

                    break;

                case 1:
                    if (heightPos == 0)
                    {
                        Array rv = null;

                        for (int h = 0; h < height; h++)
                        {
                            var fwArr = cube.GetValue(h) as Array;
                            for (int w = 0; w < width; w++)
                            {
                                var wArr = fwArr.GetValue(index) as Array;
                                if (rv == null)
                                {
                                    var pixType = wArr.GetValue(0).GetType();
                                    rv = Array.CreateInstance(typeof(Array), height);
                                    for (int i = 0; i < height; i++)
                                    {
                                        rv.SetValue(Array.CreateInstance(pixType, width), i);
                                    }
                                }

                                (rv.GetValue(h) as Array).SetValue(wArr.GetValue(w), w);
                            }
                        }

                        return rv;
                    }
                    else if (heightPos == 2)
                    {
                        Array rv = null;

                        for (int w = 0; w < width; w++)
                        {
                            var fhArr = cube.GetValue(w) as Array;
                            for (int h = 0; h < height; h++)
                            {
                                var hArr = fhArr.GetValue(index) as Array;
                                if (rv == null)
                                {
                                    var pixType = hArr.GetValue(0).GetType();
                                    rv = Array.CreateInstance(typeof(Array), height);
                                    for (int i = 0; i < height; i++)
                                    {
                                        rv.SetValue(Array.CreateInstance(pixType, width), i);
                                    }
                                }

                                (rv.GetValue(h) as Array).SetValue(hArr.GetValue(h), w);
                            }
                        }

                        return rv;
                    }
                    break;

                case 2:
                    if (heightPos == 0)
                    {
                        Array rv = null;

                        for (int h = 0; h < height; h++)
                        {
                            var wfArr = cube.GetValue(h) as Array;
                            for (int w = 0; w < width; w++)
                            {
                                var fArr = wfArr.GetValue(w) as Array;
                                if (rv == null)
                                {
                                    var pixType = fArr.GetValue(index).GetType();
                                    rv = Array.CreateInstance(typeof(Array), height);
                                    for (int i = 0; i < height; i++)
                                    {
                                        rv.SetValue(Array.CreateInstance(pixType, width), i);
                                    }
                                }

                                (rv.GetValue(h) as Array).SetValue(fArr.GetValue(index), w);
                            }
                        }

                        return rv;
                    }
                    else if (heightPos == 1)
                    {
                        Array rv = null;

                        for (int w = 0; w < width; w++)
                        {
                            var hfArr = cube.GetValue(w) as Array;
                            for (int h = 0; h < height; h++)
                            {
                                var fArr = hfArr.GetValue(h) as Array;
                                if (rv == null)
                                {
                                    var pixType = fArr.GetValue(index).GetType();
                                    rv = Array.CreateInstance(typeof(Array), height);
                                    for (int i = 0; i < height; i++)
                                    {
                                        rv.SetValue(Array.CreateInstance(pixType, width), i);
                                    }
                                }

                                (rv.GetValue(h) as Array).SetValue(fArr.GetValue(index), w);
                            }
                        }

                        return rv;
                    }
                    break;
            }

            return null;
        }
    }
}
