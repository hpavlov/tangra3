using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using nom.tam.fits;
using Tangra.Model.Config;

namespace Tangra.Model.Helpers
{
    public interface IFITSTimeStampReader
    {
        DateTime? ParseExposure(string fileName, Header header, out bool isMidPoint, out double? fitsExposure);
    }

    public class FITSTimeStampReader : IFITSTimeStampReader
    {
        private TangraConfig.FITSFieldConfig m_Config;

        public const string SECONDS_FROM_MIDNIGHT = "SECONDS-FROM-MIDNIGHT";

        public static string[] STANDARD_DATE_FORMATS = new string[] { "yyyy-MM-dd", "dd/MM/yyyy" };
        public static string[] STANDARD_TIME_FORMATS = new string[] { "HH:mm:ss.fff", SECONDS_FROM_MIDNIGHT };
        public static string[] STANDARD_TIMESTAMP_FORMATS = new string[] { "yyyy-MM-ddTHH:mm:ss.fff", "dd/MM/yyyy HH:mm:ss.fff" };

        public FITSTimeStampReader(TangraConfig.FITSFieldConfig config)
        {
            m_Config = config;
        }

        private DateTime? ParseExposureCase1(Header header, out bool isMidPoint, out double? fitsExposure)
        {
            isMidPoint = true;
            fitsExposure = null;

            var exposureCard = header.FindCard(m_Config.ExposureHeader);
            if (exposureCard != null && TimeStampCardsFound(header))
            {
                double parsedExposure;
                if (double.TryParse(exposureCard.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out parsedExposure))
                    fitsExposure = ConvertExposureInSeconds(parsedExposure);

                DateTime parsedTimeStamp;
                if (ParseFirstTimeStamp(header, out parsedTimeStamp))
                {
                    switch (m_Config.TimeStampType)
                    {
                        case TangraConfig.TimeStampType.StartExposure:
                            if (fitsExposure != null)
                            {
                                isMidPoint = true;
                                return parsedTimeStamp.AddSeconds(0.5 * fitsExposure.Value);
                            }
                            else
                                return parsedTimeStamp;

                        case TangraConfig.TimeStampType.MidExposure:
                            isMidPoint = true;
                            return parsedTimeStamp;

                        case TangraConfig.TimeStampType.EndExposure:
                            if (fitsExposure != null)
                            {
                                isMidPoint = true;
                                return parsedTimeStamp.AddSeconds(-0.5 * fitsExposure.Value);
                            }
                            else
                                return parsedTimeStamp;
                    }
                }
            }

            return null;
        }

        private bool TimeStampCardsFound(Header header)
        {
            if (m_Config.IsTimeStampAndExposure)
            {
                if (m_Config.TimeStampIsDateTimeParts)
                    return header.FindCard(m_Config.TimeStampHeader) != null && header.FindCard(m_Config.TimeStampHeader2) != null;
                else
                    return header.FindCard(m_Config.TimeStampHeader) != null;
            }
            else
            {
                if (m_Config.TimeStamp2IsDateTimeParts)
                    return 
                        header.FindCard(m_Config.TimeStampHeader) != null && header.FindCard(m_Config.TimeStampHeader2) != null &&
                        header.FindCard(m_Config.TimeStamp2Header) != null && header.FindCard(m_Config.TimeStamp2Header2) != null;
                else
                    return header.FindCard(m_Config.TimeStampHeader) != null && header.FindCard(m_Config.TimeStamp2Header) != null;
            }
        }

        private bool ParseFirstTimeStamp(Header header, out DateTime parsedTimeStamp)
        {
            return ParseTimeStampInternal(
                header,
                () => m_Config.TimeStampIsDateTimeParts,
                () => m_Config.TimeStampHeader,
                () => m_Config.TimeStampHeader2,
                () => m_Config.TimeStampFormat,
                () => m_Config.TimeStampFormat2,
                out parsedTimeStamp);
        }

        private bool ParseSecondTimeStamp(Header header, out DateTime parsedTimeStamp)
        {
            return ParseTimeStampInternal(
                header, 
                () => m_Config.TimeStamp2IsDateTimeParts, 
                () => m_Config.TimeStamp2Header,
                () => m_Config.TimeStamp2Header2,
                () => m_Config.TimeStamp2Format,
                () => m_Config.TimeStamp2Format2,
                out parsedTimeStamp);
        }

        private bool ParseTimeStampInternal(Header header, Func<bool> getIsDateTimeParts, Func<string> getHeaderName, Func<string> getHeader2Name, Func<string> getFormat, Func<string> getFormat2, out DateTime parsedTimeStamp)
        {
            if (getIsDateTimeParts())
            {
                var dateCard = header.FindCard(getHeaderName());
                
                DateTime time;
                if (DateTime.TryParseExact(dateCard.Value, getFormat(), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out parsedTimeStamp))
                {
                    var timeFormat = getFormat2();
                    var timeCard = header.FindCard(getHeader2Name());
                    if (timeFormat == SECONDS_FROM_MIDNIGHT)
                    {
                        double secondsSinceMidnight;
                        if (!double.TryParse(timeCard.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out secondsSinceMidnight))
                            return false;

                        parsedTimeStamp = parsedTimeStamp.Date.AddSeconds(secondsSinceMidnight);
                    }
                    else
                    {
                        if (!DateTime.TryParseExact(timeCard.Value, timeFormat, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out time))
                            return false;

                        parsedTimeStamp = parsedTimeStamp.Date.Add(time.TimeOfDay);
                    }

                    return true;
                }
            }
            else
            {
                var timeStampCard = header.FindCard(getHeaderName());

                if (DateTime.TryParseExact(timeStampCard.Value, getFormat(), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out parsedTimeStamp))
                    return true;
            }

            return false;
        }

        private DateTime? ParseExposureCase2(Header header, out bool isMidPoint, out double? fitsExposure)
        {
            isMidPoint = true;
            fitsExposure = null;

            if (TimeStampCardsFound(header))
            {
                DateTime parsedStartTimeStamp;
                DateTime parsedEndTimeStamp;
                if (ParseFirstTimeStamp(header, out parsedStartTimeStamp) && ParseSecondTimeStamp(header, out parsedEndTimeStamp))
                {
                    var exposureSec = (parsedEndTimeStamp - parsedStartTimeStamp).TotalSeconds;
                    fitsExposure = exposureSec;
                    return parsedStartTimeStamp.AddSeconds(0.5 * exposureSec);
                }
            }

            return null;
        }

        private double ConvertExposureInSeconds(double exposure)
        {
            switch (m_Config.ExposureUnit)
            {
                case TangraConfig.ExposureUnit.Nanoseconds:
                    return exposure * 1E-09;
                case TangraConfig.ExposureUnit.Microseconds:
                    return exposure * 1E-06;
                case TangraConfig.ExposureUnit.Milliseconds:
                    return exposure * 1E-03;
                case TangraConfig.ExposureUnit.Seconds:
                    return exposure;
                case TangraConfig.ExposureUnit.Minutes:
                    return exposure * 60;
                case TangraConfig.ExposureUnit.Hours:
                    return exposure * 3600;
                case TangraConfig.ExposureUnit.Days:
                    return exposure * 24 * 3600;
                default:
                    throw new ArgumentOutOfRangeException(string.Format("Unsupported FITS ExposureUnit: '{0}'.", m_Config.ExposureUnit));
            }
        }


        public DateTime? ParseExposure(string fileName, Header header, out bool isMidPoint, out double? fitsExposure)
        {
            if (m_Config.IsTimeStampAndExposure)
                return ParseExposureCase1(header, out isMidPoint, out fitsExposure);
            else
                return ParseExposureCase2(header, out isMidPoint, out fitsExposure);
        }

        public string GetTimeStampHeaders()
        {
            var headers = new List<string>();

            if (m_Config.IsTimeStampAndExposure)
            {
                headers.Add(m_Config.ExposureHeader);
                headers.Add(m_Config.TimeStampHeader);
                if (m_Config.TimeStampIsDateTimeParts) headers.Add(m_Config.TimeStampHeader2);
            }
            else
            {
                headers.Add(m_Config.ExposureHeader);
                headers.Add(m_Config.TimeStampHeader);
                headers.Add(m_Config.TimeStamp2Header);
                if (m_Config.TimeStamp2IsDateTimeParts)
                {
                    headers.Add(m_Config.TimeStampHeader2);
                    headers.Add(m_Config.TimeStamp2Header2);
                }
            }

            return string.Join(", ", headers.Take(headers.Count - 1)) + " and " + headers[headers.Count - 1];
        }
    }
}
