/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#include "StdAfx.h"
#include "adv_image_section.h"
#include "utils.h"
#include "stdlib.h"
#include "math.h"
#include <stdio.h>
#include <assert.h>

#include "adv_profiling.h"

namespace AdvLib
{

#define UNINITIALIZED_LAYOUT_ID 0	
unsigned char m_PreviousLayoutId;
unsigned int m_NumFramesInThisLayoutId;

AdvImageSection::AdvImageSection(unsigned int width, unsigned int height, unsigned char dataBpp)
{
	Width = width;
	Height = height;
	DataBpp = dataBpp;		
	
	m_PreviousLayoutId = UNINITIALIZED_LAYOUT_ID;
	m_NumFramesInThisLayoutId = 0;
}

AdvImageLayout* AdvImageSection::AddImageLayout(unsigned char layoutId, const char* layoutType, const char* compression, unsigned char bpp, int keyFrame)
{
	AdvLib::AdvImageLayout* layout = new AdvLib::AdvImageLayout(Width, Height, layoutId, layoutType, compression, bpp, keyFrame); 
	m_ImageLayouts.insert(make_pair(layoutId, layout));
	return layout;
}

AdvImageSection::~AdvImageSection()
{
	map<unsigned char, AdvImageLayout*>::iterator currIml = m_ImageLayouts.begin();
	while (currIml != m_ImageLayouts.end()) 
	{
		AdvImageLayout* imageLayout = currIml->second;	
		delete imageLayout;
		
		currIml++;
	}
	
	m_ImageLayouts.empty();
}

int m_MaxImageLayoutFrameBufferSize = -1;

int AdvImageSection::MaxFrameBufferSize()
{
	// Max frame buffer size is the max frame buffer size of the largest image layout
	if (m_MaxImageLayoutFrameBufferSize == -1)
	{
		map<unsigned char, AdvImageLayout*>::iterator curr = m_ImageLayouts.begin();
		while (curr != m_ImageLayouts.end()) 
		{
			int maxBuffSize = curr->second->MaxFrameBufferSize;
				
			if (m_MaxImageLayoutFrameBufferSize < maxBuffSize) 
				m_MaxImageLayoutFrameBufferSize = maxBuffSize;
			
			curr++;
		}		
	}
		
	return m_MaxImageLayoutFrameBufferSize;
}

void AdvImageSection::AddOrUpdateTag(const char* tagName, const char* tagValue)
{
	map<string, string>::iterator curr = m_ImageTags.begin();
	while (curr != m_ImageTags.end()) 
	{
		const char* existingTagName = curr->first.c_str();	
		
		if (0 == strcmp(existingTagName, tagName))
		{
			m_ImageTags.erase(curr);
			break;
		}
		
		curr++;
	}
	
	m_ImageTags.insert(make_pair(string(tagName), string(tagValue == NULL ? "" : tagValue)));
}

void AdvImageSection::BeginFrame()
{	
	
}

AdvImageLayout* AdvImageSection::GetImageLayoutById(unsigned char layoutId)
{
	map<unsigned char, AdvImageLayout*>::iterator curr = m_ImageLayouts.begin();
	while (curr != m_ImageLayouts.end()) 
	{
		unsigned char id =curr->first;
	
		if (id == layoutId)
			return curr->second;
			
		curr++;
	}
	
	return NULL;
}

unsigned char* AdvImageSection::GetDataBytes(unsigned char layoutId, unsigned short* currFramePixels, unsigned int *bytesCount, char* byteMode, unsigned char pixelsBpp)
{
	AdvImageLayout* currentLayout = GetImageLayoutById(layoutId);
	
	if (m_PreviousLayoutId == layoutId)
		m_NumFramesInThisLayoutId++;
	else
	{
		m_NumFramesInThisLayoutId = 0;
		currentLayout->StartNewDiffCorrSequence();
	}
	
	enum GetByteMode mode = Normal;
	
	if (currentLayout->IsDiffCorrLayout)
	{
		bool isKeyFrame = (m_NumFramesInThisLayoutId % currentLayout->KeyFrame) == 0;
		bool diffCorrFromPrevFramePixels = isKeyFrame || currentLayout->BaseFrameType == DiffCorrPrevFrame;
		
		if (isKeyFrame)
		{
			// this is a key frame
			mode = KeyFrameBytes;		
		}
		else
		{
			// this is not a key frame, compute and save the diff corr
			mode = DiffCorrBytes;
		}
	}	
	
	unsigned char* pixels = currentLayout->GetDataBytes(currFramePixels, mode, bytesCount, pixelsBpp);
	
	
	m_PreviousLayoutId = layoutId;
	*byteMode = (char)mode;
	
	return pixels;
}
	
unsigned int AdvImageSection::ComputePixelsCRC32(unsigned short* pixels)
{
	return compute_crc32((unsigned char*)pixels, 2 * Height * Width);
}
	
void AdvImageSection::WriteHeader(FILE* pFile)
{
	unsigned char buffChar;
	
	buffChar = 1;
	fwrite(&buffChar, 1, 1, pFile); /* Version */

	
	fwrite(&Width, 4, 1, pFile);
	fwrite(&Height, 4, 1, pFile);
	fwrite(&DataBpp, 1, 1, pFile);	
	
	buffChar = (unsigned char)m_ImageLayouts.size();
	fwrite(&buffChar, 1, 1, pFile);
	
	map<unsigned char, AdvImageLayout*>::iterator currIml = m_ImageLayouts.begin();
	while (currIml != m_ImageLayouts.end()) 
	{
		char layoutId = currIml->first;	
		fwrite(&layoutId, 1, 1, pFile);
		
		AdvImageLayout* imageLayout = currIml->second;	
		imageLayout->WriteHeader(pFile);
		
		currIml++;
	}
	
	buffChar = (unsigned char)m_ImageTags.size();
	fwrite(&buffChar, 1, 1, pFile);
	
	map<string, string>::iterator curr = m_ImageTags.begin();
	while (curr != m_ImageTags.end()) 
	{
		char* tagName = const_cast<char*>(curr->first.c_str());	
		WriteString(pFile, tagName);
		
		char* tagValue = const_cast<char*>(curr->second.c_str());	
		WriteString(pFile, tagValue);
		
		curr++;
	}
}

}

