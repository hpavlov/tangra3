using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic.FileIO;
using Tangra.Model.Helpers;

namespace Tangra.VideoOperations.Astrometry.MotionFitting
{
    public class MeasurementPositionCSVProvider : IMeasurementPositionProvider
    {
        private List<MeasurementPositionEntry> m_Measurements = new List<MeasurementPositionEntry>();

        public MeasurementPositionCSVProvider(string fileName)
        {
            using (TextFieldParser parser = new TextFieldParser(fileName))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                parser.TrimWhiteSpace = true;
                bool headerRowPassed = false;
                while (!parser.EndOfData)
                {
                    string[] fieldRow = parser.ReadFields();
                    if (!headerRowPassed)
                    {
                        headerRowPassed = true;
                        continue;
                    }

                    var entry = new MeasurementPositionEntry();

                    try
                    {
                        entry.FrameNo = int.Parse(fieldRow[0]);
                        if (!string.IsNullOrWhiteSpace(fieldRow[1])) entry.TimeOfDayUTC = double.Parse(fieldRow[1]);
                        if (!string.IsNullOrWhiteSpace(fieldRow[2])) entry.RADeg = double.Parse(fieldRow[2]);
                        if (!string.IsNullOrWhiteSpace(fieldRow[3])) entry.DEDeg = double.Parse(fieldRow[3]);
                        if (!string.IsNullOrWhiteSpace(fieldRow[4])) entry.Mag = double.Parse(fieldRow[4]);
                        if (!string.IsNullOrWhiteSpace(fieldRow[5])) entry.SolutionUncertaintyRACosDEArcSec = double.Parse(fieldRow[5]);
                        if (!string.IsNullOrWhiteSpace(fieldRow[6])) entry.SolutionUncertaintyDEArcSec = double.Parse(fieldRow[6]);
                        if (!string.IsNullOrWhiteSpace(fieldRow[7])) entry.FWHM = double.Parse(fieldRow[7]);
                        if (!string.IsNullOrWhiteSpace(fieldRow[8])) entry.DetectionCertainty = double.Parse(fieldRow[8]);
                        if (!string.IsNullOrWhiteSpace(fieldRow[9])) entry.SNR = double.Parse(fieldRow[9]);

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

        public string ObjectDesignation { get; private set; }
        
        public string ObservatoryCode { get; private set; }
        
        public DateTime ObservationDate { get; private set; }

        public decimal InstrumentalDelay { get; private set; }
    }
}
