using System;
using System.Collections.Generic;
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

        public SoftwareUpdate(XmlElement node)
            : base(node)
        {
            m_Version = int.Parse(node.Attributes["Version"].Value, CultureInfo.InvariantCulture); ;
            Path = node.Attributes["Path"].Value;
            if (node.Attributes["ArchivedPath"] != null)
                ArchivedPath = node.Attributes["ArchivedPath"].Value;
        }

        public override bool NewUpdatesAvailable(string occuRecPath)
        {
            Assembly asm = GetLocalOccuRecUpdateAssembly();
            if (asm != null)
            {
                object[] atts = asm.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true);
                if (atts.Length == 1)
                {
                    string currVersionString = ((AssemblyFileVersionAttribute)atts[0]).Version;
                    int currVersionAsInt = Config.Instance.OccuRecUpdateVersionStringToVersion(currVersionString);

                    if (base.Version > currVersionAsInt)
                    {
						Trace.WriteLine(string.Format("Update required for '{0}': local version: {1}; server version: {2}", SharedUpdateConstants.MAIN_UPDATER_EXECUTABLE_NAME, currVersionAsInt, Version));
                        return true;
                    }
                }
            }

            return false;
        }
        
        private Assembly GetLocalOccuRecUpdateAssembly()
        {
			if (Assembly.GetExecutingAssembly().GetName().Name == SharedUpdateConstants.UPDATER_PROGRAM_NAME)
                return Assembly.GetExecutingAssembly();

			string probeFile = System.IO.Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + "\\" + SharedUpdateConstants.MAIN_UPDATER_EXECUTABLE_NAME);
            if (System.IO.File.Exists(probeFile))
                return Assembly.ReflectionOnlyLoadFrom(probeFile);            

            return null;
        }

        public override void Update(Updater updater, string occuRecPath, bool acceptBetaUpdates, IProgressUpdate progress)
        {
			string newVerFileLocalFileName = System.IO.Path.GetFullPath(System.IO.Path.GetTempPath() + "\\" + SharedUpdateConstants.MAIN_UPDATER_EXECUTABLE_NAME);

            if (System.IO.File.Exists(newVerFileLocalFileName))
                System.IO.File.Delete(newVerFileLocalFileName);

            // Download the new OccuRecUpdate version under newVerFileLocalFileName
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

            string selfUpdaterExecutable = Config.Instance.PrepareOccuRecSelfUpdateTempFile(occuRecPath, acceptBetaUpdates, newVerFileLocalFileName);

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
