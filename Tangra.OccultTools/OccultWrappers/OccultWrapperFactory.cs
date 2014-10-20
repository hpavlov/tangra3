/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

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

            private string m_IncompatibleVersionsErrorMessage = null;


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
                    m_IncompatibleVersionsErrorMessage = m_Delegate.HasSupportedVersionOfOccult(occultLocation);
                    if (m_IncompatibleVersionsErrorMessage == null)
                        return true;
                }
                catch
                {
                    m_Delegate = null;
                }

                m_Delegate = null;
                return false;
            }

            public string HasSupportedVersionOfOccult(string occultLocation)
            {
                if (!EnsureDelegate(occultLocation))
                    return m_IncompatibleVersionsErrorMessage;

                return m_Delegate.HasSupportedVersionOfOccult(occultLocation);
            }

            public string GetOccultCurrentOccultVersion(string occultLocation)
            {
                if (!EnsureDelegate(occultLocation))
                    return m_IncompatibleVersionsErrorMessage;

                return m_Delegate.GetOccultCurrentOccultVersion(occultLocation);
            }

            public bool RunAOTA(SDK.ILightCurveDataProvider dataProvider, System.Windows.Forms.IWin32Window parentWindow)
            {
                EnsureDelegate();

                return m_Delegate.RunAOTA(dataProvider, parentWindow);
            }

            public AotaReturnValue GetAOTAResult()
            {
                if (m_Delegate != null)
                    return m_Delegate.GetAOTAResult();
                else
                    return null;
            }

            public void EnsureAOTAClosed()
            {
                if (m_Delegate != null)
                    m_Delegate.EnsureAOTAClosed();
            }

            public void NotifyAOTAOfCurrentFrameChanged(int currFrameId)
            {
                if (m_Delegate != null)
                    m_Delegate.NotifyAOTAOfCurrentFrameChanged(currFrameId);
            }
        }

        internal static IOccultWrapper CreateOccultWrapper(OccultToolsAddinSettings settings, IAOTAClientCallbacks callbacks)
        {
            return new DelayedCreationOccultWrapper(settings, callbacks);
        }
    }

}
