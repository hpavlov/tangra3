using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.Model.Video
{
	public enum MovementType
	{
		Jump,
		Step,
		StepBackwards,
		Refresh
	}

	public enum FramePlaySpeed
	{
		Normal = 0,
		Slower = 1,
		Fastest = 2
	}

	public enum PixelIntegrationType
	{
		Mean = 0,
		Median
	}

	public enum FrameIntegratingMode
	{
		NoIntegration = 0,
		SlidingAverage,
		SteppedAverage
	}
}
