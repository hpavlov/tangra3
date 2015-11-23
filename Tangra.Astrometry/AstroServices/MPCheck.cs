/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using Tangra.Model.Config;
using Tangra.Model.Helpers;

namespace Tangra.AstroServices
{
	public class MPCheck
	{
		public static List<MPCheckEntry> CheckRegion(
			DateTime utcDate, double raDeg, double deDeg, double radiusArcSec, double magLimit, string observatoryCode)
		{
			double dayPart = utcDate.Day + (utcDate.Hour + utcDate.Minute / 60.0 + (utcDate.Second + (utcDate.Millisecond / 1000.0)) / 3600.0) / 24.0;

			string requestUrl =
				string.Format(
                    TangraConfig.Settings.Urls.MPCheckServiceUrl + "?year={0}&month={1}&day={2}&which=pos&ra={3}&decl={4}&TextArea=&radius={5}&limit={6}&oc={7}&sort=d&mot=h&tmot=s&pdes=u&needed=f&ps=n&type=p",
					utcDate.Year, utcDate.Month, dayPart.ToString("0.000"),
					AstroConvert.ToStringValue(raDeg / 15.0, "HH MM SS"),
					AstroConvert.ToStringValue(deDeg, "+DD MM SS"),
					radiusArcSec.ToString("0"),
					magLimit.ToString("0.0"),
					observatoryCode);

			try
			{
				HttpWebRequest req = (HttpWebRequest)WebRequest.Create(requestUrl);

                Version ver = Assembly.GetExecutingAssembly().GetName().Version;
                req.UserAgent = "Tangra, ver " + ver.ToString();

				HttpWebResponse response = (HttpWebResponse)req.GetResponse();
				Stream responseStream = response.GetResponseStream();
				string responseString = null;
				using (StreamReader reader = new StreamReader(responseStream, Encoding.UTF8))
				{
					responseString = reader.ReadToEnd();
				}

				int startIdx = responseString.IndexOf("<pre>");
				int endIdx = responseString.IndexOf("</pre>");

				List<MPCheckEntry> parsedData = ParseMPCheckResponse(responseString.Substring(startIdx + 5, endIdx - startIdx - 5));

				return parsedData;
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.ToString());
				return null;
			}
		}

		private static List<MPCheckEntry> ParseMPCheckResponse(string preTagContent)
		{
			List<MPCheckEntry> result = new List<MPCheckEntry>();

			string[] lines = preTagContent.Split(new string[] { "\n" }, StringSplitOptions.None);
			foreach (string line in lines)
			{
				Trace.WriteLine(line);
				int testVal;
				if (line.Length > 28 &&
					int.TryParse(line.Substring(25, 2), out testVal))
				{
					MPCheckEntry entry = ParseLine(line);
					if (entry != null) result.Add(entry);
				}
			}

			return result;
		}

		private static MPCheckEntry ParseLine(string line)
		{
			// Object designation         R.A.      Decl.     V       Offsets     Motion/hr   Orbit  <a href="http://www.cfa.harvard.edu/iau/info/FurtherObs.html">Further observations?</a>
			//                           h  m  s     &#176;  '  "        R.A.   Decl.  R.A.  Decl.        Comment (Elong/Decl/V at date 1)
			// 
			//                                                                                                     1         1         1         1         1
			//          1         2         3         4         5         6         7         8         9         0         1         2         3         4    
			//012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890
			//    (55) Pandora         22 18 26.2 -20 16 13  11.4   4.4W   7.2N    21-     8-   56o  None needed at this time.
			//(148989) 2001 YM73       22 18 35.1 -20 08 33  19.8   2.3W  14.9N    19-    17-    5o  None needed at this time.
			//         2008 HM2        22 17 34.4 -20 16 40  19.9  16.6W   6.8N    20-    12-    5o  Very desirable between 2009 Aug. 27-Sept. 26.  (166.0,-22.5,19.7)
			//  (1448) Lindbladia      22 18 04.5 -20 02 00  16.7   9.5W  21.5N    25-    13-   24o  None needed at this time.

			try
			{
				MPCheckEntry entry = new MPCheckEntry();
				entry.ObjectName = line.Substring(0, 22).Trim();
				entry.RAHours = AstroConvert.ToRightAcsension(line.Substring(25, 10).Trim());
				entry.DEDeg = AstroConvert.ToDeclination(line.Substring(36, 9).Trim());

			    string magStr = line.Substring(47, 4).Trim();
                entry.Mag = string.IsNullOrEmpty(magStr) 
                    ? 30 
                    : double.Parse(magStr, CultureInfo.InvariantCulture);

				return entry;
			}
			catch
			{
				return null;
			}
		}
	}


    public interface IIdentifiedObject
    {
        string ObjectName { get; }
        double RAHours { get; }
        double DEDeg { get; }
        double Mag { get; }
    }

	public class MPCheckEntry : IIdentifiedObject
	{
		public string ObjectName { get; set; }
		public double RAHours { get; set; }
		public double DEDeg { get; set; }
		public double Mag { get; set; }
	}

}
