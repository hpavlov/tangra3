/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#include "stdafx.h"
#include "adv2_image_layout.h"
#include "utils.h"
#include "math.h"
#include <stdlib.h>
#include "adv_profiling.h"

namespace AdvLib2
{

Adv2ImageLayout::Adv2ImageLayout(Adv2ImageSection* imageSection, unsigned int width, unsigned int height, unsigned char layoutId, const char* layoutType, const char* compression, unsigned char layoutBpp)
{
	m_ImageSection = imageSection;
	LayoutId = layoutId;
	Width = width;
	Height = height;
	Compression = nullptr;
	Bpp = layoutBpp;

	m_BytesLayout = FullImageRaw;
	m_UsesCompression = false;
	m_UsesLagarith16Compression = false;

	AddOrUpdateTag("DATA-LAYOUT", layoutType);
	AddOrUpdateTag("SECTION-DATA-COMPRESSION", compression);

	InitialiseBuffers();
	EnsureCompressors();
}

Adv2ImageLayout::Adv2ImageLayout(Adv2ImageSection* imageSection, char layoutId, FILE* pFile)
{
	m_ImageSection = imageSection;
	LayoutId = layoutId;
	Width = imageSection->Width;
	Height = imageSection->Height;

	m_PixelArrayBuffer = nullptr;
	m_CompressedPixels = nullptr;
	m_StateCompress = nullptr;
	m_StateDecompress = nullptr;
	m_Lagarith16Compressor = nullptr;
	
	m_BytesLayout = FullImageRaw;
	m_UsesCompression = false;
	m_UsesLagarith16Compression = false;

	unsigned char version;
	advfread(&version, 1, 1, pFile); /* Version */

	advfread(&Bpp, 1, 1, pFile);

	unsigned char tagsCount;
	advfread(&tagsCount, 1, 1, pFile);

	for (int i = 0; i < tagsCount; i++)
	{
		char* tagName = ReadUTF8String(pFile);
		char* tagValue = ReadUTF8String(pFile);

		AddOrUpdateTag(tagName, tagValue);
	}

	InitialiseBuffers();
	EnsureCompressors();
}

void Adv2ImageLayout::InitialiseBuffers()
{
	if (Bpp == 8)
	{
		MaxFrameBufferSize	= Width * Height + 1 + 4 + 16;
	}
	else if (Bpp == 12)
	{
		MaxFrameBufferSize	= (Width * Height * 3 / 2) + 1 + 4 + 2 * ((Width * Height) % 2) + 16;
	}
	else if (Bpp == 16)
	{
		MaxFrameBufferSize = (Width * Height * 2) + 1 + 4 + 16;
	}
	else 
		MaxFrameBufferSize = Width * Height * 4 + 1 + 4 + 16;

	// In accordance with Lagarith16 specs
	if (m_UsesLagarith16Compression) MaxFrameBufferSize = Width * Height * sizeof(short) + 0x20000;

	m_PixelArrayBuffer = nullptr;
	m_CompressedPixels = nullptr;
	m_DecompressedPixels = nullptr;
	m_StateCompress = nullptr;
	m_StateDecompress = nullptr;
	m_Lagarith16Compressor = nullptr;

	m_PixelArrayBuffer = (unsigned char*)malloc(MaxFrameBufferSize);
	m_CompressedPixels = (char*)malloc(MaxFrameBufferSize);
	m_DecompressedPixels = (char*)malloc(MaxFrameBufferSize);
}

Adv2ImageLayout::~Adv2ImageLayout()
{
	ResetBuffers();

	if (nullptr != Compression)
	{
		delete Compression;
		Compression = nullptr;
	}
}

void Adv2ImageLayout::ResetBuffers()
{
	if (nullptr != m_PixelArrayBuffer)
		delete m_PixelArrayBuffer;

	if (nullptr != m_CompressedPixels)
		delete m_CompressedPixels;
	
	if (nullptr != m_StateCompress)
		delete m_StateCompress;

	if (nullptr != m_StateDecompress)
		delete m_StateDecompress;
	

	if (nullptr != m_Lagarith16Compressor)
		delete m_Lagarith16Compressor;
		
	m_PixelArrayBuffer = nullptr;
	m_CompressedPixels = nullptr;
	m_StateCompress = nullptr;
	m_StateDecompress = nullptr;
	m_StateDecompress = nullptr;
	m_Lagarith16Compressor = nullptr;
}


void Adv2ImageLayout::AddOrUpdateTag(const char* tagName, const char* tagValue)
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
	
	m_LayoutTags.insert(make_pair(string(tagName), string(tagValue == nullptr ? "" : tagValue)));	

	if (0 == strcmp("SECTION-DATA-COMPRESSION", tagName))
	{
		if (Compression == nullptr) delete Compression;

		Compression = new char[strlen(tagValue) + 1];
		strcpy_s(const_cast<char*>(Compression), strlen(tagValue) + 1, tagValue);

		if (strcmp(tagValue, "UNCOMPRESSED") != 0) m_UsesCompression = true;
		if (strcmp(tagValue, "LAGARITH16") == 0) m_UsesLagarith16Compression = true;
	}

	if (0 == strcmp("DATA-LAYOUT", tagName))
	{
		IsFullImageRaw = 0 == strcmp(tagValue, "FULL-IMAGE-RAW");
		Is12BitImagePacked = 0 == strcmp(tagValue, "12BIT-IMAGE-PACKED");
		Is8BitColourImage = 0 == strcmp(tagValue, "8BIT-COLOR-IMAGE");
	}
}

void Adv2ImageLayout::WriteHeader(FILE* pFile)
{
	unsigned char buffChar;
	
	buffChar = 2;
	advfwrite(&buffChar, 1, 1, pFile); /* Version */

	advfwrite(&Bpp, 1, 1, pFile);

	buffChar = (unsigned char)m_LayoutTags.size();
	advfwrite(&buffChar, 1, 1, pFile);
	
	map<string, string>::iterator curr = m_LayoutTags.begin();
	while (curr != m_LayoutTags.end()) 
	{
		char* tagName = const_cast<char*>(curr->first.c_str());	
		WriteUTF8String(pFile, tagName);
		
		char* tagValue = const_cast<char*>(curr->second.c_str());	
		WriteUTF8String(pFile, tagValue);
		
		curr++;
	}
}

void Adv2ImageLayout::EnsureCompressors()
{
	m_StateCompress = (qlz_state_compress *)malloc(sizeof(qlz_state_compress));
	m_StateDecompress = (qlz_state_decompress *)malloc(sizeof(qlz_state_decompress));

	// Lagarith compressor is intended for 16bit images only. In order to use it for a different BPP we 
	// calculate 'adjusted' width of the corresponding 16bit image if passed bytes are grouped into int16 values.
	int widthOf16BitData = Width;
	if (Bpp == 8) widthOf16BitData /= 2;
	else if (Bpp == 12) widthOf16BitData = Width * 3 / 4;
	m_Lagarith16Compressor = new Compressor(widthOf16BitData, Height);
}

unsigned char* Adv2ImageLayout::GetDataBytes(unsigned short* currFramePixels, unsigned int *bytesCount, unsigned char dataPixelsBpp, enum GetByteOperation operation)
{
	unsigned char* bytesToCompress = GetFullImageRawDataBytes(currFramePixels, bytesCount, dataPixelsBpp, operation);
	
	if (0 == strcmp(Compression, "QUICKLZ"))
	{
		unsigned int frameSize = 0;
				
		AdvProfiling_StartFrameCompression();

		// compress and write result 
		size_t len2 = qlz_compress(bytesToCompress, m_CompressedPixels, *bytesCount, m_StateCompress); 		
		
		AdvProfiling_EndFrameCompression();
	
		*bytesCount = (unsigned int)len2;
		return (unsigned char*)(m_CompressedPixels);
	}
	if (0 == strcmp(Compression, "LAGARITH16"))
	{
		*bytesCount = m_Lagarith16Compressor->CompressData((unsigned short*)bytesToCompress, m_CompressedPixels);
		return (unsigned char*)(m_CompressedPixels);
	}
	else if (0 == strcmp(Compression, "UNCOMPRESSED"))
	{
		return bytesToCompress;
	}
			
	return nullptr;
}

unsigned char* Adv2ImageLayout::GetFullImageRawDataBytes(unsigned short* currFramePixels, unsigned int *bytesCount, unsigned char dataPixelsBpp, enum GetByteOperation operation)
{
	unsigned int buffLen = 0;
	if (dataPixelsBpp == 16)
	{
		if (operation == GetByteOperation::ConvertTo12BitPacked)
			// 2 pixels saved in 3 bytes
			GetDataBytesConvertTo12BitPacked(currFramePixels, 0, &buffLen);
		else if (operation == GetByteOperation::ConvertTo8BitBytesLooseHighByte)
			// 16 bit pixels contain 8 bit numbers, convert to 8 bit pixels
			GetDataBytesConvertTo8BitBytesLooseHighByte(currFramePixels, 0, &buffLen);
		else
		{
			buffLen = Width * Height * 2;
			memcpy(&m_PixelArrayBuffer[0], &currFramePixels[0], buffLen);
		}
	}
	else if (dataPixelsBpp == 8)
	{
		buffLen = Width * Height;
		memcpy(&m_PixelArrayBuffer[0], &currFramePixels[0], buffLen);
	}
	else if (dataPixelsBpp == 12)
	{
		// NOTE: Data will come in already packed bytes
		buffLen = Width * Height * 3 / 2;
		memcpy(&m_PixelArrayBuffer[0], &currFramePixels[0], buffLen);
	}
	
	*bytesCount = buffLen;
	return m_PixelArrayBuffer;
}

void Adv2ImageLayout::GetDataBytesConvertTo12BitPacked(unsigned short* pixels, unsigned int pixelsCRC32, unsigned int *bytesCount)
{	
	unsigned int bytesCounter = *bytesCount;
	unsigned short* pPixels = pixels;
	
	int pixel2GroupCount = Height * Width / 2;
	for (int idx = 0; idx < pixel2GroupCount; ++idx)
	{
		unsigned short p1 = *pPixels;pPixels++;
		unsigned short p2 = *pPixels;pPixels++;

		m_PixelArrayBuffer[3 * idx] = (p1 >> 4) & 0xFF;
		m_PixelArrayBuffer[3 * idx + 1] = ((p1 << 4) & 0xF0) + ((p2 >> 8) & 0x0F);
		m_PixelArrayBuffer[3 * idx + 2] = p2 & 0xFF;
		bytesCounter += 3;
	};

	m_PixelArrayBuffer[bytesCounter] = (unsigned char)(pixelsCRC32 & 0xFF);
	m_PixelArrayBuffer[bytesCounter + 1] = (unsigned char)((pixelsCRC32 >> 8) & 0xFF);
	m_PixelArrayBuffer[bytesCounter + 2] = (unsigned char)((pixelsCRC32 >> 16) & 0xFF);
	m_PixelArrayBuffer[bytesCounter + 3] = (unsigned char)((pixelsCRC32 >> 24) & 0xFF);
	(*bytesCount) = bytesCounter + 4;
}

void Adv2ImageLayout::GetDataBytesConvertTo8BitBytesLooseHighByte(unsigned short* pixels, unsigned int pixelsCRC32, unsigned int *bytesCount)
{
	unsigned int bytesCounter = *bytesCount;
	unsigned short* pPixels = pixels;
	
	int pixelCount = Height * Width;
	for (int idx = 0; idx < pixelCount; ++idx, bytesCounter++)
	{
		unsigned short p = *pPixels;pPixels++;

		m_PixelArrayBuffer[idx] = p & 0xFF;
	};

	m_PixelArrayBuffer[bytesCounter] = (unsigned char)(pixelsCRC32 & 0xFF);
	m_PixelArrayBuffer[bytesCounter + 1] = (unsigned char)((pixelsCRC32 >> 8) & 0xFF);
	m_PixelArrayBuffer[bytesCounter + 2] = (unsigned char)((pixelsCRC32 >> 16) & 0xFF);
	m_PixelArrayBuffer[bytesCounter + 3] = (unsigned char)((pixelsCRC32 >> 24) & 0xFF);
	(*bytesCount) = bytesCounter + 4;
}

void Adv2ImageLayout::GetDataFromDataBytes(unsigned char* data, unsigned int* pixels, int sectionDataLength, int startOffset)
{
	unsigned char* layoutData;
	
	if (!m_UsesCompression)
	{
		layoutData = data + startOffset;
	}
	else if (0 == strcmp(Compression, "QUICKLZ"))
	{		
		size_t size = qlz_size_decompressed((char*)(data + startOffset));
		// MaxFrameBufferSize
		qlz_decompress((char*)(data + startOffset), m_DecompressedPixels, m_StateDecompress);		
		layoutData = (unsigned char*)m_DecompressedPixels;
	}
	else  if (0 == strcmp(Compression, "LAGARITH16"))
	{		
		int size = m_Lagarith16Compressor->DecompressData((char*)(data + startOffset), (unsigned short*)m_DecompressedPixels);
		layoutData = (unsigned char*)m_DecompressedPixels;
	}

	bool crcOkay;
	int readIndex = 0;

	if (Bpp == 12)
	{
		GetPixelsFrom12BitByteArray(layoutData, pixels, &readIndex, &crcOkay);
	}
	else if (Bpp == 16)
	{
		GetPixelsFrom16BitByteArrayRawLayout(layoutData, pixels, &readIndex, &crcOkay);
	}
	else if (Bpp == 8)
	{
		GetPixelsFrom8BitByteArrayRawLayout(layoutData, pixels, &readIndex, &crcOkay);
	}
}

void Adv2ImageLayout::GetPixelsFrom8BitByteArrayRawLayout(unsigned char* layoutData, unsigned int* pixelsOut, int* readIndex, bool* crcOkay)
{
	if (Bpp == 8)
	{		
		unsigned int* pPixelsOut = pixelsOut;
		for (unsigned int y = 0; y < Height; ++y)
		{
			for (unsigned int x = 0; x < Width; ++x)
			{
				unsigned char bt1 = *layoutData;
				layoutData++;
					
				*pPixelsOut = (unsigned short)bt1;
				pPixelsOut++;
			}
		}

		*readIndex += Height * Width;
	}
	
	if (m_ImageSection->UsesCRC)
	{
		unsigned int savedFrameCrc = (unsigned int)(*layoutData + (*(layoutData + 1) << 8) + (*(layoutData + 2) << 16) + (*(layoutData + 3) << 24));
		*readIndex += 4;
	}
	else
		*crcOkay = true;
}

void Adv2ImageLayout::GetPixelsFrom16BitByteArrayRawLayout(unsigned char* layoutData, unsigned int* pixelsOut, int* readIndex, bool* crcOkay)
{
	if (m_ImageSection->DataBpp > 8)
	{		
		unsigned int* pPixelsOut = pixelsOut;
		bool isLittleEndian = m_ImageSection->ByteOrder == LittleEndian;

		for (unsigned int y = 0; y < Height; ++y)
		{
			for (unsigned int x = 0; x < Width; ++x)
			{
				unsigned char bt1 = *layoutData;
				layoutData++;
				unsigned char bt2 = *layoutData;
				layoutData++;

				unsigned short val = isLittleEndian 
						? (unsigned short)(((unsigned short)bt2 << 8) + bt1)
						: (unsigned short)(((unsigned short)bt1 << 8) + bt2);
					
				*pPixelsOut = val;
				pPixelsOut++;
			}
		}

		*readIndex += Height * Width * 2;
	}
	else
	{
		unsigned int* pPixelsOut = pixelsOut;

		for (unsigned int y = 0; y < Height; ++y)
		{
			for (unsigned int x = 0; x < Width; ++x)
			{
				unsigned char bt = *layoutData;
				layoutData++;
					
				*pPixelsOut = (unsigned int)bt;
				pPixelsOut++;
			}
		}

		*readIndex += Height * Width;
	}

	if (m_ImageSection->UsesCRC)
	{
		unsigned int savedFrameCrc = (unsigned int)(*layoutData + (*(layoutData + 1) << 8) + (*(layoutData + 2) << 16) + (*(layoutData + 3) << 24));
		*readIndex += 4;
	}
	else
		*crcOkay = true;
}

void Adv2ImageLayout::GetPixelsFrom12BitByteArray(unsigned char* layoutData, unsigned int* pixelsOut, int* readIndex, bool* crcOkay)
{
	int doubleByteCount = Height * Width / 2;
	for (int counter = 0; counter < doubleByteCount; ++counter)
	{
		// Every 2 12-bit values are be encoded in 3 bytes
		// xxxxxxxx|xxxxyyyy|yyyyyyy

		unsigned char bt1 = *layoutData; layoutData++;
		unsigned char bt2 = *layoutData; layoutData++;
		unsigned char bt3 = *layoutData; layoutData++;

		unsigned short val1 = (unsigned short)(((unsigned short)bt1 << 4) + ((bt2 >> 4) & 0x0F));
		unsigned short val2 = (unsigned short)((((unsigned short)bt2 & 0x0F) << 8) + bt3);

		*pixelsOut = val1; pixelsOut++;
		*pixelsOut = val2; pixelsOut++;
	}

	if (m_ImageSection->UsesCRC)
	{
		unsigned int savedFrameCrc = (unsigned int)(*layoutData + (*(layoutData + 1) << 8) + (*(layoutData + 2) << 16) + (*(layoutData + 3) << 24));
		*readIndex += 4;
	}
	else
		*crcOkay = true;

    return;
}

ADVRESULT Adv2ImageLayout::GetImageLayoutInfo(AdvLib2::AdvImageLayoutInfo* imageLayoutInfo)
{
	imageLayoutInfo->ImageLayoutId = LayoutId;
	imageLayoutInfo->ImageLayoutTagsCount = m_LayoutTags.size();
	imageLayoutInfo->ImageLayoutBpp = Bpp;
	imageLayoutInfo->IsFullImageRaw = IsFullImageRaw;
	imageLayoutInfo->Is12BitImagePacked = Is12BitImagePacked;
	imageLayoutInfo->Is8BitColourImage = Is8BitColourImage;
	return S_OK;
}

ADVRESULT Adv2ImageLayout::GetImageLayoutTagSizes(int tagId, int* tagNameSize, int* tagValueSize)
{
	if (tagId < 0 || tagId >= m_LayoutTags.size())
		return E_FAIL;

	map<string, string>::iterator iter = m_LayoutTags.begin();
	if (tagId > 0) std::advance(iter, tagId);	

	*tagNameSize = (int)iter->first.size();
	*tagValueSize = (int)iter->second.size();

	return S_OK;

}

ADVRESULT Adv2ImageLayout::GetImageLayoutTag(int tagId, char* tagName, char* tagValue)
{
	if (tagId < 0 || tagId >= m_LayoutTags.size())
		return E_FAIL;

	map<string, string>::iterator iter = m_LayoutTags.begin();
	if (tagId > 0) std::advance(iter, tagId);	

	strcpy_s(tagName, iter->first.size() + 1, iter->first.c_str());
	strcpy_s(tagValue, iter->second.size() + 1, iter->second.c_str());

	return S_OK;
}

}