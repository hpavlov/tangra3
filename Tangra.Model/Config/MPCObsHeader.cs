using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.Model.Config
{
    public class MPCObsHeader
    {
        public MPCObsHeader(TangraConfig.MPCHeaderSettings basicSettings)
        {
            this.COD = basicSettings.COD;
            this.CON = basicSettings.CON;
            this.CON2 = basicSettings.CON2;
            this.OBS = basicSettings.OBS;
            this.MEA = basicSettings.MEA;
            this.TEL = basicSettings.TEL;
            this.ACK = basicSettings.ACK;
            this.AC2 = basicSettings.AC2;
            this.COM = basicSettings.COM;
        }

        public MPCObsHeader(string[] lines)
        {
            foreach(string line in lines)
            {
                string[] tokens = line.Trim().Split(new char[] {' '}, 2);
                if (tokens.Length == 2)
                {
                    if (tokens[0] == "COD") COD = tokens[1];
                    if (tokens[0] == "CON")
                    {
                        if (CON == null) 
                            CON = tokens[1];
                        else if (CON2 == null) 
                            CON2 = tokens[1];
                    }
                    if (tokens[0] == "OBS") OBS = tokens[1];
                    if (tokens[0] == "MEA") MEA = tokens[1];
                    if (tokens[0] == "TEL") TEL = tokens[1];
                    if (tokens[0] == "ACK") ACK = tokens[1];
                    if (tokens[0] == "AC2") AC2 = tokens[1];
                    if (tokens[0] == "COM") COM = tokens[1];
                    if (tokens[0] == "NET") NET = tokens[1];
                }
            }
        }

        public string COD { get; set; }
        public string CON { get; set; }
        public string CON2 { get; set; }
        public string OBS { get; set; }
        public string MEA { get; set; }
        public string TEL { get; set; }
        public string ACK { get; set; }
        public string AC2 { get; set; }
        public string COM { get; set; }
        public string NET { get; set; }

        public string ToAscii()
        {
            StringBuilder output = new StringBuilder();
            output.AppendLine(string.Format("COD {0}", COD));
            if (!string.IsNullOrEmpty(CON)) output.AppendLine(string.Format("CON {0}", CON));
            if (!string.IsNullOrEmpty(CON2)) output.AppendLine(string.Format("CON {0}", CON2));
            output.AppendLine(string.Format("OBS {0}", OBS));
            if (!string.IsNullOrEmpty(MEA)) output.AppendLine(string.Format("MEA {0}", MEA));
            output.AppendLine(string.Format("TEL {0}", TEL));
            output.AppendLine(string.Format("NET {0}", NET));
            if (!string.IsNullOrEmpty(ACK)) output.AppendLine(string.Format("ACK {0}", ACK));
            if (!string.IsNullOrEmpty(AC2)) output.AppendLine(string.Format("AC2 {0}", AC2));
            if (!string.IsNullOrEmpty(COM)) output.AppendLine(string.Format("COM {0}", COM));

            return output.ToString();
        }
    }
}
