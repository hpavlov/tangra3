using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tangra.SDK;

namespace Tangra.Addins
{
	[Serializable]
	public class OccultToolsAddin : MarshalByRefObject, ITangraAddin
	{
		private ITangraHost m_Host;

		public void Initialise(ITangraHost host)
		{
			m_Host = host;
		}

		public void Finalise()
		{
			
		}
	}
}
