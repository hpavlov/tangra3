/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#pragma once

#include "cross_platform.h"

extern bool g_UsesPreProcessing;

enum PreProcessingFilter
{
	 ppfNoFilter = 0,
     ppfLowPassFilter = 1,
     ppfLowPassDifferenceFilter = 2
};

enum PreProcessingType
{
	pptpNone = 0,
	pptpStretching = 1,
	pptpClipping = 2,
	pptpBrightnessContrast = 3
};

enum RotateFlipType
{
	Rotate180FlipXY = 0,
	RotateNoneFlipNone = 0,
	Rotate270FlipXY = 1,
	Rotate90FlipNone = 1,
	Rotate180FlipNone = 2,
	RotateNoneFlipXY = 2,
	Rotate270FlipNone = 3,
	Rotate90FlipXY = 3,
	Rotate180FlipY = 4,
	RotateNoneFlipX = 4,
	Rotate270FlipY = 5,
	Rotate90FlipX = 5,
	Rotate180FlipX = 6,
	RotateNoneFlipY = 6,
	Rotate270FlipX = 7,
	Rotate90FlipY = 7
};

/* Make sure functions are exported with C linkage under C++ compilers. */
#ifdef __cplusplus
extern "C"
{
#endif

DLL_PUBLIC bool UsesPreProcessing();
DLL_PUBLIC int PreProcessingClearAll();
DLL_PUBLIC int PreProcessingAddStretching(unsigned int fromValue, unsigned int toValue);
DLL_PUBLIC int PreProcessingAddClipping(unsigned int  fromValue, unsigned int  toValue);
DLL_PUBLIC int PreProcessingAddBrightnessContrast(int brigtness, int contrast);
DLL_PUBLIC int PreProcessingAddDigitalFilter(enum PreProcessingFilter filter);
DLL_PUBLIC int PreProcessingAddGammaCorrection(float gamma);
DLL_PUBLIC int PreProcessingAddCameraResponseCorrection(int knownCameraResponse, int* responseParams);
DLL_PUBLIC int PreProcessingAddDarkFrame(float* darkFramePixels, unsigned int pixelsCount, float exposureSeconds, bool isBiasCorrected, bool isSameExposure);
DLL_PUBLIC int PreProcessingAddFlatFrame(float* flatFramePixels, unsigned int pixelsCount, float flatFrameMedian);
DLL_PUBLIC int PreProcessingAddBiasFrame(float* biasFramePixels, unsigned int pixelsCount);
DLL_PUBLIC int PreProcessingAddFlipAndRotation(enum RotateFlipType rotateFlipType);
DLL_PUBLIC int PreProcessingUsesPreProcessing(bool* usesPreProcessing);
DLL_PUBLIC int PreProcessingGetConfig(
	PreProcessingType* preProcessingType, 
	unsigned int* fromValue, 
	unsigned int* toValue, 
	int* brigtness, 
	int* contrast, 
	PreProcessingFilter* filter, 
	float* gamma, 
	int* reversedCameraResponse,
	unsigned int* darkPixelsCount, 
	unsigned int* flatPixelsCount,
	unsigned int* biasPixelsCount,
	RotateFlipType* rotateFlipType);
	
DLL_PUBLIC int ApplyPreProcessingPixelsOnly(unsigned int* pixels, int width, int height, int bpp, unsigned int normVal, float exposureSeconds);
DLL_PUBLIC int ApplyPreProcessing(unsigned int* pixels, int width, int height, int bpp, float exposureSeconds, BYTE* bitmapPixels, BYTE* bitmapBytes);
int ApplyPreProcessingWithNormalValue(unsigned int* pixels, int width, int height, int bpp, float exposureSeconds, unsigned int normVal, BYTE* bitmapPixels, BYTE* bitmapBytes);

DLL_PUBLIC HRESULT SwapVideoFields(unsigned int* pixels, unsigned int* originalPixels, int width, int height, BYTE* bitmapPixels, BYTE* bitmapBytes);		
DLL_PUBLIC HRESULT ShiftVideoFields(unsigned int* pixels, unsigned int* originalPixels, unsigned int* pixels2, unsigned int* originalPixels2, int width, int height, int fldIdx, BYTE* bitmapPixels, BYTE* bitmapBytes);		

#ifdef __cplusplus
} // __cplusplus defined.
#endif