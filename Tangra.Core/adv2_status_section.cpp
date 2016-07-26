/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#include "stdafx.h"
#include "adv2_status_section.h"
#include <string>
#include <algorithm>
#include <stdlib.h>

using namespace std;
using std::string;

namespace AdvLib2
{

Adv2StatusSection::Adv2StatusSection(__int64 utcTimestampAccuracyInNanoseconds)
{
	MaxFrameBufferSize = 0;
	UtcTimestampAccuracyInNanoseconds = utcTimestampAccuracyInNanoseconds;

	m_TagDefinitionNames.empty();
	m_TagDefinition.empty();

	m_FrameStatusLoaded = false;
	m_SectionDefinitionMode = true;
}

Adv2StatusSection::~Adv2StatusSection()
{

}

ADVRESULT Adv2StatusSection::DefineTag(const char* tagName, enum Adv2TagType tagType, unsigned int* addedTagId)
{
	if (!m_SectionDefinitionMode)
		return E_ADV_CHANGE_NOT_ALLOWED_RIGHT_NOW;

	vector<string>::iterator curr = std::find(m_TagDefinitionNames.begin(), m_TagDefinitionNames.end(), tagName);
	if (curr != m_TagDefinitionNames.end())
	{
		*addedTagId = std::distance(m_TagDefinitionNames.begin(), curr);
		m_TagDefinition[tagName] = tagType;
		return S_ADV_TAG_REPLACED;
	}

	m_TagDefinitionNames.push_back(string(tagName));
	m_TagDefinition.insert(make_pair(string(tagName), (Adv2TagType)tagType));
	
	switch(tagType)
	{
		case Adv2TagType::Int8:
			MaxFrameBufferSize+=1;
			break;
			
		case Adv2TagType::Int16:
			MaxFrameBufferSize+=2;
			break;

		case Adv2TagType::Int32:
			MaxFrameBufferSize+=4;
			break;
			
		case Adv2TagType::Long64:
			MaxFrameBufferSize+=8;
			break;			
			
		case Adv2TagType::Real4:
			MaxFrameBufferSize+=4;
			break;	
			
		case Adv2TagType::UTF8String:
			MaxFrameBufferSize+=0x10001;
			break;
	}
	
	*addedTagId = m_TagDefinitionNames.size() - 1;
	return S_OK;
}


ADVRESULT Adv2StatusSection::BeginFrame(__int64 utcStartTimeNanosecondsSinceAdvZeroEpoch, unsigned int utcExposureNanoseconds)
{
	m_FrameStatusTags.clear();
	m_FrameStatusTagsUInt8.clear();
	m_FrameStatusTagsUInt16.clear();
	m_FrameStatusTagsUInt64.clear();
	m_FrameStatusTagsUInt32.clear();
	m_FrameStatusTagsReal.clear();	

	m_UtcStartTimeNanosecondsSinceAdvZeroEpoch = utcStartTimeNanosecondsSinceAdvZeroEpoch;
	m_UtcExposureNanoseconds = utcExposureNanoseconds;

	m_SectionDefinitionMode = false;

	return S_OK;
}

ADVRESULT Adv2StatusSection::VaidateStatusTagId(unsigned int tagIndex, Adv2TagType expectedTagType, bool write)
{
	if (!write && !m_FrameStatusLoaded)
		return E_ADV_FRAME_STATUS_NOT_LOADED;

	if (tagIndex < 0 || tagIndex >= m_TagDefinitionNames.size())
		return E_ADV_INVALID_STATUS_TAG_ID;

	map<string, Adv2TagType>::iterator curr = m_TagDefinition.find(m_TagDefinitionNames[tagIndex]);
	if (curr == m_TagDefinition.end())
		return E_ADV_INVALID_STATUS_TAG_ID;
	
	if (curr->second != expectedTagType)
		return E_ADV_INVALID_STATUS_TAG_TYPE;

	return S_OK;
}

ADVRESULT Adv2StatusSection::AddFrameStatusTagUTF8String(unsigned int tagIndex, const char* tagValue)
{
	if (m_FrameStatusTags.find(tagIndex) != m_FrameStatusTags.end())
		return E_ADV_STATUS_ENTRY_ALREADY_ADDED;

	ADVRESULT rv = VaidateStatusTagId(tagIndex, Adv2TagType::UTF8String, true);
	if (rv == S_OK)
	{
		m_FrameStatusTags.insert(make_pair(tagIndex, string(tagValue == nullptr ? "" : tagValue)));
	}
	
	return rv;
}

ADVRESULT Adv2StatusSection::AddFrameStatusTagUInt8(unsigned int tagIndex, unsigned char tagValue)
{
	if (m_FrameStatusTagsUInt8.find(tagIndex) != m_FrameStatusTagsUInt8.end())
		return E_ADV_STATUS_ENTRY_ALREADY_ADDED;

	ADVRESULT rv = VaidateStatusTagId(tagIndex, Adv2TagType::Int8, true);
	if (rv == S_OK)
	{
		m_FrameStatusTagsUInt8.insert(make_pair(tagIndex, tagValue));
	}
	return rv;
}

ADVRESULT Adv2StatusSection::AddFrameStatusTagUInt16(unsigned int tagIndex, unsigned short tagValue)
{
	if (m_FrameStatusTagsUInt16.find(tagIndex) != m_FrameStatusTagsUInt16.end())
		return E_ADV_STATUS_ENTRY_ALREADY_ADDED;

	ADVRESULT rv = VaidateStatusTagId(tagIndex, Adv2TagType::Int16, true);
	if (rv == S_OK)
	{
		m_FrameStatusTagsUInt16.insert(make_pair(tagIndex, tagValue));
	}
	return rv;
}

ADVRESULT Adv2StatusSection::AddFrameStatusTagReal(unsigned int tagIndex, float tagValue)
{
	if (m_FrameStatusTagsReal.find(tagIndex) != m_FrameStatusTagsReal.end())
		return E_ADV_STATUS_ENTRY_ALREADY_ADDED;

	ADVRESULT rv = VaidateStatusTagId(tagIndex, Adv2TagType::Real4, true);
	if (rv == S_OK)
	{
		m_FrameStatusTagsReal.insert(make_pair(tagIndex, tagValue));
	}
	return rv;
}

ADVRESULT Adv2StatusSection::AddFrameStatusTagUInt32(unsigned int tagIndex, unsigned int tagValue)
{
	if (m_FrameStatusTagsUInt32.find(tagIndex) != m_FrameStatusTagsUInt32.end())
		return E_ADV_STATUS_ENTRY_ALREADY_ADDED;

	ADVRESULT rv = VaidateStatusTagId(tagIndex, Adv2TagType::Int32, true);
	if (rv == S_OK)
	{
		m_FrameStatusTagsUInt32.insert(make_pair(tagIndex, tagValue));
	}
	return rv;
}

ADVRESULT Adv2StatusSection::AddFrameStatusTagUInt64(unsigned int tagIndex, __int64 tagValue)
{
	if (m_FrameStatusTagsUInt64.find(tagIndex) != m_FrameStatusTagsUInt64.end())
		return E_ADV_STATUS_ENTRY_ALREADY_ADDED;

	ADVRESULT rv = VaidateStatusTagId(tagIndex, Adv2TagType::Long64, true);
	if (rv == S_OK)
	{
		m_FrameStatusTagsUInt64.insert(make_pair(tagIndex, tagValue));
	}
	return rv;
}

ADVRESULT Adv2StatusSection::GetStatusTagNameSize(unsigned int tagId, int* tagNameSize)
{
	if (tagId < 0 || tagId > m_TagDefinitionNames.size())
		return E_ADV_INVALID_STATUS_TAG_ID;

	string tag = m_TagDefinitionNames[tagId];
	*tagNameSize = (int)tag.size();
	return S_OK;
}

ADVRESULT Adv2StatusSection::GetStatusTagInfo(unsigned int tagId, char* tagName, Adv2TagType* tagType)
{
	if (tagId < 0 || tagId > m_TagDefinitionNames.size())
		return E_ADV_INVALID_STATUS_TAG_ID;

	string tag = m_TagDefinitionNames[tagId];
	strcpy_s(tagName, tag.size() + 1, tag.c_str());

	map<string, Adv2TagType>::iterator curr = m_TagDefinition.find(tag);
	*tagType = curr->second;

	return S_OK;
}

ADVRESULT Adv2StatusSection::GetStatusTagSizeUTF8String(unsigned int tagIndex, int* tagValueSize)
{
	ADVRESULT rv = VaidateStatusTagId(tagIndex, Adv2TagType::UTF8String, false);
	if (rv != S_OK) return rv;

	map<unsigned int, std::string>::iterator curr = m_FrameStatusTags.find(tagIndex);
	if (curr == m_FrameStatusTags.end())
		return E_ADV_STATUS_TAG_NOT_FOUND_IN_FRAME;

	*tagValueSize = (int)curr->second.size();
	return S_OK;
}

ADVRESULT Adv2StatusSection::GetStatusTagUTF8String(unsigned int tagIndex, char* tagValue)
{
	ADVRESULT rv = VaidateStatusTagId(tagIndex, Adv2TagType::UTF8String, false);
	if (rv != S_OK) return rv;

	map<unsigned int, std::string>::iterator curr = m_FrameStatusTags.find(tagIndex);
	if (curr == m_FrameStatusTags.end())
		return E_ADV_STATUS_TAG_NOT_FOUND_IN_FRAME;

	strcpy_s(tagValue, curr->second.size() + 1, curr->second.c_str());

	return S_OK;
}

ADVRESULT Adv2StatusSection::GetStatusTagUInt8(unsigned int tagIndex, unsigned char* tagValue)
{
	ADVRESULT rv = VaidateStatusTagId(tagIndex, Adv2TagType::Int8, false);
	if (rv != S_OK) return rv;

	map<unsigned int, unsigned char>::iterator curr = m_FrameStatusTagsUInt8.find(tagIndex);
	if (curr == m_FrameStatusTagsUInt8.end())
		return E_ADV_STATUS_TAG_NOT_FOUND_IN_FRAME;

	*tagValue = curr->second;

	return S_OK;
}

ADVRESULT Adv2StatusSection::GetStatusTag16(unsigned int tagIndex, unsigned short* tagValue)
{
	ADVRESULT rv = VaidateStatusTagId(tagIndex, Adv2TagType::Int16, false);
	if (rv != S_OK) return rv;

	map<unsigned int, unsigned short>::iterator curr = m_FrameStatusTagsUInt16.find(tagIndex);
	if (curr == m_FrameStatusTagsUInt16.end())
		return E_ADV_STATUS_TAG_NOT_FOUND_IN_FRAME;

	*tagValue = curr->second;

	return S_OK;
}

ADVRESULT Adv2StatusSection::GetStatusTagReal(unsigned int tagIndex, float* tagValue)
{
	ADVRESULT rv = VaidateStatusTagId(tagIndex, Adv2TagType::Real4, false);
	if (rv != S_OK) return rv;

	map<unsigned int, float>::iterator curr = m_FrameStatusTagsReal.find(tagIndex);
	if (curr == m_FrameStatusTagsReal.end())
		return E_ADV_STATUS_TAG_NOT_FOUND_IN_FRAME;

	*tagValue = curr->second;

	return S_OK;
}

ADVRESULT Adv2StatusSection::GetStatusTag32(unsigned int tagIndex, unsigned int* tagValue)
{
	ADVRESULT rv = VaidateStatusTagId(tagIndex, Adv2TagType::Int32, false);
	if (rv != S_OK) return rv;

	map<unsigned int, unsigned int>::iterator curr = m_FrameStatusTagsUInt32.find(tagIndex);
	if (curr == m_FrameStatusTagsUInt32.end())
		return E_ADV_STATUS_TAG_NOT_FOUND_IN_FRAME;

	*tagValue = curr->second;

	return S_OK;
}

ADVRESULT Adv2StatusSection::GetStatusTag64(unsigned int tagIndex, __int64* tagValue)
{
	ADVRESULT rv = VaidateStatusTagId(tagIndex, Adv2TagType::Long64, false);
	if (rv != S_OK) return rv;

	map<unsigned int, __int64>::iterator curr = m_FrameStatusTagsUInt64.find(tagIndex);
	if (curr == m_FrameStatusTagsUInt64.end())
		return E_ADV_STATUS_TAG_NOT_FOUND_IN_FRAME;

	*tagValue = curr->second;

	return S_OK;
}

unsigned int FloatToIntBits(const float x)
{
    union {
       float f;  		  // assuming 32-bit IEEE 754 single-precision
       unsigned int i;    // assuming 32-bit 2's complement int
    } u;
        
    u.f = x;
    return u.i;
}

float IntToFloat(unsigned int x)
{
	union 
	{
		float f;
		unsigned int i;
	} u;

	u.i = x;
	return u.f;
}

Adv2StatusSection::Adv2StatusSection(FILE* pFile, AdvFileInfo* fileInfo)
{
	MaxFrameBufferSize = 0;
	
	m_TagDefinitionNames.empty();
	m_TagDefinition.empty();
	
	m_SectionDefinitionMode = true;

	unsigned char version;
	advfread(&version, 1, 1, pFile); /* Version */

	advfread(&UtcTimestampAccuracyInNanoseconds, 8, 1, pFile);

	unsigned char tagsCount;
	advfread(&tagsCount, 1, 1, pFile);

	fileInfo->ErrorStatusTagId = -1;

	for (int i = 0; i < tagsCount; i++)
	{
		char* tagName = ReadUTF8String(pFile);
		unsigned char tagType;
		advfread(&tagType, 1, 1, pFile);

		// TODO: What happens if the tagIds do not correspond to the index of the tags in the list of at all possible?
		unsigned int addedTagId;
		DefineTag(tagName, (Adv2TagType)tagType, &addedTagId);

		if (strcmp("Error", tagName) == 0) fileInfo->ErrorStatusTagId = i;
	}

	fileInfo->UtcTimestampAccuracyInNanoseconds = UtcTimestampAccuracyInNanoseconds;
	fileInfo->StatusTagsCount = tagsCount;

	m_FrameStatusLoaded = false;
	m_SectionDefinitionMode = false;
}

void Adv2StatusSection::WriteHeader(FILE* pFile)
{
	unsigned char buffChar;
	
	buffChar = 2;
	advfwrite(&buffChar, 1, 1, pFile); /* Version */
	
	advfwrite(&UtcTimestampAccuracyInNanoseconds, 8, 1, pFile);

	buffChar = (unsigned char)m_TagDefinitionNames.size();
	advfwrite(&buffChar, 1, 1, pFile);
	int tagCount = buffChar;
	
	for(int i = 0; i<tagCount; i++)
	{
		char* tagName = const_cast<char*>(m_TagDefinitionNames[i].c_str());
		WriteUTF8String(pFile, tagName);
		
		map<string, Adv2TagType>::iterator currDef = m_TagDefinition.find(tagName);

		buffChar = (unsigned char)(int)((Adv2TagType)(currDef->second));
		advfwrite(&buffChar, 1, 1, pFile);
	}

	m_SectionDefinitionMode = false;
}

unsigned char* Adv2StatusSection::GetDataBytes(unsigned int *bytesCount)
{
	int size = 0;
	int arrayLength = 0;
	int numTagEntries = 0;
	
	map<unsigned int, string>::iterator curr = m_FrameStatusTags.begin();
	while (curr != m_FrameStatusTags.end()) 
	{
		char* tagValue = const_cast<char*>(curr->second.c_str());
		
		arrayLength += (int)strlen(tagValue) + 1 /* TagId*/  + 2 /* length */ ;
		curr++;
		numTagEntries++;
	}
	
	arrayLength += (int)m_FrameStatusTagsUInt8.size() * (1 /*sizeof(unsigned char)*/ + 1 /* TagId*/ );
	numTagEntries += (int)m_FrameStatusTagsUInt8.size();
	arrayLength += (int)m_FrameStatusTagsUInt16.size() * (2 /*sizeof(unsigned short)*/ + 1 /* TagId*/ );
	numTagEntries += (int)m_FrameStatusTagsUInt16.size();
	arrayLength += (int)m_FrameStatusTagsUInt64.size() * (8 /*sizeof(__int64)*/ + 1 /* TagId*/ );
	numTagEntries += (int)m_FrameStatusTagsUInt64.size();
	arrayLength += (int)m_FrameStatusTagsUInt32.size() * (4 /*sizeof(unsinged int)*/ + 1 /* TagId*/ );
	numTagEntries += (int)m_FrameStatusTagsUInt32.size();
	arrayLength += (int)m_FrameStatusTagsReal.size() * (4 /*sizeof(float)*/ + 1 /* TagId*/ );
	numTagEntries += (int)m_FrameStatusTagsReal.size();
	
	size = arrayLength + 13;
	
	unsigned char *statusData = (unsigned char*)malloc(size);

	statusData[0] = (unsigned char)(m_UtcStartTimeNanosecondsSinceAdvZeroEpoch & 0xFF);
	statusData[1] = (unsigned char)((m_UtcStartTimeNanosecondsSinceAdvZeroEpoch >> 8) & 0xFF);
	statusData[2] = (unsigned char)((m_UtcStartTimeNanosecondsSinceAdvZeroEpoch >> 16) & 0xFF);
	statusData[3] = (unsigned char)((m_UtcStartTimeNanosecondsSinceAdvZeroEpoch >> 24) & 0xFF);
	statusData[4] = (unsigned char)((m_UtcStartTimeNanosecondsSinceAdvZeroEpoch >> 32) & 0xFF);
	statusData[5] = (unsigned char)((m_UtcStartTimeNanosecondsSinceAdvZeroEpoch >> 40) & 0xFF);
	statusData[6] = (unsigned char)((m_UtcStartTimeNanosecondsSinceAdvZeroEpoch >> 48) & 0xFF);
	statusData[7] = (unsigned char)((m_UtcStartTimeNanosecondsSinceAdvZeroEpoch >> 56) & 0xFF);
	statusData[8] = (unsigned char)(m_UtcExposureNanoseconds & 0xFF);
	statusData[9] = (unsigned char)((m_UtcExposureNanoseconds >> 8) & 0xFF);
	statusData[10] = (unsigned char)((m_UtcExposureNanoseconds >> 16) & 0xFF);
	statusData[11] = (unsigned char)((m_UtcExposureNanoseconds >> 24) & 0xFF);
	statusData[12] = (numTagEntries & 0xFF);		
		
	if (arrayLength > 0)
	{
		int dataPos = 13;
		
		map<unsigned int, __int64>::iterator currUInt64 = m_FrameStatusTagsUInt64.begin();
		while (currUInt64 != m_FrameStatusTagsUInt64.end()) 
		{
			unsigned char tagId = (unsigned char)(currUInt64->first & 0xFF);
			statusData[dataPos] = tagId;

			__int64 tagValue = (__int64)(currUInt64->second);
			statusData[dataPos + 1] = (unsigned char)(tagValue & 0xFF);
			statusData[dataPos + 2] = (unsigned char)((tagValue >> 8) & 0xFF);
			statusData[dataPos + 3] = (unsigned char)((tagValue >> 16) & 0xFF);
			statusData[dataPos + 4] = (unsigned char)((tagValue >> 24) & 0xFF);
			statusData[dataPos + 5] = (unsigned char)((tagValue >> 32) & 0xFF);
			statusData[dataPos + 6] = (unsigned char)((tagValue >> 40) & 0xFF);
			statusData[dataPos + 7] = (unsigned char)((tagValue >> 48) & 0xFF);
			statusData[dataPos + 8] = (unsigned char)((tagValue >> 56) & 0xFF);
	
			dataPos+=9;
			
			currUInt64++;
		}

		map<unsigned int, unsigned int>::iterator currUInt32 = m_FrameStatusTagsUInt32.begin();
		while (currUInt32 != m_FrameStatusTagsUInt32.end()) 
		{
			unsigned char tagId = (unsigned char)(currUInt32->first & 0xFF);
			statusData[dataPos] = tagId;

			unsigned int tagValue = (__int64)(currUInt32->second);
			statusData[dataPos + 1] = (unsigned char)(tagValue & 0xFF);
			statusData[dataPos + 2] = (unsigned char)((tagValue >> 8) & 0xFF);
			statusData[dataPos + 3] = (unsigned char)((tagValue >> 16) & 0xFF);
			statusData[dataPos + 4] = (unsigned char)((tagValue >> 24) & 0xFF);
	
			dataPos+=5;
			
			currUInt32++;
		}
		
		map<unsigned int, unsigned short>::iterator currUInt16 = m_FrameStatusTagsUInt16.begin();
		while (currUInt16 != m_FrameStatusTagsUInt16.end()) 
		{
			unsigned char tagId = (unsigned char)(currUInt16->first & 0xFF);
			statusData[dataPos] = tagId;

			unsigned short tagValue = (unsigned short)(currUInt16->second);
			statusData[dataPos + 1] = (unsigned char)(tagValue & 0xFF);
			statusData[dataPos + 2] = (unsigned char)((tagValue >> 8) & 0xFF);

			dataPos+=3;
			
			currUInt16++;
		}
		
		map<unsigned int, unsigned char>::iterator currUInt8 = m_FrameStatusTagsUInt8.begin();
		while (currUInt8 != m_FrameStatusTagsUInt8.end()) 
		{
			unsigned char tagId = (unsigned char)(currUInt8->first & 0xFF);
			statusData[dataPos] = tagId;

			unsigned char tagValue = (unsigned char)(currUInt8->second);
			statusData[dataPos + 1] = tagValue;

			dataPos+=2;
			
			currUInt8++;
		}
		
		map<unsigned int, float>::iterator currReal = m_FrameStatusTagsReal.begin();
		while (currReal != m_FrameStatusTagsReal.end()) 
		{
			unsigned char tagId = (unsigned char)(currReal->first & 0xFF);
			statusData[dataPos] = tagId;

			float tagValue = (float)(currReal->second);
			unsigned int intValue = FloatToIntBits(tagValue);
			
			statusData[dataPos + 1] = intValue & 0xFF;
			statusData[dataPos + 2] = (intValue >> 8) & 0xFF;
			statusData[dataPos + 3] = (intValue >> 16) & 0xFF;
			statusData[dataPos + 4] = (intValue >> 24) & 0xFF;

			dataPos+=5;
			
			currReal++;
		}
		
		curr = m_FrameStatusTags.begin();
		while (curr != m_FrameStatusTags.end()) 
		{
			unsigned char tagId = (unsigned char)(curr->first & 0xFF);
			statusData[dataPos] = tagId;
			
			char* tagValue = const_cast<char*>(curr->second.c_str());
			
			int strLen = (int)strlen(tagValue);
			statusData[dataPos + 1] = strLen & 0xFF;
			statusData[dataPos + 2] = (strLen >> 8) & 0xFF;
			memcpy(&statusData[dataPos + 3], tagValue, strLen);
			dataPos+= strLen + 3;
			
			curr++;
		}
	}
	
	*bytesCount = size;
	return statusData;
}

void Adv2StatusSection::GetDataFromDataBytes(unsigned char* data, int sectionDataLength, int startOffset, AdvFrameInfo* frameInfo, int* systemErrorLen)
{
	unsigned char* statusData = data + startOffset;

	m_UtcStartTimeNanosecondsSinceAdvZeroEpoch = 
		 (statusData[0] + (statusData[1] << 8) + (statusData[2] << 16) + (statusData[3] << 24) +
		 ((__int64)(statusData[4] + (statusData[5] << 8) + (statusData[6] << 16) + (statusData[7] << 24)) << 32));
	frameInfo->Exposure = m_UtcExposureNanoseconds = statusData[8] + (statusData[9] << 8) + (statusData[10] << 16) + (statusData[11] << 24);
	frameInfo->UtcTimestampLo = statusData[0] + (statusData[1] << 8) + (statusData[2] << 16) + (statusData[3] << 24);
	frameInfo->UtcTimestampHi = statusData[4] + (statusData[5] << 8) + (statusData[6] << 16) + (statusData[7] << 24);
	statusData+=12;

	m_FrameStatusTagsUInt8.clear();
	m_FrameStatusTagsUInt16.clear();
	m_FrameStatusTagsUInt64.clear();
	m_FrameStatusTagsUInt32.clear();
	m_FrameStatusTagsReal.clear();	

	unsigned char tagsCount = *statusData;
	statusData++;

	for(int i = 0; i < tagsCount; i++)
	{
		unsigned char tagId = *statusData;
		
		string currById = m_TagDefinitionNames[tagId];
		const char* tagName = currById.c_str();
		map<string, Adv2TagType>::iterator currDef = m_TagDefinition.find(tagName);
		Adv2TagType type = (Adv2TagType)(currDef->second);
		
		switch(type)
		{
			case Adv2TagType::Int8:
				{
					char val = *(statusData + 1);
					statusData+=2;
					m_FrameStatusTagsUInt8.insert(make_pair(tagId, val));
					if (strcmp("TrackedSatellites", tagName) == 0)
						frameInfo->GPSTrackedSattelites = val;
					else if (strcmp("AlmanacStatus", tagName) == 0)
						frameInfo->GPSAlmanacStatus = val;
					else if (strcmp("AlmanacOffset", tagName) == 0)
						frameInfo->GPSAlmanacOffset = val;
					else if (strcmp("SatelliteFixStatus", tagName) == 0)
						frameInfo->GPSFixStatus = val;

					break;
				}
			case Adv2TagType::Int16:
				{
					unsigned char b1 = *(statusData + 1);
					unsigned char b2 = *(statusData + 2);
					unsigned short val = b1 + (b2 << 8);
					m_FrameStatusTagsUInt16.insert(make_pair(tagId, val));
					statusData+=3;
					break;
				}
			case Adv2TagType::Int32:
				{
					unsigned char  b1 = *(statusData + 1);
					unsigned char  b2 = *(statusData + 2);
					unsigned char  b3 = *(statusData + 3);
					unsigned char  b4 = *(statusData + 4);

					unsigned int value = (unsigned int)(((int)b4 << 24) + ((int)b3 << 16) + ((int)b2 << 8) + (int)b1);
					m_FrameStatusTagsUInt32.insert(make_pair(tagId, value));

					statusData+=5;
					break;
				}
			case Adv2TagType::Long64:
				{
					unsigned char b1 = *(statusData + 1);
					unsigned char b2 = *(statusData + 2);
					unsigned char b3 = *(statusData + 3);
					unsigned char b4 = *(statusData + 4);
					unsigned char b5 = *(statusData + 5);
					unsigned char b6 = *(statusData + 6);
					unsigned char b7 = *(statusData + 7);
					unsigned char b8 = *(statusData + 8);

					unsigned int valLo = (unsigned int)(((int)b4 << 24) + ((int)b3 << 16) + ((int)b2 << 8) + (int)b1);
					unsigned int valHi = (unsigned int)(((int)b8 << 24) + ((int)b7 << 16) + ((int)b6 << 8) + (int)b5);
						
					__int64 value = (__int64)valLo + ((__int64)valHi << 32);
					m_FrameStatusTagsUInt64.insert(make_pair(tagId, value));

					statusData+=9;

					if (strcmp("VideoCameraFrameId", tagName) == 0)
					{
						frameInfo->VideoCameraFrameIdLo = valLo;
						frameInfo->VideoCameraFrameIdHi = valHi;			
					}
					else if (strcmp("HardwareTimerFrameId", tagName) == 0)
					{
						frameInfo->HardwareTimerFrameIdLo = valLo;
						frameInfo->HardwareTimerFrameIdHi = valHi;			
					}
					else if (strcmp("SystemTime", tagName) == 0)
					{
						frameInfo->SystemTimestampLo = valLo;
						frameInfo->SystemTimestampHi = valHi;			
					};

					break;
				}
			case Adv2TagType::Real4: 
				{
					unsigned char b1 = *(statusData + 1);
					unsigned char b2 = *(statusData + 2);
					unsigned char b3 = *(statusData + 3);
					unsigned char b4 = *(statusData + 4);

					unsigned int value = (unsigned int)(((int)b4 << 24) + ((int)b3 << 16) + ((int)b2 << 8) + (int)b1);
										
					float fVal = IntToFloat(value);

					m_FrameStatusTagsReal.insert(make_pair(tagId, fVal));

					statusData+=5;

					if (strcmp("Offset", tagName) == 0)
						frameInfo->Offset = fVal;
					else if (strcmp("Shutter", tagName) == 0)
						frameInfo->Shutter = fVal;
					else  if (strcmp("Gamma", tagName) == 0)
						frameInfo->Gamma = fVal;
					else  if (strcmp("Gain", tagName) == 0)
						frameInfo->Gain = fVal;

					break;
				}
			case Adv2TagType::UTF8String:
				{
					unsigned char b1 = *(statusData + 1);
					unsigned char b2 = *(statusData + 2);

					unsigned short byteCount = b1 + (b2 << 8);
					statusData += 3;

					char* destBuffer = (char*)malloc(byteCount + 1);
					for (int i = 0; i < byteCount; i++)
					{
						*(destBuffer + i) = *statusData;
						statusData++;
					}
					*(destBuffer + byteCount) = '\0';

					m_FrameStatusTags.insert(make_pair(tagId, string(destBuffer)));

					if (strcmp("Error", tagName) == 0)
						*systemErrorLen = byteCount;

					statusData += byteCount;
				}
				break;
		}
	}

	m_FrameStatusLoaded = true;
}



}