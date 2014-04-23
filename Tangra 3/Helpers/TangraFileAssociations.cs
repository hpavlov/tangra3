using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BrendanGrant.Helpers.FileAssociation;
using Microsoft.Win32;
using Tangra.Model.Helpers;

namespace Tangra.Helpers
{
	public class TangraFileAssociations
	{
		public static string PGID_LC_FILE = "Tangra Light Curve";
		public static string PGID_AAV_FILE = "Astro Analogue Video";
		public static string PGID_ADV_FILE = "Astro Digital Video";

		public static string PGID_LC_FILE_EXT = ".lc";
		public static string PGID_AAV_FILE_EXT = ".aav";
		public static string PGID_ADV_FILE_EXT = ".adv";

		public static string COMMAND_LINE_ASSOCIATE = "tangra-file-assoc";

		public bool Registered { get; private set; }
		public bool CheckCompleted { get; private set; }
		public bool CanRegisterWithoutElevation { get; private set; }

		private AssociationManager m_AssociationManager;

		public TangraFileAssociations()
		{
			Registered = false;
			CheckCompleted = false;
			CanRegisterWithoutElevation = false;

			m_AssociationManager = new AssociationManager();
			try
			{
				bool lcRegistered = m_AssociationManager.CheckAssociation(PGID_LC_FILE, PGID_LC_FILE_EXT).Length == 0;
				bool aavRegistered = m_AssociationManager.CheckAssociation(PGID_AAV_FILE, PGID_AAV_FILE_EXT).Length == 0;
				bool advRegistered = m_AssociationManager.CheckAssociation(PGID_ADV_FILE, PGID_ADV_FILE_EXT).Length == 0;

				Registered = lcRegistered && aavRegistered && advRegistered;

				CheckCompleted = true;
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.GetFullStackTrace());
			}

			try
			{
				Registry.ClassesRoot.GetAccessControl();
				CanRegisterWithoutElevation = true;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		public void Associate()
		{
			// NOTE: Icons have been embedded with this tool: http://einaregilsson.com/add-multiple-icons-to-a-dotnet-application/

			m_AssociationManager.Associate(
				PGID_LC_FILE, string.Format("\"{0}\" \"%L\"", 
				Process.GetCurrentProcess().Modules[0].FileName), 
				PGID_LC_FILE_EXT,
				new ProgramIcon(string.Format("\"{0}\"", Process.GetCurrentProcess().Modules[0].FileName), 1));

			m_AssociationManager.Associate(
				PGID_AAV_FILE, string.Format("\"{0}\" \"%L\"",
				Process.GetCurrentProcess().Modules[0].FileName),
				PGID_AAV_FILE_EXT,
				new ProgramIcon(string.Format("\"{0}\"", Process.GetCurrentProcess().Modules[0].FileName), 2));

			m_AssociationManager.Associate(
				PGID_ADV_FILE, string.Format("\"{0}\" \"%L\"",
				Process.GetCurrentProcess().Modules[0].FileName),
				PGID_ADV_FILE_EXT,
				new ProgramIcon(string.Format("\"{0}\"", Process.GetCurrentProcess().Modules[0].FileName), 3));
		}
	}
}
