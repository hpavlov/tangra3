#include <vector>
#include "stdlib.h"
#include <strings.h>
#include <cstring>
#include "IntegrationUtils.h"

double* s_IntegratedValues = NULL;
std::vector<long>* s_MedianPixelLists = NULL;

bool s_IsMedianAveraging;
int s_Width;
int s_Height;
int s_FrameCount;

int IntegrationManagerGetFirstFrameToIntegrate(int producedFirstFrame, int frameCount, bool isSlidingIntegration)
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

void IntergationManagerStartNew(int width, int height, bool isMedianAveraging)
{
	s_IntegratedValues = (double*)malloc(width * height * sizeof(double));
	memset(s_IntegratedValues, 0, width * height * sizeof(double));

	s_IsMedianAveraging = isMedianAveraging;
	s_FrameCount = 0;
	s_Width = width;
	s_Height = height;

	if (isMedianAveraging)
	{
		// TODO: Which STL collection implements a fast sort method to produce the medians??

		std::vector<long>* pMedianPixelLists = s_MedianPixelLists;
		for(int i = 0; i < width * height; i++)
		{
			//*pMedianPixelLists = new std::vector<long>();
			//pMedianPixelLists++;
		}
	}
}

void IntegrationManagerAddFrame(unsigned long* framePixels)
{
	double* pIntegrated = s_IntegratedValues;
	unsigned long* pPixels = framePixels;

	if (!s_IsMedianAveraging)
	{
		for(int x = 0; x < s_Width; x++)
		for(int y = 0; y < s_Height; y++)
		{
			*pIntegrated++ += *pPixels++;
		}
	}
	else
	{
		// Copy the individual pixels into lists of long for each byte
	}

	s_FrameCount++;
}


void IntegrationManagerAddFrameEx(unsigned long* framePixels, bool isLittleEndian, int bpp)
{
	double* pIntegrated = s_IntegratedValues;
	unsigned long* pPixels = framePixels;

	if (!s_IsMedianAveraging)
	{
		for(int x = 0; x < s_Width; x++)
		for(int y = 0; y < s_Height; y++)
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
			for(int x = 0; x < s_Width; x++)
			for(int y = 0; y < s_Height; y++)
			{
				*pPixels++ = (unsigned long)(*pIntegrated++ / s_FrameCount);
			}
		}
	}	
}

void IntegrationManagerFreeResources()
{
	if (s_IntegratedValues != NULL)
		delete s_IntegratedValues;

	s_IntegratedValues = NULL;
}