using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tangra.StarCatalogues;

namespace Tangra.Tests.Calibration
{
    public class TestStar : IStar
    {
        public TestStar(ulong starNo, double raDeg, double deDeg, double mag)
        {
            StarNo = starNo;
            RADeg = raDeg;
            DEDeg = deDeg;
            Mag = mag;
            MagR = mag;
            MagB = mag;
            MagV = mag;
            MagJ = mag;
            MagK = mag;
        }

        public ulong StarNo { get; private set; }

        public double RADeg { get; private set; }

        public double DEDeg { get; private set; }

        public double Mag { get; private set; }

        public double MagR { get; private set; }

        public double MagB { get; private set; }

        public double MagV { get; private set; }

        public double MagJ { get; private set; }

        public double MagK { get; private set; }

        public string GetStarDesignation(int alternativeId)
        {
            return string.Empty;
        }

        public double GetMagnitudeForBand(Guid magBandId)
        {
            return Mag;
        }
    }
}
