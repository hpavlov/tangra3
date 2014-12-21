using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Tangra.KweeVanWoerden
{
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
        /// Converts a right ascension string in formats +DD:MM:SS.S; +DD:MM:SS; +DD:MM.M; +DD.D; +DD MM SS.S; HH MM SS; HH MM.M; HH.H to a double number
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
            int sign = System.Math.Sign(value);
            string signs = sign > 0 ? "+" : "-";
            value = System.Math.Abs(value);
            int dd = (int)value;
            double mmd = (value - dd) * 60.0;
            int mm = (int)mmd;
            double ssd = (mmd - mm) * 60.0;
            int ss = (int)ssd;
            int ssr = (int)System.Math.Round(ssd);

            int t = (int)((ssd - ss) * 10);
            int tt = (int)((ssd - ss) * 100);

            if (format.IndexOf("+") > -1)
                format = format.Replace("+", signs);
            else
                dd = dd * sign;

            format = format.Replace("DD", dd.ToString("00"));
            format = format.Replace("HH", dd.ToString("00"));
            format = format.Replace("MM", mm.ToString("00"));

            if (format.IndexOf("TT") > -1 ||
                format.IndexOf("T") > -1)
            {
                format = format.Replace("SS", ss.ToString("00"));
                format = format.Replace("TT", tt.ToString("00"));
                format = format.Replace("T", t.ToString("0"));
            }
            else
            {
                format = format.Replace("SS", ssr.ToString("00"));
            }



            return format;
        }

        private static Regex coordinateFormat0 = new Regex("^\\s*\\d+([\\.,]\\d+)?\\s*$");
        private static Regex coordinateFormat1 = new Regex("^\\s*\\d+\\s+\\d+([\\.,]\\d+)?\\s*$");
        private static Regex coordinateFormat2 = new Regex("^\\s*\\d+\\s+\\d+\\s+\\d+\\s*$");
        private static Regex coordinateFormat3 = new Regex("^\\s*\\d+\\s+\\d+\\s+\\d+([\\.,]\\d+)?\\s*$");
        private static Regex digitSequences = new Regex("\\d+([\\.,]\\d+)?");

        public static bool ReadObserverCoord(string valueToParse, out double value, out int format)
        {
            return ReadObserverCoord(valueToParse, false, out value, out format);
        }

        public static bool ReadObserverCoord(string valueToParse, bool allowSign, out double value, out int format)
        {
            int sign = 1;

            if (allowSign)
            {
                valueToParse = valueToParse.TrimStart();
                if (valueToParse.Length > 0)
                {
                    if (valueToParse.StartsWith("-"))
                    {
                        valueToParse = valueToParse.TrimStart('-');
                        sign = -1;
                    }
                    else if (valueToParse.StartsWith("+"))
                    {
                        valueToParse = valueToParse.TrimStart('+');
                    }
                }
            }
            if (coordinateFormat0.IsMatch(valueToParse)) format = 0;
            else if (coordinateFormat1.IsMatch(valueToParse)) format = 1;
            else if (coordinateFormat2.IsMatch(valueToParse)) format = 2;
            else if (coordinateFormat3.IsMatch(valueToParse)) format = 3;
            else
                format = -1;

            if (format != -1)
            {
                MatchCollection digits = digitSequences.Matches(valueToParse);
                value = 0;
                for (int i = 0; i < digits.Count; i++)
                {
                    double partVal = double.Parse(digits[i].Value.Replace(',', '.'), CultureInfo.InvariantCulture);
                    value = value + partVal / System.Math.Pow(60, i);
                }

                value = sign * value;

                return true;
            }
            else
            {
                value = double.NaN;
                return false;
            }
        }
    }
}
