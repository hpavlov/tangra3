#pragma once

#include "stdafx.h"
#include "TangraVideo.h"

namespace TangraDirectShow
{
	HRESULT DirectShowCloseFile();
	HRESULT DirectShowOpenFile(LPCTSTR fileName, VideoFileInfo* fileInfo);
	HRESULT DirectShowGetFrame(long frameNo, unsigned long* pixels, BYTE* bitmapPixels, BYTE* bitmapBytes);
	HRESULT DirectShowGetFramePixels(long frameNo, unsigned long* pixels);
	HRESULT DirectShowGetIntegratedFrame(long startFrameNo, long framesToIntegrate, bool isSlidingIntegration, bool isMedianAveraging, unsigned long* pixels, BYTE* bitmapPixels, BYTE* bitmapBytes);
}