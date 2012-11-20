using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Tangra.SDK
{
	public interface ITangraOpenedVideo
	{
		Bitmap LoadFrame(uint frameId);
		uint FirstFrame { get; }
		uint LastFrame { get; }
		uint CurrentFrame { get; }
	}
}
