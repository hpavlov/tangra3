/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Tangra.Astrometry.Recognition;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Model.Helpers;
using Tangra.Model.Image;
using Tangra.Model.Numerical;
using Tangra.StarCatalogues;

namespace Tangra.Astrometry
{
	public enum FitOrder
	{
		Linear = 0,
		Quadratic = 1,
		Cubic = 2
	}

	public class PlateConstantsSolver
	{
		private static int MIN_STARS_FOR_CUBIC_FIT = 20000; /* We are not using Cubic fit at all */
		internal static int MIN_STARS_FOR_QUADRATIC_FIT = 16;


		private double m_Tangent_RA0;
		private double m_Tangent_DE0;

		private AstroPlate m_PlateConfig;

		private List<PlateConstStarPair> m_Pairs = new List<PlateConstStarPair>();

		private Dictionary<ulong, bool> m_ExcludedForBadResiduals = new Dictionary<ulong, bool>();
		private Dictionary<ulong, bool> m_IncludedInSolution = new Dictionary<ulong, bool>();

		public int ExcludedForBadResidualsCount
		{
			get { return m_ExcludedForBadResiduals.Count; }
		}

		public int IncludedInSolutionCount
		{
			get { return m_IncludedInSolution.Count; }
		}

		public List<PlateConstStarPair> Pairs
		{
			get { return m_Pairs; }
			set { m_Pairs = value; }
		}

		public PlateConstantsSolver(AstroPlate plateConfig)
		{
			m_PlateConfig = plateConfig;
		}

		public void InitPlateSolution(double RA0Deg, double DE0)
		{
			m_Tangent_RA0 = RA0Deg;
			m_Tangent_DE0 = DE0;

			m_Pairs.Clear();
			m_ExcludedForBadResiduals.Clear();
			m_IncludedInSolution.Clear();
		}

		public void AddStar(ImagePixel plateStar, IStar celestialPyramidStarEntry)
		{
			AddStar(plateStar, celestialPyramidStarEntry, null);
		}

        public void AddStar(ImagePixel plateStar, IStar celestialPyramidStarEntry, int featureId)
        {
            var starPair = AddStar(plateStar, celestialPyramidStarEntry, null);
            starPair.FeatureId = featureId;
        }

        public PlateConstStarPair AddStar(ImagePixel plateStar, IStar celestialPyramidStarEntry, StarMapFeature feature)
		{
			double detectionCertainty = plateStar.SignalNoise;

			PlateConstStarPair starPair =
				AddStar(
					plateStar.XDouble,
					celestialPyramidStarEntry.RADeg,
					plateStar.YDouble,
					celestialPyramidStarEntry.DEDeg,
					celestialPyramidStarEntry.Mag,
					plateStar.Brightness,
					detectionCertainty,
					plateStar.IsSaturated);

#if ASTROMETRY_DEBUG
			Trace.Assert(m_Pairs.Find((pair) => pair.StarNo == celestialPyramidStarEntry.StarNo) == null);
#endif

			starPair.StarNo = celestialPyramidStarEntry.StarNo;
			starPair.FeatureId = feature != null ? feature.FeatureId : -1;
			starPair.RADeg = celestialPyramidStarEntry.RADeg;
			starPair.DEDeg = celestialPyramidStarEntry.DEDeg;

		    return starPair;
		}

		private PlateConstStarPair AddStar(double x, double RADeg, double y, double DE, double mag, int intensity, double detectionCertainty, bool isSaturated)
		{
			double xExpected, yExpected;
			TangentPlane.CelestialToTangent(RADeg, DE, m_Tangent_RA0, m_Tangent_DE0, out xExpected, out yExpected);

			PlateConstStarPair pair = new PlateConstStarPair();
			pair.x = x;
			pair.y = y;
			pair.ExpectedXTang = xExpected;
			pair.ExpectedYTang = yExpected;
			pair.Mag = mag;
			pair.Intensity = (uint)intensity;
			pair.IsSaturated = isSaturated;
			pair.DetectionCertainty = detectionCertainty;

			m_Pairs.Add(pair);

			return pair;
		}

		public LeastSquareFittedAstrometry SolveWithLinearRegression(FitOrder fitOrder, int minNumberOfStars, double maxResidual, out LeastSquareFittedAstrometry firstFit)
		{
			return SolveWithLinearRegression(fitOrder, minNumberOfStars, maxResidual, false, out firstFit);
		}

		public LeastSquareFittedAstrometry SolveWithLinearRegression(IAstrometrySettings settings, out LeastSquareFittedAstrometry firstFit)
		{
            double maxResidual = m_PlateConfig.GetDistanceInArcSec(
                    m_PlateConfig.CenterXImage, m_PlateConfig.CenterYImage,
                    m_PlateConfig.CenterXImage + settings.MaxResidualInPixels, m_PlateConfig.CenterYImage + settings.MaxResidualInPixels);

			if (m_Pairs.Count < CorePyramidConfig.Default.MinStarsForImprovementForThreshold)
			{
				double minResidual = m_PlateConfig.GetDistanceInArcSec(
					m_PlateConfig.CenterXImage, m_PlateConfig.CenterYImage,
					m_PlateConfig.CenterXImage + CorePyramidConfig.Default.MaxResidualThresholdForImprovementInPixels, m_PlateConfig.CenterYImage);

				if (maxResidual < minResidual)
					maxResidual = minResidual;
			}

			if (settings.Method == AstrometricMethod.AutomaticFit)
				return SolveWithLinearRegression(FitOrder.Cubic, settings.MinimumNumberOfStars, maxResidual, true, out firstFit);
			else if (settings.Method == AstrometricMethod.LinearFit)
				return SolveWithLinearRegression(FitOrder.Linear, settings.MinimumNumberOfStars, maxResidual, false, out firstFit);
			else if (settings.Method == AstrometricMethod.QuadraticFit)
				return SolveWithLinearRegression(FitOrder.Quadratic, settings.MinimumNumberOfStars, maxResidual, false, out firstFit);
			else if (settings.Method == AstrometricMethod.CubicFit)
				return SolveWithLinearRegression(FitOrder.Cubic, settings.MinimumNumberOfStars, maxResidual, false, out firstFit);
			else
				throw new NotImplementedException();
		}

		private LeastSquareFittedAstrometry SolveWithLinearRegression(FitOrder fitOrder, int minNumberOfStars, double maxResidual, bool upgradeIfPossible, out LeastSquareFittedAstrometry firstFit)
		{
		    bool failed = false;
			try
			{
				PlateConstantsFit firstPlateConstantsFit = null;

				if (!upgradeIfPossible)
				{
					PlateConstantsFit bestFit = LinearFitWithExcludingResiduals(fitOrder, minNumberOfStars, maxResidual, out firstPlateConstantsFit);

					firstFit = firstPlateConstantsFit != null ? new LeastSquareFittedAstrometry(m_PlateConfig, m_Tangent_RA0, m_Tangent_DE0, firstPlateConstantsFit) : null;

					if (bestFit != null)
						return new LeastSquareFittedAstrometry(m_PlateConfig, m_Tangent_RA0, m_Tangent_DE0, bestFit);
				}
				else
				{
					// less than 11 stars - do a linear fit
					// between 12 and 19 - do a quadratic fit
					// more than 20 - do a cubic fit

					PlateConstantsFit bestFit = null;

					int numStars = m_Pairs.Count;
					if (m_Pairs.Count >= MIN_STARS_FOR_CUBIC_FIT)
					{
						bestFit = LinearFitWithExcludingResiduals(FitOrder.Cubic, minNumberOfStars, maxResidual, out firstPlateConstantsFit);

						if (bestFit != null)
							numStars = bestFit.FitInfo.NumberOfStarsUnsedInSolution();
						else
							numStars = MIN_STARS_FOR_CUBIC_FIT - 1;
					}


					if (numStars >= MIN_STARS_FOR_QUADRATIC_FIT && numStars < MIN_STARS_FOR_CUBIC_FIT)
					{
						bestFit = LinearFitWithExcludingResiduals(FitOrder.Quadratic, minNumberOfStars, maxResidual, out firstPlateConstantsFit);

						if (bestFit != null)
							numStars = bestFit.FitInfo.NumberOfStarsUnsedInSolution();
						else
							numStars = MIN_STARS_FOR_QUADRATIC_FIT - 1;
					}

					if (numStars < MIN_STARS_FOR_QUADRATIC_FIT)
					{
						bestFit = LinearFitWithExcludingResiduals(FitOrder.Linear, minNumberOfStars, maxResidual, out firstPlateConstantsFit);
					}

					firstFit = firstPlateConstantsFit != null ? new LeastSquareFittedAstrometry(m_PlateConfig, m_Tangent_RA0, m_Tangent_DE0, firstPlateConstantsFit) : null;

					if (bestFit != null)
						return new LeastSquareFittedAstrometry(m_PlateConfig, m_Tangent_RA0, m_Tangent_DE0, bestFit);
				}

			    failed = true;
				return null;
			}
			finally
			{
				m_IncludedInSolution.Clear();
				m_ExcludedForBadResiduals.Clear();

				for (int i = 0; i < m_Pairs.Count; i++)
				{
#if ASTROMETRY_DEBUG
					Trace.Assert(!m_IncludedInSolution.ContainsKey(m_Pairs[i].StarNo));
#endif
					m_IncludedInSolution.Add(m_Pairs[i].StarNo, m_Pairs[i].FitInfo.UsedInSolution);

#if ASTROMETRY_DEBUG
					Trace.Assert(!m_ExcludedForBadResiduals.ContainsKey(m_Pairs[i].StarNo));
#endif
					m_ExcludedForBadResiduals.Add(m_Pairs[i].StarNo, m_Pairs[i].FitInfo.ExcludedForHighResidual);
				}

			    if (failed)
			    {
                    if (TangraConfig.Settings.TraceLevels.PlateSolving.TraceInfo())
                        Trace.WriteLine(string.Format("Solution LeastSquareFit failed. {0} included stars, {1} excluded for high residuals.", m_IncludedInSolution.Count, m_ExcludedForBadResiduals.Count));
			    }
			}
		}

		private PlateConstantsFit LinearFitWithExcludingResiduals(FitOrder fitOrder, int minNumStars, double maxResidualLimit, out PlateConstantsFit firstPlateConstantsFit)
		{
			bool hasBadStars;
			PlateConstantsFit fit;
			firstPlateConstantsFit = null;

			for (int i = 0; i < m_Pairs.Count; i++)
				m_Pairs[i].FitInfo.ExcludedForHighResidual = false;

			do
			{
				hasBadStars = false;
				fit = PlateConstantsFit.LeastSquareSolve(m_Pairs, m_Tangent_RA0, m_Tangent_DE0, fitOrder, minNumStars);
				if (firstPlateConstantsFit == null && fit != null) firstPlateConstantsFit = fit;

				int starToExclude = -1;
				double maxResidual = 0;

				for (int i = 0; i < m_Pairs.Count; i++)
				{
					if (!m_Pairs[i].FitInfo.UsedInSolution) continue;

					double residual =
						Math.Sqrt(m_Pairs[i].FitInfo.ResidualRAArcSec * m_Pairs[i].FitInfo.ResidualRAArcSec +
								  m_Pairs[i].FitInfo.ResidualDEArcSec * m_Pairs[i].FitInfo.ResidualDEArcSec);

					if (residual > maxResidual)
					{
						maxResidual = residual;
						starToExclude = i;
					}
				}

				// If any of the residuals are greater than 1px, then remove those stars and compute again
				if (maxResidual > maxResidualLimit &&
					starToExclude != -1)
				{
					m_Pairs[starToExclude].FitInfo.UsedInSolution = false;
					m_Pairs[starToExclude].FitInfo.ExcludedForHighResidual = true;
					hasBadStars = true;
					continue;
				}

			}
			while (hasBadStars && fit != null);

			return fit;
		}

		public bool IsPlateSolvingStar(uint starNo)
		{
			if (m_IncludedInSolution.ContainsKey(starNo))
				return m_IncludedInSolution[starNo];
			else
				return false;
		}

		public bool IsExcludedForHighResidual(uint starNo)
		{
			if (m_ExcludedForBadResiduals.ContainsKey(starNo))
				return m_ExcludedForBadResiduals[starNo];
			else
				return false;
		}

		private FocalLengthFit m_FocalLengthFit;

		public void SetFocalLengthFit(FocalLengthFit fit)
		{
			m_FocalLengthFit = fit;
		}

		public FocalLengthFit ComputeFocalLengthFit()
		{
			if (m_FocalLengthFit != null)
				return m_FocalLengthFit;

			List<DistSolveEntry> entries = new List<DistSolveEntry>();

			for (int i = 0; i < m_Pairs.Count; i++)
			{
				if (!m_Pairs[i].FitInfo.UsedInSolution) continue;
				if (m_Pairs[i].FitInfo.ExcludedForHighResidual) continue;

				for (int j = 0; j < m_Pairs.Count; j++)
				{
					if (i == j) continue;
					if (!m_Pairs[j].FitInfo.UsedInSolution) continue;
					if (m_Pairs[j].FitInfo.ExcludedForHighResidual) continue;

					DistSolveEntry entry = new DistSolveEntry();
					entry.DX = Math.Abs(m_Pairs[i].x - m_Pairs[j].x);
					entry.DY = Math.Abs(m_Pairs[i].y - m_Pairs[j].y);

					entry.StarNo1 = m_Pairs[i].StarNo;
					entry.StarNo2 = m_Pairs[j].StarNo;

					// NOTE: two ways of computing distances - by vx,vy,vz and Elongation()
					//entry.DistRadians = Math.Acos(m_Pairs[i].VX * m_Pairs[j].VX + m_Pairs[i].VY * m_Pairs[j].VY + m_Pairs[i].VZ * m_Pairs[j].VZ);

					double elong = AngleUtility.Elongation(m_Pairs[i].RADeg, m_Pairs[i].DEDeg, m_Pairs[j].RADeg, m_Pairs[j].DEDeg);
					entry.DistRadians = elong * Math.PI / 180.0;

					if (entry.DX == 0 || entry.DY == 0) continue;

					entries.Add(entry);
				}
			}

			SafeMatrix A = new SafeMatrix(entries.Count, 2);
			SafeMatrix X = new SafeMatrix(entries.Count, 1);

			int numStars = 0;
			foreach (DistSolveEntry entry in entries)
			{
				A[numStars, 0] = entry.DX * entry.DX;
				A[numStars, 1] = entry.DY * entry.DY;

				X[numStars, 0] = entry.DistRadians * entry.DistRadians;

				numStars++;
			}

			// Insufficient stars to solve the plate
			if (numStars < 3) return null;

			SafeMatrix a_T = A.Transpose();
			SafeMatrix aa = a_T * A;
			SafeMatrix aa_inv = aa.Inverse();
			SafeMatrix bx = (aa_inv * a_T) * X;

			double a = bx[0, 0];
			double b = bx[1, 0];

			double residualSum = 0;
			int numResiduals = 0;

			foreach (DistSolveEntry entry in entries)
			{
				entry.ResidualRadians = entry.DistRadians - Math.Sqrt(a * entry.DX * entry.DX + b * entry.DY * entry.DY);
				entry.ResidualPercent = entry.ResidualRadians * 100.0 / entry.DistRadians;
				entry.ResidualArcSec = 3600.0 * entry.ResidualRadians * 180.0 / Math.PI;

				numResiduals++;
				residualSum += entry.ResidualRadians * entry.ResidualRadians;
			}

			double variance = Math.Sqrt(residualSum / (numResiduals - 1));

			return new FocalLengthFit(a, b, variance, entries);
		}

		//public double ComputeFocalLengthFit(double effectivePixelWidth, double effectivePixelHeight);
		//{
		//    double dxRad = (x1 - x2) * effectivePixelWidth / (effectiveFocalLength * 1000.0);
		//    double dyRad = (y1 - y2) * effectivePixelHeight / (effectiveFocalLength * 1000.0);
		//    return AngleUtility.Elongation(0, 0, dxRad * 180 / Math.PI, dyRad * 180 / Math.PI) * 3600.0;            
		//}
	}

	public class PlateConstStarPair
	{
		// image/fit data
		internal double ExpectedXTang;
		internal double ExpectedYTang;
		public double x;
		public double y;

		// Star data
		public double Mag;
		public ulong StarNo;
		public int FeatureId;
		public double RADeg;
		public double DEDeg;

		public uint Intensity;
		public bool IsSaturated;
		public double DetectionCertainty;

		public FittedStar FitInfo = new FittedStar();
	}

	public class FittedStar
	{
		public bool UsedInSolution;
		public bool ExcludedForHighResidual;

		public double ResidualXTang;
		public double ResidualYTang;
		//public double ResidualXImg;
		//public double ResidualYImg;
		public double ResidualRAArcSec;
		public double ResidualDEArcSec;
		public double ResidualArcSec
		{
			get { return Math.Sqrt(ResidualRAArcSec * ResidualRAArcSec + ResidualDEArcSec * ResidualDEArcSec);}
		}
	}
}
