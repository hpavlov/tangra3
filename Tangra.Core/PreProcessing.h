#pragma once

extern bool g_UsesPreProcessing;

typedef unsigned char BYTE;   // 8-bit unsigned entity.

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

bool __cdecl UsesPreProcessing();
long __cdecl PreProcessingClearAll();
long __cdecl PreProcessingAddStretching(unsigned int fromValue, unsigned int toValue);
long __cdecl PreProcessingAddClipping(unsigned int  fromValue, unsigned int  toValue);
long __cdecl PreProcessingAddBrightnessContrast(long brigtness, long contrast);
long __cdecl PreProcessingAddDigitalFilter(enum PreProcessingFilter filter);
long __cdecl PreProcessingAddGammaCorrection(float gamma);
long __cdecl PreProcessingAddDarkFrame(long* darkFramePixels, unsigned int pixelsCount);
long __cdecl PreProcessingAddFlatFrame(long* flatFramePixels, unsigned int pixelsCount, unsigned long flatFrameMedian);
long __cdecl PreProcessingUsesPreProcessing(bool* usesPreProcessing);
long __cdecl PreProcessingGetConfig(PreProcessingType* preProcessingType, unsigned int* fromValue, unsigned int* toValue, long* brigtness, long* contrast, PreProcessingFilter* filter, float* gamma, unsigned int* darkPixelsCount, unsigned int* flatPixelsCount);

long __cdecl ApplyPreProcessingPixelsOnly(unsigned long* pixels, long width, long height, int bpp);
long __cdecl ApplyPreProcessing(unsigned long* pixels, long width, long height, int bpp, BYTE* bitmapPixels, BYTE* bitmapBytes);

#ifdef __cplusplus
} // __cplusplus defined.
#endif