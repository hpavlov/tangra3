using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.Model.Video
{
    public enum VideoFileFormat
    {
        [VideoFormatType(VideoFormatType.Analogue)]
        Unknown = 0,

        [VideoFormatType(VideoFormatType.Digital)]
        ADV,

        [VideoFormatType(VideoFormatType.Analogue)]
        AAV,

        [VideoFormatType(VideoFormatType.Analogue)]
        AVI,

        [VideoFormatType(VideoFormatType.Digital)]
        SER,

        [VideoFormatType(VideoFormatType.Digital)]
        FITS,

        [VideoFormatType(VideoFormatType.Analogue)]
        AAV2,

        [VideoFormatType(VideoFormatType.Digital)]
        ADV2
    }
}
