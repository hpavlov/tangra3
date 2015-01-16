using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace Tangra.Model.Helpers
{
    public static class Precision
    {
        // 2^-24
        public const float FLOAT_EPSILON = 0.0000000596046448f;

        // 2^-53
        public const double DOUBLE_EPSILON = 0.00000000000000011102230246251565d;

        public static bool AlmostEquals(this double a, double b, double epsilon = DOUBLE_EPSILON)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            if (a == b)
            {
                return true;
            }
            // ReSharper restore CompareOfFloatsByEqualityOperator

            return (System.Math.Abs(a - b) < epsilon);
        }

        public static bool AlmostEquals(this float a, float b, float epsilon = FLOAT_EPSILON)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            if (a == b)
            {
                return true;
            }
            // ReSharper restore CompareOfFloatsByEqualityOperator

            return (System.Math.Abs(a - b) < epsilon);
        }
    }

    public static class SignificantDigits
    {
        public static double Round(this double value, int significantDigits)
        {
            int unneededRoundingPosition;
            return RoundSignificantDigits(value, significantDigits, out unneededRoundingPosition);
        }

        public static string ToString(this double value, int significantDigits)
        {
            // this method will round and then append zeros if needed.
            // i.e. if you round .002 to two significant figures, the resulting number should be .0020.

            var currentInfo = CultureInfo.InvariantCulture.NumberFormat;

            if (double.IsNaN(value))
            {
                return currentInfo.NaNSymbol;
            }

            if (double.IsPositiveInfinity(value))
            {
                return currentInfo.PositiveInfinitySymbol;
            }

            if (double.IsNegativeInfinity(value))
            {
                return currentInfo.NegativeInfinitySymbol;
            }

            int roundingPosition;
            var roundedValue = RoundSignificantDigits(value, significantDigits, out roundingPosition);

            // when rounding causes a cascading round affecting digits of greater significance, 
            // need to re-round to get a correct rounding position afterwards
            // this fixes a bug where rounding 9.96 to 2 figures yeilds 10.0 instead of 10
            RoundSignificantDigits(roundedValue, significantDigits, out roundingPosition);

            if (Math.Abs(roundingPosition) > 9)
            {
                // use exponential notation format
                // ReSharper disable FormatStringProblem
                return string.Format(currentInfo, "{0:E" + (significantDigits - 1) + "}", roundedValue);
                // ReSharper restore FormatStringProblem
            }

            // string.format is only needed with decimal numbers (whole numbers won't need to be padded with zeros to the right.)
            // ReSharper disable FormatStringProblem
            return roundingPosition > 0 ? string.Format(currentInfo, "{0:F" + roundingPosition + "}", roundedValue) : roundedValue.ToString(currentInfo);
            // ReSharper restore FormatStringProblem
        }

        private static double RoundSignificantDigits(double value, int significantDigits, out int roundingPosition)
        {
            // this method will return a rounded double value at a number of signifigant figures.
            // the sigFigures parameter must be between 0 and 15, exclusive.

            roundingPosition = 0;

            if (value.AlmostEquals(0d))
            {
                roundingPosition = significantDigits - 1;
                return 0d;
            }

            if (double.IsNaN(value))
            {
                return double.NaN;
            }

            if (double.IsPositiveInfinity(value))
            {
                return double.PositiveInfinity;
            }

            if (double.IsNegativeInfinity(value))
            {
                return double.NegativeInfinity;
            }

            if (significantDigits < 1 || significantDigits > 15)
            {
                throw new ArgumentOutOfRangeException("significantDigits", value, "The significantDigits argument must be between 1 and 15.");
            }

            // The resulting rounding position will be negative for rounding at whole numbers, and positive for decimal places.
            roundingPosition = significantDigits - 1 - (int)(Math.Floor(Math.Log10(Math.Abs(value))));

            // try to use a rounding position directly, if no scale is needed.
            // this is because the scale mutliplication after the rounding can introduce error, although 
            // this only happens when you're dealing with really tiny numbers, i.e 9.9e-14.
            if (roundingPosition > 0 && roundingPosition < 16)
            {
                return Math.Round(value, roundingPosition, MidpointRounding.AwayFromZero);
            }

            // Shouldn't get here unless we need to scale it.
            // Set the scaling value, for rounding whole numbers or decimals past 15 places
            var scale = Math.Pow(10, Math.Ceiling(Math.Log10(Math.Abs(value))));

            return Math.Round(value / scale, significantDigits, MidpointRounding.AwayFromZero) * scale;
        }
    }

    public static class AstroConvert
    {
        /// <summary>
        /// Converts a right ascension string in formats HH:MM:SS.S; HH:MM:SS; HH:MM.M; HH.H; HH MM SS.S; HH MM SS; HH MM.M; HH.H to a double number
        /// </summary>
        /// <param name="hhmmss"></param>
        /// <returns></returns>
        public static double ToRightAcsension(string value)
        {
            return ToDoubleValue(value, false);
        }

        /// <summary>
        /// Converts a declination string in formats +DD:MM:SS.S; +DD:MM:SS; +DD:MM.M; +DD.D; +DD MM SS.S; DD MM SS; DD MM.M; DD.D to a double number
        /// </summary>
        /// <param name="hhmmss"></param>
        /// <returns></returns>
        public static double ToDeclination(string value)
        {
            return ToDoubleValue(value, true);
        }


        private static double ToDoubleValue(string value, bool allowSign)
        {
            if (value == null) throw new ArgumentNullException("value");
            if (value == string.Empty) throw new ArgumentException("'value' cannot be blank");

            value = value.Trim();

            char groupDelimiter = ':';
            if (value.IndexOf(' ') > -1) groupDelimiter = ' ';

            int sign = 1;

            if (allowSign)
            {
                if (value[0] == '-') sign = -1;
                if ("-+".IndexOf(value[0]) > -1)
                {
                    value = value.Substring(1).Trim();
                }
            }

            string[] tokens = value.Split(new char[] { groupDelimiter }, StringSplitOptions.RemoveEmptyEntries);

            if (tokens.Length > 3) throw new FormatException("");

            double retval = double.NaN;

            if (tokens.Length == 3)
            {
                retval = double.Parse(tokens[2], CultureInfo.InvariantCulture) / 3600.0;
                retval += (int.Parse(tokens[1], CultureInfo.InvariantCulture) / 60.0);
                retval += int.Parse(tokens[0], CultureInfo.InvariantCulture);
            }
            else if (tokens.Length == 2)
            {
                retval = (double.Parse(tokens[1], CultureInfo.InvariantCulture) / 60.0);
                retval += int.Parse(tokens[0], CultureInfo.InvariantCulture);
            }
            else if (tokens.Length == 1)
            {
                retval = double.Parse(tokens[0], CultureInfo.InvariantCulture);
            }

            return retval * sign;
        }

		private static string CheckSpecialFormat(string format)
		{
			if (format == "DEC") return "+DD°MM'SS\"";
			if (format == "DEC.1") return "+DD°MM'SS.T\"";

            if (format == "DE") return "+DD°MM'SS\"";
            if (format == "DE.1") return "+DD°MM'SS.T\"";

			if (format == "REC") return "HHhMMmSSs";
			if (format == "REC.1") return "HHhMMmSS.Ts";
            if (format == "REC.2") return "HHhMMmSS.TTs";

            if (format == "RA") return "HHhMMmSSs";
            if (format == "RA.1") return "HHhMMmSS.Ts";
            if (format == "RA.2") return "HHhMMmSS.TTs";

			return format;
		}
        /// <summary>
        /// Converts a double value into a string representation using the specified format. The following
        /// formats are supported:
        /// 
        /// + - adds a plus sign if the value is positive
        /// HH or DD - whole part (degrees or hours)
        /// MM - minutes; SS - seconds; T and TT - parts of a second
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ToStringValue(double value, string format)
        {
            if (double.IsNaN(value))
                return "NaN";

        	format = CheckSpecialFormat(format);

            int sign = Math.Sign(value);
            string signs = sign > 0 ? "+" : "-";
            value = Math.Abs(value);
            int dd = (int)value;
            double mmd = (value - dd) * 60.0;
            int mm = (int)mmd;
            double ssd = (mmd - mm) * 60.0;
            int ss = (int)ssd;
            int ssr = (int)Math.Round(ssd);

            double td = ssd - ss;

            if (format.IndexOf("+") > -1)
                format = format.Replace("+", signs);                
            else
                dd = dd * sign;

            if (format.IndexOf("TT") > -1 ||
                format.IndexOf("T") > -1)
            {
                if (format.IndexOf("TT") > -1)
                {
                    int ttdr = int.Parse((td * 100).ToString("00"));
                    if (ttdr == 100)
                    {
                        ttdr = 0;
                        ss += 1;
                    }
                    format = format.Replace("TT", ttdr.ToString("00"));
                }
                else if (format.IndexOf("T") > -1)
                {
                    int tdr = int.Parse((td * 10).ToString("0"));
                    if (tdr == 10)
                    {
                        tdr = 0;
                        ss += 1;
                    }
                    format = format.Replace("T", tdr.ToString("0"));
                }

                if (ss >= 60)
                {
                    ss -= 60;
                    mm += 1;
                }

                format = format.Replace("SS", ss.ToString("00"));
            }
            else
            {
                format = format.Replace("SS", ssr.ToString("00"));
            }


            if (mm >= 60)
            {
                mm -= 60;
                dd += 1;
            }

            format = format.Replace("MM", mm.ToString("00"));
            format = format.Replace("DDD", dd.ToString("000"));
            format = format.Replace("DD", dd.ToString("00"));
            format = format.Replace("HH", dd.ToString("00"));
            
            return format;
        }
    }
}
