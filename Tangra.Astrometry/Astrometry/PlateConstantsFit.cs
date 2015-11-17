/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Tangra.Astrometry.Recognition;
using Tangra.Model.Config;
using Tangra.Model.Helpers;
using Tangra.Model.Image;
using Tangra.Model.Numerical;

namespace Tangra.Astrometry
{
	public class FitInfo
	{
		private List<PlateConstStarPair> m_StarPairs;

		public FitInfo(List<PlateConstStarPair> starPairs)
		{
			m_StarPairs = starPairs;
		}

		public List<PlateConstStarPair> AllStarPairs
		{
			get { return m_StarPairs; }
		}

		public int NumberOfStarsUnsedInSolution()
		{
			int count = 0;
			foreach (PlateConstStarPair star in m_StarPairs)
				if (star.FitInfo.UsedInSolution) count++;
			return count;
		}

		public PlateConstStarPair GetFittedStar(ImagePixel starCenter)
		{
			return GetFittedStar(starCenter.X, starCenter.Y);
		}

		public PlateConstStarPair GetFittedStar(int x, int y)
		{
			foreach (PlateConstStarPair fittedStar in m_StarPairs)
			{
				if (Math.Abs(fittedStar.x - x) < 5 &&
					Math.Abs(fittedStar.y - y) < 5)
				{
					return fittedStar;
				}
			}

			return null;
		}

		public double FittedFocalLength { get; set; }
		public double DetectedLimitingMagnitude { get; set; }
	}

	public abstract class PlateConstantsFit
	{
		protected FitOrder m_FitOrder;
		protected FitInfo m_FitInfo;

		internal double Variance;
		internal double VarianceArcSecRA;
		internal double VarianceArcSecDE;

		public abstract void GetTangentCoordsFromImageCoords(double x, double y, out double xTang, out double yTang);
		public abstract void GetImageCoordsFromTangentCoords(double xTang, double yTang, out double x, out double y);

		public FitOrder FitOrder
		{
			get { return m_FitOrder; }
		}

		internal FitInfo FitInfo
		{
			get { return m_FitInfo; }
		}

		protected List<PlateConstStarPair> m_StarPairs;

		protected abstract void ConfigureObservation(SafeMatrix A, SafeMatrix AReverse, int i);
		protected abstract bool ReadSolvedConstants(SafeMatrix bx, SafeMatrix by);
		protected abstract void ReadSolvedReversedConstants(SafeMatrix bx, SafeMatrix by);

		public static PlateConstantsFit LeastSquareSolve(
			List<PlateConstStarPair> pairs,
			double ra0Deg,
			double de0Deg,
			FitOrder fitOrder,
			int minNumberOfStars)
		{
			if (fitOrder == FitOrder.Linear)
			{
				PlateConstantsLinearFit linearFit = new PlateConstantsLinearFit(pairs);
				if (linearFit.LeastSquareSolve(ra0Deg, de0Deg, minNumberOfStars))
					return linearFit;
			}

			if (fitOrder == FitOrder.Quadratic)
			{
				PlateConstantsQadraticFit qadraticFit = new PlateConstantsQadraticFit(pairs);
				if (qadraticFit.LeastSquareSolve(ra0Deg, de0Deg, minNumberOfStars))
					return qadraticFit;
			}

			return null;
		}

	    public static PlateConstantsFit ConstructLinearConstantsFit(
            double a, double b, double c, double d, double e, double f, 
            double a1, double b1, double c1, double d1, double e1, double f1)
	    {
	        PlateConstantsLinearFit linearFit = new PlateConstantsLinearFit(new List<PlateConstStarPair>());
	        linearFit.Const_A = a;
            linearFit.Const_B = b;
            linearFit.Const_C = c;
            linearFit.Const_D = d;
            linearFit.Const_E = e;
            linearFit.Const_F = f;
            linearFit.Const_A1 = a1;
            linearFit.Const_B1 = b1;
            linearFit.Const_C1 = c1;
            linearFit.Const_D1 = d1;
            linearFit.Const_E1 = e1;
            linearFit.Const_F1 = f1;
	        return linearFit;
	    }

		private bool LeastSquareSolve(double ra0Deg, double de0Deg, int minNumberOfStars)
		{
			int[] NUM_CONSTANTS = new int[] { 3, 6, 10 };

			SafeMatrix A = new SafeMatrix(m_StarPairs.Count, NUM_CONSTANTS[(int)m_FitOrder]);
			SafeMatrix X = new SafeMatrix(m_StarPairs.Count, 1);
			SafeMatrix Y = new SafeMatrix(m_StarPairs.Count, 1);

			SafeMatrix AReverse = new SafeMatrix(m_StarPairs.Count, NUM_CONSTANTS[(int)m_FitOrder]);
			SafeMatrix XReverse = new SafeMatrix(m_StarPairs.Count, 1);
			SafeMatrix YReverse = new SafeMatrix(m_StarPairs.Count, 1);

			int numStars = 0;

			for (int i = 0; i < m_StarPairs.Count; i++)
			{
				m_StarPairs[i].FitInfo.UsedInSolution = false;

				if (m_StarPairs[i].FitInfo.ExcludedForHighResidual) continue;

				numStars++;
				m_StarPairs[i].FitInfo.UsedInSolution = true;

				ConfigureObservation(A, AReverse, i);

				X[i, 0] = m_StarPairs[i].ExpectedXTang;
				Y[i, 0] = m_StarPairs[i].ExpectedYTang;
				XReverse[i, 0] = m_StarPairs[i].x;
				YReverse[i, 0] = m_StarPairs[i].y;
			}

			// Insufficient stars to solve the plate
			if (numStars < minNumberOfStars)
			{
                if (TangraConfig.Settings.TraceLevels.PlateSolving.TraceVerbose())
				    Debug.WriteLine(string.Format("Insufficient number of stars to do a fit. At least {0} stars requested.", minNumberOfStars));

				return false;
			}

			SafeMatrix a_T = A.Transpose();
			SafeMatrix aa = a_T * A;
			SafeMatrix aa_inv = aa.Inverse();
			SafeMatrix bx = (aa_inv * a_T) * X;
			SafeMatrix by = (aa_inv * a_T) * Y;

			if (ReadSolvedConstants(bx, by))
			{
				a_T = AReverse.Transpose();
				aa = a_T * AReverse;
				aa_inv = aa.Inverse();
				bx = (aa_inv * a_T) * XReverse;
				by = (aa_inv * a_T) * YReverse;

				ReadSolvedReversedConstants(bx, by);
			}

			double residualSum = 0;
			double residualSumArcSecRA = 0;
			double residualSumArcSecDE = 0;
			int numResiduals = 0;

			for (int i = 0; i < m_StarPairs.Count; i++)
			{
				double computedXTang, computedYTang;
				GetTangentCoordsFromImageCoords(m_StarPairs[i].x, m_StarPairs[i].y, out computedXTang, out computedYTang);

				m_StarPairs[i].FitInfo.ResidualXTang = m_StarPairs[i].ExpectedXTang - computedXTang;
				m_StarPairs[i].FitInfo.ResidualYTang = m_StarPairs[i].ExpectedYTang - computedYTang;

				double raComp, deComp;
				TangentPlane.TangentToCelestial(computedXTang, computedYTang, ra0Deg, de0Deg, out raComp, out deComp);

				m_StarPairs[i].FitInfo.ResidualRAArcSec = 3600.0 * AngleUtility.Elongation(m_StarPairs[i].RADeg, 0, raComp, 0);
				m_StarPairs[i].FitInfo.ResidualDEArcSec = 3600.0 * AngleUtility.Elongation(0, m_StarPairs[i].DEDeg, 0, deComp);

				if (!m_StarPairs[i].FitInfo.UsedInSolution) continue;
				numResiduals++;
				residualSum += Math.Abs(m_StarPairs[i].FitInfo.ResidualXTang * m_StarPairs[i].FitInfo.ResidualYTang);
				residualSumArcSecRA += m_StarPairs[i].FitInfo.ResidualRAArcSec * m_StarPairs[i].FitInfo.ResidualRAArcSec;
				residualSumArcSecDE += m_StarPairs[i].FitInfo.ResidualDEArcSec * m_StarPairs[i].FitInfo.ResidualDEArcSec;
			}

			Variance = residualSum / (numResiduals - 1);
			VarianceArcSecRA = residualSumArcSecRA / (numResiduals - 1);
			VarianceArcSecDE = residualSumArcSecDE / (numResiduals - 1);



			return true;
		}

		public static PlateConstantsFit FromReflectedObject(object reflObj)
		{
			string constType = reflObj.GetType().ToString();
			
			if (constType.Contains("PlateConstantsLinearFit"))
				return PlateConstantsLinearFit.FromReflectedLinearFit(reflObj);
			else if (constType.Contains("PlateConstantsQadraticFit"))
				return PlateConstantsQadraticFit.FromReflectedQadraticFit(reflObj);
			else
				return null;
		}
	}
}
