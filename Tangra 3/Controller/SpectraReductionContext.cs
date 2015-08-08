using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.VideoOperations.Spectroscopy.Helpers;

namespace Tangra.Controller
{
    internal class SpectraReductionContext
    {
        public int FramesToMeasure { get; set; }
        public int MeasurementAreaWing { get; set; }
        public int BackgroundAreaWing { get; set; }
        public int BackgroundAreaGap { get; set; }
        public PixelCombineMethod BackgroundMethod { get; set; }
        public PixelCombineMethod FrameCombineMethod { get; set; }
        public bool UseFineAdjustments { get; set; }
        public int? AlignmentAbsorptionLinePos { get; set; }
        public float PixelValueCoefficient { get; set; }
	    public float ExposureSeconds { get; set; }

        public void Reset()
        {
            
        }
    }
}
