using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.Model.Config
{
	public class CatalogMagnitudeBand
	{
		private string m_DisplayName;
		private Guid m_Id;

		public Guid Id
		{
			get { return m_Id; }
		}

		public CatalogMagnitudeBand(Guid id, string displayName)
		{
			m_DisplayName = displayName;
			m_Id = id;
		}

		public override string ToString()
		{
			return m_DisplayName;
		}
	}
}
