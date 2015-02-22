/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.Astrometry.Recognition.Logging
{
	public enum SolutionImprovementFailureReason
	{
		FailedToImproveSolution,
		LessThanFourStars,
		TooManyUncertainStars,
		TooManyExcludedStars,
		MaxCoreMeanResidualTooHigh
	}

	public class SolutionImprovementEntry
	{
        public int i;
        public int j;
        public int k;

		private SolutionImprovementFailureReason m_Reason;

		internal LeastSquareFittedAstrometry FitToImprove { get; set; }
		internal LeastSquareFittedAstrometry ImprovedSolution { get; set; }

		internal SolutionImprovementFailureReason Reason
		{
			get { return m_Reason; }
		}

        internal SolutionImprovementEntry(LeastSquareFittedAstrometry fit, int i, int j, int k)
		{
			FitToImprove = fit;

            this.i = i;
            this.j = j;
            this.k = k;
		}

		internal void Fail(SolutionImprovementFailureReason reason)
		{
			m_Reason = reason;
		}
	}
}
