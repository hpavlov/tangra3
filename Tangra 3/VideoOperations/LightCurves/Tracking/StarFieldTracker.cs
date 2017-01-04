using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Helpers;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Model.Helpers;
using Tangra.Model.Image;
using Tangra.Model.VideoOperations;
using Tangra.VideoOperations.Astrometry.Engine;

namespace Tangra.VideoOperations.LightCurves.Tracking
{
	internal class StarFieldTracker : ITracker
	{
		private List<ITrackedObject> m_TrackedObjects = new List<ITrackedObject>();
		private Dictionary<int, TrackedObject> TrackedObjectsByTargetId = new Dictionary<int, TrackedObject>();
		private List<List<double>> m_PivotDistances = new List<List<double>>();

		private Dictionary<int, List<double>> m_TrackedObjectsPivotDistancesX = new Dictionary<int, List<double>>();
		private Dictionary<int, List<double>> m_TrackedObjectsPivotDistancesY = new Dictionary<int, List<double>>();

		private List<List<double>> m_TargetPivotDistancesListX = new List<List<double>>();
		private List<List<double>> m_TargetPivotDistancesListY = new List<List<double>>();

		private float m_FWHMAverage;
		private float m_FWHM1;
		private float m_FWHM2;
		private float m_FWHM3;
		private float m_FWHM4;

		double[] UNINITIALIZED_DISTANCES = new double[] { double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN };

		private bool m_IsTrackedSuccessfully;
		private StarMapInternalConfig m_StarMapConfig;

		public StarFieldTracker(List<TrackedObjectConfig> measuringStars)
		{			
			int i = -1;
			foreach (var originalObject in measuringStars)
			{
				i++;
				TrackedObject trackedObject = new TrackedObject((byte)i, originalObject);
				TrackedObjects.Add(trackedObject);
				TrackedObjectsByTargetId.Add(trackedObject.TargetNo, trackedObject);
			}
		}

		public bool IsTrackedSuccessfully
		{
			get { return m_IsTrackedSuccessfully; }
		}

		public bool InitializeNewTracking(IAstroImage astroImage)
		{
			m_StarMapConfig = new StarMapInternalConfig(StarMapInternalConfig.Default);
			m_StarMapConfig.CustomMaxSignalValue = astroImage.GetPixelmapPixels().Max();
			m_StarMapConfig.CustomOptimumStarsValue = 25;
			m_StarMapConfig.IsFITSFile = true;			

			StarMap starMap = new StarMap();
			starMap.FindBestMap(
							m_StarMapConfig,
							(AstroImage)astroImage,
							Rectangle.Empty,
							new Rectangle(0, 0, astroImage.Width, astroImage.Height), 
							AstrometryContext.Current.LimitByInclusion);

			if (starMap.Features.Count < 10)
			{
				MessageBox.Show("Cannot initialize object tracking as less than 10 stars can be identified in the field");
				return false;				
			}

			// Build a signature of the largest 10 features (pivots)
			m_PivotDistances.Clear();
			for (int i = 0; i < 10; i++) m_PivotDistances.Add(new List<double>(UNINITIALIZED_DISTANCES));

			double fwhmSum = 0;
			int fwhmCount = 0;

			starMap.Features.Sort((x, y) => y.PixelCount.CompareTo(x.PixelCount));
			for (int i = 0; i < 10; i++)
			{
				var feature_i = starMap.Features[i];
				for (int j = i + 1; j < 10; j++)
				{
					var feature_j = starMap.Features[j];
					double distance = feature_j.GetCenter().DistanceTo(feature_i.GetCenter());
					m_PivotDistances[i][j] = distance;
					m_PivotDistances[j][i] = distance;
				}

				int x0 = feature_i.GetCenter().X;
				int y0 = feature_i.GetCenter().Y;
				PSFFit fit = new PSFFit((int) x0, (int) y0);
				uint[,] data = ((AstroImage) astroImage).GetMeasurableAreaPixels((int) x0, (int) y0);
				fit.Fit(data);

				if (fit.IsSolved)
				{
					fwhmSum += fit.FWHM;
					fwhmCount++;
				}
			}

			m_FWHMAverage = (float)(fwhmSum / fwhmCount);

			for (int i = 0; i < m_TrackedObjects.Count; i++)
			{
				m_TrackedObjectsPivotDistancesX[i] = new List<double>();
				m_TrackedObjectsPivotDistancesY[i] = new List<double>();
				for (int j = 0; j < 10; j++)
				{
					m_TrackedObjectsPivotDistancesX[i].Add(m_TrackedObjects[i].Center.XDouble - starMap.Features[j].GetCenter().XDouble);
					m_TrackedObjectsPivotDistancesY[i].Add(m_TrackedObjects[i].Center.YDouble - starMap.Features[j].GetCenter().YDouble);
				}

				int x0 = m_TrackedObjects[i].Center.X;
				int y0 = m_TrackedObjects[i].Center.Y;
				PSFFit fit = new PSFFit((int)x0, (int)y0);
				uint[,] data = ((AstroImage)astroImage).GetMeasurableAreaPixels((int)x0, (int)y0);
				fit.Fit(data);

				if (fit.IsSolved)
				{
					SetTargetFWHM(i, (float)fit.FWHM);
				}
			}
		
			m_TargetPivotDistancesListX.Clear();
			m_TargetPivotDistancesListY.Clear();

			return true;
		}

		private void SetTargetFWHM(int targetId, float fwhm)
		{
			switch (targetId)
			{
				case 0:
					m_FWHM1 = fwhm;
					break;
				case 1:
					m_FWHM2 = fwhm;
					break;
				case 2:
					m_FWHM3 = fwhm;
					break;
				case 3:
					m_FWHM4 = fwhm;
					break;
			}
		}

		public List<ITrackedObject> TrackedObjects
		{
			get { return m_TrackedObjects; }
		}

		public float RefinedAverageFWHM
		{
			get { return m_FWHMAverage; }
		}

		public float[] RefinedFWHM
		{
			get { return new float[] { m_FWHM1, m_FWHM2, m_FWHM3, m_FWHM4 }; }
		}

		public float PositionTolerance
		{
			get { return 0; }
		}

		public uint MedianValue
		{
			get { return 0; }
		}

		public float RefiningPercentageWorkLeft
		{
			get { return 0; }
		}

		public bool SupportsManualCorrections
		{
			get { return false; }
		}

		public void DoManualFrameCorrection(int targetId, int manualTrackingDeltaX, int manualTrackingDeltaY)
		{ }

		public void NextFrame(int frameNo, IAstroImage astroImage)
		{
			m_IsTrackedSuccessfully = false;

			StarMap starMap = new StarMap();
			starMap.FindBestMap(
							m_StarMapConfig,
							(AstroImage)astroImage,
							Rectangle.Empty,
							new Rectangle(0, 0, astroImage.Width, astroImage.Height), 
							AstrometryContext.Current.LimitByInclusion);

			if (starMap.Features.Count >= 5)
			{
				starMap.Features.Sort((x, y) => y.PixelCount.CompareTo(x.PixelCount));
				int featuresToConsider = Math.Min(10, starMap.Features.Count);

				var featureDistances = new List<List<double>>();
				for (int i = 0; i < featuresToConsider; i++)
				{
					var dist = new List<double>();
					for (int j = 0; j < featuresToConsider; j++) dist.Add(double.NaN);
					featureDistances.Add(dist);
				}

				var pivotDistances = new List<List<double>>();
				for (int i = 0; i < featuresToConsider; i++) pivotDistances.Add(new List<double>(UNINITIALIZED_DISTANCES));


				for (int i = 0; i < featuresToConsider; i++)
				{
					var feature_i = starMap.Features[i];
					for (int j = i + 1; j < featuresToConsider; j++)
					{
						var feature_j = starMap.Features[j];
						double distance = feature_j.GetCenter().DistanceTo(feature_i.GetCenter());
						pivotDistances[i][j] = distance;
						pivotDistances[j][i] = distance;
					}
				}

				Dictionary<int, int> pivotMap = IdentifyPivots(pivotDistances);

				int identifiedObjects = 0;

				for (int i = 0; i < m_TrackedObjects.Count; i++)
				{
					((TrackedObject)m_TrackedObjects[i]).NewFrame();

					var xVals = new List<double>();
					var yVals = new List<double>();
					foreach (int key in pivotMap.Keys)
					{
						int mapsToSourceFeatureId = pivotMap[key];
						xVals.Add(starMap.Features[key].GetCenter().XDouble + m_TrackedObjectsPivotDistancesX[i][mapsToSourceFeatureId]);
						yVals.Add(starMap.Features[key].GetCenter().YDouble + m_TrackedObjectsPivotDistancesY[i][mapsToSourceFeatureId]);
					}

					double x0 = xVals.Median();
					double y0 = yVals.Median();

					double sigmaX = Math.Sqrt(xVals.Select(x => (x - x0)*(x - x0)).Sum()) / (xVals.Count - 1);
					double sigmaY = Math.Sqrt(yVals.Select(y => (y - y0)*(y - y0)).Sum()) / (yVals.Count - 1);

					if (!double.IsNaN(x0) && !double.IsNaN(y0) && xVals.Count > 1 &&
						(sigmaX > m_FWHMAverage || sigmaY > m_FWHMAverage))
					{
						// Some of the pivots may have been misidentified. Remove all entries with too large residuals and try again
						xVals.RemoveAll(x => Math.Abs(x - x0) > sigmaX);
						yVals.RemoveAll(y => Math.Abs(y - y0) > sigmaY);

						if (xVals.Count > 1)
						{
							x0 = xVals.Median();
							y0 = yVals.Median();

							sigmaX = Math.Sqrt(xVals.Select(x => (x - x0) * (x - x0)).Sum()) / (xVals.Count - 1);
							sigmaY = Math.Sqrt(yVals.Select(y => (y - y0) * (y - y0)).Sum()) / (yVals.Count - 1);							
						}
					}

					if (!double.IsNaN(x0) && !double.IsNaN(y0) && xVals.Count > 1 &&
						(sigmaX > m_FWHMAverage || sigmaY > m_FWHMAverage))
					{
						// There is something really wrong about this. Reject the position and fail the frame
						m_TrackedObjects[i].SetIsTracked(false, NotMeasuredReasons.FoundObjectNotWithInExpectedPositionTolerance, null, null);
					}
					else
					{
						PSFFit fit = new PSFFit((int)x0, (int)y0);
						uint[,] data = ((AstroImage)astroImage).GetMeasurableAreaPixels((int)x0, (int)y0);
						fit.Fit(data);

						if (fit.IsSolved)
						{							
							m_TrackedObjects[i].SetIsTracked(true, NotMeasuredReasons.TrackedSuccessfully, new ImagePixel(fit.XCenter, fit.YCenter), fit.Certainty);
							((TrackedObject)m_TrackedObjects[i]).ThisFrameX = (float)fit.XCenter;
							((TrackedObject)m_TrackedObjects[i]).ThisFrameY = (float)fit.YCenter;
							((TrackedObjectBase)m_TrackedObjects[i]).PSFFit = fit;
							identifiedObjects++;
						}
						else
						{
							m_TrackedObjects[i].SetIsTracked(false, NotMeasuredReasons.PSFFittingFailed, null, null);
						}
					}
				}

				m_IsTrackedSuccessfully = identifiedObjects == m_TrackedObjects.Count;

				if (m_IsTrackedSuccessfully)
					UpdatePivotDistances(starMap, pivotMap);
			}
		}

		private int MAX_DIST_HISTORY = 10;

		private void UpdatePivotDistances(StarMap starMap, Dictionary<int, int> pivotMap)
		{
			// Remove distance records that are too old
			while (m_TargetPivotDistancesListX.Count >= MAX_DIST_HISTORY) m_TargetPivotDistancesListX.RemoveAt(0);
			while (m_TargetPivotDistancesListY.Count >= MAX_DIST_HISTORY) m_TargetPivotDistancesListY.RemoveAt(0);

			// Add the new distance recods
			for (int i = 0; i < m_TrackedObjects.Count; i++)
			{
				var targetXDistList = new List<double>();
				var targetYDistList = new List<double>();

				m_TargetPivotDistancesListX.Add(targetXDistList);
				m_TargetPivotDistancesListY.Add(targetYDistList);

				for (int j = 0; j < pivotMap.Count; j++)
				{
					targetXDistList.Add(m_TrackedObjects[i].Center.XDouble - starMap.Features[pivotMap[j]].GetCenter().XDouble);
					targetYDistList.Add(m_TrackedObjects[i].Center.YDouble - starMap.Features[pivotMap[j]].GetCenter().YDouble);
				}
			}

			// Calcluate the new median and set the current values
			for (int i = 0; i < m_TrackedObjects.Count; i++)
			{
				// Updating the distances is important for a slow field rotation. However this may break the
				// tracking of some of the pivots are not recognized correctly. No updates for now.

				// TODO: Implement this once the pivot identification has been made very reliable
			}
		}

		private Dictionary<int, int> IdentifyPivots(List<List<double>> unorderedPivotDistances)
		{
			// TODO: This should be done based on the Pyramid recognition used for Astrometry 
			//       rather than distance comparison only

			var rv = new Dictionary<int, int>();

			var stats = new Dictionary<int, List<int>>();
			for (int i = 0; i < 10; i++) stats.Add(i, new List<int>());

			for (int i = 0; i < unorderedPivotDistances.Count; i++)
			{
				List<double> testDistances = unorderedPivotDistances[i];

				for (int j = 0; j < 10; j++)
				{
					List<double> pivotDistances = m_PivotDistances[j];

					for (int ii = 0; ii < unorderedPivotDistances.Count; ii++)
					{
						if (double.IsNaN(testDistances[ii])) continue;

						double[] diffs = pivotDistances.Select(x => Math.Abs(x - testDistances[ii])).ToArray();
						int[] indexes = new [] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
						Array.Sort(diffs, indexes);

						if (double.IsNaN(diffs[0]) && diffs[1] < 5)
						{
							// The identified distance is between pivots i and ii and it matches
							// the tested distance between j and indexes[1] so either (i -> j, ii = indexes[1]) or (ii -> j, i = indexes[1])
							stats[i].Add(j);
							stats[i].Add(indexes[1]);
							stats[ii].Add(j);
							stats[ii].Add(indexes[1]);
						}
							
					}					
				}
			}

			for (int i = 0; i < 10; i++)
			{
				int[] uniqueIDs = stats[i].Distinct().ToArray();
				int[] occurances = new List<int>(uniqueIDs).ToArray();
				for (int j = 0; j < uniqueIDs.Length; j++)
				{
					occurances[j] = stats[i].Count(x => x == uniqueIDs[j]);	
				}
				Array.Sort(occurances, uniqueIDs);
				rv[i] = uniqueIDs[uniqueIDs.Length - 1];
			}

			return rv;
		}

		public void BeginMeasurements(IAstroImage astroImage)
		{ }
	}
}
