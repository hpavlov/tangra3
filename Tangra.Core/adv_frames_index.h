#ifndef ADVFRAMESINDEX_H
#define ADVFRAMESINDEX_H

#include <vector>
#include <stdio.h>

using namespace std;

namespace AdvLib
{
		
class IndexEntry
{
	public:
		unsigned int ElapsedTime;
		__int64 FrameOffset;
		unsigned int  BytesCount;
};

class AdvFramesIndex {

	private:
		vector<IndexEntry*> m_IndexEntries;
	
	public:
		AdvFramesIndex();
		AdvFramesIndex(FILE* pFile);
		~AdvFramesIndex();

		void AddFrame(unsigned int frameNo, unsigned int elapedTime, __int64 frameOffset, unsigned int  bytesCount);
		
		IndexEntry* GetIndexForFrame(unsigned int frameNo);
};

}

#endif // ADVFRAMESINDEX_H
