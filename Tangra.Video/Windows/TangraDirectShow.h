/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#pragma once

#include "stdafx.h"
#include "TangraVideo.h"

namespace TangraDirectShow
{
	HRESULT DirectShowCloseFile();
	HRESULT DirectShowOpenFile(LPCTSTR fileName, VideoFileInfo* fileInfo);
	HRESULT DirectShowGetFrame(long frameNo, unsigned long* pixels, BYTE* bitmapPixels, BYTE* bitmapBytes);
	HRESULT DirectShowGetFramePixels(long frameNo, unsigned long* pixels);
	HRESULT DirectShowGetIntegratedFrame(long startFrameNo, long framesToIntegrate, bool isSlidingIntegration, bool isMedianAveraging, unsigned long* pixels, BYTE* bitmapPixels, BYTE* bitmapBytes);
}