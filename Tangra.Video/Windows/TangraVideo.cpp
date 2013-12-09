#include "stdafx.h"
#include "TangraVideo.h"
#include "TangraAviFile.h"
#include "TangraDirectShow.h"
#include "include/TangraCore.h"

int g_SelectedEngine = -1;

PAVISTREAM g_paviStream = NULL;
PGETFRAME g_pGetFrame = NULL;


HRESULT TangraVideoEnumVideoEngines(char* videoEngines)
{
	strcpy(videoEngines, "VideoForWindows;DirectShow");
	
	return S_OK;
}

HRESULT TangraVideoSetVideoEngine(int videoEngine)
{
	if (videoEngine >= 0 && videoEngine <= 1)
	{
		g_SelectedEngine = videoEngine;
		return S_OK;
	}
	else
	{
		g_SelectedEngine = 0;
		return E_FAIL;
	}
}

HRESULT OpenAviFile(const char* fileName, VideoFileInfo* fileInfo)
{
	TangraVideoCloseFile();

	HRESULT rv = TangraAviFile::AviFileOpenFile(fileName, &g_paviStream);
	if (SUCCEEDED(rv))
	{
		AVISTREAMINFO streamInfo;
		BITMAPINFOHEADER lpFormat;
		long firstFrame;
		long countFrames;

		rv = TangraAviFile::AviFileGetStreamInfo(g_paviStream, &streamInfo, &lpFormat, &firstFrame, &countFrames);

		if (SUCCEEDED(rv))
		{
			BITMAPINFOHEADER bih;
			bih.biBitCount = lpFormat.biBitCount;
			bih.biClrImportant = 0;
			bih.biClrUsed = 0;
			bih.biCompression = 0;
			bih.biPlanes = 1;
			bih.biSize = sizeof(bih);
			bih.biXPelsPerMeter = 0;
			bih.biYPelsPerMeter = 0;

			// Corrections by M. Covington:
			// If these are pre-set, interlaced video is not handled correctly.
			// Better to give zeroes and let Windows fill them in.
			bih.biHeight = 0; 
			bih.biWidth = 0; 
			bih.biSizeImage = 0;

			// Corrections by M. Covington:
			// Validate the bit count, because some AVI files give a bit count
			// that is not one of the allowed values in a BitmapInfoHeader.
			// Here 0 means for Windows to figure it out from other information.
			if (bih.biBitCount > 24)
			{
				bih.biBitCount = 32;
			}
			else if (bih.biBitCount > 16)
			{
				bih.biBitCount = 24;
			}
			else if (bih.biBitCount > 8)
			{
				bih.biBitCount = 16;
			}
			else if (bih.biBitCount > 4)
			{
				bih.biBitCount = 8;
			}
			else if (bih.biBitCount > 0)
			{
				bih.biBitCount = 4;
			}

			// In a case of 32 bit images, we ask for 24 bit bitmaps
			if (bih.biBitCount == 32)
				bih.biBitCount = 24;

			g_pGetFrame = TangraAviFile::AviFileGetFrameOpen(g_paviStream, &bih);

			if (g_pGetFrame != 0)
			{
				fileInfo->CountFrames = countFrames;
				fileInfo->FirstFrame = firstFrame;
				fileInfo->Width = lpFormat.biWidth;
				fileInfo->Height = lpFormat.biHeight;
				fileInfo->BitmapImageSize= lpFormat.biSizeImage;
				fileInfo->FrameRate = (float)streamInfo.dwRate / (float)streamInfo.dwScale;
				strncpy(fileInfo->EngineBuffer, "VWF\0", 4);
				fileInfo->VideoFileTypeBuffer[0] = (lpFormat.biCompression >> 24) & 0xFF;
				fileInfo->VideoFileTypeBuffer[1] = (lpFormat.biCompression >> 16) & 0xFF;
				fileInfo->VideoFileTypeBuffer[2] = (lpFormat.biCompression >> 8) & 0xFF;
				fileInfo->VideoFileTypeBuffer[3] = lpFormat.biCompression & 0xFF;
				fileInfo->VideoFileTypeBuffer[4] = 0;

				rv = S_OK;
			}
			else
			{
				TangraVideoCloseFile();
			}								
		}
	}

	return rv;
}

HRESULT TangraVideoOpenFile(const char* fileName, VideoFileInfo* fileInfo)
{
	bool usesPreProcessing = TangraCore::UsesPreProcessing();

	HRESULT rv = E_FAIL;

	if (g_SelectedEngine == 0)
	{
		rv = OpenAviFile(fileName, fileInfo);
	}
	else if (g_SelectedEngine == 1)
	{
		TangraDirectShow::DirectShowCloseFile();

		rv = TangraDirectShow::DirectShowOpenFile(fileName, fileInfo);

		if (!SUCCEEDED(rv))
		{
			TangraDirectShow::DirectShowCloseFile();
		}
	}

	return rv;
}



HRESULT TangraVideoCloseFile()
{
	if (g_pGetFrame != 0)
	{
		TangraAviFile::AviFileGetFrameClose(g_pGetFrame);
		g_pGetFrame = 0;
	}

	if (NULL != g_paviStream)
	{
		TangraAviFile::AviFileCloseStream(g_paviStream);
		g_paviStream = NULL;
	}

	TangraDirectShow::DirectShowCloseFile();

	return S_OK;
}

HRESULT TangraVideoGetFrame(long frameNo, unsigned long* pixels, unsigned char* bitmapPixels, unsigned char* bitmapBytes)
{
	HRESULT rv = E_FAIL;

	if (g_SelectedEngine == 0)
	{
		if (NULL != g_paviStream && g_pGetFrame != 0)
		{			
			rv = TangraAviFile::AviFileGetFrame(g_pGetFrame, frameNo, pixels, bitmapPixels, bitmapBytes);
		}
		else
			rv = E_FAIL;
	}
	else if (g_SelectedEngine == 1)
	{
		rv = TangraDirectShow::DirectShowGetFrame(frameNo, pixels, bitmapPixels, bitmapBytes);
	}

	return rv;
}

HRESULT TangraVideoGetFramePixels(long frameNo, unsigned long* pixels)
{
	HRESULT rv = E_FAIL;

	if (g_SelectedEngine == 0)
	{
		if (NULL != g_paviStream && g_pGetFrame != 0)
		{			
			rv = TangraAviFile::AviFileGetFramePixels(g_pGetFrame, frameNo, pixels);
		}
		else
			rv = E_FAIL;
	}
	else if (g_SelectedEngine == 1)
	{
		rv = TangraDirectShow::DirectShowGetFramePixels(frameNo, pixels);
	}

	return rv;
}

HRESULT TangraVideoGetIntegratedFrame(long startFrameNo, long framesToIntegrate, bool isSlidingIntegration, bool isMedianAveraging, unsigned long* pixels, unsigned char* bitmapPixels, unsigned char* bitmapBytes)
{
	HRESULT rv = E_FAIL;

	if (g_SelectedEngine == 0)
	{
		if (NULL != g_paviStream && g_pGetFrame != 0)
		{			
			rv = TangraAviFile::AviGetIntegratedFrame(g_pGetFrame, startFrameNo, framesToIntegrate, isSlidingIntegration, isMedianAveraging, pixels, bitmapPixels, bitmapBytes);
		}
		else
			rv = E_FAIL;
	}
	else if (g_SelectedEngine == 1)
	{
		rv = TangraDirectShow::DirectShowGetIntegratedFrame(startFrameNo, framesToIntegrate, isSlidingIntegration, isMedianAveraging, pixels, bitmapPixels, bitmapBytes);
	}

	return rv;
}

int GetTangraVideoVersion()
{	
	return (VERSION_MAJOR << 28) + (VERSION_MINOR << 16) + VERSION_REVISION;
}