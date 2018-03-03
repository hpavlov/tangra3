#include "stdafx.h"
#include "TangraVideo.h"
#include "TangraAviFile.h"
#include "TangraDirectShow.h"
#include "include/TangraCore.h"

#include "Avi.h"
#include "Gdiplus.h"

int g_SelectedEngine = -1;

PAVISTREAM g_paviStream = NULL;
PGETFRAME g_pGetFrame = NULL;


HRESULT TangraVideoEnumVideoEngines(char* videoEngines, int len)
{
	strcpy_s(videoEngines, len, "VideoForWindows (2Gb Limit);DirectShow");
	
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
				strncpy_s(fileInfo->EngineBuffer, "VWF\0", 4);
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

Avi* s_AviFile = NULL;
IStream* s_pStream = NULL;

#define ERROR_BUFFER_SIZE 200
char s_LastAviErrorMessage[ERROR_BUFFER_SIZE];

long s_AviFrameNo = 0;
long s_AviFrameWidth = 0;
long s_AviFrameHeight = 0;
long s_AviFrameBpp = 8;
bool s_ShowCompressionDialog = false;
unsigned long s_UsedCompression = 0;
long s_AddedFrames;

void EnsureAviFileClosed()
{
	if (NULL != s_pStream)
	{
		s_pStream->Release();
		s_pStream = NULL;
	}

	if (NULL != s_AviFile)
	{
		s_AviFile->Close();
		delete s_AviFile;
		s_AviFile = NULL;
	}
}

unsigned long GetUsedAviCompression()
{
	return s_UsedCompression;
}

HRESULT TangraCreateNewAviFile(LPCTSTR szFileName, long width, long height, long bpp, double fps, bool showCompressionDialog)
{
	HRESULT rv = S_OK;

	EnsureAviFileClosed();
	
	s_AviFile = new Avi(szFileName, static_cast<int>(1000.0/fps), 0);

	s_AviFrameNo = 0;
	s_AviFrameWidth = width;
	s_AviFrameHeight = height;
	s_AviFrameBpp = bpp;
	s_ShowCompressionDialog = showCompressionDialog;
	s_UsedCompression = 0;

	::CreateStreamOnHGlobal(NULL, TRUE, &s_pStream);

	s_AddedFrames = 0;

	return rv;
}

HRESULT SetAviFileCompression(HBITMAP* bmp)
{
	AVICOMPRESSOPTIONS opts; 
	ZeroMemory(&opts,sizeof(opts));
	// Use uncompressed by default (unless user selects the compression)
	if (!s_ShowCompressionDialog)
	{
		opts.fccHandler= mmioFOURCC('D','I','B',' '); //0x20424944
		opts.dwFlags = AVICOMPRESSF_VALID;
	}

	HRESULT rv = s_AviFile->compression(*bmp, &opts, s_ShowCompressionDialog, 0);
	
	if (rv != S_OK) 
	{
		rv = s_AviFile->compression(*bmp, 0, false, 0);
		s_UsedCompression = 0;
	}
	else
	{
		s_UsedCompression = opts.fccHandler;
	}

	if (rv != S_OK) 
		FormatAviMessage(rv, s_LastAviErrorMessage, ERROR_BUFFER_SIZE);

	return rv;
}

HRESULT TangraGetLastAviFileError(char* szErrorMessage, int len)
{
	strncpy_s(szErrorMessage, ERROR_BUFFER_SIZE, s_LastAviErrorMessage, len);

	return S_OK;
}

long ABS(long x)
{
	if (x < 0)
		return -x;
	return x;
}

BYTE* GetBitmapPixels(long width, long height, long* pixels, int bpp)
{
	BYTE* bitmapPixels = (BYTE*)malloc(sizeof(BYTE) * ((width * height * 3) + 40 + 14 + 1));
	BYTE* bitmapPixelsStartPtr = bitmapPixels;

	BYTE* pp = bitmapPixels;

	// define the bitmap information header
	BITMAPINFOHEADER bih;
	bih.biSize = sizeof(BITMAPINFOHEADER);
	bih.biPlanes = 1;
	bih.biBitCount = 24;                          // 24-bit
	bih.biCompression = BI_RGB;                   // no compression
	bih.biSizeImage = width * ABS(height) * 3;    // width * height * (RGB bytes)
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

	int shiftVal = 0;
	if (bpp == 8) shiftVal = 0;
	else if (bpp == 9) shiftVal = 1;
	else if (bpp == 10) shiftVal = 2;
	else if (bpp == 11) shiftVal = 3;
	else if (bpp == 12) shiftVal = 4;
	else if (bpp == 13) shiftVal = 5;
	else if (bpp == 14) shiftVal = 6;
	else if (bpp == 15) shiftVal = 7;
	else if (bpp == 16) shiftVal = 8;

	int total = width * height;
	while(total--) {
		if (currLinePos == 0) {
			currLinePos = width;
			bitmapPixels-=6*width;
		};

		unsigned int val = *pixels;
		pixels++;

		unsigned int dblVal;
		if (bpp == 8) {
			dblVal = val;
		} else {
			dblVal = val >> shiftVal;
		}


		BYTE btVal = (BYTE)(dblVal & 0xFF);

		*bitmapPixels = btVal;
		*(bitmapPixels + 1) = btVal;
		*(bitmapPixels + 2) = btVal;
		bitmapPixels+=3;

		currLinePos--;
	}

	return bitmapPixelsStartPtr;
}

BYTE* BuildBitmap(long width, long height, long bpp, long* pixels)
{
	BYTE* bitmapPixels = (BYTE*)malloc(sizeof(BYTE) * ((width * height * 3) + 40 + 14 + 1));
	BYTE* bitmapPixelsStartPtr = bitmapPixels;

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
	RtlMoveMemory(memBitmapInfo, &bih, sizeof(bih));

	BITMAPFILEHEADER bfh;
	bfh.bfType=19778;    //BM header
	bfh.bfSize=55 + bih.biSizeImage;
	bfh.bfReserved1=0;
	bfh.bfReserved2=0;
	bfh.bfOffBits=sizeof(BITMAPINFOHEADER) + sizeof(BITMAPFILEHEADER); //54

	// Copy the display bitmap including the header
	RtlMoveMemory(bitmapPixels, &bfh, sizeof(bfh));
	RtlMoveMemory(bitmapPixels + sizeof(bfh), &memBitmapInfo, sizeof(memBitmapInfo));

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

		currLinePos--;
	}

	return bitmapPixelsStartPtr;
}

HRESULT TangraAviFileAddFrame(long* pixels)
{
	HRESULT rv = S_OK;

	BYTE* bitmapPixels = GetBitmapPixels(s_AviFrameWidth, s_AviFrameHeight, pixels, s_AviFrameBpp);
	
	if(s_pStream)
	{
		LARGE_INTEGER ZERO_POS = { 0 };
		s_pStream->Seek(ZERO_POS, 0, NULL); 

		rv = s_pStream->Write(&bitmapPixels[0], ULONG(sizeof(BYTE) * ((s_AviFrameWidth * s_AviFrameHeight * 3) + 40 + 14 + 1)), NULL);
		if(rv == S_OK)
		{
			HBITMAP hbmp = NULL;
			Gdiplus::Bitmap* pBitmap = Gdiplus::Bitmap::FromStream(s_pStream);
			if (pBitmap->GetHBITMAP(Gdiplus::Color(255, 0, 0, 0), &hbmp) == Gdiplus::Ok)
			{
				if (s_ShowCompressionDialog && s_AddedFrames == 0) SetAviFileCompression(&hbmp);

				rv = s_AviFile->add_frame(hbmp);

				s_AddedFrames++;

				if (rv != S_OK)
				  FormatAviMessage(rv, s_LastAviErrorMessage, ERROR_BUFFER_SIZE);

				::DeleteObject(pBitmap);
			}

			::DeleteObject(hbmp);
		}
	}

	delete bitmapPixels;

	return rv;
}


HRESULT TangraAviFileClose()
{
	EnsureAviFileClosed();

	return S_OK;
}