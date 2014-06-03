/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#ifndef ADVIMAGESECTION_H
#define ADVIMAGESECTION_H

#include "adv_image_layout.h"
#include <stdio.h>
#include "utils.h"

#include <map>
#include <string>

using namespace std;
using std::string;

namespace AdvLib
{

class AdvImageSection {

	private:
		map<string, string> m_ImageTags;
		map<unsigned char, AdvImageLayout*> m_ImageLayouts;
		
	private:
		unsigned int ComputePixelsCRC32(unsigned short* pixels);
		
	public:
		unsigned int Width;
		unsigned int Height;
		unsigned char DataBpp;
		
	public:

		AdvImageSection(unsigned int width, unsigned int height, unsigned char dataBpp);
		~AdvImageSection();
		AdvImageLayout* AddImageLayout(unsigned char layoutId, const char* layoutType, const char* compression, unsigned char bpp, int keyFrame);
		void AddOrUpdateTag(const char* tagName, const char* tagValue);
		void WriteHeader(FILE* pfile);
		unsigned char* GetDataBytes(unsigned char layoutId, unsigned short* currFramePixels, unsigned int *bytesCount, char* byteMode, unsigned char pixelsBpp);		
		void BeginFrame();
		int MaxFrameBufferSize();
		AdvImageLayout* GetImageLayoutById(unsigned char layoutId);
};

}

#endif // ADVIMAGESECTION_H