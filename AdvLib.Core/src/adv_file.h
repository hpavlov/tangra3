/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#ifndef ADVFILE_H
#define ADVFILE_H

#include "adv_image_layout.h"
#include "adv_image_section.h"
#include "adv_status_section.h"
#include "adv_frames_index.h"

#include <map>
#include <string>
#include <stdio.h>
#include "quicklz.h"

using namespace std;
using std::string;

namespace AdvLib
{

	class AdvFile {
		public:
			AdvLib::AdvImageSection* ImageSection;
			AdvLib::AdvStatusSection* StatusSection;
			
		protected:
			AdvLib::AdvFramesIndex* m_Index;
			map<string, string> m_FileTags;			
			
		private:
			AdvLib::AdvImageLayout* m_CurrentImageLayout;
			__int64 m_NewFrameOffset;
			unsigned int m_FrameNo;

			unsigned char *m_FrameBytes;
			unsigned int m_FrameBufferIndex; 
			unsigned int m_ElapedTime;
						
			map<string, string> m_UserMetadataTags;

			void InitFileState();
		public:
			AdvFile();
			~AdvFile();
			
			bool BeginFile(const char* fileName);
			void EndFile();
			
			void AddImageSection(AdvLib::AdvImageSection* section);
			
			int AddFileTag(const char* tagName, const char* tagValue);
			int AddUserTag(const char* tagName, const char* tagValue);
			
			void BeginFrame(long long timeStamp, unsigned int elapsedTime, unsigned int exposure);
			void AddFrameStatusTag(unsigned int tagIndex, const char* tagValue);
			void AddFrameStatusTagMessage(unsigned int tagIndex, const char* tagValue);
			void AddFrameStatusTagUInt8(unsigned int tagIndex, unsigned char tagValue);
			void AddFrameStatusTagUInt16(unsigned int tagIndex, unsigned short tagValue);			
			void AddFrameStatusTagUInt32(unsigned int tagIndex, unsigned int tagValue);
			void AddFrameStatusTagUInt64(unsigned int tagIndex, long long tagValue);
			void AddFrameStatusTagReal(unsigned int tagIndex, float tagValue);
			void AddFrameImage(unsigned char layoutId, unsigned short* pixels, unsigned char pixelsBpp);
			void EndFrame();
		};

}

#endif // ADVFILE_H
