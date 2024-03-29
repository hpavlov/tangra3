﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using Microsoft.Win32;
using Tangra.Helpers;
using Tangra.Model.Config;
using Tangra.Properties;
using Tangra.Model.Helpers;

namespace Tangra.Controller
{
	public class AutoUpdatesController
	{
		private frmMain m_MainFrom;
		private VideoController m_VideoController;

		public AutoUpdatesController(frmMain mainFrom, VideoController videoController)
		{
			m_MainFrom = mainFrom;
			m_VideoController = videoController;
		}

		internal delegate void OnUpdateEventDelegate(int eventCode);

		internal void OnUpdateEvent(int eventCode)
		{
			if (eventCode == MSG_ID_NEW_TANGRA3_UPDATE_AVAILABLE)
			{
				m_MainFrom.pnlNewVersionAvailable.Visible = true;
				m_ShowNegativeResultMessage = false;
                PlatformID platform = Environment.OSVersion.Platform;

                if (platform == PlatformID.Win32Windows || platform == PlatformID.Win32NT || platform == PlatformID.Win32S)
				    m_MainFrom.pnlNewVersionAvailable.Text = "New version of Tangra is available. Click here to upgrade.";
                else
                    // Room available on Lunix/OSX for for label is too small
                    m_MainFrom.pnlNewVersionAvailable.Text = "New version of Tangra is available.";

				m_MainFrom.pnlNewVersionAvailable.LinkColor = Color.Lime;
			}
			else if (eventCode == MSG_ID_NEW_TANGRA3_ADDIN_UPDATE_AVAILABLE)
			{
				m_MainFrom.pnlNewVersionAvailable.Visible = true;
                PlatformID platform = Environment.OSVersion.Platform;

                if (platform == PlatformID.Win32Windows || platform == PlatformID.Win32NT || platform == PlatformID.Win32S)
                    m_MainFrom.pnlNewVersionAvailable.Text = "New add-in version is available. Click here to upgrade.";
                else
                    // Room available on Lunix/OSX for for label is too small
                    m_MainFrom.pnlNewVersionAvailable.Text = "New add-in version is available.";

				m_MainFrom.pnlNewVersionAvailable.LinkColor = Color.Cyan;
				m_ShowNegativeResultMessage = false;
			}
			else if (eventCode == MSG_ID_NO_TANGRA3_UPDATES_AVAILABLE)
			{
				if (m_ShowNegativeResultMessage)
				{
					MessageBox.Show(
						string.Format("There are no new {0}updates.", TangraConfig.Settings.Generic.AcceptBetaUpdates ? "beta " : ""),
						"Information",
						MessageBoxButtons.OK,
						MessageBoxIcon.Information);
				}

				m_ShowNegativeResultMessage = false;
			}
		}

		private bool m_ShowNegativeResultMessage = false;
		private DateTime m_LastUpdateTime;
		private Thread m_CheckForUpdatesThread;

		private byte MSG_ID_NEW_TANGRA3_UPDATE_AVAILABLE = 13;
		private byte MSG_ID_NO_TANGRA3_UPDATES_AVAILABLE = 14;
		private byte MSG_ID_NEW_TANGRA3_ADDIN_UPDATE_AVAILABLE = 15;

        private Version GetDotNetVersionFromRegistry()
        {
            try
            {
                using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\"))
                {
                    int releaseKey = Convert.ToInt32(ndpKey.GetValue("Release"));
                    if (releaseKey >= 393273)
                    {
                        return new Version(4, 6);
                    }
                    if ((releaseKey >= 379893))
                    {
                        return new Version(4, 5, 2);
                    }
                    if ((releaseKey >= 378675))
                    {
                        return new Version(4, 5, 1);
                    }
                    if ((releaseKey >= 378389))
                    {
                        return new Version(4, 5);
                    }
                }
            }
            catch { }

            return new Version(4, 0);
        }


		public void CheckForUpdates(bool manualCheck)
		{
		    if (CurrentOS.IsWinTangraEndOfLife)
		    {
		        if (!TangraConfig.Settings.Generic.TangraEndOfLifeWarningShown)
		        {
		            MessageBox.Show(
                        "Tangra has reached an 'End of Update' status for versions of Windows older than Windows Vista.\r\n\r\nNo more updates will be offered or delivered. Please upgrade your Windows to receive further updates.", 
                        "Tangra - End of Updates", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);

		            TangraConfig.Settings.Generic.TangraEndOfLifeWarningShown = true;
		            TangraConfig.Settings.Save();
		        }

		        var tangraUpdateLocation = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\Tangra3Update.exe");
		        if (File.Exists(tangraUpdateLocation))
		        {
		            try
		            {
		                File.Delete(tangraUpdateLocation);
		            }
                    catch
		            { }
		        }
		        return;
		    }

		    if (GetDotNetVersionFromRegistry() < new Version(4, 5))
		    {
		        if (manualCheck || new Random((int)DateTime.Now.Ticks).Next(100) <= 5)
		        {
                    // Show warning message for all manual checks for update or in 5% of the automatic checks
                    MessageBox.Show(
                        "The next update of Tangra will require .NET version 4.5 or later which is currently not installed on this system.\r\n\r\nPlease install the latest version of the .NET Framework to receive further updates of Tangra.",
                        "Tangra Update",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
		        }

		        return;
		    }

			m_ShowNegativeResultMessage = manualCheck;

			if (
					m_CheckForUpdatesThread == null ||
					!m_CheckForUpdatesThread.IsAlive
				)
			{
				IntPtr handleHack = m_MainFrom.Handle;

				m_CheckForUpdatesThread = new Thread(new ParameterizedThreadStart(CheckForUpdates));
				m_CheckForUpdatesThread.Start(handleHack);
			}
		}

		private void CheckForUpdates(object state)
		{
		    if (CurrentOS.IsWinTangraEndOfLife)
		        return;

			try
			{
				m_LastUpdateTime = DateTime.Now;

				int serverConfigVersion;
				NewUpdateState updateState = NewUpdatesAvailable(out serverConfigVersion);
				if (updateState == NewUpdateState.TangraUpdate)
				{
					Trace.WriteLine("There is a new update.", "Update");
					m_MainFrom.Invoke(new OnUpdateEventDelegate(OnUpdateEvent), MSG_ID_NEW_TANGRA3_UPDATE_AVAILABLE);
				}
				if (updateState == NewUpdateState.AddinModuleUpdate)
				{
					Trace.WriteLine("There is a new add-in update.", "Update");
					m_MainFrom.Invoke(new OnUpdateEventDelegate(OnUpdateEvent), MSG_ID_NEW_TANGRA3_ADDIN_UPDATE_AVAILABLE);
				}
				else
				{
					Trace.WriteLine(
						string.Format("There are no new {0}updates.", TangraConfig.Settings.Generic.AcceptBetaUpdates ? "beta " : ""),
						"Update");
					m_MainFrom.Invoke(new OnUpdateEventDelegate(OnUpdateEvent), MSG_ID_NO_TANGRA3_UPDATES_AVAILABLE);
				}

				Settings.Default.LastCheckedForUpdates = m_LastUpdateTime;
				Settings.Default.Save();
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex, "Update");
			}

			try
			{
				if (TangraConfig.Settings.Generic.SubmitUsageStats && Settings.Default.UsageStatsLastSent.AddDays(30) < DateTime.Now)
				{
					UsageStats.Instance.Save();
					string usageStats = Settings.Default.UsageStatistics;

					WebRequest req = WebRequest.Create("http://www.occultwatcher.net/CGI-BIN/TangraUsageStats.ashx");
					
					req.ContentType = "application/xml";
					req.Method = "POST";
					
					byte[] bytes = Encoding.UTF8.GetBytes(usageStats);

					req.ContentLength = bytes.Length;
					System.IO.Stream os = req.GetRequestStream();
					os.Write(bytes, 0, bytes.Length);
					os.Close();

					HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
					if (resp.StatusCode == HttpStatusCode.OK)
					{
						UsageStats.ClearStats();
						Settings.Default.UsageStatsLastSent = DateTime.Now;
						Settings.Default.Save();
					}
				}
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex, "UsageStats");
			}
		}

		public void RunTangra3UpdaterForWindows()
		{
			try
			{
				string currentPath = AppDomain.CurrentDomain.BaseDirectory;
				string updaterFileName = Path.GetFullPath(currentPath + "\\Tangra3Update.zip");

				int currUpdVer = CurrentlyInstalledTangra3UpdateVersion();
				int servUpdVer = int.Parse(tangra3UpdateServerVersion);
				if (!File.Exists(Path.GetFullPath(currentPath + "\\Tangra3Update.exe")) || /* If there is no Tangra3Update.exe*/
					servUpdVer > currUpdVer/* Or it is an older version ... */
					)
				{
					if (servUpdVer > currUpdVer)
						Trace.WriteLine(string.Format("Update required for 'Tangra3Update.exe': local version: {0}; server version: {1}", currUpdVer, servUpdVer));

					Uri updateUri = new Uri(UpdateLocation + "/Tangra3Update.zip");

					HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(updateUri);
					httpRequest.Method = "GET";
					httpRequest.Timeout = 1200000; //1200 sec = 20 min

					HttpWebResponse response = null;

					try
					{
						response = (HttpWebResponse)httpRequest.GetResponse();

						Stream streamResponse = response.GetResponseStream();

						try
						{
							try
							{
								if (File.Exists(updaterFileName))
									File.Delete(updaterFileName);

								using (BinaryReader reader = new BinaryReader(streamResponse))
								using (BinaryWriter writer = new BinaryWriter(new FileStream(updaterFileName, FileMode.Create)))
								{
									byte[] chunk = null;
									do
									{
										chunk = reader.ReadBytes(1024);
										writer.Write(chunk);
									}
									while (chunk != null && chunk.Length == 1024);

									writer.Flush();
								}
							}
							catch (UnauthorizedAccessException uex)
							{
								m_VideoController.ShowMessageBox(
									uex.Message + "\r\n\r\nYou may need to run Tangra3 as administrator to complete the update.", 
									"Error", 
									MessageBoxButtons.OK, 
									MessageBoxIcon.Exclamation);
							}
							catch (Exception ex)
							{
								Trace.WriteLine(ex);
							}

							if (File.Exists(updaterFileName))
							{
								string tempOutputDir = Path.ChangeExtension(Path.GetTempFileName(), "");
								Directory.CreateDirectory(tempOutputDir);
								try
								{
									string zippedFileName = updaterFileName;
									ZipUnzip.UnZip(updaterFileName, tempOutputDir, true);
									string[] files = Directory.GetFiles(tempOutputDir);
									updaterFileName = Path.ChangeExtension(updaterFileName, ".exe");
									System.IO.File.Copy(files[0], updaterFileName, true);
									System.IO.File.Delete(zippedFileName);
								}
								finally
								{
									Directory.Delete(tempOutputDir, true);
								}
							}
						}
						finally
						{
							streamResponse.Close();
						}
					}
					catch (WebException)
					{
						MessageBox.Show("There was an error trying to download the Tangra3Update program. Please ensure that you have an active internet connection and try again later.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
					finally
					{
						// Close response
						if (response != null)
							response.Close();
					}
				}

				// Make sure after the update is completed a new check is done. 
				// This will check for Add-in updates 
				Settings.Default.LastCheckedForUpdates = DateTime.Now.AddDays(-15);
				Settings.Default.Save();

				updaterFileName = Path.GetFullPath(currentPath + "\\Tangra3Update.exe");

				if (File.Exists(updaterFileName))
				{
					var processInfo = new ProcessStartInfo();

					if (System.Environment.OSVersion.Version.Major > 5)
						// UAC Elevate as Administrator for Windows Vista, Win7 and later
						processInfo.Verb = "runas";

					processInfo.FileName = updaterFileName;
					processInfo.Arguments = string.Format("{0}", TangraConfig.Settings.Generic.AcceptBetaUpdates ? "beta" : "full");
					Process.Start(processInfo);
				}
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex, "Update");
				m_MainFrom.pnlNewVersionAvailable.Enabled = true;
			}
		}

		private string tangra3UpdateServerVersion;

		public enum NewUpdateState
		{
			NoNewUpdate,
			TangraUpdate,
			AddinModuleUpdate
		}

		public NewUpdateState NewUpdatesAvailable(out int configUpdateVersion)
		{
			NewUpdateState rv = NewUpdateState.NoNewUpdate;
			configUpdateVersion = -1;
			Uri updateUri = new Uri(UpdatesXmlFileLocation);

			HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(updateUri);
			httpRequest.Method = "GET";
			httpRequest.Timeout = 30000; //30 sec

			HttpWebResponse response = null;

			try
			{
				response = (HttpWebResponse)httpRequest.GetResponse();

				string updateXml = null;

				Stream streamResponse = response.GetResponseStream();

				try
				{
					using (TextReader reader = new StreamReader(streamResponse))
					{
						updateXml = reader.ReadToEnd();
					}
				}
				finally
				{
					streamResponse.Close();
				}

				if (updateXml != null)
				{
					XmlDocument xmlDoc = new XmlDocument();
					xmlDoc.LoadXml(updateXml);

					int latestVersion = CurrentlyInstalledTangra3Version();
					XmlNode latestVersionNode = null;

					foreach (XmlNode updateNode in xmlDoc.SelectNodes("/Tangra3/Update"))
					{
						int Version = int.Parse(updateNode.Attributes["Version"].Value);
						if (latestVersion < Version)
						{
						    if (CurrentOS.IsWindows)
						    {
						        Trace.WriteLine("Update location: " + updateUri.ToString());
						        Trace.WriteLine("Current version: " + latestVersion.ToString());
						        Trace.WriteLine("New version: " + Version.ToString());
						    }
						    else
						    {
                                Console.WriteLine("Update location: " + updateUri.ToString());
                                Console.WriteLine("Current version: " + latestVersion.ToString());
                                Console.WriteLine("New version: " + Version.ToString());
                            }

#if WIN32
							XmlNode tangra3UpdateNode = xmlDoc.SelectSingleNode("/Tangra3/AutoUpdate");
							if (tangra3UpdateNode != null)
							{
								tangra3UpdateServerVersion = tangra3UpdateNode.Attributes["Version"].Value;
                                Trace.WriteLine("Tangra3Update new version: " + tangra3UpdateServerVersion);
							}
							else
								tangra3UpdateServerVersion = null;

							latestVersion = Version;
							latestVersionNode = updateNode;

							rv = NewUpdateState.TangraUpdate;
#endif
                        }
					}

#if WIN32
					foreach (XmlNode updateNode in xmlDoc.SelectNodes("/Tangra3/ModuleUpdate"))
					{
						if (updateNode.Attributes["Version"] == null) continue;
                        bool mustExist = updateNode.Attributes["MustExist"] != null && updateNode.Attributes["MustExist"].Value == "true";
						bool isAddin = updateNode.Attributes["IsAddin"] != null && updateNode.Attributes["IsAddin"].Value == "true";

						int Version = int.Parse(updateNode.Attributes["Version"].Value);
						latestVersion = CurrentlyInstalledModuleVersion(updateNode.Attributes["File"].Value);

						if (mustExist && latestVersion == 0) continue;

						if (latestVersion < Version)
						{
                            Trace.WriteLine("Update location: " + updateUri.ToString());
                            Trace.WriteLine("Module: " + updateNode.Attributes["File"].Value);
                            Trace.WriteLine("Current version: " + latestVersion.ToString());
                            Trace.WriteLine("New version: " + Version.ToString());

							XmlNode tangra3UpdateNode = xmlDoc.SelectSingleNode("/Tangra3/AutoUpdate");
							if (tangra3UpdateNode != null)
							{
								tangra3UpdateServerVersion = tangra3UpdateNode.Attributes["Version"].Value;
                                Trace.WriteLine("Tangra3Update new version: " + tangra3UpdateServerVersion);
							}
							else
								tangra3UpdateServerVersion = null;

							latestVersion = Version;
							latestVersionNode = updateNode;
							rv = isAddin ? NewUpdateState.AddinModuleUpdate : NewUpdateState.TangraUpdate;
							break;
						}
					}

					XmlNode cfgUpdateNode = xmlDoc.SelectSingleNode("/Tangra3/ConfigurationUpdate");
					if (cfgUpdateNode != null)
					{
						XmlNode cfgUpdVer = cfgUpdateNode.Attributes["Version"];
						if (cfgUpdVer != null)
						{
							configUpdateVersion = int.Parse(cfgUpdVer.InnerText);
						}
					}
#endif
				}
			}
			finally
			{
				// Close response
				if (response != null)
					response.Close();
			}

			return rv;
		}

		public int CurrentlyInstalledTangra3Version()
		{
			try
			{
			    Assembly asm = Assembly.GetExecutingAssembly();
			    object[] attr = asm.GetCustomAttributes(typeof (AssemblyFileVersionAttribute), true);
                if (attr.Length == 1)
                {
                    Regex fvregex = new Regex("^(\\d+)\\.(\\d+)\\.(\\d+)$");
                    Match match = fvregex.Match(((AssemblyFileVersionAttribute) attr[0]).Version);
                    return 10000 * int.Parse(match.Groups[1].Value) + 1000 * int.Parse(match.Groups[2].Value) + int.Parse(match.Groups[3].Value);
                }
                else
                {
                    Version owVer = asm.GetName().Version;
                    return 10000 * owVer.Major + 1000 * owVer.Minor + 100 * owVer.Build + owVer.Revision;
                }
			}
			catch (Exception ex)
			{
			    Console.WriteLine(ex.GetFullStackTrace());
			}

			return 0;
		}

		public static int CurrentlyInstalledTangra3UpdateVersion()
		{
			try
			{
				string woupdatePath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + "\\Tangra3Update.exe");
				if (File.Exists(woupdatePath))
				{
					AssemblyName an = AssemblyName.GetAssemblyName(woupdatePath);
					Version owVer = an.Version;
					return 10000 * owVer.Major + 1000 * owVer.Minor + 100 * owVer.Build + owVer.Revision;
				}
				else
					return 0;
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.ToString());
			}

			return 0;
		}

		private int VersionStringToVersion(string versionString)
		{
			string[] tokens = versionString.Split('.');
			int version = 10000 * int.Parse(tokens[0]) + 1000 * int.Parse(tokens[1]) + 100 * int.Parse(tokens[2]) + int.Parse(tokens[3]);
			return version;
		}

		private int CurrentlyInstalledModuleVersion(string moduleFileName)
		{
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += CurrentDomain_ReflectionOnlyAssemblyResolve;
			try
			{
			    string modulePath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + "\\" + moduleFileName);
			    if (File.Exists(modulePath))
			    {
			        Assembly asm = Assembly.ReflectionOnlyLoadFrom(modulePath);

			        IList<CustomAttributeData> atts = CustomAttributeData.GetCustomAttributes(asm);
			        foreach (CustomAttributeData cad in atts)
			        {
			            if (cad.Constructor.DeclaringType.FullName == "System.Reflection.AssemblyFileVersionAttribute")
			            {
			                string currVersionString = (string) cad.ConstructorArguments[0].Value;
			                return VersionStringToVersion(currVersionString);
			            }
			        }
			    }
			    else
			        return 0;
			}
			catch (Exception ex)
			{
			    Trace.WriteLine(ex.ToString());
			}
			finally
			{
                AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= CurrentDomain_ReflectionOnlyAssemblyResolve;
			}

			return 1000000;
		}

        Assembly CurrentDomain_ReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
        {
            return Assembly.ReflectionOnlyLoad(args.Name);
        }

		private string UpdateLocation
		{
			get { return "http://www.hristopavlov.net/Tangra3"; }
		}

		private string UpdatesXmlFileLocation
		{
			get
			{
				return UpdateLocation + (TangraConfig.Settings.Generic.AcceptBetaUpdates ? "/Beta.xml" : "/Updates.xml");
			}
		}
	}
}
