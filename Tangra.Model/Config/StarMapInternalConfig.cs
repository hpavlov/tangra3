using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.Model.Config
{
	public class StarMapInternalConfig
	{
		public static StarMapInternalConfig Default = new StarMapInternalConfig();

		public void Reset()
		{
			MaxStarsInField = 500;
			MinStarsInField = 10;

			StarMapperTolerance = 2;
			MinStarMapThreashold = 0;
			FeatureSearchRadius = 7;

			MaxPixelsInFeature = 160;
			OptimumStarsInField = null;
		}

		private StarMapInternalConfig()
		{
			Reset();
		}

		/// <summary>
		/// The star mapping will not return a successful map if less that this number of features is found
		/// </summary>
		public int MinStarsInField { get; set; }

		/// <summary>
		/// The maximum number of features. No more features will be returned.
		/// </summary>
		public int MaxStarsInField { get; set; }

		/// <summary>
		/// If the number of pixels in a feature 
		/// This is used in conjuction with FeatureSearchRadius and there is no point
		/// of setting it bigger than 4 * FeatureSearchRadius * FeatureSearchRadius
		/// </summary>
		public int MaxPixelsInFeature { get; set; }

		/// <summary>
		/// The search for the feature will be done in a square NxN around the peak pixel
		/// This is to avoid too many pixels getting added to the feature or too irregular feature
		/// This flag should be used in combination with MaxPixelsInFeature
		/// </summary>
		public int FeatureSearchRadius { get; set; }

		public int StarMapperTolerance { get; set; }

		/// <summary>
		///  The background threashold. We will not try to resolve any features fainter than this number
		/// </summary>
		public int MinStarMapThreashold { get; set; }

		/// <summary>
		/// NULL means default (TangraConfig.Settings.Astrometry.PyramidOptimumStarsToMatch), -1 means 
		/// "Do not try for optimum stars but used the manually given StarMapperTolerance", any other values
		/// means "Try for that many optimum stars in the field"
		/// </summary>
		public int? OptimumStarsInField { get; set; }
	}
}
