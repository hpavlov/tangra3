/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.Model.Helpers;

namespace Tangra.Astrometry
{
	internal static class TangentPlane
	{
		public static void CelestialToTangent(double raDeg, double deDeg, double ra0Deg, double de0Deg, out double X, out double Y)
		{
			double ra = raDeg * Math.PI / 180;
			double de = deDeg * Math.PI / 180;
			double ra0 = ra0Deg * Math.PI / 180;
			double de0 = de0Deg * Math.PI / 180;

			X = -(Math.Cos(de) * Math.Sin(ra - ra0)) / (Math.Cos(de0) * Math.Cos(de) * Math.Cos(ra - ra0) + Math.Sin(de) * Math.Sin(de0));
			Y = -(Math.Sin(de0) * Math.Cos(de) * Math.Cos(ra - ra0) - Math.Cos(de0) * Math.Sin(de)) / (Math.Cos(de0) * Math.Cos(de) * Math.Cos(ra - ra0) + Math.Sin(de) * Math.Sin(de0));
		}

		public static void TangentToCelestial(double X, double Y, double ra0Deg, double de0Deg, out double raDeg, out double deDeg)
		{
			double ra0 = ra0Deg * Math.PI / 180;
			double de0 = de0Deg * Math.PI / 180;

			double ra = ra0 + Math.Atan(-X / (Math.Cos(de0) - Y * Math.Sin(de0)));
			double de = Math.Asin((Math.Sin(de0) + Y * Math.Cos(de0)) / Math.Sqrt(1 + X * X + Y * Y));

			deDeg = de * 180 / Math.PI;

			ra = AngleUtility.Range(ra, 2 * Math.PI);
			raDeg = ra * 180 / Math.PI;
		}
	}
}
