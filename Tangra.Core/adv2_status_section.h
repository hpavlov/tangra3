/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#ifndef ADV_STATUS_SECTION2_H
#define ADV_STATUS_SECTION2_H

#include <map>
#include <list>
#include <vector>
#include <string>
#include <stdio.h>
#include "utils.h"

using namespace std;
using std::string;

namespace AdvLib2
{
	class Adv2StatusSection {

		private:
			vector<string> m_TagDefinitionNames;
			map<string, Adv2TagType> m_TagDefinition;
		
			map<unsigned int, string> m_FrameStatusTags;
			map<unsigned int, unsigned char> m_FrameStatusTagsUInt8;
			map<unsigned int, unsigned short> m_FrameStatusTagsUInt16;
			map<unsigned int, unsigned int> m_FrameStatusTagsUInt32;
			map<unsigned int, __int64> m_FrameStatusTagsUInt64;
			map<unsigned int, float> m_FrameStatusTagsReal;

			__int64 m_UtcStartTimeNanosecondsSinceAdvZeroEpoch;
			unsigned int m_UtcExposureNanoseconds;

			bool m_FrameStatusLoaded;
			ADVRESULT VaidateStatusTagId(unsigned int tagIndex, Adv2TagType expectedTagType, bool write);

			bool m_SectionDefinitionMode;

		public:
			int MaxFrameBufferSize;
			__int64 UtcTimestampAccuracyInNanoseconds;

		public:
			Adv2StatusSection(__int64 utcTimestampAccuracyInNanoseconds);
			Adv2StatusSection(FILE* pFile, AdvFileInfo* fileInfo);
			~Adv2StatusSection();

			ADVRESULT DefineTag(const char* tagName, enum Adv2TagType tagType, unsigned int* addedTagId);

			ADVRESULT BeginFrame(__int64 utcStartTimeNanosecondsSinceAdvZeroEpoch, unsigned int utcExposureNanoseconds);
			void WriteHeader(FILE* pfile);
			ADVRESULT AddFrameStatusTagUTF8String(unsigned int tagIndex, const char* tagValue);
			ADVRESULT AddFrameStatusTagUInt8(unsigned int tagIndex, unsigned char tagValue);
			ADVRESULT AddFrameStatusTagUInt16(unsigned int tagIndex, unsigned short tagValue);
			ADVRESULT AddFrameStatusTagReal(unsigned int tagIndex, float tagValue);
			ADVRESULT AddFrameStatusTagUInt32(unsigned int tagIndex, unsigned int tagValue);
			ADVRESULT AddFrameStatusTagUInt64(unsigned int tagIndex, __int64 tagValue);

			unsigned char* GetDataBytes(unsigned int *bytesCount);
			void GetDataFromDataBytes(unsigned char* data, int sectionDataLength, int startOffset, AdvFrameInfo* frameInfo, int* systemErrorLen);

			ADVRESULT GetStatusTagNameSize(unsigned int tagId, int* tagNameSize);
			ADVRESULT GetStatusTagInfo(unsigned int tagId, char* tagName, Adv2TagType* tagType);
			ADVRESULT GetStatusTagSizeUTF8String(unsigned int tagIndex, int* tagValueSize);
			ADVRESULT GetStatusTagUTF8String(unsigned int tagIndex, char* tagValue);
			ADVRESULT GetStatusTagUInt8(unsigned int tagIndex, unsigned char* tagValue);
			ADVRESULT GetStatusTag16(unsigned int tagIndex, unsigned short* tagValue);
			ADVRESULT GetStatusTagReal(unsigned int tagIndex, float* tagValue);
			ADVRESULT GetStatusTag32(unsigned int tagIndex, unsigned int* tagValue);
			ADVRESULT GetStatusTag64(unsigned int tagIndex, __int64* tagValue);
	};
}

#endif //ADV_STATUS_SECTION2_H