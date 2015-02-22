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
	internal class PlateConstantsQadraticFit : PlateConstantsFit, ISerializable
	{
		// xExpected = A * x * x + B * y * y + C * x * y + D * x + E * y + F
		// yExpected = G * x * x + H * y * y + K * x * y + L * x + M * y + N

		internal double Const_A;
		internal double Const_B;
		internal double Const_C;
		internal double Const_D;
		internal double Const_E;
		internal double Const_F;
		internal double Const_G;
		internal double Const_H;
		internal double Const_K;
		internal double Const_L;
		internal double Const_M;
		internal double Const_N;


		internal double Const_A1;
		internal double Const_B1;
		internal double Const_C1;
		internal double Const_D1;
		internal double Const_E1;
		internal double Const_F1;
		internal double Const_G1;
		internal double Const_H1;
		internal double Const_K1;
		internal double Const_L1;
		internal double Const_M1;
		internal double Const_N1;

		internal PlateConstantsQadraticFit(List<PlateConstStarPair> starPairs)
		{
			m_StarPairs = starPairs;
			m_FitOrder = FitOrder.Quadratic;
			m_FitInfo = new FitInfo(starPairs);
		}

		#region ISerializable Members

		public PlateConstantsQadraticFit(SerializationInfo info, StreamingContext context)
		{
			m_FitOrder = (FitOrder)info.GetInt32("m_FitOrder");

			Const_A = info.GetDouble("A");
			Const_B = info.GetDouble("B");
			Const_C = info.GetDouble("C");
			Const_D = info.GetDouble("D");
			Const_E = info.GetDouble("E");
			Const_F = info.GetDouble("F");
			Const_G = info.GetDouble("G");
			Const_H = info.GetDouble("H");
			Const_K = info.GetDouble("K");
			Const_L = info.GetDouble("L");
			Const_M = info.GetDouble("M");
			Const_N = info.GetDouble("N");

			Const_A1 = info.GetDouble("A1");
			Const_B1 = info.GetDouble("B1");
			Const_C1 = info.GetDouble("C1");
			Const_D1 = info.GetDouble("D1");
			Const_E1 = info.GetDouble("E1");
			Const_F1 = info.GetDouble("F1");
			Const_G1 = info.GetDouble("G1");
			Const_H1 = info.GetDouble("H1");
			Const_K1 = info.GetDouble("K1");
			Const_L1 = info.GetDouble("L1");
			Const_M1 = info.GetDouble("M1");
			Const_N1 = info.GetDouble("N1");

			this.m_FitInfo = new FitInfo(new List<PlateConstStarPair>());
			this.m_FitInfo.FittedFocalLength = info.GetDouble("FittedFocalLength");			
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("m_FitOrder", m_FitOrder);

			info.AddValue("A", Const_A);
			info.AddValue("B", Const_B);
			info.AddValue("C", Const_C);
			info.AddValue("D", Const_D);
			info.AddValue("E", Const_E);
			info.AddValue("F", Const_F);
			info.AddValue("G", Const_G);
			info.AddValue("H", Const_H);
			info.AddValue("K", Const_K);
			info.AddValue("L", Const_L);
			info.AddValue("M", Const_M);
			info.AddValue("N", Const_N);

			info.AddValue("A1", Const_A1);
			info.AddValue("B1", Const_B1);
			info.AddValue("C1", Const_C1);
			info.AddValue("D1", Const_D1);
			info.AddValue("E1", Const_E1);
			info.AddValue("F1", Const_F1);
			info.AddValue("G1", Const_G1);
			info.AddValue("H1", Const_H1);
			info.AddValue("K1", Const_K1);
			info.AddValue("L1", Const_L1);
			info.AddValue("M1", Const_M1);
			info.AddValue("N1", Const_N1);

			info.AddValue("FittedFocalLength", this.FitInfo != null ? this.FitInfo.FittedFocalLength : -1);
		}

		private PlateConstantsQadraticFit()
		{ }

		public static PlateConstantsQadraticFit FromReflectedQadraticFit(object reflObj)
		{
			var rv = new PlateConstantsQadraticFit();

			rv.Const_A = StarMap.GetPropValue<double>(reflObj, "Const_A");
			rv.Const_B = StarMap.GetPropValue<double>(reflObj, "Const_B");
			rv.Const_C = StarMap.GetPropValue<double>(reflObj, "Const_C");
			rv.Const_D = StarMap.GetPropValue<double>(reflObj, "Const_D");
			rv.Const_E = StarMap.GetPropValue<double>(reflObj, "Const_E");
			rv.Const_F = StarMap.GetPropValue<double>(reflObj, "Const_F");
			rv.Const_G = StarMap.GetPropValue<double>(reflObj, "Const_G");
			rv.Const_H = StarMap.GetPropValue<double>(reflObj, "Const_H");
			rv.Const_K = StarMap.GetPropValue<double>(reflObj, "Const_K");
			rv.Const_L = StarMap.GetPropValue<double>(reflObj, "Const_L");
			rv.Const_M = StarMap.GetPropValue<double>(reflObj, "Const_M");
			rv.Const_N = StarMap.GetPropValue<double>(reflObj, "Const_N");

			rv.Const_A1 = StarMap.GetPropValue<double>(reflObj, "Const_A1");
			rv.Const_B1 = StarMap.GetPropValue<double>(reflObj, "Const_B1");
			rv.Const_C1 = StarMap.GetPropValue<double>(reflObj, "Const_C1");
			rv.Const_D1 = StarMap.GetPropValue<double>(reflObj, "Const_D1");
			rv.Const_E1 = StarMap.GetPropValue<double>(reflObj, "Const_E1");
			rv.Const_F1 = StarMap.GetPropValue<double>(reflObj, "Const_F1");
			rv.Const_G1 = StarMap.GetPropValue<double>(reflObj, "Const_G1");
			rv.Const_H1 = StarMap.GetPropValue<double>(reflObj, "Const_H1");
			rv.Const_K1 = StarMap.GetPropValue<double>(reflObj, "Const_K1");
			rv.Const_L1 = StarMap.GetPropValue<double>(reflObj, "Const_L1");
			rv.Const_M1 = StarMap.GetPropValue<double>(reflObj, "Const_M1");
			rv.Const_N1 = StarMap.GetPropValue<double>(reflObj, "Const_N1");

			rv.m_FitInfo = new FitInfo(new List<PlateConstStarPair>());
			object fitInfo = StarMap.GetPropValue<object>(reflObj, "m_FitInfo");
			rv.m_FitInfo.FittedFocalLength = StarMap.GetPropValue<double>(fitInfo, "FittedFocalLength");

			return rv;
		}

		#endregion

		public override void GetTangentCoordsFromImageCoords(double x, double y, out double xTang, out double yTang)
		{
			// xExpected = A * x * x + B * y * y + C * x * y + D * x + E * y + F
			// yExpected = G * x * x + H * y * y + K * x * y + L * x + M * y + N

			xTang = Const_A * x * x + Const_B * y * y + Const_C * x * y + Const_D * x + Const_E * y + Const_F;
			yTang = Const_G * x * x + Const_H * y * y + Const_K * x * y + Const_L * x + Const_M * y + Const_N;
		}

		public override void GetImageCoordsFromTangentCoords(double xTang, double yTang, out double x, out double y)
		{
			x = Const_A1 * xTang * xTang + Const_B1 * yTang * yTang + Const_C1 * xTang * yTang + Const_D1 * xTang + Const_E1 * yTang + Const_F1;
			y = Const_G1 * xTang * xTang + Const_H1 * yTang * yTang + Const_K1 * xTang * yTang + Const_L1 * xTang + Const_M1 * yTang + Const_N1;
		}


		protected override void ConfigureObservation(SafeMatrix A, SafeMatrix AReverse, int i)
		{
			A[i, 0] = m_StarPairs[i].x * m_StarPairs[i].x;
			A[i, 1] = m_StarPairs[i].y * m_StarPairs[i].y;
			A[i, 2] = m_StarPairs[i].x * m_StarPairs[i].y;
			A[i, 3] = m_StarPairs[i].x;
			A[i, 4] = m_StarPairs[i].y;
			A[i, 5] = 1;

			AReverse[i, 0] = m_StarPairs[i].ExpectedXTang * m_StarPairs[i].ExpectedXTang;
			AReverse[i, 1] = m_StarPairs[i].ExpectedYTang * m_StarPairs[i].ExpectedYTang;
			AReverse[i, 2] = m_StarPairs[i].ExpectedXTang * m_StarPairs[i].ExpectedYTang;
			AReverse[i, 3] = m_StarPairs[i].ExpectedXTang;
			AReverse[i, 4] = m_StarPairs[i].ExpectedYTang;
			AReverse[i, 5] = 1;
		}

		protected override bool ReadSolvedConstants(SafeMatrix bx, SafeMatrix by)
		{
			Const_A = bx[0, 0];
			Const_B = bx[1, 0];
			Const_C = bx[2, 0];
			Const_D = bx[3, 0];
			Const_E = bx[4, 0];
			Const_F = bx[5, 0];

			Const_G = by[0, 0];
			Const_H = by[1, 0];
			Const_K = by[2, 0];
			Const_L = by[3, 0];
			Const_M = by[4, 0];
			Const_N = by[5, 0];

			return true;
		}

		protected override void ReadSolvedReversedConstants(SafeMatrix bx, SafeMatrix by)
		{
			Const_A1 = bx[0, 0];
			Const_B1 = bx[1, 0];
			Const_C1 = bx[2, 0];
			Const_D1 = bx[3, 0];
			Const_E1 = bx[4, 0];
			Const_F1 = bx[5, 0];

			Const_G1 = by[0, 0];
			Const_H1 = by[1, 0];
			Const_K1 = by[2, 0];
			Const_L1 = by[3, 0];
			Const_M1 = by[4, 0];
			Const_N1 = by[5, 0];
		}
	}
}
