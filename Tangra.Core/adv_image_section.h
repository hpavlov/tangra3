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
#include "cross_platform.h"

using namespace std;
using std::string;

namespace AdvLib
{

class AdvImageLayout;

class AdvImageSection {

	private:
		map<const char*, const char*> m_ImageTags;
		map<unsigned char, AdvImageLayout*> m_ImageLayouts;		
		
	public:
		unsigned int Width;
		unsigned int Height;
		unsigned char DataBpp;
		unsigned char DynaBits;
		unsigned int NormalisationValue;
		ImageByteOrder ByteOrder;
		bool UsesCRC;

	public:
		AdvImageSection(FILE* pFile);
		~AdvImageSection();

		void AddOrUpdateTag(const char* tagName, const char* tagValue);

		int MaxFrameBufferSize();

		AdvImageLayout* GetImageLayoutById(unsigned char layoutId);
		void GetDataFromDataBytes(unsigned char* data, unsigned int* prevFrame, unsigned int* pixels, int sectionDataLength, int startOffset);
};

}

#endif // ADVIMAGESECTION_H