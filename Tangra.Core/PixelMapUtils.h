#ifndef PIXELMAP_UTILS
#define PIXELMAP_UTILS

#include "cross_platform.h"

/* Make sure functions are exported with C linkage under C++ compilers. */
#ifdef __cplusplus
extern "C"
{
#endif

DLL_PUBLIC HRESULT GetPixelMapBits(BYTE* pDIB, long* width, long* height, DWORD imageSize, unsigned long* pixels, BYTE* bitmapPixels, BYTE* bitmapBytes);
DLL_PUBLIC HRESULT GetPixelMapBitsAndHBitmap(BYTE* pDIB, long* width, long* height, DWORD imageSize, unsigned long* pixels, BYTE* bitmapPixels, BYTE* bitmapBytes, HBITMAP hBitmap);

// Only returns the pixelmap pixels, does not create the bitmap structures
DLL_PUBLIC HRESULT GetPixelMapPixelsOnly(BYTE* pDIB, long width, long height, unsigned long* pixels);
DLL_PUBLIC HRESULT GetBitmapPixels(long width, long height, unsigned long* pixels, BYTE* bitmapPixels, BYTE* bitmapBytes, bool isLittleEndian, int bpp);
DLL_PUBLIC HRESULT BitmapSplitFieldsOSD(BYTE* bitmapPixels, long firstOsdLine, long lastOsdLine);


// Pre-Processing 
DLL_PUBLIC HRESULT PreProcessingStretch(unsigned long* pixels, long width, long height, int bpp, int fromValue, int toValue);
DLL_PUBLIC HRESULT PreProcessingClip(unsigned long* pixels, long width, long height, int bpp, int fromValue, int toValue);
DLL_PUBLIC HRESULT PreProcessingBrightnessContrast(unsigned long* pixels, long width, long height, int bpp, long brightness, long cotrast);
DLL_PUBLIC HRESULT PreProcessingGamma(unsigned long* pixels, long width, long height, int bpp, float gamma);
DLL_PUBLIC HRESULT PreProcessingApplyDarkFlatFrame(
	unsigned long* pixels,
	long width, 
	long height, 
	int bpp, 
	unsigned long* darkPixels, 
	unsigned long* flatPixels, 
	unsigned long darkMedian, 
	bool darkFrameAdjustLevelToMedian, 
	unsigned long flatMedian);
DLL_PUBLIC HRESULT PreProcessingLowPassFilter(unsigned long* pixels, long width, long height, int bpp);
DLL_PUBLIC HRESULT PreProcessingLowPassDifferenceFilter(unsigned long* pixels, long width, long height, int bpp);


#ifdef __cplusplus
} // __cplusplus defined.
#endif

#endif