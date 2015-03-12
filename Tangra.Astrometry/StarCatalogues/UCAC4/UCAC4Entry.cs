/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Tangra.StarCatalogues.UCAC4
{
    [StructLayout(LayoutKind.Explicit)]
    [Serializable]
    public class UCAC4Entry : IStar
    {
        public static Guid BAND_ID_UNFILTERED = new Guid("FA245D04-5BB9-4B81-BFF6-836CDBEFC86E");
        public static Guid BAND_ID_V = new Guid("377F9F25-EEEB-4A6F-83EC-6F6473B13253");
        public static Guid BAND_ID_B = new Guid("2F18CC1D-E73A-4F6A-9C00-5DD9135FAB2C");
        public static Guid BAND_ID_R = new Guid("4D0ACF4E-B30D-4CAC-8522-1D878B80A657");
        public static Guid BAND_ID_SLOAN_r = new Guid("FFE750C3-DB0F-4A63-B07F-76C0D81FFE9E");
        public static Guid BAND_ID_SLOAN_g = new Guid("743C264D-2DDF-4C0D-A470-1F82CEFC5990");
        public static Guid BAND_ID_SLOAN_i = new Guid("1AF9079A-338E-40DC-9CBA-F9FF274C76EB");
        public static Guid BAND_ID_J = new Guid("4DAD93F2-6AB7-46A2-A601-343A5BEAE9B2");
        public static Guid BAND_ID_K = new Guid("67AC6400-C780-4561-B683-2B7E664B066B");

        // col byte item   fmt unit       explanation                            notes
        // ---------------------------------------------------------------------------
        //  1  1- 3 ra     I*4 mas        right ascension at  epoch J2000.0 (ICRS) (1)
        //  2  5- 8 spd    I*4 mas        south pole distance epoch J2000.0 (ICRS) (1)
        //  3  9-10 magm   I*2 millimag   UCAC fit model magnitude                 (2)
        //  4 11-12 maga   I*2 millimag   UCAC aperture  magnitude                 (2)
        //  5 13    sigmag I*1 1/100 mag  error of UCAC magnitude                  (3)
        //  6 14    objt   I*1            object type                              (4)
        //  7 15    cdf    I*1            combined double star flag                (5)
        //          15
        //  8 16    sigra  I*1 mas        s.e. at central epoch in RA (*cos Dec)   (6)
        //  9 17    sigdc  I*1 mas        s.e. at central epoch in Dec             (6)
        // 10 18    na1    I*1            total # of CCD images of this star
        // 11 19    nu1    I*1            # of CCD images used for this star       (7)
        // 12 20    cu1    I*1            # catalogs (epochs) used for proper motions
        //           5
        // 13 21-22 cepra  I*2 0.01 yr    central epoch for mean RA, minus 1900
        // 14 23-24 cepdc  I*2 0.01 yr    central epoch for mean Dec,minus 1900
        // 15 25-26 pmrac  I*2 0.1 mas/yr proper motion in RA*cos(Dec)             (8)
        // 16 27-28 pmdc   I*2 0.1 mas/yr proper motion in Dec
        // 17 29    sigpmr I*1 0.1 mas/yr s.e. of pmRA * cos Dec                   (9)
        // 18 30    sigpmd I*1 0.1 mas/yr s.e. of pmDec                            (9)
        //          10
        // 19 31-34 pts_key I*4           2MASS unique star identifier            (10)
        // 20 35-36 j_m    I*2 millimag   2MASS J  magnitude
        // 21 37-38 h_m    I*2 millimag   2MASS H  magnitude
        // 22 39-40 k_m    I*2 millimag   2MASS K_s magnitude
        // 23 41    icqflg I*1            2MASS cc_flg*10 + ph_qual flag for J    (11)
        // 24 42     (2)   I*1            2MASS cc_flg*10 + ph_qual flag for H    (11)
        // 25 43     (3)   I*1            2MASS cc_flg*10 + ph_qual flag for K_s  (11)
        // 26 44    e2mpho I*1 1/100 mag  error 2MASS J   magnitude               (12)
        // 27 45     (2)   I*1 1/100 mag  error 2MASS H   magnitude               (12)
        // 28 46     (3)   I*1 1/100 mag  error 2MASS K_s magnitude               (12)
        //          16
        // 29 47-48 apasm  I*2 millimag   B magnitude from APASS                  (13)
        // 30 49-50  (2)   I*2 millimag   V magnitude from APASS                  (13)
        // 31 51-52  (3)   I*2 millimag   g magnitude from APASS                  (13)
        // 32 53-54  (4)   I*2 millimag   r magnitude from APASS                  (13)
        // 33 55-56  (5)   I*2 millimag   i magnitude from APASS                  (13)
        // 34 57    apase  I*1 1/100 mag  error of B magnitude from APASS         (14)
        // 35 58     (2)   I*1 1/100 mag  error of V magnitude from APASS         (14)
        // 36 59     (3)   I*1 1/100 mag  error of g magnitude from APASS         (14)
        // 37 60     (4)   I*1 1/100 mag  error of r magnitude from APASS         (14)
        // 38 61     (5)   I*1 1/100 mag  error of i magnitude from APASS         (14)
        // 39 62    gcflg  I*1            Yale SPM g-flag*10  c-flag              (15)
        //          16
        // 40 63-66 icf(1) I*4            FK6-Hipparcos-Tycho source flag         (16)
        // 41       icf(2) ..             AC2000       catalog match flag         (17)
        // 42       icf(3) ..             AGK2 Bonn    catalog match flag         (17)
        // 43       icf(4) ..             AKG2 Hamburg catalog match flag         (17)
        // 44       icf(5) ..             Zone Astrog. catalog match flag         (17)
        // 45       icf(6) ..             Black Birch  catalog match flag         (17)
        // 46       icf(7) ..             Lick Astrog. catalog match flag         (17)
        // 47       icf(8) ..             NPM  Lick    catalog match flag         (17)
        // 48       icf(9) ..             SPM  YSJ1    catalog match flag         (17)
        //           4
        // 49 67    leda   I*1            LEDA galaxy match flag                  (18)
        // 50 68    x2m    I*1            2MASS extend.source flag                (19)
        // 51 69-72 rnm    I*4            unique star identification number       (20)
        // 52 73-74 zn2    I*2            zone number of UCAC2 (0 = no match)     (21)
        // 53 75-78 rn2    I*4            running record number along UCAC2 zone  (21)
        //          12
        // ---------------------------------------------------------------------------
        //          78 = total number of bytes per star record

        //  I*4 mas        right ascension at  epoch J2000.0 (ICRS) (1)
        [FieldOffset(0)]
		internal int ra;

        //  I*4 mas        south pole distance epoch J2000.0 (ICRS) (1)
        [FieldOffset(4)]
		internal int spd;

        //  I*2 millimag   UCAC fit model magnitude                 (2)
        [FieldOffset(8)]
        internal short magm;

		//  I*2 millimag   UCAC aperture  magnitude                 (2)
		[FieldOffset(10)]
		internal short maga;

        //  I*1 1/100 mag  error of UCAC magnitude                  (3)
        [FieldOffset(12)]
		internal byte sigmag;

        //  I*1            object type                              (4)
        [FieldOffset(13)]
		internal byte objt;

        //  I*1            combined double star flag                (5)
        [FieldOffset(14)]
		internal byte cdf;

        //  I*1 mas        s.e. at central epoch in RA (*cos Dec)   (6)
        [FieldOffset(15)]
		internal byte sigra;

        //  I*1 mas        s.e. at central epoch in Dec             (6)
        [FieldOffset(16)]
		internal byte sigdc;

        //  I*1            total # of CCD images of this star
        [FieldOffset(17)]
		internal byte na1;

        //  I*1            # of CCD images used for this star       (7)
        [FieldOffset(18)]
		internal byte nu1;

        //  I*1            # catalogs (epochs) used for proper motions
        [FieldOffset(19)]
		internal byte cu1;

        //  I*2 0.01 yr    central epoch for mean RA, minus 1900
        [FieldOffset(20)]
		internal short cepra;

        //  I*2 0.01 yr    central epoch for mean Dec,minus 1900
        [FieldOffset(22)]
		internal short cepdc;

        //  I*2 0.1 mas/yr proper motion in RA*cos(Dec)             (8)
        [FieldOffset(24)]
		internal short pmrac;

        //  I*2 0.1 mas/yr proper motion in Dec
        [FieldOffset(26)]
		internal short pmdc;

        //  I*1 0.1 mas/yr s.e. of pmRA * cos Dec                   (9)
        [FieldOffset(28)]
		internal byte sigpmr;

        //  I*1 0.1 mas/yr s.e. of pmDec                            (9)
        [FieldOffset(29)]
		internal byte sigpmd;

        //  I*4           2MASS unique star identifier            (10)
        [FieldOffset(30)]
		internal int pts_key;

        //  I*2 millimag   2MASS J  magnitude
        [FieldOffset(34)]
		internal short j_m;

        //  I*2 millimag   2MASS H  magnitude
        [FieldOffset(36)]
		internal short h_m;

        //  I*2 millimag   2MASS K_s magnitude
        [FieldOffset(38)]
		internal short k_m;

        //  I*1            2MASS cc_flg*10 + ph_qual flag for J    (11)
        [FieldOffset(40)]
		internal byte icqflg;

        //  I*1            2MASS cc_flg*10 + ph_qual flag for H    (11)
        [FieldOffset(41)]
		internal byte icqflg2;

        //  I*1            2MASS cc_flg*10 + ph_qual flag for J    (11)
        [FieldOffset(42)]
		internal byte icqflg3;

        //  I*1 1/100 mag  error 2MASS J   magnitude               (12)
        [FieldOffset(43)]
		internal byte e2mpho;

        //  I*1 1/100 mag  error 2MASS H   magnitude               (12)
        [FieldOffset(44)]
		internal byte e2mpho2;

        //  I*1 1/100 mag  error 2MASS K_s magnitude               (12)
        [FieldOffset(45)]
		internal byte e2mpho3;

        //  I*2 millimag   B magnitude from APASS                  (13)
        [FieldOffset(46)]
		internal short apasm_B;

        //  I*2 millimag   V magnitude from APASS                  (13)
        [FieldOffset(48)]
		internal short apasm_V;

        //  I*2 millimag   g magnitude from APASS                  (13)
        [FieldOffset(50)]
		internal short apasm_g;

        //  I*2 millimag   r magnitude from APASS                  (13)
        [FieldOffset(52)]
		internal short apasm_r;

        //  I*2 millimag   i magnitude from APASS                  (13)
        [FieldOffset(54)]
		internal short apasm_i;

        //  I*1 1/100 mag  error of B magnitude from APASS         (14)
        [FieldOffset(56)]
		public byte apase_B;

        //  I*1 1/100 mag  error of V magnitude from APASS         (14)
        [FieldOffset(57)]
		public byte apase_V;

        //  I*1 1/100 mag  error of g magnitude from APASS         (14)
        [FieldOffset(58)]
		public byte apase_g;

        //  I*1 1/100 mag  error of r magnitude from APASS         (14)
        [FieldOffset(59)]
		public byte apase_r;

        //  I*1 1/100 mag  error of i magnitude from APASS         (14)
        [FieldOffset(60)]
		public byte apase_i;

        //  I*1            Yale SPM g-flag*10  c-flag              (15)
        [FieldOffset(61)]
		internal byte gcflg;

        //  I*4            FK6-Hipparcos-Tycho source flag         (16)
        //  ..             AC2000       catalog match flag         (17)
        //  ..             AGK2 Bonn    catalog match flag         (17)
        //  ..             AKG2 Hamburg catalog match flag         (17)
        //  ..             Zone Astrog. catalog match flag         (17)
        //  ..             Black Birch  catalog match flag         (17)
        //  ..             Lick Astrog. catalog match flag         (17)
        //  ..             NPM  Lick    catalog match flag         (17)
        //  ..             SPM  YSJ1    catalog match flag         (17)
        [FieldOffset(62)]
		internal int icf;

        //  I*1            LEDA galaxy match flag                  (18)
        [FieldOffset(66)]
		internal byte leda;

        //  I*1            2MASS extend.source flag                (19)
        [FieldOffset(67)]
		internal byte x2m;

        //  I*4            unique star identification number       (20)
        [FieldOffset(68)]
		internal int rnm;

        //  I*2            zone number of UCAC2 (0 = no match)     (21)
        //[FieldOffset(72)]
        //public short zn2;

        //  I*4            running record number along UCAC2 zone  (21)
        //[FieldOffset(74)]
        //public int rn2;


        private static double s_pmDiff = 0;
        public const int Size = 78;

        public static float TargetEpoch
        {
            set
            {
                s_pmDiff = value - 2000.0;
            }
        }

        public void InitUCAC4Entry()
        {
            //m_RAJ2000 = double.NaN;
            //m_DEJ2000 = double.NaN;
        }

        public void InitUCAC4Entry(ushort zone, uint starInZone)
        {
            InitUCAC4Entry();

            m_Zone = zone;
            m_StarInZone = starInZone;
        }

        [FieldOffset(74)] /* Overwrite the 10 major catalogs matched */
        private uint m_StarInZone;
        [FieldOffset(72)] /* Overwrite the 10 major catalogs matched */
        private ushort m_Zone;

        public double RAJ2000
        {
            get
            {
                return (ra + s_pmDiff * ProperMotionRA) / 3600000.0;
            }
        }

        public double DEJ2000
        {
            get
            {
                return ((spd + s_pmDiff * ProperMotionDE) / 3600000.0) - 90.0;
            }
        }

        public double RACat
        {
            get
            {
                return ra / 3600000.0;
            }
        }

        public double DECat
        {
            get
            {
                return (spd / 3600000.0) - 90.0;
            }
        }

        public double ProperMotionRA
        {
            get { return pmrac * 0.1 / Math.Cos(DEJ2000 * Math.PI / 180); }
        }

        public double ProperMotionDE
        {
            get { return pmdc * 0.1; }
        }

        #region IStar Members

        public ulong StarNo
        {
			get { return m_StarInZone + (ulong)10000000 * m_Zone; }
        }

        public double RADeg
        {
            get { return RAJ2000; }
        }

        public double DEDeg
        {
            get { return DEJ2000; }
        }

        public double Mag
        {
            get { return magm * 0.001; }
        }

        public double Mag_g
        {
            get
            {
                if (apasm_g != 20000)
                    return apasm_g * 0.001;
                else
                    return double.NaN;
            }
        }

        public double Mag_r
        {
            get
            {
                if (apasm_r != 20000)
                    return apasm_r * 0.001;
                else
                    return double.NaN;
            }
        }

        public double Mag_i
        {
            get
            {
                if (apasm_i != 20000)
                    return apasm_i * 0.001;
                else
                    return double.NaN;
            }
        }

        public double MagR
        {
            get
            {
                // Johnson-Cousins BVRI photometry for faint field stars
                // ftp://ftp.lowell.edu/pub/bas/starcats/loneos.phot 
                // R = V - 0.508 * (B - V) - 0.040   (0.3 < B - V < 0.9) 
                double b_min_v = MagB - MagV;

                if (b_min_v > 0.3 && b_min_v < 0.9)
                    return MagV - 0.508 * b_min_v - 0.040;
                else
                    return double.NaN;
            }
        }

        public double MagB
        {
            get
            {
                if (apasm_B != 20000)
                    return apasm_B * 0.001;
                else
                    return double.NaN;
            }
        }

        public double MagV
        {
            get
            {
                if (apasm_V != 20000)
                    return apasm_V * 0.001;
                else
                    return double.NaN;
            }
        }

        public double MagJ
        {
            get
            {
                if (j_m != 20000)
                    return j_m * 0.001;
                else
                    return double.NaN;
            }
        }

        public double MagK
        {
            get
            {
                if (k_m != 20000)
                    return k_m * 0.001;
                else
                    return double.NaN;
            }
        }

        public string GetStarDesignation(int alternativeId)
        {
            return string.Concat("4U ", m_Zone.ToString().PadLeft(3, '0'), "-", m_StarInZone.ToString().PadLeft(7, '0'));
        }

        public double GetMagnitudeForBand(Guid magBandId)
        {
            if (magBandId == BAND_ID_V)
                return MagV;
            if (magBandId == BAND_ID_B)
                return MagB;
            if (magBandId == BAND_ID_R)
                return MagR;
            else if (magBandId == BAND_ID_SLOAN_g)
                return Mag_g;
            else if (magBandId == BAND_ID_SLOAN_r)
                return Mag_r;
            else if (magBandId == BAND_ID_SLOAN_i)
                return Mag_i;
            else if (magBandId == BAND_ID_J)
                return MagJ;
            else if (magBandId == BAND_ID_K)
                return MagK;
            else
                return Mag;
        }
        #endregion
    }
}
