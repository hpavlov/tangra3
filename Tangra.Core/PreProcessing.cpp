#include "cross_platform.h"
#include "PreProcessing.h"
#include "PixelMapUtils.h"


PreProcessingType s_PreProcessingType;
PreProcessingFilter s_PreProcessingFilter;
unsigned int  g_PreProcessingFromValue;
unsigned int  g_PreProcessingToValue;
long g_PreProcessingBrigtness;
long g_PreProcessingContrast;
float g_EncodingGamma;
bool g_UsesPreProcessing;

long* g_DarkFramePixelsCopy = NULL;
long* g_FlatFramePixelsCopy = NULL;
long g_FlatFrameMedian = 0;
unsigned int g_DarkFramePixelsCount = 0;
unsigned int g_FlatFramePixelsCount = 0;

bool UsesPreProcessing()
{
	return g_UsesPreProcessing;
}

long PreProcessingClearAll()
{
	s_PreProcessingType = pptpNone;
	g_PreProcessingFromValue = 0;
	g_PreProcessingToValue = 0;
	g_PreProcessingBrigtness = 0;
	g_PreProcessingContrast = 0;
	g_DarkFramePixelsCount = 0;
	g_FlatFramePixelsCount = 0;
	g_UsesPreProcessing = false;

	s_PreProcessingFilter = ppfNoFilter;
	g_EncodingGamma = 1;

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

	return S_OK;
}

long PreProcessingUsesPreProcessing(bool* usesPreProcessing)
{
	*usesPreProcessing = g_UsesPreProcessing;

	return S_OK;
}

long PreProcessingGetConfig(PreProcessingType* preProcessingType, unsigned int* fromValue, unsigned int* toValue, long* brigtness, long* contrast, PreProcessingFilter* filter, float* gamma, unsigned int* darkPixelsCount, unsigned int* flatPixelsCount)
{
	if (g_UsesPreProcessing)
	{
		*preProcessingType = s_PreProcessingType;
		*fromValue = g_PreProcessingFromValue;
		*toValue = g_PreProcessingToValue;
		*brigtness = g_PreProcessingBrigtness;
		*contrast = g_PreProcessingContrast;
		*darkPixelsCount = g_DarkFramePixelsCount;
		*flatPixelsCount = g_FlatFramePixelsCount;

		*filter = s_PreProcessingFilter;
		*gamma = g_EncodingGamma = 1;
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
	g_UsesPreProcessing = true;
	
	return S_OK;
}

long PreProcessingAddDarkFrame(long* darkFramePixels, unsigned int pixelsCount)
{
	if (NULL != g_DarkFramePixelsCopy)
	{
		delete g_DarkFramePixelsCopy;
		g_DarkFramePixelsCopy = NULL;
	}

	g_DarkFramePixelsCopy = (long*)malloc(pixelsCount * sizeof(long));
	memcpy(g_DarkFramePixelsCopy, darkFramePixels, pixelsCount);
	g_DarkFramePixelsCount = pixelsCount;

	g_UsesPreProcessing = true;

	return S_OK;
}

long PreProcessingAddFlatFrame(long* flatFramePixels, unsigned int pixelsCount, unsigned long flatFrameMedian)
{
	if (NULL != g_FlatFramePixelsCopy)
	{
		delete g_FlatFramePixelsCopy;
		g_FlatFramePixelsCopy = NULL;
	}

	g_FlatFramePixelsCopy = (long*)malloc(pixelsCount * sizeof(long));
	memcpy(g_FlatFramePixelsCopy, flatFramePixels, pixelsCount);
	g_FlatFramePixelsCount = pixelsCount;
	g_FlatFrameMedian = flatFrameMedian;

	g_UsesPreProcessing = true;

	return S_OK;
}


long ApplyPreProcessing(unsigned long* pixels, long width, long height, int bpp, BYTE* bitmapPixels, BYTE* bitmapBytes)
{
	long rv = ApplyPreProcessingPixelsOnly(pixels, width, height, bpp);
	if (!SUCCEEDED(rv)) return rv;

	return GetBitmapPixels(width, height, pixels, bitmapPixels, bitmapBytes, false, bpp);
}

long ApplyPreProcessingPixelsOnly(unsigned long* pixels, long width, long height, int bpp)
{
	// TODO: What is the order ??
	// (1) Dark/Flat
	// (2) Stretch/Clip/Brightness
	// (3) Gamma

	long rv = S_OK;

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
		// TODO: Apply low pass filter	
	}
	else if (s_PreProcessingFilter == ppfLowPassDifferenceFilter)
	{
		// TODO: Apply low pass difference filter
	}

	return rv;
}

/*
bool Conv3x3(Pixelmap image, ConvMatrix m)
{
    // Avoid divide by zero errors
    if (0 == m.Factor)
        return false;

	var result = new Pixelmap(image.Width, image.Height, image.BitPixCamera);

	for (int y = 0; y < image.Height - 2; ++y)
    {
		for (int x = 0; x < image.Width - 2; ++x)
        {
            ulong nPixel = (ulong) Math.Round((((image[x, y] * m.TopLeft) +
                                        (image[x + 1, y]*m.TopMid) +
                                        (image[x + 2, y]*m.TopRight) +
                                        (image[x, y + 1]*m.MidLeft) +
                                        (image[x + 1, y + 1]*m.Pixel) +
                                        (image[x + 2, y + 1]*m.MidRight) +
                                        (image[x, y + 2]*m.BottomLeft) +
                                        (image[x, y + 2]*m.BottomMid) +
                                        (image[x, y + 2]*m.BottomRight))
                                        /m.Factor) + m.Offset);

            if (nPixel < 0) nPixel = 0;
            if (nPixel > image.MaxPixelValue) nPixel = image.MaxPixelValue;
            result[x + 1, y] = (uint) nPixel;
        }
    }
}
*/