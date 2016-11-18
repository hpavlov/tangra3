/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Astrometry;
using Tangra.Astrometry.Recognition;
using Tangra.Config;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Model.Image;
using Tangra.Model.VideoOperations;
using Tangra.StarCatalogues;
using Tangra.VideoOperations.Astrometry.Engine;

namespace Tangra.VideoOperations.Astrometry
{
	internal class PlateCalibration
	{
		private PlateConstantsSolver m_ConstantsSolver;
		private IAstrometrySettings m_AstrometrySettings;
		private CalibrationContext m_Context;

		private IAstrometricFit m_SolvedPlate;
		private IAstrometryController m_AstrometryController;

		internal PlateConstantsSolver ConstantsSolver
		{
			get { return m_ConstantsSolver; }
		}

		public PlateCalibration(
			CalibrationContext context,
			IAstrometrySettings astrometrySettings,
			IAstrometryController astrometryController)
		{
			m_Context = context;
			m_AstrometrySettings = astrometrySettings;
			m_AstrometryController = astrometryController;
		}

		public IAstrometricFit PreliminaryFit
		{
			set { m_Context.PreliminaryFit = value; }
		}

		internal void SaveContext(string fileName)
		{
			PlateConstantsSolver oldValue = m_Context.ConstantsSolver;
			try
			{
				m_Context.ConstantsSolver = m_ConstantsSolver;
				byte[] data = m_Context.Serialize();
				System.IO.File.WriteAllBytes(fileName, data);
			}
			finally
			{
				m_Context.ConstantsSolver = oldValue;
			}			
		}

		/// <summary>
		/// Used to calibrate a plate. Does the more precise astrometric fit
		/// Precondition: A coarser (m_PreliminaryFit) is required where the user has visually roughly fitted the stars to the plate 
		/// </summary>
		/// <param name="limitMag">The faintest magnitude of a star to be used for the fit</param>
		/// <param name="fineFit">Used to determine the search area for the stars. When the fit is not a 'fine' fit then 2.5 times
		/// larger are is used for the fit</param>
		public bool SolvePlateConstantsPhase1(double limitMag, bool fineFit)
		{
			int searchDistance = 10;
			IAstrometricFit previousFit;
			if (!fineFit)
			{
                searchDistance = ((int)Math.Ceiling(2.5 * CoreAstrometrySettings.Default.SearchArea) + 1);
				previousFit = m_Context.PreliminaryFit;
			}
			else
			{
                searchDistance = (int)Math.Ceiling(CoreAstrometrySettings.Default.SearchArea) + 1;
				previousFit = m_Context.FirstAstrometricFit;
			}

			m_ConstantsSolver = new PlateConstantsSolver(m_Context.PlateConfig);


			// NOTE: Get the coordinates of the center of the plate.
			double ra0deg, de0Deg;
			previousFit.GetRADEFromImageCoords(
				m_Context.PlateConfig.CenterXImage, m_Context.PlateConfig.CenterYImage, out ra0deg, out de0Deg);

			m_ConstantsSolver.InitPlateSolution(ra0deg, de0Deg);


			#region PASS 1 - Only using the stars down to limitMag

			int idx = -1;
			foreach (IStar star in m_Context.FieldSolveContext.CatalogueStars)
			{
				idx++;

				double x, y;
				previousFit.GetImageCoordsFromRADE(star.RADeg, star.DEDeg, out x, out y);

				if (m_Context.FitExcludeArea.Contains((int)x, (int)y))
					continue;

                if (x < 0 || x > m_Context.PlateConfig.ImageWidth || y < 0 || y > m_Context.PlateConfig.ImageHeight)
                    continue;

				if (limitMag < star.Mag) continue;

				StarMapFeature feature = m_Context.StarMap.GetFeatureInRadius((int)x, (int)y, searchDistance);

                Trace.WriteLine(string.Format("Searching for {0} ({1:0.0} mag) at ({2:0.0},{3:0.0}).{4}Found!", star.GetStarDesignation(0), star.Mag, x, y, feature != null ? "" : "NOT "));

				if (fineFit)
				{
					ImagePixel centroid = m_Context.StarMap.GetCentroid((int)x, (int)y, searchDistance);
					if (centroid != null)
					{
						if (centroid.SignalNoise >= m_AstrometrySettings.DetectionLimit)
						{
							if (!m_Context.FitExcludeArea.Contains(centroid.X, centroid.Y))
							{
								// NOTE: Don't add a star if outside the selected area 
								m_ConstantsSolver.AddStar(centroid, star, feature);
							}
						}
					}
					else
					{
						//Trace.WriteLine("m_SolvePlateConts.NoMatch:" + star.StarNo);
					}
				}
				else if (feature != null)
				{
					ImagePixel plateStar = feature.GetCenter();
					if (!m_Context.FitExcludeArea.Contains(plateStar.X, plateStar.Y))
					{
						// NOTE: Don't add a star if outside the selected area 
						m_ConstantsSolver.AddStar(plateStar, star, feature);
					}
				}
				else
				{
					//Trace.WriteLine("m_SolvePlateConts.NoMatch:" + star.StarNo);
				}
			}

			// TODO: Shouldn't this be comming from the settings??
			return m_ConstantsSolver.Pairs.Count >= 4;

			#endregion
		}

		/// <summary>
		/// Does the second step of the plate calibration: Least Square with the mapped celestrial stars (from step 1)
		/// </summary>
		/// <param name="fitOrder"></param>
		/// <returns></returns>
		public bool SolvePlateConstantsPhase2(FitOrder fitOrder, bool fineFit)
		{
			LeastSquareFittedAstrometry firstSolution;
			m_SolvedPlate = m_ConstantsSolver.SolveWithLinearRegression(m_AstrometrySettings, out firstSolution);

			if (firstSolution != null)
			{
				if (fineFit)
				{
					m_Context.InitialSecondAstrometricFit = LeastSquareFittedAstrometry.FromReflectedObject(firstSolution);
					m_Context.InitialSecondAstrometricFit.FitInfo.AllStarPairs.AddRange(firstSolution.FitInfo.AllStarPairs);
				}
				else
				{
					m_Context.InitialFirstAstrometricFit = LeastSquareFittedAstrometry.FromReflectedObject(firstSolution);
					m_Context.InitialFirstAstrometricFit.FitInfo.AllStarPairs.AddRange(firstSolution.FitInfo.AllStarPairs);
				}				
			}

			if (m_SolvedPlate != null)
			{
				// TODO: Make this configurable
				if (m_ConstantsSolver.ExcludedForBadResidualsCount < 2 * m_ConstantsSolver.IncludedInSolutionCount)
				{
					LeastSquareFittedAstrometry lsfa = m_SolvedPlate as LeastSquareFittedAstrometry;
					// At least 33% of the stars should be included in the solution
					if (fineFit)
						m_Context.SecondAstrometricFit = lsfa;
					else
					{
						m_Context.FirstAstrometricFit = LeastSquareFittedAstrometry.FromReflectedObject(lsfa);
						m_Context.FirstAstrometricFit.FitInfo.AllStarPairs.AddRange(lsfa.FitInfo.AllStarPairs);
					}

#if ASTROMETRY_DEBUG
					//foreach (PlateConstStarPair pair in lsfa.FitInfo.AllStarPairs)
					//{
					//    double x, y;
					//    lsfa.GetImageCoordsFromRADE(pair.RADeg, pair.DEDeg, out x, out y);
					//    double dist = lsfa.GetDistanceInArcSec(x, y, pair.x, pair.y);
					//    Trace.Assert(Math.Abs(dist) < pair.FitInfo.ResidualArcSec*1.1);
					//}
#endif

					return true;
				}
			}

			return false;
		}

		public LeastSquareFittedAstrometry SolvePlateConstantsPhase4(double pyramidLimitMag)
		{
			double ra0deg, de0Deg;
			m_SolvedPlate.GetRADEFromImageCoords(m_SolvedPlate.Image.CenterXImage, m_SolvedPlate.Image.CenterYImage, out ra0deg, out de0Deg);

			FocalLengthFit distFit = m_ConstantsSolver.ComputeFocalLengthFit();
			m_Context.PlateConfig.EffectiveFocalLength = m_Context.FieldSolveContext.FocalLength;
			distFit.GetFocalParameters(m_Context.PlateConfig.EffectiveFocalLength, out m_Context.PlateConfig.EffectivePixelWidth, out m_Context.PlateConfig.EffectivePixelHeight);

			DistanceBasedAstrometrySolver distMatch = new DistanceBasedAstrometrySolver(
				m_AstrometryController,
				m_Context.PlateConfig, 
				m_AstrometrySettings, 
				m_Context.FieldSolveContext.CatalogueStars, 
				m_Context.FieldSolveContext.DetermineAutoLimitMagnitude);
            
            distMatch.SetMinMaxMagOfStarsForAstrometry(7, 18);

			distMatch.SetMinMaxMagOfStarsForPyramidAlignment(7, pyramidLimitMag);
			Trace.WriteLine(string.Format("Stars for alignment in range: 7.0 - {0} mag", pyramidLimitMag.ToString("0.0")));

			distMatch.InitNewMatch(m_Context.StarMap, PyramidMatchType.PlateSolve, null);

#if ASTROMETRY_DEBUG
            Dictionary<int, ulong> debugInfo = new Dictionary<int, ulong>();

			var starList = m_Context.SecondAstrometricFit.FitInfo.AllStarPairs
				.Where(p => !p.FitInfo.ExcludedForHighResidual);

			foreach(var x in starList)
			{
				if (!debugInfo.ContainsKey(x.FeatureId))
					debugInfo.Add(x.FeatureId, x.StarNo);
			}				

			foreach(var f in m_Context.StarMap.Features)
			{
				Trace.WriteLine(string.Format("{0} - {1}", f.FeatureId, debugInfo.ContainsKey(f.FeatureId) ? "INCLUDED" : "MISSING"));
			}

			distMatch.SetDebugData(new DistanceBasedAstrometrySolver.PyramidDebugContext()
			                       	{
										ResolvedStars = debugInfo,
										ResolvedFocalLength = m_Context.SecondAstrometricFit.FitInfo.FittedFocalLength
			                       	});
#endif

			LeastSquareFittedAstrometry fit = null;
			LeastSquareFittedAstrometry improvedFit = null;
			PlateConstantsSolver solver;
			distMatch.PerformMatch(out fit, out improvedFit, out solver);

			m_Context.DistanceBasedFit = fit;
			m_Context.ImprovedDistanceBasedFit = improvedFit;

			if (fit != null)
			{
				m_Context.PlateConstants = new TangraConfig.PersistedPlateConstants
				                           	{
				                           		EffectiveFocalLength = m_Context.PlateConfig.EffectiveFocalLength,
												EffectivePixelWidth = m_Context.PlateConfig.EffectivePixelWidth,
												EffectivePixelHeight = m_Context.PlateConfig.EffectivePixelHeight
				                           	};
		
				m_Context.ConstantsSolver = solver;

				if (improvedFit != null)
				{
					foreach (var pair in improvedFit.FitInfo.AllStarPairs)
					{
						if (pair.FitInfo.ExcludedForHighResidual) continue;

						Trace.WriteLine(string.Format("{6}; {0}; {1}; {2}; {3}; ({4}\", {5}\")",
							pair.RADeg, pair.DEDeg, pair.x.ToString("0.00"), pair.y.ToString("0.00"),
							pair.FitInfo.ResidualRAArcSec.ToString("0.00"), pair.FitInfo.ResidualDEArcSec.ToString("0.00"),
							pair.StarNo));

						double x, y;
						improvedFit.GetImageCoordsFromRADE(pair.RADeg, pair.DEDeg, out x, out y);
						double dist = improvedFit.GetDistanceInArcSec(x, y, pair.x, pair.y);
					}					
				}
			}
			else
			{
				m_Context.PlateConstants = null;
			}

			return fit;
		}
	}
}
