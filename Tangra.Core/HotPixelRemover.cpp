/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#include "HotPixelRemover.h"

extern void Convolution_GaussianBlur_Area(unsigned int* pixels, unsigned int maxPixelValue, int x0, int y0, int areaWidth, int areaHeight, int width, int height);


void AlignHotPixelModel(unsigned int* pixels, int width, int height, unsigned int* model, unsigned int x0, unsigned int y0, unsigned int imageMedian, int* deltaX, int* deltaY)
{
	double smallestDiff = 1E99;
	*deltaX = 0;
	*deltaY = 0;

	for (int dx = -3; dx < 3; dx++)
	{
		for (int dy = -3; dy < 3; dy++)
		{
			long diffScore = 0;
			int numPixelsInDiff = 0;
			for (int x = 0; x < 7; x++)
			{
				for (int y = 0; y < 7; y++)
				{
					int x1 = x + x0 + dx;
					int y1 = y + y0 + dy;

					if (x1 < 0) x1 = 0;
					if (x1 >= width) x1 = width;
					if (y1 < 0) y1 = 0;
					if (y1 >= height) y1 = height;
					numPixelsInDiff++;
					diffScore += abs((int)model[x + 7 * y] - (int)pixels[x1 + width * y1]);
				}
			}

			double weightedDiff = 1.0*diffScore/numPixelsInDiff;
			if (smallestDiff > weightedDiff)
			{
				smallestDiff = weightedDiff;
				*deltaX = dx;
				*deltaY = dy;
			}
		}
	}
	
	for (int x = 0; x < 7; x++)
	{
		for (int y = 0; y < 7; y++)
		{
			int x1 = x + x0 + *deltaX;
			int y1 = y + y0 + *deltaY;

			if (x1 < 0) x1 = 0;
			if (x1 >= width) x1 = width;
			if (y1 < 0) y1 = 0;
			if (y1 >= height) y1 = height;
			
			pixels[x1 + width * y1] = (unsigned int)abs(((int)model[x + 7 * y] - (int)pixels[x1 + width * y1]) / 2 + (int)imageMedian);
		}
	}
}


void RemoveHotPixel(unsigned int* pixels, int width, int height, unsigned int* model, unsigned int xCorner, unsigned int yCorner, unsigned int imageMedian, unsigned int maxPixelValue)
{
	int dx;
	int dy;
	AlignHotPixelModel(pixels, width, height, model, xCorner, yCorner, imageMedian, &dx, &dy);
	
	Convolution_GaussianBlur_Area(pixels, maxPixelValue, xCorner, yCorner, 7, 7, width, height);
}

HRESULT PreProcessingRemoveHotPixels(unsigned int* pixels, int width, int height, unsigned int* model, unsigned int numPixels, unsigned int* xPos, unsigned int* yPos, unsigned int imageMedian, unsigned int maxPixelValue)
{
	for (unsigned int i = 0; i < numPixels; i++)
	{
		if (xPos[i] - 3 < 0 || yPos[i] - 3 < 0) continue;
		
		RemoveHotPixel(pixels, width, height, model, xPos[i] - 3, yPos[i] - 3, imageMedian, maxPixelValue);
	}
}