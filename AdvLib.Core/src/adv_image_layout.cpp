/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#include "stdafx.h"
#include "adv_image_layout.h"
#include "utils.h"
#include "stdlib.h"
#include "math.h"
#include <stdio.h>
#include <assert.h>

#include "cross_platform.h"
#include "adv_profiling.h"

namespace AdvLib
{

bool m_UsesCompression;
	
AdvImageLayout::AdvImageLayout(unsigned int width, unsigned int height, unsigned char layoutId, const char* layoutType, const char* compression, unsigned char bpp, int keyFrame)
{	
	LayoutId = layoutId;
	Width = width;
	Height = height;
	Bpp = bpp;	
	KeyFrame = keyFrame;
	IsDiffCorrLayout = false;
	
	SIGNS_MASK = new unsigned char(8);
	SIGNS_MASK[0] = 0x01;
	SIGNS_MASK[1] = 0x02;
	SIGNS_MASK[2] = 0x04;
	SIGNS_MASK[3] = 0x08;
	SIGNS_MASK[4] = 0x10;
	SIGNS_MASK[5] = 0x20;
	SIGNS_MASK[6] = 0x40;
	SIGNS_MASK[7] = 0x80;
	
	if (Bpp == 12)
	{
		MaxFrameBufferSize	= (Width * Height * 3 / 2) + 1 + 4 + 2 * ((Width * Height) % 2) + 16;
	}
	else if (Bpp == 16)
	{
		MaxFrameBufferSize = (Width * Height * 2) + 1 + 4 + 16;
	}
	else 
		MaxFrameBufferSize = Width * Height * 4 + 1 + 4 + + 16;

	AddOrUpdateTag("DATA-LAYOUT", layoutType);
	AddOrUpdateTag("SECTION-DATA-COMPRESSION", compression);

	Compression = new char[strlen(compression) + 1];
	strcpy(const_cast<char*>(Compression), compression);
	m_UsesCompression = 0 != strcmp(compression, "UNCOMPRESSED");
	
	if (keyFrame > 0)
	{
		char keyFrameStr [5];
		snprintf(keyFrameStr, 5, "%d", keyFrame);
		AddOrUpdateTag("DIFFCODE-KEY-FRAME-FREQUENCY", keyFrameStr);		
		AddOrUpdateTag("DIFFCODE-BASE-FRAME", "KEY-FRAME");		
	}
	
	m_MaxSignsBytesCount = (unsigned int)ceil(Width * Height / 8.0);
	
	if (Bpp == 12)
		m_MaxPixelArrayLengthWithoutSigns = 1 + 4 + 3 * (Width * Height) / 2 + 2 * ((Width * Height) % 2);	
	else if (Bpp == 16)
		m_MaxPixelArrayLengthWithoutSigns = 1 + 4 + 2 * Width * Height;	
	else
		m_MaxPixelArrayLengthWithoutSigns = 1 + 4 + 4 * Width * Height;	
		
	m_MaxPixelArrayLengthWithoutSigns = 1 + 4 + 4 * Width * Height;	
	m_KeyFrameBytesCount = Width * Height * sizeof(unsigned short);
	
	m_PrevFramePixels = NULL;
	m_PrevFramePixelsTemp = NULL;
	m_PixelArrayBuffer = NULL;
	m_CompressedPixels = NULL;	
	m_StateCompress = NULL;
	
	m_PixelArrayBuffer = (unsigned char*)malloc(m_MaxPixelArrayLengthWithoutSigns + m_MaxSignsBytesCount);
	m_PrevFramePixels = (unsigned short*)malloc(m_KeyFrameBytesCount);		
	memset(m_PrevFramePixels, 0, m_KeyFrameBytesCount);
	
	m_PrevFramePixelsTemp = (unsigned short*)malloc(m_KeyFrameBytesCount);	
	m_CompressedPixels = (char*)malloc(m_MaxPixelArrayLengthWithoutSigns + m_MaxSignsBytesCount + 401);
	
	m_StateCompress = (qlz_state_compress *)malloc(sizeof(qlz_state_compress));
}

AdvImageLayout::~AdvImageLayout()
{
	ResetBuffers();	
}

void AdvImageLayout::ResetBuffers()
{
	if (NULL != m_PrevFramePixels)
		delete m_PrevFramePixels;		

	if (NULL != m_PrevFramePixelsTemp)
		delete m_PrevFramePixelsTemp;		

	if (NULL != m_PixelArrayBuffer)
		delete m_PixelArrayBuffer;

	if (NULL != m_CompressedPixels)
		delete m_CompressedPixels;
	
	if (NULL != m_StateCompress)
		delete m_StateCompress;	
	
	m_PrevFramePixels = NULL;
	m_PrevFramePixelsTemp = NULL;
	m_PixelArrayBuffer = NULL;
	m_CompressedPixels = NULL;	
	m_StateCompress = NULL;
}


void AdvImageLayout::StartNewDiffCorrSequence()
{
   //TODO: Reset the prev frame buffer (do we need to do anything??)
}

void AdvImageLayout::AddOrUpdateTag(const char* tagName, const char* tagValue)
{
	map<string, string>::iterator curr = m_LayoutTags.begin();
	while (curr != m_LayoutTags.end()) 
	{
		const char* existingTagName = curr->first.c_str();	
		
		if (0 == strcmp(existingTagName, tagName))
		{
			m_LayoutTags.erase(curr);
			break;
		}
		
		curr++;
	}
	
	m_LayoutTags.insert(make_pair(string(tagName), string(tagValue)));
	
	if (0 == strcmp("DIFFCODE-BASE-FRAME", tagName))
	{
		if (0 == strcmp("KEY-FRAME", tagValue))
		{
			BaseFrameType = DiffCorrKeyFrame;
		}
		else if (0 == strcmp("PREV-FRAME", tagValue))
		{
			BaseFrameType = DiffCorrPrevFrame;
		}
	}
	
	if (0 == strcmp("DATA-LAYOUT", tagName))
	{
		m_BytesLayout = FullImageRaw;
		if (0 == strcmp("FULL-IMAGE-DIFFERENTIAL-CODING", tagValue)) m_BytesLayout = FullImageDiffCorrWithSigns;
		IsDiffCorrLayout = m_BytesLayout == FullImageDiffCorrWithSigns;
	}	
}


void AdvImageLayout::WriteHeader(FILE* pFile)
{
	unsigned char buffChar;
	
	buffChar = 1;
	fwrite(&buffChar, 1, 1, pFile); /* Version */

	fwrite(&Bpp, 1, 1, pFile);	

	
	buffChar = (unsigned char)m_LayoutTags.size();
	fwrite(&buffChar, 1, 1, pFile);
	
	map<string, string>::iterator curr = m_LayoutTags.begin();
	while (curr != m_LayoutTags.end()) 
	{
		char* tagName = const_cast<char*>(curr->first.c_str());	
		WriteString(pFile, tagName);
		
		char* tagValue = const_cast<char*>(curr->second.c_str());	
		WriteString(pFile, tagValue);
		
		curr++;
	}		
}


unsigned int WordSignMask(int bit)
{
	switch (bit + 1)	
	{
		case 1:
			return 0x00000001;
			break;
		case 2:
			return 0x00000002;
			break;
		case 3:
			return 0x00000004;
			break;
		case 4:
			return 0x00000008;
			break;
		case 5:
			return 0x00000010;
			break;
		case 6:
			return 0x00000020;
			break;
		case 7:
			return 0x00000040;
			break;
		case 8:
			return 0x00000080;
			break;
		case 9:
			return 0x00000100;
			break;
		case 10:
			return 0x00000200;
			break;
		case 11:
			return 0x00000400;
			break;
		case 12:
			return 0x00000800;
			break;
		case 13:
			return 0x00001000;
			break;
		case 14:
			return 0x00002000;
			break;
		case 15:
			return 0x00004000;
			break;
		case 16:
			return 0x00008000;
			break;			
		case 17:
			return 0x00010000;
			break;
		case 18:
			return 0x00020000;
			break;
		case 19:
			return 0x00040000;
			break;
		case 20:
			return 0x00080000;
			break;
		case 21:
			return 0x00100000;
			break;
		case 22:
			return 0x00200000;
			break;
		case 23:
			return 0x00400000;
			break;
		case 24:
			return 0x00800000;
			break;
		case 25:
			return 0x01000000;
			break;
		case 26:
			return 0x02000000;
			break;
		case 27:
			return 0x04000000;
			break;
		case 28:
			return 0x08000000;
			break;
		case 29:
			return 0x10000000;
			break;
		case 30:
			return 0x20000000;
			break;
		case 31:
			return 0x40000000;
			break;
		case 32:
			return 0x80000000;
			break;
		default:
			return 0x00000000;
			break;			
	}
}


unsigned char* AdvImageLayout::GetDataBytes(unsigned short* currFramePixels, enum GetByteMode mode, unsigned int *bytesCount, unsigned char dataPixelsBpp)
{
	unsigned char* bytesToCompress;
	
	if (m_BytesLayout == FullImageDiffCorrWithSigns)
		bytesToCompress = GetFullImageDiffCorrWithSignsDataBytes(currFramePixels, mode, bytesCount, dataPixelsBpp);
	else if (m_BytesLayout == FullImageRaw)
		bytesToCompress = GetFullImageRawDataBytes(currFramePixels, bytesCount, dataPixelsBpp);

	
	if (0 == strcmp(Compression, "QUICKLZ"))
	{
		unsigned int frameSize = 0;
				
		AdvProfiling_StartFrameCompression();

		// compress and write result 
		size_t len2 = qlz_compress(bytesToCompress, m_CompressedPixels, *bytesCount, m_StateCompress); 		
		
		AdvProfiling_EndFrameCompression();
	
		*bytesCount = len2;
		return (unsigned char*)(m_CompressedPixels);
	}
	else if (0 == strcmp(Compression, "UNCOMPRESSED"))
	{
		return bytesToCompress;
	}
	
		
	return NULL;
}

unsigned char* AdvImageLayout::GetFullImageRawDataBytes(unsigned short* currFramePixels, unsigned int *bytesCount, unsigned char dataPixelsBpp)
{
	int buffLen = 0;
	if (dataPixelsBpp == 16)
	{
		buffLen = Width * Height * 2;
		memcpy(&m_PixelArrayBuffer[0], &currFramePixels[0], buffLen);		
	}
	else if (dataPixelsBpp == 8)
	{
		buffLen = Width * Height;
		memcpy(&m_PixelArrayBuffer[0], &currFramePixels[0], buffLen);
	}
	else
		throw "12Bpp not supported in Raw layout";
	
	*bytesCount = buffLen;
	return m_PixelArrayBuffer;
}

unsigned char* AdvImageLayout::GetFullImageDiffCorrWithSignsDataBytes(unsigned short* currFramePixels, enum GetByteMode mode, unsigned int *bytesCount, unsigned char dataPixelsBpp)
{
	bool isKeyFrame = mode == KeyFrameBytes;
	bool diffCorrFromPrevFramePixels = isKeyFrame || this->BaseFrameType == DiffCorrPrevFrame;
	
	if (diffCorrFromPrevFramePixels)
	{
		// STEP1 from maintaining the old pixels for DiffCorr
		if (mode == DiffCorrBytes)
			memcpy(&m_PrevFramePixelsTemp[0], &currFramePixels[0], m_KeyFrameBytesCount);
		else
			memcpy(&m_PrevFramePixels[0], &currFramePixels[0], m_KeyFrameBytesCount);
	}	

    // NOTE: The CRC computation is a huge overhead and is currently disabled
	//unsigned int pixelsCrc = ComputePixelsCRC32(currFramePixels);
	unsigned int pixelsCrc = 0;
	
	if (mode == KeyFrameBytes)
	{
		*bytesCount = 0;
	}
	else if (mode == DiffCorrBytes)
	{					
		*bytesCount = 0;
	
		AdvProfiling_StartTestingOperation();	

		unsigned int* pCurrFramePixels = (unsigned int*)currFramePixels;
		unsigned int* pPrevFramePixels = (unsigned int*)m_PrevFramePixels;
		for (int j = 0; j < Height; ++j)
		{
			for (int i = 0; i < Width / 2; ++i)
			{
				int wordCurr = (int)*pCurrFramePixels;
				int wordOld = (int)*pPrevFramePixels;
				
				unsigned int pixLo = (unsigned int)((unsigned short)((wordCurr & 0xFFFF) - (wordOld & 0xFFFF)));
				unsigned int pixHi = (unsigned int)((unsigned short)(((wordCurr & 0xFFFF0000) >> 16) - ((wordOld & 0xFFFF0000) >> 16)));
				
				/*
				unsigned short currLo = (unsigned short)pixLo + (unsigned short)(wordOld & 0xFFFF);
				unsigned short currHi = (unsigned short)pixHi + (unřsigned short)((wordOld & 0xFFFF0000) >> 16);
				
				if ((j > 0 || i > 8))
				{
					if (currLo != (unsigned short)(wordCurr & 0xFFFF) ||
						currHi != (unsigned short)((wordCurr & 0xFFFF0000) >> 16) ||
						wordCurr & 0x000F00F != 0)
						{
							printf("%d", currLo);
						}				
				} */
				
				*pCurrFramePixels = (pixHi << 16) + pixLo;
				
				pCurrFramePixels++;
				pPrevFramePixels++;
			}
		}
		AdvProfiling_EndTestingOperation();
	}
	
	if (diffCorrFromPrevFramePixels && mode == DiffCorrBytes)
		// STEP2 from maintaining the old pixels for DiffCorr
		memcpy(&m_PrevFramePixels[0], &m_PrevFramePixelsTemp[0], m_KeyFrameBytesCount);
	
	if (Bpp == 12)
	{		
		GetDataBytes12Bpp(currFramePixels, mode, pixelsCrc, bytesCount, dataPixelsBpp);
		return m_PixelArrayBuffer;
	}
	else if (Bpp = 16)
	{
		GetDataBytes16Bpp(currFramePixels, mode, pixelsCrc, bytesCount, dataPixelsBpp);
		return m_PixelArrayBuffer;
	}
	else
	{
		*bytesCount = 0;
		return NULL;
	}
}

void AdvImageLayout::GetDataBytes12BppIndex12BppWords(unsigned short* pixels, enum GetByteMode mode, unsigned int pixelsCRC32, unsigned int *bytesCount, unsigned char dataPixelsBpp)
{	
	// NOTE: This code has never been tested or used !!!

	// Flags: 0 - no key frame used, 1 - key frame follows, 2 - diff corr data follows
	bool isKeyFrame = mode == KeyFrameBytes;
	bool noKeyFrameUsed = mode == Normal;
	bool isDiffCorrFrame = mode == DiffCorrBytes;

	// Every 2 12-bit values can be encoded in 3 bytes
	// xxxxxxxx|xxxxyyyy|yyyyyyy

	//int arrayLength = 1 + 4 + 3 * (Width * Height) / 2 + 2 * ((Width * Height) % 2) + *bytesCount;

	//unsigned char *imageData = (unsigned char*)malloc(arrayLength);
	
	//int signsBytesCnt = *bytesCount;
	
	unsigned int bytesCounter = *bytesCount;
	
	m_PixelArrayBuffer[0] = noKeyFrameUsed ? (unsigned char)0 : (isKeyFrame ? (unsigned char)1 : (unsigned char)2);
	bytesCounter++;
		
	unsigned int* pPixelArrayWords =  (unsigned int*)(&m_PixelArrayBuffer[0] + bytesCounter);
	unsigned int* pPixels = (unsigned int*)pixels;
	
	int counter = 0;
	int pixel8GroupCount = Height * Width / 8;
	for (int idx = 0; idx < pixel8GroupCount; ++idx)
	{
		unsigned int word1 = *pPixels;
		unsigned int word2 = *pPixels;pPixels++;
		unsigned int word3 = *pPixels;pPixels++;
		unsigned int word4 = *pPixels;pPixels++;
				
		//         word1                 word2                 word3                 word4
		// | 00aa aaaa 00bb bbbb | 00cc cccc 00dd dddd | 00ee eeee 00ff ffff | 00gg gggg 00hh hhhh|
        // | aaaa aabb bbbb cccc | ccdd dddd eeee eeff | ffff gggg gghh hhhh |
		//       encoded1               encoded2             encoded3
		
		unsigned int encodedPixelWord1 = ((word1 << 4) && 0xFFF00000) + ((word1 << 8) && 0x000FFF00) + (word2 >> 20);
		unsigned int encodedPixelWord2 = ((word2 << 12) && 0xF0000000) + (word2 << 16) + ((word3 >> 12) && 0x0000FFF0)+ ((word3 >> 8) && 0x0000000F);
		unsigned int encodedPixelWord3 = (word4 << 24) + ((word4 >> 4) && 0x00FFF000) + (word4 && 0x00000FFF);
		
		*pPixelArrayWords = encodedPixelWord1;pPixelArrayWords++;
		*pPixelArrayWords = encodedPixelWord2;pPixelArrayWords++;
		*pPixelArrayWords = encodedPixelWord3;pPixelArrayWords++;
		
		bytesCounter += 12;
	};

	*pPixelArrayWords = pixelsCRC32; pPixelArrayWords++;	
	(*bytesCount) = bytesCounter + 4;

	//if (isDiffCorrFrame)
	//	memcpy(&m_PixelArrayBuffer[1], &m_SignsBuffer[0], signsBytesCnt);
}

void AdvImageLayout::GetDataBytes12BppIndex16BppWords(unsigned short* pixels, enum GetByteMode mode, unsigned int pixelsCRC32, unsigned int *bytesCount, unsigned char dataPixelsBpp)
{	
	// Flags: 0 - no key frame used, 1 - key frame follows, 2 - diff corr data follows
	bool isKeyFrame = mode == KeyFrameBytes;
	bool noKeyFrameUsed = mode == Normal;
	bool isDiffCorrFrame = mode == DiffCorrBytes;

	// Every 2 12-bit values can be encoded in 3 bytes
	// xxxxxxxx|xxxxyyyy|yyyyyyy

	//int arrayLength = 1 + 4 + 3 * (Width * Height) / 2 + 2 * ((Width * Height) % 2) + *bytesCount;

	//unsigned char *imageData = (unsigned char*)malloc(arrayLength);
	
	//int signsBytesCnt = *bytesCount;
	
	unsigned int bytesCounter = *bytesCount;
	
	m_PixelArrayBuffer[0] = noKeyFrameUsed ? (unsigned char)0 : (isKeyFrame ? (unsigned char)1 : (unsigned char)2);
	bytesCounter++;
		
	unsigned int* pPixelArrayWords =  (unsigned int*)(&m_PixelArrayBuffer[0] + bytesCounter);
	unsigned int* pPixels = (unsigned int*)pixels;
	
	int counter = 0;
	int pixel8GroupCount = Height * Width / 8;
	for (int idx = 0; idx < pixel8GroupCount; ++idx)
	{
		unsigned int word1 = *pPixels;
		unsigned int word2 = *pPixels;pPixels++;
		unsigned int word3 = *pPixels;pPixels++;
		unsigned int word4 = *pPixels;pPixels++;
		
		//(int)(pixels[x + y * Width] * 4095 / 65535)
		
		//         word1                 word2                 word3                 word4
		// | 00aa aaaa 00bb bbbb | 00cc cccc 00dd dddd | 00ee eeee 00ff ffff | 00gg gggg 00hh hhhh|
        // | aaaa aabb bbbb cccc | ccdd dddd eeee eeff | ffff gggg gghh hhhh |
		//       encoded1               encoded2             encoded3
		
		unsigned int encodedPixelWord1 = ((word1 << 4) && 0xFFF00000) + ((word1 << 8) && 0x000FFF00) + (word2 >> 20);
		unsigned int encodedPixelWord2 = ((word2 << 12) && 0xF0000000) + (word2 << 16) + ((word3 >> 12) && 0x0000FFF0)+ ((word3 >> 8) && 0x0000000F);
		unsigned int encodedPixelWord3 = (word4 << 24) + ((word4 >> 4) && 0x00FFF000) + (word4 && 0x00000FFF);
		
		*pPixelArrayWords = encodedPixelWord1;pPixelArrayWords++;
		*pPixelArrayWords = encodedPixelWord2;pPixelArrayWords++;
		*pPixelArrayWords = encodedPixelWord3;pPixelArrayWords++;
		
		bytesCounter += 12;
	};

	*pPixelArrayWords = pixelsCRC32; pPixelArrayWords++;	
	(*bytesCount) = bytesCounter + 4;

	//if (isDiffCorrFrame)
	//	memcpy(&m_PixelArrayBuffer[1], &m_SignsBuffer[0], signsBytesCnt);
}

void AdvImageLayout::GetDataBytes12BppIndexBytes(unsigned short* pixels, enum GetByteMode mode, unsigned int pixelsCRC32, unsigned int *bytesCount, unsigned char dataPixelsBpp)
{	
	// Flags: 0 - no key frame used, 1 - key frame follows, 2 - diff corr data follows
	bool isKeyFrame = mode == KeyFrameBytes;
	bool noKeyFrameUsed = mode == Normal;
	bool isDiffCorrFrame = mode == DiffCorrBytes;

	// Every 2 12-bit values can be encoded in 3 bytes
	// xxxxxxxx|xxxxyyyy|yyyyyyy

	unsigned int bytesCounter = *bytesCount;
	
	//m_PixelArrayBuffer[0] = noKeyFrameUsed ? (unsigned char)0 : (isKeyFrame ? (unsigned char)1 : (unsigned char)2);
	//bytesCounter++;
		
	int counter = 0;
	for (int y = 0; y < Height; ++y)
	{
		for (int x = 0; x < Width; ++x)
		{					
			unsigned short value =  dataPixelsBpp == 12 
				? (unsigned short)(pixels[x + y * Width] & 0xFFF)
				: (unsigned short)(pixels[x + y * Width] >> 4);
				
			counter++;

			switch (counter % 2)
			{
				case 1:
					m_PixelArrayBuffer[bytesCounter] = (unsigned char)(value >> 4);
					bytesCounter++;
					
					m_PixelArrayBuffer[bytesCounter] = (unsigned char)((value & 0x0F) << 4);
					break;

				case 0:
					m_PixelArrayBuffer[bytesCounter] += (unsigned char)(value >> 8);
					bytesCounter++;
					m_PixelArrayBuffer[bytesCounter] = (unsigned char)(value & 0xFF);
					bytesCounter++;
					break;
			}
		}
	}

	//m_PixelArrayBuffer[bytesCounter] = (unsigned char)(pixelsCRC32 & 0xFF);
	//m_PixelArrayBuffer[bytesCounter + 1] = (unsigned char)((pixelsCRC32 >> 8) & 0xFF);
	//m_PixelArrayBuffer[bytesCounter + 2] = (unsigned char)((pixelsCRC32 >> 16) & 0xFF);
	//m_PixelArrayBuffer[bytesCounter + 3] = (unsigned char)((pixelsCRC32 >> 24) & 0xFF);
	(*bytesCount) = bytesCounter;
}

void AdvImageLayout::GetDataBytes12Bpp(unsigned short* pixels, enum GetByteMode mode, unsigned int pixelsCRC32, unsigned int *bytesCount, unsigned char dataPixelsBpp)
{
	GetDataBytes12BppIndexBytes(pixels, mode, pixelsCRC32, bytesCount, dataPixelsBpp);
}

void AdvImageLayout::GetDataBytes16Bpp(unsigned short* pixels, enum GetByteMode mode, unsigned int pixelsCRC32, unsigned int *bytesCount, unsigned char dataPixelsBpp)
{
	/*
	// Flags: 0 - no key frame used, 1 - key frame follows, 2 - diff corr data follows
	bool isKeyFrame = mode == KeyFrameBytes;
	bool noKeyFrameUsed = mode == Normal;
	bool isDiffCorrFrame = mode == DiffCorrBytes;
	
	//int arrayLength = 1 + 4 + 2 * Width * Height + *bytesCount;

	//unsigned char *imageData = (unsigned char*)malloc(arrayLength);
	
	int signsBytesCnt = *bytesCount;
	
	unsigned int bytesCounter = *bytesCount;
	
	m_PixelArrayBuffer[0] = noKeyFrameUsed ? (unsigned char)0 : (isKeyFrame ? (unsigned char)1 : (unsigned char)2);
	bytesCounter++;

	for (int y = 0; y < Height; ++y)
	{
		for (int x = 0; x < Width; ++x)
		{
			unsigned char lo = (unsigned char)(pixels[x + y * Width] & 0xFF);
			unsigned char hi = (unsigned char)(pixels[x + y * Width] >> 8);

			m_PixelArrayBuffer[bytesCounter] = hi;
			bytesCounter++;
			m_PixelArrayBuffer[bytesCounter] = lo;
			bytesCounter++;
		}
	}

	m_PixelArrayBuffer[bytesCounter] = (unsigned char)(pixelsCRC32 & 0xFF);
	m_PixelArrayBuffer[bytesCounter + 1] = (unsigned char)((pixelsCRC32 >> 8) & 0xFF);
	m_PixelArrayBuffer[bytesCounter + 2] = (unsigned char)((pixelsCRC32 >> 16) & 0xFF);
	m_PixelArrayBuffer[bytesCounter + 3] = (unsigned char)((pixelsCRC32 >> 24) & 0xFF);
	*bytesCount = bytesCounter + 4;

	if (isDiffCorrFrame)
		memcpy(&m_PixelArrayBuffer[1], &m_SignsBuffer[0], signsBytesCnt);
	*/
}


}