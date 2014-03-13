#ifndef ADV_LIB
#define ADV_LIB

#include "adv_file.h"

extern char* g_CurrentAdvFile;
extern AdvLib::AdvFile* g_AdvFile;
extern bool g_FileStarted;

		
char* AdvGetCurrentFilePath(void);
void AdvNewFile(const char* fileName);
void AdvDefineImageSection(unsigned short width, unsigned short height, unsigned char dataBpp);
void AdvDefineImageLayout(unsigned char layoutId, const char* layoutType, const char* compression, unsigned char layoutBpp, int keyFrame, const char* diffCorrFromBaseFrame);
unsigned int AdvDefineStatusSectionTag(const char* tagName, int tagType);
unsigned int AdvAddFileTag(const char* tagName, const char* tagValue);
void AdvAddOrUpdateImageSectionTag(const char* tagName, const char* tagValue);
void AdvEndFile();
bool AdvBeginFrame(long long timeStamp, unsigned int elapsedTime, unsigned int exposure);
void AdvFrameAddImage(unsigned char layoutId, unsigned short* pixels, unsigned char pixelsBpp);
void AdvFrameAddImageBytes(unsigned char layoutId, unsigned char* pixels, unsigned char pixelsBpp);
void AdvFrameAddStatusTag(unsigned int tagIndex, const char* tagValue);
void AdvFrameAddStatusTagMessage(unsigned int tagIndex, const char* tagValue);
void AdvFrameAddStatusTagUInt8(unsigned int tagIndex, unsigned char tagValue);
void AdvFrameAddStatusTag16(unsigned int tagIndex, unsigned short tagValue);
void AdvFrameAddStatusTagReal(unsigned int tagIndex, float tagValue);
void AdvFrameAddStatusTag32(unsigned int tagIndex, unsigned long tagValue);
void AdvFrameAddStatusTag64(unsigned int tagIndex, long long tagValue);
void AdvEndFrame();

#endif

