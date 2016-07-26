/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#ifndef ADVFILE2_H
#define ADVFILE2_H

#include "adv2_image_section.h"
#include "adv2_status_section.h"
#include "adv2_image_layout.h"
#include "adv2_frames_index.h"
#include "utils.h"

#include <map>
#include <string>
#include <stdio.h>
#include "quicklz.h"

using namespace std;
using std::string;

namespace AdvLib2
{
	class Adv2File {
		public:
			AdvLib2::Adv2ImageSection* ImageSection;
			AdvLib2::Adv2StatusSection* StatusSection;

		protected:
			AdvLib2::Adv2FramesIndex* m_Index;
			map<string, string> m_FileTags;
			
		private:
			AdvLib2::Adv2ImageLayout* m_CurrentImageLayout;
			unsigned char m_CurrentStreamId;

			__int64 m_NewFrameOffset;

			__int64 m_MainFrameCountPosition;
			__int64 m_CalibrationFrameCountPosition;

			unsigned int m_MainFrameNo;
			unsigned int m_CalibrationFrameNo;

			unsigned char *m_FrameBytes;
			unsigned int m_FrameBufferIndex; 
			__int64 m_CurrentFrameElapsedTicks;

			__int64 m_FirstFrameInStreamTicks[2];
			__int64 m_PrevFrameInStreamTicks[2];

			map<string, string> m_UserMetadataTags;

			map<string, string> m_MainStreamTags;
			map<string, string> m_CalibrationStreamTags;

			__int64 m_MainStreamClockFrequency;
			unsigned int m_MainStreamTickAccuracy;
			__int64 m_CalibrationStreamClockFrequency;
			unsigned int m_CalibrationStreamTickAccuracy;
			bool m_UsesExternalMainStreamClock;
			bool m_UsesExternalCalibrationStreamClock;

			int m_NumberOfMainFrames;
			int m_NumberOfCalibrationFrames;

			bool m_ImageAdded;
			bool m_FrameStarted;
			int m_LastSystemSpecificFileError;
			bool m_FileDefinitionMode;

			void InitFileState();
			void AddFrameImageInternal(unsigned char layoutId, unsigned short* pixels, unsigned char pixelsBpp, enum GetByteOperation operation);

		public:
			int TotalNumberOfMainFrames;
			int TotalNumberOfCalibrationFrames;

			Adv2File();
			~Adv2File();
			
			ADVRESULT BeginFile(const char* fileName);
			ADVRESULT SetTicksTimingPrecision(int mainStreamAccuracy, int calibrationStreamAccuracy);
			ADVRESULT DefineExternalClockForMainStream(__int64 clockFrequency, int ticksTimingAccuracy);
			ADVRESULT DefineExternalClockForCalibrationStream(__int64 clockFrequency, int ticksTimingAccuracy);
			void EndFile();
			
			int LoadFile(const char* fileName, AdvFileInfo* fileInfo);
			bool CloseFile();
			
			ADVRESULT AddImageSection(AdvLib2::Adv2ImageSection* section);
			ADVRESULT AddStatusSection(AdvLib2::Adv2StatusSection* section);

			ADVRESULT AddFileTag(const char* tagName, const char* tagValue);
			ADVRESULT AddUserTag(const char* tagName, const char* tagValue);

			ADVRESULT AddMainStreamTag(const char* tagName, const char* tagValue);
			ADVRESULT AddCalibrationStreamTag(const char* tagName, const char* tagValue);
			
			ADVRESULT BeginFrame(unsigned char streamId, __int64 startFrameTicks, __int64 endFrameTicks,__int64 elapsedTicksSinceFirstFrame, __int64 utcStartTimeNanosecondsSinceAdvZeroEpoch, unsigned int utcExposureNanoseconds);
			ADVRESULT BeginFrame(unsigned char streamId, __int64 utcStartTimeNanosecondsSinceAdvZeroEpoch, unsigned int utcExposureNanoseconds);
			ADVRESULT EndFrame();

			ADVRESULT AddFrameImage(unsigned char layoutId, unsigned short* pixels, unsigned char pixelsBpp);
			ADVRESULT AddFrameImage(unsigned char layoutId, unsigned char* pixels, unsigned char pixelsBpp);

			void GetFrameImageSectionHeader(int streamId, int frameId, unsigned char* layoutId, enum GetByteMode* mode);
			void GetFrameSectionData(int streamId, int frameId, unsigned int* pixels, AdvFrameInfo* frameInfo, int* systemErrorLen);
			ADVRESULT GetMainStreamTagSizes(int tagId, int* tagNameSize, int* tagValueSize);
			ADVRESULT GetMainStreamTag(int tagId, char* tagName, char* tagValue);
			ADVRESULT GetCalibrationStreamTagSizes(int tagId, int* tagNameSize, int* tagValueSize);
			ADVRESULT GetCalibrationStreamTag(int tagId, char* tagName, char* tagValue);
			ADVRESULT GetSystemMetadataTagSizes(int tagId, int* tagNameSize, int* tagValueSize);
			ADVRESULT GetSystemMetadataTag(int tagId, char* tagName, char* tagValue);
			ADVRESULT GetUserMetadataTagSizes(int tagId, int* tagNameSize, int* tagValueSize);
			ADVRESULT GetUserMetadataTag(int tagId, char* tagName, char* tagValue);

			int GetLastSystemSpecificFileError();
		};

}

#endif // ADVFILE2_H
