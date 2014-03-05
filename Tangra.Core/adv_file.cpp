#include "adv_file.h"
#include <iostream>
#include <stdlib.h>
#include <stdio.h>
#include <vector>
#include "utils.h"
#include "cross_platform.h"

// http://stackoverflow.com/questions/965725/large-file-support-in-c
// -D_FILE_OFFSET_BITS=64


// To run cl.exe: http://i-am-bryan.com/webs/tutorials/fix-cl-exe-exited-with-code-1073741515-cannot-run-rc-exe/

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
		fclose(m_File);
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
}

unsigned char CURRENT_DATAFORMAT_VERSION = 1;

void AdvFile::OpenFile(const char* fileName, AdvFileInfo* fileInfo)
{
	m_File = fopen(fileName, "rb");

	unsigned int buffInt;
	unsigned char buffChar;
	
	fread(&buffInt, 4, 1, m_File);
	if (buffInt != 0x46545346)
		return;

	unsigned char dataFormatVer;
	fread(&dataFormatVer, 1, 1, m_File);

	fread(&TotalNumberOfFrames, 4, 1, m_File);

    __int64 indexTableOffset;
	__int64 metadataSystemTableOffset;
	__int64 metadataUserTableOffset;

	fread(&indexTableOffset, 8, 1, m_File);
	fread(&metadataSystemTableOffset, 8, 1, m_File);
	fread(&metadataUserTableOffset, 8, 1, m_File);

	if (dataFormatVer >= 1)
    {
        char sectionsCount;
		fread(&sectionsCount, 1, 1, m_File);

        map<__int64, string> sectionDefs;

        for (int i = 0; i < sectionsCount; i++)
        {
			char* sectionType = ReadString(m_File);

			__int64 sectionHeaderOffset;
			fread(&sectionHeaderOffset, 8, 1, m_File);

			sectionDefs.insert(make_pair((long long)(sectionHeaderOffset), string(sectionType)));

			delete sectionType;
        }

		map<__int64, string>::iterator curr = sectionDefs.begin();
		while (curr != sectionDefs.end()) 
		{			
			advfsetpos(m_File, &curr->first);
			const char* sectionName = curr->second.c_str();

			if (0 == strcmp("IMAGE", sectionName))
			{
				ImageSection = new AdvLib::AdvImageSection(m_File);
				
			}
			else if (0 == strcmp("STATUS", sectionName))
			{
				StatusSection = new AdvLib::AdvStatusSection(m_File);
			}

			curr++;
		}

        advfsetpos(m_File, &indexTableOffset);
		m_Index = new AdvLib::AdvFramesIndex(m_File);

		// Reads the system metadata table
		advfsetpos(m_File, &metadataSystemTableOffset);

		unsigned int numTags;
		fread(&numTags, 4, 1, m_File);
		
		m_FileTags.clear();
		
		for(int i = 0; i < numTags; i++)
		{
			char* tagName = ReadString(m_File);
			char* tagValue = ReadString(m_File);

			m_FileTags.insert(make_pair(string(tagName), string(tagValue)));
				
			delete tagName;
			delete tagValue;
		}

		// Reads the user metadata table
		advfsetpos(m_File, &metadataUserTableOffset);

		fread(&numTags, 4, 1, m_File);
			
		for(int i = 0; i < numTags; i++)
		{
			char* tagName = ReadString(m_File);
			char* tagValue = ReadString(m_File);

			m_FileTags.insert(make_pair(string(tagName), string(tagValue)));

			delete tagName;
			delete tagValue;
		}
    }
			
	char fileTagValue[256];
	GetFileTag("FSTF-TYPE", &fileTagValue[0]);
	bool isAAVFile = strcmp(fileTagValue, "AAV") == 0;	
	GetFileTag("OCR-ENGINE", &fileTagValue[0]);
	bool hasAAVEmbeddedTimestamps = strlen(fileTagValue) > 0;
	GetFileTag("EFFECTIVE-FRAME-RATE", &fileTagValue[0]);
	float effectiveFrameRate = 0;
	if (strlen(fileTagValue) > 0)
	{
		char* pEnd;
		effectiveFrameRate = strtod(fileTagValue, &pEnd);
	}	
	GetFileTag("AAV16-NORMVAL", &fileTagValue[0]);
	if (strlen(fileTagValue) > 0)
	{
		char* pEnd;
		ImageSection->NormalisationValue = strtod(fileTagValue, &pEnd);
	}
	else
		ImageSection->NormalisationValue = 0;	
			
	fileInfo->Bpp = ImageSection->DataBpp;
	fileInfo->Aav16NormVal = ImageSection->NormalisationValue;
	fileInfo->CountFrames = TotalNumberOfFrames;
	fileInfo->Height = ImageSection->Height;
	fileInfo->Width = ImageSection->Width;
	
	if (isAAVFile)
		// For AAV files read the frame rate from the AAV file header	
		fileInfo->FrameRate = effectiveFrameRate;
	else
		// For ADV files and for AAV files that have embedded timestamps, the framerate will be ignored
		fileInfo->FrameRate = 0; // This is variable
}

void AdvFile::GetFileTag(const char* fileTagName, char* fileTagValue)
{
	map<string, string>::iterator itTag = m_FileTags.find(fileTagName);
	if (itTag != m_FileTags.end())
	{
		strcpy(&fileTagValue[0], &itTag->second.c_str()[0]);
	}
}

void AdvFile::GetFrameImageSectionHeader(int frameId, unsigned char* layoutId, enum GetByteMode* mode)
{
	AdvLib::IndexEntry* indexEntry = m_Index->GetIndexForFrame(frameId);

	advfsetpos(m_File, &indexEntry->FrameOffset);


	long frameDataMagic;
	fread(&frameDataMagic, 4, 1, m_File);

	if (frameDataMagic == 0xEE0122FF)
	{
		// Skip 16 bytes forward
		long ignoredValue;
		fread(&ignoredValue, 4, 1, m_File);
		fread(&ignoredValue, 4, 1, m_File);
		fread(&ignoredValue, 4, 1, m_File);
		fread(&ignoredValue, 4, 1, m_File);

		fread(layoutId, 1, 1, m_File);

		unsigned char byteMode;

		fread(&byteMode, 1, 1, m_File);

		*mode = (GetByteMode)byteMode;
	}
}

void AdvFile::GetFrameSectionData(int frameId, unsigned long* prevFrame, unsigned long* pixels, AdvFrameInfo* frameInfo, char* gpsFix, char* userCommand, char* systemError)
{
	 AdvLib::IndexEntry* indexEntry = m_Index->GetIndexForFrame(frameId);

	advfsetpos(m_File, &indexEntry->FrameOffset);

	long frameDataMagic;
	fread(&frameDataMagic, 4, 1, m_File);

	if (frameDataMagic == 0xEE0122FF)
	{
		unsigned char* data = (unsigned char*)malloc(indexEntry->BytesCount);
		fread(data, indexEntry->BytesCount, 1, m_File);

		// Read the timestamp and exposure 
		frameInfo->StartTimeStampLo = data[0] + (data[1] << 8) + (data[2] << 16) + (data[3] << 24);
		frameInfo->StartTimeStampHi = data[4] + (data[5] << 8) + (data[6] << 16) + (data[7] << 24);
    	frameInfo->Exposure10thMs = data[8] + (data[9] << 8) + (data[10] << 16) + (data[11] << 24);

	    int dataOffset = 12;
		int sectionDataLength = data[dataOffset] + (data[dataOffset + 1] << 8) + (data[dataOffset + 2] << 16) + (data[dataOffset + 3] << 24);

		ImageSection->GetDataFromDataBytes(data, prevFrame, pixels, sectionDataLength, dataOffset + 4);
		dataOffset += sectionDataLength + 4;

		sectionDataLength = data[dataOffset] + (data[dataOffset + 1] << 8) + (data[dataOffset + 2] << 16) + (data[dataOffset + 3] << 24);
		StatusSection->GetDataFromDataBytes(data, sectionDataLength, dataOffset + 4, frameInfo, gpsFix, userCommand, systemError);
		
		delete data;
	}
}

void AdvFile::GetFrameStatusSectionData(int frameId, AdvFrameInfo* frameInfo, char* gpsFix, char* userCommand, char* systemError)
{
	 AdvLib::IndexEntry* indexEntry = m_Index->GetIndexForFrame(frameId);

	advfsetpos(m_File, &indexEntry->FrameOffset);

	long frameDataMagic;
	fread(&frameDataMagic, 4, 1, m_File);

	if (frameDataMagic == 0xEE0122FF)
	{
		unsigned char* data = (unsigned char*)malloc(indexEntry->BytesCount);
		fread(data, indexEntry->BytesCount, 1, m_File);

		frameInfo->StartTimeStampLo = data[0] + (data[1] << 8) + (data[2] << 16) + (data[3] << 24);
		frameInfo->StartTimeStampHi = data[4] + (data[5] << 8) + (data[6] << 16) + (data[7] << 24);
    	frameInfo->Exposure10thMs = data[8] + (data[9] << 8) + (data[10] << 16) + (data[11] << 24);

	    int dataOffset = 12;
		int sectionDataLength = data[dataOffset] + (data[dataOffset + 1] << 8) + (data[dataOffset + 2] << 16) + (data[dataOffset + 3] << 24);

		dataOffset += sectionDataLength + 4;

		sectionDataLength = data[dataOffset] + (data[dataOffset + 1] << 8) + (data[dataOffset + 2] << 16) + (data[dataOffset + 3] << 24);
		StatusSection->GetDataFromDataBytes(data, sectionDataLength, dataOffset + 4, frameInfo, gpsFix, userCommand, systemError);
		
		delete data;
	}
}

}
