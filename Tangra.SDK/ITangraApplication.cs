using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Tangra.SDK
{
	public interface ITangraApplication
	{
		ITangraLightCurve GetOpenedLightCurve();
		ITangraAstrometricSolution GetCurrentFrameAstrometricSolution();
		ITangraOpenedVideo GetTangraOpenedVideo();

		IWin32Window MainWindow { get; }
		IWin32Window LightCurvesWindow { get; }

	    Version GetVersion();
	}
}
