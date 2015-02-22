/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Tangra.Astrometry.Recognition;
using Tangra.Model.Astro;

namespace Tangra.Astrometry
{
	public class DistSolveEntry
	{
		public double DX;
		public double DY;
		public double DistRadians;
		public double ResidualRadians;
		public double ResidualPercent;
		public double ResidualArcSec;
		public ulong StarNo1;
        public ulong StarNo2;
	}

	[Serializable]
	public class FocalLengthFit : ISerializable
	{
		public double A;
		public double B;
		public double Variance;
		public readonly List<DistSolveEntry> Entries;

		public FocalLengthFit(double a, double b, double variance, List<DistSolveEntry> entries)
		{
			A = Math.Sqrt(a);
			B = Math.Sqrt(b);
			Variance = variance;
			Entries = entries;

			// A = Mx / F
			// B = My / F
		}

		public void GetFocalParameters(double focalLengthInMilimeters, out double cellWidthInMicrons, out double cellHeightInMicrons)
		{
			cellWidthInMicrons = A * focalLengthInMilimeters * 1000.0;
			cellHeightInMicrons = B * focalLengthInMilimeters * 1000.0;
		}

		public void GetFocalParameters(ref double cellWidthInMicrons, ref double cellHeightInMicrons, out double focalLengthInMilimeters)
		{
			if (cellWidthInMicrons < cellHeightInMicrons)
			{
				focalLengthInMilimeters = cellWidthInMicrons / A;
				cellHeightInMicrons = B * focalLengthInMilimeters;
			}
			else
			{
				focalLengthInMilimeters = cellHeightInMicrons / B;
				cellWidthInMicrons = A * focalLengthInMilimeters;
			}

			focalLengthInMilimeters /= 1000.0;
		}

		public void GetFocalParameters(double cellWidthInMicrons, double cellHeightInMicrons, out double focalLengthInMilimeters)
		{
			double fla = cellWidthInMicrons / (A * 1000.0);
			double flb = cellWidthInMicrons / (B * 1000.0);

			focalLengthInMilimeters = (fla + flb) / 2;
		}

		#region ISerializable Members

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("A", A);
			info.AddValue("B", B);
			info.AddValue("Variance", Variance);
		}

		public FocalLengthFit(SerializationInfo info, StreamingContext context)
		{
			A = info.GetDouble("A");
			B = info.GetDouble("B");
			Variance = info.GetDouble("Variance");
		}

		private FocalLengthFit()
		{ }

		public static FocalLengthFit FromReflectedObject(object reflObj)
		{
			var rv = new FocalLengthFit();

			rv.A = StarMap.GetPropValue<double>(reflObj, "A");
			rv.B = StarMap.GetPropValue<double>(reflObj, "B");
			rv.Variance = StarMap.GetPropValue<double>(reflObj, "Variance");

			return rv;
		}

		#endregion
	}
}
