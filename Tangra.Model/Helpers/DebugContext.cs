using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangra.Model.Helpers
{
	public class DebugContext
	{
		public class SubPixelData
		{
			public bool[,] Included;
			public double TotalSubpixels;
			public double TotalReading;
			public bool FullyIncluded;
			public float X0;
			public float Y0;
		}

		public static List<SubPixelData[,]> CurrentSubPixels = new List<SubPixelData[,]>();

		public static int Width;

		public static int Height;

		public static int TargetNo;

		public static bool DebugSubPixelMeasurements = false;
	}
}
