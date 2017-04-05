/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.Astrometry.Recognition
{
	public class CorePyramidConfig
	{
		public static CorePyramidConfig Default = new CorePyramidConfig();

		/// <summary>
		/// A Pyramid fit is not considered successfull if there are more than this percentage
		/// of unfit features, brigher than 25% brigher than the faintest fit feature
		/// </summary>
		public int MinPercentage75BrighterUnfitFeatures = 45;

		/// <summary>
		/// A Pyramid fit is not considered successfull if there are more than this percentage
		/// of unfit features, brigher than the faintest fit feature
		/// </summary>
		public int MinPercentage100BrighterUnfitFeatures = 60;



		public int MaxAllowedResidualInPixelsInSuccessfulFit = 5;


		public int MaxMeanResidualInPixelsInSuccessfulFit = 2;

		/// <summary>
		/// A warning will be displayed if there are more than this many celestial stars in the FOV
		/// </summary>
		public int CrowdedFieldStarsNumber = 300;

        public int OptimalStarsForAligment = 100;

		public int MinStarsPerAreaForAlignment = 1;

		/// <summary>
		/// Astrometry will now stars if there are less than this many celestial stars in the FOV
		/// </summary>
		public int MinStarsForAligment = 5;


		public int MinStarsForImprovementForThreshold = 10;


		public double MaxResidualThresholdForImprovementInPixels = 2.0;


	    /// <summary>
	    /// If that many stars have been recognised with Pyramid alignment then we try to improve the 
	    /// solution and see whether m_Settings.NumberOfStars will be located
	    /// </summary>
	    public int MinPyramidAlignedStars =
	        3 /* We always need at least 3 stars */+
	        1 /* And then we request that many additional stars*/;

		/// <summary>
		/// How many bright stars will be ignored when computing the unfit feature percentage
		/// This number is to allow missing catalog stars and minor planets in the field
		/// </summary>
		public int BrighterUnfitFeaturesTolerance = 3;


		public int MinMatchedStarsForCalibration = 7;

		/// <summary>
		/// After this number of combinations the pyramid matching will fail. 2300 is combinations
		/// are all unique triangles involving 25 satrs.
		/// </summary>
		public int MaxNumberOfCombinations = 2300;

		public double MinDetectionLimitForSolutionImprovement = 0.25; /* This is a really low value that will exclude very noisy data */

		public double MaxPreliminaryResidualForSolutionImprovementInPixels = 1.5 /* pixels */; 

	    public double DefaultMinPyramidMagnitude = 4.0;  // Bright enough for large field of view
	    public double DefaultMaxPyramidMagnitude = 12.0; // No too faint for a reasonable number of pairs to check

        public double DefaultMinAstrometryMagnitude = 4.0;  // Bright enough for large field of view
        public double DefaultMaxAstrometryMagnitude = 16.0;  // Generally matches the faintest magnitude in the supported astrometric catalogues

	    public double MaxFWHMExcludedImprovemntStarsCoeff = 2.5; 

		public bool ForceAlwaysUsePyramidWithRatios = false;

		public double MagFitTestMaxStdDev = 2.0;

		public double MagFitTestMaxInclination = 0.005;

		public double MaxThreeIdentifiedStarsFitResidual = 16;


        /// <summary>
        /// This is how we determine the limiting magnitude for the alignment stars when doing calibration.
        /// We start from mag 12 until we reach this number of stars.
        /// </summary>
        public int CalibrationMaxStarsForAlignment = 250;


		/// <summary>
        /// The max IAstrometricSettings.MaxResidualInPixels when doing calibration
		/// </summary>
		public double CalibrationMaxResidualInPixels = 3.0;

		/// <summary>
		/// The max IAstrometricSettings.PyramidDistanceTolerance when doing calibration
		/// </summary>		
		public double CalibrationPyramidDistanceToleranceInPixels = 6.0;

		/// <summary>
		/// The max IAstrometricSettings.NumberOfStars when doing calibration
		/// </summary>		
		public int CalibrationNumberOfStars = 7;

		/// <summary>
		/// The max IAstrometricSettings.SearchArea when doing calibration
		/// </summary>		
		public double CalibrationSearchArea = 10.0;

		// This is to exlude wrong star mapping due to a coarse preliminary fit in crowded areas
		public int CalibrationFirstPlateSolveLimitMagnitude = 12;

        public bool AttempDoubleSolutionImprovement = false;


		public void Reset()
		{
			BrighterUnfitFeaturesTolerance = 3;
			ForceAlwaysUsePyramidWithRatios = false;
			MaxThreeIdentifiedStarsFitResidual = 16;
			CrowdedFieldStarsNumber = 300;
			MinStarsForAligment = 5;

			MaxMeanResidualInPixelsInSuccessfulFit = 2;
			MaxAllowedResidualInPixelsInSuccessfulFit = 5;

			MinPercentage100BrighterUnfitFeatures = 60;
			MinPercentage75BrighterUnfitFeatures = 45;
		    AttempDoubleSolutionImprovement = false;
		}
	}
}
