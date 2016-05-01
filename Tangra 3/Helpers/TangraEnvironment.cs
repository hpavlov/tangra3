/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Tangra.PInvoke;

namespace Tangra.Helpers
{
    public static class TangraEnvironment
    {
        public static string TangraCoreVersion
        {
            get
            {
                return TangraCore.GetTangraCoreVersion();
            }
        }

        public static string TangraVideoEngineVersion
        {
            get
            {
                return TangraVideo.GetVideoEngineVersion();
            }
        }
    }

    // Code based on http://blez.wordpress.com/2012/09/17/determine-os-with-netmono/
    // by blez

    public static class CurrentOS
    {
        public static bool IsWindows { get; private set; }
        public static bool IsUnix { get; private set; }
        public static bool IsMac { get; private set; }
        public static bool IsLinux { get; private set; }
        public static bool IsUnknown { get; private set; }
        public static bool Is32bit { get; private set; }
        public static bool Is64bit { get; private set; }
        public static bool Is64BitProcess { get { return (IntPtr.Size == 8); } }
        public static bool Is32BitProcess { get { return (IntPtr.Size == 4); } }
        public static string Name { get; private set; }

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWow64Process([In] IntPtr hProcess, [Out] out bool wow64Process);

        private static bool Is64bitWindows
        {
            get
            {
                if ((Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor >= 1) || Environment.OSVersion.Version.Major >= 6)
                {
                    using (Process p = Process.GetCurrentProcess())
                    {
                        bool retVal;
                        if (!IsWow64Process(p.Handle, out retVal)) return false;
                        return retVal;
                    }
                }
                else return false;
            }
        }

        static CurrentOS()
        {
            IsWindows = Path.DirectorySeparatorChar == '\\';
            if (IsWindows)
            {
                Name = Environment.OSVersion.VersionString;

                Name = Name.Replace("Microsoft ", "");
                Name = Name.Replace("  ", " ");
                Name = Name.Replace(" )", ")");
                Name = Name.Trim();

                Name = Name.Replace("NT 6.2", "8 6.2");
                Name = Name.Replace("NT 6.1", "7 6.1");
                Name = Name.Replace("NT 6.0", "Vista 6.0");
                Name = Name.Replace("NT 5.", "XP 5.");
                Name = Name + (Is64bitWindows ? " (64 bit)" : " (32 bit)");

                if (Is64bitWindows)
                    Is64bit = true;
                else
                    Is32bit = true;
            }
            else
            {
                string UnixName = ReadProcessOutput("uname");
                if (UnixName.Contains("Darwin"))
                {
                    IsUnix = true;
                    IsMac = true;

                    Name = "MacOS X " + ReadProcessOutput("sw_vers", "-productVersion");
                    Name = Name.Trim();

                    string machine = ReadProcessOutput("uname", "-m");
                    if (machine.Contains("x86_64"))
                        Is64bit = true;
                    else
                        Is32bit = true;

                    Name += " " + (Is64bit ? "(64 bit)" : "(32 bit)");
                }
                else if (UnixName.Contains("Linux"))
                {
                    IsUnix = true;
                    IsLinux = true;

                    Name = ReadProcessOutput("lsb_release", "-d");
                    Name = Name.Substring(Name.IndexOf(":") + 1);
                    Name = Name.Trim();

                    string machine = ReadProcessOutput("uname", "-m");
                    if (machine.Contains("x86_64"))
                        Is64bit = true;
                    else
                        Is32bit = true;

                    Name += " " + (Is64bit ? "(64 bit)" : "(32 bit)");
                }
                else if (UnixName != "")
                {
                    IsUnix = true;
                }
                else
                {
                    IsUnknown = true;
                }
            }
        }

        private static string ReadProcessOutput(string name)
        {
            return ReadProcessOutput(name, null);
        }

        private static string ReadProcessOutput(string name, string args)
        {
            try
            {
                Process p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                if (args != null && args != "") p.StartInfo.Arguments = " " + args;
                p.StartInfo.FileName = name;
                p.Start();
                // Do not wait for the child process to exit before
                // reading to the end of its redirected stream.
                // p.WaitForExit();
                // Read the output stream first and then wait.
                string output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                if (output == null) output = "";
                output = output.Trim();
                Trace.WriteLine(output);
                return output;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.StackTrace);
                return "";
            }
        }
    }

    [AttributeUsage(AttributeTargets.Assembly)]
    public class ReleaseDateAttribute : Attribute 
    {
        public DateTime ReleaseDate;

        public ReleaseDateAttribute(string releaseDate)
        {
            ReleaseDate = DateTime.ParseExact(releaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
    }

    [AttributeUsage(AttributeTargets.Assembly)]
    public class BetaReleaseAttribute : Attribute
    { }

    [AttributeUsage(AttributeTargets.Assembly)]
    public class MinimumVersionRequiredAttribute : Attribute
    {
        public string MinimumVersionRequired;

        public MinimumVersionRequiredAttribute(string minimumVersionRequired)
        {
            MinimumVersionRequired = minimumVersionRequired;
        }

        internal bool IsReqiredVersion(string currentVersion)
        {
            string[] tokensCurr = currentVersion.Split('.');
            string[] tokensReq = MinimumVersionRequired.Split('.');

            if (int.Parse(tokensCurr[0]) < int.Parse(tokensReq[0])) return false;
            if (int.Parse(tokensCurr[1]) < int.Parse(tokensReq[1])) return false;
            if (int.Parse(tokensCurr[2]) < int.Parse(tokensReq[2])) return false;

            return true;
        }
    }

    [AttributeUsage(AttributeTargets.Assembly)]
    public class TangraCoreVersionRequiredAttribute : MinimumVersionRequiredAttribute
    {
        public TangraCoreVersionRequiredAttribute(string minimumVersionRequired) :
            base(minimumVersionRequired)
        { }
    }

    [AttributeUsage(AttributeTargets.Assembly)]
    public class TangraVideoVersionRequiredAttribute : MinimumVersionRequiredAttribute
    {
        public TangraVideoVersionRequiredAttribute(string minimumVersionRequired) :
            base(minimumVersionRequired)
        { }
    }

    [AttributeUsage(AttributeTargets.Assembly)]
    public class TangraVideoLinuxVersionRequiredAttribute : MinimumVersionRequiredAttribute
    {
        public TangraVideoLinuxVersionRequiredAttribute(string minimumVersionRequired) :
            base(minimumVersionRequired)
        { }
    }

    [AttributeUsage(AttributeTargets.Assembly)]
    public class TangraVideoOSXVersionRequiredAttribute : MinimumVersionRequiredAttribute
    {
        public TangraVideoOSXVersionRequiredAttribute(string minimumVersionRequired) :
            base(minimumVersionRequired)
        { }
    }
}
