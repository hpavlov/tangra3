using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Xml.Serialization;
using Tangra.SDK;

namespace Tangra.KweeVanWoerden
{
	[Serializable]
	public class KweeVanWoerdenAddinSettings
	{
		public bool UseSimulatedDataSet { get; set; }
	}

	public static class Extensions
	{
		internal static ISettingsStorageProvider SettingsProvider { get; set; }
		private static XmlSerializer m_Serializer;

		static Extensions()
		{
			m_Serializer = new XmlSerializer(typeof(KweeVanWoerdenAddinSettings));
		}

		public static void Save(this KweeVanWoerdenAddinSettings settings)
		{
			var output = new StringBuilder();
			using (var wrt = new StringWriter(output))
			{
				m_Serializer.Serialize(wrt, settings);
			}

			SettingsProvider.WriteSettings(output.ToString());
		}

		public static KweeVanWoerdenAddinSettings Load(this string serializedSettings)
		{
			try
			{
				using (var rdr = new StringReader(serializedSettings))
				{
					return (KweeVanWoerdenAddinSettings)m_Serializer.Deserialize(rdr);
				}
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.ToString());
			}

			return new KweeVanWoerdenAddinSettings();
		}
	}

	[Serializable]
	public class KweeVanWoerdenAddin : MarshalByRefObject, ITangraAddin
	{
		private ITangraHost m_Host;
		private ISettingsStorageProvider m_SettingsProvider;
		private ITangraAddinAction[] m_SupportedAddinActions;
		private KweeVanWoerdenMinimum m_KweeCanWoerdenAction;

		private KweeVanWoerdenAddinSettings m_Settings;

		public void Initialise(ITangraHost host)
		{
			m_Host = host;
			m_SettingsProvider = m_Host.GetSettingsProvider();
			Extensions.SettingsProvider = m_SettingsProvider;
			m_Settings = m_SettingsProvider.ReadSettings().Load();

			m_KweeCanWoerdenAction = new KweeVanWoerdenMinimum(m_Host, m_Settings);

			m_SupportedAddinActions = new ITangraAddinAction[] { m_KweeCanWoerdenAction };

			RemotingConfiguration.RegisterWellKnownServiceType(typeof(KweeVanWoerdenAddin), "KweeVanWoerdenAddin", WellKnownObjectMode.Singleton);
			RemotingConfiguration.RegisterWellKnownServiceType(typeof(KweeVanWoerdenMinimum), "KweeVanWoerdenMinimum", WellKnownObjectMode.Singleton);
		}

		public void Finalise()
		{

		}

		public void Configure()
		{
			var frm = new frmAddinConfig();
			frm.SetSettings(m_Settings);
			frm.ShowDialog(m_Host.ParentWindow);
		}

		public string DisplayName
		{
			get { return "Eclipsing Binaries for Tangra"; }
		}

		public string Author
		{
			get { return "A.Mallama && H.Pavlov"; }
		}

		public string Version
		{
			get { return "1.0"; }
		}

		public string Description
		{
			get { return "Addin for Tangra 3 for determining the time of minima of eclipsing binary stars"; }
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
