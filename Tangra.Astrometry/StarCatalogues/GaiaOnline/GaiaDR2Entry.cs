using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Tangra.StarCatalogues.GaiaOnline
{
    [Serializable]
    public struct GaiaDR2Entry : IStar
    {
        public static Guid BAND_ID_V = new Guid("4523635F-43DC-4D76-901A-505D7A26667E");
        public static Guid BAND_ID_R = new Guid("C7728E02-1FE2-4506-A25E-BBA6D5370BA6");

        private static int SerialisationVersion = 1;

        private string m_SolutionId;
        private string m_Designation;
        private string m_SourceId;
        private float m_RefEpoch;
        private double m_RaDeg;
        private double m_RaErrMas;
        private double m_DeDeg;
        private double m_DeErrMas;
        private double m_ParalaxMas;
        private double m_ParalaxErrMas;
        private double m_PmRaMasYear;
        private double m_PmRaErrMasYear;
        private double m_PmDeMasYear;
        private double m_PmDeErrMasYear;
        private double m_phot_g_mean_mag;
        private double m_bp_rp;

        public GaiaDR2Entry(XmlNode node)
        {
            string[] data = new string[17];
            int idx = 0;
            foreach (XmlNode childNode in node.ChildNodes)
            {
                data[idx] = childNode.InnerXml;
                idx++;
            }

            //  0 <FIELD name="solution_id" ucd="meta.version" datatype="long" />
            //  1 <FIELD name="designation" ucd="meta.id;meta.main" arraysize="4000" datatype="char" />
            //  2 <FIELD name="source_id" ucd="meta.id" datatype="long" />
            //  3 <FIELD name="ref_epoch" unit="Time[Julian Years]" ucd="meta.ref;time.epoch" datatype="double" />
            //  4 <FIELD name="ra" unit="Angle[deg]" ucd="pos.eq.ra;meta.main" datatype="double" />
            //  5 <FIELD name="ra_error" unit="Angle[mas]" ucd="stat.error;pos.eq.ra" datatype="double" />
            //  6 <FIELD name="dec" unit="Angle[deg]" ucd="pos.eq.dec;meta.main" datatype="double" />
            //  7 <FIELD name="dec_error" unit="Angle[mas]" ucd="stat.error;pos.eq.dec" datatype="double" />
            //  8 <FIELD name="parallax" unit="Angle[mas]" ucd="pos.parallax" datatype="double" />
            //  9 <FIELD name="parallax_error" unit="Angle[mas]" ucd="stat.error;pos.parallax" datatype="double" />
            // 10 <FIELD name="pmra" unit="Angular Velocity[mas/year]" ucd="pos.pm;pos.eq.ra" datatype="double" />
            // 11 <FIELD name="pmra_error" unit="Angular Velocity[mas/year]" ucd="stat.error;pos.pm;pos.eq.ra" datatype="double" />
            // 12 <FIELD name="pmdec" unit="Angular Velocity[mas/year]" ucd="pos.pm;pos.eq.dec" datatype="double" />
            // 13 <FIELD name="pmdec_error" unit="Angular Velocity[mas/year]" ucd="stat.error;pos.pm;pos.eq.dec" datatype="double" />
            // 14 <FIELD name="phot_g_mean_mag" unit="Magnitude[mag]" ucd="phot.mag;stat.mean;em.opt" datatype="float" />
            // 15 <FIELD name="bp_rp" unit="Magnitude[mag]" ucd="phot.color" datatype="float" />
            m_SolutionId = data[0];
            m_Designation = data[1];
            if (m_Designation != null)
            {
                m_Designation = m_Designation.Replace("Gaia", "").Replace("DR2", "").Trim();
            }
            m_SourceId = data[2];
            if (!float.TryParse(data[3], NumberStyles.Float, CultureInfo.InvariantCulture, out m_RefEpoch))
            {
                m_RefEpoch = 2015.5f;
            }
            if (!double.TryParse(data[4], NumberStyles.Float, CultureInfo.InvariantCulture, out m_RaDeg))
            {
                m_RaDeg = double.NaN;
            }
            if (!double.TryParse(data[5], NumberStyles.Float, CultureInfo.InvariantCulture, out m_RaErrMas))
            {
                m_RaErrMas = 0;
            }
            if (!double.TryParse(data[6], NumberStyles.Float, CultureInfo.InvariantCulture, out m_DeDeg))
            {
                m_DeDeg = double.NaN;
            }
            if (!double.TryParse(data[7], NumberStyles.Float, CultureInfo.InvariantCulture, out m_DeErrMas))
            {
                m_DeErrMas = 0;
            }

            if (data[8] == "None" ||
                !double.TryParse(data[8], NumberStyles.Float, CultureInfo.InvariantCulture, out m_ParalaxMas))
            {
                m_ParalaxMas = 0;
            }
            if (data[9] == "None"||
                !double.TryParse(data[9], NumberStyles.Float, CultureInfo.InvariantCulture, out m_ParalaxErrMas))
            {
                m_ParalaxErrMas = 0;
            }
            if (data[10] == "None"||
                !double.TryParse(data[10], NumberStyles.Float, CultureInfo.InvariantCulture, out m_PmRaMasYear))
            {
                m_PmRaMasYear = 0;
            }
            if (data[11] == "None"||
                !double.TryParse(data[11], NumberStyles.Float, CultureInfo.InvariantCulture, out m_PmRaErrMasYear))
            {
                m_PmRaErrMasYear = 0;
            }
            if (data[12] == "None"||
                !double.TryParse(data[12], NumberStyles.Float, CultureInfo.InvariantCulture, out m_PmDeMasYear))
            {
                m_PmDeMasYear = 0;
            }
            if (data[13] == "None"||
                !double.TryParse(data[13], NumberStyles.Float, CultureInfo.InvariantCulture, out m_PmDeErrMasYear))
            {
                m_PmDeErrMasYear = 0;
            }
            if (data[14] == "None"||
                !double.TryParse(data[14], NumberStyles.Float, CultureInfo.InvariantCulture, out m_phot_g_mean_mag))
            {
                m_phot_g_mean_mag = double.NaN;
            }
            if (data[15] == "None"||
                !double.TryParse(data[15], NumberStyles.Float, CultureInfo.InvariantCulture, out m_bp_rp))
            {
                m_bp_rp = 0;
            }

            m_MatchedStarDesignation = null;
        }

        public GaiaDR2Entry(GaiaDR2Entry clone, string matchedDesignation)
        {
            m_SolutionId = clone.m_SolutionId;
            m_Designation = clone.m_Designation;
            m_SourceId = clone.m_SourceId;
            m_RefEpoch = clone.m_RefEpoch;
            m_RaDeg = clone.m_RaDeg;
            m_RaErrMas = clone.m_RaErrMas;
            m_DeDeg = clone.m_DeDeg;
            m_DeErrMas = clone.m_DeErrMas;
            m_ParalaxMas = clone.m_ParalaxMas;
            m_ParalaxErrMas = clone.m_ParalaxErrMas;
            m_PmRaMasYear = clone.m_PmRaMasYear;
            m_PmRaErrMasYear = clone.m_PmRaErrMasYear;
            m_PmDeMasYear = clone.m_PmDeMasYear;
            m_PmDeErrMasYear = clone.m_PmDeErrMasYear;
            m_phot_g_mean_mag = clone.m_phot_g_mean_mag;
            m_bp_rp = clone.m_bp_rp;
            m_MatchedStarDesignation = matchedDesignation;
        }

        private string m_MatchedStarDesignation;

        private static double s_targetEpoch;

        public static float TargetEpoch
        {
            set
            {
                s_targetEpoch = value;
            }
        }

        public double RAJ2000
        {
            get
            {
                return m_RaDeg + ((s_targetEpoch - m_RefEpoch) * m_PmRaMasYear / Math.Cos(DEJ2000 * Math.PI / 180)) / 3600000.0;
            }
        }

        public double DEJ2000
        {
            get
            {
                return m_DeDeg + (s_targetEpoch - m_RefEpoch) * m_PmDeMasYear / 3600000.0;
            }
        }

        public ulong StarNo
        {
            get
            {
                return m_SourceId.Length > 19 ? Convert.ToUInt64(m_SourceId.Substring(m_SourceId.Length - 19, 19)) : ulong.Parse(m_SourceId);
            }
        }

        public double RADeg
        {
            get
            {
                return RAJ2000;
            }
        }

        public double DEDeg
        {
            get
            {
                return DEJ2000;
            }
        }

        public double Mag
        {
            get
            {
                return m_phot_g_mean_mag;
            }
        }

        // Magnitude conversions for GaiaDR2 from
        // https://gea.esac.esa.int/archive/documentation/GDR2/Data_processing/chap_cu5pho/sec_cu5pho_calibr/ssec_cu5pho_PhotTransf.html

        public double MagR
        {
            get
            {
                // G − R  = -0.003226 + 0.3833(BP-RP) - 0.1345(BP-RP)*(BP-RP) +/- 0.04840;
                return m_phot_g_mean_mag + 0.003226 - 0.3833 * m_bp_rp + 0.1345* m_bp_rp* m_bp_rp;
            }
        }

        public double MagB
        {
            get
            {
                // G − BT = -0.02441 - 0.4899(BP-RP) - 0.9740(BP-RP)(BP-RP) + 0.2496(BP-RP)(BP-RP)(BP-RP) +/- 0.07293 
                return m_phot_g_mean_mag + 0.02441 + 0.4899 * m_bp_rp + 0.9740 * m_bp_rp * m_bp_rp - 0.2496 * m_bp_rp * m_bp_rp;
            }
        }

        public double MagV
        {
            get
            {
                // G − V  = -0.01760 - 0.006860(BP-RP) - 0.1732(BP-RP)*(BP-RP) +/- 0.04585;
                return m_phot_g_mean_mag + 0.01760 + 0.006860 * m_bp_rp + 0.1732 * m_bp_rp * m_bp_rp;
            }
        }

        public double MagJ
        {
            get
            {
                //G − J = -0.01883 + 1.394(BP-RP) - 0.07893(BP-RP)(BP-RP) +/- 0.05246;
                return m_phot_g_mean_mag + 0.01883 - 1.394 * m_bp_rp + 0.07893 * m_bp_rp * m_bp_rp;
            }
        }

        public double MagK
        {
            get
            {
                //G − KS = -0.1885 + 2.092(BP-RP) - 0.1345(BP-RP)(BP-RP) +/- 0.08281;
                return m_phot_g_mean_mag + 0.1885 + -2.092 * m_bp_rp + 0.1345 * m_bp_rp * m_bp_rp;
            }
        }

        public bool IsForInitialPlateSolve
        {
            get
            {
                // We only use matched star for the initial plate solve
                return m_MatchedStarDesignation != null;
            }
        }

        public string GetStarDesignation(int alternativeId)
        {
            return m_MatchedStarDesignation ?? m_Designation;
        }

        public double GetMagnitudeForBand(Guid magBandId)
        {
            if (magBandId == BAND_ID_V)
                return MagV;
            else if (magBandId == BAND_ID_R)
                return MagR;
            else
                return Mag;
        }

        public void Serialize(BinaryWriter bw)
        {
            bw.Write(SerialisationVersion);

            bw.Write(m_SolutionId);
            bw.Write(m_Designation);
            bw.Write(m_SourceId);
            bw.Write(m_RefEpoch);
            bw.Write(m_RaDeg);
            bw.Write(m_RaErrMas);
            bw.Write(m_DeDeg);
            bw.Write(m_DeErrMas);
            bw.Write(m_ParalaxMas);
            bw.Write(m_ParalaxErrMas);
            bw.Write(m_PmRaMasYear);
            bw.Write(m_PmRaErrMasYear);
            bw.Write(m_PmDeMasYear);
            bw.Write(m_PmDeErrMasYear);
            bw.Write(m_phot_g_mean_mag);
            bw.Write(m_bp_rp);
        }

        public GaiaDR2Entry(BinaryReader br)
        {
            var version = br.ReadInt32();

            m_SolutionId = br.ReadString();
            m_Designation = br.ReadString();
            m_SourceId = br.ReadString();
            m_RefEpoch = br.ReadSingle();
            m_RaDeg = br.ReadDouble();
            m_RaErrMas = br.ReadDouble();
            m_DeDeg = br.ReadDouble();
            m_DeErrMas = br.ReadDouble();
            m_ParalaxMas = br.ReadDouble();
            m_ParalaxErrMas = br.ReadDouble();
            m_PmRaMasYear = br.ReadDouble();
            m_PmRaErrMasYear = br.ReadDouble();
            m_PmDeMasYear = br.ReadDouble();
            m_PmDeErrMasYear = br.ReadDouble();
            m_phot_g_mean_mag = br.ReadDouble();
            m_bp_rp = br.ReadDouble();

            if (version > 1)
            {
                // TODO: Version specific loading, when and if implemented
            }

            m_MatchedStarDesignation = null;
        }
    }
}
