#include "adv_frames_index.h"
#include "stdio.h"

namespace AdvLib
{
	
AdvFramesIndex::AdvFramesIndex()
{

}

AdvFramesIndex::~AdvFramesIndex()
{

}

void AdvFramesIndex::AddFrame(unsigned int frameNo, unsigned int elapedTime, __int64 frameOffset, unsigned int  bytesCount)
{
	IndexEntry *entry = new IndexEntry();
	entry->BytesCount = bytesCount;
	entry->FrameOffset = frameOffset;
	entry->ElapsedTime = elapedTime;
	
	m_IndexEntries.push_back(entry);
}

IndexEntry* AdvFramesIndex::GetIndexForFrame(unsigned int frameNo)
{
	IndexEntry* indexEntryAtPos = m_IndexEntries.at(frameNo);
	return indexEntryAtPos;
}

AdvFramesIndex::AdvFramesIndex(FILE* pFile)
{
	unsigned int framesCount;
	fread(&framesCount, 4, 1, pFile);

	m_IndexEntries.clear();

	unsigned int elapedTime;
	__int64 frameOffset;
	unsigned int bytesCount;

	for(int i = 0; i < framesCount; i++)
	{
		fread(&elapedTime, 4, 1, pFile);
		fread(&frameOffset, 8, 1, pFile);
		fread(&bytesCount, 4, 1, pFile);

		AddFrame(i, elapedTime, frameOffset, bytesCount);
	}
}

}

