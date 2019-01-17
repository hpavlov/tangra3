/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#pragma once

#include <string>
#include <stdio.h>
#include "cross_platform.h"

using namespace std;
using std::string;

namespace SerLib
{
	enum ColourFormat
	{
		ColourFormat_MONO = 0,
		ColourFormat_BAYER_RGGB = 8,
		ColourFormat_BAYER_GRBG = 9,
		ColourFormat_BAYER_GBRG = 10,
		ColourFormat_BAYER_BGGR = 11,
		ColourFormat_BAYER_CYYM = 16,
		ColourFormat_BAYER_YCMY = 17,
		ColourFormat_BAYER_YMCY = 18,
		ColourFormat_BAYER_MYYC = 19,
		ColourFormat_RGB = 100,
		ColourFormat_BGR = 101
	};
	
	struct SerFileInfo
	{
		int CameraId;
		int ColourId;
		int LittleEndian;
		int Width;
		int Height;
		int PixelDepthPerPlane;
		int CountFrames;
		int SequenceStartTimeLo;
		int SequenceStartTimeHi;
		int SequenceStartTimeUTCLo;
		int SequenceStartTimeUTCHi;
		unsigned int NormalisationValue;
		int NumPlanes;
	};
	
	struct SerFrameInfo
	{
		int TimeStampUtcLo;
		int TimeStampUtcHi;
		unsigned __int64 TimeStampUtc64;
	};
	
	struct MarshalledSerFrameInfo
	{
		int TimeStampUtcLo;
		int TimeStampUtcHi;
	};	
	
	class SerFile {
		private:
			char* m_Telescope;
			FILE* m_File;
			int m_ColourId;
			int m_NumPlanes;
			int m_BytesPerPixel;
			int m_PixelsPerFrame;
			int m_CountFrames;
			__int64 m_TimeStampStartOffset;
			bool m_GrayScaleRGB;
		
		private:
			unsigned char* m_RawFrameBuffer;
			int m_RawFrameSize;
			HRESULT ProcessRawFrame(unsigned int* pixels, unsigned int cameraBitPix);
			
		public:
			int Width;
			int Height;
			int Bpp;
			bool LittleEndian;
			int NormalisationValue;
			bool HasTimeStamps;
		
		public:
			SerFile();
			~SerFile();
			
			void OpenFile(const char* filePath, SerLib::SerFileInfo* fileInfo, char* observer, char* instrument, char* telescope, bool checkMagic, bool greyScaleRGB);
			void CloseFile();
			HRESULT GetFrame(int frameNo, unsigned int* pixels, unsigned int cameraBitPix, SerLib::SerFrameInfo* frameInfo);
			HRESULT GetFrameInfo(int frameNo, SerLib::SerFrameInfo* frameInfo);
	};
}

/* Make sure functions are exported with C linkage under C++ compilers. */
#ifdef __cplusplus
extern "C"
{
#endif

DLL_PUBLIC HRESULT SEROpenFile(char* fileName, SerLib::SerFileInfo* fileInfo, char* observer, char* instrument, char* telescope, bool checkMagic, bool greyScaleRGB);
DLL_PUBLIC HRESULT SERCloseFile();
DLL_PUBLIC HRESULT SERGetFrame(int frameNo, unsigned int* pixels, unsigned int* originalPixels, BYTE* bitmapPixels, BYTE* bitmapBytes, unsigned int cameraBitPix, SerLib::MarshalledSerFrameInfo* frameInfo);		
DLL_PUBLIC HRESULT SERGetIntegratedFrame(int startFrameNo, int framesToIntegrate, bool isSlidingIntegration, bool isMedianAveraging, unsigned int* pixels, unsigned int* originalPixels, BYTE* bitmapBytes, BYTE* bitmapDisplayBytes, unsigned int cameraBitPix, SerLib::MarshalledSerFrameInfo* frameInfo);
DLL_PUBLIC HRESULT SERGetFrameInfo(int frameNo, SerLib::MarshalledSerFrameInfo* frameInfo);
		
#ifdef __cplusplus
} // __cplusplus defined.
#endif