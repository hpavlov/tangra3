using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.Model.Helpers
{
    public static class JulianDayHelper
    {
        public static double JDUtcAtDate(DateTime date)
        {
            double tNow = (double)date.Ticks - 6.30822816E+17;	// .NET ticks at 01-Jan-2000T00:00:00
            double j = 2451544.5 + (tNow / 8.64E+11);		// Tick difference to days difference
            return j;
        }

	    public static DateTime DateTimeAtJD(double jd)
	    {
		    long ticks = (long)Math.Round(6.30822816E+17 + (jd - 2451544.5)*8.64E+11);
		    return new DateTime(ticks);
	    }
    }
}
