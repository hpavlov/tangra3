#ifndef CROSSPLATFORM_H
#define CROSSPLATFORM_H

#include <stdio.h>
#include <stdlib.h>
#include <strings.h>
#include <cstring>

#ifndef _WIN32

typedef int HRESULT;
typedef int BOOL;
typedef long LONG;
typedef int WORD;
typedef unsigned char BYTE;
typedef unsigned long DWORD;

#define S_OK 0
#define E_FAIL ((HRESULT)0x80004005L)
#define E_NOTIMPL ((HRESULT)0x80004001L)

BOOL SUCCEEDED(HRESULT hr);

#define BI_RGB 0

typedef void* HBITMAP;

#pragma pack(1)
typedef struct tagBITMAPINFOHEADER {
  DWORD biSize;
  LONG  biWidth;
  LONG  biHeight;
  WORD  biPlanes;
  WORD  biBitCount;
  DWORD biCompression;
  DWORD biSizeImage;
  LONG  biXPelsPerMeter;
  LONG  biYPelsPerMeter;
  DWORD biClrUsed;
  DWORD biClrImportant;
} BITMAPINFOHEADER, *PBITMAPINFOHEADER;
#pragma pack()

#pragma pack(1)
typedef struct tagBITMAPFILEHEADER {
  WORD  bfType;
  DWORD bfSize;
  WORD  bfReserved1;
  WORD  bfReserved2;
  DWORD bfOffBits;
} BITMAPFILEHEADER, *PBITMAPFILEHEADER;
#pragma pack()

#else

#include "windows.h"

#endif

#ifdef _linux_
typedef long long __int64;
#endif

int advfsetpos(FILE* file, const __int64* pos);

#endif // CROSSPLATFORM_H