#pragma once

#include "cross_platform.h"

/* Make sure functions are exported with C linkage under C++ compilers. */
#ifdef __cplusplus
extern "C"
{
#endif

DLL_PUBLIC HRESULT ADVOpenFile(char* fileName, AdvLib::AdvFileInfo* fileInfo);
DLL_PUBLIC HRESULT ADVCloseFile();
DLL_PUBLIC HRESULT ADVGetFrame(int frameNo, unsigned long* pixels, BYTE* bitmapPixels, BYTE* bitmapBytes, AdvLib::AdvFrameInfo* frameInfo, char* gpsFix, char* userCommand, char* systemError);
DLL_PUBLIC HRESULT ADVGetFrame2(int frameNo, unsigned long* pixels, BYTE* bitmapPixels, BYTE* bitmapBytes);
DLL_PUBLIC HRESULT ADVGetIntegratedFrame(int startFrameNo, int framesToIntegrate, bool isSlidingIntegration, bool isMedianAveraging, unsigned long* pixels, BYTE* bitmapBytes, BYTE* bitmapDisplayBytes, AdvLib::AdvFrameInfo* frameInfo);
DLL_PUBLIC HRESULT ADVGetFramePixels(int frameNo, unsigned long* pixels, AdvLib::AdvFrameInfo* frameInfo, char* gpsFix, char* userCommand, char* systemError);
DLL_PUBLIC HRESULT ADVGetFrameStatusChannel(int frameNo, AdvLib::AdvFrameInfo* frameInfo, char* gpsFix, char* userCommand, char* systemError);
DLL_PUBLIC HRESULT ADVGetFileTag(char* fileTagName, char* fileTagValue);

#ifdef __cplusplus
} // __cplusplus defined.
#endif