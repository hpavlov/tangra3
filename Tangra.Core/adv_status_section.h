/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#ifndef ADVSTATUSSECTION_H
#define ADVSTATUSSECTION_H

#include <map>
#include <string>
#include <vector>
#include <stdio.h>
#include "utils.h"

#include "cross_platform.h"

using namespace std;
using std::string;

namespace AdvLib
{
	
struct AdvFrameInfo
{	
	int StartTimeStampLo;
	int StartTimeStampHi;
	int Exposure10thMs;
	float Gamma;
	float Gain;
	float Shutter;
	float Offset;
	int SystemTimeLo;
	int SystemTimeHi;
	unsigned char GPSTrackedSattelites;
	unsigned char GPSAlmanacStatus;
	unsigned char GPSFixStatus;
	char GPSAlmanacOffset;
	int VideoCameraFrameIdLo;
	int VideoCameraFrameIdHi;
	int HardwareTimerFrameIdLo;
	int HardwareTimerFrameIdHi;
	int IntegratedFrames;
	int EndNtpTimeStampLo;
	int EndNtpTimeStampHi;
	int EndSecondaryTimeStampHi;
	int EndSecondaryTimeStampLo;
	int NtpTimeStampError;
	float Temperature;
};


class AdvStatusSection {

	private:		
		map<string, AdvTagType> m_TagDefinition;
		vector<string> m_TagList;	
		float IntToFloat(unsigned int x);

	public:
		int MaxFrameBufferSize;
		
	public:
		AdvStatusSection();
		AdvStatusSection(FILE* pFile);
		~AdvStatusSection();

		void GetDataFromDataBytes(unsigned char* data, int sectionDataLength, int startOffset, AdvFrameInfo* frameInfo, char* gpsFix, char* userCommand, char* systemError);
		
};


}

#endif // ADVSTATUSSECTION_H