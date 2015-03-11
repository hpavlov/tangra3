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
            m_TangraAstrometricSolution = localAstrometryProvider.GetCurrentFrameAstrometricSolution();
        }

        public ITangraAstrometricSolution GetCurrentFrameAstrometricSolution()
        {
            return m_TangraAstrometricSolution;
        }
    }
}
