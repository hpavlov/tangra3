#ifndef TANGRAMATH_H
#define TANGRAMATH_H

#include "cross_platform.h"

extern unsigned long SATURATION_8BIT;
extern unsigned long SATURATION_12BIT;
extern unsigned long SATURATION_14BIT;
	
extern double* s_TransponseBuffer;
extern long s_NumVariables;
extern long s_MaxEquations;

/* Make sure functions are exported with C linkage under C++ compilers. */
#ifdef __cplusplus
extern "C"
{
#endif

DLL_PUBLIC void SolveLinearSystem(double* a, long aRows, long aCols, double* x, long xRows, long xCols, double* y);

DLL_PUBLIC void SolveLinearSystemFast(double* a, double* x, long numEquations, double* y);
DLL_PUBLIC void LinearSystemFastInitialiseSolution(long numVariables, long maxEquations);
DLL_PUBLIC void DoNonLinearPfsFit(unsigned long* intensity, const long squareSize, const long saturation, bool* isSolved, double* iBackground, double* iStarMax, double* x0, double* y0, double* r0, double* residuals);
DLL_PUBLIC void ConfigureSaturationLevels(unsigned long saturation8Bit, unsigned long saturation12Bit, unsigned long saturation14Bit);

#ifdef __cplusplus
} // __cplusplus defined.
#endif

#endif // SAFEMATRIX_H