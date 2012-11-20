#include "stdafx.h"
#include "TangraVideo.h"


HRESULT TangraVideoEnumVideoEngines(char* videoEngines)
{
	strcpy(videoEngines, "AviFile;DirectShow");
	
	return S_OK;
}

HRESULT TangraVideoSetVideoEngine(int videoEngine)
{
	return S_OK;
}

HRESULT TangraVideoOpenFile(const char* fileName, VideoFileInfo* fileInfo)
{
	return S_OK;
}

HRESULT TangraVideoCloseFile()
{
	return S_OK;
}

HRESULT TangraVideoGetFrame(long frameNo, unsigned long* pixels, unsigned char* bitmapPixels, unsigned char* bitmapBytes)
{
	return S_OK;
}

HRESULT TangraVideoGetFramePixels(long frameNo, unsigned long* pixels)
{
	return S_OK;
}

HRESULT TangraVideoGetIntegratedFrame(long startFrameNo, long framesToIntegrate, bool isSlidingIntegration, bool isMedianAveraging, unsigned long* pixels, unsigned char* bitmapPixels, unsigned char* bitmapBytes)
{
	return S_OK;
}