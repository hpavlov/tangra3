/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Text;
using System.Windows.Forms;
using BrendanGrant.Helpers.FileAssociation;
using Microsoft.Win32;
using Tangra.Model.Helpers;

namespace Tangra.Helpers
{
#if WIN32
	public class TangraFileAssociations
	{
		public static string PGID_LC_FILE = "Tangra Light Curve";
		public static string PGID_AAV_FILE = "Astro Analogue Video";
		public static string PGID_ADV_FILE = "Astro Digital Video";
		public static string PGID_SPECTRA_FILE = "Tangra Spectra";

		public static string PGID_LC_FILE_EXT = ".lc";
		public static string PGID_AAV_FILE_EXT = ".aav";
		public static string PGID_ADV_FILE_EXT = ".adv";
		public static string PGID_SPECTRA_FILE_EXT = ".spectra";

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
				bool spectraRegistered = m_AssociationManager.CheckAssociation(PGID_SPECTRA_FILE, PGID_SPECTRA_FILE_EXT).Length == 0;

				Registered = lcRegistered && aavRegistered && advRegistered && spectraRegistered;

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

		public void Associate(bool showError)
		{
			// NOTE: Icons have been embedded with this tool: http://einaregilsson.com/add-multiple-icons-to-a-dotnet-application/

            try
            {
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

				m_AssociationManager.Associate(
					PGID_SPECTRA_FILE, string.Format("\"{0}\" \"%L\"",
					Process.GetCurrentProcess().Modules[0].FileName),
					PGID_SPECTRA_FILE_EXT,
					new ProgramIcon(string.Format("\"{0}\"", Process.GetCurrentProcess().Modules[0].FileName), 4));
            }
            catch (SecurityException sex)
            {
                if (showError)
                {
                    MessageBox.Show("You need to run Tangra as Administrator to do this.", "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                    Trace.WriteLine(sex.ToString());
            }
		}
	}
#endif
}
