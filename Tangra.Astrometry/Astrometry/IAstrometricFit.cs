/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.Model.Astro;

namespace Tangra.Astrometry
{
	public interface IAstrometricFit
	{
		void GetRADEFromImageCoords(double x, double y, out double RADeg, out double DEDeg);
		void GetImageCoordsFromRADE(double RADeg, double DEDeg, out double x, out double y);
		double GetDistanceInArcSec(double x1, double y1, double x2, double y2);
		double RA0Deg { get; }
		double DE0Deg { get; }
		AstroPlate Image { get; }
	}
}
