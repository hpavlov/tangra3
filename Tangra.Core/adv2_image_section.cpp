/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#include "stdafx.h"
#include "adv2_image_section.h"
#include "utils.h"
#include <cstdlib>

namespace AdvLib2
{

#define UNINITIALIZED_LAYOUT_ID 0	
unsigned char m_PreviousLayoutId;
unsigned int m_NumFramesInThisLayoutId;

Adv2ImageSection::Adv2ImageSection(unsigned int width, unsigned int height, unsigned char dataBpp)
{
	Width = width;
	Height = height;
	DataBpp = dataBpp;
	
	m_PreviousLayoutId = UNINITIALIZED_LAYOUT_ID;
	m_NumFramesInThisLayoutId = 0;
	ByteOrder = ImageByteOrder::LittleEndian;
	UsesCRC = false;
	MaxPixelValue = 0;
	m_RGBorBGR = false;
	m_SectionDefinitionMode = true;
}

Adv2ImageSection::~Adv2ImageSection()
{
	map<unsigned char, Adv2ImageLayout*>::iterator currIml = m_ImageLayouts.begin();
	while (currIml != m_ImageLayouts.end()) 
	{
		Adv2ImageLayout* imageLayout = currIml->second;
		delete imageLayout;
		
		currIml++;
	}
	
	m_ImageLayouts.empty();
}

ADVRESULT Adv2ImageSection::AddImageLayout(unsigned char layoutId, const char* layoutType, const char* compression, unsigned char layoutBpp)
{
	if (!m_SectionDefinitionMode)
		return E_ADV_CHANGE_NOT_ALLOWED_RIGHT_NOW;

	if (m_ImageLayouts.find(layoutId) != m_ImageLayouts.end())
		return E_ADV_IMAGE_LAYOUT_ALREADY_DEFINED;

	if (layoutType == nullptr)
		return E_ADV_INVALID_IMAGE_LAYOUT_TYPE;

	if (strcmp(layoutType, "FULL-IMAGE-RAW") != 0 &&
		strcmp(layoutType, "12BIT-IMAGE-PACKED") != 0 &&
		strcmp(layoutType, "8BIT-COLOR-IMAGE") != 0)
	{
		return E_ADV_INVALID_IMAGE_LAYOUT_TYPE;
	}

	if (compression == nullptr)
		return E_ADV_INVALID_IMAGE_LAYOUT_COMPRESSION;

	if (strcmp(compression, "UNCOMPRESSED") != 0 &&
		strcmp(compression, "LAGARITH16") != 0 &&
		strcmp(compression, "QUICKLZ") != 0)
	{
		return E_ADV_INVALID_IMAGE_LAYOUT_COMPRESSION;
	}

	if (layoutBpp == 0 || layoutBpp > 32)
		return E_ADV_INVALID_IMAGE_LAYOUT_BPP;

	Adv2ImageLayout* layout = new AdvLib2::Adv2ImageLayout(this, Width, Height, layoutId, layoutType, compression, layoutBpp); 
	m_ImageLayouts.insert(make_pair(layoutId, layout));
	return S_OK;
}

ADVRESULT Adv2ImageSection::AddOrUpdateTag(const char* tagName, const char* tagValue)
{
	if (!m_SectionDefinitionMode)
		return E_ADV_CHANGE_NOT_ALLOWED_RIGHT_NOW;

	ADVRESULT rv = S_OK;
	if (m_ImageTags.find(tagName) != m_ImageTags.end())
	{
		m_ImageTags.erase(tagName);
		rv = S_ADV_TAG_REPLACED;
	}

	if (strcmp("IMAGE-BYTE-ORDER", tagName) == 0)
	{
		ByteOrder = ImageByteOrder::LittleEndian;

		if (strcmp("BIG-ENDIAN", tagValue) == 0)
			ByteOrder = ImageByteOrder::BigEndian;
	}

	if (strcmp("SECTION-DATA-REDUNDANCY-CHECK", tagName) == 0)
	{
		UsesCRC = strcmp("CRC32", tagValue) == 0;
	}

	if (strcmp("IMAGE-MAX-PIXEL-VALUE", tagName) == 0 && tagValue != nullptr)
	{
		MaxPixelValue = atoi(tagValue);
	}

	if (strcmp("IMAGE-BAYER-PATTERN", tagName) == 0 && tagValue != nullptr)
	{
		m_RGBorBGR = strcmp("RGB", tagValue) == 0 || strcmp("BGR", tagValue) == 0;
		IsColourImage = strcmp("MONOCHROME", tagValue) != 0;
		strcpy_s(ImageBayerPattern, strlen(tagValue) + 1, tagValue);
	}
	
	m_ImageTags.insert(make_pair(string(tagName), string(tagValue == nullptr ? "" : tagValue)));
	return rv;
}

Adv2ImageSection::Adv2ImageSection(FILE* pFile, AdvFileInfo* fileInfo)
{
	unsigned char version;
	advfread(&version, 1, 1, pFile); /* Version */

	advfread(&Width, 4, 1, pFile);
	advfread(&Height, 4, 1, pFile);
	advfread(&DataBpp, 1, 1, pFile);

	ByteOrder = ImageByteOrder::LittleEndian;
	UsesCRC = false;
	IsColourImage = false;
	MaxPixelValue = 0;
	m_RGBorBGR = false;

	unsigned char imageLayouts;
	advfread(&imageLayouts, 1, 1, pFile);

	for (int i = 0; i < imageLayouts; i++)
	{
		char layoutId;
		advfread(&layoutId, 1, 1, pFile);

		Adv2ImageLayout* imageLayout = new AdvLib2::Adv2ImageLayout(this, layoutId, pFile);
		m_ImageLayouts.insert(make_pair(layoutId, imageLayout));
	}

	unsigned char tagsCount;
	advfread(&tagsCount, 1, 1, pFile);

	for (int i = 0; i < tagsCount; i++)
	{
		char* tagName = ReadUTF8String(pFile);
		char* tagValue = ReadUTF8String(pFile);

		AddOrUpdateTag(tagName, tagValue);
	}

	fileInfo->Width = Width;
	fileInfo->Height = Height;
	fileInfo->DataBpp = DataBpp;
	fileInfo->MaxPixelValue = MaxPixelValue;
	fileInfo->IsColourImage = IsColourImage;
	fileInfo->ImageLayoutsCount = (int)m_ImageLayouts.size();
	fileInfo->ImageSectionTagsCount = m_ImageTags.size();	

	m_SectionDefinitionMode = false;
}

void Adv2ImageSection::WriteHeader(FILE* pFile)
{
	unsigned char buffChar;
	
	buffChar = 2;
	advfwrite(&buffChar, 1, 1, pFile); /* Version */

	
	advfwrite(&Width, 4, 1, pFile);
	advfwrite(&Height, 4, 1, pFile);
	advfwrite(&DataBpp, 1, 1, pFile);
	
	buffChar = (unsigned char)m_ImageLayouts.size();
	advfwrite(&buffChar, 1, 1, pFile);
	
	map<unsigned char, Adv2ImageLayout*>::iterator currIml = m_ImageLayouts.begin();
	while (currIml != m_ImageLayouts.end()) 
	{
		char layoutId = currIml->first;	
		advfwrite(&layoutId, 1, 1, pFile);
		
		Adv2ImageLayout* imageLayout = currIml->second;	
		imageLayout->WriteHeader(pFile);
		
		currIml++;
	}
	
	buffChar = (unsigned char)m_ImageTags.size();
	advfwrite(&buffChar, 1, 1, pFile);
	
	map<string, string>::iterator curr = m_ImageTags.begin();
	while (curr != m_ImageTags.end()) 
	{
		char* tagName = const_cast<char*>(curr->first.c_str());
		WriteUTF8String(pFile, tagName);
		
		char* tagValue = const_cast<char*>(curr->second.c_str());
		WriteUTF8String(pFile, tagValue);
		
		curr++;
	}

	m_SectionDefinitionMode = false;
}

ADVRESULT Adv2ImageSection::BeginFrame()
{
	if (m_ImageLayouts.size() == 0)
		return E_ADV_IMAGE_LAYOUTS_UNDEFINED;

	m_SectionDefinitionMode = false;

	return S_OK;
}

int m_MaxImageLayoutFrameBufferSize = -1;

int Adv2ImageSection::MaxFrameBufferSize()
{
	// Max frame buffer size is the max frame buffer size of the largest image layout
	if (m_MaxImageLayoutFrameBufferSize == -1)
	{
		map<unsigned char, Adv2ImageLayout*>::iterator curr = m_ImageLayouts.begin();
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

unsigned char* Adv2ImageSection::GetDataBytes(unsigned char layoutId, unsigned short* currFramePixels, unsigned int *bytesCount, unsigned char pixelsBpp, enum GetByteOperation operation)
{
	Adv2ImageLayout* currentLayout;
	ADVRESULT rv = GetImageLayoutById(layoutId, &currentLayout);	
	
	if (m_PreviousLayoutId == layoutId)
		m_NumFramesInThisLayoutId++;
	else
	{
		m_NumFramesInThisLayoutId = 0;
	}
	
	unsigned char* pixels = currentLayout->GetDataBytes(currFramePixels, bytesCount, pixelsBpp, operation);	
	
	m_PreviousLayoutId = layoutId;
	
	return pixels;
}

ADVRESULT Adv2ImageSection::GetImageLayoutById(unsigned char layoutId, AdvLib2::Adv2ImageLayout** layout)
{
	map<unsigned char, Adv2ImageLayout*>::iterator curr = m_ImageLayouts.find(layoutId);
	if (curr != m_ImageLayouts.end()) 
	{
		*layout = curr->second;
		return S_OK;
	}

	return E_ADV_INVALID_IMAGE_LAYOUT_ID;
}

void Adv2ImageSection::GetDataFromDataBytes(unsigned char* data, unsigned int* pixels, int sectionDataLength, int startOffset)
{
	unsigned char* sectionData = data + startOffset;
	unsigned char layoutId = *sectionData;
	sectionData++;

	enum GetByteMode reservedUnusedField = (GetByteMode)*sectionData;
	sectionData++;

	Adv2ImageLayout* imageLayout;
	ADVRESULT rv = GetImageLayoutById(layoutId, &imageLayout);	

	imageLayout->GetDataFromDataBytes(data, pixels, sectionDataLength - 2, startOffset + 2);
}

ADVRESULT Adv2ImageSection::GetImageSectionTagSizes(int tagId, int* tagNameSize, int* tagValueSize)
{
	if (tagId < 0 || tagId >= m_ImageTags.size())
		return E_FAIL;

	map<string, string>::iterator iter = m_ImageTags.begin();
	if (tagId > 0) std::advance(iter, tagId);	

	*tagNameSize = (int)iter->first.size();
	*tagValueSize = (int)iter->second.size();

	return S_OK;
}

ADVRESULT Adv2ImageSection::GetImageLayoutTagSizes(int layoutId, int tagId, int* tagNameSize, int* tagValueSize)
{
	Adv2ImageLayout* imageLayout;
	ADVRESULT rv = GetImageLayoutById(layoutId, &imageLayout);	

	if (rv != S_OK)
		return rv;
	else
		return imageLayout->GetImageLayoutTagSizes(tagId, tagNameSize, tagValueSize);
}

ADVRESULT Adv2ImageSection::GetImageLayoutInfo(int layoutIndex, AdvLib2::AdvImageLayoutInfo* imageLayoutInfo)
{
	if (layoutIndex < 0 || layoutIndex >= m_ImageLayouts.size())
		return E_FAIL;

	map<unsigned char, Adv2ImageLayout*>::iterator it = m_ImageLayouts.begin();
	std::advance(it, layoutIndex);

	if (it != m_ImageLayouts.end()) 
		return it->second->GetImageLayoutInfo(imageLayoutInfo);
	else
		return E_FAIL;
}

ADVRESULT Adv2ImageSection::GetImageSectionTag(int tagId, char* tagName, char* tagValue)
{
	if (tagId < 0 || tagId >= m_ImageTags.size())
		return E_FAIL;

	map<string, string>::iterator iter = m_ImageTags.begin();
	if (tagId > 0) std::advance(iter, tagId);	

	strcpy_s(tagName, iter->first.size() + 1, iter->first.c_str());
	strcpy_s(tagValue, iter->second.size() + 1, iter->second.c_str());

	return S_OK;
}

ADVRESULT Adv2ImageSection::GetImageLayoutTag(int layoutId, int tagId, char* tagName, char* tagValue)
{
	Adv2ImageLayout* imageLayout;
	ADVRESULT rv = GetImageLayoutById(layoutId, &imageLayout);	

	if (rv != S_OK)
		return rv;
	else
		return imageLayout->GetImageLayoutTag(tagId, tagName, tagValue);
}

}