using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.Model.Config;
using Tangra.Properties;

namespace Tangra.Helpers
{
    internal class ApplicationSettingsSerializer : ISettingsSerializer
    {
        public static ApplicationSettingsSerializer Instance = new ApplicationSettingsSerializer();

        public string LoadSettings()
        {
            return Settings.Default.TangraSettings;
        }

        public void SaveSettings(string settings)
        {
            Settings.Default.TangraSettings = settings;
            Settings.Default.Save();
        }

        public string LoadRecentFiles()
        {
            return Settings.Default.TangraRecentFiles;
        }

        public void SaveRecentFiles(string settings)
        {
            Settings.Default.TangraRecentFiles = settings;
            Settings.Default.Save();            
        }
    }
}
