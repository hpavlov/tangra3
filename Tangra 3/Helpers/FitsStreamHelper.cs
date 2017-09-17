/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Tangra.Model.Context;
using Tangra.Model.Image;
using Tangra.PInvoke;
using nom.tam.fits;
using nom.tam.util;
using Tangra.Model.Helpers;
using Tangra.Video.FITS;

namespace Tangra.Helpers
{
	public static class FitsStreamHelper
	{
        internal static Pixelmap BuildFitsPixelmap(int width, int height, uint[] pixelsFlat, int bitPix, bool hasUtcTimeStamps, double? exposure, DateTime? timestamp, BasicHDU fitsImage, Dictionary<string, string> cards)
	    {
            byte[] displayBitmapBytes = new byte[width * height];
            byte[] rawBitmapBytes = new byte[(width * height * 3) + 40 + 14 + 1];

            uint[] flatPixelsCopy = new uint[pixelsFlat.Length];
            Array.Copy(pixelsFlat, flatPixelsCopy, pixelsFlat.Length);

            TangraCore.PreProcessors.ApplyPreProcessingPixelsOnly(pixelsFlat, flatPixelsCopy, width, height, bitPix, 0 /* No normal value for FITS files */, exposure.HasValue ? (float)exposure.Value : 0);

            TangraCore.GetBitmapPixels(width, height, flatPixelsCopy, rawBitmapBytes, displayBitmapBytes, true, (ushort)bitPix, 0);

            Bitmap displayBitmap = Pixelmap.ConstructBitmapFromBitmapPixels(displayBitmapBytes, width, height);

            Pixelmap rv = new Pixelmap(width, height, bitPix, flatPixelsCopy, displayBitmap, displayBitmapBytes);
            rv.UnprocessedPixels = pixelsFlat;
            if (hasUtcTimeStamps)
            {
                rv.FrameState.CentralExposureTime = timestamp.HasValue ? timestamp.Value : DateTime.MinValue;
                rv.FrameState.ExposureInMilliseconds = exposure.HasValue ? (float)(exposure.Value * 1000.0) : 0;
            }

            rv.FrameState.Tag = fitsImage;
            rv.FrameState.AdditionalProperties = new SafeDictionary<string, object>();
            foreach (string key in cards.Keys)
                rv.FrameState.AdditionalProperties.Add(key, cards[key]);

            return rv;
	        
	    }
	}
}
