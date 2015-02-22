/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.StarCatalogues
{
    // http://www.aerith.net/astro/color_conversion.html
    // Conversion Formulas 
    // General 
    //R = V - 0.508 * (B - V) - 0.040   (0.3 < B - V < 0.9) 

    //Johnson-Cousins BVRI photometry for faint field stars
    //ftp://ftp.lowell.edu/pub/bas/starcats/loneos.phot 
    //(B - I) = 2.36 * (B - V) 

    //The use of the (B-I) color index and applications of the (B-I) versus (B-V) relationship
    //Natali F., Natali G., Pompei E., Pedichini F., 1994, A&A 289, 756
    //http://ads.nao.ac.jp/cgi-bin/nph-bib_query?bibcode=1994A%26A...289..756N&db_key=AST&high=3d1846678a28870 
    // Tycho-2 Catalogue 
    //V = VT - 0.090 * (BT - VT)
    //B - V = 0.850 * (BT - VT) 

    //The Tycho-2 Catalogue
    //http://adc.gsfc.nasa.gov/cgi-bin/adc/cat.pl?/catalogs/1/1259/ 
    //V = VT + 0.00097 - 0.1334 * (BT - VT) + 0.05486 * (BT - VT)^2 - 0.01998 * (BT - VT)^3   (-0.25 < BT - VT < 2.0)
    //B - V = (BT - VT) - 0.007813 * (BT - VT) - 0.1489 * (BT - VT)^2 + 0.03384 * (BT - VT)^3   (0.5 < BT - VT < 2.0)
    //B - V = (BT - VT) - 0.006 - 0.1069 * (BT - VT) + 0.1459 * (BT - VT)^2   (-0.25 < BT - VT < 0.5) 

    //Post-T Tauri Stars in the Nearest OB Association
    //Mamajek, Eric E.; Meyer, Michael R.; Liebert, James
    //Astronomical Journal, Volume 124, Issue 3, pp. 1670-1694
    //2002AJ....124.1670M 
    //* Note that +0.007813 in this paper is typo, -0.007813 is correct.

    //Erratum:
    //Mamajek, Meyer, Liebert, 2006, AJ, 131, 2360 
    // USNO-A1.0 
    //V = r + 0.375 * (b - r)
    //B - V = 0.625 * (b - r) 

    //[vsnet-chat 700] Re: Comparison sequences
    //Taichi Kato
    //http://vsnet.kusastro.kyoto-u.ac.jp/vsnet/Mail/vsnet-chat/msg00700.html 
    // USNO-A2.0 
    //B = 1.097 * USNO(B) - 1.216   (12 < USNO(B) < 19)
    //V = 1.064 * USNO(V) - 0.822   (12 < USNO(V) < 19)
    //R = 1.031 * USNO(R) - 0.417   (12 < USNO(R) < 19) 

    //The USNO A2.0 photometric scale compared to the standard Landolt BVR magnitudes
    //http://www.iac.es/galeria/mrk/comets/USNO_Landolt.htm 
    // USNO-B1.0 
    //B - V = 0.556 * (B1 - R1) 

    //Crude USNO B1.0 B1-R1 to B-V transformation
    //John Greaves
    //http://www.aerith.net/astro/color_conversion/JG/USNO-B1.0.html 
    // 2MASS 
    //V - Rc = 0.8625 * (J - Ks) + 0.0202   (0.1 < J - Ks < 0.8)
    //V - Ic = 1.6069 * (J - Ks) + 0.0503   (0.1 < J - Ks < 0.8) 

    //2MASS J and K Photometry Conversion to V-Rc and V-Ic
    //Doug West
    //http://members.aol.com/dwest61506/page72.html 
    // Sloan Digital Sky Survey 
    //V = r + 0.44 * (g - r) - 0.02
    //B = V + 1.04 * (g - r) + 0.19 

    //The Sloan Digital Sky Survey Moving Object Catalog
    //http://www.astro.princeton.edu/~ivezic/sdssmoc/sdssmoc.html 
    //The Sloan Digital Sky Survey Photometric System
    //Fukugita, M.; Ichikawa, T.; Gunn, J. E.; Doi, M.; Shimasaku, K.; Schneider, D. P.
    //Astronomical Journal v.111, p.1748
    //1996AJ....111.1748F 
    // Carlsberg Meridian Catalogue, Vol. 13 (CMC13) 
    //V = 0.6 * (J - Ks) + r'_CMT - 0.03   (-0.2 < J - Ks < 1.2, 9 < r'_CMT < 14)
    //Rc = r'_CMT - 0.2   (9 < r'_CMT < 14) 

    //[vsnet-chat 6758] (fwd) Johnson V estimation from r'_CMT and 2MASS J-Ks
    //John Greaves
    //http://vsnet.kusastro.kyoto-u.ac.jp/vsnet/Mail/vsnet-chat/msg06758.html 
    //[vsnet-chat 6762] (fwd) Re: Johnson V estimation from r'_CMT and 2MASS J-Ks
    //John Greaves
    //http://vsnet.kusastro.kyoto-u.ac.jp/vsnet/Mail/vsnet-chat/msg06762.html 
    // Carlsberg Meridian Catalogue, Vol. 14 (CMC14) 
    //V = 0.6278 * (J - K) + 0.9947 * r'   (9 < r' < 16) 

    //A method for determining the V magnitude of asteroids from CCD images
    //Roger Dymock & Richard Miles
    //J.Br.Astron.Assoc. 119,3,2009 149-156
    //http://www.britastro.org/asteroids/JBAA%20119%20149-156%20Dymock1.pdf 
    //V = 0.6 * (J - Ks) + r'_CMT - 0.03 +/- 0.06   (0 < J - Ks < 1, 9 < r'_CMT < 15) 

    //[vsnet-alert 9907] re DA55 aka QSO B0133 +47
    //John Greaves 
    //* Note that the formula is corrected in [vsnet-alert 9908].

    //Rc = r' - 0.21 +/- 0.13   (9 < r' < 15.5, 0 < V - Rc < +0.6)
    //Rc = 0.984 r' +/- 0.13 (9 < r' < 15.5, 0 < V - Rc < +0.6)


    //Red Magnitudes
    //John Greaves
    //http://www.aerith.net/astro/color_conversion/JG/redmags.pdf 
    // Northern Sky Variability Survey (NSVS) = ROTSE-1 
    //mag = Vt - (Bt - Vt) / 1.875 

    //ROTSE All-Sky Surveys for Variable Stars. I. Test Fields
    //Akerlof, C. et al.
    //The Astronomical Journal, Volume 119, Issue 4, pp. 1901-1913.
    //http://ads.nao.ac.jp/cgi-bin/nph-bib_query?bibcode=2000AJ....119.1901A&db_key=AST&high=417a7ffc7113205 
    //V = 0.9425 * ROTSE1 + 0.6914   (+/- 0.2)
    //V - ROTSE1 = 0.8360 * (J - Ks) - 0.4576   (0.0 < J - Ks < 1.2, +/- 0.07) 

    //* Note that ROTSE-1 magnitudes may have a systematic offset of up to +/- 0.1 depending on what part of the sky. 
    //John Greaves
    //personnal communication 

    // Unfiltered CCD Instrumental Mag. 
    //(V - inst) = 0.2600 * (B - V)   (SITe back-illuminated)
    //(V - inst) = 0.3602 * (B - V)   (KAF-E)
    //(V - inst) = 0.5174 * (B - V)   (KAF)
    //(V - inst) = -0.0606 * (B - V)   (Sony)
    //(V - inst) = 0.4292 * (B - V)   (TI TC245)
    //(V - inst) = 0.2905 * (B - V)   (TI TC241 (VP))


    //(R - inst) = -0.2716 * (V - I)   (SITe back-illuminated)
    //(R - inst) = -0.1541 * (V - I)   (KAF-E)
    //(R - inst) = 0.0005 * (V - I)   (KAF)
    //(R - inst) = -0.6256 * (V - I)   (Sony)
    //(R - inst) = -0.1021 * (V - I)   (TI TC245)
    //(R - inst) = -0.2510 * (V - I)   (TI TC241 (VP))


    //The M67 Unfiltered Photometry Experiment
    //2000, Journal AAVSO vol. 29, page 35.
    //Arne A. Henden
    //ftp://ftp.nofs.navy.mil/pub/outgoing/aah/m67/paper/ 
    // Links 
    // Conversion between color indices 
    //Statistical Relations between the Photometric Colours of Common Types of Stars in the UBV(RI)c, JHK and uvby Systems
    //Caldwell J.A.R., Cousins A.W.J., Ahlers C.C., van Wamelen P., Maritz E.J., South African Astron. Obs. Circ., 15, 1-29 (1993)
    //http://ads.nao.ac.jp/cgi-bin/nph-bib_query?bibcode=1993SAAOC..15....1C&db_key=AST&high=3c73311ce511014 
    // Conversion between color index and spectral type. 
    //Intrinsic b-y, H-beta (for B stars), and V-I colors, and absolute magnitudes
    //ftp://ftp.lowell.edu/pub/bas/starcats/uvby.calib 
    //Intrinsic colours as a function of spectral type
    //http://www-int.stsci.edu/~inr/intrins.html 
    //A Stellar Spectral Flux Library, 1150-25000 A
    //Pickles A.J., PASP, 110, 863 (1998)
    //http://webast.ast.obs-mip.fr/stelib/literature/Pickles_tbl2.txt
    //http://webast.ast.obs-mip.fr/stelib/literature/Pickles_1998.ps

    // Conversion between color index and absolute magnitude. 
    //Photometric properties of low-mass stars and brown dwarfs
    //http://www-int.stsci.edu/~inr/cmd.html 
    // Conversion between 2MASS magnitude system and other infrared systems. 
    //2MASS Color Transformations
    //http://www.astro.caltech.edu/~jmc/2mass/v3/transformations/ 

    public struct ColourIndexEntry
    {
        public string SP;
        public double VR;
        public double JK;
    }
    public class ColourIndexTables
    {
        public static List<ColourIndexEntry> VR_JK_Table = new List<ColourIndexEntry>();

        public static List<ColourIndexEntry> JohnsonCousinsTable = new List<ColourIndexEntry>();

        static ColourIndexTables()
        {
            #region V-R => J - K
            // http://www-int.stsci.edu/~inr/intrins.html
            VR_JK_Table.Add(new ColourIndexEntry() {SP = "A1.0", VR = -0.010, JK = -0.020 + -0.030});            
            VR_JK_Table.Add(new ColourIndexEntry() {SP = "A3.0", VR =  0.050, JK = -0.010 +  0.040});
            VR_JK_Table.Add(new ColourIndexEntry() {SP = "A7.0", VR =  0.140, JK =  0.080 +  0.050});
            VR_JK_Table.Add(new ColourIndexEntry() {SP = "F5.0", VR =  0.250, JK =  0.180 +  0.060});
            VR_JK_Table.Add(new ColourIndexEntry() {SP = "F6.0", VR =  0.270, JK =  0.200 +  0.080});
            VR_JK_Table.Add(new ColourIndexEntry() {SP = "F8.0", VR =  0.330, JK =  0.290 +  0.090});
            VR_JK_Table.Add(new ColourIndexEntry() {SP = "F9.0", VR =  0.340, JK =  0.300 +  0.090});
            VR_JK_Table.Add(new ColourIndexEntry() {SP = "G0.0", VR =  0.335, JK =  0.835 +  0.060});
            VR_JK_Table.Add(new ColourIndexEntry() {SP = "G3.0", VR =  0.370, JK =  0.330 +  0.060});
            VR_JK_Table.Add(new ColourIndexEntry() {SP = "G5.0", VR =  0.380, JK =  0.328 +  0.090});
            VR_JK_Table.Add(new ColourIndexEntry() {SP = "G6.0", VR =  0.380, JK =  0.340 +  0.090});
            VR_JK_Table.Add(new ColourIndexEntry() {SP = "G8.0", VR =  0.422, JK =  0.328 +  0.120});
            VR_JK_Table.Add(new ColourIndexEntry() {SP = "G9.0", VR =  0.420, JK =  0.370 +  0.160});
            VR_JK_Table.Add(new ColourIndexEntry() {SP = "K0.0", VR =  0.468, JK =  0.440 +  0.133});
            VR_JK_Table.Add(new ColourIndexEntry() {SP = "K1.0", VR =  0.432, JK =  0.396 +  0.142});
            VR_JK_Table.Add(new ColourIndexEntry() {SP = "K2.0", VR =  0.590, JK =  0.522 +  0.108});
            VR_JK_Table.Add(new ColourIndexEntry() {SP = "K3.0", VR =  0.565, JK =  0.513 +  0.111});
            VR_JK_Table.Add(new ColourIndexEntry() {SP = "K3.5", VR =  0.665, JK =  0.540 +  0.140});
            VR_JK_Table.Add(new ColourIndexEntry() {SP = "K4.0", VR =  0.722, JK =  0.582 +  0.184});
            VR_JK_Table.Add(new ColourIndexEntry() {SP = "K4.5", VR =  0.750, JK =  0.610 +  0.140});
            VR_JK_Table.Add(new ColourIndexEntry() {SP = "K5.0", VR =  0.747, JK =  0.629 +  0.131});
            VR_JK_Table.Add(new ColourIndexEntry() {SP = "K7.0", VR =  0.860, JK =  0.615 +  0.189});
            VR_JK_Table.Add(new ColourIndexEntry() {SP = "M0.0", VR =  0.910, JK =  0.638 +  0.199});
            VR_JK_Table.Add(new ColourIndexEntry() {SP = "M0.5", VR =  0.939, JK =  0.630 +  0.194});
            VR_JK_Table.Add(new ColourIndexEntry() {SP = "M1.0", VR =  0.934, JK =  0.587 +  0.223});
            VR_JK_Table.Add(new ColourIndexEntry() {SP = "M1.5", VR =  0.968, JK =  0.584 +  0.220});
            VR_JK_Table.Add(new ColourIndexEntry() {SP = "M2.0", VR =  1.038, JK =  0.583 +  0.248});
            VR_JK_Table.Add(new ColourIndexEntry() {SP = "M2.5", VR =  1.044, JK =  0.561 +  0.255});
            VR_JK_Table.Add(new ColourIndexEntry() {SP = "M3.0", VR =  1.083, JK =  0.584 +  0.251});
            VR_JK_Table.Add(new ColourIndexEntry() {SP = "M3.5", VR =  1.157, JK =  0.564 +  0.256});
            VR_JK_Table.Add(new ColourIndexEntry() {SP = "M4.0", VR =  1.252, JK =  0.593 +  0.234});
            VR_JK_Table.Add(new ColourIndexEntry() {SP = "M4.5", VR =  1.336, JK =  0.521 +  0.303});
            VR_JK_Table.Add(new ColourIndexEntry() {SP = "M5.0", VR =  1.515, JK =  0.575 +  0.315});
            VR_JK_Table.Add(new ColourIndexEntry() {SP = "M5.5", VR =  1.558, JK =  0.580 +  0.317});
            VR_JK_Table.Add(new ColourIndexEntry() {SP = "M6.0", VR =  1.770, JK =  0.570 +  0.325});
            VR_JK_Table.Add(new ColourIndexEntry() {SP = "M6.5", VR =  2.110, JK =  0.587 +  0.373});
            VR_JK_Table.Add(new ColourIndexEntry() {SP = "M7.0", VR =  2.150, JK =  0.580 +  0.370});
            VR_JK_Table.Add(new ColourIndexEntry() {SP = "M7.5", VR =  2.290, JK =  0.500 +  0.380});
            VR_JK_Table.Add(new ColourIndexEntry() {SP = "M8.0", VR =  2.207, JK =  0.680 +  0.443});
            VR_JK_Table.Add(new ColourIndexEntry() {SP = "M8.5", VR =  2.295, JK =  0.685 +  0.520});
            VR_JK_Table.Add(new ColourIndexEntry() {SP = "M9.5", VR =  2.310, JK =  0.815 +  0.555});
            VR_JK_Table.Add(new ColourIndexEntry() {SP = "L0.0", VR =  2.410, JK =  0.760 +  0.540});
            VR_JK_Table.Add(new ColourIndexEntry() {SP = "L0.5", VR =  2.380, JK =  0.760 +  0.480});
            VR_JK_Table.Add(new ColourIndexEntry() {SP = "L1.0", VR =  2.520, JK =  0.740 +  0.545});
            VR_JK_Table.Add(new ColourIndexEntry() {SP = "L2.0", VR =  2.670, JK =  0.950 +  0.670});
            VR_JK_Table.Add(new ColourIndexEntry() {SP = "L3.0", VR =  2.510, JK =  0.910 +  0.620});
            VR_JK_Table.Add(new ColourIndexEntry() {SP = "L4.5", VR =  2.990, JK =  0.905 +  0.630});
            VR_JK_Table.Add(new ColourIndexEntry() {SP = "L5.0", VR =  3.860, JK =  0.997 +  0.627});
            VR_JK_Table.Sort((x, y) => x.VR.CompareTo(y.VR));
            #endregion

//      U-B  (B-V) (V-R)C(V-I)C (V-J) (V-H) (V-K) (V-L) (V-M) (V-N) (V-R)J(V-I)J  
//B0.0 -1.08 -0.30 -0.19 -0.31 -0.80 -0.92 -0.97 -1.13 -1.00 -9.99 -0.22 -0.44 
//B0.5 -1.00 -0.28 -0.18 -0.31 -0.77 -0.89 -0.95 -1.11 -0.99 -9.99 -0.20 -0.43 
//B1.0 -0.95 -0.26 -0.16 -0.30 -0.73 -0.85 -0.93 -1.08 -0.96 -9.99 -0.18 -0.42 
//B1.5 -0.88 -0.25 -0.15 -0.29 -0.70 -0.82 -0.91 -1.05 -0.94 -9.99 -0.17 -0.41 
//B2.0 -0.81 -0.24 -0.14 -0.29 -0.67 -0.79 -0.89 -1.02 -0.92 -9.99 -0.15 -0.40 
//B2.5 -0.72 -0.22 -0.13 -0.28 -0.64 -0.76 -0.86 -0.97 -0.88 -0.96 -0.14 -0.39 
//B3.0 -0.68 -0.20 -0.12 -0.27 -0.60 -0.72 -0.82 -0.92 -0.84 -0.91 -0.13 -0.38 
//B3.5 -0.65 -0.19 -0.12 -0.26 -0.58 -0.70 -0.80 -0.90 -0.82 -0.87 -0.12 -0.37 
//B4.0 -0.63 -0.18 -0.11 -0.25 -0.56 -0.68 -0.77 -0.86 -0.79 -0.84 -0.11 -0.35 
//B4.5 -0.61 -0.17 -0.11 -0.24 -0.54 -0.65 -0.74 -0.83 -0.76 -0.80 -0.11 -0.34 
//B5.0 -0.58 -0.16 -0.10 -0.24 -0.51 -0.62 -0.71 -0.78 -0.73 -0.75 -0.10 -0.33 
//B6.0 -0.49 -0.14 -0.10 -0.21 -0.46 -0.57 -0.64 -0.70 -0.65 -0.66 -0.09 -0.29 
//B7.0 -0.43 -0.13 -0.09 -0.19 -0.41 -0.51 -0.57 -0.61 -0.58 -0.58 -0.08 -0.26 
//B7.5 -0.40 -0.12 -0.09 -0.17 -0.39 -0.48 -0.54 -0.57 -0.54 -0.53 -0.08 -0.24 
//B8.0 -0.36 -0.11 -0.08 -0.16 -0.36 -0.45 -0.49 -0.52 -0.49 -0.48 -0.07 -0.22 
//B8.5 -0.27 -0.09 -0.08 -0.13 -0.31 -0.40 -0.43 -0.43 -0.42 -0.39 -0.07 -0.18 
//B9.0 -0.18 -0.07 -0.07 -0.10 -0.26 -0.34 -0.33 -0.34 -0.34 -0.30 -0.06 -0.14 
//B9.5 -0.10 -0.04 -0.05 -0.08 -0.22 -0.29 -0.26 -0.27 -0.26 -0.22 -0.03 -0.11 
//A0.0 -0.02 -0.01 -0.04 -0.04 -0.16 -0.19 -0.17 -0.18 -0.18 -0.14 -0.01 -0.05 
//A1.0  0.01  0.02 -0.02 -0.02 -0.11 -0.12 -0.11 -0.12 -0.13 -0.08  0.01 -0.03 
//A2.0  0.05  0.05 -0.01  0.00 -0.07 -0.04 -0.05 -0.07 -0.08 -0.02  0.03  0.00 
//A3.0  0.08  0.08  0.01  0.02 -0.02  0.03  0.01 -0.01 -0.02  0.03  0.05  0.02 
//A4.0  0.09  0.12  0.02  0.05  0.03  0.11  0.08  0.05 -0.04  0.09  0.07  0.07 
//A5.0  0.09  0.15  0.04  0.09  0.09  0.19  0.15  0.12  0.10  0.16  0.10  0.12 
//A6.0  0.10  0.17  0.05  0.12  0.13  0.30  0.21  0.17  0.15  0.21  0.11  0.16 
//A7.0  0.10  0.20  0.06  0.15  0.18  0.32  0.27  0.23  0.20  0.26  0.13  0.19 
//A8.0  0.09  0.27  0.09  0.20  0.25  0.42  0.36  0.33  0.29  0.34  0.16  0.26 
//A9.0  0.08  0.30  0.10  0.24  0.31  0.49  0.44  0.41  0.36  0.41  0.18  0.31 
//F0.0  0.03  0.32  0.12  0.28  0.37  0.57  0.52  0.49  0.43  0.48  0.21  0.36 
//F1.0  0.00  0.34  0.14  0.31  0.43  0.64  0.58  0.57  0.49  0.54  0.23  0.40 
//F2.0  0.00  0.35  0.15  0.35  0.48  0.71  0.66  0.66  0.56  0.60  0.25  0.45 
//F5.0 -0.02  0.45  0.21  0.44  0.67  0.93  0.89  0.90  0.77  0.80  0.33  0.57 
//F8.0  0.02  0.53  0.24  0.50  0.79  1.06  1.03  1.06  0.91  0.91  0.37  0.64 
//G0.0  0.06  0.60  0.27  0.54  0.87  1.15  1.14  1.18  1.01  1.01  0.41  0.70 
//G2.0  0.09  0.63  0.30  0.58  0.97  1.25  1.26  1.31  1.12  1.11  0.45  0.75 
//G3.0  0.12  0.65  0.30  0.59  0.98  1.27  1.28  1.33  1.14  1.13  0.45  0.76 
//G5.0  0.20  0.68  0.31  0.61  1.02  1.31  1.32  1.38  1.18  1.17  0.47  0.78 
//G8.0  0.30  0.74  0.35  0.66  1.14  1.44  1.47  1.55  1.34  1.30  0.52  0.85 
//K0.0  0.44  0.81  0.42  0.75  1.34  1.67  1.74  1.85  1.61  1.54  0.61  0.97 
//K1.0  0.48  0.86  0.46  0.82  1.46  1.80  1.89  2.02  1.78  1.68  0.67  1.05 
//K2.0  0.67  0.92  0.50  0.89  1.60  1.94  2.06  2.21  1.97  1.84  0.73  1.14 
//K3.0  0.73  0.95  0.55  0.97  1.73  2.09  2.23  2.40  2.17  2.01  0.80  1.25 
//K4.0  1.00  1.00  0.60  1.04  1.84  2.22  2.38  2.57  2.36  2.15  0.86  1.34 
//K5.0  1.06  1.15  0.68  1.20  2.04  2.46  2.66  2.87  2.71  2.44  0.97  1.54 
//K7.0  1.21  1.33  0.62  1.45  2.30  2.78  3.01  3.25  3.21  2.83  1.13  1.86 
//M0.0  1.23  1.37  0.70  1.67  2.49  3.04  3.29  3.54  3.65  3.16  1.26  2.15 
//M1.0  1.18  1.47  0.76  1.84  2.61  3.22  3.47  3.72  3.95  3.39  1.36  2.36 
//M2.0  1.15  1.47  0.83  2.06  2.74  3.42  3.67  3.92  4.31  3.66  1.46  2.62 
//M3.0  1.17  1.50  0.89  2.24  2.84  3.58  3.83  4.08  4.62  3.89  1.56  2.84 
//M4.0  1.07  1.52  0.94  2.43  2.93  3.74  3.98  4.22  4.93  4.11  1.65  3.07 

        }

        public static double GetJKFromVR(double vr)
        {
            // V - Rc = 0.8625 * (J - Ks) + 0.0202   (0.1 < J - Ks < 0.8)
            return (vr - 0.0202)/0.8625;
        }

        public static double GetBVFromVR(double vr)
        {
            // R = V - 0.508 * (B - V) - 0.040   (0.3 < B - V < 0.9) 
            // V - R = 0.508 * (B - V) + 0.040

            return (vr - 0.040) / 0.508;
        }

        public static double GetJKFromVRTable(double vr)
        {
            // TODO: Write a few unit tests

            for (int i = 0; i < VR_JK_Table.Count - 1; i++)
            {
                ColourIndexEntry prev = VR_JK_Table[i];
                ColourIndexEntry next = VR_JK_Table[i + 1];

                if (prev.VR <= vr && next.VR >= vr)
                {
                    if (prev.VR - vr < 0.05) return prev.JK;
                    if (vr - next.VR < 0.05) return next.JK;

                    return prev.JK + (next.JK - prev.JK) * ((vr - prev.VR) / (next.VR - prev.VR));
                }
            }

            return double.NaN;
        }

        public static double GetVFromRAndVR(double r, double vr)
        {
            return vr + r;
        }

        public static double GetRFromVAndVR(double v, double vr)
        {
            return vr - v;
        }

        public static double GetVFromBAndVR(double r, double vr)
        {
            double bv = GetBVFromVR(vr);
            return bv + vr + r;
        }

        public static double GetRFromBAndVR(double v, double vr)
        {
            double bv = GetBVFromVR(vr);
            return bv + v;
        }


        // http://brucegary.net/dummies/method0.html
        // Warner and Harris (2007)
        //
        //B =  J + 0.198  + 5.215  * (J-K) - 2.7785 * (J-K)2 + 1.7495 * (J-K)3 
        //V =  J + 0.1496 + 3.5143 * (J-K) - 2.325  * (J-K)2 + 1.4688 * (J-K)3
        //Rc = J + 0.1045 + 2.5105 * (J-K) - 1.7849 * (J-K)2 + 1.123  * (J-K)3
        //Ic = J + 0.0724 + 1.2816 * (J-K) - 0.4866 * (J-K)2 + 0.2963 * (J-K)3
        //
        //You can expect SE = 0.08, 0.05, 0.04 and 0.035 provided -0.1 < (J-K)< 1.0.

        public static double GetBVfromJK(double jk)
        {
            return (0.198 - 0.1496) + (5.215 - 3.5143) * jk - (2.7785 - 2.325) * jk * jk + (1.7495 - 1.4688) * jk * jk * jk;
        }
    }
}
