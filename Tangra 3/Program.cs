using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
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
		static void Main()
		{
            Trace.WriteLine(string.Format("Starting Tangra v{0}", ((AssemblyFileVersionAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true)[0]).Version));

			//Debugger.Launch();

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

		    Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			
			Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
			
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
			string action = Environment.GetCommandLineArgs().Length > 1 ? Environment.GetCommandLineArgs()[1].Trim('"') : null;

			bool isRegisterAssociationsCommand = TangraFileAssociations.COMMAND_LINE_ASSOCIATE.Equals(action, StringComparison.InvariantCultureIgnoreCase);

			try
			{
				var fileAssociation = new TangraFileAssociations();
				if (!fileAssociation.Registered || isRegisterAssociationsCommand)
				{
					fileAssociation.Associate();
				}
			}
			catch (Exception ex)
			{
				if (isRegisterAssociationsCommand)
					Trace.WriteLine(ex.GetFullStackTrace());
			}


			return isRegisterAssociationsCommand;
		}

		static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			frmUnhandledException.HandleExceptionNoRestart(null, e.Exception);			
		}

		private static void CheckUnmanagedLibraries()
		{
			string engineVersion = TangraCore.GetTangraCoreVersion();
			Trace.WriteLine(string.Format("Tangra Core v{0}", engineVersion));

			engineVersion = TangraVideo.GetVideoEngineVersion();
			Trace.WriteLine(string.Format("Tangra Video Engine v{0}", engineVersion));
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
