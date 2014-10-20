/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tangra.SDK;

namespace Tangra.Addins
{
	[Serializable]
	public class StandardTangraAddin : MarshalByRefObject, ITangraAddin
	{
		private ITangraHost m_Host;

		public void Initialise(ITangraHost host)
		{
			m_Host = host;
		}

		public void Finalise()
		{
			
		}

		public void Configure()
		{
			
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
			get { return "1.0"; }
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
			return new ITangraAddinAction[] { };
		}

		public void OnEventNotification(AddinFiredEventType eventType)
		{
			
		}
	}
}
