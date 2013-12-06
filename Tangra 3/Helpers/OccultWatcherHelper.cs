using System;
using System.Collections.Generic;
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
        [Conditional("WIN32")]
        public static void NotifyOccultWatcherIfInstalled(EventTimesReport report, IWin32Window parentForm)
        {
            bool owNotified = false;

            if (!report.ReportFileSaved || report.ReportFileName == null)
                return;
            try
            {
                RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"Software\OccultWatcher\IncomingEventReports", true);
                if (registryKey != null)
                {

                    int maxValId = 0;
                    string[] allValueNames = registryKey.GetValueNames();
                    foreach (string valName in allValueNames)
                    {
                        string savedReportLocation = Convert.ToString(registryKey.GetValue(valName));
                        if (report.ReportFileName.Equals(savedReportLocation, StringComparison.InvariantCultureIgnoreCase))
                        {
                            // File has been saved already and hasn't been used yet. Nothing to do.
                            owNotified = true;
                            return;
                        }

                        int valId;
                        if (int.TryParse(valName, out valId))
                        {
                            if (valId > maxValId)
                                maxValId = valId;
                        }
                    }

                    maxValId++;
                    registryKey.SetValue(maxValId.ToString(), report.ReportFileName);
                    owNotified = true;
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
                        "The event information has been saved and made available for the OccultWatcher Reporting Add-in.\r\n\r\nRight click in OccultWatcher on the corresponding event and choose 'Report Observation' to pre-populate your report with the information made available by Tangra.",
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
    }
}
