/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#ifndef PIXELMAP_UTILS
#define PIXELMAP_UTILS

#include "cross_platform.h"
#include "PreProcessing.h"

/* Make sure functions are exported with C linkage under C++ compilers. */
#ifdef __cplusplus
extern "C"
{
#endif

DLL_PUBLIC HRESULT GetPixelMapBits(BYTE* pDIB,int* width,int* height, DWORD imageSize, unsigned int* pixels, BYTE* bitmapPixels, BYTE* bitmapBytes);
DLL_PUBLIC HRESULT GetPixelMapBitsAndHBitmap(BYTE* pDIB,int* width,int* height, DWORD imageSize, unsigned int* pixels, BYTE* bitmapPixels, BYTE* bitmapBytes, HBITMAP hBitmap);

// Only returns the pixelmap pixels, does not create the bitmap structures
DLL_PUBLIC HRESULT GetPixelMapPixelsOnly(BYTE* pDIB, int width, int height, unsigned int* pixels);
DLL_PUBLIC HRESULT GetBitmapPixels(int width, int height, unsigned int* pixels, BYTE* bitmapPixels, BYTE* bitmapBytes, bool isLittleEndian, int bpp, unsigned int normVal);
DLL_PUBLIC HRESULT BitmapSplitFieldsOSD(BYTE* bitmapPixels, int firstOsdLine, int lastOsdLine);


// Pre-Processing 
DLL_PUBLIC HRESULT PreProcessingFlipRotate(unsigned int* pixels, int width, int height, int bpp, enum RotateFlipType flipRotateType);
DLL_PUBLIC HRESULT PreProcessingStretch(unsigned int* pixels, int width, int height, int bpp, unsigned int normVal, int fromValue, int toValue);
DLL_PUBLIC HRESULT PreProcessingClip(unsigned int* pixels, int width, int height, int bpp, unsigned int normVal, int fromValue, int toValue);
DLL_PUBLIC HRESULT PreProcessingBrightnessContrast(unsigned int* pixels, int width, int height, int bpp, unsigned int normVal, int brightness, int cotrast);
DLL_PUBLIC HRESULT PreProcessingGamma(unsigned int* pixels, int width, int height, int bpp, unsigned int normVal, float gamma);
DLL_PUBLIC HRESULT PreProcessingReverseCameraResponse(unsigned int* pixels, int width, int height, int bpp, unsigned int normVal, int knownCameraResponse, int* knownCameraResponseParams);
DLL_PUBLIC HRESULT PreProcessingApplyBiasDarkFlatFrame(
	unsigned int* pixels,
	int width, 
	int height, 
	int bpp, 
	unsigned int normVal,
	float* biasPixels, float* darkPixels, float* flatPixels, 
	float scienseExposure, float darkExposure, bool darkFrameIsBiasCorrected, bool isSameExposureDarkFrame, float flatMedian);
DLL_PUBLIC HRESULT PreProcessingLowPassFilter(unsigned int* pixels, int width, int height, int bpp, unsigned int normVal);
DLL_PUBLIC HRESULT PreProcessingLowPassDifferenceFilter(unsigned int* pixels, int width, int height, int bpp, unsigned int normVal);

DLL_PUBLIC HRESULT GetRotatedFrameDimentions(int width, int height, double angleDegrees,int* newWidth,int* newHeight);
DLL_PUBLIC HRESULT RotateFrame(int width, int height, double angleDegrees, unsigned int* originalPixels, int destWidth, int destHeight, unsigned int* pixels, BYTE* bitmapPixels, BYTE* bitmapBytes, int dataBpp, unsigned int normalisationValue);

#ifdef __cplusplus
} // __cplusplus defined.
#endif

__uint64 GetUInt64Average(__uint64 a, __uint64 b);
__uint64 GetUInt64Average(unsigned int aLo, unsigned int aHi, unsigned int bLo, unsigned int bHi);

#endif