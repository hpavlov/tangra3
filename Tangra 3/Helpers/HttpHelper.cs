using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tangra.Helpers
{
    public static class HttpHelper
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
    }
}
