using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.VideoOperations.LightCurves.Tracking
{
	public class TrackedObjectLight : ITrackedObject
	{
		public Model.Image.IImagePixel Center { get; private set; }

		public Model.Image.IImagePixel LastKnownGoodPosition { get; set; }

		public bool IsLocated { get; private set; }

		public bool IsOffScreen { get; private set; }

		public ITrackedObjectConfig OriginalObject { get; private set; }

		public TrackedObjectLight(ITrackedObjectConfig objectConfig)
		{
			OriginalObject = objectConfig;
		}


		public int TargetNo { get; private set; }


		public Model.Astro.PSFFit PSFFit { get; private set; }
	}
}
