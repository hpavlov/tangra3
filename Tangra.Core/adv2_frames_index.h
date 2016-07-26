/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#ifndef ADV_FRAME_INDEX2_H
#define ADV_FRAME_INDEX2_H

#include <vector>
#include <stdio.h>

using namespace std;

namespace AdvLib2
{

	class Index2Entry
	{
		public:
			__int64 ElapsedTicks;
			__int64 FrameOffset;
			unsigned int  BytesCount;
	};

	class Adv2FramesIndex {

	private:
		vector<Index2Entry*>* m_MainIndexEntries;
		vector<Index2Entry*>* m_CalibrationIndexEntries;

		public:
			Adv2FramesIndex();
			Adv2FramesIndex(FILE* pFile);
			~Adv2FramesIndex();

			void WriteIndex(FILE *file);
			void AddFrame(unsigned char streamId, unsigned int frameNo, __int64 elapsedTicks, __int64 frameOffset, unsigned int  bytesCount);

			Index2Entry* GetIndexForFrame(unsigned char streamId, unsigned int frameNo);
			unsigned int GetFramesCount(unsigned char streamId);
	};
}

#endif //ADV_FRAME_INDEX2_H