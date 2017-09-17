using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using nom.tam.fits;
using Tangra.Model.Config;
using Tangra.Model.Helpers;

namespace Tangra.Video.FITS
{
    public class HeaderEntry
    {
        public HeaderCard Card;

        public HeaderEntry(HeaderCard card)
        {
            Card = card;
        }

        public override string ToString()
        {
            return Card.Key;
        }
    }

    public class FitsTimestampHelper
    {
        private string m_FilesHash;
        private List<HeaderEntry> m_AllCards = new List<HeaderEntry>();

        private Action<TangraConfig.FITSFieldConfig> UseRecentFITSConfig;

        public FitsTimestampHelper(string fileHash, List<HeaderEntry> allCards, Action<TangraConfig.FITSFieldConfig> useRecentFITSConfigCallback)
        {
            m_FilesHash = fileHash;
            m_AllCards = allCards;
            UseRecentFITSConfig = useRecentFITSConfigCallback;
        }

        public void TryIdentifyPreviousConfigApplyingForCurrentFiles()
        {
            if (TangraConfig.Settings.RecentFITSFieldConfig.Items.Count > 0)
            {
                var sameHashConfig = TangraConfig.Settings.RecentFITSFieldConfig.Items.FirstOrDefault(x => x.FileHash == m_FilesHash);
                if (sameHashConfig != null && RecentFITSConfigMatches(sameHashConfig))
                {
                    UseRecentFITSConfig(sameHashConfig);
                    return;
                }

                foreach (var config in TangraConfig.Settings.RecentFITSFieldConfig.Items)
                {
                    if (RecentFITSConfigMatches(config))
                    {
                        UseRecentFITSConfig(config);
                        return;
                    }
                }
            }
        }

        private bool RecentFITSConfigMatches(TangraConfig.FITSFieldConfig config)
        {
            if (config.IsTimeStampAndExposure)
            {
                var exposureCard = m_AllCards.FirstOrDefault(x => x.Card.Key == config.ExposureHeader);
                var timestampCard = m_AllCards.FirstOrDefault(x => x.Card.Key == config.TimeStampHeader);

                if (exposureCard != null && timestampCard != null)
                {
                    return
                        VerifyExposure(exposureCard.Card.Value) &&
                        VerifyTimeStamp(timestampCard.Card.Value, config.TimeStampFormat);
                }
                else
                    return false;
            }
            else
            {
                var startTimestampCard = m_AllCards.FirstOrDefault(x => x.Card.Key == config.TimeStampHeader);
                var endTimestampCard = m_AllCards.FirstOrDefault(x => x.Card.Key == config.TimeStamp2Header);

                if (startTimestampCard != null && endTimestampCard != null)
                {
                    return
                        VerifyTimeStamp(startTimestampCard.Card.Value, config.TimeStampFormat) &&
                        VerifyTimeStamp(endTimestampCard.Card.Value, config.TimeStamp2Format);
                }
                else
                    return false;
            }
        }

        public bool VerifyExposure(string value)
        {
            try
            {
                var parsedValue = double.Parse(value, CultureInfo.InvariantCulture);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool VerifyTimeStamp(string value, string format)
        {
            try
            {
                if (format == FITSTimeStampReader.SECONDS_FROM_MIDNIGHT)
                {
                    double secs;
                    if (!double.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out secs) || secs < 0 || secs > 3600*24)
                        return false;
                }
                else
                {
                    var parsedTimestamp = DateTime.ParseExact(value, format, CultureInfo.InvariantCulture);    
                }
                
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool VerifyTimeStamp(string dateValue, string dateFormat, string timeValue, string timeFormat)
        {
            try
            {
                var parsedDate = DateTime.ParseExact(dateValue, dateFormat, CultureInfo.InvariantCulture);

                if (timeFormat == FITSTimeStampReader.SECONDS_FROM_MIDNIGHT)
                {
                    double secs;
                    if (!double.TryParse(timeValue, NumberStyles.Number, CultureInfo.InvariantCulture, out secs) || secs < 0 || secs > 3600 * 24)
                        return false;
                }
                else
                {
                    var parsedTime = DateTime.ParseExact(timeValue, timeFormat, CultureInfo.InvariantCulture);
                }
                
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
