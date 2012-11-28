#pragma once

/* Make sure functions are exported with C linkage under C++ compilers. */
#ifdef __cplusplus
extern "C"
{
#endif

void IntergationManagerStartNew(int width, int height, bool isMedianAveraging);
void IntegrationManagerAddFrame(unsigned long* framePixels);
void IntegrationManagerAddFrameEx(unsigned long* framePixels, bool isLittleEndian, int bpp);
void IntegrationManagerProduceIntegratedFrame(unsigned long* framePixels);
void IntegrationManagerFreeResources();
int IntegrationManagerGetFirstFrameToIntegrate(int producedFirstFrame, int frameCount, bool isSlidingIntegration);

#ifdef __cplusplus
} // __cplusplus defined.
#endif