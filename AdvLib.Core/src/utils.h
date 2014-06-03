/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#ifndef UTILS_H
#define UTILS_H

#include <stdio.h>
#include <string.h>
#include <time.h>

void WriteString(FILE* pFile, const char* str);

enum AdvTagType
{
	UInt8 = 0,
	UInt16 = 1,
	UInt32 = 2,
	ULong64 = 3,
	Real = 4, // IEEE/REAL*4
	AnsiString255 = 5,
	List16OfAnsiString255 = 6,
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

void crc32_init(void);
unsigned int compute_crc32(unsigned char *data, int len);

extern time_t TIME_ADV_ZERO;
extern tm* s_timeinfo;
extern time_t s_initTime;

long long SystemTimeToAavTicks(SYSTEMTIME systemTime);
long long DateTimeToAavTicks(__int64 dayTicks, int hour, int minute, int sec, int tenthMs);
long long WindowsTicksToAavTicks(__int64 windowsTicks);
void DebugViewPrint(const wchar_t* formatText, ...);

#endif // UTILS_H