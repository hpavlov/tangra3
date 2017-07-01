using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic.FileIO;
using Tangra.Model.Helpers;
using Tangra.MotionFitting;

namespace Tangra.VideoOperations.Astrometry.MotionFitting
{
    public class MeasurementPositionCSVProvider : IMeasurementPositionProvider
    {
        public static MeasurementPositionCSVProvider Empty = new MeasurementPositionCSVProvider();

        private List<MeasurementPositionEntry> m_Measurements = new List<MeasurementPositionEntry>();

        private string m_FilePath;
        private double m_InstrumentalDelay;
        private string m_DelayUnits;
        private int m_IntegratedFrames;
        private double m_IntegratedExposureSec;
        private string m_FrameTimeType;
        private string m_NativeVideoFormat;

        public bool IsTangraAstrometryExport { get; private set; }

        private MeasurementPositionCSVProvider()
        { }

        public MeasurementPositionCSVProvider(string fileName)
        {
            using (TextFieldParser parser = new TextFieldParser(fileName))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                parser.TrimWhiteSpace = true;
                bool headerRowPassed = false;
                bool readExport10Header = false;
                string[] headers = null;
                string[] dataFields = null;
                while (!parser.EndOfData)
                {
                    string[] fieldRow = parser.ReadFields();
                    if (!headerRowPassed)
                    {
                        if (fieldRow[0] == "Tangra Astrometry Export v1.0")
                        {
                            continue;
                        }
                        else if (fieldRow[0] == "FilePath")
                        {
                            readExport10Header = true;
                            headers = fieldRow.ToArray();
                            continue;
                        }
                        else if (readExport10Header)
                        {
                            var headerDict = new Dictionary<string, string>();
                            for (int i = 0; i < headers.Length; i++)
                            {
                                if (fieldRow.Length > i)
                                    headerDict[headers[i].Trim()] = fieldRow[i].Trim();
                            }

                            m_FilePath = GrabString(headerDict, "FilePath");
                            ObservationDate = ParseDateTime(headerDict, "Date", "yyyy-MM-dd");
                            m_InstrumentalDelay = ParseDouble(headerDict, "InstrumentalDelay");
                            m_DelayUnits = GrabString(headerDict, "DelayUnits");
                            m_IntegratedFrames = ParseInt(headerDict, "IntegratedFrames");
                            m_IntegratedExposureSec = ParseDouble(headerDict, "IntegratedExposure(sec)");
                            m_FrameTimeType = GrabString(headerDict, "FrameTimeType");
                            m_NativeVideoFormat = GrabString(headerDict, "NativeVideoFormat");
                            ObservatoryCode = GrabString(headerDict, "ObservatoryCode");
                            ObjectDesignation = GrabString(headerDict, "Object");
                            CatalogueCode = GrabString(headerDict, "CatalogueCode");

                            Unmeasurable = false;

                            if (m_NativeVideoFormat == "AVI-Integrated" && m_IntegratedFrames != 1)
                                // Integrated video measured from the AVI file is not supported in the fast motion astrometry at the moment.
                                Unmeasurable = true;

                            if (headerDict.ContainsKey("ArsSecsInPixel"))
                            {
                                double arcSecInPix;
                                if (double.TryParse(headerDict["ArsSecsInPixel"], out arcSecInPix))
                                    ArsSecsInPixel = arcSecInPix;
                            }
                            else
                                ArsSecsInPixel = 0;

                            IsTangraAstrometryExport = true;
                            readExport10Header = false;
                            continue;
                        }
                        else if (fieldRow[0] == "FrameNo")
                        {
                            headerRowPassed = true;
                            dataFields = fieldRow.ToArray();
                            continue;
                        }
                    }

                    var entry = new MeasurementPositionEntry();

                    try
                    {
                        var dataDict = new Dictionary<string, string>();
                        for (int i = 0; i < dataFields.Length; i++)
                        {
                            if (fieldRow.Length > i)
                                dataDict[dataFields[i].Trim()] = fieldRow[i].Trim();
                        }

                        entry.FrameNo = ParseInt(dataDict, "FrameNo");
                        entry.TimeOfDayUTC = ParseDouble(dataDict, "TimeUTC(Uncorrected)");
                        entry.RawTimeStamp = GrabString(dataDict, "Timestamp");
                        entry.RADeg = ParseDouble(dataDict, "RADeg");
                        entry.DEDeg = ParseDouble(dataDict, "DEDeg");
                        entry.Mag = ParseDouble(dataDict, "Mag");
                        entry.SolutionUncertaintyRACosDEArcSec = ParseDouble(dataDict, "SolutionUncertaintyRA*Cos(DE)[arcsec]");
                        entry.SolutionUncertaintyDEArcSec = ParseDouble(dataDict, "SolutionUncertaintyDE[arcsec]");
                        entry.FWHMArcSec = ParseDouble(dataDict, "FWHM[arcsec]");
                        entry.DetectionCertainty = ParseDouble(dataDict, "DetectionCertainty");
                        entry.SNR = ParseDouble(dataDict, "SNR");

                        m_Measurements.Add(entry);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.GetFullStackTrace());
                    }
                }
            }
        }

        private static double ParseDouble(Dictionary<string, string> dict, string key, double defaultVal = 0)
        {
            if (dict.ContainsKey(key))
            {
                double dblVal;
                if (double.TryParse(dict[key], NumberStyles.Float, CultureInfo.InvariantCulture, out dblVal))
                    return dblVal;
            }

            return defaultVal;
        }

        private static int ParseInt(Dictionary<string, string> dict, string key, int defaultVal = 0)
        {
            if (dict.ContainsKey(key))
            {
                int intVal;
                if (int.TryParse(dict[key], NumberStyles.Integer, CultureInfo.InvariantCulture, out intVal))
                    return intVal;
            }

            return defaultVal;
        }

        private static string GrabString(Dictionary<string, string> dict, string key, string defaultVal = null)
        {
            return dict.ContainsKey(key) ? dict[key] : defaultVal;
        }

        private static DateTime? ParseDateTime(Dictionary<string, string> dict, string key, string format)
        {
            if (dict.ContainsKey(key))
            {
                DateTime date;
                if (DateTime.TryParseExact(dict[key], format, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out date))
                    return date;
            }

            return null;
        }

        public IEnumerable<MeasurementPositionEntry> Measurements
        {
            get { return m_Measurements; }
        }

        public int NumberOfMeasurements
        {
            get { return m_Measurements.Count; }
        }

        public string ObjectDesignation { get; private set; }
        
        public string ObservatoryCode { get; private set; }

        public string CatalogueCode { get; private set; }

        public bool Unmeasurable { get; private set; }
        
        public DateTime? ObservationDate { get; private set; }

        public decimal InstrumentalDelaySec
        {
            get
            {
                if ("seconds".Equals(m_DelayUnits, StringComparison.InvariantCultureIgnoreCase))
                    return (decimal) m_InstrumentalDelay;
                else if ("frames".Equals(m_DelayUnits, StringComparison.InvariantCultureIgnoreCase))
                {
                    if ("PAL".Equals(m_NativeVideoFormat, StringComparison.InvariantCultureIgnoreCase))
                        return (decimal)(0.04 * m_InstrumentalDelay);
                    else if ("NTSC".Equals(m_NativeVideoFormat, StringComparison.InvariantCultureIgnoreCase))
                        return (decimal)(0.0333667 * m_InstrumentalDelay);
                    else
                    {
                        return (decimal) (m_InstrumentalDelay * m_IntegratedExposureSec / m_IntegratedFrames);
                    }
                }
                
                return 0;
            }
        }

        public double ArsSecsInPixel { get; private set; }
    }
}
