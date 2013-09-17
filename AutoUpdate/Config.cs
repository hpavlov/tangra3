using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using System.Globalization;
using System.Security.Cryptography;
using AutoUpdateSelfUpdate;
using Microsoft.Win32;
using System.Security;
using System.Xml;

namespace AutoUpdate
{
    internal class Config
    {
        private Config()
        { }

        private readonly static Config m_Instance = new Config();

        public static Config Instance
        {
            get { return m_Instance; }
        }

        public string UpdateLocation;
        public string UpdatesXmlFileName;
        public string SelfUpdateFileNameToDelete;

        public bool IsFirtsTimeRun = false;

        public void Load(string tangra3Path, bool acceptBetaUpdates)
        {
			UpdateLocation = SharedUpdateConstants.UPDATE_URL_LOCATION;
            UpdatesXmlFileName = acceptBetaUpdates ? "/Beta.xml" : "/Updates.xml";

            RegistryKey key = null;

            try
            {
				key = Registry.CurrentUser.OpenSubKey(SharedUpdateConstants.REGISTRY_KEY, RegistryKeyPermissionCheck.ReadSubTree);
                if (key == null)
					key = Registry.CurrentUser.CreateSubKey(SharedUpdateConstants.REGISTRY_KEY, RegistryKeyPermissionCheck.ReadWriteSubTree);
            }
            catch (UnauthorizedAccessException) { }
            catch (SecurityException) { }

            if (key != null)
            {
                try
                {
					UpdateLocation = Convert.ToString(key.GetValue(SharedUpdateConstants.REG_ENTRY_DEFAULT_UPDATE_LOCATION, SharedUpdateConstants.UPDATE_URL_LOCATION), CultureInfo.InvariantCulture);
                    UpdateLocation = UpdateLocation.TrimEnd(new char[] { '/' });
					SelfUpdateFileNameToDelete = Convert.ToString(key.GetValue(SharedUpdateConstants.REG_ENTRY_SELFUPDATE_TEMP_FILE, null));
                }
                finally
                {
                    key.Close();
                }
            }

			IsFirtsTimeRun = !File.Exists(Tangra3ExePath(tangra3Path));

            try
            {
                if (File.Exists("C:\\UpdateConfig.xml"))
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load("C:\\UpdateConfig.xml");
                    XmlNode node = xmlDoc.SelectSingleNode("/configuration/updateLocation");
                    if (node != null)
                        UpdateLocation = node.InnerText;
                }
            }
            catch { }
        }

        public string Tangra3ExePath(string tangra3Location)
        {
            return Path.GetFullPath(tangra3Location + @"/" + SharedUpdateConstants.MAIN_EXECUTABLE_NAME);
        }

        public string PrepareTangra3SelfUpdateTempFile(string tangra3Location, bool acceptBetaUpdates, string newVersionLocation)
        {
            RegistryKey key = null;

            try
            {
				key = Registry.CurrentUser.OpenSubKey(SharedUpdateConstants.REGISTRY_KEY, RegistryKeyPermissionCheck.ReadWriteSubTree);
                if (key == null)
					key = Registry.CurrentUser.CreateSubKey(SharedUpdateConstants.REGISTRY_KEY, RegistryKeyPermissionCheck.ReadWriteSubTree);
            }
            catch (UnauthorizedAccessException) { }
            catch (SecurityException) { }

            string tempFileName = Path.ChangeExtension(Path.GetTempFileName(), ".exe");
            if (key != null)
            {
                try
                {
					key.SetValue(SharedUpdateConstants.REG_ENTRY_SELFUPDATE_TEMP_FILE, tempFileName);
					key.SetValue(SharedUpdateConstants.REG_ENTRY_COPY_FROM_FULL_FILE_NAME, newVersionLocation);
					key.SetValue(SharedUpdateConstants.REG_ENTRY_COPY_TO_DIRECTORY_NAME, AppDomain.CurrentDomain.BaseDirectory);
					key.SetValue(SharedUpdateConstants.REG_ENTRY_UPDATE_LOCATION, tangra3Location);
					key.SetValue(SharedUpdateConstants.REG_ENTRY_ACCEPT_BETA_VERSION, acceptBetaUpdates);
                }
                catch { }
                finally
                {
                    key.Close();
                }              
            }

            return tempFileName;
        }

        public void ResetSelfUpdateFileName()
        {
            RegistryKey key = null;

            try
            {
				key = Registry.CurrentUser.OpenSubKey(SharedUpdateConstants.REGISTRY_KEY, RegistryKeyPermissionCheck.ReadWriteSubTree);
                if (key == null)
					key = Registry.CurrentUser.CreateSubKey(SharedUpdateConstants.REGISTRY_KEY, RegistryKeyPermissionCheck.ReadWriteSubTree);
            }
            catch (UnauthorizedAccessException) { }
            catch (SecurityException) { }

            if (key != null)
            {
                try
                {
					key.DeleteValue(SharedUpdateConstants.REG_ENTRY_SELFUPDATE_TEMP_FILE, false);
					key.DeleteValue(SharedUpdateConstants.REG_ENTRY_COPY_FROM_FULL_FILE_NAME, false);
					key.DeleteValue(SharedUpdateConstants.REG_ENTRY_COPY_TO_DIRECTORY_NAME, false);
					key.DeleteValue(SharedUpdateConstants.REG_ENTRY_UPDATE_LOCATION, false);
					key.DeleteValue(SharedUpdateConstants.REG_ENTRY_ACCEPT_BETA_VERSION, false);
                }
                catch (Exception)
                { }
                finally
                {
                    key.Close();
                }
            }
        }

        public int CurrentlyInstalledFileVersion(string tangra3Location, string relativeFilePath)
        {
			if (Directory.Exists(tangra3Location))
            {
				string owExeFilePath = Path.GetFullPath(tangra3Location + "/" + relativeFilePath);
                if (File.Exists(owExeFilePath))
                {
                    try
                    {
                        AssemblyName an = AssemblyName.GetAssemblyName(owExeFilePath);
                        Version owVer = an.Version;
                        return 10000 * owVer.Major + 1000 * owVer.Minor + 100 * owVer.Build + owVer.Revision;
                    }
                    catch
                    { }
                }
                else
                    return -1;
            }
            else
            {
				Directory.CreateDirectory(tangra3Location);
            }

            return 0;
        }

		public int CurrentlyInstalledTangra3Version(string tangra3Location)
        {
			if (Directory.Exists(tangra3Location))
            {
				string tangraExeFilePath = Path.GetFullPath(tangra3Location + "/" + SharedUpdateConstants.MAIN_EXECUTABLE_NAME);
                if (File.Exists(tangraExeFilePath))
                {
                    try
                    {
                        AssemblyName an = AssemblyName.GetAssemblyName(tangraExeFilePath);
                        Version owVer = an.Version;
                        return 10000 * owVer.Major + 1000 * owVer.Minor + 100 * owVer.Build + owVer.Revision;
                    }
                    catch
                    { }
                }
            }
            else
            {
				Directory.CreateDirectory(tangra3Location);
            }

            return 0;
        }

        public string VersionToVersionString(int version)
        {
            var outVer = new StringBuilder();
            outVer.Append(Convert.ToString(version / 10000));
            outVer.Append(".");

            version = version % 10000;
            outVer.Append(Convert.ToString(version / 1000));
            outVer.Append(".");

            version = version % 1000;
            outVer.Append(Convert.ToString(version / 100));
            outVer.Append(".");
            outVer.Append(Convert.ToString(version % 100));

            return outVer.ToString();
        }

        public int VersionStringToVersion(string versionString)
        {
            string[] tokens = versionString.Split('.');
            int version =
                10000 * int.Parse(tokens[0]) +
                1000 * int.Parse(tokens[1]) +
                (tokens.Length > 2 ? 100 * int.Parse(tokens[2]) : 0) + 
                (tokens.Length > 3 ? int.Parse(tokens[3]) : 0);
            return version;
        }

        public int Tangra3UpdateVersionStringToVersion(string versionString)
        {
            string[] tokens = versionString.Split('.');
            int version = 
                10000 * int.Parse(tokens[0]) + 
                1000 * int.Parse(tokens[1]) +
                (tokens.Length > 2 ? 100 * int.Parse(tokens[2]) : 0) + 
                (tokens.Length > 2 ? int.Parse(tokens[3]) : 0);

            return version;
        }
    }
}
