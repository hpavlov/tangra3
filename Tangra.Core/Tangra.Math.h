/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#ifndef TANGRAMATH_H
#define TANGRAMATH_H

#include "cross_platform.h"

extern unsigned int SATURATION_8BIT;
extern unsigned int SATURATION_12BIT;
extern unsigned int SATURATION_14BIT;
extern unsigned int SATURATION_16BIT;
	
extern double* s_TransponseBuffer;
extern int s_NumVariables;
extern int s_MaxEquations;

void EnsureLinearSystemSolutionBuffers(int numVariables);

/* Make sure functions are exported with C linkage under C++ compilers. */
#ifdef __cplusplus
extern "C"
{
#endif

DLL_PUBLIC void SolveLinearSystem(double* a, int aRows, int aCols, double* x, int xRows, int xCols, double* y);

DLL_PUBLIC void SolveLinearSystemFast(double* a, double* x, int numEquations, double* y);
DLL_PUBLIC void LinearSystemFastInitialiseSolution(int numVariables, int maxEquations);
DLL_PUBLIC void DoNonLinearPfsFit(unsigned int* intensity, const int squareSize, const int saturation, bool* isSolved, double* iBackground, double* iStarMax, double* x0, double* y0, double* r0, double* residuals);
DLL_PUBLIC void ConfigureSaturationLevels(unsigned int saturation8Bit, unsigned int saturation12Bit, unsigned int saturation14Bit, unsigned int saturation16Bit);

#ifdef __cplusplus
} // __cplusplus defined.
#endif

#endif // TANGRAMATH_H