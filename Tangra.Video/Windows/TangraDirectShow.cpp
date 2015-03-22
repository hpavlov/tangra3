/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#include "stdafx.h"
#include <dshow.h>
#include <tchar.h>
#include <atlbase.h>

// http://jaewon.mine.nu/jaewon/2009/06/17/a-workaround-for-a-missing-file-dxtrans-h-in-directx-sdk/

#pragma include_alias( "dxtrans.h", "qedit.h" ) 
#define __IDxtCompositor_INTERFACE_DEFINED__ 
#define __IDxtAlphaSetter_INTERFACE_DEFINED__ 
#define __IDxtJpeg_INTERFACE_DEFINED__ 
#define __IDxtKey_INTERFACE_DEFINED__ 
#include <Qedit.h>

#include "include/TangraCore.h"
#include "TangraDirectShow.h"

char *m_pBuffer = NULL;
IMediaDet* m_MediaDet = NULL;
long m_Width;
long m_Height;
long m_FrameDataSize;
double m_FrameRateMS;
double m_ClipLength;


void WINAPI FreeMediaType(AM_MEDIA_TYPE& mt)
{
    if (mt.cbFormat != 0) {
        CoTaskMemFree((PVOID)mt.pbFormat);

        // Strictly unnecessary but tidier
        mt.cbFormat = 0;
        mt.pbFormat = NULL;
    }
    if (mt.pUnk != NULL) {
        mt.pUnk->Release();
        mt.pUnk = NULL;
    }
}

HRESULT TangraDirectShow::DirectShowCloseFile()
{
	if(m_pBuffer){
		delete [] m_pBuffer;
		m_pBuffer = NULL;
	}

	if(m_MediaDet){
		m_MediaDet->Release();
		m_MediaDet = NULL;
	}

	return S_OK;
}

HRESULT TangraDirectShow::DirectShowOpenFile(LPCTSTR fileName, VideoFileInfo* fileInfo)
{
	DirectShowCloseFile();

	HRESULT hr;
	long lStreams;
	bool bFound = false;

	CoInitializeEx(NULL, COINIT_APARTMENTTHREADED);

	hr = CoCreateInstance(CLSID_MediaDet, NULL, CLSCTX_INPROC, IID_IMediaDet, (void**)&m_MediaDet);
	if (FAILED(hr))
	{
		return hr;
	}

	CComBSTR bstrFileName(fileName);

	hr = m_MediaDet->put_Filename(bstrFileName);
	if (FAILED(hr))
	{
		return hr;
	}

	hr = m_MediaDet->get_OutputStreams(&lStreams);
	if (FAILED(hr))
	{
		return hr;
	}    
	
	for (long i = 0; i < lStreams; i++)
	{ 
		GUID major_type;
		hr = m_MediaDet->put_CurrentStream(i);
		if (SUCCEEDED(hr))
		{
			hr = m_MediaDet->get_StreamType(&major_type);
		}
		
		if (FAILED(hr))
		{
			break;
		}
		
		if (major_type == MEDIATYPE_Video)
		{
			bFound = true;
			break;
		}
	}
	
	if (!bFound)
	{
		return VFW_E_INVALIDMEDIATYPE;
	}

	//long m_Width = 0, m_Height = 0;
	AM_MEDIA_TYPE mt;

	hr = m_MediaDet->get_StreamMediaType(&mt);

	if (SUCCEEDED(hr)) 
	{
		if ((mt.formattype == FORMAT_VideoInfo) && (mt.cbFormat >= sizeof(VIDEOINFOHEADER)))
		{
			VIDEOINFOHEADER *pVih = (VIDEOINFOHEADER*)(mt.pbFormat);
			
			fileInfo->Width = pVih->bmiHeader.biWidth;
			fileInfo->Height = pVih->bmiHeader.biHeight;
			fileInfo->BitmapImageSize= pVih->bmiHeader.biWidth * pVih->bmiHeader.biHeight * 3; // Will be always working with 24bit BMP

			m_Width = pVih->bmiHeader.biWidth;
			m_Height = pVih->bmiHeader.biHeight;

			// We want the absolute m_Height, don't care about orientation.
			if (fileInfo->Width < 0) 
			{
				fileInfo->Width *= -1;
				m_Height *= -1;
			}

			strncpy(fileInfo->EngineBuffer, "DirectShow\0", 11);
			fileInfo->VideoFileTypeBuffer[0] = (pVih->bmiHeader.biCompression >> 24) & 0xFF;
			fileInfo->VideoFileTypeBuffer[1] = (pVih->bmiHeader.biCompression >> 16) & 0xFF;
			fileInfo->VideoFileTypeBuffer[2] = (pVih->bmiHeader.biCompression >> 8) & 0xFF;
			fileInfo->VideoFileTypeBuffer[3] = pVih->bmiHeader.biCompression & 0xFF;
			fileInfo->VideoFileTypeBuffer[4] = 0;
		}
		else
		{
			hr = VFW_E_INVALIDMEDIATYPE; // Should not happen, in theory.
		}

		FreeMediaType(mt);
	}

	if (FAILED(hr))
	{
		return hr;
	}

	double frameRate;
	m_MediaDet->get_FrameRate(&frameRate);
	
	m_FrameRateMS = 1000.0 / frameRate;
	m_MediaDet->get_StreamLength(&m_ClipLength);

	fileInfo->FrameRate = frameRate;
	fileInfo->CountFrames = (UINT)( (m_ClipLength*1000) / m_FrameRateMS ); // Number of frames

	hr = m_MediaDet->GetBitmapBits(0, &m_FrameDataSize, NULL, m_Width, m_Height);
}


HRESULT TangraDirectShow::DirectShowGetFrame(long frameNo, unsigned long* pixels, BYTE* bitmapPixels, BYTE* bitmapBytes)
{
	// Find the required buffer size.
	HRESULT hr = 1; //Only important if !mFrameDataSize, so make default 1
	double frameno;

	if (SUCCEEDED(hr)) 
	{
		if(!m_pBuffer)
			m_pBuffer = new char[m_FrameDataSize];

		if (!m_pBuffer)
			return E_OUTOFMEMORY;

		double frametime = frameNo * m_FrameRateMS  / 1000.0;

		hr = m_MediaDet->GetBitmapBits(frametime, NULL, m_pBuffer, m_Width, m_Height);
		if (SUCCEEDED(hr))
		{
			hr = TangraCore::GetPixelMapBits((BYTE*)(m_pBuffer), &m_Width, &m_Height, 0, pixels, bitmapPixels, bitmapBytes);
		}
		else
		{
			delete [] m_pBuffer;
			m_pBuffer = NULL;
			return E_OUTOFMEMORY;
		}
	}

	return hr;
}

HRESULT TangraDirectShow::DirectShowGetFramePixels(long frameNo, unsigned long* pixels)
{
	// Find the required buffer size.
	HRESULT hr = 1; //Only important if !mFrameDataSize, so make default 1
	double frameno;

	if(!m_FrameDataSize){
		hr = m_MediaDet->GetBitmapBits(0, &m_FrameDataSize, NULL, m_Width, m_Height);
	}

	if (SUCCEEDED(hr)) 
	{
		if(!m_pBuffer){
			m_pBuffer = new char[m_FrameDataSize];
		}
		if (!m_pBuffer){
			return E_OUTOFMEMORY;
		}

		double frametime = frameNo * m_FrameRateMS  / 1000.0;


		hr = m_MediaDet->GetBitmapBits(frametime, NULL, m_pBuffer, m_Width, m_Height);
		if (SUCCEEDED(hr))
		{
			hr = TangraCore::GetPixelMapPixelsOnly((BYTE*)(m_pBuffer), m_Width, m_Height, pixels);
		}
		else
		{
			delete [] m_pBuffer;
			m_pBuffer = NULL;
			return E_OUTOFMEMORY;
		}
	}

	return hr;
}

HRESULT TangraDirectShow::DirectShowGetIntegratedFrame(long startFrameNo, long framesToIntegrate, bool isSlidingIntegration, bool isMedianAveraging, unsigned long* pixels, BYTE* bitmapPixels, BYTE* bitmapBytes)
{	
	HRESULT rv;

	int firstFrameToIntegrate = TangraCore::IntegrationManagerGetFirstFrameToIntegrate(startFrameNo, framesToIntegrate, isSlidingIntegration);
	TangraCore::IntergationManagerStartNew(m_Width, m_Height, isMedianAveraging);

	for(int idx = 0; idx < framesToIntegrate; idx++)
	{
		rv = DirectShowGetFramePixels(firstFrameToIntegrate + idx, pixels);	
		if (rv != S_OK)
		{
			TangraCore::IntegrationManagerFreeResources();
			return rv;
		}

		TangraCore::IntegrationManagerAddFrame(pixels);
	}

	TangraCore::IntegrationManagerProduceIntegratedFrame(pixels);
	TangraCore::IntegrationManagerFreeResources();

	return TangraCore::GetBitmapPixels(m_Width, m_Height, pixels, bitmapPixels, bitmapBytes, false, 8);
}
