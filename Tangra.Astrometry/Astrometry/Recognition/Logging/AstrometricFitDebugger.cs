using System;
using System.Collections.Generic;
using System.Linq;
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System.Text;
using Tangra.Model.Config;

namespace Tangra.Astrometry.Recognition.Logging
{
#if ASTROMETRY_DEBUG
	public class AstrometricFitDebugger
	{
		private static object s_SyncLock = new object();

		internal static IAstrometrySettings FitSettings { get; set; }
		internal static double PyramidMinMag { get; set; }
		internal static double PyramidMaxMag { get; set; }
		internal static double AstrometryMinMag { get; set; }
		internal static double AstrometryMaxMag { get; set; }

		private static List<SolutionImprovementEntry> s_ImprovementEntries = new List<SolutionImprovementEntry>();
		private static List<PyramidEntry> s_FailedPyramidEntries = new List<PyramidEntry>();

	    public static List<SolutionImprovementEntry> ImprovementEntries { get { return s_ImprovementEntries; } }
        public static List<PyramidEntry> FailedPyramidEntries { get { return s_FailedPyramidEntries; } }

		private static void ResetInternal()
		{
			s_ImprovementEntries.Clear();
			s_FailedPyramidEntries.Clear();

			FitSettings = null;
		}

		public static void Reset()
		{
			lock (s_SyncLock)
			{
				ResetInternal();
			}
		}

		public static void Init(IAstrometrySettings fitSettings, double pyramidMinMag, double pyramidMaxMag, double astrometryMinMag, double astrometryMaxMag)
		{
			lock (s_SyncLock)
			{
				ResetInternal();

				FitSettings = fitSettings;
				PyramidMinMag = pyramidMinMag;
				PyramidMaxMag = pyramidMaxMag;
				AstrometryMinMag = astrometryMinMag;
				AstrometryMaxMag = astrometryMaxMag;
			}
		}

		public static void RegisterSolutionToImprove(SolutionImprovementEntry improvementEntry)
		{
			lock(s_SyncLock)
			{
				s_ImprovementEntries.Add(improvementEntry);
			}
		}

		public static void RegisterFailedPyramid(PyramidEntry entry)
		{
			lock(s_SyncLock)
			{
				s_FailedPyramidEntries.Add(entry);	
			}
		}
	}
#endif
}
