#ifndef UTILS_H
#define UTILS_H

#include <stdio.h>
#include <string.h>

char* ReadString(FILE* pFile);
void WriteString(FILE* pFile, const char* str);

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

void crc32_init(void);
unsigned int compute_crc32(unsigned char *data, int len);

long DateTimeToAdvTicks(int year, int month, int day, int hour, int minute, int sec, int ms);
void AdvTicksToDateTime(long ticks, int *year, int *month, int *day, int *hour, int *minute, int *sec, int *ms);

#endif // UTILS_H