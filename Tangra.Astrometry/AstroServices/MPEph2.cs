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
	public class MPEph2
	{
		public static MPEphEntry GetCoordinatesForSingleDate(double longitude, double latitude, string objectDesgn, DateTime utcTime)
		{
			double hourOfTheDay = utcTime.Hour + utcTime.Minute / 60.0 + (utcTime.Second + (utcTime.Millisecond / 1000.0)) / 3600.0;
            string postData =
				string.Format("ty=e&TextArea={0}&d={1}&l=1&i=&u=d&uto={2}&c=&long={3}&lat={4}&alt=&raty=a&s=t&m=m&adir=S&oed=&e=-2&resoc=&tit=&bu=&ch=c&ce=f&js=f",
					objectDesgn,
					utcTime.ToString("yyyy-MM-dd"),
					hourOfTheDay.ToString("0.0000"),
					longitude.ToString("0.0000", CultureInfo.InvariantCulture),
					latitude.ToString("0.0000", CultureInfo.InvariantCulture));

            return GetCoordinates(TangraConfig.Settings.Urls.MPCEphe2ServiceUrl, postData);
		}

		public static MPEphEntry GetCoordinatesForSingleDate(string observatoryCode, string objectDesgn, DateTime utcTime)
		{
			double hourOfTheDay = utcTime.Hour + utcTime.Minute / 60.0 + (utcTime.Second + (utcTime.Millisecond / 1000.0)) / 3600.0;
            string postData =
				string.Format("ty=e&TextArea={0}&d={1}&l=1&i=&u=d&uto={2}&c={3}&long=&lat=&alt=&raty=a&s=t&m=m&adir=S&oed=&e=-2&resoc=&tit=&bu=&ch=c&ce=f&js=f",
					objectDesgn,
					utcTime.ToString("yyyy-MM-dd"),
					hourOfTheDay.ToString("0.0000"),
					observatoryCode);

            return GetCoordinates(TangraConfig.Settings.Urls.MPCEphe2ServiceUrl, postData);
		}

        private static MPEphEntry GetCoordinates(string requestUrl, string postData)
		{
			try
			{
				HttpWebRequest req = (HttpWebRequest)WebRequest.Create(requestUrl);

                Version ver = Assembly.GetExecutingAssembly().GetName().Version;
                req.UserAgent = "Tangra, ver " + ver.ToString();

			    req.Method = "POST";
			    byte[] postBytes = Encoding.ASCII.GetBytes(postData);                
                req.ContentType = "application/x-www-form-urlencoded";
			    req.ContentLength = postBytes.Length;

                using(Stream reqStream = req.GetRequestStream())
                {
                    reqStream.Write(postBytes, 0, postBytes.Length);
                }

				HttpWebResponse response = (HttpWebResponse)req.GetResponse();
				Stream responseStream = response.GetResponseStream();
				string responseString = null;
				using (StreamReader reader = new StreamReader(responseStream, Encoding.UTF8))
				{
					responseString = reader.ReadToEnd();
				}



				int startIdx = responseString.IndexOf("<b>");
				int endIdx = responseString.IndexOf("</b>");

				string objectName = responseString.Substring(startIdx + 3, endIdx - startIdx - 3).Trim();

				startIdx = responseString.IndexOf("<pre>");
				endIdx = responseString.IndexOf("</pre>");

				List<MPEphEntry> parsedData = MPEph2ResponseParser.Parse(responseString.Substring(startIdx + 5, endIdx - startIdx - 5));

				if (parsedData.Count > 0)
				{
					parsedData[0].ObjectName = objectName;
					return parsedData[0];
				}
				else
					return null;
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.ToString());
				return null;
			}
		}

		public class MPEphEntry : IIdentifiedObject
		{
			public DateTime UtcDate;
			public double RAHours { get; set; }
			public double DEDeg { get; set; }
			public string ObjectName { get; set; }
			public double Mag { get; set; }

            public bool IsCheckedAgainstSolvedPlate { get; set; }

            public void MarkCheckedAgainstSolvedPlate(double raHours, double deDeg)
            {
                IsCheckedAgainstSolvedPlate = true;
                RAHours = raHours;
                DEDeg = deDeg;
            }

		}

		internal static class MPEph2ResponseParser
		{
			internal static List<MPEphEntry> Parse(string preTagContent)
			{
				List<MPEphEntry> result = new List<MPEphEntry>();

				string[] lines = preTagContent.Split(new string[] { "\n" }, StringSplitOptions.None);
				foreach (string line in lines)
				{
					Trace.WriteLine(line);
					if (line.StartsWith("20"))
					{
						MPEphEntry entry = ParseLine(line);
						if (entry != null) result.Add(entry);
					}
				}

				return result;
			}


			private static MPEphEntry ParseLine(string line)
			{
				// 00055
				// Date       UT      R.A. (J2000) Decl.    Delta     r     El.    Ph.   V      Sky Motion        Object    Sun   Moon                Uncertainty info
				//            h m s                                                            "/min    P.A.    Azi. Alt.  Alt.  Phase Dist. Alt.    3-sig/" P.A.
				//                                                                                                     1         1         1         1         1
				//           1         2         3         4         5         6         7         8         9         0         1         2         3         4    
				// 012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890
				// 2009 07 27 124800 22 18 36.1 -20 15 18   1.545   2.497  153.8  10.3  11.4    0.40    249.0    264  +47   -68   0.36   130  +03         0 318.1
                // 2010 12 11 130549 07 34 48.3 -17 52 01   0.342   1.220  126.5  40.5   7.9    0.54    235.0    261  +46   -32   0.31   137  -03

				try
				{
					MPEphEntry retVal = new MPEphEntry();

					int year = int.Parse(line.Substring(0, 4).Trim());
					int month = int.Parse(line.Substring(5, 2).Trim());
					int day = int.Parse(line.Substring(8, 2).Trim());
					int hr = int.Parse(line.Substring(11, 2).Trim());
					int min = int.Parse(line.Substring(13, 2).Trim());
					int sec = int.Parse(line.Substring(15, 2).Trim());

					retVal.UtcDate = new DateTime(year, month, day, hr, min, sec);
					retVal.RAHours = AstroConvert.ToRightAcsension(line.Substring(18, 10).Trim());
					retVal.DEDeg = AstroConvert.ToDeclination(line.Substring(29, 9).Trim());
					retVal.Mag = double.Parse(line.Substring(69, 4).Trim(), CultureInfo.InvariantCulture);

					return retVal;
				}
				catch
				{
					return null;
				}
			}
		}
	}
}
