#ifndef ADV_CROSS_PLATFORM
#define ADV_CROSS_PLATFORM

#define _FILE_OFFSET_BITS  64
#include "stdio.h"

#if _MSC_VER
#define snprintf _snprintf
#endif

FILE* advfopen(const char* fileName, const char* modes);
size_t advfwrite(const void* pData, size_t size, size_t count, FILE* file);
void advfgetpos64(FILE* file, __int64* pos);
int advfsetpos64(FILE* file, const __int64* pos);
int advfsetpos64(FILE* file, const __int64* pos, int origin);
int advfseek(FILE* stream, __int64 off, int whence);
int advfclose(FILE* file);
int advfflush(FILE* file);

#endif