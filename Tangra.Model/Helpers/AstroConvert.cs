using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace Tangra.Model.Helpers
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
            format = format.Replace("DD", dd.ToString("00"));
            format = format.Replace("HH", dd.ToString("00"));
            
            return format;
        }
    }
}
