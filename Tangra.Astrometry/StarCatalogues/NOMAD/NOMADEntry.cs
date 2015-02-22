/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Tangra.StarCatalogues;

namespace Tangra.StarCatalogues.NOMAD
{
    //Each record contains 22 integers (4 byte).  The schema is:

    //   ( 1)   RA at 2000.0 in integer 0.001 arcsec
    //   ( 2)   SPD at 2000.0 in integer 0.001 arcsec
    //   ( 3)   std. dev. of RA*COS(dec) in integer 0.001 arcsec at central epoch
    //   ( 4)   std. dev. of SPD in integer 0.001 arcsec at central epoch
    //   ( 5)   proper motion of RA*COS(dec) in integer 0.0001 arcsec/year
    //   ( 6)   proper motion of SPD in integer 0.0001 arcsec/year
    //   ( 7)   std. dev. of (5) in integer 0.0001 arcsec/year
    //   ( 8)   std. dev. of (6) in integer 0.0001 arcsec/year
    //   ( 9)   central epoch of RA in integer 0.001 year
    //   (10)   central epoch of SPD in integer 0.001 year
    //   (11)   B magnitude in integer 0.001 mag
    //   (12)   V magnitude in integer 0.001 mag
    //   (13)   R magnitude in integer 0.001 mag
    //   (14)   J magnitude in integer 0.001 mag
    //   (15)   H magnitude in integer 0.001 mag
    //   (16)   K magnitude in integer 0.001 mag
    //   (17)   USNO-B1.0 ID integer
    //   (18)   2MASS ID integer
    //   (19)   YB6 ID integer
    //   (20)   UCAC-2 ID integer
    //   (21)   Tycho2 ID integer
    //   (22)   flags integer
    [StructLayout(LayoutKind.Explicit)]
    [Serializable]
    public class NOMADEntry : IStar
    {
        public static Guid BAND_ID_B = new Guid("1A81FD1A-DDBA-49F0-8A1A-BA3575F0642B");
        public static Guid BAND_ID_V = new Guid("DDC4D4F5-66C6-4B99-B635-98716A8B5BFD");
        public static Guid BAND_ID_R = new Guid("151D91EC-7D64-4469-8124-0A985CC2AF60");
		public static Guid BAND_ID_J = new Guid("AE9A7570-6F03-4702-AB78-783B7C0ABBD4");
    	public static Guid BAND_ID_K = new Guid("7A1525DE-4B44-41C9-AF10-A713B3B4380F");

    	public const int Size = 88;

    	private static double s_pmDiff = 0;

    	public static float TargetEpoch
    	{
    		set
    		{
    			s_pmDiff = value - 2000.0;
    		}
    	}

    	[FieldOffset(0)]
    	public UInt32 RA;

    	[FieldOffset(4)]
    	public Int32 DE;

    	[FieldOffset(8)]
    	public Int32 e_RA;

    	[FieldOffset(12)]
    	public Int32 e_DE;

    	[FieldOffset(16)]
    	public Int32 pmRA;

    	[FieldOffset(20)]
    	public Int32 pmDE;

    	[FieldOffset(24)]
    	public Int32 e_pmRA;

    	[FieldOffset(28)]
    	public Int32 e_pmDE;

    	[FieldOffset(32)]
    	public UInt32 epochRA;

    	[FieldOffset(36)]
    	public UInt32 epochDE;

    	[FieldOffset(40)]
    	public Int32 m_B;

    	[FieldOffset(44)]
    	public Int32 m_V;

    	[FieldOffset(48)]
    	public Int32 m_R;

    	[FieldOffset(52)]
    	public Int32 m_J;

    	[FieldOffset(56)]
    	public Int32 m_H;

    	[FieldOffset(60)]
    	public Int32 m_K;

    	[FieldOffset(64)]
    	public UInt32 no_USNOB1;

    	[FieldOffset(68)]
    	public UInt32 no_2MASS;

    	[FieldOffset(72)]
    	public UInt32 no_YB6;

    	[FieldOffset(76)]
    	public UInt32 no_UCAC2;

    	[FieldOffset(80)]
    	public UInt32 no_Tycho2;

    	[FieldOffset(84)]
    	public UInt32 flags;

       

    	public double RAJ2000
    	{
    		get
    		{
    			return (RA + s_pmDiff * ProperMotionRA) / 3600000.0 ;
    		}
    	}

    	public double DEJ2000
    	{
    		get
    		{
    			return (DE + s_pmDiff * ProperMotionDE) / 3600000.0 - 90.0;
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
    			return (DE / 3600000.0) - 90.0;
    		}
    	}

    	public double ProperMotionRA
    	{
    		get { return pmRA * 0.1 / Math.Cos(DEJ2000 * Math.PI / 180); }
    	}

    	public double ProperMotionDE
    	{
    		get { return pmDE * 0.1; }
    	}


    	[FieldOffset(32)]  /* Overwrite the EpochRA id*/
    		public UInt32 m_ZoneId;

    	[FieldOffset(36)]  /* Overwrite the EpochDE id*/
    		public UInt32 m_StarInZone;

    	[FieldOffset(72)] /* Overwrite the EpochDE id*/
    		public UInt32 m_NomadId;

    	public void InitNOMADEntry(uint nomadId, uint zone, uint starInZone)
    	{
    		m_NomadId = nomadId;
    		m_ZoneId = zone;
    		m_StarInZone = starInZone;
    	}

    	#region IStar Members

    	public ulong StarNo
    	{
    		get { return m_NomadId; }
    	}

    	public string GetStarDesignation(int alternativeId)
    	{
    		if (alternativeId == 1)
    			return NOMAD_Id();
    		else if (alternativeId == 2)
    			return UCAC2_Id();
    		else if (alternativeId == 3)
    			return _2M_Id();
    		else if (alternativeId == 4)
    			return TYC_Id();

    		if (no_Tycho2 != 0)
    			return TYC_Id();
    		else if (no_UCAC2 != 0)
    			return UCAC2_Id();
    		else if (no_2MASS != 0)
    			return _2M_Id();
    		else
    			return NOMAD_Id();
    	}

    	private string NOMAD_Id()
    	{
    		return string.Concat(
    			"NOMAD ",
    			m_ZoneId.ToString().PadLeft(4, '0'), /* The first zone is '0', not '1' */
    			"-",
    			(m_StarInZone + 1).ToString().PadLeft(7, '0') /* The first star in a zone has id '1' rather than '0' */
    			); 
    	}

    	private string TYC_Id()
    	{
    		if (no_Tycho2 == 0)
    			return string.Empty;
    		else
    			return string.Concat("TYC ", no_Tycho2.ToString());
    	}

    	private string UCAC2_Id()
    	{
    		if (no_UCAC2 == 0)
    			return string.Empty;
    		else
    			return string.Concat("UCAC2 ", no_UCAC2.ToString());
    	}

    	private string _2M_Id()
    	{
    		if (no_2MASS == 0)
    			return string.Empty;
    		else
    			return string.Concat("2M ", no_2MASS.ToString());
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
    		get
    		{
    			if (m_V != 30000)
    				return m_V / 1000.0;
    			else if (m_R != 30000)
    				return m_R / 1000.0;
    			else if (m_B != 30000)
    				return m_B / 1000.0;

    			return 30.0;
    		}
    	}

    	public double MagR
    	{
    		get 
    		{
    			if (m_R != 30000)
    				return m_R / 1000.0;
    			else
    				return double.NaN;
    		}
    	}

    	public double MagB
    	{
    		get
    		{
    			if (m_B != 30000)
    				return m_B / 1000.0;
    			else
    				return double.NaN;
    		}
    	}

    	public double MagV
    	{
    		get
    		{
    			if (m_V != 30000)
    				return m_V / 1000.0;
    			else
    				return double.NaN;
    		}
    	}

		public double MagJ
		{
			get
			{
				if (m_J != 30000)
					return m_J / 1000.0;
				else
					return double.NaN;
			}
		}

		public double MagK
		{
			get
			{
				if (m_K != 30000)
					return m_K / 1000.0;
				else
					return double.NaN;
			}
		}


    	public double GetMagnitudeForBand(Guid magBandId)
    	{
    		if (magBandId == BAND_ID_V)
    			return MagV;
    		else if (magBandId == BAND_ID_R)
    			return MagR;
    		else if (magBandId == BAND_ID_B)
    			return MagB;
    		else if (magBandId == BAND_ID_J)
    			return m_J / 1000.0;
    		else if (magBandId == BAND_ID_K)
    			return m_K / 1000.0;

    		return Mag;
    	}

    	#endregion
    }
}
