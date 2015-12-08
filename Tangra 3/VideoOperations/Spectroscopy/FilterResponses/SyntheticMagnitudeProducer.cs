using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.VideoOperations.Spectroscopy.FilterResponses
{
	[Flags]
	public enum ExportedMags
	{
		None = 0,
		Johnson_U = 1,
		Johnson_B = 2,
		Johnson_V = 4,
		Johnson_R = 8,
		Johnson_I = 16,
		Sloan_u = 32,
		Sloan_g = 64,
		Sloan_r = 128,
		Sloan_i = 256,
		Sloan_z = 512
	}

	public class SyntheticMagnitudeProducer
	{

	}
}
