#ifndef UTILS_H
#define UTILS_H

#include <stdio.h>
#include <string.h>

char* ReadString(FILE* pFile);
void WriteString(FILE* pFile, const char* str);

void DbgPrintBytes(unsigned char *bytes, int maxLen);

//const char *KEY_BITPIX = "BITPIX"; // Bits per pixel 
//const char *KEY_WIDTH = "WIDTH";
//const char *KEY_HEIGHT = "HEIGHT";


//const char *KEY_DATE = "DATE"; // Date the file was created
//const char *KEY_DATE_OBS = "DATE-OBS"; // Date of data acquisition
//const char *KEY_DATETIME_OBS_START = "UT-OBS-BEGING";
//const char *KEY_DATETIME_OBS_END = "UT-OBS-END";

//char KEY_DATAFRAME_COMPRESSION[] = "DATA-COMPRESSION";
//char KEY_COMPRESSION_KEY_FRAME_FREQUENCY[] = "COMPRESSION-KEY-FRAME-FREQUENCY";

//const char *KEY_INSTRUMENT = "INSTRUME";
//const char *KEY_TELESCOPE = "TELESCOPE";
//const char *KEY_OBSERVER = "OBSERVER";
//const char *KEY_COMMENT = "COMMENT";
//const char *KEY_RECORDER = "RECORDER";
//const char *KEY_LONGITUDE = "LONGITUDE";
//const char *KEY_LATITUDE = "LATITUDE";

//char SECTION_IMAGE[] = "IMAGE";
//char SECTION_SYSTEM_STATUS[] = "STATUS";
//char COMPR_UNCOMPRESSED[] = "UNCOMPRESSED";
//const char *COMPR_DIFF_CORR_HUFFMAN = "DCHUFFMAN";
//char COMPR_DIFF_CORR_QUICKLZ[] = "DCQUICKLZ";

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