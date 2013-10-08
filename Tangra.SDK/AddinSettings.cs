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
