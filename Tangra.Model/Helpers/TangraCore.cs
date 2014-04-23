using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Tangra.Model.Astro;
using Tangra.Model.Numerical;

namespace Tangra.Model.Helpers
{
	public static class TangraModelCore
	{
		internal const string LIBRARY_TANGRA_CORE = "TangraCore";

		[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
		//void SolveLinearSystem(double* a, long aRows, long aCols, double* x, long xRows, long xCols, double y)
		public static extern void SolveLinearSystem([In] double[] A, int aRows, int aCols, [In] double[] X, int xRows, int xCols, [In, Out] double[] Y);

		[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
		//void SolveLinearSystemFast(double* a, double* x, long numEquations, double* y);
		public static extern void SolveLinearSystemFast([In] double[] A, [In] double[] X, int numEquations, [In, Out] double[] Y);

		[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
		//void LinearSystemFastInitialiseSolution(long numVariables, long maxEquations);
		public static extern void LinearSystemFastInitialiseSolution(int numVariables, int maxEquations);

		[DllImport(LIBRARY_TANGRA_CORE, CallingConvention = CallingConvention.Cdecl)]
		//void DoNonLinearPfsFit(unsigned long* intensity, const long squareSize, const long saturation, bool* isSolved, double* iBackground, double* iStarMax, double* x0, double* y0, double* r0, double* residuals);
		public static extern void DoNonLinearPfsFit(
			[In] uint[] intensity, int squareSize, int saturation,
			[In, Out] ref bool isSolved, [In, Out] ref double iBackground, [In, Out] ref double iStarMax, [In, Out] ref double x0, [In, Out] ref double y0, [In, Out] ref double r0, [In, Out] double[] residuals);

		public static SafeMatrix SolveLinearSystem(SafeMatrix A, SafeMatrix X)
		{
			double[] a = A.GetElements();
			double[] x = X.GetElements();
			double[] y = new double[A.ColumnCount * X.ColumnCount];

			SolveLinearSystem(a, A.RowCount, A.ColumnCount, x, X.RowCount, X.ColumnCount, y);

			SafeMatrix rv = new SafeMatrix(X.RowCount, X.ColumnCount);
			rv.SetElements(y);

			return rv;
		}

		private static int s_NumVariables = -1;

		/// <summary>
		/// Warning: This method is not thread safe!
		/// </summary>
		public static SafeMatrix SolveLinearSystemFast(SafeMatrix A, SafeMatrix X)
		{
			if (A.RowCount > 35  * 35)
				throw new InvalidOperationException("Not a PSF fitting linear system.");

			if (s_NumVariables != A.ColumnCount)
			{
				LinearSystemFastInitialiseSolution(A.ColumnCount, 35*35);
				s_NumVariables = A.ColumnCount;
			}

			double[] a = A.GetElements();
			double[] x = X.GetElements();
			double[] y = new double[A.ColumnCount * X.ColumnCount];

			SolveLinearSystemFast(a, x, A.RowCount, y);

			SafeMatrix rv = new SafeMatrix(X.RowCount, X.ColumnCount);
			rv.SetElements(y);

			return rv;
		}

		public static void EnsureBuffers(int squareSize, int numVariables)
		{
			if (squareSize > 35)
				throw new InvalidOperationException("Not a PSF fitting linear system.");

			if (s_NumVariables != numVariables)
			{
				LinearSystemFastInitialiseSolution(numVariables, 35*35);
				s_NumVariables = numVariables;
			}
		}
	}
}
