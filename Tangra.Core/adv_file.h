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
		long Width;
		long Height;
		float FrameRate;
		long CountFrames;
		long Bpp;
	};
	

	class AdvFile {
		public:
			AdvLib::AdvImageSection* ImageSection;
			AdvLib::AdvStatusSection* StatusSection;
			
			// OpenFile Properties
			long TotalNumberOfFrames;

		protected:
			AdvLib::AdvFramesIndex* m_Index;
			map<const char*, const char*> m_FileTags;
			
			
		private:
			AdvLib::AdvImageLayout* m_CurrentImageLayout;
			fpos_t m_NewFrameOffset;
			unsigned int m_FrameNo;
			long m_FirstFrameTime;

			unsigned char *m_FrameBytes;
			unsigned int m_FrameBufferIndex; 
			unsigned int m_ElapedTime;
						
			void InitFileState();
		public:
			AdvFile();
			~AdvFile();
			
			void OpenFile(const char* fileName, AdvFileInfo* fileInfo);
			
			void GetFrameImageSectionHeader(int frameId, unsigned char* layoutId, enum GetByteMode* mode);
			void GetFrameSectionData(int frameId, unsigned long* prevFrame, unsigned long* pixels, AdvFrameInfo* frameInfo, char* gpsFix, char* userCommand, char* systemError);
		};

}

#endif // ADVFILE_H