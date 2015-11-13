/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.Model.Config;
using Tangra.StarCatalogues;
using Tangra.StarCatalogues.NOMAD;
using Tangra.StarCatalogues.PPMXL;
using Tangra.StarCatalogues.UCAC2;
using Tangra.StarCatalogues.UCAC3;
using Tangra.StarCatalogues.UCAC4;

namespace Tangra.Astrometry.Recognition
{
	public class StarCatalogueFacade : ICatalogValidator
	{
        private TangraConfig.CatalogSettings m_Settings;

		public StarCatalogueFacade(TangraConfig.CatalogSettings tangraSettings)
		{
		    m_Settings = new TangraConfig.CatalogSettings()
		                     {
		                         Catalog = (TangraConfig.StarCatalog) tangraSettings.Catalog,
		                         CatalogLocation = tangraSettings.CatalogLocation
		                     };
		}

	    public string CatalogNETCode
	    {
	        get
	        {
	            return m_Settings.Catalog.ToString();
	        }
	    }

	    public CatalogMagnitudeBand[] CatalogMagnitudeBands
	    {
            get
            {
			    if (m_Settings.Catalog == TangraConfig.StarCatalog.UCAC2)
			    {
				    return UCAC2Catalogue.CatalogMagnitudeBands;
			    }
			    else if (m_Settings.Catalog == TangraConfig.StarCatalog.UCAC3)
			    {
				    return UCAC3Catalogue.CatalogMagnitudeBands;
			    }
			    else if (m_Settings.Catalog == TangraConfig.StarCatalog.NOMAD)
			    {
				    return NOMADCatalogue.CatalogMagnitudeBands;
			    }
                else if (m_Settings.Catalog == TangraConfig.StarCatalog.PPMXL)
                {
                    return PPMXLCatalogue.CatalogMagnitudeBands;
                }
                else if (m_Settings.Catalog == TangraConfig.StarCatalog.UCAC4)
                {
                    return UCAC4Catalogue.CatalogMagnitudeBands;
                }                

			    return new CatalogMagnitudeBand[]{ };
            }
	    }

		public List<IStar> GetStarsInRegion(double raDeg, double deDeg, double diameterDeg, double limitMag, float epoch)
		{
			if (m_Settings.Catalog == TangraConfig.StarCatalog.UCAC2)
			{
				UCAC2Catalogue cat = new UCAC2Catalogue(m_Settings.CatalogLocation);
				List<IStar> ucac2Stars = cat.GetStarsInRegion(raDeg, deDeg, diameterDeg, limitMag, epoch);
				return (List<IStar>)ucac2Stars;
			}
			else if (m_Settings.Catalog == TangraConfig.StarCatalog.UCAC3)
			{
				UCAC3Catalogue cat = new UCAC3Catalogue(m_Settings.CatalogLocation);
				List<IStar> ucac3Stars = cat.GetStarsInRegion(raDeg, deDeg, diameterDeg, limitMag, epoch);
				return (List<IStar>)ucac3Stars;
			}
			else if (m_Settings.Catalog == TangraConfig.StarCatalog.NOMAD)
			{
				NOMADCatalogue cat = new NOMADCatalogue(m_Settings.CatalogLocation);
				List<IStar> nomadStars = cat.GetStarsInRegion(raDeg, deDeg, diameterDeg, limitMag, epoch);
				return (List<IStar>)nomadStars;
			}
            else if (m_Settings.Catalog == TangraConfig.StarCatalog.PPMXL)
            {
                PPMXLCatalogue cat = new PPMXLCatalogue(m_Settings.CatalogLocation);
                List<IStar> ppmxlStars = cat.GetStarsInRegion(raDeg, deDeg, diameterDeg, limitMag, epoch);
                return (List<IStar>)ppmxlStars;
            }
            else if (m_Settings.Catalog == TangraConfig.StarCatalog.UCAC4)
            {
                UCAC4Catalogue cat = new UCAC4Catalogue(m_Settings.CatalogLocation);
                List<IStar> ucac4Stars = cat.GetStarsInRegion(raDeg, deDeg, diameterDeg, limitMag, epoch);
                return (List<IStar>)ucac4Stars;
            }
			return null;
		}

        public double ConvertMagnitude(double measuredMag, double vrColorIndex, Guid catalogMagBand, TangraConfig.MagOutputBand magOutputBand)
        {
            if (m_Settings.Catalog == TangraConfig.StarCatalog.UCAC2)
            {
                return UCAC2Catalogue.ConvertMagnitude(measuredMag, vrColorIndex, catalogMagBand, magOutputBand);
            }
            else if (m_Settings.Catalog == TangraConfig.StarCatalog.UCAC3)
            {
                return UCAC3Catalogue.ConvertMagnitude(measuredMag, vrColorIndex, catalogMagBand, magOutputBand);
            }
            else if (m_Settings.Catalog == TangraConfig.StarCatalog.NOMAD)
            {
                return NOMADCatalogue.ConvertMagnitude(measuredMag, vrColorIndex, catalogMagBand, magOutputBand);
            }
            else if (m_Settings.Catalog == TangraConfig.StarCatalog.PPMXL)
            {
                return PPMXLCatalogue.ConvertMagnitude(measuredMag, vrColorIndex, catalogMagBand, magOutputBand);
            }
            else if (m_Settings.Catalog == TangraConfig.StarCatalog.UCAC4)
            {
                return UCAC4Catalogue.ConvertMagnitude(measuredMag, vrColorIndex, catalogMagBand, magOutputBand);
            }

            return double.NaN;
        }

        bool ICatalogValidator.IsValidCatalogLocation(TangraConfig.StarCatalog catalog, ref string folderPath)
        {
            return IsValidCatalogLocation(catalog, ref folderPath);
        }

	    public static bool IsValidCatalogLocation(TangraConfig.StarCatalog catalog, ref string folderPath)
		{
			if (catalog == TangraConfig.StarCatalog.UCAC2)
				return UCAC2Catalogue.IsValidCatalogLocation(ref folderPath);
			else if (catalog == TangraConfig.StarCatalog.NOMAD)
				return NOMADCatalogue.IsValidCatalogLocation(ref folderPath);
			else if (catalog == TangraConfig.StarCatalog.UCAC3)
				return UCAC3Catalogue.IsValidCatalogLocation(ref folderPath);
            else if (catalog == TangraConfig.StarCatalog.PPMXL)
                return PPMXLCatalogue.IsValidCatalogLocation(ref folderPath);
            else if (catalog == TangraConfig.StarCatalog.UCAC4)
                return UCAC4Catalogue.IsValidCatalogLocation(ref folderPath);

			return false;
		}

        bool ICatalogValidator.VerifyCurrentCatalogue(TangraConfig.StarCatalog catalog, ref string path)
        {
            return VerifyCurrentCatalogue(catalog, ref path);
        }

	    public static bool VerifyCurrentCatalogue(TangraConfig.StarCatalog catalog, ref string path)
		{
			if (catalog == TangraConfig.StarCatalog.UCAC2)
			{
				if (!UCAC2Catalogue.IsValidCatalogLocation(ref path))
					return false;

				if (!UCAC2Catalogue.CheckAndWarnIfNoBSS(path, null))
					return false;
			}
			else if (catalog == TangraConfig.StarCatalog.NOMAD)
			{
				if (!NOMADCatalogue.IsValidCatalogLocation(ref path))
					return false;

				// TODO: Check index files??
			}
			else if (catalog == TangraConfig.StarCatalog.UCAC3)
			{
				if (!UCAC3Catalogue.IsValidCatalogLocation(ref path))
					return false;

			}
            else if (catalog == TangraConfig.StarCatalog.PPMXL)
            {
                if (!PPMXLCatalogue.IsValidCatalogLocation(ref path))
                    return false;

            }
            else if (catalog == TangraConfig.StarCatalog.UCAC4)
            {
                if (!UCAC4Catalogue.IsValidCatalogLocation(ref path))
                    return false;

            }

			return true;
		}

        object[] ICatalogValidator.MagnitudeBandsForCatalog(TangraConfig.StarCatalog catalog)
        {
            return MagnitudeBandsForCatalog(catalog);
        }

	    public static object[] MagnitudeBandsForCatalog(TangraConfig.StarCatalog catalog)
		{
			if (catalog == TangraConfig.StarCatalog.UCAC2)
				return UCAC2Catalogue.CatalogMagnitudeBands;
			else if (catalog == TangraConfig.StarCatalog.NOMAD)
				return NOMADCatalogue.CatalogMagnitudeBands;
			else if (catalog == TangraConfig.StarCatalog.UCAC3)
				return UCAC3Catalogue.CatalogMagnitudeBands;
            else if (catalog == TangraConfig.StarCatalog.PPMXL)
                return PPMXLCatalogue.CatalogMagnitudeBands;
            else if (catalog == TangraConfig.StarCatalog.UCAC4)
                return UCAC4Catalogue.CatalogMagnitudeBands;

			return new object[] { };
		}

	}
}
