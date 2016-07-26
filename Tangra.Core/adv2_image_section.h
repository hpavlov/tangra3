/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#ifndef ADV_IMAGE_SECTION2_H
#define ADV_IMAGE_SECTION2_H

#include <stdio.h>
#include "adv2_image_layout.h"
#include "utils.h"

#include <map>
#include <string>

using namespace std;
using std::string;

namespace AdvLib2
{
	class Adv2ImageLayout;

	class Adv2ImageSection {

	private:
		map<string, string> m_ImageTags;
		map<unsigned char, Adv2ImageLayout*> m_ImageLayouts;
		bool m_RGBorBGR;
		bool m_SectionDefinitionMode;

	public:
		unsigned int Width;
		unsigned int Height;
		unsigned char DataBpp;
		
		enum ImageByteOrder ByteOrder;
		bool UsesCRC;
		int MaxPixelValue;
		bool IsColourImage;
		char ImageBayerPattern[128];

	public:

		Adv2ImageSection(unsigned int width, unsigned int height, unsigned char dataBpp);
		Adv2ImageSection(FILE* pfile, AdvFileInfo* fileInfo);
		~Adv2ImageSection();

		void WriteHeader(FILE* pfile);
		ADVRESULT BeginFrame();

		unsigned char* GetDataBytes(unsigned char layoutId, unsigned short* currFramePixels, unsigned int *bytesCount, unsigned char pixelsBpp, enum GetByteOperation operation);
		ADVRESULT GetImageLayoutById(unsigned char layoutId, AdvLib2::Adv2ImageLayout** layout);
		ADVRESULT AddOrUpdateTag(const char* tagName, const char* tagValue);
		ADVRESULT AddImageLayout(unsigned char layoutId, const char* layoutType, const char* compression, unsigned char layoutBpp);

		int MaxFrameBufferSize();

		void GetDataFromDataBytes(unsigned char* data, unsigned int* pixels, int sectionDataLength, int startOffset);

		ADVRESULT GetImageLayoutInfo(int layoutIndex, AdvLib2::AdvImageLayoutInfo* imageLayoutInfo);
		ADVRESULT GetImageSectionTagSizes(int tagId, int* tagNameSize, int* tagValueSize);
		ADVRESULT GetImageLayoutTagSizes(int layoutId, int tagId, int* tagNameSize, int* tagValueSize);
		ADVRESULT GetImageSectionTag(int tagId, char* tagName, char* tagValue);
		ADVRESULT GetImageLayoutTag(int layoutId, int tagId, char* tagName, char* tagValue);
	};
}

#endif //ADV_IMAGE_SECTION2_H