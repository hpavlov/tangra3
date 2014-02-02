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
using Occult.SDK;
using Tangra.OccultTools.OccultWrappers;
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
	public class OccultToolsAddin : MarshalByRefObject, ITangraAddin, IAOTAClientCallbacks
	{
		private ITangraHost m_Host;
		private ISettingsStorageProvider m_SettingsProvider;
		private ITangraAddinAction[] m_SupportedAddinActions;
        private IOccultWrapper m_OccultWrapper;

		private OccultToolsAddinSettings m_Settings;
	    private AotaAction m_AotaAction;

		public void Initialise(ITangraHost host)
		{
			m_Host = host;
			m_SettingsProvider = m_Host.GetSettingsProvider();
			Extensions.SettingsProvider = m_SettingsProvider;

			m_Settings = m_SettingsProvider.ReadSettings().Load();

            m_OccultWrapper = OccultWrapperFactory.CreateOccultWrapper(m_Settings, this);

		    m_AotaAction = new AotaAction(m_Settings, host, m_OccultWrapper, this);

			m_SupportedAddinActions = new ITangraAddinAction[] { m_AotaAction };

			RemotingConfiguration.RegisterWellKnownServiceType(typeof(OccultToolsAddin), "OccultToolsAddin", WellKnownObjectMode.Singleton);
			RemotingConfiguration.RegisterWellKnownServiceType(typeof(AotaAction), "AotaAction", WellKnownObjectMode.Singleton);
		}

		public void Finalise()
		{
			RemotingServices.Disconnect(this);
		}

		public void Configure()
		{
			var frm = new frmConfig();
            frm.SetSettings(m_Settings, m_OccultWrapper);
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
			get
			{
			    string occultStatus;

			    if (string.IsNullOrEmpty(m_Settings.OccultLocation))
			        occultStatus = "Occult location hasn't been configured.";
			    else
			    {
			        occultStatus = m_OccultWrapper.HasSupportedVersionOfOccult(m_Settings.OccultLocation);
                    if (occultStatus == null)
                    {
                        occultStatus = m_OccultWrapper.GetOccultCurrentOccultVersion(m_Settings.OccultLocation);
                    }
			    }
                

                string descr = string.Format("This addin provides utilities from Occult, including the Asteroid Occultation Timing Analyser (AOTA).\r\n\r\n{0}", occultStatus);

			    return descr;
			}
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
			if (eventType == AddinFiredEventType.LightCurveSelectedFrameChanged)
			{
			    m_AotaAction.OnLightCurveSelectedFrameChanged();
			}
		}

        public void PositionToFrame(int frameNo)
        {
            m_Host.PositionToFrame(frameNo);
        }

        public void OnAOTAFormClosing()
        {
            m_AotaAction.OnAOTAFormClosing();
        }
    }
}
