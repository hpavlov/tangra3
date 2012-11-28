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
DLL_PUBLIC long PreProcessingAddDarkFrame(long* darkFramePixels, unsigned int pixelsCount);
DLL_PUBLIC long PreProcessingAddFlatFrame(long* flatFramePixels, unsigned int pixelsCount, unsigned long flatFrameMedian);
DLL_PUBLIC long PreProcessingUsesPreProcessing(bool* usesPreProcessing);
DLL_PUBLIC long PreProcessingGetConfig(PreProcessingType* preProcessingType, unsigned int* fromValue, unsigned int* toValue, long* brigtness, long* contrast, PreProcessingFilter* filter, float* gamma, unsigned int* darkPixelsCount, unsigned int* flatPixelsCount);

DLL_PUBLIC long ApplyPreProcessingPixelsOnly(unsigned long* pixels, long width, long height, int bpp);
DLL_PUBLIC long ApplyPreProcessing(unsigned long* pixels, long width, long height, int bpp, BYTE* bitmapPixels, BYTE* bitmapBytes);

#ifdef __cplusplus
} // __cplusplus defined.
#endif