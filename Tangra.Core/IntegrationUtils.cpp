/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#include <vector>
#include "stdlib.h"
#include <strings.h>
#include <cstring>
#include "IntegrationUtils.h"
#include <algorithm>

double* s_IntegratedValues = NULL;
std::vector< std::vector<unsigned long>* > s_MedianPixelLists;

bool s_IsMedianAveraging;
long s_Width;
long s_Height;
long s_FrameCount;

long IntegrationManagerGetFirstFrameToIntegrate(long producedFirstFrame, long frameCount, bool isSlidingIntegration)
{
	if (isSlidingIntegration)
	{
		return producedFirstFrame;
	}
	else
	{
		// Return the first frame from the current sequential batch of [frameCount] frames
		return producedFirstFrame / frameCount;
	}
}

void IntergationManagerStartNew(long width, long height, bool isMedianAveraging)
{
	s_IntegratedValues = (double*)malloc(width * height * sizeof(double));
	memset(s_IntegratedValues, 0, width * height * sizeof(double));

	s_IsMedianAveraging = isMedianAveraging;
	s_FrameCount = 0;
	s_Width = width;
	s_Height = height;

	if (isMedianAveraging)
	{
		s_MedianPixelLists.clear();
		for(long i = 0; i < width * height; i++)
		{
			s_MedianPixelLists.push_back(new std::vector<unsigned long>());
		}
	}
}

void IntegrationManagerAddFrame(unsigned long* framePixels)
{
	double* pIntegrated = s_IntegratedValues;
	unsigned long* pPixels = framePixels;

	if (!s_IsMedianAveraging)
	{
		for(long x = 0; x < s_Width; x++)
		for(long y = 0; y < s_Height; y++)
		{
			*pIntegrated++ += *pPixels++;
		}
	}
	else
	{
		long idx = -1;
		for(long x = 0; x < s_Width; x++)
		for(long y = 0; y < s_Height; y++)
		{
			s_MedianPixelLists[++idx]->push_back(*pPixels++);
		}		
	}

	s_FrameCount++;
}


void IntegrationManagerAddFrameEx(unsigned long* framePixels, bool isLittleEndian, long bpp)
{
	double* pIntegrated = s_IntegratedValues;
	unsigned long* pPixels = framePixels;

	if (!s_IsMedianAveraging)
	{
		for(long x = 0; x < s_Width; x++)
		for(long y = 0; y < s_Height; y++)
		{
			if (isLittleEndian)
			{
				unsigned long littleEndianValue = *pPixels++;
				unsigned long bigEndianValue = 0;
				if (bpp == 16)
				{
					bigEndianValue = ((littleEndianValue & 0xFF) << 8) + ((littleEndianValue & 0xFF00) >> 8);

					*pIntegrated++ += bigEndianValue;
				}
				else
					*pIntegrated++ += littleEndianValue;
				
			}
			else
			{
				*pIntegrated++ += *pPixels++;
			}			
		}
	}
	else
	{
		long idx = -1;
		for(long x = 0; x < s_Width; x++)
		for(long y = 0; y < s_Height; y++)
		{
			unsigned long pixelValue;
			if (isLittleEndian)
			{
				unsigned long littleEndianValue = *pPixels++;
				unsigned long bigEndianValue = 0;
				if (bpp == 16)
				{
					bigEndianValue = ((littleEndianValue & 0xFF) << 8) + ((littleEndianValue & 0xFF00) >> 8);

					pixelValue = bigEndianValue;
				}
				else
					pixelValue = littleEndianValue;
				
			}
			else
			{
				pixelValue = *pPixels++;
			}
			
			s_MedianPixelLists[++idx]->push_back(pixelValue);
		}
	}	

	s_FrameCount++;
}

void IntegrationManagerProduceIntegratedFrame(unsigned long* framePixels)
{
	double* pIntegrated = s_IntegratedValues;
	unsigned long* pPixels = framePixels;

	if (!s_IsMedianAveraging)
	{
		if (s_FrameCount > 0)
		{
			for(long x = 0; x < s_Width; x++)
			for(long y = 0; y < s_Height; y++)
			{
				*pPixels++ = (unsigned long)(*pIntegrated++ / s_FrameCount);
			}
		}
	}
	else
	{
		if (s_FrameCount > 0)
		{
			long idx = -1;
			for(long x = 0; x < s_Width; x++)
			for(long y = 0; y < s_Height; y++)
			{
				idx++;
				std::vector<unsigned long>* pixelsVec = s_MedianPixelLists[idx];
				size_t n = pixelsVec->size() / 2;
				std::nth_element(pixelsVec->begin(), pixelsVec->begin() + n, pixelsVec->end());
				unsigned long median = (*pixelsVec)[n];				
				*pPixels++ = median;
			}
		}			
	}
}

void IntegrationManagerFreeResources()
{
	if (s_IntegratedValues != NULL)
		delete s_IntegratedValues;

	s_IntegratedValues = NULL;
	
	for(long i = 0; i < s_MedianPixelLists.size(); i++)
	{
		delete s_MedianPixelLists[i];
		s_MedianPixelLists[i] = NULL;
	}
	
	s_MedianPixelLists.clear();
}