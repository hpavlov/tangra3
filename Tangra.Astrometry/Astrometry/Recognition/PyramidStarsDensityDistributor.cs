/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.StarCatalogues;


namespace Tangra.Astrometry.Recognition
{
	public class PyramidStarsDensityDistributor
	{
        private int MAX_STARS_IN_AREA = 10;

		private List<IStar> m_Stars;
		private AstroPlate m_Image;	
		private double m_XAreaSideDeg;
		private double m_YAreaSideDeg;
	    
		public List<DensityArea> Areas = new List<DensityArea>();
	    internal double m_RA0Deg;
        internal double m_DE0Deg;
        internal Dictionary<int, ulong> DebugResolvedStarsWithAppliedExclusions;

		public PyramidStarsDensityDistributor(List<IStar> stars, AstroPlate image, IAstrometrySettings settings, double ra0Deg, double de0Deg)
		{
			m_Stars = stars;
			m_Image = image;
			m_XAreaSideDeg = image.GetDistanceInArcSec(0, image.CenterYImage, image.CenterXImage, image.CenterYImage) / 3600.0;
			m_YAreaSideDeg = image.GetDistanceInArcSec(image.CenterXImage, 0, image.CenterXImage, image.CenterYImage) / 3600.0;

		    m_RA0Deg = ra0Deg;
		    m_DE0Deg = de0Deg;
			MAX_STARS_IN_AREA = settings.DistributionZoneStars;
		}

        public void Initialize(List<ulong> alwaysIncludeStars)
		{
			if (DebugResolvedStarsWithAppliedExclusions != null)
			{
				List<ulong> debugStarIds = DebugResolvedStarsWithAppliedExclusions.Values.ToList();
				List<IStar> missingstars = m_Stars.Where(s => !debugStarIds.Contains(s.StarNo)).ToList();
				Trace.Assert(missingstars.Count == 0, 
					string.Format("There are {0} of the debug stars not found among the initial pyramid stars", missingstars.Count));
			}

            if (alwaysIncludeStars != null && alwaysIncludeStars.Count > 0)
            {
                foreach(var starNo in alwaysIncludeStars.ToArray())
                    if (m_Stars.FirstOrDefault(s => s.StarNo == starNo) == null)
                    {
                        Trace.WriteLine(string.Format("Cannot locate always include star {0}. Removing ...", starNo));
                        alwaysIncludeStars.Remove(starNo);
                    }
            }

            double minDE = double.MaxValue;
            double maxDE = double.MinValue;
            double minRA = double.MaxValue;
            double maxRA = double.MinValue;
            for (int i = 0; i < m_Stars.Count; i++)
            {
                IStar star = m_Stars[i];
                if (star.RADeg > maxRA) maxRA = star.RADeg;
                if (star.RADeg < minRA) minRA = star.RADeg;
                if (star.DEDeg > maxDE) maxDE = star.DEDeg;
                if (star.DEDeg < minDE) minDE = star.DEDeg;
            }

            TangentalTransRotAstrometry astrometryBase = new TangentalTransRotAstrometry(m_Image, m_RA0Deg, m_DE0Deg, 0);
			TangentalNormalizedDirectionAstrometry astrometry = new TangentalNormalizedDirectionAstrometry(astrometryBase);
            DensityArea middleArea = new DensityArea(astrometry, MAX_STARS_IN_AREA, m_RA0Deg - m_XAreaSideDeg / 2, m_RA0Deg + m_XAreaSideDeg / 2, m_DE0Deg - m_YAreaSideDeg / 2, m_DE0Deg + m_YAreaSideDeg / 2);
		    middleArea.DebugResolvedStarsWithAppliedExclusions = DebugResolvedStarsWithAppliedExclusions;

			double xSidePlatePix = Math.Abs(middleArea.XTo - middleArea.XFrom);
			double ySidePlatePix = Math.Abs(middleArea.YTo - middleArea.YFrom);

			double raInterval = astrometryBase.GetDistanceInArcSec(middleArea.XFrom, middleArea.YMiddle, middleArea.XTo, middleArea.YMiddle) / 3600.0;
			double deInterval = astrometryBase.GetDistanceInArcSec(middleArea.XMiddle, middleArea.YFrom, middleArea.XMiddle, middleArea.YTo) / 3600.0;

			List<DensityArea> areasToCheckFurther = new List<DensityArea>();
			areasToCheckFurther.Add(middleArea);
			Areas.Add(middleArea);

			do
			{
				DensityArea[] areasToCheckNow = areasToCheckFurther.ToArray();
				areasToCheckFurther.Clear();

				foreach (DensityArea area in areasToCheckNow)
				{
					List<DensityArea> potentiallyNewAreas = GetSurroundingAreas(area, astrometry, xSidePlatePix, ySidePlatePix);
					foreach(DensityArea par in potentiallyNewAreas)
					{
						bool areadyAdded = Areas.Exists(a => a.ContainsPoint(par.XMiddle, par.YMiddle));
						if (!areadyAdded)
						{
							if (par.RAFrom + raInterval < minRA ||
								par.RATo - raInterval > maxRA ||
								par.DEFrom + deInterval < minDE ||
								par.DETo - deInterval > maxDE)
							{
								// Area not in the coordinates of interest
							}
							else
							{
								areasToCheckFurther.Add(par);
								Areas.Add(par);
							}
						}
					}
				}
			}
			while (areasToCheckFurther.Count > 0);

			m_Stars.Sort((s1, s2) => s1.Mag.CompareTo(s2.Mag));
            var aiss = (alwaysIncludeStars != null && alwaysIncludeStars.Count > 0) ? m_Stars.Where(s => alwaysIncludeStars.Contains(s.StarNo)).ToArray() : new IStar[0]; 
            Areas.ForEach(a =>
            {
                foreach (var star in m_Stars)
                {
                    a.CheckAndAddStar(star);
                    if (a.IncludedStarNos.Count >= MAX_STARS_IN_AREA)
                        break;                    
                }

                foreach (var ais in aiss)
                {
                    if (a.CheckAndAddStar(ais))
                    {
                        if (!a.IncludedStarNos.Contains(ais.StarNo))
                            a.IncludedStarNos.Add(ais.StarNo);
                    }
                }
           });
		}

		private bool TestAreaConsitency(List<DensityArea> areas)
		{
			// Selftest
			for (int i = 0; i < areas.Count; i++)
			{
				for (int j = 0; j < areas.Count; j++)
				{
					if (i == j) continue;

					DensityArea area1 = areas[i];
					DensityArea area2 = areas[j];

					Rectangle rect1 = new Rectangle(
						(int)(area1.XFrom * 1000) + 1,
						(int)(area1.YFrom * 1000) + 1,
						(int)(area1.XTo * 1000) - (int)(area1.XFrom * 1000) - 1,
						(int)(area1.YTo * 1000) - (int)(area1.YFrom * 1000) - 1);

					Rectangle rect2 = new Rectangle(
						(int)(area2.XFrom * 1000) + 1,
						(int)(area2.YFrom * 1000) + 1,
						(int)(area2.XTo * 1000) - (int)(area2.XFrom * 1000) - 1,
						(int)(area2.YTo * 1000) - (int)(area2.YFrom * 1000) - 1);

					if (rect1.IntersectsWith(rect2) || rect1.Contains(rect2))
						return false;
				}
			}

			return true;
		}

		private List<DensityArea> GetSurroundingAreas(
            DensityArea middleArea, 
            TangentalNormalizedDirectionAstrometry astrometry, 
            double xSide, 
            double ySide)
		{
			var rv = new List<DensityArea>();

			// Top 3
            rv.Add(ComputeAreaFromXYPixCoords(astrometry, middleArea.XFrom - xSide, middleArea.YFrom - ySide, middleArea.XFrom, middleArea.YFrom));
            rv.Add(ComputeAreaFromXYPixCoords(astrometry, middleArea.XFrom, middleArea.YFrom - ySide, middleArea.XTo, middleArea.YFrom));
            rv.Add(ComputeAreaFromXYPixCoords(astrometry, middleArea.XTo, middleArea.YFrom - ySide, middleArea.XTo + xSide, middleArea.YFrom));

			// Middle 2
            rv.Add(ComputeAreaFromXYPixCoords(astrometry, middleArea.XFrom - xSide, middleArea.YFrom, middleArea.XFrom, middleArea.YTo));
            rv.Add(ComputeAreaFromXYPixCoords(astrometry, middleArea.XTo, middleArea.YFrom, middleArea.XTo + xSide, middleArea.YTo));

			// Bottom 3
            rv.Add(ComputeAreaFromXYPixCoords(astrometry, middleArea.XFrom - xSide, middleArea.YTo, middleArea.XFrom, middleArea.YTo + ySide));
            rv.Add(ComputeAreaFromXYPixCoords(astrometry, middleArea.XFrom, middleArea.YTo, middleArea.XTo, middleArea.YTo + ySide));
            rv.Add(ComputeAreaFromXYPixCoords(astrometry, middleArea.XTo, middleArea.YTo, middleArea.XTo + xSide, middleArea.YTo + ySide));
			
			return rv;
		}

		private DensityArea ComputeAreaFromXYPixCoords(
			TangentalNormalizedDirectionAstrometry astrometry,
			double x1, double y1, double x2, double y2)
		{
			double ra1, de1, ra2, de2;
			astrometry.GetRADEFromImageCoords(x1, y1, out ra1, out de1);
			astrometry.GetRADEFromImageCoords(x2, y2, out ra2, out de2);
            DensityArea area = new DensityArea(astrometry, MAX_STARS_IN_AREA, ra1, ra2, de1, de2);
		    area.DebugResolvedStarsWithAppliedExclusions = DebugResolvedStarsWithAppliedExclusions;

#if ASTROMETRY_DEBUG
			Trace.Assert(Math.Abs(area.XFrom - x1) < 1);
			Trace.Assert(Math.Abs(area.XTo - x2) < 1);
			Trace.Assert(Math.Abs(area.YFrom - y1) < 1);
			Trace.Assert(Math.Abs(area.YTo - y2) < 1);
#endif

			double xx1, yy1, xx2, yy2;
			astrometry.GetImageCoordsFromRADE(area.RAFrom, area.DEFrom, out xx1, out yy1);
			astrometry.GetImageCoordsFromRADE(area.RATo, area.DETo, out xx2, out yy2);

#if ASTROMETRY_DEBUG
			Trace.Assert(Math.Abs(area.XFrom - xx1) < 1);
			Trace.Assert(Math.Abs(area.YFrom - yy1) < 1);
			Trace.Assert(Math.Abs(area.XTo - xx2) < 1);
			Trace.Assert(Math.Abs(area.YTo - yy2) < 1);

			Trace.Assert(Math.Abs(x1 - xx1) < 1);
			Trace.Assert(Math.Abs(y1 - yy1) < 1);
			Trace.Assert(Math.Abs(x2 - xx2) < 1);
			Trace.Assert(Math.Abs(y2 - yy2) < 1);
#endif

			return area;
		}
	
		public void MarkStar(IStar s)
		{
			Areas.ForEach(a => a.MarkStar(s));
		}

        public bool CheckStar(IStar s)
        {
            return Areas
                .Select(a => a.CheckStarReturnTrueIfParticipating(s))
                .Aggregate((a, b) => a || b);
        }

		public bool IsComplete
		{
			get
			{
                return Areas.FirstOrDefault(a => a.MarkedStars < MAX_STARS_IN_AREA) == null;
			}
		}
	}

	public class DensityArea
	{
		private TangentalNormalizedDirectionAstrometry m_Astrometry;
		private double m_RAFrom;
		private double m_RATo;
		private double m_DEFrom;
		private double m_DETo;

		public double RAFrom { get { return m_RAFrom; } }
		public double RATo { get { return m_RATo; } }
		public double DEFrom { get { return m_DEFrom; } }
		public double DETo { get { return m_DETo; } }

		private double m_XFrom;
		private double m_XTo;
		private double m_YFrom;
		private double m_YTo;

		public double XFrom { get { return m_XFrom; } }
		public double XTo { get { return m_XTo; } }
		public double YFrom { get { return m_YFrom; } }
		public double YTo { get { return m_YTo; } }

		private int m_MaxStarsInArea;

		public double XMiddle { get { return (m_XFrom + m_XTo) / 2.0; } }
		public double YMiddle { get { return (m_YFrom + m_YTo) / 2.0; } }

        public List<ulong> IncludedStarNos = new List<ulong>();
        internal List<ulong> m_UsedStarNos = new List<ulong>();

		public int MarkedStars { get; set; }

		public string AreaId
		{
			get { return string.Format("Area [{0:0.0}, {1:0.0}]", XMiddle, YMiddle); }
		}

        private List<ulong> DebugStarNosWithAppliedExclusions;
        private Dictionary<int, ulong> m_DebugResolvedStars;

        internal Dictionary<int, ulong> DebugResolvedStarsWithAppliedExclusions
	    {
	        set
	        {
	            m_DebugResolvedStars = value;
                if (value != null)
                    DebugStarNosWithAppliedExclusions = value.Values.ToList();
                else
                    DebugStarNosWithAppliedExclusions = null;
	        }
	    }

		public bool ContainsPoint(double x, double y)
		{
			return (m_XFrom <= x && m_XTo >= x && m_YFrom <= y && m_YTo >= y);
		}

		public DensityArea(
			TangentalNormalizedDirectionAstrometry astrometry,
			int maxStarsInArea,
			double raFrom, 
			double raTo, 
			double deFrom, 
			double deTo)
		{
			m_RAFrom = raFrom;
			m_RATo = raTo;
			m_DEFrom = deFrom;
			m_DETo = deTo;
			MarkedStars = 0;
			m_Astrometry = astrometry;

			m_MaxStarsInArea = maxStarsInArea;

			double x1, y1, x2, y2;
			astrometry.GetImageCoordsFromRADE(raFrom, deFrom, out x1, out y1);
			astrometry.GetImageCoordsFromRADE(raTo, deTo, out x2, out y2);

			m_XFrom = x1;
			m_YFrom = y1;
			m_XTo = x2;
			m_YTo = y2;

#if ASTROMETRY_DEBUG
			Trace.Assert(x1 < x2);
			Trace.Assert(y1 < y2);
#endif
		}

		public bool CheckAndAddStar(IStar star)
		{
			if (star.RADeg >= m_RAFrom && star.RADeg <= m_RATo &&
				star.DEDeg >= m_DEFrom && star.DEDeg <= m_DETo)
			{                
                if (IncludedStarNos.Count < m_MaxStarsInArea)
                {
                    IncludedStarNos.Add(star.StarNo);
                }
                else 
                {
                    if (DebugStarNosWithAppliedExclusions != null &&
                        DebugStarNosWithAppliedExclusions.Contains(star.StarNo))
                    {
                        Trace.Assert(false, 
							string.Format("Debug Star {0} not added as cap is reached in {1}.", star.StarNo, AreaId));
                    }
                }

			    return true;
			}

		    return false;
		}

		public bool MarkStar(IStar star)
		{
			if (IncludedStarNos.Contains(star.StarNo) &&
				!m_UsedStarNos.Contains(star.StarNo))
			{
				m_UsedStarNos.Add(star.StarNo);

				MarkedStars++;

				return MarkedStars < m_MaxStarsInArea;
			}

			return false;
		}

        public bool CheckStarReturnTrueIfParticipating(IStar star)
        {
            return IncludedStarNos.Contains(star.StarNo);
        }
	}

	public class TangentalNormalizedDirectionAstrometry
	{
		private TangentalTransRotAstrometry m_Astrometry;
		private bool m_ReverseX = false;
		private bool m_ReverseY = false;

		public TangentalNormalizedDirectionAstrometry(TangentalTransRotAstrometry baseAstrometry)
		{
			m_Astrometry = baseAstrometry;

			double fovDeg = baseAstrometry.Image.GetMaxFOVInArcSec() / 3600;
			double raFrom = baseAstrometry.m_RA0Deg - fovDeg / 2.0;
			double raTo = baseAstrometry.m_RA0Deg + fovDeg / 2.0;
			double deFrom = baseAstrometry.m_DE0Deg - fovDeg / 2.0;
			double deTo = baseAstrometry.m_DE0Deg + fovDeg / 2.0;

			double x1, y1, x2, y2;
			
			baseAstrometry.GetImageCoordsFromRADE(raFrom, deFrom, out x1, out y1);
			baseAstrometry.GetImageCoordsFromRADE(raTo, deTo, out x2, out y2);

			m_ReverseX = x1 > x2;
			m_ReverseY = y1 > y2;
		}

		public void GetRADEFromImageCoords(double x, double y, out double RADeg, out double DE)
		{
			if (m_ReverseX) x = CenterXImage - x;
			if (m_ReverseY) y = CenterYImage - y;

			m_Astrometry.GetRADEFromImageCoords(x, y, out RADeg, out DE);
		}

		public void GetImageCoordsFromRADE(double RADeg, double DE, out double x, out double y)
		{
			m_Astrometry.GetImageCoordsFromRADE(RADeg, DE, out x, out y);

			if (m_ReverseX) x = CenterXImage - x;
			if (m_ReverseY) y = CenterYImage - y;
		}

		public double CenterXImage
		{
			get { return m_Astrometry.Image.CenterXImage; }
		}

		public double CenterYImage
		{
			get { return m_Astrometry.Image.CenterYImage; }
		}
	}

}
