using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tangra.SDK;

namespace Tangra.OccultTools
{
	[Serializable]
	public class AotaAction : MarshalByRefObject, ITangraAddinAction
	{
		private ITangraHost m_TangraHost;
		private OccultToolsAddinSettings m_Settings;

		public AotaAction(OccultToolsAddinSettings settings, ITangraHost tangraHost)
		{
			m_Settings = settings;
			m_TangraHost = tangraHost;
		}

		public AddinActionType ActionType
		{
			get { return AddinActionType.LightCurve; }
		}

		public IntPtr Icon
		{
			get { return Properties.Resource.Occult.ToBitmap().GetHbitmap(); }
		}

		public int IconTransparentColorARGB
		{
			get { return System.Drawing.Color.Transparent.ToArgb(); }
		}

		public void Execute()
		{
			ILightCurveDataProvider dataProvider = m_TangraHost.GetLightCurveDataProvider();
			if (dataProvider != null && OccultUtilitiesWrapper.HasSupportedVersionOfOccult(m_Settings.OccultLocation))
			{
				OccultUtilitiesWrapper.RunAOTA(dataProvider);
			}
		}	

		public string DisplayName
		{
			get { return "Extract Event Times with AOTA"; }
		}
	}
}
