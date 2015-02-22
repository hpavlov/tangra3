/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.Model.Helpers;

namespace Tangra.StarCatalogues
{

    internal class SearchZone
    {
        internal double RAFrom;
        internal double RATo;
        internal double DEFrom;
        internal double DETo;

        internal int RADeciHoursFrom
        {
            get { return (int)(RAFrom / 1.5); }
        }

        internal int RADeciHoursTo
        {
            get { return Math.Min((int)(RATo / 1.5) + 1, 240); }
        }
    }

    internal static class CatalogHelper
    {
        internal static List<SearchZone> GetZones(double raDeg, double deDeg, double radiusDeg)
        {
            raDeg = AngleUtility.Range(raDeg, 360);
            List<SearchZone> zones = new List<SearchZone>();

            double deFrom = deDeg - radiusDeg / 2.0;
            double deTo = deDeg + radiusDeg / 2.0; ;
            if (deFrom < -90)
            {
                deTo = deDeg + Math.Max(Math.Abs(deFrom + 90), radiusDeg / 2.0);
                deFrom = -90;
            }
            else if (deTo > 90)
            {
                deFrom = 90 - Math.Max(Math.Abs(deTo - 90), radiusDeg / 2.0);
                deTo = 90;
            }

            double aspect = Math.Cos(Math.Max(Math.Abs(deFrom), Math.Abs(deTo)) * Math.PI / 180);
            double raFrom = raDeg - (radiusDeg / (2.0 * aspect));
            double raTo = raDeg + (radiusDeg / (2.0 *  aspect));

            if (raTo - raFrom > 360)
            {
                raFrom = 0;
                raTo = 360;
            }

            if (raFrom < 0)
            {
                // 2 zones: raFrom..360; 0..raTo
                raFrom = AngleUtility.Range(raFrom, 360);

                zones.Add(new SearchZone { RAFrom = raFrom, RATo = 360, DEFrom = deFrom, DETo = deTo });
                zones.Add(new SearchZone { RAFrom = 0, RATo = raTo, DEFrom = deFrom, DETo = deTo });
            }
            else if (raTo > 360)
            {
                // 2 zones: raFrom..360; 0..raTo
                raTo = AngleUtility.Range(raTo, 360);

                zones.Add(new SearchZone { RAFrom = raFrom, RATo = 360, DEFrom = deFrom, DETo = deTo });
                zones.Add(new SearchZone { RAFrom = 0, RATo = raTo, DEFrom = deFrom, DETo = deTo });
            }
            else
                zones.Add(new SearchZone { RAFrom = raFrom, RATo = raTo, DEFrom = deFrom, DETo = deTo });

            return zones;
        }
    }
}
