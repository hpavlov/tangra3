/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#ifndef CORE_CONTEXT
#define CORE_CONTEXT

#include "cross_platform.h"
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