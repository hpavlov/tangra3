using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.Model.Video
{
    public enum VideoFormatType
    {
        Analogue,
        Digital
    }

    public class VideoFormatTypeAttribute : Attribute
    {
        public VideoFormatTypeAttribute(VideoFormatType videoFormatType)
        {
            VideoFormatType = videoFormatType;
        }

        public VideoFormatType VideoFormatType { get; private set; }
    }
}
