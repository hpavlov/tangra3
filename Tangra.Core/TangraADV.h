#pragma once



/* Make sure functions are exported with C linkage under C++ compilers. */
#ifdef __cplusplus
extern "C"
{
#endif

HRESULT __cdecl ADVOpenFile(char* fileName, AdvLib::AdvFileInfo* fileInfo);
HRESULT __cdecl ADVCloseFile();
HRESULT __cdecl ADVGetFrame(int frameNo, unsigned long* pixels, BYTE* bitmapPixels, BYTE* bitmapBytes, AdvLib::AdvFrameInfo* frameInfo, char* gpsFix, char* userCommand, char* systemError);
HRESULT __cdecl ADVGetFrame2(int frameNo, unsigned long* pixels, BYTE* bitmapPixels, BYTE* bitmapBytes);
HRESULT __cdecl ADVGetIntegratedFrame(int startFrameNo, int framesToIntegrate, bool isSlidingIntegration, bool isMedianAveraging, unsigned long* pixels, BYTE* bitmapBytes, BYTE* bitmapDisplayBytes, AdvLib::AdvFrameInfo* frameInfo);
HRESULT __cdecl ADVGetFramePixels(int frameNo, unsigned long* pixels, AdvLib::AdvFrameInfo* frameInfo, char* gpsFix, char* userCommand, char* systemError);


#ifdef __cplusplus
} // __cplusplus defined.
#endif