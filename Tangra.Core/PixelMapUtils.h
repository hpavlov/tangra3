#ifndef PIXELMAP_UTILS
#define PIXELMAP_UTILS

#include <windows.h>

/* Make sure functions are exported with C linkage under C++ compilers. */
#ifdef __cplusplus
extern "C"
{
#endif

HRESULT __cdecl GetPixelMapBits(BYTE* pDIB, long* width, long* height, DWORD imageSize, unsigned long* pixels, BYTE* bitmapPixels, BYTE* bitmapBytes);
HRESULT GetPixelMapBitsAndHBitmap(BYTE* pDIB, long* width, long* height, DWORD imageSize, unsigned long* pixels, BYTE* bitmapPixels, BYTE* bitmapBytes, HBITMAP hBitmap);

// Only returns the pixelmap pixels, does not create the bitmap structures
HRESULT __cdecl GetPixelMapPixelsOnly(BYTE* pDIB, long width, long height, unsigned long* pixels);
HRESULT __cdecl GetBitmapPixels(long width, long height, unsigned long* pixels, BYTE* bitmapPixels, BYTE* bitmapBytes, bool isLittleEndian, int bpp);


// Pre-Processing 
HRESULT __cdecl PreProcessingStretch(unsigned long* pixels, long width, long height, int bpp, int fromValue, int toValue);
HRESULT __cdecl PreProcessingClip(unsigned long* pixels, long width, long height, int bpp, int fromValue, int toValue);
HRESULT __cdecl PreProcessingBrightnessContrast(unsigned long* pixels, long width, long height, int bpp, long brightness, long cotrast);

#ifdef __cplusplus
} // __cplusplus defined.
#endif

#endif