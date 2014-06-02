#include "stdafx.h"
#include "adv_cross_platform.h"
#include "adv_profiling.h"

int advfclose(FILE* file)
{
	AdvProfiling_StartHddOperation();
	fclose(file);
	AdvProfiling_EndHddOperation();
	return 0;
}

FILE* advfopen(const char* fileName, const char* modes)
{
	AdvProfiling_StartHddOperation();
	FILE* file = fopen(fileName, modes);
	AdvProfiling_EndHddOperation();
	return file;
}

size_t advfwrite(const void* pData, size_t size, size_t count, FILE* file)
{
	AdvProfiling_StartHddOperation();
	size_t written = fwrite(pData, size, count, file);
	ADVRPF_HDDWRITE_BYTES_WRITTEN += size * count;
	AdvProfiling_EndHddOperation();
	return written;
}

void advfgetpos64(FILE* file, __int64* pos)
{
	AdvProfiling_StartHddOperation();
	*pos = _ftelli64(file);
	AdvProfiling_EndHddOperation();
}

int advfsetpos64(FILE* file, const __int64* pos, int origin)
{
	AdvProfiling_StartHddOperation();
	int rv = _fseeki64(file, *pos, origin);
	AdvProfiling_EndHddOperation();
	return rv;
}

int advfsetpos64(FILE* file, const __int64* pos)
{
	AdvProfiling_StartHddOperation();
	int rv = _fseeki64(file, *pos, SEEK_SET);
	AdvProfiling_EndHddOperation();
	return rv;
}

int advfseek(FILE* file, __int64 off, int whence)
{
	AdvProfiling_StartHddOperation();
	int rv = advfsetpos64(file, &off, whence);
	AdvProfiling_EndHddOperation();
	return rv;
}

int advfflush(FILE* file)
{
	AdvProfiling_StartHddOperation();
	int rv = fflush(file);
	AdvProfiling_EndHddOperation();
	return rv;
}