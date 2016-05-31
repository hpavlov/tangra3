/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#pragma once

#include "cross_platform.h"

/* Make sure functions are exported with C linkage under C++ compilers. */
#ifdef __cplusplus
extern "C"
{
#endif

DLL_PUBLIC void IntergationManagerStartNew(int width, int height, bool isMedianAveraging);
DLL_PUBLIC void IntegrationManagerAddFrame(unsigned int* framePixels);
DLL_PUBLIC void IntegrationManagerAddFrameEx(unsigned int* framePixels, bool isLittleEndian, int bpp);
DLL_PUBLIC void IntegrationManagerProduceIntegratedFrame(unsigned int* framePixels);
DLL_PUBLIC void IntegrationManagerFreeResources();
DLL_PUBLIC int IntegrationManagerGetFirstFrameToIntegrate(int producedFirstFrame, int frameCount, bool isSlidingIntegration);

#ifdef __cplusplus
} // __cplusplus defined.
#endif