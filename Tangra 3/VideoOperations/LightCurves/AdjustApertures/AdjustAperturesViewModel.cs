using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.Model.Config;
using Tangra.VideoOperations.LightCurves.Tracking;

namespace Tangra.VideoOperations.LightCurves.AdjustApertures
{
	internal class AdjustAperturesViewModel
	{
		private float[] m_StartingApertures;
		public TrackedObjectConfig[] MeasuringStars;

		public float[] Apertures;
		public float[] FWHMs;

		internal AdjustAperturesViewModel(List<float> startingApertures, List<TrackedObjectConfig> measuringStars)
		{
			m_StartingApertures = startingApertures.ToArray();
			MeasuringStars = measuringStars.ToArray();

			Apertures = new float[m_StartingApertures.Length];
			FWHMs = new float[m_StartingApertures.Length];
			for (int i = 0; i < m_StartingApertures.Length; i++)
			{
				Apertures[i] = m_StartingApertures[i];
				if (MeasuringStars[i].Gaussian != null && MeasuringStars[i].Gaussian.IsSolved)
					FWHMs[i] = (float)MeasuringStars[i].Gaussian.FWHM;
				else if (TangraConfig.Settings.Photometry.SignalApertureUnitDefault == TangraConfig.SignalApertureUnit.FWHM)
					FWHMs[i] = MeasuringStars[i].ApertureInPixels / TangraConfig.Settings.Photometry.DefaultSignalAperture;
				else
					FWHMs[i] = MeasuringStars[i].ApertureInPixels / 1.2f;
			}
		}
	}
}
