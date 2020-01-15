using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tangra.Model.Astro;

namespace Tangra.Helpers
{
    public class QHYImageHeader
    {
        public enum QHYGPSStatus
        {
            PoweredUp = 0,
            NoTiming = 1,
            NoPPS = 2,
            PPS = 3
        }

        public static bool IsAvailable(AstroImage image)
        {
            if (image != null && image.Pixelmap != null && image.Pixelmap.Pixels != null &&
                image.Pixelmap.Pixels.Length > 45)
            {
                try
                {
                    var data = image.Pixelmap.Pixels.Take(45).Select(x => (byte)x).ToArray();
                    var hdr = new QHYImageHeader(data);
                    var startTime = hdr.StartTime;
                    var endTime = hdr.EndTime;
                    var lng = hdr.ParseLongitude;
                    var lat = hdr.ParseLatitude;
                    return hdr.MaxClock >= 9999000 &&
                           hdr.MaxClock <= 10000500 &&
                           endTime > startTime &&
                           (endTime - startTime).TotalMinutes < 60 &&
                           endTime.Year > 2015 && endTime.Year < 2100 &&
                           lng >= -360 && lng <= 360 &&
                           lat >= -90 && lat <= 90;

                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.ToString());
                }
            }

            return false;
        }

        public QHYImageHeader(AstroImage image)
            : this(image.Pixelmap.Pixels.Take(45).Select(x => (byte)x).ToArray())
        { }

        private QHYImageHeader(byte[] imageHead)
        {
            SeqNumber = 256 * 256 * 256 * imageHead[0] + 256 * 256 * imageHead[1] + 256 * imageHead[2] + imageHead[3];
            TempNumber = imageHead[4];
            Width = 256 * imageHead[5] + imageHead[6];
            Height = 256 * imageHead[7] + imageHead[8];
            Latitude = 256 * 256 * 256 * imageHead[9] + 256 * 256 * imageHead[10] + 256 * imageHead[11] + imageHead[12];
            Longitude = 256 * 256 * 256 * imageHead[13] + 256 * 256 * imageHead[14] + 256 * imageHead[15] + imageHead[16];

            StartFlag = imageHead[17];
            StartSec = 256 * 256 * 256 * imageHead[18] + 256 * 256 * imageHead[19] + 256 * imageHead[20] + imageHead[21];
            StartUs = 256 * 256 * imageHead[22] + 256 * imageHead[23] + imageHead[24];

            EndFlag = imageHead[25];
            EndSec = 256 * 256 * 256 * imageHead[26] + 256 * 256 * imageHead[27] + 256 * imageHead[28] + imageHead[29];
            EndUs = 256 * 256 * imageHead[30] + 256 * imageHead[31] + imageHead[32];

            NowFlag = imageHead[33];
            NowSec = 256 * 256 * 256 * imageHead[34] + 256 * 256 * imageHead[35] + 256 * imageHead[36] + imageHead[37];
            NowUs = 256 * 256 * imageHead[38] + 256 * imageHead[39] + imageHead[40];
            MaxClock = 256 * 256 * imageHead[41] + 256 * imageHead[42] + imageHead[43];

            // NOTE: Lo and Hi 4-bits of NowFlag are the GPS Status at Start and End exposure. We only use one of the flags here
            GPSStatus = (QHYGPSStatus)((NowFlag >> 4) & 0x0F);
        }

        public readonly QHYGPSStatus GPSStatus;
        public readonly int SeqNumber;
        public readonly int TempNumber;
        public readonly int Width;
        public readonly int Height;
        public readonly int Latitude;
        public readonly int Longitude;

        public double ParseLatitude
        {
            get
            {
                int part = Latitude % 100000;
                int min = (Latitude / 100000) % 100;
                int deg = (Latitude / 10000000) % 100;
                int sign = (Latitude / 1000000000) == 1 ? -1 : 1;
                return sign * (deg + min / 60.0 + (part / 100000.0) / 60.0);
            }
        }

        public double ParseLongitude
        {
            get
            {
                int part = Longitude % 10000;
                int min = (Longitude / 10000) % 100;
                int deg = (Longitude / 1000000) % 1000;
                int sign = (Longitude / 1000000000) == 1 ? -1 : 1;
                return sign * (deg + min / 60.0 + (part / 10000.0) / 60.0);

            }
        }

        public readonly int StartFlag;
        public readonly int StartSec;
        public readonly int StartUs;
        public DateTime StartTime
        {
            get
            {
                return new DateTime(1900, 1, 1, 12, 0, 0).AddDays(34979.5 + StartSec / 3600.0 / 24.0).AddTicks(StartUs);
            }
        }

        public readonly int EndFlag;
        public readonly int EndSec;
        public readonly int EndUs;
        public DateTime EndTime
        {
            get
            {
                return new DateTime(1900, 1, 1, 12, 0, 0).AddDays(34979.5 + EndSec / 3600.0 / 24.0).AddTicks(EndUs);
            }
        }

        public readonly int NowFlag;
        public readonly int NowSec;
        public readonly int NowUs;
        public readonly int MaxClock;

        public bool GpsTimeAvailable
        {
            get { return MaxClock < 10000500; }
        }
    }
}
