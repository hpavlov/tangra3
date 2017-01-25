using System;
using System.Collections.Generic;
using System.Linq;
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

    public interface ITangraAstrometricSolution2
    {
        List<ITangraAstrometricMeasurement> GetAllMeasurements();
        double InstrumentalDelay { get; }
        string InstrumentalDelayUnits { get; }
        string FrameTimeType { get; }
        int IntegratedFramesCount { get; }
        double IntegratedExposureSeconds { get; }
        bool AavIntegration { get; }
        bool AavStackedMode { get; }
		string VideoFileFormat { get; }
        string NativeVideoFormat { get; }
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

    public enum BackgroundMethod
    {
        Unknown,
        AverageBackground,
        BackgroundMode,
        Background3DPolynomial,
        PSFBackground,
        BackgroundMedian
    }

    public enum PhotometryReductionMethod
    {
        Unknown,
        AperturePhotometry,
        PsfPhotometry,
        OptimalExtraction
    }

    public interface ITangraStarMeasurementInfo
    {
        PhotometryReductionMethod MeaSignalMethod { get; }
        BackgroundMethod MeaBackgroundMethod { get; }
        float? MeaSingleApertureSize { get;  }
        int MeaBackgroundPixelCount { get; }
        uint MeaSaturationLevel { get;  }
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

    public interface ITangraAPASSStarMagnitudes
    {
        //  I*2 millimag   B magnitude from APASS                  (13)
        float B { get; }

        //  I*2 millimag   V magnitude from APASS                  (13)
        float V { get; }

        //  I*2 millimag   g magnitude from APASS                  (13)
        float g { get; }

        //  I*2 millimag   r magnitude from APASS                  (13)
        float r { get; }

        //  I*2 millimag   i magnitude from APASS                  (13)
        float i { get; }

        //  I*1 1/100 mag  error of B magnitude from APASS         (14)
        float e_B { get; }

        //  I*1 1/100 mag  error of V magnitude from APASS         (14)
        float e_V { get; }

        //  I*1 1/100 mag  error of g magnitude from APASS         (14)
        float e_g { get; }

        //  I*1 1/100 mag  error of r magnitude from APASS         (14)
        float e_r { get; }

        //  I*1 1/100 mag  error of i magnitude from APASS         (14)
        float e_i { get; }
    }

    public interface ITangraAstrometricMeasurement
    {
        double RADeg { get; }
        double DEDeg { get; }
        double Mag { get; }
        double SolutionUncertaintyRACosDEArcSec { get; }
        double SolutionUncertaintyDEArcSec { get; }
        double FWHMArcSec { get; }
        double Detection { get; }
        double SNR { get; }
        int FrameNo { get; }
        DateTime? UncorrectedTimeStamp { get; }
    }
}
