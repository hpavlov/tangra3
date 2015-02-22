/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.StarCatalogues;

namespace Tangra.Astrometry.Recognition
{
	public class DistanceEntry
	{
		internal double DistanceArcSec;
		internal IStar Star1;
		internal IStar Star2;

		internal DistanceEntry(IStar star1, IStar star2, double distArcSec)
		{
			Star1 = star1;
			Star2 = star2;

			DistanceArcSec = distArcSec;
		}
	}
}
