/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Tangra.Model.Helpers;

namespace Tangra.StarCatalogues.PPMXL
{

    //--------------------------------------------------------------------------------
    //   Bytes Format Units    Label   Explanations
    //--------------------------------------------------------------------------------
    //   1- 19  A19   ---      PPMXL   Identifier (5)
    //  21- 30  F10.6 deg      RAdeg   Right Ascension J2000.0, epoch 2000.0
    //  31- 40  F10.6 deg      DEdeg   Declination J2000.0, epoch 2000.0
    //  42- 49  F8.1  mas/yr   pmRA    Proper Motion in RA*cos(DEdeg)
    //  51- 58  F8.1  mas/yr   pmDE    Proper Motion in Dec
    //  60- 66  F7.2  yr       epRA    Mean Epoch (RA)
    //  68- 74  F7.2  yr       epDE    Mean Epoch (Dec)
    //  76- 78  I3    mas     e_RAdeg  Mean error in RA*cos(DEdeg) at mean epoch (6)
    //  80- 82  I3    mas     e_DEdeg  Mean error in Dec at mean epoch (6)
    //  84- 87  F4.1  mas/yr   e_pmRA  Mean error in pmRA*cos(DEdeg)
    //  89- 92  F4.1  mas/yr   e_pmDE  Mean error in pmDec
    //  94- 99  F6.3  mag      Jmag    ?=- J magnitude from 2MASS (Cat. II/246)
    //     100  A1    ---      ---     [:]
    // 101-105  F5.3  mag      e_Jmag  ?=- J total magnitude uncertainty
    // 107-112  F6.3  mag      Hmag    ?=- H magnitude from 2MASS (Cat. II/246)
    //     113  A1    ---      ---     [:]
    // 114-118  F5.3  mag      e_Hmag  ?=- H total magnitude uncertainty
    // 120-125  F6.3  mag      Kmag    ?=- Ks magnitude from 2MASS (Cat. II/246)
    //     126  A1    ---      ---     [:]
    // 127-131  F5.3  mag      e_Kmag  ?=- Ks total magnitude uncertainty
    // 133-137  F5.2  mag      b1mag   ?=- B mag from USNO-B, first epoch (1)
    // 139-143  F5.2  mag      b2mag   ?=- B mag from USNO-B, second epoch (1)
    // 145-149  F5.2  mag      r1mag   ?=- R mag from USNO-B, first epoch (1)
    // 151-155  F5.2  mag      r2mag   ?=- R mag from USNO-B, second epoch (1)
    // 157-161  F5.2  mag      imag    ?=- I mag from USNO-B (1)
    // 163-167  A5    ---      Smags   [0-8-] Surveys used for USNO-B magnitudes (2)
    // 169-170  I2    ---      No      ?=- Number of observations used (4)
    // 172-173  I2    ---      fl      Flags (3)
    //--------------------------------------------------------------------------------
    public class PPMXLEntry : IStar
    {
        public static int Size = 174;

        public static Guid BAND_ID_B = new Guid("E39CE3CD-E4E0-404B-B11E-DA3CCF1F1400");
        public static Guid BAND_ID_R = new Guid("4430CBF4-9B6F-49EC-AD6C-5DA4D4BA7BBF");
		public static Guid BAND_ID_J = new Guid("3E12D334-ED8B-42A5-9ECE-9E48CD395D79");
    	public static Guid BAND_ID_K = new Guid("249C45BA-8F35-47E4-A21A-1CF64A50636F");

        //#PPMXL_ID          |   RA   (ICRS)Dec        pmRA     pmDE|  epRA    epDE  eRA eDE epma epmd|  Jmag:+/-     Hmag:+/-     Kmag:+/-  |b1mag b2mag r1mag r2mag  imag|bbrri|No|fl
        // 480381039136370330|000.031470+89.788187    +10.4     -6.7|1985.06 1985.06  89  89  4.2  4.2|16.229:0.126 15.708:0.180 15.497:0.223|19.73 20.28 17.96 18.32 17.35|02137| 6| 0
        // 480381586595717165|000.170323+89.839475     +7.0     -3.4|1980.36 1980.36 103 103  4.5  4.5| ---  :---    ---  :---    ---  :---  |20.83 20.72 18.95 19.38 18.51|02137| 5| 0
        // 480381168402082849|000.211899+89.809931   -185.1   -313.1|1983.28 1983.28 133 133  6.1  6.1| ---  :---    ---  :---    ---  :---  |21.03  ---   ---  19.79 18.46|0--37| 3| 0
        //012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789
        //          1         2         3         4         5         6         7         8         9         0         1         2         3         4         5         6         7            
        //                                                                                                    1         1         1         1         1         1         1         1 
        private string m_DataLine;

        private ulong m_PPMXL_ID;
        private double m_RADeg;
        private double m_DEDeg;
        private double m_pmRA;
        private double m_pmDE;
        private double m_b1Mag = double.NaN;
        private double m_b2Mag = double.NaN;
        private double m_r1Mag = double.NaN;
        private double m_r2Mag = double.NaN;
        private double m_jMag = double.NaN;
        private double m_kMag = double.NaN;
        private double m_bMag = double.NaN;
        private double m_rMag = double.NaN;

        public PPMXLEntry(string line)
        {
            m_DataLine = line;
            m_PPMXL_ID = ulong.Parse(m_DataLine.Substring(0, 18).Trim());
            m_RADeg = double.Parse(m_DataLine.Substring(20, 10).Trim());
            m_DEDeg = double.Parse(m_DataLine.Substring(30, 10).Trim());
            m_pmRA = double.Parse(m_DataLine.Substring(41, 8).Trim());
            m_pmDE = double.Parse(m_DataLine.Substring(50, 8).Trim());
            m_pmRA = m_pmRA / Math.Cos(m_DEDeg * Math.PI / 180);

            string magStr = m_DataLine.Substring(93, 6).Trim();
            if (magStr != "---") m_jMag = double.Parse(magStr);

            magStr = m_DataLine.Substring(119, 6).Trim();
            if (magStr != "---") m_kMag = double.Parse(magStr);

            magStr = m_DataLine.Substring(132, 5).Trim();
            if (magStr != "---") m_b1Mag = double.Parse(magStr);

            magStr = m_DataLine.Substring(138, 5).Trim();
            if (magStr != "---") m_b2Mag = double.Parse(magStr);

            magStr = m_DataLine.Substring(144, 5).Trim();
            if (magStr != "---") m_r1Mag = double.Parse(magStr);

            magStr = m_DataLine.Substring(150, 5).Trim();
            if (magStr != "---") m_r2Mag = double.Parse(magStr);

            if (!double.IsNaN(m_r1Mag) && !double.IsNaN(m_r2Mag)) m_rMag = (m_r1Mag + m_r2Mag) / 2;
            if (!double.IsNaN(m_r1Mag) && double.IsNaN(m_r2Mag)) m_rMag = m_r1Mag;
            if (double.IsNaN(m_r1Mag) && !double.IsNaN(m_r2Mag)) m_rMag = m_r2Mag;

            if (!double.IsNaN(m_b1Mag) && !double.IsNaN(m_b2Mag)) m_bMag = (m_b1Mag + m_b2Mag) / 2;
            if (!double.IsNaN(m_b1Mag) && double.IsNaN(m_b2Mag)) m_bMag = m_b1Mag;
            if (double.IsNaN(m_b1Mag) && !double.IsNaN(m_b2Mag)) m_bMag = m_b2Mag;
        }

        private static double s_pmDiff = 0;

        public static float TargetEpoch
        {
            set
            {
                s_pmDiff = value - 2000.0;
            }
        }

        public double RAJ2000
        {
            get
            {
                return m_RADeg + (s_pmDiff * m_pmRA) / 3600000.0;
            }
        }

        public double DEJ2000
        {
            get
            {
                return m_DEDeg + (s_pmDiff * m_pmDE) / 3600000.0;
            }
        }

        public double pmRA
        {
            get { return m_pmRA; }
        }

        public double pmDE
        {
            get { return m_pmDE; }
        }

        public ulong StarNo
        {
            get { return m_PPMXL_ID; }
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
                if (!double.IsNaN(m_rMag))
                    return m_rMag;
                else if (!double.IsNaN(m_bMag))
                    return m_bMag;
                else if (!double.IsNaN(m_jMag))
                    return m_jMag;
                else if (!double.IsNaN(m_kMag))
                    return m_kMag;

                return double.NaN;
            }
        }

        public double MagR
        {
            get { return m_rMag; }
        }

        public double MagB
        {
            get { return m_bMag; }
        }

        public double MagV
        {
            get { throw new NotImplementedException(); }
        }

        public double MagJ
        {
            get { return m_jMag; }
        }

        public double MagK
        {
            get { return m_kMag; }
        }

        public bool IsForInitialPlateSolve
        {
            get
            {
                // We only all star for the initial plate solve
                return true;
            }
        }

        public string GetStarDesignation(int alternativeId)
        {
            // PPMXL number and HHDDMM.S+DDMMSS designations

            if (alternativeId == 1)
                return IAU_Id();

            return PPMXL_Id();
        }

        private string PPMXL_Id()
        {
            return string.Format("PPMXL {0}", m_PPMXL_ID);
        }

        private string IAU_Id()
        {
            return string.Format("PPMXL {0}{1}", 
                AstroConvert.ToStringValue(m_RADeg, "HHMMSS.T"),
                AstroConvert.ToStringValue(m_DEDeg, "+DDMMSS"));
        }

        public double GetMagnitudeForBand(Guid magBandId)
        {
            if (BAND_ID_B == magBandId)
                return m_bMag;
            else if (BAND_ID_R == magBandId)
                return m_rMag;
    		else if (BAND_ID_J == magBandId)
    			return m_jMag;
			else if (BAND_ID_K == magBandId)
    			return m_kMag;
            else
                return Mag;
        }


    }
}
