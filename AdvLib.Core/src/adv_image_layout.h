/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#ifndef ADV_IMAGE_LAYOUT
#define ADV_IMAGE_LAYOUT

#include <stdio.h>
#include "utils.h"
#include "quicklz.h"

#include <map>
#include <string>

using namespace std;
using std::string;

namespace AdvLib
{
	class AdvImageLayout 
	{

	private:
		unsigned char *SIGNS_MASK;
		map<string, string> m_LayoutTags;
		ImageBytesLayout m_BytesLayout;	
		
		int m_KeyFrameBytesCount;
		unsigned short *m_PrevFramePixels;
		unsigned short *m_PrevFramePixelsTemp;
		unsigned char *m_PixelArrayBuffer;
		unsigned int m_MaxSignsBytesCount;
		unsigned int m_MaxPixelArrayLengthWithoutSigns;
		char* m_CompressedPixels;
		qlz_state_compress* m_StateCompress;		
		
	public:
		unsigned char LayoutId;
		unsigned int Width;
		unsigned int Height;		
		unsigned char Bpp;
	
		const char* Compression;
		bool IsDiffCorrLayout;
		int KeyFrame;
		
		int MaxFrameBufferSize;
		enum DiffCorrBaseFrame BaseFrameType;
	
	private:
		void GetDataBytes12Bpp(unsigned short* currFramePixels, enum GetByteMode mode, unsigned int pixelsCRC32, unsigned int *bytesCount, unsigned char dataPixelsBpp);
		void GetDataBytes16Bpp(unsigned short* currFramePixels, enum GetByteMode mode, unsigned int pixelsCRC32, unsigned int *bytesCount, unsigned char dataPixelsBpp);
		
		void GetDataBytes12BppIndex12BppWords(unsigned short* currFramePixels, enum GetByteMode mode, unsigned int pixelsCRC32, unsigned int *bytesCount, unsigned char dataPixelsBpp);
		void GetDataBytes12BppIndex16BppWords(unsigned short* currFramePixels, enum GetByteMode mode, unsigned int pixelsCRC32, unsigned int *bytesCount, unsigned char dataPixelsBpp);
		void GetDataBytes12BppIndexBytes(unsigned short* currFramePixels, enum GetByteMode mode, unsigned int pixelsCRC32, unsigned int *bytesCount, unsigned char dataPixelsBpp);
		
		unsigned char* GetFullImageDiffCorrWithSignsDataBytes(unsigned short* currFramePixels, enum GetByteMode mode, unsigned int *bytesCount, unsigned char dataPixelsBpp);
		unsigned char* GetFullImageRawDataBytes(unsigned short* currFramePixels, unsigned int *bytesCount, unsigned char dataPixelsBpp);
		
		
		void ResetBuffers();
		
	public:
		AdvImageLayout(unsigned int width, unsigned int height, unsigned char layoutId, const char* layoutType, const char* compression, unsigned char bpp, int keyFrame);
		~AdvImageLayout();
		
		void AddOrUpdateTag(const char* tagName, const char* tagValue);
		unsigned char* GetDataBytes(unsigned short* currFramePixels, enum GetByteMode mode, unsigned int *bytesCount, unsigned char dataPixelsBpp);
		void WriteHeader(FILE* pfile);
		void StartNewDiffCorrSequence();
	};

};
#endif


