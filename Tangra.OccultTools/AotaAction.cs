using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tangra.OccultTools.OccultWrappers;
using Tangra.SDK;

namespace Tangra.OccultTools
{
	[Serializable]
	public class AotaAction : MarshalByRefObject, ITangraAddinAction
	{
		private ITangraHost m_TangraHost;
		private OccultToolsAddinSettings m_Settings;
		private OccultToolsAddin m_Addin;
	    private IOccultWrapper m_OccultWrapper;
	    private bool m_AOTAFormVisible = false;

		internal AotaAction(OccultToolsAddinSettings settings, ITangraHost tangraHost, IOccultWrapper occultWrapper, OccultToolsAddin addin)
		{
			m_Addin = addin;
			m_Settings = settings;
			m_TangraHost = tangraHost;
		    m_OccultWrapper = occultWrapper;
		    m_AOTAFormVisible = false;
		}

		public AddinActionType ActionType
		{
			get { return AddinActionType.LightCurveEventTimeExtractor; }
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
            if (m_AOTAFormVisible)
            {
                ShowErrorMessage("AOTA is already running.");
                return;
            }

	        ILightCurveDataProvider dataProvider = m_TangraHost.GetLightCurveDataProvider();

	        if (!Directory.Exists(m_Settings.OccultLocation))
	        {
	            string locationFromOW = OccultWatcherHelper.GetConfiguredOccultLocation();
                if (Directory.Exists(locationFromOW))
	            {
	                m_Settings.OccultLocation = locationFromOW;
	                m_Settings.Save();
	            }
                else
	                m_Addin.Configure();
	        }

	        string errorMessage = m_OccultWrapper.HasSupportedVersionOfOccult(m_Settings.OccultLocation);
            if (errorMessage != null)
	        {
                ShowErrorMessage(errorMessage);
	            m_Addin.Configure();
	        }

	        if (dataProvider != null)
	        {
	            errorMessage = m_OccultWrapper.HasSupportedVersionOfOccult(m_Settings.OccultLocation);
                if (errorMessage == null)
                {
                    if (m_OccultWrapper.RunAOTA(dataProvider, m_TangraHost.ParentWindow))
                        m_AOTAFormVisible = true;
                }
		        else
                    ShowErrorMessage(errorMessage);
	        }
	    }

        public void OnAOTAFormClosing()
        {
            if (m_AOTAFormVisible)
            {
                ILightCurveDataProvider dataProvider = m_TangraHost.GetLightCurveDataProvider();

                AotaReturnValue result = m_OccultWrapper.GetAOTAResult();

                if (result != null &&
                    result.AreResultsAvailable)
                {
                    dataProvider.SetTimeExtractionEngine(result.AOTAVersion);

                    if (result.IsMiss)
                    {
                        dataProvider.SetNoOccultationEvents();
                    }
                    else
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            if (!result.EventResults[i].IsNonEvent)
                            {
                                dataProvider.SetFoundOccultationEvent(
                                    i,
                                    result.EventResults[i].D_Frame,
                                    result.EventResults[i].R_Frame,
                                    result.EventResults[i].D_FrameUncertMinus,
                                    result.EventResults[i].D_FrameUncertPlus,
                                    result.EventResults[i].R_FrameUncertMinus,
                                    result.EventResults[i].R_FrameUncertPlus,
                                    result.EventResults[i].D_UTC,
                                    result.EventResults[i].R_UTC);
                            }
                        }
                    }
                }

                m_AOTAFormVisible = false;

                dataProvider.FinishedLightCurveEventTimeExtraction();
            }
        }

        public void OnLightCurveSelectedFrameChanged()
        {
            if (m_AOTAFormVisible)
            {
                ILightCurveDataProvider dataProvider = m_TangraHost.GetLightCurveDataProvider();
                int currFrameId = dataProvider.CurrentlySelectedFrameNumber;
                m_OccultWrapper.NotifyAOTAOfCurrentFrameChanged(currFrameId);
            }
        }

        public void Finalise()
        {
            m_OccultWrapper.EnsureAOTAClosed();
            m_AOTAFormVisible = false;
        }

	    private void ShowErrorMessage(string errorMessage)
		{
			MessageBox.Show(
				m_TangraHost.ParentWindow,
                errorMessage,
				"Occult Tools for Tangra",
				MessageBoxButtons.OK,
				MessageBoxIcon.Error);			
		}

		public string DisplayName
		{
			get { return "Extract Event Times with AOTA"; }
		}
	}
}
