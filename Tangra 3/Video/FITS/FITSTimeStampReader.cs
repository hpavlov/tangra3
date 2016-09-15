using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using nom.tam.fits;
using Tangra.Model.Config;

namespace Tangra.Video.FITS
{
    public class FITSTimeStampReader
    {
        private TangraConfig.FITSFieldConfig m_Config;

        public FITSTimeStampReader(TangraConfig.FITSFieldConfig config)
        {
            m_Config = config;
        }

        private DateTime? ParseExposureCase1(Header header, out bool isMidPoint, out double? fitsExposure)
        {
            isMidPoint = true;
            fitsExposure = null;

            var timeStampCard = header.FindCard(m_Config.TimeStampHeader);
            var exposureCard = header.FindCard(m_Config.ExposureHeader);
            if (timeStampCard != null && exposureCard != null)
            {
                double parsedExposure;
                if (double.TryParse(exposureCard.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out parsedExposure))
                    fitsExposure = ConvertExposureInSeconds(parsedExposure);

                DateTime parsedTimeStamp;
                if (DateTime.TryParseExact(timeStampCard.Value, m_Config.TimeStampFormat, CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal, out parsedTimeStamp))
                {
                    switch (m_Config.TimeStampType)
                    {
                        case TangraConfig.TimeStampType.StartExposure:
                            if (fitsExposure != null)
                            {
                                isMidPoint = true;
                                return parsedTimeStamp.AddSeconds(0.5*parsedExposure);
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
                                return parsedTimeStamp.AddSeconds(-0.5*parsedExposure);
                            }
                            else
                                return parsedTimeStamp;
                    }
                }
            }

            return null;
        }

        private DateTime? ParseExposureCase2(Header header, out bool isMidPoint, out double? fitsExposure)
        {
            throw new NotImplementedException();
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


        public DateTime? ParseExposure(Header header, out bool isMidPoint, out double? fitsExposure)
        {
            if (m_Config.IsTimeStampAndExposure)
                return ParseExposureCase1(header, out isMidPoint, out fitsExposure);
            else
                return ParseExposureCase2(header, out isMidPoint, out fitsExposure);
        }
    }
}
