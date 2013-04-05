#include "utils.h"
#include "stdlib.h"
#include "strings.h"
#include <time.h>


char* ReadString(FILE* pFile)
{
	char strLen;
	fread(&strLen, 1, 1, pFile);
	strLen++;

	char* strRetVal = (char*)malloc(strLen);
	memset(strRetVal, 0, strLen);

	fgets(strRetVal, strLen, pFile);

	return strRetVal;
}

void WriteString(FILE* pFile, const char* str)
{
	unsigned char len;
	len = strlen(str);
	
	fwrite(&len, 1, 1, pFile);
	fputs(str, pFile);
}

void DbgPrintBytes(unsigned char *bytes, int maxLen)
{
	for(int i = 0; i < maxLen; i++)
	{
		printf("%x", bytes[i]);
		if (i % 10 == 0) printf("\r\n");
	}
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

long DateTimeToAdvTicks(int year, int month, int day, int hour, int minute, int sec, int ms)
{
	// the miliseconds since 1 Jan 2000, 00:00:00.000 (negative vaslues are before 2000)
	//struct tm *timeinfo;
	
	//timeinfo->tm_year = 100;
	//timeinfo->tm_mon = 1;
	//timeinfo->tm_mday = 1;
	
	time_t TIME_ADV_ZERO;
	time_t initTime;
	
	time(&initTime);
	tm *timeinfo = localtime (&initTime);
	
	timeinfo->tm_year = year - 1900;
	timeinfo->tm_mon = month;
	timeinfo->tm_mday = day;
	timeinfo->tm_hour = hour;
	timeinfo->tm_min = minute;
	timeinfo->tm_sec = sec;	
	time_t userTime = mktime(timeinfo);
	
	timeinfo->tm_year = 100;
	timeinfo->tm_mon = 1;
	timeinfo->tm_mday = 1;
	timeinfo->tm_hour = 0;
	timeinfo->tm_min = 0;
	timeinfo->tm_sec = 0;
	TIME_ADV_ZERO = mktime(timeinfo);
	
	double diff = difftime(userTime, TIME_ADV_ZERO);
	
	return 1000 * (long)(diff) + ms;
}

void AdvTicksToDateTime(long ticks, int *year, int *month, int *day, int *hour, int *minute, int *sec, int *ms)
{
	// the miliseconds since 1 Jan 2000, 00:00:00.000 (negative vaslues are before 2000)
}