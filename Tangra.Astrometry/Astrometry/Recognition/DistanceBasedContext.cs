/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;
using Tangra.Astrometry.Recognition.Logging;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Model.Helpers;
using Tangra.Model.Numerical;
using Tangra.Model.VideoOperations;
using Tangra.Model.Image;
using Tangra.StarCatalogues;

namespace Tangra.Astrometry.Recognition
{
	public class DistanceBasedContext : IDisposable, INotificationReceiver
	{
		private Dictionary<long, double> m_FeaturesDistanceCache = new Dictionary<long, double>();
        private Dictionary<ulong, Dictionary<ulonglong, DistanceEntry>> m_StarsDistanceCache = new Dictionary<ulong, Dictionary<ulonglong, DistanceEntry>>();

		List<double> m_Distances = new List<double>();
		internal List<DistanceEntry> m_Entries = new List<DistanceEntry>();

		internal Dictionary<int, int> m_IndexLower = new Dictionary<int, int>();
		internal Dictionary<int, int> m_IndexUpper = new Dictionary<int, int>();

		private AstroPlate m_PlateConfig;

		private long m_FeatureId_i;
		private long m_FeatureId_j;
		private long m_FeatureId_k;

		private StarMapFeature m_Feature_i;
		private StarMapFeature m_Feature_j;
		private StarMapFeature m_Feature_k;


		//private double m_ToleranceInArcSec;
		//private int m_MinimumNumberOfStars;

		private IAstrometrySettings m_Settings;
		private double m_MinMag;
		private double m_MaxMag;

	    public double ImprovedSolutionIncludedMinMag
	    {
	        get { return m_MinMag; }
	    }

        public double ImprovedSolutionIncludedMaxMag
        {
            get { return m_DetermineAutoLimitMagnitude ? m_DetectedLimitingMagnitude : m_MaxMag; }
        }

		private Dictionary<StarMapFeature, IStar> m_ManualPairs;
		private FitInfo m_PreviousFit;

		private IAstrometricFit m_Solution;
		private PlateConstantsSolver m_SolutionSolver;
		private LeastSquareFittedAstrometry m_ImprovedSolution;
		private LeastSquareFittedAstrometry m_FirstImprovedSolution;

		private string m_MatchedTriangle;

		private Dictionary<ImagePixel, IStar> m_MatchedPairs = new Dictionary<ImagePixel, IStar>();
        private Dictionary<int, ulong> m_MatchedFeatureIdToStarIdIndexes = new Dictionary<int, ulong>();
		private List<ImagePixel> m_AmbiguousMatches = new List<ImagePixel>();
		internal bool m_AbortSearch = false;
		private double m_MaxLeastSquareResidual = 1.0;

#if UNIT_TESTS 
        internal 
#else
        private
#endif
 List<IStar> m_CelestialPyramidStars;

        private double m_RA0Deg;
        private double m_DE0Deg;
		private List<IStar> m_CelestialAllStars;

		private IStarMap m_StarMap;

        private IOperationNotifier m_OperationNotifier;

		public DistanceBasedContext(
            IOperationNotifier operationNotifier,
			AstroPlate plateConfigs,
			IAstrometrySettings settings,
			double maxLeastSquareResidualInPixels,
			double minMag,
			double maxMag)
		{
			m_PlateConfig = plateConfigs;

			if (settings.AlignmentMethod != FieldAlignmentMethod.Pyramid)
				throw new NotSupportedException("Only the Pyramid field alignment method is supported.");

			m_Settings = settings;
			m_MinMag = minMag;
			m_MaxMag = maxMag;

            m_MaxLeastSquareResidual = maxLeastSquareResidualInPixels * Math.Max(plateConfigs.EffectivePixelWidth, plateConfigs.EffectivePixelHeight);
            m_OperationNotifier = operationNotifier;

            m_OperationNotifier.Subscribe(this, typeof(OperationNotifications));
		}

		internal Dictionary<long, double> FeaturesDistanceCache
		{
			get { return m_FeaturesDistanceCache; }
		}

        internal Dictionary<ulong, Dictionary<ulonglong, DistanceEntry>> StarsDistanceCache
		{
			get { return m_StarsDistanceCache; }
		}

		internal List<DistanceEntry> Entries
		{
			get { return m_Entries; }
		}

		internal Dictionary<int, int> IndexLower
		{
			get { return m_IndexLower; }
		}

		internal Dictionary<int, int> IndexUpper
		{
			get { return m_IndexUpper; }
		}
		
		internal Dictionary<int, ulong> MatchedFeatureIdToStarIdIndexes
		{
			get
			{
				return m_MatchedFeatureIdToStarIdIndexes;
			}
		}

		internal Dictionary<ImagePixel, IStar> MatchedPairs
		{
			get { return m_MatchedPairs; }
		}

		public List<IStar> ConsideredStars
		{
			get { return m_CelestialPyramidStars; }
		}

		private List<DistanceEntry> m_DistancesByMagnitude = new List<DistanceEntry>();

		internal PyramidStarsDensityDistributor m_Distributor;

        private void BuildPyramidMatchingByMagnitude()
        {
        	Stopwatch sw = new Stopwatch();
        	sw.Start();
            var pyramidStars = new List<IStar>();
            try
            {
                Dictionary<int, ulong> debugResolvesdStarsWithAppliedExclusions =
            		DebugResolvedStars == null
            			? null
            			: DebugResolvedStars
            			  	.Where(kvp => DebugExcludeStars == null || !DebugExcludeStars.ContainsKey(kvp.Key))
            			  	.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                List<ulong> alwaysIncludeStars = null;
                if (m_ManualPairs != null && m_ManualPairs.Count > 0)
                    alwaysIncludeStars = m_ManualPairs.Values.Select(x => x.StarNo).ToList();

                pyramidStars = m_CelestialPyramidStars.Where(x => x.IsForInitialPlateSolve || (alwaysIncludeStars != null && alwaysIncludeStars.Contains(x.StarNo))).ToList();

                m_Distributor = BuildPyramidMatchingByMagnitude(
                    m_RA0Deg, m_DE0Deg,
                    pyramidStars, 
                    m_PlateConfig,
					m_Settings,
					debugResolvesdStarsWithAppliedExclusions,
                    alwaysIncludeStars,
                    out m_DistancesByMagnitude, 
                    out m_StarsDistanceCache);
            }
			finally
			{
				sw.Stop();

                if (TangraConfig.Settings.TraceLevels.PlateSolving.TraceVerbose())
				    Trace.WriteLine(string.Format(
					    "Building pyramid pairs from a total of {0} stars. Time taken: {1:0}ms. Total of {2} stars for astrometric solution.",
                        pyramidStars.Count, sw.ElapsedMilliseconds, m_CelestialAllStars.Count));

				if (m_Distributor != null)
				{
                    if (TangraConfig.Settings.TraceLevels.PlateSolving.TraceVerbose())
					    Trace.WriteLine(string.Format(
						    "{0} stars used in {1} zones and {2} pairs.",
						    m_StarsDistanceCache.Count,
						    m_Distributor.Areas.Count,
						    m_DistancesByMagnitude.Count));
				}
			}
        }

		private static string SerializeAsBase64String(object graph)
		{
			BinaryFormatter binFmt = new BinaryFormatter();
			using (MemoryStream mem = new MemoryStream())
			{
				binFmt.Serialize(mem, graph);

				mem.Seek(0, SeekOrigin.Begin);

				return Convert.ToBase64String(mem.ToArray());
			}
		}

        public static PyramidStarsDensityDistributor BuildPyramidMatchingByMagnitude(
            double ra0Deg, double de0Deg,
            List<IStar> pyramidStars, 
			AstroPlate image, 
			IAstrometrySettings settings,
            Dictionary<int, ulong> debugResolvedStarsWithAppliedExclusions,
            List<ulong> alwaysIncludeStars,
            out List<DistanceEntry> distancesByMagnitude,
            out Dictionary<ulong, Dictionary<ulonglong, DistanceEntry>> starsDistanceCache)
		{
			// 1) This should be the first N brightest (or all stars) 
			// 2) The memory structure should be a dictionary of Pairs with values all other pairs that include one of the pair
			// 3) The dictionary should be sorted by brightness i.e. brightest pairs should be on the top/ NOTE: Exclude the brightest
			//    stars until the mag difference between the next 2 bright stars becomes less than 1 mag
			// 4) Matching should be done by searching the distance match checking the brightest pairs first. 

            distancesByMagnitude = new List<DistanceEntry>();
            starsDistanceCache = new Dictionary<ulong, Dictionary<ulonglong, DistanceEntry>>();

            if (pyramidStars.Count == 0)
				return null;

            PyramidStarsDensityDistributor distributor;

            pyramidStars.Sort((s1, s2) => s1.Mag.CompareTo(s2.Mag));

            int n = pyramidStars.Count;
            double maxFovInDeg = image.GetMaxFOVInArcSec() / 3600.0;

			distributor = new PyramidStarsDensityDistributor(pyramidStars, image, settings, ra0Deg, de0Deg);
            distributor.DebugResolvedStarsWithAppliedExclusions = debugResolvedStarsWithAppliedExclusions;
            distributor.Initialize(alwaysIncludeStars);

            distancesByMagnitude.Clear();
            starsDistanceCache.Clear();

            List<ulong> resolvedDebugStarsNos = null;
            if (debugResolvedStarsWithAppliedExclusions != null)
            {

				// This is disabled at the moment
                resolvedDebugStarsNos = debugResolvedStarsWithAppliedExclusions.Values.ToList();
                int pyramidStarsLocated = 0;
                for (int i = resolvedDebugStarsNos.Count - 1; i >= 0 ; i--)
                {
                    ulong starNo = resolvedDebugStarsNos[i];

                    IStar star = pyramidStars.FirstOrDefault(s => s.StarNo == starNo);
                    if (star != null)
                        pyramidStarsLocated++;
                    else
                        resolvedDebugStarsNos.Remove(starNo);
                    Trace.Assert(star != null, string.Format("Debug Star {0} not found in the pyramid stars!", starNo));
                }

                if (TangraConfig.Settings.TraceLevels.PlateSolving.TraceVerbose())
                    Trace.WriteLine(
					    string.Format("DEBUG ALIGN: {0} out of {1} Debug Stars found among the pyramid stars ({2})",
					    pyramidStarsLocated, resolvedDebugStarsNos.Count,
					    resolvedDebugStarsNos.Count > 1 
						    ? resolvedDebugStarsNos.Select(s => s.ToString()).Aggregate((a, b) => string.Concat(a, " ", b))
						    : ""));
            }

			// Start building the pairs
			for (int j = 0; j < n; j++)
			{
                IStar jStar = pyramidStars[j];
                if (!distributor.CheckStar(jStar))
                {
                    if (resolvedDebugStarsNos != null)
                    {
                        if (resolvedDebugStarsNos.Contains(jStar.StarNo))
                        {
                            //Trace.Assert(false, "DebugResolved star pair not added to the pyramid areas because the distributor rejected it.");
                        }
                    }
                    continue;
                }

				for (int i = j + 1; i < n; i++)
				{
                    IStar iStar = pyramidStars[i];
						
					double distDeg = AngleUtility.Elongation(iStar.RADeg, iStar.DEDeg, jStar.RADeg, jStar.DEDeg);

					if (distDeg > maxFovInDeg)
					{
                        if (resolvedDebugStarsNos != null)
                        {
                            if (resolvedDebugStarsNos.Contains(iStar.StarNo) &&
                                resolvedDebugStarsNos.Contains(jStar.StarNo))
                            {
                                //Trace.Assert(false, "DebugResolved star pair not added to the pyramid areas because the distance is too large.");
                            }
                        }
					    continue;
					}

				    if (!distributor.CheckStar(iStar))
				    {
                        if (resolvedDebugStarsNos != null)
                        {
                            if (resolvedDebugStarsNos.Contains(iStar.StarNo))
                            {
                                //Trace.Assert(false, string.Format("DebugResolved star {0} not added to the pyramid areas because the distributor rejected it.", iStar.StarNo));
                            }
                        }
				        continue;
				    }

					distributor.MarkStar(iStar);
					distributor.MarkStar(jStar);
						
					DistanceEntry entry = new DistanceEntry(iStar, jStar, distDeg * 3600);
                    distancesByMagnitude.Add(entry);

					#region PerStar distance cache
                    ulonglong id1 = new ulonglong(iStar.StarNo, jStar.StarNo);
                    ulonglong id2 = new ulonglong(jStar.StarNo, iStar.StarNo);

					Dictionary<ulonglong, DistanceEntry> map;
                    if (!starsDistanceCache.TryGetValue(iStar.StarNo, out map))
					{
                        map = new Dictionary<ulonglong, DistanceEntry>();
                        starsDistanceCache.Add(iStar.StarNo, map);
					}
					map.Add(id1, entry);

                    if (!starsDistanceCache.TryGetValue(jStar.StarNo, out map))
					{
                        map = new Dictionary<ulonglong, DistanceEntry>();
                        starsDistanceCache.Add(jStar.StarNo, map);
					}
					map.Add(id2, entry);
					#endregion
				}
			}

			//if (resolvedDebugStarsNos != null)
			//{
			//    foreach(uint starNo in resolvedDebugStarsNos)
			//    {
			//        DensityArea area = distributor.m_Areas.FirstOrDefault(a => a.m_IncludedStarNos.Contains(starNo));
			//        if (area != null)
			//            Trace.WriteLine(string.Format("DEBUG ALIGN: Star {0} located to area [{1:0.00}, {2:0.0}]", starNo, area.XMiddle, area.YMiddle));
			//        Trace.Assert(area != null);
			//    }
			//}

            return distributor;
		}

		internal class DebugTripple
		{
			public int Id1;
			public int Id2;
			public int Id3;
		}

		internal bool DebugSaveAlignImages = false;
		internal List<DebugTripple> m_DebugTripples = null;
        private Dictionary<int, ulong> m_DebugResolvedStars = null;
        internal Dictionary<int, ulong> DebugResolvedStars
		{
			get { return m_DebugResolvedStars; }
			set
			{
				m_DebugResolvedStars = value;
				BuildDebugTripples();
			}
		}

        private Dictionary<int, ulong> m_DebugExcludeStars;
        internal Dictionary<int, ulong> DebugExcludeStars
		{
			get { return m_DebugExcludeStars; }
			set
			{
				m_DebugExcludeStars = value;
				BuildDebugTripples();
			}
		}

		private List<string> m_DebugExcludeCombinations;
		internal List<string> DebugExcludeCombinations
		{
			get { return m_DebugExcludeCombinations; }
			set
			{
				m_DebugExcludeCombinations = value;
				BuildDebugTripples();
			}
		}

		internal double DebugResolvedFocalLength = double.NaN;
		internal double DebugResolvedRA0Deg = double.NaN;
		internal double DebugResolvedDE0Deg = double.NaN;

		private void BuildDebugTripples()
		{
			m_DebugTripples = null;

			if (m_DebugResolvedStars != null)
			{
				m_DebugTripples = new List<DebugTripple>();
				int n = m_DebugResolvedStars.Count;

				int[] featureIds = m_DebugResolvedStars.Keys.ToArray();
				List<int> excludeFeatureIds = m_DebugExcludeStars != null 
					? m_DebugExcludeStars.Keys.ToList()
					: new List<int>();

				List<int> excludeCombinations = new List<int>();

				if (m_DebugExcludeCombinations != null)
				{
					foreach(string combStr in m_DebugExcludeCombinations)
					{
						string[] tokens = combStr.Split('-');
						int a = int.Parse(tokens[0]);
						int b = int.Parse(tokens[1]);
						int c = int.Parse(tokens[2]);

						excludeCombinations.Add(a * 1000000 + b * 1000 + c);
						excludeCombinations.Add(a * 1000000 + c * 1000 + b);
						excludeCombinations.Add(b * 1000000 + a * 1000 + c);
						excludeCombinations.Add(b * 1000000 + c * 1000 + a);
						excludeCombinations.Add(c * 1000000 + a * 1000 + b);
						excludeCombinations.Add(c * 1000000 + b * 1000 + a);
					}
				}

				for (int k = 3; k <= n; k++)
				{
					if (excludeFeatureIds.IndexOf(featureIds[k - 1]) != -1) continue;

					for (int j = 2; j < k; j++)
					{
						if (excludeFeatureIds.IndexOf(featureIds[j - 1]) != -1) continue;

						for (int i = 1; i < j; i++)
						{
							if (excludeFeatureIds.IndexOf(featureIds[i - 1]) != -1) continue;

							if (excludeCombinations.IndexOf(featureIds[i - 1] * 1000000 + featureIds[j - 1] * 1000 + featureIds[k - 1]) != -1) continue;

							m_DebugTripples.Add(
								new DebugTripple()
								{
									Id1 = featureIds[i - 1],
									Id2 = featureIds[j - 1],
									Id3 = featureIds[k - 1]
								});
						}
					}
				}
			}			
		}

		private bool m_DetermineAutoLimitMagnitude;

        private double m_PyramidMinMag;
        private double m_PyramidMaxMag;

		internal void Initialize(
            double ra0Deg, double de0Deg,
			List<IStar> allCelestialStars, 
			double pyramidMinMag, 
			double pyramidMaxMag,
			bool determineAutoLimitMagnitude,
            Dictionary<StarMapFeature, IStar> manualPairs)
		{
#if ASTROMETRY_DEBUG
			Trace.Assert(allCelestialStars != null);
#endif
		    m_PyramidMinMag = pyramidMinMag;
            m_PyramidMaxMag = pyramidMaxMag;
			m_DetermineAutoLimitMagnitude = determineAutoLimitMagnitude;


		    m_RA0Deg = ra0Deg;
		    m_DE0Deg = de0Deg;
			m_CelestialAllStars = allCelestialStars;

		    m_CelestialPyramidStars = m_CelestialAllStars;

            m_ManualPairs = manualPairs;

            InitializePyramidMatching();
		}

        private void InitializePyramidMatching()
		{
			if (CoreAstrometrySettings.Default.UseQuickAlign)
				BuildPyramidMatchingByMagnitude();
			else
				BuildPiramidMatchingUsingOldKVector();

			if (DebugResolvedStars != null)
			{
				int allResolved = m_CelestialAllStars.Count((star) => DebugResolvedStars.ContainsValue(star.StarNo));
				int allPyramidResolved = m_CelestialPyramidStars.Count((star) => DebugResolvedStars.ContainsValue(star.StarNo));
                if (TangraConfig.Settings.TraceLevels.PlateSolving.TraceInfo())
				    Trace.WriteLine(string.Format("Resolved: {2}; All: {3}; Pyramid: {4}; AllResolved:{0}; PyramidResolved:{1}",
					    allResolved, allPyramidResolved, DebugResolvedStars.Count, m_CelestialAllStars.Count, m_CelestialPyramidStars.Count));
			}

			// TODO: After a successful fit, use the LimitReferenceStarDetection to find the faintest detectable star
			//       then using this load the appropriate number of stars from the Catalog and also
			//       
			// TODO: The first plate solve may take longer time if unsuccessful, but then it should be good after that
			//       Need to be able to determine the first plate solve (see how we do consequative fits when Playing)
			//       But the actual faintest detected magnitude should be saved in the Plate Solve context
			//       an could be made available in a loose context (RegisterService)			
		}

		private void BuildPiramidMatchingUsingOldKVector()
		{
			#region The old way of doing it
			// The distance base "Pyramid" matching only uses limited number of stars
			m_CelestialPyramidStars = m_CelestialAllStars.FindAll((star) => star.Mag >= m_PyramidMinMag && star.Mag <= m_PyramidMaxMag);

            if (TangraConfig.Settings.TraceLevels.PlateSolving.TraceVerbose())
			    Trace.WriteLine(string.Format("Building Pyramid Alignment Dataset of {0}/{1} stars in range {2}m - {3}m", m_CelestialPyramidStars.Count, m_CelestialAllStars.Count, m_PyramidMinMag.ToString("0.0"), m_PyramidMaxMag.ToString("0.0")));

			m_Distances.Clear();
			m_Entries.Clear();
			m_StarsDistanceCache.Clear();
			m_IndexLower.Clear();
			m_IndexUpper.Clear();

			// Build a list of distances 
			double maxFovInDeg = m_PlateConfig.GetMaxFOVInArcSec() / 3600.0;
			for (int i = 0; i < m_CelestialPyramidStars.Count; i++)
			{
				IStar star1 = m_CelestialPyramidStars[i];

				for (int j = i + 1; j < m_CelestialPyramidStars.Count; j++)
				{
					IStar star2 = m_CelestialPyramidStars[j];

					double dist = AngleUtility.Elongation(star1.RADeg, star1.DEDeg, star2.RADeg, star2.DEDeg);
					if (dist > maxFovInDeg) continue;

					dist *= 3600.0;

					m_Distances.Add(dist);
					DistanceEntry entry = new DistanceEntry(star1, star2, dist);
					m_Entries.Add(entry);

					#region PerStar distance cache
                    ulonglong id1 = new ulonglong(star1.StarNo, star2.StarNo);
                    ulonglong id2 = new ulonglong(star2.StarNo, star1.StarNo);

					Dictionary<ulonglong, DistanceEntry> map;
					if (!m_StarsDistanceCache.TryGetValue(star1.StarNo, out map))
					{
                        map = new Dictionary<ulonglong, DistanceEntry>();
						m_StarsDistanceCache.Add(star1.StarNo, map);
					}
					map.Add(id1, entry);

					if (!m_StarsDistanceCache.TryGetValue(star2.StarNo, out map))
					{
                        map = new Dictionary<ulonglong, DistanceEntry>();
						m_StarsDistanceCache.Add(star2.StarNo, map);
					}
					map.Add(id2, entry);
					#endregion
				}
			}


			// Build a K-Vector

			double[] distArr = m_Distances.ToArray();
			DistanceEntry[] entryArr = m_Entries.ToArray();
			Array.Sort(distArr, entryArr);

			m_Distances = new List<double>(distArr);
			m_Entries = new List<DistanceEntry>(entryArr);

			//Console.WriteLine(m_Distances.Count.ToString() + " match distances.");
			double errorInArcSec = m_PlateConfig.GetDistanceInArcSec(m_Settings.PyramidDistanceToleranceInPixels);

			errorInArcSec = 0; /* the error will be factored in later on */

			int maxIdx = (int)Math.Round(m_PlateConfig.GetMaxFOVInArcSec() + 2 * errorInArcSec) + 1;
			for (int i = 0; i < maxIdx; i++)
			{
				m_IndexLower.Add(i, -1);
				m_IndexUpper.Add(i, -1);
			}

			int lower = -1;
			int upper = -1;
			for (int i = 0; i < m_Distances.Count; i++)
			{
				int dstL = Math.Max((int)(m_Distances[i] - errorInArcSec), (int)errorInArcSec);

				if (dstL > lower)
				{
					lower = dstL;
					for (int j = lower; j >= 0 && m_IndexLower[j] == -1; j--)
						m_IndexLower[j] = Math.Max(i - 1, 0);
				}

				int dstU = Math.Max((int)Math.Round(m_Distances[i] + errorInArcSec), (int)errorInArcSec);
				if (dstU > upper)
				{
					upper = dstU;
					for (int j = upper; j >= 0 && m_IndexUpper[j] == -1; j--)
						m_IndexUpper[j] = Math.Min(i, m_Distances.Count - 1);
				}
			}

			for (int j = m_IndexLower.Count - 1; j >= 0 && m_IndexLower[j] == -1; j--)
				m_IndexLower[j] = m_Distances.Count - 1;

			for (int j = m_IndexUpper.Count - 1; j >= 0 && m_IndexUpper[j] == -1; j--)
				m_IndexUpper[j] = m_Distances.Count - 1;

			#endregion			
		}

		internal FieldAlignmentResult DoFieldAlignment(IStarMap starMap)
		{
			return DoFieldAlignment(starMap, (FitInfo)null, false);
		}

		internal FieldAlignmentResult DoFieldAlignment(IStarMap starMap, Dictionary<StarMapFeature, IStar> manualPairs)
		{
			m_ManualPairs = manualPairs;
			return DoFieldAlignment(starMap, (FitInfo)null, false);
		}

		internal FieldAlignmentResult DoFieldAlignment(IStarMap starMap, double fittedFocalLength)
		{
			m_PlateConfig.EffectiveFocalLength = fittedFocalLength;
			return DoFieldAlignment(starMap, (FitInfo)null, false);
		}

		internal FieldAlignmentResult DoFieldAlignmentFitFocalLength(IStarMap starMap)
		{
			return DoFieldAlignment(starMap, (FitInfo)null, true);
		}

		internal class FieldAlignmentResult
		{
			public IAstrometricFit Solution;
			public IAstrometricFit ImprovedSolution;
			public PlateConstantsSolver Solver;
			public string MatchedTriangle;
		}

		private double m_DetectedLimitingMagnitude = double.NaN;

		internal FieldAlignmentResult DoFieldAlignment(
			IStarMap starMap, 
			FitInfo previousFit, 
			bool fitFocalLength)
		{
#if ASTROMETRY_DEBUG
			Trace.Assert(m_CelestialPyramidStars != null, "m_CelestialPyramidStars hasn't been set! Initialize() not called or called twice?");
			Trace.Assert(m_Settings.PyramidDistanceToleranceInPixels > 0);
#endif
			double toleranceInArcSec = m_PlateConfig.GetDistanceInArcSec(m_Settings.PyramidDistanceToleranceInPixels);
			m_PreviousFit = previousFit;

			m_FeaturesDistanceCache.Clear();
			m_MatchedPairs.Clear();
			m_AmbiguousMatches.Clear();
			m_MatchedFeatureIdToStarIdIndexes.Clear();
			m_StarMap = starMap;

			if (m_DetermineAutoLimitMagnitude)
			{
#if ASTROMETRY_DEBUG
				Trace.Assert(previousFit == null || previousFit.DetectedLimitingMagnitude != 0);
#endif
				double detectedLimitingMagnitude = double.NaN;
				if (previousFit != null &&
					!double.IsNaN(previousFit.DetectedLimitingMagnitude))
				{
					detectedLimitingMagnitude = previousFit.DetectedLimitingMagnitude;
				}

				if (!(double.IsNaN(detectedLimitingMagnitude) && double.IsNaN(m_DetectedLimitingMagnitude)) &&
					detectedLimitingMagnitude != m_DetectedLimitingMagnitude)
				{
					m_DetectedLimitingMagnitude = detectedLimitingMagnitude;
					if (double.IsNaN(m_DetectedLimitingMagnitude))
					{						
						m_CelestialPyramidStars = m_CelestialAllStars;

                        if (TangraConfig.Settings.TraceLevels.PlateSolving.TraceVerbose())
						    Trace.WriteLine(string.Format("Now using all {0} loaded stars as alignment stars", m_CelestialPyramidStars.Count));
					}
					else
					{
						m_CelestialPyramidStars = m_CelestialAllStars
							.Where(s => s.Mag <= m_DetectedLimitingMagnitude)
							.ToList();

                        if (TangraConfig.Settings.TraceLevels.PlateSolving.TraceVerbose())
						    Trace.WriteLine(string.Format("Limitting alignment stars to {0} stars up to mag {1:0.0}", m_CelestialPyramidStars.Count, m_DetectedLimitingMagnitude));
					}
					InitializePyramidMatching();
				}				
			}

			m_AbortSearch = false;
			try
			{
                m_OperationNotifier.SendNotification(new OperationNotifications(NotificationType.SearchStarted, null));
			
				if (CorePyramidConfig.Default.ForceAlwaysUsePyramidWithRatios ||
					fitFocalLength)
				{

                    if (CoreAstrometrySettings.Default.UseQuickAlign)
                    {
                        if (LoopThroghFeatureTriangles(starMap, toleranceInArcSec, CheckTrianglesWithRatiosByMagnitude))
                        {
                            return new FieldAlignmentResult() { Solution = m_Solution, ImprovedSolution = m_ImprovedSolution, Solver = m_SolutionSolver, MatchedTriangle = m_MatchedTriangle };
                        }                        
                    }
                    else
                    {
                        if (LoopThroghFeatureTriangles(starMap, toleranceInArcSec, CheckTriangleWithRatios))
                        {
                            return new FieldAlignmentResult() { Solution = m_Solution, ImprovedSolution = m_ImprovedSolution, Solver = m_SolutionSolver, MatchedTriangle = m_MatchedTriangle };
                        }                        
                    }

				}
				else
				{
                    if (CoreAstrometrySettings.Default.UseQuickAlign)
                    {
                        if (LoopThroghFeatureTriangles(starMap, toleranceInArcSec, CheckTriangleByMagnitude))
                        {
                            return new FieldAlignmentResult() { Solution = m_Solution, ImprovedSolution = m_ImprovedSolution, Solver = m_SolutionSolver, MatchedTriangle = m_MatchedTriangle };
                        }	
                    }
                    else
                    {
                        if (LoopThroghFeatureTriangles(starMap, toleranceInArcSec, CheckTriangle))
                        {
                            return new FieldAlignmentResult() { Solution = m_Solution, ImprovedSolution = m_ImprovedSolution, Solver = m_SolutionSolver, MatchedTriangle = m_MatchedTriangle };
                        }	                        
                    }
				}

				return null;
			}
			finally
			{
                m_OperationNotifier.SendNotification(new OperationNotifications(NotificationType.SearchFinished, null));
			}
		}

		private bool CheckTriangle(int i, int j, int k, double dij, double dik, double djk, double toleranceInArcSec)
		{
			// Candidates for the matches
			int idxIJLower = m_IndexLower[Math.Min(Math.Max((int)Math.Round(dij - toleranceInArcSec) - 1, 0), m_IndexUpper.Keys.Count - 1)];
			int idxIJUpper = m_IndexUpper[Math.Min(Math.Max((int)Math.Round(dij + toleranceInArcSec) + 1, 0), m_IndexUpper.Keys.Count - 1)];
			int idxIKLower = m_IndexLower[Math.Min(Math.Max((int)Math.Round(dik - toleranceInArcSec) - 1, 0), m_IndexUpper.Keys.Count - 1)];
			int idxIKUpper = m_IndexUpper[Math.Min(Math.Max((int)Math.Round(dik + toleranceInArcSec) + 1, 0), m_IndexUpper.Keys.Count - 1)];
			int idxJKLower = m_IndexLower[Math.Min(Math.Max((int)Math.Round(djk - toleranceInArcSec) - 1, 0), m_IndexUpper.Keys.Count - 1)];
			int idxJKUpper = m_IndexUpper[Math.Min(Math.Max((int)Math.Round(djk + toleranceInArcSec) + 1, 0), m_IndexUpper.Keys.Count - 1)];

			DistanceEntry ijEntry = null;
			DistanceEntry ikEntry = null;
			DistanceEntry jkEntry = null;

			for (int ij = idxIJLower; ij <= idxIJUpper; ij++)
			{
				ijEntry = m_Entries[ij];
				if (ijEntry.DistanceArcSec + toleranceInArcSec < dij) continue;
				if (ijEntry.DistanceArcSec - toleranceInArcSec > dij) continue;

				for (int ik = idxIKLower; ik <= idxIKUpper; ik++)
				{
					ikEntry = m_Entries[ik];
					if (ikEntry.DistanceArcSec + toleranceInArcSec < dik) continue;
					if (ikEntry.DistanceArcSec - toleranceInArcSec > dik) continue;

                    ulong foundIdx0 = uint.MaxValue;
                    ulong needIdx1 = uint.MaxValue;
                    ulong needIdx2 = uint.MaxValue;
					if (ikEntry.Star1.StarNo == ijEntry.Star1.StarNo)
					{
						// ik_1 = ij_1 = (i)
						foundIdx0 = ikEntry.Star1.StarNo;
						needIdx1 = ikEntry.Star2.StarNo;
						needIdx2 = ijEntry.Star2.StarNo;
					}
					else if (ikEntry.Star1.StarNo == ijEntry.Star2.StarNo)
					{
						// ik_1 = ij_2 = (i)
						foundIdx0 = ikEntry.Star1.StarNo;
						needIdx1 = ikEntry.Star2.StarNo; // (k)
						needIdx2 = ijEntry.Star1.StarNo; // (j)
					}
					else if (ikEntry.Star2.StarNo == ijEntry.Star1.StarNo)
					{
						// ik_2 = ij_1 = (i)
						foundIdx0 = ikEntry.Star2.StarNo;
						needIdx1 = ikEntry.Star1.StarNo; // (k)
						needIdx2 = ijEntry.Star2.StarNo; // (j)
					}
					else if (ikEntry.Star2.StarNo == ijEntry.Star2.StarNo)
					{
						// ik_2 = ij_2 = (i)
						foundIdx0 = ikEntry.Star2.StarNo;
						needIdx1 = ikEntry.Star1.StarNo; // (k)
						needIdx2 = ijEntry.Star1.StarNo; // (j)
					}
					else
						continue;

					if (needIdx1 == needIdx2) continue;

#if DEBUG
                                int bestKId = -1;
                                double bestDiff = double.MaxValue;
#endif
					for (int jk = idxJKLower; jk <= idxJKUpper; jk++)
					{
						jkEntry = m_Entries[jk];
#if DEBUG

                                    double diff = Math.Abs(jkEntry.DistanceArcSec - djk);
                                    if (diff < bestDiff)
                                    {
                                        bestDiff = diff;
                                        bestKId = jk;
                                    }
#endif

						if (jkEntry.DistanceArcSec + toleranceInArcSec < djk) continue;
						if (jkEntry.DistanceArcSec - toleranceInArcSec > djk) continue;

						if (jkEntry.Star1.StarNo == needIdx1 &&
							jkEntry.Star2.StarNo == needIdx2)
						{
#if PYRAMID_DEBUG
                                        Trace.WriteLine(
                                            string.Format("Match: {0} - {1} - {2} ({3}-{4}-{5}) {6}\" {7}\" {8}\"",
                                                          foundIdx0, needIdx1, needIdx2, i, j, k, dij.ToString("0.0"),
                                                          dik.ToString("0.0"), djk.ToString("0.0")));
#endif

							m_Solution = IsSuccessfulMatch(m_StarMap, i, j, k, ijEntry, ikEntry, jkEntry,
														   foundIdx0, needIdx1, needIdx2, toleranceInArcSec);
							if (m_Solution != null) return true;
						}

						if (jkEntry.Star1.StarNo == needIdx2 &&
							jkEntry.Star2.StarNo == needIdx1)
						{
#if PYRAMID_DEBUG
                                        Trace.WriteLine(
                                            string.Format("Match: {0} - {1} - {2} ({3}-{4}-{5}) {6}\" {7}\" {8}\"",
                                                          foundIdx0, needIdx1, needIdx2, i, j, k, dij.ToString("0.0"),
                                                          dik.ToString("0.0"), djk.ToString("0.0")));
#endif
							m_Solution = IsSuccessfulMatch(m_StarMap, i, j, k, ijEntry, ikEntry, jkEntry,
														   foundIdx0, needIdx1, needIdx2, toleranceInArcSec);
							if (m_Solution != null) return true;
						}
					}

#if DEBUG
                                jkEntry = m_Entries[bestKId];

                                if (TangraConfig.Settings.TraceLevels.PlateSolving.TraceVerbose())
                                    Debug.WriteLine(
                                        string.Format("3-rd star match failed by {0}\" ({1}, {2}, {3}) -> ({4}: {5}, {6}); [{7},{8}]",
                                        bestDiff, i, j, k,
                                        foundIdx0, needIdx1, needIdx2,
                                        jkEntry.Star1.StarNo, jkEntry.Star2.StarNo));
#endif
				}
			}


			return false;			
		}

		private bool CheckTriangleWithRatios(int i, int j, int k, ImagePixel iCenter, ImagePixel jCenter, ImagePixel kCenter, double toleranceInArcSec)
		{
			double dijMax = m_PlateConfig.GetDistanceInArcSec(iCenter.XDouble, iCenter.YDouble, jCenter.XDouble, jCenter.YDouble, (1 - m_Settings.PyramidFocalLengthAllowance) * m_PlateConfig.EffectiveFocalLength);
            double dijMin = m_PlateConfig.GetDistanceInArcSec(iCenter.XDouble, iCenter.YDouble, jCenter.XDouble, jCenter.YDouble, (1 + m_Settings.PyramidFocalLengthAllowance) * m_PlateConfig.EffectiveFocalLength);
            double dikMax = m_PlateConfig.GetDistanceInArcSec(iCenter.XDouble, iCenter.YDouble, kCenter.XDouble, kCenter.YDouble, (1 - m_Settings.PyramidFocalLengthAllowance) * m_PlateConfig.EffectiveFocalLength);
            double dikMin = m_PlateConfig.GetDistanceInArcSec(iCenter.XDouble, iCenter.YDouble, kCenter.XDouble, kCenter.YDouble, (1 + m_Settings.PyramidFocalLengthAllowance) * m_PlateConfig.EffectiveFocalLength);
            double djkMax = m_PlateConfig.GetDistanceInArcSec(jCenter.XDouble, jCenter.YDouble, kCenter.XDouble, kCenter.YDouble, (1 - m_Settings.PyramidFocalLengthAllowance) * m_PlateConfig.EffectiveFocalLength);
            double djkMin = m_PlateConfig.GetDistanceInArcSec(jCenter.XDouble, jCenter.YDouble, kCenter.XDouble, kCenter.YDouble, (1 + m_Settings.PyramidFocalLengthAllowance) * m_PlateConfig.EffectiveFocalLength);
			
			
			// Candidates for the matches
			int idxIJLower = m_IndexLower[Math.Min(Math.Max((int)Math.Round(dijMin - toleranceInArcSec) - 1, 0), m_IndexUpper.Keys.Count - 1)];
			int idxIJUpper = m_IndexUpper[Math.Min(Math.Max((int)Math.Round(dijMax + toleranceInArcSec) + 1, 0), m_IndexUpper.Keys.Count - 1)];
			int idxIKLower = m_IndexLower[Math.Min(Math.Max((int)Math.Round(dikMin - toleranceInArcSec) - 1, 0), m_IndexUpper.Keys.Count - 1)];
			int idxIKUpper = m_IndexUpper[Math.Min(Math.Max((int)Math.Round(dikMax + toleranceInArcSec) + 1, 0), m_IndexUpper.Keys.Count - 1)];
			int idxJKLower = m_IndexLower[Math.Min(Math.Max((int)Math.Round(djkMin - toleranceInArcSec) - 1, 0), m_IndexUpper.Keys.Count - 1)];
			int idxJKUpper = m_IndexUpper[Math.Min(Math.Max((int)Math.Round(djkMax + toleranceInArcSec) + 1, 0), m_IndexUpper.Keys.Count - 1)];

            ulong iDebugStarNo = 0, jDebugStarNo = 0, kDebugStarNo = 0;
			bool debugStarMatch = false;
			if (DebugResolvedStars != null)
			{
				if (DebugResolvedStars.TryGetValue(i, out iDebugStarNo) &&
					DebugResolvedStars.TryGetValue(j, out jDebugStarNo) &&
					DebugResolvedStars.TryGetValue(k, out kDebugStarNo))
				{
                    IStar iStarAllDbg = m_CelestialAllStars.FirstOrDefault(s => s.StarNo == iDebugStarNo);
                    IStar jStarAllDbg = m_CelestialAllStars.FirstOrDefault(s => s.StarNo == jDebugStarNo);
                    IStar kStarAllDbg = m_CelestialAllStars.FirstOrDefault(s => s.StarNo == kDebugStarNo);

#if ASTROMETRY_DEBUG
                    Trace.Assert(iStarAllDbg != null);
                    Trace.Assert(jStarAllDbg != null);
                    Trace.Assert(kStarAllDbg != null);
#endif

                    IStar iStarDbg = m_CelestialPyramidStars.FirstOrDefault(s => s.StarNo == iDebugStarNo);
                    IStar jStarDbg = m_CelestialPyramidStars.FirstOrDefault(s => s.StarNo == jDebugStarNo);
                    IStar kStarDbg = m_CelestialPyramidStars.FirstOrDefault(s => s.StarNo == kDebugStarNo);

                    if (iStarDbg != null && jStarDbg != null && kStarDbg != null)
                    {
                        debugStarMatch = true;

                        DistanceEntry deIJDbg = m_Entries.FirstOrDefault(e => (e.Star1.StarNo == iDebugStarNo && e.Star2.StarNo == jDebugStarNo) || (e.Star1.StarNo == jDebugStarNo && e.Star2.StarNo == iDebugStarNo));
                        DistanceEntry deIKDbg = m_Entries.FirstOrDefault(e => (e.Star1.StarNo == iDebugStarNo && e.Star2.StarNo == kDebugStarNo) || (e.Star1.StarNo == kDebugStarNo && e.Star2.StarNo == iDebugStarNo));
                        DistanceEntry deJKDbg = m_Entries.FirstOrDefault(e => (e.Star1.StarNo == kDebugStarNo && e.Star2.StarNo == jDebugStarNo) || (e.Star1.StarNo == jDebugStarNo && e.Star2.StarNo == kDebugStarNo));

#if ASTROMETRY_DEBUG
                        Trace.Assert(deIJDbg != null);
                        Trace.Assert(deIKDbg != null);
                        Trace.Assert(deJKDbg != null);
#endif

                        double dijDbg = AngleUtility.Elongation(iStarDbg.RADeg, iStarDbg.DEDeg, jStarDbg.RADeg, jStarDbg.DEDeg) * 3600;
                        double dikDbg = AngleUtility.Elongation(iStarDbg.RADeg, iStarDbg.DEDeg, kStarDbg.RADeg, kStarDbg.DEDeg) * 3600;
                        double djkDbg = AngleUtility.Elongation(kStarDbg.RADeg, kStarDbg.DEDeg, jStarDbg.RADeg, jStarDbg.DEDeg) * 3600;

#if ASTROMETRY_DEBUG
                        Trace.Assert(Math.Abs(dijDbg - deIJDbg.DistanceArcSec) < 1);
                        Trace.Assert(Math.Abs(dikDbg - deIKDbg.DistanceArcSec) < 1);
                        Trace.Assert(Math.Abs(djkDbg - deJKDbg.DistanceArcSec) < 1);
#endif
                        //TODO: Find the real focal length, then find the difference in percentages

                        //NOTE: Stars are not included because initial focal length is not correct !!!
#if ASTROMETRY_DEBUG
                        Trace.Assert(dijMax > dijDbg);
                        Trace.Assert(dijMin < dijDbg);
                        Trace.Assert(dikMax > dikDbg);
                        Trace.Assert(dikMin < dikDbg);
                        Trace.Assert(djkMax > djkDbg);
                        Trace.Assert(djkMin < djkDbg);
#endif                        
                    }
                    else
                    {
#if ASTROMETRY_DEBUG
                        if (iStarDbg == null)
                            Trace.Assert((iStarAllDbg.Mag > m_PyramidMaxMag) || (iStarAllDbg.Mag < m_PyramidMinMag));

                        if (jStarDbg == null)
                            Trace.Assert((jStarAllDbg.Mag > m_PyramidMaxMag) || (jStarAllDbg.Mag < m_PyramidMinMag));

                        if (kStarDbg == null)
                            Trace.Assert((kStarAllDbg.Mag > m_PyramidMaxMag) || (kStarAllDbg.Mag < m_PyramidMinMag));
#endif
                    }
				}
			}

			DistanceEntry ijEntry = null;
			DistanceEntry ikEntry = null;
			DistanceEntry jkEntry = null;

			for (int ij = idxIJLower; ij <= idxIJUpper; ij++)
			{
				ijEntry = m_Entries[ij];

				if (debugStarMatch)
				{
					if ((ijEntry.Star1.StarNo == iDebugStarNo && ijEntry.Star2.StarNo == jDebugStarNo) ||
						(ijEntry.Star1.StarNo == jDebugStarNo && ijEntry.Star2.StarNo == iDebugStarNo))
					{
#if ASTROMETRY_DEBUG
						Trace.Assert(ijEntry.DistanceArcSec + toleranceInArcSec >= dijMin);
						Trace.Assert(ijEntry.DistanceArcSec - toleranceInArcSec <= dijMax);
#endif
					}
				}

				if (ijEntry.DistanceArcSec + toleranceInArcSec < dijMin) continue;
				if (ijEntry.DistanceArcSec - toleranceInArcSec > dijMax) continue;

				for (int ik = idxIKLower; ik <= idxIKUpper; ik++)
				{
					ikEntry = m_Entries[ik];

				    bool debugIKPairFound = false;
					if (debugStarMatch)
					{
						if ((ikEntry.Star1.StarNo == iDebugStarNo && ikEntry.Star2.StarNo == kDebugStarNo) ||
							(ikEntry.Star1.StarNo == kDebugStarNo && ikEntry.Star2.StarNo == iDebugStarNo))
						{
#if ASTROMETRY_DEBUG
							Trace.Assert(ikEntry.DistanceArcSec + toleranceInArcSec >= dikMin);
							Trace.Assert(ikEntry.DistanceArcSec - toleranceInArcSec <= dikMax);
							Trace.Assert(Math.Abs(ijEntry.DistanceArcSec - (dijMin / dikMin) * ikEntry.DistanceArcSec) < toleranceInArcSec);
#endif
						    debugIKPairFound = true;
						}
					}

					if (ikEntry.DistanceArcSec + toleranceInArcSec < dikMin) continue;
					if (ikEntry.DistanceArcSec - toleranceInArcSec > dikMax) continue;

					if (ikEntry.Star1.StarNo != ijEntry.Star1.StarNo &&
						ikEntry.Star1.StarNo != ijEntry.Star2.StarNo &&
						ikEntry.Star2.StarNo != ijEntry.Star1.StarNo &&
						ikEntry.Star2.StarNo != ijEntry.Star2.StarNo)
					{
						// There is no possible (i, j, k) configuration that involves the same 3 stars
						continue;
					}

					double ratioDifference = Math.Abs(ijEntry.DistanceArcSec - (dijMin/dikMin) * ikEntry.DistanceArcSec);
					if (ratioDifference > toleranceInArcSec) continue;
					

					// Ratios are preserved when changing the focal length (with linear fit) so check the ratios here
					double fittedFocalLength = 
						m_PlateConfig.GetFocalLengthFromMatch(
							ijEntry.DistanceArcSec, ikEntry.DistanceArcSec, 
							iCenter, jCenter, kCenter, 
							toleranceInArcSec, m_Settings.PyramidFocalLengthAllowance);

                    if (debugIKPairFound)
					{
#if ASTROMETRY_DEBUG
					    Trace.Assert(!double.IsNaN(fittedFocalLength));
#endif
					}

					if (double.IsNaN(fittedFocalLength)) continue;

					double djk = m_PlateConfig.GetDistanceInArcSec(jCenter.XDouble, jCenter.YDouble, kCenter.XDouble, kCenter.YDouble, fittedFocalLength);

                    ulong iStarNo = uint.MaxValue;
                    ulong needIdx1 = uint.MaxValue;
                    ulong needIdx2 = uint.MaxValue;
					if (ikEntry.Star1.StarNo == ijEntry.Star1.StarNo)
					{
						// ik_1 = ij_1 = (i)
						iStarNo = ikEntry.Star1.StarNo;
						needIdx1 = ikEntry.Star2.StarNo;
						needIdx2 = ijEntry.Star2.StarNo;
					}
					else if (ikEntry.Star1.StarNo == ijEntry.Star2.StarNo)
					{
						// ik_1 = ij_2 = (i)
						iStarNo = ikEntry.Star1.StarNo;
						needIdx1 = ikEntry.Star2.StarNo; // (k)
						needIdx2 = ijEntry.Star1.StarNo; // (j)
					}
					else if (ikEntry.Star2.StarNo == ijEntry.Star1.StarNo)
					{
						// ik_2 = ij_1 = (i)
						iStarNo = ikEntry.Star2.StarNo;
						needIdx1 = ikEntry.Star1.StarNo; // (k)
						needIdx2 = ijEntry.Star2.StarNo; // (j)
					}
					else if (ikEntry.Star2.StarNo == ijEntry.Star2.StarNo)
					{
						// ik_2 = ij_2 = (i)
						iStarNo = ikEntry.Star2.StarNo;
						needIdx1 = ikEntry.Star1.StarNo; // (k)
						needIdx2 = ijEntry.Star1.StarNo; // (j)
					}
					else
						continue;

					if (needIdx1 == needIdx2) continue;

					jkEntry = m_Entries.Where(e => 
						(e.Star1.StarNo == needIdx1 && e.Star2.StarNo == needIdx2) ||
						(e.Star1.StarNo == needIdx2 && e.Star2.StarNo == needIdx1)).FirstOrDefault();

                    if (debugIKPairFound)
                    {
#if ASTROMETRY_DEBUG
                        Trace.Assert(jkEntry != null);
#endif

                        if (jkEntry != null)
                        {
#if ASTROMETRY_DEBUG
                            Trace.Assert(jkEntry.DistanceArcSec + toleranceInArcSec >= djk);
                            Trace.Assert(jkEntry.DistanceArcSec - toleranceInArcSec <= djk);

                            Trace.Assert(jkEntry.DistanceArcSec + toleranceInArcSec >= djkMin);
                            Trace.Assert(jkEntry.DistanceArcSec - toleranceInArcSec <= djkMax);                            
#endif
                        }
                    }

                    if (jkEntry != null)
                    {
                        ulong jStarNo = ulong.MinValue;
                        ulong kStarNo = ulong.MinValue;

                        if (jkEntry.DistanceArcSec + toleranceInArcSec < djkMin) continue;
                        if (jkEntry.DistanceArcSec - toleranceInArcSec > djkMax) continue;

                        if (jkEntry.DistanceArcSec + toleranceInArcSec < djk) continue;
                        if (jkEntry.DistanceArcSec - toleranceInArcSec > djk) continue;

                        if (jkEntry.Star1.StarNo == needIdx1 &&
                            jkEntry.Star2.StarNo == needIdx2)
                        {
							
                            m_Solution = IsSuccessfulMatch(m_StarMap, i, j, k, ijEntry, ikEntry, jkEntry,
														   iStarNo, needIdx1, needIdx2, fittedFocalLength, true, toleranceInArcSec);
                            if (m_Solution == null)
                                continue;

                            jStarNo = needIdx1;
                            kStarNo = needIdx2;
                        }

                        if (jkEntry.Star1.StarNo == needIdx2 &&
                            jkEntry.Star2.StarNo == needIdx1)
                        {
                            m_Solution = IsSuccessfulMatch(m_StarMap, i, j, k, ijEntry, ikEntry, jkEntry,
														   iStarNo, needIdx1, needIdx2, fittedFocalLength, true, toleranceInArcSec);
                            if (m_Solution == null)
                                continue;

                            jStarNo = needIdx2;
                            kStarNo = needIdx1;
                        }

                        if (m_Solution != null)
                        {
                            if (TangraConfig.Settings.Astrometry.PyramidNumberOfPivots == 3)
                                // Exist the initial alignment with only 3 candidate pivots
                                return true;

                            for (int l = 0; l < m_StarMap.Features.Count; l++)
                            {
                                var piramid = m_StarMap.Features[l];
                                if (piramid.FeatureId == i || piramid.FeatureId == j || piramid.FeatureId == k)
                                    continue;

                                var lCenter = piramid.GetCenter();

                                // NOTE: Continue until a set of distances is found in the cache for the 4-th pivot
                                double dli = m_PlateConfig.GetDistanceInArcSec(lCenter.XDouble, lCenter.YDouble, iCenter.XDouble, iCenter.YDouble, fittedFocalLength);
                                double dlj = m_PlateConfig.GetDistanceInArcSec(lCenter.XDouble, lCenter.YDouble, jCenter.XDouble, jCenter.YDouble, fittedFocalLength);
                                double dlk = m_PlateConfig.GetDistanceInArcSec(lCenter.XDouble, lCenter.YDouble, kCenter.XDouble, kCenter.YDouble, fittedFocalLength);

                                List<DistanceEntry> ilCandidates = m_DistancesByMagnitude
                                    .Where(e =>
                                        (e.Star1.StarNo == iStarNo || e.Star2.StarNo == iStarNo) &&
                                        e.DistanceArcSec < dli + toleranceInArcSec && e.DistanceArcSec > dli - toleranceInArcSec)
                                    .ToList();

                                foreach (var cand_il in ilCandidates)
                                {
                                    var lStar = cand_il.Star1.StarNo == iStarNo ? cand_il.Star2 : cand_il.Star1;

                                    List<DistanceEntry> jlCandidates = m_DistancesByMagnitude
                                    .Where(e =>
                                        ((e.Star1.StarNo == jStarNo && e.Star2.StarNo == lStar.StarNo) ||
                                         (e.Star1.StarNo == lStar.StarNo && e.Star2.StarNo == jStarNo)) &&
                                        e.DistanceArcSec < dlj + toleranceInArcSec && e.DistanceArcSec > dlj - toleranceInArcSec)
                                    .ToList();

                                    List<DistanceEntry> klCandidates = m_DistancesByMagnitude
                                        .Where(e =>
                                            ((e.Star1.StarNo == kStarNo && e.Star2.StarNo == lStar.StarNo) ||
                                             (e.Star1.StarNo == lStar.StarNo && e.Star2.StarNo == kStarNo)) &&
                                            e.DistanceArcSec < dlk + toleranceInArcSec && e.DistanceArcSec > dlk - toleranceInArcSec)
                                        .ToList();

                                    if (jlCandidates.Count > 0 && klCandidates.Count > 0)
                                    {
                                        if (klCandidates.Count > 0)
                                            return true;
                                    }
                                }
                            }
                        }
                    }
//#if DEBUG
//                                int bestKId = -1;
//                                double bestDiff = double.MaxValue;
//#endif
//                    for (int jk = idxJKLower; jk <= idxJKUpper; jk++)
//                    {
//                        jkEntry = m_Entries[jk];
//#if DEBUG

//                                    double diff = Math.Abs(jkEntry.DistanceArcSec - djk);
//                                    if (diff < bestDiff)
//                                    {
//                                        bestDiff = diff;
//                                        bestKId = jk;
//                                    }
//#endif


//                    }

//#if DEBUG
//                                jkEntry = m_Entries[bestKId];
//                                Debug.WriteLine(
//                                    string.Format("3-rd star match failed by {0}\" ({1}, {2}, {3}) -> ({4}: {5}, {6}); [{7},{8}]",
//                                    bestDiff, i, j, k,
//                                    foundIdx0, needIdx1, needIdx2,
//                                    jkEntry.Star1.StarNo, jkEntry.Star2.StarNo));
//#endif
				}
			}


			return false;
		}

		private bool CheckTriangleByMagnitude(int i, int j, int k, double dij, double dik, double djk, double toleranceInArcSec)
		{
			List<DistanceEntry> ijCandidates = m_DistancesByMagnitude
                .Where(e => e.DistanceArcSec > dij - toleranceInArcSec && e.DistanceArcSec < dij + toleranceInArcSec).ToList();

            if (m_ManualPairs != null && m_ManualPairs.Count <= 3)
                LimitIJtoManualPairs(ijCandidates);

			foreach (DistanceEntry ijEntry in ijCandidates)
			{
				List<DistanceEntry> ikCandidates = m_DistancesByMagnitude
					.Where(e =>
						(e.Star1.StarNo == ijEntry.Star1.StarNo || e.Star2.StarNo == ijEntry.Star1.StarNo) &&
                         e.DistanceArcSec > dik - toleranceInArcSec && e.DistanceArcSec < dik + toleranceInArcSec)
					.ToList();

				foreach (DistanceEntry ikEntry in ikCandidates)
				{
                    ulong foundIdx0 = uint.MaxValue;
                    ulong needIdx1 = uint.MaxValue;
                    ulong needIdx2 = uint.MaxValue;
					if (ikEntry.Star1.StarNo == ijEntry.Star1.StarNo)
					{
						// ik_1 = ij_1 = (i)
						foundIdx0 = ikEntry.Star1.StarNo;
						needIdx1 = ikEntry.Star2.StarNo;
						needIdx2 = ijEntry.Star2.StarNo;
					}
					else if (ikEntry.Star1.StarNo == ijEntry.Star2.StarNo)
					{
						// ik_1 = ij_2 = (i)
						foundIdx0 = ikEntry.Star1.StarNo;
						needIdx1 = ikEntry.Star2.StarNo; // (k)
						needIdx2 = ijEntry.Star1.StarNo; // (j)
					}
					else if (ikEntry.Star2.StarNo == ijEntry.Star1.StarNo)
					{
						// ik_2 = ij_1 = (i)
						foundIdx0 = ikEntry.Star2.StarNo;
						needIdx1 = ikEntry.Star1.StarNo; // (k)
						needIdx2 = ijEntry.Star2.StarNo; // (j)
					}
					else if (ikEntry.Star2.StarNo == ijEntry.Star2.StarNo)
					{
						// ik_2 = ij_2 = (i)
						foundIdx0 = ikEntry.Star2.StarNo;
						needIdx1 = ikEntry.Star1.StarNo; // (k)
						needIdx2 = ijEntry.Star1.StarNo; // (j)
					}
					else
						continue;

					if (needIdx1 == needIdx2) continue;

					DistanceEntry jkEntry = m_DistancesByMagnitude.Where(e =>
						(e.Star1.StarNo == needIdx1 && e.Star2.StarNo == needIdx2) ||
						(e.Star1.StarNo == needIdx2 && e.Star2.StarNo == needIdx1)).FirstOrDefault();

					if (jkEntry != null)
					{
						if (jkEntry.DistanceArcSec + toleranceInArcSec < djk) continue;
						if (jkEntry.DistanceArcSec - toleranceInArcSec > djk) continue;

						if (jkEntry.Star1.StarNo == needIdx1 &&
							jkEntry.Star2.StarNo == needIdx2)
						{
							m_Solution = IsSuccessfulMatch(m_StarMap, i, j, k, ijEntry, ikEntry, jkEntry,
														   foundIdx0, needIdx1, needIdx2, toleranceInArcSec);
							if (m_Solution != null)
								return true;
						}

						if (jkEntry.Star1.StarNo == needIdx2 &&
							jkEntry.Star2.StarNo == needIdx1)
						{
							m_Solution = IsSuccessfulMatch(m_StarMap, i, j, k, ijEntry, ikEntry, jkEntry,
														   foundIdx0, needIdx1, needIdx2, toleranceInArcSec);
							if (m_Solution != null)
								return true;
						}
					}
				}
			}

			return false;
		}

        private void LimitIKtoManualPairs(List<DistanceEntry> ijCandidates)
        {
            if (m_ManualPairs.Count == 3)
            {
                ulong starNo1 = m_ManualPairs.Values.ToList()[0].StarNo;
                ulong starNo3 = m_ManualPairs.Values.ToList()[2].StarNo;
                ijCandidates.RemoveAll(x => x.Star1.StarNo != starNo1 && x.Star2.StarNo != starNo1);
                ijCandidates.RemoveAll(x => x.Star1.StarNo != starNo3 && x.Star2.StarNo != starNo3);
            }
        }

        private void LimitIJtoManualPairs(List<DistanceEntry> ijCandidates)
	    {
            if (m_ManualPairs.Count == 1)
            {
                ulong starNo = m_ManualPairs.Values.ToList()[0].StarNo;
                ijCandidates.RemoveAll(x => x.Star1.StarNo != starNo && x.Star2.StarNo != starNo);
            }
            else if (m_ManualPairs.Count >= 2)
            {
                ulong starNo1 = m_ManualPairs.Values.ToList()[0].StarNo;
                ulong starNo2 = m_ManualPairs.Values.ToList()[1].StarNo;
                ijCandidates.RemoveAll(x => x.Star1.StarNo != starNo1 && x.Star2.StarNo != starNo1);
                ijCandidates.RemoveAll(x => x.Star1.StarNo != starNo2 && x.Star2.StarNo != starNo2);
            }
	    }
		private bool CheckTrianglesWithRatiosByMagnitude(int i, int j, int k, ImagePixel iCenter, ImagePixel jCenter, ImagePixel kCenter, double toleranceInArcSec)
		{
		    if (iCenter == null || jCenter == null || kCenter == null) return false;

			double dijMax = m_PlateConfig.GetDistanceInArcSec(iCenter.XDouble, iCenter.YDouble, jCenter.XDouble, jCenter.YDouble, (1 - m_Settings.PyramidFocalLengthAllowance) * m_PlateConfig.EffectiveFocalLength);
            double dijMin = m_PlateConfig.GetDistanceInArcSec(iCenter.XDouble, iCenter.YDouble, jCenter.XDouble, jCenter.YDouble, (1 + m_Settings.PyramidFocalLengthAllowance) * m_PlateConfig.EffectiveFocalLength);
            double dikMax = m_PlateConfig.GetDistanceInArcSec(iCenter.XDouble, iCenter.YDouble, kCenter.XDouble, kCenter.YDouble, (1 - m_Settings.PyramidFocalLengthAllowance) * m_PlateConfig.EffectiveFocalLength);
            double dikMin = m_PlateConfig.GetDistanceInArcSec(iCenter.XDouble, iCenter.YDouble, kCenter.XDouble, kCenter.YDouble, (1 + m_Settings.PyramidFocalLengthAllowance) * m_PlateConfig.EffectiveFocalLength);
            double djkMax = m_PlateConfig.GetDistanceInArcSec(jCenter.XDouble, jCenter.YDouble, kCenter.XDouble, kCenter.YDouble, (1 - m_Settings.PyramidFocalLengthAllowance) * m_PlateConfig.EffectiveFocalLength);
            double djkMin = m_PlateConfig.GetDistanceInArcSec(jCenter.XDouble, jCenter.YDouble, kCenter.XDouble, kCenter.YDouble, (1 + m_Settings.PyramidFocalLengthAllowance) * m_PlateConfig.EffectiveFocalLength);
			
			List<DistanceEntry> ijCandidates = m_DistancesByMagnitude
				.Where(e => e.DistanceArcSec > dijMin && e.DistanceArcSec < dijMax).ToList();

		    if (m_ManualPairs != null && m_ManualPairs.Count <= 3) 
                LimitIJtoManualPairs(ijCandidates);

			bool debugFieldIdentified = true;
			bool debug = false;
            ulong debugiStarNo = 0;
            ulong debugjStarNo = 0;
            ulong debugkStarNo = 0;
			if (DebugResolvedStars != null)
			{
				if (DebugResolvedStars.ContainsKey(i) &&
					DebugResolvedStars.ContainsKey(j) &&
					DebugResolvedStars.ContainsKey(k))
				{
					debugiStarNo = DebugResolvedStars[i];
					debugjStarNo = DebugResolvedStars[j];
					debugkStarNo = DebugResolvedStars[k];

					debug = true;
					debugFieldIdentified = false;
				}
			}

			try
			{
				if (debug)
				{
					// The ijCandidates must contain the IJ pair
					DistanceEntry rightijEntry = ijCandidates.FirstOrDefault(c =>
						(c.Star1.StarNo == debugiStarNo && c.Star2.StarNo == debugjStarNo) ||
						(c.Star1.StarNo == debugjStarNo && c.Star2.StarNo == debugiStarNo));

					if(rightijEntry == null)
					{
						DistanceEntry masterijEntry = 
							m_DistancesByMagnitude.FirstOrDefault(c =>
						        (c.Star1.StarNo == debugiStarNo && c.Star2.StarNo == debugjStarNo) ||
						        (c.Star1.StarNo == debugjStarNo && c.Star2.StarNo == debugiStarNo));

						Trace.Assert(masterijEntry != null);
					}
				}

				foreach (DistanceEntry ijEntry in ijCandidates)
				{
					List<DistanceEntry> ikCandidates = m_DistancesByMagnitude
						.Where(e =>
							(e.Star1.StarNo == ijEntry.Star1.StarNo || e.Star2.StarNo == ijEntry.Star1.StarNo ||
							 e.Star1.StarNo == ijEntry.Star2.StarNo || e.Star2.StarNo == ijEntry.Star2.StarNo) &&
							 e.DistanceArcSec > dikMin && e.DistanceArcSec < dikMax)
						.ToList();

                    if (m_ManualPairs != null && m_ManualPairs.Count == 3)
                        LimitIKtoManualPairs(ikCandidates);

					if (debug && (
							(debugiStarNo == ijEntry.Star1.StarNo && debugjStarNo == ijEntry.Star2.StarNo) ||
							(debugiStarNo == ijEntry.Star2.StarNo && debugjStarNo == ijEntry.Star1.StarNo)))
					{
						// The ikCandidates must contain the IK pair
						DistanceEntry rightikEntry = ikCandidates.FirstOrDefault(c =>
							(c.Star1.StarNo == debugkStarNo || c.Star2.StarNo == debugkStarNo));

						if (rightikEntry == null)
						{
							rightikEntry = m_DistancesByMagnitude.FirstOrDefault(c =>
								(c.Star1.StarNo == debugiStarNo && c.Star2.StarNo == debugkStarNo) ||
								(c.Star1.StarNo == debugkStarNo && c.Star2.StarNo == debugiStarNo));

							if (rightikEntry != null)
								Trace.Assert(rightikEntry.DistanceArcSec > dikMin && rightikEntry.DistanceArcSec < dikMax);
							else
								Trace.Assert(false, string.Format("Cannot find the ik pair ({0}, {1}) in the area distances.", debugiStarNo, debugkStarNo));
						}

					}



					foreach (DistanceEntry ikEntry in ikCandidates)
					{
						bool debugTrippleFound = false;
						if (debug && 
							((debugiStarNo == ijEntry.Star1.StarNo && debugjStarNo == ijEntry.Star2.StarNo) ||
							(debugiStarNo == ijEntry.Star2.StarNo && debugjStarNo == ijEntry.Star1.StarNo)) &&
							(debugkStarNo == ikEntry.Star1.StarNo || debugkStarNo == ikEntry.Star2.StarNo))
						{
							debugTrippleFound = true;
						}
					
						// Ratios are preserved when changing the focal length (with linear fit) so check the ratios here
						double fittedFocalLength =
							m_PlateConfig.GetFocalLengthFromMatch(
								ijEntry.DistanceArcSec, ikEntry.DistanceArcSec,
								iCenter, jCenter, kCenter,
								toleranceInArcSec, m_Settings.PyramidFocalLengthAllowance);

						if (double.IsNaN(fittedFocalLength))
						{
							Trace.Assert(!debugTrippleFound);
							continue;
						}

						double djk = m_PlateConfig.GetDistanceInArcSec(jCenter.XDouble, jCenter.YDouble, kCenter.XDouble, kCenter.YDouble, fittedFocalLength);

                        ulong iStarNo = uint.MaxValue;
                        ulong needIdx1 = uint.MaxValue;
                        ulong needIdx2 = uint.MaxValue;

						if (ikEntry.Star1.StarNo == ijEntry.Star1.StarNo)
						{
							// ik_1 = ij_1 = (i)
							iStarNo = ikEntry.Star1.StarNo;
							needIdx1 = ikEntry.Star2.StarNo;
							needIdx2 = ijEntry.Star2.StarNo;
						}
						else if (ikEntry.Star1.StarNo == ijEntry.Star2.StarNo)
						{
							// ik_1 = ij_2 = (i)
							iStarNo = ikEntry.Star1.StarNo;
							needIdx1 = ikEntry.Star2.StarNo; // (k)
							needIdx2 = ijEntry.Star1.StarNo; // (j)
						}
						else if (ikEntry.Star2.StarNo == ijEntry.Star1.StarNo)
						{
							// ik_2 = ij_1 = (i)
							iStarNo = ikEntry.Star2.StarNo;
							needIdx1 = ikEntry.Star1.StarNo; // (k)
							needIdx2 = ijEntry.Star2.StarNo; // (j)
						}
						else if (ikEntry.Star2.StarNo == ijEntry.Star2.StarNo)
						{
							// ik_2 = ij_2 = (i)
							iStarNo = ikEntry.Star2.StarNo;
							needIdx1 = ikEntry.Star1.StarNo; // (k)
							needIdx2 = ijEntry.Star1.StarNo; // (j)
						}
						else
						{
							Trace.Assert(!debugTrippleFound);
							continue;
						}
							

						if (needIdx1 == needIdx2)
						{
							Trace.Assert(!debugTrippleFound);
							continue;
						}

						DistanceEntry jkEntry = m_DistancesByMagnitude.Where(e =>
							(e.Star1.StarNo == needIdx1 && e.Star2.StarNo == needIdx2) ||
							(e.Star1.StarNo == needIdx2 && e.Star2.StarNo == needIdx1)).FirstOrDefault();

						if (jkEntry != null)
						{
                            ulong jStarNo = uint.MaxValue;
                            ulong kStarNo = uint.MaxValue;
                        
							if (jkEntry.DistanceArcSec + toleranceInArcSec < djkMin)
							{
								Trace.Assert(!debugTrippleFound);
								continue;
							}
							if (jkEntry.DistanceArcSec - toleranceInArcSec > djkMax)
							{
								Trace.Assert(!debugTrippleFound);
								continue;
							}

							if (jkEntry.DistanceArcSec + toleranceInArcSec < djk)
							{
								Trace.Assert(!debugTrippleFound);
								continue;
							}
							if (jkEntry.DistanceArcSec - toleranceInArcSec > djk)
							{
								Trace.Assert(!debugTrippleFound);
								continue;
							}

							if (jkEntry.Star1.StarNo == needIdx1 &&
								jkEntry.Star2.StarNo == needIdx2)
							{

								m_Solution = IsSuccessfulMatch(m_StarMap, i, j, k, ijEntry, ikEntry, jkEntry,
															   iStarNo, needIdx1, needIdx2, fittedFocalLength, true, toleranceInArcSec);
							    if (m_Solution != null)
							    {
							        jStarNo = needIdx1;
							        kStarNo = needIdx2;
							        debugFieldIdentified = true;
							    }
							    else
							    {
                                    Trace.Assert(!debugTrippleFound);
                                    continue;
							    }
									
							}

							if (jkEntry.Star1.StarNo == needIdx2 &&
								jkEntry.Star2.StarNo == needIdx1)
							{
								m_Solution = IsSuccessfulMatch(m_StarMap, i, j, k, ijEntry, ikEntry, jkEntry,
															   iStarNo, needIdx1, needIdx2, fittedFocalLength, true, toleranceInArcSec);
							    if (m_Solution != null)
							    {
							        jStarNo = needIdx2;
							        kStarNo = needIdx1;
							        debugFieldIdentified = true;
							    }
							    else
							    {
                                    Trace.Assert(!debugTrippleFound);
							        continue;
							    }
							}

						    if (m_Solution != null)
						    {
						        if (TangraConfig.Settings.Astrometry.PyramidNumberOfPivots == 3)
                                    // Exist the initial alignment with only 3 candidate pivots
						            return true;

                                for (int l = 0; l < m_StarMap.Features.Count; l++)
                                {
                                    var piramid = m_StarMap.Features[l];
                                    if (piramid.FeatureId == i || piramid.FeatureId == j || piramid.FeatureId == k)
                                        continue;

                                    var lCenter = piramid.GetCenter();

                                    // NOTE: Continue until a set of distances is found in the cache for the 4-th pivot
                                    double dli = m_PlateConfig.GetDistanceInArcSec(lCenter.XDouble, lCenter.YDouble, iCenter.XDouble, iCenter.YDouble, fittedFocalLength);
                                    double dlj = m_PlateConfig.GetDistanceInArcSec(lCenter.XDouble, lCenter.YDouble, jCenter.XDouble, jCenter.YDouble, fittedFocalLength);
                                    double dlk = m_PlateConfig.GetDistanceInArcSec(lCenter.XDouble, lCenter.YDouble, kCenter.XDouble, kCenter.YDouble, fittedFocalLength);

                                    List<DistanceEntry> ilCandidates = m_DistancesByMagnitude
						                .Where(e =>
							                (e.Star1.StarNo == iStarNo || e.Star2.StarNo == iStarNo) &&
                                            e.DistanceArcSec < dli + toleranceInArcSec && e.DistanceArcSec > dli - toleranceInArcSec)
						                .ToList();

                                    foreach (var cand_il in ilCandidates)
                                    {
                                        var lStar = cand_il.Star1.StarNo == iStarNo ? cand_il.Star2 : cand_il.Star1;

                                        List<DistanceEntry> jlCandidates = m_DistancesByMagnitude
                                        .Where(e =>
                                            ((e.Star1.StarNo == jStarNo && e.Star2.StarNo == lStar.StarNo) ||
                                             (e.Star1.StarNo == lStar.StarNo && e.Star2.StarNo == jStarNo)) &&
                                            e.DistanceArcSec < dlj + toleranceInArcSec && e.DistanceArcSec > dlj - toleranceInArcSec)
                                        .ToList();

                                        List<DistanceEntry> klCandidates = m_DistancesByMagnitude
                                            .Where(e =>
                                                ((e.Star1.StarNo == kStarNo && e.Star2.StarNo == lStar.StarNo) ||
                                                 (e.Star1.StarNo == lStar.StarNo && e.Star2.StarNo == kStarNo)) &&
                                                e.DistanceArcSec < dlk + toleranceInArcSec && e.DistanceArcSec > dlk - toleranceInArcSec)
                                            .ToList();

                                        if (jlCandidates.Count > 0 && klCandidates.Count > 0)
                                        {
                                            if (klCandidates.Count > 0)
                                                return true;        
                                        }
                                    }                                    
                                }
						    }
						}
					}
				}
			}
			finally
			{
				if (debug && !debugFieldIdentified)
				{
					DebugCheckUnidentifiedStars(
						dijMin, dijMax, dikMin, dikMax, djkMin, djkMax,
						debugiStarNo, debugjStarNo, debugkStarNo, i, j, k);
				}
					
			}

			return false;
		}

		private void DebugCheckUnidentifiedStars(
			double dijMin, double dijMax, double dikMin, double dikMax, double djkMin, double djkMax,
            ulong iStarNo, ulong jStarNo, ulong kStarNo, int i, int j, int k)
		{
			IStar iStar = m_CelestialPyramidStars.FirstOrDefault(s => s.StarNo == iStarNo);
			IStar jStar = m_CelestialPyramidStars.FirstOrDefault(s => s.StarNo == jStarNo);
			IStar kStar = m_CelestialPyramidStars.FirstOrDefault(s => s.StarNo == kStarNo);

			double distDegij = AngleUtility.Elongation(iStar.RADeg, iStar.DEDeg, jStar.RADeg, jStar.DEDeg);
			double distDegik = AngleUtility.Elongation(iStar.RADeg, iStar.DEDeg, kStar.RADeg, kStar.DEDeg);
			double distDegkj = AngleUtility.Elongation(kStar.RADeg, kStar.DEDeg, jStar.RADeg, jStar.DEDeg);

			double maxDist = m_PlateConfig.GetMaxFOVInArcSec() / 3600.0;
			if (distDegij > maxDist || distDegik > maxDist || distDegkj > maxDist)
				// Stars too far away
				return;

            List<ulong> starNos = new List<ulong>(new ulong[] { iStarNo, jStarNo, kStarNo });

			List<DistanceEntry> distEntries = m_DistancesByMagnitude.Where(e => starNos.Contains(e.Star1.StarNo) || starNos.Contains(e.Star2.StarNo)).ToList();

#if ASTROMETRY_DEBUG
			Trace.Assert(distEntries.Count >= 3, "Expected to find 3 distance entries for the stars ");
#endif
			
			double minFL = (1 - m_Settings.PyramidFocalLengthAllowance) * m_PlateConfig.EffectiveFocalLength;
			double maxFL = (1 + m_Settings.PyramidFocalLengthAllowance) * m_PlateConfig.EffectiveFocalLength;

#if ASTROMETRY_DEBUG
			Trace.Assert(DebugResolvedFocalLength >= minFL && DebugResolvedFocalLength <= maxFL,
						 string.Format("Actual focal length {0:0.0} is outside the checked range ({1:0.0}, {2:0.0}) with diviation of {3:0.0}%",
						 DebugResolvedFocalLength, minFL, maxFL, 100 * Math.Abs(m_PlateConfig.EffectiveFocalLength - DebugResolvedFocalLength) / m_PlateConfig.EffectiveFocalLength));
#endif

			DistanceEntry ijDE = distEntries
				.FirstOrDefault(e => (e.Star1.StarNo == iStarNo && e.Star2.StarNo == jStarNo) ||
				            (e.Star1.StarNo == jStarNo && e.Star2.StarNo == iStarNo));

			DistanceEntry ikDE = distEntries
				.FirstOrDefault(e => (e.Star1.StarNo == iStarNo && e.Star2.StarNo == kStarNo) ||
							(e.Star1.StarNo == kStarNo && e.Star2.StarNo == iStarNo));

			DistanceEntry jkDE = distEntries
				.FirstOrDefault(e => (e.Star1.StarNo == kStarNo && e.Star2.StarNo == jStarNo) ||
							(e.Star1.StarNo == jStarNo && e.Star2.StarNo == kStarNo));

            if (ijDE != null && ikDE != null && jkDE != null)
            {
                // ijDE -> dijMax
                // ikDE -> dikMax
                // jkDE -> djkMax
                if (ijDE.DistanceArcSec > dijMin && ijDE.DistanceArcSec > dijMax)
                {

#if ASTROMETRY_DEBUG
                    Trace.Assert(ijDE.DistanceArcSec > dijMin && ijDE.DistanceArcSec > dijMax,
                                 string.Format(
                                     "The expected distance for {0}-{1} is ({2:0.0}, {3:0.0}) but distance of {4:0.0} was found",
                                     i, j, dijMin, dijMax, ijDE.DistanceArcSec));

                    Trace.Assert(ikDE.DistanceArcSec > dikMin && ikDE.DistanceArcSec > dikMax,
                                 string.Format(
                                     "The expected distance for {0}-{1} is ({2:0.0}, {3:0.0}) but distance of {4:0.0} was found",
                                     i, k, dikMin, dikMax, ikDE.DistanceArcSec));

                    Trace.Assert(jkDE.DistanceArcSec > djkMin && jkDE.DistanceArcSec > djkMax,
                                 string.Format(
                                     "The expected distance for {0}-{1} is ({2:0.0}, {3:0.0}) but distance of {4:0.0} was found",
                                     j, k, djkMin, djkMax, jkDE.DistanceArcSec));

                    Trace.Assert(false);
#endif
                }
            }
            else
            {
#if ASTROMETRY_DEBUG
                Trace.Assert(false, "Could not find one or more of the distances");
#endif
            }
		}

		private ImagePixel GetCenterOfFeature(StarMapFeature feature, IStarMap starMap)
		{
			ImagePixel center = feature.GetCenter();
            if (center == null)
				return ImagePixel.Unspecified;

            return starMap.GetCentroid(center.X, center.Y, (int)CoreAstrometrySettings.Default.SearchArea);
		}

		

		internal IAstrometricFit IsSuccessfulMatch(
			IStarMap starMap,
			int i, int j, int k,
			DistanceEntry ijEntry, DistanceEntry ikEntry, DistanceEntry jkEntry,
            ulong iStarNo, ulong starNo2, ulong starNo3, double toleranceInArcSec)
		{
			return IsSuccessfulMatch(
				starMap, i, j, k, 
				ijEntry, ikEntry, jkEntry, 
				iStarNo, starNo2, starNo3,
				m_PlateConfig.EffectiveFocalLength, false, toleranceInArcSec);
		}


		internal IAstrometricFit IsSuccessfulMatch(
			IStarMap starMap,
			int i, int j, int k,
			DistanceEntry ijEntry, DistanceEntry ikEntry, DistanceEntry jkEntry,
            ulong iStarNo, ulong starNo2, ulong starNo3, double fittedFocalLength, bool isRatioFittedFocalLength, double toleranceInArcSec)
		{
			i--;
			j--;
			k--;
			ImagePixel feature_i = GetCenterOfFeature(starMap.GetFeatureById(i), starMap);
			ImagePixel feature_j = GetCenterOfFeature(starMap.GetFeatureById(j), starMap);
			ImagePixel feature_k = GetCenterOfFeature(starMap.GetFeatureById(k), starMap);

			#region find the numbers of the three stars: i, j, k
#if ASTROMETRY_DEBUG
			Trace.Assert(ijEntry.Star1.StarNo == iStarNo || ijEntry.Star2.StarNo == iStarNo);
#endif

            ulong jStarNo, kStarNo;
			if (ijEntry.Star1.StarNo == iStarNo)
			{
				jStarNo = ijEntry.Star2.StarNo;
				if (ijEntry.Star2.StarNo == starNo2) kStarNo = starNo3; else kStarNo = starNo2;
			}
			else
			{
				jStarNo = ijEntry.Star1.StarNo;
				if (ijEntry.Star1.StarNo == starNo2) kStarNo = starNo3; else kStarNo = starNo2;
			}

#if ASTROMETRY_DEBUG
			Trace.Assert(ikEntry.Star1.StarNo == kStarNo || ikEntry.Star2.StarNo == kStarNo);
			Trace.Assert(jkEntry.Star1.StarNo == kStarNo || jkEntry.Star2.StarNo == kStarNo);
#endif

			#endregion

			//if (DebugResolvedStars != null)
			//{
			//    uint ii = 0, jj = 0, kk = 0;
			//    if (DebugResolvedStars.TryGetValue(i + 1, out ii) &&
			//        DebugResolvedStars.TryGetValue(j + 1, out jj) &&
			//        DebugResolvedStars.TryGetValue(k + 1, out kk))
			//    {
			//        if (ii == iStarNo && jj == starNo2 && kk == starNo3)
			//        {
			//            Debugger.Break();
			//        }
			//        else
			//            Trace.WriteLine(string.Format("PYRAMID: {0} = {1}, {2} = {3}, {4} = {5}",
			//                                        i, i == ii ? "YES" : "NO"
			//                                      , j, j == jj ? "YES" : "NO"
			//                                      , k, k == kk ? "YES" : "NO"));
			//    }
			//    else
			//        Trace.WriteLine(string.Format("PYRAMID: {0} = {1}, {2} = {3}, {4} = {5}",
			//                                        i, i == ii ? "YES" : "MISSING"
			//                                      , j, j == jj ? "YES" : "MISSING"
			//                                      , k, k == kk ? "YES" : "MISSING"));
			//}

#if ASTROMETRY_DEBUG
            PyramidEntry pyramidLog = new PyramidEntry(i, j, k, feature_i, feature_j, feature_k, iStarNo, jStarNo, kStarNo);
#endif
            //// Note this is actually cheap way to confirm whether the 3 stars are good or not.
            //List<IStar> threeStars = m_CelestialAllStars.FindAll(s => s.StarNo == iStarNo || s.StarNo == jStarNo || s.StarNo == kStarNo);
            //if (threeStars.Count == 3)
            //{
            //    Dictionary<AstroPixel, IStar> threeStarDict = new Dictionary<AstroPixel, IStar>();

            //    IStar stari = threeStars.Find(s => s.StarNo == iStarNo);
            //    threeStarDict.Add(feature_i, stari);

            //    IStar starj = threeStars.Find(s => s.StarNo == jStarNo);
            //    threeStarDict.Add(feature_j, starj);

            //    IStar stark = threeStars.Find(s => s.StarNo == kStarNo);
            //    threeStarDict.Add(feature_k, stark);

            //    DirectTransRotAstrometry solution = DirectTransRotAstrometry.SolveByThreeStars(m_PlateConfig, threeStarDict);
            //    if (solution != null)
            //    {
            //        pyramidLog.RegisterPreliminaryThreeStarFit(solution);
            //    }
            //}

		    int locatedStars = 3;
			m_MatchedPairs.Clear();
			m_AmbiguousMatches.Clear();
			m_MatchedFeatureIdToStarIdIndexes.Clear();

            List<ulong> usedPyramidAngles = new List<ulong>();

			foreach (StarMapFeature feature in starMap.Features)
			{
				if (feature.FeatureId == i) continue;
				if (feature.FeatureId == j) continue;
				if (feature.FeatureId == k) continue;

				long idx_ix = ((long)i << 32) + (long)feature.FeatureId;

				double dist_ix;
				ImagePixel feature_x = GetCenterOfFeature(feature, starMap);

				if (m_MatchedPairs.ContainsKey(feature_x)) continue;

				if (isRatioFittedFocalLength || !m_FeaturesDistanceCache.TryGetValue(idx_ix, out dist_ix))
				{
					dist_ix = m_PlateConfig.GetDistanceInArcSec(feature_i.X, feature_i.Y, feature_x.X, feature_x.Y, fittedFocalLength);
					long idx_xi = ((long)feature.FeatureId << 32) + (long)i;

					if (!isRatioFittedFocalLength)
					{
						m_FeaturesDistanceCache.Add(idx_ix, dist_ix);
						m_FeaturesDistanceCache.Add(idx_xi, dist_ix);
					}
				}

                 
                Dictionary<ulonglong, DistanceEntry> iStarDists = m_StarsDistanceCache[iStarNo];
                foreach (ulonglong key in iStarDists.Keys)
				{
					// We have found a distance that matches the current feature
				    ulong xStarNo = key.Lo;
					if (usedPyramidAngles.IndexOf(xStarNo) != -1) continue;

					DistanceEntry entry_ix = iStarDists[key];
					if (entry_ix.DistanceArcSec + toleranceInArcSec < dist_ix) continue;
					if (entry_ix.DistanceArcSec - toleranceInArcSec > dist_ix) continue;

                    Dictionary<ulonglong, DistanceEntry> xStarDists = m_StarsDistanceCache[xStarNo];

					#region Test the J-X pair
                    ulonglong jxKey = new ulonglong(xStarNo , jStarNo);

					DistanceEntry entry_jx;
					if (!xStarDists.TryGetValue(jxKey, out entry_jx)) continue;

					long idx_jx = ((long)j << 32) + (long)feature.FeatureId;
					double dist_jx;
					if (isRatioFittedFocalLength || !m_FeaturesDistanceCache.TryGetValue(idx_jx, out dist_jx))
					{
						dist_jx = m_PlateConfig.GetDistanceInArcSec(feature_j.X, feature_j.Y, feature_x.X, feature_x.Y, fittedFocalLength);
						long idx_xj = ((long)feature.FeatureId << 32) + (long)j;

						if (!isRatioFittedFocalLength)
						{
							m_FeaturesDistanceCache.Add(idx_jx, dist_jx);
							m_FeaturesDistanceCache.Add(idx_xj, dist_jx);
						}
					}

					if (entry_jx.DistanceArcSec + toleranceInArcSec < dist_jx) continue;
					if (entry_jx.DistanceArcSec - toleranceInArcSec > dist_jx) continue;
					#endregion

					#region Test the K-X pair
                    ulonglong kxKey = new ulonglong(xStarNo, kStarNo);
					DistanceEntry entry_kx;
					if (!xStarDists.TryGetValue(kxKey, out entry_kx)) continue;

					long idx_kx = ((long)k << 32) + (long)feature.FeatureId;
					double dist_kx;
					if (isRatioFittedFocalLength || !m_FeaturesDistanceCache.TryGetValue(idx_kx, out dist_kx))
					{
						dist_kx = m_PlateConfig.GetDistanceInArcSec(feature_k.X, feature_k.Y, feature_x.X, feature_x.Y, fittedFocalLength);
						long idx_xk = ((long)feature.FeatureId << 32) + (long)k;

						if (!isRatioFittedFocalLength)
						{
							m_FeaturesDistanceCache.Add(idx_kx, dist_kx);
							m_FeaturesDistanceCache.Add(idx_xk, dist_kx);
						}
					}

					if (entry_kx.DistanceArcSec + toleranceInArcSec < dist_kx) continue;
					if (entry_kx.DistanceArcSec - toleranceInArcSec > dist_kx) continue;
					#endregion

					// If we are here, then we have found another star

					locatedStars++;
					IStar xStar = entry_kx.Star1.StarNo == xStarNo ? entry_kx.Star1 : entry_kx.Star2;

#if ASTROMETRY_DEBUG
					Trace.Assert(xStar.StarNo != iStarNo);
					Trace.Assert(xStar.StarNo != jStarNo);
					Trace.Assert(xStar.StarNo != kStarNo);
#endif

					if (RegisterRecognizedPair(feature_x, xStar, feature.FeatureId))
					{
						usedPyramidAngles.Add(xStar.StarNo);
					}

					//Console.WriteLine(string.Format("      {0} ({1}) {2}\" {3}\" {4}\"", xStarNo, feature.FeatureId, dist_ix.ToString("0.0"), dist_jx.ToString("0.0"), dist_kx.ToString("0.0")));
				}
			}

			if (locatedStars >= CorePyramidConfig.Default.MinPyramidAlignedStars)
			{
				ThreeStarFit.StarPair pair_i = new ThreeStarFit.StarPair(feature_i.X, feature_i.Y);
				ThreeStarFit.StarPair pair_j = new ThreeStarFit.StarPair(feature_j.X, feature_j.Y);
				ThreeStarFit.StarPair pair_k = new ThreeStarFit.StarPair(feature_k.X, feature_k.Y);

				if (ijEntry.Star1.StarNo == iStarNo)
				{
					pair_i.RADeg = ijEntry.Star1.RADeg;
					pair_i.DEDeg = ijEntry.Star1.DEDeg;
					pair_i.Star = ijEntry.Star1;

					pair_j.RADeg = ijEntry.Star2.RADeg;
					pair_j.DEDeg = ijEntry.Star2.DEDeg;
					pair_j.Star = ijEntry.Star2;

#if ASTROMETRY_DEBUG
					Trace.Assert(ijEntry.Star1.StarNo == iStarNo);
					Trace.Assert(ijEntry.Star2.StarNo == jStarNo);
#endif

					RegisterRecognizedPair(feature_i, ijEntry.Star1, i);
					RegisterRecognizedPair(feature_j, ijEntry.Star2, j);
				}
				else
				{
					pair_i.RADeg = ijEntry.Star2.RADeg;
					pair_i.DEDeg = ijEntry.Star2.DEDeg;
					pair_i.Star = ijEntry.Star2;

					pair_j.RADeg = ijEntry.Star1.RADeg;
					pair_j.DEDeg = ijEntry.Star1.DEDeg;
					pair_j.Star = ijEntry.Star1;

#if ASTROMETRY_DEBUG
					Trace.Assert(ijEntry.Star2.StarNo == iStarNo);
					Trace.Assert(ijEntry.Star1.StarNo == jStarNo);
#endif

					RegisterRecognizedPair(feature_i, ijEntry.Star2, i);
					RegisterRecognizedPair(feature_j, ijEntry.Star1, j);
				}

				if (ikEntry.Star1.StarNo == kStarNo)
				{
					pair_k.RADeg = ikEntry.Star1.RADeg;
					pair_k.DEDeg = ikEntry.Star1.DEDeg;
					pair_k.Star = ikEntry.Star1;

#if ASTROMETRY_DEBUG
					Trace.Assert(ikEntry.Star1.StarNo == kStarNo);
#endif
					RegisterRecognizedPair(feature_k, ikEntry.Star1, k);
				}
				else
				{
					pair_k.RADeg = ikEntry.Star2.RADeg;
					pair_k.DEDeg = ikEntry.Star2.DEDeg;
					pair_k.Star = ikEntry.Star2;

#if ASTROMETRY_DEBUG
					Trace.Assert(ikEntry.Star2.StarNo == kStarNo);
#endif

					RegisterRecognizedPair(feature_k, ikEntry.Star2, k);
				}

				if (m_AmbiguousMatches.Count > 0 &&
					locatedStars - m_AmbiguousMatches.Count >= CorePyramidConfig.Default.MinPyramidAlignedStars)
				{
					// If we have sufficient number of stars and ambiguous stars (close doubles that satisfy more than one solution)
					// then remove all ambiguous stars before proceeding
					foreach (ImagePixel matchedPixel in m_AmbiguousMatches)
					{
						IStar matchedStar = m_MatchedPairs[matchedPixel];

						int featureToRemove = -1;
						foreach (int featureId in m_MatchedFeatureIdToStarIdIndexes.Keys)
						{
							if (m_MatchedFeatureIdToStarIdIndexes[featureId] == matchedStar.StarNo)
							{
								featureToRemove = featureId;
								break;
							}
						}

						m_MatchedFeatureIdToStarIdIndexes.Remove(featureToRemove);
						m_MatchedPairs.Remove(matchedPixel);
					}
				}

                if (TangraConfig.Settings.TraceLevels.PlateSolving.TraceVerbose())
				    Debug.WriteLine(string.Format("Attempting DistanceBasedContext.LeastSquareFittedAstrometry ({0}={1}; {2}={3}; {4}={5})", i, iStarNo, j, jStarNo, k, kStarNo));
				
                return SolveStarPairs(
					starMap, m_MatchedPairs, m_MatchedFeatureIdToStarIdIndexes, pair_i, pair_j, pair_k,					
					fittedFocalLength, 
#if ASTROMETRY_DEBUG
					pyramidLog
#else
					null
#endif
					);
			}
			else
			{
#if PYRAMID_DEBUG || DEBUG

                foreach(ImagePixel pixel in m_MatchedPairs.Keys)
                {
                    IStar star = m_MatchedPairs[pixel];
                    foreach(int featureId in m_MatchedFeatureIdToStarIdIndexes.Keys)
                    {
                        if (m_MatchedFeatureIdToStarIdIndexes[featureId] == star.StarNo)
                        {
                            if (TangraConfig.Settings.TraceLevels.PlateSolving.TraceVerbose())
                            {
#if DEBUG
                                Debug
#endif
#if PYRAMID_DEBUG
                                Trace 
#endif
                                .WriteLine(string.Format("({0}, {1}) - StarNo: {2}; FeatureId: {3}", pixel.X, pixel.Y, star.StarNo, featureId));
                            }

                            break;
                        }
                    }
                }
#endif

#if ASTROMETRY_DEBUG
				pyramidLog.FailBecauseOfTooFiewLocatedStars(locatedStars);
				AstrometricFitDebugger.RegisterFailedPyramid(pyramidLog);
#endif
			}

			return null;
		}

		private bool RegisterRecognizedPair(ImagePixel starCenter, IStar celestialStar, int featureId)
		{
			if (m_MatchedPairs.ContainsKey(starCenter))
			{
				// The feature has been already matched with a different star (from a different triangle)
				// Probably the two stars are very close to each other. In this case we don't add the second match
				// and mark the first match as ambigous. Then later on if there is sufficient amount of pairs
				// the first match may be removed as well.
				if (m_AmbiguousMatches.IndexOf(starCenter) == -1)
					m_AmbiguousMatches.Add(starCenter);

				return false;
			}

			m_MatchedPairs.Add(starCenter, celestialStar);
			m_MatchedFeatureIdToStarIdIndexes.Add(featureId, celestialStar.StarNo);
			return true;
		}

		private LeastSquareFittedAstrometry SolveStarPairs(
			IStarMap starMap,
			Dictionary<ImagePixel, IStar> matchedPairs,
			Dictionary<int, ulong> matchedFeatureIdToStarIdIndexes,
			ThreeStarFit.StarPair pair_i,
			ThreeStarFit.StarPair pair_j,
			ThreeStarFit.StarPair pair_k,
			double fittedFocalLength,
			PyramidEntry pyramidLog, 
            int? minMatchedStars = null)
		{
			double RA0Deg, DE0Deg;

			ThreeStarFit coarseFit = new ThreeStarFit(m_PlateConfig, pair_i, pair_j, pair_k);
			if (!coarseFit.IsSolved)
			{
				if (coarseFit.IsSingularity)
				{
                    if (TangraConfig.Settings.TraceLevels.PlateSolving.TraceVerbose())
					    Trace.WriteLine("ThreeStarFit.Var1 - Singularity");

					Dictionary<ImagePixel, IStar> threeStarDict = new Dictionary<ImagePixel, IStar>();

				    try
				    {
                        threeStarDict.Add(ImagePixel.CreateImagePixelWithFeatureId(0, 255, pair_i.XImage, pair_i.YImage), pair_i.Star);
                        threeStarDict.Add(ImagePixel.CreateImagePixelWithFeatureId(1, 255, pair_j.XImage, pair_j.YImage), pair_j.Star);
                        threeStarDict.Add(ImagePixel.CreateImagePixelWithFeatureId(2, 255, pair_k.XImage, pair_k.YImage), pair_k.Star);
				    }
                    catch(ArgumentException)
                    {
                        if (pyramidLog != null) pyramidLog.FailureReason = PyramidEntryFailureReason.ThreeStarFitFailed;

                        if (TangraConfig.Settings.TraceLevels.PlateSolving.TraceVerbose())
                            Trace.WriteLine("ThreeStarFit.Var2 - Failed with ArgumentException");

                        return null;
                    }

					DirectTransRotAstrometry threeStarSolution = 
						DirectTransRotAstrometry.SolveByThreeStars(m_PlateConfig, threeStarDict, 2);

					if (threeStarSolution == null)
					{
						if (pyramidLog != null) pyramidLog.FailureReason = PyramidEntryFailureReason.ThreeStarFitFailed;

                        if (TangraConfig.Settings.TraceLevels.PlateSolving.TraceVerbose())
						    Trace.WriteLine("ThreeStarFit.Var2 - Failed");
						return null;						
					}
					else
					{
						RA0Deg = threeStarSolution.RA0Deg;
						DE0Deg = threeStarSolution.DE0Deg;
					}
				}
				else
				{
					if (pyramidLog != null) pyramidLog.FailureReason = PyramidEntryFailureReason.ThreeStarFitFailed;

                    if (TangraConfig.Settings.TraceLevels.PlateSolving.TraceVerbose())
					    Trace.WriteLine("ThreeStarFit.Var1 - Failed");
					return null;					
				}
			}
			else
			{
				if (pyramidLog != null) pyramidLog.RegisterThreeStarFit(coarseFit);
				RA0Deg = coarseFit.RA0Deg;
				DE0Deg = coarseFit.DE0Deg;
			}
			

#if DEBUG || PYRAMID_DEBUG
		    if (TangraConfig.Settings.TraceLevels.PlateSolving.TraceVerbose())
		    {
                foreach (int key in matchedFeatureIdToStarIdIndexes.Keys)
#if PYRAMID_DEBUG
                    Trace
#else
                    Debug
#endif
                    .WriteLine(string.Format("Star({0}) -> Feature({1})", matchedFeatureIdToStarIdIndexes[key], key));
		    }
#endif

            PlateConstantsSolver solver = new PlateConstantsSolver(m_PlateConfig);
			solver.InitPlateSolution(RA0Deg, DE0Deg);
		    foreach (ImagePixel feature in matchedPairs.Keys)
		    {
		        IStar star = matchedPairs[feature];
		        var kvp = matchedFeatureIdToStarIdIndexes.Single(x => x.Value == star.StarNo);
		        int featureId = kvp.Key;
                solver.AddStar(feature, star, featureId);
		    }

		    LeastSquareFittedAstrometry leastSquareFittedAstrometry = null;
			LeastSquareFittedAstrometry firstFit = null;
			try
			{
				// This is a linear regression when doing simple field alignment. We always use a Linear Fit
				leastSquareFittedAstrometry = solver.SolveWithLinearRegression(
					FitOrder.Linear,
				 	CorePyramidConfig.Default.MinPyramidAlignedStars,
					m_MaxLeastSquareResidual, 
					out firstFit);
			}
			catch (DivideByZeroException)
			{ }

			if (leastSquareFittedAstrometry != null)
			{
				if (pyramidLog != null) pyramidLog.RegisterLinearFit(leastSquareFittedAstrometry);

                if (TangraConfig.Settings.TraceLevels.PlateSolving.TraceVerbose())
				    Trace.WriteLine("Checking possible solution. ");

				List<ulong> usedStarIds = leastSquareFittedAstrometry.FitInfo.AllStarPairs
					.Where(p => p.FitInfo.UsedInSolution)
					.Select(p => p.StarNo)
					.ToList();

				int usedStars = usedStarIds.Count;

				matchedFeatureIdToStarIdIndexes = matchedFeatureIdToStarIdIndexes
					.Where(kvp => usedStarIds.Contains(kvp.Value))
					.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

				List<double> residuals = 
					leastSquareFittedAstrometry.FitInfo.AllStarPairs
						.Where(p => !p.FitInfo.ExcludedForHighResidual)
						.Select(p => p.FitInfo.ResidualArcSec)
						.ToList();


				double secondLargeResidual = 0;

				if (residuals.Count > 0)
				{
					residuals = residuals.OrderByDescending(r => r).ToList();
					secondLargeResidual = residuals.Count > 1 ? residuals[1] : residuals[0];
				}

				double onePixDistArcSec = m_PlateConfig.GetDistanceInArcSec(0, 0, 1, 1);
				if (secondLargeResidual > onePixDistArcSec * CorePyramidConfig.Default.MaxAllowedResidualInPixelsInSuccessfulFit)
				{
					if (pyramidLog != null) pyramidLog.FailureReason = PyramidEntryFailureReason.SecondLargestResidualIsTooLarge;

                    if (TangraConfig.Settings.TraceLevels.PlateSolving.TraceVerbose())
					    Trace.WriteLine(string.Format(
						    "Failing preliminary solution because the second largest residual {0}\" is larger than {1}px",
						    secondLargeResidual.ToString("0.0"), CorePyramidConfig.Default.MaxAllowedResidualInPixelsInSuccessfulFit));

					return null;
				}

                if (minMatchedStars.HasValue)
				{
                    if (usedStars < minMatchedStars.Value)
					{
						if (pyramidLog != null) pyramidLog.FailureReason = PyramidEntryFailureReason.InsufficientStarsForCalibration;

                        if (TangraConfig.Settings.TraceLevels.PlateSolving.TraceVerbose())
						    Trace.WriteLine(string.Format(
						    "Failing preliminary solution because on {0} stars are used but {1} are required as a minimum for calibration.",
						    usedStars, CorePyramidConfig.Default.MinMatchedStarsForCalibration));

						return null;
					}
				}
			}
			else
			{
				if (pyramidLog != null) pyramidLog.FailureReason = PyramidEntryFailureReason.LinearFitFailed;

			    if (TangraConfig.Settings.TraceLevels.PlateSolving.TraceVerbose())
			    {
                    Debug.WriteLine("DistanceBasedContext.LeastSquareFittedAstrometry Failed!");

                    foreach (PlateConstStarPair pair in solver.Pairs)
                    {
#if PYRAMID_DEBUG
                        Trace
#else
                        Debug
#endif
                        .WriteLine(string.Format("{0} ({1}) -> Residuals: {2}\", {3}\"", pair.StarNo,
                                                      pair.FitInfo.UsedInSolution ? "Included" : "Excluded",
                                                      pair.FitInfo.ResidualRAArcSec.ToString("0.00"),
                                                      pair.FitInfo.ResidualDEArcSec.ToString("0.00")));
                    }

			    }
			}

			if (leastSquareFittedAstrometry != null)
			{
				leastSquareFittedAstrometry.FitInfo.FittedFocalLength = fittedFocalLength;
				if (pyramidLog != null) pyramidLog.RegisterFocalLength(fittedFocalLength);
			}

			return leastSquareFittedAstrometry;
		}

		internal delegate bool CheckTriangleCallback(int i, int j, int k, double dij, double dik, double djk, double toleranceInArcSec);
		internal delegate bool CheckTriangleWithRatiosCallback(int i, int j, int k, ImagePixel iCenter, ImagePixel jCenter, ImagePixel kCenter, double toleranceInArcSec);

		internal bool LoopThroghFeatureTriangles(IStarMap starMap, double toleranceInArcSec, CheckTriangleCallback callback)
		{
			return LoopThroghFeatureTriangles(starMap, toleranceInArcSec, callback, null);
		}

		internal bool LoopThroghFeatureTriangles(IStarMap starMap, double toleranceInArcSec, CheckTriangleWithRatiosCallback callback)
		{
			return LoopThroghFeatureTriangles(starMap, toleranceInArcSec, null, callback);
		}

		internal void GetNumStarsInRegionAndPyramidSet(
			double pyramidMinMag, 
			double pyramidMaxMag,
			out int numInmagRegion,
			out int numPyramidStars)
		{
			numInmagRegion = DebugResolvedStars
				.Select(s => m_CelestialAllStars.First(p => p.StarNo == s.Value))
				.Where(s => s.Mag >= pyramidMinMag && s.Mag <= pyramidMaxMag)
				.Count();

			
			numPyramidStars = DebugResolvedStars
				.Where(s => m_CelestialPyramidStars.Exists(p => p.StarNo == s.Value))
				.Count();
		}

		private bool LoopThroghFeatureTriangles(
			IStarMap starMap, 
			double toleranceInArcSec, 
			CheckTriangleCallback callback, 
			CheckTriangleWithRatiosCallback callbackWithRatios)
		{
			if (m_PreviousFit != null)
			{
			    if (TryMatchingPairsFromPreviousFit(starMap))
			    {
                    // Successfull fit from star to feature matching inferred from the previous fit was successfull
                    if (ImproveAndRetestSolution(0, 0, 0, true))
                        return true;
                    else
                    {
                        Trace.WriteLine("Could improve solution from previous fit");
                    }
			    }
			    else
			    {
			        Trace.WriteLine("Could not match stars from previous fit");
			    }
			}

			if (m_ManualPairs != null &&
				TryMatchingPairsFromManualPairs(starMap))
			{
				// Successfull fit from star to feature matching inferred from the previous fit was successfull
                if (ImproveAndRetestSolution(0, 0, 0))
					return true;
			}

			int n = starMap.FeaturesCount;

			if (m_Settings.PyramidOptimumStarsToMatch < n)
			{
				// TODO: Need to extract only the largest m_Settings.PyramidOptimumStarsToMatch features.
			}

			int total = n * (n - 1) * (n - 2) / 6;
			int counter = 0;
			int maxCombinationsBeforeFail = CorePyramidConfig.Default.MaxNumberOfCombinations;

			Stopwatch timeTaken = new Stopwatch();
			timeTaken.Start();

			bool delayWarningSent = false;

//            if (DebugResolvedStars != null)
//            {
//                int numInmagRegion, numPyramidStars;

//                GetNumStarsInRegionAndPyramidSet(
//                    m_PyramidMinMag, m_PyramidMaxMag,
//                    out numInmagRegion, out numPyramidStars);

//#if ASTROMETRY_DEBUG
//                Trace.Assert(numInmagRegion > 3);
//                Trace.Assert(numPyramidStars > 3); 			
//#endif
//            }

			if (m_ManualPairs != null && m_ManualPairs.Count <= 3)
			{
				if (m_ManualPairs.Count == 1)
				{
                    int fixedFeatureIndex = m_ManualPairs.Keys.ToList()[0].FeatureId + 1;
					for (int k = 2; k <= n; k++)
					{
						for (int j = 1; j < k; j++)
						{
                            var rv = CheckCombination(fixedFeatureIndex, j, k, starMap, toleranceInArcSec, callback, callbackWithRatios,
								timeTaken, ref counter, ref delayWarningSent, maxCombinationsBeforeFail, total, n);
							if (rv != null) return rv.Value;
						}
					}
				}
				else if (m_ManualPairs.Count == 2)
				{
					int fixedFeatureIndex1 = m_ManualPairs.Keys.ToList()[0].FeatureId + 1;
                    int fixedFeatureIndex2 = m_ManualPairs.Keys.ToList()[1].FeatureId + 1;
					for (int k = 1; k <= n; k++)
					{
                        var rv = CheckCombination(fixedFeatureIndex1, fixedFeatureIndex2, k, starMap, toleranceInArcSec, callback, callbackWithRatios,
							timeTaken, ref counter, ref delayWarningSent, maxCombinationsBeforeFail, total, n);
						if (rv != null) return rv.Value;
					}
				}
                else if (m_ManualPairs.Count == 3)
                {
                    var m_FeatureId_i = m_ManualPairs.Keys.ToList()[0].FeatureId;
                    var m_FeatureId_j = m_ManualPairs.Keys.ToList()[1].FeatureId;
                    var m_FeatureId_k = m_ManualPairs.Keys.ToList()[2].FeatureId;
                    ulong starNo1 = m_ManualPairs.Values.ToList()[0].StarNo;
                    ulong starNo2 = m_ManualPairs.Values.ToList()[1].StarNo;
                    ulong starNo3 = m_ManualPairs.Values.ToList()[2].StarNo;

                    int fixedFeatureIndex1 = m_FeatureId_i + 1;
                    int fixedFeatureIndex2 = m_FeatureId_j + 1;
                    int fixedFeatureIndex3 = m_FeatureId_k + 1;


                    var ijEntry = m_DistancesByMagnitude.FirstOrDefault(x => (x.Star1.StarNo == starNo1 && x.Star2.StarNo == starNo2) || (x.Star1.StarNo == starNo2 && x.Star2.StarNo == starNo1));
                    var ikEntry = m_DistancesByMagnitude.FirstOrDefault(x => (x.Star1.StarNo == starNo1 && x.Star2.StarNo == starNo3) || (x.Star1.StarNo == starNo3 && x.Star2.StarNo == starNo1));
                    var jkEntry = m_DistancesByMagnitude.FirstOrDefault(x => (x.Star1.StarNo == starNo3 && x.Star2.StarNo == starNo2) || (x.Star1.StarNo == starNo2 && x.Star2.StarNo == starNo3));

                    m_Solution = IsSuccessfulMatch(m_StarMap, fixedFeatureIndex1, fixedFeatureIndex2, fixedFeatureIndex3, ijEntry, ikEntry, jkEntry,
                               starNo1, starNo2, starNo3, toleranceInArcSec);

                    if (m_Solution != null)
                    {
                        if (ImproveAndRetestSolution(fixedFeatureIndex1, fixedFeatureIndex2, fixedFeatureIndex3))
                        {
                            m_MatchedTriangle = string.Format("{0}-{1}-{2}:{5}:[{3}/{4}]", fixedFeatureIndex1, fixedFeatureIndex2, fixedFeatureIndex3, counter, total, n);
                            return true;
                        }
                    }
                }
			}

		    for (int k = 3; k <= n; k++)
			{
				for (int j = 2; j < k; j++)
				{
					for (int i = 1; i < j; i++)
					{
						var rv = CheckCombination(i, j, k, starMap, toleranceInArcSec, callback, callbackWithRatios, 
							timeTaken, ref counter, ref delayWarningSent, maxCombinationsBeforeFail, total, n);
						if (rv != null) return rv.Value;
					}
				}
			}

			return false;
		}

	    internal string CurrentCombination { get; set; }

	    private bool? CheckCombination(int i, int j, int k, 
			IStarMap starMap,
			double toleranceInArcSec,
			CheckTriangleCallback callback,
			CheckTriangleWithRatiosCallback callbackWithRatios,
			Stopwatch timeTaken,
			ref int counter,
			ref bool delayWarningSent,
			int maxCombinationsBeforeFail,
			int total,
			int n)
		{
			if (i == j || i == k || j == k) return null;

	        CurrentCombination = string.Format("{0}-{1}-{2}", i, j, k);
			counter++;

			if (DebugResolvedStars != null)
			{
				if (!DebugResolvedStars.ContainsKey(i)) return null;
				if (!DebugResolvedStars.ContainsKey(j)) return null;
				if (!DebugResolvedStars.ContainsKey(k)) return null;

				if (DebugExcludeStars != null)
				{
					if (DebugExcludeStars.ContainsKey(i)) return null;
					if (DebugExcludeStars.ContainsKey(j)) return null;
					if (DebugExcludeStars.ContainsKey(k)) return null;
				}
			}
#if PYRAMID_DEBUG
                        Trace.WriteLine(string.Format("Trying triangle {0}-{1}-{2}", i, j, k));
#endif

			if (counter >= maxCombinationsBeforeFail)
				return false;

			if (m_AbortSearch) return false;

			if (counter % 5000 == 0)
                m_OperationNotifier.SendNotification(new OperationNotifications(NotificationType.SearchProgressed, null));

			if (!delayWarningSent &&
				timeTaken.ElapsedMilliseconds > m_Settings.PyramidTimeoutInSeconds * 100 &&
				timeTaken.ElapsedMilliseconds > 5000)
			{
                m_OperationNotifier.SendNotification(new OperationNotifications(NotificationType.SearchTakingLonger, timeTaken.ElapsedMilliseconds));
				delayWarningSent = true;
			}

			if (timeTaken.ElapsedMilliseconds > m_Settings.PyramidTimeoutInSeconds * 1000)
			{
				if (TangraConfig.Settings.TraceLevels.PlateSolving.TraceError())
					Trace.WriteLine(string.Format("Timeout of {0}sec reached at {1}-{2}-{3}", m_Settings.PyramidTimeoutInSeconds, i, j, k));
				return false;
			}

			if (callbackWithRatios != null)
			{
				if (CheckTriangleWithRatios(starMap, callbackWithRatios, i, j, k, toleranceInArcSec))
				{
					if (ImproveAndRetestSolution(i, j, k))
					{
						m_MatchedTriangle = string.Format("{0}-{1}-{2}:{5}:[{3}/{4}]", i, j, k, counter, total, n);
						return true;
					}
				}

			}
			else
			{
				if (CheckTriangle(starMap, callback, i, j, k, toleranceInArcSec))
				{
					if (ImproveAndRetestSolution(i, j, k))
					{
						m_MatchedTriangle = string.Format("{0}-{1}-{2}:{5}:[{3}/{4}]", i, j, k, counter, total, n);
						return true;
					}
				}
			}

			return null;
		}

        private void LogUnsuccessfulFitImage(LeastSquareFittedAstrometry fit, int i, int j, int k, string traceMessage)
	    {
            if (TangraConfig.Settings.TraceLevels.PlateSolving.TraceVerbose())
                Trace.WriteLine(traceMessage);
				           
            if (DebugSaveAlignImages)
                SaveAlignImage(fit.FitInfo.AllStarPairs, i, j, k, false, traceMessage);

        }
		private bool ImproveAndRetestSolution(int i, int j, int k, bool fromPreviousFit = false)
		{
			if (m_Solution != null)
			{
				LeastSquareFittedAstrometry fit = m_Solution as LeastSquareFittedAstrometry;

				if (fit != null)
				{
#if ASTROMETRY_DEBUG
                    SolutionImprovementEntry improvementLog = new SolutionImprovementEntry(fit, i, j, k);
					AstrometricFitDebugger.RegisterSolutionToImprove(improvementLog);
#endif
					ImproveSolution(fit, 1, i, j, k);

				    if (m_ImprovedSolution != null && CorePyramidConfig.Default.AttempDoubleSolutionImprovement)
				    {
                        if (TangraConfig.Settings.TraceLevels.PlateSolving.TraceInfo())
                            Trace.WriteLine("Attempting double solution improvement.");

				        ImproveSolution(m_ImprovedSolution, 2, i, j, k);
				    }

					if (m_ImprovedSolution == null)
					{
#if ASTROMETRY_DEBUG
						improvementLog.Fail(SolutionImprovementFailureReason.FailedToImproveSolution);
#endif
                        if (TangraConfig.Settings.TraceLevels.PlateSolving.TraceVerbose())
						    Trace.WriteLine("Failing potential fit because the improved solultion is NULL.");
					
						// Check for a false negative
						double? deltaArcSec = null;

						if (!double.IsNaN(DebugResolvedRA0Deg) && !double.IsNaN(DebugResolvedDE0Deg))
						{
							deltaArcSec = AngleUtility.Elongation(
								fit.RA0Deg,
								fit.DE0Deg,
								DebugResolvedRA0Deg,
								DebugResolvedDE0Deg) * 3600;

							double deltaPixels = m_PlateConfig.GetDistanceInPixels(deltaArcSec.Value);

							if (deltaPixels < 5)
							{
                                if (TangraConfig.Settings.TraceLevels.PlateSolving.TraceVerbose())
								    Trace.WriteLine(string.Format("DEBUG ALIGN: False negative alignment on {0}-{1}-{2}", i, j, k));

								Trace.Assert(false, "False Negative!");
								SaveAlignImage(fit.FitInfo.AllStarPairs, i, j, k, false);
								return false;
							}
						}

						return false;
					}					

#if ASTROMETRY_DEBUG
					improvementLog.ImprovedSolution = m_ImprovedSolution;
#endif
					List<PlateConstStarPair> allPairsInTheFrame =
						m_ImprovedSolution.FitInfo.AllStarPairs
							.Where(p => p.x > 0 && p.x < m_PlateConfig.ImageWidth && p.y > 0 && p.y < m_PlateConfig.ImageHeight)
							.ToList();

					if (allPairsInTheFrame.Count < 4)
					{
#if ASTROMETRY_DEBUG
						improvementLog.Fail(SolutionImprovementFailureReason.LessThanFourStars);
#endif
                        LogUnsuccessfulFitImage(m_ImprovedSolution, i, j, k, 
                            "Failing potential fit because there are less than 4 included stars in the improved solultion .");

						return false;
					}

					double detectionLimit = m_Settings.DetectionLimit;					
					int uncertainStars = allPairsInTheFrame.Count(p => p.DetectionCertainty < detectionLimit);

					if (allPairsInTheFrame.Count - uncertainStars < uncertainStars)
					{
						// Cant be right!
#if ASTROMETRY_DEBUG
						improvementLog.Fail(SolutionImprovementFailureReason.TooManyUncertainStars);
#endif
                        LogUnsuccessfulFitImage(m_ImprovedSolution, i, j, k, 
                            string.Format("Failing potential fit because there are {0} out of {1} uncertain stars in the improved solution.", uncertainStars, allPairsInTheFrame.Count));

						return false;
					}

					int excludedStars = allPairsInTheFrame.Count(p => p.FitInfo.ExcludedForHighResidual);
					if (allPairsInTheFrame.Count - excludedStars < 20 && allPairsInTheFrame.Count - excludedStars < excludedStars)
					{
						// Cant be right!
#if ASTROMETRY_DEBUG
						improvementLog.Fail(SolutionImprovementFailureReason.TooManyExcludedStars);
#endif
                        LogUnsuccessfulFitImage(m_ImprovedSolution, i, j, k, 
                            string.Format("Failing potential fit because there are {0} out of {1} excluded stars in the improved solultion.", excludedStars, allPairsInTheFrame.Count));

						return false;
					}

					List<double> residuls = allPairsInTheFrame
						.Select(p => p.FitInfo.ResidualArcSec)
						.OrderByDescending(r => r).ToList();

					double averageResidual = residuls.Average();

					if (averageResidual >
						CorePyramidConfig.Default.MaxMeanResidualInPixelsInSuccessfulFit * m_PlateConfig.GetDistanceInArcSec(0, 0, 1, 1))
					{
#if ASTROMETRY_DEBUG
						improvementLog.Fail(SolutionImprovementFailureReason.MaxCoreMeanResidualTooHigh);
#endif
                        LogUnsuccessfulFitImage(m_ImprovedSolution, i, j, k, 
                            string.Format("Failing potential fit because the average residual of {0}\" is larger than {1} px", averageResidual.ToString("0.00"), CorePyramidConfig.Default.MaxMeanResidualInPixelsInSuccessfulFit.ToString("0.0")));

						return false;						
					}

					if (m_DetermineAutoLimitMagnitude && double.IsNaN(m_DetectedLimitingMagnitude))
					{
						List<PlateConstStarPair> sortedPairs = m_ImprovedSolution.FitInfo.AllStarPairs
						.Where(p => p.DetectionCertainty >= m_Settings.LimitReferenceStarDetection)
						.ToList();

						List<ulong> starNosUnderDetectionLimit = m_ImprovedSolution.FitInfo.AllStarPairs
						.Where(p => p.DetectionCertainty < m_Settings.LimitReferenceStarDetection)
						.Select(p => p.StarNo)
						.ToList();

						m_ImprovedSolution.FitInfo.AllStarPairs
							.RemoveAll(p => p.DetectionCertainty < m_Settings.LimitReferenceStarDetection);

						sortedPairs.Sort((a, b) => a.DetectionCertainty.CompareTo(b.DetectionCertainty));

						double limitingMag = GetLimitingMagnitudeFromSortedPairs(sortedPairs) + 0.01;					
						if (!double.IsNaN(limitingMag))
						{
							m_ImprovedSolution.FitInfo.DetectedLimitingMagnitude = limitingMag;

							m_DetectedLimitingMagnitude = m_ImprovedSolution.FitInfo.DetectedLimitingMagnitude;
							m_CelestialPyramidStars = m_CelestialAllStars
								.Where(s => s.Mag <= m_DetectedLimitingMagnitude && !starNosUnderDetectionLimit.Contains(s.StarNo))
								.ToList();

							// Also reduce all stars. This would mean if the integration changes on the way, the user will
							// need to start the astrometry again
							m_CelestialAllStars = m_CelestialPyramidStars;

							m_ImprovedSolution.FitInfo.AllStarPairs.RemoveAll(p => !p.FitInfo.UsedInSolution || p.Mag > limitingMag);
							allPairsInTheFrame.RemoveAll(p => !p.FitInfo.UsedInSolution || p.Mag > limitingMag || p.DetectionCertainty < m_Settings.LimitReferenceStarDetection);
						
							InitializePyramidMatching();

                            if (TangraConfig.Settings.TraceLevels.PlateSolving.TraceVerbose())
							    Trace.WriteLine(string.Format("Limiting reference star magnitude estimated as {0:0.0}", limitingMag));
						}
					}
					else if (m_DetermineAutoLimitMagnitude)
					{
						m_ImprovedSolution.FitInfo.DetectedLimitingMagnitude = m_DetectedLimitingMagnitude;
					}

					if (m_ImprovedSolution.FitInfo.AllStarPairs.Count > m_Settings.MaximumNumberOfStars)
					{
						m_ImprovedSolution.FitInfo.AllStarPairs.Sort((a, b) => a.Mag.CompareTo(b.Mag));

						List<PlateConstStarPair> usedPairs = m_ImprovedSolution.FitInfo.AllStarPairs.Where(p => p.FitInfo.UsedInSolution).ToList();
						if (usedPairs.Count > m_Settings.MaximumNumberOfStars)
						{
							double maxMagToInclude = usedPairs[m_Settings.MaximumNumberOfStars - 1].Mag - 0.01;

							m_ImprovedSolution.FitInfo.AllStarPairs.RemoveAll(p => p.Mag > maxMagToInclude);
							allPairsInTheFrame.RemoveAll(p => p.Mag > maxMagToInclude);

							if (m_DetermineAutoLimitMagnitude)
							{
								// This will remove all grayed circles of the excess stars which have been excluded 
								m_CelestialPyramidStars.RemoveAll(p => p.Mag > maxMagToInclude);
								m_CelestialAllStars = m_CelestialPyramidStars;
								
								InitializePyramidMatching();
							}
						}
                    }

                    #region Check how many of the considered stars, brighter than the detected limiting magnitude were not used in the solution
                    int detectedStars = 0;
                    int consideredStars = 0;
                    int missingStars = 0;
                    double checkMag = ImprovedSolutionIncludedMaxMag;
				    
                    double minDetectionCertaintyUsedInSolution = m_ImprovedSolution.FitInfo.AllStarPairs.Where(x => x.FitInfo.UsedInSolution).Min(x => x.DetectionCertainty);
				    if (TangraConfig.Settings.Astrometry.DetectionLimit < minDetectionCertaintyUsedInSolution)
				        minDetectionCertaintyUsedInSolution = TangraConfig.Settings.Astrometry.DetectionLimit;

				    foreach (IStar catStar in ConsideredStars)
				    {
                        double x, y;
				        m_ImprovedSolution.GetImageCoordsFromRADE(catStar.RADeg, catStar.DEDeg, out x, out y);
				        if (x > 0 && y > 0 && x < m_PlateConfig.ImageWidth && y < m_PlateConfig.ImageHeight)
				        {
				            if (catStar.Mag < checkMag)
				            {
				                if (m_StarMap.GetFeatureInRadius((int) x, (int) y, 2) == null)
				                {
                                    // Brighter star not in the solition. Is there actually a star there
                                    PSFFit psfFit;
                                    m_StarMap.GetPSFFit((int)x, (int)y, PSFFittingMethod.NonLinearFit, out psfFit);

				                    if (psfFit != null && psfFit.IsSolved &&
				                        psfFit.Certainty > minDetectionCertaintyUsedInSolution)
				                    {
				                        consideredStars++;
				                    }
				                    else
				                        missingStars++;
				                }
                                else
				                    consideredStars++;

				                if (m_ImprovedSolution.FitInfo.AllStarPairs.FirstOrDefault(s => s.StarNo == catStar.StarNo) != null)
				                    detectedStars++;
				            }
				        }
				    }
                    if (detectedStars < 25 && !fromPreviousFit)
				    {
				        if (missingStars > detectedStars || missingStars > consideredStars/2)
				        {
                            LogUnsuccessfulFitImage(fit, i, j, k,
                                string.Format("Failing potential fit because {0} missing stars are more than {1} detected stars OR they are more than half of the {2} considered stars.", missingStars, detectedStars, consideredStars));
				            return false;
				        }

				        double detectedConsideredStarsPercentage = 1.0 * detectedStars/(consideredStars + missingStars);

				        if (detectedConsideredStarsPercentage < 0.50 && consideredStars > 2 * detectedStars)
				        {
                            LogUnsuccessfulFitImage(m_ImprovedSolution, i, j, k, 
                                string.Format("Failing potential fit because the {0} detected considered stars are less than 50% and {1} considered stars are more than double the {2} detected stars.", detectedConsideredStarsPercentage, consideredStars, detectedStars));

				            return false;
				        }
				        if (detectedConsideredStarsPercentage < 0.25 && consideredStars > 10)
				        {
                            LogUnsuccessfulFitImage(m_ImprovedSolution, i, j, k, 
                                string.Format("Failing potential fit because the {0}% of detected considered stars are less than 25% and {1} considered stars are more than 10.", detectedConsideredStarsPercentage, consideredStars));

				            return false;
				        }
                    }
                    #endregion

                    if (DebugResolvedStars != null)
					{
						DebugTripple tripple = m_DebugTripples.FirstOrDefault(t =>
							(t.Id1 == i && t.Id2 == j && t.Id3 == k) ||
							(t.Id1 == i && t.Id2 == k && t.Id3 == j) ||
							(t.Id1 == j && t.Id2 == i && t.Id3 == k) ||
							(t.Id1 == j && t.Id2 == k && t.Id3 == i) ||
							(t.Id1 == k && t.Id2 == i && t.Id3 == j) ||
							(t.Id1 == k && t.Id2 == j && t.Id3 == i));

						if (tripple != null)
						{
							double? deltaArcSec = null;

							if (!double.IsNaN(DebugResolvedRA0Deg) && !double.IsNaN(DebugResolvedDE0Deg))
							{
								deltaArcSec = AngleUtility.Elongation(
									m_ImprovedSolution.RA0Deg, 
									m_ImprovedSolution.DE0Deg, 
									DebugResolvedRA0Deg, 
									DebugResolvedDE0Deg) * 3600;

								double deltaPixels = m_PlateConfig.GetDistanceInPixels(deltaArcSec.Value);

								if (deltaPixels > 1)
								{
									Trace.Assert(false, "False Positive!");
                                    if (TangraConfig.Settings.TraceLevels.PlateSolving.TraceVerbose())
									    Trace.WriteLine(string.Format("DEBUG ALIGN: False positive alignment on {0}-{1}-{2}", i, j, k));
									return false;
								}
							}

                            if (TangraConfig.Settings.TraceLevels.PlateSolving.TraceVerbose())
							    Trace.WriteLine(string.Format(
								    "DEBUG ALIGN: Successful alignment on {0}-{1}-{2}{4}. Remaining combinations: {3}",
								    i, j, k, m_DebugTripples.Count,
								    deltaArcSec.HasValue ? string.Format(" ({0:0.000}mas Diff)", 1000 * deltaArcSec.Value) : ""));

							m_DebugTripples.Remove(tripple);

							if (DebugSaveAlignImages)
								SaveAlignImage(m_ImprovedSolution.FitInfo.AllStarPairs, i, j, k, true);

							return m_DebugTripples.Count == 0;
						}
						else
						{
                            LogUnsuccessfulFitImage(fit, i, j, k, 
                                string.Format("DEBUG ALIGN: Unexpected alignment on {0}-{1}-{2}", i, j, k));

							return false;
						}
					}
					return true;					
				}
			}

            if (TangraConfig.Settings.TraceLevels.PlateSolving.TraceVerbose())
			    Trace.WriteLine("Failing potential fit because there is no solution or it hasn't been fitted using least squares.");

			return false;
		}

		private Font s_DebugFont = new Font(FontFamily.GenericSerif, 8);

        [DebuggerStepThrough]
        private void SaveAlignImage(List<PlateConstStarPair> pairs, int i, int j, int k, bool success, string traceMessage = null)
		{
            try
            {
                if ((m_StarMap as StarMap).GetPixelmap().DisplayBitmap == null) 
                    return;

                Bitmap image = (Bitmap)((m_StarMap as StarMap).GetDisplayBitmap()).Clone();

                using (Graphics g = Graphics.FromImage(image))
                {
                    foreach (PlateConstStarPair pair in pairs)
                    {
                        Pen pen = Pens.Gray;
                        if (pair.FitInfo.UsedInSolution) pen = Pens.Green;
                        if (pair.FitInfo.ExcludedForHighResidual) pen = Pens.Orange;
                        g.DrawEllipse(pen, (float)pair.x - 6f, (float)pair.y - 6f, 12, 12);

                        StarMapFeature feature = m_StarMap.Features
                            .Where(delegate(StarMapFeature f)
                            {
                                ImagePixel center = f.GetCenter();
                                double distance = Math.Sqrt(
                                    (center.XDouble - pair.x) * (center.XDouble - pair.x) +
                                    (center.YDouble - pair.y) * (center.YDouble - pair.y));

                                return distance < 3;
                            })
                            .FirstOrDefault();

                        if (feature == null) continue;

                        g.DrawString(
                            string.Format("{0}({1})",
                                feature == null ? "?" : (feature.FeatureId + 1).ToString(),
                                FormatPrintStarNo(pair.StarNo)),
                            s_DebugFont, Brushes.Yellow, (float)pair.x + 7f, (float)pair.y + 18f);

                        if (feature != null)
                        {
                            if (feature.FeatureId == i - 1 || feature.FeatureId == j - 1 || feature.FeatureId == k - 1)
                            {
                                g.DrawEllipse(pen, (float)pair.x - 5f, (float)pair.y - 5f, 10, 10);
                                g.DrawEllipse(pen, (float)pair.x - 8f, (float)pair.y - 8f, 16, 16);
                            }
                        }
                    }

                    if (traceMessage != null)
                    {
                        var txtSize = g.MeasureString(traceMessage, s_DebugFont);
                        g.FillRectangle(Brushes.Black, 10, 10, txtSize.Width, txtSize.Height);
                        g.DrawString(traceMessage, s_DebugFont, Brushes.Pink, 10, 10);
                    }

                    g.Save();
                }

                string outFileName = Path.GetFullPath(EnsureDebugSessionDirectory() + string.Format(@"\AlignmentTest_{3}{0}-{1}-{2}.bmp", i, j, k, success ? "" : "Failed_"));
                if (TangraConfig.Settings.TraceLevels.PlateSolving.TraceVerbose())
                    Trace.WriteLine(string.Format("Saving report file: {0}", outFileName));
                image.Save(outFileName);			
            }
            catch (Exception)
            { }
		}

	    private string FormatPrintStarNo(ulong starNo)
	    {
	        var rv = starNo.ToString();
	        if (rv.Length > 8)
	            rv = rv.Substring(0, 2) + ".." + rv.Substring(rv.Length - 4, 4);
	        return rv;
	    }

        private string _debugOutputDir = null;

	    private string EnsureDebugSessionDirectory()
	    {
	        if (_debugOutputDir == null)
	        {
                _debugOutputDir = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + string.Format(@"\AstrometryDebug\{0}_{1}", DateTime.Now.ToString("yyyy_MM_dd_HH_mm"), Guid.NewGuid()));
                if (!Directory.Exists(_debugOutputDir))
                    Directory.CreateDirectory(_debugOutputDir);	            
	        }
            return _debugOutputDir;
	    }

		private double GetLimitingMagnitudeFromSortedPairs(List<PlateConstStarPair> sortedPairs)
		{
			List<IStar> stars = new List<IStar>();

			if (sortedPairs.Count == 0)
				return double.NaN;
			else if (sortedPairs.Count <= 3)
			{			
				PlateConstStarPair faintestPair = sortedPairs[0];
				IStar star = m_CelestialAllStars.FirstOrDefault(s => s.StarNo == faintestPair.StarNo);
				return star.Mag;
			}
			else if (sortedPairs.Count <= 7)
			{
				PlateConstStarPair faintestPair1 = sortedPairs[0];
				IStar star1 = m_CelestialAllStars.FirstOrDefault(s => s.StarNo == faintestPair1.StarNo);
				PlateConstStarPair faintestPair2 = sortedPairs[1];
				IStar star2 = m_CelestialAllStars.FirstOrDefault(s => s.StarNo == faintestPair2.StarNo);
				PlateConstStarPair faintestPair3 = sortedPairs[2];
				IStar star3 = m_CelestialAllStars.FirstOrDefault(s => s.StarNo == faintestPair3.StarNo);

				return Math.Max(star3.Mag, Math.Max(star1.Mag, star2.Mag));
			}
            else if (sortedPairs.Count <= 50)
			{
				PlateConstStarPair faintestPair1 = sortedPairs[0];
				IStar star1 = m_CelestialAllStars.FirstOrDefault(s => s.StarNo == faintestPair1.StarNo);
				PlateConstStarPair faintestPair2 = sortedPairs[1];
				IStar star2 = m_CelestialAllStars.FirstOrDefault(s => s.StarNo == faintestPair2.StarNo);
				PlateConstStarPair faintestPair3 = sortedPairs[2];
				IStar star3 = m_CelestialAllStars.FirstOrDefault(s => s.StarNo == faintestPair3.StarNo);

				stars.Add(star1);
				stars.Add(star2);
				stars.Add(star3);
			}
            else if (sortedPairs.Count <= 100)
            {
                PlateConstStarPair faintestPair1 = sortedPairs[0];
                IStar star1 = m_CelestialAllStars.FirstOrDefault(s => s.StarNo == faintestPair1.StarNo);
                PlateConstStarPair faintestPair2 = sortedPairs[1];
                IStar star2 = m_CelestialAllStars.FirstOrDefault(s => s.StarNo == faintestPair2.StarNo);
                PlateConstStarPair faintestPair3 = sortedPairs[2];
                IStar star3 = m_CelestialAllStars.FirstOrDefault(s => s.StarNo == faintestPair3.StarNo);
                PlateConstStarPair faintestPair4 = sortedPairs[3];
                IStar star4 = m_CelestialAllStars.FirstOrDefault(s => s.StarNo == faintestPair4.StarNo);
                PlateConstStarPair faintestPair5 = sortedPairs[4];
                IStar star5 = m_CelestialAllStars.FirstOrDefault(s => s.StarNo == faintestPair5.StarNo);

				stars.Add(star1);
				stars.Add(star2);
				stars.Add(star3);
				stars.Add(star4);
				stars.Add(star5);
			}
            else
            {
                PlateConstStarPair faintestPair1 = sortedPairs[0];
                IStar star1 = m_CelestialAllStars.FirstOrDefault(s => s.StarNo == faintestPair1.StarNo);
                PlateConstStarPair faintestPair2 = sortedPairs[1];
                IStar star2 = m_CelestialAllStars.FirstOrDefault(s => s.StarNo == faintestPair2.StarNo);
                PlateConstStarPair faintestPair3 = sortedPairs[2];
                IStar star3 = m_CelestialAllStars.FirstOrDefault(s => s.StarNo == faintestPair3.StarNo);
                PlateConstStarPair faintestPair4 = sortedPairs[3];
                IStar star4 = m_CelestialAllStars.FirstOrDefault(s => s.StarNo == faintestPair4.StarNo);
                PlateConstStarPair faintestPair5 = sortedPairs[4];
                IStar star5 = m_CelestialAllStars.FirstOrDefault(s => s.StarNo == faintestPair5.StarNo);
                PlateConstStarPair faintestPair6 = sortedPairs[5];
                IStar star6 = m_CelestialAllStars.FirstOrDefault(s => s.StarNo == faintestPair6.StarNo);
                PlateConstStarPair faintestPair7 = sortedPairs[6];
                IStar star7 = m_CelestialAllStars.FirstOrDefault(s => s.StarNo == faintestPair7.StarNo);

				stars.Add(star1);
				stars.Add(star2);
				stars.Add(star3);
				stars.Add(star4);
				stars.Add(star5);
				stars.Add(star6);
				stars.Add(star7);
            }

			stars.Sort((s1, s2) => s1.Mag.CompareTo(s2.Mag));
			return stars[stars.Count / 2].Mag;
		}

        private void ImproveSolution(LeastSquareFittedAstrometry fit, double coeff, int i, int j, int k)
		{
			m_SolutionSolver = new PlateConstantsSolver(m_PlateConfig);

			double ra0, de0;
			fit.GetRADEFromImageCoords(m_PlateConfig.CenterXImage, m_PlateConfig.CenterXImage, out ra0, out de0);
			m_SolutionSolver.InitPlateSolution(ra0, de0);

            List<IStar> consideredStars = new List<IStar>();
            List<ulong> nonStellarStars = new List<ulong>();

			foreach (IStar star in m_CelestialAllStars)
			{
				if (star.Mag < m_MinMag || star.Mag > m_MaxMag)
					continue;

				if (m_DetermineAutoLimitMagnitude && 
					!double.IsNaN(m_DetectedLimitingMagnitude) &&
					star.Mag > m_DetectedLimitingMagnitude)
				{
#if ASTROMETRY_DEBUG
					Trace.Assert(false);
#endif
					continue;
				}

				double x, y;
				fit.GetImageCoordsFromRADE(star.RADeg, star.DEDeg, out x, out y);
				if (x < 0 || x > m_PlateConfig.ImageWidth ||
					y < 0 || y > m_PlateConfig.ImageHeight)
					continue;


				ImagePixel pixel = null;
				PSFFit psfFit;
				PSFFit asymPsfFit = null; 
				//if (m_FitSettings.CenterDetectionMethod == StarCenterDetection.PSFFit)
				{
					if (m_Settings.PyramidRemoveNonStellarObject)
						m_StarMap.GetPSFFit((int)x, (int)y, PSFFittingMethod.NonLinearAsymetricFit, out asymPsfFit);

					pixel = m_StarMap.GetPSFFit((int)x, (int)y, PSFFittingMethod.NonLinearFit, out psfFit);
				}
				//else if (m_FitSettings.CenterDetectionMethod == StarCenterDetection.Centroid)
				{
					// NOTE: Centroid detection is way faster and PSF will not lead to big improvement considering the threshold for star matching
					//pixel = m_StarMap.GetCentroid((int)x, (int)y, (int)Math.Ceiling(m_Settings.SearchArea));
				}

				if (pixel != null &&
                    psfFit.Certainty >= CorePyramidConfig.Default.MinDetectionLimitForSolutionImprovement / coeff)
				{
					consideredStars.Add(star);

					double distancePixels = fit.GetDistanceInPixels(pixel.XDouble, pixel.YDouble, x, y);
                    if (distancePixels < CorePyramidConfig.Default.MaxPreliminaryResidualForSolutionImprovementInPixels / coeff)
					{
#if ASTROMETRY_DEBUG
						Trace.Assert(!double.IsNaN(pixel.XDouble));
						Trace.Assert(!double.IsNaN(pixel.YDouble));
#endif

						if (
							Math.Sqrt((x - pixel.XDouble) * (x - pixel.XDouble) + (y - pixel.YDouble) * (y - pixel.YDouble)) >
                            CoreAstrometrySettings.Default.SearchArea)
							continue;

						pixel.SignalNoise = psfFit.Certainty;
						pixel.Brightness = psfFit.Brightness; 
						m_SolutionSolver.AddStar(pixel, star);

						if (m_Settings.PyramidRemoveNonStellarObject &&
							(
								asymPsfFit.FWHM < m_Settings.MinReferenceStarFWHM ||
								asymPsfFit.FWHM > m_Settings.MaxReferenceStarFWHM ||
								asymPsfFit.ElongationPercentage > m_Settings.MaximumPSFElongation)
							)
						{
							nonStellarStars.Add(star.StarNo);
						}
					}
				}
			}

			double ffl = fit.FitInfo.FittedFocalLength;
			m_ImprovedSolution = m_SolutionSolver.SolveWithLinearRegression(m_Settings, out m_FirstImprovedSolution);
            //if (m_ImprovedSolution != null && m_ImprovedSolution.FitInfo.AllStarPairs.Count < 12)
            //{
            //    // Attempt to reject errorous solutions with small number of fitted stars 
            //    int totalMatched = 0;
            //    var largeFeatures = m_StarMap.Features.Where(x => x.MaxBrightnessPixels > 1).ToList();
            //    if (largeFeatures.Count > 5)
            //    {
            //        foreach (var feature in largeFeatures)
            //        {
            //            var ftrCenter = feature.GetCenter();
            //            // TODO: PFS Fit on the feature?
            //            var matchedFittedStar = m_ImprovedSolution.FitInfo.AllStarPairs.FirstOrDefault(
            //                x =>
            //                    Math.Sqrt(Math.Pow(x.x - ftrCenter.XDouble, 2) + Math.Pow(x.x - ftrCenter.XDouble, 2)) <
            //                    2);
            //            if (matchedFittedStar != null) totalMatched++;
            //        }

            //        double percentLargeFeaturesMatched = 1.0*totalMatched/largeFeatures.Count;
            //        if (percentLargeFeaturesMatched < 0.75)
            //        {
            //            if (TangraConfig.Settings.TraceLevels.PlateSolving.TraceInfo())
            //                Trace.WriteLine(string.Format("Only {0:0.0}% ({1} features) of the large {2} features have been matched, where 75% is required.", percentLargeFeaturesMatched*100, totalMatched, largeFeatures.Count));
            //            // At least 75% of the bright features from the video need to be matched to stars for the solution to be accepted
            //            m_ImprovedSolution = null;
            //        }
            //    }
            //}
            
            if (m_ImprovedSolution != null)
			{
				m_ImprovedSolution.FitInfo.FittedFocalLength = ffl;

                if (TangraConfig.Settings.TraceLevels.PlateSolving.TraceWarning())
                    Trace.WriteLine(string.Format("Improved solution: {0} considered stars, UsedInSolution: {1}, ExcludedForHighResidual: {2}", 
                        m_ImprovedSolution.FitInfo.AllStarPairs.Count(), 
                        m_ImprovedSolution.FitInfo.AllStarPairs.Count(x => x.FitInfo.UsedInSolution),
                        m_ImprovedSolution.FitInfo.AllStarPairs.Count(x => x.FitInfo.ExcludedForHighResidual)));

				// Fit was successful, exclude all unused non stellar objects so they 
				// don't interfere with the included/excluded stars improved solution tests
				m_ImprovedSolution.FitInfo.AllStarPairs.RemoveAll(p =>
					(p.FitInfo.ExcludedForHighResidual || !p.FitInfo.UsedInSolution) &&
					nonStellarStars.Contains(p.StarNo));

				// NOTE: How excluding stars for FWHM/Elongation may lead to incorrectly accepted solutions that include small number of stars
				//       because the majority haven't been used for the fit. This is why we have another solution check here.
				if (m_ImprovedSolution.FitInfo.AllStarPairs.Count > 3)
				{
                    List<PlateConstStarPair> usedStarPairs = m_ImprovedSolution.FitInfo.AllStarPairs.Where(p => p.FitInfo.UsedInSolution).ToList();
                    double maxIncludedMag = usedStarPairs.Max(p => p.Mag);

                    int nonIncludedConsideredStars = consideredStars.Count(s => s.Mag <= maxIncludedMag) - usedStarPairs.Count;
                    if (nonIncludedConsideredStars > CorePyramidConfig.Default.MaxFWHMExcludedImprovemntStarsCoeff * usedStarPairs.Count)
                    {
                        LogUnsuccessfulFitImage(m_ImprovedSolution, i, j, k, 
                            string.Format("More than {0:0.0}% of the stars ({1} stars) down to mag {2:0.00} have not been matched. Attempted stars: {3}, Coeff: {4:0.00}", 
                                CorePyramidConfig.Default.MaxFWHMExcludedImprovemntStarsCoeff * 100,
                                nonIncludedConsideredStars,
                                maxIncludedMag, 
                                m_SolutionSolver.Pairs.Count, 
                                nonIncludedConsideredStars / m_SolutionSolver.Pairs.Count));
						m_ImprovedSolution = null;
						return;
					}

					List<double> intensities = usedStarPairs.Select(s => (double)s.Intensity).ToList();
					List<double> mags = usedStarPairs.Select(s => s.Mag).ToList();

                    if (usedStarPairs.Count > 3)
                    {
                        LinearRegression reg = new LinearRegression();
                        int pointsAdded = 0;
                        for (int ii = 0; ii < intensities.Count; ii++)
                        {
                            if (intensities[ii] > 0)
                            {
                                reg.AddDataPoint(intensities[ii], Math.Log10(mags[ii]));
                                pointsAdded++;
                            }
                        }

                        if (pointsAdded > 3)
                        {
                            reg.Solve();

                            if (Math.Pow(10, reg.StdDev) > CorePyramidConfig.Default.MagFitTestMaxStdDev || reg.A > CorePyramidConfig.Default.MagFitTestMaxInclination)
                            {
                                LogUnsuccessfulFitImage(m_ImprovedSolution, i, j, k, 
                                    string.Format("Failing solution for failed magnitude fit. Intensity(Log10(Magntude)) = {1} * x + {2}, StdDev = {0:0.0000}, ChiSquare = {3:0.000}", Math.Pow(10, reg.StdDev), reg.A, reg.B, reg.ChiSquare));

                                m_ImprovedSolution = null;
                                return;
                            }                            
                        }
                    }
				}
			}
		}

		private bool TryMatchingPairsFromPreviousFit(IStarMap starMap)
		{
			ThreeStarFit.StarPair[] pairs = new ThreeStarFit.StarPair[3];
			Dictionary<ImagePixel, IStar> matchedPairs = new Dictionary<ImagePixel, IStar>();
            Dictionary<int, ulong> matchedFeatureIdToStarIdIndexes = new Dictionary<int, ulong>();

            int matchingSearchDistance = Math.Max(3, (int)CoreAstrometrySettings.Default.SearchArea / 3);
			int idx = 0;
			foreach (PlateConstStarPair pair in m_PreviousFit.AllStarPairs)
			{
				if (pair.FitInfo.ExcludedForHighResidual) continue;
                StarMapFeature ftr = starMap.GetFeatureInRadius((int)pair.x, (int)pair.y, matchingSearchDistance);
			    if (ftr == null)
			    {
			        PSFFit psf;
                    IImagePixel psfFitCenter = starMap.GetPSFFit((int) pair.x, (int) pair.y, PSFFittingMethod.NonLinearFit, out psf);
                    if (psfFitCenter != null && psf != null &&
                        Math.Sqrt(Math.Pow(psfFitCenter.XDouble - pair.x, 2) + Math.Pow(psfFitCenter.YDouble - pair.y, 2)) <= matchingSearchDistance &&
                        psf.Certainty > CoreAstrometrySettings.Default.MinPreviousFramePlatesolveMatchingStarCertainty)
			        {
                        ftr = starMap.AppendFeature((int)pair.x, (int)pair.y);
			        }
			    }

				if (ftr != null)
				{
					ImagePixel center = starMap.GetCentroid(
                        (int)pair.x, (int)pair.y, (int)CoreAstrometrySettings.Default.SearchArea);
					IStar star = null;

					foreach (IStar s in m_CelestialPyramidStars)
					{
						if (s.StarNo == pair.StarNo)
						{
							star = s;
							break;
						}
					}

					if (star != null &&
						center != null &&
						!matchedFeatureIdToStarIdIndexes.ContainsKey(ftr.FeatureId) &&
                        !matchedPairs.ContainsKey(center) /* Close features may return ImagePixels with the same hash code, so make sure we haven't added one already */)
					{
						if (idx < 3)
						{
							pairs[idx] = new ThreeStarFit.StarPair(center.X, center.Y);
							pairs[idx].RADeg = star.RADeg;
							pairs[idx].DEDeg = star.DEDeg;
							pairs[idx].Star = star;
							idx++;
						}

						matchedPairs.Add(center, star);
						matchedFeatureIdToStarIdIndexes.Add(ftr.FeatureId, star.StarNo);
					}
				}
			}

			// Shortcurcuit to FeautreId - StarNo detection
			if (matchedPairs.Count >= m_Settings.MinimumNumberOfStars)
			{
                // When there was a previous fit and we have sufficient stars from the current star map
                // then don't require % of the bright features to approve the solution. Do it as a calibration fit (not too precise)
                LeastSquareFittedAstrometry fit = SolveStarPairs(
                    starMap, matchedPairs,
                    matchedFeatureIdToStarIdIndexes,
                    pairs[0], pairs[1], pairs[2], m_PreviousFit.FittedFocalLength, null, 
                    TangraConfig.Settings.Astrometry.MinimumNumberOfStars);

                if (fit != null)
                {
                    m_Solution = fit;
                    m_MatchedPairs = matchedPairs;
                    m_MatchedFeatureIdToStarIdIndexes = matchedFeatureIdToStarIdIndexes;
                    return true;
                }
            }

			return false;
		}


		private bool TryMatchingPairsFromManualPairs(IStarMap starMap)
		{
			ThreeStarFit.StarPair[] pairs = new ThreeStarFit.StarPair[3];
			Dictionary<ImagePixel, IStar> matchedPairs = new Dictionary<ImagePixel, IStar>();
            Dictionary<int, ulong> matchedFeatureIdToStarIdIndexes = new Dictionary<int, ulong>();

			int idx = 0;
			foreach (StarMapFeature feature in m_ManualPairs.Keys)
			{
				IStar star = m_ManualPairs[feature];
				ImagePixel center = feature.GetCenter();

                StarMapFeature ftr = starMap.GetFeatureInRadius((int)center.X, (int)center.Y, (int)CoreAstrometrySettings.Default.SearchArea);
				if (ftr != null)
				{
					if (!matchedFeatureIdToStarIdIndexes.ContainsKey(ftr.FeatureId))
					{
						if (idx < 3)
						{
							pairs[idx] = new ThreeStarFit.StarPair(center.X, center.Y);
							pairs[idx].RADeg = star.RADeg;
							pairs[idx].DEDeg = star.DEDeg;
							pairs[idx].Star = star;
							idx++;
						}

						matchedPairs.Add(center, star);
						matchedFeatureIdToStarIdIndexes.Add(ftr.FeatureId, star.StarNo);
					}
				}
			}

			if (matchedPairs.Count >= m_Settings.MinimumNumberOfStars)
			{
                // When there was a previous fit and we have sufficient stars from the current star map
                // then don't require % of the bright features to approve the solution. Do it as a calibration fit (not too precise)
                LeastSquareFittedAstrometry fit = SolveStarPairs(
                    starMap, matchedPairs,
                    matchedFeatureIdToStarIdIndexes,
                    pairs[0], pairs[1], pairs[2], m_PlateConfig.EffectiveFocalLength, null, 
                    TangraConfig.Settings.Astrometry.MinimumNumberOfStars);

                if (fit != null)
                {
                    m_Solution = fit;
                    m_MatchedPairs = matchedPairs;
                    m_MatchedFeatureIdToStarIdIndexes = matchedFeatureIdToStarIdIndexes;
                    return true;
                }
            }

			return false;
		}

		private bool CheckTriangle(IStarMap starMap, CheckTriangleCallback callback, int i, int j, int k, double toleranceInArcSec)
		{
			m_Feature_i = starMap.Features[i - 1];
			m_Feature_j = starMap.Features[j - 1];
			m_Feature_k = starMap.Features[k - 1];

			m_FeatureId_i = m_Feature_i.FeatureId;
			m_FeatureId_j = m_Feature_j.FeatureId;
			m_FeatureId_k = m_Feature_k.FeatureId;

			long idx_ij = (m_FeatureId_i << 32) + m_FeatureId_j;
			long idx_ik = (m_FeatureId_i << 32) + m_FeatureId_k;
			long idx_jk = (m_FeatureId_j << 32) + m_FeatureId_k;

			ImagePixel feature_i_Center = null;
			ImagePixel feature_j_Center = null;
			ImagePixel feature_k_Center = null;

			double dist_ij;
			if (!m_FeaturesDistanceCache.TryGetValue(idx_ij, out dist_ij))
			{
				if (feature_i_Center == null) feature_i_Center = GetCenterOfFeature(starMap.GetFeatureById((int)m_FeatureId_i), starMap);
				if (feature_j_Center == null) feature_j_Center = GetCenterOfFeature(starMap.GetFeatureById((int)m_FeatureId_j), starMap);

				dist_ij = m_PlateConfig.GetDistanceInArcSec(feature_i_Center.X, feature_i_Center.Y, feature_j_Center.X, feature_j_Center.Y);
				long idx_ji = (m_FeatureId_j << 32) + m_FeatureId_i;

				m_FeaturesDistanceCache.Add(idx_ij, dist_ij);
				m_FeaturesDistanceCache.Add(idx_ji, dist_ij);
			}

			double dist_ik;
			if (!m_FeaturesDistanceCache.TryGetValue(idx_ik, out dist_ik))
			{
				if (feature_i_Center == null) feature_i_Center = GetCenterOfFeature(starMap.GetFeatureById((int)m_FeatureId_i), starMap);
				if (feature_k_Center == null) feature_k_Center = GetCenterOfFeature(starMap.GetFeatureById((int)m_FeatureId_k), starMap);

				dist_ik = m_PlateConfig.GetDistanceInArcSec(feature_i_Center.X, feature_i_Center.Y, feature_k_Center.X, feature_k_Center.Y);
				long idx_ki = (m_FeatureId_k << 32) + m_FeatureId_i;

				m_FeaturesDistanceCache.Add(idx_ik, dist_ik);
				m_FeaturesDistanceCache.Add(idx_ki, dist_ik);
			}

			double dist_jk;
			if (!m_FeaturesDistanceCache.TryGetValue(idx_jk, out dist_jk))
			{
				if (feature_j_Center == null) feature_j_Center = GetCenterOfFeature(starMap.GetFeatureById((int)m_FeatureId_j), starMap);
				if (feature_k_Center == null) feature_k_Center = GetCenterOfFeature(starMap.GetFeatureById((int)m_FeatureId_k), starMap);

				dist_jk = m_PlateConfig.GetDistanceInArcSec(feature_j_Center.X, feature_j_Center.Y, feature_k_Center.X, feature_k_Center.Y);
				long idx_kj = (m_FeatureId_k << 32) + m_FeatureId_j;

				m_FeaturesDistanceCache.Add(idx_jk, dist_jk);
				m_FeaturesDistanceCache.Add(idx_kj, dist_jk);
			}

			if (callback(i, j, k, dist_ij, dist_ik, dist_jk, toleranceInArcSec))
				// solution found
				return true;

			return false;
		}

		private bool CheckTriangleWithRatios(IStarMap starMap, CheckTriangleWithRatiosCallback callback, int i, int j, int k, double toleranceInArcSec)
		{
			m_Feature_i = starMap.Features[i - 1];
			m_Feature_j = starMap.Features[j - 1];
			m_Feature_k = starMap.Features[k - 1];

			m_FeatureId_i = m_Feature_i.FeatureId;
			m_FeatureId_j = m_Feature_j.FeatureId;
			m_FeatureId_k = m_Feature_k.FeatureId;

			long idx_ij = (m_FeatureId_i << 32) + m_FeatureId_j;
			long idx_ik = (m_FeatureId_i << 32) + m_FeatureId_k;
			long idx_jk = (m_FeatureId_j << 32) + m_FeatureId_k;

			ImagePixel feature_i_Center = null;
			ImagePixel feature_j_Center = null;
			ImagePixel feature_k_Center = null;

			double dist_ij;
			if (!m_FeaturesDistanceCache.TryGetValue(idx_ij, out dist_ij))
			{
				if (feature_i_Center == null) feature_i_Center = GetCenterOfFeature(starMap.GetFeatureById((int)m_FeatureId_i), starMap);
				if (feature_j_Center == null) feature_j_Center = GetCenterOfFeature(starMap.GetFeatureById((int)m_FeatureId_j), starMap);
			}

			double dist_ik;
			if (!m_FeaturesDistanceCache.TryGetValue(idx_ik, out dist_ik))
			{
				if (feature_i_Center == null) feature_i_Center = GetCenterOfFeature(starMap.GetFeatureById((int)m_FeatureId_i), starMap);
				if (feature_k_Center == null) feature_k_Center = GetCenterOfFeature(starMap.GetFeatureById((int)m_FeatureId_k), starMap);
			}

			double dist_jk;
			if (!m_FeaturesDistanceCache.TryGetValue(idx_jk, out dist_jk))
			{
				if (feature_j_Center == null) feature_j_Center = GetCenterOfFeature(starMap.GetFeatureById((int)m_FeatureId_j), starMap);
				if (feature_k_Center == null) feature_k_Center = GetCenterOfFeature(starMap.GetFeatureById((int)m_FeatureId_k), starMap);
			}

			if (callback(i, j, k, feature_i_Center, feature_j_Center, feature_k_Center, toleranceInArcSec))
				// solution found
				return true;

			return false;			
		}

		#region IDisposable Members

		public void Dispose()
		{
            m_OperationNotifier.Unsubscribe(this);
		}

		#endregion

		#region INotificationReceiver Members

		public void ReceieveMessage(object notification)
		{
			var opNot = notification as OperationNotifications;

			if (opNot != null && opNot.Notification == NotificationType.AbortSearch)
			{
				m_AbortSearch = true;
			}
		}

		#endregion
	}
}
