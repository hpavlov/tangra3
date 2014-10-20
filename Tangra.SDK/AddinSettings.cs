/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Tangra.SDK
{
	[XmlRoot]
	public class AllAddinSettings
	{
		[XmlArrayItem("Setting")]
		public List<AddinSettings> PersistedSettings = new List<AddinSettings>();
	}

	[XmlRoot]
	public class AddinSettings
	{
		public string Settings { get; set; }
		public string ClassName { get; set; }
	}

}
