/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#ifndef ADV_LIB
#define ADV_LIB

#include "adv_file.h"

extern char* g_CurrentAdvFile;
extern AdvLib::AdvFile* g_AdvFile;
extern bool g_FileStarted;

#ifdef __cplusplus
extern "C"
{
#endif 

DLL_PUBLIC char* AdvGetCurrentFilePath(void);
DLL_PUBLIC void AdvNewFile(const char* fileName);
DLL_PUBLIC void AdvDefineImageSection(unsigned short width, unsigned short height, unsigned char dataBpp);
DLL_PUBLIC void AdvDefineImageLayout(unsigned char layoutId, const char* layoutType, const char* compression, unsigned char layoutBpp, int keyFrame, const char* diffCorrFromBaseFrame);
DLL_PUBLIC unsigned int AdvDefineStatusSectionTag(const char* tagName, int tagType);
DLL_PUBLIC unsigned int AdvAddFileTag(const char* tagName, const char* tagValue);
DLL_PUBLIC void AdvAddOrUpdateImageSectionTag(const char* tagName, const char* tagValue);
DLL_PUBLIC void AdvEndFile();
DLL_PUBLIC bool AdvBeginFrame(long long timeStamp, unsigned int elapsedTime, unsigned int exposure);
DLL_PUBLIC void AdvFrameAddImage(unsigned char layoutId, unsigned short* pixels, unsigned char pixelsBpp);
DLL_PUBLIC void AdvFrameAddImageBytes(unsigned char layoutId, unsigned char* pixels, unsigned char pixelsBpp);
DLL_PUBLIC void AdvFrameAddStatusTag(unsigned int tagIndex, const char* tagValue);
DLL_PUBLIC void AdvFrameAddStatusTagMessage(unsigned int tagIndex, const char* tagValue);
DLL_PUBLIC void AdvFrameAddStatusTagUInt8(unsigned int tagIndex, unsigned char tagValue);
DLL_PUBLIC void AdvFrameAddStatusTag16(unsigned int tagIndex, unsigned short tagValue);
DLL_PUBLIC void AdvFrameAddStatusTagReal(unsigned int tagIndex, float tagValue);
DLL_PUBLIC void AdvFrameAddStatusTag32(unsigned int tagIndex, unsigned long tagValue);
DLL_PUBLIC void AdvFrameAddStatusTag64(unsigned int tagIndex, long long tagValue);
DLL_PUBLIC void AdvEndFrame();

#ifdef __cplusplus
}
#endif
 
#endif

