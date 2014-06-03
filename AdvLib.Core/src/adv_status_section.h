/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#ifndef ADVSTATUSSECTION_H
#define ADVSTATUSSECTION_H

#include <map>
#include <list>
#include <string>
#include <stdio.h>
#include "utils.h"

using namespace std;
using std::string;

namespace AdvLib
{
	
class AdvStatusSection {

	private:		
		list<string> m_TagDefinitionNames;
		list<AdvTagType> m_TagDefinitionTypes;
		
		map<unsigned int, string> m_FrameStatusTags;
		map<unsigned int, unsigned char> m_FrameStatusTagsUInt8;
		map<unsigned int, unsigned short> m_FrameStatusTagsUInt16;
		map<unsigned int, unsigned int> m_FrameStatusTagsUInt32;
		map<unsigned int, long long> m_FrameStatusTagsUInt64;
		map<unsigned int, float> m_FrameStatusTagsReal;
		map<unsigned int, list<string> > m_FrameStatusTagsMessages;
		
	public:
		int MaxFrameBufferSize;
		
	public:
		AdvStatusSection();
		~AdvStatusSection();

		unsigned int DefineTag(const char* tagName, enum AdvTagType tagType);
		void WriteHeader(FILE* pfile);
		void AddFrameStatusTag(unsigned int tagIndex, const char* tagValue);
		void AddFrameStatusTagMessage(unsigned int tagIndex, const char* tagValue);
		void AddFrameStatusTagUInt8(unsigned int tagIndex, unsigned char tagValue);
		void AddFrameStatusTagUInt16(unsigned int tagIndex, unsigned short tagValue);
		void AddFrameStatusTagReal(unsigned int tagIndex, float tagValue);
		void AddFrameStatusTagUInt32(unsigned int tagIndex, unsigned int tagValue);
		void AddFrameStatusTagUInt64(unsigned int tagIndex, long long tagValue);
		unsigned char* GetDataBytes(unsigned int *bytesCount);
		void BeginFrame();
		
};


}

#endif // ADVSTATUSSECTION_H
