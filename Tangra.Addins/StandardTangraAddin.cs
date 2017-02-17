/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using Tangra.SDK;

namespace Tangra.Addins
{
	[Serializable]
	public class StandardTangraAddin : MarshalByRefObject, ITangraAddin
	{
		private ITangraHost m_Host;
		private MeasurementsExportAddin m_MeasurementsExportAddin;

		public void Initialise(ITangraHost host)
		{
			m_Host = host;

			m_MeasurementsExportAddin = new MeasurementsExportAddin();
			m_MeasurementsExportAddin.Initialise(m_Host);


			RemotingConfiguration.RegisterWellKnownServiceType(typeof(StandardTangraAddin), "StandardTangraAddin", WellKnownObjectMode.Singleton);
			RemotingConfiguration.RegisterWellKnownServiceType(typeof(MeasurementsExportAddin), "MeasurementsExportAddin", WellKnownObjectMode.Singleton);
		}

		public void Finalise()
		{
			m_MeasurementsExportAddin.Finalise();
		}

		public void Configure()
		{
			var frm = new frmConfigureAddin();
			frm.ShowDialog(m_Host.ParentWindow);
		}

		public string DisplayName
		{
			get { return "Standard Tangra Addins"; }
		}

		public string Author
		{
			get { return "Hristo Pavlov"; }
		}

		public string Version
		{
			get { return "1.1"; }
		}

		public string Description
		{
			get { return "Standard set of addins for Tangra 3."; }
		}

		public string Url
		{
			get { return "http://www.hristopavlov.net/tangra3/addins"; }
		}

		public ITangraAddinAction[] GetAddinActions()
		{
			return new ITangraAddinAction[] { m_MeasurementsExportAddin };
		}

		public void OnEventNotification(AddinFiredEventType eventType)
		{
			if (eventType == AddinFiredEventType.BeginMultiFrameAstrometry)
			{
				m_MeasurementsExportAddin.OnBeginMultiFrameAstrometry();
			}
			else if (eventType == AddinFiredEventType.EndMultiFrameAstrometry)
			{
				m_MeasurementsExportAddin.OnEndMultiFrameAstrometry();
			}
		}
	}
}
