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
using Tangra.Model.Numerical;

namespace Tangra.Astrometry
{
	[Serializable]
	internal class PlateConstantsLinearFit : PlateConstantsFit, ISerializable
	{
		internal double Const_A;
		internal double Const_B;
		internal double Const_C;
		internal double Const_D;
		internal double Const_E;
		internal double Const_F;

		internal double Const_A1;
		internal double Const_B1;
		internal double Const_C1;
		internal double Const_D1;
		internal double Const_E1;
		internal double Const_F1;

		internal PlateConstantsLinearFit(List<PlateConstStarPair> starPairs)
		{
			m_StarPairs = starPairs;
			m_FitOrder = FitOrder.Linear;
			m_FitInfo = new FitInfo(starPairs);
		}

		protected override void ConfigureObservation(SafeMatrix A, SafeMatrix AReverse, int i)
		{
			A[i, 0] = m_StarPairs[i].x;
			A[i, 1] = m_StarPairs[i].y;
			A[i, 2] = 1;

			AReverse[i, 0] = m_StarPairs[i].ExpectedXTang;
			AReverse[i, 1] = m_StarPairs[i].ExpectedYTang;
			AReverse[i, 2] = 1;
		}

		protected override bool ReadSolvedConstants(SafeMatrix bx, SafeMatrix by)
		{
			Const_A = bx[0, 0];
			Const_B = bx[1, 0];
			Const_C = bx[2, 0];

			Const_D = by[0, 0];
			Const_E = by[1, 0];
			Const_F = by[2, 0];

			Const_A1 = Const_E / (Const_E * Const_A - Const_B * Const_D);
			Const_B1 = -Const_B / (Const_E * Const_A - Const_B * Const_D);
			Const_C1 = (Const_B * Const_F - Const_C * Const_E) / (Const_E * Const_A - Const_B * Const_D);
			Const_D1 = Const_D / (Const_B * Const_D - Const_A * Const_E);
			Const_E1 = -Const_A / (Const_B * Const_D - Const_A * Const_E);
			Const_F1 = (Const_A * Const_F - Const_C * Const_D) / (Const_B * Const_D - Const_A * Const_E);

			return false;
		}

		protected override void ReadSolvedReversedConstants(SafeMatrix bx, SafeMatrix by)
		{ }

		public override void GetTangentCoordsFromImageCoords(double x, double y, out double xTang, out double yTang)
		{
			xTang = Const_A * x + Const_B * y + Const_C;
			yTang = Const_D * x + Const_E * y + Const_F;
		}

		public override void GetImageCoordsFromTangentCoords(double xTang, double yTang, out double x, out double y)
		{
			x = Const_A1 * xTang + Const_B1 * yTang + Const_C1;
			y = Const_D1 * xTang + Const_E1 * yTang + Const_F1;
		}

		#region ISerializable Members

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("m_FitOrder", m_FitOrder);

			info.AddValue("Const_A", Const_A);
			info.AddValue("Const_B", Const_B);
			info.AddValue("Const_C", Const_C);
			info.AddValue("Const_D", Const_D);
			info.AddValue("Const_E", Const_E);
			info.AddValue("Const_F", Const_F);

			info.AddValue("Const_A1", Const_A1);
			info.AddValue("Const_B1", Const_B1);
			info.AddValue("Const_C1", Const_C1);
			info.AddValue("Const_D1", Const_D1);
			info.AddValue("Const_E1", Const_E1);
			info.AddValue("Const_F1", Const_F1);

			info.AddValue("FittedFocalLength", this.FitInfo != null ? this.FitInfo.FittedFocalLength : -1);
		}

		public PlateConstantsLinearFit(SerializationInfo info, StreamingContext context)
		{
			m_FitOrder = (FitOrder)info.GetInt32("m_FitOrder");

			Const_A = info.GetDouble("Const_A");
			Const_B = info.GetDouble("Const_B");
			Const_C = info.GetDouble("Const_C");
			Const_D = info.GetDouble("Const_D");
			Const_E = info.GetDouble("Const_E");
			Const_F = info.GetDouble("Const_F");

			Const_A1 = info.GetDouble("Const_A1");
			Const_B1 = info.GetDouble("Const_B1");
			Const_C1 = info.GetDouble("Const_C1");
			Const_D1 = info.GetDouble("Const_D1");
			Const_E1 = info.GetDouble("Const_E1");
			Const_F1 = info.GetDouble("Const_F1");

			this.m_FitInfo = new FitInfo(new List<PlateConstStarPair>());
			this.m_FitInfo.FittedFocalLength = info.GetDouble("FittedFocalLength");
		}

		private PlateConstantsLinearFit()
		{ }

		public static PlateConstantsLinearFit FromReflectedLinearFit(object reflObj)
		{
			var rv = new PlateConstantsLinearFit();

			rv.Const_A = StarMap.GetPropValue<double>(reflObj, "Const_A");
			rv.Const_B = StarMap.GetPropValue<double>(reflObj, "Const_B");
			rv.Const_C = StarMap.GetPropValue<double>(reflObj, "Const_C");
			rv.Const_D = StarMap.GetPropValue<double>(reflObj, "Const_D");
			rv.Const_E = StarMap.GetPropValue<double>(reflObj, "Const_E");
			rv.Const_F = StarMap.GetPropValue<double>(reflObj, "Const_F");

			rv.Const_A1 = StarMap.GetPropValue<double>(reflObj, "Const_A1");
			rv.Const_B1 = StarMap.GetPropValue<double>(reflObj, "Const_B1");
			rv.Const_C1 = StarMap.GetPropValue<double>(reflObj, "Const_C1");
			rv.Const_D1 = StarMap.GetPropValue<double>(reflObj, "Const_D1");
			rv.Const_E1 = StarMap.GetPropValue<double>(reflObj, "Const_E1");
			rv.Const_F1 = StarMap.GetPropValue<double>(reflObj, "Const_F1");

			rv.m_FitInfo = new FitInfo(new List<PlateConstStarPair>());
			object fitInfo = StarMap.GetPropValue<object>(reflObj, "m_FitInfo");
			rv.m_FitInfo.FittedFocalLength = StarMap.GetPropValue<double>(fitInfo, "FittedFocalLength");

			return rv;
		}

		#endregion
	}

}
