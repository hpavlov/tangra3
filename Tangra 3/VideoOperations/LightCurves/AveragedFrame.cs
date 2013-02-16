using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Tangra.Model.Astro;
using Tangra.Model.Image;

namespace Tangra.VideoOperations.LightCurves
{
	internal class AveragedFrame
	{
		private AstroImage m_Image;

		public AveragedFrame(AstroImage image)
		{
#if !PRODUCTION
			Trace.Assert(image != null && image.Pixelmap != null);
#endif

			m_Image = image;
		}

		public Pixelmap Pixelmap
		{
			get { return m_Image.Pixelmap; }
		}
	}
}
