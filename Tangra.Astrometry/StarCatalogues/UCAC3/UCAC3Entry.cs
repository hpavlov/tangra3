/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Tangra.StarCatalogues;

namespace Tangra.StarCatalogues.UCAC3
{
    /// <summary>
    /// num item   fmt unit        explanation                            remark
    //------------------------------------------------------------------------
    // 1  ra     I*4 mas         right ascension at  epoch J2000.0 (ICRS)  (1)
    // 2  spd    I*4 mas         south pole distance epoch J2000.0 (ICRS)  (1)
    // 3  im1    I*2 millimag    UCAC fit model magnitude                  (2)
    // 4  im2    I*2 millimag    UCAC aperture  magnitude                  (2)
    // 5  sigmag I*2 millimag    UCAC error on magnitude (larger of sc.mod)(3)
    // 6  objt   I*1             object type                               (4)   
    // 7  dsf    I*1             double star flag                          (5)   
    //         16
    // 8  sigra  I*2 mas         s.e. at central epoch in RA (*cos Dec)      
    // 9  sigdc  I*2 mas         s.e. at central epoch in Dec                 
    //10  na1    I*1             total # of CCD images of this star
    //11  nu1    I*1             # of CCD images used for this star        (6)
    //12  us1    I*1             # catalogs (epochs) used for proper motions
    //13  cn1    I*1             total numb. catalogs (epochs) initial match
    //          8
    //14  cepra  I*2 0.01 yr     central epoch for mean RA, minus 1900     
    //15  cepdc  I*2 0.01 yr     central epoch for mean Dec,minus 1900  
    //16  pmrac  I*4 0.1 mas/yr  proper motion in RA*cos(Dec)           
    //17  pmdc   I*4 0.1 mas/yr  proper motion in Dec                    
    //18  sigpmr I*2 0.1 mas/yr  s.e. of pmRA * cos Dec                   
    //19  sigpmd I*2 0.1 mas/yr  s.e. of pmDec                            
    //         16
    //20  id2m   I*4             2MASS pts_key star identifier          
    //21  jmag   I*2 millimag    2MASS J  magnitude                     
    //22  hmag   I*2 millimag    2MASS H  magnitude                       
    //23  kmag   I*2 millimag    2MASS K_s magnitude                     
    //24  icqflg I*1 * 3         2MASS cc_flg*10 + phot.qual.flag          (7)
    //25  e2mpho I*1 * 3         2MASS error photom. (1/100 mag)           (8)
    //         16
    //26  smB    I*2 millimag    SuperCosmos Bmag
    //27  smR2   I*2 millimag    SC R2mag                                  (9)
    //28  smI    I*2 millimag    SC Imag
    //29  clbl   I*1             SC star/galaxy classif./quality flag     (10)
    //30  qfB    I*1             SC quality flag Bmag                     (11)
    //31  qfR2   I*1             SC quality flag R2mag                    (11)
    //32  qfI    I*1             SC quality flag Imag                     (11)
    //         10
    //33  catflg I*1 * 10        mmf flag for 10 major catalogs matched   (12)
    //34  g1     I*1             Yale SPM object type (g-flag)            (13)
    //35  c1     I*1             Yale SPM input cat.  (c-flag)            (14)
    //36  leda   I*1             LEDA galaxy match flag                   (15)
    //37  x2m    I*1             2MASS extend.source flag                 (16)
    //38  rn     I*4             MPOS star number; identifies HPM stars   (17)
    //         18
    //------------------------------------------------------------------------
    //         84 = total number of bytes per star record
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    [Serializable]
    public class UCAC3Entry : IStar
    {
        public static Guid BAND_ID_UNFILTERED = new Guid("F019643E-AEF5-4026-BE25-DC77CA33AEFA");
        public static Guid BAND_ID_V = new Guid("BD7ACDC0-314A-4FCF-9772-597A5C22835D");
        public static Guid BAND_ID_R = new Guid("43F4C0FF-B3D4-4F94-9734-9DFB17F31E9C");
		public static Guid BAND_ID_J = new Guid("DFF77F5A-00C9-4320-B903-3C576AC7D664");
    	public static Guid BAND_ID_K = new Guid("16AA9F7E-DCF8-4E6C-A9D2-14A0BD9AC4F2");

    	//      I*4 mas         right ascension at  epoch J2000.0 (ICRS)  (1)
    	[FieldOffset(0)]
    	private int ra;

    	//    I*4 mas         south pole distance epoch J2000.0 (ICRS)  (1)
    	[FieldOffset(4)]
    	private int spd;


    	// I*2 millimag    UCAC fit model magnitude                  (2)
    	[FieldOffset(8)]
    	public short fMag;

    	//     I*2 millimag    UCAC aperture  magnitude                  (2)
    	[FieldOffset(10)]
    	public short apMag;

    	// I*2 millimag    UCAC error on magnitude (larger of sc.mod)(3)
    	[FieldOffset(12)]
    	private short sigmag;

    	//    I*1             object type                               (4)  
    	[FieldOffset(14)]
    	private byte objt;

    	//     I*1             double star flag                          (5)  
    	[FieldOffset(15)]
    	private byte dsf;

    	// 16 bytes

    	// I*2 mas         s.e. at central epoch in RA (*cos Dec)      
    	[FieldOffset(16)]
    	private short sigra;

    	//  I*2 mas         s.e. at central epoch in Dec                 
    	[FieldOffset(18)]
    	private short sigdc;

    	//     I*1             total # of CCD images of this star
    	[FieldOffset(20)]
    	private byte na1;

    	//     I*1             # of CCD images used for this star        (6)
    	[FieldOffset(21)]
    	private byte nu1;

    	//     I*1             # catalogs (epochs) used for proper motions
    	[FieldOffset(22)]
    	private byte us1;

    	//     I*1             total numb. catalogs (epochs) initial match
    	[FieldOffset(23)]
    	private byte cn1;


    	// 8 bytes

    	//   I*2 0.01 yr     central epoch for mean RA, minus 1900     
    	[FieldOffset(24)]
    	private short cepra;

    	// I*2 0.01 yr     central epoch for mean Dec,minus 1900  
    	[FieldOffset(26)]
    	private short cepdc;

    	// I*4 0.1 mas/yr  proper motion in RA*cos(Dec)           
    	[FieldOffset(28)]
    	private int pmrac;

    	//    I*4 0.1 mas/yr  proper motion in Dec                    
    	[FieldOffset(32)]
    	private int pmdc;

    	//  I*2 0.1 mas/yr  s.e. of pmRA * cos Dec                   
    	[FieldOffset(36)]
    	private short sigpmr;

    	//  I*2 0.1 mas/yr  s.e. of pmDec                            
    	[FieldOffset(38)]
    	private short sigpmd;


    	// 16 bytes

    	//    I*4             2MASS pts_key star identifier          
    	[FieldOffset(40)]
    	private int id2m;

    	//    I*2 millimag    2MASS J  magnitude                     
    	[FieldOffset(44)]
    	public short jmag;

    	//    I*2 millimag    2MASS H  magnitude                       
    	[FieldOffset(46)]
    	private short hmag;

    	//    I*2 millimag    2MASS K_s magnitude   
    	[FieldOffset(48)]
    	public short kmag;

    	//  I*1 * 3         2MASS cc_flg*10 + phot.qual.flag          (7)
    	[FieldOffset(50)]
    	private byte icqflg_1;
    	[FieldOffset(51)]
    	private byte icqflg_2;
    	[FieldOffset(52)]
    	private byte icqflg_3;

    	//  I*1 * 3         2MASS error photom. (1/100 mag)           (8)
    	[FieldOffset(53)]
    	private byte e2mpho_1;
    	[FieldOffset(54)]
    	private byte e2mpho_2;
    	[FieldOffset(55)]
    	private byte e2mpho_3;

    	// 16 bytes

    	//     I*2 millimag    SuperCosmos Bmag
    	[FieldOffset(56)]
    	private short smB;

    	//   I*2 millimag    SC R2mag                                  (9)
    	[FieldOffset(58)]
    	private short smR2;

    	//     I*2 millimag    SC Imag
    	[FieldOffset(60)]
    	private short smI;

    	//    I*1             SC star/galaxy classif./quality flag     (10)
    	[FieldOffset(62)]
    	private byte clbl;

    	//     I*1             SC quality flag Bmag                     (11)
    	[FieldOffset(63)]
    	private byte qfB;

    	//    I*1             SC quality flag R2mag                    (11)
    	[FieldOffset(64)]
    	private byte qfR2;

    	//     I*1             SC quality flag Imag                     (11)
    	[FieldOffset(65)]
    	private byte qfI;

    	//  I*1 * 10        mmf flag for 10 major catalogs matched   (12)
    	[FieldOffset(66)]
    	private byte catflg_1;
    	[FieldOffset(67)]
    	private byte catflg_2;
    	[FieldOffset(68)]
    	private byte catflg_3;
    	[FieldOffset(69)]
    	private byte catflg_4;
    	[FieldOffset(70)]
    	private byte catflg_5;
    	[FieldOffset(71)]
    	private byte catflg_6;
    	[FieldOffset(72)]
    	private byte catflg_7;
    	[FieldOffset(73)]
    	private byte catflg_8;
    	[FieldOffset(74)]
    	private byte catflg_9;
    	[FieldOffset(75)]
    	private byte catflg_10;

    	//      I*1             Yale SPM object type (g-flag)            (13)
    	[FieldOffset(76)]
    	private byte g1;

    	//      I*1             Yale SPM input cat.  (c-flag)            (14)
    	[FieldOffset(77)]
    	private byte c1;

    	//    I*1             LEDA galaxy match flag                   (15)
    	[FieldOffset(78)]
    	private byte leda;

    	//     I*1             2MASS extend.source flag                 (16)
    	[FieldOffset(79)]
    	private byte x2m;

    	//      I*4             MPOS star number; identifies HPM stars   (17)
    	[FieldOffset(80)]
    	private int rn;


    	private static double s_pmDiff = 0;
    	public const int Size = 84;

    	public static float TargetEpoch
    	{
    		set
    		{
    			s_pmDiff = value - 2000.0;
    		}
    	}

    	public void InitUCAC3Entry()
    	{
    		//m_RAJ2000 = double.NaN;
    		//m_DEJ2000 = double.NaN;
    	}

    	public void InitUCAC3Entry(ushort zone, uint starInZone)
    	{
    		InitUCAC3Entry();

    		m_Zone = zone;
    		m_StarInZone = starInZone;
    	}

    	[FieldOffset(66)] /* Overwrite the 10 major catalogs matched */
    		private uint m_StarInZone;
    	[FieldOffset(70)] /* Overwrite the 10 major catalogs matched */
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

    	public double MagB
    	{
    		get { return smB * 0.001; }
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
    		get { return m_StarInZone + (uint)10000000 * m_Zone; }
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
    		get { return fMag * 0.001; }
    	}

    	public double MagRSuperCosmos
    	{
    		get { return smR2 * 0.001; } 
    	}

    	public double MagR
    	{
    		get
    		{
    			double jk = jmag * 0.001 - kmag * 0.001;
    			double f = fMag * 0.001;

    			return Math.Round(-0.295 * jk + 1.323*f - 0.0377*f*f + 0.001142*f*f*f - 0.68, 2);
    		}
    	}

		public double MagJ
		{
			get
			{
				if (jmag != 30000)
					return jmag * 0.001;
				else
					return double.NaN;
			}
		}

		public double MagK
		{
			get
			{
				if (kmag != 30000)
					return kmag * 0.001;
				else
					return double.NaN;
			}
		}
    	public double MagV
    	{
    		get
    		{
    			double jk = jmag*0.001 - kmag*0.001;
    			double f = fMag*0.001;

    			return Math.Round(0.552*jk + 1.578*f - 0.0560*f*f + 0.001562*f*f*f - 1.76, 2);
    		}
    	}

    	public string GetStarDesignation(int alternativeId)
    	{
    		return string.Concat("3U ", m_Zone.ToString().PadLeft(3, '0'), "-", m_StarInZone.ToString().PadLeft(7, '0'));
    	}

    	public double GetMagnitudeForBand(Guid magBandId)
    	{
    		if (magBandId == BAND_ID_V)
    			return MagV;
    		else if (magBandId == BAND_ID_R)
    			return MagR;
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
