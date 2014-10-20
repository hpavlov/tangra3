/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;

#if WIN32
using System.Windows.Forms;
using Microsoft.Win32;
#endif
using Tangra.Model.Helpers;
using Tangra.VideoOperations.LightCurves.Report;

namespace Tangra.Helpers
{
    public class OccultWatcherHelper
    {
#if WIN32
		private const string REGKEY_INCOMING_EVENTS = @"Software\OccultWatcherReporting\IncomingEventReports";
		private const string REGKEY_USED_EVENTS = @"Software\OccultWatcherReporting\UsedEventReports";

        [Conditional("WIN32")]
        public static void NotifyOccultWatcherIfInstalled(EventTimesReport report, IWin32Window parentForm)
        {
            bool owNotified = false;

            if (!report.ReportFileSaved || report.ReportFileName == null)
                return;

	        var usedReports = new List<string>();

			try
			{
				RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(REGKEY_USED_EVENTS, true);
				if (registryKey != null)
				{
					string[] allValueNames = registryKey.GetValueNames();
					foreach (string valName in allValueNames)
					{
						string savedReportLocation = Convert.ToString(registryKey.GetValue(valName));
						if (File.Exists(savedReportLocation))
							usedReports.Add(savedReportLocation);
					}
				}
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex);
			}
			

            try
            {
				RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(REGKEY_INCOMING_EVENTS, true);
                if (registryKey != null)
                {
	                var availableList = new List<string>();
                    string[] allValueNames = registryKey.GetValueNames();
                    foreach (string valName in allValueNames)
                    {
                        string savedReportLocation = Convert.ToString(registryKey.GetValue(valName));
						if (usedReports.IndexOf(savedReportLocation) > -1)
						{
							// File already used. We need to delete the value from the incoming events
							registryKey.DeleteValue(valName);
						}
						else
						{
							availableList.Add(savedReportLocation);

							if (report.ReportFileName.Equals(savedReportLocation, StringComparison.InvariantCultureIgnoreCase))
							{
								// File has been saved already and hasn't been used yet. Nothing to do.
								owNotified = true;
							}
						}
                    }

                    if (!owNotified)
                    {
                        availableList.Add(report.ReportFileName);
                        owNotified = true;
                    }

                    for (int i = 0; i < allValueNames.Length; i++)
					{
						registryKey.DeleteValue(allValueNames[0], false);
					}

					for (int i = 0; i < availableList.Count; i++)
	                {
						registryKey.SetValue(i.ToString(), availableList[i]);    
	                }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.GetFullStackTrace());
            }
            finally
            {
                if (owNotified)
                {
                    MessageBox.Show(
                        parentForm,
                        "The light curve analysis results have been saved and made available for the OccultWatcher IOTA Reporting Add-in.\r\n\r\nRight click in OccultWatcher on the corresponding event and choose 'Report Observation'. Once you have submitted your report press 'Prefill Report File' to pre-populate your report with the information made available by Tangra.",
                        "Tangra3 - " + report.AddinAction,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
        }


        [Conditional("WIN32")]
        public static void GetConfiguredOccultLocation(ref string location)
        {
            location = null;

            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"Software\OccultWatcher");
            if (registryKey != null)
            {
                location = Convert.ToString(registryKey.GetValue("OccultSetupDir", null));
            }
        }

		public static bool IsOccultWatcherFound()
		{
			RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"Software\OccultWatcher");
			return registryKey != null;
		}

		public static bool IsOccultWatcherReportingAddinFound()
		{
			RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(REGKEY_INCOMING_EVENTS);
			return registryKey != null;
		}
#endif
    }
}
