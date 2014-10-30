using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.SDK;

namespace Tangra.KweeVanWoerden
{
	[Serializable]
	public class KweeVanWoerdenAddin : MarshalByRefObject, ITangraAddin
	{
		private ITangraHost m_Host;
		private ITangraAddinAction[] m_SupportedAddinActions;
		private KweeVanWoerdenMinimum m_KweeCanWoerdenAction;

		public void Initialise(ITangraHost host)
		{
			m_Host = host;

			m_KweeCanWoerdenAction = new KweeVanWoerdenMinimum(m_Host);

			m_SupportedAddinActions = new ITangraAddinAction[] { m_KweeCanWoerdenAction };
		}

		public void Finalise()
		{

		}

		public void Configure()
		{ }

		public string DisplayName
		{
			get { return "Eclipsing Binaries Addin"; }
		}

		public string Author
		{
			get { return "Anthony Mallama"; }
		}

		public string Version
		{
			get { return "1.0"; }
		}

		public string Description
		{
			get { return "Addin for Tangra 3 for determining eclipsing binaries minimum epoch using the K. K. Kwee & H. Van Woerden method (http://adsabs.harvard.edu/abs/1956BAN....12..327K)."; }
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
