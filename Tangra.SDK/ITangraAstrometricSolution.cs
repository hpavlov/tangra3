using System;
using System.Collections.Generic;
using System.Text;

namespace Tangra.SDK
{
	public interface ITangraAstrometricSolution
	{
		string StarCatalog { get; }
		DateTime UtcTime { get; }
		int FrameNoOfUtcTime { get; }
		float AutoLimitMagnitude { get; }

		float ResolvedFocalLength { get; }
		float ResolvedCenterRADeg { get; }
		float ResolvedCenterDEDeg { get; }
		float StdDevRAArcSec { get; }
		float StdDevDEArcSec { get; }

		List<ITangraMatchedStar> GetAllMatchedStars();
	}

	public interface ITangraMatchedStar
	{
		float X { get; }
		float Y { get; }
		float RADeg { get; }
		float DEDeg { get; }
		bool ExcludedForHighResidual { get; }
		float ResidualRAArcSec { get; }
		float ResidualDEArcSec { get; }
		float DetectionCertainty { get; }
		int PSFAmplitude { get; }
		float Intensity { get; }
		float Mag { get; }
        bool IsSaturated { get; }
		ITangraCatalogStar CatalogStar { get; }
	}

	public interface ITangraCatalogStar
	{
		/// <summary>
		/// Unique catalog star identifier
		/// </summary>
        ulong StarNo { get; }

		/// <summary>
		/// Right Ascension in degrees at epoch and equinox J2000.0
		/// </summary>
		float RAJ2000Deg { get; }

		/// <summary>
		/// Declination in degrees at epoch and equinox J2000.0
		/// </summary>
		float DEJ2000Deg { get; }

		/// <summary>
		/// The main magnitude listed in the catalog
		/// </summary>
		float Mag { get; }
		
		/// <summary>
		/// V magnitude either listed in the catalog or derived from available catalog data and known transformations
		/// </summary>
		float MagV { get; }

		/// <summary>
		/// R magnitude either listed in the catalog or derived from available catalog data and known transformations
		/// </summary>		
		float MagR { get; }
		
		/// <summary>
		/// B magnitude either listed in the catalog or derived from available catalog data and known transformations
		/// </summary>		
		float MagB { get; }
		
		/// <summary>
		/// J magnitude either listed in the catalog or derived from available catalog data and known transformations
		/// </summary>		
		float MagJ { get; }
		
		/// <summary>
		/// K magnitude either listed in the catalog or derived from available catalog data and known transformations
		/// </summary>		
		float MagK { get; }
	}
}
