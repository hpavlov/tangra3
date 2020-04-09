using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NUnit.Framework.Constraints;

namespace Tangra.StarCatalogues.GaiaOnline
{
    public class GaiaCatalogue
    {
        private IStarCatalogue localCatalogue;
        private GaiaTapCatalogue gaiaTapCatalogue;

        public GaiaCatalogue(IStarCatalogue localCatalogue, string apiToken)
        {
            this.localCatalogue = localCatalogue;
            gaiaTapCatalogue = new GaiaTapCatalogue(apiToken);
        }

        public List<IStar> GetStarsInRegion(double raDeg, double deDeg, double radiusDeg, double limitMag, float epoch, IWin32Window parentWindow)
        {
            List<IStar> rv = new List<IStar>();

            var localStars = localCatalogue.GetStarsInRegion(raDeg, deDeg, radiusDeg, limitMag, epoch);
            var gaiaStars = gaiaTapCatalogue.GetStarsInRegion(raDeg, deDeg, radiusDeg, limitMag, epoch, parentWindow);

            double starMatchDistanceDeg = 2 / 3600.0; // 2 arcsec

            foreach (var star in gaiaStars)
            {
                var matchingStar = localStars.Where(x => Math.Abs(x.RADeg - star.RADeg) < starMatchDistanceDeg && Math.Abs(x.DEDeg - star.DEDeg) < starMatchDistanceDeg).ToList();
                if (matchingStar.Count == 1)
                {
                    rv.Add(new GaiaDR2Entry((GaiaDR2Entry)star, matchingStar[0].GetStarDesignation(0)));
                }
                else
                {
                    rv.Add(new GaiaDR2Entry((GaiaDR2Entry)star, null));
                }
            }

            return rv;
        }
    }
}
