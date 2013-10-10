using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using Tangra.SDK;

namespace Tangra.OccultTools
{
	[Serializable]
	public class OccultToolsAddinSettings
	{
		public string OccultLocation { get; set; }
	}

	public static class Extensions
	{
		internal static ISettingsStorageProvider SettingsProvider { get; set; }
		private static XmlSerializer m_Serializer;

		static Extensions()
		{
			m_Serializer = new XmlSerializer(typeof (OccultToolsAddinSettings));
		}

		public static void Save(this OccultToolsAddinSettings settings)
		{			
			var output = new StringBuilder();
			using (var wrt = new StringWriter(output))
			{
				m_Serializer.Serialize(wrt, settings);
			}

			SettingsProvider.WriteSettings(output.ToString());
		}

		public static OccultToolsAddinSettings Load(this string serializedSettings)
		{
			try
			{
				using (var rdr = new StringReader(serializedSettings))
				{
					return (OccultToolsAddinSettings)m_Serializer.Deserialize(rdr);
				}
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.ToString());
			}

			return new OccultToolsAddinSettings();
		}
	}

	[Serializable]
	public class OccultToolsAddin : MarshalByRefObject, ITangraAddin
	{
		private ITangraHost m_Host;
		private ISettingsStorageProvider m_SettingsProvider;
		private ITangraAddinAction[] m_SupportedAddinActions;

		private OccultToolsAddinSettings m_Settings;

		public void Initialise(ITangraHost host)
		{
			m_Host = host;
			m_SettingsProvider = m_Host.GetSettingsProvider();
			Extensions.SettingsProvider = m_SettingsProvider;

			m_Settings = m_SettingsProvider.ReadSettings().Load();

			m_SupportedAddinActions = new ITangraAddinAction[]
			{
				new AotaAction(m_Settings, host)
			};
		}

		public override object InitializeLifetimeService()
		{
			return null;
		}

		public void Finalise()
		{
			RemotingServices.Disconnect(this);
		}

		public void Configure()
		{
			var frm = new frmConfig();
			frm.SetSettings(m_Settings);
			frm.ShowDialog(m_Host.ParentWindow);
			
		}

		public string DisplayName
		{
			get { return "Occult Tools for Tangra"; }
		}

		public string Author
		{
			get { return "Hristo Pavlov"; }
		}

		public string Version
		{
			get { return "1.0"; }
		}

		public string Description
		{
			get { return "This addin provides utilities from Occult, including the Asteroid Occultation Timing Analyser (AOTA)"; }
		}

		public string Url
		{
			get { return "http://www.hristopavlov.net/tangra3/addins"; }
		}

		public ITangraAddinAction[] GetAddinActions()
		{
			return m_SupportedAddinActions;
		}

		public void OnEventNotification(AddinFiredEventType eventType)
		{
			
		}

	}
}
