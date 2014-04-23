#include "adv_image_section.h"
#include "utils.h"
#include "stdlib.h"
#include "math.h"
#include <stdio.h>
#include <assert.h>

namespace AdvLib
{

#define UNINITIALIZED_LAYOUT_ID 0	
unsigned char m_PreviousLayoutId;
unsigned int m_NumFramesInThisLayoutId;

AdvImageSection::~AdvImageSection()
{
	map<unsigned char, AdvImageLayout*>::iterator currIml = m_ImageLayouts.begin();
	while (currIml != m_ImageLayouts.end()) 
	{
		AdvImageLayout* imageLayout = currIml->second;	
		delete imageLayout;
		
		currIml++;
	}
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
	map<const char*, const char*>::iterator curr = m_ImageTags.begin();
	while (curr != m_ImageTags.end())
	{
		const char* existingTagName = curr->first;	
		
		if (0 == strcmp(existingTagName, tagName))
		{
			m_ImageTags.erase(curr);
			break;
		}
		
		curr++;
	}
	
	m_ImageTags.insert(make_pair(tagName, tagValue));

	if (0 == strcmp("IMAGE-BYTE-ORDER", tagName))
	{
		ByteOrder = BigEndian;
		if (0 == strcmp("LITTLE-ENDIAN", tagValue)) ByteOrder = LittleEndian;
	}

	if (0 == strcmp("IMAGE-DYNABITS", tagName))
	{
		if (0 == strcmp("12", tagValue)) DynaBits = 12;
		if (0 == strcmp("14", tagValue)) DynaBits = 14;
		if (0 == strcmp("16", tagValue)) DynaBits = 16;
	}
	
	if (0 == strcmp("SECTION-DATA-REDUNDANCY-CHECK", tagName))
	{
		UsesCRC = false;
		if (0 == strcmp("CRC32", tagValue)) UsesCRC = true;
	}
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

AdvImageSection::AdvImageSection(FILE* pFile)
{
	unsigned char version;
	fread(&version, 1, 1, pFile);

	if (version >= 1)
	{
		fread(&Width, 4, 1, pFile);
		fread(&Height, 4, 1, pFile);
		fread(&DataBpp, 1, 1, pFile);	

		DynaBits = 16; // Default value for old ADV files

		unsigned char numLayouts;
		fread(&numLayouts, 1, 1, pFile);
		
		m_ImageLayouts.clear();

		for(int i = 0; i < numLayouts; i++)
		{
			unsigned char layoutId;
			fread(&layoutId, 1, 1, pFile);	

			AdvImageLayout* imageLayout = new AdvImageLayout(this, layoutId, Width, Height, DataBpp, pFile);
			m_ImageLayouts.insert(make_pair(imageLayout->LayoutId, imageLayout));
		}

		unsigned char numImageTags;
		fread(&numImageTags, 1, 1, pFile);
		
		m_ImageTags.clear();

		for(int i = 0; i < numImageTags; i++)
		{
			char* tagName = ReadString(pFile);
			char* tagValue = ReadString(pFile);

			AddOrUpdateTag(tagName, tagValue);

			delete tagName;
			delete tagValue;
		}
	}
}

void AdvImageSection::GetDataFromDataBytes(unsigned char* data, unsigned long* prevFrame, unsigned long* pixels, int sectionDataLength, int startOffset)
{
	unsigned char* sectionData = data + startOffset;
	unsigned char layoutId = *sectionData;
	sectionData++;

	enum GetByteMode byteMode = (GetByteMode)*sectionData;
	sectionData++;

	AdvImageLayout* imageLayout = GetImageLayoutById(layoutId);	
	imageLayout->GetDataFromDataBytes(byteMode, data, prevFrame, pixels, sectionDataLength - 2, startOffset + 2);
}

}