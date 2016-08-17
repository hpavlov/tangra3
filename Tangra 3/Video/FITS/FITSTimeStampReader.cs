using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using nom.tam.fits;

namespace Tangra.Video.FITS
{
    public enum ExposureUnit
    {
        Seconds,
        Milliseconds,
        Microseconds,
        Nanoseconds,
        Minutes,
        Hours,
        Days
    }

    public enum TimeStampType
    {
        StartExposure,
        MidExposure,
        EndExposure
    }

    public class FITSTimeStampReader
    {
        private string m_ExposureHeader;
        private ExposureUnit m_ExposureUnit;
        private string m_TimeStampHeader;
        private string m_TimeStampFormat;
        private TimeStampType m_TimeStampType;
        private string m_TimeStamp2Header;
        private string m_TimeStamp2Format;

        private bool m_IsStartEndTimeStampParsing = false;

        public FITSTimeStampReader(string exposureHeader, ExposureUnit exposureUnit, string timeStampHeader, string timeStampFormat, TimeStampType timeStampType)
        {
            m_IsStartEndTimeStampParsing = false;

            m_ExposureHeader = exposureHeader;
            m_ExposureUnit = exposureUnit;
            m_TimeStampHeader = timeStampHeader;
            m_TimeStampFormat = timeStampFormat;
            m_TimeStampType = timeStampType;
        }

        public FITSTimeStampReader(string startTimeStampHeader, string startTimeStampFormat, string endTimeStampHeader, string endTimeStampFormat)
        {
            m_IsStartEndTimeStampParsing = true;

            m_TimeStampHeader = startTimeStampHeader;
            m_TimeStampFormat = startTimeStampFormat;
            m_TimeStampType = TimeStampType.StartExposure;
            m_TimeStamp2Header = endTimeStampHeader;
            m_TimeStamp2Format = endTimeStampFormat;
        }

        private DateTime? ParseExposureCase1(Header header, out bool isMidPoint, out double? fitsExposure)
        {
            throw new NotImplementedException();
        }

        private DateTime? ParseExposureCase2(Header header, out bool isMidPoint, out double? fitsExposure)
        {
            isMidPoint = true;
            fitsExposure = null;

            var timeStampCard = header.FindCard(m_TimeStampHeader);
            var exposureCard = header.FindCard(m_ExposureHeader);
            if (timeStampCard != null && exposureCard != null)
            {
                double parsedExposure;
                if (double.TryParse(exposureCard.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out parsedExposure))
                    fitsExposure = ConvertExposureInSeconds(parsedExposure);

                DateTime parsedTimeStamp;
                if (DateTime.TryParseExact(timeStampCard.Value, m_TimeStampFormat, CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal, out parsedTimeStamp))
                {
                    switch (m_TimeStampType)
                    {
                        case TimeStampType.StartExposure:
                            if (fitsExposure != null)
                            {
                                isMidPoint = true;
                                return parsedTimeStamp.AddSeconds(0.5*parsedExposure);
                            }
                            else
                                return parsedTimeStamp;

                        case TimeStampType.MidExposure:
                            isMidPoint = true;
                            return parsedTimeStamp;

                        case TimeStampType.EndExposure:
                            if (fitsExposure != null)
                            {
                                isMidPoint = true;
                                return parsedTimeStamp.AddSeconds(-0.5*parsedExposure);
                            }
                            else
                                return parsedTimeStamp;
                    }
                }
            }

            return null;
        }

        private double ConvertExposureInSeconds(double exposure)
        {
            switch (m_ExposureUnit)
            {
                case ExposureUnit.Nanoseconds:
                    return exposure * 1E-09;
                case ExposureUnit.Microseconds:
                    return exposure * 1E-06;
                case ExposureUnit.Milliseconds:
                    return exposure * 1E-03;
                case ExposureUnit.Seconds:
                    return exposure;
                case ExposureUnit.Minutes:
                    return exposure * 60;
                case ExposureUnit.Hours:
                    return exposure * 3600;
                case ExposureUnit.Days:
                    return exposure * 24 * 3600;
                default:
                    throw new ArgumentOutOfRangeException(string.Format("Unsupported FITS ExposureUnit: '{0}'.", m_ExposureUnit));
            }
        }

        public DateTime? ParseExposure(Header header, out bool isMidPoint, out double? fitsExposure)
        {
            if (m_IsStartEndTimeStampParsing)
                return ParseExposureCase1(header, out isMidPoint, out fitsExposure);
            else
                return ParseExposureCase2(header, out isMidPoint, out fitsExposure);
        }
    }
}
