#include "adv_image_layout.h"
#include "utils.h"
#include "stdlib.h"
#include "math.h"
#include <stdio.h>
#include <assert.h>

namespace AdvLib
{
	
void AdvImageLayout::InitialiseBuffers()
{
	SIGNS_MASK = new unsigned char(8);
	SIGNS_MASK[0] = 0x01;
	SIGNS_MASK[1] = 0x02;
	SIGNS_MASK[2] = 0x04;
	SIGNS_MASK[3] = 0x08;
	SIGNS_MASK[4] = 0x10;
	SIGNS_MASK[5] = 0x20;
	SIGNS_MASK[6] = 0x40;
	SIGNS_MASK[7] = 0x80;
	
	
	int signsBytesCount = (unsigned int)ceil(Width * Height / 8.0);
	
	if (Bpp == 12)
	{
		MaxFrameBufferSize	= (Width * Height * 3 / 2) + 1 + 4 + 2 * ((Width * Height) % 2) + signsBytesCount + 16;
	}
	else if (Bpp == 16)
	{
		MaxFrameBufferSize = (Width * Height * 2) + 1 + 4 + signsBytesCount + 16;
	}
	else 
		MaxFrameBufferSize = Width * Height * 4 + 1 + 4 + 2 * signsBytesCount + 16;

	
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
	m_SignsBuffer = NULL;
	m_DecompressedPixels = NULL;	
	m_StateDecompress = NULL;
	
	m_PixelArrayBuffer = (unsigned char*)malloc(m_MaxPixelArrayLengthWithoutSigns + m_MaxSignsBytesCount);
	m_PrevFramePixels = (unsigned short*)malloc(m_KeyFrameBytesCount);		
	memset(m_PrevFramePixels, 0, m_KeyFrameBytesCount);
	
	m_PrevFramePixelsTemp = (unsigned short*)malloc(m_KeyFrameBytesCount);	
	m_SignsBuffer = (unsigned char*)malloc(m_MaxSignsBytesCount);		
	m_DecompressedPixels = (char*)malloc(MaxFrameBufferSize);
	
	m_StateDecompress = (qlz_state_decompress *)malloc(sizeof(qlz_state_decompress));
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

	if (NULL != m_SignsBuffer)
		delete m_SignsBuffer;

	if (NULL != m_DecompressedPixels)
		delete m_DecompressedPixels;
	
	if (NULL != m_StateDecompress)
		delete m_StateDecompress;	
	
	m_PrevFramePixels = NULL;
	m_PrevFramePixelsTemp = NULL;
	m_PixelArrayBuffer = NULL;
	m_SignsBuffer = NULL;
	m_DecompressedPixels = NULL;	
	m_StateDecompress = NULL;
}


void AdvImageLayout::StartNewDiffCorrSequence()
{
   //TODO: Reset the prev frame buffer (do we need to do anything??)
}

void AdvImageLayout::AddOrUpdateTag(const char* tagName, const char* tagValue)
{
	map<const char*, const char*>::iterator curr = m_LayoutTags.begin();
	while (curr != m_LayoutTags.end()) 
	{
		const char* existingTagName = curr->first;	
		
		if (0 == strcmp(existingTagName, tagName))
		{
			m_LayoutTags.erase(curr);
			break;
		}
		
		curr++;
	}
	
	m_LayoutTags.insert(make_pair(tagName, tagValue));

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

	if (0 == strcmp("SECTION-DATA-COMPRESSION", tagName))
	{
		if (Compression == NULL) delete Compression;

		Compression = new char[strlen(tagValue) + 1];
		strcpy(const_cast<char*>(Compression), tagValue);

		m_UsesCompression = 0 != strcmp(tagValue, "UNCOMPRESSED");
	}
}

AdvImageLayout::AdvImageLayout(AdvImageSection* imageSection, unsigned char layoutId, unsigned int width, unsigned int height, unsigned char dataBpp, FILE* pFile)
{
	m_ImageSection = imageSection;
	LayoutId = layoutId;
	Width = width;
	Height = height;
	Compression = NULL;
	DataBpp = dataBpp;


	unsigned char version;
	fread(&version, 1, 1, pFile);

	if (version >= 1)
	{
		fread(&Bpp, 1, 1, pFile);	

		unsigned char numTags;
		fread(&numTags, 1, 1, pFile);
		
		m_LayoutTags.clear();
		
		for(int i = 0; i < numTags; i++)
		{
			char* tagName = ReadString(pFile);
			char* tagValue = ReadString(pFile);

			AddOrUpdateTag(tagName, tagValue);

			delete tagName;
			delete tagValue;
		}
	}

	InitialiseBuffers();
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


void AdvImageLayout::GetDataFromDataBytes(enum GetByteMode mode, unsigned char* data, unsigned long* prevFrame, unsigned long* pixels, int sectionDataLength, int startOffset)
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

	bool crcOkay;
	int readIndex = 0;

	if (Bpp == 12)
	{
		GetPixelsFrom12BitByteArray(layoutData, prevFrame, pixels, mode, &readIndex, &crcOkay);
	}
	else if (Bpp == 16)
	{
		if (IsDiffCorrLayout)
			GetPixelsFrom16BitByteArrayDiffCorrLayout(layoutData, prevFrame, pixels, &readIndex, &crcOkay);
		else
			GetPixelsFrom16BitByteArrayRawLayout(layoutData, prevFrame, pixels, &readIndex, &crcOkay);
	}

	//return new AdvImageData()
	//{
	//	ImageData = imageData,
	//	CRCOkay = crcOkay,
	//	Bpp = BitsPerPixel
	//};
}

void AdvImageLayout::GetPixelsFrom16BitByteArrayRawLayout(unsigned char* layoutData, unsigned long* prevFrame, unsigned long* pixelsOut, int* readIndex, bool* crcOkay)
{
	if (DataBpp == 12)
	{		
		unsigned long* pPixelsOut = pixelsOut;
		bool isLittleEndian = m_ImageSection->ByteOrder == LittleEndian;

		for (int y = 0; y < Height; ++y)
		{
			for (int x = 0; x < Width; ++x)
			{
				unsigned char bt1 = *layoutData;
				layoutData++;
				unsigned char bt2 = *layoutData;
				layoutData++;

				unsigned short val = isLittleEndian 
						? (unsigned short)(((unsigned short)bt2 << 8) + bt1)
						: (unsigned short)(((unsigned short)bt1 << 8) + bt2);
				
				val = (unsigned short)(val >> 4);

				*pPixelsOut = val;
				pPixelsOut++;
			}
		}

		*readIndex += Height * Width * 2;
	}
	else if (DataBpp == 16)
	{
		// TODO: Direct copy from the buffer
	}

	if (m_ImageSection->UsesCRC)
	{
		unsigned int savedFrameCrc = (unsigned int)(*layoutData + (*(layoutData + 1) << 8) + (*(layoutData + 2) << 16) + (*(layoutData + 3) << 24));
		*readIndex += 4;

		// TODO: Convert the 32bit array to 16bit and pass it for CRC computation
		//unsigned int crc3 = ComputePixelsCRC32(pixelsOut);
		//*crcOkay = crc3 == savedFrameCrc;
	}
	else
		*crcOkay = true;
}


unsigned int AdvImageLayout::ComputePixelsCRC32(unsigned short* pixels)
{
	return compute_crc32((unsigned char*)pixels, 2 * Height * Width);
}

void AdvImageLayout::GetPixelsFrom12BitByteArray(unsigned char* layoutData, unsigned long* prevFrame, unsigned long* pixelsOut, enum GetByteMode mode, int* readIndex, bool* crcOkay)
{
	//var rv = new ushort[Width, Height];

	bool isLittleEndian = m_ImageSection->ByteOrder == LittleEndian;
	bool convertTo12Bit = m_ImageSection->DataBpp == 12;
	bool convertTo16Bit = m_ImageSection->DataBpp == 16;

	//bool isKeyFrame = byteMode == GetByteMode.KeyFrameBytes;
	//bool keyFrameNotUsed = byteMode == GetByteMode.Normal;
	bool isDiffCorrFrame = mode == DiffCorrBytes;

	unsigned long* pPrevFrame = prevFrame;

	int counter = 0;
	for (int y = 0; y < Height; ++y)
	{
		for (int x = 0; x < Width; ++x)
		{
			counter++;
			// Every 2 12-bit values can be encoded in 3 bytes
			// xxxxxxxx|xxxxyyyy|yyyyyyy

			unsigned char bt1;
			unsigned char bt2;
			unsigned short val;

			switch (counter % 2)
			{
				case 1:
					bt1 = *layoutData;
					layoutData++;
					bt2 = *layoutData;

					val = (unsigned short)(((unsigned short)bt1 << 4) + ((bt2 >> 4) & 0x0F));
					if (!isLittleEndian)
					{
						val = (unsigned short)(val << 4);
						val = (unsigned short)((unsigned short)((val & 0xFF) << 8) + (unsigned short)(val >> 8));

						if (convertTo12Bit)
							throw "NotSupportedException";
					}
					else
						if (convertTo16Bit) val = (unsigned short)(val << 4);

					if (isDiffCorrFrame)
					{
						val = (unsigned short)((unsigned short)*pPrevFrame + (unsigned short)val);
						pPrevFrame++;
						if (convertTo12Bit && val > 4095) val -= 4095;
					}

					*pixelsOut = val;
					pixelsOut++;

					if (counter < 10 || counter > Height * Width - 10) 
						printf("%d: %d", counter, val);
					break;

				case 0:
					bt1 = *layoutData;
					layoutData++;
					bt2 = *layoutData;
					layoutData++;

					val = (unsigned short)((((unsigned short)bt1 & 0x0F) << 8) + bt2);
					if (!isLittleEndian)
					{
						val = (unsigned short)(val << 4);
						val = (unsigned short)((unsigned short)((val & 0xFF) << 8) + (unsigned short)(val >> 8));

						if (convertTo12Bit) 
							throw "NotSupportedException";
					}
					else
						if (convertTo16Bit) val = (unsigned short)(val << 4);

					if (isDiffCorrFrame)
					{
						val = (unsigned short)((unsigned short)*pPrevFrame + (unsigned short)val);
						pPrevFrame++;
						if (convertTo12Bit && val > 4095) val -= 4095;
					}

					*pixelsOut = val;
					pixelsOut++;
					if (counter < 10 || counter > Height * Width - 10) 
						printf("%d: %d", counter, val);
					break;
			}
		}
	}

	if (m_ImageSection->UsesCRC)
	{
		unsigned int savedFrameCrc = (unsigned int)(*layoutData + (*(layoutData + 1) << 8) + (*(layoutData + 2) << 16) + (*(layoutData + 3) << 24));
		*readIndex += 4;

		// TODO: Convert the 32bit array to 16bit and pass it for CRC computation
		//unsigned int crc3 = ComputePixelsCRC32(pixelsOut);
		//*crcOkay = crc3 == savedFrameCrc;
	}
	else
		*crcOkay = true;
				
    return;
}

void AdvImageLayout::GetPixelsFrom16BitByteArrayDiffCorrLayout(unsigned char* layoutData, unsigned long* prevFrame, unsigned long* pixelsOut, int* readIndex, bool* crcOkay)
{
    return;
}

}