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
DLL_PUBLIC long PreProcessingClearAll();
DLL_PUBLIC long PreProcessingAddStretching(unsigned int fromValue, unsigned int toValue);
DLL_PUBLIC long PreProcessingAddClipping(unsigned int  fromValue, unsigned int  toValue);
DLL_PUBLIC long PreProcessingAddBrightnessContrast(long brigtness, long contrast);
DLL_PUBLIC long PreProcessingAddDigitalFilter(enum PreProcessingFilter filter);
DLL_PUBLIC long PreProcessingAddGammaCorrection(float gamma);
DLL_PUBLIC long PreProcessingAddDarkFrame(float* darkFramePixels, unsigned long pixelsCount, float exposureSeconds, bool isBiasCorrected, bool isSameExposure);
DLL_PUBLIC long PreProcessingAddFlatFrame(float* flatFramePixels, unsigned long pixelsCount, float flatFrameMedian);
DLL_PUBLIC long PreProcessingAddBiasFrame(float* biasFramePixels, unsigned long pixelsCount);
DLL_PUBLIC long PreProcessingAddFlipAndRotation(enum RotateFlipType rotateFlipType);
DLL_PUBLIC long PreProcessingUsesPreProcessing(bool* usesPreProcessing);
DLL_PUBLIC long PreProcessingGetConfig(
	PreProcessingType* preProcessingType, 
	unsigned int* fromValue, 
	unsigned int* toValue, 
	long* brigtness, 
	long* contrast, 
	PreProcessingFilter* filter, 
	float* gamma, 
	unsigned int* darkPixelsCount, 
	unsigned int* flatPixelsCount,
	unsigned int* biasPixelsCount,
	RotateFlipType* rotateFlipType);

DLL_PUBLIC long ApplyPreProcessingPixelsOnly(unsigned long* pixels, long width, long height, int bpp, float exposureSeconds);
DLL_PUBLIC long ApplyPreProcessing(unsigned long* pixels, long width, long height, int bpp, float exposureSeconds, BYTE* bitmapPixels, BYTE* bitmapBytes);
long ApplyPreProcessingWithNormalValue(unsigned long* pixels, long width, long height, int bpp, float exposureSeconds, unsigned long normVal, BYTE* bitmapPixels, BYTE* bitmapBytes);

#ifdef __cplusplus
} // __cplusplus defined.
#endif