/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Globalization;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Net;
using AutoUpdateSelfUpdate;

namespace AutoUpdate.Schema
{
    class SoftwareUpdate : UpdateObject 
    {
		// <SoftwareUpdate Version="16000" Path="SoftwareUpdate.exe" ArchivedPath="SoftwareUpdate.zip"/>

        internal readonly string Path = null;
        internal readonly string ArchivedPath = null;

	    private bool? m_NewUpdateRequired = null;


        public SoftwareUpdate(XmlElement node)
            : base(node)
        {
            m_Version = int.Parse(node.Attributes["Version"].Value, CultureInfo.InvariantCulture); ;
            Path = node.Attributes["Path"].Value;
            if (node.Attributes["ArchivedPath"] != null)
                ArchivedPath = node.Attributes["ArchivedPath"].Value;
        }

        public override bool NewUpdatesAvailable(string tangra3Path)
        {
	        if (m_NewUpdateRequired.HasValue)
		        return m_NewUpdateRequired.Value;

            Assembly asm = GetLocalTangra3UpdateAssembly();
            if (asm != null)
            {
	            CustomAttributeData[] atts = asm.GetCustomAttributesData().Where(x=>x.Constructor.DeclaringType == typeof(AssemblyFileVersionAttribute)).ToArray();
                if (atts.Length == 1)
                {
                    string currVersionString = (atts[0]).ConstructorArguments[0].Value.ToString();
                    int currVersionAsInt = Config.Instance.Tangra3UpdateVersionStringToVersion(currVersionString);

	                m_NewUpdateRequired = base.Version > currVersionAsInt;
					if (m_NewUpdateRequired.Value)
                    {
						Trace.WriteLine(string.Format("Update required for '{0}': local version: {1}; server version: {2}", SharedUpdateConstants.MAIN_UPDATER_EXECUTABLE_NAME, currVersionAsInt, Version));
                        return true;
                    }
                }
            }

            return false;
        }
        
        private Assembly GetLocalTangra3UpdateAssembly()
        {
			if (Assembly.GetExecutingAssembly().GetName().Name == SharedUpdateConstants.UPDATER_PROGRAM_NAME)
                return Assembly.GetExecutingAssembly();

			string probeFile = System.IO.Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + "\\" + SharedUpdateConstants.MAIN_UPDATER_EXECUTABLE_NAME);
            if (System.IO.File.Exists(probeFile))
                return Assembly.ReflectionOnlyLoadFrom(probeFile);            

            return null;
        }

        public override void Update(Updater updater, string tangra3Path, bool acceptBetaUpdates, IProgressUpdate progress)
        {
			string newVerFileLocalFileName = System.IO.Path.GetFullPath(System.IO.Path.GetTempPath() + "\\" + SharedUpdateConstants.MAIN_UPDATER_EXECUTABLE_NAME);

            if (System.IO.File.Exists(newVerFileLocalFileName))
                System.IO.File.Delete(newVerFileLocalFileName);

            // Download the new AutoUpdate version under newVerFileLocalFileName
            string fileLocation = null;
            if (string.IsNullOrEmpty(ArchivedPath))
                fileLocation = Path;
            else
                fileLocation = ArchivedPath;

            try
            {
                progress.RefreshMainForm();
                updater.UpdateFile(fileLocation, newVerFileLocalFileName, !string.IsNullOrEmpty(ArchivedPath), progress);
            }
            catch (WebException wex)
            {
                progress.OnError(wex);

                return;
            }

            string selfUpdaterExecutable = Config.Instance.PrepareTangra3SelfUpdateTempFile(tangra3Path, acceptBetaUpdates, newVerFileLocalFileName);

			using (Stream selfUpdater = Shared.AssemblyHelper.GetEmbededResourceStreamThatClientMustDispose("AutoUpdate.SelfUpdate", "AutoUpdateSelfUpdate.bin"))
            {
                byte[] buffer = new byte[selfUpdater.Length];
                selfUpdater.Read(buffer, 0, buffer.Length);
                System.IO.File.WriteAllBytes(selfUpdaterExecutable, buffer);
            }

            // Run the copyFileName passing as argument current process ID and then terminate the current process fast!
            var currProcess = Process.GetCurrentProcess();
            var pi = new ProcessStartInfo(selfUpdaterExecutable, currProcess.Id.ToString());
            
            if (System.Environment.OSVersion.Version.Major > 5)
                // UAC Elevate as Administrator for Windows Vista, Win7 and later
                pi.Verb = "runas";

            pi.WindowStyle = ProcessWindowStyle.Hidden;
            Process.Start(pi);

            currProcess.Kill();
        }
    }
}
