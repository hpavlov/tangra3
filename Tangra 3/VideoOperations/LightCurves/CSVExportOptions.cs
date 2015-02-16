using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Tangra.Model.Helpers;

namespace Tangra.VideoOperations.LightCurves
{
    public enum TimeFormat
    {
        String,
        DecimalDays,
        DecimalJulianDays
    }

    public enum PhotometricFormat
    {
        RelativeFlux,
        Magnitudes
    }

    public class CSVExportOptions
    {
        public TimeFormat TimeFormat { get; set; }

        public PhotometricFormat PhotometricFormat { get; set; }

		public int Spacing { get; set; }

		public bool ForceSignalMinusBackground { get; set; }

	    public DateTime? FistMeasurementDay { get; set; }
        public DateTime? FistMeasurementTimeStamp { get; set; }

        private DateTime? m_FirstConfirmedMeasurementTimeStamp;

        private DateTime FirstConfirmedMeasurementTimeStamp
        {
            get
            {
                if (m_FirstConfirmedMeasurementTimeStamp == null)
                    m_FirstConfirmedMeasurementTimeStamp = FistMeasurementDay.Value.Date.AddTicks(FistMeasurementTimeStamp.Value.Ticks - FistMeasurementTimeStamp.Value.Date.Ticks);

                return m_FirstConfirmedMeasurementTimeStamp.Value;
            }
        }
        public double M0 { get; set; }

        public bool ExportAtmosphericExtinction { get; set; }

        public double RAHours { get; set; }
        public double DEDeg { get; set; }
        public double LongitudeDeg { get; set; }
        public double LatitudeDeg { get; set; }
        public double HeightKM { get; set; }

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
				return time.ToString(GetStringTimeFormat(timePrecisionSec, false), CultureInfo.InvariantCulture);
            }
            else if (TimeFormat == TimeFormat.DecimalDays)
            {
                double decimalDay = (time.Hour + time.Minute / 60.0 + (time.Second + time.Millisecond / 1000.0) / 3600.0) / 24.0;
				return decimalDay.ToString(GetDecimalDaysTimeFormat(timePrecisionSec), CultureInfo.InvariantCulture);
            }
            else if (TimeFormat == TimeFormat.DecimalJulianDays)
            {
                if (FistMeasurementDay.HasValue && FistMeasurementTimeStamp.HasValue)
                    time = FirstConfirmedMeasurementTimeStamp.AddTicks(time.Ticks - FistMeasurementTimeStamp.Value.Ticks);

                double jd = JDUtcAtDate(time);
                return jd.ToString(GetDecimalDaysTimeFormat(timePrecisionSec), CultureInfo.InvariantCulture);
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

        public string FormatPhotometricValueHeaderForObject(int objectNo, bool onlyExportSignalMunusBg, bool binning)
        {
            if (PhotometricFormat == PhotometricFormat.RelativeFlux)
            {
                if (binning)
                    return string.Format(",Binned Measurment ({0})", objectNo);

                if (!onlyExportSignalMunusBg)
                    return string.Format(",Signal ({0}), Background ({0})", objectNo);
                else
                    return string.Format(",SignalMinusBackground ({0})", objectNo);
            }
            else
            {
                if (binning)
                    return string.Format(",Binned Magnitude ({0})", objectNo);

                return string.Format(",Magnitude ({0})", objectNo);
            }
        }

        public string FormatPhotometricValue(bool isSuccessfulReading, double totalReading, double totalBackground, bool onlyExportSignalMunusBg, bool binning)
        {
            if (PhotometricFormat == PhotometricFormat.RelativeFlux)
            {
                if (isSuccessfulReading || binning)
                {
                    if (binning)
                        return string.Format(",{0}", totalReading.ToString(5));

                    if (!onlyExportSignalMunusBg)
                        return string.Format(",{0},{1}",
                                             totalReading.ToString(5),
                                             totalBackground.ToString(5));
                    else
                        return string.Format(",{0}", (totalReading - totalBackground).ToString(5));
                }
                else
                {
                    if (!onlyExportSignalMunusBg)
                        return ",,";
                    else
                        return ",";
                }
            }
            else if (PhotometricFormat == PhotometricFormat.Magnitudes)
            {
                if (isSuccessfulReading || binning)
                {
                    double flux;

                    if (binning)
                        flux = totalReading;
                    else if (!onlyExportSignalMunusBg)
                        flux = totalReading - totalBackground;
                    else
                        flux = totalReading;

                    double mag = M0 - 2.5*Math.Log10(Math.Max(1, flux));

                    return string.Format(",{0}", mag.ToString(5));
                }
                else
                {
                    return ",";
                }
            }
            else
                throw new NotImplementedException();
        }
    }
}
