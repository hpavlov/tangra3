#pragma once

#include "cross_platform.h"

#define MAX_CONVOLUTION_WIDTH 1280
#define MAX_CONVOLUTION_HEIGHT 1024

unsigned long RESULT_BUFFER[MAX_CONVOLUTION_WIDTH * MAX_CONVOLUTION_HEIGHT];

const double GAUSSIAN_BLUR_MATRIX[] = { 
			1 / 16.0, 1 / 8.0, 1 / 16.0 , 
			1 / 8.0,  1 / 4.0, 1 / 8.0, 
            1 / 16.0, 1 / 8.0, 1 / 16.0 };

const double SHARPEN_MATRIX[] = { 
			-1.0, -1.0, -1.0, 
			-1.0,  9.0, -1.0, 
            -1.0, -1.0, -1.0 };
			
const double DENOISE_MATRIX[] = { 
			1/9.0, 1/9.0, 1/9.0, 
			1/9.0, 1/9.0, 1/9.0, 
            1/9.0, 1/9.0, 1/9.0 };
			
void Convolution_GaussianBlur(unsigned long* pixels, long bpp, long width, long height);
void Convolution_Sharpen(unsigned long* pixels, long bpp, long width, long height, unsigned long* average);
void Convolution_Denoise(unsigned long* pixels, long bpp, long width, long height, unsigned long* average, bool cutEdges);

/* Make sure functions are exported with C linkage under C++ compilers. */
#ifdef __cplusplus
extern "C"
{
#endif

DLL_PUBLIC HRESULT Convolution(unsigned long* data, long bpp, long width, long height, const double* convMatrix, bool cutEdges, bool calculateAverage, unsigned long* average);
DLL_PUBLIC HRESULT PrepareImageForOCR(unsigned long* pixels, long bpp, long width, long height);
DLL_PUBLIC HRESULT PrepareImageForOCRSingleStep(unsigned long* pixels, long bpp, long width, long height, long stepNo, unsigned long* average);
DLL_PUBLIC HRESULT LargeChunkDenoise(unsigned long* pixels, long width, long height, unsigned long onColour, unsigned long offColour);

#ifdef __cplusplus
} // __cplusplus defined.
#endif