using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.Model.Config
{
	public enum AstrometricMethod
	{
		AutomaticFit,
		LinearFit,
		QuadraticFit,
		CubicFit
	}

	public enum FieldAlignmentMethod
	{
		Pyramid
	}

	public interface IAstrometrySettings
	{
		double DetectionLimit { get; }

		AstrometricMethod Method { get; }
		int MinimumNumberOfStars { get; }
		int MaximumNumberOfStars { get; }
		double MaxResidual { get; }

		FieldAlignmentMethod AlignmentMethod { get; }
       	double MaxPreliminaryResidual { get; }

		[Obsolete]
		double PyramidDistanceTolerance { get; }

		double PyramidDistanceToleranceInPixels { get; }
		double PyramidOptimumStarsToMatch { get; }
		int DistributionZoneStars { get; }

		bool PyramidRemoveNonStellarObject { get; }
		double LimitReferenceStarDetection { get; }
		double MinReferenceStarFWHM { get; }
		double MaxReferenceStarFWHM { get; }
		int MaximumPSFElongation { get; }

		double PyramidFocalLengthAllowance { get; }
		bool PyramidForceFixedFocalLength { get; }
		int PyramidTimeoutInSeconds { get; }

		//MagInputBand DefaultMagInputBand { get; }
		//MagOutputBand DefaultMagOutputBand { get; }
	}
}
