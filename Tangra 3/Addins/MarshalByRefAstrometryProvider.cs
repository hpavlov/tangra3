using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.SDK;

namespace Tangra.Addins
{
    [Serializable]
    public class MarshalByRefAstrometryProvider : IAstrometryProvider
    {
        private ITangraAstrometricSolution m_TangraAstrometricSolution;

        public MarshalByRefAstrometryProvider(IAstrometryProvider localAstrometryProvider)
        {
            if (localAstrometryProvider != null)
                m_TangraAstrometricSolution = localAstrometryProvider.GetCurrentFrameAstrometricSolution();
            else
                m_TangraAstrometricSolution = null;
        }

        public ITangraAstrometricSolution GetCurrentFrameAstrometricSolution()
        {
            return m_TangraAstrometricSolution;
        }
    }
}
