/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tangra.SDK;

namespace Tangra.Addins
{
	[Serializable]
	public class TangraHostDelegate : MarshalByRefObject, ITangraHost
	{
		private string m_AddinTypeName;
		private IAddinManager m_AddinManager;

		public TangraHostDelegate(string addinTypeName, IAddinManager addinManager)
		{
			m_AddinTypeName = addinTypeName;
			m_AddinManager = addinManager;
		}

		public override object InitializeLifetimeService()
		{
			// The lifetime of the object is managed by Tangra
			return null;
		}

		public ISettingsStorageProvider GetSettingsProvider()
		{
			return new SettingsStorageProvider(m_AddinTypeName);;
		}

		public IWin32Window ParentWindow
		{
			get { return m_AddinManager.ParentWindow; }
		}


		public ILightCurveDataProvider GetLightCurveDataProvider()
		{
			return m_AddinManager.LightCurveDataProvider;
		}

        public void PositionToFrame(int frameNo)
        {
            m_AddinManager.PositionToFrame(frameNo);
        }
        
	}
}
