/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#ifndef ADV_CROSS_PLATFORM
#define ADV_CROSS_PLATFORM

#define _FILE_OFFSET_BITS  64
#include "stdio.h"

#if _MSC_VER
#define snprintf _snprintf
#define MSVC

// Including SDKDDKVer.h defines the highest available Windows platform.

// If you wish to build your application for a previous Windows platform, include WinSDKVer.h and
// set the _WIN32_WINNT macro to the platform you wish to support before including SDKDDKVer.h.

#include <SDKDDKVer.h>

#endif

#if defined _WIN32 || defined __CYGWIN__
 /*
  #ifdef BUILDING_DLL
    #ifdef __GNUC__
      #define DLL_PUBLIC __attribute__ ((dllexport))
    #else
      #define DLL_PUBLIC __declspec(dllexport) // Note: actually gcc seems to also supports this syntax.
    #endif
  #else
    #ifdef __GNUC__
      #define DLL_PUBLIC __attribute__ ((dllimport))
    #else
      #define DLL_PUBLIC __declspec(dllimport) // Note: actually gcc seems to also supports this syntax.
    #endif
  #endif
   */
    #define DLL_PUBLIC __declspec(dllexport)
    #define DLL_LOCAL   
#else
  #if __GNUC__ >= 4
    #define DLL_PUBLIC __attribute__ ((visibility ("default")))
    #define DLL_LOCAL  __attribute__ ((visibility ("hidden")))
  #else
    #define DLL_PUBLIC
    #define DLL_LOCAL
  #endif
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