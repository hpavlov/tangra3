using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.VideoOperations.LightCurves
{
    class LCStateViewingLightCurve : LCState
    {
        internal LCStateViewingLightCurve(LCStateMachine context)
            : base(context)
        { }

		public override void Initialize()
		{
			base.Initialize();

			Context.SetCanPlayVideo(false);
		}
    }
}
