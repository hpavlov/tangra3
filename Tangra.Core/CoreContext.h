/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#ifndef CORE_CONTEXT
#define CORE_CONTEXT

#include "cross_platform.h"
#include "adv_profiling.h"
#include "version.h"

enum ColorChannel
{
	Red = 0,
	Green = 1,
	Blue = 2,
	GrayScale = 3
};

// Red = 0, Green = 1, Blue = 2, GrayScale = 3
long s_COLOR_CHANNEL;

#ifndef _WIN32

BOOL SUCCEEDED(HRESULT hr)
{
	return hr >= 0;
}

#endif

// http://www.firstobject.com/fseeki64-ftelli64-in-vc++.htm

int advfsetpos(FILE* file, const __int64* pos)
{
#ifdef __linux__
	return fsetpos64(file, reinterpret_cast<const fpos64_t*>(pos));
#elif _WIN32
	return fseeko64(file, *pos, SEEK_SET);
#elif __APPLE__
	return fseeko(file, (off_t )*pos, SEEK_SET);
#else
	#error Platform not supported
#endif
};

#if __GNUC__
void fopen_s(FILE **f, const char *name, const char *mode) {
    *f = fopen(name, mode);
}
#endif

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
	FILE* file;
	fopen_s(&file, fileName, modes);
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

void advfread(void* pData, size_t size, size_t count, FILE* file)
{
	AdvProfiling_StartHddOperation();
	fread(pData, size, count, file);
	AdvProfiling_EndHddOperation();
}

void advfgetpos64(FILE* file, __int64* pos)
{
	AdvProfiling_StartHddOperation();
	
#ifdef MSVC
	*pos = _ftelli64(file);
#elif _WIN32 || _WIN64
	int rv = fgetpos(file, reinterpret_cast<fpos_t*>(pos));
#elif __linux__
	int rv = fgetpos64(file, reinterpret_cast<fpos64_t*>(pos));
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
	int rv = fseeko64(file, (__off64_t)(*pos), origin);
#elif _WIN32
	int rv = fseeko64(file, *pos, origin);
#elif __APPLE__
	int rv = fseeko(file, (off_t)*pos, origin);
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
	int rv = fsetpos64(file, (fpos64_t*)pos);
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

__int64 advgetclockresolution()
{
	__int64 rv = 0;
#ifdef MSVC
	LARGE_INTEGER li;
	QueryPerformanceFrequency(&li);
	rv = li.QuadPart;
#elif __linux__
	rv = 1E+09; // On Linux time is returned in nanoseconds so frequency is 1E+09 Hz
#elif _WIN32
	LARGE_INTEGER li;
	QueryPerformanceFrequency(&li);
	rv = li.QuadPart;
#elif __APPLE__
	rv = 0;
#else
	#error Platform not supported
#endif
	return rv;
}

__int64 advgetclockticks()
{
	__int64 rv = 0;
#ifdef MSVC
	LARGE_INTEGER li;
	QueryPerformanceCounter(&li);
	rv = li.QuadPart;
#elif __linux__
	struct timespec spec;
	clock_gettime(CLOCK_MONOTONIC, &spec);
	rv = spec.tv_sec * 1E+09 + spec.tv_nsec;
#elif _WIN32
	LARGE_INTEGER li;
	QueryPerformanceCounter(&li);
	rv = li.QuadPart;
#elif __APPLE__
	rv = 0;
#else
	#error Platform not supported
#endif
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
/* Make sure functions are exported with C linkage under C++ compilers. */
#ifdef __cplusplus
extern "C"
{
#endif

HRESULT InitTangraCore()
{
	s_COLOR_CHANNEL = 0;

	return S_OK;
};

HRESULT Set8BitColourChannel(int colourChannel)
{
	s_COLOR_CHANNEL = colourChannel;

	return S_OK;
};

DLL_PUBLIC int GetProductVersion()
{
	return (VERSION_MAJOR << 28) + (VERSION_MINOR << 16) + VERSION_REVISION;
}

DLL_PUBLIC int GetProductBitness()
{
	return sizeof(long) * 8;
}

#ifdef __cplusplus
} // __cplusplus defined.
#endif

#endif