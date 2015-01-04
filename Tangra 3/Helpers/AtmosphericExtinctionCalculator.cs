using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.Helpers
{
    public class AtmosphericExtinctionCalculator
    {
        public double RAHours { get; private set; }
        public double DEDeg { get; private set; }
        public double Longitude { get; private set; }
        public double Latitude { get; private set; }
        public double HeightKm { get; private set; }

        public static DateTime FIRST_JAN_2000_MIDDAY = new DateTime(2000, 1, 1, 12, 0, 0);
        public static double DEG_TO_RAD = Math.PI / 180.0;

        public AtmosphericExtinctionCalculator(double raHours, double deDeg, double longitude, double latitude, double heightKm)
        {
            RAHours = raHours;
            DEDeg = deDeg;
            Longitude = longitude;
            Latitude = latitude;
            HeightKm = heightKm;
        }

        public double CalculateZenithAngle(DateTime time)
        {
            double daysSince1Jan2000MidDay = new TimeSpan(time.Ticks - FIRST_JAN_2000_MIDDAY.Ticks).TotalDays;

            // GMST = 18.697 374 558 + 24.065 709 824 419 08 * D, D - interval in days from 1 Jan 2000 12h UT
            double MST = 18.697374558 + 24.06570982441908 * daysSince1Jan2000MidDay;

            // HA = MST + LON - RA
            double HA = MST + Longitude / 15.0 - RAHours;

            // sin(ALT) = sin(DEC)·sin(LAT) + cos(DEC)·cos(LAT)·cos(HA)
            double sinALT = Math.Sin(DEDeg * DEG_TO_RAD) * Math.Sin(Latitude * DEG_TO_RAD) + Math.Cos(DEDeg * DEG_TO_RAD) * Math.Cos(Latitude * DEG_TO_RAD) * Math.Cos(HA * 15 * DEG_TO_RAD);

            double Z_Radians = (Math.PI / 2) - Math.Asin(sinALT);

            return Z_Radians * 180 / Math.PI;
        }

        public double CalculateExtinction(DateTime time, out double altitudeDeg, out double airMass)
        {
            double A_OZ = 0.016;
            double A_RAY = 0.1451 * Math.Exp(-HeightKm / 7.996);
            double A_AER = 0.120 * Math.Exp(-HeightKm / 1.5);

            double zenithAngleDeg = CalculateZenithAngle(time);
            double Z_Radians =  zenithAngleDeg * DEG_TO_RAD;

            airMass = 1 / (Math.Cos(Z_Radians) + 0.025 * Math.Exp(-11 * Math.Cos(Z_Radians)));
            altitudeDeg = 90 - zenithAngleDeg;

            return airMass * (A_OZ + A_RAY + A_AER);
        }
    }
}
