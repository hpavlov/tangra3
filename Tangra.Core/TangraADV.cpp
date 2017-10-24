/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#include "PixelMapUtils.h"
#include "cross_platform.h"
#include "string"

#include "adv_file.h"
#include "adv2_file.h"
#include "IntegrationUtils.h"
#include "PreProcessing.h"
#include "TangraADV.h"

#include "Compressor.h"

AdvLib::AdvFile* g_TangraAdvFile;
int g_MaxFrameBufferSize;
int prevFrameNo;
unsigned int* prevFramePixels;
Compressor* m_Lagarith16Decompressor = NULL;
int m_CurrentLagarithWidth = 0;
int m_CurrentLagaritHeight = 0; 
bool g_IsAAV = false;

char* g_CurrentAdvFile;
AdvLib2::Adv2File* g_TangraAdv2File;

void EnsureAdvFileClosed()
{
	if (NULL != g_TangraAdvFile)
	{
		delete g_TangraAdvFile;
		g_TangraAdvFile = NULL;		
	}

	if (NULL != prevFramePixels)
	{
		delete prevFramePixels;
		prevFramePixels = NULL;
	}
	
	g_IsAAV = false;
}

HRESULT ADVOpenFile(char* fileName, AdvLib::AdvFileInfo* fileInfo)
{
	// Ensure file is closed
	EnsureAdvFileClosed();

	g_TangraAdvFile = new AdvLib::AdvFile();
	g_TangraAdvFile->OpenFile(fileName, fileInfo);

	g_IsAAV = g_TangraAdvFile->IsAAV();
	g_MaxFrameBufferSize = g_TangraAdvFile->ImageSection->MaxFrameBufferSize() * 2 /*MaxBuff is calculated for ushorts*/;

	prevFrameNo = -1;
	prevFramePixels = (unsigned int*)malloc(g_MaxFrameBufferSize);

	return S_OK;
}


HRESULT ADVCloseFile()
{
	EnsureAdvFileClosed();

	return S_OK;
}

HRESULT ADVGetFileTag(char* fileTagName, char* fileTagValue)
{
	g_TangraAdvFile->GetFileTag(fileTagName, fileTagValue);
	
	return S_OK;
}

HRESULT ADVGetFramePixels(int frameNo, unsigned int* pixels, AdvLib::AdvFrameInfo* frameInfo, char* gpsFix, char* userCommand, char* systemError)
{
	if (frameNo < g_TangraAdvFile->TotalNumberOfFrames)
    {
        unsigned char layoutId;
        enum GetByteMode byteMode;

        g_TangraAdvFile->GetFrameImageSectionHeader(frameNo, &layoutId, &byteMode);

		AdvLib::AdvImageLayout* layout = g_TangraAdvFile->ImageSection->GetImageLayoutById(layoutId);

		if (layout->IsDiffCorrLayout && prevFrameNo == frameNo)
		{
			// Asking for the last frame again. Need to reset the prev frame 
			prevFrameNo = -1;
		}

		if (layout->IsDiffCorrLayout && byteMode == DiffCorrBytes && prevFrameNo != frameNo - 1)
		{
			// Move back and find the nearest previous key frame
			int keyFrameIdx = frameNo;
			do
			{
				keyFrameIdx--;
				g_TangraAdvFile->GetFrameImageSectionHeader(keyFrameIdx, &layoutId, &byteMode);
			}
			while(keyFrameIdx > 0 && byteMode != KeyFrameBytes);

			unsigned int* keyFrameData = (unsigned int*)malloc(g_MaxFrameBufferSize);
			AdvLib::AdvFrameInfo prefFrameInfo;
			
			g_TangraAdvFile->GetFrameSectionData(keyFrameIdx, NULL, keyFrameData, &prefFrameInfo, NULL, NULL, NULL);

			if (layout->BaseFrameType == DiffCorrPrevFrame && keyFrameIdx + 1 < frameNo)
			{
				for (int i = keyFrameIdx + 1; i < frameNo; i++)
				{
					g_TangraAdvFile->GetFrameSectionData(i, keyFrameData, prevFramePixels, &prefFrameInfo, NULL, NULL, NULL);

					memcpy(keyFrameData, prevFramePixels, g_MaxFrameBufferSize);
				}
			}
			else
			{
				// Copy bytes to the prevFramePixels 
				memcpy(prevFramePixels, keyFrameData, g_MaxFrameBufferSize);
			}

			delete keyFrameData;
		}

		
        g_TangraAdvFile->GetFrameSectionData(frameNo, prevFramePixels, pixels, frameInfo, gpsFix, userCommand, systemError);
	
		if (byteMode != Normal)
				memcpy(prevFramePixels, pixels, g_TangraAdvFile->ImageSection->Width * g_TangraAdvFile->ImageSection->Height * 4);

		prevFrameNo = frameNo;

		return S_OK;
    }
    else
        return E_FAIL; 

	return E_FAIL;
}

HRESULT ADVGetFrame(int frameNo, unsigned int* pixels, unsigned int* originalPixels, BYTE* bitmapPixels, BYTE* bitmapBytes, AdvLib::AdvFrameInfo* frameInfo, char* gpsFix, char* userCommand, char* systemError)
{
	HRESULT rv = ADVGetFramePixels(frameNo, pixels, frameInfo, gpsFix, userCommand, systemError);
	if (SUCCEEDED(rv))
	{
		memcpy(originalPixels, pixels, g_TangraAdvFile->ImageSection->Width * g_TangraAdvFile->ImageSection->Height * sizeof(unsigned int));

		if (g_UsesPreProcessing && 
		    (!g_IsAAV || frameInfo->IntegratedFrames > 0) /*Not the first/last AAV control frame*/) 
		{
			return ApplyPreProcessingWithNormalValue(
				originalPixels, pixels, g_TangraAdvFile->ImageSection->Width, g_TangraAdvFile->ImageSection->Height, g_TangraAdvFile->ImageSection->DataBpp, frameInfo->Exposure10thMs / 10000.0,
				g_TangraAdvFile->ImageSection->NormalisationValue, bitmapPixels, bitmapBytes);
				
		}
		else
			return GetBitmapPixels(
				g_TangraAdvFile->ImageSection->Width, 
				g_TangraAdvFile->ImageSection->Height, 
				pixels, 
				bitmapPixels, 
				bitmapBytes, 
				g_TangraAdvFile->ImageSection->ByteOrder == LittleEndian, 
				g_TangraAdvFile->ImageSection->DataBpp, 
				g_TangraAdvFile->ImageSection->NormalisationValue);
	}
	
	return rv;
}

HRESULT ADVGetFrameStatusChannel(int frameNo, AdvLib::AdvFrameInfo* frameInfo, char* gpsFix, char* userCommand, char* systemError)
{
	g_TangraAdvFile->GetFrameStatusSectionData(frameNo, frameInfo, gpsFix, userCommand, systemError);
	
	return S_OK;
}

HRESULT ADVGetIntegratedFrame(int startFrameNo, int framesToIntegrate, bool isSlidingIntegration, bool isMedianAveraging, unsigned int* pixels, unsigned int* originalPixels, BYTE* bitmapBytes, BYTE* bitmapDisplayBytes, AdvLib::AdvFrameInfo* frameInfo)
{
	HRESULT rv;
	int firstFrameToIntegrate = IntegrationManagerGetFirstFrameToIntegrate(startFrameNo, framesToIntegrate, isSlidingIntegration);
	IntergationManagerStartNew(g_TangraAdvFile->ImageSection->Width, g_TangraAdvFile->ImageSection->Height, isMedianAveraging);

	AdvLib::AdvFrameInfo firstFrameInfo;
	AdvLib::AdvFrameInfo lastFrameInfo;
	int totalExposure10thMs = 0;
	
	int idxOfFrameInfo = framesToIntegrate / 2;

	for(int idx = 0; idx < framesToIntegrate; idx++)
	{
		AdvLib::AdvFrameInfo* singleFrameInfo = idx == 0 ? &firstFrameInfo : &lastFrameInfo;
		
		rv = ADVGetFramePixels(firstFrameToIntegrate + idx, pixels, singleFrameInfo, NULL, NULL, NULL);	
		
		totalExposure10thMs+= singleFrameInfo->Exposure10thMs;
		
		if (!SUCCEEDED(rv))
		{
			IntegrationManagerFreeResources();
			return rv;
		}

		if (g_UsesPreProcessing)
		{
			memcpy(originalPixels, pixels, g_TangraAdvFile->ImageSection->Width * g_TangraAdvFile->ImageSection->Height * sizeof(unsigned int));
			
			rv = ApplyPreProcessingPixelsOnly(
				originalPixels, pixels, 
				g_TangraAdvFile->ImageSection->Width, 
				g_TangraAdvFile->ImageSection->Height, 
				g_TangraAdvFile->ImageSection->DataBpp, 
				singleFrameInfo->Exposure10thMs / 10000.0,
				g_TangraAdvFile->ImageSection->NormalisationValue);

			if (!SUCCEEDED(rv))
			{
				IntegrationManagerFreeResources();
				return rv;
			}
		}

		IntegrationManagerAddFrameEx(pixels, g_TangraAdvFile->ImageSection->ByteOrder == LittleEndian, g_TangraAdvFile->ImageSection->DataBpp);
	}

	IntegrationManagerProduceIntegratedFrame(pixels);
	IntegrationManagerFreeResources();
	
	frameInfo->Exposure10thMs = totalExposure10thMs;
	frameInfo->GPSAlmanacOffset = lastFrameInfo.GPSAlmanacOffset;
	frameInfo->GPSAlmanacStatus = lastFrameInfo.GPSAlmanacStatus;
	frameInfo->GPSFixStatus = lastFrameInfo.GPSFixStatus;
	frameInfo->GPSTrackedSattelites = lastFrameInfo.GPSTrackedSattelites;
	frameInfo->Gain = (firstFrameInfo.Gain + lastFrameInfo.Gain) / 2.0;
	frameInfo->Gamma = (firstFrameInfo.Gamma + lastFrameInfo.Gamma) / 2.0;
	frameInfo->Offset = (firstFrameInfo.Offset + lastFrameInfo.Offset) / 2.0;
	
	__int64 midTimeStamp = GetUInt64Average(firstFrameInfo.StartTimeStampLo, firstFrameInfo.StartTimeStampHi, lastFrameInfo.StartTimeStampLo, lastFrameInfo.StartTimeStampHi);
	frameInfo->StartTimeStampHi = midTimeStamp & 0xFFFFFFFF;
	frameInfo->StartTimeStampLo = midTimeStamp >> 32;
	
	midTimeStamp = GetUInt64Average(firstFrameInfo.EndNtpTimeStampLo, firstFrameInfo.EndNtpTimeStampHi, lastFrameInfo.EndNtpTimeStampLo, lastFrameInfo.EndNtpTimeStampHi);
	frameInfo->EndNtpTimeStampHi = midTimeStamp & 0xFFFFFFFF;
	frameInfo->EndNtpTimeStampLo = midTimeStamp >> 32;
	
	midTimeStamp = GetUInt64Average(firstFrameInfo.EndSecondaryTimeStampLo, firstFrameInfo.EndSecondaryTimeStampHi, lastFrameInfo.EndSecondaryTimeStampLo, lastFrameInfo.EndSecondaryTimeStampHi);
	frameInfo->EndSecondaryTimeStampHi = midTimeStamp & 0xFFFFFFFF;
	frameInfo->EndSecondaryTimeStampLo = midTimeStamp >> 32;

	return GetBitmapPixels(g_TangraAdvFile->ImageSection->Width, g_TangraAdvFile->ImageSection->Height, pixels, bitmapBytes, bitmapDisplayBytes, false, g_TangraAdvFile->ImageSection->DataBpp, g_TangraAdvFile->ImageSection->NormalisationValue);
}

HRESULT ADVGetFrame2(int frameNo, unsigned int* pixels, unsigned int* originalPixels, BYTE* bitmapPixels, BYTE* bitmapBytes)
{
	AdvLib::AdvFrameInfo* frameInfo = new AdvLib::AdvFrameInfo();
	ADVGetFrame(frameNo, pixels, originalPixels, bitmapPixels, bitmapBytes, frameInfo, NULL, NULL, NULL);
	delete frameInfo;

	return S_OK;
}

HRESULT Lagarith16Decompress(int width, int height, unsigned char* compressedBytes, unsigned char* decompressedBytes)
{
	if (m_CurrentLagarithWidth != width || m_CurrentLagaritHeight != height)
	{
		if (NULL != m_Lagarith16Decompressor)
			delete m_Lagarith16Decompressor;
			
		m_Lagarith16Decompressor = new Compressor(width, height);
		m_CurrentLagarithWidth = width;
		m_CurrentLagaritHeight = height;
	}
	
	m_Lagarith16Decompressor->DecompressData(compressedBytes, (unsigned short*)decompressedBytes);
	
	return S_OK;
}

ADVRESULT ADV2GetFormatVersion(char* fileName)
{
	FILE* probe = advfopen(fileName, "rb");
	if (probe == 0) return E_FAIL;
	
	unsigned int buffInt;
	unsigned char buffChar;

	advfread(&buffInt, 4, 1, probe);
	advfread(&buffChar, 1, 1, probe);
	advfclose(probe);	
	
	if (buffInt != 0x46545346) return E_FAIL;
	return (unsigned int)buffChar;
}

ADVRESULT ADV2OpenFile(char* fileName, AdvLib2::AdvFileInfo* fileInfo)
{
	if (nullptr != g_TangraAdv2File)
	{
		delete g_TangraAdv2File;
		g_TangraAdv2File = nullptr;
	}
	
	int len = (int)strlen(fileName);
	if (len > 0)
	{
		g_CurrentAdvFile = new char[len + 1];
		strncpy_s(g_CurrentAdvFile, len + 1, fileName, len + 1);
	
		g_TangraAdv2File = new AdvLib2::Adv2File();
		int res = !g_TangraAdv2File->LoadFile(fileName, fileInfo);
		if (res < 0)
		{
			delete g_TangraAdv2File;
			g_TangraAdv2File = nullptr;
			return res;
		}
	}
	
	return S_OK;
}

ADVRESULT ADV2CloseFile()
{
	if (nullptr != g_TangraAdv2File)
	{
		g_TangraAdv2File->CloseFile();
		delete g_TangraAdv2File;
		g_TangraAdv2File = nullptr;
	}

	if (nullptr != g_CurrentAdvFile)
	{
		delete g_CurrentAdvFile;
		g_CurrentAdvFile = nullptr;
	}
	
	return S_OK;
}

ADVRESULT ADV2GetFrame(int frameNo, unsigned int* pixels, unsigned int* originalPixels, BYTE* bitmapPixels, BYTE* bitmapBytes, AdvLib2::AdvFrameInfo* frameInfo)
{
	int errMsgLen;
	HRESULT rv = ADV2GetFramePixels(frameNo, pixels, frameInfo, &errMsgLen);
	if (SUCCEEDED(rv))
	{
		if (g_UsesPreProcessing) 
		{
			memcpy(originalPixels, pixels, g_TangraAdv2File->ImageSection->Width * g_TangraAdv2File->ImageSection->Height * sizeof(unsigned int));
			
			float exposureSeconds = frameInfo->Exposure / 10000.0;
			
			return ApplyPreProcessingWithNormalValue(
				originalPixels, pixels, g_TangraAdv2File->ImageSection->Width, g_TangraAdv2File->ImageSection->Height, g_TangraAdv2File->ImageSection->DataBpp, exposureSeconds,
				g_TangraAdv2File->ImageSection->MaxPixelValue, bitmapPixels, bitmapBytes);
				
		}
		else
			return GetBitmapPixels(
				g_TangraAdv2File->ImageSection->Width, 
				g_TangraAdv2File->ImageSection->Height, 
				pixels, 
				bitmapPixels, 
				bitmapBytes, 
				g_TangraAdv2File->ImageSection->ByteOrder == LittleEndian, 
				g_TangraAdv2File->ImageSection->DataBpp, 
				g_TangraAdv2File->ImageSection->MaxPixelValue);
	}
	
	return rv;
}

ADVRESULT ADV2GetFramePixels(int frameNo, unsigned int* pixels, AdvLib2::AdvFrameInfo* frameInfo, int* systemErrorLen)
{
	if (g_TangraAdv2File == nullptr)
		return E_ADV_NOFILE;

	if (g_TangraAdv2File->ImageSection == nullptr)
		return E_ADV_IMAGE_SECTION_UNDEFINED;

	if (frameNo >= g_TangraAdv2File->TotalNumberOfMainFrames)
		return E_FAIL;

	unsigned char layoutId;
	enum GetByteMode byteMode;

	g_TangraAdv2File->GetFrameImageSectionHeader(0, frameNo, &layoutId, &byteMode);

	AdvLib2::Adv2ImageLayout* layout;
	ADVRESULT rv = g_TangraAdv2File->ImageSection->GetImageLayoutById(layoutId, &layout);
	if (rv != S_OK) 
		return rv;

	g_TangraAdv2File->GetFrameSectionData(0, frameNo, pixels, frameInfo, systemErrorLen);

	return S_OK;
}

ADVRESULT ADV2GetFrameStatusChannel(int frameNo, AdvLib2::AdvFrameInfo* frameInfo)
{
	return E_NOTIMPL;
}

ADVRESULT ADV2GetFileTag(char* fileTagName, char* fileTagValue)
{
	return E_NOTIMPL;
}
