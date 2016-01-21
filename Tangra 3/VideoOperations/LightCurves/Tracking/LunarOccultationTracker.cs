using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.Model.Astro;

namespace Tangra.VideoOperations.LightCurves.Tracking
{
	internal class LunarOccultationTracker : OneStarTracker
	{
		public LunarOccultationTracker(List<TrackedObjectConfig> measuringStars)
			: base(measuringStars)
		{ }

		protected override void TrackSingleStar(int frameNo, IAstroImage astroImage)
		{
			// TODO: Do some smarts for Lunar Occultations? 

			base.TrackSingleStar(frameNo, astroImage);
		}
	}
}
