#pragma once

#include "WindowsTypes.h"


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


int GetProductVersion();

HRESULT TangraVideoEnumVideoEngines(char* videoEngines);

HRESULT TangraVideoSetVideoEngine(int videoEngine);

HRESULT TangraVideoOpenFile(const char* fileName, VideoFileInfo* fileInfo);

HRESULT TangraVideoCloseFile();

HRESULT TangraVideoGetFrame(long frameNo, unsigned long* pixels, unsigned char* bitmapPixels, unsigned char* bitmapBytes);

HRESULT TangraVideoGetFramePixels(long frameNo, unsigned long* pixels);

HRESULT TangraVideoGetIntegratedFrame(long startFrameNo, long framesToIntegrate, bool isSlidingIntegration, bool isMedianAveraging, unsigned long* pixels, unsigned char* bitmapPixels, unsigned char* bitmapBytes);

#ifdef  __cplusplus
}
#endif