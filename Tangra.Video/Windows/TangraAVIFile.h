#pragma once

#include <time.h>
#include <stdio.h>
#include <windows.h>
#include <vfw.h>

namespace TangraAviFile
{

HRESULT AviFileOpenFile(LPCTSTR szFileName, PAVISTREAM* paviStream);
HRESULT AviFileGetStreamInfo(PAVISTREAM paviStream, AVISTREAMINFO* streamInfo, BITMAPINFOHEADER *lpFormat, long* firstFrame, long* countFrames);
PGETFRAME AviFileGetFrameOpen(PAVISTREAM paviStream, BITMAPINFOHEADER *bih);
HRESULT AviFileGetFrameClose(PGETFRAME frameObject);
HRESULT AviFileGetFrame(PGETFRAME frameObject, long frameNo, unsigned long* pixels, BYTE* bitmapPixels, BYTE* bitmapBytes);
HRESULT AviFileGetFramePixels(PGETFRAME frameObject, long frameNo, unsigned long* pixels);
HRESULT AviGetIntegratedFrame(PGETFRAME frameObject, long startFrameNo, long framesToIntegrate, bool isSlidingIntegration, bool isMedianAveraging, unsigned long* pixels, BYTE* bitmapPixels, BYTE* bitmapBytes);
HRESULT AviFileCloseStream(PAVISTREAM paviStream);

}