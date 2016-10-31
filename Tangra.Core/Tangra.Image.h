/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#pragma once

#include "cross_platform.h"

#define MAX_CONVOLUTION_WIDTH 1280
#define MAX_CONVOLUTION_HEIGHT 1024

unsigned int RESULT_BUFFER[MAX_CONVOLUTION_WIDTH * MAX_CONVOLUTION_HEIGHT];

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
			
void Convolution_GaussianBlur(unsigned int* pixels, int bpp, int width, int height);
void Convolution_GaussianBlur_Area(unsigned int* pixels, unsigned int maxPixelValue, int x0, int y0, int areaWidth, int areaHeight, int width, int height);
void Convolution_Sharpen(unsigned int* pixels, int bpp, int width, int height, unsigned int* average);
void Convolution_Denoise(unsigned int* pixels, int bpp, int width, int height, unsigned int* average, bool cutEdges);

/* Make sure functions are exported with C linkage under C++ compilers. */
#ifdef __cplusplus
extern "C"
{
#endif

DLL_PUBLIC HRESULT Convolution(unsigned int* data, int bpp, int width, int height, const double* convMatrix, bool cutEdges, bool calculateAverage, unsigned int* average);
DLL_PUBLIC HRESULT PrepareImageForOCR(unsigned int* pixels, int bpp, int width, int height);
DLL_PUBLIC HRESULT PrepareImageForOCRSingleStep(unsigned int* pixels, int bpp, int width, int height, int stepNo, unsigned int* average);
DLL_PUBLIC HRESULT LargeChunkDenoise(unsigned int* pixels, int width, int height, unsigned int onColour, unsigned int offColour);

#ifdef __cplusplus
} // __cplusplus defined.
#endif