#include "PixelMapUtils.h"
#include "CoreContext.h"
#include <stdlib.h>
#include "cross_platform.h"
#include <stdio.h>
#include <string.h>


void CopyPixelsInTriplets(BYTE* pDIB, BITMAPINFOHEADER bih, unsigned long* pixels, BYTE* bitmapPixels, BYTE* bitmapBytes)
{
	long bytesPerPixel = bih.biBitCount / 8;
	long length = (bih.biWidth * bih.biHeight); //bih.biSizeImage / bytesPerPixel;
	BYTE* src = pDIB + sizeof(BITMAPINFOHEADER);

	long width = bih.biWidth;
	long currLinePos = 0;
	pixels+=length + width;
	bitmapBytes+=length + width;

	BYTE val1, val2, val3, val4;
	unsigned long b12, b23, b34;

	unsigned long *lsrc = (unsigned long *)src;
	size_t triplets = length >> 2;
	while (triplets--)
	{
		b12 = *lsrc++;
		b23 = *lsrc++;
		b34 = *lsrc++;

		if (currLinePos == 0) 
		{
			currLinePos = width;
			pixels-=2*width;
			bitmapBytes-=2*width;
		}

		//depends upon bigendian or little endian cpu

		if (s_COLOR_CHANNEL == Blue)
		{
			val1 = (unsigned char)(b12); 
			val2 = (unsigned char)(b12 >> 24);
			val3 = (unsigned char)(b23 >> 16);
			val4 = (unsigned char)(b34 >> 8);
		}
		else if (s_COLOR_CHANNEL == Green)
		{
			val1 = (unsigned char)(b12 >> 8); 
			val2 = (unsigned char)(b23);
			val3 = (unsigned char)(b23 >> 24);
			val4 = (unsigned char)(b34 >> 16);
		}
		else if (s_COLOR_CHANNEL == Red)
		{
			val1 = (unsigned char)(b12 >> 16); 
			val2 = (unsigned char)(b23 >> 8);
			val3 = (unsigned char)(b34);
			val4 = (unsigned char)(b34 >> 24);
		}
		else if (s_COLOR_CHANNEL == GrayScale)
		{
			val1 = (unsigned char)(.299 * (unsigned char)(b12) + .587 * (unsigned char)(b12 >> 8) + .114 * (unsigned char)(b12 >> 16)); 
			val2 = (unsigned char)(.299 * (unsigned char)(b12 >> 24) + .587 * (unsigned char)(b23) + .114 * (unsigned char)(b23 >> 8));
			val3 = (unsigned char)(.299 * (unsigned char)(b23 >> 16) + .587 * (unsigned char)(b23 >> 16) + .114 * (unsigned char)(b34));
			val4 = (unsigned char)(.299 * (unsigned char)(b34 >> 8) + .587 * (unsigned char)(b34 >> 16) + .114 * (unsigned char)(b34 >> 24));
		}


		*pixels++=val1;
		*bitmapBytes++=val1;
		currLinePos--;

		if (currLinePos == 0) 
		{
			currLinePos = width;
			pixels-=width;
			bitmapBytes-=width;
		}

		*pixels++=val2;
		*bitmapBytes++=val2;
		currLinePos--;

		if (currLinePos == 0) 
		{
			currLinePos = width;
			pixels-=width;
			bitmapBytes-=width;
		}
		
		*pixels++=val3;
		*bitmapBytes++=val3;
		currLinePos--;

		if (currLinePos == 0) 
		{
			currLinePos = width;
			pixels-=width;
			bitmapBytes-=width;
		}

		*pixels++=val4;
		*bitmapBytes++=val4;
		currLinePos--;
	}
}

void CopyPixelsInTriplets(BYTE* pDIB, long width, long height, unsigned long* pixels)
{
	long bytesPerPixel = 3;
	long length = (width * height);
	BYTE* src = pDIB + sizeof(BITMAPINFOHEADER);

	long currLinePos = 0;
	pixels+=length + width;

	BYTE val1, val2, val3, val4;
	unsigned long b12, b23, b34;

	unsigned long *lsrc = (unsigned long *)src;
	size_t triplets = length >> 2;
	while (triplets--)
	{
		b12 = *lsrc++;
		b23 = *lsrc++;
		b34 = *lsrc++;

		if (currLinePos == 0) 
		{
			currLinePos = width;
			pixels-=2*width;
		}

		//depends upon bigendian or little endian cpu

		if (s_COLOR_CHANNEL == Blue)
		{
			val1 = (unsigned char)(b12); 
			val2 = (unsigned char)(b12 >> 24);
			val3 = (unsigned char)(b23 >> 16);
			val4 = (unsigned char)(b34 >> 8);
		}
		else if (s_COLOR_CHANNEL == Green)
		{
			val1 = (unsigned char)(b12 >> 8); 
			val2 = (unsigned char)(b23);
			val3 = (unsigned char)(b23 >> 24);
			val4 = (unsigned char)(b34 >> 16);
		}
		else if (s_COLOR_CHANNEL == Red)
		{
			val1 = (unsigned char)(b12 >> 16); 
			val2 = (unsigned char)(b23 >> 8);
			val3 = (unsigned char)(b34);
			val4 = (unsigned char)(b34 >> 24);
		}
		else if (s_COLOR_CHANNEL == GrayScale)
		{
			val1 = (unsigned char)(.299 * (unsigned char)(b12) + .587 * (unsigned char)(b12 >> 8) + .114 * (unsigned char)(b12 >> 16)); 
			val2 = (unsigned char)(.299 * (unsigned char)(b12 >> 24) + .587 * (unsigned char)(b23) + .114 * (unsigned char)(b23 >> 8));
			val3 = (unsigned char)(.299 * (unsigned char)(b23 >> 16) + .587 * (unsigned char)(b23 >> 16) + .114 * (unsigned char)(b34));
			val4 = (unsigned char)(.299 * (unsigned char)(b34 >> 8) + .587 * (unsigned char)(b34 >> 16) + .114 * (unsigned char)(b34 >> 24));
		}


		*pixels++=val1;
		currLinePos--;

		if (currLinePos == 0) 
		{
			currLinePos = width;
			pixels-=width;
		}

		*pixels++=val2;
		currLinePos--;

		if (currLinePos == 0) 
		{
			currLinePos = width;
			pixels-=width;
		}
		
		*pixels++=val3;
		currLinePos--;

		if (currLinePos == 0) 
		{
			currLinePos = width;
			pixels-=width;
		}

		*pixels++=val4;
		currLinePos--;
	}
}


HRESULT GetPixelMapPixelsOnly(BYTE* pDIB, long width, long height, unsigned long* pixels)
{
	CopyPixelsInTriplets(pDIB, width, height, pixels);
	return S_OK;
}

HRESULT GetPixelMapBits(BYTE* pDIB, long* width, long* height, DWORD imageSize, unsigned long* pixels, BYTE* bitmapPixels, BYTE* bitmapBytes)
{
	HBITMAP hBitmap = NULL;
	return GetPixelMapBitsAndHBitmap(pDIB, width, height, imageSize, pixels, bitmapPixels, bitmapBytes, hBitmap);
}

HRESULT GetPixelMapBitsAndHBitmap(BYTE* pDIB, long* width, long* height, DWORD imageSize, unsigned long* pixels, BYTE* bitmapPixels, BYTE* bitmapBytes, HBITMAP hBitmap)
{
	//OutputDebugString("GetPixelMapBitsAndHBitmap.214");
	BITMAPINFOHEADER bih;
	memmove(&bih.biSize, pDIB, sizeof(BITMAPINFOHEADER));

	if (*width == 0) *width = bih.biWidth;
	if (*height == 0) *height = bih.biHeight;

	if (!bih.biSizeImage)
		bih.biSizeImage = ((((*width * bih.biBitCount) + 31) & ~31) / 8) * *height;

	if (imageSize != 0 && bih.biSizeImage != imageSize)
		throw "Image size mismatch exception";

	////and BitmapInfo variable-length UDT
	BYTE memBitmapInfo[40];
	memmove(memBitmapInfo, &bih, sizeof(bih));

	BITMAPFILEHEADER bfh;
	bfh.bfType=19778;    //BM header
	bfh.bfSize=55 + bih.biSizeImage;
	bfh.bfReserved1=0;
	bfh.bfReserved2=0;
	bfh.bfOffBits=sizeof(BITMAPINFOHEADER) + sizeof(BITMAPFILEHEADER); //54

	// Copy the display bitmap including the header
	memmove(bitmapPixels, &bfh, sizeof(bfh));
	memmove(bitmapPixels + sizeof(bfh), &memBitmapInfo, sizeof(memBitmapInfo));
	memmove(bitmapPixels + sizeof(bfh) + sizeof(memBitmapInfo), pDIB + sizeof(BITMAPINFOHEADER), bih.biSizeImage);
	
	//BITMAPINFO bmi;
	//ZeroMemory(&bmi, sizeof(BITMAPINFO));
	//CopyMemory(&(bmi.bmiHeader), m_bmih, sizeof(BITMAPINFOHEADER));

	//HDC hdcDest = GetDC(0);
	//HBITMAP hBitmap = CreateDIBitmap(hdcDest, m_bmih, CBM_INIT, m_pRGBData, &bmi, DIB_RGB_COLORS);

	//FILE* fp=fopen("Frame-00.bmp", "wb");
 //   if (fp!=NULL)
 //   {
 //       fwrite(bitmapPixels, bih.biSizeImage + sizeof(bfh) + sizeof(memBitmapInfo), 1, fp);
 //       fclose(fp);
 //   }

    //FILE* fp=fopen("Frame-00.bmp", "wb");
    //if (fp!=NULL)
    //{
    //    fwrite(&bfh, sizeof(bfh), 1, fp);
    //    fwrite(&memBitmapInfo, sizeof(memBitmapInfo), 1, fp);
    //    fwrite(pDIB + sizeof(BITMAPINFOHEADER), bih.biSizeImage, 1, fp);
    //    fclose(fp);
    //}

	CopyPixelsInTriplets(pDIB, bih, pixels, bitmapPixels, bitmapBytes);

	return S_OK;
}


HRESULT GetBitmapPixels(long width, long height, unsigned long* pixels, BYTE* bitmapPixels, BYTE* bitmapBytes, bool isLittleEndian, int bpp)
{
	BYTE* pp = bitmapPixels;

	// define the bitmap information header 
	BITMAPINFOHEADER bih;
	bih.biSize = sizeof(BITMAPINFOHEADER); 
	bih.biPlanes = 1; 
	bih.biBitCount = 24;                          // 24-bit 
	bih.biCompression = BI_RGB;                   // no compression 
	bih.biSizeImage = width * abs(height) * 3;    // width * height * (RGB bytes) 
	bih.biXPelsPerMeter = 0; 
	bih.biYPelsPerMeter = 0; 
	bih.biClrUsed = 0; 
	bih.biClrImportant = 0; 
	bih.biWidth = width;                          // bitmap width 
	bih.biHeight = height;                        // bitmap height 

	// and BitmapInfo variable-length UDT
	BYTE memBitmapInfo[40];
	memmove(memBitmapInfo, &bih, sizeof(bih));

	BITMAPFILEHEADER bfh;
	bfh.bfType=19778;    //BM header
	bfh.bfSize=55 + bih.biSizeImage;
	bfh.bfReserved1=0;
	bfh.bfReserved2=0;
	bfh.bfOffBits=sizeof(BITMAPINFOHEADER) + sizeof(BITMAPFILEHEADER); //54

	// Copy the display bitmap including the header
	memmove(bitmapPixels, &bfh, sizeof(bfh));
	memmove(bitmapPixels + sizeof(bfh), &memBitmapInfo, sizeof(memBitmapInfo));

	bitmapPixels = bitmapPixels + sizeof(bfh) + sizeof(memBitmapInfo);

	long currLinePos = 0;
	int length = width * height;
	bitmapPixels+=3 * (length + width);

	int shiftVal = bpp == 12 ? 4 : 8;

	int total = width * height;
	while(total--)
	{
		if (currLinePos == 0) 
		{
			currLinePos = width;
			bitmapPixels-=6*width;
		};

		unsigned int val = *pixels;
		pixels++;

		unsigned int dblVal;
		if (bpp == 8)
		{
			dblVal = val;
		}
		else
		{
			dblVal = val >> shiftVal;
		}
		 

		BYTE btVal = (BYTE)(dblVal & 0xFF);
		
		*bitmapPixels = btVal;
		*(bitmapPixels + 1) = btVal;
		*(bitmapPixels + 2) = btVal;
		bitmapPixels+=3;

		*bitmapBytes++ = btVal;

		currLinePos--;
	}

	//FILE* f = fopen("bmp_test.bmp", "wb");
	//fwrite(pp, 55 + bih.biSizeImage, 1, f);
	//fflush(f);
	//fclose(f);

	return S_OK;
}

void GetMinMaxValuesForBpp(int bpp, int* minValue, int* maxValue)
{
	*minValue = 0;
	*maxValue = 0;

	if (bpp == 8)
		*maxValue = 0xFF;
	else if (bpp == 12)
		*maxValue = 0xFFF;
	else if (bpp == 16)
		*maxValue = 0xFFFF;
}

HRESULT PreProcessingStretch(unsigned long* pixels, long width, long height, int bpp, int fromValue, int toValue)
{
	int minValue, maxValue;
	GetMinMaxValuesForBpp(bpp, &minValue, &maxValue);

	if (fromValue < minValue) fromValue = minValue;
	if (toValue > maxValue) toValue = maxValue;

	long totalPixels = width * height;
	unsigned long* pPixels = pixels;
	float coeff = ((float)(maxValue - minValue)) / (toValue - fromValue);

	while(totalPixels--)
	{
		if (*pPixels < fromValue) 
			*pPixels = minValue;
		else if (*pPixels > toValue) 
			*pPixels = maxValue;
		else	
		{
			float newValue = coeff * (*pPixels - fromValue);
			if (newValue < minValue) newValue = minValue;
			if (newValue > maxValue) newValue = maxValue;
			*pPixels = (long)newValue;
		}
		pPixels++;
	}

	return S_OK;
}

HRESULT PreProcessingClip(unsigned long* pixels, long width, long height, int bpp, int fromValue, int toValue)
{
	int minValue, maxValue;
	GetMinMaxValuesForBpp(bpp, &minValue, &maxValue);

	if (fromValue < minValue) fromValue = minValue;
	if (toValue > maxValue) toValue = maxValue;

	long totalPixels = width * height;
	unsigned long* pPixels = pixels;
	
	while(totalPixels--)
	{
		if (*pPixels < fromValue) 
			*pPixels = fromValue;
		else if (*pPixels > toValue) 
			*pPixels = toValue;

		pPixels++;
	}

	return S_OK;
}

HRESULT PreProcessingBrightnessContrast(unsigned long* pixels, long width, long height, int bpp, long brightness, long cotrast)
{
	int minValue, maxValue;
	GetMinMaxValuesForBpp(bpp, &minValue, &maxValue);

	if (brightness < -255) brightness = -255;
	if (brightness > 255) brightness = 255;

	double bppBrightness = brightness;
	if (bpp == 12) bppBrightness *= 0xF;
	if (bpp == 16) bppBrightness *= 0xFF;

	long totalPixels = width * height;
	unsigned long* pPixels = pixels;

	if (cotrast < -100) cotrast = -100;
	if (cotrast > 100) cotrast = 100;

	double contrastCoeff = (100.0 + cotrast) / 100.0;
	contrastCoeff *= contrastCoeff;
	
	while(totalPixels--)
	{
		double pixel = *pPixels + bppBrightness;
		if (pixel < minValue) pixel = minValue;
		if (pixel > maxValue) pixel = maxValue;

		pixel = pixel * 1.0 / maxValue;
		pixel -= 0.5;
		pixel *= contrastCoeff;
		pixel += 0.5;
		pixel *= maxValue;
		if (pixel < minValue) pixel = minValue;
		if (pixel > maxValue) pixel = maxValue;
		
		*pPixels = (long)pixel;

 		pPixels++;
	}

	return S_OK;
}


