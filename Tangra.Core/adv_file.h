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

#include "cross_platform.h"

using namespace std;
using std::string;

namespace AdvLib
{
	struct AdvFileInfo
	{
		int Width;
		int Height;
		float FrameRate;
		int CountFrames;
		int Bpp;
		unsigned int Aav16NormVal;
	};
	

	class AdvFile {
		public:
			AdvLib::AdvImageSection* ImageSection;
			AdvLib::AdvStatusSection* StatusSection;
			
			// OpenFile Properties
			int TotalNumberOfFrames;

		protected:
			AdvLib::AdvFramesIndex* m_Index;
			map<string, string> m_FileTags;
			
			
		private:
			AdvLib::AdvImageLayout* m_CurrentImageLayout;
			fpos_t m_NewFrameOffset;
			unsigned int m_FrameNo;
			int m_FirstFrameTime;

			unsigned char *m_FrameBytes;
			unsigned int m_FrameBufferIndex; 
			unsigned int m_ElapedTime;
			bool m_IsAAV;
						
			void InitFileState();
		public:
			AdvFile();
			~AdvFile();
			
			void OpenFile(const char* fileName, AdvFileInfo* fileInfo);
			
			void GetFrameImageSectionHeader(int frameId, unsigned char* layoutId, enum GetByteMode* mode);
			void GetFrameSectionData(int frameId, unsigned int* prevFrame, unsigned int* pixels, AdvFrameInfo* frameInfo, char* gpsFix, char* userCommand, char* systemError);
			void GetFrameStatusSectionData(int frameId, AdvFrameInfo* frameInfo, char* gpsFix, char* userCommand, char* systemError);		
			
			void GetFileTag(const char* fileTagName, char* fileTagValue);
			bool IsAAV();
		};

}

#endif // ADVFILE_H
