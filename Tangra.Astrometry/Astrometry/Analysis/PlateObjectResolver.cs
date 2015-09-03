/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using Tangra.Astrometry.Recognition;
using Tangra.Model.Astro;
using Tangra.Model.Config;
using Tangra.Model.Helpers;
using Tangra.Model.Image;
using Tangra.Model.VideoOperations;
using Tangra.Photometry;
using Tangra.StarCatalogues;

namespace Tangra.Astrometry.Analysis
{
	public class PlateObjectResolver
	{
		private IAstrometryController m_AstrometryController;
		private IVideoController m_VideoController;

		private AstroImage m_Image;
		private LeastSquareFittedAstrometry m_Astrometry;
		private List<IStar> m_Stars;

		private double m_BackgroundFlux;
		private double m_BackgroundMag;

		private StarMagnitudeFit m_MagnitudeFit;
		private IAstrometrySettings m_AstrometrySettings;
		private double m_MaxMagForAstrometry;

		private Dictionary<PSFFit, double> m_UidentifiedObjects = new Dictionary<PSFFit, double>();
		private Dictionary<PSFFit, IStar> m_IdentifiedObjects = new Dictionary<PSFFit, IStar>();
		private Dictionary<PSFFit, double> m_UnknownObjects = new Dictionary<PSFFit, double>();

		public Dictionary<PSFFit, double> UidentifiedObjects
		{
			get
			{
				return m_UidentifiedObjects;
			}
		}

		public Dictionary<PSFFit, IStar> IdentifiedObjects
		{
			get
			{
				return m_IdentifiedObjects;
			}
		}

		public Dictionary<PSFFit, double> UnknownObjects
		{
			get
			{
				return m_UnknownObjects;
			}
		}

		public double BackgroundFlux { get { return m_BackgroundFlux; } }

		public double BackgroundMag { get { return m_BackgroundMag; } }

		public PlateObjectResolver(
			IAstrometryController astrometryController,
			IVideoController videoController,
			AstroImage image,
			LeastSquareFittedAstrometry impSol,
			List<IStar> stars,
			double maxMagForAstrometry)
		{
			m_AstrometryController = astrometryController;
			m_VideoController = videoController;

			m_Image = image;
			m_Astrometry = impSol;
			m_Stars = stars;
			m_MaxMagForAstrometry = maxMagForAstrometry;
		}

		public Bitmap ResolveObjects(
			TangraConfig.PhotometryReductionMethod photometryReductionMethod,
			TangraConfig.PsfQuadrature psfQuadrature,
			TangraConfig.PsfFittingMethod psfFittingMethod,
			TangraConfig.BackgroundMethod backgroundMethod,
			TangraConfig.PreProcessingFilter filter,
			Guid magnitudeBandId,
			Rectangle osdRectangleToExclude,
			Rectangle rectToInclude,
			bool limitByInclusion,
			IAstrometrySettings astrometrySettings,
			ObjectResolverSettings objectResolverSettings)
		{
			m_AstrometrySettings = astrometrySettings;

			StarMap starMap = new StarMap(
				astrometrySettings.PyramidRemoveNonStellarObject,
				astrometrySettings.MinReferenceStarFWHM,
				astrometrySettings.MaxReferenceStarFWHM,
				astrometrySettings.MaximumPSFElongation,
				astrometrySettings.LimitReferenceStarDetection);

			starMap.FindBestMap(StarMapInternalConfig.Default, m_Image, osdRectangleToExclude, rectToInclude, limitByInclusion);

			float r0 = 0;

			m_MagnitudeFit = StarMagnitudeFit.PerformFit(
				m_AstrometryController,
				m_VideoController,
				m_Image.Pixelmap.BitPixCamera,
                m_Image.Pixelmap.MaxSignalValue,
				m_Astrometry.FitInfo,
				photometryReductionMethod,
				psfQuadrature,
				psfFittingMethod,
				backgroundMethod,
				filter,
				m_Stars,
				magnitudeBandId,
				1.0f,
				null, null, null,
				ref r0);


			m_BackgroundFlux = m_MagnitudeFit.GetBackgroundIntencity();
			m_BackgroundMag = m_MagnitudeFit.GetMagnitudeForIntencity(m_BackgroundFlux);

			Trace.WriteLine(string.Format("Plate FWHM: {0}", 2 * Math.Sqrt(Math.Log(2)) * r0));

			PeakPixelResolver resolver = new PeakPixelResolver(m_Image);
			resolver.ResolvePeakPixels(osdRectangleToExclude, rectToInclude, limitByInclusion, objectResolverSettings.ExcludeEdgeAreaPixels, objectResolverSettings.MinDistanceBetweenPeakPixels);

			List<double> identifiedMagnitudes = new List<double>();
			List<double> identifiedR0s = new List<double>();

			m_IdentifiedObjects.Clear();
			m_UidentifiedObjects.Clear();

			foreach(KeyValuePair<int, int> peakPixel in resolver.PeakPixels.Keys)
			{
				int x = peakPixel.Key;
				int y = peakPixel.Value;

				bool isSaturated;
				double intencity = m_MagnitudeFit.GetIntencity(new ImagePixel(255, x, y), out isSaturated);
				double magnitude = m_MagnitudeFit.GetMagnitudeForIntencity(intencity);

				if (magnitude < m_MaxMagForAstrometry)
				{
					double RADeg, DEDeg;

					PSFFit fit;
					starMap.GetPSFFit(x, y, PSFFittingMethod.NonLinearFit, out fit);

					
					if (fit.IMax < 0) continue;
					if (fit.IMax < fit.I0) continue;
					if (fit.Certainty < objectResolverSettings.MinCertainty) continue;
					if (fit.FWHM < objectResolverSettings.MinFWHM) continue;
					if (fit.IMax - fit.I0 < objectResolverSettings.MinAmplitude) continue;

					m_Astrometry.GetRADEFromImageCoords(fit.XCenter, fit.YCenter, out RADeg, out DEDeg);

					// All stars closer than 2 arcsec to this position
					List<IStar> matchingStars = m_Stars.Where(s => Math.Abs(AngleUtility.Elongation(s.RADeg, s.DEDeg, RADeg, DEDeg) * 3600.0) < objectResolverSettings.MaxStarMatchMagDif).ToList();

					bool identified = false;
					if (matchingStars.Count >= 1)
					{
						foreach(IStar star in matchingStars)
						{
							if (objectResolverSettings.MaxStarMatchMagDif >= Math.Abs(magnitude - star.Mag))
							{
								// The star is identified. Do we care more?
								Trace.WriteLine(string.Format("STAR ({0}, {1}) No -> {2}; Mag -> {3} (Expected: {4}); R0 = {5}",
									x, y, star.StarNo, magnitude.ToString("0.00"), star.Mag.ToString("0.00"), fit.R0.ToString("0.0")));
								identifiedMagnitudes.Add(magnitude);
								identifiedR0s.Add(fit.R0);
								m_IdentifiedObjects.Add(fit, star);
								identified = true;
								break;
							}
						}
					}

					if (matchingStars.Count == 0 || 
						!identified)
					{
						// The object is not in the star database

						// TODO: Test for hot pixel. Match to hot pixel profile from the brightest pixel in the area
						m_UidentifiedObjects.Add(fit, magnitude);
					}
				}
				else
				{
					// Don't bother with too faint objects
				}
			}

			if (m_IdentifiedObjects.Count > 0)
			{
				double mean = identifiedR0s.Average();
				double variance = 0;
				foreach (double rr0 in identifiedR0s)
				{
					variance += (rr0 - mean) * (rr0 - mean);
				}
				variance = Math.Sqrt(variance / (m_IdentifiedObjects.Count - 1));
				double minR0 = mean - variance;
				double maxR0 = mean + variance;

				identifiedMagnitudes.Sort();
				double maxStarMag = identifiedMagnitudes[Math.Max(0, (int)Math.Truncate(0.9 * identifiedMagnitudes.Count))];

				Trace.WriteLine(string.Format("Max Star Mag: {0}; R0 ({1}, {2})", maxStarMag.ToString("0.00"), minR0.ToString("0.0"), maxR0.ToString("0.0")));

				// NOTE: The R0 exclusion may ignore bright comets !
				m_UnknownObjects = m_UidentifiedObjects
					.Where(p => p.Value < maxStarMag && p.Key.R0 >= minR0 && p.Key.R0 <= maxR0)
					.ToDictionary(p => p.Key, p => p.Value);

				foreach (PSFFit obj in m_UnknownObjects.Keys)
				{
					Trace.WriteLine(string.Format("UNK: ({0}, {1}) Mag -> {2}; R0 = {3}", obj.XCenter.ToString("0.0"), obj.YCenter.ToString("0.0"), m_UnknownObjects[obj].ToString("0.00"), obj.R0.ToString("0.0")));
				}
			}

			Bitmap bitmap = m_Image.Pixelmap.CreateDisplayBitmapDoNotDispose();
			using (Graphics g = Graphics.FromImage(bitmap))
			{
				foreach (PSFFit star in m_IdentifiedObjects.Keys)
				{
					float x = (float)star.XCenter;
					float y = (float)star.YCenter;

					g.DrawEllipse(Pens.GreenYellow, x - 5, y - 5, 10, 10);
				}

				foreach (PSFFit star in m_UnknownObjects.Keys)
				{
					float x = (float)star.XCenter;
					float y = (float)star.YCenter;

					g.DrawEllipse(Pens.Tomato, x - 8, y - 8, 16, 16);
				}

				g.Save();
			}

			return bitmap;
		}

	}
}
