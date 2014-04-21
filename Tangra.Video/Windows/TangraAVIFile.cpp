#include "stdafx.h"

#include <stdlib.h>

#include "include/TangraCore.h"
#include "TangraAVIFile.h"

long s_AviImageWidth;
long s_AviImageHeight;
WORD s_AviBitCount;
DWORD s_AviImageSize;
DWORD s_AviCompression;

namespace TangraAviFile
{

HRESULT AviFileOpenFile(LPCTSTR szFileName, PAVISTREAM* paviStream)
{
	AVIFileInit();
    
	HRESULT hr = AVIStreamOpenFromFile(paviStream, szFileName, streamtypeVIDEO, 0, OF_READ, NULL);

	if(FAILED(hr))
	{
		printf("Could not locate AVI file <%s>.\n", szFileName);
		return E_FAIL;
	}

	return S_OK;
}

HRESULT AviFileGetStreamInfo(PAVISTREAM paviStream, AVISTREAMINFO* streamInfo, BITMAPINFOHEADER *lpFormat, long* firstFrame, long* countFrames)
{
	HRESULT hr = AVIStreamInfo(paviStream, streamInfo, sizeof(AVISTREAMINFO));

	if(FAILED(hr))
	{	     
		printf("Stream information does not exist.\n");
		throw "Not Supported";
    	return E_FAIL;
	}
    
	long formatSize = sizeof(AVISTREAMINFO);
	hr = AVIStreamReadFormat(paviStream, 0, lpFormat, &formatSize);

	if(FAILED(hr))
	{	     
		printf("Could not get stream read format.\n");
		throw "Not Supported";
    	return E_FAIL;
	}

	s_AviImageWidth = lpFormat->biWidth;
	s_AviImageHeight = lpFormat->biHeight;
	s_AviBitCount = lpFormat->biBitCount;
	s_AviImageSize = lpFormat->biSizeImage;
	s_AviCompression = lpFormat->biCompression;
	
	*firstFrame = AVIStreamStart(paviStream);
    *countFrames = AVIStreamLength(paviStream);

	return S_OK;
}

	PGETFRAME AviFileGetFrameOpen(PAVISTREAM paviStream, BITMAPINFOHEADER *bih)
	{
		PGETFRAME pgfFrame;

		if(NULL == (pgfFrame = AVIStreamGetFrameOpen(paviStream, bih))) 
		{
			printf("Opening AVI Stream was unsuccessful\n");
    		return 0;
		}

		s_AviBitCount = bih->biBitCount;
		s_AviImageSize = ((((s_AviImageWidth * s_AviBitCount) + 31) & ~31) / 8) * s_AviImageHeight;

	   return pgfFrame;
	}

	HRESULT AviFileGetFrameClose(PGETFRAME frameObject)
	{
		if (0 != frameObject)
			AVIStreamGetFrameClose(frameObject);

		return S_OK;
	}

	HRESULT AviFileGetFrame(PGETFRAME frameObject, long frameNo, unsigned long* pixels, BYTE* bitmapPixels, BYTE* bitmapBytes)
	{
		if (s_AviImageSize == 0)
			throw "Not Supported";

		//Decompress the frame and return a pointer to the DIB
		BYTE* pDIB = (BYTE*) AVIStreamGetFrame(frameObject, frameNo);

		long width = 0;
		long height = 0;


		HRESULT rv = TangraCore::GetPixelMapBits(pDIB, &width, &height, s_AviImageSize, pixels, bitmapPixels, bitmapBytes);
	
		if (TangraCore::UsesPreProcessing() && SUCCEEDED(rv)) 
			rv = TangraCore::ApplyPreProcessing(pixels, width, height, 8, bitmapPixels, bitmapBytes);

		return rv;
	}

	HRESULT AviFileGetFramePixels(PGETFRAME frameObject, long frameNo, unsigned long* pixels)
	{
		//Decompress the frame and return a pointer to the DIB
		BYTE* pDIB = (BYTE*) AVIStreamGetFrame(frameObject, frameNo);

		HRESULT rv = TangraCore::GetPixelMapPixelsOnly(pDIB, s_AviImageWidth, s_AviImageHeight, pixels);

		if (TangraCore::UsesPreProcessing() && SUCCEEDED(rv)) 
			rv = TangraCore::ApplyPreProcessingPixelsOnly(pixels, s_AviImageWidth, s_AviImageHeight, 8);

		return rv;
	}

	HRESULT AviGetIntegratedFrame(PGETFRAME frameObject, long startFrameNo, long framesToIntegrate, bool isSlidingIntegration, bool isMedianAveraging, unsigned long* pixels, BYTE* bitmapPixels, BYTE* bitmapBytes)
	{	
		HRESULT rv;
		int firstFrameToIntegrate = TangraCore::IntegrationManagerGetFirstFrameToIntegrate(startFrameNo, framesToIntegrate, isSlidingIntegration);
		TangraCore::IntergationManagerStartNew(s_AviImageWidth, s_AviImageHeight, isMedianAveraging);

		for(int idx = 0; idx < framesToIntegrate; idx++)
		{
			rv = AviFileGetFramePixels(frameObject, firstFrameToIntegrate + idx, pixels);	
			if (rv != S_OK)
			{
				TangraCore::IntegrationManagerFreeResources();
				return rv;
			}

			TangraCore::IntegrationManagerAddFrame(pixels);
		}

		TangraCore::IntegrationManagerProduceIntegratedFrame(pixels);
		TangraCore::IntegrationManagerFreeResources();

		return TangraCore::GetBitmapPixels(s_AviImageWidth, s_AviImageHeight, pixels, bitmapPixels, bitmapBytes, false, 8);
	}

	HRESULT AviFileCloseStream(PAVISTREAM paviStream)
	{
		AVIStreamClose(paviStream);

		return S_OK;
	}

}

void cpy(unsigned char *src, unsigned long *dst, size_t length)
{
    unsigned long *lsrc = (unsigned long *)src;

    size_t longs = length >> 2;
    while (longs--)
    {
        unsigned long b4 = *lsrc++;

        //depends upon bigendian or little endian cpu
        *dst++=(unsigned long)(unsigned char)b4;  b4 >>= 8;
        *dst++= (unsigned long) (unsigned char)b4;  b4 >>= 8;
        *dst++=(unsigned long) (unsigned char)b4;  b4 >>= 8;
        *dst++= (unsigned long) (unsigned char)b4;
    }

    src = (unsigned char *)lsrc;
    length &= 3;
    while (length--) *dst++ = (unsigned long)*src++;
}