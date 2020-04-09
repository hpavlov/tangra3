/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.Model.Config;
using Tangra.StarCatalogues;
using Tangra.StarCatalogues.GaiaOnline;
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
		                         CatalogLocation = tangraSettings.CatalogLocation,
                                 CatalogMagnitudeBandId = tangraSettings.CatalogMagnitudeBandId,
                                 GaiaAPIToken = tangraSettings.GaiaAPIToken,
                                 UseGaiaDR2 = tangraSettings.UseGaiaDR2
		                     };
		}

	    public string CatalogNETCode
	    {
	        get
	        {
                if (m_Settings.UseGaiaDR2)
                {
                    return "GAIA-DR2";
                }

                switch (m_Settings.Catalog)
                {
                    case TangraConfig.StarCatalog.NotSpecified:
                        return string.Empty;
                    case TangraConfig.StarCatalog.UCAC2:
                        return "UCAC2";
                    case TangraConfig.StarCatalog.UCAC3:
                        return "UCAC3";
                    case TangraConfig.StarCatalog.NOMAD:
                        return "NOMAD";
                    case TangraConfig.StarCatalog.PPMXL:
                        return "PPMXL";
                    case TangraConfig.StarCatalog.UCAC4:
                        return "UCAC4";
                    default:
                        return m_Settings.Catalog.ToString();
                }
	        }
	    }

	    public CatalogMagnitudeBand[] CatalogMagnitudeBands
	    {
            get
            {
                if (m_Settings.UseGaiaDR2)
                {
                    return GaiaTapCatalogue.CatalogMagnitudeBands;
                }

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

		public List<IStar> GetStarsInRegion(double raDeg, double deDeg, double diameterDeg, double limitMag, float epoch, IWin32Window parentWindow = null)
		{
		    IStarCatalogue catalogue;
			if (m_Settings.Catalog == TangraConfig.StarCatalog.UCAC2)
			{
                catalogue = new UCAC2Catalogue(m_Settings.CatalogLocation);
			}
			else if (m_Settings.Catalog == TangraConfig.StarCatalog.UCAC3)
			{
                catalogue = new UCAC3Catalogue(m_Settings.CatalogLocation);
			}
			else if (m_Settings.Catalog == TangraConfig.StarCatalog.NOMAD)
			{
                catalogue = new NOMADCatalogue(m_Settings.CatalogLocation);
			}
            else if (m_Settings.Catalog == TangraConfig.StarCatalog.PPMXL)
            {
                catalogue = new PPMXLCatalogue(m_Settings.CatalogLocation);
            }
            else if (m_Settings.Catalog == TangraConfig.StarCatalog.UCAC4)
            {
                catalogue = new UCAC4Catalogue(m_Settings.CatalogLocation);
            }
            else
            {
                return null;
            }

		    if (m_Settings.UseGaiaDR2)
		    {
                var gaiaCatalogue = new GaiaCatalogue(catalogue, m_Settings.GaiaAPIToken);
                return gaiaCatalogue.GetStarsInRegion(raDeg, deDeg, diameterDeg, limitMag, epoch, parentWindow);
		    }
		    else
		    {
                return catalogue.GetStarsInRegion(raDeg, deDeg, diameterDeg, limitMag, epoch);
		    }
			
		}

        public double ConvertMagnitude(double measuredMag, double vrColorIndex, Guid catalogMagBand, TangraConfig.MagOutputBand magOutputBand)
        {
            if (m_Settings.UseGaiaDR2)
            {
                return GaiaTapCatalogue.ConvertMagnitude(measuredMag, vrColorIndex, catalogMagBand, magOutputBand);
            }

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

        object[] ICatalogValidator.MagnitudeBandsForCatalog(TangraConfig.StarCatalog catalog, bool useGaia)
        {
            return MagnitudeBandsForCatalog(catalog, useGaia);
        }

        public static object[] MagnitudeBandsForCatalog(TangraConfig.StarCatalog catalog, bool useGaia)
		{
            if (useGaia)
                return GaiaTapCatalogue.CatalogMagnitudeBands;

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
