/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#include "StdAfx.h"
#include "adv_file.h"
#include <iostream>
#include <stdlib.h>
#include <stdio.h>
#include <vector>
#include "utils.h"
#include "cross_platform.h"
#include "adv_profiling.h"

using namespace std;

namespace AdvLib
{

FILE* m_File;
	
AdvFile::AdvFile()
{
	StatusSection = new AdvLib::AdvStatusSection();
	
	crc32_init();	
	
	m_FrameBytes = NULL;
}

AdvFile::~AdvFile()
{
	if (NULL != m_File)
	{
		advfclose(m_File);
		m_File = NULL;
	}
	
	if (NULL != ImageSection)
	{
		delete ImageSection;
		ImageSection = NULL;
	}
	
	if (NULL != StatusSection)
	{
		delete StatusSection;
		StatusSection = NULL;
	}
	
	if (NULL != m_Index)
	{
		delete m_Index;
		m_Index = NULL;
	}
	
	if (NULL != m_FrameBytes)
	{
		delete m_FrameBytes;
		m_FrameBytes = NULL;
	}

	m_UserMetadataTags.clear();
	m_FileTags.clear();
}

unsigned char CURRENT_DATAFORMAT_VERSION = 1;

bool AdvFile::BeginFile(const char* fileName)
{
	m_File = advfopen(fileName, "wb");
	if (m_File == 0) return false;
	
	unsigned int buffInt;
	unsigned long buffLong;
	unsigned char buffChar;
	
	buffInt = 0x46545346;
	advfwrite(&buffInt, 4, 1, m_File);
	advfwrite(&CURRENT_DATAFORMAT_VERSION, 1, 1, m_File);

	buffInt = 0;
	buffLong = 0;
	advfwrite(&buffInt, 4, 1, m_File); // Number of frames (will be saved later) 
	advfwrite(&buffLong, 8, 1, m_File); // Offset of index table (will be saved later) 
	advfwrite(&buffLong, 8, 1, m_File); // Offset of system metadata table (will be saved later) 
	advfwrite(&buffLong, 8, 1, m_File); // Offset of user metadata table (will be saved later) 
	
	buffChar = (unsigned char)2;
	advfwrite(&buffChar, 1, 1, m_File); // Number of sections (image and status) 

	__int64 sectionHeaderOffsetPositions[2];
	
	WriteString(m_File, "IMAGE");	
	advfgetpos64(m_File, &sectionHeaderOffsetPositions[0]);
	buffLong = 0;
	advfwrite(&buffLong, 8, 1, m_File);
	
	WriteString(m_File, "STATUS");	
	advfgetpos64(m_File, &sectionHeaderOffsetPositions[1]);
	buffLong = 0;
	advfwrite(&buffLong, 8, 1, m_File);

	// Write section headers
	__int64 sectionHeaderOffsets[2];
	advfgetpos64(m_File, &sectionHeaderOffsets[0]);
	ImageSection->WriteHeader(m_File);
	advfgetpos64(m_File, &sectionHeaderOffsets[1]);
	StatusSection->WriteHeader(m_File);

	// Write section headers positions
	advfsetpos64(m_File, &sectionHeaderOffsetPositions[0]);
	advfwrite(&sectionHeaderOffsets[0], 8, 1, m_File);
	advfsetpos64(m_File, &sectionHeaderOffsetPositions[1]);
	advfwrite(&sectionHeaderOffsets[1], 8, 1, m_File);
		
	advfseek(m_File, 0, SEEK_END);
	
	// Write system metadata table
	__int64 systemMetadataTablePosition;
	advfgetpos64(m_File, &systemMetadataTablePosition);
	
	unsigned int fileTagsCount = m_FileTags.size();
	advfwrite(&fileTagsCount, 4, 1, m_File);
	
	map<string, string>::iterator curr = m_FileTags.begin();
	while (curr != m_FileTags.end()) 
	{
		char* tagName = const_cast<char*>(curr->first.c_str());	
		WriteString(m_File, tagName);
		
		char* tagValue = const_cast<char*>(curr->second.c_str());	
		WriteString(m_File, tagValue);
		
		curr++;
	}
	
	// Write system metadata table position to the file header
	advfseek(m_File, 0x11, SEEK_SET);
	advfwrite(&systemMetadataTablePosition, 8, 1, m_File);
	
	advfseek(m_File, 0, SEEK_END);
	
    m_Index = new AdvLib::AdvFramesIndex();	
	
	advfflush(m_File);
		
	m_FrameNo = 0;
	m_UserMetadataTags.clear();

	return true;
}

void AdvFile::EndFile()
{
	__int64 indexTableOffset;
	advfgetpos64(m_File, &indexTableOffset);
	
	m_Index->WriteIndex(m_File);
		
	__int64 userMetaTableOffset;
	advfgetpos64(m_File, &userMetaTableOffset);

	advfseek(m_File, 5, SEEK_SET);
	advfwrite(&m_FrameNo, 4, 1, m_File);
	advfwrite(&indexTableOffset, 8, 1, m_File);	
	advfseek(m_File, 0x19, SEEK_SET);	
	advfwrite(&userMetaTableOffset, 8, 1, m_File);
		
	// Write the metadata table
	advfseek(m_File, 0, SEEK_END);	

	unsigned int userTagsCount = m_UserMetadataTags.size();
	advfwrite(&userTagsCount, 4, 1, m_File);
	
	map<string, string>::iterator curr = m_UserMetadataTags.begin();
	while (curr != m_UserMetadataTags.end()) 
	{
		char* userTagName = const_cast<char*>(curr->first.c_str());	
		WriteString(m_File, userTagName);
		
		char* userTagValue = const_cast<char*>(curr->second.c_str());	
		WriteString(m_File, userTagValue);
		
		curr++;
	}
	
	
	advfflush(m_File);
	advfclose(m_File);	
	
	m_File = NULL;
}

void AdvFile::AddImageSection(AdvLib::AdvImageSection* section)
{
	ImageSection = section;	

	char convStr [10];
	snprintf(convStr, 10, "%d", section->Width);
	m_FileTags.insert(make_pair(string("WIDTH"), string(convStr)));
	
	snprintf(convStr, 10, "%d", section->Height);
	m_FileTags.insert(make_pair(string("HEIGHT"), string(convStr)));
	
	snprintf(convStr, 10, "%d", section->DataBpp);
	m_FileTags.insert(make_pair(string("BITPIX"), string(convStr)));
}

int AdvFile::AddFileTag(const char* tagName, const char* tagValue)
{	
	m_FileTags.insert((make_pair(string(tagName == NULL ? "" : tagName), string(tagValue == NULL ? "" : tagValue))));
	
	return m_FileTags.size();	
}

int AdvFile::AddUserTag(const char* tagName, const char* tagValue)
{
	m_UserMetadataTags.insert((make_pair(string(tagName == NULL ? "" : tagName), string(tagValue == NULL ? "" : tagValue))));
	
	return m_UserMetadataTags.size();	
}

void AdvFile::BeginFrame(long long timeStamp, unsigned int elapsedTime, unsigned int exposure)
{
	AdvProfiling_StartBytesOperation();

	advfgetpos64(m_File, &m_NewFrameOffset);

	m_FrameBufferIndex = 0;
	
	m_ElapedTime = elapsedTime;
		
	if (m_FrameBytes == NULL)
	{
		int maxUncompressedBufferSize = 
			4 + // frame start magic
			8 + // timestamp
			4 + // exposure			
			4 + 4 + // the length of each of the 2 sections 
			StatusSection->MaxFrameBufferSize +
			ImageSection->MaxFrameBufferSize() + 
			100; // Just in case
		
		m_FrameBytes = new unsigned char[maxUncompressedBufferSize];
	};		
	
	// Add the timestamp
	m_FrameBytes[0] = (unsigned char)(timeStamp & 0xFF);
	m_FrameBytes[1] = (unsigned char)((timeStamp >> 8) & 0xFF);
	m_FrameBytes[2] = (unsigned char)((timeStamp >> 16) & 0xFF);
	m_FrameBytes[3] = (unsigned char)((timeStamp >> 24) & 0xFF);
	m_FrameBytes[4] = (unsigned char)((timeStamp >> 32) & 0xFF);
	m_FrameBytes[5] = (unsigned char)((timeStamp >> 40) & 0xFF);
	m_FrameBytes[6] = (unsigned char)((timeStamp >> 48) & 0xFF);
	m_FrameBytes[7] = (unsigned char)((timeStamp >> 56) & 0xFF);
	
	// Add the exposure
	m_FrameBytes[8] = (unsigned char)(exposure & 0xFF);
	m_FrameBytes[9] = (unsigned char)((exposure >> 8) & 0xFF);
	m_FrameBytes[10] = (unsigned char)((exposure >> 16) & 0xFF);
	m_FrameBytes[11] = (unsigned char)((exposure >> 24) & 0xFF);
	
	m_FrameBufferIndex = 12;
	
	StatusSection->BeginFrame();
	ImageSection->BeginFrame();	
	
	AdvProfiling_EndBytesOperation();
}

void AdvFile::AddFrameStatusTag(unsigned int tagIndex, const char* tagValue)
{
	StatusSection->AddFrameStatusTag(tagIndex, tagValue);
}

void AdvFile::AddFrameStatusTagMessage(unsigned int tagIndex, const char* tagValue)
{
	StatusSection->AddFrameStatusTagMessage(tagIndex, tagValue);
}

void AdvFile::AddFrameStatusTagUInt16(unsigned int tagIndex, unsigned short tagValue)
{
	StatusSection->AddFrameStatusTagUInt16(tagIndex, tagValue);
}

void AdvFile::AddFrameStatusTagReal(unsigned int tagIndex, float tagValue)
{
	StatusSection->AddFrameStatusTagReal(tagIndex, tagValue);
}

void AdvFile::AddFrameStatusTagUInt8(unsigned int tagIndex, unsigned char tagValue)
{
	StatusSection->AddFrameStatusTagUInt8(tagIndex, tagValue);
}

void AdvFile::AddFrameStatusTagUInt32(unsigned int tagIndex, unsigned int tagValue)
{
	StatusSection->AddFrameStatusTagUInt32(tagIndex, tagValue);
}

void AdvFile::AddFrameStatusTagUInt64(unsigned int tagIndex, long long tagValue)
{
	StatusSection->AddFrameStatusTagUInt64(tagIndex, tagValue);
}

void AdvFile::AddFrameImage(unsigned char layoutId, unsigned short* pixels, unsigned char pixelsBpp)
{
	AdvProfiling_StartGenericProcessing();
	AdvProfiling_StartBytesOperation();
	
	unsigned int imageBytesCount = 0;	
	char byteMode = 0;
	m_CurrentImageLayout = ImageSection->GetImageLayoutById(layoutId);
	unsigned char *imageBytes = ImageSection->GetDataBytes(layoutId, pixels, &imageBytesCount, &byteMode, pixelsBpp);
	
	int imageSectionBytesCount = imageBytesCount + 2; // +1 byte for the layout id and +1 byte for the byteMode (See few lines below)
	
	m_FrameBytes[m_FrameBufferIndex] = imageSectionBytesCount & 0xFF;
	m_FrameBytes[m_FrameBufferIndex + 1] = (imageSectionBytesCount >> 8) & 0xFF;
	m_FrameBytes[m_FrameBufferIndex + 2] = (imageSectionBytesCount >> 16) & 0xFF;
	m_FrameBytes[m_FrameBufferIndex + 3] = (imageSectionBytesCount >> 24) & 0xFF;
	m_FrameBufferIndex+=4;
	
	// It is faster to write the layoutId and byteMode directly here
	m_FrameBytes[m_FrameBufferIndex] = m_CurrentImageLayout->LayoutId;
	m_FrameBytes[m_FrameBufferIndex + 1] = byteMode;
	m_FrameBufferIndex+=2;	
		
	memcpy(&m_FrameBytes[m_FrameBufferIndex], &imageBytes[0], imageBytesCount);
	m_FrameBufferIndex+= imageBytesCount;
		
	unsigned int statusBytesCount = 0;
	unsigned char *statusBytes = StatusSection->GetDataBytes(&statusBytesCount);
	
	m_FrameBytes[m_FrameBufferIndex] = statusBytesCount & 0xFF;
	m_FrameBytes[m_FrameBufferIndex + 1] = (statusBytesCount >> 8) & 0xFF;
	m_FrameBytes[m_FrameBufferIndex + 2] = (statusBytesCount >> 16) & 0xFF;
	m_FrameBytes[m_FrameBufferIndex + 3] = (statusBytesCount >> 24) & 0xFF;
	m_FrameBufferIndex+=4;
	
	if (statusBytesCount > 0)
	{
		memcpy(&m_FrameBytes[m_FrameBufferIndex], &statusBytes[0], statusBytesCount);
		m_FrameBufferIndex+=statusBytesCount;

		delete statusBytes;		
	}
	
	AdvProfiling_EndBytesOperation();
	AdvProfiling_EndGenericProcessing();
}
			
void AdvFile::EndFrame()
{	
	AdvProfiling_StartGenericProcessing();
	
	__int64 frameOffset;
	advfgetpos64(m_File, &frameOffset);
		
	// Frame start magic
	unsigned int frameStartMagic = 0xEE0122FF;
	advfwrite(&frameStartMagic, 4, 1, m_File);	
	
	advfwrite(m_FrameBytes, m_FrameBufferIndex, 1, m_File);
		
	m_Index->AddFrame(m_FrameNo, m_ElapedTime, frameOffset, m_FrameBufferIndex);
	
	advfflush(m_File);
	
	m_FrameNo++;
	AdvProfiling_NewFrameProcessed();
	
	AdvProfiling_EndGenericProcessing();
}

}
