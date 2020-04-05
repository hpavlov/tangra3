using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Tangra.Model.Helpers
{
    public static class ShellHelper
    {
        private static string defaultBrowser = null;

        public static void OpenUrl(string url)
        {
            string browser = string.Empty;
            RegistryKey key = null;
            try
            {
                key = Registry.ClassesRoot.OpenSubKey(@"ChromeHTML\shell\open\command");
                if (key != null)
                {
                    // Get default Browser
                    browser = key.GetValue(null).ToString().ToLower().Trim(new[] {'"'});
                }
                if (!browser.EndsWith("exe"))
                {
                    //Remove all after the ".exe"
                    defaultBrowser = browser.Substring(0, browser.LastIndexOf(".exe", StringComparison.InvariantCultureIgnoreCase) + 4);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                defaultBrowser = null;
            }
            finally
            {
                if (key != null)
                {
                    key.Close();
                }
            }

            try
            {
                if (defaultBrowser != null)
                {
                    Process.Start(defaultBrowser, url);
                }
                else
                {
                    Process.Start(url);
                }
            }
            catch
            {
                MessageBox.Show("Cannot open url " + url, "Tangra", MessageBoxButtons.OK , MessageBoxIcon.Error);
            }
        }

		public static void OpenFile(string filePath)
		{
			try
			{
				Process.Start(filePath);
			}
			catch
			{
				MessageBox.Show("Cannot open file:\r\n\r\n" + filePath + "\r\n\r\nThere may be no program that is registered to open this type of files on your system.", "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
    }
}
