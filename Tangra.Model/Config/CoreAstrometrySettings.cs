using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.Model.Config
{
	public class CoreAstrometrySettings
	{
		public static CoreAstrometrySettings Default = new CoreAstrometrySettings();

		public float IdentifiedObjectResidualInArcSec = 4.5f;
		public int PreMeasureSearchCentroidRadius = 5;

		public int MaxStarsForSeeingCalculation = 25;
		public int MinDistanceInPixelsForSeeingCalculation = 10;

	    public bool UseQuickAlign = true;

	    public bool SearchAreaAuto = true;

	    public double SearchArea = 10;

	    public int OnlineObjectIdentificationArea = 20; // pixels

	}
}
