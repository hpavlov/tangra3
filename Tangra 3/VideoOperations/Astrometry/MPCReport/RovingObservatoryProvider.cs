using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tangra.Model.Config;
using Tangra.MotionFitting;

namespace Tangra.VideoOperations.Astrometry.MPCReport
{
    public class RovingObservatoryProvider : IRovingObsLocationProvider
    {
        private IWin32Window m_ParentWindow;

        public RovingObservatoryProvider(IWin32Window parentWindow)
        {
            m_ParentWindow = parentWindow;
        }

        private RovingObsLocation m_CurrentDataFileRovingObsLocation = null;

        public void ResetCurrentObcLocation()
        {
            m_CurrentDataFileRovingObsLocation = null;
        }

        public RovingObsLocation GetRovingObsLocation()
        {
            if (m_CurrentDataFileRovingObsLocation != null &&
                m_CurrentDataFileRovingObsLocation.IsValid)
            {
                return m_CurrentDataFileRovingObsLocation;
            }

            var frmRovObsLoc = new frmRovingObsLocation();
            if (frmRovObsLoc.ShowDialog(m_ParentWindow) == DialogResult.OK)
            {
                var obsLoc = frmRovObsLoc.ObservatoryLocation;
                if (obsLoc != null && obsLoc.IsValid)
                {
                    m_CurrentDataFileRovingObsLocation = obsLoc;
                }
                return obsLoc;
            }

            return new RovingObsLocation() { IsValid = false };
        }
    }
}
