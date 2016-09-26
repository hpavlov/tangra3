using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.Model.Astro;

namespace Tangra.Astrometry
{
    public class Rect
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;
    }

    public class PlateSolveTesterConfig
    {
        public double RADeg;
        public double DEDeg;
        public double ErrFoVs;
        public float Epoch;
        public double PyramidMinMag;
        public double PyramidMaxMag;
        public double LimitMagn;
        public bool DetermineAutoLimitMagnitude;
        public AstroPlate PlateConfig;
        public Rect OSDRectToExclude;
        public Rect RectToInclude;
        public bool LimitByInclusion;
        public int Width;
        public int Height;
        public int BitPix;
        public uint[] Pixels;
    }
}
