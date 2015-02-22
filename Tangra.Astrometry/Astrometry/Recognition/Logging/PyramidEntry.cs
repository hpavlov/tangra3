/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.Model.Image;

namespace Tangra.Astrometry.Recognition.Logging
{
	public enum PyramidEntryFailureReason
	{
		OperationSuccessful,
		TooFiewInitialStars,
		ThreeStarFitFailed,
		SecondLargestResidualIsTooLarge,
		InsufficientStarsForCalibration,
		NotEnoughBrightFeaturesResolvedWhenLessThan7StarsInSolution,
		LinearFitFailed
	}

	public class PyramidEntry
	{
	    public int i;
        public int j;
        public int k;

		public double Xi;
		public double Xj;
		public double Xk;
		public double Yi;
		public double Yj;
		public double Yk;

        public ulong SNi;
        public ulong SNj;
        public ulong SNk;

		public ThreeStarFit CoarseFit;
	    public DirectTransRotAstrometry PreliminaryFit;
		internal LeastSquareFittedAstrometry LinearFit;
		public double FittedFocalLength;

		public int LocatedStars;
		public PyramidEntryFailureReason FailureReason;

		public PyramidEntry(int i, int j, int k, ImagePixel iPixel, ImagePixel jPixel, ImagePixel kPixel, ulong iStarNo, ulong jStarNo, ulong kStarNo)
		{
			Xi = iPixel.XDouble;
			Yi = iPixel.YDouble;
			Xj = jPixel.XDouble;
			Yj = jPixel.YDouble;
			Xk = kPixel.XDouble;
			Yk = kPixel.YDouble;

			SNi = iStarNo;
			SNj = jStarNo;
			SNk = kStarNo;

		    this.i = i;
            this.j = j;
            this.k = k;
		}

		public void FailBecauseOfTooFiewLocatedStars(int locatedStars)
		{
			LocatedStars = locatedStars;
			FailureReason = PyramidEntryFailureReason.TooFiewInitialStars;
		}

        public void FailBecauseOfTooFiewrightFeaturesResolved(int locatedStars)
		{
			LocatedStars = locatedStars;
			FailureReason = PyramidEntryFailureReason.NotEnoughBrightFeaturesResolvedWhenLessThan7StarsInSolution;
		}

		public void RegisterThreeStarFit(ThreeStarFit coarseFit)
		{
			CoarseFit = coarseFit;
		}

        public void RegisterPreliminaryThreeStarFit(DirectTransRotAstrometry solution)
        {
            PreliminaryFit = solution;
        }

		internal void RegisterLinearFit(LeastSquareFittedAstrometry leastSquareFittedAstrometry)
		{
			LinearFit = leastSquareFittedAstrometry;
		}

		public void RegisterFocalLength(double fittedFocalLength)
		{
			FittedFocalLength = fittedFocalLength;
		}
	}
}
