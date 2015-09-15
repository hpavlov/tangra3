/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#pragma once

#include "cross_platform.h"

extern bool g_IsAAV;

/* Make sure functions are exported with C linkage under C++ compilers. */
#ifdef __cplusplus
extern "C"
{
#endif

DLL_PUBLIC HRESULT ADVOpenFile(char* fileName, AdvLib::AdvFileInfo* fileInfo);
DLL_PUBLIC HRESULT ADVCloseFile();
DLL_PUBLIC HRESULT ADVGetFrame(long frameNo, unsigned long* pixels, unsigned long* originalPixels, BYTE* bitmapPixels, BYTE* bitmapBytes, AdvLib::AdvFrameInfo* frameInfo, char* gpsFix, char* userCommand, char* systemError);
DLL_PUBLIC HRESULT ADVGetFrame2(long frameNo, unsigned long* pixels, unsigned long* originalPixels, BYTE* bitmapPixels, BYTE* bitmapBytes);
DLL_PUBLIC HRESULT ADVGetIntegratedFrame(long startFrameNo, long framesToIntegrate, bool isSlidingIntegration, bool isMedianAveraging, unsigned long* pixels, unsigned long* originalPixels, BYTE* bitmapBytes, BYTE* bitmapDisplayBytes, AdvLib::AdvFrameInfo* frameInfo);
DLL_PUBLIC HRESULT ADVGetFramePixels(long frameNo, unsigned long* pixels, AdvLib::AdvFrameInfo* frameInfo, char* gpsFix, char* userCommand, char* systemError);
DLL_PUBLIC HRESULT ADVGetFrameStatusChannel(long frameNo, AdvLib::AdvFrameInfo* frameInfo, char* gpsFix, char* userCommand, char* systemError);
DLL_PUBLIC HRESULT ADVGetFileTag(char* fileTagName, char* fileTagValue);
DLL_PUBLIC HRESULT Lagarith16Decompress(long width, long height, unsigned char* compressedBytes, unsigned char* decompressedBytes);

#ifdef __cplusplus
} // __cplusplus defined.
#endif