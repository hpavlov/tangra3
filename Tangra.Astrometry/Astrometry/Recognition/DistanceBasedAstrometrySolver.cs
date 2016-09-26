/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Tangra.Astrometry.Recognition.Logging;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Model.Helpers;
using Tangra.Model.VideoOperations;
using Tangra.StarCatalogues;

namespace Tangra.Astrometry.Recognition
{
	public enum PerformMatchResult
	{
		FieldAlignmentFailed,
		FitImprovementFailed,
		FitSucceessfull,
		SearchAborted
	}

	public enum PyramidMatchType
	{
		PlateSolve,
		ConfigCalibration
	}

	public class DistanceBasedAstrometrySolver
	{
#if UNIT_TESTS
		internal class SolutionFlags
		{
			internal bool CacheFromLastTimeAttempted;
			internal bool CacheFromLastTimeSuccessful;
			internal bool ManualMatchAttempted;
			internal bool ManualMatchSuccessful;
			internal bool RatioBasedFocalLengthFitAttempted;
			internal bool RatioBasedFocalLengthFitSuccessful;
			internal bool FitWithPreFittedFocalLengthAttempted;
			internal bool FitWithPreFittedFocalLengthSuccessful;
			internal bool FitWithFixedFocalLengthAttempted;
			internal bool FitWithFixedFocalLengthSuccessful;

			public void Reset()
			{
				CacheFromLastTimeAttempted = false;
				CacheFromLastTimeSuccessful = false;
				ManualMatchAttempted = false;
				ManualMatchSuccessful = false;
				RatioBasedFocalLengthFitAttempted = false;
				RatioBasedFocalLengthFitSuccessful = false;
				FitWithPreFittedFocalLengthAttempted = false;
				FitWithPreFittedFocalLengthSuccessful = false;
				FitWithFixedFocalLengthAttempted = false;
				FitWithFixedFocalLengthSuccessful = false;
			}
		}

		internal SolutionFlags Flags = new SolutionFlags();
#endif

		private Stopwatch m_PerformanceWatch = new Stopwatch();

		private AstroPlate m_PlateConfig;
		private IStarMap m_StarMap;
		private List<IStar> m_CelestialStars;
		private LeastSquareFittedAstrometry m_Solution;
		private Dictionary<StarMapFeature, IStar> m_ManualStarMatch;

		private IAstrometrySettings m_FitSettings;

		public DistanceBasedContext Context;

		private bool m_SearchAborted = false;
		private bool m_RatioBasedFittedFocalLengthIsDerived = false;
		private double m_RatioBasedFittedFocalLength;

        private double m_PyramidMinMag = 7.0;
        private double m_PyramidMaxMag = 12.0;
        private double m_AstrometryMinMag = 7.0;
        private double m_AstrometryMaxMag = 16.0;

		private bool m_IsCalibration = false;

        private IOperationNotifier m_OperationNotifier;

		public bool SearchAborted
		{
			get { return m_SearchAborted; }
		}

		private bool m_DetermineAutoLimitMagnitude;

		public DistanceBasedAstrometrySolver(
            IOperationNotifier operationNotifier,
			AstroPlate plateConfig, 
			IAstrometrySettings fitSettings, 
			List<IStar> celestialStars,
			bool determineAutoLimitMagnitude)
		{
			m_PlateConfig = plateConfig;
			m_FitSettings = fitSettings;
			m_CelestialStars = celestialStars;
			m_DetermineAutoLimitMagnitude = determineAutoLimitMagnitude;

            m_OperationNotifier = operationNotifier;

            Context = new DistanceBasedContext(operationNotifier, plateConfig, fitSettings, fitSettings.MaxResidualInPixels, m_AstrometryMinMag, m_AstrometryMaxMag);
		}

		public void InitNewMatch(IStarMap imageFeatures, PyramidMatchType matchType, Dictionary<PSFFit, IStar> manualStars)
		{
			m_StarMap = imageFeatures;

            if (manualStars != null)
                SetManuallyIdentifiedHints(manualStars);

			matchType = PyramidMatchType.PlateSolve;

			m_IsCalibration = matchType == PyramidMatchType.ConfigCalibration;

            Context.Initialize(m_CelestialStars, m_PyramidMinMag, m_PyramidMaxMag, m_IsCalibration, m_DetermineAutoLimitMagnitude, m_ManualStarMatch);

#if ASTROMETRY_DEBUG
			AstrometricFitDebugger.Init(m_FitSettings, m_PyramidMinMag, m_PyramidMaxMag, m_AstrometryMinMag, m_AstrometryMaxMag);
#endif
		}

        public void InitNewFrame(IStarMap imageFeatures)
        {
            m_StarMap = imageFeatures;
			// NOTE: The m_Solution will be reused until the DistanceBasedAstrometrySolver is recreated
        }

		public class PyramidDebugContext
		{
			public Dictionary<int, ulong> ResolvedStars = null;
            public Dictionary<int, ulong> ExcludeStars = null;
			public List<string> ExcludeCombinations = null;
			public double ResolvedFocalLength = double.NaN;
			public double RA0Deg = double.NaN;
			public double DE0Deg = double.NaN;
			public bool SaveSuccessfulAlignImages = false;
		}
		
		public void SetDebugData(PyramidDebugContext debugContext)
		{
			Context.DebugResolvedStars = debugContext.ResolvedStars;
			Context.DebugExcludeStars = debugContext.ExcludeStars;
			Context.DebugResolvedFocalLength = debugContext.ResolvedFocalLength;
			Context.DebugResolvedRA0Deg = debugContext.RA0Deg;
			Context.DebugResolvedDE0Deg = debugContext.DE0Deg;
			Context.DebugSaveAlignImages = debugContext.SaveSuccessfulAlignImages;
			Context.DebugExcludeCombinations = debugContext.ExcludeCombinations;
		}

		public void SetManuallyMatchedPairs(Dictionary<StarMapFeature, IStar> manualFit)
		{
			m_ManualStarMatch = manualFit;
		}

		public void SetMinMaxMagOfStarsForAstrometry(double minMag, double maxMag)
		{
            m_AstrometryMinMag = minMag;
            m_AstrometryMaxMag = maxMag;
		}

        public void SetMinMaxMagOfStarsForPyramidAlignment(double minMag, double maxMag)
        {
            m_PyramidMinMag = minMag;
            m_PyramidMaxMag = maxMag;
        }

		public void SetManuallyIdentifiedHints(Dictionary<PSFFit, IStar> manuallyIdentifiedStars)
		{
			m_ManualStarMatch = new Dictionary<StarMapFeature, IStar>();
			foreach (PSFFit fit in manuallyIdentifiedStars.Keys)
			{
				StarMapFeature feature = m_StarMap.GetFeatureInRadius((int)fit.XCenter, (int)fit.YCenter, 2);
				if (feature != null && !m_ManualStarMatch.ContainsKey(feature))
					m_ManualStarMatch.Add(feature, manuallyIdentifiedStars[fit]);
			}
		}

		public PerformMatchResult PerformMatch(out LeastSquareFittedAstrometry improvedSolution)
		{
			LeastSquareFittedAstrometry distanceBasedSolution;
			PlateConstantsSolver solver;
			return PerformMatch(out distanceBasedSolution, out improvedSolution, out solver);
		}

		public PerformMatchResult PerformMatch(
			out LeastSquareFittedAstrometry distanceBasedSolution,
			out LeastSquareFittedAstrometry improvedSolution, 
			out PlateConstantsSolver solver)
		{
			DistanceBasedContext.FieldAlignmentResult alignmentResult = null;
			improvedSolution = null;

#if UNIT_TESTS
			Flags.Reset();
#endif
			m_PerformanceWatch.Reset();
			m_PerformanceWatch.Start();
			try
			{
				if (!m_IsCalibration &&
				    m_Solution != null)
				{
					// Try using the cache from the last time
					alignmentResult =
						Context.DoFieldAlignment(m_StarMap, m_Solution.FitInfo, m_IsCalibration, false);

					m_Solution = GetSolution(alignmentResult);
#if UNIT_TESTS
					Flags.CacheFromLastTimeAttempted = true;
					Flags.CacheFromLastTimeSuccessful = m_Solution != null;
#endif
				}

				if (m_Solution == null && 
                    !m_IsCalibration &&
				    m_ManualStarMatch != null)
				{
					// Try using the manual star match
					alignmentResult = Context.DoFieldAlignment(m_StarMap, m_ManualStarMatch, m_IsCalibration);

					m_Solution = GetSolution(alignmentResult);
#if UNIT_TESTS
					Flags.ManualMatchAttempted = true;
					Flags.ManualMatchSuccessful = m_Solution != null;
#endif
				}
				
				if (m_Solution == null &&
					!m_RatioBasedFittedFocalLengthIsDerived && 
					!m_FitSettings.PyramidForceFixedFocalLength &&
					!m_IsCalibration)
				{
                    m_OperationNotifier.NotifyBeginLongOperation("Performing a plate solve ...");
					try
					{
						// If this is the first fit, then fit a focal length 
						alignmentResult = Context.DoFieldAlignmentFitFocalLength(m_StarMap);
						m_Solution = GetSolution(alignmentResult);

						if (m_Solution != null)
						{
							m_RatioBasedFittedFocalLengthIsDerived = true;
							m_RatioBasedFittedFocalLength = m_Solution.FitInfo.FittedFocalLength;
						}
						else
						{
                            if (TangraConfig.Settings.TraceLevels.PlateSolving.TraceInfo())
							    Trace.WriteLine("No Solution.");
						}
					}
					finally
					{
                        m_OperationNotifier.NotifyEndLongOperation();
					}
#if UNIT_TESTS
					Flags.RatioBasedFocalLengthFitAttempted = true;
					Flags.RatioBasedFocalLengthFitSuccessful = m_RatioBasedFittedFocalLengthIsDerived;
#endif
				}
				else
				{
					if (m_Solution == null)
					{
						if (m_RatioBasedFittedFocalLengthIsDerived &&
							!m_IsCalibration)
						{
							alignmentResult = Context.DoFieldAlignment(m_StarMap, m_RatioBasedFittedFocalLength);

							m_Solution = GetSolution(alignmentResult);
#if UNIT_TESTS
							Flags.FitWithPreFittedFocalLengthAttempted = true;
							Flags.FitWithPreFittedFocalLengthSuccessful = m_Solution != null;
#endif

						}
						else
						{
							alignmentResult = Context.DoFieldAlignment(m_StarMap, m_IsCalibration);

							m_Solution = GetSolution(alignmentResult);
#if UNIT_TESTS
							Flags.FitWithFixedFocalLengthAttempted = true;
							Flags.FitWithFixedFocalLengthSuccessful = m_Solution != null;
#endif
						}
					}
				}

				m_SearchAborted = Context.m_AbortSearch;

				if (alignmentResult != null)
				{
					distanceBasedSolution = (LeastSquareFittedAstrometry)alignmentResult.Solution;
					improvedSolution = (LeastSquareFittedAstrometry)alignmentResult.ImprovedSolution;
					m_Solution = GetSolution(alignmentResult);					
					solver = alignmentResult.Solver;					
				}
				else
				{
					distanceBasedSolution = m_Solution;
					solver = null;
				}

				if (m_SearchAborted)
					return PerformMatchResult.SearchAborted;

#if ASTROMETRY_DEBUG
				if (m_Solution != null)
					// No need to keep the debug log any longer when the fit is successful
					AstrometricFitDebugger.Reset();
#endif
				if (m_Solution != null) 
					// Clear manually identified starting position in a case of a successful plate solve
					m_ManualStarMatch = null;

				return m_Solution != null
					? PerformMatchResult.FitSucceessfull
					: PerformMatchResult.FitImprovementFailed;

			}
			finally
			{
				// Reset the manually matched stars after every fit attempt
				m_ManualStarMatch = null;

				m_PerformanceWatch.Stop();

				if (m_Solution != null)
                {
                    if (TangraConfig.Settings.TraceLevels.PlateSolving.TraceError())
                    {
                        Trace.WriteLine(string.Format(
                            "Pyramid Match Successful: {0} ms, {1} stars total, aligned on {2} stars, {3} stars matched {4}. Combination: {5}",
                            m_PerformanceWatch.ElapsedMilliseconds, m_CelestialStars.Count,
                            alignmentResult != null ? (alignmentResult.Solution as LeastSquareFittedAstrometry).FitInfo.NumberOfStarsUnsedInSolution() : 0,
                            improvedSolution != null ? improvedSolution.FitInfo.NumberOfStarsUnsedInSolution().ToString() : "N/A",
                            improvedSolution != null ? string.Format(" ({0}-{1} mag)", Context.ImprovedSolutionIncludedMinMag.ToString("0.00"), Context.ImprovedSolutionIncludedMaxMag.ToString("0.00")) : null,
                            alignmentResult != null ? alignmentResult.MatchedTriangle : null));   
                    }
                }
				else
				{
                    if (TangraConfig.Settings.TraceLevels.PlateSolving.TraceError())
					    Trace.WriteLine(string.Format("Pyramid Match Failed: {0} ms, {1} stars total, NO MATCH",
					                                  m_PerformanceWatch.ElapsedMilliseconds, m_CelestialStars.Count));

					if (Context.DebugResolvedStars != null)
					{
						foreach(DistanceBasedContext.DebugTripple tripple in Context.m_DebugTripples)
						{
                            if (TangraConfig.Settings.TraceLevels.PlateSolving.TraceVerbose())
							    Trace.WriteLine(string.Format("DEBUG ALIGN: Missed alignment on {0}-{1}-{2}", tripple.Id1, tripple.Id2, tripple.Id3));
						}
					}
				}
#if ASTROMETRY_DEBUG
				Trace.Assert(!m_DetermineAutoLimitMagnitude || m_Solution == null || m_Solution.FitInfo.DetectedLimitingMagnitude != 0);
#endif
			}
		}

		private LeastSquareFittedAstrometry GetSolution(DistanceBasedContext.FieldAlignmentResult alignmentResult)
		{
			return alignmentResult != null
				? ((LeastSquareFittedAstrometry)alignmentResult.ImprovedSolution ?? (LeastSquareFittedAstrometry)alignmentResult.Solution)
				: null;
		}
	}
}
