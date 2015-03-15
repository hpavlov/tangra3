/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#include "cross_platform.h"
#include "PreProcessing.h"
#include "PixelMapUtils.h"


PreProcessingType s_PreProcessingType;
PreProcessingFilter s_PreProcessingFilter;
RotateFlipType g_RotateFlipType;
unsigned int  g_PreProcessingFromValue;
unsigned int  g_PreProcessingToValue;
long g_PreProcessingBrigtness;
long g_PreProcessingContrast;
float g_EncodingGamma;
bool g_UsesPreProcessing = false;

float* g_DarkFramePixelsCopy = NULL;
float* g_FlatFramePixelsCopy = NULL;
float* g_BiasFramePixelsCopy = NULL;
float g_FlatFrameMedian = 0;
float g_DarkFrameExposure = 0;
unsigned int g_DarkFramePixelsCount = 0;
unsigned int g_FlatFramePixelsCount = 0;
unsigned int g_BiasFramePixelsCount = 0;

float ABS(float x)
{
	if (x < 0)
		return -x;
	return x;
}

bool UsesPreProcessing()
{
	return g_UsesPreProcessing;
}

long PreProcessingClearAll()
{
	s_PreProcessingType = pptpNone;
	g_RotateFlipType = RotateNoneFlipNone;
	g_PreProcessingFromValue = 0;
	g_PreProcessingToValue = 0;
	g_PreProcessingBrigtness = 0;
	g_PreProcessingContrast = 0;
	g_DarkFramePixelsCount = 0;
	g_FlatFramePixelsCount = 0;
	g_BiasFramePixelsCount = 0;
	g_UsesPreProcessing = false;

	s_PreProcessingFilter = ppfNoFilter;
	g_EncodingGamma = 1;

	if (NULL != g_BiasFramePixelsCopy)
	{
		delete g_BiasFramePixelsCopy;
		g_BiasFramePixelsCopy = NULL;
	}

	if (NULL != g_DarkFramePixelsCopy)
	{
		delete g_DarkFramePixelsCopy;
		g_DarkFramePixelsCopy = NULL;
	}

	if (NULL != g_FlatFramePixelsCopy)
	{
		delete g_FlatFramePixelsCopy;
		g_FlatFramePixelsCopy = NULL;
	}

	g_FlatFrameMedian = 0;
	g_DarkFrameExposure = 0;

	return S_OK;
}

long PreProcessingUsesPreProcessing(bool* usesPreProcessing)
{
	*usesPreProcessing = g_UsesPreProcessing;

	return S_OK;
}

long PreProcessingGetConfig(PreProcessingType* preProcessingType, unsigned int* fromValue, unsigned int* toValue, long* brigtness, long* contrast, PreProcessingFilter* filter, float* gamma, unsigned int* darkPixelsCount, unsigned int* flatPixelsCount, unsigned int* biasPixelsCount, RotateFlipType* rotateFlipType)
{
	if (g_UsesPreProcessing)
	{
		*preProcessingType = s_PreProcessingType;
		*rotateFlipType = g_RotateFlipType;
		*fromValue = g_PreProcessingFromValue;
		*toValue = g_PreProcessingToValue;
		*brigtness = g_PreProcessingBrigtness;
		*contrast = g_PreProcessingContrast;
		*darkPixelsCount = g_DarkFramePixelsCount;
		*flatPixelsCount = g_FlatFramePixelsCount;
		*biasPixelsCount = g_BiasFramePixelsCount;

		*filter = s_PreProcessingFilter;
		*gamma = g_EncodingGamma;
	}

	return S_OK;
}

long PreProcessingAddStretching(unsigned int fromValue, unsigned int toValue)
{
	s_PreProcessingType = pptpStretching;
	g_PreProcessingFromValue = fromValue;
	g_PreProcessingToValue = toValue;
	g_UsesPreProcessing = true;

	return S_OK;
}

long PreProcessingAddClipping(unsigned int  fromValue, unsigned int  toValue)
{
	s_PreProcessingType = pptpClipping;
	g_PreProcessingFromValue = fromValue;
	g_PreProcessingToValue = toValue;
	g_UsesPreProcessing = true;

	return S_OK;
}

long PreProcessingAddBrightnessContrast(long brigtness, long contrast)
{
	s_PreProcessingType = pptpBrightnessContrast;
	g_PreProcessingBrigtness = brigtness;
	g_PreProcessingContrast = contrast;
	g_UsesPreProcessing = true;

	return S_OK;
}

long PreProcessingAddDigitalFilter(enum PreProcessingFilter filter)
{
	s_PreProcessingFilter = filter;
	g_UsesPreProcessing = true;

	return S_OK;
}

long PreProcessingAddGammaCorrection(float gamma)
{
	g_EncodingGamma = gamma;
	g_UsesPreProcessing = g_UsesPreProcessing || ABS(g_EncodingGamma - 1.0f) > 0.01;
	
	return S_OK;
}

long PreProcessingAddFlipAndRotation(enum RotateFlipType rotateFlipType)
{
	g_RotateFlipType = rotateFlipType;
	g_UsesPreProcessing = true;
	
	return S_OK;	
}

long PreProcessingAddDarkFrame(float* darkFramePixels, unsigned long pixelsCount, float exposureSeconds)
{
	if (NULL != g_DarkFramePixelsCopy)
	{
		delete g_DarkFramePixelsCopy;
		g_DarkFramePixelsCopy = NULL;
	}

	long bytesCount = pixelsCount * sizeof(float);
	
	g_DarkFramePixelsCopy = (float*)malloc(bytesCount);
	memcpy(g_DarkFramePixelsCopy, darkFramePixels, bytesCount);
	g_DarkFramePixelsCount = pixelsCount;
	g_DarkFrameExposure = exposureSeconds;

	g_UsesPreProcessing = true;

	return S_OK;
}

long PreProcessingAddBiasFrame(float* biasFramePixels, unsigned long pixelsCount)
{
	if (NULL != g_BiasFramePixelsCopy)
	{
		delete g_BiasFramePixelsCopy;
		g_BiasFramePixelsCopy = NULL;
	}

	long bytesCount = pixelsCount * sizeof(float);
	
	g_BiasFramePixelsCopy = (float*)malloc(bytesCount);
	memcpy(g_BiasFramePixelsCopy, biasFramePixels, bytesCount);
	g_BiasFramePixelsCount = pixelsCount;

	g_UsesPreProcessing = true;

	return S_OK;
}

long PreProcessingAddFlatFrame(float* flatFramePixels, unsigned long pixelsCount, float flatFrameMedian)
{
	if (NULL != g_FlatFramePixelsCopy)
	{
		delete g_FlatFramePixelsCopy;
		g_FlatFramePixelsCopy = NULL;
	}

	long bytesCount = pixelsCount * sizeof(float);

	g_FlatFramePixelsCopy = (float*)malloc(bytesCount);
	memcpy(g_FlatFramePixelsCopy, flatFramePixels, bytesCount);
	g_FlatFramePixelsCount = pixelsCount;
	g_FlatFrameMedian = flatFrameMedian;

	g_UsesPreProcessing = true;

	return S_OK;
}


long ApplyPreProcessingWithNormalValue(unsigned long* pixels, long width, long height, int bpp, float exposureSeconds, unsigned long normVal, BYTE* bitmapPixels, BYTE* bitmapBytes)
{
	long rv = ApplyPreProcessingPixelsOnly(pixels, width, height, bpp, exposureSeconds);
	if (!SUCCEEDED(rv)) return rv;

	return GetBitmapPixels(width, height, pixels, bitmapPixels, bitmapBytes, false, bpp, normVal);
}

long ApplyPreProcessing(unsigned long* pixels, long width, long height, int bpp, float exposureSeconds, BYTE* bitmapPixels, BYTE* bitmapBytes)
{
	long rv = ApplyPreProcessingPixelsOnly(pixels, width, height, bpp, exposureSeconds);
	if (!SUCCEEDED(rv)) return rv;

	return GetBitmapPixels(width, height, pixels, bitmapPixels, bitmapBytes, false, bpp, 0);
}

long ApplyPreProcessingPixelsOnly(unsigned long* pixels, long width, long height, int bpp, float exposureSeconds)
{
	// Use the following order when applying pre-processing
	// (1) Bias/Dark/Flat
	// (2) Stretch/Clip/Brightness
	// (3) Gamma

	long rv = S_OK;

	if (NULL != g_BiasFramePixelsCopy || NULL != g_DarkFramePixelsCopy || NULL != g_FlatFramePixelsCopy)
	{
		rv = PreProcessingApplyBiasDarkFlatFrame(
			pixels, width, height, bpp, 
			g_BiasFramePixelsCopy, g_DarkFramePixelsCopy, g_FlatFramePixelsCopy, 
			exposureSeconds, g_DarkFrameExposure, g_FlatFrameMedian);
			
		if (rv != S_OK) return rv;
	}

	if (g_RotateFlipType > 0)
	{
		rv = PreProcessingFlipRotate(pixels, width, height, bpp, g_RotateFlipType); 
		if (rv != S_OK) return rv;
	}
	
	if (s_PreProcessingType == pptpStretching)
	{
		rv = PreProcessingStretch(pixels, width, height, bpp, g_PreProcessingFromValue, g_PreProcessingToValue);
		if (rv != S_OK) return rv;
	}
	else if (s_PreProcessingType == pptpClipping)
	{
		rv = PreProcessingClip(pixels, width, height, bpp, g_PreProcessingFromValue, g_PreProcessingToValue);
		if (rv != S_OK) return rv;
	}
	else if (s_PreProcessingType == pptpBrightnessContrast)
	{
		rv = PreProcessingBrightnessContrast(pixels, width, height, bpp, g_PreProcessingBrigtness, g_PreProcessingContrast);
		if (rv != S_OK) return rv;
	}

	if (s_PreProcessingFilter == ppfLowPassFilter)
	{
		rv = PreProcessingLowPassFilter(pixels, width, height, bpp);
		if (rv != S_OK) return rv;
	}
	else if (s_PreProcessingFilter == ppfLowPassDifferenceFilter)
	{
		rv = PreProcessingLowPassDifferenceFilter(pixels, width, height, bpp);
		if (rv != S_OK) return rv;
	}
	
	if (ABS(g_EncodingGamma - 1.0f) > 0.01)
	{
		rv = PreProcessingGamma(pixels, width, height, bpp, g_EncodingGamma);
		if (rv != S_OK) return rv;
	}

	return rv;
}