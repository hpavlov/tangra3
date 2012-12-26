#include "TangraVideo.h"
#include "include/TangraCore.h"
#include <string.h>


// http://www.codeproject.com/Articles/109639/nVLC
//
int g_SelectedEngine = -1;


HRESULT TangraVideoEnumVideoEngines(char* videoEngines)
{
	strcpy(videoEngines, "VLC");
	
	return S_OK;
}

HRESULT TangraVideoSetVideoEngine(int videoEngine)
{
	if (videoEngine >= 0 && videoEngine <= 0)
	{
		g_SelectedEngine = videoEngine;
		return S_OK;
	}
	else
		return E_FAIL;
}

HRESULT TangraVideoOpenFile(const char* fileName, VideoFileInfo* fileInfo)
{
	return E_FAIL;
}

HRESULT TangraVideoCloseFile()
{
	return E_FAIL;
}

HRESULT TangraVideoGetFrame(long frameNo, unsigned long* pixels, unsigned char* bitmapPixels, unsigned char* bitmapBytes)
{
	return E_FAIL;
}

HRESULT TangraVideoGetFramePixels(long frameNo, unsigned long* pixels)
{
	return E_FAIL;
}

HRESULT TangraVideoGetIntegratedFrame(long startFrameNo, long framesToIntegrate, bool isSlidingIntegration, bool isMedianAveraging, unsigned long* pixels, unsigned char* bitmapPixels, unsigned char* bitmapBytes)
{
	return E_FAIL;
}

int GetProductVersion()
{	
	int major = 3;
	int minor = 0;
	int revision = 15;

	return (major << 28) + (minor << 16) + revision;
}