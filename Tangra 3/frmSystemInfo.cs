/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Windows.Forms;
using Tangra.Helpers;
using Tangra.Model.Helpers;

namespace Tangra
{
    public partial class frmSystemInfo : Form
    {
        public frmSystemInfo()
        {
            InitializeComponent();
        }

        public static string GetFullVersionInfo()
        {
            OperatingSystem os = Environment.OSVersion;
            PlatformID pid = os.Platform;
            string platformInfo = "Unkn";
            switch (pid)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    platformInfo = "Windows";
                    break;
                case PlatformID.Unix:
                    platformInfo = "Unix";
                    break;
                case PlatformID.MacOSX:
                    platformInfo = "OSX";
                    break;
            }

            string productName = String.Format("{0} v{1}", AssemblyProduct, AssemblyFileVersion);

            string platformName = CurrentOS.Name;

            string platformVersion = String.Format("{0} ({1} bit)", platformInfo, Environment.Is64BitOperatingSystem ? "64" : "32");

            bool isMono = Type.GetType("Mono.Runtime") != null;

            string clrVersion = (isMono ? "Mono CLR " : "Microsoft CLR ") + Assembly.GetExecutingAssembly().ImageRuntimeVersion;

			string monoVersion = GetMonoVersion();
	        if (monoVersion != null)
				clrVersion += string.Format("\r\nMono Version {0}", monoVersion);

            string componentVersions = string.Format("Tangra Core v{0}\r\nTangra Video Engine v{1}", TangraEnvironment.TangraCoreVersion, TangraEnvironment.TangraVideoEngineVersion);

            return string.Format("{0}\r\nOS: {1}\r\nPlatform: {2}\r\nRuntime: {3}\r\n{4}",
                productName, platformName, platformVersion, clrVersion, componentVersions);            
        }

        private void frmSystemInfo_Load(object sender, EventArgs e)
        {
            string totalInfo = GetFullVersionInfo();

            tbxSystemInfo.Text = totalInfo;
            tbxSystemInfo.DeselectAll();            
        }

        public static string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public static string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public static string AssemblyFileVersion
        {
            get
            {
                object[] atts = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true);
                if (atts != null && atts.Length == 1)
                    return ((AssemblyFileVersionAttribute)atts[0]).Version;
                else
                    return AssemblyVersion;
            }
        }

	    private static string GetMonoVersion()
	    {
			try
			{
				Type type = Type.GetType("Mono.Runtime");
				if (type != null)
				{
					MethodInfo dispalayName = type.GetMethod("GetDisplayName", BindingFlags.NonPublic | BindingFlags.Static);
					if (dispalayName != null)
						return (string)dispalayName.Invoke(null, null);
				}
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex);
			}

			return null;
	    }
    }
}
