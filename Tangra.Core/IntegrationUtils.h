#pragma once

/* Make sure functions are exported with C linkage under C++ compilers. */
#ifdef __cplusplus
extern "C"
{
#endif

void __cdecl IntergationManagerStartNew(int width, int height, bool isMedianAveraging);
void __cdecl IntegrationManagerAddFrame(unsigned long* framePixels);
void IntegrationManagerAddFrameEx(unsigned long* framePixels, bool isLittleEndian, int bpp);
void __cdecl IntegrationManagerProduceIntegratedFrame(unsigned long* framePixels);
void __cdecl IntegrationManagerFreeResources();
int __cdecl IntegrationManagerGetFirstFrameToIntegrate(int producedFirstFrame, int frameCount, bool isSlidingIntegration);

#ifdef __cplusplus
} // __cplusplus defined.
#endif