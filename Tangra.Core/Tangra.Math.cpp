#include "Tangra.Math.h"
#include "safe_matrix.h"
#include "stdlib.h"
#include "stdio.h"
#include "math.h"
#include <string.h>
#include <cmath>

unsigned long SATURATION_8BIT = 250;
unsigned long SATURATION_12BIT = 4000;
unsigned long SATURATION_14BIT = 16000;
unsigned long SATURATION_16BIT = 65000;

static double DotBuffer(double* bufferV, const int vRow, double* bufferW, const int wCol, const long vColsCount, const long wRowsCount, const long wColsCount)
{
	if (vColsCount != wRowsCount)
		return 0;

	double buf = 0;

	for (int i = 0; i < vColsCount; i++) {
		buf += bufferV[vRow * vColsCount + i] * bufferW[i * wColsCount + wCol];
	}

	return buf;
}

static bool CrossInBuffer(double* bufferA, double* bufferB, double* bufferC, const long aRowsCount, const long aColsCount, const long bRowsCount, const long bColsCount)
{
	if (aColsCount != bRowsCount)
		return false;

	for (int i = 0; i < aRowsCount; i++) {
		for (int j = 0; j < bColsCount; j++) {
			bufferC[i * bColsCount + j] = DotBuffer(bufferA, i, bufferB, j, aColsCount, bRowsCount, bColsCount);
		}
	}
	
	return true;
}

static void TransposeInBuffer(double* buffer, const int rows, const int cols, double* transponseBuffer)
{
	for (int i = 0; i < cols; i++) {
		for (int j = 0; j < rows; j++) {
			transponseBuffer[i * rows + j] = buffer[j * cols + i];
		}
	}
}

static double DiagProdBuffer(double* buffer, const int rows, const int cols)
{
	double buf = 1;
	int dim = rows < cols ? rows : cols;

	for (int i = 0; i < dim; i++) {
		buf *= buffer[i * cols + i];
	}

	return buf;
}

static bool LUBuffer(double* buffer, const int rows, const int cols)
{
	if (rows != cols)
		return false;

	int p_row = 0;
	int p_col = 0;
	int n = cols;

	int p_k = 0;

	for (int k = 0; k < n; p_k += n, k++) {
		for (int j = k; j < n; j++) {
			p_col = 0;

			for (int p = 0; p < k; p_col += n, p++)
				buffer[p_k + j] -= buffer[p_k + p] * buffer[p_col + j];
		}

		if (buffer[p_k + k] == 0.0)
			return false;

		p_row = p_k + n;

		for (int i = k + 1; i < n; p_row += n, i++) {
			p_col = 0;

			for (int p = 0; p < k; p_col += n, p++)
				buffer[p_row + k] -= buffer[p_row + p] * buffer[p_col + k];

			buffer[p_row + k] /= buffer[p_k + k];
		}
	}

	return true;
}

static double DeterminantOfBuffer(double* buffer, const int rows, const int cols)
{
	if (rows != cols)
		return 0;

	if (cols == 1)
		return *buffer;

	if (!LUBuffer(buffer, rows, cols))
	{
		return 0;
	}
	else
	{
		double diagProd = DiagProdBuffer(buffer, rows, cols);
		return diagProd;		
	}
}

static void MinorInBuffer(double* buffer, double* minorBuffer, const int squareSide, int row, int col)
{
	// THIS IS THE LOW-LEVEL SOLUTION ~ O(n^2)

	int r = 0;
	int c = 0;

	for (int i = 0; i < squareSide; i++) {
		if (i != row) {
			for (int j = 0; j < squareSide; j++) {
				if (j != col) {
					minorBuffer[r * (squareSide - 1) + c] = buffer[i * squareSide + j];
					c++;
				}
			}

			c = 0;
			r++;
		}
	}
}

static bool InverseInBuffer(double* buffer, double* bufferCopy, double* minorBuffer, const int squareSide)
{
	memcpy(bufferCopy, buffer, squareSide * squareSide * sizeof(double));
	
	double det = DeterminantOfBuffer(buffer, squareSide, squareSide);
	
	if (det == 0 || det != det )
		return false;
	
	for (int i = 0; i < squareSide; i++) 
	{
		for (int j = 0; j < squareSide; j++) 
		{
			MinorInBuffer(bufferCopy, minorBuffer, squareSide, j, i);
			double detMinor = DeterminantOfBuffer(minorBuffer, squareSide - 1, squareSide - 1);
			buffer[i * squareSide + j] = (pow(-1, i + j) * detMinor) / det;
		}
	}

	return true;
}

double* s_TransponseBuffer = NULL;
double* m_IntermediateAABuffer = NULL;
double* m_IntermediateAA2Buffer = NULL;
double* m_IntermediateY1Buffer = NULL;
double* m_MinorBuffer = NULL;
double* m_PsfBuffers = NULL;
double* m_PsfMatrixA = NULL;
double* m_PsfMatrixX = NULL;
double* m_PsfMatrixY = NULL;
long s_NumVariables;
long s_MaxEquations;
long s_FullWidth;


void EnsureLinearSystemSolutionBuffers(long numVariables)
{
	if (numVariables == s_NumVariables && NULL != s_TransponseBuffer)
		return;
		
	LinearSystemFastInitialiseSolution(numVariables, 35 * 35);
}
		
void LinearSystemFastInitialiseSolution(long numVariables, long maxEquations)
{
	s_NumVariables = numVariables;
	s_MaxEquations = maxEquations;
	s_FullWidth = 1 + (long)sqrt(maxEquations);
	
	if (NULL != s_TransponseBuffer)
	{
		delete s_TransponseBuffer;
		s_TransponseBuffer = NULL;
	}
	
	if (NULL != m_IntermediateAABuffer)
	{
		delete m_IntermediateAABuffer;
		m_IntermediateAABuffer = NULL;
	}
	
	if (NULL != m_IntermediateAA2Buffer)
	{
		delete m_IntermediateAA2Buffer;
		m_IntermediateAA2Buffer = NULL;
	}
	
	if (NULL != m_MinorBuffer)
	{
		delete m_MinorBuffer;
		m_MinorBuffer = NULL;
	}
	
	if (NULL != m_IntermediateY1Buffer)
	{
		delete m_IntermediateY1Buffer;
		m_IntermediateY1Buffer = NULL;
	}
	
	if (NULL != m_PsfBuffers)
	{
		delete m_PsfBuffers;
		m_PsfBuffers = NULL;
	}
	
	if (NULL != m_PsfMatrixA)
	{
		delete m_PsfMatrixA;
		m_PsfMatrixA = NULL;	
	}

	if (NULL != m_PsfMatrixX)
	{
		delete m_PsfMatrixX;
		m_PsfMatrixX = NULL;	
	}
	
	if (NULL != m_PsfMatrixY)
	{
		delete m_PsfMatrixY;
		m_PsfMatrixY = NULL;	
	}
	
	s_TransponseBuffer = (double*)malloc(numVariables * maxEquations * sizeof(double));
	m_IntermediateAABuffer = (double*)malloc(numVariables * numVariables * sizeof(double));
	m_IntermediateAA2Buffer = (double*)malloc(numVariables * numVariables * sizeof(double));
	m_IntermediateY1Buffer = (double*)malloc(numVariables * maxEquations * sizeof(double));
	m_MinorBuffer = (double*)malloc((numVariables - 1) * (maxEquations - 1) * sizeof(double));
	m_PsfBuffers = (double*)malloc(numVariables * s_FullWidth * sizeof(double));
	m_PsfMatrixA = (double*)malloc(numVariables * maxEquations * sizeof(double));
	m_PsfMatrixX = (double*)malloc(maxEquations * sizeof(double));
	m_PsfMatrixY = (double*)malloc(numVariables * sizeof(double));
}

void SolveLinearSystemFast(double* a, double* x, long numEquations, double* y)
{
	TransposeInBuffer(a, numEquations, s_NumVariables, s_TransponseBuffer);
	
	if (!CrossInBuffer(s_TransponseBuffer, a, m_IntermediateAABuffer, s_NumVariables, numEquations, numEquations, s_NumVariables))
		return;	
		
	if (!InverseInBuffer(m_IntermediateAABuffer, m_IntermediateAA2Buffer, m_MinorBuffer, s_NumVariables))
		return;
		
	if (!CrossInBuffer(m_IntermediateAABuffer, s_TransponseBuffer, m_IntermediateY1Buffer, s_NumVariables, s_NumVariables, s_NumVariables, numEquations))
		return;

	if (!CrossInBuffer(m_IntermediateY1Buffer, x, y, s_NumVariables, numEquations, numEquations, 1))
		return;
}

#define NUMBER_ITERATIONS 10

void DoNonLinearPfsFit(
	unsigned long* intensity, const long squareSize, const long saturation, 
	bool* isSolved, double*	iBackground, double* iStarMax, double*	x0, double*	y0, double*	r0, double* residuals)
{
	*isSolved = false;

	long full_width = (int) squareSize;
	
	long half_width = full_width/2;

	long nonSaturatedPixels = 0;

	*iBackground = 0;
	*r0 = 4.0;
	*iStarMax = 0;

	double found_x = half_width;
	double found_y = half_width;
	
	for (long iter = NUMBER_ITERATIONS; iter > 0; iter--)
	{
		if (iter == NUMBER_ITERATIONS)
		{
			unsigned long zval = 0;

			*iBackground = 0.0; // assume no backgnd intensity at first... 
			for (long i = 0; i < full_width; i++)
			{
				for (long j = 0; j < full_width; j++)
				{
					if (intensity[i + full_width * j] > zval) zval = intensity[i + full_width * j];
					if (intensity[i + full_width * j] < saturation) nonSaturatedPixels++;
				}
			}
			*iStarMax = (double) zval - *iBackground;
		}

		double* dx = m_PsfBuffers;
		double* dy = m_PsfBuffers + s_FullWidth;
		double* fx = m_PsfBuffers + 2 * s_FullWidth;
		double* fy = m_PsfBuffers + 3 * s_FullWidth;

		double r0_squared = (*r0) * (*r0);

		for (long i = 0; i < full_width; i++)
		{
			dx[i] = (double) i - found_x;
			fx[i] = exp(-dx[i]*dx[i]/r0_squared);
			dy[i] = (double) i - found_y;
			fy[i] = exp(-dy[i]*dy[i]/r0_squared);
		}

		int index = -1;
		for (long i = 0; i < full_width; i++)
		{
			for (long j = 0; j < full_width; j++)
			{
				unsigned long zval = intensity[i + full_width * j];

				if (zval < saturation)
				{
					index++;

					double exp_val = fx[i]*fy[j];

					double residual = (*iBackground) + (*iStarMax) * exp_val - (double) zval;
					m_PsfMatrixX[index] = -residual;

					m_PsfMatrixA[index * 5 + 0] = 1.0; // slope in i0 
					m_PsfMatrixA[index * 5 + 1] = exp_val; // slope in a0 
					m_PsfMatrixA[index * 5 + 2] = 2.0 * ((*iStarMax) * dx[i] * exp_val) /r0_squared;
					m_PsfMatrixA[index * 5 + 3] = 2.0 * ((*iStarMax) * dy[j] * exp_val) /r0_squared;
					m_PsfMatrixA[index * 5 + 4] = 2.0 * ((*iStarMax) *(dx[i]*dx[i] + dy[j]*dy[j]) * exp_val) / ((*r0) * r0_squared);
				}
			}
		}

		SolveLinearSystemFast(m_PsfMatrixA, m_PsfMatrixX, nonSaturatedPixels, m_PsfMatrixY);

		// we need at least 6 unsaturated pixels to solve 5 params
		if (nonSaturatedPixels > 6) // Request all pixels to be good!
		{
			*iBackground += m_PsfMatrixY[0];
			*iStarMax += m_PsfMatrixY[1];

			for (int i = 2; i < 4; i++)
			{
				if (m_PsfMatrixY[i] > 1.0) m_PsfMatrixY[i] = 1.0;
				if (m_PsfMatrixY[i] < -1.0) m_PsfMatrixY[i] = -1.0;
			}

			found_x += m_PsfMatrixY[2];
			found_y += m_PsfMatrixY[3];

			if (m_PsfMatrixY[4] > (*r0)/10.0) 
				m_PsfMatrixY[4] = (*r0)/10.0;
				
			if (m_PsfMatrixY[4] < -(*r0)/10.0) 
				m_PsfMatrixY[4] = -(*r0)/10.0;

			*r0 += m_PsfMatrixY[4];
		}
		else
		{
			*isSolved = false;
			return;
		}
	}

	*isSolved = true;
	*x0 = found_x;
	*y0= found_y;

	for (int x = 0; x < full_width; x++)
	{
		for (int y = 0; y < full_width; y++)
		{
			residuals[x + full_width * y] = intensity[x + full_width * y] - (*iBackground + *iStarMax * exp(-((x - *x0) * (x - *x0) + (y - *y0) * (y - *y0)) / (*r0 * *r0)));
		}
	}
}


void ConfigureSaturationLevels(unsigned long saturation8Bit, unsigned long saturation12Bit, unsigned long saturation14Bit, unsigned long saturation16Bit)
{
	SATURATION_8BIT = saturation8Bit;
	SATURATION_12BIT = saturation12Bit;
	SATURATION_14BIT = saturation14Bit;
	SATURATION_16BIT = saturation16Bit;
}