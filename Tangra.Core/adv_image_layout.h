#ifndef ADV_IMAGE_LAYOUT
#define ADV_IMAGE_LAYOUT

#include <stdio.h>
#include "utils.h"
#include "quicklz.h"

#include <map>
#include <string>

#include "adv_image_section.h"
#include "cross_platform.h"

using namespace std;
using std::string;

namespace AdvLib
{
	class AdvImageSection;

	class AdvImageLayout 
	{

	private:
		AdvImageSection* m_ImageSection;
		unsigned char *SIGNS_MASK;
		map<const char*, const char*> m_LayoutTags;			
		ImageBytesLayout m_BytesLayout;
		
		int m_KeyFrameBytesCount;
		unsigned short *m_PrevFramePixels;
		unsigned short *m_PrevFramePixelsTemp;
		unsigned char *m_PixelArrayBuffer;
		unsigned char *m_SignsBuffer;
		unsigned int m_MaxSignsBytesCount;
		unsigned int m_MaxPixelArrayLengthWithoutSigns;
		char* m_DecompressedPixels;
		qlz_state_decompress* m_StateDecompress;
		bool m_UsesCompression;

		
	private:
		unsigned int ComputePixelsCRC32(unsigned short* pixels);
		
	public:
		unsigned char LayoutId;
		unsigned int Width;
		unsigned int Height;		
		unsigned char Bpp;
		unsigned char DataBpp;
	
		const char* Compression;
		bool IsDiffCorrLayout;
		int KeyFrame;
		
		int MaxFrameBufferSize;
		enum DiffCorrBaseFrame BaseFrameType;
	
	private:
		void GetPixelsFrom12BitByteArray(unsigned char* layoutData, unsigned long* prevFrame, unsigned long* pixelsOut, enum GetByteMode mode, int* readIndex, bool* crcOkay);
		void GetPixelsFrom16BitByteArrayRawLayout(unsigned char* layoutData, unsigned long* prevFrame, unsigned long* pixelsOut, int* readIndex, bool* crcOkay);
		void GetPixelsFrom16BitByteArrayDiffCorrLayout(unsigned char* layoutData, unsigned long* prevFrame, unsigned long* pixelsOut, int* readIndex, bool* crcOkay);

		void ResetBuffers();
		void InitialiseBuffers();
		
	public:
		AdvImageLayout(AdvImageSection* imageSection, unsigned char layoutId, unsigned int width, unsigned int height, unsigned char dataBpp, FILE* pFile);
		~AdvImageLayout();
		
		void AddOrUpdateTag(const char* tagName, const char* tagValue);
		unsigned char* GetDataBytes(unsigned short* currFramePixels, enum GetByteMode mode, unsigned int *bytesCount, unsigned char dataPixelsBpp);
		void StartNewDiffCorrSequence();

		void GetDataFromDataBytes(enum GetByteMode mode, unsigned char* data, unsigned long* prevFrame, unsigned long* pixels, int sectionDataLength, int startOffset);
	};

};
#endif


