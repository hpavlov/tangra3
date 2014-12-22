using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Tangra.KweeVanWoerden
{
    public class EclipsingVariable
    {
        public string Designation { get; set; }

        [XmlIgnore]
        public string StandardDesignation
        {
            get
            {
                string[] tokens = Designation.Split(" ".ToCharArray(), 2);
                return string.Concat(tokens[1], " ", tokens[0]);
            }
        }
        public string PeriodType { get; set; }
        public double M0 { get; set; }
        public double Period { get; set; }
        public double MagMin { get; set; }
        public double MagMax { get; set; }
        public string Coordinates { get; set; }
        public int AllReports { get; set; }
        public int PriReports { get; set; }
        public int SecReports { get; set; }
        public double RA { get; set; }
        public double Dec { get; set; }
        public double? GCVS_Epoch { get; set; }
        public double? GCVS_Period { get; set; }
    }

    public class EclipsingVariableCatalogue
    {
        [XmlArray("Stars")]
        public List<EclipsingVariable> Stars = new List<EclipsingVariable>();

        public static EclipsingVariableCatalogue m_Instance = null;
        private static object s_SyncRoot = new object();

        public static EclipsingVariableCatalogue Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    lock (s_SyncRoot)
                    {
                        if (m_Instance == null)
                        {
                            string catalog = AssemblyHelper.GetEmbededResource("Tangra.KweeVanWoerden.Resources", "EclipsingBinaries.xml", Encoding.UTF8);
                            m_Instance = Serialization.Deserialize<EclipsingVariableCatalogue>(catalog);
                        }
                    }
                }

                return m_Instance;
            }
        }
    }
}
