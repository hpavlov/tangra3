using System;/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.Astrometry.Analysis
{
	public class ObjectResolverSettings
	{
		public static ObjectResolverSettings Default = new ObjectResolverSettings();

		public double MaxStarMatchDiffAcrSec = 2.0;
		public double MaxStarMatchMagDif = 2.0;
		public int ExcludeEdgeAreaPixels = 7;
		public double MinCertainty = 0.2;
		public int MinAmplitude = 10;
		public int MinDistanceBetweenPeakPixels = 5;
		public double MinFWHM = 2.75;
	}
}
