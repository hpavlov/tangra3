/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#include "safe_matrix.h"
#include "stdlib.h"
#include "stdio.h"
#include "math.h"
#include <string.h>
#include <cmath>

SafeMatrix::SafeMatrix(long rows, long columns)	
{
	Elements = (double*)malloc(sizeof(double) * rows * columns);
	m_ElementsBufferAllocated = true;
	
	ColumnCount = columns;
	RowCount = rows;
	MatrixSize = columns * rows;
}

SafeMatrix::SafeMatrix(double* values, long rows, long columns)
{
	Elements = values;
	m_ElementsBufferAllocated = false;
	ColumnCount = columns;
	RowCount = rows;
	MatrixSize = columns * rows;
}

SafeMatrix::~SafeMatrix()
{
	if (m_ElementsBufferAllocated)
		delete Elements;

	Elements = NULL;
}

double SafeMatrix::GetValueAt(long row, long col)
{
	return *(Elements + col * RowCount + row);
}

void SafeMatrix::SetValueAt(long row, long col, double value)
{
	*(Elements + col * RowCount + row) = value;
}

bool SafeMatrix::IsSquare()
{
	return RowCount == ColumnCount;
}

static double Dot(const SafeMatrix& v, const int vRow, const SafeMatrix& w, const int wCol)
{
	if (v.ColumnCount != w.RowCount)
		return 0;

	double buf = 0;

	for (int i = 0; i < v.ColumnCount; i++) {
		buf += v.Elements[vRow * v.ColumnCount + i] * w.Elements[i * w.ColumnCount + wCol];
	}

	return buf;
}

static SafeMatrix* Cross(const SafeMatrix& A, const SafeMatrix& B)
{
	if (A.ColumnCount != B.RowCount)
		return NULL;

	SafeMatrix* C = new SafeMatrix(A.RowCount, B.ColumnCount);

	for (int i = 0; i < A.RowCount; i++) {
		for (int j = 0; j < B.ColumnCount; j++) {
			C->Elements[i * C->ColumnCount + j] = Dot(A, i, B, j);
		}
	}

	return C;
}

SafeMatrix* SafeMatrix::Transpose()
{
	SafeMatrix* M = new SafeMatrix(ColumnCount, RowCount);

	for (int i = 0; i < ColumnCount; i++) {
		for (int j = 0; j < RowCount; j++) {
			M->Elements[i * M->ColumnCount + j] = Elements[j * ColumnCount + i];
		}
	}

	return M;
}

void SafeMatrix::TransposeInBuffer(double* transponseBuffer)
{
	for (int i = 0; i < ColumnCount; i++) {
		for (int j = 0; j < RowCount; j++) {
			transponseBuffer[i * RowCount + j] = Elements[j * ColumnCount + i];
		}
	}
}

SafeMatrix* SafeMatrix::Clone()
{
	SafeMatrix* A = new SafeMatrix(RowCount, ColumnCount);
	//memccpy(&A->Elements[0], Elements[0], RowCount * ColumnCount, sizeof(double));
	for (int i = 0; i < MatrixSize; i++) A->Elements[i] = Elements[i];

	return A;
}

double SafeMatrix::DiagProd()
{
	double buf = 1;
	int dim = RowCount < ColumnCount ? RowCount : ColumnCount;

	for (int i = 0; i < dim; i++) {
		buf *= Elements[i * ColumnCount + i];
	}

	return buf;
}

double SafeMatrix::Determinant()
{
	if (!IsSquare())
		return 0;

	if (ColumnCount == 1)
		return GetValueAt(0, 0);

	// perform LU-decomposition & return product of diagonal elements of U
	SafeMatrix* X = Clone();

	if (!X->LU())
	{
		delete X;
		return 0;
	}
	else
	{
		double diagProd = X->DiagProd();
		delete X;

		return diagProd;		
	}
}

bool SafeMatrix::LU()
{
	if (!IsSquare())
		return false;

	int p_row = 0;
	int p_col = 0;
	int n = ColumnCount;

	int p_k = 0;

	for (int k = 0; k < n; p_k += n, k++) {
		for (int j = k; j < n; j++) {
			p_col = 0;

			for (int p = 0; p < k; p_col += n, p++)
				Elements[p_k + j] -= Elements[p_k + p] * Elements[p_col + j];
		}

		if (Elements[p_k + k] == 0.0)
			return false;

		p_row = p_k + n;

		for (int i = k + 1; i < n; p_row += n, i++) {
			p_col = 0;

			for (int p = 0; p < k; p_col += n, p++)
				Elements[p_row + k] -= Elements[p_row + p] * Elements[p_col + k];

			Elements[p_row + k] /= Elements[p_k + k];
		}
	}

	return true;
}

SafeMatrix* SafeMatrix::Minor(int row, int col)
{
	SafeMatrix* buf = new SafeMatrix(RowCount - 1, ColumnCount - 1);
	InPlaceMinor(buf, row, col);
	return buf;
}

void SafeMatrix::InPlaceMinor(SafeMatrix* minor, int row, int col)
{
	// THIS IS THE LOW-LEVEL SOLUTION ~ O(n^2)

	int r = 0;
	int c = 0;

	for (int i = 0; i < RowCount; i++) {
		if (i != row) {
			for (int j = 0; j < ColumnCount; j++) {
				if (j != col) {
					minor->Elements[r * minor->ColumnCount + c] = Elements[i * ColumnCount + j];
					c++;
				}
			}

			c = 0;
			r++;
		}
	}
}

SafeMatrix* SafeMatrix::Inverse()
{
	if (!IsSquare())
		return NULL;

	double det = Determinant();

	
	if (det == 0 || det != det /*this translates to is not NaN*/)
		return NULL;

	SafeMatrix* buf = new SafeMatrix(ColumnCount, ColumnCount);
	SafeMatrix* minor = new SafeMatrix(ColumnCount - 1, ColumnCount - 1);
	for (int i = 0; i < ColumnCount; i++) {
		for (int j = 0; j < ColumnCount; j++) {
			InPlaceMinor(minor, j, i);
			buf->Elements[i * buf->ColumnCount + j] = (pow(-1, i + j) * minor->Determinant()) / det;
		}
	}

	delete minor;

	return buf;
}


void SolveLinearSystem(double* a, long aRows, long aCols, double* x, long xRows, long xCols, double* y)
{
	//SafeMatrix a_T = A.Transpose();
	//SafeMatrix aa = a_T*A;
	//SafeMatrix aa_inv = aa.Inverse();
	//SafeMatrix Y = (aa_inv*a_T)*X;		
	
	SafeMatrix A(a, aRows, aCols);
	SafeMatrix X(x, xRows, xCols);
	SafeMatrix* A_T = A.Transpose();
	SafeMatrix* aa = Cross(*A_T, A);
	
	if (aa != NULL)
	{
		SafeMatrix* aa_inv = aa->Inverse();
	
		if (aa_inv != NULL)
		{
			SafeMatrix* y1 = Cross(*aa_inv, *A_T);
			
			if (y1 != NULL)
			{
				SafeMatrix* y2 = Cross(*y1, X);		
				
				if (y2 != NULL)
				{
					memcpy(y, y2->Elements, y2->ColumnCount * y2->RowCount * sizeof(double));
					
					delete y2;
				}
				
				delete y1;
			}
			
			delete aa_inv;
		}

		delete aa;	
	}
	
	delete A_T;
}