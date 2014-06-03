/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#include "StdAfx.h"
#include <stdio.h>
#include <iostream>
#include <vector>
#include <stdlib.h>
#include <string.h>

#include "adv_lib.h"
#include "adv_image_layout.h"
#include "adv_profiling.h"


char* g_CurrentAdvFile;
AdvLib::AdvFile* g_AdvFile;
bool g_FileStarted = false;

using namespace std;

char* AdvGetCurrentFilePath(void)
{
	return g_CurrentAdvFile;
}

void AdvNewFile(const char* fileName)
{
	AdvProfiling_ResetPerformanceCounters();
	AdvProfiling_StartProcessing();
	
    if (NULL != g_AdvFile)
	{
		delete g_AdvFile;
		g_AdvFile = NULL;		
	}
	
	if (NULL != g_CurrentAdvFile)
	{
		delete g_CurrentAdvFile;
		g_CurrentAdvFile = NULL;
	}
	
	g_FileStarted = false;
	
	int len = strlen(fileName);	
	if (len > 0)
	{
		g_CurrentAdvFile = new char[len + 1];
		strncpy(g_CurrentAdvFile, fileName, len + 1);
	
		g_AdvFile = new AdvLib::AdvFile();	
	}
	AdvProfiling_EndProcessing();
}

void AdvEndFile()
{
	if (NULL != g_AdvFile)
	{
		g_AdvFile->EndFile();
		
		delete g_AdvFile;
		g_AdvFile = NULL;		
	}
	
	if (NULL != g_CurrentAdvFile)
	{
		delete g_CurrentAdvFile;
		g_CurrentAdvFile = NULL;
	}
	
	g_FileStarted = false;
}

void AdvDefineImageSection(unsigned short width, unsigned short height, unsigned char dataBpp)
{
	AdvProfiling_StartProcessing();
	AdvLib::AdvImageSection* imageSection = new AdvLib::AdvImageSection(width, height, dataBpp);
	g_AdvFile->AddImageSection(imageSection);
	AdvProfiling_EndProcessing();
}

void AdvDefineImageLayout(unsigned char layoutId, const char* layoutType, const char* compression, unsigned char bpp, int keyFrame, const char* diffCorrFromBaseFrame)
{	
	AdvProfiling_StartProcessing();
	AdvLib::AdvImageLayout* imageLayout = g_AdvFile->ImageSection->AddImageLayout(layoutId, layoutType, compression, bpp, keyFrame);
	if (diffCorrFromBaseFrame != NULL)
		imageLayout->AddOrUpdateTag("DIFFCODE-BASE-FRAME", diffCorrFromBaseFrame);
		
	AdvProfiling_EndProcessing();
}

unsigned int AdvDefineStatusSectionTag(const char* tagName, int tagType)
{
	AdvProfiling_StartProcessing();
	unsigned int statusTagId = g_AdvFile->StatusSection->DefineTag(tagName, (AdvTagType)tagType);
	AdvProfiling_EndProcessing();
	return statusTagId;
}

unsigned int AdvAddFileTag(const char* tagName, const char* tagValue)
{
	AdvProfiling_StartProcessing();	
	unsigned int fileTagId = g_AdvFile->AddFileTag(tagName, tagValue);
	AdvProfiling_EndProcessing();
	return fileTagId;
}

void AdvAddOrUpdateImageSectionTag(const char* tagName, const char* tagValue)
{
	AdvProfiling_StartProcessing();
	return g_AdvFile->ImageSection->AddOrUpdateTag(tagName, tagValue);
	AdvProfiling_EndProcessing();
}

bool AdvBeginFrame(long long timeStamp, unsigned int elapsedTime, unsigned int exposure)
{
	AdvProfiling_StartProcessing();
	if (!g_FileStarted)
	{
		bool success = g_AdvFile->BeginFile(g_CurrentAdvFile);
		if (success)
		{
			g_FileStarted = true;	
		}
		else
		{
			g_FileStarted = false;
			return false;
		}		
	}
	
	g_AdvFile->BeginFrame(timeStamp, elapsedTime, exposure);
	AdvProfiling_EndProcessing();
	return true;
}

void AdvFrameAddImageBytes(unsigned char layoutId,  unsigned char* pixels, unsigned char pixelsBpp)
{
	AdvProfiling_StartProcessing();
	g_AdvFile->AddFrameImage(layoutId, (unsigned short*)pixels, pixelsBpp);
	AdvProfiling_EndProcessing();
}

void AdvFrameAddImage(unsigned char layoutId,  unsigned short* pixels, unsigned char pixelsBpp)
{
	AdvProfiling_StartProcessing();
	g_AdvFile->AddFrameImage(layoutId, pixels, pixelsBpp);
	AdvProfiling_EndProcessing();
}

void AdvFrameAddStatusTag(unsigned int tagIndex, const char* tagValue)
{
	AdvProfiling_StartProcessing();
	g_AdvFile->AddFrameStatusTag(tagIndex, tagValue);
	AdvProfiling_EndProcessing();
}

void AdvFrameAddStatusTagMessage(unsigned int tagIndex, const char* tagValue)
{	
	AdvProfiling_StartProcessing();
	g_AdvFile->AddFrameStatusTagMessage(tagIndex, tagValue);
	AdvProfiling_EndProcessing();
}

void AdvFrameAddStatusTagUInt8(unsigned int tagIndex, unsigned char tagValue)
{
	AdvProfiling_StartProcessing();
	g_AdvFile->AddFrameStatusTagUInt8(tagIndex, tagValue);
	AdvProfiling_EndProcessing();	
}

void AdvFrameAddStatusTag32(unsigned int tagIndex, unsigned long tagValue)
{
	AdvProfiling_StartProcessing();
	g_AdvFile->AddFrameStatusTagUInt32(tagIndex, tagValue);
	AdvProfiling_EndProcessing();
}

void AdvFrameAddStatusTag64(unsigned int tagIndex, long long tagValue)
{
	AdvProfiling_StartProcessing();
	g_AdvFile->AddFrameStatusTagUInt64(tagIndex, tagValue);
	AdvProfiling_EndProcessing();
}

void AdvFrameAddStatusTag16(unsigned int tagIndex, unsigned short tagValue)
{
	AdvProfiling_StartProcessing();
	g_AdvFile->AddFrameStatusTagUInt16(tagIndex, tagValue);
	AdvProfiling_EndProcessing();	
}

void AdvFrameAddStatusTagReal(unsigned int tagIndex, float tagValue)
{
	AdvProfiling_StartProcessing();
	g_AdvFile->AddFrameStatusTagReal(tagIndex, tagValue);
	AdvProfiling_EndProcessing();	
}

void AdvEndFrame()
{
	AdvProfiling_StartProcessing();
	g_AdvFile->EndFrame();
	AdvProfiling_EndProcessing();
}