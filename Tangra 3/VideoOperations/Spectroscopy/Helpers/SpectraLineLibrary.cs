using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.VideoOperations.Spectroscopy.Helpers
{
    public class LineEntry
    {
        public bool IsWideArea;
        public float FromWavelength;
        public float ToWavelength;
        public string Element;
        public string Designation;
    }


    public static class SpectraLineLibrary
    {
        #region Wavelengths
        //http://en.wikipedia.org/wiki/Fraunhofer_lines

        //Designation Element Wavelength (nm)

        //y O2 898.765 
        //Z O2 822.696 
        //A O2 759.370 
        //B O2 686.719 
        //C Hα 656.281 
        //a O2 627.661 
        //D1 Na 589.592 
        //D2 Na 588.995 
        //D3 or d He 587.5618 
        //e Hg 546.073 
        //E2 Fe 527.039 
        //b1 Mg 518.362 
        //b2 Mg 517.270 
        //b3 Fe 516.891 
        //b4 Mg 516.733 


        //Designation Element Wavelength (nm)

        //c Fe 495.761 
        //F Hβ 486.134 
        //d Fe 466.814 
        //e Fe 438.355 
        //G' Hγ 434.047 
        //G Fe 430.790 
        //G Ca 430.774 
        //h Hδ 410.175 
        //H Ca+ 396.847 
        //K Ca+ 393.368 
        //L Fe 382.044 
        //N Fe 358.121 
        //P Ti+ 336.112 
        //T Fe 302.108 
        //t Ni 299.444 



        // http://www.astrosurf.com/buil/us/vatlas/vatlas.htm
        // http://www.astrosurf.com/buil/us/spe2/hresol4.htm
        #endregion

        private static object s_SyncLock = new object();

        public static List<LineEntry> CommonLines = new List<LineEntry>();

        static SpectraLineLibrary()
        {
            lock (s_SyncLock)
            {
				CommonLines.Add(new LineEntry() { Designation = "h", Element = "Hδ", FromWavelength = 4101.75f });
                CommonLines.Add(new LineEntry() { Designation = "G'", Element = "Hγ", FromWavelength = 4340.47f });
                CommonLines.Add(new LineEntry() { Designation = "F", Element = "Hβ", FromWavelength = 4861.34f });
				CommonLines.Add(new LineEntry() { Designation = "a, telluric", Element = "O²", FromWavelength = 6277.7f }); 
                CommonLines.Add(new LineEntry() { Designation = "C", Element = "Hα", FromWavelength = 6562.81f });
				CommonLines.Add(new LineEntry() { Designation = "B, telluric", Element = "O²", FromWavelength = 6869.0f });
				CommonLines.Add(new LineEntry() { Designation = "telluric", Element = "H₂O", FromWavelength = 7160f, ToWavelength = 7396f, IsWideArea = true });
                CommonLines.Add(new LineEntry() { Designation = "A", Element = "O²", FromWavelength = 7605.0f });
				CommonLines.Add(new LineEntry() { Designation = "telluric", Element = "H₂O", FromWavelength = 8100f, ToWavelength = 8400f, IsWideArea = true });
                CommonLines.Add(new LineEntry() { Designation = "telluric", Element = "H₂O", FromWavelength = 9200f, ToWavelength = 9700f, IsWideArea = true });
                CommonLines.Add(new LineEntry() { Element = "Paδ", FromWavelength = 10049.3f });
            }
        }
    }
}
