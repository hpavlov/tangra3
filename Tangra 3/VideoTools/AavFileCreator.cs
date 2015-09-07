using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            AdvLib.AdvEndFile();
        }

        public static void StartNewFile(string fileName, int width, int height, int integrationRate)
        {
            AdvLib.AdvNewFile(fileName);

            s_FirstRecordedFrameTimestamp = 0;
            s_Width = width;
            s_Height = width;
            s_IntegrationRate = integrationRate;

            AdvLib.AdvAddFileTag("AAVR-SOFTWARE-VERSION", "1.0");
            AdvLib.AdvAddFileTag("RECORDER", "Tangra Video Model Simulator");
            AdvLib.AdvAddFileTag("FSTF-TYPE", "AAV");
            AdvLib.AdvAddFileTag("AAV-VERSION", "1");

            AdvLib.AdvAddFileTag("GRABBER", "");
            AdvLib.AdvAddFileTag("VIDEO-MODE", string.Format("{0}x{1}@25fps", width, height));
            AdvLib.AdvAddFileTag("CAMERA-MODEL", "");
            AdvLib.AdvAddFileTag("CAMERA-BITPIX", "8");

            AdvLib.AdvAddFileTag("FRAME-COMBINING", "Binning");

            AdvLib.AdvAddFileTag("OSD-FIRST-LINE", (height - 3).ToString());
            AdvLib.AdvAddFileTag("OSD-LAST-LINE", (height - 1).ToString());

            AdvLib.AdvAddFileTag("FRAME-STACKING-RATE", integrationRate.ToString());

            float effectiveIntegrationRate = 25.0f / integrationRate;
            AdvLib.AdvAddFileTag("EFFECTIVE-FRAME-RATE", effectiveIntegrationRate.ToString("0.00000"));

            AdvLib.AdvAddFileTag("NATIVE-FRAME-RATE", "25.00");
            AdvLib.AdvAddFileTag("NATIVE-VIDEO-STANDARD", "PAL");

            AdvLib.AdvAddFileTag("CAPHNTP-TIMING-CORRECTION", "0");

            AdvLib.AdvAddFileTag("BITPIX", "16");
            AdvLib.AdvAddFileTag("AAV16-NORMVAL", (255 * integrationRate).ToString());

            AdvLib.AdvDefineImageSection((ushort)width, (ushort)height, 16);
            AdvLib.AdvAddOrUpdateImageSectionTag("IMAGE-BYTE-ORDER", "LITTLE-ENDIAN");
            AdvLib.AdvDefineImageLayout(1, "FULL-IMAGE-RAW", "UNCOMPRESSED", 16, 0, null);
            AdvLib.AdvDefineImageLayout(2, "FULL-IMAGE-DIFFERENTIAL-CODING-NOSIGNS", "LAGARITH16", 16, 32, "PREV-FRAME");
            AdvLib.AdvDefineImageLayout(3, "FULL-IMAGE-DIFFERENTIAL-CODING",  "LAGARITH16", 16, 32, "PREV-FRAME");
            AdvLib.AdvDefineImageLayout(4, "FULL-IMAGE-RAW",  "LAGARITH16", 16,  0, null);

            STATUS_TAG_SYSTEM_TIME = AdvLib.AdvDefineStatusSectionTag("SystemTime", AdvTagType.ULong64);
            STATUS_TAG_NUMBER_INTEGRATED_FRAMES = AdvLib.AdvDefineStatusSectionTag("IntegratedFrames", AdvTagType.UInt16);
            STATUS_TAG_START_FRAME_ID = AdvLib.AdvDefineStatusSectionTag("StartFrame", AdvTagType.ULong64);
            STATUS_TAG_END_FRAME_ID = AdvLib.AdvDefineStatusSectionTag("EndFrame", AdvTagType.ULong64);
            STATUS_TAG_EXPOSURE = AdvLib.AdvDefineStatusSectionTag("CameraExposure", AdvTagType.AnsiString255);

            STATUS_TAG_NTP_START_TIMESTAMP = AdvLib.AdvDefineStatusSectionTag("NTPStartTimestamp", AdvTagType.ULong64);
            STATUS_TAG_NTP_END_TIMESTAMP = AdvLib.AdvDefineStatusSectionTag("NTPEndTimestamp", AdvTagType.ULong64);
            STATUS_TAG_NTP_TIME_ERROR = AdvLib.AdvDefineStatusSectionTag("NTPTimestampError", AdvTagType.UInt16);
        }

        public static void AddVideoFrame(DateTime startTimeStamp, DateTime endTimeStamp, Pixelmap pixmap)
        {
            long timeStamp = WindowsTicksToAavTicks((startTimeStamp.Ticks + endTimeStamp.Ticks) / 2);
            uint exposureIn10thMilliseconds = (uint)(endTimeStamp.Ticks - startTimeStamp.Ticks) / 1000;

            if (s_FirstRecordedFrameTimestamp == 0)
                s_FirstRecordedFrameTimestamp = timeStamp;

            // since the first recorded frame was taken
            uint elapsedTimeMilliseconds = (uint)((timeStamp - s_FirstRecordedFrameTimestamp) * 0xFFFFFFFF);

            AdvLib.AdvBeginFrame(timeStamp, elapsedTimeMilliseconds, exposureIn10thMilliseconds);

            AdvLib.AdvFrameAddStatusTag16(STATUS_TAG_NUMBER_INTEGRATED_FRAMES, (ushort)s_IntegrationRate);

            ulong currentSystemTime = (ulong)WindowsTicksToAavTicks(DateTime.Now.Ticks);
            AdvLib.AdvFrameAddStatusTag64(STATUS_TAG_SYSTEM_TIME, currentSystemTime);

            AdvLib.AdvFrameAddStatusTag64(STATUS_TAG_NTP_START_TIMESTAMP, (ulong)WindowsTicksToAavTicks(startTimeStamp.Ticks));
            AdvLib.AdvFrameAddStatusTag64(STATUS_TAG_NTP_END_TIMESTAMP, (ulong)WindowsTicksToAavTicks(endTimeStamp.Ticks));
            AdvLib.AdvFrameAddStatusTag16(STATUS_TAG_NTP_TIME_ERROR, 320);

            AdvLib.AdvFrameAddStatusTag(STATUS_TAG_EXPOSURE, string.Format("x{0}", s_IntegrationRate * 2));

            ushort[] pixels = new ushort[pixmap.Pixels.Length];
            for (int i = 0; i < pixmap.Pixels.Length; i++) pixels[i] = (ushort)pixmap.Pixels[i];
            AdvLib.AdvFrameAddImage(1, pixels, 16);

            AdvLib.AdvEndFrame();
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
