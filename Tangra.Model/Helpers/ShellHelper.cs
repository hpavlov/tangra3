using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tangra.Model.Helpers
{
    public static class ShellHelper
    {
        public static void OpenUrl(string url)
        {
            try
            {
                Process.Start(url);
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
