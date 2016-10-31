/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#pragma once

#include "cross_platform.h"

HRESULT PreProcessingRemoveHotPixels(unsigned int* pixels, int width, int height, unsigned int* model, unsigned int numPixels, unsigned int* xPos, unsigned int* yPos, unsigned int imageMedian, unsigned int maxPixelValue);
