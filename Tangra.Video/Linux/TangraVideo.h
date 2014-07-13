#pragma once

#include "WindowsTypes.h"

#define VERSION_MAJOR 3
#define VERSION_MINOR 0
#define VERSION_REVISION 11

struct VideoFileInfo
{
	long Width;
	long Height;
	float FrameRate;
	long CountFrames;
	long FirstFrame;
	long BitmapImageSize;
};

#ifdef  __cplusplus
extern "C" {
#endif


DLL_PUBLIC int GetTangraVideoVersion();

DLL_PUBLIC HRESULT TangraVideoEnumVideoEngines(char* videoEngines);

DLL_PUBLIC HRESULT TangraVideoSetVideoEngine(int videoEngine);

DLL_PUBLIC HRESULT TangraVideoOpenFile(const char* fileName, VideoFileInfo* fileInfo);

DLL_PUBLIC HRESULT TangraVideoCloseFile();

DLL_PUBLIC HRESULT TangraVideoGetFrame(long frameNo, unsigned long* pixels, unsigned char* bitmapPixels, unsigned char* bitmapBytes);

DLL_PUBLIC HRESULT TangraVideoGetFramePixels(long frameNo, unsigned long* pixels);

DLL_PUBLIC HRESULT TangraVideoGetIntegratedFrame(long startFrameNo, long framesToIntegrate, bool isSlidingIntegration, bool isMedianAveraging, unsigned long* pixels, unsigned char* bitmapPixels, unsigned char* bitmapBytes);

#ifdef  __cplusplus
}
#endif