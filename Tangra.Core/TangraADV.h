/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#pragma once

#include "cross_platform.h"
#include "adv2_error_codes.h"

extern bool g_IsAAV;

/* Make sure functions are exported with C linkage under C++ compilers. */
#ifdef __cplusplus
extern "C"
{
#endif

DLL_PUBLIC HRESULT ADVOpenFile(char* fileName, AdvLib::AdvFileInfo* fileInfo);
DLL_PUBLIC HRESULT ADVCloseFile();
DLL_PUBLIC HRESULT ADVGetFrame(int frameNo, unsigned int* pixels, unsigned int* originalPixels, BYTE* bitmapPixels, BYTE* bitmapBytes, AdvLib::AdvFrameInfo* frameInfo, char* gpsFix, char* userCommand, char* systemError);
DLL_PUBLIC HRESULT ADVGetFrame2(int frameNo, unsigned int* pixels, unsigned int* originalPixels, BYTE* bitmapPixels, BYTE* bitmapBytes);
DLL_PUBLIC HRESULT ADVGetIntegratedFrame(int startFrameNo, int framesToIntegrate, bool isSlidingIntegration, bool isMedianAveraging, unsigned int* pixels, unsigned int* originalPixels, BYTE* bitmapBytes, BYTE* bitmapDisplayBytes, AdvLib::AdvFrameInfo* frameInfo);
DLL_PUBLIC HRESULT ADVGetFramePixels(int frameNo, unsigned int* pixels, AdvLib::AdvFrameInfo* frameInfo, char* gpsFix, char* userCommand, char* systemError);
DLL_PUBLIC HRESULT ADVGetFrameStatusChannel(int frameNo, AdvLib::AdvFrameInfo* frameInfo, char* gpsFix, char* userCommand, char* systemError);
DLL_PUBLIC HRESULT ADVGetFileTag(char* fileTagName, char* fileTagValue);
DLL_PUBLIC HRESULT Lagarith16Decompress(int width, int height, unsigned char* compressedBytes, unsigned char* decompressedBytes);

DLL_PUBLIC ADVRESULT ADV2GetFormatVersion(char* fileName);
DLL_PUBLIC ADVRESULT ADV2OpenFile(char* fileName, AdvLib2::AdvFileInfo* fileInfo);
DLL_PUBLIC ADVRESULT ADV2CloseFile();
DLL_PUBLIC ADVRESULT ADV2GetFrame(int frameNo, unsigned int* pixels, unsigned int* originalPixels, BYTE* bitmapPixels, BYTE* bitmapBytes, AdvLib2::AdvFrameInfo* frameInfo);
DLL_PUBLIC ADVRESULT ADV2GetFramePixels(int frameNo, unsigned int* pixels, AdvLib2::AdvFrameInfo* frameInfo, int* systemErrorLen);
DLL_PUBLIC ADVRESULT ADV2GetFrameStatusChannel(int frameNo, AdvLib2::AdvFrameInfo* frameInfo);
DLL_PUBLIC ADVRESULT ADV2GetFileTag(char* fileTagName, char* fileTagValue);

#ifdef __cplusplus
} // __cplusplus defined.
#endif