using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.Model.Config
{
	public class TangraConfig
	{
		public static TangraConfig Settings = new TangraConfig(false);

		public TangraConfig()
			: this(true)
		{ }

		public TangraConfig(bool readOnly)
		{
			m_IsReadOnly = readOnly;
		}

		private bool m_IsReadOnly = true;
		public bool IsReadOnly
		{
			get { return m_IsReadOnly; }
		}

		public AdvsSettings ADVS = new AdvsSettings();
	}
}
