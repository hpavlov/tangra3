using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Model.Helpers;
using Tangra.Model.Image;


namespace Tangra.Model.Astro
{
	public class StarMapException : Exception
	{
		public StarMapException(string message)
			: base(message)
		{ }
	}

	[Serializable]
	public class StarMap : StarMapBase, IStarMap
	{
		private StarMapInternalConfig m_Config;

		private List<int> m_CheckedIndexes = new List<int>();
		private List<StarMapFeature> m_Features = new List<StarMapFeature>();

		private Rectangle m_OSDFrameToExclude = Rectangle.Empty;
		private Rectangle m_FrameToInclude = Rectangle.Empty;
		private bool m_LimitByInclusion = false;
		private Dictionary<int, int> m_IndexToFeatureIdMap = new Dictionary<int, int>();
		private int m_FeatureId = 0;

		protected double m_MinFeatureFWHM = 1.8;
		protected double m_MaxFeatureFWHM = 6.0;
		protected double m_MaxPSFElongationPercentage = 25;
		protected double m_MinFeatureCertainty = 0.75;
		protected bool m_ForceStellarObjectRequirements = false;

		Stopwatch m_PerformanceWatch = new Stopwatch();

		public double TotalSeconds
		{
			get { return m_PerformanceWatch.Elapsed.TotalSeconds; }
		}

		public StarMapInternalConfig Config
		{
			get { return m_Config; }
		}

		public StarMap()
		{ }

		public StarMap(bool forceStellarObjectRequirements, double minFWHM, double maxFWHM, double maxElongation, double minCertainty)
		{
			m_ForceStellarObjectRequirements = forceStellarObjectRequirements;
			m_MinFeatureFWHM = minFWHM;
			m_MaxFeatureFWHM = maxFWHM;
			m_MaxPSFElongationPercentage = maxElongation;
			m_MinFeatureCertainty = minCertainty;
		}

		public int FindBestMap(
			StarMapInternalConfig config, AstroImage image,
			Rectangle osdFrameToExclude, Rectangle frameToInclude, bool limitByInclusion)
		{
			m_Config = config;

			m_PerformanceWatch.Reset();
			m_PerformanceWatch.Start();

			m_FullWidth = image.Width;
			m_FullHeight = image.Height;
			m_Pixelmap = image.Pixelmap;

			m_OSDFrameToExclude = osdFrameToExclude;
			m_FrameToInclude = frameToInclude;
			m_LimitByInclusion = limitByInclusion;

			int optimumStarsInField = config.OptimumStarsInField ?? (TangraConfig.Settings != null ? (int)TangraConfig.Settings.Astrometry.PyramidOptimumStarsToMatch : 25);
			uint maxSignalValue = config.CustomMaxSignalValue ?? m_Pixelmap.MaxSignalValue;
			
			m_AverageBackgroundNoise = maxSignalValue;
			
			int MIN_INTENSITY = config.IsFITSFile ? 1 : 5;
			int MAX_INTENSITY = config.IsFITSFile ? 1000 : 100;
			int INTENSITY_LARGE_STEP = config.IsFITSFile ? 50 : 5;
			int INTENSITY_SMALL_STEP = config.IsFITSFile ? 2 : 5;
			if (config.CustomOptimumStarsValue.HasValue) optimumStarsInField = config.CustomOptimumStarsValue.Value;

			double snRatio = config.IsFITSFile ? 0.005 : 0.15;
			try
			{
				int featuresThisRun = 0;
				int featuresLastRun = 0;
				bool tryForOptimumStars = optimumStarsInField > config.MinStarsInField;

				List<StarMapFeature> bestMapFeatures = new List<StarMapFeature>();

				int intensity = MAX_INTENSITY;
				int tolerance = config.StarMapperTolerance;
				InitCreateMap();
				do
				{
					if (intensity > 2 * INTENSITY_LARGE_STEP) intensity -= INTENSITY_LARGE_STEP;
					else intensity -= INTENSITY_SMALL_STEP;

					uint backgroundThreshold = (uint)Math.Round(maxSignalValue * (intensity * 1.0 / MAX_INTENSITY));

					if (backgroundThreshold < config.MinStarMapThreashold) break;
					if (backgroundThreshold < snRatio * (maxSignalValue - m_AverageBackgroundNoise)) break;

					try
					{
						CreateMap(backgroundThreshold, config);
					}
					catch (StarMapException)
					{
						backgroundThreshold = (uint)Math.Round(maxSignalValue * ((intensity + INTENSITY_LARGE_STEP) * 1.0 / MAX_INTENSITY));
						snRatio = (double)(backgroundThreshold - 1.0) / (maxSignalValue - m_AverageBackgroundNoise);
						intensity += 2 * INTENSITY_LARGE_STEP;
						continue;
					}

					if (m_Features.Count > 2.5 * optimumStarsInField)
					{
						m_Features.Sort((x, y) => y.PixelCount.CompareTo(x.PixelCount));
						m_Features = m_Features.Take(2 * optimumStarsInField).ToList();
					}

					featuresLastRun = featuresThisRun;
					featuresThisRun = m_Features.Count;

					if (tryForOptimumStars)
					{
						if (
							featuresLastRun <= optimumStarsInField &&
							featuresThisRun >= optimumStarsInField &&
							featuresLastRun > 0.5 * optimumStarsInField &&
							featuresThisRun < 1.5 * optimumStarsInField)
						{
							if (optimumStarsInField - featuresLastRun <=
								featuresThisRun - optimumStarsInField)
							{
								break;
							}
							else
							{
								bestMapFeatures.Clear();
								bestMapFeatures.AddRange(m_Features);
								break;
							}	
						}
						else
						{
							if (featuresThisRun >= 1.5 * optimumStarsInField)
								// We couldn't fit in the optimum star range, so not trying to any more
								tryForOptimumStars = false;
						}
					}

					bestMapFeatures.Clear();
					bestMapFeatures.AddRange(m_Features);

					if (featuresThisRun > config.MaxStarsInField)
					{
							break;
					}

					if (m_Features.Count > config.MinStarsInField &&
						featuresThisRun > featuresLastRun)
					{
						bool allNewFeaturesHaveOnePixel = true;
						foreach (StarMapFeature feature in m_Features)
						{
							if (feature.Generation == 1 &&
								feature.m_Pixels.Count > 1)
							{
								allNewFeaturesHaveOnePixel = false;
								break;
							}
						}

						if (allNewFeaturesHaveOnePixel)
						{
							tolerance--;

							if (tolerance <= 0 && !tryForOptimumStars)
								break;
						}

					}

					m_NoiseLevel = backgroundThreshold;
				}
				while (intensity > MIN_INTENSITY);

				if (m_ForceStellarObjectRequirements) 
					ApplyPSFExclusionRulesToFeatures();				

				return intensity;
			}
			finally
			{
				m_PerformanceWatch.Stop();

                if (TangraConfig.Settings.TraceLevels.PlateSolving.TraceVerbose())
				    Trace.WriteLine(string.Format("StarMapGen: {0} ms, {1} features, {2} generations (tolerance = {3})",
					    m_PerformanceWatch.ElapsedMilliseconds,
					    m_Features.Count, m_Features.Count > 0 ? m_Features[0].Generation : 0,
					    config.StarMapperTolerance));

				int lastGenMin = int.MaxValue;
				int thisGenMin = int.MaxValue;
				int lastGen = int.MaxValue;
				for (int i = 0; i < m_Features.Count; i++)
				{
					int currMax = int.MinValue;
					StarMapFeature ftr = m_Features[i];
					foreach (int intens in ftr.m_Pixels.Values)
						if (intens > currMax) currMax = intens;

#if ASTROMETRY_DEBUG
					Trace.Assert(lastGen >= ftr.Generation);
#endif

					if (lastGen > ftr.Generation)
					{
						lastGen = ftr.Generation;
						lastGenMin = thisGenMin;
					}

#if ASTROMETRY_DEBUG
					Trace.Assert(currMax < lastGenMin);
#endif

					thisGenMin = currMax;
				}

				ReorderFeatureNumbersBasedOnIntensity();
			}
		}

		private void ApplyPSFExclusionRulesToFeatures()
		{
		    try
		    {
                List<int> rejectedFeatureIds = new List<int>();

                foreach (StarMapFeature feature in m_Features)
                {
					ImagePixel center = feature.GetCenter();
                    PSFFit fit;
                    if (center != null)
                    {
                        GetPSFFit(center.X, center.Y, PSFFittingMethod.NonLinearAsymetricFit, out fit);
                        bool isGoodPsf =
                            fit.Certainty >= m_MinFeatureCertainty &&
                            fit.FWHM >= m_MinFeatureFWHM &&
                            fit.FWHM <= m_MaxFeatureFWHM &&
                            fit.ElongationPercentage < m_MaxPSFElongationPercentage;


#if ASTROMETRY_DEBUG
                        Trace.WriteLine(string.Format("{0} Feature: ({1}, {2}) Certainty = {3:0.00} FWHM = {4:0.0} Elongation = {5:0}%",
                            isGoodPsf ? "Accepted" : "Rejected", center.X, center.Y, fit.Certainty, fit.FWHM, fit.ElongationPercentage));
#endif

                        if (!isGoodPsf)
                        {
                            rejectedFeatureIds.Add(feature.FeatureId);
                        }
                    }
                }

                if (rejectedFeatureIds.Count > 0)
                {
                    m_Features.RemoveAll(f => rejectedFeatureIds.Contains(f.FeatureId));
                    m_Features.Sort((a, b) => a.FeatureId.CompareTo(b.FeatureId));

                    for (int i = 0; i < m_Features.Count; i++)
                        m_Features[i].FixFeatureIdAfterSetExcusion(i);
                }
		    }
		    catch (Exception ex)
		    {
		        Trace.WriteLine(ex);
		    }
		}

		private void ReorderFeatureNumbersBasedOnIntensity()
		{
			m_Features.Sort((a, b) => b.Intensity.CompareTo(a.Intensity));
			int newIdx = 0;
			foreach (StarMapFeature feature in m_Features)
			{
				feature.FixFeatureIdAfterSetExcusion(newIdx);
				newIdx++;
			}
		}

		public void InitializeStarMapButDontProcess(StarMapInternalConfig config, AstroImage image, Rectangle osdFrameToExclude, Rectangle frameToInclude, bool limitByInclusion)
		{
			m_FullWidth = image.Width;
			m_FullHeight = image.Height;
			m_Pixelmap = image.Pixelmap;
			m_OSDFrameToExclude = osdFrameToExclude;
			m_FrameToInclude = frameToInclude;
			m_LimitByInclusion = limitByInclusion;

			try
			{
				m_AverageBackgroundNoise = 0;
				InitCreateMap();
			}
			finally
			{
				m_PerformanceWatch.Stop();
			}
		}

		internal void CreateMap(AstroImage image, Rectangle frame, StarMapInternalConfig config)
		{
			m_FullWidth = image.Width;
			m_FullHeight = image.Height;
			m_Pixelmap = image.Pixelmap;

			InitCreateMap();
			CreateMap(config);
		}

		internal void CopyMap(StarMap copyFrom)
		{
			m_FullWidth = copyFrom.m_FullWidth;
			m_FullHeight = copyFrom.m_FullHeight;

			m_CheckedIndexes.Clear();
			m_CheckedIndexes.AddRange(copyFrom.m_CheckedIndexes);

			m_Features.Clear();
			foreach (StarMapFeature feature in copyFrom.m_Features) m_Features.Add((StarMapFeature)feature.Clone());

			m_OSDFrameToExclude = new Rectangle(copyFrom.m_OSDFrameToExclude.Left, copyFrom.m_OSDFrameToExclude.Top, copyFrom.m_OSDFrameToExclude.Width, copyFrom.m_OSDFrameToExclude.Height);
			m_FrameToInclude = new Rectangle(copyFrom.m_FrameToInclude.Left, copyFrom.m_FrameToInclude.Top, copyFrom.m_FrameToInclude.Width, copyFrom.m_FrameToInclude.Height);
			m_LimitByInclusion = copyFrom.m_LimitByInclusion;

			m_IndexToFeatureIdMap.Clear();
			foreach (int key in copyFrom.m_IndexToFeatureIdMap.Keys) m_IndexToFeatureIdMap.Add(key, copyFrom.m_IndexToFeatureIdMap[key]);

			m_FeatureId = copyFrom.m_FeatureId;
		}

		private void InitCreateMap()
		{
			m_Features.Clear();
			m_IndexToFeatureIdMap.Clear();
			m_FeatureId = 0;
		}

		private void CreateMap(StarMapInternalConfig config)
		{
			CreateMap(0, config);
		}

		private void CreateMap(uint maxIntensity, StarMapInternalConfig config)
		{
			m_CheckedIndexes.Clear();

			m_AverageBackgroundNoise = 0;
			int bgCount = 0;

			try
			{
				for (int i = 0; i < m_Pixelmap.Pixels.Length; i ++)
				{
					if (m_Pixelmap.Pixels[i] < maxIntensity)
					{
						m_AverageBackgroundNoise += m_Pixelmap.Pixels[i];
						bgCount++;
						continue;
					}

					if (m_CheckedIndexes.IndexOf(i) == -1)
					{
						if (m_FeatureId > config.MaxStarsInField)
							return;

						int y = i / m_FullWidth;
						int x = (i % m_FullWidth);

						if (!PixelIsInProcessArea(x, y)) continue;

						StarMapFeature feature = new StarMapFeature(m_FeatureId, m_Pixelmap.Width);

						if (!CheckFeature(feature, config, x, y, maxIntensity))
						{
#if ASTROMETRY_DEBUG
							Trace.Assert(feature.m_Pixels.Count > 0);
#endif

							m_FeatureId++;
							m_Features.Add(feature);
						}
						else
						{
							m_AverageBackgroundNoise += m_Pixelmap.Pixels[i];
							bgCount++;
						}
					}
				}
			}
			finally
			{
				// a "Too many features" exception may be thrown
				if (bgCount != 0)
					m_AverageBackgroundNoise /= (uint)bgCount;

#if ASTROMETRY_DEBUG
				Trace.Assert(m_AverageBackgroundNoise >= 0);
				Trace.Assert(m_AverageBackgroundNoise <= m_Pixelmap.MaxPixelValue);
#endif

				foreach (StarMapFeature feature in m_Features)
					feature.m_Generation++;
			}
		}

		public uint NoiseLevel
		{
			get { return m_NoiseLevel; }
		}

		public StarMapFeature GetFeatureById(int feautreId)
		{
			foreach (StarMapFeature feature in m_Features)
			{
				if (feature.FeatureId == feautreId)
					return feature;
			}

			throw new IndexOutOfRangeException(string.Format("Image feature {0} cannot be found in the current feature map.", feautreId));
		}

		private bool PixelIsInProcessArea(int x, int y)
		{
			// Outside the full frame
			if (x < 0 || y < 0 || x >= m_FullWidth || y >= m_FullHeight) 
				return false;

			if (m_LimitByInclusion)
				// Include pixels inside the defined area
				return m_FrameToInclude.Contains(x, y);
			else
				// Inside the OSD frame so don't include
				if (m_OSDFrameToExclude.Contains(x, y))
					return false;

			return true;
		}

		private Queue<ulong> m_PointsToCheckFurther = new Queue<ulong>();

		private bool CheckFeature(StarMapFeature feature, StarMapInternalConfig config, int x0, int y0, uint maxIntensity)
		{
			// The found pixel is already part of another feature
			int existingFeatureId;
			int idx0 = m_Pixelmap.Width * y0 + x0;
			if (m_IndexToFeatureIdMap.TryGetValue(idx0, out existingFeatureId))
			{
				return true;
			}

			// Find the closest peak pixel
			FindLocalPeak(ref x0, ref y0);

			idx0 = m_Pixelmap.Width * y0 + x0;
			if (m_IndexToFeatureIdMap.TryGetValue(idx0, out existingFeatureId))
			{
				return true;
			}

			m_PointsToCheckFurther.Clear();
			m_PointsToCheckFurther.Enqueue(((ulong)x0 << 32) + (ulong)y0);

			while (m_PointsToCheckFurther.Count > 0)
			{
				if (feature.PixelCount > config.MaxPixelsInFeature)
					break;

				ulong point = m_PointsToCheckFurther.Dequeue();
				int x = (int)(point >> 32);
				int y = (int)(point & 0xFFFFFFFF);

				if (!PixelIsInProcessArea(x, y)) continue;

				int idx = m_Pixelmap.Width * y + x;

				if (m_CheckedIndexes.IndexOf(idx) == -1)
				{
					m_CheckedIndexes.Add(idx);

					if (m_IndexToFeatureIdMap.TryGetValue(idx, out existingFeatureId))
					{
						// Existing feature already contains this pixel. 
					}
					else
					{
						uint pixel = m_Pixelmap.Pixels[idx];
						feature.AddPixel(x, y, pixel);
						m_IndexToFeatureIdMap.Add(idx, feature.FeatureId);

						for (int i = -1; i <= 1; i++)
						{
							for (int j = -1; j <= 1; j++)
							{
								if (!(i == 0 && j == 0))
								{
									if (Math.Abs(y0 - y - j) <= config.FeatureSearchRadius &&
										Math.Abs(x0 - x - i) <= config.FeatureSearchRadius)
									{
										idx = m_Pixelmap.Width * (y + j) + (x + i);

										if (idx >= 0 && idx < m_Pixelmap.Pixels.Length)
										{
											if (m_Pixelmap.Pixels[idx] <= pixel &&
												m_CheckedIndexes.IndexOf(idx) == -1)
											{
												m_PointsToCheckFurther.Enqueue(((ulong)(x + i) << 32) + (ulong)(y + j));
											}
										}
									}
								}
							}
						}
					}
				}
			}

			return false;
		}

		private void FindLocalPeak(ref int x0, ref int y0)
		{
			uint maxPixel = m_Pixelmap[x0, y0];
			bool newPeak;
			do
			{
				int x = x0;
				int y = y0;
				newPeak = false;

				newPeak |= TestMaxPixelAndSet(ref x0, ref y0, ref maxPixel, x - 1, y - 1);
				newPeak |= TestMaxPixelAndSet(ref x0, ref y0, ref maxPixel, x, y - 1);
				newPeak |= TestMaxPixelAndSet(ref x0, ref y0, ref maxPixel, x + 1, y - 1);

				newPeak |= TestMaxPixelAndSet(ref x0, ref y0, ref maxPixel, x - 1, y);
				newPeak |= TestMaxPixelAndSet(ref x0, ref y0, ref maxPixel, x + 1, y);

				newPeak |= TestMaxPixelAndSet(ref x0, ref y0, ref maxPixel, x - 1, y + 1);
				newPeak |= TestMaxPixelAndSet(ref x0, ref y0, ref maxPixel, x, y + 1);
				newPeak |= TestMaxPixelAndSet(ref x0, ref y0, ref maxPixel, x + 1, y + 1);
			}
			while (newPeak);
		}

		private bool TestMaxPixelAndSet(ref int x0, ref int y0, ref uint maxPixel, int x, int y)
		{
			if (!PixelIsInProcessArea(x, y)) 
				return false;

			uint thisPixel = m_Pixelmap[x, y];
			if (thisPixel > maxPixel)
			{
				maxPixel = thisPixel;
				y0 = y;
				x0 = x;
				return true;
			}

			return false;
		}

		public int FeaturesCount
		{
			get { return m_Features.Count; }
		}

		public List<StarMapFeature> Features
		{
			get { return m_Features; }
		}

		public StarMapFeature this[int x, int y]
		{
			get
			{
				foreach (StarMapFeature feature in m_Features)
				{
					if (feature[x, y] != 0)
						return feature;
				}

				return null;
			}
		}

		public override StarMapFeature GetFeatureInRadius(int x0, int y0, int radius)
		{
			List<StarMapFeature> candidates = new List<StarMapFeature>();

			for (int i = -radius; i <= radius; i++)
			{
				for (int j = -radius; j <= radius; j++)
				{
					int y = y0 + i;
					int x = x0 + j;
					if (x < 0 || x > m_FullWidth) continue;
					if (y < 0 || y > m_FullHeight) continue;

					ulong key = (ulong)(y * m_Pixelmap.Width + x);

					foreach (StarMapFeature feature in m_Features)
					{
						if (candidates.IndexOf(feature) != -1) continue;

						if (feature.m_Pixels.ContainsKey(key))
						{
							candidates.Add(feature);
							continue;
						}
					}
				}
			}

			StarMapFeature brightestFeature = null;

			foreach (StarMapFeature feature in candidates)
			{
				if (brightestFeature == null ||
					feature.PixelCount > brightestFeature.PixelCount)
				{
					brightestFeature = feature;
				}
			}

			return brightestFeature;
		}

	    public List<StarMapFeature> GetFeaturesInRadius(int x0, int y0, int radius)
	    {
            var rv = new List<StarMapFeature>();

            for (int i = -radius; i <= radius; i++)
            {
                for (int j = -radius; j <= radius; j++)
                {
                    int y = y0 + i;
                    int x = x0 + j;
                    if (x < 0 || x > m_FullWidth) continue;
                    if (y < 0 || y > m_FullHeight) continue;

                    ulong key = (ulong)(y * m_Pixelmap.Width + x);

                    foreach (StarMapFeature feature in m_Features)
                    {
                        if (rv.IndexOf(feature) != -1) continue;

                        if (feature.m_Pixels.ContainsKey(key))
                        {
                            rv.Add(feature);
                            continue;
                        }
                    }
                }
            }

	        return rv;
	    }

		public Pixelmap GetPixelmap()
		{
			return m_Pixelmap;
		}

		public Bitmap GetDisplayBitmap()
		{
			return m_Pixelmap.CreateDisplayBitmapDoNotDispose();
        }

		public static T GetPropValue<T>(object reflObj, string propName)
		{
			FieldInfo fi = reflObj.GetType().GetField(propName, BindingFlags.NonPublic | BindingFlags.Instance);
			if (fi == null)
			{
				fi = reflObj.GetType().GetField(propName, BindingFlags.Public | BindingFlags.Instance);

				if (fi != null)
					return (T) fi.GetValue(reflObj);
				else
				{
					PropertyInfo pi = reflObj.GetType().GetProperty(propName, BindingFlags.NonPublic | BindingFlags.Instance);
					if (pi == null)
						pi = reflObj.GetType().GetProperty(propName, BindingFlags.Public | BindingFlags.Instance);

					return (T)pi.GetValue(reflObj, null);					
				}
			}
			else
			{
				return (T)fi.GetValue(reflObj);
			}
		}

		internal static void SetPropValue<T>(object reflObj, string propName, T objVal)
		{
			FieldInfo fi = reflObj.GetType().GetField(propName, BindingFlags.NonPublic | BindingFlags.Instance);
			if (fi == null)
			{
				fi = reflObj.GetType().GetField(propName, BindingFlags.Public | BindingFlags.Instance);

				if (fi != null)
				{
					fi.SetValue(reflObj, objVal);
				}
				else
				{
					PropertyInfo pi = reflObj.GetType().GetProperty(propName, BindingFlags.NonPublic | BindingFlags.Instance);
					if (pi == null)
						pi = reflObj.GetType().GetProperty(propName, BindingFlags.Public | BindingFlags.Instance);

					pi.SetValue(reflObj, objVal, null);
				}
			}
			else
			{
				fi.SetValue(reflObj, objVal);
			}
		}
		
		public static StarMap FromReflectedObject(object reflObj)
		{
			var starMap = new StarMap();

			//starMap.m_Stride = GetPropValue<int>(reflObj, "m_Stride");
			//starMap.m_BytesPerPixel = GetPropValue<int>(reflObj, "m_BytesPerPixel");
			//starMap.m_FullWidth = GetPropValue<int>(reflObj, "m_FullWidth");
			//starMap.m_FullHeight = GetPropValue<int>(reflObj, "m_FullHeight");
			//starMap.m_TotalBytesCount = GetPropValue<int>(reflObj, "m_TotalBytesCount");

			//starMap.m_PixelData = GetPropValue<byte[,]>(reflObj, "m_PixelData");
			//starMap.m_CheckedIndexes = GetPropValue<List<int>>(reflObj, "m_CheckedIndexes");

			//starMap.m_OSDFrameToExclude = GetPropValue<Rectangle>(reflObj, "m_OSDFrameToExclude");
			//starMap.m_IndexToFeatureIdMap = GetPropValue<Dictionary<int, int>>(reflObj, "m_IndexToFeatureIdMap");

			//starMap.m_FeatureId = GetPropValue<int>(reflObj, "m_FeatureId");
			//starMap.m_ThumbPrint = GetPropValue<long>(reflObj, "m_ThumbPrint");
			//starMap.m_AverageBackgroundNoise = GetPropValue<int>(reflObj, "m_AverageBackgroundNoise");
			//starMap.m_NoiseLevel = GetPropValue<int>(reflObj, "m_NoiseLevel");

			//starMap.m_Features = new List<StarMapFeature>();

			//object featureList = GetPropValue<object>(reflObj, "m_Features");
			//IList lst = featureList as IList;
			//foreach(object el in lst)
			//{
			//    starMap.m_Features.Add(StarMapFeature.FromReflectedObject(el));
			//}
			
			return starMap;
		}
	}
}
