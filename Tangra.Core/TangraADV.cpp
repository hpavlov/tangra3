#include "PixelMapUtils.h"
#include "cross_platform.h"
#include "string"

#include "adv_file.h"
#include "IntegrationUtils.h"
#include "PreProcessing.h"
#include "TangraADV.h"

AdvLib::AdvFile* g_TangraAdvFile;
int g_MaxFrameBufferSize;
int prevFrameNo;
unsigned long* prevFramePixels;

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
}

HRESULT ADVOpenFile(char* fileName, AdvLib::AdvFileInfo* fileInfo)
{
	// Ensure file is closed
	EnsureAdvFileClosed();

	g_TangraAdvFile = new AdvLib::AdvFile();
	g_TangraAdvFile->OpenFile(fileName, fileInfo);

	g_MaxFrameBufferSize = g_TangraAdvFile->ImageSection->MaxFrameBufferSize() * 2 /*MaxBuff is calculated for ushorts*/;

	prevFrameNo = -1;
	prevFramePixels = (unsigned long*)malloc(g_MaxFrameBufferSize);

	return S_OK;
}


HRESULT ADVCloseFile()
{
	EnsureAdvFileClosed();

	return S_OK;
}

HRESULT ADVCropFile(char* newfileName, int firstFrameId, int lastFrameId)
{
	if (firstFrameId >= 0 && 
		lastFrameId <= g_TangraAdvFile->TotalNumberOfFrames && 
		firstFrameId <= lastFrameId)
    {
		g_TangraAdvFile->CropFile(newfileName, firstFrameId, lastFrameId);
	}
	
	return S_OK;
}

HRESULT ADVGetFramePixels(int frameNo, unsigned long* pixels, AdvLib::AdvFrameInfo* frameInfo, char* gpsFix, char* userCommand, char* systemError)
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

			unsigned long* keyFrameData = (unsigned long*)malloc(g_MaxFrameBufferSize);
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

HRESULT ADVGetFrame(int frameNo, unsigned long* pixels, BYTE* bitmapPixels, BYTE* bitmapBytes, AdvLib::AdvFrameInfo* frameInfo, char* gpsFix, char* userCommand, char* systemError)
{
	HRESULT rv = ADVGetFramePixels(frameNo, pixels, frameInfo, gpsFix, userCommand, systemError);
	if (SUCCEEDED(rv))
	{
		if (g_UsesPreProcessing) 
			return ApplyPreProcessing(pixels, g_TangraAdvFile->ImageSection->Width, g_TangraAdvFile->ImageSection->Height, g_TangraAdvFile->ImageSection->DataBpp, bitmapPixels, bitmapBytes);
		else
			return GetBitmapPixels(g_TangraAdvFile->ImageSection->Width, g_TangraAdvFile->ImageSection->Height, pixels, bitmapPixels, bitmapBytes, g_TangraAdvFile->ImageSection->ByteOrder == LittleEndian, g_TangraAdvFile->ImageSection->DataBpp);
	}
	
	return rv;
}

HRESULT ADVGetFrameStatusChannel(int frameNo, AdvLib::AdvFrameInfo* frameInfo, char* gpsFix, char* userCommand, char* systemError)
{
	g_TangraAdvFile->GetFrameStatusSectionData(frameNo, frameInfo, gpsFix, userCommand, systemError);
	
	return S_OK;
}

HRESULT ADVGetIntegratedFrame(int startFrameNo, int framesToIntegrate, bool isSlidingIntegration, bool isMedianAveraging, unsigned long* pixels, BYTE* bitmapBytes, BYTE* bitmapDisplayBytes, AdvLib::AdvFrameInfo* frameInfo)
{
	HRESULT rv;
	int firstFrameToIntegrate = IntegrationManagerGetFirstFrameToIntegrate(startFrameNo, framesToIntegrate, isSlidingIntegration);
	IntergationManagerStartNew(g_TangraAdvFile->ImageSection->Width, g_TangraAdvFile->ImageSection->Height, isMedianAveraging);

	AdvLib::AdvFrameInfo firstFrameInfo;
	AdvLib::AdvFrameInfo lastFrameInfo;
	long totalExposure10thMs = 0;
	
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
			rv = ApplyPreProcessingPixelsOnly(pixels, g_TangraAdvFile->ImageSection->Width, g_TangraAdvFile->ImageSection->Height, g_TangraAdvFile->ImageSection->DataBpp);

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
	frameInfo->StartTimeStampHi = (firstFrameInfo.StartTimeStampHi + lastFrameInfo.StartTimeStampHi) / 2;
	frameInfo->StartTimeStampLo = (firstFrameInfo.StartTimeStampLo + lastFrameInfo.StartTimeStampLo) / 2;
	 
	return GetBitmapPixels(g_TangraAdvFile->ImageSection->Width, g_TangraAdvFile->ImageSection->Height, pixels, bitmapBytes, bitmapDisplayBytes, false, g_TangraAdvFile->ImageSection->DataBpp);
}

HRESULT ADVGetFrame2(int frameNo, unsigned long* pixels, BYTE* bitmapPixels, BYTE* bitmapBytes)
{
	AdvLib::AdvFrameInfo* frameInfo = new AdvLib::AdvFrameInfo();
	ADVGetFrame(frameNo, pixels, bitmapPixels, bitmapBytes, frameInfo, NULL, NULL, NULL);
	delete frameInfo;

	return S_OK;
}