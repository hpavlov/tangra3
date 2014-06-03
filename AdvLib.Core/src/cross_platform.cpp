/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#include "stdafx.h"
#include "cross_platform.h"
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
	
#ifdef MSVC
	*pos = _ftelli64(file);
#elif _WIN32
	int rv = fgetpos(file, reinterpret_cast<fpos_t*>(pos));
#else
	#error Platform not supported
#endif

	AdvProfiling_EndHddOperation();
}

int advfsetpos64(FILE* file, const __int64* pos, int origin)
{
	AdvProfiling_StartHddOperation();
	
#ifdef MSVC
	int rv = _fseeki64(file, *pos, origin);
#elif __linux__
	int rv = fsetpos64(file, reinterpret_cast<const fpos64_t*>(pos));
#elif _WIN32
	int rv = fseeko64(file, *pos, origin);
#elif __APPLE__
	int rv = fseeko(file, (off_t )*pos, origin);
#else
	#error Platform not supported
#endif
	
	AdvProfiling_EndHddOperation();

	return rv;
}

int advfsetpos64(FILE* file, const __int64* pos)
{
	AdvProfiling_StartHddOperation();
	
#ifdef MSVC
	int rv = _fseeki64(file, *pos, SEEK_SET);
#elif __linux__
	int rv = fsetpos64(file, reinterpret_cast<const fpos64_t*>(pos));
#elif _WIN32
	int rv = fseeko64(file, *pos, SEEK_SET);
#elif __APPLE__
	int rv = fseeko(file, (off_t )*pos, SEEK_SET);
#else
	#error Platform not supported
#endif
	
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