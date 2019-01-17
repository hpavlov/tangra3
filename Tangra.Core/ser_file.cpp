/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#include "ser_file.h"
#include <iostream>
#include <stdlib.h>
#include <stdio.h>

#include "PreProcessing.h"
#include "PixelMapUtils.h"
#include "IntegrationUtils.h"

using namespace std;

namespace SerLib
{

SerFile::SerFile()
{
	m_RawFrameBuffer = NULL;

	// No normalisation of videos that are 8, 12, 14 or 16 bit. NormalisationValue will be set later if different BBP is used.
	NormalisationValue = 0;
}

SerFile::~SerFile()
{
	if (NULL != m_RawFrameBuffer) {
		delete m_RawFrameBuffer;
		m_RawFrameBuffer = NULL;
	}
}


void SerFile::OpenFile(const char* filePath, SerLib::SerFileInfo* fileInfo, char* observer, char* instrument, char* telescope, bool checkMagic, bool greyScaleRGB)
{
	m_File = fopen(filePath, "rb");

	unsigned int buffInt;
	unsigned char buffChar;
	char magic[15];

	fread(&magic[0], 14, 1, m_File);
	magic[14] = 0;

	if (checkMagic) {
		if (strcmp(magic, "LUCAM-RECORDER") != 0 &&
		    strcmp(magic, "FireCaptureV24") != 0)
			return;
	}

	fread(&buffInt, 4, 1, m_File);
	fileInfo->CameraId = buffInt;

	fread(&buffInt, 4, 1, m_File);
	fileInfo->ColourId = buffInt;
	m_ColourId = buffInt;

	if (m_ColourId == ColourFormat_RGB || m_ColourId == ColourFormat_BGR)
	{
		m_NumPlanes = 3;
		m_GrayScaleRGB = greyScaleRGB;
	}
	else
	{
		m_NumPlanes = 1;
	}
		
	fileInfo->NumPlanes = m_NumPlanes;

	fread(&buffInt, 4, 1, m_File);
	fileInfo->LittleEndian = buffInt;
	LittleEndian = buffInt == 1;

	fread(&buffInt, 4, 1, m_File);
	fileInfo->Width = buffInt;
	Width = buffInt;

	fread(&buffInt, 4, 1, m_File);
	fileInfo->Height = buffInt;
	Height = buffInt;

	fread(&buffInt, 4, 1, m_File);
	fileInfo->PixelDepthPerPlane = buffInt;
	Bpp = buffInt;

	m_BytesPerPixel = Bpp > 8 ? 2 : 1;

	fileInfo->NormalisationValue = 0;
	if (
	    (m_BytesPerPixel == 2 && Bpp != 12 && Bpp != 14 && Bpp != 16) ||
	    (m_BytesPerPixel == 1 && Bpp < 8)
	) {
		NormalisationValue = 1 << Bpp;
		fileInfo->NormalisationValue = NormalisationValue;
	}

	fread(&buffInt, 4, 1, m_File);
	fileInfo->CountFrames = buffInt;
	m_CountFrames = buffInt;

	fread(&observer[0], 40, 1, m_File);
	fread(&instrument[0], 40, 1, m_File);
	fread(&telescope[0], 40, 1, m_File);

	m_RawFrameSize = Width * Height * m_NumPlanes * m_BytesPerPixel;
	m_RawFrameBuffer = (unsigned char*)malloc(m_RawFrameSize + 16);
	m_PixelsPerFrame = Width * Height;
    
	unsigned __int64 buffInt64;
	fread(&buffInt64, 8, 1, m_File);

	HasTimeStamps = ((buffInt64 >> 0x3F) & 0x01) == 0x00;

	if (HasTimeStamps)
	{
		// According to the SER v3 documentation, if the initial timestamp is present and the value is greater than zero
		// then the file contains embedded timestamps. However based on real life recorded SER files this is not a sufficient indication
		// On the top of this Tangra also requires:
		// - The file to be large enough to actually contain the trailing records with the timestamps
		// - The first timestamp to not be zero

		m_TimeStampStartOffset = 178 + (__int64)m_CountFrames * (__int64)m_RawFrameSize;
		advfseek(m_File, 0L, SEEK_END);
		__int64 fileSize = advftell64(m_File);
		if (fileSize < m_TimeStampStartOffset + (__int64)8)
		{
			HasTimeStamps = false;
		}
		else
		{
			__int64 timestampPosition = m_TimeStampStartOffset;
			advfsetpos(m_File, &timestampPosition);
			unsigned __int64 firstTimeStamp;
			fread(&firstTimeStamp, 8, 1, m_File);
			HasTimeStamps = firstTimeStamp > 0;
		}
	}

	if (HasTimeStamps)
	{
		fileInfo->SequenceStartTimeLo = buffInt64 & 0xFFFFFFFF;
		fileInfo->SequenceStartTimeHi = buffInt64 >> 32;

		fread(&buffInt64, 8, 1, m_File);
		fileInfo->SequenceStartTimeUTCLo = buffInt64 & 0xFFFFFFFF;
		fileInfo->SequenceStartTimeUTCHi = buffInt64 >> 32;
	}
	else
	{
		fileInfo->SequenceStartTimeLo = 0;
		fileInfo->SequenceStartTimeHi = 0;
		fileInfo->SequenceStartTimeUTCLo = 0;
		fileInfo->SequenceStartTimeUTCHi = 0;
		m_TimeStampStartOffset = -1;
	}
}

void SerFile::CloseFile()
{
	if (m_File > 0) {
		fclose(m_File);
		m_File = 0;
	}
}

HRESULT SerFile::GetFrame(int frameNo, unsigned int* pixels, unsigned int cameraBitPix, SerLib::SerFrameInfo* frameInfo)
{
	if (frameNo >=0 && frameNo < m_CountFrames) {
		__int64 imagePosition = (__int64)178 + (__int64)frameNo * (__int64)m_RawFrameSize;

		advfsetpos(m_File, &imagePosition);
		fread(m_RawFrameBuffer, m_RawFrameSize, 1, m_File);
		HRESULT rv = ProcessRawFrame(pixels, cameraBitPix);

		if (SUCCEEDED(rv)) {
			if (HasTimeStamps) {
				__int64 timestampPosition = m_TimeStampStartOffset + (__int64)(frameNo * 8);

				advfsetpos(m_File, &timestampPosition);

				unsigned __int64 timeStampUtc64;

				fread(&timeStampUtc64, 8, 1, m_File);
				frameInfo->TimeStampUtcLo = timeStampUtc64 & 0xFFFFFFFF;
				frameInfo->TimeStampUtcHi = timeStampUtc64 >> 32;
				frameInfo->TimeStampUtc64 = timeStampUtc64;
			} else {
				frameInfo->TimeStampUtcLo = 0;
				frameInfo->TimeStampUtcHi = 0;
				frameInfo->TimeStampUtc64 = 0;
			}
		}

		return rv;
	}

	return E_FAIL;
}

HRESULT SerFile::GetFrameInfo(int frameNo, SerLib::SerFrameInfo* frameInfo)
{
	if (frameNo >=0 && frameNo < m_CountFrames) {

		if (HasTimeStamps) {
			__int64 timestampPosition = m_TimeStampStartOffset + (__int64)(frameNo * 8);

			advfsetpos(m_File, &timestampPosition);

			unsigned __int64 timeStampUtc64;

			fread(&timeStampUtc64, 8, 1, m_File);
			frameInfo->TimeStampUtcLo = timeStampUtc64 & 0xFFFFFFFF;
			frameInfo->TimeStampUtcHi = timeStampUtc64 >> 32;
			frameInfo->TimeStampUtc64 = timeStampUtc64;
		} else {
			frameInfo->TimeStampUtcLo = 0;
			frameInfo->TimeStampUtcHi = 0;
			frameInfo->TimeStampUtc64 = 0;
		}

		return S_OK;
	}

	return E_FAIL;	
}

HRESULT SerFile::ProcessRawFrame(unsigned int* pixels, unsigned int cameraBitPix)
{
	if (m_NumPlanes == 1) {
		if (m_BytesPerPixel == 1) {
			unsigned char* charBuff = (unsigned char*)m_RawFrameBuffer;
			for(int idx = 0; idx < m_PixelsPerFrame; idx++) {
				*pixels = *charBuff;
				pixels++;
				charBuff++;
			}
		} else if (m_BytesPerPixel == 2) {
			int shiftVal = Bpp - cameraBitPix;

			unsigned short* shortBuff = (unsigned short*)m_RawFrameBuffer;
			for(int idx = 0; idx < m_PixelsPerFrame; idx++) {
				*pixels = *shortBuff;
				if (shiftVal > 0) *pixels = (*pixels >> shiftVal);
				pixels++;
				shortBuff++;
			}
		}
		return S_OK;
		
	} else if (m_NumPlanes == 3) {
		if (m_BytesPerPixel == 1) {
			unsigned char* charBuff = (unsigned char*)m_RawFrameBuffer;
			for(int idx = 0; idx < m_PixelsPerFrame; idx++) {
				if (m_GrayScaleRGB)
				{
					unsigned char r = *charBuff; charBuff++;
					unsigned char g = *charBuff; charBuff++;
					unsigned char b = *charBuff; charBuff++;
				
					if (m_ColourId == ColourFormat_BGR)
					{
						*pixels = (unsigned char)(.299 * b + .587 * g + .114 * r);	
					}
					else
					{
						*pixels = (unsigned char)(.299 * r + .587 * g + .114 * b);
					}
				}
				else
				{
					*pixels = *charBuff; charBuff+=3;	
				}
				
				pixels++;
			}
		} else if (m_BytesPerPixel == 2) {
			int shiftVal = Bpp - cameraBitPix;

			unsigned short* shortBuff = (unsigned short*)m_RawFrameBuffer;
			for(int idx = 0; idx < m_PixelsPerFrame; idx++) {
				
				if (m_GrayScaleRGB)
				{
					unsigned short r = *shortBuff; shortBuff++;
					unsigned short g = *shortBuff; shortBuff++;
					unsigned short b = *shortBuff; shortBuff++;
				
					if (shiftVal > 0)
					{
						 r = (r >> shiftVal);
						 g = (g >> shiftVal);
						 b = (b >> shiftVal);
					}
				
					if (m_ColourId == ColourFormat_BGR)
					{
						*pixels = (unsigned short)(.299 * b + .587 * g + .114 * r);	
					}
					else
					{
						*pixels = (unsigned short)(.299 * r + .587 * g + .114 * b);
					}
				}
				else
				{
					*pixels = *shortBuff; shortBuff+=3;	
					if (shiftVal > 0) *pixels = (*pixels >> shiftVal);
				}
				
				pixels++;
			}
		}
		return S_OK;
		
	} else {
		return E_NOTIMPL;
	}
}

};

SerLib::SerFile* m_SerFile = NULL;

HRESULT SEROpenFile(char* fileName, SerLib::SerFileInfo* fileInfo, char* observer, char* instrument, char* telescope, bool checkMagic, bool greyScaleRGB)
{
	SERCloseFile();

	m_SerFile = new SerLib::SerFile();
	m_SerFile->OpenFile(fileName, fileInfo, observer, instrument, telescope, checkMagic, greyScaleRGB);

	return S_OK;
}

HRESULT SERCloseFile()
{
	if (NULL != m_SerFile) {
		m_SerFile->CloseFile();

		delete m_SerFile;
		m_SerFile = NULL;
	}

	return S_OK;
}

HRESULT SERGetFrame(int frameNo, unsigned int* pixels, unsigned int* originalPixels, BYTE* bitmapPixels, BYTE* bitmapBytes, unsigned int cameraBitPix, SerLib::MarshalledSerFrameInfo* marshalledFrameInfo)
{
	if (NULL != m_SerFile) {
		if (cameraBitPix == 0) cameraBitPix = m_SerFile->Bpp;

		SerLib::SerFrameInfo frameInfo;
		HRESULT rv = m_SerFile->GetFrame(frameNo, pixels, cameraBitPix, &frameInfo);
		marshalledFrameInfo->TimeStampUtcLo = frameInfo.TimeStampUtcLo;
		marshalledFrameInfo->TimeStampUtcHi = frameInfo.TimeStampUtcHi;

		if (SUCCEEDED(rv)) {
			if (g_UsesPreProcessing)
			{
				memcpy(originalPixels, pixels, m_SerFile->Width * m_SerFile->Height * sizeof(unsigned int));
				
				return ApplyPreProcessingWithNormalValue(
					originalPixels, pixels, 
					m_SerFile->Width, 
					m_SerFile->Height, 
					cameraBitPix, 
					0 /* Not Supported */, 
					m_SerFile->NormalisationValue, 
					bitmapPixels, 
					bitmapBytes);					
			}
			else
				return GetBitmapPixels(
				           m_SerFile->Width,
				           m_SerFile->Height,
				           pixels,
				           bitmapPixels,
				           bitmapBytes,
				           m_SerFile->LittleEndian,
				           cameraBitPix,
				           m_SerFile->NormalisationValue);
		}

		return rv;
	}

	return E_FAIL;
}

HRESULT SERGetFrameInfo(int frameNo, SerLib::MarshalledSerFrameInfo* marshalledFrameInfo)
{
	if (NULL != m_SerFile) {
		SerLib::SerFrameInfo frameInfo;
		HRESULT rv = m_SerFile->GetFrameInfo(frameNo, &frameInfo);
		marshalledFrameInfo->TimeStampUtcLo = frameInfo.TimeStampUtcLo;
		marshalledFrameInfo->TimeStampUtcHi = frameInfo.TimeStampUtcHi;
		
		return rv;
	}
	
	return E_FAIL;
}

HRESULT SERGetIntegratedFrame(int startFrameNo, int framesToIntegrate, bool isSlidingIntegration, bool isMedianAveraging, unsigned int* pixels, unsigned int* originalPixels, BYTE* bitmapBytes, BYTE* bitmapDisplayBytes, unsigned int cameraBitPix, SerLib::MarshalledSerFrameInfo* frameInfo)
{
	if (NULL != m_SerFile) {
		HRESULT rv;
		int firstFrameToIntegrate = IntegrationManagerGetFirstFrameToIntegrate(startFrameNo, framesToIntegrate, isSlidingIntegration);
		IntergationManagerStartNew(m_SerFile->Width, m_SerFile->Height, isMedianAveraging);

		SerLib::SerFrameInfo firstFrameInfo;
		SerLib::SerFrameInfo lastFrameInfo;

		if (cameraBitPix == 0) cameraBitPix = m_SerFile->Bpp;

		for(int idx = 0; idx < framesToIntegrate; idx++) {
			SerLib::SerFrameInfo* singleFrameInfo = idx == 0 ? &firstFrameInfo : &lastFrameInfo;

			rv = m_SerFile->GetFrame(firstFrameToIntegrate + idx, pixels, cameraBitPix, singleFrameInfo);

			if (!SUCCEEDED(rv)) {
				IntegrationManagerFreeResources();
				return rv;
			}

			if (g_UsesPreProcessing) 
			{
				memcpy(originalPixels, pixels, m_SerFile->Width * m_SerFile->Height * sizeof(unsigned int));
			
				rv = ApplyPreProcessingPixelsOnly(
					originalPixels, pixels, 
					m_SerFile->Width, 
					m_SerFile->Height, 
					cameraBitPix, 
					0 /* Exposure not supported for applying Bias/Dark/Flat */,
					m_SerFile->NormalisationValue);

				if (!SUCCEEDED(rv)) {
					IntegrationManagerFreeResources();
					return rv;
				}
			}

			IntegrationManagerAddFrameEx(pixels, m_SerFile->LittleEndian, cameraBitPix);
		}

		IntegrationManagerProduceIntegratedFrame(pixels);
		IntegrationManagerFreeResources();

		if (m_SerFile->HasTimeStamps) {		
			unsigned __int64 timeStampUtc64 = GetUInt64Average(firstFrameInfo.TimeStampUtc64, lastFrameInfo.TimeStampUtc64);
			frameInfo->TimeStampUtcLo= timeStampUtc64 & 0xFFFFFFFF;
			frameInfo->TimeStampUtcHi = timeStampUtc64 >> 32;
		}

		return GetBitmapPixels(m_SerFile->Width, m_SerFile->Height, pixels, bitmapBytes, bitmapDisplayBytes, false, cameraBitPix, m_SerFile->NormalisationValue);
	}
}
