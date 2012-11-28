#pragma once

#include "cross_platform.h"

/* Make sure functions are exported with C linkage under C++ compilers. */
#ifdef __cplusplus
extern "C"
{
#endif

DLL_PUBLIC void IntergationManagerStartNew(int width, int height, bool isMedianAveraging);
DLL_PUBLIC void IntegrationManagerAddFrame(unsigned long* framePixels);
DLL_PUBLIC void IntegrationManagerAddFrameEx(unsigned long* framePixels, bool isLittleEndian, int bpp);
DLL_PUBLIC void IntegrationManagerProduceIntegratedFrame(unsigned long* framePixels);
DLL_PUBLIC void IntegrationManagerFreeResources();
DLL_PUBLIC int IntegrationManagerGetFirstFrameToIntegrate(int producedFirstFrame, int frameCount, bool isSlidingIntegration);

#ifdef __cplusplus
} // __cplusplus defined.
#endif