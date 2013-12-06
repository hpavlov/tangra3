using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Win32;

namespace Tangra.OccultTools
{
    public class OccultWatcherHelper
    {
        public static string GetConfiguredOccultLocation()
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"Software\OccultWatcher");
            if (registryKey != null)
            {
                return Convert.ToString(registryKey.GetValue("OccultSetupDir", null));
            }

            return null;
        }
    }
}
