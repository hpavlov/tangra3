using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.AstroServices;
using Tangra.Astrometry;
using Tangra.Model.Config;
using Tangra.Model.Helpers;
using Tangra.SDK;
using Tangra.StarCatalogues;
using Tangra.StarCatalogues.NOMAD;
using Tangra.StarCatalogues.UCAC2;
using Tangra.StarCatalogues.UCAC3;
using Tangra.StarCatalogues.UCAC4;
using Tangra.VideoOperations.Astrometry;
using Tangra.VideoOperations.Astrometry.Engine;

namespace Tangra.Addins
{
	[Serializable]
    internal class AstrometricSolutionImpl : ITangraAstrometricSolution, ITangraAstrometricSolution2
	{
		public string StarCatalog { get; internal set; }
		public DateTime UtcTime { get; internal set; }
		public int FrameNoOfUtcTime { get; internal set; }
		public float AutoLimitMagnitude { get; internal set; }

		public float ResolvedFocalLength { get; internal set; }
		public float ResolvedCenterRADeg { get; internal set; }
		public float ResolvedCenterDEDeg { get; internal set; }
		public float StdDevRAArcSec { get; internal set; }
		public float StdDevDEArcSec { get; internal set; }

		private TangraUserObjectImpl m_UserObject;

		public AstrometricSolutionImpl(LeastSquareFittedAstrometry astrometry, StarMagnitudeFit photometry, AstrometricState state, FieldSolveContext fieldSolveContext, MeasurementContext measurementContext)
		{
			StarCatalog = fieldSolveContext.StarCatalogueFacade.CatalogNETCode;
			UtcTime = fieldSolveContext.UtcTime;
			FrameNoOfUtcTime = fieldSolveContext.FrameNoOfUtcTime;
			AutoLimitMagnitude = (float)fieldSolveContext.AutoLimitMagnitude;

			ResolvedFocalLength = (float)fieldSolveContext.FocalLength;

			if (astrometry != null)
			{
				ResolvedCenterRADeg = (float)astrometry.RA0Deg;
				ResolvedCenterDEDeg = (float)astrometry.DE0Deg;
				StdDevRAArcSec = (float)astrometry.StdDevRAArcSec;
				StdDevDEArcSec = (float)astrometry.StdDevDEArcSec;
			}
			else
			{
				ResolvedCenterRADeg = float.NaN;
				ResolvedCenterDEDeg = float.NaN;
				StdDevRAArcSec = float.NaN;
				StdDevDEArcSec = float.NaN;
			}

			if (state.SelectedObject != null)
			{
				m_UserObject = new TangraUserObjectImpl();
				m_UserObject.RADeg = (float)state.SelectedObject.RADeg;
				m_UserObject.DEDeg = (float)state.SelectedObject.DEDeg;
				m_UserObject.X = (float)state.SelectedObject.X0;
				m_UserObject.Y = (float)state.SelectedObject.Y0;

				if (state.IdentifiedObjects != null &&
					state.IdentifiedObjects.Count == 1)
				{
					foreach (IIdentifiedObject idObj in state.IdentifiedObjects)
					{
						if (AngleUtility.Elongation(idObj.RAHours * 15.0, idObj.DEDeg, state.SelectedObject.RADeg, state.SelectedObject.DEDeg) * 3600 < 120)
						{
							m_UserObject.ResolvedName = idObj.ObjectName;
							break;
						}
					}
				}
			}

		    InstrumentalDelay = measurementContext.InstrumentalDelay;
            InstrumentalDelayUnits = measurementContext.InstrumentalDelayUnits.ToString();
            FrameTimeType = measurementContext.FrameTimeType.ToString();
            IntegratedFramesCount = measurementContext.IntegratedFramesCount;
            IntegratedExposureSeconds = measurementContext.IntegratedExposureSeconds;
            AavIntegration = measurementContext.AavIntegration;
            AavStackedMode = measurementContext.AavStackedMode;
            VideoFileFormat = measurementContext.VideoFileFormat.ToString();
            NativeVideoFormat = measurementContext.NativeVideoFormat;
            if (!string.IsNullOrEmpty(state.IdentifiedObjectToMeasure))
                ObjectDesignation = MPCObsLine.GetObjectCode(state.IdentifiedObjectToMeasure);
            else if (state.IdentifiedObjects != null && state.IdentifiedObjects.Count == 1)
                ObjectDesignation = MPCObsLine.GetObjectCode(state.IdentifiedObjects[0].ObjectName);

            ObservatoryCode = TangraConfig.Settings.Astrometry.MPCObservatoryCode;

		    m_MeasurementsImpl = new List<TangraAstrometricMeasurementImpl>();

		    if (state.Measurements != null)
		    {
		        foreach (var mea in state.Measurements)
		        {
                    m_MeasurementsImpl.Add(new TangraAstrometricMeasurementImpl()
                    {
                        DEDeg = mea.DEDeg,
                        RADeg = mea.RADeg,
                        FrameNo = mea.FrameNo,
                        SolutionUncertaintyRACosDEArcSec = mea.SolutionUncertaintyRACosDEArcSec,
                        SolutionUncertaintyDEArcSec = mea.SolutionUncertaintyDEArcSec,
                        FWHMArcSec = mea.FWHMArcSec,
                        Detection = mea.Detection,
                        SNR = mea.SNR,
                        UncorrectedTimeStamp = mea.FrameTimeStamp,
                        Mag = mea.Mag
                    });
		        }
		    }

			m_MatchedStarImpl = new List<TangraMatchedStarImpl>();

			if (astrometry != null)
			{
				foreach (PlateConstStarPair pair in astrometry.FitInfo.AllStarPairs)
				{
					if (pair.FitInfo.UsedInSolution)
					{
						var star = new TangraMatchedStarImpl()
						{
							X = (float)pair.x,
							Y = (float)pair.y,
							RADeg = (float)pair.RADeg,
							DEDeg = (float)pair.DEDeg,
							StarNo = pair.StarNo,
							ExcludedForHighResidual = pair.FitInfo.ExcludedForHighResidual,
							ResidualRAArcSec = (float)pair.FitInfo.ResidualRAArcSec,
							ResidualDEArcSec = (float)pair.FitInfo.ResidualDEArcSec,
							DetectionCertainty = (float)pair.DetectionCertainty,
							PSFAmplitude = (int)pair.Intensity,
							IsSaturated = pair.IsSaturated,
							Mag = (float)pair.Mag
						};

						TangraCatalogStarImpl catStar = null;

						IStar catalogStar = fieldSolveContext.CatalogueStars.Find(s => s.StarNo == pair.StarNo);
						if (catalogStar != null)
						{
							if (catalogStar is UCAC4Entry)
								catStar = new TangraAPASSStar();
							else
								catStar = new TangraCatalogStarImpl();

							catStar.StarNo = catalogStar.StarNo;
							catStar.MagR = (float)catalogStar.MagR;
							catStar.MagV = (float)catalogStar.MagV;
							catStar.MagB = (float)catalogStar.MagB;
							catStar.Mag = (float)catalogStar.Mag;

							if (catalogStar is UCAC3Entry)
							{
								UCAC3Entry ucac3Star = (UCAC3Entry)catalogStar;
								catStar.MagJ = (float)(ucac3Star.jmag * 0.001);
								catStar.MagK = (float)(ucac3Star.kmag * 0.001);
								catStar.RAJ2000Deg = (float)ucac3Star.RACat;
								catStar.DEJ2000Deg = (float)ucac3Star.DECat;
							}
							else if (catalogStar is UCAC2Entry)
							{
								UCAC2Entry ucac2Star = (UCAC2Entry)catalogStar;
								catStar.MagJ = (float)(ucac2Star._2m_J * 0.001);
								catStar.MagK = (float)(ucac2Star._2m_Ks * 0.001);
								catStar.RAJ2000Deg = (float)ucac2Star.RACat;
								catStar.DEJ2000Deg = (float)ucac2Star.DECat;
							}
							else if (catalogStar is NOMADEntry)
							{
								NOMADEntry nomadStar = (NOMADEntry)catalogStar;
								catStar.MagJ = (float)(nomadStar.m_J * 0.001);
								catStar.MagK = (float)(nomadStar.m_K * 0.001);
								catStar.RAJ2000Deg = (float)nomadStar.RACat;
								catStar.DEJ2000Deg = (float)nomadStar.DECat;
							}
							else if (catalogStar is UCAC4Entry)
							{
								UCAC4Entry ucac4Star = (UCAC4Entry)catalogStar;
								catStar.MagJ = (float)(ucac4Star.MagJ);
								catStar.MagK = (float)(ucac4Star.MagK);
								catStar.RAJ2000Deg = (float)ucac4Star.RACat;
								catStar.DEJ2000Deg = (float)ucac4Star.DECat;

								((TangraAPASSStar)catStar).B = (float)ucac4Star.MagB;
								((TangraAPASSStar)catStar).V = (float)ucac4Star.MagV;
								((TangraAPASSStar)catStar).g = (float)ucac4Star.Mag_g;
								((TangraAPASSStar)catStar).r = (float)ucac4Star.Mag_r;
								((TangraAPASSStar)catStar).i = (float)ucac4Star.Mag_i;
								((TangraAPASSStar)catStar).e_B = ucac4Star.apase_B * 0.001f;
								((TangraAPASSStar)catStar).e_V = ucac4Star.apase_V * 0.001f;
								((TangraAPASSStar)catStar).e_g = ucac4Star.apase_g * 0.001f;
								((TangraAPASSStar)catStar).e_r = ucac4Star.apase_r * 0.001f;
								((TangraAPASSStar)catStar).e_i = ucac4Star.apase_i * 0.001f;
							}
						}

						star.CatalogStar = catStar;

						if (photometry != null)
						{
							IStar photometryStar = photometry.StarNumbers.FirstOrDefault(s => s.StarNo == pair.StarNo);
							if (photometryStar != null)
							{
								int idx = photometry.StarNumbers.IndexOf(photometryStar);
								star.Intensity = (float)photometry.Intencities[idx];
								star.IsSaturated = photometry.SaturatedFlags[idx];
                                star.MeaSignalMethod = ConvertSignalMethod(photometry.MeaSignalMethod);
                                star.MeaBackgroundMethod = ConvertBackgroundMethod(photometry.MeaBackgroundMethod);
                                star.MeaSingleApertureSize = photometry.MeaSingleAperture;
                                star.MeaBackgroundPixelCount = photometry.MeaBackgroundPixelCount;
                                star.MeaSaturationLevel = photometry.MeaSaturationLevel;
							}
						}

						m_MatchedStarImpl.Add(star);
					}
				}
			}
		}

	    private PhotometryReductionMethod ConvertSignalMethod(TangraConfig.PhotometryReductionMethod method)
	    {
	        switch (method)
	        {
	            case TangraConfig.PhotometryReductionMethod.AperturePhotometry:
                    return PhotometryReductionMethod.AperturePhotometry;
                case TangraConfig.PhotometryReductionMethod.PsfPhotometry:
                    return PhotometryReductionMethod.PsfPhotometry;
                case TangraConfig.PhotometryReductionMethod.OptimalExtraction:
                    return PhotometryReductionMethod.OptimalExtraction;

                default:
	                return PhotometryReductionMethod.Unknown;
	        }
	    }

        private BackgroundMethod ConvertBackgroundMethod(TangraConfig.BackgroundMethod method)
        {
            switch (method)
            {
                case TangraConfig.BackgroundMethod.AverageBackground:
                    return BackgroundMethod.AverageBackground;
                case TangraConfig.BackgroundMethod.BackgroundMedian:
                    return BackgroundMethod.BackgroundMedian;
                case TangraConfig.BackgroundMethod.BackgroundMode:
                    return BackgroundMethod.BackgroundMode;
                case TangraConfig.BackgroundMethod.PSFBackground:
                    return BackgroundMethod.PSFBackground;
                case TangraConfig.BackgroundMethod.Background3DPolynomial:
                    return BackgroundMethod.Background3DPolynomial;
                default:
                    return BackgroundMethod.Unknown;
            }
        }

		private List<TangraMatchedStarImpl> m_MatchedStarImpl;

		public List<TangraMatchedStarImpl> MatchedStarImpl
		{
			get { return m_MatchedStarImpl; }
		}


	    private List<TangraAstrometricMeasurementImpl> m_MeasurementsImpl;

	    public double InstrumentalDelay { get; private set; }
        public string InstrumentalDelayUnits { get; private set; }
        public string FrameTimeType { get; private set; }
        public int IntegratedFramesCount { get; private set; }
        public double IntegratedExposureSeconds { get; private set; }
        public bool AavIntegration { get; private set; }
        public bool AavStackedMode { get; private set; }
        public string VideoFileFormat { get; private set; }
        public string NativeVideoFormat { get; private set; }
        public string ObservatoryCode { get; private set; }
        public string ObjectDesignation { get; private set; }

		public List<ITangraMatchedStar> GetAllMatchedStars()
		{
			return m_MatchedStarImpl
				.Cast<ITangraMatchedStar>()
				.ToList();
		}

        public List<ITangraAstrometricMeasurement> GetAllMeasurements()
        {
            return m_MeasurementsImpl
                .Cast<ITangraAstrometricMeasurement>()
                .ToList();
        }

		[Serializable]
        internal class TangraMatchedStarImpl : ITangraMatchedStar, ITangraStarMeasurementInfo
		{
			public float X { get; internal set; }
			public float Y { get; internal set; }
			public float RADeg { get; internal set; }
			public float DEDeg { get; internal set; }
			public ulong StarNo { get; internal set; }
			public bool ExcludedForHighResidual { get; internal set; }
			public float ResidualRAArcSec { get; internal set; }
			public float ResidualDEArcSec { get; internal set; }
			public float DetectionCertainty { get; internal set; }
			public int PSFAmplitude { get; internal set; }
			public float Intensity { get; internal set; }
			public float Mag { get; internal set; }
			public bool IsSaturated { get; internal set; }
			public ITangraCatalogStar CatalogStar { get; internal set; }

            public PhotometryReductionMethod MeaSignalMethod { get; internal set; }
            public BackgroundMethod MeaBackgroundMethod { get; internal set; }
            public float? MeaSingleApertureSize { get; internal set; }
            public int MeaBackgroundPixelCount { get; internal set; }
            public uint MeaSaturationLevel { get; internal set; }
		}

		[Serializable]
		internal class TangraCatalogStarImpl : ITangraCatalogStar
		{
			public ulong StarNo { get; internal set; }
			public float RAJ2000Deg { get; internal set; }
			public float DEJ2000Deg { get; internal set; }
			public float Mag { get; internal set; }
			public float MagV { get; internal set; }
			public float MagR { get; internal set; }
			public float MagB { get; internal set; }
			public float MagJ { get; internal set; }
			public float MagK { get; internal set; }
		}

        [Serializable]
	    internal class TangraAstrometricMeasurementImpl : ITangraAstrometricMeasurement
	    {
            public double RADeg { get; internal set; }
            public double DEDeg { get; internal set; }
            public double Mag { get; internal set; }
            public double SolutionUncertaintyRACosDEArcSec { get; internal set; }
            public double SolutionUncertaintyDEArcSec { get; internal set; }
            public double FWHMArcSec { get; internal set; }
            public double Detection { get; internal set; }
            public double SNR { get; internal set; }
            public int FrameNo { get; internal set; }
            public DateTime? UncorrectedTimeStamp { get; internal set; }
        }

		[Serializable]
		internal class TangraAPASSStar : TangraCatalogStarImpl, ITangraAPASSStarMagnitudes
		{
			public float B { get; internal set; }
			public float V { get; internal set; }
			public float g { get; internal set; }
			public float r { get; internal set; }
			public float i { get; internal set; }
			public float e_B { get; internal set; }
			public float e_V { get; internal set; }
			public float e_g { get; internal set; }
			public float e_r { get; internal set; }
			public float e_i { get; internal set; }
		}

		[Serializable]
		internal class TangraUserObjectImpl
		{
			public float X { get; internal set; }
			public float Y { get; internal set; }
			public float RADeg { get; internal set; }
			public float DEDeg { get; internal set; }
			public string ResolvedName { get; internal set; }
		}
	}
}
