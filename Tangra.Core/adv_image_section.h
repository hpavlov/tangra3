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
		ImageByteOrder ByteOrder;
		bool UsesCRC;

	public:
		AdvImageSection(FILE* pFile);
		~AdvImageSection();

		void AddOrUpdateTag(const char* tagName, const char* tagValue);

		int MaxFrameBufferSize();

		AdvImageLayout* GetImageLayoutById(unsigned char layoutId);
		void GetDataFromDataBytes(unsigned char* data, unsigned long* prevFrame, unsigned long* pixels, int sectionDataLength, int startOffset);
};

}

#endif // ADVIMAGESECTION_H