using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.VideoOperations.LightCurves
{
    public enum TimeFormat
    {
        String,
        DecimalDays,
        DecimalJulianDays
    }

    public class CSVExportOptions
    {
        public TimeFormat TimeFormat { get; set; }

        private string GetStringTimeFormat(double timePrecisionSec, bool isBadTime)
        {
            if (timePrecisionSec < 0.005)
                return isBadTime ? "??:??:??.???" : "HH:mm:ss.fff";
            else if (timePrecisionSec < 0.05)
                return isBadTime ? "??:??:??.??" : "HH:mm:ss.ff";
            else if (timePrecisionSec < 0.5)
                return isBadTime ? "??:??:??.?" : "HH:mm:ss.f";
            else
                return isBadTime ? "??:??:??" : "HH:mm:ss";
        }

        private string GetDecimalDaysTimeFormat(double timePrecisionSec)
        {
            // 0.001 sec = 0.00000001
            // 0.005 sec = 
            // 0.01 sec = 0.0000001
            // 0.05 sec = 
            // 0.1 sec = 0.000001
            // 0.5 sec = 
            // 1 sec = 0.00001

            if (timePrecisionSec < 0.005)
                return "0.########";
            else if (timePrecisionSec < 0.05)
                return "0.#######";
            else if (timePrecisionSec < 0.5)
                return "0.######";
            else
                return "0.#####";            
        }

        public string FormatTimeLabel()
        {
            if (TimeFormat == TimeFormat.String)
            {
                return "UT";
            }
            else if (TimeFormat == TimeFormat.DecimalDays)
            {
                return "Days UT";
            }
            else if (TimeFormat == TimeFormat.DecimalJulianDays)
            {
                return "JD";
            }
            else
                throw new ArgumentOutOfRangeException();

        }

        public string FormatInvalidTime(double timePrecisionSec)
        {
            if (TimeFormat == TimeFormat.String)
            {
                return GetStringTimeFormat(timePrecisionSec, true);
            }
            else if (TimeFormat == TimeFormat.DecimalDays)
            {
                return "?";
            }
            else if (TimeFormat == TimeFormat.DecimalJulianDays)
            {
                return "?";
            }
            else
                throw new ArgumentOutOfRangeException();
        }

        public string FormatTime(DateTime time, double timePrecisionSec)
        {
            if (TimeFormat == TimeFormat.String)
            {
                return time.ToString(GetStringTimeFormat(timePrecisionSec, false));
            }
            else if (TimeFormat == TimeFormat.DecimalDays)
            {
                double decimalDay = (time.Hour + time.Minute / 60.0 + (time.Second + time.Millisecond / 1000.0) / 3600.0) / 24.0;
                return decimalDay.ToString(GetDecimalDaysTimeFormat(timePrecisionSec));
            }
            else if (TimeFormat == TimeFormat.DecimalJulianDays)
            {
                double jd = JDUtcAtDate(time);
                return jd.ToString(GetDecimalDaysTimeFormat(timePrecisionSec));
            }
            else
                throw new ArgumentOutOfRangeException();
        }

        private double JDUtcAtDate(DateTime date)
        {
            double tNow = (double)date.Ticks - 6.30822816E+17;	// .NET ticks at 01-Jan-2000T00:00:00
            double j = 2451544.5 + (tNow / 8.64E+11);		// Tick difference to days difference
            return j;
        }
    }
}
