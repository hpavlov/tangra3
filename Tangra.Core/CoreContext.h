#ifndef CORE_CONTEXT
#define CORE_CONTEXT

#include "cross_platform.h"

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

HRESULT Set8BitColourChannel(long colourChannel)
{
	s_COLOR_CHANNEL = colourChannel;

	return S_OK;
};

DLL_PUBLIC int GetProductVersion()
{
	int major = 3;
	int minor = 0;
	int revision = 15;

	return (major << 28) + (minor << 16) + revision;
}

#ifdef __cplusplus
} // __cplusplus defined.
#endif

#endif