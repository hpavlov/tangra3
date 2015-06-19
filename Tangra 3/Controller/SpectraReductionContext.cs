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
        public PixelCombineMethod BackgroundMethod { get; set; }
        public PixelCombineMethod FrameCombineMethod { get; set; }
        public bool UseFineAdjustments { get; set; }
        public bool UseLowPassFilter { get; set; }

        public void Reset()
        {
            
        }
    }
}
