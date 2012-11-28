#pragma once

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

bool UsesPreProcessing();
long PreProcessingClearAll();
long PreProcessingAddStretching(unsigned int fromValue, unsigned int toValue);
long PreProcessingAddClipping(unsigned int  fromValue, unsigned int  toValue);
long PreProcessingAddBrightnessContrast(long brigtness, long contrast);
long PreProcessingAddDigitalFilter(enum PreProcessingFilter filter);
long PreProcessingAddGammaCorrection(float gamma);
long PreProcessingAddDarkFrame(long* darkFramePixels, unsigned int pixelsCount);
long PreProcessingAddFlatFrame(long* flatFramePixels, unsigned int pixelsCount, unsigned long flatFrameMedian);
long PreProcessingUsesPreProcessing(bool* usesPreProcessing);
long PreProcessingGetConfig(PreProcessingType* preProcessingType, unsigned int* fromValue, unsigned int* toValue, long* brigtness, long* contrast, PreProcessingFilter* filter, float* gamma, unsigned int* darkPixelsCount, unsigned int* flatPixelsCount);

long ApplyPreProcessingPixelsOnly(unsigned long* pixels, long width, long height, int bpp);
long ApplyPreProcessing(unsigned long* pixels, long width, long height, int bpp, BYTE* bitmapPixels, BYTE* bitmapBytes);

#ifdef __cplusplus
} // __cplusplus defined.
#endif