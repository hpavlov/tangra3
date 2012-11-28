#include "adv_status_section.h"
#include <string>
#include <stdlib.h>

using namespace std;
using std::string;

namespace AdvLib
{

AdvStatusSection::AdvStatusSection()
{
	MaxFrameBufferSize = 0;
}

AdvStatusSection::~AdvStatusSection()
{
}

float AdvStatusSection::IntToFloat(unsigned int x)
{
	union 
	{
		float f;
		unsigned int i;
	} u;

	u.i = x;
	return u.f;
}

void AdvStatusSection::GetDataFromDataBytes(unsigned char* data, int sectionDataLength, int startOffset, AdvFrameInfo* frameInfo, char* gpsFix, char* userCommand, char* systemError)
{
	unsigned char* statusData = data + startOffset;
	unsigned char tagsCount = *statusData;
	statusData++;

	for(int i = 0; i < tagsCount; i ++)
	{
		unsigned char tagId = *statusData;

		int idx = 0;
		const char* tagName = NULL;
		vector<string>::iterator curr = m_TagList.begin();
		while (curr != m_TagList.end())
		{
			if (idx == tagId)
			{
				tagName = curr->c_str();
				break;
			}

			idx++;
			curr++;
		}
		
		if (strcmp("SystemTime", tagName) == 0)
		{
			unsigned char  b1 = *(statusData + 1);
			unsigned char  b2 = *(statusData + 2);
			unsigned char  b3 = *(statusData + 3);
			unsigned char  b4 = *(statusData + 4);
			unsigned char  b5 = *(statusData + 5);
			unsigned char  b6 = *(statusData + 6);
			unsigned char  b7 = *(statusData + 7);
			unsigned char  b8 = *(statusData + 8);

			long valLo = (long)(((long)b4 << 24) + ((long)b3 << 16) + ((long)b2 << 8) + (long)b1);
			long valHi = (long)(((long)b8 << 24) + ((long)b7 << 16) + ((long)b6 << 8) + (long)b5);
			frameInfo->SystemTimeLo = valLo;
			frameInfo->SystemTimeHi = valHi;
			
			statusData+=9;
		}
		else if (strcmp("GPSTrackedSatellites", tagName) == 0)
		{
			char val = *(statusData + 1);
			frameInfo->GPSTrackedSattelites = val;
			statusData+=2;
		}
		else if (strcmp("GPSAlmanacStatus", tagName) == 0)
		{
			char val = *(statusData + 1);
			frameInfo->GPSAlmanacStatus = val;
			statusData+=2;
		}
		else if (strcmp("GPSFixStatus", tagName) == 0)
		{
			char val = *(statusData + 1);
			frameInfo->GPSFixStatus = val;
			statusData+=2;
		}
		else if (strcmp("Gain", tagName) == 0)
		{
			unsigned char  b1 = *(statusData + 1);
			unsigned char  b2 = *(statusData + 2);
			unsigned char  b3 = *(statusData + 3);
			unsigned char  b4 = *(statusData + 4);

			unsigned int value = (unsigned int)(((int)b4 << 24) + ((int)b3 << 16) + ((int)b2 << 8) + (int)b1);
			float fVal = IntToFloat(value);

			frameInfo->Gain = fVal;
			
			statusData+=5;
		}
		else if (strcmp("Gamma", tagName) == 0)
		{
			unsigned char  b1 = *(statusData + 1);
			unsigned char  b2 = *(statusData + 2);
			unsigned char  b3 = *(statusData + 3);
			unsigned char  b4 = *(statusData + 4);

			unsigned int value = (unsigned int)(((int)b4 << 24) + ((int)b3 << 16) + ((int)b2 << 8) + (int)b1);
			float fVal = IntToFloat(value);

			frameInfo->Gamma = fVal;
			
			statusData+=5;			
		}
		else if (strcmp("Shutter", tagName) == 0)
		{
			unsigned char  b1 = *(statusData + 1);
			unsigned char  b2 = *(statusData + 2);
			unsigned char  b3 = *(statusData + 3);
			unsigned char  b4 = *(statusData + 4);

			unsigned int value = (unsigned int)(((int)b4 << 24) + ((int)b3 << 16) + ((int)b2 << 8) + (int)b1);
			float fVal = IntToFloat(value);

			frameInfo->Shutter = fVal;

			statusData+=5;			
		}
		else if (strcmp("Offset", tagName) == 0)
		{
			unsigned char  b1 = *(statusData + 1);
			unsigned char  b2 = *(statusData + 2);
			unsigned char  b3 = *(statusData + 3);
			unsigned char  b4 = *(statusData + 4);

			unsigned int value = (unsigned int)(((int)b4 << 24) + ((int)b3 << 16) + ((int)b2 << 8) + (int)b1);
			float fVal = IntToFloat(value);

			frameInfo->Offset = fVal;
			
			statusData+=5;			
		}
		if (strcmp("VideoCameraFrameId", tagName) == 0 || strcmp("HardwareTimerFrameId", tagName) == 0)
		{
			unsigned char  b1 = *(statusData + 1);
			unsigned char  b2 = *(statusData + 2);
			unsigned char  b3 = *(statusData + 3);
			unsigned char  b4 = *(statusData + 4);
			unsigned char  b5 = *(statusData + 5);
			unsigned char  b6 = *(statusData + 6);
			unsigned char  b7 = *(statusData + 7);
			unsigned char  b8 = *(statusData + 8);

			long valLo = (long)(((long)b4 << 24) + ((long)b3 << 16) + ((long)b2 << 8) + (long)b1);
			long valHi = (long)(((long)b8 << 24) + ((long)b7 << 16) + ((long)b6 << 8) + (long)b5);

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
			
			statusData+=9;
		}
		else if (
			strcmp("GPSFix", tagName) == 0 ||
			strcmp("UserCommand", tagName) == 0 ||
			strcmp("SystemError", tagName) == 0)
		{
			char* destBuffer = NULL;
			if (strcmp("GPSFix", tagName) == 0)
				destBuffer = gpsFix;
			else if (strcmp("UserCommand", tagName) == 0)
				destBuffer = userCommand;
			else if (strcmp("SystemError", tagName) == 0)
				destBuffer = systemError;

			unsigned char count = *(statusData + 1);
			statusData += 2;
			for (int j = 0; j < count; j++)
			{
				unsigned char len = *statusData;
				
				if (destBuffer != NULL)
				{
					strncpy(destBuffer,  (char*)(statusData + 1), len);
					destBuffer+=len;
					*destBuffer = '\n';
					*(destBuffer + 1) = '\r';
					destBuffer+=2;

					*destBuffer = '\0';
				}

				statusData += 1 + len;
			}			
		}
	}
}

AdvStatusSection::AdvStatusSection(FILE* pFile)
{
	unsigned char version;
	fread(&version, 1, 1, pFile);

	if (version >= 1)
	{
		unsigned char numTags;
		fread(&numTags, 1, 1, pFile);
		
		m_TagDefinition.clear();
		m_TagList.clear();

		for(int i = 0; i < numTags; i++)
		{
			char* tagName = ReadString(pFile);

			unsigned char tagType;
			fread(&tagType, 1, 1, pFile);

			m_TagDefinition.insert(make_pair(string(tagName), (AdvTagType)tagType));
			m_TagList.push_back(string(tagName));

			delete tagName;
		}
	}
}

}

