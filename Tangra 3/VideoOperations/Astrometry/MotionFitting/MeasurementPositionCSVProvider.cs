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
        private List<MeasurementPositionEntry> m_Measurements = new List<MeasurementPositionEntry>();

        private string m_FilePath;
        private double m_InstrumentalDelay;
        private string m_DelayUnits;
        private int m_IntegratedFrames;
        private double m_IntegratedExposureSec;
        private string m_FrameTimeType;
        private string m_NativeVideoFormat;

        public bool IsTangraAstrometryExport { get; private set; }

        public MeasurementPositionCSVProvider(string fileName)
        {
            using (TextFieldParser parser = new TextFieldParser(fileName))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                parser.TrimWhiteSpace = true;
                bool headerRowPassed = false;
                bool readExport10Header = false;
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
                            continue;
                        }
                        else if (readExport10Header)
                        {
                            // FilePath, Date, InstrumentalDelay, DelayUnits, IntegratedFrames, IntegratedExposure(sec), FrameTimeType, NativeVideoFormat
                            // "2013 WT44a8.00.aav",2014-03-15,0.16,Seconds,1,0.32,NonIntegratedFrameTime,PAL
                            m_FilePath = fieldRow[0];
                            DateTime date;
                            if (DateTime.TryParseExact(fieldRow[1], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out date))
                                ObservationDate = date;
                            else
                                ObservationDate = null;

                            double.TryParse(fieldRow[2], out m_InstrumentalDelay);
                            m_DelayUnits = fieldRow[3];
                            int.TryParse(fieldRow[4], out m_IntegratedFrames);
                            double.TryParse(fieldRow[5], out m_IntegratedExposureSec);
                            m_FrameTimeType = fieldRow[6];
                            m_NativeVideoFormat = fieldRow[7];
                            ObservatoryCode = fieldRow.Length > 8 ? fieldRow[8] : null;
                            ObjectDesignation = fieldRow.Length > 9 ? fieldRow[9] : null;

                            ArsSecsInPixel = 0;
                            if (fieldRow.Length > 10)
                            {
                                double arcSecInPix;
                                if (double.TryParse(fieldRow[10], out arcSecInPix))
                                    ArsSecsInPixel = arcSecInPix;
                            }

                            IsTangraAstrometryExport = true;
                            readExport10Header = false;
                            continue;
                        }
                        else if (fieldRow[0] == "FrameNo")
                        {
                            headerRowPassed = true;
                            continue;
                        }
                    }

                    var entry = new MeasurementPositionEntry();

                    try
                    {
                        entry.FrameNo = int.Parse(fieldRow[0]);
                        if (!string.IsNullOrWhiteSpace(fieldRow[1])) entry.TimeOfDayUTC = double.Parse(fieldRow[1]);
                        entry.RawTimeStamp = fieldRow[2];
                        if (!string.IsNullOrWhiteSpace(fieldRow[3])) entry.RADeg = double.Parse(fieldRow[3]);
                        if (!string.IsNullOrWhiteSpace(fieldRow[4])) entry.DEDeg = double.Parse(fieldRow[4]);
                        if (!string.IsNullOrWhiteSpace(fieldRow[5])) entry.Mag = double.Parse(fieldRow[5]);
                        if (!string.IsNullOrWhiteSpace(fieldRow[6])) entry.SolutionUncertaintyRACosDEArcSec = double.Parse(fieldRow[6]);
                        if (!string.IsNullOrWhiteSpace(fieldRow[7])) entry.SolutionUncertaintyDEArcSec = double.Parse(fieldRow[7]);
                        if (!string.IsNullOrWhiteSpace(fieldRow[8])) entry.FWHMArcSec = double.Parse(fieldRow[8]);
                        if (!string.IsNullOrWhiteSpace(fieldRow[9])) entry.DetectionCertainty = double.Parse(fieldRow[9]);
                        if (!string.IsNullOrWhiteSpace(fieldRow[10])) entry.SNR = double.Parse(fieldRow[10]);

                        m_Measurements.Add(entry);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.GetFullStackTrace());
                    }
                }
            }
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
        
        public DateTime? ObservationDate { get; private set; }

        public decimal InstrumentalDelaySec
        {
            get
            {
                if (m_DelayUnits == "seconds")
                    return (decimal) m_InstrumentalDelay;
                else if (m_DelayUnits == "frames")
                {
                    if (m_NativeVideoFormat == "PAL")
                        return (decimal)(0.04 * m_InstrumentalDelay);
                    else if (m_NativeVideoFormat == "NTSC")
                        return (decimal)(0.0333667 * m_InstrumentalDelay);
                }
                
                return 0;
            }
        }

        public double ArsSecsInPixel { get; private set; }
    }
}
