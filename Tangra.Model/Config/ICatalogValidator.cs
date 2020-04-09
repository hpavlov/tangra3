using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.Model.Config
{
	public interface ICatalogValidator
	{
		bool IsValidCatalogLocation(TangraConfig.StarCatalog catalog, ref string path);
		bool VerifyCurrentCatalogue(TangraConfig.StarCatalog catalog, ref string path);
		object[] MagnitudeBandsForCatalog(TangraConfig.StarCatalog catalog, bool useGaia);
	}
}
