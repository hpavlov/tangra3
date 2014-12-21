using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Tangra.KweeVanWoerden
{
    internal class AstroUtilities
    {
        internal class EarthAberrationData
        {
            internal const int REC_LENGTH = 28;

            public EarthAberrationData(BinaryReader reader)
            {
                I1 = reader.ReadInt16(); /* 2 */
                I2 = reader.ReadInt16(); /* 2 */
                D1 = reader.ReadDouble(); /* 8 */
                D2 = reader.ReadDouble(); /* 8 */
                D3 = reader.ReadDouble(); /* 8 */
            }

            internal readonly int I1;
            internal readonly int I2;
            internal readonly double D1;
            internal readonly double D2;
            internal readonly double D3;
        }

        internal class EarthNutationData
        {
            internal const int REC_LENGTH = 32;

            public EarthNutationData(BinaryReader reader)
            {
                I1 = reader.ReadInt16(); /* 2 */
                I2 = reader.ReadInt16(); /* 2 */
                I3 = reader.ReadInt16(); /* 2 */
                I4 = reader.ReadInt16(); /* 2 */
                I5 = reader.ReadInt16(); /* 2 */
                D1 = reader.ReadDouble(); /* 8 */
                F1 = reader.ReadSingle(); /* 4 */
                D2 = reader.ReadDouble(); /* 8 */
                I6 = reader.ReadInt16(); /* 2 */
            }

            internal readonly int I1;
            internal readonly int I2;
            internal readonly int I3;
            internal readonly int I4;
            internal readonly int I5;
            internal readonly int I6;

            internal readonly double D1;
            internal readonly double D2;
            internal readonly float F1;
        }

        private static EarthAberrationData[] s_AberrationArgs;
        private static EarthNutationData[] s_NutationArgs;

        internal class ConstellationBoundaryEntry
        {
            internal float RaFrom;
            internal float RaTo;
            internal float Dec;
            internal int ConstellationIndex;
        }

        internal class Constellation
        {
            internal string Name;
            internal string Abbreviation;
        }

        private static ConstellationBoundaryEntry[] s_ConstellationBoundaries = new ConstellationBoundaryEntry[357];
        private static Constellation[] s_Constellations = new Constellation[88];

        static AstroUtilities()
        {
            using (Stream resStr = AssemblyHelper.GetEmbededResourceStreamThatClientMustDispose("Tangra.KweeVanWoerden.Resources", "Earth.bin"))
            using (BinaryReader reader = new BinaryReader(resStr))
            {
                long entriesCount = resStr.Length / EarthAberrationData.REC_LENGTH;

                s_AberrationArgs = new EarthAberrationData[entriesCount];

                for (int i = 0; i < entriesCount; i++)
                {
                    s_AberrationArgs[i] = new EarthAberrationData(reader);
                }
            }

            using (Stream resStr = AssemblyHelper.GetEmbededResourceStreamThatClientMustDispose("Tangra.KweeVanWoerden.Resources", "Nutation.bin"))
            using (BinaryReader reader = new BinaryReader(resStr))
            {
                long entriesCount = resStr.Length / EarthNutationData.REC_LENGTH;

                s_NutationArgs = new EarthNutationData[entriesCount];

                for (int i = 0; i < entriesCount; i++)
                {
                    s_NutationArgs[i] = new EarthNutationData(reader);
                }
            }


            using (Stream resStr = AssemblyHelper.GetEmbededResourceStreamThatClientMustDispose("Tangra.KweeVanWoerden.Resources", "Constellations.bin"))
            using (TextReader reader = new StreamReader(resStr))
            {
                for (int i = 0; i < 88; i++)
                {
                    s_Constellations[i] = new Constellation();
                    s_Constellations[i].Name = reader.ReadLine();
                }

                for (int i = 0; i < 88; i++)
                    s_Constellations[i].Abbreviation = reader.ReadLine();

                for (int i = 0; i < 357; i++)
                {
                    s_ConstellationBoundaries[i] = new ConstellationBoundaryEntry();
                    s_ConstellationBoundaries[i].RaFrom = float.Parse(reader.ReadLine(), CultureInfo.InvariantCulture);
                    s_ConstellationBoundaries[i].RaTo = float.Parse(reader.ReadLine(), CultureInfo.InvariantCulture);
                    s_ConstellationBoundaries[i].Dec = float.Parse(reader.ReadLine(), CultureInfo.InvariantCulture);
                    s_ConstellationBoundaries[i].ConstellationIndex = int.Parse(reader.ReadLine(), CultureInfo.InvariantCulture);
                }
            }
        }

        private static double s_AccOldJD = 0;
        private static double s_AccNutLonArcsec = 0;
        private static double s_AccNutOblArcsec = 0;
        private static double s_AccEcliptic = 0;
        private static double s_Acc_C = 0;
        private static double s_Acc_D = 0;

        public static void ApparentStarPosition(ref double RA, ref double Dec, double muRA, double muDec, int Equinox, double JD)
        {
            double num;
            double num2;
            double num3;
            double num4;
            double num5;

            Precession(Equinox, JD, ref RA, ref Dec, muRA, muDec);

            if (Math.Abs((double)(JD - s_AccOldJD)) > 0.3)
            {
                Nutation(JD, out num, out num2, out num3);
                Aberration(JD, out num4, out num5);
                s_AccOldJD = JD;
                s_AccNutLonArcsec = num;
                s_AccNutOblArcsec = num2;
                s_AccEcliptic = num3;
                s_Acc_C = num4;
                s_Acc_D = num5;
            }
            else
            {
                JD = s_AccOldJD;
                num = s_AccNutLonArcsec;
                num2 = s_AccNutOblArcsec;
                num3 = s_AccEcliptic;
                num4 = s_Acc_C;
                num5 = s_Acc_D;
            }

            RA += (((num * (Math.Cos(num3) + ((Math.Sin(num3) * Math.Sin(RA)) * Math.Tan(Dec)))) - ((num2 * Math.Cos(RA)) * Math.Tan(Dec))) / 3600.0) / 57.295779513082323;
            Dec += ((((num * Math.Sin(num3)) * Math.Cos(RA)) + (num2 * Math.Sin(RA))) / 3600.0) / 57.295779513082323;
            RA += ((num4 * Math.Cos(RA)) + (num5 * Math.Sin(RA))) / Math.Cos(Dec);
            Dec += ((num4 * ((Math.Tan(num3) / Math.Tan(Dec)) - Math.Sin(RA))) + (num5 * Math.Cos(RA))) * Math.Sin(Dec);
        }

        public static void Precession(int StartEquinox, double JDEnd, ref double RA, ref double Dec, double muRA, double muDec)
        {
            double num;
            double num2;
            double num3;
            if (StartEquinox == 0x79e)
            {
                RA += 50.0 * muRA;
                Dec += 50.0 * muDec;
                RA += 0.0055878714;
                num = Math.Cos(Dec) * Math.Sin(RA);
                num2 = ((Math.Cos(0.0048580348) * Math.Cos(Dec)) * Math.Cos(RA)) - (Math.Sin(0.0048580348) * Math.Sin(Dec));
                num3 = (Math.Cos(0.0048580348) * Math.Sin(Dec)) + ((Math.Sin(0.0048580348) * Math.Cos(Dec)) * Math.Cos(RA));
                RA = Math.Atan2(num, num2) + 0.0055888302;
                Dec = Math.Atan(num3 / Math.Sqrt((num * num) + (num2 * num2)));
                muRA = ((muRA + (0.0048581 * (((muRA * Math.Cos(RA)) * Math.Tan(Dec)) + (((muDec * Math.Sin(RA)) / Math.Cos(Dec)) / Math.Cos(Dec))))) - ((2.114E-08 * Math.Sin(RA)) * Math.Tan(Dec))) + 6.7E-09;
                muDec = (muDec - ((0.0048581 * muRA) * Math.Sin(RA))) - (2.114E-08 * Math.Cos(RA));
                RA = (RA + ((1.651E-06 * Math.Sin(RA + 2.945)) / Math.Cos(Dec))) + 5.636E-06;
                Dec = (Dec + ((1.653E-06 * Math.Cos(RA + 2.945)) * Math.Sin(Dec))) + (1.406E-07 * Math.Cos(Dec));
                if (RA < 0.0)
                {
                    RA += 6.2831853071795862;
                }
                if (RA > 6.2831853071795862)
                {
                    RA -= 6.2831853071795862;
                }
            }
            double num4 = (JDEnd - 2451545.0) / 36525.0;
            double num5 = (((0.640616139 * num4) + ((8.3856E-05 * num4) * num4)) + (((4.9994E-06 * num4) * num4) * num4)) / 57.295779513082323;
            double num6 = (((0.640616139 * num4) + ((0.000304078 * num4) * num4)) + (((5.0564E-06 * num4) * num4) * num4)) / 57.295779513082323;
            double d = (((0.556753028 * num4) - ((0.000118514 * num4) * num4)) - (((1.16203E-05 * num4) * num4) * num4)) / 57.295779513082323;
            RA += (muRA * num4) * 100.0;
            Dec += (muDec * num4) * 100.0;
            RA += num5;
            num = Math.Cos(Dec) * Math.Sin(RA);
            num2 = ((Math.Cos(d) * Math.Cos(Dec)) * Math.Cos(RA)) - (Math.Sin(d) * Math.Sin(Dec));
            num3 = (Math.Cos(d) * Math.Sin(Dec)) + ((Math.Sin(d) * Math.Cos(Dec)) * Math.Cos(RA));
            RA = Math.Atan2(num, num2) + num6;
            if (RA < 0.0)
            {
                RA += 6.2831853071795862;
            }
            if (RA > 6.2831853071795862)
            {
                RA -= 6.2831853071795862;
            }
            Dec = Math.Atan(num3 / Math.Sqrt((num * num) + (num2 * num2)));
        }

        public static void PrecessStartToEnd(double JDStart, double JDEnd, ref double RA, ref double Dec)
        {
            double num7 = (JDStart - JDEnd) / 36525.0;
            double num8 = (JDEnd - JDStart) / 36525.0;
            double num = ((((0.640616139 + (0.00038793 * num7)) * num8) + (((8.3856E-05 - (9.56E-08 * num7)) * num8) * num8)) + (((4.9994E-06 * num8) * num8) * num8)) / 57.295779513082323;
            double num2 = ((((0.640616139 + (0.00038793 * num7)) * num8) + (((0.000304078 + (1.8E-08 * num7)) * num8) * num8)) + (((5.0564E-06 * num8) * num8) * num8)) / 57.295779513082323;
            double d = ((((0.556753028 - (0.00023703 * num7)) * num8) - (((0.000118514 + (6.03E-08 * num7)) * num8) * num8)) - (((1.16203E-05 * num8) * num8) * num8)) / 57.295779513082323;
            RA += num;
            double y = Math.Cos(Dec) * Math.Sin(RA);
            double x = ((Math.Cos(d) * Math.Cos(Dec)) * Math.Cos(RA)) - (Math.Sin(d) * Math.Sin(Dec));
            double num6 = (Math.Cos(d) * Math.Sin(Dec)) + ((Math.Sin(d) * Math.Cos(Dec)) * Math.Cos(RA));
            RA = Math.Atan2(y, x) + num2;
            if (RA < 0.0)
            {
                RA += 6.2831853071795862;
            }
            if (RA > 6.2831853071795862)
            {
                RA -= 6.2831853071795862;
            }
            Dec = Math.Atan(num6 / Math.Sqrt((y * y) + (x * x)));
        }



        public static void Nutation(double JD, out double Longitude_arcsec, out double Obliquity_arcsec, out double Ecliptic)
        {
            double num = (JD - 2451545.0) / 36525.0;
            double num2 = (((297.8502042 + (445267.1115168 * num)) - ((0.0016335 * num) * num)) + (((num * num) * num) / 546300.0)) / 57.295779513082323;
            double num3 = (((93.2720993 + (483202.0175273 * num)) - ((0.0034064 * num) * num)) - (((num * num) * num) / 6550000.0)) / 57.295779513082323;
            double num4 = (((357.5291092 + (35999.0502909 * num)) - ((0.000156 * num) * num)) - (((num * num) * num) / 2280000.0)) / 57.295779513082323;
            double num5 = (((134.9634114 + (477198.8676313 * num)) + ((0.0089937 * num) * num)) + (((num * num) * num) / 73725.0)) / 57.295779513082323;
            double num6 = ((((218.3164591 + (481267.88134236 * num)) - ((0.0013298 * num) * num)) + (((num * num) * num) / 546300.0)) / 57.295779513082323) - num3;

            double num17 = 0.0;
            double num18 = 0.0;
            foreach (EarthNutationData nutArg in s_NutationArgs)
            {
                double a = ((((nutArg.I1 * num2) + (nutArg.I2 * num4)) + (nutArg.I3 * num5)) + (nutArg.I4 * num3)) + (nutArg.I5 * num6);
                num17 += (nutArg.D1 + (nutArg.F1 * num)) * Math.Sin(a);
                num18 += (nutArg.D2 + (nutArg.I6 * num)) * Math.Cos(a);
            }

            Longitude_arcsec = num17 / 10000.0;
            Obliquity_arcsec = num18 / 10000.0;
            Ecliptic = ((((23.4392911 - (0.01300417 * num)) - ((1.64E-06 * num) * num)) + (((5.036E-07 * num) * num) * num)) + (Obliquity_arcsec / 3600.0)) / 57.295779513082323;
        }

        public static void Aberration(double JD, out double C, out double D)
        {
            double[] numArray2;
            double[] numArray = new double[3];
            double t = (JD - 2451545.0) / 365250.0;

            numArray2 = new double[] { 0.0, 1.0, t, t * t, Math.Pow(t, 4), Math.Pow(t, 8), Math.Pow(t, 16) };

            double num3 = ((13.96971 * numArray2[2]) + (0.03086 * numArray2[3])) / 57.295779513082323;
            double num2 = Math.Cos((23.4392911 - (0.1300417 * numArray2[2])) / 57.295779513082323);

            numArray[1] = 0.0;
            numArray[2] = 0.0;

            for (int i = 1; i <= s_AberrationArgs.Length; i++)
            {
                EarthAberrationData entry = s_AberrationArgs[i - 1];

                double d = entry.D2 + (entry.D3 * numArray2[2]);
                if (entry.I1 != 3)
                    numArray[entry.I1] -= (entry.D1 * (((entry.I2 * Math.Cos(d)) * numArray2[entry.I2]) - ((entry.D3 * Math.Sin(d)) * numArray2[entry.I2 + 1]))) / 365250.0;

                if (entry.D1 < 1E-05)
                    break;
            }

            double num5 = ((1191.28 * numArray[1]) / 57.295779513082323) / 3600.0;
            double num4 = (((-1191.28 * numArray[2]) * num2) / 57.295779513082323) / 3600.0;
            C = num4 - ((num3 * num2) * num5);
            D = num5 + ((num3 / num2) * num4);
        }


        public static double DateTimeToJD(DateTime dt)
        {
            int Year = dt.Year;
            int Month = dt.Month;
            double day = dt.Day + (dt.Hour + dt.Minute / 60.0 + dt.Second / 3600.0 + dt.Millisecond / (3600.0 * 1000.0)) / 24.0;

            double num = ((12.0 * (Convert.ToDouble(Year) + 4800.0)) + Month) - 3.0;
            double num2 = Math.Floor((double)((((2.0 * (num % 12.0)) + 7.0) + (365.0 * num)) / 12.0)) + day;
            double num3 = (num2 + Math.Floor((double)(num / 48.0))) - 32083.5;
            if (((Year > 0x62e) | ((Year == 0x62e) & (Month > 10))) | (((Year == 0x62e) & (Month == 10)) & (day > 4.0)))
            {
                num3 = ((num3 + Math.Floor((double)(num / 4800.0))) - Math.Floor((double)(num / 1200.0))) + 38.0;
            }
            return num3;
        }

        public static string GetConstellation(double ra2000Hrs, double dec2000Deg, out string abbreviatedName)
        {
            double rA = ra2000Hrs * 15 * Math.PI / 180;
            double dec = dec2000Deg * Math.PI / 180;

            PrecessStartToEnd(2451545.0, 2405890.5, ref rA, ref dec);
            rA = (rA * 57.295779513082323) / 15.0;
            dec *= 57.295779513082323;

            int index = -1;
            do
            {
                index++;
            }
            while (s_ConstellationBoundaries[index].Dec > dec);
            do
            {
                index--;
                do
                {
                    index++;
                }
                while (s_ConstellationBoundaries[index].RaTo <= rA);
                index--;
                do
                {
                    index++;
                }
                while (s_ConstellationBoundaries[index].RaFrom > rA);
            }
            while (s_ConstellationBoundaries[index].RaTo <= rA);

            Constellation constellation = s_Constellations[s_ConstellationBoundaries[index].ConstellationIndex - 1];

            abbreviatedName = constellation.Abbreviation;
            return constellation.Name;
        }

        public static void Distance(double RA1, double Dec1, double RA2, double Dec2, out double Distance, out double PA)
        {
            double a = RA2 - RA1;
            double y = Math.Cos(Dec2) * Math.Sin(a);
            double x = (Math.Cos(Dec1) * Math.Sin(Dec2)) - ((Math.Sin(Dec1) * Math.Cos(Dec2)) * Math.Cos(a));
            double num3 = (Math.Sin(Dec1) * Math.Sin(Dec2)) + ((Math.Cos(Dec1) * Math.Cos(Dec2)) * Math.Cos(a));
            PA = Math.Atan2(y, x);
            if (PA < 0.0)
            {
                PA += 6.2831853071795862;
            }
            Distance = Math.Atan2(Math.Sqrt((y * y) + (x * x)), num3);
            if (Distance < 0.0)
            {
                Distance += 3.1415926535897931;
            }
        }

        public static void QuickMoon(double JD, out double RA, out double Dec, out double Parallax, out double MoonLongitude, out double MoonLatitude)
        {
            double num = (JD - 2451545.0) / 36525.0;
            double a = ((297.8502042 + (445267.1115168 * num)) - ((0.0016335 * num) * num)) / 57.295779513082323;
            double num3 = ((93.2720993 + (483202.0175273 * num)) - ((0.0034064 * num) * num)) / 57.295779513082323;
            double num4 = ((357.5291092 + (35999.0502909 * num)) - ((0.000156 * num) * num)) / 57.295779513082323;
            double num5 = ((134.9634114 + (477198.8676313 * num)) + ((0.0089937 * num) * num)) / 57.295779513082323;
            double num7 = (((218.3164591 + (481267.88134236 * num)) - ((0.0013298 * num) * num)) % 360.0) / 57.295779513082323;
            double d = (23.4392911 - (0.01300417 * num)) / 57.295779513082323;
            double num9 = (((((((((((((((((((((((((((14.0 * Math.Sin(4.0 * a)) + (2369.0 * Math.Sin(2.0 * a))) + (192.0 * Math.Sin(num5 + (2.0 * a)))) + (22639.0 * Math.Sin(num5))) - (4586.0 * Math.Sin(num5 - (2.0 * a)))) - (38.0 * Math.Sin(num5 - (4.0 * a)))) - (24.0 * Math.Sin(num4 + (2.0 * a)))) - (668.0 * Math.Sin(num4))) - (165.0 * Math.Sin(num4 - (2.0 * a)))) - (125.0 * Math.Sin(a))) + (14.0 * Math.Sin((2.0 * num5) + (2.0 * a)))) + (769.0 * Math.Sin(2.0 * num5))) - (211.0 * Math.Sin((2.0 * num5) - (2.0 * a)))) - (30.0 * Math.Sin((2.0 * num5) - (4.0 * a)))) - (110.0 * Math.Sin(num5 + num4))) - (206.0 * Math.Sin((num5 + num4) - (2.0 * a)))) + (15.0 * Math.Sin((num5 - num4) + (2.0 * a)))) + (148.0 * Math.Sin(num5 - num4))) + (28.0 * Math.Sin((num5 - num4) - (2.0 * a)))) - (411.0 * Math.Sin(2.0 * num3))) - (55.0 * Math.Sin((2.0 * num3) - (2.0 * a)))) - (55.0 * Math.Sin((2.0 * num3) - (2.0 * a)))) + (18.0 * Math.Sin(num5 - a))) - (18.0 * Math.Sin(num5 + a))) + (36.0 * Math.Sin(3.0 * num5))) - (13.0 * Math.Sin((3.0 * num5) - (2.0 * a)))) - (45.0 * Math.Sin(num5 + (2.0 * num3)))) + (39.0 * Math.Sin(num5 - (2.0 * num3)));
            double num8 = num7 + ((num9 / 3600.0) / 57.295779513082323);
            double num10 = ((((((0.033 * Math.Sin(num3 - (2.0 * a))) + (5.128 * Math.Sin(num3))) - (0.173 * Math.Sin(num3 - (2.0 * a)))) + (0.281 * Math.Sin(num5 + num3))) - (0.046 * Math.Sin((num5 + num3) - (2.0 * a)))) + (0.055 * Math.Sin((num3 - num5) + (2.0 * a)))) - (0.278 * Math.Sin(num3 - num5));
            num10 /= 57.295779513082323;
            Parallax = ((((((((((((((((((((3422.5 + (0.26 * Math.Cos(4.0 * a))) + (28.23 * Math.Cos(2.0 * a))) + (3.09 * Math.Cos(num5 + (2.0 * a)))) + (186.54 * Math.Cos(num5))) + (34.31 * Math.Cos(num5 - (2.0 * a)))) + (0.6 * Math.Cos(num5 - (4.0 * a)))) - (0.3 * Math.Cos(num4 + (2.0 * a)))) - (0.4 * Math.Cos(num4))) + (1.92 * Math.Cos(num4 - (2.0 * a)))) - (0.98 * Math.Cos(a))) + (0.28 * Math.Cos((2.0 * num5) + (2.0 * a)))) + (10.17 * Math.Cos(2.0 * num5))) - (0.3 * Math.Cos((2.0 * num5) - (2.0 * a)))) + (0.37 * Math.Cos((2.0 * num5) - (4.0 * a)))) - (0.95 * Math.Cos(num5 + num4))) + (1.44 * Math.Cos((num5 + num4) - (2.0 * a)))) + (1.15 * Math.Cos(num5 - num4))) + (0.62 * Math.Cos(3.0 * num5))) - (0.71 * Math.Cos(num5 - (2.0 * num3)))) / 3600.0) / 57.295779513082323;
            double y = ((Math.Cos(num10) * Math.Sin(num8)) * Math.Cos(d)) - (Math.Sin(num10) * Math.Sin(d));
            double x = Math.Cos(num10) * Math.Cos(num8);
            double num13 = (Math.Sin(num10) * Math.Cos(d)) + ((Math.Cos(num10) * Math.Sin(d)) * Math.Sin(num8));
            RA = Math.Atan2(y, x);
            if (RA < 0.0)
            {
                RA += 6.2831853071795862;
            }
            Dec = Math.Atan(num13 / Math.Sqrt((y * y) + (x * x)));
            MoonLongitude = num8;
            MoonLatitude = num10;
        }

        public static void QuickPlanet(double JD, int PlanetNo, bool EquinoxOfDate, out double RA, out double Dec, out double GeocentricDistance)
        {
            double longitude = 0.0;
            double latitude = 0.0;
            double planetHeliocentricDistance = 0.0;
            QuickPlanet(JD, PlanetNo, EquinoxOfDate, out RA, out Dec, out GeocentricDistance, out longitude, out latitude, out planetHeliocentricDistance);
        }

        public static void QuickPlanet(double JD, int PlanetNo, bool EquinoxOfDate, out double RA, out double Dec, out double Distance, out double Longitude, out double Latitude, out double PlanetHeliocentricDistance)
        {
            double num2;
            double num3;
            double num4;
            double num5;
            double num6;
            double num7;
            double num8;
            double num9;
            double num16;
            PlanetHeliocentricDistance = num16 = 0.0;
            double x = num2 = num3 = num4 = num5 = num6 = num7 = num8 = num9 = num16;
            if (PlanetNo != 3)
            {
                PlanetXYZ(JD, PlanetNo, 0.0001, ref num4, ref num5, ref num6);
                PlanetHeliocentricDistance = Math.Sqrt(((num4 * num4) + (num5 * num5)) + (num6 * num6));
            }
            PlanetXYZ(JD, 3, 0.0001, ref x, ref num2, ref num3);
            num7 = num4 - x;
            num8 = num5 - num2;
            num9 = num6 - num3;
            Distance = Math.Sqrt(((num7 * num7) + (num8 * num8)) + (num9 * num9));
            XYZ_to_RA_Dec(num7, num8, num9, out RA, out Dec);
            if (EquinoxOfDate)
            {
                double num13;
                double num14;
                double num15;
                Precession(0x7d0, JD, ref RA, ref Dec, 0.0, 0.0);
                Nutation(JD, out num13, out num14, out num15);
                RA += (((num13 * (Math.Cos(num15) + ((Math.Sin(num15) * Math.Sin(RA)) * Math.Tan(Dec)))) - ((num14 * Math.Cos(RA)) * Math.Tan(Dec))) / 3600.0) / 57.295779513082323;
                Dec += ((((num13 * Math.Sin(num15)) * Math.Cos(RA)) + (num14 * Math.Sin(RA))) / 3600.0) / 57.295779513082323;
            }
            double num10 = Math.Cos(Dec) * Math.Cos(RA);
            double y = ((Math.Cos(Dec) * Math.Sin(RA)) * 0.917482062146321) + (Math.Sin(Dec) * 0.39777715575399);
            double num12 = ((-Math.Cos(Dec) * Math.Sin(RA)) * 0.39777715575399) + (Math.Sin(Dec) * 0.917482062146321);
            Longitude = Math.Atan2(y, num10);
            if (Longitude < 0.0)
            {
                Longitude += 6.2831853071795862;
            }
            Latitude = Math.Atan(num12 / Math.Sqrt((num10 * num10) + (y * y)));
        }

        public static void XYZ_to_RA_Dec(double X, double Y, double Z, out double RA, out double Dec)
        {
            if (X == 0.0)
            {
                RA = Math.Atan(Y);
            }
            else
            {
                RA = Math.Atan2(Y, X);
            }
            if (RA < 0.0)
            {
                RA += 6.2831853071795862;
            }
            if ((X == 0.0) | (Y == 0.0))
            {
                Dec = 0.0;
            }
            else
            {
                Dec = Math.Atan2(Z, Math.Sqrt((X * X) + (Y * Y)));
            }
        }

        public static void PlanetXYZ(double JD, int PlanetNumber, double Accuracy, ref double X, ref double Y, ref double Z)
        {
            double[] numArray = new double[4];
            double[] numArray2 = new double[6];

            numArray2[0] = 1.0;
            numArray2[1] = (JD - 2451545.0) / 365250.0;
            numArray2[2] = numArray2[1] * numArray2[1];
            numArray2[3] = numArray2[2] * numArray2[1];
            numArray2[4] = numArray2[3] * numArray2[1];
            numArray2[5] = numArray2[4] * numArray2[1];

            numArray[1] = 0.0;
            numArray[2] = 0.0;
            numArray[3] = 0.0;


            for (int i = 1; i <= s_AberrationArgs.Length; i++)
            {
                EarthAberrationData entry = s_AberrationArgs[i - 1];

                numArray[entry.I1] += (entry.D1 * Math.Cos(entry.D2 + (entry.D3 * numArray2[1]))) * numArray2[entry.I2];
                if (entry.D1 < Accuracy)
                {
                    break;
                }
            }

            X = (numArray[1] + (4.4036E-07 * numArray[2])) - (1.90919E-07 * numArray[3]);
            Y = ((-4.79966E-07 * numArray[1]) + (0.917482137087 * numArray[2])) - (0.397776982902 * numArray[3]);
            Z = (0.397776982902 * numArray[2]) + (0.917482137087 * numArray[3]);
        }

        public static void GetDatumToWGS84Corrections(int DatumFlag, double Longitude, double Latitude, double Altitude_metres, out double DLongitude_arcsec, out double DLatitude_arcsec)
        {
            double num;
            double num2;
            double num3;
            double num4;
            double num5;
            string str;
            GetDatumParameters(DatumFlag, out num3, out num4, out num5, out num, out num2, out str);
            double num6 = 0.0033528106647474805 - num;
            double num10 = System.Math.Sqrt((2.0 * num) - (num * num));
            double num7 = 6378137.0 - num2;
            double num11 = 1.0 - num;
            double num12 = System.Math.Sin(Latitude) * System.Math.Cos(Latitude);
            double num8 = (num2 * (1.0 - (num10 * num10))) / System.Math.Pow(1.0 - (((num10 * System.Math.Sin(Latitude)) * num10) * System.Math.Sin(Latitude)), 1.5);
            double num9 = num2 / System.Math.Pow(1.0 - (((num10 * System.Math.Sin(Latitude)) * num10) * System.Math.Sin(Latitude)), 0.5);
            DLatitude_arcsec = (((((((-num3 * System.Math.Sin(Latitude)) * System.Math.Cos(Longitude)) - ((num4 * System.Math.Sin(Latitude)) * System.Math.Sin(Longitude))) + (num5 * System.Math.Cos(Latitude))) + (((((num7 * num9) * num10) * num10) * num12) / num2)) + ((num6 * ((num8 / num11) + (num9 * num11))) * num12)) / (num8 + Altitude_metres)) / 4.8481E-06;
            DLongitude_arcsec = ((((-num3 * System.Math.Sin(Longitude)) + (num4 * System.Math.Cos(Longitude))) / (num9 + Altitude_metres)) / System.Math.Cos(Latitude)) / 4.8481E-06;
        }

        public static void GetDatumParameters(int DatumFlag, out double X, out double Y, out double Z, out double f, out double EarthRadius, out string DatumName)
        {
            switch (DatumFlag)
            {
                case 0:
                    f = 0.0033528106647474805;
                    EarthRadius = 6378137.0;
                    X = 0.0;
                    Y = 0.0;
                    Z = 0.0;
                    DatumName = "WGS84";
                    return;

                case 10:
                    f = 0.0033528106647474805;
                    EarthRadius = 6378137.0;
                    X = 0.0;
                    Y = 0.0;
                    Z = 0.0;
                    DatumName = "WGS84 - GoogleEarth";
                    return;

                case 12:
                    f = 0.0033670033670033669;
                    EarthRadius = 6378388.0;
                    X = 0.0;
                    Y = 0.0;
                    Z = 0.0;
                    DatumName = "Christmas Isl Astro 1967";
                    return;

                case 13:
                    f = 0.0033670033670033669;
                    EarthRadius = 6378388.0;
                    X = -134.0;
                    Y = 229.0;
                    Z = -29.0;
                    DatumName = "Chua Astro (Brazil)";
                    return;

                case 14:
                    f = 0.0033670033670033669;
                    EarthRadius = 6378388.0;
                    X = -206.0;
                    Y = 172.0;
                    Z = -6.0;
                    DatumName = "Corrego Alegre (Brazil)";
                    return;

                case 15:
                    f = 0.0033670033670033669;
                    EarthRadius = 6378388.0;
                    X = 211.0;
                    Y = 147.0;
                    Z = 111.0;
                    DatumName = "Easter Island Astro 1967";
                    return;

                case 0x10:
                    f = 0.0033670033670033669;
                    EarthRadius = 6378388.0;
                    X = -87.0;
                    Y = -98.0;
                    Z = -121.0;
                    DatumName = "ED/ED1950/ED1979";
                    return;

                case 0x11:
                    f = 0.0033670033670033669;
                    EarthRadius = 6378388.0;
                    X = -104.0;
                    Y = 167.0;
                    Z = -38.0;
                    DatumName = "Graciosa Island (Azores)";
                    return;

                case 0x12:
                    f = 0.0033670033670033669;
                    EarthRadius = 6378388.0;
                    X = 230.0;
                    Y = -199.0;
                    Z = -752.0;
                    DatumName = "Gizo, Provisional DOS";
                    return;

                case 0x13:
                    f = 0.0033900753040885176;
                    EarthRadius = 6378206.4;
                    X = -100.0;
                    Y = -248.0;
                    Z = 259.0;
                    DatumName = "Guam";
                    return;

                case 20:
                    f = 0.0033670033670033669;
                    EarthRadius = 6378388.0;
                    X = 0.0;
                    Y = 0.0;
                    Z = 0.0;
                    DatumName = "Heard Astro 1969";
                    return;

                case 0x15:
                    f = 0.0033900753040885176;
                    EarthRadius = 6378206.4;
                    X = 0.0;
                    Y = 0.0;
                    Z = 0.0;
                    DatumName = "Iben Astro, Navy 1947 (Truk)";
                    return;

                case 0x16:
                    f = 0.003324449296662885;
                    EarthRadius = 6377276.345;
                    X = 295.0;
                    Y = 736.0;
                    Z = 257.0;
                    DatumName = "Indian";
                    return;

                case 0x17:
                    f = 0.0033900753040885176;
                    EarthRadius = 6378206.4;
                    X = 0.0;
                    Y = 0.0;
                    Z = 0.0;
                    DatumName = "Isla Socorro Astro";
                    return;

                case 0x18:
                    f = 0.0033670033670033669;
                    EarthRadius = 6378388.0;
                    X = 189.0;
                    Y = -79.0;
                    Z = -202.0;
                    DatumName = "Johnston Island 1961";
                    return;

                case 0x19:
                    f = 0.0033670033670033669;
                    EarthRadius = 6378388.0;
                    X = 647.0;
                    Y = 1777.0;
                    Z = 1124.0;
                    DatumName = "Kusaie Astro 1962, 1965";
                    return;

                case 0x1a:
                    f = 0.0033900753040885176;
                    EarthRadius = 6378206.4;
                    X = -133.0;
                    Y = -77.0;
                    Z = -51.0;
                    DatumName = "Luzon 1911 (Philippines)";
                    return;

                case 0x1b:
                    f = 0.0033670033670033669;
                    EarthRadius = 6378388.0;
                    X = 403.0;
                    Y = -81.0;
                    Z = 277.0;
                    DatumName = "Midway Astro 1961";
                    return;

                case 0x1c:
                    f = 0.0033670033670033669;
                    EarthRadius = 6378388.0;
                    X = 84.0;
                    Y = -22.0;
                    Z = 209.0;
                    DatumName = "New Zealand 1949";
                    return;

                case 0x1d:
                    f = 0.0033900753040885176;
                    EarthRadius = 6378206.4;
                    X = -8.0;
                    Y = 160.0;
                    Z = 176.0;
                    DatumName = "NAD27/NAD1927";
                    return;

                case 30:
                    f = 0.0033900753040885176;
                    EarthRadius = 6378206.4;
                    X = -2.0;
                    Y = 151.0;
                    Z = 181.0;
                    DatumName = "Cape Canaveral";
                    return;

                case 0x1f:
                    f = 0.0033900753040885176;
                    EarthRadius = 6378206.4;
                    X = -2.0;
                    Y = 151.0;
                    Z = 181.0;
                    DatumName = "White Sands*";
                    return;

                case 0x20:
                    f = 0.0033427731821748059;
                    EarthRadius = 6377397.155;
                    X = 0.0;
                    Y = 0.0;
                    Z = 0.0;
                    DatumName = "Old Bavarian*";
                    return;

                case 0x21:
                    f = 0.0033900753040885176;
                    EarthRadius = 6378206.4;
                    X = 61.0;
                    Y = -285.0;
                    Z = -181.0;
                    DatumName = "Old Hawaiian";
                    return;

                case 0x22:
                    f = 0.0033408506414970775;
                    EarthRadius = 6377563.396;
                    X = 375.0;
                    Y = -111.0;
                    Z = 431.0;
                    DatumName = "ORDN1936/GB1936";
                    return;

                case 0x23:
                    f = 0.0033670033670033669;
                    EarthRadius = 6378388.0;
                    X = -307.0;
                    Y = -92.0;
                    Z = 127.0;
                    DatumName = "Pico de las Nieves (Canaries)";
                    return;

                case 0x24:
                    f = 0.0033670033670033669;
                    EarthRadius = 6378388.0;
                    X = 185.0;
                    Y = 165.0;
                    Z = 42.0;
                    DatumName = "Pitcairn Island Astro";
                    return;

                case 0x25:
                    f = 0.0033427731821748059;
                    EarthRadius = 6377397.155;
                    X = -591.0;
                    Y = -81.0;
                    Z = -396.0;
                    DatumName = "Potsdam";
                    return;

                case 0x26:
                    f = 0.0033670033670033669;
                    EarthRadius = 6378388.0;
                    X = -288.0;
                    Y = 175.0;
                    Z = -376.0;
                    DatumName = "Provisional South American 1956";
                    return;

                case 0x27:
                    f = 0.0033670033670033669;
                    EarthRadius = 6378388.0;
                    X = 16.0;
                    Y = 196.0;
                    Z = 93.0;
                    DatumName = "Provisional South Chile 1963";
                    return;

                case 40:
                    f = 0.003352329869259135;
                    EarthRadius = 6378245.0;
                    X = 28.0;
                    Y = -130.0;
                    Z = -95.0;
                    DatumName = "Pulkovo 1942";
                    return;

                case 0x29:
                    f = 0.0033528918692372171;
                    EarthRadius = 6378160.0;
                    X = -57.0;
                    Y = 1.0;
                    Z = -41.0;
                    DatumName = "SAM1969/SA69";
                    return;

                case 0x2a:
                    f = 0.003407561378699334;
                    EarthRadius = 6378249.145;
                    X = 41.0;
                    Y = -220.0;
                    Z = -134.0;
                    DatumName = "Southeast Island (Mahe)";
                    return;

                case 0x2b:
                    f = 0.0033670033670033669;
                    EarthRadius = 6378388.0;
                    X = -794.0;
                    Y = 119.0;
                    Z = -298.0;
                    DatumName = "South Georgia Astro";
                    return;

                case 0x2c:
                    f = 0.0033670033670033669;
                    EarthRadius = 6378388.0;
                    X = 0.0;
                    Y = 0.0;
                    Z = 0.0;
                    DatumName = "Swallow Islands (Solomons)";
                    return;

                case 0x2d:
                    f = 0.0033670033670033669;
                    EarthRadius = 6378388.0;
                    X = -189.0;
                    Y = -242.0;
                    Z = -91.0;
                    DatumName = "Tananarive";
                    return;

                case 0x2e:
                    f = 0.0033427731821748059;
                    EarthRadius = 6377397.155;
                    X = -148.0;
                    Y = 507.0;
                    Z = 685.0;
                    DatumName = "Tokyo/TD";
                    return;

                case 0x2f:
                    f = 0.0033670033670033669;
                    EarthRadius = 6378388.0;
                    X = -632.0;
                    Y = 438.0;
                    Z = -609.0;
                    DatumName = "Tristan Astro 1968";
                    return;

                case 0x30:
                    f = 0.003407561378699334;
                    EarthRadius = 6378249.145;
                    X = 51.0;
                    Y = 391.0;
                    Z = -36.0;
                    DatumName = "Viti Levu 1916 (Fiji)";
                    return;

                case 0x31:
                    f = 0.0033670033670033669;
                    EarthRadius = 6378388.0;
                    X = 276.0;
                    Y = -57.0;
                    Z = 149.0;
                    DatumName = "Wake island Astro 1952";
                    return;

                case 50:
                    f = 0.003407561378699334;
                    EarthRadius = 6378249.145;
                    X = 0.0;
                    Y = 0.0;
                    Z = 0.0;
                    DatumName = "Yof Astro 1967 (Dakar)*";
                    return;

                case 0x33:
                    f = 0.0033670033670033669;
                    EarthRadius = 6378388.0;
                    X = -104.0;
                    Y = -129.0;
                    Z = 239.0;
                    DatumName = "Palmer Astro 1969 (Antarctica)";
                    return;

                case 0x34:
                    f = 0.0033670033670033669;
                    EarthRadius = 6378388.0;
                    X = -127.0;
                    Y = -769.0;
                    Z = 472.0;
                    DatumName = "Efate (New Hebrides)";
                    return;

                case 0x35:
                    f = 0.0033670033670033669;
                    EarthRadius = 6378388.0;
                    X = 124.0;
                    Y = -234.0;
                    Z = -25.0;
                    DatumName = "Marcus Island 1965";
                    return;

                case 0x36:
                    f = 0.0033670033670033669;
                    EarthRadius = 6378388.0;
                    X = 298.0;
                    Y = -304.0;
                    Z = -375.0;
                    DatumName = "Canton Astro 1966";
                    return;

                case 0x38:
                    f = 0.0033670033670033669;
                    EarthRadius = 6378388.0;
                    X = 0.0;
                    Y = 0.0;
                    Z = 0.0;
                    DatumName = "Yap Island*";
                    return;

                case 0x3a:
                    f = 0.0033670033670033669;
                    EarthRadius = 6378388.0;
                    X = -288.0;
                    Y = 175.0;
                    Z = -376.0;
                    DatumName = "Kourou (French Guiana)";
                    return;

                case 0x3b:
                    f = 0.0033408506414970775;
                    EarthRadius = 6377563.396;
                    X = 0.0;
                    Y = 0.0;
                    Z = 0.0;
                    DatumName = "Ordnance Survay of Great Britain 1970 *";
                    return;

                case 60:
                    f = 0.0033670033670033669;
                    EarthRadius = 6378388.0;
                    X = 164.0;
                    Y = 138.0;
                    Z = -189.0;
                    DatumName = "Qornoq (Greenland)";
                    return;

                case 0x3d:
                    f = 0.003407561378699334;
                    EarthRadius = 6378249.145;
                    X = -166.0;
                    Y = -15.0;
                    Z = 204.0;
                    DatumName = "Adindan (Ethiopia)";
                    return;

                case 0x3e:
                    f = 0.0033900753040885176;
                    EarthRadius = 6378206.4;
                    X = -115.0;
                    Y = 118.0;
                    Z = 426.0;
                    DatumName = "American Samoa 1962";
                    return;

                case 0x3f:
                    f = 0.003407561378699334;
                    EarthRadius = 6378249.145;
                    X = -136.0;
                    Y = -108.0;
                    Z = -292.0;
                    DatumName = "Arc-Cape/Cape (South Africa)";
                    return;

                case 0x40:
                    f = 0.0033670033670033669;
                    EarthRadius = 6378388.0;
                    X = -148.0;
                    Y = 136.0;
                    Z = 90.0;
                    DatumName = "Campo Inchauspe (Argentine)";
                    return;

                case 0x41:
                    f = 0.0033670033670033669;
                    EarthRadius = 6378388.0;
                    X = -205.0;
                    Y = 107.0;
                    Z = 53.0;
                    DatumName = "Ascension Island 1958";
                    return;

                case 0x42:
                    f = 0.0033528918692372171;
                    EarthRadius = 6378160.0;
                    X = -133.0;
                    Y = -48.0;
                    Z = 148.0;
                    DatumName = "Australian Geodetic/AU1966";
                    return;

                case 0x43:
                    f = 0.0033900753040885176;
                    EarthRadius = 6378206.4;
                    X = -73.0;
                    Y = -213.0;
                    Z = 296.0;
                    DatumName = "Bermuda 1957";
                    return;

                case 0x44:
                    f = 0.0033427731821748059;
                    EarthRadius = 6377397.155;
                    X = 0.0;
                    Y = 0.0;
                    Z = 0.0;
                    DatumName = "Berne 1898*";
                    return;

                case 0x45:
                    f = 0.0033670033670033669;
                    EarthRadius = 6378388.0;
                    X = 0.0;
                    Y = 0.0;
                    Z = 0.0;
                    DatumName = "Betio Island 1966*";
                    return;

                case 70:
                    f = 0.0033670033670033669;
                    EarthRadius = 6378388.0;
                    X = -104.0;
                    Y = -129.0;
                    Z = 239.0;
                    DatumName = "Camp Area Astro 1961-62 USGS";
                    return;

                case 0x47:
                    f = 0.0033427731821748059;
                    EarthRadius = 6377397.155;
                    X = -377.0;
                    Y = 681.0;
                    Z = -50.0;
                    DatumName = "Batavia (Java)";
                    return;

                case 0x48:
                    f = 0.0033670033670033669;
                    EarthRadius = 6378388.0;
                    X = -103.0;
                    Y = -106.0;
                    Z = -141.0;
                    DatumName = "Palestine (Israel, Jordan)";
                    return;

                case 0x49:
                    f = 0.0033427731821748059;
                    EarthRadius = 6377397.155;
                    X = 682.0;
                    Y = -203.0;
                    Z = 480.0;
                    DatumName = "Hermannskogel (Austria, Czech, Yugoslavia)";
                    return;

                case 0x4a:
                    f = 0.003324449296662885;
                    EarthRadius = 6377276.345;
                    X = -97.0;
                    Y = 787.0;
                    Z = 86.0;
                    DatumName = "Kandawala (Sri Lanka)";
                    return;

                case 80:
                    f = 0.0033528106647474805;
                    EarthRadius = 6378137.0;
                    X = 0.0;
                    Y = 0.0;
                    Z = 0.0;
                    DatumName = "ETRS89 (European Terrestial Reference System)";
                    return;

                case 0x51:
                    f = 0.0033900753040885176;
                    EarthRadius = 6378206.4;
                    X = 0.0;
                    Y = 0.0;
                    Z = 0.0;
                    DatumName = "Amersfoort 1885 (Netherlands)";
                    return;

                case 0x52:
                    f = 0.0033528106647474805;
                    EarthRadius = 6378137.0;
                    X = 0.0;
                    Y = 0.0;
                    Z = 0.0;
                    DatumName = "NAD83/NAD1983";
                    return;

                case 0x54:
                    f = 0.0033528106647474805;
                    EarthRadius = 6378137.0;
                    X = 0.0;
                    Y = 0.0;
                    Z = 0.0;
                    DatumName = "WGS84";
                    return;

                case 0x55:
                    f = 0.0033528106647474805;
                    EarthRadius = 6378137.0;
                    X = 0.0;
                    Y = 0.0;
                    Z = 0.0;
                    DatumName = "JGD2000";
                    return;

                case 0x56:
                    f = 0.0033528106647474805;
                    EarthRadius = 6378137.0;
                    X = 0.0;
                    Y = 0.0;
                    Z = 0.0;
                    DatumName = "GDA94";
                    return;

                case 0x57:
                    f = 0.0033528106647474805;
                    EarthRadius = 6378137.0;
                    X = 0.0;
                    Y = 0.0;
                    Z = 0.0;
                    DatumName = "NZGD2000";
                    return;

                case 0x58:
                    f = 0.0033528106647474805;
                    EarthRadius = 6378137.0;
                    X = 0.0;
                    Y = 0.0;
                    Z = 0.0;
                    DatumName = "NGRF2000";
                    return;

                case 0x59:
                    f = 0.0033528106647474805;
                    EarthRadius = 6378137.0;
                    X = 0.0;
                    Y = 0.0;
                    Z = 0.0;
                    DatumName = "KDG2000";
                    return;

                case 90:
                    f = 0.0033528106647474805;
                    EarthRadius = 6378137.0;
                    X = 0.0;
                    Y = 0.0;
                    Z = 0.0;
                    DatumName = "Hartebeesthoek94";
                    return;

                case 0x5b:
                    f = 0.0033528106647474805;
                    EarthRadius = 6378137.0;
                    X = 0.0;
                    Y = 0.0;
                    Z = 0.0;
                    DatumName = "TWD94";
                    return;
            }
            f = 0.0033528106647474805;
            EarthRadius = 6378137.0;
            X = 0.0;
            Y = 0.0;
            Z = 0.0;
            DatumName = "No datum";
        }

    }
}
