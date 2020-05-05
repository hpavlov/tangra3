using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tangra.Model.Video;

namespace Tangra.VideoOperations.LightCurves.InfoForms
{
    public enum AnalogueVideoFormat
    {
        Unknown,
        PAL,
        NTSC
    }

    public enum RawTimestampSource
    {
        [Description("Uknown")]
        Unknown,

        [Description("Derived by user entered start/end timestamp")]
        UserEntered,

        [Description("OCR-ed from VTI OSD")]
        OCR,

        [Description("NTP Server")]
        NTP,

        [Description("Saved with video frame")]
        Embedded
    }

    public class TimeCorrectonsInfo
    {
        public TimeCorrectonsInfo(VideoFormatType videoFormatType, VideoFileFormat videoFileType, MeasurementTimingType measurementTimingType)
        {
            VideoFormatType = videoFormatType;
            VideoFileType = videoFileType;
            switch (measurementTimingType)
            {
                case MeasurementTimingType.UserEnteredFrameReferences:
                    RawTimestampSource = RawTimestampSource.UserEntered;
                    break;

                case MeasurementTimingType.OCRedTimeForEachFrame:
                    RawTimestampSource = RawTimestampSource.OCR;
                    break;

                case MeasurementTimingType.EmbeddedTimeForEachFrame:
                    RawTimestampSource = RawTimestampSource.Embedded;
                    break;
            }
            
        }

        public VideoFormatType VideoFormatType { get; private set; }

        public VideoFileFormat VideoFileType { get; private set; }

        public string Message { get; set; }

        public string CameraName { get; set; }

        public int? IntegratedFrames { get; set; }

        public DateTime MidFrameTimestamp { get; set; }

        public DateTime? RawFrameTimeStamp { get; set; }

        public double? InstrumentalDelaySec { get; set; }

        public AnalogueVideoFormat AnalogueVideoFormat { get; set; }

        public RawTimestampSource RawTimestampSource { get; set; }

        public bool NotAffectedByAcquisitionDelays { get; set; }

        public double? AcquisitionDelaySec { get; set; }

        public double? ReferenceTimeOffsetSec { get; set; }

        public DateTime FinalTimestamp { get; set; }
    }
}
