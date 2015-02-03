using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.Video.FITS
{
    public interface IFITSStream
    {
        uint MinPixelValue { get; }
        uint MaxPixelValue { get; }
    }
}
