using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Tangra.Model.Helpers;
using Tangra.Properties;
using Tangra.SDK;

namespace Tangra.Addins
{
	[Serializable]
	public class SettingsStorageProvider: MarshalByRefObject, ISettingsStorageProvider
	{
		private XmlSerializer m_AllSettingSerializer;
		private string m_AddinTypeName;

		public SettingsStorageProvider(string addinTypeName)
		{
			m_AllSettingSerializer = new XmlSerializer(typeof(AllAddinSettings));
			m_AddinTypeName = addinTypeName;
		}

		public override object InitializeLifetimeService()
		{
			// The lifetime of the object is managed by Tangra
			return null;
		}

		private AllAddinSettings GetFreshAllAddinSettings()
		{
			try
			{
				return (AllAddinSettings)m_AllSettingSerializer.Deserialize(new StringReader(Settings.Default.AddinSettings));
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.GetFullStackTrace());

				return new AllAddinSettings();
			}
		}

		public string ReadSettings()
		{
			AllAddinSettings allAddinSettings = GetFreshAllAddinSettings();
			AddinSettings addinSettings = null;			
			try
			{
				addinSettings = allAddinSettings.PersistedSettings.SingleOrDefault(x => x.ClassName == m_AddinTypeName);
				if (addinSettings != null)
					return addinSettings.Settings;
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.GetFullStackTrace());
			}

			return string.Empty;
		}

		public void WriteSettings(string settings)
		{
			AllAddinSettings allAddinSettings = GetFreshAllAddinSettings();
			AddinSettings addinSettings = null;			
			try
			{
				addinSettings = allAddinSettings.PersistedSettings.SingleOrDefault(x => x.ClassName == m_AddinTypeName);				
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.GetFullStackTrace());
			}

			if (addinSettings == null)
			{
				addinSettings = new AddinSettings() {ClassName = m_AddinTypeName};
				allAddinSettings.PersistedSettings.Add(addinSettings);
			}

			addinSettings.Settings = settings;

			var output = new StringBuilder();
			using (var wrt = new StringWriter(output))
			{
				m_AllSettingSerializer.Serialize(wrt, allAddinSettings);
			}

			Settings.Default.AddinSettings = output.ToString();
			Settings.Default.Save();
		}
	}
}
