using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adv;
using Tangra.Model.Image;

namespace Tangra.VideoTools
{
    public class AavFileCreator
    {
        private static uint STATUS_TAG_SYSTEM_TIME;
        private static uint STATUS_TAG_START_FRAME_ID;
        private static uint STATUS_TAG_END_FRAME_ID;
        private static uint STATUS_TAG_NUMBER_INTEGRATED_FRAMES;
        private static uint STATUS_TAG_EXPOSURE;
        private static uint STATUS_TAG_NTP_START_TIMESTAMP;
        private static uint STATUS_TAG_NTP_END_TIMESTAMP;
        private static uint STATUS_TAG_NTP_TIME_ERROR;

        private static long s_FirstRecordedFrameTimestamp;
        private static int s_Width;
        private static int s_Height;
        private static int s_IntegrationRate;

        public static void CloseFile()
        {
            AdvLib.Obsolete.AdvVer1.EndFile();
        }

        public static void StartNewFile(string fileName, int width, int height, int integrationRate)
        {
            AdvLib.Obsolete.AdvVer1.NewFile(fileName);

            s_FirstRecordedFrameTimestamp = 0;
            s_Width = width;
            s_Height = width;
            s_IntegrationRate = integrationRate;

            AdvLib.Obsolete.AdvVer1.AddFileTag("AAVR-SOFTWARE-VERSION", "1.0");
            AdvLib.Obsolete.AdvVer1.AddFileTag("RECORDER", "Tangra Video Model Simulator");
            AdvLib.Obsolete.AdvVer1.AddFileTag("FSTF-TYPE", "AAV");
            AdvLib.Obsolete.AdvVer1.AddFileTag("AAV-VERSION", "1");

            AdvLib.Obsolete.AdvVer1.AddFileTag("GRABBER", "");
            AdvLib.Obsolete.AdvVer1.AddFileTag("VIDEO-MODE", string.Format("{0}x{1}@25fps", width, height));
            AdvLib.Obsolete.AdvVer1.AddFileTag("CAMERA-MODEL", "");
            AdvLib.Obsolete.AdvVer1.AddFileTag("CAMERA-BITPIX", "8");

            AdvLib.Obsolete.AdvVer1.AddFileTag("FRAME-COMBINING", "Binning");

            AdvLib.Obsolete.AdvVer1.AddFileTag("OSD-FIRST-LINE", (height - 3).ToString());
            AdvLib.Obsolete.AdvVer1.AddFileTag("OSD-LAST-LINE", (height - 1).ToString());

            AdvLib.Obsolete.AdvVer1.AddFileTag("FRAME-STACKING-RATE", integrationRate.ToString());

            float effectiveIntegrationRate = 25.0f / integrationRate;
            AdvLib.Obsolete.AdvVer1.AddFileTag("EFFECTIVE-FRAME-RATE", effectiveIntegrationRate.ToString("0.00000"));

            AdvLib.Obsolete.AdvVer1.AddFileTag("NATIVE-FRAME-RATE", "25.00");
            AdvLib.Obsolete.AdvVer1.AddFileTag("NATIVE-VIDEO-STANDARD", "PAL");

            AdvLib.Obsolete.AdvVer1.AddFileTag("CAPHNTP-TIMING-CORRECTION", "0");

            AdvLib.Obsolete.AdvVer1.AddFileTag("BITPIX", "16");
            AdvLib.Obsolete.AdvVer1.AddFileTag("AAV16-NORMVAL", (255 * integrationRate).ToString());

            AdvLib.Obsolete.AdvVer1.DefineImageSection((ushort)width, (ushort)height, 16);
            AdvLib.Obsolete.AdvVer1.AddOrUpdateImageSectionTag("IMAGE-BYTE-ORDER", "LITTLE-ENDIAN");
            AdvLib.Obsolete.AdvVer1.DefineImageLayout(1, "FULL-IMAGE-RAW", "UNCOMPRESSED", 16, 0, null);
            AdvLib.Obsolete.AdvVer1.DefineImageLayout(2, "FULL-IMAGE-DIFFERENTIAL-CODING-NOSIGNS", "LAGARITH16", 16, 32, "PREV-FRAME");
            AdvLib.Obsolete.AdvVer1.DefineImageLayout(3, "FULL-IMAGE-DIFFERENTIAL-CODING", "LAGARITH16", 16, 32, "PREV-FRAME");
            AdvLib.Obsolete.AdvVer1.DefineImageLayout(4, "FULL-IMAGE-RAW", "LAGARITH16", 16, 0, null);

            STATUS_TAG_SYSTEM_TIME = AdvLib.Obsolete.AdvVer1.DefineStatusSectionTag("SystemTime", AdvTagType.ULong64);
            STATUS_TAG_NUMBER_INTEGRATED_FRAMES = AdvLib.Obsolete.AdvVer1.DefineStatusSectionTag("IntegratedFrames", AdvTagType.UInt16);
            STATUS_TAG_START_FRAME_ID = AdvLib.Obsolete.AdvVer1.DefineStatusSectionTag("StartFrame", AdvTagType.ULong64);
            STATUS_TAG_END_FRAME_ID = AdvLib.Obsolete.AdvVer1.DefineStatusSectionTag("EndFrame", AdvTagType.ULong64);
            STATUS_TAG_EXPOSURE = AdvLib.Obsolete.AdvVer1.DefineStatusSectionTag("CameraExposure", AdvTagType.AnsiString255);

            STATUS_TAG_NTP_START_TIMESTAMP = AdvLib.Obsolete.AdvVer1.DefineStatusSectionTag("NTPStartTimestamp", AdvTagType.ULong64);
            STATUS_TAG_NTP_END_TIMESTAMP = AdvLib.Obsolete.AdvVer1.DefineStatusSectionTag("NTPEndTimestamp", AdvTagType.ULong64);
            STATUS_TAG_NTP_TIME_ERROR = AdvLib.Obsolete.AdvVer1.DefineStatusSectionTag("NTPTimestampError", AdvTagType.UInt16);
        }

        public static void AddVideoFrame(DateTime startTimeStamp, DateTime endTimeStamp, Pixelmap pixmap)
        {
            long timeStamp = WindowsTicksToAavTicks((startTimeStamp.Ticks + endTimeStamp.Ticks) / 2);
            uint exposureIn10thMilliseconds = (uint)(endTimeStamp.Ticks - startTimeStamp.Ticks) / 1000;

            if (s_FirstRecordedFrameTimestamp == 0)
                s_FirstRecordedFrameTimestamp = timeStamp;

            // since the first recorded frame was taken
            uint elapsedTimeMilliseconds = (uint)((timeStamp - s_FirstRecordedFrameTimestamp) * 0xFFFFFFFF);

            AdvLib.Obsolete.AdvVer1.BeginFrame(timeStamp, elapsedTimeMilliseconds, exposureIn10thMilliseconds);

            AdvLib.Obsolete.AdvVer1.FrameAddStatusTag16(STATUS_TAG_NUMBER_INTEGRATED_FRAMES, (ushort)s_IntegrationRate);

            ulong currentSystemTime = (ulong)WindowsTicksToAavTicks(DateTime.Now.Ticks);
            AdvLib.Obsolete.AdvVer1.FrameAddStatusTag64(STATUS_TAG_SYSTEM_TIME, currentSystemTime);

            AdvLib.Obsolete.AdvVer1.FrameAddStatusTag64(STATUS_TAG_NTP_START_TIMESTAMP, (ulong)WindowsTicksToAavTicks(startTimeStamp.Ticks));
            AdvLib.Obsolete.AdvVer1.FrameAddStatusTag64(STATUS_TAG_NTP_END_TIMESTAMP, (ulong)WindowsTicksToAavTicks(endTimeStamp.Ticks));
            AdvLib.Obsolete.AdvVer1.FrameAddStatusTag16(STATUS_TAG_NTP_TIME_ERROR, 320);

            AdvLib.Obsolete.AdvVer1.FrameAddStatusTag(STATUS_TAG_EXPOSURE, string.Format("x{0}", s_IntegrationRate * 2));

            ushort[] pixels = new ushort[pixmap.Pixels.Length];
            for (int i = 0; i < pixmap.Pixels.Length; i++) pixels[i] = (ushort)pixmap.Pixels[i];
            AdvLib.Obsolete.AdvVer1.FrameAddImage(1, pixels, 16);

            AdvLib.Obsolete.AdvVer1.EndFrame();
        }

        private static long ADV_EPOCH_ZERO_TICKS = 633979008000000000;

        private static long WindowsTicksToAavTicks(long windowsTicks)
        {
	        if (windowsTicks > 0)
	        {
		        long advTicks = (long)(windowsTicks - ADV_EPOCH_ZERO_TICKS) / 10000;

		        return advTicks;
	        }
	        else
		        return 0;
        }
    }
}
