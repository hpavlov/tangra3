using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nom.tam.fits;

namespace Tangra.Video.FITS
{
    public interface IFITSTimeStampReader
    {
        DateTime? ParseExposure(string fileName, Header header, out bool isMidPoint, out double? fitsExposure);
    }
}
