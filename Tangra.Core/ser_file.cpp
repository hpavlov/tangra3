#include "ser_file.h"
#include <iostream>
#include <stdlib.h>
#include <stdio.h>

#include "PreProcessing.h"
#include "PixelMapUtils.h"

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
	if (NULL != m_RawFrameBuffer)
	{
		delete m_RawFrameBuffer;
		m_RawFrameBuffer = NULL;		
	}
}


void SerFile::OpenFile(const char* filePath, SerLib::SerFileInfo* fileInfo, char* observer, char* instrument, char* telescope)
{
	m_File = fopen(filePath, "rb");

	unsigned int buffInt;
	unsigned char buffChar;
	char magic[15];
	
	fread(&magic[0], 14, 1, m_File);
	magic[14] = 0;
	
	if (strcmp(magic, "LUCAM-RECORDER") != 0)
		return;
		
	fread(&buffInt, 4, 1, m_File);
	fileInfo->CameraId = buffInt;
	
	fread(&buffInt, 4, 1, m_File);
	fileInfo->ColourId = buffInt;
	m_ColourId = buffInt;
	
	if (m_ColourId == ColourFormat_RGB || m_ColourId == ColourFormat_BGR)
		m_NumPlanes = 3;
	else
		m_NumPlanes = 1;
	
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
	
	if (
			(m_BytesPerPixel == 2 && Bpp != 12 && Bpp != 14 && Bpp != 16) ||
			(m_BytesPerPixel == 1 && Bpp < 8)
		)
	{
		NormalisationValue = 1 << Bpp;
	}
		
	fread(&buffInt, 4, 1, m_File);
	fileInfo->CountFrames = buffInt;
	m_CountFrames = buffInt;
	
	fread(&observer[0], 40, 1, m_File);
	fread(&instrument[0], 40, 1, m_File);
	fread(&telescope[0], 40, 1, m_File);
	
	__int64 buffInt64;
	fread(&buffInt64, 8, 1, m_File);
	fileInfo->SequenceStartTime = buffInt;
	
	m_HasTimeStamps = ((buffInt64 >> 0x3F) & 0x01) == 0x01;
	
	fread(&buffInt64, 8, 1, m_File);
	fileInfo->SequenceStartTimeUTC = buffInt;
	
	m_RawFrameSize = Width * Height * m_NumPlanes * m_BytesPerPixel;
	m_RawFrameBuffer = (unsigned char*)malloc(m_RawFrameSize + 16);
	m_PixelsPerFrame = Width * Height;
	
	if (m_HasTimeStamps)
		m_TimeStampStartOffset = 178 + m_CountFrames * m_RawFrameSize;
	else
		m_TimeStampStartOffset = -1;
	
}

void SerFile::CloseFile()
{
	if (m_File > 0)
	{
		fclose(m_File);
		m_File = 0;		
	}
}

HRESULT SerFile::GetFrame(int frameNo, unsigned long* pixels, SerLib::SerFrameInfo* frameInfo)
{
	if (frameNo >=0 && frameNo < m_CountFrames)
	{
		__int64 imagePosition = 178 + frameNo * m_RawFrameSize;
			
		advfsetpos(m_File, &imagePosition);
		fread(m_RawFrameBuffer, m_RawFrameSize, 1, m_File);
		HRESULT rv = ProcessRawFrame(pixels);
		
		if (SUCCEEDED(rv))
		{
			if (m_HasTimeStamps)
			{
				__int64 timestampPosition = m_TimeStampStartOffset + frameNo * 8;
				
				advfsetpos(m_File, &timestampPosition);
				fread(&frameInfo->TimeStamp, 4, 1, m_File);
			}
			else 
				frameInfo->TimeStamp = 0;			
		}

		return rv;
	}
	
	return E_FAIL;
}

HRESULT SerFile::ProcessRawFrame(unsigned long* pixels)
{
	if (m_NumPlanes = 1)
	{
		if (m_BytesPerPixel == 1)
		{
			unsigned char* charBuff = (unsigned char*)m_RawFrameBuffer;
			for(int idx = 0; idx < m_PixelsPerFrame; idx++)
			{
				*pixels = *charBuff;
				pixels++;
				charBuff++;
			}
		}
		else if (m_BytesPerPixel == 2)
		{
			unsigned short* shortBuff = (unsigned short*)m_RawFrameBuffer;
			for(int idx = 0; idx < m_PixelsPerFrame; idx++)
			{
				*pixels = *shortBuff;
				pixels++;
				shortBuff++;
			}			
		}
		
		return S_OK;
	}
	else
	{
		return E_NOTIMPL;
	}
}

};

SerLib::SerFile* m_SerFile = NULL;

HRESULT SEROpenFile(char* fileName, SerLib::SerFileInfo* fileInfo, char* observer, char* instrument, char* telescope)
{
	SERCloseFile();
	
	m_SerFile = new SerLib::SerFile();
	m_SerFile->OpenFile(fileName, fileInfo, observer, instrument, telescope);
	
	return S_OK;
}

HRESULT SERCloseFile()
{
	if (NULL != m_SerFile)
	{
		m_SerFile->CloseFile();
		
		delete m_SerFile;
		m_SerFile = NULL;
	}
	
	return S_OK;
}

HRESULT SERGetFrame(int frameNo, unsigned long* pixels, BYTE* bitmapPixels, BYTE* bitmapBytes, SerLib::SerFrameInfo* frameInfo)
{
	if (NULL != m_SerFile)
	{
		HRESULT rv = m_SerFile->GetFrame(frameNo, pixels, frameInfo);

		if (SUCCEEDED(rv))
		{
			if (g_UsesPreProcessing) 
				return ApplyPreProcessingWithNormalValue(pixels, m_SerFile->Width, m_SerFile->Height, m_SerFile->Bpp, m_SerFile->NormalisationValue, bitmapPixels, bitmapBytes);
			else
				return GetBitmapPixels(m_SerFile->Width, m_SerFile->Height, pixels, bitmapPixels, bitmapBytes, m_SerFile->LittleEndian, m_SerFile->Bpp, m_SerFile->NormalisationValue);
		}
	
		return rv;
	}
	
	return E_FAIL;
}
