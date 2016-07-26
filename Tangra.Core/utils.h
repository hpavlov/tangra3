/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#ifndef UTILS_H
#define UTILS_H

#include <stdio.h>
#include <string.h>

char* ReadString(FILE* pFile);
void WriteString(FILE* pFile, const char* str);

void WriteUTF8String(FILE* pFile, const char* str);
char* ReadUTF8String(FILE* pFile);

void DbgPrintBytes(unsigned char *bytes, int maxLen);

enum AdvTagType
{
	UInt8 = 0,
    UInt16 = 1,
    UInt32 = 2,
	ULong64 = 3,
    Real= 4,
    AnsiString254 = 5,
	List16AnsiString254 = 6
};

enum Adv2TagType
{
	Int8 = 0,
	Int16 = 1,
	Int32 = 2,
	Long64 = 3,
	Real4 = 4,
	UTF8String = 5
};

enum GetByteMode
{
	Normal = 0,
	KeyFrameBytes = 1,
	DiffCorrBytes = 2
};

enum DiffCorrBaseFrame
{
	DiffCorrKeyFrame = 0,
	DiffCorrPrevFrame = 1
};

enum ImageBytesLayout
{
	FullImageRaw = 0,
	FullImageDiffCorrWithSigns = 1
};

enum ImageByteOrder
{
	BigEndian = 0,
	LittleEndian = 1
};

namespace AdvLib2
{
	enum GetByteOperation
	{
		None = 0,
		ConvertTo12BitPacked = 1,
		ConvertTo8BitBytesLooseHighByte = 2
	};

	struct AdvFileInfo
	{
       int Width;
       int Height;
       int CountMaintFrames;
       int CountCalibrationFrames;
       int DataBpp;
       int MaxPixelValue;	   
       __int64 MainClockFrequency;
       int MainStreamAccuracy;
       __int64 CalibrationClockFrequency;
       int CalibrationStreamAccuracy;
	   unsigned char MainStreamTagsCount;
	   unsigned char CalibrationStreamTagsCount;
	   unsigned char SystemMetadataTagsCount;
	   unsigned char UserMetadataTagsCount;
	   __int64 UtcTimestampAccuracyInNanoseconds;
	   bool IsColourImage;
	   int ImageLayoutsCount;
	   int StatusTagsCount;
	   int ImageSectionTagsCount;
	   int ErrorStatusTagId;
	};

	struct AdvFrameInfo
	{	
		unsigned int StartTicksLo;
		unsigned int StartTicksHi;
		unsigned int EndTicksLo;
		unsigned int EndTicksHi;
		
		unsigned int UtcTimestampLo;
		unsigned int UtcTimestampHi;
		unsigned int Exposure;

		float Gamma;
		float Gain;
		float Shutter;
		float Offset;

		unsigned char GPSTrackedSattelites;
		unsigned char GPSAlmanacStatus;
		unsigned char GPSFixStatus;
		char GPSAlmanacOffset;

		unsigned int VideoCameraFrameIdLo;
		unsigned int VideoCameraFrameIdHi;
		unsigned int HardwareTimerFrameIdLo;
		unsigned int HardwareTimerFrameIdHi;

		unsigned int SystemTimestampLo;
		unsigned int SystemTimestampHi;
	};

	struct AdvImageLayoutInfo
	{
		int ImageLayoutId;
		int ImageLayoutTagsCount;
		char ImageLayoutBpp;
		bool IsFullImageRaw;
		bool Is12BitImagePacked;
		bool Is8BitColourImage;
	};
}

void crc32_init(void);
unsigned int compute_crc32(unsigned char *data, int len);

int DateTimeToAdvTicks(int year, int month, int day, int hour, int minute, int sec, int ms);
void AdvTicksToDateTime(int ticks, int *year, int *month, int *day, int *hour, int *minute, int *sec, int *ms);

#endif // UTILS_H