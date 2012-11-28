#include "stdafx.h"
#include "TangraVideo.h"
#include "TangraAviFile.h"
#include "TangraCore.h"

int g_SelectedEngine = -1;

PAVISTREAM g_paviStream = NULL;
PGETFRAME g_pGetFrame = NULL;


HRESULT TangraVideoEnumVideoEngines(char* videoEngines)
{
	strcpy(videoEngines, "AviFile;DirectShow");
	
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
		return E_FAIL;
}

HRESULT TangraVideoOpenFile(const char* fileName, VideoFileInfo* fileInfo)
{
	bool usesPreProcessing = TangraCore::UsesPreProcessing();

	HRESULT rv = E_FAIL;

	if (g_SelectedEngine == 0)
	{
		TangraVideoCloseFile();

		rv = TangraAviFile::AviFileOpenFile(fileName, &g_paviStream);
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

				// Always ask for a 24bit frames
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

					rv = S_OK;
				}
				else
				{
					TangraVideoCloseFile();
				}
								
			}
		}
	}

	return rv;
}

HRESULT TangraVideoCloseFile()
{
	HRESULT rv = E_FAIL;

	if (g_SelectedEngine == 0)
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

		rv = S_OK;
	}

	return rv;
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

	return rv;
}

int GetProductVersion()
{	
	int major = 3;
	int minor = 0;
	int revision = 100;

	return (major << 28) + (minor << 16) + revision;
}