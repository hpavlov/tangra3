using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using Tangra.Model.Config;
using Tangra.Properties;

namespace Tangra.Controller
{
	public class AutoUpdatesController
	{
		private frmMain m_MainFrom;

		public AutoUpdatesController(frmMain mainFrom)
		{
			m_MainFrom = mainFrom;
		}

		internal delegate void OnUpdateEventDelegate(int eventCode);

		internal void OnUpdateEvent(int eventCode)
		{
			if (eventCode == MSG_ID_NEW_OCCUREC_UPDATE_AVAILABLE)
			{
				m_MainFrom.pnlNewVersionAvailable.Visible = true;
				m_ShowNegativeResultMessage = false;
			}
			else if (eventCode == MSG_ID_NO_OCCUREC_UPDATES_AVAILABLE)
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

		private byte MSG_ID_NEW_OCCUREC_UPDATE_AVAILABLE = 13;
		private byte MSG_ID_NO_OCCUREC_UPDATES_AVAILABLE = 14;

		public void CheckForUpdates(bool manualCheck)
		{
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
			try
			{
				m_LastUpdateTime = DateTime.Now;

				int serverConfigVersion;
				if (NewUpdatesAvailable(out serverConfigVersion) != null)
				{
					Trace.WriteLine("There is a new update.", "Update");
					m_MainFrom.Invoke(new OnUpdateEventDelegate(OnUpdateEvent), MSG_ID_NEW_OCCUREC_UPDATE_AVAILABLE);
				}
				else
				{
					Trace.WriteLine(string.Format("There are no new {0}updates.", TangraConfig.Settings.Generic.AcceptBetaUpdates ? "beta " : ""), "Update");
					m_MainFrom.Invoke(new OnUpdateEventDelegate(OnUpdateEvent), MSG_ID_NO_OCCUREC_UPDATES_AVAILABLE);
				}

				Settings.Default.LastCheckedForUpdates = m_LastUpdateTime;
				Settings.Default.Save();
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex, "Update");
			}
		}

		private string occuRecUpdateServerVersion;

		public XmlNode NewUpdatesAvailable(out int configUpdateVersion)
		{
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
							Trace.WriteLine("Update location: " + updateUri.ToString());
							Trace.WriteLine("Current version: " + latestVersion.ToString());
							Trace.WriteLine("New version: " + Version.ToString());

							XmlNode occuRecUpdateNode = xmlDoc.SelectSingleNode("/Tangra3/AutoUpdate");
							if (occuRecUpdateNode != null)
							{
								occuRecUpdateServerVersion = occuRecUpdateNode.Attributes["Version"].Value;
								Trace.WriteLine("OccuRecUpdate new version: " + occuRecUpdateServerVersion);
							}
							else
								occuRecUpdateServerVersion = null;

							latestVersion = Version;
							latestVersionNode = updateNode;
						}
					}

					foreach (XmlNode updateNode in xmlDoc.SelectNodes("/Tangra3/ModuleUpdate[@MustExist = 'false']"))
					{
						if (updateNode.Attributes["Version"] == null) continue;

						int Version = int.Parse(updateNode.Attributes["Version"].Value);
						latestVersion = CurrentlyInstalledModuleVersion(updateNode.Attributes["File"].Value);

						if (latestVersion < Version)
						{
							Trace.WriteLine("Update location: " + updateUri.ToString());
							Trace.WriteLine("Module: " + updateNode.Attributes["File"].Value);
							Trace.WriteLine("Current version: " + latestVersion.ToString());
							Trace.WriteLine("New version: " + Version.ToString());

							XmlNode occuRecUpdateNode = xmlDoc.SelectSingleNode("/Tangra3/AutoUpdate");
							if (occuRecUpdateNode != null)
							{
								occuRecUpdateServerVersion = occuRecUpdateNode.Attributes["Version"].Value;
								Trace.WriteLine("Tangra3Update new version: " + occuRecUpdateServerVersion);
							}
							else
								occuRecUpdateServerVersion = null;

							latestVersion = Version;
							latestVersionNode = updateNode;
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


					return latestVersionNode;
				}
			}
			finally
			{
				// Close response
				if (response != null)
					response.Close();
			}

			return null;
		}

		private int CurrentlyInstalledTangra3Version()
		{
			try
			{
				Assembly asm = Assembly.GetExecutingAssembly();
				Version owVer = asm.GetName().Version;
				return 10000 * owVer.Major + 1000 * owVer.Minor + 100 * owVer.Build + owVer.Revision;
			}
			catch
			{ }

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
							string currVersionString = (string)cad.ConstructorArguments[0].Value;
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

			return 1000000;
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
