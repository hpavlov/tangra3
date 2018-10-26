/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tangra.Automation;
using Tangra.Helpers;
using Tangra.Model.Helpers;
using Tangra.PInvoke;
using Tangra.Properties;

namespace Tangra
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
        static void Main(string[] args)
		{
            Trace.WriteLine(string.Format("Starting Tangra v{0}", ((AssemblyFileVersionAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true)[0]).Version));

			//Debugger.Launch();
			//DebugContext.DebugSubPixelMeasurements = true;

		    PlatformID platform = Environment.OSVersion.Platform;
            if (platform != PlatformID.MacOSX && platform != PlatformID.Unix && 
                platform != PlatformID.Win32Windows && platform != PlatformID.Win32NT && platform != PlatformID.Win32S)
            {
                // NOTE: Show a warning message that this may be unsupported or untested platform
            }

			#region Make sure the settings are not forgotten between application version updates
			System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
			Version appVersion = a.GetName().Version;
			string appVersionString = appVersion.ToString();

			try
			{
				if (Properties.Settings.Default.ApplicationVersion != appVersion.ToString())
				{
					Properties.Settings.Default.Upgrade();
					Properties.Settings.Default.ApplicationVersion = appVersionString;
					Properties.Settings.Default.Save();
				}
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex);
			}
			#endregion

            var manager = new AutomationManager(args);
            if (manager.IsAutomationCommand)
            {
                ConsoleHelper.InitConsoleHandles();

                try
                {
                    manager.Run();
                }
                finally
                {
                    ConsoleHelper.ReleaseConsoleHandles();
                }

                return;
            }

		    Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
			Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
			
			bool fatalError = false;
			try
			{
				CheckUnmanagedLibraries();
			}
			catch (Exception ex)
			{
				MessageBox.Show(
					string.Concat(ex.GetType(), ": ", ex.Message, "\r\n\r\n", ex.StackTrace),
					"Tangra 3 Startup Error", 
					MessageBoxButtons.OK, 
					MessageBoxIcon.Error);

				fatalError = true;
			}

			if (!fatalError)
			{
				if (!ExecuteNonUICommandLineActions())
					Application.Run(new frmMain());
				else
					Application.Exit();
			}
		}

		static bool ExecuteNonUICommandLineActions()
		{
		    string[] args = Environment.GetCommandLineArgs();
			string action = args.Length > 1 ? args[1].Trim('"') : null;

		    if (CurrentOS.IsWindows)
		    {
#if WIN32
                bool isRegisterAssociationsCommand = TangraFileAssociations.COMMAND_LINE_ASSOCIATE.Equals(action, StringComparison.InvariantCultureIgnoreCase);

                try
                {
                    var fileAssociation = new TangraFileAssociations();
                    if (!fileAssociation.Registered || isRegisterAssociationsCommand)
                    {
                        fileAssociation.Associate(false);
                    }
                }
                catch (Exception ex)
                {
                    if (isRegisterAssociationsCommand)
                        Trace.WriteLine(ex.GetFullStackTrace());
                }


                return isRegisterAssociationsCommand;
#else
		        return false;
#endif
            }
            else
                return false;
		}

		static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			frmUnhandledException.HandleExceptionNoRestart(null, e.Exception);
		}

        static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            frmUnhandledException.HandleExceptionNoRestart(null, e.Exception);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            frmUnhandledException.HandleExceptionNoRestart(null, e.ExceptionObject as Exception);
        }

		private static void CheckUnmanagedLibraries()
		{
            TangraCoreVersionRequiredAttribute minCoreVersionRequired = ((TangraCoreVersionRequiredAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(TangraCoreVersionRequiredAttribute), false)[0]);
            TangraVideoVersionRequiredAttribute minVideoVersionRequired = ((TangraVideoVersionRequiredAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(TangraVideoVersionRequiredAttribute), false)[0]);
            TangraVideoLinuxVersionRequiredAttribute minVideoLinuxVersionRequired = ((TangraVideoLinuxVersionRequiredAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(TangraVideoLinuxVersionRequiredAttribute), false)[0]);
            TangraVideoOSXVersionRequiredAttribute minVideoOSXVersionRequired = ((TangraVideoOSXVersionRequiredAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(TangraVideoOSXVersionRequiredAttribute), false)[0]);

			string engineVersion = TangraCore.GetTangraCoreVersion();
		    int engineBitness = TangraCore.GetTangraCoreBitness();
            if (engineBitness > 0)
                Trace.WriteLine(string.Format("Tangra Core v{0} ({1} bit)", engineVersion, engineBitness));
            else
                Trace.WriteLine(string.Format("Tangra Core v{0})", engineVersion));

            if (minCoreVersionRequired != null && !minCoreVersionRequired.IsReqiredVersion(engineVersion))
            {
                string fileName = CurrentOS.IsWindows ? "TangraCore.dll" : (CurrentOS.IsMac ? "libTangraCore.dylib" : "libTangraCore.so");

                MessageBox.Show(string.Format("Your installation of Tangra3 desn't have the latest version of {0}. Please check for updates.", fileName), "Tangra 3", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

			engineVersion = TangraVideo.GetVideoEngineVersion();
			Trace.WriteLine(string.Format("Tangra Video Engine v{0}", engineVersion));

            if (CurrentOS.IsWindows)
            {
                if (minVideoVersionRequired != null && !minVideoVersionRequired.IsReqiredVersion(engineVersion))
                    MessageBox.Show("Your installation of Tangra3 desn't have the latest version of TangraVideo.dll. Please check for updates.", "Tangra 3", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (CurrentOS.IsMac)
            {
                if (minVideoOSXVersionRequired != null && !minVideoOSXVersionRequired.IsReqiredVersion(engineVersion))
                    MessageBox.Show("Your installation of Tangra3 desn't have the latest version of libTangraVideo.dylib. Please check for updates.", "Tangra 3", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (minVideoLinuxVersionRequired != null && !minVideoLinuxVersionRequired.IsReqiredVersion(engineVersion))
                    MessageBox.Show("Your installation of Tangra3 desn't have the latest version of libTangraVideo.so. Please check for updates.", "Tangra 3", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
		}

		public static bool IsLinux
		{
			get
			{
				int p = (int)Environment.OSVersion.Platform; 
				return (p == 4) || (p == 6) || (p == 128);
			}
		}
	}
}
