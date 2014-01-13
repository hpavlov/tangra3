using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Occult.SDK;

namespace Tangra.OccultTools.OccultWrappers
{
    public class OccultWrapperFactory
    {
        private class DelayedCreationOccultWrapper : IOccultWrapper
        {
            private OccultToolsAddinSettings m_Settings;
            private IAOTAClientCallbacks m_Callbacks;
            private IOccultWrapper m_Delegate;

            public DelayedCreationOccultWrapper(OccultToolsAddinSettings settings, IAOTAClientCallbacks callbacks)
            {
                m_Settings = settings;
                m_Callbacks = callbacks;
                m_Delegate = null;
            }

            private void EnsureDelegate()
            {
                if (!EnsureDelegate(m_Settings.OccultLocation))
                    throw new InvalidOperationException("Cannot fund Occult");
            }


            private bool EnsureDelegate(string occultLocation)
            {
                if (m_Delegate != null)
                    return true;

                try
                {
                    m_Delegate = new OccultSDKWrapper(m_Callbacks);
                    if (m_Delegate.HasSupportedVersionOfOccult(occultLocation))
                        return true;
                }
                catch
                {
                    m_Delegate = null;
                }


                try
                {
                    m_Delegate = new OccultReflectionWrapper();
                    if (m_Delegate.HasSupportedVersionOfOccult(occultLocation))
                        return true;
                }
                catch
                { }

                m_Delegate = null;
                return false;
            }

            public bool HasSupportedVersionOfOccult(string occultLocation)
            {
                if (!EnsureDelegate(occultLocation))
                    return false;

                return m_Delegate.HasSupportedVersionOfOccult(occultLocation);
            }

            public AotaReturnValue RunAOTA(SDK.ILightCurveDataProvider dataProvider, System.Windows.Forms.IWin32Window parentWindow)
            {
                EnsureDelegate();

                return m_Delegate.RunAOTA(dataProvider, parentWindow);
            }

            public void EnsureAOTAClosed()
            {
                if (m_Delegate != null)
                    m_Delegate.EnsureAOTAClosed();
            }
        }

        internal static IOccultWrapper CreateOccultWrapper(OccultToolsAddinSettings settings, IAOTAClientCallbacks callbacks)
        {
            return new DelayedCreationOccultWrapper(settings, callbacks);
        }
    }

}
