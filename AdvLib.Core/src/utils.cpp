/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#include "StdAfx.h"
#include "utils.h"

void WriteString(FILE* pFile, const char* str)
{
	unsigned char len;
	len = strlen(str);
	
	fwrite(&len, 1, 1, pFile);
	fputs(str, pFile);
}

unsigned int crctab[256];

void crc32_init(void)
{
    int i,j;

    unsigned int crc;

    for (i = 0; i < 256; i++)
    {
        crc = i << 24;
        for (j = 0; j < 8; j++)
        {
            if (crc & 0x80000000)
                crc = (crc << 1) ^ 0x04c11db7;
            else
                crc = crc << 1;
        }
        crctab[i] = crc;
    }
}

unsigned int compute_crc32(unsigned char *data, int len)
{
    unsigned int        result;
    int                 i;
    unsigned char       octet;
    
    result = *data++ << 24;
    result |= *data++ << 16;
    result |= *data++ << 8;
    result |= *data++;
    result = ~ result;
    len -=4;
    
    for (i=0; i<len; i++)
    {
        result = (result << 8 | *data++) ^ crctab[result >> 24];
    }
    
    return ~result;
}

#define ADV_EPOCH_ZERO_TICKS 633979008000000000
#define EPOCH_1601_JAN_1_TICKS 504911232000000000

long long SystemTimeToAavTicks(SYSTEMTIME systemTime)
{
	FILETIME fileTime;
	SystemTimeToFileTime(&systemTime, &fileTime);

	ULARGE_INTEGER uli;
	uli.LowPart = fileTime.dwLowDateTime;
	uli.HighPart = fileTime.dwHighDateTime;

	return WindowsTicksToAavTicks(uli.QuadPart + EPOCH_1601_JAN_1_TICKS);
}

long long DateTimeToAavTicks(__int64 dayTicks, int hour, int minute, int sec, int tenthMs)
{
	if (dayTicks > 0)
	{
		long long advTicks = 
				(long long)(dayTicks - ADV_EPOCH_ZERO_TICKS) / 10000 + 
				(long long)(hour * 3600 + minute * 60 + sec) * 1000 +
				tenthMs / 10;

		return advTicks;		
	}
	else
		return 0;
}

long long WindowsTicksToAavTicks(__int64 windowsTicks)
{
	if (windowsTicks > 0)
	{
		long long advTicks = 
				(long long)(windowsTicks - ADV_EPOCH_ZERO_TICKS) / 10000;

		return advTicks;		
	}
	else
		return 0;
}

void DebugViewPrint(const wchar_t* formatText, ...)
{
#ifdef MSVC	
	wchar_t debug512CharBuffer[512];
    va_list args;
    va_start(args, formatText);
	vswprintf(debug512CharBuffer, 512, formatText, args);
    
	OutputDebugString(debug512CharBuffer);
	va_end(args);
#endif	
}