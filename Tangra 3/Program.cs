using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Tangra.Helpers;
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
			Trace.WriteLine(string.Format("Starting Tangra v{0}", Assembly.GetExecutingAssembly().GetName().Version));

		    //MessageBox.Show("Attach Debugger Now!");

		    PlatformID platform = Environment.OSVersion.Platform;
            if (platform != PlatformID.MacOSX && platform != PlatformID.Unix && 
                platform != PlatformID.Win32Windows && platform != PlatformID.Win32NT && platform != PlatformID.Win32S)
            {
                // NOTE: Show a warning message that this may be unsupported or untested platform
            }

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
				Application.Run(new frmMain());
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
