#include "Tangra.Image.h"
#include "math.h"
#include <cmath>
#include <vector>
#include <algorithm>
#include <stack>

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

long* s_CheckedPixels = NULL;
long s_ChunkDenoiseIndex;
long s_ChunkDenoiseWidth;
long s_ChunkDenoiseHeight;
long s_ChunkDenoiseMaxIndex;
long* s_ObjectPixelsIndex = NULL;
long s_ObjectPixelsCount;
long s_ObjectPixelsXFrom;
long s_ObjectPixelsXTo;
long s_ObjectPixelsYFrom;
long s_ObjectPixelsYTo;
long s_MaxLowerBoundNoiseChunkPixels;
long s_MinUpperBoundNoiseChunkPixels;
long s_MinLowerBoundNoiseChunkHeight;
long s_MaxUpperBoundNoiseChunkWidth;
std::stack<long> s_ObjectPixelsPath;

void SetObjectPixelsXFromTo(long pixelIndex)
{
	long width = pixelIndex % s_ChunkDenoiseWidth;
	long height = pixelIndex / s_ChunkDenoiseWidth;
	
	if (s_ObjectPixelsXFrom > width) s_ObjectPixelsXFrom = width;
	if (s_ObjectPixelsXTo < width) s_ObjectPixelsXTo = width;
	if (s_ObjectPixelsYFrom > height) s_ObjectPixelsYFrom = height;
	if (s_ObjectPixelsYTo < height) s_ObjectPixelsYTo = height;	
}
			
void EnsureChunkDenoiseBuffers(long width, long height)
{
	if (s_ChunkDenoiseWidth != width || s_ChunkDenoiseHeight != height)
	{
		if (NULL == s_CheckedPixels)
		{
			delete s_CheckedPixels;
			s_CheckedPixels = NULL;
		}
		
		if (NULL == s_ObjectPixelsIndex)
		{
			delete s_ObjectPixelsIndex;
			s_ObjectPixelsIndex = NULL;
		}
		
		s_ChunkDenoiseWidth = width;
		s_ChunkDenoiseHeight = height;
		s_ChunkDenoiseMaxIndex = width * height;		
		s_CheckedPixels = (long*)malloc(sizeof(long) * s_ChunkDenoiseMaxIndex);
		s_ObjectPixelsIndex = (long*)malloc(sizeof(long) * s_ChunkDenoiseMaxIndex);
		
		// The max noise chink part to be removed is 50% of the pixels in "1".
		// This value is determined experimentally and varied based on the area hight
       // Block(22x16), AreaHeight(20) -> '1' is 70 pixels (50% = 35 pixels)
		s_MaxLowerBoundNoiseChunkPixels = (long)round(35.0 * height / 20);
		// The min noise chunk is 80% of a fully black square
		s_MinUpperBoundNoiseChunkPixels = (long)round(0.8 * width * height);
		s_MinLowerBoundNoiseChunkHeight = (long)round(0.5 * (height - 4));
		s_MaxUpperBoundNoiseChunkWidth = height;
	}
}

#define UNCHECKED 0
#define WENT_UP 1
#define WENT_LEFT 2
#define WENT_RIGHT 3
#define WENT_DOWN 4
#define CHECKED 5

long FindNextObjectPixel(unsigned long* pixels, unsigned long onColour)
{
	while (s_ChunkDenoiseIndex < s_ChunkDenoiseMaxIndex - 1)
	{
		s_ChunkDenoiseIndex++;
		if (s_CheckedPixels[s_ChunkDenoiseIndex] == UNCHECKED)
		{
			if (pixels[s_ChunkDenoiseIndex] != onColour)
				s_CheckedPixels[s_ChunkDenoiseIndex] = CHECKED;
			else
				return s_ChunkDenoiseIndex;
		}
	}

	return -1;	
}

bool ProcessNoiseObjectPixel(unsigned long* pixels, long* pixelRef, unsigned long onColour)
{
	long pixel = *pixelRef;
	long x = pixel % s_ChunkDenoiseWidth;
	long y = pixel / s_ChunkDenoiseWidth;
	long width = s_ChunkDenoiseWidth;
	long nextPixel;

	if (s_CheckedPixels[pixel] == UNCHECKED)
	{
		nextPixel = (y - 1) * width + x;

		s_CheckedPixels[pixel] = WENT_UP;

		if (y > 0)
		{
			if (s_CheckedPixels[nextPixel] == UNCHECKED)
			{
				if (pixels[nextPixel] == onColour)
				{
					s_ObjectPixelsPath.push(nextPixel);
					*pixelRef = nextPixel;
					s_ObjectPixelsIndex[s_ObjectPixelsCount] = nextPixel;
					s_ObjectPixelsCount++;
					SetObjectPixelsXFromTo(nextPixel);
				}
				else
					s_CheckedPixels[nextPixel] = CHECKED;
			}
		}
		
		return true;
	}
	else if (s_CheckedPixels[pixel] == WENT_UP)
	{
		nextPixel = y * width + (x - 1);

		s_CheckedPixels[pixel] = WENT_LEFT;

		if (x > 0)
		{
			if (s_CheckedPixels[nextPixel] == UNCHECKED)
			{
				if (pixels[nextPixel] == onColour)
				{
					s_ObjectPixelsPath.push(nextPixel);
					*pixelRef = nextPixel;
					s_ObjectPixelsIndex[s_ObjectPixelsCount] = nextPixel;
					s_ObjectPixelsCount++;
					SetObjectPixelsXFromTo(nextPixel);
				}
				else
					s_CheckedPixels[nextPixel] = CHECKED;
			}
		}
		
		return true;
	}
	else if (s_CheckedPixels[pixel] == WENT_LEFT)
	{
		nextPixel = y * width + (x + 1);

		s_CheckedPixels[pixel] = WENT_RIGHT;

		if (x < width - 1)
		{
			if (s_CheckedPixels[nextPixel] == UNCHECKED)
			{
				if (pixels[nextPixel] == onColour)
				{
					s_ObjectPixelsPath.push(nextPixel);
					*pixelRef = nextPixel;
					s_ObjectPixelsIndex[s_ObjectPixelsCount] = nextPixel;
					s_ObjectPixelsCount++;
					SetObjectPixelsXFromTo(nextPixel);
				}
				else
					s_CheckedPixels[nextPixel] = CHECKED;
			}
		}
	
		return true;
	}
	else if (s_CheckedPixels[pixel] == WENT_RIGHT)
	{
		nextPixel = (y + 1) * width + x;

		s_CheckedPixels[pixel] = WENT_DOWN;

		if (y < s_ChunkDenoiseHeight - 1)
		{
			if (s_CheckedPixels[nextPixel] == UNCHECKED)
			{
				if (pixels[nextPixel] == onColour)
				{
					s_ObjectPixelsPath.push(nextPixel);
					*pixelRef = nextPixel;
					s_ObjectPixelsIndex[s_ObjectPixelsCount] = nextPixel;
					s_ObjectPixelsCount++;
					SetObjectPixelsXFromTo(nextPixel);
				}
				else
					s_CheckedPixels[nextPixel] = CHECKED;
			}
		}

		return true;
	}
	else if (s_CheckedPixels[pixel] >= WENT_DOWN)
	{
		if (s_ObjectPixelsPath.empty())
			return false;

		s_CheckedPixels[pixel] = CHECKED;

		nextPixel = s_ObjectPixelsPath.top();
		s_ObjectPixelsPath.pop();

		if (pixel == nextPixel && !s_ObjectPixelsPath.empty())
		{
			nextPixel = s_ObjectPixelsPath.top();
			s_ObjectPixelsPath.pop();
		}

		*pixelRef = nextPixel;
		return true;
	}
	else
		return false;	
}

bool CurrentObjectChunkIsNoise()
{
	return 
		s_ObjectPixelsCount < s_MaxLowerBoundNoiseChunkPixels ||
		s_ObjectPixelsCount > s_MinUpperBoundNoiseChunkPixels ||
		(s_ObjectPixelsYTo - s_ObjectPixelsYFrom) < s_MinLowerBoundNoiseChunkHeight ||
		(s_ObjectPixelsXTo - s_ObjectPixelsXFrom) > s_MaxUpperBoundNoiseChunkWidth;
}

long CheckAndRemoveNoiseObjectAsNecessary(unsigned long* pixels, long firstPixel, unsigned long onColour, unsigned long offColour)
{
	s_ObjectPixelsCount = 0;
	s_ObjectPixelsXFrom = 0xFFFF;
	s_ObjectPixelsXTo = 0;
	s_ObjectPixelsYFrom = 0xFFFF;
	s_ObjectPixelsYTo = 0;
	while(!s_ObjectPixelsPath.empty()) s_ObjectPixelsPath.pop();
	s_ObjectPixelsPath.push(firstPixel);

	long currPixel = firstPixel;

	s_ObjectPixelsIndex[s_ObjectPixelsCount] = firstPixel;
	s_ObjectPixelsCount++;
	SetObjectPixelsXFromTo(firstPixel);

	while (ProcessNoiseObjectPixel(pixels, &currPixel, onColour))
	{ }

	if (CurrentObjectChunkIsNoise())
	{
		for (int i = 0; i < s_ObjectPixelsCount; i++)
		{
			pixels[s_ObjectPixelsIndex[i]] = offColour;
		}
	}	
}

HRESULT LargeChunkDenoise(unsigned long* pixels, long width, long height, unsigned long onColour, unsigned long offColour)
{
	EnsureChunkDenoiseBuffers(width, height);
	
	s_ObjectPixelsCount = -1;
	s_ChunkDenoiseIndex = -1;
	
	memset(s_CheckedPixels, 0, s_ChunkDenoiseMaxIndex * sizeof(long));
	
	do
	{
		long nextObjectPixelId = FindNextObjectPixel(pixels, onColour);

		if (nextObjectPixelId != -1)
			CheckAndRemoveNoiseObjectAsNecessary(pixels, nextObjectPixelId, onColour, offColour);
		else
			break;

	} 
	while (true);	
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
	
	LargeChunkDenoise(pixels, width, height, 0, 255);
	
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