#include "Tangra.Image.h"
#include "math.h"
#include <cmath>
#include <vector>
#include <algorithm>

unsigned long GetMaxValueForBitPix(long bpp)
{
 if (bpp == 8)
	return 0xFF;
 else if (bpp == 12)
	return 0xFFF;
 else if (bpp == 14)
	return 0x3FFF;
 else if (bpp == 16)
	return 0xFFFF;
 else
	return 0xFFFFFFFF;
}

HRESULT Convolution(unsigned long* data, long bpp, long width, long height, const double* convMatrix, bool cutEdges, bool calculateAverage, unsigned long* average)
{
	if (width > MAX_CONVOLUTION_WIDTH || width > MAX_CONVOLUTION_WIDTH)
		return E_FAIL;
		
	//uint[] result = new uint[cutEdges ? (width - 1) * (height - 1) : data.Length];

	unsigned long maxValue = GetMaxValueForBitPix(bpp);

	long nPixel;
	double Pixel;
	double sum = 0;
	const double FOUR_PIXEL_FACTOR = 9.0 / 4.0;
	const double SIX_PIXEL_FACTOR = 9.0 / 6.0;

	double convTopLeft = convMatrix[0];
	double convTopMid = convMatrix[1];
	double convTopRight = convMatrix[2];
	double convMidLeft = convMatrix[3];
	double convPixel = convMatrix[4];
	double convMidRight = convMatrix[5];
	double convBottomLeft = convMatrix[6];
	double convBottomMid = convMatrix[7];
	double convBottomRight = convMatrix[8];
	
	for (int y = 0; y < height; ++y)
	{
		for (int x = 0; x < width; ++x)
		{

			if (cutEdges && (x == 0 || y == 0 || x == width - 1 || y == height - 1))
				continue;

			if (y == 0 && x == 0)
			{
				// . . .
				// . # #
				// . # #
				Pixel = ((
											(data[0] * convPixel) +
											(data[1] * convMidRight) +
											(data[width] * convBottomMid) +
											(data[width + 1] * convBottomRight)
											) * FOUR_PIXEL_FACTOR);
			}
			else if (y == height - 1 && x == 0)
			{
				// . # #
				// . # #
				// . . .
				Pixel = ((
											(data[width * (height - 2)] * convTopMid) +
											(data[width * (height - 2) + 1] * convTopRight) +
											(data[width * (height - 1)] * convPixel) +
											(data[width * (height - 1) + 1] * convMidRight)
											) * FOUR_PIXEL_FACTOR);
			}
			else if (y == 0 && x == width - 1)
			{
				// . . .
				// # # .
				// # # .
				Pixel = ((
											(data[width - 2] * convMidLeft) +													
											(data[width - 1] * convPixel) +
											(data[2 * width - 2] * convBottomLeft) +
											(data[2 * width - 1] * convBottomMid)
										   ) * FOUR_PIXEL_FACTOR);
			}
			else if (y == height - 1 && x == width - 1)
			{
				// # # .
				// # # .
				// . . .
				Pixel = ((
											(data[width * height - width - 1] * convTopLeft) +
											(data[width * height - width - 2] * convTopMid) +
											(data[width * height - 2] * convMidLeft) +
											(data[width * height - 1] * convPixel)
											) * FOUR_PIXEL_FACTOR);

			}
			else if (y == 0)
			{
				// . . .
				// # # #
				// # # #
				Pixel = ((
											(data[x - 1 + y * width] * convMidLeft) +
											(data[x + y * width] * convPixel) +
											(data[x + 1 + y * width] * convMidRight) +
											(data[x - 1 + (y + 1) * width] * convBottomLeft) +
											(data[x + (y + 1) * width] * convBottomMid) +
											(data[x + 1 + (y + 1) * width] * convBottomRight)
											) * SIX_PIXEL_FACTOR);
			}
			else if (x == 0)
			{
				// . # #
				// . # #
				// . # #
				Pixel = ((
											(data[x + (y - 1) * width] * convTopMid) +
											(data[x + 1 + (y - 1) * width] * convTopRight) +
											(data[x + y * width] * convPixel) +
											(data[x + 1 + y * width] * convMidRight) +
											(data[x + (y + 1) * width] * convBottomMid) +
											(data[x + 1 + (y + 1) * width] * convBottomRight)
											) * SIX_PIXEL_FACTOR);
			}
			else if (y == height - 1)
			{
				// # # #
				// # # #
				// . . .
				Pixel = ((  
											(data[x - 1 + (y - 1) * width] * convTopLeft) +
											(data[x + (y - 1) * width] * convTopMid) +
											(data[x + 1 + (y - 1) * width] * convTopRight) +
											(data[x - 1 + y * width] * convMidLeft) +
											(data[x + y * width] * convPixel) +
											(data[x + 1 + y * width] * convMidRight)
											) * SIX_PIXEL_FACTOR);
			}
			else if (x == width - 1)
			{
				// # # .
				// # # .
				// # # .
				Pixel = ((
											(data[x - 1 + (y - 1) * width] * convTopLeft) +
											(data[x + (y - 1) * width] * convTopMid) +
											(data[x - 1 + y * width] * convMidLeft) +
											(data[x + y * width] * convPixel) +
											(data[x - 1 + (y + 1) * width] * convBottomLeft) +
											(data[x + (y + 1) * width] * convBottomMid)
											) * SIX_PIXEL_FACTOR);
			}
			else
			{
				// # # #
				// # # #
				// # # #
				Pixel = (
											(data[x - 1 + (y - 1) * width] * convTopLeft) +
											(data[x + (y - 1) * width] * convTopMid) +
											(data[x + 1 + (y - 1) * width] * convTopRight) +
											(data[x - 1 + y * width] * convMidLeft) +
											(data[x + y * width] * convPixel) +
											(data[x + 1 + y * width] * convMidRight) +
											(data[x - 1 + (y + 1) * width] * convBottomLeft) +
											(data[x + (y + 1) * width] * convBottomMid) +
											(data[x + 1 + (y + 1) * width] * convBottomRight));						
			}

			nPixel = (unsigned long)round(Pixel);
			
			if (cutEdges)
			{
				if (nPixel < 0)
					RESULT_BUFFER[(x - 1) + (y - 1) * width] = 0;
				else if (nPixel > maxValue)
					RESULT_BUFFER[(x - 1) + (y - 1) * width] = maxValue;
				else
					RESULT_BUFFER[(x - 1) + (y - 1) * width] = (unsigned long)nPixel;

				if (calculateAverage)
					sum += RESULT_BUFFER[(x - 1) + (y - 1) * width];
			}
			else
			{
				if (nPixel < 0)
					RESULT_BUFFER[x + y * width] = 0;
				else if (nPixel > maxValue)
					RESULT_BUFFER[x + y * width] = maxValue;
				else
					RESULT_BUFFER[x + y * width] = (unsigned long)nPixel;

				if (calculateAverage)
					sum += RESULT_BUFFER[x + y * width];						
			}
		}
	}

	long totalPixels = cutEdges ? (width - 1) * (height - 1) : width * height;
	
	if (calculateAverage)
		*average = (unsigned long)round(sum / totalPixels);

	memcpy(&data[0], &RESULT_BUFFER[0], totalPixels * sizeof(unsigned long));
	
	return S_OK;	
}

void Convolution_GaussianBlur(unsigned long* pixels, long bpp, long width, long height)
{
	Convolution(pixels, bpp, width, height, GAUSSIAN_BLUR_MATRIX, false, false, NULL);
}

void Convolution_Sharpen(unsigned long* pixels, long bpp, long width, long height, unsigned long* average)
{
	Convolution(pixels, bpp, width, height, SHARPEN_MATRIX, false, true, average);
}

void Convolution_Denoise(unsigned long* pixels, long bpp, long width, long height, unsigned long* average, bool cutEdges)
{
	Convolution(pixels, bpp, width, height, DENOISE_MATRIX, cutEdges, true, average);
}

HRESULT PrepareImageForOCR(unsigned long* pixels, long bpp, long width, long height)
{
	long size = width * height;
	
	// Dark median correct
	std::vector<unsigned long> pixelsVec;
	pixelsVec.insert(pixelsVec.end(), &pixels[0], &pixels[size]);
	size_t n = pixelsVec.size() / 2;
    std::nth_element(pixelsVec.begin(), pixelsVec.begin() + n, pixelsVec.end());
    long median = pixelsVec[n];
	
	for (int i = 0; i < size; i++)
	{
		long darkCorrectedValue = (long) pixels[i] - (int) median;
		if (darkCorrectedValue < 0) darkCorrectedValue = 0;
		pixels[i] = (unsigned long) darkCorrectedValue;
	}

	// Blur
	Convolution_GaussianBlur(pixels, 8, width, height);
	
	// Sharpen
	unsigned long average = 128;
	Convolution_Sharpen(pixels, 8, width, height, &average);

	// Binerize and Inverse
	for (int i = 0; i < size; i++)
	{
		pixels[i] = pixels[i] > average ? (unsigned long)0 : (unsigned long)255;
	}
	
	// Denoise
	Convolution_Denoise(pixels, 8, width, height, &average, false);


	// Binerize again (after the Denise)
	for (int i = 0; i < size; i++)
	{
		pixels[i] = pixels[i] < 127 ? (unsigned long)0 : (unsigned long)255;
	}
	
	return S_OK;
}

HRESULT PrepareImageForOCRSingleStep(unsigned long* pixels, long bpp, long width, long height, long stepNo, unsigned long* average)
{
	long size = width * height;
	
	if (stepNo == 0)
	{
		// Dark median correct
		std::vector<unsigned long> pixelsVec;
		pixelsVec.insert(pixelsVec.end(), &pixels[0], &pixels[size]);
		size_t n = pixelsVec.size() / 2;
		std::nth_element(pixelsVec.begin(), pixelsVec.begin() + n, pixelsVec.end());
		long median = pixelsVec[n];
		
		for (int i = 0; i < size; i++)
		{
			long darkCorrectedValue = (long) pixels[i] - (int) median;
			if (darkCorrectedValue < 0) darkCorrectedValue = 0;
			pixels[i] = (unsigned long) darkCorrectedValue;
		}
	}

	if (stepNo == 1)
		// Blur
		Convolution_GaussianBlur(pixels, 8, width, height);

	if (stepNo == 2)
	{
		*average = 128;

		// Sharpen
		Convolution_Sharpen(pixels, 8, width, height, average);
	}
	
	if (stepNo == 3)
	{
		// Binerize and Inverse
		for (int i = 0; i < size; i++)
		{
			pixels[i] = pixels[i] > *average ? (unsigned long)0 : (unsigned long)255;
		}		
	}
	
	if (stepNo == 4)
	{
		*average = 128;
		
		// Denoise
		Convolution_Denoise(pixels, 8, width, height, average, false);
	}
	
	if (stepNo == 5)
	{
		// Binerize again (after the Denise)
		for (int i = 0; i < size; i++)
		{
			pixels[i] = pixels[i] < 127 ? (unsigned long)0 : (unsigned long)255;
		}		
	}
	
	return S_OK;	
}