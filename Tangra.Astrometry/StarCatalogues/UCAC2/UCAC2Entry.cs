/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Tangra.StarCatalogues;

namespace Tangra.StarCatalogues.UCAC2
{
    // Num Bytes  Fmt Unit       Label   Explanation
    //-----------------------------------------------------------------------------
    // 1   1- 4  I*4 mas        RA      Right Ascension at epoch J2000.0 (ICRS) (2)
    // 2   5- 8  I*4 mas        DE      Declination at epoch J2000.0 (ICRS)     (2)
    // 3   9-10  I*2 0.01 mag   U2Rmag  Internal UCAC magnitude (red bandpass)  (3)
    // 4  11     I*1 mas        e_RAm   s.e. at central epoch in RA (*cos DEm)(1,4)
    // 5  12     I*1 mas        e_DEm   s.e. at central epoch in Dec          (1,4)
    // 6  13     I*1            nobs    Number of UCAC observations of this star(5)
    // 7  14     I*1            e_pos   Error of original UCAC observ. (mas)  (1,6)
    // 8  15     I*1            ncat    # of catalog positions used for pmRA, pmDC
    // 9  16     I*1            cflg    ID of major catalogs used in pmRA, pmDE (7)
    //10  17-18  I*2 0.001 yr   EpRAm   Central epoch for mean RA, minus 1975   (8)
    //11  19-20  I*2 0.001 yr   EpDEm   Central epoch for mean DE, minus 1975   (8)
    //12  21-24  I*4 0.1 mas/yr pmRA    Proper motion in RA (no cos DE)         (9)
    //13  25-28  I*4 0.1 mas/yr pmDE    Proper motion in DE                     (9)
    //14  29     I*1 0.1 mas/yr e_pmRA  s.e. of pmRA (*cos DEm)                 (1)
    //15  30     I*1 0.1 mas/yr e_pmDE  s.e. of pmDE                            (1)
    //16  31     I*1 0.05       q_pmRA  Goodness of fit for pmRA             (1,11)
    //17  32     I*1 0.05       q_pmDE  Goodness of fit for pmDE             (1,11)
    //18  33-36  I*4            2m_id   2MASS pts_key star identifier          (12)
    //19  37-38  I*2 0.001 mag  2m_J    2MASS J  magnitude                     (13)
    //20  39-40  I*2 0.001 mag  2m_H    2MASS H  magnitude                     (13)
    //21  41-42  I*2 0.001 mag  2m_Ks   2MASS K_s magnitude                    (13)
    //22  43     I*1            2m_ph   2MASS modified ph_qual flag          (1,14)
    //23  44     I*1            2m_cc   2MASS modified cc_flg                (1,15)
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Explicit)]
    [Serializable]
    public struct UCAC2Entry : IStar
    {
        public static Guid BAND_ID_UNFILTERED = new Guid("1EF71A89-79F6-4B62-9D64-85FA9FF60B0A");
        public static Guid BAND_ID_V = new Guid("5B71D457-B124-4377-AFAC-D530ECEAD842");
        public static Guid BAND_ID_R = new Guid("51516B5B-FAE5-4947-A0C4-45CD63D80F9D");
		public static Guid BAND_ID_J = new Guid("937CADA3-200A-4779-806E-BA155FB56F1D");
    	public static Guid BAND_ID_K = new Guid("1F9EE3D0-FDCC-4D5E-8BFA-BB64F385C08A");

    	public const int FIRST_BSS_STAR_NO = 50000000;

    	public const int Size = 44;

    	public static float TargetEpoch
    	{
    		set
    		{
    			s_pmDiff = value - 2000.0;
    		}
    	}

    	public void InitUCAC2Entry()
    	{
    		//m_RAJ2000 = double.NaN;
    		//m_DEJ2000 = double.NaN;
    	}

    	[FieldOffset(32)] /* Overwrite the 2M id*/
    	private uint m_UCAC2;

    	public void InitUCAC2Entry(uint ucac2No)
    	{
    		InitUCAC2Entry();

    		m_UCAC2 = ucac2No;
    	}

    	[FieldOffset(0)]
    	public UInt32 RA;
    	[FieldOffset(4)]
    	public Int32 DE;
    	[FieldOffset(8)]
    	public Int16 U2Rmag;
    	[FieldOffset(10)]
    	public byte e_RAm;
    	[FieldOffset(11)]
    	public byte e_DEm;
    	[FieldOffset(12)]
    	public byte nobs;
    	[FieldOffset(13)]
    	public byte e_pos;
    	[FieldOffset(14)]
    	public byte ncat;
    	[FieldOffset(15)]
    	public byte cflg;
    	[FieldOffset(16)]
    	public Int16 EpRAm;
    	[FieldOffset(18)]
    	public Int16 EpDEm;
    	[FieldOffset(20)]
    	public Int32 pmRA;
    	[FieldOffset(24)]
    	public Int32 pmDE;
    	[FieldOffset(28)]
    	public byte e_pmRA;
    	[FieldOffset(29)]
    	public byte e_pmDE;
    	[FieldOffset(30)]
    	public byte q_pmRA;
    	[FieldOffset(31)]
    	public byte q_pmDE;
    	[FieldOffset(32)]
    	public Int32 _2m_id;
    	[FieldOffset(36)]
    	public Int16 _2m_J;
    	[FieldOffset(38)]
    	public Int16 _2m_H;
    	[FieldOffset(40)]
    	public Int16 _2m_Ks;
    	[FieldOffset(42)]
    	public byte _2m_ph;
    	[FieldOffset(43)]
    	public byte _2m_cc;

        
    	//[FieldOffset(0)]
    	//private double m_RAJ2000;
    	//[FieldOffset(0)]
    	//private double m_DEJ2000;
    	private static double s_pmDiff = 0;

    	public double RAJ2000
    	{
    		get 
    		{
    			return (RA + s_pmDiff * ProperMotionRA) / 3600000.0; 
    		}
    	}

    	public double DEJ2000
    	{
    		get 
    		{
    			return (DE + s_pmDiff * ProperMotionDE) / 3600000.0;
    		}
    	}

    	public double RACat
    	{
    		get
    		{
    			return RA / 3600000.0;
    		}
    	}

    	public double DECat
    	{
    		get
    		{
    			return DE / 3600000.0;
    		}
    	}

    	//  1) Get the RA, DEC for each star in the master catalogue. Then for the selected guide stars
    	//     save the ID, RA, DEC, Vx, Vy, Vz; where Vx = Cos(RA) * Cos (DE), Vy = Sin(RA) * Cos(DE), Vz = Sin(DE)

    	public double VX
    	{
    		get { return Math.Cos(RAJ2000 * Math.PI / 180) * Math.Cos(DEJ2000 * Math.PI / 180); }
    	}

    	public double VY
    	{
    		get { return Math.Sin(RAJ2000 * Math.PI / 180) * Math.Cos(DEJ2000 * Math.PI / 180); }
    	}

    	public double VZ
    	{
    		get { return Math.Sin(DEJ2000 * Math.PI / 180); }
    	}

    	public double Mag
    	{
    		get { return U2Rmag * 0.01; }
    	}

    	public double MagB
    	{
    		get
    		{
    			return double.NaN;
    		}
    	}

    	public double MagV
    	{
    		get
    		{
    			return (MagJ - MagK) * 0.482510826567376 + Mag * 0.69853741307966 + Mag * Mag * 0.053709249992501 +
					   Mag * Mag * Mag * -0.00221766445441987 + -0.186573576388743;
			}
    	}

    	public double MagR
    	{
    		get
    		{
    			return U2Rmag * 0.01;
    		}
    	}

		public double MagJ
		{
			get
			{
				if (_2m_J != 30000)
					return _2m_J * 0.001;
				else
					return double.NaN;
			}
		}

		public double MagK
		{
			get
			{
				if (_2m_Ks != 30000)
					return _2m_Ks * 0.001;
				else
					return double.NaN;
			}
		}

        public bool IsForInitialPlateSolve
        {
            get
            {
                // We only all star for the initial plate solve
                return true;
            }
        }

    	public double EpochRA
    	{
    		get { return EpRAm / 1000.0 + 1975; }
    	}

    	public double EpochDE
    	{
    		get { return EpDEm / 1000.0 + 1975; }
    	}

    	public double ProperMotionRA
    	{
    		get { return pmRA * 0.1 / Math.Cos(DEJ2000 * Math.PI / 180); }
    	}

    	public double ProperMotionDE
    	{
    		get { return pmDE * 0.1; }
    	}

    	#region IStar Members

    	public ulong StarNo
    	{
    		get { return m_UCAC2; }
    	}

    	public string GetStarDesignation(int alternativeId)
    	{
    		return string.Concat("UCAC2 ", m_UCAC2.ToString());
    	}

    	public double RADeg
    	{
    		get { return RAJ2000; }
    	}

    	public double DEDeg
    	{
    		get { return DEJ2000; }
    	}

    	public double GetMagnitudeForBand(Guid magBandId)
    	{

    		if (magBandId == BAND_ID_V)
				return (MagJ - MagK) * 0.482510826567376 + Mag * 0.69853741307966 + Mag * Mag * 0.053709249992501 +
					   Mag * Mag * Mag * -0.00221766445441987 + -0.186573576388743;
    		else if (magBandId == BAND_ID_R)
				return (MagJ - MagK) * -0.262157262293991 + Mag * 0.972995989114809 + Mag * Mag * 0.0294054519219995 +
    			       Mag*Mag*Mag*-0.00152172319138341 + -1.27151807006964;
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
