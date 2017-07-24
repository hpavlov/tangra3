/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#pragma once

#include "cross_platform.h"

HRESULT PreProcessingMaskOutArea(unsigned int* pixels, int width, int height, unsigned int imageMedian, unsigned int numCorners, unsigned int* xPos, unsigned int* yPos);
